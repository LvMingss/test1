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
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace urban_archive.Controllers
{
    public class OfficeDanganZhuangjuController : Controller
    {
        private OfficeEntities db = new OfficeEntities();
        private UrbanConEntities ab = new UrbanConEntities();
        private UrbanUsersEntities cb = new UrbanUsersEntities();
        public ActionResult HeJuFeiYongEdit(long? id)
        {
            ArchivesContainer archivesContainer = db.ArchivesContainer.Find(id);
            var charge = ab.Charger.Where(a => a.searchNo == id).First();
            ViewBag.total = charge.totalExpense;
            if (charge.isCharge == true)
            {
                Response.Write("<script>alert('此费用已经收取，不能进行修改 !');window.history.back();</script>");
            }

            ViewBag.Selected = new SelectList(ab.DepartmentCode, "value", "Text", charge.fromDepartment);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "转向财务科", Value = "1"},
                new SelectListItem { Text = "转向复印室", Value = "0"},
            };

            ViewBag.Selected1 = new SelectList(list1, "Value", "Text", charge.whereTransfer);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.seqNo = archivesContainer.seqNo;
            ViewBag.totle = charge.totalExpense;
            ViewBag.twoCentiCnt = archivesContainer.twoCentiCnt;
            ViewBag.thrCentiCnt = archivesContainer.thrCentiCnt;
            ViewBag.fourCentiCnt = archivesContainer.fourCentiCnt;
            ViewBag.fiveCentiCnt = archivesContainer.fiveCentiCnt;
            ViewBag.coverCnt = archivesContainer.coverCnt;
            ViewBag.catalogueCnt = archivesContainer.catalogueCnt;
            ViewBag.proformaCnt = archivesContainer.proformaCnt;
            if (archivesContainer == null)
            {
                return HttpNotFound();
            }
            return View(archivesContainer);
        }

        // POST: OfficeDanganZhuangju/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HeJuFeiYongEdit(string action, [Bind(Include = "ID,twoCentiCnt,thrCentiCnt,fourCentiCnt,fiveCentiCnt,coverCnt,catalogueCnt,proformaCnt,DepartName,submitDate,submitPerson,archiveBoxFee,archiveCataLogFee,singleBoxFee,singleCatalogFee,seqNo")]ArchivesContainer archivesContainer, Charger charger)
        {
            long id = long.Parse(Request.Form["ID"]);
            var charge = from ad in ab.Charger
                         where ad.searchNo == id
                         select ad;
            var charge1 = charge.First();
            ViewBag.total = charge1.totalExpense;
            ViewBag.Selected = new SelectList(ab.DepartmentCode, "value", "Text", charge1.fromDepartment);
            List<SelectListItem> list3 = new List<SelectListItem> {
                new SelectListItem { Text = "转向财务科", Value = "1"},
                new SelectListItem { Text = "转向复印室", Value = "0"},
            };

            ViewBag.Selected1 = new SelectList(list3, "Value", "Text", charge1.whereTransfer);
            //Charger charger = new Charger();
            long max_chargerID = ab.Charger.Max(d => d.ID);
            long newchargerID = max_chargerID + 1;
            string totle = Request.Form["total"];
            string zhuanxiang = Request.Form["Selected1"];
            string data = Request.Form["submitDate"];
            string no = data.Replace("-", "");
            if (action == "生成最大收费编号")
            {
                if (data.Contains("-"))
                {
                    if (ModelState.IsValid)
                    {
                        var list1 = from ad in db.ArchivesContainer
                                    where ad.seqNo.Contains(no)
                                    orderby ad.ID descending
                                    select ad;
                        if (list1.Count() != 0)
                        {
                            var list = list1.First();
                            long max_seqno = long.Parse(list.seqNo);
                            long seqno = max_seqno + 1;
                            archivesContainer.seqNo = seqno.ToString();
                            ViewBag.seqNo = archivesContainer.seqNo;
                            ViewBag.totle = charge1.totalExpense;
                            ViewBag.twoCentiCnt = archivesContainer.twoCentiCnt;
                            ViewBag.thrCentiCnt = archivesContainer.thrCentiCnt;
                            ViewBag.fourCentiCnt = archivesContainer.fourCentiCnt;
                            ViewBag.fiveCentiCnt = archivesContainer.fiveCentiCnt;
                            ViewBag.coverCnt = archivesContainer.coverCnt;
                            ViewBag.catalogueCnt = archivesContainer.catalogueCnt;
                            ViewBag.proformaCnt = archivesContainer.proformaCnt;
                        }
                        else
                        {
                            archivesContainer.seqNo = no + "001";
                            ViewBag.seqNo = archivesContainer.seqNo;
                            ViewBag.totle = charge1.totalExpense;
                            ViewBag.twoCentiCnt = archivesContainer.twoCentiCnt;
                            ViewBag.thrCentiCnt = archivesContainer.thrCentiCnt;
                            ViewBag.fourCentiCnt = archivesContainer.fourCentiCnt;
                            ViewBag.fiveCentiCnt = archivesContainer.fiveCentiCnt;
                            ViewBag.coverCnt = archivesContainer.coverCnt;
                            ViewBag.catalogueCnt = archivesContainer.catalogueCnt;
                            ViewBag.proformaCnt = archivesContainer.proformaCnt;
                        }
                    }
                }
                else
                    Response.Write("<script >alert('请选择提交时间');window.history.back();</script >");
            }
            if (action == "保存")
            {
                if (ModelState.IsValid)
                {
                    string NO = Request.Form["seqNo"];
                    archivesContainer.seqNo = NO;
                    string person = Request.Form["Selected"];
                    archivesContainer.submitPerson = person;
                    db.Entry(archivesContainer).State = EntityState.Modified;
                    db.SaveChanges();
                    charge1.searchNo = archivesContainer.ID;
                    charge1.totalExpense = decimal.Parse(totle);
                    charge1.unitName = archivesContainer.DepartName;
                    charge1.chargeTime = archivesContainer.submitDate;
                    charge1.seqNo = archivesContainer.seqNo;
                    charge1.theoryExpense = decimal.Parse(totle);
                    charge1.whereTransfer = int.Parse(zhuanxiang);
                    ab.Entry(charge1).State = EntityState.Modified;
                    ab.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(archivesContainer);
        }

        public ActionResult FeiYongChaXun(long?id,string action)
        {
            ViewData["pagename"] = "OfficeDanganZhuangju/FeiYongChaXun";           

            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "事项名称", Value = "0"},
                new SelectListItem { Text = "缴费单位", Value = "1"},
                 new SelectListItem { Text = "缴费日期(如2016-1-1或2016.1.1)", Value = "2"},
                  new SelectListItem { Text = "编号", Value = "3"}
            };
            ViewBag.dropdowmlist = new SelectList(list, "Value", "Text");
            var UserID = User.Identity.GetUserId();
            var department = cb.AspNetUsers.Find(UserID).DepartmentName;
            if (action == "查询")
            {
                
                string m = Request.Form["search"];
                string n = Request.Form["dropdowmlist"];
                if (m != null)
                {
                    ViewBag.dropdowmlist = new SelectList(list, "Value", "Text", n);
                    ViewBag.select =m;
                    if (n == "0")
                    {
                        var chaxun = from ad in ab.vw_charge
                                     where ad.itemName.Contains(m)
                                     where ad.text.Trim() == department
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                        //return View(chaxun.ToPagedList(pageNumber, pageSize));
                    }
                    if (n == "1")
                    {
                        var chaxun = from ad in ab.vw_charge
                                     where ad.unitName.Contains(m)
                                     where ad.text.Trim() == department
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "2")
                    {
                        DateTime time = DateTime.Parse(m);
                        var chaxun = from ad in ab.vw_charge
                                     where ad.chargeTime == time
                                     where ad.text.Trim() == department
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "3")
                    {
                        var chaxun = from ad in ab.vw_charge
                                     where ad.seqNo.Contains(m)
                                     where ad.text.Trim() == department
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                }
            }
            var danganzhuangju = ab.vw_charge.Where(a => a.text.Trim() == department).OrderByDescending(a => a.ID);
            ViewBag.result = JsonConvert.SerializeObject(danganzhuangju);
            return View();
        }
        // GET: OfficeDanganZhuangju
        public ActionResult Index(string action)
        {
            ViewData["pagename"] = "OfficeDanganZhuangju";

            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "单位名称", Value = "0"},
                new SelectListItem { Text = "提交人", Value = "1"},
                 new SelectListItem { Text = "提交日期(时间段以~分割)", Value = "2"}
            };
            ViewBag.dropdowmlist = new SelectList(list, "Value", "Text");
            var danganzhuangju = db.ArchivesContainer.OrderByDescending(a => a.ID);
            string m = Request.Form["Search"];
            ViewBag.chaxun = m;
            if (!String.IsNullOrEmpty(m))
            {
                int n = int.Parse(Request.Form["dropdowmlist"]);
                ViewBag.dropdowmlist = new SelectList(list, "Value", "Text", n);
                switch (n)
                {
                    case 0:
                        danganzhuangju = db.ArchivesContainer.Where(a => a.DepartName.Contains(m)).OrderByDescending(a => a.ID);
                        break;
                    case 1:
                        danganzhuangju = db.ArchivesContainer.Where(a => a.submitPerson.Contains(m)).OrderByDescending(a => a.ID);
                        break;
                    case 2:
                        if (m.Contains("~"))
                        {
                            DateTime starttime = DateTime.Parse(m.Substring(0, m.IndexOf("~")));
                            DateTime endtime = DateTime.Parse(m.Substring(m.IndexOf("~") + 1));
                            danganzhuangju = db.ArchivesContainer.Where(ad => ad.submitDate <= endtime).Where(ad => ad.submitDate >= starttime).OrderByDescending(a => a.ID);
                        }
                        else {
                            DateTime time = DateTime.Parse(m);
                            danganzhuangju = db.ArchivesContainer.Where(ad => ad.submitDate == time).OrderByDescending(a => a.ID);
                        } 
                        break;
                }
            }
            ViewBag.result = JsonConvert.SerializeObject(danganzhuangju);
            return View();
        }

        // GET: OfficeDanganZhuangju/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivesContainer archivesContainer = db.ArchivesContainer.Find(id);
            long NO = archivesContainer.ID;
            var charger = from ad in ab.Charger
                          where ad.searchNo ==NO
                          select ad;
            var charger1 = charger.First();
            ViewBag.totle = charger1.totalExpense;
            ViewBag.trasfer = charger1.whereTransfer;
            if (archivesContainer == null)
            {
                return HttpNotFound();
            }
            return View(archivesContainer);
        }

        // GET: OfficeDanganZhuangju/Create
        public ActionResult Create()
        {
            ViewData["pagename"] = "OfficeDanganZhuangju/Create";
            var UserID = User.Identity.GetUserId();
            var department1 = cb.AspNetUsers.Find(UserID).DepartmentName;
            var departmentid = ab.DepartmentCode.Where(a => a.text == department1).First().value;
            ViewBag.Selected = new SelectList(ab.DepartmentCode, "value", "Text", departmentid);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "转向财务科", Value = "1"},
                new SelectListItem { Text = "转向复印室", Value = "0"},
            };
            ViewBag.Selected1 = new SelectList(list1, "Value", "Text",1);
            ViewBag.DATE = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        // POST: OfficeDanganZhuangju/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "twoCentiCnt,thrCentiCnt,fourCentiCnt,fiveCentiCnt,coverCnt,catalogueCnt,proformaCnt,DepartName,submitDate,submitPerson,archiveBoxFee,archiveCataLogFee,singleBoxFee,singleCatalogFee")] ArchivesContainer archivesContainer)
        {
            
            var UserID = User.Identity.GetUserId();
            var department1 = cb.AspNetUsers.Find(UserID).DepartmentName;
            var departmentid = ab.Charger.Where(a => a.@operator == department1).First().fromDepartment;
            ViewBag.Selected = new SelectList(ab.DepartmentCode, "value", "Text", departmentid);
            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "转向财务科", Value = "1"},
                new SelectListItem { Text = "转向复印室", Value = "0"},
            };
            ViewBag.Selected1 = new SelectList(list2, "Value", "Text");
            Charger charger = new Charger(); 
            long max_ID = db.ArchivesContainer.Max(d => d.ID);
            long newID = max_ID + 1;
            long max_chargerID = ab.Charger.Max(d => d.ID);
            long newchargerID = max_chargerID + 1;
            string totle = Request.Form["total"];
            string zhuanxiang = Request.Form["Selected1"];
            string data = Request.Form["submitDate"];
            string no = data.Replace("-", "")+"001";
            DateTime date = DateTime.Parse(data);
            if (ModelState.IsValid)
            {
                var list1 = ab.Charger.Where(a => a.chargeTime == date).OrderByDescending(a => a.seqNo);
                //var list1 = ab.Charger.Where(a => a.seqNo.CompareTo(no) >= 0).OrderByDescending(a => a.seqNo);
                            //where ad.seqNo.Substring(0,7)==no
                            //orderby ad.searchNo descending
                            //select ad;
                if (list1.Count() != 0)
                {
                    var list = list1.First();
                    long max_seqno = long.Parse(list.seqNo);
                    long seqno = max_seqno + 1;
                    string person = Request.Form["Selected"];
                    archivesContainer.submitPerson = ab.DepartmentCode.Where(a=>a.value==person).First().text;
                    archivesContainer.ID = newID;
                    archivesContainer.seqNo = seqno.ToString();
                    db.ArchivesContainer.Add(archivesContainer);//添加到ArchivesContainer
                    charger.ID = newchargerID;
                    charger.itemName = "档案盒及目录装具等费用";
                    charger.searchNo = archivesContainer.ID;
                    charger.totalExpense = decimal.Parse(totle);
                    charger.fromDepartment = "4";
                    charger.unitName = archivesContainer.DepartName;
                    charger.chargeClassify = 8;
                    charger.@operator = "办公室";
                    charger.chargeTime = archivesContainer.submitDate;
                    charger.seqNo = archivesContainer.seqNo;
                    charger.theoryExpense = decimal.Parse(totle);
                    charger.whereTransfer = int.Parse(zhuanxiang);
                    charger.remarks = Request.Form["remarks"];
                    charger.isCharge = false;
                    charger.isBack = false;
                    charger.chargeExtra = "";
                    charger.buildingArea = 0;
                    charger.chargeDetail = "";
                    charger.charger1 = "";
                    charger.remarks = "";
                    charger.backNote = "";
                    charger.centiCnt = 0;
                    ab.Charger.Add(charger);//添加到charger
                }
                else
                {
                    string person = Request.Form["Selected"];
                    archivesContainer.submitPerson =  ab.DepartmentCode.Where(a => a.value == person).First().text; ;
                    archivesContainer.ID = newID;
                    archivesContainer.seqNo = no;
                    db.ArchivesContainer.Add(archivesContainer);
                    charger.ID = newchargerID;
                    charger.itemName = "档案盒及目录装具等费用";
                    charger.searchNo = archivesContainer.ID;
                    charger.totalExpense = decimal.Parse(totle);
                    charger.fromDepartment = "4";
                    charger.unitName = archivesContainer.DepartName;
                    charger.chargeClassify = 8;
                    charger.@operator = "办公室";
                    charger.chargeTime = archivesContainer.submitDate;
                    charger.seqNo = archivesContainer.seqNo;
                    charger.theoryExpense = decimal.Parse(totle);
                    charger.whereTransfer = int.Parse(zhuanxiang);
                    charger.isCharge = false;
                    charger.isBack = false;
                    charger.chargeExtra = "";
                    charger.buildingArea = 0;
                    charger.chargeDetail = "";
                    charger.charger1 = "";
                    charger.remarks = "";
                    charger.backNote = "";
                    charger.centiCnt = 0;
                    ab.Charger.Add(charger);          
                }
                db.SaveChanges();
                ab.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(archivesContainer);
        }

        // GET: OfficeDanganZhuangju/Edit/5
        public ActionResult Edit(long? id)
        {
            ArchivesContainer archivesContainer = db.ArchivesContainer.Find(id);
            var charge = ab.Charger.Where(a => a.searchNo == id).First();
            if (charge.isCharge == true)
            {
                return Content("<script>alert('此费用已经收取，不能进行修改 !');window.history.back();</script>");
            }
            ViewBag.total = charge.totalExpense;
            ViewBag.Selected = new SelectList(ab.DepartmentCode, "value", "Text",charge.fromDepartment);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "转向财务科", Value = "1"},
                new SelectListItem { Text = "转向复印室", Value = "0"},
            };
            
            ViewBag.Selected1 = new SelectList(list1, "Value", "Text", charge.whereTransfer);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.seqNo = archivesContainer.seqNo;
            ViewBag.totle = charge.totalExpense;
            ViewBag.twoCentiCnt = archivesContainer.twoCentiCnt;
            ViewBag.thrCentiCnt = archivesContainer.thrCentiCnt;
            ViewBag.fourCentiCnt = archivesContainer.fourCentiCnt;
            ViewBag.fiveCentiCnt = archivesContainer.fiveCentiCnt;
            ViewBag.coverCnt = archivesContainer.coverCnt;
            ViewBag.catalogueCnt = archivesContainer.catalogueCnt;
            ViewBag.proformaCnt = archivesContainer.proformaCnt;
            if (archivesContainer == null)
            {
                return HttpNotFound();
            }
            return View(archivesContainer);
        }

        // POST: OfficeDanganZhuangju/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string action, [Bind(Include = "ID,twoCentiCnt,thrCentiCnt,fourCentiCnt,fiveCentiCnt,coverCnt,catalogueCnt,proformaCnt,DepartName,submitDate,submitPerson,archiveBoxFee,archiveCataLogFee,singleBoxFee,singleCatalogFee,seqNo")]ArchivesContainer archivesContainer,Charger charger)
        {
            long id = long.Parse(Request.Form["ID"]);
            var charge = from ad in ab.Charger
                         where ad.searchNo == id
                         select ad;
            var charge1 = charge.First();
            ViewBag.total = charge1.totalExpense;
            ViewBag.Selected = new SelectList(ab.DepartmentCode, "value", "Text", charge1.fromDepartment);
            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "转向财务科", Value = "1"},
                new SelectListItem { Text = "转向复印室", Value = "0"},
            };

            ViewBag.Selected1 = new SelectList(list2, "Value", "Text", charge1.whereTransfer);
            //Charger charger = new Charger();
            long max_chargerID = ab.Charger.Max(d => d.ID);
            long newchargerID = max_chargerID + 1;
            string totle = Request.Form["total"];
            string zhuanxiang = Request.Form["Selected1"];
            string data = Request.Form["submitDate"];
            string no = data.Replace("-", "");
            if (action == "生成最大收费编号")
            {
                if (data.Contains("-"))
                {
                    if (ModelState.IsValid)
                    {
                        var list1 = from ad in db.ArchivesContainer
                                    where ad.seqNo.Contains(no)
                                    orderby ad.ID descending
                                    select ad;
                        if (list1.Count() != 0)
                        {
                            var list = list1.First();
                            long max_seqno = long.Parse(list.seqNo);
                            long seqno = max_seqno + 1;
                            archivesContainer.seqNo = seqno.ToString();
                            ViewBag.seqNo = archivesContainer.seqNo;
                            ViewBag.totle = charge1.totalExpense;
                            ViewBag.twoCentiCnt = archivesContainer.twoCentiCnt;
                            ViewBag.thrCentiCnt = archivesContainer.thrCentiCnt;
                            ViewBag.fourCentiCnt = archivesContainer.fourCentiCnt;
                            ViewBag.fiveCentiCnt = archivesContainer.fiveCentiCnt;
                            ViewBag.coverCnt = archivesContainer.coverCnt;
                            ViewBag.catalogueCnt = archivesContainer.catalogueCnt;
                            ViewBag.proformaCnt = archivesContainer.proformaCnt;
                        }
                        else
                        {
                            archivesContainer.seqNo = no + "001";
                            ViewBag.seqNo = archivesContainer.seqNo;
                            ViewBag.totle = charge1.totalExpense;
                            ViewBag.twoCentiCnt = archivesContainer.twoCentiCnt;
                            ViewBag.thrCentiCnt = archivesContainer.thrCentiCnt;
                            ViewBag.fourCentiCnt = archivesContainer.fourCentiCnt;
                            ViewBag.fiveCentiCnt = archivesContainer.fiveCentiCnt;
                            ViewBag.coverCnt = archivesContainer.coverCnt;
                            ViewBag.catalogueCnt = archivesContainer.catalogueCnt;
                            ViewBag.proformaCnt = archivesContainer.proformaCnt;
                        }
                    }
                }
                else
                    Response.Write("<script >alert('请选择提交时间');window.history.back();</script >");
            }
            if (action == "保存")
            {
                if (ModelState.IsValid)
                {
                    string NO = Request.Form["seqNo"];
                    archivesContainer.seqNo = NO;
                    string person = Request.Form["Selected"];
                    var personname = ab.DepartmentCode.Where(a => a.value == person).First().text;
                    archivesContainer.submitPerson = personname;
                    db.Entry(archivesContainer).State = EntityState.Modified;
                    db.SaveChanges();
                    charge1.searchNo = archivesContainer.ID;
                    charge1.totalExpense = decimal.Parse(totle);
                    charge1.unitName = archivesContainer.DepartName;
                    charge1.chargeTime = archivesContainer.submitDate;
                    charge1.seqNo = archivesContainer.seqNo;
                    charge1.theoryExpense = decimal.Parse(totle);
                    charge1.whereTransfer = int.Parse(zhuanxiang);
                    ab.Entry(charge1).State = EntityState.Modified;
                    ab.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(archivesContainer);
        }

        // GET: OfficeDanganZhuangju/Delete/5
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ArchivesContainer archivesContainer = db.ArchivesContainer.Find(id);
        //    long NO = archivesContainer.ID;
        //    var charger = from ad in ab.Charger
        //                  where ad.searchNo == NO
        //                  select ad;
        //    var charger1 = charger.First();
        //    ViewBag.totle = charger1.totalExpense;
        //    ViewBag.trasfer = charger1.whereTransfer;
        //    if (archivesContainer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(archivesContainer);
        //}

        //// POST: OfficeDanganZhuangju/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteIndex(long id)
        {
            ArchivesContainer archivesContainer = db.ArchivesContainer.Find(id);
            long NO = archivesContainer.ID;
            var charger = from ad in ab.Charger
                          where ad.searchNo == NO
                          select ad;
            var charger1 = charger.First();
            if (charger1.isCharge == true)
            {
                return Content("<script>alert('此费用已经收取，不能进行删除 !');window.history.back();</script>");
            }
            db.ArchivesContainer.Remove(archivesContainer);
            ab.Charger.Remove(charger1);
            db.SaveChanges();
            ab.SaveChanges();
            return Content("<script>alert('已成功删除！');window.location.href='/OfficeDanganZhuangju/Index'</script>");
        }
        public ActionResult DeleteChaxun(long id)
        {
            ArchivesContainer archivesContainer = db.ArchivesContainer.Find(id);
            long NO = archivesContainer.ID;
            var charger = from ad in ab.Charger
                          where ad.searchNo == NO
                          select ad;
            var charger1 = charger.First();
            if (charger1.isCharge == true)
            {
                return Content("<script>alert('此费用已经收取，不能进行删除 !');window.history.back();</script>");
            }
            db.ArchivesContainer.Remove(archivesContainer);
            ab.Charger.Remove(charger1);
            db.SaveChanges();
            ab.SaveChanges();
            return Content("<script>alert('已成功删除！');window.location.href='/OfficeDanganZhuangju/FeiYongChaXun'</script>");
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
