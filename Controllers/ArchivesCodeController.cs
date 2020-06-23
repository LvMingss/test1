﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using PagedList;
using urban_archive.Models;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using System.IO;

namespace urban_archive.Controllers
{
    public class ArchivesCodeController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        private UrbanUsersEntities ab = new UrbanUsersEntities();
        private gxArchivesContainer abc = new gxArchivesContainer();
        public ActionResult CodeManagement()
        {
            ViewData["pagename"] = "ArchivesCode/CodeManagement";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CodeManagement(string action, SubDictionary subdictionary, MinorDictionary minordictionary)
        {
            
            string name = Request.Form["choose"];
            string nameid = Request.Form["text1"];
            string creatname = Request.Form["text2"];
            int a = nameid.IndexOf(".");
            if (action == "添加子类")
            {
                if (nameid.Length == 1)//选择第一级
                {
                    if (ModelState.IsValid)
                    {
                        subdictionary.subDictionaryName = creatname;
                        subdictionary.mainCategoryID = nameid;
                        var listid = from ad in db.SubDictionary
                                     where ad.mainCategoryID == nameid
                                     select ad;
                        string max_ID = listid.Max(d => d.subDictionaryID);
                        int id = int.Parse(max_ID) + 1;
                        subdictionary.subDictionaryID = id.ToString().Trim();
                        db.SubDictionary.Add(subdictionary);
                        db.SaveChanges();
                    }
                }
                else if ((nameid.Length > 1) && (nameid.IndexOf(".") == -1))//选择第二级
                {
                    if (ModelState.IsValid)
                    {
                        minordictionary.minorDictionaryName = creatname;
                        minordictionary.mainCategoryID = nameid.Substring(0, 1);
                        minordictionary.subDictionaryID = name.Substring(0, 1);
                        var listid = from ad in db.MinorDictionary
                                     where ad.mainCategoryID == nameid.Substring(0, 1)
                                     where ad.subDictionaryID == name.Substring(0, 1)
                                     select ad;
                        string max_ID = listid.Max(d => d.minorDictionaryID);
                        int id = int.Parse(max_ID) + 1;
                        minordictionary.minorDictionaryID = id.ToString().Trim();
                        db.MinorDictionary.Add(minordictionary);
                        db.SaveChanges();
                    }
                }
                else
                    Response.Write("<script>alert('无法添加子类，请选择上一级分类!');</script>");
            }
            if (action == "删除")
            {
                if ((nameid.Length > 1) && (nameid.IndexOf(".") == -1))//选择第二级
                {
                    if (ModelState.IsValid)
                    {
                        string n = nameid.Substring(0, 1);
                        string m = name.Split(':').First();
                        var list = from ad in db.SubDictionary
                                   where ad.mainCategoryID == n
                                   where ad.subDictionaryID ==m
                                   select ad;
                        SubDictionary sub = list.FirstOrDefault();
                        db.SubDictionary.Remove(sub);
                        db.SaveChanges();
                    }
                }
                if ((nameid.Length > 1) && (nameid.IndexOf(".") != -1))//选择第三级
                {
                    if (ModelState.IsValid)
                    {
                        var list = from ad in db.MinorDictionary
                                   where ad.mainCategoryID == nameid.Substring(0, 1)
                                   where ad.minorDictionaryID == name.Substring(0, 1)
                                   where ad.subDictionaryID == nameid.Substring(3, 1)
                                   select ad;
                        MinorDictionary minor = list.FirstOrDefault();
                        db.MinorDictionary.Remove(minor);
                        db.SaveChanges();
                    }
                }
            }
            return View();
        }
        public ActionResult ZTree()
        {
            return View();
        }
        public JsonResult fenleihao()
        {
            var list1 = from bb in db.MainCategory.ToList()
                        select new
                        {
                            id = bb.mainCategoryID,
                            pId = "0",
                            name = bb.mainCategoryID + ":" + bb.mainCategoryName
                        };

            var list2 = from bb in db.SubDictionary.ToList()
                        select new
                        {
                            id = bb.mainCategoryID + bb.subDictionaryID,
                            pId = bb.mainCategoryID,
                            name = bb.subDictionaryID + ":" + bb.subDictionaryName,
                        };
            var list4 = list1.Union(list2);
            var list3 = from bb in db.MinorDictionary.ToList()
                        select new
                        {
                            id = bb.mainCategoryID + bb.subDictionaryID + "." + bb.minorDictionaryID,
                            pId = bb.mainCategoryID + bb.subDictionaryID,
                            name = bb.minorDictionaryID + ":" + bb.minorDictionaryName
                        };
            var list5 = list4.Union(list3);
            return Json(list5, JsonRequestBehavior.AllowGet);
        }
        // GET: ArchivesCode
        public ActionResult ClassCode(string currentFilter, string SearchString,  int? SelectedID)
        {
            ViewData["pagename"] = "ArchivesCode/ClassCode";            
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程序号", Value = "1"},
                new SelectListItem { Text = "设计单位", Value = "2" },
                new SelectListItem { Text = "施工单位", Value = "3" },
                new SelectListItem { Text = "接收人", Value = "4" },
                new SelectListItem { Text = "接受日期", Value = "5" },
                new SelectListItem { Text = "整理人", Value = "6" },
                new SelectListItem { Text = "整理日期", Value = "7" },

            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            int t = SelectedID.GetValueOrDefault();

            
            ViewBag.CurrentFilter = SearchString;
            var vwprojectlist = from ad in db.vw_projectList
                                where ad.status == "5"
                                select ad;


            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (t)
                {
                    case 0:
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectName == SearchString);//根据工程名称搜索
                        break;
                    case 1:
                      
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectNo.ToString().Contains(SearchString));//根据地点搜索
                        break;
                    case 2:

                        vwprojectlist = vwprojectlist.Where(ad => ad.disignOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwprojectlist = vwprojectlist.Where(ad => ad.constructionOrganization.Contains(SearchString) );//根据施工单位搜索
                        break;
                    case 4:

                        vwprojectlist = vwprojectlist.Where(ad => ad.recipient.Contains(SearchString));//根据工程序号搜索
                        break;
                    case 5:
                        DateTime date = DateTime.Parse(SearchString);
                        vwprojectlist = vwprojectlist.Where(ad => ad.dateReceived == date);//根据责任书编号搜索
                        break;
                    case 6:

                        vwprojectlist = vwprojectlist.Where(ad => ad.collator.Contains(SearchString));//根据责任书编号搜索
                        break;
                    case 7:
                        DateTime date2 = DateTime.Parse(SearchString);
                        vwprojectlist = vwprojectlist.Where(ad => ad.lqDate == date2);//根据责任书编号搜索
                        break;

                }

            }

            string user = User.Identity.Name;
            bool flag = false;
            var depart = from c in ab.AspNetUsers
                         where c.UserName == user&&c.DepartmentName=="管理科"
                         select c.RoleName;

            foreach (string Role in depart)
            {
                if (Role == "科长")
                {
                    flag = true; break;
                }
                else
                {
                    flag = false;
                }
            }
            if (flag==false)
            {
                var depart1 = from c in ab.AspNetUsers
                             where c.UserName == user && c.DepartmentName == "业务科"
                             select c.RoleName;
                bool flag1 = false;
                foreach (string Role in depart1)
                {
                    if (Role == "科员"||Role=="科长")
                    {
                        flag1 = true; break;
                    }
                    else
                    {
                        flag1 = false;
                    }
                }
                if (flag1==true)
                {
                     vwprojectlist = from a in vwprojectlist
                                where a.collator.Contains(user)
                                select a;
                }
                
            }

            vwprojectlist = vwprojectlist.OrderBy(s => s.projectNo);// 默认按项目顺序号排列
            
