using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace MSI.Afterburner
{
    public sealed class SharedMemory : IDisposable
    {
        private IntPtr hMMF = IntPtr.Zero;
        public uint AllocationGranularity;
        private BinaryFormatter Formatter = new BinaryFormatter();
        public const int MAX_PATH = 260;
        public const int MACM_SHARED_MEMORY_VF_CURVE_POINTS_MAX = 80;
        public const int MACM_SHARED_MEMORY_VF_CURVE_TUPLES_MAX = 70;
        public const Int32 INVALID_HANDLE_VALUE = -1;

        public static IntPtr CheckSharedMemory(string name, Win32API.FileMapAccess accessLevel)
        {
            IntPtr hm = Win32API.OpenFileMapping(accessLevel, false, name);
            Console.WriteLine("hm: " + hm.ToString());
            //Win32API.CloseHandle(hm);
            return hm;
        }

        internal SharedMemory(string name, Win32API.FileMapAccess accessLevel)
        {
            this.hMMF = Win32API.OpenFileMapping(accessLevel, false, name);
            if (this.hMMF == IntPtr.Zero)
                throw new Win32Exception();
            Win32API.SYSTEM_INFO lpSystemInfo = new Win32API.SYSTEM_INFO();
            Win32API.GetSystemInfo(ref lpSystemInfo);
            this.AllocationGranularity = lpSystemInfo.dwAllocationGranularity;
        }

        public unsafe void Write(object obj, long offset)
        {
            IntPtr lpBaseAddress = IntPtr.Zero;
            MACM_SHARED_MEMORY_GPU_ENTRY gpuEntry = new MACM_SHARED_MEMORY_GPU_ENTRY();
            try
            {
                long ddFileOffset = offset / (long)this.AllocationGranularity * (long)this.AllocationGranularity;
                long len = (long)Marshal.SizeOf(gpuEntry);
                lpBaseAddress = Win32API.MapViewOfFile(this.hMMF, Win32API.FileMapAccess.FileMapWrite, ddFileOffset, (int)offset + (int)len);
                if (lpBaseAddress == IntPtr.Zero) throw new Win32Exception();
                IntPtr ptr = !(lpBaseAddress == IntPtr.Zero) ? new IntPtr(lpBaseAddress.ToInt64() + offset) : throw new Win32Exception();
                //gpuEntry = (MACM_SHARED_MEMORY_GPU_ENTRY)Marshal.PtrToStructure(ptr, typeof(MACM_SHARED_MEMORY_GPU_ENTRY));
                Marshal.StructureToPtr(obj, ptr, true);
                Win32API.FlushViewOfFile(lpBaseAddress, (int)len);
            }
            finally
            {
                if (lpBaseAddress != IntPtr.Zero)
                    Win32API.UnmapViewOfFile(lpBaseAddress);
            }
        }

        public unsafe object Read(long offset)
        {
            IntPtr lpBaseAddress = IntPtr.Zero;
            try
            {
                long ddFileOffset = offset / (long)this.AllocationGranularity * (long)this.AllocationGranularity;
                long num1 = offset % (long)this.AllocationGranularity + (long)this.AllocationGranularity;
                long num2 = offset - ddFileOffset;
                lpBaseAddress = Win32API.MapViewOfFile(this.hMMF, Win32API.FileMapAccess.FileMapRead, ddFileOffset, (int)num1);
                if (lpBaseAddress == IntPtr.Zero)
                    throw new Win32Exception();

                //return this.Formatter.Deserialize((Stream) new UnmanagedMemoryStream((byte*) ((IntPtr) lpBaseAddress.ToPointer() + (IntPtr) num2), num1, num1, FileAccess.Read));
                return this.Formatter.Deserialize((Stream)new UnmanagedMemoryStream((byte*)(IntPtr)((Int64)lpBaseAddress.ToPointer() + (Int64)num2), num1, num1, FileAccess.ReadWrite));
            }
            finally
            {
                if (lpBaseAddress != IntPtr.Zero)
                    Win32API.UnmapViewOfFile(lpBaseAddress);
            }
        }

        public unsafe object Read(long offset, int size)
        {
            IntPtr lpBaseAddress = IntPtr.Zero;
            try
            {
                long ddFileOffset = offset / (long)this.AllocationGranularity * (long)this.AllocationGranularity;
                long num1 = (long)size;
                long num2 = offset - ddFileOffset;
                lpBaseAddress = Win32API.MapViewOfFile(this.hMMF, Win32API.FileMapAccess.FileMapRead, ddFileOffset, (int)num1);
                if (lpBaseAddress == IntPtr.Zero)
                    throw new Win32Exception();
                return this.Formatter.Deserialize((Stream)new UnmanagedMemoryStream((byte*)(IntPtr)((Int64)lpBaseAddress.ToPointer() + (Int64)num2), num1, num1, FileAccess.Read));
            }
            finally
            {
                if (lpBaseAddress != IntPtr.Zero)
                    Win32API.UnmapViewOfFile(lpBaseAddress);
            }
        }
        public unsafe int Read(byte[] buffer, int bytesToRead, long offset)
        {
            IntPtr lpBaseAddress = IntPtr.Zero;
            try
            {
                long ddFileOffset = offset / (long)this.AllocationGranularity * (long)this.AllocationGranularity;
                long len = (long)bytesToRead;
                lpBaseAddress = Win32API.MapViewOfFile(this.hMMF, Win32API.FileMapAccess.FileMapRead, ddFileOffset, (int)offset + (int)len);
                if (lpBaseAddress == IntPtr.Zero) throw new Win32Exception();

                long num1 = offset % (long)this.AllocationGranularity + (long)this.AllocationGranularity;
                long num2 = offset - ddFileOffset;
                IntPtr ptr = !(lpBaseAddress == IntPtr.Zero) ? new IntPtr(lpBaseAddress.ToInt64() + offset) : throw new Win32Exception();

                UnmanagedMemoryStream unmanagedMemoryStream = new UnmanagedMemoryStream((byte*)(IntPtr)((Int64)lpBaseAddress.ToPointer() + (Int64)num2), num1, num1, FileAccess.Read);
                byte[] numArray = new byte[bytesToRead];
                byte[] buffer1 = buffer;
                int count = bytesToRead;
                return unmanagedMemoryStream.Read(buffer1, 0, count);
            }
            finally
            {
                if (lpBaseAddress != IntPtr.Zero)
                    Win32API.UnmapViewOfFile(lpBaseAddress);
            }
        }

        public unsafe void Write(long offset, object obj, int size)
        {
            IntPtr lpBaseAddress = IntPtr.Zero;
            try
            {
                long ddFileOffset = offset / (long)this.AllocationGranularity * (long)this.AllocationGranularity;
                long num1 = (long)size;
                long num2 = offset - ddFileOffset;
                lpBaseAddress = Win32API.MapViewOfFile(this.hMMF, Win32API.FileMapAccess.FileMapRead, ddFileOffset, (int)num1);
                if (lpBaseAddress == IntPtr.Zero)
                    throw new Win32Exception();
                this.Formatter.Serialize((Stream)new UnmanagedMemoryStream((byte*)(IntPtr)((Int64)lpBaseAddress.ToPointer() + (Int64)num2), num1, num1, FileAccess.Write), obj);
                Win32API.FlushViewOfFile(lpBaseAddress, (int)num1);
            }
            finally
            {
                if (lpBaseAddress != IntPtr.Zero)
                    Win32API.UnmapViewOfFile(lpBaseAddress);
            }
        }

        public unsafe void Write(byte[] buffer, int bytesToWrite, long offset)
        {
            IntPtr lpBaseAddress = IntPtr.Zero;
            try
            {
                long ddFileOffset = offset / (long)this.AllocationGranularity * (long)this.AllocationGranularity;
                long num1 = offset % (long)this.AllocationGranularity + (long)this.AllocationGranularity;
                long num2 = offset - ddFileOffset;
                lpBaseAddress = Win32API.MapViewOfFile(this.hMMF, Win32API.FileMapAccess.FileMapWrite, ddFileOffset, (int)num1);
                if (lpBaseAddress == IntPtr.Zero)
                    throw new Win32Exception();
                new UnmanagedMemoryStream((byte*)(IntPtr)((Int64)lpBaseAddress.ToPointer() + (Int64)num2), num1, num1, FileAccess.Write).Write(buffer, 0, bytesToWrite);
                Win32API.FlushViewOfFile(lpBaseAddress, (int)num1);
            }
            finally
            {
                if (lpBaseAddress != IntPtr.Zero)
                    Win32API.UnmapViewOfFile(lpBaseAddress);
            }
        }

        public void ReadMACMHeader(ref MACM_SHARED_MEMORY_HEADER header)
        {
            IntPtr num = IntPtr.Zero;
            try
            {
                num = Win32API.MapViewOfFile(this.hMMF, Win32API.FileMapAccess.FileMapRead, (long)(0U / this.AllocationGranularity * this.AllocationGranularity), Marshal.SizeOf((object)header));
                header = !(num == IntPtr.Zero) ? (MACM_SHARED_MEMORY_HEADER)Marshal.PtrToStructure(num, typeof(MACM_SHARED_MEMORY_HEADER)) : throw new Win32Exception();
            }
            finally
            {
                if (num != IntPtr.Zero)
                    Win32API.UnmapViewOfFile(num);
            }
        }

        public void ReadMACMGpuEntry(ref MACM_SHARED_MEMORY_GPU_ENTRY gpuEntry, long offset)
        {
            IntPtr lpBaseAddress = IntPtr.Zero;
            try
            {
                lpBaseAddress = Win32API.MapViewOfFile(this.hMMF, Win32API.FileMapAccess.FileMapRead, offset / (long)this.AllocationGranularity * (long)this.AllocationGranularity, (int)(offset + (long)Marshal.SizeOf((object)gpuEntry)));
                IntPtr ptr = !(lpBaseAddress == IntPtr.Zero) ? new IntPtr(lpBaseAddress.ToInt64() + offset) : throw new Win32Exception();
                gpuEntry = (MACM_SHARED_MEMORY_GPU_ENTRY)Marshal.PtrToStructure(ptr, typeof(MACM_SHARED_MEMORY_GPU_ENTRY));
            }
            finally
            {
                if (lpBaseAddress != IntPtr.Zero)
                    Win32API.UnmapViewOfFile(lpBaseAddress);
            }
        }

        public void ReadMAHMHeader(ref MAHM_SHARED_MEMORY_HEADER header)
        {
            IntPtr num = IntPtr.Zero;
            try
            {
                num = Win32API.MapViewOfFile(this.hMMF, Win32API.FileMapAccess.FileMapRead, (long)(0U / this.AllocationGranularity * this.AllocationGranularity), Marshal.SizeOf((object)header));
                header = !(num == IntPtr.Zero) ? (MAHM_SHARED_MEMORY_HEADER)Marshal.PtrToStructure(num, typeof(MAHM_SHARED_MEMORY_HEADER)) : throw new Win32Exception();
            }
            finally
            {
                if (num != IntPtr.Zero)
                    Win32API.UnmapViewOfFile(num);
            }
        }

        public void ReadMAHMEntry(ref MAHM_SHARED_MEMORY_ENTRY entry, long offset)
        {
            IntPtr lpBaseAddress = IntPtr.Zero;
            try
            {
                long num1 = offset / (long)this.AllocationGranularity;
                long num2 = offset - num1 * (long)this.AllocationGranularity;
                long num3 = (long)Marshal.SizeOf((object)entry);
                lpBaseAddress = Win32API.MapViewOfFile(this.hMMF, Win32API.FileMapAccess.FileMapRead, num1 * (long)this.AllocationGranularity, Convert.ToInt32(num2 + num3));
                IntPtr ptr = !(lpBaseAddress == IntPtr.Zero) ? new IntPtr(lpBaseAddress.ToInt64() + num2) : throw new Win32Exception();
                entry = (MAHM_SHARED_MEMORY_ENTRY)Marshal.PtrToStructure(ptr, typeof(MAHM_SHARED_MEMORY_ENTRY));
            }
            finally
            {
                if (lpBaseAddress != IntPtr.Zero)
                    Win32API.UnmapViewOfFile(lpBaseAddress);
            }
        }

        public void ReadMAHMGpuEntry(ref MAHM_SHARED_MEMORY_GPU_ENTRY gpuEntry, long offset)
        {
            IntPtr lpBaseAddress = IntPtr.Zero;
            try
            {
                long num1 = offset / (long)this.AllocationGranularity;
                long num2 = offset - num1 * (long)this.AllocationGranularity;
                long num3 = (long)Marshal.SizeOf((object)gpuEntry);
                lpBaseAddress = Win32API.MapViewOfFile(this.hMMF, Win32API.FileMapAccess.FileMapRead, num1 * (long)this.AllocationGranularity, Convert.ToInt32(num2 + num3));
                IntPtr ptr = !(lpBaseAddress == IntPtr.Zero) ? new IntPtr(lpBaseAddress.ToInt64() + num2) : throw new Win32Exception();
                gpuEntry = (MAHM_SHARED_MEMORY_GPU_ENTRY)Marshal.PtrToStructure(ptr, typeof(MAHM_SHARED_MEMORY_GPU_ENTRY));
            }
            finally
            {
                if (lpBaseAddress != IntPtr.Zero)
                    Win32API.UnmapViewOfFile(lpBaseAddress);
            }
        }


        public long Size(object T)
        {
            MemoryStream memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize((Stream)memoryStream, T);
            return memoryStream.Length;
        }

        public void Dispose()
        {
            if (this.hMMF != IntPtr.Zero)
                Win32API.CloseHandle(this.hMMF);
            this.hMMF = IntPtr.Zero;
        }

        public static IntPtr OffsetPointer(IntPtr src, int offset)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return new IntPtr(src.ToInt32() + offset);
                case 8:
                    return new IntPtr(src.ToInt64() + (long)offset);
                default:
                    throw new NotSupportedException("Pointer arithmatic not supported.");
            }
        }

        public static IntPtr OffsetPointer(IntPtr src, long offset)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return new IntPtr((long)src.ToInt32() + offset);
                case 8:
                    return new IntPtr(src.ToInt64() + offset);
                default:
                    throw new NotSupportedException("Pointer arithmatic not supported.");
            }
        }
    }
}
