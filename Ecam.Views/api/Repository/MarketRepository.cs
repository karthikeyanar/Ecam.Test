
using Ecam.Contracts;
using Ecam.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Framework.Repository {

    public interface IMarketRepository {
        PaginatedListResult<TRA_MARKET> Get(TRA_COMPANY_SEARCH criteria,Paging paging);
    }

    public class MarketRepository:IMarketRepository {
        public PaginatedListResult<TRA_MARKET> Get(TRA_COMPANY_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string prefix = "ct";
            string sqlFormat = "select {0} from tra_market " + prefix + " {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            where.Append(string.Format(" ct.trade_date>='{0}'",criteria.start_date.Value.ToString("yyyy-MM-dd")));
            where.Append(string.Format(" and ct.trade_date<='{0}'",criteria.end_date.Value.ToString("yyyy-MM-dd")));

            if(string.IsNullOrEmpty(criteria.symbols) == false) {
                where.AppendFormat(" and ct.symbol in({0})",Helper.ConvertStringSQLFormat(criteria.symbols));
            }

            if(string.IsNullOrEmpty(criteria.ema_signal) == false) {
                where.AppendFormat(" and ct.ema_signal='{0}' ",criteria.ema_signal);
            }

            if(string.IsNullOrEmpty(criteria.super_trend_signal) == false) {
                where.AppendFormat(" and ct.super_trend_signal='{0}' ",criteria.super_trend_signal);
            }

            if(string.IsNullOrEmpty(criteria.macd_signal) == false) {
                where.AppendFormat(" and ct.macd_signal='{0}' ",criteria.macd_signal);
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

            //if (criteria.is_archive.HasValue)
            //{
            where.AppendFormat(" and ifnull(c.is_archive,0)={0}",((criteria.is_archive ?? false) == true ? "1" : "0"));
            //}

            if(criteria.is_book_mark.HasValue) {
                where.AppendFormat(" and ifnull(c.is_book_mark,0)={0}",((criteria.is_book_mark ?? false) == true ? "1" : "0"));
            }

            if(criteria.is_macd_check.HasValue) {
                if((criteria.is_macd_check ?? false) == true) {
                    where.Append(" and ifnull(ct.macd,0)<=0 and ifnull(ct.macd_histogram,0)<=0 ");
                }
            }

            joinTables += " join tra_company c on c.symbol = ct.symbol ";

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

            if(string.IsNullOrEmpty(paging.SortName) == false) {
                orderBy += ",ct.trade_date asc";
            }

            selectFields = "ct.*" +
                           ",c.is_archive" +
                           ",c.is_book_mark" +
                           ",c.company_name as company_name" +
                           ",c.money_control_url" +
                           ",(select m.close_price from tra_market m where m.trade_date >= ct.trade_date and m.symbol = ct.symbol and (ifnull(m.super_trend_signal,'')='S' or ifnull(m.super_trend_signal,'')='') order by m.trade_date desc limit 0,1) as current_price" +
                           ",(select m.trade_date from tra_market m where m.trade_date >= ct.trade_date and m.symbol = ct.symbol and (ifnull(m.super_trend_signal,'')='S' or ifnull(m.super_trend_signal,'')='') order by m.trade_date desc limit 0,1) as last_date" +
                           ""
                           ;

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            Helper.Log(sql);

            List<TRA_MARKET> rows = new List<TRA_MARKET>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<TRA_MARKET>(sql).ToList();
            }
            return new PaginatedListResult<TRA_MARKET> { total = paging.Total,rows = rows };
        }
    }
}
