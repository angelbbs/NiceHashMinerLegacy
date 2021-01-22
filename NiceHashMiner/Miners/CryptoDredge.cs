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
        private int _benchmarkTimeWait = 120;
        private string[,] myServers = Form_Main.myServers;
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
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckaroo29)
            {
                algo = "--algo cuckaroo29";
                //IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.CryptoDredge;
                IsApiReadException = true; //0.18.0 api broken
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
                " -o " + alg + "." + myServers[1, 0] + nhsuff + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o " + alg + "." + myServers[2, 0] + nhsuff + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o " + alg + "." + myServers[3, 0] + nhsuff + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o " + alg + "." + myServers[4, 0] + nhsuff + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o " + alg + "." + myServers[5, 0] + nhsuff + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o " + alg + "." + myServers[0, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x " +
                " -o " + url + " -u " + username + " -p x --log " + GetLogFileName() +
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

            if (File.Exists("miners\\CryptoDredge\\" + GetLogFileName()))
                File.Delete("miners\\CryptoDredge\\" + GetLogFileName());

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
            {
                commandLine = "--algo x16rv2" +
                " --url=stratum+tcp://" + alg + "." + myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o stratum+tcp://x16rv2.eu.mine.zpool.ca:3637" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                " --log " + GetLogFileName() +
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
                " --url=stratum+tcp://" + alg + "." + myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o stratum+tcp://rvn.2miners.com:6060" + " -u RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.CryptoDredge" + " -p x" +
                " --log " + GetLogFileName() +
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
                " --url=stratum+tcp://" + alg + "." + myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o stratum+tcp://lyra2v3.eu.mine.zpool.ca:4550" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                " --log " + GetLogFileName() +
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
                " --url=stratum+tcp://" + alg + "." + myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o stratum+tcp://neoscrypt.na.mine.zpool.ca:4233" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " + //no support for failover pools?
                " -o stratum+tcp://neoscrypt.jp.mine.zpool.ca:4233" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                " -o stratum+tcp://neoscrypt.sea.mine.zpool.ca:4233" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                " -o stratum+tcp://neoscrypt.eu.mine.zpool.ca:4233" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                " --log " + GetLogFileName() +
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
                " --url=stratum+tcp://" + alg + "." + myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o stratum+tcp://xzc.2miners.com:8080" + " -u aMGfYX8ARy4wKE57fPxkEBcnNuHegDBweE." + ConfigManager.GeneralConfig.WorkerName.Trim() + " -p x " +
                " --log " + GetLogFileName() +
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
                " --url=stratum+tcp://" + alg + "." + myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o stratum+tcp://ae.f2pool.com:7898" + " -u ak_2f9AMwztStKs5roPmT592wTbUEeTyqRgYVZNrc5TyZfr94m7fM." + ConfigManager.GeneralConfig.WorkerName.Trim() + " -p x " +
                " --log " + GetLogFileName() +
                apiBind +
                " -d " + GetDevicesCommandString() + " " +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
             //   TotalCount = 3;
                Total = 0.0d;
                return commandLine;
            }
            commandLine = algo +
                " -o " + url + " -u " + username + " -p x " +
                " --url=stratum+tcp://" + alg + "." + myServers[0, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o " + alg + "." + myServers[5, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o " + alg + "." + myServers[4, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o " + alg + "." + myServers[3, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o " + alg + "." + myServers[2, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
                " -o " + alg + "." + myServers[1, 0] + ".nicehash.com:" + port + " -u " + username + " -p x " +
                " -o " + url + " -u " + username + " -p x --log " + GetLogFileName() +
                apiBind +
                " -d " + GetDevicesCommandString() + " " +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";

           // TotalCount = 2;
            Total = 0.0d;
            return commandLine;
        }

        protected override bool BenchmarkParseLine(string outdata)
        {
          try
          {
                //[20:42:11] INFO  - GPU0 GTX 1070 : 18,83MH/s (Avr 17,33MH/s) : 124,1KH/W : T=60C Fan=0%
                double tmp = 0;
                    if (outdata.Contains("GPU") && outdata.ToUpper().Contains("GH/S)"))
                    {
                        var st = outdata.IndexOf("Avr ");
                        var e = outdata.ToUpper().IndexOf("GH/S)");
                        var parse = outdata.Substring(st + 4, e - st - 4).Trim().Replace(",",".");
                  //  if (count > 0)//skip first
                    {
                        tmp = Double.Parse(parse, CultureInfo.InvariantCulture);
                        tmp *= 10000000000;
                    }
                        speed += tmp;
                        count++;
                        TotalCount--;
                        goto norm;
                    } else if (outdata.Contains("GPU") && outdata.ToUpper().Contains("MH/S)"))
                    {
                    var st = outdata.IndexOf("Avr ");
                        var e = outdata.ToUpper().IndexOf("MH/S)");
                        var parse = outdata.Substring(st + 4, e - st - 4).Trim().Replace(",", ".");
                   // if (count > 0)//skip first

                        tmp = Double.Parse(parse, CultureInfo.InvariantCulture);
                        tmp *= 1000000;

                        speed += tmp;
                        count++;
                        TotalCount--;
                        goto norm;
                    } else if (outdata.Contains("GPU") && outdata.ToUpper().Contains("KH/S)"))
                    {
                    var st = outdata.IndexOf("Avr ");
                        var e = outdata.ToUpper().IndexOf("KH/S)");
                        var parse = outdata.Substring(st + 4, e - st - 4).Trim().Replace(",", ".");
                 //   if (count > 0)//skip first
                    {
                        tmp = Double.Parse(parse, CultureInfo.InvariantCulture);
                        tmp *= 1000;
                    }
                        speed += tmp;
                        count++;
                        TotalCount--;
                        goto norm;
                    }
                    else if (outdata.Contains("GPU") && outdata.ToUpper().Contains("H/S)"))
                    {
                    var st = outdata.IndexOf("Avr ");
                        var e = outdata.ToUpper().IndexOf("H/S)");
                        var parse = outdata.Substring(st + 4, e - st - 4).Trim().Replace(",", ".");
                 //   if (count > 0)//skip first
                    {
                        tmp = Double.Parse(parse, CultureInfo.InvariantCulture);
                    }
                        speed += tmp;
                        count++;
                        TotalCount--;
                        goto norm;
                    }
norm:
                    if (TotalCount <= 0 && speed > 0.0d)
                    {
                    BenchmarkAlgorithm.BenchmarkSpeed = speed / (count);
                    BenchmarkSignalFinnished = true;
                    return true;
                    }

                return false;
          }
          catch
          {
                MessageBox.Show("Unsupported miner version " + MiningSetup.MinerPath,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                BenchmarkSignalFinnished = true;
                return false;
          }
        }

        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }

        protected override void BenchmarkThreadRoutine(object CommandLine)
        {
            BenchmarkSignalQuit = false;
            BenchmarkSignalHanged = false;
            BenchmarkSignalFinnished = false;
            BenchmarkException = null;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                BenchmarkHandle = BenchmarkStartProcess((string)CommandLine);

                BenchmarkThreadRoutineStartSettup();
                BenchmarkTimeInSeconds = 300;
                BenchmarkProcessStatus = BenchmarkProcessStatus.Running;
                var exited = BenchmarkHandle.WaitForExit((BenchmarkTimeoutInSeconds(BenchmarkTimeInSeconds) + 20) * 1000);
                if (BenchmarkSignalTimedout && !TimeoutStandard)
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

                if (BenchmarkSignalHanged || !exited)
                {
                    throw new Exception("Miner is not responding");
                }

                if (BenchmarkSignalFinnished)
                {
                    //break;
                }
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
