
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
    public class CategoryController:BaseApiController<TRA_CATEGORY,tra_category> {
        public CategoryController()
            : this(new CategoryRepository()) {
        }

        public CategoryController(ICategoryRepository currencyRepository) {
            _CategoryRepository = currencyRepository;
        }

        ICategoryRepository _CategoryRepository;

        [HttpGet]
        [ActionName("List")]
        public PaginatedListResult<TRA_CATEGORY> List([FromUri] TRA_CATEGORY_SEARCH criteria,[FromUri] Paging paging) {
            return _CategoryRepository.Get(criteria,paging);
        }

        [HttpGet]
        [ActionName("GetMonthlyProfitCategories")]
        public List<string> GetMonthlyProfitCategories([FromUri] DateTime startDate,[FromUri] DateTime endDate) {
            return _CategoryRepository.GetMonthlyProfitCategories(startDate,endDate);
        }

        [HttpPost]
        [ActionName("UpdateArchive")]
        public IHttpActionResult UpdateArchive() {
            string category_name = HttpContext.Current.Request["category_name"];
            string is_archive = HttpContext.Current.Request["is_archive"];
            this.UpdateArchive(category_name,(is_archive == "true"));
            return Ok();
        }

        [HttpPost]
        [ActionName("UpdateBookMark")]
        public IHttpActionResult UpdateBookMark() {
            string category_name = HttpContext.Current.Request["category_name"];
            string is_book_mark = HttpContext.Current.Request["is_book_mark"];
            this.UpdateBookMark(category_name,(is_book_mark == "true"));
            return Ok();
        }

        private void UpdateArchive(string category_name,bool is_archive) {
            using(EcamContext context = new EcamContext()) {
                tra_category category = (from q in context.tra_category
                                         where q.category_name == category_name
                                         select q).FirstOrDefault();
                if(category != null) {
                    category.is_archive = is_archive;
                    context.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        private void UpdateBookMark(string category_name,bool is_book_mark) {
            using(EcamContext context = new EcamContext()) {
                tra_category category = (from q in context.tra_category
                                         where q.category_name == category_name
                                         select q).FirstOrDefault();
                if(category != null) {
                    category.is_book_mark = is_book_mark;
                    context.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }
    }
}
