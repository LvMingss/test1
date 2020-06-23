using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using urban_archive.Models;
using PagedList;
using Newtonsoft.Json;

namespace urban_archive.Controllers
{
    public class BriefingsController : Controller
    {
        private BianYanEntities db = new BianYanEntities();

        // GET: Briefings
        public ActionResult Index(string action, int? page)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "文章标题", Value = "0"},
                new SelectListItem { Text = "文章副标题", Value = "1"},
                 new SelectListItem { Text = "报纸来源", Value = "2"},
                 new SelectListItem { Text = "编辑人", Value = "3"},
                 new SelectListItem { Text = "报刊日期", Value = "4"}
            };
            ViewBag.Selected = new SelectList(list, "Value", "Text");
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            var information = from ad in db.Briefing
                       orderby ad.ID
                       select ad;//按类别排序
            if (action == "查询")
            {
                string n = Request.Form["Selected"];
                string m = Request.Form["search"];
                if (n == "0")
                {

                    var chaxun = from ad in db.Briefing
                                 where ad.briefTitle.Contains(m)
                                 orderby ad.ID
                                 select ad;
                    return View(chaxun.ToPagedList(pageNumber, pageSize));
                }
                if (n == "1")
                {
                    var chaxun = from ad in db.Briefing
                                 where ad.briefCoTitle.Contains(m)
                                 orderby ad.ID
                                 select ad;
                    return View(chaxun.ToPagedList(pageNumber, pageSize));
                }
                if (n == "2")
                {
                    var chaxun = from ad in db.Briefing
                                 where ad.paperName.Contains(m)
                                 orderby ad.ID
                                 select ad;
                    return View(chaxun.ToPagedList(pageNumber, pageSize));
                }
                if (n == "3")
                {
                    var chaxun = from ad in db.Briefing
                                 where ad.maker.Contains(m)
                                 orderby ad.ID
                                 select ad;
                    return View(chaxun.ToPagedList(pageNumber, pageSize));
                }
                if (n == "4")
                {
                    var chaxun = from ad in db.Briefing
                                 where ad.briefTime.Contains(m)
                                 orderby ad.ID
                                 select ad;
                    return View(chaxun.ToPagedList(pageNumber, pageSize));
                }
            }
            return View(information.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult DownLoadFile(string id)     //文件下载
        {
            //if (string.IsNullOrEmpty(id))
            //{
            //    throw new ArgumentNullException("fileId is errror");
            //}
            int Id = Convert.ToInt32(id);
            var findFile = db.Briefing.Find(Id);
            if (findFile == null)
            {
                Response.Write("<script >alert('未找到文件！')</script>");
            }
            string filePath = Request.MapPath("~/files/chengjianInformation");
            string path = filePath + "/" + findFile.fileName;
            //以字符流的形式下载文件
            if (!System.IO.File.Exists(path)) {
                return Content("<script >alert('该公文没有下载文件！');window.history.back();</script>");
            }
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(findFile.fileName, System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
            return View();
        }
        // GET: Briefings/Details/5
        public ActionResult Details(string action)
        {
            ViewData["pagename"] = "Briefings/Details";

            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "文章标题", Value = "0"},
                new SelectListItem { Text = "文章副标题", Value = "1"},
                 new SelectListItem { Text = "报纸来源", Value = "2"},
                 new SelectListItem { Text = "编辑人", Value = "3"},
                 new SelectListItem { Text = "报刊日期", Value = "4"}
            };
            ViewBag.Selected = new SelectList(list, "Value", "Text");
            var information = from ad in db.Briefing
                              orderby ad.ID descending
                              select ad;//按类别排序
            if (action == "查询")
            {
                string n = Request.Form["Selected"];
                string m = Request.Form["search"];
                if (m != "")
                {
                    ViewBag.Selected = new SelectList(list, "Value", "Text", n);
                    ViewBag.search = m;
                    if (n == "0")
                    {

                        var chaxun = from ad in db.Briefing
                                     where ad.briefTitle.Contains(m)
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "1")
                    {
                        var chaxun = from ad in db.Briefing
                                     where ad.briefCoTitle.Contains(m)
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "2")
                    {
                        var chaxun = from ad in db.Briefing
                                     where ad.paperName.Contains(m)
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "3")
                    {
                        var chaxun = from ad in db.Briefing
                                     where ad.maker.Contains(m)
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "4")
                    {
                        var chaxun = from ad in db.Briefing
                                     where ad.briefTime.Contains(m)
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                }
            }
            ViewBag.result = JsonConvert.SerializeObject(information);
            return View();
        }

        // GET: Briefings/Create
        public ActionResult Create()
        {
            ViewData["pagename"] = "Briefings/Create";

            ViewBag.year= DateTime.Now.Year.ToString();
            ViewBag.month= DateTime.Now.Month.ToString();
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "青岛早报", Value = "0"},
                new SelectListItem { Text = "青岛晚报", Value = "1"},
            };
            ViewBag.paperName = new SelectList(list, "Text", "Text");
            int max_id = db.Briefing.Max(d => d.ID);
            ViewBag.ID = max_id + 1;
            return View();
        }

        // POST: Briefings/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,briefTitle,briefYear,briefMonth,paperName,maker,remarks,fileName,briefTime,briefCoTitle")] Briefing briefing)
        {
            string file = Request.Form["MyUploadile"];
            string filename = Request.Form["name"];
            int max_id = db.Briefing.Max(d => d.ID);
            ViewBag.ID = max_id + 1;
            if (ModelState.IsValid)//文件上传
            {
                var files = Request.Files;
                if (files.Count > 0)
                {
                    var file1 = files[0];
                    string strFileSavePath = Request.MapPath("~/files/chengjianInformation");//文件存储路径
                    file1.SaveAs(strFileSavePath + "/" + filename);
                }
                briefing.fileName = filename;
                db.Briefing.Add(briefing);
                db.SaveChanges();
                return RedirectToAction("Details");
            }

                return View(briefing);
            }

        // GET: Briefings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Briefing briefing = db.Briefing.Find(id);
            ViewBag.year = DateTime.Now.Year.ToString();
            ViewBag.month = DateTime.Now.Month.ToString();
            ViewBag.name = briefing.fileName;
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "青岛早报", Value = "0"},
                new SelectListItem { Text = "青岛晚报", Value = "1"},
            };
            ViewBag.paperName = new SelectList(list, "Text", "Text",briefing.paperName);
            if (briefing == null)
            {
                return HttpNotFound();
            }
            return View(briefing);
        }

        // POST: Briefings/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,briefTitle,briefYear,briefMonth,paperName,maker,remarks,fileName,briefTime,briefCoTitle")] Briefing briefing)
        {
            string file = Request.Form["MyUploadile"];
            string filename = Request.Form["name"];
            if (ModelState.IsValid)//文件上传
            {
                var files = Request.Files;
                if (files.Count > 0)
                {
                    var file1 = files[0];
                    string strFileSavePath = Request.MapPath("~/files/chengjianInformation");//文件存储路径
                    file1.SaveAs(strFileSavePath + "/" + filename);
                }
                briefing.fileName = filename;
                db.Entry(briefing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            return View(briefing);
        }

        // GET: Briefings/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Briefing briefing = db.Briefing.Find(id);
        //    if (briefing == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(briefing);
        //}

        // POST: Briefings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Briefing briefing = db.Briefing.Find(id);
            db.Briefing.Remove(briefing);
            db.SaveChanges();
            return Content("<script>alert('已成功删除！');window.location.href='/Briefings/Details'</script>");
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
