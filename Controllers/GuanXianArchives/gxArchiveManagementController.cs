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
    public class gxArchiveManagementController : Controller
    {
        // GET: ArchiveManagement
        private gxArchivesContainer cb = new gxArchivesContainer();
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();

        public ActionResult ArchiveMagaNB(string SearchString,string action)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程序号", Value = "1"},
                new SelectListItem { Text = "设计单位", Value = "2" },
                new SelectListItem { Text = "施工单位", Value = "3" },
                new SelectListItem { Text = "项目顺序号", Value = "4" },
                new SelectListItem { Text = "盒号", Value = "5" },
                new SelectListItem { Text = "测绘单位", Value = "6" },



        };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text","5");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            string n = Request.Form["SelectedID"];

            ViewBag.CurrentFilter = SearchString;
            string user = User.Identity.Name;
            var depart = from c in ab.AspNetUsers
                         where c.UserName == user
                         select c;
            string name = depart.First().UserName;
            var vwprojectlist = from ad in cb.vw_gxprojectList
                                where ad.isNB == "内部"
                                where ad.classifyID == "1         "
                                where ad.collator == name
                                select ad;
            if (name == "管线科" || name == "业务科")
            {
                vwprojectlist = from ad in cb.vw_gxprojectList

                                where ad.classifyID == "1         "
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
                        int box = int.Parse(SearchString);
                        vwprojectlist = vwprojectlist.Where(ad => ad.boxNo == box);
                        break;
                    case 6:
                        vwprojectlist = vwprojectlist.Where(ad => ad.MapOrginisation.Contains(SearchString));//根据地点搜索
                        break;
                }

            }

            if (action == "添加案卷")
            {
                var passlist = from a in cb.vw_gxpassList
                               orderby a.paperProjectSeqNo descending
                               select a;
                long b = passlist.First().paperProjectSeqNo;

                return RedirectToAction("AddArchive", new { id = b });
            }

            vwprojectlist = vwprojectlist.OrderByDescending(s => s.paperProjectSeqNo);// 默认按项目顺序号排列
            ViewBag.result = JsonConvert.SerializeObject(vwprojectlist);
            return View();

        }
        public ActionResult SZArchiveMagaNB(string SearchString,string action)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程序号", Value = "1"},
                new SelectListItem { Text = "设计单位", Value = "2" },
                new SelectListItem { Text = "施工单位", Value = "3" },
                new SelectListItem { Text = "项目顺序号", Value = "4" },
                new SelectListItem { Text = "盒号", Value = "5" },
                 new SelectListItem { Text = "测绘单位", Value = "6"},

            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text","5");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            string n = Request.Form["SelectedID"];

            ViewBag.CurrentFilter = SearchString;
            string user = User.Identity.Name;
            var depart = from c in ab.AspNetUsers
                         where c.UserName == user
                         select c;
            string name = depart.First().UserName;
            var vwprojectlist = from ad in cb.vw_gxprojectList
                                where ad.isNB == "内部"
                                where ad.classifyID== "2         "
                                where ad.collator == name
                                select ad;
            if (name == "管线科"||name=="业务科")
            {
                vwprojectlist = from ad in cb.vw_gxprojectList
                                
                                where ad.classifyID== "2         "
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
                        int box = int.Parse(SearchString);
                        vwprojectlist = vwprojectlist.Where(ad => ad.boxNo == box);

                        break;
                    case 6:
                        vwprojectlist = vwprojectlist.Where(ad => ad.MapOrginisation.Contains(SearchString));//根据建设单位搜索
                        break;

                }

            }

            if (action == "添加案卷")
            {
                var passlist = from a in cb.vw_gxpassList
                               orderby a.paperProjectSeqNo descending
                               select a;
                long b = passlist.First().paperProjectSeqNo;

                return RedirectToAction("AddArchive", new { id = b });
            }

            vwprojectlist = vwprojectlist.OrderByDescending(s => s.paperProjectSeqNo);// 默认按项目顺序号排列
            ViewBag.result = JsonConvert.SerializeObject(vwprojectlist);
            return View();

        }
        public ActionResult ArchiveMaga(string SearchString, string action)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "档号", Value = "0"},
                new SelectListItem { Text = "案卷题名", Value = "1"},
                new SelectListItem { Text = "第一责任者", Value = "2"},
                new SelectListItem { Text = "项目顺序号", Value = "3"},
                new SelectListItem { Text = "工程序号", Value = "4"},
                new SelectListItem { Text = "测绘单位", Value = "5"},
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            string n = Request.Form["SelectedID"];
            ViewBag.CurrentFilter = SearchString;
            
            var archivemanagement = from ad in cb.vw_gxarchiveInfo
                                    where ad.classifyID== "1         "
                                    where ad.isNB=="外部"
                                    select ad;
            if (!String.IsNullOrEmpty(SearchString))
            {
                int t = int.Parse(n);
                ViewBag.SelectedID = new SelectList(list, "Value", "Text",n);
                switch (t)
                {
                    case 0:
                        archivemanagement = archivemanagement.Where(ad => ad.archivesNo == SearchString);//根据工程名称搜索
                        break;
                    case 1:
                        archivemanagement = archivemanagement.Where(ad => ad.archivesTitle.Contains(SearchString));//根据地点搜索
                        break;
                    case 2:
                        archivemanagement = archivemanagement.Where(ad => ad.firstResponsible.Contains(SearchString));//根据地点搜索
                        break;
                    case 3:
                        long? id = long.Parse(SearchString);
                        archivemanagement = archivemanagement.Where(ad => ad.paperProjectSeqNo==id);//根据地点搜索
                        break;
                    case 4:
                        long? id1 = long.Parse(SearchString);
                        archivemanagement = archivemanagement.Where(ad => ad.projectNo == id1);//根据地点搜索
                        break;
                    case 5:
                        archivemanagement = archivemanagement.Where(ad => ad.MapOrginisation.Contains(SearchString));//根据地点搜索
                        break;
                }

            }
            if(action=="添加案卷")
            {
                var passlist = from a in cb.vw_gxpassList
                               orderby a.paperProjectSeqNo descending
                               select a;
                long b = passlist.First().paperProjectSeqNo;
                
                return RedirectToAction("AddArchive",new { id= b });
            }
            archivemanagement = archivemanagement.OrderByDescending(s => s.paperProjectSeqNo);// 默认按项目顺序号排列
            ViewBag.result = JsonConvert.SerializeObject(archivemanagement);
            return View();
        }
        public ActionResult SZArchiveMaga(string SearchString, string action)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "档号", Value = "0"},
                new SelectListItem { Text = "案卷题名", Value = "1"},
                new SelectListItem { Text = "第一责任者", Value = "2"},
                 new SelectListItem { Text = "项目顺序号", Value = "3"},
                new SelectListItem { Text = "工程序号", Value = "4"},
                new SelectListItem { Text = "测绘单位", Value = "5"},
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            string n = Request.Form["SelectedID"];
            ViewBag.CurrentFilter = SearchString;
            var archivemanagement = from ad in cb.vw_gxarchiveInfo
                                    where ad.classifyID== "2         "
                                    where ad.isNB=="外部"
                                    select ad;
            if (!String.IsNullOrEmpty(SearchString))
            {
                int t = int.Parse(n);
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", n);
                switch (t)
                {
                    case 0:
                        archivemanagement = archivemanagement.Where(ad => ad.archivesNo == SearchString);//根据工程名称搜索
                        break;
                    case 1:
                        archivemanagement = archivemanagement.Where(ad => ad.archivesTitle.Contains(SearchString));//根据地点搜索
                        break;
                    case 2:
                        archivemanagement = archivemanagement.Where(ad => ad.firstResponsible.Contains(SearchString));//根据第一责任者搜索
                        break;
                    case 3:
                        long? id = long.Parse(SearchString);
                        archivemanagement = archivemanagement.Where(ad => ad.paperProjectSeqNo == id);//根据地点搜索
                        break;
                    case 4:
                        long? id1 = long.Parse(SearchString);
                        archivemanagement = archivemanagement.Where(ad => ad.projectNo == id1);//根据地点搜索
                        break;
                    case 5:
                        archivemanagement = archivemanagement.Where(ad => ad.MapOrginisation.Contains(SearchString));//根据第一责任者搜索
                        break;
                }

            }
            if (action == "添加案卷")
            {
                var passlist = from a in cb.vw_gxpassList
                               orderby a.paperProjectSeqNo descending
                               select a;
                long b = passlist.First().paperProjectSeqNo;

                return RedirectToAction("AddArchive", new { id = b });
            }
            archivemanagement = archivemanagement.OrderByDescending(s => s.paperProjectSeqNo);// 默认按项目顺序号排列
            ViewBag.result = JsonConvert.SerializeObject(archivemanagement);
            return View();
        }

        public ActionResult AddArchive()
        {
            ViewBag.retentionPeriodName = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName");
            ViewBag.securityName = new SelectList(db.SecurityClassification, "securityID", "securityName");
            ViewBag.indexer = new SelectList(ab.AspNetUsers, "UserName", "UserName");
            ViewBag.checker = new SelectList(ab.AspNetUsers, "UserName", "UserName");
            ViewBag.Typist = new SelectList(ab.AspNetUsers, "UserName", "UserName");
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "没有", Value = "0"},
                new SelectListItem { Text = "有", Value = "1"},
            };
            ViewBag.changeLog = new SelectList(list, "Value", "Text");
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //vw_gxpassList pass = new vw_gxpassList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //传入

        public ActionResult AddArchive(int? securityName, string hezai, string totallong, string zaojia, string Fee, int? retentionPeriodName, string structureTypeID, long paperProjectSeqNo, string action, string RegistrationNo, string archivesNo, string shizhengNo, string licenseNo, string mapsheetNo, string microNo, string buildingArea, string archivesTitle, string firstResponsible, string responsibleOther, string developmentOrganization, string transferUnit, string disignOrganization, string PaiJiaNo, string constructionOrganization, string TextMaterial, string drawing, string PhotoCount, string ArchiveThickness, string bianzhiTime, string jgDate, string Material, string location, string remarks, string newLocation, string overground, string underground, int? changeLog, string fazhaoTime, string jungongTime, string kaigongTime, string indexer, string indexeDate, string checker, string checkDate, string Typist, string TyperDate)
        {
            if(action=="返回")
            {
                return RedirectToAction("ArchiveMaga");
            }
            var papaerarchive = from a in cb.gxPaperArchives
                                where a.paperProjectSeqNo == paperProjectSeqNo
                                select a;
            gxPaperArchives paperArchives = papaerarchive.First();
            long ID =Convert.ToInt32(paperArchives.projectID);
            var projectInfo = from b in cb.gxProjectInfo
                              where b.projectID == ID
                              select b;
            gxProjectInfo projects = projectInfo.First();
            gxArchivesDetail archive = new gxArchivesDetail();
            //var SEQNO = cb.vw_gxpassList.Where(a => a.paperProjectSeqNo == paperProjectSeqNo).Where(a=>a.classifyID.Trim()=="1").First().paperProjectSeqNo;
            gxArchivesDetail archive1 = cb.gxArchivesDetail.Where(a=>a.paperProjectSeqNo== paperProjectSeqNo).OrderByDescending(a=>a.volNo).First();
            archive.registrationNo = RegistrationNo;
            archive.archivesNo = archivesNo;
            archive.paperProjectSeqNo = paperProjectSeqNo;
           
            archive.volNo = archive1.volNo;
            archive1.volNo = archive1.volNo + 1;
            archive.paijiaNo = PaiJiaNo.Trim();
            archive.archiveThickness = Convert.ToInt32(ArchiveThickness);

            if (TextMaterial.Trim() != "")
            {
                archive.textMaterial = Int32.Parse(TextMaterial.Trim());//文字材料

            }
            archive.archivesTitle = archivesTitle;//案卷提名
            archive.firstResponsible = firstResponsible;//第一负责人

            if (drawing.Trim() != "")
            {
                archive.drawing = Int32.Parse(drawing.Trim());//图纸
            }
            if (PhotoCount.Trim() != "")//照片
            {
                archive.photoCount = Int32.Parse(PhotoCount.Trim());
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
            archive.shizhengNo = shizhengNo; //市政档案号

            archive.remarks = remarks;//备注
            string strbiaoyinriqi = indexeDate;//标引日期
            archive.indexDate = DateTime.Parse(strbiaoyinriqi);
            archive.indexer = indexer;//标引员


            string strshenheriqi = checkDate;//审核日期
            archive.checkDate = DateTime.Parse(strshenheriqi);
            archive.checker = checker;//审核员

            archive.kaigongTime = kaigongTime.Trim();//开工日期
            archive.jungongTime = jungongTime.Trim();//竣工日期
            archive.fazhaoTime = fazhaoTime.Trim();//发照日期
            if (jgDate != null)
            {
                archive.jgDate = DateTime.Parse(jgDate.Trim());
            }
            string strlururiqi = TyperDate;//录入日期
            if (strlururiqi != null)
            {
                archive.typerDate = DateTime.Parse(strlururiqi);
            }
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
                cb.Entry(archive1).State = EntityState.Modified;
                cb.gxArchivesDetail.Add(archive);
                    cb.SaveChanges();
            }

            //if (projects != null)
            //{
            //    if (projects.developmentOrganization.Trim() != developmentOrganization.Trim() || projects.constructionOrganization.Trim() != constructionOrganization.Trim() || projects.disignOrganization.Trim() != disignOrganization.Trim())
            //    {
            //        if (developmentOrganization.Trim() != "")
            //            projects.developmentOrganization = developmentOrganization.Trim();
            //        if (constructionOrganization.Trim() != "")
            //            projects.constructionOrganization = constructionOrganization.Trim();
            //        if (disignOrganization.Trim() != "")
            //            projects.disignOrganization = disignOrganization.Trim();

            //    }
            //    projects.securityID = securityName.ToString().Trim();
            //    projects.retentionPeriodNo = retentionPeriodName.ToString().Trim();
            //    projects.structureTypeID = structureTypeID.ToString().Trim();
            //    projects.newLocation = newLocation.Trim();
            //    if (ModelState.IsValid)
            //    {
            //        cb.Entry(projects).State = EntityState.Modified;
            //        cb.SaveChanges();
            //    }
            //}
            if (paperArchives != null)
            {
                string jinguandata = Convert.ToDateTime(paperArchives.jgDate).ToString("yyyy-MM-dd");
                if (jinguandata != jgDate.Trim())
                {
                    paperArchives.jgDate = DateTime.Parse(jgDate.Trim());
                }
                paperArchives.structureTypeID = structureTypeID.ToString().Trim();
                paperArchives.buildingArea = Convert.ToDouble((buildingArea.Trim()));
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
                paperArchives.archivesCount = (int.Parse(paperArchives.archivesCount) + 1).ToString();
                //int volCount = 0;
                //int.TryParse(paperArchives.archivesCount, out volCount);
                int vol = Convert.ToInt32(archive.volNo);

                if (vol == 1)
                {
                    paperArchives.shizhengNoStart = shizhengno;
                }
                if (vol == int.Parse(paperArchives.archivesCount))
                {
                    paperArchives.shizhengNoEnd = shizhengno;
                }
                
            if (ModelState.IsValid)
                {
                    cb.Entry(paperArchives).State = EntityState.Modified;
                    cb.SaveChanges();
                    return Content("<script >alert('保存成功！');window.location.href='/gxArchiveManagement/AddArchive';</script >");
                }
            }
            return View();

        }
    }
}