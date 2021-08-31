using NiceHashMiner.Algorithms;
using NiceHashMiner.Benchmarking.BenchHelpers;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Interfaces;
using NiceHashMiner.Miners;
using NiceHashMiner.Miners.Grouping;
using NiceHashMinerLegacy.Common.Enums;
using System.Collections.Generic;
using System.Threading;

namespace NiceHashMiner.Benchmarking
{
    public class BenchmarkHandler : IBenchmarkComunicator
    {
        private readonly Queue<Algorithm> _benchmarkAlgorithmQueue;
        private readonly int _benchmarkAlgorithmsCount;
        private int _benchmarkCurrentIndex = -1;
        private readonly List<string> _benchmarkFailedAlgo = new List<string>();
        private readonly IBenchmarkForm _benchmarkForm;
        private Algorithm _currentAlgorithm;
        private Miner _currentMiner;
        private readonly BenchmarkPerformanceType _performanceType;
        private ClaymoreZcashBenchHelper _claymoreZcashStatus;

        private CpuBenchHelper _cpuBenchmarkStatus;

        private PowerHelper _powerHelper;

        // CPU sweet spots
        private readonly List<AlgorithmType> _cpuAlgos = new List<AlgorithmType>
        {
            AlgorithmType.RandomX
        };

        public BenchmarkHandler(ComputeDevice device, Queue<Algorithm> algorithms, IBenchmarkForm form,
            BenchmarkPerformanceType performance)
        {
            Device = device;
            _benchmarkAlgorithmQueue = algorithms;
            _benchmarkForm = form;
            _performanceType = performance;

            _benchmarkAlgorithmsCount = _benchmarkAlgorithmQueue.Count;
            _powerHelper = new PowerHelper(device);
        }

        public ComputeDevice Device { get; }

        public void Start()
        {
            var thread = new Thread(NextBenchmark);
            if (thread.Name == null)
                thread.Name = $"dev_{Device.DeviceType}-{Device.ID}_benchmark";
            thread.Start();
        }

        public void OnBenchmarkComplete(bool success, string status)
        {
            if (!_benchmarkForm.InBenchmark) return;

            var rebenchSame = false;
            if (success && _cpuBenchmarkStatus != null && _cpuAlgos.Contains(_currentAlgorithm.NiceHashID) &&
                _currentAlgorithm.MinerBaseType == MinerBaseType.XmrStak)
            {
                _cpuBenchmarkStatus.SetNextSpeed(_currentAlgorithm.BenchmarkSpeed);
                rebenchSame = _cpuBenchmarkStatus.HasTest();
                _currentAlgorithm.LessThreads = _cpuBenchmarkStatus.LessTreads;
                if (rebenchSame == false)
                {
                    _cpuBenchmarkStatus.FindFastest();
                    _currentAlgorithm.BenchmarkSpeed = _cpuBenchmarkStatus.GetBestSpeed();
                    _currentAlgorithm.LessThreads = _cpuBenchmarkStatus.GetLessThreads();
                }
            }

            var power = _powerHelper.Stop();
            /*
            var dualAlgo = _currentAlgorithm as DualAlgorithm;
            if (dualAlgo != null && dualAlgo.TuningEnabled)
            {
                dualAlgo.SetPowerForCurrent(power);

                if (dualAlgo.IncrementToNextEmptyIntensity())
                    rebenchSame = true;
            }
            else
            {
                _currentAlgorithm.PowerUsage = power;//**********power
            }
            */
            if (_currentAlgorithm.PowerUsageBenchmark > 0)
            {
                _currentAlgorithm.PowerUsage = _currentAlgorithm.PowerUsageBenchmark;//усредненное при бенчмарке
            }
            else
            {
                _currentAlgorithm.PowerUsage = power;
            }

            if (!rebenchSame) _benchmarkForm.RemoveFromStatusCheck(Device, _currentAlgorithm);

            if (!success && !rebenchSame)
            {
                // add new failed list
                _benchmarkFailedAlgo.Add(_currentAlgorithm.AlgorithmName);
                _benchmarkForm.SetCurrentStatus(Device, _currentAlgorithm, status);
            }
            else if (!rebenchSame)
            {
                // set status to empty string it will return speed
                _currentAlgorithm.ClearBenchmarkPending();
                _benchmarkForm.SetCurrentStatus(Device, _currentAlgorithm, "");
            }

            if (rebenchSame)
            {
                _powerHelper.Start();

                if (_cpuBenchmarkStatus != null)
                {
                    _currentMiner.BenchmarkStart(_cpuBenchmarkStatus.Time, this);
                }
                else if (_claymoreZcashStatus != null)
                {
                    _currentMiner.BenchmarkStart(_claymoreZcashStatus.Time, this);
                }
            }
            else
            {
                NextBenchmark();
            }
        }

        private void NextBenchmark()
        {
            ConfigManager.CommitBenchmarks();
            ++_benchmarkCurrentIndex;
            if (_benchmarkCurrentIndex > 0) _benchmarkForm.StepUpBenchmarkStepProgress();
            if (_benchmarkCurrentIndex >= _benchmarkAlgorithmsCount)
            {
                EndBenchmark();
                return;
            }

            if (_benchmarkAlgorithmQueue.Count > 0)
                _currentAlgorithm = _benchmarkAlgorithmQueue.Dequeue();

            if (Device != null && _currentAlgorithm != null)
            {
                _currentMiner = MinerFactory.CreateMiner(Device, _currentAlgorithm);
                _cpuBenchmarkStatus = null;

                if (_currentAlgorithm is DualAlgorithm dualAlgo && dualAlgo.TuningEnabled) dualAlgo.StartTuning();
            }

            if (_currentMiner != null && _currentAlgorithm != null && Device != null)
            {
                _currentMiner.InitBenchmarkSetup(new MiningPair(Device, _currentAlgorithm));

                var time = GetBenchmarktime(_performanceType, Device.DeviceGroupType);

                _benchmarkForm.AddToStatusCheck(Device, _currentAlgorithm);

                _currentMiner.BenchmarkStart(time, this);
                _powerHelper.Start();
            }
            else
            {
                NextBenchmark();
            }
        }

        private int GetBenchmarktime(BenchmarkPerformanceType benchmarkPerformanceType, DeviceGroupType deviceGroupType)
        {
            switch (benchmarkPerformanceType)
            {
                case BenchmarkPerformanceType.Standard:
                    return 60;
                case BenchmarkPerformanceType.Precise:
                    return 180;
                default:
                    return 60;
            }
            return 60;
        }

        private void EndBenchmark()
        {
            _currentAlgorithm?.ClearBenchmarkPending();
            _benchmarkForm.EndBenchmarkForDevice(Device, _benchmarkFailedAlgo.Count > 0);
        }

        public void InvokeQuit()
        {
            // clear benchmark pending status
            _currentAlgorithm?.ClearBenchmarkPending();
            if (_currentMiner != null)
            {
                _currentMiner.BenchmarkSignalQuit = true;
                _currentMiner.InvokeBenchmarkSignalQuit();
            }

            _currentMiner = null;
        }
    }
}
