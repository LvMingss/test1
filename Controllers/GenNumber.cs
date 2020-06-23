using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using urban_archive.Models;

namespace urban_archive.GenNumber
{
    public class GenNumber
    {
        public struct InInfo
        {
            //以下三个参数需用户输入
            public int drawerWidth;     //层宽度 
            public string totalMix;     //案卷字符串
            public int maxDrawerNum;    //最大层数

            //以下两个参数需查询数据库计算
            public int remainWidth;     //剩余宽度
            public string curPaiJiaHao; //当前排架号

            //以下两参数通过解析用户输入字符串计算
            public int totalVolume;     //总卷数
            public int totalWidth;      //总厚度
        }
        public struct OutInfo
        {
            public StringBuilder[] s;   //保存中间过程字符串
            public string errorMsg;     //用于提示柜子已满
            public int changeDraIndex;  //换层后对于返回字符串数组 outInfo.s 从此下标开始填入数据
        }
        public struct ReturnInfo
        {
            public StringBuilder[] s;  //返回含有排架号信息的字符串
            public int currentDraNum;  //返回当前层号   
            public int remainWidth;    //返回当前层剩余宽度
        }

#region 静态全局变量
        public static StringBuilder[] g_s = new StringBuilder[1000];
        public static int g_index = 0;
        public static InInfo g_midInfo = new InInfo();
        public static int g_drawerNum;
        public static int g_remainWidth;
        public static bool g_isFirst;
        public static bool g_isChangeCab; //add 20090831 标识是否换换子了
        public static int g_totalVolume;
        public static bool g_isFilled;   //add 20090921 标识是否已经填充了中间字符串
#endregion
        

        private static void GenWithAdequateSpace(InInfo inInfo, ref OutInfo outInfo)
        {
            for (int i = 0; i < inInfo.totalVolume; i++)
            {
                //获取当前排架号的第七、八位数值即个位数与十位数
                int curSingle = Convert.ToInt32(inInfo.curPaiJiaHao.Substring(7, 1));
                int curTen = Convert.ToInt32(inInfo.curPaiJiaHao.Substring(6, 1));

                //计算新号的个位和十位数值
                int newSingle = (i + 1) % 10 + curSingle;
                int newTen = (i + 1) / 10 + curTen;
                newTen += newSingle / 10;
                newSingle = newSingle % 10;

                //向返回字符串中加入起始排号的前六位
               
                outInfo.s[i+outInfo.changeDraIndex].Append(inInfo.curPaiJiaHao.Substring(0,6));
                //加入第七位与第八位构成完整的八位排架号
                outInfo.s[i + outInfo.changeDraIndex].Append(Convert.ToString(newTen));
                outInfo.s[i + outInfo.changeDraIndex].Append(Convert.ToString(newSingle));
            }

            //返回除排架号外的额外信息
            g_drawerNum = Convert.ToInt32(inInfo.curPaiJiaHao.Substring(5,1));
            g_remainWidth = inInfo.remainWidth - inInfo.totalWidth;
        }
        private static void GenWithInadquateSpace(InInfo inInfo, ref OutInfo outInfo)
        {
            //以输入的字符串的顺序把案卷厚度装入数组array中
            int[] array = new int[inInfo.totalVolume];
            ArrangeArray(inInfo, ref outInfo, array );

            //查找当前层剩余宽度能装到第几份案卷
            int sum = 0;
            int index = 0;
            for (int i = 0; i < inInfo.totalVolume; i++)
            {
                sum += array[i];
                if (sum > inInfo.remainWidth)
                {
                    index = i - 1;
                    sum -= array[i];
                    break;
                }
            }
            //本抽屉不足以放宽度最小的，下移一个抽屉
            if (index == -1)
            {
                InInfo newInfo = new InInfo();
                newInfo = inInfo;
                //重新计算起始排架号
                newInfo.curPaiJiaHao = inInfo.curPaiJiaHao.Substring(0, 5) + (Convert.ToInt32(inInfo.curPaiJiaHao.Substring(5, 1)) + 1).ToString() + "00";
                if (Convert.ToInt32(newInfo.curPaiJiaHao.Substring(5, 1)) > inInfo.maxDrawerNum) //modify 20090831
                {
                    outInfo.errorMsg = "此柜已满，请换下一柜。";
                    return;
                }
                else
                {
                    newInfo.remainWidth = inInfo.drawerWidth;
                    GenShelfNumber(newInfo, ref outInfo);
                }
            }
            //本抽屉至少能放一个宽度最小的
            else
            {
                for (int i = 0; i <= index; i++)
                {
                    //获取当前排架号的第七、八位数值即个位数与十位数
                    int curSingle = Convert.ToInt32(inInfo.curPaiJiaHao.Substring(7, 1));
                    int curTen = Convert.ToInt32(inInfo.curPaiJiaHao.Substring(6, 1));

                    //计算新号的个位和十位数值
                    int newSingle = (i + 1) % 10 + curSingle;
                    int newTen = (i + 1) / 10 + curTen;
                    newTen += newSingle / 10;
                    newSingle = newSingle % 10;

                    //向返回字符串中加入起始排号的前六位
                    outInfo.s[i+outInfo.changeDraIndex].Append(inInfo.curPaiJiaHao.Substring(0, 6));
                    //加入第七位与第八位构成完整的八位排架号
                    outInfo.s[i+outInfo.changeDraIndex].Append(Convert.ToString(newTen));
                    outInfo.s[i+outInfo.changeDraIndex].Append(Convert.ToString(newSingle));
                }
                InInfo newInfo = new InInfo();
                CaculateNewInfo(inInfo, ref newInfo, array, index, sum);
                if (Convert.ToInt32(newInfo.curPaiJiaHao.Substring(5,1)) > inInfo.maxDrawerNum) //modify 20090831
                {
                    outInfo.errorMsg = "此柜已满，请换下一柜。";
                    g_midInfo.totalVolume = inInfo.totalVolume - (index + 1); //modify 20090831 --换柜子后的中间信息
                    g_midInfo.totalMix = newInfo.totalMix; 
                    g_isChangeCab = true;  
                    return;
                }
                else
                {
                    outInfo.changeDraIndex += index + 1;
                    GenShelfNumber(newInfo, ref outInfo);
                }
            }

        }

