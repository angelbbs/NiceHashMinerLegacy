// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.MONITORING_SOURCE_ID
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

namespace MSI.Afterburner
{
    public enum MONITORING_SOURCE_ID : uint
    {
        GPU_TEMPERATURE = 0,
        PCB_TEMPERATURE = 1,
        MEM_TEMPERATURE = 2,
        VRM_TEMPERATURE = 3,
        FAN_SPEED = 16, // 0x00000010
        FAN_TACHOMETER = 17, // 0x00000011
        CORE_CLOCK = 32, // 0x00000020
        SHADER_CLOCK = 33, // 0x00000021
        MEMORY_CLOCK = 34, // 0x00000022
        GPU_USAGE = 48, // 0x00000030
        MEMORY_USAGE = 49, // 0x00000031
        GPU_VOLTAGE = 64, // 0x00000040
        AUX_VOLTAGE = 65, // 0x00000041
        MEMORY_VOLTAGE = 66, // 0x00000042
        FRAMERATE = 80, // 0x00000050
        GPU_POWER = 96, // 0x00000060
        UNKNOWN = 4294967295, // 0xFFFFFFFF
    }
}
