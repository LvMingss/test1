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
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Collections;
//using SoftPOS;
using System.Runtime.InteropServices;
using System.Web;
using SoftPOS;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

namespace urban_archive.Controllers
{
    public class MyNameTransfom : ICSharpCode.SharpZipLib.Core.INameTransform
    {
        #region INameTransform 成员
        public string TransformDirectory(string name)
        {
            return null;
        }
        public string TransformFile(string name)
        {
            return Path.GetFileName(name);
        }
        #endregion      
    }
    public class ArchiveSearchController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        private VideoArchiveEntities ac = new VideoArchiveEntities();
        private PlanArchiveEntities ae = new PlanArchiveEntities();
        private VideoArchiveEntities db_video = new VideoArchiveEntities();
        private gxArchivesContainer ag = new gxArchivesContainer();


        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ArchiveSearchMainWindow()
        {
            ViewData["pagename"] = "ArchiveSearch/ArchiveSearchMainWindow";
            return View();

        }
        public string ArchiveSearchMainWindowList(string name, string classNo)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (name == "FinshList")
            {
                List<SelectListItem> item = new List<SelectListItem> {
                new SelectListItem { Text = "案卷题名", Value = "0"},
                new SelectListItem { Text = "项目顺序号", Value = "1"},
                new SelectListItem { Text = "执照号", Value = "2"},
             
                new SelectListItem { Text = "工程地点", Value = "3"},
                new SelectListItem { Text = "最新工程地点", Value = "4"},
                new SelectListItem { Text = "档号", Value = "5"},
                new SelectListItem { Text = "分类号", Value = "6"},
              
                new SelectListItem { Text = "工程名称", Value = "7"},
                new SelectListItem { Text = "建设单位", Value = "8"},
                new SelectListItem { Text = "施工单位", Value = "9"},
                new SelectListItem { Text = "设计单位", Value = "10"},
                new SelectListItem { Text = "移交单位", Value = "11"},
                new SelectListItem { Text = "建筑面积", Value = "12"},
                new SelectListItem { Text = "有无扫描件", Value = "13"},
                new SelectListItem { Text = "市政档案号", Value = "14"},
                new SelectListItem { Text = "总登记号", Value = "15"},
                new SelectListItem { Text = "排架号", Value = "16"},
                new SelectListItem { Text = "文件材料题名(卷内)", Value = "17"},
                };
                items = item;

                //var model = (from a in db.ArchiveAttribute
                //             join b in db.FinishArchiveAttribute on a.codeID equals b.codeID
                //             select a).Union(from a in db.ArchiveAttribute
                //                             join c in db.ProjectInfoAttribute on a.codeID equals c.codeID
                //                             select a).Union(from a in db.ArchiveAttribute
                //                                             join d in db.ContractInfoAttribute on a.codeID equals d.codeID
                //                                             select a).OrderBy(a => a.ID);
                //foreach (var item in model)
                //{
                //    items.Add(new SelectListItem { Text = item.chinesename, Value = item.codeID.ToString() });
                //}
            }
            if (name == "QingzhaoList")
            {
                
                
                string str = classNo.Trim();
                string ids = getClassifyIDsByNames(str);
                if (ids == "1" || ids == "2")
                {
                    List<SelectListItem> item = new List<SelectListItem> {
                    new SelectListItem { Text = "申请单位(其他)", Value = "0"},
                    new SelectListItem { Text = "工程地点(其他)", Value = "1"},
                    new SelectListItem { Text = "最新工程地点(其他)", Value = "2"},
                    new SelectListItem { Text = "执照号(其他)", Value = "3"},
                    new SelectListItem { Text = "工程内容(其他)", Value = "4"},
                    new SelectListItem { Text = "年度序号(其他)", Value = "5"},
                    new SelectListItem { Text = "总登记号(其他)", Value = "6"},
                    new SelectListItem { Text = "区工程顺序号(其他)", Value = "7"},
                    };
                    items = item;
                }
                else {
                    var ds = from a in db.OtherArchiveAttribute
                             where a.ID > 0
                             orderby a.ID
                             select a;
                    foreach (var dr in ds)
                    {
                        string str1 = dr.classify.ToString().Trim();
                        bool flag = matchString(str1, ids);
                        if (flag == true)
                        {
                            items.Add(new SelectListItem { Text = dr.Description.ToString(), Value = dr.colName });
                        }
                    }
                }
                
                
            }
            if (name == "LookPlanList")
            {

                List<SelectListItem> item = new List<SelectListItem> {
                new SelectListItem { Text = "工程地点", Value = "0"},
                new SelectListItem { Text = "建设单位", Value = "1"},
                new SelectListItem { Text = "案卷题名", Value = "2"},
                new SelectListItem { Text = "文件编号", Value = "3"},
                new SelectListItem { Text = "案卷顺序号", Value = "4"},
                new SelectListItem { Text = "工程内容", Value = "5"},
                new SelectListItem { Text = "排架号", Value = "6"},
                new SelectListItem { Text = "盒号", Value = "7"},
                new SelectListItem { Text = "档号", Value = "8"},
                new SelectListItem { Text = "总登记号", Value = "9"},
                new SelectListItem { Text = "年度", Value = "10"},
                new SelectListItem { Text = "工程总顺序号", Value = "11"},
                };
                items = item;
            }

            if (name == "LookYeWu")
            {

                List<SelectListItem> item = new List<SelectListItem> {
                new SelectListItem { Text = "工程地点", Value = "0"},
                new SelectListItem { Text = "建设单位", Value = "1"},
                new SelectListItem { Text = "案卷题名", Value = "2"},
                new SelectListItem { Text = "文件编号", Value = "3"},
                new SelectListItem { Text = "案卷顺序号", Value = "4"},
                new SelectListItem { Text = "工程内容", Value = "5"},
                new SelectListItem { Text = "排架号", Value = "6"},
                new SelectListItem { Text = "盒号", Value = "7"},
                new SelectListItem { Text = "档号", Value = "8"},
                new SelectListItem { Text = "总登记号", Value = "9"},
                new SelectListItem { Text = "年度", Value = "10"},
                new SelectListItem { Text = "工程总顺序号", Value = "11"},
                };
                items = item;
            }

