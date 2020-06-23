using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using urban_archive.Models;
using Microsoft.Reporting.WebForms;



namespace urban_archive.Controllers
{
    public class DaYinController : Controller
    {
        private OfficeEntities db = new OfficeEntities();
        private UrbanConEntities ab = new UrbanConEntities();
        private VideoArchiveEntities cb = new VideoArchiveEntities();
        private PlanArchiveEntities bb = new PlanArchiveEntities();
        private UrbanConEntities db1 = new UrbanConEntities();
        // GET: DaYin
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult ZTree()
        {
            return View();
        }

        public ActionResult ZhuludanDanXiang(string action, string type = "PDF")   //打印工程项目著录单（单项）
        {
            if (action == "打印工程项目著录单（单项）")
            {
                LocalReport localReport = new LocalReport();
                string SS = Request.Form["SeqNoS"];
                string SE = Request.Form["SeqNoE"];
                if (SS == "" && SE != "")
                {
                    Response.Write("<script>alert('起始项目顺序号不能为空!');</script>");
                }
                if (SS != "" && SE == "")
                {
                    Response.Write("<script>alert('终止项目顺序号不能为空!');</script>");
                }
                if (SS == "" && SE == "")
                {
                    Response.Write("<script>alert('请输入项目顺序号范围!');</script>");
                }
                if (SS != "" && SE != "")
                {
                    long SeqNoS = long.Parse(Request.Form["SeqNoS"]);
                    long SeqNoE = long.Parse(Request.Form["SeqNoE"]);
                    var ds1 = db.vw_projectTypelist.Where(ad => ad.paperProjectSeqNo >= SeqNoS).Where(ad => ad.paperProjectSeqNo <= SeqNoE).OrderBy(ad => ad.paperProjectSeqNo);
                    List<vw_projectTypelist> list = ds1.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].mainCategoryID != null)
                            list[i].mainCategoryID = list[i].startArchiveNo.Split('-').First();
                        if (list[i].projectName != null)
                            list[i].projectName = list[i].projectName.Trim();
                        if (list[i].developmentOrganization != null)
                            list[i].developmentOrganization = list[i].developmentOrganization.Trim();
                        if (list[i].location != null)
                            list[i].location = list[i].location.Trim();
                        if (list[i].AlicenseNo != null)
                            list[i].AlicenseNo = list[i].AlicenseNo.Trim();
                        if (list[i].notearea != null)
                            list[i].notearea = list[i].notearea.Replace("\n", " ");
                        if (list[i].remarks != null)
                            list[i].remarks = list[i].remarks.Replace("\n", " ");
                    }
                    var ds2 = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/ZhuludanDanXiang.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("ZhuludanDanXiang", ds2);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult DangAnTongJi(string action, string type = "PDF")    //生成档案统计表
        {
            if (action == "生成档案统计表")
            {
                LocalReport localReport = new LocalReport();
                string dateS = Request.Form["startdata"];
                string dateE = Request.Form["enddata"];
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                //征地档案
                var zhengdi = ab.zdArchive.Where(ad => ad.luruDate >= DataFrom).Where(ad => ad.luruDate <= DataTo);
                int zdProCnt = zhengdi.Count();        //征地总工程数
                List<zdArchive> list = zhengdi.ToList();
                int? zdArchThick = 0;
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].ArchiveThick == null)
                    {
                        list[i].ArchiveThick = 0;
                    }
                    zdArchThick += list[i].ArchiveThick;     //征地案卷厚度
                }
                //执照档案
                var zhizhao = db.OtherArchives.Where(ad => string.Compare(ad.luruTime, dateS) >= 0).Where(ad => string.Compare(ad.luruTime, dateE) <= 0).Where(ad => ad.classTypeID == 1);
                int zhizhaoProCnt = zhizhao.Count();   //执照总工程数
                List<OtherArchives> list1 = zhizhao.ToList();
                int? zhizhaoArchThick = 0;
                for (int i = 0; i < list1.Count(); i++)
                {
                    if (list1[i].ArchiveThick == null)
                    {
                        list1[i].ArchiveThick = 0;
                    }
                    zhizhaoArchThick += list1[i].ArchiveThick;   //执照案卷厚度
                }
                //道路档案
                var daolu = db.OtherArchives.Where(ad => string.Compare(ad.luruTime, dateS) >= 0).Where(ad => string.Compare(ad.luruTime, dateE) <= 0).Where(ad => ad.classTypeID == 2);
                int daoluProCnt = daolu.Count();   //道路总工程数
                List<OtherArchives> list2 = daolu.ToList();
                int? daoluArchThick = 0;
                for (int i = 0; i < list2.Count(); i++)
                {
                    if (list2[i].ArchiveThick == null)
                    {
                        list2[i].ArchiveThick = 0;
                    }
                    daoluArchThick += list2[i].ArchiveThick;   //道路案卷厚度
                }
                //分类档案
                var fenlei = db.OtherArchives.Where(ad => string.Compare(ad.luruTime, dateS) >= 0).Where(ad => string.Compare(ad.luruTime, dateE) <= 0).Where(ad => ad.classTypeID == 3);
                int fenleiProCnt = fenlei.Count();   //分类总工程数
                List<OtherArchives> list3 = fenlei.ToList();
                int? fenleiArchThick = 0;
                for (int i = 0; i < list3.Count(); i++)
                {
                    if (list3[i].ArchiveThick == null)
                    {
                        list3[i].ArchiveThick = 0;
                    }
                    fenleiArchThick += list3[i].ArchiveThick;   //分类案卷厚度
                }
                //声像档案
                var shengxiang = cb.VideoArchives.Where(ad => ad.dateReceived >= DataFrom).Where(ad => ad.dateReceived <= DataTo); ;
                int shengxiangProCnt = shengxiang.Count();   //声像总工程数
                List<VideoArchives> list4 = shengxiang.ToList();
                int? videoArchCnt = 0;
                int? photoArchCnt = 0;
                for (int i = 0; i < list4.Count(); i++)
                {
                    videoArchCnt += int.Parse(list4[i].videoCassetteBoxCount);   //视频案卷数
                    photoArchCnt += int.Parse(list4[i].photoBoxCount);      //照片案卷数
                }
                //规划档案
                var guihua = bb.PlanProject.Where(ad => ad.dateReceived >= DataFrom).Where(ad => ad.dateReceived <= DataTo);
                int guihuaProCnt = guihua.Count();   //规划总工程数
                int? NoS = guihua.Min(d => d.seqNo);
                int? NoE = guihua.Max(d => d.seqNo);
                var guihuabox = bb.PlanArchiveBox.Where(ad => ad.ID >= NoS).Where(ad => ad.ID <= NoE);
                int guihuaArchCnt = guihuabox.Count();  //规划案卷数
                List<PlanArchiveBox> list5 = guihuabox.ToList();
                int? guihuaArchThick = 0;
                for (int i = 0; i < list5.Count(); i++)
                {
                    if (list5[i].ArchiveThick == null)
                    {
                        list5[i].ArchiveThick = 0;
                    }
                    guihuaArchThick += list5[i].ArchiveThick;   //规划案卷厚度
                }
                //竣工档案
                var jungong = ab.PaperArchives.Where(ad => ad.dateReceived >= DataFrom).Where(ad => ad.dateReceived <= DataTo);
                int jungongProCnt = jungong.Count();   //竣工总工程数
                var jungonganjuan = from a in ab.PaperArchives
                                    join b in ab.ArchivesDetail
                                    on a.paperProjectSeqNo equals b.paperProjectSeqNo
                                    where a.dateReceived >= DataFrom
                                    where a.dateReceived <= DataTo
                                    select b;
                int jungongArchCnt = jungonganjuan.Count();  //竣工案卷数
                List<ArchivesDetail> list6 = jungonganjuan.ToList();
                long? jungongArchThick = 0;
                for (int i = 0; i < list6.Count(); i++)
                {
                    if (list6[i].archiveThickness == null)
                    {
                        list6[i].archiveThickness = 0;
                    }
                    jungongArchThick += list6[i].archiveThickness;   //竣工案卷厚度
                }
                //总计
                int totalProCnt = zdProCnt + zhizhaoProCnt + daoluProCnt + fenleiProCnt + shengxiangProCnt + guihuaProCnt + jungongProCnt;
                long? totalArchThick = zdArchThick + zhizhaoArchThick + daoluArchThick + fenleiArchThick + guihuaArchThick + jungongArchThick;
                int? totalArchCnt = jungongArchCnt + guihuaArchCnt + photoArchCnt + videoArchCnt;
                localReport.ReportPath = Server.MapPath("~/Report/Office/DangAnTongJi.rdlc");
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdProCnt", zdProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchThick", zdArchThick.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoProCnt", zhizhaoProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchThick", zhizhaoArchThick.ToString().Trim()));
                parameterList.Add(new ReportParameter("daoluProCnt", daoluProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("daoluArchThick", daoluArchThick.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiProCnt", fenleiProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchThick", fenleiArchThick.ToString().Trim()));
                parameterList.Add(new ReportParameter("shengxiangProCnt", shengxiangProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("videoArchCnt", videoArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("photoArchCnt", photoArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaArchCnt", guihuaArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaProCnt", guihuaProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaArchThick", guihuaArchThick.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongProCnt", jungongProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongArchCnt", jungongArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongArchThick", jungongArchThick.ToString().Trim()));
                parameterList.Add(new ReportParameter("totalProCnt", totalProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("totalArchThick", totalArchThick.ToString().Trim()));
                parameterList.Add(new ReportParameter("totalArchCnt", totalArchCnt.ToString().Trim()));
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult GuiHuaFengPi(string action, string type = "PDF")
        {
            if (action == "打印案卷（工程）封皮")
            {
                LocalReport localReport = new LocalReport();
                string SS = Request.Form["SeqNoS"];
                string SE = Request.Form["SeqNoE"];
                if (SS == "" && SE != "")
                {
                    Response.Write("<script>alert('起始项目顺序号不能为空!');</script>");
                }
                if (SS != "" && SE == "")
                {
                    Response.Write("<script>alert('终止项目顺序号不能为空!');</script>");
                }
                if (SS == "" && SE == "")
                {
                    Response.Write("<script>alert('请输入项目顺序号范围!');</script>");
                }
                if (SS != "" && SE != "")
                {

                    string SeqNoS = SS.PadLeft(8, '0');
                    string SeqNoE = SE.PadLeft(8, '0'); ;
                    var ds1 = db.View_PlanProjectWithBox.Where(ad => string.Compare(ad.totalSeqNo, SeqNoS) >= 0).Where(ad => string.Compare(ad.totalSeqNo, SeqNoE) <= 0);

                    localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaAnJuanFengPi.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("GuiHuaAnJuanFengPi", ds1);
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
            if (action == "打印工程封皮")
            {
                LocalReport localReport = new LocalReport();
                string SS = Request.Form["SeqNoS"];
                string SE = Request.Form["SeqNoE"];
                if (SS == "" && SE != "")
                {
                    Response.Write("<script>alert('起始项目顺序号不能为空!');</script>");
                }
                if (SS != "" && SE == "")
                {
                    Response.Write("<script>alert('终止项目顺序号不能为空!');</script>");
                }
                if (SS == "" && SE == "")
                {
                    Response.Write("<script>alert('请输入项目顺序号范围!');</script>");
                }
                if (SS != "" && SE != "")
                {

                    string SeqNoS = SS.PadLeft(8, '0');
                    string SeqNoE = SE.PadLeft(8, '0'); ;
                    var ds1 = db.vw_PlanProjectList.Where(ad => string.Compare(ad.totalSeqNo, SeqNoS) >= 0).Where(ad => string.Compare(ad.totalSeqNo, SeqNoE) <= 0);
                    List<vw_PlanProjectList> list = ds1.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].fileNo != null)
                            list[i].fileNo = list[i].fileNo.Trim();
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengFengPi.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("GongChengFengPi", ds1);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult FenLeiDangAnAnJuan(string action, string type = "PDF")
        {
            List<SelectListItem> LIST = new List<SelectListItem> {
                new SelectListItem { Text = "全部", Value = "-1"},
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1"},
            };
            ViewBag.Class = new SelectList(LIST, "Value", "Text");

            if (action == "打印分类档案（精确）")
            {

                LocalReport localReport = new LocalReport();
                string ANoS = Request.Form["archiveNoS"];
                string ANoE = Request.Form["archiveNoE"];
                int choose = int.Parse(Request.Form["Class"]);
                if (ANoS == "" && ANoE != "")
                {
                    Response.Write("<script>alert('起始档案号不能为空!');</script>");
                }
                if (ANoS != "" && ANoE == "")
                {
                    Response.Write("<script>alert('终止档案号不能为空!');</script>");
                }
                if (ANoS == "" && ANoE == "")
                {
                    Response.Write("<script>alert('请输入档案号范围!');</script>");
                }
                if (ANoS != "" && ANoE != "")
                {
                    if (choose == -1)
                    {
                        string title = "分类档案案卷目录（全部）";
                        var temp = db.vw_ClassArchives.Where(ad => string.Compare(ad.archiveNo, ANoS) >= 0);
                        var temp1 = temp.Where(ad => string.Compare(ad.archiveNo, ANoE) <= 0).OrderBy(ad => ad.archiveNo);
                        List<vw_ClassArchives> list = temp1.ToList();
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].archiveTitle != null)
                                list[i].archiveTitle = list[i].archiveTitle.Trim();
                            if (list[i].archiveNo != null)
                                list[i].archiveNo = list[i].archiveNo.Trim();
                            if (list[i].bianzhiUnit != null)
                                list[i].bianzhiUnit = list[i].bianzhiUnit.Trim();
                            if (list[i].measureUnit != null)
                                list[i].measureUnit = list[i].measureUnit.Trim();
                        }
                        var ds = list;
                        localReport.ReportPath = Server.MapPath("~/Report/Office/FenLeiDangAnAnJuan.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("FenLei", ds);
                        localReport.DataSources.Add(reportDataSource);
                        List<ReportParameter> parameterList = new List<ReportParameter>();
                        parameterList.Add(new ReportParameter("title", title));
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
                    if (choose == 0)
                    {
                        string title = "分类档案案卷目录";
                        var temp = db.vw_ClassArchives.Where(ad => string.Compare(ad.archiveNo, ANoS) >= 0);
                        var temp1 = temp.Where(ad => string.Compare(ad.archiveNo, ANoE) <= 0).Where(ad => ad.isJungongArch == 0).OrderBy(ad => ad.archiveNo);
                        List<vw_ClassArchives> list = temp1.ToList();
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].archiveTitle != null)
                                list[i].archiveTitle = list[i].archiveTitle.Trim();
                            if (list[i].archiveNo != null)
                                list[i].archiveNo = list[i].archiveNo.Trim();
                            if (list[i].bianzhiUnit != null)
                                list[i].bianzhiUnit = list[i].bianzhiUnit.Trim();
                            if (list[i].measureUnit != null)
                                list[i].measureUnit = list[i].measureUnit.Trim();
                        }
                        var ds = list;
                        localReport.ReportPath = Server.MapPath("~/Report/Office/FenLeiDangAnAnJuan.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("FenLei", ds);
                        localReport.DataSources.Add(reportDataSource);
                        List<ReportParameter> parameterList = new List<ReportParameter>();
                        parameterList.Add(new ReportParameter("title", title));
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
                    if (choose == 1)
                    {
                        string title = "分类竣工档案案卷目录";
                        var temp = db.vw_ClassArchives.Where(ad => string.Compare(ad.archiveNo, ANoS) >= 0);
                        var temp1 = temp.Where(ad => string.Compare(ad.archiveNo, ANoE) <= 0).Where(ad => ad.isJungongArch == 1).OrderBy(ad => ad.archiveNo);
                        List<vw_ClassArchives> list = temp1.ToList();
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].archiveTitle != null)
                                list[i].archiveTitle = list[i].archiveTitle.Trim();
                            if (list[i].archiveNo != null)
                                list[i].archiveNo = list[i].archiveNo.Trim();
                            if (list[i].bianzhiUnit != null)
                                list[i].bianzhiUnit = list[i].bianzhiUnit.Trim();
                            if (list[i].measureUnit != null)
                                list[i].measureUnit = list[i].measureUnit.Trim();
                        }
                        var ds = list;
                        localReport.ReportPath = Server.MapPath("~/Report/Office/FenLeiDangAnAnJuan.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("FenLei", ds);
                        localReport.DataSources.Add(reportDataSource);
                        List<ReportParameter> parameterList = new List<ReportParameter>();
                        parameterList.Add(new ReportParameter("title", title));
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
                }
            }
            if (action == "打印分类档案（模糊）")
            {

                LocalReport localReport = new LocalReport();
                string ANo = Request.Form["archiveNo"];
                int choose = int.Parse(Request.Form["Class"]);
                if (ANo == "")
                {
                    Response.Write("<script>alert('请输入档案号!');</script>");
                }
                if (ANo != null)
                {
                    if (choose == -1)
                    {
                        string title = "分类档案案卷目录（全部）";
                        var temp = db.vw_ClassArchives.Where(ad => ad.archiveNo.Contains(ANo)).OrderBy(ad => ad.archiveNo);
                        List<vw_ClassArchives> list = temp.ToList();
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].archiveTitle != null)
                                list[i].archiveTitle = list[i].archiveTitle.Trim();
                            if (list[i].archiveNo != null)
                                list[i].archiveNo = list[i].archiveNo.Trim();
                            if (list[i].bianzhiUnit != null)
                                list[i].bianzhiUnit = list[i].bianzhiUnit.Trim();
                            if (list[i].measureUnit != null)
                                list[i].measureUnit = list[i].measureUnit.Trim();
                        }
                        var ds = list;
                        localReport.ReportPath = Server.MapPath("~/Report/Office/FenLeiDangAnAnJuan.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("FenLei", ds);
                        localReport.DataSources.Add(reportDataSource);
                        List<ReportParameter> parameterList = new List<ReportParameter>();
                        parameterList.Add(new ReportParameter("title", title));
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
                    if (choose == 0)
                    {
                        string title = "分类档案案卷目录";
                        var temp = db.vw_ClassArchives.Where(ad => ad.archiveNo.Contains(ANo)).Where(ad => ad.isJungongArch == 0).OrderBy(ad => ad.archiveNo);
                        List<vw_ClassArchives> list = temp.ToList();
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].archiveTitle != null)
                                list[i].archiveTitle = list[i].archiveTitle.Trim();
                            if (list[i].archiveNo != null)
                                list[i].archiveNo = list[i].archiveNo.Trim();
                            if (list[i].bianzhiUnit != null)
                                list[i].bianzhiUnit = list[i].bianzhiUnit.Trim();
                            if (list[i].measureUnit != null)
                                list[i].measureUnit = list[i].measureUnit.Trim();
                        }
                        var ds = list;
                        localReport.ReportPath = Server.MapPath("~/Report/Office/FenLeiDangAnAnJuan.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("FenLei", ds);
                        localReport.DataSources.Add(reportDataSource);
                        List<ReportParameter> parameterList = new List<ReportParameter>();
                        parameterList.Add(new ReportParameter("title", title));
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
                    if (choose == 1)
                    {
                        string title = "分类竣工档案案卷目录";
                        var temp = db.vw_ClassArchives.Where(ad => ad.archiveNo.Contains(ANo)).Where(ad => ad.isJungongArch == 1).OrderBy(ad => ad.archiveNo);
                        List<vw_ClassArchives> list = temp.ToList();
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].archiveTitle != null)
                                list[i].archiveTitle = list[i].archiveTitle.Trim();
                            if (list[i].archiveNo != null)
                                list[i].archiveNo = list[i].archiveNo.Trim();
                            if (list[i].bianzhiUnit != null)
                                list[i].bianzhiUnit = list[i].bianzhiUnit.Trim();
                            if (list[i].measureUnit != null)
                                list[i].measureUnit = list[i].measureUnit.Trim();
                        }
                        var ds = list;
                        localReport.ReportPath = Server.MapPath("~/Report/Office/FenLeiDangAnAnJuan.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("FenLei", ds);
                        localReport.DataSources.Add(reportDataSource);
                        List<ReportParameter> parameterList = new List<ReportParameter>();
                        parameterList.Add(new ReportParameter("title", title));
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
                }
            }
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult FenLeiBiao(string action, string type = "PDF")
        {
            if (action == "打印分类表")
            {
                LocalReport localReport = new LocalReport();
                string SS = Request.Form["SeqNoS"];
                string SE = Request.Form["SeqNoE"];
                if (SS == "" && SE != "")
                {
                    Response.Write("<script>alert('起始项目顺序号不能为空!');</script>");
                }
                if (SS != "" && SE == "")
                {
                    Response.Write("<script>alert('终止项目顺序号不能为空!');</script>");
                }
                if (SS == "" && SE == "")
                {
                    Response.Write("<script>alert('请输入项目顺序号范围!');</script>");
                }
                if (SS != "" && SE != "")
                {
                    long SeqNoS = long.Parse(Request.Form["SeqNoS"]);
                    long SeqNoE = long.Parse(Request.Form["SeqNoE"]);
                    var ds1 = db.vw_archiveClassi.Where(ad => ad.paperProjectSeqNo >= SeqNoS).Where(ad => ad.paperProjectSeqNo <= SeqNoE).OrderBy(ad => ad.paperProjectSeqNo);
                    List<vw_archiveClassi> list = ds1.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {

                        if (list[i].projectName != null)
                            list[i].projectName = list[i].projectName.Replace("\n", "");
                    }
                    var ds2 = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/FenLeiBiao.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("FenLeiBiao", ds2);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult FenLeiDangAn(string action, string type = "PDF")
        {
            if (action == "打印分类档案（精确）")
            {
                LocalReport localReport = new LocalReport();
                string ANoS = Request.Form["archiveNoS"];
                string ANoE = Request.Form["archiveNoE"];
                if (ANoS == "" && ANoE != "")
                {
                    Response.Write("<script>alert('起始档案号不能为空!');</script>");
                }
                if (ANoS != "" && ANoE == "")
                {
                    Response.Write("<script>alert('终止档案号不能为空!');</script>");
                }
                if (ANoS == "" && ANoE == "")
                {
                    Response.Write("<script>alert('请输入档案号范围!');</script>");
                }
                if (ANoS != "" && ANoE != "")
                {
                    var temp = db.vw_ClassArchives.Where(ad => string.Compare(ad.archiveNo, ANoS) >= 0);
                    var temp1 = temp.Where(ad => string.Compare(ad.archiveNo, ANoE) <= 0).OrderBy(ad => ad.archiveNo);
                    List<vw_ClassArchives> list = temp1.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].archiveTitle != null)
                            list[i].archiveTitle = list[i].archiveTitle.Trim();
                        if (list[i].archiveNo != null)
                            list[i].archiveNo = list[i].archiveNo.Trim();
                        if (list[i].bianzhiUnit != null)
                            list[i].bianzhiUnit = list[i].bianzhiUnit.Trim();
                        if (list[i].measureUnit != null)
                            list[i].measureUnit = list[i].measureUnit.Trim();
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/FenLeiDangAn.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("FenLei", ds);
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
            if (action == "打印分类档案（模糊）")
            {
                LocalReport localReport = new LocalReport();
                string ANo = Request.Form["archiveNo"];
                if (ANo == "")
                {
                    Response.Write("<script>alert('请输入档案号!');</script>");
                }
                if (ANo != null)
                {
                    var temp = db.vw_ClassArchives.Where(ad => ad.archiveNo == ANo).OrderBy(ad => ad.archiveNo);
                    List<vw_ClassArchives> list = temp.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].archiveTitle != null)
                            list[i].archiveTitle = list[i].archiveTitle.Trim();
                        if (list[i].archiveNo != null)
                            list[i].archiveNo = list[i].archiveNo.Trim();
                        if (list[i].bianzhiUnit != null)
                            list[i].bianzhiUnit = list[i].bianzhiUnit.Trim();
                        if (list[i].measureUnit != null)
                            list[i].measureUnit = list[i].measureUnit.Trim();
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/FenLeiDangAn.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("FenLei", ds);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult ChengJianBMuLu(string action, string type = "PDF")
        {
            if (action == "打印城建档案B大类目录（精确）")
            {
                LocalReport localReport = new LocalReport();
                string ANoS = Request.Form["archiveNoS"];
                string ANoE = Request.Form["archiveNoE"];
                if (ANoS == "" && ANoE != "")
                {
                    Response.Write("<script>alert('起始档案号不能为空!');</script>");
                }
                if (ANoS != "" && ANoE == "")
                {
                    Response.Write("<script>alert('终止档案号不能为空!');</script>");
                }
                if (ANoS == "" && ANoE == "")
                {
                    Response.Write("<script>alert('请输入档案号范围!');</script>");
                }
                if (ANoS != "" && ANoE != "")
                {
                    var temp = db.vw_ClassArchives.Where(ad => string.Compare(ad.archiveNo, ANoS) >= 0);
                    var temp1 = temp.Where(ad => string.Compare(ad.archiveNo, ANoE) <= 0).OrderBy(ad => ad.archiveNo);
                    List<vw_ClassArchives> list = temp1.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].archiveTitle != null)
                            list[i].archiveTitle = list[i].archiveTitle.Trim();
                        if (list[i].archiveNo != null)
                            list[i].archiveNo = list[i].archiveNo.Trim();
                        if (list[i].bianzhiUnit != null)
                            list[i].bianzhiUnit = list[i].bianzhiUnit.Trim();
                        if (list[i].measureUnit != null)
                            list[i].measureUnit = list[i].measureUnit.Trim();
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/ChengJianBMuLu.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("FenLei", ds);
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
            if (action == "打印城建档案B大类目录（模糊）")
            {
                LocalReport localReport = new LocalReport();
                string ANo = Request.Form["archiveNo"];
                if (ANo == "")
                {
                    Response.Write("<script>alert('请输入档案号!');</script>");
                }
                if (ANo != null)
                {
                    var temp = db.vw_ClassArchives.Where(ad => ad.archiveNo.Contains(ANo)).OrderBy(ad => ad.archiveNo);
                    List<vw_ClassArchives> list = temp.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].archiveTitle != null)
                            list[i].archiveTitle = list[i].archiveTitle.Trim();
                        if (list[i].archiveNo != null)
                            list[i].archiveNo = list[i].archiveNo.Trim();
                        if (list[i].bianzhiUnit != null)
                            list[i].bianzhiUnit = list[i].bianzhiUnit.Trim();
                        if (list[i].measureUnit != null)
                            list[i].measureUnit = list[i].measureUnit.Trim();
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/ChengJianBMuLu.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("FenLei", ds);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult ZDDangAn(string action, string type = "PDF")
        {
            if (action == "打印征地档案（精确）")
            {
                LocalReport localReport = new LocalReport();
                string ANoS = Request.Form["archiveNoS"];
                string ANoE = Request.Form["archiveNoE"];
                if (ANoS == "" && ANoE != "")
                {
                    Response.Write("<script>alert('起始档案号不能为空!');</script>");
                }
                if (ANoS != "" && ANoE == "")
                {
                    Response.Write("<script>alert('终止档案号不能为空!');</script>");
                }
                if (ANoS == "" && ANoE == "")
                {
                    Response.Write("<script>alert('请输入档案号范围!');</script>");
                }
                if (ANoS != "" && ANoE != "")
                {
                    var temp = db.vw_zdArchives.Where(ad => string.Compare(ad.archiveNo, ANoS) >= 0);
                    var temp1 = temp.Where(ad => string.Compare(ad.archiveNo, ANoE) <= 0).OrderBy(ad => ad.regisNo);
                    List<vw_zdArchives> list = temp1.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].firstResponsible != null)
                            list[i].firstResponsible = list[i].firstResponsible.Replace("\n", "");
                        if (list[i].archiveTitle != null)
                            list[i].archiveTitle = list[i].archiveTitle.Replace("\n", "");
                        if (list[i].hbLocation != null)
                            list[i].hbLocation = list[i].hbLocation.Replace("\n", "");
                        if (list[i].firstResponsible != null)
                            list[i].firstResponsible = list[i].firstResponsible.Replace("\n", "");
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/ZDDangAn.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("ZDDangAn", ds);
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
            if (action == "打印征地档案（模糊）")
            {
                LocalReport localReport = new LocalReport();
                string ANo = Request.Form["archiveNo"];
                if (ANo == "")
                {
                    Response.Write("<script>alert('请输入档案号!');</script>");
                }
                if (ANo != null)
                {
                    var temp = db.vw_zdArchives.Where(ad => ad.archiveNo == ANo).OrderBy(ad => ad.regisNo);
                    List<vw_zdArchives> list = temp.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].firstResponsible != null)
                            list[i].firstResponsible = list[i].firstResponsible.Replace("\n", "");
                        if (list[i].archiveTitle != null)
                            list[i].archiveTitle = list[i].archiveTitle.Replace("\n", "");
                        if (list[i].hbLocation != null)
                            list[i].hbLocation = list[i].hbLocation.Replace("\n", "");
                        if (list[i].firstResponsible != null)
                            list[i].firstResponsible = list[i].firstResponsible.Replace("\n", "");
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/ZDDangAn.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("ZDDangAn", ds);
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
            if (action == "打印征地档案（总登记号精确）")
            {
                LocalReport localReport = new LocalReport();
                string ANoS = Request.Form["archiveNoS1"];
                string ANoE = Request.Form["archiveNoE1"];
                if (ANoS == "" && ANoE != "")
                {
                    Response.Write("<script>alert('起始总登记号不能为空!');</script>");
                }
                if (ANoS != "" && ANoE == "")
                {
                    Response.Write("<script>alert('终止总登记号不能为空!');</script>");
                }
                if (ANoS == "" && ANoE == "")
                {
                    Response.Write("<script>alert('请输入总登记号范围!');</script>");
                }
                if (ANoS != "" && ANoE != "")
                {
                    var temp = db.vw_zdArchives.Where(ad => string.Compare(ad.regisNo, ANoS) >= 0);
                    var temp1 = temp.Where(ad => string.Compare(ad.regisNo, ANoE) <= 0);
                    List<vw_zdArchives> list = temp1.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].firstResponsible != null)
                            list[i].firstResponsible = list[i].firstResponsible.Replace("\n", "");
                        if (list[i].archiveTitle != null)
                            list[i].archiveTitle = list[i].archiveTitle.Replace("\n", "");
                        if (list[i].hbLocation != null)
                            list[i].hbLocation = list[i].hbLocation.Replace("\n", "");
                        if (list[i].firstResponsible != null)
                            list[i].firstResponsible = list[i].firstResponsible.Replace("\n", "");
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/ZDDangAn.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("ZDDangAn", ds);
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
            if (action == "打印征地档案（总登记号）")
            {
                LocalReport localReport = new LocalReport();
                string ANo = Request.Form["archiveNo1"];
                if (ANo == "")
                {
                    Response.Write("<script>alert('请输入总登记号!');</script>");
                }
                if (ANo != null)
                {
                    var temp = db.vw_zdArchives.Where(ad => ad.regisNo == ANo);
                    List<vw_zdArchives> list = temp.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].firstResponsible != null)
                            list[i].firstResponsible = list[i].firstResponsible.Replace("\n", "");
                        if (list[i].archiveTitle != null)
                            list[i].archiveTitle = list[i].archiveTitle.Replace("\n", "");
                        if (list[i].hbLocation != null)
                            list[i].hbLocation = list[i].hbLocation.Replace("\n", "");
                        if (list[i].firstResponsible != null)
                            list[i].firstResponsible = list[i].firstResponsible.Replace("\n", "");
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/ZDDangAn.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("ZDDangAn", ds);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult GuiHuaAnJuanMuLu(string action, string type = "PDF")
        {
            ViewBag.PlanArchiveClassify = new SelectList(db.PlanArchiveClassify, "classifyName", "classifyName");
            if (action == "打印规划档案案卷目录（顺序号范围）")
            {
                string classify = Request.Form["PlanArchiveClassify"].Trim();
                string NoS = Request.Form["seqNoStart"];
                string NoE = Request.Form["seqNoEnd"];
                if (NoS == "" && NoE != "")
                {
                    Response.Write("<script>alert('起始顺序号不能为空!');</script>");
                }
                if (NoS != "" && NoE == "")
                {
                    Response.Write("<script>alert('终止顺序号不能为空!');</script>");
                }
                if (NoS == "" && NoE == "")
                {
                    Response.Write("<script>alert('请输入顺序号范围!');</script>");
                }
                if ((NoS != "") && (NoE != ""))
                {
                    int seqNoE = int.Parse(NoE);
                    int seqNoS = int.Parse(NoS);
                    LocalReport localReport = new LocalReport();
                    var temp = ab.vw_PlanProjectBoxList1.Where(ad => ad.seqNo1 >= seqNoS).Where(ad => ad.seqNo1 <= seqNoE).OrderBy(ad => ad.seqNo1);
                    if (classify != "")
                    {
                        //var Class = db.PlanArchiveClassify.Where(ab => ab.classifyName == classify).First();
                        //string classifyName = classify + "-" + Class.classifySX;
                        //var temp1 = temp.Where(ab => ab.classify == classifyName);
                        var temp1 = temp.Where(ab => ab.classifyName.Contains(classify));
                        List<vw_PlanProjectBoxList1> list = temp1.ToList();
                        var ds = list;
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].Expr5 != null)
                                list[i].archiveTitle = list[i].Expr5.Trim();
                            if (list[i].classifyName != null)
                                list[i].classify = list[i].classifyName.Trim();
                        }
                        localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaAnJuanMuLu.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("GuiHuaAnJuanMuLu", ds);
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
                    else
                    {
                        //var temp1 = bb.PlanArchiveBox.Where(ad => ad.seqNo >= seqNoS).Where(ad => ad.seqNo <= seqNoE).OrderBy(ad => ad.seqNo);
                        List<vw_PlanProjectBoxList1> list = temp.ToList();
                        var ds = list;
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].Expr5 != null)
                                list[i].archiveTitle = list[i].Expr5.Trim();
                            if (list[i].classifyName != null)
                                list[i].classify = list[i].classifyName.Trim();
                        }
                        localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaAnJuanMuLu.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("GuiHuaAnJuanMuLu", ds);
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
            }

            if (action == "取消")
            {
                return View("Home");
            }
            return View();

        }

        public ActionResult GuiHuaGongChengMuLu(string action, string type = "PDF")
        {
            ViewBag.PlanArchiveClassify = new SelectList(db.PlanArchiveClassify, "classifyName", "classifyName");

            if (action == "打印规划档案工程目录")
            {
                string classify = Request.Form["PlanArchiveClassify"];
                string YS = Request.Form["yearS"].ToString().PadLeft(6, '0');
                string YE = Request.Form["yearE"].ToString().PadLeft(6, '0');

                if ((YS != "000000") && (YE != "000000"))
                {
                    LocalReport localReport = new LocalReport();
                    var temp = db.vw_PlanProjectList.Where(ad => string.Compare(ad.projectno, YS) >= 0);
                    if (classify != "")
                    {
                        var temp1 = temp.Where(ad => string.Compare(ad.projectno, YE) <= 0).Where(ab => ab.classifyName == classify).OrderBy(ad => ad.projectno);
                        List<vw_PlanProjectList> list = temp1.ToList();
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].classifyName != null)
                                list[i].classifyName = list[i].classifyName.Trim();
                            if (list[i].boxNo != null)
                                list[i].boxNo = list[i].boxNo.Trim();
                            if (list[i].fileNo != null)
                                list[i].fileNo = list[i].fileNo.Trim();
                        }
                        var ds = list;
                        localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("GHGCML", ds);
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
                    else
                    {
                        var temp1 = temp.Where(ad => string.Compare(ad.projectno, YE) <= 0).Where(ad => string.Compare(ad.projectno, YS) >= 0).OrderBy(ad => ad.projectno);
                        List<vw_PlanProjectList> list = temp1.ToList();
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].classifyName != null)
                                list[i].classifyName = list[i].classifyName.Trim();
                            if (list[i].boxNo != null)
                                list[i].boxNo = list[i].boxNo.Trim();
                            if (list[i].fileNo != null)
                                list[i].fileNo = list[i].fileNo.Trim();
                        }
                        var ds = list;
                        localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("GHGCML", ds);
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
                if ((YS != "000000") && (YE == "000000"))
                {
                    LocalReport localReport = new LocalReport();
                    var temp = db.vw_PlanProjectList.Where(ad => string.Compare(ad.projectno, YS) == 0);
                    if (classify != "")
                    {
                        var temp1 = temp.Where(ab => ab.classifyName == classify).OrderBy(ad => ad.projectno);
                        List<vw_PlanProjectList> list = temp1.ToList();
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].classifyName != null)
                                list[i].classifyName = list[i].classifyName.Trim();
                            if (list[i].boxNo != null)
                                list[i].boxNo = list[i].boxNo.Trim();
                            if (list[i].fileNo != null)
                                list[i].fileNo = list[i].fileNo.Trim();
                        }
                        var ds = list;
                        localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("GHGCML", ds);
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
                    else
                    {
                        var temp1 = temp.Where(ad => string.Compare(ad.projectno, YS) == 0).OrderBy(ad => ad.projectno);
                        List<vw_PlanProjectList> list = temp1.ToList();
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].classifyName != null)
                                list[i].classifyName = list[i].classifyName.Trim();
                            if (list[i].boxNo != null)
                                list[i].boxNo = list[i].boxNo.Trim();
                            if (list[i].fileNo != null)
                                list[i].fileNo = list[i].fileNo.Trim();
                        }
                        var ds = list;
                        localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("GHGCML", ds);
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
                if ((YS == "000000") && (YE == "000000"))
                {
                    if (classify != "")
                    {
                        LocalReport localReport = new LocalReport();
                        var temp = db.vw_PlanProjectList.Where(ab => ab.classifyName == classify).OrderBy(ad => ad.projectno);
                        List<vw_PlanProjectList> list = temp.ToList();
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].classifyName != null)
                                list[i].classifyName = list[i].classifyName.Trim();
                            if (list[i].boxNo != null)
                                list[i].boxNo = list[i].boxNo.Trim();
                            if (list[i].fileNo != null)
                                list[i].fileNo = list[i].fileNo.Trim();
                        }
                        var ds = list;
                        localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("GHGCML", ds);
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
                    else
                    {
                        LocalReport localReport = new LocalReport();
                        List<vw_PlanProjectList> list = db.vw_PlanProjectList.ToList();
                        for (int i = 0; i < list.Count(); i++)
                        {
                            if (list[i].classifyName != null)
                                list[i].classifyName = list[i].classifyName.Trim();
                            if (list[i].boxNo != null)
                                list[i].boxNo = list[i].boxNo.Trim();
                            if (list[i].fileNo != null)
                                list[i].fileNo = list[i].fileNo.Trim();
                        }
                        var ds = list;
                        localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
                        ReportDataSource reportDataSource = new ReportDataSource("GHGCML", ds);
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
            }

            //if (action == "打印规划档案工程目录（顺序号）")
            //{
            //    string classify = Request.Form["PlanArchiveClassify"];
            //    string NoE = Request.Form["seqNoEnd"];
            //    string NoS = Request.Form["seqNoStart"];
            //    string jnE = Request.Form["juanneiEnd"];
            //    string jnS = Request.Form["juanneiStart"];

            //    if ((NoE != "") && (jnE != "") && (NoS != "") && (jnS != ""))
            //    {
            //        if (classify != "")
            //        {
            //            LocalReport localReport = new LocalReport();
            //            int seqNoS = int.Parse(Request.Form["seqNoStart"]);
            //            int seqNoE = int.Parse(Request.Form["seqNoEnd"]);
            //            int juanneiS = int.Parse(Request.Form["juanneiStart"]);
            //            int juanneiE = int.Parse(Request.Form["juanneiEnd"]);
            //            var temp = db.vw_PlanProjectList.Where(ad => ad.seqNo >= seqNoS).Where(ad => ad.seqNo <= seqNoE);
            //            var temp1 = temp.Where(ad => ad.juanneiSeqNo >= juanneiS).Where(ad => ad.juanneiSeqNo <= juanneiE).Where(ad => ad.classifyName == classify);
            //            List<vw_PlanProjectList> list = temp1.ToList();
            //            for (int i = 0; i < list.Count(); i++)
            //            {
            //                if (list[i].classifyName != null)
            //                    list[i].classifyName = list[i].classifyName.Trim();
            //                if (list[i].boxNo != null)
            //                    list[i].boxNo = list[i].boxNo.Trim();
            //                if (list[i].fileNo != null)
            //                    list[i].fileNo = list[i].fileNo.Trim();
            //            }
            //            var ds = list;
            //            localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
            //            ReportDataSource reportDataSource = new ReportDataSource("GuiHuaGongChengMuLu", ds);
            //            localReport.DataSources.Add(reportDataSource);
            //            string reportType = type;
            //            string mimeType;
            //            string encoding;
            //            string fileNameExtension;
            //            string deviceInfo =
            //            "<DeviceInfo>" +
            //            "<OutPutFormat>" + type + "</OutPutFormat>" +
            //            "</DeviceInfo>";
            //            Warning[] warnings;
            //            string[] streams;
            //            byte[] renderedBytes;
            //            renderedBytes = localReport.Render(
            //                   reportType,
            //                   deviceInfo,
            //                   out mimeType,
            //                   out encoding,
            //                   out fileNameExtension,
            //                   out streams,
            //                   out warnings
            //                   );
            //            return File(renderedBytes, mimeType);
            //        }
            //        else
            //        {
            //            LocalReport localReport = new LocalReport();
            //            int seqNoS = int.Parse(Request.Form["seqNoStart"]);
            //            int seqNoE = int.Parse(Request.Form["seqNoEnd"]);
            //            int juanneiS = int.Parse(Request.Form["juanneiStart"]);
            //            int juanneiE = int.Parse(Request.Form["juanneiEnd"]);
            //            var temp = db.vw_PlanProjectList.Where(ad => ad.seqNo >= seqNoS).Where(ad => ad.seqNo <= seqNoE);
            //            var temp1 = temp.Where(ad => ad.juanneiSeqNo >= juanneiS).Where(ad => ad.juanneiSeqNo <= juanneiE);
            //            List<vw_PlanProjectList> list = temp1.ToList();
            //            for (int i = 0; i < list.Count(); i++)
            //            {
            //                if (list[i].classifyName != null)
            //                    list[i].classifyName = list[i].classifyName.Trim();
            //                if (list[i].boxNo != null)
            //                    list[i].boxNo = list[i].boxNo.Trim();
            //                if (list[i].fileNo != null)
            //                    list[i].fileNo = list[i].fileNo.Trim();
            //            }
            //            var ds = list;
            //            localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
            //            ReportDataSource reportDataSource = new ReportDataSource("GuiHuaGongChengMuLu", ds);
            //            localReport.DataSources.Add(reportDataSource);
            //            string reportType = type;
            //            string mimeType;
            //            string encoding;
            //            string fileNameExtension;
            //            string deviceInfo =
            //            "<DeviceInfo>" +
            //            "<OutPutFormat>" + type + "</OutPutFormat>" +
            //            "</DeviceInfo>";
            //            Warning[] warnings;
            //            string[] streams;
            //            byte[] renderedBytes;
            //            renderedBytes = localReport.Render(
            //                   reportType,
            //                   deviceInfo,
            //                   out mimeType,
            //                   out encoding,
            //                   out fileNameExtension,
            //                   out streams,
            //                   out warnings
            //                   );
            //            return File(renderedBytes, mimeType);
            //        }
            //    }
            //    if ((NoE == "") && (NoS == ""))
            //    {
            //        if (classify != "")
            //        {
            //            LocalReport localReport = new LocalReport();
            //            int juanneiS = int.Parse(Request.Form["juanneiStart"]);
            //            int juanneiE = int.Parse(Request.Form["juanneiEnd"]);
            //            var temp1 = db.vw_PlanProjectList.Where(ad => ad.juanneiSeqNo >= juanneiS).Where(ad => ad.juanneiSeqNo <= juanneiE).Where(ad => ad.classifyName == classify);
            //            List<vw_PlanProjectList> list = temp1.ToList();
            //            for (int i = 0; i < list.Count(); i++)
            //            {
            //                if (list[i].classifyName != null)
            //                    list[i].classifyName = list[i].classifyName.Trim();
            //                if (list[i].boxNo != null)
            //                    list[i].boxNo = list[i].boxNo.Trim();
            //                if (list[i].fileNo != null)
            //                    list[i].fileNo = list[i].fileNo.Trim();
            //            }
            //            var ds = list;
            //            localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
            //            ReportDataSource reportDataSource = new ReportDataSource("GuiHuaGongChengMuLu", ds);
            //            localReport.DataSources.Add(reportDataSource);
            //            string reportType = type;
            //            string mimeType;
            //            string encoding;
            //            string fileNameExtension;
            //            string deviceInfo =
            //            "<DeviceInfo>" +
            //            "<OutPutFormat>" + type + "</OutPutFormat>" +
            //            "</DeviceInfo>";
            //            Warning[] warnings;
            //            string[] streams;
            //            byte[] renderedBytes;
            //            renderedBytes = localReport.Render(
            //                   reportType,
            //                   deviceInfo,
            //                   out mimeType,
            //                   out encoding,
            //                   out fileNameExtension,
            //                   out streams,
            //                   out warnings
            //                   );
            //            return File(renderedBytes, mimeType);
            //        }
            //        else
            //        {
            //            LocalReport localReport = new LocalReport();
            //            int juanneiS = int.Parse(Request.Form["juanneiStart"]);
            //            int juanneiE = int.Parse(Request.Form["juanneiEnd"]);
            //            var temp1 = db.vw_PlanProjectList.Where(ad => ad.juanneiSeqNo >= juanneiS).Where(ad => ad.juanneiSeqNo <= juanneiE);
            //            List<vw_PlanProjectList> list = temp1.ToList();
            //            for (int i = 0; i < list.Count(); i++)
            //            {
            //                if (list[i].classifyName != null)
            //                    list[i].classifyName = list[i].classifyName.Trim();
            //                if (list[i].boxNo != null)
            //                    list[i].boxNo = list[i].boxNo.Trim();
            //                if (list[i].fileNo != null)
            //                    list[i].fileNo = list[i].fileNo.Trim();
            //            }
            //            var ds = list;
            //            localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
            //            ReportDataSource reportDataSource = new ReportDataSource("GuiHuaGongChengMuLu", ds);
            //            localReport.DataSources.Add(reportDataSource);
            //            string reportType = type;
            //            string mimeType;
            //            string encoding;
            //            string fileNameExtension;
            //            string deviceInfo =
            //            "<DeviceInfo>" +
            //            "<OutPutFormat>" + type + "</OutPutFormat>" +
            //            "</DeviceInfo>";
            //            Warning[] warnings;
            //            string[] streams;
            //            byte[] renderedBytes;
            //            renderedBytes = localReport.Render(
            //                   reportType,
            //                   deviceInfo,
            //                   out mimeType,
            //                   out encoding,
            //                   out fileNameExtension,
            //                   out streams,
            //                   out warnings
            //                   );
            //            return File(renderedBytes, mimeType);
            //        }
            //    }
            //}
            //if (action == "打印规划档案工程目录（流水号）")
            //{
            //    string classify = Request.Form["PlanArchiveClassify"];
            //    string totalS = Request.Form["totalSeqNoS"];
            //    string totalE = Request.Form["totalSeqNoE"];
            //    string S = totalS.ToString().PadLeft(8, '0');
            //    string E = totalE.ToString().PadLeft(8, '0');
            //    if ((totalS != "") && (totalE != ""))
            //    {
            //        if (classify != "")
            //        {
            //            LocalReport localReport = new LocalReport();
            //            var temp = db.vw_PlanProjectList.Where(ad => string.Compare(ad.totalSeqNo, S) >= 0);
            //            var temp1 = temp.Where(ad => string.Compare(ad.totalSeqNo, E) <= 0).Where(ab => ab.classifyName == classify);
            //            List<vw_PlanProjectList> list = temp1.ToList();
            //            for (int i = 0; i < list.Count(); i++)
            //            {
            //                if (list[i].classifyName != null)
            //                    list[i].classifyName = list[i].classifyName.Trim();
            //                if (list[i].boxNo != null)
            //                    list[i].boxNo = list[i].boxNo.Trim();
            //                if (list[i].fileNo != null)
            //                    list[i].fileNo = list[i].fileNo.Trim();
            //            }
            //            var ds = list;
            //            localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
            //            ReportDataSource reportDataSource = new ReportDataSource("GuiHuaGongChengMuLu", ds);
            //            localReport.DataSources.Add(reportDataSource);
            //            string reportType = type;
            //            string mimeType;
            //            string encoding;
            //            string fileNameExtension;
            //            string deviceInfo =
            //            "<DeviceInfo>" +
            //            "<OutPutFormat>" + type + "</OutPutFormat>" +
            //            "</DeviceInfo>";
            //            Warning[] warnings;
            //            string[] streams;
            //            byte[] renderedBytes;
            //            renderedBytes = localReport.Render(
            //                   reportType,
            //                   deviceInfo,
            //                   out mimeType,
            //                   out encoding,
            //                   out fileNameExtension,
            //                   out streams,
            //                   out warnings
            //                   );
            //            return File(renderedBytes, mimeType);
            //        }
            //        else
            //        {
            //            LocalReport localReport = new LocalReport();
            //            var temp = db.vw_PlanProjectList.Where(ad => string.Compare(ad.totalSeqNo, S) >= 0);
            //            var temp1 = temp.Where(ad => string.Compare(ad.totalSeqNo, E) <= 0);
            //            List<vw_PlanProjectList> list = temp1.ToList();
            //            for (int i = 0; i < list.Count(); i++)
            //            {
            //                if (list[i].classifyName != null)
            //                    list[i].classifyName = list[i].classifyName.Trim();
            //                if (list[i].boxNo != null)
            //                    list[i].boxNo = list[i].boxNo.Trim();
            //                if (list[i].fileNo != null)
            //                    list[i].fileNo = list[i].fileNo.Trim();
            //            }
            //            var ds = list;
            //            localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
            //            ReportDataSource reportDataSource = new ReportDataSource("GuiHuaGongChengMuLu", ds);
            //            localReport.DataSources.Add(reportDataSource);
            //            string reportType = type;
            //            string mimeType;
            //            string encoding;
            //            string fileNameExtension;
            //            string deviceInfo =
            //            "<DeviceInfo>" +
            //            "<OutPutFormat>" + type + "</OutPutFormat>" +
            //            "</DeviceInfo>";
            //            Warning[] warnings;
            //            string[] streams;
            //            byte[] renderedBytes;
            //            renderedBytes = localReport.Render(
            //                   reportType,
            //                   deviceInfo,
            //                   out mimeType,
            //                   out encoding,
            //                   out fileNameExtension,
            //                   out streams,
            //                   out warnings
            //                   );
            //            return File(renderedBytes, mimeType);
            //        }
            //    }
            //    if ((totalS == "") && (totalE == ""))
            //    {
            //        if (classify != "")
            //        {
            //            LocalReport localReport = new LocalReport();
            //            var temp = db.vw_PlanProjectList.Where(ab => ab.classifyName == classify);
            //            List<vw_PlanProjectList> list = temp.ToList();
            //            for (int i = 0; i < list.Count(); i++)
            //            {
            //                if (list[i].classifyName != null)
            //                    list[i].classifyName = list[i].classifyName.Trim();
            //                if (list[i].boxNo != null)
            //                    list[i].boxNo = list[i].boxNo.Trim();
            //                if (list[i].fileNo != null)
            //                    list[i].fileNo = list[i].fileNo.Trim();
            //            }
            //            var ds = list;
            //            localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
            //            ReportDataSource reportDataSource = new ReportDataSource("GuiHuaGongChengMuLu", ds);
            //            localReport.DataSources.Add(reportDataSource);
            //            string reportType = type;
            //            string mimeType;
            //            string encoding;
            //            string fileNameExtension;
            //            string deviceInfo =
            //            "<DeviceInfo>" +
            //            "<OutPutFormat>" + type + "</OutPutFormat>" +
            //            "</DeviceInfo>";
            //            Warning[] warnings;
            //            string[] streams;
            //            byte[] renderedBytes;
            //            renderedBytes = localReport.Render(
            //                   reportType,
            //                   deviceInfo,
            //                   out mimeType,
            //                   out encoding,
            //                   out fileNameExtension,
            //                   out streams,
            //                   out warnings
            //                   );
            //            return File(renderedBytes, mimeType);
            //        }
            //        else
            //        {
            //            LocalReport localReport = new LocalReport();
            //            List<vw_PlanProjectList> list = db.vw_PlanProjectList.ToList();
            //            for (int i = 0; i < list.Count(); i++)
            //            {
            //                if (list[i].classifyName != null)
            //                    list[i].classifyName = list[i].classifyName.Trim();
            //                if (list[i].boxNo != null)
            //                    list[i].boxNo = list[i].boxNo.Trim();
            //                if (list[i].fileNo != null)
            //                    list[i].fileNo = list[i].fileNo.Trim();
            //            }
            //            var ds = list;
            //            localReport.ReportPath = Server.MapPath("~/Report/Office/GuiHuaGongChengMuLu.rdlc");
            //            ReportDataSource reportDataSource = new ReportDataSource("GuiHuaGongChengMuLu", ds);
            //            localReport.DataSources.Add(reportDataSource);
            //            string reportType = type;
            //            string mimeType;
            //            string encoding;
            //            string fileNameExtension;
            //            string deviceInfo =
            //            "<DeviceInfo>" +
            //            "<OutPutFormat>" + type + "</OutPutFormat>" +
            //            "</DeviceInfo>";
            //            Warning[] warnings;
            //            string[] streams;
            //            byte[] renderedBytes;
            //            renderedBytes = localReport.Render(
            //                   reportType,
            //                   deviceInfo,
            //                   out mimeType,
            //                   out encoding,
            //                   out fileNameExtension,
            //                   out streams,
            //                   out warnings
            //                   );
            //            return File(renderedBytes, mimeType);
            //        }
            //    }
            //}
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult ZhiZhaoDangAn(string action, string type = "PDF")
        {
            if (action == "打印执照档案（精确）")
            {
                LocalReport localReport = new LocalReport();
                string LNoS = Request.Form["licenceNoS"];
                string LNoE = Request.Form["licenceNoE"];
                if (LNoS == "" && LNoE != "")
                {
                    Response.Write("<script>alert('起始执照号不能为空!');</script>");
                }
                if (LNoS != "" && LNoE == "")
                {
                    Response.Write("<script>alert('终止执照号不能为空!');</script>");
                }
                if (LNoS == "" && LNoE == "")
                {
                    Response.Write("<script>alert('请输入执照号范围!');</script>");
                }
                if (LNoS != "" && LNoE != "")
                {
                    var temp = db.vw_licenceArchive.Where(ad => string.Compare(ad.licenceNo, LNoS) >= 0);
                    var temp1 = temp.Where(ad => string.Compare(ad.licenceNo, LNoE) <= 0).OrderBy(ad => ad.licenceNo);
                    List<vw_licenceArchive> list = temp1.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].licenceNo != null)
                            list[i].licenceNo = list[i].licenceNo.Trim();
                        if (list[i].location != null)
                            list[i].location = list[i].location.Trim();
                        if (list[i].applyUnit != null)
                            list[i].applyUnit = list[i].applyUnit.Trim();
                        if (list[i].projectRange != null)
                            list[i].projectRange = list[i].projectRange.Trim();
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/ZhiZhaoDangAn.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("ZhiZhaoDangAn", ds);
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
            if (action == "打印执照档案（年份）")
            {
                LocalReport localReport = new LocalReport();
                string LNoS = Request.Form["yearNoS"];
                string LNoE = Request.Form["yearNoE"];
                if (LNoS == "" && LNoE != "")
                {
                    Response.Write("<script>alert('起始年份不能为空!');</script>");
                }
                if (LNoS != "" && LNoE == "")
                {
                    Response.Write("<script>alert('终止年份不能为空!');</script>");
                }
                if (LNoS == "" && LNoE == "")
                {
                    Response.Write("<script>alert('请输入年份范围!');</script>");
                }
                if (LNoS != "" && LNoE != "")
                {
                    var temp = db.vw_licenceArchive.Where(ad => string.Compare(ad.year, LNoS) >= 0);
                    var temp1 = temp.Where(ad => string.Compare(ad.year, LNoE) <= 0).OrderBy(ad => ad.licenceNo);
                    List<vw_licenceArchive> list = temp1.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].licenceNo != null)
                            list[i].licenceNo = list[i].licenceNo.Trim();
                        if (list[i].location != null)
                            list[i].location = list[i].location.Trim();
                        if (list[i].applyUnit != null)
                            list[i].applyUnit = list[i].applyUnit.Trim();
                        if (list[i].projectRange != null)
                            list[i].projectRange = list[i].projectRange.Trim();
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/ZhiZhaoDangAn.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("ZhiZhaoDangAn", ds);
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
            if (action == "打印执照档案（模糊）")
            {
                LocalReport localReport = new LocalReport();
                string LNo = Request.Form["licenceNo"];
                if (LNo == "")
                {
                    Response.Write("<script>alert('请输入执照号!');</script>");
                }
                if (LNo != "")
                {
                    var temp = db.vw_licenceArchive.Where(ad => ad.licenceNo.Contains(LNo)).OrderBy(ad => ad.licenceNo);
                    List<vw_licenceArchive> list = temp.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].licenceNo != null)
                            list[i].licenceNo = list[i].licenceNo.Trim();
                        if (list[i].location != null)
                            list[i].location = list[i].location.Trim();
                        if (list[i].applyUnit != null)
                            list[i].applyUnit = list[i].applyUnit.Trim();
                        if (list[i].projectRange != null)
                            list[i].projectRange = list[i].projectRange.Trim();
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/ZhiZhaoDangAn.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("ZhiZhaoDangAn", ds);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult ZhiZhaoDangAn1(string action, string type = "PDF")
        {
            if (action == "打印执照档案（精确）")
            {
                LocalReport localReport = new LocalReport();
                string LNoS = Request.Form["licenceNoS"];
                string LNoE = Request.Form["licenceNoE"];
                if (LNoS == "" && LNoE != "")
                {
                    Response.Write("<script>alert('起始执照号不能为空!');</script>");
                }
                if (LNoS != "" && LNoE == "")
                {
                    Response.Write("<script>alert('终止执照号不能为空!');</script>");
                }
                if (LNoS == "" && LNoE == "")
                {
                    Response.Write("<script>alert('请输入执照号范围!');</script>");
                }
                if (LNoS != "" && LNoE != "")
                {
                    var temp = db.vw_licenceArchive.Where(ad => string.Compare(ad.licenceNo, LNoS) >= 0);
                    var temp1 = temp.Where(ad => string.Compare(ad.licenceNo, LNoE) <= 0);
                    List<vw_licenceArchive> list = temp1.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].licenceNo != null)
                            list[i].licenceNo = list[i].licenceNo.Trim();
                        if (list[i].location != null)
                            list[i].location = list[i].location.Trim();
                        if (list[i].applyUnit != null)
                            list[i].applyUnit = list[i].applyUnit.Trim();
                        if (list[i].projectRange != null)
                            list[i].projectRange = list[i].projectRange.Trim();
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/zhizhao/ZhiZhaoDangAn.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("ZhiZhaoDangAn", ds);
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
            if (action == "打印执照档案（年份）")
            {
                LocalReport localReport = new LocalReport();
                string LNoS = Request.Form["yearNoS"];
                string LNoE = Request.Form["yearNoE"];
                if (LNoS == "" && LNoE != "")
                {
                    Response.Write("<script>alert('起始年份不能为空!');</script>");
                }
                if (LNoS != "" && LNoE == "")
                {
                    Response.Write("<script>alert('终止年份不能为空!');</script>");
                }
                if (LNoS == "" && LNoE == "")
                {
                    Response.Write("<script>alert('请输入年份范围!');</script>");
                }
                if (LNoS != "" && LNoE != "")
                {
                    var temp = db.vw_licenceArchive.Where(ad => string.Compare(ad.year, LNoS) >= 0);
                    var temp1 = temp.Where(ad => string.Compare(ad.year, LNoE) <= 0);
                    List<vw_licenceArchive> list = temp1.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].licenceNo != null)
                            list[i].licenceNo = list[i].licenceNo.Trim();
                        if (list[i].location != null)
                            list[i].location = list[i].location.Trim();
                        if (list[i].applyUnit != null)
                            list[i].applyUnit = list[i].applyUnit.Trim();
                        if (list[i].projectRange != null)
                            list[i].projectRange = list[i].projectRange.Trim();
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/zhizhao/ZhiZhaoDangAn.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("ZhiZhaoDangAn", ds);
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
            if (action == "打印执照档案（模糊）")
            {
                LocalReport localReport = new LocalReport();
                string LNo = Request.Form["licenceNo"];
                if (LNo == "")
                {
                    Response.Write("<script>alert('请输入执照号!');</script>");
                }
                if (LNo != "")
                {
                    var temp = db.vw_licenceArchive.Where(ad => ad.licenceNo.Contains(LNo));
                    List<vw_licenceArchive> list = temp.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].licenceNo != null)
                            list[i].licenceNo = list[i].licenceNo.Trim();
                        if (list[i].location != null)
                            list[i].location = list[i].location.Trim();
                        if (list[i].applyUnit != null)
                            list[i].applyUnit = list[i].applyUnit.Trim();
                        if (list[i].projectRange != null)
                            list[i].projectRange = list[i].projectRange.Trim();
                    }
                    var ds = list;
                    localReport.ReportPath = Server.MapPath("~/Report/zhizhao/ZhiZhaoDangAn.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("ZhiZhaoDangAn", ds);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult DaoLuDangAn(string action, string type = "PDF")
        {
            if (action == "打印道路档案（年度、卷号范围）")
            {
                LocalReport localReport = new LocalReport();
                string yearS = Request.Form["yearStart"];
                string VNoS = Request.Form["VolNoStart"];
                string yearE = Request.Form["yearEnd"];
                string VNoE = Request.Form["VolNoEnd"];
                string vols = VNoS.ToString().PadLeft(4, '0');
                string vole = VNoE.ToString().PadLeft(4, '0');
                var temp = db.vw_roadArchive.Where(ad => string.Compare(ad.year, yearS) >= 0);
                var temp1 = temp.Where(ad => string.Compare(ad.year, yearE) <= 0);
                var temp2 = temp1.Where(ad => string.Compare(ad.volNo, vols) >= 0);
                var temp3 = temp2.Where(ad => string.Compare(ad.volNo, vole) <= 0).OrderBy(ad => ad.volNo);
                List<vw_roadArchive> list = temp3.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].location != null)
                        list[i].location = list[i].location.Trim();
                    if (list[i].applyUnit != null)
                        list[i].applyUnit = list[i].applyUnit.Trim();
                    if (list[i].projectRange != null)
                        list[i].projectRange = list[i].projectRange.Trim();
                    if (list[i].doorplate != null)
                        list[i].doorplate = list[i].doorplate.Trim();
                    if (list[i].landNo != null)
                        list[i].landNo = list[i].landNo.Trim();
                }
                var ds = list;
                localReport.ReportPath = Server.MapPath("~/Report/Office/DaoLuDangAn.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("DaoLuDangAn", ds);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult ChengJianDangAnAnJuanMuLu(string action, string type = "PDF")
        {
            if (action == "打印城市建设档案案卷目录")
            {
                LocalReport localReport = new LocalReport();
                string SS = Request.Form["SeqNoS"];
                string SE = Request.Form["SeqNoE"];
                if (SS == "" && SE != "")
                {
                    Response.Write("<script>alert('起始项目顺序号不能为空!');</script>");
                }
                if (SS != "" && SE == "")
                {
                    Response.Write("<script>alert('终止项目顺序号不能为空!');</script>");
                }
                if (SS == "" && SE == "")
                {
                    Response.Write("<script>alert('请输入项目顺序号范围!');</script>");
                }
                if (SS != "" && SE != "")
                {
                    long SeqNoS = long.Parse(Request.Form["SeqNoS"]);
                    long SeqNoE = long.Parse(Request.Form["SeqNoE"]);
                    var ds1 = ab.vw_ArchiveList.Where(ad => ad.paperProjectSeqNo >= SeqNoS).Where(ad => ad.paperProjectSeqNo <= SeqNoE).OrderBy(ad => ad.paperProjectSeqNo);
                    List<vw_ArchiveList> list = ds1.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].licenseNo != null)
                            list[i].licenseNo = list[i].licenseNo.Trim();
                        if (list[i].projectName != null)
                            list[i].projectName = list[i].projectName.Trim();
                    }
                    var ds2 = list;
                    localReport.ReportPath = Server.MapPath("~/Report/Office/ChengJianDangAnAnJuanMuLu.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("ChengJianDangAnAnJuanMuLu", ds2);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult ChengJianDangAnLiYongDengJiDanJuan(string action, string type = "PDF")
        {
            if (action == "打印城建档案利用登记表（单卷显示）")
            {
                LocalReport localReport = new LocalReport();
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);

                var ds = ab.BorrowRegistration.Where(ad => ad.borrowDate >= DataFrom).Where(ad => ad.borrowDate <= DataTo).OrderBy(ad => ad.ID);
                List<BorrowRegistration> list = ds.ToList();

                localReport.ReportPath = Server.MapPath("~/Report/Office/LiYongDengJiBiaoDanJuan.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("DangAnLiYongDengJi", ds);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult ChengJianDangAnLiYongDengJiLieBiao(string action, string type = "PDF")
        {
            if (action == "打印城建档案利用登记表（列表显示）")
            {
                LocalReport localReport = new LocalReport();
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                var ds = ab.BorrowRegistration.Where(ad => ad.borrowDate >= DataFrom).Where(ad => ad.borrowDate <= DataTo).OrderBy(ad => ad.borrowSeqNo);
                var ds1 = ds.Where(ad => ad.application1 == true).ToList();
                int count1 = ds1.Count();
                var ds2 = ds.Where(ad => ad.application2 == true).ToList();
                int count2 = ds2.Count();
                var ds3 = ds.Where(ad => ad.application3 == true).ToList();
                int count3 = ds3.Count();
                var ds4 = ds.Where(ad => ad.application4 == true).ToList();
                int count4 = ds4.Count();
                var ds5 = ds.Where(ad => ad.application5 == true).ToList();
                int count5 = ds5.Count();
                var ds6 = ds.Where(ad => ad.application6 == true).ToList();
                int count6 = ds6.Count();

                var us1 = ds.Where(ad => ad.userEffects1 == true).ToList();
                int count7 = us1.Count();
                var us2 = ds.Where(ad => ad.userEffects2 == true).ToList();
                int count8 = us2.Count();
                var us3 = ds.Where(ad => ad.userEffects3 == true).ToList();
                int count9 = us3.Count();
                var us4 = ds.Where(ad => ad.userEffects4 == true).ToList();
                int count10 = us4.Count();
                var us5 = ds.Where(ad => ad.userEffects5 == true).ToList();
                int count11 = us5.Count();

                var go1 = ds.Where(ad => ad.goal1 == true).ToList();
                int count12 = go1.Count();
                var go2 = ds.Where(ad => ad.goal2 == true).ToList();
                int count13 = go2.Count();
                var go3 = ds.Where(ad => ad.goal3 == true).ToList();
                int count14 = go3.Count();
                var go4 = ds.Where(ad => ad.goal4 == true).ToList();
                int count15 = go4.Count();
                var go5 = ds.Where(ad => ad.goal5 == true).ToList();
                int count16 = go5.Count();
                var go6 = ds.Where(ad => ad.goal6 == true).ToList();
                int count17 = go6.Count();

                List<BorrowRegistration> list = ds.ToList();
                int count = list.Count();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].consultVolumeCount == null || list[i].consultVolumeCount == "")
                    {
                        list[i].consultVolumeCount = "0";
                    }
                    if (list[i].copyPageCount == null || list[i].copyPageCount == "")
                    {
                        list[i].copyPageCount = "0";
                    }
                    var userID = list[i].ID;
                    var BindUserAndImageDown = from b in db1.BindUserAndImageDown
                                               where b.realuserID == userID
                                               select b;

                    //统计需要借阅的案卷信息
                    if (BindUserAndImageDown.Count() != 0)
                    {
                        int j = 0;
                        string str = "";
                        var BindUserAndImageDown1 = BindUserAndImageDown.ToArray();
                        for (int k = 0; k < BindUserAndImageDown.Count(); k++)
                        {
                            if (str != BindUserAndImageDown1[k].archivesNo)
                            {
                                str = BindUserAndImageDown1[k].archivesNo;
                                j++;
                            }
                        }
                        list[i].consultVolumeCount = j.ToString();
                    }
                    else
                    {
                        list[i].consultVolumeCount = "0";
                    }
                    //if (list[i].application8 == null)
                    //{
                    //    list[i].application8 = bool.Parse("0");
                    //}
                }
                var ds0 = list;

                localReport.ReportPath = Server.MapPath("~/Report/Office/LiYongDengJiBiaoLieBiao.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("DangAnLiYongDengJi", ds0);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("count", count.ToString().Trim()));
                parameterList.Add(new ReportParameter("count1", count1.ToString().Trim()));
                parameterList.Add(new ReportParameter("count2", count2.ToString().Trim()));
                parameterList.Add(new ReportParameter("count3", count3.ToString().Trim()));
                parameterList.Add(new ReportParameter("count4", count4.ToString().Trim()));
                parameterList.Add(new ReportParameter("count5", count5.ToString().Trim()));
                parameterList.Add(new ReportParameter("count6", count6.ToString().Trim()));
                parameterList.Add(new ReportParameter("count7", count7.ToString().Trim()));
                parameterList.Add(new ReportParameter("count8", count8.ToString().Trim()));
                parameterList.Add(new ReportParameter("count9", count9.ToString().Trim()));
                parameterList.Add(new ReportParameter("count10", count10.ToString().Trim()));
                parameterList.Add(new ReportParameter("count11", count11.ToString().Trim()));
                parameterList.Add(new ReportParameter("count12", count12.ToString().Trim()));
                parameterList.Add(new ReportParameter("count13", count13.ToString().Trim()));
                parameterList.Add(new ReportParameter("count14", count14.ToString().Trim()));
                parameterList.Add(new ReportParameter("count15", count15.ToString().Trim()));
                parameterList.Add(new ReportParameter("count16", count16.ToString().Trim()));
                parameterList.Add(new ReportParameter("count17", count17.ToString().Trim()));
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().ToString().Split(' ')[0].Trim()));

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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult AnJuanZhuLuDan(string action, string type = "PDF")
        {
            if (action == "打印案卷著录单")
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
                var temp = db.vw_categoryList.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.paperProjectSeqNo <= m).OrderBy(ad => ad.paperProjectSeqNo).ThenBy(ad => ad.volNo);
                var temp1 = temp.Where(ad => ad.volNo >= a).Where(ad => ad.volNo <= b);
                List<vw_categoryList> list = temp1.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].archivesNo != null)
                        list[i].archivesNo = list[i].archivesNo.Trim();
                    if (list[i].archivesTitle != null)
                        list[i].archivesTitle = list[i].archivesTitle.Trim();
                    if (list[i].developmentOrganization != null)
                        list[i].developmentOrganization = list[i].developmentOrganization.Trim();
                }
                var ds = list;
                localReport.ReportPath = Server.MapPath("~/Report/Office/AnJuanZhuLuDan.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("AnJuanZhuLuDan", ds);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult AnJuanZhuLuDanFenLei(string action, string type = "PDF")
        {
            if (action == "打印案卷著录单")
            {
                LocalReport localReport = new LocalReport();
                string fenleiNo = Request.Form["No"];
                long SeqNoS = long.Parse(Request.Form["SeqNoS"]);
                long SeqNoE = long.Parse(Request.Form["SeqNoE"]);
                string n = fenleiNo.Substring(0, 1);
                string m = fenleiNo.Substring(1, 1);
                string a = fenleiNo.Split('.').Last();
                var ds = from ad in db.vw_categoryList
                         where ad.mainCategoryID == n
                         where ad.subDictionaryID == m
                         where ad.minorDictionaryID == a
                         select ad;
                var ds1 = ds.Where(ad => ad.paperProjectSeqNo >= SeqNoS).Where(ad => ad.paperProjectSeqNo <= SeqNoE).OrderBy(ad => ad.paperProjectSeqNo).ThenBy(ad => ad.volNo);
                List<vw_categoryList> list = ds1.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].archivesNo != null)
                        list[i].archivesNo = list[i].archivesNo.Trim();
                    if (list[i].archivesTitle != null)
                        list[i].archivesTitle = list[i].archivesTitle.Trim();
                    if (list[i].developmentOrganization != null)
                        list[i].developmentOrganization = list[i].developmentOrganization.Trim();
                }
                var ds2 = list;
                localReport.ReportPath = Server.MapPath("~/Report/Office/AnJuanZhuLuDan.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("AnJuanZhuLuDan", ds2);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("fenleiNo", fenleiNo));
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }


        public ActionResult GongChengGaiKuang(string action, string type = "PDF")
        {
            if (action == "打印工程概况")
            {
                LocalReport localReport = new LocalReport();
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);

                var ds = ab.vw_projectProfile.Where(ad => ad.dateArchive >= DataFrom).Where(ad => ad.dateArchive <= DataTo);
                List<vw_projectProfile> list = ds.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].projectName != null)
                        list[i].projectName = list[i].projectName.Trim();
                }
                var ds2 = list;
                localReport.ReportPath = Server.MapPath("~/Report/Office/GongChengGaiKuang.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("GongChengGaiKuang", ds2);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult danganjieshoumingxi(string action, string type = "PDF")
        {
            if (action == "打印档案接收明细（工程序号）")
            {
                LocalReport localReport = new LocalReport();
                string PNoS = Request.Form["seqNoStart"];
                string PNoE = Request.Form["seqNoEnd"];
                long n = long.Parse(PNoS);
                long m = long.Parse(PNoE);
                var temp = db.vw_receiveArchiveDetail.Where(ad => ad.projectNo >= n).Where(ad => ad.projectNo <= m);
                List<vw_receiveArchiveDetail> list = temp.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].projectName != null)
                        list[i].projectName = list[i].projectName.Trim();
                    if (list[i].constructionOrganization != null)
                        list[i].constructionOrganization = list[i].constructionOrganization.Trim();
                    if (list[i].developmentOrganization != null)
                        list[i].developmentOrganization = list[i].developmentOrganization.Trim();
                }
                var ds = list;

                localReport.ReportPath = Server.MapPath("~/Report/Office/DangAnJieShouMingXi.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("danganjieshou", ds);
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
            if (action == "打印档案接收明细（项目顺序号）")
            {
                LocalReport localReport = new LocalReport();
                string SeqS = Request.Form["txtSeqNoS"];
                string SeqE = Request.Form["txtSeqNoE"];
                long n = long.Parse(SeqS);
                long m = long.Parse(SeqE);
                var temp = db.vw_receiveArchiveDetail.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.paperProjectSeqNo <= m);
                List<vw_receiveArchiveDetail> list = temp.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].projectName != null)
                        list[i].projectName = list[i].projectName.Trim();
                    if (list[i].constructionOrganization != null)
                        list[i].constructionOrganization = list[i].constructionOrganization.Trim();
                    if (list[i].developmentOrganization != null)
                        list[i].developmentOrganization = list[i].developmentOrganization.Trim();
                }
                var ds = list;
                localReport.ReportPath = Server.MapPath("~/Report/Office/DangAnJieShouMingXi.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("danganjieshou", ds);
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

            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult danganzongmulu(string action, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            if (action == "打印档案总目录（项目顺序号范围）")
            {
                string PNoS = Request.Form["seqNoStart"];
                string VNoS = Request.Form["VolNoStart"];
                string PNoE = Request.Form["seqNoEnd"];
                string VNoE = Request.Form["VolNoEnd"];
                long n = long.Parse(PNoS);
                long m = long.Parse(PNoE);
                long a = long.Parse(VNoS);
                long b = long.Parse(VNoE);
                var temp = db.vw_archiveMainList.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.paperProjectSeqNo <= m);
                var temp1 = temp.Where(ad => ad.volNo >= a).Where(ad => ad.volNo <= b).OrderBy(ad => ad.paperProjectSeqNo).ThenBy(ad => ad.volNo);

                List<vw_archiveMainList> list = temp1.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].licenseNo != null)
                        list[i].licenseNo = list[i].licenseNo.Trim();
                    if (list[i].changeLog != null)
                        list[i].changeLog = list[i].changeLog.Trim();
                    if (list[i].location != null)
                        list[i].location = list[i].location.Trim();
                    if (list[i].archivesTitle != null)
                        list[i].archivesTitle = list[i].archivesTitle.Trim();
                    if (list[i].developmentOrganization != null)
                        list[i].developmentOrganization = list[i].developmentOrganization.Trim();
                }
                var ds = list;
                localReport.ReportPath = Server.MapPath("~/Report/Office/cjzongmulu.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("cjzongmulu", ds);
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
            if (action == "打印档案总目录（起始）")
            {
                string SeqS = Request.Form["SeqNoS"];
                string VolS = Request.Form["VolNoS"];
                long n = long.Parse(SeqS);
                long m = long.Parse(VolS);
                var temp = db.vw_archiveMainList.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.volNo >= m);
                List<vw_archiveMainList> list = temp.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].licenseNo != null)
                        list[i].licenseNo = list[i].licenseNo.Trim();
                    if (list[i].changeLog != null)
                        list[i].changeLog = list[i].changeLog.Trim();
                    if (list[i].location != null)
                        list[i].location = list[i].location.Trim();
                    if (list[i].archivesTitle != null)
                        list[i].archivesTitle = list[i].archivesTitle.Trim();
                    if (list[i].developmentOrganization != null)
                        list[i].developmentOrganization = list[i].developmentOrganization.Trim();
                }
                var ds = list;
                localReport.ReportPath = Server.MapPath("~/Report/Office/cjzongmulu.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("cjzongmulu", ds);
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


            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult gerenzhenglihuizong(string action, string type = "PDF")
        {
            if (action == "打印整理档案汇总")
            {
                LocalReport localReport = new LocalReport();
                string Person = Request.Form["collator"];
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                var ds = from ad in ab.PaperArchives
                         where ad.collator == Person
                         select ad;
                var ds1 = ds.Where(ad => ad.dateArchive >= DataFrom).Where(ad => ad.dateArchive <= DataTo);
                localReport.ReportPath = Server.MapPath("~/Report/Office/GeRenZhengLiHuiZong.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("gerenzhengli1", ds1);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }

        public ActionResult fenleizhuludan(string action, string type = "PDF")
        {
            if (action == "打印工程项目著录单")
            {
                LocalReport localReport = new LocalReport();
                string fenleiNo = Request.Form["No"];
                long SeqNoS = long.Parse(Request.Form["SeqNoS"]);
                long SeqNoE = long.Parse(Request.Form["SeqNoE"]);
                string n = fenleiNo.Substring(0, 1);
                string m = fenleiNo.Substring(1, 1);
                int z = fenleiNo.Length;
                string a = fenleiNo.Split('.').Last();
                var ds = from ad in db.vw_projectRecordList1
                         where ad.mainCategoryID == n
                         where ad.subDictionaryID == m
                         where ad.minorDictionaryID == a
                         select ad;
                var ds1 = ds.Where(ad => ad.paperProjectSeqNo >= SeqNoS).Where(ad => ad.paperProjectSeqNo <= SeqNoE).OrderBy(ad => ad.paperProjectSeqNo);
                List<vw_projectRecordList1> list = ds1.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].licenseNo != null)
                        list[i].licenseNo = list[i].licenseNo.Trim();
                    if (list[i].projectName != null)
                        list[i].projectName = list[i].projectName.Trim().Replace("\n", "");
                    if (list[i].transferUnit != null)
                        list[i].transferUnit = list[i].transferUnit.Trim();
                }
                var ds2 = list;
                localReport.ReportPath = Server.MapPath("~/Report/Office/FenLeiZhuLuDan.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("FenLeiZhuLuDan", ds2);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("fenleiNo", fenleiNo));
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult ZhuludanAuto(string action, string type = "PDF")
        {
            if (action == "打印工程项目著录单（自动分类）")
            {
                LocalReport localReport = new LocalReport();

                long SeqNoS = long.Parse(Request.Form["SeqNoS"]);
                long SeqNoE = long.Parse(Request.Form["SeqNoE"]);
                var ds1 = db.vw_projectRecordList1.Where(ad => ad.paperProjectSeqNo >= SeqNoS).Where(ad => ad.paperProjectSeqNo <= SeqNoE).OrderBy(ad => ad.paperProjectSeqNo);

                List<vw_projectRecordList1> list = ds1.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].licenseNo != null)
                        list[i].licenseNo = list[i].licenseNo.Trim();
                    if (list[i].projectName != null)
                        list[i].projectName = list[i].projectName.Trim().Replace("\n", "");
                    if (list[i].transferUnit != null)
                        list[i].transferUnit = list[i].transferUnit.Trim();
                    if (list[i].mainCategoryID != null)
                        list[i].mainCategoryID = list[i].startArchiveNo.Split('-').First();

                }
                var ds2 = list;

                localReport.ReportPath = Server.MapPath("~/Report/Office/FenLeiZhuLuDanAuto.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("FenLeiZhuLuDan", ds2);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult ZhuludanFull(string action, string type = "PDF")
        {
            if (action == "打印工程项目著录单（全部）")
            {
                LocalReport localReport = new LocalReport();
                long SeqNoS = long.Parse(Request.Form["SeqNoS"]);
                long SeqNoE = long.Parse(Request.Form["SeqNoE"]);
                var ds1 = db.vw_projectRecordList1.Where(ad => ad.paperProjectSeqNo >= SeqNoS).Where(ad => ad.paperProjectSeqNo <= SeqNoE).OrderBy(ad => ad.paperProjectSeqNo);
                List<vw_projectRecordList1> list = ds1.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].licenseNo != null)
                        list[i].licenseNo = list[i].licenseNo.Trim();
                    if (list[i].projectName != null)
                        list[i].projectName = list[i].projectName.Trim().Replace("\n", "");
                    if (list[i].transferUnit != null)
                        list[i].transferUnit = list[i].transferUnit.Trim();
                }
                var ds2 = list;
                localReport.ReportPath = Server.MapPath("~/Report/Office/FenLeiZhuLuDanFull.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("FenLeiZhuLuDan", ds2);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult ZhuludanZhuludanDanXiang(string action, string type = "PDF")
        {
            if (action == "打印工程项目著录单（单项）")
            {
                LocalReport localReport = new LocalReport();

                long SeqNoS = long.Parse(Request.Form["SeqNoS"]);
                long SeqNoE = long.Parse(Request.Form["SeqNoE"]);
                var ds1 = db.vw_projectTypelist.Where(ad => ad.paperProjectSeqNo >= SeqNoS).Where(ad => ad.paperProjectSeqNo <= SeqNoE);

                List<vw_projectTypelist> list = ds1.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].licenseNo != null)
                        list[i].licenseNo = list[i].licenseNo.Trim();
                    if (list[i].projectName != null)
                        list[i].projectName = list[i].projectName.Trim();
                    if (list[i].transferUnit != null)
                        list[i].transferUnit = list[i].transferUnit.Trim();
                    if (list[i].mainCategoryID != null)
                        list[i].mainCategoryID = list[i].startArchiveNo.Split('-').First();

                }
                var ds2 = list;

                localReport.ReportPath = Server.MapPath("~/Report/Office/ZhuludanDanXiang.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("FenLeiZhuLuDanDanJuan", ds2);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }

        public ActionResult YeWuGcMl(string action, string type = "PDF")
        {
            if (action == "打印业务档案工程目录")
            {
                string classify = Request.Form["PlanArchiveClassify"];
                string YS = Request.Form["yearS"];
                string YE = Request.Form["yearE"];
                int YS1 = int.Parse(YS);
                int YE1 = int.Parse(YE);

                if ((YS == "") || (YE == "" || YS1 > YE1))
                {
                    return Content("<script >alert('请正确输入起始顺序号');window.history.back();</script>");
                }
                LocalReport localReport = new LocalReport();
                var temp = bb.businessPlanProject.Where(ad => ad.seqNo >= YS1).Where(ad => ad.seqNo <= YE1).OrderBy(ad => ad.seqNo) ;
                var ds = temp;
                localReport.ReportPath = Server.MapPath("~/Report/YeWu/YeWuGcMl.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("YeWuGcMl", ds);
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
    }
}
    
