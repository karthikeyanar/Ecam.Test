
using Ecam.Contracts;
using Ecam.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Framework.Repository {

    public interface IFinancialCategoryRepository {
        PaginatedListResult<TRA_FINANCIAL_CATEGORY> Get(TRA_FINANCIAL_CATEGORY_SEARCH criteria,Paging paging);
    }

    public class FinancialCategoryRepository:IFinancialCategoryRepository {
        public PaginatedListResult<TRA_FINANCIAL_CATEGORY> Get(TRA_FINANCIAL_CATEGORY_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from tra_financial_category c {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            if((criteria.id ?? 0) > 0) {
                where.AppendFormat(" c.financial_category_id={0}",criteria.id);
            } else {
                where.AppendFormat(" c.financial_category_id>0 ");
            }

            where.AppendFormat(" and ifnull(c.is_archive,0)={0}",((criteria.is_archive ?? false) == true ? "1" : "0"));

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
            selectFields = "count(*) as cnt" + Environment.NewLine +
                           "";
            sql = string.Format(sqlFormat,selectFields,joinTables,where,"","","");
            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);
            selectFields = "c.financial_category_id as id" + Environment.NewLine +
                           ",c.*" + Environment.NewLine +
                           "";
            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);
            Helper.Log(sql);
            List<TRA_FINANCIAL_CATEGORY> rows = new List<TRA_FINANCIAL_CATEGORY>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<TRA_FINANCIAL_CATEGORY>(sql).ToList();
            }
            return new PaginatedListResult<TRA_FINANCIAL_CATEGORY> { total = paging.Total,rows = rows };
        }
    }
}
