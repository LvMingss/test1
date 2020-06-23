using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using urban_archive.Models;
using System.IO;
using Newtonsoft.Json;

namespace urban_archive.Controllers
{
    public class VideoCassetteListsController : Controller
    {
        private VideoArchiveEntities db = new VideoArchiveEntities();

        // GET: VideoCassetteLists
        public ActionResult Index(int videoProjectSeqNo)
        {
            var videoArchive = db.VideoArchives.Where(x => x.videoProjectSeqNo == videoProjectSeqNo);
            ViewBag.projectName = videoArchive.FirstOrDefault().projectName;
            ViewBag.videoProjectSeqNo = videoProjectSeqNo;
            var videoCassetteList = db.VideoCassetteList.Where(x => x.ProjectIDS == videoProjectSeqNo.ToString());
            string status = videoArchive.FirstOrDefault().videoStatus;
            if (status == "2")
            {
                ViewBag.fanhui = "Index_shenhe";
            }
            else if(status == "3")
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
        // Post: VideoCassetteLists
        [HttpPost]
        public ActionResult Index(int? videoProjectSeqNo,String action)
        {
            if (videoProjectSeqNo == null)
            {
                return Content("<script >alert('该工程缺失工程序号！');window.history.back();</script >");
            }
            var videoArchive = db.VideoArchives.Where(x => x.videoProjectSeqNo == videoProjectSeqNo).FirstOrDefault();
            if (action== "返回")
            {
                return RedirectToAction("Index_shenhe", "VideoArchives");
            }
            else if (action == "上传完成")
            {
                videoArchive.videoStatus = "3";
                db.Entry(videoArchive).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index_dairuku", "VideoArchives");
            }
            else if (action == "驳回")
            {
                videoArchive.videoStatus = "5";
                db.Entry(videoArchive).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index_bohui", "VideoArchives");
            }
            else if (action == "暂入库")
            {
                videoArchive.videoStatus = "6";
                db.Entry(videoArchive).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index_zanruku", "VideoArchives");
            }

            ViewBag.projectName = videoArchive.projectName;
            ViewBag.videoProjectSeqNo = videoProjectSeqNo;
            string filingDesc = Request.Form["filingDesc"];
            var filingDesc1 = db.VideoCassetteList.Where(x => x.ProjectIDS == videoProjectSeqNo.ToString()).Where(x => x.filingDesc == filingDesc);
            if (filingDesc1.Count() > 0 )
            {
                return Content("<script >alert('该类别文件记录已存在！');window.history.back();</script >");
            }
            if ( Request.Form["videoMinutesCount"] != null)//如果没用该类别文件记录则新建一记录
            {
                VideoCassetteList videocassetteList = new VideoCassetteList();
                int max_id = db.VideoCassetteList.Max(x => x.ID);
                videocassetteList.ID = max_id + 1;
                videocassetteList.filingDesc = Request.Form["filingDesc"];//当前视频记录类型：工程前期、基础工程...
                videocassetteList.ProjectIDS = videoProjectSeqNo.ToString();//所属工程ID
                videocassetteList.shootingDate = DateTime.Parse(Request.Form["shootingDate"]);//拍摄日期
                videocassetteList.videoMinutesCount = Request.Form["videoMinutesCount"];//视频总长度
                videocassetteList.videoContent = Request.Form["videoContent"];//视频详细内容
                videocassetteList.checker = "待上传";//
                db.VideoCassetteList.Add(videocassetteList);
                db.SaveChanges();
            }
            var videoCassetteList = db.VideoCassetteList.Where(x => x.ProjectIDS == videoProjectSeqNo.ToString());
            return View(videoCassetteList.ToList());
        }

        public PartialViewResult PartialView_Create(int? videoProjectSeqNo)
        {
            List<SelectListItem> list_filingDesc = new List<SelectListItem> {
                new SelectListItem { Text = "1、工程前期", Value = "1、工程前期"},
                new SelectListItem { Text = "2、基础工程", Value = "2、基础工程" },
                new SelectListItem { Text = "3、主体工程", Value = "3、主体工程" },
                new SelectListItem { Text = "4、节能工程", Value = "4、节能工程" },
                new SelectListItem { Text = "5、安装工程", Value = "5、安装工程" },
                new SelectListItem { Text = "6、工程竣工", Value = "6、工程竣工" }
                //new SelectListItem { Text = "4、屋面保温、防水施工及验收", Value = "4、屋面保温、防水施工及验收"},
                //new SelectListItem { Text = "5、外墙施工及验收", Value = "5、外墙施工及验收" },
                //new SelectListItem { Text = "6、给排水工程施工", Value = "6、给排水工程施工" },
                //new SelectListItem { Text = "7、电气工程安装及验收", Value = "7、电气工程安装及验收"},
                //new SelectListItem { Text = "8、设备安装及验收", Value = "8、设备安装及验收"},
                //new SelectListItem { Text = "9、工程竣工验收及全貌", Value = "9、工程竣工验收及全貌"}
                ////new SelectListItem { Text = "照片档案卷内目录（工程名称）", Value = "照片档案卷内目录（工程名称）"}
            };
            ViewBag.filingDesc= new SelectList(list_filingDesc, "Value", "Text");

            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            String now= currentTime.ToString("d");
            ViewBag.now = now;
            return PartialView();
        }

        [HttpPost]
        public void Cailiaoshenhe()//材料审核
        {
            string strJson = "[{'id':1,'name':'张三'},{'id':2,'name':'李四'},{'id':3,'name':'王五'}]";
            Response.Write(JsonConvert.SerializeObject(strJson));
            //Response.Write("{result: '材料齐全,审核通过!'}");
        }
        [HttpPost]
        public void UpLoad(int? id)//文件上传
        {
            if(id==null)
            {
                Response.Write("{error: '工程序号不存在！请返回上级重新录入.'}");
            }
            var videoCassetteList = db.VideoCassetteList.Find(id);
            string ProjectIDS = videoCassetteList.ProjectIDS.Trim();
            string viedeoType = videoCassetteList.filingDesc.Trim();
            //int videoProjectSeqNo = int.Parse(videoCassetteList.ProjectIDS);
            //var videoArchive = db.VideoArchives.Where(x => x.videoProjectSeqNo == videoProjectSeqNo);
            var files = Request.Files;
            if (files.Count > 0)
            {
                string type = files.Keys[0];
                var file1 = files[0];              
                string strFileSavePath = Server.MapPath("~/声像资料/" + ProjectIDS+ "/声像照片/" + viedeoType+"/");//文件存储路径
                if (type == "video")
                    strFileSavePath = Server.MapPath("~/声像资料/" + ProjectIDS + "/声像视频/" + viedeoType + "/");//文件存储路径
                if (!Directory.Exists(strFileSavePath))
                        Directory.CreateDirectory(strFileSavePath);
                file1.SaveAs(strFileSavePath + file1.FileName);
            }
            Response.Write("{}");
        }
        [HttpPost]
        public void UpLoad_jnml(int? videoProjectSeqNo)//文件上传
        {
            if (videoProjectSeqNo == null)
            {
                Response.Write("{error: '工程序号不存在！请返回上级重新录入.'}");
            }
            string ProjectIDS = videoProjectSeqNo.ToString();
            var files = Request.Files;
            if (files.Count > 0)
            {
                string type = files.Keys[0];
                var file1 = files[0];
                string strFileSavePath = Server.MapPath("~/声像资料/" + ProjectIDS  + "/");//文件存储路径               
                if (!Directory.Exists(strFileSavePath))
                    Directory.CreateDirectory(strFileSavePath);
                file1.SaveAs(strFileSavePath + file1.FileName);
            }
            Response.Write("{}");
        }
        // GET: VideoCassetteLists/Create
        public ActionResult UpLoadFiles(int id)//文件上传
        {
            ViewBag.id = id;
            var videoCassetteList = db.VideoCassetteList.Find(id);
            int videoProjectSeqNo = int.Parse(videoCassetteList.ProjectIDS);
            var videoArchive = db.VideoArchives.Where(x => x.videoProjectSeqNo == videoProjectSeqNo);
            ViewBag.projectName = videoArchive.FirstOrDefault().projectName;
            ViewBag.filingDesc = videoCassetteList.filingDesc;
            ViewBag.videoContent = videoCassetteList.videoContent;
            ViewBag.videoProjectSeqNo = videoProjectSeqNo;
            return View();
        }

        // POST: VideoCassetteLists/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpLoadFiles(int id, String action)//文件上传
        {
            ViewBag.id = Request.Form["id"];
            var videoCassetteList = db.VideoCassetteList.Find(id);
            int videoProjectSeqNo = int.Parse(videoCassetteList.ProjectIDS);
            var videoArchive = db.VideoArchives.Where(x => x.videoProjectSeqNo == videoProjectSeqNo);
            string projectName = videoArchive.FirstOrDefault().projectName;
            ViewBag.projectName = projectName;
            ViewBag.filingDesc = videoCassetteList.filingDesc;
            ViewBag.videoContent = videoCassetteList.videoContent;           
            if (action == "通过")
            {
                videoCassetteList.checker = "文件齐全";//一期系统checker字段没用，拿来当做状态字段
            }
            else
            {
                //videoCassetteList.checker = "文件不齐全";
                int pic=0, video=0;       
                string ProjectIDS = videoCassetteList.ProjectIDS.ToString().Trim();
                string filingDesc = db.VideoCassetteList.Find(id).filingDesc.ToString().Trim();
                string folderFullName1 = Server.MapPath("~/声像资料/" + ProjectIDS + "/声像视频/" + filingDesc + "/");//视频文件存储路径
                string folderFullName2 = Server.MapPath("~/声像资料/" + ProjectIDS + "/声像照片/" + filingDesc + "/");//图片文件存储路径
                if (Directory.Exists(folderFullName1))
                {
                    DirectoryInfo TheFolder1 = new DirectoryInfo(folderFullName1);
                    video = TheFolder1.GetFiles().Count();
                    //return Content("<script >alert('尚未提交视频文件！！');window.history.back();</script >");
                }
                if (Directory.Exists(folderFullName2))
                {
                    DirectoryInfo TheFolder2 = new DirectoryInfo(folderFullName2);               
                    pic = TheFolder2.GetFiles().Count();
                    //return Content("<script >alert('尚未提交照片文件！！');window.history.back();</script >");
                }                            
                try
                {
                    videoCassetteList.videoContent = "1.视频：" + video + "  段\n2.照片：" + pic + " 张";
                    db.Entry(videoCassetteList).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("UpLoadFiles", new { id = videoCassetteList.ID });
                }
                catch(Exception e)
                {
                    return Content("<script >alert('统计出错，请核查！');window.history.back();</script >");
                }
            }
            videoCassetteList.videoContent = Request.Form["videoContent"];//更新文件备注信息
            db.Entry(videoCassetteList).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { videoProjectSeqNo = videoProjectSeqNo });
        }


