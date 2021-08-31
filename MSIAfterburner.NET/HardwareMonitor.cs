// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.HardwareMonitor
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using MSI.Afterburner.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MSI.Afterburner
{
    [Serializable]
    public class HardwareMonitor : ISerializable, IXmlSerializable
    {
        private SharedMemory mmf;
        private HardwareMonitorHeader header;
        private HardwareMonitorEntry[] entries;
        private HardwareMonitorGpuEntry[] gpuEntries;
        public static uint GPU_GLOBAL_INDEX = uint.MaxValue;

        public HardwareMonitorHeader Header => this.header;

        public HardwareMonitorEntry[] Entries => this.entries;

        public HardwareMonitorGpuEntry[] GpuEntries => this.gpuEntries;

        public HardwareMonitor()
        {
            this.header = new HardwareMonitorHeader();
            this.ReloadAll();
        }

        ~HardwareMonitor() => this.Disconnect();

        private void loadEntry(uint index)
        {
            long offset = (long)(this.header.HeaderSize + this.header.EntrySize * index);
            this.mmf.ReadMAHMEntry(ref this.entries[(int)index].mahmEntry, offset);
        }

        private void loadGpuEntry(uint gpuIndex)
        {
            long offset = (long)(uint)((int)this.header.HeaderSize + (int)this.header.EntrySize * (int)this.header.EntryCount + (int)this.header.GpuEntrySize * (int)gpuIndex);
            this.mmf.ReadMAHMGpuEntry(ref this.gpuEntries[(int)gpuIndex].mahmGpuEntry, offset);
        }

        public void Connect()
        {
            this.Disconnect();
            try
            {
                this.mmf = new SharedMemory("MAHMSharedMemory", Win32API.FileMapAccess.FileMapAllAccess);
            }
            catch (FileNotFoundException ex)
            {
                throw new SharedMemoryNotFound((Exception)ex);
            }
            catch (Exception ex)
            {
                throw new SharedMemoryNotFound(ex);
            }
        }

        public void Disconnect()
        {
            if (this.mmf != null)
            {
                this.mmf.Dispose();
                this.mmf = (SharedMemory)null;
            }
            GC.Collect();
        }

        public void ReloadAll()
        {
            this.ReloadHeader();
            this.entries = new HardwareMonitorEntry[(int)this.header.EntryCount];
            for (uint index = 0; index < this.header.EntryCount; ++index)
            {
                this.entries[(int)index] = new HardwareMonitorEntry();
                this.loadEntry(index);
                //Console.WriteLine(this.entries[(int) index].ToString());
            }
            this.gpuEntries = new HardwareMonitorGpuEntry[(int)this.header.GpuEntryCount];
            for (uint index = 0; index < this.header.GpuEntryCount; ++index)
            {
                this.gpuEntries[(int)index] = new HardwareMonitorGpuEntry(index);
                this.loadGpuEntry(index);
                //Console.WriteLine(this.gpuEntries[(int) index].ToString());
            }
        }

        public void ReloadHeader()
        {
            this.Connect();
            this.mmf.ReadMAHMHeader(ref this.header.mahmHeader);
            this.header.Validate();
        }

        public void ReloadGpuEntry(uint gpuIndex)
        {
            if (gpuIndex < 0U || gpuIndex > this.header.GpuEntryCount - 1U)
                throw new ArgumentOutOfRangeException();
            this.loadGpuEntry(gpuIndex);
        }

        public void ReloadEntry(uint index)
        {
            if (index < 0U || index > this.header.EntryCount - 1U)
                throw new ArgumentOutOfRangeException();
            this.loadEntry(index);
        }

        public void ReloadEntry(uint gpuIndex, MONITORING_SOURCE_ID id)
        {
            if (!this.VerifyGpuIndex(gpuIndex))
                throw new ArgumentOutOfRangeException();
            if (!Enum.IsDefined(typeof(MONITORING_SOURCE_ID), (object)id))
                throw new ArgumentOutOfRangeException();
            for (uint index = 0; index < this.header.EntryCount; ++index)
            {
                if ((int)this.entries[(int)index].GPU == (int)gpuIndex && (MONITORING_SOURCE_ID)this.entries[(int)index].SrcId == id)
                    this.loadEntry(index);
            }
        }

        public void ReloadEntry(uint gpuIndex, string name)
        {
            if (!this.VerifyGpuIndex(gpuIndex))
                throw new ArgumentOutOfRangeException();
            for (uint index = 0; index < this.header.EntryCount; ++index)
            {
                if ((int)this.entries[(int)index].GPU == (int)gpuIndex && this.entries[(int)index].SrcName == name)
                    this.loadEntry(index);
            }
        }

        public void ReloadEntry(HardwareMonitorEntry dataSource)
        {
            for (uint index = 0; index < this.header.EntryCount; ++index)
            {
                if (this.entries[(int)index] == dataSource)
                    this.loadEntry(index);
            }
        }

        public HardwareMonitorEntry GetEntry(uint gpuIndex, MONITORING_SOURCE_ID id)
        {
            if (!this.VerifyGpuIndex(gpuIndex))
                throw new ArgumentOutOfRangeException();
            if (!Enum.IsDefined(typeof(MONITORING_SOURCE_ID), (object)id))
                throw new ArgumentOutOfRangeException();
            for (int index = 0; (long)index < (long)this.header.EntryCount; ++index)
            {
                if ((int)this.entries[index].GPU == (int)gpuIndex && (MONITORING_SOURCE_ID)this.entries[index].SrcId == id)
                    return this.entries[index];
            }
            return (HardwareMonitorEntry)null;
        }

        public HardwareMonitorEntry GetEntry(uint gpuIndex, string name)
        {
            if (!this.VerifyGpuIndex(gpuIndex))
                throw new ArgumentOutOfRangeException();
            for (int index = 0; (long)index < (long)this.header.EntryCount; ++index)
            {
                if ((int)this.entries[index].GPU == (int)gpuIndex && this.entries[index].SrcName == name)
                    return this.entries[index];
            }
            return (HardwareMonitorEntry)null;
        }

        private bool VerifyGpuIndex(uint gpuIndex) => gpuIndex == uint.MaxValue || gpuIndex >= 0U && gpuIndex <= this.header.EntryCount - 1U;

        public override string ToString()
        {
            try
            {
                string str1 = this.header.ToString() + "\n";
                for (int index = 0; (long)index < (long)this.header.EntryCount; ++index)
                    str1 = str1 + this.entries[index].ToString() + "\n";
                string str2 = str1 + "\n";
                for (int index = 0; (long)index < (long)this.header.GpuEntryCount; ++index)
                    str2 = str2 + this.gpuEntries[index].ToString() + "\n";
                return str2;
            }
            catch
            {
                return base.ToString();
            }
        }

        private HardwareMonitor(SerializationInfo info, StreamingContext ctxt)
        {
            this.header = (HardwareMonitorHeader)info.GetValue(nameof(header), typeof(HardwareMonitorHeader));
            this.entries = (HardwareMonitorEntry[])info.GetValue(nameof(entries), typeof(HardwareMonitorEntry[]));
            this.gpuEntries = (HardwareMonitorGpuEntry[])info.GetValue(nameof(gpuEntries), typeof(HardwareMonitorGpuEntry[]));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("header", (object)this.Header);
            info.AddValue("entries", (object)this.Entries);
            info.AddValue("gpuEntries", (object)this.GpuEntries);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            new XmlSerializer(typeof(HardwareMonitorHeader)).Serialize(writer, (object)this.Header);
            writer.WriteStartElement("HardwareMonitorEntries");
            for (int index = 0; (long)index < (long)this.Header.EntryCount; ++index)
                new XmlSerializer(typeof(HardwareMonitorEntry)).Serialize(writer, (object)this.Entries[index]);
            writer.WriteEndElement();
            writer.WriteStartElement("HardwareMonitorGpuEntries");
            for (int index = 0; (long)index < (long)this.Header.GpuEntryCount; ++index)
                new XmlSerializer(typeof(HardwareMonitorGpuEntry)).Serialize(writer, (object)this.GpuEntries[index]);
            writer.WriteEndElement();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "HardwareMonitorHeader")
            {
                this.header = new HardwareMonitorHeader();
                this.header.ReadXml(reader);
            }
            reader.Read();
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "HardwareMonitorEntries")
            {
                List<HardwareMonitorEntry> hardwareMonitorEntryList = new List<HardwareMonitorEntry>();
                reader.Read();
                while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "HardwareMonitorEntry")
                {
                    HardwareMonitorEntry hardwareMonitorEntry = new HardwareMonitorEntry();
                    hardwareMonitorEntry.ReadXml(reader);
                    hardwareMonitorEntryList.Add(hardwareMonitorEntry);
                    reader.Read();
                }
                this.entries = hardwareMonitorEntryList.ToArray();
            }
            reader.Read();
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "HardwareMonitorGpuEntries")
            {
                List<HardwareMonitorGpuEntry> hardwareMonitorGpuEntryList = new List<HardwareMonitorGpuEntry>();
                reader.Read();
                while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "HardwareMonitorGpuEntry")
                {
                    HardwareMonitorGpuEntry hardwareMonitorGpuEntry = new HardwareMonitorGpuEntry();
                    hardwareMonitorGpuEntry.ReadXml(reader);
                    hardwareMonitorGpuEntryList.Add(hardwareMonitorGpuEntry);
                    reader.Read();
                }
                this.gpuEntries = hardwareMonitorGpuEntryList.ToArray();
            }
            reader.Read();
            reader.ReadEndElement();
        }

        XmlSchema IXmlSerializable.GetSchema() => (XmlSchema)null;

        internal static byte[] RawSerialize(object anything)
        {
            int length = Marshal.SizeOf(anything);
            IntPtr num = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(anything, num, false);
            byte[] destination = new byte[length];
            Marshal.Copy(num, destination, 0, length);
            Marshal.FreeHGlobal(num);
            return destination;
        }

        internal static object RawDeserialize(byte[] rawdatas, Type anytype)
        {
            int num1 = Marshal.SizeOf(anytype);
            if (num1 > rawdatas.Length)
                return (object)null;
            IntPtr num2 = Marshal.AllocHGlobal(num1);
            Marshal.Copy(rawdatas, 0, num2, num1);
            object structure = Marshal.PtrToStructure(num2, anytype);
            Marshal.FreeHGlobal(num2);
            return structure;
        }
    }
}
