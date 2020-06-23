using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using urban_archive.Models;
using Microsoft.Reporting.WebForms;
using System.Globalization;
using Newtonsoft.Json;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Text;
using System.Data.SqlClient;
using System.Data.OleDb;
using CrystalDecisions.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.IO;
using FileInfo = urban_archive.Models.FileInfo;
using QRCoder;
using System.Drawing;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using System.Text.RegularExpressions;

namespace urban_archive.Controllers
{
    public class ArchivesEnterController : Controller
    {
        // GET: ArchivesEnter
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        private OfficeEntities cb = new OfficeEntities();
        private ReportDocument rptH = new ReportDocument();
        public ActionResult window()
        {
            return View();
        }
        public ActionResult jiaojiemulu(long? paperProjectSeqNo, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db.vw_passList
                      where ad.paperProjectSeqNo == paperProjectSeqNo
                      orderby ad.registrationNo
                      select ad;
            var person = User.Identity.Name;
            localReport.ReportPath = Server.MapPath("~/Report/jungong/jiaojiemulu.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("jiaojiemulu", ds1);
            localReport.DataSources.Add(reportDataSource1);
            List<ReportParameter> parameterList = new List<ReportParameter>();
            parameterList.Add(new ReportParameter("person", person.ToString().Trim()));
            localReport.SetParameters(parameterList);
            string reportType = type;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
                "<DeviceInfo>" +
                "<OutPutFormat>" + type + "</OutPutFormat>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            renderedBytes = localReport.Render(
                   reportType,
                   deviceInfo,
                   out mimeType,
                   out encoding,
                   out fileNameExtension,
                   out streams,
                   out warnings
                   );
            return File(renderedBytes, mimeType);
        }
        public ActionResult yijiaoshu(ArchivesDetail archivesdetail, long? paperProjectSeqNo, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db.ArchivesDetail
                      where ad.paperProjectSeqNo == paperProjectSeqNo
                      select ad;
            var ds2 = from ad in db.vw_transferPaper
                      where ad.paperProjectSeqNo == paperProjectSeqNo
                      select ad;
            int totaljuanshu = ds1.Count();
            int n = totaljuanshu % 9;
            int page = totaljuanshu / 9;
            if (n != 0) {
                page = (totaljuanshu / 9) + 1;
            }
            localReport.ReportPath = Server.MapPath("~/Report/jungong/yijiaoshu.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("yijiaoshu2", ds1);
            ReportDataSource reportDataSource2 = new ReportDataSource("yijiaoshu1", ds2);
            localReport.DataSources.Add(reportDataSource1);
            localReport.DataSources.Add(reportDataSource2);
            List<ReportParameter> parameterList = new List<ReportParameter>();
            parameterList.Add(new ReportParameter("totaljuanshu", totaljuanshu.ToString()));
            parameterList.Add(new ReportParameter("page", page.ToString()));
            localReport.SetParameters(parameterList);
            string reportType = type;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
                "<DeviceInfo>" +
                "<OutPutFormat>" + type + "</OutPutFormat>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            renderedBytes = localReport.Render(
                   reportType,
                   deviceInfo,
                   out mimeType,
                   out encoding,
                   out fileNameExtension,
                   out streams,
                   out warnings
                   );
            return File(renderedBytes, mimeType);
        }
        public ActionResult LuRuTongJi(string action, string type = "PDF")
        {
            ViewData["pagename"] = "ArchivesEnter/LuRuTongJi";
            if (action == "打印竣工档案移交书统计表")
            {

                string sNo = Request.Form["sseqNo"];
                string eNo = Request.Form["eseqNo"];

                if (sNo == "" && eNo == "")
                {
                    DateTime DataFrom = DateTime.Parse(Request.Form["DateStart"]);
                    DateTime DataTo = DateTime.Parse(Request.Form["DateEnd"]);
                    LocalReport localReport = new LocalReport();
                    var ds = db.vw_transferStatis.Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo).OrderBy(ad => ad.paperProjectSeqNo);
                    List<vw_transferStatis> list = ds.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].projectName != null)
                            list[i].projectName = list[i].projectName.Replace("\n", "").Trim();
                        if (list[i].developmentOrganization != null)
                            list[i].developmentOrganization = list[i].developmentOrganization.Replace("\n", "").Trim();
                    }
                    localReport.ReportPath = Server.MapPath("~/Report/jungong/YiJiaoShuTongJi.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("YiJiaoShuTongJi", ds);
                    localReport.DataSources.Add(reportDataSource);
                    List<ReportParameter> parameterList = new List<ReportParameter>();
                    parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Trim()));
                    parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().Trim()));
                    localReport.SetParameters(parameterList);
                    string reportType = type;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;
                    string deviceInfo =
                        "<DeviceInfo>" +
                        "<OutPutFormat>" + type + "</OutPutFormat>" +
                        "</DeviceInfo>";
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;
                    renderedBytes = localReport.Render(
                           reportType,
                           deviceInfo,
                           out mimeType,
                           out encoding,
                           out fileNameExtension,
                           out streams,
                           out warnings
                           );
                    return File(renderedBytes, mimeType);
                }
                if (sNo != "" && eNo != "")
                {
                    long n = long.Parse(sNo);
                    long m = long.Parse(eNo);
                    LocalReport localReport = new LocalReport();
                    var ds = db.vw_transferStatis.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.paperProjectSeqNo <= m).OrderBy(ad => ad.paperProjectSeqNo);
                    List<vw_transferStatis> list = ds.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].projectName != null)
                            list[i].projectName = list[i].projectName.Replace("\n", "").Trim();
                        if (list[i].developmentOrganization != null)
                            list[i].developmentOrganization = list[i].developmentOrganization.Replace("\n", "").Trim();
                    }
                    localReport.ReportPath = Server.MapPath("~/Report/jungong/YiJiaoShuTongJi2.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("YiJiaoShuTongJi1", ds);
                    localReport.DataSources.Add(reportDataSource);
                    string reportType = type;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;
                    string deviceInfo =
                        "<DeviceInfo>" +
                        "<OutPutFormat>" + type + "</OutPutFormat>" +
                        "</DeviceInfo>";
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;
                    renderedBytes = localReport.Render(
                           reportType,
                           deviceInfo,
                           out mimeType,
                           out encoding,
                           out fileNameExtension,
                           out streams,
                           out warnings
                           );
                    return File(renderedBytes, mimeType);
                }
            }
            if (action == "打印竣工档案案卷目录")
            {
                LocalReport localReport = new LocalReport();
                string PNoS = Request.Form["seqNoStart"];
                string VNoS = Request.Form["VolNoStart"];
                string PNoE = Request.Form["seqNoEnd"];
                string VNoE = Request.Form["VolNoEnd"];
                long n = long.Parse(PNoS);
                long m = long.Parse(PNoE);

                var temp = cb.vw_archiveMainList.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.paperProjectSeqNo <= m).OrderBy(ad => ad.paperProjectSeqNo).ThenBy(ad => ad.volNo);
                if (VNoS != "" && VNoE != "")
                {
                    long a = long.Parse(VNoS);
                    long b = long.Parse(VNoE);
                    temp = cb.vw_archiveMainList.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.paperProjectSeqNo <= m).Where(ad => ad.volNo >= a).Where(ad => ad.volNo <= b).OrderBy(ad => ad.paperProjectSeqNo).ThenBy(ad => ad.volNo);
                }
                List<vw_archiveMainList> list = temp.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].licenseNo != null)
                        list[i].licenseNo = list[i].licenseNo.Trim().Replace("\n", "");
                    if (list[i].changeLog != null)
                        list[i].changeLog = list[i].changeLog.Trim().Replace("\n", "");
                    if (list[i].archivesTitle != null)
                        list[i].archivesTitle = list[i].archivesTitle.Trim().Replace("\n", "");
                    if (list[i].developmentOrganization != null)
                        list[i].developmentOrganization = list[i].developmentOrganization.Trim().Replace("\n", "");
                    if (list[i].constructionOrganization != null)
                        list[i].constructionOrganization = list[i].constructionOrganization.Trim().Replace("\n", "");
                    if (list[i].location != null)
                        list[i].location = list[i].location.Trim().Replace("\n", "");
                }
                var ds = list;
                localReport.ReportPath = Server.MapPath("~/Report/jungong/AnJuanMuLu.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("AnJuanMuLu", ds);
                localReport.DataSources.Add(reportDataSource);
                string reportType = type;
                string mimeType;
                string encoding;
                string fileNameExtension;
                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutPutFormat>" + type + "</OutPutFormat>" +
                    "</DeviceInfo>";
                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;
                renderedBytes = localReport.Render(
                       reportType,
                       deviceInfo,
                       out mimeType,
                       out encoding,
                       out fileNameExtension,
                       out streams,
                       out warnings
                       );
                return File(renderedBytes, mimeType);
            }
            if (action == "打印城建建设竣工档案工程目录")
            {
                LocalReport localReport = new LocalReport();
                long Sseq = long.Parse(Request.Form["startseqNo"]);
                long Eseq = long.Parse(Request.Form["endseqNo"]);
                var ds = db.vw_ArchiveList.Where(ad => ad.paperProjectSeqNo >= Sseq).Where(ad => ad.paperProjectSeqNo <= Eseq).OrderBy(ad => ad.paperProjectSeqNo);
                List<vw_ArchiveList> list = ds.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].licenseNo != null)
                        list[i].licenseNo = list[i].licenseNo.Trim().Replace("\n", "");
                    if (list[i].projectName != null)
                        list[i].projectName = list[i].projectName.Trim().Replace("\n", "");
                    if (list[i].archivesCount != null)
                        list[i].archivesCount = list[i].archivesCount.Trim().Replace("\n", "");
                    if (list[i].developmentOrganization != null)
                        list[i].developmentOrganization = list[i].developmentOrganization.Trim().Replace("\n", "");
                }
                var ds1 = list;
                localReport.ReportPath = Server.MapPath("~/Report/jungong/GongChengMuLu.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("GongChengMuLu", ds1);
                localReport.DataSources.Add(reportDataSource);
                string reportType = type;
                string mimeType;
                string encoding;
                string fileNameExtension;
                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutPutFormat>" + type + "</OutPutFormat>" +
                "</DeviceInfo>";
                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;
                renderedBytes = localReport.Render(
                       reportType,
                       deviceInfo,
                       out mimeType,
                       out encoding,
                       out fileNameExtension,
                       out streams,
                       out warnings
                       );
                return File(renderedBytes, mimeType);
            }
            return View();
        }
        public ActionResult juanneimulu(string myid, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds = from ad in db.vw_fileList
                     where ad.archivesNo1 == myid
                     orderby ad.seqNo
                     select ad;
            //var n = ds.Count();
            //int page = n / 15 + 1;
            //int t = page * 15;
            //for (int i = n; i < t; i++)
            //{
            //    var ds1 = ds.ToList().AddRange();
            //}
            var ds1 = ds.ToList();
            DataSet dataset = new DataSet();
            DataTable dt = new DataTable("jgjuanneimulu");
            DataColumn dc = new DataColumn("fileNo");
            dt.Columns.Add(dc);
            dc = new DataColumn("firstResponsible");
            dt.Columns.Add(dc);
            dc = new DataColumn("fileName");
            dt.Columns.Add(dc);
            dc = new DataColumn("startPageNo");
            dt.Columns.Add(dc);
            dc = new DataColumn("startDate");
            dt.Columns.Add(dc);
            dc = new DataColumn("endDate");
            dt.Columns.Add(dc);
            dc = new DataColumn("endPageNo");
            dt.Columns.Add(dc);
            dc = new DataColumn("remarks");
            dt.Columns.Add(dc);
            dc = new DataColumn("seqNo");
            dt.Columns.Add(dc);
            dc = new DataColumn("responsible");
            dt.Columns.Add(dc);
            dc = new DataColumn("id");
            dt.Columns.Add(dc);
            dc = new DataColumn("archivesNo1");
            dt.Columns.Add(dc);

            for (int i = 0; i < ds.Count(); i++)
            {
                DataRow dr = dt.NewRow();
                dr["fileNo"] = ds1[i].fileNo;
                dr["firstResponsible"] = ds1[i].firstResponsible;
                dr["fileName"] = ds1[i].fileName;
                dr["startPageNo"] = ds1[i].startPageNo;
                dr["startDate"] = ds1[i].startDate;
                dr["endDate"] = ds1[i].endDate;
                dr["endPageNo"] = ds1[i].endPageNo;
                dr["remarks"] = ds1[i].remarks;
                dr["seqNo"] = ds1[i].seqNo;
                dr["id"] = ds1[i].id;
                dr["responsible"] = ds1[i].responsible;
                dr["archivesNo1"] = ds1[i].archivesNo1;
                dt.Rows.Add(dr);
            }
            int yushu = ds.Count() % 15;
            if (yushu != 0)
            {
                int bushu = 15 - yushu;
                for (int i = 0; i < bushu; i++)
                {
                    var row = dt.NewRow(); dt.Rows.Add(row);
                }
            }
            localReport.ReportPath = Server.MapPath("~/Report/jungong/JuanNeiMuLu.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("jgjuanneimulu", dt);

            localReport.DataSources.Add(reportDataSource1);
            string reportType = type;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
                "<DeviceInfo>" +
                "<OutPutFormat>" + type + "</OutPutFormat>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            renderedBytes = localReport.Render(
                   reportType,
                   deviceInfo,
                   out mimeType,
                   out encoding,
                   out fileNameExtension,
                   out streams,
                   out warnings
                   );
            return File(renderedBytes, mimeType);
        }
        //public ActionResult Reporting1(string archivesNo, string type = "PDF")
        //{
        //    LocalReport localReport = new LocalReport();
        //    var ds1 = from ad in db.vw_archiveInfo
        //              where ad.archivesNo == archivesNo
        //              select ad;
        //    string title = ds1.First().archivesTitle;
        //    if (title.Count() <= 23)
        //    {
        //        string title1 = title;
        //        localReport.ReportPath = Server.MapPath("~/Report/jungong/anjuanfengpi.rdlc");
        //        ReportDataSource reportDataSource1 = new ReportDataSource("jgfengpiDataSet", ds1);
        //        localReport.DataSources.Add(reportDataSource1);
        //        List<ReportParameter> parameterList = new List<ReportParameter>();
        //        parameterList.Add(new ReportParameter("title1", title1.ToString().Trim()));
        //        localReport.SetParameters(parameterList);

        //    }
        //    if (title.Count() <= 46 && title.Count() > 23)
        //    {
        //        string title1 = title.Substring(0, 23);
        //        string title2 = title.Remove(0, 23);
        //        localReport.ReportPath = Server.MapPath("~/Report/jungong/anjuanfengpi.rdlc");
        //        ReportDataSource reportDataSource1 = new ReportDataSource("jgfengpiDataSet", ds1);
        //        localReport.DataSources.Add(reportDataSource1);
        //        List<ReportParameter> parameterList = new List<ReportParameter>();
        //        parameterList.Add(new ReportParameter("title1", title1.ToString().Trim()));
        //        parameterList.Add(new ReportParameter("title2", title2.ToString().Trim()));
        //        localReport.SetParameters(parameterList);

        //    }
        //    if (title.Count() <= 69 && title.Count() > 46)
        //    {
        //        string title1 = title.Substring(0, 23);
        //        string title2 = title.Remove(0, 23).Substring(0, 23);

        //        string title3 = title.Remove(0, 46);
        //        localReport.ReportPath = Server.MapPath("~/Report/jungong/anjuanfengpi.rdlc");
        //        ReportDataSource reportDataSource1 = new ReportDataSource("jgfengpiDataSet", ds1);
        //        localReport.DataSources.Add(reportDataSource1);
        //        List<ReportParameter> parameterList = new List<ReportParameter>();
        //        parameterList.Add(new ReportParameter("title1", title1.ToString().Trim()));
        //        parameterList.Add(new ReportParameter("title2", title2.ToString().Trim()));
        //        parameterList.Add(new ReportParameter("title3", title3.ToString().Trim()));
        //        localReport.SetParameters(parameterList);

        //    }
        //    if (title.Count() > 69 && title.Count() <= 92)
        //    {

        //        string title1 = title.Substring(0, 23);
        //        string title2 = title.Remove(0, 23).Substring(0, 23);
        //        string title3 = title.Remove(0, 46).Substring(0, 23);
        //        string title4 = title.Remove(0, 69);
        //        localReport.ReportPath = Server.MapPath("~/Report/jungong/anjuanfengpi.rdlc");
        //        ReportDataSource reportDataSource1 = new ReportDataSource("jgfengpiDataSet", ds1);
        //        localReport.DataSources.Add(reportDataSource1);
        //        List<ReportParameter> parameterList = new List<ReportParameter>();
        //        parameterList.Add(new ReportParameter("title1", title1.ToString().Trim()));
        //        parameterList.Add(new ReportParameter("title2", title2.ToString().Trim()));
        //        parameterList.Add(new ReportParameter("title3", title3.ToString().Trim()));
        //        parameterList.Add(new ReportParameter("title4", title4.ToString().Trim()));
        //        localReport.SetParameters(parameterList);

        //    }
        //    if (title.Count() > 92)
        //    {

        //        string title1 = title.Substring(0, 23);
        //        string title2 = title.Remove(0, 23).Substring(0, 23);
        //        string title3 = title.Remove(0, 46).Substring(0, 23);
        //        string title4 = title.Remove(0, 69).Substring(0, 23);
        //        localReport.ReportPath = Server.MapPath("~/Report/jungong/anjuanfengpi.rdlc");
        //        ReportDataSource reportDataSource1 = new ReportDataSource("jgfengpiDataSet", ds1);
        //        localReport.DataSources.Add(reportDataSource1);
        //        List<ReportParameter> parameterList = new List<ReportParameter>();
        //        parameterList.Add(new ReportParameter("title1", title1.ToString().Trim()));
        //        parameterList.Add(new ReportParameter("title2", title2.ToString().Trim()));
        //        parameterList.Add(new ReportParameter("title3", title3.ToString().Trim()));
        //        parameterList.Add(new ReportParameter("title4", title4.ToString().Trim()));
        //        localReport.SetParameters(parameterList);

        //    }
        //    string reportType = type;
        //    string mimeType;
        //    string encoding;
        //    string fileNameExtension;

        //    string deviceInfo =
        //        "<DeviceInfo>" +
        //        "<OutPutFormat>" + type + "</OutPutFormat>" +
        //        "</DeviceInfo>";
        //    Warning[] warnings;
        //    string[] streams;
        //    byte[] renderedBytes;
        //    renderedBytes = localReport.Render(
        //           reportType,
        //           deviceInfo,
        //           out mimeType,
        //           out encoding,
        //           out fileNameExtension,
        //           out streams,
        //           out warnings
        //           );
        //    return File(renderedBytes, mimeType);
        //}

        public string pickUUID()
        {
            string uuidInQR = "";
            /*************************数据库初始化**************************/
            //数据库链接
            SqlConnection connect = new SqlConnection();
            string conectionString = "server=.;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web"; ;
            connect.ConnectionString = conectionString;
            connect.Open();
            //数据库查询，提取ArchivesDetail表
            SqlCommand command = new SqlCommand();
            command.Connection = connect;
            command.CommandType = CommandType.Text;
            command.CommandText = "select * from ArchivesDetail";
            //解析表
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataSet);
            dataTable = dataSet.Tables[0];
            /*************************数据库初始化**************************/
            /***********************生成或提取UUID********************************/

            //这里是对所有的uuid进行操作，如果有uuid则提取，没有则生成
            //提取以UUID为key的列表,对所有的uuid进行检测
            foreach (DataRow row in dataTable.Rows)
            {
                string uuid = Convert.ToString(row["UUID"]);
                //判断某一行的uuid是否为空
                if (!uuid.Equals(""))
                {
                    //uuid不为空，提取uuid
                    uuidInQR = uuid;
                }
                else if (uuid.Equals(""))
                {
                    //uuid为空，创建uuid
                    string id = System.Guid.NewGuid().ToString("N");  //创建uuid
                    //预增加uuid的值
                    row["UUID"] = id;
                    uuidInQR = id;

                }
            }
            //最后uuidInQR应该是表中最后一个uuid，应该根据你所需要的项目提取不同的uuid
            /***********************生成或提取UUID********************************/

            //更新数据库
            SqlCommandBuilder mySqlCommandBuilder = new SqlCommandBuilder(adapter);
            adapter.Update(dataSet);
            adapter.Dispose();
            connect.Close();
            return uuidInQR;
        }





        public byte[] QRCodeGenerate(string uuid)
        {
            string address = "http://222.195.148.137:801/UUID/?id=0&uuid=" + uuid;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(address, QRCodeGenerator.ECCLevel.Q);
            QRCode qrcode = new QRCode(qrCodeData);
            //生成二维码
            Bitmap image = qrcode.GetGraphic(5, Color.Black, Color.White, null, 15, 6, true);
            //大小     暗色      亮色    中心图标及边框及大小    
            MemoryStream imageStream = new MemoryStream();
            image.Save(imageStream, ImageFormat.Jpeg);
            byte[] dataBytes = imageStream.GetBuffer();
            return dataBytes;
        }

        public ActionResult Reporting1(string archivesNo)
        {
            //FileStream fs1 = new FileStream(@"C:/Users/Administrator/Desktop/269.jpg", FileMode.Open);
            //FileStream fs2 = new FileStream(@"C:/Users/Administrator/Desktop/268.jpg", FileMode.Open);
            //BinaryReader br1 = new BinaryReader(fs1);
            //byte[] bt1 = br1.ReadBytes((int)fs1.Length);
            var a = from aa in db.ArchivesDetail
                    where aa.archivesNo == archivesNo
                    select aa;
            ArchivesDetail archivesDetail = a.First();
            string uu = archivesDetail.UUID;
            if (uu == null || uu == "") {
                //pickUUID();//第一次初始化案卷表

                //如果该条案卷没有uuid，先生成
                archivesDetail.UUID = System.Guid.NewGuid().ToString("N");  //创建uuid
                uu = archivesDetail.UUID;
            }
            archivesDetail.UUID = uu;
            byte[] bt1 = QRCodeGenerate(uu);         
            archivesDetail.pic = bt1;
            archivesDetail.UUID = uu;
            db.Entry(archivesDetail).State = EntityState.Modified;
            db.SaveChanges();

            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider=SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT     vw_archiveInfo.* FROM         vw_archiveInfo where archivesNo='" + archivesNo + "' ";
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            DataSet ds = new DataSet(); 
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            CrystalReportViewer repview = new CrystalReportViewer();
            adapter.Fill(ds);                ///////报表连接数据库,根据建水晶报表时的连接字符串设置
            ds.DataSetName = "jgfengpiDataSet";
            DataTable dt1 = ds.Tables[0];
            conn.Close();
            rptH.Load(Server.MapPath("~/") + "//Report//jungong//AnJuanFengPi.rpt");
            rptH.SetDataSource(dt1);


            System.IO.Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);


            return File(stream, "application/pdf");
        }
        public ActionResult Reporting2(string archivesNo, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db.ArchivesDetail
                      where ad.archivesNo == archivesNo
                      select ad;      
            localReport.ReportPath = Server.MapPath("~/Report/jungong/备考表.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("beikaobiao", ds1);
            localReport.DataSources.Add(reportDataSource1);

            string reportType = type;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
                "<DeviceInfo>" +
                "<OutPutFormat>" + type + "</OutPutFormat>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            renderedBytes = localReport.Render(
                   reportType,
                   deviceInfo,
                   out mimeType,
                   out encoding,
                   out fileNameExtension,
                   out streams,
                   out warnings
                   );
            return File(renderedBytes, mimeType);
        }

        public JsonResult title()//jsy动态框方法
        {
            var list = from bb in db.WordTable.ToList()
                       where bb.character == 1
                       orderby bb.newid
                       select new
                       {
                           ID = bb.id,
                           name = bb.wordName,
                           ch = bb.character,
                           newId = bb.newid
                       };
            //var name = Request.Form["fileName"];
            //var title = from bb in db.WordTable.ToList()
            //            where bb.character == 1
            //            where bb.wordName.Trim() == name
            //            select bb;
            //ViewBag.title = title.First().id;
            
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Responser()
        {
            var list1 = from bb in db.WordTable.ToList()
                        where bb.character == 2
                        orderby bb.newid
                        select new
                        {
                            ID = bb.id,
                            name = bb.wordName,
                            ch = bb.character,
                            newId = bb.newid
                        };
            //var responser = Request.Form["responsible"];
            //var response = from bb in db.WordTable.ToList()
            //               where bb.character == 2
            //               where bb.wordName.Trim() == responser
            //               select bb;
            //ViewBag.response = response.First().id;
            return Json(list1, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ArchiveEnter(string sortOrder, string currentFilter, string SearchString, int? page, int? SelectedID)
        {
            ViewData["pagename"] = "ArchivesEnter/ArchiveEnter";
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程序号", Value = "1"},
                new SelectListItem { Text = "设计单位", Value = "2" },
                new SelectListItem { Text = "施工单位", Value = "3" },
                new SelectListItem { Text = "项目顺序号", Value = "4" },
                new SelectListItem { Text = "起始排架号", Value = "5" },
                new SelectListItem { Text = "档号", Value = "6" },


            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            int t = SelectedID.GetValueOrDefault();

            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }
            ViewBag.CurrentFilter = SearchString;
            var vwprojectlist = from ad in db.vw_projectList
                                where ad.status == "6"
                                select ad;


            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (t)
                {
                    case 0:
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 1:
                    
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectNo.ToString().Contains(SearchString));//根据地点搜索
                        break;
                    case 2:

                        vwprojectlist = vwprojectlist.Where(ad => ad.disignOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwprojectlist = vwprojectlist.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                        break;
                    case 4:

                        vwprojectlist = vwprojectlist.Where(ad => ad.paperProjectSeqNo.ToString().Contains(SearchString));//根据工程序号搜索
                        break;
                    case 5:

                        vwprojectlist = vwprojectlist.Where(ad => ad.startPaijiaNo.ToString().Contains(SearchString));
                        break;
                    case 6:

                        vwprojectlist = vwprojectlist.Where(ad => ad.startArchiveNo.Contains(SearchString));//根据责任书编号搜索
                        break;



                }

            }


            string user = User.Identity.Name;
            bool flag = false;
            var depart = from c in ab.AspNetUsers
                         where c.UserName == user && c.DepartmentId==2
                         select c.RoleName;
            if(depart.Count()!=0)
            {
                foreach (string Role in depart)
                {
                    if (Role == "科长" || Role == "科员")
                    {
                        flag = true; break;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                if (flag == true)
                {
                     vwprojectlist = from ad in vwprojectlist
                                     where ad.collator.Contains(user)
                                        select ad;
                }
            }
           
            

            
            vwprojectlist = vwprojectlist.OrderByDescending(s => s.paperProjectSeqNo);// 默认按项目顺序号排列
            ViewBag.result = JsonConvert.SerializeObject(vwprojectlist);
            return View();
        }
        public ActionResult Enter(long? id)
        {
            if (id== null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var archive = from a in db.ArchivesDetail
                          where a.paperProjectSeqNo ==id
                          orderby a.volNo
                          select a;
            ViewBag.id = id;
            if (archive == null)
            {
                return HttpNotFound();
            }
            return View(archive);
           

            
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //传入
        public ActionResult Enter(string action, long id)
        {
            if (action == "案卷批量导入")
            {

                HttpPostedFileBase file = Request.Files["files"];//获取上传的文件
                string FileName;
                string savePath;
                Array arrFilinfo;
                string juanneiPath;
                if (file == null || file.ContentLength <= 0)
                {
                    ViewBag.error = "文件不能为空";
                    return View();
                }
                else
                {
                    string filename = System.IO.Path.GetFileName(file.FileName);
                    int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
                    string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名
                    string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名
                    int Maxsize = 10000 * 1024;//定义上传文件的最大空间大小为10M
                    string FileType = ".xls,.xlsx";//定义上传文件的类型字符串
                    FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;
                    if (!FileType.Contains(fileEx))
                    {
                        ViewBag.error = "文件类型不对，只能导入xls和xlsx格式的文件";
                        return View();
                    }
                    if (filesize >= Maxsize)
                    {
                        ViewBag.error = "上传文件超过10M，不能上传";
                        return View();
                    }
                    string path = AppDomain.CurrentDomain.BaseDirectory + "uploads\\excel\\";
                    savePath = System.IO.Path.Combine(path, FileName);
                    file.SaveAs(savePath);

                    //string Upfile = file.FileName.Substring(0, file.FileName.LastIndexOf("\\")) + "\\" + NoFileName + "\\";
                    ////if (System.IO.Directory.Exists(Upfile)) {
                    //    arrFilinfo = new System.IO.DirectoryInfo(Upfile).GetFiles();             
                    ////}
                }
                string result = string.Empty;
                string strConn;
                //strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + savePath + "; " + "Extended Properties=Excel 8.0;";
                strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + savePath + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'"; //此连接可以操作.xls与.xlsx文件 (支持Excel2003 和 Excel2007 的连接字符串)                                                                                                                                                  //备注： "HDR=yes;"是说Excel文件的第一行是列名而不是数据，"HDR=No;"正好与前面的相反。
                                                                                                                                                   //      "IMEX=1 "如果列中的数据类型不一致，使用"IMEX=1"可必免数据类型冲突。
                DataSet myDataSet = new DataSet();
                try
                {
                    OleDbConnection conn = new OleDbConnection(strConn);
                    conn.Open();
                    //返回Excel的架构，包括各个sheet表的名称,类型，创建时间和修改时间等　
                    DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
                    //包含excel中表名的字符串数组
                    string[] strTableNames = new string[dtSheetName.Rows.Count];
                    for (int k = 0; k < dtSheetName.Rows.Count; k++)
                    {
                        strTableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
                    }
                    OleDbDataAdapter myCommand = null;
                    DataTable dt = new DataTable();
                    //从指定的表明查询数据,可先把所有表明列出来供用户选择
                    string strExcel = "select*from [" + strTableNames[0] + "]";
                    myCommand = new OleDbDataAdapter(strExcel, strConn);
                    myCommand.Fill(myDataSet, "ExcelInfo");
                }
                catch (Exception ex)
                {
                    ViewBag.error = ex.Message;
                    return View();
                }
                DataTable table = myDataSet.Tables["ExcelInfo"].DefaultView.ToTable();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string projectName1 = table.Rows[i][0].ToString().Trim();//工程名称
                    if (projectName1 == "")
                    {
                        projectName1 = "";
                    }
                    string archivesCount1 = table.Rows[i][1].ToString().Trim();//总巻数
                    if (archivesCount1 == "")
                    {
                        archivesCount1 = "";
                    }
                    string volNo1 = table.Rows[i][2].ToString().Trim();//卷数
                    int volNo2;
                    if (volNo1 == "")
                    {
                        volNo2 = 0;
                    }
                    else
                    {
                        try
                        {
                            volNo2 = int.Parse(volNo1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('卷数应输入数字！');window.history.back();</script >");
                        }
                    }
                    string registrationNo1 = table.Rows[i][3].ToString().Trim();//总登记号
                    if (registrationNo1 == "")
                    {
                        registrationNo1 = "";
                    }
                    else if (registrationNo1.Length != 5)
                    {
                        registrationNo1 = registrationNo1.PadLeft(5, '0');
                    }
                    string shizhengNo1 = table.Rows[i][4].ToString().Trim();//市政号
                    if (shizhengNo1 == "")
                    {
                        shizhengNo1 = "";
                    }
                    string archivesNo1 = table.Rows[i][5].ToString().Trim();//档号
                    if (archivesNo1 == "")
                    {
                        archivesNo1 = "";
                    }
                    string paperProjectSeqNo1 = table.Rows[i][6].ToString().Trim();//项目顺序号
                    int paperProjectSeqNo2;
                    if (paperProjectSeqNo1 == "")
                    {
                        paperProjectSeqNo2 = 0;
                    }
                    else
                    {
                        try
                        {
                            paperProjectSeqNo2 = int.Parse(paperProjectSeqNo1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('项目顺序号应输入数字！');window.history.back();</script >");
                        }
                    }
                    string paijiaNo1 = table.Rows[i][7].ToString().Trim();//排架号
                    if (paijiaNo1 == "")
                    {
                        paijiaNo1 = "";
                    }
                    string licenseNo1 = table.Rows[i][8].ToString().Trim();// dr["执照号"].ToString().Trim();
                    if (licenseNo1 == "")
                    {
                        licenseNo1 = "";
                    }
                    string mapsheetNo1 = table.Rows[i][10].ToString().Trim(); //dr["图幅号"].ToString().Trim();
                    if (mapsheetNo1 == "")
                    {
                        mapsheetNo1 = "";
                    }
                    string microNo1 = table.Rows[i][11].ToString().Trim(); //dr["微缩号"].ToString().Trim();
                    if (microNo1 == "")
                    {
                        microNo1 = "";
                    }
                    string securityName1 = table.Rows[i][12].ToString().Trim(); //dr["密级"].ToString().Trim();
                    if (securityName1 != "秘密" && securityName1 != "机密" && securityName1 != "绝密" && securityName1 != "一般")
                    {
                        return Content("<script >alert('密级的输入格式不正确！（秘密/机密/绝密/一般）');window.history.back();</script >");
                    }
                    string retentionPeriodName1 = table.Rows[i][13].ToString().Trim(); //dr["保存期限"].ToString().Trim();
                    if (retentionPeriodName1 != "长期" && retentionPeriodName1 != "永久" && retentionPeriodName1 != "短期")
                    {
                        return Content("<script >alert('保存期限的输入格式不正确！（长期/永久/短期）');window.history.back();</script >");
                    }
                    string structureTypeName1 = table.Rows[i][14].ToString().Trim(); //dr["结构类型"].ToString().Trim();
                    if (structureTypeName1 != "砖混" && structureTypeName1 != "框架" && structureTypeName1 != "钢架" && structureTypeName1 != "无" && structureTypeName1 != "框剪" && structureTypeName1 != "轻钢" && structureTypeName1 != "砼剪")
                    {
                        return Content("<script >alert('结构类型的输入格式不正确！（砖混/框架/钢架/无/框剪/轻钢/砼剪）');window.history.back();</script >");
                    }
                    string buildingArea1 = table.Rows[i][15].ToString().Trim(); //dr["建筑面积"].ToString().Trim();
                    float buildingArea2 = 0;
                    if (buildingArea1 == "")
                    {
                        buildingArea2 = 0;
                    }
                    else
                    {
                        try
                        {
                            buildingArea2 = float.Parse(buildingArea1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('建筑面积应输入数字！');window.history.back();</script >");
                        }
                    }
                    string archivesTitle1 = table.Rows[i][16].ToString().Trim(); //dr["案卷题名"].ToString().Trim();
                    if (archivesTitle1 == "")
                    {
                        archivesTitle1 = "";
                    }
                    string firstResponsible1 = table.Rows[i][17].ToString().Trim(); //dr["第一责任者"].ToString().Trim();
                    if (firstResponsible1 == "")
                    {
                        firstResponsible1 = "";
                    }
                    string responsibleOther1 = table.Rows[i][18].ToString().Trim(); //dr["其他责任者"].ToString().Trim();
                    if (responsibleOther1 == "")
                    {
                        responsibleOther1 = "";
                    }
                    string developmentOrganization1 = table.Rows[i][19].ToString().Trim(); //dr["建设单位"].ToString().Trim();
                    if (developmentOrganization1 == "")
                    {
                        developmentOrganization1 = "";
                    }
                    string transferUnit1 = table.Rows[i][20].ToString().Trim(); //dr["移交单位"].ToString().Trim();
                    if (transferUnit1 == "")
                    {
                        transferUnit1 = "";
                    }
                    string disignOrganization1 = table.Rows[i][21].ToString().Trim(); //dr["设计单位"].ToString().Trim();
                    if (disignOrganization1 == "")
                    {
                        disignOrganization1 = "";
                    }
                    string constructionOrganization1 = table.Rows[i][22].ToString().Trim(); //dr["施工单位"].ToString().Trim();
                    if (constructionOrganization1 == "")
                    {
                        constructionOrganization1 = "";
                    }
                    string textMaterial1 = table.Rows[i][23].ToString().Trim(); //dr["文字材料"].ToString().Trim();
                    int textMaterial2 = 0;
                    if (textMaterial1 == "")
                    {
                        textMaterial2 = 0;
                    }
                    else
                    {
                        try
                        {
                            textMaterial2 = int.Parse(textMaterial1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('文字材料应输入数字！');window.history.back();</script >");
                        }
                    }
                    string drawing1 = table.Rows[i][24].ToString().Trim(); //dr["图纸"].ToString().Trim();
                    int drawing2 = 0;
                    if (drawing1 == "")
                    {
                        drawing2 = 0;
                    }
                    else
                    {
                        try
                        {
                            drawing2 = int.Parse(drawing1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('图纸应输入数字！');window.history.back();</script >");
                        }
                    }
                    string photoCount1 = table.Rows[i][25].ToString().Trim(); //dr["照片"].ToString().Trim();
                    int photoCount2 = 0;
                    if (photoCount1 == "")
                    {
                        photoCount2 = 0;
                    }
                    else
                    {
                        try
                        {
                            photoCount2 = int.Parse(photoCount1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('照片应输入数字！');window.history.back();</script >");
                        }
                    }
                    string archiveThickness1 = table.Rows[i][26].ToString().Trim(); //dr["案卷厚度"].ToString().Trim();
                    int archiveThickness2 = 0;
                    if (archiveThickness1 == "")
                    {
                        archiveThickness2 = 0;
                    }
                    else
                    {
                        try
                        {
                            archiveThickness2 = int.Parse(archiveThickness1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('案卷厚度应输入数字！');window.history.back();</script >");
                        }
                    }
                    string bianzhiTime1 = table.Rows[i][27].ToString().Trim(); //dr["编制日期"].ToString().Trim();
                    if (bianzhiTime1 == "")
                    {
                        bianzhiTime1 = "";
                    }
                    string jgDate1 = table.Rows[i][28].ToString().Trim(); //dr["进馆日期"].ToString().Trim();
                    DateTime? jgDate2;
                    if (jgDate1 == "")
                    {
                        jgDate2 = null;
                    }
                    else
                    {
                        try
                        {
                            jgDate2 = DateTime.Parse(jgDate1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('进馆日期格式不正确！（2018/10/1）');window.history.back();</script >");
                        }
                    }
                    string height1 = table.Rows[i][29].ToString().Trim(); //dr["高度"].ToString().Trim();
                    float height2 = 0;
                    if (height1 == "")
                    {
                        height2 = 0;
                    }
                    else
                    {
                        try
                        {
                            height2 = float.Parse(height1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('高度应输入数字！');window.history.back();</script >");
                        }
                    }
                    string changeLog1 = table.Rows[i][30].ToString().Trim(); //dr["变更情况"].ToString().Trim();
                    if (changeLog1 != "没有" && changeLog1 != "有")
                    {
                        return Content("<script >alert('变更情况的输入格式不正确！（没有/有）');window.history.back();</script >");
                    }
                    string location1 = table.Rows[i][31].ToString().Trim(); //dr["工程地址"].ToString().Trim();
                    if (location1 == "")
                    {
                        location1 = "";
                    }
                    string remarks1 = table.Rows[i][32].ToString().Trim(); //dr["备注"].ToString().Trim();
                    if (remarks1 == "")
                    {
                        remarks1 = "";
                    }

                    string newlocation1 = table.Rows[i][33].ToString().Trim(); //最新工程地址
                    if (newlocation1 == "")
                    {
                        newlocation1 = "";
                    }

                    string overground1 = table.Rows[i][34].ToString().Trim(); //dr["地上"].ToString().Trim();
                    if (overground1 == "")
                    {
                        overground1 = "";
                    }
                    string underground1 = table.Rows[i][35].ToString().Trim(); //dr["地下"].ToString().Trim();
                    if (underground1 == "")
                    {
                        underground1 = "";
                    }
                    string fazhaoTime1 = table.Rows[i][36].ToString().Trim(); //dr["发照日期"].ToString().Trim();
                    if (fazhaoTime1 == "")
                    {
                        fazhaoTime1 = "";
                    }
                    string kaigongTime1 = table.Rows[i][37].ToString().Trim(); //dr["开工日期"].ToString().Trim();
                    if (kaigongTime1 == "")
                    {
                        kaigongTime1 = "";
                    }
                    string jungongTime1 = table.Rows[i][38].ToString().Trim(); //dr["竣工日期"].ToString().Trim();
                    if (jungongTime1 == "")
                    {
                        jungongTime1 = "";
                    }
                    string indexer1 = table.Rows[i][39].ToString().Trim(); //dr["标引员"].ToString().Trim();
                    if (indexer1 == "")
                    {
                        indexer1 = "";
                    }
                    //else if (indexer1 != "孙美艳" && indexer1 != "臧宁" && indexer1 != "庞蕾")
                    //{
                    //    return Content("<script >alert('标引员的输入不正确！（孙美艳/臧宁/庞蕾）');window.history.back();</script >");
                    //}
                    string checker1 = table.Rows[i][40].ToString().Trim(); //dr["审核员"].ToString().Trim();
                    if (checker1 == "")
                    {
                        checker1 = "";
                    }
                    //else if (checker1 != "孙美艳" && checker1 != "臧宁" && checker1 != "庞蕾")
                    //{
                    //    return Content("<script >alert('审核员的输入不正确！（孙美艳/臧宁/庞蕾）');window.history.back();</script >");
                    //}
                    string Typist1 = table.Rows[i][41].ToString().Trim(); //dr["录入员"].ToString().Trim();
                    if (Typist1 == "")
                    {
                        Typist1 = "";
                    }
                    //else if (Typist1 != "孙美艳" && Typist1 != "臧宁" && Typist1 != "庞蕾")
                    //{
                    //    return Content("<script >alert('录入员的输入不正确！（孙美艳/臧宁/庞蕾）');window.history.back();</script >");
                    //}
                    string indexeDate1 = table.Rows[i][42].ToString().Trim(); //dr["标引日期"].ToString().Trim();
                    DateTime? indexeDate2;
                    if (indexeDate1 == "")
                    {
                        indexeDate2 = null;
                    }
                    else
                    {
                        try
                        {
                            indexeDate2 = DateTime.Parse(indexeDate1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('标引日期格式不正确！（2018/10/1）');window.history.back();</script >");
                        }
                    }
                    string checkDate1 = table.Rows[i][43].ToString().Trim(); //dr["审核日期"].ToString().Trim();
                    DateTime? checkDate2;
                    if (checkDate1 == "")
                    {
                        checkDate2 = null;
                    }
                    else
                    {
                        try
                        {
                            checkDate2 = DateTime.Parse(checkDate1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('审核日期格式不正确！（2018/10/1）');window.history.back();</script >");
                        }
                    }
                    string TyperDate1 = table.Rows[i][44].ToString().Trim(); //dr["录入日期"].ToString().Trim();
                    DateTime? TyperDate2;
                    if (TyperDate1 == "")
                    {
                        TyperDate2 = null;
                    }
                    else
                    {
                        try
                        {
                            TyperDate2 = DateTime.Parse(TyperDate1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('录入日期格式不正确！（2018/10/1）');window.history.back();</script >");
                        }
                    }
                    string strSql1 = string.Format("update UrbanCon.dbo.ProjectInfo set securityID=(select securityID from UrbanCon.dbo.SecurityClassification where securityName='" + securityName1 + "'),retentionPeriodNo=(select retentionPeriodNo from UrbanCon.dbo.RetentionPeriod where retentionPeriodName='" + retentionPeriodName1 + "'),developmentOrganization='" + developmentOrganization1 + "',disignOrganization='" + disignOrganization1 + "',constructionOrganization='" + constructionOrganization1 + "',textMaterial=" + textMaterial2 + ",drawing=" + drawing2 + ",location='" + location1 + ",newLocation" + newlocation1 + "' where projectName =' " + projectName1 + "'");
                    //string strSql2 = string.Format("update UrbanCon.dbo.PaperArchives set licenseNo='" + licenseNo1 + "',structureTypeID=(select structureTypeID from UrbanCon.dbo.StructureType where structureTypeName='" + structureTypeName1 + "'),buildingArea=" + buildingArea2 + ",firstResponsible='" + firstResponsible1 + "',responsibleOther='" + responsibleOther1 + "',transferUnit='" + transferUnit1 + "',textMaterial=" + textMaterial2 + ",drawing=" + drawing2 + ",photoCount=" + photoCount2 + ",jgDate='" + jgDate2 + "',height='" + height2 + "',changeLog='" + changeLog1 + "',remarks='" + remarks1 + "',overground='" + overground1 + "',underground='" + underground1 + "',archivesCount='" + archivesCount1 + "' where paperProjectSeqNo = " + paperProjectSeqNo2);
                    string strSql2 = string.Format("update UrbanCon.dbo.PaperArchives set licenseNo='" + licenseNo1 + "',structureTypeID=(select structureTypeID from UrbanCon.dbo.StructureType where structureTypeName='" + structureTypeName1 + "'),buildingArea=" + buildingArea2 + ",firstResponsible='" + firstResponsible1 + "',responsibleOther='" + responsibleOther1 + "',transferUnit='" + transferUnit1 + "',textMaterial=" + textMaterial2 + ",drawing=" + drawing2 + ",jgDate='" + jgDate2 + "',remarks='" + remarks1 + "',overground='" + overground1 + "',underground='" + underground1 + "',archivesCount='" + archivesCount1 + "' where paperProjectSeqNo = " + paperProjectSeqNo2);
                    var pap = from a in db.ArchivesDetail
                              where a.paperProjectSeqNo == paperProjectSeqNo2
                              select a;
                    string strSql3;
                    if (pap.Count() == 0)
                    {
                        strSql3 = string.Format("insert into UrbanCon.dbo.ArchivesDetail(volNo, registrationNo, archivesNo, shizhengNo, paperProjectSeqNo, paijiaNo, licenseNo, archivesTitle, firstResponsible,responsibleOther,developmentUnit,transferUnit,designUnit,constructionUnit,textMaterial,drawing,photoCount,archiveThickness,bianzhiTime,jgDate,remarks,fazhaoTime,kaigongTime,jungongTime,indexer,checker,typist,indexDate,checkDate,typerDate) values(" + volNo2 + ",'" + registrationNo1 + "','" + archivesNo1 + "','" + shizhengNo1 + "'," + paperProjectSeqNo2 + ",'" + paijiaNo1 + "','" + licenseNo1 + "','" + archivesTitle1 + "','" + firstResponsible1 + "','" + responsibleOther1 + "','" + developmentOrganization1 + "','" + transferUnit1 + "','" + disignOrganization1 + "','" + constructionOrganization1 + "'," + textMaterial2 + "," + drawing2 + "," + photoCount2 + "," + archiveThickness2 + ",'" + bianzhiTime1 + "','" + jgDate2 + "','" + remarks1 + "','" + fazhaoTime1 + "','" + kaigongTime1 + "','" + jungongTime1 + "','" + indexer1 + "','" + checker1 + "','" + Typist1 + "','" + indexeDate2 + "','" + checkDate2 + "','" + TyperDate2 + "')");
                    }
                    else
                    {
                        strSql3 = string.Format("update UrbanCon.dbo.ArchivesDetail set volNo=" + volNo2 + ",registrationNo='" + registrationNo1 + "',archivesNo='" + archivesNo1 + "',shizhengNo='" + shizhengNo1 + "',paijiaNo='" + paijiaNo1 + "',licenseNo ='" + licenseNo1 + "',archivesTitle='" + archivesTitle1 + "',firstResponsible='" + firstResponsible1 + "',responsibleOther='" + responsibleOther1 + "',developmentUnit='" + developmentOrganization1 + "',transferUnit='" + transferUnit1 + "',designUnit='" + disignOrganization1 + "',constructionUnit='" + constructionOrganization1 + "',textMaterial=" + textMaterial2 + ",drawing=" + drawing2 + ",photoCount=" + photoCount2 + ",archiveThickness=" + archiveThickness2 + ",bianzhiTime='" + bianzhiTime1 + "',jgDate='" + jgDate2 + "',remarks='" + remarks1 + "',fazhaoTime='" + fazhaoTime1 + "',kaigongTime='" + kaigongTime1 + "',jungongTime='" + jungongTime1 + "',indexer='" + indexer1 + "',checker='" + checker1 + "',typist='" + Typist1 + "',indexDate='" + indexeDate2 + "',checkDate='" + checkDate2 + "',typerDate='" + TyperDate2 + "' where registrationNo = '" + registrationNo1 + "'");
                    }
                    SqlConnection sqlConnection = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
                    try
                    {
                        sqlConnection.Open();
                        SqlCommand sqlCmd = new SqlCommand();
                        sqlCmd.CommandText = strSql1;
                        sqlCmd.Connection = sqlConnection;
                        SqlDataReader sqlDataReader1 = sqlCmd.ExecuteReader();
                        sqlDataReader1.Close();
                        sqlCmd.CommandText = strSql2;
                        sqlCmd.Connection = sqlConnection;
                        SqlDataReader sqlDataReader2 = sqlCmd.ExecuteReader();
                        sqlDataReader2.Close();
                        sqlCmd.CommandText = strSql3;
                        sqlCmd.Connection = sqlConnection;
                        SqlDataReader sqlDataReader3 = sqlCmd.ExecuteReader();
                        sqlDataReader3.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                    //return RedirectToAction("EnterAndSee", new { id = RegistrationNo });
                }
                return Content("<script >alert('案卷成功导入，请导入卷内信息！');window.location.href='/ArchivesEnter/Enter/?id=" + id + "';</script >");
          
            }
            if (action == "卷内信息导入")
            {
                var files = Request.Files;
                for (int j = 1; j < files.Count; j++)
                {
                    HttpPostedFileBase fileitem = files[j];
                    string FileName;
                    string savePath;
                    string NoFileName;
                    if (fileitem == null || fileitem.ContentLength <= 0)
                    {
                        ViewBag.error = "文件不能为空";
                        return View();
                    }
                    else
                    {
                        string filename = System.IO.Path.GetFileName(fileitem.FileName);
                        int filesize = fileitem.ContentLength;//获取上传文件的大小单位为字节byte
                        string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名
                        NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名
                        int Maxsize = 10000 * 1024;//定义上传文件的最大空间大小为10M
                        string FileType = ".xls,.xlsx";//定义上传文件的类型字符串
                        FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;
                        if (!FileType.Contains(fileEx))
                        {
                            ViewBag.error = "文件类型不对，只能导入xls和xlsx格式的文件";
                            return View();
                        }
                        if (filesize >= Maxsize)
                        {
                            ViewBag.error = "上传文件超过10M，不能上传";
                            return View();
                        }
                        string path = AppDomain.CurrentDomain.BaseDirectory + "uploads\\excel\\";
                        savePath = System.IO.Path.Combine(path, FileName);
                        fileitem.SaveAs(savePath);
                    }
                    string result = string.Empty;
                    string strConn;
                    //strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + savePath + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + savePath + "; " + "Extended Properties = 'Excel 8.0;HDR=Yes;IMEX=1'";

                    //strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + savePath + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'"; //此连接可以操作.xls与.xlsx文件 (支持Excel2003 和 Excel2007 的连接字符串)
                    //备注： "HDR=yes;"是说Excel文件的第一行是列名而不是数据，"HDR=No;"正好与前面的相反。
                    //      "IMEX=1 "如果列中的数据类型不一致，使用"IMEX=1"可必免数据类型冲突。
                    DataSet myDataSet = new DataSet();
                    try
                    {
                        OleDbConnection conn = new OleDbConnection(strConn);
                        conn.Open();
                        //返回Excel的架构，包括各个sheet表的名称,类型，创建时间和修改时间等　
                        DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
                        //包含excel中表名的字符串数组
                        string[] strTableNames = new string[dtSheetName.Rows.Count];
                        for (int k = 0; k < dtSheetName.Rows.Count; k++)
                        {
                            strTableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
                        }
                        OleDbDataAdapter myCommand = null;
                        DataTable dt = new DataTable();
                        //从指定的表明查询数据,可

                        //先把所有表明列出来供用户选择
                        string strExcel = "select*from [" + strTableNames[0] + "]";
                        myCommand = new OleDbDataAdapter(strExcel, strConn);
                        myCommand.Fill(myDataSet, "ExcelInfo");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.error = ex.Message;
                        return View();
                    }
                    DataTable table = myDataSet.Tables["ExcelInfo"].DefaultView.ToTable();
                    string name = System.IO.Path.GetFileNameWithoutExtension(NoFileName);
                    if (name.Length != 5)
                    {
                        name = name.PadLeft(5, '0');
                    }
                    //int a = int.Parse(name);
                    var num = from ad in db.FileInfo
                              where ad.dengjihao == name
                              select ad;
                    int m = num.Count();
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        string SeqNo = table.Rows[i][0].ToString().Trim();
                        int seq;
                        if (SeqNo == "")
                        {
                            break;
                            //seq = 0;
                        }
                        else
                        {
                            try
                            {
                                seq = Convert.ToInt32(SeqNo);
                            }
                            catch (Exception ex)
                            {
                                return Content("<script >alert('序号应是数字！');window.history.back();</script >");
                            }

                        }
                        string Type = table.Rows[i][1].ToString().Trim();
                        if (Type == "")
                        {
                            Type = "";
                        }
                        else if (Type == "文字" || Type == "图纸" || Type == "文字及图纸")
                        {
                        }
                        else
                        {
                            return Content("<script >alert('类型输入有错！（文字/图纸/文字及图纸）');window.history.back();</script >");
                        }
                        string FileNo = table.Rows[i][2].ToString().Trim();
                        //int member;
                        if (FileNo == "")
                        {
                            FileNo = "";
                        }
                        //else
                        //{
                        //    try
                        //    {
                        //        member = Convert.ToInt32(FileNo);

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        return Content("<script >alert('文件编号有错！');window.history.back();</script >");
                        //    }

                        //}
                        string FileTitle = table.Rows[i][3].ToString().Trim();
                        if (FileTitle == "")
                        {
                            FileTitle = "";
                        }

                        string Responsible = table.Rows[i][4].ToString().Trim();
                        if (Responsible == "")
                        {
                            Responsible = "";
                        }
                        else {
                            Responsible = new Regex("[\\s]+").Replace(Responsible, "");
                        }
                       
                        string StartDate = table.Rows[i][5].ToString().Trim();
                        if (StartDate == "")
                        {
                            StartDate = "";
                        }
                        string StartPage = table.Rows[i][6].ToString().Trim();
                        if (StartPage == "")
                        {
                            StartPage = "";
                        }
                        string Remarks = table.Rows[i][7].ToString().Trim();
                        if (Remarks == "")
                        {
                            Remarks = "";
                        }
                        var archiveno = from ab in db.ArchivesDetail
                                        where ab.registrationNo == name
                                        select ab;
                        string archno = archiveno.First().archivesNo;
                        string strSql;
                        if (m == 0)
                        {
                            strSql = string.Format("insert into UrbanCon.dbo.FileInfo(seqNo, type, fileNo,fileName, responsible, startPageNo, startDate, remarks,dengjihao,archivesNo) values(" + seq + ",'" + Type + "','" + FileNo + "','" + FileTitle + "','" + Responsible + "','" + StartPage + "','" + StartDate + "','" + Remarks + "','" + name + "','" + archno + "')");
                            SqlConnection sqlConnection = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
                            try
                            {
                                sqlConnection.Open();
                                SqlCommand sqlCmd = new SqlCommand();
                                sqlCmd.CommandText = strSql;
                                sqlCmd.Connection = sqlConnection;
                                SqlDataReader sqlDataReader1 = sqlCmd.ExecuteReader();
                                sqlDataReader1.Close();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                sqlConnection.Close();
                            }
                        }
                        else
                        {
                            return Content("<script >alert('请勿重复输入！');window.history.back();</script >");
                        }
                    }
                }
                return Content("<script >alert('导入完成！');window.location.href='/ArchivesEnter/Enter/?id=" + id + "';</script >");
            }
            return View();
        }



        public ActionResult EnterAndSee(string id,int? id2)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(id2==0)
            {
                ViewData["button5"] = true;
            }
            var projectinfo = from b in db.vw_passList
                              where b.registrationNo == id
                              select b;
            long paperseq = projectinfo.First().paperProjectSeqNo;
            string seq = paperseq.ToString().Trim();
            var project = from c in db.vw_passList
                          where c.paperProjectSeqNo == paperseq
                          orderby c.volNo
                          select c;
            if (projectinfo.Count() == 0)
            {
                ViewData["checkname1"] = 3;
               
            }
            //初始化相关选择控件
            string user = User.Identity.Name.Trim();
            if (user != "业务科")
            {
                projectinfo.First().typist = user;
            }
            ViewBag.indexer = new SelectList(ab.AspNetUsers, "UserName", "UserName", projectinfo.First().indexer);
            //ViewBag.checker = new SelectList(ab.AspNetUsers, "UserName", "UserName", projectinfo.First().checker);
            ViewBag.checker = new SelectList(ab.AspNetUsers, "UserName", "UserName","张春颖");
            ViewBag.Typist = new SelectList(ab.AspNetUsers, "UserName", "UserName", projectinfo.First().typist);
            ViewBag.securityName = new SelectList(db.SecurityClassification, "securityID", "securityName", projectinfo.First().securityID);
            ViewBag.retentionPeriodName = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", projectinfo.First().retentionPeriodNo);
            ViewBag.structureTypeName = new SelectList(db.StructureType, "structureTypeID", "structureTypeName", projectinfo.First().Expr14);
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "没有", Value = "0"},
                new SelectListItem { Text = "有", Value = "1"},
            };
            if (projectinfo.First().changeLog != null)
            {
                if (projectinfo.First().changeLog.Trim() == "没有")
                {
                    ViewBag.changeLog = new SelectList(list, "Value", "Text", 0);
                }
                else
                {
                    ViewBag.changeLog = new SelectList(list, "Value", "Text", 1);
                }
            }

            else
            {
                ViewBag.changeLog = new SelectList(list, "Value", "Text", 0);
            }


            //进行卷数判断，设置按钮可用性
            string Acount = projectinfo.First().archivesCount;
            if (Acount == null || Acount == "")
            {
                Acount = "0";
            }
                int zongjuanshu = Int32.Parse(Acount);
                int dijijuan;
            if (zongjuanshu<2)
                {
                    ViewData["button1"] = true;
                    ViewData["button2"] = true;
                    ViewData["button3"] = true;
                    ViewData["button4"] = true;
                ViewBag.jiaojiemulu = "display:inline-block";
                ViewBag.yijiaoshu = "display:inline-block";
            }
            if(projectinfo.First().volNo!=0)
                {
                     dijijuan =Convert.ToInt32(projectinfo.First().volNo);
                ViewBag.jiaojiemulu = "display:none";
                ViewBag.yijiaoshu = "display:none";
            }
                else if (projectinfo.First().startRegisNo.Trim() != "" && projectinfo.First().registrationNo.Trim() != "")
                {
                    string s1 = projectinfo.First().registrationNo.Trim();
                    string s2 = projectinfo.First().startRegisNo.Trim();
                    dijijuan = Int32.Parse(s1.Substring(2, s1.Length - 2)) - Int32.Parse(s2.Substring(2, s2.Length - 2)) + 1;
                }
                else
                    dijijuan = 1;
                if (dijijuan <1)
                {
                   ViewData["checkname1"] = 4;
           
                     
                }
                if (dijijuan== project.First().volNo)
                {
                    ViewData["button1"] = true;
                    ViewData["button3"] = true;
                ViewBag.jiaojiemulu = "display:none";
                ViewBag.yijiaoshu = "display:none";
            }
                if (dijijuan == zongjuanshu)
                {
                    ViewData["button2"] = true;
                ViewData["button4"] = true;
                ViewBag.jiaojiemulu = "display:inline-block";
                ViewBag.yijiaoshu = "display:inline-block";
            }
                //编制分类号
                string strfenleihao = projectinfo.First().mainCategoryID;
                if (projectinfo.First().subDictionaryID != null)
                {
                 if (projectinfo.First().subDictionaryID.Trim() != "0")
                  {


                    strfenleihao = strfenleihao + projectinfo.First().subDictionaryID;
                    if (projectinfo.First().minorDictionaryID.Trim() != "0")
                    {
                        strfenleihao = strfenleihao + "." + projectinfo.First().minorDictionaryID;

                    }

                 }
                }
                ViewData["ClassNo"] = strfenleihao; //分类号
                ViewData["ProjectName"] = projectinfo.First().projectName;
           
            string jgdate =Convert.ToDateTime(projectinfo.First().jgDate).ToString("yyyy-MM-dd");
             ViewData["jgDate"]= jgdate;
            if (jgdate.Trim() == "1753-01-01")
             {
                ViewData["jgDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
             }

             string bydate  = Convert.ToDateTime(projectinfo.First().indexDate).ToString("yyyy-MM-dd");
             ViewData["indexeDate"] = bydate;
             if (bydate.Trim() == "1753-01-01")
                {
                    ViewData["indexeDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
                }
                string jcdate = Convert.ToDateTime(projectinfo.First().checkDate).ToString("yyyy-MM-dd");
               ViewData["checkDate"] = jcdate;
                if (jcdate.Trim() == "1753-01-01")
                {
                    ViewData["checkDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
                }
                string lrdate = Convert.ToDateTime(projectinfo.First().typerDate).ToString("yyyy-MM-dd");
                ViewData["TyperDate"] = lrdate;
               if (lrdate == "1753-01-01")
                {
                ViewData["TyperDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") +"-"+ DateTime.Now.Day.ToString("D2");
                }
               vw_passList paper = new vw_passList();
               paper = projectinfo.First();
            //判断案卷是否录入完毕
            if (project.First().archivesTitle!=null&& project.First().archivesTitle !="")//该卷是接着前一卷录入，将前一卷的值进行传递
            {

                if (projectinfo.First().archivesTitle == "" || projectinfo.First().archivesTitle == null)
                {
                    string lr = DateTime.Now.ToString();
                    ViewData["TyperDate"] = Convert.ToDateTime(lr.Trim()).ToString("yyyy-MM-dd");
                    ViewData["checkDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
                    string regis = id;
                    long re = Int32.Parse(regis);
                    int index = regis.Substring(0,1).IndexOf('0');
                    re = re - 1;
                    string regisNo = "";
                    if (index == -1)
                    {
                        regisNo = re.ToString();
                    }
                    else
                    {
                        regisNo = "0" + re.ToString();
                    }
                    var archive = from a in db.vw_passList
                                  where a.registrationNo == regisNo
                                  select a;
                  //初始化均为0 20170615 周林
                    paper.textMaterial = 0;
                    paper.photoCount = 0;
                    paper.drawing = 0;
                    paper.archivesTitle = archive.First().archivesTitle;
                    paper.licenseNo = archive.First().licenseNo;
                    paper.firstResponsible = archive.First().firstResponsible;
                    paper.responsibleOther = archive.First().responsibleOther;
                    paper.developmentOrganization = archive.First().developmentOrganization;
                    paper.transferUnit = archive.First().transferUnit;
                    paper.constructionOrganization = archive.First().constructionOrganization;
                    paper.designUnit = archive.First().designUnit;
                    //ViewBag.changeLog = new SelectList(list, "Value", "Text", archive.First().changeLog);
                    paper.location = archive.First().location;
                    paper.newLocation = archive.First().newLocation;
                    paper.height = archive.First().height;
                    paper.overground = archive.First().overground;
                    paper.underground = archive.First().underground;
                    ViewBag.indexer = new SelectList(ab.AspNetUsers, "UserName", "UserName", archive.First().indexer);
                    //ViewBag.checker = new SelectList(ab.AspNetUsers, "UserName", "UserName", archive.First().checker);
                    ViewBag.checker = new SelectList(ab.AspNetUsers, "UserName", "UserName", "张春颖");
                    ViewBag.Typist = new SelectList(ab.AspNetUsers, "UserName", "UserName", archive.First().typist);


                    paper.fazhaoTime = archive.First().fazhaoTime;
                    paper.jungongTime = archive.First().jungongTime;
                    paper.kaigongTime = archive.First().kaigongTime;
                    ViewData["checkname"] = 1;
                }
            }
            else
            {
                ViewData["checkname"] = 2;
                paper.textMaterial = 0;
                paper.photoCount = 0;
                paper.drawing = 0;
            }
                //判断案卷名是否录入
                string archivesNo="";
                bool flag=false;
                var item = from c in project
                       where c.archivesTitle == ""
                       select c;
              if(item.Count()!=0)
              {
                flag = true;
                 archivesNo = item.First().archivesTitle.Trim();
              }
          
            if (flag == true)
                {
                    ViewData["checkname"]= archivesNo;
                    
                }

            
            return View(paper);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //传入
        public ActionResult EnterAndSee(int? securityName, int ?retentionPeriodName,int ?structureTypeName, long paperProjectSeqNo ,string action,string RegistrationNo, string archivesNo, string volNo, string shizhengNo, string licenseNo,string TuFuhao,string SuoWeiHao,string buildingArea,string archivesTitle, string firstResponsible, string responsibleOther, string developmentOrganization,string transferUnit,string disignOrganization,string PaiJiaNo,string constructionOrganization,string TextMaterial,string  drawing, string PhotoCount, string ArchiveThickness,string bianzhiTime,string jgDate, string height, string location,string remarks, string newLocation,string overground,string underground,int ? changeLog,string fazhaoTime, string jungongTime,string kaigongTime,string indexer, string indexeDate,string checker,string checkDate,string   Typist,string  TyperDate)
        {

            
             var paperArchive = from a in db.PaperArchives
                               where a.paperProjectSeqNo == paperProjectSeqNo
                               select a;
            PaperArchives paperArchives = paperArchive.First();
            long project = Convert.ToInt32(paperArchives.projectID);
            var projectInfo = from b in db.ProjectInfo
                              where b.projectID == project
                              select b;
            ProjectInfo projects = projectInfo.First();
            var archivedetail = from c in db.ArchivesDetail
                                where c.paperProjectSeqNo == paperProjectSeqNo
                                select c;
           ArchivesDetail archivedetails = archivedetail.First();
            var vwprojictlist = from d in db.vw_projectList
                                where d.paperProjectSeqNo == paperProjectSeqNo
                                select d;
           
            var proje = from c in db.vw_passList
                          where c.paperProjectSeqNo == paperProjectSeqNo
                          select c;
            string stratregis = proje.First().startRegisNo;
            string endregis = proje.First().endRegisNo;
            //给相关选择控件赋值
            ViewBag.indexer = new SelectList(ab.AspNetUsers, "UserName", "UserName",indexer);
            ViewBag.checker = new SelectList(ab.AspNetUsers, "UserName", "UserName", checker);
            ViewBag.Typist = new SelectList(ab.AspNetUsers, "UserName", "UserName", Typist);
            ViewBag.securityName = new SelectList(db.SecurityClassification, "securityID", "securityName", securityName);
            ViewBag.retentionPeriodName = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", retentionPeriodName);
            ViewBag.structureTypeName = new SelectList(db.StructureType, "structureTypeID", "structureTypeName", structureTypeName);
            //List<SelectListItem> list = new List<SelectListItem> {
            //    new SelectListItem { Text = "没有", Value = "0"},
            //    new SelectListItem { Text = "有", Value = "1"},
            //};
            //int t = changeLog.GetValueOrDefault();
            //ViewBag.changeLog = new SelectList(list, "Value", "Text",t);

            if (action=="该工程案卷列表")
            {
                
                
              return RedirectToAction("Enter", new { id = paperProjectSeqNo });

            }
         

            if (action=="该卷文件列表")
            {
                int index = archivesNo.IndexOf('.');
                int index1 = index + 1;
                string str1 = archivesNo.Substring(0, index + 1);
                string str2 = archivesNo.Substring(index + 1, archivesNo.Length - 1 - index);
                return RedirectToAction("FileList", new { id1 = archivesNo, id = 0, id2 = 0 });
            }


            if (action=="首卷")
            {
   
              
                
                    var pro = from a in db.vw_passList
                              where a.paperProjectSeqNo== paperProjectSeqNo
                              orderby a.registrationNo 
                              select a.registrationNo;
                    string startregisNo = pro.First();
                    return RedirectToAction("EnterAndSee", new { id = startregisNo });
                
               
            

            }
           
            if (action == "前一卷")
            {
                string regis = RegistrationNo;
                long re = Int32.Parse(regis);
                int index = regis.Substring(0,1).IndexOf('0');
                re = re - 1;
                string regisNo = "";
                if (index==-1)
                {
                    regisNo = re.ToString();
                }
                else
                {
                    regisNo = "0" + re.ToString();
                }
                
                if (regisNo.ToString()==stratregis)
                {
                    var pro = from a in db.vw_passList
                              where a.paperProjectSeqNo== paperProjectSeqNo
                              orderby a.registrationNo
                              select a.registrationNo;
                    string startregisNo = pro.First();
                    return RedirectToAction("EnterAndSee", new { id = startregisNo });
                }
                else
                {
                    var pro = from a in db.vw_passList
                              where a.paperProjectSeqNo== paperProjectSeqNo && a.registrationNo == regisNo.ToString()
                              select a.registrationNo;
                    string startregisNo = pro.First();
                    return RedirectToAction("EnterAndSee", new { id = startregisNo });
                }


            }
            if (action == "保存并提交")
            {
                string archiveNo = archivesNo.Trim();
                if (archiveNo == "")
                {
                    return Content("<script >alert('档号不能为空！');window.history.back();</script >");

                }

                var ArchiveDe = from a in db.ArchivesDetail
                                where a.registrationNo == RegistrationNo
                                select a;
                ArchivesDetail archiveDetail = ArchiveDe.First();
                archiveDetail.registrationNo = RegistrationNo;
               
                if(archiveNo!= archiveDetail.archivesNo)
                {
                    var archive = from f in db.ArchivesDetail
                                  where f.archivesNo == archiveNo
                                  select f;
                    if(archive.Count()==0)
                    {
                        return Content("<script >alert('该档号已经使用，请修改档号后提交！');window.history.back();</script >");
                    }
                }
                    archiveDetail.archivesNo = archiveNo;
                    if (volNo.Trim() != "")
                    {
                        archiveDetail.volNo = Int32.Parse(volNo.Trim());
                    }
                    archiveDetail.paijiaNo = PaiJiaNo.Trim();
                    archiveDetail.archiveThickness = Int32.Parse(ArchiveThickness.Trim());

                    if (TextMaterial.Trim() != "")
                    {
                        archiveDetail.textMaterial = Int32.Parse(TextMaterial.Trim());//文字材料

                    }
                    archiveDetail.archivesTitle = archivesTitle;//案卷提名
                    archiveDetail.firstResponsible = firstResponsible;//第一负责人

                    if (drawing.Trim() != "")
                        archiveDetail.drawing = Int32.Parse(drawing.Trim());//图纸
                    if (PhotoCount.Trim() != "")//照片
                    {
                        archiveDetail.photoCount = Int32.Parse(PhotoCount.Trim());
                    }
                    archiveDetail.responsibleOther = responsibleOther;//其他责任者


                    archiveDetail.developmentUnit = developmentOrganization;
                    archiveDetail.constructionUnit = constructionOrganization;
                    archiveDetail.designUnit = disignOrganization;

                    archiveDetail.bianzhiTime = bianzhiTime.Trim();//编制日期
                                                                   //archiveDetail.bianzhiTime = DateTime.ParseExact(strbianzhiTime, "yyyy-MM-dd", null).Date;
                    archiveDetail.transferUnit = transferUnit;//移交单位           
                    archiveDetail.notearea = location;//附注改为工程地址
                    archiveDetail.licenseNo = licenseNo;//执照号
                    archiveDetail.shizhengNo = shizhengNo.Trim(); //市政档案号
                   
                    archiveDetail.remarks = remarks;//备注
                    string strbiaoyinriqi = indexeDate;//标引日期
                    archiveDetail.indexDate =Convert.ToDateTime (strbiaoyinriqi);
                    archiveDetail.indexer = indexer;//标引员


                    string strshenheriqi = checkDate;//审核日期
                    archiveDetail.checkDate = Convert.ToDateTime(strshenheriqi);
                    archiveDetail.checker = checker;//审核员

                    archiveDetail.kaigongTime = kaigongTime.Trim();//开工日期
                    archiveDetail.jungongTime = jungongTime.Trim();//竣工日期
                 if(fazhaoTime == null)
                {
                    fazhaoTime = "";
                }
                    archiveDetail.fazhaoTime = fazhaoTime.Trim();//发照日期
                    archiveDetail.jgDate = DateTime.Parse(jgDate.Trim());
                    string strlururiqi = TyperDate;//录入日期
                    archiveDetail.typerDate = Convert.ToDateTime(strlururiqi);
                    archiveDetail.typist = Typist;//录入员  

                    archiveDetail.isImageExist = "无";
                    if (TuFuhao == "")
                    {
                        archiveDetail.mapsheetNo = "0";
                    }
                    else
                    {
                        archiveDetail.mapsheetNo = TuFuhao;//图幅号
                    }
                    if (SuoWeiHao == "")
                    {
                        archiveDetail.microNo = "0";
                    }
                    else
                    {
                        archiveDetail.microNo = SuoWeiHao;//微缩号
                    }

                
               
                
                    if (ModelState.IsValid)
                    {
                        db.Entry(archiveDetail).State = EntityState.Modified;
                       
                     }

                    if (projects != null)
                    {
                        if (projects.developmentOrganization.Trim() != developmentOrganization.Trim() || projects.constructionOrganization.Trim() != constructionOrganization.Trim() || projects.disignOrganization.Trim() != disignOrganization.Trim())
                        {
                            if (developmentOrganization.Trim() != "")
                                projects.developmentOrganization = developmentOrganization.Trim();
                            if (constructionOrganization.Trim() != "")
                                projects.constructionOrganization = constructionOrganization.Trim();
                            if (disignOrganization.Trim() != "")
                                projects.disignOrganization = disignOrganization.Trim();

                        }
                        projects.securityID = securityName.ToString().Trim();
                        projects.retentionPeriodNo = retentionPeriodName.ToString().Trim();
                        projects.structureTypeID = structureTypeName.ToString().Trim();
                        projects.newLocation = newLocation.Trim();
                        projects.location = location;
                        if (ModelState.IsValid)
                        {
                            db.Entry(projects).State = EntityState.Modified;

                        
                        }
                    }
                    if (paperArchives != null)
                    {
                        string jinguandata = Convert.ToDateTime(paperArchives.jgDate).ToString("yyyy-MM-dd"); 
                        if (jinguandata != jgDate.Trim())
                        {
                            paperArchives.jgDate = DateTime.Parse(jgDate.Trim());
                        }
                    if (buildingArea == null) {
                        buildingArea = "";
                    }
                    if (buildingArea.Trim() != "") {
                        paperArchives.buildingArea = Convert.ToDouble((buildingArea.Trim()));
                    }
                        
                        paperArchives.underground = underground.Trim();
                        paperArchives.overground = overground.Trim();
                        paperArchives.structureTypeID = structureTypeName.ToString().Trim();
                    if (height == null)
                      {
                        height = "";
                      }
                      if (height.Trim() != "")
                      {
                        paperArchives.height = Convert.ToDouble((height.Trim()));
                       }
                       
                        paperArchives.luruTime = TyperDate.Trim();
                        paperArchives.projectStartDate = kaigongTime.Trim();
                        paperArchives.projectFinishDate = jungongTime.Trim();
                        paperArchives.licenseNo = licenseNo.Trim();
                        paperArchives.licenseDate = checkDate;
                       if (changeLog==0)
                      {
                        paperArchives.changeLog = "没有";
                      }
                      if (changeLog == 1)
                      {
                        paperArchives.changeLog = "有";
                   

                         paperArchives.transferUnit = transferUnit;
                        if (TextMaterial.Trim() != "")
                        {
                            paperArchives.textMaterial = long.Parse(TextMaterial);
                        }

                            if (paperArchives.textMaterial != 0)
                            {

                                paperArchives.textMaterial = paperArchives.textMaterial + Int32.Parse(TextMaterial.Trim());
                            }
                            else
                            {
                                paperArchives.textMaterial = Int32.Parse(TextMaterial.Trim());
                            }
                        }
                        if (TextMaterial.Trim() != "")
                        {
                            if (paperArchives.drawing != 0)
                            {
                                paperArchives.drawing = paperArchives.drawing + Int32.Parse(TextMaterial.Trim());
                            }
                            else
                            {
                                paperArchives.drawing = Int32.Parse(TextMaterial.Trim());
                            }
                        }
                        if (PhotoCount.Trim() != "")
                        {
                            if (paperArchives.PhotoCount != 0)
                            {
                                paperArchives.PhotoCount = paperArchives.PhotoCount + Int32.Parse(PhotoCount.Trim());
                            }
                            else
                            {
                                paperArchives.PhotoCount = Int32.Parse(PhotoCount.Trim());
                            }
                        }
                        paperArchives.firstResponsible = firstResponsible.Trim();
                        paperArchives.responsibleOther = responsibleOther.Trim();


                        string shizhengno = shizhengNo.Trim();
                        int volCount = 0;
                        int.TryParse(paperArchives.archivesCount, out volCount);
                        int vol = Convert.ToInt32(archiveDetail.volNo);

                        if (vol == 1)
                        {
                            paperArchives.shizhengNoStart = shizhengno;
                        }
                        if (vol == volCount)
                        {
                            paperArchives.shizhengNoEnd = shizhengno;
                        }
                        if (ModelState.IsValid)
                        {
                            db.Entry(paperArchives).State = EntityState.Modified;
                          
                          
                         }
                  }
                db.SaveChanges();
                ViewData["button5"] = true;
                ViewData["checkname"] = 3;

                //return RedirectToAction("EnterAndSee", "ArchivesEnter", new {id= RegistrationNo .Trim()});
                //return Content("<script >alert('保存成功！');window.history.back();</script >");
                return Content("<script >alert('保存成功！');window.location.href='/ArchivesEnter/EnterAndSee/?id=" + RegistrationNo.Trim() + "';</script >");
            }





                if (action == "后一卷")
              {
                string regis = RegistrationNo;
                long re = Int32.Parse(regis);
                int index=regis.Substring(0,1).IndexOf('0');
                re = re+1;
                string regisNo = "";
                if (index== -1)
                {
                    regisNo = re.ToString();
                }
                else
                {
                    regisNo = "0" + re.ToString();
                }
                if (regisNo.ToString()==endregis)
                {
                    var pro = from a in proje
                              where a.paperProjectSeqNo== paperProjectSeqNo
                              orderby a.registrationNo descending
                              select a.registrationNo;
                    string startregisNo = pro.First();
                    return RedirectToAction("EnterAndSee", new { id = startregisNo });
                }
                else
                {
                    var pro = from a in proje
                              where a.paperProjectSeqNo == paperProjectSeqNo && a.registrationNo == regisNo.ToString()
                              select a.registrationNo;
                    string startregisNo = pro.First();
                    return RedirectToAction("EnterAndSee", new { id = startregisNo });
                }

            }
            if (action == "末卷")
            {
                var pro = from a in proje
                          where a.paperProjectSeqNo == paperProjectSeqNo
                          orderby a.registrationNo descending
                          select a.registrationNo;
                string startregisNo = pro.First();
                ViewBag.jiaojiemulu = "display:inline-block";
                ViewBag.jiaojiemulu = "display:inline-block";
                return RedirectToAction("EnterAndSee", new { id = startregisNo });
            }
           


            return View(proje.First());
        }
        public string PreAndNex(string name, string RegistrationNo, string paperno, string volNo)
        {
            long paper = long.Parse(paperno);
            var PaperNo = from a in db.ArchivesDetail
                          where a.paperProjectSeqNo == paper
                          orderby a.registrationNo
                          select a;
            string stratregis = PaperNo.First().registrationNo;
            var PaperNo1 = from a in db.ArchivesDetail
                           where a.paperProjectSeqNo == paper
                           orderby a.registrationNo descending
                           select a;
            string endregis = PaperNo1.First().registrationNo;
            DataTable myTable = null;
            myTable = new DataTable();

            DataRow myDataRow;

            myTable.Columns.Add("Registion", Type.GetType("System.String"));

            myTable.Columns.Add("ArchiveNo", Type.GetType("System.String"));
            myTable.Columns.Add("volno", Type.GetType("System.String"));
            myTable.Columns.Add("flag", Type.GetType("System.String"));
            myDataRow = myTable.NewRow();
            string flag = "";//flag用于判断页码状态，1：正常；2：第一页；3：最后一页
            string startregisNo = "", achiveno = "", volno = "";
            if (name == "pre")
            {
                string regis = RegistrationNo;
                long re = Int32.Parse(regis);
                int index = regis.IndexOf('0');
                re = re - 1;
                string regisNo = "";
                if (index == -1)
                {
                    regisNo = re.ToString();
                }
                else
                {
                    regisNo = "0" + re.ToString();
                }

                if (regisNo.ToString() == stratregis)
                {
                    var pro = from a in db.vw_passList
                              where a.paperProjectSeqNo == paper
                              orderby a.registrationNo
                              select a;
                    startregisNo = pro.First().registrationNo;
                    achiveno = pro.First().archivesNo;
                    volno = pro.First().volNo.ToString();
                    flag = "2";


                }
                else
                {
                    var pro = from a in db.vw_passList
                              where a.paperProjectSeqNo == paper && a.registrationNo == regisNo.ToString()
                              select a;
                    startregisNo = regisNo.ToString();
                    achiveno = pro.First().archivesNo;
                    volno = pro.First().volNo.ToString();
                    flag = "1";

                }
            }
            if (name == "nex")
            {
                string regis = RegistrationNo;
                long re = Int32.Parse(regis);
                int index = regis.IndexOf('0');
                re = re + 1;
                string regisNo = "";
                if (index == -1)
                {
                    regisNo = re.ToString();
                }
                else
                {
                    regisNo = "0" + re.ToString();
                }
                if (regisNo.ToString() == endregis)
                {
                    var pro = from a in db.vw_passList
                              where a.paperProjectSeqNo == paper
                              orderby a.registrationNo descending
                              select a;
                    startregisNo = regisNo.ToString();
                    achiveno = pro.First().archivesNo;
                    volno = pro.First().volNo.ToString();
                    flag = "3";

                }
                else
                {
                    var pro = from a in db.vw_passList
                              where a.paperProjectSeqNo == paper && a.registrationNo == regisNo.ToString()
                              select a;
                    startregisNo = regisNo.ToString();
                    achiveno = pro.First().archivesNo;
                    volno = pro.First().volNo.ToString();
                    flag = "1";

                }

            }



            myDataRow["Registion"] = startregisNo;
            myDataRow["ArchiveNo"] = achiveno;
            myDataRow["volno"] = volno;
            myDataRow["flag"] = flag;
            myTable.Rows.Add(myDataRow);

            return JsonConvert.SerializeObject(myTable);
        }
        //public ActionResult FileList(string id1,int id2,int id)
        //{

        //    if (id1 == "" || id1 == null)
        //    {
        //        return Content("<script >alert('该案卷档号为空，请核查！');window.history.back();</script >");
        //    }

        //    List<SelectListItem> list = new List<SelectListItem> {
        //        new SelectListItem { Text = "文字", Value = "文字"},
        //        new SelectListItem { Text = "图纸", Value = "图纸"},
        //        new SelectListItem { Text = "文字及图纸", Value = "文字及图纸" },
        //        };
        //    ViewBag.SelectedID = new SelectList(list, "Value", "Text");
        //    ViewData["juanniemulu"] = true;
        //    var file3 = from a in db.FileInfo
        //               where a.archivesNo == id1.Trim()
        //               orderby a.seqNo
        //               select a;
        //    var registion = from g in db.vw_passList
        //                    where g.archivesNo == id1.Trim()
        //                    select g.registrationNo;
        //      ViewData["ArchiveNo"] = id1.Trim();
        //    if (file3.Count() == 0)
        //    {
        //        ViewData["div"] = "display:block";
        //    }
        //    else
        //    {
        //        ViewData["div"] = "display:none";
        //        ViewData["juanniemulu"] = false;
        //    }
        //    ViewBag.Edit = "display:none";
        //    ViewBag.add = "display:none";
        //    if (id2 == 1)
        //    {
        //        string str3 = id1.Trim();



        //        var file = from a in db.FileInfo
        //                   where a.archivesNo == str3
        //                   orderby a.seqNo descending
        //                   select a;

        //        var file1 = from b in db.FileInfo
        //                    where b.archivesNo == str3
        //                    orderby b.endPageNo descending
        //                    select b;
        //        FileInfo file2 = new FileInfo();
        //        if (file.Count() == 0)
        //        {
        //            file2.seqNo = 1;
        //            file2.startPageNo = "1";
        //        }
        //        else
        //        {
        //            file2.seqNo = file.First().seqNo + 1;
        //            file2.startPageNo = (file.First().endPageNo + 1).ToString();
        //            List<SelectListItem> list2 = new List<SelectListItem> {
        //        new SelectListItem { Text = "文字", Value = "文字"},
        //        new SelectListItem { Text = "图纸", Value = "图纸"},
        //        new SelectListItem { Text = "文字及图纸", Value = "文字及图纸" },
        //        };
        //            ViewBag.SelectedID = new SelectList(list2, "Value", "Text", file.First().type.Trim());
        //            //int index = file1.First().startPageNo.ToString().IndexOf('-');
        //            //if (index == -1)
        //            //{
        //            //    file2.startPageNo = (file1.First().endPageNo + 1).ToString().Trim();
        //            //}
        //            //else
        //            //{
        //            //    string str1 = file1.First().endPageNo.ToString().Substring(index + 1, file2.startPageNo.Length - index - 1);

        //            //    int str5 = Int32.Parse(str1.Trim()) + 1;

        //            //    file2.startPageNo = str5.ToString().Trim();
        //            //}
        //        }

        //        ViewBag.startPageNo = file2.startPageNo;
        //        ViewBag.seqNo = file2.seqNo;
        //        ViewBag.Edit = "display:none";
        //        ViewBag.add = "display:block";
        //        ViewBag.id = db.FileInfo.Max(a => a.id) + 1;
        //        if (file.Count() == 0)
        //        {
        //            ViewBag.fileName = "";
        //            ViewBag.responsible = "";
        //        }
        //        else
        //        {
        //            //ViewBag.fileName = file.First().fileName;
        //            //ViewBag.responsible = file.First().responsible;
        //            string name=file.First().fileName;
        //            string res = file.First().responsible;
        //            if (name == ""&& res != "")
        //            {
        //                ViewBag.filenameid = 0;

        //                var response = db.WordTable.Where(a => a.character == 2).Where(a => a.wordName.Trim() == res);
        //                if (response.Count() == 0)
        //                {
        //                    ViewBag.responsibleid = 0;
        //                }
        //                else
        //                {
        //                    ViewBag.responsibleid = response.First().id;
        //                }
        //            }
        //           if (res == ""&& name != "")
        //            {
        //                ViewBag.responsibleid = 0;
        //                var filename = db.WordTable.Where(a => a.character == 1).Where(a => a.wordName.Trim() == name);
        //                if (filename.Count() == 0)
        //                {
        //                    ViewBag.filenameid = 0;
        //                }
        //                else
        //                {
        //                    ViewBag.filenameid = filename.First().id;
        //                }
        //            }
        //            if (res != "" && name != "")
        //            {
        //                var filename = db.WordTable.Where(a => a.character == 1).Where(a => a.wordName.Trim() == name);
        //                if (filename.Count() == 0)
        //                {
        //                    ViewBag.filenameid = 0;
        //                }
        //                else
        //                {
        //                    ViewBag.filenameid = filename.First().id;
        //                }
        //                var response = db.WordTable.Where(a => a.character == 2).Where(a => a.wordName.Trim() == res);
        //                if (response.Count() == 0)
        //                {
        //                    ViewBag.responsibleid = 0;
        //                }
        //                else
        //                {
        //                    ViewBag.responsibleid = response.First().id;
        //                }
        //            }
        //            if (res == "" && name == "")
        //            {
        //                ViewBag.filenameid = 0;
        //                ViewBag.responsibleid = 0;
        //            }

        //        }

        //    }
        //    if (id2 == 2)
        //    {
        //        ViewBag.Edit = "display:block";
        //        ViewBag.add = "display:none";

        //        var file = from ad in db.FileInfo
        //                   where (ad.id == id)
        //                   select ad;
        //        string a = file.First().archivesNo;
        //        var file2 = from ac in db.FileInfo
        //                    where ac.archivesNo == a
        //                    orderby ac.seqNo
        //                    select ac;
        //        var f1 = file2.First();
        //        int seq1 = Convert.ToInt32(f1.seqNo);
        //        var file4= from ae in db.FileInfo
        //                    where ae.archivesNo == a
        //                    orderby ae.seqNo descending
        //                    select ae;
        //        int seq2 = Convert.ToInt32(file4.First().seqNo);
        //        if(seq2==1)
        //        {
        //            ViewData["button1"] = true;
        //            ViewData["button2"] = true;
        //        }
        //        if (file.First().seqNo == seq1)
        //        {
        //            ViewData["button1"] = true;
        //        }
        //        if (file.First().seqNo == seq2)
        //        {
        //            ViewData["button2"] = true;
        //        }
        //        ViewData["startPageNo"] = file.First().startPageNo;
        //        //ViewData["endPageNo"] = file.First().endPageNo;
        //        ViewData["startDate"] = file.First().startDate;
        //        //ViewData["endDate"] = file.First().endDate;
        //        List<SelectListItem> list1 = new List<SelectListItem> {
        //        new SelectListItem { Text = "文字", Value = "文字"},
        //        new SelectListItem { Text = "图纸", Value = "图纸"},
        //        new SelectListItem { Text = "文字及图纸", Value = "文字及图纸" },
        //        };
        //        ViewBag.SelectedID = new SelectList(list1, "Value", "Text", file.First().type.Trim());
        //        ViewBag.fileName = file.First().fileName;
        //        ViewBag.responsible = file.First().responsible;
        //        ViewBag.startDate = file.First().startDate;
        //        ViewBag.remarks = file.First().remarks;
        //        ViewBag.startPageNo = file.First().startPageNo;
        //        ViewBag.seqNo = file.First().seqNo;
        //        ViewBag.fileNo = file.First().fileNo;
        //        ViewBag.id = file.First().id;
        //        string name = file.First().fileName;
        //        string res = file.First().responsible;
        //        if (name == "" && res != "")
        //        {
        //            ViewBag.filenameid1 = 0;

        //            var response = db.WordTable.Where(b => b.character == 2).Where(b => b.wordName.Trim() == res);
        //            if (response.Count() == 0)
        //            {
        //                ViewBag.responsibleid1 = 0;
        //            }
        //            else
        //            {
        //                ViewBag.responsibleid1 = response.First().id;
        //            }
        //        }
        //        if (res == "" && name != "")
        //        {
        //            ViewBag.responsibleid1 = 0;
        //            var filename = db.WordTable.Where(b => b.character == 1).Where(b => b.wordName.Trim() == name);
        //            if (filename.Count() == 0)
        //            {
        //                ViewBag.filenameid1 = 0;
        //            }
        //            else
        //            {
        //                ViewBag.filenameid1 = filename.First().id;
        //            }
        //        }
        //        if (res != "" && name != "")
        //        {
        //            var filename = db.WordTable.Where(b => b.character == 1).Where(b => b.wordName.Trim() == name);
        //            if (filename.Count() == 0)
        //            {
        //                ViewBag.filenameid1 = 0;
        //            }
        //            else
        //            {
        //                ViewBag.filenameid1 = filename.First().id;
        //            }
        //            var response = db.WordTable.Where(b => b.character == 2).Where(b => b.wordName.Trim() == res);
        //            if (response.Count() == 0)
        //            {
        //                ViewBag.responsibleid1 = 0;
        //            }
        //            else
        //            {
        //                ViewBag.responsibleid1 = response.First().id;
        //            }
        //        }
        //        if (res == "" && name == "")
        //        {
        //            ViewBag.filenameid1 = 0;
        //            ViewBag.responsibleid1 = 0;
        //        }


        //    }

        //    //file2.archivesNo = str3;

        //    //return View(file.ToList());
        //    ViewData["registion"] = registion.First();
        //    ViewBag.result = JsonConvert.SerializeObject(file3.ToList());
        //    return View();


        //}
        public ActionResult FileList(string id1, int id2, int id)
        {
            ViewData["lvming"] = "none";
            if (id1 == "" || id1 == null)
            {
                return Content("<script >alert('该案卷档号为空，请核查！');window.history.back();</script >");
            }

            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "文字", Value = "文字"},
                new SelectListItem { Text = "图纸", Value = "图纸"},
                new SelectListItem { Text = "文字及图纸", Value = "文字及图纸" },
                };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            ViewData["juanniemulu"] = true;
            var file3 = from a in db.FileInfo
                        where a.archivesNo == id1.Trim()
                        orderby a.seqNo
                        select a;
            var registion = from g in db.vw_passList
                            where g.archivesNo == id1.Trim()
                            select g.registrationNo;
            ViewData["ArchiveNo"] = id1.Trim();
            if (file3.Count() == 0)
            {
                ViewData["div"] = "display:block";
            }
            else
            {
                ViewData["div"] = "display:none";
                ViewData["juanniemulu"] = false;
            }
            ViewBag.Edit = "display:none";
            ViewBag.add = "display:none";
            if (id2 == 1)
            {
                ViewData["lvming"] = "inline-block";
                string str3 = id1.Trim();



                var file = from a in db.FileInfo
                           where a.archivesNo == str3
                           orderby a.seqNo descending
                           select a;

                var file1 = from b in db.FileInfo
                            where b.archivesNo == str3
                            orderby b.endPageNo descending
                            select b;
                FileInfo file2 = new FileInfo();
                if (file.Count() == 0)
                {
                    file2.seqNo = 1;
                    file2.startPageNo = "1";
                }
                else
                {
                    file2.seqNo = file.First().seqNo + 1;
                    file2.startPageNo = (file.First().endPageNo + 1).ToString();
                    List<SelectListItem> list2 = new List<SelectListItem> {
                      new SelectListItem { Text = "文字", Value = "文字"},
                      new SelectListItem { Text = "图纸", Value = "图纸"},
                      new SelectListItem { Text = "文字及图纸", Value = "文字及图纸" },
                    };
                    string choice = file.First().type;
                    if (choice == "" || choice == null) {
                        ViewBag.SelectedID = new SelectList(list2, "Value", "Text", "文字");
                    }
                    else {
                        ViewBag.SelectedID = new SelectList(list2, "Value", "Text", choice.Trim());
                    }
                    
                    //int index = file1.First().startPageNo.ToString().IndexOf('-');
                    //if (index == -1)
                    //{
                    //    file2.startPageNo = (file1.First().endPageNo + 1).ToString().Trim();
                    //}
                    //else
                    //{
                    //    string str1 = file1.First().endPageNo.ToString().Substring(index + 1, file2.startPageNo.Length - index - 1);

                    //    int str5 = Int32.Parse(str1.Trim()) + 1;

                    //    file2.startPageNo = str5.ToString().Trim();
                    //}
                }

                ViewBag.startPageNo = file2.startPageNo;
                ViewBag.seqNo = file2.seqNo;
                ViewBag.Edit = "display:none";
                ViewBag.add = "display:block";
                ViewBag.id = db.FileInfo.Max(a => a.id) + 1;
                if (file.Count() == 0)
                {
                    ViewBag.fileName = "";
                    ViewBag.responsible = "";
                }
                else
                {
                    //ViewBag.fileName = file.First().fileName;
                    //ViewBag.responsible = file.First().responsible;
                    string name = file.First().fileName;
                    string res = file.First().responsible;
                    if (name == "" && res != "")
                    {
                        ViewBag.filenameid = 0;

                        var response = db.WordTable.Where(a => a.character == 2).Where(a => a.wordName.Trim() == res);
                        if (response.Count() == 0)
                        {
                            ViewBag.responsibleid = 0;
                        }
                        else
                        {
                            ViewBag.responsibleid = response.First().id;
                        }
                    }
                    if (res == "" && name != "")
                    {
                        ViewBag.responsibleid = 0;
                        var filename = db.WordTable.Where(a => a.character == 1).Where(a => a.wordName.Trim() == name);
                        if (filename.Count() == 0)
                        {
                            ViewBag.filenameid = 0;
                        }
                        else
                        {
                            ViewBag.filenameid = filename.First().id;
                        }
                    }
                    if (res != "" && name != "")
                    {
                        var filename = db.WordTable.Where(a => a.character == 1).Where(a => a.wordName.Trim() == name);
                        if (filename.Count() == 0)
                        {
                            ViewBag.filenameid = 0;
                        }
                        else
                        {
                            ViewBag.filenameid = filename.First().id;
                        }
                        var response = db.WordTable.Where(a => a.character == 2).Where(a => a.wordName.Trim() == res);
                        if (response.Count() == 0)
                        {
                            ViewBag.responsibleid = 0;
                        }
                        else
                        {
                            ViewBag.responsibleid = response.First().id;
                        }
                    }
                    if (res == "" && name == "")
                    {
                        ViewBag.filenameid = 0;
                        ViewBag.responsibleid = 0;
                    }

                }

            }
            if (id2 == 2)
            {
                ViewBag.Edit = "display:block";
                ViewBag.add = "display:none";


                var file = from ad in db.FileInfo
                           where (ad.id == id)
                           select ad;
                string a = file.First().archivesNo;
                var file2 = from ac in db.FileInfo
                            where ac.archivesNo == a
                            orderby ac.seqNo
                            select ac;
                var f1 = file2.First();
                int seq1 = Convert.ToInt32(f1.seqNo);
                var file4 = from ae in db.FileInfo
                            where ae.archivesNo == a
                            orderby ae.seqNo descending
                            select ae;
                int seq2 = Convert.ToInt32(file4.First().seqNo);
                if (seq2 == 1)
                {
                    ViewData["button1"] = true;
                    ViewData["button2"] = true;
                }
                if (file.First().seqNo == seq1)
                {
                    ViewData["button1"] = true;
                }
                if (file.First().seqNo == seq2)
                {
                    ViewData["button2"] = true;
                }
                ViewData["startPageNo"] = file.First().startPageNo;
                //ViewData["endPageNo"] = file.First().endPageNo;
                ViewData["startDate"] = file.First().startDate;
                //ViewData["endDate"] = file.First().endDate;
                List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "文字", Value = "文字"},
                new SelectListItem { Text = "图纸", Value = "图纸"},
                new SelectListItem { Text = "文字及图纸", Value = "文字及图纸" },
                };
                //string n1 = file.First().type.Trim();
                var file11 = file.First();
                ViewBag.SelectedID = new SelectList(list1, "Value", "Text", file.First().type.Trim());
                ViewBag.fileName = file.First().fileName;
                ViewBag.responsible = file.First().responsible;
                ViewBag.startDate = file.First().startDate;
                ViewBag.remarks = file.First().remarks;
                ViewBag.startPageNo = file.First().startPageNo;
                ViewBag.seqNo = file.First().seqNo;
                ViewBag.fileNo = file.First().fileNo;
                ViewBag.id = file.First().id;
                string name = file.First().fileName;
                string res = file.First().responsible;
                if (name == "" && res != "")
                {
                    ViewBag.filenameid1 = 0;

                    var response = db.WordTable.Where(b => b.character == 2).Where(b => b.wordName.Trim() == res);
                    if (response.Count() == 0)
                    {
                        ViewBag.responsibleid1 = 0;
                    }
                    else
                    {
                        ViewBag.responsibleid1 = response.First().id;
                    }
                }
                if (res == "" && name != "")
                {
                    ViewBag.responsibleid1 = 0;
                    var filename = db.WordTable.Where(b => b.character == 1).Where(b => b.wordName.Trim() == name);
                    if (filename.Count() == 0)
                    {
                        ViewBag.filenameid1 = 0;
                    }
                    else
                    {
                        ViewBag.filenameid1 = filename.First().id;
                    }
                }
                if (res != "" && name != "")
                {
                    var filename = db.WordTable.Where(b => b.character == 1).Where(b => b.wordName.Trim() == name);
                    if (filename.Count() == 0)
                    {
                        ViewBag.filenameid1 = 0;
                    }
                    else
                    {
                        ViewBag.filenameid1 = filename.First().id;
                    }
                    var response = db.WordTable.Where(b => b.character == 2).Where(b => b.wordName.Trim() == res);
                    if (response.Count() == 0)
                    {
                        ViewBag.responsibleid1 = 0;
                    }
                    else
                    {
                        ViewBag.responsibleid1 = response.First().id;
                    }
                }
                if (res == "" && name == "")
                {
                    ViewBag.filenameid1 = 0;
                    ViewBag.responsibleid1 = 0;
                }


            }

            //file2.archivesNo = str3;

            //return View(file.ToList());
            ViewData["registion"] = registion.First();
            ViewBag.result = JsonConvert.SerializeObject(file3.ToList());
            return View();


        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult FileList(string ArchiveNo, string action,string registion)
        //{
        //    var file = from a in db.FileInfo
        //               where a.archivesNo == ArchiveNo.Trim()
        //               orderby a.seqNo
        //               select a;
        //    var file1 = from b in db.ArchivesDetail
        //                where b.archivesNo == ArchiveNo.Trim()
        //                select b.paperProjectSeqNo;
        //    string id = Request.Form["id"];
        //    if (action == "添加")
        //    {
        //        return RedirectToAction("FileList", new { id1 = ArchiveNo.Trim(), id = 0, id2 = 1 });
        //    }
        //    if (action == "返回案卷信息")
        //    {
        //         return RedirectToAction("EnterAndSee", new { id =registion.Trim()});
        //    }
        //    if (action == "录入完毕")
        //    {
        //        ViewData["juanniemulu"] = false;
        //        //ViewData["button3"] = true;
        //        return Content("<script >alert('该案卷卷内文件已录入完毕！');window.history.back();</script >");
        //    }
        //    if (action == "打印卷内目录")
        //    {
        //        string ID = ArchiveNo.Trim();
        //        return RedirectToAction("juanneimulu", "ArchivesEnter", new { myid = ID, format = "PDF" });
        //    }
        //    var file2 = from a in db.ArchivesDetail
        //               where a.archivesNo == ArchiveNo
        //               select a.registrationNo;

        //    int index = ArchiveNo.IndexOf('.');
        //    int index1 = index + 1;
        //    string str1 = ArchiveNo.Substring(0, index + 1);
        //    string str2 = ArchiveNo.Substring(index + 1, ArchiveNo.Length - 1 - index);
        //    FileInfo file3 = new FileInfo();

        //    file3.id = db.FileInfo.Max(a => a.id) + 1;
        //    file3.seqNo = int.Parse(Request.Form["seqNo"]);
        //    //if (Request.Form["SelectedID"]!= null && Request.Form["SelectedID"] != "")
        //    //{
        //    //    switch (Request.Form["SelectedID"])
        //    //    {
        //    //        case "0":
        //    //            file3.type="文字";
        //    //            break;
        //    //        case "1":
        //    //            file3.type="图纸";
        //    //            break;
        //    //        case "2":
        //    //            file3.type="文字及图纸";
        //    //            break;


        //    //    }

        //    //}
        //    file3.type = Request.Form["SelectedID"];
        //    file3.fileNo = Request.Form["fileNo"];
        //    file3.fileName = Request.Form["fileName"];
        //    file3.responsible = Request.Form["responsible"];
        //    file3.startPageNo = Request.Form["startPageNo"];
        //    file3.startDate = Request.Form["startDate"];
        //    file3.remarks = Request.Form["remarks"];
        //    file3.archivesNo = ArchiveNo;
        //    file3.dengjihao = file2.First();
        //    if (action == "确定")
        //    {
        //        string page= Request.Form["startPageNo"];
        //        var no = page.IndexOf('-');
        //        if (page.IndexOf('-') != -1)
        //        {
        //            file3.endPageNo = int.Parse(page.Split('-').Last());
        //        }
        //        else {
        //            file3.endPageNo = int.Parse(page);
        //        }
        //        if (ModelState.IsValid)
        //        {
        //            db.FileInfo.Add(file3);
        //            db.SaveChanges();
        //            return Content("<script >alert('添加成功！');window.location.href='/ArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + id + "&id2=1" + "';</script >");
        //        }
        //    }
        //    if (action == "删除词条")
        //    {
        //        string temp = Request.Form["DELETE_ID"];
        //        var id1 = int.Parse(Request.Form["DELETE_ID"].Split('-').First());
        //        WordTable wordtable = db.WordTable.Find(id1);
        //        db.WordTable.Remove(wordtable);
        //        var list1 = db.WordTable.Where(ad => ad.newid > wordtable.newid).OrderBy(ad => ad.newid);
        //        foreach (var i in list1)
        //        {
        //            i.newid -= 1;
        //            db.Entry(i).State = EntityState.Modified;
        //        }
        //        db.SaveChanges();
        //        return RedirectToAction("FileList", new { id1 = ArchiveNo.Trim(), id = id, id2 = 1 });
        //        //return Content("<script >alert('删除成功！');window.location.href='/ArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + id + "&id2=1" + "';</script >");
        //    }
        //    if (action == "取消")
        //    {
        //        return RedirectToAction("FileList", new { id1 = ArchiveNo.Trim(), id = 0, id2 = 0 });
        //    }

        //    if (file == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View();




        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FileList(string ArchiveNo, string action, string registion)
        {
            var file = from a in db.FileInfo
                       where a.archivesNo == ArchiveNo.Trim()
                       orderby a.seqNo
                       select a;
            var file1 = from b in db.ArchivesDetail
                        where b.archivesNo == ArchiveNo.Trim()
                        select b.paperProjectSeqNo;
            string id = Request.Form["id"];
            if (action == "添加")
            {

                return RedirectToAction("FileList", new { id1 = ArchiveNo.Trim(), id = 0, id2 = 1 });
            }
            if (action == "返回案卷信息")
            {
                return RedirectToAction("EnterAndSee", new { id = registion.Trim() });
            }
            if (action == "录入完毕")
            {
                ViewData["juanniemulu"] = false;
                //ViewData["button3"] = true;
                return Content("<script >alert('该案卷卷内文件已录入完毕！');window.history.back();</script >");
            }
            if (action == "打印卷内目录")
            {
                string ID = ArchiveNo.Trim();
                return RedirectToAction("juanneimulu", "ArchivesEnter", new { myid = ID, format = "PDF" });
            }
            var file2 = from a in db.ArchivesDetail
                        where a.archivesNo == ArchiveNo
                        select a.registrationNo;

            int index = ArchiveNo.IndexOf('.');
            int index1 = index + 1;
            string str1 = ArchiveNo.Substring(0, index + 1);
            string str2 = ArchiveNo.Substring(index + 1, ArchiveNo.Length - 1 - index);
            FileInfo file3 = new FileInfo();

            file3.id = db.FileInfo.Max(a => a.id) + 1;
            file3.seqNo = int.Parse(Request.Form["seqNo"]);
            //if (Request.Form["SelectedID"]!= null && Request.Form["SelectedID"] != "")
            //{
            //    switch (Request.Form["SelectedID"])
            //    {
            //        case "0":
            //            file3.type="文字";
            //            break;
            //        case "1":
            //            file3.type="图纸";
            //            break;
            //        case "2":
            //            file3.type="文字及图纸";
            //            break;


            //    }

            //}
            file3.type = Request.Form["SelectedID"];
            file3.fileNo = Request.Form["fileNo"];
            file3.fileName = Request.Form["fileName"];
            file3.responsible = Request.Form["responsible"];
            file3.startPageNo = Request.Form["startPageNo"];
            file3.startDate = Request.Form["startDate"];
            file3.remarks = Request.Form["remarks"];
            file3.archivesNo = ArchiveNo;
            file3.dengjihao = file2.First();
            if (action == "确定")
            {
                string page = Request.Form["startPageNo"];
                var no = page.IndexOf('-');
                if (page.IndexOf('-') != -1)
                {
                    file3.endPageNo = int.Parse(page.Split('-').Last());
                }
                else
                {
                    file3.endPageNo = int.Parse(page);
                }
                if (ModelState.IsValid)
                {
                    db.FileInfo.Add(file3);
                    db.SaveChanges();
                    return Content("<script >alert('添加成功！');window.location.href='/ArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + id + "&id2=1" + "';</script >");
                }
            }
            if (action == "删除词条")
            {
                string temp = Request.Form["DELETE_ID"];
                var id1 = int.Parse(Request.Form["DELETE_ID"].Split('-').First());
                WordTable wordtable = db.WordTable.Find(id1);
                db.WordTable.Remove(wordtable);
                var list1 = db.WordTable.Where(ad => ad.newid > wordtable.newid).OrderBy(ad => ad.newid);
                foreach (var i in list1)
                {
                    i.newid -= 1;
                    db.Entry(i).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("FileList", new { id1 = ArchiveNo.Trim(), id = id, id2 = 1 });
                //return Content("<script >alert('删除成功！');window.location.href='/ArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + id + "&id2=1" + "';</script >");
            }
            if (action == "取消")
            {
                return RedirectToAction("FileList", new { id1 = ArchiveNo.Trim(), id = 0, id2 = 0 });
            }

            if (file == null)
            {
                return HttpNotFound();
            }

            //导入导出excel  吕鸣 2018
            if (action == "导出为Excel")
            {
                DataSet ds = new DataSet();
                SqlConnection con = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
                SqlDataAdapter da = new SqlDataAdapter("Select seqNo,type,fileNo,fileName,responsible,startPageNo,startDate,remarks From FileInfo where id = " + id, con);
                da.Fill(ds, "FileInfo");
                DataTable dt = ds.Tables["FileInfo"];
                dt.Columns["seqNo"].ColumnName = "序号";
                dt.Columns["type"].ColumnName = "类型";
                dt.Columns["fileNo"].ColumnName = "文件编号";
                dt.Columns["responsible"].ColumnName = "责任者";
                dt.Columns["fileName"].ColumnName = "文件题名";
                dt.Columns["startDate"].ColumnName = "日期";
                dt.Columns["startPageNo"].ColumnName = "页次";
                dt.Columns["remarks"].ColumnName = "备注";

                System.IO.StringWriter sw = new System.IO.StringWriter();
                GridView dv = new GridView();
                dv.DataSource = dt;
                dv.DataBind();
                dv.AllowPaging = false;

                Response.ClearContent();
                Response.Charset = "GB2312";
                Response.AppendHeader("Content-Disposition", "attachment;filename=anjuan.xls");
                // 如果设置为 GetEncoding("GB2312");导出的文件将会出现乱码！！！
                //Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.ContentEncoding = System.Text.Encoding.Default;
                Response.ContentType = "application/excel";

                HtmlTextWriter htw = new HtmlTextWriter(sw);

                dv.RenderControl(htw);

                Response.Write(sw.ToString());
                Response.End();
                return RedirectToAction("FileList", new { id1 = ArchiveNo.Trim(), id2 = 0, id = id });
            }
            //导入Excel
            if (action == "导入Excel")
            {
                HttpPostedFileBase file6 = Request.Files["FileUpload"];//获取上传的文件
                string FileName;
                string savePath;
                if (file6 == null || file6.ContentLength <= 0)
                {
                    ViewBag.error = "文件不能为空";
                    return View();
                }
                else
                {
                    string filename = System.IO.Path.GetFileName(file6.FileName);
                    int filesize = file6.ContentLength;//获取上传文件的大小单位为字节byte
                    string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名
                    string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名
                    int Maxsize = 10000 * 1024;//定义上传文件的最大空间大小为10M
                    string FileType = ".xls,.xlsx";//定义上传文件的类型字符串
                    FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;
                    if (!FileType.Contains(fileEx))
                    {
                        ViewBag.error = "文件类型不对，只能导入xls和xlsx格式的文件";
                        return View();
                    }
                    if (filesize >= Maxsize)
                    {
                        ViewBag.error = "上传文件超过10M，不能上传";
                        return View();
                    }
                    string path = AppDomain.CurrentDomain.BaseDirectory + "uploads\\excel\\";
                    savePath = System.IO.Path.Combine(path, FileName);
                    file6.SaveAs(savePath);
                }
                string result = string.Empty;
                string strConn;
                //strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + savePath + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";
                //strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + savePath + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'"; //此连接可以操作.xls与.xlsx文件 (支持Excel2003 和 Excel2007 的连接字符串)
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + savePath + "; " + "Extended Properties = 'Excel 8.0;HDR=Yes;IMEX=1'";
                //strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + savePath + ";" + "Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";
                //strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + savePath + ";" + "Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';";
                //备注： "HDR=yes;"是说Excel文件的第一行是列名而不是数据，"HDR=No;"正好与前面的相反。
                //      "IMEX=1 "如果列中的数据类型不一致，使用"IMEX=1"可必免数据类型冲突。
                DataSet myDataSet = new DataSet();
                try
                {
                    OleDbConnection conn = new OleDbConnection(strConn);
                    conn.Open();
                    //返回Excel的架构，包括各个sheet表的名称,类型，创建时间和修改时间等　
                    DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
                    //包含excel中表名的字符串数组
                    string[] strTableNames = new string[dtSheetName.Rows.Count];
                    for (int k = 0; k < dtSheetName.Rows.Count; k++)
                    {
                        strTableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
                    }
                    OleDbDataAdapter myCommand = null;
                    DataTable dt = new DataTable();
                    //从指定的表明查询数据,可

                    //先把所有表明列出来供用户选择
                    string strExcel = "select*from [" + strTableNames[0] + "]";
                    myCommand = new OleDbDataAdapter(strExcel, strConn);
                    myCommand.Fill(myDataSet, "ExcelInfo");
                }
                catch (Exception ex)
                {
                    ViewBag.error = ex.Message;
                    return View();
                }
                DataTable table = myDataSet.Tables["ExcelInfo"].DefaultView.ToTable();
                //循环  2018/9/6  吕鸣
                int i = 0;

                for (i = 0; i < table.Rows.Count; i++)
                {
                    string SeqNo = table.Rows[i][0].ToString().Trim();
                    int seq;
                    if (SeqNo == "")
                    {
                        break;
                        //seq = 0;
                    }
                    else
                    {
                        try
                        {
                            seq = Convert.ToInt32(SeqNo);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('序号应是数字！');window.history.back();</script >");
                        }

                    }
                    string Type = table.Rows[i][1].ToString().Trim();
                    if (Type == "")
                    {
                        Type = "";
                    }
                    else if (Type == "文字" || Type == "图纸" || Type == "文字及图纸")
                    {
                    }
                    else
                    {
                        return Content("<script >alert('类型输入有错！（文字/图纸/文字及图纸）');window.history.back();</script >");
                    }
                    string FileNo = table.Rows[i][2].ToString().Trim();
                    int member;
                    if (FileNo == "")
                    {
                        FileNo = "";
                    }
                    //else
                    //{
                    //    try
                    //    {
                    //        member = Convert.ToInt32(FileNo);

                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        return Content("<script >alert('文件编号有错！');window.history.back();</script >");
                    //    }

                    //}
                    string FileTitle = table.Rows[i][3].ToString().Trim();
                    if (FileTitle == "")
                    {
                        FileTitle = "";
                    }
                    string Responsible = table.Rows[i][4].ToString().Trim();
                    if (Responsible == "")
                    {
                        Responsible = "";
                    }

                    string StartDate = table.Rows[i][5].ToString().Trim();
                    if (StartDate == "")
                    {
                        StartDate = "";
                    }
                    string StartPage = table.Rows[i][6].ToString().Trim();
                    if (StartPage == "")
                    {
                        StartPage = "";
                    }
                    string StartPage1 = table.Rows[i][6].ToString().Trim();

                    string Remarks = table.Rows[i][7].ToString().Trim();
                    if (Remarks == "")
                    {
                        Remarks = "";
                    }
                    int a = int.Parse(id) + i;
                    var num = from ad in db.FileInfo
                              where ad.id == a
                              select ad;/*1*/
                    string strSql1;
                    //if (num.Count() != 0)
                    //{
                    //    strSql1 = string.Format("update UrbanCon.dbo.FileInfo set seqNo=" + seq + ",type='" + Type + "',fileNo=" + FileNo + ",fileName='" + FileTitle + "',responsible='" + Responsible + "',startPageNo=" + StartPage + ",startDate='" + StartDate + "',remarks=" + Remarks + ",dengjihao= " + file2.First() + ",archivesNo=' " + ArchiveNo + "' where id = " + id + i);
                    //}
                    //else
                    //{
                    //    strSql1 = string.Format("insert into UrbanCon.dbo.FileInfo(seqNo, type, fileNo,fileName, responsible, startPageNo, startDate, remarks,dengjihao,archivesNo) values(" + seq + ",'" + Type + "','" + FileNo + "','" + FileTitle + "','" + Responsible + "','" + StartPage + "','" + StartDate + "'," + Remarks + ",'" + file2.First() + "','" + ArchiveNo + "')");
                    //}
                    //SqlConnection sqlConnection = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
                    //try
                    if (num.Count() != 0)
                    {
                        strSql1 = string.Format("update UrbanCon.dbo.FileInfo set seqNo=" + seq + ",type='" + Type + "',fileNo='" + FileNo + "',fileName='" + FileTitle + "',responsible='" + Responsible + "',startPageNo='" + StartPage + "',startDate='" + StartDate + "',remarks='" + Remarks + "',dengjihao='" + file2.First() + "',archivesNo='" + ArchiveNo + "' where id = " + id + i);
                    }
                    else
                    {
                        strSql1 = string.Format("insert into UrbanCon.dbo.FileInfo(seqNo, type, fileNo,fileName, responsible, startPageNo, startDate, remarks,dengjihao,archivesNo) values(" + seq + ",'" + Type + "','" + FileNo + "','" + FileTitle + "','" + Responsible + "','" + StartPage + "','" + StartDate + "','" + Remarks + "','" + file2.First() + "','" + ArchiveNo + "')");
                    }
                    SqlConnection sqlConnection = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
                    try
                    {
                        sqlConnection.Open();
                        SqlCommand sqlCmd = new SqlCommand();
                        sqlCmd.CommandText = strSql1;
                        sqlCmd.Connection = sqlConnection;
                        SqlDataReader sqlDataReader1 = sqlCmd.ExecuteReader();
                        sqlDataReader1.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
                return RedirectToAction("FileList", new { id1 = ArchiveNo.Trim(), id2 = 0, id = id });
            }
            return View();


        }
        public ActionResult Edit(long ?id,string archivesNo)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var file = from ad in db.FileInfo
                       where (ad.id == id)                     
                       select ad;
            string a = file.First().archivesNo;
            var file2 = from ac in db.FileInfo
                        where ac.archivesNo == a
                        orderby ac.seqNo
                        select ac;
            var f1 = file2.First();
            int seq1 = Convert.ToInt32(f1.seqNo);
            var file4 = from ae in db.FileInfo
                        where ae.archivesNo == a
                        orderby ae.seqNo descending
                        select ae;
            int seq2 = Convert.ToInt32(file4.First().seqNo);
         
            if(seq2==1)
            {
                ViewData["button1"] = true;
                ViewData["button2"] = true;
            }
            if (file.First().seqNo == seq1)
            {
                ViewData["button1"] = true;
            }
            if (file.First().seqNo == seq2)
            {
                ViewData["button2"] = true;
            }
            ViewData["startPageNo"] = file.First().startPageNo;
            //ViewData["endPageNo"] = file.First().endPageNo;
            ViewData["startDate"] = file.First().startDate;
            //ViewData["endDate"] = file.First().endDate;
            ViewBag.fileName= file.First().fileName;
            ViewBag.responsible = file.First().responsible;
            ViewData["archivesNo"] = archivesNo;

            if (file == null)
            {
                return HttpNotFound();
            }
            
                return View(file.First());
       }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id,int seqNo,string type,string fileNo,string fileName1,string responsible1,string startPageNo,string startDate,string remarks,string action,string archivesNo)
     {
        
        var file = from ad in db.FileInfo
                   where ad.id == id
                   select ad;
            ViewData["startPageNo"] = file.First().startPageNo;
            //ViewData["endPageNo"] = file.First().endPageNo;
            ViewData["startDate"] = file.First().startDate;
            //ViewData["endDate"] = file.First().endDate;
            ViewBag.fileName = file.First().fileName;
            ViewBag.responsible = file.First().responsible;
            FileInfo fileinfo = db.FileInfo.Where(a=>a.id==id).First();
        var archiveno = from ac in db.FileInfo
                            where ac.id == id
                            select ac.archivesNo;
            string d = archiveno.First();
            var file2 = from ab in db.FileInfo
                        where ab.archivesNo == d
                        orderby ab.seqNo
                        select ab;
            string ArchiveNo = archiveno.First();
            
                int index = ArchiveNo.IndexOf('.');
                int index1 = index + 1;
                string str1 = ArchiveNo.Substring(0, index+1);
                string str2 = ArchiveNo.Substring(index + 1, ArchiveNo.Length - 1 - index);         
            var c = file2.First();
            int b =Convert.ToInt32(c.seqNo);
            int seq1 = b;
           
            var file4 = from ae in db.FileInfo
                        where ae.archivesNo == d
                        orderby ae.seqNo descending
                        select ae;
            int seq2 = Convert.ToInt32(file4.First().seqNo);
            if (seq2 == seq1)
            {
                ViewData["button1"] = true;
                ViewData["button2"] = true;
            }
            if (seqNo == seq1)
            {
                ViewData["button1"] = true;
            }
            if (seqNo == seq2)
            {
                ViewData["button2"] = true;
            }
            if (action=="修改")
            {
                if (Request.Form["SelectedID"] != null && Request.Form["SelectedID"] != "")
                {
                    
                     fileinfo.type = Request.Form["SelectedID"];

                }
                fileinfo.seqNo = seqNo;
                fileinfo.fileNo = fileNo;
                fileinfo.fileName = fileName1;
                fileinfo.responsible = responsible1;
                fileinfo.startDate = startDate;
                fileinfo.startPageNo = startPageNo;
                fileinfo.remarks = remarks;
                string page = Request.Form["startPageNo"];
                var no = page.IndexOf('-');
                if (page.IndexOf('-') != -1)
                {
                    fileinfo.endPageNo = int.Parse(page.Split('-').Last());
                }
                else
                {
                    fileinfo.endPageNo = int.Parse(page);
                }
                if (ModelState.IsValid)
                {
                    db.Entry(fileinfo).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("<script>alert('已成功修改！');window.location.href='/ArchivesEnter/FileList?id1=" + ArchiveNo +"&id="+id+"&id2=2" +"';</script >"); ;
                }
            }
            //if(action=="返回")
            //{
            //    //if (index1 ==0)
            //    //  {
            //    //      str1 = "";

            //    //      return RedirectToAction("FileList", new { id1 = str1, id2 = str2,id3=index1 });
            //    //  }
            //    return RedirectToAction("FileList", new { id1 = archivesNo,id=id,id2=2}); 
            ////    return Redirect(url1);               
            //}
            if(action=="上一页")
            {
                if(seqNo== seq1)
                {
                    ViewData["button1"] = true;
                    return View(file.First());
                }
                else
                {
                    int e = seqNo - 1;
                    int seq3 = Convert.ToInt32(e);
                   var file3 = from ac in db.FileInfo
                            where ac.seqNo == seq3 && ac.archivesNo==c.archivesNo
                            select ac;
                    return Content("<script>window.location.href='/ArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + file3.First().id + "&id2=2" + "';</script >"); ;
                    //return RedirectToAction("FileList", new { id1 = archivesNo, id = file3.First().id, id2 = 2 });
                }
            }
            if(action=="下一页")
            {
                if (seqNo == seq2)
                {
                    ViewData["button2"] = true;
                }
                else
                {
                    int f = seqNo + 1;
                    int seq4 = Convert.ToInt32(f);
                    var file5= from ac in db.FileInfo
                                where ac.seqNo == seq4 && ac.archivesNo == c.archivesNo
                                select ac;
                    return Content("<script>window.location.href='/ArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + file5.First().id + "&id2=2" + "';</script >"); ;

                    //return RedirectToAction("FileList", new { id1 = archivesNo, id = file5.First().id, id2 = 2 });
                }
            }
            if (action == "删除词条")
            {
                var id1 = int.Parse(Request.Form["DELETE_ID1"].Split('-').First());
                WordTable wordtable = db.WordTable.Find(id1);
                db.WordTable.Remove(wordtable);
                var list1 = db.WordTable.Where(ad => ad.newid > wordtable.newid).OrderBy(ad => ad.newid);
                foreach (var i in list1)
                {
                    i.newid -= 1;
                    db.Entry(i).State = EntityState.Modified;
                }
                db.SaveChanges();
                //return RedirectToAction("FileList", new { id1 = ArchiveNo.Trim(), id = id, id2 = 2 });
                return Content("<script>window.location.href='/ArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + id + "&id2=2" + "';</script >"); ;
            }
            return View(file.First());
      } 
        public ActionResult Delete(long id,string id2)
        {
            var file = from ad in db.FileInfo
                       where ad.id == id
                       select ad;

            FileInfo fileinfo = file.First();
            string archivesNo = file.First().archivesNo;
            db.Entry(fileinfo).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("FileList", new { id1 = archivesNo.Trim(), id = id, id2 = 0 });
            /*href='/ArchivesEnter/FileList'*/
            //return JavaScript("删除成功！");
            //return Content("<script >alert('该联系单缺失工程信息！请重新录入工作联系单！');window.location.href='/VideoContractSheets/Index';</script >");
            //return Content("<script>alert('已成功删除！');window.location.href='/ArchivesEnter/FileList?id1=" + archivesNo + "&id=" + id + "&id2=0" + "';</script >");
            //return Content("<script >alert('已成功删除！');window.location.href='http://localhost:59320/ProjectInfoes/ManagementPrint/?archiveno=" + "';</script >");


        }
        public  ActionResult Details(long id,string archivesNo)
        {
            var file = from ad in db.FileInfo
                       where (ad.id == id)
                       select ad;
            FileInfo fileinfo = file.First();
            ViewData["archivesNo"] = archivesNo;
            return View(fileinfo);
        }
        [HttpPost]
        public ActionResult Details( string archivesNo)
        {
            //return Redirect("/ArchivesEnter/FileList/?id="+ id2);
            return RedirectToAction("FileList",new{id1= archivesNo,id=0,id2=0});
        }

        public ActionResult Create(string  ID3)
        {
            ViewData["button2"] = true;
            string str3 = ID3.Trim();



            var file = from a in db.FileInfo
                       where a.archivesNo == str3
                       orderby a.seqNo descending
                       select a;

            var file1 = from b in db.FileInfo
                        where b.archivesNo == str3
                        orderby b.endPageNo descending
                        select b;

            FileInfo file2 = new FileInfo();
            if (file.Count() == 0)
            {
                file2.seqNo = 1;
                file2.startPageNo = "1";
            }
            else
            {
                file2.seqNo = file.First().seqNo + 1;
                int index = file1.First().endPageNo.ToString().IndexOf('-');
                if (index == -1)
                {
                    file2.startPageNo = (file1.First().endPageNo + 1).ToString().Trim();
                }
                else
                {
                    string str1 = file1.First().endPageNo.ToString().Substring(index + 1, file2.startPageNo.Length - index - 1);

                    int str5 = Int32.Parse(str1.Trim()) + 1;

                    file2.startPageNo = str5.ToString().Trim();
                }
            }


            file2.archivesNo = str3;
            return View(file2);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(long id, int seqNo, string type, string fileNo, string fileName, string responsible, string startPageNo, string startDate, string remarks, string action,string archivesNo)
        {
            var file = from a in db.ArchivesDetail
                       where a.archivesNo == archivesNo
                       select a.registrationNo;
            string ArchiveNo = archivesNo;
            int index = ArchiveNo.IndexOf('.');
            int index1 = index + 1;
            string str1 = ArchiveNo.Substring(0, index+1);
            string str2 = ArchiveNo.Substring(index + 1, ArchiveNo.Length - 1 - index);
            FileInfo file3 = new FileInfo();
            file3.id =Convert.ToInt32(id + 1);
            file3.seqNo = seqNo;
            file3.type = type;
            file3.fileNo = fileNo;
            file3.fileName = fileName;
            file3.responsible = responsible;
            file3.startPageNo = startPageNo;
            file3.startDate = startDate;
            file3.remarks = remarks;
            file3.archivesNo = archivesNo;
            file3.dengjihao = file.First();
            if (action == "确定")
            {
                if (ModelState.IsValid)
                {
                    db.FileInfo.Add(file3);

                    db.SaveChanges();

                    return Content("<script >alert('添加成功！');window.location.href='/ArchivesEnter/FileList?id1=" + ArchiveNo + "';</script >");
                }
            }
            if(action=="取消")
            {

                return RedirectToAction("FileList", new { id1 = archivesNo });
            }
            if (action == "删除词条")
            {
                string temp = Request.Form["DELETE_ID"];
                id = int.Parse(Request.Form["DELETE_ID"].Split('-').First());
                WordTable wordtable = db.WordTable.Find(id);
                db.WordTable.Remove(wordtable);
                var list1 = db.WordTable.Where(ad => ad.newid > wordtable.newid).OrderBy(ad => ad.newid);
                foreach (var i in list1)
                {
                    i.newid -= 1;
                    db.Entry(i).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            return View();


        }
        public ActionResult GuiDang(long ?id)
        {
            var archivedetail = from c in db.ArchivesDetail
                                where c.paperProjectSeqNo == id
                                orderby c.paperProjectSeqNo
                                select c;
            bool flag = false;
            string archivesNo = string.Empty;
            foreach (var dr in archivedetail)
            {
                if (dr.archivesTitle==""|| dr.archivesTitle.ToString()==null)
                {
                    flag = true;
                    archivesNo = dr.archivesTitle.ToString().Trim();
                    break;
                }
            }
            if (flag == true)
            {
                return Content("<script >alert('档号为[" + archivesNo + "]的案卷题名内容为空，不能归档！');window.history.back();</script >");
              
            }
            var paperArchive = from a in db.PaperArchives
                               where a.paperProjectSeqNo == id
                               select a;
            PaperArchives paper = paperArchive.First();
            long id1 =Convert.ToInt32(paper.projectID);
            var project = from b in db.ProjectInfo
                          where b.projectID == id1
                          select b;
            ProjectInfo pro = project.First();
            paper.dateArchive = DateTime.Today.Date;
            pro.status = "10";
            db.Entry(paper).State = EntityState.Modified;
            db.Entry(pro).State = EntityState.Modified;
           
            
                db.SaveChanges();
           return RedirectToAction("ArchiveEnter");
        }
        public ActionResult WaitStorage( string currentFilter, string SearchString, int? SelectedID)
        {
            ViewData["pagename"] = "ArchivesEnter/WaitStorage";
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程序号", Value = "1"},
                new SelectListItem { Text = "设计单位", Value = "2" },
                new SelectListItem { Text = "施工单位", Value = "3" },
                new SelectListItem { Text = "项目顺序号", Value = "4" },
                new SelectListItem { Text = "起始排架号", Value = "5" },
                new SelectListItem { Text = "档号", Value = "6" },


            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            int t = SelectedID.GetValueOrDefault();

           
            ViewBag.CurrentFilter = SearchString;
         
 
            var vwprojectlist = from ad in db.vw_projectList
                                where ad.status == "10"
                                select ad;
            
            

            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (t)
                {
                    case 0:
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 1:
                        
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectNo.ToString().Contains(SearchString));//根据地点搜索
                        break;
                    case 2:
                        vwprojectlist = vwprojectlist.Where(ad => ad.disignOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwprojectlist = vwprojectlist.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                        break;
                    case 4:
                        vwprojectlist = vwprojectlist.Where(ad => ad.paperProjectSeqNo.ToString().Contains(SearchString));//根据工程序号搜索
                        break;
                    case 5:
                        vwprojectlist = vwprojectlist.Where(ad => ad.startPaijiaNo.Contains(SearchString));
                        break;
                    case 6:
                        vwprojectlist = vwprojectlist.Where(ad => ad.startArchiveNo.Contains(SearchString));//根据责任书编号搜索
                        break;
                }

            }


            string user = User.Identity.Name;
            bool flag = false;
            var depart = from c in ab.AspNetUsers
                         where c.UserName == user && c.DepartmentName == "业务科"
                         select c.RoleName;

            foreach (string Role in depart)
            {
                if (Role == "科员")
                {
                    flag = true; break;
                }
                else
                {
                    flag = false;
                }
            }

            if (flag == true)
            {
                vwprojectlist = from a in vwprojectlist
                                where a.collator.Contains(user)
                                select a;
            }
            //vwprojectlist = vwprojectlist.OrderByDescending(s => s.projectNo);// 默认按项目顺序号排列
            vwprojectlist = vwprojectlist.OrderByDescending(s => s.projectNo).ThenBy(s => s.paperProjectSeqNo);
            ViewBag.result = JsonConvert.SerializeObject(vwprojectlist);
            return View();

        }
        public ActionResult EnterStorage(long ?id)
        {
            var paperArchive = from a in db.PaperArchives
                               where a.paperProjectSeqNo == id
                               select a;
            PaperArchives paper = paperArchive.First();
            long id1 = Convert.ToInt32(paper.projectID);
            var project = from b in db.ProjectInfo
                          where b.projectID == id1
                          select b;
            ProjectInfo pro = project.First();
            paper.dateArchive = DateTime.Now;
            pro.status = "7";
            db.Entry(paper).State = EntityState.Modified;
            db.Entry(pro).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("WaitStorage");

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                rptH.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}