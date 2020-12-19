namespace NiceHashMiner
{
    public static class Links
    {
        public const string VisitUrl = "https://www.nicehash.com";

        // add version
        public const string VisitUrlNew = "https://github.com/angelbbs/NiceHashMinerLegacy/releases/";

        // add btc adress as parameter
        public const string CheckStats = "https://old.nicehash.com/index.jsp?p=miners&addr=";
        public const string CheckStatsNew = "https://nicehash.com/my/miner/";

        // help and faq
        public const string NhmHelp = "https://github.com/angelbbs/NiceHashMinerLegacy/";
        public const string NhmNoDevHelp = "https://github.com/nicehash/NiceHashMinerLegacy/wiki/Troubleshooting#nosupportdev";

        // faq
        public const string NhmBtcWalletFaq = "https://old.nicehash.com/help/how-to-create-the-bitcoin-addresswallet";
        public const string NhmBtcWalletFaqNew = "https://www.nicehash.com/support";
        public const string NhmPayingFaq = "https://old.nicehash.com/help/when-and-how-do-you-get-paid";
        public const string NhmPayingFaqNew = "https://www.nicehash.com/support/mining-help/earnings-and-payments/when-and-how-do-you-get-paid";

        // API
        // btc adress as parameter
        public const string NhmApiStats = "https://api.nicehash.com/api?method=stats.provider&addr=";
        public const string NhmApiInfo = "https://api.nicehash.com/api?method=simplemultialgo.info";
        public const string NhmApiVersion = "https://api.nicehash.com/nicehashminer?method=version&legacy";
        //public static string NHM_API_stats_provider_workers = "https://api.nicehash.com/api?method=stats.provider.workers&addr=";

        // device profits
        public const string NhmProfitCheck = "https://api.nicehash.com/p=calc&name=";

        // SMA Socket
        public const string NhmSocketAddress = "wss://nhmws.nicehash.com/v3/nhml";
        public const string NhmSocketAddressFailover = "wss://nhmws.nicehash.net/v3/nhml";
        //public const string NhmSocketAddress = "https://nhmws.nicehash.com/v3/nhml";
        public const string NhmSocketAddress_new = "https://nhmws-new.nicehash.com/v3/nhml";
        public static string NhmSocketAddress_old = "wss://api.nicehash.com/v2/nhm";
    }
}
