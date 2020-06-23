using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using urban_archive.Models;
using Microsoft.Reporting.WebForms;
using System.Text.RegularExpressions;

namespace urban_archive.Controllers
{
    public class guanlikeTJController : Controller
    {
        private UrbanConEntities ab = new UrbanConEntities();
        private UrbanUsersEntities ac = new UrbanUsersEntities();
        private PlanArchiveEntities bb = new PlanArchiveEntities();
        private VideoArchiveEntities cb = new VideoArchiveEntities();
        private gxArchivesContainer gb = new gxArchivesContainer();
        private OfficeEntities db = new OfficeEntities();

        // GET: guanlikeTJ
        public ActionResult Archive_use_TJ(string action, string action1, string startdata,string enddata,string startdata1, string enddata1, string type = "PDF")
        {
           
             var User1 = from b in ac.AspNetUsers
                             //where b.DepartmentId == 1 && (b.UserName == "管理科高级" || b.UserName == "管理科借阅" || b.UserName == "借阅用户")
                         where b.DepartmentId == 1 && (b.RoleName == "高级用户" || b.RoleName == "借阅用户")
                         select b;

            //string user1 = User.Identity.Name;
            ViewBag.operator2 = new SelectList(User1, "UserName", "UserName");
            if (action == "查看统计结果")
            {
                if (startdata == null || startdata==""|| enddata==""|| enddata == null)
                {
                    return Content("<script >alert('输入日期格式不正确，请核查！');window.history.back();</script >");
                }
                LocalReport localReport = new LocalReport();
                string jingbanren = Request.Form["operator2"];
                DateTime DataFrom = DateTime.Parse(startdata);
                DateTime DataTo = DateTime.Parse(enddata);
            

                var ds_all = ab.BorrowRegistration.Where(ad => ad.borrowDate >= DataFrom).Where(ad => ad.borrowDate <= DataTo);
                var ds = ab.BorrowRegistration.Where(ad => ad.operator1.Contains(jingbanren)).Where(ad => ad.borrowDate >= DataFrom).Where(ad => ad.borrowDate <= DataTo);
                if (jingbanren=="")
                {
                    ds = ab.BorrowRegistration.Where(ad => ad.operator1==jingbanren).Where(ad => ad.borrowDate >= DataFrom).Where(ad => ad.borrowDate <= DataTo);
                }
               
                int chayue_count = 0;
                foreach (var s1 in ds)

                {

                    if (s1.archiveSerialNo != "" && s1 != null)
                    {
                        if (s1.archiveSerialNo.IndexOf(',') == -1)
                        {
                            chayue_count++;
                        }
                        else
                        {
                            string[] Str = s1.archiveSerialNo.Split(',');
                            chayue_count += Str.Length;
                        }

                    }


                }
                int count_all = ds_all.Count();
                var ds1 = ds.Where(ad => ad.application1 == true).ToList();
                int count1 = ds1.Count();
                var ds2 = ds.Where(ad => ad.application2 == true).ToList();
                int count2 = ds2.Count();
                var ds3 = ds.Where(ad => ad.application3 == true).ToList();
                int count3 = ds3.Count();
                var ds4 = ds.Where(ad => ad.application4 == true).ToList();
                int count4 = ds4.Count();
                var ds5 = ds.Where(ad => ad.application5 == true).ToList();
                int count5 = ds5.Count();
                var ds6 = ds.Where(ad => ad.application6 == true).ToList();
                int count6 = ds6.Count();

                var us1 = ds.Where(ad => ad.userEffects1 == true).ToList();
                int count7 = us1.Count();
                var us2 = ds.Where(ad => ad.userEffects2 == true).ToList();
                int count8 = us2.Count();
                var us3 = ds.Where(ad => ad.userEffects3 == true).ToList();
                int count9 = us3.Count();
                var us4 = ds.Where(ad => ad.userEffects4 == true).ToList();
                int count10 = us4.Count();
                var us5 = ds.Where(ad => ad.userEffects5 == true).ToList();
                int count11 = us5.Count();

                var go1 = ds.Where(ad => ad.goal1 == true).ToList();
                int count12 = go1.Count();
                var go2 = ds.Where(ad => ad.goal2 == true).ToList();
                int count13 = go2.Count();
                var go3 = ds.Where(ad => ad.goal3 == true).ToList();
                int count14 = go3.Count();
                var go4 = ds.Where(ad => ad.goal4 == true).ToList();
                int count15 = go4.Count();
                var go5 = ds.Where(ad => ad.goal5 == true).ToList();
                int count16 = go5.Count();
                var go6 = ds.Where(ad => ad.goal6 == true).ToList();
                int count17 = go6.Count();

                

                List<BorrowRegistration> list = ds.ToList();
                int count = list.Count();
                //int allconsultVolumeCount = 0;
                int allconsultFilePersonTime = 0;

                for (int i = 0; i < list.Count(); i++)
                {
                    //if (list[i].consultVolumeCount == null || list[i].consultVolumeCount == "")
                    //{
                    //    list[i].consultVolumeCount = "0";
                    //}
                    //allconsultVolumeCount += int.Parse(list[i].consultVolumeCount);

                    allconsultFilePersonTime += int.Parse(list[i].consultFilePersonTime);


                }
                var ds0 = list;

                var ds_1 = ab.vw_ImageAndBorrow.Where(ad => ad.imageTime >= DataFrom).Where(ad => ad.imageTime <= DataTo).Where(ad=>ad.operator1.Contains(jingbanren));
                if(jingbanren=="")
                {
                    ds_1 = ab.vw_ImageAndBorrow.Where(ad => ad.imageTime >= DataFrom).Where(ad => ad.imageTime <= DataTo).Where(ad => ad.operator1=="");
                }
                int count_1 = ds_1.Count();

                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/Archive_use_TJ.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("DangAnLiYongDengJi", ds0);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("count", count.ToString().Trim()));
                parameterList.Add(new ReportParameter("count_all", count_all.ToString().Trim()));
                parameterList.Add(new ReportParameter("count1", count1.ToString().Trim()));
                parameterList.Add(new ReportParameter("count2", count2.ToString().Trim()));
                parameterList.Add(new ReportParameter("count3", count3.ToString().Trim()));
                parameterList.Add(new ReportParameter("count4", count4.ToString().Trim()));
                parameterList.Add(new ReportParameter("count5", count5.ToString().Trim()));
                parameterList.Add(new ReportParameter("count6", count6.ToString().Trim()));
                parameterList.Add(new ReportParameter("count7", count7.ToString().Trim()));
                parameterList.Add(new ReportParameter("count8", count8.ToString().Trim()));
                parameterList.Add(new ReportParameter("count9", count9.ToString().Trim()));
                parameterList.Add(new ReportParameter("count10", count10.ToString().Trim()));
                parameterList.Add(new ReportParameter("count11", count11.ToString().Trim()));
                parameterList.Add(new ReportParameter("count12", count12.ToString().Trim()));
                parameterList.Add(new ReportParameter("count13", count13.ToString().Trim()));
                parameterList.Add(new ReportParameter("count14", count14.ToString().Trim()));
                parameterList.Add(new ReportParameter("count15", count15.ToString().Trim()));
                parameterList.Add(new ReportParameter("count16", count16.ToString().Trim()));
                parameterList.Add(new ReportParameter("count17", count17.ToString().Trim()));
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("count_1", count_1.ToString().Trim()));
                parameterList.Add(new ReportParameter("jingbanren", jingbanren.ToString().Trim()));
                parameterList.Add(new ReportParameter("chayue_count", chayue_count.ToString().Trim()));
                parameterList.Add(new ReportParameter("allconsultFilePersonTime", allconsultFilePersonTime.ToString().Trim()));

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
            if (action1 == "查看统计结果")
            {
                 if (startdata1 == null || startdata1 == "" || enddata1 == "" || enddata1 == null)
                    {
                    return Content("<script >alert('输入日期格式不正确，请核查！');window.history.back();</script >");
                }
                LocalReport localReport = new LocalReport();
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata1"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata1"]);


                var ds_all = ab.BorrowRegistration.Where(ad => ad.borrowDate >= DataFrom).Where(ad => ad.borrowDate <= DataTo);

                int count_1all = ds_all.Count();
                //foreach (var s1 in ds_all)
                //{

                //    if (s1.archiveSerialNo != "" && s1 != null && s1.archiveSerialNo != null)
                //    {
                //        if (s1.archiveSerialNo.IndexOf(',') == -1)
                //        {
                //            chayue_count_all++;
                //        }
                //        else
                //        {
                //            string[] Str = s1.archiveSerialNo.Split(',');
                //            chayue_count_all += Str.Length;
                //        }

                //    }
                //}
                //List<BorrowRegistration> list_all = ds_all.ToList();
                //int allconsultFilePersonTime_all = 0;
                //for (int i = 0; i < list_all.Count(); i++)
                //{

                //    allconsultFilePersonTime_all += int.Parse(list_all[i].consultFilePersonTime);


                //}
                //var ds_gao1 = ab.vw_ImageAndBorrow.Where(ad => ad.operator1.Contains("管理科高级")).Where(ad => ad.imageTime >= DataFrom).Where(ad => ad.imageTime <= DataTo).ToArray();
                var ds_gao1 = ab.vw_ImageAndBorrow.Where(ad => ad.operator1.Contains("管理科高级")).Where(ad => ad.imageTime >= DataFrom).Where(ad => ad.imageTime <= DataTo);

                //var ds_gao1 = from a in ab.vw_ImageAndBorrow
                //              where a.imageTime >= DataFrom
                //              where a.imageTime <= DataTo
                //              select a.realuserID;


                int chayue_count_gao = 0;
                long? flag = 0;
                foreach (var s in ds_gao1)

                {
                    if (flag != s.realuserID)
                    {
                        flag = s.realuserID;
                    }
                    else
                    {
                        continue;
                    }
                    var n = from a in ab.BorrowRegistration
                            where a.ID == s.realuserID
                            select a;
                    var s1 = n.First();
                    if (s1.archiveSerialNo != "" && s1 != null)
                    {
                        if (s1.archiveSerialNo.IndexOf(',') == -1)
                        {
                            chayue_count_gao++;
                        }
                        else
                        {
                            string[] Str = s1.archiveSerialNo.Split(',');
                            chayue_count_gao += Str.Length;
                        }

                    }

                }
                var ds_gao = ab.BorrowRegistration.Where(ad => ad.operator1.Contains("管理科高级")).Where(ad => ad.borrowDate >= DataFrom).Where(ad => ad.borrowDate <= DataTo);

                int count_11 = ds_gao.Count();

                //foreach (var s1 in ds_gao)

                //{
                //    if (s1.archiveSerialNo != "" && s1 != null)
                //    {
                //        if (s1.archiveSerialNo.IndexOf(',') == -1)
                //        {
                //            chayue_count_gao++;
                //        }
                //        else
                //        {
                //            string[] Str = s1.archiveSerialNo.Split(',');
                //            chayue_count_gao += Str.Length;
                //        }

                //    }
                //}
                //List<BorrowRegistration> list_gao = ds_gao.ToList();
                //int allconsultFilePersonTime_gao = 0;
                //for (int i = 0; i < list_gao.Count(); i++)
                //{

                //    allconsultFilePersonTime_gao += int.Parse(list_gao[i].consultFilePersonTime);


                //}
                var ds_1 = ab.vw_ImageAndBorrow.Where(ad => ad.imageTime >= DataFrom).Where(ad => ad.imageTime <= DataTo).Where(ad => ad.operator1.Contains("管理科高级"));
                int count_1 = ds_1.Count();


                var ds_gj1 = ab.vw_ImageAndBorrow.Where(ad => ad.operator1.Contains("管理科借阅")).Where(ad => ad.imageTime >= DataFrom).Where(ad => ad.imageTime <= DataTo);
                int chayue_count_gj = 0;
                long? flag1 = 0;
                foreach (var s in ds_gj1)

                {
                    if (flag1 != s.realuserID)
                    {
                        flag1 = s.realuserID;
                    }
                    else
                    {
                        continue;
                    }
                    var n = from a in ab.BorrowRegistration
                            where a.ID == s.realuserID
                            select a;
                    var s1 = n.First();
                    if (s1.archiveSerialNo != "" && s1 != null)
                    {
                        if (s1.archiveSerialNo.IndexOf(',') == -1)
                        {
                            chayue_count_gj++;
                        }
                        else
                        {
                            string[] Str = s1.archiveSerialNo.Split(',');
                            chayue_count_gj += Str.Length;
                        }

                    }

                }

                var ds_gj = ab.BorrowRegistration.Where(ad => ad.operator1.Contains("管理科借阅")).Where(ad => ad.borrowDate >= DataFrom).Where(ad => ad.borrowDate <= DataTo);
                int count_12 = ds_gj.Count();


                //foreach (var s1 in ds_gj)
                //{

                //    if (s1.archiveSerialNo != "" && s1 != null)
                //    {
                //        if (s1.archiveSerialNo.IndexOf(',') == -1)
                //        {
                //            chayue_count_gj++;
                //        }
                //        else
                //        {
                //            string[] Str = s1.archiveSerialNo.Split(',');
                //            chayue_count_gj += Str.Length;
                //        }

                //    }

                //}
                //List<BorrowRegistration> list_gj = ds_gj.ToList();
                //int allconsultFilePersonTime_gj = 0;
                //for (int i = 0; i < list_gj.Count(); i++)
                //{

                //    allconsultFilePersonTime_gj += int.Parse(list_gj[i].consultFilePersonTime);


                //}
                var ds_2 = ab.vw_ImageAndBorrow.Where(ad => ad.imageTime >= DataFrom).Where(ad => ad.imageTime <= DataTo).Where(ad => ad.operator1.Contains("管理科借阅"));
                int count_2 = ds_2.Count();



                var ds_user1 = ab.vw_ImageAndBorrow.Where(ad => ad.operator1.Contains("借阅用户")).Where(ad => ad.imageTime >= DataFrom).Where(ad => ad.imageTime <= DataTo);
                int chayue_count_user = 0;
                long? flag2 = 0;
                foreach (var s in ds_user1)

                {
                    if (flag2 != s.realuserID)
                    {
                        flag2 = s.realuserID;
                    }
                    else
                    {
                        continue;
                    }
                    var n = from a in ab.BorrowRegistration
                            where a.ID == s.realuserID
                            select a;
                    var s1 = n.First();
                    if (s1.archiveSerialNo != "" && s1 != null)
                    {
                        if (s1.archiveSerialNo.IndexOf(',') == -1)
                        {
                            chayue_count_user++;
                        }
                        else
                        {
                            string[] Str = s1.archiveSerialNo.Split(',');
                            chayue_count_user += Str.Length;
                        }

                    }

                }

