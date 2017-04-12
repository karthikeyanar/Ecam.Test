
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
        List<Select2List> GetCompanys(string name, int pageSize = 50, string categories = "");
        List<Select2List> GetCategories(string name, int pageSize = 50);
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
                    List<string> categorySymbolList = (from q in context.tra_company_category
                                                       where categoryList.Contains(q.category_name) == true
                                                       select q.symbol).Distinct().ToList();
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
                           ",(((ifnull(ct.ltp_price, 0) - ifnull(ct.prev_price, 0)) / ifnull(ct.prev_price, 0)) * 100) as prev_percentage" +
                           ",(((ifnull(ct.ltp_price, 0) - ifnull(ct.week_52_high, 0)) / ifnull(ct.week_52_high, 0)) * 100) as week_52_percentage" +
                           ",(((ifnull(ct.ltp_price, 0) - ifnull(ct.week_52_low, 0)) / ifnull(ct.week_52_low, 0)) * 100) as week_52_low_percentage" +
                           ",ct.company_id as id"
                           ;

            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);

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
                row.category_name = (from q in companyCategories
                                     where q.symbol == row.symbol
                                     select q.category_name).FirstOrDefault();
            }
            return new PaginatedListResult<TRA_COMPANY> { total = paging.Total, rows = rows };
        }
    }
}
