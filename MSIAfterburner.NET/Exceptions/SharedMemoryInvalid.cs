// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.Exceptions.SharedMemoryInvalid
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;

namespace MSI.Afterburner.Exceptions
{
    public class SharedMemoryInvalid : CustomErrorException
    {
        private const string errMsg = "Connected to invalid MSI Afterburner shared memory.";

        public SharedMemoryInvalid()
          : base("Connected to invalid MSI Afterburner shared memory.")
        {
        }

        public SharedMemoryInvalid(Exception innerEx)
          : base("Connected to invalid MSI Afterburner shared memory.", innerEx)
        {
        }
    }
}
