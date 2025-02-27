using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using WebApplication3.Controllers.ViewProductsControllerFacade;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class ViewProductsController : Controller
    {
        private readonly ViewProductsFacade facade;
        private const int ITEMS_PER_PAGE = 4;
        private readonly Cache cache = HttpRuntime.Cache;

        public ViewProductsController()
        {
            facade = new ViewProductsFacade();
        }
        /// <summary>
        /// Lấy danh sách Categories, sử dụng cache
        /// </summary>
        private List<Category> GetCategories()
        {
            string keycacheCategories = "_listallcategories";
            if (!facade.TryGetValue(keycacheCategories, out List<Category> categories) || categories == null)
            {
                categories = facade.GetListCategoryParentIsNull() ?? new List<Category>();
                facade.SetCache(keycacheCategories, categories);
            }
            return categories;
        }

        /// <summary>
        /// Tìm Category theo Slug bằng duyệt đệ quy
        /// </summary>
        private Category FindCategoryById(List<Category> categories, int id)
        {
            if (categories == null) return null;

            foreach (var c in categories)
            {
                if (c.Id == id) return c;

                var childCategories = c.Products?.Select(p => p.Category1).Where(cat => cat != null).ToList();
                var childCategory = FindCategoryById(childCategories, id);

                if (childCategory != null)
                    return childCategory;
            }
            return null;
        }


        /// <summary>
        /// Hiển thị danh sách sản phẩm theo Category (trang danh mục)
        /// </summary>
        public ActionResult Index(int? page, int? categoryId)
        {
            int pageNumber = page ?? 1;
            var categories = GetCategories();
            Category category = null;

            if (categoryId.HasValue)
            {
                category = FindCategoryById(categories, categoryId.Value);
                if (category == null)
                {
                    return HttpNotFound("Không tìm thấy danh mục");
                }
            }

            ViewBag.categories = categories;
            ViewBag.categoryId = categoryId;
            ViewBag.CurrentCategory = category;

            // Lấy danh sách sản phẩm
            var products = facade.GetAllProducts();

            if (category != null)
            {
                products = products.Where(p => p.Category1 != null && p.Category1.Id == category.Id);
            }

            int totalItems = products.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / ITEMS_PER_PAGE);
            totalPages = Math.Max(totalPages, 1);
            if (pageNumber > totalPages) return RedirectToAction("Index", new { categoryId, page = totalPages });

            // Phân trang
            products = products
                .OrderByDescending(p => p.ProductID)
                .Skip(ITEMS_PER_PAGE * (pageNumber - 1))
                .Take(ITEMS_PER_PAGE);

            ViewBag.pageNumber = pageNumber;
            ViewBag.totalPages = totalPages;

            return View(products.ToList());
        }

        /// <summary>
        /// Hiển thị chi tiết sản phẩm
        /// </summary>
        public ActionResult DisplayProduct(int? productId)
        {
            if (!productId.HasValue) return HttpNotFound("Không tìm thấy sản phẩm");

            var product = facade.GetProductById(productId.Value);
            if (product == null) return HttpNotFound("Không tìm thấy sản phẩm");

            ViewBag.categories = GetCategories();
            return View(product);
        }
    }
}
