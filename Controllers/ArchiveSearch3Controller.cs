using System.Net;
using System.Web.Mvc;
using urban_archive.Models;
using PagedList;
using System.Web.Script.Serialization;
using System.Data.OleDb;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Microsoft.AspNet.Identity;
using static urban_archive.WebService;
using static urban_archive.Controllers.ArchiveSearchController;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
namespace urban_archive.Controllers
{
    public class ArchiveSearch3Controller : Controller
    {
        // GET: ArchiveSearch3
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();

        public ActionResult LookYuanChuanProject(string SerachText)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider=SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count FROM UrbanCon.dbo.vw_YuanChuanprojectList  where status='7' and ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachText;
        
            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.ChineseTableList
                               where b.chinese == Require
                               select b.tablename;
                    Require = name.First();
                    Optertitle = split[1];
                    Content1 = split[2];
                }
                else
                {
                    Require = split[1];
                    var name = from c in db.ChineseTableList
                               where c.chinese == Require
                               select c.tablename;
                    Require = name.First();
                    AndOr = split[0];

                    Optertitle = split[2];
                    Content1 = split[3];
                }
                //判断来自哪个表
                if (Require == "archivesTitle" || Require == "archivesNo")
                {

                    sql = "SELECT count(*) as count FROM UrbanCon.dbo.vw_YuanChuanprojectList  a where a.status='7' and a.paperProjectSeqNo in ( select paperProjectSeqNo from UrbanCon.dbo.YuanChuanArchivesDetail c where c.";
            
                }
               
               

                if (Optertitle == "包含")
                {
                    sql += Require + " like '%" + Content1 + "%'";

                }
                if (Optertitle == "模糊包含")
                {
                    sql += Require + " like '%";
                    for (int j = 0; j < Content1.Length; j++)
                    {
                        sql += Content1[j].ToString();
                        sql += "%";

                    }
                    sql += "'";

                }
                if (Optertitle == "等于")
                {

                    sql += Require + "=" + "'" + Content1 + "'";

                }
                if (Optertitle == "前方一致")
                {
                    sql += Require + " like " + "'" + Content1 + "%'";

                }
                if (Optertitle == "大于")
                {
                    if (Require == "paperProjectSeqNo")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索条件非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
                    }



