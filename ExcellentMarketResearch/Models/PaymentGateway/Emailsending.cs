using ExcellentMarketResearch.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace ExcellentMarketResearch.Models.PaymentGateway
{
    public class Emailsending
    {
        public bool SendEmail(string From, string DisplayName, string To, string CC, string Bcc, string Subject, string MailBody)
        {

            MailMessage msg = new MailMessage();
            bool result = false;
            //sending Mail 
            try
            {
                msg.From = new MailAddress(From, DisplayName);
                //if (To != "") msg.To.Add(To);

                string[] strTo = To.Split(','); if (To != "" && strTo.Length > 0) { for (int count = 0; count < strTo.Length; count++) msg.To.Add(strTo[count].Trim()); }
                string[] strCc = CC.Split(','); if (CC != "" && strCc.Length > 0) { for (int count = 0; count < strCc.Length; count++) msg.CC.Add(strCc[count].Trim()); }
                string[] strBcc = Bcc.Split(','); if (Bcc != "" && strBcc.Length > 0) { for (int count = 0; count < strBcc.Length; count++) msg.Bcc.Add(strBcc[count].Trim()); }
                msg.Priority = MailPriority.Normal;
                //msg.Subject = "Search_Radiology " + Subject;
                msg.Subject = Subject;
                msg.Body = MailBody;
                msg.BodyEncoding = System.Text.Encoding.UTF8;
                msg.SubjectEncoding = System.Text.Encoding.Default;
                msg.IsBodyHtml = true;
                SmtpClient client = new SmtpClient()
                {
                    Host = "smtp.excellentmarketresearch.com",
                    //  Host = "smtp.gmail.com",
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials=false,
                    // Credentials = new NetworkCredential("danielmiller@excellentmarketresearch.com", "Jmt?23r2"),
                    Credentials = new NetworkCredential("sales@excellentmarketresearch.com", "6xJ26Ac+d8=pc+g&"),
                    EnableSsl = false
                    //DeliveryMethod = SmtpDeliveryMethod.Network,
                    // UseDefaultCredentials = false,



                };
                client.Send(msg);//uncomment while uploading
                result = true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                result = false;
            }
            finally
            {
                msg.Dispose();
            }
            return result;
        }


        public string GenerateMailBody_PaymentInitiated(string ReportTitle, string Name, string EmailID, string ContactNo, string NameOfCompany, string Address, string State, string CountryName, string ZipCode)
        {
            string result = "";
            result = "Dear Admin, </br> Payment for report <b> " + ReportTitle + " </b> <br/>"
                    + "<table> <tr><td> <b> Customer Name - </b> </td><td> " + Name + " </td></tr>"
                        + "<tr><td><b> Email Id -</b> </td><td>" + EmailID + "</td></tr>"
                        + "<tr><td><b> Phone -</b></td><td> " + ContactNo + " </td></tr>"
                        + "<tr><td><b> Name of company -</b></td><td> " + NameOfCompany + " </td></tr>"
                        + "<tr><td><b>Address</b></td><td>" + Address + "</td></tr>"
                        + "<tr><td><b>State</b></td><td>" + State + "</td></tr>"
                        + "<tr><td><b>Country Name -</b></td><td>" + CountryName + " </td></tr>"
                        + "<tr><td>Zip Code<b></b></td><td>" + ZipCode + "</td></tr>"
                        + "<tr><td><b>IP Address -</b> </td><td>" + ExcellentMarketResearch.Models.PaymentGateway.IPAddress.GetIPAddress() + "</td></tr>"
                    + "</table>";
            return result;
        }

        public string GenerateMailBody_PaymentInitiated_AutoReply(string ReportTitle, string Name)
        {
            string result = "";
            result = "Dear " + Name + ","
                + "<br/><br/>Thank you for your interest in our research report " + ReportTitle + "."
                + "<br />Let us know in any problem while payment."
                + "<br/><br/>Thanks";
            return result;
        }

        public string GenerateMailBody_RequestSample_AutoReply(string Name, string ReportTitle)
        {
            string result = "";
            if (ReportTitle == "")
            {
                result = "Dear " + Name + ","
                     + "<br /><br />Thank you for your interest in <b>" + "ExcellentMarketResearch.com" + "</b>."
                     + "<br /><br />We'll contact you soon to serve your research needs."
                     + "<b><br /><br />Regards,"
                     + "<br /Robert Smith | Corporate Sales Specialist, USA"
                     + "<br />Direct line: +1-312-588-9716"
                     + "<br />" + "https://www.excellentmarketresearch.com"
                     + "<br />E-mail: sales@excellentmarketresearch.com | Web: " + "https://www.excellentmarketresearch.com" + "</b>";
            }
            else
            {
                result = "Dear " + Name + ","
                    + "<br /><br />Thank you for your interest in our research report, <b>" + ReportTitle + "</b>."
                    //+ "<br /><br />I will share the sample pages shortly."
                    //+ "<br /><br />For your reference please find the below link."
                    //+ "<br /><br />" + "hhj.biz".Substring(0, "jhjhg.biz".Length - 1) + ReportURL
                    + "<br /><br />We'll contact you soon to serve your research needs."
                    + "<b><br /><br />Regards,"
                     + "<br /Robert Smith  | Corporate Sales Specialist, USA"
                     + "<br />Direct line: +1-312-588-9716"
                     + "<br />" + "https://www.excellentmarketresearch.com"
                     + "<br />E-mail: sales@excellentmarketresearch.com | Web: " + "https://www.excellentmarketresearch.com" + "</b>";
            }
            return result;
        }

        public string GenerateMailBody_RequestSample(string reporttitle, string name, string emailid, string contactno, string nameofcompany, string countryname, string designation, string customermessage, string ipaddress)
        {
            string result = "";
            result = "dear admin,<br /><br />" + "<table>";
            result += reporttitle != "" ? "<tr> <td valign='top' width='30%'><b>report title</b></td>   <td valign='top' width='2%'><b> : </b></td> <td valign='top' width='68%'>" + reporttitle + "</td> </tr>" : "";
            result += name != "" ? "<tr> <td valign='top'><b>customer name</b></td>  <td valign='top'><b> : </b></td> <td valign='top'>" + name + "</td> </tr>" : "";
            result += emailid != "" ? "<tr> <td valign='top'><b>email id</b></td>       <td valign='top'><b> : </b></td> <td valign='top'>" + emailid + "</td> </tr>" : "";
            result += contactno != "" ? "<tr> <td valign='top'><b>phone</b></td>          <td valign='top'><b> : </b></td> <td valign='top'>" + contactno + "</td> </tr>" : "";
            result += nameofcompany != "" ? "<tr> <td valign='top'><b>company name</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + nameofcompany + "</td> </tr>" : "";
            result += designation != "" ? "<tr> <td valign='top'><b>designation</b></td>    <td valign='top'><b> : </b></td> <td valign='top'>" + designation + "</td> </tr>" : "";
            result += countryname != "" ? "<tr> <td valign='top'><b>country name</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + countryname + "</td> </tr>" : "";
            result += customermessage != "" ? "<tr> <td valign='top'><b>enquiry text</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + customermessage + "</td> </tr>" : "";
            result += "<tr> <td valign='top'><b>publisher</b></td>      <td valign='top'><b> : </b></td> <td valign='top'>" + "Excellent Market Research " + "</td> </tr>";
            result += "<tr> <td valign='top'><b>ip address</b></td>     <td valign='top'><b> : </b></td> <td valign='top'>" + ipaddress == null ? ExcellentMarketResearch.Models.PaymentGateway.IPAddress.GetIPAddress() : ipaddress + "</td> </tr>";
            result += "</table>";
            return result;
        }


        public string GenerateMailBody_PaymentMade(string PaymentSucess, BuyingVM userdata)
        {
            string result = "";
            //result = "Dear Admin, Payment made for <br /><br />" + "<table>";
            result = PaymentSucess + "<table>";
            result += userdata.ReportTitle != "" ? "<tr> <td valign='top' width='30%'><b>Report Title</b></td>   <td valign='top' width='2%'><b> : </b></td> <td valign='top' width='68%'>" + userdata.ReportTitle + "</td> </tr>" : "";
            result += userdata.Name != "" ? "<tr> <td valign='top'><b>Customer Name</b></td>  <td valign='top'><b> : </b></td> <td valign='top'>" + userdata.Name + "</td> </tr>" : "";
            result += userdata.EmailId != "" ? "<tr> <td valign='top'><b>Email ID</b></td>       <td valign='top'><b> : </b></td> <td valign='top'>" + userdata.EmailId + "</td> </tr>" : "";
            result += userdata.PhoneNumber != "" ? "<tr> <td valign='top'><b>Phone</b></td>          <td valign='top'><b> : </b></td> <td valign='top'>" + userdata.PhoneNumber + "</td> </tr>" : "";
            result += userdata.Company != "" ? "<tr> <td valign='top'><b>Company Name</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + userdata.Company + "</td> </tr>" : "";
            result += userdata.Designation != "" ? "<tr> <td valign='top'><b>Designation</b></td>    <td valign='top'><b> : </b></td> <td valign='top'>" + userdata.Designation + "</td> </tr>" : "";
            result += userdata.Country != "" ? "<tr> <td valign='top'><b>Country Name</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + userdata.Country + "</td> </tr>" : "";
            //result += txtComment.Text.Trim() != "" ? "<tr> <td valign='top'><b>Enquiry Text</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + txtComment.Text.Trim() + "</td> </tr>" : "";
            result += "<tr> <td valign='top'><b>IP Address</b></td>     <td valign='top'><b> : </b></td> <td valign='top'>" + userdata.IPAddress + "</td> </tr>";
            result += "</table>";
            return result;
        }

        public string GenerateMailBody_PaymentMade_AutoReply(string Name, string ReportTitle)
        {
            string result = "";
            result = "Dear " + Name + ","
                + "<br /><br />Thanks for payment for report, <b>" + ReportTitle + "</b>."
                + "<br /><br />Hope you like our service. We would like to service you again."
                + "<br /><br />Thanks,"
                + "<br />" + "www.excellentmarketresearch.com" + " | " + "www.excellentmarketresearch.com";
            return result;
        }

        //public string GenerateMailBody_PaypalError_AutoReply(string Name, string ReportTitle, string ReportURL)
        //{
        //    string result = "";
        //    result = "Dear " + Name + ","
        //        + "<br /><br />You canceled payment for report,"
        //        + "<br /><b>" + ReportTitle + "</b>."
        //        + "<br />" + "www.excellentmarketresearch.com" + "/report/" + ReportURL
        //        + "<br /><br />Did you experienced problem in our service?"
        //        + "<br /><br />Let us know."
        //        + "<b><br /><br />Warm regards,"
        //        + "<br />Robert Smith | Corporate Sales Specialist,USA"
        //        + "<br />Direct line: + 1-855-465-4651"
        //        + "<br />Excellent Market Research"
        //        + "<br />E-mail: joel@9dresearchgroup.com | Web: " + "www.excellentmarketresearch.com" + "</b>"
        //        + "<br /><br />Thanks,"
        //        + "<br />Excellent Market Research ";
        //    return result;
        //}
        public string GenerateMailBody_PaypalError_AutoReply(string Name, string ReportTitle, string ReportURL)
        {
            string result = "";

            result = "Dear " + Name + ","
                + "<br /><br />You canceled payment for report,"
                + "<br /><b>" + ReportTitle + "</b>."
                + "<br />" + "https://www.excellentmarketresearch.com/report/" + ReportURL
                + "<br /><br />Did you experienced problem in our service?"
                + "<br /><br />Let us know."
                + "<b><br /><br />Warm regards,"
                + "<br />Robert Smith | Corporate Sales Specialist,USA"
                + "<br />Direct line: +1-312-588-9716"
                + "<br />excellentmarketresearch"
                + "<br />E-mail: sales@excellentmarketresearch.com | Web: " + "https://www.excellentmarketresearch.com" + "</b>"
                + "<br /><br />Thanks,"
                + "<br />www.excellentmarketresearch.com";

            return result;
        }
    }
}