/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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
    public class XmrigAMD : Miner
    {
        private readonly int GPUPlatformNumber;
        private int _benchmarkTimeWait = 180;
        private const string _lookForStart = "speed 10s/60s/15m";
        private const string _lookForEnd = "n/a h/s max";
        private string[,] myServers = Form_Main.myServers;

        public XmrigAMD() : base("XmrigAMD") {
            GPUPlatformNumber = ComputeDeviceManager.Available.AmdOpenCLPlatformNum;
        }

        public override void Start(string url, string btcAdress, string worker) {
            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            ProcessHandle = _Start();
        }

        private string GetStartCommand(string url, string btcAdress, string worker) {
            string nhsuff = "";
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }
            var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
            var algo = "cryptonightv7";
            var port = "3363";
            var variant = " --variant 1 ";
            string username = GetUsername(btcAdress, worker);
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightV8))
            {
                algo = "cryptonightv8";
                port = "3367";
                variant = " --variant 2 ";
                return $" -o {url} {variant} -u {username}:x --nicehash {extras} --api-port {ApiPort} --donate-level=1"
                + $" -o stratum+tcp://{algo}.{myServers[1, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[2, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[3, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[4, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[5, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[0, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + " --opencl-devices=" + GetDevicesCommandString().TrimStart();
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightHeavy))
            {
                algo = "cryptonightheavy";
                port = "3364";
                variant = " --variant 2 ";
                return $" --algo=cryptonight-heavy -o {url} {variant} -u {username}:x --nicehash {extras} --api-port {ApiPort} --donate-level=1"
                + $" -o stratum+tcp://{algo}.{myServers[1, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[2, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[3, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[4, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[5, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[0, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + " --opencl-devices=" + GetDevicesCommandString().TrimStart();
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightR))
            {
                algo = "cryptonightr";
                port = "3375";
                variant = " --variant 2 ";
                return $" --algo=cryptonight/r -o {url} -u {username}:x --nicehash {extras} --api-port {ApiPort} --donate-level=1"
                + $" -o stratum+tcp://{algo}.{myServers[1, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[2, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[3, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[4, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[5, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[0, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + " --opencl-devices=" + GetDevicesCommandString().TrimStart();
            }
            return $" -a {algo} -o {url} {variant} -u {username}:x --nicehash {extras} --api-port {ApiPort} --donate-level=1"
                + $" -o stratum+tcp://{algo}.{myServers[1, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x "
                + $" -o stratum+tcp://{algo}.{myServers[2, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x "
                + $" -o stratum+tcp://{algo}.{myServers[3, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x "
                + $" -o stratum+tcp://{algo}.{myServers[4, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x "
                + $" -o stratum+tcp://{algo}.{myServers[5, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x "
                + $" -o stratum+tcp://{algo}.{myServers[6, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x "
                + " --opencl-devices=" + GetDevicesCommandString().TrimStart();
        }

        private string GetStartBenchmarkCommand(string url, string btcAdress, string worker)
        {
            string nhsuff = "";
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }
            if (url.Contains("Auto"))
            {
                url = url.Replace("Auto", "eu" + nhsuff);
            }
            var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
            var algo = "cryptonightv7";
            var port = "3363";
            var variant = " --variant 1 ";
            string username = GetUsername(btcAdress, worker);
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightV8))
            {
                algo = "cryptonightv8";
                port = "3367";
                variant = " --variant 2 ";
                return $" -o {url} {variant} -u {username}:x --nicehash {extras} --api-port {ApiPort} --donate-level=1"
                + $" -o stratum+tcp://{algo}.{myServers[1, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[2, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[3, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[4, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[5, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[0, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x --nicehash"
                + " --opencl-devices=" + GetDevicesCommandString().TrimStart() + " --opencl-platform=" + GPUPlatformNumber;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightHeavy))
            {
                algo = "cryptonightheavy";
                port = "3364";
                variant = " --variant 2 ";
                return $" --algo=cryptonight-heavy -o {url} {variant} -u {username}:x --nicehash {extras} --api-port {ApiPort} --donate-level=1 "
                + $" -o stratum+tcp://{algo}.{myServers[1, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[2, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[3, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[4, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[5, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[0, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + " --opencl-devices=" + GetDevicesCommandString().TrimStart() + " --opencl-platform=" + GPUPlatformNumber;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightR))
            {
                algo = "cryptonightr";
                port = "3375";
                variant = " --variant 2 ";
                return $" --algo=cryptonight/r -o stratum+tcp://xmr-eu1.nanopool.org:14444 -u 42fV4v2EC4EALhKWKNCEJsErcdJygynt7RJvFZk8HSeYA9srXdJt58D9fQSwZLqGHbijCSMqSP4mU7inEEWNyer6F7PiqeX.{worker} -p x {extras} --api-port {ApiPort} --donate-level=1 "
                + $" -o stratum+tcp://{algo}.{myServers[1, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[2, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[3, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + $" -o stratum+tcp://{algo}.{myServers[4, 0]}{nhsuff}.nicehash.com:{port} -u {username}:x --nicehash"
                + " --opencl-devices=" + GetDevicesCommandString().TrimStart() + " --opencl-platform=" + GPUPlatformNumber;
            }
            return $" -o {url} {variant} -u {username}:x --nicehash {extras} --api-port {ApiPort} --donate-level=1"
                + $" -o stratum+tcp://{algo}.{myServers[1, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x "
                + $" -o stratum+tcp://{algo}.{myServers[2, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x "
                + $" -o stratum+tcp://{algo}.{myServers[3, 0]}.{nhsuff}nicehash.com:{port} {variant} -u {username}:x "
                + $" -o stratum+tcp://{algo}.{myServers[4, 0]}{nhsuff}.nicehash.com:{port} {variant} -u {username}:x "
                + " --opencl-devices=" + GetDevicesCommandString().TrimStart() + " --opencl-platform=" + GPUPlatformNumber;
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
            // Xmrig reports 2.5s and 60s averages, so prefer to use 60s values for benchmark
            // but fall back on 2.5s values if 60s time isn't hit
            var twoSecTotal = 0d;
            var sixtySecTotal = 0d;
            var twoSecCount = 0;
            var sixtySecCount = 0;
            foreach (var line in lines) {
                BenchLines.Add(line);
                var lineLowered = line.ToLower();
                if (lineLowered.Contains(_lookForStart.ToLower())) {
                    var speeds = Regex.Match(lineLowered, $"{_lookForStart.ToLower()} (.+?) {_lookForEnd.ToLower()}").Groups[1].Value.Split();

                    try {
                    if (double.TryParse(speeds[1], out var sixtySecSpeed)) {
                        sixtySecTotal += sixtySecSpeed;
                        ++sixtySecCount;
                        } else if (double.TryParse(speeds[0], out var twoSecSpeed)) {
                        // Store 2.5s data in case 60s is never reached
                        twoSecTotal += twoSecSpeed;
                        ++twoSecCount;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return;
                    }
                }
            }

            if (sixtySecCount > 0 && sixtySecTotal > 0) {
                // Run iff 60s averages are reported
                BenchmarkAlgorithm.BenchmarkSpeed = sixtySecTotal / sixtySecCount;
            } else if (twoSecCount > 0) {
                // Run iff no 60s averages are reported but 2.5s are
                BenchmarkAlgorithm.BenchmarkSpeed = twoSecTotal / twoSecCount;
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
