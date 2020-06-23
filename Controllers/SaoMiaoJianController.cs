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

namespace urban_archive.Controllers
{
    public class SaoMiaoJianController : Controller
    {
        private UrbanConEntities db = new UrbanConEntities();
        private PlanArchiveEntities db_plan = new PlanArchiveEntities();
        private gxArchivesContainer db1 = new gxArchivesContainer();
        // GET: SaoMiaoJian
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SaoMiaoJianPiPeiJY(string action)
        {
            //if (action == "删除已匹配竣工档案") {
            //    string se = Request.Form["seq"];
            //    if (se != "") {
            //        long seq = long.Parse(se);
            //        var delete = db.ArchivesDetail.Where(a => a.paperProjectSeqNo == seq).ToList();
            //        for (int i = 0; i < delete.Count(); i++) {
            //            delete[i].isImageExist = "无";
            //            db.Entry(delete[i]).State = EntityState.Modified;
            //            db.SaveChanges();
            //        }
            //    }
            //}
            if (action == "开始匹配竣工档案") {
                string se = Request.Form["seq"];
                if ((long.Parse(se) < 10276))
                {
                    long ps = 0;
                    if (se != "")
                    {
                        if (!long.TryParse(se, out ps))
                        {

                            return Content("<script>alert('项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPeiJY'</script>");
                        }
                    }
                    DirectoryInfo theDir = new DirectoryInfo("G:\\JunGongArchives");
                    DirectoryInfo[] theSubDirs = theDir.GetDirectories();

                    foreach (DirectoryInfo seqNo in theSubDirs)
                    {

                        long seqNoName = long.Parse(seqNo.Name);

                        if (se == "")
                        {
                            DirectoryInfo[] danghaos = seqNo.GetDirectories();
                            foreach (DirectoryInfo danghao in danghaos)
                            {
                                var n = danghao.ToString().Split('-').Count();

                                string danghao2 = danghao.ToString();

                                if (n != 2)
                                {
                                    int danghao1 = int.Parse(danghao2);
                                    var OK = db.ArchivesDetail.Where(a => a.volNo == danghao1).ToList();
                                    for (int i = 0; i < OK.Count(); i++)
                                    {
                                        OK[i].isImageExist = "有";
                                        db.Entry(OK[i]).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                if (n == 2)
                                {
                                    var OK = db.ArchivesDetail.Where(a => a.archivesNo == danghao2).ToList();
                                    for (int i = 0; i < OK.Count(); i++)
                                    {
                                        OK[i].isImageExist = "有";
                                        db.Entry(OK[i]).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }

                            }
                        }
                       
                        if (se != "")
                        {
                            if (seqNoName == ps)
                            {
                                DirectoryInfo[] danghaos = seqNo.GetDirectories();
                                foreach (DirectoryInfo danghao in danghaos)
                                {
                                    var n = danghao.ToString().Split('-').Count();
                                    string danghao2 = danghao.ToString();
                                    if (n != 2)
                                    {
                                        int danghao1 = int.Parse(danghao2);
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo == ps).Where(a => a.volNo == danghao1).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                    if (n == 2)
                                    {
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo == ps).Where(a => a.archivesNo == danghao2).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if ((long.Parse(se) > 10276))
                {
                    long ps = 0;
                    if (se != "")
                    {
                        if (!long.TryParse(se, out ps))
                        {

                            return Content("<script>alert('项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPeiJY'</script>");
                        }
                    }
                    DirectoryInfo theDir = new DirectoryInfo("G:\\JunGongArchives1");
                    DirectoryInfo[] theSubDirs = theDir.GetDirectories();

                    foreach (DirectoryInfo seqNo in theSubDirs)
                    {

                        long seqNoName = long.Parse(seqNo.Name);

                        if (se == "")
                        {
                            DirectoryInfo[] danghaos = seqNo.GetDirectories();
                            foreach (DirectoryInfo danghao in danghaos)
                            {
                                var n = danghao.ToString().Split('-').Count();

                                string danghao2 = danghao.ToString();

                                if (n != 2)
                                {
                                    int danghao1 = int.Parse(danghao2);
                                    var OK = db.ArchivesDetail.Where(a => a.volNo == danghao1).ToList();
                                    for (int i = 0; i < OK.Count(); i++)
                                    {
                                        OK[i].isImageExist = "有";
                                        db.Entry(OK[i]).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                if (n == 2)
                                {
                                    var OK = db.ArchivesDetail.Where(a => a.archivesNo == danghao2).ToList();
                                    for (int i = 0; i < OK.Count(); i++)
                                    {
                                        OK[i].isImageExist = "有";
                                        db.Entry(OK[i]).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        } 
                        if (se != "")
                        {
                            if (seqNoName == ps)
                            {
                                DirectoryInfo[] danghaos = seqNo.GetDirectories();
                                foreach (DirectoryInfo danghao in danghaos)
                                {
                                    var n = danghao.ToString().Split('-').Count();
                                    string danghao2 = danghao.ToString();
                                    if (n != 2)
                                    {
                                        int danghao1 = int.Parse(danghao2);
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo == ps).Where(a => a.volNo == danghao1).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                    if (n == 2)
                                    {
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo == ps).Where(a => a.archivesNo == danghao2).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return Content("<script>alert('匹配完成！');window.location.href='SaoMiaoJianPiPeiJY'</script>");
            }
            return View();
        }
        public ActionResult SaoMiaoJianPiPei(string action)
        {
            if (action == "删除已匹配竣工档案")
            {
                string s = Request.Form["seqS"];
                string e = Request.Form["seqE"];
                if (s != "" && e != "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.ArchivesDetail.Where(a => a.paperProjectSeqNo >= seqs).Where(a => a.paperProjectSeqNo <= seqe).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (s != "" && e == "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.ArchivesDetail.Where(a => a.paperProjectSeqNo >= seqs).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (s == "" && e != "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.ArchivesDetail.Where(a => a.paperProjectSeqNo <= seqe).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            if (action == "开始匹配竣工档案")
            {
                string seqs = Request.Form["seqS"];
                string seqe = Request.Form["seqE"];
                if ((long.Parse(seqs) < 10276) && long.Parse(seqe) < 10276)
                {
                    long ps = 0, pe = 0;
                    if (seqs != "")
                    {
                        if (!long.TryParse(seqs, out ps))
                        {

                            return Content("<script>alert('开始项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPei'</script>");
                        }
                    }
                    if (seqe != "")
                    {
                        if (!long.TryParse(seqe, out pe))
                        {
                            return Content("<script>alert('结束项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPei'</script>"); ;
                        }
                    }
                    DirectoryInfo theDir = new DirectoryInfo("G:\\JunGongArchives");
                    DirectoryInfo[] theSubDirs = theDir.GetDirectories();

                    foreach (DirectoryInfo seqNo in theSubDirs)
                    {

                        long seqNoName = long.Parse(seqNo.Name);

                        if (seqs == "" && seqe == "")
                        {
                            DirectoryInfo[] danghaos = seqNo.GetDirectories();
                            foreach (DirectoryInfo danghao in danghaos)
                            {
                                var n = danghao.ToString().Split('-').Count();

                                string danghao2 = danghao.ToString();

                                if (n != 2)
                                {
                                    int danghao1 = int.Parse(danghao2);
                                    var OK = db.ArchivesDetail.Where(a => a.volNo == danghao1).ToList();
                                    for (int i = 0; i < OK.Count(); i++)
                                    {
                                        OK[i].isImageExist = "有";
                                        db.Entry(OK[i]).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                if (n == 2)
                                {
                                    var OK = db.ArchivesDetail.Where(a => a.archivesNo == danghao2).ToList();
                                    for (int i = 0; i < OK.Count(); i++)
                                    {
                                        OK[i].isImageExist = "有";
                                        db.Entry(OK[i]).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }

                            }
                        }
                        if (seqs != "" && seqe == "")
                        {
                            if (seqNoName >= ps)
                            {
                                DirectoryInfo[] danghaos = seqNo.GetDirectories();
                                foreach (DirectoryInfo danghao in danghaos)
                                {
                                    var n = danghao.ToString().Split('-').Count();
                                    string danghao2 = danghao.ToString();
                                    if (n != 2)
                                    {

                                        int danghao1 = int.Parse(danghao2);
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo >= ps).Where(a => a.volNo == danghao1).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                    if (n == 2)
                                    {
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo >= ps).Where(a => a.archivesNo == danghao2).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        if (seqs == "" && seqe != "")
                        {
                            if (seqNoName <= pe)
                            {
                                DirectoryInfo[] danghaos = seqNo.GetDirectories();
                                foreach (DirectoryInfo danghao in danghaos)
                                {
                                    var n = danghao.ToString().Split('-').Count();
                                    string danghao2 = danghao.ToString();
                                    if (n != 2)
                                    {

                                        int danghao1 = int.Parse(danghao2);
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo <= pe).Where(a => a.volNo == danghao1).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                    if (n == 2)
                                    {
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo <= pe).Where(a => a.archivesNo == danghao2).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        if (seqs != "" && seqe != "")
                        {
                            if (seqNoName <= pe && seqNoName >= ps)
                            {
                                DirectoryInfo[] danghaos = seqNo.GetDirectories();
                                foreach (DirectoryInfo danghao in danghaos)
                                {
                                    var n = danghao.ToString().Split('-').Count();
                                    string danghao2 = danghao.ToString();
                                    if (n != 2)
                                    {
                                        int danghao1 = int.Parse(danghao2);
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo <= pe).Where(a => a.paperProjectSeqNo >= ps).Where(a => a.volNo == danghao1).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                    if (n == 2)
                                    {
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo <= pe).Where(a => a.paperProjectSeqNo >= ps).Where(a => a.archivesNo == danghao2).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if ((long.Parse(seqs) > 10276) && long.Parse(seqe) > 10276)
                {
                    long ps = 0, pe = 0;
                    if (seqs != "")
                    {
                        if (!long.TryParse(seqs, out ps))
                        {

                            return Content("<script>alert('开始项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPei'</script>");
                        }
                    }
                    if (seqe != "")
                    {
                        if (!long.TryParse(seqe, out pe))
                        {
                            return Content("<script>alert('结束项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPei'</script>"); ;
                        }
                    }
                    DirectoryInfo theDir = new DirectoryInfo("G:\\JunGongArchives");
                    DirectoryInfo[] theSubDirs = theDir.GetDirectories();

                    foreach (DirectoryInfo seqNo in theSubDirs)
                    {
                        if (seqNo.Name == "temporary") {
                            continue;
                        }
                        long seqNoName = long.Parse(seqNo.Name);

                        if (seqs == "" && seqe == "")
                        {
                            DirectoryInfo[] danghaos = seqNo.GetDirectories();
                            foreach (DirectoryInfo danghao in danghaos)
                            {
                                var n = danghao.ToString().Split('-').Count();

                                string danghao2 = danghao.ToString();

                                if (n != 2)
                                {
                                    int danghao1 = int.Parse(danghao2);
                                    var OK = db.ArchivesDetail.Where(a => a.volNo == danghao1).ToList();
                                    for (int i = 0; i < OK.Count(); i++)
                                    {
                                        OK[i].isImageExist = "有";
                                        db.Entry(OK[i]).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                if (n == 2)
                                {
                                    var OK = db.ArchivesDetail.Where(a => a.archivesNo == danghao2).ToList();
                                    for (int i = 0; i < OK.Count(); i++)
                                    {
                                        OK[i].isImageExist = "有";
                                        db.Entry(OK[i]).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }

                            }
                        }
                        if (seqs != "" && seqe == "")
                        {
                            if (seqNoName >= ps)
                            {
                                DirectoryInfo[] danghaos = seqNo.GetDirectories();
                                foreach (DirectoryInfo danghao in danghaos)
                                {
                                    var n = danghao.ToString().Split('-').Count();
                                    string danghao2 = danghao.ToString();
                                    if (n != 2)
                                    {

                                        int danghao1 = int.Parse(danghao2);
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo >= ps).Where(a => a.volNo == danghao1).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                    if (n == 2)
                                    {
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo >= ps).Where(a => a.archivesNo == danghao2).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        if (seqs == "" && seqe != "")
                        {
                            if (seqNoName <= pe)
                            {
                                DirectoryInfo[] danghaos = seqNo.GetDirectories();
                                foreach (DirectoryInfo danghao in danghaos)
                                {
                                    var n = danghao.ToString().Split('-').Count();
                                    string danghao2 = danghao.ToString();
                                    if (n != 2)
                                    {

                                        int danghao1 = int.Parse(danghao2);
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo <= pe).Where(a => a.volNo == danghao1).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                    if (n == 2)
                                    {
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo <= pe).Where(a => a.archivesNo == danghao2).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        if (seqs != "" && seqe != "")
                        {
                            if (seqNoName <= pe && seqNoName >= ps)
                            {
                                DirectoryInfo[] danghaos = seqNo.GetDirectories();
                                foreach (DirectoryInfo danghao in danghaos)
                                {
                                    var n = danghao.ToString().Split('-').Count();
                                    string danghao2 = danghao.ToString();
                                    if (n != 2)
                                    {
                                        int danghao1 = int.Parse(danghao2);
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo <= pe).Where(a => a.paperProjectSeqNo >= ps).Where(a => a.volNo == danghao1).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                    if (n == 2)
                                    {
                                        var OK = db.ArchivesDetail.Where(a => a.paperProjectSeqNo <= pe).Where(a => a.paperProjectSeqNo >= ps).Where(a => a.archivesNo == danghao2).ToList();
                                        for (int i = 0; i < OK.Count(); i++)
                                        {
                                            OK[i].isImageExist = "有";
                                            db.Entry(OK[i]).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                return Content("<script>alert('匹配完成！');window.location.href='SaoMiaoJianPiPei'</script>");
            }

            if (action == "删除已匹配援川档案")
            {
                string s = Request.Form["YCseqS"];
                string e = Request.Form["YCseqE"];
                if (s != "" && e != "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.YuanChuanArchivesDetail.Where(a => a.paperProjectSeqNo >= seqs).Where(a => a.paperProjectSeqNo <= seqe).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (s != "" && e == "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.YuanChuanArchivesDetail.Where(a => a.paperProjectSeqNo >= seqs).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (s == "" && e != "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.YuanChuanArchivesDetail.Where(a => a.paperProjectSeqNo <= seqe).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            if (action == "开始匹配援川档案")
            {
                string seqs = Request.Form["YCseqS"];
                string seqe = Request.Form["YCseqE"];
                long ps = 0, pe = 0;
                if (seqs != "")
                {
                    if (long.TryParse(seqs, out ps))
                    {

                        return Content("<script>alert('开始项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPei'</script>");
                    }
                }
                if (seqe != "")
                {
                    if (long.TryParse(seqe, out pe))
                    {
                        return Content("<script>alert('结束项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPei'</script>"); ;
                    }
                }
                DirectoryInfo theDir = new DirectoryInfo("D:\\YuanChuanArchives");
                DirectoryInfo[] theSubDirs = theDir.GetDirectories();
                foreach (DirectoryInfo seqNo in theSubDirs)
                {
                    long seqNoName = long.Parse(seqNo.Name);
                    if (seqs == "" && seqe == "")
                    {
                        DirectoryInfo[] danghaos = seqNo.GetDirectories();
                        foreach (DirectoryInfo danghao in danghaos)
                        {
                            string danghao1 = danghao.ToString();
                            var OK = db.YuanChuanArchivesDetail.Where(a => a.archivesNo == danghao1).ToList();
                            for (int i = 0; i < OK.Count(); i++)
                            {
                                OK[i].isImageExist = "有";
                                db.Entry(OK[i]).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    if (seqs != "" && seqe == "")
                    {
                        if (seqNoName >= ps)
                        {
                            DirectoryInfo[] danghaos = seqNo.GetDirectories();
                            foreach (DirectoryInfo danghao in danghaos)
                            {
                                string danghao1 = danghao.ToString();
                                var OK = db.YuanChuanArchivesDetail.Where(a => a.archivesNo == danghao1).ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db.Entry(OK[i]).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    if (seqs == "" && seqe != "")
                    {
                        if (seqNoName <= pe)
                        {
                            DirectoryInfo[] danghaos = seqNo.GetDirectories();
                            foreach (DirectoryInfo danghao in danghaos)
                            {
                                string danghao1 = danghao.ToString();
                                var OK = db.YuanChuanArchivesDetail.Where(a => a.archivesNo == danghao1).ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db.Entry(OK[i]).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    if (seqs != "" && seqe != "")
                    {
                        if (seqNoName <= pe && seqNoName >= ps)
                        {
                            DirectoryInfo[] danghaos = seqNo.GetDirectories();
                            foreach (DirectoryInfo danghao in danghaos)
                            {
                                string danghao1 = danghao.ToString();
                                var OK = db.YuanChuanArchivesDetail.Where(a => a.archivesNo == danghao1).ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db.Entry(OK[i]).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
                return Content("<script>alert('匹配完成！');window.location.href='SaoMiaoJianPiPei'</script>");
            }

            if (action == "删除已匹配管线档案")
            {
                string s = Request.Form["GXseqS"];
                string e = Request.Form["GXseqE"];
                if (s != "" && e != "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db1.gxArchivesDetail.Where(a => a.paperProjectSeqNo >= seqs).Where(a => a.paperProjectSeqNo <= seqe).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db1.Entry(del[i]).State = EntityState.Modified;
                        db1.SaveChanges();
                    }
                }
                if (s != "" && e == "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db1.gxArchivesDetail.Where(a => a.paperProjectSeqNo >= seqs).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db1.Entry(del[i]).State = EntityState.Modified;
                        db1.SaveChanges();
                    }
                }
                if (s == "" && e != "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db1.gxArchivesDetail.Where(a => a.paperProjectSeqNo <= seqe).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db1.Entry(del[i]).State = EntityState.Modified;
                        db1.SaveChanges();
                    }
                }
            }
            if (action == "开始匹配管线档案")
            {
                string seqs = Request.Form["GXseqS"];
                string seqe = Request.Form["GXseqE"];
                long ps = 0, pe = 0;
                if (seqs != "")
                {
                    if (!long.TryParse(seqs, out ps))
                    {
                        return Content("<script>alert('开始项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPei'</script>");
                    }
                }
                if (seqe != "")
                {
                    if (!long.TryParse(seqe, out pe))
                    {
                        return Content("<script>alert('结束项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPei'</script>"); ;
                    }
                }
                DirectoryInfo theDir = new DirectoryInfo("H:\\GuanXian");
                DirectoryInfo[] theSubDirs = theDir.GetDirectories();
                foreach (DirectoryInfo seqNo in theSubDirs)
                {
                    long seqNoName = long.Parse(seqNo.Name);
                    if (seqs == "" && seqe == "")
                    {
                        DirectoryInfo[] danghaos = seqNo.GetDirectories();
                        foreach (DirectoryInfo danghao in danghaos)
                        {
                            string danghao1 = danghao.ToString();
                            var OK = db1.gxArchivesDetail.Where(a => a.archivesNo == danghao1).ToList();
                            for (int i = 0; i < OK.Count(); i++)
                            {
                                OK[i].isImageExist = "有";
                                db1.Entry(OK[i]).State = EntityState.Modified;
                                db1.SaveChanges();
                            }
                        }
                    }
                    if (seqs != "" && seqe == "")
                    {
                        if (seqNoName >= ps)
                        {
                            DirectoryInfo[] danghaos = seqNo.GetDirectories();
                            foreach (DirectoryInfo danghao in danghaos)
                            {
                                string danghao1 = danghao.ToString();
                                var OK = db1.gxArchivesDetail.Where(a => a.archivesNo == danghao1).ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db1.Entry(OK[i]).State = EntityState.Modified;
                                    db1.SaveChanges();
                                }
                            }
                        }
                    }
                    if (seqs == "" && seqe != "")
                    {
                        if (seqNoName <= pe)
                        {
                            DirectoryInfo[] danghaos = seqNo.GetDirectories();
                            foreach (DirectoryInfo danghao in danghaos)
                            {
                                string danghao1 = danghao.ToString();
                                var OK = db1.gxArchivesDetail.Where(a => a.archivesNo == danghao1).ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db1.Entry(OK[i]).State = EntityState.Modified;
                                    db1.SaveChanges();
                                }
                            }
                        }
                    }
                    if (seqs != "" && seqe != "")
                    {
                        if (seqNoName <= pe && seqNoName >= ps)
                        {
                            DirectoryInfo[] danghaos = seqNo.GetDirectories();
                            foreach (DirectoryInfo danghao in danghaos)
                            {
                                string danghao1 = danghao.ToString();
                                var OK = db1.gxArchivesDetail.Where(a => a.archivesNo == danghao1).ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db1.Entry(OK[i]).State = EntityState.Modified;
                                    db1.SaveChanges();
                                }                                
                            }
                        }
                    }
                }
                return Content("<script>alert('匹配完成！');window.location.href='SaoMiaoJianPiPei'</script>");
            }

            
            if (action == "开始匹配规划档案")
            {
                string seqs = long.Parse(Request.Form["GHseqS"]).ToString();
                string seqe = long.Parse(Request.Form["GHseqE"]).ToString();
                long ps = 0, pe = 0;
                if (seqs != "")
                {
                    if (!long.TryParse(seqs, out ps))
                    {

                        return Content("<script>alert('开始项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPei'</script>");
                    }
                }
                if (seqe != "")
                {
                    if (!long.TryParse(seqe, out pe))
                    {
                        return Content("<script>alert('结束项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPei'</script>"); ;
                    }
                }
                DirectoryInfo theDir = new DirectoryInfo("H:\\PlanArchives");
                DirectoryInfo[] theSubDirs = theDir.GetDirectories();
                foreach (DirectoryInfo totalSeqNo in theSubDirs)
                {
                    string seqNoName = long.Parse(totalSeqNo.Name).ToString();
                    if (seqs == "" && seqe == "")
                    {
                        if (totalSeqNo.GetDirectories().Length > 0)
                        {
                            foreach (DirectoryInfo volNo in totalSeqNo.GetDirectories())
                            {
                                seqNoName = totalSeqNo.Name + "-" + volNo.Name;
                                var OK = db_plan.PlanProject.Where(a => a.totalSeqNo == seqNoName).ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db_plan.Entry(OK[i]).State = EntityState.Modified;
                                    db_plan.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            var OK = db_plan.PlanProject.Where(a => a.totalSeqNo == seqNoName).ToList();
                            for (int i = 0; i < OK.Count(); i++)
                            {
                                OK[i].isImageExist = "有";
                                db_plan.Entry(OK[i]).State = EntityState.Modified;
                                db_plan.SaveChanges();
                            }
                        }
                    }
                    if (seqs != "" && seqe == "")
                    {
                        long seqNoName1 = long.Parse(seqNoName);
                        long seqs1 = long.Parse(seqs);
                        long seqe1 = long.Parse(seqe);
                        if (seqNoName1 >= seqs1)
                            //if (string.Compare(seqNoName, seqs) >= 0)
                        {
                            if (totalSeqNo.GetDirectories().Length > 0)
                            {
                                foreach (DirectoryInfo volNo in totalSeqNo.GetDirectories())
                                {
                                    seqNoName = totalSeqNo.Name + "-" + volNo.Name;
                                    var OK = db_plan.PlanProject.Where(a => a.totalSeqNo == seqNoName).ToList();
                                    for (int i = 0; i < OK.Count(); i++)
                                    {
                                        OK[i].isImageExist = "有";
                                        db_plan.Entry(OK[i]).State = EntityState.Modified;
                                        db_plan.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                var OK = db_plan.PlanProject.Where(a => a.totalSeqNo == seqNoName).ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db_plan.Entry(OK[i]).State = EntityState.Modified;
                                    db_plan.SaveChanges();
                                }
                            }
                        }
                    }
                    if (seqs == "" && seqe != "")
                    {
                        long seqNoName1 = long.Parse(seqNoName);
                        long seqs1 = long.Parse(seqs);
                        long seqe1 = long.Parse(seqe);
                        if (seqNoName1 <= seqe1)
                            //if (string.Compare(seqNoName, seqe) <= 0)
                        {
                            if (totalSeqNo.GetDirectories().Length > 0)
                            {
                                foreach (DirectoryInfo volNo in totalSeqNo.GetDirectories())
                                {
                                    seqNoName = totalSeqNo.Name + "-" + volNo.Name;
                                    var OK = db_plan.PlanProject.Where(a => a.totalSeqNo == seqNoName).ToList();
                                    for (int i = 0; i < OK.Count(); i++)
                                    {
                                        OK[i].isImageExist = "有";
                                        db_plan.Entry(OK[i]).State = EntityState.Modified;
                                        db_plan.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                var OK = db_plan.PlanProject.Where(a => a.totalSeqNo == seqNoName).ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db_plan.Entry(OK[i]).State = EntityState.Modified;
                                    db_plan.SaveChanges();
                                }
                            }
                        }
                    }
                    if (seqe != "" && seqs != "")
                    {
                        long seqNoName1 = long.Parse(seqNoName);
                        long seqs1 = long.Parse(seqs);
                        long seqe1 = long.Parse(seqe);
                        if(seqNoName1>=seqs1&& seqNoName1<=seqe1)
                        //if (string.Compare(seqNoName, seqe) <= 0 && string.Compare(seqNoName, seqs) >= 0)
                        {
                            if (totalSeqNo.GetDirectories().Length > 0)
                            {
                                foreach (DirectoryInfo volNo in totalSeqNo.GetDirectories())
                                {
                                    seqNoName = totalSeqNo.Name.PadLeft(8,'0') + "-" + volNo.Name;
                                    var OK = db_plan.PlanProject.Where(a => a.totalSeqNo == seqNoName).ToList();
                                    for (int i = 0; i < OK.Count(); i++)
                                    {
                                        OK[i].isImageExist = "有";
                                        db_plan.Entry(OK[i]).State = EntityState.Modified;
                                        db_plan.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                seqNoName = totalSeqNo.Name.PadLeft(8, '0');
                                var OK = db_plan.PlanProject.Where(a => a.totalSeqNo == seqNoName).ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db_plan.Entry(OK[i]).State = EntityState.Modified;
                                    db_plan.SaveChanges();
                                }                               
                            }
                        }
                    }
                }
                return Content("<script>alert('匹配完成！');window.location.href='SaoMiaoJianPiPei'</script>");
            }

            if (action == "删除已匹配分类档案")
            {
                string s = Request.Form["NoS"];
                string e = Request.Form["NoE"];
                if (s != "" && e != "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.OtherArchives.Where(a => a.archiveNo.CompareTo(e) <=0).Where(a => a.archiveNo.CompareTo(s) >= 0).Where(a => a.classNo == "3").ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (s != "" && e == "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.OtherArchives.Where(a => a.archiveNo.CompareTo(s) >= 0).Where(a => a.classNo == "3").ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (s == "" && e != "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.OtherArchives.Where(a => a.archiveNo.CompareTo(e) <= 0).Where(a => a.classNo == "3").ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            if (action == "开始匹配分类档案")
            {
                string nos = Request.Form["NoS"];
                string noe = Request.Form["NoE"];
                //long ps = 0, pe = 0;
                //if (nos != "")
                //{
                //    if (!long.TryParse(nos, out ps))
                //    {
                //        return Content("<script>alert('开始项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPei'</script>");
                //    }
                //}
                //if (noe != "")
                //{
                //    if (!long.TryParse(noe, out pe))
                //    {
                //        return Content("<script>alert('结束项目顺序号输入格式不正确，请输入数字！');window.location.href='SaoMiaoJianPiPei'</script>"); ;
                //    }
                //}
                DirectoryInfo theDir = new DirectoryInfo("H:\\OtherArchives\\Classify");
                DirectoryInfo[] theSubDirs = theDir.GetDirectories();
                foreach (DirectoryInfo archiveNo in theSubDirs)
                {
                    string archiveno = archiveNo.Name;
                    if (nos == "" && noe == "")
                    {
                        var OK = db.OtherArchives.Where(a => a.archiveNo == archiveno).Where(a=>a.classNo=="3").ToList();
                        for (int i = 0; i < OK.Count(); i++)
                        {
                            OK[i].isImageExist = "有";
                            db.Entry(OK[i]).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    if (nos != "" && noe == "")
                    {
                        if (string.Compare(archiveno, noe) >= 0)
                        {
                            var OK = db.OtherArchives.Where(a => a.archiveNo == archiveno).Where(a => a.classNo == "3").ToList();
                            for (int i = 0; i < OK.Count(); i++)
                            {
                                OK[i].isImageExist = "有";
                                db.Entry(OK[i]).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    if (nos == "" && noe != "")
                    {
                        if (string.Compare(archiveno, noe) <= 0)
                        {
                            var OK = db.OtherArchives.Where(a => a.archiveNo == archiveno).Where(a => a.classNo == "3").ToList();
                            for (int i = 0; i < OK.Count(); i++)
                            {
                                OK[i].isImageExist = "有";
                                db.Entry(OK[i]).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    if (nos != "" && noe != "")
                    {
                        if (string.Compare(archiveno, noe) <= 0 && string.Compare(archiveno, nos) >= 0)
                        {
                            var OK = db.OtherArchives.Where(a => a.archiveNo == archiveno).Where(a => a.classNo == "3").ToList();
                            for (int i = 0; i < OK.Count(); i++)
                            {
                                OK[i].isImageExist = "有";
                                db.Entry(OK[i]).State = EntityState.Modified;
                                db.SaveChanges();
                            }                            
                        }
                    }
                }
                return Content("<script>alert('匹配完成！');window.location.href='SaoMiaoJianPiPei'</script>");
            }

            if (action == "删除已匹配道路档案")
            {
                string s = Request.Form["daoluYear"];
                if (s != "" )
                {
                    var del = db.OtherArchives.Where(a => a.year == s).Where(a => a.classTypeID == 2).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
               
            }
            if (action == "开始匹配道路档案")
            {
                string year = Request.Form["daoluYear"];
                if (year == "")
                {
                    return Content("<script>alert('请输入匹配年份！');window.location.href='SaoMiaoJianPiPei'</script>");;
                }
                if(year!="")
                {
                    DirectoryInfo theLicenceDirOfYear = new DirectoryInfo("H:\\OtherArchives\\Road");
                    DirectoryInfo[] theLicenceDirOfYear1 = theLicenceDirOfYear.GetDirectories();

                    foreach (DirectoryInfo yearno in theLicenceDirOfYear1)
                    {
                        
                        //long yearNo = long.Parse(yearno.Name);
                        if (year == yearno.ToString()) {
                            DirectoryInfo[] volNos = yearno.GetDirectories();
                            foreach (DirectoryInfo volNo in volNos)
                            {
                                string vol = volNo.ToString().Trim();
                                vol = vol.PadLeft(4, '0');
                                var OK = db.OtherArchives.Where(a => a.volNo == vol).Where(a => a.classTypeID == 2).Where(a => a.year == year).ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db.Entry(OK[i]).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                            }
                        }
                        
                    }
                    return Content("<script>alert('匹配完成！');window.location.href='SaoMiaoJianPiPei'</script>");
                }
            }

            if (action == "删除已匹配图纸档案")
            {
                string s = Request.Form["TZseqS"];
                string e = Request.Form["TZseqE"];
                if (s != "" && e != "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.TuzhiArchives.Where(a => a.seqNo >= seqs).Where(a => a.seqNo <= seqe).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (s != "" && e == "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.TuzhiArchives.Where(a => a.seqNo >= seqs).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (s == "" && e != "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.TuzhiArchives.Where(a => a.seqNo <= seqe).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            if (action == "开始匹配图纸档案")
            {
                long? tuzhiseqNoS = long.Parse(Request.Form["TZseqS"]); 
                long? tuzhiseqNoE = long.Parse(Request.Form["TZseqE"]);

                DirectoryInfo theDir = new DirectoryInfo("H:\\OtherArchives\\Tuzhi");
                DirectoryInfo[] theSubDirs = theDir.GetDirectories();

                foreach (DirectoryInfo seqNo in theSubDirs)
                {
                    long seqNoName = long.Parse(seqNo.Name);
                    if (tuzhiseqNoS == null && tuzhiseqNoE == null)
                    {
                        var OK = db.TuzhiArchives.Where(a => a.seqNo == seqNoName).ToList();
                        for (int i = 0; i < OK.Count(); i++)
                        {
                            OK[i].isImageExist = "有";
                            db.Entry(OK[i]).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    if (tuzhiseqNoS != null && tuzhiseqNoE == null)
                    {
                        if (seqNoName>=tuzhiseqNoS)
                        {
                            var OK = db.TuzhiArchives.Where(a => a.seqNo == seqNoName).ToList();
                            for (int i = 0; i < OK.Count(); i++)
                            {
                                OK[i].isImageExist = "有";
                                db.Entry(OK[i]).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    if (tuzhiseqNoS == null && tuzhiseqNoE != null)
                    {
                        if (seqNoName<=tuzhiseqNoE)
                        {
                            var OK = db.TuzhiArchives.Where(a => a.seqNo == seqNoName).ToList();
                            for (int i = 0; i < OK.Count(); i++)
                            {
                                OK[i].isImageExist = "有";
                                db.Entry(OK[i]).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    if (tuzhiseqNoE !=null && tuzhiseqNoS !=null)
                    {
                        if (seqNoName<=tuzhiseqNoE &&seqNoName>=tuzhiseqNoS)
                        {
                            var OK = db.TuzhiArchives.Where(a => a.seqNo == seqNoName).ToList();
                            for (int i = 0; i < OK.Count(); i++)
                            {
                                OK[i].isImageExist = "有";
                                db.Entry(OK[i]).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return Content("<script>alert('匹配完成！');window.location.href='SaoMiaoJianPiPei'</script>");
            }

            if (action == "删除已匹配征地档案")
            {
                string s = Request.Form["ZDarchiveS"];
                string e = Request.Form["ZDarchiveE"];
                if (s != "" && e != "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.zdArchive.Where(a => a.seqNo >= seqs).Where(a => a.seqNo <= seqe).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (s != "" && e == "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.zdArchive.Where(a => a.seqNo >= seqs).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (s == "" && e != "")
                {
                    long seqs = long.Parse(s);
                    long seqe = long.Parse(e);
                    var del = db.zdArchive.Where(a => a.seqNo <= seqe).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            if (action == "开始匹配征地档案")
            {
                string zdarchiveNoS = Request.Form["ZDarchiveS"];
                string zdarchiveNoE = Request.Form["ZDarchiveE"];
                DirectoryInfo theDir = new DirectoryInfo("H:\\ZDArchives");
                DirectoryInfo[] theSubDirs = theDir.GetDirectories();

                foreach (DirectoryInfo archiveNo in theSubDirs)
                {
                    string seqNoName = archiveNo.Name;
                    if (zdarchiveNoS == "" && zdarchiveNoE == "")
                    {
                        var OK = db.zdArchive.Where(a => a.regisNo == seqNoName).ToList();
                        for (int i = 0; i < OK.Count(); i++)
                        {
                            OK[i].isImageExist = "有";
                            db.Entry(OK[i]).State = EntityState.Modified;
                            db.SaveChanges(); 
                        }
                    }
                    if (zdarchiveNoS != "" && zdarchiveNoE == "")
                    {
                        if (string.Compare(seqNoName, zdarchiveNoE) >= 0)
                        {
                            var OK = db.zdArchive.Where(a => a.regisNo == seqNoName).ToList();
                            for (int i = 0; i < OK.Count(); i++)
                            {
                                OK[i].isImageExist = "有";
                                db.Entry(OK[i]).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    if (zdarchiveNoS == "" && zdarchiveNoE != "")
                    {
                        if (string.Compare(seqNoName, zdarchiveNoE) <= 0)
                        {
                            var OK = db.zdArchive.Where(a => a.regisNo == seqNoName).ToList();
                            for (int i = 0; i < OK.Count(); i++)
                            {
                                OK[i].isImageExist = "有";
                                db.Entry(OK[i]).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    if (zdarchiveNoE != "" && zdarchiveNoS != "")
                    {
                        if (string.Compare(seqNoName, zdarchiveNoE) <= 0 && string.Compare(seqNoName, zdarchiveNoS) >= 0)
                        {
                            var OK = db.zdArchive.Where(a => a.regisNo == seqNoName).ToList();
                            for (int i = 0; i < OK.Count(); i++)
                            {
                                OK[i].isImageExist = "有";
                                db.Entry(OK[i]).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return Content("<script>alert('匹配完成！');window.location.href='SaoMiaoJianPiPei'</script>");
            }

            if (action == "删除已匹配执照档案（新）")
            {
                string s = Request.Form["zhizhaoYear"];
                if (s != "")
                {
                    var del = db.OtherArchives.Where(a => a.year == s).Where(a => a.classTypeID == 1).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

            }
            if (action == "开始匹配执照档案（新）")
            {
                string year = Request.Form["zhizhaoYear"];
                if (year == "")
                {
                    return Content("<script>alert('请输入匹配年份！');window.location.href='SaoMiaoJianPiPei'</script>"); ;
                }
                if (year != "")
                {
                    DirectoryInfo theLicenceDirOfYear = new DirectoryInfo("H:\\OtherArchives\\License\\"+year);
                    DirectoryInfo[] licenveNo = theLicenceDirOfYear.GetDirectories();
                    foreach (DirectoryInfo volNos in licenveNo)
                    {
                        string volNo = volNos.Name.ToString();
                        DirectoryInfo[] volNos1 = volNos.GetDirectories();
                        if (volNos1.Count() > 0)
                        {
                            foreach (DirectoryInfo volNo1 in volNos1)
                            {
                                if (int.Parse(year)<2007)
                                {
                                    string vol1 = "";
                                    if (volNo1.ToString() == "内部")
                                    {
                                        vol1 = volNo;
                                    }
                                    else
                                    {
                                        vol1 = volNo + "-00" + volNo1.ToString().Trim();
                                    }
                                    var OK = db.OtherArchives.Where(a => a.licenceNo == vol1).Where(a => a.classTypeID == 1).ToList();
                                    for (int i = 0; i < OK.Count(); i++)
                                    {
                                        OK[i].isImageExist = "有";
                                        db.Entry(OK[i]).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    string vol1 = "";
                                    if (volNo1.ToString() == "内部")
                                    {
                                        vol1 = year + "-" + volNo;
                                    }
                                    else
                                    {
                                        vol1 = year + "-"+volNo + "-00" + volNo1.ToString().Trim();
                                    }
                                    var OK = db.OtherArchives.Where(a => a.licenceNo == vol1).Where(a => a.classTypeID == 1).ToList();
                                    for (int i = 0; i < OK.Count(); i++)
                                    {
                                        OK[i].isImageExist = "有";
                                        db.Entry(OK[i]).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (int.Parse(year) < 2007)
                            {
                                var OK = db.OtherArchives.Where(a => a.licenceNo == volNo).Where(a => a.classTypeID == 1).ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db.Entry(OK[i]).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                string volNo1 = year + "-" + volNo;
                                var OK = db.OtherArchives.Where(a => a.licenceNo == volNo1).Where(a => a.classTypeID == 1).ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db.Entry(OK[i]).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    return Content("<script>alert('匹配完成！');window.location.href='SaoMiaoJianPiPei'</script>");
                }
            }

            //if (action == "删除已匹配执照档案（区）")
            //{
            //    string s = Request.Form["zhizhaoYear"];
            //    if (s != "")
            //    {
            //        var del = db.OtherArchives.Where(a => a.year == s).Where(a => a.classTypeID == 1).ToList();
            //        for (int i = 0; i < del.Count(); i++)
            //        {
            //            del[i].isImageExist = "无";
            //            db.Entry(del[i]).State = EntityState.Modified;
            //            db.SaveChanges();
            //        }
            //    }

            //}
            //if (action == "开始匹配执照档案（区）")
            //{
            //    string year = Request.Form["zhizhaoYear"];
            //    if (year == "")
            //    {
            //        return Content("<script>alert('请输入匹配年份！');window.location.href='SaoMiaoJianPiPei'</script>"); ;
            //    }
            //    if (year != "")
            //    {
            //        DirectoryInfo theLicenceDirOfYear = new DirectoryInfo("H:\\OtherArchives\\License\\" + year);
            //        DirectoryInfo[] licenveNo = theLicenceDirOfYear.GetDirectories();
            //        foreach (DirectoryInfo volNos in licenveNo)
            //        {
            //            string volNo = volNos.Name.ToString();
            //            DirectoryInfo[] volNos1 = volNos.GetDirectories();
            //            if (volNos1.Count() > 0)
            //            {
            //                foreach (DirectoryInfo volNo1 in volNos1)
            //                {
            //                    if (int.Parse(year) < 2007)
            //                    {
            //                        string vol1 = "";
            //                        if (volNo1.ToString() == "内部")
            //                        {
            //                            vol1 = volNo;
            //                        }
            //                        else
            //                        {
            //                            vol1 = volNo + "-00" + volNo1.ToString().Trim();
            //                        }
            //                        var OK = db.OtherArchives.Where(a => a.licenceNo == vol1).Where(a => a.classTypeID == 1).ToList();
            //                        for (int i = 0; i < OK.Count(); i++)
            //                        {
            //                            OK[i].isImageExist = "有";
            //                            db.Entry(OK[i]).State = EntityState.Modified;
            //                            db.SaveChanges();
            //                        }
            //                    }
            //                    else
            //                    {
            //                        string vol1 = "";
            //                        if (volNo1.ToString() == "内部")
            //                        {
            //                            vol1 = year + "-" + volNo;
            //                        }
            //                        else
            //                        {
            //                            vol1 = year + "-" + volNo + "-00" + volNo1.ToString().Trim();
            //                        }
            //                        var OK = db.OtherArchives.Where(a => a.licenceNo == vol1).Where(a => a.classTypeID == 1).ToList();
            //                        for (int i = 0; i < OK.Count(); i++)
            //                        {
            //                            OK[i].isImageExist = "有";
            //                            db.Entry(OK[i]).State = EntityState.Modified;
            //                            db.SaveChanges();
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                if (int.Parse(year) < 2007)
            //                {
            //                    var OK = db.OtherArchives.Where(a => a.licenceNo == volNo).Where(a => a.classTypeID == 1).ToList();
            //                    for (int i = 0; i < OK.Count(); i++)
            //                    {
            //                        OK[i].isImageExist = "有";
            //                        db.Entry(OK[i]).State = EntityState.Modified;
            //                        db.SaveChanges();
            //                    }
            //                }
            //                else
            //                {
            //                    string volNo1 = year + "-" + volNo;
            //                    var OK = db.OtherArchives.Where(a => a.licenceNo == volNo1).Where(a => a.classTypeID == 1).ToList();
            //                    for (int i = 0; i < OK.Count(); i++)
            //                    {
            //                        OK[i].isImageExist = "有";
            //                        db.Entry(OK[i]).State = EntityState.Modified;
            //                        db.SaveChanges();
            //                    }
            //                }
            //            }
            //        }
            //        return Content("<script>alert('匹配完成！');window.location.href='SaoMiaoJianPiPei'</script>");
            //    }
            //}



            if (action == "删除已匹配东区执照档案")
            {
                string s = Request.Form["DQNo"];
                if (s != "")
                {
                    var del = db.OtherArchives.Where(a => a.licenceNo == s).Where(a => a.classTypeID == 1).ToList();
                    for (int i = 0; i < del.Count(); i++)
                    {
                        del[i].isImageExist = "无";
                        db.Entry(del[i]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

            }
            if (action == "开始匹配东区执照档案")
            {
                string DQNo = Request.Form["DQNo"];
                if (DQNo == "")
                {
                    return Content("<script>alert('请输入匹配东区号！');window.location.href='SaoMiaoJianPiPei'</script>"); ;
                }
                if (DQNo != "")
                {
                    DirectoryInfo theLicenceDirOfYear = new DirectoryInfo("H:\\OtherArchives\\License\\" + DQNo);
                    if (theLicenceDirOfYear.Exists)
                    {
                        DirectoryInfo[] subdirectories = theLicenceDirOfYear.GetDirectories();
                        if (subdirectories.Length > 0)
                        {
                            foreach (DirectoryInfo volNo in subdirectories)
                            {
                                string volNo1 = "-" + volNo.Name;
                                var OK1 = from ad in db.OtherArchives
                                          where ad.classTypeID == 1
                                          where ad.licenceNo.Contains(DQNo)
                                          where ad.licenceNo.Contains(volNo1)
                                          orderby ad.ID descending
                                          select ad;
                                var OK = OK1.ToList();
                                for (int i = 0; i < OK.Count(); i++)
                                {
                                    OK[i].isImageExist = "有";
                                    db.Entry(OK[i]).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                //if (volNo.Name.Length > 3)
                                //{
                                //    var OK1 = from ad in db.OtherArchives
                                //              where ad.classTypeID == 1
                                //              where ad.licenceNo.Contains(volNo.Name)
                                //              orderby ad.ID descending
                                //              select ad;
                                //    var OK = OK1.ToList();
                                //    for (int i = 0; i < OK.Count(); i++)
                                //    {
                                //        OK[i].isImageExist = "有";
                                //        db.Entry(OK[i]).State = EntityState.Modified;
                                //        db.SaveChanges();
                                //    }
                                //}
                                //else
                                //{
                                //    string volNo1 = DQNo + "-00" + volNo.Name;
                                //    var OK1 = from ad in db.OtherArchives
                                //              where ad.classTypeID == 1
                                //              where ad.licenceNo.Contains(volNo1)
                                //              orderby ad.ID descending
                                //              select ad;
                                //    var OK = OK1.ToList();
                                //    for (int i = 0; i < OK.Count(); i++)
                                //    {
                                //        OK[i].isImageExist = "有";
                                //        db.Entry(OK[i]).State = EntityState.Modified;
                                //        db.SaveChanges();
                                //    }
                                //}
                                //string strSql2 = "update OtherArchives set isImageExist='有'  where  classTypeID=1 and (LicenceNo like '" + AreaNo + "%-" + volNo + "') ";
                                //DbHelperSQL.ExecuteSql(strSql2);
                            }
                        }
                        else
                        {
                            var OK1 = from ad in db.OtherArchives
                                      where ad.classTypeID == 1
                                      where ad.licenceNo.Contains(DQNo)
                                      orderby ad.ID descending
                                      select ad;
                            var OK = OK1.ToList();
                            for (int i = 0; i < OK.Count(); i++)
                            {
                                OK[i].isImageExist = "有";
                                db.Entry(OK[i]).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            //string strSql2 = "update OtherArchives set isImageExist='有'  where  classTypeID=1 and (LicenceNo like '" + AreaNo + "%') ";
                            //DbHelperSQL.ExecuteSql(strSql2);
                        }

                        Response.Write("<script language=\'javascript\'>alert('匹配完成!');</script>");
                    }
                    else
                    {
                        Response.Write("<script language=\'javascript\'>alert('该目录不存在!');</script>");
                    }
                }
            }
            return View();
        }      
        
    }
}
