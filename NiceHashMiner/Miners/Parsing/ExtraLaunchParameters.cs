using NiceHashMiner.Configs.ConfigJsonFile;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;

namespace NiceHashMiner.Miners.Parsing
{
    public class MinerOptionPackageFile : ConfigFile<MinerOptionPackage>
    {
        public MinerOptionPackageFile(string name)
            : base(Folders.Temp, $"{name}.json", $"{name}.json")
        { }
    }

    public static class ExtraLaunchParameters
    {
        private static readonly List<MinerOptionPackage> Defaults = new List<MinerOptionPackage>
        {
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
                    new MinerOption("trex_mt", "-mt", "--mt", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_pl", "--pl", "--pl", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_mclock", "--mclock", "--mclock", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_cclock", "--cclock", "--cclock", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_lock_cclock", "--lock-cclock", "--lock-cclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_lock_cv", "--lock-cv", "--lock-cv", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_pstate", "--pstate", "--pstate", "p0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_fan", "--fan", "--fan", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_fant", "--fan t:", "--fan t:", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_fantm", "--fan tm:", "--fan tm:", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_lhr_tune", "-lhr-tune", "--lhr-tune", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_lhr_autotune-step-size", "-lhr-autotune-step-size", "--lhr-autotune-step-size", "0.1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_lhr_autotune-interval", "-lhr-autotune-interval", "--lhr-autotune-interval", "20", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_lhr_dataset-mode", "-dataset-mode", "--dataset-mode", "2", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_lhr_autotune", "-lhr-autotune-mode", "--lhr-autotune-mode", null, MinerOptionFlagType.SingleParam),
                    new MinerOption("trex_lhr_autotune_interval", "-lhr-autotune-interval", "--lhr-autotune-interval", null, MinerOptionFlagType.SingleParam),
                    new MinerOption("trex_lhr-low-power", "-lhr-low-power", "--lhr-low-power", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("trex_dag-build-mode", "--dag-build-mode", "--dag-build-mode", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("LogPath", "-l", "--log-path", "-1", MinerOptionFlagType.SingleParam),
                    new MinerOption("trex_reconnect-on-fail-shares", "--reconnect-on-fail-shares", "--reconnect-on-fail-shares", "-1", MinerOptionFlagType.SingleParam),
                    new MinerOption("trex-no-watchdog", "--no-watchdog", "--no-watchdog", "", MinerOptionFlagType.Uni),
                    new MinerOption("trex-validate-shares", "--validate-shares", "--validate-shares", null, MinerOptionFlagType.Uni),
                    //new MinerOption("trex_reconnect-on-fail-shares", "--reconnect-on-fail-shares", "--reconnect-on-fail-shares", "", MinerOptionFlagType.Uni),
                    new MinerOption("trex-no-new-block-info", "--no-new-block-info", "--no-new-block-info", "", MinerOptionFlagType.Uni),
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
                    new MinerOption("nbminer_temperature-limit", "--tl", "--temperature-limit", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("nbminer_temperature-start", "--ts", "--temperature-start", "-1", MinerOptionFlagType.MultiParam, ","),
                    //new MinerOption("nbminer_lhr", "-lhr", "-lhr", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("nbminer_lhr-mode", "--lhr-mode", "--lhr-mode", "1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("nbminer_power-limit", "--power-limit", "--pl", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("nbminer_cclock", "--cclock", "--cclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("nbminer_mclock", "--mclock", "--mclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("nbminer_lock-cv", "--lock-cv", "--lock-cv", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("nbminer_fan", "--fan", "--fan", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("nbminer_log", "--log", "--log", null, MinerOptionFlagType.Uni),
                    new MinerOption("nbminer_log-file", "--log-file", "--log-file", "", MinerOptionFlagType.SingleParam),
                    new MinerOption("nbminer_log-no-job", "--log-no-job", "--log-no-job", null, MinerOptionFlagType.Uni),
                    new MinerOption("nbminer_log-cycle", "--log-cycle", "--log-cycle", "", MinerOptionFlagType.SingleParam),
                    new MinerOption("nbminer_lrv", "--lrv", "--lhr-reduce-value", "", MinerOptionFlagType.SingleParam),
                    new MinerOption("nbminer_lrt", "--lrt", "--lhr-reduce-time", "", MinerOptionFlagType.SingleParam),
                    new MinerOption("nbminer_lrl", "--lrl", "--lhr-reduce-limit", "", MinerOptionFlagType.SingleParam),
                     new MinerOption("nbminer_oc1", "--oc1", "--oc1", null, MinerOptionFlagType.SingleParam, "")
                },
                new List<MinerOption>()

            ),
                new MinerOptionPackage(
                MinerType.miniZ,
                new List<MinerOption>() {
                    new MinerOption("miniZ_logfile", "--log-file=", "--log-file=", "", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("miniZ_fanspeed-all", "--fanspeed-all=", "--fanspeed-all=", "", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("miniZ_fantemp-all", "--fantemp-all=", "--fantemp-all=", "", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("miniZ_fanmax", "--fanmax=", "--fanmax=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_fantemp", "--fantemp=", "--fantemp=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_gpuclock", "--gpuclock=", "--gpuclock=", "1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_memclock", "--memclock=", "--memclock=", "1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_gpuoffset", "--gpuoffset=", "--gpuoffset=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_coreoffset2", "--coreoffset2=", "--coreoffset2=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_memoffset", "--memoffset=", "--memoffset=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_memoffset2", "--memoffset2=", "--memoffset2=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_power", "-pl", "--power=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_power2", "-pl2", "--power2=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_Intensity", "--intensity=", "--intensity=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_zil-init", "--zil-init", "--zil-init", null, MinerOptionFlagType.Uni),
                    new MinerOption("miniZ_protocol-dump", "--protocol-dump", "--protocol-dump", null, MinerOptionFlagType.Uni),
                    new MinerOption("miniZ_nocolor", "--nocolor", "--nocolor", null, MinerOptionFlagType.Uni),
                    new MinerOption("miniZ_nocolour", "--nocolour", "--nocolour", null, MinerOptionFlagType.Uni),
                    new MinerOption("miniZ_no-ocx", "--no-ocx", "--no-ocx", null, MinerOptionFlagType.Uni),
                    new MinerOption("miniZ_powerf", "--powerf", "--powerf", null, MinerOptionFlagType.Uni),
                    new MinerOption("miniZ_profit", "--profit", "--profit", null, MinerOptionFlagType.Uni),
                    new MinerOption("miniZ_accprofit", "--accprofit", "--accprofit", null, MinerOptionFlagType.Uni),
                    new MinerOption("miniZ_f11", "--f11=", "--f11=", "0", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("miniZ_extra", "--extra", "--extra", null, MinerOptionFlagType.Uni),
                    new MinerOption("miniZ_oc1", "--oc1", "--oc1", null, MinerOptionFlagType.Uni, ""),
                    new MinerOption("miniZ_oc2", "--oc2", "--oc2", null, MinerOptionFlagType.Uni, ""),
                    new MinerOption("miniZ_ocX", "--ocX", "--ocX", null, MinerOptionFlagType.Uni, ""),
                    new MinerOption("miniZ_mod", "--mode=", "--mode=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_autoclocks", "--autoclocks=", "--autoclocks=", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("miniZ_stocksettings", "--stocksettings=", "--stocksettings=", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("miniZ_priority", "--priority=", "--priority=", "0", MinerOptionFlagType.MultiParam, ",")
                },
                new List<MinerOption>()

            ),
               
            new MinerOptionPackage(
                MinerType.teamredminer,
                new List<MinerOption>() {
                    // SingleParam
                    new MinerOption("Platform", "", "--platform=", null, MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("TRM_watchdog_script", "", "--watchdog_script=", null, MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("TRM_kas_start", "", "--kas_start", null, MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("TRM_kas_end", "", "--kas_end", null, MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("TRM_iron_start", "", "--iron_start", null, MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("TRM_iron_end", "", "--iron_end", null, MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("TRMintensity", "", "--cn_config=", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMtemp_limit", "", "--temp_limit=", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMtemp_resume", "", "--temp_resume=", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMeth_4g_max_alloc", "", "--eth_4g_max_alloc=", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMeth_4g_alloc_adjust", "", "--eth_4g_alloc_adjust=", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMeth_alloc_epoch", "", "--eth_alloc_epoch=", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMeth_aggr_mode", "", "--eth_aggr_mode", "", MinerOptionFlagType.Uni, ""),
                    new MinerOption("TRMeth_eth_config", "", "--eth_config", "B1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMprog_micro_tune", "", "--prog_micro_tune", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMeth_smooth_power", "", "--eth_smooth_power", "1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMclk_core_mh", "", "--clk_core_mhz=", "", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMclk_core_mv", "", "--clk_core_mv=", "", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMclk_mem_mhz", "", "--clk_mem_mhz=", "", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMclk_mem_mv", "", "--clk_mem_mv=", "", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMclk_timing", "", "--clk_timing=", "", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMfan_control", "", "--fan_control=", "", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMeth_dag_buf=", "", "--eth_dag_buf=", "", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("TRMforce_colors", "", "--force_colors", "", MinerOptionFlagType.Uni, ""),
                    new MinerOption("TRMforce_watchdog_disabled", "", "--watchdog_disabled", "", MinerOptionFlagType.Uni, "")
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
                    new MinerOption("lolMiner_dualfactor", "", "--dualfactor", "auto", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("lolMiner_dualmode", "", "--dualmode", "none", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("lolMiner_enablezilcache", "--enablezilcache=", "--enablezilcache=", "0", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("lolMiner_win4galloc", "", "--win4galloc", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_maxdualimpact", "", "--maxdualimpact", "*", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_4gallocsize", "", "--4g-alloc-size", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_keepfree", "", "--keepfree", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_zombie-tune", "", "--zombie-tune", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_lhrtune", "", "--lhrtune", "auto", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_cclk", "", "--cclk", "*", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_mclk", "", "--mclk", "*", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_coff", "", "--coff", "*", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_moff", "", "--moff", "*", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_pl", "", "--pl", "*", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_fan", "", "--fan", "*", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("lolMiner_ergo-prebuild", "--ergo-prebuild", "--ergo-prebuild", "0", MinerOptionFlagType.MultiParam, ","),
                    //new MinerOption("lolMinerasm", "", "--asm", "0", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("lolMinerbasecolor", "", "--basecolor", "", MinerOptionFlagType.Uni, ""),
                    new MinerOption("lolMinernocolor", "", "--nocolor", "", MinerOptionFlagType.Uni, ""),
                    new MinerOption("lolMinernocl", "", "--no-cl", "", MinerOptionFlagType.Uni, ""),
                    new MinerOption("lolMiner_lhrwait", "--lhrwait", "--lhrwait", "0", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("lolMiner_screen ", "--screen ", "--screen", "0", MinerOptionFlagType.SingleParam, ""),
                },
                // TemperatureOptions
                new List<MinerOption>() {
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
                MinerType.SRBMiner,
                new List<MinerOption>() {
                    new MinerOption("SRBMiner-log-file", "--log-file", "--log-file", null, MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("SRBMiner-cpu-threads", "--cpu-threads", "--cpu-threads", null, MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("SRBMiner-ramp-up", "--enable-workers-ramp-up", "--enable-workers-ramp-up", null, MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("SRBMiner-randomx-use-tweaks", "--randomx-use-tweaks", "--randomx-use-tweaks", null, MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("SRBMiner-a0-oc-script", "--a0-oc-script", "--a0-oc-script", null, MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("SRBMiner-gpu-cclock0", "--gpu-cclock0", "--gpu-cclock0", null, MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-gpu-mclock0", "--gpu-mclock0", "--gpu-mclock0", null, MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-gpu-coffset0", "--gpu-coffset0", "--gpu-coffset0", "0", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-gpu-moffset0", "--gpu-moffset0", "--gpu-moffset0", "0", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner--gpu-plimit0", "--gpu-plimit0", "--gpu-plimit0", "100", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-a1-oc-script", "--a1-oc-script", "--a1-oc-script", null, MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("SRBMiner-gpu-cclock1", "--gpu-cclock1", "--gpu-cclock1", null, MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-gpu-mclock1", "--gpu-mclock1", "--gpu-mclock1", null, MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-gpu-coffset1", "--gpu-coffset1", "--gpu-coffset1", "0", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-gpu-moffset1", "--gpu-moffset1", "--gpu-moffset1", "0", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner--gpu-plimit1", "--gpu-plimit1", "--gpu-plimit1", "100", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-oc-delay", "--oc-delay", "--oc-delay", null, MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("SRBMiner-zil-epoch", "--zil-epoch", "--zil-epoch", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("SRBMiner-zil-pool", "--zil-pool", "--zil-pool", null, MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-zil-wallet", "--zil-wallet", "--zil-wallet", null, MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-zil-password", "--zil-password", "--zil-password", null, MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-zil-esm", "--zil-esm", "--zil-esm", null, MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-zil-enable", "--zil-enable", "--zil-enable", null, MinerOptionFlagType.Uni),
                    new MinerOption("SRBMiner-zil-cclock", "--zil-cclock", "--zil-cclock", null, MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-zil-mclock", "--zil-mclock", "--zil-mclock", null, MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-zil-coffset", "--zil-coffset", "--zil-coffset", "0", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-zil-moffset", "--zil-moffset", "--zil-moffset", "0", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-zil-plimit", "--zil-plimit", "--zil-plimit", "100", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-gpu-disable-auto-buffer", "--gpu-disable-auto-buffer", "--gpu-disable-auto-buffer", null, MinerOptionFlagType.Uni),
                    new MinerOption("SRBMiner-gpu-buffer-mode", "--gpu-buffer-mode", "--gpu-buffer-mode", null, MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-randomx-use-1gb-pages", "--randomx-use-1gb-pages", "--randomx-use-1gb-pages", null, MinerOptionFlagType.Uni, " "),
                    new MinerOption("SRBMiner-intensity", "--gpu-intensity","--gpu-intensity" , "23", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-gpu-raw-intensity", "--gpu-raw-intensity","--gpu-raw-intensity" , "-1", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-threads", "--gpu-threads", "--gpu-threads", "-1", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-worksize", "--gpu-worksize", "--gpu-worksize", "0", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-auto-intensity", "--gpu-auto-intensity", "--gpu-auto-intensity", "0", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner-gpu-auto-tune", "--gpu-auto-tune", "--gpu-auto-tune", "0", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner_gpu_autolykos2_preload", "--gpu-autolykos2-preload", "--gpu-autolykos2-preload", "0", MinerOptionFlagType.MultiParam, "!"),
                    new MinerOption("SRBMiner_gpu_boost", "--gpu-boost", "--gpu-boost", "0", MinerOptionFlagType.MultiParam, "!")
                },
                new List<MinerOption>(){ }
             ),
            
            new MinerOptionPackage(
                MinerType.Phoenix,
                new List<MinerOption>
                {
                    new MinerOption("Phoenix_stales", "-stales", "-stales", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_mcdag", "-mcdag", "-mcdag", "0", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_ftimeout", "-ftimeout", "-ftimeout", "", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_AMD", "-amd", "-amd", "", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_NVIDIA", "-nvidia", "-nvidia", "", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_acm", "-acm", "-acm", "", MinerOptionFlagType.SingleParam, ","),
                    new MinerOption("Phoenix_mi", "-mi", "-mi", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_gt", "-gt", "-gt", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_clKernel", "-clKernel", "-clKernel", "1", MinerOptionFlagType.MultiParam, ","),
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
                    new MinerOption("Phoenix_tt", "-tt", "-tt", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_fanmax", "-fanmax", "-fanmax", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_fanmin", "-fanmin", "-fanmin", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_fanstop", "-fanstop", "-fanstop", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_tmax", "-tmax", "-tmax", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_ttj", "-ttj", "-ttj", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_ttmem", "-ttmem", "-ttmem", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_tmaxj", "-tmaxj", "-tmaxj", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_tmaxmem", "-tmaxmem", "-tmaxmem", "-1", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_powlim", "-powlim", "-powlim", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_cclock", "-cclock", "-cclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_cvddc", "-cvddc", "-cvddc", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_mclock", "-mclock", "-mclock", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_mvddc", "-mvddc", "-mvddc", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_daglim", "-daglim", "-daglim", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_rxboost", "-rxboost", "-rxboost", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_lhr", "-lhr", "-lhr", "0", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Phoenix_minrigspeed", "-minrigspeed", "-minrigspeed", "0", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("Phoenix_gsi", "-gsi", "-gsi", "", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("Phoenix_log", "-log", "-log", "", MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("Phoenix_gswin", "-gswin", "-gswin", "", MinerOptionFlagType.SingleParam, ""),
                },
                new List<MinerOption>
                {
                    // temperature stuff
                 }
            ),
            
            new MinerOptionPackage(
                MinerType.Nanominer,
                new List<MinerOption>
                {
                    new MinerOption("Nanominer_memTweak", "memTweak=", "memTweak=", "1", MinerOptionFlagType.NanoMiner, ","),
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.Rigel,
                new List<MinerOption>
                {
                    new MinerOption("Rigel-temp-limit-tc", "--temp-limit tc", "--temp-limit tc", "_", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Rigel-temp-limit-tm", "--temp-limit tm", "--temp-limit tm", "_", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Rigel-cpu-check", "--cpu-check", "--cpu-check", null, MinerOptionFlagType.Uni),
                    new MinerOption("Rigel-no-tui", "--no-tui", "--no-tui", null, MinerOptionFlagType.Uni),
                    new MinerOption("Rigel-no-colour", "--no-colour", "--no-colour", null, MinerOptionFlagType.Uni),
                    new MinerOption("Rigel---log-network", "--log-network", "--log-network", null, MinerOptionFlagType.Uni),
                    new MinerOption("Rigel-zil-countdown", "--zil-countdown", "--zil-countdown", null, MinerOptionFlagType.Uni),
                    new MinerOption("Rigel-dns-over-https", "--dns-over-https", "--dns-over-https", null, MinerOptionFlagType.Uni),
                    new MinerOption("Rigel-hashrate-avg", "--hashrate-avg", "--hashrate-avg", "10", MinerOptionFlagType.SingleParam),
                    new MinerOption("Rigel-log-file", "--log-file", "--log-file", "", MinerOptionFlagType.SingleParam),
                    new MinerOption("Rigel-dag-reset-mclock", "--dag-reset-mclock", "--dag-reset-mclock", "", MinerOptionFlagType.SingleParam),
                    new MinerOption("Rigel-cclock", "--cclock", "--cclock", "_", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Rigel-mclock", "--mclock", "--mclock", "_", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Rigel-lock-cclock", "--lock-cclock", "--lock-cclock", "_", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Rigel-lock-mclock", "--lock-mclock", "--lock-mclock", "_", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Rigel-pl", "--pl", "--pl", "_", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Rigel-mt", "--mt", "--mt", "_", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Rigel-zil", "--zil", "--zil", "_", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Rigel-zil", "--zil", "--zil", "_", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Rigel-zil-cache-dag", "--zil-cache-dag", "--zil-cache-dag", "_", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Rigel-dual-mode", "--dual-mode", "--dual-mode", "_", MinerOptionFlagType.MultiParam, ","),
                    new MinerOption("Rigel-fan-control", "--fan-control", "--fan-control", "_", MinerOptionFlagType.MultiParam, ","),
                },
                new List<MinerOption>()
            ),
            new MinerOptionPackage(
                MinerType.GMiner,
                new List<MinerOption>
                {
                    // parameters differ according to algorithm
                    new MinerOption("GMiner_logfile", "-l", "--logfile", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_log_newjob", "--log_newjob", "--log_newjob", "1", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_templimit", "-t", "--templimit", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_watchdog", "-w", "--watchdog", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_color", "-c", "--color", "1", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_pec", "--pec", "--pec", "1", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_Intensity", "-i", "--intensity", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_DualIntensity", "-di", "--dual_intensity", "-1", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_ZilDualIntensity", "-zildi", "--zildual_intensity", "-1", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_OC", "--oc", "--oc", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_oc1", "--oc1", "--oc1", null, MinerOptionFlagType.SingleParam, ""),
                    new MinerOption("GMiner_mt", "--mt", "--mt", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_zilmt", "--zilmt", "--zilmt", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_safe_dag", "--safe_dag", "--safe_dag", "1", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_tfan", "--tfan", "--tfan", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_tfan_min", "--tfan_min", "--tfan_min", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_tfan_max", "--tfan_max", "--tfan_max", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_lhr", "--lhr", "--lhr", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_lhr_tune", "--lhr_tune", "--lhr_tune", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_lhr_mode", "--lhr_mode", "--lhr_mode", "1", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_fan", "--fan", "--fan", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_pl", "--pl", "--pl", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_cclock", "--cclock", "--cclock", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_mclock", "--mclock", "--mclock", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_zilcclock", "--zilcclock", "--zilcclock", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_zilmclock", "--zilmclock", "--zilmclock", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_zilpl", "--zilpl", "--zilpl", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_zillock_cclock", "--zillock_cclock", "--zillock_cclock", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_zillock_mclock", "--zillock_mclock", "--zillock_mclock", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_lock_mclock", "--lock_mclock", "--lock_mclock", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_cvddc", "--cvddc", "--cvddc", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_zilcvddc", "--zilcvddc", "--zilcvddc", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_lock_voltage", "--lock_voltage", "--lock_voltage", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_zillock_voltage", "--zillock_voltage", "--zillock_voltage", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_lock_cclock", "--lock_cclock", "--lock_cclock", "0", MinerOptionFlagType.MultiParam, " "),
                    new MinerOption("GMiner_electricity_cost", "--electricity_cost", "--electricity_cost", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_lhr_autotune", "--lhr_autotune", "--lhr_autotune", "1", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_lhr_autotune_step", "--lhr_autotune_step", "--lhr_autotune_step", "0.5", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_watchdog_mode", "--watchdog_mode", "--watchdog_mode", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_min_rig_speed", "--min_rig_speed", "--min_rig_speed", "-1", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_dag_gen_limit", "--dag_gen_limit", "--dag_gen_limit", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_dataset_mode", "--dataset_mode", "--dataset_mode", "0", MinerOptionFlagType.SingleParam, " "),
                    new MinerOption("GMiner_log_pool_efficiency", "--log_pool_efficiency", "--log_pool_efficiency", "0", MinerOptionFlagType.SingleParam, " "),
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
            )
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
