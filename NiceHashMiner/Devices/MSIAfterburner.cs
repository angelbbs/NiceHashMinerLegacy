using MSI.Afterburner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceHashMiner.Devices
{
    public static class MSIAfterburner
    {
        public static ControlMemory macm;
        public static HardwareMonitor mahm;
        public static bool MSIAfterburnerInit()
        {
            try
            {
                ControlMemory macm = new ControlMemory();
                HardwareMonitor mahm = new HardwareMonitor();
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("MSI AB error", ex.Message);
                if (ex.InnerException != null)
                    Helpers.ConsolePrint("MSI AB error", ex.InnerException.Message);
                return false;
            }
            return true;
        }
    }
}
