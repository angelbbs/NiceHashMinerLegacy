using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Miners.Grouping;
using NiceHashMinerLegacy.Common.Enums;

namespace NiceHashMiner.Devices.Algorithms
{
    /// <summary>
    /// GroupAlgorithms creates defaults supported algorithms. Currently based in Miner implementation
    /// </summary>
    public static class GroupAlgorithms
    {
        private static Dictionary<MinerBaseType, List<Algorithm>> CreateForDevice(ComputeDevice device)
        {
            if (device == null) return null;
            var algoSettings = CreateDefaultsForGroup(device.DeviceGroupType);
            if (algoSettings == null) return null;
            if (device.DeviceType == DeviceType.AMD)
            {
                
                
                // drivers algos issue
                if (device.DriverDisableAlgos)
                {
                    algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        //   AlgorithmType.NeoScrypt,
                        //   AlgorithmType.Lyra2REv2
                    });
                }

                // disable another gpu
                //Helpers.ConsolePrint("GPU device", "Name: "+device.Name);
                /*
                if (algoSettings.ContainsKey(MinerBaseType.CastXMR) && (!device.Name.Contains("470")
                    && !device.Name.Contains("480") && !device.Name.Contains("570") && !device.Name.Contains("580") &&
                        !device.Name.Contains("Vega"))
                )
                {
                    algoSettings = FilterMinerBaseTypes(algoSettings, new List<MinerBaseType>
                    {
                        MinerBaseType.CastXMR
                    });
                }
                */

                // disable by default
                {
                    var minerBases = new List<MinerBaseType>
                    {
                        MinerBaseType.ethminer,
                        MinerBaseType.OptiminerAMD
                    };
                    foreach (var minerKey in minerBases)
                    {
                        if (!algoSettings.ContainsKey(minerKey)) continue;
                        foreach (var algo in algoSettings[minerKey])
                        {
                            algo.Enabled = false;
                        }
                    }
                    if (algoSettings.ContainsKey(MinerBaseType.sgminer))
                    {
                        foreach (var algo in algoSettings[MinerBaseType.sgminer])
                        {
                            if (algo.NiceHashID == AlgorithmType.DaggerHashimoto)
                            {
                                algo.Enabled = false;
                            }
                        }
                    }
                    //if (algoSettings.ContainsKey(MinerBaseType.Claymore)) {
                    //    foreach (var algo in algoSettings[MinerBaseType.Claymore]) {
                    //        if (algo.NiceHashID == AlgorithmType.CryptoNight) {
                    //            algo.Enabled = false;
                    //        }
                    //    }
                    //}
                }
            } // END AMD case

