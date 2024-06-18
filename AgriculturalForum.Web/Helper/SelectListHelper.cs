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
                var ListOfCategories = _dbContext.CategoryPosts.Where(p => p.IsActive).ToList();
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
                var ListOfCategories = _dbContext.CategoryProducts.Where(p => p.IsActive).ToList();
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

        public static List<SelectListItem> GetStatusSelectListItems()
        {
            List<SelectListItem> lsStatus = new List<SelectListItem>
            {
                new SelectListItem { Text = "Tất cả trạng thái", Value = "" },
                new SelectListItem { Text = "Hoạt động", Value = "true" },
                new SelectListItem { Text = "Khóa", Value = "false" }
            };
            return lsStatus;
        }
    }
}
