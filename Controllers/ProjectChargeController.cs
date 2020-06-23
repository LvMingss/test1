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
using Microsoft.Reporting.WebForms;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
namespace urban_archive.Controllers
{
    public class ProjectChargeController : Controller
    {
          private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities user = new UrbanUsersEntities(); 
        // GET: ProjectCharge
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult charge(int? flag,string action, string type = "PDF")
        {
            if (flag == 1)
            {

            }
            else if (flag ==2)
            {

            }

            if (action == "打印财务收费明细")
            {
                LocalReport localReport = new LocalReport();
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                var ds = db.vw_charge.Where(ad => ad.chargeTime >= DataFrom).Where(ad => ad.chargeTime <= DataTo).ToList();
                int charge = ds.Where(a => a.isCharge == true).ToList().Count();
                int nocharge=ds.Where(a => a.isCharge == false).ToList().Count();
                int total = ds.Count();
                decimal? n = 0;
                for (int i = 0; i < total; i++)
                {
                    decimal? fee = ds[i].totalExpense;
                    n += fee;
                }
                //var ds = db.vw_projectList.Where(ad => ad.paperProjectSeqNo > 20).Where(ad => ad.paperProjectSeqNo < 40);
                localReport.ReportPath = Server.MapPath("~/Report/jungong/xiangxishoufei.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("xiangxishoufeiDataSet", ds);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().Trim()));
                parameterList.Add(new ReportParameter("charge", charge.ToString().Trim()));
                parameterList.Add(new ReportParameter("nocharge", nocharge.ToString().Trim()));
                parameterList.Add(new ReportParameter("total", total.ToString().Trim()));
                parameterList.Add(new ReportParameter("fee", n.ToString().Trim()));
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
            //if (action == "取消")
            //{
            //    Response.Write("<script>window.close();</script>");
            //}
            return View();
        }

        public ActionResult classify(string action, string type = "PDF")
        {
            ViewBag.shoufeitype = new SelectList(db.ChargeType, "value", "text");
            if (action == "打印统计分类明细") {
                LocalReport localReport = new LocalReport();
                string shoufeitype = Request.Form["shoufeitype"];
                string shoufeitype1 = Request.Form["shoufeitype1"];
                if (Request.Form["startdata"] == "" || Request.Form["enddata"] == "") {
                    return Content("<script>alert('请输入起止时间！');history.go(-1);</script>");
                }
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                var ds = db.Charger.Where(ad => ad.chargeClassify.ToString() == shoufeitype).Where(ad => ad.chargeTime >= DataFrom).Where(ad => ad.chargeTime <= DataTo).ToList();
                if (ds.Count() == 0) {
                    return Content("<script>alert('所查内容没有记录！');history.back();</script>");
                }
                decimal? totalexpense = 0;
                for (int i = 0; i < ds.Count(); i++) {
                    totalexpense = totalexpense + ds[i].totalExpense;
                }
                localReport.ReportPath = Server.MapPath("~/Report/jungong/shoufeifenlei.rdlc");
                ReportDataSource reportDataSource1 = new ReportDataSource("shoufeifenleiDataSet", ds);
                localReport.DataSources.Add(reportDataSource1);
                List<ReportParameter> fenleilist = new List<ReportParameter>();
                fenleilist.Add(new ReportParameter("DataFrom", DataFrom.ToString().Trim()));
                fenleilist.Add(new ReportParameter("DataTo", DataTo.ToString().Trim()));
                fenleilist.Add(new ReportParameter("shoufeitype1", shoufeitype1.ToString().Trim()));
                fenleilist.Add(new ReportParameter("totalexpense", totalexpense.ToString().Trim()));
                localReport.SetParameters(fenleilist);
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
        // GET: ProjectCharge/Details/5
        public ActionResult Details(int ?id,string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var test = from ad in db.vw_projectProfile
                       where (ad.projectID == id)
                       select ad;
            vw_projectProfile projectProfile = test.First();
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", test.First().retentionPeriodNo);
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", test.First().securityID);
            if (projectProfile == null)
            {
                return HttpNotFound();
            }
            if (action == "返回")
            {
                return RedirectToAction("projectchargeindex", "ProjectCharge");
            }
            return View(projectProfile);
         

        }

        // GET: ProjectCharge/Create
        public ActionResult Create(string sortOrder, string currentFilter, string SearchString, int? page, int? SelectedID)
        {
            ViewData["pagename"] = "ProjectCharge-Create";
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "事项名称", Value = "0"},
                new SelectListItem { Text = "收费单位", Value = "1"},
                new SelectListItem { Text = "收费日期(格式：2017-06-01）", Value = "2" },
                new SelectListItem { Text = "编号", Value = "3" },
               
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            int t = SelectedID.GetValueOrDefault();

            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }
            ViewBag.CurrentFilter = SearchString;
            //var depart = from a in db.DepartmentCode
            //             where a.text == User.Identity.Name
            //             select a.value;
            //jsy修改
            var UserID = User.Identity.GetUserId();
            var department = user.AspNetUsers.Find(UserID).DepartmentName;
            var vwprojectcharge = from ad in db.vw_charge
                                  where ad.chargeType == "档案整理费" && ad.text ==department&&ad.centiCnt!=-1
                                  select ad;
                                       //jsy修改根据登陆用户查询记录
            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (t)
                {
                    case 0:
                        vwprojectcharge = vwprojectcharge.Where(ad => ad.itemName.Contains(SearchString));//根据事项名称搜索
                        break;
                    case 1:
                       
                        vwprojectcharge = vwprojectcharge.Where(ad => ad.unitName.Contains(SearchString));//根据收费单位搜索
                        break;
                    case 2:
                        DateTime date = DateTime.Parse(SearchString);
                        vwprojectcharge = vwprojectcharge.Where(ad => ad.chargeTime== date);//根据收费日期搜索
                        break;
                    case 3:
                        vwprojectcharge = vwprojectcharge.Where(ad => ad.seqNo.ToString().Contains(SearchString));//根据编号搜索
                        break;
                }

            }
            vwprojectcharge = vwprojectcharge.OrderByDescending(s => s.seqNo);// 默认按收费编号排
            ViewBag.result = JsonConvert.SerializeObject(vwprojectcharge);
            return View();

        }

        // POST: ProjectCharge/Create
        [HttpPost] 
         public ActionResult Create(string action)
        
        {
            
            return View();

        }

    // GET: ProjectCharge/Edit/5
    public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var test = from ad in db.vw_charge
                       where (ad.ID == id)
                       select ad;
            vw_charge charge = test.First();
            
            var test1 = from a in db.FuyinFeeDetail
                        where a.feeListNo ==charge.seqNo
                        select a;
            //相关按钮可见性设置
            ViewData["lbProjectNo"] = "none";
            ViewData["txtProjectNo"] = "none";
            ViewData["shoufeiTypedisplay"] = "none";
            ViewData["centiCntdisplay"] = "none";
            //jsy添加
            if (charge.isCharge == true)
            {
                Response.Write("<script>alert('此费用已经收取，不能进行修改 !');window.history.back();</script>");
            }

                                                                                                 //jsy添加
            if (charge == null)
            {
                return HttpNotFound();
            }
           
            else
            {
                if(charge.fromDepartment.Trim()=="1")//业务科
                {
                    ViewData["lbProjectNo"] = "display";
                    ViewData["txtProjectNo"] = "display";
                    ViewData["shoufeiTypedisplay"] = "display";
                   
                    if (charge.chargeExtra.Trim()=="1")//档案馆整理
                    {
                      //不显示公分数
                    }
                    if (charge.chargeExtra.Trim() == "2")//档案馆整理
                    {
                        if(charge.centiCnt!=-1)
                        {
                             ViewData["centiCntdisplay"] = "display";
                        }
                       
                    }
                }
                else
                {
                    //不显示收费类型
                }
                if (charge.chargeExtra.Trim() == "1" || charge.chargeExtra == "")
                {
                    charge.chargeExtra = "1";
                }

                //相关控件的初始化
                ViewBag.chargeClassify = new SelectList(db.ChargeType, "value", "text", charge.chargeClassify);
                ViewBag.fromDepartment = new SelectList(db.DepartmentCode, "value", "text", charge.fromDepartment);
                ViewBag.operator1 = new SelectList(user.AspNetUsers, "UserName", "UserName", charge.@operator);
                //费用转向
                List<SelectListItem> list = new List<SelectListItem>
                {
                new SelectListItem { Text = "转向复印室", Value = "0"},
                new SelectListItem { Text = "转向财务科", Value = "1"},
               };
              
                ViewBag.whereTransfer = new SelectList(list, "Value", "Text", charge.whereTransfer);
                //相关按钮的可见性设置

                if (test1.Count()!=0)
                {
                    FuyinFeeDetail charge1 = test1.First();

                    
                        ViewData["a4total"] = charge1.A4PageCnt;
                        ViewData["a4word"] = charge1.A4TextCnt;
                        ViewData["a4pic"] = charge1.A4DrawingCnt;
                        ViewData["a4wordTotalFee"] = charge1.A4TextFee;
                        ViewData["a4picTotalFee"] = charge1.A4DrawingFee;
                        ViewData["a3total"] = charge1.A3PageCnt;
                        ViewData["a3word"] = charge1.A3TextCnt;
                        ViewData["a3pic"] = charge1.A3DrawingCnt;
                        ViewData["a3wordTotalFee"] = charge1.A3TextFee;
                        ViewData["a3picTotalFee"] = charge1.A4DrawingFee;
                        ViewData["a2total"] = charge1.A2PageCnt;
                        ViewData[" a2pic"] = charge1.A2DrawingCnt;
                        ViewData["a2picTotalFee"] = charge1.A2DrawingFee;
                        ViewData["a1total"] = charge1.A1PageCnt;
                        ViewData["a1pic"] = charge1.A1DrawingCnt;
                        ViewData["a1picTotalFee"] = charge1.A1DrawingFee;
                        ViewData["a0total"] = charge1.A0PageCnt;
                        ViewData["a0pic"] = charge1.A1AddDrawingCnt;
                        ViewData["a0picTotalFee"] = charge1.A0DrawingFee;
                        ViewData["a0totalLonger"] = charge1.A0AddPageCnt;
                        ViewData["a0picLonger"] = charge1.A0DrawingCnt;
                        ViewData["a0picTotalFeeLonger"] = charge1.A0AddDrawingFee;
                        ViewData["a1totalLonger"] = charge1.A1AddPageCnt;
                        ViewData["a1picLonger"] = charge1.A1DrawingCnt;
                        ViewData["a1picTotalFeeLonger"] = charge1.A1AddDrawingFee;
                    }
            }
            
            return View(charge );
        }

