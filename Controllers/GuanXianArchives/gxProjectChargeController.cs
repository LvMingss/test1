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
    public class gxProjectChargeController : Controller
    {
        private gxArchivesContainer ab = new gxArchivesContainer();
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
                var ds = db.gridview1.Where(ad => ad.chargeTime >= DataFrom).Where(ad => ad.chargeTime <= DataTo);
                //var ds = db.vw_projectList.Where(ad => ad.paperProjectSeqNo > 20).Where(ad => ad.paperProjectSeqNo < 40);
                localReport.ReportPath = Server.MapPath("~/Report/jungong/xiangxishoufei.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("xiangxishoufei", ds);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().Trim()));
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
        // GET: ProjectCharge/Details/5
        public ActionResult Details(int ?id,string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var test = from ad in ab.vw_gxprojectProfile
                       where (ad.projectID == id)
                       select ad;
            vw_gxprojectProfile gxprojectProfile = test.First();

            if (gxprojectProfile == null)
            {
                return HttpNotFound();
            }
            if (action == "返回")
            {
                return RedirectToAction("projectchargeindex", "gxProjectCharge");
            }
            return View(gxprojectProfile);
         

        }

        // GET: ProjectCharge/Create
        public ActionResult Create(string SearchString)
        {
            ViewData["pagename"] = "ProjectCharge-Create";
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "事项名称", Value = "0"},
                new SelectListItem { Text = "收费单位", Value = "1"},
                new SelectListItem { Text = "收费日期", Value = "2" },
                new SelectListItem { Text = "编号", Value = "3" },
               
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            string n = Request.Form["SelectedID"];

            ViewBag.CurrentFilter = SearchString;
            //var depart = from a in db.DepartmentCode
            //             where a.text == User.Identity.Name
            //             select a.value;
            //jsy修改
            var UserID = User.Identity.GetUserId();
            var department = user.AspNetUsers.Find(UserID).DepartmentName;
            var vwprojectcharge = from ad in db.vw_charge
                                  where ad.fromDepartment=="7"
                                  select ad;
                                       //jsy修改根据登陆用户查询记录
            if (!String.IsNullOrEmpty(SearchString))
            {
                int t = int.Parse(n);
                ViewBag.SelectedID = new SelectList(list, "Value", "Text",n);
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
                        vwprojectcharge = vwprojectcharge.Where(ad => ad.chargeTime == date);//根据收费日期搜索
                        break;
                    case 3:
                        vwprojectcharge = vwprojectcharge.Where(ad => ad.seqNo == SearchString);//根据编号搜索
                        break;
                }

            }
            vwprojectcharge = vwprojectcharge.OrderByDescending(s => s.seqNo);// 默认按收费编号排
            ViewBag.result = JsonConvert.SerializeObject(vwprojectcharge);
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
                if (charge.chargeExtra.Trim() == "1" || charge.chargeExtra == "")
                {
                    charge.chargeExtra = "1";
                }

                //相关控件的初始化
                ViewBag.chargeClassify = new SelectList(db.ChargeType, "value", "text", charge.chargeClassify);
                ViewBag.fromDepartment = new SelectList(db.DepartmentCode, "value", "text", charge.fromDepartment);
                ViewBag.operator1 = new SelectList(user.AspNetUsers, "UserName", "UserName", charge.@operator);
                //费用转向
                List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "转向复印室", Value = "0"},
                new SelectListItem { Text = "转向财务科", Value = "1"},
               };
              
                ViewBag.whereTransfer = new SelectList(list, "Value", "Text", charge.whereTransfer);
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
        
        public ActionResult projectchargeindex(string SearchString,int? SelectedID)
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
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            string n = Request.Form["SelectedID"];

            ViewBag.CurrentFilter = SearchString;
            var gxproject = from ad in ab.vw_gxprojectProfile
                            where ad.isNB=="外部"
                                  select ad;


            if (!String.IsNullOrEmpty(SearchString))
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);
                int? t = SelectedID;
                switch (t)
                {
                    case 0:
                        gxproject = gxproject.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 1:

                        gxproject = gxproject.Where(ad => ad.location.Contains(SearchString));//根据地点搜索
                        break;
                    case 2:

                        gxproject = gxproject.Where(ad => ad.developmentOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        gxproject = gxproject.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                        break;
                    case 4:
                        long search = Convert.ToInt32(SearchString);
                        gxproject = gxproject.Where(ad => ad.projectNo == search);//根据工程序号搜索
                        break;
                    case 5:

                        gxproject = gxproject.Where(ad => ad.contractNo== SearchString);//根据责任书编号搜索
                        break;

                }

            }
            gxproject = gxproject.OrderByDescending(s => s.projectID);// 默认按项目顺序号排列
            ViewBag.result = JsonConvert.SerializeObject(gxproject);

            return View();

        
        }
        public ActionResult ChargeItem(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var test = from ad in ab.vw_gxprojectProfile
                       where ad.projectID == id
                       select ad;
            vw_gxprojectProfile charge = test.First();
            long projectNo = Convert.ToInt32(charge.projectID);
            var ch = from c in db.vw_charge
                     where c.searchNo == projectNo && c.fromDepartment == "7"
                     select c;
            //费用转向
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "转向复印室", Value = "0"},
                new SelectListItem { Text = "转向财务科", Value = "1"},
               };
            var classid = test.First().classifyID.Trim();
            ViewBag.whereTransfer = new SelectList(list, "Value", "Text","1");
            ViewBag.classname = ab.gxClassType.Where(a => a.classTypeID == classid).First().classTypeName;
            if (charge.buildingArea == null)
            {
                charge.buildingArea = 0;
            }
            //显示档案馆整理费
            if (charge.buildingArea < 3000)
            {
                ViewData["Urbansettle"] = 3000;
            }
            else
            {
                ViewData["Urbansettle"] = charge.buildingArea;
            }
            ViewData["Department"] = 3000;


            if (ch.Count()!=0)//费用已收
            {
                ViewData["checkname"] = 1;//说明此工程费用已经收取
               
                if (ch.First().chargeExtra =="1")//1.档案馆整理，2报送单位整理
                {
                    ViewBag.radiobutton =1;
                    ViewData["Urbansettle"] = ch.First().totalExpense;
                }
                else
                {
                    ViewBag.radiobutton = 2;
                    ViewData["Department"]= ch.First().totalExpense;
                }
                
                if (ch.First().isCharge == true)//财务科已收费，不允许业务科再修改
                {
                    ViewData["queding"] = true;
                    
                }
                else//业务科可以修改
                {
                    ViewData["queding"] = false;
                    
                }
                ViewData["txtremarx"] = ch.First().remarks;
                
            }




            return View(charge);
        }
        [HttpPost]
        public ActionResult ChargeItem(long projectID, long projectNo, string projectName, string developmentOrganization, string submitPerson, double buildingArea, string recipient, string radiobutton, string zhengli, string Department, string cent, string Urbansettle, string txtremarx, string whereTransfer, string action)
        {
            if (action == "返回")
            {
                return RedirectToAction("projectchargeindex", "gxProjectCharge");
            }

            var paperArchive = from a in ab.gxPaperArchives
                               where a.projectID == projectID
                               select a;
            var projectInfo = from b in ab.gxProjectInfo
                              where b.projectID == projectID
                              select b;

            var charge = from c in db.vw_charge
                         where c.searchNo == projectNo && c.fromDepartment == "7"
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
                string depart = "7";
                int iType;//收费类型
                string chargeExtra = "", chargeDetail = "";//1从档案馆整理2从报送单位整理
                double feeNumbers, feeNumbers2, centiCnt = 0; ;//,实际收费，理论收费,公分数
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
                if (charge.Count() == 0)
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
                    ab.SaveChanges();

                }
                else
                {
                    if (charge.First().isCharge != true)//修改
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
                return Content("<script >alert('请到财务科缴费！');window.location.href='/gxProjectCharge/projectchargeindex';</script >");
            }



            return View(charge);
        }
        public string InsertFeesListremarks(long projectID, string projectName, string submitPerson, double buildingArea, string recipient, string depart ,int chargetype, double total, DateTime chargetime, string txtremarx,string chargeDetail, string chargeExtra,string whereTransfer,double centiCnt,double feeNumbers2)
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
                charge.searchNo = projectID;
                charge.itemName = projectName;
                charge.unitName = projectName;
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
        private string getCurDayMaxSeqNo()
        {
            string seqNo = "";
            string CurDate = DateTime.Now.Year.ToString() + DateTime.Now.Date.Month.ToString("D2") + DateTime.Now.Date.Day.ToString("D2");
            string sn;
            //查询最大的收费编号在BorrowRegistration表中
            var ibor = from a in db.BorrowRegistration
                       where a.seqNo.Contains(CurDate)
                       select a;
            if (ibor.Count() == 0)
            {
                sn = CurDate + "000";
            }
            else
            {
                sn = ibor.First().seqNo;
            }

            long lst3 = long.Parse(sn.Substring(sn.Length - 3, 3)) + 1;
            long maxLast3Bit = 1;
            //查询含有CurDate的Charge表中的Seqno字段的所有记录
            var charge = from b in db.Charger
                         where b.seqNo.Contains(CurDate)
                         orderby b.ID
                         select b;
            if (charge.Count() != 0)
            {

                foreach (var item in charge)
                {
                    string last3Bit = item.seqNo.ToString().Trim();
                    last3Bit = last3Bit.Substring(last3Bit.Length - 3, 3);
                    if (Int32.Parse(last3Bit) > maxLast3Bit)
                    {
                        maxLast3Bit = long.Parse(last3Bit);
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
            var Pro = from a in ab.gxPaperArchives
                      where a.projectNo == projectNo
                      select a;
            long projectid = Convert.ToInt32(Pro.First().projectID);
            var projectinfo = from b in ab.gxProjectInfo
                              where b.projectID == projectid
                              select b;
            gxProjectInfo model = projectinfo.First();

            if (model != null)
            {
                model.isCharge = true;
                model.isFinanceCharge = false;
                model.seqNo = seqNo;
                ab.Entry(model).State = EntityState.Modified;
                ab.SaveChanges();
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
