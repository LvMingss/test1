using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using urban_archive.Models;
using Newtonsoft.Json;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Data.SqlClient;
using System.Data.OleDb;
namespace urban_archive.Controllers.GuanXianArchives
{

    public class LuruProjectController : Controller
    {
        private UrbanConEntities db2 = new UrbanConEntities();
        private UrbanUsersEntities ab2 = new UrbanUsersEntities();
        private gxArchivesContainer db = new gxArchivesContainer();
        private UrbanConEntities db1 = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        private OfficeEntities cb = new OfficeEntities();
        public ActionResult ZTree()
        {
            return View();
        }
        public ActionResult ComplishProject()
        {
            return View();
        }

        
        // GET: LuruProject
        public ActionResult Index()
        {
            var gxprojectinfo = from ad in db.gxProjectInfo
                              //orderby ad.leibie
                          orderby ad.projectID descending
                          select ad;//按类别排序
            return View(gxprojectinfo);
        }

        // GET: LuruProject/Details/5
        public ActionResult Details(long? id, string id2,string classify, string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var test = from ad in db.vw_gxprojectProfile
                       where (ad.projectNo == id)
                       where ad.classifyID==classify
                       select ad;
            vw_gxprojectProfile gxprojectProfile = test.First();
            if (gxprojectProfile == null)
            {
                return HttpNotFound();
            }
            if (action == "返回")
            {
                if (id2 == "3")
                {
                    return RedirectToAction("linquzhengli", "gxPaperSettle");
                }
                if (id2 == "4")
                {
                    return RedirectToAction("informationzhengli", "gxPaperSettle");
                }
                return RedirectToAction("ManagementPrint", "LuruProject");
            }
            if (action == "删除")
            {
                var c = from a in db.gxProjectInfo
                        where a.projectID == id
                        select a;
                gxProjectInfo gxprojectInfo = c.First();
                var d = from b in db.gxPaperArchives
                        where b.projectID == id
                        select b;
                gxPaperArchives gxpaperArchive = d.First();
                db.gxProjectInfo.Remove(gxprojectInfo);
                db.gxPaperArchives.Remove(gxpaperArchive);
                db.SaveChanges();
                if (id2 == "3")
                {
                    return Content("<script >alert('删除成功！');window.location.href='/gxPaperSettle/linquzhengli';</script >");
                }
                if (id2 == "4")
                {
                    return Content("<script >alert('删除成功！');window.location.href='/gxPaperSettle/informationzhengli';</script >");
                }
                return Content("<script >alert('删除成功！');parent.location.href='/LuruProject/ManagementPrint';</script >");
            }

            return View(gxprojectProfile);
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
                var temp = cb.vw_receiveArchiveDetail.Where(ad => ad.projectNo >= n).Where(ad => ad.projectNo <= m);
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
                var temp = cb.vw_receiveArchiveDetail.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.paperProjectSeqNo <= m);
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
            return View();
        }
        public ActionResult ManagementPrint(string SearchString)
        {
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
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            string t = Request.Form["SelectedID"];
            ViewBag.CurrentFilter = SearchString;
            var vwgxProjectStatus = from ad in db.vw_gxProjectStatus
                                select ad;


            if (!String.IsNullOrEmpty(SearchString))
            {
                int n = int.Parse(Request.Form["SelectedID"]);
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", t);
                switch (n)
                {
                    case 0:
                        vwgxProjectStatus = vwgxProjectStatus.Where(ad => ad.contractNo.Contains(SearchString));//根据责任书编号搜索
                        break;
                    case 1:
                        vwgxProjectStatus = vwgxProjectStatus.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 2:
                        vwgxProjectStatus = vwgxProjectStatus.Where(ad => ad.developmentOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwgxProjectStatus = vwgxProjectStatus.Where(ad => ad.location.Contains(SearchString));//根据工程地点
                        break;
                    case 4:
                        int Search = Convert.ToInt32(SearchString);
                        vwgxProjectStatus = vwgxProjectStatus.Where(ad => ad.projectNo == Search); //根据工程序号
                        break;
                    case 5:
                        vwgxProjectStatus = vwgxProjectStatus.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位
                        break;
                    case 6:
                        vwgxProjectStatus = vwgxProjectStatus.Where(ad => ad.disignOrganization.Contains(SearchString));//根据设计单位
                        break;
                    case 7:
                        vwgxProjectStatus = vwgxProjectStatus.Where(ad => ad.jianliUnit.Contains(SearchString));//根据监理单位
                        break;
                    case 8:
                        long seq = Convert.ToInt32(SearchString);
                        vwgxProjectStatus = vwgxProjectStatus.Where(ad => ad.paperProjectSeqNo == seq);//根据项目顺序号
                        break;

                }

            }
            // 默认按责任书编号排
            vwgxProjectStatus = vwgxProjectStatus.OrderByDescending(s => s.projectNo);
            ViewBag.result = JsonConvert.SerializeObject(vwgxProjectStatus);
            return View();
        }
        // GET: LuruProject/Create
        public ActionResult gxCreate()

        {
            ViewBag.classifyID = new SelectList(db.gxClassType,"classTypeID","classTypeName");//管线分类
            vw_gxprojectProfile vmprojectPfofile = new vw_gxprojectProfile();
            var ProjectNo= from a in db.gxPaperArchives//首次进入默认显示建筑工程地下管线工程（外部）最大工程序号
                           where a.classifyID== "1         "
                           orderby a.projectNo descending
                            select a;
            long max_ProjectNo = 0, max_projectID=0;
            if (ProjectNo.Count()==0)//第一次录入数据,初始化工程序号
            {
                max_ProjectNo=20170001;
            }
             else
            {
                max_ProjectNo = Convert.ToInt32(ProjectNo.First().projectNo);
                vmprojectPfofile.projectNo = max_ProjectNo + 1;
            }
            var ProjectId = from b in db.gxProjectInfo
                            orderby b.projectID descending
                            select b;
            if(ProjectId.Count()==0)
            {
                max_projectID = 0;
            }
            else
            {
                max_projectID = ProjectId.First().projectID;
                vmprojectPfofile.projectID = max_projectID + 1;
            }
            //初始化相关控件

            ViewBag.retentionPeriodNo = new SelectList(db1.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName",1);
            ViewBag.securityID = new SelectList(db1.SecurityClassification, "securityID", "securityName",1);
            var users = from ad in ab.AspNetUsers
                        where ad.DepartmentId == 2 || ad.DepartmentId ==4
                        select ad;
            ViewBag.recipient = new SelectList(users, "UserName", "UserName");
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

            };
            ViewBag.isYD = new SelectList(list, "Value", "Text",0);
            ViewBag.data = DateTime.Now.ToString("yyyy-MM-dd");


            return View(vmprojectPfofile);
        }


        // POST: ProjectInfoes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken] //传入
        public ActionResult gxCreate([Bind(Include = "projectID,contractNo,projectName,location,newLocation,submitPerson,mobilephoneSubmitPerson,telphoneSubmitPerson,securityID,projectNo,buildingArea,Material,layerCount,prevClassNo,retentionPeriodNo,dateReceived,recipient,projectProfile ,collationRequirement,developmentOrganization,devolonpentOrgContacter,mobilephoneNoDevelopment,telphoneNoDevelopment,jianliUnit,jianliUnitContacter,telphoneNoJianliUnit,constructionOrganization,constructionOrgContacter,telphoneNoConstruction,disignOrganization,designOrgaContacter,telphoneNoDesignOrga,MapOrginisation,Mapper,TeleNoMap,csyjPerson,csyjDate,memo,isFafangHegezheng,isLingquYijiaoshu,isCharge,isFinanceCharge,seqNo,classifyID,dateF,dateE,height,MapNo,guangpanNo,coordinate")] vw_gxprojectList vwprojectProfile, string action, string radiobutton,  string csyj,string isYD)
        {

           
            var classid = vwprojectProfile.classifyID.Trim();
           
            if (action == "返回")
            {
                return RedirectToAction("Index", "ProjectManagement");
            }
           
             if (action == "录入新工程")
            {
                return RedirectToAction("gxCreate");
              

            }

            if (action == "提交")
            {

                if (vwprojectProfile.projectNo == null || vwprojectProfile.projectNo.ToString() == "")
                {
                    return Content("<script >alert('工程序号不能为空，请核查！');window.history.back();</script >");
                }
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
                if (vwprojectProfile.retentionPeriodNo == null || vwprojectProfile.retentionPeriodNo == "")
                {
                    return Content("<script >alert('保存期限不能为空！');window.history.back();</script >");
                }
                if (vwprojectProfile.securityID == null || vwprojectProfile.securityID == "")
                {
                    return Content("<script >alert('档案密级不能为空！');window.history.back();</script >");
                }
                if (csyj == null || csyj == "")
                {
                    return Content("<script >alert('初审意见不能为空！');window.history.back();</script >");
                }
                if (vwprojectProfile.csyjDate == null)
                {
                    return Content("<script >alert('初审日期不能为空！');window.history.back();</script >");
                }
                if (vwprojectProfile.prevClassNo == null || vwprojectProfile.prevClassNo == "")
                {
                    return Content("<script >alert('拟分类号不能为空，请核查！');window.history.back();</script >");
                }
                if (ModelState.IsValid)
                {
                    string file = Request.Form["MyUploadile"];
                    string filename = Request.Form["name1"];
                    var files = Request.Files;
                    //if (filename != "")
                    //{
                    //    if (files.Count > 0)
                    //    {
                    //        var file1 = files[0];
                    //        var no = vwprojectProfile.projectNo;
                    //        string pa1 = "~/files/guanxianWord/" + no;
                    //        string pa = AppDomain.CurrentDomain.BaseDirectory + "files\\guanxianWord\\" + no;
                    //        if (!Directory.Exists(pa))
                    //        {
                    //            Directory.CreateDirectory(pa);
                    //        }
                    //        string strFileSavePath = Request.MapPath(pa1);//文件存储路径
                    //        file1.SaveAs(strFileSavePath + "/" + filename);
                    //    }
                    //}
                    if (filename != "")
                    {
                        if (files.Count > 0)
                        {
                            for (int i = 0; i < files.Count - 1; i++)//还包括前台ID = project
                            {
                                var file1 = files[i];
                                var no = vwprojectProfile.projectNo;
                                string pa1 = "~/files/guanxianWord/" + no;
                                string pa = AppDomain.CurrentDomain.BaseDirectory + "files\\guanxianWord\\" + no;
                                if (!Directory.Exists(pa))
                                {
                                    Directory.CreateDirectory(pa);
                                }
                                string strFileSavePath = Request.MapPath(pa1);//文件存储路径
                                //file1.SaveAs(strFileSavePath + "/" + filename);
                                var pos = files[i].FileName.LastIndexOf("\\");
                                var str = files[i].FileName.Substring(pos + 1);
                                file1.SaveAs(strFileSavePath + "/" + str);
                            }
                        }
                    }

                    gxPaperArchives paperArchive = new gxPaperArchives();
                    gxProjectInfo projectInfo = new gxProjectInfo();
                    paperArchive.paperProjectSeqNo = 0;//paperProjectSeqNo还不能编号，先统一赋值0
                    
                    //更新ProjectInfo表                                          
                    projectInfo.dateE = vwprojectProfile.dateE;
                    projectInfo.dateF = vwprojectProfile.dateF;

                    var n1 = vwprojectProfile.projectNo;
                    string p1 = AppDomain.CurrentDomain.BaseDirectory + "files\\guanxianWord\\" + n1;
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
                    projectInfo.contractNo = "";
                    projectInfo.projectName = vwprojectProfile.projectName;
                    projectInfo.location = vwprojectProfile.location;
                    projectInfo.newLocation = vwprojectProfile.newLocation;
                    projectInfo.securityID = vwprojectProfile.securityID;
                    projectInfo.retentionPeriodNo = vwprojectProfile.retentionPeriodNo;

                    projectInfo.developmentOrganization = vwprojectProfile.developmentOrganization;
                    projectInfo.mobilephoneNoDevelopment = vwprojectProfile.mobilephoneNoDevelopment;
                    projectInfo.telphoneNoDevelopment = vwprojectProfile.telphoneNoDevelopment;
                    projectInfo.devolonpentOrgContacter = vwprojectProfile.devolonpentOrgContacter;
                    projectInfo.constructionOrganization = vwprojectProfile.constructionOrganization;//施工单位
                    projectInfo.telphoneNoConstruction = vwprojectProfile.telphoneNoConstruction;//施工单位技术人员
                    projectInfo.constructionOrgContacter = vwprojectProfile.constructionOrgContacter;//施工单位法人
                    projectInfo.mobilephoneNoConstruction = "";//没用了
                    projectInfo.disignOrganization = vwprojectProfile.disignOrganization;
                    projectInfo.designOrgaContacter = vwprojectProfile.designOrgaContacter;
                    projectInfo.telphoneNoDesignOrga = vwprojectProfile.telphoneNoDesignOrga;
                    projectInfo.jianliUnit = vwprojectProfile.jianliUnit;
                    projectInfo.jianliUnitContacter = vwprojectProfile.jianliUnitContacter;
                    projectInfo.telphoneNoJianliUnit = vwprojectProfile.telphoneNoJianliUnit;
                    projectInfo.mobilephoneNoJianliUnit = "";
                    projectInfo.MapOrginisation = vwprojectProfile.MapOrginisation;
                    projectInfo.Mapper = vwprojectProfile.Mapper;
                    projectInfo.TeleNoMap = vwprojectProfile.TeleNoMap;
                    projectInfo.MapNo = vwprojectProfile.MapNo;
                    projectInfo.guangpanNo = vwprojectProfile.guangpanNo;
                    if (isYD.Trim() == "是")
                    {
                        projectInfo.isYD = true;

                    }
                    else
                    {
                        projectInfo.isYD = false;
                    }
                    projectInfo.memo = vwprojectProfile.memo;

                    projectInfo.status = "3";
                    projectInfo.classifyID = classid;
                    //更新paperArchive表 
                    paperArchive.coordinate = vwprojectProfile.coordinate;
                    paperArchive.height = vwprojectProfile.height;
                    paperArchive.projectID = vwprojectProfile.projectID;
                    paperArchive.projectNo = vwprojectProfile.projectNo;
                    paperArchive.submitPerson = vwprojectProfile.submitPerson;
                    paperArchive.mobilephoneSubmitPerson = vwprojectProfile.mobilephoneSubmitPerson;
                    paperArchive.telphoneSubmitPerson = vwprojectProfile.telphoneSubmitPerson;
                    paperArchive.buildingArea = vwprojectProfile.buildingArea;
                    paperArchive.Material = vwprojectProfile.Material;
                    paperArchive.layerCount = vwprojectProfile.layerCount;
                    paperArchive.prevClassNo = vwprojectProfile.prevClassNo;
                    paperArchive.recipient = vwprojectProfile.recipient;
                    paperArchive.projectProfile = vwprojectProfile.projectProfile;
                    paperArchive.collationRequirement = vwprojectProfile.collationRequirement;
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


                    paperArchive.bianhaoTime = DateTime.Now.Date;
                    paperArchive.fzryj = "经审核，该工程初审意见基本属实，整理费用已收。";
                    paperArchive.fzryjDate = DateTime.Now.Date;
                    paperArchive.zgyj = "同意";
                    paperArchive.zgyjDate = DateTime.Now.Date;
                    paperArchive.structureTypeID = "1";
                    paperArchive.classifyID = classid;

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

                    projectInfo.projectName = vwprojectProfile.projectName;
                    projectInfo.isNB = "外部";
                    projectInfo.projectID = vwprojectProfile.projectID;//工程信息表的projectID与工程目录表projectID为一对一关系
                    paperArchive.projectID = vwprojectProfile.projectID;

                    db.gxPaperArchives.Add(paperArchive);
                    db.gxProjectInfo.Add(projectInfo);
                    db.SaveChanges();
                    return Content("<script >alert('保存成功！');window.history.back();</script >");
                    
                }

                else
                {
                    return Content("<script >alert('数据错误，请重新提交！');window.history.back();</script >");
                }

            }

            if (action == "文件上传")
            {
                return RedirectToAction("Fileuploading", "ProjectInfoes");
            }
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
                //循环  2018/9/6  吕鸣
                int i = 0;

                for (i = 0; i < table.Rows.Count; i++)
                {
                    ////工程类型
                    //string projectType = table.Rows[i][0].ToString().Trim();//档案密级
                    //if (projectType != "建筑工程地下管线工程" && projectType != "市政地下管线工程")
                    //{
                    //    return Content("<script >alert('工程类型输入有错！（建筑工程地下管线工程/市政地下管线工程）');window.history.back();</script >");
                    //}
                    //int ProjectTyp = 1;
                    //if (projectType == "建筑工程地下管线工程")
                    //{
                    //    ProjectTyp = 1;
                    //}
                    //else if (projectType == "市政地下管线工程")
                    //{
                    //    ProjectTyp = 2;
                    //}

                    string ProjectName = table.Rows[i][0].ToString().Trim();//工程项目题名
                    if (ProjectName == "")
                    {
                        ProjectName = "";
                    }
                    string ProjectLocation = table.Rows[i][1].ToString().Trim();//工程地点
                    if (ProjectLocation == "")
                    {
                        ProjectLocation = "";
                    }
                    string NewProjectLocation = table.Rows[i][2].ToString().Trim();//最新工程地点
                    //int member;
                    if (NewProjectLocation == "")
                    {
                        NewProjectLocation = "";
                    }
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
                    //string projectNo = table.Rows[i][8].ToString().Trim();//工程序号
                    //if (projectNo == "")
                    //{
                    //    return Content("<script >alert('工程序号不正确');window.history.back();</script >");
                    //}

                    //开工日期
                    string kgDate = table.Rows[i][7].ToString().Trim(); //dr["开工日期"].ToString().Trim();
                    DateTime? kgDate1;
                    if (kgDate == "")
                    {
                        kgDate1 = null;
                    }
                    else
                    {
                        try
                        {
                            kgDate1 = DateTime.Parse(kgDate);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('开工日期格式不正确！（2018/10/1）');window.history.back();</script >");
                        }
                    }

                    //竣工日期
                    string jgDate = table.Rows[i][8].ToString().Trim(); //dr["竣工日期"].ToString().Trim();
                    DateTime? jgDate1;
                    if (jgDate == "")
                    {
                        jgDate1 = null;
                    }
                    else
                    {
                        try
                        {
                            jgDate1 = DateTime.Parse(jgDate);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('竣工日期格式不正确！（2018/10/1）');window.history.back();</script >");
                        }
                    }

                    //管线长度
                    string gxlength = table.Rows[i][9].ToString().Trim(); //dr["管线长度"].ToString().Trim();
                    float gxlength1 = 0;
                    if (gxlength == "")
                    {
                        gxlength1 = 0;
                    }
                    else
                    {
                        try
                        {
                            gxlength1 = float.Parse(gxlength);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('管线长度应输入数字！');window.history.back();</script >");
                        }
                    }

                    //管径
                    string gxr1 = table.Rows[i][10].ToString().Trim(); //dr["管径"].ToString().Trim();
                    if (gxr1 == "") {
                        gxr1 = null;
                    }

                    //材质
                    string gxmeteria = table.Rows[i][11].ToString().Trim(); //dr["材质"].ToString().Trim();
                    if (gxmeteria == "")
                    {
                        gxmeteria = null;
                    }

                    //埋深
                    string gxmaishen1 = table.Rows[i][12].ToString().Trim(); //dr["埋深"].ToString().Trim();
                    if (gxmaishen1 == "")
                    {
                        gxmaishen1 = null;
                    }

                    //拟分类号
                    string Nfenlei = table.Rows[i][13].ToString().Trim();//拟分类号
                    if (Nfenlei == "")
                    {
                        Nfenlei = "";
                    }
                    //保存期限

                    string retentionPeriodName1 = table.Rows[i][14].ToString().Trim(); //dr["保存期限"].ToString().Trim();
                    if (retentionPeriodName1 != "长期" && retentionPeriodName1 != "永久" && retentionPeriodName1 != "短期")
                    {
                        return Content("<script >alert('保存期限的输入格式不正确！（长期/永久/短期）');window.history.back();</script >");
                    }
                    //测绘编码
                    string chcode = table.Rows[i][15].ToString().Trim(); //dr["测绘编码"].ToString().Trim();
                    if (chcode == "")
                    {
                        chcode = "";
                    }
                    //光盘编号
                    string gpNo = table.Rows[i][16].ToString().Trim(); //dr["光盘编号"].ToString().Trim();
                    if (gpNo == "")
                    {
                        gpNo = "";
                    }
                    //接受日期
                    string jsDate1 = table.Rows[i][17].ToString().Trim(); //dr["接收日期"].ToString().Trim();
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
                    //接收人
                    string jspeople = table.Rows[i][18].ToString().Trim(); //dr["接收人"].ToString().Trim();
                    if (jspeople == "")
                    {
                        jspeople = "";
                    }
                    //异地存放
                    string yd = table.Rows[i][19].ToString().Trim(); //dr["异地存放"].ToString().Trim();
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
                    //项目概况
                    string xmgaikuang = table.Rows[i][20].ToString().Trim(); //dr["项目概况"].ToString().Trim();
                    if (xmgaikuang == "")
                    {
                        xmgaikuang = "";
                    }
                    //整理要求
                    string zlyaoqiu = "整理一套";

                    //建设单位
                    string developmentOrganization1 = table.Rows[i][21].ToString().Trim(); //dr["建设单位"].ToString().Trim();
                    if (developmentOrganization1 == "")
                    {
                        developmentOrganization1 = "";
                    }
                    //建设单位负责人
                    string jianshefzpeople = table.Rows[i][22].ToString().Trim(); //dr["建设单位负责人"].ToString().Trim();
                    if (jianshefzpeople == "")
                    {
                        jianshefzpeople = "";
                    }
                    //建设单位联系人
                    string jianshelxpeople = table.Rows[i][23].ToString().Trim(); //dr["联系人"].ToString().Trim();
                    if (jianshelxpeople == "")
                    {
                        jianshelxpeople = "";
                    }
                    //建设单位移动电话
                    string jiansheTelephone = table.Rows[i][24].ToString().Trim();//建设单位移动电话
                    if (jiansheTelephone == "")
                    {
                        jiansheTelephone = "";
                    }
                    //监理单位
                    string jlOrganization = table.Rows[i][25].ToString().Trim(); //dr["监理单位"].ToString().Trim();
                    if (jlOrganization == "")
                    {
                        jlOrganization = "";
                    }
                    //监理单位负责人
                    string jlfzpeople = table.Rows[i][26].ToString().Trim(); //dr["监理负责人"].ToString().Trim();
                    if (jlfzpeople == "")
                    {
                        jlfzpeople = "";
                    }
                    //监理者
                    string telphoneNoJianliUnit = table.Rows[i][27].ToString().Trim(); //dr["监理者"].ToString().Trim();
                    if (telphoneNoJianliUnit == "")
                    {
                        telphoneNoJianliUnit = "";
                    }
                    //施工单位
                    string constructionOrganization1 = table.Rows[i][28].ToString().Trim(); //dr["施工单位"].ToString().Trim();
                    if (constructionOrganization1 == "")
                    {
                        constructionOrganization1 = "";
                    }
                    //法人
                    string fapeople = table.Rows[i][29].ToString().Trim(); //dr["法人"].ToString().Trim();
                    if (fapeople == "")
                    {
                        fapeople = "";
                    }
                    //技术人员
                    string jishupeople = table.Rows[i][30].ToString().Trim(); //dr["技术人员"].ToString().Trim();
                    if (jishupeople == "")
                    {
                        jishupeople = "";
                    }
                    //设计单位
                    string disignOrganization1 = table.Rows[i][31].ToString().Trim(); //dr["设计单位"].ToString().Trim();
                    if (disignOrganization1 == "")
                    {
                        disignOrganization1 = "";
                    }
                    //负责人
                    string sjfzpeople = table.Rows[i][32].ToString().Trim(); //dr["负责人"].ToString().Trim();
                    if (sjfzpeople == "")
                    {
                        sjfzpeople = "";
                    }
                    //设计者
                    string telphoneNoDesignOrga = table.Rows[i][33].ToString().Trim(); //dr["设计人"].ToString().Trim();
                    if (telphoneNoDesignOrga == "")
                    {
                        telphoneNoDesignOrga = "";
                    }
                    //测绘单位
                    string chdanwei = table.Rows[i][34].ToString().Trim(); //dr["测绘单位"].ToString().Trim();
                    if (chdanwei == "")
                    {
                        chdanwei = "";
                    }
                    //测绘者
                    string chren = table.Rows[i][35].ToString().Trim();//测绘者
                    if (chren == "")
                    {
                        chren = "";
                    }

                    //测绘联系人电话
                    string chTelephone = table.Rows[i][36].ToString().Trim();//测绘联系人电话
                    if (chTelephone == "")
                    {
                        chTelephone = "";
                    }

                    //初审意见
                    string chushenyijian = "经审查,该工程竣工档案基本齐全,建议接收进馆,提请科长审核。";

                    //初审意见人
                    string chushenyijianren = jspeople;//初审意见人
                    if (chushenyijianren == "")
                    {
                        chushenyijianren = "";
                    }
                    //初审日期
                    DateTime? chushenDate1 = jsDate2;//dr["初审日期"].ToString().Trim();

                    //备忘录
                    string remark = table.Rows[i][37].ToString().Trim(); //dr["备忘录"].ToString().Trim();
                    if (remark != "材料齐全" && remark != "材料不全")
                    {
                        return Content("<script >alert('材料是否齐全不正确！');window.history.back();</script >");
                    }

                    string isNB = "外部";
                    string status = "3";
                    string retentionPeriodNo1 = db2.RetentionPeriod.Where(ad => ad.retentionPeriodName == retentionPeriodName1).First().retentionPeriodNo;
                    string MJ1 = db2.SecurityClassification.Where(ad => ad.securityName == MJ).First().securityID;
                    var project = from ad in db.vw_gxprojectList
                                  select ad.projectID;
                    long num = project.Max() + 1;
                    string strSql1 = string.Format("insert into UrbanCon.dbo.gxPaperArchives(submitPerson,mobilephoneSubmitPerson,telphoneSubmitPerson,buildingArea,height,Material,layerCount,prevClassNo,dateReceived,recipient,projectProfile,collationRequirement,csyj,csyjPerson,csyjDate,paperProjectSeqNo,projectID,projectNo) values(" + SendPeople + "','" + Telephone + "','" + FixTelephone + "'," + gxlength1 + ",'" + gxr1 + "','" + gxmeteria + "','" + gxmaishen1 + "','" + Nfenlei + "','" + jsDate2 + "','" + jspeople + "','" + xmgaikuang + "','" + zlyaoqiu + "','" + chushenyijian + "','" + chushenyijianren + "','" + chushenDate1 + "',0 " + "," + num + "," + vwprojectProfile.projectNo + ")");
                    string strSql2 = string.Format("insert into UrbanCon.dbo.gxProjectInfo(projectName,location,newLocation,securityID,dateF,dateE,retentionPeriodNo,guangpanNo,MapNo,isYD,developmentOrganization,telphoneNoDevelopment,devolonpentOrgContacter,mobilephoneNoDevelopment,jianliUnit,jianliUnitContacter,telphoneNoJianliUnit,constructionOrganization,constructionOrgContacter,telphoneNoConstruction,disignOrganization,designOrgaContacter,telphoneNoDesignOrga,MapOrginisation,Mapper,TeleNoMap,memo,isNB,status,projectID) values('" + ProjectName + "','" + ProjectLocation + "','" + NewProjectLocation + "','" + MJ1 + "','" + kgDate + "','" + jgDate + "','" + retentionPeriodNo1 + "','" + gpNo + "','" + chcode + "'," + yd1 + ",'" + developmentOrganization1 + "','" + jianshefzpeople + "','" + jianshelxpeople + "','" + jiansheTelephone + "','" + jlOrganization + "','" + jlfzpeople + "','" + telphoneNoJianliUnit + "','" + constructionOrganization1 + "','" + fapeople + "','" + jishupeople + "','" + disignOrganization1 + "','" + sjfzpeople + "','" + telphoneNoDesignOrga + "','" + chdanwei + "','" + chren + "','" + chTelephone + "','" + remark + "','" + isNB + "','" + status + "'," + num + ")");
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
                return Content("<script >alert('导入成功！');window.history.back();</script >");
                //return RedirectToAction("gxCreate");
            }
            return View(vwprojectProfile);


        }
       public string GetCurpro(string year,string classid)
        {
            string txtCurMaxProNo = "";
      
                var curproNo = from a in db.gxPaperArchives
                               where a.projectNo.ToString().Contains(year)
                               where a.classifyID.Trim()==classid
                               orderby a.paperProjectSeqNo
                               select a;
                if(curproNo.Count()!=0)
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
                        for (int i = 0; i < 8 - len; i++)
                        {
                            year += "0";
                        }
                        //year += "1";
                        txtCurMaxProNo = year;
                    }
                    
                }

       
            return txtCurMaxProNo;
        }
        // GET: LuruProject/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var vwproject = from a in db.vw_gxprojectProfile
                            where a.projectID == id
                            select a;
            ViewBag.retentionPeriodNo = new SelectList(db1.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", vwproject.First().retentionPeriodNo);
            ViewBag.securityID = new SelectList(db1.SecurityClassification, "securityID", "securityName", vwproject.First().securityID);
            var users = from ad in ab.AspNetUsers
                        where ad.DepartmentName == User.Identity.Name
                        select ad;
            ViewBag.recipient = new SelectList(users, "UserName", "UserName", vwproject.First().recipient);

            ViewBag.status = new SelectList(db1.ArchivesStatus, "status", "statusName", vwproject.First().status);
            if (vwproject.Count() == 0)
            {
                return HttpNotFound();
            }
            //ViewBag.status = new SelectList(db.ArchivesStatus, "status", "statusName", projectProfile.status);
            return View(vwproject.First());
        }

        // POST: LuruProject/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "projectName,location,newLocation,projectID,submitPerson,mobilephoneSubmitPerson,telphoneSubmitPerson,paperProjectSeqNo,projectNo,buildingArea,securityID,retentionPeriodNo,isYD,status,developmentOrganization,devolonpentOrgContacter,telphoneNoDevelopment,mobilephoneNoDevelopment,jianliUnit,jianliUnitContacter,telphoneNoJianliUnit,constructionOrganization,constructionOrgContacter,telphoneNoConstruction,disignOrganization,designOrgaContacter,telphoneNoDesignOrga,dateReceived，recipient，projectProfile，collationRequirement，csyj，csyjPerson，csyjDate，memo")] vw_gxprojectProfile vmprojectProfile, string action)
        {
            var projects = from ac in db.gxProjectInfo
                           where ac.projectID == vmprojectProfile.projectID
                           select ac;
            gxProjectInfo project = projects.First();
            var paperArchives = from ad in db.gxPaperArchives
                                where ad.projectID == vmprojectProfile.projectID
                                select ad;
            gxPaperArchives paperArchive = paperArchives.First();
            if (action == "修改")
            {
                //更新ProjectInfo表
                project.projectName = vmprojectProfile.projectName;
                project.location = vmprojectProfile.location;
                project.devolonpentOrgContacter = vmprojectProfile.devolonpentOrgContacter;
                project.telphoneNoDevelopment = vmprojectProfile.telphoneNoDevelopment;
                project.mobilephoneNoDevelopment = vmprojectProfile.mobilephoneNoDevelopment;
                project.securityID = vmprojectProfile.securityID;
                project.retentionPeriodNo = vmprojectProfile.retentionPeriodNo;
                project.isYD = vmprojectProfile.isYD;
                project.status = vmprojectProfile.status;
                project.developmentOrganization = vmprojectProfile.developmentOrganization;
                project.newLocation = vmprojectProfile.newLocation;
                project.jianliUnit = vmprojectProfile.jianliUnit;
                project.jianliUnitContacter = vmprojectProfile.jianliUnitContacter;
                project.telphoneNoJianliUnit = vmprojectProfile.telphoneNoJianliUnit;
                project.constructionOrganization = vmprojectProfile.constructionOrganization;
                project.constructionOrgContacter = vmprojectProfile.constructionOrgContacter;
                project.telphoneNoConstruction = vmprojectProfile.telphoneNoConstruction;
                project.disignOrganization = vmprojectProfile.disignOrganization;
                project.designOrgaContacter = vmprojectProfile.designOrgaContacter;
                project.telphoneNoDesignOrga = vmprojectProfile.telphoneNoDesignOrga;
                project.memo = vmprojectProfile.memo;
                //更新PaperArchive表
                paperArchive.paperProjectSeqNo = vmprojectProfile.paperProjectSeqNo;
                paperArchive.projectNo = vmprojectProfile.projectNo;
                paperArchive.buildingArea = vmprojectProfile.buildingArea;
                paperArchive.submitPerson = vmprojectProfile.submitPerson;
                paperArchive.dateReceived = vmprojectProfile.dateReceived;
                paperArchive.recipient = vmprojectProfile.recipient;
                paperArchive.projectProfile = vmprojectProfile.projectProfile;
                paperArchive.collationRequirement = vmprojectProfile.collationRequirement;
                paperArchive.csyj = vmprojectProfile.csyj;
                paperArchive.csyjDate = vmprojectProfile.csyjDate;
                paperArchive.csyjPerson = vmprojectProfile.csyjPerson;
                paperArchive.mobilephoneSubmitPerson = vmprojectProfile.mobilephoneSubmitPerson;
                paperArchive.telphoneSubmitPerson = vmprojectProfile.telphoneSubmitPerson;
                if (ModelState.IsValid)
                {
                    db.Entry(project).State = EntityState.Modified;
                    db.Entry(paperArchive).State = EntityState.Modified;

                    db.SaveChanges();
                    return Content("<script >alert('修改成功！');window.location.href='/LuruProject/ManagementPrint';</script >");
                }
            }
            if (action == "删除")
            {

                db.gxProjectInfo.Remove(project);
                db.gxPaperArchives.Remove(paperArchive);
                db.SaveChanges();
                return Content("<script >alert('已成功删除！');window.location.href='/LuruProject/ManagementPrint';</script >");
            }
            if (action == "返回")
            {
                return RedirectToAction("ManagementPrint", "LuruProject");
            }

            return View(vmprojectProfile);
        }

        // GET: LuruProject/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LuruProject/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public string GetMaxId2()
        {
            string str = DateTime.Now.Year.ToString();
            int curno = 0;
            var curproNo = from a in db.gxPaperArchives
                           where a.projectNo.ToString().Contains(str)
                           orderby a.projectNo descending
                           select a.projectNo;

           
           
            if (curproNo.Count()==0)
            {
                str = DateTime.Now.Year.ToString() + "0001";
                
                return str;
            }
            else
            {
                curno =Convert.ToInt32(curproNo.First())+1;
                return curno.ToString();
            }
        }
    }
}