                var ds_user = ab.BorrowRegistration.Where(ad => ad.operator1.Contains("借阅用户")).Where(ad => ad.borrowDate >= DataFrom).Where(ad => ad.borrowDate <= DataTo);
                int count_13 = ds_user.Count();


                //foreach (var s1 in ds_user)

                //{
                //    if (s1.archiveSerialNo != "" && s1 != null)
                //    {
                //        if (s1.archiveSerialNo.IndexOf(',') == -1)
                //        {
                //            chayue_count_user++;
                //        }
                //        else
                //        {
                //            string[] Str = s1.archiveSerialNo.Split(',');
                //            chayue_count_user += Str.Length;
                //        }

                //    }

                //}
                //List<BorrowRegistration> list_user = ds_user.ToList();
                //int allconsultFilePersonTime_user = 0;
                //for (int i = 0; i < list_user.Count(); i++)
                //{

                //    allconsultFilePersonTime_user += int.Parse(list_user[i].consultFilePersonTime);


                //}
                var ds_3 = ab.vw_ImageAndBorrow.Where(ad => ad.imageTime >= DataFrom).Where(ad => ad.imageTime <= DataTo).Where(ad => ad.operator1.Contains("借阅用户"));
                int count_3 = ds_3.Count();


                var ds_empty1 = ab.BindUserAndImageDown.Where(ad => ad.imageTime >= DataFrom).Where(ad => ad.imageTime <= DataTo)/*.Where((x, i) => ab.vw_ImageAndBorrow.ToList().FindIndex(z => z.realuserID == x.realuserID) == i)*/;
                int chayue_count_empty = 0;


                string archivesNo = "";
                foreach (var s in ds_empty1)

                {
                    if (archivesNo.Trim() != s.archivesNo.Trim())
                    {
                        archivesNo = s.archivesNo.Trim();
                        chayue_count_empty++;
                    }
                    else
                    {
                        continue;
                    }

                    //    var n = from a in ab.BorrowRegistration
                    //            where a.ID == s.realuserID
                    //            select a;
                    //    var s1 = n.First();
                    //    if (s1.archiveSerialNo != "" && s1 != null)
                    //    {
                    //        if (s1.archiveSerialNo.IndexOf(',') == -1)
                    //        {
                    //            chayue_count_empty++;
                    //        }
                    //        else
                    //        {
                    //            string[] Str = s1.archiveSerialNo.Split(',');
                    //            chayue_count_empty += Str.Length;
                    //        }

                    //    }

                }

                var ds_empty = ab.BorrowRegistration.Where(ad => ad.operator1 == "").Where(ad => ad.borrowDate >= DataFrom).Where(ad => ad.borrowDate <= DataTo);
                var ds_empty2 = ab.BorrowRegistration.Where(ad => ad.operator1 == null).Where(ad => ad.borrowDate >= DataFrom).Where(ad => ad.borrowDate <= DataTo);
                int count_14 = ds_empty.Count() + ds_empty2.Count();
 
                //foreach (var s1 in ds_empty)
                //{

                //    if (s1.archiveSerialNo != "" && s1 != null)
                //    {
                //        if (s1.archiveSerialNo.IndexOf(',') == -1)
                //        {
                //            chayue_count_empty++;
                //        }
                //        else
                //        {
                //            string[] Str = s1.archiveSerialNo.Split(',');
                //            chayue_count_empty += Str.Length;
                //        }

                //    }

                //}
                //List<BorrowRegistration> list_empty = ds_empty.ToList();
                //int allconsultFilePersonTime_empty = 0;
                //for (int i = 0; i < list_empty.Count(); i++)
                //{

                //    allconsultFilePersonTime_empty += int.Parse(list_empty[i].consultFilePersonTime);


                //}
                var ds_4 = ab.BindUserAndImageDown.Where(ad => ad.imageTime >= DataFrom).Where(ad => ad.imageTime <= DataTo);
                int count_4 = ds_4.Count();

                int count_all = count_1 + count_2 + count_3 + count_4;

