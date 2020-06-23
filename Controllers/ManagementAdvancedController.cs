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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace urban_archive.Controllers
{
    public class ManagementAdvancedController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        public ActionResult window()
        {
            return View();
        }
        public ActionResult ManagementPrint(string SelectedID, string SearchString, string action)
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
            if (SelectedID == null | SelectedID == "")
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", 0);
            }
            else
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);
            }
          
            ViewBag.CurrentFilter = SearchString;
            ViewBag.SearchType = action;
            return View();
        }
        public string str(int ?page,string type,string content, string SearchType)
        {
            var vwprojectFile = from ad in db.vw_ProjectStatus
                                where ad.status == "7"
                                select ad;
            if (type!=""&&type!=null&&content!=""&&content!=null)//用户在检索框中输入了检索条件
            {
                int t = Int32.Parse(type.Trim());
                if (SearchType=="模糊查找")
                {
                    switch (t)
                    {
                        case 0:
                            vwprojectFile = vwprojectFile.Where(ad => ad.contractNo.Contains(content));//根据责任书编号搜索
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
                            //long projectseNo=long.Parse(content);
                            //vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo== projectseNo);//根据项目顺序号
                            vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo.ToString().Contains(content));
                            break;

                    }

                }
               else
                {
                    switch (t)
                    {
                        case 0:
                            vwprojectFile = vwprojectFile.Where(ad => ad.contractNo==content);//根据责任书编号搜索
                            break;
                        case 1:
                            vwprojectFile = vwprojectFile.Where(ad => ad.projectName == content);//根据工程名称搜索
                            break;
                        case 2:
                            vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization == content);//根据建设单位搜索
                            break;
                        case 3:
                            vwprojectFile = vwprojectFile.Where(ad => ad.location == content);//根据工程地点
                            break;
                        case 4:
                            long pro = long.Parse(content.Trim());
                            vwprojectFile = vwprojectFile.Where(ad => ad.projectNo== pro); //根据工程序号
                            break;
                        case 5:
                            vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization == content);//根据施工单位
                            break;
                        case 6:
                            vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization == content);//根据设计单位
                            break;
                        case 7:
                            vwprojectFile = vwprojectFile.Where(ad => ad.jianliUnit == content);//根据监理单位
                            break;
                        case 8:
                            long projectseNo = long.Parse(content);
                            vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo == projectseNo);//根据项目顺序号
                            break;

                    }

                }
            }
          
          
            int pageSize =100;
            int pageNumber = (page ?? 1);
            int cnt =vwprojectFile.Count() / pageSize + 1;
            if (vwprojectFile.Count()%pageSize==0)
            {
                cnt = vwprojectFile.Count() / pageSize;
            }
            vwprojectFile = vwprojectFile.OrderBy(s => s.paperProjectSeqNo);
            var a =vwprojectFile.ToPagedList(pageNumber, pageSize);
            var b = new JObject(
                        new JProperty("last_page",cnt),
                        new JProperty("data",
                                new JArray(
                                        //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程
                                        from p in a
                                        select new JObject(
                                                 new JProperty("projectNo", p.projectNo),
                                                 new JProperty("projectName", p.projectName),
                                                 new JProperty("paperProjectSeqNo", p.paperProjectSeqNo),
                                                 new JProperty("developmentOrganization", p.developmentOrganization),
                                                 new JProperty("location", p.location),
                                                 new JProperty("statusName", p.statusName),
                                                 new JProperty("projectID", p.projectID),
                                                 new JProperty("id", p.id)

                                        )
                                )
                    )
).ToString();
            return b;

        }
        public ActionResult ArchiveMaga(string SelectedID, string SearchString, string action)
        {

            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "项目顺序号", Value = "2"},
                new SelectListItem { Text = "档号", Value = "0"},
                new SelectListItem { Text = "案卷题名", Value = "1"},


            };
            if (SelectedID == null | SelectedID == "")
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", 0);
            }
            else
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);
            }

            ViewBag.CurrentFilter = SearchString;
            ViewBag.SearchType = action;

            return View();
        }
        public string str2(int? page,string type,string content, string SearchType)
        {
            var vwprojectFile = from ad in db.ArchivesDetail
                                where ad.archivesTitle!=null&&ad.archivesTitle!=""
                                select ad;
            if (type != "" && type != null && content != "" && content != null)//用户在检索框中输入了检索条件
            {
                int t = Int32.Parse(type.Trim());
                if (SearchType == "模糊查找")
                {
                    switch (t)
                    {
                        case 0:
                            vwprojectFile = vwprojectFile.Where(ad => ad.archivesNo.ToString().Contains(content));//根据工程名称搜索
                            break;
                        case 1:
                            vwprojectFile = vwprojectFile.Where(ad => ad.archivesTitle.Contains(content));//根据地点搜索
                            break;
                        case 2:
                          
                            vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo.ToString().Contains(content));//根据地点搜索
                            break;
                    }
                }
                else
                {
                    switch (t)
                    {
                        case 0:
                            vwprojectFile = vwprojectFile.Where(ad => ad.archivesNo==content);//根据工程名称搜索
                            break;
                        case 1:
                            vwprojectFile = vwprojectFile.Where(ad => ad.archivesTitle == content);//根据地点搜索
                            break;
                        case 2:
                            long projectseqNo = long.Parse(content);
                            vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo == projectseqNo);//根据地点搜索
                            break;
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
            vwprojectFile = vwprojectFile.OrderBy(s => s.paperProjectSeqNo).ThenBy(d=>d.volNo);
            var a = vwprojectFile.ToPagedList(pageNumber, pageSize);
            var b = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                                        //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程
                                        from p in a
                                        select new JObject(
                                                 new JProperty("paperProjectSeqNo", p.paperProjectSeqNo),
                                                 new JProperty("registrationNo", p.registrationNo),
                                                 new JProperty("archivesNo", p.archivesNo),
                                                 new JProperty("archivesTitle", p.archivesTitle),
                                                 new JProperty("developmentUnit", p.developmentUnit),
                                                 new JProperty("archiveThickness", p.archiveThickness),
                                                 new JProperty("typist", p.typist),
                                                 new JProperty("volNo", p.volNo),
                                                 new JProperty("ID",p.ID)

                                        )
                                )
                    )
).ToString();
            return b;
        }
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var vwproject = from a in db.vw_projectProfile
                            where a.id == id
                            select a;
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", vwproject.First().retentionPeriodNo);
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", vwproject.First().securityID);
            var users = from ad in ab.AspNetUsers
                        where ad.DepartmentName == User.Identity.Name
                        select ad;
            ViewBag.recipient = new SelectList(users, "UserName", "UserName", vwproject.First().recipient);

            ViewBag.status = new SelectList(db.ArchivesStatus, "status", "statusName", vwproject.First().status);
            //设置是否异地存储
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

            };
            ViewBag.isYD = new SelectList(list, "Value", "Text");
            if (vwproject.First().isYD == true)
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 1);
            }
            else
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 0);
            }
            if (vwproject.Count() == 0)
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
        public ActionResult Edit(string isYD, string id, string projectName,string location,string newLocation,string submitPerson, string mobilephoneSubmitPerson, string telphoneSubmitPerson,  string buildingArea, string securityID, string retentionPeriodNo, string status, string developmentOrganization, string devolonpentOrgContacter, string telphoneNoDevelopment, string mobilephoneNoDevelopment, string jianliUnit, string jianliUnitContacter, string telphoneNoJianliUnit, string constructionOrganization, string constructionOrgContacter, string telphoneNoConstruction, string disignOrganization, string designOrgaContacter, string telphoneNoDesignOrga, string dateReceived, string recipient, string projectProfile, string collationRequirement, string csyj, string csyjPerson, string csyjDate, string memo, string action)
        {
            long proID = 0;
            if (id != "" && id != null)
            {
                proID = int.Parse(id.Trim());
            }
            var projects = from ac in db.ProjectInfo
                           where ac.id == proID
                           select ac;
            ProjectInfo project = projects.First();
            var paperArchives = from ad in db.PaperArchives
                                where ad.projectID == project.projectID
                                select ad;
            PaperArchives paperArchive = paperArchives.First();

            if (action == "修改")
            {
                //需改ProjectInfo表
                project.projectName = projectName;
                project.location = location;
                project.newLocation = newLocation;
                project.securityID = securityID;
                project.retentionPeriodNo = retentionPeriodNo;
                project.status = status;
                project.developmentOrganization = developmentOrganization;
                project.devolonpentOrgContacter = devolonpentOrgContacter;
                project.telphoneNoDevelopment = telphoneNoDevelopment;
                project.mobilephoneNoDevelopment = mobilephoneNoDevelopment;
                project.jianliUnit = jianliUnit;
                project.jianliUnitContacter = jianliUnitContacter;
                project.telphoneNoJianliUnit = telphoneNoJianliUnit;
                project.constructionOrganization = constructionOrganization;
                project.constructionOrgContacter = constructionOrgContacter;
                project.telphoneNoConstruction = telphoneNoConstruction;
                project.disignOrganization = disignOrganization;
                project.designOrgaContacter = designOrgaContacter;
                project.telphoneNoDesignOrga = telphoneNoDesignOrga;
                project.memo = memo;
                if (isYD == "1")
                {
                    project.isYD = true;
                }
                else
                {
                    project.isYD = false;
                }
                //更新PaperArhive表
                paperArchive.submitPerson = submitPerson;
                paperArchive.mobilephoneSubmitPerson = mobilephoneSubmitPerson;
                paperArchive.telphoneSubmitPerson = telphoneSubmitPerson;
                paperArchive.dateArchive = DateTime.Parse(dateReceived);
                paperArchive.recipient = recipient;
                paperArchive.projectProfile = projectProfile;
                paperArchive.collationRequirement = collationRequirement;
                paperArchive.csyj = csyj;
                paperArchive.csyjPerson = csyjPerson;
                paperArchive.csyjDate = DateTime.Parse(csyjDate);
                

                if(buildingArea==""|| buildingArea==null)
                {
                    paperArchive.buildingArea = 0;
                }
                else
                {
                    paperArchive.buildingArea = double.Parse(buildingArea.Trim());
                }

                db.Entry(paperArchive).State = EntityState.Modified;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return Content("<script >alert('修改成功！');window.history.back();</script >");
            }
        
          
           if(action=="返回")
          {

            
                return RedirectToAction("ManagementPrint", "ManagementAdvanced");
           }
            

            return View();
        }
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var test = from ad in db.vw_projectProfile
                       where (ad.id == id)
                       select ad;
            vw_projectProfile projectProfile = test.First();
            //档案密级，档案状态，保存期限的初始化
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
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

              };
            if (projectProfile.isYD == true)
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 1);
            }
            else
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 0);
            }

            return View(projectProfile);
        }

        [HttpPost]
        public ActionResult Details(string  id)
        {
           
                int id1=0;
                if (id!=null&&id!="")
                {
                   id1 = Int32.Parse(id);
                }

                var c = from g in db.ProjectInfo
                        where g.id == id1
                        select g;
                ProjectInfo projectInfo = c.First();
                var d = from f in db.PaperArchives
                        where f.projectID == projectInfo.projectID
                        select f;
                PaperArchives paperArchive = d.First();
                db.ProjectInfo.Remove(projectInfo);
                db.PaperArchives.Remove(paperArchive);
                db.SaveChanges();
                return Content("<script >alert('已成功删除！');window.location.href='/ManagementAdvanced/ManagementPrint';</script >");
          

           

        }
       
        public ActionResult EnterAndSee(string id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var projectinfo = from b in db.vw_passList
            //                  where b.registrationNo == id
            //                  select b;
            var projectinfo = from b in db.vw_passList
                              where b.ID.ToString() == id
                              select b;
            long paperseq = projectinfo.First().paperProjectSeqNo;
            string seq = paperseq.ToString().Trim();
            var project = from c in db.vw_passList
                          where c.paperProjectSeqNo == paperseq
                          orderby c.volNo
                          select c;
           
            //初始化相关选择控件
          
            ViewBag.indexer = new SelectList(ab.AspNetUsers, "UserName", "UserName", projectinfo.First().indexer);
            ViewBag.checker = new SelectList(ab.AspNetUsers, "UserName", "UserName", projectinfo.First().checker);
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
                ViewData["TyperDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-"+ DateTime.Now.Day.ToString("D2");
            }
            vw_passList paper = new vw_passList();
            paper = projectinfo.First();
          

            return View(paper);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //传入
        public ActionResult EnterAndSee(string archivesCount,long? ID, string storagePath,string volNo, string registrationNo, string shizhengNo, string ClassNo, string archivesNo, string paperProjectSeqNo, string paijiaNo, string licenseNo, string UrbanNo, string mapsheetNo, string microNo, string securityName, string retentionPeriodName, string structureTypeName, string buildingArea, string archivesTitle, string firstResponsible, string responsibleOther, string developmentOrganization, string transferUnit, string disignOrganization, string constructionOrganization, string textMaterial, string drawing, string photoCount, string archiveThickness, string bianzhiTime, string jgDate, string height, string changeLog, string location, string remarks, string fazhaoTime, string kaigongTime, string jungongTime, string indexer, string checker, string Typist, string indexeDate, string checkDate, string TyperDate, string action)
        {

            var model = from a in db.ArchivesDetail
                        where a.ID==ID
                        select a;
            //long paperseqNo = model.First().paperProjectSeqNo;
            //var paper = from b in db.PaperArchives
            //            where b.paperProjectSeqNo == paperseqNo
            //            select b;
            //long proID = Convert.ToInt64(paper.First().projectID);
            //var projectInfo = from c in db.ProjectInfo
            //                  where c.projectID == proID
            //                  select c;
            if (action=="保存")
            {
                //不联动20180425zl
                //更新paperArchive表
                //paper.First().archivesCount = archivesCount;
                //paper.First().archivesCount= archivesCount;
                //paper.First().prevClassNo = ClassNo;
                ////paper.First().paperProjectSeqNo = long.Parse(paperProjectSeqNo);
                //paper.First().licenseNo = licenseNo;
                //paper.First().PhotoCount = long.Parse(photoCount);
                //paper.First().height = double.Parse(height);
                //paper.First().changeLog = changeLog;
                //paper.First().remarks = remarks;
                
                ////更新projectinfo表
                //projectInfo.First().securityID = securityName;
                //projectInfo.First().retentionPeriodNo = retentionPeriodName;
                //projectInfo.First().structureTypeID = structureTypeName;
                //projectInfo.First().developmentOrganization = developmentOrganization;
                //projectInfo.First().disignOrganization = disignOrganization;
                //projectInfo.First().constructionOrganization = constructionOrganization;
                //projectInfo.First().textMaterial = long.Parse(textMaterial);
                //projectInfo.First().drawing = long.Parse(drawing);
                //projectInfo.First().location = location;
                //projectInfo.First().storagePath = storagePath;
                //if(buildingArea==null|| buildingArea=="")
                //{
                //    paper.First().buildingArea = 0;
                //}
                //else
                //{
                //    paper.First().buildingArea = double.Parse(buildingArea);
                //}
                //更新ArchiveDetail表
                model.First().volNo =Int32.Parse(volNo);
                model.First().shizhengNo = shizhengNo;
                model.First().registrationNo = registrationNo;
                model.First().archivesNo = archivesNo;
                model.First().paijiaNo = paijiaNo;
                model.First().mapsheetNo = mapsheetNo;
                model.First().archivesTitle = archivesTitle;
                model.First().firstResponsible = firstResponsible;
                model.First().responsibleOther = responsibleOther;
                model.First().transferUnit = transferUnit;
                model.First().microNo = microNo;
                model.First().microNo = microNo;
                model.First().microNo = microNo;
                model.First().microNo = microNo;
                model.First().microNo = microNo;
                model.First().microNo = microNo;
                model.First().microNo = microNo;
                model.First().archiveThickness = long.Parse(archiveThickness);
                model.First().bianzhiTime =bianzhiTime;
                model.First().jgDate = DateTime.Parse(jgDate);
                model.First().fazhaoTime = fazhaoTime;
                model.First().kaigongTime = kaigongTime;
                model.First().jungongTime = jungongTime;
                model.First().indexer = indexer;
                model.First().indexDate = DateTime.Parse(indexeDate);
                model.First().typist = Typist;
                model.First().typerDate = DateTime.Parse(TyperDate);
                model.First().checkDate = DateTime.Parse(checkDate);
                model.First().checker = checker;
                model.First().paperProjectSeqNo = long.Parse(paperProjectSeqNo);
                //db.Entry(projectInfo.First()).State = EntityState.Modified;
                //db.Entry(paper.First()).State = EntityState.Modified;
                db.Entry(model.First()).State = EntityState.Modified;
                db.SaveChanges();
                return Content("<script >alert('保存成功！');window.history.back();</script >");
            }
            if(action == "删除")
            {
              

                var c = from g in db.ArchivesDetail
                        where g.ID==ID
                        select g;
                ArchivesDetail archive = c.First();
                //string e = c.First().archivesNo;
                //var d = from f in db.FileInfo
                //        where f.archivesNo==e
                //        select f;
                //FileInfo filelist = d.First();
                //db.FileInfo.Remove(filelist);
                db.ArchivesDetail.Remove(archive);
                db.SaveChanges();
                return Content("<script >alert('删除成功！');window.history.back();</script >");
            }
            if (action=="返回")
            {
                return RedirectToAction("ArchiveMaga", "ManagementAdvanced");
            }




          


            return View();
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
        public ActionResult FileList(string id1, int id2, int id)
        {

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
                    ViewBag.SelectedID = new SelectList(list2, "Value", "Text", file.First().type.Trim());
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
                    return Content("<script >alert('添加成功！');window.location.href='/ManagementAdvanced/FileList?id1=" + ArchiveNo + "&id=" + id + "&id2=1" + "';</script >");
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
            return View();




        }
    }
}