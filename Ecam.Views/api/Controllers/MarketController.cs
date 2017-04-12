
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
    public class MarketController : BaseApiController<TRA_MARKET, tra_market>
    {

        public MarketController()
            : this(new MarketRepository())
        {
        }

        public MarketController(IMarketRepository currencyRepository)
        {
            _MarketRepository = currencyRepository;
        }

        IMarketRepository _MarketRepository;
         
        public override TRA_MARKET Get(int? id)
        {
            return _MarketRepository.Get(new TRA_MARKET_SEARCH { id = id }, new Paging { }).rows.FirstOrDefault();
        }

        public override PaginatedListResult<TRA_MARKET> Search([FromUri] TRA_MARKET criteria, [FromUri] Paging paging)
        {
            throw new Exception("Not available");
        }

        [HttpGet]
        [ActionName("List")]
        public PaginatedListResult<TRA_MARKET> List([FromUri] TRA_MARKET_SEARCH criteria, [FromUri] Paging paging)
        {
            return _MarketRepository.Get(criteria, paging);
        }

        
        public override IHttpActionResult Post(TRA_MARKET contract)
        {
            base.Post(contract);
            return Ok(_MarketRepository.Get(new TRA_MARKET_SEARCH { id = contract.id }, new Paging { }).rows.FirstOrDefault());
        }

        
        public override IHttpActionResult Put(int id, TRA_MARKET contract)
        {
            base.Post(contract);
            return Ok(_MarketRepository.Get(new TRA_MARKET_SEARCH { id = contract.id }, new Paging { }).rows.FirstOrDefault());
        }

        
        public override IHttpActionResult Delete(int id)
        {
            return base.Delete(id);
        }
    }
}