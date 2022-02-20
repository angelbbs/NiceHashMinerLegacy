using ManagedCuda.Nvml;
using NvAPIWrapper.Native;
using NvidiaGPUGetDataHost.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
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
            public uint loadMem;
            public uint temp;
            public uint tempMem;
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

        private static Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("log4net")) //имя dll
                return Assembly.Load(Resources.log4net); //dll в ресурсах
            return null;
        }
        [STAThread]
        public static void Main()
        {
            if (System.Diagnostics.Process.GetProcessesByName(Application.ProductName).Length > 1)
            {
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += AppDomain_AssemblyResolve;

            Logger.ConfigureWithFile();
            Logger.ConsolePrint("NvidiaGPUGetDataHost", "Start");
            try
            {
                uint devCount = 0;
                nvmlReturn ret;
                var pathVar = Environment.GetEnvironmentVariable("PATH");
                pathVar += ";" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) +
                               "\\NVIDIA Corporation\\NVSMI"; ;
                Environment.SetEnvironmentVariable("PATH", pathVar);
                nvmlDevice _nvmlDevice = new nvmlDevice();
                nvmlReturn nvmlLoaded = NvmlNativeMethods.nvmlInit();
                if (nvmlLoaded != nvmlReturn.Success)
                {
                    Logger.ConsolePrint("NvidiaGPUGetDataHost", "NVSMI Error: " + nvmlLoaded);
                    return;
                }

                ret = NvmlNativeMethods.nvmlDeviceGetCount(ref devCount);

                if (ret != nvmlReturn.Success)
                {
                    Logger.ConsolePrint("NvidiaGPUGetDataHost", "nvmlDeviceGetCount error: " + ret.ToString());
                    return;
                }
                Logger.ConsolePrint("NvidiaGPUGetDataHost", "NVIDIA devices: " + devCount.ToString());

                List<NvData> gpuList = new List<NvData>();

                int ticks = 0;
                int errors = 0;

                int devn = 0;
                var _power = 0u;
                var _fan = 0u;
                var _load = 0u;
                var _loadMem = 0u;
                var _temp = 0u;
                var _tempMem = 0u;

                int size = Marshal.SizeOf(devn) + Marshal.SizeOf(_power) + Marshal.SizeOf(_fan) + Marshal.SizeOf(_load) + Marshal.SizeOf(_loadMem) + Marshal.SizeOf(_temp) + Marshal.SizeOf(_tempMem);

                MemoryMappedFile sharedMemory = MemoryMappedFile.CreateOrOpen("NvidiaGPUGetDataHost", size * devCount + Marshal.SizeOf(devCount));
                do
                {
                    for (int dev = 0; dev < devCount; dev++)
                    {
                        /*
                        ret = NvmlNativeMethods.nvmlDeviceGetCount(ref devCount);
                        if (ret != nvmlReturn.Success)
                        {
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "NvidiaGPUGetDataHost";
                                eventLog.WriteEntry("nvmlDeviceGetCount error: " + ret.ToString(), EventLogEntryType.Error, 101, 1);
                            }
                            return;
                        }
                        */
                        ret = NvmlNativeMethods.nvmlDeviceGetHandleByIndex((uint)dev, ref _nvmlDevice);
                        if (ret != nvmlReturn.Success)
                        {
                            Logger.ConsolePrint("NvidiaGPUGetDataHost", "nvmlDeviceGetHandleByIndex error: " + ret.ToString());
                            if (!ret.ToString().Contains("NotSupported"))
                            {
                                errors++;
                                //break;
                            }
                        }
                        Thread.Sleep(50);
                        ret = NvmlNativeMethods.nvmlDeviceGetPowerUsage(_nvmlDevice, ref _power);// <- mem leak 461.40+
                        if (ret != nvmlReturn.Success)
                        {
                            if (!ret.ToString().Contains("NotSupported"))
                            {
                                Logger.ConsolePrint("NvidiaGPUGetDataHost", "nvmlDeviceGetPowerUsage error: " + ret.ToString());
                                errors++;
                            }
                            //break;
                        }
                        Thread.Sleep(50);
                        ret = NvmlNativeMethods.nvmlDeviceGetFanSpeed(_nvmlDevice, ref _fan);
                        if (ret != nvmlReturn.Success)
                        {
                            if (!ret.ToString().Contains("NotSupported"))
                            {
                                Logger.ConsolePrint("NvidiaGPUGetDataHost", "nvmlDeviceGetFanSpeed error: " + ret.ToString());
                                errors++;
                            }
                            //break;
                        }
                        Thread.Sleep(50);
                        var rates = new nvmlUtilization();
                        ret = NvmlNativeMethods.nvmlDeviceGetUtilizationRates(_nvmlDevice, ref rates);
                        if (ret != nvmlReturn.Success)
                        {
                            if (!ret.ToString().Contains("NotSupported"))
                            {
                                Logger.ConsolePrint("NvidiaGPUGetDataHost", "nvmlDeviceGetUtilizationRates error: " + ret.ToString());
                            }
                            //break;
                        }
                        Thread.Sleep(50);
                        _load = rates.gpu;
                        _loadMem = rates.memory;
                        
                        ret = NvmlNativeMethods.nvmlDeviceGetTemperature(_nvmlDevice, nvmlTemperatureSensors.Gpu, ref _temp);
                        if (ret != nvmlReturn.Success)
                        {
                            if (!ret.ToString().Contains("NotSupported"))
                            {
                                Logger.ConsolePrint("NvidiaGPUGetDataHost", "nvmlDeviceGetTemperature(Gpu) error: " + ret.ToString());
                                errors++;
                            }
                        }
                        Thread.Sleep(50);

                        var gpus = NvAPIWrapper.GPU.PhysicalGPU.GetPhysicalGPUs();
                        var sorted = gpus.OrderBy(x => x.GPUId).ToArray();

                        /*
                        for (int i=0; i<gpus.Length;i++)
                        {
                            var gpu = gpus[i];
                            Logger.ConsolePrint("NvidiaGPUGetDataHost", gpu.FullName + " GPUId: " + gpu.GPUId.ToString());
                        }
                        */
                        var gpu = sorted[dev];
                        NvmlNativeMethods.nvmlDeviceGetName(_nvmlDevice, out string name);
                        //Logger.ConsolePrint("NvidiaGPUGetDataHost", "dev: " + dev + " nvml.name: " + name + " api.FullName: " + gpu.FullName + " api.GPUId: " + gpu.GPUId.ToString());
                        var handle = GPUApi.GetPhysicalGPUFromGPUID(gpu.GPUId);
                        // find bits
                        var maxBit = 0;
                        for (; maxBit < 32; maxBit++)
                        {
                            try
                            {
                                GPUApi.QueryThermalSensors(handle, 1u << maxBit);
                            }
                            catch
                            {
                                break;
                            }
                        }
                        //Logger.ConsolePrint("NvidiaGPUGetDataHost", "maxBit: " + maxBit.ToString());
                        if (maxBit == 0)
                        {
                            return;
                        }

                        float[] t1 = new float[maxBit];
                        try
                        {
                            var temp = GPUApi.QueryThermalSensors(handle, (1u << maxBit) - 1);
                            t1 = temp.Temperatures;
                        }
                        catch
                        {
                            // ignore
                        }
                        _tempMem = (uint)t1[9];// 2-hotspot, 9-mem
                        /*
                        for (int i = 0; i < t1.Length; i++)
                        {
                            Logger.ConsolePrint("NvidiaGPUGetDataHost", "i: " + i + " " + t1[i].ToString());
                        }
                        */
                        //  Thread.Sleep(500);

                        Thread.Sleep(50);

                        using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(0, size * devCount + Marshal.SizeOf(devCount)))
                        {
                            //if (ret == nvmlReturn.Success)

                            {
                                writer.WriteArray<byte>(0, RawSerialize(devCount), 0, Marshal.SizeOf(devCount));
                            }
                            /*
                            else
                            {
                                writer.WriteArray<byte>(0, RawSerialize(0), 0, Marshal.SizeOf(devCount));
                            }
                            */
                            writer.WriteArray<byte>(size * dev + Marshal.SizeOf(devCount), BitConverter.GetBytes(Convert.ToInt32((long)_nvmlDevice.Pointer % Int32.MaxValue)), 0, Marshal.SizeOf(dev));
                            writer.WriteArray<byte>(size * dev + Marshal.SizeOf(devCount) + Marshal.SizeOf(dev), BitConverter.GetBytes(_power), 0, Marshal.SizeOf(_power));
                            writer.WriteArray<byte>(size * dev + Marshal.SizeOf(devCount) + Marshal.SizeOf(dev) + Marshal.SizeOf(_power), BitConverter.GetBytes(_fan), 0, Marshal.SizeOf(_fan));
                            writer.WriteArray<byte>(size * dev + Marshal.SizeOf(devCount) + Marshal.SizeOf(dev) + Marshal.SizeOf(_power) + Marshal.SizeOf(_fan), BitConverter.GetBytes(_load), 0, Marshal.SizeOf(_load));
                            writer.WriteArray<byte>(size * dev + Marshal.SizeOf(devCount) + Marshal.SizeOf(dev) + Marshal.SizeOf(_power) + Marshal.SizeOf(_fan) + Marshal.SizeOf(_load), BitConverter.GetBytes(_loadMem), 0, Marshal.SizeOf(_loadMem));
                            writer.WriteArray<byte>(size * dev + Marshal.SizeOf(devCount) + Marshal.SizeOf(dev) + Marshal.SizeOf(_power) + Marshal.SizeOf(_fan) + Marshal.SizeOf(_load) + Marshal.SizeOf(_loadMem), BitConverter.GetBytes(_temp), 0, Marshal.SizeOf(_temp));
                            writer.WriteArray<byte>(size * dev + Marshal.SizeOf(devCount) + Marshal.SizeOf(dev) + Marshal.SizeOf(_power) + Marshal.SizeOf(_fan) + Marshal.SizeOf(_load) + Marshal.SizeOf(_loadMem) + Marshal.SizeOf(_temp), BitConverter.GetBytes(_tempMem), 0, Marshal.SizeOf(_tempMem));
                        }
                    }
                    Thread.Sleep(200);
                    //sharedMemory.Dispose();

                    Process currentProc = Process.GetCurrentProcess();
                    double bytesInUse = currentProc.PrivateMemorySize64;
                    if (ticks > 120)
                    {
                        GC.Collect();
                    }
                    if (bytesInUse > 256 * 1048576)
                    {
                        NvmlNativeMethods.nvmlShutdown();
                        Logger.ConsolePrint("NvidiaGPUGetDataHost", "Memory leak exceeded limit in 256MB. Closing");
                        //System.Windows.Forms.Application.Restart();
                        System.Environment.Exit(1);
                    }
                    if (errors > 10)
                    {
                        NvmlNativeMethods.nvmlShutdown();

                        Logger.ConsolePrint("NvidiaGPUGetDataHost", "Too many errors. Closing");
                        //System.Windows.Forms.Application.Restart();
                        System.Environment.Exit(1);
                    }
                    ticks++;
                } while (true);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Logger.ConsolePrint("NvidiaGPUGetDataHost", "Exception: " + ex.ToString());
            }
        }

    }
}
