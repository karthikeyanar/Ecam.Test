using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Mail;
using System.Linq;
using Ecam.Framework;
using Newtonsoft.Json;
using System.IO;

namespace Ecam.Framework {


    public class SMTPMailHelper {

        //public string CompanyLogo { get; set; }
        //public string AirlineLogo { get; set; }
        public List<List<string>> ToEmails { get; set; }
        public List<List<string>> CCEmails { get; set; }
        public List<List<string>> BCCEmails { get; set; }

        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string Subject { get; set; }
        public string EmailBody { get; set; }
        public bool IsBodyHtml { get; set; }
        public string ServerName { get; set; }
        public int ServerPort { get; set; }
        public bool IsSSL { get; set; }
        public bool IsUseDefaultCredentials { get; set; }
        public string ServerUserName { get; set; }
        public string ServerPassword { get; set; }

        public bool IsNoHeaderAndFooter { get; set; }

        public List<ReplaceField> ReplaceFields { get; set; }

        public List<EmailAttachment> Attachments { get; set; }

        public MailPriority Priority { get; set; }


        public List<ErrorInfo> Errors { get; set; }

        public SMTPMailHelper() {
            this.Priority = MailPriority.High;
            this.ToEmails = new List<List<string>>();
            this.CCEmails = new List<List<string>>();
            this.BCCEmails = new List<List<string>>();
            this.ReplaceFields = new List<ReplaceField>();
            this.Attachments = new List<EmailAttachment>();
            this.IsBodyHtml = true;
            this.Errors = new List<ErrorInfo>();
        }

