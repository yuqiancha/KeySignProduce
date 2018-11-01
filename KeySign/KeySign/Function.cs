using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeySign
{
    class Function
    {

        public static ushort CRChware(ushort data, ushort genpoly, ushort crc)
        {
            int i = 8;
            data <<= 8;
            for (i = 8; i > 0; i--)
            {
                if (((data ^ crc) & 0x8000) != 0)
                {
                    crc = (ushort)((crc << 1) ^ genpoly);
                }
                else
                {
                    crc <<= 1;
                }
                data <<= 1;
            }
            return crc;
        }


        /// <summary>
        /// 十六进制String转化为BYTE数组
        /// </summary>
        /// <param name="hexString">参数：输入的十六进制String</param>
        /// <returns>BYTE数组</returns>
        public static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "").Replace("\r", "").Replace("\n", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";

            byte[] returnBytes = new byte[hexString.Length / 2];

            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;

        }

    }
}
