using NiceHashMiner.Miners.Grouping;
using System;

namespace NiceHashMiner.Interfaces
{
    public interface IMainFormRatesComunication
    {
        void ClearRatesAll();

        void AddRateInfo(string groupName, string deviceStringInfo, ApiData iApiData, double paying, double power,
           DateTime StartMinerTime, bool isApiGetException, string ProcessTag, GroupMiner groupMiners, int groupCount);
        //void RaiseAlertSharesNotAccepted(string algoName);

        // The following four must use an invoker since they may be called from non-UI thread

        void ShowNotProfitable(string msg);

        void HideNotProfitable();

        void ForceMinerStatsUpdate();

        void ClearRates(int groupCount);
    }
}
