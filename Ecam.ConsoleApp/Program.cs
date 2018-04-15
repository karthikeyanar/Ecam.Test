using CsvHelper;
using Ecam.Framework;
using Ecam.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Ecam.ConsoleApp
{
    class Program
    {
        public static string IS_DOWNLOAD_HISTORY = "";
        public static string IS_BOOK_MARK_CATEGORY = "";
        public static string IS_IMPORT_CSV = "";
        public static string IS_NIFTY_FLAG_CSV = "";
        public static string IS_CATEGORY_FLAG_CSV = "";
        public static string GOOGLE_DATA = "";
        public static string MC = "";
        public static string MONEY_CONTROL = "";
        public static string START_TIME = "";
        public static string END_TIME = "";
        private static int _INDEX = -1;
        private static string[] _COMPANIES;
        private static List<string> _URLS;
        private static List<string> _SYMBOLS_LIST;
        static void Main(string[] args)
        {
            try {
                IS_CATEGORY_FLAG_CSV = System.Configuration.ConfigurationManager.AppSettings["IS_CATEGORY_FLAG_CSV"];
                IS_NIFTY_FLAG_CSV = System.Configuration.ConfigurationManager.AppSettings["IS_NIFTY_FLAG_CSV"];
                IS_IMPORT_CSV = System.Configuration.ConfigurationManager.AppSettings["IS_IMPORT_CSV"];
                IS_DOWNLOAD_HISTORY = System.Configuration.ConfigurationManager.AppSettings["IS_DOWNLOAD_HISTORY"];
                IS_BOOK_MARK_CATEGORY = System.Configuration.ConfigurationManager.AppSettings["IS_BOOK_MARK_CATEGORY"];
                MC = System.Configuration.ConfigurationManager.AppSettings["MC"];
                GOOGLE_DATA = System.Configuration.ConfigurationManager.AppSettings["GOOGLE_DATA"];
                MONEY_CONTROL = System.Configuration.ConfigurationManager.AppSettings["MONEY_CONTROL"];
                START_TIME = System.Configuration.ConfigurationManager.AppSettings["START_TIME"];
                END_TIME = System.Configuration.ConfigurationManager.AppSettings["END_TIME"];

                DateTime eveningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " " + START_TIME);
                DateTime eveningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " " + END_TIME);
                DateTime now = DateTime.Now;
                bool isDontStart = false;
                ScriptGenerate();
                if(IS_DOWNLOAD_HISTORY != "true" && IS_IMPORT_CSV != "true" && IS_NIFTY_FLAG_CSV != "true" && IS_CATEGORY_FLAG_CSV != "true") {
                    if((now >= eveningStart && now <= eveningEnd) == false) {
                        string GOOGLE_DATA = System.Configuration.ConfigurationManager.AppSettings["GOOGLE_DATA"];
                        string[] files = System.IO.Directory.GetFiles(GOOGLE_DATA);
                        foreach(string fileName in files) {
                            System.IO.FileInfo fileInfo = new FileInfo(fileName);
                            if(fileInfo.CreationTime < eveningStart) {
                                System.IO.File.Delete(fileName);
                            }
                        }
                        isDontStart = true;
                    } else {
                        string GOOGLE_DATA = System.Configuration.ConfigurationManager.AppSettings["GOOGLE_DATA"];
                        string[] files = System.IO.Directory.GetFiles(GOOGLE_DATA);
                        foreach(string fileName in files) {
                            System.IO.FileInfo fileInfo = new FileInfo(fileName);
                            if(fileInfo.CreationTime < eveningStart) {
                                System.IO.File.Delete(fileName);
                            }
                        }
                    }
                }
                if(isDontStart == false) {
                    try {
                        DownloadStart();
                    } catch(Exception ex) {
                        Helper.Log(ex.Message,"DOWNLOAD_START_ERROR" + "_" + (new Random()).Next(1000,10000));
                    }
                    try {
                        int i;
                        for(i=0;i<1;i++) {
                            DateTime startDate = Convert.ToDateTime("01/01/" +  (DateTime.Now.Year - i).ToString());
                            DateTime endDate = Convert.ToDateTime("12/31/" +  (DateTime.Now.Year - i).ToString());
                            AddSplit(startDate,endDate);
                        }
                    } catch(Exception ex) {
                        Helper.Log(ex.Message,"AddSplit_ERROR" + "_" + (new Random()).Next(1000,10000));
                    }
                    try {
                        //int i;
                        //int total = (365*15);
                        ////if(IS_DOWNLOAD_HISTORY == "true" || IS_IMPORT_CSV == "true") {
                        ////    tra_daily_log dailyLog = null;
                        ////    using(EcamContext context = new EcamContext()) {
                        ////        dailyLog = (from q in context.tra_daily_log
                        ////                    orderby q.trade_date descending
                        ////                    select q).FirstOrDefault();
                        ////    }
                        ////    if(dailyLog != null) {
                        ////        TimeSpan ts = DateTime.Now.Date - dailyLog.trade_date;
                        ////        total = (int)ts.TotalDays + 1;
                        ////    }
                        ////    total = 30;
                        ////}
                        //for(i = 0;i < total;i++) {
                        //    DateTime dt = DateTime.Now.Date.AddDays(-i);
                        //    TradeHelper.CreateDailyLog(dt.Date,"");
                        //    TradeHelper.CreateDailyLog(dt.Date,"true");
                        //}
                        //MailSend(true);
                        //MailSend(false);
                        MailSendDailyCSV();
                    } catch(Exception ex) {
                        Helper.Log(ex.Message,"MAIL_SEND_ERROR" + "_" + (new Random()).Next(1000,10000));
                    }
                }
            } catch(Exception ex) {
                Helper.Log(ex.Message,"MAIN_ERROR" + "_" + (new Random()).Next(1000,10000));
            }
        }

        private static void ScriptGenerate() {
            List<tra_company> companies;
            using (EcamContext context = new EcamContext())
            {
                IQueryable<tra_company> query = context.tra_company;

                DateTime morningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 9:15AM");
                DateTime morningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 10:15AM");
                DateTime eveningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 3:31PM");
                DateTime eveningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 11:59PM");
                DateTime now = DateTime.Now;
                DateTime targetTime = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 3:30PM");

                if ((now >= morningStart && now <= eveningStart))
                {
                    //query = (from q in query
                    //         join h in context.tra_holding on q.symbol equals h.symbol
                    //         select q);
                    //string IS_NIFTY_50 = System.Configuration.ConfigurationManager.AppSettings["IS_NIFTY_50"];
                    //string IS_NIFTY_100 = System.Configuration.ConfigurationManager.AppSettings["IS_NIFTY_100"];
                    //string IS_NIFTY_200 = System.Configuration.ConfigurationManager.AppSettings["IS_NIFTY_200"];
                    //if (IS_NIFTY_50 == "true")
                    //{
                    //    query = query.Where(q => (q.is_nifty_50 ?? false) == true);
                    //}
                    //if (IS_NIFTY_100 == "true")
                    //{
                    //    query = query.Where(q => (q.is_nifty_100 ?? false) == true);
                    //}
                    //if (IS_NIFTY_200 == "true")
                    //{
                    //    query = query.Where(q => (q.is_nifty_200 ?? false) == true);
                    //}
                }
                if(IS_BOOK_MARK_CATEGORY == "true") {
                    List<tra_company_category> companyCategories = (from q in context.tra_company_category
                                                                    join c in context.tra_category on q.category_name equals c.category_name
                                                                    where (c.is_book_mark ?? false) == true
                                                                    select q).ToList();
                    List<string> categorySymbols = (from q in companyCategories
                                                    select q.symbol).Distinct().ToList();
                    query = (from q in query where categorySymbols.Contains(q.symbol) == true select q);
                }
                companies = (from q in query
                             orderby q.symbol ascending
                             select q).ToList();
            }

            var symbols = "";
            if(companies != null) {
                foreach(var company in companies) {
                    string type = "EQ";
                    //switch(company.symbol) {
                    //    case "EIFFL":
                    //    case "SIL":
                    //    case "SITASHREE":
                    //    case "SKML":
                    //    case "VSCL":
                    //        type = "ALL";
                    //        break;
                    //}
                    symbols += "{'symbol':'"+company.symbol+"','type':'"+type+"'},";
                }
            }
            if(string.IsNullOrEmpty(symbols)==false) {
                symbols = symbols.Substring(0,symbols.Length-1);
            } 
            string script = System.IO.File.ReadAllText("E:\\Projects\\Ecam.Test2\\Ecam.Test\\ORI_CSV_SCRIPT.txt");
            script = script.Replace("{{SYMBOLS}}",symbols);
            string p = "";
            p += string.Format("p.data_type='{0}';",System.Configuration.ConfigurationManager.AppSettings["DATA_TYPE"]);
            p += string.Format("p.date_range='{0}';",System.Configuration.ConfigurationManager.AppSettings["DATA_RANGE"]);
            p += string.Format("p.period_type='{0}';",System.Configuration.ConfigurationManager.AppSettings["PERIOD_TYPE"]);
            p += string.Format("p.from_date='{0}';",System.Configuration.ConfigurationManager.AppSettings["FROM_DATE"]);
            p += string.Format("p.to_date='{0}';",System.Configuration.ConfigurationManager.AppSettings["TO_DATE"]);
            script = script.Replace("{{PARAMS}}",p);
            System.IO.File.WriteAllText("E:\\Projects\\Ecam.Test2\\Ecam.Test\\CSV_SCRIPT.txt",script);
        }

        private static void MailSend(bool isBookMark) {
            try {
                var fromAddress = new MailAddress("priyatradevnr@gmail.com","Priya");
                string fromPassword = "priyakarthi333";

                MailMessage msg = new MailMessage();
                msg.From = fromAddress;
                msg.To.Add(new MailAddress("karthikeyanar@gmail.com","Karthi"));
                msg.CC.Add(new MailAddress("priyatradevnr@gmail.com","Priya"));
                msg.Priority = MailPriority.High;
                msg.Subject = "Daily Summary" + (isBookMark == false ? "_All" : "")  + ": " + (new Random()).Next(1000,100000) + "_" + DateTime.Now.ToString("dd_MMM_yyyy");
                
                string sql = string.Format(" select log.log_id,log.trade_date as trade_date,log.positive,log.negative " + Environment.NewLine +
                             ",if((log.positive > log.negative),'True','') as indicator " + Environment.NewLine +
                             //",(log.positive - log.negative) as diff " + Environment.NewLine +
                             ",((((log.positive - (log.positive + log.negative)) / (log.positive + log.negative))) * 100) * -1 as positive_percentage " + Environment.NewLine +
                             ",((((log.negative - (log.positive + log.negative)) / (log.positive + log.negative))) * 100) * -1 as negative_percentage " + Environment.NewLine +
                             ",(log.positive + log.negative) as total " + Environment.NewLine +
                             " from tra_daily_log log where log.trade_date >= '{0}' and log.trade_date <= '{1}' and ifnull(log.is_book_mark,0) = {2} order by log.trade_date desc limit 0,60 ",DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd"),DateTime.Now.ToString("yyyy-MM-dd"),(isBookMark == true ? 1 : 0));

                DataTable dt = GetDataTable(sql);

                List<Ecam.Framework.CSVColumn> columnFormats = new List<Ecam.Framework.CSVColumn>();
                string htmlBody = Ecam.Framework.CSVHelper.CreateHTMLTableFromGenericList_ByDataTable(dt,columnFormats);

                msg.Body = htmlBody;
                msg.IsBodyHtml = true;

                MemoryStream stream = null;
                string tempFileName = "";
                if(File.Exists(tempFileName) == true) {
                    byte[] bytes = System.IO.File.ReadAllBytes(tempFileName);
                    stream = new MemoryStream(bytes);
                }
                if(stream != null) {
                    msg.Attachments.Add(new Attachment(stream,Path.GetFileName(tempFileName)));
                }
                var smtp = new SmtpClient {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address,fromPassword),
                    Timeout = 20000
                };
                using(msg) {
                    smtp.Send(msg);
                }
            } catch(Exception ex) {
                Helper.Log("Mail Send Exception=" + ex.Message,"MAIL_SEND_" + (new Random()).Next(1000,10000));
                if(ex.InnerException != null) {
                    Helper.Log("Mail Send InnerException=" + ex.Message,"MAIL_SEND_" + (new Random()).Next(1000,10000));
                }
            }
        }

        private static void MailSendDailyCSV() {
            try {
                var fromAddress = new MailAddress("priyatradevnr@gmail.com","Priya");
                string fromPassword = "priyakarthi333";
                int rowSize = 10;
                string FILES_PATH = System.Configuration.ConfigurationManager.AppSettings["FilesPath"];
                MailMessage msg = new MailMessage();
                msg.From = fromAddress;
                msg.To.Add(new MailAddress("karthikeyanar@gmail.com","Karthi"));
                msg.CC.Add(new MailAddress("priyatradevnr@gmail.com","Priya"));
                msg.Priority = MailPriority.High;
                msg.Subject = "Daily CSV_" + rowSize + "_" + (new Random()).Next(1000,100000) + "_" + DateTime.Now.ToString("dd_MMM_yyyy");

                msg.Body = "Attachments";
                msg.IsBodyHtml = true;

                List<Ecam.Framework.CSVColumn> columnFormats = new List<Ecam.Framework.CSVColumn>();
                columnFormats.Add(new Ecam.Framework.CSVColumn { PropertyName = "diff",IsIgNore = true });
                columnFormats.Add(new Ecam.Framework.CSVColumn { PropertyName = "percentage_high",IsIgNore = true });
                columnFormats.Add(new Ecam.Framework.CSVColumn { PropertyName = "percentage_low",IsIgNore = true });
                int months = 144;
                int i;
                List<DailySummary> dailyList = new List<DailySummary>();
                for(i = 0;i < months;i++) {
                    DateTime dt = DateTime.Now.Date.AddMonths(-i);
                    DateTime monthStartDate = DataTypeHelper.GetFirstDayOfMonth(dt);
                    DateTime monthLastDate = DataTypeHelper.GetLastDayOfMonth(dt);
                    DateTime totalEndDate = DataTypeHelper.GetLastDayOfMonth(monthStartDate.AddMonths(-1).AddDays(7));
                    DateTime totalStartDate = DataTypeHelper.GetFirstDayOfMonth(totalEndDate.AddMonths(-6).AddDays(7));
                   

                    Console.WriteLine("monthStartDate=" + monthStartDate.ToString("dd/MMM/yyyy"));
                    Console.WriteLine("monthLastDate=" + monthLastDate.ToString("dd/MMM/yyyy"));
                    Console.WriteLine("totalStartDate=" + totalStartDate.ToString("dd/MMM/yyyy"));
                    Console.WriteLine("totalEndDate=" + totalEndDate.ToString("dd/MMM/yyyy"));

                    List<DailySummary> templist = Ecam.Models.Common.GetDailySummary(new Contracts.TRA_COMPANY_SEARCH {
                        categories = "",
                        end_date = monthLastDate,
                        is_book_mark_category = true,
                        monthly_investment = 20000,
                        start_date = monthStartDate,
                        total_amount = 1000000,
                        total_end_date = totalEndDate,
                        total_start_date = totalStartDate,
                    }, new Paging {
                        PageIndex = 1,
                        PageSize = rowSize,
                        SortName = "total_profit",
                        SortOrder = "desc",
                    },false);
                    if(templist != null) {
                        dailyList = dailyList.Concat(templist).ToList();
                    }
                }

                dailyList = (from q in dailyList
                             orderby q.date descending
                             select q).ToList();

                string csv = Ecam.Framework.CSVHelper.CreateCSVFromGenericList(dailyList,columnFormats);
                string tempFileName = "DailyCSV_"+ rowSize + "_"+(new Random()).Next(1000,100000)+"_"+DateTime.Now.ToString("dd_MMM_yyyy")+".csv";

                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(csv ?? ""));

                //if(File.Exists(tempFileName) == true) {
                //    byte[] bytes = System.IO.File.ReadAllBytes(tempFileName);
                //    stream = new MemoryStream(bytes);
                //}
                if(stream != null) {
                    msg.Attachments.Add(new Attachment(stream,Path.GetFileName(tempFileName)));

                }
                var smtp = new SmtpClient {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address,fromPassword),
                    Timeout = 20000
                };
                using(msg) {
                    smtp.Send(msg);
                }
            } catch(Exception ex) {
                Helper.Log("Mail Send Exception=" + ex.Message,"MAIL_SEND_" + (new Random()).Next(1000,10000));
                if(ex.InnerException != null) {
                    Helper.Log("Mail Send InnerException=" + ex.Message,"MAIL_SEND_" + (new Random()).Next(1000,10000));
                }
            }
        }

        private static DataTable GetDataTable(string sql) {
            DataTable dt = null;
            bool isUpdatePermission = false;
            string key = ""; // DataTypeHelper.ConvertString(HttpContext.Current.Request["key"]);
            if(key == "C3BB96E9C96") {
                isUpdatePermission = true;
            }
            SLExcelData excelData = new SLExcelData();
            sql = sql.Replace(Environment.NewLine,"").Trim();
            if(string.IsNullOrEmpty(sql) == false) {
                if(sql.ToLower().Contains("update ") == false
                    && sql.ToLower().Contains("delete ") == false
                    && sql.ToLower().Contains("create ") == false
                    && sql.ToLower().Contains("insert ") == false
                    && sql.ToLower().Contains("alter ") == false
                    ) {
                    if(sql.ToLower().Contains("limit") == false)
                        sql = sql + " limit 0,10";
                } else {
                    if(isUpdatePermission == false)
                        throw new Exception("Create,Update,Delete,Insert,Alter SQL Command Not supported.");
                }

                MySqlDataReader reader = MySqlHelper.ExecuteReader(Ecam.Framework.Helper.ConnectionString,sql);
                dt = new DataTable();
                dt.Load(reader);
            }
            return dt;
        }

        #region MoneyControl

        private static void DoMoneyControl()
        {
            _URLS = GetURLList();
            MoneyControlDownloadStart();
        }

        private static List<string> GetURLList()
        {
            List<string> urls = new List<string>();
            Regex regex = null;
            if (string.IsNullOrEmpty(MONEY_CONTROL) == false)
            {
                string[] files = System.IO.Directory.GetFiles(MONEY_CONTROL);
                foreach (string fileName in files)
                {
                    string html = System.IO.File.ReadAllText(fileName);
                    html = html.Replace("\n", "").Replace("\r", "").Replace("\r\n", "");
                    regex = new Regex(
                                            @"<tr(.*?)>(.*?)</tr>",
                                            RegexOptions.IgnoreCase
                                            | RegexOptions.Multiline
                                            | RegexOptions.IgnorePatternWhitespace
                                            | RegexOptions.Compiled
                                            );
                    MatchCollection trCollections = regex.Matches(html);
                    int i, j;
                    for (i = 0; i < trCollections.Count; i++)
                    {
                        Match trMatch = trCollections[i];
                        string tr = trMatch.Value;
                        regex = new Regex(
                                        @"<td(.*?)>(.*?)</td>",
                                        RegexOptions.IgnoreCase
                                        | RegexOptions.Multiline
                                        | RegexOptions.IgnorePatternWhitespace
                                        | RegexOptions.Compiled
                                        );
                        MatchCollection tdCollections = regex.Matches(tr);
                        RowCollections row = new RowCollections();
                        for (j = 0; j < tdCollections.Count; j++)
                        {
                            string v = tdCollections[j].Groups[2].Value;
                            if (string.IsNullOrEmpty(v) == false)
                            {
                                if (v.Contains("<a") == true)
                                {
                                    regex = new Regex(
                                                @"(\S+)=[""']?((?:.(?![""']?\s+(?:\S+)=|[>""']))+.)[""']?",
                                                RegexOptions.IgnoreCase
                                                | RegexOptions.Multiline
                                                | RegexOptions.IgnorePatternWhitespace
                                                | RegexOptions.Compiled
                                                );
                                    MatchCollection attrCollections = regex.Matches(v);
                                    if (attrCollections.Count > 0)
                                    {
                                        if (attrCollections[0].Groups.Count >= 3)
                                        {
                                            if (attrCollections[0].Groups[1].Value == "href")
                                            {
                                                if (string.IsNullOrEmpty(attrCollections[0].Groups[2].Value) == false)
                                                {
                                                    string url = attrCollections[0].Groups[2].Value;
                                                    if (attrCollections[0].Groups[2].Value.Contains("moneycontrol.com") == false)
                                                    {
                                                        url = "http://www.moneycontrol.com" + attrCollections[0].Groups[2].Value;
                                                    }
                                                    if (urls.Contains(url) == false)
                                                    {
                                                        urls.Add(url);
                                                    }
                                                }
                                                //Helper.Log(url + Environment.NewLine);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return urls;
        }

        #endregion

        private static void UpdatePrevPriceEquity(string symbol) {
            using(EcamContext context = new EcamContext()) {
                List<tra_market> markets = (from q in context.tra_market where q.symbol == symbol orderby q.trade_date descending select q).ToList();
                int total = markets.Count();
                int i = 0;
                foreach(var market in markets) {
                    i += 1;
                    tra_market prevMarket = (from q in context.tra_market
                                      where q.symbol == market.symbol
                                      && q.trade_date < market.trade_date
                                             orderby q.trade_date descending
                                      select q).FirstOrDefault();
                    if(prevMarket != null) {
                        market.prev_price = prevMarket.close_price;
                        context.Entry(market).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                    Console.WriteLine("Symbol=" + symbol + ",Total=" + total + ",i=" + i);
                }
            } 
        }

        private static void AddSplit(DateTime startDate, DateTime endDate)
        {
            string sql = "select " + Environment.NewLine +
                            " m.symbol" + Environment.NewLine +
                            " ,DATE_FORMAT(m.trade_date,'%d/%b/%Y') as trade_date" + Environment.NewLine +
                            " ,(((ifnull(m.prev_price,0)/ifnull(m.open_price,0)))) as prev_diff" + Environment.NewLine +
                            " ,(((ifnull(m.open_price,0)-ifnull(m.prev_price,0))/ifnull(m.prev_price,0)) *100) as prev_percentage" + Environment.NewLine +
                            " ,m.symbol" + Environment.NewLine +
                            " ,m.trade_date as trade_date2" + Environment.NewLine +
                            //" ,m.open_price" + Environment.NewLine +
                            //" ,m.prev_price" + Environment.NewLine +
                            //" ,m.* " + Environment.NewLine +
                            " from tra_market m" + Environment.NewLine +
                            " where m.trade_date>='" + startDate.ToString("yyyy-MM-dd") + "' and m.trade_date<='" + endDate.ToString("yyyy-MM-dd") + "'" + Environment.NewLine +
                            " and (((ifnull(m.ltp_price,0) - ifnull(m.prev_price,0)) / ifnull(m.prev_price,0))*100) != 0" + Environment.NewLine +
                            " and(((ifnull(m.ltp_price,0) - ifnull(m.prev_price,0)) / ifnull(m.prev_price,0))*100) <= -48" + Environment.NewLine +
                            " order by trade_date asc,symbol asc limit 0,100";
            using (MySqlDataReader dr = MySqlHelper.ExecuteReader(Ecam.Framework.Helper.ConnectionString, sql))
            {
                while (dr.Read())
                {
                    string symbol = dr["symbol"].ToString();
                    DateTime tradeDate = DataTypeHelper.ToDateTime(dr["trade_date"]);
                    decimal factor = DataTypeHelper.ToDecimal(dr["prev_diff"].ToString());
                    tra_split split = null;

                    using (EcamContext context = new EcamContext())
                    {
                        split = (from q in context.tra_split
                                 where q.symbol == symbol
                                 && q.split_date == tradeDate
                                 select q).FirstOrDefault();
                        bool isNew = false;
                        if (split == null)
                        {
                            split = new tra_split();
                            isNew = true;
                        }
                        else
                        {
                            split._prev_split = (from q in context.tra_split
                                                 where
                                                 q.id == split.id
                                                 select q).FirstOrDefault();
                        }
                        if(isNew==true) {
                            split.symbol = symbol;
                            split.split_date = tradeDate;
                            split.split_factor = factor;
                            if (split.id > 0)
                            {
                                context.Entry(split).State = System.Data.Entity.EntityState.Modified;
                            }
                            else
                            {
                                context.tra_split.Add(split);
                            }
                            context.SaveChanges();
                        }
                    }
                    if (split != null)
                    {
                        split.UpdatePrice();
                        Console.WriteLine("Split Completed symbol=" + split.symbol + ",Date=" + split.split_date);
                    }
                }
            }
        }

        private static void UpdatePrevPrice()
        {

            GoogleHistoryDownloadData history = new GoogleHistoryDownloadData();
            List<tra_company> companies;
            using (EcamContext context = new EcamContext())
            {
                companies = (from q in context.tra_company
                             orderby q.symbol ascending
                             select q).ToList();
            }
            foreach (var company in companies)
            {
                List<TempClass> tempList;
                DateTime today = DateTime.Now.Date;
                DateTime monthStartDate = DataTypeHelper.GetFirstDayOfMonth(today);
                DateTime monthEndDate = DataTypeHelper.GetLastDayOfMonth(today);
                using (EcamContext context = new EcamContext())
                {
                    tempList = (from q in context.tra_market
                                where q.symbol == company.symbol
                                && q.trade_date >= monthStartDate
                                && q.trade_date <= monthEndDate
                                orderby q.symbol ascending
                                select new TempClass
                                {
                                    symbol = company.symbol,
                                    trade_date = q.trade_date,
                                    close_price = q.close_price,
                                    high_price = q.high_price,
                                    low_price = q.low_price,
                                    open_price = q.open_price,
                                    ltp_price = q.ltp_price,
                                    //prev_price = DataTypeHelper.ToDecimal(prev),
                                }).ToList();
                }
                tempList = (from q in tempList orderby q.trade_date ascending select q).ToList();
                int rowIndex = 0;
                foreach (var temprow in tempList)
                {
                    rowIndex += 1;
                    TradeHelper.ImportPrice(temprow);
                    Console.WriteLine("Symbol=" + temprow.symbol + ",Date=" + temprow.trade_date.ToString("dd/MM/yyyy") + " Completed");
                }
                //GoogleHistoryDownloadData.CalculateRSI(company.symbol);
            }
        }

        private static void DownloadStart()
        {
            using(EcamContext context = new EcamContext()) {
                _SYMBOLS_LIST = (from q in context.tra_company
                                 select q.symbol).ToList();
            }
            //Helper.Log("DownloadStart=" + DateTime.Now.ToString(), "DOWNLOAD");
            DateTime morningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 9:15AM");
            DateTime morningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 10:15AM");
            DateTime eveningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 3:31PM");
            DateTime eveningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 11:59PM");
            DateTime now = DateTime.Now;
            if (IS_DOWNLOAD_HISTORY == "true")
            {
                GoogleHistoryData();
            }
            else if (IS_IMPORT_CSV == "true")
            {
                CSVData();
            }
            else if (IS_NIFTY_FLAG_CSV == "true")
            {
                UpdateNiftyFlagCSV();
            }
            else if (IS_CATEGORY_FLAG_CSV == "true")
            {
                UpdateCategoryFlagCSV();
            }
            else
            {
                //if ((now >= morningStart && now <= morningEnd) || (now >= eveningStart && now <= eveningEnd))
                //{
                GoogleData();
                Console.WriteLine("Completed");
                //Helper.Log("DownloadEnd=" + DateTime.Now.ToString(), "DOWNLOAD");
                //if ((now >= morningStart && now <= morningEnd))
                //{
                //    int minute1 = (1000 * 60);
                //    Console.WriteLine("Wait till=" + DateTime.Now.AddMinutes(5).ToString());
                //    System.Threading.Thread.Sleep((minute1 * 5));
                //    DownloadStart();
                //}
                //}
            }
        }

        private static string GetString(string html, string startWord, string endWord)
        {
            int startIndex = html.IndexOf(startWord);
            int endIndex = html.IndexOf(endWord);
            int length = endIndex - startIndex + endWord.Length;
            if (startIndex > 0 && endIndex > 0)
            {
                html = html.Substring(startIndex, length);
            }
            else
            {
                html = "";
                Helper.Log("ErrorOnGoogleData DownloadMC");
            }
            return html;
        }
        private static void DownloadMC()
        {
            string originalHtml = File.ReadAllText(Path.Combine(MC, "DMART.html"));
            string html = GetString(originalHtml, "<div id=\"mktdet_1\" name=\"mktdet_1\">", "<div id=\"mktdet_2\"");
            html = html.Replace("\n", "").Replace("\r", "").Replace("\r\n", "");
            decimal marketValue = 0;
            decimal pe = 0;
            decimal bookValue = 0;
            decimal dividend = 0;
            decimal marketLot = 0;
            decimal industryPE = 0;
            decimal eps = 0;
            decimal pc = 0;
            decimal priceBook = 0;
            decimal divYield = 0;
            decimal faceValue = 0;
            decimal delivarable = 0;
            Regex regex = null;
            if (string.IsNullOrEmpty(html) == false)
            {
                html = Regex.Replace(html, @"\s*(<[^>]+>)\s*", "$1", RegexOptions.Singleline);
                regex = new Regex(
                          @"<div\s+class=\""PA7\s+brdb\""(.*?)>(.*?)<div\s+class=\""CL\"""
                          + @">",
                          RegexOptions.IgnoreCase
                          | RegexOptions.Multiline
                          | RegexOptions.IgnorePatternWhitespace
                          | RegexOptions.Compiled
                          );
                MatchCollection mc = regex.Matches(html);
                foreach (Match m in mc)
                {
                    regex = new Regex(
  @"<div(.*?)>(.*?)</div>",
  RegexOptions.IgnoreCase
  | RegexOptions.Multiline
  | RegexOptions.IgnorePatternWhitespace
  | RegexOptions.Compiled
  );
                    MatchCollection rowMatches = regex.Matches(m.Value);
                    if (rowMatches.Count >= 2)
                    {
                        Match m1 = rowMatches[0];
                        Match m2 = rowMatches[1];
                        string colName = TradeHelper.RemoveHTMLTag(m1.Groups[2].Value);
                        switch (colName)
                        {
                            case "MARKET CAP (Rs Cr)":
                                marketValue = GetDecimalValue(m2.Groups[2].Value);
                                break;
                            case "P/E":
                                pe = GetDecimalValue(m2.Groups[2].Value);
                                break;
                            case "BOOK VALUE (Rs)":
                                bookValue = GetDecimalValue(m2.Groups[2].Value);
                                break;
                            case "DIV (%)":
                                dividend = GetDecimalValue(m2.Groups[2].Value);
                                break;
                            case "Market Lot":
                                marketLot = GetDecimalValue(m2.Groups[2].Value);
                                break;
                            case "INDUSTRY P/E":
                                industryPE = GetDecimalValue(m2.Groups[2].Value);
                                break;
                            case "EPS (TTM)":
                                eps = GetDecimalValue(m2.Groups[2].Value);
                                break;
                            case "P/C":
                                pc = GetDecimalValue(m2.Groups[2].Value);
                                break;
                            case "PRICE/BOOK":
                                priceBook = GetDecimalValue(m2.Groups[2].Value);
                                break;
                            case "DIV YIELD.(%)":
                                divYield = GetDecimalValue(m2.Groups[2].Value);
                                break;
                            case "FACE VALUE (Rs)":
                                faceValue = GetDecimalValue(m2.Groups[2].Value);
                                break;
                            case "DELIVERABLES (%)":
                                delivarable = GetDecimalValue(m2.Groups[2].Value);
                                break;
                        }
                    }
                }
            }
            List<RowCollections> rows = new List<RowCollections>();
            html = GetString(originalHtml, "<div id=\"findet_1\"", "<div id=\"findet_2\"");
            html = html.Replace("\n", "").Replace("\r", "").Replace("\r\n", "");
            regex = new Regex(
  @">\s*<",
  RegexOptions.IgnoreCase
  | RegexOptions.Multiline
  | RegexOptions.IgnorePatternWhitespace
  | RegexOptions.Compiled
  );
            html = regex.Replace(html, "><");

            regex = new Regex(
   @"<table(.*?)>(.*?)</table>",
   RegexOptions.IgnoreCase
   | RegexOptions.Multiline
   | RegexOptions.IgnorePatternWhitespace
   | RegexOptions.Compiled
   );
            MatchCollection collections = regex.Matches(html);
            if (collections.Count > 0)
            {
                string tableContent = collections[0].Groups[2].Value;

                regex = new Regex(
@"<tr(.*?)>(.*?)</tr>",
RegexOptions.IgnoreCase
| RegexOptions.Multiline
| RegexOptions.IgnorePatternWhitespace
| RegexOptions.Compiled
);
                MatchCollection trCollections = regex.Matches(tableContent);
                int i, j;
                for (i = 0; i < trCollections.Count; i++)
                {
                    Match trMatch = trCollections[i];
                    string tr = trMatch.Value;
                    regex = new Regex(
@"<td(.*?)>(.*?)</td>",
RegexOptions.IgnoreCase
| RegexOptions.Multiline
| RegexOptions.IgnorePatternWhitespace
| RegexOptions.Compiled
);
                    MatchCollection tdCollections = regex.Matches(tr);
                    RowCollections row = new RowCollections();
                    for (j = 0; j < tdCollections.Count; j++)
                    {
                        string v = TradeHelper.RemoveHTMLTag(tdCollections[j].Groups[2].Value);
                        if (j <= 0)
                        {
                            row.name = v;
                        }
                        row.cells.Add(v);
                    }
                    rows.Add(row);
                }
            }

        }

        private static decimal GetDecimalValue(string html)
        {
            Regex regex = new Regex(
    @"(?<d>(\d{1,3},\d{3}(,\d{3})*)(\.\d*)?|\d+\.?\d*)",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled
    );
            string removeHTML = TradeHelper.RemoveHTMLTag(html);
            decimal v = 0;
            try
            {
                MatchCollection mc = regex.Matches(removeHTML);
                foreach (Match m in mc)
                {
                    v = DataTypeHelper.ToDecimal(m.Groups["d"].Value);
                }
            }
            catch { }
            return v;
        }

        //using (EcamContext context = new EcamContext())
        //{
        //    //sql = "delete from tra_pre_calc_item";
        //    //MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
        //    var totalMarkets = (from q in context.tra_market
        //                        where (q.prev_price ?? 0) > 0
        //                        //&& q.symbol == "3MINDIA"
        //                        orderby q.trade_date descending, q.symbol ascending
        //                        select q).ToList();
        //    List<string> symbols = (from q in totalMarkets
        //                            select q.symbol).Distinct().ToList();
        //    int total = symbols.Count();
        //    int index = 0;
        //    foreach (string symbol in symbols)
        //    {
        //        index += 1;
        //        tra_prev_calc calc = (from q in context.tra_prev_calc
        //                              where q.symbol == symbol
        //                              select q).FirstOrDefault();
        //        if (calc == null)
        //        {
        //            calc = new tra_prev_calc
        //            {
        //                symbol = symbol,
        //                positive_count = 0,
        //                negative_count = 0,
        //                success_count = 0,
        //                fail_count = 0,
        //                high_profit = 0,
        //                open_profit = 0,
        //                id = 0
        //            };
        //        }
        //        else
        //        {
        //            calc.symbol = symbol;
        //            calc.positive_count = 0;
        //            calc.negative_count = 0;
        //            calc.success_count = 0;
        //            calc.fail_count = 0;
        //            calc.high_profit = 0;
        //            calc.open_profit = 0;
        //        }
        //        var markets = (from q in totalMarkets
        //                       where q.symbol == symbol
        //                       orderby q.trade_date descending
        //                       select q).ToList();
        //        foreach (var market in markets)
        //        {
        //            if ((market.prev_price ?? 0) < (market.open_price ?? 0))
        //            {
        //                calc.positive_count += 1;
        //            }
        //            else
        //            {
        //                calc.negative_count += 1;
        //            }
        //            decimal? open_profit = (((market.open_price ?? 0) - (market.prev_price ?? 0)) / (market.prev_price ?? 0)) * 100;
        //            decimal? high_profit = (((market.high_price ?? 0) - (market.prev_price ?? 0)) / (market.prev_price ?? 0)) * 100;
        //            if ((calc.open_profit ?? 0) <= (open_profit ?? 0))
        //            {
        //                calc.open_profit = open_profit;
        //            }
        //            if ((calc.high_profit ?? 0) <= (high_profit ?? 0))
        //            {
        //                calc.high_profit = high_profit;
        //            }
        //            if ((open_profit ?? 0) > 0 && (high_profit ?? 0) >= (decimal)0.5)
        //            {
        //                calc.success_count += 1;
        //            }
        //            else
        //            {
        //                calc.fail_count += 1;
        //            }
        //            //var prev = (from q in markets
        //            //            where q.symbol == market.symbol
        //            //            && q.trade_date < market.trade_date
        //            //            orderby q.trade_date descending
        //            //            select q).FirstOrDefault();
        //            //if (prev != null)
        //            //{
        //            //    decimal? profit = (((prev.ltp_price ?? 0) - (prev.prev_price ?? 0)) / (prev.prev_price ?? 0)) * 100;
        //            //    int intProfit = 0;
        //            //    intProfit = (int)profit;
        //            //    if (intProfit == 0 && (profit ?? 0) < 0)
        //            //    {
        //            //        intProfit = -100000;
        //            //    }
        //            //    tra_pre_calc_item item = (from q in context.tra_pre_calc_item
        //            //                              where q.percentage == intProfit
        //            //                              select q).FirstOrDefault();
        //            //    if (item == null)
        //            //    {
        //            //        item = new tra_pre_calc_item
        //            //        {
        //            //            success_count = 0,
        //            //            percentage = 0,
        //            //            fail_count = 0,
        //            //        };
        //            //    }
        //            //    item.percentage = intProfit;
        //            //    if ((open_profit ?? 0) > 0)
        //            //    {
        //            //        item.success_count += 1;
        //            //    }
        //            //    else
        //            //    {
        //            //        item.fail_count += 1;
        //            //    }
        //            //    if (item.id <= 0)
        //            //    {
        //            //        context.tra_pre_calc_item.Add(item);
        //            //    }
        //            //    else
        //            //    {
        //            //        context.Entry(item).State = System.Data.Entity.EntityState.Modified;
        //            //    }
        //            //    context.SaveChanges();
        //            //}
        //        }
        //        if (calc.id <= 0)
        //        {
        //            context.tra_prev_calc.Add(calc);
        //        }
        //        else
        //        {
        //            context.Entry(calc).State = System.Data.Entity.EntityState.Modified;
        //        }
        //        context.SaveChanges();
        //        Console.WriteLine("Total=" + total + ",Index=" + index);
        //    }
        //    //int total = markets.Count();
        //    //int index = 0;
        //    //foreach(var market in markets)
        //    //{
        //    //    index += 1;
        //    //    var prev = (from q in markets
        //    //                where q.symbol == market.symbol
        //    //                && q.trade_date < market.trade_date
        //    //                orderby q.trade_date descending
        //    //                select q).FirstOrDefault();
        //    //    if (prev != null)
        //    //    {
        //    //        market.prev_ltp_price = prev.ltp_price;
        //    //        sql = string.Format("update tra_market set prev_ltp_price={0} where market_id={1}", prev.ltp_price, market.id);
        //    //        MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
        //    //        //context.Entry(market).State = System.Data.Entity.EntityState.Modified;
        //    //        //context.SaveChanges();
        //    //    }
        //    //    Console.WriteLine("Total=" + total + ",Index=" + index);
        //    //}
        //}
        //Console.WriteLine("Completed");
        //Console.ReadLine();
        //MutualFunds();
        //CaculateIntraydayProfit();
        //using (EcamContext context = new EcamContext())
        //{
        //    List<tra_market> markets = (from q in context.tra_market
        //                                orderby q.symbol ascending
        //                                select q).ToList();
        //    foreach (var market in markets)
        //    {
        //        tra_market prev = (from q in context.tra_market
        //                           where q.symbol == market.symbol
        //                           && q.trade_date < market.trade_date
        //                           orderby q.trade_date descending
        //                           select q).FirstOrDefault();
        //        if (prev != null)
        //        {
        //            market.prev_price = prev.close_price;
        //            sql = string.Format("update tra_market set prev_price={0} where market_id={1}", prev.close_price, market.id);
        //            MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
        //            Console.WriteLine("Update market price symbol=" + market.symbol + ",Date=" + market.trade_date.ToString("MMM/dd/yyyy"));
        //        }
        //    }
        //    List<tra_company> companies = (from q in context.tra_company
        //                                   orderby q.symbol ascending
        //                                   select q).ToList();
        //    foreach (var company in companies)
        //    {
        //        TradeHelper.UpdateCompanyPrice(company.symbol);
        //        Console.WriteLine("Update company price symbol=" + company.symbol);
        //    }
        //}
        //private static void CaculateIntraydayProfit()
        //{
        //    List<tra_market_intra_day> rows;
        //    List<tra_company> companies;
        //    using (EcamContext context = new EcamContext())
        //    {
        //        rows = (from q in context.tra_market_intra_day orderby q.symbol, q.trade_date ascending select q).ToList();
        //        companies = (from q in context.tra_company select q).ToList();
        //        //foreach (var market in rows)
        //        //{
        //        //    DateTime tradeDate = market.trade_date.Date;
        //        //    var prev = (from q in context.tra_market
        //        //                where q.symbol == market.symbol
        //        //                && q.trade_date < tradeDate
        //        //                orderby q.trade_date descending
        //        //                select q).FirstOrDefault();
        //        //    if (prev != null)
        //        //    {
        //        //        TempRSI value = new TempRSI
        //        //        {
        //        //            symbol = market.symbol,
        //        //            close = market.ltp_price,
        //        //            prev = (prev.close_price ?? 0),
        //        //            date = market.trade_date,
        //        //        };
        //        //        value.avg_downward = (((prev.avg_downward ?? 0) * (14 - 1) + value.downward) / 14);
        //        //        value.avg_upward = (((prev.avg_upward ?? 0) * (14 - 1) + value.upward) / 14);
        //        //        //market.avg_upward = value.avg_upward;
        //        //        //market.avg_downward = value.avg_downward;
        //        //        //market.prev_rsi = prev.rsi;
        //        //        market.rsi = value.rsi;
        //        //        context.Entry(market).State = System.Data.Entity.EntityState.Modified;
        //        //        context.SaveChanges();
        //        //    }
        //        //}
        //    }
        //    if (rows.Count > 0)
        //    {
        //        List<string> symbols = (from q in rows select q.symbol).Distinct().ToList();
        //        DateTime firstDate = (from q in rows orderby q.trade_date descending select q).FirstOrDefault().trade_date.Date;
        //        DateTime startTime = Convert.ToDateTime(firstDate.ToString("dd/MMM/yyyy") + " 9:19AM");
        //        DateTime endTime = Convert.ToDateTime(firstDate.ToString("dd/MMM/yyyy") + " 10:00AM");
        //        foreach (string symbol in symbols)
        //        {
        //            var company = (from q in companies where q.symbol == symbol select q).FirstOrDefault();
        //            var firstLTP = (from q in rows where q.symbol == symbol && q.trade_date >= firstDate && q.trade_date <= startTime orderby q.trade_date descending select q).FirstOrDefault();
        //            var lastLTP = (from q in rows where q.symbol == symbol && q.trade_date >= firstDate && q.trade_date <= endTime orderby q.trade_date descending select q).FirstOrDefault();
        //            if (firstLTP != null && lastLTP != null && company != null)
        //            {
        //                try
        //                {
        //                    decimal? firstPrice = firstLTP.ltp_price;
        //                    decimal? lastPrice = lastLTP.ltp_price;
        //                    decimal? finalPrice = company.ltp_price;
        //                    decimal? openPrice = company.open_price;

        //                    decimal? rsi = firstLTP.rsi;
        //                    decimal? prevRSI = 0;
        //                    using(EcamContext context = new EcamContext())
        //                    {
        //                        DateTime dt = firstLTP.trade_date.Date;
        //                        var prev = (from q in context.tra_market
        //                                    where q.symbol == firstLTP.symbol
        //                                    && q.trade_date < dt
        //                                    orderby q.trade_date descending
        //                                    select q).FirstOrDefault();
        //                        if (prev != null)
        //                        {
        //                            prevRSI = prev.rsi;
        //                        }
        //                    }

        //                    List<decimal> percentageList = new List<decimal>();

        //                    var nextList = (from q in rows
        //                                    where q.symbol == symbol && q.trade_date > firstLTP.trade_date
        //                                    orderby q.trade_date ascending
        //                                    select q).ToList();
        //                    foreach (var nextRow in nextList)
        //                    {
        //                        decimal? p = ((nextRow.ltp_price - (firstPrice ?? 0)) / (firstPrice ?? 0)) * 100;
        //                        percentageList.Add((p ?? 0));
        //                    }

        //                    decimal? percentage = 0;
        //                    if (percentageList.Count > 0)
        //                    {
        //                        percentage = (from q in percentageList orderby q descending select q).FirstOrDefault();
        //                    }
        //                    else
        //                    {
        //                        percentage = (((lastPrice ?? 0) - (firstPrice ?? 0)) / (firstPrice ?? 0)) * 100;
        //                    }

        //                    percentageList = new List<decimal>();
        //                    foreach (var nextRow in nextList)
        //                    {
        //                        decimal? p = (((firstPrice ?? 0) - nextRow.ltp_price) / nextRow.ltp_price) * 100;
        //                        percentageList.Add((p ?? 0));
        //                    }

        //                    decimal? reversePercentage = 0;
        //                    if (percentageList.Count > 0)
        //                    {
        //                        reversePercentage = (from q in percentageList orderby q descending select q).FirstOrDefault();
        //                    }
        //                    else
        //                    {
        //                        reversePercentage = (((firstPrice ?? 0) - (lastPrice ?? 0)) / (lastPrice ?? 0)) * 100;
        //                    }

        //                    decimal? finalPercentage = (((finalPrice ?? 0) - (firstPrice ?? 0)) / (firstPrice ?? 0)) * 100;

        //                    decimal? firstPercentage = (((firstPrice ?? 0) - (openPrice ?? 0)) / (openPrice ?? 0)) * 100;

        //                    decimal? lastPercentage = (((lastPrice ?? 0) - (openPrice ?? 0)) / (openPrice ?? 0)) * 100;

        //                    bool isDay1High = (company.day_1 ?? 0) <= (company.ltp_price ?? 0);
        //                    bool isDay2High = (
        //                            (company.day_2 ?? 0) < (company.day_1 ?? 0)
        //                            && (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
        //                            );
        //                    bool isDay3High = (
        //                    (company.day_3 ?? 0) < (company.day_2 ?? 0)
        //                    && (company.day_2 ?? 0) < (company.day_1 ?? 0)
        //                    && (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
        //                    );
        //                    bool isDay4High = (
        //                        (company.day_4 ?? 0) < (company.day_3 ?? 0)
        //                    && (company.day_3 ?? 0) < (company.day_2 ?? 0)
        //                    && (company.day_2 ?? 0) < (company.day_1 ?? 0)
        //                    && (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
        //                    );

        //                    bool isDay5High = (
        //                        (company.day_5 ?? 0) < (company.day_4 ?? 0)
        //                       && (company.day_4 ?? 0) < (company.day_3 ?? 0)
        //                    && (company.day_3 ?? 0) < (company.day_2 ?? 0)
        //                    && (company.day_2 ?? 0) < (company.day_1 ?? 0)
        //                    && (company.day_1 ?? 0) <= (company.ltp_price ?? 0)
        //                    );

        //                    bool isDay1Low = (company.day_1 ?? 0) >= (company.ltp_price ?? 0);
        //                    bool isDay2Low = (
        //                            (company.day_2 ?? 0) > (company.day_1 ?? 0)
        //                            && (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
        //                            );
        //                    bool isDay3Low = (
        //                    (company.day_3 ?? 0) > (company.day_2 ?? 0)
        //                    && (company.day_2 ?? 0) > (company.day_1 ?? 0)
        //                    && (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
        //                    );
        //                    bool isDay4Low = (
        //                        (company.day_4 ?? 0) > (company.day_3 ?? 0)
        //                    && (company.day_3 ?? 0) > (company.day_2 ?? 0)
        //                    && (company.day_2 ?? 0) > (company.day_1 ?? 0)
        //                    && (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
        //                    );
        //                    bool isDay5Low = (
        //                        (company.day_5 ?? 0) > (company.day_4 ?? 0)
        //                        && (company.day_4 ?? 0) > (company.day_3 ?? 0)
        //                    && (company.day_3 ?? 0) > (company.day_2 ?? 0)
        //                    && (company.day_2 ?? 0) > (company.day_1 ?? 0)
        //                    && (company.day_1 ?? 0) >= (company.ltp_price ?? 0)
        //                    );
        //                    using (EcamContext context = new EcamContext())
        //                    {
        //                        tra_intra_day_profit profit = (from q in context.tra_intra_day_profit
        //                                                       where q.symbol == symbol
        //                                                       && q.trade_date == firstDate
        //                                                       select q).FirstOrDefault();
        //                        bool isNew = false;
        //                        if (profit == null)
        //                        {
        //                            profit = new tra_intra_day_profit();
        //                            isNew = true;
        //                        }
        //                        profit.symbol = symbol;
        //                        profit.trade_date = firstDate;
        //                        profit.profit_percentage = (percentage ?? 0);
        //                        profit.reverse_percentage = (reversePercentage ?? 0);
        //                        profit.last_percentage = (lastPercentage ?? 0);
        //                        profit.first_percentage = (firstPercentage ?? 0);
        //                        profit.final_percentage = (finalPercentage ?? 0);
        //                        profit.rsi = rsi;
        //                        profit.prev_rsi = prevRSI;
        //                        profit.diff_rsi = (rsi ?? 0) - (prevRSI ?? 0);
        //                        int highCnt = 0;
        //                        int lowCnt = 0;
        //                        if (isDay1High == true && isDay2High == true
        //                            && isDay3High == true && isDay4High == true
        //                            && isDay5High == true)
        //                        {
        //                            highCnt = 5;
        //                        }
        //                        else if (isDay1High == true && isDay2High == true
        //                            && isDay3High == true && isDay4High == true
        //                            )
        //                        {
        //                            highCnt = 4;
        //                        }
        //                        else if (isDay1High == true && isDay2High == true
        //                            && isDay3High == true
        //                            )
        //                        {
        //                            highCnt = 3;
        //                        }
        //                        else if (isDay1High == true && isDay2High == true
        //                            )
        //                        {
        //                            highCnt = 2;
        //                        }
        //                        else if (isDay1High == true)
        //                        {
        //                            highCnt = 1;
        //                        }

        //                        if (isDay1Low == true && isDay2Low == true
        //                            && isDay3Low == true && isDay4Low == true
        //                            && isDay5Low == true)
        //                        {
        //                            lowCnt = 5;
        //                        }
        //                        else if (isDay1Low == true && isDay2Low == true
        //                            && isDay3Low == true && isDay4Low == true
        //                            )
        //                        {
        //                            lowCnt = 4;
        //                        }
        //                        else if (isDay1Low == true && isDay2Low == true
        //                            && isDay3Low == true
        //                            )
        //                        {
        //                            lowCnt = 3;
        //                        }
        //                        else if (isDay1Low == true && isDay2Low == true
        //                            )
        //                        {
        //                            lowCnt = 2;
        //                        }
        //                        else if (isDay1Low == true)
        //                        {
        //                            lowCnt = 1;
        //                        }
        //                        profit.high_count = highCnt;
        //                        profit.low_count = lowCnt;

        //                        if (isNew == true)
        //                        {
        //                            context.tra_intra_day_profit.Add(profit);
        //                        }
        //                        else
        //                        {
        //                            context.Entry(profit).State = System.Data.Entity.EntityState.Modified;
        //                        }
        //                        context.SaveChanges();

        //                        var updateCompany = (from q in context.tra_company where q.symbol == symbol select q).FirstOrDefault();
        //                        if (updateCompany != null)
        //                        {
        //                            updateCompany.high_count = highCnt;
        //                            updateCompany.low_count = lowCnt;
        //                            context.Entry(updateCompany).State = System.Data.Entity.EntityState.Modified;
        //                            context.SaveChanges();
        //                        }
        //                        Console.WriteLine("Calculate profit completed symbol=" + symbol);
        //                    }
        //                }
        //                catch { }
        //            }
        //        }
        //    }
        //}


        private static void GoogleData()
        {
            List<tra_company> companies;
            using (EcamContext context = new EcamContext())
            {
                IQueryable<tra_company> query = context.tra_company;

                DateTime morningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 9:15AM");
                DateTime morningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 10:15AM");
                DateTime eveningStart = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 3:31PM");
                DateTime eveningEnd = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 11:59PM");
                DateTime now = DateTime.Now;
                DateTime targetTime = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy") + " 3:30PM");

                if ((now >= morningStart && now <= eveningStart))
                {
                    //query = (from q in query
                    //         join h in context.tra_holding on q.symbol equals h.symbol
                    //         select q);
                    //string IS_NIFTY_50 = System.Configuration.ConfigurationManager.AppSettings["IS_NIFTY_50"];
                    //string IS_NIFTY_100 = System.Configuration.ConfigurationManager.AppSettings["IS_NIFTY_100"];
                    //string IS_NIFTY_200 = System.Configuration.ConfigurationManager.AppSettings["IS_NIFTY_200"];
                    //if (IS_NIFTY_50 == "true")
                    //{
                    //    query = query.Where(q => (q.is_nifty_50 ?? false) == true);
                    //}
                    //if (IS_NIFTY_100 == "true")
                    //{
                    //    query = query.Where(q => (q.is_nifty_100 ?? false) == true);
                    //}
                    //if (IS_NIFTY_200 == "true")
                    //{
                    //    query = query.Where(q => (q.is_nifty_200 ?? false) == true);
                    //}
                }
                if(IS_BOOK_MARK_CATEGORY == "true") {
                    List<tra_company_category> companyCategories = (from q in context.tra_company_category
                                                                    join c in context.tra_category on q.category_name equals c.category_name
                                                                    where (c.is_book_mark ?? false) == true
                                                                    select q).ToList();
                    List<string> categorySymbols = (from q in companyCategories
                                                    select q.symbol).Distinct().ToList();
                    query = (from q in query where categorySymbols.Contains(q.symbol) == true select q);
                }
                companies = (from q in query
                             orderby q.symbol ascending
                             select q).ToList();
            }
            _COMPANIES = (from q in companies select q.symbol).ToArray();
            _INDEX = -1;

            //string sql = "";

            // Delete yesterday tra_market_intraday
            //sql = "delete from tra_market_intra_day where DATE_FORMAT(trade_date, '%Y-%m-%d') < DATE_FORMAT(curdate(), '%Y-%m-%d')";
            //MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);

            // Delete before 3 months tra_market
            //sql = "delete from tra_market where DATE_FORMAT(trade_date, '%Y-%m-%d') < DATE_FORMAT(DATE_ADD(curdate(), INTERVAL -3 MONTH), '%Y-%m-%d')";
            //MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);

            GoogleDownloadStart();
            //foreach (var company in companies)
            //{
            //    Thread thread = new Thread(() => GoogleDataDownload(company.symbol));
            //    thread.Start();
            //}
        }

        private static void GoogleDownloadStart()
        {
            int totalCount = _COMPANIES.Length;
            int queueCount = 64;
            // One event is used for each Fibonacci object
            ManualResetEvent[] doneEvents = new ManualResetEvent[queueCount];
            GoogleDownloadData[] downArray = new GoogleDownloadData[queueCount];
            //Random r = new Random();
            // Configure and launch threads using ThreadPool:
            Console.WriteLine("launching {0} tasks...", totalCount);
            for (int i = 0; i < queueCount; i++)
            {
                _INDEX += 1;
                string symbol = "";
                if (_INDEX < _COMPANIES.Length)
                {
                    symbol = _COMPANIES[_INDEX];
                }
                doneEvents[i] = new ManualResetEvent(false);
                GoogleDownloadData f = new GoogleDownloadData(symbol, doneEvents[i]);
                downArray[i] = f;
                ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
            }
            // Wait for all threads in pool to calculation...
            WaitHandle.WaitAll(doneEvents);
            if (_INDEX < _COMPANIES.Length)
            {
                Console.WriteLine("All calculations are complete.");
                GoogleDownloadStart();
            }
        }

        private static void GoogleHistoryDownloadStart()
        {
            int totalCount = _COMPANIES.Length;
            int queueCount = 1;
            if (totalCount <= queueCount)
            {
                queueCount = totalCount;
            }
            // One event is used for each Fibonacci object
            ManualResetEvent[] doneEvents = new ManualResetEvent[queueCount];
            GoogleHistoryDownloadData[] downArray = new GoogleHistoryDownloadData[queueCount];
            //Random r = new Random();
            // Configure and launch threads using ThreadPool:
            Console.WriteLine("launching {0} tasks...", totalCount);
            for (int i = 0; i < queueCount; i++)
            {
                _INDEX += 1;
                string symbol = "";
                if (_INDEX < _COMPANIES.Length)
                {
                    symbol = _COMPANIES[_INDEX];
                }
                doneEvents[i] = new ManualResetEvent(false);
                GoogleHistoryDownloadData f = new GoogleHistoryDownloadData(symbol, doneEvents[i]);
                downArray[i] = f;
                ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
            }
            // Wait for all threads in pool to calculation...
            WaitHandle.WaitAll(doneEvents);
            if (_INDEX < _COMPANIES.Length)
            {
                Console.WriteLine("All calculations are complete.");
                GoogleHistoryDownloadStart();
            }
        }

        private static void GoogleHistoryData()
        {
            List<tra_company> companies;
            using (EcamContext context = new EcamContext())
            {
                IQueryable<tra_company> query = context.tra_company;
                string SYMBOLS = System.Configuration.ConfigurationManager.AppSettings["SYMBOLS"];
                if (string.IsNullOrEmpty(SYMBOLS) == false)
                {
                    List<string> symbolList = Helper.ConvertStringList(SYMBOLS);
                    if (symbolList.Count > 0)
                    {
                        query = (from q in query where symbolList.Contains(q.symbol) == true select q);
                    }
                }
                if(IS_BOOK_MARK_CATEGORY == "true") {
                    List<tra_company_category> companyCategories = (from q in context.tra_company_category
                                                                    join c in context.tra_category on q.category_name equals c.category_name
                                                                    where (c.is_book_mark ?? false) == true
                                                                    select q).ToList();
                    List<string> categorySymbols = (from q in companyCategories
                                                    select q.symbol).Distinct().ToList();
                    query = (from q in query where categorySymbols.Contains(q.symbol) == true select q);
                }
                companies = (from q in query orderby q.symbol ascending select q).ToList();
            }
            _COMPANIES = (from q in companies select q.symbol).ToArray();
            _INDEX = -1;
            GoogleHistoryDownloadStart();
        }

        private static void MoneyControlDownloadStart()
        {
            int totalCount = _URLS.Count;
            int queueCount = 64;
            // One event is used for each Fibonacci object
            ManualResetEvent[] doneEvents = new ManualResetEvent[queueCount];
            MoneyControlData[] downArray = new MoneyControlData[queueCount];
            //Random r = new Random();
            // Configure and launch threads using ThreadPool:
            Console.WriteLine("launching {0} tasks...", totalCount);
            for (int i = 0; i < queueCount; i++)
            {
                _INDEX += 1;
                string url = "";
                if (_INDEX < _URLS.Count)
                {
                    url = _URLS[_INDEX];
                }
                doneEvents[i] = new ManualResetEvent(false);
                MoneyControlData f = new MoneyControlData(url, doneEvents[i]);
                downArray[i] = f;
                ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
            }
            // Wait for all threads in pool to calculation...
            WaitHandle.WaitAll(doneEvents);
            if (_INDEX < _URLS.Count)
            {
                Console.WriteLine("All calculations are complete.");
                MoneyControlDownloadStart();
            }
        }

        private static void MutualFunds()
        {
            string linkFileName = System.Configuration.ConfigurationManager.AppSettings["LINK_FILE_NAME"];
            if (string.IsNullOrEmpty(linkFileName) == false)
            {
                string content = System.IO.File.ReadAllText(linkFileName);
                string[] arr = content.Split(("\r\n").ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string url in arr)
                {
                    if (string.IsNullOrEmpty(url) == false)
                    {
                        ParseMainHTML(url);
                    }
                }
            }
            using (EcamContext context = new EcamContext())
            {
                var companies = (from q in context.tra_company select q).ToList();
                var funds = (from q in context.tra_mutual_fund_pf select q).ToList();
                foreach (var company in companies)
                {
                    company.mf_cnt = (from q in funds
                                      where q.symbol == company.symbol
                                      select q.fund_id).Count();
                    company.mf_qty = (from q in funds
                                      where q.symbol == company.symbol
                                      select q.quantity).Sum();
                    context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
            }
        }

        private static void ParseMainHTML(string url)
        {
            //string fileName = "C:\\Users\\kart\\Desktop\\MF\\1.html";
            //string content = System.IO.File.ReadAllText(fileName);
            string MF_HTML_PATH = System.Configuration.ConfigurationManager.AppSettings["MF_HTML_PATH"];
            string fundCode = url.Replace("http://www.moneycontrol.com/india/mutualfunds/mfinfo/portfolio_holdings/", "");
            string fileName = MF_HTML_PATH + "\\" + fundCode + ".html";
            string content = "";
            if (File.Exists(fileName) == true)
            {
                content = System.IO.File.ReadAllText(fileName);
            }
            else
            {
                WebClient client = new WebClient();
                content = client.DownloadString(url);
                System.IO.File.WriteAllText(fileName, content);
            }
            if (string.IsNullOrEmpty(content) == false)
            {
                Console.WriteLine("ParseMainHTML url=" + url);
                string startWord = "<table width=\"100%\" class=\"tblporhd\">";
                string endWord = "<table width=\"100%\" class=\"tblporhd MT25\">";
                int startIndex = content.IndexOf(startWord);
                int endIndex = content.IndexOf(endWord);
                int length = endIndex - startIndex + endWord.Length;
                if (startIndex > 0 && endIndex > 0)
                {
                    string parseContent = content.Substring(startIndex, length);
                    endWord = "</table>";
                    startIndex = parseContent.IndexOf(startWord);
                    endIndex = parseContent.IndexOf(endWord);
                    length = endIndex - startIndex + endWord.Length;
                    parseContent = parseContent.Substring(startIndex, length);
                    Regex regex = null;

                    string fundName = string.Empty;
                    tra_mutual_fund fund = null;
                    regex = new Regex(
            @"<h1\s*style\=\""font-size:30px;\"">(?<fund_name>.*)</h1>",
            RegexOptions.IgnoreCase
            | RegexOptions.Multiline
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled
            );
                    MatchCollection collections = regex.Matches(content);
                    if (collections.Count > 0)
                    {
                        fundName = collections[0].Groups["fund_name"].Value;
                    }
                    if (string.IsNullOrEmpty(fundName) == false)
                    {
                        using (EcamContext context = new EcamContext())
                        {
                            fund = (from q in context.tra_mutual_fund
                                    where q.fund_name == fundName
                                    select q).FirstOrDefault();
                            if (fund == null)
                            {
                                fund = new tra_mutual_fund();
                                fund.fund_name = fundName;
                                context.tra_mutual_fund.Add(fund);
                                context.SaveChanges();
                            }
                            var rows = (from q in context.tra_mutual_fund_pf
                                        where q.fund_id == fund.id
                                        select q).ToList();
                            foreach (var row in rows)
                            {
                                context.tra_mutual_fund_pf.Remove(row);
                            }
                            context.SaveChanges();
                        }
                    }
                    regex = new Regex(
         @"<tr>(.*?)</tr>",
         RegexOptions.IgnoreCase
         | RegexOptions.Multiline
         | RegexOptions.IgnorePatternWhitespace
         | RegexOptions.Compiled
         );

                    MatchCollection trCollections = regex.Matches(parseContent);
                    int i = 0;
                    foreach (Match trMatch in trCollections)
                    {
                        i += 1;
                        string tr = trMatch.Value;
                        string tagName = "td";

                        regex = new Regex(
                                    @"<" + tagName + "[^>]*>(.+?)</" + tagName + ">",
                                    RegexOptions.IgnoreCase
                                    | RegexOptions.Multiline
                                    | RegexOptions.IgnorePatternWhitespace
                                    | RegexOptions.Compiled
                                    );
                        MatchCollection rowMatches = regex.Matches(tr);
                        string equity = "";
                        string sector = "";
                        string qty = "";
                        string totalValue = "";
                        string percentage = "";

                        int colIndex = -1;
                        foreach (Match colMatch in rowMatches)
                        {
                            colIndex += 1;

                            string value = string.Empty;
                            if (colMatch.Groups.Count >= 2)
                            {
                                value = colMatch.Groups[1].Value;
                            }
                            if (string.IsNullOrEmpty(value) == false)
                            {
                                value = value.Trim();
                            }
                            switch (colIndex)
                            {
                                case 0: equity = value; break;
                                case 1: sector = value; break;
                                case 2: qty = value; break;
                                case 3: totalValue = value; break;
                                case 4: percentage = value; break;
                            }

                        }
                        string symbol = "";
                        string symbolName = "";
                        if (string.IsNullOrEmpty(equity) == false)
                        {
                            regex = new Regex(
                                    @"<[^>].+?>",
                                    RegexOptions.IgnoreCase
                                    | RegexOptions.Multiline
                                    | RegexOptions.IgnorePatternWhitespace
                                    | RegexOptions.Compiled
                                    );
                            symbolName = regex.Replace(equity, "").Trim();
                            if (string.IsNullOrEmpty(symbolName) == false)
                            {
                                using (EcamContext context = new EcamContext())
                                {
                                    tra_mutual_fund_pf temp = (from q in context.tra_mutual_fund_pf
                                                               where q.stock_name == symbolName
                                                               select q).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        symbol = temp.symbol;
                                    }
                                    if (string.IsNullOrEmpty(symbol) == false)
                                    {
                                        tra_company company = (from q in context.tra_company
                                                               where q.symbol == symbol
                                                               select q).FirstOrDefault();
                                        if (company == null)
                                        {
                                            symbol = string.Empty;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Symbol already get symbol=" + symbol);
                                        }
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(symbol) == true)
                            {
                                symbol = GetSymbol(equity, symbolName);
                            }
                        }
                        if (string.IsNullOrEmpty(symbol) == false
                            && fund != null)
                        {
                            using (EcamContext context = new EcamContext())
                            {
                                tra_company company = (from q in context.tra_company
                                                       where q.symbol == symbol
                                                       select q).FirstOrDefault();
                                if (company != null)
                                {
                                    tra_mutual_fund_pf pf = (from q in context.tra_mutual_fund_pf
                                                             where q.fund_id == fund.id
                                                             && q.symbol == symbol
                                                             select q).FirstOrDefault();
                                    bool isNew = false;
                                    if (pf == null)
                                    {
                                        pf = new tra_mutual_fund_pf();
                                        isNew = true;
                                    }
                                    pf.symbol = symbol;
                                    pf.fund_id = fund.id;
                                    pf.quantity = DataTypeHelper.ToDecimal(qty);
                                    pf.stock_value = DataTypeHelper.ToDecimal(totalValue);
                                    pf.stock_percentage = DataTypeHelper.ToDecimal(percentage);
                                    pf.stock_name = symbolName;
                                    if (isNew == true)
                                    {
                                        context.tra_mutual_fund_pf.Add(pf);
                                    }
                                    else
                                    {
                                        context.Entry(pf).State = System.Data.Entity.EntityState.Modified;
                                    }
                                    context.SaveChanges();
                                }
                                else
                                {
                                    Helper.Log("equity=" + equity + ",symbol=" + symbol, "FundDoesNotExist");
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string GetSymbol(string equity, string symbolName)
        {
            string symbol = "";
            try
            {
                Regex regex = new Regex(
        @"href=[""|'](.*?)[""|']",
        RegexOptions.IgnoreCase
        | RegexOptions.Multiline
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled
        );
                MatchCollection collections = regex.Matches(equity);
                if (collections.Count > 0)
                {
                    string content = string.Empty;
                    string HTML_PATH = System.Configuration.ConfigurationManager.AppSettings["HTML_PATH"];
                    string fileName = HTML_PATH + "\\" + symbolName + ".html";
                    if (File.Exists(fileName) == true)
                    {
                        content = System.IO.File.ReadAllText(fileName);
                        Console.WriteLine("Process File Text=" + symbolName);
                    }
                    else
                    {
                        string href = "http://www.moneycontrol.com/" + collections[0].Groups[1].Value;
                        WebClient client = new WebClient();
                        content = client.DownloadString(href);
                        System.IO.File.WriteAllText(fileName, content);
                        Console.WriteLine("Download equity=" + href);
                    }
                    //string fileName = "C:\\Users\\kart\\Desktop\\MF\\relianceindustries_RI.html";
                    //content = System.IO.File.ReadAllText(fileName);
                    regex = new Regex(
        @"NSE:(?<symbol>\s*\w*&*\w*)",
        RegexOptions.IgnoreCase
        | RegexOptions.Multiline
        | RegexOptions.IgnorePatternWhitespace
        | RegexOptions.Compiled
        );
                    collections = regex.Matches(content);
                    if (collections.Count > 0)
                    {
                        symbol = collections[0].Groups[1].Value.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += ex.InnerException.Message;
                }
                Console.WriteLine("GetSymbol_Error=" + msg);
                Helper.Log(msg, "GetSymbol_Error");
            }
            return symbol;
        }

        private static void CSVData()
        {
            string isImportCSV = System.Configuration.ConfigurationManager.AppSettings["IS_IMPORT_CSV"];
            string importCSVDirectoryPath = System.Configuration.ConfigurationManager.AppSettings["IMPORT_CSV"];
            List<string> symbols = new List<string>();
            if (isImportCSV == "true")
            {
                string[] files = System.IO.Directory.GetFiles(importCSVDirectoryPath);
                foreach (string fullFileName in files)
                {
                    symbols.Add(System.IO.Path.GetFileNameWithoutExtension(fullFileName));
                }
            }
            symbols = (from q in symbols orderby q ascending select q).ToList();
            _COMPANIES = symbols.ToArray();
            _INDEX = -1;
            CSVDownloadStart();
        }

        private static void UpdateNiftyFlagCSV()
        {
            string niftyFlagCSVDirectoryPath = System.Configuration.ConfigurationManager.AppSettings["NIFTY_FLAG_CSV"];
            List<string> symbols = new List<string>();
            if (IS_NIFTY_FLAG_CSV == "true")
            {
                string[] files = System.IO.Directory.GetFiles(niftyFlagCSVDirectoryPath);
                foreach (string fullFileName in files)
                {
                    string flag = System.IO.Path.GetFileNameWithoutExtension(fullFileName);
                    int i;
                    using (TextReader reader = File.OpenText(fullFileName))
                    {
                        CsvReader csv = new CsvReader(reader);
                        i = 0;
                        while (csv.Read())
                        {
                            i += 1;
                            string symbol = csv.GetField<string>("Symbol");
                            string series = csv.GetField<string>("Series");
                            if (string.IsNullOrEmpty(symbol) == false
                                && series == "EQ")
                            {
                                using (EcamContext context = new EcamContext())
                                {
                                    tra_company company = (from q in context.tra_company
                                                           where q.symbol == symbol
                                                           select q).FirstOrDefault();
                                    if (company != null)
                                    {
                                        switch (flag)
                                        {
                                            case "200":
                                                company.is_nifty_200 = true;
                                                break;
                                            case "100":
                                                company.is_nifty_100 = true;
                                                break;
                                            case "50":
                                                company.is_nifty_50 = true;
                                                break;
                                        }
                                        context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                                        context.SaveChanges();
                                        Console.WriteLine("Update flag=" + flag + ",symbol=" + company.symbol);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void UpdateCategoryFlagCSV()
        {
            string categoryFlagCSVDirectoryPath = System.Configuration.ConfigurationManager.AppSettings["CATEGORY_FLAG_CSV"];
            List<string> symbols = new List<string>();
            if (IS_CATEGORY_FLAG_CSV == "true")
            {
                string[] files = System.IO.Directory.GetFiles(categoryFlagCSVDirectoryPath);
                foreach (string fullFileName in files)
                {
                    string flag = System.IO.Path.GetFileNameWithoutExtension(fullFileName);
                    int i;
                    using (TextReader reader = File.OpenText(fullFileName))
                    {
                        CsvReader csv = new CsvReader(reader);
                        i = 0;
                        while (csv.Read())
                        {
                            i += 1;
                            string symbol = csv.GetField<string>("Symbol");
                            string series = csv.GetField<string>("Series");
                            string industry = csv.GetField<string>("Industry");
                            if (string.IsNullOrEmpty(symbol) == false
                                && string.IsNullOrEmpty(industry) == false
                                && series == "EQ")
                            {
                                using (EcamContext context = new EcamContext())
                                {
                                    tra_category category = (from q in context.tra_category
                                                             where q.category_name == industry
                                                             select q).FirstOrDefault();
                                    if (category == null)
                                    {
                                        context.tra_category.Add(new tra_category
                                        {
                                            category_name = industry
                                        });
                                        context.SaveChanges();
                                    }
                                    tra_company company = (from q in context.tra_company
                                                           where q.symbol == symbol
                                                           select q).FirstOrDefault();
                                    if (company != null)
                                    {
                                        tra_company_category companyCategory = (from q in context.tra_company_category
                                                                                where q.symbol == symbol
                                                                                && q.category_name == industry
                                                                                select q).FirstOrDefault();
                                        if (companyCategory == null)
                                        {
                                            context.tra_company_category.Add(new tra_company_category
                                            {
                                                category_name = industry,
                                                symbol = symbol
                                            });
                                            context.SaveChanges();
                                        }
                                        Console.WriteLine("Update category=" + industry + ",symbol=" + company.symbol);
                                    }
                                    else
                                    {
                                        Helper.Log("Company does not exist symbol=" + symbol, "UpdateCategoryFlagCSV");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CSVDownloadStart()
        {
            int totalCount = _COMPANIES.Length;
            int queueCount = 64;
            if (totalCount <= queueCount)
            {
                queueCount = totalCount;
            }
            // One event is used for each Fibonacci object
            ManualResetEvent[] doneEvents = new ManualResetEvent[queueCount];
            CSVDownloadData[] downArray = new CSVDownloadData[queueCount];
            //Random r = new Random();
            // Configure and launch threads using ThreadPool:
            Console.WriteLine("launching {0} tasks...", totalCount);
            for (int i = 0; i < queueCount; i++)
            {
                _INDEX += 1;
                string symbol = "";
                if (_INDEX < _COMPANIES.Length)
                {
                    symbol = _COMPANIES[_INDEX];
                }
                doneEvents[i] = new ManualResetEvent(false);
                CSVDownloadData f = new CSVDownloadData(symbol,_SYMBOLS_LIST, doneEvents[i]);
                downArray[i] = f;
                ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
            }
            // Wait for all threads in pool to calculation...
            WaitHandle.WaitAll(doneEvents);
            if (_INDEX < _COMPANIES.Length)
            {
                Console.WriteLine("All calculations are complete.");
                CSVDownloadStart();
            }
        }
    }


}
