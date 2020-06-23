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
    public class PaperArchivesController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();

        // GET: PaperArchives
        public ActionResult Index()
        {
            var paperArchives = from a in db.PaperArchives
                                select a;

                //db.PaperArchives.Include(p => p.ProjectInfo);
            return View(paperArchives.ToList());
        }

        // GET: PaperArchives/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaperArchives paperArchives = db.PaperArchives.Find(id);
            if (paperArchives == null)
            {
                return HttpNotFound();
            }
            return View(paperArchives);
        }

        // GET: PaperArchives/Create
        public ActionResult Create()
        {
            ViewBag.projectID = new SelectList(db.ProjectInfo, "projectID", "projectName");
            return View();
        }

        // POST: PaperArchives/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "paperProjectSeqNo,projectNo,projectID,firstResponsible,responsibleOther,submitPerson,telphoneSubmitPerson,mobilephoneSubmitPerson,recipient,dateReceived,collator,collationRequirement,qjdyYear,qjdyNo,lqDate,mainCategoryID,subDictionaryID,minorDictionaryID,csyj,csyjPerson,csyjDate,fzryj,fzryjPerson,fzryjDate,zgyj,zgyjPerson,zgyjDate,characterVolumeCount,character2cm,character3cm,character4cm,character5cm,drawingVolumeCount,drawing2cm,drawing3cm,drawing4cm,drawing5cm,dateArchive,transferPerson,transferUnit,transferDate,passingDate,archiveCertificateNo,lqyjsDate,lqperson,transferContent,projectProfile,dateConstructed,archivesCount,InchCountDetail,originalVolumeCount,originalInchCount,originalMoneyAmount,copyInchCount,copyMoneyCount,totalMoney,startArchiveNo,endArchiveNo,startPaijiaNo,endPaijiaNo,startRegisNo,endRegisNo,licenseNo,licenseDate,jgDate,projectStartDate,projectFinishDate,changeLog,keyWords,notearea,remarks,buildingArea,height,underground,overground,structureTypeID,textMaterial,drawing,paijiaRange,prevClassNo,PhotoCount,bianhaoTime,luruTime,shizhengNoStart,shizhengNoEnd")] PaperArchives paperArchives)
        {
            if (ModelState.IsValid)
            {
                db.PaperArchives.Add(paperArchives);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.projectID = new SelectList(db.ProjectInfo, "projectID", "projectName", paperArchives.projectID);
            return View(paperArchives);
        }

        // GET: PaperArchives/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaperArchives paperArchives = db.PaperArchives.Find(id);
            if (paperArchives == null)
            {
                return HttpNotFound();
            }
            ViewBag.projectID = new SelectList(db.ProjectInfo, "projectID", "projectName", paperArchives.projectID);
            return View(paperArchives);
        }

        // POST: PaperArchives/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "paperProjectSeqNo,projectNo,projectID,firstResponsible,responsibleOther,submitPerson,telphoneSubmitPerson,mobilephoneSubmitPerson,recipient,dateReceived,collator,collationRequirement,qjdyYear,qjdyNo,lqDate,mainCategoryID,subDictionaryID,minorDictionaryID,csyj,csyjPerson,csyjDate,fzryj,fzryjPerson,fzryjDate,zgyj,zgyjPerson,zgyjDate,characterVolumeCount,character2cm,character3cm,character4cm,character5cm,drawingVolumeCount,drawing2cm,drawing3cm,drawing4cm,drawing5cm,dateArchive,transferPerson,transferUnit,transferDate,passingDate,archiveCertificateNo,lqyjsDate,lqperson,transferContent,projectProfile,dateConstructed,archivesCount,InchCountDetail,originalVolumeCount,originalInchCount,originalMoneyAmount,copyInchCount,copyMoneyCount,totalMoney,startArchiveNo,endArchiveNo,startPaijiaNo,endPaijiaNo,startRegisNo,endRegisNo,licenseNo,licenseDate,jgDate,projectStartDate,projectFinishDate,changeLog,keyWords,notearea,remarks,buildingArea,height,underground,overground,structureTypeID,textMaterial,drawing,paijiaRange,prevClassNo,PhotoCount,bianhaoTime,luruTime,shizhengNoStart,shizhengNoEnd")] PaperArchives paperArchives)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paperArchives).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.projectID = new SelectList(db.ProjectInfo, "projectID", "projectName", paperArchives.projectID);
            return View(paperArchives);
        }

        // GET: PaperArchives/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaperArchives paperArchives = db.PaperArchives.Find(id);
            if (paperArchives == null)
            {
                return HttpNotFound();
            }
            return View(paperArchives);
        }

        // POST: PaperArchives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            PaperArchives paperArchives = db.PaperArchives.Find(id);
            db.PaperArchives.Remove(paperArchives);
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
        public ActionResult Search(string txtCurProNo)
        {
           
            string strCurProNo = txtCurProNo.Trim();
            UrbanConEntities entity = new UrbanConEntities();
            
            
                var projectNos = (from p in entity.PaperArchives
                                  where (p.dateArchive.Value.Year) == Convert.ToInt32(strCurProNo)
                                  orderby p.projectNo descending
                                  select p);
                
                return View(projectNos);
           
         }
            
        
        
      
    }
}
