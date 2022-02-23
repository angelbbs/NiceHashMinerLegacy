namespace NiceHashMiner
{
    public static class Links
    {
        public static string VisitUrl => CheckDNS("https://www.nicehash.com");
        public static string VisitUrlNew => CheckDNS("https://github.com/angelbbs/NiceHashMinerLegacy/releases/");
        public static string CheckStatsNew => CheckDNS("https://nicehash.com/my/miner/");
        public static string StatusNicehash => CheckDNS("https://status.nicehash.com/");
        public static string NhmHelp => CheckDNS("https://github.com/angelbbs/NiceHashMinerLegacy/");
        public static string NhmNoDevHelp => CheckDNS("https://github.com/nicehash/NiceHashMinerLegacy/wiki/Troubleshooting#nosupportdev");
        public static string NhmBtcWalletFaqNew => CheckDNS("https://www.nicehash.com/support");
        public static string NhmSocketAddress => CheckDNS("wss://nhmws.nicehash.com/v3/nhml");
        public static string NhmHashpower => CheckDNS("https://api2.nicehash.com/main/api/v2/hashpower/orderBook?algorithm=");
        public static string NhmSimplemultialgo => CheckDNS("https://api2.nicehash.com/main/api/v2/public/simplemultialgo/info");
        public static string NhmCurrent => CheckDNS("https://api2.nicehash.com/main/api/v2/public/stats/global/current");
        public static string Nhm24h => CheckDNS("https://api2.nicehash.com/main/api/v2/public/stats/global/24");
        public static string NhmExternal => CheckDNS("https://api2.nicehash.com/main/api/v2/mining/external/");
        public static string ApiUrl => CheckDNS("https://api.nicehash.com/api?method=nicehash.service.info");//?
        public static string exchangeRateList => CheckDNS("https://api2.nicehash.com/main/api/v2/exchangeRate/list/");
        public static string miningStats => CheckDNS("https://www.nicehash.com/my/mining/stats/");
        public static string githubReleases => CheckDNS("https://github.com/angelbbs/NiceHashMinerLegacy/releases");
        public static string githubLatestRelease => CheckDNS("https://api.github.com/repos/angelbbs/NiceHashMinerLegacy/releases/latest");
        public static string gitlabReleases => CheckDNS("https://gitlab.com/angelbbs/NiceHashMinerLegacy/-/releases");
        public static string gitlabRepositoryTags => CheckDNS("https://gitlab.com/api/v4/projects/26404146/repository/tags");
        public static string gitlabLastRelease => CheckDNS("https://gitlab.com/api/v4/projects/26404146/releases/");//?
        public static string githubDownload => CheckDNS("https://github.com/angelbbs/NiceHashMinerLegacy/releases/download/Fork_Fix_");

        //dns over https
        public static string CheckDNS(string domain)
        {
            Helpers.ConsolePrint("******", domain);
            return domain;
        }
    }

}
