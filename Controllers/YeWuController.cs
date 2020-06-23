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
    public class YeWuController : Controller
    {
        private PlanArchiveEntities db_plan = new PlanArchiveEntities();
        private UrbanConEntities db_urban = new UrbanConEntities();
        private UrbanUsersEntities db_user = new UrbanUsersEntities();
        private OfficeEntities db_office = new OfficeEntities();
        ReportDocument rptH = new ReportDocument();
        //吕鸣 2019/7/9
        // GET: PlanProjects/Create
        public ActionResult Create_yewu(string id2, string ID, string id1)
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
            //ViewBag.classifyID = classifyID;//案卷类型ID
            //ViewBag.classifyName = db_plan.PlanArchiveClassify.Find(classifyID).classifyName.Trim();//案卷类型mame
            //ViewBag.classiftsx = db_plan.PlanArchiveClassify.Find(classifyID).classifySX;//案卷类型缩写，为了拼接盒号
            //ViewBag.box = db_plan.PlanArchiveClassify.Find(classifyID).classifyName.Trim() + "-" + db_plan.PlanArchiveClassify.Find(classifyID).classifySX.Trim();
            //ViewBag.classname = db_plan.PlanArchiveClassify.Find(classifyID).classifyName.Trim();//文号的默认值

            var maxID = from d in db_plan.businessPlanProject
                        orderby d.ID descending
                        select d;
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "公开"},
                new SelectListItem { Text = "内部", Value = "内部" },
                new SelectListItem { Text = "公开/内部", Value = "公开/内部" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text");

            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "青岛市规划局", Value ="1"},
                new SelectListItem { Text = "青岛市规划局市北分局", Value ="2"},
                new SelectListItem { Text = "青岛市规划局原四方分局", Value ="3"},
                new SelectListItem { Text = "青岛市规划局李沧分局", Value ="4"},
                new SelectListItem { Text = "青岛市规划局市南分局", Value ="5"},
            };
            ViewBag.bianzhiUnit = new SelectList(list, "Value", "Text", 1);

            //档案密级
            ViewBag.securityID = new SelectList(db_urban.SecurityClassification, "securityID", "securityName");
            //保管年限(注意这里与)
            ViewBag.retentionPeriodID = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", 2);
            //是否为内部文件

            ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", "张春颖");//审核人
            var UserID = User.Identity.GetUserId();//获取当前用户
            ViewBag.luruPerson = db_user.AspNetUsers.Find(UserID).UserName;//录入人传到前台，前台设置成不可修改
            //第一次录入时数据库无数据，故要进行判断
            if (maxID.Count() == 0)
            {
                ViewBag.ID = 0;
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
            ViewBag.supervisorybody = "青岛市建筑工程质量监督站";
            //var filename = db_plan.PlanProject.Where(a => a.classifyID == classifyID);
            //if (filename.Count() == 0)
            //{
            //    ViewBag.filenameid = 0;
            //}
            //else
            //{
            //    ViewBag.filenameid = filename.First().ID;
            //}
            if (id2 != null && id2 != "")//将上一卷的值传递到下一卷中去，针对一卷有多盒的情况
            {
                string luruperson = db_user.AspNetUsers.Find(UserID).UserName;
                var curperson = db_plan.businessPlanProject.Where(d => d.luruPerson == luruperson).OrderByDescending(d => d.ID);
                int max_ID = curperson.First().ID;
                var model = from a in db_plan.businessPlanProject
                            where a.ID == max_ID
                            select a;
                if (id1 == "0")//针对一盒有多个工程的情况，将上一个工程的盒名传递到下一个工程中去
                {
                    ViewBag.year = model.First().yearNo;
                    int a = model.First().boxNo.IndexOf('第') + 1;
                    int b = model.First().boxNo.Trim().Length;
                    int c = model.First().boxNo.IndexOf('第');
                    ViewBag.boxid = model.First().boxNo.Substring(a, b - c - 2);
                    ViewBag.boxNo = model.First().yearNo + "年" + "-" + "第" + model.First().boxNo.Substring(a, b - c - 2) + "号";//加box字段
                    ViewBag.yearwen = model.First().yearNo;


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
        public ActionResult Create_yewu([Bind(Include = "securityID,retentionPeriodID,gongjijuan,dijijuan,projectname,boxNo,developmentUnit,projectContent,projectLocation,yearNo,,classifyID,projectID,fileNo,dateReceived,remarks,bianzhiUnit,luruPerson,juanneiSeqNo,bianzhiTime,pageNo,photoCnt,archiveID,shenhePerson,isImageExist,isNeibu,projectContent_neibu,archiveTitle,archiveTitle_neibu,pageNo_neibu,developmentUnit_neibu,projectLocation_neibu,photoCnt_neibu,wenziCnt,wenziCnt_neibu")] PlanProject businessPlanProject, String action, int ID, string DELETE_ID)
        {

            //if (action == "打印备考表(公开)")
            //{
            //    return RedirectToAction("beikaobiao", new { id = ID + 1, id1 = 0 });
            //}

            //if (action == "打印工程封皮(公开)")
            //{
            //    return RedirectToAction("GuiHuaGongChengFengPi", new { id = ID + 1, id1 = 0 });
            //}
            //if (action == "打印备考表(内部)")
            //{
            //    return RedirectToAction("beikaobiao", new { id = ID + 1, id1 = 1 });
            //}

            //if (action == "打印工程封皮(内部)")
            //{
            //    return RedirectToAction("GuiHuaGongChengFengPi", new { id = ID + 1, id1 = 1 });
            //}

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
            //if (action == "删除该工程")
            //{
            //    planProject.ID = ID;
            //    db_plan.Entry(planProject).State = EntityState.Modified;
            //    try
            //    {
            //        db_plan.PlanProject.Remove(planProject);
            //        db_plan.SaveChanges();
            //        return Content("<script >alert('删除成功!');window.history.back();</script >");

            //    }
            //    catch (Exception ex)
            //    {
            //        return Content("<script >alert('信息有误！');window.history.back();</script >");

            //    }
            //}
            if (action == "添加下一工程")
            {
                if (businessPlanProject.gongjijuan > 1)
                {
                    if (businessPlanProject.dijijuan == businessPlanProject.gongjijuan)
                    {
                        return RedirectToAction("Create_yewu", new { id1 = 0, id2 = ID });
                    }
                    else
                    {
                        return RedirectToAction("Create_yewu", new { id2 = ID });
                    }
                }
                return RedirectToAction("Create_yewu", new { id1 = 0, id2 = ID/*projectname=planProject.projectname, projectLocation =planProject.projectLocation, developmentUnit=planProject.developmentUnit, projectContent=planProject.projectContent, projectContent_neibu=planProject.projectContent_neibu, archiveTitle=planProject.archiveTitle, archiveTitle_neibu=planProject.archiveTitle_neibu*/});//将上一个工程的盒号传递到下一个工程中去
            }
            //if (action == "修改")
            //{
            //    planProject.ID = ID;
            //    db_plan.Entry(planProject).State = EntityState.Modified;
            //    try
            //    {
            //        db_plan.SaveChanges();
            //        return Content("<script >alert('修改成功');window.history.back();</script >");
            //    }
            //    catch (Exception ex)
            //    {
            //        return Content("<script >alert('修改信息有误，请检查！');window.history.back();</script >");

            //    }
            //}

            return View(businessPlanProject);
        }

        public string Baocunok_yewu(string id, string securityID, string retentionPeriodID, string gongjijuan, string dijijuan, string projectname, string boxNo, string developmentUnit, string projectContent, string projectLocation, string yearNo, string projectID, string fileNo, string dateReceived, string remarks, string bianzhiUnit, string luruPerson, string juanneiSeqNo, string bianzhiTime, string pageNo, string photoCnt, string archiveID, string shenhePerson, string isImageExist, string isNeibu, string projectContent_neibu, string archiveTitle, string archiveTitle_neibu, string pageNo_neibu, string developmentUnit_neibu, string projectLocation_neibu, string photoCnt_neibu, string wenziCnt, string wenziCnt_neibu, string name, string bei_anTime, string jungongTime, string projectNumber, string buildingArea, string projectType, string structType, string kan_chaUnit, string designUnit, string constructionUnit, string supervisorUnit, string supervisorybody, string kaigongTime, string totalSeqNo, string seqNo2)
        {


            if (name == "baocun")
            {
                if (id != "" && id != null)//防止重复提交
                {
                    int Total = Int32.Parse(gongjijuan);
                    int vol = Int32.Parse(dijijuan);
                    var a = from c in db_plan.businessPlanProject
                            where c.fileNo == fileNo && c.gongjijuan == Total && c.dijijuan == vol
                            select c;
                    if (a.Count() != 0)
                    {
                        return "7";
                    }
                }
                businessPlanProject businessPlanProjectoject = new businessPlanProject();

                businessPlanProjectoject.status = "LR";
                businessPlanProjectoject.isImageExist = "无";
                businessPlanProjectoject.totalSeqNo = "";
                businessPlanProjectoject.seqNo1 = 0;
                businessPlanProjectoject.securityID = securityID;
                businessPlanProjectoject.retentionPeriodID = retentionPeriodID;
                businessPlanProjectoject.gongjijuan = Int32.Parse(gongjijuan.Trim());
                businessPlanProjectoject.dijijuan = Int32.Parse(dijijuan.Trim());
                if (boxNo == null || boxNo == "")
                {
                    businessPlanProjectoject.boxNo = "";
                }
                else

                {
                    businessPlanProjectoject.boxNo = boxNo.Trim();
                }
                if (projectname == null || projectname == "")
                {
                    businessPlanProjectoject.projectname = "";
                }
                else

                {
                    businessPlanProjectoject.projectname = projectname.Trim();
                }

                if (fileNo == null || fileNo == "")
                {
                    businessPlanProjectoject.fileNo = "";
                }
                else

                {
                    businessPlanProjectoject.fileNo = fileNo.Trim();
                }
                //为了区分不同区的同一年份同一类别的档案，保存urban_type字段
                string type = fileNo.Trim().Substring(0, 3);//大部分都是3个字符，只有市规划是两个字符“青规”，胶南是四个字符“青规胶南”
                if (fileNo.Trim().Substring(0, 4) == "青规胶南")//先判断胶南
                {
                    businessPlanProjectoject.urban_type = 10;
                }
                else if (type != "青规北" && type != "青规四" && type != "青规李" && type != "青规南" && type != "青规崂" && type != "青规城" && type != "青规黄" && type != "青规胶" && type != "青规平" && type != "青规莱" && type != "青规即" && type != "青规开")//只能是市规划了
                {
                    businessPlanProjectoject.urban_type = 1;
                }
                else
                {


                    switch (type)
                    {
                        case "青规北":
                            businessPlanProjectoject.urban_type = 2;
                            break;
                        case "青规四":
                            businessPlanProjectoject.urban_type = 3;
                            break;
                        case "青规李":
                            businessPlanProjectoject.urban_type = 4;
                            break;
                        case "青规南":
                            businessPlanProjectoject.urban_type = 5;
                            break;
                        case "青规崂":
                            businessPlanProjectoject.urban_type = 6;
                            break;
                        case "青规城":
                            businessPlanProjectoject.urban_type = 7;
                            break;
                        case "青规黄":
                            businessPlanProjectoject.urban_type = 8;
                            break;
                        case "青规胶":
                            businessPlanProjectoject.urban_type = 9;
                            break;
                        case "青规平":
                            businessPlanProjectoject.urban_type = 11;
                            break;
                        case "青规莱":
                            businessPlanProjectoject.urban_type = 12;
                            break;
                        case "青规即":
                            businessPlanProjectoject.urban_type = 13;
                            break;
                        case "青规开":
                            businessPlanProjectoject.urban_type = 14;
                            break;

                    }

                }

                businessPlanProjectoject.developmentUnit = developmentUnit;
                businessPlanProjectoject.projectContent = projectContent;
                businessPlanProjectoject.projectLocation = projectLocation;
                businessPlanProjectoject.yearNo = yearNo;

                //if (classifyID == null && classifyID == "")
                //{
                //    return "3";//工程种类不能为空
                //}
                //project.classifyID = Int32.Parse(classifyID);
                if (projectID != null && projectID != "")
                {
                    if (projectID.Trim().IndexOf('-') != -1)
                    {
                        projectID = projectID.Split('-')[0].ToString();
                    }
                    if (projectID.Trim().IndexOf('X') != -1)
                    {
                        projectID = projectID.Split('X')[1].ToString();//针对020020100X001这种情况
                    }
                    businessPlanProjectoject.projectID = Int32.Parse(projectID.Trim());
                }
                else
                {
                    businessPlanProjectoject.projectID = 0;
                }

                if (dateReceived != null && dateReceived != "")
                {
                    businessPlanProjectoject.dateReceived = DateTime.Parse(dateReceived.Trim());
                }
                else
                {
                    businessPlanProjectoject.dateReceived = DateTime.Today;
                }
                businessPlanProjectoject.remarks = remarks;
                businessPlanProjectoject.bianzhiUnit = bianzhiUnit;
                businessPlanProjectoject.luruPerson = luruPerson;
                if (juanneiSeqNo != null && juanneiSeqNo != "")
                {
                    businessPlanProjectoject.juanneiSeqNo = Int32.Parse(juanneiSeqNo.Trim());
                }
                else
                {
                    businessPlanProjectoject.juanneiSeqNo = 0;
                }
                businessPlanProjectoject.bianzhiTime = DateTime.Parse(bianzhiTime.Trim());
                businessPlanProjectoject.pageNo = pageNo;
                businessPlanProjectoject.archiveID = 0;
                businessPlanProjectoject.shenhePerson = shenhePerson;

                businessPlanProjectoject.isNeibu = isNeibu;
                businessPlanProjectoject.projectContent_neibu = projectContent_neibu;
                businessPlanProjectoject.archiveTitle = archiveTitle;
                businessPlanProjectoject.archiveTitle_neibu = archiveTitle_neibu;
                businessPlanProjectoject.pageNo_neibu = pageNo_neibu;
                businessPlanProjectoject.developmentUnit_neibu = developmentUnit_neibu;
                businessPlanProjectoject.projectLocation_neibu = projectLocation_neibu;
                if (photoCnt != null && photoCnt != "")
                {
                    businessPlanProjectoject.photoCnt = Int32.Parse(photoCnt.Trim());
                }
                else
                {
                    businessPlanProjectoject.photoCnt = 0;
                }
                if (photoCnt_neibu != null && photoCnt_neibu != "")
                {
                    businessPlanProjectoject.photoCnt_neibu = Int32.Parse(photoCnt_neibu);
                }
                else
                {
                    businessPlanProjectoject.photoCnt_neibu = 0;
                }

                if (wenziCnt != null && wenziCnt != "")
                {
                    businessPlanProjectoject.wenziCnt = Int32.Parse(wenziCnt.Trim());
                }
                else
                {
                    businessPlanProjectoject.wenziCnt = 0;
                }
                if (wenziCnt_neibu != null && wenziCnt_neibu != "")
                {
                    businessPlanProjectoject.wenziCnt_neibu = Int32.Parse(wenziCnt_neibu.Trim());
                }
                else
                {
                    businessPlanProjectoject.wenziCnt_neibu = 0;
                }

                businessPlanProjectoject.bei_anTime = Convert.ToDateTime(bei_anTime);
                businessPlanProjectoject.jungongTime = Convert.ToDateTime(jungongTime);
                businessPlanProjectoject.projectNumber = projectNumber;
                if (buildingArea != null && buildingArea != "")
                {
                    businessPlanProjectoject.buildingArea = float.Parse(buildingArea.Trim());
                }
                else
                {
                    businessPlanProjectoject.buildingArea = 0;
                }
                businessPlanProjectoject.projectType = projectType;
                businessPlanProjectoject.structType = structType;
                businessPlanProjectoject.kan_chaUnit = kan_chaUnit;
                businessPlanProjectoject.designUnit = designUnit;
                businessPlanProjectoject.constructionUnit = constructionUnit;
                businessPlanProjectoject.supervisorUnit = supervisorUnit;
                businessPlanProjectoject.supervisorybody = supervisorybody;
                businessPlanProjectoject.kaigongTime = Convert.ToDateTime(kaigongTime);



                try
                {
                    db_plan.businessPlanProject.Add(businessPlanProjectoject);
                    db_plan.SaveChanges();
                    return "4";//保存成功
                }
                catch (Exception ex)
                {
                    return "5";//数据错误，保存失败
                }

            }

            if (name == "xiugai")
            {
                int ID1 = 0;
                if (id != null && id != "")
                {
                    ID1 = Int32.Parse(id);
                }

                var businessPlanProject = from a in db_plan.businessPlanProject
                                          where a.ID == ID1
                                          select a;
                businessPlanProject.First().securityID = securityID;
                businessPlanProject.First().retentionPeriodID = retentionPeriodID;

                businessPlanProject.First().totalSeqNo = totalSeqNo;
                businessPlanProject.First().seqNo1 = int.Parse(seqNo2);
                businessPlanProject.First().projectNumber = projectNumber;
                businessPlanProject.First().dijijuan = Int32.Parse(dijijuan.Trim());
                businessPlanProject.First().boxNo = boxNo;
                businessPlanProject.First().projectname = projectname;
                businessPlanProject.First().developmentUnit = developmentUnit;
                businessPlanProject.First().projectContent = projectContent;
                businessPlanProject.First().projectLocation = projectLocation;
                businessPlanProject.First().yearNo = yearNo;
                businessPlanProject.First().bei_anTime = Convert.ToDateTime(bei_anTime);
                businessPlanProject.First().jungongTime = Convert.ToDateTime(jungongTime);
                //if (classifyID == null && classifyID == "")
                //{
                //    return "3";//工程种类不能为空
                //}
                //project.First().classifyID = Int32.Parse(classifyID);
                if (projectID != null && projectID != "")
                {
                    if (projectID.Trim().IndexOf('-') != -1)
                    {
                        projectID = projectID.Split('-')[0].ToString();
                    }
                    businessPlanProject.First().projectID = Int32.Parse(projectID.Trim());
                }
                else
                {
                    businessPlanProject.First().projectID = 0;
                }
                businessPlanProject.First().fileNo = fileNo;
                if (photoCnt_neibu != null && photoCnt_neibu != "")
                {
                    businessPlanProject.First().photoCnt_neibu = Int32.Parse(photoCnt_neibu);
                }
                else
                {
                    businessPlanProject.First().photoCnt_neibu = 0;
                }
                if (photoCnt != null && photoCnt != "")
                {
                    businessPlanProject.First().photoCnt = Int32.Parse(photoCnt.Trim());
                }
                else
                {
                    businessPlanProject.First().photoCnt = 0;
                }
                //为了区分不同区的同一年份同一类别的档案，保存urban_type字段
                string type = fileNo.Trim().Substring(0, 3);//大部分都是3个字符，只有市规划是两个字符“青规”，胶南是四个字符“青规胶南”
                if (fileNo.Trim().Substring(0, 4) == "青规胶南")//先判断胶南
                {
                    businessPlanProject.First().urban_type = 10;
                }
                if (type != "青规北" && type != "青规四" && type != "青规李" && type != "青规南" && type != "青规崂" && type != "青规城" && type != "青规黄" && type != "青规胶" && type != "青规平" && type != "青规莱" && type != "青规即" && type != "青规开")//只能是市规划了
                {
                    businessPlanProject.First().urban_type = 1;
                }
                switch (type)
                {
                    case "青规北":
                        businessPlanProject.First().urban_type = 2;
                        break;
                    case "青规四":
                        businessPlanProject.First().urban_type = 3;
                        break;
                    case "青规李":
                        businessPlanProject.First().urban_type = 4;
                        break;
                    case "青规南":
                        businessPlanProject.First().urban_type = 5;
                        break;
                    case "青规崂":
                        businessPlanProject.First().urban_type = 6;
                        break;
                    case "青规城":
                        businessPlanProject.First().urban_type = 7;
                        break;
                    case "青规黄":
                        businessPlanProject.First().urban_type = 8;
                        break;
                    case "青规胶":
                        businessPlanProject.First().urban_type = 9;
                        break;
                    case "青规平":
                        businessPlanProject.First().urban_type = 11;
                        break;
                    case "青规莱":
                        businessPlanProject.First().urban_type = 12;
                        break;
                    case "青规即":
                        businessPlanProject.First().urban_type = 13;
                        break;
                    case "青规开":
                        businessPlanProject.First().urban_type = 14;
                        break;

                }
                if (dateReceived != null && dateReceived != "")
                {
                    businessPlanProject.First().dateReceived = DateTime.Parse(dateReceived.Trim());

                }
                else
                {
                    businessPlanProject.First().dateReceived = null;
                }
                businessPlanProject.First().remarks = remarks;
                businessPlanProject.First().bianzhiUnit = bianzhiUnit;
                businessPlanProject.First().luruPerson = luruPerson;
                if (juanneiSeqNo != null && juanneiSeqNo != "")
                {
                    businessPlanProject.First().juanneiSeqNo = Int32.Parse(juanneiSeqNo.Trim());
                }
                else
                {
                    businessPlanProject.First().juanneiSeqNo = 0;
                }
                businessPlanProject.First().bianzhiTime = DateTime.Parse(bianzhiTime.Trim());

                businessPlanProject.First().pageNo = pageNo;
                businessPlanProject.First().archiveID = 0;
                businessPlanProject.First().isImageExist = "无";
                businessPlanProject.First().shenhePerson = shenhePerson;
                businessPlanProject.First().isImageExist = isImageExist;
                businessPlanProject.First().isNeibu = isNeibu;
                businessPlanProject.First().projectContent_neibu = projectContent_neibu;
                businessPlanProject.First().archiveTitle = archiveTitle;
                businessPlanProject.First().archiveTitle_neibu = archiveTitle_neibu;
                businessPlanProject.First().pageNo_neibu = pageNo_neibu;
                businessPlanProject.First().developmentUnit_neibu = developmentUnit_neibu;
                businessPlanProject.First().projectLocation_neibu = projectLocation_neibu;
                if (photoCnt != null && photoCnt != "")
                {
                    businessPlanProject.First().photoCnt = Int32.Parse(photoCnt.Trim());
                }
                else
                {
                    businessPlanProject.First().photoCnt = 0;
                }
                if (wenziCnt != null && wenziCnt != "")
                {
                    businessPlanProject.First().wenziCnt = Int32.Parse(wenziCnt.Trim());
                }
                else
                {
                    businessPlanProject.First().wenziCnt = 0;
                }
                if (wenziCnt_neibu != null && wenziCnt_neibu != "")
                {
                    businessPlanProject.First().wenziCnt_neibu = Int32.Parse(wenziCnt_neibu.Trim());
                }
                else
                {
                    businessPlanProject.First().wenziCnt_neibu = 0;
                }
                businessPlanProject.First().gongjijuan = Int32.Parse(gongjijuan.Trim());
                businessPlanProject.First().dijijuan = Int32.Parse(dijijuan.Trim());
                if (buildingArea != null && buildingArea != "")
                {
                    businessPlanProject.First().buildingArea = float.Parse(buildingArea.Trim());
                }
                else
                {
                    businessPlanProject.First().buildingArea = 0;
                }
                businessPlanProject.First().projectType = projectType;
                businessPlanProject.First().structType = structType;
                businessPlanProject.First().kan_chaUnit = kan_chaUnit;
                businessPlanProject.First().designUnit = designUnit;
                businessPlanProject.First().constructionUnit = constructionUnit;
                businessPlanProject.First().supervisorUnit = supervisorUnit;
                businessPlanProject.First().supervisorybody = supervisorybody;
                businessPlanProject.First().kaigongTime = Convert.ToDateTime(kaigongTime);


                try
                {
                    db_plan.Entry(businessPlanProject.First()).State = EntityState.Modified;
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

        public ActionResult Index_yewu(string currentFilter, string searchString, int? SelectedID)
        {
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


            var businessPlanProject = from ad in db_plan.businessPlanProject
                                      where ad.status == "LR"
                                      select ad;//

            ViewBag.quanxian = "display:none";
            if (user == "科员")
            {
                businessPlanProject = businessPlanProject.Where(ad => ad.luruPerson == username);
                ViewBag.quanxian = "float:left;";
            }

            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        businessPlanProject = businessPlanProject.Where(ad => ad.developmentUnit.Contains(searchString));//根据建设单位检索
                        break;
                    case 1:
                        businessPlanProject = businessPlanProject.Where(ad => ad.projectContent.Contains(searchString));//根据工程内容检索
                        break;
                    case 2:
                        businessPlanProject = businessPlanProject.Where(ad => ad.projectLocation.Contains(searchString));//根据工程地点检索
                        break;
                    case 3:
                        businessPlanProject = businessPlanProject.Where(ad => ad.yearNo.Contains(searchString));//根据年份检索
                        break;
                    case 4:
                        businessPlanProject = businessPlanProject.Where(ad => ad.fileNo.Contains(searchString));//根据文件编号检索
                        break;
                    case 5:
                        businessPlanProject = businessPlanProject.Where(ad => ad.archiveTitle.Contains(searchString));//根据案卷题名检索
                        break;
                    case 6:
                        businessPlanProject = businessPlanProject.Where(ad => ad.isNeibu.Contains(searchString));//根据公开、内部检索
                        break;
                }
            }
            var planproject1 = businessPlanProject.ToList().OrderByDescending(ad => ad.yearNo).ThenByDescending(e => e.urban_type).ThenByDescending(e => e.projectID).ThenByDescending(e => e.ID);
            ViewBag.result = JsonConvert.SerializeObject(planproject1);

            var select_year = businessPlanProject.GroupBy(p => new { p.yearNo }).Select(g => g.FirstOrDefault());
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

        public ActionResult Details_yewu(int? id)
        {
            if (id == null)
            {

                return Content("<script >alert('记录不存在！');window.history.back();</script >");
            }
            businessPlanProject businessPlanProject = db_plan.businessPlanProject.Find(id);
            //ViewBag.classifyID = businessPlanProject.classifyID;//案卷类型ID
            //ViewBag.classifyName = db_plan.PlanArchiveClassify.Find(businessPlanProject.classifyID).classifyName;
            //ViewBag.classiftsx = db_plan.PlanArchiveClassify.Find(businessPlanProject.classifyID).classifySX;//案卷类型缩写，为了拼接盒号
            // ViewBag.box = db_plan.PlanArchiveClassify.Find(businessPlanProject.classifyID).classifyName.Trim() + "-" + db_plan.PlanArchiveClassify.Find(businessPlanProject.classifyID).classifySX.Trim();
            int m = businessPlanProject.boxNo.IndexOf('年');
            int m1 = businessPlanProject.boxNo.IndexOf('第');

            int m2 = businessPlanProject.boxNo.IndexOf('北');
            int m3 = businessPlanProject.boxNo.IndexOf('四');
            int m4 = businessPlanProject.boxNo.IndexOf('李');
            int m5 = businessPlanProject.boxNo.IndexOf('南');
            int m6 = businessPlanProject.boxNo.IndexOf('崂');
            int m7 = businessPlanProject.boxNo.IndexOf('城');
            int m8 = businessPlanProject.boxNo.IndexOf('黄');
            int m9 = businessPlanProject.boxNo.IndexOf('胶');
            int m10 = 0;
            int m11 = businessPlanProject.boxNo.IndexOf('平');
            int m12 = businessPlanProject.boxNo.IndexOf('莱');
            int m13 = businessPlanProject.boxNo.IndexOf('即');
            int m14 = businessPlanProject.boxNo.IndexOf('开');
            if (m9 != -1)
            {
                string n = businessPlanProject.boxNo.Substring(m9 + 1, m9 + 2);
                if (n == "南")
                {
                    m10 = -1;
                }
                else
                {
                    m10 = 1;
                }
            }

            if (m2 == -1 && m3 == -1 && m4 == -1 && m5 == -1 && m6 == -1 && m7 == -1 && m8 == -1 && m9 == -1 && m11 == -1 && m12 == -1 && m13 == -1 && m14 == -1)
            {
                ViewBag.box = (businessPlanProject.boxNo.Substring(m + 1)).Substring(0, m1 - m - 1);
            }
            else if (m9 != -1 && m10 == 1)
            {
                ViewBag.box = (businessPlanProject.boxNo.Substring(m + 1)).Substring(0, m1 - m - 2);
            }
            else
            {
                ViewBag.box = (businessPlanProject.boxNo.Substring(m + 1)).Substring(0, m1 - m - 2);
            }
            ViewBag.classname = businessPlanProject.fileNo.Substring(0, businessPlanProject.fileNo.IndexOf('字'));


            ViewBag.year = businessPlanProject.yearNo;
            int a = businessPlanProject.boxNo.IndexOf('第') + 1;
            int b = businessPlanProject.boxNo.Trim().Length;
            int c = businessPlanProject.boxNo.IndexOf('第');
            ViewBag.boxid = businessPlanProject.boxNo.Substring(a, b - c - 2);

            ViewBag.bei_anTime = businessPlanProject.bei_anTime.Value.ToString("yyyy.MM.dd");
            ViewBag.jungongTime = businessPlanProject.jungongTime.Value.ToString("yyyy.MM.dd");
            ViewBag.kaigongTime = businessPlanProject.kaigongTime.Value.ToString("yyyy.MM.dd");
            ViewBag.bianzhiTime = businessPlanProject.bianzhiTime.Value.ToString("yyyy.MM.dd");
            ViewBag.dateReceived = businessPlanProject.dateReceived.Value.ToString("yyyy.MM.dd");

            if (businessPlanProject.securityID != null && businessPlanProject.securityID != "")
            {
                switch (businessPlanProject.securityID)
                {
                    case "1":
                        businessPlanProject.securityID = "机密";
                        break;
                    case "2":
                        businessPlanProject.securityID = "秘密";
                        break;
                    case "3":
                        businessPlanProject.securityID = "绝密";
                        break;
                    case "4":
                        businessPlanProject.securityID = "一般";
                        break;
                    case "5":
                        businessPlanProject.securityID = "内部";
                        break;
                    case "6":
                        businessPlanProject.securityID = "公开/内部";
                        break;


                }

            }
            if (businessPlanProject.retentionPeriodID != null && businessPlanProject.retentionPeriodID != "")
            {
                switch (businessPlanProject.retentionPeriodID)
                {
                    case "1":
                        businessPlanProject.retentionPeriodID = "长期";
                        break;
                    case "2":
                        businessPlanProject.retentionPeriodID = "永久";
                        break;
                    case "3":
                        businessPlanProject.retentionPeriodID = "短期";
                        break;
                }

            }

            ViewBag.fanhui = "Index_LR";
            if (businessPlanProject.status.Trim() == "GD")
            {
                ViewBag.fanhui = "Index_GD";
            }
            else if (businessPlanProject.status.Trim() == "BH")
            {
                ViewBag.fanhui = "Index_BH";
            }
            else if (businessPlanProject.status.Trim() == "RK")
            {
                ViewBag.fanhui = "Index_RK";
            }
            else
            {
                ViewBag.fanhui = "Index_LR";
            }
            if (businessPlanProject.totalSeqNo != "" && businessPlanProject.totalSeqNo != null)
            {


                if (businessPlanProject.isNeibu.Trim() == "公开")
                {

                    ViewData["fengpiG"] = "inline-block";
                    ViewData["beikaobiaoG"] = "inline-block";
                    ViewData["fengpiN"] = "display: none";
                    ViewData["beikaobiaoN"] = "display: none";
                }
                if (businessPlanProject.isNeibu.Trim() == "内部")
                {
                    ViewData["fengpiG"] = "display: none";
                    ViewData["beikaobiaoG"] = "display: none";
                    ViewData["fengpiN"] = "inline-block";
                    ViewData["beikaobiaoN"] = "inline-block";
                }
                if (businessPlanProject.isNeibu.Trim() == "公开/内部")
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
            return View(businessPlanProject);
        }

        public ActionResult Edit_yewu(int? id)
        {

            businessPlanProject businessPlanProject = db_plan.businessPlanProject.Find(id);
            ViewBag.ID = id;
            //ViewBag.classifyID = planProject.classifyID;//案卷类型ID
            //ViewBag.classifyName = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName.Trim();//案卷类型mame
            //ViewBag.classiftsx = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifySX;//案卷类型缩写，为了拼接盒号
            //ViewBag.box = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName.Trim() + "-" + db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifySX.Trim();
            //if (planProject.classifyID == 28 || planProject.classifyID == 29)
            //{
            //    ViewBag.classname = planProject.fileNo.Substring(0, planProject.fileNo.IndexOf('】') + 1);
            //}
            //else
            //{
            //    ViewBag.classname = planProject.fileNo.Substring(0, planProject.fileNo.IndexOf('字'));/* db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName.Trim()*/;//文号的默认值
            //}
            int m = businessPlanProject.boxNo.IndexOf('年');
            int m1 = businessPlanProject.boxNo.LastIndexOf('第');

            int m2 = businessPlanProject.boxNo.IndexOf('北');
            int m3 = businessPlanProject.boxNo.IndexOf('四');
            int m4 = businessPlanProject.boxNo.IndexOf('李');
            int m5 = businessPlanProject.boxNo.IndexOf('南');
            int m6 = businessPlanProject.boxNo.IndexOf('崂');
            int m7 = businessPlanProject.boxNo.IndexOf('城');
            int m8 = businessPlanProject.boxNo.IndexOf('黄');
            int m9 = businessPlanProject.boxNo.IndexOf('胶');
            int m10 = 0;
            int m11 = businessPlanProject.boxNo.IndexOf('平');
            int m12 = businessPlanProject.boxNo.IndexOf('莱');
            int m13 = businessPlanProject.boxNo.IndexOf('即');
            int m14 = businessPlanProject.boxNo.IndexOf('开');
            if (m9 != -1)
            {
                string n = businessPlanProject.boxNo.Substring(m9 + 1, m9 + 2);
                if (n == "南")
                {
                    m10 = -1;
                }
                else
                {
                    m10 = 1;
                }
            }
            if (m2 == -1 && m3 == -1 && m4 == -1 && m5 == -1 && m6 == -1 && m7 == -1 && m8 == -1 && m9 == -1 && m11 == -1 && m12 == -1 && m13 == -1 && m14 == -1)
            {
                ViewBag.box = (businessPlanProject.boxNo.Substring(m + 1)).Substring(0, m1 - m - 1);
            }
            else if (m9 != -1 && m10 == 1)
            {
                ViewBag.box = (businessPlanProject.boxNo.Substring(m + 3)).Substring(0, m1 - m - 3);
            }
            else
            {
                ViewBag.box = (businessPlanProject.boxNo.Substring(m + 2)).Substring(0, m1 - m - 2);
            }
            ViewBag.classname = businessPlanProject.fileNo.Substring(0, businessPlanProject.fileNo.IndexOf('字'));
            ViewBag.year = businessPlanProject.yearNo;
            int a = businessPlanProject.boxNo.IndexOf('第') + 1;
            int b = businessPlanProject.boxNo.Trim().Length;
            int c = businessPlanProject.boxNo.IndexOf('第');
            ViewBag.boxid = businessPlanProject.boxNo.Substring(a, b - c - 2);
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
            ViewBag.quyu = new SelectList(list2, "Value", "Text", businessPlanProject.urban_type);
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "公开"},
                new SelectListItem { Text = "内部", Value = "内部" },
                new SelectListItem { Text = "公开/内部", Value = "公开/内部" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", businessPlanProject.isNeibu.Trim());
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "青岛市规划局", Value ="青岛市规划局"},
                new SelectListItem { Text = "青岛市规划局市北分局", Value ="青岛市规划局市北分局"},
                new SelectListItem { Text = "青岛市规划局原四方分局", Value ="青岛市规划局原四方分局"},
                new SelectListItem { Text = "青岛市规划局李沧分局", Value ="青岛市规划局李沧分局"},
                new SelectListItem { Text = "青岛市规划局市南分局", Value ="青岛市规划局市南分局"},
            };
            ViewBag.bianzhiUnit = new SelectList(list, "Value", "Text", businessPlanProject.bianzhiUnit.Trim());
            //档案密级
            ViewBag.securityID = new SelectList(db_urban.SecurityClassification, "securityID", "securityName", businessPlanProject.securityID.Trim());
            //保管年限(注意这里与)
            ViewBag.retentionPeriodID = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", businessPlanProject.retentionPeriodID.Trim());

            ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", businessPlanProject.shenhePerson);//审核人
            ViewBag.luruPerson = businessPlanProject.luruPerson;//录入人传到前台，前台设置成不可修改
                                                                //ViewBag.dateReceived = planProject.dateReceived == null ? "" : planProject.dateReceived.Value.ToString("yyyy.MM.dd");

            ViewBag.bei_anTime = businessPlanProject.bei_anTime.Value.ToString("yyyy.MM.dd");
            ViewBag.jungongTime = businessPlanProject.jungongTime.Value.ToString("yyyy.MM.dd");
            ViewBag.kaigongTime = businessPlanProject.kaigongTime.Value.ToString("yyyy.MM.dd");
            ViewBag.bianzhiTime = businessPlanProject.bianzhiTime.Value.ToString("yyyy.MM.dd");
            ViewBag.dateReceived = businessPlanProject.dateReceived.Value.ToString("yyyy.MM.dd");

            if (businessPlanProject.totalSeqNo != "" && businessPlanProject.totalSeqNo != null)
            {


                if (businessPlanProject.isNeibu.Trim() == "公开")
                {

                    ViewData["fengpiG"] = "inline-block";
                    ViewData["beikaobiaoG"] = "inline-block";
                    ViewData["fengpiN"] = "display: none";
                    ViewData["beikaobiaoN"] = "display: none";
                }
                if (businessPlanProject.isNeibu.Trim() == "内部")
                {
                    ViewData["fengpiG"] = "display: none";
                    ViewData["beikaobiaoG"] = "display: none";
                    ViewData["fengpiN"] = "inline-block";
                    ViewData["beikaobiaoN"] = "inline-block";
                }
                if (businessPlanProject.isNeibu.Trim() == "公开/内部")
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
            return View(businessPlanProject);
        }

        // POST: PlanProjects/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_yewu([Bind(Include = "securityID,retentionPeriodID,gongjijuan,dijijuan,projectname,ID,isNeibu,seqNo,developmentUnit,projectContent,projectLocation,yearNo,pageCnt,classifyID,projectID,fileNo,dateReceived,boxNo,status,remarks,archiveBoxCnt,mapScale,storeLocation,bianzhiUnit,fileNoTou,luruPerson,seqNo1,juanneiSeqNo,bianzhiTime,pageNo,totalSeqNo,archiveID,photoCnt,shenhePerson,isImageExist")] businessPlanProject businessPlanProject, String action)
        {
            //ViewBag.classifyID = planProject.classifyID;//案卷类型ID
            //ViewBag.classifyName = db_plan.PlanArchiveClassify.Find(planProject.classifyID).classifyName;
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
            ViewBag.quyu = new SelectList(list2, "Value", "Text", businessPlanProject.urban_type);
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "公开"},
                new SelectListItem { Text = "内部", Value = "内部" },
                new SelectListItem { Text = "公开/内部", Value = "公开/内部" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", businessPlanProject.isNeibu);
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "青岛市规划局", Value ="1"},
                new SelectListItem { Text = "青岛市规划局市北分局", Value ="2"},
                new SelectListItem { Text = "青岛市规划局原四方分局", Value ="3"},
                new SelectListItem { Text = "青岛市规划局李沧分局", Value ="4"},
                new SelectListItem { Text = "青岛市规划局市南分局", Value ="5"},
            };
            ViewBag.bianzhiUnit = new SelectList(list, "Value", "Text", businessPlanProject.bianzhiUnit.Trim());
            //档案密级
            ViewBag.securityID = new SelectList(db_urban.SecurityClassification, "securityID", "securityName", businessPlanProject.securityID);
            //保管年限(注意这里与)
            ViewBag.retentionPeriodID = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", businessPlanProject.retentionPeriodID);

            ViewBag.shenhePerson = new SelectList(db_user.AspNetUsers, "UserName", "UserName", businessPlanProject.shenhePerson);//审核人
            ViewBag.luruPerson = businessPlanProject.luruPerson;//录入人传到前台，前台设置成不可修改

            if (action == "提交保存")
            {
                if (ModelState.IsValid)
                {

                    db_plan.Entry(businessPlanProject).State = EntityState.Modified;
                    //return RedirectToAction("Index", new { archiveID = planProject.archiveID, classifyID = planProject.classifyID });
                    //return Content("<script >alert('修改成功！');window.location.href='/PlanProjects/Index?archiveID=" + planProject.archiveID + "&id="+ planProject.ID + "&id1=2" + "&classifyID=" + planProject.classifyID + "';</script >");
                    try
                    {
                        db_plan.SaveChanges();
                        Response.Write("<script>alert('保存成功!');</script>");
                        return View(businessPlanProject);
                    }
                    catch (Exception ex)
                    {
                        return Content("<script >alert('录入信息有误，请检查！');window.history.back();</script >");
                        //return Content("<script >alert('档号不能为空！');window.history.back();</script >");
                    }
                }
            }
            //if (action == "返回")
            //{
            //    string status = businessPlanProject.status.Trim();
            //    if (status == "GD")
            //    {
            //        return RedirectToAction("Index_GD", new { classifyID = planProject.classifyID });
            //    }
            //    else if (status == "BH")
            //    {
            //        return RedirectToAction("Index_BH", new { classifyID = planProject.classifyID });
            //    }
            //    else if (status == "RK")
            //    {
            //        return RedirectToAction("Index_RK", new { classifyID = planProject.classifyID });
            //    }
            //    else
            //    {
            //        return RedirectToAction("Index_LR", new { classifyID = planProject.classifyID });
            //    }

            //}
            //int ID = businessPlanProject.ID;
            //if (action == "打印备考表(公开)")
            //{
            //    return RedirectToAction("beikaobiao", new { id = ID, id1 = 0 });
            //}

            //if (action == "打印案卷封皮(公开)")
            //{
            //    return RedirectToAction("GuiHuaGongChengFengPi", new { id = ID, id1 = 0 });
            //}
            //if (action == "打印备考表(内部)")
            //{
            //    return RedirectToAction("beikaobiao", new { id = ID, id1 = 1 });
            //}

            //if (action == "打印案卷封皮(内部)")
            //{
            //    return RedirectToAction("GuiHuaGongChengFengPi", new { id = ID, id1 = 1 });
            //}
            //if (action == "打印工程目录")
            //{
            //    return RedirectToAction("GuiHuaGongChengMuLu", new { id = businessPlanProject.seqNo1 });
            //}
            return View(businessPlanProject);
        }

        public ActionResult Delete_yewu(int id)
        {
            businessPlanProject businessPlanProject = db_plan.businessPlanProject.Find(id);
            string status = businessPlanProject.status.Trim();

            try
            {
                var box1 = from a in db_plan.PlanArchiveyewuBox
                           where a.boxNo == businessPlanProject.boxNo                       
                           select a;

                var box = from a in db_plan.businessPlanProject
                          where a.boxNo == businessPlanProject.boxNo
                          where a.fileNo != businessPlanProject.fileNo
                          select a;

                if (box.Count() != 0)
                {
                    PlanArchiveyewuBox PlanArchiveyewuBox = box1.First();
                    if (box1.Count() != 0)
                    {
                        PlanArchiveyewuBox.archiveBoxCnt--;
                        PlanArchiveyewuBox.textMatirial -= (businessPlanProject.wenziCnt + businessPlanProject.wenziCnt_neibu);
                        PlanArchiveyewuBox.drawing -= (businessPlanProject.photoCnt + businessPlanProject.photoCnt_neibu);
                    }
                    db_plan.PlanArchiveyewuBox.Remove(PlanArchiveyewuBox);
                }

                db_plan.businessPlanProject.Remove(businessPlanProject);
                db_plan.SaveChanges();
                if (status == "GD")
                {
                    return RedirectToAction("Index_yewuGD");
                }
                else if (status == "BH")
                {
                    return RedirectToAction("Index_yewuBH");
                }
                else if (status == "RK")
                {
                    return RedirectToAction("Index_yewuRK");
                }
                else
                {
                    return RedirectToAction("Index_yewu");
                }
            }

            catch (Exception ex)
            {
                return Content("<script >alert('信息有误！');window.history.back();</script >");
                //return Content("<script >alert('档号不能为空！');window.history.back();</script >");
            }
            return Content("<script >alert('删除成功！');window.history.back();</script >");
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //传入
        public ActionResult Index_yewu1(string yearNo, string quyu)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID).RoleName;
            var username = db_user.AspNetUsers.Find(UserID).UserName;

            int quyu1 = Int32.Parse(quyu);
            var businessPlanProject = from ad in db_plan.businessPlanProject
                                      where ad.status == "LR"
                                      where ad.luruPerson == username
                                      where ad.yearNo == yearNo
                                      where ad.urban_type == quyu1
                                      select ad;//
            foreach (var item in businessPlanProject)//
            {
                item.status = "GD";//完成归档
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
            return RedirectToAction("Index_yewu");
        }

        public ActionResult Index_yewuGD(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
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

            var businessPlanProject = from ad in db_plan.businessPlanProject
                                      where (ad.status == "GD" || ad.status == "SH")

                                      select ad;
            ViewBag.quanxian = "display:none";
            if (user == "科员")
            {
                businessPlanProject = businessPlanProject.Where(ad => ad.luruPerson == username);
                ViewBag.quanxian = "float:left;";
            }
            //planproject = planproject.OrderBy(ad => ad.yearNo).ThenBy(ad => ad.projectID);
            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        businessPlanProject = businessPlanProject.Where(ad => ad.developmentUnit.Contains(searchString));//根据建设单位检索
                        break;
                    case 1:
                        businessPlanProject = businessPlanProject.Where(ad => ad.projectContent.Contains(searchString));//根据工程内容检索
                        break;
                    case 2:
                        businessPlanProject = businessPlanProject.Where(ad => ad.projectLocation.Contains(searchString));//根据工程地点检索
                        break;
                    case 3:
                        businessPlanProject = businessPlanProject.Where(ad => ad.yearNo.Contains(searchString));//根据年份检索
                        break;
                    case 4:
                        businessPlanProject = businessPlanProject.Where(ad => ad.fileNo.Contains(searchString));//根据文件编号检索
                        break;
                    case 5:
                        businessPlanProject = businessPlanProject.Where(ad => ad.archiveTitle.Contains(searchString));//根据案卷题名检索
                        break;
                    case 6:
                        businessPlanProject = businessPlanProject.Where(ad => ad.isNeibu.Contains(searchString));//根据公开、内部检索
                        break;
                }
            }


            var businessPlanProject1 = businessPlanProject.ToList().OrderBy(s => s.yearNo).ThenBy(ad => ad.urban_type).ThenBy(ad => ad.projectID).ThenBy(ad => ad.ID).ThenBy(ad => ad.fileNo).ThenBy(ad => ad.dijijuan);
            //var planproject1 = planproject.ToList().OrderBy(s => s.yearNo).ThenBy(ad => ad.urban_type).ThenBy(ad => ad.projectID).ThenBy(ad => ad.fileNo).ThenBy(ad => ad.dijijuan);
            ViewBag.result = JsonConvert.SerializeObject(businessPlanProject1);




            var select_year = businessPlanProject.GroupBy(p => new { p.yearNo }).Select(g => g.FirstOrDefault());
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
        public ActionResult Index_yewuGD(string yearNo, string action, int quyu)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID).RoleName;
            var username = db_user.AspNetUsers.Find(UserID).UserName;
            if (action == "编号")
            {

                if (user == "科员")
                {
                    return Content("<script >alert('当前用户没有权限！');window.history.back();</script >");
                }
                if (yearNo == null)
                {
                    return Content("<script >alert('年份为空，请选择年份！');window.history.back();</script >");

                }
                var businessPlanProject = from ad in db_plan.businessPlanProject
                                          where ad.status == "GD"
                                          where ad.yearNo == yearNo
                                          where ad.urban_type == quyu
                                          orderby ad.urban_type, ad.projectID, ad.fileNo, ad.dijijuan//同一类型、同一年份档案按文号排序->编号  
                                          select ad;
                if (businessPlanProject.Count() == 0)
                {
                    return Content("<script >alert('案卷已经编号，请勿重复编号！');window.history.back();</script >");
                }
                var box = from a in businessPlanProject
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
                              //classid = c.FirstOrDefault().classifyID,
                              classname = c.FirstOrDefault().yearNo,
                              vol = c.FirstOrDefault().dijijuan,
                              boxNo1 = c.FirstOrDefault().boxNo,
                              arear = c.FirstOrDefault().urban_type

                          };
                var box1 = from c in box
                           orderby c.arear, c.No, c.file, c.vol
                           select c;//将盒子按其盒内最大的文号升序排列,然后再按照盒名升序排列
                if (box1.Count() != 0)
                {
                    int i = 1;


                    foreach (var item in box1)//为盒子编号
                    {
                        PlanArchiveyewuBox boxi = new PlanArchiveyewuBox();
                        var PlanBoxID = db_plan.PlanArchiveyewuBox.Max(a => a.ID) + i;//盒子ID
                        var PlanBoxSeqno = db_plan.PlanArchiveyewuBox.Max(a => a.seqNo) + i;//盒子号，对应planproject表中的seqno1
                        boxi.ID = PlanBoxID;
                        boxi.seqNo = PlanBoxSeqno;
                        boxi.boxNo = item.boxNo1;
                        boxi.archiveBoxCnt = item.cnt;
                        boxi.textMatirial = item.wenzi + item.wenzi_neibu;
                        boxi.drawing = item.drawing + item.drawing_neibu;
                        boxi.yearNo = item.classname;
                        boxi.archiveCode = "437402";
                        boxi.bianzhiUnit = "青岛市规划局";
                        boxi.storeLocation = "青岛市城建档案馆";
                        boxi.isNeibu = "否";
                        db_plan.PlanArchiveyewuBox.Add(boxi);

                        i++;
                    }
                    db_plan.SaveChanges();
                }




                string max_TotalSeqNo = db_plan.businessPlanProject.Max(s => s.totalSeqNo);//获得当前最大totalSeqNo,取前六位

                if (max_TotalSeqNo == "" || max_TotalSeqNo == null)//该数据为第一条
                {
                    max_TotalSeqNo = "00000000";
                }
                max_TotalSeqNo = max_TotalSeqNo.Trim().Substring(0, 8);
                int temp = int.Parse(max_TotalSeqNo) + 1;//总顺序号的前六位 +1

                foreach (var item in businessPlanProject)//编号，seqno作为总顺序号
                {
                    string cur_max = temp.ToString().PadLeft(8, '0');//左边填充0，补齐8位  
                    item.projectno = cur_max;//保存工程序号前8位
                    if (item.gongjijuan > 1)//如果一个工程有多卷，则需要
                    {
                        item.totalSeqNo = cur_max + "-" + item.dijijuan.ToString().PadLeft(3, '0');//编号示例：000005-001
                        var PlanArchiveyewuBox = from a in db_plan.PlanArchiveyewuBox
                                      where a.boxNo == item.boxNo
                                      select a;//盒子已经编完号了，为每个工程装盒
                        if (PlanArchiveyewuBox.Count() != 0)
                        {
                            item.seqNo = PlanArchiveyewuBox.First().ID;
                            item.seqNo1 = PlanArchiveyewuBox.First().seqNo;
                        }
                        if (item.gongjijuan == item.dijijuan)
                        {
                            temp += 1;
                        }
                    }
                    else
                    {//编号示例：000005
                        var PlanArchiveyewuBox = from a in db_plan.PlanArchiveyewuBox
                                             where a.boxNo == item.boxNo
                                             select a;//盒子已经编完号了，为每个工程装盒
                        if (PlanArchiveyewuBox.Count() != 0)
                        {
                            item.seqNo = PlanArchiveyewuBox.First().ID;
                            item.seqNo1 = PlanArchiveyewuBox.First().seqNo;
                        }
                        item.totalSeqNo = cur_max;//totalSeqNo赋值为当前最大号+1
                        temp += 1;
                    }
                    item.status = "SH";//完成编号
                    db_plan.Entry(item).State = EntityState.Modified;
                }
                try
                {
                    db_plan.SaveChanges();
                    return RedirectToAction("Index_yewuGD");
                }
                catch (Exception ex)
                {
                    return Content("<script >alert('保存失败！');window.history.back();</script >");
                }
            }

            if (action == "取消编号")
            {
                if (user == "科员")
                {
                    return Content("<script >alert('当前用户没有权限！');window.history.back();</script >");
                }

                var businessPlanProject1 = from ad in db_plan.businessPlanProject
                                       //where ad.classifyID == classifyID
                                   where ad.status == "SH"
                                   where ad.yearNo == yearNo
                                   where ad.urban_type == quyu
                                   orderby ad.projectID//同一类型、同一年份档案按文号排序->编号                      
                                   select ad;//检索出已经编号的档案

                var box = from b in db_plan.businessPlanProject
                          //where b.classifyID == classifyID
                          where b.status == "SH"
                          where b.yearNo == yearNo
                          where b.urban_type == quyu
                          group b by b.boxNo
                         into c
                          select new
                          {
                              No = c.FirstOrDefault().boxNo
                          };
                foreach (var item2 in businessPlanProject1)//给每一个案卷的盒子号，盒子ID，工程总顺序号赋空
                {
                    item2.seqNo = 0;
                    item2.seqNo1 = 0;
                    item2.totalSeqNo = "";
                    item2.status = "GD";
                    db_plan.Entry(item2).State = EntityState.Modified;

                }
                foreach (var item3 in box)//删除盒子
                {
                    var c = from e in db_plan.PlanArchiveyewuBox
                            where e.boxNo == item3.No
                            select e;
                    if (c.Count() != 0)
                    {
                        db_plan.PlanArchiveyewuBox.Remove(c.First());
                    }
                }
                db_plan.SaveChanges();
            }

            if (action == "审核通过")
            {
                if (user == "科员")
                {
                    return Content("<script >alert('当前用户没有权限！');window.history.back();</script >");
                }



                var businessPlanProject1 = from ad in db_plan.businessPlanProject
                                           where ad.status == "SH"
                                           where ad.yearNo == yearNo
                                           where ad.urban_type == quyu
                                           orderby ad.urban_type, ad.projectID, ad.fileNo, ad.dijijuan//同一类型、同一年份档案按文号排序->编号                      
                                           select ad;//检索
                foreach (var item4 in businessPlanProject1)//给每一个案卷的盒子号，盒子ID，工程总顺序号赋空
                {
                    item4.status = "BH";
                    db_plan.Entry(item4).State = EntityState.Modified;

                }
                db_plan.SaveChanges();
                return RedirectToAction("Index_yewuGD");
            }
            return RedirectToAction("Index_yewuGD");
        }

        public ActionResult Index_yewuBH(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
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


            var businessPlanProject = from ad in db_plan.businessPlanProject
                                      where ad.status == "BH"
                                      select ad;//

            ViewBag.quanxian = "display:none";

            if (user == "科员")
            {
                businessPlanProject = businessPlanProject.Where(ad => ad.luruPerson == username);
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
                        businessPlanProject = businessPlanProject.Where(ad => ad.developmentUnit.Contains(searchString));//根据建设单位检索
                        break;
                    case 1:
                        businessPlanProject = businessPlanProject.Where(ad => ad.projectContent.Contains(searchString));//根据工程内容检索
                        break;
                    case 2:
                        businessPlanProject = businessPlanProject.Where(ad => ad.projectLocation.Contains(searchString));//根据工程地点检索
                        break;
                    case 3:
                        businessPlanProject = businessPlanProject.Where(ad => ad.yearNo.Contains(searchString));//根据年份检索
                        break;
                    case 4:
                        businessPlanProject = businessPlanProject.Where(ad => ad.fileNo.Contains(searchString));//根据文件编号检索
                        break;
                    case 5:
                        businessPlanProject = businessPlanProject.Where(ad => ad.archiveTitle.Contains(searchString));//根据案卷题名检索
                        break;
                    case 6:
                        businessPlanProject = businessPlanProject.Where(ad => ad.isNeibu.Contains(searchString));//根据公开、内部检索
                        break;
                }
            }
            //planproject = planproject.OrderBy(s => s.yearNo).ThenBy(ad => ad.fileNo);
            businessPlanProject = businessPlanProject.OrderBy(s => s.totalSeqNo).ThenBy(a => a.seqNo1);
            ViewBag.result = JsonConvert.SerializeObject(businessPlanProject.ToList());

            var select_year = businessPlanProject.GroupBy(p => new { p.yearNo }).Select(g => g.FirstOrDefault());
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
        public ActionResult Index_yewuBH1(string yearNo, int quyu)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID).RoleName;
            var username = db_user.AspNetUsers.Find(UserID).UserName;
            var businessPlanProject = from ad in db_plan.businessPlanProject
                                      where ad.status == "BH"
                                      where ad.yearNo == yearNo
                                      where ad.urban_type == quyu
                                      select ad;//
            if (user == "科员")
            {
                businessPlanProject = businessPlanProject.Where(a => a.luruPerson == username);
            }
            foreach (var item in businessPlanProject)//
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
            return RedirectToAction("Index_yewuBH");
        }

        public ActionResult Index_yewuRK(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
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


            var businessPlanProject = from ad in db_plan.businessPlanProject
                                      where ad.status == "RK"
                                      select ad;//

            ViewBag.quanxian = "display:none";

            if (user == "科员")
            {
                businessPlanProject = businessPlanProject.Where(ad => ad.luruPerson == username);
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
                        businessPlanProject = businessPlanProject.Where(ad => ad.developmentUnit.Contains(searchString));//根据建设单位检索
                        break;
                    case 1:
                        businessPlanProject = businessPlanProject.Where(ad => ad.projectContent.Contains(searchString));//根据工程内容检索
                        break;
                    case 2:
                        businessPlanProject = businessPlanProject.Where(ad => ad.projectLocation.Contains(searchString));//根据工程地点检索
                        break;
                    case 3:
                        businessPlanProject = businessPlanProject.Where(ad => ad.yearNo.Contains(searchString));//根据年份检索
                        break;
                    case 4:
                        businessPlanProject = businessPlanProject.Where(ad => ad.fileNo.Contains(searchString));//根据文件编号检索
                        break;
                    case 5:
                        businessPlanProject = businessPlanProject.Where(ad => ad.archiveTitle.Contains(searchString));//根据案卷题名检索
                        break;
                    case 6:
                        businessPlanProject = businessPlanProject.Where(ad => ad.isNeibu.Contains(searchString));//根据公开、内部检索
                        break;
                }
            }
            //planproject = planproject.OrderBy(s => s.yearNo).ThenBy(ad => ad.fileNo);
            businessPlanProject = businessPlanProject.OrderBy(s => s.totalSeqNo);
            ViewBag.result = JsonConvert.SerializeObject(businessPlanProject.ToList());
            return View();
        }

        public ActionResult Index__yewuALL(string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
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


            var businessPlanProject = from ad in db_plan.businessPlanProject
                                      select ad;

            ViewBag.quanxian = "display:none";

            if (user == "科员")
            {
                businessPlanProject = businessPlanProject.Where(ad => ad.luruPerson == username);
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
                        businessPlanProject = businessPlanProject.Where(ad => ad.developmentUnit.Contains(searchString));//根据建设单位检索
                        break;
                    case 1:
                        businessPlanProject = businessPlanProject.Where(ad => ad.projectContent.Contains(searchString));//根据工程内容检索
                        break;
                    case 2:
                        businessPlanProject = businessPlanProject.Where(ad => ad.projectLocation.Contains(searchString));//根据工程地点检索
                        break;
                    case 3:
                        businessPlanProject = businessPlanProject.Where(ad => ad.yearNo.Contains(searchString));//根据年份检索
                        break;
                    case 4:
                        businessPlanProject = businessPlanProject.Where(ad => ad.fileNo.Contains(searchString));//根据文件编号检索
                        break;
                    case 5:
                        businessPlanProject = businessPlanProject.Where(ad => ad.archiveTitle.Contains(searchString));//根据案卷题名检索
                        break;
                    case 6:
                        businessPlanProject = businessPlanProject.Where(ad => ad.isNeibu.Contains(searchString));//根据公开、内部检索
                        break;
                }
            }

            businessPlanProject = businessPlanProject.Take(1000).OrderBy(s => s.totalSeqNo).ThenBy(s => s.yearNo).ThenBy(s => s.projectID);
            ViewBag.result = JsonConvert.SerializeObject(businessPlanProject.ToList());
            return View();
        }

        public ActionResult YeWuGongChengMuLu(long id, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db_plan.businessPlanProject
                      where ad.seqNo1 == id
                      orderby ad.projectID, ad.fileNo
                      select ad;
            List<businessPlanProject> list = ds1.ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].fileNo != null)
                    list[i].fileNo = list[i].fileNo.Trim();
            }
            var ds = list;
            localReport.ReportPath = Server.MapPath("~/Report/YeWu/JuanNeiMuLu.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("YeWuGcJuanNeiMuLu", ds);
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

        public ActionResult YeWuGongChengFengPi(long id, int id1)
        {

            db_plan.Configuration.ProxyCreationEnabled = false;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.ConnectionString = "Provider=SQLOLEDB;Data Source=" + System.Web.Configuration.WebConfigurationManager.AppSettings["IP"] + ";Initial Catalog=UrbanCon;User ID=web;Password=web";
            OleDbConnection conn = new OleDbConnection(builder.ToString());
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            string sql = "SELECT businessPlanProject.* FROM         businessPlanProject where ID='" + id + "' ";
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            DataSet ds = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            CrystalReportViewer repview = new CrystalReportViewer();
            adapter.Fill(ds);                ///////报表连接数据库,根据建水晶报表时的连接字符串设置
            ds.DataSetName = "YWGcMlDataSet";
            DataTable dt1 = ds.Tables[0];
            string archivetitle = dt1.Rows[0]["archiveTitle"].ToString().ToString().Replace("\r\n", " ").Replace("\n", "").Trim();
            dt1.Rows[0]["archiveTitle"] = dt1.Rows[0]["archiveTitle"].ToString().ToString().Replace("\r\n", " ").Replace("\n", "").Trim();

            string mijiid = db_plan.businessPlanProject.Where(a => a.ID == id).First().securityID;
            string miji = db_urban.SecurityClassification.Where(a => a.securityID == mijiid).First().securityName; ;
            string qixianid = db_plan.businessPlanProject.Where(a => a.ID == id).First().retentionPeriodID;
            string baoguanqixian = db_urban.RetentionPeriod.Where(a => a.retentionPeriodNo == qixianid).First().retentionPeriodName;
            string page1 = db_plan.businessPlanProject.Where(a => a.ID == id).First().pageNo;
            string seqNo1 = db_plan.businessPlanProject.Where(a => a.ID == id).First().seqNo1.ToString();
            string boxno = db_plan.businessPlanProject.Where(a => a.ID == id).First().boxNo;
            string bianzhiUnit = db_plan.businessPlanProject.Where(a => a.ID == id).First().bianzhiUnit;
            string isNei = "外部";
            if (id1 == 1)
            {
                dt1.Rows[0]["archiveTitle"] = dt1.Rows[0]["archiveTitle_neibu"].ToString().ToString().Replace("\r\n", " ").Replace("\n", "").Trim();
                page1 = db_plan.PlanProject.Where(a => a.ID == id).First().pageNo_neibu;
                isNei = "内部";
            }

            conn.Close();
            rptH.Load(Server.MapPath("~/") + "//Report//YeWu//FengPi.rpt");
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

        public ActionResult YeWuAnJuanGongChengFengPi(long id, string type = "PDF")
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
        public ActionResult beikaobiao(long id, int id1, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            if (id1 == 0)
            {
                var ds1 = from ad in db_plan.businessPlanProject
                          where ad.seqNo1 == id
                          where ad.isNeibu == "公开" || ad.isNeibu == "公开/内部"
                          select ad;
                localReport.ReportPath = Server.MapPath("~/Report/YeWu/beikaobiaoG.rdlc");
                ReportDataSource reportDataSource1 = new ReportDataSource("YeWuG", ds1);
                localReport.DataSources.Add(reportDataSource1);
            }
            if (id1 == 1)
            {
                var ds1 = from ad in db_plan.businessPlanProject
                          where ad.seqNo1 == id
                          where ad.isNeibu == "内部" || ad.isNeibu == "公开/内部"
                          select ad;
                localReport.ReportPath = Server.MapPath("~/Report/YeWu/beikaobiaoN.rdlc");
                ReportDataSource reportDataSource1 = new ReportDataSource("YeWuN", ds1);
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
    }
}