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
    public class ArchiveSearch4Controller : Controller
    {
        // GET: ArchiveSearch4
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        private PlanArchiveEntities db_plan = new PlanArchiveEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LookYeWuProject(string SerachText/*, string classNo2*/)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户,进行内外部判断
            var user = ab.AspNetUsers.Find(UserID);
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count From UrbanCon.dbo.businessPlanProject  where status='RK        ' and  ";
            if (user.RoleName.Trim() == "借阅用户")
            {
                sql = "SELECT count(*) as count From UrbanCon.dbo.businessPlanProject where isNeibu!=" + "'内部'" + " and status='RK' and  ";
            }
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
                    var name = from b in db.PlanArchiveAttribute
                               where b.chinese == Require
                               select b.tablename;
                    Require = name.First();
                    Optertitle = split[1];
                    Content1 = split[2];
                }
                else
                {
                    Require = split[1];
                    var name = from c in db.PlanArchiveAttribute
                               where c.chinese == Require
                               select c.tablename;
                    Require = name.First();
                    AndOr = split[0];

                    Optertitle = split[2];
                    Content1 = split[3];
                }

                if (AndOr == "或者")
                {

                    sql += " or ";

                }
                if (AndOr == "而且")
                {
                    sql += " and ";
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
                    if (Require == "fileNo" || Require == "seqNo" || Require == "paijiaNo" || Require == "boxNo" || Require == "regisNo" || Require == "yearNo" || Require == "totalSeqNo")
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
                    if (Require == "fileNo" || Require == "seqNo" || Require == "paijiaNo" || Require == "boxNo" || Require == "regisNo" || Require == "yearNo" || Require == "totalSeqNo")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法,请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索条件非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
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
                    if (Require == "fileNo" || Require == "seqNo" || Require == "paijiaNo" || Require == "boxNo" || Require == "regisNo" || Require == "yearNo" || Require == "totalSeqNo")
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
                        return Content("<script >alert('检索条件非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
                    }

                    sql += Require + " between " + su[0] + " and " + su[1];
                }
                if (Optertitle == "不等于")
                {

                    if (Require == "fileNo" || Require == "seqNo" || Require == "paijiaNo" || Require == "boxNo" || Require == "regisNo" || Require == "yearNo" || Require == "totalSeqNo")
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
            //}
            //catch (Exception ex)
            //{
            //    return Content("<script >alert('检索格式不正确，请重新输入！');window.history.back();</script >");
            //}
            DataTable dt = ds.Tables[0];

            string count1 = dt.Rows[0]["count"].ToString();
            ViewData["count"] = count1;
            //取得数据前2500条
            sql = sql.Replace("count(*) as count", "top 2500 classifyID,fileNo,yearNo,isImageExist,totalSeqNo,projectContent,projectLocation,archiveTitle,developmentUnit,boxNo,seqNo1,ID,isNeibu");
            //sql += " order by yearNo,totalSeqNo";
            sql += " order by totalSeqNo";
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            DataSet ds1 = new DataSet();
            OleDbDataAdapter adapter1 = new OleDbDataAdapter(cmd);
            adapter1.Fill(ds1);
            DataTable dt1 = ds1.Tables[0];
            conn.Close();

            ViewBag.result1 = JsonConvert.SerializeObject(dt1);//数据转换成JSON后传给前台
            int cnt = Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) / 100 + 1;
            if (Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) % 100 == 0)
            {
                cnt = Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) / 100;
            }
            ViewData["totalpage"] = cnt;
            return View();
        }

        public ActionResult PlanArchivesInfoes(string id)
        {
            if (id == "" || id == null)
            {
                return Content("<script >alert('请选择一项记录');window.history.back();</script >");
            }
            var UserID = User.Identity.GetUserId();//获取当前用户,进行内外部判断
            var user = ab.AspNetUsers.Find(UserID);
            if (user.RoleName.Trim() == "借阅用户")
            {
                ViewBag.Niebu = "none";
                ViewData["viewflag"] = 0;
            }
            int ID = int.Parse(id.Trim());
            var Planoroject = from a in db_plan.businessPlanProject
                              where a.ID == ID
                              select a;
            //int? classID = Planoroject.First().classifyID;
            //var className = from b in ae.PlanArchiveClassify
            //                where b.classifyID == classID
            //                select b.classifyName;
            //Planoroject.First().storeLocation = className.First();

            if (Planoroject.Count() == 0)
            {
                return Content("<script >alert('该工程没有工程著录单');window.history.back();</script >");
            }

            return View(Planoroject.First());
        }

    }
}