        public ActionResult playvideo(int? id)//播放视频
        {
            if (id == null)
            {
                return Content("<script >alert('路径不存在！');window.history.back();</script >");
            }
            string result = "";
            var videoCassetteList = db.VideoCassetteList.Find(id);
            string ProjectIDS = videoCassetteList.ProjectIDS.ToString().Trim();
            string filingDesc = db.VideoCassetteList.Find(id).filingDesc.ToString().Trim();
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
            var videoArchive = db.VideoArchives.Where(x => x.videoProjectSeqNo == videoProjectSeqNo);
            ViewBag.projectName = videoArchive.FirstOrDefault().projectName;
            ViewBag.videoProjectSeqNo = videoProjectSeqNo;
            ViewBag.filingDesc = videoCassetteList.filingDesc;
            ViewBag.videoContent = videoCassetteList.videoContent;
            return View();
        }
        public ActionResult lookphoto(int? id)//图片浏览
        {
            if (id == null)
            {
                return Content("<script >alert('路径不存在！');window.history.back();</script >");
            }
            string result="";
            var videoCassetteList = db.VideoCassetteList.Find(id);
            string ProjectIDS = videoCassetteList.ProjectIDS.ToString().Trim();
            string filingDesc = db.VideoCassetteList.Find(id).filingDesc.ToString().Trim();
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
                result = result+ xdlujing + NextFile.Name + ";";   
            ViewBag.result = result;//路径下照片文件序列传给前台
            ViewBag.id = id;
            //传给前台，显示工程信息等
            int videoProjectSeqNo = int.Parse(videoCassetteList.ProjectIDS);
            var videoArchive = db.VideoArchives.Where(x => x.videoProjectSeqNo == videoProjectSeqNo);
            ViewBag.projectName = videoArchive.FirstOrDefault().projectName;
            ViewBag.videoProjectSeqNo = videoProjectSeqNo;
            ViewBag.filingDesc = videoCassetteList.filingDesc;
            ViewBag.videoContent = videoCassetteList.videoContent;
            return View();
        }
        public void DownLoadFile(int? id,string filename)
        {
            var videoCassetteList = db.VideoCassetteList.Find(id);
            string ProjectIDS = videoCassetteList.ProjectIDS.ToString().Trim();
            string filingDesc = db.VideoCassetteList.Find(id).filingDesc.ToString().Trim();
            string xdlujing = "/声像资料/" + ProjectIDS + "/声像照片/" + filingDesc + "/"+filename;
            string folderFullName = Server.MapPath("~/声像资料/" + ProjectIDS + "/声像照片/" + filingDesc + "/");//文件存储路径

            string filePath = Request.MapPath(xdlujing);
            //以字符流的形式下载文件
            FileStream fs = new FileStream(filePath, FileMode.Open);
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

        // GET: VideoCassetteLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoCassetteList videoCassetteList = db.VideoCassetteList.Find(id);
            if (videoCassetteList == null)
            {
                return HttpNotFound();
            }
            return View(videoCassetteList);
        }

