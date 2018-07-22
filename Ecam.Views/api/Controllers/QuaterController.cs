
using Ecam.Contracts;
using Ecam.Framework;
using Ecam.Framework.ExcelHelper;
using Ecam.Framework.Repository;
using Ecam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace Ecam.Views.Controllers {
    public class QuaterController:ApiController{

        public QuaterController()
            : this(new QuaterRepository()) {
        }

        public QuaterController(IQuaterRepository currencyRepository) {
            _QuaterRepository = currencyRepository;
        }

        IQuaterRepository _QuaterRepository;


        [ActionName("List")]
        public IHttpActionResult GetList([FromUri] TRA_COMPANY_SEARCH criteria,[FromUri] Paging paging) {
            DateTime? minDate = Convert.ToDateTime("01/01/1900");
            if(criteria.start_date == null)
                criteria.start_date = minDate;
            if(criteria.end_date == null)
                criteria.end_date = minDate;
            if(criteria.start_date.Value.Year <= 1900) {
                return BadRequest("Start date is required");
            }
            if(criteria.end_date.Value.Year <= 1900) {
                return BadRequest("End date is required");
            }
            PaginatedListResult<TRA_COMPANY_QUATER> quaterSummary = _QuaterRepository.GetQuater(criteria,paging);
            TRA_COMPANY_QUATER_SUMMARY grid = new TRA_COMPANY_QUATER_SUMMARY();
            grid.page = paging.PageIndex;
            grid.columns = new List<string>();
            List<string> periodGroups = (from q in quaterSummary.rows
                                         group q by new { q.period } into groups
                                         select groups.Key.period).ToList();
            foreach(string g in periodGroups) {
                grid.columns.Add(g);
            }
            List<TRA_COMPANY_QUATER> sortRows = null;
            if(string.IsNullOrEmpty(paging.SortName) == false) {
                sortRows = (from q in quaterSummary.rows
                            where q.period == paging.SortName
                            select q).ToList();
                if(sortRows.Count > 0) {
                    if(paging.SortOrder == "asc") {
                        sortRows = (from q in sortRows orderby q.percentage ascending select q).ToList();
                    } else {
                        sortRows = (from q in sortRows orderby q.percentage descending select q).ToList();
                    }
                    int index = 0;
                    foreach(var row in sortRows) {
                        index += 1;
                        row.index = index;
                    }
                }
            }

            List<TRA_COMPANY_QUATER_SUMMARY_DETAIL> companys = (from q in quaterSummary.rows
                                                           group q by new { q.symbol } into groups
                                                           select new TRA_COMPANY_QUATER_SUMMARY_DETAIL {
                                                               symbol = groups.Key.symbol,
                                                               index = int.MaxValue
                                                           }).ToList();
            if(sortRows != null) {
                foreach(var row in companys) {
                    var sortRow = (from q in sortRows
                                   where q.symbol == row.symbol
                                   select q).FirstOrDefault();
                    if(sortRow != null) {
                        row.index = sortRow.index;
                    }
                }
            }
            companys = (from q in companys orderby q.index ascending select q).ToList();
            grid.rows = new List<TRA_COMPANY_QUATER_SUMMARY_DETAIL>();
            foreach(var ag in companys) {
                string symbol = ag.symbol;
                TRA_COMPANY_QUATER company = (from q in quaterSummary.rows
                                              where q.symbol == symbol
                                              select q).FirstOrDefault();
                TRA_COMPANY_QUATER_SUMMARY_DETAIL row = new TRA_COMPANY_QUATER_SUMMARY_DETAIL {
                    symbol = symbol,
                    company_name = (company != null ? company.company_name : ""),
                    cell = new List<TRA_COMPANY_QUATER> { }
                };
                for(int i = 0;i < grid.columns.Count();i++) {
                    TRA_COMPANY_QUATER quater = (from q in quaterSummary.rows
                                              where q.symbol == symbol
                                              && q.period == grid.columns[i]
                                              select q).FirstOrDefault();
                    if(quater == null) {
                        TRA_COMPANY_QUATER firstQuater = (from q in quaterSummary.rows
                                                       where q.period == grid.columns[i]
                                                       select q).FirstOrDefault();
                        if(firstQuater != null) { 
                            quater = new TRA_COMPANY_QUATER {
                                symbol = symbol,
                                company_name = (company != null ? company.company_name:""),
                                value = 0,
                                prev_value = 0,
                                next_quater_first_date = firstQuater.next_quater_first_date,
                                next_quater_last_date = firstQuater.next_quater_last_date,
                                period = firstQuater.period,
                                quater_first_date = firstQuater.quater_first_date,
                                quater_last_date = firstQuater.quater_last_date
                            };
                        }
                    }
                    if(quater != null) {
                        row.cell.Add(quater);
                    }
                }
                grid.rows.Add(row);
            }
            grid.total = grid.rows.Count();
            return Ok(grid);
        }

    }
}
