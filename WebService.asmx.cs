using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;

using System.IO;


using System.Data;
using System.Text;
using System.Web.Script.Services;


namespace urban_archive
{
    /// <summary>
    /// WebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
     [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        private bool isImage(string extension)
        {
            string low = extension.ToLower();
            if (low == ".tiff" || low == ".tif" || low == ".jpg" || low == ".bmp")
            {
                return true;
            }
            return false;
        }
        //[WebMethod]
        //public DataTable GetOtherArchivesList(string ID)
        //{
        //    DataTable myTable = null;
        //    string strPath = string.Empty;
        //    string strWebPath = string.Empty;
        //    //HttpContext.Current.Session["paperProjectSeqNo"] = "19";
        //    if (ID != null && ID != string.Empty)
        //    {
        //        if (ID.Contains("东"))
        //        {
        //            myTable = new DataTable();
        //            string[] strPaths = ID.Split('/');
        //            if (strPaths.Length == 4)
        //            {
        //                strPath = strPaths[0] + "\\" + strPaths[1].Substring(0, 4) + "\\" + strPaths[3];
        //                strWebPath = strPaths[0] + "/" + strPaths[1].Substring(0, 4) + "/" + strPaths[3] + "/";
        //            }
        //            else
        //            {
        //                strPath = strPaths[0] + "\\" + strPaths[1].Substring(0, 4);
        //                strWebPath = strPaths[0] + "/" + strPaths[1].Substring(0, 4) + "/";
        //            }
        //            strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["OtherArchivesPath"] + strPath + "\\";
        //            DirectoryInfo myDirInfo;
        //            Array arrFileInfo;

        //            DataRow myDataRow;

        //            myTable.Columns.Add("Name", Type.GetType("System.String"));
        //            myTable.Columns.Add("WebPath", Type.GetType("System.String"));

        //            if (Directory.Exists(strPath))
        //            {
        //                //取得目录信息
        //                myDirInfo = new DirectoryInfo(strPath);

        //                //获得文件信息
        //                arrFileInfo = myDirInfo.GetFiles();
        //                //排序
        //                Array.Sort(arrFileInfo, new FileSorter());
        //                if (arrFileInfo.Length > 0)
        //                {
        //                    foreach (FileInfo myFile in arrFileInfo)
        //                    {
        //                        if (isImage(myFile.Extension.Trim()) == true)
        //                        {
        //                            myDataRow = myTable.NewRow();
        //                            myDataRow["Name"] = myFile.Name;
        //                            myDataRow["WebPath"] = System.Web.Configuration.WebConfigurationManager.AppSettings["OtherArchivesWeb"] + Server.UrlPathEncode(strWebPath + myFile.Name);

        //                            myTable.Rows.Add(myDataRow);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            myTable = new DataTable();
        //            //strPath = HttpContext.Current.Session["ID"].ToString();
        //            string[] strPaths = ID.Split('/');
        //            for (int i = 0; i < strPaths.Length - 1; i++)
        //            {
        //                strPath += strPaths[i] + "\\";
        //            }
        //            strPath += strPaths[strPaths.Length - 1];
        //            //D:\OtherArchives\Licence\1983\1983-00457
        //            strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["OtherArchivesPath"] + strPath + "\\";
        //            strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["OtherArchivesWeb"];

        //            DirectoryInfo myDirInfo;
        //            Array arrFileInfo;

        //            DataRow myDataRow;

        //            myTable.Columns.Add("Name", Type.GetType("System.String"));
        //            myTable.Columns.Add("WebPath", Type.GetType("System.String"));

        //            if (Directory.Exists(strPath))
        //            {
        //                //取得目录信息
        //                myDirInfo = new DirectoryInfo(strPath);

        //                //获得文件信息
        //                arrFileInfo = myDirInfo.GetFiles();
        //                //排序
        //                Array.Sort(arrFileInfo, new FileSorter());
        //                if (arrFileInfo.Length > 0)
        //                {
        //                    foreach (FileInfo myFile in arrFileInfo)
        //                    {
        //                        if (isImage(myFile.Extension.Trim()) == true)
        //                        {
        //                            myDataRow = myTable.NewRow();
        //                            myDataRow["Name"] = myFile.Name;
        //                            myDataRow["WebPath"] = strWebPath + Server.UrlPathEncode(ID + "/" + myFile.Name);

        //                            myTable.Rows.Add(myDataRow);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return myTable;
        //}

        [WebMethod]
        public DataTable GetTuzhiArchivesList(string strPath)
        {
            DataTable myTable = null;

            string strWebPath = string.Empty;
            //HttpContext.Current.Session["contractNo"] = "19";
            if (strPath != null && strPath != string.Empty)
            {

                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["TuzhiArchivesWeb"] + strPath + "/";
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["TuzhiArchivesPath"] + strPath + "\\";
                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));


                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();
                    //排序
                    Array.Sort(arrFileInfo, new FileSorter());
                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath + myFile.Name;


                            myTable.Rows.Add(myDataRow);
                        }
                    }

                }
            }

            return myTable;
        }
        //added by lsx 备忘录扫描件
        //[WebMethod]
        //public DataTable GetreminderList(string strPath)
        //{
        //    DataTable myTable = null;
        //    string strWebPath = string.Empty;
        //    //HttpContext.Current.Session["paperProjectSeqNo"] = "19";
        //    if (strPath != null && strPath != string.Empty)
        //    {
        //        strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["reminderWeb"] + strPath + "/";
        //        strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["reminderPath"] + strPath + "\\";

        //        DirectoryInfo myDirInfo;
        //        Array arrFileInfo;
        //        myTable = new DataTable();

        //        DataRow myDataRow;

        //        myTable.Columns.Add("Name", Type.GetType("System.String"));
        //        myTable.Columns.Add("WebPath", Type.GetType("System.String"));

        //        if (Directory.Exists(strPath))
        //        {
        //            //取得目录信息
        //            myDirInfo = new DirectoryInfo(strPath);

        //            //获得文件信息
        //            arrFileInfo = myDirInfo.GetFiles();
        //            //排序
        //            Array.Sort(arrFileInfo, new FileSorter());
        //            if (arrFileInfo.Length > 0)
        //            {
        //                foreach (FileInfo myFile in arrFileInfo)
        //                {
        //                    if (isImage(myFile.Extension.Trim()) == true)
        //                    {
        //                        myDataRow = myTable.NewRow();
        //                        myDataRow["Name"] = myFile.Name;
        //                        myDataRow["WebPath"] = strWebPath + myFile.Name;

        //                        myTable.Rows.Add(myDataRow);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return myTable;
        //}
        //[WebMethod]
        //public DataTable GetContractList(string strPath)
        //{
        //    DataTable myTable = null;
        //    string strWebPath = string.Empty;
        //    //HttpContext.Current.Session["contractNo"] = "19";
        //    if (strPath != null && strPath != string.Empty)
        //    {
        //        strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ContractImageWeb"] + strPath + "/";
        //        strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ContactImagesPath"] + strPath + "\\";

        //        DirectoryInfo myDirInfo;
        //        Array arrFileInfo;
        //        myTable = new DataTable();

        //        DataRow myDataRow;

        //        myTable.Columns.Add("Name", Type.GetType("System.String"));

        //        myTable.Columns.Add("WebPath", Type.GetType("System.String"));


        //        if (Directory.Exists(strPath))
        //        {
        //            //取得目录信息
        //            myDirInfo = new DirectoryInfo(strPath);

        //            //获得文件信息
        //            arrFileInfo = myDirInfo.GetFiles();
        //            //排序
        //            Array.Sort(arrFileInfo, new FileSorter());
        //            if (arrFileInfo.Length > 0)
        //            {
        //                foreach (FileInfo myFile in arrFileInfo)
        //                {
        //                    if (isImage(myFile.Extension.Trim()) == true)
        //                    {
        //                        myDataRow = myTable.NewRow();
        //                        myDataRow["Name"] = myFile.Name;
        //                        myDataRow["WebPath"] = strWebPath + myFile.Name;

        //                        myTable.Rows.Add(myDataRow);
        //                    }

        //                }
        //            }
        //        }
        //    }
        //    return myTable;
        //}
        //[WebMethod]
        //public DataTable GetUserAndImageList(string userSeqNo)
        //{
        //    string virtualPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ServerVirtualPath"];
        //    IBindUserAndImage idal = DataFactory.CreateBindUserAndImage();
        //    string strSql = " userID='" + userSeqNo + "'";
        //    DataSet ds = idal.GetList(strSql);
        //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //    {
        //        string str = ds.Tables[0].Rows[i]["ImageAddress"].ToString();
        //        ds.Tables[0].Rows[i]["ImageAddress"] = virtualPath + str;
        //    }
        //    return ds.Tables[0];
        //}

        //[WebMethod]
        //public DataTable GetUserAndImageListByUserId(string userSeqNo)
        //{
        //    string virtualPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ServerVirtualPath"];
        //    IBindUserAndImage idal = DataFactory.CreateBindUserAndImage();
        //    string strSql = " realuserID='" + userSeqNo + "'";
        //    DataSet ds = idal.GetList(strSql);
        //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //    {
        //        string str = ds.Tables[0].Rows[i]["ImageAddress"].ToString();
        //        ds.Tables[0].Rows[i]["ImageAddress"] = virtualPath + str;
        //    }
        //    return ds.Tables[0];
        //}


        [WebMethod]
        public DataTable GetImageArchiveList(string strPath)
        {
            DataTable myTable = null;
            string strWebPath = string.Empty;
            //HttpContext.Current.Session["contractNo"] = "19";
            if (strPath != null && strPath != string.Empty)
            {
                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ArchivesWeb"] + strPath + "/";
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ArchivesPath"] + strPath + "\\";

                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));
                myTable.Columns.Add("WebPath", Type.GetType("System.String"));

                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();
                    //排序
                    Array.Sort(arrFileInfo, new FileSorter());
                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();
                            myDataRow["Name"] = myFile.Name;
                            myDataRow["WebPath"] = strWebPath + myFile.Name;


                            myTable.Rows.Add(myDataRow);
                        }
                    }
                }
            }

            return myTable;
        }
        [WebMethod]
        public DataTable GetArchivesListSort(string strPath)
        {
            DataTable myTable = null;
            string strWebPath = string.Empty;
            //HttpContext.Current.Session["contractNo"] = "19";
            if (strPath != null && strPath != string.Empty)
            {
                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ArchivesWeb"] + strPath + "/";
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ArchivesPath"] + strPath + "\\";
                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));
                myTable.Columns.Add("WebPath", Type.GetType("System.String"));

                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();
                    //排序
                    Array.Sort(arrFileInfo, new FileSorter());
                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;
                            myDataRow["WebPath"] = strWebPath + myFile.Name;

                            myTable.Rows.Add(myDataRow);
                        }
                    }
                }
            }

            return myTable;
        }

        [WebMethod]
        public DataTable GetGuanXianArchivesListSort(string strPath)
        {
            DataTable myTable = null;
            string strWebPath = string.Empty;
            //HttpContext.Current.Session["contractNo"] = "19";
            if (strPath != null && strPath != string.Empty)
            {
                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["gxArchivesWeb"] + strPath + "/";
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["gxArchivesPath"] + strPath + "\\";
                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));
                myTable.Columns.Add("WebPath", Type.GetType("System.String"));

                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();
                    //排序
                    Array.Sort(arrFileInfo, new FileSorter());
                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;
                            myDataRow["WebPath"] = strWebPath + myFile.Name;

                            myTable.Rows.Add(myDataRow);
                        }
                    }
                }
            }

            return myTable;
        }


        [WebMethod]
        public DataTable GetJunGongPicList(string paperSeqNo, string archiveNo)
        {
            DataTable myTable = null;
            string strPath = string.Empty;
            string strWebPath = string.Empty;
            //string[] strJunGongSession = (string[])HttpContext.Current.Session["JunGongSession"];
            if (paperSeqNo != null && (paperSeqNo != null) && (archiveNo != string.Empty) && (archiveNo != string.Empty))
            {

                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + paperSeqNo + "\\" + archiveNo + "\\";
                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongWebPath"] + paperSeqNo + "/" + archiveNo + "/";

                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();
                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));


                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();

                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath + myFile.Name;


                            myTable.Rows.Add(myDataRow);
                        }
                    }


                }
            }
            return myTable;
        }

        //[WebMethod]
        //public DataTable GetJunGongPicList(string paperSeqNo, string archiveNo, string volNo)
        //{
        //    DataTable myTable = null;
        //    string strPath = string.Empty;
        //    string strWebPath = string.Empty;
        //    //string[] strJunGongSession = (string[])HttpContext.Current.Session["JunGongSession"];
        //    if (paperSeqNo != null && (paperSeqNo != "") && (archiveNo != null) && (archiveNo != string.Empty) && !string.IsNullOrEmpty(volNo))
        //    {

        //        strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + paperSeqNo + "\\" + archiveNo + "\\";
        //        string strPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + paperSeqNo + "\\" + volNo + "\\";
        //        strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongWebPath"] + paperSeqNo + "/" + archiveNo + "/";
        //        string strWebPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongWebPath"] + paperSeqNo + "/" + volNo + "/";

        //        DirectoryInfo myDirInfo;
        //        Array arrFileInfo;
        //        myTable = new DataTable();
        //        DataRow myDataRow;

        //        myTable.Columns.Add("Name", Type.GetType("System.String"));

        //        myTable.Columns.Add("WebPath", Type.GetType("System.String"));


        //        if (Directory.Exists(strPath))
        //        {
        //            //取得目录信息
        //            myDirInfo = new DirectoryInfo(strPath);

        //            //获得文件信息
        //            arrFileInfo = myDirInfo.GetFiles();

        //            if (arrFileInfo.Length > 0)
        //            {
        //                foreach (FileInfo myFile in arrFileInfo)
        //                {
        //                    myDataRow = myTable.NewRow();

        //                    myDataRow["Name"] = myFile.Name;

        //                    myDataRow["WebPath"] = strWebPath + myFile.Name;

        //                    myTable.Rows.Add(myDataRow);
        //                }
        //            }
        //        }
        //        else if (Directory.Exists(strPath2))
        //        {
        //            //取得目录信息
        //            myDirInfo = new DirectoryInfo(strPath2);

        //            //获得文件信息
        //            arrFileInfo = myDirInfo.GetFiles();

        //            if (arrFileInfo.Length > 0)
        //            {
        //                foreach (FileInfo myFile in arrFileInfo)
        //                {
        //                    myDataRow = myTable.NewRow();

        //                    myDataRow["Name"] = myFile.Name;

        //                    myDataRow["WebPath"] = strWebPath2 + myFile.Name;

        //                    myTable.Rows.Add(myDataRow);
        //                }
        //            }
        //        }
        //    }
        //    return myTable;
        //}
         [WebMethod]
        public DataTable GetJunGongPicListDemo()
        {
            DataTable myTable = null;
            string strPath = string.Empty;
            string strWebPath = string.Empty;

            //strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + paperSeqNo + "\\" + archiveNo + "\\";
            //string strPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + paperSeqNo + "\\" + volNo + "\\";
            //strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongWebPath"] + paperSeqNo + "/" + archiveNo + "/";
            //string strWebPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongWebPath"] + paperSeqNo + "/" + volNo + "/";

            strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + "00001" + "\\" + "F5.1-0002" + "\\";
            strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongWebPath"] + "00001" + "/" + "F5.1-0002" + "/";
            //D:\JunGongArchives\00001\F5.1-0002

            DirectoryInfo myDirInfo;
            Array arrFileInfo;
            myTable = new DataTable();
            DataRow myDataRow;

            myTable.Columns.Add("Name", Type.GetType("System.String"));

            myTable.Columns.Add("WebPath", Type.GetType("System.String"));


            if (Directory.Exists(strPath))
            {
                //取得目录信息
                myDirInfo = new DirectoryInfo(strPath);

                //获得文件信息
                arrFileInfo = myDirInfo.GetFiles();

                if (arrFileInfo.Length > 0)
                {
                    foreach (FileInfo myFile in arrFileInfo)
                    {
                        myDataRow = myTable.NewRow();

                        myDataRow["Name"] = myFile.Name;

                        myDataRow["WebPath"] = strWebPath + myFile.Name;

                        myTable.Rows.Add(myDataRow);
                    }
                }
            }
            //else if (Directory.Exists(strPath2))
            //{
            //    //取得目录信息
            //    myDirInfo = new DirectoryInfo(strPath2);

            //    //获得文件信息
            //    arrFileInfo = myDirInfo.GetFiles();

            //    if (arrFileInfo.Length > 0)
            //    {
            //        foreach (FileInfo myFile in arrFileInfo)
            //        {
            //            myDataRow = myTable.NewRow();

            //            myDataRow["Name"] = myFile.Name;

            //            myDataRow["WebPath"] = strWebPath2 + myFile.Name;

            //            myTable.Rows.Add(myDataRow);
            //        }
            //    }
            //}
            return myTable;
        }

        [WebMethod]
        public DataTable GetGuanXianPicList(string paperSeqNo, string archiveNo, string volNo)
        {
            DataTable myTable = null;
            string strPath = string.Empty;
            string strWebPath = string.Empty;
            //string[] strJunGongSession = (string[])HttpContext.Current.Session["JunGongSession"];
            if (paperSeqNo != null && (paperSeqNo != "") && (archiveNo != null) && (archiveNo != string.Empty) && !string.IsNullOrEmpty(volNo))
            {

                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["GuanXianPath"] + paperSeqNo + "\\" + archiveNo + "\\";
                string strPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["GuanXianPath"] + paperSeqNo + "\\" + volNo + "\\";
                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["GuanXianWebPath"] + paperSeqNo + "/" + archiveNo + "/";
                string strWebPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["GuanXianWebPath"] + paperSeqNo + "/" + volNo + "/";

                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();
                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));


                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();

                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath + myFile.Name;

                            myTable.Rows.Add(myDataRow);
                        }
                    }
                }
                else if (Directory.Exists(strPath2))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath2);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();

                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath2 + myFile.Name;

                            myTable.Rows.Add(myDataRow);
                        }
                    }
                }
            }
            return myTable;
        }


        [WebMethod]
        public DataTable GetYCPicList(string paperSeqNo, string archiveNo)
        {
            DataTable myTable = null;
            string strPath = string.Empty;
            string strWebPath = string.Empty;
            //string[] strJunGongSession = (string[])HttpContext.Current.Session["JunGongSession"];
            if (paperSeqNo != null && (paperSeqNo != null) && (archiveNo != string.Empty) && (archiveNo != string.Empty))
            {

                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["YuanChuanArchivesPath"] + paperSeqNo + "\\" + archiveNo + "\\";
                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["YuanChuanArchivesWeb"] + paperSeqNo + "/" + archiveNo + "/";

                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();
                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));


                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();

                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath + myFile.Name;


                            myTable.Rows.Add(myDataRow);
                        }
                    }


                }
            }
            return myTable;
        }


        //added by lsx at 20100323 征地档案案扫描件上传
        [WebMethod]
        public DataTable GetZDPicsList(string strPath)
        {
            DataTable myTable = null;

            string strWebPath = string.Empty;
            //HttpContext.Current.Session["paperProjectSeqNo"] = "19";
            if (strPath != null && strPath != string.Empty)
            {
                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ZDPicsProjWebPath"] + strPath + "/";
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ZDPicsProjPath"] + strPath + "\\";
                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));

                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();
                    //排序
                    Array.Sort(arrFileInfo, new FileSorter());
                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath + myFile.Name;

                            myTable.Rows.Add(myDataRow);
                        }
                    }
                }
            }

            return myTable;
        }
        [WebMethod]
        //add by niutianbo,date:20090730
        public DataTable GetPlanArchiveBatchList(string strPath)
        {
            DataTable myTable = null;

            string strWebPath = string.Empty;
            //HttpContext.Current.Session["contractNo"] = "19";
            if (strPath != null && strPath != string.Empty)
            {


                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["PlanArchiveBatchPath"] + strPath + "/";
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["PlanArchiveBatchPathLocal"] + strPath + "\\";
                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));


                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();
                    //排序
                    Array.Sort(arrFileInfo, new FileSorter());
                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath + myFile.Name;


                            myTable.Rows.Add(myDataRow);
                        }
                    }


                }
            }

            return myTable;
        }
        [WebMethod]
        //added by lsx 20091204
        public DataTable GetContractSheetList(string strPath)
        {
            DataTable myTable = null;

            string strWebPath = string.Empty;
            //HttpContext.Current.Session["contractNo"] = "19";
            if (strPath != null && strPath != string.Empty)
            {


                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ContractSheetImageWeb"] + strPath + "/";
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ContractSheetImagesPath"] + strPath + "\\";
                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));


                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();
                    //排序
                    Array.Sort(arrFileInfo, new FileSorter());
                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath + myFile.Name;


                            myTable.Rows.Add(myDataRow);
                        }
                    }

                }
            }

            return myTable;
        }
        [WebMethod]
        //add by lsx,20090924
        public DataTable GetCommissionContractList(string strPath)
        {
            DataTable myTable = null;

            string strWebPath = string.Empty;
            //HttpContext.Current.Session["contractNo"] = "19";
            if (strPath != null && strPath != string.Empty)
            {

                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["CommissionContractImageWeb"] + strPath + "/";
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["CommissionContactImagesPath"] + strPath + "\\";
                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));


                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();
                    //排序
                    Array.Sort(arrFileInfo, new FileSorter());
                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath + myFile.Name;

                            myTable.Rows.Add(myDataRow);
                        }
                    }
                }
            }

            return myTable;
        }
        [WebMethod]
        public DataTable GetVideoImageArchiveList(string strPath)
        {
            DataTable myTable = null;

            string strWebPath = string.Empty;
            //HttpContext.Current.Session["contractNo"] = "19";
            if (strPath != null && strPath != string.Empty)
            {

                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["VideoPhotoAddress"] + strPath + "/";
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["VideoPhotoPath"] + strPath + "\\";
                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));


                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();

                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath + myFile.Name;


                            myTable.Rows.Add(myDataRow);
                        }
                    }

                }
            }

            return myTable;
        }
        //add by Cathy,at 8,26,2009;for 声像档案文件照片
        [WebMethod]
        public DataTable GetVideoFileImageList(string strPath)
        {
            DataTable myTable = null;

            string strWebPath = string.Empty;
            string[] strvideoSession = (string[])HttpContext.Current.Session["videoSession"];
            if (strvideoSession[0] != null && (strvideoSession[1] != null) && (strvideoSession[0] != string.Empty) && (strvideoSession[1] != string.Empty) && (strvideoSession[2] != null) && (strvideoSession[2] != string.Empty))
            {
                //strPath = strvideoSession[0];
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["VideoPhotoPath"] + strvideoSession[0] + "\\" + strvideoSession[1] + "\\" + strvideoSession[2] + "\\";
                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["VideoPhotoAddress"] + strvideoSession[0] + "/" + strvideoSession[1] + "/" + strvideoSession[2] + "/";

                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();
                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));


                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();

                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath + myFile.Name;


                            myTable.Rows.Add(myDataRow);
                        }
                    }


                }
            }
            return myTable;
        }

        //add by Cathy,at 8,26,2009;for 声像档案文件照片
        //[WebMethod]
        //public DataTable GetVideoFileImageList(string arch, string fileid)
        //{
        //    DataTable myTable = null;

        //    IPhotoCassette myphotoCassetteIDAL = DataFactory.CreatePhotoCassette();
        //    PhotoCassetteModel myphotocassettemodel = myphotoCassetteIDAL.GetModel(arch);
        //    string projectID = myphotocassettemodel.videoProjectSeqNo.ToString();

        //    string strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["VideoPhotoPath"] + projectID + "\\" + arch + "\\" + fileid + "\\";
        //    string strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["VideoPhotoAddress"] + projectID + "/" + arch + "/" + fileid + "/";

        //    //string strWebPath = string.Empty;
        //    //string[] strvideoSession = (string[])HttpContext.Current.Session["videoSession"];
        //    if (projectID != "" && arch != "" && fileid != "")
        //    {
        //        //    //strPath = strvideoSession[0];
        //        //    strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["VideoPhotoPath"] + strvideoSession[0] + "\\" + strvideoSession[1] + "\\" + strvideoSession[2] + "\\";
        //        //    strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["VideoPhotoAddress"] + strvideoSession[0] + "/" + strvideoSession[1] + "/" + strvideoSession[2] + "/";

        //        DirectoryInfo myDirInfo;
        //        Array arrFileInfo;
        //        myTable = new DataTable();
        //        DataRow myDataRow;

        //        myTable.Columns.Add("Name", Type.GetType("System.String"));

        //        myTable.Columns.Add("WebPath", Type.GetType("System.String"));


        //        if (Directory.Exists(strPath))
        //        {
        //            //取得目录信息
        //            myDirInfo = new DirectoryInfo(strPath);

        //            //获得文件信息
        //            arrFileInfo = myDirInfo.GetFiles();

        //            if (arrFileInfo.Length > 0)
        //            {
        //                foreach (FileInfo myFile in arrFileInfo)
        //                {
        //                    myDataRow = myTable.NewRow();

        //                    myDataRow["Name"] = myFile.Name;

        //                    myDataRow["WebPath"] = strWebPath + myFile.Name;


        //                    myTable.Rows.Add(myDataRow);
        //                }
        //            }


        //        }
        //    }
        //    return myTable;
        //}


        //add by Cathy at 7.29,2009
        [WebMethod]
        public DataTable GetPlanProjList(string strPath)
        {
            DataTable myTable = null;

            string strWebPath = string.Empty;
            //HttpContext.Current.Session["contractNo"] = "19";
            if (strPath != null && strPath != string.Empty)
            {
                string webp = strPath;
                string strp = strPath;
                if (strPath.IndexOf('-') > 0)
                {
                    webp = strPath.Replace('-', '/');// strPath.Split('-')[0] + "/" + strPath.Split('-')[1];
                    strp = strPath.Replace('-', '\\');
                }
                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["PlanPicsProjAddress"] + webp + "/";
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["PlanPicsProjPath"] + strp + "\\";
                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));


                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();
                    //排序
                    Array.Sort(arrFileInfo, new FileSorter());
                    if (arrFileInfo.Length > 0)
                    {
                        foreach (FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath + myFile.Name;


                            myTable.Rows.Add(myDataRow);
                        }
                    }

                }
            }

            return myTable;
        }
        public class FileSorter : IComparer
        {
            #region IComparer Members
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                if (x == null)
                {
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }
                FileInfo xInfo = (FileInfo)x;
                FileInfo yInfo = (FileInfo)y;


                //依名稱排序  
                return xInfo.FullName.CompareTo(yInfo.FullName);//遞增  
                                                                //return yInfo.FullName.CompareTo(xInfo.FullName);//遞減  

                //依修改日期排序  
                //return xInfo.LastWriteTime.CompareTo(yInfo.LastWriteTime);//遞增  
                //return yInfo.LastWriteTime.CompareTo(xInfo.LastWriteTime);//遞減  
            }
            #endregion
        }

    }
}
