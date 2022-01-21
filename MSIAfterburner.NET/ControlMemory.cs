// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.ControlMemory
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using MSI.Afterburner.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MSI.Afterburner
{
    [Serializable]
    public class ControlMemory : ISerializable, IXmlSerializable
    {
        private SharedMemory mmf;
        private ControlMemoryHeader header;
        private ControlMemoryGpuEntry[] gpuEntries;

        public ControlMemoryHeader Header => this.header;

        public ControlMemoryGpuEntry[] GpuEntries => this.gpuEntries;

        public bool Initialized = false;
        public ControlMemory()
        {
            this.header = new ControlMemoryHeader();
            this.ReloadAll();
            Initialized = true;
        }

        ~ControlMemory() => this.Disconnect();

        public void Connect()
        {
            this.Disconnect();
            try
            {
                this.mmf = new SharedMemory("MACMSharedMemory", Win32API.FileMapAccess.FileMapAllAccess);

            }
            catch (FileNotFoundException ex)
            {
                Initialized = false;
                throw new SharedMemoryNotFound(ex);
            }
            catch (Exception ex)
            {
                Initialized = false;
                throw new SharedMemoryNotFound(ex);
            }
            Initialized = true;
        }

        public void Disconnect()
        {
            if (this.mmf != null)
            {
                this.mmf.Dispose();
                //this.mmf = (SharedMemory)null;
            }
            Initialized = false;
            GC.Collect();
        }

        public void CommitChanges(bool flush = true)
        {
            this.ReloadHeader();
            for (int index = 0; (long)index < (long)this.header.GpuEntryCount; ++index)
            {
                long offset = (long)this.header.HeaderSize + (long)this.header.GpuEntrySize * (long)index;
                this.mmf.Write((object)this.gpuEntries[index].macmGpuEntry, offset);
            }
            if (flush)
            {
                this.header.SetCommandFlush();
                Thread.Sleep(10);
                this.mmf.Write((object)this.header.macmHeader, 0L);
            }
        }

        public void Flush()
        {
            this.header.SetCommandFlush();
            Thread.Sleep(10);
            this.mmf.Write((object)this.header.macmHeader, 0L);
        }

        public void CommitChanges(int gpuIndex)
        {
            var val1 = this.gpuEntries[gpuIndex].macmGpuEntry;
            byte[] buffer0 = RawSerialize(this.GpuEntries[gpuIndex], (int)this.Header.GpuEntrySize);

            this.ReloadHeader();

            if (gpuIndex < 0 || (long)gpuIndex > (long)(this.header.GpuEntryCount - 1U))
            {
                Helpers.ConsolePrint("CommitChanges", "ArgumentOutOfRangeException");
            }
            //throw new ArgumentOutOfRangeException();
            long offset = (long)this.header.HeaderSize + (long)this.header.GpuEntrySize * (long)gpuIndex;
            this.mmf.Write((object)this.gpuEntries[gpuIndex].macmGpuEntry, offset);
            this.header.SetCommandFlush();
            Thread.Sleep(10);
            this.mmf.Write((object)this.header.macmHeader, 0L);
        }

        public byte[] ReadRaw(int gpuIndex)
        {
            this.ReloadHeader();
            if (gpuIndex < 0 || (long)gpuIndex > (long)(this.header.GpuEntryCount - 1U))
                throw new ArgumentOutOfRangeException();
            long offset = (long)this.header.HeaderSize + (long)this.header.GpuEntrySize * (long)gpuIndex;
            int bytesToRead = (int)this.header.GpuEntrySize;
            byte[] buffer = new byte[bytesToRead];
            mmf.Read(buffer, bytesToRead, offset);
            return buffer;
        }

        public void Reinitialize()
        {
            this.ReloadHeader();
            this.header.SetCommandInit();
            this.mmf.Write((object)this.header.macmHeader, 0L);
        }

        public void ReloadAll()
        {
            this.ReloadHeader();
            this.gpuEntries = new ControlMemoryGpuEntry[(int)this.header.GpuEntryCount];
            for (int index = 0; (long)index < (long)this.header.GpuEntryCount; ++index)
            {
                this.gpuEntries[index] = new ControlMemoryGpuEntry(this.header, index);
                this.ReloadGpuEntry(index);
            }
        }

        public void ReloadHeader()
        {
            this.Connect();
            this.mmf.ReadMACMHeader(ref this.header.macmHeader);
            this.header.Validate();
        }

        public void ReloadGpuEntry(int gpuIndex)
        {
            if (gpuIndex < 0 || (long)gpuIndex > (long)(this.header.GpuEntryCount - 1U))
                throw new ArgumentOutOfRangeException();
            long offset = (long)this.header.HeaderSize + (long)this.header.GpuEntrySize * (long)gpuIndex;
            this.mmf.ReadMACMGpuEntry(ref this.gpuEntries[gpuIndex].macmGpuEntry, offset);
        }

        public override string ToString()
        {
            try
            {
                string str = this.header.ToString() + "\n";
                for (int index = 0; (long)index < (long)this.header.GpuEntryCount; ++index)
                    str = str + this.gpuEntries[index].ToString() + "\n";
                return str;
            }
            catch
            {
                return base.ToString();
            }
        }

        private ControlMemory(SerializationInfo info, StreamingContext ctxt)
        {
            this.Connect();
            this.header = (ControlMemoryHeader)info.GetValue(nameof(header), typeof(ControlMemoryHeader));
            this.gpuEntries = (ControlMemoryGpuEntry[])info.GetValue(nameof(gpuEntries), typeof(ControlMemoryGpuEntry[]));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("header", (object)this.Header);
            info.AddValue("gpuEntries", (object)this.GpuEntries);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            new XmlSerializer(typeof(ControlMemoryHeader)).Serialize(writer, (object)this.Header);
            writer.WriteStartElement("ControlMemoryGpuEntries");
            for (int index = 0; (long)index < (long)this.Header.GpuEntryCount; ++index)
                new XmlSerializer(typeof(ControlMemoryGpuEntry)).Serialize(writer, (object)this.GpuEntries[index]);
            writer.WriteEndElement();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ControlMemoryHeader")
            {
                this.header = new ControlMemoryHeader();
                this.header.ReadXml(reader);
            }
            reader.Read();
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ControlMemoryGpuEntries")
            {
                List<ControlMemoryGpuEntry> controlMemoryGpuEntryList = new List<ControlMemoryGpuEntry>();
                reader.Read();
                while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ControlMemoryGpuEntry")
                {
                    ControlMemoryGpuEntry controlMemoryGpuEntry = new ControlMemoryGpuEntry();
                    controlMemoryGpuEntry.ReadXml(reader);
                    controlMemoryGpuEntryList.Add(controlMemoryGpuEntry);
                    reader.Read();
                }
                this.gpuEntries = controlMemoryGpuEntryList.ToArray();
            }
            reader.Read();
            reader.ReadEndElement();
        }

        XmlSchema IXmlSerializable.GetSchema() => (XmlSchema)null;

        public static byte[] RawSerialize(object anything)
        {
            int length = Marshal.SizeOf(anything);
            IntPtr num = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(anything, num, false);
            byte[] destination = new byte[length];
            Marshal.Copy(num, destination, 0, length);
            Marshal.FreeHGlobal(num);
            return destination;
        }

        public static byte[] RawSerialize(ControlMemoryGpuEntry obj, int length)
        {
            IntPtr num = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(obj, num, false);
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
