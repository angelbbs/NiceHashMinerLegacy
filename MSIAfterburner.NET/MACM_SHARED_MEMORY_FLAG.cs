﻿// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.MACM_SHARED_MEMORY_FLAG
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;

namespace MSI.Afterburner
{
  [Flags]
  public enum MACM_SHARED_MEMORY_FLAG : uint
  {
    None = 0,
    LINK = 1,
    SYNC = 2,
    THERMAL = 4,
  }
}
