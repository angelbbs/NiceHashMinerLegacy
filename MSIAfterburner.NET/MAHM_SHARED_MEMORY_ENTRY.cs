// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.MAHM_SHARED_MEMORY_ENTRY
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;
using System.Runtime.InteropServices;

namespace MSI.Afterburner
{
    [Serializable]
    public struct MAHM_SHARED_MEMORY_ENTRY
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public char[] srcName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public char[] srcUnits;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public char[] localizedSrcName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public char[] localizedSrcUnits;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public char[] recommendedFormat;
        public float data;
        public float minLimit;
        public float maxLimit;
        public MAHM_SHARED_MEMORY_ENTRY_FLAG flags;
        public uint gpu;
        public uint srcId;
    }
}
