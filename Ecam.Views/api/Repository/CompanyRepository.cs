
using Ecam.Contracts;
using Ecam.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Framework.Repository
{

    public interface ICompanyRepository
    {
        PaginatedListResult<TRA_COMPANY> Get(TRA_COMPANY_SEARCH criteria, Paging paging);
        PaginatedListResult<TRA_MARKET_INTRA_DAY> GetIntraDay(TRA_COMPANY_SEARCH criteria, Paging paging);
        PaginatedListResult<TRA_MARKET_RSI> GetRSI(TRA_COMPANY_SEARCH criteria, Paging paging);
        List<Select2List> GetCompanys(string name, int pageSize = 50, string categories = "");
        List<Select2List> GetCategories(string name, int pageSize = 50);
        List<Select2List> GetMFFunds(string name, int pageSize = 50);
    }

    public class CompanyRepository : ICompanyRepository
    {
        public List<Select2List> GetCategories(string name, int pageSize = 50)
        {
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

            if (string.IsNullOrEmpty(name) == false)
            {
                where.AppendFormat(" and comp.category_name like '{0}%'", name);
            }

            Paging paging = new Paging { PageIndex = 1, PageSize = pageSize };

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

            selectFields = "comp.category_name as id" +
                           ",comp.category_name as label" +
                           ",comp.category_name as value";

            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);

            List<Select2List> rows = new List<Select2List>();
            using (EcamContext context = new EcamContext())
            {
                rows = context.Database.SqlQuery<Select2List>(sql).ToList();
            }
            return rows;
        }

        public List<Select2List> GetMFFunds(string name, int pageSize = 50)
        {
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

            if (string.IsNullOrEmpty(name) == false)
            {
                where.AppendFormat(" and comp.fund_name like '{0}%'", name);
            }

            Paging paging = new Paging { PageIndex = 1, PageSize = pageSize };

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

            selectFields = "comp.mutual_fund_id as id" +
                           ",comp.fund_name as label" +
                           ",comp.fund_name as value";

            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);

            List<Select2List> rows = new List<Select2List>();
            using (EcamContext context = new EcamContext())
            {
                rows = context.Database.SqlQuery<Select2List>(sql).ToList();
            }
            return rows;
        }

        public List<Select2List> GetCompanys(string name, int pageSize = 50, string categories = "")
        {
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

            if (string.IsNullOrEmpty(name) == false)
            {
                where.AppendFormat(" and comp.company_name like '{0}%' or comp.symbol like '{0}%' ", name);
            }

            if (string.IsNullOrEmpty(categories) == false)
            {
                string categorySymbols = "-1";
                using (EcamContext context = new EcamContext())
                {
                    List<string> categoryList = Helper.ConvertStringList(categories);
                    List<string> categorySymbolList = (from q in context.tra_company_category
                                                       where categoryList.Contains(q.category_name) == true
                                                       select q.symbol).Distinct().ToList();
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
                    where.AppendFormat(" and comp.symbol in({0})", Helper.ConvertStringSQLFormat(categorySymbols));
                }
            }

            Paging paging = new Paging { PageIndex = 1, PageSize = pageSize };

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

            selectFields = "comp.symbol as id" +
                           ",comp.company_name as label" +
                           ",comp.company_name as value";

            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);

            List<Select2List> rows = new List<Select2List>();
            using (EcamContext context = new EcamContext())
            {
                rows = context.Database.SqlQuery<Select2List>(sql).ToList();
            }
            return rows;
        }

        public PaginatedListResult<TRA_COMPANY> Get(TRA_COMPANY_SEARCH criteria, Paging paging)
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
            string role = Authentication.CurrentRole;

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
                    var isAllCategory = false;
                    //if (categoryList.Count > 1)
                    //{
                    //    isAllCategory = true;
                    //}
                    List<tra_company_category> categories = null;
                    List<string> categorySymbolList = null;
                    categories = (from q in context.tra_company_category
                                  where categoryList.Contains(q.category_name) == true
                                  select q).ToList();
                    if (isAllCategory == true)
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

            selectFields = "ct.*" + Environment.NewLine + "";

            if (string.IsNullOrEmpty(criteria.mf_ids) == false)
            {
                selectFields += ",(select count(mpf.fund_id) from tra_mutual_fund_pf mpf where mpf.symbol = ct.symbol and mpf.fund_id in(" + criteria.mf_ids + ")) as mf_cnt_2" + Environment.NewLine +
                    ",(select sum(ifnull(mpf.stock_value, 0)) from tra_mutual_fund_pf mpf where mpf.symbol = ct.symbol and mpf.fund_id in(" + criteria.mf_ids + ")) as mf_qty_2" + Environment.NewLine +
                    "";
            }
            else
            {
                selectFields += ",(select count(mpf.fund_id) from tra_mutual_fund_pf mpf where mpf.symbol = ct.symbol) as mf_cnt_2" + Environment.NewLine +
                   ",(select sum(ifnull(mpf.stock_value, 0)) from tra_mutual_fund_pf mpf where mpf.symbol = ct.symbol) as mf_qty_2" + Environment.NewLine +
                   "";
            }
            selectFields += "" +
                           ",(((ifnull(ct.ltp_price, 0) - ifnull(ct.prev_price, 0)) / ifnull(ct.prev_price, 0)) * 100) as prev_percentage" + Environment.NewLine +
                           ",(ifnull(ct.open_price,0) - ifnull(ct.prev_price,0)) as diff" + Environment.NewLine +
                           ",(ifnull(ct.rsi,0) - ifnull(ct.prev_rsi,0)) as rsi_diff" + Environment.NewLine +

                           ",(((ifnull(ct.ltp_price, 0) - ifnull(ct.open_price, 0)) / ifnull(ct.open_price, 0)) * 100) as ltp_percentage" + Environment.NewLine +
                           ",(((ifnull(ct.high_price, 0) - ifnull(ct.open_price, 0)) / ifnull(ct.open_price, 0)) * 100) as high_percentage" + Environment.NewLine +
                           ",(((ifnull(ct.low_price, 0) - ifnull(ct.open_price, 0)) / ifnull(ct.open_price, 0)) * 100) as low_percentage" + Environment.NewLine +

                           ",(((ifnull(ct.ltp_price, 0) - ifnull(ct.week_52_high, 0)) / ifnull(ct.week_52_high, 0)) * 100) as week_52_percentage" + Environment.NewLine +
                           ",(((ifnull(ct.week_52_high, 0) - ifnull(ct.ltp_price, 0)) / ifnull(ct.ltp_price, 0)) * 100) as week_52_positive_percentage" + Environment.NewLine +
                           ",(((ifnull(ct.ltp_price, 0) - ifnull(ct.week_52_low, 0)) / ifnull(ct.week_52_low, 0)) * 100) as week_52_low_percentage" + Environment.NewLine +
                           ",ct.company_id as id" + Environment.NewLine +
                           "";

            sql = string.Format(sqlFormat, selectFields, joinTables, where, "", "", "");

            where = new StringBuilder();

            where.AppendFormat(" where company_id > 0");

            if ((criteria.is_sell_to_buy ?? false) == true)
            {
                where.AppendFormat(" and ifnull(open_price,0)>=ifnull(low_price,0)");
                //where.AppendFormat(" and ifnull(open_price,0)>=ifnull(high_price,0)");
                where.AppendFormat(" and ifnull(high_percentage,0)>=0");
                where.AppendFormat(" and ifnull(high_percentage,0)<=0.5");
            }

            if ((criteria.is_buy_to_sell ?? false) == true)
            {
                where.AppendFormat(" and ifnull(open_price,0)<=ifnull(high_price,0)");
                where.AppendFormat(" and ifnull(low_percentage,0)<=0");
                where.AppendFormat(" and ifnull(low_percentage,0)>=-0.5");
                //where.AppendFormat(" and ifnull(open_price,0)<=ifnull(low_price,0)");
            }

            if (string.IsNullOrEmpty(criteria.ltp_from_percentage) == false)
            {
                where.AppendFormat(" and ifnull(ltp_percentage,0)>={0}", criteria.ltp_from_percentage);
            }

            if (string.IsNullOrEmpty(criteria.ltp_to_percentage) == false)
            {
                where.AppendFormat(" and ifnull(ltp_percentage,0)<={0}", criteria.ltp_to_percentage);
            }

            string tempsql = string.Format("select " +
       "tbl.*" + Environment.NewLine +
       " from(" + Environment.NewLine +
       sql + Environment.NewLine +
       ") as tbl {0} {1} {2} {3} ", where, "", "", "");

            tempsql = "select count(*) as cnt from(" + tempsql + ") as tbl2";

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString, tempsql));

            sql = string.Format("select " +
            "tbl.*" + Environment.NewLine +
            " from(" + Environment.NewLine +
            sql + Environment.NewLine +
            ") as tbl {0} {1} {2} {3} ", where, "", orderBy, pageLimit);

            //Helper.Log(sql);
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
            return new PaginatedListResult<TRA_COMPANY> { total = paging.Total, rows = rows };
        }

        public PaginatedListResult<TRA_MARKET_INTRA_DAY> GetIntraDay(TRA_COMPANY_SEARCH criteria, Paging paging)
        {
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
            using (EcamContext context = new EcamContext())
            {
                var lastTrade = (from q in context.tra_market_intra_day orderby q.trade_date descending select q).FirstOrDefault();
                if (lastTrade != null)
                {
                    lastTradeDate = lastTrade.trade_date;
                }
            }

            where.AppendFormat(" intra.trade_date>='{0}'", lastTradeDate.ToString("yyyy-MM-dd"));

            if (string.IsNullOrEmpty(criteria.symbols) == false)
            {
                where.AppendFormat(" and intra.symbol in({0})", Helper.ConvertStringSQLFormat(criteria.symbols));
            }

            joinTables += " join tra_company c on c.symbol = intra.symbol ";

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

            selectFields = "intra.symbol" + Environment.NewLine +
                            ",intra.trade_date" + Environment.NewLine +
                            ",DATE_FORMAT(intra.trade_date, '%h:%i %p') as time" + Environment.NewLine +
                            ",c.open_price" + Environment.NewLine +
                            ",intra.ltp_price" + Environment.NewLine +
                            ",c.company_id" + Environment.NewLine +
                            ",intra.rsi" + Environment.NewLine +
                            ",(select ifnull(m.rsi,0) from tra_market m where m.symbol = intra.symbol and m.trade_date < date(intra.trade_date) order by m.trade_date desc limit 0,1) as prev_rsi" +
                            ",(((ifnull(intra.ltp_price, 0) - ifnull(c.open_price, 0)) / ifnull(c.open_price, 0)) * 100) as ltp_percentage" + Environment.NewLine + "";

            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);

            //Helper.Log(sql);
            List<TRA_MARKET_INTRA_DAY> rows = new List<TRA_MARKET_INTRA_DAY>();
            using (EcamContext context = new EcamContext())
            {
                rows = context.Database.SqlQuery<TRA_MARKET_INTRA_DAY>(sql).ToList();
            }
            return new PaginatedListResult<TRA_MARKET_INTRA_DAY> { total = paging.Total, rows = rows };
        }

        public PaginatedListResult<TRA_MARKET_RSI> GetRSI(TRA_COMPANY_SEARCH criteria, Paging paging)
        {
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

            where.AppendFormat(" m.trade_date>='{0}'", DateTime.Now.Date.AddDays(-400).ToString("yyyy-MM-dd"));

            where.AppendFormat(" and m.trade_date<='{0}'", DateTime.Now.Date.ToString("yyyy-MM-dd"));

            if (string.IsNullOrEmpty(criteria.symbols) == false)
            {
                where.AppendFormat(" and m.symbol in({0})", Helper.ConvertStringSQLFormat(criteria.symbols));
            }

            joinTables += " join tra_company c on c.symbol = m.symbol ";

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

            selectFields = "m.symbol" + Environment.NewLine +
                            ",m.trade_date" + Environment.NewLine +
                            ",m.close_price" + Environment.NewLine +
                            ",m.rsi" + Environment.NewLine +
                            ",m.prev_rsi" + Environment.NewLine +
                            ",c.company_id" + Environment.NewLine +
                            "";

            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);

            //Helper.Log(sql);
            List<TRA_MARKET_RSI> rows = new List<TRA_MARKET_RSI>();
            using (EcamContext context = new EcamContext())
            {
                rows = context.Database.SqlQuery<TRA_MARKET_RSI>(sql).ToList();
            }
            return new PaginatedListResult<TRA_MARKET_RSI> { total = paging.Total, rows = rows };
        }
    }
}
