using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace urban_archive.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据。若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
       

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
        public async Task<ClaimsIdentity> GenerateRoleIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var RoleIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return RoleIdentity;
        }
        public async Task<ClaimsIdentity> GenerateDepartmentIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var DepartmentIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return DepartmentIdentity;
        }

        public string Password { get; internal set; }
        public string RoleName { get; internal set; }
        public string DepartmentName { get; internal set; }
        public int  DepartmentId { get; internal set; }
        public int  RoleId { get; internal set; }
        public string  Name { get; internal set; }

        //public enum RoleID//角色枚举
        //{ 管理员 = 1, 馆长 = 2, 高级用户 = 3 ,科长 = 4, 职员 = 5 }
        //public enum DepartmentID//科室枚举
        //{ 管理信息科 = 1, 业务科 = 2, 声像科 = 3, 管线科 = 4, 办公室 = 5,编研科=6,档案整理室=7,复印室=8,财务室=9 }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}