using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using urban_archive.Models;

namespace urban_archive.Controllers
{
    public class FileInfoesController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();

        // GET: FileInfoes
        public ActionResult Index()
        {
            return View(db.FileInfo.ToList());
        }

        // GET: FileInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileInfo fileInfo = db.FileInfo.Find(id);
            if (fileInfo == null)
            {
                return HttpNotFound();
            }
            return View(fileInfo);
        }

        // GET: FileInfoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FileInfoes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "fileName,archivesNo,seqNo,fileNo,startPageNo,endPageNo,responsible,remarks,startDate,endDate,type,number,dengjihao,id")] FileInfo fileInfo)
        {
            if (ModelState.IsValid)
            {
                db.FileInfo.Add(fileInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fileInfo);
        }

        // GET: FileInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileInfo fileInfo = db.FileInfo.Find(id);
            if (fileInfo == null)
            {
                return HttpNotFound();
            }
            return View(fileInfo);
        }

        // POST: FileInfoes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "fileName,archivesNo,seqNo,fileNo,startPageNo,endPageNo,responsible,remarks,startDate,endDate,type,number,dengjihao,id")] FileInfo fileInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fileInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fileInfo);
        }

        // GET: FileInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileInfo fileInfo = db.FileInfo.Find(id);
            if (fileInfo == null)
            {
                return HttpNotFound();
            }
            return View(fileInfo);
        }

        // POST: FileInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FileInfo fileInfo = db.FileInfo.Find(id);
            db.FileInfo.Remove(fileInfo);
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
