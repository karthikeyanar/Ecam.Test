﻿
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


namespace Ecam.Views.Controllers
{
    public class CompanyController : BaseApiController<TRA_COMPANY, tra_company>
    {

        public CompanyController()
            : this(new CompanyRepository())
        {
        }

        public CompanyController(ICompanyRepository currencyRepository)
        {
            _CompanyRepository = currencyRepository;
        }

        ICompanyRepository _CompanyRepository;

        public override TRA_COMPANY Get(int? id)
        {
            return _CompanyRepository.Get(new TRA_COMPANY_SEARCH { id = id }, new Paging { }).rows.FirstOrDefault();
        }

        public override PaginatedListResult<TRA_COMPANY> Search([FromUri] TRA_COMPANY criteria, [FromUri] Paging paging)
        {
            throw new Exception("Not available");
        }

        [HttpGet]
        [ActionName("List")]
        public PaginatedListResult<TRA_COMPANY> List([FromUri] TRA_COMPANY_SEARCH criteria, [FromUri] Paging paging)
        {
            return _CompanyRepository.Get(criteria, paging);
        }

        [HttpGet]
        [ActionName("Export")]
        public IHttpActionResult Export([FromUri] TRA_COMPANY_SEARCH criteria, [FromUri] Paging paging)
        {
            PaginatedListResult<TRA_COMPANY> obj = _CompanyRepository.Get(criteria, paging);
            List<TRA_COMPANY> rows = obj.rows.ToList();
            List<Ecam.Framework.CSVColumn> columnFormats = new List<Ecam.Framework.CSVColumn>();
            columnFormats.Add(new CSVColumn { DisplayName = "Mark", PropertyName = "is_book_mark" });
            columnFormats.Add(new CSVColumn { DisplayName = "Company", PropertyName = "company_name" });
            columnFormats.Add(new CSVColumn { DisplayName = "Symbol", PropertyName = "symbol" });
            columnFormats.Add(new CSVColumn { DisplayName = "Category", PropertyName = "category_name" });
            columnFormats.Add(new CSVColumn { DisplayName = "LTP", PropertyName = "ltp_price", IsNumber = true });
            columnFormats.Add(new CSVColumn { DisplayName = "Change %", PropertyName = "prev_percentage", IsPercentage = true });
            columnFormats.Add(new CSVColumn { DisplayName = "Last 5 Days %", PropertyName = "last_5_day_percentage", IsPercentage = true });
            columnFormats.Add(new CSVColumn { DisplayName = "Last 10 Days %", PropertyName = "last_10_day_percentage", IsPercentage = true });
            columnFormats.Add(new CSVColumn { DisplayName = "Last 1 Month %", PropertyName = "last_1_month_percentage", IsPercentage = true });
            columnFormats.Add(new CSVColumn { DisplayName = "Last 2 Month %", PropertyName = "last_2_month_percentage", IsPercentage = true });
            columnFormats.Add(new CSVColumn { DisplayName = "Last 3 Month %", PropertyName = "last_3_month_percentage", IsPercentage = true });
            columnFormats.Add(new CSVColumn { DisplayName = "Week 52 Low %", PropertyName = "week_52_low_percentage", IsPercentage = true });
            columnFormats.Add(new CSVColumn { DisplayName = "Week 52 High %", PropertyName = "week_52_percentage", IsPercentage = true });
            return new ExportToCSV<List<TRA_COMPANY>>(string.Format("{0}", DateTime.Now.ToString("dd_MMM_yyyy")), columnFormats, rows);
        }

        [HttpGet]
        [ActionName("Select")]
        public List<Select2List> GetCompanys([FromUri] string term, [FromUri] int pageSize = 50, string categories = "")
        {
            return _CompanyRepository.GetCompanys(term, pageSize, categories);
        }


        [HttpPost]
        [ActionName("UpdateBookMark")]
        public IHttpActionResult UpdateBookMark()
        {
            string symbol = HttpContext.Current.Request["symbol"];
            string is_book_mark = HttpContext.Current.Request["is_book_mark"];
            using (EcamContext context = new EcamContext())
            {
                tra_company company = (from q in context.tra_company
                                       where q.symbol == symbol
                                       select q).FirstOrDefault();
                if (company != null)
                {
                    company.is_book_mark = (is_book_mark == "true");
                    context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            return Ok();
        }

        [HttpGet]
        [ActionName("SelectCategories")]
        public List<Select2List> GetSelectCategories([FromUri] string term, [FromUri] int pageSize = 50)
        {
            return _CompanyRepository.GetCategories(term, pageSize);
        }

        public override IHttpActionResult Post(TRA_COMPANY contract)
        {
            if (contract == null)
            {
                return BadRequest("Contract is null");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var saveContract = Repository.Save(contract);
            if (saveContract.Errors == null)
            {
                using (EcamContext context = new EcamContext())
                {
                    var companyCategories = (from q in context.tra_company_category
                                             where q.symbol == contract.symbol
                                             select q).ToList();
                    foreach (var row in companyCategories)
                    {
                        context.tra_company_category.Remove(row);
                    }
                    context.SaveChanges();
                    if (string.IsNullOrEmpty(contract.category_name) == false)
                    {
                        List<string> categories = Helper.ConvertStringList(contract.category_name);
                        foreach (var category in categories)
                        {
                            tra_company_category row = (from q in context.tra_company_category
                                                        where q.symbol == contract.symbol
                                                        && q.category_name == category
                                                        select q).FirstOrDefault();
                            if (row == null)
                            {
                                context.tra_company_category.Add(new tra_company_category
                                {
                                    symbol = contract.symbol,
                                    category_name = category
                                });
                            }
                        }
                        context.SaveChanges();
                    }
                }
                return Ok(_CompanyRepository.Get(new TRA_COMPANY_SEARCH { id = saveContract.id }, new Paging { }).rows.FirstOrDefault());
            }
            else
            {
                int index = 0;
                foreach (var err in saveContract.Errors)
                {
                    index++;
                    ModelState.AddModelError("Error" + index, err.ErrorMessage);
                }
                return BadRequest(ModelState);
            }
        }


        public override IHttpActionResult Put(int id, TRA_COMPANY contract)
        {
            throw new Exception { };
        }

        public override IHttpActionResult Delete(int id)
        {
            return base.Delete(id);
        }
    }
}