using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyDownloader.Core.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NiceHashMiner.Configs;
using NiceHashMiner.Miners.Parsing;
using NiceHashMiner.Devices;
using NiceHashMiner.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using System.Windows.Forms;

namespace NiceHashMiner.Miners
{
    public class WildRig : Miner
    {
        private readonly int GPUPlatformNumber;
        private int _benchmarkTimeWait = 120;
        //private const string _lookForStart = "speed 10s/60s/15m";
        private const string _lookForStart = "hashrate: 10s: ";
        //private const string _lookForEnd = "n/a kh/s max";
        private const string _lookForEnd = "60s:";
        private int count = 0;
        private string[,] myServers = Form_Main.myServers;

        public WildRig() : base("WildRig") {
            GPUPlatformNumber = ComputeDeviceManager.Available.AmdOpenCLPlatformNum;
        }

        public override void Start(string url, string btcAdress, string worker) {
            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            ProcessHandle = _Start();
        }
        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";

            var ids = MiningSetup.MiningPairs.OrderBy(mPair => mPair.Device.ID.ToString()).ToList();
            deviceStringCommand += string.Join(",", ids);

            return deviceStringCommand;
        }

        private string GetStartCommand(string url, string btcAdress, string worker) {
            var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
            var algo = "";
            var port = "";
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Skunk))
            {
                algo = "skunkhash";
                port = "3362";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16R))
            {
                algo = "x16r";
                port = "3366";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
            {
                algo = "x16rv2";
                port = "3379";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2REv3))
            {
                algo = "lyra2v3";
                port = "3373";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                algo = "kawpow";
                port = "3385";
            }

            string username = GetUsername(btcAdress, worker);
            return $" -a {algo} -o {url} -u {username}:x {extras} --api-port {ApiPort} "
                + $" -o stratum+tcp://{algo}.{myServers[1, 0]}.nicehash.com:{port} -u {username}:x "
                + $" -o stratum+tcp://{algo}.{myServers[2, 0]}.nicehash.com:{port} -u {username}:x "
                + $" -o stratum+tcp://{algo}.{myServers[3, 0]}.nicehash.com:{port} -u {username}:x "
                + $" -o stratum+tcp://{algo}.{myServers[4, 0]}.nicehash.com:{port} -u {username}:x "
                + "  --opencl-platforms=1 --opencl-devices=" + GetDevicesCommandString().TrimStart();
        }

        private string GetStartBenchmarkCommand(string url, string btcAdress, string worker)
        {
            var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
            var algo = "";
            var port = "";
            string username = GetUsername(btcAdress, worker);

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Skunk))
            {
                algo = "skunkhash";
                port = "3362";
                return $" -a {algo} -o stratum+tcp://skunk.eu.mine.zpool.ca:8433 -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2 -p c=BTC {extras} --api-port {ApiPort} "
               + $" -o stratum+tcp://{algo}.eu.nicehash.com:{port} -u {username}:x "
               + " --multiple-instance --opencl-devices=" + GetDevicesCommandString().TrimStart();
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16R))
            {
                algo = "x16r";
                port = "3366";
                return $" --benchmark -a {algo} -o stratum+tcp://x16r.eu.mine.zpool.ca:3636 -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2 -p c=BTC {extras} --api-port {ApiPort} "
               + $" -o stratum+tcp://{algo}.eu.nicehash.com:{port} -u {username}:x "
               + " --multiple-instance --opencl-devices=" + GetDevicesCommandString().TrimStart();
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
            {
                algo = "x16rv2";
                port = "3379";
                return $" --benchmark -a {algo} -o stratum+tcp://x16rv2.eu.mine.zpool.ca:3637 -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2 -p c=BTC {extras} --api-port {ApiPort} "
               + $" -o stratum+tcp://{algo}.eu.nicehash.com:{port} -u {username}:x "
               + " --multiple-instance --opencl-platforms=1 --opencl-devices=" + GetDevicesCommandString().TrimStart();
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2REv3))
            {
                algo = "lyra2rev3";
                port = "3366";
                return $" -a lyra2v3 -o stratum+tcp://lyra2v3.eu.mine.zpool.ca:4550 -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2 -p c=BTC {extras} --api-port {ApiPort} "
               + $" -o stratum+tcp://{algo}.eu.nicehash.com:{port} -u {username}:x "
               + " --multiple-instance --opencl-platforms=1 --opencl-devices=" + GetDevicesCommandString().TrimStart();
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                algo = "kawpow";
                port = "3385";
                return $" -a kawpow -o stratum+tcp://rvn.2miners.com:6060 -u RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.WildRig -p x {extras} --api-port {ApiPort} "
               + $" -o stratum+tcp://{algo}.eu.nicehash.com:{port} -u {username}:x "
               + " --multiple-instance --opencl-platforms=1 --opencl-devices=" + GetDevicesCommandString().TrimStart();
            }
            return "oops... strange algo";
        }

        protected override void _Stop(MinerStopType willswitch) {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        protected override int GetMaxCooldownTimeInMilliseconds() {
            return 60 * 1000 * 5;  // 5 min
        }

        public override async Task<ApiData> GetSummaryAsync()
        {
            return await GetSummaryCpuAsync();
        }

        protected override bool IsApiEof(byte third, byte second, byte last) {
            return third == 0x7d && second == 0xa && last == 0x7d;
        }

        #region Benchmark
        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time) {
            var server = Globals.GetLocationUrl(algorithm.NiceHashID,
                Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation],
                ConectionType);
            _benchmarkTimeWait = time;
            return GetStartBenchmarkCommand(server, Globals.GetBitcoinUser(), ConfigManager.GeneralConfig.WorkerName.Trim())
                + " -l "+ GetLogFileName()+ " --print-time=2";
        }

        protected override void BenchmarkThreadRoutine(object CommandLine) {
            BenchmarkThreadRoutineAlternate(CommandLine, _benchmarkTimeWait);
        }

        protected override void ProcessBenchLinesAlternate(string[] lines) {
            var twoSecTotal = 0d;
            var sixtySecTotal = 0d;
            var twoSecCount = 0;
            var sixtySecCount = 0;
            foreach (var line in lines) {
                BenchLines.Add(line);
                var lineLowered = line.ToLower();
                if (lineLowered.Contains(_lookForStart.ToLower())) {
                    /*
                    var speeds = Regex.Match(lineLowered, $"{_lookForStart.ToLower()} (.+?) {_lookForEnd.ToLower()}").Groups[1].Value.Split();
                    */
                    var speedStart = lineLowered.IndexOf(_lookForStart);
                    var speed = lineLowered.Substring(speedStart, lineLowered.Length - speedStart);
                    speed = speed.Replace(_lookForStart, "");
                    speed = speed.Substring(0, speed.IndexOf(_lookForEnd));
                    if (count >= 8 || (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2REv3) && count>=4 )) //пропустить первые 8
                    {
                        try
                        {
                            if (double.TryParse(speed, out var sixtySecSpeed))
                            {
                                sixtySecTotal += sixtySecSpeed;
                                ++sixtySecCount;
                            }
                            /*
                        if (double.TryParse(speeds[1], out var sixtySecSpeed)) {
                            sixtySecTotal += sixtySecSpeed;
                            ++sixtySecCount;
                            } else if (double.TryParse(speeds[0], out var twoSecSpeed)) {
                            // Store 2.5s data in case 60s is never reached
                            twoSecTotal += twoSecSpeed;
                            ++twoSecCount;
                            }
                            */
                        }

                        catch
                        {
                            MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            BenchmarkSignalFinnished = true;
                            return;
                        }
                    }
                    count++;
                }
            }

            if (sixtySecCount > 0 && sixtySecTotal > 0) {
                // Run iff 60s averages are reported
                BenchmarkAlgorithm.BenchmarkSpeed = (sixtySecTotal / sixtySecCount) * 1000;
            } else if (twoSecCount > 0) {
                // Run iff no 60s averages are reported but 2.5s are
                BenchmarkAlgorithm.BenchmarkSpeed = (twoSecTotal / twoSecCount) * 1000;
            }

        }

        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata) {
            CheckOutdata(outdata);
        }

        protected override bool BenchmarkParseLine(string outdata) {
            Helpers.ConsolePrint(MinerTag(), outdata);
            return false;
        }

        #endregion
    }
}