        private static void ArrangeOrder(InInfo inInfo, ref OutInfo outInfo)
        {
            string[] sTemp1 = inInfo.totalMix.Split(',');
            foreach (string s in sTemp1)
            {
                string[] sTemp2 = s.Split('*');

                switch (Int32.Parse(sTemp2[0]))
                {
                    case 1:
                        AttachDetailInfo(outInfo.s, Int32.Parse(sTemp2[1]), inInfo.totalVolume, "1");
                        break;
                    case 2:
                        AttachDetailInfo(outInfo.s, Int32.Parse(sTemp2[1]), inInfo.totalVolume, "2");
                        break;
                    case 3:
                        AttachDetailInfo(outInfo.s, Int32.Parse(sTemp2[1]), inInfo.totalVolume, "3");
                        break;
                    case 4:
                        AttachDetailInfo(outInfo.s, Int32.Parse(sTemp2[1]), inInfo.totalVolume, "4");
                        break;
                    case 5:
                        AttachDetailInfo(outInfo.s, Int32.Parse(sTemp2[1]), inInfo.totalVolume, "5");
                        break;
                }
            }

        }
        private static void ArrangeArray(InInfo inInfo, ref OutInfo outInfo, int[] array)
        {
            string[] sTemp1 = inInfo.totalMix.Split(',');
            foreach (string s in sTemp1)
            {
                string[] sTemp2 = s.Split('*'); 
                
                switch (Int32.Parse(sTemp2[0]))
                {
                    case 1:
                        FillInArray(array, Int32.Parse(sTemp2[1]), inInfo.totalVolume, 1);
                        break;
                    case 2:
                        FillInArray(array, Int32.Parse(sTemp2[1]), inInfo.totalVolume, 2);
                        break;
                    case 3:
                        FillInArray(array, Int32.Parse(sTemp2[1]), inInfo.totalVolume, 3);
                        break;
                    case 4:
                        FillInArray(array, Int32.Parse(sTemp2[1]), inInfo.totalVolume, 4);
                        break;
                    case 5:
                        FillInArray(array, Int32.Parse(sTemp2[1]), inInfo.totalVolume, 5);
                        break;
                }
            }
        }
        private static void AttachDetailInfo(StringBuilder[] s, int num, int totalNum, string detailInfo)
        {
            int counter = 0;
            for (int i = 0; i < totalNum; i++ )
            {
                for (int j = 0; j < num; j++ )
                {
                    if (s[i+j] == null)
                    {
                        counter++;
                        StringBuilder temp = new StringBuilder();
                        temp.Append(detailInfo);
                        temp.Append("-----");
                        s[i+j] = temp;
                    }
                }
                if (counter == num)
                    break;
            }
        }
        private static void FillInArray(int[] array, int num, int totalNum, int value)
        {
            int counter = 0;
            for (int i = 0; i < totalNum; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    if (array[i + j] == 0)
                    {
                        counter++;
                        array[i + j] = value;
                    }
                }
                if (counter == num)
                    break;
            }
        }
        private static void CaculateNewInfo(InInfo info, ref InInfo newInfo, int[] array, int index, int usedSumWidth)
        {//
            newInfo.drawerWidth = info.drawerWidth;
            newInfo.remainWidth = info.drawerWidth;
            newInfo.totalVolume = info.totalVolume - (index + 1);
            newInfo.totalWidth = info.totalWidth - usedSumWidth;
            newInfo.maxDrawerNum = info.maxDrawerNum;
            newInfo.curPaiJiaHao = info.curPaiJiaHao.Substring(0,5) + (Convert.ToInt32(info.curPaiJiaHao.Substring(5,1)) + 1).ToString() + "00";
            //计算新的信息字符串(中间数据)
            StringBuilder s = new StringBuilder();
            int temp = array[index + 1];
            int counter = 1;
            s.Append(Convert.ToString(temp));
            s.Append("*");
            if ((index+1) == (array.Length-1))
                s.Append(Convert.ToString(counter));

            for (int i = index + 2; i < array.Length; i++ )
            {
                if (temp != array[i])
                {
                    s.Append(Convert.ToString(counter));
                    s.Append(",");
                    temp = array[i];
                    s.Append(Convert.ToString(array[i]));
                    s.Append("*");
                    counter = 1;
                    //add by niutianbo,date:20090924
                    if (i == array.Length - 1)
                        s.Append(Convert.ToString(counter));
                }
                else
                {
                    counter++;
                    if (i == array.Length - 1)
                        s.Append(Convert.ToString(counter));
                }
            }

            //新字符串totalMix 的结果
            newInfo.totalMix = s.ToString();
        }

