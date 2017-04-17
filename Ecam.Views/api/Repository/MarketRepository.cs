
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

    public interface IMarketRepository
    {
        PaginatedListResult<TRA_MARKET> Get(TRA_MARKET_SEARCH criteria, Paging paging);
    }

    public class MarketRepository : IMarketRepository
    {
        public PaginatedListResult<TRA_MARKET> Get(TRA_MARKET_SEARCH criteria, Paging paging)
        {
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

            where.Append(string.Format(" ct.trade_date>='{0}'", criteria.start_date.Value.ToString("yyyy-MM-dd")));
            where.Append(string.Format(" and ct.trade_date<='{0}'", criteria.end_date.Value.ToString("yyyy-MM-dd")));

            if (string.IsNullOrEmpty(criteria.symbols) == false)
            {
                where.AppendFormat(" and ct.symbol in({0})", Helper.ConvertStringSQLFormat(criteria.symbols));
            }
            if ((criteria.id ?? 0) > 0)
            {
                where.AppendFormat(" and ct.market_id={0}", criteria.id);
            }
           
            if (string.IsNullOrEmpty(criteria.categories) == false)
            {
                string categorySymbols = "-1";
                using (EcamContext context = new EcamContext())
                {
                    List<string> categoryList = Helper.ConvertStringList(criteria.categories);
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
                    where.AppendFormat(" and ct.symbol in({0})", Helper.ConvertStringSQLFormat(categorySymbols));
                }
            }

            joinTables += " join tra_company c on c.symbol = ct.symbol ";

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

            selectFields = "ct.*" +
                           ",c.company_name as company_name" +
                           ",c.week_52_low" +
                           ",c.week_52_high" +
                           ",(((ifnull(ct.ltp_price, 0) - ifnull(ct.prev_price, 0)) / ifnull(ct.prev_price, 0)) * 100) as prev_percentage" +
                           ",(((ifnull(ct.ltp_price, 0) - ifnull(c.week_52_high, 0)) / ifnull(c.week_52_high, 0)) * 100) as week_52_percentage" +
                           ",(((ifnull(ct.ltp_price, 0) - ifnull(c.week_52_low, 0)) / ifnull(c.week_52_low, 0)) * 100) as week_52_low_percentage" +
                           ",ct.market_id as id"
                           ;

            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);

            List<TRA_MARKET> rows = new List<TRA_MARKET>();
            using (EcamContext context = new EcamContext())
            {
                rows = context.Database.SqlQuery<TRA_MARKET>(sql).ToList();
            }
            return new PaginatedListResult<TRA_MARKET> { total = paging.Total, rows = rows };
        }
    }
}
