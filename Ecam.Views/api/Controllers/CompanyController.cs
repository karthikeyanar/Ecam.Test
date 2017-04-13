
using Ecam.Contracts;
using Ecam.Framework;
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
        [ActionName("Select")]
        public List<Select2List> GetCompanys([FromUri] string term, [FromUri] int pageSize = 50, string categories = "")
        {
            return _CompanyRepository.GetCompanys(term,  pageSize, categories);
        }


        [HttpPost]
        [ActionName("UpdateBookMark")]
        public IHttpActionResult UpdateBookMark()
        {
            string symbol = HttpContext.Current.Request["symbol"];
            string is_book_mark = HttpContext.Current.Request["is_book_mark"];
            using(EcamContext context = new EcamContext())
            {
                tra_company company = (from q in context.tra_company
                                       where q.symbol == symbol
                                       select q).FirstOrDefault();
                if(company != null)
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
            return base.Post(contract);
        }

        
        public override IHttpActionResult Put(int id, TRA_COMPANY contract)
        {
            return base.Put(id, contract);
        }

        
        public override IHttpActionResult Delete(int id)
        {
            return base.Delete(id);
        }
    }
}