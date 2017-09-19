
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
    public class HoldingController : BaseApiController<TRA_HOLDING, tra_holding>
    {
        public HoldingController()
            : this(new HoldingRepository())
        {
        }

        public HoldingController(IHoldingRepository currencyRepository)
        {
            _HoldingRepository = currencyRepository;
        }

        IHoldingRepository _HoldingRepository;

        public override TRA_HOLDING Get(int? id)
        {
            return _HoldingRepository.Get(new TRA_HOLDING_SEARCH { id = id }, new Paging { }).rows.FirstOrDefault();
        }

        public override PaginatedListResult<TRA_HOLDING> Search([FromUri] TRA_HOLDING criteria, [FromUri] Paging paging)
        {
            throw new Exception("Not available");
        }

        [HttpGet]
        [ActionName("List")]
        public PaginatedListResult<TRA_HOLDING> List([FromUri] TRA_HOLDING_SEARCH criteria, [FromUri] Paging paging)
        {
            return _HoldingRepository.Get(criteria, paging);
        }

        public override IHttpActionResult Post(TRA_HOLDING contract)
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
                if (string.IsNullOrEmpty(contract.symbol) == false)
                {
                    using (EcamContext context = new EcamContext())
                    {
                        tra_company company = (from q in context.tra_company
                                               where q.symbol == contract.symbol
                                               select q).FirstOrDefault();
                        if (company != null)
                        {
                            company.ltp_price = contract.ltp_price;
                            context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                }
                return Ok(_HoldingRepository.Get(new TRA_HOLDING_SEARCH { id = saveContract.id }, new Paging { }).rows.FirstOrDefault());
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
