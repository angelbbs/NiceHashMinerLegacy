using NiceHashMiner.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.Extensions;
using System.Collections.Generic;

namespace NiceHashMiner.Devices.Algorithms
{
    public static class DefaultAlgorithms
    {
        #region All

        private static Dictionary<MinerBaseType, List<Algorithm>> All => new Dictionary<MinerBaseType, List<Algorithm>>
        {
            {

                MinerBaseType.XmrStak,
                new List<Algorithm>
                {
                //    new Algorithm(MinerBaseType.XmrStak, AlgorithmType.CryptoNightV7, ""),
                  //  new Algorithm(MinerBaseType.XmrStak, AlgorithmType.CryptoNightHeavy, ""),
                 //   new Algorithm(MinerBaseType.XmrStak, AlgorithmType.CryptoNightR, ""),
                  //  new Algorithm(MinerBaseType.XmrStak, AlgorithmType.CryptoNightV8, ""),

                }
            }
        };

        #endregion

        #region GPU

        private static Dictionary<MinerBaseType, List<Algorithm>> Gpu => new Dictionary<MinerBaseType, List<Algorithm>>
        {
            
            {
                MinerBaseType.Nanominer,
                new List<Algorithm>()
                {
                    new Algorithm(MinerBaseType.Nanominer, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                    {
                        ExtraLaunchParameters = "memTweak=1"
                    }
                }
            },
            
            {
                MinerBaseType.Phoenix,
                new List<Algorithm>()
                {
                    new Algorithm(MinerBaseType.Phoenix, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                    {
    //                    ExtraLaunchParameters = "-rvram -1 "
                    },
                    new Algorithm(MinerBaseType.Phoenix, AlgorithmType.ETCHash, "ETCHash")
                    {
    //                    ExtraLaunchParameters = "-rvram -1 "
                    }
                }
            },
        };

        #endregion

        #region CPU

        public static Dictionary<MinerBaseType, List<Algorithm>> Cpu => new Dictionary<MinerBaseType, List<Algorithm>>
        {
            {
                MinerBaseType.Xmrig,
                new List<Algorithm>
                {
                    new Algorithm(MinerBaseType.Xmrig, AlgorithmType.RandomX, "RandomX"),
                }
            },
                        {
                MinerBaseType.SRBMiner,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.SRBMiner, AlgorithmType.RandomX, "RandomX")
                            {
                              //  ExtraLaunchParameters = " "
                            },
                            new Algorithm(MinerBaseType.SRBMiner, AlgorithmType.VerusHash, "VerusHash")
                            {
                              //  ExtraLaunchParameters = " "
                            }
                        }
            }
        }.ConcatDict(All);

        #endregion

        #region AMD

