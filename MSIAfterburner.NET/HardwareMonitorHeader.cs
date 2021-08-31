// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.HardwareMonitorHeader
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using MSI.Afterburner.Exceptions;
using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MSI.Afterburner
{
    [Serializable]
    public class HardwareMonitorHeader : ISerializable, IXmlSerializable
    {
        internal MAHM_SHARED_MEMORY_HEADER mahmHeader;

        public uint Signature => this.mahmHeader.signature;

        public uint Version => this.mahmHeader.version;

        public uint HeaderSize => this.mahmHeader.headerSize;

        public uint EntryCount => this.mahmHeader.entryCount;

        public uint EntrySize => this.mahmHeader.entrySize;

        public uint Time => this.mahmHeader.time;

        public uint GpuEntryCount => this.mahmHeader.gpuEntryCount;

        public uint GpuEntrySize => this.mahmHeader.gpuEntrySize;

        public HardwareMonitorHeader()
        {
        }

        public string GetSignatureText()
        {
            char[] charArray = Encoding.ASCII.GetString(BitConverter.GetBytes(this.mahmHeader.signature)).ToCharArray();
            Array.Reverse((Array)charArray);
            return new string(charArray);
        }

        public DateTime GetDateTime()
        {
            DateTime dateTime = new DateTime(1970, 1, 1);
            dateTime = dateTime.AddSeconds((double)this.mahmHeader.time);
            return dateTime.ToLocalTime();
        }

        public string GetVersionText() => (this.mahmHeader.version >> 16).ToString() + "." + (object)(short)this.mahmHeader.version;

        internal void Validate()
        {
            if (this.GetSignatureText().CompareTo("MAHM") != 0)
            {
                if (this.Signature == 57005U)
                    throw new SharedMemoryDead();
                throw new SharedMemoryInvalid();
            }
            if (this.mahmHeader.version < 131072U)
                throw new SharedMemoryVersionNotSupported();
        }

        public override string ToString()
        {
            try
            {
                return "Signature = " + this.GetSignatureText() + ";Version = " + this.GetVersionText() + ";HeaderSize = " + (object)this.HeaderSize + ";EntryCount = " + (object)this.EntryCount + ";EntrySize = " + (object)this.EntrySize + ";Time = " + this.GetDateTime().ToString("hh:mm:ss MMM-dd-yyyy") + ";GpuEntryCount = " + (object)this.GpuEntryCount + ";GpuEntrySize = " + (object)this.GpuEntrySize;
            }
            catch
            {
                return base.ToString();
            }
        }

        private HardwareMonitorHeader(SerializationInfo info, StreamingContext ctxt)
        {
            this.mahmHeader.signature = (uint)info.GetValue("signature", typeof(uint));
            this.mahmHeader.version = (uint)info.GetValue("version", typeof(uint));
            this.mahmHeader.headerSize = (uint)info.GetValue("headerSize", typeof(uint));
            this.mahmHeader.entryCount = (uint)info.GetValue("entryCount", typeof(uint));
            this.mahmHeader.entrySize = (uint)info.GetValue("entrySize", typeof(uint));
            this.mahmHeader.time = (uint)info.GetValue("time", typeof(uint));
            this.mahmHeader.gpuEntryCount = (uint)info.GetValue("gpuEntryCount", typeof(uint));
            this.mahmHeader.gpuEntrySize = (uint)info.GetValue("gpuEntrySize", typeof(uint));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("signature", this.Signature);
            info.AddValue("version", this.Version);
            info.AddValue("headerSize", this.HeaderSize);
            info.AddValue("entryCount", this.EntryCount);
            info.AddValue("entrySize", this.EntrySize);
            info.AddValue("time", this.Time);
            info.AddValue("gpuEntryCount", this.GpuEntryCount);
            info.AddValue("gpuEntrySize", this.GpuEntrySize);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("signature", this.Signature.ToString());
            writer.WriteElementString("version", this.Version.ToString());
            writer.WriteElementString("headerSize", this.HeaderSize.ToString());
            writer.WriteElementString("entryCount", this.EntryCount.ToString());
            writer.WriteElementString("entrySize", this.EntrySize.ToString());
            writer.WriteElementString("time", this.Time.ToString());
            writer.WriteElementString("gpuEntryCount", this.GpuEntryCount.ToString());
            writer.WriteElementString("gpuEntrySize", this.GpuEntrySize.ToString());
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            this.mahmHeader.signature = uint.Parse(reader.ReadElementString("signature"));
            this.mahmHeader.version = uint.Parse(reader.ReadElementString("version"));
            this.mahmHeader.headerSize = uint.Parse(reader.ReadElementString("headerSize"));
            this.mahmHeader.entryCount = uint.Parse(reader.ReadElementString("entryCount"));
            this.mahmHeader.entrySize = uint.Parse(reader.ReadElementString("entrySize"));
            this.mahmHeader.time = uint.Parse(reader.ReadElementString("time"));
            this.mahmHeader.gpuEntryCount = uint.Parse(reader.ReadElementString("gpuEntryCount"));
            this.mahmHeader.gpuEntrySize = uint.Parse(reader.ReadElementString("gpuEntrySize"));
        }

        XmlSchema IXmlSerializable.GetSchema() => (XmlSchema)null;
    }
}
