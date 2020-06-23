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
using System.Data.Entity.Infrastructure;

namespace urban_archive.Controllers
{
    public class ArchivesDetailsController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();

        // GET: ArchivesDetails
        /* public ActionResult Index()
         {
             var archivesDetail = db.ArchivesDetail.Include(a => a.PaperArchives);
             return View(archivesDetail.ToList());
            // return View(await archivesDetail.ToListAsync());
         }*/

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page,int? SelectedID)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "项目顺序号", Value = "0"}, 
                new SelectListItem { Text = "档号", Value = "1" },
                new SelectListItem { Text = "总登记号", Value = "2" }
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            int t = SelectedID.GetValueOrDefault();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var archivesDetail = from ad in db.ArchivesDetail
                           select ad;
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        long temp = Int64.Parse(searchString);
                        archivesDetail = archivesDetail.Where(ad => ad.paperProjectSeqNo == temp);//根据项目顺序号搜索
                        break;
                    case 1:
                        archivesDetail = archivesDetail.Where(ad => ad.archivesNo == searchString);//根据档号搜索
                        break;
                    case 2:
                        archivesDetail = archivesDetail.Where(ad => ad.registrationNo == searchString);//根据总登记号搜索
                        break;
                }
               
            }
             // 默认按档号排
            archivesDetail = archivesDetail.OrderBy(s => s.archivesNo);
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(archivesDetail.ToPagedList(pageNumber, pageSize));
        }



        // GET: ArchivesDetails/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivesDetail archivesDetail = db.ArchivesDetail.Find(id);
            if (archivesDetail == null)
            {
                return HttpNotFound();
            }
            return View(archivesDetail);
        }

        // GET: ArchivesDetails/Create
        public ActionResult Create()
        {
           // ViewBag.paperProjectSeqNo = new SelectList(db.PaperArchives, "paperProjectSeqNo", "firstResponsible");
            return View();
        }

        // POST: ArchivesDetails/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "archivesNo,ID,archivesTitle,paperProjectSeqNo,paijiaNo,registrationNo,pageCount,volNo,mapsheetNo,microNo,storagePath,indexer,indexDate,checker,checkDate,typist,typerDate,archiveThickness,notearea,remarks,textMaterial,drawing,firstResponsible,responsibleOther,transferUnit,licenseNo,photoCount,bianzhiTime,kaigongTime,jungongTime,fazhaoTime,jgDate,developmentUnit,constructionUnit,designUnit,isImageExist,shizhengNo")] ArchivesDetail archivesDetail)
        {
            if (ModelState.IsValid)
            {
                db.ArchivesDetail.Add(archivesDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.paperProjectSeqNo = new SelectList(db.PaperArchives, "paperProjectSeqNo", "firstResponsible", archivesDetail.paperProjectSeqNo);
            return View(archivesDetail);
        }

        // GET: ArchivesDetails/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivesDetail archivesDetail = db.ArchivesDetail.Find(id);
            if (archivesDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.paperProjectSeqNo = new SelectList(db.PaperArchives, "paperProjectSeqNo", "firstResponsible", archivesDetail.paperProjectSeqNo);
            return View(archivesDetail);
        }

        // POST: ArchivesDetails/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "archivesNo,ID,archivesTitle,paperProjectSeqNo,paijiaNo,registrationNo,pageCount,volNo,mapsheetNo,microNo,storagePath,indexer,indexDate,checker,checkDate,typist,typerDate,archiveThickness,notearea,remarks,textMaterial,drawing,firstResponsible,responsibleOther,transferUnit,licenseNo,photoCount,bianzhiTime,kaigongTime,jungongTime,fazhaoTime,jgDate,developmentUnit,constructionUnit,designUnit,isImageExist,shizhengNo")] ArchivesDetail archivesDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(archivesDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.paperProjectSeqNo = new SelectList(db.PaperArchives, "paperProjectSeqNo", "firstResponsible", archivesDetail.paperProjectSeqNo);
            return View(archivesDetail);
        }

        // GET: ArchivesDetails/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivesDetail archivesDetail = db.ArchivesDetail.Find(id);
            if (archivesDetail == null)
            {
                return HttpNotFound();
            }
            return View(archivesDetail);
        }

        // POST: ArchivesDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ArchivesDetail archivesDetail = db.ArchivesDetail.Find(id);
            db.ArchivesDetail.Remove(archivesDetail);
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
