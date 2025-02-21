using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication3.Controllers;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Areas.Admin.Controllers
{
    public class CategoryController : ControllerTemplateMethod
    {
        DAPMEntities database = new DAPMEntities();
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(CategoryController));
        // GET: Admin/Category
        public CategoryController()
        {
            database = new DAPMEntities();
            CategorySingleton.Instance.Init(database);
        }
        public ActionResult Index()
        {
            return ExecuteWithSessionCheck(() =>
            {
                PrintControllerInfo();
                var categories = CategorySingleton.Instance.listCategory;
                return View(categories);
            });
        }

        [HttpGet]
        public ActionResult Create()
        {
            return ExecuteWithSessionCheck(() => View());
        }
        [HttpPost]
        public ActionResult Create(Category category)
        {
            return ExecuteWithSessionCheck(() =>
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        CategorySingleton.Instance.AddCategory(category);
                        return RedirectToAction("Index");
                    }
                    return View(category);
                }
                catch
                {
                    return Content("Lỗi khi tạo danh mục.");
                }
            });

            //try
            //{
            //    //database.Categories.Add(category);
            //    //database.SaveChanges();
            //    CategorySingleton.Instance.AddCategory(category);
            //    return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return Content("LOI TAO MOI CATEGORY");
            //}
        }
        public ActionResult Details(int id)
        {
            return ExecuteWithSessionCheck(() =>
            {
                var category = database.Categories.FirstOrDefault(c => c.Id == id);
                if (category == null) return HttpNotFound();
                return View(category);
            });
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var category = database.Categories.Where(c => c.Id == id).FirstOrDefault();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Category category)
        {

            CategorySingleton.Instance.UpdateCategory(category);
            return RedirectToAction("Index");

        }
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var category = database.Categories.Where(c => c.Id == id).FirstOrDefault();
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CategorySingleton.Instance.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return Content("Không thể xóa do liên quan đến các bảng khác");
            }
        }
        protected override void PrintRoutes()
        {
            LogDebug($@"{GetType().Name}
                Routes:
                GET: Admin/Category
                GET: Admin/Category/Details/:id
                GET: Admin/Category/Create
                POST: Admin/Category/Create
                GET: Admin/Category/Edit/5
                POST: Admin/Category/Edit/:id
                GET: Admin/Category/Delete/:id
                POST: Admin/Category/Delete/:id
                ");
        }

        protected override void PrintDIs()
        {
            LogDebug($@"
                Dependencies:
                DAPMEntities _context
                log4net.ILog _logger
                ");
        }
    }
}
