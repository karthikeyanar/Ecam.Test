
using Ecam.Contracts;
using Ecam.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Framework.Repository {

    public interface IQuaterRepository {
        PaginatedListResult<TRA_COMPANY_QUATER> GetQuater(TRA_COMPANY_SEARCH criteria,Paging paging);
    }

    public class QuaterRepository:IQuaterRepository {

        public PaginatedListResult<TRA_COMPANY_QUATER> GetQuater(TRA_COMPANY_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from tra_financial f {1} where {2} {3} {4} {5}";
            string sql = string.Empty;

            where.Append(string.Format(" f.financial_date>='{0}'",criteria.start_date.Value.ToString("yyyy-MM-dd")));
            where.Append(string.Format(" and f.financial_date<='{0}'",criteria.end_date.Value.ToString("yyyy-MM-dd")));

            where.Append(string.Format(" and f.financial_category_id={0}",criteria.financial_category_id));

            criteria.is_book_mark_category = true;
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
                        where.AppendFormat(" and f.symbol in({0})",Helper.ConvertStringSQLFormat(categorySymbols));
                    }
                }
            }

            joinTables = " join tra_company company on f.symbol = company.symbol";

            selectFields = "count(*) as cnt";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

            sql = "select count(*) from (" + sql + ") as tbl";

            selectFields = " company.company_name" + Environment.NewLine +
                           ",f.symbol" + Environment.NewLine +
                           ",f.value" + Environment.NewLine +
                           ",f.prev_value" + Environment.NewLine +
                           ",case " + Environment.NewLine +
                             " when MONTH(f.financial_date) BETWEEN 4  AND 6 then concat(YEAR(f.financial_date),' ','Q1') " + Environment.NewLine +
                             " when MONTH(f.financial_date) BETWEEN 7  AND 9 then  concat(YEAR(f.financial_date),' ','Q2') " + Environment.NewLine +
                             " when MONTH(f.financial_date) BETWEEN 10  AND 12 then concat(YEAR(f.financial_date),' ','Q3') " + Environment.NewLine +
                             " when MONTH(f.financial_date) BETWEEN 1  AND 3 then  concat(YEAR(f.financial_date) - 1,' ','Q4') " + Environment.NewLine +
                            " end as period" + Environment.NewLine +
                            ",STR_TO_DATE(case " + Environment.NewLine +
                              " when MONTH(f.financial_date) BETWEEN 4  AND 6 then concat(YEAR(f.financial_date),'/04/01')" + Environment.NewLine +
                              " when MONTH(f.financial_date) BETWEEN 7  AND 9 then  concat(YEAR(f.financial_date),'/07/01')" + Environment.NewLine +
                              " when MONTH(f.financial_date) BETWEEN 10  AND 12 then concat(YEAR(f.financial_date),'/10/01')" + Environment.NewLine +
                              " when MONTH(f.financial_date) BETWEEN 1  AND 3 then  concat(YEAR(f.financial_date),'/01/01')" + Environment.NewLine +
                            " end,'%Y/%m/%d') as quater_first_date" + Environment.NewLine +
                            ",STR_TO_DATE(case " + Environment.NewLine +
                             " when MONTH(f.financial_date) BETWEEN 4  AND 6 then concat(YEAR(f.financial_date),'/06/30')" + Environment.NewLine +
                             " when MONTH(f.financial_date) BETWEEN 7  AND 9 then  concat(YEAR(f.financial_date),'/09/30')" + Environment.NewLine +
                             " when MONTH(f.financial_date) BETWEEN 10  AND 12 then concat(YEAR(f.financial_date),'/12/31')" + Environment.NewLine +
                             " when MONTH(f.financial_date) BETWEEN 1  AND 3 then  concat(YEAR(f.financial_date),'/03/31')" + Environment.NewLine +
                            " end,'%Y/%m/%d') as quater_last_date" + Environment.NewLine +
                            ",DATE_ADD(STR_TO_DATE(case " + Environment.NewLine +
                              " when MONTH(f.financial_date) BETWEEN 4  AND 6 then concat(YEAR(f.financial_date),'/04/01')" + Environment.NewLine +
                              " when MONTH(f.financial_date) BETWEEN 7  AND 9 then  concat(YEAR(f.financial_date),'/07/01')" + Environment.NewLine +
                              " when MONTH(f.financial_date) BETWEEN 10  AND 12 then concat(YEAR(f.financial_date),'/10/01')" + Environment.NewLine +
                              " when MONTH(f.financial_date) BETWEEN 1  AND 3 then  concat(YEAR(f.financial_date),'/01/01')" + Environment.NewLine +
                            " end,'%Y/%m/%d'),INTERVAL 3 MONTH) as next_quater_first_date" + Environment.NewLine +
                            ",DATE_ADD(STR_TO_DATE(case " + Environment.NewLine +
                              " when MONTH(f.financial_date) BETWEEN 4  AND 6 then concat(YEAR(f.financial_date),'/06/30')" + Environment.NewLine +
                              " when MONTH(f.financial_date) BETWEEN 7  AND 9 then  concat(YEAR(f.financial_date),'/09/30')" + Environment.NewLine +
                              " when MONTH(f.financial_date) BETWEEN 10  AND 12 then concat(YEAR(f.financial_date),'/12/31')" + Environment.NewLine +
                              " when MONTH(f.financial_date) BETWEEN 1  AND 3 then  concat(YEAR(f.financial_date),'/03/31')" + Environment.NewLine +
                            " end,'%Y/%m/%d'),INTERVAL 3 MONTH) as next_quater_last_date" + Environment.NewLine +
                            "";

            orderBy = string.Format("order by {0} {1}","f.financial_date","desc");
            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);
            Helper.Log(sql,"GetQuater");
            List<TRA_COMPANY_QUATER> rows = new List<TRA_COMPANY_QUATER>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<TRA_COMPANY_QUATER>(sql).ToList();
            }
            return new PaginatedListResult<TRA_COMPANY_QUATER> { total = paging.Total,rows = rows };
        }
    }
}
