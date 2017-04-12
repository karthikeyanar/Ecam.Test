
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
    public class ExpertsController : BaseApiController<TRA_EXPERTS, tra_experts>
    {

        public ExpertsController()
            : this(new ExpertsRepository())
        {
        }

        public ExpertsController(IExpertsRepository currencyRepository)
        {
            _ExpertsRepository = currencyRepository;
        }

        IExpertsRepository _ExpertsRepository;

        [HttpGet]
        [ActionName("Total")]
        public IHttpActionResult GetTotalExpertss()
        {
            return Ok(_ExpertsRepository.GetTotal());
        }

        public override TRA_EXPERTS Get(int? id)
        {
            return _ExpertsRepository.Get(new TRA_EXPERTS_SEARCH { id = id }, new Paging { }).rows.FirstOrDefault();
        }

        public override PaginatedListResult<TRA_EXPERTS> Search([FromUri] TRA_EXPERTS criteria, [FromUri] Paging paging)
        {
            throw new Exception("Not available");
        }

        [HttpGet]
        [ActionName("List")]
        public PaginatedListResult<TRA_EXPERTS> List([FromUri] TRA_EXPERTS_SEARCH criteria, [FromUri] Paging paging)
        {
            return _ExpertsRepository.Get(criteria, paging);
        }

        [HttpGet]
        [ActionName("Select")]
        public List<AutoCompleteList> GetExpertss([FromUri] string term, [FromUri] int pageSize = 50)
        {
            return _ExpertsRepository.GetExpertss(term, pageSize);
        }

        public override IHttpActionResult Post(TRA_EXPERTS contract)
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
                return Ok(_ExpertsRepository.Get(new Contracts.TRA_EXPERTS_SEARCH { id = saveContract.id }, new Paging { }).rows.FirstOrDefault());
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

        public override IHttpActionResult Put(int id, TRA_EXPERTS contract)
        {
            if (contract == null)
            {
                return BadRequest("Contract is null");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            contract.id = id;
            var saveContract = Repository.Save(contract);
            if (saveContract.Errors == null)
            {
                return Ok(_ExpertsRepository.Get(new Contracts.TRA_EXPERTS_SEARCH { id = saveContract.id }, new Paging { }).rows.FirstOrDefault());
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

        [Authorize(Roles = "CA,CM")]
        public override IHttpActionResult Delete(int id)
        {
            return base.Delete(id);
        }
    }
}