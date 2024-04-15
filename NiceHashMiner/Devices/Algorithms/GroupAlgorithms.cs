using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Configs.Data;
using NiceHashMiner.Miners;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.Divert;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace NiceHashMiner.Devices.Algorithms
{
    /// <summary>
    /// GroupAlgorithms creates defaults supported algorithms. Currently based in Miner implementation
    /// </summary>
    public static class GroupAlgorithms
    {
        private static ulong minMem = (ulong)(1024 * 1024 * 8);
        private static Dictionary<MinerBaseType, List<Algorithm>> CreateForDevice(ComputeDevice device)
        {
            if (device == null) return null;
            var algoSettings = CreateDefaultsForGroup(device.DeviceGroupType);
            if (algoSettings == null) return null;

            // check if it is Etherum capable
            if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 7.5))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                {
                    AlgorithmType.DaggerHashimoto
                });
            }

            if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 7.5))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                {
                    AlgorithmType.DaggerAlephium,
                    AlgorithmType.DaggerAutolykos,
                    AlgorithmType.DaggerKarlsenHash
                });
            }

            if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.7))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.ETCHash
                    });
            }
            if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.7))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.ETCHashAlephium,
                        AlgorithmType.ETCHashKarlsenHash
                    });
            }

            if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.1))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.KAWPOW
                    });
            }

            if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.2))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.Autolykos,
                        AlgorithmType.AutolykosIronFish,
                        AlgorithmType.AutolykosZil

                    });
            }

            if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.8))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                {
                    AlgorithmType.FishHash
                });
            }
            if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.8))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                {
                    AlgorithmType.FishHashAlephium
                });
            }
            if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.8))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                {
                    AlgorithmType.FishHashKarlsenHash
                });
            }
            if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.8))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                {
                    AlgorithmType.FishHashPyrinHash
                });
            }
            
            if (algoSettings.ContainsKey(MinerBaseType.GMiner))
            {
                if (device.DeviceType == DeviceType.NVIDIA)
                {
                    foreach (var algo in algoSettings[MinerBaseType.GMiner])
                    {
                        algo.DeviceType = device.DeviceType;
                    }
                }
            }

            //Helpers.ConsolePrint("GPU MEMORY: ", device.GpuRam.ToString() + " bytes - " + device.Name);


            if (device.DeviceType == DeviceType.NVIDIA && (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 2.7) || device.GpuRam > (ulong)(1024 * 1024 * 1024 * 5.7)))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.KAWPOWLite
                    });
            }
            if (device.DeviceType == DeviceType.NVIDIA && device.GpuRam > (ulong)(1024 * 1024 * 1024 * 2.7) &&
                device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.7) && device.Enabled)
            {
                Form_Main.KawpowLite = true;
                minMem = Math.Min(minMem, device.GpuRam / 1024);

                if (minMem > (ulong)(1024 * 1024 * 2.7) && minMem < (ulong)(1024 * 1024 * 3.7))
                {
                    Form_Main.KawpowLite3GB = true;
                    Form_Main.KawpowLite4GB = false;
                    Form_Main.KawpowLite5GB = false;
                }
                if (minMem > (ulong)(1024 * 1024 * 3.7) && minMem < (ulong)(1024 * 1024 * 4.7))
                {
                    Form_Main.KawpowLite3GB = false;
                    Form_Main.KawpowLite4GB = true;
                    Form_Main.KawpowLite5GB = false;
                }
                if (minMem > (ulong)(1024 * 1024 * 4.7) && minMem < (ulong)(1024 * 1024 * 5.7))
                {
                    Form_Main.KawpowLite3GB = false;
                    Form_Main.KawpowLite4GB = false;
                    Form_Main.KawpowLite5GB = true;
                }

                if (algoSettings.ContainsKey(MinerBaseType.GMiner) && Divert.CheckWinDivert() <= 0)
                {
                    algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.KAWPOWLite
                    });
                }
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
            if (device.Name.ToLower().Contains("gtx 10") && device.DeviceType == DeviceType.NVIDIA)
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.Octopus,
                        AlgorithmType.OctopusAlephium,
                        AlgorithmType.OctopusKarlsenHash,
                        AlgorithmType.OctopusPyrinHash
                    });
            }

            if (device.Name.ToLower().Contains("gtx 10") && device.DeviceType == DeviceType.NVIDIA)
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.NexaPow
                    });
            }

            if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 6.8))
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
                    //AlgorithmType.Cuckaroom
                });
            }

            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 7.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckatoo32
                    });
            }
            if (algoSettings.ContainsKey(MinerBaseType.lolMiner) && device.DeviceType == DeviceType.AMD && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 7.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckatoo32
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
            //*******************
            if (device.DeviceType == DeviceType.AMD && algoSettings.ContainsKey(MinerBaseType.SRBMiner) &&
                (device.Codename.ToLower().Contains("ellesmere")))
            {
                foreach (var algo in algoSettings[MinerBaseType.SRBMiner])
                {
                    if (algo.DualNiceHashID == AlgorithmType.AutolykosKarlsenHash ||
                        algo.DualNiceHashID == AlgorithmType.AutolykosPyrinHash ||
                        algo.DualNiceHashID == AlgorithmType.FishHashAlephium ||
                        algo.DualNiceHashID == AlgorithmType.FishHashPyrinHash)
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }

            if (device.DeviceType == DeviceType.AMD && algoSettings.ContainsKey(MinerBaseType.lolMiner) &&
                (device.Codename.ToLower().Contains("ellesmere") ||
                device.Name.ToLower().Contains("vega") || device.Name.ToLower().Contains("vii")))
            {
                foreach (var algo in algoSettings[MinerBaseType.lolMiner])
                {
                    if (algo.DualNiceHashID == AlgorithmType.FishHashAlephium ||
                        algo.DualNiceHashID == AlgorithmType.FishHashKarlsenHash ||
                        algo.DualNiceHashID == AlgorithmType.FishHashPyrinHash)
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            
            if (device.DeviceType == DeviceType.AMD && algoSettings.ContainsKey(MinerBaseType.lolMiner) &&
                (device.Codename.ToLower().Contains("gfx1010") || device.Codename.ToLower().Contains("gfx1011") ||
                device.Codename.ToLower().Contains("gfx1012")))//RX 5500/5700/5600(M/XT)
            {
                foreach (var algo in algoSettings[MinerBaseType.lolMiner])
                {
                    if (algo.DualNiceHashID == AlgorithmType.FishHashPyrinHash)
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }

            if (device.DeviceType == DeviceType.AMD && algoSettings.ContainsKey(MinerBaseType.Claymore) &&
                (device.Codename.ToLower().Contains("gfx") || device.Codename.ToLower().Contains("vega") ||
                device.Codename.ToLower().Contains("vii")))
            {
                foreach (var algo in algoSettings[MinerBaseType.Claymore])
                {
                    if (algo.NiceHashID == AlgorithmType.NeoScrypt)
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            //*********
            /*
            if (device.DeviceType == DeviceType.AMD && algoSettings.ContainsKey(MinerBaseType.lolMiner) &&
                (device.Codename.ToLower().Contains("ellesmere") ||
                device.Name.ToLower().Contains("vega") || device.Name.ToLower().Contains("vii")))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                {
                    AlgorithmType.FishHashAlephium,
                    AlgorithmType.FishHashKarlsenHash,
                    AlgorithmType.FishHashPyrinHash
                });
            }
            */
            /*
            if (device.DeviceType == DeviceType.AMD && algoSettings.ContainsKey(MinerBaseType.lolMiner) &&
                (device.Codename.ToLower().Contains("gfx1010") || device.Codename.ToLower().Contains("gfx1011") ||
                device.Codename.ToLower().Contains("gfx1012")))//RX 5500/5700/5600(M/XT)
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                {
                    AlgorithmType.FishHashPyrinHash
                });
            }
            */

            if (algoSettings.ContainsKey(MinerBaseType.GMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.GMiner])
                {
                    if (algo.DualNiceHashID == AlgorithmType.ZelHash &&
                        device.GpuRam < (ulong)(1024 * 1024 * 1024 * 2.7))
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
                    if (algo.DualNiceHashID == AlgorithmType.ZelHash &&
                        device.GpuRam < (ulong)(1024 * 1024 * 1024 * 2.7))
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
                    if (algo.DualNiceHashID == AlgorithmType.BeamV3 &&
                        device.GpuRam < (ulong)(1024 * 1024 * 1024 * 2.3))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            //************* отключение алгоритмов, если отсутствует дополнительный файл майнера
            /*
            string minerfilename = "";
            minerfilename = "gminer\\miner.2.54.exe";
            if (algoSettings.ContainsKey(MinerBaseType.GMiner) &&
                !File.Exists(Directory.GetCurrentDirectory() + "\\miners\\" + minerfilename) &&
                File.Exists(Directory.GetCurrentDirectory() + "\\miners\\" + "gminer\\miner.exe"))
            {
                foreach (var algo in algoSettings[MinerBaseType.GMiner])
                {
                    if (algo.DualNiceHashID == AlgorithmType.BeamV3)
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                        Helpers.ConsolePrint("GroupAlgorithms", "File miners\\" + minerfilename + " not exist. Some algorithms are disabled.");
                    }
                }
            }
            */
            //******************
            if (algoSettings.ContainsKey(MinerBaseType.ZEnemy)) //not supported
            {
                foreach (var algo in algoSettings[MinerBaseType.ZEnemy])
                {
                    if (algo.DualNiceHashID == AlgorithmType.KAWPOW && device.DeviceType == DeviceType.NVIDIA &&
                        (device.Name.Contains("RTX 30")))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            if (algoSettings.ContainsKey(MinerBaseType.ZEnemy)) //not supported
            {
                foreach (var algo in algoSettings[MinerBaseType.ZEnemy])
                {
                    if (algo.DualNiceHashID == AlgorithmType.KAWPOW && device.DeviceType == DeviceType.NVIDIA &&
                        (device.Name.Contains("RTX 40")))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            if (algoSettings.ContainsKey(MinerBaseType.CryptoDredge)) //not supported
            {
                foreach (var algo in algoSettings[MinerBaseType.CryptoDredge])
                {
                    if (algo.DualNiceHashID == AlgorithmType.NeoScrypt && device.DeviceType == DeviceType.NVIDIA &&
                        (device.Name.Contains("RTX 30")))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            if (algoSettings.ContainsKey(MinerBaseType.CryptoDredge)) //not supported
            {
                foreach (var algo in algoSettings[MinerBaseType.CryptoDredge])
                {
                    if (algo.DualNiceHashID == AlgorithmType.NeoScrypt && device.DeviceType == DeviceType.NVIDIA &&
                        (device.Name.Contains("RTX 40")))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            if (algoSettings.ContainsKey(MinerBaseType.CryptoDredge)) //not supported
            {
                foreach (var algo in algoSettings[MinerBaseType.CryptoDredge])
                {
                    if (algo.DualNiceHashID == AlgorithmType.KAWPOW && device.DeviceType == DeviceType.NVIDIA &&
                        (device.Name.Contains("RTX 40")))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }

            if (algoSettings.ContainsKey(MinerBaseType.GMiner)) 
            {
                foreach (var algo in algoSettings[MinerBaseType.GMiner])
                {
                    if (algo.DualNiceHashID == AlgorithmType.GrinCuckatoo32 && device.DeviceType == DeviceType.NVIDIA &&
                        device.Name.Contains("RTX 4060") && device.Name.Contains("Laptop") &&
                        device.GpuRam < (ulong)(1024 * 1024 * 1024 * 11.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }

            if (algoSettings.ContainsKey(MinerBaseType.NBMiner)) //not supported
            {
                foreach (var algo in algoSettings[MinerBaseType.NBMiner])
                {
                    if (algo.DualNiceHashID == AlgorithmType.BeamV3 && device.DeviceType == DeviceType.NVIDIA &&
                        (device.Name.Contains("RTX 40")))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            if (algoSettings.ContainsKey(MinerBaseType.NBMiner)) //not supported
            {
                foreach (var algo in algoSettings[MinerBaseType.NBMiner])
                {
                    if (algo.DualNiceHashID == AlgorithmType.KAWPOW && device.DeviceType == DeviceType.NVIDIA &&
                        (device.Name.Contains("RTX 40")))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            if (algoSettings.ContainsKey(MinerBaseType.NBMiner)) //not supported
            {
                foreach (var algo in algoSettings[MinerBaseType.NBMiner])
                {
                    if (algo.DualNiceHashID == AlgorithmType.Octopus && device.DeviceType == DeviceType.NVIDIA &&
                        (device.Name.Contains("RTX 40")))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            if (algoSettings.ContainsKey(MinerBaseType.NBMiner)) //not supported
            {
                foreach (var algo in algoSettings[MinerBaseType.NBMiner])
                {
                    if (algo.DualNiceHashID == AlgorithmType.Autolykos && device.DeviceType == DeviceType.NVIDIA &&
                        (device.Name.Contains("RTX 40")))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }

            if (algoSettings.ContainsKey(MinerBaseType.GMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.GMiner])
                {
                    if (algo.DualNiceHashID == AlgorithmType.ZelHash &&
                        device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.2) &&
                        Form_Main.GetWinVer(Environment.OSVersion.Version) > 9)
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                        algo.BenchmarkSpeed = 0;
                        algo.BenchmarkSecondarySpeed = 0;
                    }
                }
            }

            
            if (algoSettings.ContainsKey(MinerBaseType.GMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.GMiner])
                {
                    if (algo.DualNiceHashID == AlgorithmType.BeamV3 && device.DeviceType == DeviceType.NVIDIA &&
                        device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                        algo.BenchmarkSpeed = 0;
                        algo.BenchmarkSecondarySpeed = 0;
                    }
                }
            }
            if (algoSettings.ContainsKey(MinerBaseType.GMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.GMiner])
                {
                    if (algo.DualNiceHashID == AlgorithmType.ZHash &&
                        device.GpuRam < (ulong)(1024 * 1024 * 1024 * 2.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                        algo.BenchmarkSpeed = 0;
                        algo.BenchmarkSecondarySpeed = 0;
                    }
                }
            }

            if (algoSettings.ContainsKey(MinerBaseType.miniZ))
            {
                foreach (var algo in algoSettings[MinerBaseType.miniZ])
                {
                    if (algo.DualNiceHashID == AlgorithmType.ZHash && device.DeviceType == DeviceType.AMD &&
                        device.GpuRam < (ulong)(1024 * 1024 * 1024 * 2.7))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            if (algoSettings.ContainsKey(MinerBaseType.miniZ))
            {
                foreach (var algo in algoSettings[MinerBaseType.miniZ])
                {
                    if (algo.DualNiceHashID == AlgorithmType.ZelHash && device.DeviceType == DeviceType.AMD &&
                        device.GpuRam < (ulong)(1024 * 1024 * 1024 * 2.7))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            if (algoSettings.ContainsKey(MinerBaseType.miniZ))
            {
                foreach (var algo in algoSettings[MinerBaseType.miniZ])
                {
                    if (algo.DualNiceHashID == AlgorithmType.ZelHash && device.DeviceType == DeviceType.AMD &&
                        device.Name.Contains("550"))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            if (algoSettings.ContainsKey(MinerBaseType.miniZ))
            {
                foreach (var algo in algoSettings[MinerBaseType.miniZ])
                {
                    if (algo.DualNiceHashID == AlgorithmType.ZHash && device.DeviceType == DeviceType.AMD &&
                        device.Name.Contains("550"))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            /*
            if (algoSettings.ContainsKey(MinerBaseType.miniZ))
            {
                foreach (var algo in algoSettings[MinerBaseType.miniZ])
                {
                    if (algo.DualNiceHashID == AlgorithmType.DaggerHashimoto &
                        (device.DeviceType == DeviceType.AMD || device.DeviceType == DeviceType.NVIDIA))
                    {
                        algo.Enabled = false;
                        if (MinerVersion.Get_miniZ().MinerVersion.Trim().Equals("2.2c"))
                        {
                            algo.Hidden = true;
                        }
                        else
                        {
                            algo.Hidden = false;
                        }
                    }
                }
            }
            */

            if (!ConfigManager.GeneralConfig.ShowHiddenAlgos)
            {
                    algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.X16RV2
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

                case DeviceGroupType.INTEL_OpenCL:
                    return DefaultAlgorithms.Intel;

                case DeviceGroupType.NVIDIA_2_1:
                case DeviceGroupType.NVIDIA_3_x:
                case DeviceGroupType.NVIDIA_5_x:
                case DeviceGroupType.NVIDIA_6_x:
                    var toRemoveAlgoTypes = new List<AlgorithmType>();
                    var toRemoveMinerTypes = new List<MinerBaseType>();
                    return DefaultAlgorithms.Nvidia;
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
                    var algoList = kvp.Value.FindAll(a => toRemove.IndexOf(a.DualNiceHashID) == -1);
                    if (algoList.Count > 0)
                    {
                        finalRet[kvp.Key] = algoList;
                    }
                }
                /*
                foreach (var kvp in minerAlgos)
                {
                    var algoList = kvp.Value.FindAll(a => toRemove.IndexOf(a.DualNiceHashID) == -1);
                    if (algoList.Count > 0)
                    {
                        finalRet[kvp.Key] = algoList;
                    }
                }
                */
            }
            else
            {
                foreach (var kvp in minerAlgos)
                {
                    // filter only if base key is defined
                    if (toRemoveBase.IndexOf(kvp.Key) > -1)
                    {
                        var algoList = kvp.Value.FindAll(a => toRemove.IndexOf(a.DualNiceHashID) == -1);
                        if (algoList.Count > 0)
                        {
                            finalRet[kvp.Key] = algoList;
                        }
                        /*
                        var algoList2 = kvp.Value.FindAll(a => toRemove.IndexOf(a.DualNiceHashID) == -1);
                        if (algoList2.Count > 0)
                        {
                            finalRet[kvp.Key] = algoList2;
                        }
                        */
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
