// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.Exceptions.SharedMemoryDead
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;

namespace MSI.Afterburner.Exceptions
{
    public class SharedMemoryDead : CustomErrorException
    {
        private const string errMsg = "Connected to MSI Afterburner shared memory that is flagged as dead.";

        public SharedMemoryDead()
          : base("Connected to MSI Afterburner shared memory that is flagged as dead.")
        {
        }

        public SharedMemoryDead(Exception innerEx)
          : base("Connected to MSI Afterburner shared memory that is flagged as dead.", innerEx)
        {
        }
    }
}
