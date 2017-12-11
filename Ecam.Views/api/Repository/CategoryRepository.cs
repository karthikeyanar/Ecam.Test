﻿
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

    public interface ICategoryRepository
    {
        PaginatedListResult<TRA_CATEGORY> Get(TRA_CATEGORY_SEARCH criteria, Paging paging);
    }

    public class CategoryRepository : ICategoryRepository
    {
        public PaginatedListResult<TRA_CATEGORY> Get(TRA_CATEGORY_SEARCH criteria, Paging paging)
        {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from tra_category c {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            if ((criteria.id ?? 0) > 0)
            {
                where.AppendFormat(" c.category_id={0}", criteria.id);
            }
            else
            {
                where.AppendFormat(" c.category_id>0 ");
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
            selectFields = "count(*) as cnt" + Environment.NewLine +
                           "";
            sql = string.Format(sqlFormat, selectFields, joinTables, where, "", "", "");
            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString, sql));

            orderBy = string.Format("order by {0} {1}", paging.SortName, paging.SortOrder);
            selectFields = "c.category_id as id" + Environment.NewLine +
                           ",c.*" + Environment.NewLine +
                           ",(select count(*) from tra_company_category p where p.category_name = c.category_name) as total_equity" + Environment.NewLine +
                           ",(select ifnull(p.profit,0) from tra_category_profit p where p.category_name = c.category_name and p.profit_type = 'Y' and p.profit_date = '2016-01-01' limit 0,1) as profit_2016" + Environment.NewLine +
                           ",(select ifnull(p.profit,0) from tra_category_profit p where p.category_name = c.category_name and p.profit_type = 'Y' and p.profit_date = '2017-01-01' limit 0,1) as profit_2017" + Environment.NewLine +
                           ",(select ifnull(p.profit,0) from tra_category_profit p where p.category_name = c.category_name and p.profit_type = 'Y' and p.profit_date = '2018-01-01' limit 0,1) as profit_2018" + Environment.NewLine +
                           "";
            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);
            //Helper.Log(sql);
            List<TRA_CATEGORY> rows = new List<TRA_CATEGORY>();
            using (EcamContext context = new EcamContext())
            {
                rows = context.Database.SqlQuery<TRA_CATEGORY>(sql).ToList();
            }
            return new PaginatedListResult<TRA_CATEGORY> { total = paging.Total, rows = rows };
        }
    }
}
