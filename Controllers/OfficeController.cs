using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using urban_archive.Models;
using Newtonsoft.Json;
using Microsoft.Reporting.WebForms;

namespace urban_archive.Controllers
{
    public class OfficeController : Controller
    {
        private OfficeEntities db = new OfficeEntities();
        public ActionResult chulidan(int id, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds = from ad in db.BaowenMainTable
                     where ad.ID == id
                     select ad;
                localReport.ReportPath = Server.MapPath("~/Report/gongwen/chulidan.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("chulidan", ds);
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
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult dayin()
        {
            return View();
        }
        // GET: Office
        public ActionResult Index( string action)
        {
            ViewData["pagename"] = "Office";
            ViewBag.File = new SelectList(db.BaowenFileType, "类别", "说明");
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "文件标题", Value = "0"},
                new SelectListItem { Text = "来源单位", Value = "1"},
                new SelectListItem { Text = "收文日期", Value = "2"},
                new SelectListItem { Text = "是否办结", Value = "3"}
            };
            ViewBag.Selected = new SelectList(list, "Value", "Text");
            var gongwen = from ad in db.BaowenMainTable
                          //orderby ad.leibie
                          orderby ad.ID descending
                          select ad;//按类别排序
            if (action == "查询")
            {
                string t = Request.Form["File"];
                string n = Request.Form["Selected"];
                string m = Request.Form["search"];
                ViewBag.File = new SelectList(db.BaowenFileType, "类别", "说明", t);
                ViewBag.Selected = new SelectList(list, "Value", "Text", n);
                ViewBag.search = m;
                if (m != "")
                {


                    if (n == "0")
                    {

                        var chaxun = from ad in db.BaowenMainTable
                                     where ad.leibie == t
                                     where ad.wenjianbiaoti.Contains(m)
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "1")
                    {
                        var chaxun = from ad in db.BaowenMainTable
                                     where ad.leibie == t
                                     where ad.laiwendanwei.Contains(m)
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "2")
                    {
                        var stdate = DateTime.Parse(m.Split('-')[0].Trim());
                        var endate = DateTime.Parse(m.Split('-')[1].Trim());
                        var chaxun = from ad in db.BaowenMainTable
                                     where ad.leibie == t
                                     where ad.shouwenriqi>= stdate
                                     where ad.shouwenriqi<=endate
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "3")
                    {
                        bool banjie = false;
                        if (m == "是")
                        {
                            banjie = true;
                        }
                        var chaxun = from ad in db.BaowenMainTable
                                     where ad.leibie == t
                                     where ad.banjie == banjie
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }

                }
                else
                {




                    var chaxun = from ad in db.BaowenMainTable
                                 where ad.leibie == t
                                
                                 orderby ad.ID descending
                                 select ad;



                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();












                }

            }
            ViewBag.result = JsonConvert.SerializeObject(gongwen);
            return View();
        }

