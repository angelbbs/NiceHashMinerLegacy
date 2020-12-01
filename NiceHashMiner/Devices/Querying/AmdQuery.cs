﻿/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATI.ADL;
using NiceHashMiner.Configs;
using NiceHashMiner.Forms;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.UUID;

namespace NiceHashMiner.Devices.Querying
{
    public class AmdQuery
    {
        private const string Tag = "AmdQuery";
        private const int AmdVendorID = 1002;

        private readonly List<VideoControllerData> _availableControllers;
        private readonly Dictionary<string, bool> _driverOld = new Dictionary<string, bool>();
        private readonly Dictionary<string, bool> _noNeoscryptLyra2 = new Dictionary<string, bool>();
        private readonly Dictionary<int, BusIdInfo> _busIdInfos = new Dictionary<int, BusIdInfo>();
        //private readonly SortedDictionary<int, BusIdInfo> _busIdInfos = new SortedDictionary<int, BusIdInfo>();
        private readonly List<string> _amdDeviceUuid = new List<string>();

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

        public AmdQuery(List<VideoControllerData> availControllers)
        {
            _availableControllers = availControllers;
        }

        public List<OpenCLDevice> QueryAmd(bool openCLSuccess, OpenCLJsonData openCLData)
        {
            Helpers.ConsolePrint(Tag, "QueryAMD START");

            DriverCheck();

            var amdDevices = openCLSuccess ? ProcessDevices(openCLData) : new List<OpenCLDevice>();
            
            Helpers.ConsolePrint(Tag, "QueryAMD END");

            return amdDevices;
        }

        private void DriverCheck()
        {
            // check the driver version bool EnableOptimizedVersion = true;
            var showWarningDialog = false;

            foreach (var vidContrllr in _availableControllers)
            {
                Helpers.ConsolePrint(Tag,
                    $"Checking AMD device (driver): {vidContrllr.Name} ({vidContrllr.DriverVersion})");

                _driverOld[vidContrllr.Name] = false;
                _noNeoscryptLyra2[vidContrllr.Name] = false;
                var sgminerNoNeoscryptLyra2RE = new Version("21.19.164.1");

                // TODO checking radeon drivers only?
                if ((!vidContrllr.Name.Contains("AMD") && !vidContrllr.Name.Contains("Radeon")) ||
                    showWarningDialog) continue;

                var amdDriverVersion = new Version(vidContrllr.DriverVersion);

                if (!ConfigManager.GeneralConfig.ForceSkipAMDNeoscryptLyraCheck)
                {
                    var greaterOrEqual = amdDriverVersion.CompareTo(sgminerNoNeoscryptLyra2RE) >= 0;
                    if (greaterOrEqual)
                    {
                        _noNeoscryptLyra2[vidContrllr.Name] = true;
                        /*
                        Helpers.ConsolePrint(Tag,
                            "Driver version seems to be " + sgminerNoNeoscryptLyra2RE +
                            " or higher. NeoScrypt and Lyra2REv2 will be removed from list");
                            */
                    }
                }


                if (amdDriverVersion.Major >= 15) continue;

                showWarningDialog = true;
                _driverOld[vidContrllr.Name] = true;
                Helpers.ConsolePrint(Tag,
                    "WARNING!!! Old AMD GPU driver detected! All optimized versions disabled, mining " +
                    "speed will not be optimal. Consider upgrading AMD GPU driver.");
            }

            if (ConfigManager.GeneralConfig.ShowDriverVersionWarning && showWarningDialog)
            {
                Form warningDialog = new DriverVersionConfirmationDialog();
                warningDialog.ShowDialog();
                warningDialog = null;
            }
        }

