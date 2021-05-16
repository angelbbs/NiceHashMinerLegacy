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
    public unsafe struct MACM_SHARED_MEMORY_VF_POINT_ENTRY
    {
        public uint dwVoltageuV;
        //voltage in uV
        public uint dwFrequency;
        //current frequency in KHz (may change with temperature)
        public int dwFrequencyOffset;
        //frequency offset in KHz
    }
}
