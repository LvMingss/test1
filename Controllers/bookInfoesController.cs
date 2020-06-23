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
    public class bookInfoesController : Controller
    {
        private BianYanEntities db = new BianYanEntities();
        public ActionResult PrintBook(int start,int end)
        {
            LocalReport localReport = new LocalReport();
            var ds = db.bookInfo.Where(a => a.bookNo >= start).Where(a => a.bookNo <= end);
            localReport.ReportPath = Server.MapPath("~/Report/BianYan/book.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("book", ds);
            localReport.DataSources.Add(reportDataSource);
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo =
            "<DeviceInfo>" +
            "<OutPutFormat>" + "PDF" + "</OutPutFormat>" +
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
        public ActionResult PrintBookInformation(string action)
        {
            if (action == "打印图书信息")
            {
                int start = int.Parse(Request.Form["startNo"]);
                int end = int.Parse(Request.Form["endNo"]);
                return RedirectToAction("PrintBook","bookInfoes", new { start = start, end = end });
            }
            return View();
                
        }
        public ViewResult BookInformation(string action, int? page)
        {
            db.Configuration.ProxyCreationEnabled = false;
            ViewData["pagename"] = "bookInfoes/BookInformation";
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "书名项", Value = "0"},
                new SelectListItem { Text = "著者", Value = "1"},
                 new SelectListItem { Text = "出版社", Value = "2"}
            };
            ViewBag.Selected = new SelectList(list, "Value", "Text");
            var book = from ad in db.bookInfo
                       orderby ad.bookNo
                       select ad;//按类别排序
            
            if (action == "已借出")
            {
                var chaxun = from ad in db.bookInfo
                             where ad.borrowInfo == "已借出"
                             orderby ad.bookNo
                             select ad;
                ViewBag.result = JsonConvert.SerializeObject(chaxun);
                return View();
            }
            if (action == "查询")
            {
                string n = Request.Form["Selected"];
                string m = Request.Form["serach"];
                if (m != "")
                {
                    ViewBag.Selected = new SelectList(list, "Value", "Text", n);
                    ViewBag.search = m;
                    if (n == "0")
                    {

                        var chaxun = from ad in db.bookInfo
                                     where ad.bookName.Contains(m)
                                     orderby ad.bookNo
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "1")
                    {
                        var chaxun = from ad in db.bookInfo
                                     where ad.authorZhu.Contains(m)
                                     orderby ad.bookNo
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "2")
                    {
                        var chaxun = from ad in db.bookInfo
                                     where ad.press.Contains(m)
                                     orderby ad.bookNo
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                }
            }
            ViewBag.result = JsonConvert.SerializeObject(book);
            return View();
        }
        // GET: bookInfoes
        public ViewResult Index()
        {
         

       
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "书名项", Value = "0"},
                new SelectListItem { Text = "著者", Value = "1"},
                 new SelectListItem { Text = "出版社", Value = "2"}
            };
            ViewBag.Selected = new SelectList(list, "Value", "Text");
            var book = from ad in db.bookInfo
                       orderby ad.bookNo descending 
                       select ad;//按类别排序
            ViewBag.result = JsonConvert.SerializeObject(book);
            return View();
           
        }
        [HttpPost]
        public ViewResult Index(string action, string Selected, string serach)
        {
          


            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "书名项", Value = "0"},
                new SelectListItem { Text = "著者", Value = "1"},
                 new SelectListItem { Text = "出版社", Value = "2"}
            };
          
            var book = from ad in db.bookInfo
              
                       select ad;//按类别排序

          

            if (action == "已借出")
            {

                var chaxun = from ad in db.bookInfo
                             where ad.borrowInfo == "已借出"
                             orderby ad.bookNo
                             select ad;
                ViewBag.result = JsonConvert.SerializeObject(chaxun);
                ViewBag.Selected = new SelectList(list, "Value", "Text");
                return View();
            }
            if (action == "查询")
            {

                if (serach != "" && serach != null)//用户在检索框中输入了检索条件
                {
                    int t = Int32.Parse(Selected.Trim());
                    ViewBag.Selected = new SelectList(list, "Value", "Text", t);
                    ViewBag.serach = serach;

                    switch (t)
                    {
                        case 0:
                            book = book.Where(ad => ad.bookName.Contains(serach));//根据责任书编号搜索
                            break;
                        case 1:
                            book = book.Where(ad => ad.authorZhu.Contains(serach));//根据工程名称搜索
                            break;
                        case 2:
                            book = book.Where(ad => ad.press.Contains(serach));//根据建设单位搜索
                            break;
                    }
                }
                else
                {
                    ViewBag.Selected = new SelectList(list, "Value", "Text", Selected);
                    ViewBag.serach = serach;
                }

            }

            book = book.OrderByDescending(a => a.bookNo);
            ViewBag.result = JsonConvert.SerializeObject(book);
            return View();
          
        }
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult Borrow(int ? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bookInfo bookInfo = db.bookInfo.Find(id);
            ViewBag.no = bookInfo.bookNo;
            if (bookInfo == null)
            {
                return HttpNotFound();
            }
           string book = bookInfo.borrowInfo;
            if (book == "未借出")
            {
                ViewBag.borrowDate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.returnDate= "";
                ViewBag.borrower = "";
                ViewData["button1"] = false;
                ViewData["button2"] = true;
            }
            if (book == "已借出")
            {
                var bookborrowinfo = db.BookBorrowInfo.Where(a => a.bookNo == id).First();
                ViewBag.returnDate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.borrowDate = bookborrowinfo.borrowDate;
                ViewBag.borrower = bookborrowinfo.borrower;
                ViewData["button1"] = true;
                ViewData["button2"] = false;
            }
            return View(bookInfo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Borrow([Bind(Include = "bookNo,bookName,authorZhu,authorBian,authorJi,authorZuan,authorYi,authorHui,press,publishDate,price,classificationNo,suoShuNo,huJianNo,pageCount,summary,borrowInfo,remarks")] bookInfo bookInfo, string action)
        {
            if (action == "返回")
            {
                return RedirectToAction("Index");
            }
            if (action == "借阅")
            {
                bookInfo.borrowInfo = "已借出";
                BookBorrowInfo bookborrowinfo = new BookBorrowInfo();
                var no = Request.Form["bookNo"];
                bookborrowinfo.bookNo = int.Parse(no);
                bookborrowinfo.borrower = Request.Form["borrower"];
                bookborrowinfo.borrowDate = DateTime.Parse(Request.Form["borrowDate"]);
                int maxid= db.BookBorrowInfo.Max(d => d.ID);
                bookborrowinfo.ID = maxid + 1;
                
                if (ModelState.IsValid)
                {
                    db.Entry(bookInfo).State = EntityState.Modified;
                    //db.SaveChanges();
                    db.BookBorrowInfo.Add(bookborrowinfo);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            if (action == "归还")
            {
                bookInfo.borrowInfo = "未借出";
                var no = int.Parse(Request.Form["bookNo"]);
                var book = db.BookBorrowInfo.Where(a => a.bookNo == no).First();
                book.bookNo = int.Parse(Request.Form["bookNo"]);
                book.borrower = Request.Form["borrower"];
                book.borrowDate = DateTime.Parse(Request.Form["borrowDate"]);
                book.returnDate = DateTime.Parse(Request.Form["returnDate"]);

                //bookborrowinfo.ID = book.ID;
                if (ModelState.IsValid)
                {
                    db.Entry(bookInfo).State = EntityState.Modified;
                    db.Entry(book).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: bookInfoes/Details/5
        public ActionResult Details(int? id,string action,string id2)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bookInfo bookInfo = db.bookInfo.Find(id);
            
            if (bookInfo.borrowInfo == "未借出")
            {
                ViewBag.borrower = "";
                ViewBag.borrowdate = "";
            }
            else
            {
                BookBorrowInfo bookborrowInfo = db.BookBorrowInfo.Where(a => a.bookNo == id).First();
                ViewBag.borrower = bookborrowInfo.borrower;
                ViewBag.borrowdate = bookborrowInfo.borrowDate;
            }
            if (bookInfo == null)
            {
                return HttpNotFound();
            }
            if (id2 == "2")
            {
                ViewData["button"] = "display:show";
            }
            else
            {
                ViewData["button"] = "display:none";
            }
            if (action == "返回")
            {
                if (id2 == "2")
                {
                    return RedirectToAction("Index");
                }
                else {
                    return RedirectToAction("BookInformation");
                }
            }
            return View(bookInfo);
        }

        // GET: bookInfoes/Create
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: bookInfoes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "bookNo,bookName,authorZhu,authorBian,authorJi,authorZuan,authorYi,authorHui,press,publishDate,price,classificationNo,suoShuNo,huJianNo,pageCount,summary,borrowInfo,remarks")] bookInfo bookInfo,string action)
        {
            if (action=="保存")
            {


                int max_no = db.bookInfo.Max(d => d.bookNo);
                bookInfo.bookNo = max_no + 1;
                if (bookInfo.price == null)
                {
                    bookInfo.price = 0;
                }
                //if (bookInfo.publishDate == null)
                //{
                //    bookInfo.publishDate = DateTime.Today.Date;
                //}

                try
                {
                    db.bookInfo.Add(bookInfo);
                    db.SaveChanges();
                    return Content("<script >alert('保存成功！');window.history.back();</script >");
                }

                catch (Exception)
                {
                    return Content("<script >alert('保存失败，请检查所填信息！');window.history.back();</script >");
                }
            }
            else
            {
                return RedirectToAction("Create");
            }
          
        }

        // GET: bookInfoes/Edit/5
        public ActionResult Edit(int? id,string action,string id2)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bookInfo bookInfo = db.bookInfo.Find(id);
            if (bookInfo == null)
            {
                return HttpNotFound();
            }
            if (action == "返回")
            {
                if (id2 == "2")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("BookInformation");
                }
            }
            return View(bookInfo);
        }

        // POST: bookInfoes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "bookNo,bookName,authorZhu,authorBian,authorJi,authorZuan,authorYi,authorHui,press,publishDate,price,classificationNo,suoShuNo,huJianNo,pageCount,summary,borrowInfo,remarks")] bookInfo bookInfo,string action)
        {
            if(action=="修改")
            {
                int max_no = db.bookInfo.Max(d => d.bookNo);
                bookInfo.bookNo = max_no + 1;
                if (bookInfo.price == null)
                {
                    bookInfo.price = 0;
                }
                if (bookInfo.publishDate == null)
                {
                    bookInfo.publishDate = DateTime.Today.Date;
                }

                try
                {
                    db.bookInfo.Add(bookInfo);
                    db.SaveChanges();
                    return Content("<script >alert('修改成功！');window.history.back();</script >");
                }

                catch (Exception)
                {
                    return Content("<script >alert('修改失败，请检查所填信息！');window.history.back();</script >");
                }
            }
          
              else
            {
                return RedirectToAction("Index");
            }
           
            return View(bookInfo);
        }

        // GET: bookInfoes/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    bookInfo bookInfo = db.bookInfo.Find(id);
        //    if (bookInfo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bookInfo);
        //}

        // POST: bookInfoes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            bookInfo bookInfo = db.bookInfo.Find(id);
            db.bookInfo.Remove(bookInfo);
            db.SaveChanges();
            return Content("<script>alert('已成功删除！');window.location.href='/bookInfoes/Index'</script>");
        }

       
    }
}
