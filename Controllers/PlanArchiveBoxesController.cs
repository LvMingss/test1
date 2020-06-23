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
using Microsoft.AspNet.Identity;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
namespace urban_archive.Controllers
{
    public class PlanArchiveBoxesController : Controller
    {
        private UrbanConEntities db_urban = new UrbanConEntities();
        private PlanArchiveEntities db_plan = new PlanArchiveEntities();
        private UrbanUsersEntities db_user = new UrbanUsersEntities();
        private OfficeEntities db_office = new OfficeEntities();
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult JuanNeiMuLu(long id, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db_office.vw_PlanProjectList
                      where ad.seqNo == id
                      select ad;
            List<vw_PlanProjectList> list = ds1.ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].fileNo != null)
                    list[i].fileNo = list[i].fileNo.Trim();
            }
            var ds = list;
            localReport.ReportPath = Server.MapPath("~/Report/guihua/JuanNeiMuLu.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("GongChengFengPi", ds);
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

        //规划案卷封皮
        public ActionResult GuiHuaAnJuanFengPi(long id, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db_plan.vw_printPlanArchiveBoxFengpi
                      where ad.ID == id
                      select ad;
            string seqno = db_plan.PlanProject.Where(a => a.ID == id).First().totalSeqNo;
            localReport.ReportPath = Server.MapPath("~/Report/guihua/GuiHuaAnJuanFengPi.rdlc");
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
        //规划工程封皮
        public ActionResult GuiHuaGongChengFengPi(long id, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db_office.vw_PlanProjectList
                      where ad.ID == id
                      select ad;
            List<vw_PlanProjectList> list = ds1.ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].fileNo != null)
                    list[i].fileNo = list[i].fileNo.Trim();
            }
            var ds = list;
            localReport.ReportPath = Server.MapPath("~/Report/guihua/GuiHuaGongChengFengPi.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("GH", ds);
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

        //规划案卷工程封皮
        public ActionResult GuiHuaAnJuanGongChengFengPi(long id, string type = "PDF")
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
        public ActionResult beikaobiao(long id, string type = "PDF")
        {
            LocalReport localReport = new LocalReport();
            var ds1 = from ad in db_plan.vw_PlanBeiKaoBiao
                      where ad.ID == id
                      select ad;
            localReport.ReportPath = Server.MapPath("~/Report/guihua/beikaobiao.rdlc");
            ReportDataSource reportDataSource1 = new ReportDataSource("guihuaBKB", ds1);
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
        // GET: PlanArchiveBoxes
        public ActionResult Index(string status, int? classifyID, string sortOrder, string currentFilter, string searchString, int? page, int? SelectedID)
        {
            ViewBag.bh = "display:none";
            ViewBag.quanxian = "display:none";

            ViewBag.dqzt_bh = "";
            ViewBag.dqzt_gd = "";
            ViewBag.dqzt_sh = "";
            ViewBag.dqzt_rh = "";
            ViewBag.dqzt_qb = "";

            if (status != null)
            {
                ViewBag.type = "submit";
            }
            else
            {
                ViewBag.type = "hidden";
                ViewBag.status = "hidden";
            }
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            ViewBag.classifyID = classifyID;
            
            ViewBag.status1 = status;
            var planArchiveBox = from ad in db_plan.PlanArchiveBox
                                 //where ad.lururen == user.UserName
                                 //orderby ad.yearNo descending
                                 select ad;
            if (user.RoleName == "科员")
            {
                ViewBag.classify = db_plan.PlanArchiveClassify.Find(classifyID).classifyName;//类别名
                ViewBag.quanxian = "float:left;";
                planArchiveBox = planArchiveBox.Where(ad => ad.lururen == user.UserName).Where(ad=>ad.classifyID== classifyID);
            }
                       
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "档案顺序号", Value = "0"},
                new SelectListItem { Text = "盒号", Value = "1"},
                new SelectListItem { Text = "案卷题名", Value = "2" },
                new SelectListItem { Text = "编制日期", Value = "3" }
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text",SelectedID);
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值

            int t = SelectedID.GetValueOrDefault();
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (t)
                {
                    case 0:
                        planArchiveBox = planArchiveBox.Where(ad => ad.seqNo.ToString().Contains(searchString));//根据档案顺序号搜索
                        break;
                    case 1:
                        planArchiveBox = planArchiveBox.Where(ad => ad.boxNo.Contains(searchString));//根据盒号搜索
                        break;
                    case 2:
                        planArchiveBox = planArchiveBox.Where(ad => ad.archiveTitle.Contains(searchString));//根据案卷题名搜索
                        break;
                    case 3:
                        planArchiveBox = planArchiveBox.Where(ad => ad.bianzhiDate.Contains(searchString));//根据编制日期搜索
                        break;
                }
            }
            if (status == null)
            {
                ViewBag.dqzt_qb = "dangqianzhuangtai";
                planArchiveBox = planArchiveBox.OrderByDescending(s => s.yearNo).Take(1000);
            }
            else if (status.Trim() == "GD")
            {
                planArchiveBox = planArchiveBox.Where(s => s.status.Trim() == "GD").OrderByDescending(s => s.yearNo).Take(1000);
                ViewBag.status = "归档";
                ViewBag.dqzt_gd = "dangqianzhuangtai";
            }
            else if (status.Trim() == "BH")
            {
                planArchiveBox = planArchiveBox.Where(s => s.status == "BH").OrderByDescending(s => s.yearNo).Take(1000);
                ViewBag.status = "编号";
                //ViewBag.bh = "float:left";
                ViewBag.bh = "display:inline";
                ViewBag.dqzt_bh = "dangqianzhuangtai";
            }
            else if (status.Trim() == "SH")
            {
                planArchiveBox = planArchiveBox.Where(s => s.status == "SH").OrderByDescending(s => s.yearNo).Take(1000);
                ViewBag.status = "入库";
                ViewBag.dqzt_sh = "dangqianzhuangtai";
            }
            else if (status.Trim() == "RK")
            {
                planArchiveBox = planArchiveBox.Where(s => s.status == "RK").OrderByDescending(s => s.yearNo).Take(1000);
                ViewBag.status = "返回";
                ViewBag.dqzt_rk = "dangqianzhuangtai";
            }
            else
            {
                ViewBag.dqzt_qb= "dangqianzhuangtai";
                planArchiveBox = planArchiveBox.OrderByDescending(s => s.yearNo).Take(1000);
            }
            var select_year = planArchiveBox.GroupBy(p => new { p.yearNo }).Select(g => g.FirstOrDefault());
            ViewBag.yearNo = new SelectList(select_year, "yearNo", "yearNo");
            planArchiveBox = planArchiveBox.OrderByDescending(s => s.seqNo);
            ViewBag.result = JsonConvert.SerializeObject(planArchiveBox);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(String action,int? classifyID,string yearNo)
        {
            //ViewData["pagename"] = "PlanArchiveBoxes";
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            if (action == "归档")
            {
                var planArchiveBox_temp = db_plan.PlanArchiveBox.Where(d => d.classifyID == classifyID).Where(d => d.status == "GD").ToList();
                foreach (var item in planArchiveBox_temp)//
                {
                    item.status = "SH";
                    db_plan.Entry(item).State = EntityState.Modified;
                }
                db_plan.SaveChanges();
                return RedirectToAction("Index", new { status = "SH", classifyID= classifyID });
            }
            else if (action == "编号")
            {
                if (yearNo == null)
                {
                    return Content("<script >alert('年份为空，请选择年份！');window.history.back();</script >");

                }
                String year = yearNo.Trim();//这个应该有个下拉框
                int? max_seqNo = db_plan.PlanArchiveBox.Max(s=>s.seqNo);//获得当前最大seqNo

                var planArchiveBox_temp = from ad in db_plan.PlanArchiveBox
                                    where ad.classifyID == classifyID//定点啥的，从左边选择栏传进来
                                    where ad.status == "BH"
                                    where ad.yearNo == year
                                    orderby ad.boxID,ad.archiveBoxNo
                                    select ad;//筛选出状态为待编号的规划档案（注意档案类别）
                if (planArchiveBox_temp != null)
                {
                    foreach (var item in planArchiveBox_temp)
                    {
                        item.seqNo = max_seqNo+1;//盒序号递增
                        item.status = "GD";//状态修改->GD

                        var planproject_temp = from ad in db_plan.PlanProject
                                                  where ad.seqNo==item.ID
                                                  orderby ad.seqNo1, ad.juanneiSeqNo//按照seqNo1,卷内序号排序，编号
                                                  select ad;//筛选当前盒号内的规划工程
                        string max_TotalSeqNo = db_plan.PlanProject.Max(s => s.totalSeqNo).Substring(0, 8);//获得当前最大totalSeqNo,取前八位
                        int temp = int.Parse(max_TotalSeqNo) + 1;//增1
                        foreach (var item1 in planproject_temp)
                        {
                            item1.seqNo1 = max_seqNo+1;//planproject的seqNo1就是planArchiveBox的seqNo
                            string cur_max = temp.ToString().PadLeft(8, '0');//左边填充0，补齐8位
                            string unit = item1.developmentUnit.ToString();
                            if (!unit.Contains('-'))
                            {
                                item1.totalSeqNo = cur_max;//totalSeqNo赋值为当前最大号+1
                                temp += 1;
                            }
                            else
                            {
                                string volcount = unit.Substring(unit.IndexOf("-") - 1, 1);
                                string volno = unit.Substring(unit.IndexOf("-") + 1,3);
                                item1.totalSeqNo = cur_max+"-"+ volno.PadLeft(3,'0');
                                if (volcount == volno)//当00014715-006这种号都编完了，再把00014715自增
                                    temp += 1;
                            }                         
                            db_plan.Entry(item1).State = EntityState.Modified;
                        }
                        max_seqNo += 1;//最大seqNo增1
                        db_plan.Entry(item).State = EntityState.Modified;
                    }
                    db_plan.SaveChanges();                      
                }
                return RedirectToAction("Index", new { status = "GD" ,classifyID = classifyID });
            }
            else if (action == "入库")
            {
                if (user.RoleName == "科员")
                {
                    return Content("<script >alert('当前用户没有权限！');window.history.back();</script >");
                }
                var planArchiveBox_temp = db_plan.PlanArchiveBox.Where(d => d.status == "SH").ToList();
                foreach (var item in planArchiveBox_temp)//
                {
                    item.status = "RK";
                    db_plan.Entry(item).State = EntityState.Modified;
                }
                db_plan.SaveChanges();
                return RedirectToAction("Index", new { status = "RK" });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        // GET: PlanArchiveBoxes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanArchiveBox planArchiveBox = db_plan.PlanArchiveBox.Find(id);
            List<SelectListItem> list = new List<SelectListItem> { };
            for (int i = 1; i < 6; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = i.ToString() + "厘米";
                item.Value = i.ToString();
                list.Add(item);
            }
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", planArchiveBox.ArchiveThick);
            //档案密级
            ViewBag.securityID = new SelectList(db_urban.SecurityClassification, "securityID", "securityName", planArchiveBox.securityID);
            //保管年限(注意这里与)
            ViewBag.retentionPeriodID = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", planArchiveBox.retentionPeriodID);
            //是否为内部文件
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" },
                new SelectListItem { Text = "公开/内部", Value = "2" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", planArchiveBox.isNeibu.ToString().Trim());
            if (planArchiveBox == null)
            {
                return HttpNotFound();
            }
            return View(planArchiveBox);
        }

        // GET: PlanArchiveBoxes/Create
        public ActionResult Create(int? ID,int? classifyID,string id3)
        {
            var UserID = User.Identity.GetUserId();//获取当前用户
            var user = db_user.AspNetUsers.Find(UserID);
            ViewBag.lururen = user.UserName;
            var test = from ad in db_plan.PlanArchiveBox
                       where ad.status == "LR"
                       where ad.seqNo==0
                       where ad.classifyID== classifyID
                       where ad.lururen == user.UserName
                       orderby ad.ID descending
                       select ad;//查找状态为录入且录入人为当前用户的记录，并按year排(降)序 
            int clas = Convert.ToInt32(classifyID);
            var test1 = from b in db_plan.PlanArchiveClassify//为了拼接出盒号
                        where b.classifyID ==clas
                        select b;
            PlanArchiveBox planArchiveBox = test.FirstOrDefault<PlanArchiveBox>();
            if(ID!=0&&ID!=null&&id3=="0")//加入标识符id3的目的是判断它是从添加下一卷的按钮来的，而不是从录入完卷内目录返回而来
            {
                var test2 = from a in db_plan.PlanArchiveBox
                            where a.ID == ID
                            select a;
               
                planArchiveBox = test2.First();

                int? cnt = Convert.ToInt32(test2.First().archiveBoxNo) + 1;
                planArchiveBox.archiveBoxNo = cnt;
            }
            if(ID != 0 && ID != null)//卷内文件录入完毕，返回案卷录入，保留之前录入的信息
            {
                var test3 = from a in db_plan.PlanArchiveBox
                            where a.ID == ID
                            select a;

                planArchiveBox = test3.First();

               
            }
            ViewBag.ID = 0;
            ViewBag.classifyID = classifyID;
            ViewBag.classify = test1.First().classifyName.Trim();
            ViewBag.box =test1.First().classifyName.Trim()+"-"+test1.First().classifySX.Trim();//盒号（注意是显示盒号的一部分，并不是数据库中完整的盒号boxNo）
            return View(planArchiveBox);
        }

        // POST: PlanArchiveBoxes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "isNeibu,seqNo,boxNo,archiveNo,regisNo,paijiaNo,archiveTitle,archiveCode,bianzhiUnit,bianzhiDate,securityID,retentionPeriodID,archiveBoxCnt,archiveBoxNo,pageCnt,inchCnt,classifyID,isArchive,textMatirial,drawing,photo,status,yearNo,classify,boxID,projectCnt,projectNo,ID,ArchiveThick,storeLocation,receiveTime,luruTime,classSeqNo,isYD,lururen")] PlanArchiveBox planArchiveBox, int ID, int classifyID, string box,string isYD,String action)
        {
           
                         
          
            if (action == "录入该卷工程")
            {
              
                    return RedirectToAction("Create", "PlanProjects1", new {classifyID = planArchiveBox.classifyID,id = 0, id1 = 0 });
               
            }
            if (action == "删除该案卷")
            {
                PlanArchiveBox planArchiveBox_temp = db_plan.PlanArchiveBox.Find(ID);
                if (planArchiveBox_temp != null)
                {
                    db_plan.PlanArchiveBox.Remove(planArchiveBox_temp);
                    var planProject_temp = db_plan.PlanProject.Where(d => d.archiveID == ID).ToList();
                    foreach (var item in planProject_temp)//删除案卷的同时删除相关卷内目录
                    {
                        db_plan.PlanProject.Remove(item);
                    }
                    db_plan.SaveChanges();
                    return Content("<script >alert('删除成功！');window.history.back();</script >");
                }
             

            }
          
            else if (action == "添加下一卷")
            {
                //planArchiveBox.ID = ID;
               
                if (planArchiveBox.archiveBoxNo>= planArchiveBox.archiveBoxCnt)
                {
                    return RedirectToAction("Create", new {ID=0, classifyID = planArchiveBox.classifyID});
                }
                else
                {
                 
                    return RedirectToAction("Create",new { ID= planArchiveBox.ID,classifyID=classifyID,id3=0});
                }
            }
            if (action == "打印备考表")
            {
                return RedirectToAction("beikaobiao", new { id = ID });
            }
           
            if (action == "打印案卷封皮")
            {
                return RedirectToAction("GuiHuaAnJuanFengPi", new { id = ID });
            }
            if (action == "打印案卷（工程）封皮")
            {
                return RedirectToAction("GuiHuaAnJuanGongChengFengPi", new { id = ID });
            }
            if (action == "打印工程封皮")
            {
                return RedirectToAction("GuiHuaGongChengFengPi", new { id = ID });
            }
            if (action == "打印卷内目录")
            {
                return RedirectToAction("JuanNeiMuLu", new { id = ID });
            }
            return View(planArchiveBox);
        }
        public string baocun(string ID ,string box, string boxID,string classifyID, string boxNo, string archiveBoxCnt, string archiveBoxNo,string yearNo,string name)
        {
            string flag = "0";//初始状态为0
            if(name=="baocun")
            {
                PlanArchiveBox planArchive = new PlanArchiveBox();
              
                planArchive.seqNo =0;
               
                planArchive.storeLocation = "青岛城建档案馆";
                planArchive.boxID = boxID.Trim();
                planArchive.boxNo = boxNo.Trim();
               
                planArchive.archiveCode ="437402";
               
                planArchive.bianzhiUnit ="青岛市规划局";
              
                planArchive.yearNo = yearNo;
    
               
                planArchive.archiveBoxCnt = Int32.Parse(archiveBoxCnt);
                planArchive.archiveBoxNo = Int32.Parse(archiveBoxNo);
            
            
                long max_ID = db_plan.PlanArchiveBox.Max(d => d.ID);
                planArchive.ID = int.Parse(max_ID.ToString()) + 1;//新记录的ID+1，值唯一
                planArchive.isNeibu = "是";
                flag= planArchive.ID.ToString();//将盒子的ID传到前台，以备后用
                planArchive.status = "LR";
                planArchive.classifyID=Int32.Parse(classifyID.Trim());
        
                //try
                //{
                    flag =planArchive.ID.ToString().Trim();
                    db_plan.PlanArchiveBox.Add(planArchive);
                    db_plan.SaveChanges();
                    
                   return flag;//保存成功
                //}
                //catch(Exception ex)
                //{
                //    flag = "2";
                //  return flag;//保存失败
                //}

            }
          
         
         
            return flag;
        }
        public string luru(string ID)
        {
            int id = Int32.Parse(ID.Trim());
            var plan = from a in db_plan.PlanArchiveBox
                       where a.ID == id
                       select a;
            plan.First().status = "BH";//状态改为编号
            db_plan.Entry(plan.First()).State = EntityState.Modified;
            db_plan.SaveChanges();
            string flag = "";
            return flag;
        }
        // GET: PlanArchiveBoxes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanArchiveBox planArchiveBox = db_plan.PlanArchiveBox.Find(id);
            if (planArchiveBox == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> list = new List<SelectListItem> { };
            for (int i = 1; i < 6; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = i.ToString() + "厘米";
                item.Value = i.ToString();
                list.Add(item);
            }
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", planArchiveBox.ArchiveThick);
            //是否异地
            //档案密级
            ViewBag.securityID = new SelectList(db_urban.SecurityClassification, "securityID", "securityName", planArchiveBox.securityID);
            //保管年限(注意这里与)
            ViewBag.retentionPeriodID = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", planArchiveBox.retentionPeriodID);
            //是否为内部文件
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" },
                new SelectListItem { Text = "公开/内部", Value = "2" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", planArchiveBox.isNeibu.ToString().Trim());
            ViewBag.box = planArchiveBox.boxNo;//将boxNo的盒号部分传递给前台显示
            if (planArchiveBox.status.ToString().Trim() == "LR")
            {
                ViewData["div1"] = "display:block";
                ViewData["fengpi"] = true;
                ViewData["beikaobiao"] = false;
                ViewData["juanneimulu"] = true;
                //jsy
                return View(planArchiveBox);
            }
            if (planArchiveBox.ToString().Trim() == "GD")
            {
                ViewData["div1"] = "display:block";
                ViewData["fengpi"] = false;
                ViewData["beikaobiao"] = false;
                ViewData["juanneimulu"] = false;
                //jsy
                return View(planArchiveBox);
            }
            return View(planArchiveBox);
        }

        // POST: PlanArchiveBoxes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string action,[Bind(Include = "isNeibu,seqNo,boxNo,archiveNo,regisNo,paijiaNo,archiveTitle,archiveCode,bianzhiUnit,bianzhiDate,securityID,retentionPeriodID,archiveBoxCnt,archiveBoxNo,pageCnt,inchCnt,classifyID,isArchive,textMatirial,drawing,photo,status,yearNo,classify,boxID,projectCnt,projectNo,ID,ArchiveThick,storeLocation,receiveTime,luruTime,classSeqNo,isYD")] PlanArchiveBox planArchiveBox)
        {
            List<SelectListItem> list = new List<SelectListItem> { };
            for (int i = 1; i < 6; i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = i.ToString() + "厘米";
                item.Value = i.ToString();
                list.Add(item);
            }
            ViewBag.ArchiveThick = new SelectList(list, "Value", "Text", planArchiveBox.ArchiveThick);
            //是否异地
            //档案密级
            ViewBag.securityID = new SelectList(db_urban.SecurityClassification, "securityID", "securityName", planArchiveBox.securityID);
            //保管年限(注意这里与)
            ViewBag.retentionPeriodID = new SelectList(db_urban.RetentionPeriod, "retentionPeriodNo", "retentionPeriodName", planArchiveBox.retentionPeriodID);
            //是否为内部文件
            List<SelectListItem> listNeibu = new List<SelectListItem> {
                new SelectListItem { Text = "公开", Value = "0"},
                new SelectListItem { Text = "内部", Value = "1" },
                new SelectListItem { Text = "公开/内部", Value = "2" }
            };
            ViewBag.isNeibu = new SelectList(listNeibu, "Value", "Text", planArchiveBox.isNeibu.ToString().Trim());
            ViewBag.box = planArchiveBox.boxNo;//将boxNo的盒号部分传递给前台显示
            if (action== "修改并提交")
            {
                if (ModelState.IsValid)
                {
                    var classifyID = planArchiveBox.classifyID;
                    db_plan.Entry(planArchiveBox).State = EntityState.Modified;
                    db_plan.SaveChanges();
                    ViewData["div1"] = "display:block";
                    ViewData["fengpi"] = true;
                    ViewData["beikaobiao"] = false;
                    ViewData["juanneimulu"] = true;
                    return Content("<script>alert('修改成功');window.location.href='/PlanArchiveBoxes/Index/?classifyID=" + classifyID + "';</script>");
                }
            }
            if (action == "该卷文件目录")
            {
                
                return RedirectToAction("Index", "PlanProjects", new { archiveID = planArchiveBox.ID, classifyID = planArchiveBox.classifyID });
            }
            //if (action == "删除该案卷")
            //{
            //    PlanArchiveBox planArchiveBox_remove = db_plan.PlanArchiveBox.Find(planArchiveBox.ID);
            //    db_plan.PlanArchiveBox.Remove(planArchiveBox_remove);
            //    var planProject_temp = db_plan.PlanProject.Where(d => d.archiveID == planArchiveBox.ID).ToList();
            //    foreach (var item in planProject_temp)//删除案卷的同时删除相关卷内目录
            //    {
            //        db_plan.PlanProject.Remove(item);
            //    }
            //    db_plan.SaveChanges();
            //    return Content("<script>alert('删除成功');window.location.href='" + url + "';</script>");

            //}
            if (action == "打印备考表")
            {
                return RedirectToAction("beikaobiao", new { id = planArchiveBox.ID });
            }

            if (action == "打印案卷封皮")
            {
                return RedirectToAction("GuiHuaAnJuanFengPi", new { id = planArchiveBox.ID });
            }
            if (action == "打印案卷（工程）封皮")
            {
                return RedirectToAction("GuiHuaAnJuanGongChengFengPi", new { id = planArchiveBox.ID });
            }
            if (action == "打印工程封皮")
            {
                return RedirectToAction("GuiHuaGongChengFengPi", new { id = planArchiveBox.ID });
            }
            if (action == "打印卷内目录")
            {
                return RedirectToAction("JuanNeiMuLu", new { id = planArchiveBox.ID });
            }
            if (action == "返回")
            {
                var UserID = User.Identity.GetUserId();//获取当前用户
                var user = db_user.AspNetUsers.Find(UserID).RoleName;
                var classifyID = planArchiveBox.classifyID;
                if (user == "高级用户")
                {
                    classifyID = null;
                }
                return RedirectToAction("Index",new {status= planArchiveBox.status.Trim(), archiveID = planArchiveBox.ID, classifyID = classifyID });//返回上一页面
            }
            return View(planArchiveBox);
        }

        // GET: PlanArchiveBoxes/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    PlanArchiveBox planArchiveBox = db_plan.PlanArchiveBox.Find(id);
        //    if (planArchiveBox == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(planArchiveBox);
        //}

        //// POST: PlanArchiveBoxes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id,int? classifyID)
        {
            PlanArchiveBox planArchiveBox = db_plan.PlanArchiveBox.Find(id);
            db_plan.PlanArchiveBox.Remove(planArchiveBox);
            db_plan.SaveChanges();
            return Content("<script>alert('已成功删除！');window.location.href='/PlanArchiveBoxes/Index/?classifyID=" + classifyID + "';</script>");
            //return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db_plan.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
