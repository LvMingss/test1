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
using Microsoft.Reporting.WebForms;
namespace urban_archive.Controllers
{
    public class VideoContractInfoesController : Controller
    {
        private VideoArchiveEntities db = new VideoArchiveEntities();

        // GET: VideoContractInfoes
        /*public ActionResult Index()
        {
            return View(db.VideoContractInfo.ToList());
        }*/

        public ActionResult Home()
        {
            return View();
        }
        public ActionResult TongJi( )
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = "4"},
                new SelectListItem { Text = "签订日期", Value = "0"},
                new SelectListItem { Text = "计划开工日期", Value = "1"},
                new SelectListItem { Text = "计划竣工日期", Value = "2" }             
            };

             ViewBag.SelectedID = new SelectList(list, "Value", "Text", 4);

            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = "4"},
                new SelectListItem { Text = "工程地点", Value = "0"},
                new SelectListItem { Text = "工程名称", Value = "1"},
                new SelectListItem { Text = "建设单位", Value = "2" }
            };

            ViewBag.SelectedID1 = new SelectList(list1, "Value", "Text",4);
     
            //if (type != "" && type != null)//用户在检索框中输入了检索条件
            //{
            //    int t = Int32.Parse(type.Trim());
            //    if (!String.IsNullOrEmpty(type))
            //    {

            //        switch (t)
            //        {
            //            case 0:

            //                break;
            //            case 1:

            //                vwprojectFile = vwprojectFile.Where(ad => ad.status == "3");
            //                break;
            //            case 2:
            //                vwprojectFile = vwprojectFile.Where(ad => ad.status == "4");
            //                break;
            //            case 3:
            //                vwprojectFile = vwprojectFile.Where(ad => ad.status == "5");
            //                break;
            //            case 4:

            //                vwprojectFile = vwprojectFile.Where(ad => ad.status == "6");
            //                break;
            //            case 5:

            //                vwprojectFile = vwprojectFile.Where(ad => ad.status == "10");
            //                break;



            //        }
            //    }

            //}
            //if (startdate != "" && startdate != null && enddate != "" && enddate != null)//增加查询条件
            //{
            //    DateTime start = DateTime.Parse(startdate.Trim());
            //    DateTime end = DateTime.Parse(enddate.Trim());
            //    vwprojectFile = from h in vwprojectFile
            //                    where (h.dateReceived >= start) && (h.dateReceived <= end)
            //                    select h;
            //}
            //ViewData["start"] = startdate;
            //ViewData["end"] = enddate;
            //ViewBag.count = vwprojectFile.Count();
            //return View();
            return View();
        }
        [HttpPost]
      
        public ActionResult TongJi(string action,string SelectedID1,string SelectedID,string Searching,string startdate,string enddate)
        {
            var ds = from a in db.VideoContractInfo
                     select a;
            if (action == "打印全部移交责任书统计")
            {
                LocalReport localReport = new LocalReport();
                //string Person = Request.Form["collator"];
                //DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                //DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                
                var ds1 = ds;
                var total = ds1.Count();
                localReport.ReportPath = Server.MapPath("~/Report/video.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("SXTJ", ds1);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("total", total.ToString().Trim()));
                localReport.SetParameters(parameterList);
                string reportType = "pdf";
                string mimeType;
                string encoding;
                string fileNameExtension;
                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutPutFormat>" + "pdf" + "</OutPutFormat>" +
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
            if (action == "打印移交责任书统计")
            {
                if (Searching != "" && Searching != null)//用户在检索框中输入了检索条件
                {
                    int t = Int32.Parse(SelectedID1.Trim());
                   

                    switch (t)
                        {
                            
                            case 0:

                               ds = ds.Where(ad => ad.location.Contains(Searching));
                                break;
                            case 1:
                               ds = ds.Where(ad => ad.projectName.Contains(Searching));
                            break;
                            case 2:
                               ds = ds.Where(ad => ad.transferUnit.Contains(Searching));
                            break;
                            case 4:
                                break;
                          }

                }
                if (startdate != "" && startdate != null&& enddate != "" && enddate != null)//用户在检索框中输入了检索条件
                {
                    int t1 = Int32.Parse(SelectedID.Trim());

                    DateTime s = DateTime.Parse(startdate.Trim());
                    DateTime e = DateTime.Parse(enddate.Trim());
                    switch (t1)
                    {

                        case 0:
                            
                            ds = ds.Where(ad => ad.dateSigned>= s).Where(ad => ad.dateSigned <= e);
                            break;
                        case 1:
                            ds = ds.Where(ad => ad.planningStartDate >= s).Where(ad => ad.planningStartDate <= e);
                            break;
                        case 2:
                            ds = ds.Where(ad => ad.planningEndDate>= s).Where(ad => ad.planningEndDate <= e);
                            break;
                        case 4:
                            break;
                    }

                }
                LocalReport localReport = new LocalReport();
                //string year = Request.Form["year"];
                //if (year != "")
                //{
                //    DateTime yearS = DateTime.Parse(year+".1.1");
                //    DateTime yearE= DateTime.Parse(year + ".12.31");
                //    ds = ds.Where(a => a.dateSigned>= yearS).Where(a=>a.dateSigned<=yearE).ToList();
                //}
                //string month = Request.Form["month"];
                //if (month != "")
                //{
                //    string m=year+"-"+month.PadLeft(2, '0');
                //    DateTime monthS = DateTime.Parse(m + ".1");
                //    DateTime monthE = DateTime.Parse(m + ".31");
                //    ds = ds.Where(a => a.dateSigned >= monthS).Where(a => a.dateSigned <= monthE).ToList();
                //}
                //string date = Request.Form["date"];
                //if (date!= "")
                //{
                //    DateTime date1 = DateTime.Parse(date);
                //    ds = ds.Where(a=>a.dateSigned==date1).ToList();
                //}
                //string dateS = Request.Form["dateS"];
                //if (dateS != "")
                //{
                //    DateTime dateS1 = DateTime.Parse(dateS);
                //    ds = ds.Where(a => a.planningStartDate == dateS1).ToList();
                //}
                //string dateE = Request.Form["dateE"];
                //if (dateE != "")
                //{
                //    DateTime dateE1 = DateTime.Parse(dateE);
                //    ds = ds.Where(a => a.planningEndDate == dateE1).ToList();
                //}
                //string place = Request.Form["place"];
                //if (place != "")
                //{
                //    ds = ds.Where(a => a.location.Contains(place)).ToList();
                //}
                var ds1 = ds;
                var total = ds1.Count();
                localReport.ReportPath = Server.MapPath("~/Report/video.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("SXTJ", ds1);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("total", total.ToString().Trim()));
                localReport.SetParameters(parameterList);
                string reportType = "pdf";
                string mimeType;
                string encoding;
                string fileNameExtension;
                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutPutFormat>" + "pdf" + "</OutPutFormat>" +
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
            return View();
        }
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "责任书编号", Value = "0"},
                new SelectListItem { Text = "工程名称", Value = "1" },
                new SelectListItem { Text = "建设单位", Value = "2" },
                new SelectListItem { Text = "工程地点", Value = "3" }
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
            var contractInfo = from ad in db.VideoContractInfo
                               select ad;
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        contractInfo = contractInfo.Where(ad => ad.contractNo.Contains(searchString) );//根据责任书编号搜索
                        break;
                    case 1:
                        contractInfo = contractInfo.Where(ad => ad.projectName.Contains(searchString));//根据工程名称搜索
                        break;
                    case 2:
                        contractInfo = contractInfo.Where(ad => ad.transferUnit.Contains(searchString));//根据建设单位搜索
                        break;
                    case 3:
                        contractInfo = contractInfo.Where(ad => ad.location.Contains(searchString));//根据工程地点搜索
                        break;
                }

            }
            // 默认按责任书编号排
            contractInfo = contractInfo.OrderByDescending(s => s.contractNo);
            ViewBag.result = JsonConvert.SerializeObject(contractInfo);
            
            return View();
        }

        // GET: VideoContractInfoes/Details/5
        public ActionResult Details(string id, string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoContractInfo videoContractInfo = db.VideoContractInfo.Find(id);
            if (videoContractInfo == null)
            {
                return HttpNotFound();
            }
            if (action == "返回")
            {
                return RedirectToAction("Index", "VideoContractInfoes");
            }
            return View(videoContractInfo);
        }

        // GET: VideoContractInfoes/Create
        public ActionResult Create()
        {
            VideoContractInfo contractInfo = new VideoContractInfo();
            long max_contractInNo = Int64.Parse(db.VideoContractInfo.Max(d => d.contractNo));//设置一个默认值，用户也可修改，保证8位且不重复就行

            contractInfo.contractNo = (max_contractInNo + 1).ToString();//contractNo为责任书信息表的主键自动+1
            //设置一些默认值
            contractInfo.partAaddress = "青岛市市南区黄县路1号";
            contractInfo.partALegalRepresent = "业　务　科 82879324";
            contractInfo.partAcontactTel = "声　像　科 82882207";
            contractInfo.partAweituoAgent = "管理信息科 82860632";
            string plantStime = Convert.ToDateTime(DateTime.Today.Date).ToString("yyyy-MM-dd");
            string plantEtime = Convert.ToDateTime(DateTime.Today.Date).ToString("yyyy-MM-dd");
            string dataSign = Convert.ToDateTime(DateTime.Today.Date).ToString("yyyy-MM-dd");

            //工程地点分区
            List<SelectListItem> list1 = new List<SelectListItem> {
                 new SelectListItem { Text = "", Value = ""},
                new SelectListItem { Text = "市南区", Value = "市南区"},
                new SelectListItem { Text = "市北区", Value = "市北区" },
                new SelectListItem { Text = "李沧区", Value = "李沧区" }
            };
            ViewBag.constructArea1 = new SelectList(list1, "Value", "Text");

            contractInfo.planningEndDate = DateTime.ParseExact(plantEtime.Trim(), "yyyy-MM-dd", null);
            contractInfo.planningStartDate = Convert.ToDateTime(plantStime);
            contractInfo.dateSigned = Convert.ToDateTime(dataSign);
            return View(contractInfo);
        }

        // POST: VideoContractInfoes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "contractNo,dateSigned,transferUnit,projectName,location,layerCount,buildingArea,planningStartDate,planningEndDate,partAaddress,partALegalRepresent, partAweituoAgent,partAcontactTel,partBadress,partBLegalRepresent,partBweituoAgent,partBcontactTel")] VideoContractInfo contractInfo, string contractNo, string constructArea)
        {

            var contract = from a in db.VideoContractInfo
                           where a.contractNo == contractNo
                           select a;
            if (contract.Count() != 0)
            {
                return Content("<script >alert('该责任书编号已存在，请重新输入！');window.location.href='Create';</script >");
            }
            if (contractInfo.projectName == "" || contractInfo.projectName == null)
            {
                return Content("<script >alert('工程名称不能为空，请核查！');window.history.back();</script >");
            }
            if (contractInfo.planningStartDate == null)
            {
                return Content("<script >alert('计划开工时间不能为空，请核查！');window.history.back();</script >");
            }
            if (contractInfo.planningEndDate == null)
            {
                return Content("<script >alert('计划竣工时间不能为空，请核查！');window.history.back();</script >");
            }
            if (contractInfo.dateSigned == null)
            {
                return Content("<script >alert('签订日期不能为空，请核查！');window.history.back();</script >");
            }

            if (ModelState.IsValid)
            {
                contractInfo.constructArea = constructArea;
                db.VideoContractInfo.Add(contractInfo);
                db.SaveChanges();

                return Content("<script >alert('保存成功！');window.location.href='Index';</script >");
            }
            return View(contractInfo);
        }

        // GET: VideoContractInfoes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoContractInfo videoContractInfo = db.VideoContractInfo.Find(id);
            //DateTime plantStime = DateTime.ParseExact(videoContractInfo.planningStartDate.ToString(), "yyyy-MM-dd", null);
            //DateTime plantEtime = DateTime.ParseExact(videoContractInfo.planningEndDate.ToString(), "yyyy-MM-dd", null);
            //DateTime dataSign = DateTime.ParseExact(videoContractInfo.dateSigned.ToString(), "yyyy-MM-dd", null);
            if (videoContractInfo.dateSigned != null)
            {
                ViewBag.dateSigned = videoContractInfo.dateSigned.Value.ToString("yyyy-MM-dd");
            }
            if (videoContractInfo.planningStartDate != null)
            {
                ViewBag.planningStartDate = videoContractInfo.planningStartDate.Value.ToString("yyyy-MM-dd");
            }
            if (videoContractInfo.planningEndDate != null )
            {
                ViewBag.planningEndDate = videoContractInfo.planningEndDate.Value.ToString("yyyy-MM-dd");
            }
            //工程地点分区
            List<SelectListItem> list1 = new List<SelectListItem> {
                 new SelectListItem { Text = "", Value = ""},
                new SelectListItem { Text = "市南区", Value = "市南区"},
                new SelectListItem { Text = "市北区", Value = "市北区" },
                new SelectListItem { Text = "李沧区", Value = "李沧区" }
            };
            ViewBag.constructArea1 = new SelectList(list1, "Value", "Text", videoContractInfo.constructArea);
            ViewBag.constructArea = videoContractInfo.constructArea;
            return View(videoContractInfo);
         
        }

        // POST: VideoContractInfoes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "contractNo,dateSigned,transferUnit,projectName,location,layerCount,buildingArea,planningStartDate,planningEndDate,partAaddress,partALegalRepresent, partAweituoAgent,partAcontactTel,partBadress,partBLegalRepresent,partBweituoAgent,partBcontactTel")] VideoContractInfo contractInfo,string dateSigned,string planningStartDate,string planningEndDate,string constructArea)
        {
            if (ModelState.IsValid)
            {
                if(dateSigned!=null&& dateSigned!="")
                {
                    contractInfo.dateSigned = DateTime.Parse(dateSigned);
                }
                if(planningStartDate!=null&&planningStartDate!="")
                {
                    contractInfo.planningStartDate= DateTime.Parse(planningStartDate);
                }
                if(planningEndDate!=null&& planningEndDate!="")
                {
                    contractInfo.planningEndDate = DateTime.Parse(planningEndDate);
                }
                if (contractInfo.planningStartDate == null)
                {
                    return Content("<script >alert('计划开工时间不能为空，请核查！');window.history.back();</script >");
                }
                if (contractInfo.planningEndDate == null)
                {
                    return Content("<script >alert('计划竣工时间不能为空，请核查！');window.history.back();</script >");
                }
                if (contractInfo.dateSigned == null)
                {
                    return Content("<script >alert('签订日期不能为空，请核查！');window.history.back();</script >");
                }
                contractInfo.constructArea = constructArea;
                db.Entry(contractInfo).State = EntityState.Modified;
                db.SaveChanges();
                return Content("<script >alert('修改成功！');window.location.href='/VideoContractInfoes/Index';</script >");
                //return Content("<script >alert('修改成功！');window.history.back();</script >");
            }
            return View(contractInfo);
        }

        // GET: VideoContractInfoes/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    VideoContractInfo videoContractInfo = db.VideoContractInfo.Find(id);
        //    if (videoContractInfo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(videoContractInfo);
        //}

        // POST: VideoContractInfoes/Delete/5
     
        public ActionResult DeleteConfirmed(string id)
        {
            VideoContractInfo videoContractInfo = db.VideoContractInfo.Find(id);
            db.VideoContractInfo.Remove(videoContractInfo);
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
