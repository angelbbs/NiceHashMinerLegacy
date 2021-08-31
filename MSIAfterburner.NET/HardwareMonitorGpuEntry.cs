// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.HardwareMonitorGpuEntry
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
    public class HardwareMonitorGpuEntry : ISerializable, IXmlSerializable
    {
        internal MAHM_SHARED_MEMORY_GPU_ENTRY mahmGpuEntry;
        private uint index = HardwareMonitor.GPU_GLOBAL_INDEX;

        public string GpuId => new string(this.mahmGpuEntry.gpuId).TrimEnd(new char[1]);

        public string Family => new string(this.mahmGpuEntry.family).TrimEnd(new char[1]);

        public string Device => new string(this.mahmGpuEntry.device).TrimEnd(new char[1]);

        public string Driver => new string(this.mahmGpuEntry.driver).TrimEnd(new char[1]);

        public string BIOS => new string(this.mahmGpuEntry.BIOS).TrimEnd(new char[1]);

        public uint MemAmount => this.mahmGpuEntry.memAmount;

        public uint Index => this.index;

        public HardwareMonitorGpuEntry()
        {
        }

        public HardwareMonitorGpuEntry(uint index) => this.index = index;

        public override string ToString()
        {
            try
            {
                return "GpuId = " + this.GpuId + ";Family = " + this.Family + ";Device = " + this.Device.ToString() + ";Driver = " + this.Driver.ToString() + ";BIOS = " + this.BIOS.ToString() + ";MemAmount = " + this.MemAmount.ToString();
            }
            catch
            {
                return base.ToString();
            }
        }

        private HardwareMonitorGpuEntry(SerializationInfo info, StreamingContext ctxt)
        {
            this.mahmGpuEntry.gpuId = info.GetString("gpuId").ToCharArray();
            this.mahmGpuEntry.family = info.GetString("family").ToCharArray();
            this.mahmGpuEntry.device = info.GetString("device").ToCharArray();
            this.mahmGpuEntry.driver = info.GetString("driver").ToCharArray();
            this.mahmGpuEntry.BIOS = info.GetString(nameof(BIOS)).ToCharArray();
            this.mahmGpuEntry.memAmount = info.GetUInt32("memAmount");
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("gpuId", (object)this.GpuId);
            info.AddValue("family", (object)this.Family);
            info.AddValue("device", (object)this.Device);
            info.AddValue("driver", (object)this.Driver);
            info.AddValue("BIOS", (object)this.BIOS);
            info.AddValue("memAmount", this.MemAmount);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("gpuId", this.GpuId);
            writer.WriteElementString("family", this.Family);
            writer.WriteElementString("device", this.Device);
            writer.WriteElementString("driver", this.Driver);
            writer.WriteElementString("BIOS", this.BIOS);
            writer.WriteElementString("memAmount", this.MemAmount.ToString());
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            this.mahmGpuEntry.gpuId = reader.ReadElementString("gpuId").ToCharArray();
            this.mahmGpuEntry.family = reader.ReadElementString("family").ToCharArray();
            this.mahmGpuEntry.device = reader.ReadElementString("device").ToCharArray();
            this.mahmGpuEntry.driver = reader.ReadElementString("driver").ToCharArray();
            this.mahmGpuEntry.BIOS = reader.ReadElementString("BIOS").ToCharArray();
            this.mahmGpuEntry.memAmount = uint.Parse(reader.ReadElementString("memAmount"));
        }

        XmlSchema IXmlSerializable.GetSchema() => (XmlSchema)null;
    }
}
