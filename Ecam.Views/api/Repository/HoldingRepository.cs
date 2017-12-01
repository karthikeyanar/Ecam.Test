
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

    public interface IHoldingRepository
    {
        PaginatedListResult<TRA_HOLDING> Get(TRA_HOLDING_SEARCH criteria, Paging paging);
    }

    public class HoldingRepository : IHoldingRepository
    {
        public PaginatedListResult<TRA_HOLDING> Get(TRA_HOLDING_SEARCH criteria, Paging paging)
        {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from tra_holding h {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            if ((criteria.id ?? 0) > 0)
            {
                where.AppendFormat(" h.holding_id={0}", criteria.id);
            }
            else
            {
                where.AppendFormat(" h.holding_id>0 ");
            }

            if (string.IsNullOrEmpty(criteria.symbols) == false)
            {
                where.AppendFormat(" and h.symbol in({0})", Helper.ConvertStringSQLFormat(criteria.symbols));
            }

            joinTables += " left outer join tra_company c on c.symbol = h.symbol ";

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
            selectFields = "count(*) as cnt" + Environment.NewLine +
                           "";
            sql = string.Format(sqlFormat, selectFields, joinTables, where, "", "", "");
            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString, sql));

            orderBy = string.Format("order by {0} {1}", paging.SortName, paging.SortOrder);
            selectFields = "h.holding_id as id" + Environment.NewLine +
                           ",h.*" + Environment.NewLine +
                           ",(((ifnull(c.ltp_price, 0) - ifnull(h.avg_price, 0)) / ifnull(h.avg_price, 0)) * 100) as change_percentage" + Environment.NewLine +
                           ",(ifnull(h.quantity, 0) * ifnull(h.avg_price, 0)) as investment" + Environment.NewLine +
                           ",(ifnull(h.quantity, 0) * ifnull(c.ltp_price, 0)) as current_market_value" + Environment.NewLine +
                           ",(ifnull(h.quantity, 0) * ifnull(c.ltp_price, 0)) - (ifnull(h.quantity, 0) * ifnull(h.avg_price, 0)) as change_value" + Environment.NewLine +
                           ",c.company_name" + Environment.NewLine +
                           ",c.ltp_price" + Environment.NewLine +
                           "";
            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);
            //Helper.Log(sql);
            List<TRA_HOLDING> rows = new List<TRA_HOLDING>();
            using (EcamContext context = new EcamContext())
            {
                rows = context.Database.SqlQuery<TRA_HOLDING>(sql).ToList();
            }
            return new PaginatedListResult<TRA_HOLDING> { total = paging.Total, rows = rows };
        }
    }
}
