using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.IO;
using Ecam.Framework;
using Ecam.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;

namespace Ecam.AlertEmail
{
    class Google_Api_Read_Program
    {
        static void Main2(string[] args)
        {
            List<tra_company> companies;
            using (EcamContext context = new EcamContext())
            {
                companies = (from q in context.tra_company
                             select q).ToList();
            }
            foreach (var company in companies)
            {
                Parser("NSE", company.symbol);
            }
            Console.ReadLine();
        }

        private static void Parser(string type, string symbol)
        {
            try
            {
                string url = string.Format("http://finance.google.com/finance/info?client=ig&q={0}:{1}", type, symbol);
                WebClient webClient = new WebClient();
                string jsonTEXT = webClient.DownloadString(url);
                //string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //string sFile = appPath + "\\test.json";
                //string jsonTEXT = System.IO.File.ReadAllText(sFile);
                jsonTEXT = jsonTEXT.Replace("\n", "").Replace(" ", "").Replace("//", "").Replace("[{", "{").Replace("}]", "}");
                UpdatePrice(jsonTEXT);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Parser Ex=" + ex.Message);
            }
        }


        private static void UpdatePrice(string jsonTEXT)
        {
            try
            {
                //string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //string sFile = appPath + "\\test.json";
                //string jsonTEXT = System.IO.File.ReadAllText(sFile);
                PriceDetailJSON json = JsonConvert.DeserializeObject<PriceDetailJSON>(jsonTEXT);
                DateTime dt = Convert.ToDateTime(json.lt_dts);
                using (EcamContext context = new EcamContext())
                {
                    var row = (from q in context.tra_market
                               where q.symbol == json.t
                               && q.trade_date == dt.Date
                               && q.trade_type == json.e
                               select q).FirstOrDefault();
                    bool isNew = false;
                    if (row == null)
                    {
                        row = new tra_market();
                        isNew = true;
                    }
                    row.symbol = json.t;
                    row.trade_type = json.e;
                    row.trade_date = dt.Date;
                    row.close_price = Convert.ToDecimal(json.l_fix);

                    List<tra_market> markets;

                    DateTime startDate = row.trade_date.AddDays(-364);
                    DateTime endDate = row.trade_date.AddDays(-1);
                    markets = (from q in context.tra_market
                               where q.symbol == row.symbol
                               && q.trade_date >= startDate
                               && q.trade_date <= endDate
                               && q.trade_type == row.trade_type
                               select q).ToList();

                    var market = (from q in markets
                                  where q.trade_date < row.trade_date
                                  orderby q.trade_date descending
                                  select q).FirstOrDefault();
                    if (market != null)
                    {
                        row.prev_price = market.high_price;
                    }
                    market = (from q in markets
                              where q.trade_date >= row.trade_date.AddDays(-364)
                              && q.trade_date <= row.trade_date
                              orderby q.high_price descending, q.trade_date descending
                              select q).FirstOrDefault();
                    if (market != null)
                    {
                        row.week_52_high = market.high_price;
                    }
                    market = (from q in markets
                              where q.trade_date >= row.trade_date.AddMonths(-3)
                              && q.trade_date <= row.trade_date
                              orderby q.high_price descending, q.trade_date descending
                              select q).FirstOrDefault();
                    if (market != null)
                    {
                        row.months_3_high = market.high_price;
                    }
                    market = (from q in markets
                              where q.trade_date >= row.trade_date.AddMonths(-1)
                              && q.trade_date <= row.trade_date
                              orderby q.high_price descending, q.trade_date descending
                              select q).FirstOrDefault();
                    if (market != null)
                    {
                        row.months_1_high = market.high_price;
                    }
                    market = (from q in markets
                              where q.trade_date >= row.trade_date.AddDays(-5)
                              && q.trade_date <= row.trade_date
                              orderby q.high_price descending, q.trade_date descending
                              select q).FirstOrDefault();
                    if (market != null)
                    {
                        row.day_5_high = market.high_price;
                    }

                    if (isNew == true)
                    {
                        context.tra_market.Add(row);
                    }
                    else
                    {
                        context.Entry(row).State = System.Data.Entity.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdatePrice ex=" + ex.Message);
            }
        }
    }

    public class PriceDetailJSON
    {
        public string id { get; set; }
        public string t { get; set; }
        public string e { get; set; }
        public string l { get; set; }
        public string l_fix { get; set; }
        public string l_cur { get; set; }
        public string lt_dts { get; set; }
    }
}

