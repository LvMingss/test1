
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
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
namespace urban_archive.Controllers
{
    public class ArchiveSearch2Controller : Controller
    {
        // GET: ArchiveSearch2
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        private VideoArchiveEntities ac = new VideoArchiveEntities();
        private PlanArchiveEntities ae = new PlanArchiveEntities();
        private VideoArchiveEntities db_video = new VideoArchiveEntities();
        private UrbanUsersEntities db_user = new UrbanUsersEntities();
        private VideoArchiveEntities dg = new VideoArchiveEntities();
        private gxArchivesContainer cb = new gxArchivesContainer();

        public ActionResult LookQingArchives_Zhizhao(string SerachTextZhiZhao)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
           

            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source="+ System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count From UrbanCon.dbo.OtherArchives where classTypeID=1 and status='RK' and ";
            if(user.UserName == "借阅用户"|| user.UserName == "管理科借阅")
            {
                sql = "SELECT count(*) as count From UrbanCon.dbo.OtherArchives where classTypeID=1 and isNeibu !=1 and status='RK' and ";
            }
            string[] str = SerachTextZhiZhao.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachTextZhiZhao;
          
            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.OtherArchiveAttribute
                               where b.Description == Require
                               select b.colName;
                    Require = name.First().Trim();
                    Optertitle = split[1];
                    Content1 = split[2];
                }
                else
                {
                    Require = split[1];
                    var name = from c in db.OtherArchiveAttribute
                               where c.Description == Require
                               select c.colName;
                    Require = name.First().Trim();
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
                    if (Require == "registrationNo" || Require == "paijiaNo")
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
                    if (Require == "registrationNo" || Require == "paijiaNo")
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
                    if (Require == "registrationNo" || Require == "paijiaNo")
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

                    if (Require == "registrationNo" || Require == "paijiaNo")
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


            //查询数据条数
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
                return Content("<script >alert('检索格式不正确，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
            }

            ViewData["count"] = ds.Tables[0].Rows[0]["count"].ToString();
            ViewData["SerachText"] = SerachTextZhiZhao.ToString();
            int cnt = Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) / 100 + 1;
            if (Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) % 100 == 0)
            {
                cnt = Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) / 100;
            }
            ViewData["totalpage"] = cnt;
            return View();
        }
        public string LookQingArchives_ZhizhaoData(string SerachText, int? page)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);


            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT ID,licenceNo,isImageExist,applyUnit,projectRange,location,year,areaProSeqNo,cunfangLocation,tranferUnit,urbanID,registrationNo,isNeibu From UrbanCon.dbo.OtherArchives where classTypeID=1 and status='RK' and ";
            if (user.UserName == "借阅用户" || user.UserName == "管理科借阅")
            {
                sql = "SELECT ID,licenceNo,isImageExist,applyUnit,projectRange,location,year,areaProSeqNo,cunfangLocation,tranferUnit,urbanID,registrationNo,isNeibu  From UrbanCon.dbo.OtherArchives where classTypeID=1 and isNeibu !=1 and status='RK' and ";
            }
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;

            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.OtherArchiveAttribute
                               where b.Description == Require
                               select b.colName;
                    Require = name.First().Trim();
                    Optertitle = split[1];
                    Content1 = split[2];
                }
                else
                {
                    Require = split[1];
                    var name = from c in db.OtherArchiveAttribute
                               where c.Description == Require
                               select c.colName;
                    Require = name.First().Trim();
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

            sql += " order by licenceNo";
            //查询数据条数
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
            var result1 = ds.Tables[0];
            List<OtherArchives> prolist = new List<OtherArchives>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OtherArchives vw_project = new OtherArchives();
                vw_project.ID = Convert.ToInt32(dr["ID"]);
                vw_project.licenceNo = dr["licenceNo"].ToString();
                vw_project.registrationNo = dr["registrationNo"].ToString();
                vw_project.isImageExist = dr["isImageExist"].ToString();
                vw_project.applyUnit = dr["applyUnit"].ToString();
                vw_project.projectRange = dr["projectRange"].ToString();
                vw_project.location = dr["location"].ToString();
                vw_project.year = dr["year"].ToString();
                vw_project.areaProSeqNo = dr["areaProSeqNo"].ToString();
                vw_project.cunfangLocation = dr["cunfangLocation"].ToString();
                vw_project.tranferUnit = dr["tranferUnit"].ToString();
                vw_project.urbanID = dr["urbanID"].ToString();

                vw_project.isNeibu = dr["isNeibu"].ToString(); ;
                prolist.Add(vw_project);
            }
            int a = -1;
            for (int m = 0; m < prolist.Count(); m++)
            {
                if (prolist[m].licenceNo.Contains("东"))
                {
                    a = m;
                    break;
                }
            }
            //if (a!=-1)

            //{


            //    for (int i = a; i < prolist.Count(); i++)
            //    {
            //        string part,part0;
            //        if (prolist[i].licenceNo.Trim().IndexOf('照') != -1||prolist[i].licenceNo.Trim().IndexOf('字') != -1||prolist[i].licenceNo.Trim().IndexOf('号')!=-1 || prolist[i].licenceNo.Trim().IndexOf('司') != -1)
            //        {
            //            continue;
            //        }
            //        if (prolist[i].licenceNo.Trim().IndexOf(' ') == -1)
            //        {
            //            if (prolist[i].licenceNo.Trim().IndexOf('-') == -1)
            //            {
            //                continue;
            //            }
            //            part = prolist[i].licenceNo.Trim().Split('-')[1] + "-" + prolist[i].licenceNo.Trim().Split('-')[2];
            //        }
            //        else
            //        {

            //            part = prolist[i].licenceNo.Trim().Split(' ')[1];
            //        }
            //        if(part.IndexOf('（') !=-1)
            //        {
            //            part = part.Substring(0, part.Length-part.IndexOf('（') -1);


            //        }
            //        string[] part1 = part.Split('-');
            //        if(part1[1].Trim().Length<4)
            //        {
            //            while (part1[1].Trim().Length<=4)
            //            {
            //                part1[1] = part1[1].Trim() + "0";
            //            }

            //        }
            //        string part3 = part1[0].Trim() + part1[1].Trim();
            //        for (int j = i + 1; j < prolist.Count(); j++)
            //        {


            //            if (prolist[j].licenceNo.Trim().IndexOf('照')!= -1|| prolist[j].licenceNo.Trim().IndexOf('字') != -1 || prolist[j].licenceNo.Trim().IndexOf('号') != -1||prolist[j].licenceNo.Trim().IndexOf('司') != -1)
            //            {
            //                continue;
            //            }
            //            if (prolist[j].licenceNo.Trim().IndexOf(' ') == -1)
            //            {
            //                if (prolist[j].licenceNo.Trim().IndexOf('-') == -1)
            //                {
            //                    continue;
            //                }
            //                part0 = prolist[j].licenceNo.Trim().Split('-')[1] + "-" + prolist[j].licenceNo.Trim().Split('-')[2];
            //            }
            //            else
            //            {

            //                part0 = prolist[j].licenceNo.Trim().Split(' ')[1];

            //            }
            //            if (part0.IndexOf('（') != -1)
            //            {

            //                part0 = part0.Substring(0,part0.IndexOf('（'));


            //            }
            //            string[] part5 = part0.Split('-');
            //            if (part5[1].Trim().Length < 4)
            //            {
            //                while (part5[1].Trim().Length <= 4)
            //                {
            //                    part5[1] = part5[1].Trim() + "0";
            //                }

            //            }
            //            string part6 = part5[0].Trim() + part5[1].Trim();
            //                if (Int32.Parse(part3)>Int32.Parse(part6))
            //                {
            //                    string b = "";
            //                    b = prolist[i].licenceNo;
            //                    prolist[i].licenceNo = prolist[j].licenceNo;
            //                    prolist[j].licenceNo = b;
            //                    part3 = part6;
            //                }


            //          }
            //       }
            //  }
            if (a != -1)
            {

                for (int i = a; i < prolist.Count(); i++)
                {
                    //a是数据库中所找到的第一个licenceNo含"东"的脚标
                    if (prolist[i].licenceNo.Contains("青岛市诚基经贸有限公司E-12地块"))
                    {
                        continue;
                    }
                    if (prolist[i].licenceNo.Contains('东')) {
                        //提取第i个含东licenceNo后面的数字编码
                        int str1 = -1;
                        try
                        {
                            string str0 = prolist[i].licenceNo.Substring(prolist[i].licenceNo.IndexOf('东') + 1, 3);
                            str1 = Int32.Parse(str0);
                        }
                        catch (Exception){

                        }
                        for (int j = i + 1; j < prolist.Count(); j++)
                        {
                            if (prolist[j].licenceNo.Contains("青岛市诚基经贸有限公司E-12地块"))
                            {
                                continue;
                            }
                            if (prolist[j].licenceNo.Contains('东')) {
                                int str3 = -1;
                                try
                                {
                                    //j是i后面的条目，下面str2用i的原因应该是默认了含东的licenceNo具有相同的长度及格式，然后与上面相同提取含东licenceNo后面的数字编码
                                    string str2 = prolist[j].licenceNo.Substring(prolist[i].licenceNo.IndexOf('东') + 1, 3);
                                    str3 = Int32.Parse(str2);
                                    //如果str1的编码比str3的大，也就是如果前面的序号比后面的序号大，那么交换这两个条目的编码。这里由于在循环里面，他保证了前面的序号一定是最小的
                                    if (str1 > str3)
                                    {
                                        string b = "";
                                        b = prolist[i].licenceNo;
                                        //仅仅是licenceNo的交换，存疑
                                        prolist[i].licenceNo = prolist[j].licenceNo;
                                        prolist[j].licenceNo = b;
                                        str1 = str3;

                                        //将上面的修改为对整一条数据进行交换
                                        //OtherArchives tempModel = prolist[i];
                                        //prolist[i] = prolist[j];
                                        //prolist[j] = tempModel;
                                    }
                                }
                                catch (Exception) {

                                }
                            }
                           
                        }
                    }
                    

                    
                }

            }

            var e = prolist.ToPagedList(pageNumber, pageSize);
            var d = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                         //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程

                         from p in e
                         select new JObject(
                        new JProperty("ID", p.ID),
                        new JProperty("licenceNo", p.licenceNo),
                        new JProperty("isImageExist", p.isImageExist),
                        new JProperty("applyUnit", p.applyUnit),
                        new JProperty("projectRange", p.projectRange),
                        new JProperty("location", p.location),
                        new JProperty("registrationNo", p.registrationNo),

                        new JProperty("areaProSeqNo", p.areaProSeqNo),
                        //new JProperty("kaigongTime", p.kaigongTime),
                        //new JProperty("jungongTime", p.jungongTime),
                        //new JProperty("jgDate", p.jgDate),
                        new JProperty("cunfangLocation", p.cunfangLocation),
                        new JProperty("tranferUnit", p.tranferUnit),
                        new JProperty("urbanID", p.urbanID),
                        new JProperty("isNeibu", p.isNeibu)

                                      )


               )
            )

).ToString();
            return d;
        }
        //        public string LookQingArchives_ZhizhaoData(string SerachText, int? page)
        //        {
        //            var UserID = User.Identity.GetUserId();//获取当前用户
        //            var user = db_user.AspNetUsers.Find(UserID);


        //            db.Configuration.ProxyCreationEnabled = false;
        //            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
        //            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
        //            OleDbConnection conn = new OleDbConnection(builder.ToString());
        //            conn.Open();
        //            OleDbCommand cmd = new OleDbCommand();
        //            string sql = "SELECT ID,licenceNo,isImageExist,applyUnit,projectRange,location,year,areaProSeqNo,cunfangLocation,tranferUnit,urbanID,registrationNo,isNeibu From UrbanCon.dbo.OtherArchives where classTypeID=1 and status='RK' and ";
        //            if (user.UserName == "借阅用户" || user.UserName == "管理科借阅")
        //            {
        //                sql = "SELECT ID,licenceNo,isImageExist,applyUnit,projectRange,location,year,areaProSeqNo,cunfangLocation,tranferUnit,urbanID,registrationNo,isNeibu  From UrbanCon.dbo.OtherArchives where classTypeID=1 and isNeibu !=1 and status='RK' and ";
        //            }
        //            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
        //            int count = str.Length - 1;

        //            for (int i = 0; i < count; i++)
        //            {

        //                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
        //                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

        //                //判断字符中是否存在与与或的判断符
        //                if (split.Length == 3)
        //                {
        //                    Require = split[0];
        //                    var name = from b in db.OtherArchiveAttribute
        //                               where b.Description == Require
        //                               select b.colName;
        //                    Require = name.First().Trim();
        //                    Optertitle = split[1];
        //                    Content1 = split[2];
        //                }
        //                else
        //                {
        //                    Require = split[1];
        //                    var name = from c in db.OtherArchiveAttribute
        //                               where c.Description == Require
        //                               select c.colName;
        //                    Require = name.First().Trim();
        //                    AndOr = split[0];

        //                    Optertitle = split[2];
        //                    Content1 = split[3];
        //                }

        //                if (AndOr == "或者")
        //                {

        //                    sql += " or ";

        //                }
        //                if (AndOr == "而且")
        //                {
        //                    sql += " and ";
        //                }

        //                if (Optertitle == "包含")
        //                {
        //                    sql += Require + " like '%" + Content1 + "%'";

        //                }
        //                if (Optertitle == "模糊包含")
        //                {
        //                    sql += Require + " like '%";
        //                    for (int j = 0; j < Content1.Length; j++)
        //                    {
        //                        sql += Content1[j].ToString();
        //                        sql += "%";

        //                    }
        //                    sql += "'";

        //                }
        //                if (Optertitle == "等于")
        //                {

        //                    sql += Require + "=" + "'" + Content1 + "'";

        //                }
        //                if (Optertitle == "前方一致")
        //                {
        //                    sql += Require + " like " + "'" + Content1 + "%'";

        //                }
        //                if (Optertitle == "大于")
        //                {




        //                    sql += Require + ">" + Content1;

        //                }
        //                if (Optertitle == "小于")
        //                {

        //                    sql += Require + "<" + Content1;

        //                }

        //                if (Optertitle == "为NULL")
        //                {
        //                    sql += Require + " is null";

        //                }
        //                if (Optertitle == "为空格")
        //                {
        //                    sql += Require + "=' '";

        //                }
        //                if (Optertitle == "在……之中")
        //                {
        //                    string[] strInfo = Content1.Split('-');
        //                    sql += Require + " in(";
        //                    string st = "";
        //                    foreach (string s in strInfo)
        //                    {
        //                        st += s + ",";
        //                    }
        //                    st = st.Substring(0, st.Length - 1);
        //                    sql = sql + st + ")";

        //                }
        //                if (Optertitle == "在……之间")
        //                {
        //                    string[] su = Content1.Split('-');


        //                    sql += Require + " between " + su[0] + " and " + su[1];
        //                }
        //                if (Optertitle == "不等于")
        //                {


        //                    sql += Require + "!=" + "'" + Content1 + "'";
        //                }

        //            }
        //            if (System.Text.RegularExpressions.Regex.Matches(sql, @"\(").Count != System.Text.RegularExpressions.Regex.Matches(sql, @"\)").Count)
        //            {
        //                sql += ")";
        //            }

        //            sql += " order by licenceNo";
        //            //查询数据条数
        //            cmd.CommandText = sql;
        //            cmd.CommandType = CommandType.Text;
        //            cmd.Connection = conn;
        //            DataSet ds = new DataSet();
        //            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

        //            adapter.Fill(ds);


        //            conn.Close();

        //            int count1 = ds.Tables[0].Rows.Count;
        //            int pageSize = 100;
        //            int pageNumber = (page ?? 1);
        //            int cnt = count1 / pageSize + 1;
        //            if (count1 % pageSize == 0)
        //            {
        //                cnt = count1 / pageSize;
        //            }
        //            var result1 = ds.Tables[0];
        //            List<OtherArchives> prolist = new List<OtherArchives>();

        //            foreach (DataRow dr in ds.Tables[0].Rows)
        //            {
        //                OtherArchives vw_project = new OtherArchives();
        //                vw_project.ID = Convert.ToInt32(dr["ID"]);
        //                vw_project.licenceNo = dr["licenceNo"].ToString();
        //                vw_project.registrationNo = dr["registrationNo"].ToString();
        //                vw_project.isImageExist = dr["isImageExist"].ToString();
        //                vw_project.applyUnit = dr["applyUnit"].ToString();
        //                vw_project.projectRange = dr["projectRange"].ToString();
        //                vw_project.location = dr["location"].ToString();
        //                vw_project.year = dr["year"].ToString();
        //                vw_project.areaProSeqNo = dr["areaProSeqNo"].ToString();
        //                vw_project.cunfangLocation = dr["cunfangLocation"].ToString();
        //                vw_project.tranferUnit = dr["tranferUnit"].ToString();
        //                vw_project.urbanID = dr["urbanID"].ToString();

        //                vw_project.isNeibu = dr["isNeibu"].ToString(); ;
        //                prolist.Add(vw_project);
        //            }
        //            int a = -1;
        //            for (int m = 0; m < prolist.Count(); m++)
        //            {
        //                if (prolist[m].licenceNo.Contains("东"))
        //                {
        //                    a = m;
        //                    break;
        //                }
        //            }
        //            //if (a!=-1)

        //            //{


        //            //    for (int i = a; i < prolist.Count(); i++)
        //            //    {
        //            //        string part,part0;
        //            //        if (prolist[i].licenceNo.Trim().IndexOf('照') != -1||prolist[i].licenceNo.Trim().IndexOf('字') != -1||prolist[i].licenceNo.Trim().IndexOf('号')!=-1 || prolist[i].licenceNo.Trim().IndexOf('司') != -1)
        //            //        {
        //            //            continue;
        //            //        }
        //            //        if (prolist[i].licenceNo.Trim().IndexOf(' ') == -1)
        //            //        {
        //            //            if (prolist[i].licenceNo.Trim().IndexOf('-') == -1)
        //            //            {
        //            //                continue;
        //            //            }
        //            //            part = prolist[i].licenceNo.Trim().Split('-')[1] + "-" + prolist[i].licenceNo.Trim().Split('-')[2];
        //            //        }
        //            //        else
        //            //        {

        //            //            part = prolist[i].licenceNo.Trim().Split(' ')[1];
        //            //        }
        //            //        if(part.IndexOf('（') !=-1)
        //            //        {
        //            //            part = part.Substring(0, part.Length-part.IndexOf('（') -1);


        //            //        }
        //            //        string[] part1 = part.Split('-');
        //            //        if(part1[1].Trim().Length<4)
        //            //        {
        //            //            while (part1[1].Trim().Length<=4)
        //            //            {
        //            //                part1[1] = part1[1].Trim() + "0";
        //            //            }

        //            //        }
        //            //        string part3 = part1[0].Trim() + part1[1].Trim();
        //            //        for (int j = i + 1; j < prolist.Count(); j++)
        //            //        {


        //            //            if (prolist[j].licenceNo.Trim().IndexOf('照')!= -1|| prolist[j].licenceNo.Trim().IndexOf('字') != -1 || prolist[j].licenceNo.Trim().IndexOf('号') != -1||prolist[j].licenceNo.Trim().IndexOf('司') != -1)
        //            //            {
        //            //                continue;
        //            //            }
        //            //            if (prolist[j].licenceNo.Trim().IndexOf(' ') == -1)
        //            //            {
        //            //                if (prolist[j].licenceNo.Trim().IndexOf('-') == -1)
        //            //                {
        //            //                    continue;
        //            //                }
        //            //                part0 = prolist[j].licenceNo.Trim().Split('-')[1] + "-" + prolist[j].licenceNo.Trim().Split('-')[2];
        //            //            }
        //            //            else
        //            //            {

        //            //                part0 = prolist[j].licenceNo.Trim().Split(' ')[1];

        //            //            }
        //            //            if (part0.IndexOf('（') != -1)
        //            //            {

        //            //                part0 = part0.Substring(0,part0.IndexOf('（'));


        //            //            }
        //            //            string[] part5 = part0.Split('-');
        //            //            if (part5[1].Trim().Length < 4)
        //            //            {
        //            //                while (part5[1].Trim().Length <= 4)
        //            //                {
        //            //                    part5[1] = part5[1].Trim() + "0";
        //            //                }

        //            //            }
        //            //            string part6 = part5[0].Trim() + part5[1].Trim();
        //            //                if (Int32.Parse(part3)>Int32.Parse(part6))
        //            //                {
        //            //                    string b = "";
        //            //                    b = prolist[i].licenceNo;
        //            //                    prolist[i].licenceNo = prolist[j].licenceNo;
        //            //                    prolist[j].licenceNo = b;
        //            //                    part3 = part6;
        //            //                }


        //            //          }
        //            //       }
        //            //  }
        //            if (a != -1)
        //            {

        //                for (int i = a; i < prolist.Count(); i++)
        //                {
        //                    if (prolist[i].licenceNo.Contains("青岛市诚基经贸有限公司E-12地块"))
        //                    {
        //                        continue;
        //                    }
        //                    string str0 = prolist[i].licenceNo.Substring(prolist[i].licenceNo.IndexOf('东') + 1, 3);
        //                    int str1 = Int32.Parse(str0);
        //                    for (int j = i + 1; j < prolist.Count(); j++)
        //                    {
        //                        if (prolist[j].licenceNo.Contains("青岛市诚基经贸有限公司E-12地块"))
        //                        {
        //                            continue;
        //                        }


        //                        string str2 = prolist[j].licenceNo.Substring(prolist[i].licenceNo.IndexOf('东') + 1, 3);
        //                        int str3 = 0;
        //                        try
        //                        {
        //                            str3 = Int32.Parse(str2);
        //                        }
        //                        catch (Exception ex) {
        //                            string ss = prolist[i].licenceNo;
        //                        }
        //                        if (str1 > str3)
        //                        {
        //                            string b = "";
        //                            b = prolist[i].licenceNo;
        //                            prolist[i].licenceNo = prolist[j].licenceNo;
        //                            prolist[j].licenceNo = b;
        //                            str1 = str3;
        //                        }
        //                    }
        //                }

        //            }

        //            var e = prolist.ToPagedList(pageNumber, pageSize);
        //            var d = new JObject(
        //                        new JProperty("last_page", cnt),
        //                        new JProperty("data",
        //                                new JArray(
        //                         //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程

        //                         from p in e
        //                         select new JObject(
        //                        new JProperty("ID", p.ID),
        //                        new JProperty("licenceNo", p.licenceNo),
        //                        new JProperty("isImageExist", p.isImageExist),
        //                        new JProperty("applyUnit", p.applyUnit),
        //                        new JProperty("projectRange", p.projectRange),
        //                        new JProperty("location", p.location),
        //                        new JProperty("registrationNo", p.registrationNo),

        //                        new JProperty("areaProSeqNo", p.areaProSeqNo),
        //                        //new JProperty("kaigongTime", p.kaigongTime),
        //                        //new JProperty("jungongTime", p.jungongTime),
        //                        //new JProperty("jgDate", p.jgDate),
        //                        new JProperty("cunfangLocation", p.cunfangLocation),
        //                        new JProperty("tranferUnit", p.tranferUnit),
        //                        new JProperty("urbanID", p.urbanID),
        //                        new JProperty("isNeibu", p.isNeibu)

        //                                      )


        //               )
        //            )

        //).ToString();
        //            return d;
        //        }
        //        public string LookQingArchives_ZhizhaoData(string SerachText, int? page)
        //        {
        //            var UserID = User.Identity.GetUserId();//获取当前用户
        //            var user = db_user.AspNetUsers.Find(UserID);


        //            db.Configuration.ProxyCreationEnabled = false;
        //            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
        //            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
        //            OleDbConnection conn = new OleDbConnection(builder.ToString());
        //            conn.Open();
        //            OleDbCommand cmd = new OleDbCommand();
        //            string sql = "SELECT ID,licenceNo,isImageExist,applyUnit,projectRange,location,year,areaProSeqNo,cunfangLocation,tranferUnit,urbanID,registrationNo,isNeibu From UrbanCon.dbo.OtherArchives where classTypeID=1 and status='RK' and ";
        //            if (user.UserName == "借阅用户"|| user.UserName == "管理科借阅")
        //            {
        //                sql = "SELECT ID,licenceNo,isImageExist,applyUnit,projectRange,location,year,areaProSeqNo,cunfangLocation,tranferUnit,urbanID,registrationNo,isNeibu  From UrbanCon.dbo.OtherArchives where classTypeID=1 and isNeibu !=1 and status='RK' and ";
        //            }
        //            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
        //            int count = str.Length - 1;

        //            for (int i = 0; i < count; i++)
        //            {

        //                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
        //                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

        //                //判断字符中是否存在与与或的判断符
        //                if (split.Length == 3)
        //                {
        //                    Require = split[0];
        //                    var name = from b in db.OtherArchiveAttribute
        //                               where b.Description == Require
        //                               select b.colName;
        //                    Require = name.First().Trim();
        //                    Optertitle = split[1];
        //                    Content1 = split[2];
        //                }
        //                else
        //                {
        //                    Require = split[1];
        //                    var name = from c in db.OtherArchiveAttribute
        //                               where c.Description == Require
        //                               select c.colName;
        //                    Require = name.First().Trim();
        //                    AndOr = split[0];

        //                    Optertitle = split[2];
        //                    Content1 = split[3];
        //                }

        //                if (AndOr == "或者")
        //                {

        //                    sql += " or ";

        //                }
        //                if (AndOr == "而且")
        //                {
        //                    sql += " and ";
        //                }

        //                if (Optertitle == "包含")
        //                {
        //                    sql += Require + " like '%" + Content1 + "%'";

        //                }
        //                if (Optertitle == "模糊包含")
        //                {
        //                    sql += Require + " like '%";
        //                    for (int j = 0; j < Content1.Length; j++)
        //                    {
        //                        sql += Content1[j].ToString();
        //                        sql += "%";

        //                    }
        //                    sql += "'";

        //                }
        //                if (Optertitle == "等于")
        //                {

        //                    sql += Require + "=" + "'" + Content1 + "'";

        //                }
        //                if (Optertitle == "前方一致")
        //                {
        //                    sql += Require + " like " + "'" + Content1 + "%'";

        //                }
        //                if (Optertitle == "大于")
        //                {




        //                    sql += Require + ">" + Content1;

        //                }
        //                if (Optertitle == "小于")
        //                {

        //                    sql += Require + "<" + Content1;

        //                }

        //                if (Optertitle == "为NULL")
        //                {
        //                    sql += Require + " is null";

        //                }
        //                if (Optertitle == "为空格")
        //                {
        //                    sql += Require + "=' '";

        //                }
        //                if (Optertitle == "在……之中")
        //                {
        //                    string[] strInfo = Content1.Split('-');
        //                    sql += Require + " in(";
        //                    string st = "";
        //                    foreach (string s in strInfo)
        //                    {
        //                        st += s + ",";
        //                    }
        //                    st = st.Substring(0, st.Length - 1);
        //                    sql = sql + st + ")";

        //                }
        //                if (Optertitle == "在……之间")
        //                {
        //                    string[] su = Content1.Split('-');


        //                    sql += Require + " between " + su[0] + " and " + su[1];
        //                }
        //                if (Optertitle == "不等于")
        //                {


        //                    sql += Require + "!=" + "'" + Content1 + "'";
        //                }

        //            }
        //            if (System.Text.RegularExpressions.Regex.Matches(sql, @"\(").Count != System.Text.RegularExpressions.Regex.Matches(sql, @"\)").Count)
        //            {
        //                sql += ")";
        //            }

        //            sql += " order by licenceNo";
        //            //查询数据条数
        //            cmd.CommandText = sql;
        //            cmd.CommandType = CommandType.Text;
        //            cmd.Connection = conn;
        //            DataSet ds = new DataSet();
        //            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

        //                adapter.Fill(ds);


        //            conn.Close();

        //            int count1 = ds.Tables[0].Rows.Count;
        //            int pageSize = 100;
        //            int pageNumber = (page ?? 1);
        //            int cnt = count1 / pageSize + 1;
        //            if (count1 % pageSize == 0)
        //            {
        //                cnt = count1 / pageSize;
        //            }
        //            var result1 = ds.Tables[0];
        //            List<OtherArchives> prolist = new List<OtherArchives>();

        //            foreach (DataRow dr in ds.Tables[0].Rows)
        //            {
        //                OtherArchives vw_project = new OtherArchives();
        //                vw_project.ID = Convert.ToInt32(dr["ID"]); 
        //                vw_project.licenceNo = dr["licenceNo"].ToString();
        //                vw_project.registrationNo = dr["registrationNo"].ToString();
        //                vw_project.isImageExist = dr["isImageExist"].ToString();
        //                vw_project.applyUnit = dr["applyUnit"].ToString();
        //                vw_project.projectRange = dr["projectRange"].ToString();
        //                vw_project.location = dr["location"].ToString();
        //                vw_project.year = dr["year"].ToString();
        //                vw_project.areaProSeqNo = dr["areaProSeqNo"].ToString();
        //                vw_project.cunfangLocation = dr["cunfangLocation"].ToString();
        //                vw_project.tranferUnit = dr["tranferUnit"].ToString();
        //                vw_project.urbanID = dr["urbanID"].ToString();

        //                vw_project.isNeibu = dr["isNeibu"].ToString(); ;
        //                prolist.Add(vw_project);
        //            }
        //            int a =-1;
        //            for(int m = 0; m < prolist.Count(); m++)
        //            {
        //                if(prolist[m].licenceNo.Contains("东"))
        //                {
        //                    a = m;
        //                    break;
        //                }
        //            }
        //            //if (a!=-1)

        //            //{


        //            //    for (int i = a; i < prolist.Count(); i++)
        //            //    {
        //            //        string part,part0;
        //            //        if (prolist[i].licenceNo.Trim().IndexOf('照') != -1||prolist[i].licenceNo.Trim().IndexOf('字') != -1||prolist[i].licenceNo.Trim().IndexOf('号')!=-1 || prolist[i].licenceNo.Trim().IndexOf('司') != -1)
        //            //        {
        //            //            continue;
        //            //        }
        //            //        if (prolist[i].licenceNo.Trim().IndexOf(' ') == -1)
        //            //        {
        //            //            if (prolist[i].licenceNo.Trim().IndexOf('-') == -1)
        //            //            {
        //            //                continue;
        //            //            }
        //            //            part = prolist[i].licenceNo.Trim().Split('-')[1] + "-" + prolist[i].licenceNo.Trim().Split('-')[2];
        //            //        }
        //            //        else
        //            //        {

        //            //            part = prolist[i].licenceNo.Trim().Split(' ')[1];
        //            //        }
        //            //        if(part.IndexOf('（') !=-1)
        //            //        {
        //            //            part = part.Substring(0, part.Length-part.IndexOf('（') -1);


        //            //        }
        //            //        string[] part1 = part.Split('-');
        //            //        if(part1[1].Trim().Length<4)
        //            //        {
        //            //            while (part1[1].Trim().Length<=4)
        //            //            {
        //            //                part1[1] = part1[1].Trim() + "0";
        //            //            }

        //            //        }
        //            //        string part3 = part1[0].Trim() + part1[1].Trim();
        //            //        for (int j = i + 1; j < prolist.Count(); j++)
        //            //        {


        //            //            if (prolist[j].licenceNo.Trim().IndexOf('照')!= -1|| prolist[j].licenceNo.Trim().IndexOf('字') != -1 || prolist[j].licenceNo.Trim().IndexOf('号') != -1||prolist[j].licenceNo.Trim().IndexOf('司') != -1)
        //            //            {
        //            //                continue;
        //            //            }
        //            //            if (prolist[j].licenceNo.Trim().IndexOf(' ') == -1)
        //            //            {
        //            //                if (prolist[j].licenceNo.Trim().IndexOf('-') == -1)
        //            //                {
        //            //                    continue;
        //            //                }
        //            //                part0 = prolist[j].licenceNo.Trim().Split('-')[1] + "-" + prolist[j].licenceNo.Trim().Split('-')[2];
        //            //            }
        //            //            else
        //            //            {

        //            //                part0 = prolist[j].licenceNo.Trim().Split(' ')[1];

        //            //            }
        //            //            if (part0.IndexOf('（') != -1)
        //            //            {

        //            //                part0 = part0.Substring(0,part0.IndexOf('（'));


        //            //            }
        //            //            string[] part5 = part0.Split('-');
        //            //            if (part5[1].Trim().Length < 4)
        //            //            {
        //            //                while (part5[1].Trim().Length <= 4)
        //            //                {
        //            //                    part5[1] = part5[1].Trim() + "0";
        //            //                }

        //            //            }
        //            //            string part6 = part5[0].Trim() + part5[1].Trim();
        //            //                if (Int32.Parse(part3)>Int32.Parse(part6))
        //            //                {
        //            //                    string b = "";
        //            //                    b = prolist[i].licenceNo;
        //            //                    prolist[i].licenceNo = prolist[j].licenceNo;
        //            //                    prolist[j].licenceNo = b;
        //            //                    part3 = part6;
        //            //                }


        //            //          }
        //            //       }
        //            //  }
        //            if (a != -1)
        //           {

        //                for (int i = a; i < prolist.Count(); i++)
        //                {
        //                    if(prolist[i].licenceNo.Contains("青岛市诚基经贸有限公司E-12地块"))
        //                    {
        //                        continue;
        //                    }
        //                    string str0 = prolist[i].licenceNo.Substring(prolist[i].licenceNo.IndexOf('东')+1,3);
        //                    int str1 = Int32.Parse(str0);
        //                    for (int j = i + 1; j < prolist.Count(); j++)
        //                    {
        //                        if (prolist[j].licenceNo.Contains("青岛市诚基经贸有限公司E-12地块"))
        //                        {
        //                            continue;
        //                        }
        //                        string str2 = prolist[j].licenceNo.Substring(prolist[i].licenceNo.IndexOf('东') + 1, 3);
        //                        int str3 = Int32.Parse(str2);
        //                        if (str1 > str3)
        //                        {
        //                            string b = "";
        //                            b = prolist[i].licenceNo;
        //                            prolist[i].licenceNo = prolist[j].licenceNo;
        //                            prolist[j].licenceNo = b;
        //                            str1 = str3;
        //                        }
        //                    }
        //                }

        //            }

        //            var e = prolist.ToPagedList(pageNumber, pageSize);
        //            var d = new JObject(
        //                        new JProperty("last_page", cnt),
        //                        new JProperty("data",
        //                                new JArray(
        //                         //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程

        //                         from p in e
        //                         select new JObject(
        //                        new JProperty("ID", p.ID),
        //                        new JProperty("licenceNo", p.licenceNo),
        //                        new JProperty("isImageExist", p.isImageExist),
        //                        new JProperty("applyUnit", p.applyUnit),
        //                        new JProperty("projectRange", p.projectRange),
        //                        new JProperty("location", p.location),
        //                        new JProperty("registrationNo", p.registrationNo),

        //                        new JProperty("areaProSeqNo", p.areaProSeqNo),
        //                        //new JProperty("kaigongTime", p.kaigongTime),
        //                        //new JProperty("jungongTime", p.jungongTime),
        //                        //new JProperty("jgDate", p.jgDate),
        //                        new JProperty("cunfangLocation", p.cunfangLocation),
        //                        new JProperty("tranferUnit", p.tranferUnit),
        //                        new JProperty("urbanID", p.urbanID),
        //                        new JProperty("isNeibu", p.isNeibu)

        //                                      )


        //               )
        //            )

        //).ToString();
        //            return d;
        //        }
        public ActionResult LookQingArchives_Zhizhao_SingleArchiveDetailInfo(string id)
        {
            if (id == "" || id == null)
            {
                return Content("<script >alert('请选择一项记录');window.history.back();</script >");
            }
            long ID = long.Parse(id.Trim());
            var model = from a in db.OtherArchives
                        where a.classTypeID==1&&a.ID == ID
                        select a;
            if (model.Count() == 0)
            {
                return Content("<script >alert('该案卷无详细信息');window.history.back();</script >");
            }
            OtherArchives zhizhao = model.First();
            //是否异地
           
           
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

              };
            ViewBag.isYD = new SelectList(list, "Value", "Text");
            if (zhizhao.isYD == true)
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text",1);
            }
            else
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text",0);
            }
            //是否内部
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1"},
                new SelectListItem { Text = "公开/内部", Value = "2"},
            };
            ViewBag.isNeibu = new SelectList(list1, "Value", "Text", zhizhao.isNeibu);
            //档案密级
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName",model.First().securityID);
            //保管年限
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName",model.First().retentionPeriodNo);
            zhizhao.tuzhiniandai = "执照档案(ZZ)";//档案类型
            zhizhao.tufu= (model.First().drawing + model.First().textMaterial).ToString() == "0" ? "" : (model.First().drawing + model.First().textMaterial).ToString();
            zhizhao.urbanID = "437402";
            zhizhao.ArchiveThick = zhizhao.ArchiveThick + 1;
            if(zhizhao.isImageExist=="无")
            {
                ViewData["isImage"] = "disable";
                ViewData["Message"] = "该案卷无扫描件";
            }
           
            return View(zhizhao);
        }
        
        public string  LookQingArchivesZhizhao(int ID, string licenceNo)
        {
            var model = from a in db.OtherArchives
                        where a.ID == ID
                        select a;
            int year = 0;
            if (model.Count() != 0)
            {
                if (!string.IsNullOrEmpty(model.First().year))
                {
                    int.TryParse(model.First().year.Trim(), out year);
                }
            }


            string name = "license";
            string strpath = string.Empty;

            string strID = licenceNo.Trim();

            if (strID != "")
            {
                
                if (year < 2007)
                {

                    if (strID.IndexOf('-') == -1)
                    {
                        //执照档案路径为License\\执照号
                        strpath = "License/" + strID;
                    }
                    else
                    {
                        string[] strIDs = strID.Split('-');
                        if (strIDs.Length == 2)
                        {
                            //执照档案路径为License\\年份\\执照号
                            strpath = "License/" + strIDs[0] + "/" + strID;
                        }
                        else if (strIDs.Length == 3)
                        {
                            int i = int.Parse(strIDs[2]);
                            //执照档案路径为License\\年份\\执照号\\序号
                            strpath = "License/" + strIDs[0] + "/" + strIDs[0] + "-" + strIDs[1] + "/" + i;
                        }
                    }
                }
                else
                {
                    string yearseqno = model.First().registrationNo.Trim();
                    if (yearseqno.IndexOf('-') == -1)
                    {
                        strpath = "License/" + year.ToString() + "/" + yearseqno.Substring(4);
                    }
                    else
                    {
                        string[] stryearseqno = yearseqno.Split('-');
                        int i = int.Parse(stryearseqno[1]);
                        strpath = "License/" + year.ToString() + "/" + stryearseqno[0].Substring(4) + "/" + i.ToString();
                    }
                }
                
            }

            return JsonConvert.SerializeObject(strpath);
        }
        public string LookQingArchivesRoad(string DLYea, string DLVolNo)
        {
            string strID = DLYea.Trim() + "/" + DLVolNo.Trim();
            
            //string name = "roadclassify";
            string strpath = "Road/" + strID;

            return JsonConvert.SerializeObject(strpath);
        }
        public string LookQingArchivesClass(string ArchiveNo)
        {
            //string name = "classify";
            string strID = ArchiveNo.Trim();
            string strpath = "";
            if (strID != "")
            {
                 strpath = "Classify/" + strID;
            }
            return JsonConvert.SerializeObject(strpath); 
        }
        public ActionResult OtherArchivesView1(string location)
        {
         
            
            return View();
        }
        public string GetlicensePicListDemo(string path)
        {
            DataTable myTable = null;
            string strPath = string.Empty;
            string strWebPath = string.Empty;
            //HttpContext.Current.Session["paperProjectSeqNo"] = "19";
            if (path != null && path != string.Empty)
            {
                if (path.Contains("??"))
                {
                    path = path.Replace("??", "东");
                    myTable = new DataTable();
                    string[] strPaths = path.Split('/');
                    if (strPaths.Length == 4)
                    {
                        strPath = strPaths[0] + "\\" + strPaths[1].Substring(0, 4) + "\\" + strPaths[3];
                        strWebPath = strPaths[0] + "/" + strPaths[1].Substring(0, 4) + "/" + strPaths[3] + "/";
                    }
                    else
                    {
                        strPath = strPaths[0] + "\\" + strPaths[1].Substring(0, 4);
                        strWebPath = strPaths[0] + "/" + strPaths[1].Substring(0, 4) + "/";
                    }
                    strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["OtherArchivesPath"] + strPath + "\\";
                    DirectoryInfo myDirInfo;
                    Array arrFileInfo;

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
                                if (isImage(myFile.Extension.Trim()) == true)
                                {
                                    myDataRow = myTable.NewRow();
                                    myDataRow["Name"] = myFile.Name;
                                    myDataRow["WebPath"] = System.Web.Configuration.WebConfigurationManager.AppSettings["OtherArchivesWeb"] + Server.UrlPathEncode(strWebPath + myFile.Name);

                                    myTable.Rows.Add(myDataRow);
                                }
                            }
                        }
                    }
                }
                else
                {
                    myTable = new DataTable();
                    //strPath = HttpContext.Current.Session["ID"].ToString();
                    string[] strPaths = path.Split('/');
                    for (int i = 0; i < strPaths.Length - 1; i++)
                    {
                        strPath += strPaths[i] + "\\";
                    }
                    strPath += strPaths[strPaths.Length - 1];
                    //D:\OtherArchives\Licence\1983\1983-00457
                    strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["OtherArchivesPath"] + strPath + "\\";
                    strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["OtherArchivesWeb"];

                    DirectoryInfo myDirInfo;
                    Array arrFileInfo;

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
                                if (isImage(myFile.Extension.Trim()) == true)
                                {
                                    myDataRow = myTable.NewRow();
                                    myDataRow["Name"] = myFile.Name;
                                    myDataRow["WebPath"] = strWebPath + Server.UrlPathEncode(path + "/" + myFile.Name);

                                    myTable.Rows.Add(myDataRow);
                                }
                            }
                        }
                    }
                }
            }

            
            return JsonConvert.SerializeObject(myTable);
        }

        public string GetlicensePicListDemoJN(string path)
        {
            DataTable myTable = null;
            string strPath = string.Empty;
            string strWebPath = string.Empty;
            //path = HttpUtility.UrlDecode(path, Encoding.GetEncoding("UTF-8"));
            //HttpContext.Current.Session["paperProjectSeqNo"] = "19";
            if (path != null && path != string.Empty)
            {
                if (path.Contains("??"))
                {
                    path = path.Replace("??", "东");
                    myTable = new DataTable();
                    string[] strPaths = path.Split('/');
                    if (strPaths.Length == 4)
                    {
                        strPath = strPaths[0] + "\\" + strPaths[1].Substring(0, 4) + "\\" + strPaths[3];
                        strWebPath = strPaths[0] + "/" + strPaths[1].Substring(0, 4) + "/" + strPaths[3] + "/";
                    }
                    else
                    {
                        strPath = strPaths[0] + "\\" + strPaths[1].Substring(0, 4);
                        strWebPath = strPaths[0] + "/" + strPaths[1].Substring(0, 4) + "/";
                    }
                    strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["OtherArchivesPath"] + strPath + "\\";
                    DirectoryInfo myDirInfo;
                    Array arrFileInfo;

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
                                if (isImage(myFile.Extension.Trim()) == true)
                                {
                                    myDataRow = myTable.NewRow();
                                    myDataRow["Name"] = myFile.Name;
                                    myDataRow["WebPath"] = System.Web.Configuration.WebConfigurationManager.AppSettings["OtherArchivesWeb"] + Server.UrlPathEncode(strWebPath + myFile.Name);

                                    myTable.Rows.Add(myDataRow);
                                }
                            }
                        }
                    }
                }
                else
                {
                    myTable = new DataTable();
                    //strPath = HttpContext.Current.Session["ID"].ToString();
                    string[] strPaths = path.Split('/');
                    for (int i = 0; i < strPaths.Length - 1; i++)
                    {
                        strPath += strPaths[i] + "\\";
                    }
                    strPath += strPaths[strPaths.Length - 1];
                    //D:\OtherArchives\Licence\1983\1983-00457
                    strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["OtherArchivesPath"] + strPath + "\\";
                    strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["OtherArchivesWeb"];

                    DirectoryInfo myDirInfo;
                    Array arrFileInfo;

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
                                if (isImage(myFile.Extension.Trim()) == true)
                                {
                                    myDataRow = myTable.NewRow();
                                    myDataRow["Name"] = myFile.Name;
                                    myDataRow["WebPath"] = strWebPath + Server.UrlPathEncode(path + "/" + myFile.Name);

                                    myTable.Rows.Add(myDataRow);
                                }
                            }
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(myTable);
        }

        public string GetTuZhiPicListDemo(string path)
        {
            DataTable myTable = null;

            string strWebPath = string.Empty;
          
            if (path != null && path != string.Empty)
            {

                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["TuzhiArchivesWeb"] + path + "/";
                path = System.Web.Configuration.WebConfigurationManager.AppSettings["TuzhiArchivesPath"] + path + "\\";
                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));


                if (Directory.Exists(path))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(path);

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
        public string GetPlanPicListDemo(string path,string flag)
        {
            DataTable myTable = null;

            string strWebPath = string.Empty;
            //HttpContext.Current.Session["contractNo"] = "19";
            if (path != null && path != string.Empty)
            {
                string webp = path;
                string strp = path;
                if (path.IndexOf('-') > 0)
                {
                    webp = path.Replace('-', '/');// path.Split('-')[0] + "/" + path.Split('-')[1];
                    strp = path.Replace('-', '\\');
                }
                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["PlanPicsProjAddress"] + webp + "/";
                path = System.Web.Configuration.WebConfigurationManager.AppSettings["PlanPicsProjPath"] + strp + "\\";
                string strWebPath1 = System.Web.Configuration.WebConfigurationManager.AppSettings["PlanPicsProjAddress"] + webp + "/内部/";
                string path1 = System.Web.Configuration.WebConfigurationManager.AppSettings["PlanPicsProjPath"] + strp + "\\内部\\";
                DirectoryInfo myDirInfo;
                Array arrFileInfo;
                myTable = new DataTable();

                DataRow myDataRow;

                myTable.Columns.Add("Name", Type.GetType("System.String"));

                myTable.Columns.Add("WebPath", Type.GetType("System.String"));


                if (Directory.Exists(path))
                {
                    //取得目录信息
                    myDirInfo = new DirectoryInfo(path);

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
                if (flag != "0")
                {
                    if (Directory.Exists(path1))
                    {
                        //取得目录信息
                        myDirInfo = new DirectoryInfo(path1);

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

                                myDataRow["WebPath"] = strWebPath1 + myFile.Name;


                                myTable.Rows.Add(myDataRow);
                            }
                        }

                    }
                }
            }

        

            return JsonConvert.SerializeObject(myTable);
        }
        private bool isImage(string extension)
        {
            string low = extension.ToLower();
            if (low == ".tiff" || low == ".tif" || low == ".jpg" || low == ".bmp")
            {
                return true;
            }
            return false;
        }
        public ActionResult LookQingArchives_Zhizhao_Index(int archiveID)
        {
            ViewBag.archiveID = archiveID;
           

            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
           

          
           if(user.RoleName == "借阅用户")
            {
                var licenceFiles1=from b in db.LicenceFiles
                             where b.archiveID==archiveID&&b.isNeibu=="0"
                             orderby b.juanneiSeqNo
                             select b;//按卷内序号排序
                ViewBag.result1 = JsonConvert.SerializeObject(licenceFiles1);//数据转换成JSON后传给前台
                ViewData["volCount"] = licenceFiles1.Count();

            }
           else
            {
                var licenceFiles = from ad in db.LicenceFiles
                               where ad.archiveID == archiveID
                               orderby ad.juanneiSeqNo
                               select ad;//按卷内序号排序
                ViewBag.result1 = JsonConvert.SerializeObject(licenceFiles);//数据转换成JSON后传给前台
                ViewData["volCount"] = licenceFiles.Count();
            }

            



            return View();
        }
        public ActionResult LookQingArchives_Zhizhao_JuanNeiMuLu(string id)
        {
            if(id==""||id==null)
            {
                return Content("<script >alert('请选择一卷案卷');window.history.back();</script >");
            }
            long ID = long.Parse(id.Trim());
            var model = from a in db.LicenceFiles
                        where a.ID ==ID
                        orderby a.fileNo
                        select a;
            if(model.Count()==0)
            {
                return Content("<script >alert('该案卷无卷内目录信息');window.history.back();</script >");
            }
            return View(model.First());
        }
        public ActionResult LookQingArchives_Road(string SerachTextRoad)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source="+ System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count From UrbanCon.dbo.OtherArchives where classTypeID=2 and ";
            string[] str = SerachTextRoad.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachTextRoad;

            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

             
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.OtherArchiveAttribute
                               where b.Description == Require
                               select b.colName;
                    Require = name.First();
                    Optertitle = split[1];
                    Content1 = split[2];
                }
                else
                {
                    Require = split[1];
                    var name = from c in db.OtherArchiveAttribute
                               where c.Description == Require
                               select c.colName;
                    Require = name.First();
                    AndOr = split[0];

                    Optertitle = split[2];
                    Content1 = split[3];
                }

                if (AndOr == "或")
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
                    if (Require == "volNo" || Require == "paijiaNo" || Require == "year" || Require == "registrationNo" || Require == "landNo"|| Require == "doorplate")
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
                    if (Require == "volNo" || Require == "paijiaNo" || Require == "year" || Require == "registrationNo" || Require == "landNo" || Require == "doorplate")
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
                    if (Require == "volNo" || Require == "paijiaNo" || Require == "year" || Require == "registrationNo" || Require == "landNo" || Require == "doorplate")
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

                    if (Require == "volNo" || Require == "paijiaNo" || Require == "year" || Require == "registrationNo" || Require == "landNo" || Require == "doorplate")
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

            //查询数据条数
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            DataSet ds = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            adapter.Fill(ds);
            conn.Close();
         
            ViewData["SerachText"] = SerachTextRoad.ToString();
            ViewData["count"] =ds.Tables[0].Rows[0]["count"].ToString();
            int cnt = Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) / 100 + 1;
            if (Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) % 100 == 0)
            {
                cnt = Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) / 100;
            }
            ViewData["totalpage"] = cnt;
            return View();
        }
        public string  LookQingArchives_RoadData(string SerachTextRoad,int?page)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT licenceNo,applyUnit,year,volNo,isImageExist,projectRange,location,newLocation,doorplate,landNo,registrationNo,ID,isNeibu,areaProSeqNo From UrbanCon.dbo.OtherArchives where classTypeID=2 and ";
            string[] str = SerachTextRoad.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachTextRoad;

            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);


                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.OtherArchiveAttribute
                               where b.Description == Require
                               select b.colName;
                    Require = name.First();
                    Optertitle = split[1];
                    Content1 = split[2];
                }
                else
                {
                    Require = split[1];
                    var name = from c in db.OtherArchiveAttribute
                               where c.Description == Require
                               select c.colName;
                    Require = name.First();
                    AndOr = split[0];

                    Optertitle = split[2];
                    Content1 = split[3];
                }

                if (AndOr == "或")
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

            sql += " order by year,volNo";
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
            var result1 = ds.Tables[0];
            List<OtherArchives> prolist = new List<OtherArchives>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OtherArchives vw_project = new OtherArchives();
                vw_project.ID = Convert.ToInt32(dr["ID"]);
                vw_project.licenceNo = dr["licenceNo"].ToString();
                vw_project.applyUnit = dr["applyUnit"].ToString();
                vw_project.year = dr["year"].ToString();
                vw_project.volNo = dr["volNo"].ToString();
                vw_project.isImageExist = dr["isImageExist"].ToString();
               
                vw_project.projectRange = dr["projectRange"].ToString();
                vw_project.location = dr["location"].ToString();
                vw_project.doorplate = dr["doorplate"].ToString();
                vw_project.landNo = dr["landNo"].ToString();
                vw_project.registrationNo = dr["registrationNo"].ToString();
                vw_project.isNeibu = dr["isNeibu"].ToString();
                vw_project.newLocation = dr["newLocation"].ToString();
                vw_project.areaProSeqNo = dr["areaProSeqNo"].ToString();
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
                        new JProperty("ID", p.ID),
                        new JProperty("licenceNo", p.licenceNo),
                        new JProperty("applyUnit", p.applyUnit),
                        new JProperty("year", p.year),
                        new JProperty("volNo", p.volNo),
                        new JProperty("isImageExist", p.isImageExist),
                      
                        new JProperty("projectRange", p.projectRange),
                        new JProperty("location", p.location),
                        new JProperty("doorplate", p.doorplate),
                        new JProperty("landNo", p.landNo),
                        new JProperty("registrationNo", p.registrationNo),
                        new JProperty("isNeibu", p.isNeibu),
                        new JProperty("newLocation", p.newLocation),
                        new JProperty("areaProSeqNo", p.areaProSeqNo)
                                      )


               )
            )

).ToString();
            return d;

        }
        public ActionResult LookQingArchives_Road_SingleArchiveDetailInfo(string id)
        {
            if (id == "" || id == null)
            {
                return Content("<script >alert('请选择一项记录');window.history.back();</script >");
            }
            long ID = long.Parse(id.Trim());
            var model = from a in db.OtherArchives
                        where a.classTypeID == 2 && a.ID == ID
                        select a;
            if (model.Count() == 0)
            {
                return Content("<script >alert('该案卷无详细信息');window.history.back();</script >");
            }
            OtherArchives Road = model.First();
            ////是否异地
            //if (Road.isYD == true)
            //{
            //    Road.isYD = true;
            //}
            //else
            //{
            //    Road.isYD = false;
            //}
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

              };
            ViewBag.isYD = new SelectList(list, "Value", "Text");
            if (Road.isYD == true)
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 1);
            }
            else
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 0);
            }
            //是否内部
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1"},
                new SelectListItem { Text = "公开/内部", Value = "2"},
            };
            ViewBag.isNeibu = new SelectList(list1, "Value", "Text", Road.isNeibu);
            //档案密级
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", Road.securityID);
            //保管年限
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", Road.retentionPeriodNo);
            Road.tuzhiniandai = "道路档案(DL)";//档案类型
            Road.ArchiveThick = Road.ArchiveThick + 1;
            Road.urbanID = "437402";
            if (Road.isImageExist=="无")
            {
                ViewData["isImage"] = "disable";
                ViewData["Message"] = "该案卷无扫描件";
            }

            return View(Road);
        }
        public ActionResult LookQingArchives_Road_JuanNeiMuLu(int id)
        {
            ViewBag.archiveID = id;

            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);

            if (user.RoleName == "借阅用户")
            {
                var RoadFiles1 = from b in db.RoadJuannei
                                 where b.archiveID == id
                                 orderby b.seqNo
                                 select b;//按卷内序号排序
                ViewBag.result1 = JsonConvert.SerializeObject(RoadFiles1);//数据转换成JSON后传给前台
                ViewData["volCount"] = RoadFiles1.Count();
            }
            else
            {
                var RoadFiles = from ad in db.RoadJuannei
                                where ad.archiveID == id
                                orderby ad.seqNo
                                select ad;//按卷内序号排序
                ViewBag.result1 = JsonConvert.SerializeObject(RoadFiles);//数据转换成JSON后传给前台
                ViewData["volCount"] = RoadFiles.Count();
            }
            var roadlist = from a in db.OtherArchives
                           where a.classTypeID == 2
                           where a.ID == id
                           select a;
            ViewData["year"] = roadlist.First().year;
            ViewData["volNo"] = roadlist.First().volNo;
            return View();
        }
        public ActionResult LookQingArchives_Road_JuanNeiMuLuxinxi(string id)
        {
            if (id == "" || id == null)
            {
                return Content("<script >alert('请选择一卷案卷');window.history.back();</script >");
            }
            long ID = long.Parse(id.Trim());
            var model = from a in db.RoadJuannei
                        where a.ID == ID
                        orderby a.fileNo
                        select a;
            if (model.Count() == 0)
            {
                return Content("<script >alert('该案卷无卷内目录信息');window.history.back();</script >");
            }
            return View(model.First());
        }

        public ActionResult LookRoadsaomiaoJN(string year, string volNo, string seqNo)
        {
            while (volNo.Length < 4)
            {
                volNo = "0" + volNo;
            }
            string strID = year.Trim() + "/" + volNo.Trim() + "/" + seqNo.Trim();
            string strpath = "Road/" + strID;
            ViewData["path"] = strpath;
            //ViewData["VolNo"] = volNo;
            //ViewData["seqNo"] = seqNo.Trim();
            return View();
        }

        public ActionResult LookQingArchives_Classtype(string SerachTextClass)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source="+ System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count From UrbanCon.dbo.OtherArchives where classTypeID=3 and ";
            string[] str = SerachTextClass.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachTextClass;

            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.OtherArchiveAttribute
                               where b.Description == Require
                               select b.colName;
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

                if (AndOr == "或")
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
                    if (Require == "bianzhiTime" || Require == "inHouseTime" || Require == "count" || Require == "proSeqNo" || Require == "drawing" || Require == "licenceTime" || Require == "paijiaNo" || Require == "tuzhiniandai" || Require == "registrationNo")
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
                    if (Require == "bianzhiTime" || Require == "inHouseTime" || Require == "count" || Require == "proSeqNo" || Require == "drawing" || Require == "licenceTime" || Require == "paijiaNo" || Require == "tuzhiniandai" || Require == "registrationNo")
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
                    if (Require == "bianzhiTime" || Require == "inHouseTime" || Require == "count" || Require == "proSeqNo" || Require == "drawing" || Require == "licenceTime" || Require == "paijiaNo" || Require == "tuzhiniandai" || Require == "registrationNo")
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

                    if (Require == "bianzhiTime" || Require == "inHouseTime" || Require == "count" || Require == "proSeqNo" || Require == "drawing" || Require == "licenceTime" || Require == "paijiaNo" || Require == "tuzhiniandai" || Require == "registrationNo")
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


            //查询数据条数
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            DataSet ds = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            adapter.Fill(ds);
            conn.Close();
           

            ViewData["count"] = ds.Tables[0].Rows[0]["count"].ToString();
            ViewData["SerachText"] = SerachTextClass;

            int cnt = Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) / 100 + 1;
            if (Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) % 100 == 0)
            {
                cnt = Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) / 100;
            }
            ViewData["totalpage"] = cnt;





            return View();
        }
        public string  LookQingArchives_ClasstypeData(string SerachTextClass,int?page)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT proSeqNo,isImageExist,count,archiveNo,archiveTitle,bianzhiUnit,registrationNo,classNo,ID,isNeibu From UrbanCon.dbo.OtherArchives where classTypeID=3 and ";
            string[] str = SerachTextClass.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachTextClass;

            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.OtherArchiveAttribute
                               where b.Description == Require
                               select b.colName;
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

                if (AndOr == "或")
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


            //查询数据条数
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
            var result1 = ds.Tables[0];
            List<OtherArchives> prolist = new List<OtherArchives>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OtherArchives vw_project = new OtherArchives();
                if(dr["proSeqNo"]==DBNull.Value)
                {
                    vw_project.proSeqNo = 0;
                }
                else
                {
                   vw_project.proSeqNo = Convert.ToInt32(dr["proSeqNo"]);
                }
               
                vw_project.isImageExist = dr["isImageExist"].ToString();
                if(dr["count"]==DBNull.Value)
                {
                    vw_project.count = 0;
                }
                else
                {
                    vw_project.count = Convert.ToInt32(dr["count"]); 
                }
                
                vw_project.archiveNo = dr["archiveNo"].ToString();
                vw_project.archiveTitle = dr["archiveTitle"].ToString();
                

                vw_project.bianzhiUnit = dr["bianzhiUnit"].ToString();
                vw_project.registrationNo = dr["registrationNo"].ToString();
                vw_project.classNo = dr["classNo"].ToString();
                vw_project.ID = Convert.ToInt32(dr["ID"]); 
           
                vw_project.isNeibu = dr["isNeibu"].ToString(); ;
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
                        new JProperty("proSeqNo", p.proSeqNo),
                        new JProperty("isImageExist", p.isImageExist),
                        new JProperty("count", p.count),
                        new JProperty("archiveNo", p.archiveNo),
                        new JProperty("archiveTitle", p.archiveTitle),

                        new JProperty("bianzhiUnit", p.bianzhiUnit),
                        new JProperty("registrationNo", p.registrationNo),
                        new JProperty("classNo", p.classNo),
                   
                        new JProperty("ID", p.ID),
                        new JProperty("isNeibu", p.isNeibu)

                                      )


               )
            )

).ToString();
            return d;
        }
        public ActionResult LookQingArchives_Classtype_SingleArchiveDetailInfo(string id)
        {
            if (id == "" || id == null)
            {
                return Content("<script >alert('请选择一项记录');window.history.back();</script >");
            }
            long ID = long.Parse(id.Trim());
            var model = from a in db.OtherArchives
                        where a.classTypeID ==3 && a.ID == ID
                        select a;
            if (model.Count() == 0)
            {
                return Content("<script >alert('该案卷无详细信息');window.history.back();</script >");
            }
            OtherArchives Classtype = model.First();
            //是否异地
            //if (Classtype.isYD == true)
            //{
            //    Classtype.isYD = true;
            //}
            //else
            //{
            //    Classtype.isYD = false;
            //}
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

              };
            ViewBag.isYD = new SelectList(list, "Value", "Text");
            if (Classtype.isYD == true)
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 1);
            }
            else
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 0);
            }
            //档案密级
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", model.First().securityID);
            //保管年限
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", model.First().retentionPeriodNo);
            Classtype.areaStatus = "分类档案(FL)";//档案类型
          
            Classtype.urbanID = "437402";
            Classtype.ArchiveThick = Classtype.ArchiveThick ;
            if (Classtype.isImageExist == "无")
            {
                ViewData["isImage"] = "disable";
                ViewData["Message"] = "该案卷无扫描件";
            }
            return View(Classtype);
        }
        public ActionResult LookQingArchives_Tuzhi(string SerachTextTuzhi)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source="+ System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count From UrbanCon.dbo.TuzhiArchives where ";
            string[] str = SerachTextTuzhi.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachTextTuzhi;


            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.OtherArchiveAttribute
                               where b.Description == Require
                               select b.colName;
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

                if (AndOr == "或")
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
                    if (Require == "archiveNo" || Require == "paijiaNo" || Require == "registrationNo" || Require == "tuzhiniandai" || Require == "inHouseTime" )
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.location.href='/ArchivesSearch/ArchiveSearchMainWindow/';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索条件非法，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow/';</script >");
                    }



                    sql += Require + ">" + Content1;

                }
                if (Optertitle == "小于")
                {
                    if (Require == "archiveNo" || Require == "paijiaNo" || Require == "registrationNo" || Require == "tuzhiniandai" || Require == "inHouseTime")
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
                    if (Require == "archiveNo" || Require == "paijiaNo" || Require == "registrationNo" || Require == "tuzhiniandai" || Require == "inHouseTime")
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

                    if (Require == "archiveNo" || Require == "paijiaNo" || Require == "registrationNo" || Require == "tuzhiniandai" || Require == "inHouseTime")
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



            //查询数据条数
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            DataSet ds = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            adapter.Fill(ds);
            conn.Close();
            ViewData["count"] = ds.Tables[0].Rows[0]["count"].ToString();
            ViewData["SerachText"] = SerachTextTuzhi;
            int cnt = Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) / 100 + 1;
            if (Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) % 100 == 0)
            {
                cnt = Int32.Parse(ds.Tables[0].Rows[0]["count"].ToString()) / 100;
            }
            ViewData["totalpage"] = cnt;
            return View();
        }

        public string  LookQingArchives_TuzhiData(string SerachTextTuzhi,int?page)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT seqNo,isImageExist,archiveTitle,bianzhiUnit,tuzhiYear,bilichi,tufu,tuzhiStatus,classNo,archiveNo,ID From UrbanCon.dbo.TuzhiArchives where ";
            string[] str = SerachTextTuzhi.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachTextTuzhi;


            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);

                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.OtherArchiveAttribute
                               where b.Description == Require
                               select b.colName;
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

                if (AndOr == "或")
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
            sql += " order by tuzhiYear,ID";


            //查询数据条数
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
            var result1 = ds.Tables[0];
            List<TuzhiArchives> prolist = new List<TuzhiArchives>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                TuzhiArchives vw_project = new TuzhiArchives();
                vw_project.seqNo = Convert.ToInt32(dr["seqNo"]);
                vw_project.isImageExist = dr["isImageExist"].ToString();
                vw_project.archiveTitle = dr["archiveTitle"].ToString();
                vw_project.bianzhiUnit = dr["bianzhiUnit"].ToString();
              

                vw_project.tuzhiYear = dr["tuzhiYear"].ToString();
                vw_project.bilichi = dr["bilichi"].ToString();
                vw_project.tufu = dr["tufu"].ToString();
                vw_project.tuzhiStatus = dr["tuzhiStatus"].ToString();
                vw_project.classNo = dr["classNo"].ToString();
                vw_project.archiveNo = dr["archiveNo"].ToString();
                vw_project.ID = Convert.ToInt32(dr["ID"]); 
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
                        new JProperty("seqNo", p.seqNo),
                        new JProperty("isImageExist", p.isImageExist),
                        new JProperty("archiveTitle", p.archiveTitle),
                        new JProperty("bianzhiUnit", p.bianzhiUnit),
                        new JProperty("tuzhiYear", p.tuzhiYear),

                        new JProperty("bilichi", p.bilichi),
                        new JProperty("tufu", p.tufu),
                        new JProperty("tuzhiStatus", p.tuzhiStatus),
                        new JProperty("classNo", p.classNo),
                        new JProperty("archiveNo", p.archiveNo),
                        new JProperty("ID", p.ID)

                                      )


               )
            )

).ToString();
            return d;
        }
        public ActionResult LookQingArchives_Tuzhi_SingleArchiveDetailInfo(string id)
        {
            if (id == "" || id == null)
            {
                return Content("<script >alert('请选择一项记录');window.history.back();</script >");
            }
            long ID = long.Parse(id.Trim());
            var model = from a in db.TuzhiArchives
                        where a.ID == ID
                        select a;
            if (model.Count() == 0)
            {
                return Content("<script >alert('该案卷无详细信息');window.history.back();</script >");
            }
            TuzhiArchives Tuzhi = model.First();
        
            //档案密级
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", model.First().securityID);
            //保管年限
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", model.First().retentionPeriodNo);
           

            Tuzhi.archiveCode = "437402";
            if (Tuzhi.isImageExist == "无")
            {
                ViewData["isImage"] = "disable";
                ViewData["Message"] = "该案卷无扫描件";
            }
            return View(Tuzhi);
        }
        // GET: ArchiveSearch2/Details/5
        public ActionResult LookPlanProject(string SerachText,string classNo2)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户,进行内外部判断
            var user = db_user.AspNetUsers.Find(UserID);
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source="+ System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count From UrbanCon.dbo.PlanProject  where status='RK' and  ";
            if (user.RoleName.Trim() == "借阅用户")
            {
                sql = "SELECT count(*) as count From UrbanCon.dbo.PlanProject where isNeibu!=" + "'内部'" + " and status='RK' and  ";
            }
            //进行档案类型的判断
            string strClassifyIDs = getClassifyIDsByNames(classNo2);
            string[] claNo = strClassifyIDs.Split(',');
            for(int j=0;j< claNo.Length;j++)
            {
                if (claNo[j]=="0")
                {
                    break;
                }

                if(j!= claNo.Length-1)
                {
                    sql += " classifyID = " + claNo[j] + " or ";
                }
                else
                {
                    sql +=" classifyID = " + claNo[j]+" and ";
                }
               
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
        public ActionResult LookPlanArchives(string SerachText, string classNo2)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source="+ System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count From UrbanCon.dbo.PlanArchiveBox where  ";
            int flag = 0;//只能进行一次选表判断
            //进行档案类型的判断
            string strClassifyIDs = getClassifyIDsByNames(classNo2);
            string[] claNo = strClassifyIDs.Split(',');
            for (int j = 0; j < claNo.Length; j++)
            {
                if (j != claNo.Length - 1)
                {
                    sql += " classifyID = " + claNo[j] + " or ";
                }
                else
                {
                    sql += " classifyID = " + claNo[j] + " and ";
                }

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
                //判断来自哪个表
                if ((Require == "projectLocation" || Require == "developmentUnit" || Require == "fileNo" || Require == "projectContent" || Require == "totalSeqNo") && flag == 0)
                {
                    sql = "SELECT count(*) as count FROM UrbanCon.dbo.PlanArchiveBox c  where c.ID in ( select a.SeqNo from UrbanCon.dbo.PlanProject a where ";
                    for (int j = 0; j < claNo.Length; j++)
                    {
                        if (j != claNo.Length - 1)
                        {
                            sql += " classifyID = " + claNo[j] + " or ";
                        }
                        else
                        {
                            sql += " classifyID = " + claNo[j] + " and ";
                        }

                    }
                    sql += " a.";

                    flag = 1;
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
            adapter.Fill(ds);
            DataTable dt = ds.Tables[0];
            string count1 = dt.Rows[0]["count"].ToString();
            //取得数据前2500条
            sql = sql.Replace("count(*) as count", "top 2500 classifyName,fileNo,isImageExist,totalSeqNo,projectContent,projectLocation,archiveTitle,developmentUnit,boxNo,seqNo1");
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

            return View();
        }
        public ActionResult PlanArchivesInfoes(string id)
        {
            if(id==""||id==null)
            {
                 return Content("<script >alert('请选择一项记录');window.history.back();</script >");
            }
            var UserID = User.Identity.GetUserId();//获取当前用户,进行内外部判断
            var user = db_user.AspNetUsers.Find(UserID);
            if(user.RoleName.Trim()=="借阅用户")
            {
                ViewBag.Niebu = "none";
                ViewData["viewflag"] = 0;
            }
            int ID = int.Parse(id.Trim());
            var Planoroject = from a in ae.PlanProject
                              where a.ID== ID
                              select a;
            int? classID = Planoroject.First().classifyID;
            var className = from b in ae.PlanArchiveClassify
                            where b.classifyID == classID
                            select b.classifyName;
            Planoroject.First().storeLocation = className.First();

            if (Planoroject.Count()==0)
            {
                return Content("<script >alert('该工程没有工程著录单');window.history.back();</script >");
            }

            return View(Planoroject.First());
        }
      
        public ActionResult PlanprojectInfo(string id)
        {
            if (id == "" || id == null)
            {
                return Content("<script >alert('请选择一项记录');window.history.back();</script >");
            }
            int ID = int.Parse(id.Trim());
            var Planoroject = from a in ae.PlanProject
                              where a.ID== ID
                              select a;
            if (Planoroject.Count() == 0)
            {
                return Content("<script >alert('该工程没有工程著录单');window.history.back();</script >");
            }
            //档案密级
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", Planoroject.First().securityID);
            //保管年限
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", Planoroject.First().retentionPeriodID);
           
            return View(Planoroject.First());
        }
       public ActionResult PlanprojectInfo2(string id)
        {
            if (id == "" || id == null)
            {
                return Content("<script >alert('请选择一项记录');window.history.back();</script >");
            }
            int ID = int.Parse(id.Trim());
            var Planoroject = from a in db.vw_PlanProjectBoxList
                              where a.ID == ID
                              select a;
            if (Planoroject.Count() == 0)
            {
                return Content("<script >alert('该案卷没有案卷著录单');window.history.back();</script >");
            }
            //档案密级
            ViewBag.securityID = new SelectList(db.SecurityClassification, "securityID", "securityName", Planoroject.First().securityID);
            //保管年限
            ViewBag.retentionPeriodNo = new SelectList(db.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", Planoroject.First().retentionPeriodID);
            var projectinfo = Planoroject.First();
            if (Planoroject.First().isYD == true)
            {
                projectinfo.isYD = true;
            }
            else
            {
                projectinfo.isYD = false;
            }
            return View(projectinfo);
        }

        public ActionResult PlanArchiveInProject(string id)
        {
            if (id == "" || id == null)
            {
                return Content("<script >alert('请在工程信息列表中选择一项记录');window.history.back();</script >");
            }
            int ID = int.Parse(id.Trim());
            var PlanProjectSeq = from a in db.vw_PlanProjectBoxList
                              where a.projectID == ID
                              select a.seqNo;
            if (PlanProjectSeq.Count() == 0)
            {
                return Content("<script >alert('该案卷内没有工程记录');window.history.back();</script >");
            }
            int seqNo = Convert.ToInt32(PlanProjectSeq.First());
            var project = from b in db.vw_PlanProjectBoxList
                          where b.seqNo == seqNo
                          orderby b.juanneiSeqNo
                          select b;
            if(project.Count()==0)
            {
                return Content("<script >alert('该案卷内没有工程记录');window.history.back();</script >");
            }
            ViewBag.result1 = JsonConvert.SerializeObject(project);//数据转换成JSON后传给前台
            return View();
        }
        public ActionResult PlanArchiveInProject2(string id)
        {
            if (id == "" || id == null)
            {
                return Content("<script >alert('请在工程信息列表中选择一项记录');window.history.back();</script >");
            }
            int ID = int.Parse(id.Trim());
            var PlanProjectSeq = from a in db.vw_PlanProjectBoxList
                                 where a.ID == ID
                                 select a.seqNo;
            if (PlanProjectSeq.Count() == 0)
            {
                return Content("<script >alert('该案卷内没有工程记录');window.history.back();</script >");
            }
            int seqNo = Convert.ToInt32(PlanProjectSeq.First());
            var project = from b in db.vw_PlanProjectBoxList
                          where b.seqNo == seqNo
                          orderby b.juanneiSeqNo
                          select b;
            if (project.Count() == 0)
            {
                return Content("<script >alert('该案卷内没有工程记录');window.history.back();</script >");
            }
            ViewBag.result1 = JsonConvert.SerializeObject(project);//数据转换成JSON后传给前台
            return View();
        }
        //public ActionResult PlanArchiveIinfoList()
        //{

        //}
       public ActionResult LookVideoArchives(string SerachText)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source="+ System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count From UrbanCon.dbo.VideoArchives where videoStatus=4 and   ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachText;
            int flag = 0;
            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1= "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);
                //if (split.Length == 3)
                //{
                //    Require = split[0];
                //    if ((Require == "案卷题名" || Require == "项目顺序号" || Require == "拍摄地点") && flag == 0)
                //    {
                //        sql = "SELECT count(*) as count FROM UrbanCon.dbo.tem_vwVideoProjectList c  where c.videoStatus=4 and c.videoProjectSeqNo  in ( select a.videoProjectSeqNo from UrbanCon.dbo.VideoCassette a where a.";
                //        flag = 1;
                //    }
                //    if ((Require == "合同名称" || Require == "移交单位" || Require == "签订日期" || Require == "计划开工日期" || Require == "计划竣工日期" || Require == "负责人" || Require == "联系人" || Require == "合同联系电话") && flag == 0)
                //    {
                //        sql = "SELECT count(*) as count FROM UrbanCon.dbo.vw_VideoProjectList c  where c.videoStatus=4 and c.contractNo  in ( select a.contractNo from UrbanCon.dbo.ContractInfo a where a.";
                //        flag = 1;
                //    }
                //}
                //else
                //{
                //    Require = split[1];
                //    if ((Require == "案卷题名" || Require == "项目顺序号" || Require == "拍摄地点") && flag == 0)
                //    {
                //        sql = "SELECT count(*) as count FROM UrbanCon.dbo.tem_vwVideoProjectList c  where c.videoStatus=4 and c.videoProjectSeqNo  in ( select a.videoProjectSeqNo from UrbanCon.dbo.VideoCassette a where a.";
                //        flag = 1;
                //    }
                //    if ((Require == "合同名称" || Require == "移交单位" || Require == "签订日期" || Require == "计划开工日期" || Require == "计划竣工日期" || Require == "负责人" || Require == "联系人" || Require == "合同联系电话") && flag == 0)
                //    {
                //        sql = "SELECT count(*) as count FROM UrbanCon.dbo.vw_VideoProjectList c  where c.videoStatus=4 and c.contractNo  in ( select a.contractNo from UrbanCon.dbo.ContractInfo a where a.";
                //        flag = 1;
                //    }
                //}
                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.VideoArchiveListChinese
                               where b.Chinese == Require
                               select b.TableName;
                    Require = name.First();
                    Optertitle = split[1];
                    Content1 = split[2];
                }
                else
                {
                    Require = split[1];
                    var name = from c in db.VideoArchiveListChinese
                               where c.Chinese == Require
                               select c.TableName;
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
                    if (Require == "videoProjectSeqNo" || Require == "sheetNo" || Require == "buildingArea" || Require == "dateSigned" || Require == "PlanningStartDate" || Require == "PlanningEndDate")
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
                    if (Require == "videoProjectSeqNo" || Require == "sheetNo" || Require == "buildingArea" || Require == "dateSigned" || Require == "PlanningStartDate" || Require == "PlanningEndDate")
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
                    if (Require == "videoProjectSeqNo" || Require == "sheetNo" || Require == "buildingArea" || Require == "dateSigned" || Require == "PlanningStartDate" || Require == "PlanningEndDate")
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

                    if (Require == "videoProjectSeqNo" || Require == "sheetNo" || Require == "buildingArea" || Require == "dateSigned" || Require == "PlanningStartDate" || Require == "PlanningEndDate")
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
            //    return Content("<script >alert('检索格式不正确，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
            ////}
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
        public string LookVideoArchivesData(string SerachText,int? page)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT ID,SheetID,videoProjectSeqNo,projectName,location,operater,dateReceived  From UrbanCon.dbo.VideoArchives where videoStatus=4 and   ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachText;
            int flag = 0;
            for (int i = 0; i < count; i++)
            {

                string AndOr = "", Require = "", Optertitle = "", Content1 = "";
                string[] split = str[i].Split(new string[] { "——" }, StringSplitOptions.None);
                //if (split.Length == 3)
                //{
                //    Require = split[0];
                //    if ((Require == "案卷题名" || Require == "项目顺序号" || Require == "拍摄地点") && flag == 0)
                //    {
                //        sql = "SELECT count(*) as count FROM UrbanCon.dbo.tem_vwVideoProjectList c  where c.videoStatus=4 and c.videoProjectSeqNo  in ( select a.videoProjectSeqNo from UrbanCon.dbo.VideoCassette a where a.";
                //        flag = 1;
                //    }
                //    if ((Require == "合同名称" || Require == "移交单位" || Require == "签订日期" || Require == "计划开工日期" || Require == "计划竣工日期" || Require == "负责人" || Require == "联系人" || Require == "合同联系电话") && flag == 0)
                //    {
                //        sql = "SELECT count(*) as count FROM UrbanCon.dbo.vw_VideoProjectList c  where c.videoStatus=4 and c.contractNo  in ( select a.contractNo from UrbanCon.dbo.ContractInfo a where a.";
                //        flag = 1;
                //    }
                //}
                //else
                //{
                //    Require = split[1];
                //    if ((Require == "案卷题名" || Require == "项目顺序号" || Require == "拍摄地点") && flag == 0)
                //    {
                //        sql = "SELECT count(*) as count FROM UrbanCon.dbo.tem_vwVideoProjectList c  where c.videoStatus=4 and c.videoProjectSeqNo  in ( select a.videoProjectSeqNo from UrbanCon.dbo.VideoCassette a where a.";
                //        flag = 1;
                //    }
                //    if ((Require == "合同名称" || Require == "移交单位" || Require == "签订日期" || Require == "计划开工日期" || Require == "计划竣工日期" || Require == "负责人" || Require == "联系人" || Require == "合同联系电话") && flag == 0)
                //    {
                //        sql = "SELECT count(*) as count FROM UrbanCon.dbo.vw_VideoProjectList c  where c.videoStatus=4 and c.contractNo  in ( select a.contractNo from UrbanCon.dbo.ContractInfo a where a.";
                //        flag = 1;
                //    }
                //}
                //判断字符中是否存在与与或的判断符
                if (split.Length == 3)
                {
                    Require = split[0];
                    var name = from b in db.VideoArchiveListChinese
                               where b.Chinese == Require
                               select b.TableName;
                    Require = name.First();
                    Optertitle = split[1];
                    Content1 = split[2];
                }
                else
                {
                    Require = split[1];
                    var name = from c in db.VideoArchiveListChinese
                               where c.Chinese == Require
                               select c.TableName;
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
            sql += " order by videoProjectSeqNo";

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
            List<VideoArchives> prolist = new List<VideoArchives>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                VideoArchives vw_project = new VideoArchives();
                vw_project.ID = Convert.ToInt32(dr["ID"]);
                vw_project.SheetID = Convert.ToInt32(dr["SheetID"]);
                vw_project.videoProjectSeqNo = Convert.ToInt32(dr["videoProjectSeqNo"]);
                vw_project.projectName = dr["projectName"].ToString();
                vw_project.location = dr["location"].ToString();
                vw_project.operater = dr["operater"].ToString();
                if(dr["dateReceived"]==DBNull.Value)
                {
                    vw_project.dateReceived = Convert.ToDateTime("2017.10.16");
                }
                else
                {
                  vw_project.dateReceived = Convert.ToDateTime(dr["dateReceived"]);
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
                        new JProperty("ID", p.ID),
                        new JProperty("SheetID", p.SheetID),
                        new JProperty("videoProjectSeqNo", p.videoProjectSeqNo),
                        new JProperty("projectName", p.projectName),
                        new JProperty("location", p.location),
                        new JProperty("operater", p.operater),
                        new JProperty("dateReceived", p.dateReceived)
                                      )


               )
            )

).ToString();
            return d;
        }
        public ActionResult VideoprojectInfo(string id)
        {
            if (id == null||id=="")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int ID = Int32.Parse(id);
            VideoArchives videoArchives = dg.VideoArchives.Find(ID);
            //工作联系单信息
            VideoContractSheet videoContractSheet = dg.VideoContractSheet.Find(videoArchives.SheetID);
            ViewBag.sheetNo = videoContractSheet.sheetNo;
            ViewBag.fillDate = videoContractSheet.fillDate;
            ViewBag.developmentOrgnization = videoContractSheet.developmentOrgnization;
            ViewBag.location = videoContractSheet.location;
            ViewBag.buildingArea = videoContractSheet.buildingArea;
            ViewBag.projectResponsible = videoContractSheet.projectResponsible;

            ViewBag.fzryij = "经审核，该工程声像竣工档案接收情况属实，声像科拟签发《建设工程声像竣工档案验收意见书》，请批示！";
            if (videoArchives == null)
            {
                return HttpNotFound();
            }
            return View(videoArchives);
        }
        public ActionResult VideoArchiveAlljilu(string id)
        {
            if (id == null || id == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int ID = Int32.Parse(id);
            var paperseqNo = from a in dg.VideoArchives
                             where a.ID == ID
                             select a.videoProjectSeqNo;
            if(paperseqNo.Count()==0)
            {
                return Content("<script >alert('该工程没有案卷记录记录');window.history.back();</script >");
            }
            long paperNo = paperseqNo.First();
            var videoArchive = dg.VideoArchives.Where(x => x.videoProjectSeqNo == paperNo);
            ViewBag.projectName = videoArchive.FirstOrDefault().projectName;
            ViewBag.videoProjectSeqNo =paperNo;
            var videoCassetteList = dg.VideoCassetteList.Where(x => x.ProjectIDS == paperNo.ToString());
            string status = videoArchive.FirstOrDefault().videoStatus;
            if (status == "2")
            {
                ViewBag.fanhui = "Index_shenhe";
            }
            else if (status == "3")
            {
                ViewBag.fanhui = "Index_dairuku";//待入库就是已上传完毕文件
            }
            else if (status == "-1")
            {
                ViewBag.fanhui = "Index_bohui";
            }
            else
            {
                ViewBag.fanhui = "Index_zanruku";
            }

            return View(videoCassetteList.ToList());
        }
        public ActionResult playvideo(int? id)
        {
            if (id == null)
            {
                return Content("<script >alert('路径不存在！');window.history.back();</script >");
            }
            string result = "";
            var videoCassetteList = dg.VideoCassetteList.Find(id);
            string ProjectIDS = videoCassetteList.ProjectIDS.ToString().Trim();
            string filingDesc = dg.VideoCassetteList.Find(id).filingDesc.ToString().Trim();
            string xdlujing = "/声像资料/" + ProjectIDS + "/声像视频/" + filingDesc + "/";
            string folderFullName = Server.MapPath("~/声像资料/" + ProjectIDS + "/声像视频/" + filingDesc + "/");//文件存储路径
            //string folderFullName = Server.MapPath("~/声像视频/" + ProjectIDS + "/" + filingDesc + "/");//文件存储路径
            if (!Directory.Exists(folderFullName))
            {
                return Content("<script >alert('尚未提交视频文件！！');window.history.back();</script >");
            }
            DirectoryInfo TheFolder = new DirectoryInfo(folderFullName);
            foreach (System.IO.FileInfo NextFile in TheFolder.GetFiles())
                result = result + xdlujing + NextFile.Name + ";";
            ViewBag.result = result;//路径下照片文件序列传给前台

            //传给前台，显示工程信息等
            int videoProjectSeqNo = int.Parse(videoCassetteList.ProjectIDS);
            var videoArchive = dg.VideoArchives.Where(x => x.videoProjectSeqNo == videoProjectSeqNo);
            ViewBag.projectName = videoArchive.FirstOrDefault().projectName;
            ViewBag.videoProjectSeqNo = videoProjectSeqNo;
            ViewBag.filingDesc = videoCassetteList.filingDesc;
            ViewBag.videoContent = videoCassetteList.videoContent;
            return View();
        }
        public ActionResult lookphoto(int? id)
        {
            if (id == null)
            {
                return Content("<script >alert('路径不存在！');window.history.back();</script >");
            }
            string result = "";
            var videoCassetteList = dg.VideoCassetteList.Find(id);
            string ProjectIDS = videoCassetteList.ProjectIDS.ToString().Trim();
            string filingDesc = dg.VideoCassetteList.Find(id).filingDesc.ToString().Trim();
            string xdlujing = "/声像资料/" + ProjectIDS + "/声像照片/" + filingDesc + "/";
            string folderFullName = Server.MapPath("~/声像资料/" + ProjectIDS + "/声像照片/" + filingDesc + "/");//文件存储路径
                                                                                                         // string xdlujing = "/声像照片/" + ProjectIDS + "/" + filingDesc + "/";
                                                                                                         // string folderFullName = Server.MapPath("~/声像照片/" + ProjectIDS + "/" + filingDesc + "/");//文件存储路径
            if (!Directory.Exists(folderFullName))
            {
                return Content("<script >alert('尚未提交照片文件！！');window.history.back();</script >");
            }
            DirectoryInfo TheFolder = new DirectoryInfo(folderFullName);
            foreach (System.IO.FileInfo NextFile in TheFolder.GetFiles())
                result = result + xdlujing + NextFile.Name + ";";
            ViewBag.result = result;//路径下照片文件序列传给前台

            //传给前台，显示工程信息等
            int videoProjectSeqNo = int.Parse(videoCassetteList.ProjectIDS);
            var videoArchive = dg.VideoArchives.Where(x => x.videoProjectSeqNo == videoProjectSeqNo);
            ViewBag.projectName = videoArchive.FirstOrDefault().projectName;
            ViewBag.videoProjectSeqNo = videoProjectSeqNo;
            ViewBag.filingDesc = videoCassetteList.filingDesc;
            ViewBag.videoContent = videoCassetteList.videoContent;
            return View();
        }
      public  string LookQingzhaosaomiaopost(string name1,string[] arr1, string Local, string userid)
        {
            string flag = "0";
            if (name1 == "Add")
            {
                if (arr1.Length!= 0)
                {
                  
                    if (arr1.Length==1)
                    {
                        flag = saveImageAndArchives(arr1[0].ToString().Trim(), userid,Local);
                    }
                    else
                    {
                        string[] paths = arr1;
                        foreach (string s in paths)
                        {
                            flag = saveImageAndArchives(s,userid,Local);
                        }
                    }
                }
                else
                {

                    flag = "5";
                }
            }
            if (name1 == "Adddown") {
                if (arr1.Length != 0)
                {

                    if (arr1.Length == 1)
                    {
                        flag = saveImageAndArchives2(arr1[0].ToString().Trim(), userid, Local);
                    }
                    else
                    {
                        string[] paths = arr1;
                        foreach (string s in paths)
                        {
                            flag = saveImageAndArchives2(s, userid, Local);
                        }
                    }
                }
                else
                {

                    flag = "5";
                }
            }
            if (name1 == "Delete")
            {
                FileOperationBLL bll = new FileOperationBLL();

                string path = string.Empty;
            
                if (arr1.Length==1)
                {
                    path = bll.GetVirtualDirectory("80", arr1[0].ToString().Trim());
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);


                    }
                }
                else
                {
                    string[] paths = arr1;
                    foreach (string p in paths)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);

                        }
                    }
                }
                //"扫描件删除成功";
                flag = "7";
            }

          
            return JsonConvert.SerializeObject(flag);
        }
  protected string saveImageAndArchives(string filePathWeb, string userid, string Local)
    {
        FileOperationBLL bll = new FileOperationBLL();
      
        DateTime date = DateTime.Now.Date;
        if (userid.Trim() == "" || userid.Trim() == "undefined")
            {


                //"您尚未选择借阅人";
                return "1";

            }
            long ID = long.Parse(userid.Trim());
            var borModel = from a in db.BorrowRegistration
                           where a.ID == ID
                           select a;
            string strUserId = string.Empty;
            if (borModel.Count() != 0)
            {
                strUserId = borModel.First().seqNo.Trim();//实际为收费编号
                borModel.First().isJiesuanFee = 2;
                db.Entry(borModel.First()).State = EntityState.Modified;

            }
            else
            {
                // "您尚未登记，请先登记";
                return "2";
            }

            string archiveNo = getArchiveNo(Local);
            if(archiveNo=="6")
            {
                return "6";
            }

            var buaSet = from b in db.BindUserAndArchives
                         where b.userID == ID && b.archiveNo == archiveNo
                         orderby b.userID
                         select b;
            if (buaSet.Count() == 0)
            {

                BindUserAndArchives buaaModel = new BindUserAndArchives();
                buaaModel.archiveNo = archiveNo;
                buaaModel.bindDate = date;
                buaaModel.binder = User.Identity.Name;
                buaaModel.type = 5;
                buaaModel.userID = ID;
                db.BindUserAndArchives.Add(buaaModel);

            }
            var BUAImodel = from c in db.BindUserAndImage
                            where c.userID == strUserId && c.imageTime == date && c.ImageAddress == filePathWeb
                            select c;
         if (BUAImodel.Count()==0)
        {
                BindUserAndImage BUAI = new BindUserAndImage();
                BUAI.imageTime = date;
                BUAI.userID = strUserId;
                BUAI.ImageAddress = bll.GetWebPathInDb(filePathWeb);
             
                BUAI.archivesNo = "";
                BUAI.realuserID = int.Parse(userid.Trim());


                db.BindUserAndImage.Add(BUAI);

                db.SaveChanges();


                // "保存成功";
                return "3";
            }
        else
        {
                // "此图纸已保存";
                return "4";
            }
    }
        protected string saveImageAndArchives2(string filePathWeb, string userid, string Local)
        {
            FileOperationBLL bll = new FileOperationBLL();

            DateTime date = DateTime.Now.Date;
            if (userid.Trim() == "" || userid.Trim() == "undefined")
            {
                //"您尚未选择借阅人";
                return "1";
            }
            long ID = long.Parse(userid.Trim());
            var borModel = from a in db.BorrowRegistration
                           where a.ID == ID
                           select a;
            string strUserId = string.Empty;
            if (borModel.Count() != 0)
            {
                strUserId = borModel.First().seqNo.Trim();//实际为收费编号
                borModel.First().isJiesuanFee = 2;
                db.Entry(borModel.First()).State = EntityState.Modified;
            }
            else
            {
                // "您尚未登记，请先登记";
                return "2";
            }
            string archiveNo = getArchiveNo(Local);
            if (archiveNo == "6")
            {
                return "6";
            }
            var BUAImodel = from c in db.BindUserAndImageDown
                            where c.userID == strUserId && c.imageTime == date && c.ImageAddress == filePathWeb
                            select c;
            if (BUAImodel.Count() == 0)
            {
                BindUserAndImageDown BUAI = new BindUserAndImageDown();
                BUAI.imageTime = date;
                BUAI.userID = strUserId;
                BUAI.ImageAddress = bll.GetWebPathInDb(filePathWeb);
                BUAI.archivesNo = archiveNo;
                BUAI.realuserID = int.Parse(userid.Trim());
                db.BindUserAndImageDown.Add(BUAI);
                db.SaveChanges();
                // "保存成功";
                return "3";
            }
            else
            {
                // "此图纸已保存";
                return "4";
            }
        }
        private string getArchiveNo(string Local)
        {
            int flag = -1;
            string[] paths = Local.Trim().Split('/');
            switch (paths[0].Trim())
            {
                case "License": flag = 0; break;//执照档案
                case "Road": flag = 1; break;//道路档案
                case "Classify": flag = 2; break;//分类档案
                case "Tuzhi": flag = 3;break;//图纸档案
                case "PlanArchives":flag = 4;break;//规划档案
            }
            //License/1985/1985-00001
            string archiveNo = string.Empty;
            if (flag == 0)
            {
                if (paths.Length == 4)
                {
                    while (paths[3].Length != 3)
                    {
                        paths[3] = "0"+ paths[3];
                    }
                    archiveNo = paths[2]+"-"+paths[3];
                }
                else
                {
                    archiveNo = /*Request.QueryString["rid"] + "/" +*/ paths[2];//不知道rid是什么
                }
              
            }
            if (flag == 1)
            {
                if (paths.Length == 4)
                {
                    while (paths[3].Length != 3)
                    {
                        paths[3] = "0" + paths[3];
                    }
                    archiveNo = paths[2] + "-" + paths[3];
                }
                else
                {
                    archiveNo = /*Request.QueryString["rid"] + "/" +*/ paths[2];//不知道rid是什么
                }
            }
            if (flag == 2)
            {
                archiveNo = /*Request.QueryString["rid"] + "/" +*/ paths[1];
            }
            if (flag == 3)
            {
                archiveNo = /*Request.QueryString["rid"] + "/" + */paths[1];
            }
            if (flag == 4)
            {
                archiveNo = /*Request.QueryString["rid"] + "/" + */paths[1];
            }
            if (archiveNo == string.Empty)
            {
              
                return "6";
            }
            return archiveNo;
        }
        public string LookPlansaomiaopost(string name1, string[] arr1, string Local, string userid)
        { 
            string flag = "0";
            if (name1 == "Add")
            {
                if (arr1.Length != 0)
                {

                    if (arr1.Length == 1)
                    {
                        flag = saveImageAndArchives1(arr1[0].ToString().Trim(), userid, Local);
                    }
                    else
                    {
                        string[] paths = arr1;
                        foreach (string s in paths)
                        {
                            flag = saveImageAndArchives1(s, userid, Local);
                        }
                    }
                }
                else
                {

                    flag = "5";
                }
            }
            if (name1 == "Adddown") {
                if (arr1.Length != 0)
                {
                    if (arr1.Length == 1)
                    {
                        flag = saveImageAndArchives3(arr1[0].ToString().Trim(), userid, Local);
                    }
                    else
                    {
                        string[] paths = arr1;
                        foreach (string s in paths)
                        {
                            flag = saveImageAndArchives3(s, userid, Local);
                        }
                    }
                }
                else
                {
                    flag = "5";
                }
            }
            if (name1 == "Delete")
            {
                FileOperationBLL bll = new FileOperationBLL();

                string path = string.Empty;

                if (arr1.Length == 1)
                {
                    path = bll.GetVirtualDirectory("80", arr1[0].ToString().Trim());
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);


                    }
                }
                else
                {
                    string[] paths = arr1;
                    foreach (string p in paths)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);

                        }
                    }
                }
                //"扫描件删除成功";
                flag = "7";
            }


            return JsonConvert.SerializeObject(flag);
        }
        protected string saveImageAndArchives1(string filePathWeb, string userid, string Local)
        {
            FileOperationBLL bll = new FileOperationBLL();

            DateTime date = DateTime.Now.Date;
            if (userid.Trim() == "" || userid.Trim() == "undefined")
            {


                //"您尚未选择借阅人";
                return "1";

            }
            long ID = long.Parse(userid.Trim());
            var borModel = from a in db.BorrowRegistration
                           where a.ID == ID
                           select a;
            string strUserId = string.Empty;
            if (borModel.Count() != 0)
            {
                strUserId = borModel.First().seqNo.Trim();//实际为收费编号
                borModel.First().isJiesuanFee = 2;
                db.Entry(borModel.First()).State = EntityState.Modified;

            }
            else
            {
                // "您尚未登记，请先登记";
                return "2";
            }

            string archiveNo = getArchiveNo1(Local);
            if (archiveNo == "6")
            {
                return "6";
            }

            var buaSet = from b in db.BindUserAndArchives
                         where b.userID == ID && b.archiveNo == archiveNo
                         orderby b.userID
                         select b;
            if (buaSet.Count() == 0)
            {

                BindUserAndArchives buaaModel = new BindUserAndArchives();
                buaaModel.archiveNo = archiveNo;
                buaaModel.bindDate = date;
                buaaModel.binder = User.Identity.Name;
                buaaModel.type = 5;
                buaaModel.userID = ID;
                db.BindUserAndArchives.Add(buaaModel);

            }
            var BUAImodel = from c in db.BindUserAndImage
                            where c.userID == strUserId && c.imageTime == date && c.ImageAddress == filePathWeb
                            select c;
            if (BUAImodel.Count() == 0)
            {
                BindUserAndImage BUAI = new BindUserAndImage();
                BUAI.imageTime = date;
                BUAI.userID = strUserId;
                BUAI.ImageAddress = bll.GetWebPathInDb(filePathWeb);

                BUAI.archivesNo = "";
                BUAI.realuserID = int.Parse(userid.Trim());


                db.BindUserAndImage.Add(BUAI);

                db.SaveChanges();


                // "保存成功";
                return "3";
            }
            else
            {
                // "此图纸已保存";
                return "4";
            }
        }
        protected string saveImageAndArchives3(string filePathWeb, string userid, string Local)
        {
            FileOperationBLL bll = new FileOperationBLL();

            DateTime date = DateTime.Now.Date;
            if (userid.Trim() == "" || userid.Trim() == "undefined")
            {
                //"您尚未选择借阅人";
                return "1";
            }
            long ID = long.Parse(userid.Trim());
            var borModel = from a in db.BorrowRegistration
                           where a.ID == ID
                           select a;
            string strUserId = string.Empty;
            if (borModel.Count() != 0)
            {
                strUserId = borModel.First().seqNo.Trim();//实际为收费编号
                borModel.First().isJiesuanFee = 2;
                db.Entry(borModel.First()).State = EntityState.Modified;
            }
            else
            {
                // "您尚未登记，请先登记";
                return "2";
            }

            string archiveNo = getArchiveNo1(Local);
            if (archiveNo == "6")
            {
                return "6";
            }
            var BUAImodel = from c in db.BindUserAndImageDown
                            where c.userID == strUserId && c.imageTime == date && c.ImageAddress == filePathWeb
                            select c;
            if (BUAImodel.Count() == 0)
            {
                BindUserAndImageDown BUAI = new BindUserAndImageDown();
                BUAI.imageTime = date;
                BUAI.userID = strUserId;
                BUAI.ImageAddress = bll.GetWebPathInDb(filePathWeb);
                BUAI.archivesNo = archiveNo;
                BUAI.realuserID = int.Parse(userid.Trim());
                db.BindUserAndImageDown.Add(BUAI);
                db.SaveChanges();
                // "保存成功";
                return "3";
            }
            else
            {
                // "此图纸已保存";
                return "4";
            }
        }
        private string getArchiveNo1(string Local)
        {
          
            string archiveNo = "";
            string[] paths = Local.Trim().Split('/');
          
                archiveNo =paths[1];
     
            if (archiveNo == string.Empty)
            {

                return "6";
            }
            return archiveNo;
        }
        public ActionResult TuzhiArchivesView1()
        {
            return View();
        }
        public ActionResult PlanProjView1()
        {
           return View();
        } 
        public ActionResult LookGuanXianFinshProject(string SerachText)
        {

        
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider=SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count FROM UrbanCon.dbo.tem_vwgxprojectList  where status='7' and ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachText;
            int flag = 0;//只能进行一次选表判断
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
                if ((Require == "licenseNo" || Require == "archivesTitle" || Require == "archivesNo" || Require == "kaigongTime" || Require == "jungongTime" || Require == "shizhengNo" || Require == "isImageExis") && flag == 0)
                {

                    sql = "SELECT count(*) as count FROM UrbanCon.dbo.tem_vwgxprojectList a where a.status='7' and a.paperProjectSeqNo in ( select paperProjectSeqNo from UrbanCon.dbo.gxArchivesDetail c where c.";
                    flag = 1;
                }
                if ((Require == "partBLegalRepresent" || Require == "partBweituoAgent" || Require == "partBcontactTel") && flag == 0)
                {

                    sql = "SELECT count(*) as count FROM UrbanCon.dbo.tem_vwgxprojectList a where a.status='7' and a.contractNo in ( select contractNo from UrbanCon.dbo.gxContractInfo c where c.";
                    flag = 1;
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
                    if (Require == "buildingArea" || Require == "licenseDate" || Require == "kaigongTime" || Require == "jungongTime" || Require == "paperProjectSeqNo")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.history.back();</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索条件非法，请重新输入！');window.history.back();</script >");
                    }



                    sql += Require + ">" + Content1;

                }
                if (Optertitle == "小于")
                {
                    if (Require == "buildingArea" || Require == "licenseDate" || Require == "kaigongTime" || Require == "jungongTime" || Require == "paperProjectSeqNo")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法,请重新输入！');window.history.back();</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索条件非法，请重新输入！');window.history.back();</script >");
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
                    if (Require == "buildingArea" || Require == "licenseDate" || Require == "kaigongTime" || Require == "jungongTime" || Require == "paperProjectSeqNo")
                    {
                        if ((Regex.IsMatch(su[0], "^([0-9]{1,})$") == true) && (Regex.IsMatch(su[1], "^([0-9]{1,})$") == true))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.history.back();</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索条件非法，请重新输入！');window.history.back();</script >");
                    }

                    sql += Require + " between " + su[0] + " and " + su[1];
                }
                if (Optertitle == "不等于")
                {

                    if (Require == "buildingArea" || Require == "licenseDate" || Require == "kaigongTime" || Require == "jungongTime" || Require == "paperProjectSeqNo")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.history.back();</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索条件非法，请重新输入！');window.history.back();</script >");
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
                return Content("<script >alert('检索格式不正确，请重新输入！');window.history.back();</script >");
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
        public string LookGuanXianFinshProjectData(string SerachText, int? page)
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
            string sql = "SELECT paperProjectSeqNo,archivesCount,originalInchCount,projectName,location,licenseNo,developmentOrganization,jgDate,luruTime,textMaterial,drawing,photoCount,transferUnit,startArchiveNo,endArchiveNo,projectID FROM UrbanCon.dbo.tem_vwgxprojectList  where status='7' and ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            ViewBag.coent = SerachText;
            int flag = 0;//只能进行一次选表判断
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
                    Content1 = split[2].ToString().Trim();
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
                    Content1 = split[3].ToString().Trim();
                }
                //判断来自哪个表
                if ((Require == "licenseNo" || Require == "archivesTitle" || Require == "archivesNo" || Require == "kaigongTime" || Require == "jungongTime" || Require == "shizhengNo" || Require == "isImageExis") && flag == 0)
                {

                    sql = "SELECT paperProjectSeqNo,archivesCount,originalInchCount,projectName,location,licenseNo,developmentOrganization,jgDate,luruTime,textMaterial,drawing,photoCount,transferUnit,startArchiveNo,endArchiveNo,projectID FROM UrbanCon.dbo.tem_vwgxprojectList a where a.status='7' and a.paperProjectSeqNo in ( select paperProjectSeqNo from UrbanCon.dbo.gxArchivesDetail c where c.";
                    flag = 1;
                }
                if ((Require == "partBLegalRepresent" || Require == "partBweituoAgent" || Require == "partBcontactTel") && flag == 0)
                {

                    sql = "SELECT paperProjectSeqNo,archivesCount,originalInchCount,projectName,location,licenseNo,developmentOrganization,jgDate,luruTime,textMaterial,drawing,photoCount,transferUnit,startArchiveNo,endArchiveNo,projectID FROM UrbanCon.dbo.tem_vwgxprojectList a where a.status='7' and a.contractNo in ( select contractNo from UrbanCon.dbo.gxContractInfo c where c.";
                    flag = 1;
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
            List<tem_vwprojectList> prolist = new List<tem_vwprojectList>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                tem_vwprojectList vw_project = new tem_vwprojectList();
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
                vw_project.licenseNo = dr["licenseNo"].ToString();
                vw_project.developmentOrganization = dr["developmentOrganization"].ToString();
                if (dr["jgDate"]!= DBNull.Value)
                {
                    vw_project.jgDate = Convert.ToDateTime(dr["jgDate"]);
                }
             
                vw_project.luruTime = dr["luruTime"].ToString();
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
                vw_project.transferUnit = dr["transferUnit"].ToString();
                //vw_project.constructionOrganization = dr["constructionOrganization"].ToString();
                //vw_project.disignOrganization = dr["disignOrganization"].ToString();
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
                        new JProperty("drawing", p.drawing),
                        new JProperty("PhotoCount", p.PhotoCount),
                        new JProperty("transferUnit", p.transferUnit),
                        //new JProperty("constructionOrganization", p.constructionOrganization),
                        //new JProperty("disignOrganization", p.disignOrganization),
                        new JProperty("startArchiveNo", p.startArchiveNo),
                        new JProperty("endArchiveNo", p.endArchiveNo),
                        new JProperty("projectID", p.projectID)
                                      )


               )
            )

).ToString();
            return d;

        }
        public ActionResult LookGuanXianFinshArchives(string SerachText)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count FROM UrbanCon.dbo.tem_vwgxpassList where status='7' and ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            int flag = 0;//只能进行一次选表判断

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
                if ((Require == "partBLegalRepresent" || Require == "partBweituoAgent" || Require == "partBcontactTel") && flag == 0)
                {

                    sql = "SELECT count(*) as count FROM UrbanCon.dbo.dbo.tem_vwgxpassList  a where a.status='7' and a.contractNo in ( select contractNo from UrbanCon.dbo.gxContractInfo c where c.";
                    flag = 1;
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
                    if (Require == "buildingArea" || Require == "licenseDate" || Require == "kaigongTime" || Require == "jungongTime" || Require == "paperProjectSeqNo")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.history.back()';</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索条件非法，请重新输入！');window.history.back();</script >");
                    }



                    sql += Require + ">" + Content1;

                }
                if (Optertitle == "小于")
                {
                    if (Require == "buildingArea" || Require == "licenseDate" || Require == "kaigongTime" || Require == "jungongTime" || Require == "paperProjectSeqNo")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法,请重新输入！');window.history.back();</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索条件非法，请重新输入！');window.history.back();</script >");
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
                    if (Require == "buildingArea" || Require == "licenseDate" || Require == "kaigongTime" || Require == "jungongTime" || Require == "paperProjectSeqNo")
                    {
                        if ((Regex.IsMatch(su[0], "^([0-9]{1,})$") == true) && (Regex.IsMatch(su[1], "^([0-9]{1,})$") == true))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.history.back();</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索条件非法，请重新输入！');window.history.back();</script >");
                    }

                    sql += Require + " between " + su[0] + " and " + su[1];
                }
                if (Optertitle == "不等于")
                {

                    if (Require == "buildingArea" || Require == "licenseDate" || Require == "kaigongTime" || Require == "jungongTime" || Require == "paperProjectSeqNo")
                    {
                        if (Regex.IsMatch(Content1, "^([0-9]{1,})$"))
                        {

                        }
                        else
                        {
                            return Content("<script >alert('检索内容非法，请重新输入！');window.history.back();</script >");


                        }
                    }
                    else
                    {
                        return Content("<script >alert('检索条件非法，请重新输入！');window.history.back();</script >");
                    }
                    sql += Require + "!=" + "'" + Content1 + "'";
                }

            }
            if (System.Text.RegularExpressions.Regex.Matches(sql, @"\(").Count != System.Text.RegularExpressions.Regex.Matches(sql, @"\)").Count)
            {
                sql += ")";
            }
            //查询数据条数
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
                return Content("<script >alert('检索格式不正确，请重新输入！');window.history.back();</script >");
            }
            DataTable dt = ds.Tables[0];
            string count1 = dt.Rows[0]["count"].ToString();

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
        public string LookGuanXianFinshArchivesData(string SerachText, int? page)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT paperProjectSeqNo,volNo,isImageExist,archiveThickness,archivesTitle,licenseNo,archivesNo,registrationNo,typerDate,developmentUnit,constructionUnit,designUnit,transferUnit,textMaterial,drawing,photoCount FROM UrbanCon.dbo.tem_vwgxpassList where status='7' and ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            int flag = 0;//只能进行一次选表判断

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
                if ((Require == "partBLegalRepresent" || Require == "partBweituoAgent" || Require == "partBcontactTel") && flag == 0)
                {

                    sql = "SELECT paperProjectSeqNo,volNo,isImageExist,archiveThickness,archivesTitle,licenseNo,archivesNo,registrationNo,typerDate,developmentUnit,constructionUnit,designUnit,transferUnit,textMaterial,drawing,photoCount FROM UrbanCon.dbo.tem_vwgxpassList  a where a.status='7' and a.contractNo in ( select contractNo from UrbanCon.dbo.gxContractInfo c where c.";
                    flag = 1;
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
            sql += " order by paperProjectSeqNo,volNo";
            //查询数据条数
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
            List<tem_vwpassList> prolist = new List<tem_vwpassList>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                tem_vwpassList vw_project = new tem_vwpassList();
                vw_project.paperProjectSeqNo = Convert.ToInt32(dr["paperProjectSeqNo"]);

                if (dr["volNo"] == DBNull.Value)
                {
                    vw_project.volNo = 0;
                }
                else
                {
                    vw_project.volNo = Convert.ToInt32(dr["volNo"]);
                }
                vw_project.isImageExist = dr["isImageExist"].ToString();
                if (dr["archiveThickness"] == DBNull.Value)
                {
                    vw_project.archiveThickness = 0;
                }
                else
                {
                    vw_project.archiveThickness = Convert.ToInt32(dr["archiveThickness"]);
                }

                vw_project.archivesTitle = dr["archivesTitle"].ToString();
                vw_project.licenseNo = dr["licenseNo"].ToString();
                vw_project.archivesNo = dr["archivesNo"].ToString();
                vw_project.registrationNo = dr["registrationNo"].ToString();
                //vw_project.kaigongTime = dr["kaigongTime"].ToString();
                //vw_project.jungongTime = dr["jungongTime"].ToString();
                vw_project.archivesNo = dr["archivesNo"].ToString();
              
                vw_project.typerDate = Convert.ToDateTime(dr["typerDate"]);
                vw_project.developmentUnit = dr["developmentUnit"].ToString();
                vw_project.constructionUnit = dr["constructionUnit"].ToString(); ;
                vw_project.designUnit = dr["designUnit"].ToString(); ;
                vw_project.transferUnit = dr["transferUnit"].ToString(); ;


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
                    vw_project.photoCount = 0;
                }
                else
                {
                    vw_project.photoCount = Convert.ToInt32(dr["PhotoCount"]);
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
                        new JProperty("isImageExist", p.isImageExist),
                        new JProperty("archiveThickness", p.archiveThickness),
                        new JProperty("archivesTitle", p.archivesTitle),
                        new JProperty("licenseNo", p.licenseNo),
                        new JProperty("archivesNo", p.archivesNo),
                        new JProperty("registrationNo", p.registrationNo),
                        //new JProperty("kaigongTime", p.kaigongTime),
                        //new JProperty("jungongTime", p.jungongTime),
                        //new JProperty("jgDate", p.jgDate),
                        new JProperty("typerDate", p.typerDate),
                        new JProperty("developmentUnit", p.developmentUnit),
                        new JProperty("constructionUnit", p.constructionUnit),
                        new JProperty("designUnit", p.designUnit),
                        new JProperty("transferUnit", p.transferUnit),
                        new JProperty("textMaterial", p.textMaterial),
                        new JProperty("drawing", p.drawing),
                        new JProperty("photoCount", p.photoCount)

                                      )


               )
            )

).ToString();
            return d;

        }

        public ActionResult LookGuanXianFinshFiles(string SerachText)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count FROM UrbanCon.dbo.gxFileInfo where ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            int flag = 0;//只能进行一次选表判断

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
                //if ((Require == "partBLegalRepresent" || Require == "partBweituoAgent" || Require == "partBcontactTel") && flag == 0)
                //{

                //    sql = "SELECT count(*) as count FROM UrbanCon.dbo.vw_passList   a where a.status='7' and a.contractNo in ( select contractNo from UrbanCon.dbo.ContractInfo c where c.";
                //    flag = 1;
                //}
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
                    if (Require == "buildingArea" || Require == "licenseDate" || Require == "kaigongTime" || Require == "jungongTime" || Require == "paperProjectSeqNo")
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
                    if (Require == "buildingArea" || Require == "licenseDate" || Require == "kaigongTime" || Require == "jungongTime" || Require == "paperProjectSeqNo")
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
                    if (Require == "buildingArea" || Require == "licenseDate" || Require == "kaigongTime" || Require == "jungongTime" || Require == "paperProjectSeqNo")
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

                    if (Require == "buildingArea" || Require == "licenseDate" || Require == "kaigongTime" || Require == "jungongTime" || Require == "paperProjectSeqNo")
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
            //查询数据条数
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
                return Content("<script >alert('检索格式不正确，请重新输入！');window.location.href='/ArchiveSearch/ArchiveSearchMainWindow';</script >");
            }
            DataTable dt = ds.Tables[0];
            string count1 = dt.Rows[0]["count"].ToString();

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
        public string LookGuanXianFinshFilesData(string SerachText, int? page)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT seqNo,fileNo,fileName,responsible,type,startPageNo ,dengjihao FROM UrbanCon.dbo.gxFileInfo where ";
            string[] str = SerachText.Split(new string[] { "," }, StringSplitOptions.None);
            int count = str.Length - 1;
            int flag = 0;//只能进行一次选表判断

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
                //if ((Require == "partBLegalRepresent" || Require == "partBweituoAgent" || Require == "partBcontactTel") && flag == 0)
                //{

                //    sql = "SELECT paperProjectSeqNo,volNo,isImageExist,archiveThickness,archivesTitle,licenseNo,archivesNo,registrationNo,typerDate,developmentUnit,constructionUnit,designUnit,transferUnit,textMaterial,drawing,photoCount FROM UrbanCon.dbo.vw_passList  a where a.status='7' and a.contractNo in ( select contractNo from UrbanCon.dbo.ContractInfo c where c.";
                //    flag = 1;
                //}
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
            //sql += " order by paperProjectSeqNo,volNo";
            //查询数据条数
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
            List<Models.FileInfo> filelist = new List<Models.FileInfo>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Models.FileInfo file = new Models.FileInfo();
                file.fileNo = dr["fileNo"].ToString();
                file.fileName = dr["fileName"].ToString();
                file.responsible = dr["responsible"].ToString();
                file.type = dr["type"].ToString();
                file.startPageNo = dr["startPageNo"].ToString();
                if (dr["dengjihao"] == DBNull.Value)
                {
                    file.dengjihao = "";
                }
                else
                {
                    file.dengjihao = dr["dengjihao"].ToString();
                }
                filelist.Add(file);
            }
            var e = filelist.ToPagedList(pageNumber, pageSize);
            var d = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                         //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程

                         from p in e
                         select new JObject(
                            //new JProperty("paperProjectSeqNo", p.paperProjectSeqNo),
                            new JProperty("fileNo", p.fileNo),
                            new JProperty("fileName", p.fileName),
                            new JProperty("responsible", p.responsible),
                            new JProperty("type", p.type),
                            new JProperty("startPageNo", p.startPageNo),
                            new JProperty("dengjihao", p.dengjihao)
                            )
                        )
                      )
                   ).ToString();
            return d;
        }




        public ActionResult gxProjectInfoes(long? id, string id2)
        {
            if (id == null)
            {
                return Content("<script >alert('请在工程全部记录中选择一项工程');window.history.back();</script >");
            }
            if (id == 0)
            {
                return Content("<script >alert('请在工程全部记录中选择一项工程');window.history.back();</script >");
            }
            var projectinfo = from b in cb.vw_gxpassList
                              where b.projectID == id
                              orderby b.paperProjectSeqNo, b.paijiaNo
                              select b;
            if (projectinfo.Count() == 0)
            {
                return Content("<script >alert('请在工程全部记录中选择一项工程');window.history.back();</script >");
            }
            vw_gxpassList pro = projectinfo.First();
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
            ViewData["url"] = id2;//传登url，用于工程著录单返回


            return View(pro);


        }
        public ActionResult gxProjectanjuan(long? id, string id2)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var paperno = from b in cb.gxPaperArchives
                          where b.projectID == id
                          select b.paperProjectSeqNo;
            if (paperno.Count() == 0)
            {
                return Content("<script >alert('请在工程全部记录中选择一项工程');window.history.back();</script >");
            }
            long PaperSeqNo = paperno.First();

            var archive = from a in cb.gxArchivesDetail
                          where a.paperProjectSeqNo == PaperSeqNo
                          orderby a.volNo
                          select a;

            if (archive == null)
            {
                return HttpNotFound();
            }
            ViewData["url"] = id2;
            ViewData["url1"] = Request.Url.ToString().Trim();
            return View(archive);



        }
        public ActionResult gxanjuanzhuludan(string id3, string id4)
        {
            if (id3 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id3 == "")
            {
                return Content("<script >alert('请在案卷全部记录中选择一项案卷');window.history.back();</script >");
            }
            var projectinfo = from b in cb.vw_gxpassList
                              where b.registrationNo == id3
                              select b;
            long paperseq = projectinfo.First().paperProjectSeqNo;
            if (projectinfo.Count() == 0)
            {
                return Content("<script >alert('请在案卷全部记录中选择一项案卷');window.history.back();</script >");
            }
            string seq = paperseq.ToString().Trim();
            var project = from c in cb.vw_gxpassList
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
            ViewData["checkname1"] = projectinfo.First().registrationNo;
            return View(projectinfo.First());

        }
        public ActionResult gxJuanneimulu(string id1, string url1)
        {
            if (id1 == "" || id1 == null)
            {
                return Content("<script >alert('请在案卷全部记录中选择一项案卷');window.history.back();</script >");
            }
            var archNo = from b in cb.gxArchivesDetail
                         where b.registrationNo == id1
                         select b.archivesNo;
            string archiveN = archNo.First().Trim();
            var file = from a in cb.gxFileInfo
                       where a.archivesNo == archiveN
                       orderby a.seqNo
                       select a;
            ViewData["url1"] = url1;
            if (file.Count() == 0)
            {
                return Content("<script >alert('该案卷无卷内目录信息！');window.history.back();</script >");
            }
            return View(file.ToList());

        }
        public ActionResult gxJuanneimuluxinxi(string id)
        {
            if (id == "" || id == null)
            {
                return Content("<script >alert('请选择一个案卷文件！');window.history.back();</script >");
            }
            int ID = Int32.Parse(id);
            var file = from ad in cb.gxFileInfo
                       where (ad.ID == ID)
                       select ad;

            if (file.Count() == 0)
            {
                return Content("<script >alert('该案卷文件没有卷内目录信息！');window.history.back();</script >");
            }
            gxFileInfo fileinfo = file.First();
            return View(fileinfo);
        }
      
        public ActionResult gxAllprojectjilu(string registrationNo)
        {

            var paperno = from b in cb.gxArchivesDetail
                          where b.registrationNo == registrationNo
                          select b.paperProjectSeqNo;
            if (paperno.Count() == 0)
            {
                return Content("<script >alert('请在案卷全部记录中选择一卷案卷！');window.history.back();</script >");

            }
            long PaperSeqNo = paperno.First();

            var project = from a in cb.vw_gxprojectList
                          where a.paperProjectSeqNo == PaperSeqNo
                          select a;

            if (project.Count() == 0)
            {
                return Content("<script >alert('该案卷无工程信息！');window.history.back();</script >");
            }
            ViewData["volCount"] = project.Count();
            return View(project);

        }
        public ActionResult gxLooksaomiao(string paperProjectSeqNo, string ArchiveNo, string regNo, string volNo)
        {
            if (regNo != "")
            {
                var archiveM = from a in cb.gxArchivesDetail
                               where a.registrationNo == regNo
                               select a;
                if (archiveM.Count() == 0)
                {
                    return View();
                }
                else
                {
                    long paper = long.Parse(paperProjectSeqNo.Trim());
                    var paperM = from b in cb.gxPaperArchives
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
                        ViewData["VolCnt"] = paperM.First().originalVolumeCount.ToString();

                    }
                }
            }

            return View();
        }
        public string GetGuanXianPicListDemo(string paperSeqNo, string archiveNo, string volNo)
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
                //{
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["GuanXianPath"] + paperSeqNo + "\\" + archiveNo + "\\";
            //   string strPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + paperSeqNo + "\\" + volNo + "\\";
            strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["GuanXianWebPath"] + paperSeqNo + "/" + archiveNo + "/";
            //    string strWebPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongWebPath"] + paperSeqNo + "/" + volNo + "/";

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
            //else if (Directory.Exists(strPath2))
            //{
            //    //取得目录信息
            //    myDirInfo = new DirectoryInfo(strPath2);

            //    //获得文件信息
            //    arrFileInfo = myDirInfo.GetFiles();

            //    if (arrFileInfo.Length > 0)
            //    {
            //        foreach (System.IO.FileInfo myFile in arrFileInfo)
            //        {
            //            myDataRow = myTable.NewRow();

            //            myDataRow["Name"] = myFile.Name;

            //            myDataRow["WebPath"] = strWebPath2 + myFile.Name;

            //            myTable.Rows.Add(myDataRow);
            //        }
            //    }
            //}
            //}
            return JsonConvert.SerializeObject(myTable);
        }
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ArchiveSearch2/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ArchiveSearch2/Create
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

        // GET: ArchiveSearch2/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ArchiveSearch2/Edit/5
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

        // GET: ArchiveSearch2/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ArchiveSearch2/Delete/5
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
        private string getClassifyIDsByNames(string names)
        {
            string ids = "";
            int index = names.IndexOf(',');
            if (index == -1)
            {
                int ndex = names.IndexOf('-');
                ids = names.Substring(0, ndex);
            }
            else
            {
                string[] ClassNames = names.Split(',');
                foreach (string str in ClassNames)
                {
                    if(str==""||str==null)
                    {
                        break;
                    }
                    int ndex = str.IndexOf('-');
                    ids += str.Substring(0, ndex) + ",";
                }
                ids = ids.Substring(0, ids.Length - 1);
            }
            return ids;
        }
    }
}
