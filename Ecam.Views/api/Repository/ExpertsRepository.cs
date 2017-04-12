
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

    public interface IExpertsRepository
    {
        PaginatedListResult<TRA_EXPERTS> Get(TRA_EXPERTS_SEARCH criteria, Paging paging);
        List<AutoCompleteList> GetExpertss(string name, int pageSize = 50);
        int? GetTotal();
    }

    public class ExpertsRepository : IExpertsRepository
    {

        public List<AutoCompleteList> GetExpertss(string name, int pageSize = 50)
        {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from tra_experts comp {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            where.AppendFormat(" comp.experts_id > 0");


            if (string.IsNullOrEmpty(name) == false)
            {
                where.AppendFormat(" and comp.experts_name like '{0}%'", name);
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

            selectFields = "comp.experts_id as id" +
                           ",comp.experts_name as label" +
                           ",comp.experts_name as value";

            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);

            List<AutoCompleteList> rows = new List<AutoCompleteList>();
            using (EcamContext context = new EcamContext())
            {
                rows = context.Database.SqlQuery<AutoCompleteList>(sql).ToList();
            }
            return rows;
        }

        public int? GetTotal()
        {
            using (EcamContext context = new EcamContext())
            {
                return context.tra_experts.Count();
            }
        }

        public PaginatedListResult<TRA_EXPERTS> Get(TRA_EXPERTS_SEARCH criteria, Paging paging)
        {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string prefix = "ct";
            string sqlFormat = "select {0} from tra_experts " + prefix + " {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            if ((criteria.id ?? 0) > 0)
            {
                where.AppendFormat(" ct.experts_id={0}", criteria.id);
            }
            else
            {
                where.AppendFormat(" ct.experts_id>0 ");

                if (string.IsNullOrEmpty(criteria.experts_name) == false)
                {
                    where.AppendFormat(" and ct.experts_name like '%{0}%", criteria.experts_name);
                }
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

            selectFields = "ct.*" +
                           ",ct.experts_id as id"
                           ;

            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);

            List<TRA_EXPERTS> rows = new List<TRA_EXPERTS>();
            using (EcamContext context = new EcamContext())
            {
                rows = context.Database.SqlQuery<TRA_EXPERTS>(sql).ToList();
            }
            return new PaginatedListResult<TRA_EXPERTS> { total = paging.Total, rows = rows };
        }
    }
}
