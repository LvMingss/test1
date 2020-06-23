using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using urban_archive.Models;
using PagedList;
using Microsoft.Reporting.WebForms;
using System.Web.Script.Serialization;
using System.Data.OleDb;
using Newtonsoft.Json;
using System.Text;
using System.Data.Entity.Validation;

namespace urban_archive.Controllers
{
    public class BorrowFeeDetailController : Controller
    {
        // GET: BorrowFeeDetail
        public ActionResult Index()
        {
            return View();
        }
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        private VideoArchiveEntities ac = new VideoArchiveEntities();
        private PlanArchiveEntities ae = new PlanArchiveEntities();

        // GET: ArchiveSearch
       
        public ActionResult BorrowFeeDetail( string currentFilter, string SearchString,int? SelectedID, string action)

        {
            //ViewData["pagename"] = "BorrowFeeDetail/BorrowFeeDetail";
    
            //if (action == "登记新用户")
            //{
            //    return RedirectToAction("RegisUser", "UrbanBorrow");
            //}
            //if (action == "打印登记用户")
            //{

            //}
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "收费事项", Value = "0"},
                new SelectListItem { Text = "缴费单位/缴费人", Value = "1"},
                new SelectListItem { Text = "收费日期", Value = "2" },
                new SelectListItem { Text = "编号", Value = "3" },
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            int t = SelectedID.GetValueOrDefault();
            ViewBag.CurrentFilter = SearchString;
            var borrow = db.vw_charge.Where(d => d.centiCnt != -1).Where(d => d.chargeClassify < 13).Where(d => d.fromDepartment == "2").Select(p => new { p.text, p.seqNo, p.itemName, p.unitName, p.chargeDetail, p.isCharge,p.chargeTime, p.isBack, p.backNote, p.remarks,p.searchNo }).Distinct();
            DateTime time = DateTime.Parse("2016/12/12");
            if (t==2)
            {
               
                if (SearchString != "" && SearchString != null)
                {
                    time = DateTime.Parse(SearchString.Trim());
                }
            }
          


            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (t)
                {

                    case 0:
                        borrow = borrow.Where(ad => ad.itemName.Contains(SearchString));//根据收费事项
                        break;
                    case 1:
                        borrow = borrow.Where(ad => ad.unitName.Contains(SearchString));//根据单位名称
                        break;
                    case 2:
                        borrow = borrow.Where(ad => ad.chargeTime == time);//根据收费时间
                        break;
                    case 3:
                        borrow = borrow.Where(ad => ad.seqNo.Contains(SearchString));//根据收费编号
                        break;

                }

            }

            // 默认按借阅编号排
            borrow = borrow.OrderByDescending(s => s.seqNo);
            int g = borrow.Count();
            string theString = JsonConvert.SerializeObject(borrow);


            ViewBag.feiyong = theString;
            //初始化第一条数据
            //var combine = from a in db.ChargeCombine
            //              select a;

            //if(borrow.Count()!=0)
            //{
            //    ChargeCombine com = new ChargeCombine();
            //    string[] detail = borrow.First().chargeDetail.Trim().Split('，'); 
            //    if (detail.Length >= 2)
            //    {

            //        string[] strZhengmingFee = detail[0].Split('：');
            //        string[] strChajuanFee = detail[1].Split('：');

            //        decimal totalFee = decimal.Parse(strZhengmingFee[1]) + decimal.Parse(strChajuanFee[1]);

            //          com.diaoyueFee=double.Parse(strChajuanFee[1].ToString().Trim()) ;
            //          com.zhengmingFee = double.Parse(strZhengmingFee[1].ToString().Trim()) ;
            //          com.Total = double.Parse(totalFee.ToString().Trim()) ;
            //    }
            //    else
            //    {
            //        com.diaoyueFee = 0;
            //        com.zhengmingFee = 0;
            //        com.Total= 0;
            //    }
            //if(borrow.First().fromDepartment == "1")
            //{
            //    com.depart = "业务科";
            //}
            //if (borrow.First().fromDepartment == "2")
            //{
            //    com.depart = "管理科";
            //}
            //if (borrow.First().fromDepartment == "3")
            //{
            //    com.depart = "声像科";
            //}
            //if (borrow.First().fromDepartment == "4")
            //{
            //    com.depart = "办公室";
            //}
            //if (borrow.First().fromDepartment == "5")
            //{
            //    com.depart = "复印室";
            //}
            //if(borrow.First().chargeClassify==6)
            //{
            //    com.zhengmingFee = Convert.ToDouble(borrow.First().totalExpense); 

