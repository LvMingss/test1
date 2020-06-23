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
    public class ContractInfoesController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();

        // GET: ContractInfoes
        /*public ActionResult Index()
        {
            return View(db.ContractInfo.ToList());
        }*/
       public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            
            ViewData["pagename"] = "ContractInfoes-Index";
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "责任书编号", Value = "0"},
                new SelectListItem { Text = "工程名称", Value = "1" },
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
            var contractInfo = from ad in db.ContractInfo

                                 select ad;
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        contractInfo = contractInfo.Where(ad => ad.contractNo.Contains(searchString));//根据责任书编号搜索
                        break;
                    case 1:
                        contractInfo = contractInfo.Where(ad => ad.projectName.Contains(searchString));//根据工程名称搜索
                        break;
                    case 2:
                        contractInfo = contractInfo.Where(ad => ad.transferUnit.Contains(searchString));//根据建设单位搜索
                        break;
                }

            }
            // 默认按责任书编号排
            contractInfo = contractInfo.OrderByDescending(s => s.contractNo);
            ViewBag.result = JsonConvert.SerializeObject(contractInfo);
            return View();
        }

        // GET: ContractInfoes/Details/5
        public ActionResult Details(string id,string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractInfo contractInfo = db.ContractInfo.Find(id);
            if (contractInfo == null)
            {
                return HttpNotFound();
            }
            if(action=="返回")
            {
                return RedirectToAction("Index", "ContractInfoes");
            }
            return View(contractInfo);
        }

        // GET: ContractInfoes/Create
        public ActionResult Create()
        {            
            ViewData["pagename"] = "ContractInfoes-create";
            ContractInfo contractInfo = new ContractInfo();
            long max_contractInNo = Int64.Parse(db.ContractInfo.Max(d => d.contractNo));//设置一个默认值，用户也可修改，保证8位且不重复就行

            contractInfo.contractNo = (max_contractInNo + 1).ToString();//contractNo为责任书信息表的主键自动+1
            //设置一些默认值
            contractInfo.partAaddress = "青岛市市南区黄县路1号";
            contractInfo.partALegalRepresent = "业　务　科 82879324";
            contractInfo.partAcontactTel = "声　像　科 82882207";
            contractInfo.partAweituoAgent = "管理信息科 82860632";
            string plantStime = Convert.ToDateTime(DateTime.Today.Date).ToString("yyyy-MM-dd");
            string plantEtime = Convert.ToDateTime(DateTime.Today.Date).ToString("yyyy-MM-dd");
            string dataSign = Convert.ToDateTime(DateTime.Today.Date).ToString("yyyy-MM-dd");

            contractInfo.planningEndDate = DateTime.ParseExact(plantEtime.Trim(), "yyyy-MM-dd", null);
            contractInfo.planningStartDate = Convert.ToDateTime(plantStime);
            contractInfo.dateSigned = Convert.ToDateTime(dataSign);
            return View(contractInfo);
        }

        // POST: ContractInfoes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "contractNo,dateSigned,transferUnit,projectName,location,layerCount,buildingArea,planningStartDate,planningEndDate,partAaddress,partALegalRepresent, partAweituoAgent,partAcontactTel,partBadress,partBLegalRepresent,partBweituoAgent,partBcontactTel")] ContractInfo contractInfo,string contractNo)
        {

            var contract = from a in db.ContractInfo
                           where a.contractNo == contractNo
                           select a;
            if (contract.Count() != 0)
            {
                return Content("<script >alert('该责任书编号已存在，请重新输入！');window.location.href='Create';</script >");
            }
            if (contractInfo.projectName==""||contractInfo.projectName==null)
            {
                return Content("<script >alert('工程名称不能为空，请核查！');window.history.back();</script >");
            }
            if (ModelState.IsValid)
            {

                db.ContractInfo.Add(contractInfo);
                db.SaveChanges();

                return Content("<script >alert('保存成功！');window.location.href='Index';</script >");
            }
            return View(contractInfo);
        }

        // GET: ContractInfoes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractInfo contractInfo = db.ContractInfo.Find(id);
            if (contractInfo == null)
            {
                return HttpNotFound();
            }
            return View(contractInfo);
        }

        // POST: ContractInfoes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "contractNo,contractName,contractDesc,transferUnit,projectName,location,buildingArea,dateSigned,layerCount,planningStartDate,planningEndDate,scanningCopy,isReceiveArchive,textMaterial,drawing,partAaddress,partALegalRepresent,partAweituoAgent,partAcontactTel,partBadress,partBLegalRepresent,partBweituoAgent,partBcontactTel,constructUnit,constructArea")] ContractInfo contractInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contractInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contractInfo);
        }

        // GET: ContractInfoes/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractInfo contractInfo = db.ContractInfo.Find(id);
            if (contractInfo == null)
            {
                return HttpNotFound();
            }
            return View(contractInfo);
        }

        // POST: ContractInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ContractInfo contractInfo = db.ContractInfo.Find(id);
            db.ContractInfo.Remove(contractInfo);
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
