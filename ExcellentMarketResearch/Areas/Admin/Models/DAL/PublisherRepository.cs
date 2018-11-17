using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ExcellentMarketResearch.Areas.Admin.Models.DAL
{
    public class PublisherRepository
    {
        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();
        public void InsertPublisher(PublisherVM pub)
        {
            pub.CreatedBy = 1;
            pub.CreatedDate = DateTime.Now;
            if (pub.PublisherUrl == null)
            {
                var url = ExcellentMarketResearch.Areas.Admin.Models.Common.GenerateSlug(pub.PublisherName);
                pub.PublisherUrl = url;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var c = serializer.Serialize(pub);
            PublisherMaster pubmaster = serializer.Deserialize<PublisherMaster>(c);
            try
            {
                db.PublisherMasters.Add(pubmaster);
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
     
        }
        public List<PublisherMaster> GetPublisher()
        {
            return db.PublisherMasters.Where(x => x.IsValid == true).ToList();
        }
        public PublisherVM GetById(int id)
        {
        var pub = (from l in db.PublisherMasters
                       where l.PublisherId == id
                       select new PublisherVM
                       {
                           PublisherId = l.PublisherId,
                           PublisherUrl = l.publisherUrl,
                           CreatedBy = l.CreatedBy,
                           CreatedDate = l.CreatedDate,
                           LongDescription = l.LongDescription,
                           ShortDescription = l.ShortDescription,
                           MetaDescription = l.MetaDescription,
                           PublisherName = l.PublisherName,
                           ContactName = l.ContactName,
                           PublisherContactNumber = l.PublisherContactNumber,
                           Address = l.Address
                       }).FirstOrDefault();
        return pub;

    }
        public void PublisherEdit(PublisherVM pub)
        {
            pub.ModifiedBy = 1;
            pub.ModifiedDate = DateTime.Now;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var publish = serializer.Serialize(pub);
            PublisherMaster pubmast = serializer.Deserialize<PublisherMaster>(publish);
            db.Entry(pubmast).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void DeletePublisher(int id)
        {
            var r=db.PublisherMasters.Where(x => x.PublisherId == id).FirstOrDefault();
            r.IsValid = false;
            //return db.PublisherMasters.Where(x => x.IsValid == true).ToList();

        }

            
    }
}