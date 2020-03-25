/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
namespace NiceHashMiner.Utils
{
    public static class MinersDownloadManager
    {
        public static DownloadSetup MinersDownloadSetup = new DownloadSetup(
            "https://github.com/angelbbs/NiceHashMinerLegacy/releases/download/Fork_Fix_25/miners.zip",
            "miners.zip",
            "miners");
    }
}
