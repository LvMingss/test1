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
using Newtonsoft.Json;

namespace urban_archive.Controllers
{
    public class ArchiveManagementController : Controller
    {
        // GET: ArchiveManagement
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        public ActionResult ArchiveMaga( string currentFilter, string SearchString,int? SelectedID, string action)
        {
            ViewData["pagename"] = "ArchiveManagement/ArchiveMaga";
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "项目顺序号", Value = "2"},
                new SelectListItem { Text = "档号", Value = "0"},
                new SelectListItem { Text = "案卷题名", Value = "1"},
                 

            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            int t = SelectedID.GetValueOrDefault();

           
            ViewBag.CurrentFilter = SearchString;
            var archivemanagement = from ad in db.ArchivesDetail
                                    select ad;
            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (t)
                {
                    case 0:
                        archivemanagement = archivemanagement.Where(ad => ad.archivesNo.ToString().Contains(SearchString));//根据工程名称搜索
                        break;
                    case 1:
                        archivemanagement = archivemanagement.Where(ad => ad.archivesTitle.Contains(SearchString));//根据地点搜索
                        break;
                    case 2:
                        archivemanagement = archivemanagement.Where(ad => ad.paperProjectSeqNo.ToString().Contains(SearchString));//根据地点搜索
                        break;
                }

            }
            if(action=="添加案卷")
            {
                var passlist = from a in db.vw_passList
                               orderby a.paperProjectSeqNo descending
                               select a;
                long b = passlist.First().paperProjectSeqNo;
                
                return RedirectToAction("AddArchive",new { id= b });
            }
            archivemanagement = archivemanagement.OrderByDescending(s => s.paperProjectSeqNo).Take(1000);// 默认按项目顺序号排列
            
