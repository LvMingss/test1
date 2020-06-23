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
    public class gxCodeManagementController : Controller
    {
        private gxArchivesContainer bb = new gxArchivesContainer();
        private UrbanConEntities db = new UrbanConEntities();
        public ActionResult InformationWarehouse(string id,string action)
        {
            var cabinetinfo = from ad in bb.gxCabinetInfo
                                select ad;
            cabinetinfo = cabinetinfo.OrderBy(s => s.cabinetNo);// 默认按项目顺序号排列
           
            if(action =="添加新柜子")
            {
                return RedirectToAction("CreateNewChief");

            }
            //if(action=="返回")
            //{
            //    return RedirectToAction("ComplishProject", "LuruProject");
            //}
            ViewBag.result = JsonConvert.SerializeObject(cabinetinfo);

            return View();
        }
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cabinetinfo = bb.gxCabinetInfo.Find(id);
            if (cabinetinfo == null)
            {
                return HttpNotFound();
            }
           
            return View(cabinetinfo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cabinetNo,width,cengRangeA,cengRangeB,remainWidth,startPaijiaNo")]gxCabinetInfo cabinetinfo,string action)
        {
            if (action == "修改")
            {
                if (ModelState.IsValid)
                {
                    bb.Entry(cabinetinfo).State = EntityState.Modified;
                    bb.SaveChanges();
                    return Content("<script>alert('已成功保存修改！');window.location.href='/gxCodeManagement/InformationWarehouse'</script>");
                }
            }
            if (action == "删除")
            {
                if (ModelState.IsValid)
                {
                    bb.Entry(cabinetinfo).State = EntityState.Deleted;         
                    bb.SaveChanges();
                    return Content("<script>alert('已成功删除！');window.location.href='/gxCodeManagement/InformationWarehouse'</script>");
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
            var cabinet = from ad in bb.gxCabinetInfo
                          orderby ad.cabinetNo descending
                          select ad;
            gxCabinetInfo Cabinet = new gxCabinetInfo();
            string ID = cabinet.First().cabinetNo;
            long max = Convert.ToInt32(ID)+1;
             Cabinet.cabinetNo = max.ToString();
            return View(Cabinet);
        }
       [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewChief([Bind(Include = "cabinetNo,width,cengRangeA,cengRangeB,remainWidth,startPaijiaNo")]gxCabinetInfo cabinetinfo,string action)
        {
            gxCabinetInfo Cabinet = new gxCabinetInfo();
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
                    bb.gxCabinetInfo.Add(Cabinet);
                    bb.SaveChanges();
                    return Content("<script>alert('已成功添加该信息！');window.location.href='/gxCodeManagement/InformationWarehouse'</script>");
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
            var maxno = from ad in bb.gxMaxArchiveRegisNo
                              select ad;
            maxno = maxno.OrderBy(s => s.ID);
          
            if (action == "添加新类信息")
            {
                return RedirectToAction("CreateNewClass");

            }
            //if (action == "返回")
            //{
            //    return RedirectToAction("ComplishProject", "LuruProject");
            //}
            ViewBag.result = JsonConvert.SerializeObject(maxno);
            return View();
        }
        public ActionResult CreateNewClass(string  id)
        {
            var maxno = from ad in bb.gxMaxArchiveRegisNo
                        orderby ad.ID descending
                        select ad;
            gxMaxArchiveRegisNo Maxno = new gxMaxArchiveRegisNo();
            Maxno.ID = maxno.First().ID + 1;
            return View(Maxno);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewClass([Bind(Include = "ID,width,mainCategoryID,maxArchiveNo,maxRegisNo")]gxMaxArchiveRegisNo maxarchive, string action)
        {
            gxMaxArchiveRegisNo max = new gxMaxArchiveRegisNo();
            max.ID = maxarchive.ID;
            max.mainCategoryID = maxarchive.mainCategoryID;
            max.maxArchiveNo = maxarchive.maxArchiveNo;
            max.maxRegisNo = maxarchive.maxRegisNo;
           
            if (action == "添加")
            {
                if (ModelState.IsValid)
                {
                    bb.gxMaxArchiveRegisNo.Add(max);
                    bb.SaveChanges();
                    return Content("<script>alert('已成功保存修改！');window.location.href='/gxCodeManagement/maxArchiveRegisNo'</script>");
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

            var max = bb.gxMaxArchiveRegisNo.Find(id);
            if (max == null)
            {
                return HttpNotFound();
            }

            return View(max);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editmax ([Bind(Include = "ID,width,mainCategoryID,maxArchiveNo,maxRegisNo")]gxMaxArchiveRegisNo max, string action)
        {
            if (action == "修改")
            {
                if (ModelState.IsValid)
                {
                    bb.Entry(max).State = EntityState.Modified;
                    bb.SaveChanges();
                    return Content("<script>alert('已成功保存修改！');window.location.href='/gxCodeManagement/maxArchiveRegisNo'</script>");
                }
            }
            if (action == "删除")
            {
                if (ModelState.IsValid)
                {
                    bb.Entry(max).State = EntityState.Deleted;
                    bb.SaveChanges();
                    return Content("<script>alert('已成功删除该信息！');window.location.href='/gxCodeManagement/maxArchiveRegisNo'</script>");
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