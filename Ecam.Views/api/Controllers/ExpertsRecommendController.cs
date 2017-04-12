
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
    public class ExpertsRecommendController : BaseApiController<TRA_EXPERTS_RECOMMEND, tra_experts_recommend>
    {

        public ExpertsRecommendController()
            : this(new ExpertsRecommendRepository())
        {
        }

        public ExpertsRecommendController(IExpertsRecommendRepository currencyRepository)
        {
            _ExpertsRecommendRepository = currencyRepository;
        }

        IExpertsRecommendRepository _ExpertsRecommendRepository;

        public override TRA_EXPERTS_RECOMMEND Get(int? id)
        {
            return _ExpertsRecommendRepository.Get(new TRA_EXPERTS_RECOMMEND_SEARCH { id = id }, new Paging { }).rows.FirstOrDefault();
        }

        [HttpGet]
        [ActionName("List")]
        public PaginatedListResult<TRA_EXPERTS_RECOMMEND> List([FromUri] TRA_EXPERTS_RECOMMEND_SEARCH criteria, [FromUri] Paging paging)
        {
            return _ExpertsRecommendRepository.Get(criteria, paging);
        }

        public override IHttpActionResult Post(TRA_EXPERTS_RECOMMEND contract)
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
                return Ok(_ExpertsRecommendRepository.Get(new Contracts.TRA_EXPERTS_RECOMMEND_SEARCH { id = saveContract.id }, new Paging { }).rows.FirstOrDefault());
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

        public override IHttpActionResult Put(int id, TRA_EXPERTS_RECOMMEND contract)
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
                return Ok(_ExpertsRecommendRepository.Get(new Contracts.TRA_EXPERTS_RECOMMEND_SEARCH { id = saveContract.id }, new Paging { }).rows.FirstOrDefault());
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