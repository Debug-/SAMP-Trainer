using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trainer
{
    class CPed
    {
        public const int CPed_Addr = 0xB6F5F0;
		/*Read Offsets*/
        public static int GetIntOffset(int offset)
        {
            return Program.IntReadOffset((IntPtr)CPed_Addr, offset, 0x7C4);
        }
        public static byte GetByteOffset(int offset)
        {
            return Program.ByteReadOffset((IntPtr)CPed_Addr, offset, 0x7C4);
        }
        public static float GetFloatOffset(int offset)
        {
            return Program.FloatReadOffset((IntPtr)CPed_Addr, offset, 0x7C4);
        }	
		/*Write Offsets*/
		public static void Write(byte[] res, int offset)
        {
            Program.WriteOffset((IntPtr)CPed_Addr, offset, res, 0x7C4);
        }
    }
}