        public bool Send() {
            bool isSend = false;
            MailMessage msg = new MailMessage();
            try {

                //Helper.Log("**************** Mail Send Start ***********************");
                //Helper.Log(string.Format("Model = {0}",JsonConvert.SerializeObject(model)));
                StringBuilder header = new StringBuilder();
                StringBuilder footer = new StringBuilder();
                string headerImage = string.Format("{0}/img/emailtemplateheader.gif",Helper.ServerURL);
                string footerImage = string.Format("{0}/img/emailtemplatefooter.gif",Helper.ServerURL);

                header.Append(string.Format("<!DOCTYPE html><html><head><title>{0}</title></head>",Helper.ProjectName));
                header.Append("<body style='font-family:Helvetica, Arial, sans-serif;font-size:13px;line-height:2;color:#333;background-color:#fff;'>");
                footer.Append("</body></html>");

                if(string.IsNullOrEmpty(this.EmailBody) == false) {
 
                    if(this.ReplaceFields != null) {
                        foreach(var mergeField in this.ReplaceFields) {
                            this.EmailBody = this.EmailBody.RegExReplace("{{" + mergeField.field_name + "}}",mergeField.value);
                            this.Subject = this.Subject.RegExReplace("{{" + mergeField.field_name + "}}",mergeField.value);
                        }
                    }
                      
                    msg.From = new MailAddress(this.FromEmail.ToString().Trim(),this.FromName.ToString().Trim());

                    string sendEmailAddress = string.Empty;
                    string sendDisplayName = string.Empty;
                    // To addresses
                    foreach(List<string> email in this.ToEmails) {
                        sendEmailAddress = string.Empty;
                        sendDisplayName = string.Empty;
                        if(email.Count() > 0)
                            sendEmailAddress = email[0];
                        if(email.Count() > 1)
                            sendDisplayName = email[1];

                        if(string.IsNullOrEmpty(sendEmailAddress) == false) {
                            //Helper.Log(string.Format("To Email Address = {0}, Display Name = {1}",sendEmailAddress,sendDisplayName));
                            msg.To.Add(new MailAddress(sendEmailAddress.ToString().Trim(),sendDisplayName.ToString().Trim()));
                        }
                    }

                    // CC addresses
                    foreach(List<string> email in this.CCEmails) {
                        sendEmailAddress = string.Empty;
                        sendDisplayName = string.Empty;
                        if(email.Count() > 0)
                            sendEmailAddress = email[0].ToString().Trim();
                        if(email.Count() > 1)
                            sendDisplayName = email[1].ToString().Trim();

                        if(string.IsNullOrEmpty(sendEmailAddress) == false) {
                            //Helper.Log(string.Format("CC Email Address = {0}, Display Name = {1}",sendEmailAddress,sendDisplayName));
                            msg.CC.Add(new MailAddress(sendEmailAddress.ToString().Trim(),sendDisplayName.ToString().Trim()));
                        }
                    }

                    // BCC addresses
                    foreach(List<string> email in this.BCCEmails) {
                        sendEmailAddress = string.Empty;
                        sendDisplayName = string.Empty;
                        if(email.Count() > 0)
                            sendEmailAddress = email[0].ToString().Trim();
                        if(email.Count() > 1)
                            sendDisplayName = email[1].ToString().Trim();

                        if(string.IsNullOrEmpty(sendEmailAddress) == false) {
                            //Helper.Log(string.Format("Bcc Email Address = {0}, Display Name = {1}",sendEmailAddress,sendDisplayName));
                            msg.Bcc.Add(new MailAddress(sendEmailAddress.ToString().Trim(),sendDisplayName.ToString().Trim()));
                        }
                    }

                    // Set to high priority
                    msg.Priority = this.Priority;

                    msg.Subject = this.Subject;

                    // You can specify a plain text or HTML contents
                    if(this.IsNoHeaderAndFooter == false) {
                        this.EmailBody = header.ToString() + this.EmailBody + footer.ToString();
                    }
                    msg.Body = this.EmailBody;
                    // In order for the mail client to interpret message
                    // body correctly, we mark the body as HTML
                    // because we set the body to HTML contents.

                    msg.IsBodyHtml = this.IsBodyHtml;

                    // Attaching some data
                    foreach(var att in this.Attachments) {
                        if(string.IsNullOrEmpty(att.MediaType) == true) {
                            msg.Attachments.Add(new Attachment(att.ContentStream,att.Name));
                        } else {
                            msg.Attachments.Add(new Attachment(att.ContentStream,att.Name,att.MediaType));
                        }
                    }

                    // Connecting to the server and configuring it
                    SmtpClient client = new SmtpClient();
                    client.Host = this.ServerName; // "smtp.gmail.com";
                    client.Port = this.ServerPort; // (smtpSetting.port ?? 0); // 578;

                    if(this.IsSSL == true) {
                        client.EnableSsl = true;
                    }
                    // The server requires user's credentials
                    // not the default credentials
                    client.UseDefaultCredentials = this.IsUseDefaultCredentials;
                    // Provide your credentials
                    client.Credentials = new System.Net.NetworkCredential(this.ServerUserName,this.ServerPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Use SendAsync to send the message asynchronously
                    if(Helper.IsLocal == "true") {                        
                        Helper.WriteEmail(msg.Body);
                    } else {
                        client.Send(msg);
                    }
                    //Helper.Log("Email Send Complete");
                    isSend = true;
                }
            } catch(Exception ex) {
                string errorMessage = "Email Send Exception:" + ex.Message + Environment.NewLine;                        
                if(ex.InnerException != null)
                {
                    errorMessage = "Email Send INNER Exception :" + ex.InnerException.Message + Environment.NewLine;
                }
                Helper.Log(errorMessage);
                this.Errors.Add(new ErrorInfo
                { 
                    PropertyName = "Exceptions",
                    ErrorMessage = errorMessage
                });
            } finally {
                //Helper.Log("**************** Mail Send End ***********************");
                msg.Dispose();
            }
            return isSend;
        }
    }


    public class EmailAttachment {
        public Stream ContentStream { get; set; }
        public string Name { get; set; }
        public string MediaType { get; set; }
    }


    public class ReplaceField {

        public string field_name { get; set; }

        public string field_display_name { get; set; }

        public string value { get; set; }
    }
}