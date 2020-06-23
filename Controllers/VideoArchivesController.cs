using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using urban_archive.Models;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Text;

namespace urban_archive.Controllers
{
    public class VideoArchivesController : Controller
    {
        private VideoArchiveEntities db = new VideoArchiveEntities();

        // GET: VideoArchives
        //public ActionResult Index()
        //{
        //    var videoArchives = db.VideoArchives.Where(a=>a.videoStatus=="1");
        //    return View(videoArchives.ToList());
        //}
        public ActionResult TongJi()
        {
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = "0"},
                new SelectListItem { Text = "工程名称", Value = "1"},
                new SelectListItem { Text = "工程地点分区", Value = "2"},
                new SelectListItem { Text = "工程地点", Value = "3"},
                 new SelectListItem { Text = "声像科指导人", Value = "4"},
            };

            ViewBag.SelectedID1 = new SelectList(list1, "Value", "Text", 0);

            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = "0"},
                 new SelectListItem { Text = "待审核", Value = "1"},
                new SelectListItem { Text = "已审核", Value = "2"},
                new SelectListItem { Text = "待入库", Value = "3"},
                new SelectListItem { Text = "入库", Value = "4"},
                new SelectListItem { Text = "驳回", Value = "5" },
                new SelectListItem { Text = "暂入库", Value = "6" }
            };
            ViewBag.SelectedID2 = new SelectList(list2, "Value", "Text", 0);

            List<SelectListItem> list3 = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = "0"},
                new SelectListItem { Text = "接收日期", Value = "3"},
                new SelectListItem { Text = "计划开工日期", Value = "1"},
                new SelectListItem { Text = "计划竣工日期", Value = "2" }
            };

            ViewBag.SelectedID3 = new SelectList(list3, "Value", "Text", 4);
          
            return View();
        }
        [HttpPost]
        public ActionResult TongJi(string action, string SelectedID1, string SelectedID2, string SelectedID3, string Searching, string startdate, string enddate, string startyear)
        {
            var ds = from c in db.VideoArchives
                     select c;
            foreach(var item in ds)
            {
                if(item.instructor!=null&&item.instructor!=null)
                {
                    item.instructor = item.instructor.Trim();
                }
            }
            if (action == "打印全部声像档案统计")
            {
                LocalReport localReport = new LocalReport();
                //string Person = Request.Form["collator"];
                //DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                //DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);

                var ds1 = ds;
                var total = ds1.Count();
                localReport.ReportPath = Server.MapPath("~/Report/videoarchive.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("videoarchive", ds1);
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
            if (action == "打印声像档案统计")
            {
                if (Searching != "" && Searching != null)//用户在检索框中输入了检索条件
                {
                    int t = Int32.Parse(SelectedID1.Trim());
                    switch (t)
                    {
                        case 3:
                            ds = ds.Where(ad => ad.location.Contains(Searching));
                            break;
                        case 1:
                            ds = ds.Where(ad => ad.projectName.Contains(Searching));
                            break;
                        case 2:
                            ds = ds.Where(ad => ad.constructArea.Contains(Searching));
                            break;
                        case 4:
                            ds = ds.Where(ad => ad.instructor.Contains(Searching));
                            break;
                        case 0:
                            break;
                    }
                }
                if (SelectedID2 != "0" && SelectedID2 != null)//档案状态
                {
                    string t = SelectedID2.Trim();
                    ds = ds.Where(ad => ad.videoStatus==t);
                }
                if (startdate != "" && startdate != null && enddate != "" && enddate != null)//接收日期
                {
                    try
                    {
                        DateTime s = DateTime.Parse(startdate.Trim());
                        DateTime e = DateTime.Parse(enddate.Trim());
                        int t1 = Int32.Parse(SelectedID3.Trim());
                        switch (t1)
                        {
                            case 3:
                                ds = ds.Where(ad => ad.dateReceived >= s).Where(ad => ad.dateReceived <= e);
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
                    catch (Exception e)
                    {
                        return Content("<script >alert('检索条件有误，请核查！');window.history.back();</script >");
                    }

                }
                if (startyear != "" && startyear != null)//年份
                {
                    try
                    {
                        ds = ds.Where(ad => ad.qjdsyYear.Contains(startyear));
                    }
                    catch (Exception e)
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
                var status = "";
                switch (SelectedID2.Trim())
                {
                    case "1":
                        status = "待审核";
                        break;
                    case "2":
                        status = "已审核";
                        break;
                    case "3":
                        status = "待入库";
                        break;
                    case "4":
                        status = "入库";
                        break;
                    case "5":
                        status = "驳回";
                        break;
                    case "6":
                        status = "暂入库";
                        break;
                }
                localReport.ReportPath = Server.MapPath("~/Report/videoarchive.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("videoarchive", ds1);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("total", total.ToString().Trim()));
                parameterList.Add(new ReportParameter("status", status.ToString().Trim()));

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
            if(action== "打印声像档案统计（Excel）")
            {
                if (Searching != "" && Searching != null)//用户在检索框中输入了检索条件
                {
                    int t = Int32.Parse(SelectedID1.Trim());
                    switch (t)
                    {
                        case 3:
                            ds = ds.Where(ad => ad.location.Contains(Searching));
                            break;
                        case 1:
                            ds = ds.Where(ad => ad.projectName.Contains(Searching));
                            break;
                        case 2:
                            ds = ds.Where(ad => ad.constructArea.Contains(Searching));
                            break;
                        case 4:
                            ds = ds.Where(ad => ad.instructor.Contains(Searching));
                            break;
                        case 0:
                            break;
                    }
                }
                if (SelectedID2 != "0" && SelectedID2 != null)//档案状态
                {
                    string t = SelectedID2.Trim();
                    ds = ds.Where(ad => ad.videoStatus == t);
                }
                if (startdate != "" && startdate != null && enddate != "" && enddate != null)//接收日期
                {
                    try
                    {
                        DateTime s = DateTime.Parse(startdate.Trim());
                        DateTime e = DateTime.Parse(enddate.Trim());
                        int t1 = Int32.Parse(SelectedID3.Trim());
                        switch (t1)
                        {
                            case 3:
                                ds = ds.Where(ad => ad.dateReceived >= s).Where(ad => ad.dateReceived <= e);
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
                    catch (Exception e)
                    {
                        return Content("<script >alert('检索条件有误，请核查！');window.history.back();</script >");
                    }

                }
                if (startyear != "" && startyear != null)//年份
                {
                    try
                    {
                        ds = ds.Where(ad => ad.qjdsyYear.Contains(startyear));
                    }
                    catch (Exception e)
                    {
                        return Content("<script >alert('检索条件有误，请核查！');window.history.back();</script >");
                    }
                }




                //接收需要导出的数据
                var ds1 = ds;
                //命名导出表格的StringBuilder变量
                StringBuilder sHtml = new StringBuilder(string.Empty);
                //打印表头
                sHtml.Append("<table border=\"1\" width=\"100%\">");
                sHtml.Append("<tr height=\"40\"><td colspan=\"8\" align=\"center\" style='font-size:24px'><b>声像工程信息" + "</b></td></tr>");
                //打印列名
                sHtml.Append("<tr height=\"20\" align=\"center\" ><td>序号</td><td>项目顺序号</td><td>工程名称</td><td>工程地点</td><td>工程地点分区</td><td>工程面积</td><td>档案状态</td><td>计划开工日期</td><td>计划竣工日期</td><td>声像科指导人</td></tr>");
                //循环读取List集合 
                int i = 0;
               foreach(var item in ds1)
                {
                    var status = "";
                   if(item.videoStatus=="1")
                    {
                        status = "待审核";
                    }
                    if (item.videoStatus == "2")
                    {
                        status = "已审核";
                    }
                    if (item.videoStatus == "3")
                    {
                        status = "待入库";
                    }
                    if (item.videoStatus == "4")
                    {
                        status = "入库";
                    }
                    if (item.videoStatus == "5")
                    {
                        status = "驳回";
                    }
                    if (item.videoStatus == "6")
                    {
                        status = "暂入库";
                    }

                    i++;
                    
                
                    sHtml.Append("<tr height=\"20\" align=\"left\"><td>" + i
                            + "</td><td>" + item.videoProjectSeqNo + "</td><td>" + item.projectName + "</td><td>" + item.location
                            + "</td><td>"+ item.constructArea + "</td><td>" + item.buildingArea + "</td><td>" + status + "</td><td>" +item.planningStartDate +"</td><td>"+ item.planningEndDate + "</td><td>" + item.instructor + "</td></tr>");
                }
                //打印表尾
                sHtml.Append("</table>");
                //调用输出Excel表的方法
                ExportToExcel("application/ms-excel", "声像科工程信息.xls", sHtml.ToString());
            }
            //return RedirectToAction("TongJi");
            return Content("<script >alert('生成成功！');window.history.back();</script >");
        }
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "项目顺序号", Value = "2" },
                new SelectListItem { Text = "接收日期", Value = "3" },
                new SelectListItem { Text = "声像科指导人", Value = "4" }
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
            var videoArchives = db.VideoArchives.Where(a => a.videoStatus == "1");
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        videoArchives = videoArchives.Where(ad => ad.projectName == searchString);//根据工程名称搜索
                        break;
                    case 1:
                        videoArchives = videoArchives.Where(ad => ad.location == searchString);
                        break;
                    case 2:
                        videoArchives = videoArchives.Where(ad => ad.videoProjectSeqNo == long.Parse(searchString.Trim()));//
                        break;
                    case 3:
                        videoArchives = videoArchives.Where(ad => ad.dateReceived.ToString().Contains(searchString.Trim()));//
                        break;
                    case 4:
                        videoArchives = videoArchives.Where(ad => ad.instructor == searchString.Trim());//
                        break;
                }

            }
            // 默认按责任书编号排
            videoArchives = videoArchives.OrderByDescending(s => s.ID);
            ViewBag.result = JsonConvert.SerializeObject(videoArchives);
            return View();
        }
        public ViewResult Index_shenhe(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "项目顺序号", Value = "2" },
                new SelectListItem { Text = "接收日期", Value = "3" },
                new SelectListItem { Text = "声像科指导人", Value = "4" }
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
            var videoArchives = db.VideoArchives.Where(a => a.videoStatus == "2");
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        videoArchives = videoArchives.Where(ad => ad.projectName.Contains(searchString.Trim()) );//根据工程名称搜索
                        break;
                    case 1:
                        videoArchives = videoArchives.Where(ad => ad.location.Contains(searchString.Trim()));
                        break;
                    case 2:
                        videoArchives = videoArchives.Where(ad => ad.videoProjectSeqNo == long.Parse(searchString.Trim()));//
                        break;
                    case 3:
                        videoArchives = videoArchives.Where(ad => ad.dateReceived.ToString().Contains(searchString.Trim()));//
                        break;
                    case 4:
                        videoArchives = videoArchives.Where(ad => ad.instructor.Contains(searchString.Trim()));//
                        break;
                }

            }
            // 默认按责任书编号排
            videoArchives = videoArchives.OrderByDescending(s => s.ID);
            ViewBag.result = JsonConvert.SerializeObject(videoArchives);
            return View();
        }
        public ViewResult Index_bohui(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "项目顺序号", Value = "2" },
                new SelectListItem { Text = "接收日期", Value = "3" },
                new SelectListItem { Text = "声像科指导人", Value = "4" }
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
            var videoArchives = db.VideoArchives.Where(a => a.videoStatus == "5");
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        videoArchives = videoArchives.Where(ad => ad.projectName.Contains(searchString.Trim()));//根据工程名称搜索
                        break;
                    case 1:
                        videoArchives = videoArchives.Where(ad => ad.location.Contains(searchString.Trim()));
                        break;
                    case 2:
                        videoArchives = videoArchives.Where(ad => ad.videoProjectSeqNo == long.Parse(searchString.Trim()));//
                        break;
                    case 3:
                        videoArchives = videoArchives.Where(ad => ad.dateReceived.ToString().Contains(searchString.Trim()));//
                        break;
                    case 4:
                        videoArchives = videoArchives.Where(ad => ad.instructor.Contains(searchString.Trim()));//
                        break;
                }

            }
            // 默认按责任书编号排
            videoArchives = videoArchives.OrderByDescending(s => s.ID);
            ViewBag.result = JsonConvert.SerializeObject(videoArchives);
            return View();
        }
        public ViewResult Index_dairuku(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "项目顺序号", Value = "2" },
                new SelectListItem { Text = "接收日期", Value = "3" },
                new SelectListItem { Text = "声像科指导人", Value = "4" }
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
            var videoArchives = db.VideoArchives.Where(a => a.videoStatus == "3");
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        videoArchives = videoArchives.Where(ad => ad.projectName.Contains(searchString.Trim()));//根据工程名称搜索
                        break;
                    case 1:
                        videoArchives = videoArchives.Where(ad => ad.location.Contains(searchString.Trim()));
                        break;
                    case 2:
                        videoArchives = videoArchives.Where(ad => ad.videoProjectSeqNo == long.Parse(searchString.Trim()));//
                        break;
                    case 3:
                        videoArchives = videoArchives.Where(ad => ad.dateReceived.ToString().Contains(searchString.Trim()));//
                        break;
                    case 4:
                        videoArchives = videoArchives.Where(ad => ad.instructor.Contains(searchString.Trim()));//
                        break;
                }

            }
            // 默认按责任书编号排
            videoArchives = videoArchives.OrderByDescending(s => s.ID);
            ViewBag.result = JsonConvert.SerializeObject(videoArchives);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index_dairuku()
        {
            var videoArchives = db.VideoArchives.Where(a => a.videoStatus == "3");
            foreach (var item in videoArchives)//
            {
                item.videoStatus = "4";
                item.rukutime = DateTime.Today;
                db.Entry(item).State = EntityState.Modified;
            }
            db.SaveChanges();
            return RedirectToAction("Index_ruku");
        }
        public ViewResult Index_ruku(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "项目顺序号", Value = "2" },
                new SelectListItem { Text = "接收日期", Value = "3" },
                new SelectListItem { Text = "声像科指导人", Value = "4" }
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
            var videoArchives = db.VideoArchives.Where(a => a.videoStatus == "4");
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        videoArchives = videoArchives.Where(ad => ad.projectName.Contains(searchString.Trim()));//根据工程名称搜索
                        break;
                    case 1:
                        videoArchives = videoArchives.Where(ad => ad.location.Contains(searchString.Trim()));
                        break;
                    case 2:
                        videoArchives = videoArchives.Where(ad => ad.videoProjectSeqNo == long.Parse(searchString.Trim()));//
                        break;
                    case 3:
                        videoArchives = videoArchives.Where(ad => ad.dateReceived.ToString().Contains(searchString.Trim()));//
                        break;
                    case 4:
                        videoArchives = videoArchives.Where(ad => ad.instructor.Contains(searchString.Trim()));//
                        break;
                }

            }
            // 默认按责任书编号排
            videoArchives = videoArchives.OrderByDescending(s => s.ID);
            ViewBag.result = JsonConvert.SerializeObject(videoArchives);
            return View();
        }
        public ViewResult Index_zanruku(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "项目顺序号", Value = "2" },
                new SelectListItem { Text = "接收日期", Value = "3" },
                new SelectListItem { Text = "声像科指导人", Value = "4" }
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
            var videoArchives = db.VideoArchives.Where(a => a.videoStatus == "6");
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        videoArchives = videoArchives.Where(ad => ad.projectName.Contains(searchString.Trim()));//根据工程名称搜索
                        break;
                    case 1:
                        videoArchives = videoArchives.Where(ad => ad.location.Contains(searchString.Trim()));
                        break;
                    case 2:
                        videoArchives = videoArchives.Where(ad => ad.videoProjectSeqNo == long.Parse(searchString.Trim()));//
                        break;
                    case 3:
                        videoArchives = videoArchives.Where(ad => ad.dateReceived.ToString().Contains(searchString.Trim()));//
                        break;
                    case 4:
                        videoArchives = videoArchives.Where(ad => ad.instructor.Contains(searchString.Trim()));//
                        break;
                }

            }
            // 默认按责任书编号排
            videoArchives = videoArchives.OrderByDescending(s => s.ID);
            ViewBag.result = JsonConvert.SerializeObject(videoArchives);
            return View();
        }

        // GET: VideoArchives/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoArchives videoArchives = db.VideoArchives.Find(id);
            if (videoArchives == null)
            {
                return HttpNotFound();
            }
            return View(videoArchives);
        }

        // GET: VideoArchives/Create
        public ActionResult Create(int?SheetID,int? videoProjectSeqNo)
        {
            if(SheetID==null&&videoProjectSeqNo!=null)
            {
                return Content("<script >alert('该联系单缺失工程信息！请重新录入工作联系单！');window.location.href='/VideoContractSheets/Index';</script >");
                //return RedirectToAction("Index", "VideoContractSheets");
            }
            //判断该工程是否已经被接收
            var checkisReceive = from a in db.VideoArchives
                                 where a.SheetID == SheetID
                                 select a;
            if (checkisReceive.Count() > 0)//该工程已接收
            {
                int id = checkisReceive.FirstOrDefault().ID;
                return Content("<script >alert('该工程已接收！');window.location.href='/VideoArchives/Edit?id="+id+"';</script >");
                //return RedirectToAction("Edit",new {id= checkisReceive.FirstOrDefault().ID });
            }
            //string now = Convert.ToDateTime(DateTime.Today.Date).ToString("yyyy-MM-dd");
            //ViewBag.now = now;

            ViewBag.SheetID = SheetID;
            ViewBag.videoProjectSeqNo = videoProjectSeqNo;
            int max_id = db.VideoArchives.Max(d => d.ID);
            ViewBag.ID = max_id + 1;
            var sheet = db.VideoContractSheet.Find(SheetID);
            ViewBag.projectName = sheet.projectName;//工程名称
            ViewBag.location = sheet.location;//工程地点
            ViewBag.instructor = sheet.instructor;
            ViewBag.buildingArea = sheet.buildingArea;
            if (sheet.fillDate != null)
            {
                ViewBag.fillDate = sheet.fillDate.Value.ToString("yyyy-MM-dd");
            }
            if (sheet.planningStartDate != null)
            {
                ViewBag.planningStartDate = sheet.planningStartDate.Value.ToString("yyyy-MM-dd");
            }
            if (sheet.planningEndDate != null)
            {
                ViewBag.planningEndDate = sheet.planningEndDate.Value.ToString("yyyy-MM-dd");
            }
            //工程地点分区
            List<SelectListItem> list1 = new List<SelectListItem> {
                 new SelectListItem { Text = "", Value = ""},
                new SelectListItem { Text = "市南区", Value = "市南区"},
                new SelectListItem { Text = "市北区", Value = "市北区" },
                new SelectListItem { Text = "李沧区", Value = "李沧区" }
            };
            ViewBag.constructArea1 = new SelectList(list1, "Value", "Text", sheet.constructArea);
            ViewBag.constructArea = sheet.constructArea;

            //声像科指导人
            List<SelectListItem> zhidaoren = new List<SelectListItem> {
                new SelectListItem { Text = "", Value = ""},
                new SelectListItem { Text = "李德林", Value = "李德林"},
                new SelectListItem { Text = "马卫陆", Value = "马卫陆" }
            };
            //经办人：
            ViewBag.operater = new SelectList(zhidaoren, "Value", "Text");
            //初审人：
            ViewBag.csyjPerson1 = new SelectList(zhidaoren, "Value", "Text");
            return View();
        }

        // POST: VideoArchives/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "videoProjectSeqNo,dateReceived,projectName,SheetID,location,buildingArea,planningStartDate,planningEndDate,constructArea,instructor,qjdsyYear,qjdsyNo,csyj,csyjPerson,csyjDate,operater,videoCassetteBoxCount,photoBoxCount,ID")] VideoArchives videoArchives, string csyjPerson)
        {
            videoArchives.videoStatus = "1";
            if (ModelState.IsValid)
            {
                videoArchives.csyjPerson = csyjPerson.Trim();
                db.VideoArchives.Add(videoArchives);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return Content("<script >alert('信息有误，请核查！');window.history.back();</script >");
               // return Content("<script >alert('信息有误！');window.location.href='/VideoContractSheets/Create?=SheetID="+videoArchives.SheetID+ "&videoProjectSeqNo="+videoArchives.videoProjectSeqNo+"';</script >");
            }
        }

        // GET: VideoArchives/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoArchives videoArchives = db.VideoArchives.Find(id);
            //声像科指导人
            List<SelectListItem> zhidaoren = new List<SelectListItem> {
                new SelectListItem { Text = "李德林", Value = "李德林"},
                new SelectListItem { Text = "马卫陆", Value = "马卫陆" }
            };
            //经办人：
            ViewBag.operater = new SelectList(zhidaoren, "Value", "Text", videoArchives.operater);
            //初审人：
            ViewBag.csyjPerson1 = new SelectList(zhidaoren, "Value", "Text", videoArchives.csyjPerson.Trim());
            ViewBag.csyjPerson = videoArchives.csyjPerson.Trim();
            if (videoArchives.csyjDate != null)
            {
                ViewBag.csyjDate = videoArchives.csyjDate.Value.ToString("yyyy-MM-dd");
            }
            if (videoArchives.dateReceived != null)
            {
                ViewBag.dateReceived = videoArchives.dateReceived.Value.ToString("yyyy-MM-dd");
            }
            if (videoArchives == null)
            {
                return HttpNotFound();
            }
            var status = videoArchives.videoStatus;
            ViewBag.fanhui = "Index";
            if (status == "2")
            {
                ViewBag.fanhui = "Index_shenhe";
            }
            if (status == "3")
            {
                ViewBag.fanhui = "Index_dairuku";
            }
            if (status == "4")
            {
                ViewBag.fanhui = "Index_ruku";
            }
            if (status == "5")
            {
                ViewBag.fanhui = "Index_bohui";
            }
            if (status == "6")
            {
                ViewBag.fanhui = "Index_zanruku";
            }
            if (videoArchives.planningStartDate != null)
            {
                ViewBag.planningStartDate = videoArchives.planningStartDate.Value.ToString("yyyy-MM-dd");
            }
            if (videoArchives.planningEndDate != null)
            {
                ViewBag.planningEndDate = videoArchives.planningEndDate.Value.ToString("yyyy-MM-dd");
            }
            //工程地点分区
            List<SelectListItem> list1 = new List<SelectListItem> {
                 new SelectListItem { Text = "", Value = ""},
                new SelectListItem { Text = "市南区", Value = "市南区"},
                new SelectListItem { Text = "市北区", Value = "市北区" },
                new SelectListItem { Text = "李沧区", Value = "李沧区" }
            };
            ViewBag.constructArea1 = new SelectList(list1, "Value", "Text", videoArchives.constructArea);
            ViewBag.constructArea = videoArchives.constructArea;
            return View(videoArchives);
        }

        // POST: VideoArchives/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "instructor,videoProjectSeqNo,dateReceived,projectName,SheetID,location,buildingArea,planningStartDate,planningEndDate,constructArea,instructor,qjdsyYear,qjdsyNo,csyj,csyjPerson,csyjDate,fzryj,fzryjPerson,fzryjDate,zgyj,zgyjPerson,zgyjDate,operater,checker,videoCassetteBoxCount,photoBoxCount,opticalDiskCount,constructUnit,dateConstrcuted,timeConstrcuted,startVideoArchivesNo,endVideoArchivesNo,startVideoRegisNo,endVideoRegisNo,startVideoPaijiaNo,endVideoPaijiaNo,startPhotoArchivesNo,endPhotoArchivesNo,startPhotoRegisNo,endPhotoRegisNo,startPhotoPaijiaNo,endPhotoPaijiaNo,videoStatus,ID")] VideoArchives videoArchives, string csyjPerson,string action)
        {
            if (action == "返回")
            {
                var status = videoArchives.videoStatus;
                if (status == "2")
                {
                    return RedirectToAction("Index_shenhe");
                }
                if (status == "3")
                {
                    return RedirectToAction("Index_dairuku");
                }
                if (status == "4")
                {
                    return RedirectToAction("Index_ruku");
                }
                if (status == "5")
                {
                    return RedirectToAction("Index_bohui");
                }
                if (status == "6")
                {
                    return RedirectToAction("Index_zanruku");
                }
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                videoArchives.csyjPerson = csyjPerson.Trim();
                var status = videoArchives.videoStatus;
                db.Entry(videoArchives).State = EntityState.Modified;
                db.SaveChanges();
                if (status == "2")
                {
                    return RedirectToAction("Index_shenhe");
                }
                if (status == "3")
                {
                    return RedirectToAction("Index_dairuku");
                }
                if (status == "4")
                {
                    return RedirectToAction("Index_ruku");
                }
                if (status == "5")
                {
                    return RedirectToAction("Index_bohui");
                }
                if (status == "6")
                {
                    return RedirectToAction("Index_zanruku");
                }
                return RedirectToAction("Index");
            }
            else
            {
                return Content("<script >alert('信息有误，请核查！');window.history.back();</script >");
            }
        }

        // GET: VideoArchives/Edit/5
        public ActionResult Shenhe(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoArchives videoArchives = db.VideoArchives.Find(id);
            //工作联系单信息
            VideoContractSheet videoContractSheet = db.VideoContractSheet.Find(videoArchives.SheetID);
            ViewBag.sheetNo = videoContractSheet.sheetNo;
            ViewBag.fillDate = videoContractSheet.fillDate;
            ViewBag.developmentOrgnization = videoContractSheet.developmentOrgnization;
            ViewBag.location = videoContractSheet.location;
            ViewBag.buildingArea = videoContractSheet.buildingArea;
            ViewBag.projectResponsible = videoContractSheet.projectResponsible;

            ViewBag.fzryij = "经审核，该工程声像竣工档案接收情况属实，声像科拟签发《建设工程声像竣工档案验收意见书》，请批示！";
            if (videoArchives == null)
            {
                return HttpNotFound();
            }
            return View(videoArchives);
        }

        // POST: VideoArchives/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Shenhe([Bind(Include = "videoProjectSeqNo,dateReceived,projectName,SheetID,location,buildingArea,planningStartDate,planningEndDate,constructArea,instructor,qjdsyYear,qjdsyNo,csyj,csyjPerson,csyjDate,fzryj,fzryjPerson,fzryjDate,zgyj,zgyjPerson,zgyjDate,operater,checker,videoCassetteBoxCount,photoBoxCount,opticalDiskCount,constructUnit,dateConstrcuted,timeConstrcuted,startVideoArchivesNo,endVideoArchivesNo,startVideoRegisNo,endVideoRegisNo,startVideoPaijiaNo,endVideoPaijiaNo,startPhotoArchivesNo,endPhotoArchivesNo,startPhotoRegisNo,endPhotoRegisNo,startPhotoPaijiaNo,endPhotoPaijiaNo,videoStatus,ID")] VideoArchives videoArchives)
        {
            if (ModelState.IsValid)
            {
                db.Entry(videoArchives).State = EntityState.Modified;
                videoArchives.videoStatus = "2";
                db.SaveChanges();
                return RedirectToAction("Index_shenhe");
            }
            return View(videoArchives);
        }

        //// GET: VideoArchives/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    VideoArchives videoArchives = db.VideoArchives.Find(id);
        //    if (videoArchives == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(videoArchives);
        //}

        //// POST: VideoArchives/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    VideoArchives videoArchives = db.VideoArchives.Find(id);
        //    db.VideoArchives.Remove(videoArchives);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        public ActionResult Delete(long id)
        {
            try
            {
                VideoArchives videoArchives = db.VideoArchives.Find(id);
                var status = videoArchives.videoStatus;
                var videoCassetteList = db.VideoCassetteList.Where(a => a.ProjectIDS.Trim() == videoArchives.videoProjectSeqNo.ToString().Trim());
                foreach(var tem in videoCassetteList)
                {
                    VideoCassetteList del = tem;
                    db.VideoCassetteList.Remove(del);
                }               
                db.VideoArchives.Remove(videoArchives);
                string strFileSavePath = Server.MapPath("~/声像资料/" + videoArchives.videoProjectSeqNo);//文件存储路径
                if (Directory.Exists(strFileSavePath))
                {
                    DirectoryInfo dir = new DirectoryInfo(strFileSavePath);//删除该目录
                    dir.Delete(true);
                }
                db.SaveChanges();
                if (status == "2")
                {
                    return RedirectToAction("Index_shenhe");
                }
                if (status == "3")
                {
                    return RedirectToAction("Index_dairuku");
                }
                if (status == "4")
                {
                    return RedirectToAction("Index_ruku");
                }
                if (status == "5")
                {
                    return RedirectToAction("Index_bohui");
                }
                if (status == "6")
                {
                    return RedirectToAction("Index_zanruku");
                }
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
        //输入HTTP头，然后把指定的流输出到指定的文件名，然后指定文件类型
        public void ExportToExcel(string FileType, string FileName, string ExcelContent)
        {
            System.Web.HttpContext.Current.Response.ContentType = FileType;
            System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            System.Web.HttpContext.Current.Response.Charset = "utf-8";
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(FileName, System.Text.Encoding.UTF8).ToString());

            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.HttpContext.Current.Response.Output.Write(ExcelContent.ToString());
            /*乱码BUG修改 20140505*/
            //如果采用以上代码导出时出现内容乱码，可将以下所注释的代码覆盖掉上面【System.Web.HttpContext.Current.Response.Output.Write(ExcelContent.ToString());】即可实现。
            //System.Web.HttpContext.Current.Response.Write("<meta http-equiv=\"content-type\" content=\"application/ms-excel; charset=utf-8\"/>" + ExcelContent.ToString());
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
        }
    }
}
