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
    public class CodeManagementController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        public ActionResult InformationWarehouse(string id,string action)
        {
            ViewData["pagename"] = "CodeManagement/InformationWarehouse";
            var cabinetinfo = from ad in db.CabinetInfo
                                select ad;
            cabinetinfo = cabinetinfo.OrderBy(s => s.cabinetNo);// 默认按项目顺序号排列
           
            if(action =="添加新柜子")
            {
                return RedirectToAction("CreateNewChief");

            }
            if(action=="返回")
            {
                return RedirectToAction("ComplishProject", "ProjectInfoes");
            }
            //return View(cabinetinfo);
            ViewBag.result = JsonConvert.SerializeObject(cabinetinfo);
            return View();
        }
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cabinetinfo = db.CabinetInfo.Find(id);
            if (cabinetinfo == null)
            {
                return HttpNotFound();
            }
           
            return View(cabinetinfo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cabinetNo,width,cengRangeA,cengRangeB,remainWidth,startPaijiaNo")]CabinetInfo cabinetinfo,string action)
        {
            if (action == "修改")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cabinetinfo).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("InformationWarehouse");
                }
            }
            if (action == "删除")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cabinetinfo).State = EntityState.Deleted;         

                    db.SaveChanges();
                    return RedirectToAction("InformationWarehouse");
                }
            }
            if (action == "返回")
            {
                return RedirectToAction("InformationWarehouse");
            }
          
            
            return View(cabinetinfo);
        }
        public ActionResult CreateNewChief(string id)
        {
            var cabinet = from ad in db.CabinetInfo
                          orderby ad.cabinetNo descending
                          select ad;
            CabinetInfo Cabinet = new CabinetInfo();
            string ID = cabinet.First().cabinetNo;
            long max = Convert.ToInt32(ID)+1;
             Cabinet.cabinetNo = max.ToString();
            return View(Cabinet);
        }
       [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewChief([Bind(Include = "cabinetNo,width,cengRangeA,cengRangeB,remainWidth,startPaijiaNo")]CabinetInfo cabinetinfo,string action)
        {
            CabinetInfo Cabinet = new CabinetInfo();
            Cabinet.cabinetNo = cabinetinfo.cabinetNo;
            Cabinet.width = cabinetinfo.width;
            Cabinet.cengRangeA = cabinetinfo.cengRangeA;
            Cabinet.cengRangeB = cabinetinfo.cengRangeB;
            Cabinet.remainWidth = cabinetinfo.remainWidth;
            Cabinet.startPaijiaNo = cabinetinfo.startPaijiaNo;
            if (action == "添加")
            {
                if (ModelState.IsValid)
                {
                    db.CabinetInfo.Add(Cabinet);
                    db.SaveChanges();
                    return RedirectToAction("InformationWarehouse");
                }
            }
            if (action == "返回")
            {
                return RedirectToAction("InformationWarehouse");
            }


            return View(Cabinet);

        }
        public ActionResult maxArchiveRegisNo(int? page, string action)
        {
            ViewData["pagename"] = "CodeManagement/MaxArchiveRegisNo";
            var maxno = from ad in db.MaxArchiveRegisNo
                              select ad;
            maxno = maxno.OrderBy(s => s.ID);
          
            if (action == "添加新类信息")
            {
                return RedirectToAction("CreateNewClass");

            }
            if (action == "返回")
            {
                return RedirectToAction("ComplishProject", "ProjectInfoes");
            }
            //return View(maxno);
            ViewBag.result = JsonConvert.SerializeObject(maxno);
            return View();
        }
        public ActionResult CreateNewClass(string  id)
        {
            var maxno = from ad in db.MaxArchiveRegisNo
                        orderby ad.ID descending
                        select ad;
            MaxArchiveRegisNo Maxno = new MaxArchiveRegisNo();
            Maxno.ID = maxno.First().ID + 1;
            return View(Maxno);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewClass([Bind(Include = "ID,width,mainCategoryID,maxArchiveNo,maxRegisNo")]MaxArchiveRegisNo maxarchive, string action)
        {
            MaxArchiveRegisNo max = new MaxArchiveRegisNo();
            max.ID = maxarchive.ID;
            max.mainCategoryID = maxarchive.mainCategoryID;
            max.maxArchiveNo = maxarchive.maxArchiveNo;
            max.maxRegisNo = maxarchive.maxRegisNo;
           
            if (action == "添加")
            {
                if (ModelState.IsValid)
                {
                    db.MaxArchiveRegisNo.Add(max);
                    db.SaveChanges();
                    return RedirectToAction("maxArchiveRegisNo");
                }
            }
            if (action == "返回")
            {
                return RedirectToAction("maxArchiveRegisNo");
            }


            return View(max);

        }
        public ActionResult Editmax(long ? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var max = db.MaxArchiveRegisNo.Find(id);
            if (max == null)
            {
                return HttpNotFound();
            }

            return View(max);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editmax ([Bind(Include = "ID,width,mainCategoryID,maxArchiveNo,maxRegisNo")]MaxArchiveRegisNo max, string action)
        {
            if (action == "修改")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(max).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("<script>alert('已成功保存修改！');window.location.href='/CodeManagement/maxArchiveRegisNo'</script>");
                }
            }
            if (action == "删除")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(max).State = EntityState.Deleted;

                    db.SaveChanges();
                    return Content("<script>alert('已成功删除该信息！');window.location.href='/CodeManagement/maxArchiveRegisNo'</script>");
                }
            }
            if (action == "返回")
            {
                return RedirectToAction("maxArchiveRegisNo");
            }


            return View(max);
        }
    }
}