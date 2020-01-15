/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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
                    new Algorithm(MinerBaseType.XmrStak, AlgorithmType.CryptoNightR, ""),
                  //  new Algorithm(MinerBaseType.XmrStak, AlgorithmType.CryptoNightV8, ""),

                }
            }
        };

        #endregion

        #region GPU

        private static Dictionary<MinerBaseType, List<Algorithm>> Gpu => new Dictionary<MinerBaseType, List<Algorithm>>
        {
            {
                MinerBaseType.Claymore,
                new List<Algorithm>()
                {
                    new Algorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, "")
//                    new DualAlgorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, AlgorithmType.Decred),
             //       new DualAlgorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, AlgorithmType.Lbry),
//                    new DualAlgorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, AlgorithmType.Pascal),
//                    new DualAlgorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, AlgorithmType.Sia),
              //      new DualAlgorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, AlgorithmType.Blake2s),
               //     new DualAlgorithm(MinerBaseType.Claymore, AlgorithmType.DaggerHashimoto, AlgorithmType.Keccak)
                }
            },
            {
                MinerBaseType.Phoenix,
                new List<Algorithm>()
                {
                    new Algorithm(MinerBaseType.Phoenix, AlgorithmType.DaggerHashimoto, "")
                    {
    //                    ExtraLaunchParameters = "-rvram -1 -eres 0 "
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
                    new Algorithm(MinerBaseType.Xmrig, AlgorithmType.CryptoNightR, ""),
                    new Algorithm(MinerBaseType.Xmrig, AlgorithmType.RandomX, ""),
                  //  new Algorithm(MinerBaseType.Xmrig, AlgorithmType.CryptoNightHeavy, "")
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
            {
                MinerBaseType.lyclMiner,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.lyclMiner, AlgorithmType.Lyra2REv3, "Lyra2REv3") { }
                        }
            },
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
            {
                MinerBaseType.SRBMiner,
                        new List<Algorithm>() {
                            /*
                            new Algorithm(MinerBaseType.SRBMiner, AlgorithmType.CryptoNightV8, "CryptoNightV8")
                            {
                                ExtraLaunchParameters = "--enablegpurampup --cgputhreads 2 "
                            },
                            */
                            new Algorithm(MinerBaseType.SRBMiner, AlgorithmType.CryptoNightR, "CryptoNightR")
                            {
                                ExtraLaunchParameters = "--enablegpurampup --cgputhreads 2 "
                            }
                            /*
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
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.BeamV2, "")
                        {
//                                ExtraLaunchParameters = "--asm 0 "
                        },
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.ZHash, "")
                        {
                        },
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.CuckooCycle, "")
                        {
                        },
                        new Algorithm(MinerBaseType.GMiner, AlgorithmType.GrinCuckaroo29, "")
                        {
                        }
                    }
            },
            {
                MinerBaseType.teamredminer,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.teamredminer, AlgorithmType.X16R, "X16R"),
                            new Algorithm(MinerBaseType.teamredminer, AlgorithmType.X16RV2, "X16Rv2"),
                            new Algorithm(MinerBaseType.teamredminer, AlgorithmType.DaggerHashimoto, "DaggerHashimoto"),
                        //    new Algorithm(MinerBaseType.teamredminer, AlgorithmType.MTP, "MTP"),
                            new Algorithm(MinerBaseType.teamredminer, AlgorithmType.GrinCuckarood29, "GrinCuckarood29"),
                            new Algorithm(MinerBaseType.teamredminer, AlgorithmType.Lyra2REv3, "Lyra2REv3"),
                         //   new Algorithm(MinerBaseType.teamredminer, AlgorithmType.CryptoNightV8, "CryptoNightV8"),
                            new Algorithm(MinerBaseType.teamredminer, AlgorithmType.CryptoNightR, "CryptoNightR")
                        }
            },
            {
                MinerBaseType.lolMiner,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.ZHash, "ZHash")
                            {
                                ExtraLaunchParameters = "--asm 1 "
                            },
                            //lolminer broken
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.BeamV2, "BeamV2")
                            {
                                ExtraLaunchParameters = "--asm 1 "
                            },
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.GrinCuckarood29, "GrinCuckarood29")
                            {
                                ExtraLaunchParameters = "--asm 1 "
                            },
                            new Algorithm(MinerBaseType.lolMiner, AlgorithmType.GrinCuckatoo31, "GrinCuckatoo31")
                            {
                                ExtraLaunchParameters = "--asm 1 "
                            }
                         //   new Algorithm(MinerBaseType.lolMiner, AlgorithmType.GrinCuckatoo31, "GrinCuckatoo31"),
                        }
            },
            {
                MinerBaseType.WildRig,
                        new List<Algorithm>() {
                            /*
                            new Algorithm(MinerBaseType.WildRig, AlgorithmType.Skunk, "Skunk")
                            {
                                ExtraLaunchParameters = "--opencl-threads 3 --opencl-launch 20x0 "
                            },
                            */
                            new Algorithm(MinerBaseType.WildRig, AlgorithmType.X16R, "X16R")
                            {
                                ExtraLaunchParameters = "--opencl-threads 2 --opencl-launch 18x0 "
                            },
                            new Algorithm(MinerBaseType.WildRig, AlgorithmType.X16RV2, "X16RV2")
                            {
                                ExtraLaunchParameters = "--opencl-threads 2 --opencl-launch 18x0 "
                            },
                            new Algorithm(MinerBaseType.WildRig, AlgorithmType.Lyra2REv3, "Lyra2REv3")
                            {
                                ExtraLaunchParameters = "--opencl-threads auto --opencl-launch auto "
                            }
                        }
            },
            {
                MinerBaseType.Claymore,
                            new List<Algorithm>
                            {
                    //new Algorithm(MinerBaseType.Claymore, AlgorithmType.CryptoNightV7, ""),
                                new Algorithm(MinerBaseType.Claymore, AlgorithmType.NeoScrypt, "neoscrypt")
                   // new Algorithm(MinerBaseType.Claymore, AlgorithmType.Equihash, "equihash")
                            }
            },
            {
                MinerBaseType.Prospector,
                new List<Algorithm>
                {
                  //  new Algorithm(MinerBaseType.Prospector, AlgorithmType.Skunk, "sigt"),
                    //new Algorithm(MinerBaseType.Prospector, AlgorithmType.Sia, "sia")
                }
            },
            { 
                 MinerBaseType.NBMiner,
                 new List<Algorithm>
                 {
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto")
                            {

                            },
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.Eaglesong, "Eaglesong")
                            {

                            },
                            new DualAlgorithm(MinerBaseType.NBMiner, AlgorithmType.DaggerHashimoto, AlgorithmType.Eaglesong)
                            {
                                ExtraLaunchParameters = "--di 0"
                            }
                 }
            }

        }.ConcatDictList(All, Gpu);

        #endregion

        #region NVIDIA

        public static Dictionary<MinerBaseType, List<Algorithm>> Nvidia => new Dictionary<MinerBaseType, List<Algorithm>>
        {
            {
                MinerBaseType.ccminer,
                new List<Algorithm>
                {
                    //new Algorithm(MinerBaseType.ccminer, AlgorithmType.NeoScrypt, "neoscrypt"),
                    //new Algorithm(MinerBaseType.ccminer, AlgorithmType.Lyra2REv2, "lyra2v2"),
                    //new Algorithm(MinerBaseType.ccminer, AlgorithmType.Decred, "decred"),
                    //new Algorithm(MinerBaseType.ccminer, AlgorithmType.Lbry, "lbry"),
                    //new Algorithm(MinerBaseType.ccminer, AlgorithmType.X11Gost, "sib"),
                    //new Algorithm(MinerBaseType.ccminer, AlgorithmType.Blake2s, "blake2s"),
                    //new Algorithm(MinerBaseType.ccminer, AlgorithmType.Sia, "sia"),
                    //new Algorithm(MinerBaseType.ccminer, AlgorithmType.Keccak, "keccak"),
                    //new Algorithm(MinerBaseType.ccminer, AlgorithmType.Skunk, "skunk"),
                    //new Algorithm(MinerBaseType.ccminer, AlgorithmType.Lyra2z, "lyra2z"),
                    //new Algorithm(MinerBaseType.ccminer, AlgorithmType.MTP, "MTP")
                    //{
                    //            ExtraLaunchParameters = "-i 20 "
                    //},
                }
            },
            /*
            {
                MinerBaseType.ccminer_alexis,
                new List<Algorithm>
                {
                    new Algorithm(MinerBaseType.ccminer_alexis, AlgorithmType.X11Gost, "sib"),
                    new Algorithm(MinerBaseType.ccminer_alexis, AlgorithmType.Keccak, "keccak")
                }
            },
            */
                        { MinerBaseType.hsrneoscrypt,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.hsrneoscrypt, AlgorithmType.NeoScrypt, "Neoscrypt"),
                        }
                    },
                        { MinerBaseType.CryptoDredge,
                        new List<Algorithm>() {
                        //    new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.Lyra2REv2, "Lyra2REv2"),
                        //    new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.Blake2s, "Blake2s"),
                            new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.NeoScrypt, "NeoScrypt"),
                          //  new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.Skunk, "Skunk"),
                            new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.X16R, "X16R"),
                            new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.X16RV2, "X16Rv2"),
                        //   new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.CryptoNightHeavy, "CryptoNightHeavy"),
                        //   new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.CryptoNightV7, "CryptoNightV7"),
                      //     new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.CryptoNightV8, "CryptoNightV8"),
                           new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.Lyra2REv3, "Lyra2REv3"),
                       //    new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.MTP, "MTP"),
                           new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.GrinCuckaroo29, "GrinCuckaroo29"),
                          // new Algorithm(MinerBaseType.CryptoDredge, AlgorithmType.CuckooCycle, "CuckooCycle"),
                        }
                    },

                        { MinerBaseType.trex,
                        new List<Algorithm>() {
                          //  new Algorithm(MinerBaseType.trex, AlgorithmType.Skunk, "Skunk"),
                         //   new Algorithm(MinerBaseType.trex, AlgorithmType.MTP, "MTP"),
                            new Algorithm(MinerBaseType.trex, AlgorithmType.X16R, "X16R"),
                            new Algorithm(MinerBaseType.trex, AlgorithmType.X16RV2, "X16Rv2"),
                        }
                    },

                        { MinerBaseType.ZEnemy,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.ZEnemy, AlgorithmType.X16R, "X16R"),
                            new Algorithm(MinerBaseType.ZEnemy, AlgorithmType.X16RV2, "X16Rv2"),
                        //    new Algorithm(MinerBaseType.ZEnemy, AlgorithmType.Skunk, "Skunk"),
                        }
                    },
                        
                    { MinerBaseType.XmrigNVIDIA,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.XmrigNVIDIA, AlgorithmType.CryptoNightR, "CryptoNightR") { },
                            new Algorithm(MinerBaseType.XmrigNVIDIA, AlgorithmType.RandomX, "RandomX") { }
                        }
                    },
                    
                        { MinerBaseType.TTMiner,
                        new List<Algorithm>() {
                           // new Algorithm(MinerBaseType.TTMiner, AlgorithmType.Lyra2REv3, "Lyra2REv3"),
                       //     new Algorithm(MinerBaseType.TTMiner, AlgorithmType.MTP, "MTP"),
                        }
                    },
                        { MinerBaseType.miniZ,
                        new List<Algorithm>() {
                            /*
                            new Algorithm(MinerBaseType.miniZ, AlgorithmType.Beam, "Beam")
                            {
                                //ExtraLaunchParameters = "--mode=3 --extra "
                            },
                            */
                            new Algorithm(MinerBaseType.miniZ, AlgorithmType.BeamV2, "BeamV2")
                            {
                                //ExtraLaunchParameters = "--mode=3 --extra "
                            },
                             new Algorithm(MinerBaseType.miniZ, AlgorithmType.ZHash, "ZHash")
                            {
                              //  ExtraLaunchParameters = "--mode=3 --extra "
                            },
                        }
                    },
                        { MinerBaseType.NBMiner,
                        new List<Algorithm>() {
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.GrinCuckaroo29, "GrinCuckaroo29"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.GrinCuckarood29, "GrinCuckarood29"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.GrinCuckatoo31, "GrinCuckatoo31"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.CuckooCycle, "CuckooCycle"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.Eaglesong, "Eaglesong"),
                            new Algorithm(MinerBaseType.NBMiner, AlgorithmType.DaggerHashimoto, "DaggerHashimoto"),
                            new DualAlgorithm(MinerBaseType.NBMiner, AlgorithmType.DaggerHashimoto, AlgorithmType.Eaglesong)
                            {
                                ExtraLaunchParameters = "--di 100"
                            }
                        }
                    },
        /*
            {
                MinerBaseType.ethminer,
                new List<Algorithm>
                {
                    new Algorithm(MinerBaseType.ethminer, AlgorithmType.DaggerHashimoto, "daggerhashimoto")
                }
            },
            */
            /*
            {
                MinerBaseType.nheqminer,
                new List<Algorithm>
                {
                    new Algorithm(MinerBaseType.nheqminer, AlgorithmType.Equihash, "equihash")
                }
            },
            */

            {
                MinerBaseType.EWBF,
                new List<Algorithm>
                {
                    new Algorithm(MinerBaseType.EWBF, AlgorithmType.ZHash, "")
                }
            },
            {
                MinerBaseType.GMiner,
                new List<Algorithm>
                {
                    //gminer broken
                  //  new Algorithm(MinerBaseType.GMiner, AlgorithmType.DaggerHashimoto, ""),
                    /*
                    new DualAlgorithm(MinerBaseType.GMiner, AlgorithmType.DaggerHashimoto, AlgorithmType.Eaglesong)
                    {
                        ExtraLaunchParameters = "--dual_intensity 100"
                    },
                    */
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.ZHash, "")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    },
                //    new Algorithm(MinerBaseType.GMiner, AlgorithmType.Beam, "")
                  //  {
                                //ExtraLaunchParameters = "--pec 1 "
                    //},
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.BeamV2, "")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    },
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.GrinCuckaroo29, "")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    },
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.GrinCuckarood29, "")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    },
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.Cuckaroom, "")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    },
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.CuckooCycle, "")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    },
                    /*
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.Eaglesong, "")
                    {
                    },
                    */
                    new Algorithm(MinerBaseType.GMiner, AlgorithmType.GrinCuckatoo31, "")
                    {
                                //ExtraLaunchParameters = "--pec 1 "
                    }
                }
            },
/*
            {
                MinerBaseType.Bminer,
                new List<Algorithm>
                {
                    new Algorithm(MinerBaseType.Bminer, AlgorithmType.ZHash, "")
                    {
                    },
                    new Algorithm(MinerBaseType.Bminer, AlgorithmType.Beam, "")
                    {
                    },
                    new Algorithm(MinerBaseType.Bminer, AlgorithmType.GrinCuckaroo29, "")
                    {
                    },
                    new Algorithm(MinerBaseType.Bminer, AlgorithmType.GrinCuckatoo31, "")
                    {
                    }
                }
            },
*/
/*
            {
                MinerBaseType.dstm,
                new List<Algorithm>
                {
                    new Algorithm(MinerBaseType.dstm, AlgorithmType.Equihash, "")
                }
            }
            */
        }.ConcatDictList(All, Gpu);

        #endregion
    }
}
