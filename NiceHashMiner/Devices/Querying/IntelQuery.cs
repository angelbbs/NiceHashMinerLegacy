using Microsoft.Win32;
using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.UUID;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static IGCL.IGCL;
using static NiceHashMiner.Devices.ComputeDeviceManager.Query;

namespace NiceHashMiner.Devices.Querying
{
    public class IntelQuery
    {
        private const string Tag = "IntelQuery";
        private const int IntelVendorID = 8086;

        private static List<VideoControllerData> _availableControllers;
        private static readonly Dictionary<int, BusIdInfo> _busIdInfos = new Dictionary<int, BusIdInfo>();
        private static readonly List<string> _IntelDeviceUuid = new List<string>();

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

        public static List<OpenCLDevice> ProcessDevices(List<VideoControllerData> availControllers)
        {
            _availableControllers = availControllers;
            List<OpenCLDevice> IntelOclDevices = IntelDetection();

            var IntelDevices = new List<OpenCLDevice>();
            var IntelPlatformNumFound = WindowsDisplayAdapters.HasIntelVideoController();

            if (!IntelPlatformNumFound)
            {
                Helpers.ConsolePrint("IntelQuery", "Intel Arc OpenCL platform not found");
                return IntelDevices;
            }
            // get only Intel gpus
            string[] _PNPDeviceID;
            foreach (var oclDev in IntelOclDevices)
            {
                if (oclDev._CL_DEVICE_TYPE.Contains("GPU"))
                {
                    string UUID = "";
                    string _mf = "";
                    string _InfSection = "";
                    foreach (var vc in ComputeDeviceManager.Query.AvaliableVideoControllers)
                    {
                        if (vc.BusID == oclDev.BUS_ID)
                        {
                            _PNPDeviceID = vc.PnpDeviceID.Split('\\');
                            UUID = vc.PnpDeviceID.Split('&')[0] + "&" + vc.PnpDeviceID.Split('&')[1] + "_" + vc.PnpDeviceID.Split('&')[4];
                            UUID = UUID.Replace("\\", "_");

                            _IntelDeviceUuid.Add(UUID);

                            _mf = vc.Manufacturer;
                            _InfSection = vc.InfSection;

                            const string hklm = "HKEY_LOCAL_MACHINE";
                            string keyPath = hklm + @"\SYSTEM\CurrentControlSet\Enum\PCI\" + _PNPDeviceID[1] + "\\" + _PNPDeviceID[2];
                            const string value = "LocationInformation";

                            try
                            {
                                var readValue = Registry.GetValue(keyPath, value, new object());
                                //@System32\drivers\pci.sys,#65536;PCI-шина %1, устройство %2, функция %3;(6,0,0)
                                int s = readValue.ToString().LastIndexOf(';') + 2;
                                string t = readValue.ToString().Substring(s, readValue.ToString().Length - s);
                                string r = t.Split(',')[0];
                                int.TryParse(r, out int _busID);
                                if (_busID >= 0)
                                {
                                    //oclDev.BUS_ID = _busID;
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Helpers.ConsolePrint("ProcessDevices", ex.ToString());
                            }

                        }
                    }
                    IntelDevices.Add(oclDev);
                    var info = new BusIdInfo
                    {
                        Name = oclDev._CL_DEVICE_NAME.Replace("(R)", "").Replace("(TM)", ""),
                        MF_Intel = _mf,
                        Uuid = UUID,
                        InfSection = _InfSection,
                        DeviceIndex = (int)oclDev.DeviceID,
                        DeviceHandle = oclDev.DeviceHandle
                    };

                    _busIdInfos.Add(oclDev.BUS_ID, info);
                }
            }



            if (IntelDevices.Count == 0)
            {
                Helpers.ConsolePrint(Tag, "Intel GPUs count is 0");
                return IntelOclDevices;
            }

            Helpers.ConsolePrint(Tag, "Intel GPUs count : " + IntelDevices.Count);

            var isBusIDOk = true;
            {
                var busIDs = new HashSet<int>();
                for (var i = 0; i < IntelOclDevices.Count; i++)
                {
                    var IntelOclDev = IntelOclDevices[i];
                    if (IntelOclDev.BUS_ID < 0 || !_busIdInfos.ContainsKey(IntelOclDev.BUS_ID))
                    {
                        isBusIDOk = false;
                        break;
                    }

                    busIDs.Add(IntelOclDev.BUS_ID);
                }

                isBusIDOk = isBusIDOk && busIDs.Count == IntelOclDevices.Count;
            }
            Helpers.ConsolePrint(Tag,
                isBusIDOk
                    ? "Intel Bus IDs are unique and valid. OK"
                    : "Intel Bus IDs IS INVALID. Using fallback Intel detection mode");


            if (isBusIDOk)
            {
                return IntelDeviceCreationPrimary(IntelOclDevices);
            }

            return IntelDeviceCreationFallback(IntelOclDevices);
        }

        static byte[] StreamToByteArray(Stream inputStream)
        {
            if (!inputStream.CanRead)
            {
                throw new ArgumentException();
            }

            // This is optional
            if (inputStream.CanSeek)
            {
                inputStream.Seek(0, SeekOrigin.Begin);
            }

            byte[] output = new byte[inputStream.Length];
            int bytesRead = inputStream.Read(output, 0, output.Length);
            Debug.Assert(bytesRead == output.Length, "Bytes read from stream matches stream length");
            return output;
        }

        private static List<OpenCLDevice> IntelDeviceCreationPrimary(List<OpenCLDevice> intelDevices)
        {
            Helpers.ConsolePrint(Tag, "Using INTEL device creation DEFAULT Reliable mappings");
            Helpers.ConsolePrint(Tag,
                intelDevices.Count == _IntelDeviceUuid.Count
                    ? "INTEL OpenCL query COUNTS GOOD/SAME"
                    : "INTEL OpenCL query COUNTS DIFFERENT/BAD");
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("QueryINTEL [DEFAULT query] devices: ");
            try
            {
                foreach (var dev in intelDevices.OrderBy(i => i.BUS_ID))//****************************************************
                //foreach (var dev in amdDevices)
                {
                    ComputeDeviceManager.Available.HasIntel = true;

                    var busID = dev.BUS_ID;
                    var gpuRAM = dev._CL_DEVICE_GLOBAL_MEM_SIZE + 16384 * 1024;

                    if (busID != -1 && _busIdInfos.ContainsKey(busID))
                    {
                        var deviceName = _busIdInfos[busID].Name;
                        var manufacturer = _busIdInfos[busID].MF_Intel;

                        IntelGpuDevice newIntelDev = new IntelGpuDevice(dev, false,
                            _busIdInfos[busID].InfSection, false)
                        {
                            DeviceName = deviceName,
                            UUID = _busIdInfos[busID].Uuid,
                            AdapterIndex = _busIdInfos[busID].DeviceIndex,
                            DeviceHandle = _busIdInfos[busID].DeviceHandle,
                            IntelManufacturer = _busIdInfos[busID].MF_Intel,
                            DeviceGlobalMemory = gpuRAM
                        };

                        int _prevmonitorRefreshRate = 0;
                        //*************
                        string PnpDeviceID = "";
                        ulong gpumem = 0;
                        ulong gpumemadd = 1048576; //add 1MB to gpumem
                        var moc = new ManagementObjectSearcher("root\\CIMV2",
                            "SELECT * FROM Win32_VideoController WHERE PNPDeviceID LIKE 'PCI%'").Get();

                        foreach (var manObj in moc)
                        {
                            ulong.TryParse(SafeGetProperty(manObj, "AdapterRAM"), out var memTmp);
                            int.TryParse(SafeGetProperty(manObj, "CurrentRefreshRate"), out var _monitorRefreshRate);
                            PnpDeviceID = SafeGetProperty(manObj, "PNPDeviceID");
                            gpumem = memTmp + gpumemadd‬;

                            if (PnpDeviceID.Split('&')[4].Equals(newIntelDev.UUID.Split('_')[4]))
                            {
                                if (_monitorRefreshRate > 0 & _monitorRefreshRate > _prevmonitorRefreshRate)
                                {
                                    newIntelDev.MonitorConnected = true;
                                }
                                if (newIntelDev.DeviceGlobalMemory < gpumem)
                                {
                                    Helpers.ConsolePrint("INTELQUERY", deviceName + " GPU mem size is not equal: " + newIntelDev.DeviceGlobalMemory.ToString() + " < " + gpumem.ToString());
                                    newIntelDev.DeviceGlobalMemory = gpumem;
                                    dev._CL_DEVICE_GLOBAL_MEM_SIZE = gpumem;
                                }
                            }
                        }
                        //*************
                        var isDisabledGroup = ConfigManager.GeneralConfig.DeviceDetection
                            .DisableDetectionINTEL;
                        var skipOrAdd = isDisabledGroup ? "SKIPED" : "ADDED";
                        var isDisabledGroupStr = isDisabledGroup ? " (INTEL group disabled)" : "";
                        var etherumCapableStr = newIntelDev.IsEtherumCapable() ? "YES" : "NO";

                        ComputeDeviceManager.Available.Devices.Add(
                            new IntelComputeDevice(newIntelDev, ++ComputeDeviceManager.Query.GpuCount, false,
                                _busIdInfos[busID].DeviceIndex));
                        var infSection = newIntelDev.InfSection;
                        //var PnpDeviceID = dev.PnpDeviceID;
                        //var PnpDeviceID = vidController.PnpDeviceID;
                        var infoToHashed = $"{newIntelDev.DeviceID}--{DeviceType.INTEL}--{newIntelDev.DeviceGlobalMemory}--{newIntelDev.Codename}--{newIntelDev.DeviceName}";
                        infoToHashed += newIntelDev.UUID.Replace("PCI_", "PCI/");//PnpDeviceID неверный!

                        var uuidHEX = UUID.GetHexUUID(infoToHashed);
                        var Newuuid = $"INTEL-{uuidHEX}";
                        newIntelDev.NewUUID = Newuuid;
                        // just in case
                        try
                        {
                            stringBuilder.AppendLine($"\t{skipOrAdd} device{isDisabledGroupStr}:");
                            stringBuilder.AppendLine($"\t\tNAME: {newIntelDev.DeviceName}");
                            stringBuilder.AppendLine($"\t\tCODE_NAME: {newIntelDev.Codename}");
                            stringBuilder.AppendLine($"\t\tMonitor connected: {newIntelDev.MonitorConnected}");
                            stringBuilder.AppendLine($"\t\tManufacturer: {newIntelDev.IntelManufacturer}");
                            stringBuilder.AppendLine($"\t\tUUID: {newIntelDev.UUID}");
                            stringBuilder.AppendLine($"\t\tNewUUID: {newIntelDev.NewUUID}");
                            stringBuilder.AppendLine($"\t\tBusID: {newIntelDev.BusID}");
                            stringBuilder.AppendLine($"\t\tDeviceID: {newIntelDev.DeviceID}");
                            stringBuilder.AppendLine($"\t\tInfSection: {newIntelDev.InfSection}");
                            stringBuilder.AppendLine($"\t\tMEMORY: {newIntelDev.DeviceGlobalMemory}");
                            stringBuilder.AppendLine($"\t\tETHEREUM: {etherumCapableStr}");
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        stringBuilder.AppendLine($"\tDevice not added, Bus No. {busID} not found:");
                    }
                }
            }
            catch (Exception er)
            {
                Helpers.ConsolePrint("iNTELDeviceCreationPrimary", er.ToString());
            }

            Helpers.ConsolePrint(Tag, stringBuilder.ToString());

            return intelDevices;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate _ctl_result_t ctlEnumerateDevices(ulong hAPIHandle, IntPtr _Adapter_count, ulong[] hDevices);
        public static List<OpenCLDevice> IntelDetection()
        {
            List<OpenCLDevice> IntelDevices = new List<OpenCLDevice>();

            _CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
            ctl_init_args_t CtlInitArgs = new ctl_init_args_t();
            CtlInitArgs.AppVersion = CTL_MAKE_VERSION(CTL_IMPL_MAJOR_VERSION, CTL_IMPL_MINOR_VERSION);
            CtlInitArgs.flags = CTL_INIT_FLAG_USE_LEVEL_ZERO;
            CtlInitArgs.Size = (uint)Marshal.SizeOf(typeof(ctl_init_args_t));
            CtlInitArgs.Version = 0;
            CtlInitArgs.SupportedVersion = 0;

            IntPtr hAPIHandle = new IntPtr(0);
            _ctl_result_t r = (_ctl_result_t)ctlInit(ref CtlInitArgs, ref hAPIHandle);
            if (r == _ctl_result_t.CTL_RESULT_SUCCESS)
            {
                string sv = (CtlInitArgs.SupportedVersion & 0xFFFF).ToString() + "." + (CtlInitArgs.SupportedVersion >> 16).ToString();
                uint Adapter_count = 0;

                r = (_ctl_result_t)ctlEnumerateDevices(hAPIHandle, ref Adapter_count, null);
                if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
                {
                    Helpers.ConsolePrint("IntelDeviceCreationPrimary", "ctlEnumerateDevices 1 ERROR: " + r.ToString());
                    return IntelDevices;
                }
                Helpers.ConsolePrint("IntelDeviceCreationPrimary", "Adapter_count: " + Adapter_count.ToString("X"));

                long[] hDevices = new long[(int)Adapter_count];
                r = (_ctl_result_t)ctlEnumerateDevices(hAPIHandle, ref Adapter_count, hDevices);
                if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
                {
                    Helpers.ConsolePrint("IntelDeviceCreationPrimary", "ctlEnumerateDevices 2 ERROR: " + r.ToString());
                    return IntelDevices;
                }

                for (int dev = 0; dev < Adapter_count; dev++)
                {
                    OpenCLDevice intelOpenCLDevice = new OpenCLDevice();
                    ctl_device_adapter_properties_t StDeviceAdapterProperties = new ctl_device_adapter_properties_t();
                    StDeviceAdapterProperties.Size = Marshal.SizeOf(typeof(ctl_device_adapter_properties_t));
                    StDeviceAdapterProperties.pDeviceID = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Luid)));
                    StDeviceAdapterProperties.device_id_size = Marshal.SizeOf(typeof(Luid));
                    StDeviceAdapterProperties.Version = 2;

                    r = ctlGetDeviceProperties(hDevices[dev], ref StDeviceAdapterProperties);
                    if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
                    {
                        Helpers.ConsolePrint("IntelDeviceCreationPrimary", "ctlGetDeviceProperties ERROR: " + r.ToString());
                        return IntelDevices;
                    }
                    if (StDeviceAdapterProperties.device_type != ctl_device_type_t.CTL_DEVICE_TYPE_GRAPHICS) continue;

                    intelOpenCLDevice._CL_DEVICE_TYPE = "GPU";
                    string n = new string(StDeviceAdapterProperties.name);
                    intelOpenCLDevice._CL_DEVICE_NAME = n.TrimEnd('\u0000');
                    intelOpenCLDevice._CL_DEVICE_VENDOR_ID = StDeviceAdapterProperties.pci_subsys_vendor_id;
                    intelOpenCLDevice._CL_DEVICE_VERSION = StDeviceAdapterProperties.rev_id.ToString();
                    intelOpenCLDevice._CL_DRIVER_VERSION = StDeviceAdapterProperties.driver_version.ToString();
                    intelOpenCLDevice.DeviceHandle = hDevices[dev];

                    ctl_pci_properties_t Pci_properties = new ctl_pci_properties_t();
                    Pci_properties.Size = Marshal.SizeOf(typeof(ctl_pci_properties_t));
                    r = ctlPciGetProperties(hDevices[dev], ref Pci_properties);
                    if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
                    {
                        Helpers.ConsolePrint("IntelDeviceCreationPrimary", "ctlPciGetProperties ERROR: " + r.ToString());
                        //ctlClose(hAPIHandle);
                        return IntelDevices;
                    }
                    intelOpenCLDevice.BUS_ID = Pci_properties.address.bus;
                    intelOpenCLDevice.DeviceID = Pci_properties.address.device;
//                    Helpers.ConsolePrint("StDeviceAdapterProperties*********", StDeviceAdapterProperties.pci_device_id.ToString()); ;
                    //*************************
                    uint MemoryHandlerCount = 0;

                    r = (_ctl_result_t)ctlEnumMemoryModules(hDevices[dev], ref MemoryHandlerCount, null);
                    if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
                    {
                        Helpers.ConsolePrint("IntelDeviceCreationPrimary", "ctlEnumMemoryModules 1 ERROR: " + r.ToString());
                        //ctlClose(hAPIHandle);
                        return IntelDevices;
                    }

                    long[] pMemoryHandle = new long[(int)MemoryHandlerCount];
                    r = (_ctl_result_t)ctlEnumMemoryModules(hDevices[dev], ref MemoryHandlerCount, pMemoryHandle);
                    if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
                    {
                        Helpers.ConsolePrint("IntelDeviceCreationPrimary", "ctlEnumMemoryModules 2 ERROR: " + r.ToString());
                        //ctlClose(hAPIHandle);
                        return IntelDevices;
                    }

                    for (int mem = 0; mem < MemoryHandlerCount; mem++)
                    {
                        ctl_mem_properties_t memoryProperties = new ctl_mem_properties_t();
                        memoryProperties.Size = Marshal.SizeOf(typeof(ctl_mem_properties_t));
                        r = (_ctl_result_t)ctlMemoryGetProperties(pMemoryHandle[mem], ref memoryProperties);
                        if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
                        {
                            Helpers.ConsolePrint("IntelDeviceCreationPrimary", "ctlMemoryGetProperties ERROR: " + r.ToString());
                            //ctlClose(hAPIHandle);
                            return IntelDevices;
                        }

                        if (memoryProperties.location == ctl_mem_loc_t.CTL_MEM_LOC_DEVICE)
                        {
                            intelOpenCLDevice._CL_DEVICE_GLOBAL_MEM_SIZE = memoryProperties.physicalSize;
                            break;
                        }
                    }
                    IntelDevices.Add(intelOpenCLDevice);
                }
            }
            else
            {
                Helpers.ConsolePrint("IntelDeviceCreationPrimary", r.ToString());
            }
            //ctlClose(hAPIHandle);
            return IntelDevices;
        }

