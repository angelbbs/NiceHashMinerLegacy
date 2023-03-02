using ATI.ADL;
using Microsoft.Win32;
using NiceHashMiner.Configs;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.UUID;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using static IGCL.IGCL;

namespace NiceHashMiner.Devices.Querying
{
    public class IntelQuery
    {
        private const string Tag = "IntelQuery";
        private const int IntelVendorID = 8086;

        private readonly List<VideoControllerData> _availableControllers;
        private readonly Dictionary<int, BusIdInfo> _busIdInfos = new Dictionary<int, BusIdInfo>();
        private readonly List<string> _IntelDeviceUuid = new List<string>();

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

        public IntelQuery(List<VideoControllerData> availControllers)
        {
            _availableControllers = availControllers;
        }

        public List<OpenCLDevice> QueryIntel(bool openCLSuccess, OpenCLJsonData openCLData)
        {
            Helpers.ConsolePrint(Tag, "QueryIntel START");
            var IntelDevices = openCLSuccess ? ProcessDevices(openCLData) : new List<OpenCLDevice>();
            Helpers.ConsolePrint(Tag, "QueryIntel END");
            return IntelDevices;
        }

        private List<OpenCLDevice> ProcessDevices(OpenCLJsonData openCLData)
        {
            var IntelOclDevices = new List<OpenCLDevice>();
            var IntelDevices = new List<OpenCLDevice>();

            var IntelPlatformNumFound = false;
            foreach (var oclEl in openCLData.Platforms)
            {
                /*
                if (!oclEl.PlatformName.ToLower().Contains("intel")) continue;
                if (!oclEl.PlatformName.ToLower().Contains("arc")) continue;
                if (!oclEl.PlatformName.ToLower().Contains("iris")) continue;
                */
                IntelPlatformNumFound = true;
                var IntelOpenCLPlatformStringKey = oclEl.PlatformName;
                ComputeDeviceManager.Available.IntelOpenCLPlatformNum = oclEl.PlatformNum;
                IntelOclDevices = oclEl.Devices;
                Helpers.ConsolePrint(Tag,
                    $"Intel platform found: Key: {IntelOpenCLPlatformStringKey}, Num: {ComputeDeviceManager.Available.IntelOpenCLPlatformNum}");
                break;
            }

            if (!IntelPlatformNumFound) return IntelDevices;

            // get only Intel gpus
            string PNPDeviceID = "";
            string[] _PNPDeviceID;
            foreach (var oclDev in IntelOclDevices)
            {
                if (oclDev._CL_DEVICE_TYPE.Contains("GPU"))
                {
                    uint devNum = oclDev.DeviceID;
                    string UUID = "";
                    string _mf = "";
                    string _InfSection = "";
                    foreach (var vc in ComputeDeviceManager.Query.AvaliableVideoControllers)
                    {
                        if (vc.ID == devNum)
                        {
                            _PNPDeviceID = vc.PnpDeviceID.Split('\\');
                            UUID = vc.PnpDeviceID.Split('&')[0] + "&" + vc.PnpDeviceID.Split('&')[1] + "_" + vc.PnpDeviceID.Split('&')[4];
                            UUID = UUID.Replace("\\", "_");

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
                                    oclDev.BUS_ID = _busID;
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
                        MF = _mf,
                        Uuid = UUID,
                        InfSection = _InfSection,
                        Adl1Index = (int)oclDev.DeviceID
                    };

                    _busIdInfos.Add(oclDev.BUS_ID, info);
                }
            }

            if (IntelDevices.Count == 0)
            {
                Helpers.ConsolePrint(Tag, "Intel GPUs count is 0");
                return IntelDevices;
            }

            Helpers.ConsolePrint(Tag, "Intel GPUs count : " + IntelDevices.Count);

            var isBusIDOk = true;
            // check if buss ids are unique and different from -1
            {
                var busIDs = new HashSet<int>();
                // Override Intel bus IDs
                //var overrides = ConfigManager.GeneralConfig.OverrideIntelBusIds.Split(',');
                for (var i = 0; i < IntelDevices.Count; i++)
                {
                    var IntelOclDev = IntelDevices[i];
                    /*
                    if (overrides.Count() > i &&
                        int.TryParse(overrides[i], out var overrideBus) &&
                        overrideBus >= 0)
                    {
                        IntelOclDev.BUS_ID = overrideBus;
                    }
                    */
                    if (IntelOclDev.BUS_ID < 0 || !_busIdInfos.ContainsKey(IntelOclDev.BUS_ID))
                    {
                        isBusIDOk = false;
                        break;
                    }

                    busIDs.Add(IntelOclDev.BUS_ID);
                }

                // check if unique
                isBusIDOk = isBusIDOk && busIDs.Count == IntelDevices.Count;
            }
            // print BUS id status
            Helpers.ConsolePrint(Tag,
                isBusIDOk
                    ? "Intel Bus IDs are unique and valid. OK"
                    : "Intel Bus IDs IS INVALID. Using fallback Intel detection mode");

            ///////
            // Intel device creation (in NHM context)
            if (isBusIDOk)
            {
                return IntelDeviceCreationPrimary(IntelDevices);
            }

            return IntelDeviceCreationFallback(IntelDevices);
        }

        private List<OpenCLDevice> IntelDeviceCreationPrimary(List<OpenCLDevice> IntelDevices)
        {
            ctl_init_args_t CtlInitArgs = new ctl_init_args_t();
            CtlInitArgs.AppVersion = (1 << 16) | (1 & 0x0000ffff);
            CtlInitArgs.flags = (1 << 0);
            CtlInitArgs.Size = (uint)Marshal.SizeOf(typeof(ctl_init_args_t));
            CtlInitArgs.Version = 0;
            var auid = new ctl_application_id_t();
            auid.Data1 = 0;
            auid.Data2 = 0;
            auid.Data3 = 0;
            auid.Data40 = 0;
            auid.Data41 = 0;
            CtlInitArgs.ApplicationUID = auid;
            ulong hAPIHandle = 0;
            uint Adapter_count = 0;
            ulong[] hDevices = new ulong[1];
            //var r = IGCL.IGCL.TestIntel(ref CtlInitArgs, ref hAPIHandle);
            var r = IGCL.IGCL.ctlInit(ref CtlInitArgs, ref hAPIHandle);
            if (r == IGCL.IGCL._ctl_result_t.CTL_RESULT_SUCCESS)
            {
                Helpers.ConsolePrint("IGCL **********", "Handle: " + hAPIHandle.ToString());
                //Helpers.ConsolePrint("IGCL **********", "ApplicationUID: " + CtlInitArgs.ApplicationUID);
                //Helpers.ConsolePrint("IGCL **********", "SupportedVersion: " + CtlInitArgs.SupportedVersion.ToString());
                //Helpers.ConsolePrint("IGCL **********", "flags: " + CtlInitArgs.flags);
                r = IGCL.IGCL.ctlEnumerateDevices(hAPIHandle, out Adapter_count, hDevices);
                //r = IGCL.IGCL.ctlEnumerateDevices(hAPIHandle, ref Adapter_count);
                Helpers.ConsolePrint("IGCL **********", "Adapter_count: " + Adapter_count.ToString());
                Helpers.ConsolePrint("IGCL **********", "hDevices count: " + hDevices.Count().ToString());
                Helpers.ConsolePrint("IGCL **********", "result: " + r.ToString());
                Helpers.ConsolePrint("IGCL **********", "device handle: " + hDevices[0].ToString());
                /*
                foreach(var d in hDevices)
                {
                    Helpers.ConsolePrint("IGCL **********", "device handle: " + d.ToString());
                }
                */
            }
            else
            {
                Helpers.ConsolePrint("IGCL **********", r.ToString());
            }
            
            //Process.GetCurrentProcess().Kill();


            Helpers.ConsolePrint(Tag, "Using Intel device creation DEFAULT Reliable mappings");
            Helpers.ConsolePrint(Tag,
                IntelDevices.Count == _IntelDeviceUuid.Count
                    ? "Intel OpenCL query COUNTS GOOD/SAME"
                    : "Intel OpenCL query COUNTS DIFFERENT/BAD");
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("QueryIntel [DEFAULT query] devices: ");
            try
            {
                foreach (var dev in IntelDevices.OrderBy(i => i.BUS_ID))//****************************************************
                //foreach (var dev in IntelDevices)
                {
                    ComputeDeviceManager.Available.HasIntel = true;

                    var busID = dev.BUS_ID;
                    var gpuRAM = dev._CL_DEVICE_GLOBAL_MEM_SIZE + 16384 * 1024 + 1375731712;//6442450944
                    //var man = dev._CL_DEVICE_VENDOR_ID;

                    if (busID != -1 && _busIdInfos.ContainsKey(busID))
                    {
                        var deviceName = _busIdInfos[busID].Name;
                        var manufacturer = _busIdInfos[busID].MF;

                        IntelGpuDevice newIntelDev = new IntelGpuDevice(dev, false,
                            _busIdInfos[busID].InfSection, false)
                        {
                            DeviceName = deviceName,
                            UUID = _busIdInfos[busID].Uuid,
                            AdapterIndex = _busIdInfos[busID].Adl1Index,
                            IntelManufacturer = _busIdInfos[busID].MF,
                            DeviceGlobalMemory = gpuRAM
                        };
                        
                        int _prevmonitorRefreshRate = 0;
                        //*************
                        string PnpDeviceID = "";
                        ulong gpumem = 0;
                        ulong gpumemadd = 1048576;//add 1MB to gpumem
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
                                    Helpers.ConsolePrint("IntelQUERY", deviceName + " GPU mem size is not equal: " + newIntelDev.DeviceGlobalMemory.ToString() + " < " + gpumem.ToString());
                                    newIntelDev.DeviceGlobalMemory = gpumem;
                                    dev._CL_DEVICE_GLOBAL_MEM_SIZE = gpumem;
                                }
                            }
                        }
                        //*************
                        var isDisabledGroup = ConfigManager.GeneralConfig.DeviceDetection
                            .DisableDetectionINTEL;
                        var skipOrAdd = isDisabledGroup ? "SKIPED" : "ADDED";
                        var isDisabledGroupStr = isDisabledGroup ? " (Intel group disabled)" : "";
                        var etherumCapableStr = newIntelDev.IsEtherumCapable() ? "YES" : "NO";

                        ComputeDeviceManager.Available.Devices.Add(
                            new IntelComputeDevice(newIntelDev, ++ComputeDeviceManager.Query.GpuCount, false,
                                _busIdInfos[busID].Adl2Index));
                        var infSection = newIntelDev.InfSection;
                        //var PnpDeviceID = dev.PnpDeviceID;
                        //var PnpDeviceID = vidController.PnpDeviceID;
                        var infoToHashed = $"{newIntelDev.DeviceID}--{DeviceType.INTEL}--{newIntelDev.DeviceGlobalMemory}--{newIntelDev.Codename}--{newIntelDev.DeviceName}";
                        infoToHashed += newIntelDev.UUID.Replace("PCI_", "PCI/");//PnpDeviceID неверный!

                        var uuidHEX = UUID.GetHexUUID(infoToHashed);
                        var Newuuid = $"Intel-{uuidHEX}";
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
                Helpers.ConsolePrint("IntelDeviceCreationPrimary", er.ToString());
            }

            Helpers.ConsolePrint(Tag, stringBuilder.ToString());

            return IntelDevices;
        }

        private List<OpenCLDevice> IntelDeviceCreationFallback(List<OpenCLDevice> IntelDevices)
        {
            Helpers.ConsolePrint(Tag, "Using Intel device creation FALLBACK UnReliable mappings");
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("QueryIntel [FALLBACK query] devices: ");

            // get video Intel controllers and sort them by RAM
            // (find a way to get PCI BUS Numbers from PNPDeviceID)
            var IntelVideoControllers = _availableControllers.Where(vcd =>
                (vcd.Name.ToLower().Contains("intel") && vcd.Name.ToLower().Contains("arc"))).ToList();
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
                    stringBuilder.AppendLine($"\t\tETHEREUM: {etherumCapableStr}");
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
            public string Uuid;
            public string InfSection;
            public int Adl1Index;
            public int Adl2Index;
        }
    }
}
