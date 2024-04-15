using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NiceHashMiner.Miners
{
    class teamredminer : Miner
    {
        private readonly int GPUPlatformNumber;
        Stopwatch _benchmarkTimer = new Stopwatch();
        private int TotalCount = 0;
        private int _benchmarkTimeWait = 180;
        private double _power = 0.0d;
        double _powerUsage = 0;
        int _apiErrors = 0;

        public teamredminer()
            : base("teamredminer")
        {
            GPUPlatformNumber = ComputeDeviceManager.Available.AmdOpenCLPlatformNum;
            IsKillAllUsedMinerProcs = true;
            IsNeverHideMiningWindow = true;

        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000;
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
            //    Killteamredminer();
        }

        private string GetServer(string algo, string username, string port)
        {
            string ret = "";
            string ssl = "";
            string psw = "x";
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            if (ConfigManager.GeneralConfig.ProxySSL && Globals.MiningLocation.Length > 1)//teamredminer почему-то падает при подключении к серверам с сертификатом
                //letsencrypt. С локальным самоподписанным всё хорошо
            {
                port = "1" + port;
                ssl = "stratum+tcp://";
            }
            else
            {
                port = "1" + port;
                ssl = "stratum+tcp://";
            }
            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + "-o stratum+tcp://" + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 -u " + 
                        username + " -p " + psw + " ";
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + "-o " + ssl + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " -u " + 
                        username + " -p " + psw + " ";
                }
            }
            return ret;
        }
        private string GetServerDual(string algo, string algoDual, string algoDualPrefix, string username, string port, string portDual)
        {
            string ret = "";
            string ssl = "";
            string psw = "x";
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            if (ConfigManager.GeneralConfig.ProxySSL)//teamredminer почему-то падает при подключении к серверам с сертификатом
                                                     //letsencrypt. С локальным самоподписанным всё хорошо
            {
                port = "1" + port;
                portDual = "1" + portDual;
                ssl = "stratum+tcp://";
            }
            else
            {
                port = "1" + port;
                portDual = "1" + portDual;
                ssl = "stratum+tcp://";
            }
            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + "-o stratum+tcp://" + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 -u " +
                        username + " -p " + psw + " " +
                        algoDualPrefix + " -o stratum+tcp://" + Links.CheckDNS(algoDual + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 -u " +
                        username + " -p " + psw + " ";
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + "-o " + ssl + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " -u " +
                        username + " -p " + psw + " " +
                        algoDualPrefix + " -o " + ssl + Links.CheckDNS(algoDual + "." + serverUrl).Replace("stratum+tcp://", "") + ":" + portDual + " -u " +
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
            string username = GetUsername(btcAdress, worker);
            //IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.teamredminer;
            IsApiReadException = false;

            //add failover
            string algo = "";
            string algo2 = "";
            string port = "";

            string apiBind = " --api_listen=127.0.0.1:" + ApiPort;
            string apiBind2 = "";
            if (!MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))
            {
                apiBind2 = " --api2_listen=127.0.0.1:" + "1" + ApiPort.ToString();
            }

            var sc = "";
            if (Form_Main.GetWinVer(Environment.OSVersion.Version) < 8)
            {
                sc = variables.TRMiner_add1;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                algo = "ethash";
                algo2 = "daggerhashimoto";
                port = "3353";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                algo = "kawpow";
                algo2 = "kawpow";
                port = "3385";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KarlsenHash))
            {
                algo = "karlsen";
                algo2 = "karlsenhash";
                port = "3398";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos) && 
                MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))
            {
                algo = "autolykos2";
                algo2 = "autolykos";
                port = "3390";
            }
            /*
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos) &&
                MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.IronFish))
            {
                algo = "autolykos2";
                algo2 = "autolykos";
                port = "3390";
                LastCommandLine = sc + "" +
                    " -d " + GetDevicesCommandString() +
                    " -a " + algo + " " +
            GetServerDual(algo2, "ironfish", "--iron", username, port, "3397") +
            " --rig_id=" + username.Split('.')[1] + " --pool_force_ensub " +
                              apiBind + apiBind2 +
                              " " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD);

                ProcessHandle = _Start();
                return;
            }
            */

            LastCommandLine = sc + "" + "-a " + algo + " " +
            GetServer(algo2, username, port) +
                              apiBind +
                              " " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                              " -d " + GetDevicesCommandString();

            ProcessHandle = _Start();
        }

        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = "";
            var ids = new List<string>();
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.BusID).ToList();
            int id;
            foreach (var mPair in sortedMinerPairs)
            {
                id = mPair.Device.IDByBus;
                ids.Add(id.ToString());
            }

            deviceStringCommand += string.Join(",", ids);
            return deviceStringCommand;
        }
        // new decoupled benchmarking routines
        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            var CommandLine = "";
            string apiBind = " --api_listen=127.0.0.1:" + ApiPort;
            string apiBind2 = "";
            if (!MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))
            {
                apiBind2 = " --api2_listen=127.0.0.1:" + "1" + ApiPort.ToString();
            }
            var sc = "";
            _benchmarkTimeWait = time;
            if (Form_Main.GetWinVer(Environment.OSVersion.Version) < 8)
            {
                sc = variables.TRMiner_add1;
            }
            // demo for benchmark
            string username = Globals.GetBitcoinUser();
            string worker = "";
            if (ConfigManager.GeneralConfig.WorkerName.Length > 0)
            {
                username += "." + ConfigManager.GeneralConfig.WorkerName.Trim();
                worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                CommandLine = sc + " -a ethash --eth_no_ramp_up" +
                 " -o " + Links.CheckDNS("stratum+tcp://ethw.2miners.com:2020") + " -u 0x266b27bd794d1A65ab76842ED85B067B415CD505.teamred" + " -p x -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                CommandLine = sc + " -a kawpow" +
                 " -o stratum+tcp://rvn.2miners.com:6060" + " -u RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.teamred" + " -p x -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KarlsenHash))
            {
                CommandLine = sc + " -a karlsen" +
                 " -o stratum+tcp://kls.2miners.com:2020" + " -u karlsen:qrnsjf7ka334kx0rlgfxxvqf04c9qthdltfj7q7amm6nqvmqz9csunnazj64s.teamred" + " -p x -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos) &&
                MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))
            {
                CommandLine = sc + " -a autolykos2" +
                 " -o " + Links.CheckDNS("stratum+tcp://pool.woolypooly.com:3100") + " -u 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.teamred" + " -p x -d ";
            }
            /*
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos) &&
                MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.IronFish))
            {
                CommandLine = sc + " -a autolykos2" +
                 " -o " + Links.CheckDNS("stratum+tcp://pool.woolypooly.com:3100") + " -u 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.teamred" + " -p x -d " + GetDevicesCommandString() +
                 " --iron -o " + Links.CheckDNS("stratum+tcp://ru.ironfish.herominers.com:1145") + " -u fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.teamred" + " -p x -d ";
            }
            */
            CommandLine += GetDevicesCommandString() +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD) +
                apiBind + apiBind2;
            TotalCount = (time / 30) * 2;
            return CommandLine;

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
            double summspeedDual = 0.0d;

            int delay_before_calc_hashrate = 10;
            int MinerStartDelay = 10;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
                //if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 30;
                }
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                Helpers.ConsolePrint(MinerTag(), "Benchmark should end in: " + _benchmarkTimeWait + " seconds");
                BenchmarkHandle = BenchmarkStartProcess((string)commandLine);
                //BenchmarkHandle.WaitForExit(_benchmarkTimeWait + 2);
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

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                    {
                        delay_before_calc_hashrate = 30;
                        MinerStartDelay = 0;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
                    {
                        delay_before_calc_hashrate = 30;
                        MinerStartDelay = 10;
                    }
                    
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
                    {
                        delay_before_calc_hashrate = 30;
                        MinerStartDelay = 0;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KarlsenHash))
                    {
                        delay_before_calc_hashrate = 30;
                        MinerStartDelay = 0;
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
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString() + 
                                " Second speed: " + ad.Result.SecondarySpeed.ToString() +
                                " power: " + _power.ToString());
                            summspeed = Math.Max(summspeed, ad.Result.Speed);
                            summspeedDual = Math.Max(summspeedDual, ad.Result.SecondarySpeed);
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

                            BenchmarkHandle.Kill();
                            BenchmarkHandle.Dispose();
                            EndBenchmarkProcces();

                            break;
                        }

                    }
                }
                BenchmarkAlgorithm.BenchmarkSpeed = summspeed;
                BenchmarkAlgorithm.BenchmarkSecondarySpeed = summspeedDual;
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
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }


        #endregion // Decoupled benchmarking routines

        private ApiData ad;
        public override ApiData GetApiData()
        {
            return ad;
        }

        protected async Task<string> GetApiDataAsync(int port, string dataToSend, bool exitHack = false,
            bool overrideLoop = false)
        {
            string responseFromServer = null;
            try
            {
                var tcpc = new TcpClient("127.0.0.1", port);
                var nwStream = tcpc.GetStream();
                nwStream.ReadTimeout = 2 * 1000;
                nwStream.WriteTimeout = 2 * 1000;

                var bytesToSend = Encoding.ASCII.GetBytes(dataToSend);
                await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);

                var incomingBuffer = new byte[tcpc.ReceiveBufferSize];
                var prevOffset = -1;
                var offset = 0;
                var fin = false;

                while (!fin && tcpc.Client.Connected)
                {
                    var r = await nwStream.ReadAsync(incomingBuffer, offset, tcpc.ReceiveBufferSize - offset);
                    for (var i = offset; i < offset + r; i++)
                    {
                        if (incomingBuffer[i] == 0x7C || incomingBuffer[i] == 0x00
                                                      || (i > 2 && IsApiEof(incomingBuffer[i - 2],
                                                              incomingBuffer[i - 1], incomingBuffer[i]))
                                                      || overrideLoop)
                        {
                            fin = true;
                            break;
                        }

                        // Not working
                        //if (IncomingBuffer[i] == 0x5d || IncomingBuffer[i] == 0x5e) {
                        //    fin = true;
                        //    break;
                        //}
                    }

                    offset += r;
                    if (exitHack)
                    {
                        if (prevOffset == offset)
                        {
                            fin = true;
                            break;
                        }

                        prevOffset = offset;
                    }
                }

                tcpc.Close();

                if (offset > 0)
                    responseFromServer = Encoding.ASCII.GetString(incomingBuffer);
            }
            catch (Exception ex)
            {
                _apiErrors++;
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + " GetAPIData reason: " + ex.Message);
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                if (_apiErrors > 60)
                {
                    _apiErrors = 0;
                    Helpers.ConsolePrint("GetApiDataAsync", "Need RESTART TEAMREDMINER");
                    CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                    ad.Speed = 0;
                    ad.SecondarySpeed = 0;
                    ad.ThirdSpeed = 0;
                    return null;
                }
                return null;
            }
            _apiErrors = 0;
            return responseFromServer;
        }
        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.BusID).ToList();
            var bytesToSend = "devs";
            string resp1 = await GetApiDataAsync(ApiPort, bytesToSend);
            string resp2 = "";
            if (!MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))
            {
                int ApiPort2 = ApiPort + 10000;
                resp2 = await GetApiDataAsync(ApiPort2, bytesToSend);
            }
            ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);
            ad.ThirdAlgorithmID = AlgorithmType.NONE;
            if (resp1 == null)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.NONE;
                ad.Speed = 0.0d;
                return null;
            }
            resp1 = resp1.Trim('\x00');
            resp2 = resp2.Trim('\x00');
           // Helpers.ConsolePrint("API1 <- ", resp1.Trim());
            //Helpers.ConsolePrint("API2 <- ", resp2.Trim());
            if (resp1.Contains("Status=Dead") || resp2.Contains("Status=Dead"))
            {
                Helpers.ConsolePrint("GetSummaryAsync", "Dead GPU detected. Need Restart miner.");
                CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                ad.Speed = 0;
                ad.SecondarySpeed = 0;
                ad.ThirdSpeed = 0;
                return ad;
            }
            try
            {
                //if (MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))
                {
                    var devStatus = resp1.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    int dev = 0;
                    double totalSpeed = 0.0d;
                    foreach (var s in devStatus)
                    {
                        if (s.Contains("GPU="))
                        {
                            var st = s.LastIndexOf("MHS 30s=");
                            var e = s.LastIndexOf(",KHS av=");
                            string cSpeed = s.Substring(st + 8, e - st - 8);
                            //Helpers.ConsolePrint("API: ", cSpeed);
                            double.TryParse(cSpeed, out double devSpeed);
                            if (devSpeed > 1000 && MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOW)//1000 MH
                            {
                                Helpers.ConsolePrint("GetSummaryAsync", "Dead GPU#" + dev.ToString() + " detected. Restart miner.");
                                CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                                ad.Speed = 0;
                                ad.SecondarySpeed = 0;
                                ad.ThirdSpeed = 0;
                                return ad;
                            }
                            sortedMinerPairs[dev].Device.MiningHashrate = devSpeed * 1000000;
                            _power = sortedMinerPairs[dev].Device.PowerUsage;
                            totalSpeed = totalSpeed + devSpeed * 1000000;
                            ad.Speed = totalSpeed;
                            dev++;
                        }

                    }
                }
                if (!MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))
                {
                    var devStatus = resp2.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    int dev = 0;
                    double totalSpeed2 = 0.0d;
                    ad.SecondaryAlgorithmID = MiningSetup.CurrentSecondaryAlgorithmType;
                    foreach (var s in devStatus)
                    {
                        if (s.Contains("GPU="))
                        {
                            var st = s.LastIndexOf("MHS 30s=");
                            var e = s.LastIndexOf(",KHS av=");
                            string cSpeed = s.Substring(st + 8, e - st - 8);
                            //Helpers.ConsolePrint("API2: ", cSpeed);
                            double.TryParse(cSpeed, out double devSpeed2);
                            sortedMinerPairs[dev].Device.MiningHashrateSecond = devSpeed2 * 1000000;
                            _power = sortedMinerPairs[dev].Device.PowerUsage;
                            //totalSpeed = totalSpeed + devSpeed * 1000000;
                            ad.SecondarySpeed = devSpeed2 * 1000000;
                            dev++;
                        }
                    }
                }
            }
            catch
            {
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                return null;
            }

            CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
            // check if speed zero
            if (ad.Speed == 0) CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;

            return ad;
        }
    }
}