        private static void CombineString(StringBuilder[] s, int num)
        {
            if (g_isFilled == false)
            {
                g_isFilled = true; 
                for (int i = GenNumber.g_index; i < num + GenNumber.g_index; i++)
                {
                    GenNumber.g_s[i] = s[i - GenNumber.g_index];
                }
                GenNumber.g_index += num;
            }
        }

        public static void PreNeed(ref InInfo inInfo, ref OutInfo outInfo)
        {
            inInfo.totalVolume = 0;
            inInfo.totalWidth = 0;

            //计算案卷的总卷数和总宽度
            string[] sTemp1 = inInfo.totalMix.Split(',');
            foreach (string s in sTemp1)
            {
                string[] sTemp2 = s.Split('*');
                inInfo.totalVolume += Int32.Parse(sTemp2[1]);
                inInfo.totalWidth += Int32.Parse(sTemp2[0]) * Int32.Parse(sTemp2[1]);
            }
            //根据总卷数定义返回信息字符串的维度
            outInfo.s = new StringBuilder[inInfo.totalVolume];

            //安排次序---给每个字符串预先加入信息 , 如 "二厘米-----"
            ArrangeOrder(inInfo, ref outInfo);
  
        }
        public static void GenShelfNumber(InInfo inInfo, ref OutInfo outInfo)
        {
            //总卷数在第一次点击“生成排架号按纽”后确定，以后不再修改。
            if (g_isFirst == false)
            {
                g_isFirst = true;
                g_totalVolume = inInfo.totalVolume;
            }
            g_isFilled = false; //add 20090921  
            if (inInfo.remainWidth >= inInfo.totalWidth)
                GenWithAdequateSpace(inInfo, ref outInfo);
            else    //当前层剩余宽度不够装完所有案卷
                GenWithInadquateSpace(inInfo, ref outInfo);   

            int num;
            if (outInfo.errorMsg != null)
            {
                num = outInfo.s.Length - GenNumber.g_midInfo.totalVolume;
                CombineString(outInfo.s, num);
            }
            else
            {
                num = outInfo.s.Length;
                CombineString(outInfo.s, num);
            }
        }

        public static ReturnInfo GetGenNumInfo()
        {
            ReturnInfo reInfo = new ReturnInfo();
            reInfo.s = new StringBuilder[g_index];
            for (int i = 0; i < g_index; i++ )
            {
                reInfo.s[i] = g_s[i];
                g_s[i] = null;
            }
            //静态全局参数初始化
            g_index = 0;
            g_isFirst = false;
            g_isChangeCab = false;
            g_midInfo.totalVolume = 0;

            reInfo.currentDraNum = g_drawerNum;
            reInfo.remainWidth = g_remainWidth;
            return reInfo;
        }
    }
}
