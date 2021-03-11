using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NiceHashMiner.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMiner.Configs;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using NiceHashMiner.Devices;

namespace NiceHashMiner.Miners
{
    public class CryptoDredge : Miner
    {
        public CryptoDredge() : base("CryptoDredge")
        { }

        private int TotalCount = 0;

        private double Total = 0;
        private const int TotalDelim = 2;
        double speed = 0;
        int count = 0;
        private int _benchmarkTimeWait = 180;
        private bool _benchmarkException => MiningSetup.MinerPath == MinerPaths.Data.CryptoDredge;

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 8;
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            if (!IsInit)
            {
                Helpers.ConsolePrint(MinerTag(), "MiningSetup is not initialized exiting Start()");
                return;
            }
            var username = GetUsername(btcAdress, worker);

         //    IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.CryptoDredge;

            var algo = "";
            var apiBind = "";
            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);
            algo = "--algo " + MiningSetup.MinerName;
            apiBind = " --api-bind 127.0.0.1:" + ApiPort;
            IsApiReadException = false;

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                algo = "--algo kawpow";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckooCycle)
            {
                algo = "--algo aeternity";
                //IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.CryptoDredge;
                //IsApiReadException = true; //0.18.0 api broken
            }
            string nhsuff = "";

            LastCommandLine = algo +
                " -o " + url + " -u " + username + " -p x " +
                " -o " + alg + "." + Form_Main.myServers[1, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x " +
                " -o " + alg + "." + Form_Main.myServers[2, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x " +
                " -o " + alg + "." + Form_Main.myServers[3, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x " +
                " -o " + alg + "." + Form_Main.myServers[0, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x " +
                " -o " + url + " -u " + username + " -p x " +
                apiBind +
                " -d " + GetDevicesCommandString() + " " +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
            ProcessHandle = _Start();
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
            Thread.Sleep(200);
            try { ProcessHandle.SendCtrlC((uint)Process.GetCurrentProcess().Id); } catch { }
            Thread.Sleep(200);
            foreach (var process in Process.GetProcessesByName("CryptoDredge"))
            {
                try {
                    process.Kill();
                    Thread.Sleep(200);
                    process.Kill();
                }
                catch (Exception e) { Helpers.ConsolePrint(MinerDeviceName, e.ToString()); }
            }
        }

        // new decoupled benchmarking routines

        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            string url = Globals.GetLocationUrl(algorithm.NiceHashID, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], this.ConectionType);
            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);
            var username = GetUsername(Globals.GetBitcoinUser(), ConfigManager.GeneralConfig.WorkerName.Trim());
            var apiBind = " --api-bind 127.0.0.1:" + ApiPort;
            var algo = "--algo " + MiningSetup.MinerName;
            var commandLine = "";
            _benchmarkTimeWait = time;
            TotalCount = _benchmarkTimeWait/60;
            /*
            if (File.Exists("miners\\CryptoDredge\\" + GetLogFileName()))
                File.Delete("miners\\CryptoDredge\\" + GetLogFileName());
            */
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
            {
                commandLine = "--algo x16rv2" +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o stratum+tcp://x16rv2.eu.mine.zpool.ca:3637" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                apiBind +
                " -d " + GetDevicesCommandString() + " " +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
                //   TotalCount = 3;
                Total = 0.0d;
                return commandLine;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                commandLine = "--algo kawpow" +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o stratum+tcp://rvn.2miners.com:6060" + " -u RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.CryptoDredge" + " -p x" +
                apiBind +
                " -d " + GetDevicesCommandString() + " " +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
            //    TotalCount = 2;
                Total = 0.0d;
                return commandLine;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2REv3))
            {
                commandLine = "--algo lyra2v3" +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o stratum+tcp://lyra2v3.eu.mine.zpool.ca:4550" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                apiBind +
                " -d " + GetDevicesCommandString() + " " +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
              //  TotalCount = 2;
                Total = 0.0d;
                return commandLine;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.NeoScrypt))
            {
                commandLine = "--algo neoscrypt --retry-pause 5" +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o stratum+tcp://neoscrypt.na.mine.zpool.ca:4233" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " + //no support for failover pools?
                " -o stratum+tcp://neoscrypt.jp.mine.zpool.ca:4233" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                " -o stratum+tcp://neoscrypt.sea.mine.zpool.ca:4233" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                " -o stratum+tcp://neoscrypt.eu.mine.zpool.ca:4233" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                apiBind +
                " -d " + GetDevicesCommandString() + " " +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
             //   TotalCount = 2;
                Total = 0.0d;
                return commandLine;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.MTP))
            {
                algo = "--algo mtp";
                commandLine = algo +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o stratum+tcp://xzc.2miners.com:8080" + " -u aMGfYX8ARy4wKE57fPxkEBcnNuHegDBweE." + ConfigManager.GeneralConfig.WorkerName.Trim() + " -p x " +
                apiBind +
                " -d " + GetDevicesCommandString() + " " +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
             //   TotalCount = 3;
                Total = 0.0d;
                return commandLine;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CuckooCycle))
            {
                algo = "--algo aeternity";
                commandLine = algo +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o stratum+tcp://ae.f2pool.com:7898" + " -u ak_2f9AMwztStKs5roPmT592wTbUEeTyqRgYVZNrc5TyZfr94m7fM." + ConfigManager.GeneralConfig.WorkerName.Trim() + " -p x " +
                apiBind +
                " -d " + GetDevicesCommandString() + " " +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
             //   TotalCount = 3;
                Total = 0.0d;
                return commandLine;
            }
            commandLine = algo +
                " -o " + url + " -u " + username + " -p x " +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o " + alg + "." + Form_Main.myServers[3, 0] + ".nicehash.com:" + port + " -u " + username + " -p x " +
                " -o " + alg + "." + Form_Main.myServers[2, 0] + ".nicehash.com:" + port + " -u " + username + " -p x " +
                " -o " + alg + "." + Form_Main.myServers[1, 0] + ".nicehash.com:" + port + " -u " + username + " -p x " +
                " -o " + url + " -u " + username + " -p x " +
                apiBind +
                " -d " + GetDevicesCommandString() + " " +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";

           // TotalCount = 2;
            Total = 0.0d;
            return commandLine;
        }

        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
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

            int delay_before_calc_hashrate = 10;
            int MinerStartDelay = 10;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
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

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 20;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
                    {
                        delay_before_calc_hashrate = 2;
                        MinerStartDelay = 10;
                    }

                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
                        BenchmarkAlgorithm.BenchmarkProgressPercent = (int)(benchProgress * 100);
                        if (repeats > delay_before_calc_hashrate)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString());
                            summspeed += ad.Result.Speed;
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
                BenchmarkAlgorithm.BenchmarkSpeed = Math.Round(summspeed / (repeats - delay_before_calc_hashrate), 2);
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
        #endregion // Decoupled benchmarking routines

        public override async Task<ApiData> GetSummaryAsync()
        {
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckaroo29)) //0.18.0 api broken
            {
                var totalSpeed = 0.0d;
                foreach (var miningPair in MiningSetup.MiningPairs)
                {
                    var algo = miningPair.Device.GetAlgorithm(MinerBaseType.CryptoDredge, AlgorithmType.GrinCuckaroo29, AlgorithmType.NONE);
                    if (algo != null)
                    {
                        totalSpeed += algo.BenchmarkSpeed;
                    }
                }

                var cdData = new ApiData(MiningSetup.CurrentAlgorithmType)
                {
                    Speed = totalSpeed
                };
                CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                // check if speed zero
                if (cdData.Speed == 0) CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                return cdData;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CuckooCycle)) //0.18.0 api broken
            {
                var totalSpeed = 0.0d;
                foreach (var miningPair in MiningSetup.MiningPairs)
                {
                    var algo = miningPair.Device.GetAlgorithm(MinerBaseType.CryptoDredge, AlgorithmType.CuckooCycle, AlgorithmType.NONE);
                    if (algo != null)
                    {
                        totalSpeed += algo.BenchmarkSpeed;
                    }
                }

                var cdData = new ApiData(MiningSetup.CurrentAlgorithmType)
                {
                    Speed = totalSpeed
                };
                CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                // check if speed zero
                if (cdData.Speed == 0) CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                return cdData;
            }


            CurrentMinerReadStatus = MinerApiReadStatus.NONE;
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);
            double tmp = 0;

            string resp = null;
            try
            {
                var bytesToSend = Encoding.ASCII.GetBytes("summary");
                var client = new TcpClient("127.0.0.1", ApiPort);
                var nwStream = client.GetStream();
                await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
                var bytesToRead = new byte[client.ReceiveBufferSize];
                var bytesRead = await nwStream.ReadAsync(bytesToRead, 0, client.ReceiveBufferSize);
                var respStr = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                //Helpers.ConsolePrint(MinerTag(), "API: " + respStr);
                client.Close();
                resp = respStr;
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), "GetSummary exception: " + ex.Message);
            }

            if (resp != null )
            {
                    var st = resp.IndexOf(";KHS=");
                    var e = resp.IndexOf(";SOLV=");
                    var parse = resp.Substring(st + 5, e - st - 5).Trim();
                try
                {
                    tmp = Double.Parse(parse, CultureInfo.InvariantCulture);
                }
                catch
                {
                    MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    BenchmarkSignalFinnished = true;
                }
                ad.Speed = tmp*1000;
                /*
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckaroo29))
                {
                    ad.Speed = BenchmarkAlgorithm.BenchmarkSpeed;
                }
                */

                if (ad.Speed == 0)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                } else
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                }

                if (ad.Speed < 0)
                {
                    Helpers.ConsolePrint(MinerTag(), "Reporting negative speeds will restart...");
                    Restart();
                }
            }

            return ad;
        }


    }

}
