using System;
using System.Text;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Collections;

namespace detection.common
{
         public struct strJianDianCeng
        {
            public byte[] head;
            public byte jian;
            public byte[][] dianchen;
            public byte crc;
            public byte maxpoint;
            public byte[] maxlayerperjian;
            public byte[] dianperjian;
            public byte[] real18num;
            public byte[] t18num;
            public byte[] pointnum;
            public UInt16 total18num;
        }
     
    class MyMethod
    {

        static byte[] CrcTable = new byte[256]{
	        0, 94, 188, 226, 97, 63, 221, 131, 194, 156, 126, 32, 163, 253, 31, 65,
	        157, 195, 33, 127, 252, 162, 64, 30, 95, 1, 227, 189, 62, 96, 130, 220,
	        35, 125, 159, 193, 66, 28, 254, 160, 225, 191, 93, 3, 128, 222, 60, 98,
	        190, 224, 2, 92, 223, 129, 99, 61, 124, 34, 192, 158, 29, 67, 161, 255,
	        70, 24, 250, 164, 39, 121, 155, 197, 132, 218, 56, 102, 229, 187, 89, 7,
	        219, 133, 103, 57, 186, 228, 6, 88, 25, 71, 165, 251, 120, 38, 196, 154,
	        101, 59, 217, 135, 4, 90, 184, 230, 167, 249, 27, 69, 198, 152, 122, 36,
	        248, 166, 68, 26, 153, 199, 37, 123, 58, 100, 134, 216, 91, 5, 231, 185,
	        140, 210, 48, 110, 237, 179, 81, 15, 78, 16, 242, 172, 47, 113, 147, 205,
	        17, 79, 173, 243, 112, 46, 204, 146, 211, 141, 111, 49, 178, 236, 14, 80,
	        175, 241, 19, 77, 206, 144, 114, 44, 109, 51, 209, 143, 12, 82, 176, 238,
	        50, 108, 142, 208, 83, 13, 239, 177, 240, 174, 76, 18, 145, 207, 45, 115,
	        202, 148, 118, 40, 171, 245, 23, 73, 8, 86, 180, 234, 105, 55, 213, 139,
	        87, 9, 235, 181, 54, 104, 138, 212, 149, 203, 41, 119, 244, 170, 72, 22,
	        233, 183, 85, 11, 136, 214, 52, 106, 43, 117, 151, 201, 74, 20, 246, 168,
	        116, 42, 200, 150, 21, 75, 169, 247, 182, 232, 10, 84, 215, 137, 107, 53};

        #region 调用windowsAPI
        [DllImport("Kernel32.dll")]
        private static extern int GetPrivateProfileString(string strAppName, string strKeyName, string strDefault, StringBuilder sbReturnString, int nSize, string strFileName);
        [DllImport("Kernel32.dll")]
        private extern static int GetPrivateProfileStringA(string strAppName, string strKeyName, string sDefault, byte[] buffer, int nSize, string strFileName);
        [DllImport("Kernel32.dll")]
        private static extern int GetPrivateProfileInt(string strAppName, string strKeyName, int nDefault, string strFileName);
        //获取ini文件所有的section
        [DllImport("Kernel32.dll")]
        private extern static int GetPrivateProfileSectionNamesA(byte[] buffer, int iLen, string fileName);
        //获取指定Section的key和value        
        [DllImport("Kernel32.dll")]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpReturnedString, int nSize, string lpFileName);
        //根据传入参数的不同进行写入或修改或删除操作（返回值 Long，非零表示成功，零表示失败）
        [DllImport("Kernel32.dll")]
        public static extern long WritePrivateProfileString(string strAppName, string strKeyName, string strKeyValue, string strFileName);
        //添加一个section内容列表
        [DllImport("Kernel32.dll")]
        public static extern long WritePrivateProfileSection(string strAppName, string strkeyandvalue, string strFileName);
        #endregion

