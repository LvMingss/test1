using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using urban_archive.Models;
using PagedList;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml.Linq;
using System.IO.Compression;

namespace urban_archive.Controllers
{
    

    public class ProjectInfoesController : Controller
    {
       
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        private OfficeEntities cb = new OfficeEntities();
        private gxArchivesContainer bb = new gxArchivesContainer();
        private gxProjectInfo bb1 = new gxProjectInfo();
        private VideoArchiveEntities xs = new VideoArchiveEntities();
        private static int count = 1;


        public ActionResult ZTree()
        {
            return View();
        }
        public ActionResult danganjieshoumingxi(string action, string type = "PDF")
        {
            if (action == "打印档案接收明细")
            {
                LocalReport localReport = new LocalReport();
                string PNoS = Request.Form["seqNoStart"];
                string PNoE = Request.Form["seqNoEnd"];
                string SeqS = Request.Form["txtSeqNoS"];
                string SeqE = Request.Form["txtSeqNoE"];
                if ((PNoE != "" && PNoS != "") && (SeqE == "" && SeqS == ""))
                {
                    long n = long.Parse(PNoS);
                    long m = long.Parse(PNoE);
                    //var temp = cb.vw_receiveArchiveDetail.Where(ad => ad.projectNo >= n && ad.projectNo <= m).OrderBy(ad => ad.projectNo).Distinct();
                    var temp = cb.vw_receiveArchiveDetail.Where(ad => ad.projectNo >= n).Where(ad => ad.projectNo <= m).OrderBy(ad => ad.projectNo);
                    List < vw_receiveArchiveDetail > list = temp.ToList();
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
                }
                if ((PNoE == "" && PNoS == "") && (SeqE != "" && SeqS != ""))
                {
                    long n = long.Parse(SeqS);
                    long m = long.Parse(SeqE);
                    var temp = cb.vw_receiveArchiveDetail.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.paperProjectSeqNo <= m).OrderBy(ad => ad.projectNo);
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
                }
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
        public ActionResult hegezhengqianfa(int id)
        {
            LocalReport localReport = new LocalReport();
            var ds1 = db.vw_ProjectStatus.Where(ad => ad.projectID == id);
            localReport.ReportPath = Server.MapPath("~/Report/jungong/shenpi.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("shenpi", ds1);
            localReport.DataSources.Add(reportDataSource);
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
        public ActionResult chushenyijian(int id)
        {
            LocalReport localReport = new LocalReport();
            var ds1 =db.PaperArchives.Where(ad=>ad.projectID==id);
            var ds2 = db.ProjectInfo.Where(ad => ad.projectID == id);
            localReport.ReportPath = Server.MapPath("~/Report/jungong/chushenyijian.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("chushenyijian2", ds2);
            localReport.DataSources.Add(reportDataSource1);
            string reportType = "WORD";
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
                "<DeviceInfo>" +
                "<OutPutFormat>" + "WORD" + "</OutPutFormat>" +
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
        public ActionResult hegezhengming(int id)
        {
            LocalReport localReport = new LocalReport();
            var ds1 = db.PaperArchives.Where(ad => ad.projectID == id);
            var ds2 = db.ProjectInfo.Where(ad => ad.projectID == id);
            localReport.ReportPath = Server.MapPath("~/Report/jungong/hegezheng.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("chushenyijian2", ds2);
            localReport.DataSources.Add(reportDataSource1);
            string reportType = "word";
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
                "<DeviceInfo>" +
                "<OutPutFormat>" + "word" + "</OutPutFormat>" +
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

        public JsonResult fenleihao()
        {
            var list1 = from bb in db.MainCategory.ToList()
                        select new
                        {
                            id = bb.mainCategoryID.Trim(),
                            pId = "0",
                            name = bb.mainCategoryID.Trim() + ":" + bb.mainCategoryName.Trim()
                        };

            var list2 = from bb in db.SubDictionary.ToList()
                        select new
                        {
                            id = bb.mainCategoryID.Trim() + bb.subDictionaryID.Trim(),
                            pId = bb.mainCategoryID.Trim(),
                            name = bb.subDictionaryID.Trim() + ":" + bb.subDictionaryName.Trim(),
                        };
            var list4 = list1.Union(list2);
            var list3 = from bb in db.MinorDictionary.ToList()
                        select new
                        {
                            id = bb.mainCategoryID.Trim() + bb.subDictionaryID.Trim() + "." + bb.minorDictionaryID.Trim(),
                            pId = bb.mainCategoryID.Trim() + bb.subDictionaryID.Trim(),
                            name = bb.minorDictionaryID.Trim() + ":" + bb.minorDictionaryName.Trim()
                        };
            var list5 = list4.Union(list3);
            return Json(list5, JsonRequestBehavior.AllowGet);
        }
        // GET: ProjectInfoes
        public ActionResult Index()
        {
            //var projectInfo =
            //    db.ProjectInfo.Include(p => p.ArchivesStatus);
            return View(/*projectInfo.ToList()*/);
        }

        // GET: ProjectInfoes/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var test = from ad in db.vw_projectProfile
                       where (ad.projectID == id)
                       select ad;
            vw_projectProfile projectProfile = test.First();
            //档案密级，档案状态，保存期限的初始化
            string a= projectProfile.status, b= projectProfile.securityID, e= projectProfile.retentionPeriodNo;
            if (a!=null&&a!="")
            {
                switch (a.Trim())
                {
                    case "1":
                        projectProfile.status = "接收档案";
                        break;
                    case "2":
                        projectProfile.status = "审核";
                        break;
                    case "3":
                        projectProfile.status = "通过审核";
                        break;
                    case "4":
                        projectProfile.status = "整理";
                        break;
                    case "5":
                        projectProfile.status = "编号";
                        break;
                    case "6":
                        projectProfile.status = "录入";
                        break;
                    case "7":
                        projectProfile.status = "入库";
                        break;
                    case "8":
                        projectProfile.status = "补录";
                        break;
                    case "9":
                        projectProfile.status = "等待编号";
                        break;
                    case "10":
                        projectProfile.status = "等待入库";
                        break;
                   
                }

            }
            if (b != null && b != "")
            {
                switch (b)
                {
                    case "1":
                        projectProfile.securityID = "机密";
                        break;
                    case "2":
                        projectProfile.securityID = "秘密";
                        break;
                    case "3":
                        projectProfile.securityID = "绝密";
                        break;
                    case "4":
                        projectProfile.securityID = "一般";
                        break;
                    case "5":
                        projectProfile.securityID = "内部";
                        break;
                    case "6":
                        projectProfile.securityID = "公开/内部";
                        break;
                   

                }

            }
            if (e!= null && e!= "")
            {
                switch (e)
                {
                    case "1":
                        projectProfile.retentionPeriodNo = "长期";
                        break;
                    case "2":
                        projectProfile.retentionPeriodNo = "永久";
                        break;
                    case "3":
                        projectProfile.retentionPeriodNo = "短期";
                        break;
                }

            }
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

              };
            if(projectProfile.isYD==true)
            {
               ViewBag.isYD = new SelectList(list, "Value", "Text",1);
            }
            else
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 0);
            }

            return View(projectProfile);
        }
        //zip文件输出流
        static ZipOutputStream zos = null;
        [HttpPost]
        public ActionResult Details(long? id, string id2, string action)
        {
            
            if (action == "返回")
            {
                if (id2 == "3")
                {
                    return RedirectToAction("linquzhengli", "PaperSettle");
                }
                if (id2 == "4")
                {
                    return RedirectToAction("informationzhengli", "PaperSettle");
                }
                return RedirectToAction("ManagementPrint", "ProjectInfoes");
            }
            if (action == "删除")
            {
                var c = from g in db.ProjectInfo
                        where g.projectID == id
                        select g;
                ProjectInfo projectInfo = c.First();
                var d = from f in db.PaperArchives
                        where f.projectID == id
                        select f;
                PaperArchives paperArchive = d.First();

                var n1 = paperArchive.projectNo;
                string no = AppDomain.CurrentDomain.BaseDirectory + "files\\guanxianWord\\" + n1;
                if (System.IO.Directory.Exists(no))
                {
                    //删除文件夹
                    Directory.Delete(no, true);
                }

                db.ProjectInfo.Remove(projectInfo);
                db.PaperArchives.Remove(paperArchive);
                db.SaveChanges();

                if (id2 == "3")
                {
                    return Content("<script >alert('删除成功！');window.location.href='/PaperSettle/linquzhengli';</script >");
                }
                if (id2 == "4")
                {
                    return Content("<script >alert('删除成功！');window.location.href='/PaperSettle/informationzhengli';</script >");
                }
                return Content("<script >alert('删除成功！');window.location.href='/ProjectInfoes/ManagementPrint';</script >");
            }
            if (action == "文件下载")
            {
                //if (string.IsNullOrEmpty(id))
                //{
                //    throw new ArgumentNullException("fileId is errror");
                //}
                int Id = Convert.ToInt32(id);
                var findFile = db.ProjectInfo.Where(a => a.projectID == Id).First();
                if (findFile.storagePath == null)
                {
                    Response.Write("<script >alert('该工程未上传文件！');window.history.back();</script>");
                }
                else
                {
                    var d = from f in db.PaperArchives
                            where f.projectID == id
                            select f;
                    PaperArchives projectInfo = d.First();
                    string no = AppDomain.CurrentDomain.BaseDirectory + "files\\jungongWord\\" + projectInfo.projectNo;

                    if (System.IO.Directory.Exists(no))
                    {
                        MemoryStream ms = null;
                        Response.ContentType = "application/octet-stream";
                        var strFileName = projectInfo.projectNo;
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + projectInfo.projectNo + ".zip");
                        ms = new MemoryStream();
                        zos = new ZipOutputStream(ms);
                        addZipEntry(no);
                        zos.Finish();
                        zos.Close();
                        Response.Clear();
                        Response.BinaryWrite(ms.ToArray());
                        Response.End();
                    }

                    //string filePath = Request.MapPath(no);
                    //string path = filePath + "/" + findFile.storagePath;
                    //以字符流的形式下载文件
                    //FileStream fs = new FileStream(path, FileMode.Open);
                    //byte[] bytes = new byte[(int)fs.Length];
                    //fs.Read(bytes, 0, bytes.Length);
                    //fs.Close();
                    //Response.ContentType = "application/octet-stream";
                    ////通知浏览器下载文件而不是打开
                    //Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(findFile.storagePath, System.Text.Encoding.UTF8));
                    //Response.BinaryWrite(bytes);
                    //Response.Flush();
                    //Response.End();
                }
            }
            return View();
        }
        //添加文件至压缩包
        private void addZipEntry(string PathStr)
        {
            DirectoryInfo di = new DirectoryInfo(PathStr);
            foreach (DirectoryInfo item in di.GetDirectories())
            {
                addZipEntry(item.FullName);
            }
            foreach (System.IO.FileInfo item in di.GetFiles())
            {
                FileStream fs = System.IO.File.OpenRead(item.FullName);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                string strEntryName = item.FullName.Replace(PathStr, "");
                ZipEntry entry = new ZipEntry(strEntryName);
                zos.PutNextEntry(entry);
                zos.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
        }

        //public ActionResult  Create(string id,string txtCurProNo,long ?id2)

        //{
        //    ViewData["pagename"] = "ProjectInfoes-Create1";

        //    //从责任书录入工程中的录入新工程按钮来录入新工程


        //    if (id2 == 1)//从责任书录入工程中的录入新工程按钮来录入新工程
        //    {
        //        id = "";
        //    }
        //    if(id==null)
        //    {
        //        ViewData["pagename"] = "ProjectInfoes-Create";
        //        id = "";
        //    }
        //    //判断该工程是否已经被接收
        //    var checkisReceive = from a in db.vw_projectProfile
        //                         where a.contractNo == id
        //                         select a.projectNo;
        //   vw_projectProfile vmprojectPfofile = new vw_projectProfile();
        //    if (checkisReceive.Count()==0||id=="")//该工程未被录入，可以录入
        //    {
        //        ViewData["tijiao"] = false;
        //        ViewData["new"] = true;
        //        //long max_ProjectNo = Convert.ToInt32(db.PaperArchives.Max(d => d.projectNo));//设置一个默认值，用户也可修改，保证8位且不重复就行
        //        //vmprojectPfofile.projectNo = max_ProjectNo + 1;
        //        string proNo = GetMaxId2();
        //        vmprojectPfofile.projectNo = long.Parse(proNo.Trim());
        //        long max_projectID = db.ProjectInfo.Max(d => d.projectID);
        //        vmprojectPfofile.projectID = max_projectID + 1;//projectID为工程信息表的主键自动+1

        //        var contract = from b in db.ContractInfo//判断是否是直接录入工程
        //                       where b.contractNo == id
        //                       select b;
        //        if (contract.Count()==0)//直接录入工程
        //        {

        //        }
        //        else//从责任书录入工程
        //        {
        //            vmprojectPfofile.contractNo = contract.First().contractNo;//关联合同编号
        //            vmprojectPfofile.projectName = contract.First().projectName;
        //            vmprojectPfofile.location = contract.First().location;
        //            if (contract.First().buildingArea == "" || contract.First().buildingArea == null)
        //            {
        //                contract.First().buildingArea = "0";
        //            }
        //            vmprojectPfofile.buildingArea = Convert.ToDouble(contract.First().buildingArea);
        //            vmprojectPfofile.developmentOrganization = contract.First().transferUnit;
        //            vmprojectPfofile.devolonpentOrgContacter = contract.First().partBweituoAgent;
        //            vmprojectPfofile.telphoneNoDevelopment = contract.First().partBcontactTel;
        //            string csDate = DateTime.Today.Date.ToString("yyyy-MM-dd");
        //            string receiveDate = DateTime.Today.Date.ToString("yyyy-MM-dd");
        //            vmprojectPfofile.csyjDate = DateTime.ParseExact(csDate.Trim(), "yyyy-MM-dd", null).Date;
        //            vmprojectPfofile.dateReceived = DateTime.ParseExact(receiveDate, "yyyy-MM-dd", null).Date;
        //            vmprojectPfofile.contractNo = id;

        //        }

        //    }
        //    else//该工程已被接收，不可重复接收
        //    {
        //        var checkisReceive1 = from c in db.vw_projectProfile
        //                              where c.contractNo == id
        //                              select c;
        //        ViewData["ClassNo"] = checkisReceive1.First().prevClassNo;
        //        ViewData["Reception"] = checkisReceive1.First().recipient;
        //        ViewData["checkname"] = 1;
        //        ViewData["tijiao"] = true;

        //        ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", checkisReceive1.First().retentionPeriodNo);
        //        ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", checkisReceive1.First().securityID);
        //        var users1 = from ad in ab.AspNetUsers
        //                where ad.DepartmentId == 2
        //                select ad;
        //        string user = User.Identity.Name;
        //        ViewBag.recipient = new SelectList(users1, "UserName", "UserName", user);
        //        if (checkisReceive1.First().memo=="材料齐全")
        //        {
        //            ViewBag.radiobutton = 1;
        //        }
        //        else
        //        {
        //            ViewBag.radiobutton = 2;
        //        }
        //        return View(checkisReceive1.First());
        //    }


        //    ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName",1);
        //    ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName",1);
        //    var users = from ad in ab.AspNetUsers
        //                where ad.DepartmentId == 2
        //                select ad;
        //    //string user1 = User.Identity.Name;
        //    ViewBag.recipient = new SelectList(users, "UserName", "UserName");


        //    if (!String.IsNullOrEmpty(txtCurProNo))
        //    {
        //        var projectNos = from p in db.PaperArchives
        //                         where p.projectNo.ToString().Contains(txtCurProNo)
        //                         orderby p.projectNo descending
        //                         select p;

        //        ViewData["Message"] = projectNos.First().projectNo.ToString();

        //    }
        //    List<SelectListItem> list = new List<SelectListItem> {
        //        new SelectListItem { Text = "是", Value = "1"},
        //        new SelectListItem { Text = "否", Value = "0"},

        //    };
        //    ViewBag.isYD = new SelectList(list, "Value", "Text");
        //    //string time = DateTime.Today.Year.ToString() + "-" + DateTime.Today.Month.ToString() + "-" + DateTime.Today.Day.ToString();

        //    vmprojectPfofile.csyjDate = DateTime.Now;
        //    vmprojectPfofile.dateReceived = DateTime.Now;

        //    return View(vmprojectPfofile);
        //}
        public ActionResult Create(string id, string txtCurProNo, long? id2)

        {
            ViewData["pagename"] = "ProjectInfoes-Create1";

            //从责任书录入工程中的录入新工程按钮来录入新工程


            if (id2 == 1)//从责任书录入工程中的录入新工程按钮来录入新工程
            {
                id = "";
            }
            else if (id2 == 0)
            {
                long proNo = long.Parse(txtCurProNo);
                var checkisReceive1 = from c in db.vw_projectProfile
                                      where c.projectNo == proNo
                                      select c;
                ViewData["ClassNo"] = checkisReceive1.First().prevClassNo;
                ViewData["Reception"] = checkisReceive1.First().recipient;
                //ViewData["checkname"] = 1;
                ViewData["tijiao"] = false;
                ViewBag.prevClassNo = checkisReceive1.First().prevClassNo;
                ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", checkisReceive1.First().retentionPeriodNo);
                ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", checkisReceive1.First().securityID);
                List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

                };
                ViewBag.isYD = new SelectList(list1, "Value", "Text", checkisReceive1.First().isYD);
                var users1 = from ad in ab.AspNetUsers
                             where ad.DepartmentId == 2
                             select ad;
                string user = User.Identity.Name;
                ViewBag.recipient = new SelectList(users1, "UserName", "UserName", user);
                //if (checkisReceive1.First().memo == "材料齐全")
                //{
                //    ViewBag.radiobutton = 1;
                //}
                //else
                //{
                //    ViewBag.radiobutton = 2;
                //}
                return View(checkisReceive1.First());
            }
            if (id == null)
            {
                ViewData["pagename"] = "ProjectInfoes-Create";
                id = "";
            }
            //判断该工程是否已经被接收
            var checkisReceive = from a in db.vw_projectProfile
                                 where a.contractNo == id
                                 select a.projectNo;
            vw_projectProfile vmprojectPfofile = new vw_projectProfile();
            if (checkisReceive.Count() == 0 || id == "")//该工程未被录入，可以录入
            {
                ViewData["tijiao"] = false;
                ViewData["new"] = true;
                //long max_ProjectNo = Convert.ToInt32(db.PaperArchives.Max(d => d.projectNo));//设置一个默认值，用户也可修改，保证8位且不重复就行
                //vmprojectPfofile.projectNo = max_ProjectNo + 1;
                string proNo = GetMaxId2();
                vmprojectPfofile.projectNo = long.Parse(proNo.Trim());
                long max_projectID = db.ProjectInfo.Max(d => d.projectID);
                vmprojectPfofile.projectID = max_projectID + 1;//projectID为工程信息表的主键自动+1
                ViewBag.id = vmprojectPfofile.projectNo;
                var contract = from b in db.ContractInfo//判断是否是直接录入工程
                               where b.contractNo == id
                               select b;
                if (contract.Count() == 0)//直接录入工程
                {

                }
                else//从责任书录入工程
                {
                    vmprojectPfofile.contractNo = contract.First().contractNo;//关联合同编号
                    vmprojectPfofile.projectName = contract.First().projectName;
                    vmprojectPfofile.location = contract.First().location;
                    if (contract.First().buildingArea == "" || contract.First().buildingArea == null)
                    {
                        contract.First().buildingArea = "0";
                    }
                    vmprojectPfofile.buildingArea = Convert.ToDouble(contract.First().buildingArea);
                    vmprojectPfofile.developmentOrganization = contract.First().transferUnit;
                    vmprojectPfofile.devolonpentOrgContacter = contract.First().partBweituoAgent;
                    vmprojectPfofile.telphoneNoDevelopment = contract.First().partBcontactTel;
                    string csDate = DateTime.Today.Date.ToString("yyyy-MM-dd");
                    string receiveDate = DateTime.Today.Date.ToString("yyyy-MM-dd");
                    vmprojectPfofile.csyjDate = DateTime.ParseExact(csDate.Trim(), "yyyy-MM-dd", null).Date;
                    vmprojectPfofile.dateReceived = DateTime.ParseExact(receiveDate, "yyyy-MM-dd", null).Date;
                    vmprojectPfofile.contractNo = id;
                    

                }

            }
            else//该工程已被接收，不可重复接收
            {
                var checkisReceive1 = from c in db.vw_projectProfile
                                      where c.contractNo == id
                                      select c;
                ViewData["ClassNo"] = checkisReceive1.First().prevClassNo;
                ViewData["Reception"] = checkisReceive1.First().recipient;
                ViewData["checkname"] = 1;
                ViewData["tijiao"] = true;

                ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", checkisReceive1.First().retentionPeriodNo);
                ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", checkisReceive1.First().securityID);
                var users1 = from ad in ab.AspNetUsers
                             where ad.DepartmentId == 2
                             select ad;
                string user = User.Identity.Name;
                ViewBag.recipient = new SelectList(users1, "UserName", "UserName", user);
                if (checkisReceive1.First().memo == "材料齐全")
                {
                    ViewBag.radiobutton = 1;
                }
                else
                {
                    ViewBag.radiobutton = 2;
                }
                return View(checkisReceive1.First());
            }


            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", 1);
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", 1);
            var users = from ad in ab.AspNetUsers
                        where ad.DepartmentId == 2
                        select ad;
            //string user1 = User.Identity.Name;
            ViewBag.recipient = new SelectList(users, "UserName", "UserName");


            if (!String.IsNullOrEmpty(txtCurProNo))
            {
                var projectNos = from p in db.PaperArchives
                                 where p.projectNo.ToString().Contains(txtCurProNo)
                                 orderby p.projectNo descending
                                 select p;
                if (projectNos.Count() != 0)
                {
                    ViewData["Message"] = projectNos.First().projectNo.ToString();
                }


            }
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

            };
            ViewBag.isYD = new SelectList(list, "Value", "Text");
            //string time = DateTime.Today.Year.ToString() + "-" + DateTime.Today.Month.ToString() + "-" + DateTime.Today.Day.ToString();

            vmprojectPfofile.csyjDate = DateTime.Now;
            vmprojectPfofile.dateReceived = DateTime.Now;

            return View(vmprojectPfofile);
        }

        public static bool CopyFolder(string sourceFolder, string targetFolder)
        {
            try
            {
                if (targetFolder[targetFolder.Length - 1] != Path.DirectorySeparatorChar)
                {
                    targetFolder += Path.DirectorySeparatorChar;
                }
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }
                // 得到源目录的文件列表，里面是包含文件以及目录路径的一个数组
                string[] fileList = Directory.GetFileSystemEntries(sourceFolder);
                foreach (var file in fileList)
                {
                    // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                    if (Directory.Exists(file))
                    {
                        CopyFolder(file, targetFolder + Path.GetFileName(file));
                    }
                    else// 否则直接Copy文件
                    {
                        System.IO.File.Copy(file, targetFolder + Path.GetFileName(file), true);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message, ex);
            }
        }
        void read_sip()
        {
            string x_path = "F:\\数据\\sip\\resource";//文件的路径，保证文件存在
            string x_path1 = "F:\\数据\\sip\\data";//文件的路径，保证文件存在
            string x_path2 = "F:\\数据\\sip\\backup";//文件的路径，保证文件存在

            CopyFolder(x_path, x_path2);
            if (!Directory.Exists(x_path))
            {
                rizhi();
            }
            else
            {
                MoveFolderTo(x_path, x_path1);
            }
        }
        //日志
        void rizhi()
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string txt = string.Empty;
            if (count == 1)
            {
                txt += string.Format("========系统重启========\r\n");
            }
            txt += string.Format("写入时间:{0},次数{1}", date, count);
            txt += "没有文件";
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                string path = "D:\\readsip.txt";//文件的路径，保证文件存在。
                fs = new FileStream(path, FileMode.Append);
                sw = new StreamWriter(fs);
                sw.WriteLine(txt);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sw.Dispose();
                sw.Close();
                fs.Dispose();
                fs.Close();
            }
            count++;
        }


        ////拷贝文件到新目录中
        //private void MoveFolderTo(string p, string p_2)
        //{
        //    //先来移动文件
        //    DirectoryInfo info = new DirectoryInfo(p);
        //    Array files = info.GetFiles();
        //    foreach (System.IO.FileInfo file in files)
        //    {
        //        string urlPath = file.FullName;
        //        string name1 = file.Name;
        //        string path = Path.GetFileNameWithoutExtension(urlPath);//获取没有后缀的文件名

        //        if (!judge_Wenjian(p_2, path))
        //        {
        //            continue;
        //        }
        //        string path1 = p_2 + "\\" + path;
        //        // path1 + "\\" + path;
        //        if (!Directory.Exists(path1))
        //        {
        //            Directory.CreateDirectory(path1);
        //        }
        //        string name = path1 + "\\" + path + ".zip";
        //        //string name = path1 + "\\" + file.Name;
        //        System.IO.File.Copy(urlPath, name, false); //复制文件
        //        System.IO.Compression.ZipFile.ExtractToDirectory(name, path1);//解压文件
        //        //读取文件

