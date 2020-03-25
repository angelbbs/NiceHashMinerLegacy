/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System.IO;

namespace NiceHashMiner.Utils
{
    public static class MinersExistanceChecker
    {
        public static bool IsMinersBins_ALL_Init()
        {
            foreach (var filePath in MinersBins.ALL_FILES_BINS)
            {
                if (!File.Exists($"miners{filePath}"))
                {
                    Helpers.ConsolePrint("MinersExistanceChecker", $"miners{filePath} doesn't exist! Warning");
                    return false;
                }
            }
            return true;
        }

        public static bool IsMinersBinsInit()
        {
            //return isOk;
            return IsMinersBins_ALL_Init();
        }
    }
}
