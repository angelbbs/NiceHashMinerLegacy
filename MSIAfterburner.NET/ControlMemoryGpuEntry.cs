// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.ControlMemoryGpuEntry
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using MSI.Afterburner.Exceptions;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MSI.Afterburner
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class ControlMemoryGpuEntry : ISerializable, IXmlSerializable
    {
        internal MACM_SHARED_MEMORY_GPU_ENTRY macmGpuEntry;
        private bool isMaster;
        private int index = -1;

        //public MACM_SHARED_MEMORY_GPU_ENTRY_FLAG Flags => this.macmGpuEntry.flags;
        public MACM_SHARED_MEMORY_GPU_ENTRY_FLAG Flags
        {
            get => this.macmGpuEntry.flags;
            set => this.macmGpuEntry.flags = value;
        }

        public uint CoreClockCur
        {
            get => this.macmGpuEntry.coreClockCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_CLOCK) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_CLOCK)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support changing the core clock speed.");
                }
                //                    throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support changing the core clock speed.");
                if (value < this.CoreClockMin || value > this.CoreClockMax)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "CoreClockCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                else
                {
                    this.macmGpuEntry.coreClockCur = value;
                }
            }
        }

        public uint CoreClockMin => this.macmGpuEntry.coreClockMin;

        public uint CoreClockMax => this.macmGpuEntry.coreClockMax;

        public uint CoreClockDef => this.macmGpuEntry.coreClockDef;

        public uint ShaderClockCur
        {
            get => this.macmGpuEntry.shaderClockCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.SHADER_CLOCK) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.SHADER_CLOCK)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support changing the shader clock speed.");
                }
                //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support changing the shader clock speed.");
                if (value < this.ShaderClockMin || value > this.ShaderClockMax)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "shaderClockCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                else
                {
                    this.macmGpuEntry.shaderClockCur = value;
                }
            }
        }

        public uint ShaderClockMin => this.macmGpuEntry.shaderClockMin;

        public uint ShaderClockMax => this.macmGpuEntry.shaderClockMax;

        public uint ShaderClockDef => this.macmGpuEntry.shaderClockDef;

        public uint MemoryClockCur
        {
            get => this.macmGpuEntry.memoryClockCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_CLOCK) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_CLOCK)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support changing the memory clock speed.");
                }
                //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support changing the memory clock speed.");
                if (value < this.MemoryClockMin || value > this.MemoryClockMax)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "memoryClockCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                else
                {
                    this.macmGpuEntry.memoryClockCur = value;
                }
            }
        }

        public uint MemoryClockMin => this.macmGpuEntry.memoryClockMin;

        public uint MemoryClockMax => this.macmGpuEntry.memoryClockMax;

        public uint MemoryClockDef => this.macmGpuEntry.memoryClockDef;

        public uint FanSpeedCur
        {
            get => this.macmGpuEntry.fanSpeedCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.FAN_SPEED) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.FAN_SPEED)
                {
                    //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support changing the fan speed.");
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support changing the fan speed.");
                }
                    //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support changing the fan speed.");
                if (this.FanFlagsCur == MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.AUTO)
                {
                    //throw new MACMFanControlNotManual();
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "Fan is currently set to auto.  Cannot set fan speed.");
                }
                //throw new MACMFanControlNotManual();
                if (value < this.FanSpeedMin || value > this.FanSpeedMax)
                {
                    //throw new ArgumentOutOfRangeException();
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "fanSpeedCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                else
                {
                    this.macmGpuEntry.fanSpeedCur = value;
                }
            }
        }

        public MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG FanFlagsCur
        {
            get => this.macmGpuEntry.fanFlagsCur;
            set => this.macmGpuEntry.fanFlagsCur = value;
        }

        public uint FanSpeedMin => this.macmGpuEntry.fanSpeedMin;

        public uint FanSpeedMax => this.macmGpuEntry.fanSpeedMax;

        public uint FanSpeedDef => this.macmGpuEntry.fanSpeedDef;

        public MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG FanFlagsDef => this.macmGpuEntry.fanFlagsDef;

        public uint CoreVoltageCur
        {
            get => this.macmGpuEntry.coreVoltageCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_VOLTAGE) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_VOLTAGE)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support changing the core voltage.");
                }
                //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support changing the core voltage.");
                if (value < this.CoreVoltageMin || value > this.CoreVoltageMax)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "coreVoltageCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                else
                {
                    this.macmGpuEntry.coreVoltageCur = value;
                }
            }
        }

        public uint CoreVoltageMin => this.macmGpuEntry.coreVoltageMin;

        public uint CoreVoltageMax => this.macmGpuEntry.coreVoltageMax;

        public uint CoreVoltageDef => this.macmGpuEntry.coreVoltageDef;

        public uint MemoryVoltageCur
        {
            get => this.macmGpuEntry.memoryVoltageCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_VOLTAGE) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_VOLTAGE)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support changing the memory voltage.");
                }
                //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support changing the memory voltage.");
                if (value < this.MemoryVoltageMin || value > this.MemoryVoltageMax)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "memoryVoltageCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                else
                {
                    this.macmGpuEntry.memoryVoltageCur = value;
                }
            }
        }

        public uint MemoryVoltageMin => this.macmGpuEntry.memoryVoltageMin;

        public uint MemoryVoltageMax => this.macmGpuEntry.memoryVoltageMax;

        public uint MemoryVoltageDef => this.macmGpuEntry.memoryVoltageDef;

        public uint AuxVoltageCur
        {
            get => this.macmGpuEntry.auxVoltageCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.AUX_VOLTAGE) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.AUX_VOLTAGE)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support changing the auxilary voltage.");
                }
                //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support changing the auxilary voltage.");
                if (value < this.AuxVoltageMin || value > this.AuxVoltageMax)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "auxVoltageCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                else
                {
                    this.macmGpuEntry.auxVoltageCur = value;
                }
            }
        }

        public uint AuxVoltageMin => this.macmGpuEntry.auxVoltageMin;

        public uint AuxVoltageMax => this.macmGpuEntry.auxVoltageMax;

        public uint AuxVoltageDef => this.macmGpuEntry.auxVoltageDef;

        public int CoreVoltageBoostCur
        {
            get => this.macmGpuEntry.coreVoltageBoostCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_VOLTAGE_BOOST) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_VOLTAGE_BOOST)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support core voltage boost.");
                }
                    //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support core voltage boost.");
                if (value < this.CoreVoltageBoostMin || value > this.CoreVoltageBoostMax)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "coreVoltageBoostCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                {
                    this.macmGpuEntry.coreVoltageBoostCur = value;
                }
            }
        }

        public int CoreVoltageBoostMin => this.macmGpuEntry.coreVoltageBoostMin;

        public int CoreVoltageBoostMax => this.macmGpuEntry.coreVoltageBoostMax;

        public int CoreVoltageBoostDef => this.macmGpuEntry.coreVoltageBoostDef;

        public int MemoryVoltageBoostCur
        {
            get => this.macmGpuEntry.memoryVoltageBoostCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_VOLTAGE_BOOST) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_VOLTAGE_BOOST)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support memory voltage boost.");
                }
                //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support memory voltage boost.");
                if (value < this.MemoryVoltageBoostMin || value > this.MemoryVoltageBoostMax)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "memoryVoltageBoostCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                else
                {
                    this.macmGpuEntry.memoryVoltageBoostCur = value;
                }
            }
        }

        public int MemoryVoltageBoostMin => this.macmGpuEntry.memoryVoltageBoostMin;

        public int MemoryVoltageBoostMax => this.macmGpuEntry.memoryVoltageBoostMax;

        public int MemoryVoltageBoostDef => this.macmGpuEntry.memoryVoltageBoostDef;

        public int AuxVoltageBoostCur
        {
            get => this.macmGpuEntry.auxVoltageBoostCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.AUX_VOLTAGE_BOOST) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.AUX_VOLTAGE_BOOST)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support auxilary voltage boost.");
                }
                    //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support auxilary voltage boost.");
                if (value < this.AuxVoltageBoostMin || value > this.AuxVoltageBoostMax)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "auxVoltageBoostCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                {
                    this.macmGpuEntry.auxVoltageBoostCur = value;
                }
            }
        }

        public int AuxVoltageBoostMin => this.macmGpuEntry.auxVoltageBoostMin;

        public int AuxVoltageBoostMax => this.macmGpuEntry.auxVoltageBoostMax;

        public int AuxVoltageBoostDef => this.macmGpuEntry.auxVoltageBoostDef;

        public int PowerLimitCur
        {
            get => this.macmGpuEntry.powerLimitCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.POWER_LIMIT) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.POWER_LIMIT)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support power limits.");
                }
                //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support power limits.");
                if (value < this.PowerLimitMin || value > this.PowerLimitMax)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "powerLimitCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                else
                {
                    this.macmGpuEntry.powerLimitCur = value;
                }
            }
        }

        public int PowerLimitMin => this.macmGpuEntry.powerLimitMin;

        public int PowerLimitMax => this.macmGpuEntry.powerLimitMax;

        public int PowerLimitDef => this.macmGpuEntry.powerLimitDef;

        public int CoreClockBoostCur
        {
            get => this.macmGpuEntry.coreClockBoostCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_CLOCK_BOOST) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_CLOCK_BOOST)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support core clock boost.");
                }
                    //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support core clock boost.");
                if (value < this.CoreClockBoostMin || value > this.CoreClockBoostMax)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "coreClockBoostCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                {
                    this.macmGpuEntry.coreClockBoostCur = value;
                }
            }
        }

        public int CoreClockBoostMin => this.macmGpuEntry.coreClockBoostMin;

        public int CoreClockBoostMax => this.macmGpuEntry.coreClockBoostMax;

        public int CoreClockBoostDef => this.macmGpuEntry.coreClockBoostDef;

        public int MemoryClockBoostCur
        {
            get => this.macmGpuEntry.memoryClockBoostCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_CLOCK_BOOST) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_CLOCK_BOOST)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support memory clock boost.");
                }
                //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support memory clock boost.");
                if (value < this.MemoryClockBoostMin || value > this.MemoryClockBoostMax)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "memoryClockBoostCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                else
                {
                    this.macmGpuEntry.memoryClockBoostCur = value;
                }
            }
        }

        public int MemoryClockBoostMin => this.macmGpuEntry.memoryClockBoostMin;

        public int MemoryClockBoostMax => this.macmGpuEntry.memoryClockBoostMax;

        public int MemoryClockBoostDef => this.macmGpuEntry.memoryClockBoostDef;

        public int ThermalLimitCur
        {
            get => this.macmGpuEntry.thermalLimitCur;
            set
            {
                if ((this.Flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.THERMAL_LIMIT) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.THERMAL_LIMIT)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "GPU " + (object)this.Index + " does not support changing the thermal limit.");
                }
                //throw new MACMFeatureNotSupported("GPU " + (object)this.Index + " does not support changing the thermal limit.");
                if (value < this.ThermalLimitMin || value > this.ThermalLimitMax)
                {
                    Helpers.ConsolePrint("ControlMemoryGpuEntry", "thermalLimitCur ArgumentOutOfRangeException");
                }
                //throw new ArgumentOutOfRangeException();
                else
                {
                    this.macmGpuEntry.thermalLimitCur = value;
                }
            }
        }

        public int ThermalLimitMin => this.macmGpuEntry.thermalLimitMin;

        public int ThermalLimitMax => this.macmGpuEntry.thermalLimitMax;

        public int ThermalLimitDef => this.macmGpuEntry.thermalLimitDef;
        public uint thermalPrioritizeCur => this.macmGpuEntry.thermalPrioritizeCur;
        public uint thermalPrioritizeDef => this.macmGpuEntry.thermalPrioritizeDef;

        public uint Aux2VoltageCur => this.macmGpuEntry.Aux2VoltageCur;
        public uint Aux2VoltageMin => this.macmGpuEntry.Aux2VoltageMin;
        public uint Aux2VoltageMax => this.macmGpuEntry.Aux2VoltageMax;
        public uint Aux2VoltageDef => this.macmGpuEntry.Aux2VoltageDef;
        public int Aux2VoltageBoostCur => this.macmGpuEntry.Aux2VoltageBoostCur;
        public int Aux2VoltageBoostMin => this.macmGpuEntry.Aux2VoltageBoostMin;
        public int Aux2VoltageBoostMax => this.macmGpuEntry.Aux2VoltageBoostMax;
        public int Aux2VoltageBoostDef => this.macmGpuEntry.Aux2VoltageBoostDef;

        //public MACM_SHARED_MEMORY_VF_CURVE vfCurve => this.macmGpuEntry.vfCurve;
        
        /*
        public MACM_SHARED_MEMORY_VF_CURVE vfCurve
        {
            get
            {
                return this.macmGpuEntry.vfCurve;
            }
            set
            {
                this.macmGpuEntry.vfCurve = value;
            }
        }
        */
        public MACM_SHARED_MEMORY_VF_CURVE vfCurve
        {
            get => this.macmGpuEntry.vfCurve;
            set => this.macmGpuEntry.vfCurve = value;
        }

        public uint CurveLockIndex
        {
            get => this.macmGpuEntry.curveLockIndex;
            set
            {
                this.macmGpuEntry.curveLockIndex = value;
            }
        }
        /*
        public uint CurveLockIndex
        {
            get => this.macmGpuEntry.curveLockIndex;
            set => this.macmGpuEntry.curveLockIndex = value;
        }
        */
        public unsafe void ClearCurve() 
        {
            MACM_SHARED_MEMORY_VF_CURVE curve = new MACM_SHARED_MEMORY_VF_CURVE();
            this.vfCurve = curve;
           // this.macmGpuEntry.vfCurve.dwLockIndex = 0x12;//не применяется
        }

        //public unsafe char GpuId => this.macmGpuEntry.szGpuId[SharedMemory.MAX_PATH];
        public string GpuId => new string(this.macmGpuEntry.szGpuId).TrimEnd(new char[1]);
        /*
        public string GpuId
        {
            get
            {
                return new string(this.macmGpuEntry.szGpuId).TrimEnd(new char[1]);
            }
            set
            {
                this.macmGpuEntry.szGpuId = value.ToCharArray();
            }
        }
        */
        public bool IsMaster => this.isMaster;

        public int Index => this.index;
        
        public ControlMemoryGpuEntry()
        {
        }

        public ControlMemoryGpuEntry(ControlMemoryHeader header, int index)
        {
            this.index = index;
            if ((long)index != (long)header.MasterGpu)
                return;
            this.isMaster = true;
        }

        public void ResetToDefaults()
        {
            try
            {
                this.macmGpuEntry.Aux2VoltageBoostCur = this.Aux2VoltageBoostDef;
                this.macmGpuEntry.auxVoltageCur = this.Aux2VoltageDef;
                this.macmGpuEntry.coreClockBoostCur = this.CoreClockBoostDef;
                this.macmGpuEntry.coreClockCur = this.CoreClockDef;
                this.macmGpuEntry.coreVoltageBoostCur = this.CoreVoltageBoostDef;
                this.macmGpuEntry.coreVoltageCur = this.CoreVoltageDef;
                this.macmGpuEntry.fanFlagsCur = this.FanFlagsDef;
                this.macmGpuEntry.fanSpeedCur = this.FanSpeedDef;
                //if ((this.macmGpuEntry.flags & MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE) != MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE)
                if (this.macmGpuEntry.flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE_ENABLED))
                {
                    this.macmGpuEntry.flags = this.macmGpuEntry.flags - (int)MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE_ENABLED;
                }
                this.macmGpuEntry.memoryClockBoostCur = this.MemoryClockBoostDef;
                this.macmGpuEntry.memoryClockCur = this.MemoryClockDef;
                this.macmGpuEntry.memoryVoltageBoostCur = this.MemoryVoltageBoostDef;
                this.macmGpuEntry.memoryVoltageCur = this.MemoryVoltageDef;
                this.macmGpuEntry.powerLimitCur = this.PowerLimitDef;
                this.macmGpuEntry.shaderClockCur = this.ShaderClockDef;
                this.macmGpuEntry.thermalLimitCur = this.ThermalLimitDef;
                this.macmGpuEntry.thermalPrioritizeCur = this.thermalPrioritizeDef;
            }
            catch
            {
            }
        }

        internal void Update(ControlMemoryHeader header, int index)
        {
            this.index = index;
            if ((long)index != (long)header.MasterGpu)
                return;
            this.isMaster = true;
        }

        public override string ToString()
        {
            try
            {
                return "IsMaster = " + this.IsMaster.ToString() + ";Flags = " + (object)this.Flags + ";FanFlagsCur = " + (object)this.FanFlagsCur + ";FanFlagsDev = " + (object)this.FanFlagsDef + ";FanSpeed = Cur:" + (object)this.FanSpeedCur + ", Min:" + (object)this.FanSpeedMin + ", Max:" + (object)this.FanSpeedMax + ", Def:" + (object)this.FanSpeedDef + ";CoreClock = Cur:" + (object)this.CoreClockCur + ", Min:" + (object)this.CoreClockMin + ", Max:" + (object)this.CoreClockMax + ", Def:" + (object)this.CoreClockDef + ";ShaderClock = Cur:" + (object)this.ShaderClockCur + ", Min:" + (object)this.ShaderClockMin + ", Max:" + (object)this.ShaderClockMax + ", Def:" + (object)this.ShaderClockDef + ";MemoryClock = Cur:" + (object)this.MemoryClockCur + ", Min:" + (object)this.MemoryClockMin + ", Max:" + (object)this.MemoryClockMax + ", Def:" + (object)this.MemoryClockDef + ";CoreVoltage = Cur:" + (object)this.CoreVoltageCur + ", Min:" + (object)this.CoreVoltageMin + ", Max:" + (object)this.CoreVoltageMax + ", Def:" + (object)this.CoreVoltageDef + ";MemoryVoltage = Cur:" + (object)this.MemoryVoltageCur + ", Min:" + (object)this.MemoryVoltageMin + ", Max:" + (object)this.MemoryVoltageMax + ", Def:" + (object)this.MemoryVoltageDef + ";AuxVoltage = Cur:" + (object)this.AuxVoltageCur + ", Min:" + (object)this.AuxVoltageMin + ", Max:" + (object)this.AuxVoltageMax + ", Def:" + (object)this.AuxVoltageDef + ";CoreVoltageBoost = Cur:" + (object)this.CoreVoltageBoostCur + ", Min:" + (object)this.CoreVoltageBoostMin + ", Max:" + (object)this.CoreVoltageBoostMax + ", Def:" + (object)this.CoreVoltageBoostDef + ";MemoryVoltageBoost = Cur:" + (object)this.MemoryVoltageBoostCur + ", Min:" + (object)this.MemoryVoltageBoostMin + ", Max:" + (object)this.MemoryVoltageBoostMax + ", Def:" + (object)this.MemoryVoltageBoostDef + ";AuxVoltageBoost = Cur:" + (object)this.AuxVoltageBoostCur + ", Min:" + (object)this.AuxVoltageBoostMin + ", Max:" + (object)this.AuxVoltageBoostMax + ", Def:" + (object)this.AuxVoltageBoostDef + ";PowerLimit = Cur:" + (object)this.PowerLimitCur + ", Min:" + (object)this.PowerLimitMin + ", Max:" + (object)this.PowerLimitMax + ", Def:" + (object)this.PowerLimitDef + ";CoreClockBoost = Cur:" + (object)this.CoreClockBoostCur + ", Min:" + (object)this.CoreClockBoostMin + ", Max:" + (object)this.CoreClockBoostMax + ", Def:" + (object)this.CoreClockBoostDef + ";MemoryClockBoost = Cur:" + (object)this.MemoryClockBoostCur + ", Min:" + (object)this.MemoryClockBoostMin + ", Max:" + (object)this.MemoryClockBoostMax + ", Def:" + (object)this.MemoryClockBoostDef;
            }
            catch
            {
                return base.ToString();
            }
        }


        private ControlMemoryGpuEntry(SerializationInfo info, StreamingContext ctxt)
        {
            this.macmGpuEntry.flags = (MACM_SHARED_MEMORY_GPU_ENTRY_FLAG)info.GetValue("flags", typeof(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG));
            this.macmGpuEntry.coreClockCur = info.GetUInt32("coreClockCur");
            this.macmGpuEntry.coreClockMin = info.GetUInt32("coreClockMin");
            this.macmGpuEntry.coreClockMax = info.GetUInt32("coreClockMax");
            this.macmGpuEntry.coreClockDef = info.GetUInt32("coreClockDef");
            this.macmGpuEntry.shaderClockCur = info.GetUInt32("shaderClockCur");
            this.macmGpuEntry.shaderClockMin = info.GetUInt32("shaderClockMin");
            this.macmGpuEntry.shaderClockMax = info.GetUInt32("shaderClockMax");
            this.macmGpuEntry.shaderClockDef = info.GetUInt32("shaderClockDef");
            this.macmGpuEntry.memoryClockCur = info.GetUInt32("memoryClockCur");
            this.macmGpuEntry.memoryClockMin = info.GetUInt32("memoryClockMin");
            this.macmGpuEntry.memoryClockMax = info.GetUInt32("memoryClockMax");
            this.macmGpuEntry.memoryClockDef = info.GetUInt32("memoryClockDef");
            this.macmGpuEntry.fanSpeedCur = info.GetUInt32("fanSpeedCur");
            this.macmGpuEntry.fanFlagsCur = (MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG)info.GetValue("fanFlagsCur", typeof(MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG));
            this.macmGpuEntry.fanSpeedMin = info.GetUInt32("fanSpeedMin");
            this.macmGpuEntry.fanSpeedMax = info.GetUInt32("fanSpeedMax");
            this.macmGpuEntry.fanSpeedDef = info.GetUInt32("fanSpeedDef");
            this.macmGpuEntry.fanFlagsDef = (MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG)info.GetValue("fanFlagsDef", typeof(MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG));
            this.macmGpuEntry.coreVoltageCur = info.GetUInt32("coreVoltageCur");
            this.macmGpuEntry.coreVoltageMin = info.GetUInt32("coreVoltageMin");
            this.macmGpuEntry.coreVoltageMax = info.GetUInt32("coreVoltageMax");
            this.macmGpuEntry.coreVoltageDef = info.GetUInt32("coreVoltageDef");
            this.macmGpuEntry.memoryVoltageCur = info.GetUInt32("memoryVoltageCur");
            this.macmGpuEntry.memoryVoltageMin = info.GetUInt32("memoryVoltageMin");
            this.macmGpuEntry.memoryVoltageMax = info.GetUInt32("memoryVoltageMax");
            this.macmGpuEntry.memoryVoltageDef = info.GetUInt32("memoryVoltageDef");
            this.macmGpuEntry.auxVoltageCur = info.GetUInt32("auxVoltageCur");
            this.macmGpuEntry.auxVoltageMin = info.GetUInt32("auxVoltageMin");
            this.macmGpuEntry.auxVoltageMax = info.GetUInt32("auxVoltageMax");
            this.macmGpuEntry.auxVoltageDef = info.GetUInt32("auxVoltageDef");
            this.macmGpuEntry.coreVoltageBoostCur = info.GetInt32("coreVoltageBoostCur");
            this.macmGpuEntry.coreVoltageBoostMin = info.GetInt32("coreVoltageBoostMin");
            this.macmGpuEntry.coreVoltageBoostMax = info.GetInt32("coreVoltageBoostMax");
            this.macmGpuEntry.coreVoltageBoostDef = info.GetInt32("coreVoltageBoostDef");
            this.macmGpuEntry.memoryVoltageBoostCur = info.GetInt32("memoryVoltageBoostCur");
            this.macmGpuEntry.memoryVoltageBoostMin = info.GetInt32("memoryVoltageBoostMin");
            this.macmGpuEntry.memoryVoltageBoostMax = info.GetInt32("memoryVoltageBoostMax");
            this.macmGpuEntry.memoryVoltageBoostDef = info.GetInt32("memoryVoltageBoostDef");
            this.macmGpuEntry.auxVoltageBoostCur = info.GetInt32("auxVoltageBoostCur");
            this.macmGpuEntry.auxVoltageBoostMin = info.GetInt32("auxVoltageBoostMin");
            this.macmGpuEntry.auxVoltageBoostMax = info.GetInt32("auxVoltageBoostMax");
            this.macmGpuEntry.auxVoltageBoostDef = info.GetInt32("auxVoltageBoostDef");
            this.macmGpuEntry.powerLimitCur = info.GetInt32("powerLimitCur");
            this.macmGpuEntry.powerLimitMin = info.GetInt32("powerLimitMin");
            this.macmGpuEntry.powerLimitMax = info.GetInt32("powerLimitMax");
            this.macmGpuEntry.powerLimitDef = info.GetInt32("powerLimitDef");
            this.macmGpuEntry.coreClockBoostCur = info.GetInt32("coreClockBoostCur");
            this.macmGpuEntry.coreClockBoostMin = info.GetInt32("coreClockBoostMin");
            this.macmGpuEntry.coreClockBoostMax = info.GetInt32("coreClockBoostMax");
            this.macmGpuEntry.coreClockBoostDef = info.GetInt32("coreClockBoostDef");
            this.macmGpuEntry.memoryClockBoostCur = info.GetInt32("memoryClockBoostCur");
            this.macmGpuEntry.memoryClockBoostMin = info.GetInt32("memoryClockBoostMin");
            this.macmGpuEntry.memoryClockBoostMax = info.GetInt32("memoryClockBoostMax");
            this.macmGpuEntry.memoryClockBoostDef = info.GetInt32("memoryClockBoostDef");
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("flags", (object)this.Flags);
            info.AddValue("coreClockCur", this.CoreClockCur);
            info.AddValue("coreClockMin", this.CoreClockMin);
            info.AddValue("coreClockMax", this.CoreClockMax);
            info.AddValue("coreClockDef", this.CoreClockDef);
            info.AddValue("shaderClockCur", this.ShaderClockCur);
            info.AddValue("shaderClockMin", this.ShaderClockMin);
            info.AddValue("shaderClockMax", this.ShaderClockMax);
            info.AddValue("shaderClockDef", this.ShaderClockDef);
            info.AddValue("memoryClockCur", this.MemoryClockCur);
            info.AddValue("memoryClockMin", this.MemoryClockMin);
            info.AddValue("memoryClockMax", this.MemoryClockMax);
            info.AddValue("memoryClockDef", this.MemoryClockDef);
            info.AddValue("fanSpeedCur", this.FanSpeedCur);
            info.AddValue("fanFlagsCur", (object)this.FanFlagsCur);
            info.AddValue("fanSpeedMin", this.FanSpeedMin);
            info.AddValue("fanSpeedMax", this.FanSpeedMax);
            info.AddValue("fanSpeedDef", this.FanSpeedDef);
            info.AddValue("fanFlagsDef", (object)this.FanFlagsDef);
            info.AddValue("coreVoltageCur", this.CoreVoltageCur);
            info.AddValue("coreVoltageMin", this.CoreVoltageMin);
            info.AddValue("coreVoltageMax", this.CoreVoltageMax);
            info.AddValue("coreVoltageDef", this.CoreVoltageDef);
            info.AddValue("memoryVoltageCur", this.MemoryVoltageCur);
            info.AddValue("memoryVoltageMin", this.MemoryVoltageMin);
            info.AddValue("memoryVoltageMax", this.MemoryVoltageMax);
            info.AddValue("memoryVoltageDef", this.MemoryVoltageDef);
            info.AddValue("auxVoltageCur", this.AuxVoltageCur);
            info.AddValue("auxVoltageMin", this.AuxVoltageMin);
            info.AddValue("auxVoltageMax", this.AuxVoltageMax);
            info.AddValue("auxVoltageDef", this.AuxVoltageDef);
            info.AddValue("coreVoltageBoostCur", this.CoreVoltageBoostCur);
            info.AddValue("coreVoltageBoostMin", this.CoreVoltageBoostMin);
            info.AddValue("coreVoltageBoostMax", this.CoreVoltageBoostMax);
            info.AddValue("coreVoltageBoostDef", this.CoreVoltageBoostDef);
            info.AddValue("memoryVoltageBoostCur", this.MemoryVoltageBoostCur);
            info.AddValue("memoryVoltageBoostMin", this.MemoryVoltageBoostMin);
            info.AddValue("memoryVoltageBoostMax", this.MemoryVoltageBoostMax);
            info.AddValue("memoryVoltageBoostDef", this.MemoryVoltageBoostDef);
            info.AddValue("auxVoltageBoostCur", this.AuxVoltageBoostCur);
            info.AddValue("auxVoltageBoostMin", this.AuxVoltageBoostMin);
            info.AddValue("auxVoltageBoostMax", this.AuxVoltageBoostMax);
            info.AddValue("auxVoltageBoostDef", this.AuxVoltageBoostDef);
            info.AddValue("powerLimitCur", this.PowerLimitCur);
            info.AddValue("powerLimitMin", this.PowerLimitMin);
            info.AddValue("powerLimitMax", this.PowerLimitMax);
            info.AddValue("powerLimitDef", this.PowerLimitDef);
            info.AddValue("coreClockBoostCur", this.CoreClockBoostCur);
            info.AddValue("coreClockBoostMin", this.CoreClockBoostMin);
            info.AddValue("coreClockBoostMax", this.CoreClockBoostMax);
            info.AddValue("coreClockBoostDef", this.CoreClockBoostDef);
            info.AddValue("memoryClockBoostCur", this.MemoryClockBoostCur);
            info.AddValue("memoryClockBoostMin", this.MemoryClockBoostMin);
            info.AddValue("memoryClockBoostMax", this.MemoryClockBoostMax);
            info.AddValue("memoryClockBoostDef", this.MemoryClockBoostDef);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("flags", this.Flags.ToString());
            writer.WriteElementString("coreClockCur", this.CoreClockCur.ToString());
            writer.WriteElementString("coreClockMin", this.CoreClockMin.ToString());
            writer.WriteElementString("coreClockMax", this.CoreClockMax.ToString());
            writer.WriteElementString("coreClockDef", this.CoreClockDef.ToString());
            writer.WriteElementString("shaderClockCur", this.ShaderClockCur.ToString());
            writer.WriteElementString("shaderClockMin", this.ShaderClockMin.ToString());
            writer.WriteElementString("shaderClockMax", this.ShaderClockMax.ToString());
            writer.WriteElementString("shaderClockDef", this.ShaderClockDef.ToString());
            writer.WriteElementString("memoryClockCur", this.MemoryClockCur.ToString());
            writer.WriteElementString("memoryClockMin", this.MemoryClockMin.ToString());
            writer.WriteElementString("memoryClockMax", this.MemoryClockMax.ToString());
            writer.WriteElementString("memoryClockDef", this.MemoryClockDef.ToString());
            writer.WriteElementString("fanSpeedCur", this.FanSpeedCur.ToString());
            writer.WriteElementString("fanFlagsCur", this.FanFlagsCur.ToString());
            writer.WriteElementString("fanSpeedMin", this.FanSpeedMin.ToString());
            writer.WriteElementString("fanSpeedMax", this.FanSpeedMax.ToString());
            writer.WriteElementString("fanSpeedDef", this.FanSpeedDef.ToString());
            writer.WriteElementString("fanFlagsDef", this.FanFlagsDef.ToString());
            writer.WriteElementString("coreVoltageCur", this.CoreVoltageCur.ToString());
            writer.WriteElementString("coreVoltageMin", this.CoreVoltageMin.ToString());
            writer.WriteElementString("coreVoltageMax", this.CoreVoltageMax.ToString());
            writer.WriteElementString("coreVoltageDef", this.CoreVoltageDef.ToString());
            writer.WriteElementString("memoryVoltageCur", this.MemoryVoltageCur.ToString());
            writer.WriteElementString("memoryVoltageMin", this.MemoryVoltageMin.ToString());
            writer.WriteElementString("memoryVoltageMax", this.MemoryVoltageMax.ToString());
            writer.WriteElementString("memoryVoltageDef", this.MemoryVoltageDef.ToString());
            writer.WriteElementString("auxVoltageCur", this.AuxVoltageCur.ToString());
            writer.WriteElementString("auxVoltageMin", this.AuxVoltageMin.ToString());
            writer.WriteElementString("auxVoltageMax", this.AuxVoltageMax.ToString());
            writer.WriteElementString("auxVoltageDef", this.AuxVoltageDef.ToString());
            writer.WriteElementString("coreVoltageBoostCur", this.CoreVoltageBoostCur.ToString());
            writer.WriteElementString("coreVoltageBoostMin", this.CoreVoltageBoostMin.ToString());
            writer.WriteElementString("coreVoltageBoostMax", this.CoreVoltageBoostMax.ToString());
            writer.WriteElementString("coreVoltageBoostDef", this.CoreVoltageBoostDef.ToString());
            writer.WriteElementString("memoryVoltageBoostCur", this.MemoryVoltageBoostCur.ToString());
            writer.WriteElementString("memoryVoltageBoostMin", this.MemoryVoltageBoostMin.ToString());
            writer.WriteElementString("memoryVoltageBoostMax", this.MemoryVoltageBoostMax.ToString());
            writer.WriteElementString("memoryVoltageBoostDef", this.MemoryVoltageBoostDef.ToString());
            writer.WriteElementString("auxVoltageBoostCur", this.AuxVoltageBoostCur.ToString());
            writer.WriteElementString("auxVoltageBoostMin", this.AuxVoltageBoostMin.ToString());
            writer.WriteElementString("auxVoltageBoostMax", this.AuxVoltageBoostMax.ToString());
            writer.WriteElementString("auxVoltageBoostDef", this.AuxVoltageBoostDef.ToString());
            writer.WriteElementString("powerLimitCur", this.PowerLimitCur.ToString());
            writer.WriteElementString("powerLimitMin", this.PowerLimitMin.ToString());
            writer.WriteElementString("powerLimitMax", this.PowerLimitMax.ToString());
            writer.WriteElementString("powerLimitDef", this.PowerLimitDef.ToString());
            writer.WriteElementString("coreClockBoostCur", this.CoreClockBoostCur.ToString());
            writer.WriteElementString("coreClockBoostMin", this.CoreClockBoostMin.ToString());
            writer.WriteElementString("coreClockBoostMax", this.CoreClockBoostMax.ToString());
            writer.WriteElementString("coreClockBoostDef", this.CoreClockBoostDef.ToString());
            writer.WriteElementString("memoryClockBoostCur", this.MemoryClockBoostCur.ToString());
            writer.WriteElementString("memoryClockBoostMin", this.MemoryClockBoostMin.ToString());
            writer.WriteElementString("memoryClockBoostMax", this.MemoryClockBoostMax.ToString());
            writer.WriteElementString("memoryClockBoostDef", this.MemoryClockBoostDef.ToString());
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            this.macmGpuEntry.flags = (MACM_SHARED_MEMORY_GPU_ENTRY_FLAG)Enum.Parse(typeof(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG), reader.ReadElementString("flags"));
            this.macmGpuEntry.coreClockCur = uint.Parse(reader.ReadElementString("coreClockCur"));
            this.macmGpuEntry.coreClockMin = uint.Parse(reader.ReadElementString("coreClockMin"));
            this.macmGpuEntry.coreClockMax = uint.Parse(reader.ReadElementString("coreClockMax"));
            this.macmGpuEntry.coreClockDef = uint.Parse(reader.ReadElementString("coreClockDef"));
            this.macmGpuEntry.shaderClockCur = uint.Parse(reader.ReadElementString("shaderClockCur"));
            this.macmGpuEntry.shaderClockMin = uint.Parse(reader.ReadElementString("shaderClockMin"));
            this.macmGpuEntry.shaderClockMax = uint.Parse(reader.ReadElementString("shaderClockMax"));
            this.macmGpuEntry.shaderClockDef = uint.Parse(reader.ReadElementString("shaderClockDef"));
            this.macmGpuEntry.memoryClockCur = uint.Parse(reader.ReadElementString("memoryClockCur"));
            this.macmGpuEntry.memoryClockMin = uint.Parse(reader.ReadElementString("memoryClockMin"));
            this.macmGpuEntry.memoryClockMax = uint.Parse(reader.ReadElementString("memoryClockMax"));
            this.macmGpuEntry.memoryClockDef = uint.Parse(reader.ReadElementString("memoryClockDef"));
            this.macmGpuEntry.fanSpeedCur = uint.Parse(reader.ReadElementString("fanSpeedCur"));
            this.macmGpuEntry.fanFlagsCur = (MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG)Enum.Parse(typeof(MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG), reader.ReadElementString("fanFlagsCur"));
            this.macmGpuEntry.fanSpeedMin = uint.Parse(reader.ReadElementString("fanSpeedMin"));
            this.macmGpuEntry.fanSpeedMax = uint.Parse(reader.ReadElementString("fanSpeedMax"));
            this.macmGpuEntry.fanSpeedDef = uint.Parse(reader.ReadElementString("fanSpeedDef"));
            this.macmGpuEntry.fanFlagsDef = (MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG)Enum.Parse(typeof(MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG), reader.ReadElementString("fanFlagsDef"));
            this.macmGpuEntry.coreVoltageCur = uint.Parse(reader.ReadElementString("coreVoltageCur"));
            this.macmGpuEntry.coreVoltageMin = uint.Parse(reader.ReadElementString("coreVoltageMin"));
            this.macmGpuEntry.coreVoltageMax = uint.Parse(reader.ReadElementString("coreVoltageMax"));
            this.macmGpuEntry.coreVoltageDef = uint.Parse(reader.ReadElementString("coreVoltageDef"));
            this.macmGpuEntry.memoryVoltageCur = uint.Parse(reader.ReadElementString("memoryVoltageCur"));
            this.macmGpuEntry.memoryVoltageMin = uint.Parse(reader.ReadElementString("memoryVoltageMin"));
            this.macmGpuEntry.memoryVoltageMax = uint.Parse(reader.ReadElementString("memoryVoltageMax"));
            this.macmGpuEntry.memoryVoltageDef = uint.Parse(reader.ReadElementString("memoryVoltageDef"));
            this.macmGpuEntry.auxVoltageCur = uint.Parse(reader.ReadElementString("auxVoltageCur"));
            this.macmGpuEntry.auxVoltageMin = uint.Parse(reader.ReadElementString("auxVoltageMin"));
            this.macmGpuEntry.auxVoltageMax = uint.Parse(reader.ReadElementString("auxVoltageMax"));
            this.macmGpuEntry.auxVoltageDef = uint.Parse(reader.ReadElementString("auxVoltageDef"));
            this.macmGpuEntry.coreVoltageBoostCur = int.Parse(reader.ReadElementString("coreVoltageBoostCur"));
            this.macmGpuEntry.coreVoltageBoostMin = int.Parse(reader.ReadElementString("coreVoltageBoostMin"));
            this.macmGpuEntry.coreVoltageBoostMax = int.Parse(reader.ReadElementString("coreVoltageBoostMax"));
            this.macmGpuEntry.coreVoltageBoostDef = int.Parse(reader.ReadElementString("coreVoltageBoostDef"));
            this.macmGpuEntry.memoryVoltageBoostCur = int.Parse(reader.ReadElementString("memoryVoltageBoostCur"));
            this.macmGpuEntry.memoryVoltageBoostMin = int.Parse(reader.ReadElementString("memoryVoltageBoostMin"));
            this.macmGpuEntry.memoryVoltageBoostMax = int.Parse(reader.ReadElementString("memoryVoltageBoostMax"));
            this.macmGpuEntry.memoryVoltageBoostDef = int.Parse(reader.ReadElementString("memoryVoltageBoostDef"));
            this.macmGpuEntry.auxVoltageBoostCur = int.Parse(reader.ReadElementString("auxVoltageBoostCur"));
            this.macmGpuEntry.auxVoltageBoostMin = int.Parse(reader.ReadElementString("auxVoltageBoostMin"));
            this.macmGpuEntry.auxVoltageBoostMax = int.Parse(reader.ReadElementString("auxVoltageBoostMax"));
            this.macmGpuEntry.auxVoltageBoostDef = int.Parse(reader.ReadElementString("auxVoltageBoostDef"));
            this.macmGpuEntry.powerLimitCur = int.Parse(reader.ReadElementString("powerLimitCur"));
            this.macmGpuEntry.powerLimitMin = int.Parse(reader.ReadElementString("powerLimitMin"));
            this.macmGpuEntry.powerLimitMax = int.Parse(reader.ReadElementString("powerLimitMax"));
            this.macmGpuEntry.powerLimitDef = int.Parse(reader.ReadElementString("powerLimitDef"));
            this.macmGpuEntry.coreClockBoostCur = int.Parse(reader.ReadElementString("coreClockBoostCur"));
            this.macmGpuEntry.coreClockBoostMin = int.Parse(reader.ReadElementString("coreClockBoostMin"));
            this.macmGpuEntry.coreClockBoostMax = int.Parse(reader.ReadElementString("coreClockBoostMax"));
            this.macmGpuEntry.coreClockBoostDef = int.Parse(reader.ReadElementString("coreClockBoostDef"));
            this.macmGpuEntry.memoryClockBoostCur = int.Parse(reader.ReadElementString("memoryClockBoostCur"));
            this.macmGpuEntry.memoryClockBoostMin = int.Parse(reader.ReadElementString("memoryClockBoostMin"));
            this.macmGpuEntry.memoryClockBoostMax = int.Parse(reader.ReadElementString("memoryClockBoostMax"));
            this.macmGpuEntry.memoryClockBoostDef = int.Parse(reader.ReadElementString("memoryClockBoostDef"));
        }

        XmlSchema IXmlSerializable.GetSchema() => (XmlSchema)null;
    }
}
