using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Trainer
{
    class Program
    {
        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);
        
        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);

        [DllImport("kernel32.dll")]
        static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        static int ProcID;
        static IntPtr Handle;

        public static class OutIgnore<T>
        {
            [ThreadStatic]
            public static T Ignored;
        }	
        public static void WriteMemory(int Address, byte[] data)
        {
            WriteProcessMemory(Handle, (IntPtr)Address, data, data.Length, out OutIgnore<IntPtr>.Ignored);
        }
		
        public static byte[] Read(IntPtr Address, int length)
        {
            byte[] ret = new byte[length];
            ReadProcessMemory(Handle, Address, ret, (UInt32)ret.Length, ref OutIgnore<uint>.Ignored);
            return ret;
        }
		
        public static int ReadInt32(IntPtr Address, int len)
        {
            return BitConverter.ToInt32(Read(Address, len), 0);
        }
		
        public static byte ByteReadOffset(IntPtr addr, int offset, int len)
        {
            int address = ReadInt32(addr, len);
            return Read((IntPtr)address + offset, 1)[0];
        }
		
        public static float FloatReadOffset(IntPtr addr, int offset, int len)
        {
            int address = ReadInt32(addr, len);
            byte[] naddr = Read((IntPtr)address + offset, sizeof(float));
            return BitConverter.ToSingle(naddr, 0);
        }
		
        public static int IntReadOffset(IntPtr addr, int offset, int len)
        {
            int address = ReadInt32(addr, len);
            byte[] naddr = Read((IntPtr)address + offset, sizeof(int));
            return BitConverter.ToInt32(naddr, 0);
        }
		
        public static void WriteOffset(IntPtr addr, int offset, byte[] res, int len)
        {
            int address = ReadInt32(addr, len);
            WriteMemory(address + offset, res);
        }
		
        static void Main(string[] args)
        {
            Console.WriteLine("Started. W8 process....");
            while (ProcID == 0)
            {
                foreach (Process id in Process.GetProcessesByName("gta_sa"))
                {
                    ProcID = id.Id;
                }
            }

            Handle = OpenProcess(0x001F0FFF, false, ProcID);

            bool radar = false;
            bool free_repain = false;
			
            Console.WriteLine("Injected");
			
            while (true)
            {
                /*Gray Radar*/
                WriteMemory(0xA444A4, (!radar) ? new byte[] { 0x0 } : new byte[] { 0x1 });

                string cmd = Console.ReadLine();

                if (cmd == "exit")
                    return;

                if (cmd == "free_repain")
                {
                    free_repain = (!free_repain);
                    WriteMemory(0xA444A4, (!free_repain) ? new byte[] { 0x0 } : new byte[] { 0x1 });
                }
                if (cmd == "get")
                {
                    Console.WriteLine("State: " + CPed.GetByteOffset(0x46F));
                    Console.WriteLine("Animation State: " + CPed.GetByteOffset(0x4DF));
                    Console.WriteLine("Money: " + ReadInt32((IntPtr)0xB7CE50, 4));
                    Console.WriteLine("HP: " + CPed.GetFloatOffset(0x540));
                }
                if (cmd == "money")
                {
                    WriteMemory(0xB7CE50, BitConverter.GetBytes (60000));
                    Console.WriteLine("New money value: " + ReadInt32((IntPtr)0xB7CE50, 4));
                }
                if (cmd == "radar")
				{
                    Console.WriteLine("Gray radar " + ((radar = (!radar)) ? "Activated" : "Deactivated"));
                }
            }
        }
    }
}