        // POST: ProjectCharge/Edit/5
        [HttpPost]
         public ActionResult Edit(string seqNo,string searchNo, string unitName,string itemName, string chargeClassify , string chargeTime, string totalExpense, string theoryExpense, string a4total, string a4picPricePer, string a4pic ,string a4picTotalFee, string a4word, string a4wordPricePer, string a4wordTotalFee, string a3total, string a3pic, string a3picPricePer, string a3picTotalFee, string a3word, string a3wordPricePer, string a3wordTotalFee, string a2total, string a2pic, string a2picPricePer, string a2picTotalFee, string a1total, string a1pic, string a1picPricePer, string a1picTotalFee, string a0total, string a0pic, string a0picPricePer, string a0picTotalFee, string a1totalLonger, string a1picLonge, string a1picPricePerLonger, string a1picTotalFeeLonger, string a0totalLonger, string a0picLonger, string a0picPricePerLonger, string a0picTotalFeeLonger , string fromDepartment,string @operator,string remarks,string chargeExtra,string buildingArea,string centiCnt,string whereTransfer, string action, long ?id)


        {
            
            Charger charge = db.Charger.Find(id);
           
            if (action == "修改")
            {
                bool flag= false;
                string roles = User.Identity.Name.ToString();
                if (roles =="打印")
                 {
                        flag = true; 
                 }
              if(flag==true)
                { 
                    if(chargeClassify.Trim()=="4")
                    {
                        var file = from a in db.FuyinFeeDetail
                                   where a.feeListNo == seqNo
                                   select a;
                        if (file.Count() != 0)
                        {
                           
                            FuyinFeeDetail filefuyin = file.First();
                            if(a4total==""|| a4total==null)
                            {
                                a4total = "0";
                            }
                            int A4pageCnt = int.Parse(a4total.Trim());
                            filefuyin.A4PageCnt = A4pageCnt;
                            if(a4pic == ""|| a4pic == null)
                            {
                                a4total = "0";
                            }
                            int A4picCnt = int.Parse(a4pic.Trim());
                            filefuyin.A4DrawingCnt = A4picCnt;
                            if (a4picPricePer == "" || a4picPricePer == null)
                            {
                                a4total = "0";
                            }
                            float A4picprice = float.Parse(a4picPricePer.Trim());
                            filefuyin.A4DrawingUnitPrice = A4picprice;
                            if (a4picTotalFee == "" || a4picTotalFee == null)
                            {
                                a4total = "0";
                            }
                            float  A4pictotal =float.Parse(a4picTotalFee.Trim());
                            filefuyin.A4DrawingFee = A4pictotal;
                            if (a4word == "" || a4word == null)
                            {
                                a4total = "0";
                            }

                            int A4wordCnt = int.Parse(a4word.Trim());
                            filefuyin.A4TextCnt = A4wordCnt;
                            if (a4wordPricePer == "" || a4wordPricePer == null)
                            {
                                a4total = "0";
                            }
                            float A4wordprice = float.Parse(a4wordPricePer.Trim());
                            filefuyin.A3TextUnitPrice = A4wordprice;
                            if (a4wordTotalFee == "" || a4wordTotalFee == null)
                            {
                                a4total = "0";
                            }
                            float A4wordtotal = float.Parse(a4wordTotalFee.Trim());
                            filefuyin.A4TextFee = A4wordtotal;
                            if (a3total == "" || a3total == null)
                            {
                                a4total = "0";
                            }
                            int a3pagecn = int.Parse(a3total.Trim());
                            filefuyin.A3PageCnt = a3pagecn;
                            if (a3pic == "" || a3pic == null)
                            {
                                a4total = "0";
                            }
                            int A3piccnt = int.Parse(a3pic.Trim());
                            filefuyin.A3DrawingCnt = A3piccnt;
                            if (a3picPricePer == "" || a3picPricePer == null)
                            {
                                a4total = "0";
                            }
                            float A3picprice = float.Parse(a3picPricePer.Trim());
                            filefuyin.A3DrawingUnitPrice = A3picprice;
                            if (a3picTotalFee == "" || a3picTotalFee == null)
                            {
                                a4total = "0";
                            }
                            float A3pictotal = float .Parse(a3picTotalFee.Trim());
                            filefuyin.A3DrawingFee = A3pictotal;
                            if (a3word == "" || a3word == null)
                            {
                                a4total = "0";
                            }
                            int A3wordcnt = int.Parse(a3word.Trim());
                            filefuyin.A3TextCnt = A3wordcnt;
                            if (a3wordPricePer == "" || a3wordPricePer == null)
                            {
                                a4total = "0";
                            }
                            float  A3wordprice= float.Parse(a3wordPricePer.Trim());
                           filefuyin.A3TextUnitPrice = A3wordprice;
                            if (a3wordTotalFee == "" || a3wordTotalFee == null)
                            {
                                a4total = "0";
                            }
                            float A3wordtotal = float.Parse(a3wordTotalFee.Trim());
                           filefuyin.A3TextFee = A3wordtotal;
                            if (a2total == "" || a2total == null)
                            {
                                a4total = "0";
                            }
                            int A2pagecn = int.Parse(a2total.Trim());
                            filefuyin.A4PageCnt = A4pageCnt;
                            if (a2pic == "" || a2pic == null)
                            {
                                a4total = "0";
                            }
                            int A2pageCnt = int.Parse(a2pic.Trim());
                            filefuyin.A2PageCnt = A2pageCnt;
                            if (a2picPricePer == "" || a2picPricePer == null)
                            {
                                a4total = "0";
                            }
                            float A2picprice = float.Parse(a2picPricePer.Trim());
                            filefuyin.A2DrawingUnitPrice = A2picprice;
                            if (a2picTotalFee == "" || a2picTotalFee == null)
                            {
                                a4total = "0";
                            }
                            float A2pictoyal = float.Parse(a2picTotalFee.Trim());
                            filefuyin.A2DrawingFee = A2pictoyal;
                            if (a1total == "" || a1total == null)
                            {
                                a4total = "0";
                            }
                            int A1pageCnt = int.Parse(a1total.Trim());
                            filefuyin.A1PageCnt = A1pageCnt;
                            if (a1pic == "" || a1pic == null)
                            {
                                a4total = "0";
                            }
                            int A1picCnt = int.Parse(a1pic.Trim());
                            filefuyin.A1DrawingCnt = A1picCnt;
                            if (a1picPricePer == "" || a1picPricePer == null)
                            {
                                a4total = "0";
                            }
                            float  A1picprice = float.Parse(a1picPricePer.Trim());
                            filefuyin.A1DrawingUnitPrice = A1picprice;
                            if (a1picTotalFee == "" || a1picTotalFee == null)
                            {
                                a4total = "0";
                            }
                            float A1pictotal = float.Parse(a1picTotalFee.Trim());
                            filefuyin.A1DrawingFee = A1pictotal;
                            if (a0total == "" || a0total == null)
                            {
                                a4total = "0";
                            }
                            int A0pageCnt = int.Parse(a0total.Trim());
                            filefuyin.A0PageCnt= A0pageCnt;
                            if (a0pic == "" || a0pic == null)
                            {
                                a4total = "0";
                            }
                            int A0picCnt = int.Parse(a0pic.Trim());
                            filefuyin.A0DrawingCnt = A0picCnt;
                            if (a0picPricePer == "" || a0picPricePer == null)
                            {
                                a4total = "0";
                            }
                            float A0picprice = float.Parse(a0picPricePer.Trim());
                            filefuyin.A0DrawingUnitPrice = A0picprice;
                            if (a0picTotalFee == "" || a0picTotalFee == null)
                            {
                                a4total = "0";
                            }
                            float  A0total = float .Parse(a0picTotalFee.Trim());
                            filefuyin.A0DrawingFee = A0total;

                            if (a1totalLonger == "" || a1totalLonger == null)
                            {
                                a4total = "0";
                            }
                            int A1addpageCnt = int.Parse(a1totalLonger.Trim());
                            filefuyin.A1AddDrawingCnt = A1addpageCnt;
                            if (a1picLonge == "" || a1picLonge == null)
                            {
                                a4total = "0";
                            }
                            int A1addpiccnt = int.Parse(a1picLonge.Trim());
                            filefuyin.A1AddDrawingCnt = A1addpiccnt;
                            if (a1picPricePerLonger == "" || a1picPricePerLonger == null)
                            {
                                a4total = "0";
                            }
                            float A1addpicprice = float .Parse(a1picPricePerLonger.Trim());
                            filefuyin.A1AddDrawingUnitPrice = A1addpicprice;
                            if (a1picTotalFeeLonger == "" || a1picTotalFeeLonger == null)
                            {
                                a4total = "0";
                            }
                            float  A1addtotal = float .Parse(a1picTotalFeeLonger);
                            filefuyin.A1AddDrawingFee= A1addtotal;
                            if (a0totalLonger == "" || a0totalLonger == null)
                            {
                                a4total = "0";
                            }
                            int a0addpagecnt = int.Parse(a0totalLonger.Trim());
                            filefuyin.A0AddPageCnt = a0addpagecnt;
                            if (a0picLonger == "" || a0picLonger == null)
                            {
                                a4total = "0";
                            }
                            int A0addpiccnt = int.Parse(a0picLonger.Trim());
                            filefuyin.A0AddDrawingCnt = A0addpiccnt;
                            if (a0picPricePerLonger == "" || a0picPricePerLonger == null)
                            {
                                a4total = "0";
                            }
                            float  a0addprice = float.Parse(a0picPricePerLonger.Trim());
                            filefuyin.A0AddDrawingFee = a0addprice;
                            if (a0picTotalFeeLonger == "" || a0picTotalFeeLonger == null)
                            {
                                a4total = "0";
                            }
                            float  A0addtotal = float .Parse(a0picTotalFeeLonger.Trim());
                            filefuyin.A0AddDrawingFee = A0addtotal;
                            DateTime chargetime = DateTime.Today;
                            filefuyin.dateCharged = chargetime;
                            filefuyin.unitCharged = @operator;
                            if (theoryExpense.Trim() != ""|| theoryExpense.Trim()==null)
                            {
                                filefuyin.theoryFee = double.Parse(theoryExpense.Trim());

                            }
                            filefuyin.searchNo =seqNo.Trim();
                            if (totalExpense.Trim() != "")
                            {
                                filefuyin.totalFee = double.Parse(totalExpense.Trim());

                            }
                            db.Entry(filefuyin).State = EntityState.Modified;
                        }

                    }
                    
                }
              if(charge!=null)
                {
                    charge.unitName = unitName;
                    charge.itemName = itemName;
                    bool flag1 = false;
                    var type = from a in db.ChargeType
                               where a.value.ToString() == chargeClassify
                               select a;
                    if(type.Count()!=0)
                    {
                        flag1= true;
                        charge.chargeClassify = Convert.ToInt32(chargeClassify);    
                    }
                    if(flag1==false)
                    {
                        //将新的收费项添加到列表中区
                        var type1 = from b in db.ChargeType
                                    orderby b.value descending
                                    select b;
                        ChargeType type3 = new ChargeType();
                        type3.value = type1.First().value + 1;
                        type3.text = chargeClassify;
                        db.ChargeType.Add(type3);

                    }
                    charge.@operator = @operator;
                    charge.seqNo = seqNo;
                    charge.remarks = remarks;
                    DateTime time = Convert.ToDateTime(chargeTime);
                    charge.chargeTime = time;
                    if (totalExpense.Trim() != "")
                    {
                        charge.totalExpense = decimal.Parse(totalExpense.Trim());
                    }
                    else
                    {
                        return Content("<script >alert('实际收费不能为空！');window.history.back();</script >");
                        
                        
                    }
                    if (theoryExpense.Trim() != "")
                    {
                        charge.theoryExpense = decimal.Parse(theoryExpense.Trim());
                    }
                    else
                    {
                        charge.theoryExpense = charge.totalExpense;
                    }
                    charge.fromDepartment = fromDepartment;
                    charge.whereTransfer = Int32.Parse(whereTransfer);
                    charge.chargeDetail = "收取" + charge.unitName + "," + charge.itemName + ",共" + totalExpense.Trim() + "元";
                    charge.isBack = false;
                    if (charge.fromDepartment.Trim() == "1")//来自业务科
                    {
                        if (chargeExtra=="1")
                        {
                            charge.chargeExtra = "1";
                        }
                        if (chargeExtra=="2")
                        {
                            charge.chargeExtra = "2";
                            if (centiCnt.Trim() == "")
                            {
                                return Content("<script >alert('公分数不能为空！');window.history.back();</script >");
                               
                            }
                            charge.centiCnt = float.Parse(centiCnt.Trim());
                        }
                        if (buildingArea.Trim() == "")
                        {
                            return Content("<script >alert('建筑面积不能为空！');window.history.back();</script >");
                          
                        }
                        charge.searchNo = Int32.Parse(searchNo);
                        charge.buildingArea = float.Parse(buildingArea.Trim());
                    }
                    try
                    {
                        db.Entry(charge).State = EntityState.Modified;
                        db.SaveChanges();
                        ViewData["xiugai"] = true;
                        return Content("<script >alert('收费修改成功');window.history.back();</script >");
                       
                    }
                    catch (System.Exception ex)
                    {
                        Response.Write("<script language=\'javascript\'>alert('" + ex.Message + "');</script>");
                      
                    }
                }

                
            }
            if (action == "返回")             //jsy修改
            {
                if (charge.centiCnt != -1)
                {
                    return RedirectToAction("Create", "ProjectCharge");
                }
                else
                {
                    return RedirectToAction("OtherFeiYongChaXun", "Charge");
                }
            }
                                                           //jsy修改

            return View();

        }
        
