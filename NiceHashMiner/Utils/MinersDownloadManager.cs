/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
namespace NiceHashMiner.Utils
{
    public static class MinersDownloadManager
    {
        public static DownloadSetup StandardDlSetup = new DownloadSetup(
             "https://github.com/angelbbs/NiceHashMinerLegacy/releases/download/Fork_Fix_25/bin_ff25.zip",
            "bins.zip",
            "bin");

        public static DownloadSetup ThirdPartyDlSetup = new DownloadSetup(
            "https://github.com/angelbbs/NiceHashMinerLegacy/releases/download/Fork_Fix_25/bin_3rdparty_ff25.zip",
            "bins_3rdparty.zip",
            "bin_3rdparty");
    }
}
