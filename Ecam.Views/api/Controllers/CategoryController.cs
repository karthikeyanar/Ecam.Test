
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
    public class CategoryController : BaseApiController<TRA_CATEGORY, tra_category>
    {
        public CategoryController()
            : this(new CategoryRepository())
        {
        }

        public CategoryController(ICategoryRepository currencyRepository)
        {
            _CategoryRepository = currencyRepository;
        }

        ICategoryRepository _CategoryRepository;

        [HttpGet]
        [ActionName("List")]
        public PaginatedListResult<TRA_CATEGORY> List([FromUri] TRA_CATEGORY_SEARCH criteria, [FromUri] Paging paging)
        {
            return _CategoryRepository.Get(criteria, paging);
        }
    }
}
