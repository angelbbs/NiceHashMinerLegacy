/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Management;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using System.Threading;
using System.Threading.Tasks;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;

namespace NiceHashMiner.Miners
{
    class mkxminer : Miner
    {
        private readonly int GPUPlatformNumber;
        Stopwatch _benchmarkTimer = new Stopwatch();
        int count = 0;

        public mkxminer()
            : base("mkxminer_AMD")
        {
            GPUPlatformNumber = ComputeDeviceManager.Available.AmdOpenCLPlatformNum;
            IsKillAllUsedMinerProcs = true;
            IsNeverHideMiningWindow = true;

        }

        public void Killmkxminer() {

            // foreach (Process process in Process.GetProcessesByName("mkxminer")) {
            //     try { process.Kill(); } catch (Exception e) { Helpers.ConsolePrint(MinerDeviceName, e.ToString()); }
            // }

            if (ProcessHandle != null)
            {
                try { ProcessHandle.Kill(); }
                catch { }

                //try { ProcessHandle.SendCtrlC((uint)Process.GetCurrentProcess().Id); } catch { }
                ProcessHandle.Close();
                ProcessHandle = null;

                if (IsKillAllUsedMinerProcs) KillAllUsedMinerProcesses();
            }

        }

        private static void KillProcessAndChildren(int pid)
        {
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }
            Process proc;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                proc = Process.GetProcessById(pid);
                proc.Kill();
                /*
                Thread.Sleep(200);
                if (proc != null)
                {
                    foreach (Process process in Process.GetProcessesByName("mkxminer")) {
                         try { process.Kill(); } catch (Exception e) { Helpers.ConsolePrint("mkxminer-kill", e.ToString()); }
                    }
                }
                */
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }
        public override void EndBenchmarkProcces()
        {
            if (BenchmarkProcessStatus != BenchmarkProcessStatus.Killing && BenchmarkProcessStatus != BenchmarkProcessStatus.DoneKilling)
            {
                BenchmarkProcessStatus = BenchmarkProcessStatus.Killing;
                try
                {
                    Helpers.ConsolePrint("BENCHMARK",
                        $"Trying to kill benchmark process {BenchmarkProcessPath} algorithm {BenchmarkAlgorithm.AlgorithmName}");

                    int k = ProcessTag().IndexOf("pid(");
                    int i = ProcessTag().IndexOf(")|bin");
                    var cpid  = ProcessTag().Substring(k+4, i-k-4).Trim();

                    int pid = int.Parse(cpid, CultureInfo.InvariantCulture);

//                    ManagementObject mo = new ManagementObject("win32_process.handle='" + cpid + "'");
//                    mo.Get();
//                    int parentId = Convert.ToInt32(mo["ParentProcessId"]);

                    KillProcessAndChildren(pid);
                    BenchmarkHandle.Kill();
                    BenchmarkHandle.Close();
                    Killmkxminer();
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



protected override int GetMaxCooldownTimeInMilliseconds() {
            return 240;
        }

        protected override void _Stop(MinerStopType willswitch) {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
            //Killmkxminer();
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            if (!IsInit)
            {
                Helpers.ConsolePrint(MinerTag(), "MiningSetup is not initialized exiting Start()");
                return;
            }
            string username = GetUsername(btcAdress, worker);
            IsApiReadException = true; //** in miner
            //add failover
            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);

            LastCommandLine = " --asm --algorithm lyra2z -o " + url +
                              " -u " + username +
                              " -p x " +
                              " " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                              " -d ";

            LastCommandLine += GetDevicesCommandString();
            ProcessHandle = _Start();
        }

        // new decoupled benchmarking routines
        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time) {
            string CommandLine;

            string url = Globals.GetLocationUrl(algorithm.NiceHashID, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], this.ConectionType);

            // demo for benchmark
            string username = Globals.GetBitcoinUser() + "."+ ConfigManager.GeneralConfig.WorkerName.Trim();

            if (ConfigManager.GeneralConfig.WorkerName.Length > 0)
                username += "." + ConfigManager.GeneralConfig.WorkerName.Trim();

            // cd to the cgminer for the process bins

            CommandLine = " /C \"cd /d miners/mkxminer && mkxminer.exe " + " --algorithm lyra2z "+
                          " --url stratum+tcp://lyra2z.eu.mine.zpool.ca:4553" +
                          " --user 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" +
                          " -p c=BTC " +
                          ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                          " --device ";

            CommandLine += GetDevicesCommandString();
            //          CommandLine += " >benchmark.txt";
            //    CommandLine += " && del dump.txt\"";

            /*
                        CommandLine =           " --url " + url + "/#xnsub" +
                                                " --user " + Globals.GetBitcoinUser() +
                                                " -p x " +
                                                ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                                      MiningSetup,
                                                                                      DeviceType.AMD) +
                                                " --device ";

                        CommandLine += GetDevicesCommandString();
            */
            // CommandLine += " >benchmark.txt";
            return CommandLine;

        }

