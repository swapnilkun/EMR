using ExcellentMarketResearch.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExcellentMarketResearch.Areas.Admin.Controllers
{
    public class ExcelNPOIController : Controller
    {
        //
        // GET: /Admin/ExcelNPOI/
        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Import()
        {
            return View();
        }
        [HttpPost]
        // [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult Import(HttpPostedFileBase excelfile)
        {
            string Value = string.Empty;
            List<ReportMaster> reportlist = new List<ReportMaster>();
            //   Excelimport (excelfile);
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                ViewBag.Error = "Upload the excel file !....";
                return View("Index");
            }

            else
            {

                if (excelfile.FileName.EndsWith("xsl") || excelfile.FileName.EndsWith("xlsx"))
                {

                    string path = "";
                    //   string path = Server.MapPath("~/Content/" + excelfile.FileName + DateTime.Now.ToString("MM.dd.yyyy-hh.mm.ss"));
                    try
                    {
                        //path = Server.MapPath("~/UploadSheet/" + excelfile.FileName + DateTime.Now.ToString("MM.dd.yyyy"));
                        path = Server.MapPath("~/UploadSheet/");
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        excelfile.SaveAs(path + Path.GetFileName(excelfile.FileName));
                        path = path + Path.GetFileName(excelfile.FileName);
                        string sFileExtention = Path.GetExtension(path);
                        ISheet sheet;
                        
                        using (var stream = new FileStream(path, FileMode.Open))
                        {
                            stream.Position = 0;
                            if (sFileExtention == ".xls")
                            {
                                HSSFWorkbook hSSFWorkbook = new HSSFWorkbook(stream);
                                sheet = hSSFWorkbook.GetSheetAt(0);

                            }
                            else
                            {
                                XSSFWorkbook sSFWorkbook = new XSSFWorkbook(stream);
                                sheet = sSFWorkbook.GetSheetAt(0);

                            }
                            IRow hrow = sheet.GetRow(0);
                            int lastcell = hrow.LastCellNum;
                            string HTMLTable = "<table border='1'>";
                            for (int i = 1; i <= sheet.LastRowNum; i++)
                            {
                                IRow row1 = sheet.GetRow(i);
                                ReportMaster r = new ReportMaster();

                                int _SerialNumber;
                                int SerialNumber = 0;
                                int _CategoryID;
                                int _PublisherID;
                                int _DeliveryFormatID;
                                DateTime _PublishedDate;
                                int _NumberOfPages;
                                Decimal _SingleUserPrice;
                                Decimal _MultiUserPrice;
                                Decimal _CorporateUserPrice;
                                bool IsVaidate = true;
                                string _MetaTitle = "";
                                string Message = "";
                                string ReportTitleTemp = string.Empty;
                                for (int j = 0; j < lastcell; j++)
                                {

                                    if (Convert.ToString(hrow.Cells[j]) == "Serial Number")
                                    {
                                        if (Int32.TryParse(row1.Cells[j].ToString().Trim(), out _SerialNumber))
                                            SerialNumber = _SerialNumber;
                                        else
                                        { IsVaidate = false; Message += "Serial Number<br />"; }
                                    }
                                    else if (Convert.ToString(hrow.Cells[j]) == "Category ID")
                                    {
                                        if (Int32.TryParse(row1.Cells[j].ToString().Trim(), out _CategoryID))
                                            r.CategoryId = _CategoryID;
                                        else
                                        { IsVaidate = false; Message += "Category ID <br />"; }
                                    }
                                    else if (hrow.Cells[j].ToString() == "Publisher ID")
                                    {
                                        if (Int32.TryParse(row1.Cells[j].ToString().Trim(), out _PublisherID))
                                            r.PublishereId = _PublisherID;
                                        else
                                        { IsVaidate = false; Message += "Publisher ID <br />"; }
                                    }
                                    else if (hrow.Cells[j].ToString() == "Delivery Format ID")
                                    {
                                        if (Int32.TryParse(row1.Cells[j].ToString().Trim(), out _DeliveryFormatID))
                                            r.ReportDeliveryTypeId = _DeliveryFormatID;
                                        else
                                        { IsVaidate = false; Message += "Delivery Format ID <br />"; }
                                    }
                                    else if (hrow.Cells[j].ToString() == "Title")
                                    {
                                        ReportTitleTemp = Convert.ToString(row1.Cells[j].ToString().Trim());
                                        if (db.ReportMasters.Count(g => g.ReportTitle == ReportTitleTemp) > 0 ? true : false)

                                        { IsVaidate = false; Message += "Title <br />"; }
                                        else
                                            r.ReportTitle = row1.Cells[j].ToString().Trim().Replace("   ", " ").Replace("  ", " ").Replace("  ", " ");

                                        r.ReportUrl = ExcellentMarketResearch.Areas.Admin.Models.Common.GenerateSlug(row1.Cells[j].ToString().Trim());

                                        if (db.ReportMasters.Count(url => url.ReportUrl == r.ReportUrl.ToLower()) > 0 ? true : false)
                                        { r.ReportUrl = ""; IsVaidate = false; Message += "Long URL <br />"; }
                                    }
                                    else if (hrow.Cells[j].ToString() == "Published Date")
                                    {
                                        if (DateTime.TryParse(row1.Cells[j].ToString().Trim(), out _PublishedDate))
                                            r.PublishingDate = _PublishedDate;
                                        else
                                        { IsVaidate = false; Message += "Published Date <br />"; }
                                    }
                                    else if (hrow.Cells[j].ToString() == "Number Of Pages")
                                    {
                                        if (Int32.TryParse(row1.Cells[j].ToString().Trim(), out _NumberOfPages))
                                            r.NumberOfPages = _NumberOfPages;
                                        else
                                        { IsVaidate = false; Message += "Number Of Pages <br />"; }
                                    }
                                    else if (hrow.Cells[j].ToString() == "Long Description")
                                    {
                                        if (row1.Cells[j].ToString().Trim() != "")
                                            r.LongDescritpion = "<pre>" + row1.Cells[j].ToString().Trim() + "</pre>";
                                        else
                                        { IsVaidate = false; Message += "Long Description \n"; }
                                    }
                                    else if (hrow.Cells[j].ToString() == "Long Table Of Content")
                                        r.TableOfContent = "<pre>" + row1.Cells[j].ToString().Trim() + "</pre>";

                                    else if (hrow.Cells[j].ToString() == "Single User Price")
                                    {
                                        if (Decimal.TryParse(row1.Cells[j].ToString().Trim() == "" ? "0" : row1.Cells[j].ToString().Trim(), out _SingleUserPrice))
                                            r.SinglePrice = _SingleUserPrice;
                                        else
                                        { IsVaidate = false; Message += "Single User Price <br />"; }
                                    }
                                    else if (hrow.Cells[j].ToString() == "Multi User Price")
                                    {
                                        if (Decimal.TryParse(row1.Cells[j].ToString().Trim() == "" ? "0" : row1.Cells[j].ToString().Trim(), out _MultiUserPrice))
                                            r.MultiUserPrice = _MultiUserPrice;
                                        else
                                        { IsVaidate = false; Message += "Multi User Price <br />"; }
                                    }
                                    else if (hrow.Cells[j].ToString() == "Corporate User Price")
                                    {
                                        if (Decimal.TryParse(row1.Cells[j].ToString().Trim() == "" ? "0" : row1.Cells[j].ToString().Trim(), out _CorporateUserPrice))
                                            r.CorporateUserPrice = _CorporateUserPrice;
                                        else
                                        { IsVaidate = false; Message += "Corporate User Price <br />"; }
                                    }
                                }
                                    if (r.SinglePrice <= 0 && r.MultiUserPrice <= 0 && r.CorporateUserPrice <= 0)
                                    { IsVaidate = false; Message += "Add Atleast one Price <br />"; }

                                    if (IsVaidate)
                                    {
                                        r.ReportImage = null;

                                        r.IsActive = true;
                                        
                                        r.ReportTypeId = r.ReportDeliveryTypeId;
                                        r.CreatedBy = 1;//Convert.ToInt32(QYGroupRepository.Areas.Admin.Models.CommonCode.MySession());
                                        r.CreatedDate = DateTime.Now;
                                        //reportlist.Add(r);
                                        db.ReportMasters.Add(r);
                                        db.SaveChanges();
                                        var repturl = r.ReportUrl + ".html";
                                        r.ReportUrl = repturl;
                                        db.Entry(r).State = EntityState.Modified;
                                        db.SaveChanges();
                                        Message += "Inserted <br/>";
                                    }
                                HTMLTable += "<tr><td>" + SerialNumber + "</td><td>" + ReportTitleTemp + "</td><td>" + r.ReportUrl + "</td><td>" + Message + "</td></tr>";
                            }
                            HTMLTable += "</table>";
                            ViewBag.Html = HTMLTable;
                            return View();
                            //ViewData.Add("HTMLTable", HTMLTable);
                            //ViewData["HTMLTable"] = HTMLTable;
                            }
                        }
                        
                 
                                      
                    catch (Exception ex)
                    {
                        var x = "<br />Report status: The file could not be processed. The following error occured: " + ex.Message;
                    }
                    finally
                    {
                        
                    }

                }
                
            }
            return View();
        }

    }
}
