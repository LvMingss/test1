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
namespace urban_archive.Controllers
{
    public class TuzhiArchivesController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult TuZhiMuLu(string action, string type = "PDF")
        {
            if (action == "打印图纸目录")
            {
                LocalReport localReport = new LocalReport();
                string SS = Request.Form["seqS"];
                string SE = Request.Form["seqE"];
                if (SS == "" && SE != "")
                {
                    Response.Write("<script>alert('起始序号不能为空!');</script>");
                }
                if (SS != "" && SE == "")
                {
                    Response.Write("<script>alert('终止序号不能为空!');</script>");
                }
                if (SS == "" && SE == "")
                {
                    Response.Write("<script>alert('请输入序号范围!');</script>");
                }
                if (SS != "" && SE != "")
                {
                    long SeqNoS = long.Parse(Request.Form["seqS"]);
                    long SeqNoE = long.Parse(Request.Form["seqE"]);
                    var ds1 = db.TuzhiArchives.Where(ad => ad.seqNo >= SeqNoS).Where(ad => ad.seqNo <= SeqNoE);
                    List<TuzhiArchives> list = ds1.ToList();
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].tuzhiYear != null)
                            list[i].tuzhiYear = list[i].tuzhiYear.ToString().Trim();
                        if (list[i].tuzhiStatus != null)
                            list[i].tuzhiStatus = list[i].tuzhiStatus.ToString().Trim();
                    }
                    var ds2 = list;
                    localReport.ReportPath = Server.MapPath("~/Report/TuZhiMuLu.rdlc");
                    ReportDataSource reportDataSource = new ReportDataSource("TuZhiMuLu", ds2);
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
            return View();
        }
        // GET: TuzhiArchives
        public ActionResult Index1(string type, string search)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "案卷题名", Value = "0"},
                new SelectListItem { Text = "分类号", Value = "1"},
                new SelectListItem { Text = "档号", Value = "2"},
                new SelectListItem { Text = "序号", Value = "3"},
                new SelectListItem { Text = "编制单位", Value = "4"},
                new SelectListItem { Text = "图纸年代", Value = "5"},
                new SelectListItem { Text = "进馆时间", Value = "6"},
                new SelectListItem { Text = "内容题要", Value = "7"},
                new SelectListItem { Text = "存放地点", Value = "8"},
                new SelectListItem { Text = "移交单位", Value = "9"},
                new SelectListItem { Text = "比例", Value = "10"},
                new SelectListItem { Text = "图幅", Value = "11"}
            };
            if (type == null | type == "")
            {
                ViewBag.Selected = new SelectList(list, "Value", "Text", 0);
            }
            else
            {
                ViewBag.Selected = new SelectList(list, "Value", "Text", type);
            }
            ViewBag.CurrentFilter = search;
            return View();
        }
        public ActionResult Index(string action, int? page)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "案卷题名", Value = "0"},
                new SelectListItem { Text = "分类号", Value = "1"},
                new SelectListItem { Text = "档号", Value = "2"},
                new SelectListItem { Text = "序号", Value = "3"},
                new SelectListItem { Text = "编制单位", Value = "4"},
                new SelectListItem { Text = "图纸年代", Value = "5"},
                new SelectListItem { Text = "进馆时间", Value = "6"},
                new SelectListItem { Text = "内容题要", Value = "7"},
                new SelectListItem { Text = "存放地点", Value = "8"},
                new SelectListItem { Text = "移交单位", Value = "9"},
                new SelectListItem { Text = "比例", Value = "10"},
                new SelectListItem { Text = "图幅", Value = "11"}
            };
            ViewBag.Selected = new SelectList(list, "Value", "Text", 0);
            var tuzhi = db.TuzhiArchives.OrderBy(a => a.ID);
            //int pageSize = 50;
            //int pageNumber = (page ?? 1);

            if (action == "查询")
            {
                string n = Request.Form["Selected"];
                string m = Request.Form["search"];
                if (n == "0")
                {
                    var chaxun = tuzhi.Where(a => a.archiveTitle.Contains(m)).OrderBy(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "1")
                {
                    var chaxun = tuzhi.Where(a => a.classNo.Contains(m)).OrderBy(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "2")
                {
                    var chaxun = tuzhi.Where(a => a.archiveNo.Contains(m)).OrderBy(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "3")
                {
                    var chaxun = tuzhi.Where(a => a.seqNo.ToString().Contains(m)).OrderBy(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "4")
                {
                    var chaxun = tuzhi.Where(a => a.measureUnit.Contains(m)).OrderBy(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "5")
                {
                    var chaxun = tuzhi.Where(a => a.tuzhiYear.Contains(m)).OrderBy(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "6")
                {
                    var chaxun = tuzhi.Where(a => a.jgDate.ToString().Contains(m)).OrderBy(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "7")
                {
                    var chaxun = tuzhi.Where(a => a.neirongTiyao.Contains(m)).OrderBy(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "8")
                {
                    var chaxun = tuzhi.Where(a => a.storeLocation.Contains(m)).OrderBy(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "9")
                {
                    var chaxun = tuzhi.Where(a => a.transferUnit.Contains(m)).OrderBy(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "10")
                {
                    var chaxun = tuzhi.Where(a => a.bilichi.Contains(m)).OrderBy(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "11")
                {
                    var chaxun = tuzhi.Where(a => a.tufu.Contains(m)).OrderBy(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
            }
            ViewBag.result = JsonConvert.SerializeObject(tuzhi);
            return View();
        }

        // GET: TuzhiArchives/Details/5
        public ActionResult Details(long? id,string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TuzhiArchives tuzhiArchives = db.TuzhiArchives.Find(id);
            ViewBag.ID = id;
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", tuzhiArchives.securityID);
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", tuzhiArchives.retentionPeriodNo);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false" },
                new SelectListItem { Text = "是", Value = "true" },
            };
            ViewBag.isYD = new SelectList(list1, "Value", "Text", tuzhiArchives.isYD);
            if (tuzhiArchives == null)
            {
                return HttpNotFound();
            }
            if (action == "返回")
            {
                return RedirectToAction("Index");
            }
            //if (action == "编辑")
            //{
            //    return RedirectToAction("Edit", new { id = id });
            //}
            return View(tuzhiArchives);
        }

        // GET: TuzhiArchives/Create
        public ActionResult Create()
        {
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName");
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName");
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false"},
                new SelectListItem { Text = "是", Value = "true"},
            };
            ViewBag.isYD = new SelectList(list1, "Value", "Text");
            ViewBag.ID = db.TuzhiArchives.Max(a => a.ID) + 1;
            ViewData["next"] = "true";
            return View();
        }

        // POST: TuzhiArchives/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,seqNo,classNo,archiveNo,registrationNo,paijiaNo,archiveTitle,bianzhiUnit,tuzhiYear,jgDate,tuzhiStatus,isBiaohu,measureUnit,tuzhiCount,securityID,retentionPeriodNo,neirongTiyao,storeLocation,transferUnit,bilichi,tufu,archiveCode,luruTime,isYD,coordinate")] TuzhiArchives tuzhiArchives,string action)
        {
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName");
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName");
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false" },
                new SelectListItem { Text = "是", Value = "true" },
            };
            ViewBag.isYD = new SelectList(list1, "Value", "Text");
            ViewBag.ID = db.TuzhiArchives.Max(a => a.ID) + 1;
            if (action == "提交")
            {


                if (tuzhiArchives.archiveTitle == null) 
                {
                    return Content("<script>alert('案卷提名不能为空！');window.history.back();</script>");
                }
                //var a = tuzhiArchives.archiveTitle.Trim();
                if (ModelState.IsValid)
                {
                    if (tuzhiArchives.tuzhiCount == null)
                    {
                        tuzhiArchives.tuzhiCount = 0;
                    }
                    tuzhiArchives.isImageExist = "否";
                    db.TuzhiArchives.Add(tuzhiArchives);
                    db.SaveChanges();
                    Response.Write("<script>alert('已成功保存！');</script>");
                    ViewData["next"] = false;
                    return View(tuzhiArchives);
                }
            }
            if(action=="添加下一条")
            {
                return RedirectToAction("Create");
            }
            if (action == "返回")
            {
                return RedirectToAction("Index");
            }

            return View(tuzhiArchives);
        }

        // GET: TuzhiArchives/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TuzhiArchives tuzhiArchives = db.TuzhiArchives.Find(id);
            ViewBag.ID = id;
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName",tuzhiArchives.securityID);
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", tuzhiArchives.retentionPeriodNo);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false" },
                new SelectListItem { Text = "是", Value = "true" },
            };
            ViewBag.isYD = new SelectList(list1, "Value", "Text",tuzhiArchives.isYD);
            if (tuzhiArchives == null)
            {
                return HttpNotFound();
            }
            return View(tuzhiArchives);
        }

        // POST: TuzhiArchives/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,seqNo,classNo,archiveNo,registrationNo,paijiaNo,archiveTitle,bianzhiUnit,tuzhiYear,jgDate,tuzhiStatus,isBiaohu,measureUnit,tuzhiCount,securityID,retentionPeriodNo,neirongTiyao,storeLocation,transferUnit,bilichi,tufu,archiveCode,luruTime,isImageExist,isYD,coordinate")] TuzhiArchives tuzhiArchives,string action)
        {
            if (action == "保存修改")
            {
                if (tuzhiArchives.archiveTitle.Trim() == "")
                {
                    return Content("<script>alert('案卷提名不能为空！');window.history.back();");
                }
                if (ModelState.IsValid)
                {
                    if (tuzhiArchives.tuzhiCount == null)
                    {
                        tuzhiArchives.tuzhiCount = 0;
                    }
                    tuzhiArchives.isImageExist = "否";
                    db.Entry(tuzhiArchives).State = EntityState.Modified;
                    db.SaveChanges();
                    //return Content("<script>alert('已修改成功！');window.location.href='../Index'</script>");
                    return Content("<script>alert('已修改成功！');history.go(-1);</script>");
                }
            }
            if (action == "返回")
            {
                return RedirectToAction("Index");
            }
            return View(tuzhiArchives);
        }

        // GET: TuzhiArchives/Delete/5
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TuzhiArchives tuzhiArchives = db.TuzhiArchives.Find(id);
        //    if (tuzhiArchives == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tuzhiArchives);
        //}

        // POST: TuzhiArchives/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            TuzhiArchives tuzhiArchives = db.TuzhiArchives.Find(id);
            db.TuzhiArchives.Remove(tuzhiArchives);
            db.SaveChanges();
            return RedirectToAction("Index");
            //return Content("<script>alert('删除成功！');history.go(-1);</script>");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
