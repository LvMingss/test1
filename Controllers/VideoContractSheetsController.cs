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
    public class VideoContractSheetsController : Controller
    {
        private VideoArchiveEntities db = new VideoArchiveEntities();
        private UrbanConEntities db_urban = new UrbanConEntities();
        // GET: VideoContractSheets
        //public ActionResult Index()
        //{
        //    return View(db.VideoContractSheet.ToList());
        //}
        public ActionResult TongJi()
        {
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = "0"},
                new SelectListItem { Text = "工程名称", Value = "1"},
                new SelectListItem { Text = "工程地点分区", Value = "2" },
                new SelectListItem { Text = "工程地点", Value = "3"},
                new SelectListItem { Text = "声像科指导人", Value = "4" }
            };

            ViewBag.SelectedID1 = new SelectList(list1, "Value", "Text", 0);

            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = "0"},
                new SelectListItem { Text = "签订日期", Value = "3"},
                new SelectListItem { Text = "计划开工日期", Value = "1"},
                new SelectListItem { Text = "计划竣工日期", Value = "2" }
            };

            ViewBag.SelectedID2 = new SelectList(list2, "Value", "Text", 0);

            List<SelectListItem> list3 = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = "0"},
                new SelectListItem { Text = "年份", Value = "1"}
            };

            ViewBag.SelectedID3 = new SelectList(list3, "Value", "Text", 0);

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
        public ActionResult TongJi(string action, string SelectedID1, string SelectedID2, string Searching, string startdate, string enddate, string startyear)
        {
            var ds = from b in db.VideoContractSheet
                     select b;
            if (action == "打印全部联系单统计")
            {
                LocalReport localReport = new LocalReport();
                //string Person = Request.Form["collator"];
                //DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                //DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);

                var ds1 = ds;
                var total = ds1.Count();
                localReport.ReportPath = Server.MapPath("~/Report/videosheet.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("videosheet", ds1);
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
            if (action == "打印联系单统计")
            {
                if (Searching != "" && Searching != null)//用户在检索框中输入了检索条件
                {
                    int t = Int32.Parse(SelectedID1.Trim());               
                    switch (t)
                    {                     
                        case 1:
                            ds = ds.Where(ad => ad.projectName.Contains(Searching));
                            break;
                        case 2:
                            ds = ds.Where(ad => ad.constructArea.Contains(Searching));
                            break;
                        case 3:
                            ds = ds.Where(ad => ad.location.Contains(Searching));
                            break;
                        case 4:
                            ds = ds.Where(ad => ad.instructor.Contains(Searching));
                            break;
                        case 0:                    
                            break;
                    }
                }
                if (startdate != "" && startdate != null && enddate != "" && enddate != null)//用户在检索框中输入了检索条件
                {
                    try
                    {
                        DateTime s = DateTime.Parse(startdate.Trim());
                        DateTime e = DateTime.Parse(enddate.Trim());
                        int t1 = Int32.Parse(SelectedID2.Trim());
                        switch (t1)
                        {
                            case 3:
                                ds = ds.Where(ad => ad.fillDate >= s).Where(ad => ad.fillDate <= e);
                                break;
                            case 1:
                                ds = ds.Where(ad => ad.planningStartDate >= s).Where(ad => ad.planningStartDate <= e);
                                break;
                            case 2:
                                ds = ds.Where(ad => ad.planningEndDate >= s).Where(ad => ad.planningEndDate <= e);
                                break;
                            case 0:
                                break;
                        }
                    }
                    catch(Exception e)
                    {
                        return Content("<script >alert('检索条件有误，请核查！');window.history.back();</script >");
                    }
               
                }
                if (startyear != "" && startyear != null)//年份
                {
                    try
                    {
                        ds = ds.Where(ad => ad.year.Contains(startyear));
                    }
                    catch(Exception e)
                    {
                        return Content("<script >alert('检索条件有误，请核查！');window.history.back();</script >");
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
                localReport.ReportPath = Server.MapPath("~/Report/videosheet.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("videosheet", ds1);
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
            db.Configuration.ProxyCreationEnabled = false;
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "建设单位", Value = "2" },
                new SelectListItem { Text = "责任书编号", Value = "3" },
                new SelectListItem { Text = "联系单序号", Value = "4" },
                new SelectListItem { Text = "负责人", Value = "5" },
                new SelectListItem { Text = "指导人", Value = "6" }
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
            var  videoContractSheet = from ad in db.VideoContractSheet 
                               select ad;
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        videoContractSheet = videoContractSheet.Where(ad => ad.projectName.Contains(searchString));//根据工程名称搜索
                        break;
                    case 1:
                        videoContractSheet = videoContractSheet.Where(ad => ad.location.Contains(searchString));//根据工程地点搜索
                        break;
                    case 2:
                        videoContractSheet = videoContractSheet.Where(ad => ad.developmentOrgnization.Contains(searchString));//根据建设单位搜索
                        break;
                    case 3:
                        videoContractSheet = videoContractSheet.Where(ad => ad.contractNo.Contains(searchString));//根据责任书编号搜索
                        break;
                    case 4:
                        videoContractSheet = videoContractSheet.Where(ad => ad.sheetNo.Contains(searchString));//根据工作联系单搜索
                        break;
                    case 5:
                        videoContractSheet = videoContractSheet.Where(ad => ad.videoResponsible.Contains(searchString));//根据工程名称搜索
                        break;
                    case 6:
                        videoContractSheet = videoContractSheet.Where(ad => ad.instructor.Contains(searchString));//根据负责人搜索
                        break;
                }

            }
            // 默认按责任书编号排
            videoContractSheet = videoContractSheet.OrderByDescending(s => s.ID);
            ViewBag.result = JsonConvert.SerializeObject(videoContractSheet);
            return View();
        }

        // GET: VideoContractSheets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoContractSheet videoContractSheet = db.VideoContractSheet.Find(id);
            if (videoContractSheet == null)
            {
                return HttpNotFound();
            }
            return View(videoContractSheet);
        }

        // GET: VideoContractSheets/Create
        public ActionResult Create(int? contractNo)
        {
            VideoContractSheet VideoSheet = new VideoContractSheet();
            //工程地点分区
            List<SelectListItem> list1 = new List<SelectListItem> {
                 new SelectListItem { Text = "", Value = ""},
                new SelectListItem { Text = "市南区", Value = "市南区"},
                new SelectListItem { Text = "市北区", Value = "市北区" },
                new SelectListItem { Text = "李沧区", Value = "李沧区" }
            };
            ViewBag.constructArea1 = new SelectList(list1, "Value", "Text");     

            string plantStime = Convert.ToDateTime(DateTime.Today.Date).ToString("yyyy-MM-dd");
            string plantEtime = Convert.ToDateTime(DateTime.Today.Date).ToString("yyyy-MM-dd");

            ViewBag.planningStartDate = Convert.ToDateTime(plantStime);
            ViewBag.planningEndDate = Convert.ToDateTime(plantEtime);

            if (contractNo != null)
            {
                ViewBag.contractNo = contractNo;
                //判断该联系单是否已经被签订
                var checkisReceive = from a in db.VideoContractSheet
                                     where a.contractNo == contractNo.ToString()
                                     select a;
                if (checkisReceive.Count() > 0)//该联系单已经签订
                {
                    int id = checkisReceive.FirstOrDefault().ID;
                    return Content("<script >alert('该联系单已经签订！');window.location.href='/VideoContractSheets/Edit?id=" + id + "';</script >");
 
                }
                else//从移交责任书的界面签订工作联系单
                {
                    //将移交责任书的相应值传递到联系单中  
                    var sheet = from a in db.VideoContractInfo
                                where a.contractNo == contractNo.ToString()
                                select a;
                   
                    VideoSheet.developmentOrgnization = sheet.First().transferUnit;
                    VideoSheet.projectName = sheet.First().projectName;
                    VideoSheet.location = sheet.First().location;
                    if (sheet.First().buildingArea != null && sheet.First().buildingArea != "")
                    {
                        VideoSheet.buildingArea = decimal.Parse(sheet.First().buildingArea.Trim());
                    }
                    VideoSheet.projectResponsible = sheet.First().partBLegalRepresent;
                    VideoSheet.telephoneNo = sheet.First().partBcontactTel;

                    ViewBag.constructArea1 = new SelectList(list1, "Value", "Text", sheet.First().constructArea.Trim());
                    ViewBag.constructArea = sheet.First().constructArea;

                    ViewBag.planningStartDate = sheet.First().planningStartDate.Value.ToString("yyyy-MM-dd");
                    ViewBag.planningEndDate = sheet.First().planningEndDate.Value.ToString("yyyy-MM-dd");
                }
            }
          
            //档案密级
            ViewBag.securityID = new SelectList(db_urban.SecurityClassification, "securityID", "securityName");
            //保管年限
            ViewBag.retentionPeriodNo = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName");
            //声像科指导人
            List<SelectListItem> zhidaoren = new List<SelectListItem> {
                 new SelectListItem { Text = "", Value = ""},
                new SelectListItem { Text = "李德林", Value = "李德林"},
                new SelectListItem { Text = "孙良浩", Value = "孙良浩" },
                new SelectListItem { Text = "刘吉滨", Value = "刘吉滨" },
                 new SelectListItem { Text = "郑俊泽", Value = "郑俊泽" },
                new SelectListItem { Text = "于小桐", Value = "于小桐" },
                new SelectListItem { Text = "马卫陆", Value = "马卫陆" },
                new SelectListItem { Text = "吴威", Value = "吴威" }
            };
            ViewBag.instructor1 = new SelectList(zhidaoren, "Value", "Text");
            //声像科责任人
            List<SelectListItem> zerenren = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = ""},
                new SelectListItem { Text = "李德林", Value = "李德林"},
                new SelectListItem { Text = "马卫陆", Value = "马卫陆" }
            };
            ViewBag.videoResponsible = new SelectList(zerenren, "Value", "Text");
                  
            int max_id = db.VideoContractSheet.Max(d => d.ID);
            ViewBag.ID = max_id + 1;
            ViewBag.year = DateTime.Now.Year.ToString();
            ViewBag.tianbiaoDate = DateTime.Now.Year.ToString()+"-"+DateTime.Now.Month.ToString()+"-"+ DateTime.Now.Day.ToString();

            return View(VideoSheet);
        }

        // POST: VideoContractSheets/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,sheetNo,contractNo,developmentOrgnization,projectName,location,retentionPeriodNo,securityID,buildingArea,planningStartDate,planningEndDate,constructArea,totalCost,projectResponsible,telephoneNo,mobilephoneNo,contractTel,contractMobileNo,address,email,videoResponsible,weituoContactNo,costFee,partB,fillDate,introduction,isCharge,archiveBoxMaterailFee,scanningCopy,year")] VideoContractSheet videoContractSheet,string instructor)
        {
            videoContractSheet.costFee = 0;
            videoContractSheet.partB = "";
            videoContractSheet.isCharge = false;
            videoContractSheet.archiveBoxMaterailFee = 0;
            videoContractSheet.scanningCopy = false;
            videoContractSheet.instructor = instructor;
           //var checkisReceive = from a in db.VideoContractSheet
           //                      where a.sheetNo == videoContractSheet.sheetNo
           //                      select a;
            //if (checkisReceive.Count() > 0)//该联系单单号已经存在
            //{
            //    return Content("<script >alert('该联系单单号已存在，请重新填写！！');window.history.back();</script >");

            //}

            string yearno = videoContractSheet.year;//获取前台传过来的year
            var sheet = db.VideoContractSheet.Where(x => x.year == yearno);
            if (sheet.Count()>0)//假如有该年份的联系单了，工程号自动累加+1
            {
                videoContractSheet.projectID = sheet.Max(x => x.projectID)+1;
            }
            else//假如没有该年份的联系单了，工程号初始化比如：2017001
            {
                videoContractSheet.projectID = long.Parse(yearno.ToString() + "001");
            }
            if (ModelState.IsValid)
            {
                db.VideoContractSheet.Add(videoContractSheet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(videoContractSheet);
        }

        // GET: VideoContractSheets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoContractSheet videoContractSheet = db.VideoContractSheet.Find(id);
            //档案密级
            ViewBag.securityID= new SelectList(db_urban.SecurityClassification, "securityID", "securityName");
            if (videoContractSheet.securityID != null)
            {
                ViewBag.securityID = new SelectList(db_urban.SecurityClassification, "securityID", "securityName", videoContractSheet.securityID);
            }
            //保管年限
            ViewBag.retentionPeriodNo = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName");
            if (videoContractSheet.retentionPeriodNo != null)
            {
                ViewBag.retentionPeriodNo = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", videoContractSheet.retentionPeriodNo);
            }
            //声像科指导人
            List<SelectListItem> zhidaoren = new List<SelectListItem> {
                 new SelectListItem { Text = "", Value = ""},
                new SelectListItem { Text = "李德林", Value = "李德林"},
                new SelectListItem { Text = "孙良浩", Value = "孙良浩" },
                new SelectListItem { Text = "刘吉滨", Value = "刘吉滨" },
                 new SelectListItem { Text = "郑俊泽", Value = "郑俊泽" },
                new SelectListItem { Text = "于小桐", Value = "于小桐" },
                new SelectListItem { Text = "马卫陆", Value = "马卫陆" },
                new SelectListItem { Text = "吴威", Value = "吴威" }
            };
            ViewBag.instructor1 = new SelectList(zhidaoren, "Value", "Text");
            ViewBag.instructor = videoContractSheet.instructor;

            //声像科责任人
            List<SelectListItem> zerenren = new List<SelectListItem> {
                 new SelectListItem { Text = " ", Value = " "},
                new SelectListItem { Text = "李德林", Value = "李德林"},
                new SelectListItem { Text = "马卫陆", Value = "马卫陆" }
            };

            ViewBag.videoResponsible = new SelectList(zerenren, "Value", "Text");
            if (videoContractSheet.videoResponsible != null)
            {
                ViewBag.videoResponsible = new SelectList(zerenren, "Value", "Text", videoContractSheet.videoResponsible.Trim());
            }

           
            ViewBag.tianbiaoDate = videoContractSheet.fillDate.Value.ToString("yyyy-MM-dd");
            if (videoContractSheet.fillDate != null)
            {
                ViewBag.fillDate = videoContractSheet.fillDate.Value.ToString("yyyy-MM-dd");
            }
            if (videoContractSheet.planningStartDate != null)
            {
                ViewBag.planningStartDate = videoContractSheet.planningStartDate.Value.ToString("yyyy-MM-dd");
            }
            if (videoContractSheet.planningEndDate != null)
            {
                ViewBag.planningEndDate = videoContractSheet.planningEndDate.Value.ToString("yyyy-MM-dd");
            }
            //工程地点分区
            List<SelectListItem> list1 = new List<SelectListItem> {
                 new SelectListItem { Text = "", Value = ""},
                new SelectListItem { Text = "市南区", Value = "市南区"},
                new SelectListItem { Text = "市北区", Value = "市北区" },
                new SelectListItem { Text = "李沧区", Value = "李沧区" }
            };
            ViewBag.constructArea1 = new SelectList(list1, "Value", "Text");
            ViewBag.constructArea = videoContractSheet.constructArea;
            //if (videoContractSheet == null)
            //{
            //    return HttpNotFound();
            //}
            return View(videoContractSheet);
        }

        // POST: VideoContractSheets/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,sheetNo,contractNo,developmentOrgnization,projectName,location,retentionPeriodNo,securityID,buildingArea,planningStartDate,planningEndDate,constructArea,totalCost,projectResponsible,telephoneNo,mobilephoneNo,contractTel,contractMobileNo,address,email,videoResponsible,weituoContactNo,costFee,partB,fillDate,introduction,isCharge,archiveBoxMaterailFee,scanningCopy,projectID,year")] VideoContractSheet videoContractSheet,string instructor)
        {
            if (ModelState.IsValid)
            {
                videoContractSheet.instructor = instructor;
                db.Entry(videoContractSheet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(videoContractSheet);
        }

        //// GET: VideoContractSheets/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    VideoContractSheet videoContractSheet = db.VideoContractSheet.Find(id);
        //    if (videoContractSheet == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(videoContractSheet);
        //}

        //// POST: VideoContractSheets/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    VideoContractSheet videoContractSheet = db.VideoContractSheet.Find(id);
        //    db.VideoContractSheet.Remove(videoContractSheet);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        public ActionResult Delete(long id)
        {
            try
            {
                VideoContractSheet videoContractSheet = db.VideoContractSheet.Find(id);
                db.VideoContractSheet.Remove(videoContractSheet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return Content("<script >alert('删除失败，请核查！');window.history.back();</script >");
            }

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
