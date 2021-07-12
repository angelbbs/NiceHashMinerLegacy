using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NiceHashMiner.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.Extensions;

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
            /*
            {
                MinerBaseType.Claymore,
                new List<Algorithm>()
                {
                    new Algorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, ""),
                    //new Algorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto3GB, "DaggerHashimoto3GB")
//                    new DualAlgorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, AlgorithmType.Decred),
             //       new DualAlgorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, AlgorithmType.Lbry),
//                    new DualAlgorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, AlgorithmType.Pascal),
//                    new DualAlgorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, AlgorithmType.Sia),
              //      new DualAlgorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, AlgorithmType.Blake2s),
               //     new DualAlgorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, AlgorithmType.Keccak)
                }
            },
            */
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
                                ExtraLaunchParameters = "--gpu-boost 3"
                            },
                            new Algorithm(MinerBaseType.SRBMiner, AlgorithmType.Autolykos, "Autolykos")
                            {
                                ExtraLaunchParameters = "--gpu-boost 3"
                            },
                            new DualAlgorithm(MinerBaseType.SRBMiner, AlgorithmType.Autolykos, AlgorithmType.DaggerHashimoto, "Autolykos+Zilliqa")
                            {
                                ExtraLaunchParameters = "--gpu-boost 3"
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
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
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
                        }
            },
            {
                MinerBaseType.lolMiner,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.ZHash, "ZHash")
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
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.GrinCuckatoo31, "GrinCuckatoo31")
                            {
                                ExtraLaunchParameters = ""
                            },
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                            {
                                ExtraLaunchParameters = "--enablezilcache=1"
                            },
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.GrinCuckatoo32, "GrinCuckatoo32")
                            {
                                ExtraLaunchParameters = ""
                            }
                        }
            },
            {
                MinerBaseType.Claymore,
                            new List<Algorithm>
                            {
                                new Algorithm(MinerBaseType.Claymore, AlgorithmType.NeoScrypt, "NeoScrypt"),
                                new Algorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto4GB, "DaggerHashimoto4GB")
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

                            },
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.KAWPOW, "KAWPOW")
                            {

                            },
                            
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.Autolykos, "Autolykos")
                            {

                            },
                            
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
        }.ConcatDictList(All, Gpu);

        #endregion

        #region NVIDIA

        public static Dictionary<MinerBaseType, List<Algorithm>> Nvidia => new Dictionary<MinerBaseType, List<Algorithm>>
        {
            {
                MinerBaseType.Claymore,
                new List<Algorithm>()
                {
                    new Algorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto3GB, "DaggerHashimoto3GB")

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
                                ExtraLaunchParameters = ""
                            },
                            new Algorithm(MinerBaseType.trex, AlgorithmType.Autolykos, "Autolykos")
                            {
                                ExtraLaunchParameters = "--mt 1"
                            }
                        }
            },

            { MinerBaseType.ZEnemy,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.ZEnemy, AlgorithmType.KAWPOW, "KAWPOW"),
                        }
            },

            { MinerBaseType.miniZ,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.miniZ, AlgorithmType.BeamV3, "BeamV3")
                            {
                            },
                             new Algorithm(MinerBaseType.miniZ, AlgorithmType.ZHash, "ZHash")
                            {
                            },
                            new Algorithm(MinerBaseType.miniZ, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                            {
                            },
                        }
            },
            { MinerBaseType.NBMiner,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.GrinCuckatoo31, "GrinCuckatoo31"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.GrinCuckatoo32, "GrinCuckatoo32"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.CuckooCycle, "CuckooCycle"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.Octopus, "Octopus"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.KAWPOW, "KAWPOW"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.BeamV3, "BeamV3"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.Autolykos, "Autolykos"),
                        }
            },
            {
            MinerBaseType.GMiner,
                new List<Algorithm>
                {
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto"),
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

                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.KAWPOW, "KAWPOW")
                    {
                    }
                }
            },
        }.ConcatDictList(All, Gpu);

        #endregion
    }
}
