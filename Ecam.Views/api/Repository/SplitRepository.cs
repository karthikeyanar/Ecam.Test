
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

    public interface ISplitRepository
    {
        PaginatedListResult<TRA_SPLIT> Get(TRA_SPLIT_SEARCH criteria, Paging paging);
    }

    public class SplitRepository : ISplitRepository
    {
        public PaginatedListResult<TRA_SPLIT> Get(TRA_SPLIT_SEARCH criteria, Paging paging)
        {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from tra_split s {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            if ((criteria.id ?? 0) > 0)
            {
                where.AppendFormat(" s.split_id={0}", criteria.id);
            }
            else
            {
                where.AppendFormat(" s.split_id>0 ");
            }

            if (string.IsNullOrEmpty(criteria.symbols) == false)
            {
                where.AppendFormat(" and s.symbol in({0})", Helper.ConvertStringSQLFormat(criteria.symbols));
            }

            joinTables += " left outer join tra_company c on c.symbol = s.symbol ";

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
            selectFields = "s.split_id as id" + Environment.NewLine +
                           ",c.company_name" + Environment.NewLine +
                           ",s.*" + Environment.NewLine +
                           "";
            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);
            List<TRA_SPLIT> rows = new List<TRA_SPLIT>();
            using (EcamContext context = new EcamContext())
            {
                rows = context.Database.SqlQuery<TRA_SPLIT>(sql).ToList();
            }
            return new PaginatedListResult<TRA_SPLIT> { total = paging.Total, rows = rows };
        }
    }
}
