using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using urban_archive.Models;

namespace urban_archive.Controllers
{
    
    public class UUIDController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        private gxArchivesContainer bb = new gxArchivesContainer();
        // GET: UUID
        [AllowAnonymous]
        public ActionResult Index(int id,string uuid)
        {
            //string uuid = "297d413364dc4e7d81e5858c1bfcfbb5";
            var test = from ad in db.ArchivesDetail
                       where (ad.UUID == uuid)
                       select ad;
            if (test.Count() == 0)
            {
                var test1 = from ad in bb.gxArchivesDetail
                            where (ad.UUID == uuid)
                            select ad;
                
                if (test1.Count() == 0)
                {
                    return HttpNotFound(); ;
                }
                //管线数据
                gxArchivesDetail gxArchivesDetail = test1.First();
                long paperProjectSeqNo = gxArchivesDetail.paperProjectSeqNo;
                var n = from a in bb.gxPaperArchives
                        where a.paperProjectSeqNo == paperProjectSeqNo
                        select a;
                gxPaperArchives gxPaperArchives = n.First();

                var n1 = from a in bb.gxProjectInfo
                         where a.projectID == gxPaperArchives.projectID
                         where a.isNB == "外部"
                         select a;
                gxProjectInfo gxProjectInfo = n1.First();

                ViewBag.projectName = gxProjectInfo.projectName;//工程名称
                ViewBag.location = gxProjectInfo.location;//工程地点
                ViewBag.developmentOrganization = gxProjectInfo.developmentOrganization;//建设单位
                ViewBag.devolonpentOrgContacter = gxProjectInfo.devolonpentOrgContacter;//建设单位联系人
                ViewBag.mobilephoneNoDevelopment = gxProjectInfo.mobilephoneNoDevelopment;//建设单位联系人电话

                ViewBag.constructionOrganization = gxProjectInfo.constructionOrganization;//施工单位
                ViewBag.constructionOrgContacter = gxProjectInfo.constructionOrgContacter;//施工单位法人
                ViewBag.telphoneNoDevelopment = gxProjectInfo.telphoneNoDevelopment;//施工单位电话

                ViewBag.disignOrganization = gxProjectInfo.disignOrganization;//设计单位
                ViewBag.designOrgaContacter = gxProjectInfo.designOrgaContacter;//设计单位联系人
                ViewBag.telphoneNoDesignOrga = gxProjectInfo.telphoneNoDesignOrga;//设计者

                ViewBag.jianliUnit = gxProjectInfo.jianliUnit;//监理单位
                ViewBag.jianliUnitContacter = gxProjectInfo.jianliUnitContacter;//监理单位负责人
                ViewBag.telphoneNoJianliUnit = gxProjectInfo.telphoneNoJianliUnit;//监理者

                ViewBag.Kcdw = gxProjectInfo.Kcdw;//勘察单位
                ViewBag.Kcdwlxr = gxProjectInfo.Kcdwlxr;//勘察单位负责人
                ViewBag.Kcdwlxdh = gxProjectInfo.Kcdwlxdh;//勘察单位电话


                //ViewBag.jgdate = Convert.ToDateTime(gxArchivesDetail.jgDate).ToString("yyyy-MM-dd");//进馆时间
                ViewBag.kaigongTime = Convert.ToDateTime(gxArchivesDetail.kaigongTime).ToString("yyyy-MM-dd");//开工时间
                ViewBag.jungongTime = Convert.ToDateTime(gxArchivesDetail.jungongTime).ToString("yyyy-MM-dd");//竣工时间
                ViewBag.archivesTitle = gxArchivesDetail.archivesTitle;//案卷题名
                ViewBag.firstResponsible = gxArchivesDetail.firstResponsible;//责任者
                ViewBag.bianzhiTime = gxArchivesDetail.bianzhiTime;//编制时间
                ViewBag.indexer = gxArchivesDetail.indexer;//立卷人
                ViewBag.indexDate = Convert.ToDateTime(gxArchivesDetail.indexDate).ToString("yyyy-MM-dd"); ;//立卷时间
                ViewBag.key1 = gxArchivesDetail.key1;//主题词
                ViewBag.remarks = gxArchivesDetail.remarks;//备注

                var gxfileInfo = from ad in bb.gxFileInfo
                               select ad;
                gxfileInfo = gxfileInfo.Where(ad => ad.archivesNo.ToString().Equals(gxArchivesDetail.archivesNo));//根据档号搜索
                gxfileInfo = gxfileInfo.OrderBy(s => s.ID).Take(1000);
                ViewBag.result = JsonConvert.SerializeObject(gxfileInfo);
                return View();
            }
            else
            {
                //竣工数据
                ArchivesDetail ArchivesDetail = test.First();
                long paperProjectSeqNo = ArchivesDetail.paperProjectSeqNo;
                var n = from a in db.PaperArchives
                        where a.paperProjectSeqNo == paperProjectSeqNo
                        select a;
                if (n.Count() == 0) {
                    return Content("<script >alert('找不到工程文件！');</script >");
                }
                PaperArchives PaperArchives = n.First();

                var n1 = from a in db.ProjectInfo
                        where a.projectID == PaperArchives.projectID
                        select a;
                ProjectInfo ProjectInfo = n1.First();

                ViewBag.projectName = ProjectInfo.projectName;//工程名称
                ViewBag.location = ProjectInfo.location;//工程地点
                ViewBag.developmentOrganization = ProjectInfo.developmentOrganization;//建设单位
                ViewBag.devolonpentOrgContacter = ProjectInfo.devolonpentOrgContacter;//建设单位联系人
                ViewBag.mobilephoneNoDevelopment = ProjectInfo.mobilephoneNoDevelopment;//建设单位联系人电话

                ViewBag.constructionOrganization = ProjectInfo.constructionOrganization;//施工单位
                ViewBag.constructionOrgContacter = ProjectInfo.constructionOrgContacter;//施工单位法人
                ViewBag.telphoneNoDevelopment = ProjectInfo.telphoneNoDevelopment;//施工单位电话

                ViewBag.disignOrganization = ProjectInfo.disignOrganization;//设计单位
                ViewBag.designOrgaContacter = ProjectInfo.designOrgaContacter;//设计单位联系人
                ViewBag.telphoneNoDesignOrga = ProjectInfo.telphoneNoDesignOrga;//设计者

                ViewBag.jianliUnit = ProjectInfo.jianliUnit;//监理单位
                ViewBag.jianliUnitContacter = ProjectInfo.jianliUnitContacter;//监理单位负责人
                ViewBag.telphoneNoJianliUnit = ProjectInfo.telphoneNoJianliUnit;//监理者

                ViewBag.Kcdw = ProjectInfo.Kcdw;//勘察单位
                ViewBag.Kcdwlxr = ProjectInfo.Kcdwlxr;//勘察单位负责人
                ViewBag.Kcdwlxdh = ProjectInfo.Kcdwlxdh;//勘察单位电话


                //ViewBag.jgdate = Convert.ToDateTime(ArchivesDetail.jgDate).ToString("yyyy-MM-dd");//进馆时间
                ViewBag.kaigongTime = Convert.ToDateTime(ArchivesDetail.kaigongTime).ToString("yyyy-MM-dd");//开工时间
                ViewBag.jungongTime = Convert.ToDateTime(ArchivesDetail.jungongTime).ToString("yyyy-MM-dd");//竣工时间
                ViewBag.archivesTitle = ArchivesDetail.archivesTitle;//案卷题名
                ViewBag.firstResponsible = ArchivesDetail.firstResponsible;//责任者
                ViewBag.bianzhiTime = ArchivesDetail.bianzhiTime;//编制时间
                ViewBag.indexer = ArchivesDetail.indexer;//立卷人
                ViewBag.indexDate = Convert.ToDateTime(ArchivesDetail.indexDate).ToString("yyyy-MM-dd"); ;//立卷时间
                ViewBag.key1 = ArchivesDetail.key1;//主题词
                ViewBag.remarks = ArchivesDetail.remarks;//备注

                var fileInfo = from ad in db.FileInfo
                                        select ad;
                fileInfo = fileInfo.Where(ad => ad.archivesNo.ToString().Equals(ArchivesDetail.archivesNo));//根据档号搜索
                fileInfo = fileInfo.OrderBy(s => s.id).Take(1000);
                ViewBag.result = JsonConvert.SerializeObject(fileInfo);

                if (ArchivesDetail == null)
                {
                    return HttpNotFound();
                }
                return View();
            }
        }
    }
}