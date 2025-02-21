using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WebApplication3.Models;
using System.Net;
using PagedList;
using PagedList.Mvc;

namespace WebApplication3.Controllers
{
    public class CustomerProductsController : Controller
    {
        private DAPMEntities db = new DAPMEntities();
        // GET: CustomerProducts
        public ActionResult Index(string category, int? page, double min = double.MinValue, double max = double.MinValue, string Searching = "")
        {
            int pageSize = 9;
            int pageNum = (page ?? 1);

            var productList = db.Products.AsQueryable(); // Lấy danh sách sản phẩm từ DbContext

            if (!string.IsNullOrEmpty(Searching))
            {
                productList = productList.Where(x => x.NamePro.ToUpper().Contains(Searching.ToUpper()));
            }

            if (category != null)
            {
                productList = productList.Where(p => p.Category == category);
            }

            // Sắp xếp sản phẩm theo tên hoặc điều kiện tùy chọn khác nếu cần
            productList = productList.OrderBy(x => x.NamePro);

            // Trả về trang sản phẩm sử dụng PagedList
            return View(productList.ToPagedList(pageNum, pageSize));
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new
                HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
       
        public ActionResult GetProductsByCategory()
        {
            var categories = db.Categories.ToList();
            return PartialView("CategoriesPartialView", categories);
        }
        public ActionResult GetProductsByCateId(int id)
        {
            var products = db.Products.Where(p => p.Category1.Id == id).ToList();
            return View("Index", products);
        }
    }
}