using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CherngerTechnology
{
    class SmartKey
    {
        [DllImport("SL_Dll.dll")]
        public static extern int UsbReadAll(byte[] RPW, byte[] ReadBuffer);
        [DllImport("SL_Dll.dll")]
        public static extern int UsbWriteAll(byte[] WPW, byte[] WriteBuffer);
        [DllImport("SL_Dll.dll")]
        public static extern int UsbRead(byte[] RPW, byte wdAddress);
        [DllImport("SL_Dll.dll")]
        public static extern int UsbWrite(byte[] WPW, byte wdAddress, byte ival);
        [DllImport("SL_Dll.dll")]
        public static extern int UsbEncrypt(byte bSeed, ushort usLength, byte[] EncryptBuffer);
        [DllImport("SL_Dll.dll")]
        public static extern int UsbDecrypt(byte bSeed, ushort usLength, byte[] DecryptBuffer);

        private int RandomVal(int minValue, int maxValue)
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            return rnd.Next(minValue, maxValue);
        }

        public int ReadPortKey(int num)
        {
            int ErrorCode = -99;
            byte[] RPW = { 0xCE, 0xE8, 0x08, 0x10 };
            List<byte> KEY = new List<byte>
            { 0x34, 0x23, 0x1B, 0x0E, 0x8D, 0xAA, 0xC4, 0x05, 0x4B, 0xA9,
                0xD5, 0x68, 0x13, 0x12, 0xAA, 0x55, 0x12, 0x28, 0xEE, 0x89,
                0xD8, 0xA2, 0x88, 0x52, 0xA9, 0x3A, 0x38, 0xA6, 0x10, 0x2B,
                0xDA, 0x1B, 0x5C, 0x1D, 0x44, 0x22, 0x93, 0x75, 0x65, 0xC4,
                0x91, 0x04, 0x37, 0xC0, 0x07, 0xBC, 0x59, 0x76, 0x8B, 0xC3,
                0x0E, 0x8B, 0x34, 0x33, 0x52, 0x3A, 0xE3, 0x1B, 0xAD, 0xBD,
                0x23, 0x19, 0x0B, 0x07, 0x55, 0x74, 0xA9, 0x49, 0x2E, 0xDA,
                0x0C, 0xCC, 0x23, 0x72, 0x3B, 0x7B, 0x73, 0xD1, 0x60, 0xB1,
                0x33, 0x18, 0x99, 0x69, 0x29, 0x93, 0x47, 0xB5, 0x69, 0x1C,
                0xC1, 0x94, 0x76, 0x1D, 0x43, 0x9B, 0xA7, 0x2E, 0xE0, 0xB3,
                0x7D, 0x27, 0x0A, 0x53, 0x8A
            };

            for (int i = 0; i < num; i++)
            {
                int CN = RandomVal(0, 104);
                ErrorCode = UsbRead(RPW, Convert.ToByte(CN));

                if (ErrorCode > -1)
                {
                    //MessageBox.Show((CN + 1).ToString() + " | " + KEY[CN].ToString("X2") + " - " + Convert.ToByte(ErrorCode).ToString("X2"));

                    if (KEY[CN].Equals(Convert.ToByte(ErrorCode))) ErrorCode = 0;
                    else
                    {
                        ErrorCode = -99;
                        break;
                    }
                }
            }

            return ErrorCode;
        }
    }
}
