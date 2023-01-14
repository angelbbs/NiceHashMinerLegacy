#region Copyright

/*******************************************************************************
 Copyright(c) 2008 - 2009 Advanced Micro Devices, Inc. All Rights Reserved.
 Copyright (c) 2002 - 2006  ATI Technologies Inc. All Rights Reserved.

 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
 ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDED BUT NOT LIMITED TO
 THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 PARTICULAR PURPOSE.

 File:        ADL.cs

 Purpose:     Implements ADL interface

 Description: Implements some of the methods defined in ADL interface.

 ********************************************************************************/

#endregion Copyright

#region Using

using System;
using System.Runtime.InteropServices;
using FARPROC = System.IntPtr;
using HMODULE = System.IntPtr;
// ReSharper disable All
#pragma warning disable

#endregion Using

#region ATI.ADL

namespace ATI.ADL
{
    #region Export Delegates
    /// <summary> ADL Memory allocation function allows ADL to callback for memory allocation</summary>
    /// <param name="size">input size</param>
    /// <returns> retrun ADL Error Code</returns>
    internal delegate IntPtr ADL_Main_Memory_Alloc(int size);

    // ///// <summary> ADL Create Function to create ADL Data</summary>
    /// <param name="callback">Call back functin pointer which is ised to allocate memeory </param>
    /// <param name="enumConnectedAdapters">If it is 1, then ADL will only retuen the physical exist adapters </param>
    ///// <returns> retrun ADL Error Code</returns>
    internal delegate int ADL_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters);

    internal delegate int ADL2_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters, ref IntPtr context);

    /// <summary> ADL Destroy Function to free up ADL Data</summary>
    /// <returns> retrun ADL Error Code</returns>
    internal delegate int ADL_Main_Control_Destroy();

    internal delegate int ADL2_Main_Control_Destroy(IntPtr context);

    /// <summary> ADL Function to get the number of adapters</summary>
    /// <param name="numAdapters">return number of adapters</param>
    /// <returns> retrun ADL Error Code</returns>
    internal delegate int ADL_Adapter_NumberOfAdapters_Get(ref int numAdapters);

    /// <summary> ADL Function to get the GPU adapter information</summary>
    /// <param name="info">return GPU adapter information</param>
    /// <param name="inputSize">the size of the GPU adapter struct</param>
    /// <returns> retrun ADL Error Code</returns>
    internal delegate int ADL_Adapter_AdapterInfo_Get(IntPtr info, int inputSize);

    internal delegate int ADL2_Adapter_AdapterInfo_Get(IntPtr context, IntPtr lpInfo, int iInputSize);

    /// <summary> Function to determine if the adapter is active or not.</summary>
    /// <remarks>The function is used to check if the adapter associated with iAdapterIndex is active</remarks>
    /// <param name="adapterIndex"> Adapter Index.</param>
    /// <param name="status"> Status of the adapter. True: Active; False: Dsiabled</param>
    /// <returns>Non zero is successfull</returns>
    internal delegate int ADL_Adapter_Active_Get(int adapterIndex, ref int status);

    /// <summary>Get display information based on adapter index</summary>
    /// <param name="adapterIndex">Adapter Index</param>
    /// <param name="numDisplays">return the total number of supported displays</param>
    /// <param name="displayInfoArray">return ADLDisplayInfo Array for supported displays' information</param>
    /// <param name="forceDetect">force detect or not</param>
    /// <returns>return ADL Error Code</returns>
    internal delegate int ADL_Display_DisplayInfo_Get(int adapterIndex, ref int numDisplays, out IntPtr displayInfoArray, int forceDetect);

    internal delegate int ADL_Overdrive5_CurrentActivity_Get(int adapterIndex, ref ADLPMActivity activity);

    internal delegate int ADL_Overdrive5_Temperature_Get(int adapterIndex, int thermalControllerIndex, ref ADLTemperature temperature);

    internal delegate int ADL_Overdrive5_FanSpeed_Get(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedValue temperature);

    internal delegate int ADL2_OverdriveN_Temperature_Get(IntPtr context, int adapterIndex, ADLODNTemperatureType temperatureType, ref int temperature);

    internal delegate int ADL2_Overdrive6_CurrentPower_Get(IntPtr context, int adapterIndex, int iPowerType, ref int lpCurrentValue);
    
    internal delegate int ADL2_New_QueryPMLogData_Get(IntPtr context, int adapterIndex, ref ADLPMLogDataOutput aDLPMLogDataOutput);

    #endregion Export Delegates

    #region Export Struct

    #region ADLSensorType
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLSingleSensorData
    {
        public int supported;
        public int value;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLPMLogDataOutput
    {
        public int size;
        internal const int ADL_PMLOG_MAX_SENSORS = 256;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ADL_PMLOG_MAX_SENSORS)]
        public ADLSingleSensorData[] sensors;
    }

    public enum ADLSensorType
    {
        SENSOR_MAXTYPES = 0,
        PMLOG_CLK_GFXCLK = 1,
        PMLOG_CLK_MEMCLK = 2,
        PMLOG_CLK_SOCCLK = 3,
        PMLOG_CLK_UVDCLK1 = 4,
        PMLOG_CLK_UVDCLK2 = 5,
        PMLOG_CLK_VCECLK = 6,
        PMLOG_CLK_VCNCLK = 7,
        PMLOG_TEMPERATURE_EDGE = 8,
        PMLOG_TEMPERATURE_MEM = 9,
        PMLOG_TEMPERATURE_VRVDDC = 10,
        PMLOG_TEMPERATURE_VRMVDD = 11,
        PMLOG_TEMPERATURE_LIQUID = 12,
        PMLOG_TEMPERATURE_PLX = 13,
        PMLOG_FAN_RPM = 14,
        PMLOG_FAN_PERCENTAGE = 15,
        PMLOG_SOC_VOLTAGE = 16,
        PMLOG_SOC_POWER = 17,
        PMLOG_SOC_CURRENT = 18,
        PMLOG_INFO_ACTIVITY_GFX = 19,
        PMLOG_INFO_ACTIVITY_MEM = 20,
        PMLOG_GFX_VOLTAGE = 21,
        PMLOG_MEM_VOLTAGE = 22,
        PMLOG_ASIC_POWER = 23,
        PMLOG_TEMPERATURE_VRSOC = 24,
        PMLOG_TEMPERATURE_VRMVDD0 = 25,
        PMLOG_TEMPERATURE_VRMVDD1 = 26,
        PMLOG_TEMPERATURE_HOTSPOT = 27,
        PMLOG_TEMPERATURE_GFX = 28,
        PMLOG_TEMPERATURE_SOC = 29,
        PMLOG_GFX_POWER = 30,
        PMLOG_GFX_CURRENT = 31,
        PMLOG_TEMPERATURE_CPU = 32,
        PMLOG_CPU_POWER = 33,
        PMLOG_CLK_CPUCLK = 34,
        PMLOG_THROTTLER_STATUS = 35,
        PMLOG_CLK_VCN1CLK1 = 36,
        PMLOG_CLK_VCN1CLK2 = 37,
        PMLOG_SMART_POWERSHIFT_CPU = 38,
        PMLOG_SMART_POWERSHIFT_DGPU = 39,
        PMLOG_BUS_SPEED = 40,
        PMLOG_BUS_LANES = 41,
        PMLOG_TEMPERATURE_LIQUID0 = 42,
        PMLOG_TEMPERATURE_LIQUID1 = 43,
        PMLOG_CLK_FCLK = 44,
        PMLOG_THROTTLER_STATUS_CPU = 45,
        PMLOG_MAX_SENSORS_REAL
    }
    #endregion

    #region ADLAdapterInfo
    /// <summary> ADLAdapterInfo Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLAdapterInfo
    {
        /// <summary>The size of the structure</summary>
        int Size;
        /// <summary> Adapter Index</summary>
        internal int AdapterIndex;
        /// <summary> Adapter UDID</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string UDID;
        /// <summary> Adapter Bus Number</summary>
        internal int BusNumber;
        /// <summary> Adapter Driver Number</summary>
        internal int DriverNumber;
        /// <summary> Adapter Function Number</summary>
        internal int FunctionNumber;
        /// <summary> Adapter Vendor ID</summary>
        internal int VendorID;
        /// <summary> Adapter Adapter name</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string AdapterName;
        /// <summary> Adapter Display name</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DisplayName;
        /// <summary> Adapter Present status</summary>
        internal int Present;
        /// <summary> Adapter Exist status</summary>
        internal int Exist;
        /// <summary> Adapter Driver Path</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DriverPath;
        /// <summary> Adapter Driver Ext Path</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DriverPathExt;
        /// <summary> Adapter PNP String</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string PNPString;
        /// <summary> OS Display Index</summary>
        internal int OSDisplayIndex;
    }


    /// <summary> ADLAdapterInfo Array</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLAdapterInfoArray
    {
        /// <summary> ADLAdapterInfo Array </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)ADL.ADL_MAX_ADAPTERS)]
        internal ADLAdapterInfo[] ADLAdapterInfo;
    }
    #endregion ADLAdapterInfo


    #region ADLDisplayInfo
    /// <summary> ADLDisplayID Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLDisplayID
    {
        /// <summary> Display Logical Index </summary>
        internal int DisplayLogicalIndex;
        /// <summary> Display Physical Index </summary>
        internal int DisplayPhysicalIndex;
        /// <summary> Adapter Logical Index </summary>
        internal int DisplayLogicalAdapterIndex;
        /// <summary> Adapter Physical Index </summary>
        internal int DisplayPhysicalAdapterIndex;
    }

    /// <summary> ADLDisplayInfo Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLDisplayInfo
    {
        /// <summary> Display Index </summary>
        internal ADLDisplayID DisplayID;
        /// <summary> Display Controller Index </summary>
        internal int DisplayControllerIndex;
        /// <summary> Display Name </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DisplayName;
        /// <summary> Display Manufacturer Name </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL.ADL_MAX_PATH)]
        internal string DisplayManufacturerName;
        /// <summary> Display Type : < The Display type. CRT, TV,CV,DFP are some of display types,</summary>
        internal int DisplayType;
        /// <summary> Display output type </summary>
        internal int DisplayOutputType;
        /// <summary> Connector type</summary>
        internal int DisplayConnector;
        ///<summary> Indicating the display info bits' mask.<summary>
        internal int DisplayInfoMask;
        ///<summary> Indicating the display info value.<summary>
        internal int DisplayInfoValue;
    }
    #endregion ADLDisplayInfo

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLPMActivity
    {
        public int Size;
        public int EngineClock;
        public int MemoryClock;
        public int Vddc;
        /// <summary>
        /// GPU Utilization
        /// </summary>
        public int ActivityPercent;
        public int CurrentPerformanceLevel;
        public int CurrentBusSpeed;
        public int CurrentBusLanes;
        public int MaximumBusLanes;
        public int Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLTemperature
    {
        public int Size;
        /// <summary>
        /// Temperature in millidegrees Celsius
        /// </summary>
        public int Temperature;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLFanSpeedValue
    {
        public int Size;
        public int SpeedType;
        public int FanSpeed;
        public int Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLFanSpeedInfo
    {
        public int Size;
        public int Flags;
        public int MinPercent;
        public int MaxPercent;
        public int MinRPM;
        public int MaxRPM;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODParameterRange
    {
        public int Min;
        public int Max;
        public int Step;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODParameters
    {
        public int Size;
        public int NumberOfPerformanceLevels;
        public int ActivityReportingSupported;
        public int DiscretePerformanceLevels;
        public int Reserved;
        public ADLODParameterRange EngineClock;
        public ADLODParameterRange MemoryClock;
        public ADLODParameterRange Vddc;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNPerformanceStatus
    {
        public int CoreClock;
        public int MemoryClock;
        public int DCEFClock;
        public int GFXClock;
        public int UVDClock;
        public int VCEClock;
        public int GPUActivityPercent;
        public int CurrentCorePerformanceLevel;
        public int CurrentMemoryPerformanceLevel;
        public int CurrentDCEFPerformanceLevel;
        public int CurrentGFXPerformanceLevel;
        public int UVDPerformanceLevel;
        public int VCEPerformanceLevel;
        public int CurrentBusSpeed;
        public int CurrentBusLanes;
        public int MaximumBusLanes;
        public int VDDC;
        public int VDDCI;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLVersionsInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string DriverVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string CatalystVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string CatalystWebLink;
    }

    internal enum ADLODNCurrentPowerType
    {
        TOTAL_POWER = 0,
        PPT_POWER,
        SOCKET_POWER,
        CHIP_POWER,
    }

    internal enum ADLODNTemperatureType
    {
        CORE = 1,
        MEMORY = 2,
        VRM_CORE = 3,
        VRM_MEMORY = 4,
        LIQUID = 5,
        PLX = 6,
        HOTSPOT = 7,
    }

    internal enum ADLStatus : int
    {
        /// <summary>
        /// All OK, but need to wait.
        /// </summary>  
        OK_WAIT = 4,

        /// <summary>
        /// All OK, but need restart.
        /// </summary>  
        OK_RESTART = 3,

        /// <summary>
        /// All OK but need mode change.
        /// </summary>
        OK_MODE_CHANGE = 2,

        /// <summary>
        /// All OK, but with warning.
        /// </summary>
        OK_WARNING = 1,

        /// <summary>
        /// ADL function completed successfully.
        /// </summary>
        OK = 0,

        /// <summary>
        /// Generic Error. Most likely one or more of the Escape calls to the driver 
        /// failed!
        /// </summary>
        ERR = -1,

        /// <summary>
        /// ADL not initialized.
        /// </summary>
        ERR_NOT_INIT = -2,

        /// <summary>
        /// One of the parameter passed is invalid.
        /// </summary>
        ERR_INVALID_PARAM = -3,

        /// <summary>
        /// One of the parameter size is invalid.
        /// </summary>
        ERR_INVALID_PARAM_SIZE = -4,

        /// <summary>
        /// Invalid ADL index passed.
        /// </summary>
        ERR_INVALID_ADL_IDX = -5,

        /// <summary>
        /// Invalid controller index passed.
        /// </summary>
        ERR_INVALID_CONTROLLER_IDX = -6,

        /// <summary>
        /// Invalid display index passed.
        /// </summary>
        ERR_INVALID_DIPLAY_IDX = -7,

        /// <summary>
        /// Function not supported by the driver.
        /// </summary>
        ERR_NOT_SUPPORTED = -8,

        /// <summary>
        /// Null Pointer error.
        /// </summary>
        ERR_NULL_POINTER = -9,

        /// <summary>
        /// Call can't be made due to disabled adapter.
        /// </summary>
        ERR_DISABLED_ADAPTER = -10,

        /// <summary>
        /// Invalid Callback.
        /// </summary>
        ERR_INVALID_CALLBACK = -11,

        /// <summary>
        /// Display Resource conflict.
        /// </summary>
        ERR_RESOURCE_CONFLICT = -12,

        /// <summary>
        /// Failed to update some of the values. Can be returned by set request that 
        /// include multiple values if not all values were successfully committed.
        /// </summary>
        ERR_SET_INCOMPLETE = -20,

        /// <summary>
        /// There's no Linux XDisplay in Linux Console environment. 
        /// </summary>
        ERR_NO_XDISPLAY = -21
    }


    #endregion Export Struct

    #region ADL Class
    /// <summary> ADL Class</summary>
    internal static class ADL
    {
        #region Internal Constant
        /// <summary> Define the maximum path</summary>
        internal const int ADL_MAX_PATH = 256;
        /// <summary> Define the maximum adapters</summary>
        internal const int ADL_MAX_ADAPTERS = 250;
        /// <summary> Define the maximum displays</summary>
        internal const int ADL_MAX_DISPLAYS = 40 /* 150 */;
        /// <summary> Define the maximum device name length</summary>
        internal const int ADL_MAX_DEVICENAME = 32;
        /// <summary> Define the successful</summary>
        internal const int ADL_SUCCESS = 0;
        /// <summary> Define the failure</summary>
        internal const int ADL_FAIL = -1;
        internal const int ADL_ERR_INVALID_ADL_IDX = -5;
        internal const int ADL_NOT_SUPPORTED = -8;
        /// <summary> Define the driver ok</summary>
        internal const int ADL_DRIVER_OK = 0;
        /// <summary> Maximum number of GL-Sync ports on the GL-Sync module </summary>
        internal const int ADL_MAX_GLSYNC_PORTS = 8;
        /// <summary> Maximum number of GL-Sync ports on the GL-Sync module </summary>
        internal const int ADL_MAX_GLSYNC_PORT_LEDS = 8;
        /// <summary> Maximum number of ADLMOdes for the adapter </summary>
        internal const int ADL_MAX_NUM_DISPLAYMODES = 1024;

        internal const int ADL_DL_FANCTRL_SPEED_TYPE_PERCENT = 1;
        internal const int ADL_DL_FANCTRL_SPEED_TYPE_RPM = 2;
        public const int ADL_DL_FANCTRL_SUPPORTS_PERCENT_READ = 1;
        public const int ADL_DL_FANCTRL_SUPPORTS_PERCENT_WRITE = 2;
        public const int ADL_DL_FANCTRL_SUPPORTS_RPM_READ = 4;
        public const int ADL_DL_FANCTRL_SUPPORTS_RPM_WRITE = 8;
        public const int ADL_DL_FANCTRL_FLAG_USER_DEFINED_SPEED = 1;
        #endregion Internal Constant

        #region Class ADLImport
        /// <summary> ADLImport class</summary>
        private static class ADLImport
        {
            #region Internal Constant
            /// <summary> Atiadlxx_FileName </summary>
            internal const string Atiadlxx_FileName = "atiadlxx.dll";
            /// <summary> Kernel32_FileName </summary>
            internal const string Kernel32_FileName = "kernel32.dll";
            #endregion Internal Constant

            #region DLLImport
            [DllImport(Kernel32_FileName, CallingConvention = CallingConvention.StdCall)]
            internal static extern HMODULE GetModuleHandle(string moduleName);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL2_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters, ref IntPtr context);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Main_Control_Destroy();

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL2_Main_Control_Destroy(IntPtr context);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Main_Control_IsFunctionValid(HMODULE module, string procName);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern FARPROC ADL_Main_Control_GetProcAddress(HMODULE module, string procName);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Adapter_NumberOfAdapters_Get(ref int numAdapters);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Adapter_AdapterInfo_Get(IntPtr info, int inputSize);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL2_Adapter_AdapterInfo_Get(IntPtr context, IntPtr lpInfo, int iInputSize);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Adapter_Active_Get(int adapterIndex, ref int status);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Display_DisplayInfo_Get(int adapterIndex, ref int numDisplays, out IntPtr displayInfoArray, int forceDetect);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Overdrive5_CurrentActivity_Get(int iAdapterIndex, ref ADLPMActivity activity);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Overdrive5_Temperature_Get(int adapterIndex, int thermalControllerIndex, ref ADLTemperature temperature);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL2_OverdriveN_Temperature_Get(IntPtr context, int adapterIndex, ADLODNTemperatureType temperatureType, ref int temperature);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Overdrive5_FanSpeed_Get(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedValue fanSpeedValue);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL2_Overdrive6_CurrentPower_Get(IntPtr context, int adapterIndex, int iPowerType, ref int lpCurrentValue);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL2_New_QueryPMLogData_Get(IntPtr context, int adapterIndex, ref ADLPMLogDataOutput aDLPMLogDataOutput);
            #endregion DLLImport
        }
        #endregion Class ADLImport

        #region Class ADLCheckLibrary
        /// <summary> ADLCheckLibrary class</summary>
        private class ADLCheckLibrary
        {
            #region Private Members
            private HMODULE ADLLibrary = System.IntPtr.Zero;
            #endregion Private Members

            #region Static Members
            /// <summary> new a private instance</summary>
            private static ADLCheckLibrary ADLCheckLibrary_ = new ADLCheckLibrary();
            #endregion Static Members

            #region Constructor
            /// <summary> Constructor</summary>
            private ADLCheckLibrary()
            {
                try
                {
                    if (1 == ADLImport.ADL_Main_Control_IsFunctionValid(IntPtr.Zero, "ADL_Main_Control_Create"))
                    {
                        ADLLibrary = ADLImport.GetModuleHandle(ADLImport.Atiadlxx_FileName);
                    }
                }
                catch (DllNotFoundException) { }
                catch (EntryPointNotFoundException) { }
                catch (Exception) { }
            }
            #endregion Constructor

            #region Destructor
            /// <summary> Destructor to force calling ADL Destroy function before free up the ADL library</summary>
            ~ADLCheckLibrary()
            {
                if (System.IntPtr.Zero != ADLCheckLibrary_.ADLLibrary)
                {
                    ADLImport.ADL_Main_Control_Destroy();
                }
            }
            #endregion Destructor

            #region Static IsFunctionValid
            /// <summary> Check the import function to see it exists or not</summary>
            /// <param name="functionName"> function name</param>
            /// <returns>return true, if function exists</returns>
            internal static bool IsFunctionValid(string functionName)
            {
                bool result = false;
                if (System.IntPtr.Zero != ADLCheckLibrary_.ADLLibrary)
                {
                    if (1 == ADLImport.ADL_Main_Control_IsFunctionValid(ADLCheckLibrary_.ADLLibrary, functionName))
                    {
                        result = true;
                    }
                }
                return result;
            }
            #endregion Static IsFunctionValid

            #region Static GetProcAddress
            /// <summary> Get the unmanaged function pointer </summary>
            /// <param name="functionName"> function name</param>
            /// <returns>return function pointer, if function exists</returns>
            internal static FARPROC GetProcAddress(string functionName)
            {
                FARPROC result = System.IntPtr.Zero;
                if (System.IntPtr.Zero != ADLCheckLibrary_.ADLLibrary)
                {
                    result = ADLImport.ADL_Main_Control_GetProcAddress(ADLCheckLibrary_.ADLLibrary, functionName);
                }
                return result;
            }
            #endregion Static GetProcAddress
        }
        #endregion Class ADLCheckLibrary

        #region Export Functions

        #region ADL_Main_Memory_Alloc
        /// <summary> Build in memory allocation function</summary>
        internal static ADL_Main_Memory_Alloc ADL_Main_Memory_Alloc = ADL_Main_Memory_Alloc_;
        /// <summary> Build in memory allocation function</summary>
        /// <param name="size">input size</param>
        /// <returns>return the memory buffer</returns>
        private static IntPtr ADL_Main_Memory_Alloc_(int size)
        {
            IntPtr result = Marshal.AllocCoTaskMem(size);
            return result;
        }
        #endregion ADL_Main_Memory_Alloc

        #region ADL_Main_Memory_Free
        /// <summary> Build in memory free function</summary>
        /// <param name="buffer">input buffer</param>
        internal static void ADL_Main_Memory_Free(IntPtr buffer)
        {
            if (IntPtr.Zero != buffer)
            {
                Marshal.FreeCoTaskMem(buffer);
            }
        }
        #endregion ADL_Main_Memory_Free

        #region ADL_Main_Control_Create
        /// <summary> ADL_Main_Control_Create Delegates</summary>
        internal static ADL_Main_Control_Create ADL_Main_Control_Create
        {
            get
            {
                if (!ADL_Main_Control_Create_Check && null == ADL_Main_Control_Create_)
                {
                    ADL_Main_Control_Create_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Main_Control_Create"))
                    {
                        ADL_Main_Control_Create_ = ADLImport.ADL_Main_Control_Create;
                    }
                }
                return ADL_Main_Control_Create_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Main_Control_Create ADL_Main_Control_Create_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Main_Control_Create_Check = false;
        /// <summary> ADL_Main_Control_Create Delegates</summary>
        internal static ADL2_Main_Control_Create ADL2_Main_Control_Create
        {
            get
            {
                if (!ADL2_Main_Control_Create_Check && null == ADL2_Main_Control_Create_)
                {
                    ADL2_Main_Control_Create_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Main_Control_Create"))
                    {
                        ADL2_Main_Control_Create_ = ADLImport.ADL2_Main_Control_Create;
                    }
                }
                return ADL2_Main_Control_Create_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_Main_Control_Create ADL2_Main_Control_Create_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_Main_Control_Create_Check = false;
        #endregion ADL_Main_Control_Create

        #region ADL_Main_Control_Destroy
        /// <summary> ADL_Main_Control_Destroy Delegates</summary>
        internal static ADL_Main_Control_Destroy ADL_Main_Control_Destroy
        {
            get
            {
                if (!ADL_Main_Control_Destroy_Check && null == ADL_Main_Control_Destroy_)
                {
                    ADL_Main_Control_Destroy_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Main_Control_Destroy"))
                    {
                        ADL_Main_Control_Destroy_ = ADLImport.ADL_Main_Control_Destroy;
                    }
                }
                return ADL_Main_Control_Destroy_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Main_Control_Destroy ADL_Main_Control_Destroy_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Main_Control_Destroy_Check = false;
        internal static ADL2_Main_Control_Destroy ADL2_Main_Control_Destroy
        {
            get
            {
                if (!ADL2_Main_Control_Destroy_Check && null == ADL2_Main_Control_Destroy_)
                {
                    ADL2_Main_Control_Destroy_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Main_Control_Destroy"))
                    {
                        ADL2_Main_Control_Destroy_ = ADLImport.ADL2_Main_Control_Destroy;
                    }
                }
                return ADL2_Main_Control_Destroy_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_Main_Control_Destroy ADL2_Main_Control_Destroy_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_Main_Control_Destroy_Check = false;
        #endregion ADL_Main_Control_Destroy

        #region ADL_Adapter_NumberOfAdapters_Get
        /// <summary> ADL_Adapter_NumberOfAdapters_Get Delegates</summary>
        internal static ADL_Adapter_NumberOfAdapters_Get ADL_Adapter_NumberOfAdapters_Get
        {
            get
            {
                if (!ADL_Adapter_NumberOfAdapters_Get_Check && null == ADL_Adapter_NumberOfAdapters_Get_)
                {
                    ADL_Adapter_NumberOfAdapters_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_NumberOfAdapters_Get"))
                    {
                        ADL_Adapter_NumberOfAdapters_Get_ = ADLImport.ADL_Adapter_NumberOfAdapters_Get;
                    }
                }
                return ADL_Adapter_NumberOfAdapters_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Adapter_NumberOfAdapters_Get ADL_Adapter_NumberOfAdapters_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Adapter_NumberOfAdapters_Get_Check = false;
        #endregion ADL_Adapter_NumberOfAdapters_Get

        #region ADL_Adapter_AdapterInfo_Get
        /// <summary> ADL_Adapter_AdapterInfo_Get Delegates</summary>
        internal static ADL_Adapter_AdapterInfo_Get ADL_Adapter_AdapterInfo_Get
        {
            get
            {
                if (!ADL_Adapter_AdapterInfo_Get_Check && null == ADL_Adapter_AdapterInfo_Get_)
                {
                    ADL_Adapter_AdapterInfo_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_AdapterInfo_Get"))
                    {
                        ADL_Adapter_AdapterInfo_Get_ = ADLImport.ADL_Adapter_AdapterInfo_Get;
                    }
                }
                return ADL_Adapter_AdapterInfo_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Adapter_AdapterInfo_Get ADL_Adapter_AdapterInfo_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Adapter_AdapterInfo_Get_Check = false;

        /// <summary> ADL_Adapter_AdapterInfo_Get Delegates</summary>
        internal static ADL2_Adapter_AdapterInfo_Get ADL2_Adapter_AdapterInfo_Get
        {
            get
            {
                if (!ADL2_Adapter_AdapterInfo_Get_Check && null == ADL2_Adapter_AdapterInfo_Get_)
                {
                    ADL2_Adapter_AdapterInfo_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_AdapterInfo_Get"))
                    {
                        ADL2_Adapter_AdapterInfo_Get_ = ADLImport.ADL2_Adapter_AdapterInfo_Get;
                    }
                }
                return ADL2_Adapter_AdapterInfo_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_Adapter_AdapterInfo_Get ADL2_Adapter_AdapterInfo_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_Adapter_AdapterInfo_Get_Check = false;
        #endregion ADL_Adapter_AdapterInfo_Get

        #region ADL_Adapter_Active_Get
        /// <summary> ADL_Adapter_Active_Get Delegates</summary>
        internal static ADL_Adapter_Active_Get ADL_Adapter_Active_Get
        {
            get
            {
                if (!ADL_Adapter_Active_Get_Check && null == ADL_Adapter_Active_Get_)
                {
                    ADL_Adapter_Active_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_Active_Get"))
                    {
                        ADL_Adapter_Active_Get_ = ADLImport.ADL_Adapter_Active_Get;
                    }
                }
                return ADL_Adapter_Active_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Adapter_Active_Get ADL_Adapter_Active_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Adapter_Active_Get_Check = false;
        #endregion ADL_Adapter_Active_Get

        #region ADL_Display_DisplayInfo_Get
        /// <summary> ADL_Display_DisplayInfo_Get Delegates</summary>
        internal static ADL_Display_DisplayInfo_Get ADL_Display_DisplayInfo_Get
        {
            get
            {
                if (!ADL_Display_DisplayInfo_Get_Check && null == ADL_Display_DisplayInfo_Get_)
                {
                    ADL_Display_DisplayInfo_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Display_DisplayInfo_Get"))
                    {
                        ADL_Display_DisplayInfo_Get_ = ADLImport.ADL_Display_DisplayInfo_Get;
                    }
                }
                return ADL_Display_DisplayInfo_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Display_DisplayInfo_Get ADL_Display_DisplayInfo_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Display_DisplayInfo_Get_Check = false;
        #endregion ADL_Display_DisplayInfo_Get

        internal static ADL_Overdrive5_CurrentActivity_Get ADL_Overdrive5_CurrentActivity_Get
        {
            get
            {
                if (!ADL_Overdrive5_CurrentActivity_Get_Check && null == ADL_Overdrive5_CurrentActivity_Get_)
                {
                    ADL_Overdrive5_CurrentActivity_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_CurrentActivity_Get"))
                    {
                        ADL_Overdrive5_CurrentActivity_Get_ = ADLImport.ADL_Overdrive5_CurrentActivity_Get;
                    }
                }
                return ADL_Overdrive5_CurrentActivity_Get_;
            }
        }
        private static ADL_Overdrive5_CurrentActivity_Get ADL_Overdrive5_CurrentActivity_Get_ = null;
        private static bool ADL_Overdrive5_CurrentActivity_Get_Check = false;

        internal static ADL_Overdrive5_Temperature_Get ADL_Overdrive5_Temperature_Get
        {
            get
            {
                if (!ADL_Overdrive5_Temperature_Get_Check && null == ADL_Overdrive5_Temperature_Get_)
                {
                    ADL_Overdrive5_Temperature_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_Temperature_Get"))
                    {
                        ADL_Overdrive5_Temperature_Get_ = ADLImport.ADL_Overdrive5_Temperature_Get;
                    }
                }
                return ADL_Overdrive5_Temperature_Get_;
            }
        }
        private static ADL_Overdrive5_Temperature_Get ADL_Overdrive5_Temperature_Get_ = null;
        private static bool ADL_Overdrive5_Temperature_Get_Check = false;

        internal static ADL2_OverdriveN_Temperature_Get ADL2_OverdriveN_Temperature_Get
        {
            get
            {
                if (!ADL2_OverdriveN_Temperature_Get_Check && null == ADL2_OverdriveN_Temperature_Get_)
                {
                    ADL2_OverdriveN_Temperature_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_OverdriveN_Temperature_Get"))
                    {
                        ADL2_OverdriveN_Temperature_Get_ = ADLImport.ADL2_OverdriveN_Temperature_Get;
                    }
                }

                return ADL2_OverdriveN_Temperature_Get_;
            }
        }

        private static ADL2_OverdriveN_Temperature_Get ADL2_OverdriveN_Temperature_Get_ = null;
        private static bool ADL2_OverdriveN_Temperature_Get_Check = false;


        internal static ADL_Overdrive5_FanSpeed_Get ADL_Overdrive5_FanSpeed_Get
        {
            get
            {
                if (!ADL_Overdrive5_FanSpeed_Get_Check && null == ADL_Overdrive5_FanSpeed_Get_)
                {
                    ADL_Overdrive5_FanSpeed_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_FanSpeed_Get"))
                    {
                        ADL_Overdrive5_FanSpeed_Get_ = ADLImport.ADL_Overdrive5_FanSpeed_Get;
                    }
                }
                return ADL_Overdrive5_FanSpeed_Get_;
            }
        }
        private static ADL_Overdrive5_FanSpeed_Get ADL_Overdrive5_FanSpeed_Get_ = null;
        private static bool ADL_Overdrive5_FanSpeed_Get_Check = false;

        internal static ADL2_New_QueryPMLogData_Get ADL2_New_QueryPMLogData_Get
        {
            get
            {
                if (!ADL2_New_QueryPMLogData_Get_Check && null == ADL2_New_QueryPMLogData_Get_)
                {
                    ADL2_New_QueryPMLogData_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_New_QueryPMLogData_Get"))
                    {
                        ADL2_New_QueryPMLogData_Get_ = ADLImport.ADL2_New_QueryPMLogData_Get;
                    }
                }

                return ADL2_New_QueryPMLogData_Get_;
            }
        }

        private static ADL2_New_QueryPMLogData_Get ADL2_New_QueryPMLogData_Get_ = null;
        private static bool ADL2_New_QueryPMLogData_Get_Check = false;

        internal static ADL2_Overdrive6_CurrentPower_Get ADL2_Overdrive6_CurrentPower_Get
        {
            get
            {
                if (!ADL2_Overdrive6_CurrentPower_Get_Check && null == ADL2_Overdrive6_CurrentPower_Get_)
                {
                    ADL2_Overdrive6_CurrentPower_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_Overdrive6_CurrentPower_Get"))
                    {
                        ADL2_Overdrive6_CurrentPower_Get_ = ADLImport.ADL2_Overdrive6_CurrentPower_Get;
                    }
                }

                return ADL2_Overdrive6_CurrentPower_Get_;
            }
        }

        private static ADL2_Overdrive6_CurrentPower_Get ADL2_Overdrive6_CurrentPower_Get_ = null;
        private static bool ADL2_Overdrive6_CurrentPower_Get_Check = false;

        #endregion Export Functions
    }
    #endregion ADL Class
}

#endregion ATI.ADL
