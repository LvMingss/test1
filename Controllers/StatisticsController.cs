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
using Newtonsoft.Json;

namespace urban_archive.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Statistics
        private UrbanConEntities db = new UrbanConEntities();
        private gxArchivesContainer bb = new gxArchivesContainer();
        private VideoArchiveEntities vb = new VideoArchiveEntities();

        public ActionResult jgArchivesReceiveTJ(string startdate, string enddate)
        {
            var vwprojectFile = from ad in db.vw_projectList
                                select ad;
            ViewData["start"] = startdate;
            ViewData["end"] = enddate;
            if (startdate != "" && startdate != null && enddate != "" && enddate != null)//增加查询条件
            {
                DateTime start = DateTime.Parse(startdate.Trim());
                DateTime end = DateTime.Parse(enddate.Trim());
                vwprojectFile = from h in db.vw_projectList
                                where (h.dateReceived >= start) && (h.dateReceived <= end)
                                select h;
            }
            // 默认按责任书编号排 
            ViewBag.count = vwprojectFile.Count();
            vwprojectFile = vwprojectFile.OrderByDescending(s => s.paperProjectSeqNo);

            ViewBag.result = JsonConvert.SerializeObject(vwprojectFile);
            return View();
        }
        public ActionResult gxArchivesReceiveTJ(string startdate, string enddate)
        {
            var vwprojectFile = from ad in bb.vw_gxprojectList
                                select ad;
            ViewData["start"] = startdate;
            ViewData["end"] = enddate;
            if (startdate != "" && startdate != null && enddate != "" && enddate != null)//增加查询条件
            {
                DateTime start = DateTime.Parse(startdate.Trim());
                DateTime end = DateTime.Parse(enddate.Trim());
                vwprojectFile = from h in bb.vw_gxprojectList
                                where (h.dateReceived >= start) && (h.dateReceived <= end)
                                select h;
            }
            // 默认按责任书编号排 
            ViewBag.count = vwprojectFile.Count();
            vwprojectFile = vwprojectFile.OrderByDescending(s => s.paperProjectSeqNo);

            ViewBag.result = JsonConvert.SerializeObject(vwprojectFile);
            return View();
        }
        public ActionResult videoArchivesReceiveTJ(string startdate, string enddate)
        {
            var vwprojectFile = from ad in vb.VideoArchives
                                select ad;
            ViewData["start"] = startdate;
            ViewData["end"] = enddate;
            if (startdate != "" && startdate != null && enddate != "" && enddate != null)//增加查询条件
            {
                DateTime start = DateTime.Parse(startdate.Trim());
                DateTime end = DateTime.Parse(enddate.Trim());
                vwprojectFile = from h in vb.VideoArchives
                                where (h.dateReceived >= start) && (h.dateReceived <= end)
                                select h;
            }
            // 默认按责任书编号排 
            ViewBag.count = vwprojectFile.Count();
            vwprojectFile = vwprojectFile.OrderByDescending(s => s.videoProjectSeqNo);

            ViewBag.result = JsonConvert.SerializeObject(vwprojectFile);
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}