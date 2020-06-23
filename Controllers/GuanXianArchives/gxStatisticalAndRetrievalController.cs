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
using Newtonsoft.Json;

namespace urban_archive.Controllers
{
    public class gxStatisticalAndRetrievalController : Controller
    {
        // GET: StatisticalAndRetrieval
        private gxArchivesContainer bb = new gxArchivesContainer();
        private UrbanConEntities db = new UrbanConEntities();
        public ActionResult zhenglidayin(string action,string type = "PDF")
        {
            if (action == "打印")
            {
                LocalReport localReport = new LocalReport();
                string Person = Request.Form["collator"];
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                var ds = from ad in bb.vw_gxprojectList
                             //where ad.paperProjectSeqNo == 35
                         where ad.collator == Person
                         select ad;
                var ds1 = ds.Where(ad => ad.dateArchive >= DataFrom).Where(ad => ad.dateArchive <= DataTo);
                //var ds = db.vw_projectList.Where(ad => ad.paperProjectSeqNo > 20).Where(ad => ad.paperProjectSeqNo < 40);
                localReport.ReportPath = Server.MapPath("~/Report/guanxian/gerenzhenglidayin.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("gxgerenzhengli", ds1);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("Person", Person));
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
            if (action == "取消")
            {
                Response.Write("<script>window.close();</script>");
            }
            return View();
        }
        public ActionResult AllArchives(string SearchString, string type)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "项目顺序号", Value = "1"},
                new SelectListItem { Text = "建设单位", Value = "2" },
                new SelectListItem { Text = "建设单位联系人", Value = "3" },
                new SelectListItem { Text = "案卷题名", Value = "4" },
                new SelectListItem { Text = "设计单位", Value = "5" },
                new SelectListItem { Text = "施工单位", Value = "6" },
                new SelectListItem { Text = "工程地点", Value = "7" },
                new SelectListItem { Text = "最新工程地点", Value = "8" },
                new SelectListItem { Text = "档号", Value = "9" },
                new SelectListItem { Text = "分类号", Value = "10" },
                new SelectListItem { Text = "移交单位", Value = "11" },
                new SelectListItem { Text = "测绘单位", Value = "12" },
                 new SelectListItem { Text = "签订日期", Value = "13" },
                new SelectListItem { Text = "计划开工日期", Value = "14" },
                new SelectListItem { Text = "计划竣工日期", Value = "15" },
                new SelectListItem { Text = "负责人", Value = "16" },
                new SelectListItem { Text = "建设单位手机", Value = "17" },
                new SelectListItem { Text = "施工单位联系人", Value = "18" },
                new SelectListItem { Text = "工程序号", Value = "19" },

            };
            if (type == null | type == "")
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", 0);
            }
            else
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", type);
            }
            
            string n = Request.Form["SelectedID"];
            ViewBag.CurrentFilter = SearchString;
            var vwprojectFile = from ad in bb.vw_gxprojectList
                                where ad.classifyID == "1"
                                where ad.InchCountDetail != ""
                                select ad;
            if (!String.IsNullOrEmpty(SearchString))
            {
                int t = int.Parse(n);
                ViewBag.SelectedID = new SelectList(list, "Value", "Text",n);
                switch (t)
                {
                    case 0:
                        vwprojectFile = vwprojectFile.Where(ad => ad.projectName.Contains(SearchString));//根据责任书编号搜索
                        break;
                    case 1:
                        long paper = Convert.ToInt32(SearchString);
                        vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo == paper);//根据工程名称搜索
                        break;
                    case 2:
                        vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwprojectFile = vwprojectFile.Where(ad => ad.devolonpentOrgContacter.Contains(SearchString));//根据工程地点
                        break;
                    case 4:
                        var seq = bb.vw_gxpassList.Where(ad => ad.archivesTitle.Contains(SearchString)).First().paperProjectSeqNo;
                        vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo==seq); //根据工程序号
                        break;
                    case 5:
                        vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization.Contains(SearchString));//根据设计单位
                        break;
                    case 6:
                        vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位
                        break;
                    case 7:
                        vwprojectFile = vwprojectFile.Where(ad => ad.location.Contains(SearchString));//根据监理单位
                        break;
                    case 8:
                        vwprojectFile = vwprojectFile.Where(ad => ad.newLocation.Contains(SearchString));//根据监理单位
                        break;
                    case 9:

                        var archive = bb.vw_gxpassList.Where(ad => ad.archivesNo.Contains(SearchString)).First().paperProjectSeqNo;
                        vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo == archive); //根据工程序号
                        break;
                    case 10:

                        vwprojectFile = vwprojectFile.Where(ad => ad.prevClassNo==SearchString);//根据项目顺序号
                        break;
                    case 11:
                        vwprojectFile = vwprojectFile.Where(ad => ad.transferUnit.Contains(SearchString));//根据项目顺序号
                        break;
                    case 12:
                        vwprojectFile = vwprojectFile.Where(ad => ad.MapOrginisation.Contains(SearchString));//根据项目顺序号
                        break;
                    case 13:
                        DateTime time = Convert.ToDateTime(SearchString);
                        var project = bb.gxContractInfo.Where(a => a.dateSigned == time).First().projectName;
                        vwprojectFile = vwprojectFile.Where(ad => ad.projectName == project);//根据项目顺序号
                        break;
                    case 14:
                        DateTime time1 = Convert.ToDateTime(SearchString);
                        vwprojectFile = vwprojectFile.Where(ad => ad.dateF == time1);//根据项目顺序号
                        break;
                    case 15:
                        DateTime time2 = Convert.ToDateTime(SearchString);
                        vwprojectFile = vwprojectFile.Where(ad => ad.dateE == time2);//根据项目顺序号
                        break;
                    case 16:
                        vwprojectFile = vwprojectFile.Where(ad => ad.telphoneNoDevelopment.Contains(SearchString));//根据项目顺序号
                        break;
                    case 17:
                        vwprojectFile = vwprojectFile.Where(ad => ad.mobilephoneNoDevelopment.Contains(SearchString));//根据项目顺序号
                        break;
                    case 18:
                        vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrgContacter.Contains(SearchString));//根据项目顺序号
                        break;
                    case 19:
                        long paperNo = Convert.ToInt32(SearchString);
                        vwprojectFile = vwprojectFile.Where(ad => ad.projectNo == paperNo);//根据工程序号
                        break;
                }
            }
            // 默认按责任书编号排
            vwprojectFile = vwprojectFile.OrderBy(s => s.paperProjectSeqNo);
            ViewBag.result = JsonConvert.SerializeObject(vwprojectFile);
            ViewBag.count = vwprojectFile.Count();
            return View();
        }
        public ActionResult AllArchivesSZ(string SearchString)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "项目顺序号", Value = "1"},
                new SelectListItem { Text = "建设单位", Value = "2" },
                new SelectListItem { Text = "建设单位联系人", Value = "3" },
                new SelectListItem { Text = "案卷题名", Value = "4" },
                new SelectListItem { Text = "设计单位", Value = "5" },
                new SelectListItem { Text = "施工单位", Value = "6" },
                new SelectListItem { Text = "工程地点", Value = "7" },
                new SelectListItem { Text = "最新工程地点", Value = "8" },
                new SelectListItem { Text = "档号", Value = "9" },
                new SelectListItem { Text = "分类号", Value = "10" },
                new SelectListItem { Text = "移交单位", Value = "11" },
                new SelectListItem { Text = "测绘单位", Value = "12" },
                 new SelectListItem { Text = "签订日期", Value = "13" },
                new SelectListItem { Text = "计划开工日期", Value = "14" },
                new SelectListItem { Text = "计划竣工日期", Value = "15" },
                new SelectListItem { Text = "负责人", Value = "16" },
                new SelectListItem { Text = "建设单位手机", Value = "17" },
                new SelectListItem { Text = "施工单位联系人", Value = "18" },
                new SelectListItem { Text = "工程序号", Value = "19" },

            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            string n = Request.Form["SelectedID"];
            ViewBag.CurrentFilter = SearchString;
            var vwprojectFile = from ad in bb.vw_gxprojectList
                                where ad.classifyID=="2"
                                where ad.InchCountDetail!=""
                                select ad;
            if (!String.IsNullOrEmpty(SearchString))
            {
                int t = int.Parse(n);
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", n);
                switch (t)
                {
                    case 0:
                        vwprojectFile = vwprojectFile.Where(ad => ad.projectName.Contains(SearchString));//根据工程序号
                        break;
                    case 1:
                        long paper = Convert.ToInt32(SearchString);
                        vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo == paper);//根据项目顺序号
                        break;
                    case 2:
                        vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwprojectFile = vwprojectFile.Where(ad => ad.devolonpentOrgContacter.Contains(SearchString));//根据建设单位联系人
                        break;
                    case 4:
                        var seq = bb.vw_gxpassList.Where(ad => ad.archivesTitle.Contains(SearchString)).First().paperProjectSeqNo;
                        vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo == seq); //根据案卷题名
                        break;
                    case 5:
                        vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization.Contains(SearchString));//根据设计单位
                        break;
                    case 6:
                        vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位
                        break;
                    case 7:
                        vwprojectFile = vwprojectFile.Where(ad => ad.location.Contains(SearchString));//根据工程地点
                        break;
                    case 8:
                        vwprojectFile = vwprojectFile.Where(ad => ad.newLocation.Contains(SearchString));//根据最新工程地点
                        break;
                    case 9:

                        var archive = bb.vw_gxpassList.Where(ad => ad.archivesNo.Contains(SearchString)).First().paperProjectSeqNo;
                        vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo == archive); //根据档号
                        break;
                    case 10:

                        vwprojectFile = vwprojectFile.Where(ad => ad.prevClassNo == SearchString);//根据分类号
                        break;
                    case 11:
                        vwprojectFile = vwprojectFile.Where(ad => ad.transferUnit.Contains(SearchString));//根据移交单位
                        break;
                    case 12:
                        vwprojectFile = vwprojectFile.Where(ad => ad.MapOrginisation.Contains(SearchString));//根据测绘单位
                        break;
                    case 13:
                        DateTime time = Convert.ToDateTime(SearchString);
                        var project = bb.gxContractInfo.Where(a => a.dateSigned == time).First().projectName;
                        vwprojectFile = vwprojectFile.Where(ad => ad.projectName == project);//根据签订日期
                        break;
                    case 14:
                        DateTime time1 = Convert.ToDateTime(SearchString);
                        vwprojectFile = vwprojectFile.Where(ad => ad.dateF == time1);//根据计划开工日期
                        break;
                    case 15:
                        DateTime time2 = Convert.ToDateTime(SearchString);
                        vwprojectFile = vwprojectFile.Where(ad => ad.dateE == time2);//根据计划竣工日期
                        break;
                    case 16:
                        vwprojectFile = vwprojectFile.Where(ad => ad.telphoneNoDevelopment.Contains(SearchString));//根据负责人
                        break;
                    case 17:
                        vwprojectFile = vwprojectFile.Where(ad => ad.mobilephoneNoDevelopment.Contains(SearchString));//根据建设单位手机
                        break;
                    case 18:
                        vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrgContacter.Contains(SearchString));//根据施工单位联系人
                        break;
                    case 19:
                        long paperNo = Convert.ToInt32(SearchString);
                        vwprojectFile = vwprojectFile.Where(ad => ad.projectNo == paperNo);//根据工程序号
                        break;
                }
            }
            // 默认按责任书编号排
            vwprojectFile = vwprojectFile.OrderBy(s => s.paperProjectSeqNo);
            ViewBag.result = JsonConvert.SerializeObject(vwprojectFile);
            return View();
        }
        public ActionResult SeeReceive(long? id, string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false"},
                new SelectListItem { Text = "是", Value = "true"},
            };
            
            var test = from ad in bb.vw_gxprojectProfile
                       where (ad.projectID == id)
                       select ad;
            ViewBag.yidi = new SelectList(list, "Value", "Text", test.First().isYD);
            vw_gxprojectProfile projectProfile = test.First();
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
            if (action == "返回")
            {
                //return RedirectToAction("StatisticalAndAnalysis");
                return RedirectToAction("AllArchives");
            }
            if (action == "文件下载")
            {

                return RedirectToAction("DownLoadFile", "ProjectManagement", new { id = id });
            }
            if (projectProfile == null)
            {
                return HttpNotFound();
            }
            return View(projectProfile);

        }
        public ActionResult SeeArchives(long? id,string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (action == "返回")
            {
                //return RedirectToAction("StatisticalAndAnalysis");
                return RedirectToAction("AllArchives");
            }
            if(id==0)
            {
                return Content("<script >alert('此工程尚未录入案卷');window.history.back();</script >");

            }
            
            var archive = from a in bb.gxArchivesDetail
                          where a.paperProjectSeqNo == id
                          orderby a.volNo
                          select a;
            if (archive == null)
            {
                return HttpNotFound();
            }
            ViewBag.result = JsonConvert.SerializeObject(archive);
            return View();



        }
        public ActionResult SeeSettle(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var test = from ad in bb.vw_gxarchiveQueryList
                       where (ad.projectID == id)
                       select ad;
            vw_gxarchiveQueryList archive = test.First();
            if (archive == null)
            {
                return HttpNotFound();
            }
           
            return View();
        }
        [HttpPost]
        public ActionResult SeeSettle([Bind(Include = "InchCountDetail,characterVolumeCount,character2cm,character3cm,character4cm,character5cm,originalVolumeCount,originalInchCount,drawingVolumeCount,drawing2cm,drawing3cm,drawing4cm,drawing5cm,copyInchCount")] vw_archiveQueryList archiveQueryList, string action, long? id)
        {

            var paper = from ad in bb.gxPaperArchives
                        where (ad.projectID == id)
                        select ad;
            gxPaperArchives paperArchive = paper.First();
            var project = from ac in bb.gxProjectInfo
                          where (ac.projectID == id)
                          select ac;
            gxProjectInfo projectinfo = project.First();
            paperArchive.InchCountDetail = archiveQueryList.InchCountDetail;

            paperArchive.character2cm = archiveQueryList.character2cm;
            paperArchive.character3cm = archiveQueryList.character3cm;
            paperArchive.character4cm = archiveQueryList.character4cm;
            paperArchive.character5cm = archiveQueryList.character5cm;
            //ViewData["characterVolumeCount"] = archiveQueryList.character2cm + archiveQueryList.character3cm + archiveQueryList.character4cm + archiveQueryList.character5cm;
            paperArchive.characterVolumeCount = archiveQueryList.character2cm + archiveQueryList.character3cm + archiveQueryList.character4cm + archiveQueryList.character5cm;

            paperArchive.drawing2cm = archiveQueryList.drawing2cm;
            paperArchive.drawing3cm = archiveQueryList.drawing3cm;
            paperArchive.drawing4cm = archiveQueryList.drawing4cm;
            paperArchive.drawing5cm = archiveQueryList.drawing5cm;
            //ViewData["drawingVolumeCount"] = archiveQueryList.drawing2cm + archiveQueryList.drawing3cm + archiveQueryList.drawing4cm + archiveQueryList.drawing5cm;
            paperArchive.drawingVolumeCount = archiveQueryList.drawing2cm + archiveQueryList.drawing3cm + archiveQueryList.drawing4cm + archiveQueryList.drawing5cm;
            //ViewData["originalVolumeCount"] = paperArchive.characterVolumeCount + paperArchive.drawingVolumeCount;
            //ViewData["originalInchCount"] = (archiveQueryList.character2cm + archiveQueryList.drawing2cm) * 2 + (archiveQueryList.character3cm + archiveQueryList.drawing3cm) * 3 + (archiveQueryList.character4cm + archiveQueryList.drawing4cm) * 4 + (archiveQueryList.character5cm + archiveQueryList.drawing5cm) * 5;
            paperArchive.originalVolumeCount = archiveQueryList.character2cm + archiveQueryList.character3cm + archiveQueryList.character4cm + archiveQueryList.character5cm + archiveQueryList.drawing2cm + archiveQueryList.drawing3cm + archiveQueryList.drawing4cm + archiveQueryList.drawing5cm;
            paperArchive.originalInchCount = (archiveQueryList.character2cm + archiveQueryList.drawing2cm) * 2 + (archiveQueryList.character3cm + archiveQueryList.drawing3cm) * 3 + (archiveQueryList.character4cm + archiveQueryList.drawing4cm) * 4 + (archiveQueryList.character5cm + archiveQueryList.drawing5cm) * 5;
            paperArchive.copyInchCount = archiveQueryList.copyInchCount;
            paperArchive.archivesCount = (paperArchive.characterVolumeCount + paperArchive.drawingVolumeCount).ToString();

            if (action == "修改")
            {
                if(Convert.ToInt32(projectinfo.status)==3|| Convert.ToInt32(projectinfo.status)==5)
                {
                    ViewData["onlyread"] = false;
                }
                else
                {
                    ViewData["onlyread"] = true;
                }
                if (Convert.ToInt32(projectinfo.status) == 4)
                {
                    projectinfo.status = "9";
                }

                if (ModelState.IsValid)
                {
                    bb.Entry(paperArchive).State = EntityState.Modified;
                    bb.Entry(projectinfo).State = EntityState.Modified;
                    bb.SaveChanges();
                    return RedirectToAction("informationzhengli", "gxPaperSettle");
                }
            }
           

            if (action == "返回")
            {
                return RedirectToAction("StatisticalAndAnalysis");
            }
            return View();
        }
        public ActionResult StatisticalAndAnalysis()
        {
            ViewData["pagename"] = "gxStatisticalAndRetrieval/StatisticalAndAnalysis";

            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "全部", Value = "0"},
                new SelectListItem { Text = "通过审核", Value = "1"},
                new SelectListItem { Text = "整理", Value = "2" },
                new SelectListItem { Text = "编号", Value = "3" },
                new SelectListItem { Text = "录入", Value = "4" },
                new SelectListItem { Text = "等待入库", Value = "5" },
              
            };

            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值

            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            var vwprojectFile = from ad in bb.vw_gxprojectList
                                orderby ad.paperProjectSeqNo descending
                                select ad;
            ViewBag.result1 = JsonConvert.SerializeObject(vwprojectFile);
            return View();

        }
        [HttpPost]
        public ActionResult StatisticalAndAnalysis(string currentFilter, int? page, int? SelectedID, string startdate, string enddate)
        {
            ViewData["pagename"] = "StatisticalAndRetrieval/StatisticalAndAnalysis";

            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "全部", Value = "0"},
                new SelectListItem { Text = "通过审核", Value = "1"},
                new SelectListItem { Text = "整理", Value = "2" },
                new SelectListItem { Text = "编号", Value = "3" },
                new SelectListItem { Text = "录入", Value = "4" },
                new SelectListItem { Text = "等待入库", Value = "5" },

            };

            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值

            ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);
            ViewData["start"] = startdate;
            ViewData["end"] = enddate;



            int t = SelectedID.GetValueOrDefault();
            

            var vwprojectFile = from ad in bb.vw_gxprojectList
                                select ad;
            switch (t)
            {
                case 0:

                    vwprojectFile = from a in bb.vw_gxprojectList
                                    select a;
                    break;
                case 1:

                    vwprojectFile = vwprojectFile.Where(ad => ad.status == "3");
                    break;
                case 2:
                    vwprojectFile = vwprojectFile.Where(ad => ad.status == "4");
                    break;
                case 3:
                    vwprojectFile = vwprojectFile.Where(ad => ad.status == "5");
                    break;
                case 4:

                    vwprojectFile = vwprojectFile.Where(ad => ad.status == "6");
                    break;
                case 5:

                    vwprojectFile = vwprojectFile.Where(ad => ad.status == "10");
                    break;



            }

            if (startdate != "" && startdate != null && enddate != "" && enddate != null)//增加查询条件
            {
                DateTime start = DateTime.Parse(startdate.Trim());
                DateTime end = DateTime.Parse(enddate.Trim());
                vwprojectFile = from h in bb.vw_gxprojectList
                                where h.dateReceived > start && h.dateReceived < end
                                select h;
            }
            // 默认按责任书编号排
            ViewBag.result1 = JsonConvert.SerializeObject(vwprojectFile);
            return View();
        }
    }
}