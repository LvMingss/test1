using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Drawing;
using urban_archive.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;
using System.Text.RegularExpressions;
using System.Collections;

namespace urban_archive.Controllers
{
    public class MapController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();

        public ActionResult ArchiveList(int projectid)
        {
            vw_projectList w = db.vw_projectList.SingleOrDefault(c => c.paperProjectSeqNo == projectid);
            //ProjectInfo p = db.ProjectInfo.Single(c => c.projectID == w.projectID);
            return RedirectToAction("LookFinshProject","ArchiveSearch", new { SerachText = "项目顺序号——包含——" + w.paperProjectSeqNo + "," });
        }

        public ActionResult ArchiveListDL(int projectid)
        {
            return RedirectToAction("LookQingArchives_Road_SingleArchiveDetailInfo", "ArchiveSearch2", new { id = projectid });
        }

        public ActionResult ArchiveListZZ(int projectid)
        {
            return RedirectToAction("LookQingArchives_Zhizhao_SingleArchiveDetailInfo", "ArchiveSearch2", new { id = projectid });
        }

        // GET: Map
        public ActionResult Index()
        {
            //var projectList = db.vw_projectList.ToList();
            //var projectList1 = from a in db.vw_projectList
            //                   where a.coordinate != null
            //                   where a.coordinate != ""
            //                   select a;
            //var projectList2 = projectList1.ToList();
            //ViewBag.projectList = Newtonsoft.Json.JsonConvert.SerializeObject(projectList2);
            return View();
        }
        //public string  Index1()
        //{
        //    var projectList = db.vw_projectList.ToList();

        //    return Newtonsoft.Json.JsonConvert.SerializeObject(projectList);
        //}
        public string  ismodel(string id)
        {

            var projectID = Int32.Parse(id.Trim());
            var model = from a in db.SpatialData
                        where a.projectID == projectID
                        select a.model;
            if(model.First()==null|| model.First()=="")
            {
                return "0";
            }
            return "1";
        }

