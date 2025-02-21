using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private DAPMEntities db = new DAPMEntities();

        public ActionResult Index()
        {
            var products = db.Products.AsQueryable();
            var comboProducts = products.Where(p => p.Category1.NameCate == "Combo Gà Rán").Take(4).ToList();
            var mainDishes = products.Where(p => p.Category1.NameCate == "Món chính nước").Take(4).ToList();
            var mainDrinks = products.Where(p => p.Category1.NameCate == "Combo Nước").Take(4).ToList();

            ViewBag.ComboProducts = comboProducts;
            ViewBag.MainDishes = mainDishes;
            ViewBag.MainDrink = mainDrinks;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}