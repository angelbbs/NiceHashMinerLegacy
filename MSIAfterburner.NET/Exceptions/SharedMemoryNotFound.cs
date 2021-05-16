// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.Exceptions.SharedMemoryNotFound
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;

namespace MSI.Afterburner.Exceptions
{
    public class SharedMemoryNotFound : CustomErrorException
    {
        private const string errMsg = "Could not connect to MSI Afterburner 2.1 or later.";

        public SharedMemoryNotFound()
          : base("Could not connect to MSI Afterburner 2.1 or later.")
        {
        }

        public SharedMemoryNotFound(Exception innerEx)
          : base("Could not connect to MSI Afterburner 2.1 or later.", innerEx)
        {
        }
    }
}