        protected override bool BenchmarkParseLine(string outdata) {
            string hashSpeed = "";
            int kspeed = 1;
            double speed;
            //> 0.32MH/s | Temp(C): 53 | Fan: - | HW: 0 | Rej: 0.0%
            // > Off Off 0.00MH/s Off Off
            //Accepted diff 2 share 111ce87a GPU#0 in 117ms
            try
            {
                if (outdata.Contains("Temp(C)"))
                {
                    //                int i = outdata.IndexOf("> ");
                    int k = outdata.IndexOf("H/s");
                    int i = k - 6;
                    hashSpeed = outdata.Substring(i, k - i - 1).Trim();
                    Helpers.ConsolePrint(hashSpeed, "");
                    if (outdata.Contains("H/s"))
                    {
                        kspeed = 1;
                    }
                    if (outdata.Contains("KH/s"))
                    {
                        kspeed = 1000;
                    }
                    if (outdata.Contains("MH/s"))
                    {
                        kspeed = 1000000;
                    }
                    speed = Double.Parse(hashSpeed, CultureInfo.InvariantCulture);
                    BenchmarkAlgorithm.BenchmarkSpeed = Math.Max(BenchmarkAlgorithm.BenchmarkSpeed, speed * kspeed);
                    return false;
                }
            }
            catch
            {
                MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                BenchmarkSignalFinnished = true;
                return false;
            }

            if (outdata.Contains("Accepted"))
            {
                count++;
            }

            if (outdata.Contains("Accepted") & BenchmarkAlgorithm.BenchmarkSpeed != 0 & count >= 15 ) //need more time
            {
                Helpers.ConsolePrint("BENCHMARK", "BenchmarkAlgorithm.BenchmarkSpeed ="+ BenchmarkAlgorithm.BenchmarkSpeed.ToString());
               // BenchmarkSignalHanged = true;
               // BenchmarkSignalFinnished = true;
                return true;
            }

            return false;

        }

        protected override void BenchmarkThreadRoutineStartSettup() {
            AlgorithmType nhDataIndex = BenchmarkAlgorithm.NiceHashID;

            if (!NHSmaData.HasData)
            {
                Helpers.ConsolePrint("BENCHMARK", "Skipping mkxminer benchmark because there is no internet " +
                    "connection. mkxminer needs internet connection to do benchmarking.");

                throw new Exception("No internet connection");
            }

            NHSmaData.TryGetPaying(nhDataIndex, out var paying);
            if (paying == 0)
            {
                Helpers.ConsolePrint("BENCHMARK", "Skipping mkxminer benchmark because there is no work on Nicehash.com " +
                    "[algo: " + BenchmarkAlgorithm.AlgorithmName + "(" + nhDataIndex + ")]");

                throw new Exception("No work can be used for benchmarking");
            }
            _benchmarkTimer.Reset();
            _benchmarkTimer.Start();
            // call base, read only outpus
            //BenchmarkHandle.BeginOutputReadLine();


        }
        //майнер не закрывается после бенчмарка
        //проверить время, за которое проходит бенчмарк и получает ли максимальный хешрейт
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            //  _benchmarkTimer.Stop();
            // this is safe in a benchmark
            // Killmkxminer();
            // BenchmarkSignalHanged = true;