        //private List<OpenCLDevice> ProcessDevices(IEnumerable<OpenCLJsonData> openCLData)
        private List<OpenCLDevice> ProcessDevices(OpenCLJsonData openCLData)
        {
            var amdOclDevices = new List<OpenCLDevice>();
            var amdDevices = new List<OpenCLDevice>();

            var amdPlatformNumFound = false;
            foreach (var oclEl in openCLData.Platforms)
            {
                if (!oclEl.PlatformName.Contains("AMD") && !oclEl.PlatformName.Contains("amd")) continue;
                amdPlatformNumFound = true;
                var amdOpenCLPlatformStringKey = oclEl.PlatformName;
                ComputeDeviceManager.Available.AmdOpenCLPlatformNum = oclEl.PlatformNum;
                amdOclDevices = oclEl.Devices;
                Helpers.ConsolePrint(Tag,
                    $"AMD platform found: Key: {amdOpenCLPlatformStringKey}, Num: {ComputeDeviceManager.Available.AmdOpenCLPlatformNum}");
                break;
            }
            
            if (!amdPlatformNumFound) return amdDevices;

            // get only AMD gpus
            {
                foreach (var oclDev in amdOclDevices)
                {
                    if (oclDev._CL_DEVICE_TYPE.Contains("GPU"))
                    {
                        amdDevices.Add(oclDev);
                    }
                }
            }

            if (amdDevices.Count == 0)
            {
                Helpers.ConsolePrint(Tag, "AMD GPUs count is 0");
                return amdDevices;
            }

            Helpers.ConsolePrint(Tag, "AMD GPUs count : " + amdDevices.Count);
            Helpers.ConsolePrint(Tag, "AMD Getting device name and serial from ADL");
            // ADL
            var isAdlInit = true;
            try
            {
                isAdlInit = QueryAdl();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(Tag, "AMD ADL exception: " + ex.Message);
                isAdlInit = false;
            }

            var isBusIDOk = true;
            // check if buss ids are unique and different from -1
            {
                var busIDs = new HashSet<int>();
                // Override AMD bus IDs
                var overrides = ConfigManager.GeneralConfig.OverrideAMDBusIds.Split(',');
                for (var i = 0; i < amdDevices.Count; i++)
                {
                    var amdOclDev = amdDevices[i];
                    if (overrides.Count() > i &&
                        int.TryParse(overrides[i], out var overrideBus) &&
                        overrideBus >= 0)
                    {
                        amdOclDev.BUS_ID = overrideBus;
                    }

                    if (amdOclDev.BUS_ID < 0 || !_busIdInfos.ContainsKey(amdOclDev.BUS_ID))
                    {
                        isBusIDOk = false;
                        break;
                    }

                    busIDs.Add(amdOclDev.BUS_ID);
                }

                // check if unique
                isBusIDOk = isBusIDOk && busIDs.Count == amdDevices.Count;
            }
            // print BUS id status
            Helpers.ConsolePrint(Tag,
                isBusIDOk
                    ? "AMD Bus IDs are unique and valid. OK"
                    : "AMD Bus IDs IS INVALID. Using fallback AMD detection mode");

            ///////
            // AMD device creation (in NHM context)
            if (isAdlInit && isBusIDOk)
            {
                return AmdDeviceCreationPrimary(amdDevices);
            }

            return AmdDeviceCreationFallback(amdDevices);
        }

