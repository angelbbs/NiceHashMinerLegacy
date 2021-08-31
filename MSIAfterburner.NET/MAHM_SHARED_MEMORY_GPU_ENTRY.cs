// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.MAHM_SHARED_MEMORY_GPU_ENTRY
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;
using System.Runtime.InteropServices;

namespace MSI.Afterburner
{
    [Serializable]
    public struct MAHM_SHARED_MEMORY_GPU_ENTRY
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public char[] gpuId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public char[] family;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public char[] device;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public char[] driver;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public char[] BIOS;
        public uint memAmount;
    }
}