            if (_benchmarkTimer.Elapsed.TotalSeconds >= 420)
            {
                Helpers.ConsolePrint("mkxminer benchmark timer end", "");
                _benchmarkTimer.Stop();
                // this is safe in a benchmark
                //Killmkxminer();
                BenchmarkSignalHanged = true;
                BenchmarkSignalFinnished = true;
            }
            if (!BenchmarkSignalFinnished && outdata != null)
            {
                CheckOutdata(outdata);
            }


        }

        protected override string GetFinalBenchmarkString() {
            if (BenchmarkAlgorithm.BenchmarkSpeed <= 0) {
                Helpers.ConsolePrint("mkxminer_GetFinalBenchmarkString", International.GetText("mkxminer_precise_try"));
                return International.GetText("mkxminer_precise_try");
            }

            return base.GetFinalBenchmarkString();
        }

        protected override void BenchmarkThreadRoutine(object CommandLine) {
            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS * 3); // increase wait for sgminer

            BenchmarkSignalQuit = false;
            BenchmarkSignalHanged = false;
            BenchmarkSignalFinnished = false;
            BenchmarkException = null;

            try {
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                BenchmarkHandle = BenchmarkStartProcess((string)CommandLine);
                BenchmarkThreadRoutineStartSettup();
                // wait a little longer then the benchmark routine if exit false throw
                //var timeoutTime = BenchmarkTimeoutInSeconds(BenchmarkTimeInSeconds);
                //var exitSucces = BenchmarkHandle.WaitForExit(timeoutTime * 1000);
                // don't use wait for it breaks everything
                BenchmarkProcessStatus = BenchmarkProcessStatus.Running;
                while(true) {
                    string outdata = BenchmarkHandle.StandardOutput.ReadLine();
                    BenchmarkOutputErrorDataReceivedImpl(outdata);
                    // terminate process situations


                    if (BenchmarkSignalQuit
                        || BenchmarkSignalFinnished
                        || BenchmarkSignalHanged
                        || BenchmarkSignalTimedout
                        || BenchmarkException != null) {
                        Helpers.ConsolePrint(MinerTag(), ProcessTag() + " BenchmarkSignalQuit: ");
                        EndBenchmarkProcces();
                        // this is safe in a benchmark
                        Killmkxminer();
                        if (BenchmarkSignalTimedout) {
                            throw new Exception("Benchmark timedout");
                        }
                        if (BenchmarkException != null) {
                            throw BenchmarkException;
                        }
                        if (BenchmarkSignalQuit) {
                            throw new Exception("Termined by user request");
                        }
                        if (BenchmarkSignalHanged) {
                            throw new Exception("mkxminer is not responding");
                        }
                        if (BenchmarkSignalFinnished) {
                            break;
                        }
                    } else {

                        // wait a second reduce CPU load
                        Thread.Sleep(1000);
                    }
                }
            } catch (Exception ex) {
                BenchmarkThreadRoutineCatch(ex);
            } finally {
                BenchmarkThreadRoutineFinish();
            }
        }

        #endregion // Decoupled benchmarking routines

        // TODO _currentMinerReadStatus
        public override async Task<ApiData> GetSummaryAsync()
        {
            //if (!IsApiReadException) return await GetSummaryCpuCcminerAsync();
            // check if running
            if (ProcessHandle == null)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Could not read data from mkxminer Proccess is null");
                return null;
            }
            try
            {
                Process.GetProcessById(ProcessHandle.Id);
            }
            catch (ArgumentException ex)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Could not read data from mkxminer reason: " + ex.Message);
                return null; // will restart outside
            }
            catch (InvalidOperationException ex)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Could not read data from mkxminer reason: " + ex.Message);
                return null; // will restart outside
            }
            /*
            var totalSpeed = MiningSetup.MiningPairs
                .Select(miningPair =>
                    miningPair.Device.GetAlgorithm(MinerBaseType.lyclMiner, AlgorithmType.Lyra2REv2, AlgorithmType.NONE))
                .Where(algo => algo != null).Sum(algo => algo.BenchmarkSpeed);
                */
            var totalSpeed = 0.0d;
            foreach (var miningPair in MiningSetup.MiningPairs)
            {
                var algo = miningPair.Device.GetAlgorithm(MinerBaseType.mkxminer, AlgorithmType.Lyra2z, AlgorithmType.NONE);
                if (algo != null)
                {
                    totalSpeed += algo.BenchmarkSpeed;
                }
            }

            var mkxminerData = new ApiData(MiningSetup.CurrentAlgorithmType)
            {
                Speed = totalSpeed
            };
            CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
            // check if speed zero
            if (mkxminerData.Speed == 0) CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
            return mkxminerData;
        }

    }
}
