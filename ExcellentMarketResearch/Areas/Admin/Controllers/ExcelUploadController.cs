using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcellentMarketResearch.Models;
using System.Data.OleDb;
using ExcellentMarketResearch.Areas.Admin.Models;
using System.IO;

namespace ExcellentMarketResearch.Areas.Admin.Controllers
{
    public class ExcelUploadController : Controller
    {
        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();
        //
        // GET: /Admin/ExcelUpload/
        public ActionResult Index()
        {
            //var x = db.ReportMasters.ToList();
            return View();
        }

        public ActionResult Import()
        {
            return View();
        }
        [HttpPost]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult Import(HttpPostedFileBase excelfile)
        {
            string Value = string.Empty;

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
                        path =path + Path.GetFileName(excelfile.FileName);
                       // excelfile.SaveAs(path);
                    }
                    catch (Exception ex)
                    {

                    }
                    System.Data.OleDb.OleDbConnection myConnection = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; " + "data source='" + path + "';" + "Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\" ");
                    try
                    {
                        myConnection.Open();
                        DataTable mySheets = myConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                        DataSet ds = new DataSet();
                        DataTable dt;

                        for (int i = 0; i < mySheets.Rows.Count; i++)
                        {
                            dt = makeDataTableFromSheetName(path, mySheets.Rows[i]["TABLE_NAME"].ToString());
                            ds.Tables.Add(dt);

                        }
                        if (ds.Tables.Count > 0)
                        {
                            CreateReport(ds);
                            return View();

                        }
                        //StatusLabel.Text += "<br />Report status: File processed!";
                    }
                    catch (Exception ex)
                    {
                        //btnDownloadReport.Visible = false;
                        //grdReport.DataSource = null;
                        //grdReport.DataBind();
                        var x = "<br />Report status: The file could not be processed. The following error occured: " + ex.Message;
                    }
                    finally
                    {
                        myConnection.Close();
                    }

                }

            }
            return View();
        }

        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult TotalReports()
        {
            var Reports = db.ReportMasters.ToList();
            return View(Reports);
        }

        private static DataTable makeDataTableFromSheetName(string filename, string sheetName)
        {
            System.Data.OleDb.OleDbConnection myConnection = new System.Data.OleDb.OleDbConnection(
            "Provider=Microsoft.ACE.OLEDB.12.0; " +
            "data source='" + filename + "';" +
            "Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\" ");

            DataTable dtImport = new DataTable();
            System.Data.OleDb.OleDbDataAdapter myImportCommand = new System.Data.OleDb.OleDbDataAdapter("select * from [" + sheetName + "]", myConnection);
            myImportCommand.Fill(dtImport);
            return dtImport;
        }

        public void CreateReport(DataSet ds)
        {
            DataTable dtReport = CreateDataSet();
            DataSet dsReport = new DataSet();
            ReportMaster r = new ReportMaster();
            List<ReportMaster> reportlist = new List<ReportMaster>();

            string HTMLTable = "<table border='1'>";
            for (int tables = 0; tables < ds.Tables.Count; tables++)
            {
                #region rowdata
                for (int rows = 0; rows < ds.Tables[tables].Rows.Count; rows++)
                {


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
                    string Message = "";
                    string ReportTitleTemp = string.Empty;

                    for (int columns = 0; columns < ds.Tables[tables].Columns.Count; columns++)
                    {
                        if (ds.Tables[tables].Columns[columns].ToString() == "Serial Number")
                        {
                            if (Int32.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _SerialNumber))
                                SerialNumber = _SerialNumber;
                            else
                            { IsVaidate = false; Message += "Serial Number<br />"; }
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "Category ID")
                        {
                            if (Int32.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _CategoryID))
                                r.CategoryId = _CategoryID;
                            else
                            { IsVaidate = false; Message += "Category ID <br />"; }
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "Publisher ID")
                        {
                            if (Int32.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _PublisherID))
                                r.PublishereId = _PublisherID;
                            else
                            { IsVaidate = false; Message += "Publisher ID <br />"; }
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "Delivery Format ID")
                        {
                            if (Int32.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _DeliveryFormatID))
                                r.ReportDeliveryTypeId = _DeliveryFormatID;
                            else
                            { IsVaidate = false; Message += "Delivery Format ID <br />"; }
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "Title")
                        {
                            ReportTitleTemp = Convert.ToString(ds.Tables[tables].Rows[rows][columns].ToString().Trim());
                            if (db.ReportMasters.Count(g => g.ReportTitle == ReportTitleTemp) > 0 ? true : false)

                            { IsVaidate = false; Message += "Title <br />"; }
                            else
                                r.ReportTitle = ds.Tables[tables].Rows[rows][columns].ToString().Trim().Replace("   ", " ").Replace("  ", " ").Replace("  ", " ");

                            r.ReportUrl = ExcellentMarketResearch.Areas.Admin.Models.Common.GenerateSlug(ds.Tables[tables].Rows[rows][columns].ToString().Trim());

                            if (db.ReportMasters.Count(url => url.ReportUrl == r.ReportUrl.ToLower()) > 0 ? true : false)
                            { r.ReportUrl = ""; IsVaidate = false; Message += "Long URL <br />"; }
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "Published Date")
                        {
                            if (DateTime.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _PublishedDate))
                                r.PublishingDate = _PublishedDate;
                            else
                            { IsVaidate = false; Message += "Published Date <br />"; }
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "Number Of Pages")
                        {
                            if (Int32.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _NumberOfPages))
                                r.NumberOfPages = _NumberOfPages;
                            else
                            { IsVaidate = false; Message += "Number Of Pages <br />"; }
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "Long Description")
                        {
                            if (ds.Tables[tables].Rows[rows][columns].ToString().Trim() != "")
                                r.LongDescritpion = "<pre>" + ds.Tables[tables].Rows[rows][columns].ToString().Trim() + "</pre>";
                            else
                            { IsVaidate = false; Message += "Long Description \n"; }
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "Long Table Of Content")
                            r.TableOfContent = "<pre>" + ds.Tables[tables].Rows[rows][columns].ToString().Trim() + "</pre>";

                        else if (ds.Tables[tables].Columns[columns].ToString() == "Single User Price")
                        {
                            if (Decimal.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim() == "" ? "0" : ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _SingleUserPrice))
                                r.SinglePrice = _SingleUserPrice;
                            else
                            { IsVaidate = false; Message += "Single User Price <br />"; }
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "Multi User Price")
                        {
                            if (Decimal.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim() == "" ? "0" : ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _MultiUserPrice))
                                r.MultiUserPrice = _MultiUserPrice;
                            else
                            { IsVaidate = false; Message += "Multi User Price <br />"; }
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "Corporate User Price")
                        {
                            if (Decimal.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim() == "" ? "0" : ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _CorporateUserPrice))
                                r.CorporateUserPrice = _CorporateUserPrice;
                            else
                            { IsVaidate = false; Message += "Corporate User Price <br />"; }
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "Meta Title")
                            r.MetaTitle = ds.Tables[tables].Rows[rows][columns].ToString().Trim();
                        else if (ds.Tables[tables].Columns[columns].ToString() == "Meta Description")
                            r.MetaDescription = ds.Tables[tables].Rows[rows][columns].ToString().Trim();
                        else if (ds.Tables[tables].Columns[columns].ToString() == "Meta Keywords")
                            r.Keywords = ds.Tables[tables].Rows[rows][columns].ToString().Trim();
                    }

                    if (r.SinglePrice <= 0 && r.MultiUserPrice <= 0 && r.CorporateUserPrice <= 0)
                    { IsVaidate = false; Message += "Add Atleast one Price <br />"; }

                    //Save data in Database.
                    if (IsVaidate)
                    {
                        r.ReportImage = null;
                        //  r.ListOfCharts = null;
                        //  r.FreeAnalysis = null;
                        //   r.Summary = "null";
                        //   r.Methodology = null;

                        // r.DiscountPrice = 0;
                        r.IsActive = true;
                        // r.IsUpcomming = true;
                        r.ReportTypeId = r.ReportDeliveryTypeId;
                        r.CreatedBy = 1;//Convert.ToInt32(QYGroupRepository.Areas.Admin.Models.CommonCode.MySession());
                        r.CreatedDate = DateTime.Now;
                        reportlist.Add(r);
                        db.ReportMasters.Add(r);
                        db.SaveChanges();
                        var repturl = r.ReportUrl + ".html";
                        r.ReportUrl = repturl;
                        db.Entry(r).State = EntityState.Modified;
                        db.SaveChanges();

                        //PriceMaster p = new PriceMaster();

                        //p.PriceType = "Single User Price ";
                        //p.Price = r.PriceSingleUser;
                        //db.PriceMasters.Add(p);
                        //db.SaveChanges();

                        //p.PriceType = "Multi User Price ";
                        //p.Price = r.PriceMultiUser;
                        //db.PriceMasters.Add(p);
                        //db.SaveChanges();

                        //p.PriceType = "Corporate User Price ";
                        //p.Price = r.PriceCUL;
                        //db.PriceMasters.Add(p);
                        //db.SaveChanges();

                        Message += "Inserted <br/>";
                    }
                    HTMLTable += "<tr><td>" + SerialNumber + "</td><td>" + ReportTitleTemp + "</td><td>" + r.ReportUrl + "</td><td>" + Message + "</td></tr>";
                }
                HTMLTable += "</table>";

                ViewData.Add("HTMLTable", HTMLTable);
                #endregion rows
            }
            //return View();
        }
        //private DataSet CreateReport(DataSet ds)
        //{
        //    DataTable dtReport = CreateDataSet();
        //    DataSet dsReport = new DataSet();

        //    if (true)
        //    {
        //        for (int tables = 0; tables < ds.Tables.Count; tables++)
        //        {
        //            for (int rows = 0; rows < ds.Tables[tables].Rows.Count; rows++)
        //            {
        //                ReportMaster r = new ReportMaster();
        //                List<ReportMaster> reportlist = new List<ReportMaster>();

        //                int _SerialNumber;
        //                int SerialNumber = 0;
        //                int _CategoryID;
        //                int _PublisherID;    
        //                int _DeliveryFormatID;                  
        //                DateTime _PublishedDate;
        //                int _NumberOfPages;
        //                Decimal _SingleUserPrice;
        //                Decimal _MultiUserPrice;
        //                Decimal _CorporateUserPrice;
        //                bool IsVaidate = true;
        //                string Message = "";

        //                for (int columns = 0; columns < ds.Tables[tables].Columns.Count; columns++)
        //                {
        //                    if (ds.Tables[tables].Columns[columns].ToString() == "Serial Number")
        //                    {
        //                        if (Int32.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _SerialNumber))
        //                            SerialNumber = _SerialNumber;
        //                        else
        //                        { IsVaidate = false; Message += "Serial Number<br />"; }
        //                    }
        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Category ID")
        //                    {
        //                        if (Int32.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _CategoryID))
        //                            r.CategoryId = _CategoryID;
        //                        else
        //                        { IsVaidate = false; Message += "Category ID <br />"; }
        //                    }
        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Publisher ID")
        //                    {
        //                        if (Int32.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _PublisherID))
        //                            r.PublishereId = _PublisherID;
        //                        else
        //                        { IsVaidate = false; Message += "Publisher ID <br />"; }
        //                    }
        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Delivery Format ID")
        //                    {
        //                        if (Int32.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _DeliveryFormatID))
        //                            r.ReportDeliveryTypeId = _DeliveryFormatID;
        //                        else
        //                        { IsVaidate = false; Message += "Delivery Format ID <br />"; }
        //                    }
        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Title")
        //                    {

        //                        if (db.ReportMasters.Count(g => g.ReportTitle == ds.Tables[tables].Rows[rows][columns].ToString().Trim())>0?true:false)

        //                        { IsVaidate = false; Message += "Title <br />"; }
        //                        else
        //                           r.ReportTitle = ds.Tables[tables].Rows[rows][columns].ToString().Trim().Replace("   ", " ").Replace("  ", " ").Replace("  ", " ");

        //                       r.ReportUrl = ExcellentMarketResearch.Areas.Admin.Models.Common.GenerateSlug(ds.Tables[tables].Rows[rows][columns].ToString().Trim());

        //                       if (db.ReportMasters.Count(url => url.ReportUrl == r.ReportUrl.ToLower()) > 0 ? true : false)
        //                       { r.ReportUrl = ""; IsVaidate = false; Message += "Long URL <br />"; }
        //                    }
        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Published Date")
        //                    {
        //                        if (DateTime.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _PublishedDate))
        //                            r.PublishingDate = _PublishedDate;
        //                        else
        //                        { IsVaidate = false; Message += "Published Date <br />"; }
        //                    }
        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Number Of Pages")
        //                    {
        //                        if (Int32.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _NumberOfPages))
        //                            r.NumberOfPages = _NumberOfPages;
        //                        else
        //                        { IsVaidate = false; Message += "Number Of Pages <br />"; }
        //                    }
        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Long Description")
        //                    {
        //                        if (ds.Tables[tables].Rows[rows][columns].ToString().Trim() != "")
        //                            r.LongDescritpion= ds.Tables[tables].Rows[rows][columns].ToString().Trim();
        //                        else
        //                        { IsVaidate = false; Message += "Long Description \n"; }
        //                    }
        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Long Table Of Content")
        //                       r.TableOfContent= "<pre>" + ds.Tables[tables].Rows[rows][columns].ToString().Trim() + "</pre>";

        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Single User Price")
        //                    {
        //                        if (Decimal.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim() == "" ? "0" : ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _SingleUserPrice))
        //                            r.SinglePrice = _SingleUserPrice;
        //                        else
        //                        { IsVaidate = false; Message += "Single User Price <br />"; }
        //                    }
        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Multi User Price")
        //                    {
        //                        if (Decimal.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim() == "" ? "0": ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _MultiUserPrice))
        //                            r.MultiUserPrice = _MultiUserPrice;
        //                        else
        //                        { IsVaidate = false; Message += "Multi User Price <br />"; }
        //                    }
        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Corporate User Price")
        //                    {
        //                        if (Decimal.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim() == "" ? "0": ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _CorporateUserPrice))
        //                            r.CorporateUserPrice = _CorporateUserPrice;
        //                        else
        //                        { IsVaidate = false; Message += "Corporate User Price <br />"; }
        //                    }
        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Meta Title")
        //                       r.MetaTitle = ds.Tables[tables].Rows[rows][columns].ToString().Trim();
        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Meta Description")
        //                      r.MetaDescription = ds.Tables[tables].Rows[rows][columns].ToString().Trim();
        //                    else if (ds.Tables[tables].Columns[columns].ToString() == "Meta Keywords")
        //                       r.Keywords = ds.Tables[tables].Rows[rows][columns].ToString().Trim();
        //                }

        //                if (r.SinglePrice <= 0 && r.MultiUserPrice <= 0 && r.CorporateUserPrice <= 0)
        //                { IsVaidate = false; Message += "Add Atleast one Price <br />"; }

        //                //Save data in Database.
        //                if (IsVaidate)
        //                {
        //                    r.ReportImage = null;
        //                    r.ListOfCharts = null;
        //                  //  r.FreeAnalysis = null;
        //                   // r.Summary = "null";
        //                    //r.Methodology = null;
        //                   // r.DiscountPrice = 0;
        //                    r.IsActive = true;
        //                    //r.IsUpcomming = true;
        //                    r.ReportTypeId = r.ReportDeliveryTypeId;
        //                    r.CreatedBy = Convert.ToInt32(ExcellentMarketResearch.Areas.Admin.Models.Common.MySession());
        //                    r.CreatedDate = DateTime.Now;
        //                    reportlist.Add(r);
        //                    db.ReportMasters.Add(r);
        //                    db.SaveChanges();
        //                    var repturl = r.ReportUrl + "/" + r.ReportId.ToString();
        //                    r.ReportUrl = repturl;
        //                    db.Entry(r).State = EntityState.Modified;
        //                    //db.Entry(rid).CurrentValues.SetValues(reportmaster);
        //                    db.SaveChanges();

        //                }
        //                else
        //                {
        //                    return View();
        //                }



        //            }


        //        }
        //    }

        //    dsReport.Tables.Add(dtReport);
        //    return dsReport;
        //}

        private DataTable CreateDataSet()
        {
            DataTable dtReport = new DataTable("dtReport");

            DataColumn SerialNumber = new DataColumn("Serial Number");
            SerialNumber.DataType = System.Type.GetType("System.Int32");
            dtReport.Columns.Add(SerialNumber);

            DataColumn PublisherID = new DataColumn("Publisher ID");
            PublisherID.DataType = System.Type.GetType("System.Int32");
            dtReport.Columns.Add(PublisherID);

            DataColumn Title = new DataColumn("Title");
            Title.DataType = System.Type.GetType("System.String");
            dtReport.Columns.Add(Title);

            DataColumn LongURL = new DataColumn("Long URL");
            LongURL.DataType = System.Type.GetType("System.String");
            dtReport.Columns.Add(LongURL);

            DataColumn SingleUserPrice = new DataColumn("Single User Price");
            SingleUserPrice.DataType = System.Type.GetType("System.Decimal");
            dtReport.Columns.Add(SingleUserPrice);

            DataColumn MultiUserPrice = new DataColumn("Multi User Price");
            MultiUserPrice.DataType = System.Type.GetType("System.Decimal");
            dtReport.Columns.Add(MultiUserPrice);

            DataColumn CorporateUserPrice = new DataColumn("Corporate User Price");
            CorporateUserPrice.DataType = System.Type.GetType("System.Decimal");
            dtReport.Columns.Add(CorporateUserPrice);

            DataColumn Message = new DataColumn("Message");
            Message.DataType = System.Type.GetType("System.String");
            dtReport.Columns.Add(Message);


            return dtReport;
        }


    }
}
