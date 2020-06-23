using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace urban_archive.Models
{
    public class AspNetUsersController : Controller
    {
        private UrbanUsersEntities db = new UrbanUsersEntities();

        // GET: AspNetUsers
        public ActionResult Index(AspNetUsers model)
        {
            var UserID = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(UserID);

            if (user.RoleId == 4)
            {

                return View(db.AspNetUsers.ToList());
            }

            else
                ViewBag.RoleName = user.RoleName;
            ViewBag.Password = user.Password;
            ViewBag.DepartmentName = user.DepartmentName;
            ViewBag.Email = user.Email;
            ViewBag.UserName = user.UserName;
            return View("~/Views/AspNetUsers/Users.cshtml");
        }


        // GET: AspNetUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        // GET: AspNetUsers/Create
        public ActionResult Create()
        {
            List<SelectListItem> list = new List<SelectListItem>
            {
                    new SelectListItem { Text = "请选择科室", Value = "0"},
                    new SelectListItem { Text = "管理科", Value = "1"},
                    new SelectListItem { Text = "业务科", Value = "2" },
                    new SelectListItem { Text = "声像科", Value = "3" },
                    new SelectListItem { Text = "管线科", Value = "4"},
                    new SelectListItem { Text = "办公室", Value = "5" },
                    new SelectListItem { Text = "编研科", Value = "6" },
                    new SelectListItem { Text = "档案整理室", Value = "7"},
                    new SelectListItem { Text = "复印室", Value = "8" },
                    new SelectListItem { Text = "财务科", Value = "9" }
              };
            ViewBag.DepartmentID = new SelectList(list, "Value", "Text");
            List<SelectListItem> list1 = new List<SelectListItem>
            {
                    new SelectListItem { Text = "请选择职位", Value = "0" },
                    new SelectListItem { Text = "高级用户", Value = "1" },
                    new SelectListItem { Text = "科长", Value = "2" },
                    new SelectListItem { Text = "科员", Value = "3" },
                    new SelectListItem { Text = "管理员", Value = "4" },
                    new SelectListItem { Text = "借阅用户", Value = "5" },
                    new SelectListItem { Text = "馆长", Value = "6" },
                    new SelectListItem { Text = "外聘人员", Value = "7" }


              };
            ViewBag.RoleID = new SelectList(list1, "Value", "Text");
            return View();
        }

        // POST: AspNetUsers/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterViewModel model, [Bind(Include = "Password,DepartmentId,RoleId,Email,UserName")] AspNetUsers aspNetUsers, int? DepartmentID, int? RoleID)
        {
            List<SelectListItem> list = new List<SelectListItem>
            {
                    new SelectListItem { Text = "请选择科室", Value = "0"},
                    new SelectListItem { Text = "管理科", Value = "1"},
                    new SelectListItem { Text = "业务科", Value = "2" },
                    new SelectListItem { Text = "声像科", Value = "3" },
                    new SelectListItem { Text = "管线科", Value = "4"},
                    new SelectListItem { Text = "办公室", Value = "5" },
                    new SelectListItem { Text = "编研科", Value = "6" },
                    new SelectListItem { Text = "档案整理室", Value = "7"},
                    new SelectListItem { Text = "复印室", Value = "8" },
                    new SelectListItem { Text = "财务科", Value = "9" }
                };
            ViewBag.DepartmentID = new SelectList(list, "Value", "Text");
            int t = DepartmentID.GetValueOrDefault();
            string text = list.Find(s => s.Value == t.ToString()).Text;
            List<SelectListItem> list1 = new List<SelectListItem>
            {
                    new SelectListItem { Text = "请选择职位", Value = "0" },
                    new SelectListItem { Text = "高级用户", Value = "1" },
                    new SelectListItem { Text = "科长", Value = "2" },
                    new SelectListItem { Text = "科员", Value = "3" },
                    new SelectListItem { Text = "管理员", Value = "4" },
                    new SelectListItem { Text = "借阅用户", Value = "5" },
                    new SelectListItem { Text = "馆长", Value = "6" },
                    new SelectListItem { Text = "外聘人员", Value = "7" }

              };
            ViewBag.RoleID = new SelectList(list1, "Value", "Text");
            int t1 = RoleID.GetValueOrDefault();
            string text1 = list1.Find(s => s.Value == t1.ToString()).Text;

            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspNetUsers);
                db.Entry(aspNetUsers).State = EntityState.Modified;
                model.RoleID = t1;
                model.DepartmentID = t;
                model.Role = text1;
                model.DepartmentName = text;
                db.SaveChanges();
                return View("Index");
            }

            return View(aspNetUsers);
        }

        // GET: AspNetUsers/Edit/5
        // GET: AspNetUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            int t = aspNetUsers.DepartmentId;
            int n = aspNetUsers.RoleId;
            //ViewBag.RoleID = new SelectList(db.AspNetUsers, "RoleId", "RoleName", n);
            //ViewBag.DepartmentID = new SelectList(db.AspNetUsers, "DepartmentId", "DepartmentName", t);
            List<SelectListItem> list = new List<SelectListItem>
            {
                    new SelectListItem { Text = "请选择科室", Value = "0"},
                    new SelectListItem { Text = "管理科", Value = "1"},
                    new SelectListItem { Text = "业务科", Value = "2" },
                    new SelectListItem { Text = "声像科", Value = "3" },
                    new SelectListItem { Text = "管线科", Value = "4"},
                    new SelectListItem { Text = "办公室", Value = "5" },
                    new SelectListItem { Text = "编研科", Value = "6" },
                    new SelectListItem { Text = "档案整理室", Value = "7"},
                    new SelectListItem { Text = "复印室", Value = "8" },
                    new SelectListItem { Text = "财务科", Value = "9" }
              };
            ViewBag.DepartmentID = new SelectList(list, "Value", "Text", t);
            List<SelectListItem> list1 = new List<SelectListItem>
            {
                    new SelectListItem { Text = "请选择职位", Value = "0" },
                    new SelectListItem { Text = "高级用户", Value = "1" },
                    new SelectListItem { Text = "科长", Value = "2" },
                    new SelectListItem { Text = "科员", Value = "3" },
                    new SelectListItem { Text = "管理员", Value = "4" },
                    new SelectListItem { Text = "借阅用户", Value = "5" },
                    new SelectListItem { Text = "馆长", Value = "6" },
                    new SelectListItem { Text = "外聘人员", Value = "7" }

              };
            ViewBag.RoleID = new SelectList(list1, "Value", "Text", n);
            return View(aspNetUsers);
        }

        // POST: AspNetUsers/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Password,RoleName,DepartmentName,DepartmentId,RoleId,Name,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AspNetUsers aspNetUsers, int? DepartmentID, int? RoleID)
        {
            int t = aspNetUsers.DepartmentId;
            int n = aspNetUsers.RoleId;
            //ViewBag.RoleID = new SelectList(db.AspNetUsers, "RoleId", "RoleName", n);
            //ViewBag.DepartmentID = new SelectList(db.AspNetUsers, "DepartmentId", "DepartmentName", t);
            List<SelectListItem> list = new List<SelectListItem>
            {
                    new SelectListItem { Text = "管理科", Value = "1"},
                    new SelectListItem { Text = "业务科", Value = "2" },
                    new SelectListItem { Text = "声像科", Value = "3" },
                    new SelectListItem { Text = "管线科", Value = "4"},
                    new SelectListItem { Text = "办公室", Value = "5" },
                    new SelectListItem { Text = "编研科", Value = "6" },
                    new SelectListItem { Text = "档案整理室", Value = "7"},
                    new SelectListItem { Text = "复印室", Value = "8" },
                    new SelectListItem { Text = "财务科", Value = "9" }
              };
            ViewBag.DepartmentID = new SelectList(list, "Value", "Text", t);
            List<SelectListItem> list1 = new List<SelectListItem>
            {
                    new SelectListItem { Text = "高级用户", Value = "1" },
                    new SelectListItem { Text = "科长", Value = "2" },
                    new SelectListItem { Text = "科员", Value = "3" },
                    new SelectListItem { Text = "管理员", Value = "4" },
                    new SelectListItem { Text = "借阅用户", Value = "5" },
                    new SelectListItem { Text = "馆长", Value = "6" },
                    new SelectListItem { Text = "外聘人员", Value = "7" }

              };
            ViewBag.RoleID = new SelectList(list1, "Value", "Text", n);

            if (ModelState.IsValid)
            {
                db.Entry(aspNetUsers).State = EntityState.Modified;
                int t1 = DepartmentID.GetValueOrDefault();
                int n1 = RoleID.GetValueOrDefault();
                string Duser = list.ToList().Find(s => s.Value == t1.ToString()).Text;
                string Ruser = list1.ToList().Find(s => s.Value == n1.ToString()).Text;
                aspNetUsers.DepartmentId = t1;
                aspNetUsers.RoleId = n1;
                aspNetUsers.DepartmentName = Duser;
                aspNetUsers.RoleName = Ruser;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUsers);
        }


        //// GET: AspNetUsers/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
        //    if (aspNetUsers == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(aspNetUsers);
        //}

        // POST: AspNetUsers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUsers);
            db.SaveChanges();
            return Content("<script>alert('已成功删除！');window.location.href='/AspNetUsers/Index';</script >");

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

