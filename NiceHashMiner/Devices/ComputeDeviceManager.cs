using ATI.ADL;
using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMiner.Interfaces;
using NVIDIA.NVAPI;
using ManagedCuda.Nvml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using NiceHashMiner.Devices.Querying;
using NiceHashMiner.Forms;
using NiceHashMinerLegacy.Common.Enums;
using System.Threading.Tasks;
using MSI.Afterburner;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace NiceHashMiner.Devices
{
    /// <summary>
    /// ComputeDeviceManager class is used to query ComputeDevices avaliable on the system.
    /// Query CPUs, GPUs [Nvidia, AMD]
    /// </summary>
    public static class ComputeDeviceManager
    {
        #region JSON settings
        private static JsonSerializerSettings _jsonSettings;

        public static void DeviceDetectionPrinter()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Culture = CultureInfo.InvariantCulture
            };
        }
        #endregion JSON settings
        private static int CUDAQueryCount = 1;
        public static int CudaDevicesCountFromNVMLHost = 0;
        private static string nvmlRootPath = "";

        //�������, ����� � nvidia ������� ���� deviceID �� ��������� � �������� busID (1230 - 0123)
        public static List<ComputeDevice> ReSortDevices(List<ComputeDevice> computeDevices)
        {
            //separate 
            List<ComputeDevice> _computeDevices = computeDevices;
            List<ComputeDevice> _computeDevicesCPU = new List<ComputeDevice>();
            List<ComputeDevice> _computeDevicesAMD = new List<ComputeDevice>();
            List<ComputeDevice> _computeDevicesNVIDIA = new List<ComputeDevice>();

            Form_Main.PowerAllDevices = 0;
            foreach (var cpu in _computeDevices)
            {
                if (cpu.DeviceType == DeviceType.CPU)
                {
                    _computeDevicesCPU.Add(cpu);
                    Form_Main.PowerAllDevices += cpu.PowerUsage;
                }
            }
            foreach (var amd in _computeDevices)
            {
                if (amd.DeviceType == DeviceType.AMD)
                {
                    _computeDevicesAMD.Add(amd);
                    Form_Main.PowerAllDevices += amd.PowerUsage;
                }
            }
            foreach (var nvidia in _computeDevices)
            {
                if (nvidia.DeviceType == DeviceType.NVIDIA)
                {
                    _computeDevicesNVIDIA.Add(nvidia);
                    Form_Main.PowerAllDevices += nvidia.PowerUsage;
                }
            }

            if (Form_Main.NVIDIA_orderBug)//������� ��-�� ������������ ��������� ����
            {
                _computeDevicesNVIDIA.Sort((a, b) => a.ID.CompareTo(b.ID));
            }

            List<ComputeDevice> all = new List<ComputeDevice>();

            all.AddRange(_computeDevicesCPU);
            all.AddRange(_computeDevicesNVIDIA);
            all.AddRange(_computeDevicesAMD);

            return all;
        }
        public static class Query
        {
            private const string Tag = "ComputeDeviceManager.Query";

            // format 372.54;
            public class NvidiaSmiDriver
            {
                public NvidiaSmiDriver(int left, int right)
                {
                    LeftPart = left;
                    _rightPart = right;
                }

                public bool IsLesserVersionThan(NvidiaSmiDriver b)
                {
                    if (LeftPart < b.LeftPart)
                    {
                        return true;
                    }
                    return LeftPart == b.LeftPart && GetRightVal(_rightPart) < GetRightVal(b._rightPart);
                }

                public override string ToString()
                {
                    return $"{LeftPart}.{_rightPart}";
                }

                public readonly int LeftPart;
                private readonly int _rightPart;

                private static int GetRightVal(int val)
                {
                    if (val >= 10)
                    {
                        return val;
                    }
                    return val * 10;
                }
            }

            private static readonly NvidiaSmiDriver NvidiaRecomendedDriver = new NvidiaSmiDriver(456, 71);
            private static readonly NvidiaSmiDriver NvidiaMinDetectionDriver = new NvidiaSmiDriver(456, 71);
            public static NvidiaSmiDriver _currentNvidiaSmiDriver = new NvidiaSmiDriver(-1, -1);
            private static readonly NvidiaSmiDriver InvalidSmiDriver = new NvidiaSmiDriver(-1, -1);

            private static readonly NvidiaSmiDriver NvidiaCuda92Driver = new NvidiaSmiDriver(398, 26);
            private static readonly NvidiaSmiDriver NvidiaCuda10Driver = new NvidiaSmiDriver(411, 31);
            private static readonly NvidiaSmiDriver NvidiaCuda101Driver = new NvidiaSmiDriver(418, 96);
            private static readonly NvidiaSmiDriver NvidiaCuda11Driver = new NvidiaSmiDriver(451, 48);
            private static readonly NvidiaSmiDriver NvidiaCuda111Driver = new NvidiaSmiDriver(456, 71);//457.51 last worked driver
            public static readonly NvidiaSmiDriver LastGoodNvidiaCuda111Driver = new NvidiaSmiDriver(457, 51);//457.51 last worked driver

            public static string CUDA_version;
            // naming purposes
            public static int CpuCount = 0;

            public static int GpuCount = 0;

            

            private static NvidiaSmiDriver GetNvidiaSmiDriver()
            {
                if (WindowsDisplayAdapters.HasNvidiaVideoController())
                {
                    string stdErr;
                    string args;
                    var stdOut = stdErr = args = string.Empty;
                    var smiPath = nvmlRootPath + "\\nvidia-smi.exe";
                    try
                    {
                        var P = new Process
                        {
                            StartInfo =
                            {
                                FileName = smiPath,
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                        };
                        P.Start();
                        P.WaitForExit(30 * 1000);

                        stdOut = P.StandardOutput.ReadToEnd();
                        stdErr = P.StandardError.ReadToEnd();

                        const string findString = "Driver Version: ";
                        using (var reader = new StringReader(stdOut))
                        {
                            var line = string.Empty;
                            do
                            {
                                line = reader.ReadLine();
                                if (line != null && line.Contains(findString))
                                {
                                    var start = line.IndexOf(findString);
                                    var driverVer = line.Substring(start, start + 7);
                                    driverVer = driverVer.Replace(findString, "").Substring(0, 7).Trim();
                                    var drVerDouble = double.Parse(driverVer, CultureInfo.InvariantCulture);
                                    var dot = driverVer.IndexOf(".");
                                    var leftPart = int.Parse(driverVer.Substring(0, 3));
                                    var rightPart = int.Parse(driverVer.Substring(4, 2));
                                    return new NvidiaSmiDriver(leftPart, rightPart);
                                }
                            } while (line != null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint(Tag, "GetNvidiaSMIDriver Exception: " + ex.Message);
                        return InvalidSmiDriver;
                    }
                }
                return InvalidSmiDriver;
            }
            /*
            private static void ShowMessageAndStep(string infoMsg)
            {
                MessageNotifier?.SetMessageAndIncrementStep(infoMsg);
            }
            */
            private static void SetValueAndMsg(int num, string infoMsg)
            {
                MessageNotifier?.SetValueAndMsg(num, infoMsg);
            }
            public static IMessageNotifier MessageNotifier { get; private set; }

            public static int CheckVideoControllersCountMismath()
            {
                int ret = -1;
                Helpers.ConsolePrint("ComputeDeviceManager.CheckCount", "1");
                if (!WindowsDisplayAdapters.HasNvidiaVideoController()) return ret;

                var gpusOld = _cudaDevices.CudaDevices.Count;
                var gpusNew = CudaDevicesCountFromNVMLHost;

                Helpers.ConsolePrint("ComputeDeviceManager.CheckCount",
                    "CUDA GPUs count: Old: " + gpusOld + " / New: " + gpusNew);

                if (gpusOld == gpusNew)
                {
                    CUDAQueryCount = 0;
                    return ret;
                }
                else
                {

                    foreach (var dev in _cudaDevices.CudaDevices)
                    {
                        int devn = (int)dev.DeviceID;
                        nvmlDevice _nvmlDevice = new nvmlDevice();
                        foreach (var devNVML in Form_Main.gpuList)
                        {
                            var retNVML = NvmlNativeMethods.nvmlDeviceGetHandleByIndex((uint)devn, ref _nvmlDevice);
                            if (retNVML != nvmlReturn.Success)
                            {
                                ret = devn;
                                Helpers.ConsolePrint("ComputeDeviceManager.CheckCount", "GPU#" + devn.ToString() + " error");
                                break;
                            }
                        }
                    }
                    CUDAQueryCount++;
                }
                if (CUDAQueryCount >= 2)
                {
                    return ret;
                }
                return -1;
            }
            private static bool TryAddNvmlToEnvPath()
            {
                string defaultpath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) +
                                   "\\NVIDIA Corporation\\NVSMI";
                nvmlRootPath = defaultpath;
                /*
                nvmlRootPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows) +
               "\\System32";
                Helpers.ConsolePrint(Tag, $"Adding NVML to PATH='{nvmlRootPath}'");
                if (Directory.Exists(nvmlRootPath))
                {
                    var pathVar = Environment.GetEnvironmentVariable("PATH");
                    pathVar += ";" + nvmlRootPath;
                    Environment.SetEnvironmentVariable("PATH", pathVar);
                    return true;
                }
                */
                    
                    if (!File.Exists(Path.Combine(nvmlRootPath, "nvml.dll")) || !File.Exists(Path.Combine(nvmlRootPath, "nvidia-smi.exe")))
                    {
                        nvmlRootPath = GetNVMLFiles();
                        if (!Directory.Exists(defaultpath))
                        {
                            try
                            {
                                Directory.CreateDirectory(defaultpath);
                            }
                            catch (Exception e)
                            {
                                Helpers.ConsolePrint(Tag, "CreateDirectory failed: " + e.Message);
                            }
                        }
                        if (File.Exists(nvmlRootPath + "\\nvidia-smi.exe") || !File.Exists(defaultpath + "\\nvidia-smi.exe"))
                        {
                            try
                            {
                                var copyToPath = defaultpath + "\\nvidia-smi.exe";
                                File.Copy(nvmlRootPath + "\\nvidia-smi.exe", copyToPath, true);
                                Helpers.ConsolePrint(Tag, $"Copy from {nvmlRootPath + "\\nvidia-smi.exe"} to {copyToPath} done");
                            }
                            catch (Exception e)
                            {
                                Helpers.ConsolePrint(Tag, "Copy nvidia-smi.exe failed: " + e.Message);
                            }
                        }
                        if (File.Exists(nvmlRootPath + "\\nvml.dll") || !File.Exists(defaultpath + "\\nvml.dll"))
                        {
                            try
                            {
                                var copyToPath = defaultpath + "\\nvml.dll";
                                File.Copy(nvmlRootPath + "\\nvml.dll", copyToPath, true);
                                Helpers.ConsolePrint(Tag, $"Copy from {nvmlRootPath + "\\\nvml.dll"} to {copyToPath} done");
                                nvmlRootPath = defaultpath;
                            }
                            catch (Exception e)
                            {
                                Helpers.ConsolePrint(Tag, "Copy nvml.dll failed: " + e.Message);
                            }
                        }
                    }

                    if (File.Exists(defaultpath + "\\nvml.dll"))
                    {
                        Helpers.ConsolePrint(Tag, $"Adding NVML to PATH='{nvmlRootPath}'");
                        if (Directory.Exists(nvmlRootPath))
                        {
                            var pathVar = Environment.GetEnvironmentVariable("PATH");
                            pathVar += ";" + nvmlRootPath;
                            Environment.SetEnvironmentVariable("PATH", pathVar);
                            return true;
                        }
                    } else
                    {
                        Helpers.ConsolePrint(Tag, "Warning! nvml.dll not found!");
                        return false;
                    }
                    
                    return false;
            }

            private static string GetNVMLFiles()
            {
                DateTime dt = new DateTime();
                string pathToFiles = null;
                string DriverFolder = "C:\\Windows\\System32\\DriverStore\\FileRepository";
                try
                {
                    string[] folders = Directory.GetDirectories(DriverFolder);
                    foreach (string folder in folders)
                    {
                        string[] files = Directory.GetFiles(folder);
                        foreach (string filename in files)
                        {
                            if (filename.Contains("nvml.dll"))
                            {
                                FileInfo fi = new FileInfo(filename);
                                if (DateTime.Compare(fi.CreationTime, dt) > 0)
                                {
                                    dt = fi.CreationTime;
                                    pathToFiles = folder;
                                }
                            }
                        }

                    }
                }
                catch (System.Exception e)
                {
                    Helpers.ConsolePrint("GetNVMLFiles", e.ToString());
                }
                return pathToFiles;
            }

            public static void QueryDevices(IMessageNotifier messageNotifier)
            {
                WindowsDisplayAdapters.QueryVideoControllers();
                Helpers.ConsolePrint(Tag, "HasNvidiaVideoController: " + WindowsDisplayAdapters.HasNvidiaVideoController());
                if (WindowsDisplayAdapters.HasNvidiaVideoController())
                {

                    if (TryAddNvmlToEnvPath())
                    {
                        nvmlReturn nvmlLoaded = NvmlNativeMethods.nvmlInit();
                        //Helpers.ConsolePrint(Tag, "nvmlLoaded: " + nvmlLoaded);
                        if (nvmlLoaded != nvmlReturn.Success)
                        {
                            int check = ComputeDeviceManager.Query.CheckVideoControllersCountMismath();
                            Helpers.ConsolePrint(Tag, "NVSMI Error: " + nvmlLoaded);
                            if ((int)nvmlLoaded == 6 || (int)nvmlLoaded == 15 || (int)nvmlLoaded == 16)
                            {
                                if (ConfigManager.GeneralConfig.RestartWindowsOnCUDA_GPU_Lost)
                                {
                                    var onGpusLost = new ProcessStartInfo(Directory.GetCurrentDirectory() + "\\OnGPUsLost.bat")
                                    {
                                        WindowStyle = ProcessWindowStyle.Minimized
                                    };
                                    onGpusLost.Arguments = "1 " + check;
                                    Helpers.ConsolePrint("ERROR", "Restart Windows due CUDA GPU#" + check.ToString() + " is lost");
                                    Process.Start(onGpusLost);
                                    Thread.Sleep(2000);
                                }
                                if (ConfigManager.GeneralConfig.RestartDriverOnCUDA_GPU_Lost)
                                {
                                    var onGpusLost = new ProcessStartInfo(Directory.GetCurrentDirectory() + "\\OnGPUsLost.bat")
                                    {
                                        WindowStyle = ProcessWindowStyle.Minimized
                                    };
                                    onGpusLost.Arguments = "2 " + check;
                                    Helpers.ConsolePrint("ERROR", "Restart driver due CUDA GPU#" + check.ToString() + " is lost");
                                    Form_Benchmark.RunCMDAfterBenchmark();
                                    Thread.Sleep(1000);
                                    Process.Start(onGpusLost);
                                    Thread.Sleep(2000);
                                }
                                MessageBox.Show("NVSMI Error: " + nvmlLoaded + ". Please restart NVIDIA driver","ERROR!");
                            }
                        }
                    }
                }

                MessageNotifier = messageNotifier;

                // Order important CPU Query must be first
                // #1 CPU
                if (!ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionCPU)
                {
                    Cpu.QueryCpus();
                }
                // #2 CUDA
                if (Nvidia.IsSkipNvidia())
                {
                    Helpers.ConsolePrint(Tag, "Skipping NVIDIA device detection, settings are set to disabled");
                }
                else
                {
                    SetValueAndMsg(2, International.GetText("Compute_Device_Query_Manager_CUDA_Query"));
                    Nvidia.QueryCudaDevices();
                }
                // OpenCL and AMD
                if (ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionAMD)
                {
                    Helpers.ConsolePrint(Tag, "Skipping AMD device detection, settings set to disabled");
                    //SetValueAndMsg(3, International.GetText("Compute_Device_Query_Manager_AMD_Query_Skip"));
                }
                else
                {
                    // #3 OpenCL
                    //ShowMessageAndStep(International.GetText("Compute_Device_Query_Manager_OpenCL_Query"));
                    SetValueAndMsg(3, International.GetText("Compute_Device_Query_Manager_OpenCL_Query"));
                    OpenCL.QueryOpenCLDevices();
                    // #4 AMD query AMD from OpenCL devices, get serial and add devices
                    //ShowMessageAndStep(International.GetText("Compute_Device_Query_Manager_AMD_Query"));
                    SetValueAndMsg(3, International.GetText("Compute_Device_Query_Manager_AMD_Query"));
                    var amd = new AmdQuery(AvaliableVideoControllers);
                    AmdDevices = amd.QueryAmd(_isOpenCLQuerySuccess, _openCLJsonData);
                }
                // #5 uncheck CPU if GPUs present, call it after we Query all devices
                Group.UncheckedCpu();

                // TODO update this to report undetected hardware
                // #6 check NVIDIA, AMD devices count
                var nvidiaCount = 0;
                {
                    var amdCount = 0;
                    foreach (var vidCtrl in AvaliableVideoControllers)
                    {
                        if (vidCtrl.Name.ToLower().Contains("nvidia") && CudaUnsupported.IsSupported(vidCtrl.Name))
                        {
                            nvidiaCount += 1;
                        }
                        else if (vidCtrl.Name.ToLower().Contains("nvidia"))
                        {
                            Helpers.ConsolePrint(Tag,
                                "Device not supported NVIDIA/CUDA device not supported " + vidCtrl.Name);
                        }
                        amdCount += (vidCtrl.Name.ToLower().Contains("amd") || vidCtrl.Name.ToLower().Contains("radeon")) ? 1 : 0;
                    }
                    Helpers.ConsolePrint(Tag,
                        nvidiaCount == _cudaDevices.CudaDevices.Count
                            ? "Cuda NVIDIA/CUDA device count GOOD"
                            : "Cuda NVIDIA/CUDA device count BAD!!!");
                    Helpers.ConsolePrint(Tag,
                        amdCount == AmdDevices.Count ? "AMD GPU device count GOOD" : "AMD GPU device count BAD!!! " +
                        amdCount.ToString() + " " + AmdDevices.Count.ToString());
                }
                // allerts
                _currentNvidiaSmiDriver = GetNvidiaSmiDriver();
                if (!_currentNvidiaSmiDriver.IsLesserVersionThan(NvidiaCuda111Driver))
                {
                    CUDA_version = "CUDA 11.1";
                }
                if (!_currentNvidiaSmiDriver.IsLesserVersionThan(NvidiaCuda11Driver))
                {
                    CUDA_version = "CUDA 11.0";
                }
                if (!_currentNvidiaSmiDriver.IsLesserVersionThan(NvidiaCuda101Driver))
                {
                    CUDA_version = "CUDA 10.1";
                }
                if (_currentNvidiaSmiDriver.IsLesserVersionThan(NvidiaCuda101Driver) && !_currentNvidiaSmiDriver.IsLesserVersionThan(NvidiaCuda10Driver))
                {
                    CUDA_version = "CUDA 10.0";
                }
                if (_currentNvidiaSmiDriver.IsLesserVersionThan(NvidiaCuda10Driver) && !_currentNvidiaSmiDriver.IsLesserVersionThan(NvidiaCuda92Driver))
                {
                    CUDA_version = "CUDA 9.2";
                }
                Helpers.ConsolePrint("NVIDIA driver", CUDA_version);
                // if we have nvidia cards but no CUDA devices tell the user to upgrade driver
                var isNvidiaErrorShown = false; // to prevent showing twice
                var showWarning = ConfigManager.GeneralConfig.ShowDriverVersionWarning &&
                                  WindowsDisplayAdapters.HasNvidiaVideoController();
                if (showWarning && _cudaDevices.CudaDevices.Count != nvidiaCount)
                {
                    isNvidiaErrorShown = true;
                    new Task(() =>
                    MessageBox.Show(International.GetText("Compute_Device_Query_Manager_LostDevice"), "Error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)).Start();
                }
                // recomended driver
                if (showWarning && _currentNvidiaSmiDriver.IsLesserVersionThan(NvidiaRecomendedDriver) &&
                  !isNvidiaErrorShown && _currentNvidiaSmiDriver.LeftPart > -1)
                {
                    var recomendDrvier = NvidiaRecomendedDriver.ToString();
                    var nvdriverString = _currentNvidiaSmiDriver.LeftPart > -1
                        ? string.Format(
                            International.GetText("Compute_Device_Query_Manager_NVIDIA_Driver_Recomended_PART"),
                            _currentNvidiaSmiDriver)
                        : "";
                    new Task(() => MessageBox.Show(string.Format(
                            International.GetText("Compute_Device_Query_Manager_NVIDIA_Driver_Recomended"),
                            recomendDrvier, nvdriverString, recomendDrvier),
                        International.GetText("Compute_Device_Query_Manager_NVIDIA_RecomendedDriver_Title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)).Start();
                }

                // no devices found
                if (Available.Devices.Count <= 0)
                {
                    var result = MessageBox.Show(International.GetText("Compute_Device_Query_Manager_No_Devices"),
                        International.GetText("Compute_Device_Query_Manager_No_Devices_Title"),
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.OK)
                    {
                        Process.Start(Links.NhmNoDevHelp);
                    }
                }
                foreach (var dev in Available.Devices)
                {
                    Helpers.ConsolePrint("QueryDevices", "ID: " + dev.ID.ToString() + " BusID: " + 
                        dev.BusID.ToString() + " IDByBus: " + dev.IDByBus + " Index: " + dev.Index + " lolMinerBusID:" + 
                        dev.lolMinerBusID + " " + dev.Name);
                }
                //Available.Devices.FindAll((a) => a.DeviceType == DeviceType.NVIDIA || a.DeviceType == DeviceType.AMD).Sort((x, y) => x.BusID.CompareTo(y.BusID));
               /*
                Available.Devices.Sort((x, y) => x.BusID.CompareTo(y.BusID));

                foreach (var dev in Available.Devices)
                {
                    Helpers.ConsolePrint("After sorting", "ID: " + dev.ID.ToString() + " BusID: " + dev.BusID.ToString() + " " + dev.Name);
                }
               */
                // create AMD bus ordering for Claymore
                var amdDevices = Available.Devices.FindAll((a) => a.DeviceType == DeviceType.AMD);
                amdDevices.Sort((a, b) => a.BusID.CompareTo(b.BusID));
                for (var i = 0; i < amdDevices.Count; i++)
                {
                    amdDevices[i].IDByBus = i;
                }
                //create NV bus ordering for Claymore
                var nvDevices = Available.Devices.FindAll((a) => a.DeviceType == DeviceType.NVIDIA);
                nvDevices.Sort((a, b) => a.BusID.CompareTo(b.BusID));
                for (var i = 0; i < nvDevices.Count; i++)
                {
                    nvDevices[i].IDByBus = i;
                }

                //create bus ordering for lolMiner
                var allDevices = Available.Devices.FindAll((a) => a.DeviceType == DeviceType.NVIDIA || a.DeviceType == DeviceType.AMD);
                allDevices.Sort((a, b) => a.BusID.CompareTo(b.BusID));
                for (var i = 0; i < allDevices.Count; i++)
                {
                    allDevices[i].lolMinerBusID = i;
                }

                // get GPUs RAM sum
                // bytes
                Available.NvidiaRamSum = 0;
                Available.AmdRamSum = 0;
                foreach (var dev in Available.Devices)
                {
                    if (dev.DeviceType == DeviceType.NVIDIA)
                    {
                        Available.NvidiaRamSum += dev.GpuRam;
                    }
                    else if (dev.DeviceType == DeviceType.AMD)
                    {
                        Available.AmdRamSum += dev.GpuRam;
                    }
                }
                // Make gpu ram needed not larger than 4GB per GPU
                var totalGpuRam = Math.Min((Available.NvidiaRamSum + Available.AmdRamSum) * 0.6 / 1024,
                    (double)Available.AvailGpUs * 4 * 1024 * 1024);
                double totalSysRam = SystemSpecs.FreePhysicalMemory + SystemSpecs.FreeVirtualMemory;
                // check
                if (ConfigManager.GeneralConfig.ShowDriverVersionWarning && totalSysRam < totalGpuRam)
                {
                    Helpers.ConsolePrint(Tag, "virtual memory size BAD");
                    MessageBox.Show(International.GetText("VirtualMemorySize_BAD"),
                        International.GetText("Warning_with_Exclamation"),
                        MessageBoxButtons.OK);
                }
                else
                {
                    Helpers.ConsolePrint(Tag, "virtual memory size GOOD");
                }

                // #x remove reference
                MessageNotifier = null;
            }

            #region Helpers

            private static readonly List<VideoControllerData> AvaliableVideoControllers =
                new List<VideoControllerData>();

            public static class WindowsDisplayAdapters
            {
                private static string SafeGetProperty(ManagementBaseObject mbo, string key)
                {
                    try
                    {
                        var o = mbo.GetPropertyValue(key);
                        if (o != null)
                        {
                            return o.ToString();
                        }
                    }
                    catch { }

                    return "key is null";
                }

                public static void QueryVideoControllers()
                {
                    QueryVideoControllers(AvaliableVideoControllers, true);
                }
                public static string GetManufacturer(string man)
                {
                    switch (man)
                    {
                        case "1002":
                            man = "AMD";
                            break;
                        case "1043":
                            man = "ASUS";
                            break;
                        case "196D":
                            man = "Club 3D";
                            break;
                        case "1092":
                            man = "Diamond Multimedia";
                            break;
                        case "18BC":
                            man = "GeCube";
                            break;
                        case "1458":
                            man = "Gigabyte";
                            break;
                        case "17AF":
                            man = "HIS";
                            break;
                        case "1787":
                            man = "HIS";
                            break;
                        case "16F3":
                            man = "Jetway";
                            break;
                        case "1462":
                            man = "MSI";
                            break;
                        case "1DA2":
                            man = "Sapphire";
                            break;
                        case "174B":
                            man = "Sapphire";
                            break;
                        case "148C":
                            man = "PowerColor";
                            break;
                        case "1545":
                            man = "VisionTek";
                            break;
                        case "1682":
                            man = "XFX";
                            break;
                        case "107D":
                            man = "Leadtek";
                            break;
                        case "10B0":
                            man = "Gainward";
                            break;
                        case "10DE":
                            man = "NVIDIA";
                            break;
                        case "154B":
                            man = "PNY";
                            break;
                        case "19DA":
                            man = "Zotac";
                            break;
                        case "19F1":
                            man = "BFG";
                            break;
                        case "1B4C":
                            man = "KFA2";
                            break;
                        case "3842":
                            man = "EVGA";
                            break;
                        case "7377":
                            man = "Colorful";
                            break;
                        default:
                            break;
                    }
                    return man;
                }

                private static void QueryVideoControllers(List<VideoControllerData> avaliableVideoControllers,
                    bool warningsEnabled)
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("");
                    stringBuilder.AppendLine("QueryVideoControllers: ");
                    var moc = new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_VideoController WHERE PNPDeviceID LIKE 'PCI%'").Get();
                    var allVideoContollersOK = true;
                    foreach (var manObj in moc)
                    {
                        //Int16 ram_Str = manObj["ProtocolSupported"] as Int16; manObj["AdapterRAM"] as string
                        ulong.TryParse(SafeGetProperty(manObj, "AdapterRAM"), out var memTmp);
                        var man = SafeGetProperty(manObj, "PNPDeviceID").Split('&')[2];
                        //PCI\VEN_1002&DEV_67DF&SUBSYS_2379148C&REV_EF\4&18803EC9&0&00E4
                        var vidController = new VideoControllerData
                        {
                            Name = SafeGetProperty(manObj, "Name"),
                            Description = SafeGetProperty(manObj, "Description"),
                            Manufacturer = man.Substring(man.Length - 4),
                            PnpDeviceID = SafeGetProperty(manObj, "PNPDeviceID"),
                            DeviceID = SafeGetProperty(manObj, "DeviceID"),
                            DriverVersion = SafeGetProperty(manObj, "DriverVersion"),
                            Status = SafeGetProperty(manObj, "Status"),
                            InfSection = SafeGetProperty(manObj, "InfSection"),
                            AdapterRam = memTmp
                        };

                        stringBuilder.AppendLine("\tWin32_VideoController detected:");
                        stringBuilder.AppendLine($"\t\tName {vidController.Name}");
                        stringBuilder.AppendLine($"\t\tDescription {vidController.Description}");
                        stringBuilder.AppendLine($"\t\tManufacturer {GetManufacturer(vidController.Manufacturer)} ({vidController.Manufacturer})");
                        stringBuilder.AppendLine($"\t\tPNPDeviceID {vidController.PnpDeviceID}");
                        stringBuilder.AppendLine($"\t\tDeviceID {vidController.DeviceID}");
                        stringBuilder.AppendLine($"\t\tDriverVersion {vidController.DriverVersion}");
                        stringBuilder.AppendLine($"\t\tStatus {vidController.Status}");
                        stringBuilder.AppendLine($"\t\tInfSection {vidController.InfSection}");
                        stringBuilder.AppendLine($"\t\tAdapterRAM {vidController.AdapterRam}");

                        // check if controller ok
                        if (allVideoContollersOK && !vidController.Status.ToLower().Equals("ok"))
                        {
                            allVideoContollersOK = false;
                        }

                        avaliableVideoControllers.Add(vidController);

                        if (vidController.DriverVersion.Contains("4.6079"))
                        {
                            MessageBox.Show("Unsupported NVIDIA driver version 460.79\r " +
                                "Please revert to previous drivers or install newest",
    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                    Helpers.ConsolePrint(Tag, stringBuilder.ToString());

                    if (warningsEnabled)
                    {
                        if (ConfigManager.GeneralConfig.ShowDriverVersionWarning && !allVideoContollersOK)
                        {
                            var msg = International.GetText("QueryVideoControllers_NOT_ALL_OK_Msg");
                            foreach (var vc in avaliableVideoControllers)
                            {
                                if (!vc.Status.ToLower().Equals("ok"))
                                {
                                    msg += Environment.NewLine
                                           + string.Format(
                                               International.GetText("QueryVideoControllers_NOT_ALL_OK_Msg_Append"),
                                               vc.Name, vc.Status, vc.PnpDeviceID);
                                }
                            }
                            new Task(() => MessageBox.Show(msg,
                                International.GetText("QueryVideoControllers_NOT_ALL_OK_Title"),
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)).Start();
                        }
                    }
                    //test_msi_ab();
                }



                public static bool HasNvidiaVideoController()
                {
                    return AvaliableVideoControllers.Any(vctrl => vctrl.Name.ToLower().Contains("nvidia"));
                }

                
            }
            

            private static class Cpu
            {
                public static void QueryCpus()
                {
                    Helpers.ConsolePrint(Tag, "QueryCpus START");
                    // get all CPUs
                    Available.CpusCount = CpuID.GetPhysicalProcessorCount();
                    Available.IsHyperThreadingEnabled = CpuID.IsHypeThreadingEnabled();

                    Helpers.ConsolePrint(Tag,
                        Available.IsHyperThreadingEnabled
                            ? "HyperThreadingEnabled = TRUE"
                            : "HyperThreadingEnabled = FALSE");

                    // get all cores (including virtual - HT can benefit mining)
                    var threadsPerCpu = CpuID.GetVirtualCoresCount() / Available.CpusCount;

                    if (!Helpers.Is64BitOperatingSystem)
                    {
                        if (ConfigManager.GeneralConfig.ShowDriverVersionWarning)
                        {
                            MessageBox.Show(International.GetText("Form_Main_msgbox_CPUMining64bitMsg"),
                                International.GetText("Warning_with_Exclamation"),
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        Available.CpusCount = 0;
                    }

                    if (threadsPerCpu * Available.CpusCount > 64)
                    {
                        if (ConfigManager.GeneralConfig.ShowDriverVersionWarning)
                        {
                            MessageBox.Show(International.GetText("Form_Main_msgbox_CPUMining64CoresMsg"),
                                International.GetText("Warning_with_Exclamation"),
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        Available.CpusCount = 0;
                    }

                    // TODO important move this to settings
                    var threadsPerCpuMask = threadsPerCpu;
                    Globals.ThreadsPerCpu = threadsPerCpu;

                    if (CpuUtils.IsCpuMiningCapable())
                    {
                        if (Available.CpusCount == 1)
                        {
                            Available.Devices.Add(
                                new CpuComputeDevice(0, "CPU0", CpuID.GetCpuName().Trim(), threadsPerCpu, 0,
                                    ++CpuCount)
                            );
                        }
                        else if (Available.CpusCount > 1)
                        {
                            for (var i = 0; i < Available.CpusCount; i++)
                            {
                                Available.Devices.Add(
                                    new CpuComputeDevice(i, "CPU" + i, CpuID.GetCpuName().Trim(), threadsPerCpu,
                                        CpuID.CreateAffinityMask(i, threadsPerCpuMask), ++CpuCount)
                                );
                            }
                        }
                    }

                    Helpers.ConsolePrint(Tag, "QueryCpus END");
                }
            }

            private static CudaDevicesList _cudaDevices = new CudaDevicesList();

            public static class Nvidia
            {
                private static string _queryCudaDevicesString = "";

                private static void QueryCudaDevicesOutputErrorDataReceived(object sender, DataReceivedEventArgs e)
                {
                    if (e.Data != null)
                    {
                        _queryCudaDevicesString += e.Data;
                    }
                }

                public static bool IsSkipNvidia()
                {
                    return ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionNVIDIA;
                }

                

                public static void QueryCudaDevices()
                {
                    Helpers.ConsolePrint(Tag, "QueryCudaDevices START");
                    QueryCudaDevices(ref _cudaDevices);

                    if (_cudaDevices != null && _cudaDevices.CudaDevices.Count != 0)
                    {
                        Available.HasNvidia = true;
                        var stringBuilder = new StringBuilder();
                        stringBuilder.AppendLine("");
                        stringBuilder.AppendLine("CudaDevicesDetection:");

                        // Enumerate NVAPI handles and map to busid
                        var idHandles = new Dictionary<int, NvPhysicalGpuHandle>();
                        if (NVAPI.IsAvailable)
                        {
                            var handles = new NvPhysicalGpuHandle[NVAPI.MAX_PHYSICAL_GPUS];
                            if (NVAPI.NvAPI_EnumPhysicalGPUs == null)
                            {
                                Helpers.ConsolePrint("NVAPI", "NvAPI_EnumPhysicalGPUs unavailable");
                            }
                            else
                            {
                                var status = NVAPI.NvAPI_EnumPhysicalGPUs(handles, out var _);
                                if (status != NvStatus.OK)
                                {
                                    Helpers.ConsolePrint("NVAPI", "Enum physical GPUs failed with status: " + status);
                                }
                                else
                                {
                                    foreach (var handle in handles)
                                    {
                                        var idStatus = NVAPI.NvAPI_GPU_GetBusID(handle, out var id);
                                        if (idStatus != NvStatus.EXPECTED_PHYSICAL_GPU_HANDLE)
                                        {
                                            if (idStatus != NvStatus.OK)
                                            {
                                                Helpers.ConsolePrint("NVAPI",
                                                    "Bus ID get failed with status: " + idStatus);
                                            }
                                            else
                                            {
                                                Helpers.ConsolePrint("NVAPI", "Found handle for busid " + id);
                                                idHandles[id] = handle;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var nvmlInit = false;
                        try
                        {
                            var ret = NvmlNativeMethods.nvmlInit();
                            if (ret != nvmlReturn.Success)
                            {

                                if (ret == nvmlReturn.Uninitialized)
                                {
                                    Helpers.ConsolePrint("NVML", "Uninitialized twice!");
                                    MessageBox.Show("Invalid NVIDIA driver installation!\r Update or reinstall drivers",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                throw new Exception($"NVML init failed with code {ret}");
                            }
                            nvmlInit = true;
                        }
                        catch (Exception e)
                        {
                            Helpers.ConsolePrint("NVML", e.ToString());
                        }
                        
                        //check id & busid order
                        int oldId = -1;
                        foreach (var cudaDev in _cudaDevices.CudaDevices.OrderBy(i => i.pciBusID))
                        {
                            if ((int)cudaDev.DeviceID < oldId)
                            {
                                Helpers.ConsolePrint("QueryCudaDevices", "NVIDIA ID & BusID order bug detected");
                                Form_Main.NVIDIA_orderBug = true;
                            }
                            oldId = (int)cudaDev.DeviceID;
                        }
                        
                        foreach (var cudaDev in _cudaDevices.CudaDevices.OrderBy(i => i.pciBusID))
                        {
                            // check sm vesrions
                            bool isUnderSM21;
                            {
                                var isUnderSM2Major = cudaDev.SM_major < 2;
                                var isUnderSM1Minor = cudaDev.SM_minor < 1;
                                isUnderSM21 = isUnderSM2Major && isUnderSM1Minor;
                            }
                            string Manufacturer = (cudaDev.pciSubSystemId).ToString("X16").Substring((cudaDev.pciSubSystemId).ToString("X16").Length - 4);
                            cudaDev.CUDAManufacturer = ComputeDevice.GetManufacturer(Manufacturer);
                            //bool isOverSM6 = cudaDev.SM_major > 6;
                            var skip = isUnderSM21;
                            var skipOrAdd = skip ? "SKIPED" : "ADDED";
                            const string isDisabledGroupStr = ""; // TODO remove
                            var etherumCapableStr = cudaDev.IsEtherumCapable() ? "YES" : "NO";
                            stringBuilder.AppendLine($"\t{skipOrAdd} device{isDisabledGroupStr}:");
                            stringBuilder.AppendLine($"\t\tID: {cudaDev.DeviceID}");
                            stringBuilder.AppendLine($"\t\tpciBusID: {cudaDev.pciBusID}");
                            stringBuilder.AppendLine($"\t\tNAME: {cudaDev.GetName()}");
                            stringBuilder.AppendLine($"\t\tMANUFACTURER: {cudaDev.CUDAManufacturer} ({Manufacturer})");
                            stringBuilder.AppendLine($"\t\tVENDOR: {cudaDev.VendorName}");
                            stringBuilder.AppendLine($"\t\tUUID: {cudaDev.UUID}");
                            stringBuilder.AppendLine($"\t\tMonitor: {cudaDev.HasMonitorConnected}");
                            stringBuilder.AppendLine($"\t\tMEMORY: {cudaDev.DeviceGlobalMemory}");
                            stringBuilder.AppendLine($"\t\tETHEREUM: {etherumCapableStr}");
                            //force 1080 detection
                            if (cudaDev.GetName().Contains("1080") || cudaDev.GetName().Contains("Titan Xp"))
                            {
                                Form_Main.ShouldRunEthlargement = true;
                            }

                            if (!skip)
                            {
                                DeviceGroupType group;
                                switch (cudaDev.SM_major)
                                {
                                    case 2:
                                        group = DeviceGroupType.NVIDIA_2_1;
                                        break;
                                    case 3:
                                        group = DeviceGroupType.NVIDIA_3_x;
                                        break;
                                    case 5:
                                        group = DeviceGroupType.NVIDIA_5_x;
                                        break;
                                    case 6:
                                        group = DeviceGroupType.NVIDIA_6_x;
                                        break;
                                    default:
                                        group = DeviceGroupType.NVIDIA_6_x;
                                        break;
                                }

                                var nvmlHandle = new nvmlDevice();

                                if (nvmlInit)
                                {
                                    var ret = NvmlNativeMethods.nvmlDeviceGetHandleByUUID(cudaDev.UUID, ref nvmlHandle);
                                    stringBuilder.AppendLine(
                                        "\t\tNVML HANDLE: " +
                                        $"{(ret == nvmlReturn.Success ? nvmlHandle.Pointer.ToString() : $"Failed with code ret {ret}")}");
                                }

                                idHandles.TryGetValue(cudaDev.pciBusID, out var handle);
                                Available.Devices.Add(
                                    new CudaComputeDevice(cudaDev, group, ++GpuCount, handle, nvmlHandle)
                                );
                            }
                        }
                        
                        
                        Helpers.ConsolePrint(Tag, stringBuilder.ToString());
                    }
                    Helpers.ConsolePrint(Tag, "QueryCudaDevices END");
                }

                private static List<CudaDevices2> _CudaDeviceList = new List<CudaDevices2>();
                public static void QueryCudaDevices(ref CudaDevicesList _cudaDevices)
                {
                    _queryCudaDevicesString = "";

                    Process cudaDevicesDetection = new Process();
                    cudaDevicesDetection.StartInfo.FileName = "common/DeviceDetection/device_detection.exe";
                    cudaDevicesDetection.StartInfo.Arguments = "cuda -n";
                    cudaDevicesDetection.StartInfo.UseShellExecute = false;
                    cudaDevicesDetection.StartInfo.RedirectStandardOutput = true;
                    cudaDevicesDetection.StartInfo.RedirectStandardError = true;
                    cudaDevicesDetection.StartInfo.CreateNoWindow = true;

                    const int waitTime = 5 * 1000; // 30seconds
                    try
                    {
                        if (!cudaDevicesDetection.Start())
                        {
                            Helpers.ConsolePrint(Tag, "CudaDevicesDetection process could not start");
                        }
                        else
                        {
                            _queryCudaDevicesString += cudaDevicesDetection.StandardOutput.ReadToEnd();
                            _queryCudaDevicesString += cudaDevicesDetection.StandardError.ReadToEnd();

                            if (cudaDevicesDetection.WaitForExit(waitTime))
                            {
                                cudaDevicesDetection.Close();
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        // TODO
                        Helpers.ConsolePrint(Tag, "CudaDevicesDetection threw Exception: " + ex.Message);
                    }
                    finally
                    {
                        if (cudaDevicesDetection != null)
                        {
                            cudaDevicesDetection.Close();
                            cudaDevicesDetection.Dispose();
                        }
                        if (_queryCudaDevicesString != "")
                        {
                            try
                            {
                                _cudaDevices = JsonConvert.DeserializeObject<CudaDevicesList>(_queryCudaDevicesString);
                            }
                            catch (Exception ex)
                            {
                                Helpers.ConsolePrint("QueryCudaDevices", ex.ToString());
                                _cudaDevices = null;
                            }

                                if (_cudaDevices == null || _cudaDevices.CudaDevices.Count == 0)
                                Helpers.ConsolePrint(Tag,
                                    "CudaDevicesDetection found no devices("+ _cudaDevices.CudaDevices.Count.ToString()+"). CudaDevicesDetection returned: " +
                                    _queryCudaDevicesString);
                        }
                    }
                }
            }

            private static List<OpenCLDevice> _OpenCLDevice = new List<OpenCLDevice>();
            private static OpenCLJsonData _openCLJsonData = new OpenCLJsonData();
            private static bool _isOpenCLQuerySuccess = false;

            private static class OpenCL
            {
                private static string _queryOpenCLDevicesString = "";

                private static void QueryOpenCLDevicesOutputErrorDataReceived(object sender, DataReceivedEventArgs e)
                {
                    if (e.Data != null)
                    {
                        _queryOpenCLDevicesString += e.Data;
                    }
                }

                public static void QueryOpenCLDevices()
                {
                    Helpers.ConsolePrint(Tag, "QueryOpenCLDevices START");

                    Process openCLDevicesDetection = new Process();
                    openCLDevicesDetection.StartInfo.FileName = "common/DeviceDetection/device_detection.exe";
                    openCLDevicesDetection.StartInfo.Arguments = "ocl -n";
                    openCLDevicesDetection.StartInfo.UseShellExecute = false;
                    openCLDevicesDetection.StartInfo.RedirectStandardOutput = true;
                    openCLDevicesDetection.StartInfo.RedirectStandardError = true;
                    openCLDevicesDetection.StartInfo.CreateNoWindow = true;

                    const int waitTime = 5 * 1000; // 30seconds
                    try
                    {
                        if (!openCLDevicesDetection.Start())
                        {
                            Helpers.ConsolePrint(Tag, "AMDOpenCLDeviceDetection process could not start");
                        }
                        else
                        {
                            _queryOpenCLDevicesString += openCLDevicesDetection.StandardOutput.ReadToEnd();
                            _queryOpenCLDevicesString += openCLDevicesDetection.StandardError.ReadToEnd();

                            if (openCLDevicesDetection.WaitForExit(waitTime))
                            {
                                openCLDevicesDetection.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // TODO
                        Helpers.ConsolePrint(Tag, "AMDOpenCLDeviceDetection threw Exception: " + ex.Message);
                    }
                    finally
                    {
                        if (_queryOpenCLDevicesString != "")
                        {
                            try
                            {
                                _openCLJsonData = JsonConvert.DeserializeObject<OpenCLJsonData>(_queryOpenCLDevicesString);
                            }
                            catch (Exception ex)
                            {
                                Helpers.ConsolePrint("QueryOpenCLDevices", ex.ToString());
                                _openCLJsonData = null;
                            }
                        }
                    }

                    if (_openCLJsonData == null)
                    {
                        Helpers.ConsolePrint(Tag,
                            "AMDOpenCLDeviceDetection found no devices. AMDOpenCLDeviceDetection returned: " +
                            _queryOpenCLDevicesString);
                    }
                    else
                    {
                        _isOpenCLQuerySuccess = true;
                        var stringBuilder = new StringBuilder();
                        stringBuilder.AppendLine("");
                        stringBuilder.AppendLine("AMDOpenCLDeviceDetection found devices success:");
                        foreach (var oclPlat in _openCLJsonData.Platforms)
                        {
                            stringBuilder.AppendLine($"\tFound devices for platform: {oclPlat.PlatformName}");
                            foreach (var oclDev in oclPlat.Devices)
                            {
                                stringBuilder.AppendLine("\t\tDevice:");
                                stringBuilder.AppendLine($"\t\t\tDevice ID {oclDev.DeviceID}");
                                stringBuilder.AppendLine($"\t\t\tDevice NAME {oclDev._CL_DEVICE_NAME}");
                                stringBuilder.AppendLine($"\t\t\tDevice TYPE {oclDev._CL_DEVICE_TYPE}");
                            }
                        }
                        Helpers.ConsolePrint(Tag, stringBuilder.ToString());
                    }
                    Helpers.ConsolePrint(Tag, "QueryOpenCLDevices END");
                }
            }

            public static List<OpenCLDevice> AmdDevices = new List<OpenCLDevice>();

            #endregion Helpers
        }

        public static class SystemSpecs
        {
            public static ulong FreePhysicalMemory;
            public static ulong FreeSpaceInPagingFiles;
            public static ulong FreeVirtualMemory;
            public static uint LargeSystemCache;
            public static uint MaxNumberOfProcesses;
            public static ulong MaxProcessMemorySize;

            public static uint NumberOfLicensedUsers;
            public static uint NumberOfProcesses;
            public static uint NumberOfUsers;
            public static uint OperatingSystemSKU;

            public static ulong SizeStoredInPagingFiles;

            public static uint SuiteMask;

            public static ulong TotalSwapSpaceSize;
            public static ulong TotalVirtualMemorySize;
            public static ulong TotalVisibleMemorySize;


            public static void QueryAndLog()
            {
                var winQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");

                var searcher = new ManagementObjectSearcher(winQuery);

                foreach (ManagementObject item in searcher.Get())
                {
                    if (item["FreePhysicalMemory"] != null)
                        ulong.TryParse(item["FreePhysicalMemory"].ToString(), out FreePhysicalMemory);
                    if (item["FreeSpaceInPagingFiles"] != null)
                        ulong.TryParse(item["FreeSpaceInPagingFiles"].ToString(), out FreeSpaceInPagingFiles);
                    if (item["FreeVirtualMemory"] != null)
                        ulong.TryParse(item["FreeVirtualMemory"].ToString(), out FreeVirtualMemory);
                    if (item["LargeSystemCache"] != null)
                        uint.TryParse(item["LargeSystemCache"].ToString(), out LargeSystemCache);
                    if (item["MaxNumberOfProcesses"] != null)
                        uint.TryParse(item["MaxNumberOfProcesses"].ToString(), out MaxNumberOfProcesses);
                    if (item["MaxProcessMemorySize"] != null)
                        ulong.TryParse(item["MaxProcessMemorySize"].ToString(), out MaxProcessMemorySize);
                    if (item["NumberOfLicensedUsers"] != null)
                        uint.TryParse(item["NumberOfLicensedUsers"].ToString(), out NumberOfLicensedUsers);
                    if (item["NumberOfProcesses"] != null)
                        uint.TryParse(item["NumberOfProcesses"].ToString(), out NumberOfProcesses);
                    if (item["NumberOfUsers"] != null)
                        uint.TryParse(item["NumberOfUsers"].ToString(), out NumberOfUsers);
                    if (item["OperatingSystemSKU"] != null)
                        uint.TryParse(item["OperatingSystemSKU"].ToString(), out OperatingSystemSKU);
                    if (item["SizeStoredInPagingFiles"] != null)
                        ulong.TryParse(item["SizeStoredInPagingFiles"].ToString(), out SizeStoredInPagingFiles);
                    if (item["SuiteMask"] != null) uint.TryParse(item["SuiteMask"].ToString(), out SuiteMask);
                    if (item["TotalSwapSpaceSize"] != null)
                        ulong.TryParse(item["TotalSwapSpaceSize"].ToString(), out TotalSwapSpaceSize);
                    if (item["TotalVirtualMemorySize"] != null)
                        ulong.TryParse(item["TotalVirtualMemorySize"].ToString(), out TotalVirtualMemorySize);
                    if (item["TotalVisibleMemorySize"] != null)
                        ulong.TryParse(item["TotalVisibleMemorySize"].ToString(), out TotalVisibleMemorySize);
                    // log
                    Helpers.ConsolePrint("SystemSpecs", $"FreePhysicalMemory = {FreePhysicalMemory}");
                    Helpers.ConsolePrint("SystemSpecs", $"FreeSpaceInPagingFiles = {FreeSpaceInPagingFiles}");
                    Helpers.ConsolePrint("SystemSpecs", $"FreeVirtualMemory = {FreeVirtualMemory}");
                    Helpers.ConsolePrint("SystemSpecs", $"LargeSystemCache = {LargeSystemCache}");
                    Helpers.ConsolePrint("SystemSpecs", $"MaxNumberOfProcesses = {MaxNumberOfProcesses}");
                    Helpers.ConsolePrint("SystemSpecs", $"MaxProcessMemorySize = {MaxProcessMemorySize}");
                    Helpers.ConsolePrint("SystemSpecs", $"NumberOfLicensedUsers = {NumberOfLicensedUsers}");
                    Helpers.ConsolePrint("SystemSpecs", $"NumberOfProcesses = {NumberOfProcesses}");
                    Helpers.ConsolePrint("SystemSpecs", $"NumberOfUsers = {NumberOfUsers}");
                    Helpers.ConsolePrint("SystemSpecs", $"OperatingSystemSKU = {OperatingSystemSKU}");
                    Helpers.ConsolePrint("SystemSpecs", $"SizeStoredInPagingFiles = {SizeStoredInPagingFiles}");
                    Helpers.ConsolePrint("SystemSpecs", $"SuiteMask = {SuiteMask}");
                    Helpers.ConsolePrint("SystemSpecs", $"TotalSwapSpaceSize = {TotalSwapSpaceSize}");
                    Helpers.ConsolePrint("SystemSpecs", $"TotalVirtualMemorySize = {TotalVirtualMemorySize}");
                    Helpers.ConsolePrint("SystemSpecs", $"TotalVisibleMemorySize = {TotalVisibleMemorySize}");
                    Helpers.ConsolePrint("SystemSpecs", $"ProcessorCount = {Environment.ProcessorCount}");
                }
            }
        }

        public static class Available
        {
            public static bool HasNvidia = false;
            public static bool HasAmd = false;
            public static bool HasCpu = false;
            public static int CpusCount = 0;

            public static int AvailCpus
            {
                get { return Devices.Count(d => d.DeviceType == DeviceType.CPU); }
            }

            public static int AvailNVGpus
            {
                get { return Devices.Count(d => d.DeviceType == DeviceType.NVIDIA); }
            }

            public static int AvailAmdGpus
            {
                get { return Devices.Count(d => d.DeviceType == DeviceType.AMD); }
            }

            public static int AvailGpUs => AvailAmdGpus + AvailNVGpus;
            public static int AmdOpenCLPlatformNum = -1;
            public static bool IsHyperThreadingEnabled = false;

            public static ulong NvidiaRamSum = 0;
            public static ulong AmdRamSum = 0;

            public static readonly List<ComputeDevice> Devices = new List<ComputeDevice>();

            // methods
            public static ComputeDevice GetDeviceWithUuid(string uuid)
            {
                return Devices.FirstOrDefault(dev => uuid == dev.Uuid);
            }

            public static List<ComputeDevice> GetSameDevicesTypeAsDeviceWithUuid(string uuid)
            {
                var compareDev = GetDeviceWithUuid(uuid);
                return (from dev in Devices
                    where uuid != dev.Uuid && compareDev.DeviceType == dev.DeviceType
                    select GetDeviceWithUuid(dev.Uuid)).ToList();
            }

            public static ComputeDevice GetCurrentlySelectedComputeDevice(int index, bool unique)
            {
                return Devices[index];
            }

            public static int GetCountForType(DeviceType type)
            {
                return Devices.Count(device => device.DeviceType == type);
            }
        }

        public static class Group
        {
            public static void DisableCpuGroup()
            {
                foreach (var device in Available.Devices)
                {
                    if (device.DeviceType == DeviceType.CPU)
                    {
                        device.Enabled = false;
                    }
                }
            }

            public static bool ContainsAmdGpus
            {
                get { return Available.Devices.Any(device => device.DeviceType == DeviceType.AMD); }
            }

            public static bool ContainsGpus
            {
                get
                {
                    return Available.Devices.Any(device =>
                        device.DeviceType == DeviceType.NVIDIA || device.DeviceType == DeviceType.AMD);
                }
            }

            public static void UncheckedCpu()
            {
                // Auto uncheck CPU if any GPU is found
                if (ContainsGpus) DisableCpuGroup();
            }
        }
    }
}