        public static double GetTemperature(long hDevice, bool isMemTemp = false)
        {
            uint TemperatureHandlerCount = 0;
            IGCL.IGCL._ctl_result_t r = (_ctl_result_t)ctlEnumTemperatureSensors(hDevice, ref TemperatureHandlerCount, null);
            if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
            {
                Helpers.ConsolePrint("GetTemperature", "ctlEnumTemperatureSensors 1 ERROR: " + r.ToString());
                return -1;
            }

            long[] pTtemperatureHandle = new long[(int)TemperatureHandlerCount];
            r = (_ctl_result_t)ctlEnumTemperatureSensors(hDevice, ref TemperatureHandlerCount, pTtemperatureHandle);
            if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
            {
                Helpers.ConsolePrint("GetTemperature", "ctlEnumTemperatureSensors 2 ERROR: " + r.ToString());
                return -1;
            }
            for (uint t = 0; t < TemperatureHandlerCount; t++)
            {
                ctl_temp_properties_t temperatureProperties = new ctl_temp_properties_t();
                    temperatureProperties.Size = Marshal.SizeOf(typeof(ctl_temp_properties_t));

                r = (_ctl_result_t)ctlTemperatureGetProperties(pTtemperatureHandle[t], ref temperatureProperties);
                if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
                {
                    Helpers.ConsolePrint("GetTemperature", "ctlTemperatureGetProperties ERROR: " + r.ToString());
                    return -1;
                }

                if (isMemTemp)
                {
                    if (temperatureProperties.type == ctl_temp_sensors_t.CTL_TEMP_SENSORS_MEMORY)
                    {
                        double temperature = 0;
                        r = ctlTemperatureGetState(pTtemperatureHandle[t], ref temperature);
                        if (r == _ctl_result_t.CTL_RESULT_SUCCESS)
                        {
                            if (double.IsNaN(temperature)) temperature = 0.0d;
                            return temperature;
                        }
                    }
                } else
                {
                    if (temperatureProperties.type == ctl_temp_sensors_t.CTL_TEMP_SENSORS_GPU)
                    {
                        double temperature = 0;
                        r = ctlTemperatureGetState(pTtemperatureHandle[t], ref temperature);
                        if (r == _ctl_result_t.CTL_RESULT_SUCCESS)
                        {
                            if (double.IsNaN(temperature)) temperature = 0.0d;
                            return temperature;
                        }
                    }
                }
            }

                return -1;
        }

