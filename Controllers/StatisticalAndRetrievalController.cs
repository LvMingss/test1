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
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
namespace urban_archive.Controllers
{
    public class StatisticalAndRetrievalController : Controller
    {
        // GET: StatisticalAndRetrieval
        private UrbanConEntities db = new UrbanConEntities();
        public ActionResult zhenglidayin(string action,string type = "PDF")
        {
            if (action == "打印")
            {
                LocalReport localReport = new LocalReport();
                if (Request.Form["collator"] == "" || Request.Form["startdata"] == "" || Request.Form["enddata"] == "") {
                    return Content("<script>alert('请填写完整整理人及起止时间！');history.back();</script>");
                }
                string Person = Request.Form["collator"];
                DateTime DataFrom = DateTime.Parse(Request.Form["startdata"]);
                DateTime DataTo = DateTime.Parse(Request.Form["enddata"]);
                var ds = from ad in db.vw_projectList
                             //where ad.paperProjectSeqNo == 35
                         where ad.collator == Person
                         select ad;
                var ds1 = ds.Where(ad => ad.dateArchive >= DataFrom).Where(ad => ad.dateArchive <= DataTo);
                //var ds = db.vw_projectList.Where(ad => ad.paperProjectSeqNo > 20).Where(ad => ad.paperProjectSeqNo < 40);
                localReport.ReportPath = Server.MapPath("~/Report/jungong/gerenzhenglidayin.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("gerenzhengli", ds1);
                localReport.DataSources.Add(reportDataSource);
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter("Person", Person));
                parameterList.Add(new ReportParameter("DataFrom", DataFrom.ToString().Trim()));
                parameterList.Add(new ReportParameter("DataTo", DataTo.ToString().Trim()));
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
            //if (action == "取消")
            //{
            //    Response.Write("<script>window.close();</script>");
            //}
            return View();
        }
        public ActionResult AllArchives(string SelectedID, string SearchString, string action)
        {
         
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "工程名称", Value = "0"},
                new SelectListItem { Text = "项目顺序号", Value = "1"},
                new SelectListItem { Text = "建设单位", Value = "2" },
                new SelectListItem { Text = "起始登记号", Value = "3" },
                new SelectListItem { Text = "截止登记号", Value = "4" },
                new SelectListItem { Text = "设计单位", Value = "5" },
                new SelectListItem { Text = "施工单位", Value = "6" },
                new SelectListItem { Text = "工程地点", Value = "7" },
                new SelectListItem { Text = "工程序号", Value = "8" },
                new SelectListItem { Text = "整理人", Value = "9" },
                new SelectListItem { Text = "接收人", Value = "10" },
                new SelectListItem { Text = "接收日期", Value = "11" },
                new SelectListItem { Text = "监理单位", Value = "12" },
                new SelectListItem { Text = "档号", Value = "13"},
            };
            if (SelectedID == null | SelectedID == "")
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", 0);
            }
            else
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);
            }
            var vwprojectFile = from ad in db.vw_projectList
                               select ad;
            if (SelectedID != "" && SelectedID != null && SearchString != "" && SearchString != null)//用户在检索框中输入了检索条件
            {
                    int t = Int32.Parse(SelectedID.Trim());

                if (action == "精确查找")
                {
                    switch (t)
                    {
                        case 0:
                            vwprojectFile = vwprojectFile.Where(ad => ad.projectName == SearchString);//根据责任书编号搜索
                            break;
                        case 1:
                            long projectSeq = long.Parse(SearchString);
                            vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo == projectSeq);//根据工程名称搜索
                            break;
                        case 2:
                            vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization == SearchString);//根据建设单位搜索
                            break;
                        case 3:
                            vwprojectFile = vwprojectFile.Where(ad => ad.startRegisNo == SearchString);//根据工程地点
                            break;
                        case 4:

                            vwprojectFile = vwprojectFile.Where(ad => ad.endRegisNo == SearchString); //根据工程序号
                            break;
                        case 5:
                            vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization == SearchString);//根据施工单位
                            break;
                        case 6:
                            vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization == SearchString);//根据设计单位
                            break;
                        case 7:
                            vwprojectFile = vwprojectFile.Where(ad => ad.location == SearchString);//根据监理单位
                            break;
                        case 8:

                            vwprojectFile = vwprojectFile.Where(ad => ad.projectNo.ToString() == SearchString);//根据项目顺序号
                            break;
                        case 9:

                            vwprojectFile = vwprojectFile.Where(ad => ad.collator == SearchString);//根据项目顺序号
                            break;
                        case 10:

                            vwprojectFile = vwprojectFile.Where(ad => ad.recipient == SearchString);//根据项目顺序号
                            break;
                        case 11:
                            if (SearchString.Contains("-"))
                            {
                                DateTime time = Convert.ToDateTime(SearchString);
                                vwprojectFile = vwprojectFile.Where(ad => ad.dateReceived == time);//根据项目顺序号
                                break;
                            }
                            else {
                                Response.Write("<script>alert('请输入正确的日期格式（XXXX-XX-XX）！');history.back();</script>");
                                break;
                            }    
                        case 12:

                            vwprojectFile = vwprojectFile.Where(ad => ad.jianliUnit == SearchString);//根据项目顺序号
                            break;
                        case 13:
                            vwprojectFile = vwprojectFile.Where(ad => ad.startArchiveNo == SearchString);
                            break;
                    }
                }
                else
                {
                    //for (int j = 0; j < SearchString.Length; j++)
                    //{

                        string content1 = "";
                        content1 = SearchString.ToString();
                        //content1 += SearchString[j].ToString();
                        if (!String.IsNullOrEmpty(content1))
                        {
                            switch (t)
                            {
                                case 0:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.projectName.Contains(content1));//根据责任书编号搜索
                                    break;
                                case 1:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo.ToString().Contains(content1));//根据工程名称搜索
                                    break;
                                case 2:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization.Contains(content1));//根据建设单位搜索
                                    break;
                                case 3:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.startRegisNo.Contains(content1));//根据工程地点
                                    break;
                                case 4:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.endRegisNo.Contains(content1)); //根据工程序号
                                    break;
                                case 5:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization.Contains(content1));//根据施工单位
                                    break;
                                case 6:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization.Contains(content1));//根据设计单位
                                    break;
                                case 7:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.location.Contains(content1));//根据监理单位
                                    break;
                                case 8:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.projectNo.ToString().Contains(content1));//根据项目顺序号
                                    break;
                                case 9:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.collator.Contains(content1));//根据项目顺序号
                                    break;
                                case 10:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.recipient.Contains(content1));//根据项目顺序号
                                    break;
                                case 11:
                                    //DateTime time = Convert.ToDateTime(content1);
                                    //vwprojectFile = vwprojectFile.Where(ad => ad.dateReceived == time);//根据项目顺序号
                                    vwprojectFile = vwprojectFile.Where(ad => ad.dateReceived.ToString().Contains(content1));
                                    break;
                                case 12:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.jianliUnit.Contains(content1));
                                    break;
                                case 13:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.startArchiveNo.Contains(content1));
                                    break;
                            }
                        }
                    //}
                }
            }
             

            
            vwprojectFile = vwprojectFile.Where(a => a.projectName != "重号").Where(b => b.projectName != "重").Where(c => c.projectName != "重复").Where(d => d.projectName != "重复号").Where(f => f.projectName != "作废").Where(f => f.projectName != "作废").Where(f => f.projectName != "AAAAAAA").Where(f => f.projectName != "并入20132005").Where(f => f.projectName != "删掉").Where(f => f.projectName != "修改").Where(f => f.projectName != "并入20131385号").Where(f => f.projectName != "（此号作废此工程并入20131406）李沧区楼山工业区村庄改造搬迁办公室楼山工业区石家村、徐家村村庄改造安置房工程6#、7#及地下车库、12#—15#楼工程节能资料").Where(f => f.projectName != "并入20110366青岛大剧院工程").Where(f => f.projectName != "并20150622").Where(f => f.projectName != "并入20140274号").Where(f => f.projectName != "并入20150438号").Where(f => f.projectName != "与20140084号重复").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "并入20140106号").Where(f => f.projectName != "并入20140108号").Where(f => f.projectName != "并入20140053号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20130041号").Where(f => f.projectName != "废").Where(f => f.projectName != "空白").Where(f => f.projectName != "作废").Where(f => f.projectName != "并入20131745号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20131771号").Where(f => f.projectName != "并入20131886号").Where(f => f.projectName != "并入20131751号").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "青岛作废").Where(f => f.projectName != "青作废").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "ccc").OrderBy(s => s.paperProjectSeqNo);
            ViewBag.CurrentFilter = SearchString;
            ViewBag.SearchType = action;
            ViewBag.count = vwprojectFile.Count();
            return View();
        }
        public string  AllArchivesData(int? page, string type, string content, string SearchType)
        {
            var vwprojectFile = from ad in db.vw_projectList
                                
                                select ad;
            if (type != "" && type != null && content != "" && content != null)//用户在检索框中输入了检索条件
            {
                int t = Int32.Parse(type.Trim());

                if (SearchType == "精确查找")
                {
                    switch (t)
                    {
                        case 0:
                            vwprojectFile = vwprojectFile.Where(ad => ad.projectName == content);//根据责任书编号搜索
                            break;
                        case 1:
                            long projectSeq = long.Parse(content);
                            vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo == projectSeq);//根据工程名称搜索
                            break;
                        case 2:
                            vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization == content);//根据建设单位搜索
                            break;
                        case 3:
                            vwprojectFile = vwprojectFile.Where(ad => ad.startRegisNo == content);//根据工程地点
                            break;
                        case 4:

                            vwprojectFile = vwprojectFile.Where(ad => ad.endRegisNo == content); //根据工程序号
                            break;
                        case 5:
                            vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization == content);//根据施工单位
                            break;
                        case 6:
                            vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization == content);//根据设计单位
                            break;
                        case 7:
                            vwprojectFile = vwprojectFile.Where(ad => ad.location == content);//根据监理单位
                            break;
                        case 8:

                            vwprojectFile = vwprojectFile.Where(ad => ad.projectNo.ToString() == content);//根据项目顺序号
                            break;
                        case 9:

                            vwprojectFile = vwprojectFile.Where(ad => ad.collator == content);//根据项目顺序号
                            break;
                        case 10:

                            vwprojectFile = vwprojectFile.Where(ad => ad.recipient == content);//根据项目顺序号
                            break;
                        case 11:
                            if (content.Contains("-"))
                            {
                                DateTime time = Convert.ToDateTime(content);
                                vwprojectFile = vwprojectFile.Where(ad => ad.dateReceived == time);//根据项目顺序号
                                break;
                            }
                            else
                            {
                                Response.Write("<script>alert('请输入正确的日期格式（XXXX-XX-XX）！');history.back();</script>");
                                break;
                            }
                        case 12:

                            vwprojectFile = vwprojectFile.Where(ad => ad.jianliUnit == content);//根据项目顺序号
                            break;
                        case 13:
                            vwprojectFile = vwprojectFile.Where(ad => ad.startArchiveNo == content);
                            break;
                    }
                }
                else
                {
                    //for (int j = 0; j < content.Length; j++)
                    //{

                        string content1 = "";
                    content1 = content.ToString();
                    //content1 += content[j].ToString();
                    if (!String.IsNullOrEmpty(content1))
                        {
                            switch (t)
                            {
                                case 0:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.projectName.Contains(content1));//根据责任书编号搜索
                                    break;
                                case 1:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.paperProjectSeqNo.ToString().Contains(content1));//根据工程名称搜索
                                    break;
                                case 2:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.developmentOrganization.Contains(content1));//根据建设单位搜索
                                    break;
                                case 3:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.startRegisNo.Contains(content1));//根据工程地点
                                    break;
                                case 4:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.endRegisNo.Contains(content1)); //根据工程序号
                                    break;
                                case 5:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.disignOrganization.Contains(content1));//根据施工单位
                                    break;
                                case 6:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.constructionOrganization.Contains(content1));//根据设计单位
                                    break;
                                case 7:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.location.Contains(content1));//根据监理单位
                                    break;
                                case 8:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.projectNo.ToString().Contains(content1));//根据项目顺序号
                                    break;
                                case 9:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.collator.Contains(content1));//根据项目顺序号
                                    break;
                                case 10:

                                    vwprojectFile = vwprojectFile.Where(ad => ad.recipient.Contains(content1));//根据项目顺序号
                                    break;
                                case 11:
                                    //DateTime time = Convert.ToDateTime(content1);
                                    //vwprojectFile = vwprojectFile.Where(ad => ad.dateReceived == time);//根据项目顺序号
                                    vwprojectFile = vwprojectFile.Where(ad => ad.dateReceived.ToString().Contains(content1));
                                    break;
                                case 12:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.jianliUnit.Contains(content1));
                                    break;
                                case 13:
                                    vwprojectFile = vwprojectFile.Where(ad => ad.startArchiveNo.Contains(content1));
                                    break;
                            }
                        }
                    //}
                }
            }
            vwprojectFile = vwprojectFile.Where(c => c.projectName != "重号").Where(c => c.projectName != "重").Where(c => c.projectName != "重复").Where(d => d.projectName != "重复号").Where(f => f.projectName != "作废").Where(f => f.projectName != "作废").Where(f => f.projectName != "AAAAAAA").Where(f => f.projectName != "并入20132005").Where(f => f.projectName != "删掉").Where(f => f.projectName != "修改").Where(f => f.projectName != "并入20131385号").Where(f => f.projectName != "（此号作废此工程并入20131406）李沧区楼山工业区村庄改造搬迁办公室楼山工业区石家村、徐家村村庄改造安置房工程6#、7#及地下车库、12#—15#楼工程节能资料").Where(f => f.projectName != "并入20110366青岛大剧院工程").Where(f => f.projectName != "并20150622").Where(f => f.projectName != "并入20140274号").Where(f => f.projectName != "并入20150438号").Where(f => f.projectName != "与20140084号重复").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "并入20140106号").Where(f => f.projectName != "并入20140108号").Where(f => f.projectName != "并入20140053号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20130041号").Where(f => f.projectName != "废").Where(f => f.projectName != "空白").Where(f => f.projectName != "作废").Where(f => f.projectName != "并入20131745号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20131771号").Where(f => f.projectName != "并入20131886号").Where(f => f.projectName != "并入20131751号").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "青岛作废").Where(f => f.projectName != "青作废").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "ccc").OrderBy(s => s.paperProjectSeqNo);
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            int cnt = vwprojectFile.Count() / pageSize + 1;
            if (vwprojectFile.Count() % pageSize == 0)
            {
                cnt = vwprojectFile.Count() / pageSize;
            }
            vwprojectFile = vwprojectFile.OrderBy(s => s.paperProjectSeqNo).ThenBy(s=>s.projectNo);
            var a = vwprojectFile.ToPagedList(pageNumber, pageSize);
           
            var b = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                                        //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程
                                        from p in a
                                        select new JObject(
                                                 new JProperty("projectName", p.projectName),
                                                 new JProperty("projectNo", p.projectNo),
                                                 new JProperty("paperProjectSeqNo", p.paperProjectSeqNo),
                                                 new JProperty("d", p.startArchiveNo+"-"+p.endArchiveNo),
                                                 new JProperty("e",p.startRegisNo+"-"+p.endRegisNo),
                                                 new JProperty("location", p.location),
                                                 new JProperty("developmentOrganization", p.developmentOrganization),
                                                 new JProperty("disignOrganization", p.disignOrganization),
                                                 new JProperty("constructionOrganization", p.constructionOrganization),
                                                 new JProperty("statusName", p.statusName),
                                                 new JProperty("collator", p.collator),
                                                 new JProperty("recipient", p.recipient),
                                                 new JProperty("projectID", p.projectID),
                                                 new JProperty("dateReceived", p.dateReceived),
                                                 new JProperty("dateArchive", p.dateArchive),
                                                 new JProperty("archivesCount", p.archivesCount)
                                        )
                                )
                    )
).ToString();
            return b;
        }

        public ActionResult SeeReceive(long? id, string action,string id2)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var test = from ad in db.vw_projectProfile
                       where (ad.projectID == id)
                       select ad;
            vw_projectProfile projectProfile = test.First();
            if (action == "返回")
            {
                if (id2 == "2")
                {
                    return RedirectToAction("AllArchives");

                }
                else
                {
                    return RedirectToAction("StatisticalAndAnalysis");
                }


            }
            if (action == "文件下载")
            {
                //if (string.IsNullOrEmpty(id))
                //{
                //    throw new ArgumentNullException("fileId is errror");
                //}
                int Id = Convert.ToInt32(id);
                var findFile = db.ProjectInfo.Where(a=>a.projectID==Id).First();
                if (findFile.storagePath == null)
                {
                    Response.Write("<script >alert('该工程未上传文件！');window.history.back();</script>");
                }
                else
                {
                    string filePath = Request.MapPath("~/files/jungongWord");
                    string path = filePath + "/" + findFile.storagePath;
                    //以字符流的形式下载文件
                    FileStream fs = new FileStream(path, FileMode.Open);
                    byte[] bytes = new byte[(int)fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    fs.Close();
                    Response.ContentType = "application/octet-stream";
                    //通知浏览器下载文件而不是打开
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(findFile.storagePath, System.Text.Encoding.UTF8));
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();
                }

            }
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1"},
                new SelectListItem { Text = "否", Value = "0"},

              };
            if(test.First().isYD==true)
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text",1);
            }
            else 
            {
                ViewBag.isYD = new SelectList(list, "Value", "Text", 0);
            }
            if (projectProfile == null)
            {
                return HttpNotFound();
            }
            return View(projectProfile);

        }
        public ActionResult SeeArchives(long? id,string action,string id2)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (action == "返回")
            {
                if(id2=="2")
                {
                    return RedirectToAction("AllArchives");
                   
                }
                else
                {
                   return RedirectToAction("StatisticalAndAnalysis");
                }

               
            }
            if(id==0)
            {
                return Content("<script >alert('此工程尚未录入案卷');window.history.back();</script >");

            }
            
            var archive = from a in db.ArchivesDetail
                          where a.paperProjectSeqNo == id
                          orderby a.volNo
                          select a;
            if (archive == null)
            {
                return HttpNotFound();
            }

            return View(archive);



        }
        public ActionResult SeeSettle(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var test = from ad in db.vw_archiveQueryList
                       where (ad.projectID == id)
                       select ad;
            vw_archiveQueryList archive = test.First();
            //文本框赋值
            ViewData["character1cm"]=archive.character1cm;
            ViewData["character2cm"]=archive.character2cm;
            ViewData["character3cm"]=archive.character3cm;
            ViewData["character4cm"]=archive.character4cm;
            ViewData["character5cm"]=archive.character5cm;
            ViewData["drawing1cm"] =archive.drawing1cm;
            ViewData["drawing2cm"] = archive.drawing2cm;
            ViewData["drawing3cm"] = archive.drawing3cm;
            ViewData["drawing4cm"] = archive.drawing4cm;
            ViewData["drawing5cm"] = archive.drawing5cm;

            if (archive == null)
            {
                return HttpNotFound();
            }

            return View(archive);
        }
        [HttpPost]
        public ActionResult SeeSettle([Bind(Include = "InchCountDetail,characterVolumeCount,character1cm,character2cm,character3cm,character4cm,character5cm,originalVolumeCount,originalInchCount,drawingVolumeCount,drawing1cm,drawing2cm,drawing3cm,drawing4cm,drawing5cm,copyInchCount")] vw_archiveQueryList archiveQueryList, string action, long? id,string id2)
        {

            var paper = from ad in db.PaperArchives
                        where (ad.projectID == id)
                        select ad;
            PaperArchives paperArchive = paper.First();
            var project = from ac in db.ProjectInfo
                          where (ac.projectID == id)
                          select ac;
            ProjectInfo projectinfo = project.First();
            paperArchive.InchCountDetail = archiveQueryList.InchCountDetail;
            paperArchive.character1cm = archiveQueryList.character1cm;
            paperArchive.character2cm = archiveQueryList.character2cm;
            paperArchive.character3cm = archiveQueryList.character3cm;
            paperArchive.character4cm = archiveQueryList.character4cm;
            paperArchive.character5cm = archiveQueryList.character5cm;
            //ViewData["characterVolumeCount"] = archiveQueryList.character2cm + archiveQueryList.character3cm + archiveQueryList.character4cm + archiveQueryList.character5cm;
            paperArchive.characterVolumeCount = archiveQueryList.character1cm+ archiveQueryList.character2cm + archiveQueryList.character3cm + archiveQueryList.character4cm + archiveQueryList.character5cm;
            paperArchive.drawing1cm = archiveQueryList.drawing1cm;
            paperArchive.drawing2cm = archiveQueryList.drawing2cm;
            paperArchive.drawing3cm = archiveQueryList.drawing3cm;
            paperArchive.drawing4cm = archiveQueryList.drawing4cm;
            paperArchive.drawing5cm = archiveQueryList.drawing5cm;
            //ViewData["drawingVolumeCount"] = archiveQueryList.drawing2cm + archiveQueryList.drawing3cm + archiveQueryList.drawing4cm + archiveQueryList.drawing5cm;
            paperArchive.drawingVolumeCount = archiveQueryList.drawing1cm+archiveQueryList.drawing2cm + archiveQueryList.drawing3cm + archiveQueryList.drawing4cm + archiveQueryList.drawing5cm;
            //ViewData["originalVolumeCount"] = paperArchive.characterVolumeCount + paperArchive.drawingVolumeCount;
            //ViewData["originalInchCount"] = (archiveQueryList.character2cm + archiveQueryList.drawing2cm) * 2 + (archiveQueryList.character3cm + archiveQueryList.drawing3cm) * 3 + (archiveQueryList.character4cm + archiveQueryList.drawing4cm) * 4 + (archiveQueryList.character5cm + archiveQueryList.drawing5cm) * 5;
            paperArchive.originalVolumeCount = archiveQueryList.character2cm + archiveQueryList.character3cm + archiveQueryList.character4cm + archiveQueryList.character5cm + archiveQueryList.drawing2cm + archiveQueryList.drawing3cm + archiveQueryList.drawing4cm + archiveQueryList.drawing5cm;
            paperArchive.originalInchCount = (archiveQueryList.character2cm + archiveQueryList.drawing2cm) * 2 + (archiveQueryList.character3cm + archiveQueryList.drawing3cm) * 3 + (archiveQueryList.character4cm + archiveQueryList.drawing4cm) * 4 + (archiveQueryList.character5cm + archiveQueryList.drawing5cm) * 5;
            paperArchive.copyInchCount = archiveQueryList.copyInchCount;
            paperArchive.archivesCount = (paperArchive.characterVolumeCount + paperArchive.drawingVolumeCount).ToString();

            if (action == "修改")
            {
                if(Convert.ToInt32(projectinfo.status)==3|| Convert.ToInt32(projectinfo.status)==5)
                {
                    ViewData["onlyread"] = false;
                }
                else
                {
                    ViewData["onlyread"] = true;
                }
                if (Convert.ToInt32(projectinfo.status) == 4)
                {
                    projectinfo.status = "9";
                }

                if (ModelState.IsValid)
                {
                    db.Entry(paperArchive).State = EntityState.Modified;
                    db.Entry(projectinfo).State = EntityState.Modified;
                    db.SaveChanges();
                    if(id2 == "1")
                    {
                        return Content("<script >alert('修改成功');window.location.href='../StatisticalAndRetrieval/StatisticalAndAnalysis';</script >");
                    }
                    if(id2 == "2")
                    {
                        return Content("<script >alert('修改成功');window.location.href='../StatisticalAndRetrieval/AllArchives';</script >");
                    }
                }
            }


            if (action == "返回")
            {
                if (id2 == "2")
                {
                    return RedirectToAction("AllArchives");

                }
                else
                {
                    return RedirectToAction("StatisticalAndAnalysis");
                }


            }
            return View();
        }
        public ActionResult StatisticalAndAnalysis(string SelectedID, string startdate, string enddate)
        {
          

            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "全部", Value = "0"},
                new SelectListItem { Text = "通过审核", Value = "1"},
                new SelectListItem { Text = "整理", Value = "2" },
                new SelectListItem { Text = "编号", Value = "3" },
                new SelectListItem { Text = "录入", Value = "4" },
                new SelectListItem { Text = "等待入库", Value = "5" },
                 new SelectListItem { Text = "入库", Value = "6" }

            };

          
            if(SelectedID == null| SelectedID == "")
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", 0);
            }
            else
            {
                ViewBag.SelectedID = new SelectList(list, "Value", "Text", SelectedID);
            }
            var vwprojectFile = from ad in db.vw_projectList
                                select ad;
         
            if (SelectedID != "" && SelectedID != null)//用户在检索框中输入了检索条件
            {
                int t = Int32.Parse(SelectedID.Trim());
                if (!String.IsNullOrEmpty(SelectedID))
                {

                    switch (t)
                    {
                        case 0:

                            break;
                        case 1:

                            vwprojectFile = vwprojectFile.Where(ad => ad.status == "3");
                            break;
                        case 2:
                            vwprojectFile = vwprojectFile.Where(ad => ad.status == "4");
                            break;
                        case 3:
                            vwprojectFile = vwprojectFile.Where(ad => ad.status == "5");
                            break;
                        case 4:

                            vwprojectFile = vwprojectFile.Where(ad => ad.status == "6");
                            break;
                        case 5:

                            vwprojectFile = vwprojectFile.Where(ad => ad.status == "10");
                            break;
                        case 6:

                            vwprojectFile = vwprojectFile.Where(ad => ad.status == "7");
                            break;


                    }
                }

            }
            if (startdate != "" && startdate != null && enddate != "" && enddate != null)//增加查询条件
            {
                DateTime start = DateTime.Parse(startdate.Trim());
                DateTime end = DateTime.Parse(enddate.Trim());
                vwprojectFile = from h in vwprojectFile
                                where (h.dateReceived >= start) && (h.dateReceived <= end)
                                select h;
            }
            vwprojectFile = vwprojectFile.Where(a => a.projectName != "重号").Where(b => b.projectName != "重").Where(c => c.projectName != "重复").Where(d => d.projectName != "重复号").Where(f => f.projectName != "作废").Where(f => f.projectName != "作废").Where(f => f.projectName != "AAAAAAA").Where(f => f.projectName != "并入20132005").Where(f => f.projectName != "删掉").Where(f => f.projectName != "修改").Where(f => f.projectName != "并入20131385号").Where(f => f.projectName != "（此号作废此工程并入20131406）李沧区楼山工业区村庄改造搬迁办公室楼山工业区石家村、徐家村村庄改造安置房工程6#、7#及地下车库、12#—15#楼工程节能资料").Where(f => f.projectName != "并入20110366青岛大剧院工程").Where(f => f.projectName != "并20150622").Where(f => f.projectName != "并入20140274号").Where(f => f.projectName != "并入20150438号").Where(f => f.projectName != "与20140084号重复").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "并入20140106号").Where(f => f.projectName != "并入20140108号").Where(f => f.projectName != "并入20140053号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20130041号").Where(f => f.projectName != "废").Where(f => f.projectName != "空白").Where(f => f.projectName != "作废").Where(f => f.projectName != "并入20131745号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20131771号").Where(f => f.projectName != "并入20131886号").Where(f => f.projectName != "并入20131751号").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "青岛作废").Where(f => f.projectName != "青作废").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "ccc").OrderBy(s => s.paperProjectSeqNo).ThenBy(s => s.projectNo);
            ViewData["start"] = startdate;
            ViewData["end"] = enddate;
            ViewBag.count = vwprojectFile.Count();

            return View();

        }
        public string  StatisticalAndAnalysisData(int? page, string type,string startdate,string enddate)
        {
            var vwprojectFile = from ad in db.vw_projectList

                                select ad;
            if (type != "" && type != null )//用户在检索框中输入了检索条件
            {
                int t = Int32.Parse(type.Trim());
                if (!String.IsNullOrEmpty(type))
                {
               
                    switch (t)
                    {
                        case 0:

                            break;
                        case 1:

                            vwprojectFile = vwprojectFile.Where(ad => ad.status == "3");
                            break;
                        case 2:
                            vwprojectFile = vwprojectFile.Where(ad => ad.status == "4");
                            break;
                        case 3:
                            vwprojectFile = vwprojectFile.Where(ad => ad.status == "5");
                            break;
                        case 4:

                            vwprojectFile = vwprojectFile.Where(ad => ad.status == "6");
                            break;
                        case 5:

                            vwprojectFile = vwprojectFile.Where(ad => ad.status == "10");
                            break;
                        case 6:

                            vwprojectFile = vwprojectFile.Where(ad => ad.status == "7");
                            break;


                    }
                }

            }
            if (startdate != "" && startdate != null && enddate != "" && enddate != null)//增加查询条件
            {
                DateTime start = DateTime.Parse(startdate.Trim());
                DateTime end = DateTime.Parse(enddate.Trim());
                vwprojectFile = from h in vwprojectFile
                                where (h.dateReceived >= start) && (h.dateReceived <= end)
                                select h;
            }
            //int v = vwprojectFile.Count();
            vwprojectFile = vwprojectFile.Where(c => c.projectName != "重号").Where(c => c.projectName != "重").Where(c => c.projectName != "重复").Where(d => d.projectName != "重复号").Where(f => f.projectName != "作废").Where(f => f.projectName != "作废").Where(f => f.projectName != "AAAAAAA").Where(f => f.projectName != "并入20132005").Where(f => f.projectName != "删掉").Where(f => f.projectName != "修改").Where(f => f.projectName != "并入20131385号").Where(f => f.projectName != "（此号作废此工程并入20131406）李沧区楼山工业区村庄改造搬迁办公室楼山工业区石家村、徐家村村庄改造安置房工程6#、7#及地下车库、12#—15#楼工程节能资料").Where(f => f.projectName != "并入20110366青岛大剧院工程").Where(f => f.projectName != "并20150622").Where(f => f.projectName != "并入20140274号").Where(f => f.projectName != "并入20150438号").Where(f => f.projectName != "与20140084号重复").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "并入20140106号").Where(f => f.projectName != "并入20140108号").Where(f => f.projectName != "并入20140053号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20130041号").Where(f => f.projectName != "废").Where(f => f.projectName != "空白").Where(f => f.projectName != "作废").Where(f => f.projectName != "并入20131745号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20131737号").Where(f => f.projectName != "并入20131771号").Where(f => f.projectName != "并入20131886号").Where(f => f.projectName != "并入20131751号").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "青岛作废").Where(f => f.projectName != "青作废").Where(f => f.projectName != "青岛 作废").Where(f => f.projectName != "ccc").OrderBy(s => s.paperProjectSeqNo);
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            int cnt = vwprojectFile.Count() / pageSize + 1;
            if (vwprojectFile.Count() % pageSize == 0)
            {
                cnt = vwprojectFile.Count() / pageSize;
            }

            var a = vwprojectFile.ToPagedList(pageNumber, pageSize);
            var b = new JObject(
                        new JProperty("last_page", cnt),
                        new JProperty("data",
                                new JArray(
                                        //使用LINQ to JSON可直接在select语句中生成JSON数据对象，无须其它转换过程
                                        from p in a
                                        select new JObject(
                                                 new JProperty("projectName", p.projectName),
                                                 new JProperty("projectNo", p.projectNo),
                                                 new JProperty("paperProjectSeqNo", p.paperProjectSeqNo),
                                                 new JProperty("d", p.startArchiveNo + "-" + p.endArchiveNo),
                                                 new JProperty("e", p.startRegisNo + "-" + p.endRegisNo),
                                                 new JProperty("location", p.location),
                                                 new JProperty("developmentOrganization", p.developmentOrganization),
                                                 new JProperty("disignOrganization", p.disignOrganization),
                                                 new JProperty("constructionOrganization", p.constructionOrganization),
                                                 new JProperty("statusName", p.statusName),
                                                 new JProperty("collator", p.collator),
                                                 new JProperty("recipient", p.recipient),
                                                 new JProperty("projectID", p.projectID),
                                                 new JProperty("dateReceived", p.dateReceived),
                                                 new JProperty("dateArchive", p.dateArchive)

                                        )
                                )
                    )
).ToString();
           
            return b;
        }
       

    }
}