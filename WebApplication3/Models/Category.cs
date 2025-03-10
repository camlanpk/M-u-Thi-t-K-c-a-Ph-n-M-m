﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication3.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category()
        {
            this.Products = new HashSet<Product>();
            this.CategoryChildren = new HashSet<Category>(); // Khởi tạo để tránh lỗi null
        }
    
        public int Id { get; set; }
        public string IDCate { get; set; }
        public string NameCate { get; set; }

        public int? ParentId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<Category> CategoryChildren { get; set; }

        [Display(Name = "Danh mục cha")]
        [ForeignKey("ParentId")]
        public virtual Category ParentCategory { get; set; }


        public List<Category> ListParents()
        {
            List<Category> li = new List<Category>();
            var parent = this.ParentCategory;
            while (parent != null)
            {
                li.Add(parent);
                parent = parent.ParentCategory;
            }
            li.Reverse();
            return li;
        }

        public static Category Find(ICollection<Category> list, int categoryId)
        {
            foreach (var c in list)
            {
                if (c.Id == categoryId)
                    return c;
                if (c.CategoryChildren != null)
                {
                    var cInChild = Find(c.CategoryChildren, categoryId);
                    if (cInChild != null)
                        return cInChild;
                }
            }
            return null;
        }

        public List<int> ChildCategoryIDs(ICollection<Category> childCategories = null, List<int> list = null)
        {
            if (list == null)
                list = new List<int>();

            if (childCategories == null)
                childCategories = CategoryChildren;

            if (childCategories == null)
                return list;

            foreach (var item in childCategories)
            {
                list.Add(item.Id);
                ChildCategoryIDs(item.CategoryChildren, list);
            }
            return list;
        }
    }
}