        // GET: Office/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaowenMainTable baowenMainTable = db.BaowenMainTable.Find(id);
            if (baowenMainTable == null)
            {
                return HttpNotFound();
            }
            return View(baowenMainTable);
        }
        public JsonResult shunxuNo(string id)//jsy动态框方法
        {
            string classNo = id;
            int year = DateTime.Now.Year;
            string max_shunxuhao = db.BaowenMainTable.Where(d => d.leibie == classNo).Max(d => d.shunxuhao).ToString();//最大顺序号
            string shunxuhao = year + "0001";
            if (int.Parse(max_shunxuhao.Substring(0, 4)) == year)
            {
                string shunxuhao1 = (int.Parse(max_shunxuhao) + 1).ToString();
                return Json(shunxuhao1, JsonRequestBehavior.AllowGet);
            }
            return Json(shunxuhao, JsonRequestBehavior.AllowGet);
        }
        // GET: Office/Create
        public ActionResult Create()
        {
            BaowenMainTable baowenmaintable = new BaowenMainTable();
            long max_ID = db.BaowenMainTable.Max(d => d.ID);
            ViewBag.ID = max_ID + 1;
            ViewBag.leibie = new SelectList(db.BaowenFileType, "类别", "说明");
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = " "},
                new SelectListItem { Text = "10:00", Value = "10:00"},
                new SelectListItem { Text = "11:00", Value = "11:00"},
                new SelectListItem { Text = "11:30", Value = "11:30"},
                new SelectListItem { Text = "12:00", Value = "12:00"},
                new SelectListItem { Text = "13:00", Value = "13:00"},
                new SelectListItem { Text = "14:00", Value = "14:00"},
                new SelectListItem { Text = "15:00", Value = "15:00"},
                new SelectListItem { Text = "16:00", Value = "16:00"},
                new SelectListItem { Text = "17:00", Value = "17:00"},
                new SelectListItem { Text = "17:30", Value = "17:30"},
            };
            ViewBag.banjietime = new SelectList(list, "Value", "Text");
            ViewData["next"] = true;
            ViewData["chulidan"] = true;
            return View();
        }

        // POST: Office/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,leibie,shunxuhao,shouwenriqi,wenjianzihao,laiwendanwei,wenjianbiaoti,wenjianyaoqiu,banjieriqi,guidangjuanbie,banjietime")] BaowenMainTable baowenMainTable,string action)
        {
            ViewBag.leibie = new SelectList(db.BaowenFileType, "类别", "说明");
            ViewBag.shunxuhao = baowenMainTable.shunxuhao;
            ViewBag.ID = baowenMainTable.ID;
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = " "},
                new SelectListItem { Text = "10:00", Value = "10:00"},
                new SelectListItem { Text = "11:00", Value = "11:00"},
                new SelectListItem { Text = "11:30", Value = "11:30"},
                new SelectListItem { Text = "12:00", Value = "12:00"},
                new SelectListItem { Text = "13:00", Value = "13:00"},
                new SelectListItem { Text = "14:00", Value = "14:00"},
                new SelectListItem { Text = "15:00", Value = "15:00"},
                new SelectListItem { Text = "16:00", Value = "16:00"},
                new SelectListItem { Text = "17:00", Value = "17:00"},
                new SelectListItem { Text = "17:30", Value = "17:30"},
            };
            if (baowenMainTable.banjietime == "")
            {
                ViewBag.banjietime = new SelectList(list, "Value", "Text");
            }
            else
            {
                ViewBag.banjietime = new SelectList(list, "Value", "Text", baowenMainTable.banjietime.Trim());
            }
            if (action == "保存")
            {
                if (ModelState.IsValid)
                {
                    db.BaowenMainTable.Add(baowenMainTable);
                    db.SaveChanges();
                    Response.Write("<script>alert('已成功保存！');</script>");
                    ViewData["next"] = false;
                    ViewData["chulidan"] = false;
                }
            }
            if (action == "添加下一条")
            {
                int Id = int.Parse(Request.Form["ID"]);
                var ds = from ad in db.BaowenMainTable
                         where ad.ID == Id
                         select ad;
                if (ds.Count() == 0)
                {
                    Response.Write("<script>alert('请先提交保存!');</script>");
                }
                else
                {
                    return RedirectToAction("Create");
                }
            }
            if (action == "处理单")
            {
                int Id = int.Parse(Request.Form["ID"]);
                var ds = from ad in db.BaowenMainTable
                         where ad.ID == Id
                         select ad;
                if (ds.Count() == 0)
                {
                    Response.Write("<script>alert('请先提交保存!');</script>");
                }
                else
                {
                    return RedirectToAction("chulidan", new { format = "PDF", id = Id });
                }
            }
            return View(baowenMainTable);
        }

        // GET: Office/Edit/5
        public ActionResult Edit(int? id)
        {
            BaowenMainTable baowenMainTable = db.BaowenMainTable.Find(id);
            int lei = int.Parse(baowenMainTable.leibie);
            BaowenFileType baowenfiletype = db.BaowenFileType.Find(lei);
            ViewBag.leibie = new SelectList(db.BaowenFileType, "类别", "说明", baowenfiletype.类别);
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = " "},
                new SelectListItem { Text = "10:00", Value = "10:00"},
                new SelectListItem { Text = "11:00", Value = "11:00"},
                new SelectListItem { Text = "11:30", Value = "11:30"},
                new SelectListItem { Text = "12:00", Value = "12:00"},
                new SelectListItem { Text = "13:00", Value = "13:00"},
                new SelectListItem { Text = "14:00", Value = "14:00"},
                new SelectListItem { Text = "15:00", Value = "15:00"},
                new SelectListItem { Text = "16:00", Value = "16:00"},
                new SelectListItem { Text = "17:00", Value = "17:00"},
                new SelectListItem { Text = "17:30", Value = "17:30"},
            };
            if (baowenMainTable.banjietime == "")
            {
                ViewBag.banjietime = new SelectList(list, "Value", "Text");
            }
            else {
                ViewBag.banjietime = new SelectList(list, "Value", "Text", baowenMainTable.banjietime.Trim());
            }         
            ViewBag.banjieriqi = baowenMainTable.banjieriqi.Value.ToString("yyyy-MM-dd");
            ViewBag.shouwenriqi = baowenMainTable.shouwenriqi.Value.ToString("yyyy-MM-dd");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }         
           
            if (baowenMainTable == null)
            {
                return HttpNotFound();
            }
            return View(baowenMainTable);
        }

        // POST: Office/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,leibie,shunxuhao,shouwenriqi,wenjianzihao,laiwendanwei,wenjianbiaoti,chengwenriqi,fenshu,guidangjuanbie,nibanyijian,yiyue,zhengchuli,lingdaopishi,banliqingkuang,chengbandanwei,fankuaiqingkuang,banjie,banjieriqi,beizhu,banjietime,wenjianyaoqiu")] BaowenMainTable baowenMainTable, int? id,string action)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = " "},
                new SelectListItem { Text = "10:00", Value = "10:00"},
                new SelectListItem { Text = "11:00", Value = "11:00"},
                new SelectListItem { Text = "11:30", Value = "11:30"},
                new SelectListItem { Text = "12:00", Value = "12:00"},
                new SelectListItem { Text = "13:00", Value = "13:00"},
                new SelectListItem { Text = "14:00", Value = "14:00"},
                new SelectListItem { Text = "15:00", Value = "15:00"},
                new SelectListItem { Text = "16:00", Value = "16:00"},
                new SelectListItem { Text = "17:00", Value = "17:00"},
                new SelectListItem { Text = "17:30", Value = "17:30"},
            };
            if (baowenMainTable.banjietime == "")
            {
                ViewBag.banjietime = new SelectList(list, "Value", "Text");
            }
            else
            {
                ViewBag.banjietime = new SelectList(list, "Value", "Text", baowenMainTable.banjietime.Trim());
            }
            if (action == "保存")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(baowenMainTable).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("<script >alert('保存成功！');window.history.back();</script >");
                }
            }
            if (action == "处理单")
            {
                int Id = int.Parse(Request.Form["ID"]);
                var ds = from ad in db.BaowenMainTable
                         where ad.ID == Id
                         select ad;
                if (ds.Count() == 0)
                {
                    Response.Write("<script>alert('请先提交保存!');</script>");
                }
                else
                {
                    return RedirectToAction("chulidan", new { format = "PDF", id = Id });
                }
            }
            return View(baowenMainTable);
        }

        public ActionResult Niban(int? id)
        {
            long max_ID = db.BaowenMainTable.Max(d => d.ID);
            long min_ID = db.BaowenMainTable.Min(d => d.ID);
            BaowenMainTable baowenMainTable = db.BaowenMainTable.Find(id);
            int lei = int.Parse(baowenMainTable.leibie);
            BaowenFileType baowenfiletype = db.BaowenFileType.Find(lei);
            ViewBag.leibie = new SelectList(db.BaowenFileType, "类别", "说明", baowenfiletype.类别);
            ViewBag.leader = new SelectList(db.BaowenLeaderList.OrderBy(ad=>ad.顺序), "姓名", "姓名");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id == max_ID)
            {
                ViewData["button2"] = true;
            }
            if (id == min_ID)
            {
                ViewData["button1"] = true;
            }
            if (baowenMainTable == null)
            {
                return HttpNotFound();
            }
            return View(baowenMainTable);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Niban([Bind(Include = "ID,leibie,shunxuhao,shouwenriqi,wenjianzihao,laiwendanwei,wenjianbiaoti,wenjianyaoqiu,fenshu,guidangjuanbie,nibanyijian,yiyue,zhengchuli,lingdaopishi,banliqingkuang,chengbandanwei,fankuaiqingkuang,banjie,banjieriqi,beizhu,banjietime")] BaowenMainTable baowenMainTable, int? id,string action)
        {
            long max_ID = db.BaowenMainTable.Max(d => d.ID);
            long min_ID = db.BaowenMainTable.Min(d => d.ID);
            if (action == "保存")
            {
                int? ID = id;
                if (ModelState.IsValid)
                {
                    db.Entry(baowenMainTable).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("<script >alert('保存成功！');window.history.back();</script >");
                }
            }
            if (action == "下一个")
            {
                int? ID2 = id + 1;
                if (id == max_ID)
                {
                    return RedirectToAction("Niban", new { id = id });
                }
                else
                {
                    return RedirectToAction("Niban", new { id = ID2 });
                }
            }

            if (action == "上一个")
            {
                int? ID1 = id - 1;
                if (id == min_ID)
                {
                    return RedirectToAction("Niban", new { id = id });
                }
                else
                {
                    return RedirectToAction("Niban", new { id = ID1 });
                }
            }
            return View(baowenMainTable);
        }

        public ActionResult Pishi(int? id)
        {
            BaowenMainTable baowenMainTable = db.BaowenMainTable.Find(id);
            int lei = int.Parse(baowenMainTable.leibie);
            BaowenFileType baowenfiletype = db.BaowenFileType.Find(lei);
            ViewBag.leibie = new SelectList(db.BaowenFileType, "类别", "说明", baowenfiletype.类别);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (baowenMainTable == null)
            {
                return HttpNotFound();
            }
            return View(baowenMainTable);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pishi([Bind(Include = "ID,leibie,shunxuhao,shouwenriqi,wenjianzihao,laiwendanwei,wenjianbiaoti,chengwenriqi,fenshu,guidangjuanbie,nibanyijian,yiyue,zhengchuli,lingdaopishi,banliqingkuang,chengbandanwei,fankuaiqingkuang,banjie,banjieriqi,beizhu,wenjianyaoqiu,banjietime")] BaowenMainTable baowenMainTable, int? id)
        {
            if (ModelState.IsValid)
            {
                db.Entry(baowenMainTable).State = EntityState.Modified;
                db.SaveChanges();
                return Content("<script >alert('保存成功！');window.history.back();</script >");
            }
        
            return View(baowenMainTable);
        }

        public ActionResult Liuzhuan(int? id)
        {
            long max_ID = db.BaowenMainTable.Max(d => d.ID);
            long min_ID = db.BaowenMainTable.Min(d => d.ID);
            BaowenMainTable baowenMainTable = db.BaowenMainTable.Find(id);
            int lei = int.Parse(baowenMainTable.leibie);
            BaowenFileType baowenfiletype = db.BaowenFileType.Find(lei);
            ViewBag.leibie = new SelectList(db.BaowenFileType, "类别", "说明", baowenfiletype.类别);
            ViewBag.zhengchuli = new SelectList(db.BaowenLeaderList.OrderBy(ad => ad.顺序), "姓名", "姓名",baowenMainTable.zhengchuli);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id == max_ID)
            {
                ViewData["button2"] = true;
            }
            if (id == min_ID)
            {
                ViewData["button1"] = true;
            }
            if (baowenMainTable == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

            };
            if(baowenMainTable.banjie==true)
            {
                ViewBag.banjie = new SelectList(list, "Value", "Text",1);
            }
           else
            {
                ViewBag.banjie = new SelectList(list, "Value", "Text", 0);
            }
            return View(baowenMainTable);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Liuzhuan([Bind(Include = "ID,leibie,shunxuhao,shouwenriqi,wenjianzihao,laiwendanwei,wenjianbiaoti,chengwenriqi,fenshu,guidangjuanbie,nibanyijian,yiyue,zhengchuli,lingdaopishi,banliqingkuang,chengbandanwei,fankuaiqingkuang,banjieriqi,beizhu,wenjianyaoqiu,banjietime")] BaowenMainTable baowenMainTable, int? id,string action,string banjie)
        {
            long max_ID = db.BaowenMainTable.Max(d => d.ID);
            long min_ID = db.BaowenMainTable.Min(d => d.ID);
            if (action == "保存")
            {
                int? ID = id ;
                if (ModelState.IsValid)
                {
                    if(banjie=="1")
                    {
                        baowenMainTable.banjie = true;
                    }
                    else
                    {
                        baowenMainTable.banjie = false;
                    }
                    db.Entry(baowenMainTable).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("<script >alert('保存成功！');window.history.back();</script >");
                
                }
            }
            if (action == "下一个")
            {
                int? ID2 = id + 1;
                if (id == max_ID)
                {
                    return RedirectToAction("Liuzhuan", new { id = id });
                }
                else
                {
                    return RedirectToAction("Liuzhuan", new { id = ID2 });
                }
            }

            if (action == "上一个")
            {
                int? ID1 = id - 1;
                if (id == min_ID)
                {
                    return RedirectToAction("Liuzhuan", new { id = id });
                }
                else
                {
                    return RedirectToAction("Liuzhuan", new { id = ID1 });
                }


            }
            return View(baowenMainTable);
        }
     
        public ActionResult Delete(int id)
        {
            BaowenMainTable baowenMainTable = db.BaowenMainTable.Find(id);
            db.BaowenMainTable.Remove(baowenMainTable);
            db.SaveChanges();
            return Content("<script>alert('已成功删除！');window.location.href='/Office/Index'</script>");
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
