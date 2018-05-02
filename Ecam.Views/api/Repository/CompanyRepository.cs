
using Ecam.Contracts;
using Ecam.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Framework.Repository {

    public interface ICompanyRepository {
        PaginatedListResult<TRA_COMPANY> Get(TRA_COMPANY_SEARCH criteria,Paging paging);
        PaginatedListResult<TRA_COMPANY> GetCompanies(TRA_COMPANY_SEARCH criteria,Paging paging);
        List<BatchLog> GetBatchLog(TRA_COMPANY_SEARCH criteria,Paging paging);
        List<DailySummary> GetDailySummary(TRA_COMPANY_SEARCH criteria,Paging paging);
        PaginatedListResult<TRA_COMPANY> GetMarketList(TRA_COMPANY_SEARCH criteria,Paging paging);
        PaginatedListResult<TRA_MARKET_INTRA_DAY> GetIntraDay(TRA_COMPANY_SEARCH criteria,Paging paging);
        PaginatedListResult<TRA_MARKET_AVG> GetAvg(TRA_COMPANY_SEARCH criteria,Paging paging);
        PaginatedListResult<TRA_MARKET_RSI> GetRSI(TRA_COMPANY_SEARCH criteria,Paging paging);
        List<Select2List> GetCompanys(string name,int pageSize = 50,string categories = "");
        List<Select2List> GetCategories(string name,int pageSize = 500);
        List<Select2List> GetMFFunds(string name,int pageSize = 50);
        PaginatedListResult<TRA_COMPANY> GetMonthlyAVG(TRA_COMPANY_SEARCH criteria,Paging paging);
    }

    public class CompanyRepository:ICompanyRepository {
        public List<Select2List> GetCategories(string name,int pageSize = 500) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from tra_category comp {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            where.AppendFormat(" comp.category_id > 0");

            if(string.IsNullOrEmpty(name) == false) {
                where.AppendFormat(" and comp.category_name like '{0}%'",name);
            }

            Paging paging = new Paging { PageIndex = 1,PageSize = pageSize };

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = "comp.category_name as id" +
                           ",comp.category_name as label" +
                           ",comp.category_name as value";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            List<Select2List> rows = new List<Select2List>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<Select2List>(sql).ToList();
            }
            return rows;
        }

        public List<Select2List> GetMFFunds(string name,int pageSize = 50) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from tra_mutual_fund comp {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            where.AppendFormat(" comp.mutual_fund_id > 0");

            if(string.IsNullOrEmpty(name) == false) {
                where.AppendFormat(" and comp.fund_name like '{0}%'",name);
            }

            Paging paging = new Paging { PageIndex = 1,PageSize = pageSize };

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = "comp.mutual_fund_id as id" +
                           ",comp.fund_name as label" +
                           ",comp.fund_name as value";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            List<Select2List> rows = new List<Select2List>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<Select2List>(sql).ToList();
            }
            return rows;
        }

        public List<Select2List> GetCompanys(string name,int pageSize = 50,string categories = "") {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from tra_company comp {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            where.AppendFormat(" comp.company_id > 0");

            if(string.IsNullOrEmpty(name) == false) {
                where.AppendFormat(" and comp.company_name like '{0}%' or comp.symbol like '{0}%' ",name);
            }

            if(string.IsNullOrEmpty(categories) == false) {
                string categorySymbols = "-1";
                using(EcamContext context = new EcamContext()) {
                    List<string> categoryList = Helper.ConvertStringList(categories);
                    List<string> categorySymbolList = (from q in context.tra_company_category
                                                       where categoryList.Contains(q.category_name) == true
                                                       select q.symbol).Distinct().ToList();
                    foreach(var str in categorySymbolList) {
                        categorySymbols += str + ",";
                    }
                    if(string.IsNullOrEmpty(categorySymbols) == false) {
                        categorySymbols = categorySymbols.Substring(0,categorySymbols.Length - 1);
                    }
                }
                if(string.IsNullOrEmpty(categorySymbols) == false) {
                    where.AppendFormat(" and comp.symbol in({0})",Helper.ConvertStringSQLFormat(categorySymbols));
                }
            }

            Paging paging = new Paging { PageIndex = 1,PageSize = pageSize };

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = "comp.symbol as id" +
                           ",comp.company_name as label" +
                           ",comp.company_name as value";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            List<Select2List> rows = new List<Select2List>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<Select2List>(sql).ToList();
            }
            return rows;
        }

        public PaginatedListResult<TRA_COMPANY> Get(TRA_COMPANY_SEARCH criteria,Paging paging) {
            return Common.Get(criteria,paging);
        }

        public PaginatedListResult<TRA_COMPANY> GetCompanies(TRA_COMPANY_SEARCH criteria,Paging paging) {
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

            if((criteria.id ?? 0) > 0) {
                where.AppendFormat(" ct.company_id={0}",criteria.id);
            } else {
                where.AppendFormat(" ct.company_id>0 ");

                if(string.IsNullOrEmpty(criteria.company_name) == false) {
                    where.AppendFormat(" and ct.company_name like '%{0}%",criteria.company_name);
                }


                if(string.IsNullOrEmpty(criteria.symbols) == false) {
                    where.AppendFormat(" and ct.symbol in({0})",Helper.ConvertStringSQLFormat(criteria.symbols));
                }

                List<string> categoryList = null;
                if((criteria.is_book_mark_category ?? false) == true) {
                    using(EcamContext context = new EcamContext()) {
                        var categoryNameList = (from q in context.tra_category where (q.is_book_mark ?? false) == true select q.category_name).ToList();
                        criteria.categories += Helper.ConvertStringIds(categoryNameList);
                    }
                }

                if(string.IsNullOrEmpty(criteria.categories) == false) {
                    categoryList = Helper.ConvertStringList(criteria.categories);
                }

                if(categoryList != null) {
                    if(categoryList.Count > 0) {
                        string categorySymbols = "-1";
                        using(EcamContext context = new EcamContext()) {
                            List<tra_company_category> categories = null;
                            List<string> categorySymbolList = null;
                            categories = (from q in context.tra_company_category
                                          where categoryList.Contains(q.category_name) == true
                                          select q).ToList();
                            if((criteria.is_all_category ?? false) == true) {
                                categorySymbolList = new List<string>();
                                foreach(var row in categories) {
                                    var tempList = (from q in categories where q.symbol == row.symbol select q).ToList();
                                    int selCnt = 0;
                                    foreach(var tempRow in tempList) {
                                        foreach(string str in categoryList) {
                                            if(string.IsNullOrEmpty(str) == false) {
                                                if(tempRow.category_name == str) {
                                                    selCnt += 1;
                                                }
                                            }
                                        }
                                    }
                                    if(selCnt == categoryList.Count) {
                                        categorySymbolList.Add(row.symbol);
                                    }
                                }
                                categorySymbolList = categorySymbolList.Distinct().ToList();
                            } else {
                                categorySymbolList = (from q in categories select q.symbol).Distinct().ToList();
                            }
                            if(categorySymbolList.Count > 0) {
                                categorySymbols = "";
                            }
                            foreach(var str in categorySymbolList) {
                                categorySymbols += str + ",";
                            }
                            if(string.IsNullOrEmpty(categorySymbols) == false) {
                                categorySymbols = categorySymbols.Substring(0,categorySymbols.Length - 1);
                            }
                        }
                        if(string.IsNullOrEmpty(categorySymbols) == false) {
                            where.AppendFormat(" and ct.symbol in({0})",Helper.ConvertStringSQLFormat(categorySymbols));
                        }
                    }
                }

                if(string.IsNullOrEmpty(criteria.mf_ids) == false) {
                    string symbols = "";
                    using(EcamContext context = new EcamContext()) {
                        List<int> ids = Helper.ConvertIntIds(criteria.mf_ids);
                        if(ids.Count > 0) {
                            var list = (from q in context.tra_mutual_fund_pf
                                        where ids.Contains(q.fund_id) == true
                                        select q.symbol).Distinct().ToList();
                            foreach(var str in list) {
                                symbols += str + ",";
                            }
                            if(string.IsNullOrEmpty(symbols) == false) {
                                symbols = symbols.Substring(0,symbols.Length - 1);
                            }
                        }
                    }
                    if(string.IsNullOrEmpty(symbols) == false) {
                        where.AppendFormat(" and ct.symbol in({0})",Helper.ConvertStringSQLFormat(symbols));
                    } else {
                        where.Append(" and ct.symbol='-1'");
                    }
                }

                //if (criteria.is_archive.HasValue)
                //{
                where.AppendFormat(" and ifnull(ct.is_archive,0)={0}",((criteria.is_archive ?? false) == true ? "1" : "0"));
                //}

                if(criteria.is_book_mark.HasValue) {
                    where.AppendFormat(" and ifnull(ct.is_book_mark,0)={0}",((criteria.is_book_mark ?? false) == true ? "1" : "0"));
                }

                if(criteria.from_price.HasValue) {
                    where.AppendFormat(" and ifnull(ct.close_price,0)>={0}",criteria.from_price);
                }

                if(criteria.to_price.HasValue) {
                    where.AppendFormat(" and ifnull(ct.close_price,0)<={0}",criteria.to_price);
                }


                if(criteria.from_rsi.HasValue) {
                    where.AppendFormat(" and ifnull(ct.rsi,0)>={0}",criteria.from_rsi);
                }

                if(criteria.to_rsi.HasValue) {
                    where.AppendFormat(" and ifnull(ct.rsi,0)<={0}",criteria.to_rsi);
                }

                if(criteria.from_prev_rsi.HasValue) {
                    where.AppendFormat(" and ifnull(ct.prev_rsi,0)>={0}",criteria.from_prev_rsi);
                }

                if(criteria.to_prev_rsi.HasValue) {
                    where.AppendFormat(" and ifnull(ct.prev_rsi,0)<={0}",criteria.to_prev_rsi);
                }

                if(criteria.is_nifty_50.HasValue) {
                    where.AppendFormat(" and ifnull(ct.is_nifty_50,0)={0}",((criteria.is_nifty_50 ?? false) == true ? "1" : "0"));
                }

                if(criteria.is_nifty_100.HasValue) {
                    where.AppendFormat(" and ifnull(ct.is_nifty_100,0)={0}",((criteria.is_nifty_100 ?? false) == true ? "1" : "0"));
                }

                if(criteria.is_nifty_200.HasValue) {
                    where.AppendFormat(" and ifnull(ct.is_nifty_200,0)={0}",((criteria.is_nifty_200 ?? false) == true ? "1" : "0"));
                }

                if(criteria.is_old.HasValue) {
                    where.AppendFormat(" and ifnull(ct.is_old,0)={0}",((criteria.is_old ?? false) == true ? "1" : "0"));
                }

                if(string.IsNullOrEmpty(criteria.ignore_symbols) == false) {
                    where.AppendFormat(" and ifnull(ct.symbol,0) not in({0})",Helper.ConvertStringSQLFormat(criteria.ignore_symbols));
                }

                if((criteria.is_current_stock ?? false) == true) {
                    joinTables += " join tra_holding h on h.symbol = ct.symbol ";
                }

                selectFields = "count(*) as cnt";

                sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

                //Helper.Log(sql,"GET_COUNT");

                paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

                if(string.IsNullOrEmpty(paging.SortOrder)) {
                    paging.SortOrder = "asc";
                }

                if(paging.PageSize > 0) {
                    int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                    int to = paging.PageSize;
                    pageLimit = string.Format("limit {0},{1}",from,to);
                }

                orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

                string dateFilter = string.Empty;
                DateTime? minDate = Convert.ToDateTime("01/01/1900");
                if(criteria.start_date.HasValue && criteria.end_date.HasValue) {
                    dateFilter = string.Format(" m.trade_date>='{0}' and m.trade_date<='{1}' ",criteria.start_date.Value.ToString("yyyy-MM-dd"),criteria.end_date.Value.ToString("yyyy-MM-dd"));
                }

                if(criteria.total_start_date.HasValue == false)
                    criteria.total_start_date = DateTime.Now.Date.AddDays(-(365 * 3));
                if(criteria.total_end_date.HasValue == false)
                    criteria.total_end_date = DateTime.Now.Date;

                string totalDateFilter = string.Empty;
                totalDateFilter = string.Format(" m.trade_date>='{0}' and m.trade_date<='{1}' "
                    ,criteria.total_start_date.Value.ToString("yyyy-MM-dd")
                    ,criteria.total_end_date.Value.ToString("yyyy-MM-dd")) + Environment.NewLine;

                string totalDateAVGFilter = string.Empty;
                totalDateAVGFilter = string.Format(" and m.avg_date>='{0}' and m.avg_date<='{1}' "
                    ,DataTypeHelper.GetFirstDayOfMonth(criteria.total_start_date.Value).ToString("yyyy-MM-dd")
                    ,DataTypeHelper.GetLastDayOfMonth(criteria.total_end_date.Value).ToString("yyyy-MM-dd")) + Environment.NewLine;

            }
            //if (criteria.trigger_start_date.HasValue == false)
            //    criteria.trigger_start_date = criteria.total_start_date;
            //if (criteria.trigger_end_date.HasValue == false)
            //    criteria.trigger_end_date = criteria.total_end_date;

            //string triggerDateFilter = string.Empty;
            //triggerDateFilter = string.Format(" and m.trade_date>='{0}' and m.trade_date<='{1}' "
            //    , criteria.trigger_start_date.Value.ToString("yyyy-MM-dd")
            //    , criteria.trigger_end_date.Value.ToString("yyyy-MM-dd")) + Environment.NewLine;

            selectFields = "ct.*" + Environment.NewLine + "";


            if((criteria.id ?? 0) <= 0) {
                selectFields += ",(select ifnull(open_price,0) from tra_market m where m.trade_date>='2016-04-01' and m.trade_date<='2017-03-31' and m.symbol = ct.symbol order by m.trade_date asc limit 0,1) as 2016_open" + Environment.NewLine +
                ",(select ifnull(close_price,0) from tra_market m where m.trade_date>='2016-04-01' and m.trade_date<='2017-03-31' and m.symbol = ct.symbol order by m.trade_date desc limit 0,1) as 2016_close" + Environment.NewLine +

                ",(select ifnull(open_price,0) from tra_market m where m.trade_date>='2017-04-01' and m.trade_date<='2018-03-31' and m.symbol = ct.symbol order by m.trade_date asc limit 0,1) as 2017_open" + Environment.NewLine +
                ",(select ifnull(close_price,0) from tra_market m where m.trade_date>='2017-04-01' and m.trade_date<='2018-03-31' and m.symbol = ct.symbol order by m.trade_date desc limit 0,1) as 2017_close" + Environment.NewLine +

                ",(select ifnull(open_price,0) from tra_market m where m.trade_date>='2018-04-01' and m.trade_date<='2019-03-31' and m.symbol = ct.symbol order by m.trade_date asc limit 0,1) as 2018_open" + Environment.NewLine +
                ",(select ifnull(close_price,0) from tra_market m where m.trade_date>='2018-04-01' and m.trade_date<='2019-03-31' and m.symbol = ct.symbol order by m.trade_date desc limit 0,1) as 2018_close" + Environment.NewLine +
                "";
            }

            //",(select ifnull(count(*),0) from tra_holding h where h.symbol = ct.symbol) as is_holding" +
            //",(select high_price from tra_market m where m.symbol = ct.symbol " + totalDateFilter + " and m.high_price > 0 order by m.high_price desc limit 0,1) as total_high_price" + Environment.NewLine +
            //",(select low_price from tra_market m where m.symbol = ct.symbol " + totalDateFilter + " and m.low_price > 0 order by m.low_price asc limit 0,1) as total_low_price" + Environment.NewLine +
            //",(select high_price from tra_market m where m.symbol = ct.symbol " + dateFilter + " and m.high_price > 0 order by m.high_price desc limit 0,1) as profit_high_price" + Environment.NewLine +
            //",(select low_price from tra_market m where m.symbol = ct.symbol " + dateFilter + " and m.low_price > 0 order by m.low_price asc limit 0,1) as profit_low_price" + Environment.NewLine +
            //",(select ifnull(rsi,0) from tra_market m where m.symbol = ct.symbol " + dateFilter + " order by m.trade_date desc limit 0,1) as profit_rsi" + Environment.NewLine +
            //",(select ifnull(rsi,0) from tra_market m where m.symbol = ct.symbol " + totalDateFilter + " order by m.trade_date desc limit 0,1) as total_rsi" + Environment.NewLine +

            //",(select open_price from tra_market m where " + totalDateFilter + " and m.symbol = ct.symbol order by m.trade_date asc limit 0,1) as total_first_price" + Environment.NewLine +
            //",(select ltp_price from tra_market m where " + totalDateFilter + " and m.symbol = ct.symbol order by m.trade_date desc limit 0,1) as total_last_price" + Environment.NewLine +

            //",(select open_price from tra_market m where m.symbol = ct.symbol " + triggerDateFilter + " order by m.trade_date asc limit 0,1) as trigger_first_price" + Environment.NewLine +
            //",(select ltp_price from tra_market m where m.symbol = ct.symbol " + triggerDateFilter + " order by m.trade_date desc limit 0,1) as trigger_last_price" + Environment.NewLine +

            //",(select open_price from tra_market m where " + dateFilter + " and m.symbol = ct.symbol order by m.trade_date asc limit 0,1) as first_price" + Environment.NewLine +
            //",(select ltp_price from tra_market m where " + dateFilter + " and m.symbol = ct.symbol order by m.trade_date desc limit 0,1) as last_price" + Environment.NewLine +
            //",(select count(*) from tra_market_avg m where m.symbol = ct.symbol and m.avg_type = 'M' " + totalDateAVGFilter + " and ifnull(m.percentage, 0) < 0) as negative" + Environment.NewLine +
            //",(select count(*) from tra_market_avg m where m.symbol = ct.symbol and m.avg_type = 'M' " + totalDateAVGFilter + "  and ifnull(m.percentage, 0) > 0) as positive" + Environment.NewLine +
            //",(select count(*) from tra_market_avg m where m.symbol = ct.symbol and m.avg_type = 'M' " + totalDateAVGFilter + " ) as total" + Environment.NewLine +

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
                           //",(((ifnull(ct.ltp_price, 0) - ifnull(ct.prev_price, 0)) / ifnull(ct.prev_price, 0)) * 100) as prev_percentage" + Environment.NewLine +
                           //",(ifnull(ct.open_price,0) - ifnull(ct.prev_price,0)) as diff" + Environment.NewLine +
                           //",(ifnull(ct.rsi,0) - ifnull(ct.prev_rsi,0)) as rsi_diff" + Environment.NewLine +

                           //",(((ifnull(ct.ltp_price, 0) - ifnull(ct.open_price, 0)) / ifnull(ct.open_price, 0)) * 100) as ltp_percentage" + Environment.NewLine +
                           //",(((ifnull(ct.high_price, 0) - ifnull(ct.open_price, 0)) / ifnull(ct.open_price, 0)) * 100) as high_percentage" + Environment.NewLine +
                           //",(((ifnull(ct.low_price, 0) - ifnull(ct.open_price, 0)) / ifnull(ct.open_price, 0)) * 100) as low_percentage" + Environment.NewLine +

                           //",(((ifnull(ct.ltp_price, 0) - ifnull(ct.week_52_high, 0)) / ifnull(ct.week_52_high, 0)) * 100) as week_52_percentage" + Environment.NewLine +
                           //",(((ifnull(ct.week_52_high, 0) - ifnull(ct.ltp_price, 0)) / ifnull(ct.ltp_price, 0)) * 100) as week_52_positive_percentage" + Environment.NewLine +
                           //",(((ifnull(ct.ltp_price, 0) - ifnull(ct.week_52_low, 0)) / ifnull(ct.week_52_low, 0)) * 100) as week_52_low_percentage" + Environment.NewLine +
                           ",ct.company_id as id" + Environment.NewLine +
                           "";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,"","","");

            where = new StringBuilder();

            where.AppendFormat(" where company_id > 0");

            //if((criteria.is_sell_to_buy ?? false) == true) {
            //    where.AppendFormat(" and ifnull(open_price,0)>=ifnull(low_price,0)");
            //    where.AppendFormat(" and ifnull(open_price,0)>=ifnull(high_price,0)");
            //    //where.AppendFormat(" and ifnull(high_percentage,0)>=0");
            //    //where.AppendFormat(" and ifnull(high_percentage,0)<=0.5");
            //}

            //if((criteria.is_buy_to_sell ?? false) == true) {
            //    where.AppendFormat(" and ifnull(open_price,0)<=ifnull(high_price,0)");
            //    //where.AppendFormat(" and ifnull(low_percentage,0)<=0");
            //    //where.AppendFormat(" and ifnull(low_percentage,0)>=-0.5");
            //    where.AppendFormat(" and ifnull(open_price,0)<=ifnull(low_price,0)");
            //}

            //if(string.IsNullOrEmpty(criteria.ltp_from_percentage) == false) {
            //    where.AppendFormat(" and ifnull(ltp_percentage,0)>={0}",criteria.ltp_from_percentage);
            //}

            //if(string.IsNullOrEmpty(criteria.ltp_to_percentage) == false) {
            //    where.AppendFormat(" and ifnull(ltp_percentage,0)<={0}",criteria.ltp_to_percentage);
            //}

            //if((criteria.from_profit ?? 0) != 0) {
            //    where.AppendFormat(" and ifnull((((last_price - first_price)/first_price) * 100),0)>={0}",criteria.from_profit);
            //}

            //if((criteria.to_profit ?? 0) != 0) {
            //    where.AppendFormat(" and ifnull((((last_price - first_price)/first_price) * 100),0)<={0}",criteria.to_profit);
            //}

            //if((criteria.total_from_profit ?? 0) != 0) {
            //    where.AppendFormat(" and ifnull((((total_last_price - total_first_price)/total_first_price) * 100),0)>={0}",criteria.total_from_profit);
            //}

            //if((criteria.total_to_profit ?? 0) != 0) {
            //    where.AppendFormat(" and ifnull((((total_last_price - total_first_price)/total_first_price) * 100),0)<={0}",criteria.total_to_profit);
            //}

            //where.Append(" and ifnull(total_first_price,0)>0 and ifnull(total_last_price,0)>0 and ifnull(first_price,0)>0 and ifnull(last_price,0)>0 ");

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
       ") as tbl {0} {1} {2} {3} ",where,"","","");

            tempsql = "select count(*) as cnt from(" + tempsql + ") as tbl2";

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,tempsql));

            string yearPercentage = ",if(2016_open>0,(((2016_close-2016_open)/2016_open) * 100),0) as percentage_2016" + Environment.NewLine +
            ",if(2017_open>0,(((2017_close-2017_open)/2017_open) * 100),0) as percentage_2017" + Environment.NewLine +
            ",if(2018_open>0,(((2018_close-2018_open)/2018_open) * 100),0) as percentage_2018" + Environment.NewLine +
            "";

            sql = string.Format("select " +

            //"(((last_price - first_price)/first_price) * 100) as profit" + Environment.NewLine +
            //",(((total_last_price - total_first_price)/total_first_price) * 100) as total_profit" + Environment.NewLine +
            //",(((trigger_last_price - trigger_first_price)/trigger_first_price) * 100) as trigger_profit" + Environment.NewLine +
            //",(negative/(negative+positive) * 100) as negative_percentage" + Environment.NewLine +
            //",(positive/(negative+positive) * 100) as positive_percentage" + Environment.NewLine +
            //",(((profit_high_price - ltp_price)/ltp_price) * 100) as profit_high_percentage" + Environment.NewLine +
            //",(((profit_low_price - ltp_price)/ltp_price) * 100) as profit_low_percentage" + Environment.NewLine +
            " tbl.*" + Environment.NewLine +
            ((criteria.id ?? 0) <= 0 ? yearPercentage : "") + Environment.NewLine +
            " from(" + Environment.NewLine +
            sql + Environment.NewLine +
            ") as tbl {0} {1} {2} {3} ",where,"",orderBy,pageLimit);

            Helper.Log(sql,"GET_SQL");
            List<TRA_COMPANY> rows = new List<TRA_COMPANY>();
            List<tra_company_category> companyCategories;
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<TRA_COMPANY>(sql).ToList();
                List<string> symbols = (from q in rows select q.symbol).ToList();
                companyCategories = (from q in context.tra_company_category
                                     where symbols.Contains(q.symbol) == true
                                     select q).ToList();
            }

            foreach(var row in rows) {
                row.category_list = (from q in companyCategories
                                     where q.symbol == row.symbol
                                     select q.category_name).ToList();
                string categoryName = "";
                foreach(var cat in row.category_list) {
                    categoryName += cat + ",";
                }
                if(string.IsNullOrEmpty(categoryName) == false) {
                    categoryName = categoryName.Substring(0,categoryName.Length - 1);
                }
                row.category_name = categoryName;
            }
            //foreach (var row in rows)
            //{
            //    if ((row.first_price ?? 0) <= 0)
            //    {
            //        row.first_price = row.total_last_price;
            //    }
            //    if ((row.last_price ?? 0) <= 0)
            //    {
            //        row.last_price = row.total_last_price;
            //    }
            //    if ((row.high_price ?? 0) <= 0)
            //    {
            //        row.high_price = row.total_last_price;
            //    }
            //    if ((row.low_price ?? 0) <= 0)
            //    {
            //        row.low_price = row.total_last_price;
            //    }
            //    if ((row.last_price ?? 0) <= 0)
            //    {
            //        row.profit = DataTypeHelper.SafeDivision(((row.last_price ?? 0) - (row.first_price ?? 0)), (row.first_price ?? 0)) * 100;
            //    }
            //}
            return new PaginatedListResult<TRA_COMPANY> { total = paging.Total,rows = rows };
        }

        public List<BatchLog> GetBatchLog(TRA_COMPANY_SEARCH criteria,Paging paging) {
            return Common.GetBatchLog(criteria,paging);
        }

        public List<DailySummary> GetDailySummary(TRA_COMPANY_SEARCH criteria,Paging paging) {
            return Common.GetDailySummary(criteria,paging);
        }

        public PaginatedListResult<TRA_COMPANY> GetMarketList(TRA_COMPANY_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from tra_market m {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            if((criteria.id ?? 0) > 0) {
                where.AppendFormat(" c.company_id={0}",criteria.id);
            } else {
                where.AppendFormat(" c.company_id>0 ");

                if(string.IsNullOrEmpty(criteria.company_name) == false) {
                    where.AppendFormat(" and c.company_name like '%{0}%",criteria.company_name);
                }
            }

            if(string.IsNullOrEmpty(criteria.symbols) == false) {
                where.AppendFormat(" and m.symbol in({0})",Helper.ConvertStringSQLFormat(criteria.symbols));
            }

            if(string.IsNullOrEmpty(criteria.categories) == false) {
                string categorySymbols = "-1";
                using(EcamContext context = new EcamContext()) {
                    List<string> categoryList = Helper.ConvertStringList(criteria.categories);
                    List<tra_company_category> categories = null;
                    List<string> categorySymbolList = null;
                    categories = (from q in context.tra_company_category
                                  where categoryList.Contains(q.category_name) == true
                                  select q).ToList();
                    if((criteria.is_all_category ?? false) == true) {
                        categorySymbolList = new List<string>();
                        foreach(var row in categories) {
                            var tempList = (from q in categories where q.symbol == row.symbol select q).ToList();
                            int selCnt = 0;
                            foreach(var tempRow in tempList) {
                                foreach(string str in categoryList) {
                                    if(string.IsNullOrEmpty(str) == false) {
                                        if(tempRow.category_name == str) {
                                            selCnt += 1;
                                        }
                                    }
                                }
                            }
                            if(selCnt == categoryList.Count) {
                                categorySymbolList.Add(row.symbol);
                            }
                        }
                        categorySymbolList = categorySymbolList.Distinct().ToList();
                    } else {
                        categorySymbolList = (from q in categories select q.symbol).Distinct().ToList();
                    }
                    if(categorySymbolList.Count > 0) {
                        categorySymbols = "";
                    }
                    foreach(var str in categorySymbolList) {
                        categorySymbols += str + ",";
                    }
                    if(string.IsNullOrEmpty(categorySymbols) == false) {
                        categorySymbols = categorySymbols.Substring(0,categorySymbols.Length - 1);
                    }
                }
                if(string.IsNullOrEmpty(categorySymbols) == false) {
                    where.AppendFormat(" and m.symbol in({0})",Helper.ConvertStringSQLFormat(categorySymbols));
                }
            }

            if(string.IsNullOrEmpty(criteria.mf_ids) == false) {
                string symbols = "";
                using(EcamContext context = new EcamContext()) {
                    List<int> ids = Helper.ConvertIntIds(criteria.mf_ids);
                    if(ids.Count > 0) {
                        var list = (from q in context.tra_mutual_fund_pf
                                    where ids.Contains(q.fund_id) == true
                                    select q.symbol).Distinct().ToList();
                        foreach(var str in list) {
                            symbols += str + ",";
                        }
                        if(string.IsNullOrEmpty(symbols) == false) {
                            symbols = symbols.Substring(0,symbols.Length - 1);
                        }
                    }
                }
                if(string.IsNullOrEmpty(symbols) == false) {
                    where.AppendFormat(" and m.symbol in({0})",Helper.ConvertStringSQLFormat(symbols));
                } else {
                    where.Append(" and m.symbol='-1'");
                }
            }

            //if (criteria.is_archive.HasValue)
            //{
            where.AppendFormat(" and ifnull(c.is_archive,0)={0}",((criteria.is_archive ?? false) == true ? "1" : "0"));
            //}

            if(criteria.is_book_mark.HasValue) {
                where.AppendFormat(" and ifnull(c.is_book_mark,0)={0}",((criteria.is_book_mark ?? false) == true ? "1" : "0"));
            }

            if(criteria.from_price.HasValue) {
                where.AppendFormat(" and ifnull(m.close_price,0)>={0}",criteria.from_price);
            }

            if(criteria.to_price.HasValue) {
                where.AppendFormat(" and ifnull(m.close_price,0)<={0}",criteria.to_price);
            }


            if(criteria.from_rsi.HasValue) {
                where.AppendFormat(" and ifnull(m.rsi,0)>={0}",criteria.from_rsi);
            }

            if(criteria.to_rsi.HasValue) {
                where.AppendFormat(" and ifnull(m.rsi,0)<={0}",criteria.to_rsi);
            }

            if(criteria.from_prev_rsi.HasValue) {
                where.AppendFormat(" and ifnull(m.prev_rsi,0)>={0}",criteria.from_prev_rsi);
            }

            if(criteria.to_prev_rsi.HasValue) {
                where.AppendFormat(" and ifnull(m.prev_rsi,0)<={0}",criteria.to_prev_rsi);
            }

            if(criteria.is_nifty_50.HasValue) {
                where.AppendFormat(" and ifnull(c.is_nifty_50,0)={0}",((criteria.is_nifty_50 ?? false) == true ? "1" : "0"));
            }

            if(criteria.is_nifty_100.HasValue) {
                where.AppendFormat(" and ifnull(c.is_nifty_100,0)={0}",((criteria.is_nifty_100 ?? false) == true ? "1" : "0"));
            }

            if(criteria.is_nifty_200.HasValue) {
                where.AppendFormat(" and ifnull(c.is_nifty_200,0)={0}",((criteria.is_nifty_200 ?? false) == true ? "1" : "0"));
            }

            if(criteria.is_old.HasValue) {
                where.AppendFormat(" and ifnull(c.is_old,0)={0}",((criteria.is_old ?? false) == true ? "1" : "0"));
            }

            if(criteria.trade_date.HasValue) {
                where.AppendFormat(" and m.trade_date='{0}' ",criteria.trade_date.Value.ToString("yyyy-MM-dd"));
            }

            DateTime? minDate = Convert.ToDateTime("01/01/1900");
            if(criteria.start_date.HasValue && criteria.end_date.HasValue) {
                where.AppendFormat(" and m.trade_date>='{0}' and m.trade_date<='{1}' ",criteria.start_date.Value.ToString("yyyy-MM-dd"),criteria.end_date.Value.ToString("yyyy-MM-dd"));
            }

            joinTables += " join tra_company c on c.symbol = m.symbol ";

            selectFields = "count(*) as cnt";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = "c.company_id" + Environment.NewLine +
                           ",c.company_name" + Environment.NewLine +
                           ",m.*" + Environment.NewLine +
                           ",(select m2.trade_date from tra_market m2 where m2.symbol = m.symbol and m2.trade_date < m.trade_date " +
                           " order by m2.trade_date desc limit 0,1) as yesterday_date " +
                           ",(select(((m2.close_price - m2.open_price) / m2.open_price) * 100) from tra_market m2 where m2.symbol = m.symbol and m2.trade_date < m.trade_date " +
                           " order by m2.trade_date desc limit 0,1) as yesterday_percentage " +
                           ",(((m.close_price-m.open_price)/m.open_price) * 100) as prev_percentage" +
                           "";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            //Helper.Log(sql);
            List<TRA_COMPANY> rows = new List<TRA_COMPANY>();
            List<tra_company_category> companyCategories;
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<TRA_COMPANY>(sql).ToList();
                List<string> symbols = (from q in rows select q.symbol).ToList();
                companyCategories = (from q in context.tra_company_category
                                     where symbols.Contains(q.symbol) == true
                                     select q).ToList();
            }

            foreach(var row in rows) {
                row.category_list = (from q in companyCategories
                                     where q.symbol == row.symbol
                                     select q.category_name).ToList();
                string categoryName = "";
                foreach(var cat in row.category_list) {
                    categoryName += cat + ",";
                }
                if(string.IsNullOrEmpty(categoryName) == false) {
                    categoryName = categoryName.Substring(0,categoryName.Length - 1);
                }
                row.category_name = categoryName;
            }
            return new PaginatedListResult<TRA_COMPANY> { total = paging.Total,rows = rows };
        }

        public PaginatedListResult<TRA_MARKET_INTRA_DAY> GetIntraDay(TRA_COMPANY_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string prefix = "intra";
            string sqlFormat = "select {0} from tra_market_intra_day " + prefix + " {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            DateTime lastTradeDate = DateTime.Now.Date;
            using(EcamContext context = new EcamContext()) {
                var lastTrade = (from q in context.tra_market_intra_day orderby q.trade_date descending select q).FirstOrDefault();
                if(lastTrade != null) {
                    lastTradeDate = lastTrade.trade_date;
                }
            }

            where.AppendFormat(" intra.trade_date>='{0}'",lastTradeDate.ToString("yyyy-MM-dd"));

            if(string.IsNullOrEmpty(criteria.symbols) == false) {
                where.AppendFormat(" and intra.symbol in({0})",Helper.ConvertStringSQLFormat(criteria.symbols));
            }

            joinTables += " join tra_company c on c.symbol = intra.symbol ";

            selectFields = "count(*) as cnt";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = "intra.symbol" + Environment.NewLine +
                            ",intra.trade_date" + Environment.NewLine +
                            ",DATE_FORMAT(intra.trade_date, '%h:%i %p') as time" + Environment.NewLine +
                            ",c.open_price" + Environment.NewLine +
                            ",intra.ltp_price" + Environment.NewLine +
                            ",c.company_id" + Environment.NewLine +
                            ",intra.rsi" + Environment.NewLine +
                            ",(select ifnull(m.rsi,0) from tra_market m where m.symbol = intra.symbol and m.trade_date < date(intra.trade_date) order by m.trade_date desc limit 0,1) as prev_rsi" +
                            ",(((ifnull(intra.ltp_price, 0) - ifnull(c.open_price, 0)) / ifnull(c.open_price, 0)) * 100) as ltp_percentage" + Environment.NewLine + "";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            //Helper.Log(sql);
            List<TRA_MARKET_INTRA_DAY> rows = new List<TRA_MARKET_INTRA_DAY>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<TRA_MARKET_INTRA_DAY>(sql).ToList();
            }
            return new PaginatedListResult<TRA_MARKET_INTRA_DAY> { total = paging.Total,rows = rows };
        }

        public PaginatedListResult<TRA_MARKET_RSI> GetRSI(TRA_COMPANY_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string prefix = "m";
            string sqlFormat = "select {0} from tra_market " + prefix + " {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            where.AppendFormat(" m.trade_date>='{0}'",DateTime.Now.Date.AddDays(-(365 * 3)).ToString("yyyy-MM-dd"));

            where.AppendFormat(" and m.trade_date<='{0}'",DateTime.Now.Date.ToString("yyyy-MM-dd"));

            if(string.IsNullOrEmpty(criteria.symbols) == false) {
                where.AppendFormat(" and m.symbol in({0})",Helper.ConvertStringSQLFormat(criteria.symbols));
            }

            joinTables += " join tra_company c on c.symbol = m.symbol ";

            selectFields = "count(*) as cnt";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = "m.symbol" + Environment.NewLine +
                            ",m.trade_date" + Environment.NewLine +
                            ",m.open_price" + Environment.NewLine +
                            ",m.low_price" + Environment.NewLine +
                            ",m.high_price" + Environment.NewLine +
                            ",m.ltp_price" + Environment.NewLine +
                            ",m.close_price" + Environment.NewLine +
                            ",m.rsi" + Environment.NewLine +
                            ",m.prev_rsi" + Environment.NewLine +
                            ",(((ifnull(m.ltp_price, 0) - ifnull(m.open_price, 0)) / ifnull(m.open_price, 0)) * 100) as ltp_percentage" + Environment.NewLine +
                            ",(((ifnull(m.ltp_price, 0) - ifnull(m.prev_price, 0)) / ifnull(m.prev_price, 0)) * 100) as prev_percentage" + Environment.NewLine +
                            ",(((ifnull(m.open_price, 0) - ifnull(m.prev_price, 0)) / ifnull(m.prev_price, 0)) * 100) as open_percentage" + Environment.NewLine +
                            ",(((ifnull(m.high_price, 0) - ifnull(m.prev_price, 0)) / ifnull(m.prev_price, 0)) * 100) as high_percentage" + Environment.NewLine +
                            ",c.company_id" + Environment.NewLine +
                            "";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            //Helper.Log(sql);
            List<TRA_MARKET_RSI> rows = new List<TRA_MARKET_RSI>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<TRA_MARKET_RSI>(sql).ToList();
            }
            return new PaginatedListResult<TRA_MARKET_RSI> { total = paging.Total,rows = rows };
        }

        public PaginatedListResult<TRA_MARKET_AVG> GetAvg(TRA_COMPANY_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string prefix = "intra";
            string sqlFormat = "select {0} from tra_market_avg " + prefix + " {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            where.AppendFormat(" intra.symbol in({0})",Helper.ConvertStringSQLFormat(criteria.symbols));

            joinTables += " join tra_company c on c.symbol = intra.symbol ";

            selectFields = "count(*) as cnt";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = "intra.*" + Environment.NewLine +
                "";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            //Helper.Log(sql);
            List<TRA_MARKET_AVG> rows = new List<TRA_MARKET_AVG>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<TRA_MARKET_AVG>(sql).ToList();
            }
            return new PaginatedListResult<TRA_MARKET_AVG> { total = paging.Total,rows = rows };
        }

        public PaginatedListResult<TRA_COMPANY> GetMonthlyAVG(TRA_COMPANY_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string prefix = "c";
            string sqlFormat = "select {0} from tra_company " + prefix + " {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            if((criteria.id ?? 0) > 0) {
                where.AppendFormat(" c.company_id={0}",criteria.id).Append(Environment.NewLine);
            } else {
                where.AppendFormat(" c.company_id>0 ");

                if(string.IsNullOrEmpty(criteria.company_name) == false) {
                    where.AppendFormat(" and c.company_name like '%{0}%",criteria.company_name).Append(Environment.NewLine);
                }
            }

            if(string.IsNullOrEmpty(criteria.symbols) == false) {
                where.AppendFormat(" and c.symbol in({0})",Helper.ConvertStringSQLFormat(criteria.symbols)).Append(Environment.NewLine);
            }

            if(string.IsNullOrEmpty(criteria.categories) == false) {
                string categorySymbols = "-1";
                using(EcamContext context = new EcamContext()) {
                    List<string> categoryList = Helper.ConvertStringList(criteria.categories);
                    List<tra_company_category> categories = null;
                    List<string> categorySymbolList = null;
                    categories = (from q in context.tra_company_category
                                  where categoryList.Contains(q.category_name) == true
                                  select q).ToList();
                    if((criteria.is_all_category ?? false) == true) {
                        categorySymbolList = new List<string>();
                        foreach(var row in categories) {
                            var tempList = (from q in categories where q.symbol == row.symbol select q).ToList();
                            int selCnt = 0;
                            foreach(var tempRow in tempList) {
                                foreach(string str in categoryList) {
                                    if(string.IsNullOrEmpty(str) == false) {
                                        if(tempRow.category_name == str) {
                                            selCnt += 1;
                                        }
                                    }
                                }
                            }
                            if(selCnt == categoryList.Count) {
                                categorySymbolList.Add(row.symbol);
                            }
                        }
                        categorySymbolList = categorySymbolList.Distinct().ToList();
                    } else {
                        categorySymbolList = (from q in categories select q.symbol).Distinct().ToList();
                    }
                    if(categorySymbolList.Count > 0) {
                        categorySymbols = "";
                    }
                    foreach(var str in categorySymbolList) {
                        categorySymbols += str + ",";
                    }
                    if(string.IsNullOrEmpty(categorySymbols) == false) {
                        categorySymbols = categorySymbols.Substring(0,categorySymbols.Length - 1);
                    }
                }
                if(string.IsNullOrEmpty(categorySymbols) == false) {
                    where.AppendFormat(" and c.symbol in({0})",Helper.ConvertStringSQLFormat(categorySymbols)).Append(Environment.NewLine);
                }
            }

            if(string.IsNullOrEmpty(criteria.mf_ids) == false) {
                string symbols = "";
                using(EcamContext context = new EcamContext()) {
                    List<int> ids = Helper.ConvertIntIds(criteria.mf_ids);
                    if(ids.Count > 0) {
                        var list = (from q in context.tra_mutual_fund_pf
                                    where ids.Contains(q.fund_id) == true
                                    select q.symbol).Distinct().ToList();
                        foreach(var str in list) {
                            symbols += str + ",";
                        }
                        if(string.IsNullOrEmpty(symbols) == false) {
                            symbols = symbols.Substring(0,symbols.Length - 1);
                        }
                    }
                }
                if(string.IsNullOrEmpty(symbols) == false) {
                    where.AppendFormat(" and c.symbol in({0})",Helper.ConvertStringSQLFormat(symbols)).Append(Environment.NewLine);
                } else {
                    where.Append(" and c.symbol='-1'").Append(Environment.NewLine);
                }
            }

            //if (criteria.is_archive.HasValue)
            //{
            where.AppendFormat(" and ifnull(c.is_archive,0)={0}",((criteria.is_archive ?? false) == true ? "1" : "0")).Append(Environment.NewLine);
            //}

            if(criteria.is_book_mark.HasValue) {
                where.AppendFormat(" and ifnull(c.is_book_mark,0)={0}",((criteria.is_book_mark ?? false) == true ? "1" : "0")).Append(Environment.NewLine);
            }

            if(criteria.from_price.HasValue) {
                where.AppendFormat(" and ifnull(c.close_price,0)>={0}",criteria.from_price).Append(Environment.NewLine);
            }

            if(criteria.to_price.HasValue) {
                where.AppendFormat(" and ifnull(c.close_price,0)<={0}",criteria.to_price).Append(Environment.NewLine);
            }


            if(criteria.from_rsi.HasValue) {
                where.AppendFormat(" and ifnull(c.rsi,0)>={0}",criteria.from_rsi).Append(Environment.NewLine);
            }

            if(criteria.to_rsi.HasValue) {
                where.AppendFormat(" and ifnull(c.rsi,0)<={0}",criteria.to_rsi).Append(Environment.NewLine);
            }

            if(criteria.from_prev_rsi.HasValue) {
                where.AppendFormat(" and ifnull(c.prev_rsi,0)>={0}",criteria.from_prev_rsi).Append(Environment.NewLine);
            }

            if(criteria.to_prev_rsi.HasValue) {
                where.AppendFormat(" and ifnull(c.prev_rsi,0)<={0}",criteria.to_prev_rsi).Append(Environment.NewLine);
            }

            if(criteria.is_nifty_50.HasValue) {
                where.AppendFormat(" and ifnull(c.is_nifty_50,0)={0}",((criteria.is_nifty_50 ?? false) == true ? "1" : "0")).Append(Environment.NewLine);
            }

            if(criteria.is_nifty_100.HasValue) {
                where.AppendFormat(" and ifnull(c.is_nifty_100,0)={0}",((criteria.is_nifty_100 ?? false) == true ? "1" : "0")).Append(Environment.NewLine);
            }

            if(criteria.is_nifty_200.HasValue) {
                where.AppendFormat(" and ifnull(c.is_nifty_200,0)={0}",((criteria.is_nifty_200 ?? false) == true ? "1" : "0")).Append(Environment.NewLine);
            }

            if(criteria.is_old.HasValue) {
                where.AppendFormat(" and ifnull(c.is_old,0)={0}",((criteria.is_old ?? false) == true ? "1" : "0")).Append(Environment.NewLine);
            }

            groupByName = " group by c.symbol " + Environment.NewLine;

            selectFields = "count(*) as cnt";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            string dateFilter = string.Empty;
            DateTime? minDate = Convert.ToDateTime("01/01/1900");
            if(criteria.start_date.HasValue && criteria.end_date.HasValue) {
                dateFilter = string.Format(" and m.trade_date>='{0}' and m.trade_date<='{1}' ",criteria.start_date.Value.ToString("yyyy-MM-dd"),criteria.end_date.Value.ToString("yyyy-MM-dd"));
            }

            if(criteria.total_start_date.HasValue == false)
                criteria.total_start_date = DateTime.Now.Date.AddDays(-(365 * 3));
            if(criteria.total_end_date.HasValue == false)
                criteria.total_end_date = DateTime.Now.Date;

            string totalDateFilter = string.Empty;
            totalDateFilter = string.Format(" and m.trade_date>='{0}' and m.trade_date<='{1}' "
                ,criteria.total_start_date.Value.ToString("yyyy-MM-dd")
                ,criteria.total_end_date.Value.ToString("yyyy-MM-dd")) + Environment.NewLine;


            string totalDateAVGFilter = string.Empty;
            totalDateAVGFilter = string.Format(" and m.avg_date>='{0}' and m.avg_date<='{1}' "
                ,DataTypeHelper.GetFirstDayOfMonth(criteria.total_start_date.Value).ToString("yyyy-MM-dd")
                ,DataTypeHelper.GetLastDayOfMonth(criteria.total_end_date.Value).ToString("yyyy-MM-dd")) + Environment.NewLine;

            selectFields = " c.*" + Environment.NewLine +
                            ",c.company_id as id" + Environment.NewLine +
                            ",(select high_price from tra_market m where m.symbol = c.symbol " + totalDateFilter + " and m.high_price > 0 order by m.high_price desc limit 0,1) as total_high_price" + Environment.NewLine +
                            ",(select low_price from tra_market m where m.symbol = c.symbol " + totalDateFilter + " and m.low_price > 0 order by m.low_price asc limit 0,1) as total_low_price" + Environment.NewLine +
                            ",(select high_price from tra_market m where m.symbol = c.symbol " + dateFilter + " and m.high_price > 0 order by m.high_price desc limit 0,1) as profit_high_price" + Environment.NewLine +
                            ",(select low_price from tra_market m where m.symbol = c.symbol " + dateFilter + " and m.low_price > 0 order by m.low_price asc limit 0,1) as profit_low_price" + Environment.NewLine +
                            ",(select open_price from tra_market m where m.symbol = c.symbol " + totalDateFilter + " order by m.trade_date asc limit 0,1) as total_first_price" + Environment.NewLine +
                            ",(select ltp_price from tra_market m where m.symbol = c.symbol " + totalDateFilter + " order by m.trade_date desc limit 0,1) as total_last_price" + Environment.NewLine +
                            ",(select open_price from tra_market m where m.symbol = c.symbol " + dateFilter + " order by m.trade_date asc limit 0,1) as first_price" + Environment.NewLine +
                            ",(select ltp_price from tra_market m where m.symbol = c.symbol " + dateFilter + " order by m.trade_date desc limit 0,1) as last_price" + Environment.NewLine +
                            ",(select count(*) from tra_market_avg m where m.symbol = c.symbol and m.avg_type = 'M' " + totalDateAVGFilter + " and ifnull(m.percentage, 0) < 0) as negative" + Environment.NewLine +
                            ",(select count(*) from tra_market_avg m where m.symbol = c.symbol and m.avg_type = 'M' " + totalDateAVGFilter + "  and ifnull(m.percentage, 0) > 0) as positive" + Environment.NewLine +
                            ",(select count(*) from tra_market_avg m where m.symbol = c.symbol and m.avg_type = 'M' " + totalDateAVGFilter + " ) as total" + Environment.NewLine +
                            "";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

            where = new StringBuilder();

            //where.AppendFormat(" where first_price > 0 and last_price > 0 and (((total_last_price - total_first_price)/total_first_price) * 100) >= " + ((criteria.min_profit ?? 0) > 0 ? criteria.min_profit : 25)).Append(Environment.NewLine);

            where.AppendFormat(" where company_id > 0 ").Append(Environment.NewLine);

            if((criteria.is_sell_to_buy ?? false) == true) {
                where.AppendFormat(" and ifnull(open_price,0)>=ifnull(low_price,0)").Append(Environment.NewLine);
                where.AppendFormat(" and ifnull(open_price,0)>=ifnull(high_price,0)").Append(Environment.NewLine);
                //where.AppendFormat(" and ifnull(high_percentage,0)>=0");
                //where.AppendFormat(" and ifnull(high_percentage,0)<=0.5");
            }

            if((criteria.is_buy_to_sell ?? false) == true) {
                where.AppendFormat(" and ifnull(open_price,0)<=ifnull(high_price,0)").Append(Environment.NewLine);
                //where.AppendFormat(" and ifnull(low_percentage,0)<=0");
                //where.AppendFormat(" and ifnull(low_percentage,0)>=-0.5");
                where.AppendFormat(" and ifnull(open_price,0)<=ifnull(low_price,0)").Append(Environment.NewLine);
            }

            if(string.IsNullOrEmpty(criteria.ltp_from_percentage) == false) {
                where.AppendFormat(" and ifnull(ltp_percentage,0)>={0}",criteria.ltp_from_percentage).Append(Environment.NewLine);
            }

            if(string.IsNullOrEmpty(criteria.ltp_to_percentage) == false) {
                where.AppendFormat(" and ifnull(ltp_percentage,0)<={0}",criteria.ltp_to_percentage).Append(Environment.NewLine);
            }

            if((criteria.from_profit ?? 0) != 0) {
                where.AppendFormat(" and ifnull((((last_price - first_price)/first_price) * 100),0)>={0}",criteria.from_profit).Append(Environment.NewLine);
            }

            if((criteria.to_profit ?? 0) != 0) {
                where.AppendFormat(" and ifnull((((last_price - first_price)/first_price) * 100),0)<={0}",criteria.to_profit).Append(Environment.NewLine);
            }

            if((criteria.max_negative_count ?? 0) > 0) {
                where.AppendFormat(" and ifnull(negative,0)<={0}",criteria.max_negative_count).Append(Environment.NewLine);
            }

            //orderBy = " order by (negative/(negative+positive) * 100) asc,(positive/(negative+positive) * 100) desc,(((total_last_price - total_first_price)/total_first_price) * 100) desc,monthly_avg desc,weekly_avg desc " + Environment.NewLine;

            //orderBy = " order by negative asc,(((last_price - first_price)/first_price) * 100) desc,(((total_last_price - total_first_price)/total_first_price) * 100) desc,monthly_avg desc,weekly_avg desc " + Environment.NewLine;

            orderBy = " order by (negative/(negative+positive) * 100) asc,monthly_avg desc,weekly_avg desc " + Environment.NewLine;

            sql = string.Format("select " + Environment.NewLine +
            "(((last_price - first_price)/first_price) * 100) as profit" + Environment.NewLine +
            ",(((total_last_price - total_first_price)/total_first_price) * 100) as total_profit" + Environment.NewLine +
            ",(negative/(negative+positive) * 100) as negative_percentage" + Environment.NewLine +
            ",(positive/(negative+positive) * 100) as positive_percentage" + Environment.NewLine +
            ",tbl.*" + Environment.NewLine +
            " from(" + Environment.NewLine +
            sql + Environment.NewLine +
            ") as tbl {0} {1} {2} {3} ",where,"",orderBy,pageLimit);

            //     string tempsql = string.Format("select " +
            //"tbl22.*" + Environment.NewLine +
            //" from(" + Environment.NewLine +
            //sql + Environment.NewLine +
            //") as tbl22 {0} {1} {2} {3} ", where, "", "", "");

            //     tempsql = "select count(*) as cnt from(" + tempsql + ") as tbl2";

            //     paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString, tempsql));

            //Helper.Log(sql);
            List<TRA_COMPANY> rows = new List<TRA_COMPANY>();
            List<tra_company_category> companyCategories;
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<TRA_COMPANY>(sql).ToList();
                List<string> symbols = (from q in rows select q.symbol).ToList();
                companyCategories = (from q in context.tra_company_category
                                     where symbols.Contains(q.symbol) == true
                                     select q).ToList();
            }

            foreach(var row in rows) {
                row.category_list = (from q in companyCategories
                                     where q.symbol == row.symbol
                                     select q.category_name).ToList();
                string categoryName = "";
                foreach(var cat in row.category_list) {
                    if(cat.Contains("1_") == false && cat.Contains("2_") == false) {
                        categoryName += cat + ",";
                    }
                }
                if(string.IsNullOrEmpty(categoryName) == false) {
                    categoryName = categoryName.Substring(0,categoryName.Length - 1);
                }
                row.category_name = categoryName;
            }
            paging.Total = rows.Count();
            switch(paging.SortName) {
                case "profit":
                    if(paging.SortOrder == "asc")
                        rows = (from q in rows orderby q.profit select q).ToList();
                    else
                        rows = (from q in rows orderby q.profit descending select q).ToList();
                    break;
                case "total_profit":
                    if(paging.SortOrder == "asc")
                        rows = (from q in rows orderby q.total_profit select q).ToList();
                    else
                        rows = (from q in rows orderby q.total_profit descending select q).ToList();
                    break;
                case "rsi":
                    if(paging.SortOrder == "asc")
                        rows = (from q in rows orderby q.rsi select q).ToList();
                    else
                        rows = (from q in rows orderby q.rsi descending select q).ToList();
                    break;
            }
            return new PaginatedListResult<TRA_COMPANY> { total = paging.Total,rows = rows };
        }

    }
}