                int chayue_count_all = chayue_count_gj + chayue_count_gao + chayue_count_user + chayue_count_empty;



                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/Archive_use_TJ0.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("DataSet1", ds_all);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("chayue_count_all", chayue_count_all.ToString().Trim()));
                parameterList.Add(new ReportParameter("count_11", count_11.ToString().Trim()));
                parameterList.Add(new ReportParameter("chayue_count_gao", chayue_count_gao.ToString().Trim()));
                parameterList.Add(new ReportParameter("count_12", count_12.ToString().Trim()));
                parameterList.Add(new ReportParameter("chayue_count_gj", chayue_count_gj.ToString().Trim()));
                parameterList.Add(new ReportParameter("count_13", count_13.ToString().Trim()));
                parameterList.Add(new ReportParameter("chayue_count_user", chayue_count_user.ToString().Trim()));
                parameterList.Add(new ReportParameter("count_14", count_14.ToString().Trim()));
                parameterList.Add(new ReportParameter("chayue_count_empty", chayue_count_empty.ToString().Trim()));
                parameterList.Add(new ReportParameter("count_1all", count_1all.ToString().Trim()));
                parameterList.Add(new ReportParameter("count_1", count_1.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("count_2", count_2.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("count_3", count_3.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("count_4", count_4.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("count_all", count_all.ToString().Split(' ')[0].Trim()));


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
            return View();
        }

        public ActionResult archives_use_amount_TJ(string action, string type = "PDF")
        {
            var User1 = from b in ac.AspNetUsers
                        where b.DepartmentId == 1 
                        select b;
            //string user1 = User.Identity.Name;
            ViewBag.operator2 = new SelectList(User1, "UserName", "UserName");
            if (action == "查看统计结果")
            {
                LocalReport localReport = new LocalReport();
                string jingbanren = Request.Form["operator2"]; 
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                var ds0 = ab.BorrowRegistration.Where(ad => ad.operator1== jingbanren);
                var ds = ds0.Where(ad => ad.borrowDate >= DataFrom).Where(ad => ad.borrowDate <= DataTo);
                //List<BorrowRegistration> list = ds.ToList();
                //int count= list.Count();
                int count=0;
                foreach (var s1 in ds)

                {
                    
                    if (s1.archiveSerialNo != "" && s1 != null)
                    {
                        if(s1.archiveSerialNo.IndexOf(',')==-1)
                        {
                            count++;
                        }
                        else
                        {
                            string[] Str = s1.archiveSerialNo.Split(',');
                            count += Str.Length;
                        }

                    }


                }
                //for (int i = 0; i < list.Count(); i++)
                //{
                //    if (list[i].application1 != null)
                //        list[i].application1 = "123";
                //    string name =list[i].application1
                //}

                //var ds2 = list;
                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/archives_use_amount_TJ.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("archives_use_amount_TJ", ds);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("count", count.ToString().Trim()));
                parameterList.Add(new ReportParameter("jingbanren", jingbanren.ToString().Trim()));

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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult guancang_TJ(string action, string type = "PDF")
        {
            

                LocalReport localReport = new LocalReport();
                //竣工档案
                var ds = ab.vw_projectList.Where(ad => ad.status=="7");
                int jungongProCnt = ds.Count();   
                //竣工总工程数

                var ds1 = ab.vw_passList.Where(ad => ad.status == "7");
                //List<vw_passList> list0 = ds1.ToList();
                int jungongArchCnt = ds1.Count();
            //    for (int i = 0; i < list0.Count(); i++)
            //{
            //    if (list0[i].archivesCount != null)
            //    {
            //        jungongArchCnt += int.Parse(list0[i].archivesCount);   //竣工总案卷数
            //    }

            //}
 
                //规划档案
                var guihua = bb.PlanProject.Where(ad => ad.status == "RK");
                int guihuaArchCnt = guihua.Count();   //规划总案卷数    

                var box = from a in guihua
                          group a by a.fileNo.Trim();
                int guihuaProCnt = box.Count();
                //var box1 = from a in guihua
                //           group a by a.boxNo
                //      into c
                //      select new
                //      {
                         
                //          cnt = c.Count(),
                         

                //      };

                //声像档案
                var shengxiang = cb.VideoArchives.Where(ad => ad.videoStatus == "4"); 
                int shengxiangProCnt = shengxiang.Count();   //声像总工程数
                List<VideoArchives> list4 = shengxiang.ToList();
                int videoArchCnt = 0;
                int photoArchCnt = 0;
                for (int i = 0; i < list4.Count(); i++)
                {
                if (list4[i].videoCassetteBoxCount!=null&& list4[i].videoCassetteBoxCount!=""&& list4[i].photoBoxCount!=null&& list4[i].photoBoxCount!="")
                {
                    videoArchCnt += int.Parse(list4[i].videoCassetteBoxCount);   //视频案卷数
                    photoArchCnt += int.Parse(list4[i].photoBoxCount);      //照片案卷数
                }

                   
                }
                //管线档案

                var gx = gb.gxProjectInfo.Where(ad => ad.status == "7");
                int gxProCnt = gx.Count();   //管线总工程数                
                var gxbox = gb.vw_gxArchiveList.Where(ad => ad.status == "7");
                List<vw_gxArchiveList> list1 = gxbox.ToList();
                int gxArchCnt = 0;
                for (int i = 0; i < list1.Count(); i++)
                {
                    if (list1[i].archivesCount != null)
                    {
                        gxArchCnt += int.Parse(list1[i].archivesCount);   //管线总案卷数
                    }

                }
                //道路档案
                var daolu = db.OtherArchives.Where(ad => ad.classTypeID == 2);
                var daolu_que = daolu.Where(ad => ad.applyUnit.Contains("缺"));
            //var daolu_Q = daolu.Where(ad => ad.volNo.Contains("Q"));
            //var daolu_que_Q = daolu_que.Where(ad => ad.volNo.Contains("Q"));
            //int daoluArchCnt = daolu.Count()- daolu_que.Count()- daolu_Q.Count()+ daolu_que_Q.Count();
            int daoluArchCnt = daolu.Count() - daolu_que.Count();//不用这么麻烦，缺的已经包含Q的，故减去缺的即可
            //int daoluArchCnt = daolu.Count();   //道路总案卷数
            //List<OtherArchives> list1 = daolu.ToList();
            //for (int i = 0; i < list1.Count(); i++)
            //{
            //    if (list1[i].volNo.Length>=4) 
            //    {
            //       list1[i].volNo = list1[i].volNo.Substring(0, 4).Trim();
            //     }
            //}            
            //var roadbox = from a in list1
            //              group a by a.volNo;
            //var roadProCnt = roadbox.Count();//道路总工程数    
            //var roadProCnt = 753;//道路总工程数    
            //    int daoluArchCnt = 18884;   //道路总案卷数




            //分类档案
            var fenlei = db.OtherArchives.Where(ad => ad.classTypeID == 3);
                int fenleiArchCnt = fenlei.Count();   //分类总案卷数                
                //var fenleibox = from a in fenlei
                //                group a by a.archiveNo.Trim();
                //var fenleiProCnt = fenleibox.Count();//分类总工程数    
            //图纸档案
                var tuzhi = ab.TuzhiArchives;
                int tuzhiArchCnt = tuzhi.Count();   //图纸总案卷数
                
                var tuzhibox = from a in tuzhi
                               group a by a.archiveTitle.Trim();
                var tuzhiProCnt = tuzhibox.Count();//图纸总工程数    

                //援川档案
                var yc = ab.YuanChuanProjectInfo;
                int ycProCnt = yc.Count();   //援川总工程数                
                var ycbox = ab.YuanChuanArchivesDetail;
                int ycArchCnt = ycbox.Count();  //援川总案卷数
                //东部档案
                var east = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东"));
                int eastArchCnt = east.Count();  //东部总案卷数
                List<OtherArchives> list_east = east.ToList();
                for (int i = 0; i < list_east.Count(); i++)
                {
                
                    list_east[i].licenceNo = list_east[i].licenceNo.Substring(0, 4).Trim();
               

                 }

                var east1 = from a in list_east
                            group a by a.licenceNo;
                var eastProCnt = east1.Count();//东部总工程数
                //执照档案
                var zhizhao = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1);
                int zhizhaoArchCnt = zhizhao.Count()- eastArchCnt;   //执照总案卷数(不包括东部)

                List<OtherArchives> list = zhizhao.ToList();

                for (int i = 0; i < list.Count(); i++)
                {
                Regex reg = new Regex(@"[\u4e00-\u9fa5]");//正则表达式
                
                    if (list[i].licenceNo != null && reg.IsMatch(list[i].licenceNo.Trim())==false) 
                    {
                       list[i].licenceNo = list[i].licenceNo.Substring(0, 10).Trim();
                     }
                    else
                    {
                       list[i].licenceNo = "0";
                     }
                }
                var zzbox = from a in list
                            group a by a.licenceNo;
            //var zhizhaoProCnt = -1;
            var zhizhaoProCnt = 0;
            zhizhaoProCnt = zzbox.Count() - 1;
            //if (eastArchCnt!=0)
            //    {
                    
            //        zhizhaoProCnt = zzbox.Count() - 1;
            //    }
            //    else
            //    {
            //        zhizhaoProCnt = zzbox.Count() ;

            //    }
                //征地档案
                var zhengdi = ab.zdArchive;
                int zdArchCnt = zhengdi.Count();        //征地总案卷数
                var zhengdi1 = from a in zhengdi
                               group a by a.zdwh;
                 var zhengdiProCnt = zhengdi1.Count()+50;//征地总工程数(zdwh NULL有51个)
                //全部档案
                //int allProCnt = jungongProCnt+ guihuaProCnt+ shengxiangProCnt+ gxProCnt+ zhizhaoProCnt+ roadProCnt+ fenleiProCnt+ tuzhiProCnt+ ycProCnt+ eastProCnt+ zhengdiProCnt;  
                int allArchCnt = jungongArchCnt+guihuaArchCnt+ videoArchCnt+ photoArchCnt+ gxArchCnt+ zhizhaoArchCnt+ daoluArchCnt+ fenleiArchCnt+ tuzhiArchCnt+ ycArchCnt+ eastArchCnt+ zdArchCnt;        
                
                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/guancang_TJ.rdlc");
                //ReportDataSource reportDataSource = new ReportDataSource("archives_use_amount_TJ", ds);
                //localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("jungongProCnt", jungongProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongArchCnt", jungongArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaArchCnt", guihuaArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaProCnt", guihuaProCnt.ToString().Trim()));  
                parameterList.Add(new ReportParameter("shengxiangProCnt", shengxiangProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("videoArchCnt", videoArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("photoArchCnt", photoArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("gxProCnt", gxProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("gxArchCnt", gxArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoProCnt", zhizhaoProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCnt", zhizhaoArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("daoluArchCnt", daoluArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCnt", fenleiArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("tuzhiProCnt", tuzhiProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("tuzhiArchCnt", tuzhiArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("ycProCnt", ycProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("ycArchCnt", ycArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastProCnt", eastProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCnt", eastArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhengdiProCnt", zhengdiProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCnt", zdArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCnt", allArchCnt.ToString().Trim()));


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
        public ActionResult guancang_TJby_paperProjectSeqNo(string action, string action1, string type = "PDF")
        {
            if (action == "查看统计结果")
            {
                LocalReport localReport = new LocalReport();
                var DataFrom = long.Parse(Request.Form["start_paperProjectSeqNo"]);
                var DataTo = long.Parse(Request.Form["end_paperProjectSeqNo"]);

                //竣工档案
                var ds1 = ab.vw_projectList.Where(ad => ad.status == "7").Where(ad => ad.paperProjectSeqNo >= DataFrom).Where(ad => ad.paperProjectSeqNo <= DataTo);
                int jungongProCnt = ds1.Count();   //竣工总工程数
                                                   //List<vw_ArchiveList> list0 = ds1.ToList();
                var ds = ab.vw_passList.Where(ad => ad.status == "7").Where(ad => ad.paperProjectSeqNo >= DataFrom).Where(ad => ad.paperProjectSeqNo <= DataTo);
                int jungongArchCnt = ds.Count();
                //for (int i = 0; i < list0.Count(); i++)
                //{
                //    if (list0[i].archivesCount != null)
                //    {
                //        jungongArchCnt += int.Parse(list0[i].archivesCount);   //竣工总案卷数
                //    }

                //}

                //规划档案
                var guihua = bb.PlanProject.Where(ad => ad.status == "RK");
                List<PlanProject> guihua1 = guihua.ToList();
                List<PlanProject> prolist = new List<PlanProject>();
                for (int i = 0; i < guihua1.Count(); i++)
                {
                  
                    if (long.Parse(guihua1[i].totalSeqNo.Substring(0,8))>= DataFrom&&long.Parse(guihua1[i].totalSeqNo.Substring(0, 8)) <= DataTo)
                    {
                        PlanProject prolist1 = new PlanProject();
                        prolist1.totalSeqNo = guihua1[i].totalSeqNo;
                        prolist1.fileNo = guihua1[i].fileNo;
                        prolist.Add(prolist1);
                    }
                }
                int guihuaArchCnt = prolist.Count();   //规划总案卷数    

                var box1 = from a in prolist
                           group a by a.fileNo.Trim();
                var guihuaProCnt = box1.Count(); //规划总工程数    
                                                //var box1 = from a in guihua
                                                //           group a by a.boxNo
                                                //      into c
                                                //      select new
                                                //      {

                //          cnt = c.Count(),


                //      };


                //管线档案

                var gxbox = gb.vw_gxArchiveList.Where(ad => ad.status == "7").Where(ad => ad.paperProjectSeqNo >= DataFrom).Where(ad => ad.paperProjectSeqNo <= DataTo);
                int gxProCnt = gxbox.Count();   //管线总工程数                

                List<vw_gxArchiveList> list1 = gxbox.ToList();
                var gxArchCnt = 0;
                for (int i = 0; i < list1.Count(); i++)
                {
                    if (list1[i].archivesCount != null)
                    {
                        gxArchCnt += int.Parse(list1[i].archivesCount);   //管线总案卷数
                    }

                }

                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/guancang_TJ _paperProjectSeqNo.rdlc");
                //ReportDataSource reportDataSource = new ReportDataSource("archives_use_amount_TJ", ds);
                //localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongProCnt", jungongProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongArchCnt", jungongArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaArchCnt", guihuaArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaProCnt", guihuaProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("gxProCnt", gxProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("gxArchCnt", gxArchCnt.ToString().Trim()));


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
            if (action1 == "查看统计结果")
            {
                LocalReport localReport = new LocalReport();
                var Year = Request.Form["year"];
                //道路档案
                var daolu = db.OtherArchives.Where(ad => ad.classTypeID == 2).Where(ad => ad.year == Year);
                var daolu_que = daolu.Where(ad => ad.applyUnit.Contains("缺"));
                //var daolu_Q = daolu.Where(ad => ad.volNo.Contains("Q"));
                //var daolu_que_Q = daolu_que.Where(ad => ad.volNo.Contains("Q"));
                //int daoluArchCnt = daolu.Count() - daolu_que.Count() - daolu_Q.Count() + daolu_que_Q.Count();
                int daoluArchCnt = daolu.Count() - daolu_que.Count();
                //东部档案
                var east = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.year.Contains(Year));
                int eastArchCnt = east.Count();  //东部总案卷数
                //执照档案
                var zhizhao = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains(Year));
                int zhizhaoArchCnt = zhizhao.Count() - eastArchCnt;   //执照总案卷数(不包括东部)
                List<OtherArchives> list = zhizhao.ToList();

                for (int i = 0; i < list.Count(); i++)
                {
                    Regex reg = new Regex(@"[\u4e00-\u9fa5]");//正则表达式

                    if (reg.IsMatch(list[i].licenceNo.Trim()) == false)
                    {
                        list[i].licenceNo = list[i].licenceNo.Substring(0, 10).Trim();
                    }
                    else
                    {
                        list[i].licenceNo = "0";
                    }
                }
                var list1 = list.Where(ad => ad.licenceNo != "0");
                var zzbox = from a in list1
                            group a by a.licenceNo;
                var zhizhaoProCnt = zzbox.Count();
                

                //规划档案
                var guihua = bb.PlanProject.Where(ad => ad.status == "RK").Where(ad => ad.yearNo == Year);
                int guihuaArchCnt = guihua.Count();   //规划总案卷数    
                var box = from a in guihua
                          group a by a.fileNo.Trim();
                int guihuaProCnt = box.Count();


                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/guancang_TJ _year.rdlc");
                //ReportDataSource reportDataSource = new ReportDataSource("archives_use_amount_TJ", ds);
                //localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("Year", Year.ToString().Trim()));
                parameterList.Add(new ReportParameter("daoluArchCnt", daoluArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoProCnt", zhizhaoProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCnt", zhizhaoArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaProCnt", guihuaProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaArchCnt", guihuaArchCnt.ToString().Trim()));


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
            if (action1 == "查看范围统计结果")
            {
                LocalReport localReport = new LocalReport();
                var YearStart = Request.Form["start_year"];
                var YearEnd = Request.Form["end_year"];
                //道路档案
                
                var daolu = db.OtherArchives.Where(ad => ad.classTypeID == 2).Where(ad => ad.year.CompareTo(YearStart) >= 0).Where(ad => ad.year.CompareTo(YearEnd) <= 0);
                var daolu_que = daolu.Where(ad => ad.applyUnit.Contains("缺"));
                int daoluArchCnt = daolu.Count() - daolu_que.Count();
                //东部档案
                var east = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.year.CompareTo(YearStart) >= 0).Where(ad => ad.year.CompareTo(YearEnd) <= 0);
                int eastArchCnt = east.Count();  //东部总案卷数
                //执照档案
                var zhizhao = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.year.CompareTo(YearStart) >= 0).Where(ad => ad.year.CompareTo(YearEnd) <= 0);
                int zhizhaoArchCnt = zhizhao.Count() - eastArchCnt;   //执照总案卷数(不包括东部)
                List<OtherArchives> list = zhizhao.ToList();

                for (int i = 0; i < list.Count(); i++)
                {
                    Regex reg = new Regex(@"[\u4e00-\u9fa5]");//正则表达式

                    if (reg.IsMatch(list[i].licenceNo.Trim()) == false)
                    {
                        list[i].licenceNo = list[i].licenceNo.Substring(0, 10).Trim();
                    }
                    else
                    {
                        list[i].licenceNo = "0";
                    }
                }
                var list1 = list.Where(ad => ad.licenceNo != "0");
                var zzbox = from a in list1
                            group a by a.licenceNo;
                var zhizhaoProCnt = zzbox.Count();

                //规划档案
                var guihua = bb.PlanProject.Where(ad => ad.status == "RK").Where(ad => ad.yearNo.CompareTo(YearStart) >= 0).Where(ad => ad.yearNo.CompareTo(YearEnd) <= 0);
                int guihuaArchCnt = guihua.Count();   //规划总案卷数    
                var box = from a in guihua
                          group a by a.fileNo.Trim();
                int guihuaProCnt = box.Count();
                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/guancang_TJ _year2.rdlc");
                //ReportDataSource reportDataSource = new ReportDataSource("archives_use_amount_TJ", ds);
                //localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("YearStart", YearStart.ToString().Trim()));
                parameterList.Add(new ReportParameter("YearEnd", YearEnd.ToString().Trim()));
                parameterList.Add(new ReportParameter("daoluArchCnt", daoluArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoProCnt", zhizhaoProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCnt", zhizhaoArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaProCnt", guihuaProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaArchCnt", guihuaArchCnt.ToString().Trim()));

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

            if (action == "取消")
            {
                return View("Home");
            }
            return View();

        }
        public ActionResult JG_GXarchiveRK_TJ(string action, string type = "PDF")
        {
            if (action == "查看统计结果")
            {
                LocalReport localReport = new LocalReport();
                if(Request.Form["startdata"]=="" || Request.Form["enddata"] == "")
                {
                    return Content("<script>alert('请输入起止时间！');history.back();</script>");
                }
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                //竣工档案
                var ds = ab.vw_projectList.Where(ad => ad.dateArchive>= DataFrom).Where(ad => ad.dateArchive <= DataTo).Where(ad => ad.status == "7");
                int jungongProCnt = ds.Count();   //竣工总工程数
                var jgMin_paperProjectSeqNo = "";
                var jgMax_paperProjectSeqNo = "";
                if (jungongProCnt != 0)
            
                {
                    var ds_min = ds.OrderBy(ad => ad.paperProjectSeqNo);
                    jgMin_paperProjectSeqNo = Convert.ToString(ds_min.First().paperProjectSeqNo);
                    var ds_max = ds.OrderByDescending(ad => ad.paperProjectSeqNo);
                    jgMax_paperProjectSeqNo = Convert.ToString(ds_max.First().paperProjectSeqNo);

                }
                
                var ds1 = ab.vw_ArchiveList.Where(ad => ad.dateArchive >= DataFrom).Where(ad => ad.dateArchive <= DataTo).Where(ad => ad.status == "7");
                List<vw_ArchiveList> list0 = ds1.ToList();
                int? jungongArchCnt = 0;
                for (int i = 0; i < list0.Count; i++)
                {
                    if (list0[i].archivesCount != null)
                    {
                        jungongArchCnt += int.Parse(list0[i].archivesCount);   //竣工总案卷数
                    }

                }
                //管线档案
                var gx = gb.vw_gxprojectList.Where(ad => ad.dateArchive >= DataFrom).Where(ad => ad.dateArchive <= DataTo).Where(ad => ad.status == "7");
                int gxProCnt = gx.Count();   //管线总工程数    
                var gxMax_paperProjectSeqNo = "";
                var gxMin_paperProjectSeqNo = "";

                if (gxProCnt != 0)
                {
                    var gx_max = gx.OrderByDescending(ad => ad.paperProjectSeqNo);
                    gxMax_paperProjectSeqNo = Convert.ToString(gx_max.First().paperProjectSeqNo);
                    var gx_min = gx.OrderBy(ad => ad.paperProjectSeqNo);
                    gxMin_paperProjectSeqNo = Convert.ToString(gx_min.First().paperProjectSeqNo);
                }
                
                var gxbox = gb.vw_gxArchiveList.Where(ad => ad.dateArchive >= DataFrom).Where(ad => ad.dateArchive <= DataTo).Where(ad => ad.status == "7");
                List<vw_gxArchiveList> list1 = gxbox.ToList();
                int? gxArchCnt = 0;
                for (int i = 0; i < list1.Count(); i++)
                {
                    if (list1[i].archivesCount != null)
                    {
                        gxArchCnt += int.Parse(list1[i].archivesCount);   //管线总案卷数
                    }

                }
                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/JG_GXarchiveRK_TJ .rdlc");
                //ReportDataSource reportDataSource = new ReportDataSource("archives_use_amount_TJ", ds);
                //localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("jungongProCnt", jungongProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongArchCnt", jungongArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("jgMax_paperProjectSeqNo", jgMax_paperProjectSeqNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("jgMin_paperProjectSeqNo", jgMin_paperProjectSeqNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("gxProCnt", gxProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("gxArchCnt", gxArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("gxMax_paperProjectSeqNo", gxMax_paperProjectSeqNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("gxMin_paperProjectSeqNo", gxMin_paperProjectSeqNo.ToString().Trim()));
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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult Other_archiveRK_TJ(string action, string quyu1 , string quyu2, string type1, string type = "PDF")
        {
            List<SelectListItem> list3 = new List<SelectListItem> {
                new SelectListItem { Text = "定点", Value = "1" },
                new SelectListItem { Text = "审批", Value = "2" },
                new SelectListItem { Text = "审查", Value = "3" },
                new SelectListItem { Text = "选址", Value = "4" },
                new SelectListItem { Text = "规条", Value = "5" },
                new SelectListItem { Text = "用地", Value = "6" },
                new SelectListItem { Text = "规审", Value = "7" },
                new SelectListItem { Text = "建审", Value = "8" },
                new SelectListItem { Text = "道路红线", Value = "9" },
                new SelectListItem { Text = "变审", Value = "10" },
                new SelectListItem { Text = "外环", Value = "11" },
                new SelectListItem { Text = "函业", Value = "12" },
                new SelectListItem { Text = "单验(单体验收)", Value = "13" },
                new SelectListItem { Text = "建验(规划验收合格证)", Value = "14" },
                new SelectListItem { Text = "广雕", Value = "15" },
                new SelectListItem { Text = "归验", Value = "16" },
                new SelectListItem { Text = "建拆", Value = "17" },
                new SelectListItem { Text = "市政管线", Value = "18" },
                new SelectListItem { Text = "临市政", Value = "19" },
                new SelectListItem { Text = "建管", Value = "20" },
                new SelectListItem { Text = "验整", Value = "21" },
                new SelectListItem { Text = "验线", Value = "24" },
                new SelectListItem { Text = "变备字", Value = "25" },
                new SelectListItem { Text = "控制线", Value = "26" },
                new SelectListItem { Text = "图批", Value = "27" },
                new SelectListItem { Text = "延期", Value = "28" },
                new SelectListItem { Text = "变更", Value = "29" },
                new SelectListItem { Text = "图审", Value = "30" },
                new SelectListItem { Text = "市政图审", Value = "31" },
                new SelectListItem { Text = "外装", Value = "32" },
                new SelectListItem { Text = "市政字", Value = "33" },
            };

            List<SelectListItem> list2 = new List<SelectListItem> {
                new SelectListItem { Text = "市", Value = "1" },
                new SelectListItem { Text = "市北区", Value = "2" },
                new SelectListItem { Text = "原四方区", Value = "3" },
                new SelectListItem { Text = "李沧区", Value = "4" },
                new SelectListItem { Text = "市南区", Value = "5" },
                new SelectListItem { Text = "崂山区", Value = "6" },
                new SelectListItem { Text = "城阳区", Value = "7" },
                new SelectListItem { Text = "黄岛区", Value = "8" },
                new SelectListItem { Text = "胶州区", Value = "9" },
                new SelectListItem { Text = "胶南区", Value = "10" },
                new SelectListItem { Text = "平度区", Value = "11" },
                new SelectListItem { Text = "莱西区", Value = "12" },
                new SelectListItem { Text = "即墨区", Value = "13" },
                new SelectListItem { Text = "开发区", Value = "14" },
            };
            ViewBag.type1 = new SelectList(list3, "Value", "Text");
            ViewBag.quyu1 = new SelectList(list2, "Value", "Text", "1");
            ViewBag.quyu2 = new SelectList(list2, "Value", "Text", "1");

            if (action == "查看统计结果")
            {
                LocalReport localReport = new LocalReport();
                if (Request.Form["startdata"] == "" || Request.Form["enddata"] == "") {
                    return Content("<script>alert('请输入起止时间！');history.back();</script>");
                }
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                //规划档案
                var guihua = bb.PlanProject.Where(ad => ad.status == "RK").Where(ad => ad.rukutime >= DataFrom).Where(ad => ad.rukutime <= DataTo);
                int guihuaArchCnt = guihua.Count();   //规划总案卷数    

                var box = from a in guihua
                          group a by a.totalSeqNo.Substring(0,8);
                var guihuaProCnt = box.Count(); //规划总工程数 
                var guihuaMin_totalSeqNo = "0";
                var guihuaMax_totalSeqNo = "0";
                if (guihuaProCnt != 0)
                {
                    var ds_min = guihua.OrderBy(ad => ad.totalSeqNo);
                    guihuaMin_totalSeqNo = ds_min.First().totalSeqNo;
                    var ds_max = guihua.OrderByDescending(ad => ad.totalSeqNo);
                    guihuaMax_totalSeqNo = ds_max.First().totalSeqNo;

                }

                //声像档案
                var shengxiang = cb.VideoArchives.Where(ad => ad.videoStatus == "4").Where(ad => ad.rukutime >= DataFrom).Where(ad => ad.rukutime <= DataTo);
                int shengxiangProCnt = shengxiang.Count();   //声像总工程数
                List<VideoArchives> list4 = shengxiang.ToList();
                int? videoArchCnt = 0;
                int? photoArchCnt = 0;
                for (int i = 0; i < list4.Count(); i++)
                {
                    if (list4[i].videoCassetteBoxCount == null || list4[i].videoCassetteBoxCount == "")
                    {
                        list4[i].videoCassetteBoxCount = "0";
                    }
                    if (list4[i].photoBoxCount == null || list4[i].photoBoxCount == "")
                    {
                        list4[i].photoBoxCount = "0";
                    }
                    videoArchCnt += int.Parse(list4[i].videoCassetteBoxCount);   //视频案卷数
                    photoArchCnt += int.Parse(list4[i].photoBoxCount);      //照片案卷数
                }
                var shengxiangMin_videoProjectSeqNo = "0";
                var shengxiangMax_videoProjectSeqNo = "0";
                if (shengxiangProCnt != 0)
                {
                    var shengxiang_min = shengxiang.OrderBy(ad => ad.videoProjectSeqNo);
                    shengxiangMin_videoProjectSeqNo = Convert.ToString(shengxiang_min.First().videoProjectSeqNo);
                    var shengxiang_max = shengxiang.OrderByDescending(ad => ad.videoProjectSeqNo);
                    shengxiangMax_videoProjectSeqNo = Convert.ToString(shengxiang_max.First().videoProjectSeqNo);

                }
                //东部档案
                var east = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.rukutime >= DataFrom).Where(ad => ad.rukutime <= DataTo);
                int eastArchCnt = east.Count();  //东部总案卷数
                List<OtherArchives> list_east = east.ToList();
                for (int i = 0; i < list_east.Count(); i++)
                {

                    list_east[i].licenceNo = list_east[i].licenceNo.Substring(0, 4).Trim();


                }

                var east1 = from a in list_east
                            group a by a.licenceNo;
                var eastProCnt = east1.Count();//东部总工程数
                var eastMin_licenceNo = "";
                var eastMax_licenceNo = "";
                if (eastArchCnt != 0)
                {
                    var east_min = list_east.OrderBy(ad => ad.licenceNo);
                    eastMin_licenceNo = east_min.First().licenceNo;
                    var east_max = list_east.OrderByDescending(ad => ad.licenceNo);
                    eastMax_licenceNo = east_max.First().licenceNo;

                }

                //执照档案
                var zhizhao = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo != "").Where(ad => ad.licenceNo != null).Where(ad => ad.rukutime >= DataFrom).Where(ad => ad.rukutime <= DataTo);
                int zhizhaoArchCnt = zhizhao.Count() - eastArchCnt; ;   //执照总案卷数(不包括东部)

                List<OtherArchives> list = zhizhao.ToList();

                for (int i = 0; i < list.Count(); i++)
                {
                    //Regex reg = new Regex(@"[\u4e00-\u9fa5]");//正则表达式

                    //if (reg.IsMatch(list[i].licenceNo.Trim()) == false)
                    //{
                    //    int n = list[i].licenceNo.Length;
                    //    if (n == 14)
                    //    {
                    //        list[i].licenceNo = list[i].licenceNo.Substring(0, 10).Trim();
                    //    }
                    //    else if(n > 15){
                    //        int m = list[i].licenceNo.LastIndexOf('-');
                    //        list[i].licenceNo = list[i].licenceNo.Substring(0, m).Trim();
                    //    }
                    //}
                    //else
                    //{
                    //    list[i].licenceNo = "-1";
                    //}
                    if (list[i].licenceNo != "" && list[i].licenceNo != null)
                    {
                        int n = list[i].licenceNo.Trim().Length;
                        if (n == 14)
                        {
                            list[i].licenceNo = list[i].licenceNo.Substring(0, 10).Trim();
                        }
                        else if (n > 15)
                        {
                            int m = list[i].licenceNo.LastIndexOf('-');
                            list[i].licenceNo = list[i].licenceNo.Substring(0, m).Trim();
                        }
                        else if(n == 15)
                        {
                            //list[i].licenceNo = "-1";
                            list[i].licenceNo = list[i].licenceNo.Substring(0, 11).Trim();
                        }
                    }
                    else
                    {
                        list[i].licenceNo = "-1";
                    }

                }
                var list1 = list.Where(ad => ad.licenceNo != "-1");
                var zzbox = from a in list1
                            group a by a.licenceNo;
                var zhizhaoProCnt = zzbox.Count();
                //执照总工程数
                var zhizhaoMin_licenceNo = "0";
                var zhizhaoMax_licenceNo = "0";
                if (zhizhaoArchCnt != 0)
                {
                    var zhizhao_min = list.Where(ad => ad.licenceNo != "-1").OrderBy(ad => ad.licenceNo);
                    zhizhaoMin_licenceNo = zhizhao_min.First().licenceNo;
                    var zhizhao_max = list.Where(ad => ad.licenceNo != "-1").OrderByDescending(ad => ad.licenceNo);
                    zhizhaoMax_licenceNo = zhizhao_max.First().licenceNo;

                }
                //道路档案
                var daolu = db.OtherArchives.Where(ad => ad.classTypeID == 2).Where(ad => ad.rukutime >= DataFrom).Where(ad => ad.rukutime <= DataTo);
                var daolu_que = daolu.Where(ad => ad.applyUnit.Contains("缺"));
                int daoluArchCnt = daolu.Count() - daolu_que.Count();//不用这么麻烦，缺的已经包含Q的，故减去缺的即可
                List<OtherArchives> list_daolu = daolu.ToList();
                for (int i = 0; i < list_daolu.Count(); i++)
                {

                    if (list_daolu[i].applyUnit.Contains("缺"))
                    {
                        list[i].volNo = "-1";
                    }

                }
                var daoluMin_volNo = "0";
                var daoluMax_volNo = "0";
                if (daoluArchCnt != 0)
                {
                    var daolu_min = list_daolu.Where(ad => ad.volNo != "-1").OrderBy(ad => ad.volNo);
                    daoluMin_volNo = daolu_min.First().volNo;
                    var daolu_max = list_daolu.Where(ad => ad.volNo != "-1").OrderByDescending(ad => ad.volNo);
                    daoluMax_volNo = daolu_max.First().volNo;

                }
                //分类档案
                var fenlei = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.rukutime >= DataFrom).Where(ad => ad.rukutime <= DataTo);
                int fenleiArchCnt = fenlei.Count();   //分类总案卷数          
                var fenlei1 = fenlei.Where(ad => ad.archiveNo != null).Where(ad => ad.archiveNo != "");
                var fenleiMin_archiveNo = "0";
                var fenleiMax_archiveNo = "0";
                if (fenleiArchCnt != 0)
                {
                    var fenlei_min = fenlei1.OrderBy(ad => ad.archiveNo);
                    fenleiMin_archiveNo = fenlei_min.First().archiveNo;
                    var fenlei_max = fenlei1.OrderByDescending(ad => ad.archiveNo);
                    fenleiMax_archiveNo = fenlei_max.First().archiveNo;

                }
                //图纸档案
                var tuzhi = ab.TuzhiArchives.Where(ad => ad.rukutime >= DataFrom).Where(ad => ad.rukutime <= DataTo);
                int tuzhiArchCnt = tuzhi.Count();   //图纸总案卷数

                long? tuzhiMin_seqNo = 0;
                long? tuzhiMax_seqNo = 0;
                if (tuzhiArchCnt != 0)
                {
                    var tuzhi_min = tuzhi.OrderBy(ad => ad.seqNo);
                    tuzhiMin_seqNo = tuzhi_min.First().seqNo;
                    var tuzhi_max = tuzhi.OrderByDescending(ad => ad.seqNo);
                    tuzhiMax_seqNo = tuzhi_max.First().seqNo;

                }

                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/Other_archiveRK_TJ.rdlc");
                //ReportDataSource reportDataSource = new ReportDataSource("archives_use_amount_TJ", ds);
                //localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("guihuaArchCnt", guihuaArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaProCnt", guihuaProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaMax_totalSeqNo", guihuaMax_totalSeqNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaMin_totalSeqNo", guihuaMin_totalSeqNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("shengxiangProCnt", shengxiangProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("shengxiangMin_videoProjectSeqNo", shengxiangMin_videoProjectSeqNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("shengxiangMax_videoProjectSeqNo", shengxiangMax_videoProjectSeqNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("videoArchCnt", videoArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("photoArchCnt", photoArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoProCnt", zhizhaoProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCnt", zhizhaoArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoMin_licenceNo", zhizhaoMin_licenceNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoMax_licenceNo", zhizhaoMax_licenceNo.ToString().Trim()));
                //parameterList.Add(new ReportParameter("eastProCnt", eastProCnt.ToString().Trim()));
                //parameterList.Add(new ReportParameter("eastArchCnt", eastArchCnt.ToString().Trim()));
                //parameterList.Add(new ReportParameter("eastMin_licenceNo", eastMin_licenceNo.ToString().Trim()));
                //parameterList.Add(new ReportParameter("eastMax_licenceNo", eastMax_licenceNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("daoluArchCnt", daoluArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("daoluMin_volNo", daoluMin_volNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("daoluMax_volNo", daoluMax_volNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCnt", fenleiArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiMin_archiveNo", fenleiMin_archiveNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiMax_archiveNo", fenleiMax_archiveNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("tuzhiArchCnt", tuzhiArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("tuzhiMin_seqNo", tuzhiMin_seqNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("tuzhiMax_seqNo", tuzhiMax_seqNo.ToString().Trim()));

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

            if (action == "查看统计结果(规划)")
            {
                LocalReport localReport = new LocalReport();
                if (Request.Form["startdata1"] == "" || Request.Form["enddata1"] == "")
                {
                    return Content("<script>alert('请输入起止时间！');history.back();</script>");
                }
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata1"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata1"]);
                int quyu11= int.Parse(quyu1);
                //规划档案
                var guihua = bb.PlanProject.Where(ad => ad.status == "RK").Where(ad => ad.rukutime >= DataFrom).Where(ad => ad.rukutime <= DataTo).Where(ad => ad.urban_type == quyu11);
                int guihuaArchCnt = guihua.Count();   //规划总案卷数    

                var box = from a in guihua
                          group a by a.totalSeqNo.Substring(0, 8);
                var guihuaProCnt = box.Count(); //规划总工程数 
                var guihuaMin_totalSeqNo = "0";
                var guihuaMax_totalSeqNo = "0";
                if (guihuaProCnt != 0)
                {
                    var ds_min = guihua.OrderBy(ad => ad.totalSeqNo);
                    guihuaMin_totalSeqNo = ds_min.First().totalSeqNo;
                    var ds_max = guihua.OrderByDescending(ad => ad.totalSeqNo);
                    guihuaMax_totalSeqNo = ds_max.First().totalSeqNo;

                }

                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/Other_archiveRK_GH.rdlc");
                //ReportDataSource reportDataSource = new ReportDataSource("archives_use_amount_TJ", ds);
                //localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("guihuaArchCnt", guihuaArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaProCnt", guihuaProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaMax_totalSeqNo", guihuaMax_totalSeqNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaMin_totalSeqNo", guihuaMin_totalSeqNo.ToString().Trim()));

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

            if (action == "查看统计结果(执照)")
            {
                LocalReport localReport = new LocalReport();
                if (Request.Form["startdata2"] == "" || Request.Form["enddata2"] == "")
                {
                    return Content("<script>alert('请输入起止时间！');history.back();</script>");
                }
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata2"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata2"]);

                //东部档案
                var east = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.rukutime >= DataFrom).Where(ad => ad.rukutime <= DataTo);
                int eastArchCnt = east.Count();  //东部总案卷数
                List<OtherArchives> list_east = east.ToList();
                for (int i = 0; i < list_east.Count(); i++)
                {

                    list_east[i].licenceNo = list_east[i].licenceNo.Substring(0, 4).Trim();


                }

                var east1 = from a in list_east
                            group a by a.licenceNo;
                var eastProCnt = east1.Count();//东部总工程数
                var eastMin_licenceNo = "";
                var eastMax_licenceNo = "";
                if (eastArchCnt != 0)
                {
                    var east_min = list_east.OrderBy(ad => ad.licenceNo);
                    eastMin_licenceNo = east_min.First().licenceNo;
                    var east_max = list_east.OrderByDescending(ad => ad.licenceNo);
                    eastMax_licenceNo = east_max.First().licenceNo;

                }
                int quyu22 = int.Parse(quyu2);
                //执照档案
                var zhizhao = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo != "").Where(ad => ad.licenceNo != null).Where(ad => ad.rukutime >= DataFrom).Where(ad => ad.rukutime <= DataTo).Where(ad => ad.urban_type == quyu22);
                int zhizhaoArchCnt = zhizhao.Count() - eastArchCnt; ;   //执照总案卷数(不包括东部)

                List<OtherArchives> list = zhizhao.ToList();

                for (int i = 0; i < list.Count(); i++)
                {
                    //Regex reg = new Regex(@"[\u4e00-\u9fa5]");//正则表达式

                    //if (reg.IsMatch(list[i].licenceNo.Trim()) == false)
                    //{
                    //    int n = list[i].licenceNo.Length;
                    //    if (n == 14)
                    //    {
                    //        list[i].licenceNo = list[i].licenceNo.Substring(0, 10).Trim();
                    //    }
                    //    else if (n > 15)
                    //    {
                    //        int m = list[i].licenceNo.LastIndexOf('-');
                    //        list[i].licenceNo = list[i].licenceNo.Substring(0, m).Trim();
                    //    }
                    //}
                    //else
                    //{
                    //    list[i].licenceNo = "-1";
                    //}
                    if (list[i].licenceNo != "" && list[i].licenceNo != null)
                    {
                        int n = list[i].licenceNo.Trim().Length;
                        if (n == 14)
                        {
                            list[i].licenceNo = list[i].licenceNo.Substring(0, 10).Trim();
                        }
                        else if (n > 15)
                        {
                            int m = list[i].licenceNo.LastIndexOf('-');
                            list[i].licenceNo = list[i].licenceNo.Substring(0, m).Trim();
                        }
                        else if (n == 15) //可能为废弃工程
                        {
                            //list[i].licenceNo = "-1";
                            list[i].licenceNo = list[i].licenceNo.Substring(0, 11).Trim();
                        }
                    }
                    else {
                        list[i].licenceNo = "-1";
                    }
                }
                var list1 = list.Where(ad => ad.licenceNo != "-1");
                var zzbox = from a in list1
                            group a by a.licenceNo;
                var zhizhaoProCnt = zzbox.Count();


                //执照总工程数
                var zhizhaoMin_licenceNo = "0";
                var zhizhaoMax_licenceNo = "0";
                if (zhizhaoArchCnt != 0)
                {
                    var zhizhao_min = list.Where(ad => ad.licenceNo != "-1").OrderBy(ad => ad.licenceNo);
                    if (zhizhao_min.Count() != 0) {
                        zhizhaoMin_licenceNo = zhizhao_min.First().licenceNo;
                    }
                    
                    var zhizhao_max = list.Where(ad => ad.licenceNo != "-1").OrderByDescending(ad => ad.licenceNo);
                    if (zhizhao_max.Count() != 0)
                    {
                        zhizhaoMax_licenceNo = zhizhao_max.First().licenceNo;
                    }

                }

                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/Other_archiveRK_ZZ.rdlc");
                //ReportDataSource reportDataSource = new ReportDataSource("archives_use_amount_TJ", ds);
                //localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("zhizhaoProCnt", zhizhaoProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCnt", zhizhaoArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoMin_licenceNo", zhizhaoMin_licenceNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoMax_licenceNo", zhizhaoMax_licenceNo.ToString().Trim()));
                //parameterList.Add(new ReportParameter("eastProCnt", eastProCnt.ToString().Trim()));
                //parameterList.Add(new ReportParameter("eastArchCnt", eastArchCnt.ToString().Trim()));
                //parameterList.Add(new ReportParameter("eastMin_licenceNo", eastMin_licenceNo.ToString().Trim()));
                //parameterList.Add(new ReportParameter("eastMax_licenceNo", eastMax_licenceNo.ToString().Trim()));
                
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
            if (action == "查看统计结果(规划 类型)")
            {
                LocalReport localReport = new LocalReport();
                if (Request.Form["startdata3"] == "" || Request.Form["enddata3"] == "")
                {
                    return Content("<script>alert('请输入起止时间！');history.back();</script>");
                }
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata3"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata3"]);

                var guihua = bb.PlanProject.Where(ad => ad.status == "RK").Where(ad => ad.rukutime >= DataFrom).Where(ad => ad.rukutime <= DataTo);
                if (type1 != "")
                {
                    int type2 = int.Parse(type1);
                    guihua = guihua.Where(ad => ad.classifyID == type2);
                }
                
                //规划档案
                
                int guihuaArchCnt = guihua.Count();   //规划总案卷数    

                var box = from a in guihua
                          group a by a.totalSeqNo.Substring(0, 8);
                var guihuaProCnt = box.Count(); //规划总工程数 
                var guihuaMin_totalSeqNo = "0";
                var guihuaMax_totalSeqNo = "0";
                if (guihuaProCnt != 0)
                {
                    var ds_min = guihua.OrderBy(ad => ad.totalSeqNo);
                    guihuaMin_totalSeqNo = ds_min.First().totalSeqNo;
                    var ds_max = guihua.OrderByDescending(ad => ad.totalSeqNo);
                    guihuaMax_totalSeqNo = ds_max.First().totalSeqNo;

                }

                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/Other_archiveRK_GH.rdlc");
                //ReportDataSource reportDataSource = new ReportDataSource("archives_use_amount_TJ", ds);
                //localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("guihuaArchCnt", guihuaArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaProCnt", guihuaProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaMax_totalSeqNo", guihuaMax_totalSeqNo.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaMin_totalSeqNo", guihuaMin_totalSeqNo.ToString().Trim()));

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

            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }

        public ActionResult archiveClassify_Time_TJ(string action, string type = "PDF")
        {
            if (action == "查看统计结果")
            {
                LocalReport localReport = new LocalReport();
                if (Request.Form["startdata"] == "" || Request.Form["enddata"] == "")
                {
                    return Content("<script>alert('请输入起止时间！');history.back();</script>");
                }
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);

                //竣工档案
                var dsA = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("A") ).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo); 
                var dsB = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("B")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsC = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("C")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsD = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("D")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsE = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("E")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsF = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("F")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsG = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("G")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsH = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("H")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsI = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("I")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsJ = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("J")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsK = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("K")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsL = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("L")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsM = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("M")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsN = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("N")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsO = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("O")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsP = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("P")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsQ = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("Q")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var dsR = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("R")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                int jungongA = dsA.Count();
                int jungongB = dsB.Count();
                int jungongC = dsC.Count();
                int jungongD = dsD.Count();
                int jungongE = dsE.Count();
                int jungongF = dsF.Count();
                int jungongG = dsG.Count();
                int jungongH = dsH.Count();
                int jungongI = dsI.Count();
                int jungongJ = dsJ.Count();
                int jungongK = dsK.Count();
                int jungongL = dsL.Count();
                int jungongM = dsM.Count();
                int jungongN = dsN.Count();
                int jungongO = dsO.Count();
                int jungongP = dsP.Count();
                int jungongQ = dsQ.Count();
                int jungongR = dsR.Count();
                //管线档案
                var gxA = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("A")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxB = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("B")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxC = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("C")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxD = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("D")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxE = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("E")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxF = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("F")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxG = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("G")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxH = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("H")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxI = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("I")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxJ = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("J")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxK = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("K")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxL = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("L")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxM = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("M")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxN = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("N")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxO = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("O")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxP = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("P")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxQ = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("Q")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                var gxR = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("R")).Where(ad => ad.typerDate >= DataFrom).Where(ad => ad.typerDate <= DataTo);
                int guanxianA = gxA.Count();
                int guanxianB = gxB.Count();
                int guanxianC = gxC.Count();
                int guanxianD = gxD.Count();
                int guanxianE = gxE.Count();
                int guanxianF = gxF.Count();
                int guanxianG = gxG.Count();
                int guanxianH = gxH.Count();
                int guanxianI = gxI.Count();
                int guanxianJ = gxJ.Count();
                int guanxianK = gxK.Count();
                int guanxianL = gxL.Count();
                int guanxianM = gxM.Count();
                int guanxianN = gxN.Count();
                int guanxianO = gxO.Count();
                int guanxianP = gxP.Count();
                int guanxianQ = gxQ.Count();
                int guanxianR = gxR.Count();
                //援川档案
                var ycS = db.vw_YuanChuancategoryList.Where(ad => ad.archivesNo.Contains("S")).Where(ad => ad.status == "7");
                int yuanchuanS = ycS.Count();

                //声像档案
                var shengxiang = cb.vw_VideoArchivesList.Where(ad => ad.videoStatus == "4").Where(ad => ad.shootingDate >= DataFrom).Where(ad => ad.shootingDate <= DataTo);
                int shengxiangProCnt = shengxiang.Count();   //声像总工程数
                List<vw_VideoArchivesList> list4 = shengxiang.ToList();
                int videoArchCnt = 0;
                int photoArchCnt = 0;
                for (int i = 0; i < list4.Count(); i++)
                {
                    if (list4[i].videoCassetteBoxCount != null && list4[i].videoCassetteBoxCount != "" && list4[i].photoBoxCount != null && list4[i].photoBoxCount != "")
                    {
                        videoArchCnt += int.Parse(list4[i].videoCassetteBoxCount);   //视频案卷数
                        photoArchCnt += int.Parse(list4[i].photoBoxCount);      //照片案卷数
                    }
                }
                int shengxiangArchCnt = videoArchCnt + photoArchCnt;
                //东部档案
                var eastA = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东"));
                List<OtherArchives> east1 = eastA.ToList();
                List<OtherArchives> prolistA = new List<OtherArchives>();
                for (int i = 0; i < east1.Count(); i++)
                {
                    if(east1[i].luruTime != null&& east1[i].luruTime != "")
                    {
                        if (DateTime.Parse(east1[i].luruTime) >= DataFrom && DateTime.Parse(east1[i].luruTime) <= DataTo)
                        {
                            OtherArchives prolist1 = new OtherArchives();
                            prolist1.luruTime = east1[i].luruTime;
                            prolist1.registrationNo = east1[i].registrationNo;
                            prolistA.Add(prolist1);
                        }

                    }
                }
                int eastArchCntA = prolistA.Count();  //东部总案卷数

                var eastB = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("B"));
                List<OtherArchives> east2 = eastB.ToList();
                List<OtherArchives> prolistB = new List<OtherArchives>();
                for (int i = 0; i < east2.Count(); i++)
                {

                    if (DateTime.Parse(east2[i].luruTime) >= DataFrom && DateTime.Parse(east2[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist2 = new OtherArchives();
                        prolist2.luruTime = east2[i].luruTime;
                        prolist2.registrationNo = east2[i].registrationNo;
                        prolistB.Add(prolist2);
                    }
                }
                int eastArchCntB = prolistB.Count();  //东部总案卷数
                var eastC = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("C"));
                List<OtherArchives> east3 = eastC.ToList();
                List<OtherArchives> prolistC = new List<OtherArchives>();
                for (int i = 0; i < east3.Count(); i++)
                {

                    if (DateTime.Parse(east3[i].luruTime) >= DataFrom && DateTime.Parse(east3[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east3[i].luruTime;
                        prolist.registrationNo = east3[i].registrationNo;
                        prolistC.Add(prolist);
                    }
                }
                int eastArchCntC = prolistC.Count();  //东部总案卷数
                var eastD = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("D"));
                List<OtherArchives> east4 = eastD.ToList();
                List<OtherArchives> prolistD = new List<OtherArchives>();
                for (int i = 0; i < east4.Count(); i++)
                {

                    if (DateTime.Parse(east4[i].luruTime) >= DataFrom && DateTime.Parse(east4[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east4[i].luruTime;
                        prolist.registrationNo = east4[i].registrationNo;
                        prolistD.Add(prolist);
                    }
                }

                int eastArchCntD = prolistD.Count();  //东部总案卷数
                var eastE = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("E"));
                List<OtherArchives> east5 = eastE.ToList();
                List<OtherArchives> prolistE = new List<OtherArchives>();
                for (int i = 0; i < east5.Count(); i++)
                {

                    if (DateTime.Parse(east5[i].luruTime) >= DataFrom && DateTime.Parse(east5[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east5[i].luruTime;
                        prolist.registrationNo = east5[i].registrationNo;
                        prolistE.Add(prolist);
                    }
                }

                int eastArchCntE = prolistE.Count();  //东部总案卷数
                var eastF = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("F"));
                List<OtherArchives> east6 = eastF.ToList();
                List<OtherArchives> prolistF = new List<OtherArchives>();
                for (int i = 0; i < east6.Count(); i++)
                {

                    if (DateTime.Parse(east6[i].luruTime) >= DataFrom && DateTime.Parse(east6[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east6[i].luruTime;
                        prolist.registrationNo = east6[i].registrationNo;
                        prolistF.Add(prolist);
                    }
                }
                int eastArchCntF = prolistF.Count();  //东部总案卷数
                var eastG = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("G"));
                List<OtherArchives> east7 = eastG.ToList();
                List<OtherArchives> prolistG = new List<OtherArchives>();
                for (int i = 0; i < east7.Count(); i++)
                {

                    if (DateTime.Parse(east7[i].luruTime) >= DataFrom && DateTime.Parse(east7[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east7[i].luruTime;
                        prolist.registrationNo = east7[i].registrationNo;
                        prolistG.Add(prolist);
                    }
                }

                int eastArchCntG = prolistG.Count();  //东部总案卷数
                var eastH = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("H"));
                List<OtherArchives> east8 = eastH.ToList();
                List<OtherArchives> prolistH = new List<OtherArchives>();
                for (int i = 0; i < east8.Count(); i++)
                {

                    if (DateTime.Parse(east8[i].luruTime) >= DataFrom && DateTime.Parse(east8[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east8[i].luruTime;
                        prolist.registrationNo = east8[i].registrationNo;
                        prolistH.Add(prolist);
                    }
                }

                int eastArchCntH = prolistH.Count();  //东部总案卷数
                var eastI = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("I"));
                List<OtherArchives> east9 = eastI.ToList();
                List<OtherArchives> prolistI = new List<OtherArchives>();
                for (int i = 0; i < east9.Count(); i++)
                {

                    if (DateTime.Parse(east9[i].luruTime) >= DataFrom && DateTime.Parse(east9[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east9[i].luruTime;
                        prolist.registrationNo = east9[i].registrationNo;
                        prolistI.Add(prolist);
                    }
                }


                int eastArchCntI = prolistI.Count();  //东部总案卷数
                var eastJ = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("J"));
                List<OtherArchives> east10 = eastJ.ToList();
                List<OtherArchives> prolistJ = new List<OtherArchives>();
                for (int i = 0; i < east10.Count(); i++)
                {

                    if (DateTime.Parse(east10[i].luruTime) >= DataFrom && DateTime.Parse(east10[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east10[i].luruTime;
                        prolist.registrationNo = east10[i].registrationNo;
                        prolistJ.Add(prolist);
                    }
                }


                int eastArchCntJ = prolistJ.Count();  //东部总案卷数
                var eastK = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("K"));
                List<OtherArchives> east11 = eastK.ToList();
                List<OtherArchives> prolistK = new List<OtherArchives>();
                for (int i = 0; i < east11.Count(); i++)
                {

                    if (DateTime.Parse(east11[i].luruTime) >= DataFrom && DateTime.Parse(east11[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east11[i].luruTime;
                        prolist.registrationNo = east11[i].registrationNo;
                        prolistK.Add(prolist);
                    }
                }

                int eastArchCntK = eastK.Count();  //东部总案卷数
                var eastL = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("L"));
                List<OtherArchives> east12 = eastL.ToList();
                List<OtherArchives> prolistL = new List<OtherArchives>();
                for (int i = 0; i < east12.Count(); i++)
                {

                    if (DateTime.Parse(east12[i].luruTime) >= DataFrom && DateTime.Parse(east12[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east12[i].luruTime;
                        prolist.registrationNo = east12[i].registrationNo;
                        prolistL.Add(prolist);
                    }
                }

                int eastArchCntL = prolistL.Count();  //东部总案卷数
                var eastM = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("M"));
                List<OtherArchives> east13 = eastM.ToList();
                List<OtherArchives> prolistM = new List<OtherArchives>();
                for (int i = 0; i < east13.Count(); i++)
                {

                    if (DateTime.Parse(east13[i].luruTime) >= DataFrom && DateTime.Parse(east13[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east13[i].luruTime;
                        prolist.registrationNo = east13[i].registrationNo;
                        prolistM.Add(prolist);
                    }
                }

                int eastArchCntM = prolistM.Count();  //东部总案卷数
                var eastN = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("N"));
                List<OtherArchives> east14 = eastN.ToList();
                List<OtherArchives> prolistN = new List<OtherArchives>();
                for (int i = 0; i < east14.Count(); i++)
                {

                    if (DateTime.Parse(east14[i].luruTime) >= DataFrom && DateTime.Parse(east14[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east14[i].luruTime;
                        prolist.registrationNo = east14[i].registrationNo;
                        prolistN.Add(prolist);
                    }
                }

                int eastArchCntN = prolistN.Count();  //东部总案卷数
                var eastO = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("O"));
                List<OtherArchives> east15 = eastO.ToList();
                List<OtherArchives> prolistO = new List<OtherArchives>();
                for (int i = 0; i < east15.Count(); i++)
                {

                    if (DateTime.Parse(east15[i].luruTime) >= DataFrom && DateTime.Parse(east15[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east15[i].luruTime;
                        prolist.registrationNo = east15[i].registrationNo;
                        prolistO.Add(prolist);
                    }
                }

                int eastArchCntO = prolistO.Count();  //东部总案卷数
                var eastP = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("P"));
                List<OtherArchives> east16 = eastP.ToList();
                List<OtherArchives> prolistP = new List<OtherArchives>();
                for (int i = 0; i < east16.Count(); i++)
                {

                    if (DateTime.Parse(east16[i].luruTime) >= DataFrom && DateTime.Parse(east16[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east16[i].luruTime;
                        prolist.registrationNo = east16[i].registrationNo;
                        prolistP.Add(prolist);
                    }
                }


                int eastArchCntP = prolistP.Count();  //东部总案卷数
                var eastQ = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("Q"));
                List<OtherArchives> east17 = eastQ.ToList();
                List<OtherArchives> prolistQ = new List<OtherArchives>();
                for (int i = 0; i < east17.Count(); i++)
                {

                    if (DateTime.Parse(east17[i].luruTime) >= DataFrom && DateTime.Parse(east17[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east17[i].luruTime;
                        prolist.registrationNo = east17[i].registrationNo;
                        prolistQ.Add(prolist);
                    }
                }

                int eastArchCntQ = prolistQ.Count();  //东部总案卷数
                var eastR = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("R"));
                List<OtherArchives> east18 = eastR.ToList();
                List<OtherArchives> prolistR = new List<OtherArchives>();
                for (int i = 0; i < east18.Count(); i++)
                {

                    if (DateTime.Parse(east18[i].luruTime) >= DataFrom && DateTime.Parse(east18[i].luruTime) <= DataTo)
                    {
                        OtherArchives prolist = new OtherArchives();
                        prolist.luruTime = east18[i].luruTime;
                        prolist.registrationNo = east18[i].registrationNo;
                        prolistR.Add(prolist);
                    }
                }

                int eastArchCntR = prolistR.Count();  //东部总案卷数
                //执照档案
                var zhizhaoA = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("A"));
                int zhizhaoArchCntA = zhizhaoA.Count();  //执照总案卷数
                var zhizhaoB = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("B"));
                int zhizhaoArchCntB = zhizhaoB.Count();  //执照总案卷数
                var zhizhaoC = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("C"));
                int zhizhaoArchCntC = zhizhaoC.Count();  //执照总案卷数
                var zhizhaoD = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("D"));
                int zhizhaoArchCntD = zhizhaoD.Count();  //执照总案卷数
                var zhizhaoE = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("E"));
                int zhizhaoArchCntE = zhizhaoE.Count();  //执照总案卷数
                var zhizhaoF = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("F"));
                int zhizhaoArchCntF = zhizhaoF.Count();  //执照总案卷数
                var zhizhaoG = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("G"));
                int zhizhaoArchCntG = zhizhaoG.Count();  //执照总案卷数
                var zhizhaoH = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("H"));
                int zhizhaoArchCntH = zhizhaoH.Count();  //执照总案卷数
                var zhizhaoI = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("I"));
                int zhizhaoArchCntI = zhizhaoI.Count();  //执照总案卷数
                var zhizhaoJ = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("J"));
                int zhizhaoArchCntJ = zhizhaoJ.Count();  //执照总案卷数
                var zhizhaoK = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("K"));
                int zhizhaoArchCntK = zhizhaoK.Count();  //执照总案卷数
                var zhizhaoL = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("L"));
                int zhizhaoArchCntL = zhizhaoL.Count();  //执照总案卷数
                var zhizhaoM = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("M"));
                int zhizhaoArchCntM = zhizhaoM.Count();  //执照总案卷数
                var zhizhaoN = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("N"));
                int zhizhaoArchCntN = zhizhaoN.Count();  //执照总案卷数
                var zhizhaoO = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("O"));
                int zhizhaoArchCntO = zhizhaoO.Count();  //执照总案卷数
                var zhizhaoP = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("P"));
                int zhizhaoArchCntP = zhizhaoP.Count();  //执照总案卷数
                var zhizhaoQ = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("Q"));
                int zhizhaoArchCntQ = zhizhaoQ.Count();  //执照总案卷数
                var zhizhaoR = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("R"));
                int zhizhaoArchCntR = zhizhaoR.Count();  //执照总案卷数
                //分类档案
                var fenleiA = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("A"));
                int fenleiArchCntA = fenleiA.Count();  //分类总案卷数
                var fenleiB = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("B"));
                int fenleiArchCntB = fenleiB.Count();  //分类总案卷数
                var fenleiC = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("C"));
                int fenleiArchCntC = fenleiC.Count();  //分类总案卷数
                var fenleiD = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("D"));
                int fenleiArchCntD = fenleiD.Count();  //分类总案卷数
                var fenleiE = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("E"));
                int fenleiArchCntE = fenleiE.Count();  //分类总案卷数
                var fenleiF = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("F"));
                int fenleiArchCntF = fenleiF.Count();  //分类总案卷数
                var fenleiG = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("G"));
                int fenleiArchCntG = fenleiG.Count();  //分类总案卷数
                var fenleiH = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("H"));
                int fenleiArchCntH = fenleiH.Count();  //分类总案卷数
                var fenleiI = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("I"));
                int fenleiArchCntI = fenleiI.Count();  //分类总案卷数
                var fenleiJ = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("J"));
                int fenleiArchCntJ = fenleiJ.Count();  //分类总案卷数
                var fenleiK = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("K"));
                int fenleiArchCntK = fenleiK.Count();  //分类总案卷数
                var fenleiL = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("L"));
                int fenleiArchCntL = fenleiL.Count();  //分类总案卷数
                var fenleiM = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("M"));
                int fenleiArchCntM = fenleiM.Count();  //分类总案卷数
                var fenleiN = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("N"));
                int fenleiArchCntN = fenleiN.Count();  //分类总案卷数
                var fenleiO = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("O"));
                int fenleiArchCntO = fenleiO.Count();  //分类总案卷数
                var fenleiP = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("P"));
                int fenleiArchCntP = fenleiP.Count();  //分类总案卷数
                var fenleiQ = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("Q"));
                int fenleiArchCntQ = fenleiQ.Count();  //分类总案卷数
                var fenleiR = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("R"));
                int fenleiArchCntR = fenleiR.Count();  //分类总案卷数
                //征地档案
                var zdA = ab.zdArchive.Where(ad => ad.archiveNo.Contains("A"));
                int zdArchCntA = zdA.Count();  //征地总案卷数
                var zdB = ab.zdArchive.Where(ad => ad.archiveNo.Contains("B"));
                int zdArchCntB = zdB.Count();  //征地总案卷数
                var zdC = ab.zdArchive.Where(ad => ad.archiveNo.Contains("C"));
                int zdArchCntC = zdC.Count();  //征地总案卷数
                var zdD = ab.zdArchive.Where(ad => ad.archiveNo.Contains("D"));
                int zdArchCntD = zdD.Count();  //征地总案卷数
                var zdE = ab.zdArchive.Where(ad => ad.archiveNo.Contains("E"));
                int zdArchCntE = zdE.Count();  //征地总案卷数
                var zdF = ab.zdArchive.Where(ad => ad.archiveNo.Contains("F"));
                int zdArchCntF = zdF.Count();  //征地总案卷数
                var zdG = ab.zdArchive.Where(ad => ad.archiveNo.Contains("G"));
                int zdArchCntG = zdG.Count();  //征地总案卷数
                var zdH = ab.zdArchive.Where(ad => ad.archiveNo.Contains("H"));
                int zdArchCntH = zdH.Count();  //征地总案卷数
                var zdI = ab.zdArchive.Where(ad => ad.archiveNo.Contains("I"));
                int zdArchCntI = zdI.Count();  //征地总案卷数
                var zdJ = ab.zdArchive.Where(ad => ad.archiveNo.Contains("J"));
                int zdArchCntJ = zdJ.Count();  //征地总案卷数
                var zdK = ab.zdArchive.Where(ad => ad.archiveNo.Contains("K"));
                int zdArchCntK = zdK.Count();  //征地总案卷数
                var zdL = ab.zdArchive.Where(ad => ad.archiveNo.Contains("L"));
                int zdArchCntL = zdL.Count();  //征地总案卷数
                var zdM = ab.zdArchive.Where(ad => ad.archiveNo.Contains("M"));
                int zdArchCntM = zdM.Count();  //征地总案卷数
                var zdN = ab.zdArchive.Where(ad => ad.archiveNo.Contains("N"));
                int zdArchCntN = zdN.Count();  //征地总案卷数
                var zdO = ab.zdArchive.Where(ad => ad.archiveNo.Contains("O"));
                int zdArchCntO = zdO.Count();  //征地总案卷数
                var zdP = ab.zdArchive.Where(ad => ad.archiveNo.Contains("P"));
                int zdArchCntP = zdP.Count();  //征地总案卷数
                var zdQ = ab.zdArchive.Where(ad => ad.archiveNo.Contains("Q"));
                int zdArchCntQ = zdQ.Count();  //征地总案卷数
                var zdR = ab.zdArchive.Where(ad => ad.archiveNo.Contains("R"));
                int zdArchCntR = zdR.Count();  //征地总案卷数
                //全部档案
                int allArchCntA = jungongA + guanxianA + eastArchCntA + zhizhaoArchCntA + fenleiArchCntA + zdArchCntA;
                int allArchCntB = jungongB + guanxianB + eastArchCntB + zhizhaoArchCntB + fenleiArchCntB + zdArchCntB;
                int allArchCntC = jungongC + guanxianC + eastArchCntC + zhizhaoArchCntC + fenleiArchCntC + zdArchCntC;
                int allArchCntD = jungongD + guanxianD + eastArchCntD + zhizhaoArchCntD + fenleiArchCntD + zdArchCntD;
                int allArchCntE = jungongE + guanxianE + eastArchCntE + zhizhaoArchCntE + fenleiArchCntE + zdArchCntE;
                int allArchCntF = jungongF + guanxianF + eastArchCntF + zhizhaoArchCntF + fenleiArchCntF + zdArchCntF;
                int allArchCntG = jungongG + guanxianG + eastArchCntG + zhizhaoArchCntG + fenleiArchCntG + zdArchCntG;
                int allArchCntH = jungongH + guanxianH + eastArchCntH + zhizhaoArchCntH + fenleiArchCntH + zdArchCntH;
                int allArchCntI = jungongI + guanxianI + eastArchCntI + zhizhaoArchCntI + fenleiArchCntI + zdArchCntI;
                int allArchCntJ = jungongJ + guanxianJ + eastArchCntJ + zhizhaoArchCntJ + fenleiArchCntJ + zdArchCntJ;
                int allArchCntK = jungongK + guanxianK + eastArchCntK + zhizhaoArchCntK + fenleiArchCntK + zdArchCntK;
                int allArchCntL = jungongL + guanxianL + eastArchCntL + zhizhaoArchCntL + fenleiArchCntL + zdArchCntL;
                int allArchCntM = jungongM + guanxianM + eastArchCntM + zhizhaoArchCntM + fenleiArchCntM + zdArchCntM;
                int allArchCntN = jungongN + guanxianN + eastArchCntN + zhizhaoArchCntN + fenleiArchCntN + zdArchCntN;
                int allArchCntO = jungongO + guanxianO + eastArchCntO + zhizhaoArchCntO + fenleiArchCntO + zdArchCntO;
                int allArchCntP = jungongP + guanxianP + eastArchCntP + zhizhaoArchCntP + fenleiArchCntP + zdArchCntP;
                int allArchCntQ = jungongQ + guanxianQ + eastArchCntQ + zhizhaoArchCntQ + fenleiArchCntQ + zdArchCntQ;
                int allArchCntR = jungongR + guanxianR + eastArchCntR + zhizhaoArchCntR + fenleiArchCntR + zdArchCntR + shengxiangArchCnt;


                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/archiveClassify_Time_TJ.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("DataSet1", ycS);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("jungongA", jungongA.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongB", jungongB.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongC", jungongC.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongD", jungongD.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongE", jungongE.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongF", jungongF.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongG", jungongG.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongH", jungongH.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongI", jungongI.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongJ", jungongJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongK", jungongK.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongL", jungongL.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongM", jungongM.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongN", jungongN.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongO", jungongO.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongP", jungongP.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongQ", jungongQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongR", jungongR.ToString().Trim()));

                parameterList.Add(new ReportParameter("guanxianA", guanxianA.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianB", guanxianB.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianC", guanxianC.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianD", guanxianD.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianE", guanxianE.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianF", guanxianF.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianG", guanxianG.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianH", guanxianH.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianI", guanxianI.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianJ", guanxianJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianK", guanxianK.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianL", guanxianL.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianM", guanxianM.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianN", guanxianN.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianO", guanxianO.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianP", guanxianP.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianQ", guanxianQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianR", guanxianR.ToString().Trim()));

                //parameterList.Add(new ReportParameter("yuanchuanS", yuanchuanS.ToString().Trim()));
                parameterList.Add(new ReportParameter("shengxiangArchCnt", shengxiangArchCnt.ToString().Trim()));

                parameterList.Add(new ReportParameter("eastArchCntA", eastArchCntA.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntB", eastArchCntB.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntC", eastArchCntC.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntD", eastArchCntD.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntE", eastArchCntE.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntF", eastArchCntF.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntG", eastArchCntG.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntH", eastArchCntH.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntI", eastArchCntI.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntJ", eastArchCntJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntK", eastArchCntK.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntL", eastArchCntL.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntM", eastArchCntM.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntN", eastArchCntN.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntO", eastArchCntO.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntP", eastArchCntP.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntQ", eastArchCntQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntR", eastArchCntR.ToString().Trim()));

                parameterList.Add(new ReportParameter("zhizhaoArchCntA", zhizhaoArchCntA.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntB", zhizhaoArchCntB.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntC", zhizhaoArchCntC.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntD", zhizhaoArchCntD.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntE", zhizhaoArchCntE.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntF", zhizhaoArchCntF.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntG", zhizhaoArchCntG.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntH", zhizhaoArchCntH.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntI", zhizhaoArchCntI.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntJ", zhizhaoArchCntJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntK", zhizhaoArchCntK.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntL", zhizhaoArchCntL.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntM", zhizhaoArchCntM.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntN", zhizhaoArchCntN.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntO", zhizhaoArchCntO.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntP", zhizhaoArchCntP.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntQ", zhizhaoArchCntQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntR", zhizhaoArchCntR.ToString().Trim()));

                parameterList.Add(new ReportParameter("fenleiArchCntA", fenleiArchCntA.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntB", fenleiArchCntB.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntC", fenleiArchCntC.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntD", fenleiArchCntD.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntE", fenleiArchCntE.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntF", fenleiArchCntF.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntG", fenleiArchCntG.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntH", fenleiArchCntH.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntI", fenleiArchCntI.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntJ", fenleiArchCntJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntK", fenleiArchCntK.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntL", fenleiArchCntL.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntM", fenleiArchCntM.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntN", fenleiArchCntN.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntO", fenleiArchCntO.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntP", fenleiArchCntP.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntQ", fenleiArchCntQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntR", fenleiArchCntR.ToString().Trim()));


                parameterList.Add(new ReportParameter("zdArchCntA", zdArchCntA.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntB", zdArchCntB.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntC", zdArchCntC.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntD", zdArchCntD.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntE", zdArchCntE.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntF", zdArchCntF.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntG", zdArchCntG.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntH", zdArchCntH.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntI", zdArchCntI.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntJ", zdArchCntJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntK", zdArchCntK.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntL", zdArchCntL.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntM", zdArchCntM.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntN", zdArchCntN.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntO", zdArchCntO.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntP", zdArchCntP.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntQ", zdArchCntQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntR", zdArchCntR.ToString().Trim()));

                parameterList.Add(new ReportParameter("allArchCntA", allArchCntA.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntB", allArchCntB.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntC", allArchCntC.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntD", allArchCntD.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntE", allArchCntE.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntF", allArchCntF.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntG", allArchCntG.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntH", allArchCntH.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntI", allArchCntI.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntJ", allArchCntJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntK", allArchCntK.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntL", allArchCntL.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntM", allArchCntM.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntN", allArchCntN.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntO", allArchCntO.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntP", allArchCntP.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntQ", allArchCntQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntR", allArchCntR.ToString().Trim()));
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Split(' ')[0].Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().Split(' ')[0].Trim()));

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
            if (action == "取消")
            {
                return View("Home");
            }
            return View();
        }
        public ActionResult archiveClassify_all_TJ(string action, string type = "PDF")
        {
           
                LocalReport localReport = new LocalReport();
                

                //竣工档案
                var dsA = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("A")).Where(ad => ad.status == "7");
                var dsB = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("B")).Where(ad => ad.status == "7");
                var dsC = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("C")).Where(ad => ad.status == "7");
                var dsD = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("D")).Where(ad => ad.status == "7");
                var dsE = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("E")).Where(ad => ad.status == "7");
                var dsF = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("F")).Where(ad => ad.status == "7");
                var dsG = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("G")).Where(ad => ad.status == "7");
                var dsH = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("H")).Where(ad => ad.status == "7");
                var dsI = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("I")).Where(ad => ad.status == "7");
                var dsJ = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("J")).Where(ad => ad.status == "7");
                var dsK = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("K")).Where(ad => ad.status == "7");
                var dsL = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("L")).Where(ad => ad.status == "7");
                var dsM = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("M")).Where(ad => ad.status == "7");
                var dsN = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("N")).Where(ad => ad.status == "7");
                var dsO = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("O")).Where(ad => ad.status == "7");
                var dsP = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("P")).Where(ad => ad.status == "7");
                var dsQ = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("Q")).Where(ad => ad.status == "7");
                var dsR = ab.vw_categoryList.Where(ad => ad.archivesNo.Contains("R")).Where(ad => ad.status == "7");
                int jungongA = dsA.Count();
                int jungongB = dsB.Count();
                int jungongC = dsC.Count();
                int jungongD = dsD.Count();
                int jungongE = dsE.Count();
                int jungongF = dsF.Count();
                int jungongG = dsG.Count();
                int jungongH = dsH.Count();
                int jungongI = dsI.Count();
                int jungongJ = dsJ.Count();
                int jungongK = dsK.Count();
                int jungongL = dsL.Count();
                int jungongM = dsM.Count();
                int jungongN = dsN.Count();
                int jungongO = dsO.Count();
                int jungongP = dsP.Count();
                int jungongQ = dsQ.Count();
                int jungongR = dsR.Count();
                //管线档案
                var gxA = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("A")).Where(ad => ad.status == "7");
                var gxB = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("B")).Where(ad => ad.status == "7");
                var gxC = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("C")).Where(ad => ad.status == "7");
                var gxD = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("D")).Where(ad => ad.status == "7");
                var gxE = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("E")).Where(ad => ad.status == "7");
                var gxF = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("F")).Where(ad => ad.status == "7");
                var gxG = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("G")).Where(ad => ad.status == "7");
                var gxH = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("H")).Where(ad => ad.status == "7");
                var gxI = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("I")).Where(ad => ad.status == "7");
                var gxJ = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("J")).Where(ad => ad.status == "7");
                var gxK = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("K")).Where(ad => ad.status == "7");
                var gxL = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("L")).Where(ad => ad.status == "7");
                var gxM = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("M")).Where(ad => ad.status == "7");
                var gxN = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("N")).Where(ad => ad.status == "7");
                var gxO = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("O")).Where(ad => ad.status == "7");
                var gxP = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("P")).Where(ad => ad.status == "7");
                var gxQ = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("Q")).Where(ad => ad.status == "7");
                var gxR = gb.vw_gxcategoryList.Where(ad => ad.archivesNo.Contains("R")).Where(ad => ad.status == "7");
                int guanxianA = gxA.Count();
                int guanxianB = gxB.Count();
                int guanxianC = gxC.Count();
                int guanxianD = gxD.Count();
                int guanxianE = gxE.Count();
                int guanxianF = gxF.Count();
                int guanxianG = gxG.Count();
                int guanxianH = gxH.Count();
                int guanxianI = gxI.Count();
                int guanxianJ = gxJ.Count();
                int guanxianK = gxK.Count();
                int guanxianL = gxL.Count();
                int guanxianM = gxM.Count();
                int guanxianN = gxN.Count();
                int guanxianO = gxO.Count();
                int guanxianP = gxP.Count();
                int guanxianQ = gxQ.Count();
                int guanxianR = gxR.Count();
                //援川档案
                var ycS = db.vw_YuanChuancategoryList.Where(ad => ad.archivesNo.Contains("S")).Where(ad => ad.status=="7");
                int yuanchuanS = ycS.Count();
                //声像档案
                var shengxiang = cb.VideoArchives.Where(ad => ad.videoStatus == "4"); 
                int shengxiangProCnt = shengxiang.Count();   //声像总工程数
                List<VideoArchives> list4 = shengxiang.ToList();
                int videoArchCnt = 0;
                int photoArchCnt = 0;
                for (int i = 0; i < list4.Count(); i++)
                {
                if (list4[i].videoCassetteBoxCount != null && list4[i].videoCassetteBoxCount != "" && list4[i].photoBoxCount != null && list4[i].photoBoxCount != "")
                {
                    videoArchCnt += int.Parse(list4[i].videoCassetteBoxCount);   //视频案卷数
                    photoArchCnt += int.Parse(list4[i].photoBoxCount);      //照片案卷数
                }
                }
                int shengxiangArchCnt = videoArchCnt + photoArchCnt;
                //东部档案
                var eastA = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("A"));
                int eastArchCntA = eastA.Count();  //东部总案卷数
                var eastB = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("B"));
                int eastArchCntB = eastB.Count();  //东部总案卷数
                var eastC = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("C"));
                int eastArchCntC = eastC.Count();  //东部总案卷数
                var eastD = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("D"));
                int eastArchCntD = eastD.Count();  //东部总案卷数
                var eastE = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("E"));
                int eastArchCntE = eastE.Count();  //东部总案卷数
                var eastF = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("F"));
                int eastArchCntF = eastF.Count();  //东部总案卷数
                var eastG = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("G"));
                int eastArchCntG = eastG.Count();  //东部总案卷数
                var eastH = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("H"));
                int eastArchCntH = eastH.Count();  //东部总案卷数
                var eastI = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("I"));
                int eastArchCntI = eastI.Count();  //东部总案卷数
                var eastJ = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("J"));
                int eastArchCntJ = eastJ.Count();  //东部总案卷数
                var eastK = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("K"));
                int eastArchCntK = eastK.Count();  //东部总案卷数
                var eastL = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("L"));
                int eastArchCntL = eastL.Count();  //东部总案卷数
                var eastM = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("M"));
                int eastArchCntM = eastM.Count();  //东部总案卷数
                var eastN = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("N"));
                int eastArchCntN = eastN.Count();  //东部总案卷数
                var eastO = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("O"));
                int eastArchCntO = eastO.Count();  //东部总案卷数
                var eastP = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("P"));
                int eastArchCntP = eastP.Count();  //东部总案卷数
                var eastQ = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("Q"));
                int eastArchCntQ = eastQ.Count();  //东部总案卷数
                var eastR = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.licenceNo.Contains("东")).Where(ad => ad.archiveNo.Contains("R"));
                int eastArchCntR = eastR.Count();  //东部总案卷数
                //执照档案
                var zhizhaoA = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("A"));
                int zhizhaoArchCntA = zhizhaoA.Count();  //执照总案卷数
                var zhizhaoB = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("B"));
                int zhizhaoArchCntB = zhizhaoB.Count();  //执照总案卷数
                var zhizhaoC = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("C"));
                int zhizhaoArchCntC = zhizhaoC.Count();  //执照总案卷数
                var zhizhaoD = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("D"));
                int zhizhaoArchCntD = zhizhaoD.Count();  //执照总案卷数
                var zhizhaoE = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("E"));
                int zhizhaoArchCntE = zhizhaoE.Count();  //执照总案卷数
                var zhizhaoF = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("F"));
                int zhizhaoArchCntF = zhizhaoF.Count();  //执照总案卷数
                var zhizhaoG = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("G"));
                int zhizhaoArchCntG = zhizhaoG.Count();  //执照总案卷数
                var zhizhaoH = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("H"));
                int zhizhaoArchCntH = zhizhaoH.Count();  //执照总案卷数
                var zhizhaoI = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("I"));
                int zhizhaoArchCntI = zhizhaoI.Count();  //执照总案卷数
                var zhizhaoJ = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("J"));
                int zhizhaoArchCntJ = zhizhaoJ.Count();  //执照总案卷数
                var zhizhaoK = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("K"));
                int zhizhaoArchCntK = zhizhaoK.Count();  //执照总案卷数
                var zhizhaoL = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("L"));
                int zhizhaoArchCntL = zhizhaoL.Count();  //执照总案卷数
                var zhizhaoM = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("M"));
                int zhizhaoArchCntM = zhizhaoM.Count();  //执照总案卷数
                var zhizhaoN = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("N"));
                int zhizhaoArchCntN = zhizhaoN.Count();  //执照总案卷数
                var zhizhaoO = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("O"));
                int zhizhaoArchCntO = zhizhaoO.Count();  //执照总案卷数
                var zhizhaoP = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("P"));
                int zhizhaoArchCntP = zhizhaoP.Count();  //执照总案卷数
                var zhizhaoQ = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("Q"));
                int zhizhaoArchCntQ = zhizhaoQ.Count();  //执照总案卷数
                var zhizhaoR = db.OtherArchives.Where(ad => ad.status == "RK").Where(ad => ad.classTypeID == 1).Where(ad => ad.archiveNo.Contains("R"));
                int zhizhaoArchCntR = zhizhaoR.Count();  //执照总案卷数
                //分类档案
                var fenleiA = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("A"));
                int fenleiArchCntA = fenleiA.Count();  //分类总案卷数
                var fenleiB = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("B"));
                int fenleiArchCntB = fenleiB.Count();  //分类总案卷数
                var fenleiC = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("C"));
                int fenleiArchCntC = fenleiC.Count();  //分类总案卷数
                var fenleiD = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("D"));
                int fenleiArchCntD = fenleiD.Count();  //分类总案卷数
                var fenleiE = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("E"));
                int fenleiArchCntE = fenleiE.Count();  //分类总案卷数
                var fenleiF = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("F"));
                int fenleiArchCntF = fenleiF.Count();  //分类总案卷数
                var fenleiG = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("G"));
                int fenleiArchCntG = fenleiG.Count();  //分类总案卷数
                var fenleiH = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("H"));
                int fenleiArchCntH = fenleiH.Count();  //分类总案卷数
                var fenleiI = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("I"));
                int fenleiArchCntI = fenleiI.Count();  //分类总案卷数
                var fenleiJ = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("J"));
                int fenleiArchCntJ = fenleiJ.Count();  //分类总案卷数
                var fenleiK = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("K"));
                int fenleiArchCntK = fenleiK.Count();  //分类总案卷数
                var fenleiL = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("L"));
                int fenleiArchCntL = fenleiL.Count();  //分类总案卷数
                var fenleiM = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("M"));
                int fenleiArchCntM = fenleiM.Count();  //分类总案卷数
                var fenleiN = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("N"));
                int fenleiArchCntN = fenleiN.Count();  //分类总案卷数
                var fenleiO = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("O"));
                int fenleiArchCntO = fenleiO.Count();  //分类总案卷数
                var fenleiP = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("P"));
                int fenleiArchCntP = fenleiP.Count();  //分类总案卷数
                var fenleiQ = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("Q"));
                int fenleiArchCntQ = fenleiQ.Count();  //分类总案卷数
                var fenleiR = db.OtherArchives.Where(ad => ad.classTypeID == 3).Where(ad => ad.archiveNo.Contains("R"));
                int fenleiArchCntR = fenleiR.Count();  //分类总案卷数
                //征地档案
                var zdA = ab.zdArchive.Where(ad => ad.archiveNo.Contains("A"));
                int zdArchCntA = zdA.Count();  //征地总案卷数
                var zdB = ab.zdArchive.Where(ad => ad.archiveNo.Contains("B"));
                int zdArchCntB = zdB.Count();  //征地总案卷数
                var zdC = ab.zdArchive.Where(ad => ad.archiveNo.Contains("C"));
                int zdArchCntC = zdC.Count();  //征地总案卷数
                var zdD = ab.zdArchive.Where(ad => ad.archiveNo.Contains("D"));
                int zdArchCntD = zdD.Count();  //征地总案卷数
                var zdE = ab.zdArchive.Where(ad => ad.archiveNo.Contains("E"));
                int zdArchCntE = zdE.Count();  //征地总案卷数
                var zdF = ab.zdArchive.Where(ad => ad.archiveNo.Contains("F"));
                int zdArchCntF = zdF.Count();  //征地总案卷数
                var zdG = ab.zdArchive.Where(ad => ad.archiveNo.Contains("G"));
                int zdArchCntG = zdG.Count();  //征地总案卷数
                var zdH = ab.zdArchive.Where(ad => ad.archiveNo.Contains("H"));
                int zdArchCntH = zdH.Count();  //征地总案卷数
                var zdI = ab.zdArchive.Where(ad => ad.archiveNo.Contains("I"));
                int zdArchCntI = zdI.Count();  //征地总案卷数
                var zdJ = ab.zdArchive.Where(ad => ad.archiveNo.Contains("J"));
                int zdArchCntJ = zdJ.Count();  //征地总案卷数
                var zdK = ab.zdArchive.Where(ad => ad.archiveNo.Contains("K"));
                int zdArchCntK = zdK.Count();  //征地总案卷数
                var zdL = ab.zdArchive.Where(ad => ad.archiveNo.Contains("L"));
                int zdArchCntL = zdL.Count();  //征地总案卷数
                var zdM = ab.zdArchive.Where(ad => ad.archiveNo.Contains("M"));
                int zdArchCntM = zdM.Count();  //征地总案卷数
                var zdN = ab.zdArchive.Where(ad => ad.archiveNo.Contains("N"));
                int zdArchCntN = zdN.Count();  //征地总案卷数
                var zdO = ab.zdArchive.Where(ad => ad.archiveNo.Contains("O"));
                int zdArchCntO = zdO.Count();  //征地总案卷数
                var zdP = ab.zdArchive.Where(ad => ad.archiveNo.Contains("P"));
                int zdArchCntP = zdP.Count();  //征地总案卷数
                var zdQ = ab.zdArchive.Where(ad => ad.archiveNo.Contains("Q"));
                int zdArchCntQ = zdQ.Count();  //征地总案卷数
                var zdR = ab.zdArchive.Where(ad => ad.archiveNo.Contains("R"));
                int zdArchCntR = zdR.Count();  //征地总案卷数
                //全部档案
                int allArchCntA = jungongA+ guanxianA+ eastArchCntA+ zhizhaoArchCntA+ fenleiArchCntA+ zdArchCntA ;
                int allArchCntB = jungongB + guanxianB + eastArchCntB + zhizhaoArchCntB + fenleiArchCntB + zdArchCntB;
                int allArchCntC = jungongC + guanxianC + eastArchCntC + zhizhaoArchCntC + fenleiArchCntC + zdArchCntC;
                int allArchCntD = jungongD + guanxianD + eastArchCntD + zhizhaoArchCntD + fenleiArchCntD + zdArchCntD;
                int allArchCntE = jungongE + guanxianE + eastArchCntE + zhizhaoArchCntE + fenleiArchCntE + zdArchCntE;
                int allArchCntF = jungongF + guanxianF + eastArchCntF + zhizhaoArchCntF + fenleiArchCntF + zdArchCntF;
                int allArchCntG = jungongG + guanxianG + eastArchCntG + zhizhaoArchCntG + fenleiArchCntG + zdArchCntG;
                int allArchCntH = jungongH + guanxianH + eastArchCntH + zhizhaoArchCntH + fenleiArchCntH + zdArchCntH;
                int allArchCntI = jungongI + guanxianI + eastArchCntI + zhizhaoArchCntI + fenleiArchCntI + zdArchCntI;
                int allArchCntJ = jungongJ + guanxianJ + eastArchCntJ + zhizhaoArchCntJ + fenleiArchCntJ + zdArchCntJ;
                int allArchCntK = jungongK + guanxianK + eastArchCntK + zhizhaoArchCntK + fenleiArchCntK + zdArchCntK;
                int allArchCntL = jungongL + guanxianL + eastArchCntL + zhizhaoArchCntL + fenleiArchCntL + zdArchCntL;
                int allArchCntM = jungongM + guanxianM + eastArchCntM + zhizhaoArchCntM + fenleiArchCntM + zdArchCntM;
                int allArchCntN = jungongN + guanxianN + eastArchCntN + zhizhaoArchCntN + fenleiArchCntN + zdArchCntN;
                int allArchCntO = jungongO + guanxianO + eastArchCntO + zhizhaoArchCntO + fenleiArchCntO + zdArchCntO;
                int allArchCntP = jungongP + guanxianP + eastArchCntP + zhizhaoArchCntP + fenleiArchCntP + zdArchCntP;
                int allArchCntQ = jungongQ + guanxianQ + eastArchCntQ + zhizhaoArchCntQ + fenleiArchCntQ + zdArchCntQ;
                int allArchCntR = jungongR + guanxianR + eastArchCntR + zhizhaoArchCntR + fenleiArchCntR + zdArchCntR+ shengxiangArchCnt;

                localReport.ReportPath = Server.MapPath("~/Report/guanlikeTJ/archiveClassify_all_TJ.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("DataSet1", ycS);
                localReport.DataSources.Add(reportDataSource);

                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("jungongA", jungongA.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongB", jungongB.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongC", jungongC.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongD", jungongD.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongE", jungongE.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongF", jungongF.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongG", jungongG.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongH", jungongH.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongI", jungongI.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongJ", jungongJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongK", jungongK.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongL", jungongL.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongM", jungongM.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongN", jungongN.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongO", jungongO.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongP", jungongP.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongQ", jungongQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongR", jungongR.ToString().Trim()));

                parameterList.Add(new ReportParameter("guanxianA", guanxianA.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianB", guanxianB.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianC", guanxianC.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianD", guanxianD.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianE", guanxianE.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianF", guanxianF.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianG", guanxianG.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianH", guanxianH.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianI", guanxianI.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianJ", guanxianJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianK", guanxianK.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianL", guanxianL.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianM", guanxianM.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianN", guanxianN.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianO", guanxianO.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianP", guanxianP.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianQ", guanxianQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("guanxianR", guanxianR.ToString().Trim()));

                //parameterList.Add(new ReportParameter("yuanchuanS", yuanchuanS.ToString().Trim()));
                parameterList.Add(new ReportParameter("shengxiangArchCnt", shengxiangArchCnt.ToString().Trim()));

                parameterList.Add(new ReportParameter("eastArchCntA", eastArchCntA.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntB", eastArchCntB.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntC", eastArchCntC.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntD", eastArchCntD.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntE", eastArchCntE.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntF", eastArchCntF.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntG", eastArchCntG.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntH", eastArchCntH.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntI", eastArchCntI.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntJ", eastArchCntJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntK", eastArchCntK.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntL", eastArchCntL.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntM", eastArchCntM.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntN", eastArchCntN.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntO", eastArchCntO.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntP", eastArchCntP.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntQ", eastArchCntQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("eastArchCntR", eastArchCntR.ToString().Trim()));

                parameterList.Add(new ReportParameter("zhizhaoArchCntA", zhizhaoArchCntA.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntB", zhizhaoArchCntB.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntC", zhizhaoArchCntC.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntD", zhizhaoArchCntD.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntE", zhizhaoArchCntE.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntF", zhizhaoArchCntF.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntG", zhizhaoArchCntG.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntH", zhizhaoArchCntH.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntI", zhizhaoArchCntI.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntJ", zhizhaoArchCntJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntK", zhizhaoArchCntK.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntL", zhizhaoArchCntL.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntM", zhizhaoArchCntM.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntN", zhizhaoArchCntN.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntO", zhizhaoArchCntO.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntP", zhizhaoArchCntP.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntQ", zhizhaoArchCntQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("zhizhaoArchCntR", zhizhaoArchCntR.ToString().Trim()));

                parameterList.Add(new ReportParameter("fenleiArchCntA", fenleiArchCntA.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntB", fenleiArchCntB.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntC", fenleiArchCntC.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntD", fenleiArchCntD.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntE", fenleiArchCntE.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntF", fenleiArchCntF.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntG", fenleiArchCntG.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntH", fenleiArchCntH.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntI", fenleiArchCntI.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntJ", fenleiArchCntJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntK", fenleiArchCntK.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntL", fenleiArchCntL.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntM", fenleiArchCntM.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntN", fenleiArchCntN.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntO", fenleiArchCntO.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntP", fenleiArchCntP.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntQ", fenleiArchCntQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("fenleiArchCntR", fenleiArchCntR.ToString().Trim()));


                parameterList.Add(new ReportParameter("zdArchCntA", zdArchCntA.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntB", zdArchCntB.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntC", zdArchCntC.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntD", zdArchCntD.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntE", zdArchCntE.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntF", zdArchCntF.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntG", zdArchCntG.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntH", zdArchCntH.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntI", zdArchCntI.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntJ", zdArchCntJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntK", zdArchCntK.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntL", zdArchCntL.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntM", zdArchCntM.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntN", zdArchCntN.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntO", zdArchCntO.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntP", zdArchCntP.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntQ", zdArchCntQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("zdArchCntR", zdArchCntR.ToString().Trim()));

                parameterList.Add(new ReportParameter("allArchCntA", allArchCntA.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntB", allArchCntB.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntC", allArchCntC.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntD", allArchCntD.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntE", allArchCntE.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntF", allArchCntF.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntG", allArchCntG.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntH", allArchCntH.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntI", allArchCntI.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntJ", allArchCntJ.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntK", allArchCntK.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntL", allArchCntL.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntM", allArchCntM.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntN", allArchCntN.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntO", allArchCntO.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntP", allArchCntP.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntQ", allArchCntQ.ToString().Trim()));
                parameterList.Add(new ReportParameter("allArchCntR", allArchCntR.ToString().Trim()));

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

    }


}