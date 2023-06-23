using Newtonsoft.Json;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Forms;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NiceHashMiner.Miners
{
    public class Rigel : Miner
    {
        private int _benchmarkTimeWait = 120;
        private const double DevFee = 2.0;
        protected AlgorithmType SecondaryAlgorithmType = AlgorithmType.NONE;
        private double _power = 0.0d;
        double _powerUsage = 0;
        int addTime = 0;
        int _apiErrors = 0;

        public Rigel() : base("Rigel")
        {
            ConectionType = NhmConectionType.NONE;
        }

        public override void Start(string btcAdress, string worker)
        {
            string url = "";

            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            ProcessHandle = _Start();
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            Helpers.ConsolePrint("Rigel Stop", "");
            DeviceType devtype = DeviceType.NVIDIA;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                devtype = mPair.Device.DeviceType;
            }
            if (Form_Main.ZilMonitorRunning &&
                Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.Rigel, devtype))
            {
                ZilClient.needConnectionZIL = false;
            }
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
            KillRigel();
        }
        private string GetStartCommand(string url, string btcAddress, string worker)
        {
            var algo = "";
            var algo2 = "";
            var algoName = "";
            var algoName2 = "";
            var nicehashstratum = "";
            var ssl = "";
            string port = "";
            string port2 = "";
            string username = GetUsername(btcAddress, worker);
            
            string ZilMining = "";
            string MainMining = "";
            string ZilAlgo = "";
            DeviceType devtype = DeviceType.NVIDIA;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                devtype = mPair.Device.DeviceType;
            }
            
            if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.Rigel, devtype))
            {
                //прокси не используется
                ZilAlgo = "+zil";
                MainMining = "[1]";
                ZilMining = " -o [2]ethstratum+tcp://daggerhashimoto.auto.nicehash.com:9200 -u [2]" + username + " ";
            }
            
            
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                algo = "ethash";
                algoName = "daggerhashimoto";
                nicehashstratum = "";
                port = "3353";
                ZilMining = "";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash)
            {
                algo = "etchash";
                algoName = "etchash";
                nicehashstratum = "";
                port = "3393";
                ZilMining = "";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KHeavyHash)
            {
                algo = "kheavyhash" + ZilAlgo;
                algoName = "kheavyhash";
                nicehashstratum = "";
                port = "3395";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.NexaPow)
            {
                algo = "nexapow" + ZilAlgo;
                algoName = "nexapow";
                nicehashstratum = "";
                port = "3396";
            }



            return GetDevicesCommandString() + nicehashstratum +
                  " -a " + algo +
                  GetServer(algoName, username, port, MainMining) + ZilMining +
                  " --api-bind 127.0.0.1:" + ApiPort;
        }

        
        private string GetServer(string algo, string username, string port, string MainMining = "")
        {
            string ret = "";
            string psw = "x";
            string stratum = "";
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            if (ConfigManager.GeneralConfig.ProxySSL && Globals.MiningLocation.Length > 1)
            {
                port = "4" + port;
                stratum = "stratum+ssl://";
            }
            else
            {
                port = "1" + port;
                stratum = "stratum+tcp://";
            }
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";

            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + " -o " + MainMining + stratum + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 -u " + MainMining +
                        username + " -p " + psw;
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + " -o " + MainMining + stratum + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " -u " + MainMining +
                        username + " -p " + psw + " ";
                }
            }
            return ret;
        }
        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " --no-watchdog -d ";
            var ids = new List<string>();
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            var extra = "";
            int id;
            foreach (var mPair in sortedMinerPairs)
            {
                id = mPair.Device.IDByBus;

                if (mPair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    extra = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA);
                }
                else
                {
                    extra = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
                }

                {
                    ids.Add(id.ToString());
                }

            }

            deviceStringCommand += string.Join(",", ids);
            deviceStringCommand = deviceStringCommand + extra + " ";

            return deviceStringCommand;
        }

        protected void KillMinerBase(string exeName)
        {
            foreach (var process in Process.GetProcessesByName(exeName))
            {
                try { process.Kill(); }
                catch (Exception e) { Helpers.ConsolePrint(MinerDeviceName, e.ToString()); }
            }
        }
        private static void KillProcessAndChildren(int pid)
        {
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }

            try
            {
                var RigelHandle = new Process
                {
                    StartInfo =
                {
                    FileName = "taskkill.exe"
                }
                };
                RigelHandle.StartInfo.Arguments = "/PID " + pid.ToString() + " /F /T";
                RigelHandle.StartInfo.UseShellExecute = false;
                RigelHandle.StartInfo.CreateNoWindow = true;
                RigelHandle.Start();
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("KillProcessAndChildren", ex.ToString());
            }
            
            Thread.Sleep(100);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            /*
            Thread.Sleep(100);
            try
            {
                Process proc = Process.GetProcessById(pid);
                if (proc != new Process()) proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
            */
        }
        public override void EndBenchmarkProcces()
        {
            if (BenchmarkProcessStatus != BenchmarkProcessStatus.Killing && BenchmarkProcessStatus != BenchmarkProcessStatus.DoneKilling)
            {
                BenchmarkProcessStatus = BenchmarkProcessStatus.Killing;
                try
                {
                    Helpers.ConsolePrint("BENCHMARK",
                        $"Trying to kill benchmark process {ProcessTag()} algorithm {BenchmarkAlgorithm.AlgorithmName}");

                    int k = ProcessTag().IndexOf("pid(");
                    int i = ProcessTag().IndexOf(")|bin");
                    var cpid = ProcessTag().Substring(k + 4, i - k - 4).Trim();

                    int pid = int.Parse(cpid, CultureInfo.InvariantCulture);
                    Helpers.ConsolePrint("BENCHMARK", "Rigel.exe PID: " + pid.ToString());
                    KillProcessAndChildren(pid);
                    BenchmarkHandle.Kill();
                    BenchmarkHandle.Close();
                    if (IsKillAllUsedMinerProcs) KillAllUsedMinerProcesses();
                }
                catch { }
                finally
                {
                    BenchmarkProcessStatus = BenchmarkProcessStatus.DoneKilling;
                    Helpers.ConsolePrint("BENCHMARK",
                        $"Benchmark process {BenchmarkProcessPath} algorithm {BenchmarkAlgorithm.AlgorithmName} KILLED");
                    //BenchmarkHandle = null;
                }
            }
        }
        public void KillRigel()
        {
            try
            {
                int k = ProcessTag().IndexOf("pid(");
                int i = ProcessTag().IndexOf(")|bin");
                var cpid = ProcessTag().Substring(k + 4, i - k - 4).Trim();

                int pid = int.Parse(cpid, CultureInfo.InvariantCulture);
                Helpers.ConsolePrint("Rigel", "kill Rigel.exe PID: " + pid.ToString());
                KillProcessAndChildren(pid);
                ProcessHandle.Kill();
                ProcessHandle.Close();
            }
            catch { }
            //if (IsKillAllUsedMinerProcs) KillAllUsedMinerProcesses();

        }
        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.NvidiaLHR && MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
                {
                    //addTime = 60;
                }
            }

            _benchmarkTimeWait = time + addTime;
            var ret = "";

            var btcAddress = Globals.GetBitcoinUser();
            var worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            string username = GetUsername(btcAddress, worker);

            
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                ret = " -a ethash" +
                " -o " + Links.CheckDNS("stratum+tcp://ethw.2miners.com:2020") + " -u 0x266b27bd794d1A65ab76842ED85B067B415CD505.Rigel -p x" +
                " -o " + Links.CheckDNS("stratum+tcp://daggerhashimoto.auto.nicehash.com:9200").Replace("stratum+tcp://", "") + " -u " + Globals.DemoUser + " -p x" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash)
            {
                ret = " -a etchash" +
                " -o " + Links.CheckDNS("stratum+tcp://etc.2miners.com:1010") + " -uer 0x266b27bd794d1A65ab76842ED85B067B415CD505.Rigel -p x" +
                " -o " + Links.CheckDNS("stratum+tcp://etchash.auto.nicehash.com:9200").Replace("stratum+tcp://", "") + " -u " + Globals.DemoUser + " -p x" +
                GetDevicesCommandString();
            }
            
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KHeavyHash)
            {
                ret = " -a kheavyhash" +
                " -o " + Links.CheckDNS("pool.eu.woolypooly.com:3112") + " -u kaspa:qq9y94k2xqumnsgvx6huxn3uugzy8euzxjh9utxe338ck0ufch0hkvvd37vc0 -p x" +
                GetDevicesCommandString();
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.NexaPow)
            {
                ret = " -a nexapow" +
                " -o " + Links.CheckDNS("stratum-eu.rplant.xyz:7092") + " -u nexa:nqtsq5g55l2jhuazhre8zfzfnyxle543wjlapt4huup3x9gy.Rigel -p x" +
                GetDevicesCommandString();
            }
            

            return ret + " --api-bind 127.0.0.1:" + ApiPort; 
        }
        protected override bool BenchmarkParseLine(string outdata)
        {
            return true;
        }
        protected override void BenchmarkThreadRoutine(object commandLine)
        {
            BenchmarkSignalQuit = false;
            BenchmarkSignalHanged = false;
            BenchmarkSignalFinnished = false;
            BenchmarkException = null;
            double repeats = 0.0d;
            double summspeed = 0.0d;
            double secsummspeed = 0.0d;

            int delay_before_calc_hashrate = 15;
            int MinerStartDelay = 10;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                Helpers.ConsolePrint(MinerTag(), "Benchmark should end in : " + _benchmarkTimeWait + " seconds");
                BenchmarkHandle = BenchmarkStartProcess((string)commandLine);
                var benchmarkTimer = new Stopwatch();
                benchmarkTimer.Reset();
                benchmarkTimer.Start();

                BenchmarkProcessStatus = BenchmarkProcessStatus.Running;
                BenchmarkThreadRoutineStartSettup(); //need for benchmark log
                while (IsActiveProcess(BenchmarkHandle.Id))
                {
                    if (benchmarkTimer.Elapsed.TotalSeconds >= (_benchmarkTimeWait + 60)
                        || BenchmarkSignalQuit
                        || BenchmarkSignalFinnished
                        || BenchmarkSignalHanged
                        || BenchmarkSignalTimedout
                        || BenchmarkException != null)
                    {
                        break;
                    }
                    // wait a second due api request
                    Thread.Sleep(1000);

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KHeavyHash))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 15;
                    }
                    
                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        _powerUsage += _power;
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
                        BenchmarkAlgorithm.BenchmarkProgressPercent = (int)(benchProgress * 100);
                        if (repeats > delay_before_calc_hashrate)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString() + " Dual: " + ad.Result.SecondarySpeed.ToString() + " power: " + _power.ToString());
                            summspeed += ad.Result.Speed;
                            secsummspeed += ad.Result.SecondarySpeed;
                        }
                        else
                        {
                            Helpers.ConsolePrint(MinerTag(), "Delayed API Speed: " + ad.Result.Speed.ToString());
                        }

                        if (repeats >= _benchmarkTimeWait - MinerStartDelay - 15)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Benchmark ended");
                            ad.Dispose();
                            benchmarkTimer.Stop();

                            try
                            {
                                KillProcessAndChildren(BenchmarkHandle.Id);
                                BenchmarkHandle.Dispose();
                                EndBenchmarkProcces();
                            } catch (Exception ex)
                            {
                                Helpers.ConsolePrint("**", ex.ToString());
                            }

                            break;
                        }

                    }
                }

                BenchmarkAlgorithm.BenchmarkSpeed = Math.Round(summspeed / (repeats - delay_before_calc_hashrate), 2);
                BenchmarkAlgorithm.BenchmarkSecondarySpeed = Math.Round(secsummspeed / (repeats - delay_before_calc_hashrate), 2);
                BenchmarkAlgorithm.PowerUsageBenchmark = (_powerUsage / repeats);
            }
            catch (Exception ex)
            {
                BenchmarkThreadRoutineCatch(ex);
            }
            finally
            {

                BenchmarkThreadRoutineFinish();
            }
        }

        // stub benchmarks read from file
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }

        
        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5;  // 5 min
        }
        protected override bool IsApiEof(byte third, byte second, byte last)
        {
            return third == 0x7d && second == 0xa && last == 0x7d;
        }

        private ApiData ad;
        public override ApiData GetApiData()
        {
            return ad;
        }
        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
            ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);
            ad.ThirdAlgorithmID = AlgorithmType.NONE;

            string ResponseFromRigel;
            double total = 0;
            double total2 = 0;
            double total3 = 0;
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString() + "/stat");
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 3 * 1000;
                WR.ReadWriteTimeout = 3 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 2 * 1000;
                StreamReader Reader = new StreamReader(SS);
                Reader.BaseStream.ReadTimeout = 3 * 1000;
                ResponseFromRigel = await Reader.ReadToEndAsync();
                //Helpers.ConsolePrint("->", ResponseFromRigel);
                if (ResponseFromRigel.Length == 0 || (ResponseFromRigel[0] != '{' && ResponseFromRigel[0] != '['))
                    throw new Exception("Not JSON!");
                Reader.Close();
                Response.Close();
            }
            catch (Exception)
            {
                _apiErrors++;
                Helpers.ConsolePrint("GetSummaryAsync", "Rigel-API ERRORs count: " + _apiErrors.ToString());
                if (_apiErrors > 60)
                {
                    Helpers.ConsolePrint("GetSummaryAsync", "RESTART Rigel");
                    Restart();
                }
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                ad.Speed = 0;
                ad.SecondarySpeed = 0;
                ad.ThirdSpeed = 0;
                return ad;
            }
            //return null;
            ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);
            ad.ThirdAlgorithmID = AlgorithmType.NONE;

            ResponseFromRigel = ResponseFromRigel.Replace("-nan", "0.00");
            ResponseFromRigel = ResponseFromRigel.Replace("(ind)", "");
            //Helpers.ConsolePrint("->", ResponseFromRigel);
            string _miner = "";
            try
            {
                dynamic resp = JsonConvert.DeserializeObject(ResponseFromRigel);
                if (resp != null)
                {
                    var devices = resp.devices;

                    double[] hashrates = new double[devices.Count];
                    double[] hashrates2 = new double[devices.Count];
                    double[] hashrates3 = new double[devices.Count];
                    int i = 0;
                    foreach (var d in resp.devices)
                    {
                        int id = d.id;
                        string name = d.name;
                        bool selected = d.selected;
                        dynamic _hashrate = null;
                        if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KHeavyHash)
                        {
                            _hashrate = d.hashrate.kheavyhash;
                        }
                        if (MiningSetup.CurrentAlgorithmType == AlgorithmType.NexaPow)
                        {
                            _hashrate = d.hashrate.nexapow;
                        }

                        var _hashrate2 = d.hashrate.zil;
                        double hashrate = 0.0d;
                        double hashrate2 = 0.0d;
                        if (_hashrate == null)
                        {
                            hashrate = 0.0d;
                        } else
                        {
                            if (selected)
                            {
                                hashrate = (double)_hashrate;
                            }
                        }
                        if (_hashrate2 == null)
                        {
                            hashrate2 = 0.0d;
                        }
                        else
                        {
                            if (selected)
                            {
                                hashrate2 = (double)_hashrate2;
                            }
                        }

                        total = total + hashrate;
                        total2 = total2 + hashrate2;
                        //total3 = total3 + resp.devices[i].speed3;
                        hashrates[i] = hashrate;
                        hashrates2[i] = hashrate2;

                        //Helpers.ConsolePrint("****", id.ToString() + " " + name + " " +selected.ToString() + " " + (hashrate.kheavyhash).ToString());
                        i++;
                    }
                    //int dev = 0;
                    var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
                    if (Form_Main.NVIDIA_orderBug)
                    {
                        sortedMinerPairs.Sort((a, b) => a.Device.ID.CompareTo(b.Device.ID));
                    }
                    
                    foreach (var mPair in sortedMinerPairs)
                    {
                        _power = mPair.Device.PowerUsage;
                        mPair.Device.MiningHashrate = hashrates[mPair.Device.ID];
                        mPair.Device.MiningHashrateSecond = hashrates2[mPair.Device.ID];
                        mPair.Device.MiningHashrateThird = hashrates3[mPair.Device.ID];
                        //duals
                        if ((MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto ||
                            MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash) &&
                            MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KHeavyHash)
                        {
                            mPair.Device.MiningHashrate = hashrates[mPair.Device.ID];
                            mPair.Device.MiningHashrateSecond = hashrates2[mPair.Device.ID];
                            mPair.Device.MiningHashrateThird = 0;
                            mPair.Device.AlgorithmID = (int)MiningSetup.CurrentAlgorithmType;
                            mPair.Device.SecondAlgorithmID = (int)MiningSetup.CurrentSecondaryAlgorithmType;
                            mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                        }

                        if ((MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto ||
                            MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash) &&
                            MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.NexaPow)
                        {
                            mPair.Device.MiningHashrate = hashrates[mPair.Device.ID];
                            mPair.Device.MiningHashrateSecond = hashrates2[mPair.Device.ID];
                            mPair.Device.MiningHashrateThird = 0;
                            mPair.Device.AlgorithmID = (int)MiningSetup.CurrentAlgorithmType;
                            mPair.Device.SecondAlgorithmID = (int)MiningSetup.CurrentSecondaryAlgorithmType;
                            mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                        }

                        if (MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.NONE)//single
                        {
                            if (Form_Main.isZilRound)
                            {
                                mPair.Device.MiningHashrate = 0;
                                mPair.Device.AlgorithmID = (int)AlgorithmType.NONE;
                                mPair.Device.SecondAlgorithmID = (int)AlgorithmType.DaggerHashimoto;
                                mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                            }
                            else
                            {
                                mPair.Device.MiningHashrateSecond = 0;
                                mPair.Device.MiningHashrateThird = 0;
                                mPair.Device.AlgorithmID = (int)MiningSetup.CurrentAlgorithmType;
                                mPair.Device.SecondAlgorithmID = (int)MiningSetup.CurrentSecondaryAlgorithmType;
                                mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                            }
                        }
                        //dev++;
                    }
                    
                }
                else
                {
                    Helpers.ConsolePrint("Rigel:", "resp - null");
                }
                _apiErrors = 0;
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("Rigel API:", ex.ToString());
            }
            finally
            {
                ad.ZilRound = false;
                ad.Speed = total;
                ad.SecondarySpeed = total2;
                ad.ThirdSpeed = total3;

                if (Form_Main.isZilRound)
                {
                    
                    {
                        if (true)//+zil
                        //if (true && total2 > 0)//+zil
                        {
                            ad.Speed = 0;
                            ad.SecondarySpeed = total2;
                            ad.ThirdSpeed = 0;
                            ad.ZilRound = true;
                            ad.AlgorithmID = AlgorithmType.NONE;
                            ad.SecondaryAlgorithmID = AlgorithmType.DaggerHashimoto;
                        }
                    }
                }
                else
                {
                    ad.ZilRound = false;
                    ad.ThirdSpeed = 0;
                    ad.ThirdAlgorithmID = AlgorithmType.NONE;

                    {
                        //if (_algo.ToLower().Contains("zil"))
                        {
                            ad.Speed = total;
                            ad.SecondarySpeed = 0;
                            ad.SecondaryAlgorithmID = AlgorithmType.NONE;
                        }
                    }
                }

                if (ad.Speed == 0 && ad.SecondarySpeed == 0 && ad.ThirdSpeed == 0)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                }
                else
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                    DeviceType devtype = DeviceType.NVIDIA;
                    var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
                    foreach (var mPair in sortedMinerPairs)
                    {
                        devtype = mPair.Device.DeviceType;
                    }
                    if (!Form_Main.ZilMonitorRunning && true &&
                        Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.Rigel, devtype))
                    {
                        ZilClient.needConnectionZIL = true;
                        Form_Main.ZilMonitorRunning = true;
                        ZilClient.StartZilMonitor();
                    }
                }

            }

            Thread.Sleep(100);
            
            return ad;
        }
    }
    public class Hashrate
    {
        public double? kheavyhash { get; set; }
    }
}
