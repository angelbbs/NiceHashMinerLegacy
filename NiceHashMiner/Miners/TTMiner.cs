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
using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace NiceHashMiner.Miners
{
    public class TTMiner : Miner
    {
        public TTMiner() : base("TTMiner_NVIDIA")
        { }

        private int TotalCount = 12;
        private double speed = 0;
        private string[,] myServers = Form_Main.myServers;
        private double Total = 0;
        private const int TotalDelim = 2;
        private bool fapi = true;
        private DateTime _started;
        private bool firstStart = true;


        private bool _benchmarkException => MiningSetup.MinerPath == MinerPaths.Data.TTMiner;

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
            
            //IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.TTMiner;
            IsApiReadException = false;

            //var algo = "";
            var apiBind = "";
            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);
            //algo = "-a " + MiningSetup.MinerName;
            apiBind = " --api-bind 127.0.0.1:" + ApiPort;
            //apiBind = "";
            url = url.Replace("stratum+tcp://", "");
            var algo = "MTP";
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2REv3))
            {
                algo = "LYRA2V3";
            }
            if (NiceHashMiner.Devices.ComputeDeviceManager.Query.CUDA_version == "CUDA 10.1")
            {
                algo = "MTP-101";
            }
            else if (NiceHashMiner.Devices.ComputeDeviceManager.Query.CUDA_version == "CUDA 10.0")
            {
                algo = "MTP-100";
            }
            else if (NiceHashMiner.Devices.ComputeDeviceManager.Query.CUDA_version == "CUDA 9.2")
            {
                algo = "MTP-92";
            }
            string nhsuff = "";
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }
            /*
            LastCommandLine = " -a "+algo +
                " -P " + username + ":x@" + url +
                " -P " + username + ":x@" + alg + ".hk" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".jp" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".in" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".usa" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".br" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".eu" + nhsuff + ".nicehash.com:" + port +

                apiBind +
                " -device " + GetDevicesCommandString() + " " +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
                */
            LastCommandLine = " -a " + algo +
           " -o " + url + " -u " + username + " -p x " +
           " -o " + alg + "." + myServers[1, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
           " -o " + alg + "." + myServers[2, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
           " -o " + alg + "." + myServers[3, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
           " -o " + alg + "." + myServers[4, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
           " -o " + alg + "." + myServers[5, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
           " -o " + alg + "." + myServers[0, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
           apiBind +
           " -device " + GetDevicesCommandString() + " " +
           ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
            _started = DateTime.Now;
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
            var algo = "MTP";
            string nhsuff = "";
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }
            if (NiceHashMiner.Devices.ComputeDeviceManager.Query.CUDA_version == "CUDA 10.1")
            {
                algo = "MTP-101";
            }
            else if (NiceHashMiner.Devices.ComputeDeviceManager.Query.CUDA_version == "CUDA 10.0")
            {
                algo = "MTP-100";
            }
            else if (NiceHashMiner.Devices.ComputeDeviceManager.Query.CUDA_version == "CUDA 9.2")
            {
                algo = "MTP-92";
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.MTP))
            {
                /*
                commandLine = " -a " + algo +
                " -P aMGfYX8ARy4wKE57fPxkEBcnNuHegDBweE." + ConfigManager.GeneralConfig.WorkerName.Trim() + ":x@xzc.2miners.com:8080" +
                " -P " + username + ":x@" + alg + ".eu" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".hk" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".jp" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".in" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".usa" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".br" + nhsuff + ".nicehash.com:" + port +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                  MiningSetup,
                                  DeviceType.NVIDIA) +
                              " -nocolor -PRHRI 1 -device ";
                              */
                commandLine = " -a " + algo +
                          " -o xzc.2miners.com:8080" + " -u aMGfYX8ARy4wKE57fPxkEBcnNuHegDBweE." + ConfigManager.GeneralConfig.WorkerName.Trim() + " -p x " +
                          " -o " + alg + ".hk" + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
                          " -o " + alg + ".jp" + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
                          " -o " + alg + ".in" + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
                          " -o " + alg + ".usa" + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
                          " -o " + alg + ".br" + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
                          " -o " + alg + ".eu" + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
                          ExtraLaunchParametersParser.ParseForMiningSetup(
                                  MiningSetup,
                                  DeviceType.NVIDIA) +
                              " -nocolor -PRHRI 1 -device ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2REv3))
            {
                /*
                commandLine = " -a LYRA2V3" +
                " -P 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2." + ConfigManager.GeneralConfig.WorkerName.Trim() + ":x@lyra2v3.eu.mine.zpool.ca:4550" +
                " -P " + username + ":x@" + alg + ".eu" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".hk" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".jp" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".in" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".usa" + nhsuff + ".nicehash.com:" + port +
                " -P " + username + ":x@" + alg + ".br" + nhsuff + ".nicehash.com:" + port +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                  MiningSetup,
                                  DeviceType.NVIDIA) +
                              " -nocolor -device ";
                              */
                commandLine = " -a LYRA2V3 " +
            " -o lyra2v3.eu.mine.zpool.ca:4550" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2." + ConfigManager.GeneralConfig.WorkerName.Trim() + " -p x " +
            " -o " + alg + ".hk" + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
            " -o " + alg + ".jp" + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
            " -o " + alg + ".in" + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
            " -o " + alg + ".usa" + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
            " -o " + alg + ".br" + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
            " -o " + alg + ".eu" + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
            ExtraLaunchParametersParser.ParseForMiningSetup(
                    MiningSetup,
                    DeviceType.NVIDIA) +
                " -nocolor -PRHRI 1 -device ";
            }

            commandLine += GetDevicesCommandString();

            //TotalCount = 10;
            TotalCount = (time / 12);

            Total = 0.0d;

            return commandLine;
        }

        protected override bool BenchmarkParseLine(string outdata)
        {
            int count = 0;
            double tmp = 0;

            if (_benchmarkException)
            {
                //Helpers.ConsolePrint("BENCHMARK:", outdata);
                if ( outdata.Contains(variables.TTMiner_bench1) && outdata.Contains(variables.TTMiner_bench2)) 
                {

                    var st = outdata.IndexOf(variables.TTMiner_bench3);
                    var e = outdata.IndexOf(variables.TTMiner_bench4);
                    try
                    {
                        var parse = outdata.Substring(st + 4, e - st - 6).Trim();
                        //Helpers.ConsolePrint("BENCHMARK!:", parse);
                        tmp = Double.Parse(parse, CultureInfo.InvariantCulture);
                    } catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    // save speed

                    if (outdata.ToUpper().Contains("KH/S"))
                        tmp *= 1000;
                    else if (outdata.ToUpper().Contains("MH/S"))
                        tmp *= 1000000;
                    else if (outdata.ToUpper().Contains("GH/S"))
                        tmp *= 1000000000;

                    //speed = Math.Max(speed, tmp);
                    speed = Math.Max(speed, tmp);

                    count++;
                    TotalCount--;
                }

                if (TotalCount <= 0 && speed > 0)
                {
                    BenchmarkAlgorithm.BenchmarkSpeed = speed;
                    BenchmarkSignalFinnished = true;
                    return true;
                }

               // return false;
            }
            /*
            if (speed > 0.0d)
            {
                BenchmarkAlgorithm.BenchmarkSpeed = speed/2;
                return true;
            }
            */
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
       
        public List<string> result { get; set; }
        public int id { get; set; }
        public object error { get; set; }

        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.NONE;
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);
            var elapsedSeconds = DateTime.Now.Subtract(_started).Seconds; ////PVS-Studio - stupid program! 

            if (elapsedSeconds < 15 && firstStart)
            {
                return ad;
            }
            firstStart = false;

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
                    //Helpers.ConsolePrint(MinerTag(), "respStr: " + respStr);
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
                ad.Speed = tmpSpeed;
                /*
                    var speeds = resp.result[2].Split(';');
                    ad.Speed = 0;
                    ad.SecondarySpeed = 0;
                    foreach (var speed in speeds)
                    {
                        double tmpSpeed;
                        try
                        {
                            tmpSpeed = double.Parse(speed, CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            tmpSpeed = 0;
                        }
                        ad.Speed = tmpSpeed;
                    }
                    */
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
