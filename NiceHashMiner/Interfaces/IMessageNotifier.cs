/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
namespace NiceHashMiner.Interfaces
{
    /// <summary>
    /// IMessageNotifier interface is for message setting.
    /// </summary>
    public interface IMessageNotifier
    {
        void SetMessage(string infoMsg);
        void SetMessageAndIncrementStep(string infoMsg);
        void SetValueAndMsg(int num, string infoMsg);
    }
}
