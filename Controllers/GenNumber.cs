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
            //���������������û�����
            public int drawerWidth;     //���� 
            public string totalMix;     //�����ַ���
            public int maxDrawerNum;    //������

            //���������������ѯ���ݿ����
            public int remainWidth;     //ʣ����
            public string curPaiJiaHao; //��ǰ�żܺ�

            //����������ͨ�������û������ַ�������
            public int totalVolume;     //�ܾ���
            public int totalWidth;      //�ܺ��
        }
        public struct OutInfo
        {
            public StringBuilder[] s;   //�����м�����ַ���
            public string errorMsg;     //������ʾ��������
            public int changeDraIndex;  //�������ڷ����ַ������� outInfo.s �Ӵ��±꿪ʼ��������
        }
        public struct ReturnInfo
        {
            public StringBuilder[] s;  //���غ����żܺ���Ϣ���ַ���
            public int currentDraNum;  //���ص�ǰ���   
            public int remainWidth;    //���ص�ǰ��ʣ����
        }

#region ��̬ȫ�ֱ���
        public static StringBuilder[] g_s = new StringBuilder[1000];
        public static int g_index = 0;
        public static InInfo g_midInfo = new InInfo();
        public static int g_drawerNum;
        public static int g_remainWidth;
        public static bool g_isFirst;
        public static bool g_isChangeCab; //add 20090831 ��ʶ�Ƿ񻻻�����
        public static int g_totalVolume;
        public static bool g_isFilled;   //add 20090921 ��ʶ�Ƿ��Ѿ�������м��ַ���
#endregion
        

        private static void GenWithAdequateSpace(InInfo inInfo, ref OutInfo outInfo)
        {
            for (int i = 0; i < inInfo.totalVolume; i++)
            {
                //��ȡ��ǰ�żܺŵĵ��ߡ���λ��ֵ����λ����ʮλ��
                int curSingle = Convert.ToInt32(inInfo.curPaiJiaHao.Substring(7, 1));
                int curTen = Convert.ToInt32(inInfo.curPaiJiaHao.Substring(6, 1));

                //�����ºŵĸ�λ��ʮλ��ֵ
                int newSingle = (i + 1) % 10 + curSingle;
                int newTen = (i + 1) / 10 + curTen;
                newTen += newSingle / 10;
                newSingle = newSingle % 10;

                //�򷵻��ַ����м�����ʼ�źŵ�ǰ��λ
               
                outInfo.s[i+outInfo.changeDraIndex].Append(inInfo.curPaiJiaHao.Substring(0,6));
                //�������λ��ڰ�λ���������İ�λ�żܺ�
                outInfo.s[i + outInfo.changeDraIndex].Append(Convert.ToString(newTen));
                outInfo.s[i + outInfo.changeDraIndex].Append(Convert.ToString(newSingle));
            }

            //���س��żܺ���Ķ�����Ϣ
            g_drawerNum = Convert.ToInt32(inInfo.curPaiJiaHao.Substring(5,1));
            g_remainWidth = inInfo.remainWidth - inInfo.totalWidth;
        }
        private static void GenWithInadquateSpace(InInfo inInfo, ref OutInfo outInfo)
        {
            //��������ַ�����˳��Ѱ�����װ������array��
            int[] array = new int[inInfo.totalVolume];
            ArrangeArray(inInfo, ref outInfo, array );

            //���ҵ�ǰ��ʣ������װ���ڼ��ݰ���
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
            //�����벻���Էſ����С�ģ�����һ������
            if (index == -1)
            {
                InInfo newInfo = new InInfo();
                newInfo = inInfo;
                //���¼�����ʼ�żܺ�
                newInfo.curPaiJiaHao = inInfo.curPaiJiaHao.Substring(0, 5) + (Convert.ToInt32(inInfo.curPaiJiaHao.Substring(5, 1)) + 1).ToString() + "00";
                if (Convert.ToInt32(newInfo.curPaiJiaHao.Substring(5, 1)) > inInfo.maxDrawerNum) //modify 20090831
                {
                    outInfo.errorMsg = "�˹��������뻻��һ��";
                    return;
                }
                else
                {
                    newInfo.remainWidth = inInfo.drawerWidth;
                    GenShelfNumber(newInfo, ref outInfo);
                }
            }
            //�����������ܷ�һ�������С��
            else
            {
                for (int i = 0; i <= index; i++)
                {
                    //��ȡ��ǰ�żܺŵĵ��ߡ���λ��ֵ����λ����ʮλ��
                    int curSingle = Convert.ToInt32(inInfo.curPaiJiaHao.Substring(7, 1));
                    int curTen = Convert.ToInt32(inInfo.curPaiJiaHao.Substring(6, 1));

                    //�����ºŵĸ�λ��ʮλ��ֵ
                    int newSingle = (i + 1) % 10 + curSingle;
                    int newTen = (i + 1) / 10 + curTen;
                    newTen += newSingle / 10;
                    newSingle = newSingle % 10;

                    //�򷵻��ַ����м�����ʼ�źŵ�ǰ��λ
                    outInfo.s[i+outInfo.changeDraIndex].Append(inInfo.curPaiJiaHao.Substring(0, 6));
                    //�������λ��ڰ�λ���������İ�λ�żܺ�
                    outInfo.s[i+outInfo.changeDraIndex].Append(Convert.ToString(newTen));
                    outInfo.s[i+outInfo.changeDraIndex].Append(Convert.ToString(newSingle));
                }
                InInfo newInfo = new InInfo();
                CaculateNewInfo(inInfo, ref newInfo, array, index, sum);
                if (Convert.ToInt32(newInfo.curPaiJiaHao.Substring(5,1)) > inInfo.maxDrawerNum) //modify 20090831
                {
                    outInfo.errorMsg = "�˹��������뻻��һ��";
                    g_midInfo.totalVolume = inInfo.totalVolume - (index + 1); //modify 20090831 --�����Ӻ���м���Ϣ
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
            //�����µ���Ϣ�ַ���(�м�����)
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

            //���ַ���totalMix �Ľ��
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

            //���㰸����ܾ������ܿ��
            string[] sTemp1 = inInfo.totalMix.Split(',');
            foreach (string s in sTemp1)
            {
                string[] sTemp2 = s.Split('*');
                inInfo.totalVolume += Int32.Parse(sTemp2[1]);
                inInfo.totalWidth += Int32.Parse(sTemp2[0]) * Int32.Parse(sTemp2[1]);
            }
            //�����ܾ������巵����Ϣ�ַ�����ά��
            outInfo.s = new StringBuilder[inInfo.totalVolume];

            //���Ŵ���---��ÿ���ַ���Ԥ�ȼ�����Ϣ , �� "������-----"
            ArrangeOrder(inInfo, ref outInfo);
  
        }
        public static void GenShelfNumber(InInfo inInfo, ref OutInfo outInfo)
        {
            //�ܾ����ڵ�һ�ε���������żܺŰ�Ŧ����ȷ�����Ժ����޸ġ�
            if (g_isFirst == false)
            {
                g_isFirst = true;
                g_totalVolume = inInfo.totalVolume;
            }
            g_isFilled = false; //add 20090921  
            if (inInfo.remainWidth >= inInfo.totalWidth)
                GenWithAdequateSpace(inInfo, ref outInfo);
            else    //��ǰ��ʣ���Ȳ���װ�����а���
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
            //��̬ȫ�ֲ�����ʼ��
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
