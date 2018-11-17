using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcellentMarketResearch.Areas.Admin.Models;

namespace ExcellentMarketResearch.Areas.Admin.Controllers
{
    public class ExcelCategoryController : Controller
    {
        //
        // GET: /Admin/ExcelCategory/

        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();
        //
        // GET: /Admin/ExcelUpload/
        public ActionResult Index()
        {
            //var x = db.ReportMasters.ToList();
            return View();
        }

        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
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
                        path = Server.MapPath("~/Content/" + excelfile.FileName + DateTime.Now.ToString("MM.dd.yyyy-hh.mm.ss"));
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        excelfile.SaveAs(path);
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
                            CreateCategory(ds);
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

        public void CreateCategory(DataSet ds)
        {
            DataTable dtReport = CreateDataSet();
            DataSet dsReport = new DataSet();
            CategoryMaster r = new CategoryMaster();
            List<CategoryMaster> categorylist = new List<CategoryMaster>();

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
                    string CategoryTempName = string.Empty;

                    for (int columns = 0; columns < ds.Tables[tables].Columns.Count; columns++)
                    {
                        if (ds.Tables[tables].Columns[columns].ToString() == "Sr.No")
                        {
                            if (Int32.TryParse(ds.Tables[tables].Rows[rows][columns].ToString().Trim(), out _SerialNumber))
                                SerialNumber = _SerialNumber;
                            else
                            { IsVaidate = false; Message += "Serial Number<br />"; }
                        }                                 
                        else if (ds.Tables[tables].Columns[columns].ToString() == "CategoryName")
                        {
                            CategoryTempName = Convert.ToString(ds.Tables[tables].Rows[rows][columns].ToString().Trim());
                            if (db.CategoryMasters.Count(g => g.CategoryName == CategoryTempName) > 0 ? true : false)

                            { IsVaidate = false; Message += "Title <br />"; }
                            else
                                r.CategoryName = ds.Tables[tables].Rows[rows][columns].ToString().Trim().Replace("   ", " ").Replace("  ", " ").Replace("  ", " ");

                            r.CategoryUrl = ExcellentMarketResearch.Areas.Admin.Models.Common.GenerateSlug(ds.Tables[tables].Rows[rows][columns].ToString().Trim());

                            if (db.CategoryMasters.Count(url => url.CategoryUrl == r.CategoryUrl.ToLower()) > 0 ? true : false)
                            { r.CategoryUrl = ""; IsVaidate = false; Message += "Long URL <br />"; }
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "ParentCategoryId")
                        {
                            r.ParentCategoryId = Convert.ToInt32(ds.Tables[tables].Rows[rows][columns].ToString());
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "ShortDescription")
                        {
                            r.ShortDescription = ds.Tables[tables].Rows[rows][columns].ToString().Trim();
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "LongDescription")
                        {
                            r.LongDescription = ds.Tables[tables].Rows[rows][columns].ToString().Trim();
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "MetaTitle")
                            r.MetaTitle = ds.Tables[tables].Rows[rows][columns].ToString().Trim();

                        else if (ds.Tables[tables].Columns[columns].ToString() == "Keywords")
                        {
                            r.Keywords = ds.Tables[tables].Rows[rows][columns].ToString().Trim();
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "IsActive")
                        {
                            r.IsActive = true; //ds.Tables[tables].Rows[rows][columns].ToString().Trim();
                        }
                        else if (ds.Tables[tables].Columns[columns].ToString() == "MetaDescription")
                        {
                            r.MetaDescription = ds.Tables[tables].Rows[rows][columns].ToString().Trim();
                        }
                    }


                    //Save data in Database.
                    if (IsVaidate)
                    {
                      //  r.ReportImage = null;
                        //  r.ListOfCharts = null;
                        //  r.FreeAnalysis = null;
                        //   r.Summary = "null";
                        //   r.Methodology = null;

                        // r.DiscountPrice = 0;
                        r.IsActive = true;
                        // r.IsUpcomming = true;
                      //  r.ReportTypeId = r.ReportDeliveryTypeId;
                        r.CreatedBy = 1;//Convert.ToInt32(QYGroupRepository.Areas.Admin.Models.CommonCode.MySession());
                        r.CreatedDate = DateTime.Now;
                        categorylist.Add(r);

                     


                        try
                        {

                            db.CategoryMasters.Add(r);
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
                        //var repturl = r.ReportUrl + ".html";
                      //  r.ReportUrl = repturl;
                       //// db.Entry(r).State = EntityState.Modified;
                        //db.SaveChanges();

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
                    HTMLTable += "<tr><td>" + SerialNumber + "</td><td>" + CategoryTempName + "</td><td>" + r.CategoryUrl + "</td><td>" + Message + "</td></tr>";
                }
                HTMLTable += "</table>";

                ViewData.Add("HTMLTable", HTMLTable);
                #endregion rows
            }
            //return View();
        }
        

        private DataTable CreateDataSet()
        {
            DataTable dtReport = new DataTable("dtReport");

            DataColumn SerialNumber = new DataColumn("Serial Number");
            SerialNumber.DataType = System.Type.GetType("System.Int32");
            dtReport.Columns.Add(SerialNumber);


            DataColumn CategoryName = new DataColumn("CategoryName");
            CategoryName.DataType = System.Type.GetType("System.String");
            dtReport.Columns.Add(CategoryName);

            DataColumn ParentCategoryId = new DataColumn("ParentCategoryId");
            ParentCategoryId.DataType = System.Type.GetType("System.Int32");
            dtReport.Columns.Add(ParentCategoryId);


            DataColumn ShortDescription = new DataColumn("ShortDescription");
            ShortDescription.DataType = System.Type.GetType("System.String");
            dtReport.Columns.Add(ShortDescription);

            DataColumn SingleUserPrice = new DataColumn("LongDescription");
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
