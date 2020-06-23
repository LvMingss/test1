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
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.OleDb;
using CrystalDecisions.Web;
using CrystalDecisions.CrystalReports.Engine;

namespace urban_archive.Controllers
{
    public class OtherArchivesController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities db_user = new UrbanUsersEntities();
        ReportDocument rptH = new ReportDocument();
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult ZTree()
        {
            return View();
        }
        public ActionResult anjuanfengpi(long id, string id1)
        {
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

            //string mijiid = db_plan.PlanProject.Where(a => a.ID == id).First().securityID;
            //string miji = db_urban.SecurityClassification.Where(a => a.securityID == mijiid).First().securityName; ;
            //string qixianid = db_plan.PlanProject.Where(a => a.ID == id).First().retentionPeriodID;
            //string baoguanqixian = db_urban.RetentionPeriod.Where(a => a.retentionPeriodNo == qixianid).First().retentionPeriodName;
            //string page1 = db_plan.PlanProject.Where(a => a.ID == id).First().pageNo;
            //string seqNo1 = db_plan.PlanProject.Where(a => a.ID == id).First().seqNo1.ToString();
            //string boxno = db_plan.PlanProject.Where(a => a.ID == id).First().boxNo;
            //string bianzhiUnit = db_plan.PlanProject.Where(a => a.ID == id).First().bianzhiUnit;
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
            rptH.Load(Server.MapPath("~/") + "//Report//zhizhao//anjuanfengpi.rpt");
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
        public ActionResult N_anjuanfengpi(long id, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds = from ad in db.vw_LicenceArchivesForPrint
                     where ad.ID == id
                     select ad;
            List<vw_LicenceArchivesForPrint> list = ds.ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].volNo != null)
                    list[i].volNo = list[i].volNo.Trim();
            }
            var ds1 = list;
            localReport.ReportPath = Server.MapPath("~/Report/zhizhao/N_anjuanfengpi.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("zhizhaofengpi", ds1);

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
        public ActionResult W_anjuanfengpi(long id, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds = from ad in db.vw_LicenceArchivesForPrint
                     where ad.ID == id
                     select ad;
            List<vw_LicenceArchivesForPrint> list = ds.ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].volNo != null)
                    list[i].volNo = list[i].volNo.Trim();
            }
            var ds1 = list;
            localReport.ReportPath = Server.MapPath("~/Report/zhizhao/W_anjuanfengpi.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("zhizhaofengpi", ds1);

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
        public ActionResult W_JuanNeiMuLu(long id, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db.LicenceFiles
                      where ad.archiveID == id
                      where ad.isNeibu.Trim() == "0"
                      select ad;
            localReport.ReportPath = Server.MapPath("~/Report/zhizhao/W_JuanNeiMuLu.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("zhizhaojuanmeimulu", ds1);
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
        public ActionResult N_JuanNeiMuLu(long id, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db.LicenceFiles
                      where ad.archiveID == id
                      where ad.isNeibu.Trim() == "1"
                      select ad;
            localReport.ReportPath = Server.MapPath("~/Report/zhizhao/N_JuanNeiMuLu.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("zhizhaojuanmeimulu", ds1);
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
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            string name = user.UserName;
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db.vw_LicenceArchivesForPrint
                      where ad.ID == id
                      select ad;
            localReport.ReportPath = Server.MapPath("~/Report/zhizhao/N_beikaobiao.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("zhizhaobeikaobiao", ds1);
            localReport.DataSources.Add(reportDataSource1);
            ReportParameter r = new ReportParameter("LijuanName", name);
            localReport.SetParameters(r);

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
        public ActionResult W_beikaobiao(vw_LicenceArchivesForPrint vwLicenceArchivesForPrint, long id, string type = "PDF")//备考表外部
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            string name = user.UserName;
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db.vw_LicenceArchivesForPrint
                      where ad.ID == id
                      select ad;
            localReport.ReportPath = Server.MapPath("~/Report/zhizhao/W_beikaobiao.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("zhizhaobeikaobiao", ds1);
            localReport.DataSources.Add(reportDataSource1);
            ReportParameter r = new ReportParameter("LijuanName", name);
            localReport.SetParameters(r);

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
        // GET: OtherArchives
        public ActionResult Index( string searchString, int? page, int? SelectedID)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "申请单位", Value = "0" },
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "最新工程地点", Value = "2"},
                new SelectListItem { Text = "执照号", Value = "3"},
                new SelectListItem { Text = "执照号（不含“东”）", Value = "8"},
                new SelectListItem { Text = "工程内容", Value = "4" },
                new SelectListItem { Text = "年度序号", Value = "5"},
                new SelectListItem { Text = "总登记号", Value = "6"},
                new SelectListItem { Text = "区工程顺序号", Value = "7"},
                new SelectListItem { Text = "市（区）", Value = "9"}
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");

            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            var otherArchives = from ad in db.OtherArchives
                                where ad.classTypeID == 1
                                //orderby ad.year descending
                                select ad;//按year排(降)序
            if (user.RoleName == "科员")
            {
                otherArchives = otherArchives.Where(x => x.luruPerson == user.UserName);
            }
            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        otherArchives = otherArchives.Where(ad => ad.applyUnit.Contains(searchString));//根据申请单位搜索
                        break;
                    case 1:
                        otherArchives = otherArchives.Where(ad => ad.location.Contains(searchString));//根据工程地点搜索
                        break;
                    case 2:
                        otherArchives = otherArchives.Where(ad => ad.newLocation.Contains(searchString));//根据最新工程地点搜索
                        break;
                    case 3:
                        otherArchives = otherArchives.Where(ad => ad.licenceNo.Contains(searchString));//根据执照号搜索
                        break;
                    case 4:
                        otherArchives = otherArchives.Where(ad => ad.projectRange.Contains(searchString));//根据工程内容搜索
                        break;
                    case 5:
                        otherArchives = otherArchives.Where(ad => ad.registrationNo.Contains(searchString));//根据年度序号搜索
                        break;
                    case 6:
                        otherArchives = otherArchives.Where(ad => ad.classNo.Contains(searchString));//根据总登记号搜索
                        break;
                    case 7:
                        otherArchives = otherArchives.Where(ad => ad.areaProSeqNo.Contains(searchString));//根据区工程顺序号搜索
                        break;
                    case 8:
                        otherArchives = otherArchives.Except(otherArchives.Where(ad => ad.licenceNo.Contains(searchString)));
                        break;
                    case 9:
                        otherArchives = otherArchives.Where(ad => ad.landNo.Contains(searchString));//根据区域搜索
                        break;
                }

            }
            //otherArchives = otherArchives.Where(s => s.ID>=1).OrderBy(s => s.urban_type).ThenBy(s => s.year).ThenBy(s => s.areaProSeqNo).ThenBy(s => s.registrationNo).ThenBy(s => s.volNo);
            ViewBag.CurrentFilter = searchString;
            ViewBag.count = otherArchives.Count();
            return View();
           

        }
        public string IndexData(string searchString, int? page, int? SelectedID)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "申请单位", Value = "0" },
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "最新工程地点", Value = "2"},
                new SelectListItem { Text = "执照号", Value = "3"},
                new SelectListItem { Text = "执照号（不含“东”）", Value = "8"},
                new SelectListItem { Text = "工程内容", Value = "4" },
                new SelectListItem { Text = "年度序号", Value = "5"},
                new SelectListItem { Text = "总登记号", Value = "6"},
                new SelectListItem { Text = "区工程顺序号", Value = "7"},
                new SelectListItem { Text = "市（区）", Value = "9"}
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值

            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);

            var otherArchives = from ad in db.OtherArchives
                                where ad.classTypeID == 1
                                //orderby ad.year descending
                                select ad;//按year排(降)序
            if (user.RoleName == "科员")
            {
                otherArchives = otherArchives.Where(x => x.luruPerson == user.UserName);
            }
            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        otherArchives = otherArchives.Where(ad => ad.applyUnit.Contains(searchString));//根据申请单位搜索
                        break;
                    case 1:
                        otherArchives = otherArchives.Where(ad => ad.location.Contains(searchString));//根据工程地点搜索
                        break;
                    case 2:
                        otherArchives = otherArchives.Where(ad => ad.newLocation.Contains(searchString));//根据最新工程地点搜索
                        break;
                    case 3:
                        otherArchives = otherArchives.Where(ad => ad.licenceNo.Contains(searchString));//根据执照号搜索
                        break;
                    case 4:
                        otherArchives = otherArchives.Where(ad => ad.projectRange.Contains(searchString));//根据工程内容搜索
                        break;
                    case 5:
                        otherArchives = otherArchives.Where(ad => ad.registrationNo.Contains(searchString));//根据年度序号搜索
                        break;
                    case 6:
                        otherArchives = otherArchives.Where(ad => ad.classNo.Contains(searchString));//根据总登记号搜索
                        break;
                    case 7:
                        otherArchives = otherArchives.Where(ad => ad.areaProSeqNo.Contains(searchString));//根据区工程顺序号搜索
                        break;
                    case 8:
                        otherArchives = otherArchives.Except(otherArchives.Where(ad => ad.licenceNo.Contains("东")));
                        break;
                    case 9:
                        otherArchives = otherArchives.Where(ad => ad.landNo.Contains(searchString));//根据区域搜索
                        break;
                }

            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            int cnt = otherArchives.Count() / pageSize + 1;
            if (otherArchives.Count() % pageSize == 0)
            {
                cnt = otherArchives.Count() / pageSize;
            }
            otherArchives = otherArchives.OrderBy(s => s.year).ThenBy(s => s.urban_type).ThenBy(s => s.licenceNo).ThenBy(s => s.volNo);
            //otherArchives = otherArchives.OrderBy(s => s.licenceNo);
            var a = otherArchives.ToPagedList(pageNumber, pageSize);
            var b = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                                        //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程
                                        from p in a
                                        select new JObject(
                                                 new JProperty("registrationNo", p.registrationNo),
                                                 new JProperty("licenceNo", p.licenceNo),
                                                 new JProperty("luruPerson", p.luruPerson),
                                                 new JProperty("projectRange", p.projectRange),
                                                 new JProperty("archiveTitle", p.archiveTitle),
                                                 new JProperty("year", p.year),
                                                 new JProperty("ID", p.ID),
                                                 new JProperty("status", p.status),
                                                 new JProperty("applyUnit", p.applyUnit),
                                                 new JProperty("location", p.location),
                                                 new JProperty("newLocation", p.newLocation),
                                                 new JProperty("classNo", p.classNo),
                                                 new JProperty("areaProSeqNo", p.areaProSeqNo)
                                        )
                                )
                    )
).ToString();

            return b;

        }
        public ActionResult Index_GD(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {

            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "申请单位", Value = "0" },
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "最新工程地点", Value = "2"},
                new SelectListItem { Text = "执照号", Value = "3"},
                new SelectListItem { Text = "工程内容", Value = "4" },
                new SelectListItem { Text = "年度序号", Value = "5"},
                new SelectListItem { Text = "总登记号", Value = "6"},
                new SelectListItem { Text = "区工程顺序号", Value = "7"}
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);

            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值

            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            //bool t = User.IsInRole("业务科");
            var otherArchives = from ad in db.OtherArchives
                                where ad.classTypeID == 1
                                where ad.status == "LR"
                                //orderby ad.year descending
                                select ad;//按year排(降)序
            ;
            ;
            if (user.RoleName == "科员")
            {
                otherArchives = otherArchives.Where(x => x.luruPerson == user.UserName);
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        otherArchives = otherArchives.Where(ad => ad.applyUnit.Contains(searchString));//根据申请单位搜索
                        break;
                    case 1:
                        otherArchives = otherArchives.Where(ad => ad.location.Contains(searchString));//根据工程地点搜索
                        break;
                    case 2:
                        otherArchives = otherArchives.Where(ad => ad.newLocation.Contains(searchString));//根据最新工程地点搜索
                        break;
                    case 3:
                        otherArchives = otherArchives.Where(ad => ad.licenceNo.Contains(searchString));//根据执照号搜索
                        break;
                    case 4:
                        otherArchives = otherArchives.Where(ad => ad.projectRange.Contains(searchString));//根据工程内容搜索
                        break;
                    case 5:
                        otherArchives = otherArchives.Where(ad => ad.registrationNo.Contains(searchString));//根据年度序号搜索
                        break;
                    case 6:
                        otherArchives = otherArchives.Where(ad => ad.classNo.Contains(searchString));//根据总登记号搜索
                        break;
                    case 7:
                        otherArchives = otherArchives.Where(ad => ad.areaProSeqNo.Contains(searchString));//根据区工程顺序号搜索
                        break;
                }
            }
            //otherArchives = otherArchives.OrderByDescending(s => s.year);
            //otherArchives = otherArchives.OrderByDescending(ad => ad.year).ThenByDescending(e => e.urban_type).ThenByDescending(e => e.ID);
            otherArchives = otherArchives.OrderByDescending(ad => ad.year).ThenBy(e => e.urban_type).ThenBy(e => e.licenceNo).ThenBy(e => e.volNo);
            //otherArchives = otherArchives.OrderByDescending(ad => ad.urban_type).ThenByDescending(e => e.year).ThenBy(e => e.licenceNo).ThenBy(e => e.volNo);
            var select_year = otherArchives.GroupBy(p => new { p.year }).Select(g => g.FirstOrDefault());
            ViewBag.year = new SelectList(select_year, "year", "year");

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
            ViewBag.result = JsonConvert.SerializeObject(otherArchives);
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index_GD(string year, string quyu)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            int quyu1 = Int32.Parse(quyu);
            var otherarchive_temp = db.OtherArchives.Where(d => d.classTypeID == 1).Where(d => d.status == "LR").Where(d => d.luruPerson == user.UserName).Where(d => d.year == year).Where(d => d.urban_type == quyu1).ToList();
            foreach (var item in otherarchive_temp)//
            {
                OtherArchives otherachive = item;
                otherachive.status = "GD";
                db.Entry(otherachive).State = EntityState.Modified;
            }
            db.SaveChanges();
            return RedirectToAction("Index_GD");
        }

        public ActionResult Index_BH(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "申请单位", Value = "0" },
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "最新工程地点", Value = "2"},
                new SelectListItem { Text = "执照号", Value = "3"},
                new SelectListItem { Text = "工程内容", Value = "4" },
                new SelectListItem { Text = "年度序号", Value = "5"},
                new SelectListItem { Text = "总登记号", Value = "6"},
                new SelectListItem { Text = "区工程顺序号", Value = "7"}
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值

            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            //bool t = User.IsInRole("业务科");
            var otherArchives = from ad in db.OtherArchives
                                where ad.classTypeID == 1
                                where ad.status == "GD"
                                where ad.landNo.IndexOf("北") < 0 && ad.landNo.IndexOf("四") < 0 && ad.landNo.IndexOf("李") < 0 && ad.landNo.IndexOf("南") < 0 && ad.landNo.IndexOf("崂") < 0 && ad.landNo.IndexOf("城") < 0 && ad.landNo.IndexOf("黄") < 0 && ad.landNo.IndexOf("胶") < 0 && ad.landNo.IndexOf("胶南") < 0 && ad.landNo.IndexOf("平") < 0 && ad.landNo.IndexOf("莱") < 0 && ad.landNo.IndexOf("即") < 0 && ad.landNo.IndexOf("开") < 0 && ad.isguajie == 0 
                                //orderby ad.year descending
                                select ad;//按year排(降)序
            if (user.RoleName == "科员")
            {
                otherArchives = otherArchives.Where(x => x.luruPerson == user.UserName);
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        otherArchives = otherArchives.Where(ad => ad.applyUnit.Contains(searchString));//根据申请单位搜索
                        break;
                    case 1:
                        otherArchives = otherArchives.Where(ad => ad.location.Contains(searchString));//根据工程地点搜索
                        break;
                    case 2:
                        otherArchives = otherArchives.Where(ad => ad.newLocation.Contains(searchString));//根据最新工程地点搜索
                        break;
                    case 3:
                        otherArchives = otherArchives.Where(ad => ad.licenceNo.Contains(searchString));//根据执照号搜索
                        break;
                    case 4:
                        otherArchives = otherArchives.Where(ad => ad.projectRange.Contains(searchString));//根据工程内容搜索
                        break;
                    case 5:
                        otherArchives = otherArchives.Where(ad => ad.registrationNo.Contains(searchString));//根据年度序号搜索
                        break;
                    case 6:
                        otherArchives = otherArchives.Where(ad => ad.classNo.Contains(searchString));//根据总登记号搜索
                        break;
                    case 7:
                        otherArchives = otherArchives.Where(ad => ad.areaProSeqNo.Contains(searchString));//根据区工程顺序号搜索
                        break;
                }
            }
            otherArchives = otherArchives.OrderBy(s => s.year).ThenBy(ad => ad.urban_type).ThenBy(ad => ad.licenceNo).ThenBy(ad => ad.volNo);
            //otherArchives = otherArchives.OrderByDescending(s => s.year);
            var select_year = otherArchives.GroupBy(p => new { p.year }).Select(g => g.FirstOrDefault());
            ViewBag.year = new SelectList(select_year, "year", "year");
            
            ViewBag.result = JsonConvert.SerializeObject(otherArchives);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index_BH(string action, string year)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            if (action == "编号")
            {
                if (user.RoleName == "科员")
                {
                    return Content("<script >alert('当前用户没有权限！');window.history.back();</script >");
                }
                if (year == null)
                {
                    return Content("<script >alert('年份为空，请选择年份！');window.history.back();</script >");
                }
                //var otherArchives = from ad in db.OtherArchives
                //                    where ad.classTypeID == 1
                //                    where ad.status == "GD"
                //                    where ad.registrationNo == ""
                //                    where ad.year == year
                //                    orderby ad.licenceNo
                //                    select ad;
                var otherArchives = from ad in db.OtherArchives
                                    where ad.classTypeID == 1
                                    where ad.status == "GD"
                                    where ad.urban_type == 1
                                    //where ad.landNo.IndexOf("北") < 0 && ad.landNo.IndexOf("四") < 0 && ad.landNo.IndexOf("李") < 0 && ad.landNo.IndexOf("南") < 0 && ad.landNo.IndexOf("崂") < 0 && ad.landNo.IndexOf("城") < 0 && ad.landNo.IndexOf("黄") < 0 && ad.landNo.IndexOf("胶") < 0 && ad.landNo.IndexOf("胶南") < 0 && ad.landNo.IndexOf("平") < 0 && ad.landNo.IndexOf("莱") < 0 && ad.landNo.IndexOf("即") < 0 && ad.landNo.IndexOf("开") < 0
                                    where ad.year == year
                                    orderby ad.licenceNo, ad.volNo
                                    select ad;//筛选出状态为待编号的执照档案
                if (otherArchives.Count() == 0)
                {
                    return Content("<script >alert('案卷已经编号，请勿重复编号！');window.history.back();</script >");
                }
                if (otherArchives != null)
                {
                    string maxno = year.Trim() + "00001";//查询得到该年份执照档案最大的总登记号
                    var maxno_year = db.OtherArchives.Where(ad => ad.year == year).Where(ad => ad.classTypeID == 1).Max(ad => ad.registrationNo);
                    if (maxno_year != null && maxno_year.Trim() != "")//如果已有该年份的编号信息
                    {
                        maxno = (long.Parse(maxno_year.ToString().Substring(0, 9)) + 1).ToString();//截取最大总登记号的前9位+1
                    }
                    foreach (var bh in otherArchives)
                    {
                        string type = bh.licenceNo.Trim().Substring(0, 3);//大部分都是3个字符，只有市规划是两个字符“青规”，胶南是四个字符“青规胶南”
                        if (bh.licenceNo.Trim().Substring(0, 4) == "青规胶南")//先判断胶南
                        {
                            bh.urban_type = 10;
                        }
                        else if (type != "青规北" && type != "青规四" && type != "青规李" && type != "青规南" && type != "青规崂" && type != "青规城" && type != "青规黄" && type != "青规胶" && type != "青规平" && type != "青规莱" && type != "青规即" && type != "青规开")//只能是市规划了
                        {
                            bh.urban_type = 1;
                        }
                        else
                        {
                            switch (type)
                            {
                                case "青规北":
                                    bh.urban_type = 2;
                                    break;
                                case "青规四":
                                    bh.urban_type = 3;
                                    break;
                                case "青规李":
                                    bh.urban_type = 4;
                                    break;
                                case "青规南":
                                    bh.urban_type = 5;
                                    break;
                                case "青规崂":
                                    bh.urban_type = 6;
                                    break;
                                case "青规城":
                                    bh.urban_type = 7;
                                    break;
                                case "青规黄":
                                    bh.urban_type = 8;
                                    break;
                                case "青规胶":
                                    bh.urban_type = 9;
                                    break;
                                case "青规平":
                                    bh.urban_type = 11;
                                    break;
                                case "青规莱":
                                    bh.urban_type = 12;
                                    break;
                                case "青规即":
                                    bh.urban_type = 13;
                                    break;
                                case "青规开":
                                    bh.urban_type = 14;
                                    break;
                            }
                        }
                        if (bh.count == 1)
                        {
                            bh.registrationNo = maxno;
                            maxno = (long.Parse(maxno.ToString().Substring(0, 9)) + 1).ToString();//当前案卷编号，并把当前最大总登记号更新+1
                        }
                        else
                        {
                            String s = bh.volNo.Trim().PadLeft(3, '0');
                            bh.registrationNo = maxno + "-" + s;//当前案卷编号：200900566-001
                            if (int.Parse(bh.volNo.Trim()) >= bh.count)//如果是该案卷的最后一卷,最大总登记号前9位+1  
                            {
                                maxno = (long.Parse(maxno.ToString().Substring(0, 9)) + 1).ToString();
                            }
                        }
                        bh.status = "BH";
                        db.Entry(bh).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
            if (action == "取消编号")
            {
                if (user.RoleName == "科员")
                {
                    return Content("<script >alert('当前用户没有权限！');window.history.back();</script >");
                }
                var otherarchives1 = from ad in db.OtherArchives
                                     where ad.classTypeID == 1
                                     where ad.status == "BH"
                                     where ad.urban_type == 1
                                     select ad;//检索出已经编号的档案
                foreach (var item2 in otherarchives1)//给每一个案卷的区工程顺序号赋空
                {
                    item2.registrationNo = "";
                    item2.status = "GD";
                    db.Entry(item2).State = EntityState.Modified;

                }
                db.SaveChanges();
            }
            return RedirectToAction("Index_BH");
        }


        public ActionResult Index_BHQ(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "申请单位", Value = "0" },
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "最新工程地点", Value = "2"},
                new SelectListItem { Text = "执照号", Value = "3"},
                new SelectListItem { Text = "工程内容", Value = "4" },
                new SelectListItem { Text = "年度序号", Value = "5"},
                new SelectListItem { Text = "总登记号", Value = "6"},
                new SelectListItem { Text = "区工程顺序号", Value = "7"}
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值

            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            //bool t = User.IsInRole("业务科");
            var otherArchives = from ad in db.OtherArchives
                                where ad.classTypeID == 1
                                where ad.status == "GD"
                                where ad.landNo.IndexOf("北") > 0 || ad.landNo.IndexOf("四") > 0 || ad.landNo.IndexOf("李") > 0 || ad.landNo.IndexOf("南") > 0 || ad.landNo.IndexOf("崂") > 0 || ad.landNo.IndexOf("城") > 0 || ad.landNo.IndexOf("黄") > 0 || ad.landNo.IndexOf("胶") > 0 || ad.landNo.IndexOf("胶南") > 0 || ad.landNo.IndexOf("平") > 0 || ad.landNo.IndexOf("莱") > 0 || ad.landNo.IndexOf("即") > 0 || ad.landNo.IndexOf("开") > 0 || ad.isguajie != 0
                                //orderby ad.year descending
                                select ad;//按year排(降)序
            if (user.RoleName == "科员")
            {
                otherArchives = otherArchives.Where(x => x.luruPerson == user.UserName);
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        otherArchives = otherArchives.Where(ad => ad.applyUnit.Contains(searchString));//根据申请单位搜索
                        break;
                    case 1:
                        otherArchives = otherArchives.Where(ad => ad.location.Contains(searchString));//根据工程地点搜索
                        break;
                    case 2:
                        otherArchives = otherArchives.Where(ad => ad.newLocation.Contains(searchString));//根据最新工程地点搜索
                        break;
                    case 3:
                        otherArchives = otherArchives.Where(ad => ad.licenceNo.Contains(searchString));//根据执照号搜索
                        break;
                    case 4:
                        otherArchives = otherArchives.Where(ad => ad.projectRange.Contains(searchString));//根据工程内容搜索
                        break;
                    case 5:
                        otherArchives = otherArchives.Where(ad => ad.registrationNo.Contains(searchString));//根据年度序号搜索
                        break;
                    case 6:
                        otherArchives = otherArchives.Where(ad => ad.classNo.Contains(searchString));//根据总登记号搜索
                        break;
                    case 7:
                        otherArchives = otherArchives.Where(ad => ad.areaProSeqNo.Contains(searchString));//根据区工程顺序号搜索
                        break;
                }
            }
            otherArchives = otherArchives.OrderBy(s => s.year).ThenBy(ad => ad.urban_type).ThenBy(ad => ad.licenceNo).ThenBy(ad => ad.volNo);
            //otherArchives = otherArchives.OrderByDescending(s => s.year).ThenBy(s => s.doorplate);
            var select_year = otherArchives.GroupBy(p => new { p.year }).Select(g => g.FirstOrDefault()).OrderBy(d => d.year);
            ViewBag.year = new SelectList(select_year, "year", "year");
            List<SelectListItem> list2 = new List<SelectListItem> {
                //new SelectListItem { Text = "市规划", Value = "1" },
                new SelectListItem { Text = "市北执照", Value = "2" },
                new SelectListItem { Text = "原四方执照", Value = "3" },
                new SelectListItem { Text = "李沧执照", Value = "4" },
                new SelectListItem { Text = "市南执照", Value = "5" },
                new SelectListItem { Text = "崂山执照", Value = "6" },
                new SelectListItem { Text = "城阳执照", Value = "7" },
                new SelectListItem { Text = "黄岛执照", Value = "8" },
                new SelectListItem { Text = "胶州执照", Value = "9" },
                new SelectListItem { Text = "胶南执照", Value = "10" },
                new SelectListItem { Text = "平度执照", Value = "11" },
                new SelectListItem { Text = "莱西执照", Value = "12" },
                new SelectListItem { Text = "即墨执照", Value = "13" },
                new SelectListItem { Text = "开发区执照", Value = "14" },
            };
            ViewBag.quyu = new SelectList(list2, "Value", "Text", "2");
            ViewBag.result = JsonConvert.SerializeObject(otherArchives);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index_BHQ(string action, string year, int quyu)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            if (action == "编号")
            {
                if (user.RoleName == "科员")
                {
                    return Content("<script >alert('当前用户没有权限！');window.history.back();</script >");
                }
                if (year == null)
                {
                    return Content("<script >alert('年份为空，请选择年份！');window.history.back();</script >");
                }
                var otherArchives = from ad in db.OtherArchives
                                    where ad.classTypeID == 1
                                    where ad.status == "GD"
                                    where ad.year == year
                                    where ad.urban_type == quyu
                                    orderby ad.urban_type, ad.licenceNo, ad.volNo
                                    select ad;
                if (otherArchives.Count() == 0)
                {
                    return Content("<script >alert('案卷已经编号，请勿重复编号！');window.history.back();</script >");
                }
                if (otherArchives != null)
                {
                    string max_areaseqno = db.OtherArchives.Where(ad => ad.classTypeID == 1).Max(ad => ad.areaProSeqNo);
                    if (max_areaseqno.Trim() == "" || max_areaseqno == null)//该数据为第一条
                    {
                        max_areaseqno = "00000000";
                    }
                    max_areaseqno = max_areaseqno.Trim().Substring(0, 8);
                    int temp = int.Parse(max_areaseqno) + 1;
                    foreach (var item in otherArchives)//编号，seqno作为总顺序号
                    {
                        string type = item.licenceNo.Trim().Substring(0, 3);//大部分都是3个字符，只有市规划是两个字符“青规”，胶南是四个字符“青规胶南”
                        if (item.licenceNo.Trim().Substring(0, 4) == "青规胶南")//先判断胶南
                        {
                            item.urban_type = 10;
                        }
                        else if (type != "青规北" && type != "青规四" && type != "青规李" && type != "青规南" && type != "青规崂" && type != "青规城" && type != "青规黄" && type != "青规胶" && type != "青规平" && type != "青规莱" && type != "青规即" && type != "青规开")//只能是市规划了
                        {
                            item.urban_type = 1;
                        }
                        else
                        {
                            switch (type)
                            {
                                case "青规北":
                                    item.urban_type = 2;
                                    break;
                                case "青规四":
                                    item.urban_type = 3;
                                    break;
                                case "青规李":
                                    item.urban_type = 4;
                                    break;
                                case "青规南":
                                    item.urban_type = 5;
                                    break;
                                case "青规崂":
                                    item.urban_type = 6;
                                    break;
                                case "青规城":
                                    item.urban_type = 7;
                                    break;
                                case "青规黄":
                                    item.urban_type = 8;
                                    break;
                                case "青规胶":
                                    item.urban_type = 9;
                                    break;
                                case "青规平":
                                    item.urban_type = 11;
                                    break;
                                case "青规莱":
                                    item.urban_type = 12;
                                    break;
                                case "青规即":
                                    item.urban_type = 13;
                                    break;
                                case "青规开":
                                    item.urban_type = 14;
                                    break;
                            }
                        }
                        if (item.isguajie == 1) {
                            item.urban_type = 6;
                        }
                        string cur_max = temp.ToString().PadLeft(8, '0');//左边填充0，补齐8位  
                        if (item.count > 1)//如果一个工程有多卷，则需要
                        {
                            item.areaProSeqNo = cur_max + "-" + item.volNo.ToString().PadLeft(3, '0');//编号示例：000005-001
                            if (item.count == Int32.Parse(item.volNo))
                            {
                                temp += 1;
                            }
                        }
                        else
                        {//编号示例：000005
                            item.areaProSeqNo = cur_max;//totalSeqNo赋值为当前最大号+1
                            temp += 1;
                        }
                        item.status = "BH";//完成编号
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
            if (action == "取消编号")
            {
                if (user.RoleName == "科员")
                {
                    return Content("<script >alert('当前用户没有权限！');window.history.back();</script >");
                }
                var otherarchives1 = from ad in db.OtherArchives
                                     where ad.classTypeID == 1
                                     where ad.status == "BH"
                                     //where ad.urban_type != 1
                                     where ad.year == year
                                     where ad.urban_type == quyu
                                     select ad;//检索出已经编号的档案
                foreach (var item2 in otherarchives1)//给每一个案卷的区工程顺序号赋空
                {
                    item2.areaProSeqNo = "";
                    item2.status = "GD";
                    db.Entry(item2).State = EntityState.Modified;

                }
                db.SaveChanges();
            }
            return RedirectToAction("Index_BHQ");
        }


        public ActionResult Index_SH(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            //var num = from a in db.OtherArchives
            //          where a.urban_type == 1
            //          select a;
            //for (var i = 0; i < num.Count(); i++)
            //{
            //    if (num[i].landNo != null && num[i].landNo != "" && num[i].ID >= 68257)
            //    {
            //        var bb = num[i].landNo.Substring(0, 3);
            //        switch (bb)
            //        {
            //            case "青规北":
            //                num[i].urban_type = 2;
            //                break;
            //            case "青规四":
            //                num[i].urban_type = 3;
            //                break;
            //            case "青规李建":
            //                num[i].urban_type = 4;
            //                break;
            //            case "青规南":
            //                num[i].urban_type = 5;
            //                break;
            //            case "青规崂":
            //                num[i].urban_type = 6;
            //                break;
            //            case "青规城":
            //                num[i].urban_type = 7;
            //                break;
            //            case "青规黄":
            //                num[i].urban_type = 8;
            //                break;
            //            case "青规胶":
            //                num[i].urban_type = 9;
            //                break;
            //            case "青规胶南":
            //                num[i].urban_type = 10;
            //                break;
            //            case "青规平":
            //                num[i].urban_type = 11;
            //                break;
            //            case "青规莱":
            //                num[i].urban_type = 12;
            //                break;
            //            case "青规即":
            //                num[i].urban_type = 13;
            //                break;
            //            case "青规开":
            //                break;
            //            default:
            //                num[i].urban_type = 1;
            //                break;
            //        }
            //        if (num[i].landNo == "青规胶南")
            //        {
            //            num[i].urban_type = 10;
            //        }
            //    }
            //}

            List < SelectListItem > list = new List<SelectListItem> {
                new SelectListItem { Text = "申请单位", Value = "0" },
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "最新工程地点", Value = "2"},
                new SelectListItem { Text = "执照号", Value = "3"},
                new SelectListItem { Text = "工程内容", Value = "4" },
                new SelectListItem { Text = "年度序号", Value = "5"},
                new SelectListItem { Text = "总登记号", Value = "6"},
                new SelectListItem { Text = "区工程顺序号", Value = "7"}
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值

            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            //bool t = User.IsInRole("业务科");
            var otherArchives = from ad in db.OtherArchives
                                where ad.classTypeID == 1
                                //orderby ad.year descending
                                select ad;//按year排(降)序

            if (user.RoleName == "科员")
            {
                otherArchives = otherArchives.Where(x => x.luruPerson == user.UserName);
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        otherArchives = otherArchives.Where(ad => ad.applyUnit.Contains(searchString));//根据申请单位搜索
                        break;
                    case 1:
                        otherArchives = otherArchives.Where(ad => ad.location.Contains(searchString));//根据工程地点搜索
                        break;
                    case 2:
                        otherArchives = otherArchives.Where(ad => ad.newLocation.Contains(searchString));//根据最新工程地点搜索
                        break;
                    case 3:
                        otherArchives = otherArchives.Where(ad => ad.licenceNo.Contains(searchString));//根据执照号搜索
                        break;
                    case 4:
                        otherArchives = otherArchives.Where(ad => ad.projectRange.Contains(searchString));//根据工程内容搜索
                        break;
                    case 5:
                        otherArchives = otherArchives.Where(ad => ad.registrationNo.Contains(searchString));//根据年度序号搜索
                        break;
                    case 6:
                        otherArchives = otherArchives.Where(ad => ad.classNo.Contains(searchString));//根据总登记号搜索
                        break;
                    case 7:
                        otherArchives = otherArchives.Where(ad => ad.areaProSeqNo.Contains(searchString));//根据区工程顺序号搜索
                        break;
                }
            }
            
            otherArchives = otherArchives.Where(s => s.status == "BH").OrderBy(s => s.urban_type).ThenBy(s => s.year).ThenBy(s => s.areaProSeqNo).ThenBy(s => s.registrationNo).ThenBy(s => s.volNo);
            //otherArchives = otherArchives.Where(s => s.status == "BH").OrderBy(s => s.year).ThenBy(s => s.urban_type).ThenBy(s => s.licenceNo).ThenBy(s => s.volNo);
            //otherArchives = otherArchives.Where(s => s.status == "BH").OrderByDescending(s => s.year).ThenBy(s => s.licenceNo);
            var select_year = otherArchives.GroupBy(p => new { p.year }).Select(g => g.FirstOrDefault());
            ViewBag.year = new SelectList(select_year, "year", "year");
            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "市规划", Value = "1" },
                new SelectListItem { Text = "市北执照", Value = "2" },
                new SelectListItem { Text = "原四方执照", Value = "3" },
                new SelectListItem { Text = "李沧执照", Value = "4" },
                new SelectListItem { Text = "市南执照", Value = "5" },
                new SelectListItem { Text = "崂山执照", Value = "6" },
                new SelectListItem { Text = "城阳执照", Value = "7" },
                new SelectListItem { Text = "黄岛执照", Value = "8" },
                new SelectListItem { Text = "胶州执照", Value = "9" },
                new SelectListItem { Text = "胶南执照", Value = "10" },
                new SelectListItem { Text = "平度执照", Value = "11" },
                new SelectListItem { Text = "莱西执照", Value = "12" },
                new SelectListItem { Text = "即墨执照", Value = "13" },
                new SelectListItem { Text = "开发区执照", Value = "14" },
            };
            ViewBag.quyu = new SelectList(list2, "Value", "Text", "2");
            //ViewBag.result = JsonConvert.SerializeObject(otherArchives);
            var num = otherArchives.ToList();
            var ceshi = "";
            for (var i = 0; i < num.Count(); i++)
            {
                ceshi = num[i].licenceNo;
                //var a1 = ceshi.IndexOf("-");
                //var a2 = ceshi.LastIndexOf("-");
                var qian = ceshi.Split("-".ToCharArray());
                if (num[i].doorplate != "" && num[i].doorplate != null && qian.Count() ==  3 && int.Parse(qian[1]) != int.Parse(num[i].doorplate)) {
                    num[i].licenceNo = qian[0] + '-' + num[i].doorplate + '-' + qian[2];
                }
                //var qian = tmp[i].licenceNo.Substring(0, a1);
                //var li = tmp[i].licenceNo.Substring(a1 + 1, a2);
                //;
                //var hou = tmp[i].licenceNo.Substring(a2, tmp[i].licenceNo.Length);
                //if (li != tmp[i].doorplate)
                //{
                //    var aa = qian + '-' + tmp[i].doorplate + '-' + hou;
                //}
            }


            
            ViewBag.result = JsonConvert.SerializeObject(num);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index_SH(string action, string year, int quyu)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            if (user.RoleName == "科员")
            {
                return Content("<script >alert('当前用户没有权限！');window.history.back();</script >");
            }
            if (action == "入库") {
                var otherarchive_temp = db.OtherArchives.Where(d => d.classTypeID == 1).Where(d => d.status == "BH").Where(d => d.year == year).Where(d => d.urban_type == quyu).ToList();
                //var otherarchive_temp = db.OtherArchives.Where(d => d.classTypeID == 1).Where(d => d.status == "BH").ToList();
                foreach (var item in otherarchive_temp)//
                {
                    OtherArchives otherachive = item;
                    otherachive.status = "RK";
                    otherachive.rukutime = DateTime.Now;
                    db.Entry(otherachive).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            if (action == "取消编号")
            {
                if (user.RoleName == "科员")
                {
                    return Content("<script >alert('当前用户没有权限！');window.history.back();</script >");
                }
                var otherarchives1 = from ad in db.OtherArchives
                                     where ad.classTypeID == 1
                                     where ad.status == "BH"
                                     //where ad.urban_type != 1
                                     where ad.year == year
                                     where ad.urban_type == quyu
                                     select ad;//检索出已经编号的档案
                foreach (var item2 in otherarchives1)//给每一个案卷的区工程顺序号赋空
                {
                    item2.areaProSeqNo = "";
                    item2.registrationNo = "";
                    item2.status = "GD";
                    db.Entry(item2).State = EntityState.Modified;

                }
                db.SaveChanges();
            }
            return RedirectToAction("Index_SH");
        }
        public ActionResult Index_RK( string searchString,  int? SelectedID)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "申请单位", Value = "0" },
                new SelectListItem { Text = "工程地点", Value = "1" },
                new SelectListItem { Text = "最新工程地点", Value = "2"},
                new SelectListItem { Text = "执照号", Value = "3"},
                new SelectListItem { Text = "工程内容", Value = "4" },
                new SelectListItem { Text = "年度序号", Value = "5"},
                new SelectListItem { Text = "总登记号", Value = "6"},
                new SelectListItem { Text = "区工程顺序号", Value = "7"}
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
        
          
            ViewBag.CurrentFilter = searchString;
            return View();
        }
        public string  Index_RKData(string searchString, int? SelectedID, int? page)
        {
           

            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);

            var otherArchives = from ad in db.OtherArchives
                                where ad.classTypeID == 1
                                where ad.status == "RK"
                                //orderby ad.year descending
                                select ad;//按year排(降)序
            
            if (user.RoleName == "科员")
            {
                otherArchives = otherArchives.Where(x => x.luruPerson == user.UserName);
            }


            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        otherArchives = otherArchives.Where(ad => ad.applyUnit.Contains(searchString));//根据申请单位搜索
                        break;
                    case 1:
                        otherArchives = otherArchives.Where(ad => ad.location.Contains(searchString));//根据工程地点搜索
                        break;
                    case 2:
                        otherArchives = otherArchives.Where(ad => ad.newLocation.Contains(searchString));//根据最新工程地点搜索
                        break;
                    case 3:
                        otherArchives = otherArchives.Where(ad => ad.licenceNo.Contains(searchString));//根据执照号搜索
                        break;
                    case 4:
                        otherArchives = otherArchives.Where(ad => ad.projectRange.Contains(searchString));//根据工程内容搜索
                        break;
                    case 5:
                        otherArchives = otherArchives.Where(ad => ad.registrationNo.Contains(searchString));//根据年度序号搜索
                        break;
                    case 6:
                        otherArchives = otherArchives.Where(ad => ad.classNo.Contains(searchString));//根据总登记号搜索
                        break;
                    case 7:
                        otherArchives = otherArchives.Where(ad => ad.areaProSeqNo.Contains(searchString));//根据区工程顺序号搜索
                        break;
                }
            }
        
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            int cnt = otherArchives.Count() / pageSize + 1;
            if (otherArchives.Count() % pageSize == 0)
            {
                cnt = otherArchives.Count() / pageSize;
            }
            otherArchives = otherArchives.OrderBy(s => s.year).ThenBy(s => s.urban_type).ThenBy(s => s.licenceNo).ThenBy(s => s.volNo);
            //otherArchives = otherArchives.OrderBy(s => s.licenceNo);
            var a = otherArchives.ToPagedList(pageNumber, pageSize);
            var b = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                                        //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程
                                        from p in a
                                        select new JObject(
                                                 new JProperty("registrationNo", p.registrationNo),
                                                 new JProperty("licenceNo", p.licenceNo),
                                                 //new JProperty("landNo", p.landNo),
                                                 new JProperty("projectRange", p.projectRange),
                                                 new JProperty("archiveTitle", p.archiveTitle),
                                                 new JProperty("year", p.year),
                                                 new JProperty("ID", p.ID),
                                                 new JProperty("status", p.status),
                                                 new JProperty("applyUnit", p.applyUnit),
                                                 new JProperty("location", p.location),
                                                 new JProperty("newLocation", p.newLocation),
                                                 new JProperty("classNo", p.classNo),
                                                 new JProperty("areaProSeqNo", p.areaProSeqNo)
                                        )
                                )
                    )
).ToString();

            return b;
        }
        // GET: OtherArchives/Details/5
        public ActionResult Details(long? id,string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OtherArchives otherArchives = db.OtherArchives.Find(id);
            //案卷厚度
            List<SelectListItem> list = new List<SelectListItem> { };
            for (int i = 1; i < 6; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = i.ToString() + "厘米";
                item.Value = i.ToString();
                list.Add(item);
            }
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", otherArchives.ArchiveThick);
            //是否异地
            List<SelectListItem> li = new List<SelectListItem>
            {
                new SelectListItem { Text = "是", Value ="1" },
                new SelectListItem { Text = "否", Value = "0"}

            };
            if (otherArchives.isYD == true)
            {
                ViewBag.isYD = new SelectList(li, "Value", "Text", 1);
            }
            else
            {
                ViewBag.isYD = new SelectList(li, "Value", "Text", 0);
            }
            //是否内部
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" },
                new SelectListItem { Text = "公开/内部", Value = "2" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", otherArchives.isNeibu.Trim());
            //档案密级
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName",otherArchives.securityID);
            //保管年限
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName",otherArchives.retentionPeriodNo);
            if (otherArchives.status == "LR" || otherArchives.status == "GD") 
            {
                //if (otherArchives.isNeibu.ToString().Trim() == "0")
                //{
                //    ViewData["div1"] = "display:none";
                //    ViewData["div2"] = "display:block";
                //}
                //if (otherArchives.isNeibu.ToString().Trim() == "1")
                //{
                //    ViewData["div1"] = "display:block";
                //    ViewData["div2"] = "display:none";
                //}
                //if (otherArchives.isNeibu.ToString().Trim() == "2")
                //{
                //    ViewData["div1"] = "display:block";
                //    ViewData["div2"] = "display:block";
                //}
                //ViewData["fengpi"] = true;
                //ViewData["beikaobiao"] = false;
                //ViewData["juanneimulu"] = true;
                //jsy
                ViewData["div1"] = "display:none";
                ViewData["div2"] = "display:none";
                return View(otherArchives);
            }
            else
            {
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
            }
            if (otherArchives == null)
            {
                // return HttpNotFound();
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
            if (action == "打印卷内目录(内部)")
            {
                return RedirectToAction("N_JuanNeiMuLu", new { id = id });
            }
            if (action == "打印卷内目录(外部)")
            {
                return RedirectToAction("W_JuanNeiMuLu", new { id = id });
            }
            return View(otherArchives);
        }

        // GET: OtherArchives/Create
        public ActionResult Create(int? id, string id1, string id2)
        {
            ViewData["div1"] = "display:none";
            ViewData["div2"] = "display:none";
            ViewData["wenjianmulu"] = "display:none";

            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" },
                new SelectListItem { Text = "公开/内部", Value = "2" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text");

            List<SelectListItem> listBianzhi = new List<SelectListItem>
            {
                new SelectListItem { Text = "青岛市规划局", Value ="青岛市规划局"},
                new SelectListItem { Text = "青岛市规划局市北分局", Value ="青岛市规划局市北分局"},
                new SelectListItem { Text = "青岛市规划局原四方分局", Value ="青岛市规划局原四方分局"},
                new SelectListItem { Text = "青岛市规划局李沧分局", Value ="青岛市规划局李沧分局"},
                new SelectListItem { Text = "青岛市规划局市南分局", Value ="青岛市规划局市南分局"},
                new SelectListItem { Text = "青岛市规划局崂山分局", Value ="青岛市规划局崂山分局"},
            };
            ViewBag.bianzhiUnit = new SelectList(listBianzhi, "Value", "Text");
            //档案密级
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName");
            //保管年限(注意这里与)
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName");
            //是否异地
            List<SelectListItem> listYD = new List<SelectListItem>
            {
                new SelectListItem { Text = "是", Value ="1" },
                new SelectListItem { Text = "否", Value = "0"}

            };
            ViewBag.isYD = new SelectList(listYD, "Value", "Text");

            List<SelectListItem> listguajie = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1" },
            };
            ViewBag.isguajie = new SelectList(listguajie, "Value", "Text", "0");

            //案卷厚度
            List<SelectListItem> list = new List<SelectListItem> { };
            for (int i = 1; i < 6; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = i.ToString() + "厘米";
                item.Value = i.ToString();
                list.Add(item);
            }
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text");
            ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", "张春颖");//审核人
            var UserID = User.Identity.GetUserId();//获取当前用户
            ViewBag.luruPerson = db_user.AspNetUsers.Find(UserID).UserName;//录入人
            ViewBag.luruTime = DateTime.Now.ToString("yyyy.MM.dd");//录入时间

            if (id2 != null && id2 != "")//将上一卷的值传递到下一卷中去，针对一卷有多盒的情况
            {
                string luruperson = db_user.AspNetUsers.Find(UserID).UserName;
                var curperson = db.OtherArchives.Where(d => d.luruPerson == luruperson).OrderByDescending(d => d.ID);
                long max_ID = curperson.First().ID;
                var model = from a in db.OtherArchives
                            where a.ID == max_ID
                            select a;
                ViewBag.ID = 0;
                if (id1 == "0")//针对一盒有多个工程的情况，将上一个工程的盒名传递到下一个工程中去
                {
                    ViewBag.year = model.First().year;
                    ViewBag.landNo = model.First().landNo;
                    ViewBag.doorplate = model.First().doorplate;
                    //ViewBag.bianzhiUnit = model.First().bianzhiUnit;
                    ViewBag.bianzhiUnit = new SelectList(listBianzhi, "Value", "Text",model.First().bianzhiUnit);
                    //ViewBag.neirongTiyao = model.First().neirongTiyao;

                }
                else
                {
                    //ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", model.First().isNeibu.Trim());
                    ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", model.First().securityID);
                    ViewBag.retentionPeriodID = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", model.First().retentionPeriodNo);
                    ViewBag.bianzhiUnit = new SelectList(listBianzhi, "Value", "Text", model.First().bianzhiUnit);
                    ViewBag.year = model.First().year;
                    ViewBag.landNo = model.First().landNo;
                    ViewBag.doorplate = model.First().doorplate;
                    //ViewBag.neirongTiyao = model.First().neirongTiyao;
                    ViewBag.licenceNo = model.First().licenceNo;
                    ViewBag.applyUnit = model.First().applyUnit;
                    ViewBag.location = model.First().location;
                    ViewBag.projectRange = model.First().projectRange;
                    //model.First().projectID = model.First().projectID + 1;
                    if (Int32.Parse(model.First().volNo) < model.First().count)
                    {
                        model.First().volNo = (Int32.Parse(model.First().volNo) + 1).ToString();
                    }
                    return View(model.First());
                }
                ViewBag.landNo = model.First().landNo;

            }
            else {
                var otherarchives = db.OtherArchives.Find(id);
                if (otherarchives != null)
                {
                    ViewBag.ID = otherarchives.ID;
                    ViewBag.projectContent = otherarchives.projectContent;
                    ViewBag.projectContent_neibu = otherarchives.projectContent_neibu;
                    if (otherarchives.bianzhiUnit != null ) {
                    }
                    ViewBag.bianzhiUnit = new SelectList(listBianzhi, "Value", "Text", otherarchives.bianzhiUnit.ToString().Trim());
                    ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", otherarchives.ArchiveThick.ToString().Trim());
                    if (otherarchives.isYD == true)
                    {
                        ViewBag.isYD = new SelectList(listYD, "Value", "Text", 1);
                    }
                    else
                    {
                        ViewBag.isYD = new SelectList(listYD, "Value", "Text", 0);
                    }
                    ViewBag.isguajie = new SelectList(listguajie, "Value", "Text", otherarchives.isguajie);
                    ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", otherarchives.securityID.ToString().Trim());
                    ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", otherarchives.retentionPeriodNo.ToString().Trim());
                    ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", otherarchives.isNeibu.ToString().Trim());
                    //input添加jsy
                    if (otherarchives.landNo != null)
                    {
                        ViewBag.landNo = otherarchives.landNo.ToString().Trim();
                    }
                    else
                    {
                        ViewBag.landNo = "";
                    }
                    if (otherarchives.year != null)
                    {
                        ViewBag.year = otherarchives.year.ToString().Trim();
                    }
                    else
                    {
                        ViewBag.year = "";
                    }
                    if (otherarchives.doorplate != null)
                    {
                        ViewBag.doorplate = otherarchives.doorplate.ToString().Trim();
                    }
                    else
                    {
                        ViewBag.doorplate = "";
                    }
                    if (otherarchives.neirongTiyao != null)
                    {
                        ViewBag.neirongTiyao = otherarchives.neirongTiyao.ToString().Trim(); //号后面
                    }
                    else
                    {
                        ViewBag.neirongTiyao = "";
                    }
                    if (otherarchives.licenceNo != null)
                    {
                        ViewBag.licenceNo = otherarchives.licenceNo.ToString().Trim();
                    }
                    else
                    {
                        ViewBag.licenceNo = "";
                    }
                    if (otherarchives.applyUnit != null)
                    {
                        ViewBag.applyUnit = otherarchives.applyUnit.ToString().Trim();
                    }
                    else
                    {
                        ViewBag.applyUnit = "";
                    }
                    ViewBag.projectRange = otherarchives.projectRange;
                    ViewBag.archiveTitle = otherarchives.archiveTitle;
                    ViewBag.archiveTitle_neibu = otherarchives.archiveTitle_neibu;
                    ViewBag.luruPerson = otherarchives.luruPerson;
                    //ViewBag.bianzhiTime = Request.Form["bianzhiTime"];
                    //ViewBag.luruTime = Request.Form["luruTime"];
                    //DateTime time = DateTime.Today;
                    //ViewBag.luruTime = time;
                    ViewBag.luruTime = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("D2") + "-" + DateTime.Now.Day.ToString();//录入时间为当前时间，zl2017.06.13
                                                                                                                                                  //ViewBag.inHouseTime = Request.Form["inHouseTime"];
                    ViewBag.count = otherarchives.count;
                    ViewBag.volNo = otherarchives.volNo;
                    ViewBag.textMaterial = otherarchives.textMaterial;
                    ViewBag.drawing = otherarchives.drawing;

                    if (otherarchives.status == "LR")
                    {
                        if (otherarchives.isNeibu.ToString().Trim() == "0")
                        {
                            ViewData["div1"] = "display:none";
                            ViewData["div2"] = "display:block";
                        }
                        if (otherarchives.isNeibu.ToString().Trim() == "1")
                        {
                            ViewData["div1"] = "display:block";
                            ViewData["div2"] = "display:none";
                        }
                        if (otherarchives.isNeibu.ToString().Trim() == "2")
                        {
                            ViewData["div1"] = "display:block";
                            ViewData["div2"] = "display:block";
                        }
                        //jsy
                        return View(otherarchives);
                    }
                    if (otherarchives.status == "GD")
                    {
                        if (otherarchives.isNeibu.ToString().Trim() == "0")
                        {
                            ViewData["div1"] = "display:none";
                            ViewData["div2"] = "display:block";
                        }
                        if (otherarchives.isNeibu.ToString().Trim() == "1")
                        {
                            ViewData["div1"] = "display:block";
                            ViewData["div2"] = "display:none";
                        }
                        if (otherarchives.isNeibu.ToString().Trim() == "2")
                        {
                            ViewData["div1"] = "display:block";
                            ViewData["div2"] = "display:block";
                        }
                        //jsy
                        return View(otherarchives);
                    }
                }
                else
                {
                    ViewBag.ID = 0;
                }
            }
            return View();
        } 
        // POST: OtherArchives/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "licenceNo,doorplate,landNo,projectRange,neirongTiyao,year,volNo,archiveTitle,bianzhiUnit,bianzhiTime,inHouseTime,count,securityID,retentionPeriodNo,applyUnit,location,note,classTypeID,ArchiveThick,urbanID,developUnit,tranferUnit,textMaterial,drawing,jianzhuArea,luruPerson,luruTime,cunfangLocation,isNeibu,coordinate,newLocation,registrationNo,classNo,areaProSeqNo,paijiaNo,projectContent,projectContent_neibu,archiveTitle_neibu,textMaterial_neibu,drawing_neibu,shenhePerson,tufu,bilichi,isguajie")] OtherArchives otherArchives,int ID,string action)
        {
            //var UserID = User.Identity.GetUserId();//获取当前用户
            //var user = db_user.AspNetUsers.Find(UserID);
            //ViewBag.ID = ID;//把ID传回视图，如果ID更新，ViewBag.id也要更新
            ////input添加jsy
            ViewBag.landNo = Request.Form["landNo"];
            ViewBag.year = Request.Form["year"];
            ViewBag.doorplate = Request.Form["doorplate"];
            ViewBag.neirongTiyao = Request.Form["neirongTiyao"];
            ViewBag.licenceNo = Request.Form["licenceNo"];
            ViewBag.applyUnit = Request.Form["applyUnit"];
            ViewBag.projectRange = Request.Form["projectRange"];
            ViewBag.projectContent = Request.Form["projectContent"];
            ViewBag.projectContent_neibu = Request.Form["projectContent_neibu"];
            ViewBag.archiveTitle = Request.Form["archiveTitle"];
            ViewBag.archiveTitle_neibu = Request.Form["archiveTitle_neibu"];
            ViewBag.luruPerson = Request.Form["luruPerson"];
            ViewBag.bianzhiTime = Request.Form["bianzhiTime"];
            ViewBag.luruTime = Request.Form["luruTime"];
            ViewBag.inHouseTime = Request.Form["inHouseTime"];
            ViewBag.count = Request.Form["count"];
            ViewBag.volNo = Request.Form["volNo"];
            ViewBag.textMaterial = Request.Form["textMaterial"];
            ViewBag.textMaterial_neibu = Request.Form["textMaterial_neibu"];
            ViewBag.drawing = Request.Form["drawing"];
            ViewBag.drawing_neibu = Request.Form["drawing_neibu"];

            //jsy
            //案卷厚度
            List<SelectListItem> list = new List<SelectListItem> { };
            for (int i = 1; i < 6; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = i.ToString() + "厘米";
                item.Value = i.ToString();
                list.Add(item);
            }
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", otherArchives.ArchiveThick);
            //是否异地   
            List<SelectListItem> li = new List<SelectListItem>
            {
                new SelectListItem { Text = "是", Value = "1" },
                new SelectListItem { Text = "否", Value = "0"}

            };

            if (otherArchives.isYD == true)
            {
                ViewBag.isYD = new SelectList(li, "Value", "Text", "1");
            }
            else
            {
                ViewBag.isYD = new SelectList(li, "Value", "Text", "0");
            }
            //档案密级
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", otherArchives.securityID);
            //保管年限
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", otherArchives.retentionPeriodNo);
            //是否为内部文件
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" },
                new SelectListItem { Text = "公开/内部", Value = "2" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", otherArchives.isNeibu);

            List<SelectListItem> listguajie = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1" },
            };
            ViewBag.isguajie = new SelectList(listguajie, "Value", "Text", otherArchives.isguajie);

            List<SelectListItem> listBianzhi = new List<SelectListItem>
            {
                new SelectListItem { Text = "青岛市规划局", Value ="青岛市规划局"},
                new SelectListItem { Text = "青岛市规划局市北分局", Value ="青岛市规划局市北分局"},
                new SelectListItem { Text = "青岛市规划局原四方分局", Value ="青岛市规划局原四方分局"},
                new SelectListItem { Text = "青岛市规划局李沧分局", Value ="青岛市规划局李沧分局"},
                new SelectListItem { Text = "青岛市规划局市南分局", Value ="青岛市规划局市南分局"},
                new SelectListItem { Text = "青岛市规划局崂山分局", Value ="青岛市规划局崂山分局"},
            };
            ViewBag.bianzhiUnit = new SelectList(listBianzhi, "Value", "Text", otherArchives.bianzhiUnit);
            ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", "张春颖");//审核人
            long max_ID = db.OtherArchives.Max(d => d.ID);//当前录入记录的最大ID
            //是否有扫描件               
            if (action == "提交")
            {
                string yd = Request.Form["isYD"];
                if (yd == "1")
                {
                    otherArchives.isYD = true;
                    ViewBag.isYD = new SelectList(li, "Value", "Text", "1");
                }
                else if (yd == "0")
                {
                    otherArchives.isYD = false;
                    ViewBag.isYD = new SelectList(li, "Value", "Text", "0");
                }
                if (ID > 0)//根据传入的ID,如果大于0，说明该条记录已存在，做修改即可
                {
                    if (ModelState.IsValid)
                    {
                        string type = otherArchives.licenceNo.Trim().Substring(0, 3);//大部分都是3个字符，只有市规划是两个字符“青规”，胶南是四个字符“青规胶南”
                        if (otherArchives.licenceNo.Trim().Substring(0, 4) == "青规胶南")//先判断胶南
                        {
                            otherArchives.urban_type = 10;
                        }
                        else if (type != "青规北" && type != "青规四" && type != "青规李" && type != "青规南" && type != "青规崂" && type != "青规城" && type != "青规黄" && type != "青规胶" && type != "青规平" && type != "青规莱" && type != "青规即" && type != "青规开")//只能是市规划了
                        {
                            otherArchives.urban_type = 1;
                        }
                        else
                        {
                            switch (type)
                            {
                                case "青规北":
                                    otherArchives.urban_type = 2;
                                    break;
                                case "青规四":
                                    otherArchives.urban_type = 3;
                                    break;
                                case "青规李":
                                    otherArchives.urban_type = 4;
                                    break;
                                case "青规南":
                                    otherArchives.urban_type = 5;
                                    break;
                                case "青规崂":
                                    otherArchives.urban_type = 6;
                                    break;
                                case "青规城":
                                    otherArchives.urban_type = 7;
                                    break;
                                case "青规黄":
                                    otherArchives.urban_type = 8;
                                    break;
                                case "青规胶":
                                    otherArchives.urban_type = 9;
                                    break;
                                case "青规平":
                                    otherArchives.urban_type = 11;
                                    break;
                                case "青规莱":
                                    otherArchives.urban_type = 12;
                                    break;
                                case "青规即":
                                    otherArchives.urban_type = 13;
                                    break;
                                case "青规开":
                                    otherArchives.urban_type = 14;
                                    break;
                            }
                        }
                        otherArchives.ID = ID;
                        otherArchives.isImageExist = "无";
                        otherArchives.status = "LR";
                        otherArchives.neirongTiyao = Request.Form["neirongtiyao"];
                        ViewBag.ID = otherArchives.ID;
                        ViewBag.count = otherArchives.count;
                        ViewBag.volNo = otherArchives.volNo;
                        ViewBag.textMaterial = otherArchives.textMaterial;
                        ViewBag.textMaterial_neibu = otherArchives.textMaterial_neibu;
                        ViewBag.drawing = otherArchives.drawing;
                        ViewBag.drawing_neibu = otherArchives.drawing_neibu;
                        db.Entry(otherArchives).State = EntityState.Modified;
                        db.SaveChanges();
                        Response.Write("<script>alert('保存成功!');</script>");
                        return View(otherArchives);
                    }
                }
                else//如果传入的ID不是数据库表中最大值，说明这是一条新的记录，创建新记录并添加到数据表中
                {
                    if (ModelState.IsValid)
                    {
                        if (otherArchives.licenceNo == null) {
                            return Content("<script>alert('执照号不能为空！');window.history.back();</script>");
                        }
                        if (otherArchives.landNo == null || otherArchives.year == null || otherArchives.doorplate == null || otherArchives.neirongTiyao == null) {
                            return Content("<script>alert('文件编号不能为空！');window.history.back();</script>");
                        }
                        if (otherArchives.projectRange == null) {
                            return Content("<script>alert('工程内容不能为空！');window.history.back();</script>");
                        }
                        if (otherArchives.archiveTitle == null) {
                            return Content("<script>alert('案卷题名不能为空！');window.history.back();</script>");
                        }
                        //if (otherArchives.bianzhiTime == null) {
                        //    return Content("<script>alert('编制时间不能为空！');window.history.back();</script>");
                        //}
                        if (otherArchives.location == null) {
                            return Content("<script>alert('工程地点不能为空！');window.history.back();</script>");
                        }
                        if (otherArchives.count == null) {
                            return Content("<script>alert('总卷数不能为空！');window.history.back();</script>");
                        }
                        if (otherArchives.volNo == null) {
                            return Content("<script>alert('卷数不能为空！');window.history.back();</script>");
                        }
                        if (otherArchives.textMaterial == null) {
                            return Content("<script>alert('总页数不能为空！');window.history.back();</script>");
                        }
                        if (otherArchives.drawing == null)
                        {
                            return Content("<script>alert('图纸页数不能为空！');window.history.back();</script>");
                        }
                        string type = otherArchives.licenceNo.Trim().Substring(0, 3);//大部分都是3个字符，只有市规划是两个字符“青规”，胶南是四个字符“青规胶南”
                        if (otherArchives.licenceNo.Trim().Substring(0, 4) == "青规胶南")//先判断胶南
                        {
                            otherArchives.urban_type = 10;
                        }
                        else if (type != "青规北" && type != "青规四" && type != "青规李" && type != "青规南" && type != "青规崂" && type != "青规城" && type != "青规黄" && type != "青规胶" && type != "青规平" && type != "青规莱" && type != "青规即" && type != "青规开")//只能是市规划了
                        {
                            otherArchives.urban_type = 1;
                        }
                        else
                        {
                            switch (type)
                            {
                                case "青规北":
                                    otherArchives.urban_type = 2;
                                    break;
                                case "青规四":
                                    otherArchives.urban_type = 3;
                                    break;
                                case "青规李":
                                    otherArchives.urban_type = 4;
                                    break;
                                case "青规南":
                                    otherArchives.urban_type = 5;
                                    break;
                                case "青规崂":
                                    otherArchives.urban_type = 6;
                                    break;
                                case "青规城":
                                    otherArchives.urban_type = 7;
                                    break;
                                case "青规黄":
                                    otherArchives.urban_type = 8;
                                    break;
                                case "青规胶":
                                    otherArchives.urban_type = 9;
                                    break;
                                case "青规平":
                                    otherArchives.urban_type = 11;
                                    break;
                                case "青规莱":
                                    otherArchives.urban_type = 12;
                                    break;
                                case "青规即":
                                    otherArchives.urban_type = 13;
                                    break;
                                case "青规开":
                                    otherArchives.urban_type = 14;
                                    break;
                            }
                        }
                        if (otherArchives.isguajie == 1) {
                            otherArchives.urban_type = 6;
                        }
                        otherArchives.ID = max_ID + 1;//新记录的ID+1，值唯一
                        ViewBag.ID = otherArchives.ID;//
                        ViewBag.count = otherArchives.count;
                        ViewBag.volNo = otherArchives.volNo;
                        ViewBag.textMaterial = otherArchives.textMaterial;
                        ViewBag.textMaterial_neibu = otherArchives.textMaterial_neibu;
                        ViewBag.drawing = otherArchives.drawing;
                        ViewBag.drawing_neibu = otherArchives.drawing_neibu;

                        otherArchives.isImageExist = "无";
                        otherArchives.status = "LR";
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
                        ViewData["fengpi"] = false;
                        ViewData["beikaobiao"] = false;
                        //ViewData["juanneimulu"] = true;
                        ViewData["tijiao"] = "display:none";
                        ViewData["wenjianmulu"] = "display:inline-block";
                        Response.Write("<script>alert('保存成功!');</script>");
                        return View(otherArchives);
                    }
                }               
            }
            else if (action == "该卷文件目录")
            {
                if (ID > 0)
                {
                    return RedirectToAction("Index", "LicenceFiles", new { archiveID = ID,id=0,id1=0 });
                }
                else
                {
                    Response.Write("<script>alert('请先提交保存!');</script>");
                    //ViewData["div1"] = "display:none";
                    //ViewData["div2"] = "display:none";

                    ViewBag.count = otherArchives.count;
                    ViewBag.volNo = otherArchives.volNo;
                    ViewBag.textMaterial = otherArchives.textMaterial;
                    ViewBag.textMaterial_neibu = otherArchives.textMaterial_neibu;
                    ViewBag.drawing = otherArchives.drawing;
                    ViewBag.drawing_neibu = otherArchives.drawing_neibu;
                    return View(otherArchives);
                }          
            }
            else if (action == "删除该案卷")
            {
                OtherArchives otherArchives_temp = db.OtherArchives.Find(ID);
                if (otherArchives_temp !=null)
                {
                    db.OtherArchives.Remove(otherArchives_temp);
                    var licenceFiles_temp = db.LicenceFiles.Where(d => d.archiveID == ID).ToList();
                    foreach (var item in licenceFiles_temp)//删除案卷的同时删除相关卷内目录
                    {
                        LicenceFiles licenceFiles = item;
                        db.LicenceFiles.Remove(licenceFiles);                                           
                    }
                    db.SaveChanges();
                    //Response.Write("<script>alert('删除成功!');</script>");
                    return Content("<script>alert('删除成功!');window.location.href='/OtherArchives/Create';</script>");
                    //return RedirectToAction("Create");
                }
                else
                {
                    Response.Write("<script>alert('请先提交保存!');</script>");
                    ViewData["div1"] = "display:none";
                    ViewData["div2"] = "display:none";

                    ViewBag.count = otherArchives.count;
                    ViewBag.volNo = otherArchives.volNo;
                    ViewBag.textMaterial = otherArchives.textMaterial;
                    ViewBag.textMaterial_neibu = otherArchives.textMaterial_neibu;
                    ViewBag.drawing_neibu = otherArchives.drawing_neibu;
                    ViewBag.drawing = otherArchives.drawing;
                    return View(otherArchives);
                }
                
            }
            else if(action == "录入完毕")//录入完毕
            {
                if (ID > 0)
                {
                    otherArchives.ID = ID;
                    ViewBag.ID = ID;
                    otherArchives.isImageExist = "无";
                    otherArchives.status = "LR";//
                    db.Entry(otherArchives).State = EntityState.Modified;
                    db.SaveChanges();
                    if (otherArchives.isNeibu == "0")
                    {
                        ViewData["div1"] = "display:none";
                        ViewData["div2"] = "display:block";
                    }
                    if (otherArchives.isNeibu == "1")
                    {
                        ViewData["div1"] = "display:block";
                        ViewData["div2"] = "display:none";
                    }
                    if (otherArchives.isNeibu == "2")
                    {
                        ViewData["div1"] = "display:block";
                        ViewData["div2"] = "display:block";
                    }
                    ViewData["fengpi"] = false;
                    ViewData["beikaobiao"] = false;
                    ViewData["juanneimulu"] = false;
                    ViewData["next"] = false;
                    //Response.Write("<script>alert('该案卷已录入完毕!');window.history.back();</script>");

                    ViewBag.count = otherArchives.count;
                    ViewBag.volNo = otherArchives.volNo;
                    ViewBag.textMaterial = otherArchives.textMaterial;
                    ViewBag.textMaterial_neibu = otherArchives.textMaterial_neibu;
                    ViewBag.drawing_neibu = otherArchives.drawing_neibu;
                    ViewBag.drawing = otherArchives.drawing;
                    
                    return View(otherArchives);
                    //return RedirectToAction("Index", new { status = "GD" });
                }
                else
                {
                    ViewData["div1"] = "display:none";
                    ViewData["div2"] = "display:none";
                    return Content("<script>alert('请先提交保存!');window.history.back();</script>");
                    //return View(otherArchives);
                }              
            }
            else if (action == "添加下一卷")
            {
                if (otherArchives.count > 1)
                {
                    if (Int32.Parse(otherArchives.volNo) == otherArchives.count)
                    {
                        return RedirectToAction("Create", new { id1 = 0, id2 = ID });
                    }
                    else
                    {
                        return RedirectToAction("Create", new { id2 = ID });
                    }
                }
                return RedirectToAction("Create", new { id1 = 0, id2 = ID });//将上一个工程的盒号传递到下一个工程中去
                //otherArchives.ID = ID;
                //otherArchives.status = "GD";//状态改为编号
                //otherArchives.isImageExist = "无";
                //db.Entry(otherArchives).State = EntityState.Modified;
                //db.SaveChanges();
                //if (otherArchives.count == null)
                //{
                //    Response.Write("<script>('总卷数为空，请输入共几卷！');</script>");
                //}
                //else {
                //if (int.Parse(otherArchives.volNo) >= otherArchives.count)//第几卷大于共多少卷，说明可以录下一种案卷了
                //{
                //    return RedirectToAction("Create");
                //}
                //else
                //{
                //    OtherArchives otherArchives_temp = new OtherArchives();
                //    long max_id = db.OtherArchives.Max(x => x.ID);
                //    otherArchives_temp.ID = max_id + 1;
                //    otherArchives_temp.applyUnit = otherArchives.applyUnit;
                //    otherArchives_temp.status = "LR";
                //    otherArchives_temp.archiveNo = otherArchives.archiveNo;
                //    otherArchives_temp.ArchiveThick = otherArchives.ArchiveThick;
                //    otherArchives_temp.archiveTitle = otherArchives.archiveTitle;
                //    otherArchives_temp.archiveTitle_neibu = otherArchives.archiveTitle_neibu;
                //    otherArchives_temp.areaNo = otherArchives.areaNo;
                //    otherArchives_temp.areaProSeqNo = otherArchives.areaProSeqNo;
                //    otherArchives_temp.areaStatus = otherArchives.areaStatus;
                //    otherArchives_temp.bianzhiTime = otherArchives.bianzhiTime;
                //    otherArchives_temp.bianzhiUnit = otherArchives.bianzhiUnit;
                //    otherArchives_temp.biaoyinPerson = otherArchives.biaoyinPerson;
                //    otherArchives_temp.biaoyinTime = otherArchives.biaoyinTime;
                //    otherArchives_temp.bilichi = otherArchives.bilichi;
                //    otherArchives_temp.classNo = otherArchives.classNo;
                //    otherArchives_temp.classTypeID = otherArchives.classTypeID;
                //    otherArchives_temp.constructionUnit = otherArchives.constructionUnit;
                //    otherArchives_temp.count = otherArchives.count;
                //    otherArchives_temp.cunfangLocation = otherArchives.cunfangLocation;
                //    otherArchives_temp.designUnit = otherArchives.designUnit;
                //    otherArchives_temp.developUnit = otherArchives.developUnit;
                //    otherArchives_temp.doorplate = otherArchives.doorplate;
                //    otherArchives_temp.drawing = otherArchives.drawing;
                //    otherArchives_temp.drawing_neibu = otherArchives.drawing_neibu;
                //    otherArchives_temp.firstResponsible = otherArchives.firstResponsible;
                //    otherArchives_temp.inHouseTime = otherArchives.inHouseTime;
                //    otherArchives_temp.isImageExist = otherArchives.isImageExist;
                //    otherArchives_temp.isJungongArch = otherArchives.isJungongArch;
                //    otherArchives_temp.isNeibu = otherArchives.isNeibu;
                //    otherArchives_temp.isYD = otherArchives.isYD;
                //    otherArchives_temp.jianzhuArea = otherArchives.jianzhuArea;
                //    otherArchives_temp.jungongTime = otherArchives.jungongTime;
                //    otherArchives_temp.kaigongTime = otherArchives.kaigongTime;
                //    otherArchives_temp.landNo = otherArchives.landNo;

                //    int neirongTiyao_temp = int.Parse(otherArchives.neirongTiyao) + 1;
                //    otherArchives_temp.licenceNo = otherArchives.year+"-"+otherArchives.doorplate.PadLeft(5,'0')+"-"+ neirongTiyao_temp.ToString().PadLeft(3,'0');

                //    otherArchives_temp.licenceTime = otherArchives.licenceTime;
                //    otherArchives_temp.location = otherArchives.location;
                //    otherArchives_temp.luruPerson = otherArchives.luruPerson;
                //    otherArchives_temp.luruTime = otherArchives.luruTime;
                //    otherArchives_temp.measureUnit = otherArchives.measureUnit;
                //    otherArchives_temp.neirongTiyao = (int.Parse(otherArchives.neirongTiyao.Trim()) + 1).ToString();
                //    otherArchives_temp.newLocation = otherArchives.newLocation;
                //    otherArchives_temp.note = otherArchives.note;

                //    otherArchives_temp.landNo = otherArchives.landNo;
                //    otherArchives_temp.otherResponsible = otherArchives.otherResponsible;
                //    otherArchives_temp.paijiaNo = otherArchives.paijiaNo;
                //    otherArchives_temp.projectRange = otherArchives.projectRange;
                //    otherArchives_temp.proSeqNo = otherArchives.proSeqNo;
                //    otherArchives_temp.registrationNo = otherArchives.registrationNo;
                //    otherArchives_temp.retentionPeriodNo = otherArchives.retentionPeriodNo;
                //    otherArchives_temp.securityID = otherArchives.securityID;
                //    otherArchives_temp.shenhePerson = otherArchives.shenhePerson;
                //    otherArchives_temp.shenheTime = otherArchives.shenheTime;
                //    otherArchives_temp.textMaterial = otherArchives.textMaterial;
                //    otherArchives_temp.textMaterial_neibu = otherArchives.textMaterial_neibu;
                //    otherArchives_temp.tranferUnit = otherArchives.tranferUnit;
                //    otherArchives_temp.tufu = otherArchives.tufu;
                //    otherArchives_temp.tuzhiniandai = otherArchives.tuzhiniandai;
                //    otherArchives_temp.tuzhiStatus = otherArchives.tuzhiStatus;
                //    otherArchives_temp.urbanID = otherArchives.urbanID;
                //    otherArchives_temp.volNo = (int.Parse(otherArchives.volNo.Trim()) + 1).ToString();//第几卷+1
                //    otherArchives_temp.year = otherArchives.year;

                //    db.OtherArchives.Add(otherArchives_temp);
                //    db.SaveChanges();
                //    //Response.Write("<script>alert('添加成功!');window.history.back();</script>");
                //    //return Content("<script>alert('添加成功!');window.location.href='/OtherArchives/Create';</script>");
                //    return RedirectToAction("Create");
                //}
                //}
                
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
            if (action == "打印卷内目录(内部)")
            {
                long id = long.Parse(Request.Form["ID"]);
                return RedirectToAction("N_JuanNeiMuLu", new { id = id });
            }
            if (action == "打印卷内目录(外部)")
            {
                long id = long.Parse(Request.Form["ID"]);
                return RedirectToAction("W_JuanNeiMuLu", new { id = id });
            }
            return View(otherArchives);
        }

        // GET: OtherArchives/Edit/5
        //public ActionResult Edit(long? id)
        //{
        //    //案卷厚度
        //    List<SelectListItem> list = new List<SelectListItem> { };
        //    for (int i = 1; i < 6; i++)
        //    {
        //        SelectListItem item = new SelectListItem();
        //        // ListItem item = new ListItem();
        //        item.Text = i.ToString() + "厘米";
        //        item.Value = i.ToString();
        //        list.Add(item);
        //    }
        //    ViewBag.ArchiveThick = new SelectList(list, "Value", "Text");
        //    ////是否异地         
        //    List<SelectListItem> listyd = new List<SelectListItem> {
        //        new SelectListItem { Text = "否", Value = "0"},
        //        new SelectListItem { Text = "是", Value = "1" },
        //    };
        //    ViewBag.isYD = new SelectList(listyd, "Value", "Text");
        //    //档案密级
        //    ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName");
        //    //保管年限
        //    ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName");
        //    //是否为内部文件
        //    List<SelectListItem> listNeibu = new List<SelectListItem> {
        //        new SelectListItem { Text = "公开", Value = "0"},
        //        new SelectListItem { Text = "内部", Value = "1" },
        //        new SelectListItem { Text = "公开/内部", Value = "2" }
        //    };
        //    ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text");
        //    List<SelectListItem> listBianzhi = new List<SelectListItem>
        //    {
        //        new SelectListItem { Text = "青岛市规划局", Value ="青岛市规划局"},
        //        new SelectListItem { Text = "青岛市规划局市北分局", Value ="青岛市规划局市北分局"},
        //        new SelectListItem { Text = "青岛市规划局原四方分局", Value ="青岛市规划局原四方分局"},
        //        new SelectListItem { Text = "青岛市规划局李沧分局", Value ="青岛市规划局李沧分局"},
        //        new SelectListItem { Text = "青岛市规划局市南分局", Value ="青岛市规划局市南分局"},
        //    };
        //    ViewBag.bianzhiUnit = new SelectList(listBianzhi, "Value", "Text");
        //    ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", "张春颖");//审核人
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    OtherArchives otherArchives = db.OtherArchives.Find(id);
        //    if (otherArchives != null)
        //    {
        //        if (otherArchives.isYD == true)
        //        {
        //            ViewBag.isYD = new SelectList(listyd, "Value", "Text", "1"); ;
        //        }
        //        else {
        //            ViewBag.isYD = new SelectList(listyd, "Value", "Text", "0"); ;
        //        }
        //        ViewBag.id = otherArchives.ID;
        //        ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", otherArchives.ArchiveThick);
        //        ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", otherArchives.securityID);
        //        ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", otherArchives.retentionPeriodNo);
        //        ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", otherArchives.isNeibu.ToString().Trim());
        //        ViewBag.bianzhiUnit = new SelectList(listBianzhi, "Value", "Text",otherArchives.bianzhiUnit.ToString().Trim());
        //        //jsy添加
        //        if (otherArchives.landNo != null)
        //        {
        //            ViewBag.landNo = otherArchives.landNo.ToString().Trim();
        //        }
        //        else
        //        {
        //            ViewBag.landNo = "";
        //        }
        //        if (otherArchives.year != null)
        //        {
        //            ViewBag.year = otherArchives.year.ToString().Trim();
        //        }
        //        else
        //        {
        //            ViewBag.year = "";
        //        }
        //        if (otherArchives.doorplate != null)
        //        {
        //            ViewBag.doorplate = otherArchives.doorplate.ToString().Trim();
        //        }
        //        else
        //        {
        //            ViewBag.doorplate = "";
        //        }
        //        if (otherArchives.neirongTiyao != null)
        //        {
        //            ViewBag.neirongTiyao = otherArchives.neirongTiyao.ToString().Trim(); //号后面
        //        }
        //        else
        //        {
        //            ViewBag.neirongTiyao = "";
        //        }
        //        if (otherArchives.licenceNo != null)
        //        {
        //            ViewBag.licenceNo = otherArchives.licenceNo.ToString().Trim();
        //        }
        //        else
        //        {
        //            ViewBag.licenceNo = "";
        //        }
        //        if (otherArchives.applyUnit != null)
        //        {
        //            ViewBag.applyUnit = otherArchives.applyUnit.ToString().Trim();
        //        }
        //        else
        //        {
        //            ViewBag.applyUnit = "";
        //        }
        //        ViewBag.projectRange = otherArchives.projectRange;
        //        ViewBag.projectContent = otherArchives.projectContent;
        //        ViewBag.projectContent_neibu = otherArchives.projectContent_neibu;
        //        ViewBag.archiveTitle = otherArchives.archiveTitle;
        //        ViewBag.archiveTitle_neibu = otherArchives.archiveTitle_neibu;
        //        ViewBag.luruPerson = otherArchives.luruPerson;
        //        ViewBag.bianzhiTime = otherArchives.bianzhiTime;
        //        ViewBag.luruTime = otherArchives.luruTime;
        //        ViewBag.inHouseTime = otherArchives.inHouseTime;
        //        ViewBag.textMaterial = otherArchives.textMaterial;
        //        ViewBag.textMaterial_neibu = otherArchives.textMaterial_neibu;
        //        ViewBag.drawing = otherArchives.drawing;
        //        ViewBag.drawing_neibu = otherArchives.drawing_neibu;
        //        ViewBag.count = otherArchives.count;
        //        ViewBag.volNo = otherArchives.volNo;
        //        if (otherArchives.status == "LR" || otherArchives.status == "GD") 
        //        {
        //            //if (otherArchives.isNeibu.ToString().Trim() == "0")
        //            //{
        //            //    ViewData["div1"] = "display:none";
        //            //    ViewData["div2"] = "display:block";
        //            //}
        //            //if (otherArchives.isNeibu.ToString().Trim() == "1")
        //            //{
        //            //    ViewData["div1"] = "display:block";
        //            //    ViewData["div2"] = "display:none";
        //            //}
        //            //if (otherArchives.isNeibu.ToString().Trim() == "2")
        //            //{
        //            //    ViewData["div1"] = "display:block";
        //            //    ViewData["div2"] = "display:block";
        //            //}
        //            //ViewData["fengpi"] = true;
        //            //ViewData["beikaobiao"] = false;
        //            //ViewData["juanneimulu"] = true;
        //            //jsy
        //            ViewData["div1"] = "display:none";
        //            ViewData["div2"] = "display:none";
        //            return View(otherArchives);
        //        }
        //        else
        //        {
        //            if (otherArchives.isNeibu.ToString().Trim() == "0")
        //            {
        //                ViewData["div1"] = "display:none";
        //                ViewData["div2"] = "display:block";
        //            }
        //            if (otherArchives.isNeibu.ToString().Trim() == "1")
        //            {
        //                ViewData["div1"] = "display:block";
        //                ViewData["div2"] = "display:none";
        //            }
        //            if (otherArchives.isNeibu.ToString().Trim() == "2")
        //            {
        //                ViewData["div1"] = "display:block";
        //                ViewData["div2"] = "display:block";
        //            }
        //            ViewData["fengpi"] = false;
        //            ViewData["beikaobiao"] = false;
        //            ViewData["juanneimulu"] = false;
        //            //jsy
        //            return View(otherArchives);
        //        }
        //    }
        //    return View(otherArchives);
        //}
        public ActionResult Edit(long? id)
        {
            //案卷厚度
            List<SelectListItem> list = new List<SelectListItem> { };
            for (int i = 1; i < 6; i++)
            {
                SelectListItem item = new SelectListItem();
                // ListItem item = new ListItem();
                item.Text = i.ToString() + "厘米";
                item.Value = i.ToString();
                list.Add(item);
            }
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text");
            ////是否异地         
            List<SelectListItem> listyd = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1" },
            };
            ViewBag.isYD = new SelectList(listyd, "Value", "Text");
            //档案密级
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName");
            //保管年限
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName");
            //是否为内部文件
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" },
                new SelectListItem { Text = "公开/内部", Value = "2" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text");
            List<SelectListItem> listBianzhi = new List<SelectListItem>
            {
                new SelectListItem { Text = "青岛市规划局", Value ="青岛市规划局"},
                new SelectListItem { Text = "青岛市规划局市北分局", Value ="青岛市规划局市北分局"},
                new SelectListItem { Text = "青岛市规划局原四方分局", Value ="青岛市规划局原四方分局"},
                new SelectListItem { Text = "青岛市规划局李沧分局", Value ="青岛市规划局李沧分局"},
                new SelectListItem { Text = "青岛市规划局市南分局", Value ="青岛市规划局市南分局"},
                new SelectListItem { Text = "青岛市规划局崂山分局", Value ="青岛市规划局崂山分局"},
            };
            ViewBag.bianzhiUnit = new SelectList(listBianzhi, "Value", "Text");
            ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", "张春颖");//审核人
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OtherArchives otherArchives = db.OtherArchives.Find(id);
            if (otherArchives != null)
            {
                if (otherArchives.isYD == true)
                {
                    ViewBag.isYD = new SelectList(listyd, "Value", "Text", "1"); ;
                }
                else
                {
                    ViewBag.isYD = new SelectList(listyd, "Value", "Text", "0"); ;
                }
                ViewBag.id = otherArchives.ID;

                List<SelectListItem> listguajie = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1" },
                };
                ViewBag.isguajie = new SelectList(listguajie, "Value", "Text", otherArchives.isguajie);

                ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", otherArchives.ArchiveThick);
                ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", otherArchives.securityID);
                ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", otherArchives.retentionPeriodNo);
                ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", otherArchives.isNeibu.ToString().Trim());
                if (otherArchives.bianzhiUnit != "" && otherArchives.bianzhiUnit != null)
                {
                    ViewBag.bianzhiUnit = new SelectList(listBianzhi, "Value", "Text", otherArchives.bianzhiUnit.ToString().Trim());
                }
                else {
                    ViewBag.bianzhiUnit = new SelectList(listBianzhi, "Value", "Text", "");
                }
                //jsy添加
                if (otherArchives.landNo != null)
                {
                    ViewBag.landNo = otherArchives.landNo.ToString().Trim();
                }
                else
                {
                    ViewBag.landNo = "";
                }
                if (otherArchives.year != null)
                {
                    ViewBag.year = otherArchives.year.ToString().Trim();
                }
                else
                {
                    ViewBag.year = "";
                }
                if (otherArchives.doorplate != null)
                {
                    ViewBag.doorplate = otherArchives.doorplate.ToString().Trim();
                }
                else
                {
                    ViewBag.doorplate = "";
                }
                if (otherArchives.neirongTiyao != null)
                {
                    ViewBag.neirongTiyao = otherArchives.neirongTiyao.ToString().Trim(); //号后面
                }
                else
                {
                    ViewBag.neirongTiyao = "";
                }
                if (otherArchives.licenceNo != null)
                {
                    ViewBag.licenceNo = otherArchives.licenceNo.ToString().Trim();
                }
                else
                {
                    ViewBag.licenceNo = "";
                }
                if (otherArchives.applyUnit != null)
                {
                    ViewBag.applyUnit = otherArchives.applyUnit.ToString().Trim();
                }
                else
                {
                    ViewBag.applyUnit = "";
                }
                ViewBag.projectRange = otherArchives.projectRange;
                ViewBag.projectContent = otherArchives.projectContent;
                ViewBag.projectContent_neibu = otherArchives.projectContent_neibu;
                ViewBag.archiveTitle = otherArchives.archiveTitle;
                ViewBag.archiveTitle_neibu = otherArchives.archiveTitle_neibu;
                ViewBag.luruPerson = otherArchives.luruPerson;
                ViewBag.bianzhiTime = otherArchives.bianzhiTime;
                ViewBag.luruTime = otherArchives.luruTime;
                ViewBag.inHouseTime = otherArchives.inHouseTime;
                ViewBag.textMaterial = otherArchives.textMaterial;
                ViewBag.textMaterial_neibu = otherArchives.textMaterial_neibu;
                ViewBag.drawing = otherArchives.drawing;
                ViewBag.drawing_neibu = otherArchives.drawing_neibu;
                ViewBag.count = otherArchives.count;
                ViewBag.volNo = otherArchives.volNo;
                if (otherArchives.status == "LR" || otherArchives.status == "GD")
                {
                    //if (otherArchives.isNeibu.ToString().Trim() == "0")
                    //{
                    //    ViewData["div1"] = "display:none";
                    //    ViewData["div2"] = "display:block";
                    //}
                    //if (otherArchives.isNeibu.ToString().Trim() == "1")
                    //{
                    //    ViewData["div1"] = "display:block";
                    //    ViewData["div2"] = "display:none";
                    //}
                    //if (otherArchives.isNeibu.ToString().Trim() == "2")
                    //{
                    //    ViewData["div1"] = "display:block";
                    //    ViewData["div2"] = "display:block";
                    //}
                    //ViewData["fengpi"] = true;
                    //ViewData["beikaobiao"] = false;
                    //ViewData["juanneimulu"] = true;
                    //jsy
                    ViewData["div1"] = "display:none";
                    ViewData["div2"] = "display:none";
                    return View(otherArchives);
                }
                else
                {
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
                    //jsy
                    return View(otherArchives);
                }
            }
            return View(otherArchives);
        }


        // POST: OtherArchives/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "status,licenceNo,doorplate,landNo,neirongTiyao,projectRange,year,volNo,archiveTitle,bianzhiUnit,bianzhiTime,inHouseTime,count,securityID,retentionPeriodNo,applyUnit,location,note,classTypeID,ArchiveThick,urbanID,developUnit,tranferUnit,textMaterial,drawing,jianzhuArea,luruPerson,luruTime,cunfangLocation,isNeibu,coordinate,newLocation,registrationNo,classNo,areaProSeqNo,paijiaNo,archiveTitle_neibu,textMaterial_neibu,drawing_neibu,projectContent_neibu,projectContent,shenhePerson,tufu,bilichi,isguajie")] OtherArchives otherArchives, int ID, string action)
        {
            //案卷厚度
            List<SelectListItem> list = new List<SelectListItem> { };
            for (int i = 1; i < 6; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = i.ToString() + "厘米";
                item.Value = i.ToString();
                list.Add(item);
            }
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", otherArchives.ArchiveThick);
            //是否异地
            List<SelectListItem> listyd = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1" },
            };
            List<SelectListItem> listguajie = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "0"},
                new SelectListItem { Text = "是", Value = "1" },
            };
            ViewBag.isguajie = new SelectList(listguajie, "Value", "Text", otherArchives.isguajie);
            //档案密级
            //保管年限
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName",otherArchives.retentionPeriodNo);
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", otherArchives.securityID);
            //是否为内部文件
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" },
                new SelectListItem { Text = "公开/内部", Value = "2" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text",otherArchives.isNeibu.ToString().Trim());
            List<SelectListItem> listBianzhi = new List<SelectListItem>
            {
                new SelectListItem { Text = "青岛市规划局", Value ="青岛市规划局"},
                new SelectListItem { Text = "青岛市规划局市北分局", Value ="青岛市规划局市北分局"},
                new SelectListItem { Text = "青岛市规划局原四方分局", Value ="青岛市规划局原四方分局"},
                new SelectListItem { Text = "青岛市规划局李沧分局", Value ="青岛市规划局李沧分局"},
                new SelectListItem { Text = "青岛市规划局市南分局", Value ="青岛市规划局市南分局"},
                new SelectListItem { Text = "青岛市规划局崂山分局", Value ="青岛市规划局崂山分局"},
            };
            if (otherArchives.bianzhiUnit != "" && otherArchives.bianzhiUnit != null)
            {
                ViewBag.bianzhiUnit = new SelectList(listBianzhi, "Value", "Text", otherArchives.bianzhiUnit.ToString().Trim());
            }
            else
            {
                ViewBag.bianzhiUnit = new SelectList(listBianzhi, "Value", "Text", "");
            }

            //ViewBag.bianzhiUnit = new SelectList(listBianzhi, "Value", "Text", otherArchives.bianzhiUnit.Trim());
            ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", "张春颖");//审核人
            if (otherArchives.landNo != null)
            {
                ViewBag.landNo = otherArchives.landNo.ToString().Trim();
            }
            else
            {
                ViewBag.landNo = "";
            }
            if (otherArchives.year != null)
            {
                ViewBag.year = otherArchives.year.ToString().Trim();
            }
            else
            {
                ViewBag.year = "";
            }
            if (otherArchives.doorplate != null)
            {
                ViewBag.doorplate = otherArchives.doorplate.ToString().Trim();
            }
            else
            {
                ViewBag.doorplate = "";
            }
            if (otherArchives.neirongTiyao != null)
            {
                ViewBag.neirongTiyao = otherArchives.neirongTiyao.ToString().Trim(); //号后面
            }
            else
            {
                ViewBag.neirongTiyao = "";
            }
            if (otherArchives.licenceNo != null)
            {
                ViewBag.licenceNo = otherArchives.licenceNo.ToString().Trim();
            }
            else
            {
                ViewBag.licenceNo = "";
            }
            if (otherArchives.applyUnit != null)
            {
                ViewBag.applyUnit = otherArchives.applyUnit.ToString().Trim();
            }
            else
            {
                ViewBag.applyUnit = "";
            }
            ViewBag.projectRange = otherArchives.projectRange;
            ViewBag.projectContent = otherArchives.projectContent;
            ViewBag.projectContent_neibu = otherArchives.projectContent_neibu;
            ViewBag.archiveTitle = otherArchives.archiveTitle;
            ViewBag.archiveTitle_neibu = otherArchives.archiveTitle_neibu;
            ViewBag.luruPerson = otherArchives.luruPerson;
            ViewBag.bianzhiTime = otherArchives.bianzhiTime;
            ViewBag.luruTime = otherArchives.luruTime;
            ViewBag.inHouseTime = otherArchives.inHouseTime;
            ViewBag.textMaterial = otherArchives.textMaterial;
            ViewBag.textMaterial_neibu = otherArchives.textMaterial_neibu;
            ViewBag.drawing = otherArchives.drawing;
            ViewBag.drawing_neibu = otherArchives.drawing_neibu;
            ViewBag.count = otherArchives.count;
            ViewBag.volNo = otherArchives.volNo;
            if (action == "修改")
            {
                string yd = Request.Form["isYD"];
                if (yd == "1")
                {
                    otherArchives.isYD = true;
                    ViewBag.isYD = new SelectList(listyd, "Value", "Text", 1);
                }
                else if (yd == "0")
                {
                    otherArchives.isYD = false;
                    ViewBag.isYD = new SelectList(listyd, "Value", "Text", 0);
                }
                if (ModelState.IsValid)
                {
                    otherArchives.ID = ID;
                    otherArchives.isImageExist = "无";
                    //otherArchives.status = "LR";
                    ViewBag.ID = otherArchives.ID;
                    string type = otherArchives.licenceNo.Trim().Substring(0, 3);//大部分都是3个字符，只有市规划是两个字符“青规”，胶南是四个字符“青规胶南”
                    if (otherArchives.licenceNo.Trim().Substring(0, 4) == "青规胶南")//先判断胶南
                    {
                        otherArchives.urban_type = 10;
                    }
                    else if (type != "青规北" && type != "青规四" && type != "青规李" && type != "青规南" && type != "青规崂" && type != "青规城" && type != "青规黄" && type != "青规胶" && type != "青规平" && type != "青规莱" && type != "青规即" && type != "青规开")//只能是市规划了
                    {
                        otherArchives.urban_type = 1;
                    }
                    else
                    {
                        switch (type)
                        {
                            case "青规北":
                                otherArchives.urban_type = 2;
                                break;
                            case "青规四":
                                otherArchives.urban_type = 3;
                                break;
                            case "青规李":
                                otherArchives.urban_type = 4;
                                break;
                            case "青规南":
                                otherArchives.urban_type = 5;
                                break;
                            case "青规崂":
                                otherArchives.urban_type = 6;
                                break;
                            case "青规城":
                                otherArchives.urban_type = 7;
                                break;
                            case "青规黄":
                                otherArchives.urban_type = 8;
                                break;
                            case "青规胶":
                                otherArchives.urban_type = 9;
                                break;
                            case "青规平":
                                otherArchives.urban_type = 11;
                                break;
                            case "青规莱":
                                otherArchives.urban_type = 12;
                                break;
                            case "青规即":
                                otherArchives.urban_type = 13;
                                break;
                            case "青规开":
                                otherArchives.urban_type = 14;
                                break;
                        }
                    }
                    if (otherArchives.isguajie == 1)
                    {
                        otherArchives.urban_type = 6;
                    }

                    db.Entry(otherArchives).State = EntityState.Modified;
                    db.SaveChanges();
                    if (otherArchives.status == "LR" || otherArchives.status == "GD") 
                    {
                        ViewData["div1"] = "display:none";
                        ViewData["div2"] = "display:none";
                        //return View(otherArchives);
                        return Content("<script>alert('已修改成功！');history.go(-1);</script>");
                    }
                    else
                    {
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
                        //return View(otherArchives);
                        return Content("<script>alert('已修改成功！');history.go(-1);</script>");
                    }
                }
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
                if (action == "打印卷内目录(内部)")
                {
                    long id = long.Parse(Request.Form["ID"]);
                    return RedirectToAction("N_JuanNeiMuLu", new { id = id });
                }
                if (action == "打印卷内目录(外部)")
                {
                    long id = long.Parse(Request.Form["ID"]);
                    return RedirectToAction("W_JuanNeiMuLu", new { id = id });
                }
            return View(otherArchives);
        }

        // GET: OtherArchives/Delete/5
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

        // POST: OtherArchives/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            OtherArchives otherArchives = db.OtherArchives.Find(id);
            var licenceFiles_temp = db.LicenceFiles.Where(d => d.archiveID == id).ToList();
            foreach (var item in licenceFiles_temp)//删除案卷的同时删除相关卷内目录
            {
              LicenceFiles licenceFiles = item;
              db.LicenceFiles.Remove(licenceFiles);
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

        public ActionResult Next(int ID) {
            OtherArchives otherarchives = db.OtherArchives.Find(ID);
            if (otherarchives.count > 1)
            {
                if (Int32.Parse(otherarchives.volNo) == otherarchives.count)
                {
                    return RedirectToAction("Create", new { id1 = 0, id2 = ID });
                }
                else
                {
                    return RedirectToAction("Create", new { id2 = ID });
                }
            }
            return RedirectToAction("Create", new { id1 = 0, id2 = ID });
        }
    }
}
