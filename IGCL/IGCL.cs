using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace IGCL
{

    public class IGCL
    {
        //[StructLayout(LayoutKind.Sequential)]
        public struct ctl_init_args_t//4,1,4,4,4,16
        {
            public uint Size;                                  ///< [in] size of this structure
            public byte Version;                                ///< [in] version of this structure
            public uint AppVersion;                  ///< [in][release] App's IGCL version
            public uint flags;                         ///< [in][release] Caller version
            public uint SupportedVersion;            ///< [out][release] IGCL implementation version
            public ctl_application_id_t ApplicationUID;            ///< [in] Application Provided Unique ID.Application can pass all 0's as
                                                                   ///< the default ID
        }
        //[StructLayout(LayoutKind.Sequential)]
        public struct ctl_application_id_t//4,2,2,8=16
        {
            public uint Data1;                                 ///< [in] Data1
            public ushort Data2;                                 ///< [in] Data2
            public ushort Data3;                                 ///< [in] Data3
            //public fixed byte Data4[8];                               ///< [in] Data4
            public uint Data40;                               ///< [in] Data4
            public uint Data41;                               ///< [in] Data4

        }
        struct _ctl_api_handle_t
        {
        }

        [DllImport("ControlLib.dll")]
        public static extern _ctl_result_t ctlInit(ref ctl_init_args_t CtlInitArgs, ref ulong hAPIHandle);
        [DllImport("ControlLib.dll")]
        //public static extern _ctl_result_t ctlEnumerateDevices(ulong hAPIHandle, ref uint Adapter_count);
        public static extern _ctl_result_t ctlEnumerateDevices(ulong hAPIHandle, out uint Adapter_count,  ulong[] hDevices);

        public unsafe static _ctl_result_t TestIntel(ref ctl_init_args_t CtlInitArgs, ref ulong hAPIHandle)
        {
            _ctl_result_t Result = (_ctl_result_t)(-1);
            //ctl_init_args_t CtlInitArgs = new ctl_init_args_t();
            //long hAPIHandle = 0;
            
            CtlInitArgs.AppVersion = (1 << 16) | (1 & 0x0000ffff);
            CtlInitArgs.flags = 0;
            CtlInitArgs.Size = (uint)sizeof(ctl_init_args_t);
            CtlInitArgs.Version = 0;
            CtlInitArgs.ApplicationUID = new ctl_application_id_t();
            
            Result = ctlInit(ref CtlInitArgs, ref hAPIHandle);
            /*
            byte[] _l1 = new byte[l1];
            byte[] _l2 = new byte[l2];
            try
            {
                Result = ctlInit(_l1, ref hAPIHandle);
            } catch
            {

            }
            */
            //return (_ctl_result_t)sizeof(ctl_init_args_t);
            return Result;
        }


        public enum _ctl_result_t
        {
            CTL_RESULT_SUCCESS = 0x00000000,                ///< success
            CTL_RESULT_SUCCESS_STILL_OPEN_BY_ANOTHER_CALLER = 0x00000001,   ///< success but still open by another caller
            CTL_RESULT_ERROR_SUCCESS_END = 0x0000FFFF,      ///< "Success group error code end value, not to be used
                                                            ///< "
            CTL_RESULT_ERROR_GENERIC_START = 0x40000000,    ///< Generic error code starting value, not to be used
            CTL_RESULT_ERROR_NOT_INITIALIZED = 0x40000001,  ///< Result not initialized
            CTL_RESULT_ERROR_ALREADY_INITIALIZED = 0x40000002,  ///< Already initialized
            CTL_RESULT_ERROR_DEVICE_LOST = 0x40000003,      ///< Device hung, reset, was removed, or driver update occurred
            CTL_RESULT_ERROR_OUT_OF_HOST_MEMORY = 0x40000004,   ///< Insufficient host memory to satisfy call
            CTL_RESULT_ERROR_OUT_OF_DEVICE_MEMORY = 0x40000005, ///< Insufficient device memory to satisfy call
            CTL_RESULT_ERROR_INSUFFICIENT_PERMISSIONS = 0x40000006, ///< Access denied due to permission level
            CTL_RESULT_ERROR_NOT_AVAILABLE = 0x40000007,    ///< Resource was removed
            CTL_RESULT_ERROR_UNINITIALIZED = 0x40000008,    ///< Library not initialized
            CTL_RESULT_ERROR_UNSUPPORTED_VERSION = 0x40000009,  ///< Generic error code for unsupported versions
            CTL_RESULT_ERROR_UNSUPPORTED_FEATURE = 0x4000000a,  ///< Generic error code for unsupported features
            CTL_RESULT_ERROR_INVALID_ARGUMENT = 0x4000000b, ///< Generic error code for invalid arguments
            CTL_RESULT_ERROR_INVALID_API_HANDLE = 0x4000000c,   ///< API handle in invalid
            CTL_RESULT_ERROR_INVALID_NULL_HANDLE = 0x4000000d,  ///< Handle argument is not valid
            CTL_RESULT_ERROR_INVALID_NULL_POINTER = 0x4000000e, ///< Pointer argument may not be nullptr
            CTL_RESULT_ERROR_INVALID_SIZE = 0x4000000f,     ///< Size argument is invalid (e.g., must not be zero)
            CTL_RESULT_ERROR_UNSUPPORTED_SIZE = 0x40000010, ///< Size argument is not supported by the device (e.g., too large)
            CTL_RESULT_ERROR_UNSUPPORTED_IMAGE_FORMAT = 0x40000011, ///< Image format is not supported by the device
            CTL_RESULT_ERROR_DATA_READ = 0x40000012,        ///< Data read error
            CTL_RESULT_ERROR_DATA_WRITE = 0x40000013,       ///< Data write error
            CTL_RESULT_ERROR_DATA_NOT_FOUND = 0x40000014,   ///< Data not found error
            CTL_RESULT_ERROR_NOT_IMPLEMENTED = 0x40000015,  ///< Function not implemented
            CTL_RESULT_ERROR_OS_CALL = 0x40000016,          ///< Operating system call failure
            CTL_RESULT_ERROR_KMD_CALL = 0x40000017,         ///< Kernel mode driver call failure
            CTL_RESULT_ERROR_UNLOAD = 0x40000018,           ///< Library unload failure
            CTL_RESULT_ERROR_ZE_LOADER = 0x40000019,        ///< Level0 loader not found
            CTL_RESULT_ERROR_INVALID_OPERATION_TYPE = 0x4000001a,   ///< Invalid operation type
            CTL_RESULT_ERROR_NULL_OS_INTERFACE = 0x4000001b,///< Null OS interface
            CTL_RESULT_ERROR_NULL_OS_ADAPATER_HANDLE = 0x4000001c,  ///< Null OS adapter handle
            CTL_RESULT_ERROR_NULL_OS_DISPLAY_OUTPUT_HANDLE = 0x4000001d,///< Null display output handle
            CTL_RESULT_ERROR_WAIT_TIMEOUT = 0x4000001e,     ///< Timeout in Wait function
            CTL_RESULT_ERROR_PERSISTANCE_NOT_SUPPORTED = 0x4000001f,///< Persistance not supported
            CTL_RESULT_ERROR_PLATFORM_NOT_SUPPORTED = 0x40000020,   ///< Platform not supported
            CTL_RESULT_ERROR_UNKNOWN_APPLICATION_UID = 0x40000021,  ///< Unknown Appplicaion UID in Initialization call
            CTL_RESULT_ERROR_INVALID_ENUMERATION = 0x40000022,  ///< The enum is not valid
            CTL_RESULT_ERROR_FILE_DELETE = 0x40000023,      ///< Error in file delete operation
            CTL_RESULT_ERROR_RESET_DEVICE_REQUIRED = 0x40000024,///< The device requires a reset.
            CTL_RESULT_ERROR_FULL_REBOOT_REQUIRED = 0x40000025, ///< The device requires a full reboot.
            CTL_RESULT_ERROR_LOAD = 0x40000026,             ///< Library load failure
            CTL_RESULT_ERROR_UNKNOWN = 0x4000FFFF,          ///< Unknown or internal error
            CTL_RESULT_ERROR_RETRY_OPERATION = 0x40010000,  ///< Operation failed, retry previous operation again
            CTL_RESULT_ERROR_GENERIC_END = 0x4000FFFF,      ///< "Generic error code end value, not to be used
                                                            ///< "
            CTL_RESULT_ERROR_CORE_START = 0x44000000,       ///< Core error code starting value, not to be used
            CTL_RESULT_ERROR_CORE_OVERCLOCK_NOT_SUPPORTED = 0x44000001, ///< The Overclock is not supported.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_VOLTAGE_OUTSIDE_RANGE = 0x44000002, ///< The Voltage exceeds the acceptable min/max.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_FREQUENCY_OUTSIDE_RANGE = 0x44000003,   ///< The Frequency exceeds the acceptable min/max.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_POWER_OUTSIDE_RANGE = 0x44000004,   ///< The Power exceeds the acceptable min/max.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_TEMPERATURE_OUTSIDE_RANGE = 0x44000005, ///< The Power exceeds the acceptable min/max.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_IN_VOLTAGE_LOCKED_MODE = 0x44000006,///< The Overclock is in voltage locked mode.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_RESET_REQUIRED = 0x44000007,///< It indicates that the requested change will not be applied until the
                                                                        ///< device is reset.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_WAIVER_NOT_SET = 0x44000008,///< The $OverclockWaiverSet function has not been called.
            CTL_RESULT_ERROR_CORE_END = 0x0440FFFF,         ///< "Core error code end value, not to be used
                                                            ///< "
            CTL_RESULT_ERROR_3D_START = 0x60000000,         ///< 3D error code starting value, not to be used
            CTL_RESULT_ERROR_3D_END = 0x6000FFFF,           ///< "3D error code end value, not to be used
                                                            ///< "
            CTL_RESULT_ERROR_MEDIA_START = 0x50000000,      ///< Media error code starting value, not to be used
            CTL_RESULT_ERROR_MEDIA_END = 0x5000FFFF,        ///< "Media error code end value, not to be used
                                                            ///< "
            CTL_RESULT_ERROR_DISPLAY_START = 0x48000000,    ///< Display error code starting value, not to be used
            CTL_RESULT_ERROR_INVALID_AUX_ACCESS_FLAG = 0x48000001,  ///< Invalid flag for Aux access
            CTL_RESULT_ERROR_INVALID_SHARPNESS_FILTER_FLAG = 0x48000002,///< Invalid flag for Sharpness
            CTL_RESULT_ERROR_DISPLAY_NOT_ATTACHED = 0x48000003, ///< Error for Display not attached
            CTL_RESULT_ERROR_DISPLAY_NOT_ACTIVE = 0x48000004,   ///< Error for display attached but not active
            CTL_RESULT_ERROR_INVALID_POWERFEATURE_OPTIMIZATION_FLAG = 0x48000005,   ///< Error for invalid power optimization flag
            CTL_RESULT_ERROR_INVALID_POWERSOURCE_TYPE_FOR_DPST = 0x48000006,///< DPST is supported only in DC Mode
            CTL_RESULT_ERROR_INVALID_PIXTX_GET_CONFIG_QUERY_TYPE = 0x48000007,  ///< Invalid query type for pixel transformation get configuration
            CTL_RESULT_ERROR_INVALID_PIXTX_SET_CONFIG_OPERATION_TYPE = 0x48000008,  ///< Invalid operation type for pixel transformation set configuration
            CTL_RESULT_ERROR_INVALID_SET_CONFIG_NUMBER_OF_SAMPLES = 0x48000009, ///< Invalid number of samples for pixel transformation set configuration
            CTL_RESULT_ERROR_INVALID_PIXTX_BLOCK_ID = 0x4800000a,   ///< Invalid block id for pixel transformation
            CTL_RESULT_ERROR_INVALID_PIXTX_BLOCK_TYPE = 0x4800000b, ///< Invalid block type for pixel transformation
            CTL_RESULT_ERROR_INVALID_PIXTX_BLOCK_NUMBER = 0x4800000c,   ///< Invalid block number for pixel transformation
            CTL_RESULT_ERROR_INSUFFICIENT_PIXTX_BLOCK_CONFIG_MEMORY = 0x4800000d,   ///< Insufficient memery allocated for BlockConfigs
            CTL_RESULT_ERROR_3DLUT_INVALID_PIPE = 0x4800000e,   ///< Invalid pipe for 3dlut
            CTL_RESULT_ERROR_3DLUT_INVALID_DATA = 0x4800000f,   ///< Invalid 3dlut data
            CTL_RESULT_ERROR_3DLUT_NOT_SUPPORTED_IN_HDR = 0x48000010,   ///< 3dlut not supported in HDR
            CTL_RESULT_ERROR_3DLUT_INVALID_OPERATION = 0x48000011,  ///< Invalid 3dlut operation
            CTL_RESULT_ERROR_3DLUT_UNSUCCESSFUL = 0x48000012,   ///< 3dlut call unsuccessful
            CTL_RESULT_ERROR_AUX_DEFER = 0x48000013,        ///< AUX defer failure
            CTL_RESULT_ERROR_AUX_TIMEOUT = 0x48000014,      ///< AUX timeout failure
            CTL_RESULT_ERROR_AUX_INCOMPLETE_WRITE = 0x48000015, ///< AUX incomplete write failure
            CTL_RESULT_ERROR_I2C_AUX_STATUS_UNKNOWN = 0x48000016,   ///< I2C/AUX unkonown failure
            CTL_RESULT_ERROR_I2C_AUX_UNSUCCESSFUL = 0x48000017, ///< I2C/AUX unsuccessful
            CTL_RESULT_ERROR_LACE_INVALID_DATA_ARGUMENT_PASSED = 0x48000018,///< Lace Incorrrect AggressivePercent data or LuxVsAggressive Map data
                                                                            ///< passed by user
            CTL_RESULT_ERROR_EXTERNAL_DISPLAY_ATTACHED = 0x48000019,///< External Display is Attached hence fail the Display Switch
            CTL_RESULT_ERROR_CUSTOM_MODE_STANDARD_CUSTOM_MODE_EXISTS = 0x4800001a,  ///< Standard custom mode exists
            CTL_RESULT_ERROR_CUSTOM_MODE_NON_CUSTOM_MATCHING_MODE_EXISTS = 0x4800001b,  ///< Non custom matching mode exists
            CTL_RESULT_ERROR_CUSTOM_MODE_INSUFFICIENT_MEMORY = 0x4800001c,  ///< Custom mode insufficent memory
            CTL_RESULT_ERROR_DISPLAY_END = 0x4800FFFF,      ///< "Display error code end value, not to be used
                                                            ///< "
            CTL_RESULT_MAX

        }
    }
}
