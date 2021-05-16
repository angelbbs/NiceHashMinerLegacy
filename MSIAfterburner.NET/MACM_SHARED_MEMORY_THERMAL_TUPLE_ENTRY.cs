using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
