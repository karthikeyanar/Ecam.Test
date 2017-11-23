
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
    public class SplitController : BaseApiController<TRA_SPLIT, tra_split>
    {
        public SplitController()
            : this(new SplitRepository())
        {
        }

        public SplitController(ISplitRepository currencyRepository)
        {
            _SplitRepository = currencyRepository;
        }

        ISplitRepository _SplitRepository;

        public override TRA_SPLIT Get(int? id)
        {
            return _SplitRepository.Get(new TRA_SPLIT_SEARCH { id = id }, new Paging { }).rows.FirstOrDefault();
        }

        public override PaginatedListResult<TRA_SPLIT> Search([FromUri] TRA_SPLIT criteria, [FromUri] Paging paging)
        {
            throw new Exception("Not available");
        }

        [HttpGet]
        [ActionName("List")]
        public PaginatedListResult<TRA_SPLIT> List([FromUri] TRA_SPLIT_SEARCH criteria, [FromUri] Paging paging)
        {
            return _SplitRepository.Get(criteria, paging);
        }

        public override IHttpActionResult Post(TRA_SPLIT contract)
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
                return Ok(_SplitRepository.Get(new TRA_SPLIT_SEARCH { id = saveContract.id }, new Paging { }).rows.FirstOrDefault());
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
    }
}
