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
                    new Algorithm(MinerBaseType.Nanominer, AlgorithmType.DaggerHashimoto, "")
                    {
                        ExtraLaunchParameters = ""
                    }
                }
            },
            {
                MinerBaseType.Phoenix,
                new List<Algorithm>()
                {
                    new Algorithm(MinerBaseType.Phoenix, AlgorithmType.DaggerHashimoto, "")
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
                    //new Algorithm(MinerBaseType.Xmrig, AlgorithmType.CryptoNight, ""),
                    //new Algorithm(MinerBaseType.Xmrig, AlgorithmType.CryptoNightV7, ""),
                //    new Algorithm(MinerBaseType.Xmrig, AlgorithmType.CryptoNightV8, ""),
                   // new Algorithm(MinerBaseType.Xmrig, AlgorithmType.CryptoNightR, ""),
                    new Algorithm(MinerBaseType.Xmrig, AlgorithmType.RandomX, ""),
                  //  new Algorithm(MinerBaseType.Xmrig, AlgorithmType.CryptoNightHeavy, "")
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
            },
            {
                MinerBaseType.cpuminer,
                new List<Algorithm>
                {
             //       new Algorithm(MinerBaseType.cpuminer, AlgorithmType.Lyra2z, "lyra2z")
                }
            }
        }.ConcatDict(All);

        #endregion

        #region AMD

        private const string RemDis = " --remove-disabled";
        private const string DefaultParam = RemDis + AmdGpuDevice.DefaultParam;

        public static Dictionary<MinerBaseType, List<Algorithm>> Amd => new Dictionary<MinerBaseType, List<Algorithm>>
        {
            {
                MinerBaseType.CastXMR,
                        new List<Algorithm>() {
                            //new Algorithm(MinerBaseType.CastXMR, AlgorithmType.CryptoNightV7, "cryptonightV7") { },
                     //       new Algorithm(MinerBaseType.CastXMR, AlgorithmType.CryptoNightV8, "cryptonightV8") { },
                       //     new Algorithm(MinerBaseType.CastXMR, AlgorithmType.CryptoNightHeavy, "cryptonightHeavy") { }
                        }
            },
            /*
            {
                MinerBaseType.lyclMiner,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.lyclMiner, AlgorithmType.Lyra2REv3, "Lyra2REv3") { }
                        }
            },
            */
            /*
            {
                MinerBaseType.XmrigAMD,
                        new List<Algorithm>() {
                           // new Algorithm(MinerBaseType.XmrigAMD, AlgorithmType.CryptoNightV7, "CryptoNightV7") { },
                       //     new Algorithm(MinerBaseType.XmrigAMD, AlgorithmType.CryptoNightV8, "CryptoNightV8") { },
                         //   new Algorithm(MinerBaseType.XmrigAMD, AlgorithmType.CryptoNightHeavy, "CryptoNightHeavy") { },
                            new Algorithm(MinerBaseType.XmrigAMD, AlgorithmType.RandomX, "RandomX") { },
                            new Algorithm(MinerBaseType.XmrigAMD, AlgorithmType.CryptoNightR, "CryptoNightR") { }
                        }
            },
            */
            {
                MinerBaseType.SRBMiner,
                        new List<Algorithm>() {
                            
                            new Algorithm(MinerBaseType.SRBMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                            {
                                ExtraLaunchParameters = "--gpu-boost 3"
                            }
                            
                            /*
                            new Algorithm(MinerBaseType.SRBMiner, AlgorithmType.Eaglesong, "Eaglesong")
                            {
                                ExtraLaunchParameters = "--enablegpurampup --cgputhreads 2 "
                            }

                            new Algorithm(MinerBaseType.SRBMiner, AlgorithmType.CryptoNightHeavy, "CryptoNightHeavy")
                            {
                                ExtraLaunchParameters = "--enablegpurampup --cgputhreads 2 "
                            }
                            */
                        }
            },
            {
                MinerBaseType.GMiner,
                    new List<Algorithm>
                    {
                 //   new Algorithm(MinerBaseType.GMiner, AlgorithmType.Beam, "")
                   // {
                                //ExtraLaunchParameters = "--pec 1 "
                   // },
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.ZHash, "")
                        {
                        },
                        /*
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.CuckooCycle, "")
                        {
                        },
                        */
                        /*
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.Cuckaroo29BFC, "")
                        {
                        },
                        */
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.DaggerHashimoto, "")
                        {
                        }
                    }
            },
            {
                MinerBaseType.teamredminer,
                        new List<Algorithm>() {
                            //new Algorithm(MinerBaseType.teamredminer, AlgorithmType.X16RV2, "X16Rv2"),
                            new Algorithm(MinerBaseType.teamredminer, AlgorithmType.DaggerHashimoto, "DaggerHashimoto"),
                            //new Algorithm(MinerBaseType.teamredminer, AlgorithmType.GrinCuckarood29, "GrinCuckarood29"),
                            new Algorithm(MinerBaseType.teamredminer, AlgorithmType.KAWPOW, "KAWPOW")
                          //  new Algorithm(MinerBaseType.teamredminer, AlgorithmType.Lyra2REv3, "Lyra2REv3")
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
                                new Algorithm(MinerBaseType.Claymore, AlgorithmType.NeoScrypt, "neoscrypt"),
                                new Algorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto4GB, "DaggerHashimoto4GB")
                            }
            },
                        {
                MinerBaseType.Phoenix,
                new List<Algorithm>()
                {
                    new Algorithm(MinerBaseType.Phoenix, AlgorithmType.DaggerHashimoto4GB, "")
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
                            /*
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.Octopus, "Octopus")
                            {

                            },
                            */
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
            /*
            {
                MinerBaseType.Phoenix,
                new List<Algorithm>()
                {
                    new Algorithm(MinerBaseType.Phoenix, AlgorithmType.DaggerHashimoto3GB, "DaggerHashimoto3GB")

                }
            },
            */
            /*
            { MinerBaseType.hsrneoscrypt,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.hsrneoscrypt, AlgorithmType.NeoScrypt, "Neoscrypt"),
                        }
            },
            */
            { MinerBaseType.CryptoDredge,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.NeoScrypt, "NeoScrypt"),
                            //new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.X16RV2, "X16Rv2"),
                            new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.KAWPOW, "KAWPOW"),
                            //new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.Lyra2REv3, "Lyra2REv3"),
                          // new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.CuckooCycle, "CuckooCycle"),
                        }
            },
            { MinerBaseType.trex,
                        new List<Algorithm>() {
                            //new Algorithm(MinerBaseType.trex, AlgorithmType.X16RV2, "X16Rv2"),
                            new Algorithm(MinerBaseType.trex, AlgorithmType.Octopus, "Octopus"),
                            new Algorithm(MinerBaseType.trex, AlgorithmType.KAWPOW, "KAWPOW")
                            {
                                ExtraLaunchParameters = ""
                            },
                            new Algorithm(MinerBaseType.trex, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                            {
                                ExtraLaunchParameters = ""
                            },
                        }
            },

            { MinerBaseType.ZEnemy,
                        new List<Algorithm>() {
                            //new Algorithm(MinerBaseType.ZEnemy, AlgorithmType.X16RV2, "X16Rv2"),
                            new Algorithm(MinerBaseType.ZEnemy, AlgorithmType.KAWPOW, "KAWPOW"),
                        }
            },
            /*
            { MinerBaseType.XmrigNVIDIA,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.XmrigNVIDIA, AlgorithmType.CryptoNightR, "CryptoNightR") { },
                            new Algorithm(MinerBaseType.XmrigNVIDIA, AlgorithmType.RandomX, "RandomX") { }
                        }
            },
            */
            /*
            { MinerBaseType.TTMiner,
                        new List<Algorithm>() {
                           // new Algorithm(MinerBaseType.TTMiner, AlgorithmType.Lyra2REv3, "Lyra2REv3"),
                       //     new Algorithm(MinerBaseType.TTMiner, AlgorithmType.MTP, "MTP"),
                        }
            },
            */
            { MinerBaseType.miniZ,
                        new List<Algorithm>() {
                            /*
                            new Algorithm(MinerBaseType.miniZ, AlgorithmType.Beam, "Beam")
                            {
                                //ExtraLaunchParameters = "--mode=3 --extra "
                            },
                            */
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
            /*
            { MinerBaseType.Kawpowminer,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.Kawpowminer, AlgorithmType.KAWPOW, "KAWPOW")
                            {
                                 //  ExtraLaunchParameters = " "
                            },
                        }
            },
            */
            { MinerBaseType.NBMiner,
                        new List<Algorithm>() {
                            //new Algorithm(MinerBaseType.NBMiner, AlgorithmType.GrinCuckarood29, "GrinCuckarood29"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.GrinCuckatoo31, "GrinCuckatoo31"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.GrinCuckatoo32, "GrinCuckatoo32"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.CuckooCycle, "CuckooCycle"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.Octopus, "Octopus"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.KAWPOW, "KAWPOW"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.BeamV3, "BeamV3"),
                            /*
                            new DualAlgorithm(MinerBaseType.NBMiner, AlgorithmType.DaggerHashimoto, AlgorithmType.Eaglesong)
                            {
                                ExtraLaunchParameters = "--di 100"
                            },
                            */
                            /*
                            new DualAlgorithm(MinerBaseType.NBMiner, AlgorithmType.DaggerHashimoto, AlgorithmType.Handshake)
                            {
                                ExtraLaunchParameters = "--di 2"
                            }
                            */
                        }
            },
            /*
            { MinerBaseType.Bminer,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.Bminer, AlgorithmType.GrinCuckatoo32, "GrinCuckatoo32"),
                        }
            },
            */
            {
            MinerBaseType.GMiner,
                new List<Algorithm>
                {
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.DaggerHashimoto, ""),
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.ZHash, "")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    },
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.BeamV3, "BeamV3")
                    {
                    },
                    /*
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.GrinCuckarood29, "")
                    {
                    },
                    */
                    /*
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.CuckaRooz29, "")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    },
                    */
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.CuckooCycle, "")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    },

                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.KAWPOW, "")
                    {
                    },
                    /*
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.Cuckaroo29BFC, "")
                    {
                    },
                    */
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.GrinCuckatoo31, "")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    },
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.GrinCuckatoo32, "")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    }
                }
            },
        }.ConcatDictList(All, Gpu);

        #endregion
    }
}
