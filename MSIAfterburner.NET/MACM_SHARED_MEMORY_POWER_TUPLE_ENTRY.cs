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
    public unsafe struct MACM_SHARED_MEMORY_POWER_TUPLE_ENTRY
    {
        public uint dwPowerCur;
        //current power in %
        public uint dwPowerDef;
        //default power in %
        public uint dwFrequencyCur;
        //current frequency in KHz
        public uint dwFrequencyDef;
        //default frequency in KHz

    }
}
