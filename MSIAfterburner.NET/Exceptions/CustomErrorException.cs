// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.Exceptions.CustomErrorException
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;

namespace MSI.Afterburner.Exceptions
{
    [Serializable]
    public class CustomErrorException : ApplicationException
    {
        public string ErrorMessage => this.Message.ToString();

        public CustomErrorException(string errorMessage)
          : base(errorMessage)
        {
        }

        public CustomErrorException(string errorMessage, Exception innerEx)
          : base(errorMessage, innerEx)
        {
        }
    }
}
