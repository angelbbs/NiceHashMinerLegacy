// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.MAHM_SHARED_MEMORY_HEADER
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;

namespace MSI.Afterburner
{
    [Serializable]
    public struct MAHM_SHARED_MEMORY_HEADER
    {
        public uint signature;
        public uint version;
        public uint headerSize;
        public uint entryCount;
        public uint entrySize;
        public uint time;
        public uint gpuEntryCount;
        public uint gpuEntrySize;
    }
}
