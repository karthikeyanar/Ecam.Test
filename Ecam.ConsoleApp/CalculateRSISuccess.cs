using Ecam.Framework;
using Ecam.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Ecam.ConsoleApp
{
    class CalculateRSISuccess
    {
        public static void Start()
        {
            string sql = "delete from tra_rsi_profit";
            MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString, sql);
            List<tra_company> companies = null;
            using (EcamContext context = new EcamContext())
            {
                companies = (from q in context.tra_company
                             ///where q.is_nifty_200 == true
                             orderby q.symbol ascending
                             select q).ToList();
            }
            foreach (tra_company company in companies)
            {
                List<tra_market> markets = null;
                using (EcamContext contex = new EcamContext())
                {
                    markets = (from q in contex.tra_market
                               where q.symbol == company.symbol
                               && (q.rsi ?? 0) > 0
                               && (q.rsi ?? 0) < 45
                               orderby q.trade_date ascending
                               select q).ToList();
                }
                foreach (var market in markets)
                {
                    List<tra_market> tempMarkets = null;
                    using (EcamContext contex = new EcamContext())
                    {
                        DateTime endDate = market.trade_date.AddDays(1000);
                        tempMarkets = (from q in contex.tra_market
                                       where q.symbol == market.symbol
                                       && q.trade_date >= market.trade_date
                                       && q.trade_date <= endDate
                                       orderby q.trade_date ascending
                                       select q).ToList();
                    }
                    int total = 30;
                    int i;
                    List<TempRSI> values = new List<TempRSI>();
                    List<decimal> tempValues = new List<decimal>();
                    if (tempMarkets.Count < total)
                    {
                        string s = string.Empty;
                    }
                    else
                    {
                        for (i = 0; i < total; i++)
                        {
                            var tempMarket = tempMarkets[i];
                            values.Add(new TempRSI
                            {
                                id = tempMarket.id,
                                date = tempMarket.trade_date,
                                avg_upward = (tempMarket.avg_upward ?? 0),
                                avg_downward = (tempMarket.avg_downward ?? 0),
                                prev = 0, // (tempMarket.prev_price ?? 0),
                                close = (tempMarket.close_price ?? 0)
                            });
                            tempValues.Add((tempMarket.close_price ?? 0));
                        }
                    }

                    if (values.Count > 0)
                    {
                        bool isSuccess = false;
                        TempRSI value = (from q in values where q.close > market.close_price orderby q.close descending select q).FirstOrDefault();
                        if (value == null)
                        {
                            value = (from q in values where q.close <= market.close_price orderby q.close descending select q).FirstOrDefault();
                        }
                        else
                        {
                            isSuccess = true;
                        }
                        if (value != null)
                        {
                            
                            decimal profit = (((value.close - (market.close_price ?? 0)) / (market.close_price ?? 0)) * 100);
                            using (EcamContext context = new EcamContext())
                            {
                                tra_rsi_profit rsi = (from q in context.tra_rsi_profit
                                                      where q.symbol == market.symbol
                                                      && q.buy_date == market.trade_date
                                                      && q.sell_date == value.date
                                                      select q).FirstOrDefault();
                                rsi = null;
                                bool isNew = false;
                                if (rsi == null)
                                {
                                    rsi = new tra_rsi_profit();
                                    isNew = true;
                                }
                                rsi.buy_date = market.trade_date;
                                rsi.buy_price = market.close_price;
                                rsi.buy_rsi = market.rsi;
                                rsi.profit = profit;
                                rsi.sell_date = value.date;
                                rsi.sell_price = value.close;
                                rsi.sell_rsi = value.rsi;
                                rsi.symbol = market.symbol;
                                if (isNew == true)
                                {
                                    context.tra_rsi_profit.Add(rsi);
                                }
                                else
                                {
                                    context.Entry(rsi).State = System.Data.Entity.EntityState.Modified;
                                }
                                context.SaveChanges();
                            }
                            string msg = "Symbol=" + market.symbol + ",BUYRSI=" + market.rsi + ",SELLRSI=" + value.rsi + ",BuyPrice=" + market.close_price + ",SellPrice=" + value.close + ",BuyDate=" + market.trade_date.ToString("dd-MMM-yyyy") + ",SellDate=" + value.date.ToString("dd-MMM-yyyy") + ",Profit=" + profit;
                            Console.WriteLine(msg);
                            //Helper.Log(msg, (isSuccess == true ? "SUCCESS" + "_" + market.symbol : "FAIL" + "_" + market.symbol));
                        }
                    }
                }
            }
        }
    }
}
