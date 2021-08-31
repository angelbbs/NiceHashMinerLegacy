// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.MAHM_SHARED_MEMORY_ENTRY_FLAG
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;

namespace MSI.Afterburner
{
    [Flags]
    public enum MAHM_SHARED_MEMORY_ENTRY_FLAG : uint
    {
        None = 0,
        SHOW_IN_OSD = 1,
        SHOW_IN_LCD = 2,
        SHOW_IN_TRAY = 4,
    }
}
