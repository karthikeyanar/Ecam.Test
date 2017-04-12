using Ecam.Contracts;
using Ecam.Models;
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
using System.Web;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Data.Entity.Validation;

namespace Ecam.Models.Helpers {

    public class EmailModel {

        public EmailModel() {
            this.Priority = MailPriority.High;
            this.ToEmails = new List<List<string>>();
            this.CCEmails = new List<List<string>>();
            this.BCCEmails = new List<List<string>>();
            this.Attachments = new List<string>();
        }

        public List<List<string>> ToEmails { get; set; }
        public List<List<string>> CCEmails { get; set; }
        public List<List<string>> BCCEmails { get; set; }

        public string FromName { get; set; }


        public string EmailTemplateName { get; set; }

        public List<string> Attachments { get; set; }

        public MailPriority Priority { get; set; }

        public int CompanyID { get; set; }

        public string AirlineCode { get; set; }
    }

    public class EmailHelper {

             
    }
}