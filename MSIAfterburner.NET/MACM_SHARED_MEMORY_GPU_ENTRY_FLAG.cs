// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;

namespace MSI.Afterburner
{
    [Flags]
    public enum MACM_SHARED_MEMORY_GPU_ENTRY_FLAG : uint
    {
        None = 0,
        CORE_CLOCK = 1,
        SHADER_CLOCK = 2,
        MEMORY_CLOCK = 4,
        FAN_SPEED = 8,
        CORE_VOLTAGE = 16, // 0x00000010
        MEMORY_VOLTAGE = 32, // 0x00000020
        AUX_VOLTAGE = 64, // 0x00000040
        CORE_VOLTAGE_BOOST = 128, // 0x00000080
        MEMORY_VOLTAGE_BOOST = 256, // 0x00000100
        AUX_VOLTAGE_BOOST = 512, // 0x00000200
        POWER_LIMIT = 1024, // 0x00000400
        CORE_CLOCK_BOOST = 2048, // 0x00000800
        MEMORY_CLOCK_BOOST = 4096, // 0x00001000
        THERMAL_LIMIT = 8192, // 0x00002000
        THERMAL_PRIORITIZE = 16384, // 0x00004000

        MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_AUX2_VOLTAGE = 32768,  //                        0x00008000
        MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_AUX2_VOLTAGE_BOOST = 65536, //           0x00010000
        MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE = 131072, //                                      0x00020000
        MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE_ENABLED = 262144, //<-                     0x00040000

        UNK5 = 524288,
        SYNCHRONIZED_WITH_MASTER = 2147483648, // 0x80000000
    }
}
