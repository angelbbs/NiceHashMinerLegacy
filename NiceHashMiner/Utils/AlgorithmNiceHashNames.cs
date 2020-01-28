/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using NiceHashMinerLegacy.Common.Enums;

namespace NiceHashMiner
{
    /// <summary>
    /// AlgorithmNiceHashNames class is just a data container for mapping NiceHash JSON API names to algo type
    /// </summary>
    public static class AlgorithmNiceHashNames
    {
        public static string GetName(AlgorithmType type)
        {
            if ((AlgorithmType.INVALID <= type && type <= AlgorithmType.GrinCuckatoo32) ||
                (AlgorithmType.DaggerEaglesong <= type && type <= AlgorithmType.DaggerPascal))
            {
                return Enum.GetName(typeof(AlgorithmType), type);
            }
            return "NameNotFound type not supported";
        }
    }
}
