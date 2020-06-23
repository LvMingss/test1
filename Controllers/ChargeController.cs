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
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;


namespace urban_archive.Controllers
{
    public class ChargeController : Controller
    {
        private OfficeEntities ab = new OfficeEntities();
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities cb = new UrbanUsersEntities();
        //public class feiyongchaxun
        //{
        //}
        // GET: Charge
        public ActionResult Index()
        {
            return View(db.vw_charge.ToList());
        }
        public JsonResult NameSelection(string q)
        {
            var people = User.Identity.GetUserId();
            var department = cb.AspNetUsers.Where(a => a.Id == people).First().DepartmentName;
            var departmentid = db.DepartmentCode.Where(a => a.text.Trim() == department).First().value;
            var list = db.Charger.Select(p => new { p.unitName }).Distinct().Where(p => p.unitName.Contains(q));
            //var list = from bb in db.Charger
            //               //where bb.fromDepartment == departmentid
            //           where bb.unitName.Contains(q)
            //           orderby bb.ID descending
           var  list1 = from bb in list
                   select new
                       {
                           name = bb.unitName,
                       };
            //if (!string.IsNullOrEmpty(q))
            //{
            //    list = from bb in db.Charger
            //           where bb.fromDepartment == departmentid

            //}

            return Json(list1, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChargeStandard()
        {
            ViewData["pagename"] = "Charge/ChargeStandard";

            return View();
        }
        public ActionResult TuZhiTongJi(string action, string type = "PDF")
        {
            ViewData["pagename"] = "Charge/TuZhiTongJi";
            if (action == "生成统计表")
            {
                LocalReport localReport = new LocalReport();
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                var ds1 = db.FuyinFeeDetail.Where(ad => ad.dateCharged >= DataFrom).Where(ad => ad.dateCharged <= DataTo);
                localReport.ReportPath = Server.MapPath("~/Report/Office/TuZhiTongJi.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("TuZhiTongJi", ds1);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString("yyyy-MM-dd")));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString("yyyy-MM-dd")));
                localReport.SetParameters(parameterList);
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
            return View();
        }
        public ActionResult printFeeList( string id,string type = "PDF")               //打印打复印收费清单
        {
            string name = db.Charger.Where(a => a.seqNo == id).First().unitName;
            var ds = db.FuyinFeeDetail.Where(a => a.feeListNo == id);
            List<FuyinFeeDetail> list = ds.ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].feeListNo != null)
                    list[i].feeListNo = list[i].feeListNo.Trim();
                }
                LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Report/Office/DFYFeeList.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("DFYFeeList", ds);
            localReport.DataSources.Add(reportDataSource);
            List<ReportParameter> parameterList = new List<ReportParameter>();
            parameterList.Add(new ReportParameter("name", name.ToString().Trim()));
            localReport.SetParameters(parameterList);
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
        public ActionResult AddDFYFei(FuyinFeeDetail fuyin, string id, long? id1, string action, int? page)        //添加打复印所需费用
        {
            ViewBag.date = DateTime.Now.ToString("yyyy-MM-dd");
            //if (id != "null")
            //{
            var image = db.BindUserAndImage.Where(a => a.userID == id).ToList();
            var fei = db.Charger.Where(a => a.seqNo == id).First();
            ViewBag.no = fei.seqNo;
            ViewBag.unitname = fei.unitName;
            if (image.Count == 0)
            {
                ViewData["button"] = true;
                ViewData["div"] = "display:none";
            }
            if (image.Count != 0)
            {
                if (fei.isCharge==true)
                {
                    ViewData["tongji"] = true;
                }

                ViewData["button"] = false;
                ViewData["div"] = "display:block";
                //ViewBag.image = image;
                ViewBag.result1 = JsonConvert.SerializeObject(image);
            }

            var image2 = db.BindUserAndImageDown.Where(a => a.userID == id).ToList();
            if (image2.Count == 0)
            {
                ViewData["button2"] = true;
                ViewData["div2"] = "display:none";
            }
            if (image2.Count != 0)
            {
                ViewData["button2"] = false;
                ViewData["div2"] = "display:block";
                ViewBag.result2 = JsonConvert.SerializeObject(image2);
            }
            //}
            //    else {
            //        var image = db.BindUserAndImage.Where(a => a.realuserID == id1).ToList();
            //var fei = db.Charger.Where(a => a.searchNo == id1).First();
            //ViewBag.no = fei.seqNo;
            //        ViewBag.unitname = fei.unitName;
            //        if (image.Count == 0)
            //        {
            //            ViewData["button"] = true;
            //            ViewData["div"] = "display:none";
            //        }
            //        if (image.Count != 0)
            //        {
            //            ViewData["button"] = false;
            //            ViewData["div"] = "display:block";
            //            //ViewBag.image = image;
            //            ViewBag.result1 = JsonConvert.SerializeObject(image);
            //        }
            //    }


            if (action == "打印")
            {
                var no = Request.Form["feeListNo"];
                var list = db.FuyinFeeDetail.Where(a => a.feeListNo == no);
                if (list.Count() == 0)
                {
                    return Content("<script >window.alert('请先保存收费清单!');window.history.back();</script>");
                }
                return RedirectToAction("printFeeList", new { id = no });
            }

            if (action == "保存")
            {
                string no = Request.Form["feeListNo"];
                var panduan = db.FuyinFeeDetail.Where(a => a.feeListNo == no);
                if (panduan.Count() == 0)
                {
                    var maxID = db.FuyinFeeDetail.Max(a => a.ID);
                    fuyin.ID = maxID + 1;
                    fuyin.searchNo = "0";
                    fuyin.feeListNo = no;
                    fuyin.dateCharged = DateTime.Parse(Request.Form["dateCharged"]);
                    var addfuyin = db.vw_charge.Where(a => a.seqNo == no).First();
                    Charger charge = new Charger();
                    charge.seqNo = no;
                    charge.searchNo = addfuyin.searchNo;
                    charge.fromDepartment = "5";
                    //var fei = db.Charger.Where(a => a.seqNo == id).First();
                    charge.itemName = fei.unitName + "打印费";
                    charge.totalExpense = decimal.Parse(fuyin.totalFee.ToString());
                    charge.unitName = fei.unitName;
                    charge.buildingArea = 0;
                    charge.chargeClassify = 4;
                    charge.@operator = "打印室";
                    charge.chargeTime = addfuyin.chargeTime;
                    charge.isCharge = false;
                    charge.theoryExpense = 0;
                    charge.whereTransfer = 1;
                    charge.centiCnt = 0;
                    charge.isBack = false;
                    var chargeid = db.Charger.Max(a => a.ID);
                    charge.ID = chargeid + 1;
                    db.Charger.Add(charge);
                    db.FuyinFeeDetail.Add(fuyin);
                    db.SaveChanges();
                }
                else {
                    var charge1 = db.Charger.Where(a => a.searchNo == id1).OrderByDescending(a=>a.ID).First();
                     panduan.First().A4TextFee = fuyin.A4TextFee;
                     panduan.First().A4TextCnt = fuyin.A4TextCnt;
                     panduan.First().A4PageCnt = fuyin.A4PageCnt;
                     panduan.First().A4DrawingFee = fuyin.A4DrawingFee;
                     panduan.First().A4DrawingCnt = fuyin.A4DrawingCnt;
                     panduan.First().A3TextFee = fuyin.A3TextFee;
                     panduan.First().A3TextCnt = fuyin.A3TextCnt;
                     panduan.First().A3PageCnt = fuyin.A3PageCnt;
                     panduan.First().A3DrawingFee = fuyin.A3DrawingFee;
                     panduan.First().A3DrawingCnt = fuyin.A3DrawingCnt;
                     panduan.First().A2PageCnt = fuyin.A2PageCnt;
                     panduan.First().A2DrawingFee = fuyin.A2DrawingFee;
                     panduan.First().A2DrawingCnt = fuyin.A2DrawingCnt;
                     panduan.First().A1PageCnt = fuyin.A1PageCnt;
                     panduan.First().A1DrawingFee = fuyin.A1DrawingFee;
                     panduan.First().A1DrawingCnt = fuyin.A1DrawingCnt;
                     panduan.First().A0PageCnt = fuyin.A0PageCnt;
                     panduan.First().A0DrawingFee = fuyin.A0DrawingFee;
                     panduan.First().A0DrawingCnt = fuyin.A0DrawingCnt;

                     panduan.First().A1AddPageCnt = fuyin.A1AddPageCnt;
                     panduan.First().A1AddDrawingFee = fuyin.A1AddDrawingFee;
                     panduan.First().A1AddDrawingCnt = fuyin.A1AddDrawingCnt;
                     panduan.First().A0AddPageCnt = fuyin.A0AddPageCnt;
                     panduan.First().A0AddDrawingFee = fuyin.A0AddDrawingFee;
                     panduan.First().A0AddDrawingCnt = fuyin.A0AddDrawingCnt;
                     panduan.First().totalFee = fuyin.totalFee;
                     panduan.First().theoryFee = fuyin.theoryFee;
                    charge1.totalExpense = decimal.Parse(fuyin.totalFee.ToString());
                    charge1.theoryExpense = decimal.Parse(fuyin.totalFee.ToString());
                    db.Entry(charge1).State = EntityState.Modified;
                    db.Entry(panduan.First()).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Content("<script >window.alert('保存成功!');window.history.back();</script>");
            }

            return View();
        }

        public ActionResult DownDFYFei(FuyinFeeDetail fuyin, long id)        //添加打复印所需费用
        {
            ViewBag.date = DateTime.Now.ToString("yyyy-MM-dd");
            var image2 = db.BindUserAndImageDown.Where(a => a.realuserID == id).ToList();
            var fei = db.BorrowRegistration.Where(a => a.ID == id).First();
            ViewBag.no = fei.borrowSeqNo;
            if (fei.singleOrDepart.Trim() == "个人")
            {
                ViewBag.unitname = fei.borrower;
            }
            else
            {
                ViewBag.unitname = fei.borrowUnit;
            }
            
            if (image2.Count == 0)
            {
                ViewData["button2"] = true;
                ViewData["div2"] = "display:none";
            }
            if (image2.Count != 0)
            {
                ViewData["button2"] = false;
                ViewData["div2"] = "display:block";
                ViewBag.result2 = JsonConvert.SerializeObject(image2);
            }
            return View();
        }
        public string TotalTongji(string feeListNo)
        {
            var model = from a in db.BindUserAndImage
                        where a.userID == feeListNo
                        orderby a.ID
                        select a;
            DataTable myTable = new DataTable();
            DataRow myDataRow;
            myTable.Columns.Add("flag", Type.GetType("System.String"));
            myTable.Columns.Add("A4DrawingCnt", Type.GetType("System.String"));
            myTable.Columns.Add("A4TextCnt", Type.GetType("System.String"));
            myTable.Columns.Add("A3DrawingCnt", Type.GetType("System.String"));
            myTable.Columns.Add("A3TextCnt", Type.GetType("System.String"));
            myTable.Columns.Add("A2DrawingCnt", Type.GetType("System.String"));
            myTable.Columns.Add("A1DrawingCnt", Type.GetType("System.String"));
            myTable.Columns.Add("A0DrawingCnt", Type.GetType("System.String"));
            myTable.Columns.Add("A1AddDrawingCnt", Type.GetType("System.String"));
            myTable.Columns.Add("A0AddDrawingCnt", Type.GetType("System.String"));
            myTable.Columns.Add("16DrawingCnt", Type.GetType("System.String"));
            myTable.Columns.Add("16TextCnt", Type.GetType("System.String"));
            if(model.Count()==0)
            {
                myDataRow = myTable.NewRow();
                myDataRow["flag"] = "1";
                myTable.Rows.Add(myDataRow);
                return JsonConvert.SerializeObject(myTable);
            }
            if (model.Count()!=0)
            {
                bool flag = false;
                foreach (var  dr in model)
                {
                    if (dr.imageSize== "" || dr.imageSize == null || dr.isWordOrPic == "" || dr.isWordOrPic == null)
                    {
                        flag = true; break;
                    }
                }
                if (flag == true)
                {
                    myDataRow = myTable.NewRow();
                    myDataRow["flag"] = "1";
                    myTable.Rows.Add(myDataRow);
                    return JsonConvert.SerializeObject(myTable);

                }
            }
            
          
            int a3 = 0, a4 = 0,a16 = 0;
            int adraw0 = 0, adraw1 = 0, adraw2 = 0, adraw3 = 0, adraw4 = 0, adraw5 = 0, adraw6 = 0, adraw16 = 0;
            foreach (var page in model)
            {
                string t = page.imageSize;
                if (page.isWordOrPic == "图纸")
                {

                    switch (t)
                    {
                        case "16":
                            adraw16++;
                            break;
                        case "A0":
                            adraw0++;
                            break;
                        case "A1":
                            adraw1++;
                            break;
                        case "A2":
                            adraw2++;
                            break;
                        case "A3":
                            adraw3++;
                            break;
                        case "A4":
                            adraw4++;
                            break;
                        case "A5":
                            adraw5++;
                            break;
                        case "A6":
                            adraw6++;
                            break;

                    }
                }
                else
                {
                    switch (t)
                    {
                        case "16":
                            a16++;
                            break;
                       
                        case "A3":
                            a3++;
                            break;
                        case "A4":
                            a4++;
                            break;
                      
                    }
                }
             }
            myDataRow = myTable.NewRow();
            myDataRow["flag"] = "2";
            myDataRow["16TextCnt"] = a16.ToString().Trim();
            myDataRow["16DrawingCnt"] = adraw16.ToString().Trim();
            myDataRow["A4TextCnt"] = (a4+a16).ToString().Trim();
            myDataRow["A4DrawingCnt"] = (adraw4 + adraw16).ToString().Trim();
            myDataRow["A3TextCnt"] = a3.ToString().Trim();
            myDataRow["A3DrawingCnt"] = adraw3.ToString().Trim();
            myDataRow["A2DrawingCnt"] = adraw2.ToString().Trim();
            myDataRow["A1DrawingCnt"] = adraw1.ToString().Trim();
            myDataRow["A0DrawingCnt"] = adraw0.ToString().Trim();
            myDataRow["A0AddDrawingCnt"] = adraw6.ToString().Trim();
            myDataRow["A1AddDrawingCnt"] = adraw5.ToString().Trim();
            myTable.Rows.Add(myDataRow);
            ViewBag.result1 = JsonConvert.SerializeObject(model);//数据转换成JSON后传给前台
            return JsonConvert.SerializeObject(myTable);

        }
        public ActionResult DaFuYinList(string action)      //打复印收费费用添加列表
        {
            ViewData["pagename"] = "Charge/DaFuYinList";

            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "缴费单位/个人", Value = "1"},
                  new SelectListItem { Text = "编号", Value = "3"}
            };
            ViewBag.dropdowmlist = new SelectList(list, "Value", "Text");
            List<SelectListItem> list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "全部科室", Value = "0"},
                  new SelectListItem { Text = "业务科", Value = "1"},
                  new SelectListItem { Text = "管理科", Value = "2"},
                  new SelectListItem { Text = "声像科", Value = "3"},
                  new SelectListItem { Text = "办公室", Value = "4"},
                  new SelectListItem { Text = "复印室", Value = "5"}
            };
            ViewBag.Department = new SelectList(list1, "Value", "Text");
            List<SelectListItem> list2 = new List<SelectListItem>
            {
                new SelectListItem { Text = "未收费", Value = "0"},
                new SelectListItem { Text = "已收费", Value = "1"},
            };
            ViewBag.ischarge = new SelectList(list2, "Value", "Text");
            if (action == "查询")
            {
                string n = Request.Form["dropdowmlist"];
                string m = Request.Form["search"];
                string a = Request.Form["Department"];
                string b = Request.Form["ischarge"];
                ViewBag.dropdowmlist = new SelectList(list, "Value", "Text",n);
                ViewBag.Department = new SelectList(list1, "Value", "Text",a);
                ViewBag.ischarge = new SelectList(list2, "Value", "Text",b);
                ViewBag.search = m;
                if (a == "0")
                {
                    if (n == "1")
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.unitName.Contains(m)
                                         where ad.isCharge == true
                                         where ad.whereTransfer == 0
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new { p.seqNo, p.searchNo, p.unitName, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.feiyong = chaxundistinct;
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.unitName.Contains(m)
                                         where ad.isCharge == false
                                         where ad.whereTransfer == 0
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new { p.seqNo, p.searchNo, p.unitName, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.feiyong = chaxundistinct;
                        }
                    }
                    if (n == "")
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.isCharge == true
                                         where ad.whereTransfer == 0
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new {p.seqNo, p.searchNo, p.unitName, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.feiyong = chaxundistinct;
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.isCharge == false
                                         where ad.whereTransfer == 0
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new {p.seqNo, p.searchNo, p.unitName, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.feiyong = chaxundistinct;
                        }
                    }
                    if (n == "3")
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.seqNo.Contains(m)
                                         where ad.isCharge == true
                                         where ad.whereTransfer == 0
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new {p.seqNo, p.searchNo, p.unitName, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.feiyong = chaxundistinct;
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.seqNo.Contains(m)
                                         where ad.isCharge == false
                                         where ad.whereTransfer == 0
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new { p.seqNo, p.searchNo, p.unitName, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.feiyong = chaxundistinct;
                        }
                    }
                }
                else   //其他部门
                {
                    if (n == "1")
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.unitName.Contains(m)
                                         where ad.isCharge == true
                                         where ad.fromDepartment == a
                                         where ad.whereTransfer == 0
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new {p.seqNo, p.searchNo, p.unitName, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.feiyong = chaxundistinct;
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.unitName.Contains(m)
                                         where ad.isCharge == false
                                         where ad.fromDepartment == a
                                         where ad.whereTransfer == 0
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new {p.seqNo, p.searchNo, p.unitName, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.feiyong = chaxundistinct;
                        }
                    }
                    if (n == "")
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.isCharge == true
                                         where ad.whereTransfer == 0
                                         where ad.fromDepartment == a
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new { p.seqNo, p.searchNo, p.unitName, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.feiyong = chaxundistinct;
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.isCharge == false
                                         where ad.whereTransfer == 0
                                         where ad.fromDepartment == a
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new {p.seqNo, p.searchNo, p.unitName, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.feiyong = chaxundistinct;
                        }
                    }
                    if (n == "3")
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.seqNo.Contains(m)
                                         where ad.isCharge == true
                                         where ad.fromDepartment == a
                                         where ad.whereTransfer == 0
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new { p.seqNo, p.searchNo, p.unitName, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.feiyong = chaxundistinct;
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.seqNo.Contains(m)
                                         where ad.isCharge == false
                                         where ad.fromDepartment == a
                                         where ad.whereTransfer ==0
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new {p.seqNo, p.searchNo, p.unitName, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.feiyong = chaxundistinct;
                        }
                    }
                }
                return View();
            }
            var feiyongchaxun = db.vw_charge.Where(ab => ab.isCharge == false).Where(ab => ab.whereTransfer == 0).Select(p => new { p.seqNo, p.searchNo, p.unitName, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
            ViewBag.feiyong = feiyongchaxun;
            return View();

        }
        public ActionResult DownList(string action) {
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "借阅单位", Value = "1"},
                new SelectListItem { Text = "个人", Value = "2"},
                new SelectListItem { Text = "编号", Value = "3"}
            };
            ViewBag.dropdowmlist = new SelectList(list, "Value", "Text");
            if (action == "查询")
            {
                string n = Request.Form["dropdowmlist"];
                string m = Request.Form["search"];
                ViewBag.dropdowmlist = new SelectList(list, "Value", "Text", n);
                ViewBag.search = m;
                if (n == "")
                {
                    var chaxun = from ad in db.BorrowRegistration
                                 where ad.consultVolumeCount == "Down"
                                 orderby ad.borrowSeqNo descending
                                 select ad;
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                }
                if (n == "1")
                {
                    var chaxun = from ad in db.BorrowRegistration
                                 where ad.consultVolumeCount == "Down"
                                 where ad.borrowUnit.Contains(m)
                                 orderby ad.borrowSeqNo descending
                                 select ad;
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                }
                if (n == "2")
                {
                    var chaxun = from ad in db.BorrowRegistration
                                 where ad.consultVolumeCount == "Down"
                                 where ad.borrower.Contains(m)
                                 orderby ad.borrowSeqNo descending
                                 select ad;
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                }
                if (n == "3")
                {
                    var chaxun = from ad in db.BorrowRegistration
                                 where ad.consultVolumeCount == "Down"
                                 where ad.borrowSeqNo.Contains(m)
                                 orderby ad.borrowSeqNo descending
                                 select ad;
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                }
                return View();
            }
            ViewBag.result = JsonConvert.SerializeObject( db.BorrowRegistration.Where(ab => ab.consultVolumeCount == "Down").Distinct().OrderByDescending(ab => ab.borrowSeqNo));
            return View();
        }
        public ActionResult DaFuYinEdit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            vw_charge charge = db.vw_charge.Find(id);
            var seqno = charge.seqNo;
            FuyinFeeDetail fuyinfeidetail = db.FuyinFeeDetail.Where(a => a.feeListNo == seqno).First();
            if (fuyinfeidetail == null)
            {
                return HttpNotFound();
            }
            var UserID = User.Identity.GetUserId();
            var department = cb.AspNetUsers.Find(UserID).DepartmentName;
            if (charge.centiCnt != -1)
            {
                if (charge.text.Trim() != department)
                {
                    return Content("<script >window.alert('您没有权限修改此项费用!');window.history.back();</script>");
                }

                else if (charge.isCharge == true)
                {
                    return Content("<script>alert('此费用已经收取，不能进行修改 !');window.history.back();</script>");
                }
            
                else
                {
                    ViewBag.name = charge.unitName;
                    ViewBag.no = fuyinfeidetail.feeListNo;
                    ViewBag.date = fuyinfeidetail.dateCharged;
                    ViewBag.id = fuyinfeidetail.ID;
                }
            }


