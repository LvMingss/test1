using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using urban_archive.Models;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Text;
using System.Data.SqlClient;
using System.Data.OleDb;
using CrystalDecisions.Web;
using System.Web;
using QRCoder;
using System.Drawing;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using System.IO;

namespace urban_archive.Controllers
{
    public class gxArchivesEnterController : Controller
    {
        // GET: ArchivesEnter
        private gxArchivesContainer bb = new gxArchivesContainer();
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        private OfficeEntities cb = new OfficeEntities();
        public ActionResult window()
        {
            return View();
        }
        public ActionResult jiaojiemulu(long? paperProjectSeqNo, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in bb.vw_gxpassList
                      where ad.paperProjectSeqNo == paperProjectSeqNo
                      orderby ad.registrationNo
                      select ad;
            var NO = ds1.Count();
            var ds = ds1.Take(NO).ToList();
            var person = User.Identity.Name;
            localReport.ReportPath = Server.MapPath("~/Report/guanxian/jiaojiemulu.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("gxjiaojiemulu", ds);
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
            var ds1 = from ad in bb.gxArchivesDetail
                      where ad.paperProjectSeqNo == paperProjectSeqNo
                      select ad;
            var ds2 = from ad in bb.vw_gxtransferPaper
                      where ad.paperProjectSeqNo == paperProjectSeqNo
                      select ad;
            int totaljuanshu = ds1.Count();
            int n = totaljuanshu %9;
            int page = totaljuanshu /9;
            if (n != 0)
            {
                page = (totaljuanshu /9) + 1;
            }
            localReport.ReportPath = Server.MapPath("~/Report/guanxian/yijiaoshu.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("yijiaoshu1", ds1);
            ReportDataSource reportDataSource2 = new ReportDataSource("yijiaoshu2", ds2);
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
            if (action == "打印管线档案移交书统计表（日期）")
            {
                DateTime DataFrom = DateTime.Parse(Request.Form["DateStart"]);
                DateTime DataTo = DateTime.Parse(Request.Form["DateEnd"]);
                LocalReport localReport = new LocalReport();
                var ds = bb.vw_gxtransferStatis.Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                localReport.ReportPath = Server.MapPath("~/Report/guanxian/YiJiaoShuTongJi.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("gxYiJiaoShuTongJi", ds);
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
            if (action == "打印管线档案移交书统计表")
            {
                string sNo = Request.Form["sseqNo"];
                string eNo = Request.Form["eseqNo"];
                long n = long.Parse(sNo);
                long m = long.Parse(eNo);
                LocalReport localReport = new LocalReport();
                var ds = bb.vw_gxtransferStatis.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.paperProjectSeqNo <= m);
                localReport.ReportPath = Server.MapPath("~/Report/guanxian/YiJiaoShuTongJi1.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("gxYiJiaoShuTongJi", ds);
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
            if (action == "打印管线档案案卷目录")
            {
                LocalReport localReport = new LocalReport();
                string PNoS = Request.Form["seqNoStart"];
                string VNoS = Request.Form["VolNoStart"];
                string PNoE = Request.Form["seqNoEnd"];
                string VNoE = Request.Form["VolNoEnd"];
                long n = long.Parse(PNoS);
                long m = long.Parse(PNoE);
                long a = long.Parse(VNoS);
                long b = long.Parse(VNoE);
                var temp = bb.vw_gxarchiveMainList.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.paperProjectSeqNo <= m);
                var temp1 = temp.Where(ad => ad.volNo >= a).Where(ad => ad.volNo <= b);
                List<vw_gxarchiveMainList> list = temp1.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].licenseNo != null)
                        list[i].licenseNo = list[i].licenseNo.Trim();
                    if (list[i].changeLog != null)
                        list[i].changeLog = list[i].changeLog.Trim();
                    if (list[i].archivesTitle != null)
                        list[i].archivesTitle = list[i].archivesTitle.Trim();
                    if (list[i].developmentOrganization != null)
                        list[i].developmentOrganization = list[i].developmentOrganization.Trim();
                    if (list[i].constructionOrganization != null)
                        list[i].constructionOrganization = list[i].constructionOrganization.Trim();
                    if (list[i].location != null)
                        list[i].location = list[i].location.Trim();
                }
                var ds = list;
                localReport.ReportPath = Server.MapPath("~/Report/guanxian/AnJuanMuLu.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("gxAnJuanMuLu", ds);
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
            if (action == "打印城建建设管线档案工程目录")
            {
                LocalReport localReport = new LocalReport();
                long Sseq = long.Parse(Request.Form["startseqNo"]);
                long Eseq = long.Parse(Request.Form["endseqNo"]);
                var ds = bb.vw_gxArchiveList.Where(ad => ad.paperProjectSeqNo >= Sseq).Where(ad => ad.paperProjectSeqNo <= Eseq);
                List<vw_gxArchiveList> list = ds.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].licenseNo != null)
                        list[i].licenseNo = list[i].licenseNo.Trim();
                    if (list[i].projectName != null)
                        list[i].projectName = list[i].projectName.Trim();
                    if (list[i].archivesCount != null)
                        list[i].archivesCount = list[i].archivesCount.Trim();
                    if (list[i].developmentOrganization != null)
                        list[i].developmentOrganization = list[i].developmentOrganization.Trim();
                }
                var ds1 = list;
                localReport.ReportPath = Server.MapPath("~/Report/guanxian/GongChengMuLu.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("gxGongChengMuLu", ds1);
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
            var ds1 = from ad in bb.vw_gxfileList
                      where ad.archivesNo == myid
                      orderby ad.seqNo
                      select ad;

            var ds = ds1.ToList();
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
            dc = new DataColumn("ID");
            dt.Columns.Add(dc);
            dc = new DataColumn("archivesNo");
            dt.Columns.Add(dc);

            for (int i = 0; i < ds.Count(); i++)
            {
                DataRow dr = dt.NewRow();
                dr["fileNo"] = ds[i].fileNo;
                dr["firstResponsible"] = ds[i].firstResponsible;
                dr["fileName"] = ds[i].fileName;
                dr["startPageNo"] = ds[i].startPageNo;
                dr["startDate"] = ds[i].startDate;
                dr["endDate"] = ds[i].endDate;
                dr["endPageNo"] = ds[i].endPageNo;
                dr["remarks"] = ds[i].remarks;
                dr["seqNo"] = ds[i].seqNo;
                dr["ID"] = ds[i].ID;
                dr["responsible"] = ds[i].responsible;
                dr["archivesNo"] = ds[i].archivesNo;
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
            localReport.ReportPath = Server.MapPath("~/Report/guanxian/JuanNeiMuLu.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("gxjuanneimulu", dt);
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
        public ActionResult NBFengPi(string id1,string title,string data)
        {
            ReportDocument rptH = new ReportDocument();
            var No = id1.Split('、');
            var count = id1.Split('、').Count();
            var list = bb.vw_gxprojectList.Where(a => a.isNB == "内部").ToList();
            List<vw_gxprojectList> list1 = new List<vw_gxprojectList>();
            if (count == 1)
            {
                var m = long.Parse(id1);
                list1 = list.Where(a => a.paperProjectSeqNo == m).ToList();
            }
            if (count > 1)
            {
                for (int i = 0; i < count; i++)
                {
                    var fanwei = No[i].Split('-');
                    var fanweicount = fanwei.Count();
                    if (fanweicount == 2)
                    {
                        long fanweiS = long.Parse(fanwei[0]);
                        long fanweiE = long.Parse(fanwei[1]);

                        list1 = list.Where(a => a.paperProjectSeqNo >= fanweiS).Where(a => a.paperProjectSeqNo <= fanweiE).Union(list1).OrderBy(a => a.paperProjectSeqNo).ToList();
                    }
                    if (fanweicount == 1)
                    {
                        long fanweiNo = long.Parse(No[i]);
                        list1 = list.Where(a => a.paperProjectSeqNo == fanweiNo).Union(list1).OrderBy(a => a.paperProjectSeqNo).ToList();
                    }
                }
            }
            string bianzhi = list1.First().MapOrginisation;
            if (bianzhi == null)
            {
                bianzhi = "";
            }
            //var list = bb.vw_gxprojectList.Where(a => a.paperProjectSeqNo >= id1).Where(a => a.paperProjectSeqNo <= id2).Where(a=>a.isNB=="内部").ToList();
            long? text = 0;
            long? drawing = 0;
            int number = list1.Count();

            for (int i = 0; i < list1.Count(); i++)
            {
                if (list1[i].characterVolumeCount == null)
                {
                    list1[i].characterVolumeCount = 0;
                }
                if (list1[i].drawingVolumeCount == null)
                {
                    list1[i].drawingVolumeCount = 0;
                }
                text += list1[i].characterVolumeCount;
                drawing += list1[i].drawingVolumeCount;

            }
            string no1 = list1.First().paperProjectSeqNo.ToString();
            string no2 = list1.Last().paperProjectSeqNo.ToString();
            long? total = text + drawing;
            string jianshu = number + "(" + total + ")";
            string title1 = title + "（项目顺序号 " + no1 + " - " + no2 + "）";
            //conn.Close();
            rptH.Load(Server.MapPath("~/") + "//Report//guanxian//NeibuFengPi.rpt");
            rptH.SetParameterValue("jianshu", jianshu);
            rptH.SetParameterValue("title", title1);
            rptH.SetParameterValue("bianzhi", bianzhi);
            rptH.SetParameterValue("data", data);

            //rptH.SetDataSource(dt1);
            System.IO.Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        //public ActionResult Reporting1(ArchivesDetail archivesdetail,string registrationNo,string type = "PDF")
        //{
        //    LocalReport localReport = new LocalReport();
        //    var ds1 = from ad in bb.gxArchivesDetail
        //             where ad.registrationNo == registrationNo
        //              select ad;
        //    long temp_paperProjectSeqNo = ds1.ToList().First().paperProjectSeqNo;
        //    var ds2 = from ad in bb.gxPaperArchives
        //              where ad.paperProjectSeqNo == temp_paperProjectSeqNo
        //              select ad;
        //    long? temp_projectID = bb.gxPaperArchives.ToList().First().projectID;
        //    var ds3= from ad in bb.gxProjectInfo
        //              where ad.projectID == temp_projectID
        //             select ad;
        //    string security = ds3.ToList().First().securityID;
        //    string retentionperiod = ds3.ToList().First().retentionPeriodNo;
        //    var ds5 = from ad in db.SecurityClassification
        //              where ad.securityID == security
        //              select ad;
        //    var ds4 = from ad in db.RetentionPeriod
        //              where ad.retentionPeriodNo == retentionperiod
        //              select ad;
        //    localReport.ReportPath = Server.MapPath("~/Report/guanxian/anjuanfengpi.rdlc");
        //    ReportDataSource reportDataSource1 = new ReportDataSource("gxfengpi1", ds1);
        //    ReportDataSource reportDataSource2 = new ReportDataSource("gxfengpi2", ds2);
        //    ReportDataSource reportDataSource3 = new ReportDataSource("gxfengpi3", ds3);
        //    ReportDataSource reportDataSource5 = new ReportDataSource("gxfengpi5", ds5);
        //    ReportDataSource reportDataSource4 = new ReportDataSource("gxfengpi4", ds4);
        //    localReport.DataSources.Add(reportDataSource1);
        //    localReport.DataSources.Add(reportDataSource2);
        //    localReport.DataSources.Add(reportDataSource3);
        //    localReport.DataSources.Add(reportDataSource4);
        //    localReport.DataSources.Add(reportDataSource5);
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
            command.CommandText = "select * from gxArchivesDetail";
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


        public ActionResult Reporting1(string archivesNo,string classify)
        {
            var a = from aa in bb.vw_gxarchiveInfo
                    where aa.archivesNo == archivesNo
                    where aa.classifyID == classify
                    select aa;
            vw_gxarchiveInfo vw_gxarchiveInfo = a.First();

            var b = from aa in bb.gxArchivesDetail
                    where aa.archivesNo == vw_gxarchiveInfo.archivesNo
                    where aa.paperProjectSeqNo == vw_gxarchiveInfo.paperProjectSeqNo
                    select aa;
            gxArchivesDetail gxArchivesDetail = b.First();
            
            string uu = gxArchivesDetail.UUID;
            if (uu == null || uu == "")
            {
                //pickUUID();//第一次初始化案卷表

                //如果该条案卷没有uuid，先生成
                gxArchivesDetail.UUID = System.Guid.NewGuid().ToString("N");  //创建uuid
                uu = gxArchivesDetail.UUID;
            }
            gxArchivesDetail.UUID = uu; 
            byte[] bt1 = QRCodeGenerate(uu);
            gxArchivesDetail.pic = bt1;
            bb.Entry(gxArchivesDetail).State = EntityState.Modified;
            bb.SaveChanges();

            ReportDocument rptH = new ReportDocument();
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider=SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string NB = "外部      ";
            string sql = "SELECT     vw_gxarchiveInfo.* FROM         vw_gxarchiveInfo where archivesNo='" + archivesNo +"' and classifyID='"+classify.Trim()+ "' and isNB='" + NB +"'";
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
            rptH.Load(Server.MapPath("~/") + "//Report//guanxian//AnJuanFengPi.rpt");
            rptH.SetDataSource(dt1);
            System.IO.Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult Reporting2(/*ArchivesDetail archivesdetail,*/ string archivesNo, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in bb.gxArchivesDetail
                      where ad.archivesNo == archivesNo
                      select ad;
            localReport.ReportPath = Server.MapPath("~/Report/guanxian/备考表.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("gxbeikaobiao", ds1);
            localReport.DataSources.Add(reportDataSource1);
            //localReport.SetParameters(new ReportParameter("ReportParameter1", "123"));

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
        public ActionResult beikaobiaoNB(string  id1)
        {
            LocalReport localReport = new LocalReport();
            var No = id1.Split('、');
            var count = id1.Split('、').Count();
            var list = bb.vw_gxprojectList.Where(a => a.isNB == "内部").ToList();
            List<vw_gxprojectList> list1 = new List<vw_gxprojectList>();
            if (count == 1)
            {
                var m = long.Parse(id1);
                list1 = list.Where(a => a.paperProjectSeqNo == m).ToList();
            }
            if (count > 1)
            {
                for (int i = 0; i < count; i++)
                {
                    var fanwei = No[i].Split('-');
                    var fanweicount = fanwei.Count();
                    if (fanweicount == 2)
                    {
                        long fanweiS = long.Parse(fanwei[0]);
                        long fanweiE = long.Parse(fanwei[1]);

                        list1 = list.Where(a => a.paperProjectSeqNo >= fanweiS).Where(a => a.paperProjectSeqNo <= fanweiE).Union(list1).OrderBy(a => a.paperProjectSeqNo).ToList();
                    }
                    if (fanweicount == 1)
                    {
                        long fanweiNo = long.Parse(No[i]);
                        list1 = list.Where(a => a.paperProjectSeqNo == fanweiNo).Union(list1).OrderBy(a => a.paperProjectSeqNo).ToList();
                    }
                }
            }
            localReport.ReportPath = Server.MapPath("~/Report/guanxian/备考表NB.rdlc");
            long? text = 0;
            long? drawing = 0;
            int number = list1.Count();

            for (int i = 0; i < list1.Count(); i++)
            {
                text += list1[i].characterVolumeCount;
                drawing += list1[i].drawingVolumeCount;

            }
            long? total = text + drawing;
            //ReportDataSource reportDataSource1 = new ReportDataSource("gxbeikaobiao", ds1);
            //localReport.DataSources.Add(reportDataSource1);
            List<ReportParameter> parameterList = new List<ReportParameter>();
            parameterList.Add(new ReportParameter("text", text.ToString().Trim()));
            parameterList.Add(new ReportParameter("drawing", drawing.ToString().Trim()));
            parameterList.Add(new ReportParameter("total", total.ToString().Trim()));
            localReport.SetParameters(parameterList);
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
                "<DeviceInfo>" +
                "<OutPutFormat>" + "PDF" + "</OutPutFormat>" +
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
        public ActionResult muluNB(string id1)
        {
            LocalReport localReport = new LocalReport();
            var No = id1.Split('、');
            var count = id1.Split('、').Count();
            var list = bb.vw_gxprojectList.Where(a => a.isNB == "内部").ToList();
            List<vw_gxprojectList> list1 = new List<vw_gxprojectList>();
            if (count == 1)
            {
                var m = long.Parse(id1);
                list1 = list.Where(a => a.paperProjectSeqNo == m).ToList();
            }
            if (count > 1)
            {
                for (int i = 0; i < count; i++)
                {
                    var fanwei = No[i].Split('-');
                    var fanweicount = fanwei.Count();
                    if (fanweicount == 2)
                    {
                        long fanweiS = long.Parse(fanwei[0]);
                        long fanweiE = long.Parse(fanwei[1]);

                        list1 = list.Where(a => a.paperProjectSeqNo >= fanweiS).Where(a => a.paperProjectSeqNo <= fanweiE).Union(list1).OrderBy(a => a.paperProjectSeqNo).ToList();
                    }
                    if (fanweicount == 1)
                    {
                        long fanweiNo = long.Parse(No[i]);
                        list1 = list.Where(a => a.paperProjectSeqNo == fanweiNo).Union(list1).OrderBy(a => a.paperProjectSeqNo).ToList();
                    }
                }
            }

            localReport.ReportPath = Server.MapPath("~/Report/guanxian/muluNB.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("nbmuluDataSet", list1);
            localReport.DataSources.Add(reportDataSource1);

            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
                "<DeviceInfo>" +
                "<OutPutFormat>" + "pdf" + "</OutPutFormat>" +
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
            return Json(list1, JsonRequestBehavior.AllowGet);
        }
        public string box(string id)
        {

            int?  no = bb.gxPaperArchives.Max(a => a.boxNo);
            if (no == null)
            {
                no = 0;
            }
            string boxno = (no + 1).ToString();
            return boxno;
        }
        public ActionResult creatbox(string action)
        {
            ViewBag.box = bb.gxPaperArchives.Max(a => a.boxNo) + 1;
            ViewBag.classifyID= new SelectList(bb.gxClassType, "classTypeID", "classTypeName");
            ViewBag.title = "建筑工程地下管线工程竣工测量文件";
            if (action == "打印内部文件目录")
            {
                string id1 = Request.Form["NoS"];
                return RedirectToAction("muluNB", new { id1=id1});
            }
            if (action == "打印内部文件备考表")
            {
                string id1 = Request.Form["NoS"];
                return RedirectToAction("beikaobiaoNB", new { id1 = id1 });
            }
            if (action == "打印内部文件封皮")
            {
                string id1 = Request.Form["NoS"];
                string dataf = Request.Form["dataf"];
                string datae = Request.Form["datae"];
                string data = dataf + "-" + datae;
                string title = Request.Form["title"];
                return RedirectToAction("NBFengPi", new { id1 = id1,title=title,data=data });
            }
            if (action == "保存盒号")
            {
                string  no =Request.Form["NoS"];
                var No = no.Split('、');
                var count = no.Split('、').Count();
                var list = bb.gxPaperArchives.Where(a => a.InchCountDetail == "").ToList();
                List<gxPaperArchives>list1 = new List<gxPaperArchives>();
                if (count==1)
                {
                    var fanwei = no.Split('-');
                    var fanweicount = fanwei.Count();
                    if (fanweicount == 2)
                    {
                        long fanweiS = long.Parse(fanwei[0]);
                        long fanweiE = long.Parse(fanwei[1]);

                        list1 = list.Where(a => a.paperProjectSeqNo >= fanweiS).Where(a => a.paperProjectSeqNo <= fanweiE).Union(list1).OrderBy(a => a.paperProjectSeqNo).ToList();
                    }
                    if (fanweicount == 1)
                    {
                        var m = long.Parse(no);
                        list1 = list.Where(a => a.paperProjectSeqNo == m).ToList();
                    }
                }
                if (count > 1)
                {
                    for (int i = 0; i < count; i++)
                    {
                        var fanwei = No[i].Split('-');
                        var fanweicount = fanwei.Count();
                        if (fanweicount == 2)
                        {
                            long fanweiS = long.Parse(fanwei[0]);
                            long fanweiE = long.Parse(fanwei[1]);

                            list1 = list.Where(a => a.paperProjectSeqNo >= fanweiS).Where(a => a.paperProjectSeqNo <= fanweiE).Union(list1).OrderBy(a=>a.paperProjectSeqNo).ToList();
                        }
                        if (fanweicount == 1)
                        {
                            long fanweiNo = long.Parse(No[i]);
                            list1 = list.Where(a => a.paperProjectSeqNo == fanweiNo).Union(list1).OrderBy(a => a.paperProjectSeqNo).ToList();
                        }
                    }
                }
                
                string  box =Request.Form["boxNo"];
                int boxno = int.Parse(box);
                for (int i = 0; i < list1.Count(); i++)
                {
                    list1[i].Number = i + 1;
                    list1[i].boxNo = boxno;
                    ////给每个工程赋值总登记号
                    //string mainID = list[i].mainCategoryID.Trim()+list[i].subDictionaryID.Trim();
                    //var max_archiveNo = from a in bb.MaxArchiveRegisNo1
                    //                    where a.mainCategoryID == mainID
                    //                    select a;
                    //MaxArchiveRegisNo1 maxModel = max_archiveNo.First();
                    //string maxregis=generatefollowByMaxModel2(maxModel, max_archiveNo.First().mainCategoryID.ToString(), 1);
                    //list[i].startRegisNo = maxregis;
                    bb.Entry(list1[i]).State = EntityState.Modified;
                }
                bb.SaveChanges();
                return Content("<script >alert('已成功保存盒号！');window.history.back();</script >");
            }
            return View();
        }
        public ActionResult ArchiveEnterNB(string SearchString, string action)
        {
            if (action == "添加内部档案盒号及打印封皮")
            {
                return RedirectToAction("creatbox");
            }
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
            string n = Request.Form["SelectedID"];

            ViewBag.CurrentFilter = SearchString;
            var vwprojectlist = from ad in bb.vw_gxprojectList
                                where ad.status == "6"
                                where ad.isNB == "内部"
                                select ad;


            if (!String.IsNullOrEmpty(SearchString))
            {
                int t = int.Parse(n);
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", n);
                switch (t)
                {
                    case 0:
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 1:
                        long search = Convert.ToInt32(SearchString);
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectNo == search);//根据地点搜索
                        break;
                    case 2:

                        vwprojectlist = vwprojectlist.Where(ad => ad.disignOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwprojectlist = vwprojectlist.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                        break;
                    case 4:

                        vwprojectlist = vwprojectlist.Where(ad => ad.paperProjectSeqNo.ToString().Trim() == SearchString);//根据工程序号搜索
                        break;
                    case 5:

                        vwprojectlist = vwprojectlist.Where(ad => ad.startPaijiaNo == SearchString);
                        break;
                    case 6:

                        vwprojectlist = vwprojectlist.Where(ad => ad.startArchiveNo == SearchString);//根据责任书编号搜索
                        break;



                }

            }

            vwprojectlist = vwprojectlist.OrderByDescending(s => s.paperProjectSeqNo);// 默认按项目顺序号排列
            ViewBag.result = JsonConvert.SerializeObject(vwprojectlist);
            return View();
        }
        public ActionResult ArchiveEnter(string SearchString ,string action)
        {
            if (action == "添加内部档案盒号")
            {
                return RedirectToAction("creatbox");
            }
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
            string n = Request.Form["SelectedID"];
  
            ViewBag.CurrentFilter = SearchString;
            var vwprojectlist = from ad in bb.vw_gxprojectList
                                where ad.status == "6"
                                where ad.isNB=="外部"
                                select ad;


            if (!String.IsNullOrEmpty(SearchString))
            {
                int t = int.Parse(n);
                ViewBag.SelectedID = new SelectList(list, "Value", "Text",n);
                switch (t)
                {
                    case 0:
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 1:
                        long search = Convert.ToInt32(SearchString);
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectNo == search);//根据地点搜索
                        break;
                    case 2:

                        vwprojectlist = vwprojectlist.Where(ad => ad.disignOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwprojectlist = vwprojectlist.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                        break;
                    case 4:

                        vwprojectlist = vwprojectlist.Where(ad => ad.paperProjectSeqNo.ToString().Trim() == SearchString);//根据工程序号搜索
                        break;
                    case 5:

                        vwprojectlist = vwprojectlist.Where(ad => ad.startPaijiaNo == SearchString);
                        break;
                    case 6:

                        vwprojectlist = vwprojectlist.Where(ad => ad.startArchiveNo == SearchString);//根据责任书编号搜索
                        break;



                }

            }

            string user = User.Identity.Name;
            bool flag = false;
            var depart = from c in ab.AspNetUsers
                         where c.UserName == user && c.DepartmentId == 2
                         select c.RoleName;
            if (depart.Count() != 0)
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
        public ActionResult Enter(long? id,int? id2)
        {
            if (id== null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var archive = bb.gxArchivesDetail.Where(a => a.paperProjectSeqNo == id).ToList();
            //var count = archive.Count()-1;
            var count = archive.Count();
            if (id2 == 0)
            {
                archive = bb.gxArchivesDetail.Where(a => a.paperProjectSeqNo == id).OrderBy(a=>a.volNo).Take(count).ToList();
            }
            if (id2 == 1)
            {
                archive = bb.gxArchivesDetail.Where(a => a.paperProjectSeqNo == id).OrderByDescending(a=>a.registrationNo).Take(1).ToList();
            }
            if (archive == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = id;
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
                    string volNo1 = table.Rows[i][1].ToString().Trim();//卷数
                    int volNo2;
                    if (volNo1 == "")
                    {
                        return Content("<script >alert('卷数不能为零！');window.history.back();</script >");
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
                    string licenseNo1 = table.Rows[i][2].ToString().Trim();// dr["执照号"].ToString().Trim();
                    if (licenseNo1 == "")
                    {
                        licenseNo1 = "";
                    }
                    string mapsheetNo1 = table.Rows[i][3].ToString().Trim(); //dr["图幅号"].ToString().Trim();
                    if (mapsheetNo1 == "")
                    {
                        mapsheetNo1 = "";
                    }
                    string microNo1 = table.Rows[i][4].ToString().Trim(); //dr["微缩号"].ToString().Trim();
                    if (microNo1 == "")
                    {
                        microNo1 = "";
                    }
                    string zaojia1 = table.Rows[i][5].ToString().Trim(); //dr["工程造价（万）"].ToString().Trim();
                    if (zaojia1 == "")
                    {
                        zaojia1 = "";
                    }
                    string Fee1 = table.Rows[i][6].ToString().Trim(); //dr["工程结算（万）"].ToString().Trim();
                    if (Fee1 == "")
                    {
                        Fee1 = "";
                    }
                    string structureTypeID1 = table.Rows[i][7].ToString().Trim(); //dr["规格"].ToString().Trim();
                    if (structureTypeID1 == "")
                    {
                        structureTypeID1 = "";
                    }
                    string Material1 = table.Rows[i][8].ToString().Trim(); //dr["材质"].ToString().Trim();
                    if (Material1 == "")
                    {
                        Material1 = "";
                    }
                    string hezai1 = table.Rows[i][9].ToString().Trim(); //dr["荷载"].ToString().Trim();
                    if (hezai1 == "")
                    {
                        hezai1 = "";
                    }
                    string archivesTitle1 = table.Rows[i][10].ToString().Trim(); //dr["案卷题名"].ToString().Trim();
                    if (archivesTitle1 == "")
                    {
                        archivesTitle1 = "";
                    }
                    string firstResponsible1 = table.Rows[i][11].ToString().Trim(); //dr["第一责任者"].ToString().Trim();
                    if (firstResponsible1 == "")
                    {
                        firstResponsible1 = "";
                    }
                    string responsibleOther1 = table.Rows[i][12].ToString().Trim(); //dr["其他责任者"].ToString().Trim();
                    if (responsibleOther1 == "")
                    {
                        responsibleOther1 = "";
                    }
                    string transferUnit1 = table.Rows[i][13].ToString().Trim(); //dr["移交单位"].ToString().Trim();
                    if (transferUnit1 == "")
                    {
                        transferUnit1 = "";
                    }
                    string textMaterial1 = table.Rows[i][14].ToString().Trim(); //dr["文字材料"].ToString().Trim();
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
                    string drawing1 = table.Rows[i][15].ToString().Trim(); //dr["图纸"].ToString().Trim();
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
                    string photoCount1 = table.Rows[i][16].ToString().Trim(); //dr["照片"].ToString().Trim();
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
                    string bianzhiTime1 = table.Rows[i][17].ToString().Trim(); //dr["编制日期"].ToString().Trim();
                    if (bianzhiTime1 == "")
                    {
                        bianzhiTime1 = "";
                    }
                    string jgDate1 = table.Rows[i][18].ToString().Trim(); //dr["进馆日期"].ToString().Trim();
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
                            return Content("<script >alert('进馆日期格式不正确！（2018/10/01）');window.history.back();</script >");
                        }
                    }
                    string overground1 = table.Rows[i][19].ToString().Trim(); //dr["起点"].ToString().Trim();
                    if (overground1 == "")
                    {
                        overground1 = "";
                    }
                    string underground1 = table.Rows[i][20].ToString().Trim(); //dr["止点"].ToString().Trim();
                    if (underground1 == "")
                    {
                        underground1 = "";
                    }
                    string totallong1 = table.Rows[i][21].ToString().Trim(); //dr["总长度"].ToString().Trim();
                    if (totallong1 == "")
                    {
                        totallong1 = "";
                    }
                    string newlocation1 = table.Rows[i][22].ToString().Trim(); //最新工程地址
                    if (newlocation1 == "")
                    {
                        newlocation1 = "";
                    }
                    string remarks1 = table.Rows[i][23].ToString().Trim(); //dr["备注"].ToString().Trim();
                    if (remarks1 == "")
                    {
                        remarks1 = "";
                    }
                    string fazhaoTime1 = table.Rows[i][24].ToString().Trim(); //dr["发照日期"].ToString().Trim();
                    if (fazhaoTime1 == "")
                    {
                        fazhaoTime1 = "";
                    }
                    string kaigongTime1 = table.Rows[i][25].ToString().Trim(); //dr["开工日期"].ToString().Trim();
                    if (kaigongTime1 == "")
                    {
                        kaigongTime1 = "";
                    }
                    string jungongTime1 = table.Rows[i][26].ToString().Trim(); //dr["竣工日期"].ToString().Trim();
                    if (jungongTime1 == "")
                    {
                        jungongTime1 = "";
                    }
                    string indexeDate1 = table.Rows[i][27].ToString().Trim(); //dr["标引日期"].ToString().Trim();
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
                    string checkDate1 = table.Rows[i][28].ToString().Trim(); //dr["审核日期"].ToString().Trim();
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
                    string TyperDate1 = table.Rows[i][29].ToString().Trim(); //dr["录入日期"].ToString().Trim();
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
                    var pro = from b in bb.gxProjectInfo
                              where b.projectName == projectName1
                              select b;
                    gxProjectInfo pro1 = pro.First();
                    var pap = from c in bb.gxPaperArchives
                              where c.projectID == pro1.projectID
                              select c;
                    gxPaperArchives pap1 = pap.First();
                    if (pro1.textMaterial == null || pap1.textMaterial == null)
                    {
                        pro1.textMaterial = 0;
                        pap1.textMaterial = 0;
                        bb.Entry(pap1).State = EntityState.Modified;
                        bb.Entry(pro1).State = EntityState.Modified;
                        bb.SaveChanges();
                    }
                    if (pro1.drawing == null || pap1.drawing == null)
                    {
                        pro1.drawing = 0;
                        pap1.drawing = 0;
                        bb.Entry(pap1).State = EntityState.Modified;
                        bb.Entry(pro1).State = EntityState.Modified;
                        bb.SaveChanges();
                    }
                    if (pap1.PhotoCount == null)
                    {
                        pap1.PhotoCount = 0;
                        bb.Entry(pro1).State = EntityState.Modified;
                        bb.SaveChanges();
                    }
                    string strSql1 = string.Format("update UrbanCon.dbo.gxProjectInfo set textMaterial=textMaterial+" + textMaterial2 + ",drawing=drawing+" + drawing2 + ",newLocation='" + newlocation1 + "' where projectName ='" + projectName1 + "'");
                    //string strSql2 = string.Format("update UrbanCon.dbo.PaperArchives set licenseNo='" + licenseNo1 + "',structureTypeID=(select structureTypeID from UrbanCon.dbo.StructureType where structureTypeName='" + structureTypeName1 + "'),buildingArea=" + buildingArea2 + ",firstResponsible='" + firstResponsible1 + "',responsibleOther='" + responsibleOther1 + "',transferUnit='" + transferUnit1 + "',textMaterial=" + textMaterial2 + ",drawing=" + drawing2 + ",photoCount=" + photoCount2 + ",jgDate='" + jgDate2 + "',height='" + height2 + "',changeLog='" + changeLog1 + "',remarks='" + remarks1 + "',overground='" + overground1 + "',underground='" + underground1 + "',archivesCount='" + archivesCount1 + "' where paperProjectSeqNo = " + paperProjectSeqNo2);                   
                    string strSql2 = string.Format("update UrbanCon.dbo.gxPaperArchives set licenseNo='" + licenseNo1 + "',zaojia='" + zaojia1 + "',Fee='" + Fee1 + "',structureTypeID='" + structureTypeID1 + "',Material='" + Material1 + "',hezai='" + hezai1 + "',firstResponsible='" + firstResponsible1 + "',responsibleOther='" + responsibleOther1 + "',transferUnit='" + transferUnit1 + "',textMaterial=textMaterial+" + textMaterial2 + ",drawing=drawing+" + drawing2 + ",PhotoCount=PhotoCount+" + photoCount2 + ",jgDate='" + jgDate2 + "',overground='" + overground1 + "',underground='" + underground1 + "',totallong='" + totallong1 + "',licenseDate='" + fazhaoTime1 + "',projectStartDate='" + kaigongTime1 + "',projectFinishDate='" + jungongTime1 + "',luruTime='" + TyperDate2 + "' where projectID = " + pro.First().projectID);
                    var arc = from a in bb.gxArchivesDetail
                              where a.paperProjectSeqNo == pap1.paperProjectSeqNo
                              where a.volNo == volNo2
                              select a;
                    string strSql3 = "";
                    if (arc.Count() == 0)
                    {
                        return Content("<script >alert('表格卷数与已有卷数不符！');window.history.back();</script >");
                        //strSql3 = string.Format("insert into UrbanCon.dbo.gxArchivesDetail(volNo, registrationNo, archivesNo, shizhengNo, paperProjectSeqNo, paijiaNo, licenseNo, archivesTitle, firstResponsible,responsibleOther,developmentUnit,transferUnit,designUnit,constructionUnit,textMaterial,drawing,photoCount,archiveThickness,bianzhiTime,jgDate,remarks,fazhaoTime,kaigongTime,jungongTime,indexer,checker,typist,indexDate,checkDate,typerDate) values(" + volNo2 + ",'" + registrationNo1 + "','" + archivesNo1 + "','" + shizhengNo1 + "'," + paperProjectSeqNo2 + ",'" + paijiaNo1 + "','" + licenseNo1 + "','" + archivesTitle1 + "','" + firstResponsible1 + "','" + responsibleOther1 + "','" + developmentOrganization1 + "','" + transferUnit1 + "','" + disignOrganization1 + "','" + constructionOrganization1 + "'," + textMaterial2 + "," + drawing2 + "," + photoCount2 + "," + archiveThickness2 + ",'" + bianzhiTime1 + "','" + jgDate2 + "','" + remarks1 + "','" + fazhaoTime1 + "','" + kaigongTime1 + "','" + jungongTime1 + "','" + indexer1 + "','" + checker1 + "','" + Typist1 + "','" + indexeDate2 + "','" + checkDate2 + "','" + TyperDate2 + "')");
                    }
                    else
                    {
                        strSql3 = string.Format("update UrbanCon.dbo.gxArchivesDetail set licenseNo ='" + licenseNo1 + "',archivesTitle='" + archivesTitle1 + "',firstResponsible='" + firstResponsible1 + "',responsibleOther='" + responsibleOther1 + "',transferUnit='" + transferUnit1 + "',textMaterial=" + textMaterial2 + ",drawing=" + drawing2 + ",photoCount=" + photoCount2 + ",bianzhiTime='" + bianzhiTime1 + "',jgDate='" + jgDate2 + "',remarks='" + remarks1 + "',fazhaoTime='" + fazhaoTime1 + "',kaigongTime='" + kaigongTime1 + "',jungongTime='" + jungongTime1 + "',indexDate='" + indexeDate2 + "',checkDate='" + checkDate2 + "',typerDate='" + TyperDate2 + "' where registrationNo =" + arc.First().registrationNo);
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
                return Content("<script >alert('案卷成功导入，请导入卷内信息！');window.location.href='/gxArchivesEnter/Enter/?id=" + id + "';</script >");
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
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + savePath + "; " + "Extended Properties=Excel 8.0;";
                    //strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + savePath + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";
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
                    if (name.Length != 8)
                    {
                        name = name.PadLeft(8, '0');
                    }
                    int a = int.Parse(name);
                    var num = from ad in bb.gxArchivesDetail
                              where ad.paperProjectSeqNo == id
                              where ad.volNo == a
                              select ad;
                    gxArchivesDetail num2 = num.First();
                    var num1 = from ad in bb.gxFileInfo
                               where ad.dengjihao == num2.registrationNo
                               select ad;
                    int m = num1.Count();
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        string SeqNo = table.Rows[i][0].ToString().Trim();//序号
                        int seq;
                        if (SeqNo == "")
                        {
                            break;//当序号为空时候，结束循环
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
                        string Type = table.Rows[i][1].ToString().Trim();//类型
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
                        string FileNo = table.Rows[i][2].ToString().Trim();//文件编号
                        if (FileNo == "")
                        {
                            FileNo = "";
                        }
                        string Responsible = table.Rows[i][3].ToString().Trim();//责任者
                        if (Responsible == "")
                        {
                            Responsible = "";
                        }
                        string FileTitle = table.Rows[i][4].ToString().Trim();//材料题名
                        if (FileTitle == "")
                        {
                            FileTitle = "";
                        }
                        string StartDate = table.Rows[i][5].ToString().Trim();//编制日期
                        if (StartDate == "")
                        {
                            StartDate = "";
                        }
                        string StartPage = table.Rows[i][6].ToString().Trim();//起止页次
                        if (StartPage == "")
                        {
                            StartPage = "";
                        }
                        string Remarks = table.Rows[i][7].ToString().Trim();//备注
                        if (Remarks == "")
                        {
                            Remarks = "";
                        }
                        var archiveno = from ab in bb.gxArchivesDetail
                                        where ab.paperProjectSeqNo == id
                                        where ab.volNo == a
                                        select ab;
                        string archno = archiveno.First().archivesNo;
                        string degjihao = archiveno.First().registrationNo;
                        int EndPage;
                        if (StartPage.IndexOf('-') != -1)
                        {
                            EndPage = int.Parse(StartPage.Split('-').Last());
                        }
                        else
                        {
                            if (StartPage == "")
                            {
                                EndPage = 0;
                            }
                            else
                            {
                                EndPage = int.Parse(StartPage);
                            }
                        }
                        string strSql;
                        if (m == 0)
                        {
                            strSql = string.Format("insert into UrbanCon.dbo.gxFileInfo(fileName, archivesNo, seqNo, fileNo, startPageNo, endPageNo, responsible, remarks, startDate, type, dengjihao) values('" + FileTitle + "','" + archno + "'," + seq + ",'" + FileNo + "','" + StartPage + "'," + EndPage + ",'" + Responsible + "','" + Remarks + "','" + StartDate + "','" + Type + "','" + degjihao + "')");
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
                return Content("<script >alert('导入完成！');window.location.href='/gxArchivesEnter/Enter/?id=" + id + "';</script >");
            }
            return View();
        }

        public ActionResult EnterAndSee(string id, int? id2)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id2 == 0)
            {
                ViewData["button5"] = true;
            }
            var projectinfo = from b in bb.vw_gxpassList
                              where b.registrationNo == id
                              select b;
            string cla = projectinfo.First().classifyID;
            ViewBag.classify = bb.gxClassType.Where(a=>a.classTypeID==cla).First().classTypeName;

            
            long paperseq = projectinfo.First().paperProjectSeqNo;
            string seq = paperseq.ToString().Trim();
            var project = from c in bb.vw_gxpassList
                          where c.paperProjectSeqNo == paperseq
                          orderby c.volNo
                          select c;
            var project1 = from c in bb.vw_gxpassList
                          where c.paperProjectSeqNo == paperseq
                          orderby c.volNo descending 
                          select c;
            if (projectinfo.Count() == 0)
            {
                ViewData["checkname1"] = 3;

            }
            //初始化相关选择控件
            ViewBag.indexer = new SelectList(ab.AspNetUsers, "UserName", "UserName" ,projectinfo.First().indexer);
            ViewBag.checker = new SelectList(ab.AspNetUsers, "UserName", "UserName", projectinfo.First().checker);
            ViewBag.Typist = new SelectList(ab.AspNetUsers, "UserName", "UserName" ,projectinfo.First().typist );
            ViewBag.securityName = new SelectList(db.SecurityClassification, "securityID", "securityName", projectinfo.First().securityID);
            ViewBag.retentionPeriodName = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", projectinfo.First().retentionPeriodNo);
            //ViewBag.structureTypeName = new SelectList(db.StructureType, "structureTypeID", "structureTypeName", projectinfo.First().structureTypeID);
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "没有", Value = "0"},
                new SelectListItem { Text = "有", Value = "1"},
            };
            var n1 = projectinfo.First().changeLog;
            if (n1 != null && n1 != "" && n1.Trim() == "有")
            {
                ViewBag.changeLog = new SelectList(list, "Value", "Text", 1);
            }
            else {
                ViewBag.changeLog = new SelectList(list, "Value", "Text", 0);
            }
            
            //进行卷数判断，设置按钮可用性
            string Acount = projectinfo.First().archivesCount;
            
            if (Acount == null || Acount == "")
            {
                Acount = "0";
            }
            //int zongjuanshu = Int32.Parse(Acount)-1;
            int zongjuanshu = Int32.Parse(Acount);
            ViewBag.count = zongjuanshu;
            int dijijuan;
            if (zongjuanshu < 2)
            {
                ViewData["button1"] = true;
                ViewData["button2"] = true;
                ViewData["button3"] = true;
                ViewData["button4"] = true;
                ViewBag.jiaojiemulu = "display:inline-block";
                ViewBag.yijiaoshu = "display:inline-block";
            }
            if (projectinfo.First().volNo != 0)
            {
                dijijuan = Convert.ToInt32(projectinfo.First().volNo);
            }
            else if (projectinfo.First().startRegisNo.Trim() != "" && projectinfo.First().registrationNo.Trim() != "")
            {
                string s1 = projectinfo.First().registrationNo.Trim();
                string s2 = projectinfo.First().startRegisNo.Trim();
                dijijuan = Int32.Parse(s1.Substring(2, s1.Length - 2)) - Int32.Parse(s2.Substring(2, s2.Length - 2)) + 1;
            }
            else
                dijijuan = 1;
            if (dijijuan < 1)
            {
                ViewData["checkname1"] = 4;


            }
            //if (dijijuan < zongjuanshu)
            if (dijijuan == project.First().volNo)
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

            string jgdate = Convert.ToDateTime(projectinfo.First().jgDate).ToString("yyyy-MM-dd");
            ViewData["jgDate"] = jgdate;
            if (jgdate.Trim() == "1753-01-01")
            {
                ViewData["jgDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
            }

            string bydate = Convert.ToDateTime(projectinfo.First().indexDate).ToString("yyyy-MM-dd");
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
                ViewData["TyperDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
            }
            vw_gxpassList paper = new vw_gxpassList();
            paper = projectinfo.First();
            //判断案卷是否录入完毕
            if (project.First().archivesTitle != null && project.First().archivesTitle != "")//该卷是接着前一卷录入，将前一卷的值进行传递
            {

                if (project1.First().registrationNo != id && (projectinfo.First().archivesTitle == "" || projectinfo.First().archivesTitle == null))
                {
                    string lr = DateTime.Now.ToString();
                    ViewData["TyperDate"] = Convert.ToDateTime(lr.Trim()).ToString("yyyy-MM-dd");
                    ViewData["checkDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
                    string regis = id;
                    long re = Int32.Parse(regis);
                    int index = regis.Substring(0, 1).IndexOf('0');
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
                    var archive = from a in bb.vw_gxpassList
                                  where a.registrationNo == regisNo
                                  select a;
                    var paperarchive= from a in bb.gxPaperArchives
                                      where a.endRegisNo == regisNo
                                      select a;
                    if (paperarchive.Count()!=0)
                    {
                        var no = paperarchive.First().startRegisNo;
                        archive = from a in bb.vw_gxpassList
                                  where a.registrationNo == no
                                  select a;
                    }
                    //初始化均为0 20170615 周林
                    paper.textMaterial = archive.First().textMaterial;
                    paper.photoCount = archive.First().photoCount;
                    paper.drawing = archive.First().drawing;
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
                    ViewBag.checker = new SelectList(ab.AspNetUsers, "UserName", "UserName", archive.First().checker);
                    ViewBag.Typist = new SelectList(ab.AspNetUsers, "UserName", "UserName", archive.First().typist);


                    paper.fazhaoTime = archive.First().fazhaoTime;
                    paper.jungongTime = archive.First().jungongTime;
                    paper.kaigongTime = archive.First().kaigongTime;
                    ViewData["checkname"] = 1;
                }
                //else /*if(projectinfo.First().isNB=="内部") *///改工程对应的内部案卷，将该工程的第一卷值赋给内部案卷
                //{
                //    string lr = DateTime.Now.ToString();
                //    ViewData["TyperDate"] = Convert.ToDateTime(lr.Trim()).ToString("yyyy-MM-dd");
                //    ViewData["checkDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
                //    string regis = id;
                //    long re = Int32.Parse(regis);
                //    int index = regis.Substring(0, 1).IndexOf('0');
                   
                //    //初始化均为0 20170615 周林
                //    paper.textMaterial = project.First().textMaterial;
                //    paper.photoCount = project.First().photoCount;
                //    paper.drawing = project.First().drawing;
                //    paper.archivesTitle = project.First().archivesTitle;
                //    paper.licenseNo = project.First().licenseNo;
                //    paper.firstResponsible = project.First().firstResponsible;
                //    paper.responsibleOther = project.First().responsibleOther;
                //    paper.developmentOrganization = project.First().developmentOrganization;
                //    paper.transferUnit = project.First().transferUnit;
                //    paper.constructionOrganization = project.First().constructionOrganization;
                //    paper.designUnit = project.First().designUnit;
                //    //ViewBag.changeLog = new SelectList(list, "Value", "Text", project.First().changeLog);
                //    paper.location = project.First().location;
                //    paper.newLocation = project.First().newLocation;
                //    paper.height = project.First().height;
                //    paper.overground = project.First().overground;
                //    paper.underground = project.First().underground;
                //    ViewBag.indexer = new SelectList(ab.AspNetUsers, "UserName", "UserName", project.First().indexer);
                //    ViewBag.checker = new SelectList(ab.AspNetUsers, "UserName", "UserName", project.First().checker);
                //    ViewBag.Typist = new SelectList(ab.AspNetUsers, "UserName", "UserName", project.First().typist);


                //    paper.fazhaoTime = project.First().fazhaoTime;
                //    paper.jungongTime = project.First().jungongTime;
                //    paper.kaigongTime = project.First().kaigongTime;
                //    ViewData["checkname"] = 1;
                //}
            }
            else
            {
                string regis = id;
                long re = Int32.Parse(regis);
                int index = regis.Substring(0, 1).IndexOf('0');
                string regisNo = "";
                if (index == -1)
                {
                    regisNo = re.ToString();
                }
                else
                {
                    regisNo = "0" + re.ToString();
                }
                var archive = from a in bb.vw_gxpassList
                              where a.registrationNo == regisNo
                              select a;
                ViewData["checkname"] = 2;
                try {
                    paper.textMaterial = archive.First().textMaterial;
                    paper.photoCount = archive.First().photoCount;
                    paper.drawing = archive.First().drawing;
                }
                catch (Exception ex) {
                    paper.textMaterial = 0;
                    paper.photoCount = 0;
                    paper.drawing = 0;
                }
            }
            //判断案卷名是否录入
            string archivesNo = "";
            bool flag = false;
            var item = from c in project
                       where c.archivesTitle == ""||c.archivesTitle==null
                       select c;
            if (item.Count() != 0)
            {
                flag = true;
                archivesNo = item.First().archivesTitle.Trim();
            }

            if (flag == true)
            {
                ViewData["checkname"] = archivesNo;

            }
            //if (paper.volNo == int.Parse( Acount))
            //{
            //    paper.archivesTitle = paper.projectName+"竣工测量文件";
            //}

            return View(paper);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //传入
        public ActionResult EnterAndSee(int? securityName, string hezai,string totallong,string zaojia,string Fee, int? retentionPeriodName, string structureTypeID, long paperProjectSeqNo, string action, string RegistrationNo, string archivesNo, string volNo, string shizhengNo, string licenseNo, string mapsheetNo, string microNo, string buildingArea, string archivesTitle , string firstResponsible, string responsibleOther, string developmentOrganization, string transferUnit, string disignOrganization, string PaiJiaNo, string constructionOrganization, string TextMaterial, string drawing, string PhotoCount, string ArchiveThickness, string bianzhiTime, string jgDate, string Material, string location, string remarks, string newLocation, string overground, string underground, int? changeLog, string fazhaoTime , string jungongTime, string kaigongTime, string indexer, string indexeDate, string checker, string checkDate, string Typist, string TyperDate)
        {


            var paperArchive = from a in bb.gxPaperArchives
                               where a.paperProjectSeqNo == paperProjectSeqNo
                               select a;
            gxPaperArchives paperArchives = paperArchive.First();
            long project = Convert.ToInt32(paperArchives.projectID);
            var projectInfo = from b in bb.gxProjectInfo
                              where b.projectID == project
                              select b;
            gxProjectInfo projects = projectInfo.First();
            var archivedetail = from c in bb.gxArchivesDetail
                                where c.paperProjectSeqNo == paperProjectSeqNo
                                select c;
            gxArchivesDetail archivedetails = archivedetail.First();
            var vwprojictlist = from d in bb.vw_gxprojectList
                                where d.paperProjectSeqNo == paperProjectSeqNo
                                select d;

            var proje = from c in bb.vw_gxpassList
                        where c.paperProjectSeqNo == paperProjectSeqNo
                        select c;
            string stratregis = proje.First().startRegisNo;
            string endregis = proje.First().endRegisNo;
            //给相关选择控件赋值
            ViewBag.indexer = new SelectList(ab.AspNetUsers, "UserName", "UserName", indexer);
            ViewBag.checker = new SelectList(ab.AspNetUsers, "UserName", "UserName", checker);
            ViewBag.Typist = new SelectList(ab.AspNetUsers, "UserName", "UserName", Typist);
            ViewBag.securityName = new SelectList(db.SecurityClassification, "securityID", "securityName", securityName);
            ViewBag.retentionPeriodName = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", retentionPeriodName);
            //ViewBag.structureTypeName = new SelectList(db.StructureType, "structureTypeID", "structureTypeName", structureTypeName);
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "没有", Value = "0"},
                new SelectListItem { Text = "有", Value = "1"},
            };
            int t = changeLog.GetValueOrDefault();
            ViewBag.changeLog = new SelectList(list, "Value", "Text", t);

            if (action == "该工程案卷列表")
            {


                return RedirectToAction("Enter", new { id = paperProjectSeqNo });

            }


            if (action == "该卷文件列表")
            {
                int index = archivesNo.IndexOf('.');
                int index1 = index + 1;
                string str1 = archivesNo.Substring(0, index + 1);
                string str2 = archivesNo.Substring(index + 1, archivesNo.Length - 1 - index);
                return RedirectToAction("FileList", new { id1 = archivesNo, id = 0, id2 = 0 });
            }


            if (action == "首卷")
            {



                var pro = from a in bb.vw_gxpassList
                          where a.paperProjectSeqNo == paperProjectSeqNo
                          orderby a.registrationNo
                          select a.registrationNo;
                string startregisNo = pro.First();
                return RedirectToAction("EnterAndSee", new { id = startregisNo });




            }

            if (action == "前一卷")
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
                    var pro = from a in bb.vw_gxpassList
                              where a.paperProjectSeqNo == paperProjectSeqNo
                              orderby a.registrationNo
                              select a.registrationNo;
                    string startregisNo = pro.First();
                    return RedirectToAction("EnterAndSee", new { id = startregisNo });
                }
                else
                {
                    var pro = from a in bb.vw_gxpassList
                              where a.paperProjectSeqNo == paperProjectSeqNo && a.registrationNo == regisNo.ToString()
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

                var ArchiveDe = from a in bb.gxArchivesDetail
                                where a.registrationNo == RegistrationNo
                                select a;
                gxArchivesDetail archiveDetail = ArchiveDe.First();
                archiveDetail.registrationNo = RegistrationNo;

                if (archiveNo != archiveDetail.archivesNo)
                {
                    var archive = from f in bb.gxArchivesDetail
                                  where f.archivesNo == archiveNo
                                  select f;
                    if (archive.Count() == 0)
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
                archiveDetail.indexDate = Convert.ToDateTime(strbiaoyinriqi);
                archiveDetail.indexer = indexer;//标引员


                string strshenheriqi = checkDate;//审核日期
                archiveDetail.checkDate = Convert.ToDateTime(strshenheriqi);
                archiveDetail.checker = checker;//审核员

                archiveDetail.kaigongTime = kaigongTime.Trim();//开工日期
                archiveDetail.jungongTime = jungongTime.Trim();//竣工日期
                if (fazhaoTime == null)
                {
                    fazhaoTime = "";
                }
                archiveDetail.fazhaoTime = fazhaoTime.Trim();//发照日期
                archiveDetail.jgDate = DateTime.Parse(jgDate.Trim());
                string strlururiqi = TyperDate;//录入日期
                archiveDetail.typerDate = Convert.ToDateTime(strlururiqi);
                archiveDetail.typist = Typist;//录入员  

                archiveDetail.isImageExist = "无";

                if (mapsheetNo == "")
                {
                    archiveDetail.mapsheetNo = "0";
                }
                else
                {
                    archiveDetail.mapsheetNo = mapsheetNo;//图幅号
                }
                if (microNo == "")
                {
                    archiveDetail.microNo = "0";
                }
                else
                {
                    archiveDetail.microNo = microNo;//微缩号
                }




                if (ModelState.IsValid)
                {
                    bb.Entry(archiveDetail).State = EntityState.Modified;

                }

                if (projects != null)
                {
                    if (projects.developmentOrganization != developmentOrganization || projects.constructionOrganization != constructionOrganization || projects.disignOrganization != disignOrganization)
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
                    projects.structureTypeID = structureTypeID.ToString().Trim();
                    projects.newLocation = newLocation.Trim();
                    if (ModelState.IsValid)
                    {
                        bb.Entry(projects).State = EntityState.Modified;


                    }
                }
                if (paperArchives != null)
                {
                    string jinguandata = Convert.ToDateTime(paperArchives.jgDate).ToString("yyyy-MM-dd");
                    if (jinguandata != jgDate.Trim())
                    {
                        paperArchives.jgDate = DateTime.Parse(jgDate.Trim());
                    }
                    paperArchives.structureTypeID = structureTypeID.ToString().Trim();
                    if(buildingArea==""|| buildingArea==null)
                    {
                        paperArchives.buildingArea = 0;
                    }
                    else
                    {
                        paperArchives.buildingArea = Convert.ToDouble((buildingArea.Trim()));
                    }
            
                    paperArchives.underground = underground.Trim();
                    paperArchives.overground = overground.Trim();
                    paperArchives.Fee = Fee;
                    paperArchives.zaojia = zaojia;
                    paperArchives.Material = Material;
                    paperArchives.totallong = totallong;
                    paperArchives.hezai = hezai;
                    if (Material == null)
                    {
                        Material = "";
                    }
                    if (Material.Trim() != "")
                    {
                        paperArchives.Material = Material.Trim();
                    }

                    paperArchives.luruTime = TyperDate.Trim();
                    paperArchives.projectStartDate = kaigongTime.Trim();
                    paperArchives.projectFinishDate = jungongTime.Trim();
                    paperArchives.licenseNo = licenseNo.Trim();
                    paperArchives.licenseDate = fazhaoTime.Trim();
                    if (changeLog == 0)
                    {
                        paperArchives.changeLog = "没有";
                    }
                    if (changeLog == 1)
                    {
                        paperArchives.changeLog = "有";
                    }


                    paperArchives.transferUnit = transferUnit;
                    if (TextMaterial.Trim() != "")
                    {
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
                        bb.Entry(paperArchives).State = EntityState.Modified;
                    }
                }
                bb.SaveChanges();
                ViewData["button5"] = true;
                ViewData["checkname"] = 3;
                //return Content("<script >alert('保存成功！');window.location.href='/gxArchivesEnter/EnterAndSee/?id=" + RegistrationNo.Trim() + "';</script >");
                return Content("<script >alert('保存成功！');window.history.back();</script >");

            }

            if (action == "后一卷")
            {
                string regis = RegistrationNo;
                long re = Int32.Parse(regis);
                int index = regis.IndexOf('0');
                re = re + 1;
                //string regisNo = "";
                //if (index == -1)
                //{
                //    regisNo = re.ToString();
                //}
                //else
                //{
                //    regisNo = "0" + re.ToString();
                //}
                if (regis.ToString() == endregis)
                {
                    var pro = from a in proje
                              where a.paperProjectSeqNo == paperProjectSeqNo
                              orderby a.registrationNo descending
                              select a.registrationNo;
                    string startregisNo = pro.First();
                    return RedirectToAction("EnterAndSee", new { id = startregisNo });
                }
                else
                {
                    var pro = from a in proje
                              where a.paperProjectSeqNo == paperProjectSeqNo && a.registrationNo == regis.ToString()
                              select a.registrationNo;
                    string startregisNo = (long.Parse(pro.First())+1).ToString();
                    startregisNo = startregisNo.PadLeft(pro.First().Length, '0');
                    return RedirectToAction("EnterAndSee", new { id = startregisNo });
                }

            }
            if (action == "末卷")
            {
                var pro = from a in proje
                          where a.paperProjectSeqNo == paperProjectSeqNo
                          orderby a.registrationNo
                          select a;
                var count = pro.Count();
                pro = pro.Take(count-1).OrderByDescending(a=>a.registrationNo);
                string startregisNo = pro.First().registrationNo;
                ViewBag.jiaojiemulu = "display:inline-block";
                ViewBag.jiaojiemulu = "display:inline-block";
                return RedirectToAction("EnterAndSee", new { id = startregisNo });
            }



            return View(proje.First());
        }

        public ActionResult FileList(string id1, int id2, int id)
        {

            if (id1 == "" || id1 == null)
            {
                return Content("<script >alert('该案卷档号为空，请核查！');window.history.back();</script >");
            }
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "文字", Value = "0"},
                new SelectListItem { Text = "图纸", Value = "1"},
                new SelectListItem { Text = "文字及图纸", Value = "2" },
                };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text", 0);
            var file3 = from a in bb.gxFileInfo
                        where a.archivesNo == id1.Trim()
                        orderby a.seqNo
                        select a;
            var registion = from g in bb.vw_gxpassList
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
            }
            ViewBag.Edit = "display:none";
            ViewBag.add = "display:none";
            //id2为1的时候有上方添加案卷的视图
            if (id2 == 1)
            {
                string str3 = id1.Trim();



                var file = from a in bb.gxFileInfo
                           where a.archivesNo == str3
                           orderby a.seqNo descending
                           select a;

                var file1 = from b in bb.gxFileInfo
                            where b.archivesNo == str3
                            orderby b.endPageNo descending
                            select b;

                gxFileInfo file2 = new gxFileInfo();
                if (file.Count() == 0)
                {
                    file2.seqNo = 1;
                    file2.startPageNo = "1";
                }
                else
                {
                    file2.seqNo = file.First().seqNo + 1;
                    file2.startPageNo = (file.First().endPageNo + 1).ToString();
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
                ViewBag.id = bb.gxFileInfo.Max(a => a.ID) + 1;
                if (file.Count() == 0)
                {
                    var registion1 = from g in bb.vw_gxpassList
                                    where g.archivesNo == id1.Trim()
                                    select g;
                    //if(registion1.First().classifyID.Trim()=="1")
                    //{
                    //    ViewBag.fileName = "地下管线竣工测量工程";
                    //}
                    //if (registion1.First().classifyID.Trim() == "2")
                    //{
                    //    ViewBag.fileName = "测量成果表";
                    //}
                    ViewBag.fileName = "";
                    ViewBag.responsible = "";
                    //ViewBag.responsible = registion1.First().MapOrginisation;
                }
                else
                {
                    var registion1 = from g in bb.vw_gxpassList
                                     where g.archivesNo == id1.Trim()
                                     select g;
                    //ViewBag.fileName = "地下管线竣工测量图";
                    ViewBag.fileName = "";
                    ViewBag.SelectedID = new SelectList(list, "Value", "Text",1);
                    //ViewBag.responsible = registion1.First().MapOrginisation;
                    ViewBag.responsible = "";
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
                var file = from ad in bb.gxFileInfo
                           where (ad.ID == id)
                           select ad;
                string a = file.First().archivesNo;
                var file2 = from ac in bb.gxFileInfo
                            where ac.archivesNo == a
                            orderby ac.seqNo
                            select ac;
                var f1 = file2.First();
                int seq1 = Convert.ToInt32(f1.seqNo);
                int seq2 = file2.Count();
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
                ViewBag.fileName = file.First().fileName;
                ViewBag.responsible = file.First().responsible;
                ViewBag.startDate = file.First().startDate;
                ViewBag.remarks = file.First().remarks;
                ViewBag.startPageNo = file.First().startPageNo;
                ViewBag.seqNo = file.First().seqNo;
                ViewBag.fileNo = file.First().fileNo;
                ViewBag.type = file.First().type;
                ViewBag.id = file.First().ID;
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

            ViewBag.result = JsonConvert.SerializeObject(file3.ToList());
            return View();


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FileList(string ArchiveNo, string action)
        {
            var file = from a in bb.gxFileInfo
                       where a.archivesNo == ArchiveNo.Trim()
                       orderby a.seqNo
                       select a;
            var file1 = from b in bb.gxArchivesDetail
                        where b.archivesNo == ArchiveNo.Trim()
                        select b.paperProjectSeqNo;
            string id = Request.Form["id"];
            if (action == "添加")
            {
                return RedirectToAction("FileList", new { id1 = ArchiveNo.Trim(), id = 0, id2 = 1 });
            }
            if (action == "返回案卷信息")
            {
                var list = bb.gxPaperArchives.Where(a => a.startArchiveNo == ArchiveNo);
                if(list.Count() != 0 && list.First().endArchiveNo==null)
                {
                    return RedirectToAction("Enter", new { id = file1.First() ,id2=1});
                }
                if (list.Count() != 0 && list.First().endArchiveNo != null)
                {
                    return RedirectToAction("Enter", new { id = file1.First(), id2 = 0 });
                }
                if (list.Count() == 0 || list.First().endArchiveNo != null)
                {
                    return RedirectToAction("Enter", new { id = file1.First(), id2 = 0 });
                }
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
                return RedirectToAction("juanneimulu", "gxArchivesEnter", new { myid = ID, format = "PDF" });
            }
            var file2 = from a in bb.gxArchivesDetail
                        where a.archivesNo == ArchiveNo
                        select a.registrationNo;

            int index = ArchiveNo.IndexOf('.');
            int index1 = index + 1;
            string str1 = ArchiveNo.Substring(0, index + 1);
            string str2 = ArchiveNo.Substring(index + 1, ArchiveNo.Length - 1 - index);
            gxFileInfo file3 = new gxFileInfo();

            file3.ID = db.FileInfo.Max(a => a.id) + 1;
            file3.seqNo = int.Parse(Request.Form["seqNo"]);
            if (Request.Form["SelectedID"] != null && Request.Form["SelectedID"] != "")
            {
                switch (Request.Form["SelectedID"])
                {
                    case "0":
                        file3.type = "文字";
                        break;
                    case "1":
                        file3.type = "图纸";
                        break;
                    case "2":
                        file3.type = "文字及图纸";
                        break;


                }

            }

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
                    bb.gxFileInfo.Add(file3);
                    bb.SaveChanges();
                    return Content("<script >alert('添加成功！');window.location.href='/gxArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + id + "&id2=1" + "';</script >");
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
                return Content("<script >alert('删除成功！');window.location.href='/gxArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + id + "&id2=1" + "';</script >");
            }

            if (file == null)
            {
                return HttpNotFound();
            }
            return View();

        }
        public ActionResult Edit(long? id, string archivesNo)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var file = from ad in bb.gxFileInfo
                       where (ad.ID == id)
                       select ad;
            string a = file.First().archivesNo;
            var file2 = from ac in bb.gxFileInfo
                        where ac.archivesNo == a
                        orderby ac.seqNo
                        select ac;
            var f1 = file2.First();
            int seq1 = Convert.ToInt32(f1.seqNo);
            int seq2 = file2.Count();
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
            ViewBag.fileName = file.First().fileName;
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
    public ActionResult Edit(int id,int seqNo,string type,string fileNo,string fileName1,string responsible1,string startPageNo,string startDate,string remarks,string action,string archivesNo,string SelectedID)
     {
        
        var file = from ad in bb.gxFileInfo
                   where ad.ID == id
                   select ad;
            ViewData["startPageNo"] = file.First().startPageNo;
            //ViewData["endPageNo"] = file.First().endPageNo;
            ViewData["startDate"] = file.First().startDate;
            //ViewData["endDate"] = file.First().endDate;
            ViewBag.fileName = file.First().fileName;
            ViewBag.responsible = file.First().responsible;
            gxFileInfo fileinfo = bb.gxFileInfo.Where(a=>a.ID==id).First();
        var archiveno = from ac in bb.gxFileInfo
                            where ac.ID == id
                            select ac.archivesNo;
            string d = archiveno.First();
            var file2 = from ab in bb.gxFileInfo
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
            int seq2 = Convert.ToInt32(file2.Count());
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
                fileinfo.seqNo = seqNo;

                if (type != null && type != "")
                {
                    fileinfo.type = type;
                }
                else if (SelectedID != null && SelectedID != "")
                {
                    switch (SelectedID)
                    {
                        case "0":
                            fileinfo.type = "文字";
                            break;
                        case "1":
                            fileinfo.type = "图纸";
                            break;
                        case "2":
                            fileinfo.type = "文字及图纸";
                            break;
                    }
                }
                else {
                    fileinfo.type = "文字";
                }
                
                fileinfo.fileNo = fileNo;
                fileinfo.fileName = fileName1;
                fileinfo.responsible = responsible1;
                fileinfo.startDate = startDate;
                fileinfo.startPageNo = startPageNo;
                fileinfo.remarks = remarks;
                if (ModelState.IsValid)
                {
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
                    bb.Entry(fileinfo).State = EntityState.Modified;
                    bb.SaveChanges();
                    return Content("<script>alert('已成功修改！');window.location.href='/gxArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + id + "&id2=2" + "'</script>");
                }
            }
            //if(action=="返回")
            //{
            //    //if (index1 ==0)
            //    //  {
            //    //      str1 = "";

            //    //      return RedirectToAction("FileList", new { id1 = str1, id2 = str2,id3=index1 });
            //    //  }
            //    return RedirectToAction("FileList", new { id1 = archivesNo}); 
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
                   var file3 = from ac in bb.gxFileInfo
                            where ac.seqNo == seq3 && ac.archivesNo==c.archivesNo
                            select ac;
                    return Content("<script>window.location.href='/gxArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + file3.First().ID + "&id2=2" + "';</script >"); ;
                    //return RedirectToAction("FileList", new { id1 = archivesNo, id = file3.First().ID, id2 = 2 });
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
                    var file4 = from ac in bb.gxFileInfo
                                where ac.seqNo == seq4 && ac.archivesNo == c.archivesNo
                                select ac;
                    return Content("<script>window.location.href='/gxArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + file4.First().ID + "&id2=2" + "';</script >"); ;
                    //return RedirectToAction("FileList", new { id1 = archivesNo, id = file4.First().ID, id2 = 2 });
                    
                }
            }
            if (action == "删除词条")
            {
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
                return Content("<script>alert('删除成功！');window.location.href='/gxArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + id + "&id2=2" + "';</script >"); ;
            }
            return View(file.First());
      } 
        public ActionResult Delete(long id,string id2)
        {
            var file = from ad in bb.gxFileInfo
                       where ad.ID == id
                       select ad;

            gxFileInfo fileinfo = file.First();
            string archivesNo = file.First().archivesNo;
            bb.Entry(fileinfo).State = EntityState.Deleted;
            bb.SaveChanges();
            /*href='/ArchivesEnter/FileList'*/
            //return JavaScript("删除成功！");
            //return Content("<script >alert('该联系单缺失工程信息！请重新录入工作联系单！');window.location.href='/VideoContractSheets/Index';</script >");
            return Content("<script>alert('已成功删除！');window.location.href='/gxArchivesEnter/FileList?id1=" + archivesNo + "&id=" + id + "&id2=0" + "';</script>");

           
        }
        public  ActionResult Details(long id,string archivesNo)
        {
            var file = from ad in bb.gxFileInfo
                       where (ad.ID == id)
                       select ad;
            gxFileInfo fileinfo = file.First();
            ViewData["archivesNo"] = archivesNo;
            return View(fileinfo);
        }
        [HttpPost]
        public ActionResult Details( string archivesNo)
        {
            //return Redirect("/ArchivesEnter/FileList/?id="+ id2);
            //案卷列表
            return RedirectToAction("FileList",new{id1= archivesNo,id2 = 0,id = 0});
        }

        public ActionResult Create(string  ID3)
        {
            ViewData["button2"] = true;
            string str3 = ID3.Trim();



            var file = from a in bb.gxFileInfo
                       where a.archivesNo == str3
                       orderby a.seqNo descending
                       select a;

            var file1 = from b in bb.gxFileInfo
                        where b.archivesNo == str3
                        orderby b.endPageNo descending
                        select b;

            gxFileInfo file2 = new gxFileInfo();
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
            var file = from a in bb.gxArchivesDetail
                       where a.archivesNo == archivesNo
                       select a.registrationNo;
            string ArchiveNo = archivesNo;
            int index = ArchiveNo.IndexOf('.');
            int index1 = index + 1;
            string str1 = ArchiveNo.Substring(0, index+1);
            string str2 = ArchiveNo.Substring(index + 1, ArchiveNo.Length - 1 - index);
            gxFileInfo file3 = new gxFileInfo();
            file3.ID =Convert.ToInt32(id + 1);
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
                    bb.gxFileInfo.Add(file3);

                    bb.SaveChanges();
                    //Response.Write("<script>alert('添加成功！');window.history.back();</script>");
                    return Content("<script>alert('添加成功！');window.location.href='/gxArchivesEnter/FileList/?id1=" + archivesNo + "';</script>");
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
            var paperArchive = from a in bb.gxPaperArchives
                               where a.projectID == id
                               select a;
            gxPaperArchives paper = paperArchive.First();
            //long id1 =Convert.ToInt32(paper.projectID);
            var project = from b in bb.gxProjectInfo
                          where b.projectID == id
                          select b;
            gxProjectInfo pro = project.First();
            paper.dateArchive = DateTime.Today.Date;
            pro.status = "10";
            bb.Entry(paper).State = EntityState.Modified;
            bb.Entry(pro).State = EntityState.Modified;
           
            
                bb.SaveChanges();
            if(paper.boxNo!=null)
            {
                return RedirectToAction("ArchiveEnterNB");
            }
           return RedirectToAction("ArchiveEnter");
        }
        public ActionResult WaitStorage(string SearchString)
        {
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
            string n = Request.Form["SelectedID"];

            ViewBag.CurrentFilter = SearchString;
            string user = User.Identity.Name;
            var depart = from c in ab.AspNetUsers
                         where c.UserName == user
                         select c;
            string name = depart.First().UserName;
            var vwprojectlist = from ad in bb.vw_gxprojectList
                                where ad.status == "10"
                                where ad.isNB == "外部"
                                select ad;
            if (name != "业务科")
            {
                vwprojectlist = from ad in bb.vw_gxprojectList
                                where ad.status == "10"
                                where ad.collator == name
                                where ad.isNB == "外部"
                                select ad;
            }

            if (!String.IsNullOrEmpty(SearchString))
            {
                int t = int.Parse(n);
                ViewBag.SelectedID = new SelectList(list, "Value", "Text",n);

                switch (t)
                {
                    
                    case 0:
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 1:
                        long search = Convert.ToInt32(SearchString);
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectNo == search);//根据地点搜索
                        break;
                    case 2:
                        vwprojectlist = vwprojectlist.Where(ad => ad.disignOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwprojectlist = vwprojectlist.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                        break;
                    case 4:
                        vwprojectlist = vwprojectlist.Where(ad => ad.paperProjectSeqNo.ToString().Trim() == SearchString);//根据工程序号搜索
                        break;
                    case 5:
                        vwprojectlist = vwprojectlist.Where(ad => ad.startPaijiaNo == SearchString);
                        break;
                    case 6:
                        vwprojectlist = vwprojectlist.Where(ad => ad.startArchiveNo == SearchString);//根据责任书编号搜索
                        break;
                }

            }



            vwprojectlist = vwprojectlist.OrderByDescending(s => s.paperProjectSeqNo);// 默认按项目顺序号排列
            ViewBag.result = JsonConvert.SerializeObject(vwprojectlist);
            return View();

        }
        public ActionResult WaitStorageNB(string SearchString)
        {
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
            string n = Request.Form["SelectedID"];

            ViewBag.CurrentFilter = SearchString;
            string user = User.Identity.Name;
            var depart = from c in ab.AspNetUsers
                         where c.UserName == user
                         select c;
            string name = depart.First().UserName;
            var vwprojectlist = from ad in bb.vw_gxprojectList
                                where ad.status == "10"
                                where ad.isNB=="内部"
                                select ad;
            if (name != "业务科")
            {
                vwprojectlist = from ad in bb.vw_gxprojectList
                                where ad.status == "10"
                                where ad.collator == name
                                where ad.isNB == "内部"
                                select ad;
            }

            if (!String.IsNullOrEmpty(SearchString))
            {
                int t = int.Parse(n);
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", n);

                switch (t)
                {

                    case 0:
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 1:
                        long search = Convert.ToInt32(SearchString);
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectNo == search);//根据地点搜索
                        break;
                    case 2:
                        vwprojectlist = vwprojectlist.Where(ad => ad.disignOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwprojectlist = vwprojectlist.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                        break;
                    case 4:
                        vwprojectlist = vwprojectlist.Where(ad => ad.paperProjectSeqNo.ToString().Trim() == SearchString);//根据工程序号搜索
                        break;
                    case 5:
                        vwprojectlist = vwprojectlist.Where(ad => ad.startPaijiaNo == SearchString);
                        break;
                    case 6:
                        vwprojectlist = vwprojectlist.Where(ad => ad.startArchiveNo == SearchString);//根据责任书编号搜索
                        break;
                }

            }



            vwprojectlist = vwprojectlist.OrderByDescending(s => s.paperProjectSeqNo);// 默认按项目顺序号排列
            ViewBag.result = JsonConvert.SerializeObject(vwprojectlist);
            return View();

        }
        public ActionResult EnterStorage(long ?id)
        {
            var paperArchive = from a in bb.gxPaperArchives
                               where a.projectID == id
                               select a;
            gxPaperArchives paper = paperArchive.First();
            long id1 = Convert.ToInt32(paper.projectID);
            var project = from b in bb.gxProjectInfo
                          where b.projectID == id
                          select b;
            gxProjectInfo pro = project.First();
            paper.dateArchive = DateTime.Today.Date;
            pro.status = "7";
            bb.Entry(paper).State = EntityState.Modified;
            bb.Entry(pro).State = EntityState.Modified;
            bb.SaveChanges();
            if (paper.boxNo != null)
            {
                return RedirectToAction("WaitStorageNB");
            }
            return RedirectToAction("WaitStorage");

        }

        private string generatefollowByMaxModel2(MaxArchiveRegisNo1 maxModel, string strClassNo, int ArchivesCount)
        {
            //add by 周林,date:2016.12.13  
            int nB2 = 1 + strClassNo[0] - 'A';
            string strB2 = "";
            if (nB2 < 10)
            {
                strB2 = "0" + nB2.ToString();
            }
            else
            {
                strB2 = nB2.ToString();
            }
            
                string strStart1 = maxModel.maxRegisNo.Trim();
                string strStart = strStart1.Substring(2,strStart1.Length - 2);
                string strEnd = "";
                suanhao(ref strStart, ref strEnd, ArchivesCount);
            //更新最大登记号
            string strLast6Bit = strStart;
            var Maxdel = from c in bb.MaxArchiveRegisNo1
                         where c.ID > 0
                         select c;
            foreach (var item in Maxdel)
            {

                if (item.mainCategoryID.Trim() == strClassNo.Trim())
                {
                   
                    item.mainCategoryID = strClassNo.Trim();
                    item.maxArchiveNo =strStart;
                    

                }
                else
                {

                    string curRegisNo = item.maxRegisNo;
                    item.maxRegisNo = curRegisNo.Substring(0, 2) + strLast6Bit;

                }

                bb.Entry(item).State = EntityState.Modified;


            }

            ab.SaveChanges();


            return strStart1;
        }
        private void suanhao(ref string strstart, ref string strend, int ArchivesCount)
        {
            int len = 0; ;
            string str0len = "";
            int flag = -1;
            int temp;
            if (Int32.Parse(strstart) == 0)
            {
                flag = 0;//字符串为全0
            }
            else
            {
                for (len = 0; len < strstart.Length; len++)
                {
                    if (strstart[0] != '0')
                    {
                        flag = 2;//字符串前面没0
                        break;
                    }
                    if (strstart[len] == '0' && strstart[len + 1] != '0')
                    {
                        flag = 1; break;//字符串前面有0，非全0
                    }
                }
            }
            if (flag == 1)
            {
                len++;
                int lentemp;
                temp = Int32.Parse(strstart.Substring(len, strstart.Length - len));

                lentemp = temp.ToString().Length;
                temp += 1;
                int lentemp2 = temp.ToString().Length;
                int len21 = lentemp2 - lentemp;

                for (int i = 0; i < len - len21; i++)
                    str0len += "0";
                str0len += temp.ToString();
                strstart = str0len;

                lentemp = temp.ToString().Length;
                temp += ArchivesCount - 1;
                lentemp2 = temp.ToString().Length;
                int len22 = lentemp2 - lentemp;
                str0len = "";
                if (len <= len21 + len22)
                {
                    strend = temp.ToString();
                }
                else
                {
                    for (int i = 0; i < len - len21 - len22; i++)
                        str0len += "0";
                    strend = str0len + temp.ToString();
                }
            }
            else if (flag == 2)
            {

                temp = Int32.Parse(strstart) + 1;
                strstart = temp.ToString();
                temp += ArchivesCount - 1;
                strend = temp.ToString();
            }
            else//flag=0
            {
                len = strstart.Length;
                for (int i = 0; i < len - 1; i++)
                {
                    str0len += "0";
                }
                strstart = str0len + "1";
                str0len += "0";
                strend = ArchivesCount.ToString(str0len);
            }
        }
    }
}