using ManagedCuda.Nvml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NvidiaGPUGetDataHost
{
    class Program
    {
        public static List<NvData> gpuList;
        private static bool isclosing = false;
        [Serializable]
        public struct NvData
        {
            public uint nGpu;
            public uint power;
            public uint fan;
            public uint load;
            public uint temp;
        }

        private readonly nvmlDevice _nvmlDevice;

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

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            if (System.Diagnostics.Process.GetProcessesByName(Application.ProductName).Length > 1)
            {
                return;
            }

            try
            {
                uint devCount = 0;
                nvmlReturn ret;
                nvmlDevice _nvmlDevice = new nvmlDevice();
                NvmlNativeMethods.nvmlInit();

                ret = NvmlNativeMethods.nvmlDeviceGetCount(ref devCount);

                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "NvidiaGPUGetDataHost";
                    eventLog.WriteEntry("devCount: " + devCount.ToString(), EventLogEntryType.Information, 101, 1);
                }
                if (ret != nvmlReturn.Success)
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "NvidiaGPUGetDataHost";
                        eventLog.WriteEntry("nvmlDeviceGetCount error: " + ret.ToString(), EventLogEntryType.Error, 101, 1);
                    }
                    return;
                }

                List<NvData> gpuList = new List<NvData>();

                int ticks = 0;

                int devn = 0;
                var _power = 0u;
                var _fan = 0u;
                var _load = 0u;
                var _temp = 0u;

                int size = Marshal.SizeOf(devn) + Marshal.SizeOf(_power) + Marshal.SizeOf(_fan) + Marshal.SizeOf(_load) + Marshal.SizeOf(_temp);

                MemoryMappedFile sharedMemory = MemoryMappedFile.CreateOrOpen("NvidiaGPUGetDataHost", size * Marshal.SizeOf(devCount));
                do
                {
                    for (int dev = 0; dev < devCount; dev++)
                    {
                        ret = NvmlNativeMethods.nvmlDeviceGetHandleByIndex((uint)dev, ref _nvmlDevice);
                        if (ret != nvmlReturn.Success)
                        {
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "NvidiaGPUGetDataHost";
                                eventLog.WriteEntry("nvmlDeviceGetHandleByIndex error: " + ret.ToString(), EventLogEntryType.Error, 101, 1);
                            }
                            break;
                        }
                        
                        ret = NvmlNativeMethods.nvmlDeviceGetPowerUsage(_nvmlDevice, ref _power);// <- mem leak 461.40+
                        if (ret != nvmlReturn.Success)
                        {
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "NvidiaGPUGetDataHost";
                                eventLog.WriteEntry("nvmlDeviceGetPowerUsage error: " + ret.ToString(), EventLogEntryType.Error, 101, 1);
                            }
                            break;
                        }
                        ret = NvmlNativeMethods.nvmlDeviceGetFanSpeed(_nvmlDevice, ref _fan);
                        if (ret != nvmlReturn.Success)
                        {
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "NvidiaGPUGetDataHost";
                                eventLog.WriteEntry("nvmlDeviceGetFanSpeed error: " + ret.ToString(), EventLogEntryType.Error, 101, 1);
                            }
                            break;
                        }
                        var rates = new nvmlUtilization();
                        ret = NvmlNativeMethods.nvmlDeviceGetUtilizationRates(_nvmlDevice, ref rates);
                        if (ret != nvmlReturn.Success)
                        {
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "NvidiaGPUGetDataHost";
                                eventLog.WriteEntry("nvmlDeviceGetUtilizationRates error: " + ret.ToString(), EventLogEntryType.Error, 101, 1);
                            }
                            break;
                        }
                        _load = rates.gpu;
                        ret = NvmlNativeMethods.nvmlDeviceGetTemperature(_nvmlDevice, nvmlTemperatureSensors.Gpu, ref _temp);
                        if (ret != nvmlReturn.Success)
                        {
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "NvidiaGPUGetDataHost";
                                eventLog.WriteEntry("nvmlDeviceGetTemperature error: " + ret.ToString(), EventLogEntryType.Error, 101, 1);
                            }
                            break;
                        }

                        using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(0, size * devCount + Marshal.SizeOf(devCount)))
                        {
                            writer.WriteArray<byte>(0, RawSerialize(devCount), 0, Marshal.SizeOf(devCount));
                            writer.WriteArray<byte>(size * dev + Marshal.SizeOf(devCount), BitConverter.GetBytes(Convert.ToInt32((long)_nvmlDevice.Pointer % Int32.MaxValue)), 0, Marshal.SizeOf(dev));
                            writer.WriteArray<byte>(size * dev + Marshal.SizeOf(devCount) + Marshal.SizeOf(dev), BitConverter.GetBytes(_power), 0, Marshal.SizeOf(_power));
                            writer.WriteArray<byte>(size * dev + Marshal.SizeOf(devCount) + Marshal.SizeOf(dev) + Marshal.SizeOf(_power), BitConverter.GetBytes(_fan), 0, Marshal.SizeOf(_fan));
                            writer.WriteArray<byte>(size * dev + Marshal.SizeOf(devCount) + Marshal.SizeOf(dev) + Marshal.SizeOf(_power) + Marshal.SizeOf(_fan), BitConverter.GetBytes(_load), 0, Marshal.SizeOf(_load));
                            writer.WriteArray<byte>(size * dev + Marshal.SizeOf(devCount) + Marshal.SizeOf(dev) + Marshal.SizeOf(_power) + Marshal.SizeOf(_fan) + Marshal.SizeOf(_load), BitConverter.GetBytes(_temp), 0, Marshal.SizeOf(_temp));
                        }
                    }

                    Thread.Sleep(1000);
                    //sharedMemory.Dispose();

                    Process currentProc = Process.GetCurrentProcess();
                    double bytesInUse = currentProc.PrivateMemorySize64;
                    if (ticks > 60)
                    {
                        GC.Collect();
                    }
                    if (bytesInUse > 256 * 1048576)
                    {
                        NvmlNativeMethods.nvmlShutdown();
                        using (EventLog eventLog = new EventLog("Application"))
                        {
                            eventLog.Source = "NvidiaGPUGetDataHost";
                            eventLog.WriteEntry("Mem leak. Restart", EventLogEntryType.Warning, 101, 1);
                        }
                        System.Windows.Forms.Application.Restart();
                        System.Environment.Exit(1);
                    }
                    ticks++;
                } while (true);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "NvidiaGPUGetDataHost";
                    eventLog.WriteEntry("Exception: " + ex.ToString(), EventLogEntryType.Error, 101, 1);
                }
            }
        }
    }
}