        public static byte CRC8(byte[] inbyte, int length)
        {
            byte retCRC = 0;
            for (int i = 0; i < length; i++) //查表校验
            {
                retCRC = CrcTable[retCRC ^ inbyte[i]];
            }
            return retCRC;
        }
        public static byte[] StructToBytes(object structObj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(structObj);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);

            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }

        /// <summary>
        /// byte数组转结构体
        /// </summary>
        /// <param>byte数组</param>
        /// <param>结构体类型</param>
        /// <returns>转换后的结构体</returns>
        public static object BytesToStuct(byte[] bytes, Type type)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                //返回空
                return null;
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
        }

        
        static public byte[] string16tobytes(string ss)
        {
            ss = ss.Replace(" ", "");
            if ((ss.Length % 2) != 0) ss += "";
            byte[] buff = new byte[ss.Length / 2];
            for (int i = 0; i < buff.Length; i++)
            {
                buff[i] = Convert.ToByte(ss.Substring(i * 2, 2), 16);
            }
                return buff;
        }
        //static public byte[] makecmdstring(byte CMDname, byte[] content)
        //{
        //    byte[] rtstring = new byte[content.Length + 4];
        //    rtstring[0] = 0xe6;
        //    rtstring[1] = 0xd5;
        //    rtstring[2] = Convert.ToByte(rtstring.Length);
        //    rtstring[3] = CMDname;
        //    for (int i = 0; i < content.Length; i++)
        //    {
        //        rtstring[4 + i] = content[i];
        //    }
        //    return rtstring;
        //}
        //static public byte[] makecmdstringcrc(byte CMDname, byte[] content)
        //{
        //    byte[] rtstring = new byte[content.Length + 5];
        //    rtstring[0] = 0xe6;
        //    rtstring[1] = 0xd5;
        //    rtstring[2] = Convert.ToByte(rtstring.Length);
        //    rtstring[3] = CMDname;
        //    for (int i = 0; i < content.Length; i++)
        //    {
        //        rtstring[4 + i] = content[i];
        //    }
        //    rtstring[content.Length + 4] = CRC8(rtstring, content.Length + 4);
        //    return rtstring;
        //}
        //static public byte[] makecmdstringaddr(byte CMDname, byte addr,byte[] content)
        //{
        //    byte[] rtstring = new byte[content.Length + 6];
        //    rtstring[0] = 0xe6;
        //    rtstring[1] = 0xd5;
        //    rtstring[2] = 0x00;
        //    rtstring[3] = addr;
        //    rtstring[4] = Convert.ToByte(rtstring.Length);
        //    rtstring[5] = CMDname;
        //    for (int i = 0; i < content.Length; i++)
        //    {
        //        rtstring[6 + i] = content[i];
        //    }
        //    return rtstring;
        //}
        static public byte[] makecmdstringcrcaddr(byte CMDname, byte addr,byte[] content)
        {
            byte[] rtstring = new byte[content.Length + 7];
            rtstring[0] = 0xe6;
            rtstring[1] = 0xd5;
            rtstring[2] = 0x00;
            rtstring[3] = addr;
            rtstring[4] = Convert.ToByte(rtstring.Length);
            rtstring[5] = CMDname;
            for (int i = 0; i < content.Length; i++)
            {
                rtstring[6 + i] = content[i];
            }
            rtstring[content.Length + 6] = CRC8(rtstring, content.Length + 6);
            return rtstring;
        }
        static public string bytestostr(byte[] con)
        {
            string ss="";
            for(int i=0;i<con.Length;i++)
            {
                ss += con[i].ToString("X2");
                //ss += con[i].ToString("X2") + " ";
                }
            return ss;
        }
        static public float bytestotemp(byte th, byte tl)
        {
            float rt;
            rt = -46.85F + 175.72F * (th * 256 + tl) / 65536.0F;
            return rt;
        }
        static public float bytes18totemp(byte th, byte tl)
        {
            float rt;
            if ((th & 0x80) == 0x80)
            {
                th = (byte)~th;
                tl = (byte)(~tl + 1);
                rt = -(th * 256 + tl) * 0.0625F;
            }
            else rt =  (th * 256 + tl) * 0.0625F;
            return rt;
        }
        static public float bytestohumi(byte Hh,byte Hl)
        {
            float rt;
            rt = -6.0F + 125.0F * (Hh * 256 + Hl) / 65536.0F;
            return rt;
        }
        static public float resNTCtotemp(UInt16 reso,float Re,float B)
        {
            //double[] const1 = new double[11]{
            //-1.1029829000844744e-13,
            //5.1898837031309443e-12,
            //7.0520776478129371e-10,
            //-3.7258210165392510e-08,
            //-8.6447931354095470e-07,
            //3.0316222102362708e-05,
            //3.3851129718355326e-03,
            //-2.1252692629380746e-01,
            //8.9669117295182641e+00,
            //-3.5622050273918103e+02,
            //8.1989597970115137e+03};
            //double tt=0;

            //for (int i = 0; i < 11; i++) tt =tt*10+ const1[i];
            ////tt = const1[10] * Math.Pow(res, 10) + const1[9] * Math.Pow(res, 9) + const1[8] * Math.Pow(res, 8) +
            ////    const1[7] * Math.Pow(res, 7) + const1[6] * Math.Pow(res, 6) + const1[5] * Math.Pow(res, 5) +
            ////    const1[4] * Math.Pow(res, 4) + const1[3] * Math.Pow(res, 3) +
            ////    const1[2] * Math.Pow(res, 2) + const1[1] * res + const1[10];
            //double[] con2 = new double[4] { 4, 3, 2, 1 };
            //tt = 0;
            //for (int i = 0; i < 4; i++) tt = tt * 1.5 + con2[i];
 //           const double 
   //         double tt=0;
            double resk = (reso) / 1000.0;
            double ttfenzi1=(Math.Log(resk/Re)/B)+1/298.15;

                return (float)((1/ttfenzi1)-273.15);
        }
        static public strJianDianCeng jiandianinfotostruct(byte[] inbytes)
        {
            strJianDianCeng sjdc=new strJianDianCeng();
            sjdc.maxpoint = 0;
             sjdc.total18num=0;
            sjdc.head = new byte[6];
            for(int i=0;i<6;i++)sjdc.head[i]=inbytes[i];
            sjdc.jian = inbytes[6];
            int index=7;
            sjdc.dianchen = new byte[sjdc.jian][];
            sjdc.maxlayerperjian = new byte[sjdc.jian];
            sjdc.real18num = new byte[sjdc.jian];
            sjdc.t18num = new byte[sjdc.jian] ;
            sjdc.pointnum = new byte[sjdc.jian];
            for (int j = 0; j < sjdc.jian; j++)
            {
                sjdc.pointnum[j] = inbytes[index];
                if (sjdc.maxpoint < sjdc.pointnum[j]) sjdc.maxpoint = sjdc.pointnum[j];

                sjdc.dianchen[j] = new byte[sjdc.pointnum[j]];
                sjdc.t18num[j] = 0;
                sjdc.real18num[j] = 0;
                if (sjdc.pointnum[j] != 0)
                {
                    index++;

                    for (int k = 0; k < sjdc.pointnum[j]; k++)
                    {
                        sjdc.dianchen[j][k] = inbytes[index];
                        sjdc.t18num[j] += inbytes[index];
                        sjdc.total18num += inbytes[index];
                        if (sjdc.maxlayerperjian[j] < inbytes[index]) sjdc.maxlayerperjian[j] = inbytes[index];
                        index++;
                    }
                    sjdc.real18num[j] = inbytes[index];
                }
                index ++;
            }
            sjdc.crc = inbytes[index];
                return sjdc;
        }
        static public float[][] tempa18perjian(byte[] inbytes, byte[] everylayerperjian)
        {
            int index = 6;
            int point = everylayerperjian.Length;
            float[][] tt = new float[point][];
            for (int i = 0; i < point; i++)
            {
                tt[i]=new float[everylayerperjian[i]] ;
                for (int j = 0; j < everylayerperjian[i]; j++)
                {
                    tt[i][j] = bytes18totemp(inbytes[index], inbytes[index + 1]);
                    index += 2;
                }
            }
            return tt;
        }
        static public int totalnumsinglejian(byte[] inbytes)
        {
            int rt=0;
            int index = 7;
            for (int i=0; i < inbytes[index]; i++) rt += inbytes[index+1 + i];
                return rt;
        }
        static public byte jiediantypetobyte(string stype)
        {
            byte bb;
            if (stype == "通风窗")
            {
                bb = 0x01;
            }
            else if (stype == "轴流机窗")
            {
                bb = 0x02;
            }
            else if (stype == "通风口")
            {
                bb = 0x03;
            }
            else if (stype == "内环流")
            {
                bb = 0x04;
            }
            else if (stype == "空调")
            {
                bb = 0x05;
            }
            else
            {
                bb = 0x06;
            }
            return bb;
        }
        static public byte getmaxctrlingjiediannum(byte[] jiedinnum, bool[] bctrling)
        {
            int length = jiedinnum.Length;
            byte tempmaxjiediannum = 0;
            for (int i = 0; i < length; i++)
            {
                if (bctrling[i])
                {
                    if (jiedinnum[i] > tempmaxjiediannum) tempmaxjiediannum = jiedinnum[i];
                } 
            }
            return tempmaxjiediannum;
        }
        static public byte[] makectrljiedianbytes(byte[] jiedinnum, bool[] bctrling)
        {
            byte maxjidnum = MyMethod.getmaxctrlingjiediannum(jiedinnum, bctrling);
            byte ctrljiedbytenum = (byte)Math.Ceiling(maxjidnum / 8.0);
            byte[] cmd = new byte[ctrljiedbytenum];
            for(int i=0;i<ctrljiedbytenum;i++)cmd[i]=0x00;
            int whichbyte;
            int whichpos;
            for (int i = 0; i < jiedinnum.Length; i++)
            {
                if (bctrling[i])
                {
                    whichbyte = ((jiedinnum[i] % 8) == 0) ? ((jiedinnum[i] / 8) - 1) : (jiedinnum[i] / 8);
                    whichpos = ((jiedinnum[i] % 8) == 0) ? 7 : ((jiedinnum[i] % 8)-1);
                    cmd[whichbyte] |= (byte)((byte)0x01 << whichpos);
                }
            }
                return cmd;
        }
        static private int getnumindexoffsynum(byte jidiannum, byte[] fsynum)
        {
            
            for (int j = 0; j < fsynum.Length; j++)
            {
                if ((jidiannum & 0x7f) == fsynum[j]) return j;
            }
            return 0xff;

        }
        static public string[] unpackjiedianstatus(byte jidianstah,byte jidianstal, byte jidiannum,byte fsyindex,byte [] fsydata)
        {
            string[] ss = new string[4];
            if ((jidianstah & 0x80) == 0x80)
            {
                byte jidiatype=(byte)(jidiannum & 0x7f);

                if (jidiatype == 0x01 || jidiatype == 0x02 || jidiatype == 0x03)
                {
                    if (((jidianstah>>4) & 0x07) == 0x00) ss[0] = "关到位";
                    else if (((jidianstah>>4) & 0x07) == 0x01) ss[0] = "开动作";
                    else if (((jidianstah>>4) & 0x07) == 0x02) ss[0] = "开到位";
                    else if (((jidianstah>>4) & 0x07) == 0x03) ss[0] = "关动作";
                    else if (((jidianstah>>4) & 0x07) == 0x04) ss[0] = "停在中间";
                    else ss[0] = "异常";
                }
                else ss[0] = "无门窗";
                if (jidiatype == 0x04 || jidiatype == 0x02 || jidiatype == 0x03  || jidiatype == 0x06)
                {
                    if((jidianstah&0x08)==0x08)ss[1]="风机开";
                    else ss[1]="风机关";
                    if((jidianstah&0x04)==0x04)ss[2]="风机过流";
                    else ss[2] = (jidianstal * .1).ToString("f2") + "A";
                                  
                }
                else if (jidiatype == 0x05)
                {
                    if ((jidianstah & 0x08) == 0x08) ss[1] = "空调开";
                    else ss[1] = "空调关";
                    if ((jidianstah & 0x04) == 0x04) ss[2] = "空调过流";
                    ss[2] = (jidianstal * .1).ToString("f2") + "A";
                    
                }
                else if (jidiatype == 0x06)
                {
                    if ((jidianstah & 0x08) == 0x08) ss[1] = "调质机开";
                    else ss[1] = "调质机关";
                    if ((jidianstah & 0x04) == 0x04) ss[2] = "调质机过流";
                    ss[2] = (jidianstal * .1).ToString("f2") + "A";
                   
                }
                else
                {
                    ss[1] = "无强电";
                    ss[2] = "无强电";
                }
                //if (jidiatype != 0x01)
                //{
                //    ss[2] = (jidianstal * .1).ToString("f2") + "A";
                //}
                //else
                //{
                //    ss[2] = "无强电";
                //}
                 
                if ((jidiannum & 0x80) == 0x80)//有风速仪
                {
                    ss[3] = (fsydata[fsyindex] * 0.056).ToString("f2");
                }
                else
                {
                    ss[3] = "无";
                }
            }
            else
            {
                ss[0] = "无响应";
                ss[1] = "无响应";
                ss[2] = "无响应";
                ss[3] = "无响应";
            }
            return ss;
        }
        static public string jiediantypetostring(byte stype)
        {
            string bb;
            if (stype ==  0x01)
            {
                bb = "通风窗";
            }
            else if (stype ==  0x02)
            {
                bb = "轴流机窗";
            }
            else if (stype == 0x03)
            {
                bb = "通风口";
            }
            else if (stype ==  0x04)
            {
                bb = "内环流";
            }
            else if (stype == 0x05)
            {
                bb = "空调";
            }
            else
            {
                bb = "调质机";
            }
            return bb;
        }

        static public void SendBytesData(SerialPort serialPort, byte[] bytesSend)
        {

            if (serialPort.IsOpen)
                serialPort.Write(bytesSend, 0, bytesSend.Length);
        }

        static public ArrayList ReadKeys(string sectionName)
        {

            byte[] buffer = new byte[5120];
            int rel = GetPrivateProfileStringA(sectionName, null, "", buffer, buffer.GetUpperBound(0), "e:\\test.ini");

            int iCnt, iPos;
            ArrayList arrayList = new ArrayList();
            string tmp;
            if (rel > 0)
            {
                iCnt = 0; iPos = 0;
                for (iCnt = 0; iCnt < rel; iCnt++)
                {
                    if (buffer[iCnt] == 0x00)
                    {
                        tmp = System.Text.ASCIIEncoding.Default.GetString(buffer, iPos, iCnt - iPos).Trim();
                        iPos = iCnt + 1;
                        if (tmp != "")
                            arrayList.Add(tmp);
                    }
                }
            }
            return arrayList;
        }
        

    }
}