           ViewBag.result = JsonConvert.SerializeObject(archivemanagement);
            return View();
        }
        public ActionResult AddArchive(long? id)
        {
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName");
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName");
            ViewBag.structureTypeID = new SelectList(db.StructureType, "structureTypeID", "structureTypeName");
            ViewBag.indexer = new SelectList(ab.AspNetUsers, "UserName", "UserName");
            ViewBag.checker = new SelectList(ab.AspNetUsers, "UserName", "UserName");
            ViewBag.typist = new SelectList(ab.AspNetUsers, "UserName", "UserName");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vw_passList pass = new vw_passList();
            return View(pass);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //传入

        public ActionResult AddArchive(long paperProjectSeqNo, string action, string registrationNo, string archivesNo, string height,string volNo, string shizhenghao, string licenseNo, string mapsheetNo, string microNo, string securityID, string retentionPeriodNo, string structureTypeID, string buildingArea, string archivesTitle, string firstResponsible, string responsibleOther, string developmentOrganization, string transferUnit, string disignOrganization, string paijiaNo, string constructionOrganization, string textMaterial, string drawing, string photoCount, string archiveThickness, string bianzhiTime, string jgDate, string heigh, string location, string remarks, string newLocation, string overground, string underground, string changeLog, string fazhaoTime, string jungongTime, string kaigongTime, string indexer, string indexDate, string checker, string checkDate, string Typist, string TyperDate)
        {
            if(action=="返回")
            {
                return RedirectToAction("ArchiveMaga");
            }
            var papaerarchive = from a in db.PaperArchives
                                where a.paperProjectSeqNo == paperProjectSeqNo
                                select a;
            PaperArchives paperArchives = papaerarchive.First();
            long ID =Convert.ToInt32(paperArchives.projectID);
            var projectInfo = from b in db.ProjectInfo
                              where b.projectID == ID
                              select b;
            ProjectInfo projects = projectInfo.First();
            ArchivesDetail archive = new ArchivesDetail();
            archive.registrationNo = registrationNo;
            archive.archivesNo = archivesNo;
            archive.paperProjectSeqNo = paperProjectSeqNo;
            if (archive.volNo.ToString().Trim()!= "")
            {
                archive.volNo = Int32.Parse(archive.volNo.ToString().Trim());
            }
            archive.paijiaNo = paijiaNo.Trim();
            archive.archiveThickness = Convert.ToInt32(archiveThickness);

            if (textMaterial.Trim() != "")
            {
                archive.textMaterial = Int32.Parse(textMaterial.Trim());//文字材料

            }
            archive.archivesTitle = archivesTitle;//案卷提名
            archive.firstResponsible = firstResponsible;//第一负责人

            if (drawing.Trim() != "")
                archive.drawing = Int32.Parse(drawing.Trim());//图纸
            if (photoCount.Trim() != "")//照片
            {
                archive.photoCount = Int32.Parse(photoCount.Trim());
            }
            archive.responsibleOther = responsibleOther;//其他责任者


            archive.developmentUnit = developmentOrganization;
            archive.constructionUnit = constructionOrganization;
            archive.designUnit = disignOrganization;

            archive.bianzhiTime = bianzhiTime.Trim();//编制日期
                                                           //archive.bianzhiTime = DateTime.ParseExact(strbianzhiTime, "yyyy-MM-dd", null).Date;
            archive.transferUnit = transferUnit;//移交单位           
            archive.notearea = location;//附注改为工程地址
            archive.licenseNo = licenseNo;//执照号
            archive.shizhengNo = shizhenghao; //市政档案号

            archive.remarks = remarks;//备注
            string strbiaoyinriqi = indexDate;//标引日期
            archive.indexDate = DateTime.Parse(strbiaoyinriqi);
            archive.indexer = indexer;//标引员


            string strshenheriqi = checkDate;//审核日期
            archive.checkDate = DateTime.Parse(strshenheriqi);
            archive.checker = checker;//审核员

            archive.kaigongTime = kaigongTime.Trim();//开工日期
            archive.jungongTime = jungongTime.Trim();//竣工日期
            archive.fazhaoTime = fazhaoTime.Trim();//发照日期
            archive.jgDate = DateTime.Parse(jgDate.Trim());
            string strlururiqi = TyperDate;//录入日期
            archive.typerDate = DateTime.Parse(strlururiqi);
            archive.typist = Typist;//录入员  

            archive.isImageExist = "无";
            if (mapsheetNo == "")
            {
                archive.mapsheetNo = "0";
            }
            else
            {
                archive.mapsheetNo = mapsheetNo;//图幅号
            }
            if (microNo == "")
            {
                archive.microNo = "0";
            }
            else
            {
                archive.microNo = microNo;//微缩号
            }




            if (ModelState.IsValid)
            {
                db.ArchivesDetail.Add(archive);
                
               
                    db.SaveChanges();
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
                projects.securityID = securityID.Trim();
                projects.retentionPeriodNo = retentionPeriodNo.Trim();
                projects.structureTypeID = structureTypeID.Trim();
                projects.newLocation = newLocation.Trim();
                if (ModelState.IsValid)
                {
                    db.Entry(projects).State = EntityState.Modified;

                    db.SaveChanges();
                }
            }
            if (paperArchives != null)
            {
                string jinguandata = paperArchives.jgDate.ToString();
                if (jinguandata != jgDate.Trim())
                {
                    paperArchives.jgDate = DateTime.Parse(jgDate.Trim());
                }
                paperArchives.buildingArea = Convert.ToDouble((buildingArea.Trim()));
                paperArchives.underground = underground.Trim();
                paperArchives.overground = overground.Trim();
                if (height.Trim() != "")
                {
                    paperArchives.height = Convert.ToDouble((height.Trim()));
                }
                paperArchives.luruTime = TyperDate.Trim();
                paperArchives.projectStartDate = kaigongTime.Trim();
                paperArchives.projectFinishDate = jungongTime.Trim();
                paperArchives.licenseNo = licenseNo.Trim();
                paperArchives.licenseDate = fazhaoTime.Trim();
                paperArchives.changeLog = fazhaoTime.Trim();
                //paperArchives.changeLog = ddlChangeLog.Items[ddlChangeLog.SelectedIndex].Text;
                paperArchives.transferUnit = transferUnit;
                if (textMaterial.Trim() != "")
                {
                    if (paperArchives.textMaterial != 0)
                    {
                        paperArchives.textMaterial = paperArchives.textMaterial + Int32.Parse(textMaterial.Trim());
                    }
                    else
                    {
                        paperArchives.textMaterial = Int32.Parse(textMaterial.Trim());
                    }
                }
                if (textMaterial.Trim() != "")
                {
                    if (paperArchives.drawing != 0)
                    {
                        paperArchives.drawing = paperArchives.drawing + Int32.Parse(textMaterial.Trim());
                    }
                    else
                    {
                        paperArchives.drawing = Int32.Parse(textMaterial.Trim());
                    }
                }
                if (photoCount.Trim() != "")
                {
                    if (paperArchives.PhotoCount != 0)
                    {
                        paperArchives.PhotoCount = paperArchives.PhotoCount + Int32.Parse(photoCount.Trim());
                    }
                    else
                    {
                        paperArchives.PhotoCount = Int32.Parse(photoCount.Trim());
                    }
                }
                paperArchives.firstResponsible = firstResponsible.Trim();
                paperArchives.responsibleOther = responsibleOther.Trim();


                string shizhengno = shizhenghao;
                int volCount = 0;
                int.TryParse(paperArchives.archivesCount, out volCount);
                int vol = Convert.ToInt32(archive.volNo);

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
                    db.SaveChanges();
                    return JavaScript("保存成功");
                }
            }


            return View();

        }
    }
}