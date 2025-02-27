using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static WebApplication3.Controllers.ViewProductsControllerFacade.ViewProductsSubControllerContext;
using static WebApplication3.Controllers.ViewProductsControllerFacade.ViewProductsSubControllerLogger;
using WebApplication3.Models;
using WebApplication3.Controllers.ViewProductsControllerFacade;

namespace WebApplication3.Controllers.ViewProductsControllerFacade
{

    public class ViewProductsFacade
    {
        private ViewProductsSubControllerLogger vpscLogger;
        private ViewProductsSubControllerContext vpscContext;
        private ViewProductsSubControllerCache vpscCache;

        public ViewProductsFacade()
        {
            vpscLogger = new ViewProductsSubControllerLogger();
            vpscContext = new ViewProductsSubControllerContext();
            vpscCache = new ViewProductsSubControllerCache();
        }

        /// <summary>
        /// Ghi log tuyến đường của Controller
        /// </summary>
        public void PrintRoutes()
        {
            vpscLogger.PrintRoutes();
        }

        /// <summary>
        /// Kiểm tra xem cache có dữ liệu không
        /// </summary>
        public bool TryGetValue(string key, out List<Category> list)
        {
            return vpscCache.TryGetValue(key, out list);
        }

        /// <summary>
        /// Đặt dữ liệu vào cache
        /// </summary>
        public void SetCache(string key, List<Category> list, int durationInMinutes = 10)
        {
            vpscCache.SetCache(key, list, durationInMinutes);
        }

        /// <summary>
        /// Lấy tất cả bài viết từ database
        /// </summary>
        public IQueryable<Product> GetAllProducts()
        {
            return vpscContext.GetAllProducts();
        }

        /// <summary>
        /// Lấy bài viết theo Slug
        /// </summary>
        public Product GetProductById(int id)
        {
            return vpscContext.GetProductById(id);
        }

        /// <summary>
        /// Lấy danh sách danh mục cha không có danh mục cha
        /// </summary>
        public List<Category> GetListCategoryParentIsNull()
        {
            return vpscContext.GetListCategoryParentIsNull();
        }
    }
}