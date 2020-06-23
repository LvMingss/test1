using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.IO;

namespace urban_archive.Models
{
    public class OfficeInformationController : Controller
    {
        private OfficeEntities db = new OfficeEntities();

        // GET: OfficeInformation
        public ActionResult Index()
        {
            
            db.Configuration.ProxyCreationEnabled = false;
           

            var informationContent = from ad in db.InformationContent
                                     orderby ad.ID descending
                                     select ad;
            //var informationContent = db.InformationContent.Include(i => i.InformationSubCategory);
            ViewBag.result = JsonConvert.SerializeObject(informationContent);
            return View();
        }
        public ActionResult DownLoadFile(string id)     //文件下载
        {
            //if (string.IsNullOrEmpty(id))
            //{
            //    throw new ArgumentNullException("fileId is errror");
            //}
            int Id = Convert.ToInt32(id);
            var findFile = db.InformationContent.Find(Id);
            if (findFile == null)
            {
                Response.Write("<script >alert('未找到文件！')</script>");
            }
            string filePath = Request.MapPath("~/files/OfficeInformation");
            string path = filePath +"/"+ findFile.filename;
            //以字符流的形式下载文件
              if(!System.IO.File.Exists(path) )
            {
                return Content("<script >alert('该公文没有下载文件！');window.history.back();</script>");
                //Response.Write("<script >alert('该公文没有下载文件！');window.history.back();</script>");
                
            }
            
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(findFile.filename, System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
            return View();
        }

        public ActionResult Details(int? flag)
        {
            ViewData["pagename"] = "OfficeInformation/Details";
            db.Configuration.ProxyCreationEnabled = false;
            if (flag == 1)
            {
                ViewData["pagename"] = "OfficeInformation/Details1";
            }
            else if (flag == 2)
            {
                ViewData["pagename"] = "OfficeInformation/Details2";
            }
            

            var informationContent = from ad in db.InformationContent
                                     orderby ad.ID descending
                                     select ad;
            //var informationContent = db.InformationContent.Include(i => i.InformationSubCategory);
            ViewBag.result = JsonConvert.SerializeObject(informationContent);
            return View();
        }
        //// GET: OfficeInformation/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    InformationContent informationContent = db.InformationContent.Find(id);
        //    if (informationContent == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(informationContent);
        //}

        // GET: OfficeInformation/Create
        public ActionResult Create()
        {
            int max_id = 0;
            ViewBag.ID = max_id;
            var model = from a in db.InformationContent
                        orderby a.ID descending
                        select a;
            if (model.Count() != 0)
            {
                ViewBag.ID = model.First().ID + 1;
            }


            ViewBag.subCategoryID = new SelectList(db.InformationSubCategory, "ID", "Name");
            return View();
        }

        // POST: OfficeInformation/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,title,author,date,content,subCategoryID,ClickTimes,filename,filetype")] InformationContent informationContent)
        {
            string file = Request.Form["MyUploadile"];
            string filename = Request.Form["name"];
            int max_id = 0;
            ViewBag.ID = max_id;
            var model = from a in db.InformationContent
                        orderby a.ID descending
                        select a;
            if (model.Count() != 0)
            {
                ViewBag.ID =model.First().ID +1;
            }
            ViewBag.subCategoryID = new SelectList(db.InformationSubCategory, "ID", "Name", informationContent.subCategoryID);


            if (ModelState.IsValid)//文件上传
            {
                if (filename != "")
                {
                    var files = Request.Files;
                    if (files.Count > 0)
                    {
                        var file1 = files[0];
                        string strFileSavePath = Request.MapPath("~/files/OfficeInformation");//文件存储路径
                        file1.SaveAs(strFileSavePath + "/" + filename);
                    }
                }
                informationContent.filename = filename;
                informationContent.filetype = "." + filename.Split('.').Last();
                informationContent.ClickTimes = 0;
                db.InformationContent.Add(informationContent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            return View(informationContent);
        }

        // GET: OfficeInformation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InformationContent informationContent = db.InformationContent.Find(id);
            ViewBag.name = informationContent.filename;
            if (informationContent == null)
            {
                return HttpNotFound();
            }
            ViewBag.subCategoryID = new SelectList(db.InformationSubCategory, "ID", "Name", informationContent.subCategoryID);
            return View(informationContent);
        }

        // POST: OfficeInformation/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,title,author,date,content,subCategoryID,ClickTimes,filename,filetype")] InformationContent informationContent)
        {
            string file = Request.Form["MyUploadile"];
            string filename = Request.Form["name"];
            int max_id = db.InformationContent.Max(d => d.ID);
            ViewBag.ID = max_id + 1;
            ViewBag.subCategoryID = new SelectList(db.InformationSubCategory, "ID", "Name", informationContent.subCategoryID);
            if (ModelState.IsValid)//文件上传
            {
                var files = Request.Files;
                if (files.Count > 0)
                {
                    var file1 = files[0];
                    string strFileSavePath = Request.MapPath("~/files");//文件存储路径
                    file1.SaveAs(strFileSavePath + "/" + filename);
                }
                informationContent.filename = filename;
                informationContent.filetype = "." + filename.Split('.').Last();
                db.Entry(informationContent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(informationContent);
        }
        public ActionResult chakan(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InformationContent informationContent = db.InformationContent.Find(id);
            ViewBag.name = informationContent.filename;
            if (informationContent == null)
            {
                return HttpNotFound();
            }
            ViewBag.subCategoryID = new SelectList(db.InformationSubCategory, "ID", "Name", informationContent.subCategoryID);
            return View(informationContent);
        }

        // POST: OfficeInformation/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult chakan(string id1)
        {
            if (id1=="1")
            {
                return RedirectToAction("Details");
            }
            else
            {
                return RedirectToAction("index");
            }
            //int max_id = db.InformationContent.Max(d => d.ID);
            //ViewBag.ID = max_id + 1;
            //ViewBag.subCategoryID = new SelectList(db.InformationSubCategory, "ID", "Name", informationContent.subCategoryID);
            //return View(informationContent);
        }
        // GET: OfficeInformation/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    InformationContent informationContent = db.InformationContent.Find(id);
        //    if (informationContent == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(informationContent);
        //}

        //// POST: OfficeInformation/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id ,InformationContent informationContent1)
        {
            
            InformationContent informationContent = db.InformationContent.Find(id);
            db.InformationContent.Remove(informationContent);
            db.SaveChanges();
            return Content("<script>alert('已成功删除！');window.location.href='/OfficeInformation/Index'</script>");
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
