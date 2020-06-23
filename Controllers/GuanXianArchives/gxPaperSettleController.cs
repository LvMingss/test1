using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using urban_archive.Models;
using PagedList;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Data.OleDb;
using System.Data.SqlClient;
namespace urban_archive.Controllers
{
    public class gxPaperSettleController : Controller
    {
        private UrbanConEntities db1 = new UrbanConEntities();
        private UrbanUsersEntities ab1 = new UrbanUsersEntities();
        private gxArchivesContainer cb = new gxArchivesContainer();
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        public ActionResult zhenglidayin(string action,long? id2, string type = "PDF")
        {
            if (action == "打印外部档案整理")
            {
                LocalReport localReport = new LocalReport();
                string Person = Request.Form["collator"];
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                var ds = from ad in cb.vw_gxprojectList
                             //where ad.paperProjectSeqNo == 35
                         where ad.collator == Person
                         where ad.isNB=="外部"
                         select ad;
                var ds1 = ds.Where(ad => ad.dateArchive >= DataFrom).Where(ad => ad.dateArchive <= DataTo).OrderBy(ad => ad.paperProjectSeqNo);
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
            if (action == "打印内部档案整理")
            {
                LocalReport localReport = new LocalReport();
                string Person = Request.Form["collator"];
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                var ds = from ad in cb.vw_gxprojectList
                             //where ad.paperProjectSeqNo == 35
                         where ad.collator == Person
                         where ad.isNB == "内部"
                         select ad;
                var ds1 = ds.Where(ad => ad.dateArchive >= DataFrom).Where(ad => ad.dateArchive <= DataTo).OrderBy(ad=>ad.paperProjectSeqNo);
                //var ds = db.vw_projectList.Where(ad => ad.paperProjectSeqNo > 20).Where(ad => ad.paperProjectSeqNo < 40);
                localReport.ReportPath = Server.MapPath("~/Report/guanxian/gerenzhenglidayinNB.rdlc");
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

                return RedirectToAction("Createinformationzhengli", "gxPaperSettle", new { id = id2,id2=""});
                
            }
            return View("zhenglidayin");
        }
        // GET: PaperSettle
        public ActionResult linquzhengli(string SearchString)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程序号", Value = "1"},
                new SelectListItem { Text = "建设单位", Value = "2" },
                new SelectListItem { Text = "施工单位", Value = "3" },
                new SelectListItem { Text = "接收人", Value = "4" },
                new SelectListItem { Text = "接受日期", Value = "5" },


            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            string n = Request.Form["SelectedID"];
            ViewBag.CurrentFilter = SearchString;
            //var vwgxprojectlist = from ad in cb.vw_gxprojectList
            //                      where ad.status == "3"
            //                      select ad;
            var vwgxprojectlist = cb.vw_gxprojectList.Where(a => a.status == "3").Where(a => a.isNB == "外部");

            if (!String.IsNullOrEmpty(SearchString))
            {
                int t=int.Parse(n);
                switch (t)
                {
                    case 0:
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 1:
                        long search = Convert.ToInt32(SearchString);
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.projectNo == search);//根据地点搜索
                        break;
                    case 2:

                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.developmentOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                        break;
                    case 4:

                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.recipient.Contains(SearchString));//根据工程序号搜索
                        break;
                    case 5:
                        DateTime date = DateTime.Parse(SearchString);
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.dateReceived == date);//根据责任书编号搜索
                        break;

                }

            }



            vwgxprojectlist = vwgxprojectlist.OrderBy(s => s.projectNo);// 默认按项目顺序号排列
            ViewBag.result = JsonConvert.SerializeObject(vwgxprojectlist);
            return View();

           
        }
        public ActionResult lingqu(int ? id, int classify)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string classid = classify.ToString();
            var test = from ad in cb.vw_gxarchiveQueryList
                       where (ad.projectNo == id)
                       where ad.classifyID== classid
                       select ad;
            vw_gxarchiveQueryList archive = test.First();
            //公元2016年12月7日修改,领取时间应为现在的时间
            if (archive == null)
            {
                return HttpNotFound();
            }

            string lqdate = DateTime.Today.Date.ToString("yyyy-MM-dd");
            //archive.l = DateTime.ParseExact(lqdate.Trim(), "yyyy-MM-dd", null).Date;
            var UserID = User.Identity.GetUserId();
            var users1 = ab.AspNetUsers.Find(UserID).UserName;
            ViewBag.collator = new SelectList(ab.AspNetUsers, "UserName", "UserName",users1);
            ViewBag.lqdate = lqdate;
            ViewData["style"] = "display:none;color:crimson";
            return View(archive);
        }
        [HttpPost]
        public ActionResult lingqu(int? id, int classify, string action)
        {
            string callator = Request.Form["collator"];
            string lqDate = Request.Form["lqDate"];
            string classid = classify.ToString();
            var test = from ad in cb.vw_gxarchiveQueryList
                       where (ad.projectNo == id)
                       where ad.classifyID==classid
                       select ad;
            
            
            //var gxpaperarchive = from ad in cb.gxPaperArchives
            //                   where (ad.projectNo == projectno)
            //                   where ad.classifyID==classid
            //                   select ad;
            var gxpaperarchive = cb.gxPaperArchives.Where(a => a.projectNo == id).Where(a => a.classifyID == classid).ToList();
            long? id1 = gxpaperarchive.First().projectID;
            long? id2 = gxpaperarchive.Last().projectID;

            //var gxprojectinfo = from ad in cb.gxProjectInfo
            //                    where ((ad.projectID == id1)||(ad.projectID==id2))
            //                    select ad;
            var gxprojectinfo = cb.gxProjectInfo.Where(ad => (ad.projectID == id1 || ad.projectID == id2)).ToList();
            ViewData["style"] = "display:none;color:crimson";
            var UserID = User.Identity.GetUserId();
            var users1 = ab.AspNetUsers.Find(UserID).UserName;
            ViewBag.collator = new SelectList(ab.AspNetUsers, "UserName", "UserName", users1);
            if (action == "确定")
            {
                if (test.First() != null)
                {
                    if (lqDate == "")
                    {
                        ViewData["style"] = "display:block;color:crimson";
                    }
                    else
                    {
                        ViewData["style"] = "display:none;color:crimson";
                        for (int i = 0; i < gxprojectinfo.Count(); i++)
                        {
                            gxprojectinfo[i].status = "4";
                            gxprojectinfo[i].isLingquYijiaoshu = true;
                            gxprojectinfo[i].isFafangHegezheng = true;
                            cb.Entry(gxprojectinfo[i]).State = EntityState.Modified;
                        }
                        for (int i = 0; i < gxpaperarchive.Count(); i++)
                        {
                            gxpaperarchive[i].lqDate = Convert.ToDateTime(lqDate);
                            gxpaperarchive[i].collator = callator;
                            cb.Entry(gxpaperarchive[i]).State = EntityState.Modified;
                        }
                        
                        

                        cb.SaveChanges();
                        return Content("<script >alert('领取成功！');window.location.href='/gxPaperSettle/linquzhengli';</script >");
                    }
                }
            }
            if (action == "返回")
            {
                return RedirectToAction("linquzhengli");
            }

            return View();
        }
        public ActionResult informationzhengli(string SearchString)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程序号", Value = "1"},
                new SelectListItem { Text = "设计单位", Value = "2" },
                new SelectListItem { Text = "施工单位", Value = "3" },
                new SelectListItem { Text = "接收人", Value = "4" },
                new SelectListItem { Text = "接受日期", Value = "5" },
                new SelectListItem { Text = "整理人", Value = "6" },
                new SelectListItem { Text = "整理日期", Value = "7" },

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
            //var vwgxprojectlist = cb.vw_gxprojectList.Where(a => a.status == "4").Where(a=>a.collator==name).Select(a => new { a.classifyID, a.projectName, a.projectNo, a.developmentOrganization, a.constructionOrganization, a.recipient, a.dateReceived,a.lqDate,a.collator, a.disignOrganization }).Distinct();

            var vwgxprojectlist = from ad in cb.vw_gxprojectList
                                  where ad.status == "4"
                                  where ad.collator == name
                                  select ad;
            if (name == "业务科"||name=="管线科")
            {
                vwgxprojectlist = from ad in cb.vw_gxprojectList
                                  where ad.status == "4"
                                  select ad;
                //vwgxprojectlist =cb.vw_gxprojectList.Where (a=>a.status=="4").Select(a => new { a.classifyID, a.projectName, a.projectNo, a.developmentOrganization, a.constructionOrganization, a.recipient, a.dateReceived, a.lqDate, a.collator,a.disignOrganization }).Distinct();
            }

            if (!String.IsNullOrEmpty(SearchString))
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text",n);
                int t = int.Parse(n);
                switch (t)
                {
                    case 0:
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 1:
                        long search = Convert.ToInt32(SearchString);
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.projectNo == search);//根据地点搜索
                        break;
                    case 2:

                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.disignOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                        break;
                    case 4:

                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.recipient.Contains(SearchString));//根据工程序号搜索
                        break;
                    case 5:
                        DateTime date = DateTime.Parse(SearchString);
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.dateReceived == date);//根据责任书编号搜索
                        break;
                    case 6:

                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.collator.Contains(SearchString));//根据责任书编号搜索
                        break;
                    case 7:
                        DateTime date2 = DateTime.Parse(SearchString);
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.lqDate == date2);//根据责任书编号搜索
                        break;

                }

            }



            vwgxprojectlist = vwgxprojectlist.OrderBy(s => s.projectNo);// 默认按项目顺序号排列
            ViewBag.result = JsonConvert.SerializeObject(vwgxprojectlist);

            return View();
        }
        public ActionResult Createinformationzhengli(long? id, int? id2, string id1)
        {
            if (id1 == "外部")
            {
                ViewBag.display = "display:inline";
                ViewBag.display1 = "display:none";
            }
            ViewBag.display1 = "display:none";
            if (id1 == "内部")
            {
                ViewBag.display = "display:none";
                ViewBag.display1 = "display:inline";
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var test = from ad in cb.vw_gxarchiveQueryList
                       where (ad.projectID == id)
                       select ad;
            vw_gxarchiveQueryList gxarchive = test.First();
            var name = User.Identity.GetUserId();
            var username = ab.AspNetUsers.Where(a => a.Id == name).First().UserName;
            ViewBag.collator = new SelectList(ab.AspNetUsers, "UserName", "UserName", test.First().collator);
            ViewBag.character1cm = gxarchive.character1cm;
            ViewBag.character2cm = gxarchive.character2cm;
            ViewBag.character3cm = gxarchive.character3cm;
            ViewBag.character4cm = gxarchive.character4cm;
            ViewBag.character5cm = gxarchive.character5cm;
            ViewBag.drawing1cm = gxarchive.drawing1cm;
            ViewBag.drawing2cm = gxarchive.drawing2cm;
            ViewBag.drawing3cm = gxarchive.drawing3cm;
            ViewBag.drawing4cm = gxarchive.drawing4cm;
            ViewBag.drawing5cm = gxarchive.drawing5cm;
            ViewData["id"] = id;
            if (id2 == 1)
            {
                ViewData["coding1"] = "none";
                ViewData["coding3"] = "none";
                ViewData["id2"] = 2;
            }
            else
            {
                if (Convert.ToInt32(gxarchive.status) == 4)
                {

                    ViewData["coding1"] = "none";
                    ViewData["coding3"] = "none";
                }
                if (Convert.ToInt32(gxarchive.status) == 9)
                {

                    ViewData["coding1"] = "none";

                }
            }


            if (gxarchive == null)
            {
                return HttpNotFound();
            }

            return View(gxarchive);
        }
        [HttpPost]

        public ActionResult Createinformationzhengli(string InchCountDetail, long? characterVolumeCount, long? character1cm, long? character2cm, long? character3cm, long? character4cm, long? character5cm, long? originalVolumeCount, long? originalInchCount, long? drawingVolumeCount, long? drawing1cm, long? drawing2cm, long? drawing3cm, long? drawing4cm, long? drawing5cm, long? copyInchCount, string action, long? id,string id2,string collator)
        {

            //公元2016.12.7修改
            ViewBag.collator = new SelectList(ab.AspNetUsers, "UserName", "UserName", collator);
            var paper = from ad in cb.gxPaperArchives
                        where (ad.projectID == id)
                        select ad;
            gxPaperArchives gxpaperArchive = paper.First();
            var gxproject = from ac in cb.gxProjectInfo
                          where (ac.projectID == id)
                          select ac;
            gxProjectInfo gxprojectinfo = gxproject.First();
           
            if (action == "保存")
            {
                if (character1cm == null)
                {
                    character1cm = 0;
                }
                if (character2cm == null)
                {
                    character2cm = 0;
                }
                if (character3cm == null)
                {
                    character3cm = 0;
                }
                if (character4cm == null)
                {
                    character4cm = 0;

                }
                if (character5cm == null)
                {
                    character5cm = 0;
                }
                if (characterVolumeCount == null)
                {
                    characterVolumeCount = 0;
                }
                if (drawing1cm == null)
                {
                    drawing1cm = 0;

                }
                if (drawing2cm == null)
                {
                    drawing2cm = 0;

                }
                if (drawing3cm == null)
                {
                    drawing3cm = 0;
                }
                if (drawing4cm == null)
                {
                    drawing4cm = 0;
                }
                if (drawing5cm == null)
                {
                    drawing5cm = 0;
                }
                if (drawingVolumeCount == null)
                {
                    drawingVolumeCount = 0;
                }
                if (originalVolumeCount == null)
                {
                    originalVolumeCount = 0;
                }
                if (originalInchCount == null)
                {
                    originalInchCount = 0;

                }
                if (copyInchCount == null)
                {
                    copyInchCount = 0;
                }
                //if (gxproject.First().isNB.Trim()=="外部")
                //{
                //    InchCountDetail = InchCountDetail + ",1*1";
                //    drawing1cm = drawing1cm + 1;
                //    originalVolumeCount = originalVolumeCount + 1;
                //    originalInchCount = originalInchCount + 1;//根据管线内外部的要求，内部档案算作一卷，参与编号
                //    drawingVolumeCount = drawingVolumeCount + 1;
                //}
                gxpaperArchive.InchCountDetail = InchCountDetail;
                gxpaperArchive.character1cm = character1cm.ToString();
                gxpaperArchive.character2cm = character2cm;
                gxpaperArchive.character3cm = character3cm;
                gxpaperArchive.character4cm = character4cm;
                gxpaperArchive.character5cm = character5cm;
                gxpaperArchive.characterVolumeCount = characterVolumeCount;
                gxpaperArchive.drawing1cm = drawing1cm.ToString();
                gxpaperArchive.drawing2cm = drawing2cm;
                gxpaperArchive.drawing3cm = drawing3cm;
                gxpaperArchive.drawing4cm = drawing4cm;
                gxpaperArchive.drawing5cm = drawing5cm;
                gxpaperArchive.drawingVolumeCount = drawingVolumeCount;
                gxpaperArchive.originalVolumeCount = originalVolumeCount;
                gxpaperArchive.originalInchCount = originalInchCount;
                gxpaperArchive.copyInchCount = copyInchCount;
                gxpaperArchive.archivesCount = (gxpaperArchive.characterVolumeCount + gxpaperArchive.drawingVolumeCount).ToString();
                gxpaperArchive.collator = collator;
                //if (gxproject.First().isNB.Trim() == "内部")
                //{
                //    gxpaperArchive.archivesCount = (gxpaperArchive.characterVolumeCount + gxpaperArchive.drawingVolumeCount).ToString();
                //}
                //验证公分明细数是否与在表格中填写的数据是否相同
                //if (InchCountDetail != "")
                //{
                //    int thickCnt = 0;
                //    int thickNess = 0;
                //    string ID = id.ToString().Trim();

                //    getCorrespondThick(InchCountDetail, ref thickCnt, ref thickNess);
                //    if (thickCnt != gxpaperArchive.originalVolumeCount)
                //    {
                //        return Content("<script >alert('公分数明细的案卷数与原件卷数不一致！');window.location.href='/PaperSettle/informationzhengli';</script >");

                //    }
                //    else if (thickNess != gxpaperArchive.originalInchCount)
                //    {
                //        return Content("<script >alert('公分数明细的案卷厚度与原件公分数不一致！');window.location.href='/PaperSettle/informationzhengli';</script >");
                //    }
                //    else
                //    {
                //        thickCnt = 0;
                //        thickNess = 0;
                //        thickCnt += Convert.ToInt32(character1cm + character2cm + character3cm + character4cm + character5cm);
                //        thickCnt += Convert.ToInt32(drawing1cm + drawing2cm + drawing3cm + drawing4cm + drawing5cm);
                //        if (thickCnt != originalVolumeCount)
                //        {
                //            return Content("<script >alert('文字与图纸的案卷数与原件卷数不一致！');window.location.href='/PaperSettle/informationzhengli';</script >");

                //        }
                //        thickNess = Convert.ToInt32((character1cm + drawing1cm) * 1 + (character2cm + drawing2cm) * 2 + (drawing3cm + character3cm) * 3 + (character4cm + drawing4cm) * 4 + (drawing5cm + character5cm) * 5);


                //        if (thickNess != originalInchCount)
                //        {
                //            return Content("<script >alert('文字与图纸的案卷厚度与原件公分数不一致！');window.location.href='/PaperSettle/informationzhengli';</script >");

                //        }
                //    }
                //}

                if (InchCountDetail != "")
                {
                    int thickCnt = 0;
                    int thickNess = 0;
                    string ID = id.ToString().Trim();
                    int index0 = InchCountDetail.IndexOf('，');
                    if (index0 != -1)
                    {
                        return Content("<script >alert('公分数明细输入格式有误（注意切换英文输入法）！');window.location.href='/gxPaperSettle/informationzhengli';</script >");
                    }
                    getCorrespondThick(InchCountDetail, ref thickCnt, ref thickNess);
                    if (thickCnt != gxpaperArchive.originalVolumeCount)
                    {
                        return Content("<script >alert('公分数明细的案卷数与原件卷数不一致！');window.location.href='/gxPaperSettle/informationzhengli';</script >");

                    }
                    else if (thickNess != gxpaperArchive.originalInchCount)
                    {
                        return Content("<script >alert('公分数明细的案卷厚度与原件公分数不一致！');window.location.href='/gxPaperSettle/informationzhengli';</script >");

                    }
                    else
                    {
                        thickCnt = 0;
                        thickNess = 0;
                        thickCnt += Convert.ToInt32(character1cm + character2cm + character3cm + character4cm + character5cm);
                        thickCnt += Convert.ToInt32(drawing1cm + drawing2cm + drawing3cm + drawing4cm + drawing5cm);
                        if (thickCnt != originalVolumeCount)
                        {
                            return Content("<script >alert('文字与图纸的案卷数与原件卷数不一致！');window.location.href='/gxPaperSettle/informationzhengli';</script >");

                        }
                        thickNess = Convert.ToInt32((character1cm + drawing1cm) * 1 + (character2cm + drawing2cm) * 2 + (drawing3cm + character3cm) * 3 + (character4cm + drawing4cm) * 4 + (drawing5cm + character5cm) * 5);


                        if (thickNess != originalInchCount)
                        {
                            return Content("<script >alert('文字与图纸的案卷厚度与原件公分数不一致！');window.location.href='/gxPaperSettle/informationzhengli';</script >");

                        }
                    }
                }
                else
                {
                    return Content("<script >alert('请输入公分明细数！');window.history.back();</script >");

                }



                if (Convert.ToInt32(gxprojectinfo.status) == 4)
                {
                    gxprojectinfo.status = "9";
                }
                //if (gxprojectinfo.isNB.Trim() == "内部")
                //{
                //    gxprojectinfo.status = "6";
                //}

                if (ModelState.IsValid)
                {
                    cb.Entry(gxpaperArchive).State = EntityState.Modified;
                    cb.Entry(gxprojectinfo).State = EntityState.Modified;
                    cb.SaveChanges();
                    return Content("<script >alert('保存成功！');window.history.back();</script >");
                }
                else
                {
                    return Content("<script >alert('保存失败！');window.history.back();</script >");
                }
            }
           
            if (action == "返回")
            {
                if (id2 == "2")
                {
                   
                    return RedirectToAction("AllArchives", "gxStatisticalAndRetrieval");
                }

                if (Convert.ToInt32(gxprojectinfo.status) == 4)
                {
                    return RedirectToAction("informationzhengli", "gxPaperSettle");
                }
                if (Convert.ToInt32(gxprojectinfo.status) == 9)
                {
                    return RedirectToAction("waitcode", "gxPaperSettle");
                }
                return RedirectToAction("AllArchives", "gxStatisticalAndRetrieval");

            }
            if (action == "我要编号")
            {
                if (character1cm == null)
                {
                    character1cm = 0;
                }
                if (character2cm == null)
                {
                    character2cm = 0;
                }
                if (character3cm == null)
                {
                    character3cm = 0;
                }
                if (character4cm == null)
                {
                    character4cm = 0;

                }
                if (character5cm == null)
                {
                    character5cm = 0;
                }
                if (characterVolumeCount == null)
                {
                    characterVolumeCount = 0;
                }
                if (drawing1cm == null)
                {
                    drawing1cm = 0;

                }
                if (drawing2cm == null)
                {
                    drawing2cm = 0;

                }
                if (drawing3cm == null)
                {
                    drawing3cm = 0;
                }
                if (drawing4cm == null)
                {
                    drawing4cm = 0;
                }
                if (drawing5cm == null)
                {
                    drawing5cm = 0;
                }
                if (drawingVolumeCount == null)
                {
                    drawingVolumeCount = 0;
                }
                if (originalVolumeCount == null)
                {
                    originalVolumeCount = 0;
                }
                if (originalInchCount == null)
                {
                    originalInchCount = 0;

                }
                if (copyInchCount == null)
                {
                    copyInchCount = 0;
                }
              
               
               
           
                gxpaperArchive.InchCountDetail = InchCountDetail;
                gxpaperArchive.character1cm = character1cm.ToString();
                gxpaperArchive.character2cm = character2cm;
                gxpaperArchive.character3cm = character3cm;
                gxpaperArchive.character4cm = character4cm;
                gxpaperArchive.character5cm = character5cm;
                gxpaperArchive.characterVolumeCount = characterVolumeCount;
                gxpaperArchive.drawing1cm = drawing1cm.ToString();
                gxpaperArchive.drawing2cm = drawing2cm;
                gxpaperArchive.drawing3cm = drawing3cm;
                gxpaperArchive.drawing4cm = drawing4cm;
                gxpaperArchive.drawing5cm = drawing5cm;
                gxpaperArchive.drawingVolumeCount = drawingVolumeCount;
                gxpaperArchive.originalVolumeCount = originalVolumeCount;
                gxpaperArchive.originalInchCount = originalInchCount;
                gxpaperArchive.copyInchCount = copyInchCount;
                gxpaperArchive.archivesCount = (gxpaperArchive.characterVolumeCount + gxpaperArchive.drawingVolumeCount).ToString();
                gxpaperArchive.collator = collator;
                //if (InchCountDetail == "")
                //{

                //    return Content("<script >alert('公分明细数不能为空！');window.location.href='/gxPaperSettle/informationzhengli';</script >");
                //}
                gxprojectinfo.status = "5";
                cb.Entry(gxprojectinfo).State = EntityState.Modified;
                cb.SaveChanges();
                return Content("<script >alert('已成功编号');window.location.href='/gxPaperSettle/waitcode';</script >");
            }
            if(action== "打印个人整理档案统计表")
            {
                return RedirectToAction("zhenglidayin",new {id1="",id2= id, id3 ="" });
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
                    string gfmxs = table.Rows[i][0].ToString().Trim(); //dr["公分数明细"].ToString().Trim();
                    if (gfmxs == "")
                    {
                        gfmxs = "";
                    }

                    string wwcm1 = table.Rows[i][1].ToString().Trim(); //dr["文字1厘米"].ToString().Trim();
                    int wcm1 = 0;
                    if (wwcm1 == "")
                    {
                        wcm1 = 0;
                    }
                    else
                    {
                        try
                        {
                            wcm1 = int.Parse(wwcm1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('文字1厘米应输入数字！');window.history.back();</script >");
                        }
                    }
                    string wwcm2 = table.Rows[i][2].ToString().Trim(); //dr["文字2厘米"].ToString().Trim();
                    int wcm2 = 0;
                    if (wwcm2 == "")
                    {
                        wcm2 = 0;
                    }
                    else
                    {
                        try
                        {
                            wcm2 = int.Parse(wwcm2);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('文字2厘米应输入数字！');window.history.back();</script >");
                        }
                    }
                    string wwcm3 = table.Rows[i][3].ToString().Trim(); //dr["文字3厘米"].ToString().Trim();
                    int wcm3 = 0;
                    if (wwcm3 == "")
                    {
                        wcm3 = 0;
                    }
                    else
                    {
                        try
                        {
                            wcm3 = int.Parse(wwcm3);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('文字3厘米应输入数字！');window.history.back();</script >");
                        }
                    }
                    string wwcm4 = table.Rows[i][4].ToString().Trim(); //dr["文字4厘米"].ToString().Trim();
                    int wcm4 = 0;
                    if (wwcm4 == "")
                    {
                        wcm4 = 0;
                    }
                    else
                    {
                        try
                        {
                            wcm4 = int.Parse(wwcm4);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('文字4厘米应输入数字！');window.history.back();</script >");
                        }
                    }
                    string wwcm5 = table.Rows[i][5].ToString().Trim(); //dr["文字5厘米"].ToString().Trim();
                    int wcm5 = 0;
                    if (wwcm5 == "")
                    {
                        wcm5 = 0;
                    }
                    else
                    {
                        try
                        {
                            wcm5 = int.Parse(wwcm5);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('文字5厘米应输入数字！');window.history.back();</script >");
                        }
                    }

                    int wenzishu = wcm1 + wcm2 + wcm3 + wcm4 + wcm5;//dr["文字数"].ToString().Trim();

                    string ttcm1 = table.Rows[i][6].ToString().Trim(); //dr["图纸1厘米"].ToString().Trim();
                    int tcm1 = 0;
                    if (ttcm1 == "")
                    {
                        tcm1 = 0;
                    }
                    else
                    {
                        try
                        {
                            tcm1 = int.Parse(ttcm1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('图纸1厘米应输入数字！');window.history.back();</script >");
                        }
                    }
                    string ttcm2 = table.Rows[i][7].ToString().Trim(); //dr["图纸2厘米"].ToString().Trim();
                    int tcm2 = 0;
                    if (ttcm2 == "")
                    {
                        tcm2 = 0;
                    }
                    else
                    {
                        try
                        {
                            tcm2 = int.Parse(ttcm2);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('图纸2厘米应输入数字！');window.history.back();</script >");
                        }
                    }
                    string ttcm3 = table.Rows[i][8].ToString().Trim(); //dr["图纸3厘米"].ToString().Trim();
                    int tcm3 = 0;
                    if (ttcm3 == "")
                    {
                        tcm3 = 0;
                    }
                    else
                    {
                        try
                        {
                            tcm3 = int.Parse(ttcm3);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('图纸3厘米应输入数字！');window.history.back();</script >");
                        }
                    }
                    string ttcm4 = table.Rows[i][9].ToString().Trim(); //dr["图纸4厘米"].ToString().Trim();
                    int tcm4 = 0;
                    if (ttcm1 == "")
                    {
                        tcm4 = 0;
                    }
                    else
                    {
                        try
                        {
                            tcm4 = int.Parse(ttcm4);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('图纸4厘米应输入数字！');window.history.back();</script >");
                        }
                    }
                    string ttcm5 = table.Rows[i][10].ToString().Trim(); //dr["图纸5厘米"].ToString().Trim();
                    int tcm5 = 0;
                    if (ttcm5 == "")
                    {
                        tcm5 = 0;
                    }
                    else
                    {
                        try
                        {
                            tcm5 = int.Parse(ttcm5);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('图纸5厘米应输入数字！');window.history.back();</script >");
                        }
                    }

                    int tuzhishu = tcm1 + tcm2 + tcm3 + tcm4 + tcm5;//dr["图纸数"].ToString().Trim();

                    int yujianshu = wenzishu + tuzhishu;//dr["原件件数"].ToString().Trim();

                    int yujiangfshu = wcm1 * 1 + wcm2 * 2 + wcm3 * 3 + wcm4 * 4 + wcm5 * 5 + tcm1 * 1 + tcm2 * 2 + tcm3 * 3 + tcm4 * 4 + tcm5 * 5;//dr["原件公分数"].ToString().Trim();

                    string fuyingfshu1 = table.Rows[i][11].ToString().Trim(); //dr["复印公分数"].ToString().Trim();
                    int fuyingfshu = 0;
                    if (fuyingfshu1 == "")
                    {
                        fuyingfshu = 0;
                    }
                    else
                    {
                        try
                        {
                            fuyingfshu = int.Parse(fuyingfshu1);
                        }
                        catch (Exception ex)
                        {
                            return Content("<script >alert('复印公分数应输入数字！');window.history.back();</script >");
                        }
                    }

                    //string retentionPeriodNo1 = db.RetentionPeriod.Where(ad => ad.retentionPeriodName == retentionPeriodName1).First().retentionPeriodNo;
                    //string MJ1 = db.SecurityClassification.Where(ad => ad.securityName == MJ).First().securityID;
                    //string strSql1 = string.Format("insert into UrbanCon.dbo.PaperArchives(InchCountDetail,characterVolumeCount,character1cm,character2cm,character3cm,character4cm,character5cm,drawingVolumeCount,drawing1cm,drawing2cm,drawing3cm,drawing4cm,drawing5cm,originalVolumeCount,originalInchCount,copyInchCount) values('" + gfmxs + "'," + wenzishu + " ," + wcm1 + "," + wcm2 + "," + wcm3 + "," + wcm4 + "," + wcm5 + "," + tuzhishu + "," + tcm1 + "," + tcm2 + "," + tcm3 + "," + tcm4 + "," + tcm5 + "," + yujianshu + "," + yujiangfshu + "," + fuyingfshu + ")");
                    string strSql1 = string.Format("update UrbanCon.dbo.gxPaperArchives set InchCountDetail = '" + gfmxs + "',characterVolumeCount = " + wenzishu + ",character1cm=" + wcm1 + ",character2cm=" + wcm2 + ",character3cm=" + wcm3 + ",character4cm=" + wcm4 + ",character5cm=" + wcm5 + ",drawingVolumeCount=" + tuzhishu + ",drawing1cm=" + tcm1 + ",drawing2cm=" + tcm2 + ",drawing3cm=" + tcm3 + ",drawing4cm=" + tcm4 + ",drawing5cm=" + tcm5 + ",originalVolumeCount=" + yujianshu + ",originalInchCount=" + yujiangfshu + ",copyInchCount=" + fuyingfshu + "where projectID=" + id);
                    //string strSql2 = string.Format("update UrbanCon.dbo.PaperArchives set licenseNo='" + licenseNo1 + "',structureTypeID=(select structureTypeID from UrbanCon.dbo.StructureType where structureTypeName='" + structureTypeName1 + "'),buildingArea=" + buildingArea2 + ",firstResponsible='" + firstResponsible1 + "',responsibleOther='" + responsibleOther1 + "',transferUnit='" + transferUnit1 + "',textMaterial=" + textMaterial2 + ",drawing=" + drawing2 + ",photoCount=" + photoCount2 + ",jgDate='" + jgDate2 + "',height='" + height2 + "',changeLog='" + changeLog1 + "',remarks='" + remarks1 + "',overground='" + overground1 + "',underground='" + underground1 + "',archivesCount='" + archivesCount1 + "' where paperProjectSeqNo = " + paperProjectSeqNo2);
                    //string strSql2 = string.Format("update UrbanCon.dbo.ProjectInfo set projectName='" + ProjectName + "',location='"+ ProjectLocation + "',newLocation='"+ NewProjectLocation + "',securityID=(select securityID from UrbanCon.dbo.SecurityClassification where securityName='" + MJ + "'),isYD=" + yd1 + ",developmentOrganization='" + developmentOrganization1 + "',devolonpentOrgContacter='" + lxpeople + "',mobilephoneNoDevelopment='" + lxTelephone + "',telphoneNoDevelopment='" + lxFixTelephone + "',jianliUnit='"+ jlOrganization1 + "',jianliUnitContacter='"+ jlfzpeople + "',telphoneNoJianliUnit= '"+ telphoneNoJianliUnit + "',constructionOrganization='" + constructionOrganization1 + "',constructionOrgContacter='"+ fapeople + "',telphoneNoConstruction='"+ jishupeople + "',disignOrganization='" + disignOrganization1 + "',designOrgaContacter='" + sjfzpeople + "',telphoneNoDesignOrga='"+ telphoneNoDesignOrga + "',where paperProjectSeqNo = " + paperProjectSeqNo2);
                    //string strSql2 = string.Format("insert into UrbanCon.dbo.ProjectInfo(projectName,location,newLocation,retentionPeriodNo,securityID,isYD,developmentOrganization,devolonpentOrgContacter,mobilephoneNoDevelopment,telphoneNoDevelopment,jianliUnit,jianliUnitContacter,constructionOrganization,constructionOrgContacter,telphoneNoConstruction,disignOrganization,designOrgaContacter,telphoneNoDesignOrga,projectID,contractNo,status,mappipei) values('" + ProjectName + "','" + ProjectLocation + "','" + NewProjectLocation + "','" + retentionPeriodNo1 + "','" + MJ1 + "'," + yd1 + ",'" + developmentOrganization1 + "','" + lxpeople + "','" + lxTelephone + "','" + lxFixTelephone + "','" + jlOrganization1 + "','" + telphoneNoJianliUnit + "','" + constructionOrganization1 + "','" + fapeople + "','" + jishupeople + "','" + disignOrganization1 + "','" + sjfzpeople + "','" + telphoneNoDesignOrga + "'," + vwprojectProfile.projectID + ",'" + vwprojectProfile.contractNo + "','3',''" + ")");

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
                //return Content("<script >alert('导入成功！');window.history.back();</script >");
                return RedirectToAction("Createinformationzhengli", new { id = id, id2 = 0 });
            }
            return View();
        }
        public ActionResult waitcode(string SearchString)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程序号", Value = "1"},
                new SelectListItem { Text = "建设单位", Value = "2" },
                new SelectListItem { Text = "施工单位", Value = "3" },
                new SelectListItem { Text = "接收人", Value = "4" },
                new SelectListItem { Text = "接受日期", Value = "5" },
                new SelectListItem { Text = "整理人", Value = "6" },
                new SelectListItem { Text = "整理日期", Value = "7" },

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
            var vwgxprojectlist = cb.vw_gxprojectList.Where(a => a.status == "9").Where(a => a.collator == name).Where(a=>a.isNB=="外部");
            //var vwgxprojectlist = from ad in cb.vw_gxprojectList
            //                      where ad.status == "9"
            //                      where ad.collator == name
            //                      select ad;
            if (name == "业务科"||name=="管线科")
            {
                vwgxprojectlist =cb.vw_gxprojectList.Where(a => a.status == "9").Where(a => a.isNB == "外部");
            }

            if (!String.IsNullOrEmpty(SearchString))
            {
                int t = int.Parse(n);
                switch (t)
                {
                    
                    case 0:
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 1:
                        long search = Convert.ToInt32(SearchString);
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.projectNo == search);//根据地点搜索
                        break;
                    case 2:

                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.developmentOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                        break;
                    case 4:

                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.recipient.Contains(SearchString));//根据工程序号搜索
                        break;
                    case 5:
                        DateTime date = DateTime.Parse(SearchString);
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.dateReceived == date);//根据责任书编号搜索
                        break;
                    case 6:

                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.collator.Contains(SearchString));//根据责任书编号搜索
                        break;
                    case 7:
                        DateTime date2 = DateTime.Parse(SearchString);
                        vwgxprojectlist = vwgxprojectlist.Where(ad => ad.lqDate == date2);//根据责任书编号搜索
                        break;

                }

            }



            vwgxprojectlist = vwgxprojectlist.OrderBy(s => s.projectNo);// 默认按项目顺序号排列
            ViewBag.result = JsonConvert.SerializeObject(vwgxprojectlist);
            return View();

        }
        public ActionResult zhenglicount(int? page)
        {
           var vwprojectlist = from ad in db.vw_projectList
                                where ad.status == "9"
                                select ad;
            vwprojectlist = vwprojectlist.OrderBy(s => s.paperProjectSeqNo);// 默认按项目顺序号排列
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(vwprojectlist.ToPagedList(pageNumber, pageSize));
        }
        private void getCorrespondThick(string detail, ref int thickCnt, ref int thickness)
        {
            int ndotspace = 0;//0:逗号隔开，1：空格隔开
            if (detail == "")
            {

                thickCnt = 0;
                thickness = 0;
                return;
            }
            int index = detail.IndexOf(',');
            if (index == -1)
            {
                index = detail.IndexOf(' ');
                ndotspace = 1;
            }

            thickCnt = 0;//案卷数 
            thickness = 0;//案卷厚度
            if (index == -1)//字符串中没有逗号与空格
            {
                string[] strEnum = detail.Split('*');
                thickCnt += Int32.Parse(strEnum[1]);
                thickness += Int32.Parse(strEnum[0]) * Int32.Parse(strEnum[1]);
            }
            else//字符串中有逗号或空格
            {
                string[] strEnum;
                if (ndotspace == 0)
                {
                    strEnum = detail.Split(',');
                }
                else
                {
                    strEnum = detail.Split(' ');
                }
                foreach (string str in strEnum)
                {
                    string[] s = str.Split('*');
                    thickness += Int32.Parse(s[0]) * Int32.Parse(s[1]);
                    thickCnt += Int32.Parse(s[1]);
                }
            }
        }
   public ActionResult ZhengliTongji(long ?id)
        {
            //确定用户角色，根据角色选择要显示的工程信息
            string user = User.Identity.Name;
            ViewData["start"] = "请输入开始日期";
            ViewData["end"] = "请输入截止日期";
            var role = from c in ab.AspNetUsers
                       where c.UserName == user
                       select c.RoleName;
            var vwgxprojectlist = from ad in cb.vw_gxprojectProfile
                                where ad.status == "9" && ad.collator == user
                                  orderby ad.projectNo descending
                                select ad;
            if (role.First().ToString()!="科员"|| role.First().ToString()!="科长")
            {
                vwgxprojectlist = from ad in cb.vw_gxprojectProfile
                                    where ad.status == "9"
                                    orderby ad.projectNo descending
                                    select ad;
            }
              
            var users1 = from ad in ab.AspNetUsers
                         where ad.DepartmentId == 2
                         select ad;
            ViewBag.lingquzhengliren = new SelectList(users1, "UserName", "UserName",User.Identity.Name);
            foreach (var item in vwgxprojectlist)
            {
                int character1cm = int.Parse(item.character1cm);
                int drawing1cm = int.Parse(item.drawing1cm);
                item.status = "等待编号";
                item.copyInchCount = character1cm * 1 + item.character2cm*2 + item.character3cm*3 + item.character4cm*4 + item.character5cm*5;
                item.drawing = drawing1cm * 1 + item.drawing2cm*2 + item.drawing3cm*3 + item.drawing4cm*4 + item.drawing5cm*5;
            }
            ViewBag.result = JsonConvert.SerializeObject(vwgxprojectlist);
            return View();
        }
        [HttpPost]
        public ActionResult ZhengliTongji(long? id,string StartDate,string EndDate,string action,string lingquzhengliren)
        {

            var vwgxprojectlist = from ad in cb.vw_gxprojectProfile
                                where ad.status == "9" 
                                orderby ad.projectNo descending
                                select ad;
            var role = from c in ab.AspNetUsers
                       where c.UserName == lingquzhengliren
                       select c.RoleName;
            bool flag=false;//用于判断查询是是否要增加条件
            ViewData["start"] = StartDate;
            ViewData["End"] = EndDate;
            DateTime Start=Convert.ToDateTime("2007-8-1"), End= Convert.ToDateTime("2007-8-1");
            if (StartDate.Length==10&&EndDate.Length==10)
            {
                Start = DateTime.ParseExact(StartDate.Trim(), "yyyy-MM-dd", null);
                End = DateTime.ParseExact(EndDate.Trim(), "yyyy-MM-dd", null);
            }
            
            if (role.First().ToString()== "科员" || role.First().ToString() == "科长")
            {
                vwgxprojectlist = from ad in cb.vw_gxprojectProfile
                                where ad.status == "9" && ad.collator == lingquzhengliren/*&& ad.lqDate> Start && ad.lqDate< End*/
                                orderby ad.projectNo descending
                                select ad;
                flag = true;
            }
           


            var users1 = from ad in ab.AspNetUsers where ad.DepartmentId == 2
                         select ad;
            ViewBag.lingquzhengliren = new SelectList(users1, "UserName", "UserName", lingquzhengliren);
            foreach (var item in vwgxprojectlist)
            {
                int character1cm = int.Parse(item.character1cm);
                int drawing1cm = int.Parse(item.drawing1cm);
                item.status = "等待编号";
                item.copyInchCount = character1cm * 1 + item.character2cm * 2 + item.character3cm * 3 + item.character4cm * 4 + item.character5cm * 5;
                item.drawing = drawing1cm * 1 + item.drawing2cm * 2 + item.drawing3cm * 3 + item.drawing4cm * 4 + item.drawing5cm * 5;
            }
            if(action=="查询")
            {
                if (flag==true)
                {
                    vwgxprojectlist = from ad in vwgxprojectlist
                                    where  ad.collator == lingquzhengliren/* && ad.lqDate > Start && ad.lqDate < End*/
                                    orderby ad.projectNo descending
                                    select ad;
                }
                else
                {
                    vwgxprojectlist = from ad in vwgxprojectlist
                                      where ad.lqDate>= Start && ad.lqDate<=End
                                    orderby ad.projectNo descending
                                    select ad;
                }
            }
            ViewBag.result = JsonConvert.SerializeObject(vwgxprojectlist);
            return View();
        }
    }
}