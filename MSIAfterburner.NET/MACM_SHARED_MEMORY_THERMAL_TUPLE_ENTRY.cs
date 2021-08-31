using System;
using System.Runtime.InteropServices;

namespace MSI.Afterburner
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MACM_SHARED_MEMORY_THERMAL_TUPLE_ENTRY
    {
        public uint dwTemperatureCur;
        //current temperature in C
        public uint dwTemperatureDef;
        //default temperature in C
        public uint dwFrequencyCur;
        //current frequency in KHz
        public uint dwFrequencyDef;
        //default frequency in KHz
    }
}
