using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;

namespace ExcellentMarketResearch.Areas.Admin.Models.DAL
{
    public class NewsRepository : INewsRepository
    {
        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();
        NewsMaster newsmaster = new NewsMaster();
        public List<NewsMaster> GetNews()
        {
            return (from n in db.NewsMasters
                    orderby n.CreatedDate descending
                    where n.IsActive == true
                    select new
                    {
                        NewsId = n.NewsId,
                        NewsTitle = n.NewsTitle
                    }
                     ).ToList().Select(x => new NewsMaster
                     {
                         NewsId = x.NewsId,
                         NewsTitle = x.NewsTitle
                     }).ToList();
        }

        public void InsertNews(NewsVM news)
        {
            if (news.NewsURL == null)
            {
                var newsurl = ExcellentMarketResearch.Areas.Admin.Models.Common.GenerateSlug(news.NewsTitle);
                news.NewsURL = newsurl;
            }
            news.CreatedBy = 1;
            news.CreatedDate = DateTime.Now;
            newsmaster.NewsImage = news.NewsImage;
            newsmaster.CreatedBy = 1;
            newsmaster.CreatedDate = DateTime.Now;
            newsmaster.NewsTitle = news.NewsTitle;
            newsmaster.NewsUrl = news.NewsURL;
            newsmaster.NewsDescription = news.NewsDetail;
            newsmaster.MetaDescritpion = news.MetaDescription;
            newsmaster.Keywords = news.MetaKeywords;
            newsmaster.PublishingDate = news.NewsPublishingDate;
            newsmaster.MetaTitle = news.MetaTitle;
            newsmaster.CategoryId = news.CategoryId;
            newsmaster.IsActive = news.IsActive;


            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //var n = serializer.Serialize(news);
            //NewsMaster newsmaster = serializer.Deserialize<NewsMaster>(n);
            try
            {

                db.NewsMasters.Add(newsmaster);
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    foreach (var validationerrors in validationError.ValidationErrors)
                    {
                        System.Console.WriteLine("property name{0}", validationerrors.PropertyName);
                    }
                }
            }
        }

        public NewsVM EditNews(int id)
        {
            var z = db.NewsMasters.Where(x => x.NewsId == id).Select(x => x).FirstOrDefault();
            NewsVM news = new NewsVM();
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //var s = serializer.Serialize(z);
            //NewsVM news = serializer.Deserialize<NewsVM>(s);
            news.CategoryId = z.CategoryId;
            news.NewsDetail = z.NewsDescription;
            news.NewsId = z.NewsId;
            news.NewsPublishingDate = z.PublishingDate;
            news.NewsTitle = z.NewsTitle;
            news.NewsURL = z.NewsUrl;
            news.IsActive = (bool)z.IsActive;
            news.CreatedBy = z.CreatedBy;
            news.CreatedDate = z.CreatedDate;
            news.MetaKeywords = z.Keywords;
            news.MetaTitle = z.MetaTitle;
            news.MetaDescription = z.MetaDescritpion;
            news.NewsImage = z.NewsImage;
            return news;

        }
        public void EditpostNews(NewsVM z)
        {
            NewsMaster news = new NewsMaster();

            news.ModifiedBy = 1;
            news.ModifiedDate = DateTime.Now;
            news.CategoryId = z.CategoryId;
            news.NewsDescription = z.NewsDetail;
            news.NewsId = z.NewsId;
            news.PublishingDate = z.NewsPublishingDate;
            news.NewsTitle = z.NewsTitle;
            news.NewsUrl = z.NewsURL;
            news.IsActive = z.IsActive;
            news.CreatedBy = z.CreatedBy;
            news.CreatedDate = z.CreatedDate;
            news.Keywords = z.MetaKeywords;
            news.MetaTitle = z.MetaTitle;
            news.MetaDescritpion = z.MetaDescription;
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //var s = serializer.Serialize(news);
            //NewsMaster newsmaster = serializer.Deserialize<NewsMaster>(s);
            try 
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    foreach (var validationerrors in validationError.ValidationErrors)
                    {
                        System.Console.WriteLine("property name{0}", validationerrors.PropertyName);
                    }
                }
            }
          
        }
        public bool IsValidNewsData(NewsVM news)
        {
            bool flag = false;
            var newsTitlewiseid = db.NewsMasters.Where(x => x.NewsTitle == news.NewsTitle).Select(x => x.NewsId).FirstOrDefault();

            if (newsTitlewiseid == news.NewsId)
            {
                flag = true;
                var newsUrlwiseid = db.NewsMasters.Where(x => x.NewsUrl == news.NewsURL).Select(x => x.NewsId).FirstOrDefault();
                if (newsUrlwiseid == news.NewsId)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                    NewsUrlDuplicatError();
                }
            }
            else
            {
                flag = false;
                NewsTitleDuplicatError();
            }

            return flag;

        }
        public string NewsUrlDuplicatError()
        {
            var z = "Duplicate News Url....";
            return z;
        }

        public string NewsTitleDuplicatError()
        {
            var z = "Duplicate News Title ....";
            return z;
        }

        public List<CategoryVM> GetCategories()
        {
            var x = (from c in db.CategoryMasters
                     where c.ParentCategoryId == 0
                     orderby c.CategoryName ascending
                     select new CategoryVM
                     {
                         CategoryId = c.CategoryId,
                         CategoryName = c.CategoryName
                     }).ToList();
            return x;

        }


        public NewsMaster GetNewsById(int newsid)
        {
            return db.NewsMasters.Where(x => x.NewsId == newsid).Select(x => x).FirstOrDefault();
        }

        public NewsVM EditNews(NewsVM newvm)
        {
            throw new NotImplementedException();
        }

        public void NewsDelete(int newsid)
        {
            throw new NotImplementedException();
        }

    }
}