        public static int GetFan(long hDevice, bool isPercent = false)
        {
            uint FanHandlerCount = 0;
            IGCL.IGCL._ctl_result_t r = ctlEnumFans(hDevice, ref FanHandlerCount, null);
            if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
            {
                Helpers.ConsolePrint("GetFan", "ctlEnumFans 1 ERROR: " + r.ToString());
                return -1;
            }

            long[] pFanHandle = new long[FanHandlerCount];
            r = (_ctl_result_t)ctlEnumFans(hDevice, ref FanHandlerCount, pFanHandle);
            if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
            {
                Helpers.ConsolePrint("GetFan", "ctlEnumFans 2 ERROR: " + r.ToString());
                return -1;
            }

            if (FanHandlerCount <= 0)
            {
                //Helpers.ConsolePrint("GetFan", "ctlEnumFans 3 ERROR: " + r.ToString());
                return -1;
            }

            ctl_fan_speed_units_t units = ctl_fan_speed_units_t.CTL_FAN_SPEED_UNITS_RPM;
            if (isPercent)
            {
                units = ctl_fan_speed_units_t.CTL_FAN_SPEED_UNITS_PERCENT;
            }
            int speed = 0;

            r = ctlFanGetState(pFanHandle[FanHandlerCount - 1], units, ref speed);
            if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
            {
                if (r == _ctl_result_t.CTL_RESULT_ERROR_UNSUPPORTED_FEATURE)
                {
                    //return -1;
                    if (isPercent && speed == 0)
                    {
                        units = ctl_fan_speed_units_t.CTL_FAN_SPEED_UNITS_RPM;
                        ctlFanGetState(pFanHandle[FanHandlerCount - 1], units, ref speed);
                        if (speed > 0) return Math.Min(5000 / speed * 10, 100);//emulate 
                        if (speed == 0) return 0;
                    }
                }
                else
                {
                    Helpers.ConsolePrint("GetFan", "ctlFanGetState ERROR: " + r.ToString());
                    return -1;
                }
            }
            else
            {
                if (double.IsNaN(speed)) speed = 0;
                if (speed > 10000) speed = 0;
                return speed;
            }
            return -1;
        }

