// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.Exceptions.MACMFeatureNotSupported
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;

namespace MSI.Afterburner.Exceptions
{
    public class MACMFeatureNotSupported : CustomErrorException
    {
        private const string errMsg = "You hardware does not support this feature.";

        public MACMFeatureNotSupported()
          : base("You hardware does not support this feature.")
        {
        }

        public MACMFeatureNotSupported(Exception innerEx)
          : base("You hardware does not support this feature.", innerEx)
        {
        }

        public MACMFeatureNotSupported(string errorMessage)
          : base(errorMessage)
        {
        }
    }
}
