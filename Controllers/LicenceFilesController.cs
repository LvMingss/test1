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
    public class LicenceFilesController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        private PlanArchiveEntities db_plan = new PlanArchiveEntities();

        // GET: LicenceFiles
        public ActionResult Index(int archiveID,int id1,int id)
       {
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text");
            ViewBag.archiveID = archiveID;
            if (id1 == 2)
            {
                LicenceFiles licenceFiles1 = db.LicenceFiles.Find(id);
                
                ViewBag.archiveID = licenceFiles1.archiveID;//记得传回视图
                ViewBag.fileTitle1 = licenceFiles1.fileTitle;
                ViewBag.resUnit1 = licenceFiles1.resUnit;
                ViewBag.ID = id;
                ViewBag.juanneiSeqNo = licenceFiles1.juanneiSeqNo;//记得传回视图
                ViewBag.fileNo = licenceFiles1.fileNo;
                ViewBag.pageNo = licenceFiles1.pageNo;
                ViewBag.bianzhiDate = licenceFiles1.bianzhiDate;
                ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", licenceFiles1.isNeibu.Trim());
                ViewBag.remarks = licenceFiles1.remarks;
                ViewBag.edit = "display:block";
                ViewBag.add = "display:none";
            }
            if (id1 == 0)
            {
                ViewBag.edit = "display:none";
                ViewBag.add = "display:none";
            }
            if(id1==1)//添加
            {
                LicenceFiles licenceFiles1 = db.LicenceFiles.Find(id);
                if (licenceFiles1 != null)
                {
                    ViewBag.archiveID = licenceFiles1.archiveID;//记得传回视图
                    ViewBag.fileTitle1 = licenceFiles1.fileTitle;
                    ViewBag.resUnit1 = licenceFiles1.resUnit;
                    ViewBag.ID = id;
                    ViewBag.juanneiSeqNo = licenceFiles1.juanneiSeqNo;//记得传回视图
                    ViewBag.fileNo = licenceFiles1.fileNo;
                    ViewBag.pageNo = licenceFiles1.pageNo;
                    ViewBag.bianzhiDate = licenceFiles1.bianzhiDate;
                    ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", licenceFiles1.isNeibu.Trim());
                    ViewBag.remarks = licenceFiles1.remarks;
                }
                else {  
                    long maxid = db.LicenceFiles.Max(a => a.ID);
                    LicenceFiles licenceFiles2 = db.LicenceFiles.Find(maxid);
                    ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", licenceFiles2.isNeibu.Trim());                
                }             
                ViewBag.add = "display:block";
                ViewBag.edit = "display:none";
            }
            
            var licenceFiles = from ad in db.LicenceFiles
                               where ad.archiveID == archiveID                             
                                orderby ad.juanneiSeqNo
                                select ad;//按卷内序号排序
            OtherArchives other = db.OtherArchives.Find(archiveID);
            ViewData["BACK_GD"] = "display:none";
            ViewData["BACK_BH"] = "display:none";
            ViewData["BACK_SH"] = "display:none";
            ViewData["BACK_RK"] = "display:none";
            ViewData["BACK"] = "display:none";

            if (other.status== "RK")
            {
                ViewData["BACK_RK"] = "display:inline";
                ViewData["fanhui"] = "display:none";
            }
            else if (other.status == "GD")
            {
                ViewData["BACK_GD"] = "display:inline";
                ViewData["fanhui"] = "display:none";

            }
            else if (other.status == "BH")
            {
                ViewData["BACK_BH"] = "display:inline";
                ViewData["fanhui"] = "display:none";

            }
            else if (other.status == "SH")
            {
                ViewData["BACK_SH"] = "display:inline";
                ViewData["fanhui"] = "display:none";

            }
            else 
            {
                ViewData["BACK"] = "display:inline";
            }


            return View(licenceFiles.ToList());
        }

        // GET: LicenceFiles/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LicenceFiles licenceFiles = db.LicenceFiles.Find(id);
            if (licenceFiles == null)
            {
                return HttpNotFound();
            }
            return View(licenceFiles);
        }

        // GET: LicenceFiles/Create
        public ActionResult Create(int archiveID)
        {
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text");
            ViewBag.archiveID = archiveID;
            return View();
        }

        // POST: LicenceFiles/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "juanneiSeqNo,fileNo,fileTitle,resUnit,pageNo,bianzhiDate,remarks,isNeibu")] LicenceFiles licenceFiles,int archiveID,string action)
        {
            licenceFiles.archiveID = archiveID;//将传进来的archiveID传给licenceFiles
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text");
            ViewBag.archiveID = archiveID;
            if (action == "保存")
            {  
                if (ModelState.IsValid)
                {
                    long max_ID = db.LicenceFiles.Max(d => d.ID);//当前录入记录的最大ID
                    licenceFiles.ID = max_ID + 1;//新记录的ID+1，值唯一
                                                 //jsy前台传值到后台存储
                    licenceFiles.fileTitle = Request.Form["fileTitle"];
                    licenceFiles.resUnit = Request.Form["resUnit"];
                    //jsy
                    db.LicenceFiles.Add(licenceFiles);
                    db.SaveChanges();
                    //return Content("<script >alert('添加成功！');window.history.back();</script >");
                    return Content("<script >alert('添加成功！');window.location.href='/LicenceFiles/Index?id1=1" + "&id=" + licenceFiles.ID + "&archiveID=" + archiveID + "';</script >");
                    //return RedirectToAction("Index", new { archiveID = archiveID,id1=1,id=0 });
                }
                
            }
            if (action == "添加下一卷") {
                return RedirectToAction("Index", new { archiveID = archiveID, id1 = 1, id = 0 });
            }
            if (action == "删除词条")
            {
                int id =int.Parse( Request.Form["no"].Split('-').First());
                WordTable  wordtable = db.WordTable.Find(id);
                db.WordTable.Remove(wordtable);
                var list1 = db.WordTable.Where(ad => ad.newid > wordtable.newid).OrderBy(ad => ad.newid);
                foreach (var i in list1)
                {
                    i.newid -= 1;
                    ViewBag.fileTitle = Request.Form["fileTitle"];
                    ViewBag.resUnit = Request.Form["resUnit"];
                    db.Entry(i).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { archiveID = archiveID, id1 = 1, id = 0 });
        }
        public JsonResult title()//jsy动态框方法
        {
            var list = from bb in db.WordTable.ToList()
                       where bb.character == 3
                       orderby bb.newid
                       select new
                       {
                           ID = bb.id,
                           name = bb.wordName,
                           ch = bb.character,
                           newId = bb.newid
                       };
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Responser()
        {
            var list1 = from bb in db.WordTable.ToList()
                        where bb.character == 4
                        orderby bb.newid
                        select new
                        {
                            ID = bb.id,
                            name = bb.wordName,
                            ch = bb.character,
                            newId = bb.newid
                        };
            return Json(list1, JsonRequestBehavior.AllowGet);
        }
        //jsy
        //jsy弹出窗口
        public ActionResult window()
        {
            return View();
        }
        public void tianjiabefore()//前向添加数据存储到数据库
        {
            int cur_id = int.Parse(Request.Form["id"]);
            WordTable cur_wordTable = db.WordTable.Find(cur_id);
            WordTable wordTable = new WordTable();
                if (ModelState.IsValid)
                {
                    var list1 = db.WordTable.Where(ad => ad.newid >= cur_wordTable.newid).OrderBy(ad => ad.newid);
                    foreach (var i in list1)
                    {
                        i.newid += 1;
                        db.Entry(i).State = EntityState.Modified;
                    }
                    int max_id = db.WordTable.Max(d => d.id);
                    wordTable.id = max_id + 1;
                    wordTable.newid = cur_wordTable.newid - 1;
                    wordTable.wordName = Request.Form["txtName"];
                    wordTable.character = int.Parse(Request.Form["txtSeqNo"]);
                   db.WordTable.Add(wordTable);
                   db.SaveChanges();
            }
        }
        public void tianjiaafter()//后向添加数据存储到数据库
        {
            int cur_id = int.Parse(Request.Form["id"]);
            WordTable cur_wordTable = db.WordTable.Find(cur_id);
            WordTable wordTable = new WordTable();
            if (ModelState.IsValid)
            {
                var list1 = db.WordTable.Where(ad => ad.newid > cur_wordTable.newid).OrderBy(ad => ad.newid);
                foreach (var i in list1)
                {
                    i.newid += 1;
                    db.Entry(i).State = EntityState.Modified;

                }
                int max_id = db.WordTable.Max(d => d.id);
                wordTable.id = max_id + 1; ;
                wordTable.newid = cur_wordTable.newid + 1;
                wordTable.wordName = Request.Form["txtName"];
                wordTable.character = int.Parse(Request.Form["txtSeqNo"].Split('-').Last());
                db.WordTable.Add(wordTable);
                db.SaveChanges();
            }
        }

        public void tianjiabefore1()//业务科前向添加数据存储到数据库
        {
            int cur_id = int.Parse(Request.Form["id"]);
            PlanProjectCT cur_planProjectCT = db_plan.PlanProjectCT.Find(cur_id);
            PlanProjectCT planProjectCT = new PlanProjectCT();
            if (ModelState.IsValid)
            {
                var list1 = db_plan.PlanProjectCT.Where(ad => ad.newid >= cur_planProjectCT.newid).OrderBy(ad => ad.newid);
                var x = list1.Count();
                foreach (var i in list1)
                {
                    i.newid += 1;
                    db_plan.Entry(i).State = EntityState.Modified;
                }
                int max_id = db_plan.PlanProjectCT.Max(d => d.ID);
                planProjectCT.ID = max_id + 1;
                planProjectCT.newid = cur_planProjectCT.newid - 1;
                planProjectCT.projectContent = Request.Form["txtName"];
                planProjectCT.classifyID = int.Parse(Request.Form["txtSeqNo"]);
                db_plan.PlanProjectCT.Add(planProjectCT);
                db_plan.SaveChanges();
            }
        }
        public void tianjiaafter1()//业务科后向添加数据存储到数据库
        {
            int cur_id = int.Parse(Request.Form["id"]);
            PlanProjectCT cur_planProjectCT = db_plan.PlanProjectCT.Find(cur_id);
            PlanProjectCT planProjectCT = new PlanProjectCT();
            if (ModelState.IsValid)
            {
                var list1 = db_plan.PlanProjectCT.Where(ad => ad.newid > cur_planProjectCT.newid).OrderBy(ad => ad.newid);
                foreach (var i in list1)
                {
                    i.newid += 1;
                    db_plan.Entry(i).State = EntityState.Modified;

                }
                int max_id = db_plan.PlanProjectCT.Max(d => d.ID);
                planProjectCT.ID = max_id + 1; ;
                planProjectCT.newid = cur_planProjectCT.newid + 1;
                planProjectCT.projectContent = Request.Form["txtName"];
                planProjectCT.classifyID = int.Parse(Request.Form["txtSeqNo"].Split('-').Last());
                db_plan.PlanProjectCT.Add(planProjectCT);
                db_plan.SaveChanges();
            }
        }
        //jsy
        // GET: LicenceFiles/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            LicenceFiles licenceFiles = db.LicenceFiles.Find(id);
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", licenceFiles.isNeibu.Trim());

            ViewBag.archiveID = licenceFiles.archiveID;//记得传回视图
            ViewBag.fileTitle = licenceFiles.fileTitle;
            ViewBag.resUnit = licenceFiles.resUnit;
            ViewBag.ID = id;
            if (licenceFiles == null)
            {
                return HttpNotFound();
            }
            return View(licenceFiles);
        }

        // POST: LicenceFiles/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "isNeibu,archiveID,juanneiSeqNo,fileNo,fileTitle1,resUnit1,pageNo,bianzhiDate,remarks")] LicenceFiles licenceFiles, int ID,string action)
        {
            long archiveID = long.Parse(Request.Form["archiveID"]);
            if (action == "保存修改")
            {
                if (ModelState.IsValid)
                {
                    archiveID = long.Parse(Request.Form["archiveID"]);
                    ViewBag.archiveID = archiveID;//记得传回视图
                    ViewBag.fileTitle = licenceFiles.fileTitle;
                    ViewBag.resUnit = licenceFiles.resUnit;
                    licenceFiles.ID = ID;
                    licenceFiles.fileTitle = Request.Form["fileTitle1"];
                    licenceFiles.resUnit = Request.Form["resUnit1"];
                    db.Entry(licenceFiles).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("<script >alert('修改成功！');window.location.href='/LicenceFiles/Index?id1=2" + "&id=" + ID + "&archiveID=" + archiveID + "';</script >");
                }        
                
            }
            if (action == "删除词条")
            {
                int id = int.Parse(Request.Form["no1"].Split('-').First());
                WordTable wordtable = db.WordTable.Find(id);
                db.WordTable.Remove(wordtable);
                var list1 = db.WordTable.Where(ad => ad.newid > wordtable.newid).OrderBy(ad => ad.newid);
                foreach (var i in list1)
                {
                    i.newid -= 1;
                    db.Entry(i).State = EntityState.Modified;
                }
                db.SaveChanges();
                return Content("<script >alert('删除成功！');window.location.href='/LicenceFiles/Index?id1=2" + "&id=" + ID + "&archiveID=" + archiveID+ "';</script >");
            }
            return View();
            
        }

        // GET: LicenceFiles/Delete/5
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    LicenceFiles licenceFiles = db.LicenceFiles.Find(id);
        //    if (licenceFiles == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(licenceFiles);
        //}

        // POST: LicenceFiles/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            LicenceFiles licenceFiles = db.LicenceFiles.Find(id);
            long archiveid = licenceFiles.archiveID;
            db.LicenceFiles.Remove(licenceFiles);
            db.SaveChanges();
            return RedirectToAction("Index", new { archiveID = archiveid, id1 = 0, id = 0 });
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
