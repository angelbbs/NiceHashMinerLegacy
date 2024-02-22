using Newtonsoft.Json;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NiceHashMiner.Miners
{
    public class trex : Miner
    {
        private int _benchmarkTimeWait = 60;
        private const int TotalDelim = 2;
        private double _power = 0.0d;
        double _powerUsage = 0;
        public FileStream fs;
        private int offset = 0;
        private bool _isDual = false;
        public trex() : base("trex")
        {
        }
        private bool _benchmarkException => MiningSetup.MinerPath == MinerPaths.Data.trex;
        private string GetServer(string algo, string username, string port)
        {
            string ret = "";
            string ssl = "";
            string psw = "x";
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            if (ConfigManager.GeneralConfig.ProxySSL && Globals.MiningLocation.Length > 1)
            {
                port = "4" + port;
                ssl = "stratum2+ssl://";
            }
            else
            {
                port = "1" + port;
                ssl = "stratum2+tcp://";
            }
            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + " -o stratum2+tcp://" + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 -u " + 
                        username + " -p " + psw + " ";
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + " -o " + ssl + Links.CheckDNS("stratum." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " -u " + 
                        username + " -p " + psw + " ";
                }
            }
            return ret;
        }
        public override void Start(string btcAdress, string worker)
        {
            if (!IsInit)
            {
                Helpers.ConsolePrint(MinerTag(), "MiningSetup is not initialized exiting Start()");
                return;
            }
            var username = GetUsername(btcAdress, worker);

            // IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.trex;
            string port = "";
            var algo = "";
            var algo2 = "";
            var apiBind = "";

            apiBind = " --api-bind-http 0.0.0.0:" + ApiPort;
            IsApiReadException = false;

            foreach (var mPair in MiningSetup.MiningPairs)
            {
                if (mPair.Algorithm is DualAlgorithm algoDual)
                {
                    _isDual = true;
                }
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                port = "3353";
                algo = "ethash";
                algo2 = "daggerhashimoto";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
            {
                port = "3390";
                algo = "autolykos2";
                algo2 = "autolykos";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                port = "3385";
                algo = "kawpow";
                algo2 = "kawpow";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus))
            {
                port = "3389";
                algo = "octopus";
                algo2 = "octopus";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
            {
                port = "3393";
                algo = "etchash";
                algo2 = "etchash";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
            {
                port = "3379";
                algo = "x16rv2";
                algo2 = "x16rv2";
            }

            LastCommandLine = "-a " + algo +
            GetServer(algo2, username, port) +
            apiBind +
            " -d " + GetDevicesCommandString() + " --no-watchdog " +
            ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus))
            {
                LastCommandLine = LastCommandLine.Replace("stratum2", "stratum");
            }


            ProcessHandle = _Start();
        }

        // benchmark stuff
        protected void KillMinerBase(string exeName)
        {
            foreach (var process in Process.GetProcessesByName(exeName))
            {
                try
                {
                    Thread.Sleep(1000);
                    process.Kill();
                }
                catch (Exception e) { Helpers.ConsolePrint(MinerDeviceName, e.ToString()); }
            }
        }

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            string configfilename = GetLogFileName();
            int _location = ConfigManager.GeneralConfig.ServiceLocation;
            if (ConfigManager.GeneralConfig.ServiceLocation >= Globals.MiningLocation.Length)
            {
                _location = ConfigManager.GeneralConfig.ServiceLocation - 1;
            }
            string url = Globals.GetLocationUrl(algorithm.NiceHashID, Globals.MiningLocation[_location], this.ConectionType);
            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);
            var username = GetUsername(Globals.DemoUser, ConfigManager.GeneralConfig.WorkerName.Trim());
            var commandLine = "";
            url = url.Replace("stratum+tcp", "stratum2+tcp");

            foreach (var mPair in MiningSetup.MiningPairs)
            {
                if (mPair.Algorithm is DualAlgorithm algoDual)
                {
                    _isDual = true;
                }
            }
            Helpers.ConsolePrint("BENCHMARK", "_isDual: " + _isDual);
            Helpers.ConsolePrint("BENCHMARK", "CurrentAlgorithmType: " + MiningSetup.CurrentAlgorithmType);
            Helpers.ConsolePrint("BENCHMARK", "CurrentSecondaryAlgorithmType: " + MiningSetup.CurrentSecondaryAlgorithmType);
            if (!_isDual)
            {
                

                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
                {
                    commandLine = "--algo kawpow" +
                     " -o " + Links.CheckDNS("stratum+tcp://rvn.2miners.com:6060") + " -u RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX" + " -p x -w trex" +
                     " -o " + Links.CheckDNS(url) + " -u " + username + " -p x " +
                                  ExtraLaunchParametersParser.ParseForMiningSetup(
                                      MiningSetup,
                                      DeviceType.NVIDIA) + " --gpu-report-interval 1 --no-watchdog --api-bind-http 127.0.0.1:" + ApiPort +
                                  " -d ";
                    commandLine += GetDevicesCommandString();
                    _benchmarkTimeWait = time;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                {
                    commandLine = "--algo ethash" +
                     " -o " + Links.CheckDNS("stratum+tcp://ethw.2miners.com:2020") + " -u 0x266b27bd794d1A65ab76842ED85B067B415CD505" + " -p x -w trex" +
                     " -o " + Links.CheckDNS(url) + " -u " + username + " -p x " +
                                  ExtraLaunchParametersParser.ParseForMiningSetup(
                                      MiningSetup,
                                      DeviceType.NVIDIA) + " --gpu-report-interval 1 --no-watchdog --api-bind-http 127.0.0.1:" + ApiPort +
                                  " -d ";
                    commandLine += GetDevicesCommandString();
                    _benchmarkTimeWait = time;
                }
                if(MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
                {
                    commandLine = "--algo etchash" +
                     " -o " + Links.CheckDNS("stratum+tcp://etc.2miners.com:1010") + " -u 0x266b27bd794d1A65ab76842ED85B067B415CD505" + " -p x -w trex" +
                     " -o " + Links.CheckDNS(url) + " -u " + username + " -p x " +
                                  ExtraLaunchParametersParser.ParseForMiningSetup(
                                      MiningSetup,
                                      DeviceType.NVIDIA) + " --gpu-report-interval 1 --no-watchdog --api-bind-http 127.0.0.1:" + ApiPort +
                                  " -d ";
                    commandLine += GetDevicesCommandString();
                    _benchmarkTimeWait = time;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus))
                {
                    commandLine = "--algo octopus" +
                     " -o " + Links.CheckDNS("stratum+tcp://pool.woolypooly.com:3094") + " -u cfx:aakuw91bx9mfhn808n0tczpwt6z1habut6zjrjapsd" + " -p x -w trex" +
                                  ExtraLaunchParametersParser.ParseForMiningSetup(
                                      MiningSetup,
                                      DeviceType.NVIDIA) + " --gpu-report-interval 1 --no-watchdog --api-bind-http 127.0.0.1:" + ApiPort +
                                  " -d ";
                    commandLine += GetDevicesCommandString();
                    _benchmarkTimeWait = time;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
                {
                    commandLine = "--algo autolykos2" +
                     " -o " + Links.CheckDNS("stratum+tcp://pool.woolypooly.com:3100") + " -u 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt" + " -p x -w trex" +
                     " -o " + Links.CheckDNS(url) + " -u " + username + " -p x " +
                                  ExtraLaunchParametersParser.ParseForMiningSetup(
                                      MiningSetup,
                                      DeviceType.NVIDIA) + " --gpu-report-interval 1 --no-watchdog --api-bind-http 127.0.0.1:" + ApiPort +
                                  " -d ";
                    commandLine += GetDevicesCommandString();
                    _benchmarkTimeWait = time;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
                {
                    commandLine = "--algo x16rv2 --benchmark" +
                    " --gpu-report-interval 1 --no-watchdog --api-bind-http 127.0.0.1:" + ApiPort +
                                  " -d ";
                    commandLine += GetDevicesCommandString() + " -l " + GetLogFileName();
                    _benchmarkTimeWait = time;
                }

            }
            /*
            else
            {
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto) &&
                    MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.Autolykos))
                {
                    commandLine = "-a ethash --lhr-algo autolykos2" +
                    " -o " + Links.CheckDNS("stratum+tcp://ethw.2miners.com:2020") + " -u 0x266b27bd794d1A65ab76842ED85B067B415CD505" + " -p x -w trexdual" +
                    " --url2 " + Links.CheckDNS("stratum+tcp://pool.woolypooly.com:3100") + " --user2 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.trexdual --pass2 x" +
                    " --gpu-report-interval 1 --no-watchdog --api-bind-http 127.0.0.1:" + ApiPort +
                    " -d " + GetDevicesCommandString() +
                    ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
                    _benchmarkTimeWait = time;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto) &&
                    MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.KAWPOW))
                {
                    commandLine = "-a ethash --lhr-algo kawpow" +
                    " -o " + Links.CheckDNS("stratum+tcp://ethw.2miners.com:2020") + " -u 0x266b27bd794d1A65ab76842ED85B067B415CD505" + " -p x -w trexdual" +
                    " --url2 " + Links.CheckDNS("stratum+tcp://rvn.2miners.com:6060") + " --user2 RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.trexdual --pass2 x" +
                    " --gpu-report-interval 1 --no-watchdog --api-bind-http 127.0.0.1:" + ApiPort +
                    " -d " + GetDevicesCommandString() +
                    ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
                    _benchmarkTimeWait = time;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto) &&
                    MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.Octopus))
                {
                    commandLine = "-a ethash --lhr-algo octopus" +
                    " -o " + Links.CheckDNS("stratum+tcp://ethw.2miners.com:2020") + " -u 0x266b27bd794d1A65ab76842ED85B067B415CD505" + " -p x -w trexdual" +
                    " --url2 " + Links.CheckDNS("stratum+tcp://pool.woolypooly.com:3094") + " --user2 cfx:aakuw91bx9mfhn808n0tczpwt6z1habut6zjrjapsd.trexdual --pass2 x" +
                    " --gpu-report-interval 1 --no-watchdog --api-bind-http 127.0.0.1:" + ApiPort +
                    " -d " + GetDevicesCommandString() +
                    ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
                    _benchmarkTimeWait = time;
                }
            }
            */
            return commandLine;
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

            int delay_before_calc_hashrate = 10;
            int MinerStartDelay = 10;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
                if (_isDual)
                {
                    _benchmarkTimeWait += 60;
                }
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                Helpers.ConsolePrint(MinerTag(), "Benchmark should end in: " + _benchmarkTimeWait + " seconds");

                Thread.Sleep((int)(MinerID * 50));
                BenchmarkHandle = BenchmarkStartProcess((string)commandLine);

                var benchmarkTimer = new Stopwatch();
                benchmarkTimer.Reset();
                benchmarkTimer.Start();

                BenchmarkProcessStatus = BenchmarkProcessStatus.Running;
                BenchmarkThreadRoutineStartSettup(); //need for benchmark log

                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
                {
                    Thread.Sleep(1000);
                    try
                    {
                        if (File.Exists("miners\\t-rex\\" + GetLogFileName()))
                            File.Delete("miners\\t-rex\\" + GetLogFileName());

                        Thread.Sleep(1000);
                        do
                        {
                            Thread.Sleep(1000);
                        } while (!File.Exists("miners\\t-rex\\" + GetLogFileName()));
                        Thread.Sleep(1000);
                        fs = new FileStream("miners\\t-rex\\" + GetLogFileName(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint(MinerTag(), ex.Message);
                    }
                }


                while (IsActiveProcess(BenchmarkHandle.Id))
                {
                    if (benchmarkTimer.Elapsed.TotalSeconds >= (_benchmarkTimeWait + 60)
                        || BenchmarkSignalQuit
                        || BenchmarkSignalFinnished
                        || BenchmarkSignalHanged
                        || BenchmarkSignalTimedout
                        || BenchmarkException != null)
                    {
                        var imageName = MinerExeName.Replace(".exe", "");
                        // maybe will have to KILL process
                        EndBenchmarkProcces();
                        //  KillMinerBase(imageName);
                        if (BenchmarkSignalTimedout)
                        {
                            throw new Exception("Benchmark timedout");
                        }

                        if (BenchmarkException != null)
                        {
                            throw BenchmarkException;
                        }

                        if (BenchmarkSignalQuit)
                        {
                            throw new Exception("Termined by user request");
                        }

                        if (BenchmarkSignalFinnished)
                        {
                            break;
                        }

                        //keepRunning = false;
                        break;
                    }
                    // wait a second due api request
                    Thread.Sleep(1000);

                    if (_isDual)
                    {
                        delay_before_calc_hashrate = 15;
                        MinerStartDelay = 30;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                    {
                        delay_before_calc_hashrate = 5;
                        MinerStartDelay = 30;
                    }

                    
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 20;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus)) // not tested
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 10;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2)) 
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 10;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
                    {
                        delay_before_calc_hashrate = 5;
                        MinerStartDelay = 30;
                    }

                    var ad = GetSummaryAsync();

                    double logSpeed = 0.0d;
                    if ((MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2)) && fs.Length > offset)
                    {
                        int count = (int)(fs.Length - offset);
                        byte[] array = new byte[count];
                        fs.Read(array, 0, count);
                        offset = (int)fs.Length;
                        string textFromFile = System.Text.Encoding.Default.GetString(array).Trim();
                        //Helpers.ConsolePrint(MinerTag(), textFromFile);

                        string strStart = "Total:";
                        if (textFromFile.Contains(strStart) && textFromFile.Contains("H/s"))
                        {
                            var speedStart = textFromFile.IndexOf(strStart);
                            var speed = textFromFile.Substring(speedStart + strStart.Length, 6);
                            speed = speed.Replace(strStart, "");
                            speed = speed.Replace(" ", "");
                            double.TryParse(speed, out logSpeed);
                            if (textFromFile.Contains("MH/s")) logSpeed = logSpeed * 1000 * 1000;
                            if (textFromFile.Contains("GH/s")) logSpeed = logSpeed * 1000 * 1000 * 1000;
                            Helpers.ConsolePrint("logSpeed", logSpeed.ToString());
                        }
                    }

                    if ((ad.Result != null && ad.Result.Speed > 0) || 
                        MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
                    {
                        _powerUsage += _power;
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
                        BenchmarkAlgorithm.BenchmarkProgressPercent = (int)(benchProgress * 100);
                        if (repeats > delay_before_calc_hashrate)
                        {
                            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
                            {
                                Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + logSpeed.ToString() + " power: " + _power.ToString());
                                summspeed += logSpeed;
                            }
                            else
                            {
                                Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString() + " SecondSpeed: " + ad.Result.SecondarySpeed + " power: " + _power.ToString());
                                summspeed += ad.Result.Speed;
                                secsummspeed += ad.Result.SecondarySpeed;
                            }
                        }
                        else
                        {
                            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
                            {
                                Helpers.ConsolePrint(MinerTag(), "Delayed API Speed: " + logSpeed.ToString());
                            }
                            else
                            {
                                Helpers.ConsolePrint(MinerTag(), "Delayed API Speed: " + ad.Result.Speed.ToString());
                            }
                        }

                        if (repeats >= _benchmarkTimeWait - MinerStartDelay - 15)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Benchmark ended");
                            ad.Dispose();
                            benchmarkTimer.Stop();

                            BenchmarkHandle.Kill();
                            BenchmarkHandle.Dispose();
                            EndBenchmarkProcces();
                            /*
                            var imageName = MinerExeName.Replace(".exe", "");
                            // maybe will have to KILL process
                            KillMinerBase(imageName);
                            int k = ProcessTag().IndexOf("pid(");
                            int i = ProcessTag().IndexOf(")|bin");
                            var cpid = ProcessTag().Substring(k + 4, i - k - 4).Trim();
                            int pid = int.Parse(cpid, CultureInfo.InvariantCulture);
                            KillProcessAndChildren(pid);
                            */
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
        protected override bool BenchmarkParseLine(string outdata)
        {
            return true;
        }

        private ApiData ad;
        public override ApiData GetApiData()
        {
            return ad;
        }
        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.NONE;
            string resp = null;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            if (Form_Main.NVIDIA_orderBug)
            {
                sortedMinerPairs.Sort((a, b) => a.Device.ID.CompareTo(b.Device.ID));
            }
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString() + "/summary");
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 3 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 2 * 1000;
                StreamReader Reader = new StreamReader(SS);
                resp = await Reader.ReadToEndAsync();

                Reader.Close();
                Response.Close();
                WR.Abort();
                SS.Close();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("API", ex.Message);
                return null;
            }

            ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);

            if (resp != null)
            {
                //Helpers.ConsolePrint(MinerTag(), "API: " + resp);
                try
                {
                    dynamic respJson = JsonConvert.DeserializeObject(resp);
                    int devs = 0;
                    double HashrateSecondTotal = 0.0d;
                    foreach (var dev in respJson.gpus)
                    {
                        sortedMinerPairs[devs].Device.MiningHashrate = dev.hashrate;
                        //Helpers.ConsolePrint("********", "API device_id: " + dev.device_id + " gpu_id: " + dev.gpu_id + " gpu_user_id: " + " hashrate1: " + dev.hashrate);
                        _power = sortedMinerPairs[devs].Device.PowerUsage;
                        devs++;
                    }

                    devs = 0;
                    if (_isDual)
                    {
                        foreach (var dev in respJson.dual_stat.gpus)
                        {
                            //Helpers.ConsolePrint("********", "API device_id: " + dev.device_id + " gpu_id: " + dev.gpu_id + " gpu_user_id: " + " hashrate: " + dev.hashrate);
                            sortedMinerPairs[devs].Device.MiningHashrateSecond = dev.hashrate;
                            HashrateSecondTotal += (double)dev.hashrate;

                            devs++;
                        }
                    }
                    //Helpers.ConsolePrint(MinerTag(), "API total: " + respJson.hashrate);
                    ad.Speed = respJson.hashrate;
                    ad.SecondarySpeed = HashrateSecondTotal;
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("API eror", ex.Message);
                    return null;
                }

                if (ad.Speed == 0)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                }
                else
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                }

                if (ad.Speed < 0)
                {
                    Helpers.ConsolePrint(MinerTag(), "Reporting negative speeds will restart...");
                    CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                    ad.Speed = 0;
                    ad.SecondarySpeed = 0;
                    ad.ThirdSpeed = 0;
                    return ad;
                }
            }
            else
            {
                Thread.Sleep(1);
            }

            return ad;
        }


        protected override void _Stop(MinerStopType willswitch)
        {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
            if (ProcessHandle != null)
            {
                if (!ConfigManager.GeneralConfig.NoForceTRexClose)
                {
                    Thread.Sleep(500);
                    Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Try force killing miner!");
                    try { KillMinerBase("t-rex"); }
                    catch { }
                }
            }
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5; // 5 minute max, whole waiting time 75seconds
        }
    }
}
