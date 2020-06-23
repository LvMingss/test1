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
namespace urban_archive.Controllers
{
    public class zdArchivesController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();

        // GET: zdArchives
        public ActionResult Index(string action, int? page)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "总登记号", Value = "0"},
                new SelectListItem { Text = "档号", Value = "1"},
                new SelectListItem { Text = "拨地证号", Value = "2"},
                new SelectListItem { Text = "征地文号", Value = "3"},
                new SelectListItem { Text = "拨地地点", Value = "4"},
                new SelectListItem { Text = "案卷题名", Value = "5"},
                new SelectListItem { Text = "顺序号", Value = "6"},
            };
            ViewBag.Selected = new SelectList(list, "Value", "Text");
            var ZD = db.zdArchive.OrderByDescending(a => a.regisNo);
            //int pageSize = 50;
            //int pageNumber = (page ?? 1);

            if (action == "查询")
            {
                string n = Request.Form["Selected"];
                string m = Request.Form["search"];
                if (n == "0")
                {
                    var chaxun = ZD.Where(a => a.regisNo.Contains(m)).OrderByDescending(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "1")
                {
                    var chaxun = ZD.Where(a => a.archiveNo.Contains(m)).OrderByDescending(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "2")
                {
                    var chaxun = ZD.Where(a => a.bdzh.Contains(m)).OrderByDescending(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "3")
                {
                    var chaxun = ZD.Where(a => a.zdwh.Contains(m)).OrderByDescending(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "4")
                {
                    var chaxun = ZD.Where(a => a.hbLocation.Contains(m)).OrderByDescending(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "5")
                {
                    var chaxun = ZD.Where(a => a.archiveTitle.Contains(m)).OrderByDescending(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "6")
                {
                    var chaxun = ZD.Where(a => a.seqNo==int.Parse(m)).OrderByDescending(a => a.ID);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
            }
            ViewBag.result = JsonConvert.SerializeObject(ZD);
            return View();
        }

        // GET: zdArchives/Details/5
        public ActionResult Details(long? id,string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            zdArchive zdArchive = db.zdArchive.Find(id);
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", zdArchive.securityID);
            ViewBag.retentionN = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", zdArchive.retentionN);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1"},
            };
            ViewBag.isYD = new SelectList(list1, "Value", "Text", zdArchive.isYD);
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "请选择案卷厚度", Value = "0"},
                new SelectListItem { Text = "1厘米", Value = "1"},
                new SelectListItem { Text = "2厘米", Value = "2"},
                new SelectListItem { Text = "3厘米", Value = "3"},
                new SelectListItem { Text = "4厘米", Value = "4"},
                new SelectListItem { Text = "5厘米", Value = "5"},
            };
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", zdArchive.ArchiveThick);
            if (zdArchive == null)
            {
                return HttpNotFound();
            }
            if (action == "返回")
            {
                return RedirectToAction("Index");
            }
            if (action == "编辑")
            {
                return RedirectToAction("Edit", new { id = id });
            }
            return View(zdArchive);
        }

        // GET: zdArchives/Create
        public ActionResult Create()
        {
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName");
            ViewBag.retentionN = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName");
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1"},
            };
            ViewBag.isYD = new SelectList(list1, "Value", "Text");
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "请选择案卷厚度", Value = "0"},
                new SelectListItem { Text = "1厘米", Value = "1"},
                new SelectListItem { Text = "2厘米", Value = "2"},
                new SelectListItem { Text = "3厘米", Value = "3"},
                new SelectListItem { Text = "4厘米", Value = "4"},
                new SelectListItem { Text = "5厘米", Value = "5"},
            };
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text");
            ViewBag.ID = db.zdArchive.Max(a => a.ID) + 1;
            return View();
        }

        // POST: zdArchives/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,regisNo,archiveNo,paijiaNo,bdzh,tufuNo,zdwh,archiveTitle,firstResponsible,otherResponsible,hbDate,hbLocation,hbAreaMu,hbAreaKM,jgDate,changeLog,securityID,retentionN,textPageCnt,drawingPageCnt,photoCnt,totolPageCnt,noteArea,zhutiCi,tiyaoXiang,zhuluDate,zlPerson,luruDate,luruPerson,seqNo,ArchiveThick,isImageExist,isYD")] zdArchive zdArchive,string action)
        {
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName");
            ViewBag.retentionN = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName");
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1"},
            };
            ViewBag.isYD = new SelectList(list1, "Value", "Text");
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "请选择案卷厚度", Value = "0"},
                new SelectListItem { Text = "1厘米", Value = "1"},
                new SelectListItem { Text = "2厘米", Value = "2"},
                new SelectListItem { Text = "3厘米", Value = "3"},
                new SelectListItem { Text = "4厘米", Value = "4"},
                new SelectListItem { Text = "5厘米", Value = "5"},
            };
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text");
            ViewBag.ID = db.zdArchive.Max(a => a.ID) + 1;
            if (action == "提交")
            {
                if (ModelState.IsValid)
                {
                    if (zdArchive.regisNo == null)
                    {
                        return Content("<script>alert('总登记号不能为空！');window.history.back();</script>");
                    }
                    zdArchive.isImageExist = "否";
                    db.zdArchive.Add(zdArchive);
                    db.SaveChanges();
                    return Content("<script>alert('已成功保存！');window.location.href='./Index'</script>");
                }
            }
            if (action == "返回")
            {
                return RedirectToAction("Index");
            }
            return View(zdArchive);
        }

        // GET: zdArchives/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            zdArchive zdArchive = db.zdArchive.Find(id);
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName",zdArchive.securityID);
            ViewBag.retentionN = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName",zdArchive.retentionN);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1"},
            };
            ViewBag.isYD = new SelectList(list1, "Value", "Text",zdArchive.isYD);
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "请选择案卷厚度", Value = "0"},
                new SelectListItem { Text = "1厘米", Value = "1"},
                new SelectListItem { Text = "2厘米", Value = "2"},
                new SelectListItem { Text = "3厘米", Value = "3"},
                new SelectListItem { Text = "4厘米", Value = "4"},
                new SelectListItem { Text = "5厘米", Value = "5"},
            };
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text",zdArchive.ArchiveThick);
            if (zdArchive == null)
            {
                return HttpNotFound();
            }
            return View(zdArchive);
        }

        // POST: zdArchives/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,regisNo,archiveNo,paijiaNo,bdzh,tufuNo,zdwh,archiveTitle,firstResponsible,otherResponsible,hbDate,hbLocation,hbAreaMu,hbAreaKM,jgDate,changeLog,securityID,retentionN,textPageCnt,drawingPageCnt,photoCnt,totolPageCnt,noteArea,zhutiCi,tiyaoXiang,zhuluDate,zlPerson,luruDate,luruPerson,seqNo,ArchiveThick,isImageExist,isYD")] zdArchive zdArchive,string action)
        {
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", zdArchive.securityID);
            ViewBag.retentionN = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", zdArchive.retentionN);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1"},
            };
            ViewBag.isYD = new SelectList(list1, "Value", "Text", zdArchive.isYD);
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "请选择案卷厚度", Value = "0"},
                new SelectListItem { Text = "1厘米", Value = "1"},
                new SelectListItem { Text = "2厘米", Value = "2"},
                new SelectListItem { Text = "3厘米", Value = "3"},
                new SelectListItem { Text = "4厘米", Value = "4"},
                new SelectListItem { Text = "5厘米", Value = "5"},
            };
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", zdArchive.ArchiveThick);
            if (action == "保存修改")
            {
                if (ModelState.IsValid)
                {
                    if (zdArchive.regisNo == null)
                    {
                        return Content("<script>alert('总登记号不能为空！');window.history.back();</script>");
                    }
                    zdArchive.isImageExist = "否";
                    db.Entry(zdArchive).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("<script>alert('已修改成功！');window.location.href='../Index'</script>");
                }
            }
            if (action == "返回")
            {
                return RedirectToAction("Index");
            }
            return View(zdArchive);
        }

        // GET: zdArchives/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    zdArchive zdArchive = db.zdArchive.Find(id);
        //    if (zdArchive == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(zdArchive);
        //}

        // POST: zdArchives/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long? id)
        {
            zdArchive zdArchive = db.zdArchive.Find(id);
            db.zdArchive.Remove(zdArchive);
            db.SaveChanges();
            return RedirectToAction("Index");
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
