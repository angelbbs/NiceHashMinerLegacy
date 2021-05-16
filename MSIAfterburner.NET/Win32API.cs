// Decompiled with JetBrains decompiler
// Type: MSI.Afterburner.Win32API
// Assembly: MSIAfterburner.NET, Version=1.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DA62DD79-DC0F-45F3-A9A3-FB259E0D0B92
// Assembly location: D:\NiceHashMinerLegacy\msi afterburner\MSIAfterburner.NET.dll

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MSI.Afterburner
{
    public sealed class Win32API
    {
        [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateFileMapping(
          IntPtr hFile,
          IntPtr lpAttributes,
          Win32API.FileMapProtection flProtect,
          int dwMaxSizeHi,
          int dwMaxSizeLow,
          string lpName);

        internal static IntPtr CreateFileMapping(
          FileStream File,
          Win32API.FileMapProtection flProtect,
          long ddMaxSize,
          string lpName)
        {
            int dwMaxSizeHi = (int)(ddMaxSize / (long)int.MaxValue);
            int dwMaxSizeLow = (int)(ddMaxSize % (long)int.MaxValue);
            return Win32API.CreateFileMapping(File.SafeFileHandle.DangerousGetHandle(), IntPtr.Zero, flProtect, dwMaxSizeHi, dwMaxSizeLow, lpName);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr OpenFileMapping(
          Win32API.FileMapAccess DesiredAccess,
          bool bInheritHandle,
          string lpName);

        [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr MapViewOfFile(
          IntPtr hFileMapping,
          Win32API.FileMapAccess dwDesiredAccess,
          int dwFileOffsetHigh,
          int dwFileOffsetLow,
          int dwNumberOfBytesToMap);

        internal static IntPtr MapViewOfFile(
          IntPtr hFileMapping,
          Win32API.FileMapAccess dwDesiredAccess,
          long ddFileOffset,
          int dwNumberOfBytesToMap)
        {
            int dwFileOffsetHigh = (int)(ddFileOffset / (long)int.MaxValue);
            int dwFileOffsetLow = (int)(ddFileOffset % (long)int.MaxValue);
            return Win32API.MapViewOfFile(hFileMapping, dwDesiredAccess, dwFileOffsetHigh, dwFileOffsetLow, dwNumberOfBytesToMap);
        }

        [DllImport("kernel32.dll")]
        internal static extern bool FlushViewOfFile(IntPtr lpBaseAddress, int dwNumberOfBytesToFlush);

        [DllImport("kernel32")]
        internal static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hFile);

        [DllImport("kernel32.dll")]
        internal static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] ref Win32API.SYSTEM_INFO lpSystemInfo);

        [Flags]
        internal enum FileMapProtection : uint
        {
            PageReadonly = 2,
            PageReadWrite = 4,
            PageWriteCopy = 8,
            PageExecuteRead = 32, // 0x00000020
            PageExecuteReadWrite = 64, // 0x00000040
            SectionCommit = 134217728, // 0x08000000
            SectionImage = 16777216, // 0x01000000
            SectionNoCache = 268435456, // 0x10000000
            SectionReserve = 67108864, // 0x04000000
        }

        [Flags]
        public enum FileMapAccess : uint
        {
            FileMapCopy = 1,
            FileMapWrite = 2,
            FileMapRead = 4,
            FileMapAllAccess = 31, // 0x0000001F
            fileMapExecute = 32, // 0x00000020
        }

        internal struct SYSTEM_INFO
        {
            internal Win32API._PROCESSOR_INFO_UNION uProcessorInfo;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort dwProcessorLevel;
            public ushort dwProcessorRevision;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct _PROCESSOR_INFO_UNION
        {
            [FieldOffset(0)]
            internal uint dwOemId;
            [FieldOffset(0)]
            internal ushort wProcessorArchitecture;
            [FieldOffset(2)]
            internal ushort wReserved;
        }
    }
}