        private List<OpenCLDevice> AmdDeviceCreationPrimary(List<OpenCLDevice> amdDevices)
        {
            Helpers.ConsolePrint(Tag, "Using AMD device creation DEFAULT Reliable mappings");
            Helpers.ConsolePrint(Tag,
                amdDevices.Count == _amdDeviceUuid.Count
                    ? "AMD OpenCL and ADL AMD query COUNTS GOOD/SAME"
                    : "AMD OpenCL and ADL AMD query COUNTS DIFFERENT/BAD");
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("QueryAMD [DEFAULT query] devices: ");
            try
            {
                foreach (var dev in amdDevices.OrderBy(i => i.BUS_ID))//****************************************************
                //foreach (var dev in amdDevices)
                {
                    ComputeDeviceManager.Available.HasAmd = true;

                    var busID = dev.BUS_ID;
                    var gpuRAM = dev._CL_DEVICE_GLOBAL_MEM_SIZE;
                    //var man = dev._CL_DEVICE_VENDOR_ID;

                    if (busID != -1 && _busIdInfos.ContainsKey(busID))
                    {
                        var deviceName = _busIdInfos[busID].Name;
                        var manufacturer = _busIdInfos[busID].MF;

                        var newAmdDev = new AmdGpuDevice(dev, _driverOld[deviceName],
                            _busIdInfos[busID].InfSection, _noNeoscryptLyra2[deviceName])
                        {
                            DeviceName = deviceName,
                            UUID = _busIdInfos[busID].Uuid,
                            AdapterIndex = _busIdInfos[busID].Adl1Index,
                            AMDManufacturer = _busIdInfos[busID].MF,
                            DeviceGlobalMemory = gpuRAM
                        };
                        //PCI_VEN_1002&DEV_67DF_18803EC9
                        //PNPDeviceID PCI\VEN_1002&DEV_6811&SUBSYS_2015174B&REV_81\4&14486AD3&0&00E0       
                        //UUID: PCI_VEN_1002&DEV_6811_14486AD3
                        //PCI\VEN_1002&DEV_67DF&SUBSYS_2379148C&REV_EF powercolor

                        //*************
                        string PnpDeviceID = "";
                        ulong gpumem = 0;
                        ulong gpumemadd = 1048576; //add 1MB to gpumem
                        var moc = new ManagementObjectSearcher("root\\CIMV2",
                            "SELECT * FROM Win32_VideoController WHERE PNPDeviceID LIKE 'PCI%'").Get();

                        foreach (var manObj in moc)
                        {
                            ulong.TryParse(SafeGetProperty(manObj, "AdapterRAM"), out var memTmp);
                            PnpDeviceID = SafeGetProperty(manObj, "PNPDeviceID");
                            Helpers.ConsolePrint("AMDQUERY", "DeviceID: " + SafeGetProperty(manObj, "DeviceID"));
                            gpumem = memTmp + gpumemadd‬;

                        }

                        if (PnpDeviceID.Split('&')[4].Equals(newAmdDev.UUID.Split('_')[4]))
                        {
                            Helpers.ConsolePrint("AMDQUERY", "PnpDeviceID.Split('&')[4]=" + PnpDeviceID.Split('&')[4].ToString() +
                                " newAmdDev.UUID.Split('_')[4]" + newAmdDev.UUID.Split('_')[4].ToString() +
                                " " + dev._CL_DEVICE_NAME);
                            if (newAmdDev.DeviceGlobalMemory < gpumem)
                            {
                                Helpers.ConsolePrint("AMDQUERY", deviceName + " GPU mem size is not equal: " + newAmdDev.DeviceGlobalMemory.ToString() + " < " + gpumem.ToString());
                                newAmdDev.DeviceGlobalMemory = gpumem;
                                dev._CL_DEVICE_GLOBAL_MEM_SIZE = gpumem;
                            }
                        }

                        //*************
                        var isDisabledGroup = ConfigManager.GeneralConfig.DeviceDetection
                            .DisableDetectionAMD;
                        var skipOrAdd = isDisabledGroup ? "SKIPED" : "ADDED";
                        var isDisabledGroupStr = isDisabledGroup ? " (AMD group disabled)" : "";
                        var etherumCapableStr = newAmdDev.IsEtherumCapable() ? "YES" : "NO";

                        ComputeDeviceManager.Available.Devices.Add(
                            new AmdComputeDevice(newAmdDev, ++ComputeDeviceManager.Query.GpuCount, false,
                                _busIdInfos[busID].Adl2Index));
                        var infSection = newAmdDev.InfSection;
                        //var PnpDeviceID = dev.PnpDeviceID;
                        //var PnpDeviceID = vidController.PnpDeviceID;
                        var infoToHashed = $"{newAmdDev.DeviceID}--{DeviceType.AMD}--{newAmdDev.DeviceGlobalMemory}--{newAmdDev.Codename}--{newAmdDev.DeviceName}";
                        infoToHashed += newAmdDev.UUID.Replace("PCI_", "PCI/");//PnpDeviceID неверный!
                                                                               //var vidCtrl = availableVideoControllers?.Where(vid => vid.PCI_BUS_ID == oclDev.AMD_BUS_ID).FirstOrDefault() ?? null;
                                                                               //if (vidCtrl != null)
                                                                               //{
                                                                               //  infSection = vidCtrl.InfSection;
                                                                               // infoToHashed += vidCtrl.PnpDeviceID;
                                                                               //}
                                                                               //else
                                                                               //{
                                                                               //  Logger.Info(Tag, $"TryQueryAMDDevicesAsync cannot find VideoControllerData with bus ID {oclDev.AMD_BUS_ID}");
                                                                               //}

                        var uuidHEX = UUID.GetHexUUID(infoToHashed);
                        var Newuuid = $"AMD-{uuidHEX}";
                        newAmdDev.NewUUID = Newuuid;
                        // just in case 
                        try
                        {
                            stringBuilder.AppendLine($"\t{skipOrAdd} device{isDisabledGroupStr}:");
                            stringBuilder.AppendLine($"\t\tID: {newAmdDev.DeviceID}");
                            stringBuilder.AppendLine($"\t\tNAME: {newAmdDev.DeviceName}");
                            stringBuilder.AppendLine($"\t\tCODE_NAME: {newAmdDev.Codename}");
                            stringBuilder.AppendLine($"\t\tManufacturer: {newAmdDev.AMDManufacturer}");
                            stringBuilder.AppendLine($"\t\tUUID: {newAmdDev.UUID}");
                            stringBuilder.AppendLine($"\t\tNewUUID: {newAmdDev.NewUUID}");
                            stringBuilder.AppendLine($"\t\tBusID: {newAmdDev.BusID}");
                            stringBuilder.AppendLine($"\t\tDeviceID: {newAmdDev.DeviceID}");
                            stringBuilder.AppendLine($"\t\tInfSection: {newAmdDev.InfSection}");
                            stringBuilder.AppendLine($"\t\tMEMORY: {newAmdDev.DeviceGlobalMemory}");
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
                Helpers.ConsolePrint("AmdDeviceCreationPrimary", er.ToString());
            }

            Helpers.ConsolePrint(Tag, stringBuilder.ToString());

            return amdDevices;
        }

        private List<OpenCLDevice> AmdDeviceCreationFallback(List<OpenCLDevice> amdDevices)
        {
            Helpers.ConsolePrint(Tag, "Using AMD device creation FALLBACK UnReliable mappings");
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("QueryAMD [FALLBACK query] devices: ");

            // get video AMD controllers and sort them by RAM
            // (find a way to get PCI BUS Numbers from PNPDeviceID)
            var amdVideoControllers = _availableControllers.Where(vcd =>
                vcd.Name.ToLower().Contains("amd") || vcd.Name.ToLower().Contains("radeon") ||
                vcd.Name.ToLower().Contains("firepro")).ToList();
            // sort by ram not ideal 
            amdVideoControllers.Sort((a, b) => (int) (a.AdapterRam - b.AdapterRam));
            amdDevices.Sort((a, b) => (int) (a._CL_DEVICE_GLOBAL_MEM_SIZE - b._CL_DEVICE_GLOBAL_MEM_SIZE));
            var minCount = Math.Min(amdVideoControllers.Count, amdDevices.Count);

            for (var i = 0; i < minCount; ++i)
            {
                ComputeDeviceManager.Available.HasAmd = true;

                var deviceName = amdVideoControllers[i].Name;
                if (amdVideoControllers[i].InfSection == null)
                    amdVideoControllers[i].InfSection = "";
                var newAmdDev = new AmdGpuDevice(amdDevices[i], _driverOld[deviceName],
                    amdVideoControllers[i].InfSection,
                    _noNeoscryptLyra2[deviceName])
                {
                    DeviceName = deviceName,
                    UUID = "UNUSED"
                };
                var isDisabledGroup = ConfigManager.GeneralConfig.DeviceDetection
                    .DisableDetectionAMD;
                var skipOrAdd = isDisabledGroup ? "SKIPED" : "ADDED";
                var isDisabledGroupStr = isDisabledGroup ? " (AMD group disabled)" : "";
                var etherumCapableStr = newAmdDev.IsEtherumCapable() ? "YES" : "NO";

                ComputeDeviceManager.Available.Devices.Add(
                    new AmdComputeDevice(newAmdDev, ++ComputeDeviceManager.Query.GpuCount, true, -1));
                // just in case 
                try
                {
                    stringBuilder.AppendLine($"\t{skipOrAdd} device{isDisabledGroupStr}:");
                    stringBuilder.AppendLine($"\t\tID: {newAmdDev.DeviceID}");
                    stringBuilder.AppendLine($"\t\tNAME: {newAmdDev.DeviceName}");
                    stringBuilder.AppendLine($"\t\tCODE_NAME: {newAmdDev.Codename}");
                    stringBuilder.AppendLine($"\t\tUUID: {newAmdDev.UUID}");
                    stringBuilder.AppendLine(
                        $"\t\tMEMORY: {newAmdDev.DeviceGlobalMemory}");
                    stringBuilder.AppendLine($"\t\tETHEREUM: {etherumCapableStr}");
                }
                catch
                {
                }
            }

            Helpers.ConsolePrint(Tag, stringBuilder.ToString());

            return amdDevices;
        }

        private bool QueryAdl()
        {
            // ADL does not get devices in order map devices by bus number
            // bus id, <name, uuid>
            var isAdlInit = true;

            var adlRet = -1;
            var numberOfAdapters = 0;
            var adl2Control = IntPtr.Zero;

            if (null != ADL.ADL_Main_Control_Create)
                // Second parameter is 1: Get only the present adapters
                adlRet = ADL.ADL_Main_Control_Create(ADL.ADL_Main_Memory_Alloc, 1);
            if (ADL.ADL_SUCCESS == adlRet)
            {
                ADL.ADL_Adapter_NumberOfAdapters_Get?.Invoke(ref numberOfAdapters);
                Helpers.ConsolePrint(Tag, "Number Of Adapters: " + numberOfAdapters);

                if (0 < numberOfAdapters)
                {
                    // Get OS adpater info from ADL
                    var osAdapterInfoData = new ADLAdapterInfoArray();

                    if (null != ADL.ADL_Adapter_AdapterInfo_Get)
                    {
                        var size = Marshal.SizeOf(osAdapterInfoData);
                        var adapterBuffer = Marshal.AllocCoTaskMem(size);
                        Marshal.StructureToPtr(osAdapterInfoData, adapterBuffer, false);

                        adlRet = ADL.ADL_Adapter_AdapterInfo_Get(adapterBuffer, size);

                        var adl2Ret = -1;
                        if (ADL.ADL2_Main_Control_Create != null)
                            adl2Ret = ADL.ADL2_Main_Control_Create(ADL.ADL_Main_Memory_Alloc, 0, ref adl2Control);

                        var adl2Info = new ADLAdapterInfoArray();
                        var size2 = Marshal.SizeOf(adl2Info);
                        var buffer = Marshal.AllocCoTaskMem(size2);
                        if (adl2Ret == ADL.ADL_SUCCESS && ADL.ADL2_Adapter_AdapterInfo_Get != null)
                        {
                            Marshal.StructureToPtr(adl2Info, buffer, false);
                            adl2Ret = ADL.ADL2_Adapter_AdapterInfo_Get(adl2Control, buffer, Marshal.SizeOf(adl2Info));
                        }
                        else
                        {
                            adl2Ret = -1;
                        }

                        if (adl2Ret == ADL.ADL_SUCCESS)
                        {
                            adl2Info = (ADLAdapterInfoArray) Marshal.PtrToStructure(buffer, adl2Info.GetType());
                        }

                        if (ADL.ADL_SUCCESS == adlRet)
                        {
                            osAdapterInfoData =
                                (ADLAdapterInfoArray) Marshal.PtrToStructure(adapterBuffer,
                                    osAdapterInfoData.GetType());
                            var isActive = 0;

                            for (var i = 0; i < numberOfAdapters; i++)
                            {
                                // Check if the adapter is active
                                if (null != ADL.ADL_Adapter_Active_Get)
                                    adlRet = ADL.ADL_Adapter_Active_Get(
                                        osAdapterInfoData.ADLAdapterInfo[i].AdapterIndex, ref isActive);

                                if (ADL.ADL_SUCCESS != adlRet) continue;

                                // we are looking for amd
                                // TODO check discrete and integrated GPU separation
                                var vendorID = osAdapterInfoData.ADLAdapterInfo[i].VendorID;
                                var devName = osAdapterInfoData.ADLAdapterInfo[i].AdapterName;

                                if (vendorID != AmdVendorID && !devName.ToLower().Contains("amd") &&
                                    !devName.ToLower().Contains("radeon") &&
                                    !devName.ToLower().Contains("firepro")) continue;

                                var pnpStr = osAdapterInfoData.ADLAdapterInfo[i].PNPString;
                                // find vi controller pnp
                                var infSection = "";
                                string mf = "";
                                foreach (var vCtrl in _availableControllers)
                                {
                                    if (vCtrl.PnpDeviceID == pnpStr)
                                    {
                                        mf = vCtrl.Manufacturer;
                                        infSection = vCtrl.InfSection;
                                    }
                                }

                                var backSlashLast = pnpStr.LastIndexOf('\\');
                                var serial = pnpStr.Substring(backSlashLast, pnpStr.Length - backSlashLast);
                                var end0 = serial.IndexOf('&');
                                var end1 = serial.IndexOf('&', end0 + 1);
                                // get serial
                                serial = serial.Substring(end0 + 1, end1 - end0 - 1);

                                var udid = osAdapterInfoData.ADLAdapterInfo[i].UDID;
                                const int pciVenIDStrSize = 21; // PCI_VEN_XXXX&DEV_XXXX
                                var uuid = udid.Substring(0, pciVenIDStrSize) + "_" + serial;
                                var busId = osAdapterInfoData.ADLAdapterInfo[i].BusNumber;
                                var index = osAdapterInfoData.ADLAdapterInfo[i].AdapterIndex;

                                if (_amdDeviceUuid.Contains(uuid)) continue;

                                try
                                {
                                    Helpers.ConsolePrint(Tag,
                                        $"ADL device added BusNumber:{busId}  NAME:{devName}  MANUFACTURER:{mf}  UUID:{uuid}");
                                }
                                catch (Exception e)
                                {
                                    Helpers.ConsolePrint(Tag, e.Message);
                                }

                                _amdDeviceUuid.Add(uuid);
                                //_busIds.Add(OSAdapterInfoData.ADLAdapterInfo[i].BusNumber);
                                //_amdDeviceName.Add(devName);

                                if (_busIdInfos.ContainsKey(busId)) continue;

                                var adl2Index = -1;
                                if (adl2Ret == ADL.ADL_SUCCESS)
                                {
                                    adl2Index = adl2Info.ADLAdapterInfo
                                        .FirstOrDefault(a => a.UDID == osAdapterInfoData.ADLAdapterInfo[i].UDID)
                                        .AdapterIndex;
                                }

                                var info = new BusIdInfo
                                {
                                    Name = devName,
                                    MF = mf,
                                    Uuid = uuid,
                                    InfSection = infSection,
                                    Adl1Index = index,
                                    Adl2Index = adl2Index
                                };

                                _busIdInfos.Add(busId, info);
                            }
                        }
                        else
                        {
                            Helpers.ConsolePrint(Tag,
                                "ADL_Adapter_AdapterInfo_Get() returned error code " +
                                adlRet);
                            isAdlInit = false;
                        }

                        // Release the memory for the AdapterInfo structure
                        if (IntPtr.Zero != adapterBuffer)
                            Marshal.FreeCoTaskMem(adapterBuffer);
                        if (buffer != IntPtr.Zero)
                            Marshal.FreeCoTaskMem(buffer);
                    }
                }

                if (null != ADL.ADL_Main_Control_Destroy && numberOfAdapters <= 0)
                    // Close ADL if it found no AMD devices
                    ADL.ADL_Main_Control_Destroy();
                if (ADL.ADL2_Main_Control_Destroy != null && adl2Control != IntPtr.Zero)
                {
                    ADL.ADL2_Main_Control_Destroy(adl2Control);
                }
            }
            else
            {
                // TODO
                Helpers.ConsolePrint(Tag,
                    "ADL_Main_Control_Create() returned error code " + adlRet);
                Helpers.ConsolePrint(Tag, "Check if ADL is properly installed!");
                isAdlInit = false;
            }

            return isAdlInit;
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