            //}
            //if(borrow.First().chargeClassify==5)
            //{
            //    com.diaoyueFee = Convert.ToDouble(borrow.First().totalExpense);
            //}
            //if(borrow.First().chargeClassify ==9)
            //{
            //    com.zixunFee= Convert.ToDouble(borrow.First().totalExpense);
            //}
            //    com.seq = borrow.First().seqNo;
            //    db.ChargeCombine.Add(combine.First());

            //}


            // for(int i=1;i<borrow.Count();i++)
            //{
            //      ChargeCombine com = new ChargeCombine();
            //    if (borrow.ElementAt(i).seqNo != combine.ElementAt(combine.Count() - 1).seq)
            //    {

            //        if (borrow.ElementAt(i).fromDepartment == "1")
            //        {
            //            com.depart = "业务科";
            //        }
            //        if (borrow.ElementAt(i).fromDepartment == "2")
            //        {
            //            com.depart = "管理科";
            //        }
            //        if (borrow.ElementAt(i).fromDepartment == "3")
            //        {
            //            com.depart = "声像科";
            //        }
            //        if (borrow.ElementAt(i).fromDepartment == "4")
            //        {
            //            com.depart = "办公室";
            //        }
            //        if (borrow.ElementAt(i).fromDepartment == "5")
            //        {
            //            com.depart = "复印室";
            //        }
            //        if (borrow.ElementAt(i).chargeClassify == 6)
            //        {
            //            com.zhengmingFee = Convert.ToDouble(borrow.ElementAt(i).totalExpense);

            //        }
            //        if (borrow.ElementAt(i).chargeClassify == 5)
            //        {
            //            com.diaoyueFee = Convert.ToDouble(borrow.ElementAt(i).totalExpense);
            //        }
            //        if (borrow.ElementAt(i).chargeClassify == 9)
            //        {
            //            com.zixunFee = Convert.ToDouble(borrow.ElementAt(i).totalExpense);
            //        }
            //        db.ChargeCombine.Add(com);
            //    }
            //    else
            //    {
            //        if (borrow.ElementAt(i).chargeClassify == 6)
            //        {
            //            combine.ElementAt(combine.Count() - 1).zhengmingFee = Convert.ToDouble(borrow.ElementAt(i).totalExpense);

            //        }
            //        if (borrow.ElementAt(i).chargeClassify == 5)
            //        {
            //            combine.ElementAt(combine.Count() - 1).diaoyueFee = Convert.ToDouble(borrow.ElementAt(i).totalExpense);
            //        }
            //        if (borrow.ElementAt(i).chargeClassify == 9)
            //        {
            //            combine.ElementAt(combine.Count() - 1).zixunFee=Convert.ToDouble(borrow.ElementAt(i).totalExpense);
            //        }
            //    }
            //}
            //var ch = from a in db.ChargeCombine
            //         select a;
            //foreach (var item in borrow)
            //{


            //        string[] detail = borrow.First().chargeDetail.Trim().Split(',');
            //        if (detail.Length >= 2)
            //        {

            //            string[] strZhengmingFee = detail[0].Split('：');
            //            string[] strChajuanFee = detail[1].Split('：');

            //            decimal totalFee = decimal.Parse(strZhengmingFee[1]) + decimal.Parse(strChajuanFee[1]);

            //            item.centiCnt = double.Parse(strChajuanFee[1].ToString().Trim());
            //            com.zhengmingFee = double.Parse(strZhengmingFee[1].ToString().Trim());
            //            com.Total = double.Parse(totalFee.ToString().Trim());
            //        }
            //        else
            //        {
            //            com.diaoyueFee = 0;
            //            com.zhengmingFee = 0;
            //            com.Total = 0;
            //        }

            //    }
            //}
            //int pageSize = 100;
            //int pageNumber = (page ?? 1);
            return View();
        }

        // GET: BorrowFeeDetail/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BorrowFeeDetail/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BorrowFeeDetail/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: BorrowFeeDetail/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BorrowFeeDetail/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: BorrowFeeDetail/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BorrowFeeDetail/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
