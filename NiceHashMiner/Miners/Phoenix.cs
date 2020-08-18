/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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
    public class Phoenix : Miner
    {
        public Phoenix()
            : base("Phoenix")
        {
        }

        private bool _benchmarkException => MiningSetup.MinerPath == MinerPaths.Data.Phoenix;
        private int TotalCount = 6;
        private const int TotalDelim = 2;
        double dSpeed = 0;
        double speed = 0;
        string cSpeed = "";
        int count = 0;
        string ResponseFromPhoenix;
        private string[,] myServers = Form_Main.myServers;

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 8;
        }

        private string GetStartCommand(string url, string btcAdress, string worker)
        {
            var username = GetUsername(btcAdress, worker);
            var platform = "";
            url = url.Replace("daggerhashimoto3gb", "daggerhashimoto");
            url = url.Replace("daggerhashimoto4gb", "daggerhashimoto");
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    platform = " -nvidia ";
                }
                else
                {
                    platform = " -amd ";
                }
            }

            if (File.Exists("miners\\phoenix\\epools.txt"))
                File.Delete("miners\\phoenix\\epools.txt");


            Thread.Sleep(200);

            var epools = String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[1, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[2, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[3, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[4, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[5, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[0, 0], username) + "\n";
            try
            {
                FileStream fs = new FileStream("miners\\phoenix\\epools.txt", FileMode.Create, FileAccess.Write);
                StreamWriter w = new StreamWriter(fs);
                w.WriteAsync(epools);
                w.Flush();
                w.Close();
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("GetStartCommand", e.ToString());
            }


            if (platform == " -amd ")
            {
                return " -rvram -1 -gpus " + GetDevicesCommandString() + platform + "-retrydelay 10"
                       + $" -pool {url} -wal {username} -cdmport  127.0.0.1:{ApiPort} -proto 4 -pass x " +
                       ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
            }
            return " -rvram -1 -gpus " + GetDevicesCommandString() + platform + "-retrydelay 10"
       + $" -pool {url} -wal {username} -cdmport  127.0.0.1:{ApiPort} -proto 4 -pass x " +
       ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA);
        }

        private string GetStartBenchmarkCommand(string url, string btcAdress, string worker)
        {
            //var username = GetUsername(Globals.GetBitcoinUser(), worker);
            //var username2 = GetUsername(btcAdress, worker);
           // var username2 = btcAdress + "." + worker;
            var platform = "";
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    platform = " -nvidia ";
                }
                else
                {
                    platform = " -amd ";
                }
            }

            if (File.Exists("miners\\phoenix\\epools.txt"))
                File.Delete("miners\\phoenix\\epools.txt");

            Thread.Sleep(200);

            return " -rvram -1 -eres 0 -gpus " + GetDevicesCommandString() + platform + "-retrydelay 10"
                   + $" -pool {url} -wal {btcAdress} -cdmport  127.0.0.1:{ApiPort} -pass x " +
                   ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);

        }

        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";
            var ids = MiningSetup.MiningPairs.Select(mPair => (mPair.Device.IDByBus + 1).ToString()).ToList();
            deviceStringCommand += string.Join(",", ids);
            return deviceStringCommand;
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            LastCommandLine = GetStartCommand(url, btcAdress, worker) + " -logfile " + GetLogFileName();
            IsApiReadException = false;
            ProcessHandle = _Start();
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            //try { ProcessHandle.SendCtrlC((uint)Process.GetCurrentProcess().Id); } catch { }
            //Thread.Sleep(200);
            //Helpers.ConsolePrint("Phoenix", "Stop");
             Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
            // Thread.Sleep(200);
            /*
            foreach (var process in Process.GetProcessesByName("PhoenixMiner"))
            {
                try {
                    process.Kill();
                    Thread.Sleep(500);
                    //process.Kill();
                }
                catch (Exception e) { Helpers.ConsolePrint(MinerDeviceName, e.ToString()); }
            }
            */
        }


        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            var url = GetServiceUrl(algorithm.NiceHashID);
            string ret = "";
            if (algorithm.NiceHashID == AlgorithmType.DaggerHashimoto)
            {
                ret = GetStartBenchmarkCommand("stratum+tcp://eu1.ethermine.org:4444", "0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.Phoenix", "")
                         + " -logfile " + GetLogFileName();
            }
            if (algorithm.NiceHashID == AlgorithmType.DaggerHashimoto4GB)
            {
                ret = GetStartBenchmarkCommand("stratum+tcp://us-east.ethash-hub.miningpoolhub.com:20565", "angelbbs.Phoenix4", "") + " -proto 1"
                         + " -logfile " + GetLogFileName();
            }

            //BenchmarkTimeWait = Math.Max(60, Math.Min(120, time * 3));
            return ret;

        }

        protected override bool BenchmarkParseLine(string outdata)
        {
            if (_benchmarkException)
            {
                if (outdata.Contains("Average speed"))
                {
                    count++;
                    TotalCount--;
                }

                if (outdata.Contains("Eth speed:"))
                {
                    var st = outdata.IndexOf("Eth speed: ");
                    var e = outdata.IndexOf("/s, shares");
                    var parse = outdata.Substring(st + 11, e - st - 14).Trim().Replace(",", ".");
                    try
                    {
                        speed = Double.Parse(parse, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }

                    if (outdata.ToUpper().Contains("KH/S"))
                        speed *= 1000;
                    else if (outdata.ToUpper().Contains("MH/S"))
                        speed *= 1000000;
                    else if (outdata.ToUpper().Contains("GH/S"))
                        speed *= 10000000000;
                }

                if (TotalCount <= 0)
                {
                    BenchmarkAlgorithm.BenchmarkSpeed = speed;
                    BenchmarkSignalFinnished = true;
                    return true;
                }

                return false;
            }
            return false;

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
            // CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
            CurrentMinerReadStatus = MinerApiReadStatus.WAIT;

            var ad = new ApiData(MiningSetup.CurrentAlgorithmType);



            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString());
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 30 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 20 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromPhoenix = await Reader.ReadToEndAsync();
                Reader.Close();
                Response.Close();
            }
            catch (Exception ex)
            {
                //Helpers.ConsolePrint("API", ex.Message);
                return null;
            }

            if (ResponseFromPhoenix.Contains("Eth speed:"))
            {
                var st = ResponseFromPhoenix.LastIndexOf("Eth speed: ");
                var e = ResponseFromPhoenix.LastIndexOf("/s, shares");
//                Helpers.ConsolePrint("API st:", st.ToString());
//                Helpers.ConsolePrint("API e:", e.ToString());
//                Helpers.ConsolePrint("API:", ResponseFromPhoenix.Substring(st + 11, e - st - 14));
                cSpeed = ResponseFromPhoenix.Substring(st + 11, e - st - 14);

                try
                {
                    dSpeed = Double.Parse(cSpeed); // тут проблема
                } catch (Exception ex)
                {
                    Helpers.ConsolePrint("API exeption:", ex.Message);
                    Helpers.ConsolePrint("API st:", ResponseFromPhoenix);
                    Helpers.ConsolePrint("API st:", st.ToString());
                    Helpers.ConsolePrint("API e:", e.ToString());
                }


                if (ResponseFromPhoenix.ToUpper().Contains("KH/S"))
                    dSpeed *= 1000;
                else if (ResponseFromPhoenix.ToUpper().Contains("MH/S"))
                    dSpeed *= 1000000;
                else if (ResponseFromPhoenix.ToUpper().Contains("GH/S"))
                    dSpeed *= 10000000000;

                ad.Speed = dSpeed;

            }

            if (ad.Speed == 0)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
            }
            else
            {
                CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
            }

            //Thread.Sleep(1000);
            return ad;

        }


    }

}