            // check if it is Etherum capable
            if (device.IsEtherumCapale == false)
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                {
                    AlgorithmType.DaggerHashimoto
                });
            }

            if (algoSettings.ContainsKey(MinerBaseType.ccminer_alexis))
            {
                foreach (var unstableAlgo in algoSettings[MinerBaseType.ccminer_alexis])
                {
                    unstableAlgo.Enabled = false;
                }
            }
            if (algoSettings.ContainsKey(MinerBaseType.experimental))
            {
                foreach (var unstableAlgo in algoSettings[MinerBaseType.experimental])
                {
                    unstableAlgo.Enabled = false;
                }
            }
            Helpers.ConsolePrint("GPU MEMORY: ", device.GpuRam.ToString() + " bytes - " + device.Name);
            if (algoSettings.ContainsKey(MinerBaseType.miniZ))
            {
                var miniZAlgos = algoSettings[MinerBaseType.miniZ];

                int miniZBeamV2_Index = miniZAlgos.FindIndex((el) => el.NiceHashID == AlgorithmType.BeamV2);

                if (miniZBeamV2_Index > -1)
                {
                    if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.7))
                    {
                        miniZAlgos[miniZBeamV2_Index].ExtraLaunchParameters = " --mode=3";
                    }
                }

            }


            if (device.DeviceType == DeviceType.NVIDIA && (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 2.7) || device.GpuRam > (ulong)(1024 * 1024 * 1024 * 4.7) ))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.DaggerHashimoto3GB
                    });
            }
            if (device.DeviceType == DeviceType.NVIDIA && (device.GpuRam > (ulong)(1024 * 1024 * 1024 * 2.7) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.7)))
            {
                Form_Main.DaggerHashimoto3GB = true;
            } else
            {
                Form_Main.DaggerHashimoto3GB = false;
            }

            if (device.DeviceType == DeviceType.AMD && (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 1.7) || device.GpuRam > (ulong)(1024 * 1024 * 1024 * 4.7)))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.DaggerHashimoto4GB
                    });
            }
            if (device.DeviceType == DeviceType.AMD && (device.GpuRam > (ulong)(1024 * 1024 * 1024 * 1.2) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.7)))
            {
                Form_Main.DaggerHashimoto4GB = true;
            }
            else
            {
                Form_Main.DaggerHashimoto4GB = false;
            }

            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.CuckooCycle
                    });
            }

            if (algoSettings.ContainsKey(MinerBaseType.Bminer) && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.CuckooCycle
                    });
            }

            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckaroo29
                    });
            }

            if (algoSettings.ContainsKey(MinerBaseType.Bminer) && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckaroo29
                    });
            }

            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.7))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.Cuckaroom
                    });
            }
            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.7))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.CuckaRooz29
                    });
            }

            if (algoSettings.ContainsKey(MinerBaseType.NBMiner) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.5))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.Octopus
                    });
            }
            if (algoSettings.ContainsKey(MinerBaseType.trex) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.5))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.Octopus
                    });
            }

            if (algoSettings.ContainsKey(MinerBaseType.Bminer) && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.7))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.Cuckaroom
                    });
            }

            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckarood29
                    });
            }


            if (algoSettings.ContainsKey(MinerBaseType.GMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.GMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo31 && device.DeviceType == DeviceType.NVIDIA && 
                        device.GpuRam < (ulong)(1024 * 1024 * 1024 * 7.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo31 && device.DeviceType == DeviceType.NVIDIA &&
                        device.GpuRam > (ulong)(1024 * 1024 * 1024 * 7.4) && Form_Main.GetWinVer(Environment.OSVersion.Version) < 8.0)
                    {
                        algo.Enabled = true;
                        algo.Hidden = false;
                    }
                }
            }
            /*
            if (algoSettings.ContainsKey(MinerBaseType.GMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.GMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.Cuckaroo29BFC && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                    if (algo.NiceHashID == AlgorithmType.Cuckaroo29BFC && device.DeviceType == DeviceType.AMD && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            */
            if (algoSettings.ContainsKey(MinerBaseType.GMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.GMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo32 && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }

            if (algoSettings.ContainsKey(MinerBaseType.NBMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.NBMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo32 && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }

            if (algoSettings.ContainsKey(MinerBaseType.lolMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.lolMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo32 && device.DeviceType == DeviceType.AMD && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            
            if (algoSettings.ContainsKey(MinerBaseType.NBMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.NBMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo31 && device.DeviceType == DeviceType.NVIDIA &&
                        device.GpuRam < (ulong)(1024 * 1024 * 1024 * 7.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo31 && device.DeviceType == DeviceType.NVIDIA &&
                        device.GpuRam > (ulong)(1024 * 1024 * 1024 * 7.4) && Form_Main.GetWinVer(Environment.OSVersion.Version) < 8.0)
                    {
                        algo.Enabled = true;
                        algo.Hidden = false;
                    }
                }
            }
            /*
            if (algoSettings.ContainsKey(MinerBaseType.NBMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.NBMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.Cuckaroo29BFC && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                    if (algo.NiceHashID == AlgorithmType.Cuckaroo29BFC && device.DeviceType == DeviceType.AMD && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            */
            //это починил. 
            /*
            if (algoSettings.ContainsKey(MinerBaseType.GMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.GMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.DaggerHashimoto && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                    if (algo.NiceHashID == AlgorithmType.DaggerHashimoto && device.DeviceType == DeviceType.AMD && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            */
            if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.KAWPOW
                    });
            }
            if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 2.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.Autolykos
                    });
            }

            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && device.DeviceType == DeviceType.AMD && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.CuckooCycle
                    });
            }
            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.CuckooCycle
                    });
            }

            if (algoSettings.ContainsKey(MinerBaseType.lolMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.lolMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo31 && device.DeviceType == DeviceType.AMD &&
                        device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.7))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }

            if (algoSettings.ContainsKey(MinerBaseType.lolMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.lolMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.CuckooCycle && device.DeviceType == DeviceType.AMD &&
                        device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.7))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }

            if (algoSettings.ContainsKey(MinerBaseType.lolMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.lolMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.GrinCuckarood29 && device.DeviceType == DeviceType.AMD && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.7))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }

            if (algoSettings.ContainsKey(MinerBaseType.teamredminer))
            {
                foreach (var algo in algoSettings[MinerBaseType.teamredminer])
                {
                    if (algo.NiceHashID == AlgorithmType.GrinCuckarood29 && device.DeviceType == DeviceType.AMD && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 7.3))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }

            if (algoSettings.ContainsKey(MinerBaseType.NBMiner) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckaroo29
                    });
            }
            if (algoSettings.ContainsKey(MinerBaseType.NBMiner) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckarood29
                    });
            }
            if (algoSettings.ContainsKey(MinerBaseType.lolMiner) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.7))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.Cuckaroom
                    });
            }
            
            if (algoSettings.ContainsKey(MinerBaseType.teamredminer) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.MTP
                    });
            }
            if (algoSettings.ContainsKey(MinerBaseType.CryptoDredge) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.MTP
                    });
            }
            if (algoSettings.ContainsKey(MinerBaseType.TTMiner) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.MTP
                    });
            }


            if (algoSettings.ContainsKey(MinerBaseType.ccminer) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.MTP
                    });
            }

            return algoSettings;
        }

        public static List<Algorithm> CreateForDeviceList(ComputeDevice device)
        {
            var ret = new List<Algorithm>();
            var retDict = CreateForDevice(device);
            if (retDict != null)
            {
                foreach (var kvp in retDict)
                {
                    ret.AddRange(kvp.Value);
                }
            }
            return ret;
        }

        public static Dictionary<MinerBaseType, List<Algorithm>> CreateDefaultsForGroup(DeviceGroupType deviceGroupType)
        {
            switch (deviceGroupType)
            {
                case DeviceGroupType.CPU:
                    return DefaultAlgorithms.Cpu;

                case DeviceGroupType.AMD_OpenCL:
                    return DefaultAlgorithms.Amd;

                case DeviceGroupType.NVIDIA_2_1:
                case DeviceGroupType.NVIDIA_3_x:
                case DeviceGroupType.NVIDIA_5_x:
                case DeviceGroupType.NVIDIA_6_x:
                    var toRemoveAlgoTypes = new List<AlgorithmType>();
                    var toRemoveMinerTypes = new List<MinerBaseType>();

                    var ret = DefaultAlgorithms.Nvidia;

                    switch (deviceGroupType)
                    {
                        case DeviceGroupType.NVIDIA_6_x:
                        case DeviceGroupType.NVIDIA_5_x:
                            toRemoveMinerTypes.AddRange(new[]
                            {
                                MinerBaseType.nheqminer
                            });
                            break;
                        case DeviceGroupType.NVIDIA_2_1:
                        case DeviceGroupType.NVIDIA_3_x:
                            toRemoveAlgoTypes.AddRange(new[]
                            {
                                AlgorithmType.NeoScrypt,
                                //AlgorithmType.Lyra2RE,
                                //AlgorithmType.Lyra2REv2,
                                //AlgorithmType.CryptoNightV7
                            });
                            toRemoveMinerTypes.AddRange(new[]
                            {
                                //MinerBaseType.eqm,
                                MinerBaseType.EWBF,
                                MinerBaseType.dstm
                            });
                            break;
                    }
                    if (DeviceGroupType.NVIDIA_2_1 == deviceGroupType)
                    {
                        toRemoveAlgoTypes.AddRange(new[]
                        {
                            AlgorithmType.DaggerHashimoto,
                            //AlgorithmType.CryptoNight,
                            //AlgorithmType.Pascal,
                            //AlgorithmType.X11Gost
                        });
                        toRemoveMinerTypes.AddRange(new[]
                        {
                            MinerBaseType.Claymore,
                            MinerBaseType.XmrStak
                        });
                    }

                    // filter unused
                    var finalRet = FilterMinerAlgos(ret, toRemoveAlgoTypes, new List<MinerBaseType>
                    {
                        MinerBaseType.ccminer
                    });
                    finalRet = FilterMinerBaseTypes(finalRet, toRemoveMinerTypes);

                    return finalRet;
            }

            return null;
        }

        private static Dictionary<MinerBaseType, List<Algorithm>> FilterMinerBaseTypes(
            Dictionary<MinerBaseType, List<Algorithm>> minerAlgos, List<MinerBaseType> toRemove)
        {
            var finalRet = new Dictionary<MinerBaseType, List<Algorithm>>();
            foreach (var kvp in minerAlgos)
            {
                if (toRemove.IndexOf(kvp.Key) == -1)
                {
                    finalRet[kvp.Key] = kvp.Value;
                }
            }
            return finalRet;
        }

        private static Dictionary<MinerBaseType, List<Algorithm>> FilterMinerAlgos(
            Dictionary<MinerBaseType, List<Algorithm>> minerAlgos, IList<AlgorithmType> toRemove,
            IList<MinerBaseType> toRemoveBase = null)
        {
            var finalRet = new Dictionary<MinerBaseType, List<Algorithm>>();
            if (toRemoveBase == null)
            {
                // all minerbasekeys
                foreach (var kvp in minerAlgos)
                {
                    var algoList = kvp.Value.FindAll(a => toRemove.IndexOf(a.NiceHashID) == -1);
                    if (algoList.Count > 0)
                    {
                        finalRet[kvp.Key] = algoList;
                    }
                }
            }
            else
            {
                foreach (var kvp in minerAlgos)
                {
                    // filter only if base key is defined
                    if (toRemoveBase.IndexOf(kvp.Key) > -1)
                    {
                        var algoList = kvp.Value.FindAll(a => toRemove.IndexOf(a.NiceHashID) == -1);
                        if (algoList.Count > 0)
                        {
                            finalRet[kvp.Key] = algoList;
                        }
                    }
                    else
                    {
                        // keep all
                        finalRet[kvp.Key] = kvp.Value;
                    }
                }
            }
            return finalRet;
        }
    }
}
