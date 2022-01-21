// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.ControlMemoryHeader
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
    public class ControlMemoryHeader : ISerializable, IXmlSerializable
    {
        internal MACM_SHARED_MEMORY_HEADER macmHeader;

        public uint Signature => this.macmHeader.signature;

        public uint Version => this.macmHeader.version;

        public uint HeaderSize => this.macmHeader.headerSize;

        public uint GpuEntryCount => this.macmHeader.gpuEntryCount;

        public uint GpuEntrySize => this.macmHeader.gpuEntrySize;

        public uint MasterGpu => this.macmHeader.masterGpu;

        public MACM_SHARED_MEMORY_FLAG Flags => this.macmHeader.flags;

        public uint Time => this.macmHeader.time;

        public MACM_SHARED_MEMORY_COMMAND Command => this.macmHeader.command;

        public ControlMemoryHeader()
        {
        }

        public string GetSignatureText()
        {
            char[] charArray = Encoding.ASCII.GetString(BitConverter.GetBytes(this.macmHeader.signature)).ToCharArray();
            Array.Reverse((Array)charArray);
            return new string(charArray);
        }

        public DateTime GetDateTime()
        {
            DateTime dateTime = new DateTime(1970, 1, 1);
            dateTime = dateTime.AddSeconds((double)this.macmHeader.time);
            return dateTime.ToLocalTime();
        }

        public string GetVersionText() => (this.macmHeader.version >> 16).ToString() + "." + (object)(short)this.macmHeader.version;

        internal void SetCommandNone() => this.macmHeader.command = MACM_SHARED_MEMORY_COMMAND.None;
        internal void SetCommandFlush() => this.macmHeader.command = MACM_SHARED_MEMORY_COMMAND.FLUSH;

        internal void SetCommandInit() => this.macmHeader.command = MACM_SHARED_MEMORY_COMMAND.INIT;

        internal void Validate()
        {
            try
            {
                if (this.GetSignatureText().CompareTo("MACM") != 0)
                {
                    if (this.Signature == 57005U)
                    {
                        throw new SharedMemoryDead();
                    }

                    throw new SharedMemoryInvalid();
                }
                if (this.macmHeader.version < 131073U)
                {
                    throw new SharedMemoryVersionNotSupported();
                }
            }
            catch (Exception ex)
            {
                throw new SharedMemoryInvalid(ex);
            }
        }

        public override string ToString()
        {
            try
            {
                return "Signature = " + this.GetSignatureText() + ";Version = " + this.GetVersionText() + ";HeaderSize = " + (object)this.HeaderSize + ";GpuEntryCount = " + (object)this.GpuEntryCount + ";GpuEntrySize = " + (object)this.GpuEntrySize + ";MasterGpu = " + (object)this.MasterGpu + ";Flags = " + this.Flags.ToString() + ";Time = " + this.GetDateTime().ToString("hh:mm:ss MMM-dd-yyyy") + ";Command = " + this.Command.ToString();
            }
            catch
            {
                return base.ToString();
            }
        }

        private ControlMemoryHeader(SerializationInfo info, StreamingContext ctxt)
        {
            this.macmHeader.signature = info.GetUInt32("signature");
            this.macmHeader.version = info.GetUInt32("version");
            this.macmHeader.headerSize = info.GetUInt32("headerSize");
            this.macmHeader.gpuEntryCount = info.GetUInt32("gpuEntryCount");
            this.macmHeader.gpuEntrySize = info.GetUInt32("gpuEntrySize");
            this.macmHeader.masterGpu = info.GetUInt32("masterGpu");
            this.macmHeader.flags = (MACM_SHARED_MEMORY_FLAG)info.GetValue("flags", typeof(MACM_SHARED_MEMORY_FLAG));
            this.macmHeader.time = info.GetUInt32("time");
            this.macmHeader.command = (MACM_SHARED_MEMORY_COMMAND)info.GetValue("command", typeof(MACM_SHARED_MEMORY_COMMAND));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("signature", this.Signature);
            info.AddValue("version", this.Version);
            info.AddValue("headerSize", this.HeaderSize);
            info.AddValue("gpuEntryCount", this.GpuEntryCount);
            info.AddValue("gpuEntrySize", this.GpuEntrySize);
            info.AddValue("masterGpu", this.MasterGpu);
            info.AddValue("flags", (object)this.Flags);
            info.AddValue("time", this.Time);
            info.AddValue("command", (object)this.Command);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("signature", this.Signature.ToString());
            writer.WriteElementString("version", this.Version.ToString());
            writer.WriteElementString("headerSize", this.HeaderSize.ToString());
            writer.WriteElementString("gpuEntryCount", this.GpuEntryCount.ToString());
            writer.WriteElementString("gpuEntrySize", this.GpuEntrySize.ToString());
            writer.WriteElementString("masterGpu", this.MasterGpu.ToString());
            writer.WriteElementString("flags", this.Flags.ToString());
            writer.WriteElementString("time", this.Time.ToString());
            writer.WriteElementString("command", this.Command.ToString());
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            this.macmHeader.signature = uint.Parse(reader.ReadElementString("signature"));
            this.macmHeader.version = uint.Parse(reader.ReadElementString("version"));
            this.macmHeader.headerSize = uint.Parse(reader.ReadElementString("headerSize"));
            this.macmHeader.gpuEntryCount = uint.Parse(reader.ReadElementString("gpuEntryCount"));
            this.macmHeader.gpuEntrySize = uint.Parse(reader.ReadElementString("gpuEntrySize"));
            this.macmHeader.masterGpu = uint.Parse(reader.ReadElementString("masterGpu"));
            this.macmHeader.flags = (MACM_SHARED_MEMORY_FLAG)Enum.Parse(typeof(MACM_SHARED_MEMORY_FLAG), reader.ReadElementString("flags"));
            this.macmHeader.time = uint.Parse(reader.ReadElementString("time"));
            this.macmHeader.command = (MACM_SHARED_MEMORY_COMMAND)Enum.Parse(typeof(MACM_SHARED_MEMORY_COMMAND), reader.ReadElementString("command"));
        }

        XmlSchema IXmlSerializable.GetSchema() => (XmlSchema)null;
    }
}
