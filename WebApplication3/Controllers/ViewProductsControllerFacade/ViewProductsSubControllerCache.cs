using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using WebApplication3.Models;

namespace WebApplication3.Controllers.ViewProductsControllerFacade
{
    public class ViewProductsSubControllerCache
    {
        private Cache _cache;

        public ViewProductsSubControllerCache()
        {
            _cache = HttpRuntime.Cache; // Sử dụng Cache của ASP.NET MVC 5
        }

        /// <summary>
        /// Kiểm tra xem cache có dữ liệu không
        /// </summary>
        public bool TryGetValue(string key, out List<Category> list)
        {
            var cacheData = _cache[key];
            if (cacheData != null)
            {
                list = (List<Category>)cacheData;
                return true;
            }
            list = null;
            return false;
        }

        /// <summary>
        /// Lưu dữ liệu vào cache với thời gian hết hạn
        /// </summary>
        public void SetCache(string key, List<Category> list, int durationInMinutes = 300)
        {
            if (list != null)
            {
                _cache.Insert(
                    key,
                    list,
                    null,
                    DateTime.Now.AddMinutes(durationInMinutes),
                    Cache.NoSlidingExpiration);
            }
        }
    }
}