        // GET: VideoCassetteLists/Create
        public ActionResult Create(int? videoProjectSeqNo)
        {
            if (videoProjectSeqNo == null)
            {
                return Content("<script >alert('该工程缺失工程序号！');window.history.back();</script >");
            }
            return View();
        }

        // POST: VideoCassetteLists/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ProjectIDS,videoMinutesCount,opticalDiskNo,diskMinutesCount,startShootingTime,endShootingTime,shootingPerson,shootingDate,filingTime,filingDesc,filingPeople,checker,checkTime,bianzhiUnit,bianzhiDateStart,bianzhiTime,bianzhiDateEnd,videoContent")] VideoCassetteList videoCassetteList)
        {
            if (ModelState.IsValid)
            {
                db.VideoCassetteList.Add(videoCassetteList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(videoCassetteList);
        }

        // GET: VideoCassetteLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoCassetteList videoCassetteList = db.VideoCassetteList.Find(id);
            if (videoCassetteList == null)
            {
                return HttpNotFound();
            }
            return View(videoCassetteList);
        }

        // POST: VideoCassetteLists/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProjectIDS,videoMinutesCount,opticalDiskNo,diskMinutesCount,startShootingTime,endShootingTime,shootingPerson,shootingDate,filingTime,filingDesc,filingPeople,checker,checkTime,bianzhiUnit,bianzhiDateStart,bianzhiTime,bianzhiDateEnd,videoContent")] VideoCassetteList videoCassetteList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(videoCassetteList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(videoCassetteList);
        }

