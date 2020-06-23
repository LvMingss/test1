using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;

using System.Web.Mvc;

using PagedList;

using urban_archive.Models;
using System.Net;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using Microsoft.Reporting.WebForms;

using System.Drawing;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using iTextSharp.text;
using iTextSharp.text.pdf;



namespace urban_archive.Controllers
{
    public class UrbanBorrowController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        private VideoArchiveEntities db_video = new VideoArchiveEntities();
        private UrbanUsersEntities us = new UrbanUsersEntities();
        // GET: UserBorrow
        public ActionResult UserBorrow( string currentFilter, string SearchString, int? SelectedID,string action)
        {
            ViewData["pagename"] = "UrbanBorrow/UserBorrow";

            if (action=="登记新用户")
            {
                return RedirectToAction("RegisUser", "UrbanBorrow");
            }
            if(action=="打印登记用户")
            {

            }
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "借阅人姓名", Value = "0"},
                new SelectListItem { Text = "借阅单位", Value = "1"},
                new SelectListItem { Text = "案卷", Value = "2" },
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            int t = SelectedID.GetValueOrDefault();

            ViewBag.CurrentFilter = SearchString;
            var borrow = from ad in db.BorrowRegistration
                                select ad;


            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (t)
                {
                    case 0:
                        borrow = borrow.Where(ad => ad.borrower.Contains(SearchString));//根据借阅者姓名搜索
                        break;
                    case 1:
                        borrow = borrow.Where(ad => ad.borrowUnit.Contains(SearchString));//根据借阅单位搜索
                        break;
                    case 2:
                        borrow = borrow.Where(ad => ad.archiveSerialNo.Contains(SearchString));//根据案卷搜索
                        break;

                }

            }
            // 默认按借阅编号排
            borrow = borrow.OrderByDescending(s => s.borrowSeqNo).Take(2000);
            ViewBag.result = JsonConvert.SerializeObject(borrow);
            return View();

        }

        // GET: UserBorrow/Details/5
        public ActionResult Details(long? id,string id1)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string borr = id.ToString().Trim();
            var borrow = from a in db.BorrowRegistration
                         where a.borrowSeqNo==borr
                         select a;
            var borrow1 = from b in db.BorrowRegistration
                          orderby b.ID descending
                          select b;
            BorrowRegistration BorrowRegis = borrow.First();
            long ID1 = borrow.First().ID;
            var BindReceive = from b in db.BindUserAndArchives
                              where b.userID == ID1
                              select b;
            //判断是否是第一条记录或是最后一条记录
            long MaxID = borrow1.First().ID;
            long MinID = 1;
            if(id== MinID)
            {
                ViewData["Next"] = true;
            }
            if(id==MaxID)
            {
                ViewData["Pre"] = true;
            }
            //if (BindReceive.Count() != 0)//获取调卷数
            //{
            //    BorrowRegis.consultVolumeCount = BindReceive.Count().ToString();
            //    BorrowRegis.precisionVolumeCount = BindReceive.Count().ToString();
            //    //BorrowRegis.archiveSerialNo = getSeriNo(id);
            
            //}
            //费用是否收取
            if (BorrowRegis.chargeFlag== "False")
            {

                ViewBag.ShouFei=true;

            }

            else
            {
                ViewBag.ShouFei = false;

            }

            if (BorrowRegis.application1 == true)
            {
                ViewBag.PlantAndDesign = true;
            }
            if (BorrowRegis.application2 == true)
            {
                ViewBag.ConstAndManage = true;

            }
            if (BorrowRegis.application3 == true)
            {
                ViewBag.MarkAndRegis = true;

            }
            if (BorrowRegis.application4 == true)
            {
                ViewBag.SettleAndArgue = true;
            }
            if (BorrowRegis.application5 == true)
            {

                ViewBag.Reasearch = true;
            }
            if (BorrowRegis.application6 == true)
            {
                ViewBag.Others = true;

            }
            ViewBag.Use_UseTu = BorrowRegis.Use_UseTu;
            //判断复选框的值-目的
            if (BorrowRegis.goal1 == true)
                ViewBag.MakeHistory = true;
            if (BorrowRegis.goal2 == true)
                ViewBag.WorkSurvice = true;

            if (BorrowRegis.goal3 == true)
                ViewBag.Research = true;

            if (BorrowRegis.goal4 == true)
                ViewBag.Finance = true;

            if (BorrowRegis.goal5 == true)
                ViewBag.Education = true;

            if (BorrowRegis.goal6 == true)
                ViewBag.Others1 = true;
            //判断复选框的值-效果
            if (BorrowRegis.userEffects1 == true)
                ViewBag.Effect1 = true;

            if (BorrowRegis.userEffects2 == true)
                ViewBag.Effect2 = true;

            if (BorrowRegis.userEffects3 == true)
                ViewBag.Effect3 = true;

            if (BorrowRegis.userEffects4 == true)
                ViewBag.Effect4 = true;

            if (BorrowRegis.userEffects5 == true)
                ViewBag.Others2 = true;
            ViewBag.Use_MuDi = BorrowRegis.Use_MuDi;

            //用户绑定查阅案卷
            ViewData["checkname"] = 0;
            ViewData["url1"] =id1;
            //应交费用计算

            decimal str1 = 0, str2 = 0, str3 = 0;
            if (BorrowRegis.paoKufangRen.Trim()!=""&& BorrowRegis.paoKufangRen.Trim()!=null)
            {              
                str3 = Convert.ToDecimal(BorrowRegis.paoKufangRen.Trim());
            }
            str1 = Convert.ToDecimal(BorrowRegis.certificationFee);
            str2 = Convert.ToDecimal(BorrowRegis.consultFee);
            decimal YjFee = str1 + str2 + str3;
            ViewData["YJfee"] = YjFee.ToString("0.00");
            //借阅人需要打印图纸
     
            DateTime time = DateTime.Now.Date;
            var model = from a in db.BorrowRegistration
                        where a.ID ==ID1
                        select a;
            var model1 = from b in db.BindUserAndImage
                         select b;
            if (model.Count() != 0)
            {
                time = Convert.ToDateTime(model.First().borrowDate);
                if(!string.IsNullOrEmpty(model.First().seqNo.Trim()))
                {
                    string str = model.First().seqNo.Trim();
                    model1 = from b in db.BindUserAndImage
                         where b.imageTime == time && b.userID==str
                             orderby b.ID
                         select b;
                }
                else
                {
                    model1 = from b in db.BindUserAndImage
                             where b.imageTime == time && b.realuserID==ID1
                             orderby b.ID
                             select b;
                }
                int i = 1;
                foreach (var item in model1)
                {

                    item.recordID = i;
                    i = i + 1;
                    string address = item.ImageAddress.ToString().Trim();
                    string[] add = null;
                    if (address != "")
                    {
                       
                        if(address.IndexOf('/')!=-1)
                        {
                           add = address.Split(new string[] { "/" }, StringSplitOptions.None);
                        }
                        else
                        {
                            add = address.Split(new string[] { "//" }, StringSplitOptions.None);
                        }
                    }
                        
                        item.ImageAddress = add[add.Length - 1];
                    }

                }
                if (model1.Count() == 0)
                {
                    ViewData["checkname"] = 1;
                }
            ViewBag.ID1 = ID1;
            ViewBag.result1=JsonConvert.SerializeObject(model1);
            return View(BorrowRegis);
            

        }
        [HttpPost]
        public ActionResult Details(long ID1,string action,string url1)
        {
            var borrow1 = from b in db.BorrowRegistration
                          orderby b.ID descending
                          select b;
            var borrow2 = from c in db.BorrowRegistration
                          where c.ID == ID1
                          select c;
            //判断是否是第一条记录或是最后一条记录

            long MaxID = borrow1.First().ID;
            long MinID = 1;
           
            if (action=="前一个")
            {
                if (ID1 == 13884)//因数据库主键不断更新的缘故，中间有缺失的ID，因此采用此方法
                {
                    ID1 = 13901;
                }
                else
                {
                    ID1 = ID1 + 1;
                }

            }
            if(action=="后一个")
            {
                if (ID1 == 13901)
                {
                    ID1 = 13884;
                }
                else
                {
                    ID1 = ID1 - 1;
                }
            }
            if (action == "返回")
            {
                if (url1 == "1")
                {
                    //Response.Write("<script language='javascript'>history.go(-1);</script>");
                    //return Content("<script>history.back(-1);</script >");
                    return RedirectToAction("UserBorrow",new { id1 = url1});

                }
                if(url1 == "2")
                {
                    return Redirect("/BorrowFeeDetail/BorrowFeeDetail");         
                }
                
            }

            if (ID1 <=MinID)
            {
                return Content("<script >alert('已经是第一条');window.history.back();</script >");
            }
            if (ID1 >=MaxID)
            {
                return Content("<script >alert('已经是最后一条');window.history.back();</script >");
            }
            return RedirectToAction("Details", new { id =borrow2.First().borrowSeqNo.Trim(), id1=url1});
        }
        // GET: UserBorrow/Create
        public ActionResult RegisUser()
        {
            
            string str = DateTime.Today.Year.ToString();
            var borrow = from a in db.BorrowRegistration
                         where a.borrowSeqNoN.Substring(0,4).Contains(str)
                         orderby a.borrowSeqNoN descending
                         select a;
            BorrowRegistration borrow1 = new BorrowRegistration();
            long maxcur;
            if (borrow.Count() == 0)
            {
                string str1 = str + "00000";
                maxcur = Convert.ToInt32(str1.Trim());
                //    var borrow2 = from a in db.BorrowRegistration
                //                 orderby a.ID descending
                //                 select a;
                //    maxcurID = borrow2.First().ID + 1;
            }
            else if (borrow.First().borrowSeqNoN.Length!=9)
            {



                string str1 = str + "00000";
                maxcur = Convert.ToInt32(str1.Trim());



            }

            else
            { 
                 maxcur = Convert.ToInt32(borrow.First().borrowSeqNoN) + 1;
                //var borrow2 = from a in db.BorrowRegistration
                //              orderby a.ID descending
                //              select a;
                //maxcurID = borrow2.First().ID + 1;
            }
             
            int temp = Int32.Parse(maxcur.ToString().Substring(4, maxcur.ToString().Length - 4));
            borrow1.borrowSeqNoN = DateTime.Now.Year.ToString() + temp.ToString("D5");
            borrow1.borrowDate = DateTime.Today;
            //borrow1.ID = maxcurID;
            //默认个人
            borrow1.singleOrDepart = "个人";
            return View(borrow1);
        }

        // POST: UserBorrow/Create
        [HttpPost]
        public ActionResult RegisUser(long ID,string borrower,string consultFilePersonTime,string borrowDate,string borrowerTel,string borrowSeqNoN,string borrowUnit,string singleOrDepart,bool PlantAndDesign,bool ConstAndManage,bool MarkAndRegis,bool SettleAndArgue,bool Reasearch,bool Others,bool MakeHistory,bool WorkSurvice,bool Research,bool  Finance,bool Education,bool Others1,string Use_UseTu,string Use_MuDi,/*bool Effect1,bool Effect2,bool Effect3,bool Effect4,bool Others2,*/string action)
        {
            if (action == "登记")
            {
                string strBorrower = borrower.Trim();//
                if (strBorrower == "" || strBorrower == null)
                {
                    return Content("<script >alert('借阅人不能为空！');window.history.back();</script >");

                }
                string strBorrowTime = borrowDate.Trim();//
                if (strBorrowTime == "" || strBorrowTime == null)
                {
                    return Content("<script >alert('借阅日期不能为空！');window.history.back();</script >");

                }
                string strBorrowTel = borrowerTel.Trim();//
                if (strBorrowTel == "" || strBorrowTel == null)
                {
                    return Content("<script >alert('联系电话不能为空！');window.history.back();</script >");

                }
                if (strBorrowTel.Length < 7)
                {
                    return Content("<script >alert('联系电话位数不得少于7位！');window.history.back();</script >");

                }
                string strBorrowUnit = borrowUnit.Trim();//
                if (strBorrowUnit == "")
                {
                    return Content("<script >alert('家庭地址不能为空！');window.history.back();</script >");

                }
                string strConArchPerTime = consultFilePersonTime.Trim();//
                if (strConArchPerTime == "")
                {
                    return Content("<script >alert('查档人次不能为空！');window.history.back();</script >");

                }

                if (PlantAndDesign == false && ConstAndManage == false && MarkAndRegis == false && SettleAndArgue == false && Reasearch == false && Others == false)
                {
                    return Content("<script >alert('请选择用途！');window.history.back();</script >");

                }

                if (MakeHistory == false && WorkSurvice == false && Research == false && Finance == false && Education == false && Others1 == false)
                {
                    return Content("<script >alert('请选择利用档案目的！');window.history.back();</script >");


                }

                //if (Effect1 == false && Effect2 == false && Effect3 == false && Effect4 == false && Others2 == false)
                //{
                //    return Content("<script >alert('请选择利用效果！');window.history.back();</script >");

                //}
                var checkborrow = from b in db.BorrowRegistration
                                  where b.borrower == borrower && b.borrowDate.ToString() == borrowDate.Trim()
                                  select b;
                if (checkborrow.Count() == 0)
                {
                    //try
                    //{
                    BorrowRegistration borrow = new BorrowRegistration();
                    //判断复选框的值-用途
                    string strMeaning = "";
                    if (PlantAndDesign == true)
                    {
                        borrow.application1 = true;
                        strMeaning += "规划设计，";
                    }
                    else
                        borrow.application1 = false;
                    if (ConstAndManage == true)
                    {
                        borrow.application2 = true;
                        strMeaning += "施工管理，";
                    }
                    else
                        borrow.application2 = false;
                    if (MarkAndRegis == true)
                    {
                        borrow.application3 = true;
                        strMeaning += "房产土地登记，";
                    }
                    else
                        borrow.application3 = false;
                    if (SettleAndArgue == true)
                    {
                        borrow.application4 = true;
                        strMeaning += "解决纠纷，";
                    }
                    else
                        borrow.application4 = false;
                    if (Reasearch == true)
                    {
                        borrow.application5 = true;
                        strMeaning += "史志科研，";
                    }
                    else
                        borrow.application5 = false;
                    if (Others == true)
                    {
                        borrow.application6 = true;
                        strMeaning += "其它";
                    }
                    else
                        borrow.application6 = false;
                    borrow.application7 = false;
                    borrow.application8 = false;
                    borrow.operator2 = strMeaning;

                    //判断复选框的值-目的
                    if (MakeHistory == true)
                        borrow.goal1 = true;
                    else
                        borrow.goal1 = false;
                    if (WorkSurvice == true)
                        borrow.goal2 = true;
                    else
                        borrow.goal2 = false;
                    if (Research == true)
                        borrow.goal3 = true;
                    else
                        borrow.goal3 = false;
                    if (Finance == true)
                        borrow.goal4 = true;
                    else
                        borrow.goal4 = false;
                    if (Education == true)
                        borrow.goal5 = true;
                    else
                        borrow.goal5 = false;
                    if (Others1 == true)
                        borrow.goal6 = true;
                    else
                        borrow.goal6 = false;
                    borrow.goal7 = false;
                    borrow.goal8 = false;
                    borrow.Use_UseTu = Use_UseTu;
                    borrow.Use_MuDi = Use_UseTu;
                    //判断复选框的值-效果
                    //if (Effect1 == true)
                    //    borrow.userEffects1 = true;
                    //else
                    //    borrow.userEffects1 = false;
                    //if (Effect2 == true)
                    //    borrow.userEffects2 = true;
                    //else
                    //    borrow.userEffects2 = false;
                    //if (Effect3 == true)
                    //    borrow.userEffects3 = true;
                    //else
                    //    borrow.userEffects3 = false;
                    //if (Effect4 == true)
                    //    borrow.userEffects4 = true;
                    //else
                    //    borrow.userEffects4 = false;
                    //if (Others2 == true)
                    //    borrow.userEffects5 = true;
                    //else
                    //    borrow.userEffects5 = false;
                    //borrow.userEffects6 = false;
                    //borrow.userEffects7 = false;

                    borrow.archiveSerialNo = string.Empty;
                    if (strBorrowTime != "")
                        borrow.borrowDate = DateTime.Parse(strBorrowTime);
                    else
                        borrow.borrowDate = DateTime.Parse(System.Data.SqlTypes.SqlDateTime.MinValue.ToString());
                    borrow.borrower = strBorrower;
                    borrow.borrowerTel = strBorrowTel;
                    borrow.borrowUnit = strBorrowUnit;
                    borrow.singleOrDepart = singleOrDepart;//add by niutianbo,date:2016.12.30
                    borrow.certificationFee = 0;
                    borrow.chargeFlag = "False";
                    borrow.consultFee = 0;
                    borrow.consultFilePersonTime = strConArchPerTime;
                    borrow.seqNo = "";
                    borrow.consultVolumeCount = string.Empty;
                    borrow.copyFee = 0;//
                    borrow.copyPageCount = string.Empty;//
                    borrow.operator1 = string.Empty;
                    borrow.operator2 = string.Empty;
                    borrow.remarks = string.Empty;
                    
                    borrow.paoKufangRen = string.Empty;//
                    borrow.precisionVolumeCount = string.Empty;//
                    borrow.isJiesuanFee = 1;
                    borrow.password = "111111";
                    borrow.userEffectDetail = "";
                    borrow.ecnomicBenefit = "";
                    borrow.realFees = 0;
                    var a = from e in db.BorrowRegistration
                            orderby e.ID descending
                            select e;
                    borrow.ID=a.First().ID+1;
                    var f = from g in db.BorrowRegistration
                            where g.borrowSeqNoN == borrowSeqNoN.Trim()
                            select g;
                
               
                    if(f.Count()!=0)//说明该登记号已经保存
                    {
                        int  maxcur = Convert.ToInt32(a.First().borrowSeqNoN.Trim()) + 1;
                        int temp = Int32.Parse(maxcur.ToString().Substring(4, maxcur.ToString().Length - 4));
                        borrowSeqNoN = DateTime.Now.Year.ToString() + temp.ToString("D5");
                    }
                    borrow.borrowSeqNo = borrowSeqNoN.Trim();
                    borrow.borrowSeqNoN = borrowSeqNoN.Trim();
                    db.BorrowRegistration.Add(borrow);

                    db.SaveChanges();




                    return Content("<script >alert('登记成功');location.href='/UrbanBorrow/UserBorrow';</script >");
                    //}
                    //    catch
                    //{
                    //Response.Write("<script language=javascript>alert('登记失败！');;window.history.back();</script>");
                    //}
                }
                else
                {
                    return Content("<script >alert('该借阅人姓名已存在，请输入其他姓名');window.history.back();</script >");
                }
            }
            if(action=="返回")
            {
                return RedirectToAction("UserBorrow");
            }

            return View();
        }

        // GET: UserBorrow/Edit/5
        public ActionResult Edit(long ? id)
        {
            //DateTime time1 = DateTime.Today;
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string borr = id.ToString().Trim();
            var borrow = from a in db.BorrowRegistration
                         where a.borrowSeqNo == borr
                         select a;
            //if (time1!=borrow.First().borrowDate)
            //{
            //    return Content("<script >alert('已过费用修改日期！');window.history.back();</script >");
            //}
            var borrow1 = from b in db.BorrowRegistration
                          orderby b.ID descending
                          select b;
            BorrowRegistration BorrowRegis = borrow.First();
            long ID1 =borrow.First().ID;
            var BindReceive = from b in db.BindUserAndArchives
                              where b.userID ==ID1
                              select b;
            //费用转向控件
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "转向复印室", Value = "0"},
                new SelectListItem { Text = "转向财务科", Value = "1"},
               };
            ViewBag.WhereTransform = new SelectList(list, "Value", "Text");
            //费用是否收取
            if (BorrowRegis.chargeFlag == "False")
            {

                ViewBag.ShouFei = true;

            }

            else
            {
                ViewBag.ShouFei = false;

            }
            //判断是否是第一条记录或是最后一条记录
            long MaxID = borrow1.First().ID;
            long MinID = MaxID - borrow1.Count() + 1;
            if (id == MinID)
            {
                ViewData["Next"] = true;
            }
            if (id == MaxID)
            {
                ViewData["Pre"] = true;
            }
            //if (BindReceive.Count() != 0)//获取调卷数
            //{
            //    BorrowRegis.consultVolumeCount = BindReceive.Count().ToString();
            //    BorrowRegis.precisionVolumeCount = BindReceive.Count().ToString();
                //BorrowRegis.archiveSerialNo = getSeriNo(id);
            //}20171128by zl 因为要手填绑定卷数
            //费用是否收取
            if (BorrowRegis.chargeFlag == "False")
            {

                ViewBag.ShouFei = true;

            }

            else
            {
                ViewBag.ShouFei = false;

            }

            if (BorrowRegis.application1 == true)
            {
                ViewBag.PlantAndDesign = true;
            }
            if (BorrowRegis.application2 == true)
            {
                ViewBag.ConstAndManage = true;

            }
            if (BorrowRegis.application3 == true)
            {
                ViewBag.MarkAndRegis = true;

            }
            if (BorrowRegis.application4 == true)
            {
                ViewBag.SettleAndArgue = true;
            }
            if (BorrowRegis.application5 == true)
            {

                ViewBag.Reasearch = true;
            }
            if (BorrowRegis.application6 == true)
            {
                ViewBag.Others = true;

            }
            //判断复选框的值-目的
            if (BorrowRegis.goal1 == true)
                ViewBag.MakeHistory = true;
            if (BorrowRegis.goal2 == true)
                ViewBag.WorkSurvice= true;

            if (BorrowRegis.goal3 == true)
                ViewBag.Research = true;

            if (BorrowRegis.goal4 == true)
                ViewBag.Finance = true;

            if (BorrowRegis.goal5 == true)
                ViewBag.Education = true;

            if (BorrowRegis.goal6 == true)
                ViewBag.Others1 = true;

            ViewBag.Use_UseTu = BorrowRegis.Use_UseTu;
            ViewBag.Use_MuDi = BorrowRegis.Use_MuDi;
            //判断复选框的值-效果
            if (BorrowRegis.userEffects1 == true)
                ViewBag.Effect1 = true;

            if (BorrowRegis.userEffects2 == true)
                ViewBag.Effect2 = true;

            if (BorrowRegis.userEffects3 == true)
                ViewBag.Effect3 = true;

            if (BorrowRegis.userEffects4 == true)
                ViewBag.Effect4 = true;

            if (BorrowRegis.userEffects5 == true)
                ViewBag.Others2 = true;
            //用户绑定查阅案卷
            ViewData["checkname"] = 0;
            //应交费用计算

            decimal str1 = 0, str2 = 0, str3 = 0;
            if (BorrowRegis.paoKufangRen.Trim() != "" && BorrowRegis.paoKufangRen.Trim() != null)
            {


                str3 = Convert.ToDecimal(BorrowRegis.paoKufangRen.Trim());
            }
            else
            {
                BorrowRegis.paoKufangRen = "0";
            }
            str1 = Convert.ToDecimal(BorrowRegis.certificationFee);
            str2 = Convert.ToDecimal(BorrowRegis.consultFee);
            decimal YjFee = str1 + str2 + str3;
            ViewData["YJfee"] = YjFee.ToString("0.00");
            //借阅人需要打印图纸
            long ID = Convert.ToInt32(id);
            DateTime time = DateTime.Now.Date;
            var model = from a in db.BorrowRegistration
                        where a.borrowSeqNo == borr
                        select a;
            var model1 = from b in db.BindUserAndImage
                         select b;
            if (model.Count() != 0)
            {
                time = Convert.ToDateTime(model.First().borrowDate);
                if (!string.IsNullOrEmpty(model.First().seqNo.Trim()))
                {
                    string str = model.First().seqNo.Trim();
                    model1 = from b in db.BindUserAndImage
                             where b.imageTime == time && b.userID == str
                             orderby b.ID
                             select b;
                }
                else
                {
                    model1 = from b in db.BindUserAndImage
                             where b.imageTime == time && b.realuserID==ID1
                             orderby b.ID
                             select b;
                }
                int i = 1;
                foreach (var item in model1)
                {

                    item.recordID = i;
                    i = i + 1;
                    string address = item.ImageAddress.ToString().Trim();
                    string[] add = null;
                    if (address != "")
                    {

                        if (address.IndexOf('/') != -1)
                        {
                            add = address.Split(new string[] { "/" }, StringSplitOptions.None);
                        }
                        else
                        {
                            add = address.Split(new string[] { "//" }, StringSplitOptions.None);
                        }
                    }

                    item.ImageAddress = add[add.Length - 1];
                }

            }
            if (model1.Count() == 0)
            {
                ViewData["checkname"] = 1;
            }
            ViewBag.ID2 = ID1;
            ViewBag.result1 = JsonConvert.SerializeObject(model1);
            return View(BorrowRegis);
            
        }

        // POST: UserBorrow/Edit/5
        [HttpPost]
        public ActionResult Edit(long? ID2, string borrower, string consultFilePersonTime, string borrowDate, string borrowerTel,string borrowSeqNo, string borrowUnit, string singleOrDepart, string action,bool ShouFei, string archiveSerialNo, string certificationFee, string consultFee, string paoKufangRen, string precisionVolumeCount, string consultVolumeCount, string operator1, string remarks, string TotalFee, string realFees,  string operator2, string WhereTransform, bool PlantAndDesign, bool ConstAndManage, bool MarkAndRegis, bool SettleAndArgue, bool Reasearch, bool Others, bool MakeHistory, bool WorkSurvice, bool Research, bool Finance, bool Education, bool Others1, bool Effect1, bool Effect2, bool Effect3, bool Effect4, bool Others2,string ecnomicBenefit,string userEffectDetail,string Use_UseTu,string Use_MuDi)
        {
            var borrow1 = from b in db.BorrowRegistration
                          orderby b.ID descending
                          select b;
            string borr = borrowSeqNo.Trim();
            var borrow = from a in db.BorrowRegistration
                         where a.borrowSeqNo==borr
                         select a;
            //判断是否是第一条记录或是最后一条记录

            long MaxID = borrow1.First().ID;
            long MinID = 1;
            if (action == "前一个")
            {
                if (ID2 == 13884)//因数据库主键不断更新的缘故，中间有缺失的ID，因此采用此方法
                {
                    ID2 = 13901;
                }
                else
                {
                    ID2 = ID2 + 1;
                }
                if (ID2 >= MaxID)
                {
                    return Content("<script >alert('已经是最后一条');window.history.back();</script >");
                }
            }
            if (action == "后一个")
            {
                if (ID2 == 13901)
                {
                    ID2 = 13884;
                }
                else
                {
                    ID2 = ID2 - 1;
                }
                if (ID2 <= MinID)
                {
                    return Content("<script >alert('已经是第一条');window.history.back();</script >");
                }
            }
            if (action == "返回")
            {
                return RedirectToAction("UserBorrow");
            }
           
           
            //进行数据保存
            if (action=="修改")
            {
                if (borrow.Count() == 0)
                {
                    return Content("<script >alert('数据库中无此人信息');window.history.back();</script >");
                }
                BorrowRegistration model = borrow.First();
                model.borrower = borrower.Trim();
                model.borrowDate = DateTime.Parse(borrowDate.Trim());
                model.borrowerTel = borrowerTel.Trim();
                model.borrowUnit = borrowUnit.Trim();
                model.consultFilePersonTime = consultFilePersonTime.Trim();//      
                model.consultVolumeCount = consultVolumeCount.Trim();
                model.precisionVolumeCount = precisionVolumeCount.Trim();
                //对档案用途、利用效果、利用目的复选框进行判断，如果全部为false则进行提示
                if (PlantAndDesign == false && ConstAndManage == false && MarkAndRegis == false && SettleAndArgue == false && Reasearch == false && Others == false)
                {
                    return Content("<script >alert('请选择用途！');window.history.back();</script >");

                }

                if (MakeHistory == false && WorkSurvice == false && Research == false && Finance == false && Education == false && Others1 == false)
                {
                    return Content("<script >alert('请选择利用档案目的！');window.history.back();</script >");


                }

                if (Effect1 == false && Effect2 == false && Effect3 == false && Effect4 == false && Others2 == false)
                {
                    return Content("<script >alert('请选择利用效果！');window.history.back();</script >");

                }
                //对档案用途、利用效果、利用目的进行保存，并提取相关的字符串
                string strMeaning = "";
                if (PlantAndDesign == true)
                {
                    model.application1 = true;
                    strMeaning += "规划设计，";
                }
                else
                    model.application1 = false;
                if (ConstAndManage == true)
                {
                    model.application2 = true;
                    strMeaning += "施工管理，";
                }
                else
                    model.application2 = false;
                if (MarkAndRegis == true)
                {
                    model.application3 = true;
                    strMeaning += "房产土地登记，";
                }
                else
                    model.application3 = false;
                if (SettleAndArgue == true)
                {
                    model.application4 = true;
                    strMeaning += "解决纠纷，";
                }
                else
                    model.application4 = false;
                if (Reasearch == true)
                {
                    model.application5 = true;
                    strMeaning += "史志科研，";
                }
                else
                    model.application5 = false;
                if (Others == true)
                {
                    model.application6 = true;
                    strMeaning += "其它";
                }
                else
                    model.application6 = false;
                model.application7 = false;
                model.application8 = false;
                model.operator2 = strMeaning;
                //判断复选框的值-目的
                if (MakeHistory == true)
                    model.goal1 = true;
                else
                    model.goal1 = false;
                if (WorkSurvice == true)
                    model.goal2 = true;
                else
                    model.goal2 = false;
                if (Research == true)
                    model.goal3 = true;
                else
                    model.goal3 = false;
                if (Finance == true)
                    model.goal4 = true;
                else
                    model.goal4 = false;
                if (Education == true)
                    model.goal5 = true;
                else
                    model.goal5 = false;
                if (Others1 == true)
                    model.goal6 = true;
                else
                    model.goal6 = false;
                model.goal7 = false;
                model.goal8 = false;
                //判断复选框的值-效果
                if (Effect1 == true)
                    model.userEffects1 = true;
                else
                    model.userEffects1 = false;
                if (Effect2 == true)
                    model.userEffects2 = true;
                else
                    model.userEffects2 = false;
                if (Effect3 == true)
                    model.userEffects3 = true;
                else
                    model.userEffects3 = false;
                if (Effect4 == true)
                    model.userEffects4 = true;
                else
                    model.userEffects4 = false;
                if (Others2 == true)
                    model.userEffects5 = true;
                else
                    model.userEffects5 = false;
                model.userEffects6 = false;
                model.userEffects7 = false;
                model.userEffectDetail = userEffectDetail;
                model.ecnomicBenefit = ecnomicBenefit;
                model.archiveSerialNo = archiveSerialNo.Trim();
                model.Use_MuDi = Use_MuDi;
                model.Use_UseTu = Use_UseTu;
                if (certificationFee.Trim() != "")
                    model.certificationFee = decimal.Parse(certificationFee.Trim());
                else
                    model.certificationFee = 0;
                if (consultFee.Trim() != "")
                    model.consultFee = decimal.Parse(consultFee.Trim());
                else
                    model.consultFee = 0;

                if (paoKufangRen.Trim() != "")
                    model.paoKufangRen = paoKufangRen.Trim();
                else
                    model.paoKufangRen = "0";

                model.operator1 = operator1.Trim();
                model.remarks = remarks.Trim();
                if (ShouFei==true)
                {
                    model.chargeFlag = "True";
                }
                else
                {
                    model.chargeFlag = "False";
                }
                string ID1 = ID2.ToString().Trim();
                model.singleOrDepart = singleOrDepart.Trim();
                if (realFees.Trim() != "")
                    model.realFees = decimal.Parse(realFees.Trim());
                else
                {
                    model.realFees = 0;
                    model.chargeFlag = "False";
                    
                }
                
                    bool isfeemodify;
                    bool flag = updateChargeFee(out isfeemodify,ID1, singleOrDepart, borrower, certificationFee, consultFee, paoKufangRen, borrowUnit, WhereTransform);
                    if (flag == true)
                    {
                        if (isfeemodify)
                        {
                            model.isJiesuanFee = 3;
                        }
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                        return Content("<script >alert('修改成功！');location.href='/UrbanBorrow/UserBorrow';</script >");

                        
                    }
                else
                {
                    
                         return Content("<script >alert('此项费用已经收取，不能进行修改！');window.history.back();</script >");
                }
                
               
            }


            return RedirectToAction("Edit", new { id = borr });
    


        }

        // GET: UserBorrow/Delete/5
        public ActionResult Delete(int id)
        {

            return View();
        }

        // POST: UserBorrow/Delete/5
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
        public ActionResult FeeJieSuan(long? id,string id1)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
         
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "转向复印室", Value = "0"},
                new SelectListItem { Text = "转向财务科", Value = "1"},
               };
             var User1=from b in ab.AspNetUsers
                     where b.RoleName!="外聘人员"&&b.RoleName!="请选择职位"&&b.RoleName!="管理人员"
                     select b;
            string user1 = User.Identity.Name;
            ViewBag.operator2 =  new SelectList(User1, "UserName", "UserName", user1);
            ViewBag.operator3 = user1;
            ViewBag.WhereTransform = new SelectList(list, "Value", "Text");
            string borr = id.ToString().Trim();
            var borrow = from a in db.BorrowRegistration
                         where a.borrowSeqNo==borr
                         select a;
            BorrowRegistration BorrowRegis = borrow.First();
            BorrowRegis.certificationFee =0;
            BorrowRegis.consultFee =0;
            BorrowRegis.paoKufangRen ="";
            BorrowRegis.realFees =0;
            BorrowRegis.paoKufangRen = "0.00";
            BorrowRegis.operator1 = "";
            BorrowRegis.remarks = "";
            long useid = borrow.First().ID;
            var BindReceive = from b in db.BindUserAndArchives
                              where b.userID ==useid
                              select b;
            
            if (BindReceive.Count()!=0)//获取调卷数
            {
                BorrowRegis.consultVolumeCount = BindReceive.Count().ToString();
                BorrowRegis.precisionVolumeCount= BindReceive.Count().ToString();
                BorrowRegis.archiveSerialNo= getSeriNo(useid);
            }
      

            if (BorrowRegis.application1 == true)
            {
                ViewBag.PlantAndDesign = true;
            }
            if (BorrowRegis.application2==true)
            {
                ViewBag.ConstAndManage = true;
                
            }
            if (BorrowRegis.application3 == true)
            {
                ViewBag.MarkAndRegis = true;
                 
            }
            if (BorrowRegis.application4==true)
            {
                ViewBag.SettleAndArgue =true;
            }
            if ( BorrowRegis.application5==true)
            {

                ViewBag.Reasearch = true;
            }
            if (BorrowRegis.application6==true)
            {
                ViewBag.Others=true;
                
            }
            //判断复选框的值-目的
            if (BorrowRegis.goal1== true)
                ViewBag.MakeHistory = true;
            if (BorrowRegis.goal2 == true)
                ViewBag.WorkSurvice = true;
            
            if (BorrowRegis.goal3==true)
                ViewBag.Research =true;
           
            if (BorrowRegis.goal4==true)
                ViewBag.Finance = true;

            if (BorrowRegis.goal5== true)
                ViewBag.Education = true;
           
            if (BorrowRegis.goal6==true)
                ViewBag.Others1 = true;
            //判断复选框的值-效果
            if (BorrowRegis.userEffects1 == true)
                ViewBag.Effect1 = true;

            if (BorrowRegis.userEffects2 == true)
                ViewBag.Effect2 = true;

            if (BorrowRegis.userEffects3 == true)
                ViewBag.Effect3 =true;
            
            if (BorrowRegis.userEffects4 ==true)
                ViewBag.Effect4 =true;
           
            if ( BorrowRegis.userEffects5 ==true)
               ViewBag.Others2 =true;
            ViewBag.Use_UseTu = BorrowRegis.Use_UseTu;
            ViewBag.Use_MuDi = BorrowRegis.Use_MuDi;

            //用户绑定查阅案卷
            ViewData["checkname"] = 0;
            ViewData["url2"] = id1;
            //借阅人需要打印图纸
            long ID = Convert.ToInt32(id);
            DateTime time = DateTime.Now.Date;
            var model = from a in db.BorrowRegistration
                        where a.borrowSeqNo==borr
                        select a;
            var model1 = from b in db.BindUserAndImage
                         select b;
            if (model.Count() != 0)
            {
                time = Convert.ToDateTime(model.First().borrowDate);
                if (!string.IsNullOrEmpty(model.First().seqNo.Trim()))
                {
                    string str = model.First().seqNo.Trim();
                    model1 = from b in db.BindUserAndImage
                             where b.imageTime == time && b.userID ==str
                             orderby b.ID
                             select b;
                }
                else
                {
                    model1 = from b in db.BindUserAndImage
                             where b.imageTime == time && b.realuserID == useid
                             orderby b.ID
                             select b;
                }
                int i = 1;
                foreach (var item in model1)
                {

                    item.recordID = i;
                    i = i + 1;
                    string address = item.ImageAddress.ToString().Trim();
                    string[] add = null;
                    if (address != "")
                    {

                        if (address.IndexOf('/') != -1)
                        {
                            add = address.Split(new string[] { "/" }, StringSplitOptions.None);
                        }
                        else
                        {
                            add = address.Split(new string[] { "//" }, StringSplitOptions.None);
                        }
                    }

                    item.ImageAddress = add[add.Length - 1];
                }

            }
            if (model1.Count() == 0)
            {
                ViewData["checkname"] = 1;
                ViewData["LookSaoButton"] = "disabled";
            }

            ViewBag.result1 = JsonConvert.SerializeObject(model1);
            ViewBag.ID1 = useid;
            return View(BorrowRegis);
        }

        // POST: UserBorrow/Delete/5
        [HttpPost]
        public ActionResult FeeJieSuan(long? id,string action,string archiveSerialNo,string certificationFee,string consultFee,string paoKufangRen,string precisionVolumeCount,string consultVolumeCount,string operator1,string remarks,string TotalFee,string realFees,bool ShouFei,string singleOrDepart,string borrowUnit,string  borrower,string operator2,string borrowDate,string WhereTransform, string url2, bool Effect1, bool Effect2, bool Effect3, bool Effect4, bool Others2,string ecnomicBenefit,string userEffectDetail,string borrowSeqNo)
        {
            string borr = borrowSeqNo.Trim();
            var model1 = from a in db.BorrowRegistration
                         where a.borrowSeqNo == borr
                         select a;
            BorrowRegistration model = model1.First();
            if (action=="保存")
            {
                
                    if (model != null)
                    {
                        model.copyPageCount = "0";

                        model.copyFee = 0;

                        model.archiveSerialNo = archiveSerialNo;/* getSeriNo(id);*///案卷编号
                        string str = certificationFee.ToString().Trim();//证明费
                        if (str == "")
                        {
                            return Content("<script >alert('证明费不能为空');window.history.back();</script >");
                        }
                        model.certificationFee =Convert.ToDecimal(str);
                        str = consultFee.ToString().Trim();
                        if (str == "")
                        {
                            return Content("<script >alert('调阅费不能为空');window.history.back();</script >");


                        }
                    if (Effect1 == false && Effect2 == false && Effect3 == false && Effect4 == false && Others2 == false)
                    {
                        return Content("<script >alert('请选择利用效果！');window.history.back();</script >");

                    }
                    model.consultFee = Convert.ToDecimal(str);

                        str = paoKufangRen.Trim();
                        if (str == "")
                        {
                            return Content("<script >alert('咨询费不能为空');window.history.back();</script >");
                        }
                        model.paoKufangRen = paoKufangRen.Trim();

                        str = precisionVolumeCount.Trim();
                        if (str == "")
                        {
                            return Content("<script >alert('查准卷数不能为空');window.history.back();</script >");

                        }
                        model.precisionVolumeCount = precisionVolumeCount;
                        str = consultVolumeCount.Trim();
                        if (str == "")
                        {
                            return Content("<script >alert('调阅卷数不能为空');window.history.back();</script >");

                        }

                        model.consultVolumeCount = consultVolumeCount;
                        string s = operator1.Trim();
                       if (s!="")
                      {
                        if (s[s.Length - 1].ToString() == ",")
                        {
                            s = s.Substring(0, s.Length - 1);

                        }
                      }

                        model.operator1 = s;

                        model.remarks = remarks.Trim();
                        if (TotalFee.Trim() == "")
                        {
                            return Content("<script >alert('应交费用不能为空');window.history.back();</script >");

                        }

                        if (realFees.ToString().Trim() == "")
                        {
                            return Content("<script >alert('实交费用不能为空');window.history.back();</script >");

                        }
                        if (ShouFei == true)// && (this.realFee.Value.Trim() != "" || this.realFee.Value.Trim() != "0"))
                        {

                            model.realFees = 0;
                            model.chargeFlag = "False";
                  
                            ViewData["checkname"] = 1;
                        }

                        if (ShouFei == false)// && (this.realFee.Value.Trim() != "" || this.realFee.Value.Trim() != "0"))
                        {
                            model.chargeFlag = "True";

                        }

                        model.realFees = Convert.ToDecimal(realFees.Trim());
                        model.isJiesuanFee = 3;
                        db.Entry(model).State = EntityState.Modified;
                    
                        
                        
                    
                  }
                //////////////////////////////////////////////////////////////////////////
                /*
                 * added by 周林
                 * 保存至收费列表





                 * date 20170518
                 */
                if (ShouFei==false)
                {
                    

                        SaveFee(id, singleOrDepart, borrowUnit, borrower, operator1, operator2, certificationFee, consultFee, paoKufangRen, borrowDate, WhereTransform,  Effect1, Effect2,  Effect3,  Effect4,  Others2,  ecnomicBenefit,  userEffectDetail);
                   
                   
                }

                    db.SaveChanges();
                    return Content("<script >alert('保存成功！');location.href='/UrbanBorrow/UserBorrow';</script >");
                
            }
            if(action=="返回")
            {
                if (url2 == "1")
                {
                    return Redirect(url2.Trim());
                }
                if (url2.Trim()=="2")
                {
                    return Redirect("http://localhost:59320/BorrowFeeDetail/BorrowFeeDetail");
                }
                if(url2.Trim() == "")
                {
                     
                    return RedirectToAction("UserBorrow"); 
                }
            }
            return View();
        }
        public ActionResult DownDetail(long? id, string id1)
        {
            ViewBag.Effect1 = true;
            ViewBag.Effect2 = true;
            ViewBag.Effect3 = true;
            ViewBag.Effect4 = true;
            ViewBag.Others2 = true;
                          
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string borr = id.ToString().Trim();
            var borrow = from a in db.BorrowRegistration
                         where a.borrowSeqNo == borr
                         select a;
            BorrowRegistration BorrowRegis = borrow.First();
            BorrowRegis.certificationFee = 0;
            BorrowRegis.consultFee = 0;
            BorrowRegis.paoKufangRen = "";
            BorrowRegis.realFees = 0;
            BorrowRegis.paoKufangRen = "0.00";
            BorrowRegis.operator1 = "";
            BorrowRegis.remarks = "";
            long useid = borrow.First().ID;
            var BindReceive = from b in db.BindUserAndArchives
                              where b.userID == useid
                              select b;

            if (BindReceive.Count() != 0)//获取调卷数
            {
                BorrowRegis.consultVolumeCount = BindReceive.Count().ToString();
                BorrowRegis.precisionVolumeCount = BindReceive.Count().ToString();
                BorrowRegis.archiveSerialNo = getSeriNo(useid);
            }

            var BindUserAndImageDown = from b in db.BindUserAndImageDown
                                       where b.realuserID == useid
                                       select b;

            //统计需要借阅的案卷信息
            if (BindUserAndImageDown.Count() !=0 )
            {
                string str = "";
                string str1 = "";
                int j = 0;               
                var BindUserAndImageDown1 = BindUserAndImageDown.ToArray();
                for (int i = 0; i < BindUserAndImageDown.Count(); i++)
                {
                    if (str != BindUserAndImageDown1[i].archivesNo)
                    {
                        str = BindUserAndImageDown1[i].archivesNo;
                        str1 += BindUserAndImageDown1[i].archivesNo;
                        str1 += '\n';
                        j++;
                    }
                }
                ViewBag.anjuanNo = str1;
                ViewBag.juanshu = j;
            }

            if (BorrowRegis.application1 == true)
            {
                ViewBag.PlantAndDesign = true;
            }
            if (BorrowRegis.application2 == true)
            {
                ViewBag.ConstAndManage = true;

            }
            if (BorrowRegis.application3 == true)
            {
                ViewBag.MarkAndRegis = true;

            }
            if (BorrowRegis.application4 == true)
            {
                ViewBag.SettleAndArgue = true;
            }
            if (BorrowRegis.application5 == true)
            {

                ViewBag.Reasearch = true;
            }
            if (BorrowRegis.application6 == true)
            {
                ViewBag.Others = true;

            }
            //判断复选框的值-目的
            if (BorrowRegis.goal1 == true)
                ViewBag.MakeHistory = true;
            if (BorrowRegis.goal2 == true)
                ViewBag.WorkSurvice = true;

            if (BorrowRegis.goal3 == true)
                ViewBag.Research = true;

            if (BorrowRegis.goal4 == true)
                ViewBag.Finance = true;

            if (BorrowRegis.goal5 == true)
                ViewBag.Education = true;

            if (BorrowRegis.goal6 == true)
                ViewBag.Others1 = true;

            //判断复选框的值-效果
            if (BorrowRegis.userEffects1 == false)
                ViewBag.Effect1 = false;

            if (BorrowRegis.userEffects2 == false)
                ViewBag.Effect2 = false;

            if (BorrowRegis.userEffects3 == false)
                ViewBag.Effect3 = false;

            if (BorrowRegis.userEffects4 == false)
                ViewBag.Effect4 = false;

            if (BorrowRegis.userEffects5 == false)
                ViewBag.Others2 = false;

            //判断复选框的值-效果
            //if (BorrowRegis.userEffects1 == true)
            //    ViewBag.Effect1 = true;

            //if (BorrowRegis.userEffects2 == true)
            //    ViewBag.Effect2 = true;

            //if (BorrowRegis.userEffects3 == true)
            //    ViewBag.Effect3 = true;

            //if (BorrowRegis.userEffects4 == true)
            //    ViewBag.Effect4 = true;

            //if (BorrowRegis.userEffects5 == true)
            //    ViewBag.Others2 = true;
            //用户绑定查阅案卷
            ViewData["checkname"] = 0;
            //ViewData["url2"] = id1;
            //借阅人需要下载图纸
            long ID = Convert.ToInt32(id);
            DateTime time = DateTime.Now.Date;
            var model = from a in db.BorrowRegistration
                        where a.borrowSeqNo == borr
                        select a;
            var model1 = from b in db.BindUserAndImageDown
                         select b;
            if (model.Count() != 0)
            {
                time = Convert.ToDateTime(model.First().borrowDate);
                if (!string.IsNullOrEmpty(model.First().seqNo.Trim()))
                {
                    string str = model.First().seqNo.Trim();
                    model1 = from b in db.BindUserAndImageDown
                             where b.imageTime == time && b.userID == str
                             orderby b.ID
                             select b;
                }
                else
                {
                    model1 = from b in db.BindUserAndImageDown
                             where b.imageTime == time && b.realuserID == useid
                             orderby b.ID
                             select b;
                }
                int i = 1;
                foreach (var item in model1)
                {
                    item.recordID = i;
                    i = i + 1;
                    string address = item.ImageAddress.ToString().Trim();
                    string[] add = null;
                    if (address != "")
                    {

                        if (address.IndexOf('/') != -1)
                        {
                            add = address.Split(new string[] { "/" }, StringSplitOptions.None);
                        }
                        else
                        {
                            add = address.Split(new string[] { "//" }, StringSplitOptions.None);
                        }
                    }

                    item.ImageAddress = add[add.Length - 1];
                }

            }
            if (model1.Count() == 0)
            {
                ViewData["checkname"] = 1;
                ViewData["LookSaoButton"] = "disabled";
            }

            ViewBag.result1 = JsonConvert.SerializeObject(model1);
            ViewBag.ID1 = useid;
            return View(BorrowRegis);
        }

        // POST: UserBorrow/Delete/5
        [HttpPost]
        public ActionResult DownDetail(long? id, string action, string archiveSerialNo, string certificationFee, string consultFee, string paoKufangRen, string precisionVolumeCount, string consultVolumeCount, string operator1, string remarks, string TotalFee, string realFees, string singleOrDepart, string borrowUnit, string borrower, string operator2, string borrowDate, string WhereTransform, string url2, bool Effect1, bool Effect2, bool Effect3, bool Effect4, bool Others2, string ecnomicBenefit, string userEffectDetail, string borrowSeqNo)
        {
            string borr = borrowSeqNo.Trim();
            var model1 = from a in db.BorrowRegistration
                         where a.borrowSeqNo == borr
                         select a;
            BorrowRegistration model = model1.First();
            if (action == "保存")
            {
                if (model != null)
                {
                    model.copyPageCount = "0";
                    model.copyFee = 0;
                    model.archiveSerialNo = archiveSerialNo;/* getSeriNo(id);*///案卷编号
                    //string str = certificationFee.ToString().Trim();//证明费
                    //if (str == "")
                    //{
                    //    return Content("<script >alert('证明费不能为空');window.history.back();</script >");
                    //}
                    //model.certificationFee = Convert.ToDecimal(str);
                    //str = consultFee.ToString().Trim();
                    //if (str == "")
                    //{
                    //    return Content("<script >alert('调阅费不能为空');window.history.back();</script >");
                    //}
                    if (Effect1 == false && Effect2 == false && Effect3 == false && Effect4 == false && Others2 == false)
                    {
                        return Content("<script >alert('请选择利用效果！');window.history.back();</script >");
                    }
                    //判断复选框的值 - 效果
                    if (Effect1 == true)
                        model.userEffects1 = true;
                    else
                        model.userEffects1 = false;
                    if (Effect2 == true)
                        model.userEffects2 = true;
                    else
                        model.userEffects2 = false;
                    if (Effect3 == true)
                        model.userEffects3 = true;
                    else
                        model.userEffects3 = false;
                    if (Effect4 == true)
                        model.userEffects4 = true;
                    else
                        model.userEffects4 = false;
                    if (Others2 == true)
                        model.userEffects5 = true;
                    else
                        model.userEffects5 = false;
                    model.userEffects6 = false;
                    model.userEffects7 = false;
                    model.ecnomicBenefit = ecnomicBenefit;
                    model.userEffectDetail = userEffectDetail;
                    //model.consultFee = Convert.ToDecimal(str);
                    //str = paoKufangRen.Trim();
                    //if (str == "")
                    //{
                    //    return Content("<script >alert('咨询费不能为空');window.history.back();</script >");
                    //}
                    //model.paoKufangRen = paoKufangRen.Trim();
                    //str = precisionVolumeCount.Trim();
                    //if (str == "")
                    //{
                    //    return Content("<script >alert('查准卷数不能为空');window.history.back();</script >");
                    //}
                    model.precisionVolumeCount = precisionVolumeCount;
                    //if (consultVolumeCount.Trim() == "")
                    //{
                    //    return Content("<script >alert('调阅卷数不能为空');window.history.back();</script >");
                    //}

                    model.consultVolumeCount = "Down";
                    //string s = operator1.Trim();
                    //if (s != "")
                    //{
                    //    if (s[s.Length - 1].ToString() == ",")
                    //    {
                    //        s = s.Substring(0, s.Length - 1);
                    //    }
                    //}

                    //model.operator1 = s;
                    model.operator1 = operator1;
                    //model.remarks = remarks.Trim();
                    //if (TotalFee.Trim() == "")
                    //{
                    //    return Content("<script >alert('应交费用不能为空');window.history.back();</script >");
                    //}

                    //if (realFees.ToString().Trim() == "")
                    //{
                    //    return Content("<script >alert('实交费用不能为空');window.history.back();</script >");
                    //}
                    //if (ShouFei == true)// && (this.realFee.Value.Trim() != "" || this.realFee.Value.Trim() != "0"))
                    //{

                    //    model.realFees = 0;
                    //    model.chargeFlag = "False";

                    //    ViewData["checkname"] = 1;
                    //}

                    //if (ShouFei == false)// && (this.realFee.Value.Trim() != "" || this.realFee.Value.Trim() != "0"))
                    //{
                    //    model.chargeFlag = "True";

                    //}

                    //model.realFees = Convert.ToDecimal(realFees.Trim());
                    //model.isJiesuanFee = 3;
                    db.Entry(model).State = EntityState.Modified;
                }
                //if (ShouFei == false)
                //{
                //SaveFee(id, singleOrDepart, borrowUnit, borrower, operator1, operator2, certificationFee, consultFee, paoKufangRen, borrowDate, WhereTransform, Effect1, Effect2, Effect3, Effect4, Others2, ecnomicBenefit, userEffectDetail);
                //}
                db.SaveChanges();
                return Content("<script >alert('保存成功！');location.href='/UrbanBorrow/UserBorrow';</script >");
            }
            if (action == "返回")
            {
                //if (url2 == "1")
                //{
                //    return Redirect(url2.Trim());
                //}
                //if (url2.Trim() == "2")
                //{
                //    return Redirect("http://localhost:59320/BorrowFeeDetail/BorrowFeeDetail");
                //}
                //if (url2.Trim() == "")
                //{
                return RedirectToAction("UserBorrow");
                //}
            }
            return View();
        }

        public ActionResult DownTable(string id, string type = "PDF")
        {
            int id1 = int.Parse(id);
            var borrow = from b in db.BorrowRegistration
                         where b.ID == id1
                         select b;
            string borrowUnit = borrow.First().borrowUnit;
            string borrower = borrow.First().borrower;
            var dt = from a in db.BindUserAndImageDown
                     where a.realuserID == id1
                     select a;
            if (dt.Count() == 0) {
                return Content("<script >alert('该用户没有绑定图纸！');window.history.back();</script >");
            }
            string tiaomu = "";
            foreach (var item in dt)
            {
                string address = item.ImageAddress.ToString().Trim();
                string[] add = null;
                if (address != "")
                {
                    if (address.IndexOf('/') != -1)
                    {
                        add = address.Split(new string[] { "/" }, StringSplitOptions.None);
                    }
                    else
                    {
                        add = address.Split(new string[] { "//" }, StringSplitOptions.None);
                    }
                }
                tiaomu = tiaomu + add[add.Length - 1] + ",";
            }
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/YongYinTable.rdlc");
            List<ReportParameter> parameterList = new List<ReportParameter>();
            parameterList.Add(new ReportParameter("borrowUnit", borrowUnit.Trim()));
            parameterList.Add(new ReportParameter("borrower", borrower.Trim()));
            parameterList.Add(new ReportParameter("tiaomu", tiaomu));
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

        public ActionResult printLinq3(string Info, string i = "0")
        {
            ViewBag.i = i;
            return View();
        }
        public ActionResult printLinq2(string id,string Info)
        {
            return View();
        }
        public string GetSize(string path)
        {
            string kinds;
            //192.168.0.114:8003/JunGongArchives/00003/5/00907006001.tif
            //     //D://JunGongArchives/00003/5/00907006001.tif"
            if (path.IndexOf(".pdf") != -1)
            {
                kinds = "pdf";
            }
            else
            {
                if (path.Contains("JunGongArchives1"))
                {

                    path = path.Replace("http://192.168.0.114:8003", "G://");

                }
                else
                {

                    path = path.Replace("http://192.168.0.114:8003", "H://");

                }
                //if (path.Contains("PlanArchives"))
                //{

                //    path = path.Replace("http://192.168.0.114:8003", "G://");

                //}
                //else
                //{

                //    path = path.Replace("http://192.168.0.114:8003", "H://");

                //}
                Bitmap image = new Bitmap(path);

                var xDPI = image.HorizontalResolution;
                var yDPI = image.VerticalResolution;
                if (xDPI != 300)
                {
                    xDPI = 200;
                }
                if (yDPI != 300)
                {
                    yDPI = 200;
                }

                var width = image.Width;
                var heigt = image.Height;
                double sourceWidth = width * 25.4 / xDPI;//ima->GetXDPI();  //mm
                double sourceHeight = heigt * 25.4 / yDPI;//ima->GetYDPI(); //mm





                double max = sourceHeight;
                if (max < sourceWidth)
                {
                    max = sourceWidth;
                    sourceWidth = sourceHeight;
                    sourceHeight = max;
                }

                float range = 25;
                if (sourceHeight <= 260 + range)
                {
                    if (sourceWidth <= 184 + range)
                        kinds = "16K";
                    else
                        kinds = "A4";
                }
                else if (sourceHeight <= 297 + range)
                {
                    if (sourceWidth <= 210 + range)
                        kinds = "A4";
                    else
                        kinds = "A3";
                }
                else if (sourceHeight <= 420 + range)
                {
                    if (sourceWidth <= 294 + range)
                        kinds = "A3";
                    else
                        kinds = "A2";
                }
                else if (sourceHeight <= 594 + range)
                {
                    if (sourceWidth <= 420 + range)
                        kinds = "A2";
                    else
                        kinds = "A1";
                }
                else if (sourceHeight <= 841 + range)
                {
                    if (sourceWidth <= 594 + range)
                        kinds = "A1";
                    else
                        kinds = "A0";
                }
                else if (sourceHeight <= 1189 + range)
                {
                    if (sourceWidth <= 594 + range)
                    {
                        kinds = "A1加长";
                    }
                    else if (sourceWidth <= 841 + range)
                    {
                        kinds = "A0";
                    }
                    else
                    {
                        kinds = "A0加长";
                    }
                }
                else
                {
                    kinds = "A0加长";
                }

                image.Dispose();
            }
            return JsonConvert.SerializeObject(kinds);
        }
        public string GetALLsize(string[] arrpa)

        {
            string kinds = "";
            for (int i = 0; i < arrpa.Length; i++)
            {

                

                string arradress = arrpa[i].Substring(arrpa[i].IndexOf("/", arrpa[i].IndexOf(".")));

                var list1 = from a in db.BindUserAndImage
                            where a.ImageAddress == arradress && a.imageSize != null && a.imageSize != ""
                            orderby a.ID descending
                            select a.imageSize;
              
                    if (list1.Count() != 0)
                  {
                        if (list1.First() == "16")
                        {
                            kinds = kinds + list1.First().ToString() + "K,";
                        }
                        else {
                            kinds = kinds + list1.First().ToString() + ",";
                        }
                  }

                

                else
                {

                

                    if (arrpa[i].Contains("JunGongArchives1"))
                    {
                        arrpa[i] = arrpa[i].Replace("http://192.168.0.114:8003/", "G://");
                    }
                    else
                    {
                        arrpa[i] = arrpa[i].Replace("http://192.168.0.114:8003/", "H://");
                    }


                    Bitmap image = new Bitmap(arrpa[i]);
                    var xDPI = image.HorizontalResolution;
                    var yDPI = image.VerticalResolution;
                    if (xDPI != 300)
                    {
                        xDPI = 200;
                    }
                    if (yDPI != 300)
                    {
                        yDPI = 200;
                    }

                    var width = image.Width;
                    var heigt = image.Height;
                    double sourceWidth = width * 25.4 / xDPI;//ima->GetXDPI();  //mm
                    double sourceHeight = heigt * 25.4 / yDPI;//ima->GetYDPI(); //mm

                    double max = sourceHeight;
                    if (max < sourceWidth)
                    {
                        max = sourceWidth;
                        sourceWidth = sourceHeight;
                        sourceHeight = max;
                    }

                    float range = 25;
                    if (sourceHeight <= 260 + range)
                    {
                        if (sourceWidth <= 184 + range)
                            kinds = kinds + "16K" + ",";
                        else
                            kinds = kinds + "A4" + ",";
                    }
                    else if (sourceHeight <= 297 + range)
                    {
                        if (sourceWidth <= 210 + range)
                            kinds = kinds + "A4" + ",";
                        else
                            kinds = kinds + "A3" + ",";
                    }
                    else if (sourceHeight <= 420 + range)
                    {
                        if (sourceWidth <= 294 + range)
                            kinds = kinds + "A3" + ",";
                        else
                            kinds = kinds + "A2" + ",";
                    }
                    else if (sourceHeight <= 594 + range)
                    {
                        if (sourceWidth <= 420 + range)
                            kinds = kinds + "A2" + ",";
                        else
                            kinds = kinds + "A1" + ",";
                    }
                    else if (sourceHeight <= 841 + range)
                    {
                        if (sourceWidth <= 594 + range)
                            kinds = kinds + "A1" + ",";
                        else
                            kinds = kinds + "A0" + ",";
                    }
                    else if (sourceHeight <= 1189 + range)
                    {
                        if (sourceWidth <= 594 + range)
                        {
                            kinds = kinds + "A1加长" + ",";
                        }
                        else if (sourceWidth <= 841 + range)
                        {
                            kinds = kinds + "A0" + ",";
                        }
                        else
                        {
                            kinds = kinds + "A0加长" + ",";
                        }
                    }
                    else
                    {
                        kinds = kinds + "A0加长" + ",";
                    }

                    image.Dispose();
                }
        }
            return JsonConvert.SerializeObject(kinds);
        }
        public string GetUserAndImageListByUserId(string userSeqNo,string i)
        {
            long ID = long.Parse(userSeqNo.Trim());
            string virtualPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ServerVirtualPath"];
            DataTable myTable = null;
            var model = from a in db.BindUserAndImage
                        where a.realuserID == ID
                        orderby a.ID
                        select a;
            if (model.Count() == 0)
            {
                var model1 = from b in db.BindUserAndImageDown
                             where b.realuserID == ID
                             orderby b.ID
                             select b;
                
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));
                myTable.Columns.Add("ID", Type.GetType("System.String"));

                foreach (var item in model1)
                {
                    string address = item.ImageAddress.ToString().Trim();
                    string[] add = null;
                    if (address != "")
                    {

                        if (address.IndexOf('/') != -1)
                        {
                            add = address.Split(new string[] { "/" }, StringSplitOptions.None);
                        }
                        else
                        {
                            add = address.Split(new string[] { "//" }, StringSplitOptions.None);
                        }
                    }
                    if (address.IndexOf(".pdf") != -1)
                    {
                        if (item.yeci == "" || item.yeci == null)
                        {
                            string address2 = address.Substring(1);
                            int address3 = address2.IndexOf('/');
                            string address1 = address2.Substring(address3);
                            if (address.IndexOf("JunGongArchives1") != -1) {
                                System.IO.File.Copy("G:/" + address, "G://JunGongArchives1/temporary" + address1, true);
                                
                            } else {
                                System.IO.File.Copy("G:/" + address, "G://JunGongArchives/temporary" + address1, true);
                                
                            }
                            
                        }
                        else
                        {
                            string[] a = item.yeci.ToString().Split(',');
                            List<int> yeciList = new List<int>();
                            for (int j = 0; j < a.Length; j++)
                            {
                                if (a[j].IndexOf("-") != -1)
                                {
                                    string strLeft = a[j].Substring(0, a[j].IndexOf("-"));
                                    string strRight = a[j].Substring(a[j].IndexOf("-") + 1);
                                    int k = 0;
                                    do
                                    {
                                        yeciList.Add(int.Parse(strLeft) + k);
                                        k++;
                                    } while (yeciList.Last() != int.Parse(strRight));
                                }
                                else
                                {
                                    yeciList.Add(int.Parse(a[j]));
                                }
                            }
                            //int[] yeci = Array.ConvertAll(a, int.Parse);
                            //string addre = "E://数据/jpg" + address;
                            string addre = "G:/" + address;

                            string address2 = address.Substring(1);
                            int address3 = address2.IndexOf('/');
                            string address1 = address2.Substring(address3);

                            if (i == "1")
                            {
                                //address = "/temporary/" + address;//修改之后进入
                            }
                            else
                            {
                                if (address.IndexOf("JunGongArchives1") != -1)
                                {
                                    ExtractPages(addre, "G://JunGongArchives1/temporary" + address1, yeciList.ToArray());
                                    address = "/JunGongArchives1/temporary/" + address1;
                                }
                                else
                                {
                                    ExtractPages(addre, "G://JunGongArchives/temporary" + address1, yeciList.ToArray());
                                    address = "/JunGongArchives/temporary/" + address1;
                                }

                            }

                        }


                        myDataRow = myTable.NewRow();
                        myDataRow["Name"] = add[add.Length - 1];
                        myDataRow["WebPath"] = "http://192.168.0.113:8003" + address;
                        //myDataRow["WebPath"] = virtualPath + address;
                        myDataRow["ID"] = item.ID;
                        myTable.Rows.Add(myDataRow);
                    }
                }
            }
            else {
                
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));
                myTable.Columns.Add("ID", Type.GetType("System.String"));

                foreach (var item in model)
                {
                    string address = item.ImageAddress.ToString().Trim();



                    string[] add = null;
                    if (address != "")
                    {

                        if (address.IndexOf('/') != -1)
                        {
                            add = address.Split(new string[] { "/" }, StringSplitOptions.None);
                        }
                        else
                        {
                            add = address.Split(new string[] { "//" }, StringSplitOptions.None);
                        }
                    }


                    myDataRow = myTable.NewRow();
                    myDataRow["Name"] = add[add.Length - 1];
                    myDataRow["WebPath"] = virtualPath + address;
                    myDataRow["ID"] = item.ID;
                    myTable.Rows.Add(myDataRow);




                }
            }
            
           
        
            return JsonConvert.SerializeObject(myTable);
        }

        public void ExtractPages(string sourcePdfPath, string outputPdfPath, int[] extractThesePages)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;
            try
            {
                reader = new PdfReader(sourcePdfPath);
                sourceDocument = new Document(reader.GetPageSizeWithRotation(extractThesePages[0]));
                var outPath = outputPdfPath.Substring(0, outputPdfPath.LastIndexOf("/") + 1);
                if (!Directory.Exists(outPath))
                {
                    Directory.CreateDirectory(outPath);
                }
                pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create, FileAccess.Write));
                sourceDocument.Open();
                foreach (int pageNumber in extractThesePages)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader, pageNumber);
                    pdfCopyProvider.AddPage(importedPage);
                }
                sourceDocument.Close();
                reader.Close();
                if (sourcePdfPath.Substring(0, sourcePdfPath.LastIndexOf("/") + 1) == outPath)
                {
                    System.IO.File.Delete(sourcePdfPath);
                    System.IO.FileInfo file = new System.IO.FileInfo(outputPdfPath);
                    file.MoveTo(outPath + sourcePdfPath.Substring(sourcePdfPath.LastIndexOf("/") + 1));
                }
            }
            catch (Exception ex) { throw ex; }
        }



        public string printLinq3ok(string txtPaperSize, string txtImageId)
        {

            string[] ids = txtImageId.Trim().Split(',');
            string[] paperTypes = txtPaperSize.Trim().Split(',');
            if (ids.Length != paperTypes.Length)
            {

                return "2";
            }

            for (int i = 0; i < ids.Length; i++)
            {
                int ID = Int32.Parse(ids[i]);
                var bModel = from a in db.BindUserAndImage
                             where a.ID == ID
                             select a;
                BindUserAndImage model = bModel.First();
                if (bModel.Count() != 0)
                {
                    model.imageSize = paperTypes[i].Substring(0, 2);
                    if (model.imageSize== "A3" || model.imageSize== "A4" || model.imageSize=="16")
                    {
                        model.isWordOrPic = "文字";
                    }
                    else
                    {
                        model.isWordOrPic = "图纸";
                    }
                    db.Entry(model).State = EntityState.Modified;
                }

            }
                db.SaveChanges();
            

            return "1";
            
         
    }
    public string GetUserAndImageList(string userSeqNo)
        {
           
            string virtualPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ServerVirtualPath"];
            var model = from a in db.BindUserAndImage
                        where a.userID ==userSeqNo
                        orderby a.ID
                        select a;
            DataTable myTable = null;
            myTable = new DataTable();

            DataRow myDataRow;

            myTable.Columns.Add("Name", Type.GetType("System.String"));

            myTable.Columns.Add("WebPath", Type.GetType("System.String"));
            myTable.Columns.Add("ID", Type.GetType("System.String"));

            foreach (var item in model)
            {
                string address = item.ImageAddress.ToString().Trim();



                string[] add = null;
                if (address != "")
                {

                    if (address.IndexOf('/') != -1)
                    {
                        add = address.Split(new string[] { "/" }, StringSplitOptions.None);
                    }
                    else
                    {
                        add = address.Split(new string[] { "//" }, StringSplitOptions.None);
                    }
                }


                myDataRow = myTable.NewRow();
                myDataRow["Name"] = add[add.Length - 1];
                myDataRow["WebPath"] = virtualPath + address;
                myDataRow["ID"] = item.ID;
                myTable.Rows.Add(myDataRow);




            }


            return JsonConvert.SerializeObject(myTable);
        }
        public ActionResult Tuzhiok(/*string userSeqNo*/object obj1)
        {

            //string virtualPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ServerVirtualPath"];
            //var model = from a in db.BindUserAndImage
            //            where a.userID == userSeqNo
            //            orderby a.ID
            //            select a;
            //DataTable myTable = null;
            //myTable = new DataTable();

            //DataRow myDataRow;

            //myTable.Columns.Add("Name", Type.GetType("System.String"));

            //myTable.Columns.Add("WebPath", Type.GetType("System.String"));
            //myTable.Columns.Add("ID", Type.GetType("System.String"));

            //foreach (var item in model)
            //{
            //    string address = item.ImageAddress.ToString().Trim();



            //    string[] add = null;
            //    if (address != "")
            //    {

            //        if (address.IndexOf('/') != -1)
            //        {
            //            add = address.Split(new string[] { "/" }, StringSplitOptions.None);
            //        }
            //        else
            //        {
            //            add = address.Split(new string[] { "//" }, StringSplitOptions.None);
            //        }
            //    }


            //    myDataRow = myTable.NewRow();
            //    myDataRow["Name"] = add[add.Length - 1];
            //    myDataRow["WebPath"] = virtualPath + address;
            //    myDataRow["ID"] = item.ID;
            //    myTable.Rows.Add(myDataRow);




            //}
            //DataTable myTable = null;
            //myTable = new DataTable();

            //DataRow myDataRow;

            //myTable.Columns.Add("Name", Type.GetType("System.String"));

            //myTable.Columns.Add("WebPath", Type.GetType("System.String"));
            //myTable.Columns.Add("ID", Type.GetType("System.String"));
            //for (var i=0;i<arr1.Length;i++)
            //{
            //    myDataRow = myTable.NewRow();
            //    myDataRow["Name"] =arr1[i];
            //    myDataRow["WebPath"] =arr2[i];
              
            //    myTable.Rows.Add(myDataRow);
            //}



            //ViewBag.result1 = JsonConvert.SerializeObject(myTable);
            return View();
        }
        public ActionResult PageSize()
        {

            return View();
        }
        public ActionResult Tuzhiok2(string userSeqNo)
        {
            long id = long.Parse(userSeqNo.Trim());
            string virtualPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ServerVirtualPath"];
            var model = from a in db.BindUserAndImage
                        where a.realuserID == id
                        orderby a.ID
                        select a;
            DataTable myTable = null;
            myTable = new DataTable();

            DataRow myDataRow;

            myTable.Columns.Add("Name", Type.GetType("System.String"));

            myTable.Columns.Add("WebPath", Type.GetType("System.String"));
            myTable.Columns.Add("ID", Type.GetType("System.String"));

            foreach (var item in model)
            {
                string address = item.ImageAddress.ToString().Trim();



                string[] add = null;
                if (address != "")
                {

                    if (address.IndexOf('/') != -1)
                    {
                        add = address.Split(new string[] { "/" }, StringSplitOptions.None);
                    }
                    else
                    {
                        add = address.Split(new string[] { "//" }, StringSplitOptions.None);
                    }
                }


                myDataRow = myTable.NewRow();
                myDataRow["Name"] = add[add.Length - 1];
                myDataRow["WebPath"] = virtualPath + address;
                myDataRow["ID"] = item.ID;
                myTable.Rows.Add(myDataRow);




            }



            ViewBag.result1 = JsonConvert.SerializeObject(myTable);
            return View();
        }
        public PartialViewResult Siaomiaojian(long ?id, int? page)
        {
           
            long ID = Convert.ToInt32(id);
            DateTime time= DateTime.Now.Date;
            var model = from a in db.BorrowRegistration
                        where a.ID == id
                        select a;
            var model1= from b in db.BindUserAndImage
                        select b;
            if (model.Count()!=0)
            {
                 time =Convert.ToDateTime(model.First().borrowDate);
                 model1 = from b in db.BindUserAndImage
                          where b.imageTime==time&&b.realuserID==id
                          orderby b.ID
                          select b;
                int i = 1;
                foreach(var item in model1)
                {

                    item.recordID = i;
                    i = i + 1;
                    string address=item.ImageAddress.ToString().Trim();
                    if(address!="")
                    {
                        string[] add = address.Split(new string[] {"/"},StringSplitOptions.None);
                        item.ImageAddress = add[add.Length - 1];
                    }

                }
                if(model1.Count()==0)
                {
                    ViewData["checkname"] =1;
                }
            }
            
            int pageSize =5;
            int pageNumber = (page ?? 1);
            return PartialView(model1.ToPagedList(pageNumber, pageSize));
           
        }
       
        public ActionResult Deleting(string id,string id2)
        {
           long id3 = Convert.ToInt32(id);
            long id4 = Convert.ToInt32(id2);
            BindUserAndImage Image = db.BindUserAndImage.Find(id3);
            db.BindUserAndImage.Remove(Image);
            db.SaveChanges();
            return RedirectToAction("FeeJieSuan", new { id = id4 });
        }
        public ActionResult DeletingSaoMiao(string txtImageId,string ID )
        {
            string[] txt = txtImageId.Split(',');
            long reID = long.Parse(ID);
            var judge = from a in db.BindUserAndImage
                        where a.realuserID == reID
                        select a;
            if (txt.Count() != 0)
            {
                if (judge.Count() == 0)
                {
                    foreach (var item in txt)
                    {
                        int id = Int32.Parse(item.Trim());
                        BindUserAndImageDown Image1 = db.BindUserAndImageDown.Find(id);
                        db.BindUserAndImageDown.Remove(Image1);
                    }
                }
                else
                {
                    foreach (var item in txt)
                    {
                        int id = Int32.Parse(item.Trim());
                        BindUserAndImage Image = db.BindUserAndImage.Find(id);
                        db.BindUserAndImage.Remove(Image);
                    }
                }
                //try
                //{
                db.SaveChanges();

                //return Content("<script >alert('删除成功！');location.href='/UrbanBorrow/printLinq3/?Info='" + ID + ";</script >");
                return Content("<script >alert('删除成功！');window.location.href='/UrbanBorrow/printLinq3/?Info="+ID+"';</script >");
                //return Content("<script >alert('该工程已接收！');window.location.href='/VideoArchives/Edit?id=" + id + "';</script >");
                //return Content("<script >alert('删除成功！');window.location.href='/UrbanBorrow/printLinq3/Info='"+ID+";</script >");

            }
            //return "1";
            //     }
            //     catch
            //     {
            //         return "2";
            //     }
            // }
            else
            {
                return Content("<script >alert('删除失败！');window.history.back();</script >");
            }

         
        }

        public ActionResult DeletingSaoMiao(string txtImageId, string ID, string Cntyeci, string txtImageName)
        {
            string[] txt = txtImageId.Split(',');
            long reID = long.Parse(ID);
            var judge = from a in db.BindUserAndImage
                        where a.realuserID == reID
                        select a;
            if (txt.Count() != 0)
            {
                if (Cntyeci == "wu")
                {
                    if (judge.Count() == 0)
                    {
                        foreach (var item in txt)
                        {
                            int id = Int32.Parse(item.Trim());
                            BindUserAndImageDown Image1 = db.BindUserAndImageDown.Find(id);
                            db.BindUserAndImageDown.Remove(Image1);
                        }
                    }
                    else
                    {
                        foreach (var item in txt)
                        {
                            int id = Int32.Parse(item.Trim());
                            BindUserAndImage Image = db.BindUserAndImage.Find(id);
                            db.BindUserAndImage.Remove(Image);
                        }
                    }
                    //try
                    //{
                    db.SaveChanges();

                    //return Content("<script >alert('删除成功！');location.href='/UrbanBorrow/printLinq3/?Info='" + ID + ";</script >");
                    return Content("<script >alert('删除成功！');window.location.href='/UrbanBorrow/printLinq3/?Info=" + ID + "';</script >");
                    //return Content("<script >alert('该工程已接收！');window.location.href='/VideoArchives/Edit?id=" + id + "';</script >");
                    //return Content("<script >alert('删除成功！');window.location.href='/UrbanBorrow/printLinq3/Info='"+ID+";</script >");
                }
                else
                {

                    string[] name = txtImageName.Split(',');
                    string[] a = Cntyeci.Split(',');
                    List<int> yeciList = new List<int>();
                    for (int j = 0; j < a.Length; j++)
                    {
                        if (a[j].IndexOf("-") != -1)
                        {
                            string strLeft = a[j].Substring(0, a[j].IndexOf("-"));
                            string strRight = a[j].Substring(a[j].IndexOf("-") + 1);
                            int k = 0;
                            do
                            {
                                yeciList.Add(int.Parse(strLeft) + k);
                                k++;
                            } while (yeciList.Last() != int.Parse(strRight));
                        }
                        else
                        {
                            yeciList.Add(int.Parse(a[j]));
                        }
                    }
                    //int[] yeci = Array.ConvertAll(a, int.Parse);
                    foreach (var item in name)
                    {
                        foreach (var i in txt)
                        {
                            int id = Int32.Parse(i.Trim());
                            string address = db.BindUserAndImageDown.Find(id).ImageAddress;
                            string add = "E://数据/jpg/temporary" + address;
                            string address1 = address.Substring(0, address.LastIndexOf("/") + 1) + "zan.pdf";
                            ExtractPages(add, "E://数据/jpg/temporary" + address1, yeciList.ToArray());
                        }

                    }
                    return Content("<script >alert('删除成功！');window.location.href='/UrbanBorrow/printLinq3/?Info=" + ID + "&i=1" + "';</script >");
                }
            }
            //return "1";
            //     }
            //     catch
            //     {
            //         return "2";
            //     }
            // }
            else
            {
                return Content("<script >alert('删除失败！');window.history.back();</script >");
            }


        }
        private string getSeriNo(long ?id)
        {
            string str = "";//str=项目顺序号/第几卷
            var model = from a in db.BindUserAndArchives
                        where a.userID == id
                        select a;

            foreach (var  dr in model)
            {
                int type = Int32.Parse(dr.type.ToString());
                if (type == 1)//竣工档案
                {
                    string archiNo = dr.archiveNo.ToString();
                    str += "竣工:";
                    str += getFinishArchSeriNo(archiNo) + ",";
                }
                if (type == 2)//声像视频档案
                {

                    string archiNo = dr.archiveNo.ToString();
                    str += getVideoArchSeriNo(archiNo) + ",";
                }
                if (type == 3)//声像照片档案
                {
                    string archiNo = dr.archiveNo.ToString();
                    str += getPhotoArchSeriNo(archiNo) + ",";
                }
                if (type == 4)//规划档案
                {
                    string archiNo = dr.archiveNo.ToString();
                    str += "规划:";
                    str += archiNo + ",";
                }
                if (type == 5)//其他档案
                {
                    //if(dr.archiveNo.ToString().IndexOf('/')==-1)
                    //{
                    //    str += "请照:";
                    //    str += dr.archiveNo.ToString() + ",";
                    //}
                    //else
                    //{
                    //    string[] archiNo = dr.archiveNo.ToString().Split('/');

                    //    str += "请照:";

                    //    str += archiNo[0] + ",";
                    //}
                    str += "请照:";
                    str += dr.archiveNo.ToString() + ",";
                }
                if (type == 6)//征地档案
                {
                    str += "征地:";
                  
                    str += dr.archiveNo.ToString() + ",";
                }
                if (type == 7)//征地档案
                {
                    str += "图纸:";
                    str += dr.archiveNo.Trim()+ ",";
                }
                if (type == 8)//管线档案
                {
                    str += "管线:";
                    str += dr.archiveNo.Trim() + ",";
                }
                if (type == 9)//援川档案
                {
                    str += "援川:";
                    str += dr.archiveNo.Trim() + ",";
                }
            }
            if (str != "")
                str = str.Substring(0, str.Length - 1);
            return str;
        }
        private string getFinishArchSeriNo(string archiveNo)
        {
            string projSeqNo = string.Empty;
            string dijijuan = string.Empty;
            if (archiveNo.IndexOf('/') == -1)
            {
                archiveNo = archiveNo.Trim();
            }
            else
            {
                archiveNo = archiveNo.Split('/')[1].ToString().Trim();
            }
            var Archives=from a in db.ArchivesDetail
                               where a.archivesNo==archiveNo
                               select a;
            string str = "";
            if (Archives.Count()!=0)
            {
                ArchivesDetail model = Archives.First();
               
                projSeqNo = model.paperProjectSeqNo.ToString();
                //dijijuan = getFinishArchDJVol(projSeqNo, model.registrationNo);
                dijijuan = model.volNo.ToString();
                str = projSeqNo + "-" + dijijuan;
                //str=dijijuan;
            }

           
            return str;
        }
        private string getPhotoArchSeriNo(string archiveNo)
        {
            string projSeqNo = string.Empty;
            string dijijuan = string.Empty;
            var cassete = from a in db.PhotoCassette
                           where a.photoArchiveNo == archiveNo
                           select a;
            string str = "";
            if(cassete.Count()!=0)
            {
                PhotoCassette model = cassete.First();
                projSeqNo = model.videoProjectSeqNo.ToString().Trim();
                dijijuan = getPhotoArchDJVol(projSeqNo, model.registrationNo);
                str = projSeqNo + "/" + dijijuan;
            }
           
            return str;
        }
        private string getVideoArchSeriNo(string archiveNo)
        {
            string projSeqNo = string.Empty;
            string dijijuan = string.Empty;
            var cassete = from a in db_video.VideoCassette
                          where a.videoArchiveNo ==archiveNo
                          select a;
            string str = "";
            if (cassete.Count()!=0)
            {
               VideoCassette model = cassete.First();
                projSeqNo = model.videoProjectSeqNo.ToString().Trim();
                dijijuan = getVideoArchDJVol(projSeqNo, model.registrationNo);

                str = projSeqNo + "/" + dijijuan;
            }
            
          
            
           
            return str;
        }
        private string getPhotoArchDJVol(string photoProjSeqNo, string registrationNo)
        {
            int nDJVol = -1;
            long number = Int32.Parse(photoProjSeqNo);
            var idal = from a in db_video.VideoArchives
                       where a.videoProjectSeqNo == number
                       select a;
            VideoArchives model = idal.First();
           
           
            if (model != null)
            {
                string strStartReg = model.startPhotoRegisNo;
                if (strStartReg != null)
                {
                    nDJVol = Int32.Parse(registrationNo) - Int32.Parse(strStartReg) + 1;
                    if (nDJVol < 1)
                    {
                        nDJVol = -1;
                    }
                }
            }
            return nDJVol.ToString();
        }
        private string getVideoArchDJVol(string videoProjSeqNo, string registrationNo)
        {
            int nDJVol = -1;
            long number = Int32.Parse(videoProjSeqNo);
            var idal = from a in db_video.VideoArchives
                       where a.videoProjectSeqNo == number
                       select a;
            VideoArchives model = idal.First();
           
            if (model != null)
            {
                string strStartReg = model.startVideoRegisNo;
                if (strStartReg != null)
                {
                    nDJVol = Int32.Parse(registrationNo) - Int32.Parse(strStartReg) + 1;
                    if (nDJVol < 1)
                    {
                        nDJVol = -1;
                    }
                }
            }
            return nDJVol.ToString();
        }
        //private string getYuanchuanArchSeriNo(string archiveNo)
        //{
        //    string projSeqNo = string.Empty;
        //    string dijijuan = string.Empty;
        //    if (archiveNo.IndexOf('/') == -1)
        //    {
        //        archiveNo = archiveNo.Trim();
        //    }
        //    else
        //    {
        //        archiveNo = archiveNo.Split('/')[1].ToString().Trim();
        //    }
        //    var Archives = from a in db.ArchivesDetail
        //                   where a.archivesNo == archiveNo
        //                   select a;
        //    string str = "";
        //    if (Archives.Count() != 0)
        //    {
        //        ArchivesDetail model = Archives.First();

        //        projSeqNo = model.paperProjectSeqNo.ToString();
        //        //dijijuan = getFinishArchDJVol(projSeqNo, model.registrationNo);
        //        dijijuan = model.volNo.ToString();
        //        str = projSeqNo + "-" + dijijuan;
        //        //str=dijijuan;
        //    }


        //    return str;
        //}
        protected void SaveFee(long? id, string singleOrDepart, string borrowUnit, string borrower, string operator1, string operator2, string certificationFee, string consultFee, string paoKufangRen,string  borrowDate,string WhereTransform, bool Effect1, bool Effect2, bool Effect3, bool Effect4, bool Others2, string ecnomicBenefit, string userEffectDetail)
        {

            string feeNo = getCurDayMaxSeqNo();
            string borr = id.ToString().Trim();
            var A = from a in db.BorrowRegistration
                    where a.borrowSeqNo == borr
                    select a;
            int BusinessCode = Convert.ToInt32(A.First().ID);
            string strFeeName, strFeeDepart, strUnitName, strDealMan;
            float fArea = 0.0f;    //可空修饰符


            if (singleOrDepart.Trim() == "单位")
            {
                strFeeName = borrowUnit + "借阅费";
                strUnitName = borrowUnit;
            }
            else
            {
                strFeeName = borrower + "借阅费";
                strUnitName = borrower;
            }
            //string strOperator = "";
            strDealMan = operator2;
            int iDepart;
            iDepart = 2;
            strFeeDepart = iDepart.ToString();
            decimal[] feeNumbers = new decimal[3];
            string[] strFeeTypes = new string[3];
            int iType;

            feeNumbers[0] = Convert.ToDecimal(certificationFee.Trim());
            iType = 6;
            strFeeTypes[0] = iType.ToString();
            string chargeDetail = "证明费：" + certificationFee.ToString();

            feeNumbers[1] = Convert.ToDecimal(consultFee.Trim()) ;
            iType = 5;
            strFeeTypes[1] = iType.ToString();
            chargeDetail += "，调阅费：" + consultFee.ToString();

            feeNumbers[2] = decimal.Parse(paoKufangRen.Trim());
            decimal tot = feeNumbers[0] + feeNumbers[1] + feeNumbers[2];
            iType = 9;
            strFeeTypes[2] = iType.ToString();
            chargeDetail += "，咨询费：" + paoKufangRen;

            chargeDetail += "，总计：" + tot.ToString();

            string time = borrowDate.Trim();//借阅日期格式不可随意更改，关联扫描件，收费日期，慎之20170908
            DateTime strchargetime = Convert.ToDateTime(time.Trim());
            string strIsCharge = "false";
            int nWhereTransfer;
            if (WhereTransform=="0")
            {
                nWhereTransfer = 0;
            }
            else
            {
                nWhereTransfer = 1;
            }
            
           

                string result =InsertFeesList(BusinessCode, strFeeName, strUnitName, fArea, strDealMan, strFeeDepart, strFeeTypes, feeNumbers, strIsCharge, strchargetime, chargeDetail, nWhereTransfer);
                if (result.IndexOf("error") > -1)
                {
                  
                    Response.Write("错误，请稍后重试！");
                    
                }
                else
                {
                    var iborrowRegis = from a in db.BorrowRegistration
                                       where a.borrowSeqNo== borr
                                       select a;

                    BorrowRegistration model = iborrowRegis.First();
                //判断复选框的值 - 效果
                if (Effect1 == true)
                    model.userEffects1 = true;
                else
                    model.userEffects1 = false;
                if (Effect2 == true)
                    model.userEffects2 = true;
                else
                    model.userEffects2 = false;
                if (Effect3 == true)
                    model.userEffects3 = true;
                else
                    model.userEffects3 = false;
                if (Effect4 == true)
                    model.userEffects4 = true;
                else
                    model.userEffects4 = false;
                if (Others2 == true)
                    model.userEffects5 = true;
                else
                    model.userEffects5 = false;
                model.userEffects6 = false;
                model.userEffects7 = false;
                model.ecnomicBenefit = ecnomicBenefit;
                model.userEffectDetail =userEffectDetail;
                model.seqNo = result;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    var ibuai = from b in db.BindUserAndImage
                                where b.realuserID == BusinessCode
                                select b;
                   
                  if(ibuai.Count()!=0)
                    {
                        
                        foreach (var dr in ibuai)
                        {
                            BindUserAndImage buaim = db.BindUserAndImage.Find(dr.ID);
                            if (buaim != null)
                            {
                                buaim.userID = result;
                                db.Entry(buaim).State = EntityState.Modified;
                               
                            }
                        }
                             db.SaveChanges();
                   }

                }
           
           
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
            if(seq.Count()==0)
            {
                
                
                     
                    sn = CurDate+"000";
                
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
            if (model.Count()!= 0)
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
        public string InsertFeesList(int businessOrder, string strFeeName, string strUnit, float fArea, string strDealMan,string strFromDepart, string[] strFeeTypes, decimal[] fFeeNumbers, string strIscharge, DateTime chargeTime, string chargeDetail, int nWhereTransfer)
        {
            
                string seqNo = "";
            for (int i = 0; i < strFeeTypes.Length; i++)
            {
                Charger model = new Charger();

                model.charger1 = "";
                model.searchNo = businessOrder;
                model.itemName = strFeeName;
                model.unitName = strUnit;
                model.buildingArea = fArea;
                model.@operator = strDealMan;
                model.fromDepartment = strFromDepart;
                //model.seqNo = seqNo;
                model.chargeClassify = Int32.Parse(strFeeTypes[i].ToString().Trim());
                model.totalExpense = fFeeNumbers[i];
                model.chargeDetail = chargeDetail;
                if (strIscharge == "True")
                    model.isCharge = true;
                else
                    model.isCharge = false;
                model.chargeTime = chargeTime;
                model.whereTransfer = nWhereTransfer;
                model.centiCnt = 0;
                model.isBack = false;
                model.backNote = string.Empty;
                model.chargeExtra = "";
                model.theoryExpense = 0;
                model.remarks = "";

                //增加一条数据,自动添加财务编号
                int id = GetMaxId();
                model.ID = id;
                string seqNostr = string.Empty;
                if (0 == i)
                {
                    seqNostr = getCurDayMaxSeqNo();
                    seqNo = seqNostr;
                }
                else
                {
                    seqNostr = seqNo;
                    seqNo = seqNostr;
                }
                model.seqNo = seqNostr;
                db.Charger.Add(model);
                db.SaveChanges();
            }
                
                return seqNo;
           
        }
        public int GetMaxId()
        {
            
            var id = from a in db.Charger
                     orderby a.ID descending
                     select a;
            long obj = id.First().ID + 1;
            if (id.First()==null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        protected bool updateChargeFee(out bool isfeemodify,string ID, string singleOrDepart,string borrower, string certificationFee,string  consultFee, string paoKufangRen,string borrowUnit,string WhereTransform)
        {
            
            isfeemodify = false;
           
            int BusinessCode = int.Parse(ID.Trim());
            string strFeeName, strFeeDepart, strUnitName;


            if (singleOrDepart.Trim() == "单位")
            {
                strFeeName = borrowUnit.Trim() + "借阅费";
                strUnitName = borrowUnit.Trim();
            }
            else
            {
                strFeeName = borrower.Trim() + "借阅费";
                strUnitName = borrower.Trim();
            }

            int iDepart;
            iDepart =2;
            strFeeDepart = iDepart.ToString();
            decimal[] feeNumbers = new decimal[3];
            string[] strFeeTypes = new string[3];
            int iType;

            feeNumbers[0] = decimal.Parse(certificationFee.Trim());
            iType = 6;
            strFeeTypes[0] = iType.ToString();
            string chargeDetail = "证明费：" + certificationFee.Trim();

            feeNumbers[1] = decimal.Parse(consultFee.Trim());
            iType = 5;
            strFeeTypes[1] = iType.ToString();
            chargeDetail += "，调阅费：" + consultFee.Trim();

            feeNumbers[2] = decimal.Parse(paoKufangRen.Trim());
            decimal tot = feeNumbers[0] + feeNumbers[1] + feeNumbers[2];
            iType = 9;
            strFeeTypes[2] = iType.ToString();
            chargeDetail += "，咨询费：" + paoKufangRen.Trim();

            chargeDetail += "，总计：" + tot.ToString();

            
                for (int i = 0; i < strFeeTypes.Length; i++)
                {
                    
                    int Class =Int32.Parse(strFeeTypes[i].ToString().Trim());
                    var model = from a in db.Charger
                                where a.searchNo == BusinessCode && a.fromDepartment == strFeeDepart && a.chargeClassify == Class
                                select a;

                    if (model.Count()!=0)
                    {
                        if (model.First().isCharge == true)
                        {
                            
                            return false;
                        }
                        model.First().isBack = false;
                        model.First().itemName = strFeeName;
                        model.First().unitName = strUnitName;
                        model.First().totalExpense = feeNumbers[i];
                        model.First().chargeDetail = chargeDetail;
                        model.First().whereTransfer =Int32.Parse(WhereTransform.Trim());
                        db.Entry(model.First()).State = EntityState.Modified;
                        db.SaveChanges();
                        
                       
                        isfeemodify = true;
                    }
                }

            
           
            return true;
        }
    }
}
