using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using urban_archive.Models;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Text;
using System.Data.Entity.Validation;
using ICSharpCode.SharpZipLib.Zip;

namespace urban_archive.Controllers.GuanXianArchives
{
    public class ProjectManagementController : Controller
    {
        private gxArchivesContainer db = new gxArchivesContainer();
        private UrbanConEntities db1 = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();

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

        public ActionResult ZTree()
        {
            return View();
        }
        public ActionResult chushenyijianNo(long? id,string action)
        {
            if (action == "确认编号并打印预验收意见书")
            {
                string no = Request.Form["yuyanshgouNo"];
                gxProjectInfo gxprojectinfo = db.gxProjectInfo.Where(a => a.projectID == id).First();
                gxprojectinfo.yuyanshouNo = no;
                db.Entry(gxprojectinfo).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("chushenyijian", new { id = id });
            }
            return View();
        }
        public ActionResult hegezhengNo(long? id, string action)
        {
            if (action == "确认编号并打印合格证")
            {
                string no = Request.Form["hegezheng"];
                gxProjectInfo gxprojectinfo = db.gxProjectInfo.Where(a => a.projectID == id).First();
                gxprojectinfo.hegezhengNo = no;
                db.Entry(gxprojectinfo).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("hegezhengming", new { id = id });
            }
            return View();
        }
        public string GetCurpro(string year, string classid)
        {
            string txtCurMaxProNo = "";
          
                var curproNo = from a in db.gxPaperArchives
                               where a.projectNo.ToString().Contains(year)
                               where a.classifyID == classid
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

            
            return txtCurMaxProNo;
        }
        public string GetMaxId2()
        {//modify by zhoulin,date:20170523
            string str = DateTime.Now.Year.ToString();



            var model = from a in db.gxPaperArchives
                        where a.projectNo.ToString().Contains(str)
                        orderby a.projectNo descending
                        select a;
            string obj = (model.First().projectNo + 1).ToString();
            if (obj == null || obj.ToString().Length != 8)
            {
                str = DateTime.Now.Year.ToString() + "0001";
                return str;
            }
            else
            {
                return obj.ToString();
            }
        }
        public ActionResult CreateFromProjectManag(long? id)
        {
            var project = from a in db.vw_gxprojectProfile
                          where a.projectID == id
                          select a;
            ViewBag.classifyID = new SelectList(db.gxClassType, "classTypeID", "classTypeName",project.First().classifyID);
            string m = project.First().classifyID;
            ViewBag.classid = project.First().classifyID;
            var proNo = from b in db.vw_gxprojectProfile
                        where b.classifyID==m
                        orderby b.projectNo descending
                        select b.projectNo;
            vw_gxprojectProfile vmprojectPfofile = new vw_gxprojectProfile();
            vmprojectPfofile = project.First();
            long CurMaxproN = Convert.ToInt32(proNo.First());
            vmprojectPfofile.projectNo = CurMaxproN + 1;
           
               
                vmprojectPfofile.csyjDate = DateTime.Now.Date;
                vmprojectPfofile.dateReceived = DateTime.Now.Date;
                
                //初始化相关控件
                ViewData["ClassNo"] = vmprojectPfofile.prevClassNo;
                ViewBag.retentionPeriodNo = new SelectList(db1.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", vmprojectPfofile.retentionPeriodNo);
                ViewBag.securityID = new SelectList(db1.SecurityClassification, "securityID", "securityName", vmprojectPfofile.securityID);
                var users = from ad in ab.AspNetUsers
                            where ad.DepartmentId == 2 || ad.DepartmentId == 4
                            select ad;
                string user = User.Identity.Name;
                ViewBag.recipient = new SelectList(users, "UserName", "UserName", user);
               List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

                };
           
              if (vmprojectPfofile.isYD == null|| vmprojectPfofile.isYD==false)
              {
                  ViewBag.isYD = new SelectList(list, "Value", "Text",0);

               }
              else
             {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 1);
             }
                if (vmprojectPfofile.memo == "材料齐全")
                {
                    ViewBag.radiobutton = 1;
                }
                else
                {
                    ViewBag.radiobutton = 2;
                }
                ViewData["new"] = true;
            ViewBag.prevclassno = vmprojectPfofile.prevClassNo;
            if (vmprojectPfofile.dateE.ToString() == "")
            {
                ViewBag.datee = "";
            }
            else {
                ViewBag.datee = vmprojectPfofile.dateE.Value.ToString("yyyy-MM-dd");
            }
            if (vmprojectPfofile.dateF.ToString() == "")
            {
                ViewBag.datef = "";
            }
            else
            {
                ViewBag.datef = vmprojectPfofile.dateF.Value.ToString("yyyy-MM-dd");
            }
            