        public static Dictionary<MinerBaseType, List<Algorithm>> Amd => new Dictionary<MinerBaseType, List<Algorithm>>
        {
            {
                MinerBaseType.SRBMiner,
                        new List<Algorithm>() {

                            new Algorithm(MinerBaseType.SRBMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                            {
                                //ExtraLaunchParameters = "--gpu-boost 3"
                            },
                            new Algorithm(MinerBaseType.SRBMiner, AlgorithmType.Autolykos, "Autolykos")
                            {
                                //ExtraLaunchParameters = "--gpu-boost 3 --gpu-autolykos2-preload 1"
                            },
                            /*
                            new DualAlgorithm(MinerBaseType.SRBMiner, AlgorithmType.Autolykos, AlgorithmType.DaggerHashimoto, "Autolykos+Zilliqa")
                            {
                                //ExtraLaunchParameters = "--gpu-boost 3"
                            },
                            */
                            new Algorithm(MinerBaseType.SRBMiner, AlgorithmType.KHeavyHash, "KHeavyHash")
                            {
                                //ExtraLaunchParameters = "--gpu-boost 3 --gpu-autolykos2-preload 1"
                            },
                            new DualAlgorithm(MinerBaseType.SRBMiner, AlgorithmType.Autolykos, AlgorithmType.KHeavyHash, "AutolykosKHeavyHash")
                            {
                                ExtraLaunchParameters = "--gpu-auto-tune 1 --gpu-autolykos2-preload 1"
                            },
                            new DualAlgorithm(MinerBaseType.SRBMiner, AlgorithmType.DaggerHashimoto, AlgorithmType.KHeavyHash, "DaggerKHeavyHash")
                            {
                                ExtraLaunchParameters = "--gpu-auto-tune 1"
                            }
                        }
            },
            {
                MinerBaseType.GMiner,
                    new List<Algorithm>
                    {
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.ZHash, "ZHash")
                        {
                        },
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.ZelHash, "ZelHash")
                        {
                        },
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.KAWPOW, "KAWPOW")
                        {
                        },
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                        {
                        },
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.ETCHash, "ETCHash")
                        {
                        }
                    }
            },
            {
                MinerBaseType.teamredminer,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.teamredminer, AlgorithmType.DaggerHashimoto, "DaggerHashimoto"),
                            new Algorithm(MinerBaseType.teamredminer, AlgorithmType.KAWPOW, "KAWPOW"),
                            new Algorithm(MinerBaseType.teamredminer, AlgorithmType.Autolykos, "Autolykos")
                            /*
                            new DualAlgorithm(MinerBaseType.teamredminer, AlgorithmType.Autolykos, AlgorithmType.KHeavyHash, "AutolykosKHeavyHash")
                            {
                                ExtraLaunchParameters = "--kas_end"
                            },
                            */
                        }
            },
            {
                MinerBaseType.lolMiner,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.ZHash, "ZHash")
                            {
                                ExtraLaunchParameters = ""
                            },
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.ZelHash, "ZelHash")
                            {
                                ExtraLaunchParameters = ""
                            },
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.BeamV3, "BeamV3")
                            {
                                ExtraLaunchParameters = ""
                            },
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.CuckooCycle, "CuckooCycle")
                            {
                                ExtraLaunchParameters = ""
                            },
                            //new Algorithm(MinerBaseType.lolMiner, AlgorithmType.GrinCuckatoo31, "GrinCuckatoo31")
                            //{
                            //    ExtraLaunchParameters = ""
                            //},
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                            {
                                ExtraLaunchParameters = "--enablezilcache=1"
                            },
                            //new Algorithm(MinerBaseType.lolMiner, AlgorithmType.GrinCuckatoo32, "GrinCuckatoo32")
                            //{
                            //    ExtraLaunchParameters = ""
                            //},
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.Autolykos, "Autolykos")
                            {
                                ExtraLaunchParameters = ""
                            },
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.KHeavyHash, "KHeavyHash")
                            {
                                ExtraLaunchParameters = ""
                            },
                            new DualAlgorithm(MinerBaseType.lolMiner, AlgorithmType.DaggerHashimoto, AlgorithmType.KHeavyHash, "DaggerKHeavyHash")
                            {
                            },
                            new DualAlgorithm(MinerBaseType.lolMiner, AlgorithmType.ETCHash, AlgorithmType.KHeavyHash, "ETCHashKHeavyHash")
                            {
                            }
                        }
            },
            {
                MinerBaseType.Claymore,
                            new List<Algorithm>
                            {
                                new Algorithm(MinerBaseType.Claymore, AlgorithmType.NeoScrypt, "NeoScrypt"),
                            }
            },
            {
                MinerBaseType.Phoenix,
                new List<Algorithm>()
                {
                    new Algorithm(MinerBaseType.Phoenix, AlgorithmType.DaggerHashimoto4GB, "DaggerHashimoto4GB")
                    {
    //                    ExtraLaunchParameters = "-rvram -1 "
                    }
                }
            },
            {
                 MinerBaseType.NBMiner,
                 new List<Algorithm>
                 {
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                            {
                                ExtraLaunchParameters = "--mt 1 "
                            },
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.ETCHash, "ETCHash")
                            {
                                ExtraLaunchParameters = "--mt 1 "
                            },
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.KAWPOW, "KAWPOW")
                            {
                                ExtraLaunchParameters = "--mt 1 "
                            },

                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.Autolykos, "Autolykos")
                            {
                                ExtraLaunchParameters = "--mt 1 "
                            }

                 }
            },
            {
                MinerBaseType.Nanominer,
                new List<Algorithm>()
                {
                    new Algorithm(MinerBaseType.Nanominer, AlgorithmType.Autolykos, "Autolykos")
                    {
                        ExtraLaunchParameters = "memTweak=1"
                    },
                    new DualAlgorithm(MinerBaseType.Nanominer, AlgorithmType.Autolykos, AlgorithmType.DaggerHashimoto, "Autolykos+Zilliqa")
                    {
                        ExtraLaunchParameters = "memTweak=1"
                    }
                }
            },
            { MinerBaseType.miniZ,
                        new List<Algorithm>() {
                            //new Algorithm(MinerBaseType.miniZ, AlgorithmType.BeamV3, "BeamV3")
                            //{
                            //},
                             new Algorithm(MinerBaseType.miniZ, AlgorithmType.ZHash, "ZHash")
                            {
                            },
                            new Algorithm(MinerBaseType.miniZ, AlgorithmType.ZelHash, "ZelHash")
                            {
                            },
                            new Algorithm(MinerBaseType.miniZ, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                            {
                            },
                        }
            },
        }.ConcatDictList(All, Gpu);

        #endregion

        #region NVIDIA

        public static Dictionary<MinerBaseType, List<Algorithm>> Nvidia => new Dictionary<MinerBaseType, List<Algorithm>>
        {
            {
                MinerBaseType.Phoenix,
                new List<Algorithm>()
                {
                    new Algorithm(MinerBaseType.Phoenix, AlgorithmType.DaggerHashimoto3GB, "DaggerHashimoto3GB")
                    {
    //                    ExtraLaunchParameters = "-rvram -1 "
                    }
                }
            },

            { MinerBaseType.CryptoDredge,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.NeoScrypt, "NeoScrypt"),
                            new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.KAWPOW, "KAWPOW")
                        }
            },
            { MinerBaseType.trex,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.trex, AlgorithmType.Octopus, "Octopus"),
                            new Algorithm(MinerBaseType.trex, AlgorithmType.KAWPOW, "KAWPOW")
                            {
                                ExtraLaunchParameters = ""
                            },
                            new Algorithm(MinerBaseType.trex, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                            {
                                ExtraLaunchParameters = "--mt 1"
                            },
                            new Algorithm(MinerBaseType.trex, AlgorithmType.ETCHash, "ETCHash")
                            {
                                ExtraLaunchParameters = "--mt 1"
                            },
                            new Algorithm(MinerBaseType.trex, AlgorithmType.X16RV2, "X16RV2")
                            {
                                ExtraLaunchParameters = ""
                            },
                            /*
                            new DualAlgorithm(MinerBaseType.trex, AlgorithmType.DaggerHashimoto, AlgorithmType.Autolykos, "DaggerAutolykos")
                            {
                                ExtraLaunchParameters = "--mt 1"
                            },
                            new DualAlgorithm(MinerBaseType.trex, AlgorithmType.DaggerHashimoto, AlgorithmType.KAWPOW, "DaggerKAWPOW")
                            {
                                ExtraLaunchParameters = "--mt 1"
                            },
                            new DualAlgorithm(MinerBaseType.trex, AlgorithmType.DaggerHashimoto, AlgorithmType.Octopus, "DaggerOctopus")

                            {
                                ExtraLaunchParameters = "--mt 1"
                            },
                            */
                            new Algorithm(MinerBaseType.trex, AlgorithmType.Autolykos, "Autolykos")
                            {
                                ExtraLaunchParameters = "--mt 1"
                            }
                        }
            },
            
             //тоже, extranonce2 не поддерживает
             /*
            { MinerBaseType.ZEnemy,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.ZEnemy, AlgorithmType.KAWPOW, "KAWPOW")
                            {
                            },
                        }
        
            },
    */
            { MinerBaseType.miniZ,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.miniZ, AlgorithmType.BeamV3, "BeamV3")
                            {
                            },
                             new Algorithm(MinerBaseType.miniZ, AlgorithmType.ZHash, "ZHash")
                            {
                            },
                            new Algorithm(MinerBaseType.miniZ, AlgorithmType.ZelHash, "ZelHash")
                            {
                            },
                            new Algorithm(MinerBaseType.miniZ, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                            {
                            },
                            new Algorithm(MinerBaseType.miniZ, AlgorithmType.Octopus, "Octopus")
                            {
                            }
                        }
            },
            { MinerBaseType.NBMiner,
                        new List<Algorithm>() {
                            //new Algorithm(MinerBaseType.NBMiner, AlgorithmType.CuckooCycle, "CuckooCycle"),//майнер не поддерживает extranonce найса
                            //new Algorithm(MinerBaseType.NBMiner, AlgorithmType.GrinCuckatoo32, "GrinCuckatoo32"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.Octopus, "Octopus"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.KAWPOW, "KAWPOW"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.ETCHash, "ETCHash"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.BeamV3, "BeamV3"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.Autolykos, "Autolykos"),
                        }
            },
            {
            MinerBaseType.GMiner,
                new List<Algorithm>
                {
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto"),
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.ETCHash, "ETCHash"),
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.ZHash, "ZHash")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    },
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.BeamV3, "BeamV3")
                    {
                    },
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.CuckooCycle, "CuckooCycle")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    },
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.GrinCuckatoo32, "GrinCuckatoo32")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    },

                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.KAWPOW, "KAWPOW")
                    {
                    },
                    /*
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.GrinCuckatoo32, "GrinCuckatoo32")
                    {
                    },
                    */
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.ZelHash, "ZelHash")
                    {
                    },
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.KHeavyHash, "KHeavyHash")
                    {
                    },
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.Autolykos, "Autolykos")
                    {
                        ExtraLaunchParameters = "--mt 1"
                    },
                    new DualAlgorithm(MinerBaseType.GMiner, AlgorithmType.Autolykos, AlgorithmType.KHeavyHash, AlgorithmType.AutolykosKHeavyHash.ToString())
                    {
                        ExtraLaunchParameters = "--mt 1"
                    },
                    new DualAlgorithm(MinerBaseType.GMiner, AlgorithmType.DaggerHashimoto, AlgorithmType.KHeavyHash, AlgorithmType.DaggerKHeavyHash.ToString())
                    {
                    },
                    new DualAlgorithm(MinerBaseType.GMiner, AlgorithmType.ETCHash, AlgorithmType.KHeavyHash, AlgorithmType.ETCHashKHeavyHash.ToString())
                    {
                    }
                    //triple
                    /*
                    new DualAlgorithm(MinerBaseType.GMiner, AlgorithmType.Autolykos, AlgorithmType.KHeavyHash, AlgorithmType.AutolykosKHeavyHashZil.ToString())
                    {
                        ExtraLaunchParameters = "--mt 1"
                    }
                    */
                }
            },
            {
                MinerBaseType.lolMiner,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                            {
                                ExtraLaunchParameters = "--enablezilcache=1"
                            },
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.Autolykos, "Autolykos")
                            {
                                ExtraLaunchParameters = ""
                            },
                            
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.KHeavyHash, "KHeavyHash")
                            {
                                ExtraLaunchParameters = ""
                            },
                            new DualAlgorithm(MinerBaseType.lolMiner, AlgorithmType.DaggerHashimoto, AlgorithmType.KHeavyHash, AlgorithmType.DaggerKHeavyHash.ToString())
                            {
                            },
                        }
            },
        }.ConcatDictList(All, Gpu);

        #endregion
    }
}
