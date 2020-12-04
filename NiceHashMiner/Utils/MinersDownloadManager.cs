namespace NiceHashMiner.Utils
{
    public static class MinersDownloadManager
    {
        public static DownloadSetup MinersDownloadSetup = new DownloadSetup(
            "https://github.com/angelbbs/NiceHashMinerLegacy/releases/download/Fork_Fix_" +
            Form_Main.currentVersion.ToString().Trim() + "/miners.zip",
            "miners.zip",
            "miners");
    }
}