                    sql += Require + ">" + Content1;

                }
                if (Optertitle == "小于")
                {
                    if (Require == "paperProjectSeqNo")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
                    }
                    sql += Require + "<" + Content1;

                }

                if (Optertitle == "为NULL")
                {
                    sql += Require + " is null";

                }
                if (Optertitle == "为空格")
                {
                    sql += Require + "=' '";

                }
                if (Optertitle == "在……之中")
                {
                    string[] strInfo = Content1.Split('-');
                    sql += Require + " in(";
                    string st = "";
                    foreach (string s in strInfo)
                    {
                        st += s + ",";
                    }
                    st = st.Substring(0, st.Length - 1);
                    sql = sql + st + ")";

                }
                if (Optertitle == "在……之间")
                {
                    string[] su = Content1.Split('-');
                    if (Require == "paperProjectSeqNo")
                    {
                        if ((Regex.IsMatch(su[0], "^([0-9]{1,})$") == true) && (Regex.IsMatch(su[1], "^([0-9]{1,})$") == true))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
                    }

                    sql += Require + " between " + su[0] + " and " + su[1];
                }
                if (Optertitle == "不等于")
                {

                    if (Require == "paperProjectSeqNo")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
                    }
                    sql += Require + "!=" + "'" + Content1 + "'";
                }

            }
            if (System.Text.RegularExpressions.Regex.Matches(sql, @"\(").Count != System.Text.RegularExpressions.Regex.Matches(sql, @"\)").Count)
            {
                sql += ")";
            }

            cmd.CommandText = sql;


            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            DataSet ds = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            try
            {
                adapter.Fill(ds);
                conn.Close();
            }
            catch (Exception ex)
            {
                return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
            }

            string count1 = ds.Tables[0].Rows[0]["count"].ToString(); ;
            ViewData["count"] = count1;
            ViewData["SerachText"] = SerachText.ToString();
            int cnt = Int32.Parse(count1) / 100 + 1;
            if (Int32.Parse(count1) % 100 == 0)
            {
                cnt = Int32.Parse(count1) / 100;
            }
            ViewData["totalpage"] = cnt;
            return View();

        }
        public string LookYuanChuanProjectData(string SerachText, int? page)
        {
            if (SerachText.IndexOf(',') == -1)
            {
                SerachText = SerachText + ",";
            }
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider=SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT paperProjectSeqNo,constructionOrganization,archivesCount,originalInchCount,projectName,location,developmentOrganization,jgDate,luruTime,textMaterial,startArchiveNo,endArchiveNo,projectID FROM UrbanCon.dbo.vw_YuanChuanprojectList  where status='7' and ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachText;

            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.ChineseTableList
                               where b.chinese == Require
                               select b.tablename;
                    Require = name.First();
                    Optertitle = split[1];
                    Content1 = split[2];
                }
                else
                {
                    Require = split[1];
                    var name = from c in db.ChineseTableList
                               where c.chinese == Require
                               select c.tablename;
                    Require = name.First();
                    AndOr = split[0];

                    Optertitle = split[2];
                    Content1 = split[3];
                }
                //判断来自哪个表
                if (Require == "archivesTitle" || Require == "archivesNo")
                {

                    sql = "SELECT  paperProjectSeqNo,constructionOrganization,archivesCount,originalInchCount,projectName,location,developmentOrganization,jgDate,luruTime,textMaterial,startArchiveNo,endArchiveNo,projectID FROM UrbanCon.dbo.vw_YuanChuanprojectList  a where a.status='7' and a.paperProjectSeqNo in ( select paperProjectSeqNo from UrbanCon.dbo.YuanChuanArchivesDetail c where c.";

                }



                if (Optertitle == "包含")
                {
                    sql += Require + " like '%" + Content1 + "%'";

                }
                if (Optertitle == "模糊包含")
                {
                    sql += Require + " like '%";
                    for (int j = 0; j < Content1.Length; j++)
                    {
                        sql += Content1[j].ToString();
                        sql += "%";

                    }
                    sql += "'";

                }
                if (Optertitle == "等于")
                {

                    sql += Require + "=" + "'" + Content1 + "'";

                }
                if (Optertitle == "前方一致")
                {
                    sql += Require + " like " + "'" + Content1 + "%'";

                }
                if (Optertitle == "大于")
                {
                   



                    sql += Require + ">" + Content1;

                }
                if (Optertitle == "小于")
                {
                    
                    sql += Require + "<" + Content1;

                }

                if (Optertitle == "为NULL")
                {
                    sql += Require + " is null";

                }
                if (Optertitle == "为空格")
                {
                    sql += Require + "=' '";

                }
                if (Optertitle == "在……之中")
                {
                    string[] strInfo = Content1.Split('-');
                    sql += Require + " in(";
                    string st = "";
                    foreach (string s in strInfo)
                    {
                        st += s + ",";
                    }
                    st = st.Substring(0, st.Length - 1);
                    sql = sql + st + ")";

                }
                if (Optertitle == "在……之间")
                {
                    string[] su = Content1.Split('-');
                   

                    sql += Require + " between " + su[0] + " and " + su[1];
                }
                if (Optertitle == "不等于")
                {

                   
                    sql += Require + "!=" + "'" + Content1 + "'";
                }

            }
            if (System.Text.RegularExpressions.Regex.Matches(sql, @"\(").Count != System.Text.RegularExpressions.Regex.Matches(sql, @"\)").Count)
            {
                sql += ")";
            }
            sql += " order by paperProjectSeqNo";
            cmd.CommandText = sql;


            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            DataSet ds = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

            adapter.Fill(ds);
            conn.Close();



            int count1 = ds.Tables[0].Rows.Count;




            int pageSize = 100;
            int pageNumber = (page ?? 1);
            int cnt = count1 / pageSize + 1;
            if (count1 % pageSize == 0)
            {
                cnt = count1 / pageSize;
            }
            var result1 = ds.Tables[0];/* Select("paperProjectSeqNo > =0");*/
            List<vw_YuanChuanprojectList> prolist = new List<vw_YuanChuanprojectList>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                vw_YuanChuanprojectList vw_project = new vw_YuanChuanprojectList();
                vw_project.paperProjectSeqNo = Convert.ToInt32(dr["paperProjectSeqNo"]);
                vw_project.archivesCount = dr["archivesCount"].ToString();

                if (dr["originalInchCount"] == DBNull.Value)
                {
                    vw_project.originalInchCount = 0;
                }
                else
                {
                    vw_project.originalInchCount = Convert.ToInt32(dr["originalInchCount"]);
                }

                vw_project.projectName = dr["projectName"].ToString();
                vw_project.location = dr["location"].ToString();
               
                vw_project.developmentOrganization = dr["developmentOrganization"].ToString(); ;
                vw_project.jgDate = Convert.ToDateTime(dr["jgDate"]);
                vw_project.luruTime = dr["luruTime"].ToString();
                if (dr["textMaterial"] == DBNull.Value)
                {
                    vw_project.textMaterial = 0;
                }
                else
                {
                    vw_project.textMaterial = Convert.ToInt32(dr["textMaterial"]);
                }
               
                vw_project.constructionOrganization = dr["constructionOrganization"].ToString();
                
                vw_project.startArchiveNo = dr["startArchiveNo"].ToString();
                vw_project.endArchiveNo = dr["endArchiveNo"].ToString();
                vw_project.projectID = Convert.ToInt32(dr["projectID"]);

                prolist.Add(vw_project);
            }
            var e = prolist.ToPagedList(pageNumber, pageSize);
            var d = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                         //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程

                         from p in e
                         select new JObject(
                        new JProperty("paperProjectSeqNo", p.paperProjectSeqNo),
                        new JProperty("archivesCount", p.archivesCount),
                        new JProperty("originalInchCount", p.originalInchCount),
                        new JProperty("projectName", p.projectName),
                        new JProperty("location", p.location),
                        new JProperty("licenseNo", p.licenseNo),
                        new JProperty("developmentOrganization", p.developmentOrganization),
                        new JProperty("jgDate", p.jgDate),
                        new JProperty("luruTime", p.luruTime),
                        new JProperty("textMaterial", p.textMaterial),
                       
                        new JProperty("constructionOrganization", p.constructionOrganization),
                     
                        new JProperty("startArchiveNo", p.startArchiveNo),
                        new JProperty("endArchiveNo", p.endArchiveNo),
                        new JProperty("projectID", p.projectID)
                                      )


               )
            )

).ToString();
            return d;

        }
        public ActionResult ProjectInfoes(long? id, string id2)
        {
            if (id == null || id == 0)
            {
                return Content("<script >alert('请在工程全部记录中选择一项工程');window.history.back();</script >");
            }

            var projectinfo = from b in db.vw_YuanChuanpassList
                              where b.projectID == id
                              orderby b.paperProjectSeqNo
                              select b;
            if (projectinfo.Count() == 0)
            {
                return Content("<script >alert('请在工程全部记录中选择一项工程');window.history.back();</script >");
            }
            vw_YuanChuanpassList pro = projectinfo.First();
            int wenziCnt = 0, tuzhiCnt = 0, photoCnt = 0;
            foreach (var item in projectinfo)
            {
                wenziCnt += Convert.ToInt32(item.textMaterial);
                tuzhiCnt += Convert.ToInt32(item.drawing);
                photoCnt += Convert.ToInt32(item.PhotoCount);
            }
            pro.textMaterial = wenziCnt;
            pro.drawing = tuzhiCnt;
            pro.PhotoCount = photoCnt;
            //编制分类号

            string strfenleihao = pro.mainCategoryID;
            if (pro.subDictionaryID != null)
            {
                if (pro.subDictionaryID.Trim() != "0")
                {


                    strfenleihao = strfenleihao + pro.subDictionaryID;
                    if (pro.mainCategoryID.Trim() != "0")
                    {
                        strfenleihao = strfenleihao + "." + pro.mainCategoryID;

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
            ViewData["url"] = id2;//传登url，用于工程著录单返回
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

              };
            if (pro.isYD == null || pro.isYD == false)
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 0);
            }
            else
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 1);
            }

            return View(pro);


        }
        public ActionResult Projectanjuan(long? id, string id2)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var paperno = from b in db.YuanChuanPaperArchives
                          where b.projectID == id
                          select b.paperProjectSeqNo;
            if (paperno.Count() == 0)
            {
                return Content("<script >alert('请在工程全部记录中选择一项工程');window.history.back();</script >");
            }
            long PaperSeqNo = paperno.First();

            var archive = from a in db.YuanChuanArchivesDetail
                          where a.paperProjectSeqNo == PaperSeqNo
                          orderby a.volNo
                          select a;

            if (archive == null)
            {
                return HttpNotFound();
            }
            //ViewData["url"] = id2;
            //ViewData["url1"] = Request.Url.ToString().Trim();
            ViewData["volCount"] = archive.Count();
            ViewBag.result = JsonConvert.SerializeObject(archive);
            return View();
        



        }
        public ActionResult anjuanzhuludan(string id3,/*string id4,*/ string flag)
        {
            if (id3 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id3 == "")
            {
                return Content("<script >alert('请在案卷全部记录中选择一项案卷');window.history.back();</script >");

                //return Content("<script >alert('请在案卷全部记录中选择一项案卷');</script >");

            }
            var projectinfo = from b in db.vw_YuanChuanpassList
                              where b.registrationNo == id3
                              select b;
            long paperseq = projectinfo.First().paperProjectSeqNo;
            if (projectinfo.Count() == 0)
            {
                return Content("<script >alert('请在案卷全部记录中选择一项案卷');window.history.back();</script >");
                //return Content("<script >alert('请在案卷全部记录中选择一项案卷');</script >");
            }
            string seq = paperseq.ToString().Trim();
            var project = from c in db.vw_YuanChuanpassList
                          where c.paperProjectSeqNo == paperseq
                          orderby c.volNo
                          select c;
            if (projectinfo.Count() == 0)
            {
                ViewData["checkname1"] = 3;

            }
            //设置上下卷的按钮可用性
            var firstRegis = from a in db.ArchivesDetail
                             where a.paperProjectSeqNo == paperseq
                             orderby a.volNo
                             select a.registrationNo;
            var lastRegis = from a in db.ArchivesDetail
                            where a.paperProjectSeqNo == paperseq
                            orderby a.volNo descending
                            select a.registrationNo;
            if (id3 == firstRegis.First())//已经达到第一卷
            {

                ViewData["first"] = true;
                if (id3 == lastRegis.First())
                {
                    ViewData["last"] = true;
                }
                else
                {
                    ViewData["last"] = false;
                }
            }
            else {
                ViewData["first"] = false;
                if (id3 == lastRegis.First())
                {
                   ViewData["last"] = true;
                }
                else
                {
                   ViewData["last"] = false;
                }
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

            //ViewData["id4"] = id4;
            ViewData["checkname1"] = projectinfo.First().registrationNo;
            //if (projectinfo.First().isImageExist == "无")
            //{
            //    ViewData["isImageExist"] = true;
            //    ViewData["Message"] = "该案卷无扫描件";
            //}
            return View(projectinfo.First());

        }
        [HttpPost]
        public ActionResult anjuanzhuludan(string action, string registrationNo, string paperProjectSeqNo)
        {
            if (registrationNo == "" || paperProjectSeqNo == "")
            {
                return Content("<script >alert('录入数据有误！');window.history.back();</script >");
            }
            long paper = long.Parse(paperProjectSeqNo.Trim());
            string regisNo = "";
            var firstRegis = from a in db.YuanChuanArchivesDetail
                             where a.paperProjectSeqNo == paper
                             orderby a.volNo
                             select a.registrationNo;
            if (action == "上一卷")
            {
                string regis = registrationNo;
                long re = Int32.Parse(regis);
               
                re = re - 1;
                regisNo = "";
                regisNo = re.ToString();
                

                
            }
          
            if (action == "下一卷")
            {
                string regis = registrationNo;
                long re = Int32.Parse(regis);
              
                re = re + 1;
                regisNo = "";
                regisNo = re.ToString();
               

            }

            return RedirectToAction("anjuanzhuludan", new { id3 = regisNo });

        }
        public ActionResult Juanneimulu(string id1, string url1)
        {
            if (id1 == "" || id1 == null)
            {
                return Content("<script >alert('请在案卷全部记录中选择一项案卷');window.history.back();</script >");
            }
            var archNo = from b in db.YuanChuanArchivesDetail
                         where b.registrationNo == id1
                         select b.archivesNo;
            string archiveN = archNo.First().Trim();
            var file = from a in db.YuanChuanFileInfo
                       where a.archivesNo == archiveN
                       orderby a.seqNo
                       select a;
            ViewData["url1"] = url1;
            if (file.Count() == 0)
            {
                return Content("<script >alert('该案卷无卷内目录信息！');window.history.back();</script >");

            }
            ViewData["volCount"] = file.Count();
            return View(file.ToList());

        }
        public ActionResult Juanneimuluxinxi(string id)
        {
            if (id == "" || id == null)
            {
                return Content("<script >alert('请选择一个案卷文件！');window.history.back();</script >");
            }
            if (id.IndexOf(',') == -1)
            {
                return Content("<script >alert('请选择一个案卷文件！');window.history.back();</script >");
            }
            string[] tem = id.Split(',');
            if (tem[0] == "" || tem[1] == "")
            {
                return Content("<script >alert('请选择一个案卷文件！');window.history.back();</script >");
            }
            int ID = Int32.Parse(tem[0]);
            string ArchiveNo = tem[1];
            var file = from ad in db.YuanChuanFileInfo
                       where ad.archivesNo == ArchiveNo && ad.seqNo == ID
                       select ad;

            if (file.Count() == 0)
            {
                return Content("<script >alert('该案卷文件没有卷内目录信息！');window.history.back();</script >");
            }
            Models.YuanChuanFileInfo fileinfo = file.First();
            return View(fileinfo);
        }
        public ActionResult LookYuanChuanArchives(string SerachText)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider=SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count FROM UrbanCon.dbo.vw_YuanChuanpassList  where status='7' and ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachText;

            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.ChineseTableList
                               where b.chinese == Require
                               select b.tablename;
                    Require = name.First();
                    Optertitle = split[1];
                    Content1 = split[2];
                }
                else
                {
                    Require = split[1];
                    var name = from c in db.ChineseTableList
                               where c.chinese == Require
                               select c.tablename;
                    Require = name.First();
                    AndOr = split[0];

                    Optertitle = split[2];
                    Content1 = split[3];
                }
              


                if (Optertitle == "包含")
                {
                    sql += Require + " like '%" + Content1 + "%'";

                }
                if (Optertitle == "模糊包含")
                {
                    sql += Require + " like '%";
                    for (int j = 0; j < Content1.Length; j++)
                    {
                        sql += Content1[j].ToString();
                        sql += "%";

                    }
                    sql += "'";

                }
                if (Optertitle == "等于")
                {

                    sql += Require + "=" + "'" + Content1 + "'";

                }
                if (Optertitle == "前方一致")
                {
                    sql += Require + " like " + "'" + Content1 + "%'";

                }
                if (Optertitle == "大于")
                {
                    if (Require == "paperProjectSeqNo")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索条件非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
                    }



                    sql += Require + ">" + Content1;

                }
                if (Optertitle == "小于")
                {
                    if (Require == "paperProjectSeqNo")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
                    }
                    sql += Require + "<" + Content1;

                }

                if (Optertitle == "为NULL")
                {
                    sql += Require + " is null";

                }
                if (Optertitle == "为空格")
                {
                    sql += Require + "=' '";

                }
                if (Optertitle == "在……之中")
                {
                    string[] strInfo = Content1.Split('-');
                    sql += Require + " in(";
                    string st = "";
                    foreach (string s in strInfo)
                    {
                        st += s + ",";
                    }
                    st = st.Substring(0, st.Length - 1);
                    sql = sql + st + ")";

                }
                if (Optertitle == "在……之间")
                {
                    string[] su = Content1.Split('-');
                    if (Require == "paperProjectSeqNo")
                    {
                        if ((Regex.IsMatch(su[0], "^([0-9]{1,})$") == true) && (Regex.IsMatch(su[1], "^([0-9]{1,})$") == true))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
                    }

                    sql += Require + " between " + su[0] + " and " + su[1];
                }
                if (Optertitle == "不等于")
                {

                    if (Require == "paperProjectSeqNo")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
                    }
                    sql += Require + "!=" + "'" + Content1 + "'";
                }

            }
            if (System.Text.RegularExpressions.Regex.Matches(sql, @"\(").Count != System.Text.RegularExpressions.Regex.Matches(sql, @"\)").Count)
            {
                sql += ")";
            }

            cmd.CommandText = sql;


            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            DataSet ds = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            //try
            //{
                adapter.Fill(ds);
                conn.Close();
            //}
            //catch (Exception ex)
            //{
                //return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
            //}

            string count1 = ds.Tables[0].Rows[0]["count"].ToString();
            ViewData["count"] = count1;
            ViewData["SerachText"] = SerachText.ToString();
            int cnt = Int32.Parse(count1) / 100 + 1;
            if (Int32.Parse(count1) % 100 == 0)
            {
                cnt = Int32.Parse(count1) / 100;
            }
            ViewData["totalpage"] = cnt;
            return View();
        }
        public string LookYuanChuanArchivesData(string SerachText, int? page)
        {
            if (SerachText.IndexOf(',') == -1)
            {
                SerachText = SerachText + ",";
            }
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider=SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT paperProjectSeqNo,volNo,constructionOrganization,archiveThickness,archivesNo,registrationNo,archivesTitle,developmentOrganization,jgDate,luruTime,textMaterial,drawing,photoCount FROM UrbanCon.dbo.vw_YuanChuanpassList   where status='7' and ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachText;

            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.ChineseTableList
                               where b.chinese == Require
                               select b.tablename;
                    Require = name.First();
                    Optertitle = split[1];
                    Content1 = split[2];
                }
                else
                {
                    Require = split[1];
                    var name = from c in db.ChineseTableList
                               where c.chinese == Require
                               select c.tablename;
                    Require = name.First();
                    AndOr = split[0];

                    Optertitle = split[2];
                    Content1 = split[3];
                }
               



                if (Optertitle == "包含")
                {
                    sql += Require + " like '%" + Content1 + "%'";

                }
                if (Optertitle == "模糊包含")
                {
                    sql += Require + " like '%";
                    for (int j = 0; j < Content1.Length; j++)
                    {
                        sql += Content1[j].ToString();
                        sql += "%";

                    }
                    sql += "'";

                }
                if (Optertitle == "等于")
                {

                    sql += Require + "=" + "'" + Content1 + "'";

                }
                if (Optertitle == "前方一致")
                {
                    sql += Require + " like " + "'" + Content1 + "%'";

                }
                if (Optertitle == "大于")
                {




                    sql += Require + ">" + Content1;

                }
                if (Optertitle == "小于")
                {

                    sql += Require + "<" + Content1;

                }

                if (Optertitle == "为NULL")
                {
                    sql += Require + " is null";

                }
                if (Optertitle == "为空格")
                {
                    sql += Require + "=' '";

                }
                if (Optertitle == "在……之中")
                {
                    string[] strInfo = Content1.Split('-');
                    sql += Require + " in(";
                    string st = "";
                    foreach (string s in strInfo)
                    {
                        st += s + ",";
                    }
                    st = st.Substring(0, st.Length - 1);
                    sql = sql + st + ")";

                }
                if (Optertitle == "在……之间")
                {
                    string[] su = Content1.Split('-');


                    sql += Require + " between " + su[0] + " and " + su[1];
                }
                if (Optertitle == "不等于")
                {


                    sql += Require + "!=" + "'" + Content1 + "'";
                }

            }
            if (System.Text.RegularExpressions.Regex.Matches(sql, @"\(").Count != System.Text.RegularExpressions.Regex.Matches(sql, @"\)").Count)
            {
                sql += ")";
            }
            sql += " order by paperProjectSeqNo";
            cmd.CommandText = sql;


            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            DataSet ds = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

            adapter.Fill(ds);
            conn.Close();



            int count1 = ds.Tables[0].Rows.Count;




            int pageSize = 100;
            int pageNumber = (page ?? 1);
            int cnt = count1 / pageSize + 1;
            if (count1 % pageSize == 0)
            {
                cnt = count1 / pageSize;
            }
            var result1 = ds.Tables[0];/* Select("paperProjectSeqNo > =0");*/
            List<vw_YuanChuanpassList> prolist = new List<vw_YuanChuanpassList>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                vw_YuanChuanpassList vw_project = new vw_YuanChuanpassList();
                vw_project.paperProjectSeqNo = Convert.ToInt32(dr["paperProjectSeqNo"]);
               
                vw_project.archivesNo = dr["archivesNo"].ToString();
                if (dr["archiveThickness"] == DBNull.Value)
                {
                    vw_project.archiveThickness = 0;
                }
                else
                {
                    vw_project.archiveThickness = Convert.ToInt32(dr["archiveThickness"]);
                }
                if (dr["volNo"] == DBNull.Value)
                {
                    vw_project.volNo = 0;
                }
                else
                {
                    vw_project.volNo = Convert.ToInt32(dr["volNo"]);
                }

                vw_project.archivesTitle = dr["archivesTitle"].ToString();
                vw_project.registrationNo= dr["registrationNo"].ToString();

                vw_project.developmentOrganization = dr["developmentOrganization"].ToString(); ;
                vw_project.jgDate = Convert.ToDateTime(dr["jgDate"]);
                vw_project.luruTime = dr["luruTime"].ToString();
                if (dr["textMaterial"] == DBNull.Value)
                {
                    vw_project.textMaterial = 0;
                }
                else
                {
                    vw_project.textMaterial = Convert.ToInt32(dr["textMaterial"]);
                }

                vw_project.constructionOrganization = dr["constructionOrganization"].ToString();
                if (dr["textMaterial"] == DBNull.Value)
                {
                    vw_project.textMaterial = 0;
                }
                else
                {
                    vw_project.textMaterial = Convert.ToInt32(dr["textMaterial"]);
                }
                if (dr["drawing"] == DBNull.Value)
                {
                    vw_project.drawing = 0;
                }
                else
                {
                    vw_project.drawing = Convert.ToInt32(dr["drawing"]);
                }
                if (dr["PhotoCount"] == DBNull.Value)
                {
                    vw_project.PhotoCount = 0;
                }
                else
                {
                    vw_project.PhotoCount = Convert.ToInt32(dr["PhotoCount"]);
                }
               

                prolist.Add(vw_project);
            }
            var e = prolist.ToPagedList(pageNumber, pageSize);
            var d = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                         //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程

                         from p in e
                         select new JObject(
                        new JProperty("paperProjectSeqNo", p.paperProjectSeqNo),
                        new JProperty("volNo", p.volNo),
                        new JProperty("archiveThickness", p.archiveThickness),
                        new JProperty("archivesTitle", p.archivesTitle),
                        new JProperty("archivesNo", p.archivesNo),
                        new JProperty("registrationNo", p.registrationNo),
                        new JProperty("developmentOrganization", p.developmentOrganization),
                        new JProperty("jgDate", p.jgDate),
                        new JProperty("luruTime", p.luruTime),
                

                        new JProperty("constructionOrganization", p.constructionOrganization),

                        new JProperty("textMaterial", p.textMaterial),
                        new JProperty("drawing", p.drawing),
                        new JProperty("photoCount", p.PhotoCount)
                                      )


               )
            )

).ToString();
            return d;

        }
        public ActionResult Allprojectjilu(string registrationNo)
        {

            var paperno = from b in db.YuanChuanArchivesDetail
                          where b.registrationNo == registrationNo
                          select b.paperProjectSeqNo;
            if (paperno.Count() == 0)
            {
                return Content("<script >alert('请在案卷全部记录中选择一卷案卷！');window.history.back();</script >");

            }
            long PaperSeqNo = paperno.First();

            var project = from a in db.vw_YuanChuanprojectList
                          where a.paperProjectSeqNo == PaperSeqNo
                          select a;

            if (project.Count() == 0)
            {
                return Content("<script >alert('该案卷无工程信息！');window.history.back();</script >");
            }
            ViewData["volCount"] = project.Count();
            return View(project);

        }
        public ActionResult LookZhengDiArchives(string SerachText)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider=SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count FROM UrbanCon.dbo.zdArchive  where  ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachText;

            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.zdArchiveAttribute
                               where b.Description == Require
                               select b.colName;
                    Require = name.First();
                    Optertitle = split[1];
                    Content1 = split[2];
                }
                else
                {
                    Require = split[1];
                    var name = from c in db.zdArchiveAttribute
                               where c.Description == Require
                               select c.colName;
                    Require = name.First();
                    AndOr = split[0];

                    Optertitle = split[2];
                    Content1 = split[3];
                }



                if (Optertitle == "包含")
                {
                    sql += Require + " like '%" + Content1 + "%'";

                }
                if (Optertitle == "模糊包含")
                {
                    sql += Require + " like '%";
                    for (int j = 0; j < Content1.Length; j++)
                    {
                        sql += Content1[j].ToString();
                        sql += "%";

                    }
                    sql += "'";

                }
                if (Optertitle == "等于")
                {

                    sql += Require + "=" + "'" + Content1 + "'";

                }
                if (Optertitle == "前方一致")
                {
                    sql += Require + " like " + "'" + Content1 + "%'";

                }
                if (Optertitle == "大于")
                {
                    if (Require == "hbAreaMu"|| Require == "hbAreaKM")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索条件非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
                    }



                    sql += Require + ">" + Content1;

                }
                if (Optertitle == "小于")
                {
                    if (Require == "hbAreaMu" || Require == "hbAreaKM")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
                    }
                    sql += Require + "<" + Content1;

                }

                if (Optertitle == "为NULL")
                {
                    sql += Require + " is null";

                }
                if (Optertitle == "为空格")
                {
                    sql += Require + "=' '";

                }
                if (Optertitle == "在……之中")
                {
                    string[] strInfo = Content1.Split('-');
                    sql += Require + " in(";
                    string st = "";
                    foreach (string s in strInfo)
                    {
                        st += s + ",";
                    }
                    st = st.Substring(0, st.Length - 1);
                    sql = sql + st + ")";

                }
                if (Optertitle == "在……之间")
                {
                    string[] su = Content1.Split('-');
                    if (Require == "hbAreaMu" || Require == "hbAreaKM")
                    {
                        if ((Regex.IsMatch(su[0], "^([0-9]{1,})$") == true) && (Regex.IsMatch(su[1], "^([0-9]{1,})$") == true))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
                    }

                    sql += Require + " between " + su[0] + " and " + su[1];
                }
                if (Optertitle == "不等于")
                {

                    if (Require == "hbAreaMu" || Require == "hbAreaKM")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
                    }
                    sql += Require + "!=" + "'" + Content1 + "'";
                }

            }
            if (System.Text.RegularExpressions.Regex.Matches(sql, @"\(").Count != System.Text.RegularExpressions.Regex.Matches(sql, @"\)").Count)
            {
                sql += ")";
            }

            cmd.CommandText = sql;


            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            DataSet ds = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            try
            {
                adapter.Fill(ds);
                conn.Close();
            }
            catch (Exception ex)
            {
                return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
            }

            string count1 = ds.Tables[0].Rows[0]["count"].ToString(); ;
            ViewData["count"] = count1;
            ViewData["SerachText"] = SerachText.ToString();
            int cnt = Int32.Parse(count1) / 100 + 1;
            if (Int32.Parse(count1) % 100 == 0)
            {
                cnt = Int32.Parse(count1) / 100;
            }
            ViewData["totalpage"] = cnt;
            return View();
        }
        public string LookZhengDiArchivesData(string SerachText, int? page)
        {
            if (SerachText.IndexOf(',') == -1)
            {
                SerachText = SerachText + ",";
            }
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider=SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT regisNo,archiveNo,isImageExist,archiveTitle,bdzh,zdwh,hbLocation,firstResponsible FROM UrbanCon.dbo.zdArchive  where   ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachText;

            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.zdArchiveAttribute
                               where b.Description == Require
                               select b.colName;
                    Require = name.First();
                    Optertitle = split[1];
                    Content1 = split[2];
                }
                else
                {
                    Require = split[1];
                    var name = from c in db.zdArchiveAttribute
                               where c.Description == Require
                               select c.colName;
                    Require = name.First();
                    AndOr = split[0];

                    Optertitle = split[2];
                    Content1 = split[3];
                }




                if (Optertitle == "包含")
                {
                    sql += Require + " like '%" + Content1 + "%'";

                }
                if (Optertitle == "模糊包含")
                {
                    sql += Require + " like '%";
                    for (int j = 0; j < Content1.Length; j++)
                    {
                        sql += Content1[j].ToString();
                        sql += "%";

                    }
                    sql += "'";

                }
                if (Optertitle == "等于")
                {

                    sql += Require + "=" + "'" + Content1 + "'";

                }
                if (Optertitle == "前方一致")
                {
                    sql += Require + " like " + "'" + Content1 + "%'";

                }
                if (Optertitle == "大于")
                {




                    sql += Require + ">" + Content1;

                }
                if (Optertitle == "小于")
                {

                    sql += Require + "<" + Content1;

                }

                if (Optertitle == "为NULL")
                {
                    sql += Require + " is null";

                }
                if (Optertitle == "为空格")
                {
                    sql += Require + "=' '";

                }
                if (Optertitle == "在……之中")
                {
                    string[] strInfo = Content1.Split('-');
                    sql += Require + " in(";
                    string st = "";
                    foreach (string s in strInfo)
                    {
                        st += s + ",";
                    }
                    st = st.Substring(0, st.Length - 1);
                    sql = sql + st + ")";

                }
                if (Optertitle == "在……之间")
                {
                    string[] su = Content1.Split('-');


                    sql += Require + " between " + su[0] + " and " + su[1];
                }
                if (Optertitle == "不等于")
                {


                    sql += Require + "!=" + "'" + Content1 + "'";
                }

            }
            if (System.Text.RegularExpressions.Regex.Matches(sql, @"\(").Count != System.Text.RegularExpressions.Regex.Matches(sql, @"\)").Count)
            {
                sql += ")";
            }
            sql += " order by regisNo,archiveNo";
            cmd.CommandText = sql;


            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            DataSet ds = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

            adapter.Fill(ds);
            conn.Close();



            int count1 = ds.Tables[0].Rows.Count;




            int pageSize = 100;
            int pageNumber = (page ?? 1);
            int cnt = count1 / pageSize + 1;
            if (count1 % pageSize == 0)
            {
                cnt = count1 / pageSize;
            }
            var result1 = ds.Tables[0];/* Select("paperProjectSeqNo > =0");*/
            List<zdArchive> prolist = new List<zdArchive>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                zdArchive vw_project = new zdArchive();
                //regisNo,archiveNo,isImageExist,archiveTitle,bdzh,zdwh,hbLocation,firstResponsible
                vw_project.regisNo = dr["regisNo"].ToString();

                vw_project.archiveNo = dr["archiveNo"].ToString();
                if (dr["isImageExist"] == DBNull.Value|| dr["isImageExist"].ToString()=="0"|| dr["isImageExist"].ToString()=="无")
                {
                    vw_project.isImageExist="无";
                }
                else
                {
                    vw_project.isImageExist = "有";
                }
               

                vw_project.archiveTitle = dr["archiveTitle"].ToString();
                vw_project.bdzh = dr["bdzh"].ToString();

                vw_project.zdwh =dr["zdwh"].ToString(); ;
                vw_project.hbLocation = dr["hbLocation"].ToString();
                vw_project.firstResponsible = dr["firstResponsible"].ToString();
               

                prolist.Add(vw_project);
            }
            var e = prolist.ToPagedList(pageNumber, pageSize);
            var d = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                         //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程

                         from p in e
                         select new JObject(
                        new JProperty("regisNo", p.regisNo),
                        new JProperty("archiveNo", p.archiveNo),
                        new JProperty("isImageExist", p.isImageExist),
                        new JProperty("archiveTitle", p.archiveTitle),
                        
                        new JProperty("bdzh", p.bdzh),
                        new JProperty("zdwh", p.zdwh),
                        new JProperty("hbLocation", p.hbLocation),
                        new JProperty("firstResponsible", p.firstResponsible)


                     
                                      )


               )
            )

).ToString();
            return d;

        }
        public ActionResult ZdArchivesDetail(string id)
        {
            if (id == null)
            {
                return Content("<script >alert('请在工程全部记录中选择一项工程');window.history.back();</script >");
            }

            var projectinfo = from b in db.zdArchive
                              where b.regisNo== id
                              orderby b.regisNo
                              select b;
            if (projectinfo.Count() == 0)
            {
                return Content("<script >alert('请在工程全部记录中选择一项工程');window.history.back();</script >");
            }
           
           
           
         
            if(projectinfo.First().jgDate.ToString()!=null&& projectinfo.First().jgDate.ToString()!="")

            {
                string jgdate = projectinfo.First().jgDate.Value.ToString("yyyy-MM-dd");
                ViewData["jgDate"] = jgdate;

            }
           
         
            
   
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

              };
            if (projectinfo.First().isYD == null || projectinfo.First().isYD == false)
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 0);
            }
            else
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 1);
            }
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", projectinfo.First().retentionN);
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", projectinfo.First().securityID);
            return View(projectinfo.First());

       
        }

        public ActionResult LookYuanchuansaomiao(string paperProjectSeqNo, string ArchiveNo, string regNo, string volNo)
        {
            if (regNo != "")
            {
                var archiveM = from a in db.YuanChuanArchivesDetail
                               where a.registrationNo == regNo
                               select a;
                if (archiveM.Count() == 0)
                {
                    return View();
                }
                else
                {
                    long paper = long.Parse(paperProjectSeqNo.Trim());
                    var paperM = from b in db.YuanChuanPaperArchives
                                 where b.paperProjectSeqNo == paper
                                 select b;
                    if (paperM.Count() == 0)
                    {
                        return View();
                    }
                    else
                    {
                        while (paperProjectSeqNo.Length < 5)
                        {
                            paperProjectSeqNo = "0" + paperProjectSeqNo;
                        }

                        ViewData["proNo"] = paperProjectSeqNo;
                        ViewData["VolNo"] = archiveM.First().volNo.ToString();
                        ViewData["VolCnt"] = paperM.First().archivesCount.ToString();

                    }
                }
            }

            return View();
        }
        public string GetYuanChuanPicListDemo(string paperSeqNo, string archiveNo, string volNo, string data)
        {
            DataTable myTable = null;
            string strPath = string.Empty;
            string strWebPath = string.Empty;

            //strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + paperSeqNo + "\\" + archiveNo + "\\";
            //string strPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + paperSeqNo + "\\" + volNo + "\\";
            //strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongWebPath"] + paperSeqNo + "/" + archiveNo + "/";
            //string strWebPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongWebPath"] + paperSeqNo + "/" + volNo + "/";

            //strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + "00003" + "\\" + "G3-00005" + "\\";
            //strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongWebPath"] + "00003" + "/" + "G3-00005" + "/";
            //D:\JunGongArchives\00001\F5.1-0002
            if (paperSeqNo != null && (paperSeqNo != "") && (archiveNo != null) && (archiveNo != string.Empty) && !string.IsNullOrEmpty(volNo))
            {
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["YuanChuanPath"] + paperSeqNo + "\\" + archiveNo + "\\";
                string strPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["YuanChuanPath"] + paperSeqNo + "\\" + volNo + "\\";
                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["YuanChuanWebPath"] + paperSeqNo + "/" + archiveNo + "/";
                string strWebPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["YuanChuanWebPath"] + paperSeqNo + "/" + volNo + "/";
                string strPath3 = System.Web.Configuration.WebConfigurationManager.AppSettings["YuanChuanPath1"] + paperSeqNo + "\\" + volNo + "\\";
                string strWebPath3 = System.Web.Configuration.WebConfigurationManager.AppSettings["YuanChuanWebPath1"] + paperSeqNo + "/" + volNo + "/";
                System.IO.DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();
                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));


                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();

                    if (arrFileInfo.Length > 0)
                    {
                        foreach (System.IO.FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath + myFile.Name;

                            myTable.Rows.Add(myDataRow);
                        }
                    }
                }
                else if (Directory.Exists(strPath2))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath2);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();

                    if (arrFileInfo.Length > 0)
                    {
                        foreach (System.IO.FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath2 + myFile.Name;

                            myTable.Rows.Add(myDataRow);
                        }
                    }
                }
                else if (Directory.Exists(strPath3))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath3);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();

                    if (arrFileInfo.Length > 0)
                    {
                        foreach (System.IO.FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath3 + myFile.Name;

                            myTable.Rows.Add(myDataRow);
                        }
                    }
                }
            }

            return JsonConvert.SerializeObject(myTable);
        }
        public ActionResult LookZhengDisaomiao( string regNo)
        { 

            return View();
        }
        public string GetZhengDiPicListDemo(string strPath)
        {
            DataTable myTable = null;

            string strWebPath = string.Empty;
            //HttpContext.Current.Session["paperProjectSeqNo"] = "19";
            if (strPath != null && strPath != string.Empty)
            {
                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ZhengDiWebPath"] + strPath + "/";
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ZhengDiPath"] + strPath + "\\";
                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));

                if (Directory.Exists(strPath))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(strPath);

                    //获得文件信息
                    arrFileInfo = myDirInfo.GetFiles();
                    //排序
                    Array.Sort(arrFileInfo, new FileSorter());
                    if (arrFileInfo.Length > 0)
                    {
                        foreach (System.IO.FileInfo myFile in arrFileInfo)
                        {
                            myDataRow = myTable.NewRow();

                            myDataRow["Name"] = myFile.Name;

                            myDataRow["WebPath"] = strWebPath + myFile.Name;

                            myTable.Rows.Add(myDataRow);
                        }
                    }
                }
            }

            return JsonConvert.SerializeObject(myTable);
        }
    }
}