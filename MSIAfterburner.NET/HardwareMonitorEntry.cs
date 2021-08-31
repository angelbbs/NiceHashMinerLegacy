// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.HardwareMonitorEntry
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MSI.Afterburner
{
    [Serializable]
    public class HardwareMonitorEntry : ISerializable, IXmlSerializable
    {
        internal MAHM_SHARED_MEMORY_ENTRY mahmEntry;

        public string SrcName => new string(this.mahmEntry.srcName).TrimEnd(new char[1]);

        public string SrcUnits => new string(this.mahmEntry.srcUnits).TrimEnd(new char[1]);

        public string LocalizedSrcName => new string(this.mahmEntry.localizedSrcName).TrimEnd(new char[1]);

        public string LocalizedSrcUnits => new string(this.mahmEntry.localizedSrcUnits).TrimEnd(new char[1]);

        public string RecommendedFormat => new string(this.mahmEntry.recommendedFormat).TrimEnd(new char[1]);

        public float Data => (double)this.mahmEntry.data == 3.40282346638529E+38 ? 0.0f : this.mahmEntry.data;

        public float MinLimit => this.mahmEntry.minLimit;

        public float MaxLimit => this.mahmEntry.maxLimit;

        public MAHM_SHARED_MEMORY_ENTRY_FLAG Flags => this.mahmEntry.flags;

        public uint GPU => this.mahmEntry.gpu;

        public uint SrcId => this.mahmEntry.srcId;

        public HardwareMonitorEntry()
        {
        }

        public override string ToString()
        {
            try
            {
                return "SrcName = " + this.SrcName + ";SrcUnits = " + this.SrcUnits + ";LocalizedSourceName = " + this.LocalizedSrcName + ";LocalizedSrcUnits = " + this.LocalizedSrcUnits + ";RecommendedFormat = " + this.RecommendedFormat + ";Data = " + this.Data.ToString() + ";MinLimit = " + this.MinLimit.ToString() + ";MaxLimit = " + this.MaxLimit.ToString() + ";Flags = " + this.Flags.ToString() + ";GPU = " + this.GPU.ToString() + ";SrcId = " + this.SrcId.ToString();
            }
            catch
            {
                return base.ToString();
            }
        }

        private HardwareMonitorEntry(SerializationInfo info, StreamingContext ctxt)
        {
            this.mahmEntry.srcName = info.GetString("srcName").ToCharArray();
            this.mahmEntry.srcUnits = info.GetString("srcUnits").ToCharArray();
            this.mahmEntry.localizedSrcName = info.GetString("localizedSrcName").ToCharArray();
            this.mahmEntry.localizedSrcUnits = info.GetString("localizedSrcUnits").ToCharArray();
            this.mahmEntry.recommendedFormat = info.GetString("recommendedFormat").ToCharArray();
            this.mahmEntry.data = (float)info.GetValue("data", typeof(float));
            this.mahmEntry.minLimit = (float)info.GetValue("minLimit", typeof(float));
            this.mahmEntry.maxLimit = (float)info.GetValue("maxLimit", typeof(float));
            this.mahmEntry.flags = (MAHM_SHARED_MEMORY_ENTRY_FLAG)info.GetValue("flags", typeof(MAHM_SHARED_MEMORY_ENTRY_FLAG));
            this.mahmEntry.gpu = info.GetUInt32("gpu");
            this.mahmEntry.srcId = info.GetUInt32("srcId");
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("srcName", (object)this.SrcName);
            info.AddValue("srcUnits", (object)this.SrcUnits);
            info.AddValue("localizedSrcName", (object)this.LocalizedSrcName);
            info.AddValue("localizedSrcUnits", (object)this.LocalizedSrcUnits);
            info.AddValue("recommendedFormat", (object)this.RecommendedFormat);
            info.AddValue("data", this.Data);
            info.AddValue("minLimit", this.MinLimit);
            info.AddValue("maxLimit", this.MaxLimit);
            info.AddValue("flags", (object)this.Flags);
            info.AddValue("gpu", this.GPU);
            info.AddValue("srcId", this.SrcId);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("srcName", this.SrcName);
            writer.WriteElementString("srcUnits", this.SrcUnits);
            writer.WriteElementString("localizedSrcName", this.LocalizedSrcName);
            writer.WriteElementString("localizedSrcUnits", this.LocalizedSrcUnits);
            writer.WriteElementString("recommendedFormat", this.RecommendedFormat);
            writer.WriteElementString("data", this.Data.ToString());
            writer.WriteElementString("minLimit", this.MinLimit.ToString());
            writer.WriteElementString("maxLimit", this.MaxLimit.ToString());
            writer.WriteElementString("flags", this.Flags.ToString());
            writer.WriteElementString("gpu", this.GPU.ToString());
            writer.WriteElementString("srcId", this.SrcId.ToString());
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            this.mahmEntry.srcName = reader.ReadElementString("srcName").ToCharArray();
            this.mahmEntry.srcUnits = reader.ReadElementString("srcUnits").ToCharArray();
            this.mahmEntry.localizedSrcName = reader.ReadElementString("localizedSrcName").ToCharArray();
            this.mahmEntry.localizedSrcUnits = reader.ReadElementString("localizedSrcUnits").ToCharArray();
            this.mahmEntry.recommendedFormat = reader.ReadElementString("recommendedFormat").ToCharArray();
            this.mahmEntry.data = float.Parse(reader.ReadElementString("data"));
            this.mahmEntry.minLimit = float.Parse(reader.ReadElementString("minLimit"));
            this.mahmEntry.maxLimit = float.Parse(reader.ReadElementString("maxLimit"));
            this.mahmEntry.flags = (MAHM_SHARED_MEMORY_ENTRY_FLAG)Enum.Parse(typeof(MAHM_SHARED_MEMORY_ENTRY_FLAG), reader.ReadElementString("flags"));
            this.mahmEntry.gpu = uint.Parse(reader.ReadElementString("gpu"));
            this.mahmEntry.srcId = uint.Parse(reader.ReadElementString("srcId"));
        }

        XmlSchema IXmlSerializable.GetSchema() => (XmlSchema)null;
    }
}
