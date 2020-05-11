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
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

namespace NiceHashMiner.Miners
{
    public class Kawpowminer : Miner
    {
        public Kawpowminer() : base("Kawpowminer")
        {
            ConectionType = NhmConectionType.NONE;
        }

        private int TotalCount = 2;
        private double speed = 0;

        private int Total = 0;
        private const int TotalDelim = 2;
        private string[,] myServers = Form_Main.myServers;

        private bool _benchmarkException => MiningSetup.MinerPath == MinerPaths.Data.Kawpowminer;

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

            // IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.Kawpowminer;
            var algo = "";
            var apiBind = "";
            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);
            algo = "--algo=" + MiningSetup.MinerName;
            apiBind = " --api-bind 127.0.0.1:" + ApiPort;
            url = url.Replace("stratum+tcp://", "");
            var devs = string.Join(" ", MiningSetup.MiningPairs.Select(p => p.Device.IDByBus));

            LastCommandLine = " -U -P stratum://" + username + "@" + url + 
                " -U -P stratum://" + username + "@kawpow." + myServers[1, 0] + ".nicehash.com:3385" +
                " -U -P stratum://" + username + "@kawpow." + myServers[2, 0] + ".nicehash.com:3385" +
                " -U -P stratum://" + username + "@kawpow." + myServers[3, 0] + ".nicehash.com:3385" +
                " -U -P stratum://" + username + "@kawpow." + myServers[4, 0] + ".nicehash.com:3385" +
                " -U -P stratum://" + username + "@kawpow." + myServers[5, 0] + ".nicehash.com:3385" +
                " -U -P stratum://" + username + "@kawpow." + myServers[0, 0] + ".nicehash.com:3385" +
                " " + apiBind +
                " --failover-timeout 60 --cu-devices " + devs + " " +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";

            ProcessHandle = _Start();
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        // new decoupled benchmarking routines

        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            string url = Globals.GetLocationUrl(algorithm.NiceHashID, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], this.ConectionType);
            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);
            var username = GetUsername(Globals.GetBitcoinUser(), ConfigManager.GeneralConfig.WorkerName.Trim());
            var commandLine = "";
            var devs = string.Join(" ", MiningSetup.MiningPairs.Select(p => p.Device.IDByBus));

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                commandLine = " -U -P stratum://RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.KawpowMiner@rvn.2miners.com:6060" +
                " -U -P stratum://" + username + "@kawpow." + myServers[1, 0] + ".nicehash.com:3385" +
                " " + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) +
                "--cu-devices " + devs + " ";
            }

            TotalCount =(time / 5);

            Total =TotalCount;

            return commandLine;
        }

        protected override bool BenchmarkParseLine(string outdata)
        {
            int count = 0;
            double tmp = 0;


            if (_benchmarkException)
            {
                if ( outdata.Contains(">") && outdata.Contains("Mh"))
                //m 22:45:05 <unknown> 0:01 A1 10.00 Mh - cu0 10.00
                {

                    var st = outdata.IndexOf("> ");
                    var e = outdata.IndexOf("Mh ");
                    try
                    {
                        var parse = outdata.Substring(st + 10, e - st - 11).Trim();
                        Helpers.ConsolePrint(MinerTag(), "st = " + st.ToString());
                        Helpers.ConsolePrint(MinerTag(), "e = " + e.ToString());
                        Helpers.ConsolePrint(MinerTag(), "parse: " + parse);
                        tmp = Double.Parse(parse, CultureInfo.InvariantCulture);
                    } catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    // save speed
                        tmp *= 1000000;

                    speed = speed + tmp;
                    TotalCount--;
                }

                if (TotalCount <= 0)
                {
                    BenchmarkAlgorithm.BenchmarkSpeed = speed/Total;
                    BenchmarkSignalFinnished = true;
                    return true;
                }

               // return false;
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
            CurrentMinerReadStatus = MinerApiReadStatus.NONE;
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);
           // var elapsedSeconds = DateTime.Now.Subtract(_started).Seconds; ////PVS-Studio - stupid program!
           /*
            if (elapsedSeconds < 15 && firstStart)
            {
                return ad;
            }
            firstStart = false;
            */
            JsonApiResponse resp = null;
            try
            {
                var bytesToSend = Encoding.ASCII.GetBytes("{\"id\":0,\"jsonrpc\":\"2.0\",\"method\":\"miner_getstat1\"}\n");

                using (var client = new TcpClient("127.0.0.1", ApiPort))
                using (var nwStream = client.GetStream())
                {
                    await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
                    var bytesToRead = new byte[client.ReceiveBufferSize];
                    var bytesRead = await nwStream.ReadAsync(bytesToRead, 0, client.ReceiveBufferSize);
                    var respStr = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                   // Helpers.ConsolePrint(MinerTag(), "respStr: " + respStr);
                    resp = JsonConvert.DeserializeObject<JsonApiResponse>(respStr, Globals.JsonSettings);
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), "GetSummary exception: " + ex.Message);
            }
            //{"id":0,"jsonrpc":"2.0","result":["TT-Miner/2.2.1","0","4235681;1;0","2077789;2157892","0;0;0","off;off","71;79;59;68","mtp.hk.nicehash.com:3374","0;0;0;0"]}

            if (resp != null && resp.error == null)
            {
                var speed = resp.result[2].ToString().Split(';')[0];
                double tmpSpeed = double.Parse(speed, CultureInfo.InvariantCulture);
                ad.Speed = tmpSpeed * 1000;
                
                CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;

                if (ad.Speed == 0)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                }

            }
            //Thread.Sleep(200);
            return ad;
        }
        private class JsonApiResponse
        {
            public List<string> result { get; set; }
            public int id { get; set; }
            public object error { get; set; }
        }

    }
}