            return View(fuyinfeidetail);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DaFuYinEdit([Bind(Include = " ID,feeListNo,A4PageCnt,A4DrawingCnt,A4TextCnt,A4DrawingUnitPrice,A4TextUnitPrice,A4DrawingFee,A4TextFee,A3PageCnt,A3DrawingCnt,A3TextCnt,A3DrawingUnitPrice,A3TextUnitPrice ,A3DrawingFee,A3TextFee,A2PageCnt,A2DrawingCnt,A2DrawingUnitPrice,A2DrawingFee ,A1PageCnt,A1DrawingCnt,A1DrawingUnitPrice,A1DrawingFee,A0PageCnt,A0DrawingCnt,A0DrawingUnitPrice,A0DrawingFee,A1AddPageCnt ,A1AddDrawingCnt ,A1AddDrawingUnitPrice,A1AddDrawingFee,A0AddPageCnt ,A0AddDrawingCnt,A0AddDrawingUnitPrice,A0AddDrawingFee,totalFee,dateCharged,unitCharged,theoryFee,searchNo")] FuyinFeeDetail fuyin,int ID,string action)
        {
            if (ModelState.IsValid)
            {
                if (action == "修改")
                {
                    var no= Request.Form["feeListNo"];
                    var charge = db.Charger.Where(a => a.seqNo == no).OrderByDescending(a => a.ID).First();
                    charge.unitName = Request.Form["name"];
                    charge.itemName = Request.Form["name"] + "打印费";
                    charge.totalExpense= decimal.Parse( Request.Form["totalFee"]);
                    charge.theoryExpense = decimal.Parse(Request.Form["theoryFee"]);
                    var fuyin1 = db.FuyinFeeDetail.Where(a => a.ID == ID).First();
                    fuyin1.feeListNo = Request.Form["feeListNo"];
                    fuyin1.searchNo = Request.Form["feeListNo"];
                    fuyin1.A0AddDrawingCnt = fuyin.A0AddDrawingCnt;
                    fuyin1.A0AddDrawingFee = fuyin.A0AddDrawingFee;
                    fuyin1.A0AddDrawingUnitPrice = fuyin.A0AddDrawingUnitPrice;
                    fuyin1.A0AddPageCnt = fuyin.A0AddPageCnt;
                    fuyin1.A0DrawingCnt = fuyin.A0DrawingCnt;
                    fuyin1.A0DrawingFee = fuyin.A0DrawingFee;
                    fuyin1.A0DrawingUnitPrice = fuyin.A0DrawingUnitPrice;
                    fuyin1.A0PageCnt = fuyin.A0PageCnt;
                    fuyin1.A1AddDrawingCnt = fuyin.A1AddDrawingCnt;
                    fuyin1.A1AddDrawingFee = fuyin.A1AddDrawingFee;
                    fuyin1.A1AddDrawingUnitPrice = fuyin.A1AddDrawingUnitPrice;
                    fuyin1.A1AddPageCnt = fuyin.A1AddPageCnt;
                    fuyin1.A1DrawingCnt = fuyin.A1DrawingCnt;
                    fuyin1.A1DrawingFee = fuyin.A1DrawingFee;
                    fuyin1.A1DrawingUnitPrice = fuyin.A1DrawingUnitPrice;
                    fuyin1.A1PageCnt = fuyin.A1PageCnt;
                    fuyin1.A2DrawingCnt = fuyin.A2DrawingCnt;
                    fuyin1.A2DrawingFee = fuyin.A2DrawingFee;
                    fuyin1.A2DrawingUnitPrice = fuyin.A2DrawingUnitPrice;
                    fuyin1.A2PageCnt = fuyin.A2PageCnt;
                    fuyin1.A3DrawingCnt = fuyin.A3DrawingCnt;
                    fuyin1.A3DrawingFee = fuyin.A3DrawingFee;
                    fuyin1.A3DrawingUnitPrice = fuyin.A3DrawingUnitPrice;
                    fuyin1.A3PageCnt = fuyin.A3PageCnt;
                    fuyin1.A3TextCnt = fuyin.A3TextCnt;
                    fuyin1.A3TextFee = fuyin.A3TextFee;
                    fuyin1.A3TextUnitPrice = fuyin.A3TextUnitPrice;
                    fuyin1.A4DrawingCnt = fuyin.A4DrawingCnt;
                    fuyin1.A4DrawingFee = fuyin.A4DrawingFee;
                    fuyin1.A4DrawingUnitPrice = fuyin.A4DrawingUnitPrice;
                    fuyin1.A4PageCnt = fuyin.A4PageCnt;
                    fuyin1.A4TextCnt = fuyin.A4TextCnt;
                    fuyin1.A4TextFee = fuyin.A4TextFee;
                    fuyin1.A4TextUnitPrice = fuyin.A4TextUnitPrice;
                    fuyin1.dateCharged = fuyin.dateCharged;
                    fuyin1.theoryFee = fuyin.theoryFee;
                    fuyin1.totalFee = fuyin.totalFee;
                    fuyin1.unitCharged = fuyin.unitCharged;
                    fuyin1.ID = fuyin.ID;
                    db.Entry(charge).State = EntityState.Modified;
                    db.Entry(fuyin1).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("<script>alert('修改成功！');window.location.href='./DaFuYinFeiYongChaXun'</script>");
                }
                if (action == "返回")
                {
                    return RedirectToAction("DaFuYinFeiYongChaXun");
                }
            }
            return View(fuyin);
        }
        public ActionResult report(string id, string type = "PDF")   //打印收费明细
        {
            LocalReport localReport = new LocalReport();
            var ds1 = db.vw_charge.Where(ad => ad.seqNo == id);
            List<vw_charge> list = ds1.ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].text != null)
                    list[i].text = list[i].text.Trim();
            }
            localReport.ReportPath = Server.MapPath("~/Report/Office/ShouFeiList.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("ShouFeiList", ds1);
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
        public ActionResult Back(string id, string action)
        {
            ViewBag.seq = id;
            if (action == "驳回")
            {
                string result=Request.Form["result"];
                var chargeOK = db.Charger.Where(a => a.seqNo == id);
                var chargeOK1 = chargeOK.ToList();
                for (int i = 0; i < chargeOK1.Count(); i++)
                {
                    chargeOK1[i].isBack = true;
                    chargeOK1[i].backNote = result;
                    db.Entry(chargeOK1[i]).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Content("<script>alert('驳回成功！');window.location.href='../CaiWuShouFeiList'</script>");
            }
            if (action == "取消")
            {
                return RedirectToAction("JieSuan",new { id=id});
            }
            
            return View();
        }
        public ActionResult JieSuan(string id,string action)      //费用结算
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var charge = db.vw_charge.Where(a => a.seqNo == id).ToList();
            decimal? total = 0;
            for (int i = 0; i < charge.Count(); i++)
            {
                total =total+ charge[i].totalExpense;
            }
            ViewBag.total = total;
            ViewBag.unitName = charge.ToList().First().unitName;
            ViewBag.seqNo = charge.ToList().First().seqNo;
            ViewBag.remarks = charge.ToList().First().remarks;
            if (charge == null)
            {
                return HttpNotFound();
            }
            if (action == "确认收费")
            {
                if (ModelState.IsValid)
                {
                    string NO = Request.Form["seqNo"];
                    var chargeOK = db.Charger.Where(a => a.seqNo == NO);
                    var chargeOK1 = chargeOK.ToList();
                    for (int i = 0; i < chargeOK1.Count(); i++)
                    {
                        chargeOK1[i].isCharge = true;
                        db.Entry(chargeOK1[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Content("<script>alert('收费成功！');window.location.href='CaiWuShouFeiList'</script>");
                }
            }
            if (action == "驳回")
            {
                if (ModelState.IsValid)
                {
                    string NO = Request.Form["seqNo"];
                    var chargeOK = db.Charger.Where(a => a.seqNo == NO);
                    var chargeOK1 = chargeOK.ToList();
                    for (int i = 0; i < chargeOK1.Count(); i++)
                    {
                        chargeOK1[i].isBack = true;
                        db.Entry(chargeOK1[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Back",new { id=NO});
                }
            }
            if (action == "打印")
            {
                return RedirectToAction("report", new { id = id });
            }
            if (action == "返回")
            {
                return RedirectToAction("CaiWuShouFeiList");
            }
            return View(charge);
        }
        public ActionResult CaiWuShouFeiList(string action, int? page)      //财务收费列表
        {
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "缴费单位/个人", Value = "1"},
                  new SelectListItem { Text = "编号", Value = "3"}
            };
            ViewBag.dropdowmlist = new SelectList(list, "Value", "Text");
            List<SelectListItem> list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "全部科室", Value = "0"},
                  new SelectListItem { Text = "业务科", Value = "1"},
                  new SelectListItem { Text = "管理科", Value = "2"},
                  new SelectListItem { Text = "声像科", Value = "3"},
                  new SelectListItem { Text = "办公室", Value = "4"},
                  new SelectListItem { Text = "复印室", Value = "5"}
            };
            ViewBag.Department = new SelectList(list1, "Value", "Text");
            List<SelectListItem> list2 = new List<SelectListItem>
            {
                new SelectListItem { Text = "未收费", Value = "0"},
                new SelectListItem { Text = "已收费", Value = "1"},
            };
            ViewBag.ischarge = new SelectList(list2, "Value", "Text");
            ViewBag.Search = Request.Form["search"];
            if (action == "查询")
            {
                string n = Request.Form["dropdowmlist"];
                string m = Request.Form["search"];
                string a = Request.Form["Department"];
                string b = Request.Form["ischarge"];
                ViewBag.dropdowmlist = new SelectList(list, "Value", "Text", n);
                ViewBag.Department = new SelectList(list1, "Value", "Text", a);
                ViewBag.ischarge = new SelectList(list2, "Value", "Text", b);
                if (m == "")
                {
                    if (a == "0")
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.isCharge == true
                                         where ad.whereTransfer == 1
                                         //where ad.searchNo != 0
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                            return View();
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.isCharge == false
                                         where ad.whereTransfer == 1
                                         //where ad.searchNo != 0
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                            return View();
                        }
                    }
                    else
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.isCharge == true
                                         where ad.whereTransfer == 1
                                         //where ad.searchNo != 0
                                         where ad.fromDepartment == a
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                            return View();
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.isCharge == false
                                         where ad.whereTransfer == 1
                                         //where ad.searchNo != 0
                                         where ad.fromDepartment == a
                                         select ad;
                            var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                            ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                            return View();
                        }
                    }
                }
                else
                {
                    if (a == "0")
                    {
                        if (n == "1")
                        {
                            if (b == "1")
                            {
                                var chaxun = from ad in db.vw_charge
                                             where ad.unitName.Contains(m)
                                             where ad.isCharge == true
                                             where ad.whereTransfer == 1
                                             //where ad.searchNo != 0
                                             select ad;
                                var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                                ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                                return View();
                            }
                            else
                            {
                                var chaxun = from ad in db.vw_charge
                                             where ad.unitName.Contains(m)
                                             where ad.isCharge == false
                                             where ad.whereTransfer == 1
                                             //where ad.searchNo != 0
                                             select ad;
                                var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                                ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                                return View();
                            }
                        }
                        if (n == "0")
                        {
                            if (b == "1")
                            {
                                var chaxun = from ad in db.vw_charge
                                             where ad.isCharge == true
                                             where ad.whereTransfer == 1
                                             //where ad.searchNo != 0
                                             select ad;
                                var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                                ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                                return View();
                            }
                            else
                            {
                                var chaxun = from ad in db.vw_charge
                                             where ad.isCharge == false
                                             where ad.whereTransfer == 1
                                             //where ad.searchNo != 0
                                             select ad;
                                var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                                ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                                return View();
                            }
                        }
                        else
                        {
                            if (b == "1")
                            {
                                var chaxun = from ad in db.vw_charge
                                             where ad.seqNo.Contains(m)
                                             where ad.isCharge == true
                                             where ad.whereTransfer == 1
                                             //where ad.searchNo != 0
                                             select ad;
                                var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                                ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                                return View();
                            }
                            else
                            {
                                var chaxun = from ad in db.vw_charge
                                             where ad.seqNo.Contains(m)
                                             where ad.isCharge == false
                                             where ad.whereTransfer == 1
                                             //where ad.searchNo != 0
                                             select ad;
                                var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                                ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                                return View();
                            }
                        }
                    }
                    else   //其他部门
                    {
                        if (n == "1")
                        {
                            if (b == "1")
                            {
                                var chaxun = from ad in db.vw_charge
                                             where ad.unitName.Contains(m)
                                             where ad.isCharge == true
                                             where ad.fromDepartment == a
                                             where ad.whereTransfer == 1
                                             select ad;
                                var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                                ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                                return View();
                            }
                            else
                            {
                                var chaxun = from ad in db.vw_charge
                                             where ad.unitName.Contains(m)
                                             where ad.isCharge == false
                                             where ad.fromDepartment == a
                                             where ad.whereTransfer == 1
                                             select ad;
                                var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                                ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                                return View();
                            }
                        }
                        if (n == "0")
                        {
                            if (b == "1")
                            {
                                var chaxun = from ad in db.vw_charge
                                             where ad.isCharge == true
                                             where ad.whereTransfer == 1
                                             select ad;
                                var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                                ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                                return View();
                            }
                            else
                            {
                                var chaxun = from ad in db.vw_charge
                                             where ad.isCharge == false
                                             where ad.whereTransfer == 1
                                             select ad;
                                var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                                ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                                return View();
                            }
                        }
                        else
                        {
                            if (b == "1")
                            {
                                var chaxun = from ad in db.vw_charge
                                             where ad.seqNo.Contains(m)
                                             where ad.isCharge == true
                                             where ad.fromDepartment == a
                                             where ad.whereTransfer == 1
                                             select ad;
                                var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                                ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                                return View();
                            }
                            else
                            {
                                var chaxun = from ad in db.vw_charge
                                             where ad.seqNo.Contains(m)
                                             where ad.isCharge == false
                                             where ad.fromDepartment == a
                                             where ad.whereTransfer == 1
                                             select ad;
                                var chaxundistinct = chaxun.Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
                                ViewBag.result = JsonConvert.SerializeObject(chaxundistinct);
                                return View();
                            }
                        }
                    }
                }
            }
            var feiyongchaxun = db.vw_charge.Where(ab => ab.isCharge == false).Where(ab=>ab.isBack!=true).Where(ab => ab.whereTransfer == 1).Select(p => new { p.text, p.seqNo, p.unitName, p.totalExpense, p.isCharge, p.isBack, p.backNote }).Distinct().OrderByDescending(ab => ab.seqNo);
            ViewBag.result = JsonConvert.SerializeObject(feiyongchaxun);
            return View();
        }
        public ActionResult OtherFeiYongChaXun(string action)      //其他费用查询
        {
            ViewData["pagename"] = "Charge/OtherFeiYongChaXun";
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "事项名称", Value = "0"},
                new SelectListItem { Text = "缴费单位", Value = "1"},
                 new SelectListItem { Text = "缴费日期（XXXX-XX-XX或XXXX.XX.XX）", Value = "2"},
                  new SelectListItem { Text = "编号", Value = "3"}
            };
            ViewBag.dropdowmlist = new SelectList(list, "Value", "Text");

            if (action == "查询")
            {
                string n = Request.Form["dropdowmlist"];
                string m = Request.Form["search"];
                if (m != "")
                {
                    ViewBag.dropdowmlist = new SelectList(list, "Value", "Text", n);
                    ViewBag.search = m;
                    if (n == "0")
                    {
                        var chaxun = from ad in db.vw_charge
                                     where ad.itemName.Contains(m)
                                     where ad.centiCnt == -1
                                     orderby ad.ID descending
                                     select ad;

                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "1")
                    {
                        var chaxun = from ad in db.vw_charge
                                     where ad.unitName.Contains(m)
                                     where ad.centiCnt == -1
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "2")
                    {
                        DateTime time = DateTime.Parse(m);
                        var chaxun = from ad in db.vw_charge
                                     where ad.chargeTime == time
                                     where ad.centiCnt == -1
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "3")
                    {

                        var chaxun = from ad in db.vw_charge
                                     where ad.seqNo.Contains(m)
                                     where ad.centiCnt == -1
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                }
            }
            var feiyongchaxun = db.vw_charge.Where(a => a.centiCnt == -1).OrderByDescending(a => a.ID);
                ViewBag.result = JsonConvert.SerializeObject(feiyongchaxun);
                return View();
            }
        public ActionResult DaFuYinFeiYongChaXun(string action)      //打复印费用查询
        {
            ViewData["pagename"] = "Charge/DaFuYinFeiYongChaXun";

            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "事项名称", Value = "0"},
                new SelectListItem { Text = "缴费单位", Value = "1"},
                 new SelectListItem { Text = "缴费日期", Value = "2"},
                  new SelectListItem { Text = "编号", Value = "3"}
            };
            ViewBag.dropdowmlist = new SelectList(list, "Value", "Text");

            if (action == "查询")
            {
                string n = Request.Form["dropdowmlist"];
                string m = Request.Form["search"];

                if (m != "")
                {
                    ViewBag.dropdowmlist = new SelectList(list, "Value", "Text",n);
                    ViewBag.search = m;
                    if (n == "0")
                    {
                        var chaxun = from ad in db.vw_charge
                                     where ad.itemName.Contains(m)
                                     where ad.fromDepartment == "5"
                                     where ad.centiCnt != -1
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "1")
                    {
                        var chaxun = from ad in db.vw_charge
                                     where ad.unitName.Contains(m)
                                     where ad.fromDepartment == "5"
                                     where ad.centiCnt != -1
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "2")
                    {
                        DateTime time = DateTime.Parse(m);
                        var chaxun = from ad in db.vw_charge
                                     where ad.chargeTime == time
                                     where ad.fromDepartment == "5"
                                     where ad.centiCnt != -1
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "3")
                    {

                        var chaxun = from ad in db.vw_charge
                                     where ad.seqNo.Contains(m)
                                     where ad.fromDepartment == "5"
                                     where ad.centiCnt != -1
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                }
            }
            var feiyongchaxun = db.vw_charge.Where(a => a.fromDepartment == "5").Where(a => a.centiCnt != -1).OrderByDescending(a => a.ID);
            ViewBag.result = JsonConvert.SerializeObject(feiyongchaxun);
            return View();

        }
        public ActionResult HeJuFeiYongEdit(long? id)
        {
            ArchivesContainer archivesContainer = ab.ArchivesContainer.Find(id);
            var charge = db.Charger.Where(a => a.searchNo == id).First();
            ViewBag.total = charge.totalExpense;
            if (charge.isCharge == true)
            {
                Response.Write("<script>alert('此费用已经收取，不能进行修改 !');window.history.back();</script>");
            }

            ViewBag.Selected = new SelectList(db.DepartmentCode, "value", "Text", charge.fromDepartment);
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
            var charge = from ad in db.Charger
                         where ad.searchNo == id
                         select ad;
            var charge1 = charge.First();
            ViewBag.total = charge1.totalExpense;
            ViewBag.Selected = new SelectList(db.DepartmentCode, "value", "Text", charge1.fromDepartment);
            List<SelectListItem> list3 = new List<SelectListItem> {
                new SelectListItem { Text = "转向财务科", Value = "1"},
                new SelectListItem { Text = "转向复印室", Value = "0"},
            };

            ViewBag.Selected1 = new SelectList(list3, "Value", "Text", charge1.whereTransfer);
            //Charger charger = new Charger();
            long max_chargerID = db.Charger.Max(d => d.ID);
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
                        var list1 = from ad in ab.ArchivesContainer
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
        public ActionResult HeJuFeiYongChaXun(string action)      //盒具费用查询
        {
            ViewData["pagename"] = "Charge/HeJuFeiYongChaXun";
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "事项名称", Value = "0"},
                new SelectListItem { Text = "缴费单位", Value = "1"},
                 new SelectListItem { Text = "缴费日期", Value = "2"},
                  new SelectListItem { Text = "编号", Value = "3"}
            };
            ViewBag.dropdowmlist = new SelectList(list, "Value", "Text");
            if (action == "查询")
            {
                string n = Request.Form["dropdowmlist"];
                string m = Request.Form["search"];
                if (m != "")
                {
                    ViewBag.dropdowmlist = new SelectList(list, "Value", "Text", n);
                    ViewBag.search = m;
                    if (n == "0")
                    {
                        var chaxun = from ad in db.vw_charge
                                     where ad.itemName.Contains(m)
                                     where ad.fromDepartment == "4"
                                     where ad.centiCnt != -1
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "1")
                    {
                        var chaxun = from ad in db.vw_charge
                                     where ad.unitName.Contains(m)
                                     where ad.fromDepartment == "4"
                                     where ad.centiCnt != -1
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "2")
                    {
                        DateTime time = DateTime.Parse(m);
                        var chaxun = from ad in db.vw_charge
                                     where ad.chargeTime == time
                                     where ad.fromDepartment == "4"
                                     where ad.centiCnt != -1
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                    if (n == "3")
                    {

                        var chaxun = from ad in db.vw_charge
                                     where ad.seqNo.Contains(m)
                                     where ad.fromDepartment == "4"
                                     where ad.centiCnt != -1
                                     orderby ad.ID descending
                                     select ad;
                        ViewBag.result = JsonConvert.SerializeObject(chaxun);
                        return View();
                    }
                }
            }

            var feiyongchaxun = db.vw_charge.Where(a => a.fromDepartment == "4").Where(a => a.centiCnt != -1).OrderByDescending(a => a.ID);
                ViewBag.result = JsonConvert.SerializeObject(feiyongchaxun);
                return View();
            }
        //public ActionResult JieYueFeiYongChaXun(string action, int? page)      //借阅收费查询
        //{
        //    List<SelectListItem> list = new List<SelectListItem>
        //    {
        //        new SelectListItem { Text = "事项名称", Value = "0"},
        //        new SelectListItem { Text = "缴费单位", Value = "1"},
        //         new SelectListItem { Text = "缴费日期", Value = "2"},
        //          new SelectListItem { Text = "编号", Value = "3"}
        //    };
        //    ViewBag.dropdowmlist = new SelectList(list, "Value", "Text");
        //    int pageSize = 30;
        //    int pageNumber = (page ?? 1);

        //    if (action == "查询")
        //    {
        //        string n = Request.Form["dropdowmlist"];
        //        string m = Request.Form["search"];
        //        if (n == "0")
        //        {
        //            var chaxun = from ad in db.vw_charge
        //                         where ad.itemName.Contains(m)
        //                         where ad.fromDepartment == "2"
        //                         where ad.centiCnt != -1
        //                         orderby ad.ID descending
        //                         select ad;
        //            return View(chaxun.ToPagedList(pageNumber, pageSize));
        //        }
        //        if (n == "1")
        //        {
        //            var chaxun = from ad in db.vw_charge
        //                         where ad.unitName.Contains(m)
        //                         where ad.fromDepartment == "2"
        //                         where ad.centiCnt != -1
        //                         orderby ad.ID descending
        //                         select ad;
        //            return View(chaxun.ToPagedList(pageNumber, pageSize));
        //        }
        //        if (n == "2")
        //        {
        //            DateTime time = DateTime.Parse(m);
        //            var chaxun = from ad in db.vw_charge
        //                         where ad.chargeTime == time
        //                         where ad.fromDepartment == "2"
        //                         where ad.centiCnt != -1
        //                         orderby ad.ID descending
        //                         select ad;
        //            return View(chaxun.ToPagedList(pageNumber, pageSize));
        //        }
        //        if (n == "3")
        //        {

        //            var chaxun = from ad in db.vw_charge
        //                         where ad.seqNo.Contains(m)
        //                         where ad.fromDepartment == "2"
        //                         where ad.centiCnt != -1
        //                         orderby ad.ID descending
        //                         select ad;
        //            return View(chaxun.ToPagedList(pageNumber, pageSize));
        //        }
        //    }

        //    var feiyongchaxun = db.vw_charge.Where(a => a.fromDepartment == "2").OrderByDescending(a => a.ID);
        //    return View(feiyongchaxun.ToPagedList(pageNumber, pageSize));
        //}
        public ActionResult ShengXiangFeiYongChaXun(string action, int? page)      //声像科费用查询
        {
            ViewData["pagename"] = "Charge/ShengXiangFeiYongChaXun";
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "事项名称", Value = "0"},
                new SelectListItem { Text = "缴费单位", Value = "1"},
                 new SelectListItem { Text = "缴费日期", Value = "2"},
                  new SelectListItem { Text = "编号", Value = "3"}
            };
            ViewBag.dropdowmlist = new SelectList(list, "Value", "Text");
            

            if (action == "查询")
            {
                string n = Request.Form["dropdowmlist"];
                string m = Request.Form["search"];
                ViewBag.dropdowmlist = new SelectList(list, "Value", "Text", n);
                ViewBag.search = m;
                if (n == "0")
                {
                    var chaxun = from ad in db.vw_charge
                                 where ad.itemName.Contains(m)
                                 where ad.fromDepartment == "3"
                                 where ad.centiCnt != -1
                                 orderby ad.ID descending
                                 select ad;
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "1")
                {
                    var chaxun = from ad in db.vw_charge
                                 where ad.unitName.Contains(m)
                                 where ad.fromDepartment == "3"
                                 where ad.centiCnt != -1
                                 orderby ad.ID descending
                                 select ad;
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "2")
                {
                    DateTime time = DateTime.Parse(m);
                    var chaxun = from ad in db.vw_charge
                                 where ad.chargeTime == time
                                 where ad.fromDepartment == "3"
                                 where ad.centiCnt != -1
                                 orderby ad.ID descending
                                 select ad;
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "3")
                {

                    var chaxun = from ad in db.vw_charge
                                 where ad.seqNo.Contains(m)
                                 where ad.fromDepartment == "3"
                                 where ad.centiCnt != -1
                                 orderby ad.ID descending
                                 select ad;
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
            }

            var feiyongchaxun = db.vw_charge.Where(a => a.fromDepartment == "3").Where(a => a.centiCnt != -1).OrderByDescending(a => a.ID);
            ViewBag.result = JsonConvert.SerializeObject(feiyongchaxun);
            return View();
        }
        public ActionResult YeWuFeiYongChaXun(string action, int? page)      //业务科费用查询
        {
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "事项名称", Value = "0"},
                new SelectListItem { Text = "缴费单位", Value = "1"},
                 new SelectListItem { Text = "缴费日期", Value = "2"},
                  new SelectListItem { Text = "编号", Value = "3"}
            };
            ViewBag.dropdowmlist = new SelectList(list, "Value", "Text");
            

            if (action == "查询")
            {
                string n = Request.Form["dropdowmlist"];
                string m = Request.Form["search"];
                ViewBag.dropdowmlist = new SelectList(list, "Value", "Text",n);
                ViewBag.search = m;
                if (n == "0")
                {
                    var chaxun = from ad in db.vw_charge
                                 where ad.itemName.Contains(m)
                                 where ad.fromDepartment == "1"
                                 where ad.centiCnt != -1
                                 orderby ad.ID descending
                                 select ad;
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "1")
                {
                    var chaxun = from ad in db.vw_charge
                                 where ad.unitName.Contains(m)
                                 where ad.fromDepartment == "1"
                                 where ad.centiCnt != -1
                                 orderby ad.ID descending
                                 select ad;
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "2")
                {
                    DateTime time = DateTime.Parse(m);
                    var chaxun = from ad in db.vw_charge
                                 where ad.chargeTime == time
                                 where ad.fromDepartment == "1"
                                 where ad.centiCnt != -1
                                 orderby ad.ID descending
                                 select ad;
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "3")
                {

                    var chaxun = from ad in db.vw_charge
                                 where ad.seqNo.Contains(m)
                                 where ad.fromDepartment == "1"
                                 where ad.centiCnt != -1
                                 orderby ad.ID descending
                                 select ad;
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
            }

            var feiyongchaxun = db.vw_charge.Where(a => a.fromDepartment == "1").Where(a=>a.centiCnt!=-1).OrderByDescending(a => a.ID);
            ViewBag.result = JsonConvert.SerializeObject(feiyongchaxun);
            return View();
        }
        public JsonResult name()//jsy动态框方法
        {
            var list = from bb in db.Charger.ToList()
                       where bb.fromDepartment == "5 "
                       where bb.itemName.Length>6
                       orderby bb.ID descending
                       select new
                       {
                           name = bb.itemName,
                           ID=bb.ID
                       };
            list = list.Take(40);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public int GetMaxId()
        {

            var id = from a in db.Charger
                     orderby a.ID descending
                     select a;
            long obj = id.First().ID + 1;
            if (id.First() == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        //public ActionResult yewujieyue(string action)
        //{
        //    ViewData["pagename"] = "Charge/Create";
        //    long max_id = db.Charger.Max(d => d.ID);
        //    ViewBag.ID = max_id + 1;
        //    var people = User.Identity.GetUserId();
        //    var department = cb.AspNetUsers.Where(a => a.Id == people).First().DepartmentName;
        //    var departmentid = db.DepartmentCode.Where(a => a.text.Trim() == department).First().value;
        //    ViewBag.fromDepartment = new SelectList(db.DepartmentCode, "value", "text", departmentid);
        //    ViewBag.person = new SelectList(cb.AspNetUsers, "username", "username");
        //    ViewBag.chargeClassify = new SelectList(db.ChargeType, "value", "text");
        //    ViewBag.date = DateTime.Now.ToString("yyyy-MM-dd");
        //    var user = cb.AspNetUsers.Find(people);//获取当前用户,20170901by周林，打印室要默认费用
           
        //        ViewBag.fromDepartment = new SelectList(db.DepartmentCode, "value", "text", departmentid);
        //        ViewBag.person = new SelectList(cb.AspNetUsers, "username", "username", user.UserName);
        //    List<SelectListItem> list1 = new List<SelectListItem> {
        //        new SelectListItem { Text = "转向财务科", Value = "1"},
        //        new SelectListItem { Text = "转向复印室", Value = "0"},
        //    };
        //    ViewBag.whereTransfer = new SelectList(list1, "Value", "Text");
            
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult yewujieyue([Bind(Include = "itemName,chargeExtra,searchNo,totalExpense,fromDepartment,unitName,buildingArea,chargeClassify,chargeDetail,operator,charger,chargeTime,isCharge,remarks,seqNo,theoryExpense,whereTransfer,isBack,centiCnt,backNote")] Charger charge, string action)
        {
            ViewData["pagename"] = "Charge/Create";
            long max_id = db.Charger.Max(d => d.ID);
            ViewBag.ID = max_id + 1;
            var people = User.Identity.GetUserId();
            var department = cb.AspNetUsers.Where(a => a.Id == people).First().DepartmentName;
            var departmentid = db.DepartmentCode.Where(a => a.text.Trim() == department).First().value;
            ViewBag.fromDepartment = new SelectList(db.DepartmentCode, "value", "text", departmentid);
            ViewBag.person = new SelectList(cb.AspNetUsers, "username", "username");
            ViewBag.chargeClassify = new SelectList(db.ChargeType, "value", "text");
            ViewBag.date = DateTime.Now.ToString("yyyy-MM-dd");
            var user = cb.AspNetUsers.Find(people);//获取当前用户,20170901by周林，打印室要默认费用

            ViewBag.fromDepartment = new SelectList(db.DepartmentCode, "value", "text", departmentid);
            ViewBag.person = new SelectList(cb.AspNetUsers, "username", "username", user.UserName);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "转向财务科", Value = "1"},
                new SelectListItem { Text = "转向复印室", Value = "0"},
            };
            ViewBag.whereTransfer = new SelectList(list1, "Value", "Text");
            if (action == "返回")
            {
                return RedirectToAction("OtherFeiYongChaXun", "Charge");
            }
            if (action == "提交")
            {

                string diaoyue = Request.Form["diaoyue"];
                string zhengming = Request.Form["zhengming"];
                string zixun = Request.Form["zixun"];
                string unitName = Request.Form["unitName"];
                string itemName = Request.Form["itemName"];
                string total = Request.Form["totalExpense"];
                string classes = Request.Form["chargeClassify"];
                string people1 = Request.Form["person"];
                string department1 = Request.Form["fromDepartment"];
                string data = Request.Form["chargeTime"];
                string where = Request.Form["whereTransfer"];
                if (diaoyue == "")
                {
                    diaoyue = "0";
                }
                if (zhengming == "")
                {
                    zhengming = "0";
                }
                if (zixun == "")
                {
                    zixun = "0";
                }
                //var date = DateTime.Now.ToString("yyyymmdd");
                DateTime date1 = DateTime.Parse(Request.Form["chargeTime"]);
                var date = Request.Form["chargeTime"].Replace("-", "");
                string seqNo = date + "000";
                var seq = db.Charger.Where(a => a.chargeTime == date1);

                string[] chargetype = new string[3];
                chargetype[0] = "6";
                chargetype[1] = "5";
                chargetype[2] = "9";
                decimal[] feeNumbers = new decimal[3];
                feeNumbers[0] = decimal.Parse(zhengming);
                feeNumbers[1] = decimal.Parse(diaoyue);
                feeNumbers[2] = decimal.Parse(zixun);
                string chargeDetail = "证明费：" + zhengming;
                chargeDetail += "，调阅费：" + diaoyue;
                chargeDetail += "，咨询费：" + zixun;

                chargeDetail += "，总计：" + total;
                if (seq.Count() == 0)
                {
                    charge.seqNo = date + "001";
                }
                else
                {
                    var max_seqNo = seq.OrderByDescending(a => a.seqNo).First().seqNo;
                    long No = long.Parse(max_seqNo);
                    charge.seqNo = (No + 1).ToString();
                }
                for (int i = 0; i < chargetype.Length; i++)
                {
                    Charger charge1 = new Charger();
                    int id = GetMaxId();
                    charge1.ID = id;
                    charge1.unitName = unitName;
                    charge1.itemName = itemName;
                    if (itemName == "")
                    {
                        int id1 = int.Parse(chargetype[i]);
                        string type = db.ChargeType.Where(a => a.value == id1).First().text;
                        charge1.itemName = unitName+type;
                    }
                    
                    charge1.chargeDetail = chargeDetail;
                    charge1.totalExpense = feeNumbers[i];
                    charge1.chargeClassify = int.Parse(chargetype[i]);
                    charge1.chargeExtra = "";
                    charge1.searchNo = 0;
                    charge1.buildingArea = 0;
                    charge1.seqNo = charge.seqNo;
                    charge1.charger1 = "";
                    charge1.isCharge = false;
                    charge1.centiCnt = -1;
                    charge1.isBack = false;
                    charge1.@operator = people1;
                    charge1.chargeTime = DateTime.Parse( data);
                    charge1.fromDepartment = department1;
                    charge1.whereTransfer = int.Parse(where);
                    db.Charger.Add(charge1);
                    db.SaveChanges();
                }


            }
            return View();
        }
        // GET: OfficeInformation/Create
        public ActionResult Create()
        {
            ViewData["pagename"] = "Charge/Create";
            long max_id = db.Charger.Max(d => d.ID);
            ViewBag.ID = max_id + 1;
            var people = User.Identity.GetUserId();
            var department = cb.AspNetUsers.Where(a => a.Id == people).First().DepartmentName;
            if (department == "请选择科室")
            {
                ViewBag.fromDepartment = new SelectList(db.DepartmentCode, "value", "text");
            }
            else
            {
                var departmentid = db.DepartmentCode.Where(a => a.text.Trim() == department).First().value;
                ViewBag.fromDepartment = new SelectList(db.DepartmentCode, "value", "text", departmentid);
            }
            if (department == "业务科")
            {
                return RedirectToAction("yewujieyue");
            }
            
            

            ViewBag.person = new SelectList(cb.AspNetUsers, "username", "username");
            ViewBag.chargeClassify = new SelectList(db.ChargeType, "value", "text");
            ViewBag.date = DateTime.Now.ToString("yyyy-MM-dd");
            var user = cb.AspNetUsers.Find(people);//获取当前用户,20170901by周林，打印室要默认费用
            if (user.UserName=="打印室")
            {
                var departmentid = db.DepartmentCode.Where(a => a.text.Trim() == department).First().value;
                ViewBag.fromDepartment = new SelectList(db.DepartmentCode, "value", "text", departmentid);
                ViewBag.person = new SelectList(cb.AspNetUsers, "username", "username",user.UserName);
                ViewBag.chargeClassify = new SelectList(db.ChargeType, "value", "text",4);
            }


            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "转向财务科", Value = "1"},
                new SelectListItem { Text = "转向复印室", Value = "0"},
            };
            ViewBag.whereTransfer = new SelectList(list1, "Value", "Text");
            return View();
        }

        // POST: OfficeInformation/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,itemName,chargeExtra,searchNo,totalExpense,fromDepartment,unitName,buildingArea,chargeClassify,chargeDetail,operator,charger,chargeTime,isCharge,remarks,seqNo,theoryExpense,whereTransfer,isBack,centiCnt,backNote")] Charger charge,string action)
        {
            if (action == "返回")
            {
                return RedirectToAction("OtherFeiYongChaXun", "Charge");
            }
            string fei = Request.Form["totalExpense"];
            if (fei == "")
            {
                return Content("<script>alert('实际费用不能为空！');window.history.back();</script>");
            }
            string classes = Request.Form["chargeClassify"];
            if (classes == "")
            {
                return Content("<script>alert('请选择收费种类！');window.history.back();</script>");
            }
            string people = Request.Form["person"];
            if (people == "")
            {
                return Content("<script>alert('请选择操作人！');window.history.back();</script>");
            }
            string department = Request.Form["fromDepartment"];
            if (department == "")
            {
                return Content("<script>alert('请选择来源科室！');window.history.back();</script>");
            }
            string unitName = Request.Form["unitName"];
            string itemName = Request.Form["itemName"];
            if (itemName == "")
            {
                itemName ="";
                charge.itemName = unitName+"打印费";
            }
            string total = Request.Form["totalExpense"];
            long max_id = db.Charger.Max(d => d.ID);
            charge.chargeExtra = "";
            charge.searchNo = 0;
            charge.buildingArea = 0;
            charge.chargeDetail = "收取" + unitName + "，" + itemName + "，" + "共" + total + "元";
            charge.charger1 = "";
            charge.isCharge = false;
            charge.centiCnt = -1;
            charge.isBack = false;
            charge.@operator = people;
            decimal totalex = decimal.Parse(Request.Form["totalExpense"]);
            decimal theoryex = decimal.Parse(Request.Form["theoryExpense"]);
            charge.totalExpense = totalex;
            charge.theoryExpense = theoryex;
            //var date = DateTime.Now.ToString("yyyymmdd");
            DateTime date1 = DateTime.Parse(Request.Form["chargeTime"]);
            var date= Request.Form["chargeTime"].Replace("-", "");
            string seqNo = date + "000";
            int class1 = int.Parse(classes);
            var seq = db.Charger.Where(a => a.chargeTime == date1);
            if (seq.Count() == 0)
            {
                charge.seqNo = date + "001";
            }
            else
            {
                var max_seqNo = seq.OrderByDescending(a => a.seqNo).First().seqNo;
                long No = long.Parse(max_seqNo);
                charge.seqNo = (No+1).ToString();
            }
            db.Charger.Add(charge);
            db.SaveChanges();
            if (people == "打印室")
            {
                    FuyinFeeDetail fuyin = new FuyinFeeDetail();
                var maxID = db.FuyinFeeDetail.Max(a => a.ID);
                fuyin.ID = maxID+1;
                fuyin.searchNo = charge.seqNo;
                fuyin.unitCharged = people;
                fuyin.feeListNo = charge.seqNo; ;
                fuyin.dateCharged = date1;
                int a4p = int.Parse(Request.Form["a4picture"]);
                fuyin.A4DrawingCnt = a4p;
                double a4pfei = double.Parse(Request.Form["a4pCharge"]);
                fuyin.A4DrawingFee = a4pfei;
                double a4ptotal = double.Parse(Request.Form["a4ptotal"]);
                fuyin.A4DrawingUnitPrice = a4ptotal;
                int a4t = int.Parse(Request.Form["a4text"]);
                fuyin.A4TextCnt = a4t;
                double a4tfei = double.Parse(Request.Form["a4tCharge"]);
                fuyin.A4TextFee = a4tfei;
                double a4ttotal = double.Parse(Request.Form["a4ttotal"]);
                fuyin.A4TextUnitPrice = a4ttotal;
                int a4 = int.Parse(Request.Form["a4"]);
                fuyin.A4PageCnt = a4;
                int a3p = int.Parse(Request.Form["a3picture"]);
                fuyin.A3DrawingCnt = a3p;
                double a3pfei = double.Parse(Request.Form["a3pCharge"]);
                fuyin.A3DrawingFee = a3pfei;
                double a3ptotal = double.Parse(Request.Form["a3ptotal"]);
                fuyin.A3DrawingUnitPrice = a3ptotal;
                int a3t = int.Parse(Request.Form["a3text"]);
                fuyin.A3TextCnt = a3t;
                double a3tfei = double.Parse(Request.Form["a3tCharge"]);
                fuyin.A3TextFee = a3tfei;
                double a3ttotal = double.Parse(Request.Form["a3ttotal"]);
                fuyin.A3TextUnitPrice = a3ttotal;
                int a3 = int.Parse(Request.Form["a3"]);
                fuyin.A3PageCnt = a3;
                int a2p = int.Parse(Request.Form["a2picture"]);
                fuyin.A2DrawingCnt = a2p;
                double a2pfei = double.Parse(Request.Form["a2pCharge"]);
                fuyin.A2DrawingFee = a2pfei;
                double a2ptotal = double.Parse(Request.Form["a2ptotal"]);
                fuyin.A2DrawingUnitPrice = a2ptotal;
                int a2 = int.Parse(Request.Form["a2"]);
                fuyin.A2PageCnt = a2;
                int a1p = int.Parse(Request.Form["a1picture"]);
                fuyin.A1DrawingCnt = a1p;
                double a1pfei = double.Parse(Request.Form["a1pCharge"]);
                fuyin.A1DrawingFee = a1pfei;
                double a1ptotal = double.Parse(Request.Form["a1ptotal"]);
                fuyin.A1DrawingUnitPrice = a1ptotal;
                int a1= int.Parse(Request.Form["a1"]);
                fuyin.A1PageCnt = a1;
                int a0p = int.Parse(Request.Form["a0picture"]);
                fuyin.A0DrawingCnt = a0p;
                double a0pfei = double.Parse(Request.Form["a0pCharge"]);
                fuyin.A0DrawingFee = a0pfei;
                double a0ptotal = double.Parse(Request.Form["a0ptotal"]);
                fuyin.A0DrawingUnitPrice = a0ptotal;
                int a0 = int.Parse(Request.Form["a0"]);
                fuyin.A0PageCnt = a0;
                int a1l = int.Parse(Request.Form["a1lpicture"]);
                fuyin.A1AddDrawingCnt = a1l;
                double a1lfei = double.Parse(Request.Form["a1lpCharge"]);
                fuyin.A1AddDrawingFee = a1lfei;
                double a1lptotal = double.Parse(Request.Form["a1lptotal"]);
                fuyin.A1AddDrawingUnitPrice = a1lptotal;
                int a1long = int.Parse(Request.Form["a1long"]);
                fuyin.A1AddPageCnt = a1long;
                int a0l = int.Parse(Request.Form["a0lpicture"]);
                fuyin.A0AddDrawingCnt = a0l;
                double a0lfei = double.Parse(Request.Form["a0lpCharge"]);
                fuyin.A0AddDrawingFee = a0lfei;
                double a0lptotal = double.Parse(Request.Form["a0lptotal"]);
                fuyin.A0AddDrawingUnitPrice = a0lptotal;
                int a0long = int.Parse(Request.Form["a0long"]);
                fuyin.A0AddPageCnt = a0long;
                fuyin.totalFee = double.Parse(total);
                fuyin.theoryFee = double.Parse(Request.Form["theoryExpense"]);
                db.FuyinFeeDetail.Add(fuyin);
                db.SaveChanges();
            }
            
            return Content("<script>alert('提交成功！');window.location.href='./Create'</script>");
        }

        // GET: Charge/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vw_charge vw_charge = db.vw_charge.Find(id);
            if (vw_charge == null)
            {
                return HttpNotFound();
            }
            return View(vw_charge);
        }
        
        // GET: Charge/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Charger charge = db.Charger.Find(id);
            vw_charge VWcharge = db.vw_charge.Find(id);
            if (charge == null)
            {
                return HttpNotFound();
            }
            var UserID = User.Identity.GetUserId();
            var department = cb.AspNetUsers.Find(UserID).DepartmentName;
            if (charge.centiCnt != -1)
            {
                if (VWcharge.text.Trim() != department)
                {
                    return Content("<script >window.alert('您没有权限修改此项费用!');window.history.back();</script>");
                }

                else if (charge.isCharge == true)
                {
                    return Content("<script>alert('此费用已经收取，不能进行修改 !');window.history.back();</script>");
                }
                return RedirectToAction("HeJuFeiYongEdit", "OfficeDanganZhuangju", new { id = charge.searchNo });
            }
            else
            {
                if (charge.fromDepartment.Trim() == "1")
                {
                    return RedirectToAction("Edit", "ProjectCharge", new { id = id });
                }
                if (charge.fromDepartment.Trim() == "2")
                {
                    return RedirectToAction("FeeJieSuan", "UrbanBorrow", new { id = id,id1="2" });
                }
                else
                {
                    ViewBag.ID = charge.ID;
                    ViewBag.No = charge.seqNo;
                    ViewBag.fromDepartment = new SelectList(db.DepartmentCode, "value", "text", charge.fromDepartment);
                    ViewBag.person = new SelectList(cb.AspNetUsers, "UserName", "UserName", charge.@operator);
                    ViewBag.chargeClassify = new SelectList(db.ChargeType, "value", "text", charge.chargeClassify);
                    ViewBag.date = DateTime.Now.ToString("yyyy-MM-dd");
                    List<SelectListItem> list1 = new List<SelectListItem> {
                        new SelectListItem { Text = "转向财务科", Value = "1"},
                        new SelectListItem { Text = "转向复印室", Value = "0"},
                    };
                    ViewBag.whereTransfer = new SelectList(list1, "Value", "Text", charge.whereTransfer);
                }
            }
            
            
            return View(charge);
        }

        // POST: Charge/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,itemName,chargeExtra,searchNo,totalExpense,fromDepartment,unitName,buildingArea,chargeClassify,chargeDetail,operator,charger,chargeTime,isCharge,remarks,seqNo,theoryExpense,whereTransfer,isBack,centiCnt,backNote")] Charger charge, string action)
        {
            if (action == "返回")
            {
                return RedirectToAction("OtherFeiYongChaXun", "Charge");
            }
            if (ModelState.IsValid)
            {
                string unitName = Request.Form["unitName"];
                string itemName = Request.Form["itemName"];
                string total = Request.Form["totalExpense"];
                string people = Request.Form["person"];
                string classes = Request.Form["chargeClassify"];
                long ID = long.Parse(Request.Form["ID"]);
                string No = Request.Form["No"];
                charge.ID = ID;
                charge.chargeExtra = "";
                charge.searchNo = 0;
                charge.buildingArea = 0;
                charge.chargeDetail = "收取" + unitName + "，" + itemName + "，" + "共" + total + "元";
                charge.charger1 = "";
                charge.isCharge = false;
                charge.centiCnt = -1;
                charge.isBack = false;
                charge.@operator = people;
                charge.seqNo = No;
                db.Entry(charge).State = EntityState.Modified;
                db.SaveChanges();
                if (people == "打印室")
                {
                    FuyinFeeDetail fuyin = new FuyinFeeDetail();
                    DateTime date1 = DateTime.Parse(Request.Form["chargeTime"]);
                    var maxID = db.FuyinFeeDetail.Max(a => a.ID);
                    fuyin.ID = maxID + 1;
                    fuyin.searchNo = charge.seqNo;
                    fuyin.unitCharged = people;
                    fuyin.feeListNo = charge.seqNo;
                    fuyin.dateCharged = date1;
                    int a4p = int.Parse(Request.Form["a4picture"]);
                    fuyin.A4DrawingCnt = a4p;
                    double a4pfei = double.Parse(Request.Form["a4pCharge"]);
                    fuyin.A4DrawingFee = a4pfei;
                    double a4ptotal = double.Parse(Request.Form["a4ptotal"]);
                    fuyin.A4DrawingUnitPrice = a4ptotal;
                    int a4t = int.Parse(Request.Form["a4text"]);
                    fuyin.A4TextCnt = a4t;
                    double a4tfei = double.Parse(Request.Form["a4tCharge"]);
                    fuyin.A4TextFee = a4tfei;
                    double a4ttotal = double.Parse(Request.Form["a4ttotal"]);
                    fuyin.A4TextUnitPrice = a4ttotal;
                    int a4 = int.Parse(Request.Form["a4"]);
                    fuyin.A4PageCnt = a4;
                    int a3p = int.Parse(Request.Form["a3picture"]);
                    fuyin.A3DrawingCnt = a3p;
                    double a3pfei = double.Parse(Request.Form["a3pCharge"]);
                    fuyin.A3DrawingFee = a3pfei;
                    double a3ptotal = double.Parse(Request.Form["a3ptotal"]);
                    fuyin.A3DrawingUnitPrice = a3ptotal;
                    int a3t = int.Parse(Request.Form["a3text"]);
                    fuyin.A3TextCnt = a3t;
                    double a3tfei = double.Parse(Request.Form["a3tCharge"]);
                    fuyin.A3TextFee = a3tfei;
                    double a3ttotal = double.Parse(Request.Form["a3ttotal"]);
                    fuyin.A3TextUnitPrice = a3ttotal;
                    int a3 = int.Parse(Request.Form["a3"]);
                    fuyin.A3PageCnt = a3;
                    int a2p = int.Parse(Request.Form["a2picture"]);
                    fuyin.A2DrawingCnt = a2p;
                    double a2pfei = double.Parse(Request.Form["a2pCharge"]);
                    fuyin.A2DrawingFee = a2pfei;
                    double a2ptotal = double.Parse(Request.Form["a2ptotal"]);
                    fuyin.A2DrawingUnitPrice = a2ptotal;
                    int a2 = int.Parse(Request.Form["a2"]);
                    fuyin.A2PageCnt = a2;
                    int a1p = int.Parse(Request.Form["a1picture"]);
                    fuyin.A1DrawingCnt = a1p;
                    double a1pfei = double.Parse(Request.Form["a1pCharge"]);
                    fuyin.A1DrawingFee = a1pfei;
                    double a1ptotal = double.Parse(Request.Form["a1ptotal"]);
                    fuyin.A1DrawingUnitPrice = a1ptotal;
                    int a1 = int.Parse(Request.Form["a1"]);
                    fuyin.A1PageCnt = a1;
                    int a0p = int.Parse(Request.Form["a0picture"]);
                    fuyin.A0DrawingCnt = a0p;
                    double a0pfei = double.Parse(Request.Form["a0pCharge"]);
                    fuyin.A0DrawingFee = a0pfei;
                    double a0ptotal = double.Parse(Request.Form["a0ptotal"]);
                    fuyin.A0DrawingUnitPrice = a0ptotal;
                    int a0 = int.Parse(Request.Form["a0"]);
                    fuyin.A0PageCnt = a0;
                    int a1l = int.Parse(Request.Form["a1lpicture"]);
                    fuyin.A1AddDrawingCnt = a1l;
                    double a1lfei = double.Parse(Request.Form["a1lpCharge"]);
                    fuyin.A1AddDrawingFee = a1lfei;
                    double a1lptotal = double.Parse(Request.Form["a1lptotal"]);
                    fuyin.A1AddDrawingUnitPrice = a1lptotal;
                    int a1long = int.Parse(Request.Form["a1long"]);
                    fuyin.A1AddPageCnt = a1long;
                    int a0l = int.Parse(Request.Form["a0lpicture"]);
                    fuyin.A0AddDrawingCnt = a0l;
                    double a0lfei = double.Parse(Request.Form["a0lpCharge"]);
                    fuyin.A0AddDrawingFee = a0lfei;
                    double a0lptotal = double.Parse(Request.Form["a0lptotal"]);
                    fuyin.A0AddDrawingUnitPrice = a0lptotal;
                    int a0long = int.Parse(Request.Form["a0long"]);
                    fuyin.A0AddPageCnt = a0long;
                    fuyin.totalFee = double.Parse(total);
                    fuyin.theoryFee = double.Parse(Request.Form["theoryExpense"]);
                    db.FuyinFeeDetail.Add(fuyin);
                    db.SaveChanges();
                }
                return Content("<script>alert('修改成功！');window.location.href='./OtherFeiYongChaXun'</script>");

            }
            return View(charge);
        }

        // GET: Charge/Delete/5
        //public ActionResult Delete(long? id)
        //{
        //    Response.Write("<script >alert('确认要删除吗！！');</script >");
        //    return RedirectToAction("DeleteConfirmed");
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    vw_charge vw_charge = db.vw_charge.Find(id);
        //    if (vw_charge == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return Content("<script>alert('确认要删除吗！');window.location.href='./Delete'</script>");
        //}

        // POST: Charge/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete/*Confirmed*/(long id)
        {
            

            Charger charge = db.Charger.Find(id);
            if (charge.isCharge == true)
            {
                return Content("<script>alert('该费用已经收取，不能删除！');window.history.back();</script>");
            }
            db.Charger.Remove(charge);
            db.SaveChanges();
            return Content("<script>alert('已成功删除！');window.location.href='/Charge/OtherFeiYongChaXun'</script>");
        }
        public ActionResult Deletedfy(string id)
        {
            var charge = db.Charger.Where(a=>a.seqNo==id).ToList();
            if (charge.First().isCharge == true)
            {
                return Content("<script>alert('该费用已经收取，不能删除！');window.history.back();</script>");
            }
            for(int i=0;i<charge.Count();i++)
            {
                db.Charger.Remove(charge[i]);
            }
           
            var fuyin = db.FuyinFeeDetail.Where(a => a.feeListNo ==id).ToList();
            for (int i = 0; i < fuyin.Count(); i++)
            {
                db.FuyinFeeDetail.Remove(fuyin[i]);
            }
            db.SaveChanges();
            return Content("<script>alert('已成功删除！');window.location.href='/Charge/DaFuYinList'</script>");
        }
        public ActionResult ImageDelete(string id)
        {
            BindUserAndImage IMAGE = db.BindUserAndImage.Where(a=>a.userID==id).First();
            db.BindUserAndImage.Remove(IMAGE);
            db.SaveChanges();
            return Content("<script>alert('已成功删除！');window.location.href='/Charge/DaFuYinList'</script>");
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