        // GET: VideoCassetteLists/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    VideoCassetteList videoCassetteList = db.VideoCassetteList.Find(id);
        //    if (videoCassetteList == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(videoCassetteList);
        //}

        //// POST: VideoCassetteLists/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    VideoCassetteList videoCassetteList = db.VideoCassetteList.Find(id);
        //    db.VideoCassetteList.Remove(videoCassetteList);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        public ActionResult Delete(long id)
        {
            VideoCassetteList videoCassetteList = db.VideoCassetteList.Find(id);
            string p1 = Server.MapPath("~/声像资料/" + videoCassetteList.ProjectIDS + "/声像照片/" + videoCassetteList.filingDesc);//文件存储路径
            string p2 = Server.MapPath("~/声像资料/" + videoCassetteList.ProjectIDS + "/声像视频/" + videoCassetteList.filingDesc);//文件存储路径         
            try
            {
                if (!Directory.Exists(p1))
                    Directory.CreateDirectory(p1);
                DirectoryInfo dir1 = new DirectoryInfo(p1);//删除该目录
                dir1.Delete(true);
                if (!Directory.Exists(p2))
                    Directory.CreateDirectory(p2);
                DirectoryInfo dir2 = new DirectoryInfo(p2);//删除该目录
                dir2.Delete(true);
                db.VideoCassetteList.Remove(videoCassetteList);
                db.SaveChanges();
                return RedirectToAction("Index", new { videoProjectSeqNo = videoCassetteList.ProjectIDS });
            }
            catch (Exception e)
            {
                return Content("<script >alert('删除文件夹失败，请核查！');window.history.back();</script >");
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
