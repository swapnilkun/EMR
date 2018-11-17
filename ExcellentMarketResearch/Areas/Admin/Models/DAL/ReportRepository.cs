using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ExcellentMarketResearch.Areas.Admin.Models.DAL
{
    public class ReportRepository:IReportRepository
    {
        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

        public void InsertReport(ReportVM r)
        {

            r.CreatedBy = 1;
            r.CreatedDate = DateTime.Now;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var s = serializer.Serialize(r);
            ReportMaster re = serializer.Deserialize<ReportMaster>(s);
            re.LongDescritpion = r.FullDescription;
            re.SinglePrice = r.PriceSingleUser;
            re.MultiUserPrice = r.PriceMultiUser;
            re.CorporateUserPrice = r.PriceCUL;
            re.NumberOfPages = r.NumberOfPage;
            re.PublishereId = r.PublisherId;
            re.ReportDeliveryTypeId = r.DeliveryTypeId;
            db.ReportMasters.Add(re);
            db.SaveChanges();
            re.ReportUrl = re.ReportUrl + "-" + re.ReportId;
            db.Entry(re).State = EntityState.Modified;
            db.SaveChanges();

        }

        public ReportVM EditGetReport(int id)
        {
            var z = (from l in db.ReportMasters
                     where l.ReportId == id
                     select new ReportVM
                     {
                         ReportId = l.ReportId,
                         ReportTitle = l.ReportTitle,
                         ReportUrl = l.ReportUrl,
                         ReportImage = l.ReportImage,
                         DeliveryTypeId = l.ReportDeliveryTypeId,
                         ReportTypeId = l.ReportTypeId,
                         MetaTitle = l.MetaTitle,
                         MetaKeywords = l.Keywords,
                         MetaDescription = l.MetaDescription,
                         PublisherId = l.PublishereId,
                         PublishingDate = l.PublishingDate,
                         NumberOfPage = l.NumberOfPages,
                         PriceSingleUser = l.SinglePrice,
                         PriceMultiUser = l.MultiUserPrice,
                         PriceCUL = l.CorporateUserPrice,
                       //  DiscountPercentage = l.DiscountPrice,
                         CreatedBy = l.CreatedBy,
                         CreatedDate = l.CreatedDate,
                         FullDescription = l.LongDescritpion,
                         TableofContent = l.TableOfContent,
                         IsActive = l.IsActive,
                       //  IsUpcomming = l.IsUpcomming,
                         CategoryId = l.CategoryId
                     }
                  ).FirstOrDefault();
            return z;
        }

        public bool EditPostReport(ReportVM r)
        {
            r.ModifiedBy = 1;
            r.ModifiedDate = DateTime.Now;
            bool flag = false;
            if (r.ReportUrl == null)
            {
                r.ReportUrl = ExcellentMarketResearch.Areas.Admin.Models.Common.GenerateSlug(r.ReportTitle) + "-" + r.ReportId;
            }

            if (IsSameReportData(r))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var report = serializer.Serialize(r);
                ReportMaster repo = serializer.Deserialize<ReportMaster>(report);
                repo.LongDescritpion = r.FullDescription;
                repo.SinglePrice = r.PriceSingleUser;
                repo.MultiUserPrice = r.PriceMultiUser;
                repo.CorporateUserPrice = r.PriceCUL;
                repo.NumberOfPages = r.NumberOfPage;
                repo.PublishereId = r.PublisherId;
                repo.ReportDeliveryTypeId = r.DeliveryTypeId;
                db.Entry(repo).State = EntityState.Modified;
                db.SaveChanges();
                flag = true;
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        public List<SelectListItem> GetParent(int catid)
        {
            var Categories = DDLGetparents(catid).OrderBy(x => x).ToArray();
            List<SelectListItem> ParentCat = new List<SelectListItem>();
            if (Categories.Length == 1)
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

        public List<SelectListItem> Getchild(int catid)
        {
            var Categories = DDLGetparents(catid).OrderBy(x => x).ToArray();

            List<SelectListItem> ChildCat = new List<SelectListItem>();
            int parentcatid = Categories[0];
            int childcatid = Categories[1];
            if (Categories.Length > 1)
            {
                ChildCat = db.CategoryMasters.Where(x => x.ParentCategoryId == parentcatid).Select(y => new
                {
                    Text = y.CategoryName,
                    Value = y.CategoryId,
                }).ToList().Select(z => new SelectListItem
                {
                    Text = z.Text,
                    Value = z.Value.ToString(),
                    Selected = z.Value == Categories[1]
                }
                    ).ToList();
            }
            else
            {
                ChildCat = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            }
            return ChildCat;
        }

        public List<SelectListItem> GetChilofChild(int catid)
        {
            var Categories = DDLGetparents(catid).OrderBy(x => x).ToArray();
            List<SelectListItem> Childofchild = new List<SelectListItem>();
            int parentcatid = Categories[1];

            if (Categories.Length > 2)
            {
                int childofchild = Categories[2];

                Childofchild = db.CategoryMasters.Where(x => x.ParentCategoryId == parentcatid).Select(y => new
                {
                    Text = y.CategoryName,
                    Value = y.CategoryId
                }).ToList().Select(z => new SelectListItem
                {
                    Text = z.Text,
                    Value = z.Value.ToString(),
                    Selected = z.Value == childofchild
                }).ToList();
            }
            else
            {
                Childofchild = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            }
            return Childofchild;
        }

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

        public bool IsSameReportData(ReportVM r)
        {
            bool flag = false;

            var reportTitleid = db.ReportMasters.Where(x => x.ReportTitle == r.ReportTitle).Select(x => x.ReportId).FirstOrDefault();
            if (reportTitleid == r.ReportId)
            {
                flag = true;
            }
            else
                flag = false;

            return flag;
        }
        public ReportVM GetReportById(int ReportId)
        {
            var reportdetails = (from l in db.ReportMasters
                                 join p in db.PublisherMasters on l.PublishereId equals p.PublisherId
                                 join c in db.CategoryMasters on l.CategoryId equals c.CategoryId
                                 join d in db.ReportDeliveryTypes on l.ReportDeliveryTypeId equals d.ReportDeliveryTypeId
                                 join rt in db.ReportTypes on l.ReportTypeId equals rt.ReportTypeId
                                 where l.ReportId == ReportId
                                 select new ReportVM
                                 {
                                     ReportId = l.ReportId,
                                     ReportTitle = l.ReportTitle,
                                     ReportUrl = l.ReportUrl,
                                     ReportImage = l.ReportImage,
                                     DeliveryTypeId = l.ReportDeliveryTypeId,
                                     ReportTypeId = l.ReportDeliveryTypeId,
                                     MetaTitle = l.MetaTitle,
                                     MetaKeywords = l.Keywords,
                                     MetaDescription = l.MetaDescription,
                                     PublisherId = l.PublishereId,
                                     PublishingDate = l.PublishingDate,
                                     NumberOfPage = l.NumberOfPages,
                                     PriceSingleUser = l.SinglePrice,
                                     PriceMultiUser = l.MultiUserPrice,
                                     PriceCUL = l.CorporateUserPrice,
                                     //  DiscountPercentage = l.DiscountPrice,
                                     CreatedBy = l.CreatedBy,
                                     CreatedDate = l.CreatedDate,
                                     FullDescription = l.LongDescritpion,
                                     TableofContent = l.TableOfContent,
                                     IsActive = l.IsActive,
                                     //  IsUpcomming = l.IsUpcomming,
                                     CategoryId = l.CategoryId,
                                 }
                              ).FirstOrDefault();
            return reportdetails;
        }
        public void ReportDelete(int ReportId)
        {
            var reportdetails = db.ReportMasters.Where(x => x.ReportId == ReportId).FirstOrDefault();
            reportdetails.IsActive = false;
            db.Entry(reportdetails).State = EntityState.Modified;
            db.SaveChanges();
        }

        
    }
}