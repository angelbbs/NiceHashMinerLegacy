/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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
                // sgminer stuff
                if (algoSettings.ContainsKey(MinerBaseType.sgminer))
                {
                    var sgminerAlgos = algoSettings[MinerBaseType.sgminer];
                    //var lyra2REv2Index = sgminerAlgos.FindIndex(el => el.NiceHashID == AlgorithmType.Lyra2REv2);
                    //var neoScryptIndex = sgminerAlgos.FindIndex(el => el.NiceHashID == AlgorithmType.NeoScrypt);
                    //var cryptoNightIndex = sgminerAlgos.FindIndex(el => el.NiceHashID == AlgorithmType.CryptoNight);

                    // Check for optimized version
                    /*
                    if (lyra2REv2Index > -1)
                    {
                        sgminerAlgos[lyra2REv2Index].ExtraLaunchParameters =
                            AmdGpuDevice.DefaultParam +
                            "--nfactor 10 --xintensity 64 --thread-concurrency 0 --worksize 64 --gpu-threads 2";
                    }
                    */
                    /*
                    if (!device.Codename.Contains("Tahiti") && neoScryptIndex > -1)
                    {
                        sgminerAlgos[neoScryptIndex].ExtraLaunchParameters =
                            AmdGpuDevice.DefaultParam +
                            "--nfactor 10 --xintensity    2 --thread-concurrency 8192 --worksize  64 --gpu-threads 2";
                        Helpers.ConsolePrint("ComputeDevice",
                            "The GPU detected (" + device.Codename +
                            ") is not Tahiti. Changing default gpu-threads to 2.");
                    }
                    */
                    /*
                    if (cryptoNightIndex > -1)
                    {
                        if (device.Codename.Contains("Hawaii"))
                        {
                            sgminerAlgos[cryptoNightIndex].ExtraLaunchParameters = "--rawintensity 640 -w 8 -g 2";
                        }
                        else if (device.Name.Contains("Vega"))
                        {
                            sgminerAlgos[cryptoNightIndex].ExtraLaunchParameters =
                                AmdGpuDevice.DefaultParam + " --rawintensity 1850 -w 8 -g 2";
                        }
                    }
                    */
                }
                /*
                if (algoSettings.ContainsKey(MinerBaseType.XmrigAMD))
                {
                    var XmrigAMDAlgos = algoSettings[MinerBaseType.XmrigAMD];
                    //int xmrigCryptoNightV7_Index = XmrigAMDAlgos.FindIndex((el) => el.NiceHashID == AlgorithmType.CryptoNightV7);
                   // int xmrigCryptoNightV8_Index = XmrigAMDAlgos.FindIndex((el) => el.NiceHashID == AlgorithmType.CryptoNightV8);
                 //   int xmrigCryptoNightHeavy_Index = XmrigAMDAlgos.FindIndex((el) => el.NiceHashID == AlgorithmType.CryptoNightHeavy);
                    int xmrigCryptoNightR_Index = XmrigAMDAlgos.FindIndex((el) => el.NiceHashID == AlgorithmType.CryptoNightR);

                    //--opencl-launch=
                    //XmrigAMDAlgos[xmrigCryptoNightV7_Index].ExtraLaunchParameters =" --opencl-launch=640";
                   // XmrigAMDAlgos[xmrigCryptoNightV8_Index].ExtraLaunchParameters = " --opencl-launch=640";
                  //  XmrigAMDAlgos[xmrigCryptoNightHeavy_Index].ExtraLaunchParameters = " --opencl-launch=640";
                    XmrigAMDAlgos[xmrigCryptoNightR_Index].ExtraLaunchParameters = " --opencl-launch=640";

                    if (xmrigCryptoNightR_Index > -1)
                    {
                        if (device.Codename.Contains("gfx804")) //rx550
                        {
                            XmrigAMDAlgos[xmrigCryptoNightR_Index].ExtraLaunchParameters = " --opencl-launch=512";
                          //  XmrigAMDAlgos[xmrigCryptoNightV8_Index].ExtraLaunchParameters = " --opencl-launch=512";
                        //    XmrigAMDAlgos[xmrigCryptoNightHeavy_Index].ExtraLaunchParameters = " --opencl-launch=512";
                        }
                        if (device.Codename.Contains("Pitcairn")) //r7-370
                        {
                            XmrigAMDAlgos[xmrigCryptoNightR_Index].ExtraLaunchParameters = " --opencl-launch=640";
                           // XmrigAMDAlgos[xmrigCryptoNightV8_Index].ExtraLaunchParameters = " --opencl-launch=640";
                          //  XmrigAMDAlgos[xmrigCryptoNightHeavy_Index].ExtraLaunchParameters = " --opencl-launch=640";
                        }
                        if (device.Codename.Contains("Baffin")) //rx460/560
                        {
                            XmrigAMDAlgos[xmrigCryptoNightR_Index].ExtraLaunchParameters = " --opencl-launch=896";
                           // XmrigAMDAlgos[xmrigCryptoNightV8_Index].ExtraLaunchParameters = " --opencl-launch=896";
                          //  XmrigAMDAlgos[xmrigCryptoNightHeavy_Index].ExtraLaunchParameters = " --opencl-launch=896";
                        }

                        if (device.Codename.Contains("Ellesmere")) //rx570/580
                        {
                            XmrigAMDAlgos[xmrigCryptoNightR_Index].ExtraLaunchParameters = " --opencl-launch=896";
                           // XmrigAMDAlgos[xmrigCryptoNightV8_Index].ExtraLaunchParameters = " --opencl-launch=896";
                        //    XmrigAMDAlgos[xmrigCryptoNightHeavy_Index].ExtraLaunchParameters = " --opencl-launch=896";
                        }

                        if (device.Codename.Contains("Hawaii"))
                        {
                            XmrigAMDAlgos[xmrigCryptoNightR_Index].ExtraLaunchParameters = " --opencl-launch=896";
                           // XmrigAMDAlgos[xmrigCryptoNightV8_Index].ExtraLaunchParameters = " --opencl-launch=896";
                       //     XmrigAMDAlgos[xmrigCryptoNightHeavy_Index].ExtraLaunchParameters = " --opencl-launch=896";
                        }
                        else if (device.Name.Contains("Vega"))
                        {
                            XmrigAMDAlgos[xmrigCryptoNightR_Index].ExtraLaunchParameters = " --opencl-launch=1920";
                           // XmrigAMDAlgos[xmrigCryptoNightV8_Index].ExtraLaunchParameters = " --opencl-launch=1920";
                      //      XmrigAMDAlgos[xmrigCryptoNightHeavy_Index].ExtraLaunchParameters = " --opencl-launch=1920";
                        }
                        else if (device.Name.Contains("gfx903"))
                        {
                            XmrigAMDAlgos[xmrigCryptoNightR_Index].ExtraLaunchParameters = " --opencl-launch=1920";
                           // XmrigAMDAlgos[xmrigCryptoNightV8_Index].ExtraLaunchParameters = " --opencl-launch=1920";
                       //     XmrigAMDAlgos[xmrigCryptoNightHeavy_Index].ExtraLaunchParameters = " --opencl-launch=1920";
                        }
                    }


                }
                */
                /*
                if (algoSettings.ContainsKey(MinerBaseType.mkxminer))
                {
                    var mkxminerAlgos = algoSettings[MinerBaseType.mkxminer];
                    int mkxminer_Index = mkxminerAlgos.FindIndex((el) => el.NiceHashID == AlgorithmType.Lyra2z);

                    //--opencl-launch=
                    //mkxminerAlgos[mkxminer_Index].ExtraLaunchParameters = " ";
                    if (mkxminer_Index > -1)
                    {
                        if (device.Codename.Contains("gfx804")) //rx550
                        {
                            mkxminerAlgos[mkxminer_Index].ExtraLaunchParameters = " -I 21";
                        }
                        if (device.Codename.Contains("Pitcairn")) //r7-370
                        {
                            mkxminerAlgos[mkxminer_Index].ExtraLaunchParameters = " -I 21";
                        }
                        if (device.Codename.Contains("Baffin")) //rx460/560
                        {
                            mkxminerAlgos[mkxminer_Index].ExtraLaunchParameters = " -I 22";
                        }

                        if (device.Codename.Contains("Ellesmere")) //rx570/580
                        {
                            mkxminerAlgos[mkxminer_Index].ExtraLaunchParameters = " -I 23";
                        }

                        if (device.Codename.Contains("Hawaii"))
                        {
                            mkxminerAlgos[mkxminer_Index].ExtraLaunchParameters = " -I 22"; ;
                        }
                        else if (device.Name.Contains("Vega"))
                        {
                            mkxminerAlgos[mkxminer_Index].ExtraLaunchParameters = " -I 24";
                        }
                    }

                }
                */
                // Ellesmere, Polaris
                // Ellesmere sgminer workaround, keep this until sgminer is fixed to work with Ellesmere
                if (device.Codename.Contains("Ellesmere") || device.InfSection.ToLower().Contains("polaris"))
                {
                    foreach (var algosInMiner in algoSettings)
                    {
                        foreach (var algo in algosInMiner.Value)
                        {
                            // disable all algos in list
                            if (algo.NiceHashID == AlgorithmType.Decred || algo.NiceHashID == AlgorithmType.Lbry)
                            {
                                algo.Enabled = false;
                            }
                        }
                    }
                }
                // non sgminer optimizations
                /*
                if (algoSettings.ContainsKey(MinerBaseType.Claymore_old) &&
                    algoSettings.ContainsKey(MinerBaseType.Claymore))
                {
                    var claymoreOldAlgos = algoSettings[MinerBaseType.Claymore_old];
                    var cryptoNightOldIndex =
                        claymoreOldAlgos.FindIndex(el => el.NiceHashID == AlgorithmType.CryptoNight);

                    var claymoreNewAlgos = algoSettings[MinerBaseType.Claymore];
                    var cryptoNightNewIndex =
                        claymoreNewAlgos.FindIndex(el => el.NiceHashID == AlgorithmType.CryptoNight);

                    if (cryptoNightOldIndex > -1 && cryptoNightNewIndex > -1)
                    {
                        //string regex_a_3 = "[5|6][0-9][0-9][0-9]";
                        var a4 = new List<string>
                        {
                            "270",
                            "270x",
                            "280",
                            "280x",
                            "290",
                            "290x",
                            "370",
                            "380",
                            "390",
                            "470",
                            "480"
                        };
                        foreach (var namePart in a4)
                        {
                            if (!device.Name.Contains(namePart)) continue;
                            claymoreOldAlgos[cryptoNightOldIndex].ExtraLaunchParameters = "-a 4";
                            break;
                        }

                        var old = new List<string>
                        {
                            "Verde",
                            "Oland",
                            "Bonaire"
                        };
                        foreach (var codeName in old)
                        {
                            var isOld = device.Codename.Contains(codeName);
                            claymoreOldAlgos[cryptoNightOldIndex].Enabled = isOld;
                            claymoreNewAlgos[cryptoNightNewIndex].Enabled = !isOld;
                        }
                    }
                }
                */
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
                int miniZBeam_Index = miniZAlgos.FindIndex((el) => el.NiceHashID == AlgorithmType.Beam);

                if (miniZBeam_Index > -1)
                {
                    if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.7))
                    {
                        miniZAlgos[miniZBeam_Index].ExtraLaunchParameters = " --mode=3";
                    }
                }
                int miniZBeamV2_Index = miniZAlgos.FindIndex((el) => el.NiceHashID == AlgorithmType.BeamV2);

                if (miniZBeamV2_Index > -1)
                {
                    if (device.GpuRam < (ulong)(1024 * 1024 * 1024 * 4.7))
                    {
                        miniZAlgos[miniZBeamV2_Index].ExtraLaunchParameters = " --mode=3";
                    }
                }

            }
            //if (algoSettings.ContainsKey(MinerBaseType.Claymore) && (device.Name.Contains("3GB"))
            if (algoSettings.ContainsKey(MinerBaseType.Claymore) && device.GpuRam < (ulong)(1024 *1024*1024 * 3.7) && !device.Name.Contains("R7 370"))
             {
                algoSettings = FilterMinerBaseTypes(algoSettings, new List<MinerBaseType>
                    {
                        MinerBaseType.Claymore
                    });
            }

            if (algoSettings.ContainsKey(MinerBaseType.Phoenix) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.7) && !device.Name.Contains("R7 370"))
            {
                algoSettings = FilterMinerBaseTypes(algoSettings, new List<MinerBaseType>
                    {
                        MinerBaseType.Phoenix
                    });
            }

            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.4))
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
            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckarood29
                    });
            }
            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && device.DeviceType == DeviceType.AMD && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckaroo29
                    });
            }
            /*
            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && device.GpuRam < 1024 * 1024 * 1024 * 7.4)
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckatoo31
                    });
            }
            */
            if (algoSettings.ContainsKey(MinerBaseType.GMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.GMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo31 && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 7.4))
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
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo31 && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 7.4))
                    {
                        algo.Enabled = false;
                        algo.Hidden = true;
                    }
                }
            }
            /*
            if (algoSettings.ContainsKey(MinerBaseType.lolMiner) && device.GpuRam < 1024 * 1024 * 1024 * 3.4 )
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckarood29
                    });
            }
            */
            /*
            if (algoSettings.ContainsKey(MinerBaseType.teamredminer) && device.GpuRam < 1024 * 1024 * 1024 * 7.4)
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckarood29
                    });
            }
            */
            // разделение для типов работает && device.DeviceType == DeviceType.NVIDIA
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
            /*
            if (algoSettings.ContainsKey(MinerBaseType.GMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.GMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo31 && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < 1024 * 1024 * 1024 * 7.4)
                    {
                        algo.Enabled = false;
                    }
                }
            }
            */
            /*
            if (algoSettings.ContainsKey(MinerBaseType.NBMiner) && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < 1024 * 1024 * 1024 * 7.4)
                {
                   algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                   {
                   AlgorithmType.GrinCuckatoo31
                   });
            }
            */
            /*
            if (algoSettings.ContainsKey(MinerBaseType.NBMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.NBMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo31 && device.DeviceType == DeviceType.NVIDIA && device.GpuRam < 1024 * 1024 * 1024 * 7.4)
                    {
                        algo.Enabled = false;
                    }
                }
            }
            */
            /*
            if (algoSettings.ContainsKey(MinerBaseType.NBMiner))
            {
                var NBMinerAlgos = algoSettings[MinerBaseType.NBMiner];
                int g31_Index = NBMinerAlgos.FindIndex((el) => el.NiceHashID == AlgorithmType.GrinCuckatoo31);
                NBMinerAlgos[g31_Index].Enabled = false;
                //--opencl-launch=
                //XmrigAMDAlgos[xmrigCryptoNightV7_Index].ExtraLaunchParameters =" --opencl-launch=640";
                //XmrigAMDAlgos[xmrigCryptoNightV8_Index].ExtraLaunchParameters = " --opencl-launch=640";

            }
            */

            if (algoSettings.ContainsKey(MinerBaseType.lolMiner))
            {
                foreach (var algo in algoSettings[MinerBaseType.lolMiner])
                {
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo31 && device.DeviceType == DeviceType.AMD && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 15.1))
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
                    if (algo.NiceHashID == AlgorithmType.GrinCuckatoo31 && device.DeviceType == DeviceType.AMD && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 15.1))
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
            if (algoSettings.ContainsKey(MinerBaseType.NBMiner) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 5.4))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckarood29
                    });
            }
            /*
            if (algoSettings.ContainsKey(MinerBaseType.NBMiner) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.7))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.DaggerHashimoto
                    });
            }
            */
            /*
            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && device.GpuRam < (ulong)(1024 * 1024 * 1024 * 3.1))
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.DaggerHashimoto
                    });
            }
            */
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
            /*
            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && (device.Name.Contains("1060"))
                )
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckatoo31
                    });
            }


            if (algoSettings.ContainsKey(MinerBaseType.GMiner) && (device.Name.Contains("1050"))
                )
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckaroo29
                    });
            }


            if (algoSettings.ContainsKey(MinerBaseType.Bminer) && (device.Name.Contains("1050"))
                )
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckaroo29
                    });
            }
            if (algoSettings.ContainsKey(MinerBaseType.Bminer) && (device.Name.Contains("1050"))
                )
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckatoo31
                    });
            }


            if (algoSettings.ContainsKey(MinerBaseType.Bminer) && (device.Name.Contains("1060"))
                )
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckaroo29
                    });
            }
            if (algoSettings.ContainsKey(MinerBaseType.Bminer) && (device.Name.Contains("1060"))
                )
            {
                algoSettings = FilterMinerAlgos(algoSettings, new List<AlgorithmType>
                    {
                        AlgorithmType.GrinCuckatoo31
                    });
            }
            */
            // This is not needed anymore after excavator v1.1.4a
            //if (device.IsSM50() && algoSettings.ContainsKey(MinerBaseType.excavator)) {
            //    int Equihash_index = algoSettings[MinerBaseType.excavator].FindIndex((algo) => algo.NiceHashID == AlgorithmType.Equihash);
            //    if (Equihash_index > -1) {
            //        // -c1 1 needed for SM50 to work ATM
            //        algoSettings[MinerBaseType.excavator][Equihash_index].ExtraLaunchParameters = "-c1 1";
            //    }
            //}
            // NhEqMiner exceptions scope
            /*
            {
                const MinerBaseType minerBaseKey = MinerBaseType.nheqminer;
                if (algoSettings.ContainsKey(minerBaseKey) && device.Name.Contains("GTX")
                    && (device.Name.Contains("560") || device.Name.Contains("650") || device.Name.Contains("680") ||
                        device.Name.Contains("770"))
                )
                {
                    algoSettings = FilterMinerBaseTypes(algoSettings, new List<MinerBaseType>
                    {
                        minerBaseKey
                    });
                }
            }
            */
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