            return View(vmprojectPfofile);

        }
        [HttpPost]
        public ActionResult CreateFromProjectManag(string projectName, string projectID, string projectNo, string location, string contractNo, string newLocation, string submitPerson, string projectProfile, string mobilephoneSubmitPerson, string telphoneSubmitPerson, string securityID, string constructionOrganization, long? buildingArea, string prevClassNo, string retentionPeriodNo, string dateReceived, string recipient, string  isYD, string collationRequirement, string developmentOrganization, string devolonpentOrgContacter, string mobilephoneNoDevelopment, string telphoneNoDevelopment, string jianliUnit, string jianliUnitContacter, string telphoneNoJianliUnit, string constructionOrgContacter, string telphoneNoConstruction, string disignOrganization, string designOrgaContacter, string telphoneNoDesignOrga, string csyjPerson, string csyjDate, string memo, string seqNo,string Material,string layerCount, string action, string radiobutton, string csyj,string classifyID,string MapOrginisation,string Mapper,string TeleNoMap,string height,string MapNo, string guangpanNo,DateTime? dateF, DateTime? dateE)
        {
           
            if (action == "返回")
            {
                return RedirectToAction("gxIndex");
            }
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
                if (csyj == "" || csyj == null)
                {
                    return Content("<script >alert('初审意见不能为空！');window.history.back();</script >");

                }
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
                if (ModelState.IsValid)
                {

                    string file = Request.Form["MyUploadile"];
                    string filename = Request.Form["name1"];
                    var files = Request.Files;
                    var classid = Request.Form["classid"];
                    if (filename != "")
                    {
                        if (files.Count > 0)
                        {
                            var file1 = files[0];
                            string strFileSavePath = Request.MapPath("~/files/guanxianWord");//文件存储路径
                            file1.SaveAs(strFileSavePath + "/" + filename);
                        }
                    }

                    try
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            //添加ProjectInfo表
                            gxProjectInfo projectInfo = new gxProjectInfo();
                            var maxprojectId = from a in db.gxProjectInfo
                                               orderby a.projectID descending
                                               select a;
                            projectInfo.projectID = maxprojectId.First().projectID + 1;//工程信息表的projectID与工程目录表projectID为一对一关系
                            projectInfo.contractNo = "";
                            projectInfo.projectName = projectName;
                            projectInfo.location = location;
                            projectInfo.newLocation = newLocation;
                            projectInfo.securityID = securityID;
                            projectInfo.retentionPeriodNo = retentionPeriodNo;

                            projectInfo.developmentOrganization = developmentOrganization;
                            projectInfo.mobilephoneNoDevelopment = mobilephoneNoDevelopment;
                            projectInfo.telphoneNoDevelopment = telphoneNoDevelopment;
                            projectInfo.devolonpentOrgContacter = devolonpentOrgContacter;
                            projectInfo.constructionOrganization = constructionOrganization;//施工单位
                            projectInfo.telphoneNoConstruction = telphoneNoConstruction;//施工单位技术人员
                            projectInfo.constructionOrgContacter = constructionOrgContacter;//施工单位法人
                            projectInfo.mobilephoneNoConstruction = "";//没用了
                            projectInfo.disignOrganization = disignOrganization;
                            projectInfo.designOrgaContacter = designOrgaContacter;
                            projectInfo.telphoneNoDesignOrga = telphoneNoDesignOrga;
                            projectInfo.jianliUnit = jianliUnit;
                            projectInfo.jianliUnitContacter = jianliUnitContacter;
                            projectInfo.telphoneNoJianliUnit = telphoneNoJianliUnit;
                            projectInfo.mobilephoneNoJianliUnit = "";
                            projectInfo.MapOrginisation = MapOrginisation;
                            projectInfo.Mapper = Mapper;
                            projectInfo.TeleNoMap = TeleNoMap;
                            projectInfo.MapNo = MapNo;
                            projectInfo.guangpanNo = guangpanNo;
                            projectInfo.dateE = dateE;
                            projectInfo.dateF = dateF;

                            if (isYD.Trim() == "是")
                            {
                                projectInfo.isYD = true;

                            }
                            else
                            {
                                projectInfo.isYD = false;
                            }
                            projectInfo.memo = memo;

                            projectInfo.structureTypeID = "1";
                            projectInfo.status = "3";
                            projectInfo.classifyID = classifyID;
                            //添加PaperArchive表
                            gxPaperArchives paperArchive = new gxPaperArchives();
                            //model1.paperProjectSeqNo = proSeqNuber;
                            if (height == "")
                            {
                                paperArchive.height = "0";
                            }
                            else
                            {
                                //paperArchive.height = double.Parse(height);
                                paperArchive.height = height;
                            }

                            paperArchive.projectID = long.Parse(projectID);
                            paperArchive.projectNo = long.Parse(projectNo);
                            paperArchive.submitPerson = submitPerson;
                            paperArchive.mobilephoneSubmitPerson = mobilephoneSubmitPerson;
                            paperArchive.telphoneSubmitPerson = telphoneSubmitPerson;
                            paperArchive.buildingArea = buildingArea;
                            paperArchive.Material = Material;
                            paperArchive.layerCount = layerCount;
                            paperArchive.prevClassNo = prevClassNo;
                            paperArchive.recipient = recipient;
                            paperArchive.projectProfile = projectProfile;
                            paperArchive.collationRequirement = collationRequirement;
                            paperArchive.csyjPerson = csyjPerson;

                            if (csyjDate.ToString() != "")
                            {
                                paperArchive.csyjDate = DateTime.Parse(csyjDate);
                            }
                            else
                            {
                                paperArchive.csyjDate = DateTime.Now.Date;
                            }
                            if (dateReceived.ToString() != "")
                            {
                                paperArchive.dateReceived = DateTime.Parse(dateReceived);
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
                            paperArchive.classifyID = classifyID;
                            if (i == 0)
                            {
                                projectInfo.projectName = projectName;
                                projectInfo.isNB = "外部";
                                projectInfo.projectID = maxprojectId.First().projectID + 1;//工程信息表的projectID与工程目录表projectID为一对一关系
                                paperArchive.projectID = maxprojectId.First().projectID + 1;
                            }
                            if (i == 1)
                            {
                                projectInfo.projectName = projectName + "竣工测量文件";
                                projectInfo.isNB = "内部";
                                projectInfo.projectID = maxprojectId.First().projectID + 2;//工程信息表的projectID与工程目录表projectID为一对一关系
                                paperArchive.projectID = maxprojectId.First().projectID + 2;
                            }
                            db.gxPaperArchives.Add(paperArchive);
                            db.gxProjectInfo.Add(projectInfo);
                        }
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException dbex) { }
                    ViewData["tijiao"] = true;
                    ViewData["new"] = false;
                    return Content("<script >alert('保存成功！');window.location.href='../ProjectManagement/gxIndex';</script >");

                }
                else {
                    var msg = string.Empty;
                    foreach (var value in ModelState.Values)
                    {
                        if (value.Errors.Count > 0)
                        {
                            foreach (var error in value.Errors)
                            {
                                msg = msg + error.ErrorMessage;
                            }
                        }
                    }
                    var msg1 = msg;
                }
                
            }
            return View();
        }
        public ActionResult gxIndexSZ()
        {
            ViewData["pagename"] = "ProjectInfoes-ManagementPrint";
            //ViewBag.classifyID = new SelectList(db.gxClassType, "classTypeID", "classTypeName");//管线分类
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                 
                new SelectListItem { Text = "建设单位", Value = "1"},
                new SelectListItem { Text = "工程地点", Value = "2" },
                new SelectListItem { Text = "工程序号", Value = "3" },
                new SelectListItem { Text = "施工单位", Value = "4" },
                new SelectListItem { Text = "设计单位", Value = "5" },
                new SelectListItem { Text = "监理单位", Value = "6" },
                new SelectListItem { Text = "项目顺序号", Value = "7" },
                 new SelectListItem { Text = "接收日期", Value = "8" },
                 new SelectListItem { Text = "测绘单位", Value = "9" },
                  new SelectListItem { Text = "预验收意见书编号", Value = "10" },
                   new SelectListItem { Text = "合格证编号", Value = "11" },

            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            var vwprojectFile = from ad in db.vw_gxprojectList
                                where ad.isNB == "外部"
                                where ad.classifyID == "2"
                                select ad;



            // 默认按责任书编号排
            vwprojectFile = vwprojectFile.OrderByDescending(s => s.projectNo);
            ViewBag.result1 = JsonConvert.SerializeObject(vwprojectFile);
            return View();
        }
        // GET: ProjectManagement
        public ActionResult gxIndex()
        {
            ViewData["pagename"] = "ProjectInfoes-ManagementPrint";
            //ViewBag.classifyID = new SelectList(db.gxClassType, "classTypeID", "classTypeName");//管线分类
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                 
                new SelectListItem { Text = "建设单位", Value = "1"},
                new SelectListItem { Text = "工程地点", Value = "2" },
                new SelectListItem { Text = "工程序号", Value = "3" },
                new SelectListItem { Text = "施工单位", Value = "4" },
                new SelectListItem { Text = "设计单位", Value = "5" },
                new SelectListItem { Text = "监理单位", Value = "6" },
                new SelectListItem { Text = "项目顺序号", Value = "7" },
                 new SelectListItem { Text = "接收日期", Value = "8" },
                 new SelectListItem { Text = "测绘单位", Value = "9" },
                  new SelectListItem { Text = "预验收意见书编号", Value = "10" },
                   new SelectListItem { Text = "合格证编号", Value = "11" },

            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            var vwprojectFile = from ad in db.vw_gxprojectList
                                where ad.isNB == "外部"
                                where ad.classifyID=="1"
                                select ad;


           
            // 默认按责任书编号排
            vwprojectFile = vwprojectFile.OrderByDescending(s => s.projectNo);
            ViewBag.result1 = JsonConvert.SerializeObject(vwprojectFile);
            return View();
        }
        public string Find(int? SelectedID,string SearchString,string classID)
        {
            string classtype = classID.Trim();
            var vwprojectFile = from ad in db.vw_gxprojectList
                                where ad.classifyID==classtype
                                where ad.isNB=="外部"
                                select ad;
            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (t)
                {

                    case 0:
                        vwprojectFile = vwprojectFile.Where(ad => ad.projectName.Contains(SearchString));//根据责任书编号搜索
                        break;
                    case 1:
                        vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 2:
                        vwprojectFile = vwprojectFile.Where(ad => ad.location.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        //int prono = int.Parse();
                        vwprojectFile = vwprojectFile.Where(ad => ad.projectNo.ToString().Contains(SearchString.Trim()));//根据工程地点
                        break;
                    case 4:

                        vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization.Contains(SearchString)); //根据工程序号
                        break;
                    case 5:
                        vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization.Contains(SearchString));//根据施工单位
                        break;
                    case 6:
                        vwprojectFile = vwprojectFile.Where(ad => ad.jianliUnit.Contains(SearchString));//根据设计单位
                        break;

                    case 7:
                        //long seq = Convert.ToInt32();
                        vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo.ToString().Contains(SearchString.Trim()));//根据项目顺序号
                        break;
                    case 8:
                        //long seq = Convert.ToInt32();
                        vwprojectFile = vwprojectFile.Where(ad => ad.dateReceived.ToString().Contains(SearchString.Trim()));//根据项目顺序号
                        break;
                    case 9:
                        //long seq = Convert.ToInt32();
                        vwprojectFile = vwprojectFile.Where(ad => ad.MapOrginisation.ToString().Contains(SearchString.Trim()));//根据项目顺序号
                        break;
                    case 10:
                        //long seq = Convert.ToInt32();
                        vwprojectFile = vwprojectFile.Where(ad => ad.yuyanshouNo.ToString().Contains(SearchString.Trim()));//根据项目顺序号
                        break;
                    case 11:
                        //long seq = Convert.ToInt32();
                        vwprojectFile = vwprojectFile.Where(ad => ad.hegezhengNo.ToString().Contains(SearchString.Trim()));//根据项目顺序号
                        break;
                }

            }
            vwprojectFile = vwprojectFile.OrderByDescending(s => s.projectID);
            return JsonConvert.SerializeObject(vwprojectFile);
        }
        public void DownLoadFile(string id)     //文件下载
        {
            //if (string.IsNullOrEmpty(id))
            //{
            //    throw new ArgumentNullException("fileId is errror");
            //}
            int Id = Convert.ToInt32(id);
            var findFile = db.gxProjectInfo.Find(Id);
            if (findFile.storagePath == null)
            {
                Response.Write("<script >alert('该工程未上传文件！');window.history.back();</script>");
            }
            else
            {
                var idd = long.Parse(id);
                var d = from f in db.gxPaperArchives
                        where f.projectID == idd
                        select f;
                gxPaperArchives gxPaperArchives = d.First();
                var n1 = gxPaperArchives.projectNo;
                string no = AppDomain.CurrentDomain.BaseDirectory + "files\\guanxianWord\\" + n1;
                if (System.IO.Directory.Exists(no)) {
                    MemoryStream ms = null;
                    Response.ContentType = "application/octet-stream";
                    var strFileName = gxPaperArchives.projectNo;
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + gxPaperArchives.projectNo + ".zip");
                    ms = new MemoryStream();
                    zos = new ZipOutputStream(ms);
                    addZipEntry(no);
                    zos.Finish();
                    zos.Close();
                    Response.Clear();
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                }
            }

        }
        // GET: ProjectManagement/Details/5
        static ZipOutputStream zos = null;
        public ActionResult gxDetails(string  id,string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            int ID = int.Parse(id.Trim());
            var test = from ad in db.vw_gxprojectList
                       where (ad.projectID ==ID)
                       select ad;
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false"},
                new SelectListItem { Text = "是", Value = "true"},
            };

            ViewBag.yidi = new SelectList(list, "Value", "Text", test.First().isYD);
            vw_gxprojectList projectProfile = test.First();
            string a = projectProfile.status, b = projectProfile.securityID, e = projectProfile.retentionPeriodNo;
            if (a != null && a != "")
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
                        projectProfile.securityID = "秘密";
                        break;
                    case "2":
                        projectProfile.securityID = "机密";
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
            if (e != null && e != "")
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
            if (test.Count()==0)
            {
                return Content("<script >alert('该工程无详细信息！');window.histroy.back();</script >");
            }
            
            if (action == "文件下载")
            {

                return RedirectToAction("DownLoadFile", "ProjectManagement",new {id=id });
            }
            if (action == "返回")
            {
                
                return RedirectToAction("gxIndex", "ProjectManagement");
            }
            if (action == "删除")
            {
                var projectInfo = db.gxProjectInfo.Where(a1 => a1.projectID >= ID).Where(a1 => a1.projectID <= ID + 1).ToList();
                var d = from b1 in db.gxPaperArchives
                        where b1.projectID == ID
                        select b1;

                gxPaperArchives gxPaperArchives = d.First();
                var n1 = gxPaperArchives.projectNo;
                string no = AppDomain.CurrentDomain.BaseDirectory + "files\\guanxianWord\\" + n1;
                if (System.IO.Directory.Exists(no))
                {
                    //删除文件夹
                    Directory.Delete(no, true);
                }

                var paperArchive = db.gxPaperArchives.Where(a1 => a1.projectID >= ID).Where(a1 => a1.projectID <= ID + 1).ToList();
                for (int i = 0; i < projectInfo.Count(); i++)
                {
                    db.gxProjectInfo.Remove(projectInfo[i]);
                }
                for (int i = 0; i < paperArchive.Count(); i++)
                {
                    db.gxPaperArchives.Remove(paperArchive[i]);
                }
                db.SaveChanges();

                return Content("<script >alert('删除成功！');window.location.href='/ProjectManagement/gxIndex';</script >");
            }

            return View(projectProfile);
        }

        // GET: ProjectManagement/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProjectManagement/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ProjectManagement/Edit/5
        public ActionResult gxEdit(int ?id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "true"},
                new SelectListItem { Text = "否", Value = "false"},

            };
            ViewBag.isYD = new SelectList(list, "Value", "Text");
            var vwproject = from a in db.vw_gxprojectList
                            where a.projectID == id
                            select a;
            ViewBag.retentionPeriodNo = new SelectList(db1.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", vwproject.First().retentionPeriodNo);
            ViewBag.securityID = new SelectList(db1.SecurityClassification, "securityID", "securityName", vwproject.First().securityID);
            ViewBag.isYD = new SelectList(list, "Value", "Text", vwproject.First().isYD);
            var users = from ad in ab.AspNetUsers
                            //where ad.DepartmentName == User.Identity.Name
                        where ad.DepartmentId == 2 || ad.DepartmentId == 4
                        select ad;
            ViewBag.recipient = new SelectList(users, "UserName", "UserName", vwproject.First().recipient);

            ViewBag.status = new SelectList(db1.ArchivesStatus, "status", "statusName", vwproject.First().status);
            ViewBag.filename = vwproject.First().storagePath;
            if (vwproject.First().classifyID.Trim()=="1")
            {
                ViewBag.CLASS = "建筑工程地下管线工程";
                    }
            if (vwproject.First().classifyID.Trim() == "2")
            {
                ViewBag.CLASS = "市政地下管线工程";
            }
            if (vwproject.Count() == 0)
            {
                return HttpNotFound();
            }
            //ViewBag.status = new SelectList(db.ArchivesStatus, "status", "statusName", projectProfile.status);
            return View(vwproject.First());
        }

        // POST: ProjectManagement/Edit/5
        [HttpPost]
        public ActionResult gxEdit(string MapNo, string guangpanNo, string MapOrginisation, string Mapper, string TeleNoMap, string projectName,string location,string newLocation,string projectID,string submitPerson,string mobilephoneSubmitPerson,string telphoneSubmitPerson,string paperProjectSeqNo,string projectNo,string buildingArea,string securityID,string retentionPeriodNo,string  isYD,string status,string developmentOrganization,string devolonpentOrgContacter,string telphoneNoDevelopment,string mobilephoneNoDevelopment,string jianliUnit, string jianliUnitContacter, string telphoneNoJianliUnit, string constructionOrganization, string constructionOrgContacter, string telphoneNoConstruction, string disignOrganization, string designOrgaContacter, string telphoneNoDesignOrga, string dateReceived,string recipient,string projectProfile,string collationRequirement, string csyj,string csyjPerson,string csyjDate,string memo, string height, string Material, string layerCount, string action, string coordinate)
        {
            if(projectID==""|| projectID==null)
            {
                return Content("<script >alert('工程ID不能为空，请核查！');window.location.href='/ProjectManagement/gxIndex';</script >");
            }
            int ID = int.Parse(projectID.Trim());
            //var project1 = from ac in db.gxProjectInfo
            //               where ac.projectID>=ID
            //               where ac.projectID<=ID+1
            //               select ac;
            var project = db.gxProjectInfo.Where(a => a.projectID == ID).ToList();
            string CID = project[0].classifyID.Substring(0,1);
            //gxProjectInfo project = projects.ToList();
            //var paperArchives = from ad in db.gxPaperArchives
            //                    where ad.projectID ==ID
            //                    select ad;
            //gxPaperArchives paperArchive = paperArchives.First();
            int SeqNo = int.Parse(paperProjectSeqNo.Trim());
            var paperArchive1 = db.gxPaperArchives.Where(a => a.projectID == ID).ToList();

            if (action == "修改")
            {
                if (projectNo == null || projectNo == "")
                {
                    return Content("<script >alert('工程序号不能为空，请核查！');window.history.back();</script >");
                }
                if (projectName == null || projectName == "")
                {
                    return Content("<script >alert('工程名称不能为空，请核查！');window.history.back();</script >");
                }


                if (retentionPeriodNo == null || retentionPeriodNo == "")
                {
                    return Content("<script >alert('保存期限不能为空！');window.history.back();</script >");
                }
                if (securityID == null || securityID == "")
                {
                    return Content("<script >alert('档案密级不能为空！');window.history.back();</script >");
                }
                if (csyj == null || csyj == "")
                {
                    return Content("<script >alert('初审意见不能为空！');window.history.back();</script >");
                }
                if (csyjDate == null || csyjDate == "")
                {
                    return Content("<script >alert('初审日期不能为空！');window.history.back();</script >");
                }

                if (ModelState.IsValid)
                {
                    string file = Request.Form["MyUploadile"];
                    string filename = Request.Form["name1"];
                    if (filename != "")
                    {
                        if (filename != project.First().storagePath)
                        {
                            var files = Request.Files;
                            if (files.Count > 0)
                            {
                                for (int i = 0; i < files.Count; i++)
                                {
                                    var file1 = files[i];
                                    var pid = long.Parse(projectID);
                                    var n = from a in db.gxPaperArchives
                                            where a.projectID == pid
                                            select a;
                                    var no = n.First().projectNo;
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
                    }
                    for (int i = 0; i < project.Count(); i++)
                    {
                        //更新ProjectInfo表
                        project[i].projectName = projectName;
                        project[i].location = location;
                        project[i].devolonpentOrgContacter = devolonpentOrgContacter;
                        project[i].telphoneNoDevelopment = telphoneNoDevelopment;
                        project[i].mobilephoneNoDevelopment = mobilephoneNoDevelopment;
                        project[i].securityID = securityID;
                        project[i].retentionPeriodNo = retentionPeriodNo;
                        if (isYD.Trim() == "true")
                        {
                            project[i].isYD = true;

                        }
                        else
                        {
                            project[i].isYD = false;
                        }

                        project[i].status = status;
                        project[i].developmentOrganization = developmentOrganization;
                        project[i].newLocation = newLocation;
                        project[i].jianliUnit = jianliUnit;
                        project[i].jianliUnitContacter = jianliUnitContacter;
                        project[i].telphoneNoJianliUnit = telphoneNoJianliUnit;
                        project[i].constructionOrganization = constructionOrganization;
                        project[i].constructionOrgContacter = constructionOrgContacter;
                        project[i].telphoneNoConstruction = telphoneNoConstruction;
                        project[i].disignOrganization = disignOrganization;
                        project[i].designOrgaContacter = designOrgaContacter;
                        project[i].telphoneNoDesignOrga = telphoneNoDesignOrga;
                        project[i].memo = memo;
                        project[i].MapOrginisation = MapOrginisation;
                        project[i].Mapper = Mapper;
                        project[i].TeleNoMap = TeleNoMap;

                        var pid1 = long.Parse(projectID);
                        var n1 = from a in db.gxPaperArchives
                                 where a.projectID == pid1
                                 select a;
                        var n2 = n1.First().projectNo;
                        string p1 = AppDomain.CurrentDomain.BaseDirectory + "files\\guanxianWord\\" + n2;
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

                        project[i].storagePath = filename;
                        project[i].MapNo = MapNo;
                        project[i].guangpanNo = guangpanNo;
                        db.Entry(project[i]).State = EntityState.Modified;
                    }
                    for (int i = 0; i < paperArchive1.Count(); i++)
                    {
                        //更新PaperArchive表
                        paperArchive1[i].coordinate = coordinate;
                        if (paperProjectSeqNo != null && paperProjectSeqNo != "")
                        {
                            paperArchive1[i].paperProjectSeqNo = int.Parse(paperProjectSeqNo.Trim());
                        }
                        if (projectNo != null && projectNo != "")
                        {
                            paperArchive1[i].projectNo = Convert.ToInt32(projectNo);
                        }
                        if (buildingArea != null && buildingArea != "")
                        {
                            paperArchive1[i].buildingArea = double.Parse(buildingArea.Trim());
                        }
                        else
                        {
                            paperArchive1[i].buildingArea = 0;
                        }
                        paperArchive1[i].submitPerson = submitPerson;
                        if (dateReceived != null || dateReceived != "")
                        {
                            paperArchive1[i].dateReceived = DateTime.Parse(dateReceived.Trim());
                        }

                        paperArchive1[i].recipient = recipient;
                        paperArchive1[i].projectProfile = projectProfile;
                        paperArchive1[i].collationRequirement = collationRequirement;
                        paperArchive1[i].csyj = csyj;
                        if (csyjDate != null && csyjDate != "")
                        {
                            paperArchive1[i].csyjDate = DateTime.Parse(csyjDate.Trim());
                        }
                        paperArchive1[i].csyjPerson = csyjPerson;
                        paperArchive1[i].mobilephoneSubmitPerson = mobilephoneSubmitPerson;
                        paperArchive1[i].telphoneSubmitPerson = telphoneSubmitPerson;
                        paperArchive1[i].height = height;
                        paperArchive1[i].Material = Material;
                        paperArchive1[i].layerCount = layerCount;
                        db.Entry(paperArchive1[i]).State = EntityState.Modified;
                    }
                    

                    db.SaveChanges();
                    //return Content("<script >alert('修改成功！');window.location.href='/ProjectManagement/gxIndex';</script >");
                    if (CID == "1")
                    {
                        return Content("<script >alert('修改成功！');window.location.href='/ProjectManagement/gxIndex';</script >");
                    }
                    else if (CID == "2")
                    {
                        return Content("<script >alert('修改成功！');window.location.href='/ProjectManagement/gxIndexSZ';</script >");
                    }
                    else
                    {
                        return Content("<script >alert('修改成功！');window.history.back();</script >");
                    }

                }
            }
            if (action == "删除")
            {
                var projectInfo = db.gxProjectInfo.Where(a => a.projectID >= ID).Where(a => a.projectID <= ID + 1).ToList();
                var d = from b in db.gxPaperArchives
                        where b.projectID == ID
                        select b;

                gxPaperArchives gxPaperArchives = d.First();
                var n1 = gxPaperArchives.projectNo;
                string no = AppDomain.CurrentDomain.BaseDirectory + "files\\guanxianWord\\" + n1;
                if (System.IO.Directory.Exists(no))
                {
                    //删除文件夹
                    Directory.Delete(no, true);
                }

                var paperArchive = db.gxPaperArchives.Where(a => a.projectID >= ID).Where(a => a.projectID <= ID + 1).ToList();
                for (int i = 0; i < projectInfo.Count(); i++)
                {
                    db.gxProjectInfo.Remove(projectInfo[i]);
                }
                for (int i = 0; i < paperArchive.Count(); i++)
                {
                    db.gxPaperArchives.Remove(paperArchive[i]);
                }
                db.SaveChanges();

                return Content("<script >alert('删除成功！');window.location.href='/ProjectManagement/gxIndex';</script >");
            }
            if (action == "返回")
            {
                return RedirectToAction("gxIndex", "ProjectManagement");
            }

            return View();
        }
        public ActionResult chushenyijian(int id)
        {
            LocalReport localReport = new LocalReport();
            var ds1 = db.gxPaperArchives.Where(ad => ad.projectID == id);
            var ds2 = db.vw_gxprojectList.Where(ad => ad.projectID == id);
            localReport.ReportPath = Server.MapPath("~/Report/guanxian/chushenyijian.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("gxchushenyijianDataSet", ds2);
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
            var ds1 = db.gxPaperArchives.Where(ad => ad.projectID == id);
            var ds2 = db.vw_gxprojectList.Where(ad => ad.projectID == id);
            localReport.ReportPath = Server.MapPath("~/Report/guanxian/hegezheng.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("gxhegezhengDataSet", ds2);
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
        public ActionResult projectinformation(string action)
        {
            ViewBag.classifyID = new SelectList(db.gxClassType, "classTypeID", "classTypeName");//管线分类
            string PNoS = Request.Form["seqNoStart"];
            string PNoE = Request.Form["seqNoEnd"];
            string SeqS = Request.Form["txtSeqNoS"];
            string SeqE = Request.Form["txtSeqNoE"];
            if (action == "工程信息导出（工程序号）")
            {
                var classid = Request.Form["classifyID"];
                    long n = long.Parse(PNoS);
                    long m = long.Parse(PNoE);
                    //接收需要导出的数据
                    List<vw_gxprojectList> list = db.vw_gxprojectList.Where(ad => ad.projectNo >= n).Where(ad => ad.projectNo <= m).Where(a=>a.isNB=="外部").Where(a=>a.classifyID.Trim()==classid).ToList();
                    //命名导出表格的StringBuilder变量
                    StringBuilder sHtml = new StringBuilder(string.Empty);
                    //打印表头
                    sHtml.Append("<table border=\"1\" width=\"100%\">");
                    sHtml.Append("<tr height=\"40\"><td colspan=\"41\" align=\"center\" style='font-size:24px'><b>管线工程信息" + "</b></td></tr>");
                    //打印列名
                    sHtml.Append("<tr height=\"20\" align=\"center\" ><td>预验收意见书编号</td><td>合格证编号</td><td>工程名称</td><td>工程地点</td><td>最新工程地点</td><td>报送人</td><td>手机</td><td>固话</td><td>项目序号</td><td>工程序号</td><td>档案密级</td><td>管线长度</td><td>管径</td><td>材质</td><td>埋深</td><td>保存期限</td><td>异地存放</td><td>档案状态</td><td>建设单位</td><td>建设单位联系人</td><td>固定电话</td><td>手机电话</td><td>监理单位</td><td>监理单位负责人</td><td>监理者</td><td>施工单位</td><td>施工单位法人</td><td>技术员</td><td>设计单位</td><td>设计单位负责人</td><td>设计者</td><td>测绘单位</td><td>测绘者</td><td>联系电话</td><td>档案接收日期</td><td>档案接收人</td><td>项目概况</td><td>整理要求</td><td>初审意见</td><td>初审意见人</td><td>初审意见日期</td><td>备忘录</td><td>文件</td></tr>");
                    //循环读取List集合 
                    for (int i = 0; i < list.Count; i++)
                    {
                    if (list[i].securityID == "1")
                    {
                        list[i].securityID = "秘密";
                    }
                    if (list[i].securityID == "2")
                    {
                        list[i].securityID = "机密";
                    }
                    if (list[i].securityID == "3")
                    {
                        list[i].securityID = "绝密";
                    }
                    if (list[i].securityID == "4")
                    {
                        list[i].securityID = "一般";
                    }
                    if (list[i].securityID == "5")
                    {
                        list[i].securityID = "内部";
                    }
                    if (list[i].securityID == "6")
                    {
                        list[i].securityID = "公开/内部";
                    }
                    if (list[i].retentionPeriodNo == "1")
                    {
                        list[i].retentionPeriodNo = "长期";
                    }
                    if (list[i].retentionPeriodNo == "2")
                    {
                        list[i].retentionPeriodNo = "永久";
                    }
                    if (list[i].retentionPeriodNo == "3")
                    {
                        list[i].retentionPeriodNo = "短期";
                    }
                    string yd="";
                    if (list[i].isYD == true)
                    {
                        yd = "是";
                    }
                    if (list[i].isYD == false)
                    {
                        yd = "否";
                    }
                    sHtml.Append("<tr height=\"20\" align=\"left\"><td>" + list[i].yuyanshouNo + "</td><td>" + list[i].hegezhengNo+ "</td><td>" + list[i].projectName
                            + "</td><td>" + list[i].location + "</td><td>" + list[i].newLocation + "</td><td>" + list[i].submitPerson
                            + "</td><td>" + list[i].mobilephoneSubmitPerson + "</td><td>" + list[i].telphoneSubmitPerson + "</td><td>" + list[i].paperProjectSeqNo
                            + "</td><td>" + list[i].projectNo + "</td><td>"+ list[i].securityID + "</td><td>"+ list[i].buildingArea + "</td><td>"+ list[i].height
                            + "</td><td>"+ list[i].Material + "</td><td>"+ list[i].layerCount + "</td><td>"+ list[i].retentionPeriodNo + "</td><td>"+ yd 
                            + "</td><td>"+ list[i].statusName + "</td><td>"+ list[i].developmentOrganization + "</td><td>"+ list[i].devolonpentOrgContacter 
                            + "</td><td>" + list[i].telphoneNoDevelopment + "</td><td>" + list[i].mobilephoneNoDevelopment + "</td><td>" + list[i].jianliUnit
                            + "</td><td>" + list[i].jianliUnitContacter + "</td><td>" + list[i].telphoneNoJianliUnit + "</td><td>" + list[i].constructionOrganization 
                            + "</td><td>" + list[i].constructionOrgContacter + "</td><td>" + list[i].telphoneNoConstruction
                            + "</td><td>" + list[i].disignOrganization + "</td><td>" +list[i].designOrgaContacter
                            +"</td><td>" + list[i].telphoneNoDesignOrga + "</td><td>" + list[i].MapOrginisation + "</td><td>" + list[i].Mapper 
                            + "</td><td>" + list[i].TeleNoMap + "</td><td>" + list[i].dateReceived + "</td><td>" + list[i].recipient 
                            + "</td><td>" + list[i].projectProfile + "</td><td>" + list[i].collationRequirement + "</td><td>" + list[i].csyj 
                            + "</td><td>" + list[i].csyjPerson + "</td><td>" + list[i].csyjDate + "</td><td>" + list[i].memo + "</td><td>" + list[i].storagePath 
                            + "</td></tr>");
                    }
                    //打印表尾
                    sHtml.Append("</table>");
                    //调用输出Excel表的方法
                    ExportToExcel("application/ms-excel", "管线工程信息目录.xls", sHtml.ToString());
                }
            if (action == "工程信息导出（项目顺序号）")
            {
                var classid = Request.Form["classifyID"];
                long n = long.Parse(SeqS);
                long m = long.Parse(SeqE);
                //接收需要导出的数据
                List<vw_gxprojectList> list = db.vw_gxprojectList.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.paperProjectSeqNo <= m).Where(a => a.isNB == "外部").Where(a => a.classifyID.Trim() == classid).ToList();
                //命名导出表格的StringBuilder变量
                StringBuilder sHtml = new StringBuilder(string.Empty);
                //打印表头
                sHtml.Append("<table border=\"1\" width=\"100%\">");
                sHtml.Append("<tr height=\"40\"><td colspan=\"41\" align=\"center\" style='font-size:24px'><b>管线工程信息" + "</b></td></tr>");
                //打印列名
                sHtml.Append("<tr height=\"20\" align=\"center\" ><td>工程名称</td><td>工程地点</td><td>最新工程地点</td><td>报送人</td><td>手机</td><td>固话</td><td>项目序号</td><td>工程序号</td><td>档案密级</td><td>管线长度</td><td>管径</td><td>材质</td><td>埋深</td><td>保存期限</td><td>异地存放</td><td>档案状态</td><td>建设单位</td><td>建设单位联系人</td><td>固定电话</td><td>手机电话</td><td>监理单位</td><td>监理单位负责人</td><td>监理者</td><td>施工单位</td><td>施工单位法人</td><td>技术员</td><td>设计单位</td><td>设计单位负责人</td><td>设计者</td><td>测绘单位</td><td>测绘者</td><td>联系电话</td><td>档案接收日期</td><td>档案接收人</td><td>项目概况</td><td>整理要求</td><td>初审意见</td><td>初审意见人</td><td>初审意见日期</td><td>备忘录</td><td>文件</td></tr>");
                //循环读取List集合 
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].securityID == "1")
                    {
                        list[i].securityID = "秘密";
                    }
                    if (list[i].securityID == "2")
                    {
                        list[i].securityID = "机密";
                    }
                    if (list[i].securityID == "3")
                    {
                        list[i].securityID = "绝密";
                    }
                    if (list[i].securityID == "4")
                    {
                        list[i].securityID = "一般";
                    }
                    if (list[i].securityID == "5")
                    {
                        list[i].securityID = "内部";
                    }
                    if (list[i].securityID == "6")
                    {
                        list[i].securityID = "公开/内部";
                    }
                    if (list[i].retentionPeriodNo == "1")
                    {
                        list[i].retentionPeriodNo = "长期";
                    }
                    if (list[i].retentionPeriodNo == "2")
                    {
                        list[i].retentionPeriodNo = "永久";
                    }
                    if (list[i].retentionPeriodNo == "3")
                    {
                        list[i].retentionPeriodNo = "短期";
                    }
                    string yd = "";
                    if (list[i].isYD == true)
                    {
                        yd = "是";
                    }
                    if (list[i].isYD == false)
                    {
                        yd = "否";
                    }
                    sHtml.Append("<tr height=\"20\" align=\"left\"><td>" + list[i].projectName
                            + "</td><td>" + list[i].location + "</td><td>" + list[i].newLocation + "</td><td>" + list[i].submitPerson
                            + "</td><td>" + list[i].mobilephoneSubmitPerson + "</td><td>" + list[i].telphoneSubmitPerson + "</td><td>" + list[i].paperProjectSeqNo
                            + "</td><td>" + list[i].projectNo + "</td><td>" + list[i].securityID + "</td><td>" + list[i].buildingArea + "</td><td>" + list[i].height
                            + "</td><td>" + list[i].Material + "</td><td>" + list[i].layerCount + "</td><td>" + list[i].retentionPeriodNo + "</td><td>" + yd
                            + "</td><td>" + list[i].statusName + "</td><td>" + list[i].developmentOrganization + "</td><td>" + list[i].devolonpentOrgContacter
                            + "</td><td>" + list[i].telphoneNoDevelopment + "</td><td>" + list[i].mobilephoneNoDevelopment + "</td><td>" + list[i].jianliUnit
                            + "</td><td>" + list[i].jianliUnitContacter + "</td><td>" + list[i].telphoneNoJianliUnit + "</td><td>" + list[i].constructionOrganization
                            + "</td><td>" + list[i].constructionOrgContacter + "</td><td>" + list[i].telphoneNoConstruction
                            + "</td><td>" + list[i].disignOrganization + "</td><td>" + list[i].designOrgaContacter
                            + "</td><td>" + list[i].telphoneNoDesignOrga + "</td><td>" + list[i].MapOrginisation + "</td><td>" + list[i].Mapper
                            + "</td><td>" + list[i].TeleNoMap + "</td><td>" + list[i].dateReceived + "</td><td>" + list[i].recipient
                            + "</td><td>" + list[i].projectProfile + "</td><td>" + list[i].collationRequirement + "</td><td>" + list[i].csyj
                            + "</td><td>" + list[i].csyjPerson + "</td><td>" + list[i].csyjDate + "</td><td>" + list[i].memo + "</td><td>" + list[i].storagePath
                            + "</td></tr>");
                }
                //打印表尾
                sHtml.Append("</table>");
                //调用输出Excel表的方法
                ExportToExcel("application/ms-excel", "管线工程信息目录.xls", sHtml.ToString());
            }
            return View();
        }
        //输入HTTP头，然后把指定的流输出到指定的文件名，然后指定文件类型
        public void ExportToExcel(string FileType, string FileName, string ExcelContent)
        {
            System.Web.HttpContext.Current.Response.ContentType = FileType;
            System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            System.Web.HttpContext.Current.Response.Charset = "utf-8";
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(FileName, System.Text.Encoding.UTF8).ToString());

            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.HttpContext.Current.Response.Output.Write(ExcelContent.ToString());
            /*乱码BUG修改 20140505*/
            //如果采用以上代码导出时出现内容乱码，可将以下所注释的代码覆盖掉上面【System.Web.HttpContext.Current.Response.Output.Write(ExcelContent.ToString());】即可实现。
            //System.Web.HttpContext.Current.Response.Write("<meta http-equiv=\"content-type\" content=\"application/ms-excel; charset=utf-8\"/>" + ExcelContent.ToString());
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
        }
        public ActionResult danganjieshoumingxi(string action, string type = "PDF")
        {
                LocalReport localReport = new LocalReport();
                string PNoS = Request.Form["seqNoStart"];
                string PNoE = Request.Form["seqNoEnd"];
                string SeqS = Request.Form["txtSeqNoS"];
                string SeqE = Request.Form["txtSeqNoE"];
                if (action == "打印档案接收明细（工程序号）")
                {
                    long n = long.Parse(PNoS);
                    long m = long.Parse(PNoE);
                    var list = db.vw_gxprojectList.Where(ad => ad.projectNo >= n).Where(ad => ad.projectNo <= m).ToList();
                    //var temp = db.vw_gxreceiveArchiveDetail.Where(ad => ad.projectNo >= n).Where(ad => ad.projectNo <= m);
                    //List<vw_gxreceiveArchiveDetail> list = temp.ToList();
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

                    localReport.ReportPath = Server.MapPath("~/Report/guanxian/DangAnJieShouMingXi.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("gxjieshoumingxi", ds);
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
                    long n = long.Parse(SeqS);
                    long m = long.Parse(SeqE);
                    var list = db.vw_gxprojectList.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.paperProjectSeqNo <= m).ToList();
                    //var temp = db.vw_gxreceiveArchiveDetail.Where(ad => ad.paperProjectSeqNo >= n).Where(ad => ad.paperProjectSeqNo <= m);
                    //List<vw_gxreceiveArchiveDetail> list = temp.ToList();
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
                    localReport.ReportPath = Server.MapPath("~/Report/guanxian/DangAnJieShouMingXi.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("gxjieshoumingxi", ds);
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
        // GET: ProjectManagement/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProjectManagement/Delete/5
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
    }
}
