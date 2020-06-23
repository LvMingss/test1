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
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Reporting.WebForms;
using System.Data.OleDb;
using CrystalDecisions.Web;
using CrystalDecisions.CrystalReports.Engine;

namespace urban_archive.Controllers
{
    public class RoadController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities db_user = new UrbanUsersEntities();
        ReportDocument rptH = new ReportDocument();
        public ActionResult Home()
        {
            return View();
        }
        // GET: Road
        public ActionResult Index(string type,string searchString)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "申请单位", Value = "0" },
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "最新工程地点", Value = "2"},
                new SelectListItem { Text = "执照号", Value = "3"},
                new SelectListItem { Text = "工程内容", Value = "4" },
                new SelectListItem { Text = "年度", Value = "5"},
                new SelectListItem { Text = "总登记号", Value = "6"},
                new SelectListItem { Text = "区工程顺序号", Value = "7"}
            };
            if (type == null | type == "")
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", 0);
            }
            else
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", type);
            }
            ViewBag.CurrentFilter = searchString;
            return View();
            //    var road = db.OtherArchives.Where(j => j.classTypeID == 2).OrderBy(j => j.volNo);
            //    if (action == "查找") {
            //        string n = Request.Form["Selected"];
            //        string m = Request.Form["search"];
            //        if (n == "0")
            //        {
            //            var chazhao = road.Where(a => a.applyUnit.Contains(m)).OrderBy(a => a.volNo);
            //            ViewBag.result = JsonConvert.SerializeObject(chazhao);
            //            return View();
            //        }
            //        if (n == "1")
            //        {
            //            var chazhao = road.Where(a => a.volNo.Contains(m)).OrderBy(a => a.volNo);
            //            ViewBag.result = JsonConvert.SerializeObject(chazhao);
            //            return View();
            //        }
            //        if (n == "2")
            //        {
            //            var chazhao = road.Where(a => a.location.Contains(m)).OrderBy(a => a.volNo);
            //            ViewBag.result = JsonConvert.SerializeObject(chazhao);
            //            return View();
            //        }
            //        if (n == "3")
            //        {
            //            var chazhao = road.Where(a => a.year.Contains(m)).OrderBy(a => a.volNo);
            //            ViewBag.result = JsonConvert.SerializeObject(chazhao);
            //            return View();
            //        }
            //        if (n == "4")
            //        {
            //            var chazhao = road.Where(a => a.projectRange.Contains(m)).OrderBy(a => a.volNo);
            //            ViewBag.result = JsonConvert.SerializeObject(chazhao);
            //            return View();
            //        }
            //        if (n == "5")
            //        {
            //            var chazhao = road.Where(a => a.doorplate.Contains(m)).OrderBy(a => a.volNo);
            //            ViewBag.result = JsonConvert.SerializeObject(chazhao);
            //            return View();
            //        }
            //        if (n == "6")
            //        {
            //            var chazhao = road.Where(a => a.landNo.Contains(m)).OrderBy(a => a.volNo);
            //            ViewBag.result = JsonConvert.SerializeObject(chazhao);
            //            return View();
            //        }
            //        if (n == "7")
            //        {
            //            var chazhao = road.Where(a => a.registrationNo.Contains(m)).OrderBy(a => a.volNo);
            //            ViewBag.result = JsonConvert.SerializeObject(chazhao);
            //            return View();
            //        }
            //        if (n == "8")
            //        {
            //            var chazhao = road.Where(a => a.paijiaNo.Contains(m)).OrderBy(a => a.volNo);
            //            ViewBag.result = JsonConvert.SerializeObject(chazhao);
            //            return View();
            //        }
            //        if (n == "9")
            //        {
            //            var chazhao = road.Where(a => a.newLocation.Contains(m)).OrderBy(a => a.volNo);
            //            ViewBag.result = JsonConvert.SerializeObject(chazhao);
            //            return View();
            //        }
            //    }
            //    ViewBag.result = JsonConvert.SerializeObject(road);
            //    return View();
        }
        public string IndexData(string searchString, int? SelectedID, int? page)
        {

            var road = db.OtherArchives.Where(j => j.classTypeID == 2);

            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        road = road.Where(ad => ad.applyUnit.Contains(searchString));//根据申请单位搜索
                        break;
                    case 1:
                        road = road.Where(ad => ad.location.Contains(searchString));//根据工程地点搜索
                        break;
                    case 2:
                        road = road.Where(ad => ad.newLocation.Contains(searchString));//根据最新工程地点搜索
                        break;
                    case 3:
                        road = road.Where(ad => ad.licenceNo.Contains(searchString));//根据执照号搜索
                        break;
                    case 4:
                        road = road.Where(ad => ad.projectRange.Contains(searchString));//根据工程内容搜索
                        break;
                    case 5:
                        road = road.Where(ad => ad.year.Contains(searchString));//根据年度搜索
                        break;
                    case 6:
                        road = road.Where(ad => ad.registrationNo.Contains(searchString));//根据总登记号搜索
                        break;
                    case 7:
                        road = road.Where(ad => ad.areaProSeqNo.Contains(searchString));//根据区工程序号搜索
                        break;
                }
            }
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            int cnt = road.Count() / pageSize + 1;
            if (road.Count() % pageSize == 0)
            {
                cnt = road.Count() / pageSize;
            }
            road = road.OrderBy(s => s.licenceNo);
            //road = road.OrderBy(s => s.volNo).ThenBy(s => s.year);
            //road = road.OrderBy(s => s.year).ThenBy(s=>s.volNo);
            var a = road.ToPagedList(pageNumber, pageSize);
            var b = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                                        //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程
                                        from p in a
                                        select new JObject(
                                                 new JProperty("licenceNo", p.licenceNo),
                                                 new JProperty("registrationNo", p.registrationNo),
                                                 new JProperty("applyUnit", p.applyUnit),
                                                 new JProperty("location", p.location),
                                                 new JProperty("doorplate", p.doorplate),
                                                 new JProperty("landNo", p.landNo),
                                                 new JProperty("projectRange", p.projectRange),
                                                 new JProperty("volNo", p.volNo),
                                                 new JProperty("paijiaNo", p.paijiaNo),
                                                 new JProperty("ID", p.ID),
                                                 new JProperty("year", p.year),
                                                 new JProperty("newLocation", p.newLocation),
                                                 new JProperty("areaProSeqNo", p.areaProSeqNo)
                                        )
                                )
                    )
).ToString();

            return b;
        }
        // GET: Road/Details/5
        public ActionResult Details(long? id,string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OtherArchives otherArchives = db.OtherArchives.Find(id);
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "请选择案卷厚度", Value = "0"},
                new SelectListItem { Text = "1厘米", Value = "1"},
                new SelectListItem { Text = "2厘米", Value = "2"},
                new SelectListItem { Text = "3厘米", Value = "3"},
                new SelectListItem { Text = "4厘米", Value = "4"},
                new SelectListItem { Text = "5厘米", Value = "5"},
            };
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", otherArchives.ArchiveThick);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false"},
                new SelectListItem { Text = "是", Value = "true"},
            };
            ViewBag.isYD = new SelectList(list1, "Value", "Text", otherArchives.isYD);
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", otherArchives.securityID);
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", otherArchives.retentionPeriodNo);
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" },
                new SelectListItem { Text = "公开/内部", Value = "2" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", otherArchives.isNeibu);
            ViewData["div3"] = "display:block";
            if (otherArchives.isNeibu.ToString().Trim() == "0")
            {
                ViewData["div1"] = "display:none";
                ViewData["div2"] = "display:block";
            }
            if (otherArchives.isNeibu.ToString().Trim() == "1")
            {
                ViewData["div1"] = "display:block";
                ViewData["div2"] = "display:none";
            }
            if (otherArchives.isNeibu.ToString().Trim() == "2")
            {
                ViewData["div1"] = "display:block";
                ViewData["div2"] = "display:block";
            }
            ViewData["fengpi"] = false;
            ViewData["beikaobiao"] = false;
            ViewData["juanneimulu"] = false;
            if (otherArchives == null)
            {
                return HttpNotFound();
            }
            if (action == "返回")
            {
                return RedirectToAction("Index");
            }
            if (action == "打印备考表(内部)")
            {
                return RedirectToAction("N_beikaobiao", new { id = id });
            }
            if (action == "打印备考表(外部)")
            {
                return RedirectToAction("W_beikaobiao", new { id = id });
            }
            if (action == "打印案卷封皮(内部)")
            {
                return RedirectToAction("anjuanfengpi", new { id = id, id1 = "1" });
            }
            if (action == "打印案卷封皮(外部)")
            {
                return RedirectToAction("anjuanfengpi", new { id = id, id1 = "0" });
            }
            if (action == "打印卷内目录")
            {
                return RedirectToAction("juanneimulu", new { myid = id });
            }
            //if (action == "编辑")
            //{
            //    return RedirectToAction("Edit",new { id=id});
            //}
            return View(otherArchives);
        }

        // GET: Road/Create
        public ActionResult Create(int? id, string id1)
        {
            ViewData["div1"] = "display:none";
            ViewData["div2"] = "display:none";
            ViewData["wenjianmulu"] = "display:none";

            var user = db_user.AspNetUsers.Find(User.Identity.GetUserId());
            ViewBag.luruperson = user.UserName;
            ViewBag.luruTime = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-" + DateTime.Now.Day.ToString();
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName");
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName");
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "请选择案卷厚度", Value = "0"},
                new SelectListItem { Text = "1厘米", Value = "1"},
                new SelectListItem { Text = "2厘米", Value = "2"},
                new SelectListItem { Text = "3厘米", Value = "3"},
                new SelectListItem { Text = "4厘米", Value = "4"},
                new SelectListItem { Text = "5厘米", Value = "5"},
            };
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", 0);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false"},
                new SelectListItem { Text = "是", Value = "true"},
            };
            ViewBag.isYD = new SelectList(list1, "Value", "Text");
            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1"},
                new SelectListItem { Text = "公开/内部", Value = "2"},
            };
            ViewBag.isNeibu = new SelectList(list2, "Value", "Text");
            if (id1 == "1")
            {
                var otherarchives = db.OtherArchives.Find(id);
                ViewBag.ID = otherarchives.ID;
                return View(otherarchives);
            }
            else
            {
                var otherarchives = db.OtherArchives.Find(id);
                if (otherarchives != null)
                {
                    ViewBag.ID = otherarchives.ID;

                }
                else
                {
                    ViewBag.ID = db.OtherArchives.Max(a => a.ID) + 1;
                }
            }
            
            return View();
        }

        // POST: Road/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,licenceNo,registrationNo,doorplate,landNo,projectRange,year,volNo,paijiaNo,count,securityID,retentionPeriodNo,applyUnit,location,newLocation,classTypeID,ArchiveThick,isJungongArch,isYD,isNeibu,areaProSeqNo,archiveTitle,bianzhiUnit,bianzhiTime,note,urbanID,developUnit,tranferUnit,textMaterial,drawing,jianzhuArea,luruPerson,luruTime,cunfangLocation,coordinate")] OtherArchives otherArchives, string action)
        {
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", otherArchives.securityID);
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", otherArchives.retentionPeriodNo);
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "请选择案卷厚度", Value = "0"},
                new SelectListItem { Text = "1厘米", Value = "1"},
                new SelectListItem { Text = "2厘米", Value = "2"},
                new SelectListItem { Text = "3厘米", Value = "3"},
                new SelectListItem { Text = "4厘米", Value = "4"},
                new SelectListItem { Text = "5厘米", Value = "5"},
            };
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", otherArchives.ArchiveThick);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false"},
                new SelectListItem { Text = "是", Value = "true"},
            };
            ViewBag.isYD = new SelectList(list1, "Value", "Text", otherArchives.isYD);
            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1"},
                new SelectListItem { Text = "公开/内部", Value = "2"},
            };
            ViewBag.isNeibu = new SelectList(list2, "Value", "Text", otherArchives.isNeibu);
            if (action == "提交") {
                if (ModelState.IsValid)
                {
                    //otherArchives.securityID = "1";
                    //otherArchives.retentionPeriodNo = "1";
                    //otherArchives.isJungongArch = 0;
                    //otherArchives.count = 0;
                    //otherArchives.isNeibu = "0";
                    ViewBag.ID = otherArchives.ID;
                    ViewBag.luruTime = otherArchives.luruTime;
                    ViewBag.luruperson = otherArchives.luruPerson;
                    db.OtherArchives.Add(otherArchives);
                    db.SaveChanges();
                    if (otherArchives.isNeibu.ToString().Trim() == "0")
                    {
                        ViewData["div1"] = "display:none";
                        ViewData["div2"] = "display:block";
                    }
                    if (otherArchives.isNeibu.ToString().Trim() == "1")
                    {
                        ViewData["div1"] = "display:block";
                        ViewData["div2"] = "display:none";
                    }
                    if (otherArchives.isNeibu.ToString().Trim() == "2")
                    {
                        ViewData["div1"] = "display:block";
                        ViewData["div2"] = "display:block";
                    }
                    ViewData["wenjianmulu"] = "display:inline-block";
                    //return Content("<script>alert('已成功保存！');window.location.href='./Index'</script>");
                    Response.Write("<script>alert('保存成功!');</script>");
                    return View(otherArchives);
                }
            }
            if (action == "该卷文件目录") {
                return RedirectToAction("juannei", "Road", new { id = otherArchives.ID, id2 = 0, id1 = 0 });
            }
            if (action == "打印案卷封皮(外部)") {
                long id = long.Parse(Request.Form["ID"]);
                return RedirectToAction("anjuanfengpi", new { id = id, id1 = "0" });
            }
            if (action == "打印案卷封皮(内部)")
            {
                long id = long.Parse(Request.Form["ID"]);
                return RedirectToAction("anjuanfengpi", new { id = id, id1 = "1" });
            }
            if (action == "打印备考表(外部)") {
                long id = long.Parse(Request.Form["ID"]);
                return RedirectToAction("W_beikaobiao", new { id = id });
            }
            if (action == "打印备考表(内部)")
            {
                long id = long.Parse(Request.Form["ID"]);
                return RedirectToAction("N_beikaobiao", new { id = id });
            }
            if (action == "添加下一卷") {
                if (otherArchives.count > 1)
                {
                    if (Int32.Parse(otherArchives.volNo) == otherArchives.count)
                    {
                        return RedirectToAction("Create", new { id = otherArchives.ID, id1 = 2 });
                    }
                    else
                    {
                        return RedirectToAction("Create", new { id = otherArchives.ID, id1 = 3 });
                    }
                }
                return RedirectToAction("Create", new { id = otherArchives.ID, id1 = 2 });//将上一个工程的盒号传递到下一个工程中去
            }
            return View(otherArchives);
        }
        public ActionResult anjuanfengpi(long id, string id1) {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider=SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT vw_LicenceArchivesForPrint.* FROM vw_LicenceArchivesForPrint where ID='" + id + "' ";
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            DataSet ds = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            CrystalReportViewer repview = new CrystalReportViewer();
            adapter.Fill(ds);                ///////报表连接数据库,根据建水晶报表时的连接字符串设置
            ds.DataSetName = "GHDataSet";
            DataTable dt1 = ds.Tables[0];
            //string archivetitle = dt1.Rows[0]["archiveTitle"].ToString().ToString().Replace("\r\n", " ").Replace("\n", "").Trim();
            dt1.Rows[0]["archiveTitle_neibu"] = dt1.Rows[0]["archiveTitle_neibu"].ToString().ToString().Replace("\r\n", " ").Replace("\n", "").Trim();
            string isNeibu = "内部";
            if (id1 == "0")
            {
                dt1.Rows[0]["archiveTitle_neibu"] = dt1.Rows[0]["archiveTitle"].ToString().ToString().Replace("\r\n", " ").Replace("\n", "").Trim();
                dt1.Rows[0]["textMaterial_neibu"] = dt1.Rows[0]["textMaterial"];
                dt1.Rows[0]["drawing_neibu"] = dt1.Rows[0]["drawing"];
                //page1 = db_plan.PlanProject.Where(a => a.ID == id).First().pageNo_neibu;
                isNeibu = "外部";
            }
            conn.Close();
            rptH.Load(Server.MapPath("~/") + "//Report//Road//anjuanfengpi.rpt");
            rptH.SetDataSource(dt1);
            //rptH.SetParameterValue("page", page1);
            //rptH.SetParameterValue("seqNo", seqNo1);
            //rptH.SetParameterValue("boxNo", boxno);
            //rptH.SetParameterValue("baoguanqixian", baoguanqixian);
            //rptH.SetParameterValue("miji", miji);
            rptH.SetParameterValue("isNeibu", isNeibu);
            //rptH.SetParameterValue("bianzhiUnit", bianzhiUnit);
            System.IO.Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult W_beikaobiao(vw_LicenceArchivesForPrint vwLicenceArchivesForPrint, long id, string type = "PDF")//备考表外部
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db.vw_LicenceArchivesForPrint
                      where ad.ID == id
                      select ad;
            localReport.ReportPath = Server.MapPath("~/Report/Road/W_beikaobiao.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("roadbeikaobiao", ds1);
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
        public ActionResult N_beikaobiao(vw_LicenceArchivesForPrint vwLicenceArchivesForPrint, long id, string type = "PDF")//备考表内部
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db.vw_LicenceArchivesForPrint
                      where ad.ID == id
                      select ad;
            localReport.ReportPath = Server.MapPath("~/Report/Road/N_beikaobiao.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("roadbeikaobiao", ds1);
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
        public ActionResult juanneimulu(long myid, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds = from ad in db.RoadJuannei
                     where ad.archiveID == myid
                     orderby ad.seqNo
                     select ad;
            //var n = ds.Count();
            //int page = n / 15 + 1;
            //int t = page * 15;
            //for (int i = n; i < t; i++)
            //{
            //    var ds1 = ds.ToList().AddRange();
            //}
            var ds1 = ds.ToList();
            DataSet dataset = new DataSet();
            DataTable dt = new DataTable("DataSet1");
            DataColumn dc = new DataColumn("fileNo");
            //dt.Columns.Add(dc);
            //dc = new DataColumn("firstResponsible");
            dt.Columns.Add(dc);
            dc = new DataColumn("fileName");
            dt.Columns.Add(dc);
            dc = new DataColumn("pageNo");
            dt.Columns.Add(dc);
            dc = new DataColumn("bianzhiDate");
            //dt.Columns.Add(dc);
            //dc = new DataColumn("endDate");
            //dt.Columns.Add(dc);
            //dc = new DataColumn("endPageNo");
            dt.Columns.Add(dc);
            dc = new DataColumn("remarks");
            dt.Columns.Add(dc);
            dc = new DataColumn("seqNo");
            dt.Columns.Add(dc);
            dc = new DataColumn("responsible");
            dt.Columns.Add(dc);
            dc = new DataColumn("id");
            dt.Columns.Add(dc);
            dc = new DataColumn("archiveID");
            dt.Columns.Add(dc);

            for (int i = 0; i < ds.Count(); i++)
            {
                DataRow dr = dt.NewRow();
                dr["fileNo"] = ds1[i].fileNo;
                dr["responsible"] = ds1[i].responsible;
                dr["fileName"] = ds1[i].fileName;
                dr["pageNo"] = ds1[i].pageNo;
                dr["bianzhiDate"] = ds1[i].bianzhiDate;
                //dr["endDate"] = ds1[i].endDate;
                //dr["endPageNo"] = ds1[i].endPageNo;
                dr["remarks"] = ds1[i].remarks;
                dr["seqNo"] = ds1[i].seqNo;
                dr["id"] = ds1[i].ID;
                //dr["responsible"] = ds1[i].responsible;
                dr["archiveID"] = ds1[i].archiveID;
                dt.Rows.Add(dr);
            }
            int yushu = ds.Count() % 15;
            if (yushu != 0)
            {
                int bushu = 15 - yushu;
                for (int i = 0; i < bushu; i++)
                {
                    var row = dt.NewRow(); dt.Rows.Add(row);
                }
            }
            localReport.ReportPath = Server.MapPath("~/Report/Road/JuanNeiMuLu.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("DataSet1", dt);

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
        // GET: Road/Edit/5
        public ActionResult Edit(long? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OtherArchives otherArchives = db.OtherArchives.Find(id);
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "请选择案卷厚度", Value = "0"},
                new SelectListItem { Text = "1厘米", Value = "1"},
                new SelectListItem { Text = "2厘米", Value = "2"},
                new SelectListItem { Text = "3厘米", Value = "3"},
                new SelectListItem { Text = "4厘米", Value = "4"},
                new SelectListItem { Text = "5厘米", Value = "5"},
            };
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text",otherArchives.ArchiveThick);
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false"},
                new SelectListItem { Text = "是", Value = "true"},
            };
            ViewBag.isYD = new SelectList(list1, "Value", "Text", otherArchives.isYD);
            //档案密级
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", otherArchives.securityID);
            //保管年限
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", otherArchives.retentionPeriodNo);
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" },
                new SelectListItem { Text = "公开/内部", Value = "2" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", otherArchives.isNeibu.ToString().Trim());
            ViewData["div3"] = "display:block";
            if (otherArchives.isNeibu.ToString().Trim() == "0")
            {
                ViewData["div1"] = "display:none";
                ViewData["div2"] = "display:block";
            }
            if (otherArchives.isNeibu.ToString().Trim() == "1")
            {
                ViewData["div1"] = "display:block";
                ViewData["div2"] = "display:none";
            }
            if (otherArchives.isNeibu.ToString().Trim() == "2")
            {
                ViewData["div1"] = "display:block";
                ViewData["div2"] = "display:block";
            }
            ViewData["fengpi"] = false;
            ViewData["beikaobiao"] = false;
            ViewData["juanneimulu"] = false;
            if (otherArchives == null)
            {
                return HttpNotFound();
            }          
            return View(otherArchives);
        }

        // POST: Road/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,licenceNo,registrationNo,doorplate,landNo,projectRange,year,volNo,paijiaNo,count,securityID,retentionPeriodNo,applyUnit,location,newLocation,classTypeID,ArchiveThick,isJungongArch,isYD,isNeibu,areaProSeqNo,archiveTitle,bianzhiUnit,bianzhiTime,note,urbanID,developUnit,tranferUnit,textMaterial,drawing,jianzhuArea,luruPerson,luruTime,cunfangLocation,coordinate")] OtherArchives otherArchives,string action)
        {
            ViewData["div3"] = "display:block";
            if (otherArchives.isNeibu.ToString().Trim() == "0")
            {
                ViewData["div1"] = "display:none";
                ViewData["div2"] = "display:block";
            }
            if (otherArchives.isNeibu.ToString().Trim() == "1")
            {
                ViewData["div1"] = "display:block";
                ViewData["div2"] = "display:none";
            }
            if (otherArchives.isNeibu.ToString().Trim() == "2")
            {
                ViewData["div1"] = "display:block";
                ViewData["div2"] = "display:block";
            }
            ViewData["fengpi"] = false;
            ViewData["beikaobiao"] = false;
            ViewData["juanneimulu"] = false;
            if (action == "保存修改")
            {
                //if (ModelState.IsValid)
                //{
                    //otherArchives.securityID = "1";
                    //otherArchives.retentionPeriodNo = "1";
                    //otherArchives.isJungongArch = 0;
                    //otherArchives.count = 0;
                    //otherArchives.isNeibu = "0";
                db.Entry(otherArchives).State = EntityState.Modified;
                db.SaveChanges();
               
                //return Content("<script>alert('已修改成功！');window.location.href='../Index'</script>");
                return Content("<script>alert('已修改成功！');history.go(-1);</script>");

                //}
            }
            if (action == "返回")
            {
                return RedirectToAction("Index");
            }
            if (action == "打印备考表(内部)")
            {
                long id = long.Parse(Request.Form["ID"]);
                return RedirectToAction("N_beikaobiao", new { id = id });
            }
            if (action == "打印备考表(外部)")
            {
                long id = long.Parse(Request.Form["ID"]);
                return RedirectToAction("W_beikaobiao", new { id = id });
            }
            if (action == "打印案卷封皮(内部)")
            {
                long id = long.Parse(Request.Form["ID"]);
                return RedirectToAction("anjuanfengpi", new { id = id, id1 = "1" });
            }
            if (action == "打印案卷封皮(外部)")
            {
                long id = long.Parse(Request.Form["ID"]);
                return RedirectToAction("anjuanfengpi", new { id = id, id1 = "0" });
            }
            if (action == "打印卷内目录")
            {
                long id = long.Parse(Request.Form["ID"]);
                return RedirectToAction("juanneimulu", new { myid = id });
            }
            return View(otherArchives);
        }
        public ActionResult juannei(int? id, int id2, int id1) {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "文字", Value = "文字"},
                new SelectListItem { Text = "图纸", Value = "图纸"},
                new SelectListItem { Text = "文字及图纸", Value = "文字及图纸" },
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            ViewData["juanniemulu"] = true;
            var file3 = from a in db.RoadJuannei
                        where a.archiveID == id
                        orderby a.seqNo
                        select a;
            var registion = from g in db.OtherArchives
                            where g.ID == id
                            select g.ID;
            ViewData["ArchiveNo"] = id;
            if (file3.Count() == 0)
            {
                ViewData["div"] = "display:block";
            }
            else
            {
                ViewData["div"] = "display:none";
                ViewData["juanniemulu"] = false;
            }
            ViewBag.Edit = "display:none";
            ViewBag.add = "display:none";
            if (id2 == 1)
            {
                ViewBag.Edit = "display:none";
                ViewBag.add = "display:block";
                var file = from a in db.RoadJuannei
                           where a.archiveID == id
                           orderby a.seqNo descending
                           select a;
                RoadJuannei file2 = new RoadJuannei();
                if (file.Count() == 0)
                {
                    file2.seqNo = 1;
                    file2.pageNo = "1";
                }
                else
                {
                    file2.seqNo = file.First().seqNo + 1;
                    //file2.pageNo = (file.First().endPageNo + 1).ToString();
                    List<SelectListItem> list2 = new List<SelectListItem> {
                        new SelectListItem { Text = "文字", Value = "文字"},
                        new SelectListItem { Text = "图纸", Value = "图纸"},
                        new SelectListItem { Text = "文字及图纸", Value = "文字及图纸" },
                    };
                    ViewBag.SelectedID = new SelectList(list2, "Value", "Text", file.First().type.Trim());
                }
                ViewBag.PageNo = file2.pageNo;
                ViewBag.seqNo = file2.seqNo;
                var maxid = from b in db.RoadJuannei
                            select b;
                if (maxid.Count() == 0)
                {
                    ViewBag.id = 0;
                }
                else {
                    ViewBag.id = db.RoadJuannei.Max(a => a.ID) + 1;
                }
                
                if (file.Count() == 0)
                {
                    ViewBag.fileName = "";
                    ViewBag.responsible = "";
                }
                else
                {
                    //ViewBag.fileName = file.First().fileName;
                    //ViewBag.responsible = file.First().responsible;
                    string name = file.First().fileName;
                    string res = file.First().responsible;
                    if (name == "" && res != "")
                    {
                        ViewBag.filenameid = 0;

                        var response = db.WordTable.Where(a => a.character == 2).Where(a => a.wordName.Trim() == res);
                        if (response.Count() == 0)
                        {
                            ViewBag.responsibleid = 0;
                        }
                        else
                        {
                            ViewBag.responsibleid = response.First().id;
                        }
                    }
                    if (res == "" && name != "")
                    {
                        ViewBag.responsibleid = 0;
                        var filename = db.WordTable.Where(a => a.character == 1).Where(a => a.wordName.Trim() == name);
                        if (filename.Count() == 0)
                        {
                            ViewBag.filenameid = 0;
                        }
                        else
                        {
                            ViewBag.filenameid = filename.First().id;
                        }
                    }
                    if (res != "" && name != "")
                    {
                        var filename = db.WordTable.Where(a => a.character == 1).Where(a => a.wordName.Trim() == name);
                        if (filename.Count() == 0)
                        {
                            ViewBag.filenameid = 0;
                        }
                        else
                        {
                            ViewBag.filenameid = filename.First().id;
                        }
                        var response = db.WordTable.Where(a => a.character == 2).Where(a => a.wordName.Trim() == res);
                        if (response.Count() == 0)
                        {
                            ViewBag.responsibleid = 0;
                        }
                        else
                        {
                            ViewBag.responsibleid = response.First().id;
                        }
                    }
                    if (res == "" && name == "")
                    {
                        ViewBag.filenameid = 0;
                        ViewBag.responsibleid = 0;
                    }
                }
            }
            if (id2 == 2)
            {
                ViewBag.Edit = "display:block";
                ViewBag.add = "display:none";

                var file = from ad in db.RoadJuannei
                           where ad.ID == id1
                           select ad;
                long a = file.First().archiveID;
                var file2 = from ac in db.RoadJuannei
                            where ac.archiveID == a
                            orderby ac.seqNo
                            select ac;
                var f1 = file2.First();
                int seq1 = Convert.ToInt32(f1.seqNo);
                var file4 = from ae in db.RoadJuannei
                            where ae.archiveID == a
                            orderby ae.seqNo descending
                            select ae;
                int seq2 = Convert.ToInt32(file4.First().seqNo);
                if (seq2 == 1)
                {
                    ViewData["button1"] = true;
                    ViewData["button2"] = true;
                }
                if (file.First().seqNo == seq1)
                {
                    ViewData["button1"] = true;
                }
                if (file.First().seqNo == seq2)
                {
                    ViewData["button2"] = true;
                }
                ViewBag.PageNo = file.First().pageNo;
                ViewBag.bianzhiDate = file.First().bianzhiDate;
                List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "文字", Value = "文字"},
                new SelectListItem { Text = "图纸", Value = "图纸"},
                new SelectListItem { Text = "文字及图纸", Value = "文字及图纸" },
                };
                ViewBag.SelectedID = new SelectList(list1, "Value", "Text", file.First().type.Trim());
                ViewBag.fileName = file.First().fileName;
                ViewBag.responsible = file.First().responsible;
                ViewBag.remarks = file.First().remarks;
                ViewBag.seqNo = file.First().seqNo;
                ViewBag.fileNo = file.First().fileNo;
                ViewBag.id = file.First().ID;
                string name = file.First().fileName;
                string res = file.First().responsible;
                if (name == "" && res != "")
                {
                    ViewBag.filenameid1 = 0;

                    var response = db.WordTable.Where(b => b.character == 2).Where(b => b.wordName.Trim() == res);
                    if (response.Count() == 0)
                    {
                        ViewBag.responsibleid1 = 0;
                    }
                    else
                    {
                        ViewBag.responsibleid1 = response.First().id;
                    }
                }
                if (res == "" && name != "")
                {
                    ViewBag.responsibleid1 = 0;
                    var filename = db.WordTable.Where(b => b.character == 1).Where(b => b.wordName.Trim() == name);
                    if (filename.Count() == 0)
                    {
                        ViewBag.filenameid1 = 0;
                    }
                    else
                    {
                        ViewBag.filenameid1 = filename.First().id;
                    }
                }
                if (res != "" && name != "")
                {
                    var filename = db.WordTable.Where(b => b.character == 1).Where(b => b.wordName.Trim() == name);
                    if (filename.Count() == 0)
                    {
                        ViewBag.filenameid1 = 0;
                    }
                    else
                    {
                        ViewBag.filenameid1 = filename.First().id;
                    }
                    var response = db.WordTable.Where(b => b.character == 2).Where(b => b.wordName.Trim() == res);
                    if (response.Count() == 0)
                    {
                        ViewBag.responsibleid1 = 0;
                    }
                    else
                    {
                        ViewBag.responsibleid1 = response.First().id;
                    }
                }
                if (res == "" && name == "")
                {
                    ViewBag.filenameid1 = 0;
                    ViewBag.responsibleid1 = 0;
                }
            }
            //file2.archivesNo = str3;
            //return View(file.ToList());
            ViewData["registion"] = registion.First();
            ViewBag.result = JsonConvert.SerializeObject(file3.ToList());
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult juannei(long ArchiveNo, string action, string registion)
        {
            var file = from a in db.RoadJuannei
                       where a.archiveID == ArchiveNo
                       orderby a.seqNo
                       select a;
            //var file1 = from b in db.ArchivesDetail
            //            where b.archivesNo == ArchiveNo.Trim()
            //            select b.paperProjectSeqNo;
            string id = Request.Form["id"];
            if (action == "添加")
            {
                return RedirectToAction("juannei", new { id = ArchiveNo, id2 = 1, id1 = 0 });
            }
            if (action == "返回案卷信息")
            {
                return RedirectToAction("Create", new { id = registion, id1 = 1 });
            }
            if (action == "录入完毕")
            {
                ViewData["juanniemulu"] = false;
                //ViewData["button3"] = true;
                return Content("<script >alert('该案卷卷内文件已录入完毕！');window.history.back();</script >");
            }
            if (action == "打印卷内目录")
            {
                string ID = ArchiveNo.ToString();
                return RedirectToAction("juanneimulu", "Road", new { myid = ID, format = "PDF" });
            }
            //var file2 = from a in db.ArchivesDetail
            //            where a.archivesNo == ArchiveNo
            //            select a.registrationNo;

            //int index = ArchiveNo.IndexOf('.');
            //int index1 = index + 1;
            //string str1 = ArchiveNo.Substring(0, index + 1);
            //string str2 = ArchiveNo.Substring(index + 1, ArchiveNo.Length - 1 - index);
            RoadJuannei file3 = new RoadJuannei();
            var maxid = from a in db.RoadJuannei
                        select a;
            if (maxid.Count() == 0)
            {
                file3.ID = 0;
            }
            else
            {
                file3.ID = db.RoadJuannei.Max(a => a.ID) + 1;
            }
            file3.seqNo = int.Parse(Request.Form["seqNo"]);
            file3.type = Request.Form["SelectedID"];
            file3.fileNo = Request.Form["fileNo"];
            file3.fileName = Request.Form["fileName"];
            file3.responsible = Request.Form["responsible"];
            file3.pageNo = Request.Form["PageNo"];
            file3.bianzhiDate = Request.Form["bianzhiDate"];
            file3.remarks = Request.Form["remarks"];
            file3.archiveID = ArchiveNo;
            //file3.dengjihao = file2.First();
            if (action == "确定")
            {
                //string page = Request.Form["PageNo"];
                //var no = page.IndexOf('-');
                //if (page.IndexOf('-') != -1)
                //{
                //    file3.endPageNo = int.Parse(page.Split('-').Last());
                //}
                //else
                //{
                //    file3.endPageNo = int.Parse(page);
                //}
                if (ModelState.IsValid)
                {
                    db.RoadJuannei.Add(file3);
                    db.SaveChanges();
                    return Content("<script >alert('添加成功！');window.location.href='/Road/juannei?id=" + ArchiveNo + "&id1=" + int.Parse(id) + "&id2=1" + "';</script >");
                }
            }
            if (action == "删除词条")
            {
                string temp = Request.Form["DELETE_ID"];
                var id1 = int.Parse(Request.Form["DELETE_ID"].Split('-').First());
                WordTable wordtable = db.WordTable.Find(id1);
                db.WordTable.Remove(wordtable);
                var list1 = db.WordTable.Where(ad => ad.newid > wordtable.newid).OrderBy(ad => ad.newid);
                foreach (var i in list1)
                {
                    i.newid -= 1;
                    db.Entry(i).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("juannei", new { id = ArchiveNo, id1 = int.Parse(id), id2 = 1 });
                //return Content("<script >alert('删除成功！');window.location.href='/ArchivesEnter/FileList?id1=" + ArchiveNo + "&id=" + id + "&id2=1" + "';</script >");
            }
            if (action == "取消")
            {
                return RedirectToAction("juannei", new { id = ArchiveNo, id1 = 0, id2 = 0 });
            }

            if (file == null)
            {
                return HttpNotFound();
            }
            return View();
        }
        public ActionResult DetailsJuannei(long id, string archivesNo) {
            var file = from ad in db.RoadJuannei
                       where ad.ID == id
                       select ad;
            RoadJuannei roadjuannei = file.First();
            ViewData["archivesNo"] = archivesNo;
            return View(roadjuannei);
        }
        [HttpPost]
        public ActionResult DetailsJuannei(string archivesNo)
        {
            return RedirectToAction("juannei", new { id = archivesNo, id2 = 0, id1 = 0 });
        }
        public ActionResult DeleteJuannei(long id, string id2) {
            var file = from ad in db.RoadJuannei
                       where ad.ID == id
                       select ad;

            RoadJuannei roadjuannei = file.First();
            long archiveID = file.First().archiveID;
            db.Entry(roadjuannei).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("juannei", new { id = archiveID, id1 = id, id2 = 0 });
        }
        public ActionResult EditJuannei(int? id, string archivesNo) {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var file = from ad in db.RoadJuannei
                       where ad.ID == id
                       select ad;
            long a = file.First().archiveID;
            var file2 = from ac in db.RoadJuannei
                        where ac.archiveID == a
                        orderby ac.seqNo
                        select ac;
            var f1 = file2.First();
            int seq1 = Convert.ToInt32(f1.seqNo);
            var file4 = from ae in db.RoadJuannei
                        where ae.archiveID == a
                        orderby ae.seqNo descending
                        select ae;
            int seq2 = Convert.ToInt32(file4.First().seqNo);

            if (seq2 == 1)
            {
                ViewData["button1"] = true;
                ViewData["button2"] = true;
            }
            if (file.First().seqNo == seq1)
            {
                ViewData["button1"] = true;
            }
            if (file.First().seqNo == seq2)
            {
                ViewData["button2"] = true;
            }
            ViewData["PageNo"] = file.First().pageNo;
            ViewData["bianzhiDate"] = file.First().bianzhiDate;
            ViewBag.fileName = file.First().fileName;
            ViewBag.responsible = file.First().responsible;
            ViewData["archivesNo"] = archivesNo;

            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file.First());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJuannei(int id, int seqNo, string type, string fileNo, string fileName1, string responsible1, string PageNo, string bianzhiDate, string remarks, string action, string archivesID)
        {

            var file = from ad in db.RoadJuannei
                       where ad.ID == id
                       select ad;
            ViewData["PageNo"] = file.First().pageNo;
            ViewData["bianzhiDate"] = file.First().bianzhiDate;
            ViewBag.fileName = file.First().fileName;
            ViewBag.responsible = file.First().responsible;
            RoadJuannei roadjuannei = db.RoadJuannei.Where(a => a.ID == id).First();
            var archiveno = from ac in db.RoadJuannei
                            where ac.ID == id
                            select ac.archiveID;
            long d = archiveno.First();
            var file2 = from ab in db.RoadJuannei
                        where ab.archiveID == d
                        orderby ab.seqNo
                        select ab;
            //long ArchiveNo = archiveno.First();

            //int index = ArchiveNo.IndexOf('.');
            //int index1 = index + 1;
            //string str1 = ArchiveNo.Substring(0, index + 1);
            //string str2 = ArchiveNo.Substring(index + 1, ArchiveNo.Length - 1 - index);
            var c = file2.First();
            int b = Convert.ToInt32(c.seqNo);
            int seq1 = b;

            var file4 = from ae in db.RoadJuannei
                        where ae.archiveID == d
                        orderby ae.seqNo descending
                        select ae;
            int seq2 = Convert.ToInt32(file4.First().seqNo);
            if (seq2 == seq1)
            {
                ViewData["button1"] = true;
                ViewData["button2"] = true;
            }
            if (seqNo == seq1)
            {
                ViewData["button1"] = true;
            }
            if (seqNo == seq2)
            {
                ViewData["button2"] = true;
            }
            if (action == "修改")
            {
                if (Request.Form["SelectedID"] != null && Request.Form["SelectedID"] != "")
                {

                    roadjuannei.type = Request.Form["SelectedID"];

                }
                roadjuannei.seqNo = seqNo;
                roadjuannei.fileNo = fileNo;
                roadjuannei.fileName = fileName1;
                roadjuannei.responsible = responsible1;
                roadjuannei.bianzhiDate = bianzhiDate;
                roadjuannei.pageNo = PageNo;
                roadjuannei.remarks = remarks;
                string page = Request.Form["PageNo"];
                //var no = page.IndexOf('-');
                //if (page.IndexOf('-') != -1)
                //{
                //    roadjuannei.endPageNo = int.Parse(page.Split('-').Last());
                //}
                //else
                //{
                //    roadjuannei.endPageNo = int.Parse(page);
                //}
                if (ModelState.IsValid)
                {
                    db.Entry(roadjuannei).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("<script>alert('已成功修改！');window.location.href='/Road/juannei?id=" + d + "&id2=" + id + "&id1=2" + "';</script >"); ;
                }
            }
            //if(action=="返回")
            //{
            //    //if (index1 ==0)
            //    //  {
            //    //      str1 = "";

            //    //      return RedirectToAction("FileList", new { id1 = str1, id2 = str2,id3=index1 });
            //    //  }
            //    return RedirectToAction("FileList", new { id1 = archivesNo,id=id,id2=2}); 
            ////    return Redirect(url1);               
            //}
            if (action == "上一页")
            {
                if (seqNo == seq1)
                {
                    ViewData["button1"] = true;
                    return View(file.First());
                }
                else
                {
                    int e = seqNo - 1;
                    int seq3 = Convert.ToInt32(e);
                    var file3 = from ac in db.RoadJuannei
                                where ac.seqNo == seq3 && ac.archiveID == c.archiveID
                                select ac;
                    return Content("<script>window.location.href='/Road/juannei?id=" + d + "&id1=" + file3.First().ID + "&id2=2" + "';</script >"); ;
                    //return RedirectToAction("FileList", new { id1 = archivesNo, id = file3.First().id, id2 = 2 });
                }
            }
            if (action == "下一页")
            {
                if (seqNo == seq2)
                {
                    ViewData["button2"] = true;
                }
                else
                {
                    int f = seqNo + 1;
                    int seq4 = Convert.ToInt32(f);
                    var file5 = from ac in db.RoadJuannei
                                where ac.seqNo == seq4 && ac.archiveID == c.archiveID
                                select ac;
                    return Content("<script>window.location.href='/Road/juannei?id=" + d + "&id1=" + file5.First().ID + "&id2=2" + "';</script >"); ;

                    //return RedirectToAction("FileList", new { id1 = archivesNo, id = file5.First().id, id2 = 2 });
                }
            }
            if (action == "删除词条")
            {
                var id1 = int.Parse(Request.Form["DELETE_ID1"].Split('-').First());
                WordTable wordtable = db.WordTable.Find(id1);
                db.WordTable.Remove(wordtable);
                var list1 = db.WordTable.Where(ad => ad.newid > wordtable.newid).OrderBy(ad => ad.newid);
                foreach (var i in list1)
                {
                    i.newid -= 1;
                    db.Entry(i).State = EntityState.Modified;
                }
                db.SaveChanges();
                //return RedirectToAction("FileList", new { id1 = ArchiveNo.Trim(), id = id, id2 = 2 });
                return Content("<script>window.location.href='/Road/juannei?id=" + d + "&id1=" + id + "&id2=2" + "';</script >"); ;
            }
            return View(file.First());
        }
        public ActionResult window()
        {
            return View();
        }
        // GET: Road/Delete/5
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    OtherArchives otherArchives = db.OtherArchives.Find(id);
        //    if (otherArchives == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(otherArchives);
        //}

        // POST: Road/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            OtherArchives otherArchives = db.OtherArchives.Find(id);
            var roadJuannei_temp = db.RoadJuannei.Where(d => d.archiveID == id).ToList();
            foreach (var item in roadJuannei_temp)//删除案卷的同时删除相关卷内目录
            {
                RoadJuannei roadJuannei = item;
                db.RoadJuannei.Remove(roadJuannei);
            }
            db.OtherArchives.Remove(otherArchives);
            db.SaveChanges();
            //return RedirectToAction("Index");
            return Content("<script>alert('删除成功！');history.go(-1);</script>");
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
