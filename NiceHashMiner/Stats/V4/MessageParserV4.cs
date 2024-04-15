using HidSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Miners;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NiceHashMiner.Stats.V4
{
    static class MessageParserV4
    {
        private static readonly string _TAG = "MessageParserV4";
        private static Dictionary<string, List<DeviceDynamicProperties>> IgnoredValues = new Dictionary<string, List<DeviceDynamicProperties>>();
        /*
        internal static IMethod ParseMessage(string jsonData)
        {
            var method = MessageParser.ParseMessageData(jsonData);
            return method switch
            {
                // non rpc
                "sma" => JsonConvert.DeserializeObject<SmaMessage>(jsonData),
                "markets" => new ObsoleteMessage { Method = method },
                "balance" => JsonConvert.DeserializeObject<BalanceMessage>(jsonData),
                "versions" => JsonConvert.DeserializeObject<VersionsMessage>(jsonData),
                "burn" => JsonConvert.DeserializeObject<BurnMessage>(jsonData),
                "exchange_rates" => JsonConvert.DeserializeObject<ExchangeRatesMessage>(jsonData),
                // rpc
                "mining.set.username" => JsonConvert.DeserializeObject<MiningSetUsername>(jsonData),
                "mining.set.worker" => JsonConvert.DeserializeObject<MiningSetWorker>(jsonData),
                "mining.set.group" => JsonConvert.DeserializeObject<MiningSetGroup>(jsonData),
                "mining.enable" => JsonConvert.DeserializeObject<MiningEnable>(jsonData),
                "mining.disable" => JsonConvert.DeserializeObject<MiningDisable>(jsonData),
                "mining.start" => JsonConvert.DeserializeObject<MiningStart>(jsonData),
                "mining.stop" => JsonConvert.DeserializeObject<MiningStop>(jsonData),
                "mining.set.power_mode" => JsonConvert.DeserializeObject<MiningSetPowerMode>(jsonData),
                "miner.reset" => JsonConvert.DeserializeObject<MinerReset>(jsonData),
                "miner.call.action" => JsonConvert.DeserializeObject<MinerCallAction>(jsonData),
                "miner.set.mutable" => JsonConvert.DeserializeObject<MinerSetMutable>(jsonData),
                // non supported
                _ => throw new Exception($"Unable to deserialize '{jsonData}' got method '{method}'."),
            };
        }
        */
        internal static int NHMDeviceTypeToNHMWSDeviceType(DeviceType dt)
        {
            //rig manager enum
            //const deviceClasses = ['UNKNOWN','CPU','NVIDIA','AMD','ASIC',5,6,7,8,9,'ASIC']
            return dt switch
            {
                DeviceType.CPU => 1,
                DeviceType.NVIDIA => 2,
                DeviceType.AMD => 3,
                DeviceType.INTEL => 5, 
                _ => 0
            };
        }

        internal static IOrderedEnumerable<ComputeDevice> SortedDevices(this IEnumerable<ComputeDevice> devices)
        {
            return devices.OrderBy(d => d.DeviceType)
                .ThenBy(d => d.BusID);
        }

        private static string GetDevicePlugin(string UUID)
        {
            var devData = ComputeDeviceManager.Available.Devices.FirstOrDefault(dev => dev.DevUuid == UUID);
            if (devData == null) return "";

            return devData.MinerName.Trim() + " " + MinerVersion.GetMinerVersion(devData.MinerName).Trim();
        }

        private static (List<(string name, string? unit)> properties, JArray values) GetDeviceOptionalDynamic(ComputeDevice d, bool isLogin = false)
        {
            if (!IgnoredValues.ContainsKey(d.DevUuid))
            {
                IgnoredValues.Add(d.DevUuid, new List<DeviceDynamicProperties> { });
            }
            /*
            string getValue<T>(T o) => (typeof(T).Name, o) switch
            {
                (nameof(ILoad), ILoad g) => $"{(int)g.Load}",
                //(nameof(IMemControllerLoad), IMemControllerLoad g) => $"{g.MemoryControllerLoad}",
                (nameof(ITemp), ITemp g) => $"{g.Temp}",
                (nameof(IGetFanSpeedPercentage), IGetFanSpeedPercentage g) => $"{g.GetFanSpeedPercentage().percentage}",
                (nameof(IFanSpeedRPM), IFanSpeedRPM g) => $"{g.FanSpeedRPM}",
                (nameof(IPowerUsage), IPowerUsage g) => $"{g.PowerUsage}",
                (nameof(IVramTemp), IVramTemp g) => $"{g.VramTemp}",
                (nameof(IHotspotTemp), IHotspotTemp g) => $"{g.HotspotTemp}",
                (nameof(ICoreClock), ICoreClock g) => $"{g.CoreClock}",
                //(nameof(ICoreClockDelta), ICoreClockDelta g) => $"{g.CoreClockDelta}",
                (nameof(IMemoryClock), IMemoryClock g) => $"{g.MemoryClock}",
                //(nameof(IMemoryClockDelta), IMemoryClockDelta g) => $"{g.MemoryClockDelta}",
                (nameof(ITDP), ITDP g) => $"{g.TDPPercentage * 100}",
                (nameof(ITDPWatts), ITDPWatts g) => $"{g.TDPWatts}",
                (nameof(ICoreVoltage), ICoreVoltage g) => $"{g.CoreVoltage}",
                (_, _) => null,
            };
            */

            string getValueForName(string name) => name switch
            {
                "Miner" => $"{GetDevicePlugin(d.DevUuid)}",
                /*
                //"OC profile" => $"{d.OCProfile}",
                "OC profile" => $"",
                //"OC profile ID" => $"{d.OCProfileID}",
                "OC profile ID" => $"",
                //"Fan profile" => $"{d.FanProfile}",
                "Fan profile" => $"",
                //"Fan profile ID" => $"{d.FanProfileID}",
                "Fan profile ID" => $"",
                //"ELP profile" => $"{d.ELPProfile}",
                "ELP profile" => $"",
                //"ELP profile ID" => $"{d.ELPProfileID}",
                "ELP profile ID" => $"",
                */
                _ => null,
            };

            (DeviceDynamicProperties type, string name, string unit, string value)? pairOrNull<T>(DeviceDynamicProperties type, string name, string unit)
            {
                if (typeof(T) == typeof(string)) return (type, name, unit, getValueForName(name));

                float _ret = -1;
                if (typeof(T) == typeof(ITemp)) _ret = d.Temp;
                if (typeof(T) == typeof(IVramTemp))
                {
                    _ret = d.TempMemory;
                    if (_ret == 0) _ret = -1;
                }
                if (typeof(T) == typeof(ILoad))
                {
                    _ret = d.Load;
                }
                if (typeof(T) == typeof(IMemControllerLoad)) _ret = d.MemLoad;

                if (typeof(T) == typeof(IGetFanSpeedPercentage))
                {
                    _ret = d.FanSpeed;
                }
                if (typeof(T) == typeof(IFanSpeedRPM))
                {
                    _ret = d.FanSpeedRPM;
                }
                if (typeof(T) == typeof(IPowerUsage))
                {
                    _ret = (float)d.PowerUsage;
                    if (d.State == DeviceState.Disabled && !ConfigManager.GeneralConfig.ShowPowerOfDisabledDevices)
                    {
                        _ret = 0;
                    }
                }
                    
                if (_ret == -1)
                {
                    return (type, name, "", "-");
                } else if (_ret < 0)
                {
                    _ret = 0;
                }
                return (type, name, unit, _ret.ToString()); 
            }

            // here sort manually by type 
            var dynamicPropertiesWithValues = new List<(DeviceDynamicProperties type, string name, string unit, string value)?>
            {
                pairOrNull<ITemp>(DeviceDynamicProperties.Temperature ,"Temperature","°C"),
                pairOrNull<IVramTemp>(DeviceDynamicProperties.VramTemp,"Memory Temperature","°C"),
                pairOrNull<ILoad>(DeviceDynamicProperties.Load,"Load","%"),
                pairOrNull<IMemControllerLoad>(DeviceDynamicProperties.MemoryControllerLoad, "MemCtrl Load","%"),
                pairOrNull<IGetFanSpeedPercentage>(DeviceDynamicProperties.FanSpeedPercentage, "Fan speed","%"),
                pairOrNull<IFanSpeedRPM>(DeviceDynamicProperties.FanSpeedRPM, "Fan speed","RPM"),
                pairOrNull<IPowerUsage>(DeviceDynamicProperties.PowerUsage, "Power usage","W"),
                //pairOrNull<ICoreClock>(DeviceDynamicProperties.CoreClock, "Core clock", "MHz"),
                //pairOrNull<ICoreClockDelta>(DeviceDynamicProperties.CoreClockDelta, "Core clock delta", "MHz"),
                //pairOrNull<IMemoryClock>(DeviceDynamicProperties.MemClock, "Memory clock", "MHz"),
                //pairOrNull<IMemoryClockDelta>(DeviceDynamicProperties.MemClockDelta, "Memory clock", "MHz"),
                //pairOrNull<ICoreVoltage>(DeviceDynamicProperties.CoreVoltage, "Core voltage", "mV"),
                //pairOrNull<ITDP>(DeviceDynamicProperties.TDP, "Power Limit", "%"),
                //pairOrNull<ITDPWatts>(DeviceDynamicProperties.TDPWatts, "Power Limit", "W"),
                pairOrNull<string>(DeviceDynamicProperties.NONE, "Miner", null),
                /*
                pairOrNull<string>(DeviceDynamicProperties.NONE, "OC profile", null),//?
                pairOrNull<string>(DeviceDynamicProperties.NONE, "OC profile ID", null),
                pairOrNull<string>(DeviceDynamicProperties.NONE, "Fan profile", null),//?
                pairOrNull<string>(DeviceDynamicProperties.NONE, "Fan profile ID", null),
                pairOrNull<string>(DeviceDynamicProperties.NONE, "ELP profile", null),
                pairOrNull<string>(DeviceDynamicProperties.NONE, "ELP profile ID", null),
                */
            };
            var deviceOptionalDynamic = dynamicPropertiesWithValues
                .Where(p => p.HasValue)
                .Where(p => p.Value.value != null)
                .Select(p => p.Value)
                .ToList();

            if (isLogin)
            {
                foreach (var od in deviceOptionalDynamic)
                {
                    if ((od.type == DeviceDynamicProperties.Temperature ||
                        //od.type == DeviceDynamicProperties.HotspotTemp ||
                        od.type == DeviceDynamicProperties.VramTemp ||
                        //od.type == DeviceDynamicProperties.MemClock ||
                        od.type == DeviceDynamicProperties.PowerUsage
                        //od.type == DeviceDynamicProperties.TDP ||
                        //od.type == DeviceDynamicProperties.TDPWatts ||
                        //od.type == DeviceDynamicProperties.CoreVoltage
                        ) &&
                        Int32.TryParse(od.value, out var lessOrEqual) && lessOrEqual <= 0)
                    {
                        if (IgnoredValues.TryGetValue(d.DevUuid, out var list) &&
                            !list.Contains(od.type))
                        {
                            //list.Add(od.type);
                        }
                    }
                    if ((od.type == DeviceDynamicProperties.Load ||
                        od.type == DeviceDynamicProperties.FanSpeedRPM ||
                        od.type == DeviceDynamicProperties.FanSpeedPercentage 
                        //od.type == DeviceDynamicProperties.CoreClock
                        ) &&
                        Int32.TryParse(od.value, out var less) && less < 0)
                    {
                        if (IgnoredValues.TryGetValue(d.DevUuid, out var list) &&
                            !list.Contains(od.type))
                        {
                            //list.Add(od.type);
                        }
                    }
                }
            }

            bool shouldRemoveDynamicVal(string b64uuid, (DeviceDynamicProperties type, string name, string unit, string value) dynamicVal)
            {
                //if (dynamicVal.unit == String.Empty) return false;
                if (dynamicVal.type == DeviceDynamicProperties.NONE) return false;
                if(IgnoredValues.TryGetValue(d.DevUuid, out var list) && list.Contains(dynamicVal.type))
                {
                    return true;
                }
                return false;
            };
            deviceOptionalDynamic.RemoveAll(dynamVal => shouldRemoveDynamicVal(d.DevUuid, dynamVal));
            //deviceOptionalDynamic.Clear();

            //deviceOptionalDynamic.ForEach(dynamVal => d.SupportedDynamicProperties.Add(dynamVal.type));
            //if (isLogin) deviceOptionalDynamic.ForEach(dynamVal => d.SupportedDynamicProperties.Add(dynamVal.type));
            //foreach (DeviceDynamicProperties i in Enum.GetValues(typeof(DeviceDynamicProperties)))
            //{
            //    if (!d.SupportedDynamicProperties.Contains(i)) deviceOptionalDynamic.RemoveAll(prop => prop.type == i);
            //}
            List<(string name, string? unit)> optionalDynamicProperties = deviceOptionalDynamic.Select(p => (p.name, p.unit)).ToList();
            var values_odv = new JArray(deviceOptionalDynamic.Select(p => p.value));
            return (optionalDynamicProperties, values_odv);
        }

        // we cache device properties so we persevere  property IDs
        private static readonly Dictionary<ComputeDevice, List<OptionalMutableProperty>> _cachedDevicesOptionalMutable = new Dictionary<ComputeDevice, List<OptionalMutableProperty>>();
        
        //settings per device
        private static (List<OptionalMutableProperty> properties, JArray values) GetDeviceOptionalMutable(ComputeDevice d, bool isLogin)
        {
            //OptionalMutableProperty valueOrNull<T>(OptionalMutableProperty v) => d.DeviceMonitor is T ? v : null;
            OptionalMutableProperty valueOrNull<T>(OptionalMutableProperty v) => v;

            List<OptionalMutableProperty> getOptionalMutableProperties(ComputeDevice d)
            {
                var optionalProperties = new List<OptionalMutableProperty>();
                // TODO sort by type
                
                optionalProperties.Add(new OptionalMutablePropertyString
                {
                    PropertyID = OptionalMutableProperty.NextPropertyId(),
                    DisplayGroup = 0,
                    DisplayName = "Miners settings",//102
                    DefaultValue = "",
                    Range = (262144, ""),
                    ExecuteTask = async (object p) =>
                    {
                        if (p is not string prop) return -1;
                        var newState = JsonConvert.DeserializeObject<MinerAlgoState>(prop.ToString());
                        //Task.Run(async () => NHWebSocketV4.UpdateMinerStatus());
                        return 0;
                    },
                    GetValue = () =>
                    {
                        string ret = null;
                        ret = string.Empty;
                        ret += GetMinersForDeviceDynamic(d);
                        return ret;
                    },
                    ComputeDev = d
                });
                

                
                optionalProperties.Add(new OptionalMutablePropertyString
                {
                    PropertyID = OptionalMutableProperty.NextPropertyId(),
                    DisplayGroup = 0,
                    DisplayName = "Benchmark settings",
                    DefaultValue = "",
                    Range = (262144, ""),
                    
                    ExecuteTask = async (object p) =>
                    {
                        if (p is not string prop) return -1;
                        var newSpeed = JsonConvert.DeserializeObject<MinerAlgoSpeed>(prop);
                        return d.ApplyNewAlgoSpeeds(newSpeed);
                    },
                    
                    GetValue = () =>
                    {
                        var ret = string.Empty;
                        ret += GetMinersSpeedsForDeviceDynamic(d);
                        return ret;
                    },
                    ComputeDev = d
                });
            
                if (isLogin)
                {
                    if (ActionMutableMap.MutableList.Count < ComputeDeviceManager.Available.Devices.Count + 1)
                    {
                        optionalProperties.ForEach(i => ActionMutableMap.MutableList.Add(i));
                    }
                }
                
                return optionalProperties
                    .Where(p => p != null)
                    .ToList();
            }

            List<OptionalMutableProperty> getOptionalMutablePropertiesCached(ComputeDevice d)
            {
                if (_cachedDevicesOptionalMutable.TryGetValue(d, out var cachedProps)) return cachedProps;
                return getOptionalMutableProperties(d);
            }

            var props = getOptionalMutablePropertiesCached(d);
            var selectedValues = props
                .Where(p => p.GetValue() != null)?
                .Select(p => p.GetValue());
            JArray values_omv = null;
            if (selectedValues.Any())
            {
                values_omv = new JArray(selectedValues);
            }
            return (props, values_omv);
        }



        public static List<List<string>> DeviceOptionalDynamicToList(List<(string name, string? unit)> properties)
        {
            List<List<string>> result = new List<List<string>>();
            foreach (var property in properties)
            {
                if (property.unit == null)
                {
                    result.Add(new List<string> { property.name });
                    continue;
                }
                result.Add(new List<string> { property.name, property.unit });
            }
            return result;
        }
        public static LoginMessage CreateLoginMessage(string btc, string worker, string rigID, IOrderedEnumerable<ComputeDevice> devices)
        {
            var sorted = SortedDevices(devices);
            //if (_loginMessage != null) return _loginMessage;
            Device mapComputeDevice(ComputeDevice d)
            {
                return new Device
                {
                    StaticProperties = new Dictionary<string, object>
                    {
                        { "device_id", d.DevUuid },
                        { "class", $"{NHMDeviceTypeToNHMWSDeviceType(d.DeviceType)}" },
                        { "name", d.NameCustom },
                        { "optional", GetStaticPropertiesOptionalValues(d) },
                    },
                    Actions = CreateDefaultDeviceActions(d.DevUuid),
                    OptionalDynamicProperties = DeviceOptionalDynamicToList(GetDeviceOptionalDynamic(d, true).properties),
                    OptionalMutableProperties = GetDeviceOptionalMutable(d, true).properties,
                };
            }

            var DevicesProperties = devices.Select(mapComputeDevice).ToList(); //needs to execute first
            string winver = "Windows version " + Form_Main.GetWinVer(Environment.OSVersion.Version) +
                " (" + Environment.OSVersion.Version.Major.ToString() + "." + Environment.OSVersion.Version.Minor.ToString() +
                " build: " + Environment.OSVersion.Version.Build.ToString() + ")";

            return new LoginMessage
            {
                Btc = btc,
                Worker = worker,
                RigID = rigID,
                //Version = new List<string> { $"NHM/{NHMApplication.ProductVersion}", Environment.OSVersion.ToString() },
                Version = new List<string> { NiceHashSocket.version, winver },
                //Version = new List<string> { "NHM/3.1.0.9", "Microsoft Windows NT 10.0.22621.0" },
                OptionalMutableProperties = GetRigOptionalMutableValues(true).properties,
                OptionalDynamicProperties = GetRigOptionalDynamicValues().properties,
                Actions = CreateDefaultRigActions(),
                Devices = DevicesProperties,
                MinerState = GetMinerStateValues(worker, devices),
            };
        }
        public static (List<OptionalMutableProperty> properties, JArray values) GetRigOptionalMutableValues(bool isLogin)
        {
            List<OptionalMutableProperty> getOptionalMutableProperties()
            {
                var optionalProperties = new List<OptionalMutableProperty>()
                {
                    /*
                    new OptionalMutablePropertyString
                    {
                        PropertyID = OptionalMutableProperty.NextPropertyId(),
                        DisplayGroup = 0,
                        DisplayName = "User name",
                        DefaultValue = Configs.ConfigManager.GeneralConfig.BitcoinAddressNew,
                        Range = (64, String.Empty),
                        GetValue = () =>
                        {
                            return Configs.ConfigManager.GeneralConfig.BitcoinAddressNew;
                        }
                        //ExecuteTask = (object p) =>
                        //{
                        //    var userSetResult = await ApplicationStateManager.SetBTCIfValidOrDifferent(btc, true);
                        //    return userSetResult switch
                        //    {
                        //        NhmwsSetResult.CHANGED => true, // we return executed
                        //        NhmwsSetResult.INVALID => throw new RpcException("Mining address invalid", ErrorCode.InvalidUsername),
                        //        NhmwsSetResult.NOTHING_TO_CHANGE => throw new RpcException($"Nothing to change btc \"{btc}\" already set", ErrorCode.RedundantRpc),
                        //        _ => throw new RpcException($"", ErrorCode.InternalNhmError),
                        //    };
                        //}
                    },
                    */
                    /*
                    new OptionalMutablePropertyString
                    {
                        PropertyID = OptionalMutableProperty.NextPropertyId(),
                        DisplayGroup = 0,
                        DisplayName = "Worker name",
                        DefaultValue = Configs.ConfigManager.GeneralConfig.WorkerName,
                        Range = (64, String.Empty),
                        GetValue = () =>
                        {
                            return Configs.ConfigManager.GeneralConfig.WorkerName;
                        }
                        //ExecuteTask = (object p) =>
                        //{
                        //    var workerSetResult = ApplicationStateManager.SetWorkerIfValidOrDifferent(worker, true);
                        //    return workerSetResult switch
                        //    {
                        //        NhmwsSetResult.CHANGED => Task.FromResult(true), // we return executed
                        //        NhmwsSetResult.INVALID => throw new RpcException("Worker name invalid", ErrorCode.InvalidWorker),
                        //        NhmwsSetResult.NOTHING_TO_CHANGE => throw new RpcException($"Nothing to change worker name \"{worker}\" already set", ErrorCode.RedundantRpc),
                        //        _ => throw new RpcException($"", ErrorCode.InternalNhmError),
                        //    };
                        //}
                    },
                    */
                    
                    new OptionalMutablePropertyString
                    {
                        PropertyID = OptionalMutableProperty.NextPropertyId(),
                        DisplayGroup = 0,
                        DisplayName = "Miners settings",//104 per rig
                        DefaultValue = "",
                        Range = (262144, String.Empty),
                        GetValue = () =>
                        {
                            string ret = string.Empty;
                            if (ComputeDeviceManager.Available.Devices.Count < 12)
                            {
                            var minersSettingsGlobal = new MinerAlgoStateRig();
                            var mutables = ActionMutableMap.MutableList.Where(m => m.ComputeDev != null && m.DisplayName == "Miners settings");
                            if(mutables == null || mutables.Count() <= 0) return ret;
                            foreach (var mutable in mutables)
                            {
                                //Helpers.ConsolePrint("**********", mutables.ToList().Count.ToString() + " " + isLogin.ToString());
                                if (mutable.GetValue() is not string val) continue;
                                //Helpers.ConsolePrint("**********", isLogin.ToString());
                                //if (minersSettingsGlobal.Miners.Exists(JsonConvert.DeserializeObject<MinerAlgoState>(val))) continue;
                                minersSettingsGlobal.Miners.Add(JsonConvert.DeserializeObject<MinerAlgoState>(val));

                            }
                            ret += JsonConvert.SerializeObject(minersSettingsGlobal);
                            }
                            return ret;
                        },
                        ExecuteTask = async (object p) =>
                        {
                            if(p is not string prop) return -1;
                            var newStates = JsonConvert.DeserializeObject<MinerAlgoStateRig>(prop);
                            //for each device thats inside apply new algo state
                            var devices = ComputeDeviceManager.Available.Devices.Where(d => newStates.Miners.Any(m => m.DeviceID.Contains(d.DevUuid)));
                            if(devices == null) return -2;
                            var successCount = 0;
                            foreach(var ns in newStates.Miners)
                            {
                                var targetDev = ComputeDeviceManager.Available.Devices.FirstOrDefault(d => d.DevUuid == ns.DeviceID);
                                if(targetDev == null) continue;
                                var tempRes = targetDev.ApplyNewAlgoStates(ns);
                                if(tempRes != 0) continue;
                                successCount++;
                            }
                            return successCount == newStates.Miners.Count ? 0 : -3;
                        }
                    },
                    
                    /*
                    new OptionalMutablePropertyString
                    {
                        PropertyID = OptionalMutableProperty.NextPropertyId(),
                        DisplayGroup = 0,
                        DisplayName = "Scheduler settings",
                        DefaultValue = "",
                        Range = (4096, string.Empty),
                        GetValue = () =>
                        {
                            //string ret = SchedulesManager.Instance.ScheduleToJSON();
                            var schedules = new SchedulesWS4();
                            var ret = JsonConvert.SerializeObject(schedules);
                            return ret;
                        }
                    */
                        /*
                        ExecuteTask = async (object p) =>
                        {
                            if(p is not string prop) return -1;
                            var (schedulerEnabled, returnedSchedules) = SchedulesManager.Instance.ScheduleFromJSON(prop);
                            SchedulesManager.Instance.ClearScheduleList();
                            MiningSettings.Instance.UseScheduler = schedulerEnabled;
                            if(returnedSchedules != null)
                            {
                                foreach(var returnedSchedule in returnedSchedules)
                                {
                                    returnedSchedule.From = DateTime.Parse(returnedSchedule.From).ToLocalTime().ToString("HH:mm");
                                    returnedSchedule.To = DateTime.Parse(returnedSchedule.To).ToLocalTime().ToString("HH:mm");
                                    SchedulesManager.Instance.AddScheduleToList(returnedSchedule);
                                }
                            }
                            _ = Task.Run(async () => await NHWebSocketV4.UpdateMinerStatus());
                            return 0;
                        }
                        */
                    //},
                    
                    new OptionalMutablePropertyBool
                    {
                        PropertyID = OptionalMutableProperty.NextPropertyId(),
                        DisplayGroup = 0,
                        DisplayName = "Auto update",
                        DefaultValue = false,
                        GetValue = () =>
                        {
                            //return UpdateSettings.Instance.AutoUpdateMinerPlugins && UpdateSettings.Instance.AutoUpdateNiceHashMiner;
                            //Helpers.ConsolePrint("*************", "NextPropertyId");
                            return Configs.ConfigManager.GeneralConfig.ProgramAutoUpdate;
                        },
                    /*
                        ExecuteTask = async (object p) =>
                        {
                            if(p is not bool prop) return -1;
                            UpdateSettings.Instance.AutoUpdateMinerPlugins = prop;
                            UpdateSettings.Instance.AutoUpdateNiceHashMiner = prop;
                            _ = Task.Run(async () => await NHWebSocketV4.UpdateMinerStatus());
                            return 0;
                        }
                        */

                    },
                    
                    new OptionalMutablePropertyString
                    {
                        PropertyID = OptionalMutableProperty.NextPropertyId(),
                        DisplayGroup = 0,
                        DisplayName = "Benchmark settings",//106
                        DefaultValue = "",
                        Range = (262144, ""),
                        GetValue = () =>
                        {
                            var ret = string.Empty;
                            if (ComputeDeviceManager.Available.Devices.Count < 12)
                            {
                            var minerSpeedsGlobal = new MinerAlgoSpeedRig();
                            var mutables = ActionMutableMap.MutableList.Where(m => m.ComputeDev != null && m.DisplayName == "Benchmark settings");
                            if(mutables == null || mutables.Count() <= 0) return ret;
                            foreach (var mutable in mutables)
                            {
                                if (mutable.GetValue() is not string val) continue;
                                minerSpeedsGlobal.Miners.Add(JsonConvert.DeserializeObject<MinerAlgoSpeed>(val));
                            }
                            ret += JsonConvert.SerializeObject(minerSpeedsGlobal);
                            }
                            return ret;
                        },
                        ExecuteTask = async (object p) =>
                        {
                            if(p is not string prop) return -1;
                            var newSpeeds = JsonConvert.DeserializeObject<MinerAlgoSpeedRig>(prop);
                            //for each device thats inside apply new algo state
                            var devices = ComputeDeviceManager.Available.Devices.Where(d => newSpeeds.Miners.Any(m => m.DeviceID.Contains(d.DevUuid)));
                            if(devices == null) return -2;
                            var successCount = 0;
                            foreach(var ns in newSpeeds.Miners)
                            {
                                var targetDev = ComputeDeviceManager.Available.Devices.FirstOrDefault(d => d.DevUuid == ns.DeviceID);
                                if(targetDev == null) continue;
                                var tempRes = targetDev.ApplyNewAlgoSpeeds(ns);
                                if(tempRes != 0) continue;
                                successCount++;
                            }
                            return successCount == newSpeeds.Miners.Count ? 0 : -3;
                        }
                    }
                    
                };
                if (isLogin)
                {
                    if (ActionMutableMap.MutableList.Count < ComputeDeviceManager.Available.Devices.Count + 1)
                    {
                        optionalProperties.ForEach(i => ActionMutableMap.MutableList.Add(i));
                    }
                }
                return optionalProperties
                    .Where(p => p != null)
                    .ToList();

            };
            
            List<OptionalMutableProperty> getOptionalMutablePropertiesCached()
            {
                //if (_cachedDevicesOptionalMutable.TryGetValue(out var cachedProps)) return cachedProps;
                return getOptionalMutableProperties();
            }
            
            var props = getOptionalMutablePropertiesCached();
            var selectedValues = props
                .Where(p => p.GetValue() != null)?
                .Select(p => p.GetValue());
            JArray values_omv = null;
            if (selectedValues.Any())
            {
                values_omv = new JArray(selectedValues);
            }
            return (props, values_omv);
        }
        private static (List<List<string>> properties, JArray values) GetRigOptionalDynamicValues()
        {
            var dynamic = new List<(List<string> prop, string val)>
            {
                (new List<string>
                {
                    "Uptime",
                    "s"
                }, Math.Round(Form_Main.Uptime.TotalSeconds, 0).ToString()),
                /*
                (new List<string>
                {
                    "IP address"
                //}, "127.0.0.1"),
                //}, Helpers.GetLocalIP().ToString())
                }, Helpers.GetLocalIP().ToString())
                */
                /*
                (new List<string>
                {
                    "Profiles bundle id"
                }, ""),
                (new List<string>
                {
                    "Profiles bundle name"
                }, "")
                */
            };
            var props = dynamic.Select(d => d.prop).ToList();
            var vals = dynamic.Select(d => d.val);
            return (props, new JArray(vals));
        }

        private static JObject GetMinerStateValues(string workerName, IOrderedEnumerable<ComputeDevice> devices)
        {
            var json = JObject.FromObject(GetMinerState(workerName, devices));
            var delProp = json.Property("method");
            delProp.Remove();
            return json;
        }

        internal static MinerState GetMinerState(string workerName, IOrderedEnumerable<ComputeDevice> devices)
        {
            var rig = NiceHashStats.CalcRigStatus();
            int rigStateToInt(RigStatus s) => s switch
            {
                RigStatus.Stopped => 1, // READY/IDLE/STOPPED
                RigStatus.Mining => 2, // MINING/WORKING
                RigStatus.Benchmarking => 3, // BENCHMARKING
                RigStatus.Error => 5, // ERROR
                RigStatus.Pending => 0, // NOT DEFINED
                RigStatus.Disabled => 4, // DISABLED
                _ => 0, // UNKNOWN
            };


            MinerState.DeviceState toDeviceState(ComputeDevice d)
            {                
                int deviceStateToInt(DeviceState s) => s switch
                {
                    DeviceState.Stopped => 1, // READY/IDLE/STOPPED
                    DeviceState.Mining => 2, // MINING/WORKING
                    DeviceState.Benchmarking => 3, // BENCHMARKING
                    DeviceState.Error => 5, // ERROR
                    DeviceState.Pending => 0, // NOT DEFINED
                    DeviceState.Disabled => 4, // DISABLED
#if NHMWS4
                    //DeviceState.Gaming => 6, //GAMING
                    DeviceState.Testing => (MiscSettings.Instance.EnableGPUManagement ? 7 : 2), //TESTING
#endif
                    _ => 0, // UNKNOWN
                };

                JArray mdv(ComputeDevice d)
                {
                    var state = deviceStateToInt(d.State);
                    var speeds = NiceHashStats.GetSpeedForDevice(d.DevUuid);
                    for (int i = 0; i < speeds.Count; i++)
                    {
                        if (speeds[i].type == AlgorithmType.KAWPOWLite)
                        {
                            var ms = speeds[i];
                            ms.type = AlgorithmType.KAWPOW;
                            speeds[i] = ms;
                        }
                    }

                    return new JArray(state, new JArray(speeds.Select(kvp => new JArray((int)kvp.type, kvp.speed))));
                }
                JArray mmv(ComputeDevice d)
                {
                    return new JArray(deviceStateToInt(d.State));
                }
                //Logger.Warn(_TAG, $"\t[{d.BaseDevice.Name}](deviceState):{d.State} -- converted (int):{deviceStateToInt(d.State)}");

                return new MinerState.DeviceState
                {
                    MandatoryDynamicValues = mdv(d),
                    OptionalDynamicValues = GetDeviceOptionalDynamic(d).values, // odv
                    MandatoryMutableValues = mmv(d),
                    OptionalMutableValues = GetDeviceOptionalMutable(d, false).values, // omv
                };
            }
            //Logger.Warn(_TAG, $"Miner state (rigstatus):{rig} -- converted (int):{rigStateToInt(rig)}");
            return new MinerState
            {
                MutableDynamicValues = new JArray(rigStateToInt(rig)),
                OptionalDynamicValues = GetRigOptionalDynamicValues().values,
                MandatoryMutableValues = new JArray(rigStateToInt(rig), workerName),
                OptionalMutableValues = GetRigOptionalMutableValues(false).values,
                Devices = devices.Select(toDeviceState).ToList(),
            };
        }


        private static List<NhmwsAction> CreateDefaultDeviceActions(string uuid)
        {
            return new List<NhmwsAction>
            {
                NhmwsAction.ActionDeviceEnable(uuid),
                NhmwsAction.ActionDeviceDisable(uuid),
                /*
                NhmwsAction.ActionDeviceRebenchmark(uuid),
                NhmwsAction.ActionOcProfileTest(uuid),
                NhmwsAction.ActionOcProfileTestStop(uuid),
                NhmwsAction.ActionFanProfileTest(uuid),
                NhmwsAction.ActionFanProfileTestStop(uuid),
                NhmwsAction.ActionElpProfileTest(uuid),
                NhmwsAction.ActionElpProfileTestStop(uuid),
                */
            };
        }
        private static List<NhmwsAction> CreateDefaultRigActions()
        {
            return new List<NhmwsAction>
            {
                NhmwsAction.ActionStartMining(),
                NhmwsAction.ActionStopMining(),
                /*
                NhmwsAction.ActionRebenchmark(),
                NhmwsAction.ActionProfilesBundleSet(),
                NhmwsAction.ActionProfilesBundleReset(),
                NhmwsAction.ActionRigShutdown(),
                */
                NhmwsAction.ActionRigRestart(),
                //NhmwsAction.ActionSystemDump(),
            };
        }
        private static List<JArray> GetStaticPropertiesOptionalValues(ComputeDevice d)
        {
            /*
            return d.BaseDevice switch
            {
                IGpuDevice gpu => new List<JArray>
                    {
                        new JArray("bus_id", $"{gpu.PCIeBusID}"),
                        new JArray("vram", $"{gpu.GpuRam}"),
                        new JArray("miners", GetMinersForDeviceStatic(d)),
                        new JArray("limits", GetLimitsForDevice(d)),
                    },
                _ => new List<JArray>
                    {
                        new JArray("miners", GetMinersForDeviceStatic(d)),
                        new JArray("limits", GetLimitsForDevice(d)),
                    },
            };
            */
            var gpu = new List<JArray>();
            if (d.DeviceType == DeviceType.CPU)
            {
                gpu = new List<JArray>
                    {
                        //new JArray("miners", GetMinersForDeviceStatic(d)),
                        //new JArray("limits", GetLimitsForDevice(d)),
                    };
            } else
            {
                gpu = new List<JArray>
                    {
                        new JArray("bus_id", $"{d.BusID}"),
                        new JArray("vram", $"{d.GpuRam}"),
                        //new JArray("miners", GetMinersForDeviceStatic(d)),
                        //new JArray("limits", GetLimitsForDevice(d)),
                    };
            }
            /*
                _ => new List<JArray>
                    {
                        new JArray("miners", GetMinersForDeviceStatic(d)),
                        new JArray("limits", GetLimitsForDevice(d)),
                    },
            };
            */
            /*
        JArray ret = new JArray();

            ret.Add(new List<JToken>
            {
                "bus_id",
                d.BusID
            });
            ret.Add(new List<JToken>
            {
                "vram",
                d.GpuRam
            });
            ret.Add(new List<JToken>
            {
                "miners",
                GetMinersForDeviceStatic(d)
            });
            ret.Add(new List<JToken>
            {
                "limits",
                GetLimitsForDevice(d)
            });
            return ret;
            */
            return gpu;
        }
        private static string GetMinersForDeviceDynamic(ComputeDevice d)
        {
            var minersObject = new MinerAlgoState();
            var containers = d.GetAlgorithmSettings();   
            if (containers == null) return String.Empty;
            var grouped = containers.GroupBy(c => c.MinerBaseTypeName + MinerVersion.GetMinerVersion(c.MinerBaseTypeName)).ToList();
            if (grouped == null) return String.Empty;
            foreach (var group in grouped)
            {
                var containerEnabled = group.Any(c => c.Enabled);
                var miner = new MinerDynamic() { Id = group.Key, Enabled = containerEnabled };
                var algos = new List<Algo>();
                foreach (var algo in group)
                {
                    if (!algo.Hidden)
                    {
                        var tempAlgo = new Algo() { Id = algo.AlgorithmName, Enabled = algo.Enabled };
                        algos.Add(tempAlgo);
                    }
                }
                miner.Algos = algos;
                minersObject.Miners.Add(miner);
            }
            minersObject.DeviceID = d.DevUuid;
            minersObject.DeviceName = d.NameCustom;
            var json = JsonConvert.SerializeObject(minersObject);
            return json;
            
        }

        private static string GetMinersSpeedsForDeviceDynamic(ComputeDevice d)
        {
            //return String.Empty;

            var minersObject = new MinerAlgoSpeed();
            var containers = d.GetAlgorithmSettings();
            if (containers == null) return string.Empty;
            var grouped = containers.GroupBy(c => c.MinerBaseTypeName + MinerVersion.GetMinerVersion(c.MinerBaseTypeName)).ToList();
            if (grouped == null) return string.Empty;
            foreach (var group in grouped)
            {
                var combinations = new List<Combination>();
                foreach (var algo in group)
                {
                    var a = algo.SecondaryNiceHashID;
                    if (a == AlgorithmType.NONE)
                    {
                        var algorithms = new List<AlgoSpeed>()
                    {
                        new AlgoSpeed()
                        {
                            //Id = Convert.ToString((int)algo.IDs[0]),
                            Id = Convert.ToString((int)algo.NiceHashID),
                            Speed = algo.BenchmarkSpeed.ToString()
                        }

                    };
                        var combination = new Combination()
                        {
                            Id = algo.AlgorithmName,
                            Algos = algorithms
                        };
                        combinations.Add(combination);
                    } else//dual
                    {
                        var algorithms = new List<AlgoSpeed>()
                    {
                        new AlgoSpeed()
                        {
                            //Id = Convert.ToString((int)algo.IDs[0]),
                            Id = Convert.ToString((int)algo.NiceHashID),
                            Speed = algo.BenchmarkSpeed.ToString()
                        },
                        new AlgoSpeed()
                        {
                            //Id = Convert.ToString((int)algo.IDs[0]),
                            Id = Convert.ToString((int)algo.SecondaryNiceHashID),
                            Speed = algo.BenchmarkSecondarySpeed.ToString()
                        }

                    };
                        var combination = new Combination()
                        {
                            Id = algo.AlgorithmName,
                            Algos = algorithms
                        };
                        combinations.Add(combination);
                    }
                    /*
                    var algorithms = new List<AlgoSpeed>()
                    {
                        new AlgoSpeed()
                        {
                            //Id = Convert.ToString((int)algo.IDs[0]),
                            Id = Convert.ToString((int)algo.NiceHashID),
                            Speed = algo.BenchmarkSpeed.ToString()
                        }, a == AlgorithmType.NONE ? null :
                        new AlgoSpeed()
                        {
                            //Id = Convert.ToString((int)algo.IDs[0]),
                            Id = Convert.ToString((int)algo.SecondaryNiceHashID),
                            Speed = algo.BenchmarkSecondarySpeed.ToString()
                        }

                    };
                    var combination = new Combination()
                    {
                        Id = algo.AlgorithmName,
                        Algos = algorithms
                    };
                    combinations.Add(combination);
                    */
                }
                var miner = new MinerSpeedDynamic() { Id = group.Key, Combinations = combinations };
                minersObject.Miners.Add(miner);
            }
            minersObject.DeviceID = d.DevUuid;
            minersObject.DeviceName = d.NameCustom;
            var json = JsonConvert.SerializeObject(minersObject);
            return json;

        }

        private static string GetMinersForDeviceStatic(ComputeDevice d)
        {
            //return String.Empty;

            MinersStatic miners = new MinersStatic();
            var uniquePlugins = d.GetAlgorithmSettings()?.Select(item => item.MinerBaseTypeName + MinerVersion.GetMinerVersion(item.MinerBaseTypeName))?.Distinct()?.Where(item => !string.IsNullOrEmpty(item));
            if (uniquePlugins == null) return String.Empty;
            foreach (var plugin in uniquePlugins)
            {
                var uniqueAlgos = d.GetAlgorithmSettings()?.Where(item => item.MinerBaseTypeName + MinerVersion.GetMinerVersion(item.MinerBaseTypeName) == plugin)?.Select(item => item.AlgorithmName)?.Distinct();
                if (uniqueAlgos == null) uniqueAlgos = new List<string>();
                miners.Miners.Add(new MinerStatic() { Id = plugin, AlgoList = uniqueAlgos.ToList() });
            }
            var json = JsonConvert.SerializeObject(miners);
            return json;

        }
        private static string GetLimitsForDevice(ComputeDevice d)
        {
            //return String.Empty;

            ComplexLimit limit = new ComplexLimit();
            /*
            if (d.DeviceMonitor is ITDP && d.DeviceMonitor is ITDPLimits tdpLim)
            {
                var lims = tdpLim.GetTDPLimits();
                if (lims.ok)
                {
                    if(d.DeviceType == DeviceType.AMD)
                    {
                        limit.limits.Add(new Limit { Name = "Power Limit", Unit = "%", Def = lims.def, Range = ((int)lims.min, (int)lims.max) });
                    }
                    else
                    {
                        limit.limits.Add(new Limit { Name = "Power Limit", Unit = "W", Def = lims.def, Range = ((int)lims.min, (int)lims.max) });
                    }
                }
            }
            if (d.DeviceMonitor is ICoreClockSet)
            {
                if (d.DeviceType == DeviceType.NVIDIA && d.DeviceMonitor is ICoreClockRangeDelta ccLimDelta)
                {
                    var lims = ccLimDelta.CoreClockRangeDelta;
                    if (lims.ok)
                    {
                        limit.limits.Add(new Limit { Name = "Core clock delta", Unit = "MHz", Def = lims.def, Range = (lims.min, lims.max) });
                    }
                }
                if (d.DeviceMonitor is ICoreClockRange ccLim && !d.IsNvidiaAndSub2KSeries())
                {
                    var lims = ccLim.CoreClockRange;
                    if (lims.ok)
                    {
                        if(lims.max - lims.min <= 20) limit.limits.Add(new Limit { Name = "Core clock", Unit = "MHz", Def = lims.def, Range = (300, 3000) });//INTERFACE ERROR, limits could not be retrieved
                        else limit.limits.Add(new Limit { Name = "Core clock", Unit = "MHz", Def = lims.def, Range = (lims.min, lims.max) });
                    }
                }
            }
            if (d.DeviceMonitor is IMemoryClockSet)
            {
                if (d.DeviceType == DeviceType.NVIDIA && d.DeviceMonitor is IMemoryClockRangeDelta mcLimDelta)
                {
                    var lims = mcLimDelta.MemoryClockRangeDelta;
                    if (lims.ok)
                    {
                        limit.limits.Add(new Limit { Name = "Memory clock delta", Unit = "MHz", Def = lims.def, Range = (lims.min, lims.max) });
                    }
                }
                if (d.DeviceMonitor is IMemoryClockRange mcLim && !d.IsNvidiaAndSub2KSeries())
                {
                    var lims = mcLim.MemoryClockRange;
                    if (lims.ok)
                    {
                        if(lims.min - lims.min <= 20) limit.limits.Add(new Limit { Name = "Memory clock", Unit = "MHz", Def = lims.def, Range = (300, 10000) });//INTERFACE ERROR, limits could not be retrieved
                        else limit.limits.Add(new Limit { Name = "Memory clock", Unit = "MHz", Def = lims.def, Range = (lims.min, lims.max) });
                    }
                }
            }
            if(d.DeviceMonitor is ICoreVoltageSet && d.DeviceMonitor is ICoreVoltageRange cvRange)
            {
                var lims = cvRange.CoreVoltageRange;
                if (lims.ok && d.DeviceMonitor is ICoreVoltage cvGet)
                {
                    var def = d.DeviceType == DeviceType.INTEL ? lims.def : cvGet.CoreVoltage;
                    limit.limits.Add(new Limit { Name = "Core Voltage", Unit = "mV", Def = def, Range = (lims.min, lims.max) });
                }
            }
            */
            var json = JsonConvert.SerializeObject(limit);
            return json;

        }
    }
}
