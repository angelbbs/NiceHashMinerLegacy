using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceHashMiner.Configs.Data
{
    [Serializable]
    public class ZILConfigGMiner
    {
        public bool Autolykos_NVIDIA { get; set; }
        public bool AutolykosKHeavyHash_NVIDIA { get; set; }
        public bool AutolykosIronFish_NVIDIA { get; set; }
        public bool BeamV3_NVIDIA { get; set; }
        public bool CuckooCycle_NVIDIA { get; set; }
        public bool GrinCuckatoo32_NVIDIA { get; set; }
        public bool KAWPOW_NVIDIA { get; set; }
        public bool KHeavyHash_NVIDIA { get; set; }
        public bool IronFish_NVIDIA { get; set; }
        public bool Octopus_NVIDIA { get; set; }
        public bool OctopusKHeavyHash_NVIDIA { get; set; }
        public bool OctopusIronFish_NVIDIA { get; set; }
        public bool ZelHash_NVIDIA { get; set; }
        public bool ZHash_NVIDIA { get; set; }
        public bool KAWPOW_AMD { get; set; }
        public bool ZelHash_AMD { get; set; }
        public bool ZHash_AMD { get; set; }

        public ZILConfigGMiner()
        {
            Autolykos_NVIDIA = true;
            AutolykosKHeavyHash_NVIDIA = true;
            AutolykosIronFish_NVIDIA = true;
            BeamV3_NVIDIA = true;
            CuckooCycle_NVIDIA = true;
            GrinCuckatoo32_NVIDIA = true;
            KAWPOW_NVIDIA = true;
            KHeavyHash_NVIDIA = true;
            IronFish_NVIDIA = true;
            Octopus_NVIDIA = true;
            OctopusKHeavyHash_NVIDIA = true;
            OctopusIronFish_NVIDIA = true;
            ZelHash_NVIDIA = true;
            ZHash_NVIDIA = true;
            KAWPOW_AMD = true;
            ZelHash_AMD = true;
            ZHash_AMD = true;
        }
    }
    [Serializable]
    public class ZILConfigSRBMiner
    {
        public bool Autolykos_AMD { get; set; }
        public bool AutolykosKHeavyHash_AMD { get; set; }
        public bool KHeavyHash_AMD { get; set; }
        public ZILConfigSRBMiner()
        {
            Autolykos_AMD = true;
            AutolykosKHeavyHash_AMD = true;
            KHeavyHash_AMD = true;
        }
    }
    [Serializable]
    public class ZILConfigNanominer
    {
        public bool Autolykos_AMD { get; set; }
        public ZILConfigNanominer()
        {
            Autolykos_AMD = true;
        }
    }
    [Serializable]
    public class ZILConfigRigel
    {
        public bool Autolykos_NVIDIA { get; set; }
        public bool AutolykosKHeavyHash_NVIDIA { get; set; }
        public bool KHeavyHash_NVIDIA { get; set; }
        public bool Nexapow_NVIDIA { get; set; }
        public bool IronFish_NVIDIA { get; set; }
        public ZILConfigRigel()
        {
            Autolykos_NVIDIA = true;
            AutolykosKHeavyHash_NVIDIA = true;
            KHeavyHash_NVIDIA = true;
            Nexapow_NVIDIA = true;
            IronFish_NVIDIA = true;
        }
    }
    [Serializable]
    public class ZILConfigminiZ
    {
        public bool BeamV3_NVIDIA { get; set; }
        public bool ZelHash_NVIDIA { get; set; }
        public bool ZHash_NVIDIA { get; set; }
        public bool Octopus_NVIDIA { get; set; }
        public bool ZelHash_AMD { get; set; }
        public bool ZHash_AMD { get; set; }
        public ZILConfigminiZ()
        {
            BeamV3_NVIDIA = true;
            ZelHash_NVIDIA = true;
            ZHash_NVIDIA = true;
            Octopus_NVIDIA = true;
            ZelHash_AMD = true;
            ZHash_AMD = true;
        }
    }
}