        public void ExportTile()
        {
            int level = 11;
            int row = 128128;// 64000;
            int col = 6016;// 2944;
            string type = "shape";
            int size = 128;
            byte[] result = null;
            FileStream isBundle = null;
            FileStream isBundlx = null;
            string nullpicpath = Server.MapPath(@"~/Content/tile_error.png");

            string bundlesDir = Server.MapPath(@"~/Data/spatial/basemap/" + type + "/Layers/_alllayers");  //"F:\\data\\test1\\Layers\\_alllayers";
            string l = "0" + level;
            int lLength = l.Length;
            if (lLength > 2)
            {
                l = l.Substring(lLength - 2);
            }
            l = "L" + l;

            int rGroup = size * (row / size);
            string r = rGroup.ToString("X");
            //string r = "000" + rGroup.ToString("X");
            //int rLength = r.Length;
            while (r.Length < 4)
            {
                r = "0" + r;
            }
            //if (rLength > 4)
            //{
            //    r = r.Substring(rLength - 4);
            //}
            r = "R" + r;

            int cGroup = size * (col / size);
            string c = cGroup.ToString("X");
            int cLength = c.Length;
            while (c.Length < 4)
            {
                c = "0" + c;
            }
            c = "C" + c;

            string bundleBase = bundlesDir + "//" + l + "//" + r + c;
            string bundlxFileName = bundleBase + ".bundlx";
            string bundleFileName = bundleBase + ".bundle";

            for (int i = 0; i < 128; i++) //行
            {
                for (int j = 0; j < 128; j++) //列
                {
                    try
                    {
                        int index = size * i + j;
                        //行列号是整个范围内的，在某个文件中需要先减去前面文件所占有的行列号（都是128的整数）这样就得到在文件中的真是行列号
                        isBundlx = new FileStream(bundlxFileName, FileMode.Open, FileAccess.Read);
                        isBundlx.Seek(16 + 5 * index, SeekOrigin.Begin);
                        byte[] buffer = new byte[5];
                        isBundlx.Read(buffer, 0, 5);
                        long offset = (long)(buffer[0] & 0xff)
                               + (long)(buffer[1] & 0xff) * 256
                               + (long)(buffer[2] & 0xff) * 65536
                               + (long)(buffer[3] & 0xff) * 16777216
                               + (long)(buffer[4] & 0xff) * 4294967296L;

                        isBundle = new FileStream(bundleFileName, FileMode.Open, FileAccess.Read);
                        isBundle.Seek(offset, SeekOrigin.Begin);
                        byte[] lengthBytes = new byte[4];
                        isBundle.Read(lengthBytes, 0, 4);
                        int length = (int)(lengthBytes[0] & 0xff)
                                + (int)(lengthBytes[1] & 0xff) * 256
                                + (int)(lengthBytes[2] & 0xff) * 65536
                                + (int)(lengthBytes[3] & 0xff) * 16777216;
                        result = new byte[length];
                        //isBundle.Seek(offset+4, SeekOrigin.Begin);
                        isBundle.Read(result, 0, length);

                        System.IO.MemoryStream ms = new System.IO.MemoryStream(result);
                        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                        img.Save(bundlesDir + "//" + l + "//" + (row + j).ToString() + "_" + (col + i).ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);

                        Response.Write(i.ToString() + "," + j.ToString() + "</br>");
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            if (isBundle != null)
            {
                isBundle.Close();
                isBundlx.Close();
            }
            Response.Write("Done!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! </br>");
        }

        public void getTile(int level, int row, int col, string type)
        {
            int size = 128;
            byte[] result = null;
            FileStream isBundle = null;
            FileStream isBundlx = null;
            string nullpicpath = Server.MapPath(@"~/Content/images/tile_null.png");
            try
            {
                string bundlesDir = Server.MapPath(@"~/SpatialData/basemap/" + type + "/Layers/_alllayers");
                string l = "0" + level;
                int lLength = l.Length;
                if (lLength > 2)
                {
                    l = l.Substring(lLength - 2);
                }
                l = "L" + l;

                int rGroup = size * (row / size);
                string r = rGroup.ToString("X");
                //string r = "000" + rGroup.ToString("X");
                //int rLength = r.Length;
                while (r.Length < 4)
                {
                    r = "0" + r;
                }
                //if (rLength > 4)
                //{
                //    r = r.Substring(rLength - 4);
                //}
                r = "R" + r;

                int cGroup = size * (col / size);
                string c = cGroup.ToString("X");
                int cLength = c.Length;
                while (c.Length < 4)
                {
                    c = "0" + c;
                }
                c = "C" + c;

                string bundleBase = bundlesDir + "//" + l + "//" + r + c;
                string bundlxFileName = bundleBase + ".bundlx";
                string bundleFileName = bundleBase + ".bundle";

                int index = size * (col - cGroup) + (row - rGroup);
                //行列号是整个范围内的，在某个文件中需要先减去前面文件所占有的行列号（都是128的整数）这样就得到在文件中的真是行列号
                isBundlx = new FileStream(bundlxFileName, FileMode.Open, FileAccess.Read);
                isBundlx.Seek(16 + 5 * index, SeekOrigin.Begin);
                byte[] buffer = new byte[5];
                isBundlx.Read(buffer, 0, 5);
                long offset = (long)(buffer[0] & 0xff)
                       + (long)(buffer[1] & 0xff) * 256
                       + (long)(buffer[2] & 0xff) * 65536
                       + (long)(buffer[3] & 0xff) * 16777216
                       + (long)(buffer[4] & 0xff) * 4294967296L;

                isBundle = new FileStream(bundleFileName, FileMode.Open, FileAccess.Read);
                isBundle.Seek(offset, SeekOrigin.Begin);
                byte[] lengthBytes = new byte[4];
                isBundle.Read(lengthBytes, 0, 4);
                int length = (int)(lengthBytes[0] & 0xff)
                        + (int)(lengthBytes[1] & 0xff) * 256
                        + (int)(lengthBytes[2] & 0xff) * 65536
                        + (int)(lengthBytes[3] & 0xff) * 16777216;
                result = new byte[length];
                isBundle.Read(result, 0, length);

                //System.IO.MemoryStream ms = new System.IO.MemoryStream(result);
                //System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                //img.Save(bundlesDir + "//" + l + "//" + row.ToString() + "_" + col.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);

                Response.ContentType = "image/png";

                Response.BinaryWrite(result);
                Response.End();
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(nullpicpath, FileMode.Open, FileAccess.Read);
                Bitmap myImage = new Bitmap(fs);
                MemoryStream ms = new MemoryStream();
                myImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
            finally
            {
                if (isBundle != null)
                {
                    isBundle.Close();
                    isBundlx.Close();
                }
            }
        }


        public ActionResult VrView(int id)
        {
            SpatialData sd = db.SpatialData.Single(c => c.id == id);
            ViewData["modelname"] = string.IsNullOrEmpty(sd.model)? "SNS000_01" : sd.model;
            return View();
        }

        public ActionResult Vectorization()
        {
            //var polygon = SqlGeography.STGeomFromText(
            //       new SqlChars(
            //       new SqlString("POLYGON ((-114.01611328125 42.0003251483162, -114.0380859375 42.0003251483162,"
            //                + "-113.994140625 37.0200982013681, -109.05029296875 37.0200982013681, -109.09423828125 41.0130657870063, "
            //                + "-111.07177734375 41.0462168145206, -111.07177734375 42.0003251483162, -114.01611328125 42.0003251483162))",111)),
            //        4326);
            //var sql = "insert Cities (CityName,CityLocation) values ('test','" + polygon.ToString() + "')";
            //InsertToDB(sql);


            //var sql_query = "select CityLocation from Cities where ID = 5";
            //var result = QueryDB(sql_query);
            //var polygon_query = SqlGeography.STGeomFromText(
            //    new SqlChars(
            //    new SqlString(result)), 4326);
            //Console.WriteLine(polygon_query.ToString());
            return View();
        }

        public ActionResult VectorizationPL()
        {
            return View();
        }

        public ActionResult VectorizationZZ()
        {
            return View();
        }

        public ActionResult VectorizationDL()
        {
            return View();
        }


        public JsonResult GetProjectGeos()
        {
            List<VProjectGeos> datas = db.VProjectGeos.ToList();
            var result = from d in datas
                         select new {
                             id = d.id,
                             projectID = d.projectID,
                             projectName = d.projectName,
                             ploygon = d.ploygon.AsText(),//ToString(),
                             model = d.model,
                             paperProjectSeqNo = d.paperProjectSeqNo == null?d.projectID:d.paperProjectSeqNo
                         };

            /*神仙代码 fs 20190705 获取txt数据*/
            //string DirPath = "F:\\test.txt";
            //if (!Directory.Exists(DirPath))  //如果不存在就创建file文件夹
            //{
            //    Directory.CreateDirectory(DirPath);
            //}
            //FileStream fs = new FileStream((DirPath+ "\\test.txt"), FileMode.Open, FileAccess.ReadWrite);
          
            //StreamWriter sw = new StreamWriter(fs);
            //sw.Write(result);   //写入字符串
            //sw.Close();
            /*神仙代码 fs 20190705 获取txt数据*/

            return Json(new { datas = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectcod()
        {
            var data1 = from a in db.vw_projectList
                        where a.coordinate != ""
                        where a.coordinate != null
                        select a;

            List < vw_projectList > datas1 = data1.ToList();
            for (int i = 0; i < data1.Count(); i++)
            {
                Regex reg = new Regex(@"[\u4e00-\u9fa5a-zA-Z]");//正则表达式
                if (reg.IsMatch(datas1[i].coordinate.Trim()) == true)
                {
                    datas1[i].coordinate = "";
                }
            }

            var datas2 = from a in datas1
                        where a.coordinate != ""
                        select a;
            var data3 = datas2.ToArray();

            //ArrayList List = new ArrayList();
            List<vw_projectList> List = new List<vw_projectList>();
            for (int i = 0; i < data3.Length; i++)
            {
                data3[i].coordinate = data3[i].coordinate.Replace("，", ",");
                List.Add(data3[i]);

            }

            var result = from d in List
                         select new
                         {
                             paperProjectSeqNo = d.paperProjectSeqNo,
                             coordinate = d.coordinate,
                             projectName = d.projectName,
                         };
            //var cehuiPoint1 = data3[0].Split(',');
            //ArrayList List = new ArrayList();
            //for (int i = 0;i < data3.Length; i++) {
            //    data3[i] = data3[i].Replace("，", ",");
            //    List.Add(data3[i].Split(','));

            //}
            //var cehuiPoint1 = data3[0].Split(',');

            return Json(new { datas = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectGeosZZ()
        {
            List<VProjectGeosZZ> datas = db.VProjectGeosZZ.ToList();
            return Json(new { datas = datas }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectGeosDL()
        {
            List<VProjectGeosDL> datas = db.VProjectGeosDL.ToList();
            return Json(new { datas = datas }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetNearProjectGeos(FormCollection collection)
        {
            string geo = collection["geo"];
            SqlGeography point = SqlGeography.STPointFromText(new SqlChars(new SqlString(geo, 2052)), 4326);
            SqlGeography buffer = point.STBuffer(5);
            List<VProjectGeos> datas = db.VProjectGeos.Where(c => buffer.STIntersects(SqlGeography.STPointFromText(new SqlChars(new SqlString(c.ploygon.AsText(), 2052)), 4326)).IsTrue).ToList();
            return Json(new { datas = datas }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProjects(FormCollection collection)
        {
            string draw = collection["draw"].ToString();
            string start = collection["start"].ToString();
            string length = collection["length"].ToString();
            string globeSV = collection["search[value]"].ToString();
            string ordercolumn = collection["order[0][column]"].ToString();
            string ordertype = collection["order[0][dir]"].ToString();

            //string accountSV = collection["columns[2][search][value]"].ToString();
            //string nameSV = collection["columns[3][search][value]"].ToString();
            //string telSV = collection["columns[4][search][value]"].ToString();

            List<VProjectSpatialInfo> pros = db.VProjectSpatialInfo.ToList();
            int totalnum = pros.Count;
            if (!string.IsNullOrEmpty(globeSV))
            {
                pros = pros.Where(c => c.paperProjectSeqNo.ToString().Contains(globeSV)).ToList();
            }
            int filterednum = pros.Count;
            switch (ordercolumn)
            {
                case "1":
                    if (ordertype == "asc")
                        pros = pros.OrderBy(c => c.paperProjectSeqNo).ToList();
                    if (ordertype == "desc")
                        pros = pros.OrderByDescending(c => c.paperProjectSeqNo).ToList();
                    break;
                case "2":
                    if (ordertype == "asc")
                        pros = pros.OrderBy(c => c.projectName).ToList();
                    if (ordertype == "desc")
                        pros = pros.OrderByDescending(c => c.projectName).ToList();
                    break;
                default:
                    pros = pros.OrderByDescending(c => c.GeoID).ToList();
                    break;
            }
            pros = pros.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length)).ToList();
            var result = new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalnum,
                recordsFiltered = filterednum,
                data = pros
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProjectsZZ(FormCollection collection)
        {
            string draw = collection["draw"].ToString();
            string start = collection["start"].ToString();
            string length = collection["length"].ToString();
            string globeSV = collection["search[value]"].ToString();
            string ordercolumn = collection["order[0][column]"].ToString();
            string ordertype = collection["order[0][dir]"].ToString();

            //string accountSV = collection["columns[2][search][value]"].ToString();
            //string nameSV = collection["columns[3][search][value]"].ToString();
            //string telSV = collection["columns[4][search][value]"].ToString();

            List<VProjectSpatialInfoZZ> pros = db.VProjectSpatialInfoZZ.ToList();
            int totalnum = pros.Count;
            if (!string.IsNullOrEmpty(globeSV))
            {
                pros = pros.Where(c => c.projectRange.Contains(globeSV)).ToList();
            }
            int filterednum = pros.Count;
            switch (ordercolumn)
            {
                case "1":
                    if (ordertype == "asc")
                        pros = pros.OrderBy(c => c.licenceNo).ToList();
                    if (ordertype == "desc")
                        pros = pros.OrderByDescending(c => c.licenceNo).ToList();
                    break;
                case "2":
                    if (ordertype == "asc")
                        pros = pros.OrderBy(c => c.projectRange).ToList();
                    if (ordertype == "desc")
                        pros = pros.OrderByDescending(c => c.projectRange).ToList();
                    break;
                default:
                    pros = pros.OrderByDescending(c => c.GeoID).ToList();
                    break;
            }
            pros = pros.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length)).ToList();
            var result = new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalnum,
                recordsFiltered = filterednum,
                data = pros
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProjectsDL(FormCollection collection)
        {
            string draw = collection["draw"].ToString();
            string start = collection["start"].ToString();
            string length = collection["length"].ToString();
            string globeSV = collection["search[value]"].ToString();
            string ordercolumn = collection["order[0][column]"].ToString();
            string ordertype = collection["order[0][dir]"].ToString();

            //string accountSV = collection["columns[2][search][value]"].ToString();
            //string nameSV = collection["columns[3][search][value]"].ToString();
            //string telSV = collection["columns[4][search][value]"].ToString();

            List<VProjectSpatialInfoDL> pros = db.VProjectSpatialInfoDL.ToList();
            int totalnum = pros.Count;
            if (!string.IsNullOrEmpty(globeSV))
            {
                pros = pros.Where(c => c.location.Contains(globeSV)).ToList();
            }
            int filterednum = pros.Count;
            switch (ordercolumn)
            {
                case "1":
                    if (ordertype == "asc")
                        pros = pros.OrderBy(c => c.location).ToList();
                    if (ordertype == "desc")
                        pros = pros.OrderByDescending(c => c.location).ToList();
                    break;
                case "2":
                    if (ordertype == "asc")
                        pros = pros.OrderBy(c => c.volNo).ToList();
                    if (ordertype == "desc")
                        pros = pros.OrderByDescending(c => c.volNo).ToList();
                    break;
                default:
                    pros = pros.OrderByDescending(c => c.GeoID).ToList();
                    break;
            }
            pros = pros.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length)).ToList();
            var result = new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalnum,
                recordsFiltered = filterednum,
                data = pros
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void DeleteProjectGeo(FormCollection collection)
        {
            string id = collection["id"];
            try
            {
                string sql_query = "delete SpatialData where id = " + id;
                if (QueryDB(sql_query) == "1")
                {
                    Response.Write("ok");
                }
                else
                {
                    Response.Write("error");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        [HttpPost]
        public void DeleteProjectGeoZZ(FormCollection collection)
        {
            string id = collection["id"];
            try
            {
                string sql_query = "delete SpatialDataZZ where id = " + id;
                if (QueryDB(sql_query) == "1")
                {
                    Response.Write("ok");
                }
                else
                {
                    Response.Write("error");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        [HttpPost]
        public void DeleteProjectGeoDL(FormCollection collection)
        {
            string id = collection["id"];
            try
            {
                string sql_query = "delete SpatialDataDL where id = " + id;
                if (QueryDB(sql_query) == "1")
                {
                    Response.Write("ok");
                }
                else
                {
                    Response.Write("error");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        public ActionResult RGVectorizationContent()
        {
            return PartialView();
        }
        public ActionResult RGVectorizationContentZZ()
        {
            return PartialView();
        }
        public ActionResult RGVectorizationContentDL()
        {
            return PartialView();
        }
        public ActionResult EditVectorizationContent()
        {
            return PartialView();
        }
        public ActionResult EditVectorizationContentZZ()
        {
            return PartialView();
        }
        public ActionResult EditVectorizationContentDL()
        {
            return PartialView();
        }

        public ActionResult VectorizationContent(string g)
        {
            ViewData["g"] = Server.UrlDecode(g);
            return PartialView();
        }

        [HttpGet]
        public ActionResult VectorizationContentPL()
        {
            return PartialView();
        }

        //20190914 批量显示点击数据
        [HttpGet]
        public ActionResult ProjectInfoPL()
        {
            return PartialView();
        }

        [HttpPost]
        public string VectorizationContentPL(long startnum, long endnum)
        {
            var x = from c in db.vw_projectList
                    where c.coordinate != null && c.paperProjectSeqNo >= startnum && c.paperProjectSeqNo <= endnum
                    select new
                    {
                        paperProjectSeqNo1 = c.paperProjectSeqNo,
                        ProjectID = c.projectID,
                        Name = c.projectName,
                        Location = c.coordinate
                    };
            int totalnum = x.Count();
            ViewData["totalnum"] = totalnum;
            return Newtonsoft.Json.JsonConvert.SerializeObject(x);
        }

        public ActionResult VectorizationContentZZ(string g)
        {
            ViewData["g"] = Server.UrlDecode(g);
            return PartialView();
        }

        public ActionResult VectorizationContentDL(string g)
        {
            ViewData["g"] = Server.UrlDecode(g);
            return PartialView();
        }

        public JsonResult ProjectSelection(string q, int page)
        {
            var ps = db.tem_vwprojectList.AsQueryable();
            if (!string.IsNullOrEmpty(q))
            {
                ps = ps.Where(c=>c.paperProjectSeqNo.ToString().Contains(q));
            }
            int totalnum = ps.Count();
            ps = ps.OrderBy(c => c.paperProjectSeqNo).Skip((page - 1) * 20).Take(20);
            var datas = from pp in ps
                        select new
                        {
                            id = pp.projectID,
                            text = pp.paperProjectSeqNo+"-"+pp.projectName
                        };
            var result = new
            {
                total_count = totalnum,
                incomplete_results = false,
                items = datas
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProjectSelectionZZ(string q, int page)
        {
            var  ps = db.OtherArchives.Where(c=>c.classTypeID == 1);
            if (!string.IsNullOrEmpty(q))
            {
                ps = ps.Where(c => c.licenceNo != null && c.licenceNo.Contains(q));
            }
            int totalnum = ps.Count();
            ps = ps.OrderBy(c => c.licenceNo).Skip((page - 1) * 20).Take(20);
            var datas = from pp in ps
                        select new
                        {
                            id = pp.ID,
                            text = pp.licenceNo
                        };
            var result = new
            {
                total_count = totalnum,
                incomplete_results = false,
                items = datas
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProjectSelectionDL(string q, int page)
        {
            var ps = db.OtherArchives.Where(c => c.classTypeID == 2);
            if (!string.IsNullOrEmpty(q))
            {
                ps = ps.Where(c => c.year != null && c.volNo !=null && (c.year.Contains(q) || c.volNo.Contains(q) || (q.Contains(c.year.Trim()) && q.Contains(c.volNo.Trim()))));
            }
            int totalnum = ps.Count();
            ps = ps.OrderBy(c => c.year).ThenBy(c=>c.volNo).Skip((page - 1) * 20).Take(20);
            var datas = from pp in ps
                        select new
                        {
                            id = pp.ID,
                            text = pp.year + "-" + pp.volNo 
                        };
            var result = new
            {
                total_count = totalnum,
                incomplete_results = false,
                items = datas
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProjectVRModelSelection(string q, int page)
        {
            string modelPath = Server.MapPath("~/SpatialData/scenes/models/");
            DirectoryInfo di = new DirectoryInfo(modelPath);
            if (!di.Exists) return null;
            if (string.IsNullOrEmpty(q)) return null;
            List<DirectoryInfo> models = di.GetDirectories("*" + q + "*").ToList();            
            int totalnum = models.Count();
            models = models.OrderBy(c => c.Name).Skip((page - 1) * 20).Take(20).ToList();
            var datas = from m in models
                        select new
                        {
                            id = m.Name,
                            text = m.Name
                        };
            var result = new
            {
                total_count = totalnum,
                incomplete_results = false,
                items = datas
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void EditVectorization(FormCollection collection)
        {
            string id = collection["id"];
            string projectName = collection["projectname"];
            string projectID = collection["projectid"];
            string model = collection["vrmodel"];

            try
            {
                SpatialData sd = db.SpatialData.SingleOrDefault(c => c.id.ToString() == id);
                if (sd != null)
                {
                    if (projectID != "" && projectName != "")
                    {
                        sd.projectID = long.Parse(projectID);
                        sd.projectName = projectName;
                    }
                    if (model != "")
                    {
                        sd.model = model;
                    }
                    db.SaveChanges();
                    Response.Write("ok");
                }
            }
            catch(Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        [HttpPost]
        public void EditVectorizationZZ(FormCollection collection)
        {
            string id = collection["id"];
            string projectName = collection["projectname"];
            string projectID = collection["projectid"];

            try
            {
                SpatialDataZZ sd = db.SpatialDataZZ.SingleOrDefault(c => c.id.ToString() == id);
                if (sd != null)
                {
                    if (projectID != "" && projectName != "")
                    {
                        sd.projectID = long.Parse(projectID);
                        sd.projectName = projectName;
                    }
                    db.SaveChanges();
                    Response.Write("ok");
                }
            }
            catch (Exception ex) 
            {
                Response.Write(ex.Message);
            }
        }

        [HttpPost]
        public void EditVectorizationDL(FormCollection collection)
        {
            string id = collection["id"];
            string projectName = collection["projectname"];
            string projectID = collection["projectid"];

            try
            {
                SpatialDataDL sd = db.SpatialDataDL.SingleOrDefault(c => c.id.ToString() == id);
                if (sd != null)
                {
                    if (projectID != "" && projectName != "")
                    {
                        sd.projectID = long.Parse(projectID);
                        sd.projectName = projectName;
                    }
                    db.SaveChanges();
                    Response.Write("ok");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        [HttpPost]
        public void SaveVectorization(FormCollection collection)
        {
            string geo = collection["geo"];
            string projectName = collection["projectname"];
            string projectID = collection["projectid"];
            string model = collection["vrmodel"];

            try
            {
                SqlGeography polygon = SqlGeography.STPolyFromText(new SqlChars(new SqlString(geo, 2052)), 4326);
                var sql = "insert SpatialData (projectID,projectName,ploygon,model) values ('" + projectID + "','" + projectName + "','" + polygon.ToString() + "','" + model + "')";
                InsertToDB(sql);
                Response.Write("ok");
            }
            catch (Exception ex)
            {
                Response.Write("系统错误，" + ex.Message);
            }

        }

        [HttpPost]
        public void SaveVectorizationZZ(FormCollection collection)
        {
            string geo = collection["geo"];
            string projectName = collection["projectname"];
            string projectID = collection["projectid"];

            try
            {
                SqlGeography polygon = SqlGeography.STPolyFromText(new SqlChars(new SqlString(geo, 2052)), 4326);
                var sql = "insert SpatialDataZZ (projectID,projectName,ploygon) values ('" + projectID + "','" + projectName + "','" + polygon.ToString() + "')";
                InsertToDB(sql);
                Response.Write("ok");
            }
            catch (Exception ex)
            {
                Response.Write("系统错误，" + ex.Message);
            }

        }

        [HttpPost]
        public void SaveVectorizationDL(FormCollection collection)
        {
            string geo = collection["geo"];
            string projectName = collection["projectname"];
            string projectID = collection["projectid"];

            try
            {
                SqlGeography polygon = SqlGeography.STPolyFromText(new SqlChars(new SqlString(geo, 2052)), 4326);
                var sql = "insert SpatialDataDL (projectID,projectName,ploygon) values ('" + projectID + "','" + projectName + "','" + polygon.ToString() + "')";
                InsertToDB(sql);
                Response.Write("ok");
            }
            catch (Exception ex)
            {
                Response.Write("系统错误，" + ex.Message);
            }

        }

        public ActionResult ProjectInfo(int id)
        {
            //vw_projectList p = db.vw_projectList.SingleOrDefault(c => c.projectID == id);
            vw_projectList p = db.vw_projectList.SingleOrDefault(c => c.paperProjectSeqNo == id);
            return PartialView(p);
        }
        public ActionResult ProjectInfoZZ(int id)
        {
            OtherArchives p = db.OtherArchives.SingleOrDefault(c => c.ID == id);
            return PartialView(p);
        }
        public ActionResult ProjectInfoDL(int id)
        {
            OtherArchives p = db.OtherArchives.SingleOrDefault(c => c.ID == id);
            return PartialView(p);
        }

        private static void InsertToDB(string sql)
        {
            string scon = ConfigurationManager.ConnectionStrings["UrbanConConnectionString"].ConnectionString;
            using (var conn = new SqlConnection(scon))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    int row = cmd.ExecuteNonQuery();
                }
            }
        }

        private static string QueryDB(string sql)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UrbanConConnectionString"].ConnectionString))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    return cmd.ExecuteNonQuery().ToString();
                }
            }
        }
    }


}