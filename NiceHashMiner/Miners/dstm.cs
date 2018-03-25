﻿using Newtonsoft.Json;
using NiceHashMiner.Enums;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using NiceHashMiner.Configs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace NiceHashMiner.Miners
{
    public class DSTM : Miner
    {

        private class Result
        {
            public int gpu_id { get; set; }
            public int temperature { get; set; }
            public double sol_ps { get; set; }
            public double avg_sol_ps { get; set; }
            public double sol_pw { get; set; }
            public double avg_sol_pw { get; set; }
            public double power_usage { get; set; }
            public double avg_power_usage { get; set; }
            public int accepted_shares { get; set; }
            public int rejected_shares { get; set; }
            public int latency { get; set; }
        }
        private class JsonApiResponse
        {
            public uint id { get; set; }
            public Result[] result { get; set; }
            public uint uptime { get; set; }
            public uint contime { get; set; }
            public string server { get; set; }
            public uint port { get; set; }
            public string user { get; set; }
            public string version { get; set; }
            public object error { get; set; }
        }

        private int benchmarkTimeWait = 2 * 45;
        private const string LOOK_FOR_START = "avg:";
        int benchmark_read_count = 0;
        double benchmark_sum = 0.0d;
        const string LOOK_FOR_END = "i/s:";
        const double DevFee = 2.0;

        public DSTM() : base("dstm") {
            ConectionType = NHMConectionType.NONE;
            IsNeverHideMiningWindow = true;
        }

        public override void Start(string url, string btcAdress, string worker) {
            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            var vcp = "msvcp120.dll";
            var vcpPath = WorkingDirectory + vcp;
            ProcessHandle = _Start();
        }

        private string GetStartCommand(string url, string btcAddress, string worker) {
/*
            string alg = url.Split('.')[0];
            var ret = GetDevicesCommandString()
            + " --server " + alg + ".hk.nicehash.com"
            + " --user " + btcAddress + "." + worker + " --pass x --port " + url.Split(':')[1]
            + " --server " + alg + ".in.nicehash.com"
            + " --user " + btcAddress + "." + worker + " --pass x --port " + url.Split(':')[1]
            + " --server " + alg + ".jp.nicehash.com"
            + " --user " + btcAddress + "." + worker + " --pass x --port " + url.Split(':')[1]
            + " --server " + alg + ".usa.nicehash.com"
            + " --user " + btcAddress + "." + worker + " --pass x --port " + url.Split(':')[1]
            + " --server " + alg + ".br.nicehash.com"
            + " --user " + btcAddress + "." + worker + " --pass x --port " + url.Split(':')[1]
            + " --server " + url.Split(':')[0]
            + " --user " + btcAddress + "." + worker + " --pass x --port " + url.Split(':')[1]
            + " --telemetry=127.0.0.1:" + APIPort;
*/
                     
                        var ret = GetDevicesCommandString()
                            + " --server " + url.Split(':')[0]
                            + " --user " + btcAddress + "." + worker + " --pass x --port "
                            + url.Split(':')[1] + " --telemetry=127.0.0.1:" + APIPort + " --time --color";
             
            return ret;
        }

        protected override string GetDevicesCommandString() {
            string deviceStringCommand = " --dev ";
            foreach (var nvidia_pair in this.MiningSetup.MiningPairs) {
                deviceStringCommand += nvidia_pair.Device.ID + " ";

            }

            deviceStringCommand += " " + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA);

            return deviceStringCommand;
        }

        // benchmark stuff
        protected void KillMinerBase(string exeName) {
            foreach (Process process in Process.GetProcessesByName(exeName)) {
                try { process.Kill(); } catch (Exception e) { Helpers.ConsolePrint(MinerDeviceName, e.ToString()); }
            }
        }

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time) {
            CleanAllOldLogs();

            string server = Globals.GetLocationURL(algorithm.NiceHashID, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], this.ConectionType);
            string ret = " --logfile=benchmark_log.txt" + GetStartCommand(server, Globals.DemoUser, ConfigManager.GeneralConfig.WorkerName.Trim());
            benchmarkTimeWait = Math.Max(time * 3, 120);  // dstm takes a long time to get started
            return ret;
        }

        protected override void BenchmarkThreadRoutine(object CommandLine) {
            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            BenchmarkSignalQuit = false;
            BenchmarkSignalHanged = false;
            BenchmarkSignalFinnished = false;
            BenchmarkException = null;

            try {
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                Helpers.ConsolePrint(MinerTAG(), "Benchmark should end in : " + benchmarkTimeWait + " seconds");
                BenchmarkHandle = BenchmarkStartProcess((string)CommandLine);
                BenchmarkHandle.WaitForExit(benchmarkTimeWait + 2);
                Stopwatch _benchmarkTimer = new Stopwatch();
                _benchmarkTimer.Reset();
                _benchmarkTimer.Start();
                BenchmarkProcessStatus = BenchmarkProcessStatus.Running;
                bool keepRunning = true;
                while (keepRunning && IsActiveProcess(BenchmarkHandle.Id)) {
                    if (_benchmarkTimer.Elapsed.TotalSeconds >= (benchmarkTimeWait + 2)
                        || BenchmarkSignalQuit
                        || BenchmarkSignalFinnished
                        || BenchmarkSignalHanged
                        || BenchmarkSignalTimedout
                        || BenchmarkException != null) {

                        string imageName = MinerExeName.Replace(".exe", "");
                        // maybe will have to KILL process
                        KillMinerBase(imageName);
                        if (BenchmarkSignalTimedout) {
                            throw new Exception("Benchmark timedout");
                        }
                        if (BenchmarkException != null) {
                            throw BenchmarkException;
                        }
                        if (BenchmarkSignalQuit) {
                            throw new Exception("Termined by user request");
                        }
                        if (BenchmarkSignalFinnished) {
                            break;
                        }
                        keepRunning = false;
                        break;
                    } else {
                        // wait a second reduce CPU load
                        Thread.Sleep(1000);
                    }

                }
            } catch (Exception ex) {
                BenchmarkThreadRoutineCatch(ex);
            } finally {
                BenchmarkAlgorithm.BenchmarkSpeed = 0;
                // find latest log file
                Thread.Sleep(1000);
                string latestLogFile = "";
                var dirInfo = new DirectoryInfo(this.WorkingDirectory);
                foreach (var file in dirInfo.GetFiles("*_log.txt")) {
                    latestLogFile = file.Name;
                    break;
                }
                // read file log
                if (File.Exists(WorkingDirectory + latestLogFile)) {
                    var lines = new string[0];
                    var read = false;
                    var iteration = 0;
                    while (!read) {
                        if (iteration < 10) {
                            try {
                                lines = File.ReadAllLines(WorkingDirectory + latestLogFile);
                                read = true;
                                Helpers.ConsolePrint(MinerTAG(), "Successfully read log after " + iteration.ToString() + " iterations");
                            } catch (Exception ex) {
                                Helpers.ConsolePrint(MinerTAG(), ex.Message);
                                Thread.Sleep(1000);
                            }
                            iteration++;
                        } else {
                            read = true;  // Give up after 10s
                            Helpers.ConsolePrint(MinerTAG(), "Gave up on iteration " + iteration.ToString());
                        }
                    }

                    var addBenchLines = bench_lines.Count == 0;
                    foreach (var line in lines) {
                        if (line != null) {
                            bench_lines.Add(line);
                            string lineLowered = line.ToLower();
                            if (lineLowered.Contains(LOOK_FOR_START)) {
                                benchmark_sum += getNumber(lineLowered);
                                ++benchmark_read_count;
                            }
                        }
                    }
                    if (benchmark_read_count > 0) {
                        BenchmarkAlgorithm.BenchmarkSpeed = benchmark_sum / benchmark_read_count;
                    }
                }
                BenchmarkThreadRoutineFinish();
            }
        }

        protected void CleanAllOldLogs() {
            // clean old logs
            try {
                var dirInfo = new DirectoryInfo(this.WorkingDirectory);
                var deleteContains = "_log.txt";
                if (dirInfo != null && dirInfo.Exists) {
                    foreach (FileInfo file in dirInfo.GetFiles()) {
                        if (file.Name.Contains(deleteContains)) {
                            file.Delete();
                        }
                    }
                }
            } catch { }
        }

        // stub benchmarks read from file
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata) {
            CheckOutdata(outdata);
        }

        protected override bool BenchmarkParseLine(string outdata) {
          //  Helpers.ConsolePrint("BENCHMARK", outdata);
            return false;
        }

        protected double getNumber(string outdata) {
            return getNumber(outdata, LOOK_FOR_START, LOOK_FOR_END);
        }

        protected double getNumber(string outdata, string LOOK_FOR_START, string LOOK_FOR_END) {
            try {
                double mult = 1;
                int speedStart = outdata.IndexOf(LOOK_FOR_START);
                string speed = outdata.Substring(speedStart, outdata.Length - speedStart);
                speed = speed.Replace(LOOK_FOR_START, "");
             //   Helpers.ConsolePrint(MinerTAG(), speed);
                speed = speed.Substring(0, speed.IndexOf(LOOK_FOR_END));
             //   Helpers.ConsolePrint(MinerTAG(), speed);

                if (speed.Contains("k")) {
                    mult = 1000;
                    speed = speed.Replace("k", "");
                } else if (speed.Contains("m")) {
                    mult = 1000000;
                    speed = speed.Replace("m", "");
                }
                speed = speed.Trim();
                return (Double.Parse(speed, CultureInfo.InvariantCulture) * mult) * (1.0 - DevFee * 0.01);
            } catch (Exception ex) {
                Helpers.ConsolePrint("getNumber", ex.Message + " | args => " + outdata + " | " + LOOK_FOR_END + " | " + LOOK_FOR_START);
            }
            return 0;
        }

        public override async Task<APIData> GetSummaryAsync() {
            _currentMinerReadStatus = MinerAPIReadStatus.NONE;
            APIData ad = new APIData(MiningSetup.CurrentAlgorithmType);
            TcpClient client = null;
            dynamic resp = null;
            try {
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("{\"id\":0,\"jsonrpc\":\"2.0\",\"method\":\"sol_ps\"}n");
                client = new TcpClient("127.0.0.1", APIPort);
                NetworkStream nwStream = client.GetStream();
                await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead = await nwStream.ReadAsync(bytesToRead, 0, client.ReceiveBufferSize);
                string respStr = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                resp = JsonConvert.DeserializeObject(respStr);
             //   Helpers.ConsolePrint(MinerTAG(), "MINER RESPONCE:"+ respStr);
                client.Close();
            } catch (Exception ex) {
                Helpers.ConsolePrint(MinerTAG(), ex.Message);
            }
           // double speeds = 0;
            uint tmpSpeed = 0;
            if (resp != null && resp.error == null) {

                foreach (var result in resp.result)
                {
                    tmpSpeed = result.sol_ps;
                    ad.Speed += tmpSpeed;
                }
              //  ad.Speed = speeds;
                _currentMinerReadStatus = MinerAPIReadStatus.GOT_READ;
                if (ad.Speed == 0) {
                    _currentMinerReadStatus = MinerAPIReadStatus.READ_SPEED_ZERO;
                }
            }

            return ad;
        }

        protected override void _Stop(MinerStopType willswitch) {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        protected override int GET_MAX_CooldownTimeInMilliseconds() {
            return 60 * 1000 * 5; // 5 minute max, whole waiting time 75seconds
        }
    }
}