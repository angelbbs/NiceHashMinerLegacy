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
            //public int status;
            public uint nGpu;
            public uint power;
        }

        private readonly nvmlDevice _nvmlDevice;


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

                byte[] devArr = new byte[devCount * 2 + 1];
                devArr[0] = (byte)(devCount * 2 + 1);

                for (uint dev = 0; dev < devCount; dev++)
                {
                    devArr[dev * 2 + 1] = 0;
                    devArr[dev * 2 + 1] = 0;
                }

                int size = (int)devCount * 2 + 1;
                int ticks = 0;

                do
                {
                    for (int dev = 0; dev < devCount; dev++)
                    {
                        var _power = 0u;
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
                        else
                        {

                        }

                        devArr[dev * 2 + 1] = (byte)dev;
                        devArr[dev * 2 + 2] = (byte)(int)(_power / 1000);
                    }

                    MemoryMappedFile sharedMemory = MemoryMappedFile.CreateOrOpen("NvidiaGPUGetDataHost", size + 4);
                    using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(0, size + 4))
                    {
                        writer.WriteArray<byte>(0, devArr, 0, (int)(devCount * 2 + 1));
                        writer.Dispose();
                    }
                    Thread.Sleep(1000);
                    sharedMemory.Dispose();

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
