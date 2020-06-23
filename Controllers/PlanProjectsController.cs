using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using urban_archive.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Microsoft.Reporting.WebForms;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.OleDb;
using CrystalDecisions.Web;
using PagedList;
using Newtonsoft.Json.Linq;

namespace urban_archive.Controllers
{
    public class PlanProjectsController : Controller
    {
        private PlanArchiveEntities db_plan = new PlanArchiveEntities();
        private UrbanConEntities db_urban = new UrbanConEntities();
        private UrbanUsersEntities db_user = new UrbanUsersEntities();
        private OfficeEntities db_office = new OfficeEntities();
        ReportDocument rptH = new ReportDocument();
        public ActionResult window()
        {
            return View();
        }
        public ActionResult window1()
        {
            return View();
        }
    
        public string window2(string DELETE_ID)
        {

                if (DELETE_ID != "")
                {
                    int deleteID = int.Parse(DELETE_ID);
                    PlanProjectCT planProjectCT = db_plan.PlanProjectCT.Where(a => a.ID == deleteID).First();
                    db_plan.PlanProjectCT.Remove(planProjectCT);
                    db_plan.SaveChanges();
                    return "1";
                }
                else {
                    return "2";
                }
                

            //return View();
        }
        // GET: PlanProjects
        //public ActionResult Index(int archiveID,int? classifyID,int? id,int id1)
        //{ 
        //    ViewBag.archiveID = archiveID;
        //    ViewBag.classifyID = classifyID;
        //    ViewBag.add = "display:none";
        //    ViewBag.edit = "display:none";
        //    var UserID = User.Identity.GetUserId();//获取当前用户
        //    var user = db_user.AspNetUsers.Find(UserID).RoleName;
        //    List<SelectListItem> listNeibu = new List<SelectListItem> {
        //        new SelectListItem { Text = "公开", Value = "0"},
        //        new SelectListItem { Text = "内部", Value = "1" }
        //    };
        //    ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text");
        //    ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", "周葛强");//审核人
        //    //if (user == "高级用户")
        //    //{
        //    //     classifyID = null;
        //    //    ViewBag.classifyID = classifyID;
        //    //}     
        //    if (classifyID != null)
        //    {
        //        ViewBag.classifyName = db_plan.PlanArchiveClassify.Find(classifyID).classifyName.Trim();
        //        PlanArchiveBox planarchivebox = db_plan.PlanArchiveBox.Find(archiveID);
        //        ViewBag.status = planarchivebox.status.Trim();
        //        ViewBag.archiveTitle = planarchivebox.archiveTitle;
        //        if (planarchivebox.status.ToString().Trim() != "LR")
        //        {
        //            ViewData["fanhui"] = "display:none";
        //        }
        //    }
        //    var planproject = from ad in db_plan.PlanProject
        //                      where ad.archiveID == archiveID
        //                       orderby ad.juanneiSeqNo
        //                       select ad;//按卷内序号排序
        //    ViewBag.result = JsonConvert.SerializeObject(planproject.ToList());
        //    if (id1 == 1)
        //    {
        //        ViewBag.add = "display:block";
        //        ViewBag.edit = "display:none";
        //        ViewBag.archiveID = archiveID;//案卷ID
        //        ViewBag.classifyID = classifyID;//案卷类型ID
        //        ViewBag.classifyName = db_plan.PlanArchiveClassify.Find(classifyID).classifyName; ;//案卷类型mame

        //        string title = db_plan.PlanArchiveBox.Find(archiveID).boxNo.ToString().Trim();
        //        if (title.IndexOf('字') > 0)
        //        {
        //            ViewBag.txtTitleHead = title.Substring(0, title.IndexOf('字'));//获得盒号的txtTitleHead传给前台
        //        }
        //        else if (title.IndexOf('-') > 0)
        //        {
        //            ViewBag.txtTitleHead = title.Substring(0, title.IndexOf('-'));//获得盒号的txtTitleHead传给前台
        //        }
        //        else
        //        {
        //            ViewBag.txtTitleHead = "";
        //        }
        //        var planproject1 = db_plan.PlanProject.Where(d => d.seqNo == archiveID).OrderByDescending(d => d.projectID);
        //        ViewBag.planproject = 1;//传递给前台，卷内序号和projiectID
        //        ViewBag.jbhao = 1;
        //        ViewBag.fileNo = title;
        //        if (planproject1.ToArray().Length > 0)
        //        {
        //            var jbhao = planproject1.FirstOrDefault().fileNo;//查询fileNo,为了截取“ouctest1字（2016）3、1号 ”中的1号   
        //            int index1 = jbhao.IndexOf('号');
        //            int index2 = jbhao.IndexOf('、');
        //            ViewBag.jbhao = jbhao.Substring(index2 + 1, index1 - index2 - 1);//传递给前台，用于合成新的fileNo，注意数据库中没有对应字段，可以修改
        //            int? max_projectID = planproject1.Max(d => d.projectID);//获得当前案卷号最大projectID
        //            ViewBag.planproject = max_projectID + 1;//自增
        //            int? max_juanhao = planproject1.Max(d => d.juanneiSeqNo);
        //            ViewBag.juannei = max_juanhao+1;

        //        }
        //        ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", "周葛强");//审核人
        //        ViewBag.luruPerson = db_user.AspNetUsers.Find(UserID).UserName;//录入人传到前台，前台设置成不可修改
        //    }
        //    if (id1 == 2)
        //    {
        //        ViewBag.add = "display:none";
        //        ViewBag.edit = "display:block";
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        ViewBag.archiveID = db_plan.PlanProject.Find(id).archiveID; ;
        //        ViewBag.classifyID = classifyID;//案卷类型ID
        //        ViewBag.classifyName = db_plan.PlanArchiveClassify.Find(classifyID).classifyName;
        //        PlanProject planProject = db_plan.PlanProject.Find(id);
        //        ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", planProject.isNeibu.Trim());