        public ActionResult projectchargeindex(string currentFilter, string SearchString,int? SelectedID)
        {
            ViewData["pagename"] = "ProjectCharge-projectchargeindex";
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程地点", Value = "1"},
                new SelectListItem { Text = "建设单位", Value = "2" },
                new SelectListItem { Text = "施工单位", Value = "3" },
                new SelectListItem { Text = "工程序号", Value = "4" },
                new SelectListItem { Text = "责任书编号", Value = "5" },
               

            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text",0);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "全部", Value = "0"},
                new SelectListItem { Text = "已收费", Value = "1"},
                new SelectListItem { Text = "未收费", Value = "2" },
                


            };
            ViewBag.SelectedID1 = new SelectList(list1, "Value", "Text",0);
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            int t = SelectedID.GetValueOrDefault();

            
            var vwproject = from ad in db.vw_ProjectStatus
                                  select ad;
            vwproject = vwproject.OrderByDescending(s => s.projectNo);
            ViewBag.result = JsonConvert.SerializeObject(vwproject);
            return View();


        }
       public string Finding(string id1,string id2,string SearchString)
        {
            var vwproject = from ad in db.vw_ProjectStatus
                            select ad;
            int t = Int32.Parse(id1.Trim());
            if(id2=="0")
            {
                
                    switch (t)
                    {
                        case 0:
                            vwproject = vwproject.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                            break;
                        case 1:

                            vwproject = vwproject.Where(ad => ad.location.Contains(SearchString));//根据地点搜索
                            break;
                        case 2:

                            vwproject = vwproject.Where(ad => ad.developmentOrganization.Contains(SearchString));//根据建设单位搜索
                            break;
                        case 3:
                            vwproject = vwproject.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                            break;
                        case 4:
                 
                            vwproject = vwproject.Where(ad => ad.projectNo.ToString().Contains(SearchString));//根据工程序号搜索
                            break;
                        case 5:

                            vwproject = vwproject.Where(ad => ad.contractNo.ToString().Contains(SearchString));//根据责任书编号搜索
                            break;

                    }

                
            }
            if (id2 == "1")
            {
                
                    vwproject = from a in vwproject
                                where a.isCharge == true
                                orderby a.projectNo descending
                                select a;
                    switch (t)
                    {
                        case 0:
                            vwproject = vwproject.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                            break;
                        case 1:

                            vwproject = vwproject.Where(ad => ad.location.Contains(SearchString));//根据地点搜索
                            break;
                        case 2:

                            vwproject = vwproject.Where(ad => ad.developmentOrganization.Contains(SearchString));//根据建设单位搜索
                            break;
                        case 3:
                            vwproject = vwproject.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                            break;
                        case 4:
                      
                            vwproject = vwproject.Where(ad => ad.projectNo.ToString().Contains(SearchString));//根据工程序号搜索
                            break;
                        case 5:

                            vwproject = vwproject.Where(ad => ad.contractNo.ToString().Contains(SearchString));//根据责任书编号搜索
                            break;

                    }

               
            }
            if (id2== "2")
            {
                vwproject = from a in vwproject
                            where a.isCharge ==false
                            orderby a.projectNo descending
                            select a;
                
                    switch (t)
                    {
                        case 0:
                            vwproject = vwproject.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                            break;
                        case 1:

                            vwproject = vwproject.Where(ad => ad.location.Contains(SearchString));//根据地点搜索
                            break;
                        case 2:

                            vwproject = vwproject.Where(ad => ad.developmentOrganization.Contains(SearchString));//根据建设单位搜索
                            break;
                        case 3:
                            vwproject = vwproject.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                            break;
                        case 4:
                         
                            vwproject = vwproject.Where(ad => ad.projectNo.ToString().Contains(SearchString));//根据工程序号搜索
                            break;
                        case 5:

                            vwproject = vwproject.Where(ad => ad.contractNo.ToString().Contains(SearchString));//根据责任书编号搜索
                            break;

                    }

                
            }
            vwproject = vwproject.OrderByDescending(s => s.projectNo);
            return JsonConvert.SerializeObject(vwproject);
        }
        public ActionResult ChargeItem(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            long ID = long.Parse(id.Trim());
            var test = from ad in db.vw_projectProfile
                       where ad.projectID ==ID
                       select ad;
            vw_projectProfile charge = test.First();
            long projectNo = Convert.ToInt32(charge.projectNo);
            var ch = from c in db.vw_charge
                     where c.searchNo == projectNo && c.fromDepartment == "1"
                     select c;
            //费用转向
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "转向复印室", Value = "0"},
                new SelectListItem { Text = "转向财务科", Value = "1"},
               };

            ViewBag.whereTransfer = new SelectList(list, "Value", "Text");
            if (charge.buildingArea == null)
            {
                charge.buildingArea = 0;
            }
            //初始化，设置相关div的可用性
            ViewData["quedingtext"] = "确定";
            ViewData["seqNodisplay"] = "none";
            ViewData["zhenglidisplay"] = "display";
            ViewData["zhengli"] = true;
            ViewData["Departdisplay"] = "none";
            ViewData["Gongfenshudisplay"]= "none";
            ViewData["Urbandisplay"]= "none";
            if (ch.Count() != 0)//费用已收
            {
                ViewData["checkname"] = 1;//说明此工程费用已经收取

                if (ch.First().chargeExtra == "1")//1.档案馆整理，2报送单位整理
                {
                    ViewBag.radiobutton = 1;
                    ViewData["Urbansettle"] = ch.First().totalExpense;
                    ViewData["zhenglidisplay"] = "none";
                    ViewData["Urbandisplay"] = "display";
                }
                else
                {
                    ViewBag.radiobutton = 2;
                    ViewData["Departdisplay"] = "display";
                    ViewData["Gongfenshudisplay"] = "display";
                    ViewData["Gongfenshu"] = ch.First().centiCnt.ToString();
                    ViewData["Department"] = ch.First().totalExpense;
                }

                if (ch.First().isCharge == true)//财务科已收费，不允许业务科再修改
                {
                    ViewData["queding"] = true;

                }
                else//业务科可以修改
                {
                    //ViewData["queding"] = false;
                    ViewData["quedingtext"] = "修改";
                    ViewData["seqNodisplay"] = "display";
                    ViewData["seqNovalue"] = ch.First().seqNo;
                }
                ViewData["txtremarx"] = ch.First().remarks;

            }
            ////显示档案馆整理费zhoulin20170527
            //if (charge.buildingArea<3000)
            //{
            //    ViewData["Urbansettle"] = 3000;
            //}
            //else
            //{
            //    ViewData["Urbansettle"] = charge.buildingArea;
            //}
            //ViewData["Department"] = 3000;


           




            return View(charge);
        }
        [HttpPost]
        public ActionResult ChargeItem(long projectID, long projectNo, string projectName, string developmentOrganization, string submitPerson, double buildingArea, string recipient,string radiobutton,string zhengli,string Department,string cent,string Urbansettle, string txtremarx,string whereTransfer, string action)
        {
            if (action == "返回")
            {
                return RedirectToAction("projectchargeindex", "ProjectCharge");
            }
            
            var paperArchive = from a in db.PaperArchives
                               where a.projectID == projectID
                               select a;
            var projectInfo = from b in db.ProjectInfo
                              where b.projectID == projectID
                              select b;

            var charge = from c in db.vw_charge
                         where c.searchNo == projectNo && c.fromDepartment == "1"
                         select c;
            

            if (buildingArea.ToString() == string.Empty)
            {
                buildingArea = 0.0f;
            }
            if (action == "确定")
            {
                if (radiobutton == null || radiobutton == "")
                {
                    return Content("<script >alert('请选择收费类型！！');window.history.back();</script >");
                }
                //相关变量的定义
                DateTime chargeTime = DateTime.Today.Date;
                string depart = "1";
                int iType;//收费类型
                string chargeExtra="", chargeDetail="";//1从档案馆整理2从报送单位整理
                double  feeNumbers, feeNumbers2, centiCnt = 0; ;//,实际收费，理论收费,公分数
                if (radiobutton == "1")
                {
                    iType = 2;
                    chargeExtra = "1";
                    buildingArea = (float)1 * buildingArea;
                    feeNumbers2 = buildingArea;
                    feeNumbers = double.Parse(Urbansettle.Trim());
                    chargeDetail = "档案馆整理费（元）" + "：" + feeNumbers;
                }
                else
                {
                    iType = 2;
                    chargeExtra = "2";
                    centiCnt = float.Parse(cent.Trim());
                    centiCnt = 30 * centiCnt;
                    feeNumbers2 = centiCnt;
                    feeNumbers = double.Parse(Department.Trim());
                    chargeDetail = "报送单位整理费（元）" + "：" + feeNumbers;
                }
                if (charge.Count()==0)
                {
                    string result;
                    

                    if (buildingArea == 0)
                    {
                        result = InsertFeesListremarks(projectNo, projectName, submitPerson, 0, recipient, depart, iType, feeNumbers, chargeTime, txtremarx, chargeDetail, chargeExtra, whereTransfer, centiCnt, feeNumbers2);
                    }
                    else
                    {
                        result = InsertFeesListremarks(projectNo, projectName, submitPerson, buildingArea, recipient, depart, iType, feeNumbers, chargeTime, txtremarx, chargeDetail, chargeExtra, whereTransfer, centiCnt, feeNumbers2);
                    }
                    if (result.IndexOf("error") == -1)
                    {
                        SetProjectCharge(projectNo, result);// 科室不负责收费，在财务处负责
                    }
                    db.SaveChanges();
                   
                }
                else
                {
                    if(charge.First().isCharge!=true)//修改
                    {
                        charge.First().chargeClassify = iType;
                        charge.First().totalExpense = Convert.ToDecimal(feeNumbers); 
                        charge.First().chargeDetail = chargeDetail;
                        charge.First().chargeExtra = chargeExtra;
                        charge.First().@operator = recipient;
                        charge.First().remarks = txtremarx;
                        charge.First().itemName = projectName;
                        charge.First().theoryExpense = Convert.ToDecimal(feeNumbers2); 
                        charge.First().searchNo = projectNo;
                        charge.First().whereTransfer = Int32.Parse(whereTransfer);
                        db.Entry(charge.First()).State = EntityState.Modified;
                        SetProjectCharge(projectNo, charge.First().seqNo);
                        db.SaveChanges();
                    }
                }
                return Content("<script >alert('请到财务科缴费！');window.location.href='/ProjectCharge/projectchargeindex';</script >");
            }


        
            return View(charge);
        }
        public string InsertFeesListremarks(long projectNo, string projectName, string submitPerson, double buildingArea, string recipient, string depart ,int chargetype, double total, DateTime chargetime, string txtremarx,string chargeDetail, string chargeExtra,string whereTransfer,double centiCnt,double feeNumbers2)
        {

            string seqNo ="";
            var ch = from a in db.Charger
                     orderby a.ID descending
                     select a;

            string seqNostr = string.Empty;
            try
            {
              
                Charger charge = new Charger();
                charge.charger1 = "";
                charge.ID = Convert.ToInt32(ch.First().ID) + 1;
                charge.searchNo = projectNo;
                charge.itemName = projectName;
                charge.unitName = submitPerson;
                charge.buildingArea = buildingArea;
                charge.fromDepartment = depart;
                charge.chargeExtra = chargeExtra;//1：档案馆整理费2：报送单位整理费
                charge.chargeDetail = chargeDetail;
                charge.chargeClassify = chargetype;
                charge.totalExpense = Convert.ToDecimal(total);
                charge.theoryExpense = Convert.ToDecimal(feeNumbers2);
                charge.isCharge = false;
                charge.chargeTime = chargetime;
                charge.remarks = txtremarx;
                charge.whereTransfer = Int32.Parse(whereTransfer.Trim());
                charge.centiCnt = centiCnt;
                charge.isBack = false;
                charge.backNote = string.Empty;
                charge.@operator = recipient;
                //增加一条数据,自动添加财务编号
                int id = GetMaxId();
                charge.ID = id;
               seqNostr = getCurDayMaxSeqNo();
               seqNo = seqNostr;
               charge.seqNo = seqNostr;
               charge.backNote = "";
           //增加一条记录并返回最大收费编号
                if (ModelState.IsValid)
                {

                    db.Charger.Add(charge);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                seqNo = "error" + e.Message;
            }
            return seqNo;
        }
        public string getCurDayMaxSeqNo()
        {
            string sn = "";
            string seqNo = "";
            string CurDate = DateTime.Now.Year.ToString() + DateTime.Now.Date.Month.ToString("D2") + DateTime.Now.Date.Day.ToString("D2");
            //在BorrowRegistration表中查询含有当前日期的最大seqNo
            var seq = from a in db.BorrowRegistration
                      where a.seqNo.Contains(CurDate)
                      orderby a.seqNo descending
                      select a.seqNo;
            if (seq.Count() == 0)
            {



                sn = CurDate + "000";

            }
            else
            {
                sn = seq.First().ToString();
            }
            int lst3 = Int32.Parse(sn.Substring(sn.Length - 3, 3)) + 1;

            int maxLast3Bit = 1;
            var model = from a in db.Charger
                        where a.seqNo.Contains(CurDate)
                        orderby a.ID
                        select a;
            if (model.Count() != 0)
            {

                foreach (var dr in model)
                {
                    string last3Bit = dr.seqNo.ToString();
                    last3Bit = last3Bit.Substring(last3Bit.Length - 3, 3);
                    if (Int32.Parse(last3Bit) > maxLast3Bit)
                    {
                        maxLast3Bit = Int32.Parse(last3Bit);
                    }
                }
                maxLast3Bit += 1;
            }
            maxLast3Bit = maxLast3Bit > lst3 ? maxLast3Bit : lst3;
            seqNo = CurDate + maxLast3Bit.ToString("D3");

            return seqNo;
        }
        public void SetProjectCharge(long projectNo, string seqNo)
        {
            //更新projectInfose和paperArchive表的相关记录
            var Pro = from a in db.PaperArchives
                      where a.projectNo == projectNo
                      select a;
            long projectid = Convert.ToInt32(Pro.First().projectID);
            var projectinfo = from b in db.ProjectInfo
                              where b.projectID == projectid
                              select b;
            ProjectInfo model = projectinfo.First();

            if (model != null)
            {
                model.isCharge = true;
                model.isFinanceCharge = false;
                model.seqNo = seqNo;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
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
    }
}
