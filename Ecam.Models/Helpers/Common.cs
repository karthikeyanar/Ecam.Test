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
using Ecam.Models.Helpers;
using Ecam.Contracts.Enums;
using Ecam.Framework.ExcelHelper;

namespace Ecam.Models
{

    public static class Common
    {
        public static PaginatedListResult<TRA_COMPANY> Get(TRA_COMPANY_SEARCH criteria, Paging paging)
        {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string prefix = "ct";
            string sqlFormat = "select {0} from tra_company " + prefix + " {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            //string role = Authentication.CurrentRole;

            if ((criteria.id ?? 0) > 0)
            {
                where.AppendFormat(" ct.company_id={0}", criteria.id);
            }
            else
            {
                where.AppendFormat(" ct.company_id>0 ");

                if (string.IsNullOrEmpty(criteria.company_name) == false)
                {
                    where.AppendFormat(" and ct.company_name like '%{0}%", criteria.company_name);
                }
            }

            if (string.IsNullOrEmpty(criteria.symbols) == false)
            {
                where.AppendFormat(" and ct.symbol in({0})", Helper.ConvertStringSQLFormat(criteria.symbols));
            }

            if (string.IsNullOrEmpty(criteria.categories) == false)
            {
                string categorySymbols = "-1";
                using (EcamContext context = new EcamContext())
                {
                    List<string> categoryList = Helper.ConvertStringList(criteria.categories);
                    List<tra_company_category> categories = null;
                    List<string> categorySymbolList = null;
                    categories = (from q in context.tra_company_category
                                  where categoryList.Contains(q.category_name) == true
                                  select q).ToList();
                    if ((criteria.is_all_category ?? false) == true)
                    {
                        categorySymbolList = new List<string>();
                        foreach (var row in categories)
                        {
                            var tempList = (from q in categories where q.symbol == row.symbol select q).ToList();
                            int selCnt = 0;
                            foreach (var tempRow in tempList)
                            {
                                foreach (string str in categoryList)
                                {
                                    if (string.IsNullOrEmpty(str) == false)
                                    {
                                        if (tempRow.category_name == str)
                                        {
                                            selCnt += 1;
                                        }
                                    }
                                }
                            }
                            if (selCnt == categoryList.Count)
                            {
                                categorySymbolList.Add(row.symbol);
                            }
                        }
                        categorySymbolList = categorySymbolList.Distinct().ToList();
                    }
                    else
                    {
                        categorySymbolList = (from q in categories select q.symbol).Distinct().ToList();
                    }
                    if (categorySymbolList.Count > 0)
                    {
                        categorySymbols = "";
                    }
                    foreach (var str in categorySymbolList)
                    {
                        categorySymbols += str + ",";
                    }
                    if (string.IsNullOrEmpty(categorySymbols) == false)
                    {
                        categorySymbols = categorySymbols.Substring(0, categorySymbols.Length - 1);
                    }
                }
                if (string.IsNullOrEmpty(categorySymbols) == false)
                {
                    where.AppendFormat(" and ct.symbol in({0})", Helper.ConvertStringSQLFormat(categorySymbols));
                }
            }

            if (string.IsNullOrEmpty(criteria.mf_ids) == false)
            {
                string symbols = "";
                using (EcamContext context = new EcamContext())
                {
                    List<int> ids = Helper.ConvertIntIds(criteria.mf_ids);
                    if (ids.Count > 0)
                    {
                        var list = (from q in context.tra_mutual_fund_pf
                                    where ids.Contains(q.fund_id) == true
                                    select q.symbol).Distinct().ToList();
                        foreach (var str in list)
                        {
                            symbols += str + ",";
                        }
                        if (string.IsNullOrEmpty(symbols) == false)
                        {
                            symbols = symbols.Substring(0, symbols.Length - 1);
                        }
                    }
                }
                if (string.IsNullOrEmpty(symbols) == false)
                {
                    where.AppendFormat(" and ct.symbol in({0})", Helper.ConvertStringSQLFormat(symbols));
                }
                else
                {
                    where.Append(" and ct.symbol='-1'");
                }
            }

            //if (criteria.is_archive.HasValue)
            //{
            where.AppendFormat(" and ifnull(ct.is_archive,0)={0}", ((criteria.is_archive ?? false) == true ? "1" : "0"));
            //}

            if (criteria.is_book_mark.HasValue)
            {
                where.AppendFormat(" and ifnull(ct.is_book_mark,0)={0}", ((criteria.is_book_mark ?? false) == true ? "1" : "0"));
            }

            if (criteria.from_price.HasValue)
            {
                where.AppendFormat(" and ifnull(ct.close_price,0)>={0}", criteria.from_price);
            }

            if (criteria.to_price.HasValue)
            {
                where.AppendFormat(" and ifnull(ct.close_price,0)<={0}", criteria.to_price);
            }


            if (criteria.from_rsi.HasValue)
            {
                where.AppendFormat(" and ifnull(ct.rsi,0)>={0}", criteria.from_rsi);
            }

            if (criteria.to_rsi.HasValue)
            {
                where.AppendFormat(" and ifnull(ct.rsi,0)<={0}", criteria.to_rsi);
            }

            if (criteria.from_prev_rsi.HasValue)
            {
                where.AppendFormat(" and ifnull(ct.prev_rsi,0)>={0}", criteria.from_prev_rsi);
            }

            if (criteria.to_prev_rsi.HasValue)
            {
                where.AppendFormat(" and ifnull(ct.prev_rsi,0)<={0}", criteria.to_prev_rsi);
            }

            if (criteria.is_nifty_50.HasValue)
            {
                where.AppendFormat(" and ifnull(ct.is_nifty_50,0)={0}", ((criteria.is_nifty_50 ?? false) == true ? "1" : "0"));
            }

            if (criteria.is_nifty_100.HasValue)
            {
                where.AppendFormat(" and ifnull(ct.is_nifty_100,0)={0}", ((criteria.is_nifty_100 ?? false) == true ? "1" : "0"));
            }

            if (criteria.is_nifty_200.HasValue)
            {
                where.AppendFormat(" and ifnull(ct.is_nifty_200,0)={0}", ((criteria.is_nifty_200 ?? false) == true ? "1" : "0"));
            }

            if (string.IsNullOrEmpty(criteria.ignore_symbols) == false)
            {
                where.AppendFormat(" and ifnull(ct.symbol,0) not in({0})", Helper.ConvertStringSQLFormat(criteria.ignore_symbols));
            }

            if ((criteria.is_current_stock ?? false) == true)
            {
                joinTables += " join tra_holding h on h.symbol = ct.symbol ";
            }

            selectFields = "count(*) as cnt";

            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, "", "");

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString, sql));

            if (string.IsNullOrEmpty(paging.SortOrder))
            {
                paging.SortOrder = "asc";
            }

            if (paging.PageSize > 0)
            {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}", from, to);
            }

            orderBy = string.Format("order by {0} {1}", paging.SortName, paging.SortOrder);

            string dateFilter = string.Empty;
            DateTime? minDate = Convert.ToDateTime("01/01/1900");
            if (criteria.start_date.HasValue && criteria.end_date.HasValue)
            {
                dateFilter = string.Format(" and m.trade_date>='{0}' and m.trade_date<='{1}' ", criteria.start_date.Value.ToString("yyyy-MM-dd"), criteria.end_date.Value.ToString("yyyy-MM-dd"));
            }

            if (criteria.total_start_date.HasValue == false)
                criteria.total_start_date = DateTime.Now.Date.AddDays(-(365 * 3));
            if (criteria.total_end_date.HasValue == false)
                criteria.total_end_date = DateTime.Now.Date;

            string totalDateFilter = string.Empty;
            totalDateFilter = string.Format(" and m.trade_date>='{0}' and m.trade_date<='{1}' "
                , criteria.total_start_date.Value.ToString("yyyy-MM-dd")
                , criteria.total_end_date.Value.ToString("yyyy-MM-dd")) + Environment.NewLine;

            string totalDateAVGFilter = string.Empty;
            totalDateAVGFilter = string.Format(" and m.avg_date>='{0}' and m.avg_date<='{1}' "
                , DataTypeHelper.GetFirstDayOfMonth(criteria.total_start_date.Value).ToString("yyyy-MM-dd")
                , DataTypeHelper.GetLastDayOfMonth(criteria.total_end_date.Value).ToString("yyyy-MM-dd")) + Environment.NewLine;

            //if (criteria.trigger_start_date.HasValue == false)
            //    criteria.trigger_start_date = criteria.total_start_date;
            //if (criteria.trigger_end_date.HasValue == false)
            //    criteria.trigger_end_date = criteria.total_end_date;

            //string triggerDateFilter = string.Empty;
            //triggerDateFilter = string.Format(" and m.trade_date>='{0}' and m.trade_date<='{1}' "
            //    , criteria.trigger_start_date.Value.ToString("yyyy-MM-dd")
            //    , criteria.trigger_end_date.Value.ToString("yyyy-MM-dd")) + Environment.NewLine;

            selectFields = "ct.company_id" + Environment.NewLine +
                           ",ct.company_name" + Environment.NewLine +
                           ",ct.symbol" + Environment.NewLine +
                           ",ct.open_price" + Environment.NewLine +
                           ",ct.high_price" + Environment.NewLine +
                           ",ct.low_price" + Environment.NewLine +
                           ",ct.ltp_price" + Environment.NewLine +
                           ",ct.close_price" + Environment.NewLine +
                           ",ct.prev_price" + Environment.NewLine +
                           ",ct.week_52_high" + Environment.NewLine +
                           ",ct.week_52_low" + Environment.NewLine +
                           ",ct.is_archive" + Environment.NewLine +
                           ",ct.is_book_mark" + Environment.NewLine +
                           ",ct.is_nifty_50" + Environment.NewLine +
                           ",ct.is_nifty_100" + Environment.NewLine +
                           ",ct.is_nifty_200" + Environment.NewLine +
                           ",ct.rsi" + Environment.NewLine +
                           ",ct.prev_rsi" + Environment.NewLine +
                           ",ct.monthly_avg" + Environment.NewLine +
                           ",ct.weekly_avg" + Environment.NewLine +
                           ",ct.mc" + Environment.NewLine +
                           ",ct.pe" + Environment.NewLine +
                           ",ct.volume" + Environment.NewLine +
                           ",ct.eps" + Environment.NewLine +
                            //",(select high_price from tra_market m where m.symbol = ct.symbol " + totalDateFilter + " and m.high_price > 0 order by m.high_price desc limit 0,1) as total_high_price" + Environment.NewLine +
                            //",(select low_price from tra_market m where m.symbol = ct.symbol " + totalDateFilter + " and m.low_price > 0 order by m.low_price asc limit 0,1) as total_low_price" + Environment.NewLine +
                            //",(select high_price from tra_market m where m.symbol = ct.symbol " + dateFilter + " and m.high_price > 0 order by m.high_price desc limit 0,1) as profit_high_price" + Environment.NewLine +
                            //",(select low_price from tra_market m where m.symbol = ct.symbol " + dateFilter + " and m.low_price > 0 order by m.low_price asc limit 0,1) as profit_low_price" + Environment.NewLine +
                            //",(select ifnull(rsi,0) from tra_market m where m.symbol = ct.symbol " + dateFilter + " order by m.trade_date desc limit 0,1) as profit_rsi" + Environment.NewLine +
                            //",(select ifnull(rsi,0) from tra_market m where m.symbol = ct.symbol " + totalDateFilter + " order by m.trade_date desc limit 0,1) as total_rsi" + Environment.NewLine +

                            ",(select open_price from tra_market m where m.symbol = ct.symbol " + totalDateFilter + " order by m.trade_date asc limit 0,1) as total_first_price" + Environment.NewLine +
                            ",(select ltp_price from tra_market m where m.symbol = ct.symbol " + totalDateFilter + " order by m.trade_date desc limit 0,1) as total_last_price" + Environment.NewLine +

                            //",(select open_price from tra_market m where m.symbol = ct.symbol " + triggerDateFilter + " order by m.trade_date asc limit 0,1) as trigger_first_price" + Environment.NewLine +
                            //",(select ltp_price from tra_market m where m.symbol = ct.symbol " + triggerDateFilter + " order by m.trade_date desc limit 0,1) as trigger_last_price" + Environment.NewLine +

                            ",(select open_price from tra_market m where m.symbol = ct.symbol " + dateFilter + " order by m.trade_date asc limit 0,1) as first_price" + Environment.NewLine +
                            ",(select ltp_price from tra_market m where m.symbol = ct.symbol " + dateFilter + " order by m.trade_date desc limit 0,1) as last_price" + Environment.NewLine +
                            //",(select count(*) from tra_market_avg m where m.symbol = ct.symbol and m.avg_type = 'M' " + totalDateAVGFilter + " and ifnull(m.percentage, 0) < 0) as negative" + Environment.NewLine +
                            //",(select count(*) from tra_market_avg m where m.symbol = ct.symbol and m.avg_type = 'M' " + totalDateAVGFilter + "  and ifnull(m.percentage, 0) > 0) as positive" + Environment.NewLine +
                            //",(select count(*) from tra_market_avg m where m.symbol = ct.symbol and m.avg_type = 'M' " + totalDateAVGFilter + " ) as total" + Environment.NewLine +
                           "";

            //if (string.IsNullOrEmpty(criteria.mf_ids) == false)
            //{
            //    //selectFields += ",(select count(mpf.fund_id) from tra_mutual_fund_pf mpf where mpf.symbol = ct.symbol and mpf.fund_id in(" + criteria.mf_ids + ")) as mf_cnt" + Environment.NewLine +
            //    //    ",(select sum(ifnull(mpf.stock_value, 0)) from tra_mutual_fund_pf mpf where mpf.symbol = ct.symbol and mpf.fund_id in(" + criteria.mf_ids + ")) as mf_qty" + Environment.NewLine +
            //    //    "";
            //}
            //else
            //{
            //    //selectFields += ",(select count(mpf.fund_id) from tra_mutual_fund_pf mpf where mpf.symbol = ct.symbol) as mf_cnt" + Environment.NewLine +
            //    //   ",(select sum(ifnull(mpf.stock_value, 0)) from tra_mutual_fund_pf mpf where mpf.symbol = ct.symbol) as mf_qty" + Environment.NewLine +
            //    //   "";
            //}

            selectFields += "" +
                           ",(((ifnull(ct.ltp_price, 0) - ifnull(ct.prev_price, 0)) / ifnull(ct.prev_price, 0)) * 100) as prev_percentage" + Environment.NewLine +
                           ",(ifnull(ct.open_price,0) - ifnull(ct.prev_price,0)) as diff" + Environment.NewLine +
                           //",(ifnull(ct.rsi,0) - ifnull(ct.prev_rsi,0)) as rsi_diff" + Environment.NewLine +

                           ",(((ifnull(ct.ltp_price, 0) - ifnull(ct.open_price, 0)) / ifnull(ct.open_price, 0)) * 100) as ltp_percentage" + Environment.NewLine +
                           ",(((ifnull(ct.high_price, 0) - ifnull(ct.open_price, 0)) / ifnull(ct.open_price, 0)) * 100) as high_percentage" + Environment.NewLine +
                           ",(((ifnull(ct.low_price, 0) - ifnull(ct.open_price, 0)) / ifnull(ct.open_price, 0)) * 100) as low_percentage" + Environment.NewLine +

                           //",(((ifnull(ct.ltp_price, 0) - ifnull(ct.week_52_high, 0)) / ifnull(ct.week_52_high, 0)) * 100) as week_52_percentage" + Environment.NewLine +
                           //",(((ifnull(ct.week_52_high, 0) - ifnull(ct.ltp_price, 0)) / ifnull(ct.ltp_price, 0)) * 100) as week_52_positive_percentage" + Environment.NewLine +
                           //",(((ifnull(ct.ltp_price, 0) - ifnull(ct.week_52_low, 0)) / ifnull(ct.week_52_low, 0)) * 100) as week_52_low_percentage" + Environment.NewLine +
                           ",ct.company_id as id" + Environment.NewLine +
                           "";

            sql = string.Format(sqlFormat, selectFields, joinTables, where, "", "", "");

            where = new StringBuilder();

            where.AppendFormat(" where company_id > 0");

            if ((criteria.is_sell_to_buy ?? false) == true)
            {
                where.AppendFormat(" and ifnull(open_price,0)>=ifnull(low_price,0)");
                where.AppendFormat(" and ifnull(open_price,0)>=ifnull(high_price,0)");
                //where.AppendFormat(" and ifnull(high_percentage,0)>=0");
                //where.AppendFormat(" and ifnull(high_percentage,0)<=0.5");
            }

            if ((criteria.is_buy_to_sell ?? false) == true)
            {
                where.AppendFormat(" and ifnull(open_price,0)<=ifnull(high_price,0)");
                //where.AppendFormat(" and ifnull(low_percentage,0)<=0");
                //where.AppendFormat(" and ifnull(low_percentage,0)>=-0.5");
                where.AppendFormat(" and ifnull(open_price,0)<=ifnull(low_price,0)");
            }

            if (string.IsNullOrEmpty(criteria.ltp_from_percentage) == false)
            {
                where.AppendFormat(" and ifnull(ltp_percentage,0)>={0}", criteria.ltp_from_percentage);
            }

            if (string.IsNullOrEmpty(criteria.ltp_to_percentage) == false)
            {
                where.AppendFormat(" and ifnull(ltp_percentage,0)<={0}", criteria.ltp_to_percentage);
            }

            if ((criteria.from_profit ?? 0) != 0)
            {
                where.AppendFormat(" and ifnull((((last_price - first_price)/first_price) * 100),0)>={0}", criteria.from_profit);
            }

            if ((criteria.to_profit ?? 0) != 0)
            {
                where.AppendFormat(" and ifnull((((last_price - first_price)/first_price) * 100),0)<={0}", criteria.to_profit);
            }

            if ((criteria.total_from_profit ?? 0) != 0)
            {
                where.AppendFormat(" and ifnull((((total_last_price - total_first_price)/total_first_price) * 100),0)>={0}", criteria.total_from_profit);
            }

            if ((criteria.total_to_profit ?? 0) != 0)
            {
                where.AppendFormat(" and ifnull((((total_last_price - total_first_price)/total_first_price) * 100),0)<={0}", criteria.total_to_profit);
            }

            //if ((criteria.trigger_from_profit ?? 0) != 0)
            //{
            //    where.AppendFormat(" and ifnull((((trigger_last_price - trigger_first_price)/trigger_first_price) * 100),0)>={0}", criteria.trigger_from_profit);
            //}

            //if ((criteria.trigger_to_profit ?? 0) != 0)
            //{
            //    where.AppendFormat(" and ifnull((((trigger_last_price - trigger_first_price)/trigger_first_price) * 100),0)<={0}", criteria.trigger_to_profit);
            //}

            //if ((criteria.max_negative_count ?? -1) >= 0)
            //{
            //    where.AppendFormat(" and ifnull(negative,0)<={0}", criteria.max_negative_count).Append(Environment.NewLine);
            //}

            string tempsql = string.Format("select " +
       "tbl.*" + Environment.NewLine +
       " from(" + Environment.NewLine +
       sql + Environment.NewLine +
       ") as tbl {0} {1} {2} {3} ", where, "", "", "");

            tempsql = "select count(*) as cnt from(" + tempsql + ") as tbl2";

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString, tempsql));

            sql = string.Format("select " +
            "(((last_price - first_price)/first_price) * 100) as profit" + Environment.NewLine +
            ",(((total_last_price - total_first_price)/total_first_price) * 100) as total_profit" + Environment.NewLine +
            //",(((trigger_last_price - trigger_first_price)/trigger_first_price) * 100) as trigger_profit" + Environment.NewLine +
            //",(negative/(negative+positive) * 100) as negative_percentage" + Environment.NewLine +
            //",(positive/(negative+positive) * 100) as positive_percentage" + Environment.NewLine +
            //",(((profit_high_price - ltp_price)/ltp_price) * 100) as profit_high_percentage" + Environment.NewLine +
            //",(((profit_low_price - ltp_price)/ltp_price) * 100) as profit_low_percentage" + Environment.NewLine +
            ",tbl.*" + Environment.NewLine +
            " from(" + Environment.NewLine +
            sql + Environment.NewLine +
            ") as tbl {0} {1} {2} {3} ", where, "", orderBy, pageLimit);

            Helper.Log(sql);
            List<TRA_COMPANY> rows = new List<TRA_COMPANY>();
            List<tra_company_category> companyCategories;
            using (EcamContext context = new EcamContext())
            {
                rows = context.Database.SqlQuery<TRA_COMPANY>(sql).ToList();
                List<string> symbols = (from q in rows select q.symbol).ToList();
                companyCategories = (from q in context.tra_company_category
                                     where symbols.Contains(q.symbol) == true
                                     select q).ToList();
            }

            foreach (var row in rows)
            {
                row.category_list = (from q in companyCategories
                                     where q.symbol == row.symbol
                                     select q.category_name).ToList();
                string categoryName = "";
                foreach (var cat in row.category_list)
                {
                    categoryName += cat + ",";
                }
                if (string.IsNullOrEmpty(categoryName) == false)
                {
                    categoryName = categoryName.Substring(0, categoryName.Length - 1);
                }
                row.category_name = categoryName;
            }
            foreach (var row in rows)
            {
                if ((row.first_price ?? 0) <= 0)
                {
                    row.first_price = row.total_last_price;
                }
                if ((row.last_price ?? 0) <= 0)
                {
                    row.last_price = row.total_last_price;
                }
                if ((row.high_price ?? 0) <= 0)
                {
                    row.high_price = row.total_last_price;
                }
                if ((row.low_price ?? 0) <= 0)
                {
                    row.low_price = row.total_last_price;
                }
                if ((row.last_price ?? 0) <= 0)
                {
                    row.profit = DataTypeHelper.SafeDivision(((row.last_price ?? 0) - (row.first_price ?? 0)), (row.first_price ?? 0)) * 100;
                }
            }
            return new PaginatedListResult<TRA_COMPANY> { total = paging.Total, rows = rows };
        }

        public static List<BatchLog> GetBatchLog(TRA_COMPANY_SEARCH criteria, Paging paging)
        {
            DateTime minDate = Convert.ToDateTime("01/01/1900");
            List<BatchLog> logs = new List<BatchLog>();
            List<TRA_COMPANY> list = Common.Get(criteria, paging).rows.ToList();

            List<string> symbols = (from q in list
                                    select q.symbol).Distinct().ToList();
            List<tra_market> markets;
            using (EcamContext context = new EcamContext())
            {
                markets = (from q in context.tra_market
                           where symbols.Contains(q.symbol) == true
                           && q.trade_date >= criteria.start_date
                           && q.trade_date <= criteria.end_date
                           select q).ToList();
            }

            DateTime monthStartDate = DataTypeHelper.GetFirstDayOfMonth(criteria.end_date ?? minDate);
            DateTime monthEndDate = DataTypeHelper.GetLastDayOfMonth(criteria.end_date ?? minDate);
            List<Weeks> weeks = DataTypeHelper.GetMonthWeeks(monthStartDate);
            decimal firstPrice, lastPrice, percentage;

            foreach (var row in list)
            {
                foreach (var week in weeks)
                {
                    BatchLog log = (from q in logs
                                    where q.symbol == row.symbol
                                    && q.batch_index == paging.PageIndex
                                    && q.date == monthStartDate
                                    select q).FirstOrDefault();
                    if (log == null)
                    {
                        log = new BatchLog
                        {
                            batch_index = paging.PageIndex,
                            symbol = row.symbol,
                            date = monthStartDate
                        };
                        firstPrice = (from q in markets
                                      where q.symbol == row.symbol
                                      && q.trade_date >= monthStartDate
                                      && q.trade_date <= monthEndDate
                                      orderby q.trade_date ascending
                                      select (q.open_price ?? 0)).FirstOrDefault();
                        lastPrice = (from q in markets
                                     where q.symbol == row.symbol
                                     && q.trade_date >= monthStartDate
                                     && q.trade_date <= monthEndDate
                                     orderby q.trade_date descending
                                     select (q.close_price ?? 0)).FirstOrDefault();
                        log.first_price = firstPrice;
                        log.last_price = lastPrice;
                        percentage = DataTypeHelper.SafeDivision((lastPrice - firstPrice), firstPrice) * 100;
                        log.percentage = percentage;
                        logs.Add(log);
                    }
                    if (log != null)
                    {
                        firstPrice = (from q in markets
                                      where q.symbol == row.symbol
                                      && q.trade_date >= week.first_date
                                      && q.trade_date <= week.last_date
                                      orderby q.trade_date ascending
                                      select (q.open_price ?? 0)).FirstOrDefault();
                        lastPrice = (from q in markets
                                     where q.symbol == row.symbol
                                     && q.trade_date >= week.first_date
                                     && q.trade_date <= week.last_date
                                     orderby q.trade_date descending
                                     select (q.close_price ?? 0)).FirstOrDefault();
                        percentage = DataTypeHelper.SafeDivision((lastPrice - firstPrice), firstPrice) * 100;
                        switch (week.week_number)
                        {
                            case 1:
                                log.w1_first_price = firstPrice;
                                log.w1_last_price = lastPrice;
                                log.w1 = percentage;
                                break;
                            case 2:
                                log.w2_first_price = firstPrice;
                                log.w2_last_price = lastPrice;
                                log.w2 = percentage;
                                break;
                            case 3:
                                log.w3_first_price = firstPrice;
                                log.w3_last_price = lastPrice;
                                log.w3 = percentage;
                                break;
                            case 4:
                                log.w4_first_price = firstPrice;
                                log.w4_last_price = lastPrice;
                                log.w4 = percentage;
                                break;
                            case 5:
                                log.w5_first_price = firstPrice;
                                log.w5_last_price = lastPrice;
                                log.w5 = percentage;
                                break;
                        }
                    }
                }
            }
            foreach (var log in logs)
            {
                if (log.w1_first_price <= 0 && log.w1_last_price <= 0)
                {
                    log.w1_first_price = log.w2_first_price;
                    log.w1_last_price = log.w2_last_price;
                    log.w1 = DataTypeHelper.SafeDivision((log.w1_last_price - log.w1_first_price), log.w1_first_price) * 100;
                }
            }
            logs = (from q in logs
                    orderby q.percentage descending
                    select q).ToList();
            return logs;
        }

        public static List<DailySummary> GetDailySummary(TRA_COMPANY_SEARCH criteria, Paging paging)
        {
            DateTime minDate = Convert.ToDateTime("01/01/1900");
            List<DailySummary> logs = new List<DailySummary>();
            List<TRA_COMPANY> list = Common.Get(criteria, paging).rows.ToList();

            List<string> symbols = (from q in list
                                    select q.symbol).Distinct().ToList();
            List<tra_market> markets;
            using (EcamContext context = new EcamContext())
            {
                markets = (from q in context.tra_market
                           where symbols.Contains(q.symbol) == true
                           && q.trade_date >= criteria.start_date
                           && q.trade_date <= criteria.end_date
                           select q).ToList();
            }

            DateTime monthStartDate = DataTypeHelper.GetFirstDayOfMonth(criteria.end_date ?? minDate);
            DateTime monthLastDate = DataTypeHelper.GetLastDayOfMonth(criteria.end_date ?? minDate);
            TimeSpan ts = monthLastDate - monthStartDate;
            List<Weeks> weeks = new List<Weeks>();
            int i;
            for (i = 0; i < ts.TotalDays + 1; i++)
            {
                DateTime dt = monthStartDate.AddDays(i);

                DailySummary log = (from q in logs
                                    where q.date == dt
                                    select q).FirstOrDefault();
                if (log == null)
                {
                    log = new DailySummary
                    {
                        date = dt,
                    };
                    logs.Add(log);
                }
                if (log != null)
                {
                    foreach (string symbol in symbols)
                    {
                        DailyLog daily = (from q in log.logs
                                          where q.symbol == symbol
                                          && q.date == dt
                                          select q).FirstOrDefault();
                        if (daily == null)
                        {
                            daily = new DailyLog
                            {
                                symbol = symbol,
                                buy_price = 0,
                                close_price = 0,
                                ltp_price = 0,
                                date = dt,
                                high_price = 0,
                                total_amount = (criteria.total_amount ?? 0),
                                total_equity = symbols.Count(),
                                low_price = 0
                            };
                            log.logs.Add(daily);
                        }
                        daily.date = dt;
                        daily.buy_price = (from q in markets
                                           where q.symbol == symbol
                                           && q.trade_date >= monthStartDate
                                           orderby q.trade_date ascending
                                           select (q.open_price ?? 0)).FirstOrDefault();
                        var market = (from q in markets
                                      where q.symbol == symbol
                                      && q.trade_date == dt
                                      select q).FirstOrDefault();
                        if (market == null)
                        {
                            market = (from q in markets
                                      where q.symbol == symbol
                                      && q.trade_date <= dt
                                      orderby q.trade_date descending
                                      select q).FirstOrDefault();
                        }
                        if (market != null)
                        {
                            daily.close_price = (market.close_price ?? 0);
                            daily.ltp_price = (market.ltp_price ?? 0);
                            daily.high_price = (market.high_price ?? 0);
                            daily.low_price = (market.low_price ?? 0);
                        }
                    }
                }

            }
            logs = (from q in logs
                    orderby q.date ascending
                    select q).ToList();
            return logs;
        }

        public static void CreateCategoryProfit()
        {
            List<tra_category> categories;
            using (EcamContext context = new EcamContext())
            {
                categories = (from q in context.tra_category
                              orderby q.category_name ascending
                              select q).ToList();
            }
            foreach (var category in categories)
            {
                Common.CreateCategoryProfit(category.category_name, 2016);
                Common.CreateCategoryProfit(category.category_name, 2017);
            }
        }

        public static void CreateCategoryProfit(string categoryName, int year)
        {
            DateTime yearStartDate = Convert.ToDateTime("01/01/" + year);
            decimal initialAmount = 500000;
            decimal totalAmount = initialAmount;
            decimal monthlyInvestment = 20000;
            int totalMonths = 12;
            int i;
            for (i = 1; i <= totalMonths; i++)
            {
                DateTime dt = Convert.ToDateTime(i + "/01/" + year);
                DateTime startDate = DataTypeHelper.GetFirstDayOfMonth(dt);
                DateTime endDate = DataTypeHelper.GetLastDayOfMonth(dt);
                DateTime totalStartDate = startDate.AddMonths(-6);
                DateTime totalEndDate = DataTypeHelper.GetLastDayOfMonth(totalStartDate.AddMonths(6).AddDays(-7));

                //Console.WriteLine("StartDate=" + startDate.ToString("dd/MMM/yyyyy") + ",EndDate=" + endDate.ToString("dd/MMM/yyyyy"));
                List<TRA_COMPANY> list = Common.Get(new TRA_COMPANY_SEARCH
                {
                    categories = categoryName,
                    start_date = startDate,
                    end_date = endDate,
                    total_start_date = totalStartDate,
                    total_end_date = totalEndDate,
                    total_amount = totalAmount,
                    monthly_investment = monthlyInvestment,
                    max_negative_count = -1,
                }, new Paging
                {
                    PageIndex = 1,
                    PageSize = 10,
                    SortName = "total_profit",
                    SortOrder = "desc",
                }).rows.ToList();

                int totalEquity = list.Count();
                decimal totalInvestmentPerEquity = (totalAmount / totalEquity);
                decimal totalInvestment = 0;
                decimal totalCurrentValue = 0;
                decimal positiveCount = 0;
                decimal negativeCount = 0;

                //decimal highCurrentValue = 0;
                //decimal lowCurrentValue = 0;
                decimal balance = 0; decimal profit = 0; //decimal highProfit = 0; decimal lowProfit = 0;

                List<Investment> investments = new List<Investment>();
                string symbols = "";

                if (list != null)
                {
                    foreach (var row in list)
                    {
                        symbols += row.symbol + ",";

                        if ((row.first_price ?? 0) <= 0) { row.first_price = 1; }
                        if ((row.last_price ?? 0) <= 0) { row.last_price = 1; }
                        //if ((row.profit_high_price ?? 0) <= 0) { row.profit_high_price = 1; }
                        //if ((row.profit_low_price ?? 0) <= 0) { row.profit_low_price = 1; }

                        int quantity = (int)(totalInvestmentPerEquity / row.first_price);
                        decimal investment = (quantity * (row.first_price ?? 0));
                        decimal cmv = quantity * (row.last_price ?? 0);
                        //decimal high_cmv = quantity * (row.profit_high_price ?? 0);
                        //decimal low_cmv = quantity * (row.profit_low_price ?? 0);
                        profit = (((cmv - investment) / investment) * 100);
                        //highProfit = (((high_cmv - investment) / investment) * 100);
                        //lowProfit = (((low_cmv - investment) / investment) * 100);

                        investments.Add(new Models.Investment
                        {
                            symbol = row.symbol,
                            quantity = quantity,
                            investment = investment,
                            cmv = cmv,
                            //high_cmv = high_cmv,
                            //low_cmv = low_cmv,
                            profit = profit,
                            //high_profit = highProfit,
                            //low_profit = lowProfit,
                            first_price = (row.first_price ?? 0),
                            last_price = (row.last_price ?? 0),
                            //profit_high_price = (row.profit_high_price ?? 0),
                            //profit_low_price = (row.profit_low_price ?? 0)
                        });
                    }


                    foreach (var row in investments)
                    {
                        totalInvestment += (row.investment);
                        totalCurrentValue += (row.cmv);

                        //highCurrentValue += (row.high_cmv);
                        //lowCurrentValue += (row.low_cmv);

                        if (row.profit > 0)
                        {
                            positiveCount += 1;
                        }
                        else
                        {
                            negativeCount += 1;
                        }
                    }

                    if (investments.Count <= 0)
                    {
                        totalInvestment = totalAmount;
                        totalCurrentValue = totalAmount;
                        //highCurrentValue = totalAmount;
                        //lowCurrentValue = totalAmount;
                    }


                    totalCurrentValue = Common.CalcFinalMarketValue(totalCurrentValue, investments.Count, false);
                    //highCurrentValue = Common.CalcFinalMarketValue(highCurrentValue, investments.Count, false);
                    //lowCurrentValue = Common.CalcFinalMarketValue(lowCurrentValue, investments.Count, false);

                    decimal totalProfitAVG = ((totalCurrentValue - totalInvestment) / totalInvestment) * 100;

                    balance = 0; profit = 0; //highProfit = 0; lowProfit = 0;

                    balance = (totalAmount - totalInvestment);

                    balance = balance - (Common.GetCharges(totalInvestment, investments.Count, true));

                    profit = ((totalCurrentValue - totalInvestment) / totalInvestment) * 100;
                    //highProfit = ((highCurrentValue - totalInvestment) / totalInvestment) * 100;
                    //lowProfit = ((lowCurrentValue - totalInvestment) / totalInvestment) * 100;

                    totalAmount = (totalCurrentValue) + (balance) + monthlyInvestment;

                    decimal totalFinalAmount = initialAmount + (monthlyInvestment * i);

                    decimal yearProfit = (((totalAmount) - (totalFinalAmount)) / (totalFinalAmount)) * 100;

                    using (EcamContext context = new EcamContext())
                    {
                        tra_category_profit categoryProfit = (from q in context.tra_category_profit
                                                              where q.category_name == categoryName
                                                              && q.profit_date == startDate
                                                              && q.profit_type == "M"
                                                              select q).FirstOrDefault();
                        bool isExist = false;
                        if (categoryProfit == null)
                        {
                            categoryProfit = new tra_category_profit();
                        }
                        else
                        {
                            isExist = true;
                        }
                        categoryProfit.category_name = categoryName;
                        categoryProfit.profit_date = startDate;
                        categoryProfit.profit_type = "M";
                        categoryProfit.profit = totalProfitAVG;
                        if (isExist == true)
                        {
                            context.Entry(categoryProfit).State = System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {
                            context.tra_category_profit.Add(categoryProfit);
                        }
                        context.SaveChanges();


                        categoryProfit = (from q in context.tra_category_profit
                                          where q.category_name == categoryName
                                          && q.profit_date == yearStartDate
                                          && q.profit_type == "Y"
                                          select q).FirstOrDefault();
                        isExist = false;
                        if (categoryProfit == null)
                        {
                            categoryProfit = new tra_category_profit();
                        }
                        else
                        {
                            isExist = true;
                        }
                        categoryProfit.category_name = categoryName;
                        categoryProfit.profit_date = yearStartDate;
                        categoryProfit.profit_type = "Y";
                        categoryProfit.profit = yearProfit;
                        if (isExist == true)
                        {
                            context.Entry(categoryProfit).State = System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {
                            context.tra_category_profit.Add(categoryProfit);
                        }
                        context.SaveChanges();
                    }
                }
            }
            Console.WriteLine("Completed category=" + categoryName + ",Year=" + year);
        }

        public static decimal CalcFinalMarketValue(decimal totalMarketValue, decimal totalEquity, bool isInvestment)
        {
            return ((totalMarketValue) - Common.GetCharges(totalMarketValue, totalEquity, isInvestment));
        }

        public static decimal GetCharges(decimal totalMarketValue, decimal totalEquity, bool isInvestment)
        {
            //totalMarketValue = cFloat(totalMarketValue);
            //totalEquity = cInt(totalEquity);
            decimal stt = (decimal)(totalMarketValue * (decimal)0.1) / 100;
            decimal txn = (decimal)(totalMarketValue * (decimal)0.00325) / 100;
            decimal gst = (txn * 18) / 100;
            decimal stamb = (decimal)(totalMarketValue * (decimal)0.006) / 100;
            decimal sebi = ((15 * totalMarketValue) / 10000000);
            decimal dpcharges = (decimal)(totalEquity * (decimal)15.93);
            if (isInvestment == true)
            {
                dpcharges = 0;
            }
            return (stt + txn + gst + stamb + sebi + dpcharges);
        }
    }

    public class Investment
    {
        public string symbol { get; set; }
        public int quantity { get; set; }
        public decimal investment { get; set; }
        public decimal cmv { get; set; }
        //public decimal high_cmv { get; set; }
        //public decimal low_cmv { get; set; }
        public decimal profit { get; set; }
        //public decimal high_profit { get; set; }
        //public decimal low_profit { get; set; }
        public decimal first_price { get; set; }
        public decimal last_price { get; set; }
        //public decimal profit_high_price { get; set; }
        //public decimal profit_low_price { get; set; }
    }

    public class DailySummary
    {
        public DailySummary()
        {
            this.logs = new List<DailyLog>();
        }
        public List<DailyLog> logs { get; set; }
        public DateTime date { get; set; }
        public decimal investment {
            get {
                return (from q in this.logs select q.buy_amount).Sum();
            }
        }
        public decimal cmv {
            get {
                return (from q in this.logs select q.cmv).Sum();
            }
        }
        public decimal cmv_high {
            get {
                return (from q in this.logs select q.cmv_high).Sum();
            }
        }
        public decimal cmv_low {
            get {
                return (from q in this.logs select q.cmv_low).Sum();
            }
        }
        public decimal percentage {
            get {
                return DataTypeHelper.SafeDivision((this.cmv - this.investment), this.investment) * 100;
            }
        }
        public decimal percentage_high {
            get {
                return DataTypeHelper.SafeDivision((this.cmv_high - this.investment), this.investment) * 100;
            }
        }
        public decimal percentage_low {
            get {
                return DataTypeHelper.SafeDivision((this.cmv_low - this.investment), this.investment) * 100;
            }
        }
    }

    public class DailyLog
    {
        public DateTime date { get; set; }
        public string symbol { get; set; }
        public decimal total_amount { get; set; }
        public decimal total_equity { get; set; }
        public decimal investment {
            get {
                return DataTypeHelper.SafeDivision(this.total_amount, this.total_equity);
            }
        }
        public decimal buy_price { get; set; }
        public int quantity {
            get {
                return (int)DataTypeHelper.SafeDivision(this.investment, this.buy_price);
            }
        }
        public decimal buy_amount {
            get {
                return this.quantity * this.buy_price;
            }
        }

        public decimal high_price { get; set; }
        public decimal low_price { get; set; }
        public decimal close_price { get; set; }
        public decimal ltp_price { get; set; }

        public decimal cmv {
            get {
                return this.quantity * this.ltp_price;
            }
        }
        public decimal cmv_high {
            get {
                return this.quantity * this.high_price;
            }
        }
        public decimal cmv_low {
            get {
                return this.quantity * this.low_price;
            }
        }
    }

    public class BatchLog
    {
        public DateTime date { get; set; }
        public int batch_index { get; set; }
        public string symbol { get; set; }

        public decimal w1 { get; set; }
        public decimal w1_first_price { get; set; }
        public decimal w1_last_price { get; set; }

        public decimal w2 { get; set; }
        public decimal w2_first_price { get; set; }
        public decimal w2_last_price { get; set; }
        public decimal w2_total_percentage {
            get {
                return DataTypeHelper.SafeDivision((this.w2_last_price - this.w1_first_price), this.w1_first_price) * 100;
            }
        }

        public decimal w3 { get; set; }
        public decimal w3_first_price { get; set; }
        public decimal w3_last_price { get; set; }
        public decimal w3_total_percentage {
            get {
                return DataTypeHelper.SafeDivision((this.w3_last_price - this.w1_first_price), this.w1_first_price) * 100;
            }
        }

        public decimal w4 { get; set; }
        public decimal w4_first_price { get; set; }
        public decimal w4_last_price { get; set; }
        public decimal w4_total_percentage {
            get {
                return DataTypeHelper.SafeDivision((this.w4_last_price - this.w1_first_price), this.w1_first_price) * 100;
            }
        }

        public decimal w5 { get; set; }
        public decimal w5_first_price { get; set; }
        public decimal w5_last_price { get; set; }
        public decimal w5_total_percentage {
            get {
                return DataTypeHelper.SafeDivision((this.w5_last_price - this.w1_first_price), this.w1_first_price) * 100;
            }
        }

        public decimal first_price { get; set; }
        public decimal last_price { get; set; }
        public decimal percentage { get; set; }
    }
}