        static double deltatimestampPower = 0;
        static double prevtimestampPower = 0;
        static double curtimestampPower = 0;
        static double prevgpuEnergyCounterPower = 0;
        static double curgpuEnergyCounterPower = 0;

        static SortedList<long, double> powerList = new SortedList<long, double>();
        static DateTime prev;
        public static double GetPower(long hDevice)
        {
            double power = 0;
            try
            {
                DateTime now = DateTime.Now;
                prev = now;
                ctl_power_telemetry_t pPowerTelemetry = new ctl_power_telemetry_t();
                pPowerTelemetry.Size = (ushort)Marshal.SizeOf(typeof(ctl_power_telemetry_t));
                IGCL.IGCL._ctl_result_t r = ctlPowerTelemetryGet(hDevice, ref pPowerTelemetry);

                if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
                {
                    Helpers.ConsolePrint("GetPower", "ctlPowerTelemetryGet: " + r.ToString());
                    return -1;
                }

                prevtimestampPower = curtimestampPower;
                curtimestampPower = pPowerTelemetry.timeStamp.value.datadouble;
                deltatimestampPower = curtimestampPower - prevtimestampPower;

                if (pPowerTelemetry.gpuEnergyCounter.bSupported)
                {
                    prevgpuEnergyCounterPower = curgpuEnergyCounterPower;
                    curgpuEnergyCounterPower = pPowerTelemetry.gpuEnergyCounter.value.datadouble;
                    power = (curgpuEnergyCounterPower - prevgpuEnergyCounterPower) / deltatimestampPower;
                    if (power > 500) power = 0;
                    if (double.IsNaN(power))
                    {
                        /*
                        byte[] d = getBytes(pPowerTelemetry);
                        File.WriteAllBytes("logs\\pPowerTelemetry.bin", d);
                        Helpers.ConsolePrint("GetPower", "curgpuEnergyCounter: " + curgpuEnergyCounterPower.ToString());
                        Helpers.ConsolePrint("GetPower", "prevgpuEnergyCounter: " + prevgpuEnergyCounterPower.ToString());
                        Helpers.ConsolePrint("GetPower", "curtimestamp: " + curtimestampPower.ToString());
                        Helpers.ConsolePrint("GetPower", "prevtimestamp: " + prevtimestampPower.ToString());
                        Helpers.ConsolePrint("GetPower", "deltatimestamp: " + deltatimestampPower.ToString());
                        */
                        power = 0;
                    }
                    return Math.Round(power);
                }


            } catch (Exception ex)
            {
                Helpers.ConsolePrint("GetPower", ex.ToString());
            }

            return -1;
        }

