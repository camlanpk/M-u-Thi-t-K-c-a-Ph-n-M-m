using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication3.Models;

namespace WebApplication3.Data
{
    public sealed class CategorySingleton
    {
        private DAPMEntities _database;
        public static CategorySingleton Instance { get; } = new CategorySingleton();
        public List<Category> listCategory { get; private set; }

        private CategorySingleton()
        {
            listCategory = new List<Category>();
        }

        public void Init(DAPMEntities database)
        {
            _database = database;
            LoadCategories();

        }
        public void AddCategory(Category category)
        {
            _database.Categories.Add(category);
            _database.SaveChanges();
            LoadCategories();
        }
        private void LoadCategories()
        {
            listCategory = _database.Categories.ToList();
        }
        public void InvalidateCache()
        {
            listCategory = null;
        }

        public void UpdateCategory(Category category)
        {
            var existingCategory = _database.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (existingCategory != null)
            {
                _database.Entry(existingCategory).CurrentValues.SetValues(category);
                _database.SaveChanges();
                LoadCategories();
            }
        }

        public void DeleteCategory(int id)
        {
            var category = _database.Categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _database.Categories.Remove(category);
                _database.SaveChanges();
                InvalidateCache();
            }
        }
    }
}