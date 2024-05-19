using AgriculturalForum.Web.Extensions;
using AgriculturalForum.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AgriculturalForum.Web
{
    public static class SelectListHelper
    {
        public static List<SelectListItem> Provinces()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (var _dbContext = new KltnDbContext())
            {
                var ListOfProvinces = _dbContext.Provinces.ToList();
                list.Add(new SelectListItem()
                {
                    Value = "",
                    Text = "--Chọn tỉnh thành --"
                });
                foreach (var item in ListOfProvinces)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = item.ProvinceName,
                        Text = item.ProvinceName
                    });
                }
            }
            return list;
        }

        public static List<SelectListItem> CategoryPosts()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (var _dbContext = new KltnDbContext())
            {
                var ListOfCategories = _dbContext.CategoryPosts.ToList();
                foreach (var item in ListOfCategories)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = item.Id.ToString(),
                        Text = item.Title
                    });
                }
            }
            return list;
        }

        public static List<SelectListItem> CategoryProducts()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (var _dbContext = new KltnDbContext())
            {
                var ListOfCategories = _dbContext.CategoryProducts.ToList();
                foreach (var item in ListOfCategories)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = item.Id.ToString(),
                        Text = item.Name
                    });
                }
            }
            return list;
        }
    }
}