        //        DirectoryInfo directory = new DirectoryInfo(path1);
        //        //获取文件下的文件信息
        //        //System.IO.FileInfo[] files = directory.GetFiles();
        //        DirectoryInfo[] files2 = directory.GetDirectories();
        //        string path2 = files2[0].FullName;
        //        read_file(path2);
        //    }
        //}
        //拷贝文件到新目录中
        private void MoveFolderTo(string p, string p_2)
        {
            //先来移动文件
            DirectoryInfo info = new DirectoryInfo(p);
            Array files = info.GetFiles();
            foreach (System.IO.FileInfo file in files)
            {
                string urlPath = file.FullName;
                string name1 = file.Name;
                string path = Path.GetFileNameWithoutExtension(urlPath);//获取没有后缀的文件名

                if (!judge_Wenjian(p_2, path))
                {
                    continue;
                }
                string path1 = p_2 + "\\" + path;
                // path1 + "\\" + path;
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(path1);
                }
                string name = path1 + "\\" + path + ".zip";
                //string name = path1 + "\\" + file.Name;
                try
                {
                    System.IO.File.Copy(urlPath, name, false); //复制文件
                }
                catch (IOException copyError)
                {
                    Console.WriteLine(copyError.Message);
                }
                System.IO.Compression.ZipFile.ExtractToDirectory(name, path1);//解压文件

                //读取文件

                DirectoryInfo directory = new DirectoryInfo(path1);
                //获取文件下的文件信息
                //System.IO.FileInfo[] files = directory.GetFiles();
                // 错误提示 这里会报错，可能跟这次解压后的文件有关
                DirectoryInfo[] files2 = directory.GetDirectories();
                string path2 = files2[0].FullName;
                //endregion
                read_file(path2);
            }
            // 删除源文件
            //foreach (System.IO.FileInfo file in files)
            //{
            //    string urlPath = file.FullName;
            //    System.IO.File.Delete(urlPath);
            //}
            //endregion
        }
        void read_file(string path)
        {
            //查找文件夹里的 .xml文件,一个工程只有一个
            string[] filedir = Directory.GetFiles(path, "*.xml", SearchOption.TopDirectoryOnly);
            if (filedir.Length > 0)
            {
                Read_Xml(filedir[0], path);
            }
        }


        bool judge_Wenjian(string name, string name1)
        {
            DirectoryInfo info = new DirectoryInfo(name);
            Array arrDir = info.GetDirectories();
            foreach (DirectoryInfo dir1 in arrDir)
            {
                string name2 = dir1.Name;
                if (name2 == name1)
                {
                    return false;
                }
            }
            return true;
        }

        //判断在表内是否存在
        private bool isInTable(string idName, Object id, string tableName)
        {
            //新增
            string sqlString = "select * from " + tableName + " where " + idName + "=" + StringWithMarks(id.ToString());
            SqlConnection sqlConnection = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
            try
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = sqlString;
                sqlCmd.Connection = sqlConnection;
                SqlDataReader sqlDataReader = sqlCmd.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    sqlDataReader.Close();
                    return true;
                }
                else
                {
                    sqlDataReader.Close();
                    return false;
                }

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

        Object StringWithMarks(Object id)
        {
            if (id != null && (id.GetType() == typeof(String) || id.GetType() == typeof(DateTime)))
            {
                return "'" + id + "'";
            }
            else if (id == null) {
                return "''";
            }
            return id;
        }

        void Read_Xml(string wenjian, string path)
        {
            XDocument documnet = XDocument.Load(@wenjian);
            XElement root = documnet.Root;  //获得根目录
            XElement XProject = root.Element("Project");//获取项目Project标签
            XElement XSproject = XProject.Element("Sproject");   //获取工程Sproject标签
            int gcleixing = int.Parse(XSproject.Attribute("Dwgclx").Value);//单位工程类型(1房屋建筑工程 2市政道路工程 3市政桥梁工程 4市政园林绿化 5管线工程 6轨道交通工程 10其他通用类工程)
            bool isUpdate = false;
            if (gcleixing == 1)
            {
                ProjectInfo projectInfo = new ProjectInfo();
                PaperArchives paperArchives = new PaperArchives();
                var sid = from a in db.PaperArchives
                          select a.paperProjectSeqNo;
                var sid1 = from a in bb.gxPaperArchives
                           select a.paperProjectSeqNo;

                var pid = from a in db.PaperArchives
                          select a.projectID;

                //项目顺序号按竣工和管线的最大值来初始化
                long paperProjectSeqNo = 0;
                var projectID = pid.Max() + 1;
                string projectNo = GetMaxId2();

                long GCId = long.Parse(XSproject.Attribute("Id").Value);//工程ID
                int Dwgclx = int.Parse(XSproject.Attribute("Dwgclx").Value);//单位工程类型(1房屋建筑工程 2市政道路工程 3市政桥梁工程 4市政园林绿化 5管线工程 6轨道交通工程 10其他通用类工程)
                string projectName = XSproject.Attribute("Gcmc").Value;//工程名称
                string location = XSproject.Attribute("Gcdd").Value;//工程地点
                string newLocation = XSproject.Attribute("XGcdd").Value;//最新工程地点

                string developmentOrganization = XSproject.Attribute("Jsdw").Value;//建设单位
                string telphoneNoDevelopment = XSproject.Attribute("JSdwdh").Value;//建设单位固话
                string devolonpentOrgContacter = XSproject.Attribute("Jsdwlxr").Value;//建设单位联系人
                string mobilephoneNoDevelopment = XSproject.Attribute("Jsdwlxdh").Value;//建设单位联系电话

                string submitPerson = XSproject.Attribute("Bsr").Value;//报送人
                string mobilephoneSubmitPerson = XSproject.Attribute("Bsrsj").Value;//报送人手机
                string telphoneSubmitPerson = XSproject.Attribute("Bsrgh").Value;//报送人固话

                
                string constructionOrganization = XSproject.Attribute("Sgdw").Value;//施工单位
                string constructionOrgContacter = XSproject.Attribute("Sgdwlxr").Value;//施工单位法人
                string telphoneNoConstruction = XSproject.Attribute("Sgdwjsry").Value;//施工单位技术人员
                string TelConstruction = XSproject.Attribute("Sgdwlxdh").Value;//施工单位联系电话


                string disignOrganization = XSproject.Attribute("Sjdw").Value;//设计单位
                string designOrgaContacter = XSproject.Attribute("Sjdwlxr").Value;//设计单位联系人
                string telphoneNoDesignOrga = XSproject.Attribute("Sjdwsjz").Value;//设计单位设计者
                string TelDisign = XSproject.Attribute("Sjdwlxdh").Value;//设计单位联系人电话


                string jianliUnit = XSproject.Attribute("Jldw").Value;//监理单位
                string jianliUnitContacter = XSproject.Attribute("Jldwlxr").Value;//监理单位联系人
                string TelJianli = XSproject.Attribute("Jldwlxdh").Value;//监理单位联系电话
                string telphoneNoJianliUnit = XSproject.Attribute("Jldwjlz").Value;//监理者

                string Kcdw = XSproject.Attribute("Kcdw").Value;//勘察单位
                string Kcdwlxr = XSproject.Attribute("Kcdwlxr").Value;//勘察单位联系人
                string Kcdwlxdh = XSproject.Attribute("Kcdwlxdh").Value;//勘察单位联系人电话

                string Djdw = XSproject.Attribute("Djdw").Value;//代建单位
                string Ydghxkzh = XSproject.Attribute("Ydghxkzh").Value;//用地规划许可证号
                string Ghxkzh = XSproject.Attribute("Ghxkzh").Value;//规划许可证号
                string Lxpzwh = XSproject.Attribute("Lxpzwh").Value;//立项批准文号
                string Sgxkzh = XSproject.Attribute("Sgxkzh").Value;//施工许可证号

                float height = float.Parse(XSproject.Attribute("Gd").Value);//高度(m)
                string overground = XSproject.Attribute("Dscs").Value;//地上层数(层)
                string underground = XSproject.Attribute("Dxcs").Value;//地下层数(层)
                //修改1
                string Jclx = XSproject.Attribute("Jclx").Value;//基础类型(1条形基础 2独立基础 3桩基础 4筏形基础)

                double buildingArea = double.Parse(XSproject.Attribute("Jzmj").Value);//建筑面积(㎡)
                double Ydmj = double.Parse(XSproject.Attribute("Ydmj").Value);//用地面积(㎡)

                string structureTypeID = "5";

                //修改2
                //int Jglx = int.Parse(XSproject.Attribute("Jglx").Value);//结构类型(1框架 2框剪 3砖混 4排架 5钢结构 6剪力墙 7其他)
                string Jglx = XSproject.Attribute("Jglx").Value;//结构类型(1框架 2框剪 3砖混 4排架 5钢结构 6剪力墙 7其他)
                var abc = from b in db.StructureType
                        where b.structureTypeName.Contains(Jglx.Trim())
                        select b.structureTypeID;
                if (abc.Count() != 0 ) {
                    structureTypeID = abc.First();
                }
                //if (Jglx.Contains("框架"))
                //{
                //    structureTypeID = "2";
                //}
                //else if (Jglx.Contains("框剪"))
                //{
                //    structureTypeID = "6";
                //}
                //else if (Jglx.Contains("砖混"))
                //{
                //    structureTypeID = "1";
                //}
                //else if (Jglx.Contains("钢架"))
                //{
                //    structureTypeID = "4";
                //}
                //else if (Jglx.Contains("轻钢"))
                //{
                //    structureTypeID = "7";
                //}
                //else if (Jglx.Contains("砼剪"))
                //{
                //    structureTypeID = "8";
                //}
                //else if (Jglx == "")
                //{
                //    structureTypeID = "5";//4排架 5钢结构 6剪力墙 7其他  ProjectInfo
                //}
                else {
                    //1、生成一条记录存储到数据库里
                    var s = from aa in db.StructureType
                            select aa.structureTypeID;
                    var num = s.Max();
                    StructureType model = new StructureType();
                    model.structureTypeID = (int.Parse(num) + 1).ToString();
                    model.structureTypeName = Jglx;
                    db.StructureType.Add(model);
                    db.SaveChanges();
                    //2、给字段赋值
                    structureTypeID = model.structureTypeID;
                }

                string kaigongTime = XSproject.Attribute("Kgrq").Value;//开工日期 
                DateTime? projectStartDate;
                if (kaigongTime == "")
                {
                    projectStartDate = null;
                }
                else
                {
                    projectStartDate = DateTime.Parse(kaigongTime);//开工日期
                }
                string jungongTime = XSproject.Attribute("Jgrq").Value;//竣工日期
                DateTime? projectFinishDate;
                if (jungongTime == "")
                {
                    projectFinishDate = null;
                }
                else
                {
                    projectFinishDate = DateTime.Parse(jungongTime);//竣工日期
                }
                int Zs = int.Parse(XSproject.Attribute("Zs").Value);//幢数
                double Gcys = double.Parse(XSproject.Attribute("Gcys").Value);//工程预算(万元)
                double Gcjs = double.Parse(XSproject.Attribute("Gcjs").Value);//工程结算(万元)
                string remarks = XSproject.Attribute("Bz").Value;//备注
                string Guid = XSproject.Attribute("Key").Value;//工程Guid
                

                string ArchivesId = XSproject.Attribute("ArchivesId").Value;//档案馆ID
                long ConsUnitId = long.Parse(XSproject.Attribute("ConsUnitId").Value);//报建单位ID
                string coordinate = XSproject.Attribute("GIS").Value;//GPS坐标
                string Lrr = XSproject.Attribute("Lrr").Value;//录入人
                string luruTime = XSproject.Attribute("Lrsj").Value;
                DateTime? luruTime1;
                if (luruTime == "")
                {
                    luruTime1 = null;
                }
                else
                {
                    luruTime1 = DateTime.Parse(luruTime);//录入时间
                }
                string Ghxkzrq = XSproject.Attribute("Ghxkzrq").Value;
                DateTime? fazhaoTime;
                if (Ghxkzrq == "")
                {
                    fazhaoTime = null;
                }
                else
                {
                    fazhaoTime = DateTime.Parse(Ghxkzrq);//发照时间
                }
                string Jsrq = XSproject.Attribute("Jsrq").Value;
                DateTime? dateReceived;
                if (Jsrq == "")
                {
                    dateReceived = null;
                }
                else
                {
                    dateReceived = DateTime.Parse(Jsrq);//接收时间
                }
                string recipient = XSproject.Attribute("Jsr").Value;//接收人
                string Zlr = XSproject.Attribute("Zlr").Value;//整理人
                string Zlrq = XSproject.Attribute("Zlrq").Value;
                DateTime? Zlrq1;
                if (Zlrq == "")
                {
                    Zlrq1 = null;
                }
                else
                {
                    Zlrq1 = DateTime.Parse(Zlrq);//整理日期
                }

                string status = "5";
                string jsr = "";
                if (recipient == "")
                {
                    jsr = "业务科";
                }
                else
                {
                    jsr = recipient;
                }

                string collationRequirement = "整理一套";
                int characterVolumeCount = 0;
                int character1cm = 0;
                int character2cm = 0;
                int character3cm = 0;
                int character4cm = 0;
                int character5cm = 0;
                int drawingVolumeCount = 0;
                int drawing1cm = 0;
                int drawing2cm = 0;
                int drawing3cm = 0;
                int drawing4cm = 0;
                int drawing5cm = 0;
                int securityID = 1;//档案密级
                int retentionPeriodNo = 1;//保存期限

                string csyj1 = "经审查,该工程竣工档案基本齐全,建议接收进馆,提请科长审核。";//初审意见
                string csyjPerson = recipient;//初审意见人
                string collator = "Zlr";//整理人   //领取人
                string GfirstResponsible = developmentOrganization;//第一责任者
                string licenseNo = Ghxkzh;//执照号


                int isLingquYijiaoshu = 0;//是否领取移交书
                int isFafangHegezheng = 0;//是否发放合格证
                DateTime?  lqDate = dateReceived;//领取日期  在pap
                //string structureTypeID = "1";

                //string Gchecker = jsr;//审核人
                //DateTime? checkDate =  ;//审核日期
                string memo = "材料齐全";
                int GisYD = 0;//是否异地(默认为否)

                //移交单位 和 建设单位一致
                string transferUnit = developmentOrganization;

                string strSql1 = string.Format("insert into UrbanCon.dbo.PaperArchives(projectNo,projectID,paperProjectSeqNo,submitPerson,mobilephoneSubmitPerson,telphoneSubmitPerson,height,overground,underground,buildingArea,remarks,coordinate,projectStartDate,projectFinishDate,luruTime,recipient,dateReceived,collationRequirement,characterVolumeCount,character1cm,character2cm,character3cm,character4cm,character5cm,drawingVolumeCount,drawing1cm,drawing2cm,drawing3cm,drawing4cm,drawing5cm,csyj,csyjPerson,collator,firstResponsible,licenseNo,lqDate,transferUnit,structureTypeID) values(" + projectNo + "," + projectID + "," + paperProjectSeqNo + ",'" + submitPerson + "','" + mobilephoneSubmitPerson + "','" + telphoneSubmitPerson + "'," + height + ",'" + overground + "','" + underground + "'," + buildingArea + ",'" + remarks + "','" + coordinate + "','" + projectStartDate + "','" + projectFinishDate + "','" + luruTime + "','" + jsr + "','" + dateReceived + "','" + collationRequirement + "'," + characterVolumeCount + "," + character1cm + "," + character2cm + "," + character3cm + "," + character4cm + "," + character5cm + "," + drawingVolumeCount + "," + drawing1cm + "," + drawing2cm + "," + drawing3cm + "," + drawing4cm + "," + drawing5cm + ",'" + csyj1 + "','" + csyjPerson + "','" + collator + "','" + GfirstResponsible + "','" + licenseNo + "','" + lqDate + "','" + transferUnit + "','" + structureTypeID + "')");
                //string strSql2 = string.Format("insert into UrbanCon.dbo.ProjectInfo(GCId,status,projectID,ArchivesId,ConsUnitId,projectName,location,developmentOrganization,devolonpentOrgContacter,mobilephoneNoDevelopment,Djdw,constructionOrganization,constructionOrgContacter,TelConstruction,disignOrganization,designOrgaContacter,TelDisign,jianliUnit,jianliUnitContacter,TelJianli,Kcdw,Kcdwlxr,Kcdwlxdh,Ydghxkzh,Ghxkzh,Lxpzwh,Sgxkzh,Jclx,Ydmj,Zs,Gcys,Gcjs,securityID,retentionPeriodNo) values(" + GCId + ",'" + status + "'," + projectID + ",'" + ArchivesId + "'," + ConsUnitId + ",'" + projectName + "','" + location + "','" + developmentOrganization + "','" + devolonpentOrgContacter + "','" + mobilephoneNoDevelopment + "','" + Djdw + "','" + constructionOrganization + "','" + constructionOrgContacter + "','" + TelConstruction + "','" + disignOrganization + "','" + designOrgaContacter + "','" + TelDisign + "','" + jianliUnit + "','" + jianliUnitContacter + "','" + TelJianli + "','" + Kcdw + "','" + Kcdwlxr + "','" + Kcdwlxdh + "','" + Ydghxkzh + "','" + Ghxkzh + "','" + Lxpzwh + "','" + Sgxkzh + "'," + Jclx + "," + Ydmj + "," + Zs + "," + Gcys + "," + Gcys + "," + securityID + "," + retentionPeriodNo + ")");
                string strSql2 = string.Format("insert into UrbanCon.dbo.ProjectInfo(status,projectID,securityID,retentionPeriodNo,GCId,Dwgclx,projectName,location,newLocation,developmentOrganization,telphoneNoDevelopment,devolonpentOrgContacter,mobilephoneNoDevelopment,constructionOrganization,constructionOrgContacter,telphoneNoConstruction,TelConstruction,disignOrganization,designOrgaContacter,telphoneNoDesignOrga,TelDisign,jianliUnit,jianliUnitContacter,TelJianli,telphoneNoJianliUnit,Kcdw,Kcdwlxr,Kcdwlxdh,Djdw,Ydghxkzh,Ghxkzh,Lxpzwh,Sgxkzh,Jclx,Ydmj,Jglx,Zs,Gcys,Gcjs,Guid,ArchivesId,ConsUnitId,Lrr,fazhaoTime,Zlr,Zlrq,memo,isYD,isLingquYijiaoshu,isFafangHegezheng,structureTypeID) values('" + status + "'," + projectID + ",'" + securityID + "','" + retentionPeriodNo + "'," + GCId + "," + Dwgclx + ",'" + projectName + "','" + location + "','" + newLocation + "','" + developmentOrganization + "','" + telphoneNoDevelopment + "','" + devolonpentOrgContacter + "','" + mobilephoneNoDevelopment + "','" + constructionOrganization + "','" + constructionOrgContacter + "','" + telphoneNoConstruction + "','" + TelConstruction + "','" + disignOrganization + "','" + designOrgaContacter + "','" + telphoneNoDesignOrga + "','" + TelDisign + "','" + jianliUnit + "','" + jianliUnitContacter + "','" + TelJianli + "','" + telphoneNoJianliUnit + "','" + Kcdw + "','" + Kcdwlxr + "','" + Kcdwlxdh + "','" + Djdw + "','" + Ydghxkzh + "','" + Ghxkzh + "','" + Lxpzwh + "','" + Sgxkzh + "','" + Jclx + "'," + Ydmj + ",'" + Jglx + "'," + Zs + "," + Gcys + "," + Gcys + ",'" + Guid + "','" + ArchivesId + "'," + ConsUnitId + ",'" + Lrr + "','" + fazhaoTime + "','" + Zlr + "','" + Zlrq + "','" + memo + "'," + GisYD + "," + isLingquYijiaoshu +  "," + isFafangHegezheng + ",'" + structureTypeID + "')");

                long pano = -1;
                if (isInTable("GCId", GCId, "UrbanCon.dbo.ProjectInfo"))
                {
                    isUpdate = true;
                    isLingquYijiaoshu = 1;
                    isFafangHegezheng = 1;
                    var pno1 = from a in db.ProjectInfo
                               where a.GCId == GCId
                               select a.projectID;
                    long pno = pno1.First();
                    var pano1 = from a in db.PaperArchives
                               where a.projectID == pno
                                select a.paperProjectSeqNo;
                    pano = pano1.First();
                    strSql1 = string.Format("update UrbanCon.dbo.PaperArchives set submitPerson = " + StringWithMarks(submitPerson) + ",mobilephoneSubmitPerson = " + StringWithMarks(mobilephoneSubmitPerson) + ",telphoneSubmitPerson = " + StringWithMarks(telphoneSubmitPerson) + ",height = " + StringWithMarks(height) + ",overground = " + StringWithMarks(overground) + ",underground = " + StringWithMarks(underground) + ",buildingArea = " + StringWithMarks(buildingArea) + ",remarks = " + StringWithMarks(remarks) + ",coordinate = " + StringWithMarks(coordinate) + ",projectStartDate = " + StringWithMarks(projectStartDate) + ",projectFinishDate = " + StringWithMarks(projectFinishDate) + ",luruTime = " + StringWithMarks(luruTime) + ",recipient = " + StringWithMarks(jsr) + ",dateReceived = " + StringWithMarks(dateReceived) + ",csyjPerson = " + StringWithMarks(csyjPerson) + ",collator = " + StringWithMarks(collator) + ",firstResponsible = " + StringWithMarks(GfirstResponsible) + ",licenseNo = " + StringWithMarks(licenseNo) + ",lqDate = " + StringWithMarks(lqDate) + ",transferUnit = " + StringWithMarks(transferUnit) + ",structureTypeID = " + StringWithMarks(structureTypeID) + " where projectID = " + StringWithMarks(pno));
                    strSql2 = string.Format("update UrbanCon.dbo.ProjectInfo set Dwgclx = " + StringWithMarks(Dwgclx) + ",projectName = " + StringWithMarks(projectName) + ",location = " + StringWithMarks(location) + ",newLocation = " + StringWithMarks(newLocation) + ",developmentOrganization = " + StringWithMarks(developmentOrganization) + ",telphoneNoDevelopment = " + StringWithMarks(telphoneNoDevelopment) + ",devolonpentOrgContacter = " + StringWithMarks(devolonpentOrgContacter) + ",mobilephoneNoDevelopment = " + StringWithMarks(mobilephoneNoDevelopment) + ",constructionOrganization = " + StringWithMarks(constructionOrganization) + ",constructionOrgContacter = " + StringWithMarks(constructionOrgContacter) + ",telphoneNoConstruction = " + StringWithMarks(telphoneNoConstruction) + ",TelConstruction = " + StringWithMarks(TelConstruction) + ",disignOrganization = " + StringWithMarks(disignOrganization) + ",designOrgaContacter = " + StringWithMarks(designOrgaContacter) + ",telphoneNoDesignOrga = " + StringWithMarks(telphoneNoDesignOrga) + ",TelDisign = " + StringWithMarks(TelDisign) + ",jianliUnit = " + StringWithMarks(jianliUnit) + ",jianliUnitContacter = " + StringWithMarks(jianliUnitContacter) + ",TelJianli = " + StringWithMarks(TelJianli) + ",telphoneNoJianliUnit = " + StringWithMarks(telphoneNoJianliUnit) + ",Kcdw = " + StringWithMarks(Kcdw) + ",Kcdwlxr = " + StringWithMarks(Kcdwlxr) + ",Kcdwlxdh = " + StringWithMarks(Kcdwlxdh) + ",Djdw = " + StringWithMarks(Djdw) + ",Ydghxkzh = " + StringWithMarks(Ydghxkzh) + ",Ghxkzh = " + StringWithMarks(Ghxkzh) + ",Lxpzwh = " + StringWithMarks(Lxpzwh) + ",Sgxkzh = " + StringWithMarks(Sgxkzh) + ",Jclx = " + StringWithMarks(Jclx) + ",Ydmj = " + StringWithMarks(Ydmj) + ",Jglx = " + StringWithMarks(Jglx) + ",Zs = " + StringWithMarks(Zs) + ",Gcys = " + StringWithMarks(Gcys) + ",Gcjs = " + StringWithMarks(Gcys) + ",Guid = " + StringWithMarks(Guid) + ",ArchivesId = " + StringWithMarks(ArchivesId) + ",ConsUnitId = " + StringWithMarks(ConsUnitId) + ",Lrr = " + StringWithMarks(Lrr) + ",fazhaoTime = " + StringWithMarks(fazhaoTime) + ",Zlr = " + StringWithMarks(Zlr) + ",Zlrq = " + StringWithMarks(Zlrq) +  ",isLingquYijiaoshu = " + StringWithMarks(isLingquYijiaoshu) + ",isFafangHegezheng = " + StringWithMarks(isFafangHegezheng) + ",structureTypeID = " + StringWithMarks(structureTypeID) + " where GCId = " + StringWithMarks(GCId));
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

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }

                int cCount = 0;//文字数量
                int c1cm = 0;
                int c2cm = 0;
                int c3cm = 0;
                int c4cm = 0;
                int c5cm = 0;
                int dCount = 0;//图纸数量
                int d1cm = 0;
                int d2cm = 0;
                int d3cm = 0;
                int d4cm = 0;
                int d5cm = 0;
                int originalVolumeCount = 0;
                int originalInchCount = 0;//原件公分数
                int text = 0; //文字材料
                int tuzhi = 0; // 图纸总数
                int zhaopian = 0;//照片总数

                IEnumerable<XElement> XFiles = XSproject.Elements("File");
                int i = 0;
                foreach (XElement XFile in XFiles)
                {
                    i = i + 1;
                    ArchivesDetail archivesDetail = new ArchivesDetail();
                    //更新案卷信息
                    //使用sql语句更新
                    long paperNo = paperProjectSeqNo;//项目顺序号
                    long AnJuan_id = long.Parse(XFile.Attribute("Id").Value);//案卷ID
                    string archivesTitle = XFile.Attribute("Ajtm").Value;//案卷题名

                    string Bzdw = XFile.Attribute("Bzdw").Value;//编制单位

                    string Ajlx1 = XFile.Attribute("Ajlx").Value;//案卷类型
                    int Ajlx = 0;
                    if (Ajlx1 != "")
                    {
                        Ajlx = int.Parse(Ajlx1);
                    }

                    if (Ajlx == 1)//文字
                    {
                        cCount = cCount + 1;
                    }
                    else if ((Ajlx == 2))//图纸
                    {
                        dCount = dCount + 1;
                    }
                    string Ajhd1 = XFile.Attribute("Bjkd").Value;//案卷厚度
                    
                    int Ajhd = 0;
                    if (Ajlx1 != "")
                    {
                        Ajhd = int.Parse(Ajhd1);
                    }

                    if (Ajlx == 1)
                    {
                        switch (Ajhd)
                        {
                            case 1:
                                c1cm = c1cm + 1;
                                break;
                            case 2:
                                c2cm = c2cm + 1;
                                break;
                            case 3:
                                c3cm = c3cm + 1;
                                break;
                            case 4:
                                c4cm = c4cm + 1;
                                break;
                            case 5:
                                c5cm = c5cm + 1;
                                break;
                        }
                    }
                    else if (Ajlx == 2)
                    {
                        switch (Ajhd)
                        {
                            case 1:
                                d1cm = d1cm + 1;
                                break;
                            case 2:
                                d2cm = d2cm + 1;
                                break;
                            case 3:
                                d3cm = d3cm + 1;
                                break;
                            case 4:
                                d4cm = d4cm + 1;
                                break;
                            case 5:
                                d5cm = d5cm + 1;
                                break;
                        }
                    }
                    int Sfyj = 1;//是否原件
                    originalVolumeCount = originalVolumeCount + 1;//原件件数

                    int volNo = int.Parse(XFile.Attribute("AjXh").Value);//案卷序号
                    string firstResponsible = XFile.Attribute("Zrz").Value;//责任者
                    string Bzrq = XFile.Attribute("Bzrq").Value;
                    DateTime? Bzrq1;
                    if (Bzrq == "")
                    {
                        Bzrq1 = null;
                    }
                    else
                    {
                        Bzrq1 = DateTime.Parse(Bzrq);//编制时间
                    }

                    string Qssj = XFile.Attribute("Qssj").Value;
                    DateTime? Qssj1;
                    if (Qssj == "")
                    {
                        Qssj1 = null;
                    }
                    else
                    {
                        Qssj1 = DateTime.Parse(Qssj);//起始时间
                    }
                    string Zzsj = XFile.Attribute("Zzsj").Value;
                    DateTime? Zzsj1;
                    if (Zzsj == "")
                    {
                        Zzsj1 = null;
                    }
                    else
                    {
                        Zzsj1 = DateTime.Parse(Zzsj);//终止时间
                    }

                    string Ljr = XFile.Attribute("Ljr").Value;//立卷人

                    string Ljrq = XFile.Attribute("Ljrq").Value;
                    DateTime? Ljrq1;
                    if (Ljrq == "")
                    {
                        Ljrq1 = null;
                    }
                    else
                    {
                        Ljrq1 = DateTime.Parse(Ljrq);//立卷日期
                    }

                    string Ztc = XFile.Attribute("Ztc").Value;//主题词
                    string remark = XFile.Attribute("Bz").Value;//备注
                    string typist = XFile.Attribute("Lrr").Value;//录入人

                    string Lrsj = XFile.Attribute("Lrsj").Value;
                    DateTime? Lrsj1;
                    if (Lrsj == "")
                    {
                        Lrsj1 = null;
                    }
                    else
                    {
                        Lrsj1 = DateTime.Parse(Lrsj);//录入时间
                    }
                    string key1 = XFile.Attribute("Key").Value;//案卷Guid
                    long SingleProjectId = long.Parse(XFile.Attribute("SingleProjectId").Value);//单位工程ID  
                    string checker = XFile.Attribute("Shr").Value;//审核人

                    string Shrq = XFile.Attribute("Shrq").Value;
                    DateTime? Shrq1;
                    if (Shrq == "")
                    {
                        Shrq1 = null;
                    }
                    else
                    {
                        Shrq1 = DateTime.Parse(Shrq);//审核日期
                    }
                    string archivesNo1 = "null";


                    int j = 0;
                    //文字材料，图纸张数，照片数
                    
                    int textMaterial = 0;
                    int drawing = 0;
                    int PhotoCount = 0;
                    foreach (XElement record in XFile.Elements())
                    {
                        j = j + 1;
                        urban_archive.Models.FileInfo fileInfo = new urban_archive.Models.FileInfo();
                        //更新卷内信息
                        string fileName = record.Attribute("Wjtm").Value;//文件题名
                        string responsible = record.Attribute("Zrz").Value;//责任者
                        string Wth = record.Attribute("Wth").Value;//文图号
                        string startDate = record.Attribute("Qsrq").Value;//起始日期
                        string endDate = record.Attribute("Zzrq").Value;//终止日期
                        int Ztlx = int.Parse(record.Attribute("Ztlx").Value);//载体类型
                        if (Ztlx == 0) {
                            textMaterial = textMaterial + 1;
                        }
                        else if (Ztlx == 1) {
                            drawing = drawing + 1;
                        }
                        else if (Ztlx == 3) {
                            PhotoCount = PhotoCount + 1;
                        }
                        int NSfyj = int.Parse(record.Attribute("Sfyj").Value);//是否原件

                        string Sl = record.Attribute("Sl").Value;//文件数量
                        int Sl1;
                        if (Sl == "")
                        {
                            Sl1 = 0;
                        }
                        else
                        {
                            Sl1 = int.Parse(Sl);
                        }
                        string Gg = record.Attribute("Gg").Value;//规格
                        string Ty = record.Attribute("Ty").Value;//提要
                        string Wb = record.Attribute("Wb").Value;//文本
                        string Ztc1 = record.Attribute("Ztc").Value;//主题词
                        string remarks1 = record.Attribute("Fz").Value;//备注
                        string Lrr1 = record.Attribute("Lrr").Value;//录入人
                        string Lrsj2 = record.Attribute("Lrsj").Value;//录入时间
                        DateTime? Lrsj3;
                        if (Lrsj2 == "")
                        {
                            Lrsj3 = null;
                        }
                        else
                        {
                            Lrsj3 = Convert.ToDateTime(Lrsj2);
                        }

                        string seqNo = record.Attribute("Wjxh").Value;//文件序号
                        int seqNo1;
                        if (seqNo == "")
                        {
                            seqNo1 = 0;
                        }
                        else
                        {
                            seqNo1 = int.Parse(seqNo);
                        }
                        //long SProject = long.Parse(record.Attribute("SProject").Value);//单位工程ID
                        long SProject = GCId;//单位工程ID
                        long FileId = long.Parse(record.Attribute("FileId").Value);//案卷ID
                        string key2 = record.Attribute("Key").Value;//文件GUID
                        string Dzwjlj = record.Attribute("Dzwjlj").Value;//电子文件路径
                        string Md5 = record.Attribute("Md5").Value;//Md5
                        string PdfSize = record.Attribute("PdfSize").Value;//电子文件大小
                        long PdfSize1;
                        if (PdfSize == "")
                        {
                            PdfSize1 = 0;
                        }
                        else
                        {
                            PdfSize1 = long.Parse(PdfSize);
                        }
                        string Suffix = record.Attribute("Suffix").Value;//文件后缀

                        string PdfPage = record.Attribute("PdfPage").Value;//文件页数
                        int PdfPage1;
                        if (PdfPage == "")
                        {
                            PdfPage1 = 0;
                        }
                        else
                        {
                            PdfPage1 = int.Parse(PdfPage);
                        }
                        //string SliceId = record.Attribute("SliceId").Value;//电子文件存储切片目录名
                        string SliceId = "";//电子文件存储切片目录名
                        //string strSql4 = string.Format("insert into UrbanCon.dbo.FileInfo(FileId,fileName,Md5,responsible,startDate,endDate,Sl,Gg,Ty,Wb,Ztc,remarks,Lrr,Lrsj,seqNo,Dzwjlj,PdfSize,Suffix,PdfPage) values(" + FileId + ",'" + fileName + "','" + Md5 + "','" + responsible + "','" + startDate + "','" + endDate + "'," + Sl1 + ",'" + Gg + "','" + Ty + "','" + Wb + "','" + Ztc1 + "','" + remarks1 + "','" + Lrr + "','" + Lrsj3 + "'," + seqNo1 + ",'" + Dzwjlj + "'," + PdfSize1 + ",'" + Suffix + "'," + PdfPage1 + ")");

                        string strSql4 = string.Format("insert into UrbanCon.dbo.FileInfo(fileName,responsible,fileNo,startDate,endDate,Ztlx,Sfyj,Sl,Gg,Ty,Wb,Ztc,remarks,Lrr,Lrsj,seqNo,SProject,FileId,key1,Dzwjlj,Md5,PdfSize,Suffix,PdfPage,SliceId) values('" + fileName + "','" + responsible + "','" + Wth + "','" + startDate + "','" + endDate + "'," + Ztlx + "," + NSfyj + "," + Sl1 + ",'" + Gg + "','" + Ty + "','" + Wb + "','" + Ztc1 + "','" + remarks1 + "','" + Lrr1 + "','" + Lrsj3 + "'," + seqNo1 + "," + SProject + "," + FileId + ",'" + key2 + "','" + Dzwjlj + "','" + Md5 + "'," + PdfSize1 + ",'" + Suffix + "'," + PdfPage1 + ",'" + SliceId + "')");


                       
                        if (isUpdate)
                        {
                            if (pano > 0) {
                                
                                strSql4 = string.Format("insert into UrbanCon.dbo.FileInfo(fileName,responsible,fileNo,startDate,endDate,Ztlx,Sfyj,Sl,Gg,Ty,Wb,Ztc,remarks,Lrr,Lrsj,seqNo,SProject,FileId,key1,Dzwjlj,Md5,PdfSize,Suffix,PdfPage,SliceId) values('" + fileName + "','" + responsible + "','" + Wth + "','" + startDate + "','" + endDate + "'," + Ztlx + "," + NSfyj + "," + Sl1 + ",'" + Gg + "','" + Ty + "','" + Wb + "','" + Ztc1 + "','" + remarks1 + "','" + Lrr1 + "','" + Lrsj3 + "'," + seqNo1 + "," + SProject + "," + FileId + ",'" + key2 + "','" + Dzwjlj + "','" + Md5 + "'," + PdfSize1 + ",'" + Suffix + "'," + PdfPage1 + ",'" + SliceId + "')");

                            }
                        }
                        //新增
                        if (isInTable("key1", key2, "UrbanCon.dbo.FileInfo"))
                        {
                            strSql4 = string.Format("update UrbanCon.dbo.FileInfo set fileName = " + StringWithMarks(fileName) + ",responsible = " + StringWithMarks(responsible) + ",fileNo = " + StringWithMarks(Wth) + ",startDate = " + StringWithMarks(startDate) + ",endDate = " + StringWithMarks(endDate) + ",Ztlx = " + StringWithMarks(Ztlx) + ",Sfyj = " + StringWithMarks(Sfyj) + ",Sl = " + StringWithMarks(Sl) + ",Gg = " + StringWithMarks(Gg) + ",Ty = " + StringWithMarks(Ty) + ",Wb = " + StringWithMarks(Wb) + ",Ztc = " + StringWithMarks(Ztc) + ",remarks = " + StringWithMarks(remarks) + ",Lrr = " + StringWithMarks(Lrr) + ",Lrsj = " + StringWithMarks(Lrsj) + ",seqNo = " + StringWithMarks(seqNo) + ",SProject = " + StringWithMarks(SProject) + ",FileId = " + StringWithMarks(FileId) + ",Dzwjlj = " + StringWithMarks(Dzwjlj) + ",Md5 = " + StringWithMarks(Md5) + ",PdfSize = " + StringWithMarks(PdfSize1) + ",Suffix = " + StringWithMarks(Suffix) + ",PdfPage = " + StringWithMarks(PdfPage1) + ",SliceId = " + StringWithMarks(SliceId) + " where key1 = " + StringWithMarks(key2));
                        }
                        //if (isInTable("key1", key2, "UrbanCon.dbo.FileInfo"))
                        //{
                        //    strSql4 = string.Format("update UrbanCon.dbo.FileInfo set(fileName,responsible,fileNo,startDate,endDate,Ztlx,Sfyj,Sl,Gg,Ty,Wb,Ztc,remarks,Lrr,Lrsj,seqNo,SProject,FileId,Dzwjlj,Md5,PdfSize,Suffix,PdfPage,SliceId) values('" + fileName + "','" + responsible + "','" + Wth + "','" + startDate + "','" + endDate + "'," + Ztlx + "," + NSfyj + "," + Sl1 + ",'" + Gg + "','" + Ty + "','" + Wb + "','" + Ztc1 + "','" + remarks1 + "','" + Lrr1 + "','" + Lrsj3 + "'," + seqNo1 + "," + SProject + "," + FileId + "','" + Dzwjlj + "','" + Md5 + "'," + PdfSize1 + ",'" + Suffix + "'," + PdfPage1 + ",'" + SliceId + "')" + " where key1 = " + key2);
                        //}

                        SqlConnection sqlConnection2 = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
                        try
                        {
                            sqlConnection2.Open();
                            SqlCommand sqlCmd2 = new SqlCommand();
                            sqlCmd2.CommandText = strSql4;
                            sqlCmd2.Connection = sqlConnection2;
                            SqlDataReader sqlDataReader4 = sqlCmd2.ExecuteReader();
                            sqlDataReader4.Close();

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            sqlConnection2.Close();
                        }
                    }

                    //textMaterial = 0;
                    //drawing = 0;
                    //PhotoCount = 0;
                    //案卷的执照号和发照日期与工程一致
                    //string licenseNo1 = Ghxkzh;//执照号
                    
                    //string strSql3 = string.Format("insert into UrbanCon.dbo.ArchivesDetail(archivesNo,AnJuan_id,paperProjectSeqNo,SingleProjectId,archivesTitle,Bzdw,volNo,firstResponsible,Bzrq,Qssj,Zzsj,Ljr,Ljrq,Ztc,remarks,typist,typerDate,key1,SingleProjectId,checker,checkDate) values('" + archivesNo1 + "'," + AnJuan_id + "," +  paperNo + "," + SingleProjectId + ",'" + archivesTitle + "','" + Bzdw + "'," + volNo + ",'" + firstResponsible + "','" + Bzrq1 + "','" + Qssj1 + "','" + Zzsj1 + "','" + Ljr + "','" + Ljrq1 + "','" + Ztc + "','" + remark + "','" + typist + "','" + Lrsj1 + "','"+ key1 + "'," + AnJuanSingleProjectId + ",'"+ checker + "','" + Shrq1 + "')");
                    string strSql3 = string.Format("insert into UrbanCon.dbo.ArchivesDetail(archivesNo,paperProjectSeqNo,AnJuan_id,archivesTitle,Bzdw,Ajlx,Ajhd,Sfyj,volNo,firstResponsible,Bzrq,kaigongTime,jungongTime,Qssj,Zzsj,Ljr,Ljrq,Ztc,remarks,typist,typerDate,key1,SingleProjectId,checker,checkDate,UUID,licenseNo,fazhaoTime,textMaterial,drawing,PhotoCount,developmentUnit,constructionUnit,designUnit,transferUnit) values('" + archivesNo1 + "'," + paperNo + "," + AnJuan_id + ",'" + archivesTitle + "','" + Bzdw + "'," + Ajlx + "," + Ajhd + "," + Sfyj + "," + volNo + ",'" + firstResponsible + "','" + Bzrq1 + "','" + Qssj1 + "','" + Zzsj1 + "','" + Qssj1 + "','" + Zzsj1 + "','" + Ljr + "','" + Ljrq1 + "','" + Ztc + "','" + remark + "','" + typist + "','" + Lrsj1 + "','" + key1 + "'," + SingleProjectId + ",'" + checker + "','" + Shrq1 + "','" + key1 + "','" + licenseNo + "','" + fazhaoTime + "'," + textMaterial + "," + drawing + "," + PhotoCount + ",'" + developmentOrganization  + "','" + constructionOrganization + "','" + disignOrganization  + "','" + developmentOrganization + "')");
                    if (isUpdate)
                    {
                        if(pano > 0)
                        {
                            //拿archivesNo和registNo
                            var archivesNo = (from a in db.ArchivesDetail
                                              where a.paperProjectSeqNo == pano
                                              select a.archivesNo).First();
                            string archivesHead = archivesNo.Split('-')[0];

                            var archivesCategory = (from a in db.MaxArchiveRegisNo
                                                    where a.mainCategoryID == archivesHead
                                                    select a).First();

                            string maxArchivesNo = archivesCategory.maxArchiveNo;
                            string maxRegistNo = archivesCategory.maxRegisNo;
                            if (!isInTable("key1", key1, "UrbanCon.dbo.ArchivesDetail"))
                            {
                               
                                //maxArchivesRegisNo的表做更新

                                string maxArchivesNo_tail = maxArchivesNo.Split('-')[1];

                                maxArchivesNo = archivesHead + "-" + (long.Parse(maxArchivesNo_tail) + 1).ToString("D5");
                                maxRegistNo = (long.Parse(maxRegistNo) + 1).ToString("D8");
                                archivesCategory.maxArchiveNo = maxArchivesNo;

                                db.Entry(archivesCategory).State = EntityState.Modified;
                                db.SaveChanges();
                                var allmodel = from a in db.MaxArchiveRegisNo
                                               select a;
                                foreach (var model in allmodel) {
                                    model.maxRegisNo = maxRegistNo;
                                    db.Entry(model).State = EntityState.Modified;
                                }
                                db.SaveChanges();
                            }
                            strSql3 = string.Format("insert into UrbanCon.dbo.ArchivesDetail(archivesNo,paperProjectSeqNo,registrationNo,AnJuan_id,archivesTitle,Bzdw,Ajlx,Ajhd,Sfyj,volNo,firstResponsible,Bzrq,kaigongTime,jungongTime,Qssj,Zzsj,Ljr,Ljrq,Ztc,remarks,typist,typerDate,key1,SingleProjectId,checker,checkDate,UUID,licenseNo,fazhaoTime,textMaterial,drawing,PhotoCount,developmentUnit,constructionUnit,designUnit,transferUnit) values('" + maxArchivesNo + "'," + StringWithMarks(pano) + "," + StringWithMarks(maxArchivesNo)  + "," + AnJuan_id + ",'" + archivesTitle + "','" + Bzdw + "'," + Ajlx + "," + Ajhd + "," + Sfyj + "," + volNo + ",'" + firstResponsible + "','" + Bzrq1 + "','" + Qssj1 + "','" + Zzsj1 + "','" + Qssj1 + "','" + Zzsj1 + "','" + Ljr + "','" + Ljrq1 + "','" + Ztc + "','" + remark + "','" + typist + "','" + Lrsj1 + "','" + key1 + "'," + SingleProjectId + ",'" + checker + "','" + Shrq1 + "','" + key1 + "','" + licenseNo + "','" + fazhaoTime + "'," + textMaterial + "," + drawing + "," + PhotoCount + ",'" + developmentOrganization + "','" + constructionOrganization + "','" + disignOrganization + "','" + developmentOrganization + "')");
                        }
                    }
                    //新增
                    if (isInTable("key1", key1, "UrbanCon.dbo.ArchivesDetail"))
                    {
                        strSql3 = string.Format("update UrbanCon.dbo.ArchivesDetail set archivesTitle = " + StringWithMarks(archivesTitle) + ",Bzdw = " + StringWithMarks(Bzdw) + ",Ajlx = " + StringWithMarks(Ajlx) + ",Ajhd = " + StringWithMarks(Ajhd) + ",Sfyj = " + StringWithMarks(Sfyj) + ",volNo = " + StringWithMarks(volNo) + ",firstResponsible = " + StringWithMarks(firstResponsible) + ",Bzrq = " + StringWithMarks(Bzrq1) + ",kaigongTime =" + StringWithMarks(Qssj1) + ",jungongTime = " + StringWithMarks(Zzsj1) + ",Qssj = " + StringWithMarks(Qssj1) + ",Zzsj = " + StringWithMarks(Zzsj1) + ",Ljr = " + StringWithMarks(Ljr) + ",Ljrq = " + StringWithMarks(Ljrq) + ",Ztc = " + StringWithMarks(Ztc) + ",remarks = " + StringWithMarks(remarks) + ",typist= " + StringWithMarks(typist) + ",typerDate = " + StringWithMarks(Lrsj1) + ",SingleProjectId = " + StringWithMarks(SingleProjectId) + ",checker = " + StringWithMarks(checker) + ",checkDate = " + StringWithMarks(Shrq1) + ",UUID = " + StringWithMarks(key1) + ",licenseNo = " + StringWithMarks(licenseNo) + ",fazhaoTime = " + StringWithMarks(fazhaoTime) + ",textMaterial = " + StringWithMarks(textMaterial) + ",drawing = " + StringWithMarks(drawing) + ",PhotoCount = " + StringWithMarks(PhotoCount) + ",developmentUnit = " + StringWithMarks(developmentOrganization) + ",constructionUnit = " + StringWithMarks(constructionOrganization) + ",designUnit = " + StringWithMarks(disignOrganization) + ",transferUnit = " + StringWithMarks(developmentOrganization) + "where AnJuan_id =" + StringWithMarks(AnJuan_id));
       
                    }
                    //if (isInTable("key1", key1, "UrbanCon.dbo.ArchivesDetail"))
                    //{
                    //    strSql3 = string.Format("update UrbanCon.dbo.ArchivesDetail set(archivesNo,paperProjectSeqNo,AnJuan_id,archivesTitle,Bzdw,Ajlx,Ajhd,Sfyj,volNo,firstResponsible,Bzrq,Qssj,Zzsj,Ljr,Ljrq,Ztc,remarks,typist,typerDate,SingleProjectId,checker,checkDate,UUID) values('" + archivesNo1 + "'," + paperNo + "," + AnJuan_id + ",'" + archivesTitle + "','" + Bzdw + "'," + Ajlx + "," + Ajhd + "," + Sfyj + "," + volNo + ",'" + firstResponsible + "','" + Bzrq1 + "','" + Qssj1 + "','" + Zzsj1 + "','" + Ljr + "','" + Ljrq1 + "','" + Ztc + "','" + remark + "','" + typist + "','" + Lrsj1 + "'," + SingleProjectId + ",'" + checker + "','" + Shrq1 + "','" + key1 + "')" + " where key1 =" + key1);
                    //}

                    SqlConnection sqlConnection1 = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
                    try
                    {
                        sqlConnection1.Open();
                        SqlCommand sqlCmd1 = new SqlCommand();
                        sqlCmd1.CommandText = strSql3;
                        sqlCmd1.Connection = sqlConnection1;
                        SqlDataReader sqlDataReader3 = sqlCmd1.ExecuteReader();
                        sqlDataReader3.Close();

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlConnection1.Close();
                    }
                    text = text + textMaterial; //文字材料
                    tuzhi = tuzhi + drawing; // 图纸总数
                    zhaopian = zhaopian + PhotoCount;
                }/*
                //把扫描件文件剪切到指定目录下

                //string p1 = AppDomain.CurrentDomain.BaseDirectory + "声像资料\\" + projectNo;
                //System.IO.Directory.Move(s2, p1);
                string s7 = "E:\\数据\\jpg\\" + projectNo;
                //System.IO.Directory.Move(newStr, s7);
                CopyFolder(newStr, s7);

                */
                //原件公分数
                originalInchCount = 1 * c1cm + 2 * c2cm + 3 * c3cm + 4 * c4cm + 5 * c5cm + 1 * d1cm + 2 * d2cm + 3 * d3cm + 4 * d4cm + 5 * d5cm;
                int copyInchCount = 0;
                string InchCountDetail = "";

                //插入数据库
                long? projectNo1 = long.Parse(projectNo);
                var v = from a1 in db.PaperArchives
                        where a1.projectNo == projectNo1
                        select a1;
                if (v.Count() > 0)
                {
                    paperArchives = v.First();
                    paperArchives.characterVolumeCount = cCount;
                    paperArchives.character1cm = c1cm;
                    paperArchives.character2cm = c2cm;
                    paperArchives.character3cm = c3cm;
                    paperArchives.character4cm = c4cm;
                    paperArchives.character5cm = c5cm;
                    paperArchives.drawingVolumeCount = dCount;
                    paperArchives.drawing1cm = d1cm;
                    paperArchives.drawing2cm = d2cm;
                    paperArchives.drawing3cm = d3cm;
                    paperArchives.drawing4cm = d4cm;
                    paperArchives.drawing5cm = d5cm;
                    paperArchives.originalInchCount = originalInchCount;
                    paperArchives.copyInchCount = copyInchCount;
                    //
                    if (c1cm != 0)
                    {
                        InchCountDetail += "1*" + c1cm;
                    }
                    if (c2cm != 0 && InchCountDetail != "")
                    {
                        InchCountDetail = InchCountDetail + ",2*" + c2cm;
                    }
                    else if (c2cm != 0)
                    {
                        InchCountDetail = "2*" + c2cm;
                    }
                    if (c3cm != 0 && InchCountDetail != "")
                    {
                        InchCountDetail = InchCountDetail + ",3*" + c3cm;
                    }
                    else if (c3cm != 0)
                    {
                        InchCountDetail = "3*" + c3cm;
                    }
                    if (c4cm != 0 && InchCountDetail != "")
                    {
                        InchCountDetail = InchCountDetail + ",4*" + c4cm;
                    }
                    else if (c4cm != 0)
                    {
                        InchCountDetail = "4*" + c4cm;
                    }
                    if (c5cm != 0 && InchCountDetail != "")
                    {
                        InchCountDetail = InchCountDetail + ",5*" + c5cm;
                    }
                    else if (c5cm != 0)
                    {
                        InchCountDetail = "5*" + c5cm;
                    }
                    //图纸
                    if (d1cm != 0 && InchCountDetail != "")
                    {
                        InchCountDetail = InchCountDetail + ",1*" + d1cm;
                    }
                    else if (d1cm != 0)
                    {
                        InchCountDetail = "1*" + d1cm;
                    }
                    if (d2cm != 0 && InchCountDetail != "")
                    {
                        InchCountDetail = InchCountDetail + ",2*" + d2cm;
                    }
                    else if (d2cm != 0)
                    {
                        InchCountDetail = "2*" + d2cm;
                    }
                    if (d3cm != 0 && InchCountDetail != "")
                    {
                        InchCountDetail = InchCountDetail + ",3*" + d3cm;
                    }
                    else if (d3cm != 0)
                    {
                        InchCountDetail = "3*" + d3cm;
                    }
                    if (d4cm != 0 && InchCountDetail != "")
                    {
                        InchCountDetail = InchCountDetail + ",4*" + d4cm;
                    }
                    else if (d4cm != 0)
                    {
                        InchCountDetail = "4*" + d4cm;
                    }
                    if (d5cm != 0 && InchCountDetail != "")
                    {
                        InchCountDetail = InchCountDetail + ",5*" + d5cm;
                    }
                    else if (d5cm != 0)
                    {
                        InchCountDetail = "5*" + d5cm;
                    }

                    paperArchives.InchCountDetail = InchCountDetail;
                    paperArchives.originalVolumeCount = originalVolumeCount;
                    paperArchives.archivesCount = (paperArchives.characterVolumeCount + paperArchives.drawingVolumeCount).ToString();// 增加2
                    paperArchives.textMaterial = text;
                    paperArchives.drawing = tuzhi;
                    paperArchives.PhotoCount = zhaopian;

                    db.Entry(paperArchives).State = EntityState.Modified;
                    db.SaveChanges();
                }
                string strPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + "\\" + projectNo;
                if (isUpdate) {
                    string seqNo = pano.ToString();
                    strPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + "\\" + seqNo;
                }
                var projectID_update = (from ab in db.ProjectInfo
                                       where ab.GCId == GCId
                                       select ab.projectID
                                       ).First();

                var projectNo_update = (from ab in db.PaperArchives
                                             where ab.projectID == projectID_update
                                             select ab).First();
                
                if (isUpdate)
                {
                    movePathWithNEWName(wenjian, path, strPath2, projectNo_update.paperProjectSeqNo.ToString(), projectNo_update.projectNo.ToString());
                }
                else {
                    movePathWithNEWName(wenjian, path, strPath2, projectNo_update.projectNo.ToString(), projectNo_update.projectNo.ToString());
                }
                
                ////更新声像组信息
                //IEnumerable<XElement> AvGroups = XSproject.Elements("AvGroup");
                //foreach (XElement AvGroup in AvGroups)
                //{
                //    int Gid = int.Parse(AvGroup.Attribute("Id").Value);//声像组ID
                //    string Zmc = AvGroup.Attribute("Zmc").Value;//组名称
                //    string Zdd = AvGroup.Attribute("Zdd").Value;//组地点
                //    string Gjz = AvGroup.Attribute("Gjz").Value;//关键字
                //    int Sxh = int.Parse(AvGroup.Attribute("Sxh").Value);//组序号
                //    string Qssj1 = AvGroup.Attribute("Qssj").Value;//起始拍摄时间
                //    DateTime? Qssj;
                //    if (Qssj1 == "")
                //    {
                //        Qssj = null;
                //    }
                //    else
                //    {
                //        Qssj = DateTime.Parse(Qssj1);//起始拍摄时间
                //    }
                //    string Zzsj1 = AvGroup.Attribute("Zzsj").Value;//终止拍摄时间
                //    DateTime? Zzsj;
                //    if (Zzsj1 == "")
                //    {
                //        Zzsj = null;
                //    }
                //    else
                //    {
                //        Zzsj = DateTime.Parse(Zzsj1);//终止拍摄时间
                //    }
                //    string Zms = AvGroup.Attribute("Zms").Value;//组描述
                //    string Bz = AvGroup.Attribute("Bz").Value;//备注
                //    string key = AvGroup.Attribute("Key").Value;//声像组Guid
                //    string Lrr1 = AvGroup.Attribute("Lrr").Value;//录入人
                //    string Lrsj1 = AvGroup.Attribute("Lrsj").Value;//录入时间
                //    DateTime? Lrsj;
                //    if (Lrsj1 == "")
                //    {
                //        Lrsj = null;
                //    }
                //    else
                //    {
                //        Lrsj = DateTime.Parse(Lrsj1);//录入时间
                //    }
                //    long SingleProjectId = long.Parse(AvGroup.Attribute("SingleProjectId").Value);//单位工程ID

                //    string strSql5 = string.Format("insert into UrbanCon.dbo.AvGroup(Gid,Zmc,Zdd,Gjz,Sxh,Qssj,Zzsj,Zms,Bz,key1,Lrr,Lrsj,SingleProjectId) values(" + Gid + ",'" + Zmc + "','" + Zdd + "','" + Gjz + "'," + Sxh + ",'" + Qssj + "','" + Zzsj + "','" + Zms + "','" + Bz + "','" + key + "','" + Lrr1 + "','" + Lrsj + "'," + SingleProjectId + ")");
                //    if (isInTable("Sxh", Sxh, "UrbanCon.dbo.AvGroup"))
                //    {
                //        strSql5 = string.Format("update UrbanCon.dbo.AvGroup set(Gid,Zmc,Zdd,Gjz,Qssj,Zzsj,Zms,Bz,key1,Lrr,Lrsj,SingleProjectId) values(" + Gid + ",'" + Zmc + "','" + Zdd + "','" + Gjz + ",'" + Qssj + "','" + Zzsj + "','" + Zms + "','" + Bz + "','" + key + "','" + Lrr1 + "','" + Lrsj + "'," + SingleProjectId + ")" + " where Sxh = " + Sxh);
                //    }

                //    SqlConnection sqlConnection3 = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
                //    try
                //    {
                //        sqlConnection3.Open();
                //        SqlCommand sqlCmd3 = new SqlCommand();
                //        sqlCmd3.CommandText = strSql5;
                //        sqlCmd3.Connection = sqlConnection3;
                //        SqlDataReader sqlDataReader5 = sqlCmd3.ExecuteReader();
                //        sqlDataReader5.Close();

                //    }
                //    catch (Exception ex)
                //    {
                //        throw ex;
                //    }
                //    finally
                //    {
                //        sqlConnection3.Close();
                //    }

                //    //更新声像信息
                //    foreach (XElement RecordAV in AvGroup.Elements())
                //    {
                //        string Wjtm = RecordAV.Attribute("Wjtm").Value;//文件题名
                //        string Ymc = RecordAV.Attribute("Ymc").Value;//原名称
                //        string Zrz = RecordAV.Attribute("Zrz").Value;//责任者
                //        string Bqsyz = RecordAV.Attribute("Bqsyz").Value;//版权所有者
                //        string Psdd = RecordAV.Attribute("Psdd").Value;//拍摄地点
                //        string Pssy1 = RecordAV.Attribute("Pssy").Value;//拍摄事由(1工程档案收集 2指定拍摄收集 3城市面貌收集 4专题片制作)
                //        int Pssy;
                //        if (Pssy1 == "")
                //        {
                //            Pssy = 0;
                //        }
                //        else
                //        {
                //            Pssy = int.Parse(Pssy1);
                //        }
                //        //string Csyj = RecordAV.Attribute("Csyj").Value;//初审意见
                //        string Csyj = "";//初审意见
                //        //string Sxkfzr = RecordAV.Attribute("Sxkfzr").Value;//声像科责任人（声像文件初审人）
                //        string Sxkfzr = "";//声像科责任人（声像文件初审人）
                //        string Psz = RecordAV.Attribute("Psz").Value;//拍摄者
                //        string Psrq1 = RecordAV.Attribute("Psrq").Value;//拍摄日期
                //        DateTime? Psrq;
                //        if (Psrq1 == "")
                //        {
                //            Psrq = null;
                //        }
                //        else
                //        {
                //            Psrq = DateTime.Parse(Psrq1);//拍摄日期
                //        }
                //        string Ybh = RecordAV.Attribute("Ybh").Value;//原编号
                //        string Wjlx1 = RecordAV.Attribute("Wjlx").Value;//文件类型(1照片 2视频 3音频)
                //        int Wjlx;
                //        if (Wjlx1 == "")
                //        {
                //            Wjlx = 0;
                //        }
                //        else
                //        {
                //            Wjlx = int.Parse(Wjlx1);
                //        }
                //        string Ztlx1 = RecordAV.Attribute("Ztlx").Value;//载体类型（11数码照片 12黑白照片 13彩色照片 14反转照片 21数码 22光盘 23磁盘 24磁带 25录像带 31数码 32光盘 33磁盘 34磁带 35录像带
                //        int Ztlx;
                //        if (Ztlx1 == "")
                //        {
                //            Ztlx = 0;
                //        }
                //        else
                //        {
                //            Ztlx = int.Parse(Ztlx1);
                //        }
                //        string Dph = RecordAV.Attribute("Dph").Value;//底片号
                //        string Cjh = RecordAV.Attribute("Cjh").Value;//参见号
                //        string Ph = RecordAV.Attribute("Ph").Value;//盘号
                //        string Rw = RecordAV.Attribute("Rw").Value;//人物
                //        string Ly1 = RecordAV.Attribute("Ly").Value;//来源(1移交 2自拍 3征集)
                //        int Ly = 0;
                //        if (Ly1 != "")
                //        {
                //            Ly = int.Parse(Ly1);
                //        }
                //        string Jsrq1 = RecordAV.Attribute("Jsrq").Value;//接收日期
                //        DateTime? Jsrq2;
                //        if (Jsrq1 == "")
                //        {
                //            Jsrq2 = null;
                //        }
                //        else
                //        {
                //            Jsrq2 = DateTime.Parse(Jsrq1);//接收日期
                //        }
                //        string Gjz1 = RecordAV.Attribute("Gjz").Value;//关键字
                //        string Ms = RecordAV.Attribute("Ms").Value;//描述
                //        string Bz1 = RecordAV.Attribute("Bz").Value;//备注
                //        string Lrr2 = RecordAV.Attribute("Lrr").Value;//录入人
                //        string Lrsj3 = RecordAV.Attribute("Lrsj").Value;//录入时间
                //        DateTime? Lrsj2;
                //        if (Lrsj3 == "")
                //        {
                //            Lrsj2 = null;
                //        }
                //        else
                //        {
                //            Lrsj2 = DateTime.Parse(Lrsj3);//录入时间
                //        }
                //        int Sxh1 = int.Parse(RecordAV.Attribute("Sxh").Value);//声像文件序号
                //        string AVGroup1 = RecordAV.Attribute("AVGroup").Value;//声像组ID
                //        int AVGroup;
                //        if (AVGroup1 == "")
                //        {
                //            AVGroup = 0;
                //        }
                //        else
                //        {
                //            AVGroup = int.Parse(AVGroup1);
                //        }
                //        string Dzwjlj = RecordAV.Attribute("Dzwjlj").Value;//电子文件路径
                //        string Key = RecordAV.Attribute("Key").Value;//声像文件GUID
                //        string Md5 = RecordAV.Attribute("Md5").Value;//文件唯一md5验证码
                //        string Suffix = RecordAV.Attribute("Suffix").Value;//文件后缀
                //        string FileSize1 = RecordAV.Attribute("FileSize").Value;//电子文件大小
                //        long FileSize;
                //        if (FileSize1 == "")
                //        {
                //            FileSize = 0;
                //        }
                //        else
                //        {
                //            FileSize = long.Parse(FileSize1);
                //        }
                //        //string SliceName = RecordAV.Attribute("SliceName").Value;//电子文件存储切片目录名
                //        string SliceName = "";//电子文件存储切片目录名
                //        long SProject = GCId;//单位工程ID
                //        string strSql6 = string.Format("insert into UrbanCon.dbo.RecordAV(Wjtm,Ymc,Zrz,Bqsyz,Psdd,Pssy,Csyj,Sxkfzr,Psz,Psrq,Ybh,Wjlx,Ztlx,Dph,Cjh,Ph,Rw,Ly,Jsrq,Gjz,Ms,Bz,Lrr,Lrsj,Sxh,AVGroup,Dzwjlj,key1,Md5,Suffix,FileSize,SliceName,SProject) values('" + Wjtm + "','" + Ymc + "','" + Zrz + "','" + Bqsyz + "','" + Psdd + "'," + Pssy + ",'" + Csyj + "','" + Sxkfzr + "','" + Psz + "','" + Psrq + "','" + Ybh + "'," + Wjlx + "," + Ztlx + ",'" + Dph + "','" + Cjh + "','" + Ph + "','" + Rw + "'," + Ly + ",'" + Jsrq2 + "','" + Gjz1 + "','" + Ms + "','" + Bz1 + "','" + Lrr2 + "','" + Lrsj3 + "'," + Sxh1 + "," + AVGroup + ",'" + Dzwjlj + "','" + Key + "','" + Md5 + "','" + Suffix + "'," + FileSize + ",'" + SliceName + "'," + SProject + ")");
                //        if (isInTable("Sxh", Sxh1, "UrbanCon.dbo.RecordAV"))
                //        {
                //            strSql6 = string.Format("update UrbanCon.dbo.RecordAV set(Wjtm,Ymc,Zrz,Bqsyz,Psdd,Pssy,Csyj,Sxkfzr,Psz,Psrq,Ybh,Wjlx,Ztlx,Dph,Cjh,Ph,Rw,Ly,Jsrq,Gjz,Ms,Bz,Lrr,Lrsj,AVGroup,Dzwjlj,key1,Md5,Suffix,FileSize,SliceName,SProject) values('" + Wjtm + "','" + Ymc + "','" + Zrz + "','" + Bqsyz + "','" + Psdd + "'," + Pssy + ",'" + Csyj + "','" + Sxkfzr + "','" + Psz + "','" + Psrq + "','" + Ybh + "'," + Wjlx + "," + Ztlx + ",'" + Dph + "','" + Cjh + "','" + Ph + "','" + Rw + "'," + Ly + ",'" + Jsrq2 + "','" + Gjz1 + "','" + Ms + "','" + Bz1 + "','" + Lrr2 + "','" + Lrsj3 + "," + AVGroup + ",'" + Dzwjlj + "','" + Key + "','" + Md5 + "','" + Suffix + "'," + FileSize + ",'" + SliceName + "'," + SProject + ")" + " where Sxh = " + Sxh1);
                //        }
                //        SqlConnection sqlConnection4 = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
                //        try
                //        {
                //            sqlConnection4.Open();
                //            SqlCommand sqlCmd4 = new SqlCommand();
                //            sqlCmd4.CommandText = strSql6;
                //            sqlCmd4.Connection = sqlConnection4;
                //            SqlDataReader sqlDataReader6 = sqlCmd4.ExecuteReader();
                //            sqlDataReader6.Close();
                //        }
                //        catch (Exception ex)
                //        {
                //            throw ex;
                //        }
                //        finally
                //        {
                //            sqlConnection4.Close();
                //        }
                //    }
                //}
            }
            else if (gcleixing == 3)
            {
                string projectNo = GetGMaxId2();
                gxPaperArchives paperArchives = new gxPaperArchives();
                gxProjectInfo projectInfo = new gxProjectInfo();

                var sid = from a in db.PaperArchives
                          select a.paperProjectSeqNo;
                var sid1 = from a in bb.gxPaperArchives
                           select a.paperProjectSeqNo;

                var pid = from a in bb.gxPaperArchives
                          select a.projectID;

                //项目顺序号按竣工和管线的最大值来初始化
                long paperProjectSeqNo = 0;
                var projectID = pid.Max() + 1;

                long GCId = long.Parse(XSproject.Attribute("Id").Value);//工程ID
                int Dwgclx = int.Parse(XSproject.Attribute("Dwgclx").Value);//单位工程类型(1房屋建筑工程 2市政道路工程 3市政桥梁工程 4市政园林绿化 5管线工程 6轨道交通工程 10其他通用类工程)
                string projectName = XSproject.Attribute("Gcmc").Value;//工程名称
                string location = XSproject.Attribute("Gcdd").Value;//工程地点

                string newLocation = XSproject.Attribute("XGcdd").Value;//最新工程地点
                int Sfnb = int.Parse(XSproject.Attribute("Sfnb").Value);//是否内部 用于区分 离线 在线 （1是 0否 ）
                                                                        //int Sfnb = 0;//是否内部 用于区分 离线 在线 （1是 0否 ）

                string yuyanshouNo = XSproject.Attribute("YsyjsBh").Value;//验收意见书号
                string hegezhengNo = XSproject.Attribute("HgzBh").Value;//合格证书号

                string developmentOrganization = XSproject.Attribute("Jsdw").Value;//建设单位
                string telphoneNoDevelopment = XSproject.Attribute("JSdwdh").Value;//建设单位固话
                string devolonpentOrgContacter = XSproject.Attribute("Jsdwlxr").Value;//建设单位联系人
                string mobilephoneNoDevelopment = XSproject.Attribute("Jsdwlxdh").Value;//建设单位联系电话

                string submitPerson = XSproject.Attribute("Bsr").Value;//报送人
                string mobilephoneSubmitPerson = XSproject.Attribute("Bsrsj").Value;//报送人手机
                string telphoneSubmitPerson = XSproject.Attribute("Bsrgh").Value;//报送人固话

                string constructionOrganization = XSproject.Attribute("Sgdw").Value;//施工单位
                string constructionOrgContacter = XSproject.Attribute("Sgdwlxr").Value;//施工单位法人
                string TelConstruction = XSproject.Attribute("Sgdwlxdh").Value;//施工单位联系电话


                string disignOrganization = XSproject.Attribute("Sjdw").Value;//设计单位
                string designOrgaContacter = XSproject.Attribute("Sjdwlxr").Value;//设计单位联系人
                string TelDisign = XSproject.Attribute("Sjdwlxdh").Value;//设计单位联系人电话


                string jianliUnit = XSproject.Attribute("Jldw").Value;//监理单位
                string jianliUnitContacter = XSproject.Attribute("Jldwlxr").Value;//监理单位联系人
                string TelJianli = XSproject.Attribute("Jldwlxdh").Value;//监理单位联系电话

                string Kcdw = XSproject.Attribute("Kcdw").Value;//勘察单位
                string Kcdwlxr = XSproject.Attribute("Kcdwlxr").Value;//勘察单位联系人
                string Kcdwlxdh = XSproject.Attribute("Kcdwlxdh").Value;//勘察单位联系人电话

                string MapOrginisation = XSproject.Attribute("Chdw").Value;//测绘单位
                string Mapper = XSproject.Attribute("Chz").Value;//测绘者
                string TeleNoMap = XSproject.Attribute("Chlxdh").Value;//测绘联系电话
                string MapNo = XSproject.Attribute("Chbh").Value;//测绘编号

                string Djdw = XSproject.Attribute("Djdw").Value;//代建单位
                string Ydghxkzh = XSproject.Attribute("Ydghxkzh").Value;//用地规划许可证号
                string Ghxkzh = XSproject.Attribute("Ghxkzh").Value;//规划许可证号
                string Lxpzwh = XSproject.Attribute("Lxpzwh").Value;//立项批准文号
                string Sgxkzh = XSproject.Attribute("Sgxkzh").Value;//施工许可证号

                string Dxth = XSproject.Attribute("Dxth").Value;//地形图号
                string Qd = XSproject.Attribute("Qd").Value;//起点
                string Gj = XSproject.Attribute("Gj").Value;//管径(mm)
                string Gg = XSproject.Attribute("Gg").Value;//规格
                string Zd = XSproject.Attribute("Zd").Value;//止点
                string totallong = XSproject.Attribute("Zcd").Value;//总长度
                string buildingArea = XSproject.Attribute("Cd").Value;//长度
                string Material = XSproject.Attribute("Cz").Value;//材质
                string layerCount = XSproject.Attribute("Ms").Value;//埋深(m)
                string Chzb = XSproject.Attribute("Chzb").Value;//测绘坐标
                string Hz = XSproject.Attribute("Hz").Value;//荷载

                string kaigongTime = XSproject.Attribute("Kgrq").Value;//开工日期 
                DateTime? projectStartDate;
                if (kaigongTime == "")
                {
                    projectStartDate = null;
                }
                else
                {
                    projectStartDate = DateTime.Parse(kaigongTime);//开工日期
                }
                string jungongTime = XSproject.Attribute("Jgrq").Value;//竣工日期
                DateTime? projectFinishDate;
                if (jungongTime == "")
                {
                    projectFinishDate = null;
                }
                else
                {
                    projectFinishDate = DateTime.Parse(jungongTime);//竣工日期
                }
                double Gcys = double.Parse(XSproject.Attribute("Gcys").Value);//工程预算(万元)
                double Gcjs = double.Parse(XSproject.Attribute("Gcjs").Value);//工程结算(万元)
                string remarks = XSproject.Attribute("Bz").Value;//备注
                string Guid = XSproject.Attribute("Key").Value;//工程Guid

                //服务器上的路径
                string serverPath = path;
                string newStr = path + "\\" + projectNo;
                //path = urlconvertor(path);
                if (Directory.Exists(path))
                {
                    //获取文件夹信息
                    DirectoryInfo directory = new DirectoryInfo(serverPath);
                    //获取文件下的文件信息
                    //System.IO.FileInfo[] files = directory.GetFiles();
                    DirectoryInfo[] files = directory.GetDirectories();
                    foreach (var n in files)
                    {
                        if (n.Name == Guid)
                        {
                            string s1 = path + "\\" + Guid;
                            //n.MoveTo(newStr);
                            System.IO.Directory.Move(s1, newStr);
                        }
                    }
                }

                string ArchivesId = XSproject.Attribute("ArchivesId").Value;//档案馆ID
                long ConsUnitId = long.Parse(XSproject.Attribute("ConsUnitId").Value);//报建单位ID
                string coordinate = XSproject.Attribute("GIS").Value;//GPS坐标
                string Lrr = XSproject.Attribute("Lrr").Value;//录入人
                string luruTime = XSproject.Attribute("Lrsj").Value;//录入时间
                DateTime? luruTime1;
                if (luruTime == "")
                {
                    luruTime1 = null;
                }
                else
                {
                    luruTime1 = DateTime.Parse(luruTime);//录入时间
                }

                string Jsrq = XSproject.Attribute("Jsrq").Value;
                DateTime? dateReceived;
                if (Jsrq == "")
                {
                    dateReceived = null;
                }
                else
                {
                    dateReceived = DateTime.Parse(Jsrq);//接收时间
                }
                string recipient = XSproject.Attribute("Jsr").Value;//接收人
                string Zlr = XSproject.Attribute("Zlr").Value;//整理人
                string Zlrq = XSproject.Attribute("Zlrq").Value;
                DateTime? Zlrq1;
                if (Zlrq == "")
                {
                    Zlrq1 = null;
                }
                else
                {
                    Zlrq1 = DateTime.Parse(Zlrq);//整理日期
                }
                string Ghxkzrq = XSproject.Attribute("Ghxkzrq").Value;
                DateTime? fazhaoTime;
                if (Ghxkzrq == "")
                {
                    fazhaoTime = null;
                }
                else
                {
                    fazhaoTime = DateTime.Parse(Ghxkzrq);//发照时间
                }

                //进馆时间
                string Dajgrq = XSproject.Attribute("Dajgrq").Value;
                DateTime? jgDate;
                if (Dajgrq == "")
                {
                    jgDate = null;
                }
                else
                {
                    jgDate = DateTime.Parse(Dajgrq);//进馆时间
                }


                string status = "3";
                string jsr = "业务科";
                string collationRequirement = "整理一套";
                int characterVolumeCount = 0;
                int character1cm = 0;
                int character2cm = 0;
                int character3cm = 0;
                int character4cm = 0;
                int character5cm = 0;
                int drawingVolumeCount = 0;
                int drawing1cm = 0;
                int drawing2cm = 0;
                int drawing3cm = 0;
                int drawing4cm = 0;
                int drawing5cm = 0;
                int securityID = 1;
                int retentionPeriodNo = 1;
                string isNB = "外部";

                string classifyId = "1";

                string csyj1 = "经审查,该工程竣工档案基本齐全,建议接收进馆,提请科长审核。";//初审意见
                string csyjPerson = recipient;//初审意见人
                string collator = "Zlr";//整理人
                string GfirstResponsible = developmentOrganization;//第一责任者
                string licenseNo = Ghxkzh;//执照号

                //string Gchecker = jsr;//审核人
                //DateTime? checkDate =  ;//审核日期

                string memo = "材料齐全";
                int GisYD = 0;//是否异地(默认为否)


                string strSql1 = string.Format("insert into UrbanCon.dbo.gxPaperArchives(classifyID,projectNo,projectID,paperProjectSeqNo,totallong,buildingArea,Material,layerCount,remarks,submitPerson,mobilephoneSubmitPerson,telphoneSubmitPerson,coordinate,jgDate,luruTime,recipient,collationRequirement,characterVolumeCount,character1cm,character2cm,character3cm,character4cm,character5cm,drawingVolumeCount,drawing1cm,drawing2cm,drawing3cm,drawing4cm,drawing5cm,projectStartDate,projectFinishDate,csyj,csyjPerson,collator,firstResponsible,licenseNo) values('" + classifyId + "'," + projectNo + "," + projectID + "," + paperProjectSeqNo + ",'" + totallong + "','" + buildingArea + "','" + Material + "','" + layerCount + "','" + remarks + "','" + submitPerson + "','" + mobilephoneSubmitPerson + "','" + telphoneSubmitPerson + "','" + coordinate + "','" + jgDate + "','" + luruTime + "','" + jsr + "','" + collationRequirement + "'," + characterVolumeCount + ",'" + character1cm + "'," + character2cm + "," + character3cm + "," + character4cm + "," + character5cm + "," + drawingVolumeCount + ",'" + drawing1cm + "'," + drawing2cm + "," + drawing3cm + "," + drawing4cm + "," + drawing5cm + ",'" + projectStartDate + "','" + projectFinishDate + "','" + csyj1 + "','" + csyjPerson + "','" + collator + "','" + GfirstResponsible + "','" + licenseNo + "')");
                //string strSql2 = string.Format("insert into UrbanCon.dbo.ProjectInfo(GCId,status,projectID,ArchivesId,ConsUnitId,projectName,location,developmentOrganization,devolonpentOrgContacter,mobilephoneNoDevelopment,Djdw,constructionOrganization,constructionOrgContacter,TelConstruction,disignOrganization,designOrgaContacter,TelDisign,jianliUnit,jianliUnitContacter,TelJianli,Kcdw,Kcdwlxr,Kcdwlxdh,Ydghxkzh,Ghxkzh,Lxpzwh,Sgxkzh,Jclx,Ydmj,Zs,Gcys,Gcjs,securityID,retentionPeriodNo) values(" + GCId + ",'" + status + "'," + projectID + ",'" + ArchivesId + "'," + ConsUnitId + ",'" + projectName + "','" + location + "','" + developmentOrganization + "','" + devolonpentOrgContacter + "','" + mobilephoneNoDevelopment + "','" + Djdw + "','" + constructionOrganization + "','" + constructionOrgContacter + "','" + TelConstruction + "','" + disignOrganization + "','" + designOrgaContacter + "','" + TelDisign + "','" + jianliUnit + "','" + jianliUnitContacter + "','" + TelJianli + "','" + Kcdw + "','" + Kcdwlxr + "','" + Kcdwlxdh + "','" + Ydghxkzh + "','" + Ghxkzh + "','" + Lxpzwh + "','" + Sgxkzh + "'," + Jclx + "," + Ydmj + "," + Zs + "," + Gcys + "," + Gcys + "," + securityID + "," + retentionPeriodNo + ")");
                string strSql2 = string.Format("insert into UrbanCon.dbo.gxProjectInfo(classifyID,status,projectID,securityID,retentionPeriodNo,GCId,Dwgclx,projectName,location,newLocation,Sfnb,yuyanshouNo,hegezhengNo,developmentOrganization,telphoneNoDevelopment,devolonpentOrgContacter,mobilephoneNoDevelopment,constructionOrganization,constructionOrgContacter,TelConstruction,disignOrganization,designOrgaContacter,TelDisign,jianliUnit,jianliUnitContacter,TelJianli,Kcdw,Kcdwlxr,Kcdwlxdh,MapOrginisation,Mapper,TeleNoMap,MapNo,Djdw,Ydghxkzh,Ghxkzh,Lxpzwh,Sgxkzh,Dxth,Qd,Gj,Gg,Zd,Chzb,Hz,Gcys,Gcjs,key1,ArchivesId,ConsUnitId,Lrr,isNB,Zlr,Zlrq,fazhaoTime,memo,isYD) values('" + classifyId + "','" + status + "'," + projectID + ",'" + securityID + "','" + retentionPeriodNo + "'," + GCId + "," + Dwgclx + ",'" + projectName + "','" + location + "','" + newLocation + "'," + Sfnb + ",'" + yuyanshouNo + "','" + hegezhengNo + "','" + developmentOrganization + "','" + telphoneNoDevelopment + "','" + devolonpentOrgContacter + "','" + mobilephoneNoDevelopment + "','" + constructionOrganization + "','" + constructionOrgContacter + "','" + TelConstruction + "','" + disignOrganization + "','" + designOrgaContacter + "','" + TelDisign + "','" + jianliUnit + "','" + jianliUnitContacter + "','" + TelJianli + "','" + Kcdw + "','" + Kcdwlxr + "','" + Kcdwlxdh + "','" + MapOrginisation + "','" + Mapper + "','" + TeleNoMap + "','" + MapNo + "','" + Djdw + "','" + Ydghxkzh + "','" + Ghxkzh + "','" + Lxpzwh + "','" + Sgxkzh + "','" + Dxth + "','" + Qd + "','" + Gj + "','" + Gg + "','" + Zd + "','" + Chzb + "','" + Hz + "','" + Gcys + "','" + Gcjs + "','" + Guid + "','" + ArchivesId + "'," + ConsUnitId + ",'" + Lrr + "','" + isNB + "','" + Zlr + "','" + Zlrq + "','" + fazhaoTime + "','" + memo + "'," + GisYD + ")");

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

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
                int cCount = 0;//文字数量
                int c1cm = 0;
                int c2cm = 0;
                int c3cm = 0;
                int c4cm = 0;
                int c5cm = 0;
                int dCount = 0;//图纸数量
                int d1cm = 0;
                int d2cm = 0;
                int d3cm = 0;
                int d4cm = 0;
                int d5cm = 0;
                int originalVolumeCount = 0;
                int originalInchCount = 0;//原件公分数
                IEnumerable<XElement> XFiles = XSproject.Elements("File");

                int i = 0;
                foreach (XElement XFile in XFiles)
                {
                    i = i + 1;
                    ArchivesDetail archivesDetail = new ArchivesDetail();
                    //更新案卷信息
                    //使用sql语句更新
                    long paperNo = paperProjectSeqNo;//项目顺序号
                    long AnJuan_id = long.Parse(XFile.Attribute("Id").Value);//案卷ID
                    string archivesTitle = XFile.Attribute("Ajtm").Value;//案卷题名

                    string Bzdw = XFile.Attribute("Bzdw").Value;//编制单位

                    string Ajlx1 = XFile.Attribute("Ajlx").Value;//案卷类型
                    int Ajlx = 0;
                    if (Ajlx1 != "")
                    {
                        Ajlx = int.Parse(Ajlx1);
                    }

                    if (Ajlx == 1)//文字
                    {
                        cCount = cCount + 1;
                    }
                    else if ((Ajlx == 2))//图纸
                    {
                        dCount = dCount + 1;
                    }
                    string Ajhd1 = XFile.Attribute("Ajhd").Value;//案卷厚度
                    int Ajhd = 0;
                    if (Ajlx1 != "")
                    {
                        Ajhd = int.Parse(Ajhd1);
                    }

                    if (Ajlx == 1)
                    {
                        switch (Ajhd)
                        {
                            case 1:
                                c1cm = c1cm + 1;
                                break;
                            case 2:
                                c2cm = c2cm + 1;
                                break;
                            case 3:
                                c3cm = c3cm + 1;
                                break;
                            case 4:
                                c4cm = c4cm + 1;
                                break;
                            case 5:
                                c5cm = c5cm + 1;
                                break;
                        }
                    }
                    else if (Ajlx == 2)
                    {
                        switch (Ajhd)
                        {
                            case 1:
                                d1cm = d1cm + 1;
                                break;
                            case 2:
                                d2cm = d2cm + 1;
                                break;
                            case 3:
                                d3cm = d3cm + 1;
                                break;
                            case 4:
                                d4cm = d4cm + 1;
                                break;
                            case 5:
                                d5cm = d5cm + 1;
                                break;
                        }
                    }
                    int Sfyj = 1;//是否原件
                    originalVolumeCount = originalVolumeCount + 1;//原件件数

                    int volNo = int.Parse(XFile.Attribute("AjXh").Value);//案卷序号
                    string firstResponsible = XFile.Attribute("Zrz").Value;//责任者
                    string Bzrq = XFile.Attribute("Bzrq").Value;
                    DateTime? Bzrq1;
                    if (Bzrq == "")
                    {
                        Bzrq1 = null;
                    }
                    else
                    {
                        Bzrq1 = DateTime.Parse(Bzrq);//编制时间
                    }

                    string Qssj = XFile.Attribute("Qssj").Value;
                    DateTime? Qssj1;
                    if (Qssj == "")
                    {
                        Qssj1 = null;
                    }
                    else
                    {
                        Qssj1 = DateTime.Parse(Qssj);//起始时间
                    }
                    string Zzsj = XFile.Attribute("Zzsj").Value;
                    DateTime? Zzsj1;
                    if (Zzsj == "")
                    {
                        Zzsj1 = null;
                    }
                    else
                    {
                        Zzsj1 = DateTime.Parse(Zzsj);//终止时间
                    }

                    string Ljr = XFile.Attribute("Ljr").Value;//立卷人

                    string Ljrq = XFile.Attribute("Ljrq").Value;
                    DateTime? Ljrq1;
                    if (Ljrq == "")
                    {
                        Ljrq1 = null;
                    }
                    else
                    {
                        Ljrq1 = DateTime.Parse(Ljrq);//立卷日期
                    }

                    string Ztc = XFile.Attribute("Ztc").Value;//主题词
                    string remark = XFile.Attribute("Bz").Value;//备注
                    string typist = XFile.Attribute("Lrr").Value;//录入人

                    string Lrsj = XFile.Attribute("Lrsj").Value;
                    DateTime? Lrsj1;
                    if (Lrsj == "")
                    {
                        Lrsj1 = null;
                    }
                    else
                    {
                        Lrsj1 = DateTime.Parse(Lrsj);//录入时间
                    }
                    string key1 = XFile.Attribute("Key").Value;//案卷Guid
                    long SingleProjectId = long.Parse(XFile.Attribute("SingleProjectId").Value);//单位工程ID  
                    string checker = XFile.Attribute("Shr").Value;//审核人

                    string Shrq = XFile.Attribute("Shrq").Value;
                    DateTime? Shrq1;
                    if (Shrq == "")
                    {
                        Shrq1 = null;
                    }
                    else
                    {
                        Shrq1 = DateTime.Parse(Shrq);//审核日期
                    }
                    string archivesNo1 = "null";

                    string str1 = newStr;
                    if (Directory.Exists(str1))
                    {
                        //获取文件夹信息
                        DirectoryInfo directory1 = new DirectoryInfo(newStr);
                        //获取文件下的文件信息
                        DirectoryInfo[] file1s = directory1.GetDirectories();

                        foreach (var n in file1s)
                        {
                            string p1 = AppDomain.CurrentDomain.BaseDirectory + "声像资料\\" + projectNo;
                            if (n.Name == AnJuan_id.ToString())
                            {
                                str1 = str1 + "\\" + "00" + i;
                                //n.MoveTo(Path.Combine(str1));
                                string s1 = newStr + "\\" + n.Name;
                                //n.MoveTo(newStr);
                                System.IO.Directory.Move(s1, str1);
                            }
                            else if (n.Name == "avfiles" && !Directory.Exists(p1))
                            {
                                //1、在文件夹内创建工程序号文件夹
                                string s2 = newStr + "\\" + "avfiles";
                                //2、在工程序号文件夹里创建 声像视频 和 声像照片,分别在声像视频和声像照片里加前期工程
                                string s3 = newStr + "\\" + "avfiles" + "\\" + projectNo + "\\" + "声像视频" + "\\" + "前期工程";
                                string s4 = newStr + "\\" + "avfiles" + "\\" + projectNo + "\\" + "声像照片" + "\\" + "前期工程";
                                //3、把视频，音频放在 声像视频的前期工程中 ， 照片放在 声像照片 的前期工程中
                                Directory.CreateDirectory(s3);
                                Directory.CreateDirectory(s4);

                                string imgtype = "*.BMP|*.JPG|*.GIF|*.PNG|*.JPEG|*.jpeg";
                                string[] ImageType = imgtype.Split('|');

                                for (int l = 0; l < ImageType.Length; l++)
                                {
                                    string[] dirs = Directory.GetFiles(@s2, ImageType[l]);
                                    // string[] dirs = Directory.GetFiles(@"d:\\My Documents\\My Pictures", "*.jpg");
                                    foreach (string dir in dirs)
                                    {
                                        System.IO.FileInfo fi = new System.IO.FileInfo(dir);
                                        string s5 = s4 + "\\" + fi.Name;
                                        //fi.CopyTo(s5, true);
                                        fi.MoveTo(s5);
                                        //dir.CopyTo(s2);
                                    }
                                }
                                //剩下文件放到视频组里
                                string[] dirs1 = Directory.GetFiles(@s2);
                                foreach (string dir in dirs1)
                                {
                                    System.IO.FileInfo fi = new System.IO.FileInfo(dir);
                                    string s5 = s3 + "\\" + fi.Name;
                                    //fi.CopyTo(s5, true);
                                    fi.MoveTo(s5);
                                    //dir.CopyTo(s2);
                                }


                                //4、把工程序号文件夹放到目录下
                                //string p1 = AppDomain.CurrentDomain.BaseDirectory + "声像资料\\" + projectNo;
                                //System.IO.Directory.Move(s2, p1);
                                string s6 = s2 + "\\" + projectNo;
                                //System.IO.Directory.Move(s6, p1);
                                CopyFolder(s6, p1);
                            }
                        }
                    }

                    //string strSql3 = string.Format("insert into UrbanCon.dbo.ArchivesDetail(archivesNo,AnJuan_id,paperProjectSeqNo,SingleProjectId,archivesTitle,Bzdw,volNo,firstResponsible,Bzrq,Qssj,Zzsj,Ljr,Ljrq,Ztc,remarks,typist,typerDate,key1,SingleProjectId,checker,checkDate) values('" + archivesNo1 + "'," + AnJuan_id + "," +  paperNo + "," + SingleProjectId + ",'" + archivesTitle + "','" + Bzdw + "'," + volNo + ",'" + firstResponsible + "','" + Bzrq1 + "','" + Qssj1 + "','" + Zzsj1 + "','" + Ljr + "','" + Ljrq1 + "','" + Ztc + "','" + remark + "','" + typist + "','" + Lrsj1 + "','"+ key1 + "'," + AnJuanSingleProjectId + ",'"+ checker + "','" + Shrq1 + "')");
                    string strSql3 = string.Format("insert into UrbanCon.dbo.gxArchivesDetail(archivesNo,paperProjectSeqNo,AnJuan_id,archivesTitle,Bzdw,Sfyj,volNo,firstResponsible,Bzrq,Qssj,Zzsj,Ljr,Ljrq,Ztc,remarks,typist,typerDate,key1,SingleProjectId,checker,checkDate,UUID) values('" + archivesNo1 + "'," + paperNo + "," + AnJuan_id + ",'" + archivesTitle + "','" + Bzdw + "'," + Sfyj + "," + volNo + ",'" + firstResponsible + "','" + Bzrq1 + "','" + Qssj1 + "','" + Zzsj1 + "','" + Ljr + "','" + Ljrq1 + "','" + Ztc + "','" + remark + "','" + typist + "','" + Lrsj1 + "','" + key1 + "'," + SingleProjectId + ",'" + checker + "','" + Shrq1 + "','" + key1 + "')");
                    SqlConnection sqlConnection1 = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
                    try
                    {
                        sqlConnection1.Open();
                        SqlCommand sqlCmd1 = new SqlCommand();
                        sqlCmd1.CommandText = strSql3;
                        sqlCmd1.Connection = sqlConnection1;
                        SqlDataReader sqlDataReader3 = sqlCmd1.ExecuteReader();
                        sqlDataReader3.Close();

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlConnection1.Close();
                    }
                    int j = 0;
                    foreach (XElement record in XFile.Elements())
                    {
                        j = j + 1;
                        urban_archive.Models.FileInfo fileInfo = new urban_archive.Models.FileInfo();
                        //更新卷内信息
                        string fileName = record.Attribute("Wjtm").Value;//文件题名
                        string responsible = record.Attribute("Zrz").Value;//责任者
                        string Wth = record.Attribute("Wth").Value;//文图号
                        string startDate = record.Attribute("Qsrq").Value;//起始日期
                        string endDate = record.Attribute("Zzrq").Value;//终止日期
                        int Ztlx = int.Parse(record.Attribute("Ztlx").Value);//载体类型
                        int NSfyj = int.Parse(record.Attribute("Sfyj").Value);//是否原件

                        string Sl = record.Attribute("Sl").Value;//文件数量
                        int Sl1;
                        if (Sl == "")
                        {
                            Sl1 = 0;
                        }
                        else
                        {
                            Sl1 = int.Parse(Sl);
                        }
                        string Gg1 = record.Attribute("Gg").Value;//规格
                        string Ty = record.Attribute("Ty").Value;//提要
                        string Wb = record.Attribute("Wb").Value;//文本
                        string Ztc1 = record.Attribute("Ztc").Value;//主题词
                        string remarks1 = record.Attribute("Fz").Value;//备注
                        string Lrr1 = record.Attribute("Lrr").Value;//录入人

                        string Lrsj2 = record.Attribute("Lrsj").Value;//录入时间
                        DateTime? Lrsj3;
                        if (Lrsj2 == "")
                        {
                            Lrsj3 = null;
                        }
                        else
                        {
                            Lrsj3 = Convert.ToDateTime(Lrsj2);
                        }

                        string seqNo = record.Attribute("Wjxh").Value;//文件序号
                        int seqNo1;
                        if (seqNo == "")
                        {
                            seqNo1 = 0;
                        }
                        else
                        {
                            seqNo1 = int.Parse(seqNo);
                        }
                        //long SProject = long.Parse(record.Attribute("SProject").Value);//单位工程ID
                        long SProject = GCId;//单位工程ID
                        long FileId = long.Parse(record.Attribute("FileId").Value);//案卷ID
                        string key2 = record.Attribute("Key").Value;//文件GUID
                        string Dzwjlj = record.Attribute("Dzwjlj").Value;//电子文件路径
                        string Md5 = record.Attribute("Md5").Value;//Md5
                        string PdfSize = record.Attribute("PdfSize").Value;//电子文件大小
                        long PdfSize1;
                        if (PdfSize == "")
                        {
                            PdfSize1 = 0;
                        }
                        else
                        {
                            PdfSize1 = long.Parse(PdfSize);
                        }
                        string Suffix = record.Attribute("Suffix").Value;//文件后缀

                        string PdfPage = record.Attribute("PdfPage").Value;//文件页数
                        int PdfPage1;
                        if (PdfPage == "")
                        {
                            PdfPage1 = 0;
                        }
                        else
                        {
                            PdfPage1 = int.Parse(PdfPage);
                        }
                        //string SliceId = record.Attribute("SliceId").Value;//电子文件存储切片目录名
                        string SliceId = "";//电子文件存储切片目录名

                        //修改文件名
                        string str2 = str1;
                        if (Directory.Exists(str2))
                        {
                            var file3s = Directory.GetFiles(str2, "*.pdf");
                            foreach (var n in file3s)
                            {
                                string pdf = Dzwjlj;
                                int k = pdf.LastIndexOf("\\");
                                pdf = pdf.Substring(k + 1);

                                if (Path.GetFileName(n) == pdf)
                                {
                                    string str3 = str2 + "\\" + j + ".pdf";
                                    str2 = str2 + "\\" + pdf;
                                    System.IO.File.Move(str2, str3);
                                }
                            }
                        }

                        //string strSql4 = string.Format("insert into UrbanCon.dbo.FileInfo(FileId,fileName,Md5,responsible,startDate,endDate,Sl,Gg,Ty,Wb,Ztc,remarks,Lrr,Lrsj,seqNo,Dzwjlj,PdfSize,Suffix,PdfPage) values(" + FileId + ",'" + fileName + "','" + Md5 + "','" + responsible + "','" + startDate + "','" + endDate + "'," + Sl1 + ",'" + Gg + "','" + Ty + "','" + Wb + "','" + Ztc1 + "','" + remarks1 + "','" + Lrr + "','" + Lrsj3 + "'," + seqNo1 + ",'" + Dzwjlj + "'," + PdfSize1 + ",'" + Suffix + "'," + PdfPage1 + ")");
                        string strSql4 = string.Format("insert into UrbanCon.dbo.gxFileInfo(fileName,responsible,fileNo,startDate,endDate,Ztlx,Sfyj,Sl,Gg,Ty,Wb,Ztc,remarks,Lrr,Lrsj,seqNo,SProject,FileId,key1,Dzwjlj,Md5,PdfSize,Suffix,PdfPage,SliceId) values('" + fileName + "','" + responsible + "','" + Wth + "','" + startDate + "','" + endDate + "'," + Ztlx + "," + NSfyj + "," + Sl1 + ",'" + Gg1 + "','" + Ty + "','" + Wb + "','" + Ztc1 + "','" + remarks1 + "','" + Lrr1 + "','" + Lrsj3 + "'," + seqNo1 + "," + SProject + "," + FileId + ",'" + key2 + "','" + Dzwjlj + "','" + Md5 + "'," + PdfSize1 + ",'" + Suffix + "'," + PdfPage1 + ",'" + SliceId + "')");
                        SqlConnection sqlConnection2 = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
                        try
                        {
                            sqlConnection2.Open();
                            SqlCommand sqlCmd2 = new SqlCommand();
                            sqlCmd2.CommandText = strSql4;
                            sqlCmd2.Connection = sqlConnection2;
                            SqlDataReader sqlDataReader4 = sqlCmd2.ExecuteReader();
                            sqlDataReader4.Close();

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            sqlConnection2.Close();
                        }

                    }
                }
                //把扫描件文件剪切到指定目录下

                //string p1 = AppDomain.CurrentDomain.BaseDirectory + "声像资料\\" + projectNo;
                //System.IO.Directory.Move(s2, p1);
                string s7 = "E:\\数据\\jpg\\" + projectNo;
                //System.IO.Directory.Move(newStr, s7);
                CopyFolder(newStr, s7);

                //原件公分数
                originalInchCount = 1 * c1cm + 2 * c2cm + 3 * c3cm + 4 * c4cm + 5 * c5cm + 1 * d1cm + 2 * d2cm + 3 * d3cm + 4 * d4cm + 5 * d5cm;
                int copyInchCount = 0;
                string InchCountDetail = "";

                //插入数据库
                long? projectNo1 = long.Parse(projectNo);
                var v = from a in bb.gxPaperArchives
                        where a.projectNo == projectNo1
                        select a;

                paperArchives = v.First();
                paperArchives.characterVolumeCount = cCount;
                paperArchives.character1cm = c1cm.ToString();
                paperArchives.character2cm = c2cm;
                paperArchives.character3cm = c3cm;
                paperArchives.character4cm = c4cm;
                paperArchives.character5cm = c5cm;
                paperArchives.drawingVolumeCount = dCount;
                paperArchives.drawing1cm = d1cm.ToString();
                paperArchives.drawing2cm = d2cm;
                paperArchives.drawing3cm = d3cm;
                paperArchives.drawing4cm = d4cm;
                paperArchives.drawing5cm = d5cm;
                paperArchives.originalInchCount = originalInchCount;
                paperArchives.copyInchCount = copyInchCount;
                paperArchives.InchCountDetail = InchCountDetail;
                paperArchives.originalVolumeCount = originalVolumeCount;
                bb.Entry(paperArchives).State = EntityState.Modified;
                bb.SaveChanges();

                //更新声像组信息
                IEnumerable<XElement> AvGroups = XSproject.Elements("AvGroup");
                foreach (XElement AvGroup in AvGroups)
                {
                    int Gid = int.Parse(AvGroup.Attribute("Id").Value);//声像组ID
                    string Zmc = AvGroup.Attribute("Zmc").Value;//组名称
                    string Zdd = AvGroup.Attribute("Zdd").Value;//组地点
                    string Gjz = AvGroup.Attribute("Gjz").Value;//关键字
                    int Sxh = int.Parse(AvGroup.Attribute("Sxh").Value);//组序号
                    string Qssj1 = AvGroup.Attribute("Qssj").Value;//起始拍摄时间
                    DateTime? Qssj;
                    if (Qssj1 == "")
                    {
                        Qssj = null;
                    }
                    else
                    {
                        Qssj = DateTime.Parse(Qssj1);//起始拍摄时间
                    }
                    string Zzsj1 = AvGroup.Attribute("Zzsj").Value;//终止拍摄时间
                    DateTime? Zzsj;
                    if (Zzsj1 == "")
                    {
                        Zzsj = null;
                    }
                    else
                    {
                        Zzsj = DateTime.Parse(Zzsj1);//终止拍摄时间
                    }
                    string Zms = AvGroup.Attribute("Zms").Value;//组描述
                    string Bz = AvGroup.Attribute("Bz").Value;//备注
                    string key = AvGroup.Attribute("Key").Value;//声像组Guid
                    string Lrr1 = AvGroup.Attribute("Lrr").Value;//录入人
                    string Lrsj1 = AvGroup.Attribute("Lrsj").Value;//录入时间
                    DateTime? Lrsj;
                    if (Lrsj1 == "")
                    {
                        Lrsj = null;
                    }
                    else
                    {
                        Lrsj = DateTime.Parse(Lrsj1);//录入时间
                    }
                    long SingleProjectId = long.Parse(AvGroup.Attribute("SingleProjectId").Value);//单位工程ID

                    string strSql5 = string.Format("insert into UrbanCon.dbo.AvGroup(Gid,Zmc,Zdd,Gjz,Sxh,Qssj,Zzsj,Zms,Bz,key1,Lrr,Lrsj,SingleProjectId) values(" + Gid + ",'" + Zmc + "','" + Zdd + "','" + Gjz + "'," + Sxh + ",'" + Qssj + "','" + Zzsj + "','" + Zms + "','" + Bz + "','" + key + "','" + Lrr1 + "','" + Lrsj + "'," + SingleProjectId + ")");
                    SqlConnection sqlConnection3 = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
                    try
                    {
                        sqlConnection3.Open();
                        SqlCommand sqlCmd3 = new SqlCommand();
                        sqlCmd3.CommandText = strSql5;
                        sqlCmd3.Connection = sqlConnection3;
                        SqlDataReader sqlDataReader5 = sqlCmd3.ExecuteReader();
                        sqlDataReader5.Close();

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlConnection3.Close();
                    }

                    //更新声像信息
                    foreach (XElement RecordAV in AvGroup.Elements())
                    {
                        string Wjtm = RecordAV.Attribute("Wjtm").Value;//文件题名
                        string Ymc = RecordAV.Attribute("Ymc").Value;//原名称
                        string Zrz = RecordAV.Attribute("Zrz").Value;//责任者
                        string Bqsyz = RecordAV.Attribute("Bqsyz").Value;//版权所有者
                        string Psdd = RecordAV.Attribute("Psdd").Value;//拍摄地点
                        string Pssy1 = RecordAV.Attribute("Pssy").Value;//拍摄事由(1工程档案收集 2指定拍摄收集 3城市面貌收集 4专题片制作)
                        int Pssy = 0;
                        if (Pssy1 != "")
                        {
                            Pssy = int.Parse(Pssy1);
                        }
                        //string Csyj = RecordAV.Attribute("Csyj").Value;//初审意见
                        string Csyj = "";//初审意见
                        string Sxkfzr = RecordAV.Attribute("Sxkfzr").Value;//声像科责任人（声像文件初审人）
                        string Psz = RecordAV.Attribute("Psz").Value;//拍摄者
                        string Psrq1 = RecordAV.Attribute("Psrq").Value;//拍摄日期
                        DateTime? Psrq;
                        if (Psrq1 == "")
                        {
                            Psrq = null;
                        }
                        else
                        {
                            Psrq = DateTime.Parse(Psrq1);//拍摄日期
                        }
                        string Ybh = RecordAV.Attribute("Ybh").Value;//原编号
                        string Wjlx1 = RecordAV.Attribute("Wjlx").Value;//文件类型(1照片 2视频 3音频)
                        int Wjlx = 0;
                        if (Wjlx1 != "")
                        {
                            Wjlx = int.Parse(Wjlx1);
                        }
                        string Ztlx1 = RecordAV.Attribute("Ztlx").Value;//载体类型（11数码照片 12黑白照片 13彩色照片 14反转照片 21数码 22光盘 23磁盘 24磁带 25录像带 31数码 32光盘 33磁盘 34磁带 35录像带
                        int Ztlx = 0;
                        if (Ztlx1 != "")
                        {
                            Ztlx = int.Parse(Ztlx1);
                        }
                        string Dph = RecordAV.Attribute("Dph").Value;//底片号
                        string Cjh = RecordAV.Attribute("Cjh").Value;//参见号
                        string Ph = RecordAV.Attribute("Ph").Value;//盘号
                        string Rw = RecordAV.Attribute("Rw").Value;//人物
                        string Ly1 = RecordAV.Attribute("Ly").Value;//来源(1移交 2自拍 3征集)
                        int Ly = 0;
                        if (Ly1 != "")
                        {
                            Ly = int.Parse(Ly1);
                        }
                        string Jsrq1 = RecordAV.Attribute("Jsrq").Value;//接收日期
                        DateTime? SJsrq;
                        if (Jsrq1 == "")
                        {
                            SJsrq = null;
                        }
                        else
                        {
                            SJsrq = DateTime.Parse(Jsrq1);//接收日期
                        }
                        string Gjz1 = RecordAV.Attribute("Gjz").Value;//关键字
                        string Ms = RecordAV.Attribute("Ms").Value;//描述
                        string Bz1 = RecordAV.Attribute("Bz").Value;//备注
                        string Lrr2 = RecordAV.Attribute("Lrr").Value;//录入人
                        string Lrsj3 = RecordAV.Attribute("Lrsj").Value;//录入时间
                        DateTime? Lrsj2;
                        if (Lrsj3 == "")
                        {
                            Lrsj2 = null;
                        }
                        else
                        {
                            Lrsj2 = DateTime.Parse(Lrsj3);//录入时间
                        }
                        int Sxh1 = int.Parse(RecordAV.Attribute("Sxh").Value);//声像文件序号
                        string AVGroup1 = RecordAV.Attribute("AVGroup").Value;//声像组ID
                        int AVGroup = 0;
                        if (AVGroup1 != "")
                        {
                            AVGroup = int.Parse(AVGroup1);
                        }
                        string Dzwjlj = RecordAV.Attribute("Dzwjlj").Value;//电子文件路径
                        string Key = RecordAV.Attribute("Key").Value;//声像文件GUID
                        string Md5 = RecordAV.Attribute("Md5").Value;//文件唯一md5验证码
                        string Suffix = RecordAV.Attribute("Suffix").Value;//文件后缀
                        string FileSize1 = RecordAV.Attribute("FileSize").Value;//电子文件大小
                        long FileSize = 0;
                        if (FileSize1 != "")
                        {
                            FileSize = long.Parse(FileSize1);
                        }
                        //string SliceName = RecordAV.Attribute("SliceName").Value;//电子文件存储切片目录名
                        string SliceName = "";//电子文件存储切片目录名
                        long SProject = GCId;//单位工程ID
                        string strSql6 = string.Format("insert into UrbanCon.dbo.RecordAV(Wjtm,Ymc,Zrz,Bqsyz,Psdd,Pssy,Csyj,Sxkfzr,Psz,Psrq,Ybh,Wjlx,Ztlx,Dph,Cjh,Ph,Rw,Ly,Jsrq,Gjz,Ms,Bz,Lrr,Lrsj,Sxh,AVGroup,Dzwjlj,key1,Md5,Suffix,FileSize,SliceName,SProject) values('" + Wjtm + "','" + Ymc + "','" + Zrz + "','" + Bqsyz + "','" + Psdd + "'," + Pssy + ",'" + Csyj + "','" + Sxkfzr + "','" + Psz + "','" + Psrq + "','" + Ybh + "'," + Wjlx + "," + Ztlx + ",'" + Dph + "','" + Cjh + "','" + Ph + "','" + Rw + "'," + Ly + ",'" + Jsrq + "','" + Gjz1 + "','" + Ms + "','" + Bz1 + "','" + Lrr2 + "','" + Lrsj3 + "'," + Sxh1 + "," + AVGroup + ",'" + Dzwjlj + "','" + Key + "','" + Md5 + "','" + Suffix + "'," + FileSize + ",'" + SliceName + "'," + SProject + ")");
                        SqlConnection sqlConnection4 = new SqlConnection("Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web");
                        try
                        {
                            sqlConnection4.Open();
                            SqlCommand sqlCmd4 = new SqlCommand();
                            sqlCmd4.CommandText = strSql6;
                            sqlCmd4.Connection = sqlConnection4;
                            SqlDataReader sqlDataReader6 = sqlCmd4.ExecuteReader();
                            sqlDataReader6.Close();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            sqlConnection4.Close();
                        }
                    }
                }
            }
            else
            {

            }
        }

        void movePathWithNEWName(string wenjian, string path, string pathWillReach, string folderName,string projectNo)
        {
            XDocument documnet = XDocument.Load(@wenjian);
            XElement root = documnet.Root;  //获得根目录
            XElement XProject = root.Element("Project");//获取项目Project标签
            XElement XSproject = XProject.Element("Sproject");   //获取工程Sproject标签
            //string projectNo = (long.Parse(GetMaxId2()) - 1).ToString();
            int gcleixing = int.Parse(XSproject.Attribute("Dwgclx").Value);//单位工程类型(1房屋建筑工程 2市政道路工程 3市政桥梁工程 4市政园林绿化 5管线工程 6轨道交通工程 10其他通用类工程)
            string Guid = XSproject.Attribute("Key").Value;//工程Guid
            if (gcleixing == 1)
            {
                IEnumerable<XElement> XFiles = XSproject.Elements("File");
                int i = 0;
                string newStr = path + "\\" + folderName;
                //该文件夹名称
                if (Directory.Exists(path))
                {
                    //获取文件夹信息
                    DirectoryInfo directory = new DirectoryInfo(path);
                    //获取文件下的文件信息
                    //System.IO.FileInfo[] files = directory.GetFiles();
                    DirectoryInfo[] files = directory.GetDirectories();
                    foreach (var n in files)
                    {
                        if (n.Name == Guid)
                        {
                            string s1 = path + "\\" + Guid;
                            //n.MoveTo(newStr);
                            System.IO.Directory.Move(s1, newStr);
                        }
                    }
                }
                foreach (XElement XFile in XFiles)
                {
                    i = i + 1;
                    long AnJuan_id = long.Parse(XFile.Attribute("Id").Value);

                    string str1 = newStr;
                    if (Directory.Exists(str1))
                    {
                        //获取文件夹信息
                        DirectoryInfo directory1 = new DirectoryInfo(newStr);
                        //获取文件下的文件信息
                        DirectoryInfo[] file1s = directory1.GetDirectories();

                        foreach (var n in file1s)
                        {
                            string p1 = AppDomain.CurrentDomain.BaseDirectory + "声像资料\\" + projectNo;
                            if (n.Name == AnJuan_id.ToString())
                            {
                                //加temp的是因为有可能重名会造成重排序号失败，在循环外再改回正确的名字
                                str1 = str1 + "\\temp_" + i;
                                //n.MoveTo(Path.Combine(str1));
                                string s1 = newStr + "\\" + n.Name;
                                //n.MoveTo(newStr);
                                System.IO.Directory.Move(s1, str1);
                            }
                            else if (n.Name == "avfiles" && !Directory.Exists(p1))
                            {
                                //1、在文件夹内创建工程序号文件夹
                                string s2 = newStr + "\\" + "avfiles";
                                //2、在工程序号文件夹里创建 声像视频 和 声像照片,分别在声像视频和声像照片里加前期工程
                                string s3 = newStr + "\\" + "avfiles" + "\\" + projectNo + "\\" + "声像视频" + "\\" + "前期工程";
                                string s4 = newStr + "\\" + "avfiles" + "\\" + projectNo + "\\" + "声像照片" + "\\" + "前期工程";
                                //3、把视频，音频放在 声像视频的前期工程中 ， 照片放在 声像照片 的前期工程中
                                Directory.CreateDirectory(s3);
                                Directory.CreateDirectory(s4);

                                string imgtype = "*.BMP|*.JPG|*.GIF|*.PNG|*.JPEG|*.jpeg";
                                string[] ImageType = imgtype.Split('|');

                                for (int l = 0; l < ImageType.Length; l++)
                                {
                                    string[] dirs = Directory.GetFiles(@s2, ImageType[l]);
                                    // string[] dirs = Directory.GetFiles(@"d:\\My Documents\\My Pictures", "*.jpg");
                                    foreach (string dir in dirs)
                                    {
                                        System.IO.FileInfo fi = new System.IO.FileInfo(dir);
                                        string s5 = s4 + "\\" + fi.Name;
                                        //fi.CopyTo(s5, true);
                                        fi.MoveTo(s5);
                                        //dir.CopyTo(s2);
                                    }
                                }
                                //剩下文件放到视频组里
                                string[] dirs1 = Directory.GetFiles(@s2);
                                foreach (string dir in dirs1)
                                {
                                    System.IO.FileInfo fi = new System.IO.FileInfo(dir);
                                    string s5 = s3 + "\\" + fi.Name;
                                    //fi.CopyTo(s5, true);
                                    fi.MoveTo(s5);
                                    //dir.CopyTo(s2);
                                }

                                //4、把工程序号文件夹放到目录下
                                //string p1 = AppDomain.CurrentDomain.BaseDirectory + "声像资料\\" + projectNo;
                                //System.IO.Directory.Move(s2, p1);
                                string s6 = s2 + "\\" + projectNo;
                                //System.IO.Directory.Move(s6, p1);
                                CopyFolder(s6, p1);

                                //删除avfiles

                            }
                        }
                    }
                    int j = 0;
                    foreach (XElement record in XFile.Elements())
                    {
                        j = j + 1;
                        string Dzwjlj = record.Attribute("Dzwjlj").Value;
                        //新增
                        string fileName = record.Attribute("Wjtm").Value;
                        string[] invalidStrings = { "<", ">", "|", "?", "/", ":", "*" };
                        foreach (string invalidString in invalidStrings) {
                            if (fileName.Contains(invalidString)) {
                                fileName = fileName.Replace(invalidString, "");
                            }
                        }
                        
                        //修改文件名
                        string str2 = str1;
                        if (Directory.Exists(str2))
                        {
                            var file3s = Directory.GetFiles(str2, "*.pdf");
                            foreach (var n in file3s)
                            {
                                string pdf = Dzwjlj;
                                int k = pdf.LastIndexOf("\\");
                                pdf = pdf.Substring(k + 1);

                                if (Path.GetFileName(n) == pdf)
                                {
                                    string str3 = str2 + "\\" + fileName + ".pdf";
                                
                                    if (System.IO.File.Exists(str3)) {
                                        int index = 2;
                                        while (System.IO.File.Exists(str3)) {
                                            str3 = str2 + "\\" + fileName + "-" + index.ToString() + ".pdf";
                                            index++;
                                        }
                                    }
                                    str2 = str2 + "\\" + pdf;
                                    System.IO.File.Move(str2, str3);
                                }
                            }
                        }

                    }
                }

                //将临时文件名修改回正确的名字
                if (Directory.Exists(newStr))
                {
                    //获取文件夹信息
                    DirectoryInfo directory1 = new DirectoryInfo(newStr);
                    //获取文件下的文件信息
                    DirectoryInfo[] file1s = directory1.GetDirectories();
                    foreach (var file in file1s)
                    {
                        string filename = file.Name.Split('_')[1];
                        string filePath = file.Parent.FullName + "//" + filename;
                        System.IO.Directory.Move(file.FullName, filePath);
                    }
                }
                //最后移动整个文件夹 新增
                if (Directory.Exists(pathWillReach))
                {
                    Directory.Delete(pathWillReach, true);
                }
                CopyFolder(newStr, pathWillReach);
                if (Directory.Exists(pathWillReach))
                {
                    Directory.Delete(path, true);
                }
            }

        }
        //void get_id()
        //{
        //    string path = "G:\\JunGongArchives1\\" + id;
        //    string paperProjectSeqNo1 = paperProjectSeqNo.PadLeft(5, '0');
        //    string path1 = "G:\\JunGongArchives1\\" + paperProjectSeqNo1;
        //    string path2 = "G:\\JunGongArchives1\\" + paper.projectNo.ToString();
        //    if (Directory.Exists(path2))
        //    {
        //        System.IO.Directory.Move(path2, path1);
        //    }
        //}
        /// <summary>
        /// 递归Delete
        /// </summary>

        /// <returns></returns>
        public static bool DeleteFolder(string floderPath)
        {
            try
            {
                if (Directory.Exists(floderPath))
                {
                    var r = Directory.GetFileSystemEntries(floderPath);
                    foreach (var inst in Directory.GetFileSystemEntries(floderPath))
                    {
                        if (System.IO.File.Exists(inst))
                        {
                            System.IO.File.Delete(inst); //直接删除其中的文件 
                        }
                        else
                        {
                            DeleteFolder(inst); //递归删除子文件夹 
                        }
                    }
                    //删除已空文件夹 
                    Directory.Delete(floderPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message, ex);
            }
        }

        //
        public static bool connectState(string path, string userName, string passWord)
        {
            bool Flag = false;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use " + path + " /User:" + userName + " " + passWord + " /PERSISTENT:YES";
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg))
                {
                    Flag = true;
                }
                else
                {
                    throw new Exception(errormsg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }

        protected void Page_Load()
        {

            bool status = false;

            //连接共享文件夹
            status = connectState(@"\\192.168.0.168\ftppath", "Administrator", "Aa123456");
            if (status)
            {
                DirectoryInfo theFolder = new DirectoryInfo(@"\\192.168.0.168\ftppath");

                //遍历共享文件夹，
                foreach (System.IO.FileInfo nextFile in theFolder.GetFiles())
                {
                    //
                    if (nextFile.Extension == ".sip")
                    {
                        
                        string p = "F:\\数据\\sip\\resource\\" + nextFile.Name;
                        if (System.IO.File.Exists(@p) == false)
                        {
                            System.IO.File.Copy(nextFile.FullName, @p, true);
                            try { nextFile.Delete(); }
                            catch { }
                        }
                    }
                    else
                    {
                    }
                }

            }
            else
            {
                //return Content("<script >alert('序号应是数字！');window.history.back();</script >");
            }
        }


        // POST: ProjectInfoes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken] //传入
        public ActionResult Create([Bind(Include = "projectName,projectID,projectNo,location,contractNo,newLocation,submitPerson,projectProfile,mobilephoneSubmitPerson,telphoneSubmitPerson,securityID,buildingArea,prevClassNo,retentionPeriodNo ,dateReceived ,recipient,collationRequirement,developmentOrganization,devolonpentOrgContacter,mobilephoneNoDevelopment,telphoneNoDevelopment,jianliUnit,jianliUnitContacter,telphoneNoJianliUnit,constructionOrganization,constructionOrgContacter,telphoneNoConstruction,disignOrganization,designOrgaContacter,telphoneNoDesignOrga,csyjPerson,csyjDate,memo,isFafangHegezheng,isLingquYijiaoshu,isCharge,isFinanceCharge,seqNo,coordinate")] vw_projectProfile vwprojectProfile, string action, string radiobutton, string prevClassNo, string csyj, string isYD)
        {
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", 1);
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", 1);
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

            };
            ViewBag.isYD = new SelectList(list, "Value", "Text", 1);
            var users = from ad in ab.AspNetUsers
                        where ad.DepartmentId == 2
                        select ad;
            ViewBag.recipient = new SelectList(users, "UserName", "UserName", "沙宏伟");
            var project = from a in db.PaperArchives
                          where a.projectNo == vwprojectProfile.projectNo
                          select a;
            if (action == "返回")
            {
                return RedirectToAction("Index", "ContractInfoes");
            }
            //if (project.Count() != 0)
            //{
            //    return Content("<script >alert('该工程序号已存在，请重新输入！');window.location.href='Create';</script >");
            //}

            if (action == "更新数据")
            {
                //1、获取shp点数据
                var shp = from a in db.shp
                              //where a.paper <= 14
                              where a.paper >= 18
                              select a;
                var shp1 = shp.ToArray();
                long? pa = shp1[0].paper;
                string cod = "";
                for (int i = 0; i < shp1.Count(); i++) {
                    shp shp2 = shp1[i];
                    if (pa != shp2.paper)
                    {
                        var pap = from a in db.PaperArchives
                                  where a.paperProjectSeqNo == pa
                                  select a;
                        var paperArchives = pap.First();
                        if (paperArchives.coordinate == "" || paperArchives.coordinate == null) {
                            paperArchives.coordinate = cod;
                        }
                        else
                        {
                            paperArchives.coordinate = paperArchives.coordinate + "," + cod;
                        }
                        
                        pa = shp2.paper;
                        cod = shp2.y + " " + shp2.x;
                        db.Entry(paperArchives).State = EntityState.Modified;
                    }
                    else {
                        if (cod == "")
                        {
                            cod = shp2.y + " " + shp2.x;
                        }
                        else
                        {
                            cod = cod + "," + shp2.y + " " + shp2.x;
                        }
                    }
                }
                var pap1 = from a in db.PaperArchives
                          where a.paperProjectSeqNo == pa
                          select a;
                var paperArchives1 = pap1.First();
                if (paperArchives1.coordinate == "" || paperArchives1.coordinate == null)
                {
                    paperArchives1.coordinate = cod;
                }
                else
                {
                    paperArchives1.coordinate = paperArchives1.coordinate + "," + cod;
                }
                db.Entry(paperArchives1).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (action == "提交")
            {

                PaperArchives paperArchive = new PaperArchives();
                ProjectInfo projectInfo = new ProjectInfo();
                if (vwprojectProfile.projectName == null || vwprojectProfile.projectName == "")
                {
                    return Content("<script >alert('工程名称不能为空，请核查！');window.history.back();</script >");
                }
                if (vwprojectProfile.prevClassNo == null || vwprojectProfile.prevClassNo == "")
                {
                    return Content("<script >alert('拟分类号不能为空，请核查！');window.history.back();</script >");
                }
                if (radiobutton == "" || radiobutton == null)
                {
                    return Content("<script >alert('请选择工程状态！');window.history.back();</script >");
                }

                string file = Request.Form["MyUploadile"];
                string filename = Request.Form["name1"];
                var files = Request.Files;
                //if (filename != "" && filename != ",")
                //{
                //    if (files.Count > 0)
                //    {
                //        var file1 = files[0];
                //        //string pa = "~/files/jungongWord/projectNo";
                //        //if (!Directory.Exists(pa))
                //        //{
                //        //    System.IO.Directory.CreateDirectory("~/files/jungongWord/projectNo");

                //        //} 
                //        var no = vwprojectProfile.projectNo;
                //        string pa1 = "~/files/jungongWord/" + no;
                //        //string pa = "E:\\2019.07.17\\Urban20181226\\urban_archive\\files\\jungongWord\\" + no;
                //        //string pa = "192.168.0.114\\files\\jungongWord" + no;
                //        string pa = AppDomain.CurrentDomain.BaseDirectory + "files\\jungongWord\\" + no;
                //        if (!Directory.Exists(pa)) {
                //            Directory.CreateDirectory(pa);
                //        }
                //        string strFileSavePath = Request.MapPath(pa1);//文件存储路径
                //        file1.SaveAs(strFileSavePath + "/" + filename);

                //    }
                //}

                if (filename != "" && filename != ",")
                {
                    if (files.Count > 0)
                    {
                        for (int i = 0; i < files.Count - 1; i++)//还包括前台ID = project
                        {
                            var file1 = files[i];
                            //string pa = "~/files/jungongWord/projectNo";
                            //if (!Directory.Exists(pa))
                            //{
                            //    System.IO.Directory.CreateDirectory("~/files/jungongWord/projectNo");

                            //} 
                            var no = vwprojectProfile.projectNo;
                            string pa1 = "~/files/jungongWord/" + no;
                            //string pa = "E:\\2019.07.17\\Urban20181226\\urban_archive\\files\\jungongWord\\" + no;
                            //string pa = "192.168.0.114\\files\\jungongWord" + no;
                            string pa = AppDomain.CurrentDomain.BaseDirectory + "files\\jungongWord\\" + no;
                            if (!Directory.Exists(pa))
                            {
                                Directory.CreateDirectory(pa);
                            }
                            string strFileSavePath = Request.MapPath(pa1);//文件存储路径
                            var pos = files[i].FileName.LastIndexOf("\\");
                            var str = files[i].FileName.Substring(pos + 1);
                            file1.SaveAs(strFileSavePath + "/" + str);
                        }

                    }
                }


                paperArchive.paperProjectSeqNo = 0;//paperProjectSeqNo还不能编号，先统一赋值0
                var proInfo = from a in db.ProjectInfo
                              where a.projectID == vwprojectProfile.projectID
                              select a;
                var papArch = from b in db.PaperArchives
                              where b.projectID == vwprojectProfile.projectID
                              select b;
                if(proInfo.Count()!=0){
                    db.ProjectInfo.Remove(proInfo.First());
                    db.PaperArchives.Remove(papArch.First());//如果之前有这个projectID，删除覆盖
                }
                //更新ProjectInfo表
                var n1 = vwprojectProfile.projectNo;
                string p1 = AppDomain.CurrentDomain.BaseDirectory + "files\\jungongWord\\" + n1;
                if (System.IO.Directory.Exists(p1))
                {
                    System.IO.DirectoryInfo Dirinfo = new System.IO.DirectoryInfo(p1);
                    String addr;
                    Array arrDir = Dirinfo.GetFiles();
                    if (arrDir.Length > 0)
                    {
                        filename = "";
                        foreach (System.IO.FileInfo f1 in arrDir)
                        {
                            string name = f1.Name;
                            filename = filename + name + ',';
                        }
                    }
                }

                projectInfo.storagePath = filename;
                projectInfo.projectID = vwprojectProfile.projectID;//工程信息表的projectID与工程目录表projectID为一对一关系
                projectInfo.contractNo = vwprojectProfile.contractNo;
                projectInfo.projectName = vwprojectProfile.projectName;
                if (vwprojectProfile.developmentOrganization == null || vwprojectProfile.developmentOrganization == "")
                {
                    projectInfo.developmentOrganization = "";
                }
                else
                {
                    projectInfo.developmentOrganization = vwprojectProfile.developmentOrganization.Trim();
                }
                if (vwprojectProfile.telphoneNoDevelopment == "" || vwprojectProfile.telphoneNoDevelopment == null)
                {
                    projectInfo.telphoneNoDevelopment = "";
                }
                else
                {
                    projectInfo.telphoneNoDevelopment = vwprojectProfile.telphoneNoDevelopment.Trim();
                }
                if (vwprojectProfile.mobilephoneNoDevelopment == "" || vwprojectProfile.mobilephoneNoDevelopment == null)
                {
                    projectInfo.mobilephoneNoDevelopment = "";
                }
                else
                {
                    projectInfo.mobilephoneNoDevelopment = vwprojectProfile.mobilephoneNoDevelopment.Trim();
                }
                if (vwprojectProfile.constructionOrganization == null || vwprojectProfile.constructionOrganization == "")
                {
                    projectInfo.constructionOrganization = "";
                }
                else
                {
                    projectInfo.constructionOrganization = vwprojectProfile.constructionOrganization.Trim();
                }
                if (vwprojectProfile.telphoneNoConstruction == "" || vwprojectProfile.telphoneNoConstruction == null)
                {
                    projectInfo.telphoneNoConstruction = "";
                }
                else
                {
                    projectInfo.telphoneNoConstruction = vwprojectProfile.telphoneNoConstruction.Trim();
                }
                //projectInfo.developmentOrganization = vwprojectProfile.developmentOrganization;
                //projectInfo.mobilephoneNoDevelopment = vwprojectProfile.mobilephoneNoDevelopment;
                //projectInfo.telphoneNoDevelopment = vwprojectProfile.telphoneNoDevelopment;
                //projectInfo.constructionOrganization = vwprojectProfile.constructionOrganization;//施工单位
                //projectInfo.telphoneNoConstruction = vwprojectProfile.telphoneNoConstruction;//施工单位技术人员
                //projectInfo.constructionOrgContacter = vwprojectProfile.constructionOrgContacter;//施工单位法人
                projectInfo.mobilephoneNoConstruction = "";//没用了
                projectInfo.securityID = vwprojectProfile.securityID;
                projectInfo.retentionPeriodNo = vwprojectProfile.retentionPeriodNo;

                if (isYD == "1")
                {
                    projectInfo.isYD = true;
                }
                else
                {
                    projectInfo.isYD = false;
                }
                //projectInfo.devolonpentOrgContacter = vwprojectProfile.devolonpentOrgContacter;
                //projectInfo.disignOrganization = vwprojectProfile.disignOrganization;
                //projectInfo.designOrgaContacter = vwprojectProfile.designOrgaContacter;
                //projectInfo.telphoneNoDesignOrga = vwprojectProfile.telphoneNoDesignOrga;
                //projectInfo.jianliUnit = vwprojectProfile.jianliUnit;
                //projectInfo.jianliUnitContacter = vwprojectProfile.jianliUnitContacter;
                //projectInfo.telphoneNoJianliUnit = vwprojectProfile.telphoneNoJianliUnit;
                if (vwprojectProfile.devolonpentOrgContacter == "" || vwprojectProfile.devolonpentOrgContacter == null)
                {
                    projectInfo.devolonpentOrgContacter = "";
                }
                else
                {
                    projectInfo.devolonpentOrgContacter = vwprojectProfile.devolonpentOrgContacter.Trim();
                }
                if (vwprojectProfile.constructionOrgContacter == null || vwprojectProfile.constructionOrgContacter == "")
                {
                    projectInfo.constructionOrgContacter = "";
                }
                else
                {
                    projectInfo.constructionOrgContacter = vwprojectProfile.constructionOrgContacter.Trim();
                }
                if (vwprojectProfile.disignOrganization == "" || vwprojectProfile.disignOrganization == null)
                {
                    projectInfo.disignOrganization = "";
                }
                else
                {
                    projectInfo.disignOrganization = vwprojectProfile.disignOrganization.Trim();
                }
                if (vwprojectProfile.designOrgaContacter == null || vwprojectProfile.designOrgaContacter == "")
                {
                    projectInfo.designOrgaContacter = "";
                }
                else
                {
                    projectInfo.designOrgaContacter = vwprojectProfile.designOrgaContacter.Trim();
                }
                if (vwprojectProfile.telphoneNoDesignOrga == null || vwprojectProfile.telphoneNoDesignOrga == "")
                {
                    projectInfo.telphoneNoDesignOrga = "";
                }
                else
                {
                    projectInfo.telphoneNoDesignOrga = vwprojectProfile.telphoneNoDesignOrga.Trim();
                }

                projectInfo.mobilephoneNoDesignOrga = "";
                if (vwprojectProfile.jianliUnitContacter == "" || vwprojectProfile.jianliUnitContacter == null)
                {
                    projectInfo.jianliUnitContacter = "";
                }
                else
                {
                    projectInfo.jianliUnitContacter = vwprojectProfile.jianliUnitContacter.Trim();
                }
                if (vwprojectProfile.jianliUnit == "" || vwprojectProfile.jianliUnit == null)
                {
                    projectInfo.jianliUnit = "";
                }
                else
                {
                    projectInfo.jianliUnit = vwprojectProfile.jianliUnit.Trim();
                }

                if (vwprojectProfile.telphoneNoJianliUnit == null || vwprojectProfile.telphoneNoJianliUnit == "")
                {
                    projectInfo.telphoneNoJianliUnit = "";
                }
                else
                {
                    projectInfo.telphoneNoJianliUnit = vwprojectProfile.telphoneNoJianliUnit.Trim();
                }
                projectInfo.mobilephoneNoJianliUnit = "";
                projectInfo.memo = vwprojectProfile.memo;
                projectInfo.location = vwprojectProfile.location;
                projectInfo.structureTypeID = "1";
                projectInfo.status = "3";
                //更新paperArchive表 
                int max_ID = Convert.ToInt32(db.PaperArchives.Max(d => d.ID));//设置一个默认值，用户也可修改，保证8位且不重复就行
                paperArchive.ID = max_ID + 1;
                paperArchive.character1cm = 0;
                paperArchive.character2cm = 0;
                paperArchive.character3cm = 0;
                paperArchive.character4cm = 0;
                paperArchive.character5cm = 0;
                paperArchive.drawing1cm = 0;
                paperArchive.drawing2cm = 0;
                paperArchive.drawing3cm = 0;
                paperArchive.drawing4cm = 0;
                paperArchive.drawing5cm = 0;
                paperArchive.characterVolumeCount = 0;
                paperArchive.paperProjectSeqNo = 0;
                paperArchive.drawingVolumeCount = 0;
                paperArchive.projectID = vwprojectProfile.projectID;
                paperArchive.projectNo = vwprojectProfile.projectNo;
                paperArchive.collationRequirement = vwprojectProfile.collationRequirement;
                paperArchive.recipient = vwprojectProfile.recipient;
                paperArchive.coordinate = vwprojectProfile.coordinate;
                if (vwprojectProfile.telphoneSubmitPerson == null || vwprojectProfile.telphoneSubmitPerson == "")
                {
                    paperArchive.telphoneSubmitPerson = "";
                }
                else
                {
                    paperArchive.telphoneSubmitPerson = vwprojectProfile.telphoneSubmitPerson.Trim();
                }
                if (vwprojectProfile.mobilephoneSubmitPerson == "" || vwprojectProfile.mobilephoneSubmitPerson == null)
                {
                    paperArchive.mobilephoneSubmitPerson = "";
                }
                else
                {
                    paperArchive.mobilephoneSubmitPerson = vwprojectProfile.mobilephoneSubmitPerson.Trim();
                }

                paperArchive.submitPerson = vwprojectProfile.submitPerson;
                //paperArchive.mobilephoneSubmitPerson = vwprojectProfile.mobilephoneSubmitPerson;
                //paperArchive.telphoneSubmitPerson = vwprojectProfile.telphoneSubmitPerson;
                paperArchive.projectProfile = vwprojectProfile.projectProfile;
                paperArchive.csyjPerson = vwprojectProfile.csyjPerson;
                if (vwprojectProfile.csyjDate.ToString() != "")
                {
                    paperArchive.csyjDate = vwprojectProfile.csyjDate;
                }
                else
                {
                    paperArchive.csyjDate = DateTime.Now.Date;
                }
                if (vwprojectProfile.dateReceived.ToString() != "")
                {
                    paperArchive.dateReceived = vwprojectProfile.dateReceived;
                }
                else
                {
                    paperArchive.dateReceived = DateTime.Now.Date;
                }
                paperArchive.csyj = csyj;
                paperArchive.buildingArea = vwprojectProfile.buildingArea;
                paperArchive.prevClassNo = prevClassNo;
                paperArchive.bianhaoTime = DateTime.Now.Date;
                paperArchive.fzryj = "经审核，该工程初审意见基本属实，整理费用已收。";
                paperArchive.fzryjDate = DateTime.Now.Date;
                paperArchive.zgyj = "同意";
                paperArchive.zgyjDate = DateTime.Now.Date;
                paperArchive.structureTypeID = "1";
                paperArchive.paperProjectSeqNo = 0;

                if (vwprojectProfile.isLingquYijiaoshu == null)
                {
                    projectInfo.isLingquYijiaoshu = false;
                }
                if (vwprojectProfile.isFafangHegezheng == null)
                {
                    projectInfo.isFafangHegezheng = false;
                }
                if (vwprojectProfile.isCharge == null)
                {
                    projectInfo.isCharge = false;
                }
                if (vwprojectProfile.isFinanceCharge == null)
                {
                    projectInfo.isFinanceCharge = false;
                }


                if (ModelState.IsValid)
                {

                    db.PaperArchives.Add(paperArchive);
                    db.ProjectInfo.Add(projectInfo);
                    db.SaveChanges();


                    return Content("<script >alert('保存成功！');window.history.back();</script >");
                }

            }

            if (action == "文件上传")
            {
                return RedirectToAction("Fileuploading", "ProjectInfoes");
            }
            if (action == "录入新工程")
            {


                return RedirectToAction("Create", "ProjectInfoes", new { id2 = 1 });//设置id2参数是为了识别从何处进入create

            }
            //吕鸣
            if (action == "导入Excel")
            {
                HttpPostedFileBase file7 = Request.Files["project"];//获取上传的文件

                string FileName;
                string savePath;
                if (file7 == null || file7.ContentLength <= 0)
                {
                    ViewBag.error = "文件不能为空";
                    return View();
                }
                else
                {
                    string filename = System.IO.Path.GetFileName(file7.FileName);
                    int filesize = file7.ContentLength;//获取上传文件的大小单位为字节byte
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
                    file7.SaveAs(savePath);
                }
                string result = string.Empty;
                string strConn;
                strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + savePath + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";
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
                //循环  2018/9/6  吕鸣
                int i = 0;

                for (i = 0; i < table.Rows.Count; i++)
                {
                    string ProjectName = table.Rows[i][0].ToString().Trim();//工程项目题名
                    //int seq;
                    if (ProjectName == "")
                    {
                        ProjectName = "";
                    }
                    //else
                    //{
                    //    try
                    //    {
                    //        seq = Convert.ToInt32(SeqNo);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        return Content("<script >alert('序号应是数字！');window.history.back();</script >");
                    //    }

                    //}
                    string ProjectLocation = table.Rows[i][1].ToString().Trim();//工程地点
                    if (ProjectLocation == "")
                    {
                        ProjectLocation = "";
                    }
                    //else if (Type == "文字" || Type == "图纸" || Type == "文字及图纸")
                    //{
                    //}
                    //else
                    //{
                    //    return Content("<script >alert('类型输入有错！（文字/图纸/文字及图纸）');window.history.back();</script >");
                    //}
                    string NewProjectLocation = table.Rows[i][2].ToString().Trim();//最新工程地点
                    //int member;
                    if (NewProjectLocation == "")
                    {
                        NewProjectLocation = "";
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
                    string SendPeople = table.Rows[i][3].ToString().Trim();//报送人
                    if (SendPeople == "")
                    {
                        SendPeople = "";
                    }
                    string Telephone = table.Rows[i][4].ToString().Trim();//手机
                    if (Telephone == "")
                    {
                        Telephone = "";
                    }
                    string FixTelephone = table.Rows[i][5].ToString().Trim();//固定电话
                    if (FixTelephone == "")
                    {
                        FixTelephone = "";
                    }
                    string MJ = table.Rows[i][6].ToString().Trim();//档案密级

                    if (MJ != "秘密" && MJ != "机密" && MJ != "绝密" && MJ != "一般")
                    {
                        return Content("<script >alert('密级的输入格式不正确！（秘密/机密/绝密/一般）');window.history.back();</script >");
                    }
                    //工程序号
                    string projectNo = vwprojectProfile.projectNo.ToString();//工程序号



                    string buildingArea1 = table.Rows[i][7].ToString().Trim(); //dr["建筑面积"].ToString().Trim();
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


                    string Nfenlei = table.Rows[i][8].ToString().Trim();//拟分类号
                    if (Nfenlei == "")
                    {
                        Nfenlei = "";
                    }
                    string retentionPeriodName1 = table.Rows[i][9].ToString().Trim(); //dr["保存期限"].ToString().Trim();
                    if (retentionPeriodName1 != "长期" && retentionPeriodName1 != "永久" && retentionPeriodName1 != "短期")
                    {
                        return Content("<script >alert('保存期限的输入格式不正确！（长期/永久/短期）');window.history.back();</script >");
                    }
                    string jsDate1 = table.Rows[i][10].ToString().Trim(); //dr["接收日期"].ToString().Trim();
                    DateTime? jsDate2;
                    if (jsDate1 == "")
                    {
                        jsDate2 = null;
                    }
                    else
                    {
                        try
                        {
                            jsDate2 = DateTime.Parse(jsDate1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('接收日期格式不正确！（2018/10/1）');window.history.back();</script >");
                        }
                    }
                    string jspeople = table.Rows[i][11].ToString().Trim(); //dr["接收人"].ToString().Trim();
                    if (jspeople == "")
                    {
                        jspeople = "";
                    }
                    string yd = table.Rows[i][12].ToString().Trim(); //dr["异地存放"].ToString().Trim();
                    if (yd != "是" && yd != "否")
                    {
                        return Content("<script >alert('是否异地不正确！');window.history.back();</script >");
                    }
                    int yd1;
                    if (yd == "是")
                    {
                        yd1 = 1;
                    }
                    else
                    {
                        yd1 = 0;
                    }
                    string xmgaikuang = table.Rows[i][13].ToString().Trim(); //dr["项目概况"].ToString().Trim();
                    if (xmgaikuang == "")
                    {
                        xmgaikuang = "";
                    }
                    string zlyaoqiu = table.Rows[i][14].ToString().Trim(); //dr["整理要求"].ToString().Trim();
                    if (zlyaoqiu == "")
                    {
                        zlyaoqiu = "";
                    }
                    string coordinate = table.Rows[i][15].ToString().Trim(); //dr["测绘坐标"].ToString().Trim();
                    if (coordinate == "")
                    {
                        coordinate = "";
                    }
                    string developmentOrganization1 = table.Rows[i][16].ToString().Trim(); //dr["建设单位"].ToString().Trim();
                    if (developmentOrganization1 == "")
                    {
                        developmentOrganization1 = "";
                    }
                    string lxpeople = table.Rows[i][17].ToString().Trim(); //dr["联系人"].ToString().Trim();
                    if (lxpeople == "")
                    {
                        lxpeople = "";
                    }
                    string lxTelephone = table.Rows[i][18].ToString().Trim();//联系人移动电话
                    if (lxTelephone == "")
                    {
                        lxTelephone = "";
                    }
                    string lxFixTelephone = table.Rows[i][19].ToString().Trim();//联系人固定电话
                    if (lxFixTelephone == "")
                    {
                        lxFixTelephone = "";
                    }
                    string jlOrganization1 = table.Rows[i][20].ToString().Trim(); //dr["监理单位"].ToString().Trim();
                    if (jlOrganization1 == "")
                    {
                        jlOrganization1 = "";
                    }
                    string jlfzpeople = table.Rows[i][21].ToString().Trim(); //dr["监理负责人"].ToString().Trim();
                    if (jlfzpeople == "")
                    {
                        jlfzpeople = "";
                    }
                    string telphoneNoJianliUnit = table.Rows[i][22].ToString().Trim(); //dr["监理者"].ToString().Trim();
                    if (telphoneNoJianliUnit == "")
                    {
                        telphoneNoJianliUnit = "";
                    }

                    string constructionOrganization1 = table.Rows[i][23].ToString().Trim(); //dr["施工单位"].ToString().Trim();
                    if (constructionOrganization1 == "")
                    {
                        constructionOrganization1 = "";
                    }
                    string fapeople = table.Rows[i][24].ToString().Trim(); //dr["法人"].ToString().Trim();
                    if (fapeople == "")
                    {
                        fapeople = "";
                    }
                    string jishupeople = table.Rows[i][25].ToString().Trim(); //dr["技术人员"].ToString().Trim();
                    if (jishupeople == "")
                    {
                        jishupeople = "";
                    }


                    string disignOrganization1 = table.Rows[i][26].ToString().Trim(); //dr["设计单位"].ToString().Trim();
                    if (disignOrganization1 == "")
                    {
                        disignOrganization1 = "";
                    }
                    string sjfzpeople = table.Rows[i][27].ToString().Trim(); //dr["负责人"].ToString().Trim();
                    if (sjfzpeople == "")
                    {
                        sjfzpeople = "";
                    }
                    string telphoneNoDesignOrga = table.Rows[i][28].ToString().Trim(); //dr["设计人"].ToString().Trim();
                    if (telphoneNoDesignOrga == "")
                    {
                        telphoneNoDesignOrga = "";
                    }

                    //int a = int.Parse(id) + i;
                    //var num = from ad in db.FileInfo
                    //          where ad.id == a
                    //          select ad;/*1*/
                    string retentionPeriodNo1 = db.RetentionPeriod.Where(ad => ad.retentionPeriodName == retentionPeriodName1).First().retentionPeriodNo;
                    string MJ1 = db.SecurityClassification.Where(ad => ad.securityName == MJ).First().securityID;
                    string strSql1 = string.Format("insert into UrbanCon.dbo.PaperArchives(submitPerson,telphoneSubmitPerson,mobilephoneSubmitPerson,projectNo,buildingArea,prevClassNo,dateReceived,recipient,projectProfile,collationRequirement,coordinate,paperProjectSeqNo,projectID) values('" + SendPeople + "','" + FixTelephone + "','" + Telephone + "','" + projectNo + "'," + buildingArea1 + ",'" + Nfenlei + "','" + jsDate1 + "','" + jspeople + "','" + xmgaikuang + "','" + zlyaoqiu + "','" + coordinate + "',0," + vwprojectProfile.projectID + ")");
                    //string strSql1 = string.Format("update UrbanCon.dbo.PaperArchives set submitPerson = '" + SendPeople + "',telphoneSubmitPerson = '" + FixTelephone + "',mobilephoneSubmitPerson='" + Telephone + "',projectNo='"+ projectNo + "',buildingArea="+ buildingArea1 + ",prevClassNo='"+ Nfenlei + "',retentionPeriodNo=(select retentionPeriodNo from UrbanCon.dbo.RetentionPeriod where retentionPeriodName='" + retentionPeriodName1 + "'),dateReceived='"+ jsDate1 + "',recipient='"+ jspeople + "',projectProfile='"+ xmgaikuang + "',collationRequirement='" + zlyaoqiu + "'coordinate='"+ coordinate + "'");
                    //string strSql2 = string.Format("update UrbanCon.dbo.PaperArchives set licenseNo='" + licenseNo1 + "',structureTypeID=(select structureTypeID from UrbanCon.dbo.StructureType where structureTypeName='" + structureTypeName1 + "'),buildingArea=" + buildingArea2 + ",firstResponsible='" + firstResponsible1 + "',responsibleOther='" + responsibleOther1 + "',transferUnit='" + transferUnit1 + "',textMaterial=" + textMaterial2 + ",drawing=" + drawing2 + ",photoCount=" + photoCount2 + ",jgDate='" + jgDate2 + "',height='" + height2 + "',changeLog='" + changeLog1 + "',remarks='" + remarks1 + "',overground='" + overground1 + "',underground='" + underground1 + "',archivesCount='" + archivesCount1 + "' where paperProjectSeqNo = " + paperProjectSeqNo2);
                    //string strSql2 = string.Format("update UrbanCon.dbo.ProjectInfo set projectName='" + ProjectName + "',location='"+ ProjectLocation + "',newLocation='"+ NewProjectLocation + "',securityID=(select securityID from UrbanCon.dbo.SecurityClassification where securityName='" + MJ + "'),isYD=" + yd1 + ",developmentOrganization='" + developmentOrganization1 + "',devolonpentOrgContacter='" + lxpeople + "',mobilephoneNoDevelopment='" + lxTelephone + "',telphoneNoDevelopment='" + lxFixTelephone + "',jianliUnit='"+ jlOrganization1 + "',jianliUnitContacter='"+ jlfzpeople + "',telphoneNoJianliUnit= '"+ telphoneNoJianliUnit + "',constructionOrganization='" + constructionOrganization1 + "',constructionOrgContacter='"+ fapeople + "',telphoneNoConstruction='"+ jishupeople + "',disignOrganization='" + disignOrganization1 + "',designOrgaContacter='" + sjfzpeople + "',telphoneNoDesignOrga='"+ telphoneNoDesignOrga + "',where paperProjectSeqNo = " + paperProjectSeqNo2);
                    string strSql2 = string.Format("insert into UrbanCon.dbo.ProjectInfo(projectName,location,newLocation,retentionPeriodNo,securityID,isYD,developmentOrganization,devolonpentOrgContacter,mobilephoneNoDevelopment,telphoneNoDevelopment,jianliUnit,jianliUnitContacter,constructionOrganization,constructionOrgContacter,telphoneNoConstruction,disignOrganization,designOrgaContacter,telphoneNoDesignOrga,projectID,contractNo,status,mappipei) values('" + ProjectName + "','" + ProjectLocation + "','" + NewProjectLocation + "','" + retentionPeriodNo1 + "','" + MJ1 + "'," + yd1 + ",'" + developmentOrganization1 + "','" + lxpeople + "','" + lxTelephone + "','" + lxFixTelephone + "','" + jlOrganization1 + "','" + telphoneNoJianliUnit + "','" + constructionOrganization1 + "','" + fapeople + "','" + jishupeople + "','" + disignOrganization1 + "','" + sjfzpeople + "','" + telphoneNoDesignOrga + "'," + vwprojectProfile.projectID + ",'" + vwprojectProfile.contractNo + "','3',''" + ")");
                    //var pap = from a in db.ArchivesDetail
                    //          where a.paperProjectSeqNo == paperProjectSeqNo2
                    //          select a;
                    //string strSql3;
                    //if (pap.Count() == 0)
                    //{
                    //    strSql3 = string.Format("insert into UrbanCon.dbo.ArchivesDetail(volNo, registrationNo, archivesNo, shizhengNo, paperProjectSeqNo, paijiaNo, licenseNo, archivesTitle, firstResponsible,responsibleOther,developmentUnit,transferUnit,designUnit,constructionUnit,textMaterial,drawing,photoCount,archiveThickness,bianzhiTime,jgDate,remarks,fazhaoTime,kaigongTime,jungongTime,indexer,checker,typist,indexDate,checkDate,typerDate) values(" + volNo2 + ",'" + registrationNo1 + "','" + archivesNo1 + "','" + shizhengNo1 + "'," + paperProjectSeqNo2 + ",'" + paijiaNo1 + "','" + licenseNo1 + "','" + archivesTitle1 + "','" + firstResponsible1 + "','" + responsibleOther1 + "','" + developmentOrganization1 + "','" + transferUnit1 + "','" + disignOrganization1 + "','" + constructionOrganization1 + "'," + textMaterial2 + "," + drawing2 + "," + photoCount2 + "," + archiveThickness2 + ",'" + bianzhiTime1 + "','" + jgDate2 + "','" + remarks1 + "','" + fazhaoTime1 + "','" + kaigongTime1 + "','" + jungongTime1 + "','" + indexer1 + "','" + checker1 + "','" + Typist1 + "','" + indexeDate2 + "','" + checkDate2 + "','" + TyperDate2 + "')");
                    //}
                    //else
                    //{
                    //    strSql3 = string.Format("update UrbanCon.dbo.ArchivesDetail set volNo=" + volNo2 + ",registrationNo='" + registrationNo1 + "',archivesNo='" + archivesNo1 + "',shizhengNo='" + shizhengNo1 + "',paijiaNo='" + paijiaNo1 + "',licenseNo ='" + licenseNo1 + "',archivesTitle='" + archivesTitle1 + "',firstResponsible='" + firstResponsible1 + "',responsibleOther='" + responsibleOther1 + "',developmentUnit='" + developmentOrganization1 + "',transferUnit='" + transferUnit1 + "',designUnit='" + disignOrganization1 + "',constructionUnit='" + constructionOrganization1 + "',textMaterial=" + textMaterial2 + ",drawing=" + drawing2 + ",photoCount=" + photoCount2 + ",archiveThickness=" + archiveThickness2 + ",bianzhiTime='" + bianzhiTime1 + "',jgDate='" + jgDate2 + "',remarks='" + remarks1 + "',fazhaoTime='" + fazhaoTime1 + "',kaigongTime='" + kaigongTime1 + "',jungongTime='" + jungongTime1 + "',indexer='" + indexer1 + "',checker='" + checker1 + "',typist='" + Typist1 + "',indexDate='" + indexeDate2 + "',checkDate='" + checkDate2 + "',typerDate='" + TyperDate2 + "' where registrationNo = '" + registrationNo1 + "'");
                    //}
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
                //return Content("<script >alert('导入成功！');window.history.back();</script >");
                return RedirectToAction("Create", new { id = vwprojectProfile.contractNo, txtCurProNo = vwprojectProfile.projectNo, id2 = 0 });
            }

            if (action == "导入Xml")
            {
                //string fileName, string userName, SecureString password, string domain
                //System.Diagnostics.Process.Start("cmd.exe", @"/k Net Use * \\222.195.148.165\excel asdfghjkl123 /user:6");
                //System.Diagnostics.Process.Start("Z:");
                ViewBag.id = vwprojectProfile.projectNo;
                //Page_Load();

                //read_sip();


                //管线
                //string wenjian = "F:\\市馆\\Urban20181226\\urban_archive\\sip\\最新数据\\管线\\管线在线项目－全接收\\房建在线项目－全接收\\info.xml";
                //string path = "F:\\市馆\\Urban20181226\\urban_archive\\sip\\最新数据\\管线\\管线在线项目－全接收\\房建在线项目－全接收";

                //竣工
                //string wenjian = "F:\\市馆\\Urban20181226\\urban_archive\\sip\\最新数据\\房建\\房建在线项目－全接收\\info.xml";
                //string path = "F:\\市馆\\Urban20181226\\urban_archive\\sip\\最新数据\\房建\\房建在线项目－全接收";
                string wenjian = "F:\\数据\\sip\\data\\青岛软件园市南园核心园中心区停车楼\\青岛软件园市南园核心园中心区停车楼\\info.xml";
                string path = "F:\\数据\\sip\\data\\青岛软件园市南园核心园中心区停车楼\\青岛软件园市南园核心园中心区停车楼";
                Read_Xml(wenjian, path);
                //if (isUpdate)
                //{
                //    string seqNo = paperProjectSeqNo.ToString();
                //    strPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + "\\" + seqNo;
                //}
                //movePathWithNEWName(wenjian, path, "","44369","20200001");
                //string newStr = "F:\\数据\\sip\\data\\青岛软件园市南园核心园中心区停车楼\\青岛软件园市南园核心园中心区停车楼\\20200000";
                //string pathWillReach = "G:\\JunGongArchives1\\20200000";
                //CopyFolder(newStr, pathWillReach);
                //Directory.Delete(newStr, true);
            }


            return View(vwprojectProfile);


        }
        public string GetCurpro(string year)
        {
            string txtCurMaxProNo = "";
            if (year != "")
            {
                var curproNo = from a in db.PaperArchives
                               where a.projectNo.ToString().Substring(0,4).ToString().Contains(year)
                               orderby a.paperProjectSeqNo
                               select a;
                if (curproNo.Count() != 0)
                {
                    int maxProjNo = 0;
                    foreach (var dr in curproNo)
                    {
                        string str = dr.projectNo.ToString().Trim();
                        if (str != null)
                        {
                            if (Int32.Parse(str) > maxProjNo)
                                maxProjNo = Int32.Parse(str);
                        }
                    }
                    txtCurMaxProNo = maxProjNo.ToString();


                }
                else
                {
                    int len = year.Length;
                    if (len == 8)
                        txtCurMaxProNo = year;
                    else
                    {
                        for (int i = 0; i < 7 - len; i++)
                        {
                            year += "0";
                        }
                        year += "1";
                        txtCurMaxProNo = year;
                    }

                }

            }
            return txtCurMaxProNo;
        }
        // GET: ProjectInfoes/Edit/5
        public ActionResult Edit( long? id)
        {
            var n = from a in db.PaperArchives
                    where a.projectID == id
                    select a;
            ViewBag.id = n.First().projectNo;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = ab.AspNetUsers.Find(UserID);
            if (user.UserName!="业务科")
            {
                return Content("<script >alert('当前用户没有修改权限，不能修改！');window.history.back();</script >");
            }
            var vwproject = from a in db.vw_projectProfile
                            where a.projectID ==id
                            select a;
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", vwproject.First().retentionPeriodNo);
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", vwproject.First().securityID);
            var users = from ad in ab.AspNetUsers
                        where ad.DepartmentName == User.Identity.Name
                        select ad;
            ViewBag.recipient = new SelectList(users, "UserName", "UserName", vwproject.First().recipient);
           
            ViewBag.status = new SelectList(db.ArchivesStatus, "status", "statusName", vwproject.First().status);
            ViewBag.filename = vwproject.First().storagePath;
            //设置是否异地存储
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},
               
            };
            ViewBag.isYD = new SelectList(list, "Value", "Text");
            if (vwproject.First().isYD==true)
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text",1);
            }
            else
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text",0 );
            }
            if (vwproject.Count()==0)
            {
                return HttpNotFound();
            }
          
            return View(vwproject.First());
        }

        // POST: ProjectInfoes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string projectName, string location, string newLocation, string projectID, string submitPerson, string mobilephoneSubmitPerson, string telphoneSubmitPerson, string paperProjectSeqNo, string projectNo, string buildingArea, string securityID, string retentionPeriodNo,string isYD, string status, string developmentOrganization, string devolonpentOrgContacter, string telphoneNoDevelopment, string mobilephoneNoDevelopment, string jianliUnit, string jianliUnitContacter, string telphoneNoJianliUnit, string constructionOrganization, string constructionOrgContacter, string telphoneNoConstruction, string disignOrganization, string designOrgaContacter, string telphoneNoDesignOrga, string dateReceived,string recipient,string projectProfile,string collationRequirement,string csyj,string csyjPerson, string csyjDate, string coordinate, string memo,string action)
        {
            
            long proID = 0;
            if (projectID!=""&& projectID!=null)
            {
                proID = int.Parse(projectID.Trim());
            }
            var projects = from ac in db.ProjectInfo
                           where ac.projectID ==proID
                           select ac;
            ProjectInfo project = projects.First();
            var paperArchives = from ad in db.PaperArchives
                                where ad.projectID == proID
                                select ad;
            PaperArchives paperArchive  = paperArchives.First();
            if (action == "修改")
            {
                string file = Request.Form["MyUploadile"];
                string filename = Request.Form["name1"];
                //if (filename != "")
                //{

                //    if (filename != project.storagePath)
                //    {
                //        var files = Request.Files;
                //        if (files.Count > 0)
                //        {
                //            //var file1 = files[0];
                //            //var no = paperArchive.projectNo;
                //            //string pa1 = "~/files/jungongWord/" + no;
                //            //string pa = AppDomain.CurrentDomain.BaseDirectory + "files\\jungongWord\\" + no;
                //            //if (!Directory.Exists(pa))
                //            //{
                //            //    Directory.CreateDirectory(pa);
                //            //}
                //            //string strFileSavePath = Request.MapPath(pa1);//文件存储路径
                //            //file1.SaveAs(strFileSavePath + "/" + filename);

                //            var file1 = files[0];
                //            var no = paperArchive.projectNo;
                //            string pa1 = "~/files/jungongWord/" + no;
                //            string pa = AppDomain.CurrentDomain.BaseDirectory + "files\\jungongWord\\" + no;
                //            if (!Directory.Exists(pa))
                //            {
                //                Directory.CreateDirectory(pa);
                //            }
                //            string strFileSavePath = Request.MapPath(pa1);//文件存储路径
                //            file1.SaveAs(strFileSavePath + "/" + filename);
                //        }
                //    }
                //}

                if (filename != "")
                {

                    if (filename != project.storagePath)
                    {
                        var files = Request.Files;
                        if (files.Count > 0)
                        {
                            //var file1 = files[0];
                            //var no = paperArchive.projectNo;
                            //string pa1 = "~/files/jungongWord/" + no;
                            //string pa = AppDomain.CurrentDomain.BaseDirectory + "files\\jungongWord\\" + no;
                            //if (!Directory.Exists(pa))
                            //{
                            //    Directory.CreateDirectory(pa);
                            //}
                            //string strFileSavePath = Request.MapPath(pa1);//文件存储路径
                            //file1.SaveAs(strFileSavePath + "/" + filename);
                            for (int i = 0; i < files.Count; i++)
                            {
                                var file1 = files[i];
                                var no = paperArchive.projectNo;
                                string pa1 = "~/files/jungongWord/" + no;
                                string pa = AppDomain.CurrentDomain.BaseDirectory + "files\\jungongWord\\" + no;
                                if (!Directory.Exists(pa))
                                {
                                    Directory.CreateDirectory(pa);
                                }
                                string strFileSavePath = Request.MapPath(pa1);//文件存储路径
                                var pos = files[i].FileName.LastIndexOf("\\");
                                var str = files[i].FileName.Substring(pos + 1);
                                file1.SaveAs(strFileSavePath + "/" + str);
                            }
                        }
                    }
                }


                //更新ProjectInfo表
                project.projectName = projectName;
                project.location = location;
                project.devolonpentOrgContacter =devolonpentOrgContacter;
                project.telphoneNoDevelopment = telphoneNoDevelopment;
                project.mobilephoneNoDevelopment = mobilephoneNoDevelopment;
                project.securityID =securityID;
                project.retentionPeriodNo = retentionPeriodNo;
                if(isYD=="1")
                {
                    project.isYD = true;
                }
               else
                {
                    project.isYD = false;
                }
                project.status = status;
                project.developmentOrganization = developmentOrganization;
                project.newLocation = newLocation;
                project.jianliUnit = jianliUnit;
                project.jianliUnitContacter = jianliUnitContacter;
                project.telphoneNoJianliUnit = telphoneNoJianliUnit;
                project.constructionOrganization = constructionOrganization;
                project.constructionOrgContacter = constructionOrgContacter;
                project.telphoneNoConstruction = telphoneNoConstruction;
                project.disignOrganization = disignOrganization;
                project.designOrgaContacter =designOrgaContacter;
                project.telphoneNoDesignOrga = telphoneNoDesignOrga;
                project.memo = memo;

                var n1 = paperArchive.projectNo;
                string p1 = AppDomain.CurrentDomain.BaseDirectory + "files\\jungongWord\\" + n1;
                if (System.IO.Directory.Exists(p1))
                {
                    System.IO.DirectoryInfo Dirinfo = new System.IO.DirectoryInfo(p1);
                    String addr;
                    Array arrDir = Dirinfo.GetFiles();
                    if (arrDir.Length > 0)
                    {
                        filename = "";
                        foreach (System.IO.FileInfo f1 in arrDir)
                        {
                            string name = f1.Name;
                            filename = filename + name + ',';
                        }
                    }
                }
                project.storagePath = filename;
                //更新PaperArchive表
                if(paperProjectSeqNo!=null& paperProjectSeqNo!="")
                {
                    long paperNo = long.Parse(paperProjectSeqNo.Trim());
                     paperArchive.paperProjectSeqNo = paperNo;
                }
                if(projectNo!= null&&projectNo!="")
                {
                    long proNo = Convert.ToInt32(projectNo.Trim());
                    paperArchive.projectNo = proNo;
                }
                if(buildingArea=="0"|| buildingArea==null)
                {
                    buildingArea = "0";
                }
                if (buildingArea == "" || buildingArea == null) {
                    paperArchive.buildingArea = 0;
                }
                else
                {
                    paperArchive.buildingArea = double.Parse(buildingArea.Trim()); 
                }           
                paperArchive.submitPerson = submitPerson;
                paperArchive.coordinate = coordinate;
                paperArchive.recipient = recipient;
                paperArchive.projectProfile = projectProfile;
                paperArchive.collationRequirement = collationRequirement;
                paperArchive.csyj = csyj;
                paperArchive.csyjDate = DateTime.Parse(csyjDate.Trim());
                paperArchive.csyjPerson = csyjPerson;
                paperArchive.mobilephoneSubmitPerson =mobilephoneSubmitPerson;
                paperArchive.telphoneSubmitPerson =telphoneSubmitPerson;
                if (ModelState.IsValid)
                {
                    //if (paperArchives.First().paperProjectSeqNo!=0)
                    //{
                    //    return Content("<script >alert('该工程已编号，不能修改！');window.history.back();</script >");
                    //}
                    //20171203 根据业务科要求，可以对已经编号的工程信息进行修改，但只是在业务科的账户下
                    db.Entry(project).State = EntityState.Modified;
                    db.Entry(paperArchive).State = EntityState.Modified;
                   
                     db.SaveChanges();
                     return Content("<script >alert('修改成功！');window.location.href='/ProjectInfoes/ManagementPrint';</script >");
                }
            }
            if(action=="删除")
            {
                //if (paperArchives.First().paperProjectSeqNo != 0)
                //{
                //    return Content("<script >alert('该工程已编号，不能修改！');window.history.back();</script >");
                //}
                //20171203 根据业务科要求，可以对已经编号的工程信息进行修改，但只是在业务科的账户下
                var c = from g in db.ProjectInfo
                        where g.projectID == proID
                        select g;
                ProjectInfo projectInfo = c.First();
                var d = from f in db.PaperArchives
                        where f.projectID == proID
                        select f;
                PaperArchives paperArchive1 = d.First();

                var n1 = paperArchive1.projectNo;
                string no = AppDomain.CurrentDomain.BaseDirectory + "files\\jungongWord\\" + n1;
                if (System.IO.Directory.Exists(no))
                {
                    //删除文件夹
                    Directory.Delete(no, true);
                }

                db.ProjectInfo.Remove(project);
                db.PaperArchives.Remove(paperArchive);
                db.SaveChanges();
                return Content("<script >alert('已成功删除！');window.location.href='/ProjectInfoes/ManagementPrint';</script >");
            }
            if(action=="返回")
            {
                return RedirectToAction("ManagementPrint", "ProjectInfoes");
            }
           
            return View();
        }

        // GET: ProjectInfoes/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var test = from ad in db.vw_projectProfile
                       where (ad.projectID == id)
                       select ad;
            vw_projectProfile projectProfile = test.First();
            if (projectProfile == null)
            {
                return HttpNotFound();
            }
            return View(projectProfile);

        }

        // POST: ProjectInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ProjectInfo projectInfo = db.ProjectInfo.Find(id);
            var paperArchives = from ad in db.PaperArchives
                                where (ad.projectID == projectInfo.projectID)
                                select ad;
            PaperArchives paperArchive = paperArchives.First();
            db.PaperArchives.Remove(paperArchive);
            db.ProjectInfo.Remove(projectInfo);
            db.SaveChanges();
            
            return RedirectToAction("Index");
        }
      


     

        public ActionResult Fileuploading()
        {

            return View();
        }
        public ActionResult ManagementPrint(string SelectedID, string SearchString, string action)
        {
            ViewData["pagename"] = "ProjectInfoes-ManagementPrint";
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "责任书编号", Value = "0"},
                new SelectListItem { Text = "工程名称", Value = "1"},
                new SelectListItem { Text = "建设单位", Value = "2" },
                new SelectListItem { Text = "工程地点", Value = "3" },
                new SelectListItem { Text = "工程序号", Value = "4" },
                new SelectListItem { Text = "施工单位", Value = "5" },
                new SelectListItem { Text = "设计单位", Value = "6" },
                new SelectListItem { Text = "监理单位", Value = "7" },
                new SelectListItem { Text = "项目顺序号", Value = "8" },
            };
            if (SelectedID == null | SelectedID == "")
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", 0);
            }
            else
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);
            }
            ViewBag.SearchType = action;
            ViewBag.CurrentFilter = SearchString;
            return View();
        }
        public string  ManagementPrintData(int? page, string type, string content,string SearchType)
        {
            var vwprojectFile = from ad in db.vw_ProjectStatus
                                select ad;
          
            if (content != "" && content != null)//用户在检索框中输入了检索条件
            {
                int t = Int32.Parse(type.Trim());
                if(SearchType=="查找")
                {
                   
                     
                            switch (t)
                            {
                                case 0:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.contractNo.ToString().Contains(content));//根据责任书编号搜索
                                    break;
                                case 1:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.projectName.Contains(content));//根据工程名称搜索
                                    break;
                                case 2:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization.Contains(content));//根据建设单位搜索
                                    break;
                                case 3:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.location.Contains(content));//根据工程地点
                                    break;
                                case 4:
                                  
                                    vwprojectFile = vwprojectFile.Where(ad => ad.projectNo.ToString().Contains(content)); //根据工程序号
                                    break;
                                case 5:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization.Contains(content));//根据施工单位
                                    break;
                                case 6:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization.Contains(content));//根据设计单位
                                    break;
                                case 7:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.jianliUnit.Contains(content));//根据监理单位
                                    break;
                                case 8:
                                   
                                    vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo.ToString().Contains(content));//根据项目顺序号
                                    break;

                            

                        
                    }
                }
                else
                {
                    for (int j = 0; j < content.Length; j++)
                    {
                        string SearchString1 = "";
                        SearchString1 += content[j].ToString();
                        if (!String.IsNullOrEmpty(content))
                        {
                            switch (t)
                            {
                                case 0:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.contractNo.Contains(SearchString1));//根据责任书编号搜索
                                    break;
                                case 1:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.projectName.Contains(SearchString1));//根据工程名称搜索
                                    break;
                                case 2:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization.Contains(SearchString1));//根据建设单位搜索
                                    break;
                                case 3:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.location.Contains(SearchString1));//根据工程地点
                                    break;
                                case 4:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.projectNo.ToString().Contains(SearchString1)); //根据工程序号
                                    break;
                                case 5:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization.Contains(SearchString1));//根据施工单位
                                    break;
                                case 6:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization.Contains(SearchString1));//根据设计单位
                                    break;
                                case 7:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.jianliUnit.Contains(SearchString1));//根据监理单位
                                    break;
                                case 8:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo.ToString().Contains(SearchString1));//根据项目顺序号
                                    break;

                            }

                        }
                    }
                }
              
            }
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            int cnt = vwprojectFile.Count() / pageSize + 1;
            if (vwprojectFile.Count() % pageSize == 0)
            {
                cnt = vwprojectFile.Count() / pageSize;
            }
            vwprojectFile = vwprojectFile.OrderBy(s => s.paperProjectSeqNo).ThenByDescending(s => s.projectNo);
            var a = vwprojectFile.ToPagedList(pageNumber, pageSize);

            var b = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                                        //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程
                                        from p in a
                                        select new JObject(
                                                 new JProperty("projectID", p.projectID),
                                                 new JProperty("projectNo", p.projectNo),
                                                 new JProperty("projectName", p.projectName),
                                                 new JProperty("paperProjectSeqNo", p.paperProjectSeqNo),
                                                 new JProperty("developmentOrganization", p.developmentOrganization),
                                                 new JProperty("location", p.location),
                                                 new JProperty("contractNo", p.contractNo),
                                                 new JProperty("statusName", p.statusName),
                                                 new JProperty("constructionOrganization", p.constructionOrganization)
                                             
                                                

                                        )
                                )
                    )
).ToString();
            return b;
        
        }
        public ActionResult ComplishProject()
        {
            return View();
        }
        public ActionResult CreateFromProjectManag(long ?id)
        {


            vw_projectProfile vmprojectPfofile = new vw_projectProfile();
            string proNo = GetMaxId2();

            if (id.ToString() != "" && id.ToString() != null)
            {

                var model = from a in db.vw_projectProfile
                            where a.projectID == id
                            select a;
                vmprojectPfofile = model.First();
                string csDate = DateTime.Today.Date.ToString("yyyy-MM-dd");
                string receiveDate = DateTime.Today.Date.ToString("yyyy-MM-dd");
                vmprojectPfofile.csyjDate = DateTime.ParseExact(csDate.Trim(), "yyyy-MM-dd", null).Date;
                vmprojectPfofile.dateReceived = DateTime.ParseExact(receiveDate, "yyyy-MM-dd", null).Date;
                ViewData["reception"] = model.First().recipient;
                vmprojectPfofile.projectNo = long.Parse(proNo.Trim());
                //初始化相关控件
                ViewData["ClassNo"] = vmprojectPfofile.prevClassNo;

                var users1 = from ad in ab.AspNetUsers
                             where ad.DepartmentId == 2
                             select ad;
                string user = model.First().recipient;

                ViewBag.recipient = new SelectList(users1, "UserName", "UserName", user);
                vmprojectPfofile.collationRequirement = "整理一套";
                //vmprojectPfofile.csyjPerson = "";
                ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", vmprojectPfofile.retentionPeriodNo);
                ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", vmprojectPfofile.securityID);
                //var users = from ad in ab.AspNetUsers
                //            where ad.DepartmentId == 2
                //            select ad;
                //string user = User.Identity.Name;
                //ViewBag.recipient = new SelectList(users, "UserName", "UserName");
                List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

              };
                ViewBag.isYD = new SelectList(list, "Value", "Text");
                ViewBag.csyjPerson = vmprojectPfofile.csyjPerson;
                //if (vmprojectPfofile.isYD == null)
                //{
                //    vmprojectPfofile.isYD = false;

                //}
                //if (vmprojectPfofile.memo == "材料齐全")
                //{
                //    ViewBag.radiobutton = 1;
                //}
                //else
                //{
                //    ViewBag.radiobutton = 2;
                //}
                //ViewData["new"] = true;
                //string time = DateTime.Today.Year.ToString()+"-"+DateTime.Today.Month.ToString()+"-"+DateTime.Today.Day.ToString();

                vmprojectPfofile.dateReceived = DateTime.Now;
                vmprojectPfofile.csyjDate = DateTime.Now;
            }
            return View(vmprojectPfofile);

        }
        [HttpPost]
        public ActionResult CreateFromProjectManag(string projectName,string projectID,string projectNo,string location,string contractNo,string newLocation,string submitPerson,string projectProfile,string mobilephoneSubmitPerson,string telphoneSubmitPerson,string securityID,string constructionOrganization,string buildingArea,string prevClassNo,string retentionPeriodNo ,string dateReceived ,string recipient,string isYD,string collationRequirement,string developmentOrganization, string devolonpentOrgContacter, string mobilephoneNoDevelopment, string telphoneNoDevelopment, string jianliUnit, string jianliUnitContacter, string telphoneNoJianliUnit, string constructionOrgContacter, string telphoneNoConstruction, string disignOrganization, string designOrgaContacter, string telphoneNoDesignOrga, string csyjPerson, string csyjDate, string memo,string seqNo , string action, string radiobutton,string csyj,string coordinate,string name)
        {
            if (action == "提交")
            {
                if (projectNo.Length != 8)
                {
                    return Content("<script >alert('工程序号格式不正确！');window.history.back();</script >");
                }
                if (projectName == "" || projectName == null)
                {
                    return Content("<script >alert('工程名称不能为空！');window.history.back();</script >");
                }
                if (retentionPeriodNo == "" || retentionPeriodNo == null)
                {
                    return Content("<script >alert('保存期限不能为空！');window.history.back();</script >");
                }
                if (securityID == "" || securityID == null)
                {
                    return Content("<script >alert('档案密级不能为空！');window.history.back();</script >");
                }
                //if (csyj == "" || csyj == null)
                //{
                //    return Content("<script >alert('初审意见不能为空！');window.history.back();</script >");

                //}
                if (csyjDate == "" || csyjDate == null)
                {
                    return Content("<script >alert('初审日期不能为空！');window.history.back();</script >");
                }
                if (prevClassNo == "" || prevClassNo == null)
                {
                    return Content("<script >alert('拟分类号不能为空！');window.history.back();</script >");
                }
                if (radiobutton != "1" && radiobutton != "2")
                {

                    return Content("<script >alert('请选择该工程所提交的材料状态！');window.history.back();</script >");
                }
                //添加ProjectInfo表
                string ID = "";
                var obj = from a in db.ProjectInfo
                          orderby a.projectID descending
                          select a;
                if (obj.Count() == 0)
                {
                    ID = "1";
                }
                else
                {
                    ID = obj.First().projectID.ToString();
                }
                int iProjectid = int.Parse(ID.Trim()) + 1;


                ProjectInfo model = new ProjectInfo();

                model.projectID = iProjectid;
                model.contractNo = "";
                model.projectName = projectName;
                if (developmentOrganization == null || developmentOrganization == "")
                {
                    model.developmentOrganization = "";
                }
                else
                {
                    model.developmentOrganization = developmentOrganization.Trim();
                }
                if (telphoneNoDevelopment == "" || telphoneNoDevelopment == null)
                {
                    model.telphoneNoDevelopment = "";
                }
                else
                {
                    model.telphoneNoDevelopment = telphoneNoDevelopment.Trim();
                }
                if (mobilephoneNoDevelopment == "" || mobilephoneNoDevelopment == null)
                {
                    model.mobilephoneNoDevelopment = "";
                }
                else
                {
                    model.mobilephoneNoDevelopment = mobilephoneNoDevelopment.Trim();
                }
                if (constructionOrganization == null || constructionOrganization == "")
                {
                    model.constructionOrganization = "";
                }
                else
                {
                    model.constructionOrganization = constructionOrganization.Trim();
                }
                if (telphoneNoConstruction == "" || telphoneNoConstruction == null)
                {
                    model.telphoneNoConstruction = "";
                }
                else
                {
                    model.telphoneNoConstruction = telphoneNoConstruction.Trim();
                }
                if (constructionOrgContacter == null || model.telphoneNoDesignOrga == "")
                {
                    model.constructionOrgContacter = "";
                }
                else
                {
                    model.constructionOrgContacter = constructionOrgContacter.Trim();
                }
                model.mobilephoneNoConstruction = "";


                model.retentionPeriodNo = retentionPeriodNo;
                model.securityID = securityID;
                if (isYD == "1")
                {
                    model.isYD = true;
                }
                else
                {
                    model.isYD = false;
                }
                //added by zhoulin,20170523
                if (devolonpentOrgContacter == "" || devolonpentOrgContacter == null)
                {
                    model.devolonpentOrgContacter = "";
                }
                else
                {
                    model.devolonpentOrgContacter = devolonpentOrgContacter.Trim();
                }
                if (constructionOrgContacter == null || constructionOrgContacter == "")
                {
                    model.constructionOrgContacter = "";
                }
                else
                {
                    model.constructionOrgContacter = constructionOrgContacter.Trim();
                }
                if (designOrgaContacter == null || designOrgaContacter == "")
                {
                    model.designOrgaContacter = "";
                }
                else
                {
                    model.designOrgaContacter = designOrgaContacter.Trim();
                }
                if (telphoneNoDesignOrga == null || telphoneNoDesignOrga == "")
                {
                    model.telphoneNoDesignOrga = "";
                }
                else
                {
                    model.telphoneNoDesignOrga = telphoneNoDesignOrga.Trim();
                }

                model.mobilephoneNoDesignOrga = "";
                if (jianliUnitContacter == "" || jianliUnitContacter == null)
                {
                    model.jianliUnitContacter = "";
                }
                else
                {
                    model.jianliUnitContacter = jianliUnitContacter.Trim();
                }
                if (jianliUnit == "" || jianliUnit == null)
                {
                    model.jianliUnit = "";
                }
                else
                {
                    model.jianliUnit = jianliUnit.Trim();
                }

                if (telphoneNoJianliUnit == null || telphoneNoJianliUnit == "")
                {
                    model.telphoneNoJianliUnit = "";
                }
                else
                {
                    model.telphoneNoJianliUnit = telphoneNoJianliUnit.Trim();
                }

                model.mobilephoneNoJianliUnit = "";
                model.memo = memo;


                //added by zhoulin,20170523
                if (disignOrganization == null || disignOrganization == "")
                {
                    model.disignOrganization = "";
                }
                else

                {
                    model.disignOrganization = disignOrganization.Trim();
                }
                if (designOrgaContacter == null || designOrgaContacter == "")
                {
                    model.designOrgaContacter = "";
                }
                else
                {
                    model.designOrgaContacter = designOrgaContacter.Trim();
                }

                if (telphoneNoDesignOrga == null || telphoneNoDesignOrga == "")
                {
                    model.telphoneNoDesignOrga = "";
                }
                else
                {
                    model.telphoneNoDesignOrga = telphoneNoDesignOrga.Trim();

                }
                model.location = location;

                int temp = 3;//去除部门审核和馆长审核流程，直接进入录入流程
                model.status = temp.ToString();
                model.structureTypeID = "1";
                model.newLocation = newLocation;
                model.seqNo = "";
                //添加PaperArchive表
                PaperArchives model1 = new PaperArchives();
                model1.character1cm = 0;
                model1.character2cm = 0;
                model1.character3cm = 0;
                model1.character4cm = 0;
                model1.character5cm = 0;
                model1.drawing1cm = 0;
                model1.drawing2cm = 0;
                model1.drawing3cm = 0;
                model1.drawing4cm = 0;
                model1.drawing5cm = 0;
                model1.characterVolumeCount = 0;
                model1.paperProjectSeqNo = 0;
                model1.drawingVolumeCount = 0;
                model1.projectID = iProjectid;
                model1.projectNo = long.Parse(projectNo.Trim());
                model1.collationRequirement = collationRequirement;
                //model1.recipient = recipient;
                model1.recipient = name;
                model1.submitPerson = submitPerson;
                if (telphoneSubmitPerson == null || telphoneSubmitPerson == "")
                {
                    model1.telphoneSubmitPerson = "";
                }
                else
                {
                    model1.telphoneSubmitPerson = telphoneSubmitPerson.Trim();
                }
                if (mobilephoneSubmitPerson == "" || mobilephoneSubmitPerson == null)
                {
                    model1.mobilephoneSubmitPerson = "";
                }
                else
                {
                    model1.mobilephoneSubmitPerson = mobilephoneSubmitPerson.Trim();
                }
                model1.projectProfile = projectProfile;
                model1.csyjPerson = csyjPerson;//add by niutianbo,date:20090706
                if (csyjDate != "")
                    model1.csyjDate = DateTime.Parse(csyjDate);
                else
                    model1.csyjDate = DateTime.Now.Date;

                if (dateReceived != "")
                    model1.dateReceived = DateTime.Parse(dateReceived);
                else
                    model1.dateReceived = DateTime.Now.Date;
                model1.csyj = csyj;
                if (buildingArea==""|| buildingArea==null)
                {
                    buildingArea = "0";
                }

                model1.buildingArea = double.Parse(buildingArea);
                model1.prevClassNo = prevClassNo;
                model1.bianhaoTime = DateTime.Now.Date;

                model1.fzryj = "经审核，该工程初审意见基本属实，整理费用已收。";
                model1.fzryjDate = DateTime.Now.Date;
                model1.zgyj = "同意";
                model1.zgyjDate = DateTime.Now.Date;
                model1.coordinate = coordinate;
                model1.height = 0.0;
                if (ModelState.IsValid)
                {

                    db.PaperArchives.Add(model1);
                    db.ProjectInfo.Add(model);
                    //try
                    //{
                        db.SaveChanges();

                    //}

                    //catch (Exception ex)
                    //{
                    //    return Content("<script >alert('录入数据有误，请核查！');window.history.back();</script >");
                    //}
                    ViewData["tijiao"] = true;
                    //ViewData["new"] = false;
                    return Content("<script >alert('保存成功！');window.history.back();</script >");

                }
            }
            if (action == "返回")
            {
                return RedirectToAction("ManagementPrint");
            }
            if (action == "录入新工程")
            {
                //iProjectid
                var madID = from a in db.ProjectInfo
                            orderby a.projectID descending
                            select a;
                long ID = madID.First().projectID;
                return RedirectToAction("CreateFromProjectManag", new { id = ID });
            }
            return View();
        }
        public string GetMaxId2()
        {//modify by zhoulin,date:20170523
            string str = DateTime.Now.Year.ToString();
            
           

            var model = from a in db.PaperArchives
                      where a.projectNo.ToString().Substring(0,4).Contains(str)
                      orderby a.projectNo descending
                      select a;
            string obj = model.First().projectNo.ToString();
            if (obj==null|| obj.ToString().Length != 8)
            {
                str = DateTime.Now.Year.ToString() + "0000";
                return str;
            }
            else
            {
                return obj = (model.First().projectNo + 1).ToString();
            }
        }

        public string GetGMaxId2()
        {
            string str = DateTime.Now.Year.ToString();
            int curno = 0;
            var curproNo = from a in bb.gxPaperArchives
                           where a.projectNo.ToString().Contains(str)
                           orderby a.projectNo descending
                           select a.projectNo;



            if (curproNo.Count() == 0)
            {
                str = DateTime.Now.Year.ToString() + "0001";

                return str;
            }
            else
            {
                curno = Convert.ToInt32(curproNo.First()) + 1;
                return curno.ToString();
            }
        }

        public ActionResult AllArchives1(string SelectedID, string SearchString, string action)
        {

            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "项目顺序号", Value = "1"},
                new SelectListItem { Text = "建设单位", Value = "2" },
                new SelectListItem { Text = "起始登记号", Value = "3" },
                new SelectListItem { Text = "截止登记号", Value = "4" },
                new SelectListItem { Text = "设计单位", Value = "5" },
                new SelectListItem { Text = "施工单位", Value = "6" },
                new SelectListItem { Text = "工程地点", Value = "7" },
                new SelectListItem { Text = "工程序号", Value = "8" },
                new SelectListItem { Text = "整理人", Value = "9" },
                new SelectListItem { Text = "接收人", Value = "10" },
                new SelectListItem { Text = "接收日期", Value = "11" },
                new SelectListItem { Text = "监理单位", Value = "12" },
                new SelectListItem { Text = "档案号", Value = "13" },
            };
            if (SelectedID == null | SelectedID == "")
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", 0);
            }
            else
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);
            }
            var vwprojectFile = from ad in db.vw_projectList
                                select ad;
            if (SelectedID != "" && SelectedID != null && SearchString != "" && SearchString != null)//用户在检索框中输入了检索条件
            {
                int t = Int32.Parse(SelectedID.Trim());

                if (action == "查找")
                {
                    switch (t)
                    {
                        case 0:
                            vwprojectFile = vwprojectFile.Where(ad => ad.projectName.Contains( SearchString));//根据责任书编号搜索
                            break;
                        case 1:
                         
                            vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo.ToString().Contains( SearchString));//根据工程名称搜索
                            break;
                        case 2:
                            vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization.Contains( SearchString));//根据建设单位搜索
                            break;
                        case 3:
                            vwprojectFile = vwprojectFile.Where(ad => ad.startRegisNo.Contains( SearchString));//根据工程地点
                            break;
                        case 4:

                            vwprojectFile = vwprojectFile.Where(ad => ad.endRegisNo.Contains( SearchString)); //根据工程序号
                            break;
                        case 5:
                            vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization.Contains( SearchString));//根据施工单位
                            break;
                        case 6:
                            vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization.Contains( SearchString));//根据设计单位
                            break;
                        case 7:
                            vwprojectFile = vwprojectFile.Where(ad => ad.location.Contains( SearchString));//根据监理单位
                            break;
                        case 8:

                            vwprojectFile = vwprojectFile.Where(ad => ad.projectNo.ToString().Contains( SearchString));//根据项目顺序号
                            break;
                        case 9:

                            vwprojectFile = vwprojectFile.Where(ad => ad.collator.Contains( SearchString));//根据项目顺序号
                            break;
                        case 10:

                            vwprojectFile = vwprojectFile.Where(ad => ad.recipient.Contains( SearchString));//根据项目顺序号
                            break;
                        case 11:
                            DateTime time = Convert.ToDateTime( SearchString);
                            vwprojectFile = vwprojectFile.Where(ad => ad.dateReceived == time);//根据项目顺序号
                            break;
                        case 12:

                            vwprojectFile = vwprojectFile.Where(ad => ad.jianliUnit.Contains( SearchString));//根据项目顺序号
                            break;
                        case 13:

                            vwprojectFile = vwprojectFile.Where(ad => ad.startArchiveNo.Contains(SearchString));//根据项目顺序号
                            break;
                    }
                }
                else
                {
                    for (int j = 0; j <  SearchString.Length; j++)
                    {

                        string  SearchString1 = "";
                         SearchString1 +=  SearchString[j].ToString();
                        if (!String.IsNullOrEmpty( SearchString1))
                        {
                            switch (t)
                            {
                                case 0:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.projectName.Contains( SearchString1));//根据责任书编号搜索
                                    break;
                                case 1:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo.ToString().Contains( SearchString1));//根据工程名称搜索
                                    break;
                                case 2:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization.Contains( SearchString1));//根据建设单位搜索
                                    break;
                                case 3:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.startRegisNo.Contains( SearchString1));//根据工程地点
                                    break;
                                case 4:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.endRegisNo.Contains( SearchString1)); //根据工程序号
                                    break;
                                case 5:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization.Contains( SearchString1));//根据施工单位
                                    break;
                                case 6:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization.Contains( SearchString1));//根据设计单位
                                    break;
                                case 7:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.location.Contains( SearchString1));//根据监理单位
                                    break;
                                case 8:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.projectNo.ToString().Contains( SearchString1));//根据项目顺序号
                                    break;
                                case 9:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.collator.Contains( SearchString1));//根据项目顺序号
                                    break;
                                case 10:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.recipient.Contains( SearchString1));//根据项目顺序号
                                    break;
                                case 11:
                                    DateTime time = Convert.ToDateTime( SearchString1);
                                    vwprojectFile = vwprojectFile.Where(ad => ad.dateReceived == time);//根据项目顺序号
                                    break;
                                case 12:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.jianliUnit.Contains(SearchString));//根据项目顺序号
                                    break;
                                case 13:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.startArchiveNo.Contains(SearchString));//根据项目顺序号
                                    break;
                            }
                        }
                    }
                }
            }



            vwprojectFile = vwprojectFile.Where(a => a.projectName != "重号").Where(b => b.projectName != "重").Where(c => c.projectName != "重复").Where(d => d.projectName != "重复号").Where(f => f.projectName != "作废").Where(f => f.projectName != "作废").Where(f => f.projectName != "AAAAAAA").Where(f => f.projectName != "并入20132005").Where(f => f.projectName != "删掉").Where(f => f.projectName != "修改").Where(f => f.projectName != "并入20131385号").Where(f => f.projectName != "（此号作废此工程并入20131406）李沧区楼山工业区村庄改造搬迁办公室楼山工业区石家村、徐家村村庄改造安置房工程6#、7#及地下车库、12#—15#楼工程节能资料").Where(f => f.projectName != "并入20110366青岛大剧院工程").Where(f => f.projectName != "并20150622").Where(f => f.projectName != "并入20140274号").Where(f => f.projectName != "并入20150438号").Where(f => f.projectName != "与20140084号重复").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "并入20140106号").Where(f => f.projectName != "并入20140108号").Where(f => f.projectName != "并入20140053号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20130041号").Where(f => f.projectName != "废").Where(f => f.projectName != "空白").Where(f => f.projectName != "作废").Where(f => f.projectName != "并入20131745号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20131771号").Where(f => f.projectName != "并入20131886号").Where(f => f.projectName != "并入20131751号").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "青岛作废").Where(f => f.projectName != "青作废").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "ccc").OrderBy(s => s.paperProjectSeqNo).ThenBy(s => s.projectNo);
            ViewBag.CurrentFilter =  SearchString;
            ViewBag.SearchType = action;
            ViewBag.count = vwprojectFile.Count();
            return View();
        }
        public string AllArchivesData1(int? page, string type, string content, string SearchType)
        {
            var vwprojectFile = from ad in db.vw_projectList

                                select ad;
            if (type != "" && type != null && content != "" && content != null)//用户在检索框中输入了检索条件
            {
                int t = Int32.Parse(type.Trim());

                if (SearchType == "查找")
                {
                    switch (t)
                    {
                        case 0:
                            vwprojectFile = vwprojectFile.Where(ad => ad.projectName.Contains(content));//根据责任书编号搜索
                            break;
                        case 1:

                            vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo.ToString().Contains(content));//根据工程名称搜索
                            break;
                        case 2:
                            vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization.Contains(content));//根据建设单位搜索
                            break;
                        case 3:
                            vwprojectFile = vwprojectFile.Where(ad => ad.startRegisNo.Contains(content));//根据工程地点
                            break;
                        case 4:

                            vwprojectFile = vwprojectFile.Where(ad => ad.endRegisNo.Contains(content)); //根据工程序号
                            break;
                        case 5:
                            vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization.Contains(content));//根据施工单位
                            break;
                        case 6:
                            vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization.Contains(content));//根据设计单位
                            break;
                        case 7:
                            vwprojectFile = vwprojectFile.Where(ad => ad.location.Contains(content));//根据监理单位
                            break;
                        case 8:

                            vwprojectFile = vwprojectFile.Where(ad => ad.projectNo.ToString().Contains(content));//根据项目顺序号
                            break;
                        case 9:

                            vwprojectFile = vwprojectFile.Where(ad => ad.collator.Contains(content));//根据项目顺序号
                            break;
                        case 10:

                            vwprojectFile = vwprojectFile.Where(ad => ad.recipient.Contains(content));//根据项目顺序号
                            break;
                        case 11:
                            DateTime time = Convert.ToDateTime(content);
                            vwprojectFile = vwprojectFile.Where(ad => ad.dateReceived == time);//根据项目顺序号
                            break;
                        case 12:

                            vwprojectFile = vwprojectFile.Where(ad => ad.jianliUnit.Contains(content));//根据项目顺序号
                            break;
                        case 13:

                            vwprojectFile = vwprojectFile.Where(ad => ad.startArchiveNo.Contains(content));//根据项目顺序号
                            break;
                    }
                }
                else
                {
                    for (int j = 0; j < content.Length; j++)
                    {

                        string content1 = "";
                        content1 += content[j].ToString();
                        if (!String.IsNullOrEmpty(content1))
                        {
                            switch (t)
                            {
                                case 0:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.projectName.Contains(content1));//根据责任书编号搜索
                                    break;
                                case 1:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo.ToString().Contains(content1));//根据工程名称搜索
                                    break;
                                case 2:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization.Contains(content1));//根据建设单位搜索
                                    break;
                                case 3:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.startRegisNo.Contains(content1));//根据工程地点
                                    break;
                                case 4:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.endRegisNo.Contains(content1)); //根据工程序号
                                    break;
                                case 5:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization.Contains(content1));//根据施工单位
                                    break;
                                case 6:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization.Contains(content1));//根据设计单位
                                    break;
                                case 7:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.location.Contains(content1));//根据监理单位
                                    break;
                                case 8:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.projectNo.ToString().Contains(content1));//根据项目顺序号
                                    break;
                                case 9:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.collator.Contains(content1));//根据项目顺序号
                                    break;
                                case 10:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.recipient.Contains(content1));//根据项目顺序号
                                    break;
                                case 11:
                                    DateTime time = Convert.ToDateTime(content1);
                                    vwprojectFile = vwprojectFile.Where(ad => ad.dateReceived == time);//根据项目顺序号
                                    break;
                                case 12:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.jianliUnit.Contains(content));//根据项目顺序号
                                    break;
                                case 13:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.startArchiveNo.Contains(content));//根据项目顺序号
                                    break;
                            }
                        }
                    }
                }
            }
            vwprojectFile = vwprojectFile.Where(c => c.projectName != "重号").Where(c => c.projectName != "重").Where(c => c.projectName != "重复").Where(d => d.projectName != "重复号").Where(f => f.projectName != "作废").Where(f => f.projectName != "作废").Where(f => f.projectName != "AAAAAAA").Where(f => f.projectName != "并入20132005").Where(f => f.projectName != "删掉").Where(f => f.projectName != "修改").Where(f => f.projectName != "并入20131385号").Where(f => f.projectName != "（此号作废此工程并入20131406）李沧区楼山工业区村庄改造搬迁办公室楼山工业区石家村、徐家村村庄改造安置房工程6#、7#及地下车库、12#—15#楼工程节能资料").Where(f => f.projectName != "并入20110366青岛大剧院工程").Where(f => f.projectName != "并20150622").Where(f => f.projectName != "并入20140274号").Where(f => f.projectName != "并入20150438号").Where(f => f.projectName != "与20140084号重复").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "并入20140106号").Where(f => f.projectName != "并入20140108号").Where(f => f.projectName != "并入20140053号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20130041号").Where(f => f.projectName != "废").Where(f => f.projectName != "空白").Where(f => f.projectName != "作废").Where(f => f.projectName != "并入20131745号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20131771号").Where(f => f.projectName != "并入20131886号").Where(f => f.projectName != "并入20131751号").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "青岛作废").Where(f => f.projectName != "青作废").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "ccc").OrderBy(s => s.paperProjectSeqNo);
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            int cnt = vwprojectFile.Count() / pageSize + 1;
            if (vwprojectFile.Count() % pageSize == 0)
            {
                cnt = vwprojectFile.Count() / pageSize;
            }
            vwprojectFile = vwprojectFile.OrderBy(s => s.paperProjectSeqNo).ThenBy(s => s.projectNo);
            var a = vwprojectFile.ToPagedList(pageNumber, pageSize);

            var b = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                                        //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程
                                        from p in a
                                        select new JObject(
                                                 new JProperty("projectName", p.projectName),
                                                 new JProperty("projectNo", p.projectNo),
                                                 new JProperty("paperProjectSeqNo", p.paperProjectSeqNo),
                                                 new JProperty("d", p.startArchiveNo + "-" + p.endArchiveNo),
                                                 new JProperty("e", p.startRegisNo + "-" + p.endRegisNo),
                                                 new JProperty("location", p.location),
                                                 new JProperty("developmentOrganization", p.developmentOrganization),
                                                 new JProperty("disignOrganization", p.disignOrganization),
                                                 new JProperty("constructionOrganization", p.constructionOrganization),
                                                 new JProperty("statusName", p.statusName),
                                                 new JProperty("collator", p.collator),
                                                 new JProperty("recipient", p.recipient),
                                                 new JProperty("projectID", p.projectID),
                                                 new JProperty("dateReceived", p.dateReceived),
                                                 new JProperty("dateArchive", p.dateArchive)
                                             

                                        )
                                )
                    )
).ToString();
            return b;
        }
    }
}