        static double deltatimestampLoad = 0;
        static double prevtimestampLoad = 0;
        static double curtimestampLoad = 0;
        static double prevrenderComputeActivityCounter = 0;
        static double currenderComputeActivityCounter = 0;
        public static int GetLoad(long hDevice)
        {
            int load = 0;
            try
            {
                ctl_power_telemetry_t pPowerTelemetry = new ctl_power_telemetry_t();
                pPowerTelemetry.Size = (ushort)Marshal.SizeOf(typeof(ctl_power_telemetry_t));
                IGCL.IGCL._ctl_result_t r = (_ctl_result_t)ctlPowerTelemetryGet(hDevice, ref pPowerTelemetry);
                if (r != _ctl_result_t.CTL_RESULT_SUCCESS)
                {
                    //Helpers.ConsolePrint("GetLoad", "ctlPowerTelemetryGet: " + r.ToString());
                    return -1;
                }

                prevtimestampLoad = curtimestampLoad;
                curtimestampLoad = pPowerTelemetry.timeStamp.value.datadouble;
                deltatimestampLoad = curtimestampLoad - prevtimestampLoad;

                if (pPowerTelemetry.renderComputeActivityCounter.bSupported)
                {
                    prevrenderComputeActivityCounter = currenderComputeActivityCounter;
                    currenderComputeActivityCounter = pPowerTelemetry.renderComputeActivityCounter.value.datadouble;
                    load = (int)(((currenderComputeActivityCounter - prevrenderComputeActivityCounter) / deltatimestampLoad) * 100);
                    if (double.IsNaN(load)) load = 0;
                    if (load < -2) load = 0;
                    return load;
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetLoad", ex.ToString());
            }
            return -1;
        }

        static T BytesToStructure<T>(byte[] bytes)
        {
            int size = Marshal.SizeOf(typeof(T));
            if (bytes.Length < size)
                throw new Exception("Invalid parameter");
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, ptr, size);
                return (T)Marshal.PtrToStructure(ptr, typeof(T));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static byte[] getBytes(ctl_power_telemetry_t str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(str, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return arr;
        }
        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private static List<OpenCLDevice> IntelDeviceCreationFallback(List<OpenCLDevice> IntelDevices)
        {
            Helpers.ConsolePrint(Tag, "Using Intel device creation FALLBACK UnReliable mappings");
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("QueryIntel [FALLBACK query] devices: ");

            // get video Intel controllers and sort them by RAM
            // (find a way to get PCI BUS Numbers from PNPDeviceID)
            var IntelVideoControllers = _availableControllers.Where(vcd =>
                (vcd.Name.ToLower().Contains("intel") && vcd.Name.ToLower().Contains("arc"))).ToList();
                //(vcd.Name.ToLower().Contains("intel") && (vcd.Name.ToLower().Contains("iris") ||
                //vcd.Name.ToLower().Contains("arc")))).ToList();
            // sort by ram not ideal
            IntelVideoControllers.Sort((a, b) => (int)(a.AdapterRam - b.AdapterRam));
            IntelDevices.Sort((a, b) => (int)(a._CL_DEVICE_GLOBAL_MEM_SIZE - b._CL_DEVICE_GLOBAL_MEM_SIZE));
            var minCount = Math.Min(IntelVideoControllers.Count, IntelDevices.Count);



            for (var i = 0; i < minCount; ++i)
            {
                ComputeDeviceManager.Available.HasIntel = true;
                var deviceName = IntelVideoControllers[i].Name;
                if (IntelVideoControllers[i].InfSection == null)
                    IntelVideoControllers[i].InfSection = "";
                var newIntelDev = new IntelGpuDevice(IntelDevices[i], false,
                    IntelVideoControllers[i].InfSection,
                    false)
                {
                    DeviceName = deviceName,
                    UUID = "UNUSED"
                };
                var isDisabledGroup = ConfigManager.GeneralConfig.DeviceDetection
                    .DisableDetectionINTEL;
                var skipOrAdd = isDisabledGroup ? "SKIPED" : "ADDED";
                var isDisabledGroupStr = isDisabledGroup ? " (Intel group disabled)" : "";
                var etherumCapableStr = newIntelDev.IsEtherumCapable() ? "YES" : "NO";

                ComputeDeviceManager.Available.Devices.Add(
                    new IntelComputeDevice(newIntelDev, ++ComputeDeviceManager.Query.GpuCount, true, -1));
                // just in case
                try
                {
                    stringBuilder.AppendLine($"\t{skipOrAdd} device{isDisabledGroupStr}:");
                    stringBuilder.AppendLine($"\t\tID: {newIntelDev.DeviceID}");
                    stringBuilder.AppendLine($"\t\tAdapterIndex: {newIntelDev.AdapterIndex}");
                    stringBuilder.AppendLine($"\t\tBusID: {newIntelDev.BusID}");
                    stringBuilder.AppendLine($"\t\tManufacturer: {newIntelDev.IntelManufacturer}");
                    stringBuilder.AppendLine($"\t\tMonitorConnected: {newIntelDev.MonitorConnected}");
                    stringBuilder.AppendLine($"\t\tNewUUID: {newIntelDev.NewUUID}");
                    stringBuilder.AppendLine($"\t\tNAME: {newIntelDev.DeviceName}");
                    stringBuilder.AppendLine($"\t\tCODE_NAME: {newIntelDev.Codename}");
                    stringBuilder.AppendLine($"\t\tUUID: {newIntelDev.UUID}");
                    stringBuilder.AppendLine(
                        $"\t\tMEMORY: {newIntelDev.DeviceGlobalMemory}");
                }
                catch
                {
                }
            }

            Helpers.ConsolePrint(Tag, stringBuilder.ToString());

            return IntelDevices;
        }


        private struct BusIdInfo
        {
            public string Name;
            public string MF;
            public string MF_Intel;
            public string Uuid;
            public string InfSection;
            public int DeviceIndex;
            public long DeviceHandle;
        }
    }
}
