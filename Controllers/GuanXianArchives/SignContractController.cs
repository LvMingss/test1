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
    public class SignContractController : Controller
    {
        private gxArchivesContainer db = new gxArchivesContainer();
        private UrbanConEntities ab = new UrbanConEntities();

        // GET: ContractInfoes
        /*public ActionResult Index()
        {
            return View(db.ContractInfo.ToList());
        }*/
        public ViewResult Index( string searchString, int? SelectedID)
        {

            ViewData["pagename"] = "ContractInfoes-Index";
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "责任书编号", Value = "0"},
                new SelectListItem { Text = "工程名称", Value = "1" },
                new SelectListItem { Text = "总登记号", Value = "2" }
            };
            string n = Request.Form["SelectedID"];
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            var contractInfo = from ad in db.gxContractInfo
                               select ad;
            int t = SelectedID.GetValueOrDefault();

            if (!String.IsNullOrEmpty(searchString))
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", n);
                //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
                ViewBag.search = searchString;
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
            

            contractInfo = contractInfo.OrderByDescending(s => s.contractNo);

            ViewBag.result1 = JsonConvert.SerializeObject(contractInfo);
            return View();
        }
        public string Find(int? SelectedID, string SearchString)
        {
            var vwprojectFile = from ad in db.vw_gxprojectList
                                select ad;
            int t = SelectedID.GetValueOrDefault();



            var contractInfo = from ad in db.gxContractInfo

                               select ad;
            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (t)
                {
                    case 0:
                        contractInfo = contractInfo.Where(ad => ad.contractNo.Contains(SearchString));//根据责任书编号搜索
                        break;
                    case 1:
                        contractInfo = contractInfo.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 2:
                        contractInfo = contractInfo.Where(ad => ad.transferUnit.Contains(SearchString));//根据建设单位搜索
                        break;
                }

            }
            contractInfo = contractInfo.OrderByDescending(s => s.contractNo);
            return JsonConvert.SerializeObject(contractInfo);
        }
        // GET: ContractInfoes/Details/5
        public ActionResult Details(string id, string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            gxContractInfo contractInfo = db.gxContractInfo.Find(id);
            if (contractInfo == null)
            {
                return HttpNotFound();
            }
            if (action == "返回")
            {
                return RedirectToAction("Index", "Signcontract");
            }
            return View(contractInfo);
        }

        // GET: ContractInfoes/Create
        public ActionResult Create()
        {
            ViewBag.classifyID = new SelectList(db.gxClassType, "classTypeID", "classTypeName");//管线分类
            ViewData["pagename"] = "ContractInfoes-create";
            gxContractInfo contractInfo = new gxContractInfo();
            string year = DateTime.Now.Year.ToString();
            var no1 = ab.ContractInfo.Max(a => a.contractNo);
            var no2 = db.gxContractInfo.Where(a => a.CLASSID.Trim() == "1").Max(a => a.contractNo);
            var n = no1.CompareTo(no2);
            var no = "";
            if (n >= 0)
            {
                no = no1;
            }
            if (n < 0)
            {
                no = no2;
            }
            string noyear = no.ToString().Substring(0, 4);
            string maxno = "";
            if (year == noyear)
            {
                maxno = (long.Parse(no) + 1).ToString();//contractNo为责任书信息表的主键自动+1
            }
            else
            {
                maxno = year + "0001";
            }
            contractInfo.contractNo = maxno;
            contractInfo.partAaddress = "青岛市市南区黄县路1号";
            contractInfo.partALegalRepresent = "业　务　科 82879324";
            contractInfo.partAcontactTel = "声　像　科 82882207";
            contractInfo.partAweituoAgent = "管理信息科 82860632";
            contractInfo.partAguanxianTel = "管　线　科 82873118";
            string plantStime = Convert.ToDateTime(DateTime.Today.Date).ToString("yyyy-MM-dd");
            string plantEtime = Convert.ToDateTime(DateTime.Today.Date).ToString("yyyy-MM-dd");
            string dataSign = Convert.ToDateTime(DateTime.Today.Date).ToString("yyyy-MM-dd");

            contractInfo.planningEndDate = DateTime.ParseExact(plantEtime.Trim(), "yyyy-MM-dd", null);
            contractInfo.planningStartDate = Convert.ToDateTime(plantStime);
            contractInfo.dateSigned = Convert.ToDateTime(dataSign);
            return View(contractInfo);
        }
        public string getno()
        {
            var no1 = ab.ContractInfo.Max(a => a.contractNo);
            var no2 = db.gxContractInfo.Where(a => a.CLASSID.Trim() == "1").Max(a => a.contractNo);
            var n = no1.CompareTo(no2);
            var no = "";
            if (n>=0)
            {
                no = no1;
            }
            if (n < 0)
            {
                no = no2;
            }
            string noyear = no.ToString().Substring(0, 4);
            string maxno = "";
            string year = DateTime.Now.Year.ToString();
            if (year == noyear)
            {
                maxno = (long.Parse(no) + 1).ToString();//contractNo为责任书信息表的主键自动+1
            }
            else
            {
                maxno = year + "0001";
            }
            //string maxno = (int.Parse(no) + 1).ToString();
            return maxno;

        }
        // POST: ContractInfoes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string contractNo,string constructUnit, string classid, string dateSigned, string transferUnit, string projectName, string location, string layerCount, string buildingArea, string planningStartDate, string planningEndDate, string partAaddress, string partALegalRepresent, string partAweituoAgent, string partAcontactTel, string partBadress, string partBLegalRepresent, string partBweituoAgent, string partBcontactTel,string guanjing,string Material)
        {

            var contract = from a in db.gxContractInfo
                           where a.contractNo == contractNo
                           select a;
            //if (contract.Count() != 0)
            //{
            //    return Content("<script >alert('该责任书编号已存在，请重新输入！');window.location.href='Create';</script >");
            //}
            if (projectName == "" || projectName == null)
            {
                return Content("<script >alert('工程名称不能为空，请核查！');window.history.back();</script >");
            }
            if (ModelState.IsValid)
            {
                gxContractInfo contractInfo = new gxContractInfo();
                contractInfo.contractNo = contractNo;
                if(dateSigned!=null&& dateSigned!="")
                {
                    contractInfo.dateSigned =DateTime.Parse(dateSigned.Trim());
                }
                contractInfo.constructUnit = constructUnit;
                contractInfo.CLASSID = classid;
                contractInfo.transferUnit = transferUnit;
                contractInfo.projectName = projectName;
                contractInfo.location = location;
                contractInfo.layerCount = layerCount;
                contractInfo.buildingArea = buildingArea;
                contractInfo.guanjing = guanjing;
                contractInfo.Material = Material;
                if(planningStartDate!=null&& planningStartDate!="")
                {
                    contractInfo.planningStartDate = DateTime.Parse(planningStartDate.Trim()); 
                }
                if (planningEndDate!=null && planningEndDate!="")
                {
                    contractInfo.planningEndDate = DateTime.Parse(planningEndDate.Trim());
                }
                contractInfo.partAaddress = partAaddress;
                contractInfo.partALegalRepresent = partALegalRepresent;

                contractInfo.partAweituoAgent = partAweituoAgent;
                contractInfo.partAcontactTel = partAcontactTel;
                contractInfo.partBadress = partBadress;
                contractInfo.partBcontactTel = partBcontactTel;
                contractInfo.partBLegalRepresent = partBLegalRepresent;
                contractInfo.partBweituoAgent = partBweituoAgent;
                db.gxContractInfo.Add(contractInfo);
                db.SaveChanges();

                return Content("<script >alert('保存成功！');window.location.href='Index';</script >");
            }
            return View();
        }

        // GET: ContractInfoes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            gxContractInfo contractInfo = db.gxContractInfo.Find(id);
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
            gxContractInfo contractInfo = db.gxContractInfo.Find(id);
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
            gxContractInfo contractInfo = db.gxContractInfo.Find(id);
            db.gxContractInfo.Remove(contractInfo);
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

