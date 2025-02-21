using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public abstract class ControllerTemplateMethod : Controller 
    {
        protected virtual bool CheckSession()
        {
            if (Session == null || Session["TaiKhoan"] == null)
            {
                System.Diagnostics.Debug.WriteLine("🔴 Session không hợp lệ hoặc đã hết hạn!");
                return false;
            }

            System.Diagnostics.Debug.WriteLine("🟢 Session hợp lệ: " + ((AdminUser)Session["TaiKhoan"]).NameUser);
            return true;
        }

        // Hàm này sẽ gọi trong mỗi action
        protected ActionResult ExecuteWithSessionCheck(Func<ActionResult> action)
        {
            if (!CheckSession())
            {
                return RedirectToAction("Login", "LoginAdmin", new { area = "Admin" });
            }
            return action();
        }

        protected abstract void PrintRoutes();
        protected abstract void PrintDIs();

        protected void LogDebug(string message)
        {
            Debug.WriteLine(message); // Thay thế _logger.LogDebug()
        }

        protected void PrintControllerInfo()
        {
            PrintRoutes();
            PrintDIs();
        }
    }
}