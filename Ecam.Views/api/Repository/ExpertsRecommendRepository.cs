
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

    public interface IExpertsRecommendRepository
    {
        PaginatedListResult<TRA_EXPERTS_RECOMMEND> Get(TRA_EXPERTS_RECOMMEND_SEARCH criteria, Paging paging);
    }

    public class ExpertsRecommendRepository : IExpertsRecommendRepository
    {
        public PaginatedListResult<TRA_EXPERTS_RECOMMEND> Get(TRA_EXPERTS_RECOMMEND_SEARCH criteria, Paging paging)
        {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string prefix = "ct";
            string sqlFormat = "select {0} from tra_experts_recommend " + prefix + " {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            selectFields = "count(*) as cnt";

            where.AppendFormat(" ct.recommend_id > 0");

            if (criteria.id > 0)
            {
                where.AppendFormat(" and ct.recommend_id = {0} ", criteria.id);
            }

            joinTables += " join tra_experts expt on ct.experts_id = expt.experts_id ";
            joinTables += " join tra_company comp on comp.company_id = ct.company_id ";

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
                           ",expt.experts_name" +
                           ",comp.company_name" +
                           ",ct.recommend_id as id"
                           ;

            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);

            List<TRA_EXPERTS_RECOMMEND> rows = new List<TRA_EXPERTS_RECOMMEND>();
            using (EcamContext context = new EcamContext())
            {
                rows = context.Database.SqlQuery<TRA_EXPERTS_RECOMMEND>(sql).ToList();
            }
            return new PaginatedListResult<TRA_EXPERTS_RECOMMEND> { total = paging.Total, rows = rows };
        }
    }
}
