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
            return Enum.GetName(typeof(AlgorithmType), type);
            /*
            if ((AlgorithmType.INVALID <= type && type <= AlgorithmType.Octopus) ||
                (AlgorithmType.DaggerHashimoto4GB <= type && type <= AlgorithmType.DaggerPascal))
            {
                return Enum.GetName(typeof(AlgorithmType), type);
            }
            return "NameNotFound type not supported";
            */
        }
    }
}
