using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class UsersController : Controller
    {
        private DAPMEntities database = new DAPMEntities();
        // GET: Users
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Customer cust)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(cust.NameCus)) ModelState.AddModelError(string.Empty, "Tên đăng nhập không được để trống");
                if (string.IsNullOrEmpty(cust.PassCus)) ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
                if (string.IsNullOrEmpty(cust.EmailCus)) ModelState.AddModelError(string.Empty, "Email không được để trống");
                if (string.IsNullOrEmpty(cust.PhoneCus)) ModelState.AddModelError(string.Empty, "Điện thoại không được để trống");
                if (string.IsNullOrEmpty(cust.AddressCus)) ModelState.AddModelError(string.Empty, "Địa chỉ không được để trống"); // Thêm kiểm tra địa chỉ không được để trống
                                                                                                                                  // Kiểm tra xem có người nào đã đăng kí với tên đăng nhập này hay chưa
                var khachhang = database.Customers.FirstOrDefault(k => k.NameCus == cust.NameCus);
                if (khachhang != null) ModelState.AddModelError(string.Empty, "Đã có người đăng kí tên này");
                if (ModelState.IsValid)
                {
                    database.Customers.Add(cust);
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
        public ActionResult Login(Customer cust)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(cust.NameCus))
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập không được để trống");
                if (string.IsNullOrEmpty(cust.PassCus))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");

                if (ModelState.IsValid)
                {
                    // Tìm khách hàng có tên đăng nhập và mật khẩu hợp lệ trong cơ sở dữ liệu
                    var khachhang = database.Customers.FirstOrDefault(k => k.NameCus == cust.NameCus && k.PassCus == cust.PassCus);
                    if (khachhang != null)
                    {
                        // Lưu đối tượng khách hàng vào session
                        Session["TaiKhoan"] = khachhang;

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                    }
                }
            }
            return View("Login");
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            Session.Remove("TaiKhoan");

            return RedirectToAction("Index", "Home");
        }


        // GET: Users/Index
        public ActionResult Index()
        {
            if (Session["TaiKhoan"] != null)
            {
                var khachhang = Session["TaiKhoan"] as Customer;
                if (khachhang != null)
                {
                    return View(khachhang);
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy người dùng (ví dụ: ghi nhật ký vấn đề hoặc chuyển hướng)
                    return RedirectToAction("Login", "Users");
                }
            }
            else
            {
                // Nếu không có phiên đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Users");
            }
        }

        // GET: Users/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Users/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var user = database.Customers.FirstOrDefault(k => k.NameCus == userName);
                if (user != null)
                {
                    return RedirectToAction("Edit", new { id = user.IDCus });
                }
            }
            ViewBag.ThongBao = "Tên đăng nhập không đúng.";
            return View();
        }

        // GET: Users/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer user = database.Customers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCus,NameCus,PhoneCus,EmailCus,PassCus,AddressCus")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                var userInDb = database.Customers.Find(customer.IDCus);
                if (userInDb != null)
                {
                    userInDb.NameCus = customer.NameCus;
                    userInDb.PhoneCus = customer.PhoneCus;
                    userInDb.EmailCus = customer.EmailCus;
                    userInDb.AddressCus = customer.AddressCus;

                    // Cập nhật mật khẩu nếu người dùng nhập mật khẩu mới
                    if (!string.IsNullOrEmpty(customer.PassCus))
                    {
                        userInDb.PassCus = customer.PassCus;
                    }

                    database.Entry(userInDb).State = EntityState.Modified;
                    database.SaveChanges();

                    // Đăng xuất người dùng sau khi thông tin đã được cập nhật
                    Session.Remove("TaiKhoan");

                    // Chuyển hướng đến trang đăng nhập
                    return RedirectToAction("Login", "Users");
                }
            }
            return View(customer);
        }

    }
}
