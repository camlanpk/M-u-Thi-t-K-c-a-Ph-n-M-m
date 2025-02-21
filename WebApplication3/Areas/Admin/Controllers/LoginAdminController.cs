using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Areas.Admin.Controllers
{
    public class LoginAdminController : Controller
    {
        // GET: Admin/LoginAdmin
        private DAPMEntities database = new DAPMEntities();
        // GET: ADmin
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(AdminUser ad)
        {
            if (ModelState.IsValid)
            {
               
                if (string.IsNullOrEmpty(ad.NameUser)) ModelState.AddModelError(string.Empty, "Name không được để trống");
                if (string.IsNullOrEmpty(ad.RoleUser)) ModelState.AddModelError(string.Empty, "Role không được để trống");
                if (string.IsNullOrEmpty(ad.PasswordUser)) ModelState.AddModelError(string.Empty, "Password không được để trống");
                //Kiểm tra xem có người nào đã đăng kí với tên đăng nhập này hay chưa
                var admin = database.AdminUsers.FirstOrDefault(k => k.NameUser == ad.NameUser);
                if (admin != null) ModelState.AddModelError(string.Empty, "Đã có người đăng kí tên này");
                if (ModelState.IsValid)
                {
                    database.AdminUsers.Add(ad);
                    database.SaveChanges();
                }
                else
                {
                    return View();
                }
            }
            return RedirectToAction("Login");
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(AdminUser ad)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(ad.NameUser)) ModelState.AddModelError(string.Empty, "Name không được để trống");
              
                if (string.IsNullOrEmpty(ad.PasswordUser)) ModelState.AddModelError(string.Empty, "Password không được để trống");
                if (ModelState.IsValid)
                {
                    // tim khach hang co ten dang nhap va password hop le trong csdl
                    var admin = database.AdminUsers.FirstOrDefault(k => k.NameUser == ad.NameUser && k.PasswordUser == ad.PasswordUser);
                    if (admin != null)
                    {
                        ViewBag.ThongBao = " chuc mung ban dang nhap thanh cong";
                        //luu vao session
                        Session["TaiKhoan"] = admin;
                        return RedirectToAction("Index", "MainAdmin", "Admin");
                    }
                    else
                    {
                        ViewBag.ThongBao = " ten dang nhap hoac mat khau khong dung";
                    }
                }
            }
            return View("Login");
        }
        [HttpGet]
        public ActionResult LogOut()
        {
            Session.Remove("TaiKhoan");

            return RedirectToAction("Index", "MainAdmin","Admin");
        }
    }
}