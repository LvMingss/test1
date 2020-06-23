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
using Newtonsoft.Json.Linq;

namespace urban_archive.Controllers
{
    public class FenLeiDangAnController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        public ActionResult Home()
        {
            return View();
        }
        // GET: FenLeiDangAn
        public ActionResult Index(string searchString,string Selected)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程地点（其他）", Value = "0"},
                new SelectListItem { Text = "分类号（其他）", Value = "1"},
                new SelectListItem { Text = "档号（其他）", Value = "2"},
                new SelectListItem { Text = "案卷题名（其他）", Value = "3"},
                new SelectListItem { Text = "编制单位（其他）", Value = "4"},
                new SelectListItem { Text = "编制时间（其他）", Value = "5"},
                new SelectListItem { Text = "进馆时间（其他）", Value = "6"},
                new SelectListItem { Text = "计量单位（其他）", Value = "7"},
                new SelectListItem { Text = "数量（其他）", Value = "8"},
                new SelectListItem { Text = "项目顺序号（其他）", Value = "9"},
                new SelectListItem { Text = "第一责任人（其他）", Value = "10"},
                new SelectListItem { Text = "其他责任人（其他）", Value = "11"},
                new SelectListItem { Text = "建设单位（其他）", Value = "12"},
                new SelectListItem { Text = "移交单位（其他）", Value = "13"},
                new SelectListItem { Text = "设计单位（其他）", Value = "14"},
                new SelectListItem { Text = "施工单位（其他）", Value = "15"},
                new SelectListItem { Text = "文字材料（其他）", Value = "16"},
                new SelectListItem { Text = "图纸页数（其他）", Value = "17"},
                new SelectListItem { Text = "发照时间（其他）", Value = "18"},
                new SelectListItem { Text = "图纸年代（其他）", Value = "19" },
                 new SelectListItem { Text = "总登记号（其他）", Value = "20"},
                new SelectListItem { Text = "排架号（其他）", Value = "21"},
                new SelectListItem { Text = "最新工程地点（其他）", Value = "22"}
            };
            if (Selected == "" | Selected == null) {
                ViewBag.Selected = new SelectList(list, "Value", "Text", 0);
            }
            else
            {
                ViewBag.Selected = new SelectList(list, "Value", "Text", Selected);
            }
            ViewBag.CurrentFilter = searchString;
            return View();
        }

        public string IndexData(string searchString, int? Selected, int? page)
        {
            var fenlei = db.OtherArchives.Where(f => f.classTypeID == 3);
            int t = Selected.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        fenlei = fenlei.Where(ad => ad.location.Contains(searchString));
                        break;
                    case 1:
                        fenlei = fenlei.Where(ad => ad.classNo.Contains(searchString));
                        break;
                    case 2:
                        fenlei = fenlei.Where(ad => ad.archiveNo.Contains(searchString));
                        break;
                    case 3:
                        fenlei = fenlei.Where(ad => ad.archiveTitle.Contains(searchString));
                        break;
                    case 4:
                        fenlei = fenlei.Where(ad => ad.bianzhiUnit.Contains(searchString));
                        break;
                    case 5:
                        fenlei = fenlei.Where(ad => ad.bianzhiTime.Contains(searchString));
                        break;
                    case 6:
                        fenlei = fenlei.Where(ad => ad.inHouseTime.ToString().Contains(searchString));
                        break;
                    case 7:
                        fenlei = fenlei.Where(ad => ad.measureUnit.Contains(searchString));
                        break;
                    case 8:
                        fenlei = fenlei.Where(ad => ad.count.ToString().Contains(searchString));
                        break;
                    case 9:
                        fenlei = fenlei.Where(ad => ad.proSeqNo.ToString().Contains(searchString));
                        break;
                    case 10:
                        fenlei = fenlei.Where(ad => ad.firstResponsible.Contains(searchString));
                        break;
                    case 11:
                        fenlei = fenlei.Where(ad => ad.otherResponsible.Contains(searchString));
                        break;
                    case 12:
                        fenlei = fenlei.Where(ad => ad.developUnit.Contains(searchString));
                        break;
                    case 13:
                        fenlei = fenlei.Where(ad => ad.tranferUnit.Contains(searchString));
                        break;
                    case 14:
                        fenlei = fenlei.Where(ad => ad.designUnit.Contains(searchString));
                        break;
                    case 15:
                        fenlei = fenlei.Where(ad => ad.constructionUnit.Contains(searchString));
                        break;
                    case 16:
                        fenlei = fenlei.Where(ad => ad.textMaterial.ToString().Contains(searchString));
                        break;
                    case 17:
                        fenlei = fenlei.Where(ad => ad.drawing.ToString().Contains(searchString));
                        break;
                    case 18:
                        fenlei = fenlei.Where(ad => ad.licenceTime.Contains(searchString));
                        break;
                    case 19:
                        fenlei = fenlei.Where(ad => ad.tuzhiniandai.Contains(searchString));
                        break;
                    case 20:
                        fenlei = fenlei.Where(ad => ad.registrationNo.Contains(searchString));
                        break;
                    case 21:
                        fenlei = fenlei.Where(ad => ad.paijiaNo.Contains(searchString));
                        break;
                    case 22:
                        fenlei = fenlei.Where(ad => ad.newLocation.Contains(searchString));
                        break;
                }
            }
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            int cnt = fenlei.Count() / pageSize + 1;
            if (fenlei.Count() % pageSize == 0)
            {
                cnt = fenlei.Count() / pageSize;
            }
            fenlei = fenlei.OrderBy(s => s.proSeqNo);
            //fenlei = fenlei.OrderBy(s => s.year).ThenBy(s=>s.volNo);
            var a = fenlei.ToPagedList(pageNumber, pageSize);
            var b = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                                        //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程
                                        from p in a
                                        select new JObject(
                                                 new JProperty("location", p.location),
                                                 new JProperty("classNo", p.classNo),
                                                 new JProperty("archiveNo", p.archiveNo),
                                                 new JProperty("archiveTitle", p.archiveTitle),
                                                 new JProperty("bianzhiUnit", p.bianzhiUnit),
                                                 new JProperty("bianzhiTime", p.bianzhiTime),
                                                 new JProperty("inHouseTime", p.inHouseTime),
                                                 new JProperty("measureUnit", p.measureUnit),
                                                 new JProperty("count", p.count),
                                                 new JProperty("proSeqNo", p.proSeqNo),
                                                 new JProperty("firstResponsible", p.firstResponsible),
                                                 new JProperty("otherResponsible", p.otherResponsible),
                                                 new JProperty("developUnit", p.developUnit),
                                                 new JProperty("tranferUnit", p.tranferUnit),
                                                 new JProperty("designUnit", p.designUnit),
                                                 new JProperty("constructionUnit", p.constructionUnit),
                                                 new JProperty("textMaterial", p.textMaterial),
                                                 new JProperty("drawing", p.drawing),
                                                 new JProperty("licenceTime", p.licenceTime),
                                                 new JProperty("tuzhiniandai", p.tuzhiniandai),
                                                 new JProperty("registrationNo", p.registrationNo),
                                                 new JProperty("paijiaNo", p.paijiaNo),
                                                 new JProperty("newLocation", p.newLocation),
                                                 new JProperty("ID", p.ID)

                                        )
                                )
                    )
).ToString();

            return b;
        }
        public ActionResult luru(string archiveNo, string id ,string id2)
        {
            if (archiveNo == "" || archiveNo == null)
            {
                return Content("<script >alert('该案卷档号为空，请核查！');window.history.back();</script >");
            }
            var file3 = from a in db.fenleiFile
                        where a.archivesNo == archiveNo
                        orderby a.seqNo
                        select a;

            ViewData["registion"] = archiveNo.First();
            ViewBag.result = JsonConvert.SerializeObject(file3.ToList());
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult luru(string archivesNo, string action)
        {
            var file = from a in db.FileInfo
                       where a.archivesNo == archivesNo.Trim()
                       orderby a.seqNo
                       select a;
            if (action == "添加")
            {
                return RedirectToAction("FileList", new { id1 = archivesNo.Trim(), id = 0, id2 = 1 });
            }

            fenleiFile file3 = new fenleiFile();

            //file3.id = db.fenleiFile.Max(a => a.id) + 1;
            file3.archivesNo = archivesNo;
            file3.seqNo = int.Parse(Request.Form["seqNo"]);
            file3.fileNo = Request.Form["fileNo"];
            file3.responsible = Request.Form["responsible"];
            file3.fileName = Request.Form["fileName"];
            file3.Date = Request.Form["Date"];
            file3.PageNo = Request.Form["PageNo"];
            file3.remarks = Request.Form["remarks"];

            if (action == "确定")
            {
                if (ModelState.IsValid)
                {
                    db.fenleiFile.Add(file3);
                    db.SaveChanges();
                    return Content("<script >alert('添加成功！');window.location.href='/FenLeiDangAn/luru?archiveNo=" + archivesNo + "';</script >");
                }
            }

            if (action == "取消")
            {
                return RedirectToAction("luru", new { id1 = archivesNo.Trim(), id = 0, id2 = 0 });
            }

            if (file == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        public ActionResult DetailsjuanNei(string id)
        {
            long fid = long.Parse(id);
            var file = from ad in db.fenleiFile
                       where ad.id == fid
                       select ad;
            fenleiFile fenleiFile = file.First();
            ViewData["fid"] = id;
            return View(fenleiFile);
        }
        [HttpPost]
        public ActionResult DetailsjuanNei(string id,string action)
        {
            long fid = long.Parse(id);
            var file = from ad in db.fenleiFile
                       where ad.id == fid
                       select ad;
            fenleiFile fenleiFile = file.First();
            string archiveNo = fenleiFile.archivesNo.Trim();
            if (action == "返回") {
                return RedirectToAction("luru", new { archiveNo = archiveNo, id = 0, id2 = 1 });
            }
            return RedirectToAction("luru", new { archiveNo = archiveNo, id = 0, id2 = 1 });
        }

        public ActionResult DeletejuanNei(string id)
        {
            long fid = long.Parse(id);
            var file = from ad in db.fenleiFile
                       where ad.id == fid
                       select ad;
            fenleiFile fenleiFile = file.First();
            string archivesNo = fenleiFile.archivesNo;
            db.Entry(fenleiFile).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("luru", new { archiveNo = archivesNo, id = 0, id2 = 1 });
        }

        public ActionResult EditjuanNei(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            long fid = long.Parse(id);
            var file = from ad in db.fenleiFile
                       where ad.id == fid
                       select ad;
            
            ViewData["fid"] = id;

            if (file == null)
            {
                return HttpNotFound();
            }

            return View(file.First());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditjuanNei(string id, string action, int seqNo, string type, string fileNo, string fileName, string responsible, string PageNo, string Date, string remarks)
        {
            long fid = long.Parse(id);
            var file = from ad in db.fenleiFile
                       where ad.id == fid
                       select ad;
            fenleiFile fenleiFile = file.First();
            if (action == "修改")
            {
                string a = fenleiFile.archivesNo;
                fenleiFile.archivesNo = a;
                fenleiFile.seqNo = seqNo;
                fenleiFile.fileNo = fileNo;
                fenleiFile.fileName = fileName;
                fenleiFile.responsible = responsible;
                fenleiFile.Date = Date;
                fenleiFile.PageNo = PageNo;
                fenleiFile.remarks = remarks;

                if (ModelState.IsValid)
                {
                    db.Entry(fenleiFile).State = EntityState.Modified;
                    db.SaveChanges();
                    //return Content("<script>alert('已成功修改！');window.location.href='/ArchivesEnter/FileList?fenleiFile.archivesNo=" + a + "';</script >");
                    return Content("<script >alert('已成功修改！');window.location.href='/FenLeiDangAn/luru?archiveNo=" + a + "';</script >");
                }
            }

            //if (action == "取消")
            //{
            //    return RedirectToAction("luru", new { id1 = archivesNo.Trim(), id = 0, id2 = 0 });
            //}

            if (file == null)
            {
                return HttpNotFound();
            }
            return View();
        }




        // GET: FenLeiDangAn/Details/5
        public ActionResult Details(long? id,string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OtherArchives otherArchives = db.OtherArchives.Find(id);
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "请选择案卷厚度", Value = "0"},
                new SelectListItem { Text = "1厘米", Value = "1"},
                new SelectListItem { Text = "2厘米", Value = "2"},
                new SelectListItem { Text = "3厘米", Value = "3"},
                new SelectListItem { Text = "4厘米", Value = "4"},
                new SelectListItem { Text = "5厘米", Value = "5"},
            };
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", otherArchives.ArchiveThick);

            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false"},
                new SelectListItem { Text = "是", Value = "true"},
            };
            ViewBag.isYD = new SelectList(list2, "Value", "Text",otherArchives.isYD);

            ViewBag.biaoyinPerson = new SelectList(ab.AspNetUsers, "UserName", "UserName", otherArchives.biaoyinPerson);
            ViewBag.shenhePerson = new SelectList(ab.AspNetUsers, "UserName", "UserName", otherArchives.shenhePerson);
            ViewBag.luruPerson = new SelectList(ab.AspNetUsers, "UserName", "UserName", otherArchives.luruPerson);
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", otherArchives.securityID);
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", otherArchives.retentionPeriodNo);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1"},
            };
            ViewBag.isJungongArch = new SelectList(list1, "Value", "Text", otherArchives.isJungongArch);
            if (otherArchives == null)
            {
                return HttpNotFound();
            }
            if (action == "返回")
            {
                
                return RedirectToAction("Index");
            }
            //if (action == "编辑")
            //{
            //    return RedirectToAction("Edit", new { id = id });
            //}
            return View(otherArchives);
        }

        // GET: FenLeiDangAn/Create
        public ActionResult Create()
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "请选择案卷厚度", Value = "0"},
                new SelectListItem { Text = "1厘米", Value = "1"},
                new SelectListItem { Text = "2厘米", Value = "2"},
                new SelectListItem { Text = "3厘米", Value = "3"},
                new SelectListItem { Text = "4厘米", Value = "4"},
                new SelectListItem { Text = "5厘米", Value = "5"},
            };
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text");
            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false"},
                new SelectListItem { Text = "是", Value = "true"},
            };
            ViewBag.isYD = new SelectList(list2, "Value", "Text");

            ViewBag.biaoyinPerson = new SelectList(ab.AspNetUsers, "UserName", "UserName");
            ViewBag.shenhePerson = new SelectList(ab.AspNetUsers, "UserName", "UserName");
            ViewBag.luruPerson = new SelectList(ab.AspNetUsers, "UserName", "UserName");
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName");
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName");
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1"},
            };
            ViewBag.isJungongArch = new SelectList(list1, "Value", "Text");
            ViewBag.ID = db.OtherArchives.Max(a => a.ID) + 1;
            return View();
        }

        // POST: FenLeiDangAn/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,registrationNo,classNo,archiveNo,paijiaNo,archiveTitle,bianzhiUnit,bianzhiTime,inHouseTime,measureUnit,count,securityID,retentionPeriodNo,licenceNo,location,classTypeID,ArchiveThick,proSeqNo,urbanID,firstResponsible,otherResponsible,developUnit,tranferUnit,designUnit,constructionUnit,textMaterial,drawing,licenceTime,kaigongTime,jungongTime,jianzhuArea,bilichi,biaoyinPerson,biaoyinTime,shenhePerson,shenheTime,luruPerson,luruTime,tuzhiniandai,tufu,tuzhiStatus,newLocation,cunfangLocation,neirongTiyao,isJungongArch,note,isYD,coordinate")] OtherArchives otherArchives)
        {
            
            if (ModelState.IsValid)
            {
                if (otherArchives.bianzhiTime == null)
                {
                    return Content("<script>alert('编制日期不能为空！');window.history.back();</script>");
                }
                if (otherArchives.inHouseTime == null)
                {
                    return Content("<script>alert('进馆日期不能为空！');window.history.back();</script>");
                }
                if (otherArchives.count.ToString() == "" || otherArchives.count == null)
                {
                    otherArchives.count = 0;
                }
                if (otherArchives.proSeqNo== null)
                {
                    otherArchives.proSeqNo = 0;
                }
                if (otherArchives.paijiaNo == null)
                {
                    otherArchives.paijiaNo = "";
                }
                if (otherArchives.registrationNo == null)
                {
                    otherArchives.registrationNo = "";
                }
                if (otherArchives.textMaterial == null)
                {
                    otherArchives.textMaterial = 0;
                }
                if (otherArchives.drawing == null)
                {
                    otherArchives.drawing = 0;
                }
                if (otherArchives.jianzhuArea == null)
                {
                    otherArchives.jianzhuArea =0;
                }
                otherArchives.doorplate = "";
                otherArchives.landNo = "";
                otherArchives.projectRange = "";
                otherArchives.year = "";
                otherArchives.volNo = "";
                otherArchives.applyUnit = "";
                otherArchives.note = "";
                otherArchives.status = "";
                otherArchives.isNeibu = "0";
                db.OtherArchives.Add(otherArchives);
                db.SaveChanges();
                return Content("<script>alert('已成功保存！');window.location.href='./Index'</script>");
            }
            return View(otherArchives);
        }

        // GET: FenLeiDangAn/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OtherArchives otherArchives = db.OtherArchives.Find(id);
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "请选择案卷厚度", Value = "0"},
                new SelectListItem { Text = "1厘米", Value = "1"},
                new SelectListItem { Text = "2厘米", Value = "2"},
                new SelectListItem { Text = "3厘米", Value = "3"},
                new SelectListItem { Text = "4厘米", Value = "4"},
                new SelectListItem { Text = "5厘米", Value = "5"},
            };
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text",otherArchives.ArchiveThick);

            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false"},
                new SelectListItem { Text = "是", Value = "true"},
            };
            ViewBag.isYD = new SelectList(list2, "Value", "Text",otherArchives.isYD);

            ViewBag.biaoyinPerson = new SelectList(ab.AspNetUsers, "UserName", "UserName",otherArchives.biaoyinPerson);
            ViewBag.shenhePerson = new SelectList(ab.AspNetUsers, "UserName", "UserName", otherArchives.shenhePerson);
            ViewBag.luruPerson = new SelectList(ab.AspNetUsers, "UserName", "UserName", otherArchives.luruPerson);
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", otherArchives.securityID);
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", otherArchives.retentionPeriodNo);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1"},
            };
            ViewBag.isJungongArch = new SelectList(list1, "Value", "Text",otherArchives.isJungongArch);
            if (otherArchives == null)
            {
                return HttpNotFound();
            }
            return View(otherArchives);
        }

        // POST: FenLeiDangAn/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,registrationNo,classNo,archiveNo,paijiaNo,archiveTitle,bianzhiUnit,bianzhiTime,inHouseTime,measureUnit,count,securityID,retentionPeriodNo,licenceNo,location,classTypeID,ArchiveThick,proSeqNo,urbanID,firstResponsible,otherResponsible,developUnit,tranferUnit,designUnit,constructionUnit,textMaterial,drawing,licenceTime,kaigongTime,jungongTime,jianzhuArea,bilichi,biaoyinPerson,biaoyinTime,shenhePerson,shenheTime,luruPerson,luruTime,tuzhiniandai,tufu,tuzhiStatus,newLocation,cunfangLocation,neirongTiyao,isJungongArch,note,isYD,coordinate")] OtherArchives otherArchives, string action)
        {
            if (action == "保存修改")
            {
                //if (ModelState.IsValid)
                //{
                    if (otherArchives.bianzhiTime == null)
                    {
                        return Content("<script>alert('编制日期不能为空！');window.history.back();</script>");
                    }
                    if (otherArchives.inHouseTime == null)
                    {
                        return Content("<script>alert('尽管日期不能为空！');window.history.back();</script>");
                    }
                    if (otherArchives.count.ToString() == "" || otherArchives.count == null)
                    {
                        otherArchives.count = 0;
                    }
                    if (otherArchives.proSeqNo == null)
                    {
                        otherArchives.proSeqNo = 0;
                    }
                    if (otherArchives.paijiaNo == null)
                    {
                        otherArchives.paijiaNo = "";
                    }
                    if (otherArchives.registrationNo == null)
                    {
                        otherArchives.registrationNo = "";
                    }
                    if (otherArchives.textMaterial == null)
                    {
                        otherArchives.textMaterial = 0;
                    }
                    if (otherArchives.drawing == null)
                    {
                        otherArchives.drawing = 0;
                    }
                    if (otherArchives.jianzhuArea == null)
                    {
                        otherArchives.jianzhuArea = 0;
                    }
                    otherArchives.doorplate = "";
                    otherArchives.landNo = "";
                    otherArchives.projectRange = "";
                    otherArchives.year = "";
                    otherArchives.volNo = "";
                    otherArchives.applyUnit = "";
                    otherArchives.note = "";
                    otherArchives.status = "";
                    otherArchives.isNeibu = "0";
                    db.Entry(otherArchives).State = EntityState.Modified;
                    db.SaveChanges();
                    //return Content("<script>alert('已修改成功！');window.location.href='../Index'</script>");
                    return Content("<script>alert('已修改成功！');history.go(-1);</script>");
                //}
                //else {
                //    return Content("<script>alert('修改失败！');history.go(-1);</script>");
                //}
            }
            if (action == "返回")
            {
                return RedirectToAction("Index");
            }
            return View(otherArchives);
        }

        //// GET: FenLeiDangAn/Delete/5
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    OtherArchives otherArchives = db.OtherArchives.Find(id);
        //    if (otherArchives == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(otherArchives);
        //}

        //// POST: FenLeiDangAn/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            OtherArchives otherArchives = db.OtherArchives.Find(id);
            db.OtherArchives.Remove(otherArchives);
            db.SaveChanges();
            //return RedirectToAction("Index");
            return Content("<script>alert('删除成功！');history.go(-1);</script>");
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