        //        var jbhao = planProject.fileNo;//查询fileNo,为了截取“ouctest1字（2016）3、1号 ”中的1号   
        //        ViewBag.jbhao = 1;
        //        int index1 = jbhao.IndexOf('号');
        //        int index2 = jbhao.IndexOf('、');
        //        if (index1 != -1 && index2 != -1)
        //            ViewBag.jbhao = jbhao.Substring(index2 + 1, index1 - index2 - 1);//传递给前台，用于合成新的fileNo，注意数据库中没有对应字段，可以修改
        //        ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", planProject.shenhePerson);//审核人
        //        string title = db_plan.PlanProject.Find(id).boxNo.ToString().Trim();
        //        if (title.IndexOf('字') > 0)
        //        {
        //            ViewBag.txtTitleHead = title.Substring(0, title.IndexOf('字'));//获得盒号的txtTitleHead传给前台
        //        }
        //        else if (title.IndexOf('-') > 0)
        //        {
        //            ViewBag.txtTitleHead = title.Substring(0, title.IndexOf('-'));//获得盒号的txtTitleHead传给前台
        //        }
        //        else
        //        {
        //            ViewBag.txtTitleHead = "";
        //        }
        //        if (planProject == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(planProject);
        //    }
        //    return View();
        //}
        public ActionResult leibieEdit(int? id)
        {
            PlanArchiveClassify planclassify = db_plan.PlanArchiveClassify.Find(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (planclassify == null)
            {
                return HttpNotFound();
            }
            return View(planclassify);
        }

        // POST: Office/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult leibieEdit([Bind(Include = "classifyID,classifyName,classifySX")] PlanArchiveClassify planclassify, string action)
        {
            if (action == "修改")
            {
                if (ModelState.IsValid)
                {
                    db_plan.Entry(planclassify).State = EntityState.Modified;
                    db_plan.SaveChanges();
                    return Content("<script>alert('修改成功！');window.location.href='/PlanProjects/leibieIndex';</script>");
                }
            }
            if (action == "返回")
            {
                return RedirectToAction("leibieIndex");
            }
            return View();
        }
        public ActionResult leibieDelete(int id)
        {
            PlanArchiveClassify planclassify = db_plan.PlanArchiveClassify.Find(id);
            db_plan.PlanArchiveClassify.Remove(planclassify);
            db_plan.SaveChanges();
            return Content("<script>alert('删除成功！');window.location.href='/PlanProjects/leibieIndex';</script>");
        }
        public ActionResult leibieIndex(string action)
        {
            var leibie = from ad in db_plan.PlanArchiveClassify
                             //orderby ad.leibie
                         orderby ad.classifyID
                         select ad;//按类别排序
            int maxid = leibie.ToList().Last().classifyID;
            ViewBag.id = maxid + 1;
            ViewBag.result = JsonConvert.SerializeObject(leibie);
            if (action == "添加")
            {
                PlanArchiveClassify planclassify = new PlanArchiveClassify();
                planclassify.classifyID = int.Parse(Request.Form["classifyID"]);
                planclassify.classifyName = Request.Form["classifyName"];
                planclassify.classifySX = Request.Form["classifySX"];
                db_plan.PlanArchiveClassify.Add(planclassify);
                db_plan.SaveChanges();
                return Content("<script>alert('保存成功！');window.location.href='/PlanProjects/leibieIndex';</script>");
            }
            return View();
        }
        public ActionResult GuiHuaGongChengMuLu(long id, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db_plan.PlanProject
                      where ad.seqNo1 == id
                      orderby ad.projectID,ad.fileNo
                      select ad;
            List<PlanProject> list = ds1.ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].fileNo != null)
                    list[i].fileNo = list[i].fileNo.Trim();
            }
            var ds = list;
            localReport.ReportPath = Server.MapPath("~/Report/guihua/JuanNeiMuLu.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("GongChengFengPi", ds);
            localReport.DataSources.Add(reportDataSource1);
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
        public ActionResult GuiHuaGongChengFengPi(long id, int id1)
        {
           
            db_plan.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider=SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT PlanProject.* FROM         PlanProject where ID='" + id + "' ";
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            DataSet ds = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            CrystalReportViewer repview = new CrystalReportViewer();
            adapter.Fill(ds);                ///////报表连接数据库,根据建水晶报表时的连接字符串设置
            ds.DataSetName = "GHDataSet";
            DataTable dt1 = ds.Tables[0];
            string archivetitle= dt1.Rows[0]["archiveTitle"].ToString().ToString().Replace("\r\n", " ").Replace("\n","").Trim();
            dt1.Rows[0]["archiveTitle"] = dt1.Rows[0]["archiveTitle"].ToString().ToString().Replace("\r\n"," ").Replace("\n", "").Trim();

            string mijiid = db_plan.PlanProject.Where(a => a.ID == id).First().securityID;
            string  miji = db_urban.SecurityClassification.Where(a => a.securityID == mijiid).First().securityName; ;
            string qixianid = db_plan.PlanProject.Where(a => a.ID == id).First().retentionPeriodID;
            string baoguanqixian = db_urban.RetentionPeriod.Where(a => a.retentionPeriodNo == qixianid).First().retentionPeriodName;
            string page1 = db_plan.PlanProject.Where(a => a.ID == id).First().pageNo;
            string seqNo1 = db_plan.PlanProject.Where(a => a.ID == id).First().seqNo1.ToString();
            string boxno = db_plan.PlanProject.Where(a => a.ID == id).First().boxNo;
            string bianzhiUnit = db_plan.PlanProject.Where(a => a.ID == id).First().bianzhiUnit;
            string isNei = "外部";
            if (id1 == 1)
            {
                dt1.Rows[0]["archiveTitle"] = dt1.Rows[0]["archiveTitle_neibu"].ToString().ToString().Replace("\r\n", " ").Replace("\n", "").Trim();
                page1 = db_plan.PlanProject.Where(a => a.ID == id).First().pageNo_neibu;
                isNei = "内部";
            }

                conn.Close();
            rptH.Load(Server.MapPath("~/") + "//Report//guihua//FengPi.rpt");
            rptH.SetDataSource(dt1);
            rptH.SetParameterValue("page", page1);
            rptH.SetParameterValue("seqNo", seqNo1);
            //rptH.SetParameterValue("boxNo", boxno);
            rptH.SetParameterValue("baoguanqixian", baoguanqixian);
            rptH.SetParameterValue("miji", miji);
            rptH.SetParameterValue("isNei", isNei);
            rptH.SetParameterValue("bianzhiUnit", bianzhiUnit);
            System.IO.Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
      
        public ActionResult GuiHuaAnJuanGongChengFengPi(long id, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db_plan.vw_printPlanArchiveBoxFengpi
                      where ad.ID == id
                      select ad;
            string seqno = db_plan.PlanProject.Where(a => a.ID == id).First().totalSeqNo;
            localReport.ReportPath = Server.MapPath("~/Report/guihua/GuiHuaAnJuanGongChengFengPi.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("GuiHuafengpi", ds1);
            localReport.DataSources.Add(reportDataSource1);
            List<ReportParameter> parameterList = new List<ReportParameter>();
            parameterList.Add(new ReportParameter("seqno", seqno.Trim()));
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

        //备考表
        public ActionResult beikaobiao(long id,int id1, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            if (id1 == 0)
            {
                var ds1 = from ad in db_plan.PlanProject
                          where ad.seqNo1 == id
                          where ad.isNeibu=="公开"||ad.isNeibu== "公开/内部"
                          select ad;
                localReport.ReportPath = Server.MapPath("~/Report/guihua/beikaobiaoG.rdlc");
                ReportDataSource reportDataSource1 = new ReportDataSource("GH", ds1);
                localReport.DataSources.Add(reportDataSource1);
            }
            if (id1 == 1)
            {
                var ds1 = from ad in db_plan.PlanProject
                          where ad.seqNo1 == id
                          where ad.isNeibu=="内部" || ad.isNeibu == "公开/内部"
                          select ad;
                localReport.ReportPath = Server.MapPath("~/Report/guihua/beikaobiaoN.rdlc");
                ReportDataSource reportDataSource1 = new ReportDataSource("GH", ds1);
                localReport.DataSources.Add(reportDataSource1);
            }
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
        public ActionResult Index_LR(int classifyID,  string currentFilter, string searchString,  int? SelectedID)
        { 
            ViewBag.classifyID = classifyID;
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID).RoleName;
            var username = db_user.AspNetUsers.Find(UserID).UserName;

            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "建设单位", Value = "0"},
                new SelectListItem { Text = "工程内容", Value = "1"},
                new SelectListItem { Text = "工程地点", Value = "2" },
                new SelectListItem { Text = "年份", Value = "3"},
                new SelectListItem { Text = "文件编号", Value = "4" },
                new SelectListItem { Text = "案卷题名", Value = "5"},
                new SelectListItem { Text = "公开、内部", Value = "6" }
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);


            var planproject = from ad in db_plan.PlanProject
                              where ad.classifyID==classifyID
                              where ad.status=="LR"
                              select ad;//

            ViewBag.quanxian = "display:none";
            if (user == "科员")
            {
                planproject = planproject.Where(ad => ad.luruPerson == username);
                ViewBag.quanxian = "float:left;";
            }
          
            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        planproject = planproject.Where(ad=>ad.developmentUnit.Contains(searchString));//根据建设单位检索
                        break;
                    case 1:
                        planproject = planproject.Where(ad => ad.projectContent.Contains(searchString));//根据工程内容检索
                        break;
                    case 2:
                        planproject = planproject.Where(ad => ad.projectLocation.Contains(searchString));//根据工程地点检索
                        break;
                    case 3:
                        planproject = planproject.Where(ad => ad.yearNo.Contains(searchString));//根据年份检索
                        break;
                    case 4:
                        planproject = planproject.Where(ad => ad.fileNo.Contains(searchString));//根据文件编号检索
                        break;
                    case 5:
                        planproject = planproject.Where(ad => ad.archiveTitle.Contains(searchString));//根据案卷题名检索
                        break;
                    case 6:
                        planproject = planproject.Where(ad => ad.isNeibu.Contains(searchString));//根据公开、内部检索
                        break;
                }
            }
            //var planproject1 = planproject.ToList().OrderByDescending(ad =>ad.yearNo).ThenByDescending(e => e.urban_type).ThenByDescending(e=>e.projectID).ThenByDescending(e=>e.ID);
            var planproject1 = planproject.ToList().OrderBy(ad => ad.urban_type).ThenBy(e => e.yearNo).ThenBy(g => g.isguajie).ThenBy(s => s.projectID).ThenBy(f => f.ID);
            ViewBag.result = JsonConvert.SerializeObject(planproject1);

            var select_year = planproject.GroupBy(p => new { p.yearNo }).Select(g => g.FirstOrDefault());
            ViewBag.yearNo = new SelectList(select_year, "yearNo", "yearNo");

            //var Urban_type = planproject.GroupBy(p => new { p.urban_type }).Select(g => g.FirstOrDefault());
            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "市规划", Value = "1" },
                new SelectListItem { Text = "市北规划", Value = "2" },
                new SelectListItem { Text = "原四方规划", Value = "3" },
                new SelectListItem { Text = "李沧规划", Value = "4" },
                new SelectListItem { Text = "市南规划", Value = "5" },
                new SelectListItem { Text = "崂山规划", Value = "6" },
                new SelectListItem { Text = "城阳规划", Value = "7" },
                new SelectListItem { Text = "黄岛规划", Value = "8" },
                new SelectListItem { Text = "胶州规划", Value = "9" },
                new SelectListItem { Text = "胶南规划", Value = "10" },
                new SelectListItem { Text = "平度规划", Value = "11" },
                new SelectListItem { Text = "莱西规划", Value = "12" },
                new SelectListItem { Text = "即墨规划", Value = "13" },
                new SelectListItem { Text = "开发区规划", Value = "14" },
            };
            ViewBag.quyu = new SelectList(list2, "Value", "Text", "1");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //传入
        public ActionResult Index_LR1(int classifyID, string yearNo, string quyu)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID).RoleName;
            var username = db_user.AspNetUsers.Find(UserID).UserName;

            int quyu1 = Int32.Parse(quyu);
            var planproject = from ad in db_plan.PlanProject
                              where ad.classifyID==classifyID
                              where ad.status == "LR"
                              where ad.luruPerson == username
                              where ad.yearNo == yearNo
                              where ad.urban_type == quyu1
                              select ad;//
            foreach (var item in planproject)//
            {
                item.status = "GD";//完成归档
                db_plan.Entry(item).State = EntityState.Modified;
            }
            try
            {
                db_plan.SaveChanges();
            }
            catch(Exception ex)
            {
                return Content("<script >保存失败！');window.history.back();</script >");
            }         
            return RedirectToAction("Index_LR", new { classifyID = classifyID });
        }
        public ActionResult Index_GD(int classifyID, string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            ViewBag.classifyID = classifyID;
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID).RoleName;
            var username = db_user.AspNetUsers.Find(UserID).UserName;

            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "建设单位", Value = "0"},
                new SelectListItem { Text = "工程内容", Value = "1"},
                new SelectListItem { Text = "工程地点", Value = "2" },
                new SelectListItem { Text = "年份", Value = "3"},
                new SelectListItem { Text = "文件编号", Value = "4" },
                new SelectListItem { Text = "案卷题名", Value = "5"},
                new SelectListItem { Text = "公开、内部", Value = "6" }
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);

            var planproject = from ad in db_plan.PlanProject
                              where (ad.classifyID == classifyID) && (ad.status == "GD" || ad.status == "SH")

                              select ad;
            ViewBag.quanxian = "display:none";
            if (user == "科员")
            {
                planproject = planproject.Where(ad => ad.luruPerson == username);
                ViewBag.quanxian = "float:left;";
            }
            //planproject = planproject.OrderBy(ad => ad.yearNo).ThenBy(ad => ad.projectID);
            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        planproject = planproject.Where(ad => ad.developmentUnit.Contains(searchString));//根据建设单位检索
                        break;
                    case 1:
                        planproject = planproject.Where(ad => ad.projectContent.Contains(searchString));//根据工程内容检索
                        break;
                    case 2:
                        planproject = planproject.Where(ad => ad.projectLocation.Contains(searchString));//根据工程地点检索
                        break;
                    case 3:
                        planproject = planproject.Where(ad => ad.yearNo.Contains(searchString));//根据年份检索
                        break;
                    case 4:
                        planproject = planproject.Where(ad => ad.fileNo.Contains(searchString));//根据文件编号检索
                        break;
                    case 5:
                        planproject = planproject.Where(ad => ad.archiveTitle.Contains(searchString));//根据案卷题名检索
                        break;
                    case 6:
                        planproject = planproject.Where(ad => ad.isNeibu.Contains(searchString));//根据公开、内部检索
                        break;
                }
            }


            var planproject1 = planproject.ToList().OrderBy(s => s.yearNo).ThenBy(ad => ad.urban_type).ThenBy(ad => ad.projectID).ThenBy(ad => ad.ID).ThenBy(ad => ad.fileNo).ThenBy(ad => ad.dijijuan);
            //var planproject1 = planproject.ToList().OrderBy(s => s.yearNo).ThenBy(ad => ad.urban_type).ThenBy(ad => ad.projectID).ThenBy(ad => ad.fileNo).ThenBy(ad => ad.dijijuan);
            ViewBag.result = JsonConvert.SerializeObject(planproject1);




            var select_year = planproject.GroupBy(p => new { p.yearNo }).Select(g => g.FirstOrDefault());
            ViewBag.yearNo = new SelectList(select_year, "yearNo", "yearNo");

            //var Urban_type = planproject.GroupBy(p => new { p.urban_type }).Select(g => g.FirstOrDefault());
            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "市规划", Value = "1" },
                new SelectListItem { Text = "市北规划", Value = "2" },
                new SelectListItem { Text = "原四方规划", Value = "3" },
                new SelectListItem { Text = "李沧规划", Value = "4" },
                new SelectListItem { Text = "市南规划", Value = "5" },
                new SelectListItem { Text = "崂山规划", Value = "6" },
                new SelectListItem { Text = "城阳规划", Value = "7" },
                new SelectListItem { Text = "黄岛规划", Value = "8" },
                new SelectListItem { Text = "胶州规划", Value = "9" },
                new SelectListItem { Text = "胶南规划", Value = "10" },
                new SelectListItem { Text = "平度规划", Value = "11" },
                new SelectListItem { Text = "莱西规划", Value = "12" },
                new SelectListItem { Text = "即墨规划", Value = "13" },
                new SelectListItem { Text = "开发区规划", Value = "14" },
            };
            ViewBag.quyu = new SelectList(list2, "Value", "Text", "1");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //传入
        public ActionResult Index_GD(int classifyID, string yearNo,string action,int quyu)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID).RoleName;
            var username = db_user.AspNetUsers.Find(UserID).UserName;
            if (action=="编号")
            {

                if (user == "科员")
                {
                    return Content("<script >alert('当前用户没有权限！');window.history.back();</script >");
                }
                if (yearNo == null)
                {
                    return Content("<script >alert('年份为空，请选择年份！');window.history.back();</script >");

                }
                var planproject = from ad in db_plan.PlanProject
                                  where ad.classifyID == classifyID
                                  where ad.status == "GD"
                                  where ad.yearNo == yearNo
                                  where ad.urban_type == quyu
                                  orderby ad.urban_type,ad.isguajie,ad.projectID,ad.fileNo,ad.dijijuan//同一类型、同一年份档案按文号排序->编号  
                                  select ad;
                if (planproject.Count() == 0)
                {
                    return Content("<script >alert('案卷已经编号，请勿重复编号！');window.history.back();</script >");
                }
                var box = from a in planproject
                          group a by a.boxNo
                          into c
                          select new
                          {
                              No = c.Min(b => b.projectID),
                              cnt = c.Count(),
                              file = c.FirstOrDefault().fileNo.Trim(),
                              wenzi = c.Sum(b => b.wenziCnt),
                              wenzi_neibu = c.Sum(b => b.wenziCnt_neibu),
                              drawing = c.Sum(b => b.photoCnt),
                              drawing_neibu = c.Sum(b => b.photoCnt_neibu),
                              classid = c.FirstOrDefault().classifyID,
                              classname = c.FirstOrDefault().yearNo,
                              vol = c.FirstOrDefault().dijijuan,
                              boxNo = c.FirstOrDefault().boxNo,
                              arear = c.FirstOrDefault().urban_type,
                              //securityID = c.FirstOrDefault().securityID,
                              //retentionPeriodID = c.FirstOrDefault().retentionPeriodID
                          };
                var box1 = from c in box
                           orderby c.arear, /*c.No,*/ c.file,c.vol
                           select c;//将盒子按其盒内最大的文号升序排列,然后再按照盒名升序排列
                if (box1.Count() != 0)
                {
                    int i = 1;


                    foreach (var item in box1)//为盒子编号
                    {
                        PlanArchiveBox boxi = new PlanArchiveBox();
                 
                        //var PlanBoxID = from a in db_plan.PlanArchiveBox
                        //                orderby a.ID
                        //                descending
                        //                select a;
                        //if (PlanBoxID.Count() == 0)
                        //{
                        //    PlanBoxID.FirstOrDefault().ID = 0;
                        //}
                        //PlanBoxID.First().ID = PlanBoxID.First().ID + i;
                        //var PlanBoxSeqno = from a in db_plan.PlanArchiveBox
                        //                orderby a.seqNo
                        //                descending
                        //                select a;
                        //if (PlanBoxSeqno.Count() == 0)
                        //{
                        //    PlanBoxSeqno.FirstOrDefault().seqNo = 0;
                        //}
                        //PlanBoxSeqno.First().seqNo = PlanBoxSeqno.First().seqNo + i;
                        var PlanBoxID = db_plan.PlanArchiveBox.Max(a => a.ID) + i;//盒子ID
                        var PlanBoxSeqno = db_plan.PlanArchiveBox.Max(a => a.seqNo) + i;//盒子号，对应planproject表中的seqno1
                        boxi.ID = PlanBoxID;
                        boxi.seqNo = PlanBoxSeqno;
                        boxi.boxNo = item.boxNo;
                        boxi.archiveBoxCnt = item.cnt;
                        boxi.textMatirial = item.wenzi + item.wenzi_neibu;
                        boxi.drawing = item.drawing + item.drawing_neibu;
                        boxi.classifyID = item.classid;
                        boxi.yearNo = item.classname;
                        boxi.archiveCode = "437402";
                        boxi.bianzhiUnit = "青岛市规划局";
                        boxi.storeLocation = "青岛市城建档案馆";
                        boxi.isNeibu = "否";
                        //boxi.securityID = item.securityID;
                        //boxi.retentionPeriodID = item.retentionPeriodID;
                        db_plan.PlanArchiveBox.Add(boxi);               
                        i++;
                    }
                    db_plan.SaveChanges();
                }




                string max_TotalSeqNo = db_plan.PlanProject.Max(s => s.totalSeqNo);//获得当前最大totalSeqNo,取前六位

                if (max_TotalSeqNo == "" || max_TotalSeqNo == null)//该数据为第一条
                {
                    max_TotalSeqNo = "00000000";
                }
                max_TotalSeqNo = max_TotalSeqNo.Trim().Substring(0, 8);
                int temp = int.Parse(max_TotalSeqNo) + 1;//总顺序号的前六位 +1

                foreach (var item in planproject)//编号，seqno作为总顺序号
                {
                    string cur_max = temp.ToString().PadLeft(8, '0');//左边填充0，补齐8位  
                    item.projectno = cur_max;//保存工程序号前8位
                    if (item.gongjijuan > 1)//如果一个工程有多卷，则需要
                    {
                        item.totalSeqNo = cur_max + "-" + item.dijijuan.ToString().PadLeft(3, '0');//编号示例：000005-001
                        var planbox = from a in db_plan.PlanArchiveBox
                                      where a.boxNo==item.boxNo
                                      select a;//盒子已经编完号了，为每个工程装盒
                        if (planbox.Count() != 0)
                        {
                            item.seqNo = planbox.First().ID;
                            item.seqNo1 = planbox.First().seqNo;
                        }
                        if (item.gongjijuan == item.dijijuan)
                        {
                            temp += 1;
                        }
                    }
                    else
                    {//编号示例：000005
                        var planbox = from a in db_plan.PlanArchiveBox
                                      where a.boxNo == item.boxNo
                                      select a;//盒子已经编完号了，为每个工程装盒
                        if (planbox.Count() != 0)
                        {
                            item.seqNo = planbox.First().ID;
                            item.seqNo1 = planbox.First().seqNo;
                        }
                        item.totalSeqNo = cur_max;//totalSeqNo赋值为当前最大号+1
                        temp += 1;
                    }
                    //var totalseqno = from a in db_plan.PlanProject
                    //                 select a;

                    //foreach (var tono in totalseqno) {
                    //    if (item.totalSeqNo == tono.totalSeqNo) {
                    //        return Content("<script>alert('当前编号已存在，请检查！');window.history.back();</script >");
                    //    }
                    //}
                    item.status="SH";//完成编号
                    db_plan.Entry(item).State = EntityState.Modified;
                }
                try
                {
                    db_plan.SaveChanges();
                    //return Content("<script >保存成功！');window.history.back();</script >");
                    //return Content("<script >alert('编号成功');window.location.href='/PlanProjects/Index_GD/?classifyID='" + classifyID + ";</script >");
                    return RedirectToAction("Index_GD", new { classifyID = classifyID });
                }
                catch (Exception ex)
                {
                    return Content("<script >alert('保存失败！');window.history.back();</script >");
                }
            }
            if(action=="取消编号")
            {
                if (user == "科员")
                {
                    return Content("<script >alert('当前用户没有权限！');window.history.back();</script >");
                }
              
                var planproject1 = from ad in db_plan.PlanProject
                                  where ad.classifyID == classifyID
                                  where ad.status=="SH"
                                   where ad.yearNo == yearNo
                                   where ad.urban_type == quyu
                                   orderby ad.projectID//同一类型、同一年份档案按文号排序->编号                      
                                  select ad;//检索出已经编号的档案
                var box = from b in db_plan.PlanProject
                          where b.classifyID == classifyID
                          where b.status == "SH"
                          where b.yearNo == yearNo
                          where b.urban_type == quyu
                          group b by b.boxNo
                         into c
                          select new
                          {
                              No = c.FirstOrDefault().boxNo
                          };
                foreach (var item2 in planproject1)//给每一个案卷的盒子号，盒子ID，工程总顺序号赋空
                {
                    item2.seqNo= 0;
                    item2.seqNo1= 0;
                    item2.totalSeqNo="";
                    item2.status = "GD";
                    db_plan.Entry(item2).State = EntityState.Modified;
                    
                }
                foreach (var item3 in box)//删除盒子
                {
                    var c = from e in db_plan.PlanArchiveBox
                            where e.boxNo == item3.No
                            select e;
                    if (c.Count() != 0) {
                        db_plan.PlanArchiveBox.Remove(c.First());
                    }
                }
                db_plan.SaveChanges();
            }
            if(action == "审核通过")
            {
                if (user == "科员")
                {
                    return Content("<script >alert('当前用户没有权限！');window.history.back();</script >");
                }



                var planproject1 = from ad in db_plan.PlanProject
                                   where ad.classifyID == classifyID
                                   where ad.status == "SH"
                                   where ad.yearNo == yearNo
                                   where ad.urban_type == quyu
                                   orderby ad.urban_type, ad.projectID, ad.fileNo, ad.dijijuan//同一类型、同一年份档案按文号排序->编号                      
                                   select ad;//检索
                foreach (var item4 in planproject1)//给每一个案卷的盒子号，盒子ID，工程总顺序号赋空
                {
                    item4.status = "BH";
                    db_plan.Entry(item4).State = EntityState.Modified;
                   
                }
                db_plan.SaveChanges();
                return RedirectToAction("Index_GD", new { classifyID = classifyID });
            }
            return RedirectToAction("Index_GD", new { classifyID = classifyID });
        }
        public ActionResult Index_BH(int classifyID, string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            ViewBag.classifyID = classifyID;
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID).RoleName;
            var username = db_user.AspNetUsers.Find(UserID).UserName;

            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "建设单位", Value = "0"},
                new SelectListItem { Text = "工程内容", Value = "1"},
                new SelectListItem { Text = "工程地点", Value = "2" },
                new SelectListItem { Text = "年份", Value = "3"},
                new SelectListItem { Text = "文件编号", Value = "4" },
                new SelectListItem { Text = "案卷题名", Value = "5"},
                new SelectListItem { Text = "公开、内部", Value = "6" }
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);


            var planproject = from ad in db_plan.PlanProject
                              where ad.classifyID == classifyID
                              where ad.status == "BH"
                              select ad;//

            ViewBag.quanxian = "display:none";

            if (user == "科员")
            {
                planproject = planproject.Where(ad => ad.luruPerson == username);
                ViewBag.quanxian = "float:left;";
                ViewBag.CurrentUser = "科员";

            }
            //planproject = planproject.OrderBy(ad => ad.yearNo).ThenBy(ad => ad.projectID);
            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        planproject = planproject.Where(ad => ad.developmentUnit.Contains(searchString));//根据建设单位检索
                        break;
                    case 1:
                        planproject = planproject.Where(ad => ad.projectContent.Contains(searchString));//根据工程内容检索
                        break;
                    case 2:
                        planproject = planproject.Where(ad => ad.projectLocation.Contains(searchString));//根据工程地点检索
                        break;
                    case 3:
                        planproject = planproject.Where(ad => ad.yearNo.Contains(searchString));//根据年份检索
                        break;
                    case 4:
                        planproject = planproject.Where(ad => ad.fileNo.Contains(searchString));//根据文件编号检索
                        break;
                    case 5:
                        planproject = planproject.Where(ad => ad.archiveTitle.Contains(searchString));//根据案卷题名检索
                        break;
                    case 6:
                        planproject = planproject.Where(ad => ad.isNeibu.Contains(searchString));//根据公开、内部检索
                        break;
                }
            }
            //planproject = planproject.OrderBy(s => s.yearNo).ThenBy(ad => ad.fileNo);
            planproject = planproject.OrderBy(s => s.totalSeqNo).ThenBy(a=>a.seqNo1);
            ViewBag.result = JsonConvert.SerializeObject(planproject.ToList());

            var select_year = planproject.GroupBy(p => new { p.yearNo }).Select(g => g.FirstOrDefault());
            ViewBag.yearNo = new SelectList(select_year, "yearNo", "yearNo");

            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "市规划", Value = "1" },
                new SelectListItem { Text = "市北规划", Value = "2" },
                new SelectListItem { Text = "原四方规划", Value = "3" },
                new SelectListItem { Text = "李沧规划", Value = "4" },
                new SelectListItem { Text = "市南规划", Value = "5" },
                new SelectListItem { Text = "崂山规划", Value = "6" },
                new SelectListItem { Text = "城阳规划", Value = "7" },
                new SelectListItem { Text = "黄岛规划", Value = "8" },
                new SelectListItem { Text = "胶州规划", Value = "9" },
                new SelectListItem { Text = "胶南规划", Value = "10" },
                new SelectListItem { Text = "平度规划", Value = "11" },
                new SelectListItem { Text = "莱西规划", Value = "12" },
                new SelectListItem { Text = "即墨规划", Value = "13" },
                new SelectListItem { Text = "开发区规划", Value = "14" },
            };
            ViewBag.quyu = new SelectList(list2, "Value", "Text", "1");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index_BH1(int classifyID, string yearNo, int quyu)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID).RoleName;
            var username = db_user.AspNetUsers.Find(UserID).UserName;
            var planproject = from ad in db_plan.PlanProject
                              where ad.classifyID == classifyID
                              where ad.status == "BH"
                              where ad.yearNo == yearNo
                              where ad.urban_type == quyu
                              select ad;//
            if (user == "科员")
            {
                planproject = planproject.Where(a => a.luruPerson == username); 
            }
            foreach (var item in planproject)//
            {
                item.status = "RK";//完成入库
                item.rukutime = DateTime.Now;
                db_plan.Entry(item).State = EntityState.Modified;
            }
            try
            {
                db_plan.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content("<script >保存失败！');window.history.back();</script >");
            }
            return RedirectToAction("Index_BH", new { classifyID = classifyID });
        }

        public ActionResult Index_RK(int classifyID, string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            ViewBag.classifyID = classifyID;
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID).RoleName;
            var username = db_user.AspNetUsers.Find(UserID).UserName;

            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "建设单位", Value = "0"},
                new SelectListItem { Text = "工程内容", Value = "1"},
                new SelectListItem { Text = "工程地点", Value = "2" },
                new SelectListItem { Text = "年份", Value = "3"},
                new SelectListItem { Text = "文件编号", Value = "4" },
                new SelectListItem { Text = "案卷题名", Value = "5"},
                new SelectListItem { Text = "公开、内部", Value = "6" }
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);


            var planproject = from ad in db_plan.PlanProject
                              where ad.classifyID == classifyID
                              where ad.status == "RK"
                              select ad;//

            ViewBag.quanxian = "display:none";
            
            if (user == "科员")
            {
                planproject = planproject.Where(ad => ad.luruPerson == username);
                ViewBag.quanxian = "float:left;";
                ViewBag.CurrentUser = "科员";
            }
            //planproject = planproject.OrderBy(ad => ad.yearNo).ThenBy(ad => ad.projectID);
            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        planproject = planproject.Where(ad => ad.developmentUnit.Contains(searchString));//根据建设单位检索
                        break;
                    case 1:
                        planproject = planproject.Where(ad => ad.projectContent.Contains(searchString));//根据工程内容检索
                        break;
                    case 2:
                        planproject = planproject.Where(ad => ad.projectLocation.Contains(searchString));//根据工程地点检索
                        break;
                    case 3:
                        planproject = planproject.Where(ad => ad.yearNo.Contains(searchString));//根据年份检索
                        break;
                    case 4:
                        planproject = planproject.Where(ad => ad.fileNo.Contains(searchString));//根据文件编号检索
                        break;
                    case 5:
                        planproject = planproject.Where(ad => ad.archiveTitle.Contains(searchString));//根据案卷题名检索
                        break;
                    case 6:
                        planproject = planproject.Where(ad => ad.isNeibu.Contains(searchString));//根据公开、内部检索
                        break;
                }
            }
            //planproject = planproject.OrderBy(s => s.yearNo).ThenBy(ad => ad.fileNo);
            planproject = planproject.OrderBy(s =>s.totalSeqNo);
            ViewBag.result = JsonConvert.SerializeObject(planproject.ToList());
            return View();
        }

        public ActionResult Index_ALL(int classifyID, string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            ViewBag.classifyID = classifyID;
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID).RoleName;
            var username = db_user.AspNetUsers.Find(UserID).UserName;

            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "建设单位", Value = "0"},
                new SelectListItem { Text = "工程内容", Value = "1"},
                new SelectListItem { Text = "工程地点", Value = "2" },
                new SelectListItem { Text = "年份", Value = "3"},
                new SelectListItem { Text = "文件编号", Value = "4" },
                new SelectListItem { Text = "案卷题名", Value = "5"},
                new SelectListItem { Text = "公开、内部", Value = "6" }
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);


            var planproject = from ad in db_plan.PlanProject
                              where ad.classifyID == classifyID
                              select ad;

            ViewBag.quanxian = "display:none";
            
            if (user == "科员")
            {
                planproject = planproject.Where(ad => ad.luruPerson == username);
                ViewBag.quanxian = "float:left;";
                ViewBag.CurrentUser = "科员";
            }
            //planproject = planproject.OrderBy(ad => ad.yearNo).ThenBy(ad => ad.projectID);
            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        planproject = planproject.Where(ad => ad.developmentUnit.Contains(searchString));//根据建设单位检索
                        break;
                    case 1:
                        planproject = planproject.Where(ad => ad.projectContent.Contains(searchString));//根据工程内容检索
                        break;
                    case 2:
                        planproject = planproject.Where(ad => ad.projectLocation.Contains(searchString));//根据工程地点检索
                        break;
                    case 3:
                        planproject = planproject.Where(ad => ad.yearNo.Contains(searchString));//根据年份检索
                        break;
                    case 4:
                        planproject = planproject.Where(ad => ad.fileNo.Contains(searchString));//根据文件编号检索
                        break;
                    case 5:
                        planproject = planproject.Where(ad => ad.archiveTitle.Contains(searchString));//根据案卷题名检索
                        break;
                    case 6:
                        planproject = planproject.Where(ad => ad.isNeibu.Contains(searchString));//根据公开、内部检索
                        break;
                }
            }

            planproject = planproject.Take(1000).OrderBy(s => s.totalSeqNo).ThenBy(s => s.yearNo).ThenBy(s => s.projectID);
            ViewBag.result = JsonConvert.SerializeObject(planproject.ToList());
            return View();
        }
        public ActionResult ALLPlanproject()
        {
         
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "建设单位", Value = "0"},
                new SelectListItem { Text = "工程内容", Value = "1"},
                new SelectListItem { Text = "工程地点", Value = "2" },
                new SelectListItem { Text = "年份", Value = "3"},
                new SelectListItem { Text = "文件编号", Value = "4" },
                new SelectListItem { Text = "案卷题名", Value = "5"},
                new SelectListItem { Text = "公开、内部", Value = "6" },
                 new SelectListItem { Text = "盒号", Value = "7" }
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text",0);
            List<SelectListItem> list1 = new List<SelectListItem> {
                
                new SelectListItem { Text = "录入", Value = "0"},
                new SelectListItem { Text = "归档", Value = "1"},
                new SelectListItem { Text = "编号", Value = "2" },
                new SelectListItem { Text = "审核", Value = "5" },
                new SelectListItem { Text = "入库", Value = "3"},
                new SelectListItem { Text = "全部", Value = "4"}

            };
            ViewBag.SelectedID1 = new SelectList(list1, "Value", "Text",4);


            return View();
        }
        public string ALLPlanprojectData(int? page, string type,string type1, string content)
        {
            ViewBag.type = type;
            ViewBag.content = content;
            var vwprojectFile = from ad in db_plan.PlanProject
                                select ad;
            if(type1!=null&&type1!=null)
            {
                int t = Int32.Parse(type1.Trim());
               
                    switch (t)
                    {
                        case 0:
                            vwprojectFile = vwprojectFile.Where(ad => ad.status=="LR");//根据责任书编号搜索
                            break;
                        case 1:
                            vwprojectFile = vwprojectFile.Where(ad => ad.status=="GD");//根据工程名称搜索
                            break;
                        case 2:
                            vwprojectFile = vwprojectFile.Where(ad => ad.status=="BH");//根据建设单位搜索
                            break;
                        case 3:
                            vwprojectFile = vwprojectFile.Where(ad => ad.status=="RK");//根据工程地点
                            break;
                        case 4:

                        
                            break;
                    case 5:
                        vwprojectFile = vwprojectFile.Where(ad => ad.status == "SH");//根据工程地点
                        break;


                }
            }
            if (type != "" && type != null && content != "" && content != null)//用户在检索框中输入了检索条件
            {
                int t = Int32.Parse(type.Trim());
                if (!String.IsNullOrEmpty(content))
                {
                    switch (t)
                    {
                        case 0:
                            vwprojectFile = vwprojectFile.Where(ad => ad.developmentUnit.Contains(content));//根据责任书编号搜索
                            break;
                        case 1:
                            vwprojectFile = vwprojectFile.Where(ad => ad.projectContent.Contains(content));//根据工程名称搜索
                            break;
                        case 2:
                            vwprojectFile = vwprojectFile.Where(ad => ad.projectLocation.Contains(content));//根据建设单位搜索
                            break;
                        case 3:
                            vwprojectFile = vwprojectFile.Where(ad => ad.yearNo.Contains(content));//根据工程地点
                            break;
                        case 4:

                            vwprojectFile = vwprojectFile.Where(ad => ad.fileNo.Contains(content)); //根据工程序号
                            break;
                        case 5:
                            vwprojectFile = vwprojectFile.Where(ad => ad.archiveTitle.Contains(content));//根据施工单位
                            break;
                        case 6:
                            vwprojectFile = vwprojectFile.Where(ad => ad.isNeibu.Contains(content));//根据设计单位
                            break;
                        case 7:
                            vwprojectFile = vwprojectFile.Where(ad => ad.seqNo1.ToString().Contains(content));//根据设计单位
                            break;


                    }

                }

            }


            int pageSize = 100;
            int pageNumber = (page ?? 1);
            int cnt = vwprojectFile.Count() / pageSize + 1;
            if (vwprojectFile.Count() % pageSize == 0)
            {
                cnt = vwprojectFile.Count() / pageSize;
            }
            //vwprojectFile = vwprojectFile.OrderBy(s => s.yearNo).ThenBy(s => s.urban_type).ThenBy(s => s.classifyID).ThenBy(s => s.projectID).ThenBy(s => s.ID).ThenBy(s => s.fileNo).ThenBy(s => s.dijijuan);
            //vwprojectFile = vwprojectFile.OrderBy(s => s.yearNo).ThenBy(s => s.urban_type).ThenBy(s => s.classifyID).ThenBy(s => s.projectID).ThenBy(s => s.fileNo).ThenBy(s => s.dijijuan);

            vwprojectFile = vwprojectFile.OrderBy(s => s.urban_type).ThenBy(s => s.yearNo).ThenBy(s => s.classifyID).ThenBy(s => s.projectID).ThenBy(s => s.ID).ThenBy(s => s.fileNo).ThenBy(s => s.dijijuan);
            var a = vwprojectFile.ToPagedList(pageNumber, pageSize);
            foreach (var item in a)
            {
                string e = item.status.Trim();

                switch (e)
                {
                    case "LR":
                        item.status = "录入";
                        break;
                    case "GD":
                        item.status = "归档";
                        break;
                    case "BH":
                        item.status = "编号";
                        break;
                    case "RK":
                        item.status = "入库";
                        break;
                  
                    case "SH":
                        item.status = "审核";
                        break;


                }

            }
            var b = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                                        //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程
                                        from p in a
                                        select new JObject(
                                                 new JProperty("totalSeqNo", p.totalSeqNo),
                                                 new JProperty("SeqNo", p.seqNo1),
                                                 new JProperty("boxNo", p.boxNo),
                                                 new JProperty("luruPerson", p.luruPerson),
                                                 new JProperty("fileNo", p.fileNo),
                                                 new JProperty("developmentUnit", p.developmentUnit),
                                                 new JProperty("projectContent", p.projectContent),
                                                 new JProperty("projectLocation", p.projectLocation),
                                                 new JProperty("status", p.status),
                                                 new JProperty("pageNo", p.pageNo),
                                                 new JProperty("remarks", p.remarks),
                                                 new JProperty("bianzhiTime", p.bianzhiTime),
                                                 new JProperty("isNeibu", p.isNeibu),
                                                  new JProperty("ID", p.ID)

                                        )
                                )
                    )
).ToString();
            return b;

        
        }
        // GET: PlanProjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                
                return Content("<script >alert('记录不存在！');window.history.back();</script >");
            }
            PlanProject planProject = db_plan.PlanProject.Find(id);
            ViewBag.classifyID = planProject.classifyID;//案卷类型ID
            ViewBag.classifyName = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName;
            ViewBag.classiftsx = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifySX;//案卷类型缩写，为了拼接盒号
            ViewBag.box = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName.Trim() + "-" + db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifySX.Trim();
            ViewBag.year = planProject.yearNo;
            int a = planProject.boxNo.IndexOf('第') + 1;
            int b = planProject.boxNo.Trim().Length;
            int c = planProject.boxNo.IndexOf('第');
            ViewBag.boxid = planProject.boxNo.Substring(a, b - c - 2);
            ViewBag.dateReceived = planProject.dateReceived.Value.ToString("yyyy.MM.dd");
            if (planProject.securityID != null && planProject.securityID != "")
            {
                switch (planProject.securityID)
                {
                    case "1":
                        planProject.securityID = "机密";
                        break;
                    case "2":
                        planProject.securityID = "秘密";
                        break;
                    case "3":
                        planProject.securityID = "绝密";
                        break;
                    case "4":
                        planProject.securityID = "一般";
                        break;
                    case "5":
                        planProject.securityID = "内部";
                        break;
                    case "6":
                        planProject.securityID = "公开/内部";
                        break;


                }

            }
            if (planProject.retentionPeriodID != null && planProject.retentionPeriodID != "")
            {
                switch (planProject.retentionPeriodID)
                {
                    case "1":
                        planProject.retentionPeriodID = "长期";
                        break;
                    case "2":
                        planProject.retentionPeriodID = "永久";
                        break;
                    case "3":
                        planProject.retentionPeriodID = "短期";
                        break;
                }

            }
          
            ViewBag.fanhui = "Index_LR";
            if (planProject.status.Trim() == "GD")
            {
                ViewBag.fanhui = "Index_GD";
            }
            else if(planProject.status.Trim() == "BH")
            {
                ViewBag.fanhui = "Index_BH";
            }
            else if(planProject.status.Trim() == "RK")
            {
                ViewBag.fanhui = "Index_RK";
            }
            else
            {
                ViewBag.fanhui = "Index_LR";
            }
            if (planProject.totalSeqNo != "" && planProject.totalSeqNo != null)
            {


                if (planProject.isNeibu.Trim() == "公开")
                {

                    ViewData["fengpiG"] = "inline-block";
                    ViewData["beikaobiaoG"] = "inline-block";
                    ViewData["fengpiN"] = "display: none";
                    ViewData["beikaobiaoN"] = "display: none";
                }
                if (planProject.isNeibu.Trim() == "内部")
                {
                    ViewData["fengpiG"] = "display: none";
                    ViewData["beikaobiaoG"] = "display: none";
                    ViewData["fengpiN"] = "inline-block";
                    ViewData["beikaobiaoN"] = "inline-block";
                }
                if (planProject.isNeibu.Trim() == "公开/内部")
                {
                    ViewData["fengpiG"] = "inline-block";
                    ViewData["beikaobiaoG"] = "inline-block";
                    ViewData["fengpiN"] = "inline-block";
                    ViewData["beikaobiaoN"] = "inline-block";
                }
            }
            else
            {
                ViewData["fengpiG"] = "display: none";
                ViewData["beikaobiaoG"] = "display: none";
                ViewData["fengpiN"] = "display: none";
                ViewData["beikaobiaoN"] = "display: none";
                ViewData["ProjectContent"] = "display: none";
            }
            return View(planProject);
        }
        public ActionResult DetailsAllplan(int? id)
        {
            if (id == null)
            {

                return Content("<script >alert('记录不存在！');window.history.back();</script >");
            }
            PlanProject planProject = db_plan.PlanProject.Find(id);
            ViewBag.classifyID = planProject.classifyID;//案卷类型ID
            ViewBag.classifyName = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName;
            ViewBag.classiftsx = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifySX;//案卷类型缩写，为了拼接盒号
            ViewBag.box = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName.Trim() + "-" + db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifySX.Trim();
            ViewBag.year = planProject.yearNo;
            int a = planProject.boxNo.IndexOf('第') + 1;
            int b = planProject.boxNo.Trim().Length;
            int c = planProject.boxNo.IndexOf('第');
            ViewBag.boxid = planProject.boxNo.Substring(a, b - c - 2);
            if (planProject.securityID != null && planProject.securityID != "")
            {
                switch (planProject.securityID)
                {
                    case "1":
                        planProject.securityID = "机密";
                        break;
                    case "2":
                        planProject.securityID = "秘密";
                        break;
                    case "3":
                        planProject.securityID = "绝密";
                        break;
                    case "4":
                        planProject.securityID = "一般";
                        break;
                    case "5":
                        planProject.securityID = "内部";
                        break;
                    case "6":
                        planProject.securityID = "公开/内部";
                        break;


                }

            }
            if (planProject.retentionPeriodID != null && planProject.retentionPeriodID != "")
            {
                switch (planProject.retentionPeriodID)
                {
                    case "1":
                        planProject.retentionPeriodID = "长期";
                        break;
                    case "2":
                        planProject.retentionPeriodID = "永久";
                        break;
                    case "3":
                        planProject.retentionPeriodID = "短期";
                        break;
                }

            }

            ViewBag.fanhui = "Index_LR";
            if (planProject.status.Trim() == "GD")
            {
                ViewBag.fanhui = "Index_GD";
            }
            else if (planProject.status.Trim() == "BH")
            {
                ViewBag.fanhui = "Index_BH";
            }
            else if (planProject.status.Trim() == "RK")
            {
                ViewBag.fanhui = "Index_RK";
            }
            else
            {
                ViewBag.fanhui = "Index_LR";
            }
            if (planProject.totalSeqNo != "" && planProject.totalSeqNo != null)
            {


                if (planProject.isNeibu.Trim() == "公开")
                {

                    ViewData["fengpiG"] = "inline-block";
                    ViewData["beikaobiaoG"] = "inline-block";
                    ViewData["fengpiN"] = "display: none";
                    ViewData["beikaobiaoN"] = "display: none";
                }
                if (planProject.isNeibu.Trim() == "内部")
                {
                    ViewData["fengpiG"] = "display: none";
                    ViewData["beikaobiaoG"] = "display: none";
                    ViewData["fengpiN"] = "inline-block";
                    ViewData["beikaobiaoN"] = "inline-block";
                }
                if (planProject.isNeibu.Trim() == "公开/内部")
                {
                    ViewData["fengpiG"] = "inline-block";
                    ViewData["beikaobiaoG"] = "inline-block";
                    ViewData["fengpiN"] = "inline-block";
                    ViewData["beikaobiaoN"] = "inline-block";
                }
            }
            else
            {
                ViewData["fengpiG"] = "display: none";
                ViewData["beikaobiaoG"] = "display: none";
                ViewData["fengpiN"] = "display: none";
                ViewData["beikaobiaoN"] = "display: none";
                ViewData["ProjectContent"] = "display: none";
            }
            return View(planProject);
        }
        // GET: PlanProjects/Create
        public ActionResult Create(int classifyID,string id2,string ID,string id1, string projectname, string projectLocation, string developmentUnit, string projectContent, string projectContent_neibu, string archiveTitle, string archiveTitle_neibu)
        {
            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "市规划", Value = "1" },
                new SelectListItem { Text = "市北规划", Value = "2" },
                new SelectListItem { Text = "原四方规划", Value = "3" },
                new SelectListItem { Text = "李沧规划", Value = "4" },
                new SelectListItem { Text = "市南规划", Value = "5" },
                new SelectListItem { Text = "崂山规划", Value = "6" },
                new SelectListItem { Text = "城阳规划", Value = "7" },
                new SelectListItem { Text = "黄岛规划", Value = "8" },
                new SelectListItem { Text = "胶州规划", Value = "9" },
                new SelectListItem { Text = "胶南规划", Value = "10" },
                new SelectListItem { Text = "平度规划", Value = "11" },
                new SelectListItem { Text = "莱西规划", Value = "12" },
                new SelectListItem { Text = "即墨规划", Value = "13" },
                new SelectListItem { Text = "开发区规划", Value = "14" },
            };
            ViewBag.quyu = new SelectList(list2, "Value", "Text", "1");
            ViewBag.classifyID = classifyID;//案卷类型ID
            ViewBag.classifyName = db_plan.PlanArchiveClassify.Find(classifyID).classifyName.Trim();//案卷类型mame
            ViewBag.classiftsx = db_plan.PlanArchiveClassify.Find(classifyID).classifySX;//案卷类型缩写，为了拼接盒号
            ViewBag.box = db_plan.PlanArchiveClassify.Find(classifyID).classifyName.Trim()+ "-"+db_plan.PlanArchiveClassify.Find(classifyID).classifySX.Trim();
            //ViewBag.classname = db_plan.PlanArchiveClassify.Find(classifyID).classifyName.Trim();//文号的默认值

             var maxID = from d in db_plan.PlanProject
                        orderby d.ID descending
                        select d;
            List < SelectListItem > listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "公开"},
                new SelectListItem { Text = "内部", Value = "内部" },
                new SelectListItem { Text = "公开/内部", Value = "公开/内部" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text");

            List<SelectListItem> listguajie = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1" },
            };
            ViewBag.isguajie = new SelectList(listguajie, "Value", "Text", "0");


            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "青岛市规划局", Value ="1"},
                new SelectListItem { Text = "青岛市规划局市北分局", Value ="2"},
                new SelectListItem { Text = "青岛市规划局原四方分局", Value ="3"},
                new SelectListItem { Text = "青岛市规划局李沧分局", Value ="4"},
                new SelectListItem { Text = "青岛市规划局市南分局", Value ="5"},
                new SelectListItem { Text = "青岛市规划局崂山分局", Value ="6"},
            };
            ViewBag.bianzhiUnit = new SelectList(list, "Value", "Text", 1);

                //档案密级
            ViewBag.securityID = new SelectList(db_urban.SecurityClassification, "securityID", "securityName");
            //保管年限(注意这里与)
            ViewBag.retentionPeriodID = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName",2);
            //是否为内部文件

            ViewBag.shenhePerson= new SelectList(db_user.AspNetUsers, "UserName", "UserName", "张春颖");//审核人
            var UserID = User.Identity.GetUserId();//获取当前用户
            ViewBag.luruPerson = db_user.AspNetUsers.Find(UserID).UserName;//录入人传到前台，前台设置成不可修改
            //第一次录入时数据库无数据，故要进行判断
            if(maxID.Count()==0)
            {
                ViewBag.ID =0;
            }
            else
            {
               ViewBag.ID = maxID.First().ID;
            }
            //string bianzhiTimey = DateTime.Now.Year.ToString();
            //string bianzhiTimem = DateTime.Now.Month.ToString();
            //string bianzhiTimed = DateTime.Now.d
            //ViewBag.bianzhiTime = bianzhiTimey + "." + bianzhiTimem + "." + bianzhiTimed;
            ViewBag.bianzhiTime = DateTime.Now.ToString("yyyy.MM.dd");
            ViewBag.dateReceived = "2016.05.16";
            var filename = db_plan.PlanProject.Where(a => a.classifyID == classifyID);
            if (filename.Count() == 0)
            {
                ViewBag.filenameid = 0;
            }
            else
            {
                ViewBag.filenameid = filename.First().ID;
            }
            if (id2 != null && id2 != "")//将上一卷的值传递到下一卷中去，针对一卷有多盒的情况
            {
                string luruperson = db_user.AspNetUsers.Find(UserID).UserName;
                var curperson = db_plan.PlanProject.Where(d => d.luruPerson == luruperson).OrderByDescending(d=>d.ID);
                int max_ID = curperson.First().ID;
                var model = from a in db_plan.PlanProject
                            where a.ID == max_ID
                            select a;
                if(id1=="0")//针对一盒有多个工程的情况，将上一个工程的盒名传递到下一个工程中去
                {
                    ViewBag.year = model.First().yearNo;
                    int a = model.First().boxNo.IndexOf('第') + 1;
                    int b = model.First().boxNo.Trim().Length;
                    int c = model.First().boxNo.IndexOf('第');
                    ViewBag.boxid = model.First().boxNo.Substring(a,b-c-2);
                    ViewBag.boxNo = model.First().yearNo + "年" + db_plan.PlanArchiveClassify.Find(classifyID).classifyName.Trim() + "-" + db_plan.PlanArchiveClassify.Find(classifyID).classifySX.Trim() + "第" + model.First().boxNo.Substring(a, b - c - 2) + "号";
                    ViewBag.yearwen= model.First().yearNo;


                }
                else
                {
                    ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", model.First().isNeibu);
                    ViewBag.securityID = new SelectList(db_urban.SecurityClassification, "securityID", "securityName", model.First().securityID);
                    ViewBag.retentionPeriodID = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", model.First().retentionPeriodID);
                    ViewBag.yearwen = model.First().yearNo;
                    ViewBag.year = model.First().yearNo;
                   
                    //model.First().projectID = model.First().projectID + 1;
                    ViewBag.classname = model.First().fileNo.Substring(0, model.First().fileNo.IndexOf('字'));
                    if (model.First().dijijuan < model.First().gongjijuan)
                    {
                        model.First().dijijuan = model.First().dijijuan + 1;
                    }
                    return View(model.First());
                }
                ViewBag.classname = model.First().fileNo.Substring(0, model.First().fileNo.IndexOf('字'));
                
            }
           
           

            return View();
        }

        // POST: PlanProjects/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "securityID,retentionPeriodID,gongjijuan,dijijuan,projectname,boxNo,developmentUnit,projectContent,projectLocation,yearNo,,classifyID,projectID,fileNo,dateReceived,remarks,bianzhiUnit,luruPerson,juanneiSeqNo,bianzhiTime,pageNo,photoCnt,archiveID,shenhePerson,isImageExist,isNeibu,projectContent_neibu,archiveTitle,archiveTitle_neibu,pageNo_neibu,developmentUnit_neibu,projectLocation_neibu,photoCnt_neibu,wenziCnt,wenziCnt_neibu")] PlanProject planProject, String action,int ID, string DELETE_ID)
        {
            
            if (action == "打印备考表(公开)")
            {
                return RedirectToAction("beikaobiao", new { id = ID+1, id1 = 0 });
            }

            if (action == "打印工程封皮(公开)")
            {
                return RedirectToAction("GuiHuaGongChengFengPi", new { id = ID + 1, id1 = 0 });
            }
            if (action == "打印备考表(内部)")
            {
                return RedirectToAction("beikaobiao", new { id = ID + 1, id1 = 1 });
            }

            if (action == "打印工程封皮(内部)")
            {
                return RedirectToAction("GuiHuaGongChengFengPi", new { id = ID + 1, id1 = 1 });
            }

            //if (action == "删除词条")
            //{
            //    if (DELETE_ID != "")
            //    {
            //        int deleteID = int.Parse(DELETE_ID);
            //        PlanProjectCT planProjectCT = db_plan.PlanProjectCT.Where(a => a.ID == deleteID).First();
            //        db_plan.PlanProjectCT.Remove(planProjectCT);
            //        db_plan.SaveChanges();
            //        return Content("<script >alert('删除成功!');window.location.href='/PlanProjects/Create/?classifyID=" + planProject.classifyID + "';</script >");
            //    }
            //    else {
            //        return Content("<script >alert('没有选中词条');window.history.back();</script>");
            //    }
                
            //}
            if(action == "删除该工程")
            {
                planProject.ID = ID;
                db_plan.Entry(planProject).State = EntityState.Modified;
                try
                {
                    db_plan.PlanProject.Remove(planProject);
                    db_plan.SaveChanges();
                    return Content("<script >alert('删除成功!');window.history.back();</script >");
                  
                }
                catch (Exception ex)
                {
                    return Content("<script >alert('信息有误！');window.history.back();</script >");
                  
                }
            }
           if (action == "添加下一工程")
            {
                if (planProject.gongjijuan>1)
                {
                    if (planProject.dijijuan == planProject.gongjijuan)
                    {
                        return RedirectToAction("Create", new { classifyID = planProject.classifyID, id1 = 0, id2 = ID });
                    }
                    else
                    {
                        return RedirectToAction("Create", new { classifyID = planProject.classifyID,id2=ID});
                    }
                }
                return RedirectToAction("Create", new { classifyID = planProject.classifyID,id1=0,id2=ID/*projectname=planProject.projectname, projectLocation =planProject.projectLocation, developmentUnit=planProject.developmentUnit, projectContent=planProject.projectContent, projectContent_neibu=planProject.projectContent_neibu, archiveTitle=planProject.archiveTitle, archiveTitle_neibu=planProject.archiveTitle_neibu*/});//将上一个工程的盒号传递到下一个工程中去
            }
            if (action == "修改")
            {
                planProject.ID = ID;
                db_plan.Entry(planProject).State = EntityState.Modified;
                try
                {
                    db_plan.SaveChanges();
                    return Content("<script >alert('修改成功');window.history.back();</script >");
                }
                catch (Exception ex)
                {
                    return Content("<script >alert('修改信息有误，请检查！');window.history.back();</script >");
                 
                }
            }
                
                return View(planProject);
        }
        public string Baocunok(string id, string securityID,string retentionPeriodID, string gongjijuan, string dijijuan, string projectname, string boxNo, string developmentUnit, string projectContent, string projectLocation, string yearNo, string classifyID, string projectID, string fileNo, string dateReceived, string remarks, string bianzhiUnit, string luruPerson, string juanneiSeqNo, string bianzhiTime, string pageNo, string photoCnt, string archiveID, string shenhePerson, string isImageExist, string isNeibu, string projectContent_neibu, string archiveTitle, string archiveTitle_neibu, string pageNo_neibu, string developmentUnit_neibu, string projectLocation_neibu, string photoCnt_neibu, string wenziCnt, string wenziCnt_neibu, string name,string  totalSeqNo,string seqNo2, string coordinate,string isguajie)
        {


            if (name == "baocun")
            {
                if (id != "" && id != null)//防止重复提交
                {
                    int Total = Int32.Parse(gongjijuan);
                    int vol = Int32.Parse(dijijuan);
                    var a = from c in db_plan.PlanProject
                            where c.fileNo == fileNo && c.gongjijuan == Total && c.dijijuan == vol
                            select c;
                    if (a.Count() != 0)
                    {
                        return "7";
                    }
                }
                PlanProject project = new PlanProject();
                project.isguajie = int.Parse(isguajie);
                project.status = "LR";
                project.isImageExist = "无";
                project.totalSeqNo = "";
                project.securityID = securityID;
                project.retentionPeriodID = retentionPeriodID;
                project.gongjijuan = Int32.Parse(gongjijuan.Trim());
                project.dijijuan = Int32.Parse(dijijuan.Trim());
                project.coordinate = coordinate;
                if (boxNo == null || boxNo == "")
                {
                    project.boxNo = "";
                }
                else

                {
                    project.boxNo = boxNo.Trim();
                }
                if (projectname == null || projectname == "")
                {
                    project.projectname = "";
                }
                else

                {
                    project.projectname = projectname.Trim();
                }

                if (fileNo == null || fileNo == "")
                {
                    project.fileNo = "";
                }
                else

                {
                    project.fileNo = fileNo.Trim();
                }
                //为了区分不同区的同一年份同一类别的档案，保存urban_type字段
                string type = fileNo.Trim().Substring(0, 3);//大部分都是3个字符，只有市规划是两个字符“青规”，胶南是四个字符“青规胶南”
                if (fileNo.Trim().Substring(0, 4) == "青规胶南")//先判断胶南
                {
                    project.urban_type = 10;
                }
                else if (type != "青规北" && type != "青规四" && type != "青规李" && type != "青规南" && type != "青规崂" && type != "青规城" && type != "青规黄" && type != "青规胶" && type != "青规平" && type != "青规莱" && type != "青规即" && type != "青规开")//只能是市规划了
                {
                    project.urban_type = 1;
                }
                else
                {


                    switch (type)
                    {
                        case "青规北":
                            project.urban_type = 2;
                            break;
                        case "青规四":
                            project.urban_type = 3;
                            break;
                        case "青规李":
                            project.urban_type = 4;
                            break;
                        case "青规南":
                            project.urban_type = 5;
                            break;
                        case "青规崂":
                            project.urban_type = 6;
                            break;
                        case "青规城":
                            project.urban_type = 7;
                            break;
                        case "青规黄":
                            project.urban_type = 8;
                            break;
                        case "青规胶":
                            project.urban_type = 9;
                            break;
                        case "青规平":
                            project.urban_type = 11;
                            break;
                        case "青规莱":
                            project.urban_type = 12;
                            break;
                        case "青规即":
                            project.urban_type = 13;
                            break;
                        case "青规开":
                            project.urban_type = 14;
                            break;

                    }

                }
                if (project.isguajie == 1) {
                    project.urban_type = 6;
                }

                project.developmentUnit = developmentUnit;
                project.projectContent = projectContent;
                project.projectLocation = projectLocation;
                project.yearNo = yearNo;
              
                if (classifyID==null&&classifyID=="")
                {
                    return "3";//工程种类不能为空
                }
                project.classifyID = Int32.Parse(classifyID);
                if(projectID!=null&&projectID!="")
                {
                    if(projectID.Trim().IndexOf('-')!=-1)
                    {
                        projectID = projectID.Split('-')[0].ToString();
                    }
                    if (projectID.Trim().IndexOf('X') != -1)
                    {
                        projectID = projectID.Split('X')[1].ToString();//针对020020100X001这种情况
                    }
                    project.projectID = Int32.Parse(projectID.Trim());
                } 
                else
                {
                    project.projectID = 0; 
                }
               
                if (dateReceived != null && dateReceived != "")
                {
                    project.dateReceived = DateTime.Parse(dateReceived.Trim());
                }
                else
                {
                    project.dateReceived =DateTime.Today;
                }
                project.remarks = remarks;
                project.bianzhiUnit = bianzhiUnit;
                project.luruPerson = luruPerson;
                if(juanneiSeqNo!=null&&juanneiSeqNo!="")
                {
                    project.juanneiSeqNo = Int32.Parse(juanneiSeqNo.Trim());
                }
                else
                {
                    project.juanneiSeqNo = 0;
                }
                project.bianzhiTime = bianzhiTime;
                project.pageNo = pageNo;
                project.archiveID = 0;
                project.shenhePerson = shenhePerson;
              
                project.isNeibu = isNeibu;
                project.projectContent_neibu = projectContent_neibu;
                project.archiveTitle = archiveTitle;
                project.archiveTitle_neibu = archiveTitle_neibu;
                project.pageNo_neibu = pageNo_neibu;
                project.developmentUnit_neibu = developmentUnit_neibu;
                project.projectLocation_neibu = projectLocation_neibu;
                if(photoCnt!=null&&photoCnt!="")
                {
                    project.photoCnt = Int32.Parse(photoCnt.Trim());
                }
                else
                {
                    project.photoCnt = 0;
                }
                if (photoCnt_neibu != null && photoCnt_neibu != "")
                {
                    project.photoCnt_neibu = Int32.Parse(photoCnt_neibu);
                }
                else
                {
                    project.photoCnt_neibu = 0;
                }

                if(wenziCnt!=null&&wenziCnt!="")
                {
                    project.wenziCnt = Int32.Parse(wenziCnt.Trim());
                }
                else
                {
                    project.wenziCnt = 0;
                }
                if(wenziCnt_neibu!=null&&wenziCnt_neibu!="")
                {
                    project.wenziCnt_neibu = Int32.Parse(wenziCnt_neibu.Trim());
                }
                else
                {
                    project.wenziCnt_neibu = 0;
                }
                try
                {
                    db_plan.PlanProject.Add(project);
                    db_plan.SaveChanges();
                    return "4";//保存成功
                }
                catch (Exception ex)
                {
                    return "5";//数据错误，保存失败
                }

            }

            if(name=="xiugai")
            {
                 int ID1 = 0;
                if (id != null && id != "")
                {
                    ID1 = Int32.Parse(id);
                }
              
                var project = from a in db_plan.PlanProject
                              where a.ID ==ID1
                              select a;
                project.First().securityID = securityID;
                project.First().retentionPeriodID = retentionPeriodID;
                project.First().isguajie = int.Parse(isguajie);
                if (totalSeqNo!=""&& totalSeqNo!=null)
                {
                    project.First().totalSeqNo = totalSeqNo.Trim();
                }
                if(seqNo2!=""&&seqNo2!=null)
                {
                    project.First().seqNo1 = Int32.Parse(seqNo2.Trim());
                }
                project.First().dijijuan = Int32.Parse(dijijuan.Trim());
                project.First().boxNo = boxNo;
                project.First().projectname = projectname;
                project.First().developmentUnit = developmentUnit;
                project.First().projectContent = projectContent;
                project.First().projectLocation = projectLocation;
                project.First().yearNo = yearNo;
                if (classifyID == null && classifyID == "")
                {
                    return "3";//工程种类不能为空
                }
                project.First().classifyID = Int32.Parse(classifyID);
                if (projectID != null && projectID != "")
                {
                    if (projectID.Trim().IndexOf('-') != -1)
                    {
                        projectID = projectID.Split('-')[0].ToString();
                    }
                    project.First().projectID = Int32.Parse(projectID.Trim());
                }
                else
                {
                    project.First().projectID = 0;
                }
                project.First().fileNo = fileNo;
                //为了区分不同区的同一年份同一类别的档案，保存urban_type字段
                string type = fileNo.Trim().Substring(0, 3);//大部分都是3个字符，只有市规划是两个字符“青规”，胶南是四个字符“青规胶南”
                if (fileNo.Trim().Substring(0, 4) == "青规胶南")//先判断胶南
                {
                    project.First().urban_type = 10;
                }
                if (type != "青规北" && type != "青规四" && type != "青规李" && type != "青规南" && type != "青规崂" && type != "青规城" && type != "青规黄" && type != "青规胶" && type != "青规平" && type != "青规莱" && type != "青规即" && type != "青规开")//只能是市规划了
                {
                    project.First().urban_type = 1;
                }
                switch (type)
                {
                    case "青规北":
                        project.First().urban_type = 2;
                        break;
                    case "青规四":
                        project.First().urban_type = 3;
                        break;
                    case "青规李":
                        project.First().urban_type = 4;
                        break;
                    case "青规南":
                        project.First().urban_type = 5;
                        break;
                    case "青规崂":
                        project.First().urban_type = 6;
                        break;
                    case "青规城":
                        project.First().urban_type = 7;
                        break;
                    case "青规黄":
                        project.First().urban_type = 8;
                        break;
                    case "青规胶":
                        project.First().urban_type = 9;
                        break;
                    case "青规平":
                        project.First().urban_type = 11;
                        break;
                    case "青规莱":
                        project.First().urban_type = 12;
                        break;
                    case "青规即":
                        project.First().urban_type = 13;
                        break;
                    case "青规开":
                        project.First().urban_type = 14;
                        break;

                }
                if (project.First().isguajie == 1)
                {
                    project.First().urban_type = 6;
                }
                if (dateReceived != null&&dateReceived!="")
                {
                    project.First().dateReceived = DateTime.Parse(dateReceived.Trim());
                    
                }
                else {
                    project.First().dateReceived = null;
                }
                project.First().remarks = remarks;
                project.First().bianzhiUnit = bianzhiUnit;
                project.First().luruPerson = luruPerson;
                if (juanneiSeqNo != null && juanneiSeqNo != "")
                {
                    project.First().juanneiSeqNo = Int32.Parse(juanneiSeqNo.Trim());
                }
                else
                {
                    project.First().juanneiSeqNo = 0;
                }
                project.First().bianzhiTime = bianzhiTime;

                project.First().pageNo = pageNo;
                project.First().archiveID = 0;
                project.First().shenhePerson = shenhePerson;
                project.First().isImageExist = isImageExist;
                project.First().isNeibu = isNeibu;
                project.First().projectContent_neibu = projectContent_neibu;
                project.First().archiveTitle = archiveTitle;
                project.First().archiveTitle_neibu = archiveTitle_neibu;
                project.First().pageNo_neibu = pageNo_neibu;
                project.First().developmentUnit_neibu = developmentUnit_neibu;
                project.First().projectLocation_neibu = projectLocation_neibu;
                if (photoCnt != null && photoCnt != "")
                {
                    project.First().photoCnt = Int32.Parse(photoCnt.Trim());
                }
                else
                {
                    project.First().photoCnt = 0;
                }
                if (wenziCnt != null && wenziCnt != "")
                {
                    project.First().wenziCnt = Int32.Parse(wenziCnt.Trim());
                }
                else
                {
                    project.First().wenziCnt = 0;
                }
                if (wenziCnt_neibu != null && wenziCnt_neibu != "")
                {
                    project.First().wenziCnt_neibu = Int32.Parse(wenziCnt_neibu.Trim());
                }
                else
                {
                    project.First().wenziCnt_neibu = 0;
                }
                project.First().gongjijuan = Int32.Parse(gongjijuan.Trim());
                project.First().dijijuan = Int32.Parse(dijijuan.Trim());
                project.First().coordinate = coordinate;
                try
                {
                    db_plan.Entry(project.First()).State = EntityState.Modified;
                    db_plan.SaveChanges();
                    return "6";//修改成功
                }
                catch (Exception ex)
                {
                    return "5";//数据错误，保存失败
                }

            }
            return "0";
        }
        // GET: PlanProjects/Edit/5
        public ActionResult Edit(int? id)
        {

            PlanProject planProject = db_plan.PlanProject.Find(id);
            ViewBag.ID = id;
            ViewBag.classifyID = planProject.classifyID;//案卷类型ID
            ViewBag.classifyName = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName.Trim();//案卷类型mame
            ViewBag.classiftsx = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifySX;//案卷类型缩写，为了拼接盒号
            ViewBag.box = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName.Trim() + "-" + db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifySX.Trim();
            if (planProject.classifyID == 28 || planProject.classifyID == 29)
            {
                //string classname = planProject.fileNo.Substring(0, planProject.fileNo.IndexOf('】') + 1);
                ViewBag.classname = planProject.fileNo.Substring(0, planProject.fileNo.LastIndexOf('字'));
            }
            else {
                ViewBag.classname = planProject.fileNo.Substring(0,planProject.fileNo.IndexOf('字'));/* db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName.Trim()*/;//文号的默认值
            }
            ViewBag.year = planProject.yearNo;
            int a = planProject.boxNo.IndexOf('第') + 1;
            int b = planProject.boxNo.Trim().Length;
            int c = planProject.boxNo.IndexOf('第');
            ViewBag.boxid = planProject.boxNo.Substring(a, b - c - 2);
            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "市规划", Value = "1" },
                new SelectListItem { Text = "市北规划", Value = "2" },
                new SelectListItem { Text = "原四方规划", Value = "3" },
                new SelectListItem { Text = "李沧规划", Value = "4" },
                new SelectListItem { Text = "市南规划", Value = "5" },
                new SelectListItem { Text = "崂山规划", Value = "6" },
                new SelectListItem { Text = "城阳规划", Value = "7" },
                new SelectListItem { Text = "黄岛规划", Value = "8" },
                new SelectListItem { Text = "胶州规划", Value = "9" },
                new SelectListItem { Text = "胶南规划", Value = "10" },
                new SelectListItem { Text = "平度规划", Value = "11" },
                new SelectListItem { Text = "莱西规划", Value = "12" },
                new SelectListItem { Text = "即墨规划", Value = "13" },
                new SelectListItem { Text = "开发区规划", Value = "14" },
            };
            //ViewBag.quyu = new SelectList(list2, "Value", "Text", planProject.urban_type);
            if (planProject.isguajie == 0 || planProject.isguajie == null)
            {
                ViewBag.quyu = new SelectList(list2, "Value", "Text", planProject.urban_type);
            }
            else
            {
                ViewBag.quyu = new SelectList(list2, "Value", "Text", "1");
            }
            List < SelectListItem > listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "公开"},
                new SelectListItem { Text = "内部", Value = "内部" },
                new SelectListItem { Text = "公开/内部", Value = "公开/内部" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", planProject.isNeibu.Trim());

            List<SelectListItem> listguajie = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1" },
            };

            string isgua = planProject.isguajie.ToString();
            ViewBag.isguajie = new SelectList(listguajie, "Value", "Text", isgua);

            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "青岛市规划局", Value ="青岛市规划局"},
                new SelectListItem { Text = "青岛市规划局市北分局", Value ="青岛市规划局市北分局"},
                new SelectListItem { Text = "青岛市规划局原四方分局", Value ="青岛市规划局原四方分局"},
                new SelectListItem { Text = "青岛市规划局李沧分局", Value ="青岛市规划局李沧分局"},
                new SelectListItem { Text = "青岛市规划局市南分局", Value ="青岛市规划局市南分局"},
                new SelectListItem { Text = "青岛市规划局崂山分局", Value ="青岛市规划局市南分局"},
            };
            ViewBag.bianzhiUnit = new SelectList(list, "Value", "Text", planProject.bianzhiUnit.Trim());
            //档案密级
            ViewBag.securityID = new SelectList(db_urban.SecurityClassification, "securityID", "securityName", planProject.securityID.Trim());
            //保管年限(注意这里与)
            ViewBag.retentionPeriodID = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", planProject.retentionPeriodID.Trim());
            
            ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", planProject.shenhePerson);//审核人
            ViewBag.luruPerson = planProject.luruPerson;//录入人传到前台，前台设置成不可修改
            //ViewBag.dateReceived = planProject.dateReceived == null ? "" : planProject.dateReceived.Value.ToString("yyyy.MM.dd");
            ViewBag.dateReceived = planProject.dateReceived.Value.ToString("yyyy.MM.dd");

            if (planProject.totalSeqNo != "" && planProject.totalSeqNo != null)
            {


                if (planProject.isNeibu.Trim() == "公开")
                {

                    ViewData["fengpiG"] = "inline-block";
                    ViewData["beikaobiaoG"] = "inline-block";
                    ViewData["fengpiN"] = "display: none";
                    ViewData["beikaobiaoN"] = "display: none";
                }
                if (planProject.isNeibu.Trim() == "内部")
                {
                    ViewData["fengpiG"] = "display: none";
                    ViewData["beikaobiaoG"] = "display: none";
                    ViewData["fengpiN"] = "inline-block";
                    ViewData["beikaobiaoN"] = "inline-block";
                }
                if (planProject.isNeibu.Trim() == "公开/内部")
                {
                    ViewData["fengpiG"] = "inline-block";
                    ViewData["beikaobiaoG"] = "inline-block";
                    ViewData["fengpiN"] = "inline-block";
                    ViewData["beikaobiaoN"] = "inline-block";
                }
            }
            else
            {
                ViewData["fengpiG"] = "display: none";
                ViewData["beikaobiaoG"] = "display: none";
                ViewData["fengpiN"] = "display: none";
                ViewData["beikaobiaoN"] = "display: none";
                ViewData["ProjectContent"]= "display: none";
            }
            return View(planProject);
        }

        // POST: PlanProjects/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "securityID,retentionPeriodID,gongjijuan,dijijuan,projectname,ID,isNeibu,seqNo,developmentUnit,projectContent,projectLocation,yearNo,pageCnt,classifyID,projectID,fileNo,dateReceived,boxNo,status,remarks,archiveBoxCnt,mapScale,storeLocation,bianzhiUnit,fileNoTou,luruPerson,seqNo1,juanneiSeqNo,bianzhiTime,pageNo,totalSeqNo,archiveID,photoCnt,shenhePerson,isImageExist")] PlanProject planProject,String action)
        {
            ViewBag.classifyID = planProject.classifyID;//案卷类型ID
            ViewBag.classifyName = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName;
            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "市规划", Value = "1" },
                new SelectListItem { Text = "市北规划", Value = "2" },
                new SelectListItem { Text = "原四方规划", Value = "3" },
                new SelectListItem { Text = "李沧规划", Value = "4" },
                new SelectListItem { Text = "市南规划", Value = "5" },
                new SelectListItem { Text = "崂山规划", Value = "6" },
                new SelectListItem { Text = "城阳规划", Value = "7" },
                new SelectListItem { Text = "黄岛规划", Value = "8" },
                new SelectListItem { Text = "胶州规划", Value = "9" },
                new SelectListItem { Text = "胶南规划", Value = "10" },
                new SelectListItem { Text = "平度规划", Value = "11" },
                new SelectListItem { Text = "莱西规划", Value = "12" },
                new SelectListItem { Text = "即墨规划", Value = "13" },
                new SelectListItem { Text = "开发区规划", Value = "14" },
            };
            if (planProject.isguajie == 0 || planProject.isguajie == null) {
                ViewBag.quyu = new SelectList(list2, "Value", "Text", planProject.urban_type);
            }
            else {
                ViewBag.quyu = new SelectList(list2, "Value", "Text", "1");
            }
            
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "公开"},
                new SelectListItem { Text = "内部", Value = "内部" },
                new SelectListItem { Text = "公开/内部", Value = "公开/内部" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", planProject.isNeibu);
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "青岛市规划局", Value ="1"},
                new SelectListItem { Text = "青岛市规划局市北分局", Value ="2"},
                new SelectListItem { Text = "青岛市规划局原四方分局", Value ="3"},
                new SelectListItem { Text = "青岛市规划局李沧分局", Value ="4"},
                new SelectListItem { Text = "青岛市规划局市南分局", Value ="5"},
                new SelectListItem { Text = "青岛市规划局崂山分局", Value ="6"},
            };
            ViewBag.bianzhiUnit = new SelectList(list, "Value", "Text", planProject.bianzhiUnit.Trim());
            //档案密级
            ViewBag.securityID = new SelectList(db_urban.SecurityClassification, "securityID", "securityName", planProject.securityID);
            //保管年限(注意这里与)
            ViewBag.retentionPeriodID = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", planProject.retentionPeriodID);

            ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", planProject.shenhePerson);//审核人
            ViewBag.luruPerson = planProject.luruPerson;//录入人传到前台，前台设置成不可修改

            if (action == "提交保存")
            {
                if (ModelState.IsValid)
                {

                    db_plan.Entry(planProject).State = EntityState.Modified;
                    //return RedirectToAction("Index", new { archiveID = planProject.archiveID, classifyID = planProject.classifyID });
                    //return Content("<script >alert('修改成功！');window.location.href='/PlanProjects/Index?archiveID=" + planProject.archiveID + "&id="+ planProject.ID + "&id1=2" + "&classifyID=" + planProject.classifyID + "';</script >");
                    try
                    {
                        db_plan.SaveChanges();
                        Response.Write("<script>alert('保存成功!');</script>");
                        return View(planProject);
                    }
                    catch (Exception ex)
                    {
                        return Content("<script >alert('录入信息有误，请检查！');window.history.back();</script >");
                        //return Content("<script >alert('档号不能为空！');window.history.back();</script >");
                    }
                }
            }
            if(action == "返回")
            {
                string status = planProject.status.Trim();
                if (status == "GD")
                {
                    return RedirectToAction("Index_GD", new { classifyID = planProject.classifyID });
                }
                else if (status == "BH")
                {
                    return RedirectToAction("Index_BH", new { classifyID = planProject.classifyID });
                }
                else if (status == "RK")
                {
                    return RedirectToAction("Index_RK", new { classifyID = planProject.classifyID });
                }
                else
                {
                    return RedirectToAction("Index_LR", new { classifyID = planProject.classifyID });
                }

            }
            int ID = planProject.ID;
            if (action == "打印备考表(公开)")
            {
                return RedirectToAction("beikaobiao", new { id = ID, id1 = 0 });
            }

            if (action == "打印案卷封皮(公开)")
            {
                return RedirectToAction("GuiHuaGongChengFengPi", new { id = ID, id1 = 0 });
            }
            if (action == "打印备考表(内部)")
            {
                return RedirectToAction("beikaobiao", new { id = ID, id1 = 1 });
            }

            if (action == "打印案卷封皮(内部)")
            {
                return RedirectToAction("GuiHuaGongChengFengPi", new { id = ID, id1 = 1 });
            }
            if (action == "打印工程目录")
            {
                return RedirectToAction("GuiHuaGongChengMuLu", new { id = planProject.seqNo1});
            }
            return View(planProject);
        }

        public ActionResult EditAllPlan(int? id)
        {
            PlanProject planProject = db_plan.PlanProject.Find(id);
            ViewBag.ID = id;
            ViewBag.classifyID = planProject.classifyID;//案卷类型ID
            ViewBag.classifyName = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName.Trim();//案卷类型mame
            ViewBag.classiftsx = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifySX;//案卷类型缩写，为了拼接盒号
            ViewBag.box = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName.Trim() + "-" + db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifySX.Trim();
            if (planProject.classifyID == 28 || planProject.classifyID == 29)
            {
                ViewBag.classname = planProject.fileNo.Substring(0, planProject.fileNo.IndexOf('】') + 1);
            }
            else
            {
                ViewBag.classname = planProject.fileNo.Substring(0, planProject.fileNo.IndexOf('字'));
            }
            //ViewBag.classname = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName.Trim();//文号的默认值
            ViewBag.year = planProject.yearNo;
            int a = planProject.boxNo.IndexOf('第') + 1;
            int b = planProject.boxNo.Trim().Length;
            int c = planProject.boxNo.IndexOf('第');
            ViewBag.boxid = planProject.boxNo.Substring(a, b - c - 2);
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "公开"},
                new SelectListItem { Text = "内部", Value = "内部" },
                new SelectListItem { Text = "公开/内部", Value = "公开/内部" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", planProject.isNeibu.Trim());
            //档案密级
            if (planProject.securityID == null) {
                planProject.securityID = "";
            }
            ViewBag.securityID = new SelectList(db_urban.SecurityClassification, "securityID", "securityName", planProject.securityID.Trim());
            //保管年限(注意这里与)
            if (planProject.retentionPeriodID == null) {
                planProject.retentionPeriodID = "";
            }
            ViewBag.retentionPeriodID = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", planProject.retentionPeriodID.Trim());

            ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", planProject.shenhePerson);//审核人
            ViewBag.luruPerson = planProject.luruPerson;//录入人传到前台，前台设置成不可修改
            if (planProject.totalSeqNo != "" && planProject.totalSeqNo != null)
            {


                if (planProject.isNeibu.Trim() == "公开")
                {

                    ViewData["fengpiG"] = "inline-block";
                    ViewData["beikaobiaoG"] = "inline-block";
                    ViewData["fengpiN"] = "display: none";
                    ViewData["beikaobiaoN"] = "display: none";
                }
                if (planProject.isNeibu.Trim() == "内部")
                {
                    ViewData["fengpiG"] = "display: none";
                    ViewData["beikaobiaoG"] = "display: none";
                    ViewData["fengpiN"] = "inline-block";
                    ViewData["beikaobiaoN"] = "inline-block";
                }
                if (planProject.isNeibu.Trim() == "公开/内部")
                {
                    ViewData["fengpiG"] = "inline-block";
                    ViewData["beikaobiaoG"] = "inline-block";
                    ViewData["fengpiN"] = "inline-block";
                    ViewData["beikaobiaoN"] = "inline-block";
                }
            }
            else
            {
                ViewData["fengpiG"] = "display: none";
                ViewData["beikaobiaoG"] = "display: none";
                ViewData["fengpiN"] = "display: none";
                ViewData["beikaobiaoN"] = "display: none";
                ViewData["ProjectContent"] = "display: none";
            }
            return View(planProject);
        }
       
        public ActionResult Delete(int id)
        {
            PlanProject planProject = db_plan.PlanProject.Find(id);
            string status = planProject.status.Trim();
          
            try
            {
                //var box = from a in db_plan.PlanArchiveBox
                //          where a.boxNo == planProject.boxNo
                //          select a;
                //if (box.Count() != 0)
                //{
                //    PlanArchiveBox planArchiveBox = box.First();
                //    db_plan.PlanArchiveBox.Remove(planArchiveBox);
                //}
                var box1 = from a in db_plan.PlanProject
                           where a.boxNo == planProject.boxNo
                           where a.fileNo != planProject.fileNo
                           select a;

                var box = from a in db_plan.PlanArchiveBox
                          where a.boxNo == planProject.boxNo
                          select a;

                if (box.Count() != 0)
                {
                    PlanArchiveBox planArchiveBox = box.First();
                    if (box1.Count() != 0)
                    {
                        planArchiveBox.archiveBoxCnt--;
                        planArchiveBox.textMatirial -= (planProject.wenziCnt + planProject.wenziCnt_neibu);
                        planArchiveBox.drawing -= (planProject.photoCnt + planProject.photoCnt_neibu);
                    }
                    db_plan.PlanArchiveBox.Remove(planArchiveBox);
                }

                db_plan.PlanProject.Remove(planProject);
                db_plan.SaveChanges();

                //return Content("<script >alert('删除成功！');</script >");
                if (status == "GD")
                {
                    return RedirectToAction("Index_GD", new { classifyID = planProject.classifyID });
                }
                else if (status == "BH")
                {
                    return RedirectToAction("Index_BH", new { classifyID = planProject.classifyID });
                }
                else if (status == "RK")
                {
                    return RedirectToAction("Index_RK", new { classifyID = planProject.classifyID });
                }
                else
                {
                    return RedirectToAction("Index_LR", new { classifyID = planProject.classifyID });
                }
            }
            catch (Exception ex)
            {
                return Content("<script >alert('信息有误！');window.history.back();</script >");
                //return Content("<script >alert('档号不能为空！');window.history.back();</script >");
            }          
        }
        public ActionResult DeleteAllPlan(int id)
        {
            PlanProject planProject = db_plan.PlanProject.Find(id);

            var box1 = from a in db_plan.PlanProject
                       where a.boxNo == planProject.boxNo
                       where a.fileNo != planProject.fileNo
                       select a;

            var box = from a in db_plan.PlanArchiveBox
                      where a.boxNo == planProject.boxNo
                      select a;

            if (box.Count() != 0)
            {
                PlanArchiveBox planArchiveBox = box.First();
                if (box1.Count() != 0)
                {
                    planArchiveBox.archiveBoxCnt --;
                    planArchiveBox.textMatirial -= (planProject.wenziCnt + planProject.wenziCnt_neibu);
                    planArchiveBox.drawing -= (planProject.photoCnt + planProject.photoCnt_neibu);
                }
                db_plan.PlanArchiveBox.Remove(planArchiveBox);
            }


            db_plan.PlanProject.Remove(planProject);
            db_plan.SaveChanges();
            return RedirectToAction("AllPlanProject", "PlanProjects");
           
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db_plan.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        public JsonResult title()//jsy动态框方法
        {
            var list = from bb in db_plan.PlanProjectCT.ToList()
                           //where bb.classifyID == classifyID
                       //orderby bb.newid
                       select new
                       {
                           ID = bb.ID,
                           name = bb.projectContent,
                           //newId = bb.newid
                       };
            list = list.OrderByDescending(a => a.ID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public void tianjia() {
            PlanProjectCT planProjectCT = new PlanProjectCT();
            int max_id = db_plan.PlanProjectCT.Max(d => d.ID);
            planProjectCT.ID = max_id + 1;
            planProjectCT.projectContent = Request.Form["choose1"];
            planProjectCT.classifyID = 1;
            db_plan.PlanProjectCT.Add(planProjectCT);
            db_plan.SaveChanges();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                rptH.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}


