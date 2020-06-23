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
using System.Globalization;
using Newtonsoft.Json;

namespace urban_archive.Controllers
{
    public class guanzhangController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        private guanzhangtongjiEntities ab = new guanzhangtongjiEntities();
        private OfficeEntities cb = new OfficeEntities();
        private VideoArchiveEntities bb = new VideoArchiveEntities();
        private PlanArchiveEntities eb = new PlanArchiveEntities();
        // GET: guanzhang
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult jungongtongji(string action, string type = "PDF")
        {
            if (action == "查询竣工档案统计信息")
            {
                LocalReport localReport = new LocalReport();
                string startdate = Request.Form["startdata"];
                string enddate = Request.Form["startdata"];
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                var ds1 = ab.vw_printTongji.Where(a => a.dateReceived >= DataFrom).Where(a => a.dateReceived <= DataTo);
                localReport.ReportPath = Server.MapPath("~/Report/guanzhang/jungongtongji.rdlc");
                ReportDataSource reportDataSource1 = new ReportDataSource("jungongtongji", ds1);
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
            return View();
        }
        public ActionResult shengxiangtongji(string action, string type = "PDF")
        {
            if (action == "查询声像档案统计信息")
            {
                LocalReport localReport = new LocalReport();
                string startdate = Request.Form["startdata"];
                string enddate = Request.Form["startdata"];
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                var ds1 = bb.VideoContractSheet.Where(a => a.fillDate >= DataFrom).Where(a => a.fillDate <= DataTo);
                localReport.ReportPath = Server.MapPath("~/Report/guanzhang/shengxiangtongji.rdlc");
                ReportDataSource reportDataSource1 = new ReportDataSource("shengxiangtongji", ds1);
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
            return View();
        }
        public ActionResult totaltongji(string action, string type = "PDF")
        {
            if (action == "查询全部档案统计信息")
            {
                LocalReport localReport = new LocalReport();
                string dateS = Request.Form["startdata"];
                string dateE = Request.Form["enddata"];
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                //竣工档案
                var jungong = db.vw_projectList.Where(ad => ad.dateReceived >= DataFrom).Where(ad => ad.dateReceived <= DataTo);
                int jungongProCnt = jungong.Count();   //竣工总工程数
                List<vw_projectList> list1 = jungong.ToList();
                long? jungongArchCnt = 0;
                for (int i = 0; i < list1.Count(); i++)
                {
                    if (list1[i].archivesCount == null || list1[i].archivesCount == "")
                    {
                        list1[i].archivesCount = "0";

                        
                    }//竣工案卷数
                    string n = list1[i].archivesCount.Trim();
                    long cnt = long.Parse(n);
                    jungongArchCnt += cnt;

                }
                var jungongarea = db.vw_projectList.Where(ad => ad.dateReceived >= DataFrom).Where(ad => ad.dateReceived <= DataTo);
                List<vw_projectList> list2 = jungongarea.ToList();
                double? jungongArea = 0;
                for (int i = 0; i < list2.Count(); i++)
                {
                    if (list2[i].buildingArea == null)
                    {
                        list2[i].buildingArea = 0;
                    }
                    jungongArea += list2[i].buildingArea;   //竣工工程面积
                }
                //馆内
                var jungongchargeNei = db.Charger.Where(ad => ad.chargeTime >= DataFrom).Where(ad => ad.chargeTime <= DataTo).Where(a => a.chargeClassify == 8);
                int jungongChargeN = jungongchargeNei.Count();   //馆内整理工程数
                List<Charger> list3 = jungongchargeNei.ToList();
                decimal? jungongtotalCharge = 0;
                decimal? jungongtheoryCharge = 0;
                for (int i = 0; i < list3.Count(); i++)
                {
                    if (list3[i].totalExpense == null)
                    {
                        list3[i].totalExpense = 0;
                    }
                    if (list3[i].theoryExpense == null)
                    {
                        list3[i].theoryExpense = 0;
                    }
                    jungongtotalCharge += list3[i].totalExpense;   //馆内整理收费
                    jungongtheoryCharge += list3[i].theoryExpense;   //馆内理论整理收费
                }
                //馆外
                var jungongchargeWai = db.Charger.Where(ad => ad.chargeTime >= DataFrom).Where(ad => ad.chargeTime <= DataTo).Where(a => a.chargeClassify == 9);
                int jungongChargeW = jungongchargeWai.Count();   //馆外整理工程数
                List<Charger> list4 = jungongchargeWai.ToList();
                decimal? jungongtotalChargeW = 0;
                decimal? jungongtheoryChargeW = 0;
                for (int i = 0; i < list4.Count(); i++)
                {
                    if (list4[i].totalExpense == null)
                    {
                        list4[i].totalExpense = 0;
                    }
                    if (list4[i].theoryExpense == null)
                    {
                        list4[i].theoryExpense = 0;
                    }
                    jungongtotalChargeW += list4[i].totalExpense;   //馆外整理收费
                    jungongtheoryChargeW += list4[i].theoryExpense;   //馆外理论整理收费
                }
                //声像档案
                var shengxiang = bb.VideoArchives.Where(ad => ad.dateReceived >= DataFrom).Where(ad => ad.dateReceived <= DataTo); ;
                int shengxiangProCnt = shengxiang.Count();   //声像总工程数
                List<VideoArchives> list5 = shengxiang.ToList();
                int? videoArchCnt = 0;
                int? photoArchCnt = 0;
                double? videoarea = 0;
                for (int i = 0; i < list5.Count(); i++)
                {
                    videoArchCnt += int.Parse(list5[i].videoCassetteBoxCount);   //视频案卷数
                    photoArchCnt += int.Parse(list5[i].photoBoxCount);      //照片案卷数
                    videoarea += list3[i].buildingArea;      //声像工程面积
                }
                var videocharge = db.Charger.Where(ad => ad.chargeTime >= DataFrom).Where(ad => ad.chargeTime <= DataTo).Where(a => a.chargeClassify == 3);
                List<Charger> list6 = videocharge.ToList();
                decimal? videototalCharge = 0;
                decimal? videotheoryCharge = 0;
                for (int i = 0; i < list6.Count(); i++)
                {
                    if (list6[i].totalExpense == null)
                    {
                        list6[i].totalExpense = 0;
                    }
                    if (list6[i].theoryExpense == null)
                    {
                        list6[i].theoryExpense = 0;
                    }
                    videototalCharge += list6[i].totalExpense;   //声像档案盒资料费
                    videotheoryCharge += list6[i].theoryExpense;   //声像理论档案盒资料费
                }
                var videogongben = db.Charger.Where(ad => ad.chargeTime >= DataFrom).Where(ad => ad.chargeTime <= DataTo).Where(a => a.chargeClassify == 7);
                List<Charger> list7 = videogongben.ToList();
                decimal? videototalgongben = 0;
                decimal? videotheorygongben = 0;
                for (int i = 0; i < list7.Count(); i++)
                {
                    if (list7[i].totalExpense == null)
                    {
                        list7[i].totalExpense = 0;
                    }
                    if (list7[i].theoryExpense == null)
                    {
                        list7[i].theoryExpense = 0;
                    }
                    videototalgongben += list7[i].totalExpense;   //声像录制工本费
                    videotheorygongben += list7[i].theoryExpense;   //声像理论录制工本费
                }
                //档案借阅收费汇总,包括查卷费和证明费
                var jieyuecharge = db.Charger.Where(ad => ad.chargeTime >= DataFrom).Where(ad => ad.chargeTime <= DataTo).Where(a => (a.chargeClassify == 5) ||(a.chargeClassify == 6) || (a.chargeClassify == 9));
                List<Charger> list8 = jieyuecharge.ToList();
                decimal? jieyuefei = 0;
                for (int i = 0; i < list8.Count(); i++)
                {
                    if (list8[i].totalExpense == null)
                    {
                        list8[i].totalExpense = 0;
                    }
                    jieyuefei += list8[i].totalExpense;   //借阅费汇总
                }
                //档案复印收费汇总
                var fuyincharge = db.Charger.Where(ad => ad.chargeTime >= DataFrom).Where(ad => ad.chargeTime <= DataTo).Where(a => (a.chargeClassify == 4));
                List<Charger> list9 = fuyincharge.ToList();
                decimal? fuyinfei = 0;
                for (int i = 0; i < list9.Count(); i++)
                {
                    if (list9[i].totalExpense == null)
                    {
                        list9[i].totalExpense = 0;
                    }
                    fuyinfei += list9[i].totalExpense;   //fuyin费汇总
                }
                //收费合计
                decimal? totalFee = jungongtotalCharge + jungongtotalChargeW + videototalCharge + videototalgongben + jieyuefei + fuyinfei;
                //规划档案
                var guihua = eb.PlanProject.Where(ad => ad.dateReceived >= DataFrom).Where(ad => ad.dateReceived <= DataTo);
                int guihuaProCnt = guihua.Count();   //规划总工程数
                int? NoS = guihua.Min(d => d.seqNo);
                int? NoE = guihua.Max(d => d.seqNo);
                var guihuabox = eb.PlanArchiveBox.Where(ad => ad.ID >= NoS).Where(ad => ad.ID <= NoE);
                int guihuaArchCnt = guihuabox.Count();  //规划案卷数



                localReport.ReportPath = Server.MapPath("~/Report/guanzhang/totaltongji.rdlc");
                ReportDataSource reportDataSource1 = new ReportDataSource("total", guihua);
                localReport.DataSources.Add(reportDataSource1);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongProCnt", jungongProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongArchCnt", jungongArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongArea", jungongArea.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongChargeN", jungongChargeN.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongtotalCharge", jungongtotalCharge.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongtheoryCharge", jungongtheoryCharge.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongChargeW", jungongChargeW.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongtotalChargeW", jungongtotalChargeW.ToString().Trim()));
                parameterList.Add(new ReportParameter("jungongtheoryChargeW", jungongtheoryChargeW.ToString().Trim()));
                parameterList.Add(new ReportParameter("shengxiangProCnt", shengxiangProCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("videoArchCnt", videoArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("photoArchCnt", photoArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("videoarea", videoarea.ToString().Trim()));
                parameterList.Add(new ReportParameter("videototalCharge", videototalCharge.ToString().Trim()));
                parameterList.Add(new ReportParameter("videotheoryCharge", videotheoryCharge.ToString().Trim()));
                parameterList.Add(new ReportParameter("videototalgongben", videototalgongben.ToString().Trim()));
                parameterList.Add(new ReportParameter("videotheorygongben", videotheorygongben.ToString().Trim()));
                parameterList.Add(new ReportParameter("jieyuefei", jieyuefei.ToString().Trim()));
                parameterList.Add(new ReportParameter("fuyinfei", fuyinfei.ToString().Trim()));
                parameterList.Add(new ReportParameter("totalFee", totalFee.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaArchCnt", guihuaArchCnt.ToString().Trim()));
                parameterList.Add(new ReportParameter("guihuaProCnt", guihuaProCnt.ToString().Trim()));
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
        public ActionResult CaiWuShouFeiList(string action, int? page)      //财务收费列表
        {
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Text = "缴费单位/个人", Value = "1"},
                  new SelectListItem { Text = "编号", Value = "3"}
            };
            ViewBag.dropdowmlist = new SelectList(list, "Value", "Text");
            List<SelectListItem> list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "全部科室", Value = "0"},
                  new SelectListItem { Text = "业务科", Value = "1"},
                  new SelectListItem { Text = "声像科", Value = "3"},
            };
            ViewBag.Department = new SelectList(list1, "Value", "Text");
            List<SelectListItem> list2 = new List<SelectListItem>
            {
                new SelectListItem { Text = "未收费", Value = "0"},
                new SelectListItem { Text = "已收费", Value = "1"},
            };
            ViewBag.ischarge = new SelectList(list2, "Value", "Text");

            if (action == "查询")
            {
                string n = Request.Form["dropdowmlist"];
                string m = Request.Form["search"];
                string a = Request.Form["Department"];
                string b = Request.Form["ischarge"];
                if (a == "0")
                {
                    if (n == "1")
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.unitName.Contains(m)
                                         where ad.isCharge == true
                                         where ad.whereTransfer == 1
                                         where ad.centiCnt != -1
                                         orderby ad.ID descending
                                         select ad;
                            ViewBag.result = JsonConvert.SerializeObject(chaxun);
                            return View();
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.unitName.Contains(m)
                                         where ad.isCharge == false
                                         where ad.whereTransfer == 1
                                         where ad.centiCnt != -1
                                         orderby ad.ID descending
                                         select ad;
                            ViewBag.result = JsonConvert.SerializeObject(chaxun);
                            return View();
                        }
                    }
                    if (n == "0")
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.isCharge == true
                                         where ad.whereTransfer == 1
                                         where ad.centiCnt != -1
                                         orderby ad.ID descending
                                         select ad;
                            ViewBag.result = JsonConvert.SerializeObject(chaxun);
                            return View();
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.isCharge == false
                                         where ad.whereTransfer == 1
                                         where ad.centiCnt != -1
                                         orderby ad.ID descending
                                         select ad;
                            ViewBag.result = JsonConvert.SerializeObject(chaxun);
                            return View();
                        }
                    }
                    else
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.seqNo.Contains(m)
                                         where ad.isCharge == true
                                         where ad.whereTransfer == 1
                                         where ad.centiCnt != -1
                                         orderby ad.ID descending
                                         select ad;
                            ViewBag.result = JsonConvert.SerializeObject(chaxun);
                            return View();
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.seqNo.Contains(m)
                                         where ad.isCharge == false
                                         where ad.whereTransfer == 1
                                         where ad.centiCnt != -1
                                         orderby ad.ID descending
                                         select ad;
                            ViewBag.result = JsonConvert.SerializeObject(chaxun);
                            return View();
                        }
                    }
                }
                else   //其他部门
                {
                    if (n == "1")
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.unitName.Contains(m)
                                         where ad.isCharge == true
                                         where ad.fromDepartment == a
                                         where ad.whereTransfer == 1
                                         where ad.centiCnt != -1
                                         orderby ad.ID descending
                                         select ad;
                            ViewBag.result = JsonConvert.SerializeObject(chaxun);
                            return View();
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.unitName.Contains(m)
                                         where ad.isCharge == false
                                         where ad.fromDepartment == a
                                         where ad.whereTransfer == 1
                                         where ad.centiCnt != -1
                                         orderby ad.ID descending
                                         select ad;
                            ViewBag.result = JsonConvert.SerializeObject(chaxun);
                            return View();
                        }
                    }
                    if (n == "0")
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.isCharge == true
                                         where ad.whereTransfer == 1
                                         where ad.centiCnt != -1
                                         orderby ad.ID descending
                                         select ad;
                            ViewBag.result = JsonConvert.SerializeObject(chaxun);
                            return View();
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.isCharge == false
                                         where ad.whereTransfer == 1
                                         where ad.centiCnt != -1
                                         orderby ad.ID descending
                                         select ad;
                            ViewBag.result = JsonConvert.SerializeObject(chaxun);
                            return View();
                        }
                    }
                    else
                    {
                        if (b == "1")
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.seqNo.Contains(m)
                                         where ad.isCharge == true
                                         where ad.fromDepartment == a
                                         where ad.centiCnt != -1
                                         orderby ad.ID descending
                                         select ad;
                            ViewBag.result = JsonConvert.SerializeObject(chaxun);
                            return View();
                        }
                        else
                        {
                            var chaxun = from ad in db.vw_charge
                                         where ad.seqNo.Contains(m)
                                         where ad.isCharge == false
                                         where ad.fromDepartment == a
                                         where ad.centiCnt != -1
                                         orderby ad.ID descending
                                         select ad;
                            ViewBag.result = JsonConvert.SerializeObject(chaxun);
                            return View();
                        }
                    }
                }
            }
            var feiyongchaxun = db.vw_charge.Where(a => a.fromDepartment == "1").Where(a => a.centiCnt != -1).OrderByDescending(a => a.ID);
            ViewBag.result = JsonConvert.SerializeObject(feiyongchaxun);
            return View();
        }
        public ActionResult JieSuan(string id, string action)      //费用结算
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var charge = db.vw_charge.Where(a => a.seqNo == id).ToList();
            decimal? total = 0;
            for (int i = 0; i < charge.Count(); i++)
            {
                total = total + charge[i].totalExpense;
            }
            ViewBag.total = total;
            ViewBag.unitName = charge.ToList().First().unitName;
            ViewBag.seqNo = charge.ToList().First().seqNo;
            if (charge == null)
            {
                return HttpNotFound();
            }
            if (action == "返回")
            {
                return RedirectToAction("CaiWuShouFeiList");
            }
            return View(charge);
        }
        public ActionResult CodeEdit(string SearchString)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "工程序号", Value = "1"},
                new SelectListItem { Text = "设计单位", Value = "2" },
                new SelectListItem { Text = "施工单位", Value = "3" },
                new SelectListItem { Text = "项目顺序号", Value = "4" },
                new SelectListItem { Text = "起始排架号", Value = "5" },
                new SelectListItem { Text = "档号", Value = "6" },
                 new SelectListItem { Text = "整理人", Value = "7" },
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            string t = Request.Form["SelectedID"];
            ViewBag.CurrentFilter = SearchString;
            List<SelectListItem> list1 = new List<SelectListItem> {
                new SelectListItem { Text = "全部", Value = "0"},
                new SelectListItem { Text = "接收档案", Value = "1"},
                new SelectListItem { Text = "审核", Value = "2" },
                new SelectListItem { Text = "通过审核", Value = "3" },
                new SelectListItem { Text = "整理", Value = "4" },
                new SelectListItem { Text = "编号", Value = "5" },
                new SelectListItem { Text = "录入", Value = "6" },
                new SelectListItem { Text = "等待入库", Value = "7" },
                 new SelectListItem { Text = "入库", Value = "8" },
            };
            ViewBag.SelectedID1 = new SelectList(list1, "Value", "Text");
            string t1 = Request.Form["SelectedID1"];
            var vwprojectlist = db.vw_projectList.Where(a => a.status != "8" || a.status != "2");

            if (!String.IsNullOrEmpty(SearchString))
            {
                int n = int.Parse(Request.Form["SelectedID"]);
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", t);
                ViewBag.SelectedID1 = new SelectList(list1, "Value", "Text", t1);
                if (t1 == "0")
                {
                    switch (n)
                    {
                        case 0:
                            vwprojectlist = vwprojectlist.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                            break;
                        case 1:
                            long search = Convert.ToInt32(SearchString);
                            vwprojectlist = vwprojectlist.Where(ad => ad.projectNo == search);//根据地点搜索
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
                            vwprojectlist = vwprojectlist.Where(ad => ad.startArchiveNo.Contains(SearchString));//根据责任书编号搜索
                            break;
                        case 7:
                            vwprojectlist = vwprojectlist.Where(ad => ad.collator.Contains(SearchString));//根据整理人搜索
                            break;
                    }
                }
                else
                {
                    switch (n)
                    {
                        case 0:
                            vwprojectlist = vwprojectlist.Where(ad => ad.projectName.Contains(SearchString)).Where(a => a.status == t1);//根据工程名称搜索
                            break;
                        case 1:
                            long search = Convert.ToInt32(SearchString);
                            vwprojectlist = vwprojectlist.Where(ad => ad.projectNo == search).Where(a => a.status == t1);//根据地点搜索
                            break;
                        case 2:
                            vwprojectlist = vwprojectlist.Where(ad => ad.disignOrganization.Contains(SearchString)).Where(a => a.status == t1);//根据建设单位搜索
                            break;
                        case 3:
                            vwprojectlist = vwprojectlist.Where(ad => ad.constructionOrganization.Contains(SearchString)).Where(a => a.status == t1);//根据施工单位搜索
                            break;
                        case 4:
                            vwprojectlist = vwprojectlist.Where(ad => ad.paperProjectSeqNo.ToString().Trim() == SearchString).Where(a => a.status == t1);//根据工程序号搜索
                            break;
                        case 5:
                            vwprojectlist = vwprojectlist.Where(ad => ad.startPaijiaNo.Contains(SearchString)).Where(a => a.status == t1);
                            break;
                        case 6:
                            vwprojectlist = vwprojectlist.Where(ad => ad.startArchiveNo.Contains(SearchString)).Where(a => a.status == t1);//根据责任书编号搜索
                            break;
                        case 7:
                            vwprojectlist = vwprojectlist.Where(ad => ad.collator.Contains(SearchString)).Where(a => a.status == t1);//根据整理人搜索
                            break;
                    }
                }
            }
            vwprojectlist = vwprojectlist.OrderByDescending(s => s.projectID);// 默认按项目顺序号排列
            ViewBag.result = JsonConvert.SerializeObject(vwprojectlist);
            return View();
        }
        public ActionResult bianhao(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vw_projectList vwprojectlist = db.vw_projectList.Where(a => a.projectID == id).First();

            if (vwprojectlist == null)
            {
                return HttpNotFound();
            }
            ArchiveCertificate archivecertificate = new ArchiveCertificate();
            archivecertificate.projectID = vwprojectlist.projectID;
            archivecertificate.projectName = vwprojectlist.projectName;
            archivecertificate.developmentOrganization = vwprojectlist.developmentOrganization;
            archivecertificate.dateArchive = vwprojectlist.dateArchive;
            ViewBag.tel = "82879324";
            archivecertificate.projectStartDate = vwprojectlist.projectStartDate;
            archivecertificate.projectFinishDate = vwprojectlist.projectFinishDate;
            archivecertificate.recipient = vwprojectlist.recipient;
            archivecertificate.submitPerson = vwprojectlist.submitPerson;
            archivecertificate.archiveCertificateNo = vwprojectlist.archiveCertificateNo;
            archivecertificate.telphoneSubmitPerson = vwprojectlist.telphoneSubmitPerson;
            archivecertificate.location = vwprojectlist.location;
            archivecertificate.datebianhao = DateTime.Today;
            return View(archivecertificate);
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult bianhao(ArchiveCertificate archivecertificate, string action)
        {

            if (action == "提交")
            {
                if (ModelState.IsValid)
                {
                    ab.ArchiveCertificate.Add(archivecertificate);
                    ab.SaveChanges();
                    return Content("<script>alert('已成功提交！');window.location.href='./CodeEdit'</script>");
                }
            }
            if (action == "返回")
            {
                return RedirectToAction("CodeEdit");
            }
            return View(archivecertificate);
        }
        public ActionResult zhengjian(string SearchString ,string action)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "建设单位", Value = "1"},
                new SelectListItem { Text = "工程地点", Value = "2"},
                new SelectListItem { Text = "归档时间", Value = "3"},
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            ViewBag.CurrentFilter = SearchString;
            var zhengjian = ab.ArchiveCertificate.OrderByDescending(a => a.projectID);
            if (action == "查找")
            {
                string n = Request.Form["SelectedID"];
                ViewBag.SelectedID = new SelectList(list, "Value", "Text",n);
                if (n == "0")
                {
                    var chaxun = zhengjian.Where(a => a.projectName.Contains(SearchString)).OrderByDescending(a => a.projectName);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "1")
                {
                    var chaxun = zhengjian.Where(a => a.developmentOrganization.Contains(SearchString)).OrderByDescending(a => a.projectName);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "2")
                {
                    var chaxun = zhengjian.Where(a => a.location.Contains(SearchString)).OrderByDescending(a => a.projectName);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                }
                if (n == "3")
                {
                    DateTime date = DateTime.Parse(SearchString);
                    var chaxun = zhengjian.Where(a => a.dateArchive==date).OrderByDescending(a => a.projectName);
                    ViewBag.result = JsonConvert.SerializeObject(chaxun);
                    return View();
                } 
            }
            ViewBag.result = JsonConvert.SerializeObject(zhengjian);
            return View();
        }
        public ActionResult hegezheng(string ID, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = ab.ArchiveCertificate.Where(a => a.archiveCertificateNo == ID);
            localReport.ReportPath = Server.MapPath("~/Report/guanzhang/hegezheng.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("zhengjian", ds1);
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
        public ActionResult yuyanshou(string ID, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = ab.ArchiveCertificate.Where(a => a.archiveCertificateNo == ID);
            localReport.ReportPath = Server.MapPath("~/Report/guanzhang/yuyanshou.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("zhengjian", ds1);
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
    }
}
