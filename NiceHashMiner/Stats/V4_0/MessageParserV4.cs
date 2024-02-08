using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NiceHashMiner.Devices;
using NiceHashMiner.Stats;
using NiceHashMiner.Stats.V4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NiceHashMiner.Stats.V4
{
    static class MessageParserV4
    {
        internal static IOrderedEnumerable<ComputeDevice> SortedDevices(this IEnumerable<ComputeDevice> devices)
        {
            return devices.OrderBy(d => d.DeviceType)
                .ThenBy(d => d.BusID);
        }

        private static string GetDevicePlugin(string UUID)
        {
            var devData = ComputeDeviceManager.Available.Devices.FirstOrDefault(dev => dev.Uuid == UUID);
            if (devData == null) return "";

            return devData.MinerName;
        }

        private static (List<(string name, string unit)> properties, JArray values) GetDeviceOptionalDynamic(ComputeDevice d)
        {
            string getValue<T>(T o) => (typeof(T).Name, o) switch
            {
                (nameof(ILoad), ILoad g) => $"{(int)g.Load}",
                (nameof(IMemControllerLoad), IMemControllerLoad g) => $"{g.MemoryControllerLoad}",
                (nameof(ITemp), ITemp g) => $"{g.Temp}",
                (nameof(IGetFanSpeedPercentage), IGetFanSpeedPercentage g) => $"{g.GetFanSpeedPercentage().percentage}",
                (nameof(IPowerUsage), IPowerUsage g) => $"{g.PowerUsage}",
                (nameof(IVramTemp), IVramTemp g) => $"{g.VramTemp}",
                (nameof(IHotspotTemp), IHotspotTemp g) => $"{g.HotspotTemp}",
                (_, _) => null,
            };

            string getValueForName(string name) => name switch
            {
                "Miner" => $"{GetDevicePlugin(d.Uuid)}",
                _ => null,
            };

            (string name, string unit, string value)? pairOrNull<T>(string name, string unit)
            {
                if (d.MonitorConnected is T sensor) return (name, unit, getValue<T>(sensor));
                if (typeof(T) == typeof(string)) return (name, unit, getValueForName(name));
                return null;
            }

            // here sort manually by type 
            var dynamicPropertiesWithValues = new List<(string name, string unit, string value)?>
            {
                pairOrNull<ITemp>("Temp.","C"),
                pairOrNull<IVramTemp>("VRAM T.","C"),
                pairOrNull<ILoad>("Load","%"),
                pairOrNull<IMemControllerLoad>("MemCtrl Load","%"),
                pairOrNull<IGetFanSpeedPercentage>("Fan","%"),
                pairOrNull<IPowerUsage>("Power","W"),
                pairOrNull<string>("Miner", ""),
            };

            var deviceOptionalDynamic = dynamicPropertiesWithValues
                .Where(p => p.HasValue)
                .Where(p => p.Value.value != null)
                .Select(p => p.Value)
                .ToArray();

            var optionalDynamicProperties = deviceOptionalDynamic.Select(p => (p.name, p.unit)).ToList();
            var values_odv = new JArray(deviceOptionalDynamic.Select(p => p.value));
            return (optionalDynamicProperties, values_odv);
        }

        // we cache device properties so we persevere  property IDs
        private static readonly Dictionary<ComputeDevice, List<OptionalMutableProperty>> _cachedDevicesOptionalMutable = new Dictionary<ComputeDevice, List<OptionalMutableProperty>>();
        private static (List<OptionalMutableProperty> properties, JArray values) GetDeviceOptionalMutable(ComputeDevice d)
        {
            OptionalMutableProperty valueOrNull<T>(OptionalMutableProperty v) => d.MonitorConnected is T ? v : null;
            List<OptionalMutableProperty> getOptionalMutableProperties(ComputeDevice d)
            {
                // TODO sort by type
                var optionalProperties = new List<OptionalMutableProperty>
                    {
                        null,
                        /*
                        valueOrNull<ITDP>(new OptionalMutablePropertyEnum
                        {
                            PropertyID = OptionalMutableProperty.NextPropertyId(), // TODO this will eat up the ID
                            DisplayName = "TDP Simple",
                            DefaultValue = "Medium",
                            Range = new List<string>{ "Low", "Medium", "High" },
                            // TODO action/setter to execute
                            ExecuteTask = async (object p) =>
                            {
                                // #1 validate JSON input
                                if (p is string pstr && pstr is not null) return Task.FromResult<object>(null);
                                // TODO do something
                                return Task.FromResult<object>(null);
                            },
                            GetValue = () =>
                            {
                                var ret = d.TDPSimple switch
                                {
                                    TDPSimpleType.LOW => "Low",
                                    TDPSimpleType.MEDIUM => "Medium",
                                    TDPSimpleType.HIGH => "High",
                                    _ => "ERROR",
                                };
                                return ret;
                            }
                        }),
                        */
                    };
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
            var values_omv = new JArray(props.Select(p => p.GetValue()));

            return (props, values_omv);
        }

        private static LoginMessage _loginMessage = null;
        //public static LoginMessage CreateLoginMessage(string btc, string worker, string rigID, IOrderedEnumerable<ComputeDevice> devices)
        public static LoginMessage CreateLoginMessage(string btc, string worker, string rigID, IOrderedEnumerable<ComputeDevice> devices)
        {
            var sorted = SortedDevices(devices);
            if (_loginMessage != null) return _loginMessage;

            List<NhnwsAction> createDefaultActions() =>
                new List<NhnwsAction>
                {
                    new NhnwsAction
                    {
                        ActionID = NhnwsAction.NextActionId(),
                        DisplayName = "Mining start",
                        DisplayGroup = 1,
                    },
                    new NhnwsAction
                    {
                        ActionID = NhnwsAction.NextActionId(),
                        DisplayName = "Mining stop",
                        DisplayGroup = 1,
                    },
                    new NhnwsAction
                    {
                        ActionID = NhnwsAction.NextActionId(),
                        DisplayName = "Mining enable",
                        DisplayGroup = 1,
                    },
                    new NhnwsAction
                    {
                        ActionID = NhnwsAction.NextActionId(),
                        DisplayName = "Mining disable",
                        DisplayGroup = 1,
                    },
                };

            NiceHashMiner.Stats.V4.Device mapComputeDevice(ComputeDevice d)
            {
                List<JArray> getStaticPropertiesOptionalValues(ComputeDevice d)
                {
                    return d switch
                    {
                        IGpuDevice gpu => new List<JArray>
                        {
                            new JArray("bus_id", $"{gpu.PCIeBusID}"),
                            new JArray("vram", $"{gpu.GpuRam}"),
                        },
                        _ => new List<JArray> { },
                    };
                }

                return new NiceHashMiner.Stats.V4.Device
                {
                    StaticProperties = new Dictionary<string, object>
                    {
                        { "device_id", d.DevUuid },
                        { "class", $"{(int)d.DeviceType}" },//у найса +1
                        { "name", d.Name },
                        { "optional", getStaticPropertiesOptionalValues(d) },
                    },
                    Actions = createDefaultActions(),
                    OptionalDynamicProperties = GetDeviceOptionalDynamic(d).properties,
                    //OptionalMutableProperties = GetDeviceOptionalMutable(d).properties,
            };

            }

            string winver = "Windows version " + Form_Main.GetWinVer(Environment.OSVersion.Version) +
                " (" + Environment.OSVersion.Version.Major.ToString() + "." + Environment.OSVersion.Version.Minor.ToString() +
                " build: " + Environment.OSVersion.Version.Build.ToString() + ")";
            _loginMessage = new LoginMessage
            {
                Btc = btc,
                Worker = worker,
                RigID = rigID,
                Version = new List<string> { NiceHashSocket.version, winver },
                /*
                OptionalMutableProperties = new List<OptionalMutableProperty>
                {
                    new OptionalMutablePropertyString
                    {
                        PropertyID = OptionalMutableProperty.NextPropertyId(),
                        DisplayGroup = 0,
                        DisplayName = "User name",
                        DefaultValue = btc,
                        Range = (64, null),
                    },
                    new OptionalMutablePropertyString
                    {
                        PropertyID = OptionalMutableProperty.NextPropertyId(),
                        DisplayGroup = 0,
                        DisplayName = "Worker name",
                        DefaultValue = worker,
                        Range = (64, null),
                    },
                },
                */
                
                Actions = createDefaultActions(),
                Devices = devices.Select(mapComputeDevice).ToList(),
                MinerState = GetMinerStateValues(worker, devices),
            };

            return _loginMessage;
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
                    DeviceState.Gaming => 6, //GAMING
#endif
                    _ => 0, // UNKNOWN
                };

                JArray mdv(ComputeDevice d)
                {
                    var state = deviceStateToInt(d.State);
                    var speeds = NiceHashStats.GetSpeedForDevice(d.Uuid);
                    return new JArray(state, new JArray(speeds.Select(kvp => new JArray((int)kvp.type, kvp.speed))));
                }
                JArray mmv(ComputeDevice d)
                {
                    return new JArray(deviceStateToInt(d.State));
                }

                return new MinerState.DeviceState
                {
                    MutableDynamicValues = mdv(d),
                    OptionalDynamicValues = GetDeviceOptionalDynamic(d).values, // odv
                    MandatoryMutableValues = mmv(d),
                    OptionalMutableValues = GetDeviceOptionalMutable(d).values, // omv
                };
            }

            return new MinerState
            {
                MutableDynamicValues = new JArray(rigStateToInt(rig)),
                OptionalDynamicValues = new JArray(),
                MandatoryMutableValues = new JArray(rigStateToInt(rig), workerName),
                OptionalMutableValues = new JArray(),
                Devices = devices.Select(toDeviceState).ToList(),
            };
        }

    }
}