            ViewBag.result = JsonConvert.SerializeObject(vwprojectlist);
            return View();
        }
        public ActionResult remove(string  id)
        {
            //ProjectInfo project = db.ProjectInfo.Find(id);
            long Id = long.Parse(id);
            var project = from a in db.ProjectInfo
                          where a.projectID==Id
                          select a;
            project.First().status = "9";  
            db.Entry(project.First()).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ClassCode");

        }
        public ActionResult Coding(long? id)
        {
            string user = User.Identity.Name;
            bool flag1 = false;
            var depart = from c in ab.AspNetUsers
                         where c.UserName == user && c.DepartmentName == "管理科"
                         select c.RoleName;

            foreach (string Role in depart)
            {
                if (Role == "科长")
                {
                    flag1 = true; break;
                }
                else
                {
                    flag1 = false;
                }
            }
            if (flag1==false)
            {
                return Content("<script >alert('对不起，您没有编号权限！');window.history.back();</script >");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "有", Value = "0"},
                new SelectListItem { Text = "没有", Value = "1" },
            };
            ViewBag.IsNullCabinet = new SelectList(list, "Value", "Text",1);

            var n = from a in abc.gxPaperArchives
                    select a.paperProjectSeqNo;

            long n1 = n.Max();

            var n2 = from a in db.PaperArchives
                     select a.paperProjectSeqNo;

            long n3 = n2.Max();

            long n4 = 0;
            if (n3 > n1)
            {
                n4 = n3;
            }
            else
            {
                n4 = n1;
            }

            ViewBag.paperProjectSeqNo = n4 + 1;

            var project = from ad in db.vw_projectList
                          orderby ad.paperProjectSeqNo descending
                          select ad;

            long max_ProjectNo = project.First().paperProjectSeqNo;
            var pro = from ad in db.vw_projectList
                      where ad.projectID == id
                      select ad;
            clearSession();
            Session["isChangeCAB"] = 0;//0:未换柜子，1：换柜子
            if (GenNumber.GenNumber.g_midInfo.totalMix != "" || GenNumber.GenNumber.g_midInfo.totalMix != null)
            {
                GenNumber.GenNumber.g_midInfo.totalMix = "";
            }
            if (Session["prevCabinet1"] != null)
            {
                string prevCabinet1 = Session["prevCabinet1"].ToString();
                if (prevCabinet1 == "00000")
                {
                    ViewBag.IsNullCabinet = new SelectList(list, "Value", "Text", "1");
                    
                }
                else
                {
                    ViewData["CabinetNo"] = prevCabinet1;

                    var cabin = from a in db.CabinetInfo
                                where a.cabinetNo == prevCabinet1
                                select a;
                    CabinetInfo mode = cabin.First();
                    if (mode != null)
                    {
                        ViewData["width1"] = mode.width.ToString();
                        ViewData["remainwidth"] = mode.remainWidth.ToString();
                    }
                    else
                    {
                        return Content("<script >alert('库房信息表中没有柜号为[" + prevCabinet1 + "]的柜子信息，请检查！');window.history.back();</script >");

                    }
                }
            }
            Session["fflag"] = 0;
            string projectID = id.ToString().Trim();
            string detail = pro.First().InchCountDetail;
            if (detail != "" && detail != null)
            {
                getCorrespondThick(detail);
            }

            long sta = Int32.Parse(pro.First().status);
            if (sta == 6)
            {
                int index = pro.First().startArchiveNo.Trim().IndexOf('-');
                ViewData["ClassNo"] = pro.First().startArchiveNo.Trim().Substring(0, index);

                if (pro.First().startPaijiaNo.Trim() == "00000000")
                {
                    ViewBag.IsNullCabinet = new SelectList(list, "Value", "Text", 1);
                }
                else
                {
                    ViewBag.IsNullCabinet = new SelectList(list, "Value", "Text", 0);
                }

                ViewData["register_number_f"] = pro.First().startRegisNo;
                ViewData["register_number_b"] = pro.First().endRegisNo;
                ViewData["archive_number_f"] = pro.First().startArchiveNo;
                ViewData["archive_number_b"] = pro.First().endArchiveNo;
                string paijia = pro.First().startPaijiaNo+"-"+pro.First().endPaijiaNo;
                ViewData["paijia_number_f"] = paijia;
                ViewData["ThickInfo"] = Session["ThickInfo"].ToString();

                ViewData["checkname1"] = "";
                ViewData["button1"]= "none";
                ViewData["button3"]= "none";

                
            }
            if (sta == 5)
            {


                pro.First().paperProjectSeqNo = max_ProjectNo + 1;
                int flag = checkIsProjNameSame(projectID);

                if (flag == 1)
                {
                    ViewData["checkname"] = 1;
                    ViewData["checkname1"] = "";
                }
                if(flag== 2)
                {
                    ViewData["checkname"] = 0;
                    ViewData["checkname1"] = "";
                }
                if (flag != 1 && flag != 2)
                {
                    ViewData["checkname"] = 2;
                    ViewData["checkname1"] =flag; 
                }
               
                ViewData["button2"] = "none";
                //朱莉排错号，改了两天，容易误排，故改之2017.08.06
                //ViewData["ClassNo"] = "I1.2";


            }
            if (pro.First() == null)
            {
                return HttpNotFound();
            }
      
            return View(pro.First());
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //传入

        public ActionResult Coding(string ClassNo, string action, long projectID, string CabinetNo, string register_number_f, string register_number_b, string archive_number_f, string archive_number_b, int? IsNullCabinet, string InchCountDetail, string width, string cengRangeA, string cengRangeB, string remainWidth, string ThickInfo, string trig, string count, string PaiJiaNo, string paperProjectSeqNo, string projectNo)
        {

      

            var project = from ad in db.PaperArchives
                         where ad.projectID == projectID
                        select ad;
            var projectli = from ae in db.vw_projectList
                            where ae.projectID == projectID
                            select ae;
            vw_projectList projectlist = projectli.First();
           
            var pro = from a in db.ProjectInfo
                      where a.projectID == projectID
                     select a;
            ProjectInfo PRO = pro.First();
           
            int t = IsNullCabinet.GetValueOrDefault();
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "有", Value = "0"},
                new SelectListItem { Text = "没有", Value = "1" },
            };
            ViewBag.IsNullCabinet = new SelectList(list, "Value", "Text", t);
            PaperArchives paper = project.First();
            long max_ProjectNo = long.Parse(projectNo);
            int ArchiveCount;
            if (paper.archivesCount == null ||paper.archivesCount.ToString().Trim() == "")
            {
               ArchiveCount = 0;
            }
            ArchiveCount = Int32.Parse(paper.archivesCount);

            if (action == "生成排架号信息")
            {
                //2016.12.13by周林
                if (Int32.Parse(PRO.status) == 6)
                {
                    ViewData["button3"] = true;

                }
                string detail = InchCountDetail.ToString().Trim();
                if (detail != "")
                {
                    getCorrespondThick(detail);
                }
                else
                {

                    return Content("<script >alert('公分数明细为空，无法编号');window.history.back();</script >");
                }


                int index = t;
                if (index == 0)//有柜子
                {

                    if (CabinetNo == "")
                    {
                        return Content("<script >alert('请输入柜号（共五位）');window.history.back();</script >");

                    }
                    if (CabinetNo.Length != 5)
                    {
                        return Content("<script >alert('柜号的长度为排架号前五位');window.history.back();</script >");

                    }
                    else
                    {
                        if (Int32.Parse(CabinetNo[4].ToString()) < 0 || Int32.Parse(CabinetNo[4].ToString()) > 4)
                        {
                            return Content("<script >alert('柜号第5位长度必须在1-4之间');window.history.back();</script >");

                        }
                        if (Int32.Parse(CabinetNo[0].ToString()) < 1)
                        {
                            return Content("<script >alert('柜号第一位不能小于1');window.history.back();</script >");

                        }
                    }
                    insertCabinetInfo(width, cengRangeA, cengRangeB, CabinetNo);
                    int a = getPicAndTextPaijiaNo(InchCountDetail, CabinetNo, cengRangeA, cengRangeB, width, remainWidth, ThickInfo, register_number_f, archive_number_b);
                    ViewData["ThickInfo"] = TempData["ThickInfo"];
                    ViewData["paijia_number_f"] = TempData["paijia_number_f"];
                    ViewData["remainwidth"] = TempData["remainwidth"];
                    //返回不同的值，输出不同的提示消息
                    if (a == 1)
                    {
                        ViewData["checkname"] = 3;

                    }
                    if (a == 2)
                    {
                        ViewData["checkname"] = 4;

                    }
                    if (a == 3)
                    {
                        ViewData["checkname"] = 5;

                    }
                }
                else//没有柜子
                {
                    //进行类型转换，便于进行参数传递
                    int a = Int32.Parse(width);
                    int b = Int32.Parse(cengRangeA);
                    int c = Int32.Parse(cengRangeB);
                    insertCabinetInfo("00000", a, b, c);//插入一个不存在的柜子
                    string str = "00000000-00000000";
                    ViewData["paijia_number_f"] = str;
                    ViewData["ThickInfo"] = Session["ThickInfo"].ToString();

                }
            }
            if (action == "返回")
            {
                if (projectlist.status.Trim()=="5")
                {
                    return RedirectToAction("ClassCode", "ArchivesCode");
                    
                }
                if (projectlist.status.Trim()=="6")
                {
                    return RedirectToAction("CodeEdit", "ArchivesCode");
                }

            }
            if (action == "重新编号提交并返回")
            {
                //查询>=该项目顺序号后的案卷信息，如果分类号不一致，则说明需要进行档号与登记号的修改，并修改与两个分类号有关联的其他表信息
                //遍历所查询的所有案卷信息，如果与这两个分类号相关的案卷信息已有录入的，则予以提示
                //将>该项目顺序号的与原有分类号一致的案卷的档号与总登记号减去案卷数进行更新，并更新最大档号与登记号表的信息
                //将>该项目顺序号的与现有分类号一致的案卷的档号与总登记号加上案卷数进行更新，并更新最大档号与登记号表的信息  

                string paperSeqNo = paperProjectSeqNo;
                long seq = long.Parse(paperProjectSeqNo);
                var pap = from a in db.PaperArchives
                          where a.paperProjectSeqNo == seq
                          select a;
                PaperArchives papModel = pap.First();

                int ArchivesCount = Int32.Parse(Session["ThickCnt"].ToString());

                string[] preRegiterNoEnum = CacuRegiterNo(papModel.startRegisNo.Trim(), ArchivesCount);
                string[] preArchiveNoEnum = CacuArchiveNo(papModel.startArchiveNo.Trim(), ArchivesCount);

                int index = papModel.startArchiveNo.Trim().IndexOf('-');
                string strPrevClassNo = papModel.startArchiveNo.Trim().Substring(0, index);//原有分类号
                string curClassNo = ClassNo.Trim();//现有分类号

                if (strPrevClassNo == curClassNo)
                {
                    return Content("<script >alert('重新编号提交成功,无修改其他工程及案卷数据,返回');window.location='/ArchivesCode/CodeEdit';</script >");
                 
               
                }
                //总登记号与档号应是>该项目顺序号的工程中与现有分类号一致的最近一个工程的起始总登记号与档号
                var ds = from a in db.PaperArchives
                         where a.paperProjectSeqNo > seq && a.startArchiveNo.Contains(curClassNo + "-")
                         orderby a.paperProjectSeqNo
                         select a;
                string registerNo = register_number_f.Trim();
                string archiveNo = archive_number_f.Trim();
                StringBuilder curReventPapeSeqNo = null;//记录与现有分类号一致的所受影响的项目顺序号集合
                StringBuilder preRevenPapeSeqNo = null;//记录与原有分类号一致的所受影响的项目顺序号集合
                int i = 0;
                if (ds.Count()!= 0)//加，倒排
                {
                    //记录数据，后面更新当前项目顺序号的数据时使用
                    register_number_f= ds.First().startRegisNo;    
                    archive_number_f= ds.First().startArchiveNo;  

                    curReventPapeSeqNo = new StringBuilder();
                    for (i = ds.Count() - 1; i > 0; i--)
                    {
                        curReventPapeSeqNo.Append(ds.ElementAt(i).paperProjectSeqNo.ToString().Trim() + ",");
                    }
                    curReventPapeSeqNo.Append(ds.First().paperProjectSeqNo.ToString().Trim());
                }
                var dc = from a in db.PaperArchives
                         where a.paperProjectSeqNo > seq && a.startArchiveNo.Contains(strPrevClassNo + "-")
                         orderby a.paperProjectSeqNo
                         select a;
              
               

                if (dc.Count() != 0)//减，正排
                {
                    preRevenPapeSeqNo = new StringBuilder();
                    for (i = 0; i < dc.Count() - 1; i++)
                    {
                        preRevenPapeSeqNo.Append(dc.ElementAt(i).paperProjectSeqNo.ToString().Trim() + ",");
                    }
                    preRevenPapeSeqNo.Append(dc.First().paperProjectSeqNo.ToString().Trim());
                }
                //下面进行数据的更新

                if (ArchivesCount == 0)
                {
                    return Content("<script >alert('案卷数目不能为0');window.history.back();</script >");
                   
                }
                else
                {
                    string[] regiterNoEnum = CacuRegiterNo(registerNo, ArchivesCount);
                    string[] archiveNoEnum = CacuArchiveNo(archiveNo, ArchivesCount);

                    if (registerNo == "")
                    {
                        return Content("<script >alert('登记号范围不能为空');window.history.back();</script >");
                     
                    }
                    else if (archiveNo == "")
                        
                    {
                        return Content("<script >alert('档号范围不能为空');window.history.back();</script >");
                    }

                    else
                    {
                        try
                        {
                     
                            string YingxiangSeqNo = "";
                            //更新与现有分类号一致的所受影响的项目顺序号集合,对于现有的每个档号和总登记号加上案卷数  
                            
                            if (curReventPapeSeqNo != null)
                            {
                                YingxiangSeqNo += curReventPapeSeqNo.ToString() + ",";
                                string[] curReventpap = curReventPapeSeqNo.ToString().Split(',');
                                foreach (string papSeqNo in curReventpap)
                                {
                                    long papseq = long.Parse(papSeqNo);
                                    var pa = from d in db.PaperArchives
                                             where d.paperProjectSeqNo == papseq
                                             select d;
                                    PaperArchives Model = pa.First();
                                   preArchiveNoEnum = CacuArchiveNo(Model.startArchiveNo.Trim(), ArchivesCount);
                                    Model.startArchiveNo = calcuArchiveNo(Model.startArchiveNo, ArchivesCount, true);
                                    Model.endArchiveNo = calcuArchiveNo(Model.endArchiveNo, ArchivesCount, true);
                                    db.Entry(Model).State = EntityState.Modified;
                                   archiveNoEnum = CacuArchiveNo(papModel.startArchiveNo, ArchivesCount);
                                    for (i = archiveNoEnum.Length - 1; i >= 0; i--)//加，倒排
                                    {
                                        string archiveNO = preArchiveNoEnum[i];
                                        var archive = from f in db.ArchivesDetail
                                                      where f.archivesNo == archiveNO
                                                      select f;
                                        ArchivesDetail archModel = archive.First();
                                        if (archModel != null)
                                        {
                                            archModel.archivesNo = archiveNoEnum[i];
                                            db.Entry(archModel).State = EntityState.Modified;
                                           
                                        }
                                    }

                                 }
                                //db.SaveChanges();
                               }

                            //更新当前项目顺序号的档号与总登记号信息
                            paperSeqNo = paperSeqNo.Trim();
                            

                            preRegiterNoEnum = CacuRegiterNo(papModel.startRegisNo.Trim(), ArchivesCount);
                            preArchiveNoEnum = CacuArchiveNo(papModel.startArchiveNo.Trim(), ArchivesCount);

                            int nB2 = 1 + curClassNo[0] - 'A';
                            string strB2 = "";
                            if (nB2 < 10)
                            {
                                strB2 = "0" + nB2.ToString();
                            }
                            else
                            {
                                strB2 = nB2.ToString();
                            }

                            registerNo = register_number_f.Trim();
                            archiveNo = archive_number_f.Trim();

                            regiterNoEnum = CacuRegiterNo(registerNo, ArchivesCount);
                            archiveNoEnum = CacuArchiveNo(archiveNo, ArchivesCount);

                            papModel.startArchiveNo = archiveNoEnum[0];
                            papModel.endArchiveNo = archiveNoEnum[ArchivesCount - 1];
                            papModel.startRegisNo = strB2 + preRegiterNoEnum[0].Substring(2, preRegiterNoEnum[0].Length - 2);
                            papModel.endRegisNo = strB2 + preRegiterNoEnum[ArchivesCount - 1].Substring(2, preRegiterNoEnum[ArchivesCount - 1].Length - 2);
                            index = curClassNo.IndexOf('.');
                            if (index != -1)
                            {
                                papModel.mainCategoryID = curClassNo[0].ToString();
                                papModel.subDictionaryID = curClassNo.Substring(1, index - 1);
                                papModel.minorDictionaryID = curClassNo.Substring(index + 1, curClassNo.Length - index - 1);
                            }
                            db.Entry(papModel).State = EntityState.Modified;
                           
                            //更新当前项目顺序号所对应案卷的档号与登记号信息

                            for (i = 0; i < archiveNoEnum.Length; i++)
                            {
                                string archiveNO = preArchiveNoEnum[i];
                                var archive = from f in db.ArchivesDetail
                                              where f.archivesNo == archiveNO
                                              select f;
                                ArchivesDetail archModel = archive.First();
                               
                                if (archModel != null)
                                {
                                    archModel.archivesNo = archiveNoEnum[i];
                                    archModel.registrationNo = strB2 + preRegiterNoEnum[i].Substring(2, preRegiterNoEnum[i].Length - 2);
                                    db.Entry(archModel).State = EntityState.Modified;
                                    //db.SaveChanges();
                                }
                            }

                            //更新与原有分类号一致的所受影响的项目顺序号集合,对于原有的每个档号和总登记号减去案卷数
                            if (preRevenPapeSeqNo != null)
                            {
                                YingxiangSeqNo += preRevenPapeSeqNo.ToString();
                                string[] preReventpap = preRevenPapeSeqNo.ToString().Split(',');
                                foreach (string papSeqNo in preReventpap)
                                {
                                    long seq1 = long.Parse(papSeqNo);
                                    var model1 = from j in db.PaperArchives
                                                 where j.paperProjectSeqNo == seq1
                                                 select j;
                                    PaperArchives mod = model1.First();


                                    preArchiveNoEnum = CacuArchiveNo(mod.startArchiveNo.Trim(), ArchivesCount);

                                    mod.startArchiveNo = calcuArchiveNo(mod.startArchiveNo, ArchivesCount, false);
                                    mod.endArchiveNo = calcuArchiveNo(mod.endArchiveNo, ArchivesCount, false);

                                    db.Entry(mod).State = EntityState.Modified;
                                    //更新该项目顺序号下的案卷数据的档号与总登记号
                                    regiterNoEnum = CacuRegiterNo(papModel.startRegisNo, ArchivesCount);
                                    archiveNoEnum = CacuArchiveNo(papModel.startArchiveNo, ArchivesCount);
                                    for (i = 0; i < archiveNoEnum.Length; i++)
                                    {
                                        string archive = preArchiveNoEnum[i];
                                        var arch = from h in db.ArchivesDetail
                                                   where h.archivesNo == archive
                                                   select h;

                                        ArchivesDetail archModel1 = arch.First();
                                        if (archModel1 != null)
                                        {
                                            archModel1.archivesNo = archiveNoEnum[i];

                                            db.Entry(archModel1).State = EntityState.Modified;
                                        }

                                    }

                                }
                                //db.SaveChanges();
                                
                            }

                            //更新与两个分类号相关的最大档号与总登记号表中的档号数据
                           
                            var imaxARNo = from g in db.MaxArchiveRegisNo
                                           where g.mainCategoryID == strPrevClassNo
                                           select g;
                            MaxArchiveRegisNo maxARModel = imaxARNo.First();
                            maxARModel.maxArchiveNo = calcuArchiveNo(maxARModel.maxArchiveNo, ArchivesCount, false);
                            db.Entry(maxARModel).State = EntityState.Modified;
                            var imaxARNo1 = from g in db.MaxArchiveRegisNo
                                           where g.mainCategoryID == curClassNo
                                            select g;
                            MaxArchiveRegisNo maxARModel1 = imaxARNo1.First();
                            maxARModel1.maxArchiveNo = calcuArchiveNo(maxARModel1.maxArchiveNo, ArchivesCount, true);
                            db.Entry(maxARModel1).State = EntityState.Modified;
                            db.SaveChanges();
                            if (YingxiangSeqNo != "")
                            {
                                return Content("<script >alert('重新编号提交成功,共修改其他项目顺序号为[" + YingxiangSeqNo + "]的工程及案卷数据,返回');window.location='http://localhost:59320/ArchivesCode/CodeEdit';</script >");
                                
                            }
                            else
                            {
                                return Content("<script >alert('重新编号提交成功,无修改其他工程及案卷数据,返回,返回');window.location='http://localhost:59320/ArchivesCode/CodeEdit';</script >");
                              
                            }
                        }
                        catch (Exception ex)
                        {
                            Response.Write(ex.Message);
                        }
                      
                    }
                }
              

            }
            if (action == "提交并返回")
            {
                if (CabinetNo == ""|| CabinetNo ==null)
                {
                    Session["prevCabinet"] = "00000";
                }
                else
                {
                    Session["prevCabinet"] = CabinetNo.Trim();
                }
                if ( register_number_f==""||register_number_b==""||archive_number_f==""||archive_number_b=="")
                {
                    return Content("<script >alert('登记号和档号不能为空！');window.history.back();</script >");
                }
                string strSeqNo = paperProjectSeqNo.Trim();
                string PaijNo = PaiJiaNo.Trim();
                if (PaijNo == "")
                {
                    return Content("<script >alert('排架号不能为空');window.history.back();</script >");

                }
                bool flag = isStringValid(PaijNo);
                if (flag == false)//字符串不合法
                {
                    return Content("<script >alert('排架号输入格式不正确，请核对');window.history.back();</script >");

                }
                string registerNo = register_number_f.Trim();
                string archiveNo = archive_number_f.Trim();
                int ArchivesCount = Int32.Parse(Session["ThickCnt"].ToString());
                string thick = Session["ThickInfo"].ToString();
                if (ArchivesCount == 0)
                {
                    return Content("<script >alert('案卷数目不能为0');window.history.back();</script >");

                }
                else
                {
                    int index = t;

                    string[] pjNoEnum = CacuPaijiaNo(PaijNo, ArchivesCount, index);
                    if (pjNoEnum[0] == "a")
                    {
                        return Content("<script >alert('此" + pjNoEnum[1] + "的排架号少于8位，请核查！');window.history.back();</script >");
                    }
                    if (pjNoEnum[0] == "b")
                    {
                        return Content("<script >alert('此" + PaijNo + "排架号长度必须为8位！');window.history.back();</script >");
                    }
                    if (pjNoEnum[0] == "c")
                    {
                        return Content("<script >alert('排架号个数与案卷数目不一致，请核查！');window.history.back();</script >");
                    }
                    if (pjNoEnum[0] == "d")
                    {
                        return Content("<script >alert('此" + pjNoEnum[1] + "排架号长度不是8位！');window.history.back();</script >");
                    }
                    string[] regiterNoEnum = CacuRegiterNo(registerNo, ArchivesCount);
                    string[] archiveNoEnum = CacuArchiveNo(archiveNo, ArchivesCount);
                    string[] thickInfoEnum = CacuThickInfo(thick, ArchivesCount);
                    if (registerNo == "")
                    {
                        return Content("<script >alert('登记号范围不能为空！');window.history.back();</script >");

                    }
                    else if (archiveNo == "")
                    {
                        return Content("<script >alert('档号范围不能为空！');window.history.back();</script >");

                    }
                    else if (pjNoEnum == null)
                    {
                        return Content("<script >alert('排架号不能为空！');window.history.back();</script >");

                    }
                    else if (regiterNoEnum[ArchivesCount - 1] != register_number_b.Trim())
                    {
                        return Content("<script >alert('总登记号编号范围错误！');window.history.back();</script >");

                    }
                    else if (archive_number_b.Trim() != archiveNoEnum[ArchivesCount - 1])
                    {
                        return Content("<script >alert('档号编号范围错误！');window.history.back();</script >");

                    }
                    else
                    {
                    
                            string id = projectID.ToString();
                            string classno = ClassNo;


                            for (int i = 0; i < archiveNoEnum.Length; i++)
                            {
                                string archive = archiveNoEnum[i];
                                var archModel = from a in db.ArchivesDetail
                                                where a.archivesNo==archive
                                                select a;
                                if (archModel.Count() != 0)
                                {
                                    return Content("<script >alert('您所编辑档号[" + archiveNoEnum[i] + "]已经存在，请返回检查');window.history.back();</script >");

                                }
                            }

                        bool bFlag = updateMaxArchiveRegisPaijiaNo(classno, archiveNoEnum[ArchivesCount - 1], regiterNoEnum[ArchivesCount - 1], pjNoEnum, thickInfoEnum);
                        //if (bFlag == true)
                        //{
                        //    //更新详细档案表
                        //    insertArchivesDetail(strSeqNo, pjNoEnum, regiterNoEnum, archiveNoEnum, ArchivesCount, thickInfoEnum);
                        //    //更新档案案卷表
                        //    updatePaperArchives(Int32.Parse(id), classno, pjNoEnum, strSeqNo, archiveNoEnum[0], archiveNoEnum[ArchivesCount - 1], pjNoEnum[0], pjNoEnum[ArchivesCount - 1], regiterNoEnum[0], regiterNoEnum[ArchivesCount - 1], ArchivesCount);
                        //    //更新工程信息表
                        //    UpdateArchiveStatus(id,6);
                        //    db.SaveChanges();
                        //    return Content("<script language=javascript>alert('提交成功,返回');window.location='/ArchivesCode/ClassCode';</script>");
                        //}
                        if (bFlag == true)
                        {
                            var num = from a in db.ArchivesDetail
                                      where a.SingleProjectId == PRO.GCId
                                      //where a.SingleProjectId != null
                                      select a;
                            //更新详细档案表
                            if (num.Count() == 0 || PRO.GCId == null)
                            {
                                insertArchivesDetail(strSeqNo, pjNoEnum, regiterNoEnum, archiveNoEnum, ArchivesCount, thickInfoEnum);
                            }

                            else
                            {
                                updateArchivesDetail(PRO.GCId, strSeqNo, pjNoEnum, regiterNoEnum, archiveNoEnum, ArchivesCount, thickInfoEnum);
                                //string path = "G:\\JunGongArchives1\\" + id;
                                string paperProjectSeqNo1 = paperProjectSeqNo.PadLeft(5, '0');
                                string path1 = "G:\\JunGongArchives\\" + paperProjectSeqNo1;
                                string path2 = "G:\\JunGongArchives\\" + paper.projectNo.ToString();
                                if (Directory.Exists(path2))
                                {
                                    System.IO.Directory.Move(path2, path1);
                                }
                            }
                            //更新档案案卷表
                            updatePaperArchives(Int32.Parse(id), classno, pjNoEnum, strSeqNo, archiveNoEnum[0], archiveNoEnum[ArchivesCount - 1], pjNoEnum[0], pjNoEnum[ArchivesCount - 1], regiterNoEnum[0], regiterNoEnum[ArchivesCount - 1], ArchivesCount);
                            //更新工程信息表
                            UpdateArchiveStatus(id, 6);
                            db.SaveChanges();
                            return Content("<script language=javascript>alert('提交成功,返回');window.location='/ArchivesCode/ClassCode';</script>");
                        }

                    }
                }
               
            }

           
            ViewData["ClassNo"] = ClassNo;
            ViewData["CabinetNo"] = CabinetNo;
            ViewData["checkname1"] = "";
            return View(projectlist);
        }
        public static bool DeleteFolder(string floderPath)
        {
            try
            {
                if (Directory.Exists(floderPath))
                {
                    var r = Directory.GetFileSystemEntries(floderPath);
                    foreach (var inst in Directory.GetFileSystemEntries(floderPath))
                    {
                        if (System.IO.File.Exists(inst))
                        {
                            System.IO.File.Delete(inst); //直接删除其中的文件 
                        }
                        else
                        {
                            DeleteFolder(inst); //递归删除子文件夹 
                        }
                    }
                    //删除已空文件夹 
                    Directory.Delete(floderPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message, ex);
            }
        }
        public ActionResult  BianhaoLog()
        {
            var a = from b in db.BianHaoLog
                    select b;
            ViewBag.result = JsonConvert.SerializeObject(a);
            return View();
        }
        public string CreateArchive(string InchCountDetail,string ClassNo,string archivesCount, string projectID,string register_number_f,string register_number_b)
        {
            int flag = 0;
            
            string detail = InchCountDetail.ToString().Trim();
            long Pid = long.Parse(projectID.Trim());
      
            var model = from a in db.vw_passList
                        where a.projectID == Pid
                        select a;
            string status = model.First().status;
            DataTable myTable = null;
            myTable = new DataTable();

            DataRow myDataRow;

            myTable.Columns.Add("register_number_f", Type.GetType("System.String"));

            myTable.Columns.Add("register_number_b", Type.GetType("System.String"));
            myTable.Columns.Add("archive_number_f", Type.GetType("System.String"));
            myTable.Columns.Add("archive_number_b", Type.GetType("System.String"));
            myTable.Columns.Add("flag", Type.GetType("System.String"));
            myDataRow = myTable.NewRow();
           
            if (detail != "")
            {
                getCorrespondThick(detail);
            }
            else
            {
                flag = 1;//公分数明细为空，无法编号
                myDataRow["flag"] = flag;
                myTable.Rows.Add(myDataRow);

                return JsonConvert.SerializeObject(myTable);
            }
            string strClassNo = ClassNo.Trim();
            if (strClassNo == "")
            {
      
                flag = 2;//请先输入分类号
            }
            else
            {
                int ArchivesCount = Int32.Parse(Session["ThickCnt"].ToString());
                var max_archiveNo = from ac in db.MaxArchiveRegisNo
                                    where ac.mainCategoryID == ClassNo
                                    select ac;
                if (max_archiveNo.Count() != 0)
                {
                    MaxArchiveRegisNo maxModel = max_archiveNo.First();
                    generatefollowByMaxModel(maxModel, strClassNo, ArchivesCount, status, register_number_f, register_number_b);
                   
                    myDataRow["register_number_f"] = TempData["register_number_f"];
                    myDataRow["register_number_b"] = TempData["register_number_b"];
                    myDataRow["archive_number_f"] = TempData["archive_number_f"];
                    myDataRow["archive_number_b"] = TempData["archive_number_b"];
                   
                }

                else
                {
                    flag = 3;//最大档号与登记号表 中无此分类号信息，请填入该分类号信息

                }
            }

            myDataRow["flag"] = flag;
            myTable.Rows.Add(myDataRow);

            return JsonConvert.SerializeObject(myTable);
        }
        public string CreatePaijia(string InchCountDetail,string width,  string cengRangeA,string projectID, string cengRangeB)
        {
            long ID = long.Parse(projectID.Trim());
            var model = from g in db.ProjectInfo
                        where g.projectID == ID
                        select g;
            int flag = 0;
            if (Int32.Parse(model.First().status) == 6)
            {
                flag = 1;//使编排架号的按钮显示

            }
            string detail = InchCountDetail.ToString().Trim();
            if (detail != "")
            {
                getCorrespondThick(detail);
            }
            else
            {
                flag = 2;//公分数明细为空，无法编号
                
            }
         //进行类型转换，便于进行参数传递
                int a = Int32.Parse(width);
                int b = Int32.Parse(cengRangeA);
                int c = Int32.Parse(cengRangeB);
                insertCabinetInfo("00000", a, b, c);//插入一个不存在的柜子
                string str = "00000000-00000000";
                DataTable myTable = null;
                myTable = new DataTable();

               DataRow myDataRow;

               myTable.Columns.Add("paijia_number_f", Type.GetType("System.String"));

               myTable.Columns.Add("ThickInfo", Type.GetType("System.String"));

               myTable.Columns.Add("flag", Type.GetType("System.String"));
               myDataRow = myTable.NewRow();
               myDataRow["paijia_number_f"] = str;
               myDataRow["ThickInfo"] = Session["ThickInfo"].ToString();
              
            
               myDataRow["flag"] = flag;
               myTable.Rows.Add(myDataRow);

            return JsonConvert.SerializeObject(myTable);
        }
        private void suanhao(ref string strstart, ref string strend, int ArchivesCount)
        {
            int len = 0; ;
            string str0len = "";
            int flag = -1;
            int temp;
            if (Int32.Parse(strstart) == 0)
            {
                flag = 0;//字符串为全0
            }
            else
            {
                for (len = 0; len < strstart.Length; len++)
                {
                    if (strstart[0] != '0')
                    {
                        flag = 2;//字符串前面没0
                        break;
                    }
                    if (strstart[len] == '0' && strstart[len + 1] != '0')
                    {
                        flag = 1; break;//字符串前面有0，非全0
                    }
                }
            }
            if (flag == 1)
            {
                len++;
                int lentemp;
                temp = Int32.Parse(strstart.Substring(len, strstart.Length - len));

                lentemp = temp.ToString().Length;
                temp += 1;
                int lentemp2 = temp.ToString().Length;
                int len21 = lentemp2 - lentemp;

                for (int i = 0; i < len - len21; i++)
                    str0len += "0";
                str0len += temp.ToString();
                strstart = str0len;

                lentemp = temp.ToString().Length;
                temp += ArchivesCount - 1;
                lentemp2 = temp.ToString().Length;
                int len22 = lentemp2 - lentemp;
                str0len = "";
                if (len <= len21 + len22)
                {
                    strend = temp.ToString();
                }
                else
                {
                    for (int i = 0; i < len - len21 - len22; i++)
                        str0len += "0";
                    strend = str0len + temp.ToString();
                }
            }
            else if (flag == 2)
            {

                temp = Int32.Parse(strstart) + 1;
                strstart = temp.ToString();
                temp += ArchivesCount - 1;
                strend = temp.ToString();
            }
            else//flag=0
            {
                len = strstart.Length;
                for (int i = 0; i < len - 1; i++)
                {
                    str0len += "0";
                }
                strstart = str0len + "1";
                str0len += "0";
                strend = ArchivesCount.ToString(str0len);
            }
        }
        private string[] CacuArchiveNo(string archiveNo, int count)
        {
            string[] ArchiveNoEnum = null;
            string str = "";
            string[] strTemp = archiveNo.Split('-');
            string temp = strTemp[1];
            if (count == 1)
            {
                ArchiveNoEnum = new string[1];
                ArchiveNoEnum[0] = archiveNo;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    str += strTemp[0] + "-" + temp.ToString() + ",";
                    suanhao2(ref temp);
                }
                ArchiveNoEnum = str.Substring(0, str.Length - 1).Split(',');
            }
            return ArchiveNoEnum;
        }
        private string[] CacuRegiterNo(string regiterNo, int cnt)
        {
            string[] registerEnum = null;
            string str = "";
            int length;
            length = regiterNo.Length;
            string strSub = regiterNo.Substring(2, length - 2);
            if (cnt == 1)
            {
                registerEnum = new string[1];
                registerEnum[0] = regiterNo;
            }
            else
            {
                for (int i = 0; i < cnt; i++)
                {
                    str += regiterNo.Substring(0, 2) + strSub.ToString() + ",";
                    suanhao2(ref strSub);
                }
                registerEnum = str.Substring(0, str.Length - 1).Split(',');
            }
            return registerEnum;
        }
        private void suanhao2(ref string strstart)
        {
            int len = 0; ;
            string str0len = "";
            int flag = -1;
            int temp;
            if (Int32.Parse(strstart) == 0)
            {
                flag = 0;//字符串为全0
            }
            else
            {
                for (len = 0; len < strstart.Length; len++)
                {
                    if (strstart[0] != '0')
                    {
                        flag = 2;//字符串前面没0
                        break;
                    }
                    if (strstart[len] == '0' && strstart[len + 1] != '0')
                    {
                        flag = 1; break;//字符串前面有0，非全0
                    }
                }
            }

            if (flag == 1)
            {
                len++;
                int lentemp;
                temp = Int32.Parse(strstart.Substring(len, strstart.Length - len));

                lentemp = temp.ToString().Length;
                temp += 1;
                int lentemp2 = temp.ToString().Length;
                int len21 = lentemp2 - lentemp;

                for (int i = 0; i < len - len21; i++)
                    str0len += "0";
                str0len += temp.ToString();
                strstart = str0len;
            }
            else if (flag == 2)
            {
                temp = Int32.Parse(strstart) + 1;
                strstart = temp.ToString();
            }
            else//flag=0
            {
                len = strstart.Length;
                for (int i = 0; i < len - 1; i++)
                {
                    str0len += "0";
                }
                strstart = str0len + "1";
            }
        }
        public ActionResult CodeEdit(string sortOrder, string currentFilter, string SearchString, int? page, int? SelectedID)
        {
            ViewData["pagename"] = "ArchivesCode/CodeEdit";
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程序号", Value = "1"},
                new SelectListItem { Text = "设计单位", Value = "2" },
                new SelectListItem { Text = "施工单位", Value = "3" },
                new SelectListItem { Text = "项目顺序号", Value = "4" },
                new SelectListItem { Text = "起始排架号", Value = "5" },
                new SelectListItem { Text = "档号", Value = "6" },


            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            int t = SelectedID.GetValueOrDefault();

            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }
            ViewBag.CurrentFilter = SearchString;
            var vwprojectlist = from ad in db.vw_projectList
                                where ad.status == "6"
                                select ad;


            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (t)
                {
                    case 0:
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 1:
                        
                        vwprojectlist = vwprojectlist.Where(ad => ad.projectNo.ToString().Contains(SearchString));//根据地点搜索
                        break;
                    case 2:

                        vwprojectlist = vwprojectlist.Where(ad => ad.disignOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwprojectlist = vwprojectlist.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位搜索
                        break;
                    case 4:

                        vwprojectlist = vwprojectlist.Where(ad => ad.paperProjectSeqNo.ToString().Trim() == SearchString);//根据工程序号搜索
                        break;
                    case 5:

                        vwprojectlist = vwprojectlist.Where(ad => ad.startPaijiaNo.Contains(SearchString));
                        break;
                    case 6:

                        vwprojectlist = vwprojectlist.Where(ad => ad.collator.Contains(SearchString));//根据责任书编号搜索
                        break;



                }

            }



            vwprojectlist = vwprojectlist.OrderByDescending(s => s.paperProjectSeqNo);// 默认按项目顺序号排列
            //int pageSize = 50;
            //int pageNumber = (page ?? 1);
            //return View(vwprojectlist.ToPagedList(pageNumber, pageSize));
            ViewBag.result = JsonConvert.SerializeObject(vwprojectlist);
            return View();
        }
        private void getCorrespondThick(string detail)
        {
            int ndotspace = 0;//0:逗号隔开，1：空格隔开

            int index = detail.IndexOf(',');
            if (index == -1)
            {
                index = detail.IndexOf(' ');
                ndotspace = 1;
            }
            string strResult = "";
            int thickCnt = 0;
            if (index == -1)//字符串中没有逗号与空格



            {
                string[] strEnum = detail.Split('*');
                thickCnt += Int32.Parse(strEnum[1]);
                for (int i = 0; i < Int32.Parse(strEnum[1]); i++)
                {
                    strResult += strEnum[0] + ",";
                }
            }
            else//字符串中有逗号
            {
                string[] strEnum;
                if (ndotspace == 0)
                {
                    strEnum = detail.Split(',');
                }
                else
                {
                    strEnum = detail.Split(' ');
                }
                foreach (string str in strEnum)
                {
                    string[] s = str.Split('*');
                    thickCnt += Int32.Parse(s[1]);
                    for (int i = 0; i < Int32.Parse(s[1]); i++)
                    {
                        strResult += s[0] + ",";
                    }
                }
            }
            if (strResult.Length > 1)
                strResult = strResult.Substring(0, strResult.Length - 1);
            //txtThickInfo.Text = strResult;
            Session["ThickInfo"] = strResult;
            Session["ThickCnt"] = thickCnt;
            Session["fflag"] = 1;
        }
        private int checkIsProjNameSame(string projectID)
        {


            long ID = Convert.ToInt32(projectID);
            var pro = from b in db.ProjectInfo
                      where b.projectID == ID
                      select b;
            string str = pro.First().projectName;
            long proid = pro.First().projectID;
            var project = from a in db.ProjectInfo
                          where a.projectName == str && a.projectID != proid
                          orderby a.projectID
                          select a;
            if (project.Count() != 0)
            {
                foreach (var item in project)
                {
                    var paper = from c in db.PaperArchives
                                where c.projectID == item.projectID
                                select c;
                    if (paper.Count() != 0)
                    {
                        int a = Convert.ToInt32(paper.First().paperProjectSeqNo);
                        return a;

                    }
                    else
                    {
                        return 1;
                    }
                }


            }

            return 2;

        }
        private void insertCabinetInfo(string txtGuiWidth, string txtCengCntA, string txtCengCntB, string txtGuihao)
        {
            string strCabinetNo = txtGuihao.Trim();
            if(txtGuiWidth==""|| txtGuiWidth==null)
            {
                txtGuiWidth = "0";
            }
            int width = Int32.Parse(txtGuiWidth.Trim());
            int cengA = Int32.Parse(txtCengCntA.Trim());
            int cengB = Int32.Parse(txtCengCntB.Trim());
            var cabin = from a in db.CabinetInfo
                        where a.cabinetNo == txtGuihao
                        select a;


            if (cabin.Count() == 0)
            {
                CabinetInfo model = new CabinetInfo();
                model.cabinetNo = strCabinetNo;
                model.width = width;
                model.cengRangeA = cengA;
                model.cengRangeB = cengB;
                model.startPaijiaNo = strCabinetNo + txtCengCntA.Trim() + "00";
                model.remainWidth = width;

                db.CabinetInfo.Add(model);
                db.SaveChanges();
            }
        }
        private int getPicAndTextPaijiaNo(string InchCountDetail, string txtGuihao, string txtCengCntA, string txtCengCntB, string txtGuiWidth, string txtremainWidth, string ThickInfo, string register_number_f, string archive_number_f)
        {


            string strCabinetNo = txtGuihao.Trim();
            string classString = InchCountDetail.Trim();
            if(txtGuiWidth==null|| txtGuiWidth=="")
            {
                txtGuiWidth = "0";
            }
            int width = Int32.Parse(txtGuiWidth.Trim());
            int cengA = Int32.Parse(txtCengCntA.Trim());
            int cengB = Int32.Parse(txtCengCntB.Trim());
            int ndotspace = 0;//处理公分数明细数据，使之以逗号隔开
            int index = classString.IndexOf(',');
            if (index == -1)
            {
                index = classString.IndexOf(' ');
                if (index != -1)
                {
                    ndotspace = 1;
                }
            }
            if (ndotspace == 1)//公分数明细是以空格隔开的



            {
                string strTempClassString = "";
                string[] tempString = classString.Split(' ');
                foreach (string s in tempString)
                {
                    strTempClassString += s.Trim() + ",";
                }
                classString = strTempClassString.Substring(0, strTempClassString.Length - 1);
            }
            GenNumber.GenNumber.ReturnInfo info = new GenNumber.GenNumber.ReturnInfo();//结构体





            string str = "";
            string thick = "";
            int fflagg = getPaiJiaNoInfo(strCabinetNo, width, cengA, cengB, classString, ref info, txtremainWidth);
            if (fflagg == 0)
            {
                foreach (StringBuilder sb in info.s)
                {
                    str += sb.ToString().Substring(sb.Length - 8, 8) + ",";
                    switch (sb.ToString().Substring(0, 1))
                    {
                        case "1":
                            thick += "1,"; break;
                        case "2":
                            thick += "2,"; break;
                        case "3":
                            thick += "3,"; break;
                        case "4":
                            thick += "4,"; break;
                        case "5":
                            thick += "5,"; break;
                    }
                }
                if (thick != "")
                {
                    thick = thick.Substring(0, thick.Length - 1);
                    TempData["ThickInfo"] = thick;
                }
                if (str != "")
                {
                    str = dealWiththeString(str.Substring(0, str.Length - 1));
                    TempData["paijia_number_f"] = str;

                }

                Session["returnInfo"] = info.s;
                Session["startArchiveNo"] = archive_number_f.Trim();
                Session["startRegistNo"] = register_number_f.Trim();
                Session["count"] = Session["ThickCnt"].ToString();
                //btnShowDetail.Enabled = true;
                TempData["remainwidth"] = info.remainWidth.ToString();

                return 1;
            }
            else if (fflagg == 2)//库房已满
            {

                Session["isChangeCAB"] = 1;
                return 2;
            }
            else
            {
                return 3;

            }
        }
        private int getPaiJiaNoInfo(string strGuihao, int width, int rangeA, int rangeB, string classString, ref GenNumber.GenNumber.ReturnInfo info, string txtremainWidth)
        {
            int flag = -1;//1:柜子本身已满，0：柜子未满，2：柜子装过部分后已满
            GenNumber.GenNumber.OutInfo outInfo = new GenNumber.GenNumber.OutInfo();
            GenNumber.GenNumber.InInfo inInfo = new GenNumber.GenNumber.InInfo();
            //inInfo.cabinetNum = Int32.Parse(strGuihao);
            inInfo.drawerWidth = width;//将宽度赋给层宽度

            int fg = Int32.Parse(Session["isChangeCAB"].ToString());
            if (fg == 0)//没换柜子
            {
                inInfo.totalMix = classString;//将详细公分数的字符串形式赋给案卷字符串
            }
            else
            {
                inInfo.totalMix = GenNumber.GenNumber.g_midInfo.totalMix;//不理解啥意思
            }
            int remainWidth = 0;
            string startPaijiaNo = "";
            bool chaKFflag = chaKufang(ref startPaijiaNo, ref remainWidth, strGuihao, width, rangeB, rangeA);
            if (chaKFflag == false)
            {
                flag = 1;
            }
            else
            {
                inInfo.curPaiJiaHao = startPaijiaNo;
                inInfo.remainWidth = remainWidth;
                inInfo.maxDrawerNum = rangeB;
                //txtRemainWidth.Text = inInfo.remainWidth.ToString();

                //递归排号
                GenNumber.GenNumber.PreNeed(ref inInfo, ref outInfo);
                GenNumber.GenNumber.GenShelfNumber(inInfo, ref outInfo);
                //提示出错----柜子已满，请用户输入另一个柜子




                if (outInfo.errorMsg != null)
                {
                    flag = 2;
                }
                else
                {
                    GenNumber.GenNumber.ReturnInfo returnInfo = GenNumber.GenNumber.GetGenNumInfo();
                    info = returnInfo;
                    flag = 0;
                }
            }
            return flag;
        }
        private void clearSession()
        {
            if (Session["CurDraw"] != null)
                Session.Remove("CurDraw");
            if (Session["remainWidth"] != null)
                Session.Remove("remainWidth");
            if (Session["isChangeCAB"] != null)
                Session.Remove("isChangeCAB");
            if (Session["returnInfoWenzi"] != null)
                Session.Remove("returnInfoWenzi");
            if (Session["returnInfoTuzhi"] != null)
                Session.Remove("returnInfoTuzhi");
        }
        private bool chaKufang(ref string startPaijiaNo, ref int remainWidth, string strGuihao, int width, int txtCengCntB, int txtCengCntA)
        {

            var cabit = from d in db.CabinetInfo
                        where d.cabinetNo == strGuihao
                        select d;
            CabinetInfo model = cabit.First();
            if (model != null)

            {
                startPaijiaNo = model.startPaijiaNo.Trim();
                remainWidth = Convert.ToInt32(model.remainWidth); ;
                if (remainWidth <= 0)
                {
                    int cengshu = Int32.Parse(startPaijiaNo[5].ToString()) + 1;
                    if (cengshu > txtCengCntB)
                    {
                        return false;
                    }
                    else
                    {
                        startPaijiaNo = strGuihao + cengshu.ToString() + "01";
                        remainWidth = width;
                    }
                }
            }
            else
            {
                insertCabinetInfo(strGuihao, width, txtCengCntA, txtCengCntB);
                startPaijiaNo = strGuihao + txtCengCntA.ToString().Trim() + "01";
                remainWidth = width;
            }
            ViewData["remainwidth"] = remainWidth.ToString();

            return true;
        }
        private void insertCabinetInfo(string strCabinetNo, int txtGuiWidth, int txtCengCntA, int txtCengCntB)
        {

            int width = txtGuiWidth;
            if (strCabinetNo == "00000")//如果是不存在的柜子，则将其宽度设置足够宽，使其能一次装下需要编号的案卷
                width = 10000;
            int cengA = txtCengCntA;
            int cengB = txtCengCntB;
            var cabit = from a in db.CabinetInfo
                        where a.cabinetNo == strCabinetNo
                        select a;

            //CabinetInfo model = cabit.First();
            
            if (cabit.Count() ==0)
            {
                CabinetInfo model = new CabinetInfo();
                model.cabinetNo = strCabinetNo;
                model.width = width;
                model.cengRangeA = cengA;
                model.cengRangeB = cengB;
                model.startPaijiaNo = strCabinetNo + txtCengCntA.ToString().Trim() + "00";
                model.remainWidth = width;

                db.CabinetInfo.Add(model);
                db.SaveChanges();
                return;

            }
            else
            {
                if (strCabinetNo == "00000")
                {//如果是不存在的空柜子，更改其宽度和起始排架号，使其能继续使用
                    CabinetInfo model = cabit.First();
                    if (model.remainWidth < 5000)
                        model.remainWidth = 10000;
                    model.startPaijiaNo = "00000000";
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return;
                }
            }
        }
        private string dealWiththeString(string strs)
        {
            //根据前六位进行分类
            string strResult = "";
            int index = strs.IndexOf(',');
            if (index == -1)
            {
                strResult = strs;
            }
            else
            {
                string[] s = strs.Split(',');
                StringBuilder sBuilder = new StringBuilder();
                paijia_s[] paiJiaArray = new paijia_s[s.Length];
                int i, j = 0;
                foreach (string str in s)
                {
                    bool flag = false;
                    for (i = 0; i <= j; i++)
                    {
                        if (str.Substring(0, 6) == paiJiaArray[i].strFlag)
                        {
                            if (paiJiaArray[i].sBuilder == null || paiJiaArray[i].sBuilder == string.Empty || paiJiaArray[i].sBuilder == "")
                            {
                                flag = true;
                                paiJiaArray[i].sBuilder = str;
                            }
                            else
                            {
                                flag = true;
                                paiJiaArray[i].sBuilder = paiJiaArray[i].sBuilder.Substring(0, 8) + "-" + str;
                            }
                        }
                    }
                    if (flag == false)
                    {
                        paiJiaArray[j].strFlag = str.Substring(0, 6);
                        paiJiaArray[j].sBuilder = str;
                        j = j + 1;
                    }
                }
                for (i = 0; i < j; i++)
                {
                    strResult += paiJiaArray[i].sBuilder + ",";
                }
                strResult = strResult.Substring(0, strResult.Length - 1);
            }
            return strResult;
        }

        private void generatefollowByMaxModel(MaxArchiveRegisNo maxModel, string strClassNo, int ArchivesCount, string status, string register_number_f, string register_number_b)
        {
            //add by 周林,date:2016.12.13  
            int nB2 = 1 + strClassNo[0] - 'A';
            string strB2 = "";
            if (nB2 < 10)
            {
                strB2 = "0" + nB2.ToString();
            }
            else
            {
                strB2 = nB2.ToString();
            }
            if (status.Trim() == "5")
            {//编号
                string strStart = maxModel.maxRegisNo.Trim();
                strStart = strStart.Substring(2, strStart.Length - 2);
                string strEnd = "";
                suanhao(ref strStart, ref strEnd, ArchivesCount);
                TempData["register_number_f"] = strB2 + strStart;
                TempData["register_number_b"] = strB2 + strEnd;

                strStart = maxModel.maxArchiveNo.Trim();
                int index = strStart.IndexOf('-');
                strStart = strStart.Substring(index + 1, strStart.Length - index - 1);
                strEnd = "";
                suanhao(ref strStart, ref strEnd, ArchivesCount);
                TempData["archive_number_f"] = strClassNo + "-" + strStart;
                TempData["archive_number_b"] = strClassNo + "-" + strEnd;
            }
            if (status == "6")
            {//重新编号,总登记号只需修改前两位
                string strStart = register_number_f.Trim();
                strStart = strStart.Substring(2, strStart.Length - 2);
                string strEnd = register_number_b.Trim();
                strEnd = strEnd.Substring(2, strEnd.Length - 2);
                TempData["register_number_f"] = strB2 + strStart;
                TempData["register_number_b"] = strB2 + strEnd;

                strStart = maxModel.maxArchiveNo.Trim();
                int index = strStart.IndexOf('-');
                strStart = strStart.Substring(index + 1, strStart.Length - index - 1);
                strEnd = "";
                suanhao(ref strStart, ref strEnd, ArchivesCount);
                TempData["archive_number_f"] = strClassNo + "-" + strStart;
                TempData["archive_number_b"] = strClassNo + "-" + strEnd;

            }
        }
        private bool isStringValid(string detail)
        {//判断排架号字符串输入格式是否合法，false为不合法
            for (int i = 0; i < detail.Length; i++)
            {
                if (i == detail.Length - 1)//为字符串的最后一位




                {
                    if (detail[i] < '0' || detail[i] > '9')//最后一位不是数字




                    {
                        return false;
                    }
                }
                else
                {
                    char c = detail[i];
                    if ((c < '0' || c > '9') && c != '-' && c != ',')
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private string[] CacuPaijiaNo(string PaijiaNo, int count, int indexFlag)
        {
            string str = "";
            bool flag = false;
            string[] strPaijiaEnum = null;
            if (indexFlag == 0)//有空柜子
            {
                for (int i = 0; i < PaijiaNo.Length; i++)
                {
                    if (PaijiaNo[i] == ',')
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    try
                    {
                        string[] PaiJia = PaijiaNo.Split(',');
                        for (int i = 0; i < PaiJia.Length; i++)
                        {
                            int index = PaiJia[i].IndexOf('-');//modify by niutianbo,date:20090806
                            if (index == -1)
                                str += PaiJia[i] + ",";
                            else
                            {
                                string[] strPaijia = null;
                                strPaijia = PaiJia[i].Split('-');
                                foreach (string st in strPaijia)
                                {
                                    if (st.Length != 8)
                                    {
                                        strPaijiaEnum[0] = "a";
                                        strPaijiaEnum[1] = st;
                                        return strPaijiaEnum;
                                    }
                                }

                                int cnt = Int32.Parse(strPaijia[1]) - Int32.Parse(strPaijia[0]) + 1;
                                string temp = strPaijia[0];
                                for (int j = 0; j < cnt; j++)
                                {
                                    str += temp + ",";
                                    suanhao2(ref temp);
                                }
                            }
                        }
                        strPaijiaEnum = str.Substring(0, str.Length - 1).Split(',');
                    }
                    catch
                    { }
                }
                else
                {
                    if (count == 1)//modify by niutianbo,date:20090806
                    {
                        if (PaijiaNo.Length != 8)
                        {
                            strPaijiaEnum[0] = "b";

                            return strPaijiaEnum;


                        }
                        strPaijiaEnum = new string[1];
                        strPaijiaEnum[0] = PaijiaNo;
                    }
                    else
                    {
                        string[] strPaijia = PaijiaNo.Split('-');
                        int cnt = Int32.Parse(strPaijia[1]) - Int32.Parse(strPaijia[0]) + 1;
                        string temp = strPaijia[0];
                        for (int j = 0; j < cnt; j++)
                        {
                            str += temp + ",";
                            suanhao2(ref temp);
                        }
                        strPaijiaEnum = str.Substring(0, str.Length - 1).Split(',');
                    }
                }
            }
            else//没有空柜子
            {
                strPaijiaEnum = new string[count];
                for (int i = 0; i < count; i++)
                {
                    strPaijiaEnum[i] = "00000000";
                }
            }
            if (strPaijiaEnum.Length != count)
            {
                strPaijiaEnum[0] = "c";

                return strPaijiaEnum;

            }
            foreach (string strPE in strPaijiaEnum)
            {
                if (strPE.Length != 8)
                {
                    strPaijiaEnum[0] = "d";
                    strPaijiaEnum[0] = strPE;
                    return strPaijiaEnum;

                }
            }
            return strPaijiaEnum;
        }
        private string[] CacuThickInfo(string thick, int count)
        {
            string[] thickInfo = null;
            int index = thick.IndexOf(',');
            if (index == -1)
            {
                if (count == 1)
                {
                    thickInfo = new string[1];
                    thickInfo[0] = thick;
                }
                else
                {
                    Response.Write("<script language=javascript>alert('对应厚度数目与案卷数目不一致');</script>"); return null;
                }
            }
            else
            {
                thickInfo = thick.Split(',');
                if (thickInfo.Length != count)
                {
                    Response.Write("<script language=javascript>alert('对应厚度数目与案卷数目不一致');</script>"); return null;
                }
            }
            return thickInfo;
        }
        private bool updateMaxArchiveRegisPaijiaNo(string strClassno, string strMaxArchiveNo, string strMaxRegiterNo, string[] strPjNoEnum, string[] strThickInfoEnum)
        {
            //更新最大档号与登记号
            bool isClassNoExis = false;
            string strLast6Bit = strMaxRegiterNo.Substring(2, strMaxRegiterNo.Length - 2);
            var Maxdel = from c in db.MaxArchiveRegisNo
                         where c.ID > 0
                         select c;
            foreach (var item in Maxdel)
            {
               
                if (item.mainCategoryID.Trim()==strClassno.Trim())
                {
                    isClassNoExis = true;
                    item.mainCategoryID = strClassno.Trim();
                    item.maxArchiveNo = strMaxArchiveNo;
                    item.maxRegisNo = strMaxRegiterNo;

                }
                else
                {
                   
                    string curRegisNo = item.maxRegisNo;
                    item.maxRegisNo = curRegisNo.Substring(0, 2) + strLast6Bit;

                }
                
                   db.Entry(item).State = EntityState.Modified;
              
              
            }
          
              db.SaveChanges();
            
            
         
            if (isClassNoExis == false)
            {
                MaxArchiveRegisNo maxModel = new MaxArchiveRegisNo();
                var maxID = from b in db.MaxArchiveRegisNo
                            orderby b.ID descending
                            select b;
                int max = maxID.First().ID + 1;
                maxModel.ID = max;
                maxModel.mainCategoryID = strClassno.ToString();
                maxModel.maxArchiveNo = strMaxArchiveNo;
                maxModel.maxRegisNo = strMaxRegiterNo;
                try
                {
                    db.MaxArchiveRegisNo.Add(maxModel);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                    return false;
                }
            }
            //更新排架号



            string prevCabinetNo = strPjNoEnum[0];//在当前排架号中获得当前柜号
            int remainWidth = 0;
            string CabinetNo = prevCabinetNo.Substring(0, 5);
            var cabModel = from d in db.CabinetInfo
                           where d.cabinetNo == CabinetNo
                           select d;
            if (cabModel.Count() != 0)
            {
                int rewidth = Convert.ToInt32(cabModel.First().remainWidth);
                remainWidth = rewidth;
            }
            if (strPjNoEnum.Length == 1)//只有一个排架号
            {
                remainWidth -= Int32.Parse(strThickInfoEnum[0]);
                if (cabModel.Count() != 0)
                {
                    cabModel.First().remainWidth = remainWidth;
                    cabModel.First().startPaijiaNo = strPjNoEnum[0];
                    try
                    {

                        db.Entry(cabModel.First()).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    //这里理论上是不会运行到这里的，
                    //因为当用户在界面中输入新的柜号时，程序会判断这是否为新的柜子，若是则将其添加到柜子信息表中
                    return false;
                }
                return true;
            }
    //有多个排架号
    bool isSameCabinet = true;//同个柜子是否第一次获得model?true表示是第一次，false则否
            for (int i = 0; i<strPjNoEnum.Length; i++)
            {
                string curCabinetNo = strPjNoEnum[i];//在当前排架号中获得当前柜号




                if (prevCabinetNo.Substring(0, 5) != curCabinetNo.Substring(0, 5) || i == strPjNoEnum.Length - 1)
                //当前柜子与上个柜子不是同个柜子或已到最后一个排架号
                {
                    if (i != 1)//获得上个柜子的model
                        if (cabModel.Count() != 0)//柜子信息更新
                        {
                            if (i == strPjNoEnum.Length - 1)//是排架号的最后一个




                            {
                                if (prevCabinetNo.Substring(0, 6) != curCabinetNo.Substring(0, 6) && prevCabinetNo.Substring(0, 5) == curCabinetNo.Substring(0, 5))
                                {//此时为同个柜子的不同层



                                    cabModel.First().remainWidth = cabModel.First().width - Int32.Parse(strThickInfoEnum[i]);
                                    cabModel.First().startPaijiaNo = strPjNoEnum[i];
                                }
                                else
                                {//此时为不同的柜子或同个柜子的同个层次
                                    if (prevCabinetNo.Substring(0, 6) == curCabinetNo.Substring(0, 6))
                                    {//是同个柜子的同个层次
                                        cabModel.First().remainWidth = remainWidth - Int32.Parse(strThickInfoEnum[i]);
                                        cabModel.First().startPaijiaNo = strPjNoEnum[i];
                                    }
                                    else
                                    {//是最后一个且与前个是不同的柜子,此时应更新上个柜子的信息与这个柜子的信息
                                        cabModel.First().remainWidth = remainWidth;
                                        cabModel.First().startPaijiaNo = strPjNoEnum[i - 1];
                                        db.Entry(cabModel.First()).State = EntityState.Modified;
                                        //db.SaveChanges();

                                        //更新下个柜子的信息
                                        string curcab = curCabinetNo.Substring(0, 5);
                                        var cab = from g in db.CabinetInfo
                                           where g.cabinetNo == curcab
                                            select g;


                                        if (cab.Count() != 0)
                                        {
                                            cab.First().remainWidth = cabModel.First().remainWidth - Int32.Parse(strThickInfoEnum[i]);
                                            cab.First().startPaijiaNo = strPjNoEnum[i];
                                        }
                                    }
                                }
                            }
                            else//后一个与前个为不同柜子




                            {
                                cabModel.First().remainWidth = remainWidth;
                                cabModel.First().startPaijiaNo = strPjNoEnum[i - 1];
                            }
                            try
                            {
                                isSameCabinet = true;
                                db.Entry(cabModel.First()).State = EntityState.Modified;
                                //db.SaveChanges();
                                if (i != strPjNoEnum.Length - 1)//不是最后一个排架号
                                {//对下一个柜子的信息进行处理,下个柜子第一次获得model
                                    isSameCabinet = false;
                                    string curcab = curCabinetNo.Substring(0, 5);
                         var cab = from g in db.CabinetInfo
                         where g.cabinetNo == curcab
                         select g;

                                    if (cab.Count() != 0)
                                        remainWidth = Convert.ToInt32(cab.First().remainWidth - Int32.Parse(strThickInfoEnum[i]));
                                }
                            }
                            catch (Exception ex)
                            {

                                return false;
                            }
                        }
                        else
                        {
                            //这里理论上是不会运行到这里的，
                            //因为当用户在界面中输入新的柜号时，程序会判断这是否为新的柜子，若是则将其添加到柜子信息表中
                            return false;
                        }
                }
                else//是同个柜子
                {
                    if (isSameCabinet == true)//第一次获得model
                    {
                        string curcab = curCabinetNo.Substring(0, 5);
                        var cag = from g in db.CabinetInfo
                                  where g.cabinetNo == curcab
                                  select g;

                        remainWidth = Convert.ToInt32(cag.First().remainWidth);
                        isSameCabinet = false;
                    }
                    if (cabModel.Count() != 0)
                    {
                        string before6bit = cabModel.First().startPaijiaNo.Trim();
                        before6bit = before6bit.Substring(0, 6);//获得当前Model中排架号的前六位
                        if (strPjNoEnum[i].Substring(0, 6) == before6bit)
                        {
                            remainWidth -= Int32.Parse(strThickInfoEnum[i]);
                        }
                        else//如果在同个柜子中前六位不一样，则意味着换层
                        {//此时应用柜子的宽度减去此排架号的宽度
                            remainWidth = Convert.ToInt32(cabModel.First().width - Int32.Parse(strThickInfoEnum[i]));
                        }
                    }
                }
                prevCabinetNo = curCabinetNo;
            }
            return true;
        }
        public void updateArchivesDetail(long? GCId, string strSeqNo, string[] PaijiaNo, string[] regiterNoEnum, string[] archiveNoEnum, int count, string[] thickInfo)
        {
            //更新案卷信息
            for (int i = 0; i < count; i++)
            {
                var num = i + 1;
                var m = from a in db.ArchivesDetail
                        where a.SingleProjectId == GCId
                        where a.volNo == num
                        select a;
                ArchivesDetail model = m.First();
                //model.paijiaNo = PaijiaNo[i];
                //model.registrationNo = regiterNoEnum[i];
                //model.archivesNo = archiveNoEnum[i];
                //model.archiveThickness = Int32.Parse(thickInfo[i]);
                model.paperProjectSeqNo = Int32.Parse(strSeqNo);
                model.paijiaNo = PaijiaNo[i];
                model.registrationNo = regiterNoEnum[i];
                model.archivesNo = archiveNoEnum[i];
                model.archiveThickness = Int32.Parse(thickInfo[i]);

                var m1 = from a in db.FileInfo
                         where a.FileId == model.AnJuan_id
                         select a;
                for (int j = 0; j < m1.Count(); j++)
                {
                    //更新卷内表
                    var num1 = j + 1;
                    var m2 = from a in db.FileInfo
                             where a.FileId == model.AnJuan_id
                             where a.seqNo == num1
                             select a;
                    Models.FileInfo file = m2.First();
                    file.dengjihao = model.registrationNo;
                    file.archivesNo = model.archivesNo;
                    db.Entry(file).State = EntityState.Modified;
                }
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
        }


        public void insertArchivesDetail(string strSeqNo, string[] PaijiaNo, string[] regiterNoEnum, string[] archiveNoEnum, int count, string[] thickInfo)
        {
            for (int i = 0; i < count; i++)
            {
                ArchivesDetail model = new ArchivesDetail();
                model.volNo = i + 1;
                model.checkDate = DateTime.Parse("1753-01-01");
                model.indexDate = DateTime.Parse("1753-01-01");
                model.typerDate = DateTime.Parse("1753-01-01");
                model.jgDate = DateTime.Parse("1753-01-01");
                model.paperProjectSeqNo = Int32.Parse(strSeqNo);
                model.paijiaNo = PaijiaNo[i];
                model.registrationNo = regiterNoEnum[i];
                model.archivesNo = archiveNoEnum[i];
                model.archiveThickness = Int32.Parse(thickInfo[i]);
                model.archivesTitle = "";
                db.ArchivesDetail.Add(model);
                
            }
            db.SaveChanges();
        }
        public void updatePaperArchives(int id, string strClassNo, string[] PaijiaNo, string paperSeqNo, string startArchivesNo, string endArchivesNo, string startPaijiaNo, string endPaijiaNo, string startRegisterNo, string endRegisterNo,int count)
        {
            var paper = from a in db.PaperArchives
                        where a.projectID == id
                        select a;
            PaperArchives model = paper.First();
            if (model != null)
            {
                int index = strClassNo.IndexOf('.');
                string mainCategoryID = "";
                string subDictionaryID = "";
                string minorDictionaryID = "";
                if (index != -1)
                {
                    mainCategoryID = strClassNo.Substring(0, 1);
                    subDictionaryID = strClassNo.Substring(1, index - 1);
                    minorDictionaryID = strClassNo.Substring(index + 1, strClassNo.Length - index - 1);
                }
                else
                {
                    mainCategoryID = strClassNo.Substring(0, 1);
                    subDictionaryID = strClassNo.Substring(1, strClassNo.Length - 1);
                    minorDictionaryID = "0";
                }
                //model.projectID = id.ToString();
                model.mainCategoryID = mainCategoryID;
                model.subDictionaryID = subDictionaryID;
                model.minorDictionaryID = minorDictionaryID;
                model.archivesCount =count.ToString();
                model.paperProjectSeqNo = int.Parse(paperSeqNo);
                model.startArchiveNo = startArchivesNo;
                model.endArchiveNo = endArchivesNo;
                model.startPaijiaNo = startPaijiaNo;
                model.endPaijiaNo = endPaijiaNo;
                model.startRegisNo = startRegisterNo;
                model.endRegisNo = endRegisterNo;
                model.bianhaoTime = DateTime.Now;
                int LEN = PaijiaNo.Length - 1;
                model.paijiaRange = PaijiaNo[0] + "-" + PaijiaNo[LEN];
                //更新编号日志表
                BianHaoLog Log= new BianHaoLog();
                //var LogModel = from  a in db.BianHaoLog
                //               orderby a.ID descending
                //               select
                var log1 = from f in db.BianHaoLog
                           orderby f.ID descending
                           select f.ID;//找到最大的ID
                var UserID = User.Identity.GetUserId();
                var user = ab.AspNetUsers.Find(UserID);//获取当前用户
                Log.ID = log1.First() + 1;
                Log.ProjectNo = Convert.ToInt32(model.projectNo);
                Log.ProjectSeqNo = Convert.ToInt32(model.paperProjectSeqNo) ;                                                                             
                Log.ArchiveNoRange = startArchivesNo.Trim() + "-" + endArchivesNo.Trim();
                Log.RegistrationNoRange = startRegisterNo.Trim()+ "-" + endRegisterNo.Trim();
                Log.BianHaoTime = model.bianhaoTime;
                Log.IP = GetWebClientIp();//获取本机IP
                Log.UserName = user.UserName;
                Log.ArchiveCount = int.Parse(model.archivesCount);
                Log.InchDetail = model.InchCountDetail;
                db.BianHaoLog.Add(Log);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        public  void UpdateArchiveStatus(string strID, int status)
        {
            long ID = Int32.Parse(strID);
            var project = from a in db.ProjectInfo
                          where a.projectID == ID
                          select a;
            ProjectInfo model = project.First();
            var login = from b in db.BianHaoLog
                        orderby b.ID descending
                        select b;
            
            try
            {
                if (model != null)
                {
                    int temp =status;
                    BianHaoLog model1 = new BianHaoLog();
                    model1 = login.First();
                    model.status = temp.ToString();
                    login.First().ProjectName = model.projectName;
                    login.First().ProjectLocation = model.location;
                    login.First().DevelopmentOrginisation = model.developmentOrganization;
                    db.Entry(model1).State = EntityState.Modified;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
            catch (System.Exception err)
            {
                string e = err.Message;
                throw err;
            }

        }
    
        public struct paijia_s
        {
            public string strFlag;
            public string sBuilder;
        }
        private string calcuArchiveNo(string archiveNo, int ArchivesCount, bool flag)
        {
            int index = archiveNo.IndexOf('-');
            string strClsNo = archiveNo.Substring(0, index + 1);
            string strXuNo = archiveNo.Substring(index + 1, archiveNo.Length - index - 1);
            int length = strXuNo.Length;
            string dlength = "D" + length.ToString();
            int nXuNo = Int32.Parse(strXuNo);
            if (flag == true)//档号加上案卷数得新的档号
            {
                nXuNo = nXuNo + ArchivesCount;
                if (nXuNo >= Math.Pow(10.0, (double)length))
                {
                    length += 1;
                    dlength = "D" + length.ToString();
                }
            }
            else//档号减去案卷数得新的档号
            {
                nXuNo = nXuNo - ArchivesCount;
            }
            return strClsNo + nXuNo.ToString(dlength);
        }
        public static string GetLocalIP()
        {
           
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
          
            
        }
        public static string GetWebClientIp()
        {
            string userIP = "未获取用户IP";

            try
            {
                if (System.Web.HttpContext.Current == null
            || System.Web.HttpContext.Current.Request == null
            || System.Web.HttpContext.Current.Request.ServerVariables == null)
                    return "";

                string CustomerIP = "";

                //CDN加速后取到的IP   
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];


                if (!String.IsNullOrEmpty(CustomerIP))
                    return CustomerIP;

                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (CustomerIP == null)
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                }

                if (string.Compare(CustomerIP, "unknown", true) == 0)
                    return System.Web.HttpContext.Current.Request.UserHostAddress;
                return CustomerIP;
            }
            catch { }

            return userIP;
        }
    
    }
}

