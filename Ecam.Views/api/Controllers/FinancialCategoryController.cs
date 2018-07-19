
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
    public class FinancialCategoryController:BaseApiController<TRA_FINANCIAL_CATEGORY,tra_financial_category> {
        public FinancialCategoryController()
            : this(new FinancialCategoryRepository()) {
        }

        public FinancialCategoryController(IFinancialCategoryRepository repository) {
            _FinancialCategoryRepository = repository;
        }

        IFinancialCategoryRepository _FinancialCategoryRepository;

        [HttpGet]
        [ActionName("List")]
        public PaginatedListResult<TRA_FINANCIAL_CATEGORY> List([FromUri] TRA_FINANCIAL_CATEGORY_SEARCH criteria,[FromUri] Paging paging) {
            return _FinancialCategoryRepository.Get(criteria,paging);
        }
         
        [HttpPost]
        [ActionName("UpdateArchive")]
        public IHttpActionResult UpdateArchive() {
            string category_name = HttpContext.Current.Request["category_name"];
            string is_archive = HttpContext.Current.Request["is_archive"];
            this.UpdateArchive(category_name,(is_archive == "true"));
            return Ok();
        }

        private void UpdateArchive(string category_name,bool is_archive) {
            using(EcamContext context = new EcamContext()) {
                tra_financial_category category = (from q in context.tra_financial_category
                                         where q.category_name == category_name
                                         select q).FirstOrDefault();
                if(category != null) {
                    category.is_archive = is_archive;
                    context.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }
         
    }
}
