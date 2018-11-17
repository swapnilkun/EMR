using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ExcellentMarketResearch.Areas.Admin.Models.DAL
{
    public class CategoryRepository : ICategoryRepository
    {
      //  ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();
        public ExcellentMarketResearchEntities db;
        public CategoryRepository()
        {
            db = new ExcellentMarketResearchEntities();
        }
        public void InsertCategory(CategoryVM catvm)
        {
            CategoryMaster cat = new CategoryMaster();

            if (catvm.ParentCategoryId == null)
            {
                catvm.ParentCategoryId = 0;
            }
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //var c=serializer.Serialize(catvm);
            //CategoryMaster cat =serializer.Deserialize<CategoryMaster>(c);
            cat.ParentCategoryId = catvm.ParentCategoryId;
            cat.CategoryName = catvm.CategoryName;
            cat.CategoryUrl = catvm.CategoryURL;
            // cat.CategoryIcon = catvm.CategoryIcon;
            cat.ShortDescription = catvm.ShortDescription;
            cat.MetaTitle = catvm.MetaTitle;
            cat.Keywords = catvm.Keywords;
            cat.LongDescription = catvm.LongDescription;
            cat.MetaDescription = catvm.MetaDescription;
            cat.IsActive = catvm.IsActive;
            cat.CreatedBy = catvm.CreatedBy;
            cat.CreatedDate = catvm.CreatedDate;
            try
            {
                db.CategoryMasters.Add(cat);
                db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            //var categoryUrl = cat.CategoryUrl + "-" + cat.CategoryId;
            //cat.CategoryUrl = categoryUrl;
            //db.Entry(cat).State = EntityState.Modified;
            //db.SaveChanges();
        }


        public CategoryVM EditCategory(int catid)
        {
            var catdata = (from l in db.CategoryMasters
                           where l.CategoryId == catid
                           select new CategoryVM
                           {
                               CategoryId = l.CategoryId,
                               CategoryName = l.CategoryName,
                               CategoryURL = l.CategoryUrl,
                               MetaTitle = l.MetaTitle,
                               MetaDescription = l.MetaDescription,
                               ShortDescription = l.ShortDescription,
                               Keywords = l.Keywords,
                               ParentCategoryId = l.ParentCategoryId,
                               CreatedBy = l.CreatedBy,
                               CreatedDate = l.CreatedDate,
                               IsActive = l.IsActive,
                              // CategoryIcon = l.CategoryIcon
                           }).FirstOrDefault();
            return catdata;
        }

        public bool EditCategoryPost(CategoryVM cat)
        {
            cat.ModifiedBy = 1;
            cat.ModifiedDate = DateTime.Now;

            if (cat.ParentCategoryId == null)
            {
                cat.ParentCategoryId = 0;
            }
            bool flag = false;
            if (cat.CategoryURL == null)
            {
                var caturl = ExcellentMarketResearch.Areas.Admin.Models.Common.GenerateSlug(cat.CategoryName);
                // cat.CategoryURL = caturl + "-" + cat.CategoryId;
                cat.CategoryURL = caturl;
                if (IsSameCategoryDetails(cat) || !IsCategoryNameExist(cat))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var s = serializer.Serialize(cat);
                    CategoryMaster catm = serializer.Deserialize<CategoryMaster>(s);
                    db.Entry(catm).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    flag = true;
                }
            }
            else
            {
                if (IsSameCategoryDetails(cat) || !IsCategoryNameExist(cat))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var s = serializer.Serialize(cat);
                    CategoryMaster catm = serializer.Deserialize<CategoryMaster>(s);
                    db.Entry(catm).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    flag = true;
                }
            }
            return flag;
        }
        public List<CategoryMaster> GetParent()
        {
            var parentcat = db.CategoryMasters.Where(x => x.ParentCategoryId == 0).OrderBy(x => x.CategoryName).ToList();
            return parentcat;
        }
        //public List<CategoryMaster> GetChild(int id)
        //{
        //    var childcat = db.CategoryMasters.Where(x => x.ParentCategoryId == id).Select(x => new CategoryMaster
        //    {
        //        CategoryName = x.CategoryName,
        //        CategoryId = x.CategoryId
        //    }).ToList();

        //    return childcat;
        //}
        public int[] DDLGetparents(int catid)
        {
            List<int> arr = new List<int>();
            arr.Add(catid);
            int? parent = catid;
            while (parent != 0)
            {
                parent = db.CategoryMasters.Where(x => x.CategoryId == parent).Select(x => x.ParentCategoryId).FirstOrDefault();
                if (parent > 0)
                    arr.Add((int)parent);
            }
            return arr.ToArray();
        }

        public List<SelectListItem> Getparentcat(int catid)
        {
            var Categories = DDLGetparents(catid).OrderBy(x => x).ToArray();

            var ParentCat = new List<SelectListItem>();

            if (Categories.Length == 1)
            {
                ParentCat = new List<SelectListItem>(from l in GetParent()
                                                     select new SelectListItem
                                                     {
                                                         Text = l.CategoryName,
                                                         Value = l.CategoryId.ToString()
                                                     });
            }
            else
            {
                ParentCat = db.CategoryMasters.Where(x => x.ParentCategoryId == 0).Select(y => new
                {
                    Text = y.CategoryName,
                    Value = y.CategoryId
                }).ToList().Select(z => new SelectListItem
                {
                    Text = z.Text,
                    Value = z.Value.ToString(),
                    Selected = z.Value == Categories[0]
                }).ToList();
            }
            return ParentCat;
        }

        public List<SelectListItem> Getchildcat(int catid)
        {
            var Categories = DDLGetparents(catid).OrderBy(x => x).ToArray();
            List<SelectListItem> ChildCat = new List<SelectListItem>();
            if (Categories.Length > 1)
            {
                var condition = Categories[0];
                ChildCat = db.CategoryMasters.Where(x => x.ParentCategoryId == condition).Select(y => new
                {
                    Text = y.CategoryName,
                    Value = y.CategoryId
                }).ToList().Select(z => new SelectListItem
                {
                    Text = z.Text,
                    Value = z.Value.ToString(),
                    Selected = z.Value == Categories[1]
                }).ToList();
            }
            else
            {
                ChildCat = new List<SelectListItem> { new SelectListItem { Text = "-select Category-", Value = "" } };
            }
            return ChildCat;
        }

        public bool IsCategoryNameExist(CategoryVM cat)
        {
            ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

            bool flag = false;

            //same name of child and parent cat,to check that find out the catid of same catname
            var Catid = db.CategoryMasters.Where(x => x.CategoryName == cat.CategoryName).Select(x => x.CategoryId).FirstOrDefault();

            //Same name of Category of same parentcategory, to check that find out the parent of same catname
            var Parentcatid = db.CategoryMasters.Where(x => x.CategoryName == cat.CategoryName).Select(x => x.ParentCategoryId).FirstOrDefault();

            if (Catid != 0 || Parentcatid != null)
            {
                //check the condition if true duplicate catname error ...
                if (Catid == cat.ParentCategoryId || Parentcatid == cat.ParentCategoryId)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                flag = false;
            }

            return flag;

        }
        public bool IsSameCategoryDetails(CategoryVM cat)
        {
            bool flag = false;

            var catid = db.CategoryMasters.Where(x => x.CategoryName == cat.CategoryName).Select(x => x.CategoryId).FirstOrDefault();

            var parentid = db.CategoryMasters.Where(x => x.CategoryId == catid).Select(x => x.ParentCategoryId).FirstOrDefault();

            if (catid == cat.CategoryId && parentid == cat.ParentCategoryId)
            {
                flag = true;
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        public List<CategoryMaster> GetCategory()
        {
            return db.CategoryMasters.Where(x => x.IsActive == true).ToList();
        }
        public List<CategoryMaster> SerchedCategory(string CategoryName)
        {
            return db.CategoryMasters.Where(x => x.CategoryName.Contains(CategoryName) && x.IsActive == true).ToList();
        }

        public CategoryVM EditCategory(CategoryVM catvm)
        {
            throw new NotImplementedException();
        }

        public CategoryMaster GetCategoryById(int CategoryId)
        {
            var catdata= db.CategoryMasters.Where(x => x.CategoryId == CategoryId).Select(x => x).FirstOrDefault();
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //var c = serializer.Serialize(catdata);
            //CategoryVM catvm = serializer.Deserialize<CategoryVM>(c);
            return catdata;
        }

        public void DeleteCategory(int CategoryId)
        {
            var catm = db.CategoryMasters.Where(x => x.CategoryId == CategoryId).Select(x => x).FirstOrDefault();
            catm.IsActive = false;
            db.Entry(catm).State = EntityState.Modified;
            db.SaveChanges();
        }

    }
}