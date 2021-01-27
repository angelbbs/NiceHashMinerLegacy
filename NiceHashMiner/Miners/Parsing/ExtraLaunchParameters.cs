using NiceHashMiner.Configs.ConfigJsonFile;
using System;
using System.Collections.Generic;
using NiceHashMinerLegacy.Common.Enums;

namespace NiceHashMiner.Miners.Parsing
{
    public class MinerOptionPackageFile : ConfigFile<MinerOptionPackage>
    {
        public MinerOptionPackageFile(string name)
            : base(Folders.Internals, $"{name}.json", $"{name}.json")
        { }
    }

    public static class ExtraLaunchParameters
    {
        private static readonly List<MinerOptionPackage> Defaults = new List<MinerOptionPackage>
        {
            new MinerOptionPackage(
                MinerType.ccminer,
                new List<MinerOption>
                {
                    new MinerOption("Intensity", "-i", "--intensity=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ccminer_CUDA_Schedule", "--cuda-schedule=", "0", MinerOptionFlagType.Uni)
                },
                new List<MinerOption>()
            ),
                new MinerOptionPackage(
                MinerType.hsrneoscrypt,
                new List<MinerOption>() {
                    new MinerOption("Intensity", "-i", "--intensity=", "0", MinerOptionFlagType.MultiParam, ",")
                },
                new List<MinerOption>()
            ),
                new MinerOptionPackage(
                MinerType.CryptoDredge,
                new List<MinerOption>() {
                    new MinerOption("Intensity", "-i", "--intensity=", "6", MinerOptionFlagType.MultiParam, ",")
                },
                new List<MinerOption>()
            ),
                new MinerOptionPackage(
                MinerType.trex,
                new List<MinerOption>() {
                    new MinerOption("Intensity", "-i", "--intensity", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_mt", "--mt", "--mt", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("LogPath", "-l", "--log-path", "-1", MinerOptionFlagType.SingleParam),
                    new MinerOption("trex-no-watchdog", "--no-watchdog", "--no-watchdog", "", MinerOptionFlagType.SingleParam)
                },
                new List<MinerOption>()
            ),
                new MinerOptionPackage(
                MinerType.NBMiner,
                new List<MinerOption>() {
                    new MinerOption("nbminer_intensity", "--cuckoo-intensity", "0", MinerOptionFlagType.SingleParam),
                    new MinerOption("nbminer_Intensity", "-i", "--intensity", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("nbminer_memorytweak", "--mt", "--memory-tweak", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("nbminer_DIntensity", "--di", "--secondary-intensity", "-1", MinerOptionFlagType.MultiParam, ","),
                     new MinerOption("nbminer_oc1", "--oc1", "--oc1", null, MinerOptionFlagType.SingleParam, "")
                },
                new List<MinerOption>()

            ),
                new MinerOptionPackage(
                MinerType.miniZ,
                new List<MinerOption>() {
                    new MinerOption("miniZ_logfile", "--log-file=", "--log-file=", "", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("miniZ_Intensity", "--intensity=", "--intensity=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_nocolor", "--nocolor", "--nocolor", null, MinerOptionFlagType.Uni),
                    new MinerOption("miniZ_f11", "--f11=", "--f11=", "0", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("miniZ_extra", "--extra", "--extra", null, MinerOptionFlagType.Uni),
                    new MinerOption("miniZ_oc1", "--oc1", "--oc1", null, MinerOptionFlagType.Uni, ""),
                    new MinerOption("miniZ_oc2", "--oc2", "--oc2", null, MinerOptionFlagType.Uni, ""),
                    new MinerOption("miniZ_ocX", "--ocX", "--ocX", null, MinerOptionFlagType.Uni, ""),
                    new MinerOption("miniZ_mod", "--mode=", "--mode=", "0", MinerOptionFlagType.MultiParam, ",")
                },
                new List<MinerOption>()

            ),
                new MinerOptionPackage(
                MinerType.Kawpowminer,
                new List<MinerOption>() {
                    new MinerOption("Kawpowminer_nocolor", "--nocolor", "--nocolor", null, MinerOptionFlagType.Uni)
                },
                new List<MinerOption>()

            ),
                new MinerOptionPackage(
                MinerType.TTMiner,
                new List<MinerOption>() {
                    new MinerOption("TTMiner_logpool", "-logpool", "-logpool", null, MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("TTMiner_Intensity", "-i", "-intensity", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TTMiner_grid", "-ig", "-gs", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TTMiner_nocolor", "-nocolor", "-nocolor", null, MinerOptionFlagType.SingleParam, "")
                },
                new List<MinerOption>()

            ),
                new MinerOptionPackage(
                MinerType.ZEnemy,
                new List<MinerOption>() {
                    new MinerOption("Intensity", "-i", "--intensity=", "0", MinerOptionFlagType.MultiParam, ",")
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.ccminer_CryptoNight,
                new List<MinerOption>
                {
                    new MinerOption("Launch", "-l", "--launch=", "8x20", MinerOptionFlagType.MultiParam, ","), // default is 8x20
                    new MinerOption("Bfactor", "", "--bfactor=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Bsleep", "", "--bsleep=", "0", MinerOptionFlagType.MultiParam, ",") // TODO check default
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.ethminer_OCL,
                new List<MinerOption>
                {
                    new MinerOption("LocalWork", "", "--cl-local-work", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("GlobalWork", "", "--cl-global-work", "0", MinerOptionFlagType.MultiParam, ","),
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.ethminer_CUDA,
                new List<MinerOption>
                {
                    new MinerOption("CudaBlockSize", "", "--cuda-block-size", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("CudaGridSize", "", "--cuda-grid-size", "0", MinerOptionFlagType.MultiParam, ","),
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.mkxminer,
                new List<MinerOption>() {
                    // SingleParam
                    new MinerOption("ExitSick", "", "--exitsick", null, MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("Asm", "", "--asm", null, MinerOptionFlagType.SingleParam, ""),
                    // MultiParam TODO IMPORTANT check defaults
                    new MinerOption("GPUclock" , "", "--engine", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Memclock" , "", "--memclock", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Powertune", "", "--powertune", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("GPUvoltage", "--vddc", "--vddc", "-1", MinerOptionFlagType.MultiParam, ","), // default none
                    new MinerOption("GPUIntensity", "-I", "--intensity", "-1", MinerOptionFlagType.MultiParam, ",") // default none
                },
                // TemperatureOptions
                new List<MinerOption>() {
                    new MinerOption("GpuFan", "", "--fan", "30-95", MinerOptionFlagType.MultiParam, ","), // default none
                    new MinerOption("TempCutoff", "", "--temp-cutoff", "95", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TempOverheat", "", "--temp-overheat", "85", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TempTarget", "", "--temp-target", "75", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("AutoFan", "", "--auto-fan", null, MinerOptionFlagType.Uni, ""),
                    new MinerOption("AutoGpu", "", "--auto-gpu", null, MinerOptionFlagType.Uni, "")
                }
            ),
            new MinerOptionPackage(
                MinerType.teamredminer,
                new List<MinerOption>() {
                    // SingleParam
                    new MinerOption("Platform", "", "--platform=", null, MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("TRMintensity", "", "--cn_config=", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMtemp_limit", "", "--temp_limit=", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMtemp_resume", "", "--temp_resume=", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMeth_4g_max_alloc", "", "--eth_4g_max_alloc=", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMeth_4g_alloc_adjust", "", "--eth_4g_alloc_adjust=", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMeth_alloc_epoch", "", "--eth_alloc_epoch=", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMeth_aggr_mode", "", "--eth_aggr_mode", "", MinerOptionFlagType.Uni, ""),
                    new MinerOption("TRMeth_eth_config", "", "----eth_config", "B1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMforce_colors", "", "--force_colors", "", MinerOptionFlagType.Uni, "")
                },
                // TemperatureOptions
                new List<MinerOption>() {
                }
            ),
            new MinerOptionPackage(
                MinerType.lolMiner,
                new List<MinerOption>() {
                    // SingleParam
                    new MinerOption("lolMiner_log", "", "--logs", "0", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("lolMiner_log", "", "--logs", "0", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("lolMiner_enablezilcache", "--enablezilcache=", "--enablezilcache=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_win4galloc", "", "--win4galloc", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_4gallocsize", "", "--4g-alloc-size", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_keepfree", "", "--keepfree", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_zombie-tune", "", "--zombie-tune", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMinerasm", "", "--asm", "0", MinerOptionFlagType.SingleParam, "")
                },
                // TemperatureOptions
                new List<MinerOption>() {
                }
            ),
            new MinerOptionPackage(
                MinerType.lolMinerBEAM,
                new List<MinerOption>() {
                    // SingleParam
                    new MinerOption("lolMinerBEAM_log", "", "--logs", "0", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("lolMinerBEAMasm", "", "--asm", "0", MinerOptionFlagType.SingleParam, "")
                },
                // TemperatureOptions
                new List<MinerOption>() {
                }
            ),
            new MinerOptionPackage(
                MinerType.cpuminer_opt,
                new List<MinerOption>
                {
                    new MinerOption("Threads", "-t", "--threads=", "-1", MinerOptionFlagType.MultiParam, ","), // default none
                    new MinerOption("CpuAffinity", "", "--cpu-affinity", "-1", MinerOptionFlagType.MultiParam, ","), // default none
                    new MinerOption("CpuPriority", "", "--cpu-priority", "-1", MinerOptionFlagType.MultiParam, ",") // default
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.nheqminer_CPU,
                new List<MinerOption>
                {
                    new MinerOption("Threads", "-t", "-t", "-1", MinerOptionFlagType.SingleParam, " "), // default none
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.nheqminer_CUDA,
                new List<MinerOption>
                {
                    new MinerOption("Solver_Version", "-cv", "-cv", "0", MinerOptionFlagType.SingleParam, " "), // default 0
                    new MinerOption("Solver_Block", "-cb", "-cb", "0", MinerOptionFlagType.MultiParam, " "), // default 0
                    new MinerOption("Solver_Thread", "-ct", "-ct", "0", MinerOptionFlagType.MultiParam, " "), // default 0
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.eqm_CUDA,
                new List<MinerOption>
                {
                    new MinerOption("Solver_Mode", "-cm", "-cm", "0", MinerOptionFlagType.MultiParam, " "), // default 0
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.ClaymoreZcash,
                new List<MinerOption>
                {
                    new MinerOption("ClaymoreZcash_a", "-a", "-a", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreZcash_asm", "-asm", "-asm", "1", MinerOptionFlagType.MultiParam, ","),

                    new MinerOption("ClaymoreZcash_i", "-i", "-i", "6", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreZcash_wd", "-wd", "-wd", "1", MinerOptionFlagType.SingleParam, ","),
                    //new MinerOption(ClaymoreZcash_r      , , , , MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreZcash_nofee", "-nofee", "-nofee", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreZcash_li", "-li", "-li", "0", MinerOptionFlagType.MultiParam, ","),

                    //MinerOptionFlagType.MultiParam might not work corectly due to ADL indexing so use single param to apply to all
                    new MinerOption("ClaymoreZcash_cclock", "-cclock", "-cclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreZcash_mclock", "-mclock", "-mclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreZcash_powlim", "-powlim", "-powlim", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreZcash_cvddc", "-cvddc", "-cvddc", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreZcash_mvddc", "-mvddc", "-mvddc", "0", MinerOptionFlagType.MultiParam, ","),
                },
                new List<MinerOption>
                {
                    // temperature stuff
                    //MinerOptionFlagType.MultiParam might not work corectly due to ADL indexing so use single param to apply to all
                    new MinerOption("ClaymoreZcash_tt", "-tt", "-tt", "1", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreZcash_ttli", "-ttli", "-ttli", "70", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreZcash_tstop", "-tstop", "-tstop", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreZcash_fanmax", "-fanmax", "-fanmax", "100", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreZcash_fanmin", "-fanmin", "-fanmin", "0", MinerOptionFlagType.MultiParam, ","),
                }
            ),
            new MinerOptionPackage(
                MinerType.ClaymoreNeoscrypt,
                new List<MinerOption>() {
                    new MinerOption("ClaymoreNeoscrypt_a"      , "-a", "-a", "1", MinerOptionFlagType.MultiParam, ","),

                    new MinerOption("ClaymoreNeoscrypt_wd"     , "-wd", "-wd", "1", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreNeoscrypt_nofee"  , "-nofee", "-nofee", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreNeoscrypt_li"     , "-li", "-li", "0", MinerOptionFlagType.MultiParam, ","),

                    //MinerOptionFlagType.MultiParam might not work corectly due to ADL indexing so use single param to apply to all
                    new MinerOption("ClaymoreNeoscrypt_cclock" , "-cclock", "-cclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreNeoscrypt_mclock" , "-mclock", "-mclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreNeoscrypt_powlim" , "-powlim", "-powlim", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreNeoscrypt_cvddc"  , "-cvddc", "-cvddc", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreNeoscrypt_mvddc"  , "-mvddc", "-mvddc", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreNeoscrypt_colors"  , "-colors", "-colors", "1", MinerOptionFlagType.MultiParam, ","),
                },
                new List<MinerOption>() {
                    // temperature stuff
                    //MinerOptionFlagType.MultiParam might not work corectly due to ADL indexing so use single param to apply to all
                    new MinerOption("ClaymoreNeoscrypt_tt"     , "-tt", "-tt", "1", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreNeoscrypt_ttli"   , "-ttli", "-ttli", "70", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreNeoscrypt_tstop"  , "-tstop", "-tstop", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreNeoscrypt_fanmax" , "-fanmax", "-fanmax", "100", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreNeoscrypt_fanmin" , "-fanmin", "-fanmin", "0", MinerOptionFlagType.MultiParam, ","),
                }
            ),
            new MinerOptionPackage(
                MinerType.ClaymoreCryptoNight,
                new List<MinerOption>
                {
                    new MinerOption("ClaymoreCryptoNight_a", "-a", "-a", "0", MinerOptionFlagType.MultiParam),
                    new MinerOption("ClaymoreCryptoNight_wd", "-wd", "-wd", "1", MinerOptionFlagType.SingleParam, ","),
                    //new MinerOption(ClaymoreCryptoNight_r      , , , , MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreCryptoNight_V7"  , "-pow7", "-pow7", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreCryptoNight_nofee", "-nofee", "-nofee", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreCryptoNight_li", "-li", "-li", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreCryptoNight_h", "-h", "-h", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreCryptoNight_allpools", "-allpools", "0", MinerOptionFlagType.SingleParam),
                    new MinerOption("ClaymoreCryptoNight_cclock", "-cclock", "-cclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreCryptoNight_mclock", "-mclock", "-mclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreCryptoNight_powlim", "-powlim", "-powlim", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreCryptoNight_cvddc", "-cvddc", "-cvddc", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreCryptoNight_mvddc", "-mvddc", "-mvddc", "0", MinerOptionFlagType.MultiParam, ","),
                },
                new List<MinerOption>
                {
                    // temperature stuff
                    //MinerOptionFlagType.MultiParam might not work corectly due to ADL indexing so use single param to apply to all
                    new MinerOption("ClaymoreCryptoNight_tt", "-tt", "-tt", "1", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreCryptoNight_tstop", "-tstop", "-tstop", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreCryptoNight_fanmax", "-fanmax", "-fanmax", "100", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreCryptoNight_fanmin", "-fanmin", "-fanmin", "0", MinerOptionFlagType.MultiParam, ","),
                }
                ),

            new MinerOptionPackage(
                MinerType.XmrigAMD,
                new List<MinerOption>() {
                    new MinerOption("Xmrig_fee", "--donate-level=", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("Xmrig_variant", "--variant=", null, MinerOptionFlagType.Uni, " "),
                    new MinerOption("Xmrig_keepalive", "--keepalive", null, MinerOptionFlagType.Uni, " "),
                    new MinerOption("Xmrig_affinity", "--opencl-affinity=", null, MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Xmrig-intensity", "--opencl-launch=", null, MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Xmrig_nocolor", "--no-color", null, MinerOptionFlagType.Uni, " ")
                },
                new List<MinerOption>(){ }
             ),



            new MinerOptionPackage(
                MinerType.XmrigNVIDIA,
                new List<MinerOption>() {
                    new MinerOption("XmrigN_fee", "--donate-level=", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("XmrigN_variant", "--variant=", null, MinerOptionFlagType.Uni, " "),
                    new MinerOption("XmrigN_keepalive", "--keepalive", null, MinerOptionFlagType.Uni, " "),
                    new MinerOption("XmrigN_affinity", "--opencl-affinity=", null, MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("XmrigN-intensity", "--opencl-launch=", null, MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("XmrigN_nocolor", "--no-color", null, MinerOptionFlagType.Uni, " ")
                },
                new List<MinerOption>(){ }
             ),

            new MinerOptionPackage(
                MinerType.WildRig,
                new List<MinerOption>() {
                    new MinerOption("WildRig_fee", "--donate-level=", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("WildRig_opencl-threads", "--opencl-threads", null, MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("WildRig_good-risers", "--good-risers", null, MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("WildRig_affinity", "--opencl-affinity", null, MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("WildRig-intensity", "--opencl-launch", null, MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("WildRig_nocolor", "--no-color", null, MinerOptionFlagType.SingleParam, " ")
                },
                new List<MinerOption>(){ }
             ),
                new MinerOptionPackage(
                MinerType.SRBMiner,
                new List<MinerOption>() {
                    new MinerOption("SRBMiner-cpu-threads", "--cpu-threads", "--cpu-threads", null, MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("SRBMiner-ramp-up", "--enable-workers-ramp-up", "--enable-workers-ramp-up", null, MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("SRBMiner-randomx-use-tweaks", "--randomx-use-tweaks", "--randomx-use-tweaks", null, MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("SRBMiner-randomx-use-1gb-pages", "--randomx-use-1gb-pages", "--randomx-use-1gb-pages", null, MinerOptionFlagType.Uni, " "),
                    new MinerOption("SRBMiner-intensity", "--gpu-intensity","--gpu-intensity" , "23", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-gpu-raw-intensity", "--gpu-raw-intensity","--gpu-raw-intensity" , "-1", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-threads", "--gpu-threads", "--gpu-threads", "-1", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-worksize", "--gpu-worksize", "--gpu-worksize", "0", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-auto-intensity", "--gpu-auto-intensity", "--gpu-auto-intensity", "0", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-gpu-boost", "--gpu-boost", "--gpu-boost", "3", MinerOptionFlagType.MultiParam, "!")
                },
                new List<MinerOption>(){ }
             ),
            new MinerOptionPackage(
                MinerType.OptiminerZcash,
                new List<MinerOption>
                {
                    new MinerOption("ForceGenericKernel", "--force-generic-kernel", "--force-generic-kernel", null,
                        MinerOptionFlagType.Uni),
                    new MinerOption("ExperimentalKernel", "--experimental-kernel", "--experimental-kernel", null, MinerOptionFlagType.Uni),
                    new MinerOption("nodevfee", "--nodevfee", "--nodevfee", null, MinerOptionFlagType.Uni),
                    new MinerOption("Intensity", "-i", "--intensity", "0", MinerOptionFlagType.DuplicateMultiParam),
                    new MinerOption("PciMode", "--pci-mode", "--pci-mode", "", MinerOptionFlagType.SingleParam),
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.excavator,
                new List<MinerOption>
                {
                    // parameters differ according to algorithm
                    new MinerOption("parameter1", "-c1", "-c1", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("parameter2", "-c2", "-c2", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("debug_param", "-f", "-f", "", MinerOptionFlagType.SingleParam, " "),
                    // Overclocking not supported from NHM
                    //new MinerOption("Overclocking_os", "-os", "-os", "", MinerOptionFlagType.MultiParam, " "),
                    //new MinerOption("Overclocking_od", "-od", "-od", "2", MinerOptionFlagType.MultiParam, " "),
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.ClaymoreDual,
                new List<MinerOption>
                {
                    new MinerOption("ClaymoreDual_etha", "-etha", "-etha", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreDual_ethi", "-ethi", "-ethi", "8", MinerOptionFlagType.MultiParam, ","),

                    new MinerOption("ClaymoreDual_eres", "-eres", "-eres", "2", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreDual_erate", "-erate", "-erate", "1", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_estale", "-estale", "-estale", "1", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_gser", "-gser", "-gser", "0", MinerOptionFlagType.SingleParam, ","),

                    new MinerOption("ClaymoreDual_wd", "-wd", "-wd", "1", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_retrydelay", "-retrydelay", "-retrydelay", "20", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_nofee", "-nofee", "-nofee", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_li", "-li", "-li", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreDual_lidag", "-lidag", "-lidag", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreDual_logfile", "-logfile", "-logfile", "", MinerOptionFlagType.SingleParam, ""),

                    //MinerOptionFlagType.MultiParam might not work corectly due to ADL indexing so use single param to apply to all
                    new MinerOption("ClaymoreDual_cclock", "-cclock", "-cclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreDual_mclock", "-mclock", "-mclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreDual_powlim", "-powlim", "-powlim", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreDual_cvddc", "-cvddc", "-cvddc", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreDual_mvddc", "-mvddc", "-mvddc", "0", MinerOptionFlagType.MultiParam, ","),

                    // other and dual mining features
                    new MinerOption("ClaymoreDual_etht", "-etht", "-etht", "200", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_allcoins", "-allcoins", "-allcoins", "", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_r", "-r", "-r", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_mode", "-mode", "-mode", "0", MinerOptionFlagType.SingleParam,
                        ","), // this is not well supported in MultiParam

                    new MinerOption("ClaymoreDual_ftime", "-ftime", "-ftime", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_dpool", "-dpool", "-dpool", "", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_dwal", "-dwal", "-dwal", "", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_dpsw", "-dpsw", "-dpsw", "", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_dcoin", "-dcoin", "-dcoin", "", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_dcri", "-dcri", "-dcri", "30", MinerOptionFlagType.MultiParam, ","),

                    new MinerOption("ClaymoreDual_dcrt", "-dcrt", "-dcrt", "5", MinerOptionFlagType.SingleParam, ","),
                    // v9.0
                    new MinerOption("ClaymoreDual_asm", "-asm", "-asm", "1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreDual_strap", "-strap", "-strap", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreDual_sintensity", "-sintensity", "-sintensity", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreDual_rxboost", "-rxboost", "-rxboost", "0", MinerOptionFlagType.MultiParam, ","),
                     new MinerOption("ClaymoreDual_showpower", "-showpower", "-showpower", "1", MinerOptionFlagType.SingleParam, ","),
                },
                new List<MinerOption>
                {
                    // temperature stuff
                    //MinerOptionFlagType.MultiParam might not work corectly due to ADL indexing so use single param to apply to all
                    new MinerOption("ClaymoreDual_tt", "-tt", "-tt", "1", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_ttdcr", "-ttdcr", "-ttdcr", "1", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_ttli", "-ttli", "-ttli", "70", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_tstop", "-tstop", "-tstop", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("ClaymoreDual_fanmax", "-fanmax", "-fanmax", "100", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("ClaymoreDual_fanmin", "-fanmin", "-fanmin", "0", MinerOptionFlagType.MultiParam, ","),
                }
            ),
            new MinerOptionPackage(
                MinerType.Phoenix,
                new List<MinerOption>
                {
                    new MinerOption("Phoenix_stales", "-stales", "-stales", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_ftimeout", "-ftimeout", "-ftimeout", "", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_AMD", "-amd", "-amd", "", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_NVIDIA", "-nvidia", "-nvidia", "", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_acm", "-acm", "-acm", "", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_mi", "-mi", "-mi", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_gt", "-gt", "-gt", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_clKernel", "-clKernel", "-clKernel", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_clNew", "-clNew", "-clNew", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_clf", "-clf", "-clf", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_nvNew", "-nvNew", "-nvNew", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_eres", "-eres", "-eres", "2", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_dagrestart", "-dagrestart", "-dagrestart", "0", MinerOptionFlagType.SingleParam),
                    new MinerOption("Phoenix_rvram", "-rvram", "-rvram", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_nvf", "-nvf", "-nvf", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_mt", "-mt", "-mt", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_straps", "-straps", "-straps", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_vmt1", "-vmt1", "-vmt1", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_vmt2", "-vmt2", "-vmt2", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_vmt3", "-vmt3", "-vmt3", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_vmr", "-vmr", "-vmr", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_nvmem", "-nvmem", "-nvmem", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_fanmax", "-fanmax", "-fanmax", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_fanmin", "-fanmin", "-fanmin", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_tmax", "-tmax", "-tmax", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_powlim", "-powlim", "-powlim", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_cclock", "-cclock", "-cclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_cvddc", "-cvddc", "-cvddc", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_mclock", "-mclock", "-mclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_mvddc", "-mvddc", "-mvddc", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_daglim", "-daglim", "-daglim", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_rxboost", "-rxboost", "-rxboost", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_minrigspeed", "-minrigspeed", "-minrigspeed", "0", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("Phoenix_gsi", "-gsi", "-gsi", "", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("Phoenix_gswin", "-gswin", "-gswin", "", MinerOptionFlagType.SingleParam, ""),
                },
                new List<MinerOption>
                {
                    // temperature stuff
                 }
            ),
            new MinerOptionPackage(
                MinerType.EWBF,
                new List<MinerOption>
                {
                    // parameters differ according to algorithm
                    new MinerOption("EWBF_fee", "--fee", "--fee", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("EWBF_templimit", "--templimit", "--templimit", "90", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("EWBF_tempunits", "--tempunits", "--tempunits", "C", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("EWBF_eexit", "--eexit", "--eexit", null, MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("EWBF_solver", "--solver", "--solver", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("EWBF_intensity", "--intensity", "--intensity", "64", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("EWBF_powercalc", "--pec", "--pec", null, MinerOptionFlagType.Uni, " "),
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.Nanominer,
                new List<MinerOption>
                {
                    new MinerOption("Nanominer_memTweak", "memTweak=", "1", MinerOptionFlagType.MultiParam, ","),
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.GMiner,
                new List<MinerOption>
                {
                    // parameters differ according to algorithm
                    new MinerOption("GMiner_logfile", "-l", "--logfile", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_templimit", "-t", "--templimit", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_watchdog", "-w", "--watchdog", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_color", "-c", "--color", "1", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_pec", "--pec", "--pec", "1", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_Intensity", "-i", "--intensity", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_DualIntensity", "-di", "--dual_intensity", "-1", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_OC", "--oc", "--oc", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_oc1", "--oc1", "--oc1", null, MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("GMiner_electricity_cost", "--electricity_cost", "--electricity_cost", "0", MinerOptionFlagType.SingleParam, " "),
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.Bminer,
                new List<MinerOption>
                {
                    // parameters differ according to algorithm
                    new MinerOption("Bminer_logfile", "-logfile", "-logfile", "", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("Bminer_max-temperature", "-max-temperature", "-max-temperature", "85", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("Bminer_share-check", "-share-check", "-share-check", "900", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("Bminer_gpucheck", "-gpucheck", "-gpucheck", "90", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("Bminer_intensity", "-intensity", "-intensity", "-1", MinerOptionFlagType.MultiParam, ","),
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.Xmrig,
                new List<MinerOption>
                {
                    new MinerOption("Xmrig_fee", "--donate-level=", "0", MinerOptionFlagType.SingleParam),
                    new MinerOption("Xmrig_threads", "-t", "--threads=", null, MinerOptionFlagType.SingleParam),
                    new MinerOption("Xmrig_av", "-v", "--av=", "0", MinerOptionFlagType.SingleParam),
                    new MinerOption("Xmrig_priority", "--cpu-priority=", null, MinerOptionFlagType.SingleParam),
                    new MinerOption("Xmrig_nohugepages", "--no-huge-pages", null, MinerOptionFlagType.Uni),
                    new MinerOption("Xmrig_maxusage", "--max-cpu-usage=", "75", MinerOptionFlagType.SingleParam),
                    new MinerOption("Xmrig_safe", "--safe", null, MinerOptionFlagType.Uni),
                    new MinerOption("Xmrig_fee", "--donate-level=", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("Xmrig_variant", "--variant=", null, MinerOptionFlagType.Uni, " "),
                    new MinerOption("Xmrig_keepalive", "--keepalive", null, MinerOptionFlagType.Uni, " "),
                    new MinerOption("Xmrig_affinity", "--opencl-affinity=", null, MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Xmrig-intensity", "--opencl-launch=", null, MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Xmrig_nocolor", "--no-color", null, MinerOptionFlagType.Uni, " ")
                },
                new List<MinerOption>()
            ),
             new MinerOptionPackage(
                MinerType.CastXMR,
                new List<MinerOption>
                {
                    new MinerOption("OpenCLPlatform", "-O", "--opencl", "0", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("ForceCompute", "--forcecompute", "--forcecompute", "", MinerOptionFlagType.Uni, ""), // default none
                    new MinerOption("FastJobSwitch", "--fastjobswitch", "--fastjobswitch", "", MinerOptionFlagType.Uni, ""), // default none
                    new MinerOption("Maxmem", "--maxmem", "--maxmem", "", MinerOptionFlagType.Uni, ""), // default none
                    new MinerOption("Log", "--log", "--log", "", MinerOptionFlagType.Uni, ""), // default none
                    new MinerOption("Intensity", "--intensity", "--intensity", "0", MinerOptionFlagType.MultiParam, " "), //maybe not work
                    new MinerOption("RateWatchdog", "--ratewatchdog", "--ratewatchdog", "", MinerOptionFlagType.Uni, "") // default none
                },
                new List<MinerOption>()
             ),
             new MinerOptionPackage(
                MinerType.lyclMiner,
                new List<MinerOption>
                {
                    new MinerOption("Worksize", "worksize=", "worksize=", "1048576", MinerOptionFlagType.MultiParam, ","),
                },
                new List<MinerOption>()
                 ),
            new MinerOptionPackage(
                MinerType.dstm,
                new List<MinerOption>
                {
                    new MinerOption("dstm_time", "--time", null, MinerOptionFlagType.Uni),
                    new MinerOption("dstm_noreconnect", "--noreconnect", null, MinerOptionFlagType.Uni),
                    new MinerOption("dstm_temp-target", "--temp-target", null, MinerOptionFlagType.SingleParam),
                    new MinerOption("dstm_intensity", "--intensity=", null, MinerOptionFlagType.SingleParam)
                },
                new List<MinerOption>())
        };

        private static readonly List<MinerOptionPackage> MinerOptionPackages = new List<MinerOptionPackage>();

        public static void InitializePackages()
        {
            foreach (var pack in Defaults)
            {
                var packageName = $"MinerOptionPackage_{pack.Name}";
                var packageFile = new MinerOptionPackageFile(packageName);
                var readPack = packageFile.ReadFile();
                if (readPack == null)
                {
                    // read has failed
                    Helpers.ConsolePrint("ExtraLaunchParameters", "Creating internal params config " + packageName);
                    MinerOptionPackages.Add(pack);
                    // create defaults
                    packageFile.Commit(pack);
                }
                else
                {
                    Helpers.ConsolePrint("ExtraLaunchParameters", "Loading internal params config " + packageName);
                    MinerOptionPackages.Add(readPack);
                }
            }
            var defaultKeys = Defaults.ConvertAll(p => p.Type);
            // extra check if DEFAULTS is missing a key
            for (var type = (MinerType.NONE + 1); type < MinerType.END; ++type)
            {
                if (defaultKeys.Contains(type) == false)
                {
                    var packageName = $"MinerOptionPackage_{Enum.GetName(typeof(MinerType), type)}";
                    var packageFile = new MinerOptionPackageFile(packageName);
                    var readPack = packageFile.ReadFile();
                    if (readPack != null)
                    {
                        // read has failed
                        Helpers.ConsolePrint("ExtraLaunchParameters", "Creating internal params config " + packageName);
                        MinerOptionPackages.Add(readPack);
                    }
                }
            }
        }

        public static MinerOptionPackage GetMinerOptionPackageForMinerType(MinerType type)
        {
            var index = MinerOptionPackages.FindIndex(p => p.Type == type);
            if (index > -1)
            {
                return MinerOptionPackages[index];
            }
            // if none found
            return new MinerOptionPackage(MinerType.NONE, new List<MinerOption>(), new List<MinerOption>());
        }
    }
}
