using System;
using System.Runtime.InteropServices;

namespace MSI.Afterburner
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MACM_SHARED_MEMORY_VF_CURVE
    {
        public uint dwVersion;
        //reserved
        public uint dwFlags;
        //reserved

        public uint dwPoints;
        //3200+3*4=3212
        //number of voltage/frequency points
        //public MACM_SHARED_MEMORY_VF_POINT_ENTRY vfPoints[SharedMemory.MACM_SHARED_MEMORY_VF_CURVE_POINTS_MAX];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 960)]
        //public fixed byte vfPoints[SharedMemory.MACM_SHARED_MEMORY_VF_CURVE_POINTS_MAX * 4 * 3];//960
        public byte[] vfPoints;//960
        //voltage/frequency points array
        //2112
        public uint dwPowerTuples;
        //number of tuples in power/frequency curve
        //MACM_SHARED_MEMORY_POWER_TUPLE_ENTRY powerTuples[MACM_SHARED_MEMORY_VF_CURVE_TUPLES_MAX];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1120)]//неправильно
        //public fixed byte powerTuples[SharedMemory.MACM_SHARED_MEMORY_VF_CURVE_TUPLES_MAX * 4 * 4];//1120
        public byte[] powerTuples;//1120
        //power/frequency tuples array

        public uint dwThermalTuples;
        //number of tuples in thermal/frequency curve
        //MACM_SHARED_MEMORY_THERMAL_TUPLE_ENTRY thermalTuples[MACM_SHARED_MEMORY_VF_CURVE_TUPLES_MAX];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 984)]//неправильно
        //public fixed byte thermalTuples[SharedMemory.MACM_SHARED_MEMORY_VF_CURVE_TUPLES_MAX * 4 * 4];//1120
        public byte[] thermalTuples;//1120
        //thermal/frequency tuples array
        //public uint dwLockIndex;
        //index of locked point + 1 or zero if locking is disabled
    }
}
