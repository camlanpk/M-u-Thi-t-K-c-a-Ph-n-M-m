using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using WebApplication3.Models;

namespace WebApplication3.Controllers.ViewProductsControllerFacade
{
    public class ViewProductsSubControllerContext
    {
            private DAPMEntities db = new DAPMEntities();

            public IQueryable<Product> GetAllProducts()
            {
                return db.Products
                    .Include(p => p.Category1) // Load Category cho Product
                    .Include(p => p.OrderDetails); // Load OrderDetails của Product
            }

            public Product GetProductById(int productId)
            {
                return db.Products
                    .Where(p => p.ProductID == productId)
                    .Include(p => p.Category1)
                    .Include(p => p.OrderDetails)
                    .FirstOrDefault(); // EF 6 không có FirstOrDefaultAsync()
            }


        public List<Category> GetListCategoryParentIsNull()
        {
            return db.Categories
                .AsEnumerable()  // Chuyển sang LINQ to Objects trước khi lọc
                .Where(c => c.ParentId == null) // Lọc danh mục cha
                .ToList();
        }


    }
}