            if (name == "LookViedoList")
            {

                List<SelectListItem> item = new List<SelectListItem> {
                //new SelectListItem { Text = "案卷题名", Value = "0"},
                new SelectListItem { Text = "项目顺序号", Value = "1"},
                new SelectListItem { Text = "工程地点", Value = "2"},
                new SelectListItem { Text = "工程名称", Value = "3"},
                new SelectListItem { Text = "建设单位", Value = "4"},
                //new SelectListItem { Text = "声像联系单号", Value = "5"},
                //new SelectListItem { Text = "合同名称", Value = "6"},
                //new SelectListItem { Text = "拍摄地点", Value = "7"},
                new SelectListItem { Text = "移交单位", Value = "8"},
                new SelectListItem { Text = "建筑面积", Value = "9"},
                new SelectListItem { Text = "签订日期", Value = "10"},
                new SelectListItem { Text = "计划开工日期", Value = "11"},
                new SelectListItem { Text = "计划竣工日期", Value = "12"},
                new SelectListItem { Text = "负责人", Value = "13"},
                new SelectListItem { Text = "联系人", Value = "14"},
                new SelectListItem { Text = "合同联系电话", Value = "15"},

                };
                items = item;
            }
            if (name == "LookGuanXianList")
            {

                List<SelectListItem> item = new List<SelectListItem> {
                new SelectListItem { Text = "案卷题名", Value = "0"},
                new SelectListItem { Text = "项目顺序号", Value = "1"},
                new SelectListItem { Text = "工程地点", Value = "2"},
                new SelectListItem { Text = "档号", Value = "3"},
                new SelectListItem { Text = "分类号", Value = "4"},
                new SelectListItem { Text = "工程名称", Value = "5"},
                new SelectListItem { Text = "合同名称", Value = "6"},
                new SelectListItem { Text = "移交单位", Value = "7"},
                new SelectListItem { Text = "建筑面积", Value = "8"},
                new SelectListItem { Text = "录入日期", Value = "9"},
                new SelectListItem { Text = "计划开工日期", Value = "10"},
                new SelectListItem { Text = "计划竣工日期", Value = "11"},
                new SelectListItem { Text = "负责人", Value = "12"},
                new SelectListItem { Text = "联系人", Value = "13"},
                new SelectListItem { Text = "合同联系电话", Value = "14"},
                new SelectListItem { Text = "建设单位手机", Value = "15"},
                new SelectListItem { Text = "建设单位座机", Value = "16"},
                new SelectListItem { Text = "建设单位联系人", Value = "17"},
                new SelectListItem { Text = "施工单位联系人", Value = "18"},
                new SelectListItem { Text = "施工单位手机", Value = "19"},
                new SelectListItem { Text = "施工单位座机", Value = "20"},
                new SelectListItem { Text = "施工单位", Value = "21"},
                new SelectListItem { Text = "设计单位", Value = "22"},
                new SelectListItem { Text = "建设单位", Value = "23"},
                new SelectListItem { Text = "文件材料题名(卷内)", Value = "24"},
                };
                items = item;
            }
            if (name == "LookZhengDiList")
            {

                var model = from h in db.zdArchiveAttribute
                            select h;
                foreach (var item in model)
                {
                    items.Add(new SelectListItem { Text = item.Description, Value = item.colName });
                }
            }
            if (name == "LookYuanChuanList")
            {

                List<SelectListItem> item = new List<SelectListItem> {
                new SelectListItem { Text = "案卷题名", Value = "0"},
                new SelectListItem { Text = "工程名称", Value = "1"},
                new SelectListItem { Text = "工程地点", Value = "2"},
                new SelectListItem { Text = "建设单位", Value = "3"},
                new SelectListItem { Text = "施工单位", Value = "4"},

                new SelectListItem { Text = "项目顺序号", Value = "6"},
                new SelectListItem { Text = "档号", Value = "7"},


                };
                items = item;
            }
                                                                   return JsonConvert.SerializeObject(items);
        }
        [HttpPost]
        public ActionResult ArchiveSearchMainWindow(string action, string SearchArear, string qingzhaotype, string Plantype)
        {
            var MaxID = from a in db.BorrowRegistration
                        orderby a.ID descending
                        select a.ID;

            if (action == "查看当前用户绑定内容")
            {
                return RedirectToAction("ShowCurUserBangDing", "ArchiveSearch", new { id = MaxID.First() });
            }
            if (action == "查看请照档案")
            {
                return RedirectToAction("LookQingArchives", "ArchiveSearch", new { SerachText = SearchArear.Trim(), classNo = qingzhaotype.Trim() });
            }

            if (action == "查看竣工档案（卷内）")
            {
                return RedirectToAction("LookFinshFile", new { SerachText = SearchArear.Trim() });
            }

            if (action == "查看竣工档案（案卷）")
            {
                return RedirectToAction("LookFinshArchives", new { SerachText = SearchArear.Trim() });
            }
            if (action == "查看竣工档案（工程）")
            {

                return RedirectToAction("LookFinshProject", new { SerachText = SearchArear.Trim() });
            }

            if (action == "查看规划档案（工程)")
            {
                return RedirectToAction("LookPlanProject", "ArchiveSearch2", new { SerachText = SearchArear.Trim(), classNo2 = Plantype.Trim() });
            }
            if (action == "查看规划档案(案卷)")
            {
                return RedirectToAction("LookPlanArchives", "ArchiveSearch2", new { SerachText = SearchArear.Trim(), classNo2 = Plantype.Trim() });
            }
            if (action == "查看声像档案")
            {
                return RedirectToAction("LookVideoArchives", "ArchiveSearch2", new { SerachText = SearchArear.Trim() });
            }
            if (action == "查看管线档案（工程）")
            {
                return RedirectToAction("LookGuanXianFinshProject", "ArchiveSearch2", new { SerachText = SearchArear.Trim() });
            }
            if (action == "查看管线档案（案卷）")
            {
                return RedirectToAction("LookGuanXianFinshArchives", "ArchiveSearch2", new { SerachText = SearchArear.Trim() });
            }
            if (action == "查看管线档案（卷内）")
            {
                return RedirectToAction("LookGuanXianFinshFiles", "ArchiveSearch2", new { SerachText = SearchArear.Trim() });
            }
            if (action == "查看援川档案（工程）")
            {
                return RedirectToAction("LookYuanChuanProject", "ArchiveSearch3", new { SerachText = SearchArear.Trim() });
            }
            if (action == "查看援川档案（案卷）")
            {
                return RedirectToAction("LookYuanChuanArchives", "ArchiveSearch3", new { SerachText = SearchArear.Trim() });
            }
            if (action == "查看征地档案")
            {
                return RedirectToAction("LookZhengDiArchives", "ArchiveSearch3", new { SerachText = SearchArear.Trim() });
            }

            if (action == "查看业务档案")
            {
                return RedirectToAction("LookYeWuProject", "ArchiveSearch4", new { SerachText = SearchArear.Trim() });
            }

            return View();
        }
        public ActionResult LookQingArchives(string SerachText, string classNo)
        {
            string ss = getClassifyIDsByNames(classNo);
            string[] s = ss.Split(',');

            if (s[0] == "1")
            {
                return RedirectToAction("LookQingArchives_Zhizhao", "ArchiveSearch2", new { SerachTextZhiZhao = SerachText.Trim() });//查看执照档案
            }
            if (s[0] == "2")
            {
                return RedirectToAction("LookQingArchives_Road", "ArchiveSearch2", new { SerachTextRoad = SerachText.Trim() });//查看道路档案
            }
            if (s[0] == "3")
            {
                return RedirectToAction("LookQingArchives_Classtype", "ArchiveSearch2", new { SerachTextClass = SerachText.Trim() });//查看分类档案
            }
            if (s[0] == "4")
            {
                return RedirectToAction("LookQingArchives_Tuzhi", "ArchiveSearch2", new { SerachTextTuzhi = SerachText.Trim() });//查看图纸档案
            }



            return View();
        }
        public ActionResult LookFinshProject(string SerachText)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider=SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count FROM UrbanCon.dbo.vw_projectList where status='7' and ";
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
                if ((Require == "licenseNo" || Require == "archivesTitle" || Require == "archivesNo" || Require == "kaigongTime" || Require == "jungongTime" || Require == "shizhengNo" || Require == "isImageExist") && flag == 0)
                {

                    sql = "SELECT count(*) as count FROM UrbanCon.dbo.vw_projectList a where a.status='7' and a.paperProjectSeqNo in ( select paperProjectSeqNo from UrbanCon.dbo.ArchivesDetail c where c.";
                    flag = 1;
                }
                if ((Require == "partBLegalRepresent" || Require == "partBweituoAgent" || Require == "partBcontactTel") && flag == 0)
                {

                    sql = "SELECT count(*) as count FROM UrbanCon.dbo.vw_projectList a where a.status='7' and a.contractNo in ( select contractNo from UrbanCon.dbo.ContractInfo c where c.";
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

            string count1 = ds.Tables[0].Rows[0]["count"].ToString(); ;
            ViewData["count"] = count1;
            ViewData["SerachText"] = SerachText.ToString();
            int cnt =Int32.Parse(count1)/100 + 1;
            if (Int32.Parse(count1) % 100 == 0)
            {
                cnt = Int32.Parse(count1) / 100;
            }
            ViewData["totalpage"] = cnt;
            return View();

        }
        public string LookFinshProjectData(string SerachText, int? page)
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
            string sql = "SELECT paperProjectSeqNo,archivesCount,constructionOrganization,originalInchCount,projectName,location,licenseNo,startRegisNo,endRegisNo,developmentOrganization,jgDate,luruTime,textMaterial,drawing,photoCount,transferUnit,startArchiveNo,endArchiveNo,projectID FROM UrbanCon.dbo.vw_projectList where status='7' and ";
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
                if ((Require == "licenseNo" || Require == "archivesTitle" || Require == "archivesNo" || Require == "kaigongTime" || Require == "jungongTime" || Require == "shizhengNo" || Require == "isImageExist") && flag == 0)
                {

                    sql = "SELECT paperProjectSeqNo,constructionOrganization,archivesCount,originalInchCount,projectName,location,licenseNo,startRegisNo,endRegisNo,developmentOrganization,jgDate,luruTime,textMaterial,drawing,photoCount,transferUnit,startArchiveNo,endArchiveNo,projectID FROM UrbanCon.dbo.vw_projectList a where a.status='7' and a.paperProjectSeqNo in ( select paperProjectSeqNo from UrbanCon.dbo.ArchivesDetail c where c.";
                    flag = 1;
                }
                if ((Require == "partBLegalRepresent" || Require == "partBweituoAgent" || Require == "partBcontactTel") && flag == 0)
                {

                    sql = "SELECT paperProjectSeqNo,constructionOrganization,archivesCount,originalInchCount,projectName,location,licenseNo,startRegisNo,endRegisNo,developmentOrganization,jgDate,luruTime,textMaterial,drawing,photoCount,transferUnit,startArchiveNo,endArchiveNo,projectID FROM UrbanCon.dbo.vw_projectList a where a.status='7' and a.contractNo in ( select contractNo from UrbanCon.dbo.ContractInfo c where c.";
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
                if (vw_project.licenseNo.IndexOf("号") == -1)
                {
                    var archivesDetail = from a in db.ArchivesDetail
                                         where a.paperProjectSeqNo == vw_project.paperProjectSeqNo
                                         select a;
                    vw_project.licenseNo = archivesDetail.First().licenseNo;
                }
                vw_project.developmentOrganization = dr["developmentOrganization"].ToString();
                if (dr["jgDate"] == null)
                {
                    vw_project.jgDate = Convert.ToDateTime(dr["jgDate"]);
                }
                else {
                    vw_project.jgDate = null;
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
                vw_project.constructionOrganization = dr["constructionOrganization"].ToString();
                //vw_project.disignOrganization = dr["disignOrganization"].ToString();
                vw_project.startArchiveNo = dr["startArchiveNo"].ToString();
                vw_project.endArchiveNo = dr["endArchiveNo"].ToString();
                vw_project.startRegisNo = dr["startRegisNo"].ToString();
                vw_project.endRegisNo = dr["endRegisNo"].ToString();
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
                        new JProperty("startRegisNo", p.startRegisNo),
                        new JProperty("endRegisNo", p.endRegisNo),
                        new JProperty("developmentOrganization", p.developmentOrganization),
                        new JProperty("jgDate", p.jgDate),
                        new JProperty("luruTime", p.luruTime),
                        new JProperty("textMaterial", p.textMaterial),
                        new JProperty("drawing", p.drawing),
                        new JProperty("PhotoCount", p.PhotoCount),
                        new JProperty("transferUnit", p.transferUnit),
                        new JProperty("constructionOrganization", p.constructionOrganization),
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
        public ActionResult LookFinshArchives(string SerachText)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count FROM UrbanCon.dbo.vw_passList where status='7' and ";
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

                    sql = "SELECT count(*) as count FROM UrbanCon.dbo.vw_passList   a where a.status='7' and a.contractNo in ( select contractNo from UrbanCon.dbo.ContractInfo c where c.";
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
        public string LookFinshArchivesData(string SerachText, int? page)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT paperProjectSeqNo,volNo,isImageExist,archiveThickness,archivesTitle,licenseNo,archivesNo,registrationNo,typerDate,developmentUnit,constructionUnit,designUnit,transferUnit,textMaterial,drawing,photoCount FROM UrbanCon.dbo.vw_passList where status='7' and ";
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

                    sql = "SELECT paperProjectSeqNo,volNo,isImageExist,archiveThickness,archivesTitle,licenseNo,archivesNo,registrationNo,typerDate,developmentUnit,constructionUnit,designUnit,transferUnit,textMaterial,drawing,photoCount FROM UrbanCon.dbo.vw_passList  a where a.status='7' and a.contractNo in ( select contractNo from UrbanCon.dbo.ContractInfo c where c.";
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
                //vw_project.jgDate = Convert.ToDateTime(dr["jgDate"]);
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

        //LookFinshArchives
        public ActionResult LookFinshFile(string SerachText)
        {

            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT count(*) as count FROM UrbanCon.dbo.FileInfo where ";
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
        public string LookFinshFileData(string SerachText, int? page)
        {
            db.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider = SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT seqNo,fileNo,fileName,responsible,type,startPageNo ,dengjihao FROM UrbanCon.dbo.FileInfo where ";
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
                else {
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


        public ActionResult Allprojectanjuan(string registrationNo)
        {

            var arc = from b in db.vw_passList
                          where b.registrationNo == registrationNo
                          where b.status == "7"
                          select b;
            if (arc.Count() == 0)
            {
                return Content("<script >alert('请在卷内全部记录中选择一条记录！');window.history.back();</script >");

            }
            if (arc == null)
            {
                return HttpNotFound();
            }
            //ViewData["url"] = id2;
            //ViewData["url1"] = Request.Url.ToString().Trim();
            ViewData["volCount"] = arc.Count();
            ViewBag.result = JsonConvert.SerializeObject(arc);
            return View();

        }


        public ActionResult Allprojectjilu(string registrationNo)
        {

            var paperno = from b in db.ArchivesDetail
                          where b.registrationNo == registrationNo
                          select b.paperProjectSeqNo;
            if (paperno.Count() == 0)
            {
                return Content("<script >alert('请在案卷全部记录中选择一卷案卷！');window.history.back();</script >");

            }
            long PaperSeqNo = paperno.First();

            var project = from a in db.vw_projectList
                          where a.paperProjectSeqNo == PaperSeqNo
                          select a;

            if (project.Count() == 0)
            {
                return Content("<script >alert('该案卷无工程信息！');window.history.back();</script >");
            }
            ViewData["volCount"] = project.Count();
            return View(project);

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
            var projectinfo = from b in db.vw_passList
                              where b.registrationNo == id3
                              select b;
            long paperseq = projectinfo.First().paperProjectSeqNo;
            if (projectinfo.Count() == 0)
            {
                return Content("<script >alert('请在案卷全部记录中选择一项案卷');window.history.back();</script >");
                //return Content("<script >alert('请在案卷全部记录中选择一项案卷');</script >");
            }
            string seq = paperseq.ToString().Trim();
            var project = from c in db.vw_passList
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
            else
            {
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

            //结构类型
            string structureid = projectinfo.First().structureTypeID;
            if (structureid == "") {
                structureid = "5";
            }
            var structure = from a in db.StructureType
                            where a.structureTypeID == structureid
                            select a.structureTypeName;
            ViewData["structureType"] = structure.First();


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
            if (projectinfo.First().isImageExist == "无")
            {
                ViewData["isImageExist"] = true;
                ViewData["Message"] = "该案卷无扫描件";
            }
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
            var firstRegis = from a in db.ArchivesDetail
                             where a.paperProjectSeqNo == paper
                             orderby a.volNo
                             select a.registrationNo;
            if (action == "上一卷")
            {
                string regis = registrationNo;
                long re = Int32.Parse(regis);
                int index = regis.IndexOf('0');
                re = re - 1;
                regisNo = "";
                if (index == -1)
                {
                    regisNo = re.ToString();
                }
                else
                {
                    regisNo = "0" + re.ToString();
                }

                //if (regisNo.ToString() == firstRegis.First())
                //{
                //    flag1 = "1";//该工程的第一卷
                //    return RedirectToAction("anjuanzhuludan",new { id3= firstRegis.First(),flag=flag1});



                //}
                //else
                //{
                //flag1 = "2";//正常
                //return RedirectToAction("anjuanzhuludan", new { id3 = firstRegis.First(), flag = flag1 });
                //}
            }
            //var lastRegis = from a in db.ArchivesDetail
            //                 where a.paperProjectSeqNo == paper
            //                 orderby a.volNo descending
            //                 select a.registrationNo;
            if (action == "下一卷")
            {
                string regis = registrationNo;
                long re = Int32.Parse(regis);
                int index = regis.IndexOf('0');
                re = re + 1;
                regisNo = "";
                if (index == -1)
                {
                    regisNo = re.ToString();
                }
                else
                {
                    regisNo = "0" + re.ToString();
                }
                //if (regisNo.ToString() == lastRegis.First())
                //{
                //    flag1 = "3";//该工程的最后一卷
                //    return RedirectToAction("anjuanzhuludan", new { id3 = lastRegis.First(), flag = flag1 });

                //}
                //else
                //{
                //    flag1 = "2";//正常
                //return RedirectToAction("anjuanzhuludan", new { id3 = lastRegis.First(), flag = flag1 });

                //}

            }

            return RedirectToAction("anjuanzhuludan", new { id3 = regisNo });

        }
        public ActionResult Juanneimulu(string id1, string url1)
        {
            if (id1 == "-1")
            {
                return Content("<script >alert('不存在该卷内目录');window.history.back();</script >");
            }
            if (id1 == "" || id1 == null)
            {
                return Content("<script >alert('请在案卷全部记录中选择一项案卷');window.history.back();</script >");
            }
            var archNo = from b in db.ArchivesDetail
                         where b.registrationNo == id1
                         select b.archivesNo;
            string archiveN = archNo.First().Trim();
            var file = from a in db.FileInfo
                       where a.archivesNo == archiveN
                       orderby a.seqNo
                       select a;
            ViewData["url1"] = url1;
            if (file.Count() == 0)
            {
                return Content("<script >alert('该案卷无卷内目录信息！');window.history.back();</script >");

            }
            ViewData["volCount"] = file.Count();
            ViewData["paperProjectSeqNo"] = db.ArchivesDetail.Where(ad => ad.registrationNo == id1).First().paperProjectSeqNo;
            ViewData["volNo"] = db.ArchivesDetail.Where(ad => ad.registrationNo == id1).First().volNo;
            ViewData["archiveNo"] = db.ArchivesDetail.Where(ad => ad.registrationNo == id1).First().archivesNo;
            return View(file.ToList());

        }
        public ActionResult LooksaomiaoJN(string paperProjectSeqNo, string volNo, string seqNo, string archiveNo)
        {
            long paper = long.Parse(paperProjectSeqNo.Trim());
            var paperM = from b in db.PaperArchives
                         where b.paperProjectSeqNo == paper
                         select b;
            while (paperProjectSeqNo.Length < 5)
            {
                paperProjectSeqNo = "0" + paperProjectSeqNo;
            }
            while (volNo.Length < 3)
            {
                volNo = "0" + volNo;
            }
            if (paperM.First().archivesCount == "" || paperM.First().archivesCount == null)
            {
                ViewData["VolCnt"] = "0";
            }
            else
            {
                ViewData["VolCnt"] = paperM.First().archivesCount.ToString();
            }
            ViewData["proNo"] = paperProjectSeqNo;
            ViewData["VolNo"] = volNo;
            ViewData["seqNo"] = seqNo.Trim();
            ViewData["archiveNo"] = archiveNo;
            return View();
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
            var file = from ad in db.FileInfo
                       where ad.archivesNo == ArchiveNo && ad.seqNo == ID
                       select ad;

            if (file.Count() == 0)
            {
                return Content("<script >alert('该案卷文件没有卷内目录信息！');window.history.back();</script >");
            }
            Models.FileInfo fileinfo = file.First();
            return View(fileinfo);
        }
        [HttpPost]
        public ActionResult Juanneimulu(string url1)
        {
            return Redirect(url1);
        }
        public ActionResult ProjectInfoes(long? id, string id2)
        {
            if (id == null || id == 0)
            {
                return Content("<script >alert('请在工程全部记录中选择一项工程');window.history.back();</script >");
            }

            var projectinfo = from b in db.vw_passList
                              where b.projectID == id
                              orderby b.paperProjectSeqNo, b.paijiaNo
                              select b;
            if (projectinfo.Count() == 0)
            {
                return Content("<script >alert('请在工程全部记录中选择一项工程');window.history.back();</script >");
            }
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

            //结构类型
            string structureid = projectinfo.First().structureTypeID;
            if (structureid == "")
            {
                structureid = "5";
            }
            var structure = from a in db.StructureType
                            where a.structureTypeID == structureid
                            select a.structureTypeName;
            ViewData["structureType"] = structure.First();

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
        [HttpPost]
        public ActionResult ProjectInfoes(string url)
        {
            return Redirect(url);
        }

        public ActionResult Allprojectjilu1(string dengjihao)
        {
            string registrationNo = dengjihao.Trim();
            var pap = from a in db.vw_passList
                      where a.registrationNo == registrationNo
                      select a;
            if (pap.Count() == 0)
            {
                return Content("<script >alert('请在案卷全部记录中选择一项案卷');window.history.back();</script >");
            }

            if (pap == null)
            {
                return HttpNotFound();
            }
            //ViewData["url"] = id2;
            //ViewData["url1"] = Request.Url.ToString().Trim();
            ViewData["volCount"] = pap.Count();
            ViewBag.result = JsonConvert.SerializeObject(pap);
            return View();
        }

        public ActionResult Projectanjuan(long? id, string id2)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var paperno = from b in db.PaperArchives
                          where b.projectID == id
                          select b.paperProjectSeqNo;
            if (paperno.Count() == 0)
            {
                return Content("<script >alert('请在工程全部记录中选择一项工程');window.history.back();</script >");
            }
            long PaperSeqNo = paperno.First();

            var archive = from a in db.ArchivesDetail
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
        [HttpPost]
        public ActionResult Projectanjuan(string action, string url, string checkname1)
        {
            if (action == "关闭此页面")
            {
                return Redirect(url);
            }
            if (action == "案卷著录单")
            {
                return RedirectToAction("anjuanzhuludan", new { id3 = checkname1.Trim(), id4 = Request.Url.ToString().Trim() });
            }
            if (action == "卷内全部目录")
            {
                return RedirectToAction("Juanneimulu", new { id1 = checkname1.Trim(), url1 = Request.Url.ToString().Trim() });
            }
            if (action == "卷内目录信息")
            {
                return RedirectToAction("anjuanzhuludan", new { id3 = checkname1.Trim(), id4 = Request.Url.ToString().Trim() });
            }
            if (action == "当前用户添加")
            {
                return RedirectToAction("CurUserAdd", new { id3 = checkname1.Trim(), id4 = Request.Url.ToString().Trim() });
            }
            if (action == "非当前用户添加")
            {
                return RedirectToAction("anjuanzhuludan", new { id3 = checkname1.Trim(), id4 = Request.Url.ToString().Trim() });
            }
            if (action == "查看扫描件")
            {
                return RedirectToAction("Looksaomiao", new { id3 = checkname1.Trim(), id4 = Request.Url.ToString().Trim() });
            }
            return View();
        }

        public string Looksaomiaopost(string paperProjectSeqNo, string ArchiveNo, string volNo, string[] arr1, string info, string name1, string VolCnt1, string userid, string regNo, string Cntyeci)
        {
            string flag = "0";

            if (name1 == "Add")
            {
                if (arr1.Length != 0)
                {

                    if (arr1.Length == 1)
                    {

                        flag = saveImageAndArchives(arr1[0].ToString(), userid, paperProjectSeqNo, ArchiveNo);
                    }
                    else
                    {

                        foreach (string s in arr1)
                        {

                            flag = saveImageAndArchives(s, userid, paperProjectSeqNo, ArchiveNo);
                        }
                    }
                }
                else
                {

                    flag = "5";
                }
            }
            if (name1 == "Adddown")
            {
                if (arr1.Length != 0)
                {
                    if (arr1.Length == 1)
                    {
                        flag = saveImageAndArchives1(arr1[0].ToString(), userid, paperProjectSeqNo, ArchiveNo, Cntyeci);
                    }
                    else
                    {
                        foreach (string s in arr1)
                        {
                            flag = saveImageAndArchives1(s, userid, paperProjectSeqNo, ArchiveNo, Cntyeci);
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
                string src = info.Trim();
                if (src.IndexOf(',') == -1)
                {
                    path = bll.GetVirtualDirectory("80", src);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);


                    }
                }
                else
                {
                    string[] paths = src.Split(',');
                    foreach (string p in paths)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);

                        }
                    }
                }
                //"扫描件删除成功";
                flag = "6";
            }

            //string str1 = paperProjectSeqNo, str2 = ArchiveNo, str3 = regNo, str4 = volNo;

            //ViewData["proNo"] = paperProjectSeqNo;
            //ViewData["VolNo"] = volNo;
            //ViewData["VolCnt"] = VolCnt1;

            //return RedirectToAction("Looksaomiao",new { paperProjectSeqNo= str1, ArchiveNo= str2, regNo= str3, volNo= str4 });
            return JsonConvert.SerializeObject(flag);

        }
        public ActionResult Looksaomiao(string paperProjectSeqNo, string ArchiveNo, string regNo, string volNo)
        {
            if (regNo != "")
            {
                var archiveM = from a in db.ArchivesDetail
                               where a.registrationNo == regNo
                               select a;
                if (archiveM.Count() == 0)
                {
                    return View();
                }
                else
                {
                    long paper = long.Parse(paperProjectSeqNo.Trim());
                    var paperM = from b in db.PaperArchives
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
                        if (paperM.First().archivesCount == "" || paperM.First().archivesCount == null)
                        {
                            ViewData["VolCnt"] = "0";
                        }
                        else {
                            ViewData["VolCnt"] = paperM.First().archivesCount.ToString();
                        }
                    }
                }
            }

            return View();
        }
        public ActionResult SaomiaoPrint(object obj/*,  string [] Name, string [] Path, int count*/)
        {
            //  Name = Name[0].ToString().Split(',');
            //Path = Path[0].ToString().Split(',');

            //string a = obj.name;
            //string b = obj.path;
            // DataTable myDataTable = new DataTable();
            //myDataTable.Columns.Add("Name", typeof(string));
            //myDataTable.Columns.Add("WebPath", typeof(string));

            //if(Name.Length==1)
            //{
            //    DataRow row = myDataTable.NewRow();
            //    row["Name"] = Name[0].ToString().Trim();
            //    row["WebPath"] =Path[0].ToString().Trim();
            //    myDataTable.Rows.Add(row);
            //}
            //else
            //{
            //    for (int i = 0; i < Name.Length; i++)
            //    {
            //        DataRow row = myDataTable.NewRow();
            //        row["Name"] = Name[i].ToString().Trim();
            //        row["WebPath"] = Path[i].ToString().Trim();
            //        myDataTable.Rows.Add(row);

            //    }
            //}

            //ViewBag.result1 = JsonConvert.SerializeObject(myDataTable);
            return View();
        }


        public ActionResult CurUserAdd(string id3, string id4, string type)
        {
            var borrow = from f in db.BorrowRegistration
                         orderby f.ID descending
                         select f;
            if (id4 != null && id4 != "")
            {
                long nocurID = long.Parse(id4.Trim());
                borrow = from f in db.BorrowRegistration
                         where f.ID == nocurID
                         orderby f.ID descending
                         select f;
            }
            long ID = borrow.First().ID;

            var user = from c in ab.AspNetUsers
                       select c;
            StringBuilder strBuider = new StringBuilder();
            string info = id3;
            ViewData["userID"] = ID;
            string ty = type;
            string[] AllInfo = getSearchArear(ty, strBuider, info);
            var a = AllInfo.ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < a.Count() - 1; i++)
            {

                items.Add(new SelectListItem { Text = a[i].ToString().Trim(), Value = i.ToString() });
            }
            ViewBag.items = new MultiSelectList(items, "value", "text");
            ViewData["BorrowName"] = borrow.First().borrower;
            ViewData["bindDate"] = DateTime.Now.Date;
            ViewBag.Binder = new SelectList(user, "UserName", "UserName");
            ViewData["Archive"] = string.Join(",", AllInfo).ToString();
            ViewData["type"] = type.Trim();
            return View();
        }
        [HttpPost]
        public ActionResult CurUserAdd(string action, string userID, string Binder, string bindDate, string Archive, string url, string type1)
        {
            //if(action=="关闭")
            //{

            //    return JavaScriptResult("<script language=\'javascript\'>window.close();</script>"); 
            //}

            if (action == "添加到用户")
            {
                string borrowseq = "";//20171204byzl为了给结算页面传递borrowSeqNo
                int type = 1;
                if (type1 != null && type1 != "")
                {
                    type = Int32.Parse(type1.Trim());
                }

                int ID = Int32.Parse(userID.Trim());
                string binder = Binder;

                string dateTime = bindDate.Trim();
                if (dateTime == "")
                {
                    dateTime = DateTime.Now.Date.ToString("yyyy-MM-dd");
                }


                string[] str1 = Archive.Trim().Split(',');
                string[] str2 = null;
                string str3 = "";
                foreach (string s1 in str1)
                {
                    if (s1 == "" || s1 == null)
                    {
                        break;
                    }
                    str2 = s1.Split(new string[] { "——" }, StringSplitOptions.None);
                    str3 += str2[1].ToString().Split('：')[1].ToString() + ",";



                }
                string[] str4 = str3.Trim().Split(',');
                foreach (string s in str4)
                {
                    if (s == "" || s == null)
                    {
                        break;
                    }

                    DateTime time = DateTime.Parse(dateTime);
                    var mol = from a in db.BindUserAndArchives
                              where a.userID == ID && a.type ==type && a.bindDate == time && a.archiveNo == s
                              orderby a.userID
                              select a;

                    if (mol.Count() == 0)
                    {

                        BindUserAndArchives model = new BindUserAndArchives();


                        model.userID = ID;
                        model.type = type;
                        model.bindDate = time;
                        model.binder = binder;
                        model.archiveNo = s;
                        db.Entry(model).State = EntityState.Added;
                        //db.BindUserAndArchives.Add();

                        //catch (Exception ex)
                        //{
                        //    
                        //}
                        var brm = from b in db.BorrowRegistration
                                  where b.ID == ID
                                  select b;
                        borrowseq = brm.First().borrowSeqNo.Trim();
                        if (brm.Count() != 0)
                        {
                            brm.First().isJiesuanFee = 2;
                            db.Entry(brm.First()).State = EntityState.Modified;


                        }

                    }


                    db.SaveChanges();



                }
                return Content("<script >alert('添加成功！');window.location.href='/UrbanBorrow/FeeJieSuan/?id=" + borrowseq + "';</script >");
                //return RedirectToAction("FeeJieSuan","UrbanBorrow",new {id=userID });
            }
            return View();
        }
        public ActionResult NoCurUser(string id3, string type)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "借阅人姓名", Value = "0"},
                new SelectListItem { Text = "借阅单位", Value = "1"},

            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text", "");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            DateTime nowTime = DateTime.Now.Date;
            var borrow = (from c in db.BorrowRegistration
                          where c.isJiesuanFee == 1 && c.borrowDate == nowTime
                          select c).Union(from c in db.BorrowRegistration
                                          where c.isJiesuanFee == 2 && c.borrowDate == nowTime
                                          select c).Union(from c in db.BorrowRegistration
                                                          where c.isJiesuanFee == 3 && c.borrowDate == nowTime
                                                          select c).OrderBy(s => s.isJiesuanFee).ThenBy(s => s.ID);


            ViewBag.result1 = JsonConvert.SerializeObject(borrow);
            ViewData["name"] = id3;
            ViewData["ty"] = type;


            return View();
        }
        [HttpPost]
        public ActionResult NoCurUser()
        {
            return RedirectToAction("RegisUser", "UrbanBorrow");
        }
        public ActionResult NoCurUserDetail(long? id, string url2)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var borrow = from a in db.BorrowRegistration
                         where a.ID == id
                         select a;
            var borrow1 = from b in db.BorrowRegistration
                          orderby b.ID descending
                          select b;
            BorrowRegistration BorrowRegis = borrow.First();
            var BindReceive = from b in db.BindUserAndArchives
                              where b.userID == id
                              select b;
            //判断是否是第一条记录或是最后一条记录
            long MaxID = borrow1.First().ID;
            long MinID = 1;
            if (id == MinID)
            {
                ViewData["Next"] = true;
            }
            if (id == MaxID)
            {
                ViewData["Pre"] = true;
            }
            if (BindReceive.Count() != 0)//获取调卷数
            {
                BorrowRegis.consultVolumeCount = BindReceive.Count().ToString();
                BorrowRegis.precisionVolumeCount = BindReceive.Count().ToString();
                BorrowRegis.archiveSerialNo = getSeriNo(id);
            }
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
            //用户绑定查阅案卷
            ViewData["checkname"] = 0;

            //应交费用计算

            decimal str1 = 0, str2 = 0, str3 = 0;
            if (BorrowRegis.paoKufangRen.Trim() != "" && BorrowRegis.paoKufangRen.Trim() != null)
            {


                str3 = Convert.ToDecimal(BorrowRegis.paoKufangRen.Trim());
            }
            str1 = Convert.ToDecimal(BorrowRegis.certificationFee);
            str2 = Convert.ToDecimal(BorrowRegis.consultFee);
            decimal YjFee = str1 + str2 + str3;
            ViewData["YJfee"] = YjFee.ToString("0.00");

            return View(BorrowRegis);
        }
        [HttpPost]
        public ActionResult NoCurUserDetail(long? id, string action, string url)
        {
            var borrow1 = from b in db.BorrowRegistration
                          orderby b.ID descending
                          select b;
            //判断是否是第一条记录或是最后一条记录

            long MaxID = borrow1.First().ID;
            long MinID = 1;
            if (action == "前一个")
            {
                if (id == 13884)//因数据库主键不断更新的缘故，中间有缺失的ID，因此采用此方法
                {
                    id = 13901;
                }
                else
                {
                    id = id + 1;
                }

            }
            if (action == "后一个")
            {
                if (id == 13901)
                {
                    id = 13884;
                }
                else
                {
                    id = id - 1;
                }
            }
            if (action == "返回")
            {

                return Redirect(url);


            }
            if (id == MinID)
            {
                ViewData["Next"] = true;
            }
            if (id == MaxID)
            {
                ViewData["Pre"] = true;
            }


            return RedirectToAction("NoCurUserDetail", new { id = id });
        }
        private string[] getSearchArear(string typeNo, StringBuilder strBuider, string info)
        {

            string strType = "";
            int type = Int32.Parse(typeNo.Trim());
            if (type == 1)//竣工档案
            {
                strType = "竣工档案";
                strBuider = bindFinishArchive(strType, strBuider, info);
            }
            else if (type == 2)//声像视频档案
            {
                strType = "声像视频档案";
                strBuider = bindVideoArchive(strType, strBuider, info);
            }
            else if (type == 3)//照片档案
            {
                strType = "声像照片档案";
                strBuider = bindPhotoArchive(strType, strBuider, info);
            }
            else if (type == 4)//规划档案
            {
                strType = "规划档案";
                strBuider = bindPlanArchive(strType, strBuider, info);
            }
            else if (type == 5)//其他档案
            {
                strBuider = bindOtherArchive(strBuider, info);
            }
            else if (type == 6)//征地档案
            {
                strBuider = bindZhengDiArchive(strBuider, info);
            }
            else if (type == 7)//图纸档案
            {
                strBuider = bindTuzhiArchive(strBuider, info);
            }
            else if (type == 8)
            {
                strType = "管线档案";
                strBuider = bindgxFinishArchive(strType, strBuider, info);
            }
            else if (type == 9)
            {
                strType = "援川档案";
                strBuider = bindYuanchuanArchive(strType, strBuider, info);
            }
            string[] AllInfo = strBuider.ToString().Split(',');
            return AllInfo;
        }
        private StringBuilder bindgxFinishArchive(string strType, StringBuilder strBuider, string info)
        {
            string[] info1;

            info1 = info.Split(',');
            for (int i = 0; i < info1.Length - 1; i++)
            {
                if (info1[i] != null || info1[i] != "")
                {
                    string S = info1[i].ToString().Trim();
                    var model = from a in ag.gxArchivesDetail
                                where a.archivesNo == S
                                select a;
                    if (model.Count() != 0)
                    {
                        string archiveTitle = model.First().archivesTitle;
                        if (archiveTitle == "" || archiveTitle == string.Empty)
                        {
                            archiveTitle = "NULL";
                        }
                        strBuider.Append("类型 " + "：" + strType + "——");
                        strBuider.Append("档号 " + "：" + S + "——");
                        strBuider.Append("案卷题名" + "：" + archiveTitle + ",");
                    }
                }
                else
                {
                    break;
                }
            }


            return strBuider;
        }
        private StringBuilder bindFinishArchive(string strType, StringBuilder strBuider, string info)
        {
            string[] info1;

            info1 = info.Split(',');
            for (int i = 0; i < info1.Length - 1; i++)
            {
                if (info1[i] != null || info1[i] != "")
                {
                    string S = info1[i].ToString().Trim();
                    var model = from a in db.ArchivesDetail
                                where a.archivesNo == S
                                select a;
                    if (model.Count() != 0)
                    {
                        string archiveTitle = model.First().archivesTitle;
                        if (archiveTitle == "" || archiveTitle == string.Empty)
                        {
                            archiveTitle = "NULL";
                        }
                        strBuider.Append("类型 " + "：" + strType + "——");
                        strBuider.Append("档号 " + "：" + S + "——");
                        strBuider.Append("案卷题名" + "：" + archiveTitle + ",");
                    }
                }
                else
                {
                    break;
                }
            }


            return strBuider;
        }
        private StringBuilder bindVideoArchive(string strType, StringBuilder strBuider, string archiveNo)
        {
            string[] info = null;


            info = archiveNo.Split(',');
            foreach (string s in info)
            {
                if (s == "" || s == null)
                {
                    break;
                }
                int ID = int.Parse(s.Trim());
                var model = from a in db.vw_VideoProjectList
                            where a.ID == ID
                            select a;


                if (model.Count() != 0)
                {
                    string archiveTitle = model.First().projectName;
                    if (archiveTitle == "" || archiveTitle == string.Empty)
                    {
                        archiveTitle = "NULL";
                    }
                    strBuider.Append("类型 " + "：" + strType + "——");
                    strBuider.Append("工程ID " + "：" + s + "——");
                    strBuider.Append("工程名称" + "：" + archiveTitle + ",");
                }
            }
            return strBuider;
        }
        private StringBuilder bindPhotoArchive(string strType, StringBuilder strBuider, string archiveNo)
        {
            string[] info = null;


            info = archiveNo.Split(',');
            foreach (string s in info)
            {

                var model = from a in db.PhotoCassette
                            where a.photoArchiveNo == s
                            select a;
                if (model != null)
                {
                    string archiveTitle = model.First().archiveTitle;
                    if (archiveTitle == "" || archiveTitle == string.Empty)
                    {
                        archiveTitle = "NULL";
                    }
                    strBuider.Append("类型 " + "：" + strType + "——");
                    strBuider.Append("档号 " + "：" + s + "——");
                    strBuider.Append("案卷题名" + "：" + archiveTitle + ",");
                }
            }
            return strBuider;
        }
        private StringBuilder bindPlanArchive(string strType, StringBuilder strBuider, string archiveNo)
        {
            string[] info = null;


            info = archiveNo.Split(',');
            foreach (string s in info)
            {
                if (s == "")
                {
                    break;
                }
                string[] id = s.Split(',');
                string ID = id[0].ToString().Trim();
                var model = from a in ae.PlanProject
                            where a.totalSeqNo == ID
                            select a;
                if (model != null)
                {
                    string projContent = model.First().projectContent.Trim();
                    if (projContent == "" || projContent == string.Empty)
                    {
                        projContent = "NULL";
                    }
                    strBuider.Append("类型 " + "：" + strType + "——");
                    strBuider.Append("工程总顺序号 " + "：" + s + "——");
                    strBuider.Append("工程名称" + "：" + projContent + ",");
                }
            }
            return strBuider;
        }
        private StringBuilder bindOtherArchive(StringBuilder strBuider, string archiveNo)
        {
            string strType = "其他档案";
            string[] info = null;
            info = archiveNo.Split(',');

            foreach (string s in info)
            {
                if (s == "" || s == null)
                {
                    break;
                }
                string[] id = s.Split('/');
                int ID = Int32.Parse(id[0]);
                var model = from a in db.OtherArchives
                            where a.ID == ID
                            select a;
                if (model.Count() != 0)
                {
                    string regisNo = model.First().registrationNo;
                    if (regisNo == "" || regisNo == string.Empty || regisNo == null)
                    {
                        regisNo = "NULL";
                    }
                    int classTypeID = model.First().classTypeID;
                    var model1 = from b in db.otherArchiveType
                                 where b.classTypeID == classTypeID
                                 select b;
                    if (model1.Count() != 0)
                    {
                        strType = model1.First().classTypeName.Trim();
                    }
                    strBuider.Append("类型 " + "：" + strType + "——");
                    strBuider.Append("工程ID " + "：" + s + "——");
                    strBuider.Append("总登记号" + "：" + regisNo + ",");
                }
            }
            return strBuider;
        }
        private StringBuilder bindZhengDiArchive(StringBuilder strBuider, string archiveNo)
        {
            string strType = "征地档案";
            string[] info = null;

            info = archiveNo.Split(',');
            foreach (string s in info)
            {

                var model = from a in db.zdArchive
                            where a.regisNo == s
                            select a;

                if (model.Count() != 0)
                {
                    string regisNo = model.First().regisNo;
                    if (regisNo == "" || regisNo == string.Empty)
                    {
                        regisNo = "NULL";
                    }
                    string archiveTitle = model.First().archiveTitle.Trim();
                    strBuider.Append("类型 ：征地档案——");
                    strBuider.Append("总登记号：" + regisNo + "——");
                    strBuider.Append("案卷题名：" + "" + archiveTitle + "" + ",");
                }
            }
            return strBuider;
        }
        private StringBuilder bindTuzhiArchive(StringBuilder strBuider, string archiveNo)
        {
            string strType = "图纸档案";
            string[] info = null;
            info = archiveNo.Split(',');
            foreach (string s in info)
            {
                if (s == "" || s == null)
                {
                    break;
                }
                long ID = Int32.Parse(s);
                var model = from a in db.TuzhiArchives
                            where a.ID == ID
                            select a;
                if (model != null)
                {
                    string regisNo = model.First().registrationNo.Trim();
                    if (regisNo == "" || regisNo == string.Empty)
                    {
                        regisNo = ID.ToString();
                    }
                    string archiveTitle = model.First().archiveTitle.Trim();
                    strBuider.Append("类型 ：图纸档案——");
                    strBuider.Append("序号：" + regisNo + "——");
                    strBuider.Append("案卷题名：" + "" + archiveTitle + "" + ",");
                }
            }
            return strBuider;
        }
        private StringBuilder bindYuanchuanArchive(string strType, StringBuilder strBuider, string info)
        {
            string[] info1;

            info1 = info.Split(',');
            for (int i = 0; i < info1.Length - 1; i++)
            {
                if (info1[i] != null || info1[i] != "")
                {
                    string S = info1[i].ToString().Trim();
                    var model = from a in db.YuanChuanArchivesDetail
                                where a.archivesNo == S
                                select a;
                    if (model.Count() != 0)
                    {
                        string archiveTitle = model.First().archivesTitle;
                        if (archiveTitle == "" || archiveTitle == string.Empty)
                        {
                            archiveTitle = "NULL";
                        }
                        strBuider.Append("类型 " + "：" + strType + "——");
                        strBuider.Append("档号 " + "：" + S + "——");
                        strBuider.Append("案卷题名" + "：" + archiveTitle + ",");
                    }
                }
                else
                {
                    break;
                }
            }


            return strBuider;
        }
        private string dsToJson(DataTable dt)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
            System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
            foreach (DataRow dataRow in dt.Rows)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();  //实例化一个参数集合
                foreach (DataColumn dataColumn in dt.Columns)
                {
                    dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName].ToString());
                }
                arrayList.Add(dictionary); //ArrayList集合中添加键值
            }
            return javaScriptSerializer.Serialize(arrayList);  //返回一个json字符串
        }

        public ActionResult QingZhaoDia()
        {
            return View();
        }
        public ActionResult KindGuihuaDia()
        {
            return View();
        }


        public ActionResult ShowCurUserBangDing(long? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int userID = Convert.ToInt32(id);

            var model = from a in db.BorrowRegistration
                        where a.ID == userID
                        select a;
            //当天借阅用户的下拉框人员选择
            int status1 = 1;
            string status = status1.ToString();
            status1 = 2;
            status += status1.ToString();
            DateTime nowTime = DateTime.Now.Date;
            List<SelectListItem> items = new List<SelectListItem>();
            int flag0 = Int32.Parse(status[0].ToString().Trim()), flag1 = Int32.Parse(status[1].ToString().Trim());
            int fl = Int32.Parse(status.Trim());
            var model1 = (from a in db.BorrowRegistration
                          where a.isJiesuanFee == flag0 && a.borrowDate == nowTime
                          orderby a.isJiesuanFee, a.ID descending
                          select a).Union(from b in db.BorrowRegistration
                                          where b.isJiesuanFee == flag1 && b.borrowDate == nowTime
                                          orderby b.isJiesuanFee, b.ID descending
                                          select b);



            if (model1.Count() == 0)
            {
                ViewData["checkname"] = 1;//向前台传值，提示错误信息
                ViewBag.SelectedID = new SelectList(items, "value", "text");
            }
            else
            {
                foreach (var item in model1)
                {
                    items.Add(new SelectListItem { Text = item.ID + "-" + item.borrower, Value = item.ID.ToString() });
                }

                for (int i = 0; i < model1.Count(); i++)
                {
                    if (items.ElementAt(i).Value == userID.ToString())
                    {
                        ViewBag.SelectedID = new SelectList(items, "value", "text", items.ElementAt(i).Value);
                        break;
                    }
                }
            }
            //按照一定条件查询数据级，向前台传递
            DateTime endTime = DateTime.Now.Date;
            cacuCurDateEarlier ccde = new cacuCurDateEarlier(endTime);
            string startTime = ccde.cacu15DaysEarlier();
            DateTime start = DateTime.Parse(startTime.Trim());
            var med = from b in db.BindUserAndArchives
                      where b.userID == userID && b.bindDate >= start && b.bindDate <= endTime
                      orderby b.archiveNo
                      select b;
            if (med.Count() == 0)
            {
                ViewData["checkname"] = 1;//向前台传值，提示错误信息

            }
            if (model.Count() != 0)
            {
                ViewData["name"] = model.First().borrower;
                ViewData["fenID"] = userID.ToString();
            }

            ViewBag.result = JsonConvert.SerializeObject(med);
            return View();

        }
        [HttpPost]
        public ActionResult ShowCurUserBangDing(string action, string SelectedID)
        {

            if (action == "关闭")
            {
                return Redirect("/ArchiveSearch/ArchiveSearchMainWindow");
            }
            if (action == "选择")
            {
                if (SelectedID == "")
                {
                    return Content("<script >alert('请选择借阅人');window.history.back();</script >");
                }

                return RedirectToAction("ShowCurUserBangDing", new { id = SelectedID.Trim() });

            }
            return View();

        }
        // GET: ArchiveSearch/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ArchiveSearch/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ArchiveSearch/Create
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

        // GET: ArchiveSearch/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ArchiveSearch/Edit/5
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

        // GET: ArchiveSearch/Delete/5
        public ActionResult Delete(string id3, string id4, string id5, string id6)
        {
            DateTime time = DateTime.Parse(id5);
            long id = long.Parse(id3.Trim());
            var model = from a in db.BindUserAndArchives
                        where a.userID == id && a.archiveNo == id4 && a.bindDate == time
                        select a;
            db.BindUserAndArchives.Remove(model.First());
            db.SaveChanges();

            return Content("<script >alert('删除成功！');location.href='/ArchiveSearch/ShowCurUserBangDing/" + id3 + "';</script >");


        }

        // POST: ArchiveSearch/Delete/5
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
        public IQueryable GetAllBorrowerInfo(string flag, int index, string queryItem, string queryText)
        {

            var model1 = from j in db.BorrowRegistration
                         where j.ID == 0
                         select j;


            DateTime nowTime = DateTime.Now.Date;
            int flag0 = Int32.Parse(flag[0].ToString().Trim()), flag1 = Int32.Parse(flag[1].ToString().Trim());
            int fl = Int32.Parse(flag.Trim());


            if (flag == "12")
            {
                model1 = (from a in db.BorrowRegistration
                          where a.isJiesuanFee == flag0 && a.borrowDate == nowTime && a.borrowUnit.Contains("")
                          orderby a.isJiesuanFee, a.ID descending
                          select a).Union(from b in db.BorrowRegistration
                                          where b.isJiesuanFee == flag1 && b.borrowDate == nowTime && b.borrowUnit.Contains("")
                                          orderby b.isJiesuanFee, b.ID descending
                                          select b);



            }
            //else if (flag == "1" || flag == "2")
            //{
            //     model1 = from c in db.BorrowRegistration
            //                 where c.isJiesuanFee== fl && c.borrowDate == nowTime && c.borrowUnit.Contains("")
            //                 orderby c.ID descending
            //                 select c;

            //}
            //else if (flag == "3")
            //{

            //    if (index == 0)//只显示当天的
            //    {
            //         model1 = from d in db.BorrowRegistration
            //                     where d.isJiesuanFee== fl && d.borrowDate == nowTime && d.borrowUnit.Contains("")
            //                     orderby d.ID descending
            //                     select d;

            //    }
            //    else
            //    {
            //         model1 = (from e in db.BorrowRegistration
            //                      where e.isJiesuanFee == fl && e.borrowDate == nowTime && e.borrowUnit.Contains("")
            //                      orderby e.ID descending
            //                      select e).Union(from f in db.BorrowRegistration
            //                                      where f.isJiesuanFee.ToString() == null && f.borrowUnit.Contains("")
            //                                      orderby f.ID descending
            //                                      select f);

            //    }


            //}
            //else
            //{
            //     model1 = (from g in db.BorrowRegistration
            //                  where g.isJiesuanFee == flag0 && g.borrowDate == nowTime && g.borrowUnit.Contains("")
            //                  orderby g.ID, g.isJiesuanFee descending
            //                  select g).Union(from h in db.BorrowRegistration
            //                                  where h.isJiesuanFee.ToString() == flag[1].ToString().Trim() && h.borrowUnit.Contains("")
            //                                  orderby h.ID descending
            //                                  select h).Union(from i in db.BorrowRegistration
            //                                                  where i.isJiesuanFee.ToString() == flag[2].ToString().Trim() && i.borrowUnit.Contains("")
            //                                                  orderby i.ID descending
            //                                                  select i);

            //}


            if (model1.Count() != 0)
            {

                return model1;
            }
            else
            {
                return null;
            }
        }

        //private int getFieldAndIndexAndInfo(int flag)//flag:0,其他档案，1竣工等3个档案

        //{
        //    string value = "";
        //    string index = "";
        //    string searchField = "";
        //    string andOr = "";

        //    if (flag == 0)//其他档案
        //    {
        //        foreach (ListItem item in lboxContent.Items)
        //        {
        //            string[] strText = item.Text.Split('-');
        //            string[] str = item.Value.Split('-');
        //            if (strText[0].Contains("其他") == true || strText[1].Contains("其他") == true)
        //            {
        //                if (str.Length == 4)
        //                {
        //                    value += str[3] + ",";
        //                }
        //                else
        //                {
        //                    for (int i = 3; i < str.Length - 1; i++)
        //                    {
        //                        value += str[i] + "-";
        //                    }
        //                    value += str[str.Length - 1] + ",";
        //                }
        //                index += str[2] + ",";
        //                searchField += str[1] + ",";
        //                andOr += str[0] + ",";
        //            }

        //        }
        //    }
        //    if (flag == 1)//竣工等3个档案

        //    {
        //        foreach (ListItem item in lboxContent.Items)
        //        {
        //            string[] strText = item.Text.Split('-');
        //            string[] str = item.Value.Split('-');
        //            if (strText[0].Contains("其他") != true && strText[1].Contains("其他") != true)
        //            {
        //                if (str.Length == 4)
        //                {
        //                    value += str[3] + ",";
        //                }
        //                else
        //                {
        //                    for (int i = 3; i < str.Length - 1; i++)
        //                    {
        //                        value += str[i] + "-";
        //                    }
        //                    value += str[str.Length - 1] + ",";
        //                }
        //                index += str[2] + ",";
        //                searchField += str[1] + ",";
        //                andOr += str[0] + ",";
        //            }

        //        }
        //    }
        //    lbMessage.Text = "";
        //    if (value.Length > 1)
        //    {
        //        value = value.Substring(0, value.Length - 1);
        //        index = index.Substring(0, index.Length - 1);
        //        searchField = searchField.Substring(0, searchField.Length - 1);
        //        andOr = andOr.Substring(0, andOr.Length - 1);
        //    }
        //    if (searchField == "")
        //    {
        //        return 0;
        //    }
        //    Session["searchField"] = searchField;
        //    Session["index"] = index;
        //    Session["info"] = value;
        //    Session["andOr"] = andOr;
        //    if (flag == 0)
        //        return 2;
        //    return 1;

        //}

        private static string GetSQL<T>(IQueryable<T> query)
        {
            return query.GetType().GetMethod("ToTraceString").Invoke(query, null).ToString();
        }
        public string GetJunGongPicListDemo(string paperSeqNo, string archiveNo, string volNo,string seqNo)
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
                int volNo1 = int.Parse(volNo);
                strPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + paperSeqNo + "\\" + archiveNo + "\\";
                string strPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath"] + paperSeqNo + "\\" + volNo1 + "\\";
                strWebPath = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongWebPath"] + paperSeqNo + "/" + archiveNo + "/";
                string strWebPath2 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongWebPath"] + paperSeqNo + "/" + volNo1 + "/";
                string strPath3 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongPath1"] + paperSeqNo + "\\" + volNo1 + "\\";
                string strWebPath3 = System.Web.Configuration.WebConfigurationManager.AppSettings["JunGongWebPath1"] + paperSeqNo + "/" + volNo1 + "/";
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
                        if (seqNo != null)
                        {
                            long paperSeqNo1 = long.Parse(paperSeqNo);
                            int seqNo1 = int.Parse(seqNo);
                            var a = from b in db.FileInfo
                                    where b.archivesNo == archiveNo
                                    where b.seqNo == seqNo1
                                    select b.fileName;
                            string name = a.First();

                            myDataRow = myTable.NewRow();
                            myDataRow["Name"] = name + ".pdf";
                            myDataRow["WebPath"] = strWebPath + name + ".pdf";
                            myTable.Rows.Add(myDataRow);
                        }
                        else
                        {
                            foreach (System.IO.FileInfo myFile in arrFileInfo)
                            {
                                myDataRow = myTable.NewRow();

                                myDataRow["Name"] = myFile.Name;

                                myDataRow["WebPath"] = strWebPath + myFile.Name;

                                myTable.Rows.Add(myDataRow);
                            }
                        }
                        //foreach (System.IO.FileInfo myFile in arrFileInfo)
                        //{
                        //    myDataRow = myTable.NewRow();

                        //    myDataRow["Name"] = myFile.Name;

                        //    myDataRow["WebPath"] = strWebPath + myFile.Name;

                        //    myTable.Rows.Add(myDataRow);
                        //}
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
                        if (seqNo != null)
                        {
                            long paperSeqNo1 = long.Parse(paperSeqNo);
                            int seqNo1 = int.Parse(seqNo);
                            var a = from b in db.FileInfo
                                    where b.archivesNo == archiveNo
                                    where b.seqNo == seqNo1
                                    select b.fileName;
                            string name = a.First();

                            myDataRow = myTable.NewRow();
                            myDataRow["Name"] = name + ".pdf";
                            myDataRow["WebPath"] = strWebPath2 + name + ".pdf";
                            myTable.Rows.Add(myDataRow);
                        }
                        else
                        {
                            foreach (System.IO.FileInfo myFile in arrFileInfo)
                            {
                                myDataRow = myTable.NewRow();

                                myDataRow["Name"] = myFile.Name;

                                myDataRow["WebPath"] = strWebPath2 + myFile.Name;

                                myTable.Rows.Add(myDataRow);
                            }
                        }
                        //foreach (System.IO.FileInfo myFile in arrFileInfo)
                        //{
                        //    myDataRow = myTable.NewRow();

                        //    myDataRow["Name"] = myFile.Name;

                        //    myDataRow["WebPath"] = strWebPath2 + myFile.Name;

                        //    myTable.Rows.Add(myDataRow);
                        //}
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
                        if (seqNo != null)
                        {
                            myDataRow = myTable.NewRow();
                            myDataRow["Name"] = seqNo + ".pdf";
                            myDataRow["WebPath"] = strWebPath3 + seqNo + ".pdf";
                            myTable.Rows.Add(myDataRow);
                        }
                        else
                        {
                            foreach (System.IO.FileInfo myFile in arrFileInfo)
                            {
                                myDataRow = myTable.NewRow();

                                myDataRow["Name"] = myFile.Name;

                                myDataRow["WebPath"] = strWebPath3 + myFile.Name;

                                myTable.Rows.Add(myDataRow);
                            }
                        }
                        //foreach (System.IO.FileInfo myFile in arrFileInfo)
                        //{
                        //    myDataRow = myTable.NewRow();

                        //    myDataRow["Name"] = myFile.Name;

                        //    myDataRow["WebPath"] = strWebPath3 + myFile.Name;

                        //    myTable.Rows.Add(myDataRow);
                        //}
                    }
                }
            }

            return JsonConvert.SerializeObject(myTable);
        }
        public ActionResult borrowerHasRegistedInfo()
        {



            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "借阅人姓名", Value = "0"},
                new SelectListItem { Text = "借阅单位", Value = "1"},

            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text", "");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值



            DateTime nowTime = DateTime.Now.Date;




            var borrow = (from c in db.BorrowRegistration
                          where c.isJiesuanFee == 1 && c.borrowDate == nowTime
                          select c).Union(from c in db.BorrowRegistration
                                          where c.isJiesuanFee == 2 && c.borrowDate == nowTime
                                          select c).Union(from c in db.BorrowRegistration
                                                          where c.isJiesuanFee == 3 && c.borrowDate == nowTime
                                                          select c).OrderByDescending(s => s.borrowDate).ThenByDescending(s => s.borrowSeqNo);


            ViewBag.result1 = JsonConvert.SerializeObject(borrow);

            return View();

        }
        [HttpPost]
        public ActionResult borrowerHasRegistedInfo(long? id)
        {


            return RedirectToAction("RegisUser", "UrbanBorrow");
        }
        public string HasRegistedInfo(string SearchString, int SelectedID, string name)
        {
            var borrow = from c in db.BorrowRegistration
                         orderby c.ID descending
                         select c;
            //int count = borrow.Count();
            //int count1 = 0;//用于判断是否检索到值
            DateTime nowTime = DateTime.Now.Date;
            if (name == "Find")
            {

                int t = SelectedID;
                if (!String.IsNullOrEmpty(SearchString))
                {
                    switch (t)
                    {
                        case 0:
                            borrow = (from c in db.BorrowRegistration
                                      where c.isJiesuanFee == 1 && c.borrowDate == nowTime && c.borrower.Contains(SearchString)
                                      select c).Union(from c in db.BorrowRegistration
                                                      where c.isJiesuanFee == 2 && c.borrowDate == nowTime && c.borrower.Contains(SearchString)
                                                      select c).Union(from c in db.BorrowRegistration
                                                                      where c.isJiesuanFee == 3 && c.borrowDate == nowTime && c.borrower.Contains(SearchString)
                                                                      select c).OrderBy(s => s.isJiesuanFee).ThenBy(s => s.ID);//根据借阅者姓名搜索

                            break;
                        case 1:
                            borrow = (from c in db.BorrowRegistration
                                      where c.isJiesuanFee == 1 && c.borrowDate == nowTime && c.singleOrDepart.Contains(SearchString)
                                      select c).Union(from c in db.BorrowRegistration
                                                      where c.isJiesuanFee == 2 && c.borrowDate == nowTime && c.singleOrDepart.Contains(SearchString)
                                                      select c).Union(from c in db.BorrowRegistration
                                                                      where c.isJiesuanFee == 3 && c.borrowDate == nowTime && c.singleOrDepart.Contains(SearchString)
                                                                      select c).OrderBy(s => s.isJiesuanFee).ThenBy(s => s.ID); //根据借阅单位搜索

                            break;


                    }


                }

                //if (count==count1)
                //{
                //    BorrowRegistration borrow1 = new BorrowRegistration();
                //    return JsonConvert.SerializeObject(borrow1);
                //}
                //else
                //{
                return JsonConvert.SerializeObject(borrow);
                //}
            }
            if (name == "Showtoday")
            {
                borrow = (from c in db.BorrowRegistration
                          where c.isJiesuanFee == 1 && c.borrowDate == nowTime
                          select c).Union(from c in db.BorrowRegistration
                                          where c.isJiesuanFee == 2 && c.borrowDate == nowTime
                                          select c).Union(from c in db.BorrowRegistration
                                                          where c.isJiesuanFee == 3 && c.borrowDate == nowTime
                                                          select c).OrderBy(s => s.isJiesuanFee).ThenBy(s => s.ID);
                return JsonConvert.SerializeObject(borrow);
            }
            if (name == "Showall")
            {
                borrow = from c in db.BorrowRegistration
                         orderby c.ID descending
                         select c;
                return JsonConvert.SerializeObject(borrow);
            }


            return JsonConvert.SerializeObject(borrow);
        }
        protected string saveImageAndArchives(string filePathWeb, string userid, string paperProjectSeqNo, string ArchiveNo)
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

            string archiveNo = getArchiveNo(paperProjectSeqNo, ArchiveNo);

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
                buaaModel.type = 1;
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
                BUAI.archivesNo = archiveNo;
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

        //protected string saveImageAndArchives1(string filePathWeb, string userid, string paperProjectSeqNo, string ArchiveNo)
        //{
        //    FileOperationBLL bll = new FileOperationBLL();
        //    DateTime date = DateTime.Now.Date;
        //    if (userid.Trim() == "" || userid.Trim() == "undefined")
        //    {
        //        //"您尚未选择借阅人";
        //        return "1";
        //    }
        //    long ID = long.Parse(userid.Trim());
        //    var borModel = from a in db.BorrowRegistration
        //                   where a.ID == ID
        //                   select a;
        //    string strUserId = string.Empty;
        //    if (borModel.Count() != 0)
        //    {
        //        strUserId = borModel.First().seqNo.Trim();//实际为收费编号
        //        borModel.First().isJiesuanFee = 2;
        //        db.Entry(borModel.First()).State = EntityState.Modified;

        //    }
        //    else
        //    {
        //        // "您尚未登记，请先登记";
        //        return "2";
        //    }
        //    string archiveNo = getArchiveNo(paperProjectSeqNo, ArchiveNo);
        //    var BUAImodel = from c in db.BindUserAndImageDown
        //                    where c.userID == strUserId && c.imageTime == date && c.ImageAddress == filePathWeb
        //                    select c;

        //    if (BUAImodel.Count() == 0)
        //    {
        //        BindUserAndImageDown BUAI = new BindUserAndImageDown();
        //        BUAI.imageTime = date;
        //        BUAI.userID = strUserId;
        //        BUAI.ImageAddress = bll.GetWebPathInDb(filePathWeb);
        //        BUAI.archivesNo = archiveNo;
        //        BUAI.realuserID = int.Parse(userid.Trim());
        //        db.BindUserAndImageDown.Add(BUAI);
        //        db.SaveChanges();
        //        // "保存成功";
        //        return "3";
        //    }
        //    else
        //    {
        //        // "此图纸已保存";
        //        return "4";
        //    }
        //}

        protected string saveImageAndArchives1(string filePathWeb, string userid, string paperProjectSeqNo, string ArchiveNo, string Cntyeci)
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
            string archiveNo = getArchiveNo(paperProjectSeqNo, ArchiveNo);
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
                BUAI.yeci = Cntyeci;
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

        private string getArchiveNo(string paperProjectSeqNo, string ArchiveNo)
        {
            string strpaperProjectSeqNo = "";
            string strArchiveNo = "";
            if (paperProjectSeqNo != null && paperProjectSeqNo != "")
            {
                strpaperProjectSeqNo = paperProjectSeqNo.Trim();

            }
            else
            {
                Response.Write("<script language=\'javascript\'>alert(\'没有项目顺序号，请返回检查！\');</script>");
                return null;
            }
            if (ArchiveNo != null && ArchiveNo != "")
            {
                strArchiveNo = ArchiveNo.Trim();

            }
            return strpaperProjectSeqNo + "/" + strArchiveNo;
        }
        private string getSeriNo(long? id)
        {
            string str = "";//str=项目顺序号/第几卷
            var model = from a in db.BindUserAndArchives
                        where a.userID == id
                        select a;

            foreach (var dr in model)
            {
                int type = Int32.Parse(dr.type.ToString());
                if (type == 1)//竣工档案
                {
                    string archiNo = dr.archiveNo.ToString();
                    str += "竣工";
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
                    str += "规划";
                    str += archiNo + ",";
                }
                if (type == 5)//其他档案
                {
                    string[] archiNo = dr.archiveNo.ToString().Split('/');

                    str += "请照";
                    str += archiNo[1] + ",";
                }
                if (type == 6)//征地档案
                {
                    string archiNo = dr.archiveNo.ToString();
                    str += archiNo + ",";
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
            var Archives = from a in db.ArchivesDetail
                           where a.archivesNo == archiveNo
                           select a;
            string str = "";
            if (Archives.Count() != 0)
            {
                ArchivesDetail model = Archives.First();

                projSeqNo = model.paperProjectSeqNo.ToString();
                //dijijuan = getFinishArchDJVol(projSeqNo, model.registrationNo);
                dijijuan = model.volNo.ToString();
                str = projSeqNo + "/" + dijijuan;
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
            if (cassete.Count() != 0)
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
                          where a.videoArchiveNo == archiveNo
                          select a;
            string str = "";
            if (cassete.Count() != 0)
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
        public PartialViewResult Siaomiaojian(long? id, int? page)
        {

            long ID = Convert.ToInt32(id);
            DateTime time = DateTime.Now.Date;
            var model = from a in db.BorrowRegistration
                        where a.ID == id
                        select a;
            var model1 = from b in db.BindUserAndImage
                         select b;
            if (model.Count() != 0)
            {
                time = Convert.ToDateTime(model.First().borrowDate);
                model1 = from b in db.BindUserAndImage
                         where b.imageTime == time && b.realuserID == id
                         orderby b.ID
                         select b;
                int i = 1;
                foreach (var item in model1)
                {

                    item.recordID = i;
                    i = i + 1;
                    string address = item.ImageAddress.ToString().Trim();
                    if (address != "")
                    {
                        string[] add = address.Split('/');
                        item.ImageAddress = add[add.Length - 1];
                    }

                }
                if (model1.Count() == 0)
                {
                    ViewData["checkname"] = 1;
                }
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return PartialView(model1.ToPagedList(pageNumber, pageSize));

        }
        public class FileOperationBLL
        {
            public FileOperationBLL()
            {

            }
            //获得文件IP地址对应的绝对路径
            public string GetVirtualDirectory(string _portNumber, string _name)
            {
                // 获取网站的标识符，默认为1
                string identifier = null;
                //http://localhost/contract/21081244/S6005019.JPG
                StringBuilder sbuilder = new StringBuilder();
                for (int i = 0; i < _name.Length; i++)
                {
                    if (_name[i].ToString() == "/")
                    {
                        sbuilder.Append(i.ToString() + ",");
                    }
                }
                string str = sbuilder.ToString();
                string[] indexS = str.Substring(0, str.Length - 1).Split(',');
                int startIndex = Int32.Parse(indexS[1]);
                int length = Int32.Parse(indexS[2]) - startIndex - 1;
                string iis = "IIS://" + _name.Substring(startIndex + 1, length).ToUpper() + "/W3SVC";//localhost
                System.DirectoryServices.DirectoryEntry root = new System.DirectoryServices.DirectoryEntry(iis);
                if (root == null)
                {
                    return null;
                }
                if (root.Children == null)
                {
                    return null;
                }
                foreach (System.DirectoryServices.DirectoryEntry e in root.Children)
                {
                    if (e.SchemaClassName == "IIsWebServer")
                    {
                        foreach (object property in e.Properties["ServerBindings"])
                        {
                            if (property.Equals(":" + _portNumber + ":"))
                            {
                                identifier = e.Name;
                                break;
                            }
                        }
                        if (identifier != null)
                        {
                            break;
                        }
                    }
                }

                if (identifier == null)
                {
                    identifier = "1";
                }
                string directory = iis + "/" + identifier;
                System.DirectoryServices.DirectoryEntry _iisServer = new System.DirectoryServices.DirectoryEntry(directory);
                System.DirectoryServices.DirectoryEntry folderRoot = _iisServer.Children.Find("ROOT", "IIsWebVirtualDir");
                if (folderRoot == null)
                {
                    return null;
                }
                if (folderRoot.Children == null)
                {
                    return null;
                }
                System.DirectoryServices.DirectoryEntry existPath = null;
                try
                {
                    //http://localhost/contract/21081244/S6005019.JPG
                    startIndex = Int32.Parse(indexS[2]);
                    length = Int32.Parse(indexS[3]) - startIndex - 1;
                    string contract = _name.Substring(startIndex + 1, length);
                    existPath = folderRoot.Children.Find(contract, "IIsWebVirtualDir");
                }
                catch (Exception ex)
                {

                }
                string AbsolutePath = "";
                if (existPath != null)
                {
                    StringBuilder sb;
                    sb = new StringBuilder();
                    System.DirectoryServices.PropertyCollection props = existPath.Properties;
                    //foreach (System.DirectoryServices.PropertyValueCollection valcol in props)
                    //{
                    //    sb.Append(valcol.PropertyName);
                    //    sb.Append(":");
                    //    sb.Append(valcol.Value.ToString());
                    //    sb.Append("\r");
                    //}
                    AbsolutePath = props["Path"].Value.ToString();
                }
                string otherPath = "";///21081244/S6005019.JPG
                for (int i = 3; i < indexS.Length - 1; i++)
                {
                    startIndex = Int32.Parse(indexS[i]);
                    length = Int32.Parse(indexS[i + 1]) - startIndex - 1;
                    otherPath += _name.Substring(startIndex + 1, length);
                    otherPath += "\\";
                }
                startIndex = Int32.Parse(indexS[indexS.Length - 1]);
                length = _name.Length - startIndex - 1;
                otherPath += _name.Substring(startIndex + 1, length);
                string path = AbsolutePath + "\\" + otherPath;
                return path;
            }
            //将文件IP地址的前两节去掉，如 http://222.195.148.168/OtherArchives/Licence/Tiff1.tiff 改为 /Licence/Tiff1.tiff
            public string GetWebPathInDb(string webPath)
            {
                int index = 0;
                int count = 0;
                for (int i = 0; i < webPath.Length; i++)
                {
                    if (webPath[i] == '/')
                    {
                        count++;
                        if (count == 3)
                        {
                            index = i; break;
                        }
                    }
                }
                return webPath.Substring(index);
            }

            //获得虚拟路径
            public string GetServerVirtualPath()
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["ServerVirtualPath"];
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
                    if (str == "")
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
        private bool matchString(string s, string ids)
        {//判断IDS 是否为 s的子集

            bool flag = false;
            int index = s.IndexOf(',');
            if (index == -1)
            {
                if (ids.Trim() == s.Trim())
                {
                    flag = true;
                }
            }
            else
            {
                string[] mainStr = s.Split(',');
                int ndex = ids.IndexOf(',');
                if (ndex == -1)
                {
                    foreach (string str in mainStr)
                    {
                        if (ids.Trim() == str.Trim())
                        {
                            flag = true; break;
                        }
                    }
                }
                else
                {
                    string[] subStr = ids.Split(',');
                    foreach (string ss in subStr)
                    {
                        int len = 0;
                        foreach (string ms in mainStr)
                        {
                            if (ms.Trim() != ss.Trim())
                            {
                                len++;
                            }
                        }
                        if (len == mainStr.Length)
                        {//此时ss不在mianStr数组中,即函参中ids不是s 的子集

                            return false;
                        }

                    }
                    flag = true;
                }

            }
            return flag;
        }
        public ActionResult test()
        {
            return View();
        }


        public void DownLoadFile(string id, string filename)
        {


            //string filePath = Request.MapPath(xdlujing);
            //以字符流的形式下载文件
            if (id.Contains("JunGongArchives1"))
            {

                id = id.Replace("http://192.168.0.114:8003", "G://");

            }
            else
            {

                id = id.Replace("http://192.168.0.114:8003", "H://");

            }
         

            FileStream fs = new FileStream(id, FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();

        }

        public void sleDownLoadFile(string id, string name)
        {
            string[] arrid = Regex.Split(id, ",", RegexOptions.IgnoreCase);
            //string[] arrid = id;
            //string[] arrname = Regex.Split(filename, ",", RegexOptions.IgnoreCase);
            if (arrid.Length >= 0) {
                sleDownLoadFile1(id,name);
            }
            else {
                MemoryStream ms = new MemoryStream();
                byte[] buffer = null;
                using (ZipFile file = ZipFile.Create(ms))
                {
                    file.BeginUpdate();
                    file.NameTransform = new MyNameTransfom();//通过这个名称格式化器，可以将里面的文件名进行一些处理。默认情况下，会自动根据文件的路径在zip中创建有关的文件夹。
                    for (int i = 0; i < arrid.Count(); i++)
                    {
                        string path1 = "";
                        if (arrid[i].Contains("JunGongArchives1"))
                        {
                            
                            if (arrid[i].IndexOf(".pdf") != -1)
                            {
                                path1 = arrid[i].Replace("/JunGongArchives1", "G:/JunGongArchives1/temporary");
                            }
                            else
                            {
                                path1 = arrid[i].Insert(0, "G:");
                                //path1 = arrid[i].Insert(0, "E:/数据/jpg");
                            }
                        }
                        else
                        {
                            if (arrid[i].IndexOf(".pdf") != -1)
                            {
                                path1 = arrid[i].Replace("/JunGongArchives", "G:/JunGongArchives/temporary");
                            }
                            else
                            {
                                path1 = arrid[i].Insert(0, "G:");
                                //path1 = arrid[i].Insert(0, "E:/数据/jpg");
                            }
                            
                        }

                        //path1 = arrid[i].Replace("http://222.195.148.165:8013", "E:/数据/jpg");
                        path1 = path1.Replace("/", "\\");
                        file.Add(path1);
                    }
                    file.CommitUpdate();
                    //buffer = new byte[ms.Length];
                    buffer = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(buffer, 0, buffer.Length);

                    ms.Close();
                }
                Response.ContentType = "application/x-zip-compressed";
                Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(name, System.Text.Encoding.UTF8) + ".zip");
                Response.BinaryWrite(buffer);
                Response.Flush();
                Response.End();
            }
        }

        void DownLoad (string file,string fileName)
        {

            if (System.IO.File.Exists(file))
            {
                const long ChunkSize = 102400;//100K 每次读取文件，只读取100K，这样可以缓解服务器的压力 
                byte[] buffer = new byte[ChunkSize];

                Response.Clear();
                System.IO.FileStream iStream = System.IO.File.OpenRead(file);
                long dataLengthToRead = iStream.Length;//获取下载的文件总大小 
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName) + ".zip");
                while (dataLengthToRead > 0 && Response.IsClientConnected)
                {
                    int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(ChunkSize));//读取的大小 
                    Response.OutputStream.Write(buffer, 0, lengthRead);
                    Response.Flush();
                    dataLengthToRead = dataLengthToRead - lengthRead;
                }
                Response.Close();
            }
        }

        // Edit By LvMing
        //设计思路  批量下载扫描件 现在 服务器上的 G:\\Down 目录下 将要下载的内容放进去  然后压缩  下载成功后删除  服务器上的临时文件夹

        public ActionResult sleDownLoadFile1(string id, string name)
        {
            string rootpath = "G:\\Down";
            string StoreFile = "G:\\Down\\" + name;
            string ZipFilePath = "G:\\Down\\" + name + "1";

            DirectoryInfo dir = new DirectoryInfo(rootpath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
            if (fileinfo.Length > 1) {
                return Content("<script >alert('该文件正在下载中...');window.history.back();</script >");
            }

            //清空临时文件夹，以防出错
            DelectDir(rootpath);

            DirectoryInfo dir1 = new DirectoryInfo(rootpath);
            FileSystemInfo[] fileinfo1 = dir.GetFileSystemInfos();
            if (fileinfo1.Length > 0)
            {
                return Content("<script >alert('该文件仍在连接中，请关闭其它下载窗口，重新下载！');window.history.back();</script >");
            }

            string[] arrid = Regex.Split(id, ",", RegexOptions.IgnoreCase);//要下载的图片

            if (arrid.Length > 0 && (!Directory.Exists(StoreFile)))
            {
                Directory.CreateDirectory(StoreFile);
            }
            for (int i = 0; i < arrid.Count(); i++)
            {
                string path1 = "";
                //if (arrid[i].Contains("JunGongArchives1"))
                //{
                //    path1 = arrid[i].Insert(0, "G:");
                //}
                //else
                //{
                //    path1 = arrid[i].Insert(0, "H:");
                //}
                if (arrid[i].Contains("JunGongArchives1"))
                {

                    if (arrid[i].IndexOf(".pdf") != -1)
                    {
                        path1 = arrid[i].Replace("/JunGongArchives1", "G:/JunGongArchives1/temporary");
                    }
                    else
                    {
                        path1 = arrid[i].Insert(0, "G:");
                        //path1 = arrid[i].Insert(0, "E:/数据/jpg");
                    }
                }
                else
                {
                    if (arrid[i].IndexOf(".pdf") != -1)
                    {
                        path1 = arrid[i].Replace("/JunGongArchives", "G:/JunGongArchives/temporary");
                    }
                    else
                    {
                        path1 = arrid[i].Insert(0, "G:");
                        //path1 = arrid[i].Insert(0, "E:/数据/jpg");
                    }

                }


                //path1 = arrid[i].Replace("http://222.195.148.165:8013", "E:/数据/jpg");
                path1 = path1.Replace("/", "\\");
                if (System.IO.File.Exists(path1))
                {
                    //参数1：要复制的源文件路径，参数2：复制后的目标文件路径，参数3：是否覆盖相同文件名
                    string strName = path1.Substring(path1.LastIndexOf('\\') + 1);//文件名
                    string soreFullname = StoreFile + "\\" + strName;//新文件的路径
                    System.IO.File.Copy(path1, soreFullname, true);
                }
               
            }
            //以上代码将要下载的图片保存在服务器上的一个文件夹里

            //接着将该文件夹压缩
            CreateZip(StoreFile, ZipFilePath);

            //将压缩好的文件夹在打包到本地
            DownLoad(ZipFilePath, name);

            //删除服务器上的临时文件夹
            DelectDir(rootpath);

            return Content("<script >alert('下载成功！');window.history.back();</script >");
        }
        public static void DelectDir(string srcPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {           //如果 使用了 streamreader 在删除前 必须先关闭流 ，否则无法删除 sr.close();
                        System.IO.File.Delete(i.FullName);      //删除指定文件
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        public static void CreateZip(string sourceFilePath, string destinationZipFilePath)
        {
            if (sourceFilePath[sourceFilePath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                sourceFilePath += System.IO.Path.DirectorySeparatorChar;

            ZipOutputStream zipStream = new ZipOutputStream(System.IO.File.Create(destinationZipFilePath));
            zipStream.SetLevel(6);  // 压缩级别 0-9
            CreateZipFiles(sourceFilePath, zipStream, sourceFilePath);

            zipStream.Finish();
            zipStream.Close();
        }


        private static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream, string staticFile)
        {
            Crc32 crc = new Crc32();
            string[] filesArray = Directory.GetFileSystemEntries(sourceFilePath);
            foreach (string file in filesArray)
            {
                if (Directory.Exists(file))                     //如果当前是文件夹，递归
                {
                    CreateZipFiles(file, zipStream, staticFile);
                }

                else //如果是文件，开始压缩
                {
                    FileStream fileStream = System.IO.File.OpenRead(file);

                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    string tempFile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(tempFile);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fileStream.Length;
                    fileStream.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipStream.PutNextEntry(entry);

                    zipStream.Write(buffer, 0, buffer.Length);
                }
            }
        }




    }
    namespace ClassLibrary1
    {
        /*
        该类主要是发送指令，添加打印任务
       */

        public class PrinterJob
        {
            /// <summary>
            /// OpenPrinter 打开指定的打印机，并获取打印机的句柄 
            /// </summary>
            /// <param name="szPrinter">要打开的打印机的名字</param>
            /// <param name="hPrinter">用于装载打印机的句柄</param>
            /// <param name="pd">PRINTER_DEFAULTS，这个结构保存要载入的打印机信息</param>
            /// <returns>bool</returns>
            [System.Runtime.InteropServices.DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);
            /// <summary>
            /// ClosePrinter 关闭一个打开的打印机对象
            /// </summary>
            /// <param name="hPrinter">一个打开的打印机对象的句柄</param>
            /// <returns></returns>
            [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool ClosePrinter(IntPtr hPrinter);
            /// <summary>
            /// StartDocPrinter 在后台打印的级别启动一个新文档
            /// </summary>
            /// <param name="hPrinter">指定一个已打开的打印机的句柄（用openprinter取得）</param>
            /// <param name="level">1或2（仅用于win95）</param>
            /// <param name="di">包含一个DOC_INFO_1或DOC_INFO_2结构得缓冲区</param>
            /// <returns>bool 注: 在应用程序的级别并非有用。后台打印程序用它标识一个文档的开始</returns>
            [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);
            /// <summary>
            /// EndDocPrinter 在后台打印程序的级别指定一个文档的结束
            /// </summary>
            /// <param name="hPrinter">一个已打开的打印机的句柄（用用OpenPrinter获得）</param>
            /// <returns>bool</returns>
            [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool EndDocPrinter(IntPtr hPrinter);
            /// <summary>
            /// StartPagePrinter 在打印作业中指定一个新页的开始 
            /// </summary>
            /// <param name="hPrinter">指定一个已打开的打印机的句柄（用openprinter取得）</param>
            /// <returns>bool注:在应用程序的级别并非特别有用</returns>
            [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool StartPagePrinter(IntPtr hPrinter);
            /// <summary>
            /// EndPagePrinter 指定一个页在打印作业中的结尾 
            /// </summary>
            /// <param name="hPrinter">一个已打开的打印机对象的句柄（用OpenPrinter获得）</param>
            /// <returns>bool</returns>
            [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool EndPagePrinter(IntPtr hPrinter);
            /// <summary>
            /// WritePrinter 将发送目录中的数据写入打印机 
            /// </summary>
            /// <param name="hPrinter">指定一个已打开的打印机的句柄（用openprinter取得）</param>
            /// <param name="pBytes">任何类型，包含了要写入打印机的数据的一个缓冲区或结构</param>
            /// <param name="dwCount">dwCount缓冲区的长度</param>
            /// <param name="dwWritten">指定一个Long型变量，用于装载实际写入的字节数</param>
            /// <returns>bool</returns>
            [DllImport("winspool.Drv", EntryPoint = "WritePrinter", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);
            /// <summary>
            /// GetPrinter 取得与指定打印机有关的信息
            /// </summary>
            /// <param name="handle">一个已打开的打印机的句柄（用OpenPrinter获得）</param>
            /// <param name="level">1，2，3（仅适用于NT），4（仅适用于NT），或者5（仅适用于Windows 95 和 NT 4.0）</param>
            /// <param name="buffer">包含PRINTER_INFO_x结构的缓冲区。x代表级别</param>
            /// <param name="size">pPrinterEnum缓冲区中的字符数量</param>
            /// <param name="sizeNeeded">指向一个Long型变量的指针，该变量用于保存请求的缓冲区长度，或者实际读入的字节数量</param>
            /// <returns></returns>
            [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool GetPrinter(IntPtr handle, UInt32 level, IntPtr buffer, UInt32 size, out UInt32 sizeNeeded);
            /// <summary>
            /// EnumPrinters 枚举系统中安装的打印机
            /// </summary>
            /// <param name="flags">一个或多个下述标志
            /// PRINTER_ENUM_LOCAL 枚举本地打印机（包括Windows 95中的网络打印机）。名字会被忽略 
            ///PRINTER_ENUM_NAME 枚举由name参数指定的打印机。其中的名字可以是一个供应商、域或服务器。如name为NULL，则枚举出可用的打印机 
            ///PRINTER_ENUM_SHARE 枚举共享打印机（必须同其他常数组合使用） 
            ///PRINTER_ENUM_CONNECTIONS 枚举网络连接列表中的打印机（即使目前没有连接——仅适用于NT） 
            ///PRINTER_ENUM_NETWORK 枚举通过网络连接的打印机。级别（Level）必须为1。仅适用于NT 
            ///PRINTER_ENUM_REMOTE 枚举通过网络连接的打印机和打印服务器。级别必须为1。仅适用于NT 
            ///</param>
            /// <param name="name">null表示枚举同本机连接的打印机。否则由标志和级别决定</param>
            /// <param name="level">1，2，4或5（4仅适用于NT；5仅适用于Win95和NT 4.0），指定欲枚举的结构的类型。如果是1，则name参数由标志设置决定。如果是2或5，那么name就代表欲对其打印机进行枚举的服务器的名字；或者为vbNullString。如果是4，那么只有PRINTER_ENUM_LOCAL和PRINTER_ENUM_CONNECTIONS才有效。名字必须是vbNullString</param>
            /// <param name="pPrinterEnum">包含PRINTER_ENUM_x结构的缓冲区，其中的x代表级别（Level）</param>
            /// <param name="cbBuf">pPrinterEnum缓冲区中的字符数量</param>
            /// <param name="pcbNeeded">指向一个out Long型变量，该变量用于保存请求的缓冲区长度，或者实际读入的字节数量</param>
            /// <param name="pcReturned">载入缓冲区的结构数量（用于那些能返回多个结构的函数）</param>
            /// <returns>bool</returns>
            [DllImport("winspool.drv", CharSet = CharSet.Auto)]
            public static extern bool EnumPrinters(int flags, string name, int level, IntPtr pPrinterEnum, int cbBuf, out int pcbNeeded, out int pcReturned);
            /// <summary>
            /// 获取与指定作业有关的信息
            /// </summary>
            /// <param name="hPrinter">一个已打开的打印机的句柄（用OpenPrinter获得）</param>
            /// <param name="JobId">作业编号</param>
            /// <param name="level">1，2，3（仅适用于NT），4（仅适用于NT），或者5（仅适用于Windows 95 和 NT 4.0）</param>
            /// <param name="pPrinterEnum">包含PRINTER_INFO_x结构的缓冲区。x代表级别</param>
            /// <param name="cbBuf">pPrinterEnum缓冲区中的字符数量</param>
            /// <param name="pcbNeeded">指向一个uint32型变量的指针，该变量用于保存请求的缓冲区长度，或者实际读入的字节数量</param>
            /// <returns></returns>
            [DllImport("winspool.drv", CharSet = CharSet.Auto)]
            public static extern bool GetJob(IntPtr hPrinter, UInt32 JobId, UInt32 level, IntPtr pPrinterEnum, UInt32 cbBuf, out UInt32 pcbNeeded);
            /// <summary>
            /// 枚举打印队列中的作业
            /// </summary>
            /// <param name="hPrinter">一个已打开的打印机对象的句柄（用OpenPrinter获得）</param>
            /// <param name="FirstJob">作业列表中要枚举的第一个作业的索引（注意编号从0开始）</param>
            /// <param name="NoJobs">要枚举的作业数量</param>
            /// <param name="level">1或2</param>
            /// <param name="pJob">包含 JOB_INFO_1 或 JOB_INFO_2 结构的缓冲区</param>
            /// <param name="cbBuf">pJob缓冲区中的字符数量</param>
            /// <param name="pcbNeeded">指向一个Uint32型变量的指针，该变量用于保存请求的缓冲区长度，或者实际读入的字节数量</param>
            /// <param name="pcReturned">载入缓冲区的结构数量（用于那些能返回多个结构的函数）</param>
            /// <returns>bool</returns>
            [DllImport("winspool.drv", CharSet = CharSet.Auto)]
            public static extern bool EnumJobs(IntPtr hPrinter, UInt32 FirstJob, UInt32 NoJobs, UInt32 level, IntPtr pJob, UInt32 cbBuf, out UInt32 pcbNeeded, out UInt32 pcReturned);
            /// <summary>
            /// 提交一个要打印的作业
            /// </summary>
            /// <param name="hPrinter">一台已打开的打印机句柄</param>
            /// <param name="JobID">作业编号</param>
            /// <returns>bool</returns>
            [DllImport("winspool.drv", CharSet = CharSet.Auto)]
            public static extern bool ScheduleJob(IntPtr hPrinter, out UInt32 JobID);

            /// <summary>
            /// 取得与指定表单有关的信息
            /// </summary>
            /// <param name="hPrinter">打印机的句柄</param>
            /// <param name="pFormName">想获取信息的一个表单的名字</param>
            /// <param name="Level">设为1</param>
            /// <param name="pForm">包含FORM_INFO_1结构的缓冲区</param>
            /// <param name="cbBuf">pForm缓冲区中的字符数量</param>
            /// <param name="pcbNeeded">指向一个Long型变量的指针，该变量用于保存请求的缓冲区长度，或者实际读入的字节数量</param>
            /// <returns></returns>
            [DllImport("winspool.drv", CharSet = CharSet.Auto)]
            public static extern bool GetForm(IntPtr hPrinter, string pFormName, UInt32 Level, IntPtr pForm, UInt32 cbBuf, out UInt32 pcbNeeded);
            /// <summary>
            /// 枚举一台打印机可用的表单
            /// </summary>
            /// <param name="hPrinter"></param>
            /// <param name="Level"></param>
            /// <param name="pForm"></param>
            /// <param name="cbBuf"></param>
            /// <param name="pcbNeeded"></param>
            /// <param name="pcReturned"></param>
            /// <returns></returns>
            [DllImport("winspool.drv", CharSet = CharSet.Auto)]
            public static extern bool EnumForms(IntPtr hPrinter, UInt32 Level, IntPtr pForm, UInt32 cbBuf, out UInt32 pcbNeeded, out UInt32 pcReturned);

            /// <summary>
            /// 为系统添加一个打印机监视器
            /// </summary>
            /// <param name="pName"></param>
            /// <param name="Level"></param>
            /// <param name="pMonitors"></param>
            /// <returns></returns>
            [DllImport("winspool.drv", CharSet = CharSet.Auto)]
            public static extern bool AddMonitor(IntPtr pName, UInt32 Level, IntPtr pMonitors);

            /// <summary>
            /// 枚举可用的打印监视器
            /// </summary>
            /// <param name="hPrinter"></param>
            /// <param name="Level"></param>
            /// <param name="pForm"></param>
            /// <param name="cbBuf"></param>
            /// <param name="pcbNeeded"></param>
            /// <param name="pcReturned"></param>
            /// <returns></returns>
            [DllImport("winspool.drv", CharSet = CharSet.Auto)]
            public static extern bool EnumMonitors(string hPrinter, UInt32 Level, IntPtr pForm, UInt32 cbBuf, out UInt32 pcbNeeded, out UInt32 pcReturned);

            /// <summary>
            /// 针对指定的打印机，获取与打印机驱动程序有关的信息
            /// </summary>
            /// <param name="pName"></param>
            /// <param name="pEnvironment"></param>
            /// <param name="Level"></param>
            /// <param name="pDriverInfo"></param>
            /// <param name="cbBuf"></param>
            /// <param name="pcbNeeded"></param>
            /// <returns></returns>
            [DllImport("winspool.drv", CharSet = CharSet.Auto)]
            public static extern bool GetPrinterDriver(IntPtr pName, string pEnvironment, UInt32 Level, IntPtr pDriverInfo, UInt32 cbBuf, out UInt32 pcbNeeded);

            /// <summary>
            /// 一个灵活的设备控制函数
            /// </summary>
            /// <param name="pName"></param>
            /// <param name="nEscape"></param>
            /// <param name="nCount"></param>
            /// <param name="lpInData"></param>
            /// <param name="lpOutData"></param>
            /// <returns></returns>
            [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
            public static extern short Escape(IntPtr pName, UInt32 nEscape, UInt32 nCount, IntPtr lpInData, out IntPtr lpOutData);

            /// <summary>
            /// 这是一个灵活的打印机配置控制函数。
            /// 该函数定义了两个DEVMODE结构，可在创建一个设备场景时为单个应用程序改变打印机设置。
            /// 甚至能在文档打印期间改变打印机设置
            /// </summary>
            /// <param name="hwnd"></param>
            /// <param name="hPrinter"></param>
            /// <param name="pDeviceName"></param>
            /// <param name="pDevModeOutput"></param>
            /// <param name="pDevModeInput"></param>
            /// <param name="fMode"></param>
            /// <returns></returns>
            [DllImport("winspool.drv", CharSet = CharSet.Auto)]
            public static extern bool DocumentProperties(IntPtr hwnd, IntPtr hPrinter, string pDeviceName, out IntPtr pDevModeOutput, out IntPtr pDevModeInput, int fMode);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="hPrinter"></param>
            /// <returns></returns>
            [DllImport("gdi32.dll", EntryPoint = "EndDoc", CharSet = CharSet.Auto)]
            public static extern short EndDocAPI(IntPtr hPrinter);


            #region 定义的结构体

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct PRINTER_INFO_2
            {
                public string pServerName;
                public string pPrinterName;
                public string pShareName;
                public string pPortName;
                public string pDriverName;
                public string pComment;
                public string pLocation;
                public IntPtr pDevMode;
                public string pSepFile;
                public string pPrintProcessor;
                public string pDatatype;
                public string pParameters;
                public IntPtr pSecurityDescriptor;
                public UInt32 Attributes;
                public UInt32 Priority;
                public UInt32 DefaultPriority;
                public UInt32 StartTime;
                public UInt32 UntilTime;
                public UInt32 Status;
                public UInt32 cJobs;
                public UInt32 AveragePPM;

            }
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct PRINTER_INFO_1
            {
                public int flags;
                [MarshalAs(UnmanagedType.LPTStr)]
                public string pDescription;
                [MarshalAs(UnmanagedType.LPTStr)]
                public string pName;
                [MarshalAs(UnmanagedType.LPTStr)]
                public string pComment;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public class DOCINFOA
            {
                [MarshalAs(UnmanagedType.LPStr)]
                public string pDocName;
                [MarshalAs(UnmanagedType.LPStr)]
                public string pOutputFile;
                [MarshalAs(UnmanagedType.LPStr)]
                public string pDataType;
            }
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct RECT
            {
                public UInt32 Left;
                public UInt32 Top;
                public UInt32 Right;
                public UInt32 Bottom;
            }
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct FORM_INFO_1
            {
                public UInt32 Flags;
                public string pName;
                public UInt32 Size;
                public IntPtr ImageableArea;

            }
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct MONINTOR_INFO_2
            {
                public string pName;
                public string pEnvironment;
                public string pDLLName;
            }
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct JOB_INFO_1
            {
                public UInt32 Jobid;
                //[MarshalAs(UnmanagedType.LPStr)]
                public string pPrinterName;
                //[MarshalAs(UnmanagedType.LPStr)]
                public string pMachineName;
                //[MarshalAs(UnmanagedType.LPStr)]
                public string pUserName;
                //[MarshalAs(UnmanagedType.LPStr)]
                public string pDocument;
                //[MarshalAs(UnmanagedType.LPStr)]
                public string pDatatype;
                //[MarshalAs(UnmanagedType.LPStr)]
                public string pStatus;
                public UInt32 Status;
                public UInt32 Priority;
                public UInt32 Position;
                public UInt32 TotalPages;
                public UInt32 PagesPrinted;
                public IntPtr Submitted;
            }
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct JOB_INFO_2
            {
                public int Jobid;
                public string pPrinterName;
                public string pMachineName;
                public string pUserName;
                public string pDocument;
                public string pNotifyName;
                public string pDatatype;
                public string pPrintProcessor;
                //[MarshalAs(UnmanagedType.LPStr)]
                public string pParameters;
                public string pDriverName;
                public IntPtr pDevMode;
                public string pStatus;
                public IntPtr pSecurityDescriptor;
                public int Status;
                public int Priority;
                public int Position;
                public int StartTime;
                public int UntilTime;
                public int TotalPages;
                public int Size;
                public IntPtr Submitted;
                public int Time;
                public int PagesPrinted;

            }
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct SYSTEMTIME
            {
                public short wYear;
                public short wMonth;
                public short wDayOfWeek;
                public short wDay;
                public short wHour;
                public short wMinute;
                public short wSecond;
                public short wMilliseconds;

            }
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct DEVMODE
            {
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public string dmDeviceName;

                public short dmSpecVersion;
                public short dmDriverVersion;
                public short dmSize;
                public short dmDriverExtra;
                public int dmFields;
                public int dmPositionX;
                public int dmPositionY;
                public int dmDisplayOrientation;
                public int dmDisplayFixedOutput;
                public short dmColor;
                public short dmDuplex;
                public short dmYResolution;
                public short dmTTOption;
                public short dmCollate;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public string dmFormName;

                public short dmLogPixels;
                public short dmBitsPerPel;
                public int dmPelsWidth;
                public int dmPelsHeight;
                public int dmDisplayFlags;
                public int dmDisplayFrequency;
                public int dmICMMethod;
                public int dmICMIntent;
                public int dmMediaType;
                public int dmDitherType;
                public int dmReserved1;
                public int dmReserved2;
                public int dmPanningWidth;
                public int dmPanningHeight;
            };
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct MONITOR_INFO_1
            {
                public string pName;
                //public string pEnvironment;
                //public string pDLLName; 

            };
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct DRIVER_INFO_1
            {
                public string pName;
            };
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct DRIVER_INFO_2
            {
                public int cVersion;
                public string pName;
                public string pEnvironment;
                public string pDriverPath;
                public string pDataFile;
                public string pConfigFile;

            };
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct DRIVER_INFO_3
            {
                public int cVersion;
                public string pName;
                public string pEnvironment;
                public string pDriverPath;
                public string pDataFile;
                public string pConfigFile;
                public string pHelpFile;
                public string pDependentFiles;
                public string pMonitorName;
                public string pDefaultDataType;
            };

            #endregion

            /// <summary>
            /// 为专门设备创建设备场景
            /// </summary>
            /// <param name="pDrive"></param>
            /// <param name="pName"></param>
            /// <param name="pOutput"></param>
            /// <param name="pDevMode"></param>
            /// <returns></returns>
            [DllImport("GDI32.dll", EntryPoint = "CreateDC", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false,
                        CallingConvention = CallingConvention.StdCall)]
            internal static extern IntPtr CreateDC([MarshalAs(UnmanagedType.LPTStr)] string pDrive,
                [MarshalAs(UnmanagedType.LPTStr)] string pName,
                [MarshalAs(UnmanagedType.LPTStr)] string pOutput,
               ref DEVMODE pDevMode);


            /// <summary>
            /// 为专用设备创建一个信息场景。
            /// 信息场景可用来快速获取某设备的信息而无须创建设备场景这样的系统开销。
            /// 它可作为参数传递给GetDeviceCaps一类的信息函数以替代设备场景参数
            /// </summary>
            /// <param name="pDrive">用vbNullString传递null值给该参数，
            /// 除非：1、用DISPLAY，是获取整个屏幕的设备场景；2、用WINSPOOL，则是访问打印驱动</param>
            /// <param name="pName"></param>
            /// <param name="pOutput"></param>
            /// <param name="pDevMode"></param>
            /// <returns></returns>
            [DllImport("GDI32.dll", EntryPoint = "CreateIC", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false,
                        CallingConvention = CallingConvention.StdCall)]
            internal static extern IntPtr CreateIC([MarshalAs(UnmanagedType.LPTStr)]
            string pDrive,
                [MarshalAs(UnmanagedType.LPTStr)] string pName,
                [MarshalAs(UnmanagedType.LPTStr)] string pOutput,
               ref DEVMODE pDevMode);

            [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
            public static extern bool Rectangle(IntPtr hwnd, int x, int y, int x1, int y1);
            [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr CreateEnhMetaFile(IntPtr hdcRef, string lpFileName, ref RECT lpRect, string lpDescription);
            [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr CloseEnhMetaFile(IntPtr hdcRef);
            [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr CopyEnhMetaFile(IntPtr hdcRef, string lpszFile);
            [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetDC(IntPtr hdcRef);


            [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetDeviceCaps(IntPtr hdc, int nIndex);
            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetModuleHandle(string name);

            public struct TempFile
            {
                public string pFullName;
                public long leng;
                public string pName;
                //public string pEnvironment;
                //public string pDLLName; 

            };

            #region 发送数据，添加作业

            public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
            {
                Int32 dwError = 0, dwWritten = 0;
                IntPtr hPrinter = new IntPtr(0);
                DOCINFOA di = new DOCINFOA();
                bool bSuccess = false; // Assume failure unless you specifically succeed.

                di.pDocName = "测试文档";
                di.pDataType = "RAW";  //RAW     在虚拟打印机上测试必须将pDataType = "RAW"  改为 TEXT

                // Open the printer.
                if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    // Start a document.
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        // Start a page.
                        if (StartPagePrinter(hPrinter))
                        {
                            // Write your bytes.
                            bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);

                    }
                    //int aa= EndDocAPI(hPrinter);

                    ClosePrinter(hPrinter);
                }
                // If you did not succeed, GetLastError may give more information
                // about why not.
                if (bSuccess == false)
                {
                    dwError = Marshal.GetLastWin32Error();
                }
                return bSuccess;
            }

            public static bool SendFileToPrinter(string szPrinterName, string szFileName)
            {
                // Open the file.
                FileStream fs = new FileStream(szFileName, FileMode.Open);
                // Create a BinaryReader on the file.
                BinaryReader br = new BinaryReader(fs);
                // Dim an array of bytes big enough to hold the file's contents.
                Byte[] bytes = new Byte[fs.Length];
                bool bSuccess = false;
                // Your unmanaged pointer.
                IntPtr pUnmanagedBytes = new IntPtr(0);
                int nLength;

                nLength = Convert.ToInt32(fs.Length);
                // Read the contents of the file into the array.
                bytes = br.ReadBytes(nLength);
                // Allocate some unmanaged memory for those bytes.
                pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
                // Copy the managed byte array into the unmanaged array.
                Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
                // Send the unmanaged bytes to the printer.
                bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
                // Free the unmanaged memory that you allocated earlier.
                Marshal.FreeCoTaskMem(pUnmanagedBytes);
                return bSuccess;
            }

            public static bool SendStringToPrinter(string szPrinterName, string szString)
            {
                IntPtr pBytes;
                Int32 dwCount;
                // How many characters are in the string?
                dwCount = szString.Length;
                // Assume that the printer is expecting ANSI text, and then convert
                // the string to ANSI text.
                pBytes = Marshal.StringToCoTaskMemAnsi(szString);
                // Send the converted ANSI string to the printer.
                SendBytesToPrinter(szPrinterName, pBytes, dwCount);
                Marshal.FreeCoTaskMem(pBytes);
                return true;
            }

            #endregion
        }
    }
}
    
