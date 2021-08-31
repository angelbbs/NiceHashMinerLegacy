// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.Exceptions.MACMFanControlNotManual
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;

namespace MSI.Afterburner.Exceptions
{
    public class MACMFanControlNotManual : CustomErrorException
    {
        private const string errMsg = "Fan is currently set to auto.  Cannot set fan speed.";

        public MACMFanControlNotManual()
          : base("Fan is currently set to auto.  Cannot set fan speed.")
        {
        }

        public MACMFanControlNotManual(Exception innerEx)
          : base("Fan is currently set to auto.  Cannot set fan speed.", innerEx)
        {
        }
    }
}
