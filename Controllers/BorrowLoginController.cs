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
using Newtonsoft.Json;

namespace urban_archive.Controllers
{
    public class BorrowLoginController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        // GET: BorrowLogin
        public ActionResult Index(string id)

        {
            if (id ==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int userID = Convert.ToInt32(id);
            ViewData["fenID"] = userID.ToString();
            var model = from a in db.BorrowRegistration
                        where a.ID == userID
                        select a;
            var med= from b in db.BindUserAndArchives
                         where b.userID == userID
                         orderby b.archiveNo 
                         select b;
            if (model.Count()!=0)
            {
                ViewData["name"] = model.First().borrower;
            }
            ViewBag.result1 = JsonConvert.SerializeObject(med);//数据转换成JSON后传给前台
            return View();
        }
        
        public ActionResult Index1(string id)
        {

            int ID1 = 0;
            if (id!=null&&id!="")
            {
                 ID1 = Int32.Parse(id.Trim());
            }
            var model = from a in db.BorrowRegistration
                        where a.ID == ID1
                        select a;

            model.First().isJiesuanFee = 2;
            db.Entry(model.First()).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Effect", new { id = id });

        }
        public ActionResult Effect(long? id)
        {
            var model = from a in db.BorrowRegistration
                        where a.ID == id
                        select a;
            if(model.Count()!=0)
            {
                model.First().userEffectDetail = "";
                model.First().ecnomicBenefit = "";
            }

            return View(model.First());

        }
        [HttpPost]
        public ActionResult Effect(long? id,string action,string userEffectDetail, string ecnomicBenefit )
        {
            if(action=="返回")
            {
                return RedirectToAction("Index", new { id = id });
            }
            if(action=="保存")
            {
                var model = from a in db.BorrowRegistration
                            where a.ID == id
                            select a;
                string str = userEffectDetail.Trim();
                if (str == "")
                {
                    str = "好";
                }
                model.First().userEffectDetail = str;
                str = ecnomicBenefit.Trim();
                if (str == "")
                {
                    str = "好";
                }
                model.First().ecnomicBenefit = str;
                db.Entry(model.First()).State = EntityState.Modified;
                db.SaveChanges();
                return Content("<script >alert('保存成功，请到管理员处收费，谢谢！');location.href='http://localhost:59320/BorrowLogin/jieyuePersonLogin';</script >");
            }
            
           

            return View();

        }
        public ActionResult jieyuePersonLogin()
        {
            ViewData["pagename"] = "BorrowLogin/jieyuePersonLogin";
            return View();
        }
        [HttpPost]
        public ActionResult jieyuePersonLogin(string PassWord)
        {
            int id = 0;
            string UserName = "";
            if (PassWord.Trim() != "")
            {
                UserName = PassWord.Trim();
            }
            else
            {
                return Content("<script >alert('借阅人姓名不能为空！');window.history.back();</script >");
            
            }
            string password = "111111";
            DateTime date = DateTime.Now.Date;
            cacuCurDateEarlier ccde = new cacuCurDateEarlier(date);
            string before15Date = ccde.cacu15DaysEarlier();

            DateTime startdate =Convert.ToDateTime(before15Date.Trim());

          

            

            DateTime endtime = date;
            var set = from a in db.BorrowRegistration
                      where a.borrowDate >= startdate && a.borrowDate <= endtime && a.borrower==PassWord
                      orderby a.ID
                      select a;
            if (set.Count()!=0)
            {
                id =Convert.ToInt32(set.First().ID);
            }
            else
            {
                return Content("<script >alert('该用户尚未登记或权限已过期，请到工作人员处进行登记！');window.history.back();</script >");
                
            }
            var model = from a in db.BindUserAndArchives
                        from b in db.BorrowRegistration
                        where a.userID == b.ID && a.userID == id && a.bindDate >= startdate && a.bindDate <= endtime && b.password == password.Trim()
                        select a;
           
            if (model.Count()==0)
            {
                return Content("<script >alert('对不起，您无权查看案卷或用户已过期,请联系工作人员！');window.history.back();</script >");
                
              
            }
            return RedirectToAction("Index", new { id = id });
           
         

        
        }


        // GET: BorrowLogin/Details/5
        public ActionResult Details(string id,string id2)
        {
           
            string archive = "";
            if (id.Trim()=="1")//竣工档案
            {
                if(id2.IndexOf('/')==-1)
                {
                    archive = id2.Trim();
                }
                else
                {
                    archive = id2.Split('/')[1].ToString().Trim();
                }
                var model = from a in db.ArchivesDetail
                            where a.archivesNo== archive
                            select a;
                if(model.Count()==0)
                {
                    return Content("<script >alert('无相关工程信息！');window.close();</script >");
                }
                else
                {
                    string regist = model.First().registrationNo;
                    return RedirectToAction("anjuanzhuludan", new {id3 = regist.Trim()});
                }
                
            }
            if (id.Trim() == "2")//声像视频档案
            {

            }
            if (id.Trim() == "3")//声像照片档案
            {

            }
            if (id.Trim() == "4")//规划档案
            {

            }
            if (id.Trim() == "5")//其他档案
            {

            }
            if (id.Trim() == "6")// 征地档案
            {

            }
            if (id.Trim() == "7")//图纸档案
            {

            }
           
            return View();
        }
        public ActionResult anjuanzhuludan(string id3,string id4)
        {
            if (id3== null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
         
             var projectinfo = from b in db.vw_passList
                              where b.registrationNo == id3
                               select b;
            long paperseq = projectinfo.First().paperProjectSeqNo;
            string seq = paperseq.ToString().Trim();
            var project = from c in db.vw_passList
                          where c.paperProjectSeqNo == paperseq
                          orderby c.volNo
                          select c;
            if (projectinfo.Count() == 0)
            {
                ViewData["checkname1"] = 3;

            }
            //编制分类号
            string strfenleihao = projectinfo.First().mainCategoryID;
            if (projectinfo.First().subDictionaryID != null)
            {
                if (projectinfo.First().subDictionaryID.Trim() != "0")
                {


                    strfenleihao = strfenleihao + projectinfo.First().subDictionaryID;
                    if (projectinfo.First().minorDictionaryID.Trim() != "0")
                    {
                        strfenleihao = strfenleihao + "." + projectinfo.First().minorDictionaryID;

                    }

                }
            }
            ViewData["ClassNo"] = strfenleihao; //分类号
            ViewData["ProjectName"] = projectinfo.First().projectName;

            string jgdate = Convert.ToDateTime(projectinfo.First().jgDate).ToString("yyyy-MM-dd");
            ViewData["jgDate"] = jgdate;
            if (jgdate.Trim() == "1753-01-01")
            {
                ViewData["jgDate"] = "";
            }
            string bydate = Convert.ToDateTime(projectinfo.First().indexDate).ToString("yyyy-MM-dd");
            ViewData["indexeDate"] = bydate;
            if (bydate.Trim() == "1753-01-01")
            {
                ViewData["indexeDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
            }
            string jcdate = Convert.ToDateTime(projectinfo.First().checkDate).ToString("yyyy-MM-dd");
            ViewData["checkDate"] = jcdate;
            if (jcdate.Trim() == "1753-01-01")
            {
                ViewData["checkDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
            }
            string lrdate = Convert.ToDateTime(projectinfo.First().typerDate).ToString("yyyy-MM-dd");
            ViewData["TyperDate"] = lrdate;
            if (lrdate == "1753-01-01")
            {
                ViewData["TyperDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
            }

            ViewData["id4"] = id4;
  
            return View(projectinfo.First());
           
        }
        [HttpPost]
        public ActionResult anjuanzhuludan(string archivesNo, string action,string paperProjectSeqNo,string id4,string id5)
        {
            if(action=="显示工程信息")
            {
                var model = from a in db.vw_passList
                            where a.archivesNo == archivesNo
                            select a;
                if(model.Count()!=0)
                {
                    return RedirectToAction("ProjectInfoes", new { id3 = model.First().projectID, id6 = id5.Trim() });
                }
                else
                {
                    return Content("<script >alert('无相关工程信息！');window.history.back();</script >");
                }
            }
            if(action=="查看卷内目录")
            {
                return RedirectToAction("Juanneimulu", new { id1 =archivesNo.Trim() });
            }
           
            return View();

        }
        public ActionResult ArchiveResearch ()
        {

            return View();
        }
        public ActionResult Juanneimulu(string id1)
        {

            return View();

        }
        public ActionResult ProjectInfoes(long ?id3,string id6)
        {
            if (id3 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var projectinfo = from b in db.vw_passList
                              where b.projectID==id3
                              orderby b.paperProjectSeqNo,b.paijiaNo
                              select b;
            vw_passList pro = projectinfo.First();
             int wenziCnt = 0, tuzhiCnt = 0, photoCnt = 0;
            foreach (var item in projectinfo)
            {
                wenziCnt += Convert.ToInt32(item.textMaterial);
                tuzhiCnt += Convert.ToInt32(item.drawing);
                photoCnt += Convert.ToInt32(item.photoCount); 
            }
            pro.textMaterial = wenziCnt;
            pro.drawing = tuzhiCnt;
            pro.photoCount = photoCnt;
            //编制分类号

            string strfenleihao = pro.mainCategoryID;
            if (pro.subDictionaryID != null)
            {
                if (pro.subDictionaryID.Trim() != "0")
                {


                    strfenleihao = strfenleihao + pro.subDictionaryID;
                    if (pro.minorDictionaryID.Trim() != "0")
                    {
                        strfenleihao = strfenleihao + "." + pro.minorDictionaryID;

                    }

                }
            }
            ViewData["ClassNo"] = strfenleihao; //分类号
            ViewData["ProjectName"] = pro.projectName;

            string jgdate = Convert.ToDateTime(pro.jgDate).ToString("yyyy-MM-dd");
            ViewData["jgDate"] = jgdate;
            if (jgdate.Trim() == "1753-01-01")
            {
                ViewData["jgDate"] = "";
            }
            string bydate = Convert.ToDateTime(pro.indexDate).ToString("yyyy-MM-dd");
            ViewData["indexeDate"] = bydate;
            if (bydate.Trim() == "1753-01-01")
            {
                ViewData["indexeDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
            }
            string jcdate = Convert.ToDateTime(pro.checkDate).ToString("yyyy-MM-dd");
            ViewData["checkDate"] = jcdate;
            if (jcdate.Trim() == "1753-01-01")
            {
                ViewData["checkDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
            }
            string lrdate = Convert.ToDateTime(pro.typerDate).ToString("yyyy-MM-dd");
            ViewData["TyperDate"] = lrdate;
            if (lrdate == "1753-01-01")
            {
                ViewData["TyperDate"] = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-25";
            }
            ViewData["id6"] = id6;//传登url，用于工程著录单返回


            return View(pro);

            
        }
        [HttpPost]
        public ActionResult ProjectInfoes(string id6)
        {


            return Redirect(id6);
        }

        // GET: BorrowLogin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BorrowLogin/Create
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

        // GET: BorrowLogin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BorrowLogin/Edit/5
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

        // GET: BorrowLogin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BorrowLogin/Delete/5
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
        public class cacuCurDateEarlier
        {
            private DateTime date;
            public cacuCurDateEarlier(DateTime _date)
            {
                date = _date;
            }
            public string cacu15DaysEarlier()
            {
                string before15 = "";
                int day = date.Day;
                if (day > 15)//天数>15
                {
                    day -= 15;
                    before15 = date.Year.ToString() + "-" + date.Month.ToString() + "-" + day.ToString();
                }
                else//天数<=15,需向月份借一月
                {
                    int month = date.Month;
                    int year = date.Year;
                    month -= 1;
                    if (month == 0)//原月份为1月
                    {
                        month = 12;
                        year -= 1;
                        day += 16;
                    }
                    else if (month == 1)//原月份为2月
                    {
                        if (DateTime.IsLeapYear(date.Year) == true)//是闰年
                        {
                            day = day + 14;
                        }
                        else
                        {
                            day = day + 13;
                        }
                    }
                    else if (month == 2 || month == 4 || month == 6 || month == 7 || month == 9 || month == 11)//原月份为3,,5,7,8,10,12月
                    {
                        day = day + 16;
                    }
                    else//原月份为4，6，9，11月
                    {
                        day = day + 15;
                    }
                    before15 = year.ToString() + "-" + month.ToString() + "-" + day.ToString();
                }
                return before15;
            }
        }
    }
}
