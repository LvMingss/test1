using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using urban_archive.Models;
using System.Configuration;
using System.Data.SqlTypes;
using Newtonsoft.Json;

namespace urban_archive.Controllers
{
    public class gxLaterMaterialsController : Controller
    {
        // GET: LaterMaterials
        private gxArchivesContainer ab = new gxArchivesContainer();
        private UrbanConEntities db = new UrbanConEntities();
        public ActionResult MeterialsSend(string SearchString)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "责任书编号", Value = "0"},
                new SelectListItem { Text = "工程名称", Value = "1"},
                new SelectListItem { Text = "建设单位", Value = "2" },
                new SelectListItem { Text = "工程地点", Value = "3" },
                new SelectListItem { Text = "工程序号", Value = "4" },
                new SelectListItem { Text = "施工单位", Value = "5" },
                new SelectListItem { Text = "设计单位", Value = "6" },
                new SelectListItem { Text = "监理单位", Value = "7" },
                new SelectListItem { Text = "项目顺序号", Value = "8" },
            };
            ViewBag.SelectedID = new SelectList(list, "Value", "Text");
            //第一个参数为指定要放入SelectList的数据项，第二个参数为选中时使用的值（键值），第三个参数为每一项要显示的文本，第四项为默认的键值
            string n = Request.Form["SelectedID"];
            ViewBag.CurrentFilter = SearchString;
            var vwprojectFile = from ad in ab.vw_gxProjectStatus
                                select ad;


            if (!String.IsNullOrEmpty(SearchString))
            {
                int t = int.Parse(n);
                ViewBag.SelectedID = new SelectList(list, "Value", "Text",n);
                switch (t)
                {
                    case 0:
                        vwprojectFile = vwprojectFile.Where(ad => ad.contractNo.Contains(SearchString));//根据责任书编号搜索
                        break;
                    case 1:
                        vwprojectFile = vwprojectFile.Where(ad => ad.projectName.Contains(SearchString));//根据工程名称搜索
                        break;
                    case 2:
                        vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization.Contains(SearchString));//根据建设单位搜索
                        break;
                    case 3:
                        vwprojectFile = vwprojectFile.Where(ad => ad.location.Contains(SearchString));//根据工程地点
                        break;
                    case 4:
                        int Search = Convert.ToInt32(SearchString);
                        vwprojectFile = vwprojectFile.Where(ad => ad.projectNo == Search); //根据工程序号
                        break;
                    case 5:
                        vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization.Contains(SearchString));//根据施工单位
                        break;
                    case 6:
                        vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization.Contains(SearchString));//根据设计单位
                        break;
                    case 7:
                        vwprojectFile = vwprojectFile.Where(ad => ad.jianliUnit.Contains(SearchString));//根据监理单位
                        break;
                    case 8:
                        long seq = Convert.ToInt32(SearchString);
                        vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo == seq);//根据项目顺序号
                        break;
                }
            }
            // 默认按责任书编号排
            vwprojectFile = vwprojectFile.OrderByDescending(s => s.projectNo);
            ViewBag.result = JsonConvert.SerializeObject(vwprojectFile);
            return View();

        }
        public ActionResult SeeProject(long ?id,string action)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "否", Value = "false"},
                new SelectListItem { Text = "是", Value = "true"},
            };

            
            var test = from ad in ab.vw_gxprojectList
                       where (ad.projectID == id)
                       select ad;
            ViewBag.yidi = new SelectList(list, "Value", "Text", test.First().isYD);
            vw_gxprojectList projectProfile = test.First();
            if(action=="返回")
            {
                return RedirectToAction("MeterialsSend");
            }
            if (projectProfile == null)
            {
                return HttpNotFound();
            }
            if (action == "文件下载")
            {
                return RedirectToAction("DownLoadFile", "ProjectManagement", new { id = id });
            }
            return View(projectProfile);
            
        }
        public ActionResult SeeContract(string id,string action)
        {
            if (action == "返回")
            {
                return RedirectToAction("MeterialsSend");
            }
            if (id == null)
            {
                return Content("<script >alert('该工程无责任书');window.history.back();</script >");
            }

            else
            {
                var test = from ad in ab.gxContractInfo
                           where (ad.contractNo == id)
                           select ad;
            if(test.Count()!=0)
                {
                    return View(test.First());
                }
                else
                {
                    return Content("<script >alert('责任书编号有误，请检查数据库！');window.history.back();</script >");
                }
            }
        }
        public ActionResult LingquYiJianshu(long?id,string id2)
        {
            if(id!=null)
            {
                var vwArchiveList = from a in ab.vw_gxarchiveQueryList
                                    where a.projectID == id
                                    select a;
                if(vwArchiveList.Count()!=0)
                {

                }
                if(id2!=null&&id2!="")
                {
                    
                    
                        if (id2 == "True")
                        {
                          var liqu = from b in ab.gxLingquInterchangeForm
                                   where b.projectID == id
                                   select b;
                          if (liqu.Count() != 0)
                          {
                            ViewData["button1"] = true;
                            ViewData["button2"] = false;
                            ViewData["workUnit"] = liqu.First().workUnit;
                            ViewData["lingquPerson"] = liqu.First().lingquPerson;
                            ViewData["contractTel"] = liqu.First().contractTel;
                            ViewData["fafangPerson"] = liqu.First().fafangPerson;
                            ViewData["lingquDate"] = liqu.First().lingquDate;
                          }

                        }
                        else
                        {
                            ViewData["button1"] = false;
                            ViewData["button2"] = true;
                            ViewData["lingquDate"] = DateTime.Today.Date.ToString("yyyy-MM-dd");
                        }
                    
                }
                ViewData["fafangPerson"] = User.Identity.Name;
                ViewData["workUnit"] = vwArchiveList.First().developmentOrganization;
                return View(vwArchiveList.First());
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //传入
        public ActionResult LingquYiJianshu(long projectID,string workUnit,string lingquPerson, string contractTel,string fafangPerson, DateTime? lingquDate, string isLingquYijiaoshu,string  action)
        {
            if (action == "返回")
            {
                return RedirectToAction("MeterialsSend");
            }
            
            var project = from b in ab.gxProjectInfo
                           where b.projectID == projectID
                           select b;
            gxProjectInfo d = project.First();
            if(action=="确定")
            {
                gxLingquInterchangeForm lingqu = new gxLingquInterchangeForm();
                lingqu.workUnit = workUnit.Trim();
                lingqu.lingquPerson = lingquPerson.Trim();
                lingqu.contractTel = contractTel.Trim();
                lingqu.fafangPerson = fafangPerson.Trim();
               
                string str = Convert.ToDateTime(lingquDate).ToString("yyyy-MM-dd");
                lingqu.lingquDate = DateTime.ParseExact(str.Trim(), "yyyy-MM-dd", null).Date;
                lingqu.projectID = projectID;
                var maxid = ab.gxLingquInterchangeForm.Max(a => a.ID);
                lingqu.ID = maxid + 1;
                d.isLingquYijiaoshu = true;
                ab.gxLingquInterchangeForm.Add(lingqu);
                ab.Entry(d).State = EntityState.Modified;
                ab.SaveChanges();
                return Content("<script >alert('确认成功');window.history.back();</script >");

            }
            if(action=="更新")
            {
                var vwArchiveList = from a in ab.gxLingquInterchangeForm
                                    where a.projectID == projectID
                                    select a;
                if (vwArchiveList.Count() == 0)
                {
                    return Content("<script >alert('请先领取');window.history.back();</script >");
                }
                gxLingquInterchangeForm c = vwArchiveList.First();
                c.workUnit = workUnit.Trim();
                c.lingquPerson = lingquPerson.Trim();
                c.contractTel = contractTel.Trim();
                c.fafangPerson = fafangPerson.Trim();
                string str = Convert.ToDateTime(lingquDate).ToString("yyyy-MM-dd");
                c.lingquDate = DateTime.ParseExact(str.Trim(), "yyyy-MM-dd", null).Date;
                if (isLingquYijiaoshu.ToString()=="true")
                {
                    d.isLingquYijiaoshu =true;
                }
                else
                {
                    d.isLingquYijiaoshu = false;
                }
                ab.Entry(c).State = EntityState.Modified;
                ab.Entry(d).State = EntityState.Modified;
                ab.SaveChanges();
                return Content("<script >alert('修改成功');window.history.back();</script >");
            }
              
            return View();
        }
        public ActionResult FaFangHeGeZheng(long? id, string id2)
        {
            string strCertificateNo = ConfigurationManager.AppSettings["qingdaoNo"];
            if (id != null)
            {
                var vwArchiveList = from a in ab.vw_gxarchiveQueryList
                                    where a.projectID == id
                                    select a;
                var paper1 = from b in ab.gxPaperArchives
                            orderby b.archiveCertificateNo descending
                            select b;
                var paper2 = from b in ab.gxPaperArchives
                             where b.projectID == id
                             orderby b.archiveCertificateNo descending
                             select b;
               string flag = "";
                if (id2!=null&&id2!="")
                {
                    if (id2 == "True")
                    {

                        var liqu = from b in ab.gxFafangArchiveCertificate
                                   where b.projectID == id
                                   select b;
                        if (liqu.Count() != 0)
                        {
                            var paper = from c in ab.gxPaperArchives
                                        where c.projectID == id
                                        select c.archiveCertificateNo;
                            ViewData["button1"] = true;
                            ViewData["button2"] = false;
                            flag = "true";
                            ViewData["workUnit"] = liqu.First().workUnit;
                            ViewData["lingquPerson"] = liqu.First().lingquPerson;
                            ViewData["contractTel"] = liqu.First().contractTel;
                           
                            ViewData["lingquDate"] = liqu.First().lingquDate;

                            ViewData["archiveCertificateNo"] = paper.First();
                        }

                    }
                    else
                    {
                        ViewData["button1"] = false;
                        flag = "false";
                        ViewData["button2"] = true;
                        ViewData["lingquDate"] = DateTime.Today.Date.ToString("yyyy-MM-dd");
                    }
                }
                ViewData["fafangPeroson"] = User.Identity.Name;
                ViewData["workUnit"] = vwArchiveList.First().developmentOrganization;
               
                if (flag.Trim()=="false")
                {
                    string certificate = paper1.First().archiveCertificateNo.Substring(4, 3);
                    long archive = Int32.Parse(certificate) + 1;
                     string strOrderNo = archive.ToString("D3");
                    strCertificateNo = strCertificateNo + DateTime.Now.Year.ToString() + strOrderNo;
                    ViewData["archiveCertificateNo"] = strCertificateNo;

                }
                else
                {
                    if(paper2.Count()!=0)
                    {
                        ViewData["archiveCertificateNo"] = paper2.First().archiveCertificateNo;
                    }
                }
                return View(vwArchiveList.First());
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //传入
        public ActionResult FaFangHeGeZheng(string archiveCertificateNo, long projectID, string workUnit, string lingquPerson, string contractTel, string fafangPeroson, DateTime? lingquDate, string isFafangHegezheng, string action)
        {
            if (action == "返回")
            {
                return RedirectToAction("MeterialsSend");
            }
            
            var project = from b in ab.gxProjectInfo
                          where b.projectID == projectID
                          select b;
            gxProjectInfo d = project.First();
            var paperArchive = from f in ab.gxPaperArchives
                               where f.projectID==projectID
                               select f;
            gxPaperArchives g = paperArchive.First();
            if (action == "确定")
            {
                gxFafangArchiveCertificate fafang = new gxFafangArchiveCertificate();
                fafang.workUnit = workUnit;
                fafang.lingquPerson = lingquPerson;
                fafang.contractTel = contractTel;
                fafang.fafangPeroson =fafangPeroson;
                string str= Convert.ToDateTime(lingquDate).ToString("yyyy-MM-dd");
                fafang.lingquDate = DateTime.ParseExact(str.Trim(), "yyyy-MM-dd", null).Date;
                fafang.projectID = projectID;
                var maxid = ab.gxLingquInterchangeForm.Max(a => a.ID);
                fafang.ID = maxid + 1;
                d.isFafangHegezheng = true;
                g.archiveCertificateNo = archiveCertificateNo;
                ab.gxFafangArchiveCertificate.Add(fafang);
                ab.Entry(d).State = EntityState.Modified;
                ab.Entry(g).State = EntityState.Modified;
                ab.SaveChanges();
                return Content("<script >alert('确认成功');window.history.back();</script >");

            }
            if (action == "更新")
            {
                var vwArchiveList = from a in ab.gxFafangArchiveCertificate
                                    where a.projectID == projectID
                                    select a;
                if (vwArchiveList.Count() == 0)
                {
                    return Content("<script >alert('请先领取');window.history.back();</script >");
                }
                gxFafangArchiveCertificate c = vwArchiveList.First();
                c.workUnit = workUnit;
                c.lingquPerson = lingquPerson.Trim();
                c.contractTel = contractTel.Trim();
                c.fafangPeroson = fafangPeroson.Trim();
                string str = Convert.ToDateTime(lingquDate).ToString("yyyy-MM-dd");
                c.lingquDate = DateTime.ParseExact(str.Trim(), "yyyy-MM-dd", null).Date;
               
                if (isFafangHegezheng.ToString() == "True")
                {
                    d.isFafangHegezheng =true;
                }
                else
                {
                    d.isFafangHegezheng =false;
                }
                ab.Entry(c).State = EntityState.Modified;
                ab.Entry(d).State = EntityState.Modified;
                ab.SaveChanges();
               return Content("<script >alert('修改成功');window.history.back();</script >");


            }
            

            return View();
        
       
        }
        
    }
}