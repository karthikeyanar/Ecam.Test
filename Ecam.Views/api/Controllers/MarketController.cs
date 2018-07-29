
using Ecam.Contracts;
using Ecam.Framework;
using Ecam.Framework.Repository;
using Ecam.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace Ecam.Views.Controllers {
    public class MarketController:BaseApiController<TRA_COMPANY,tra_company> {

        public MarketController()
            : this(new MarketRepository()) {
        }

        public MarketController(IMarketRepository currencyRepository) {
            _MarketRepository = currencyRepository;
        }

        IMarketRepository _MarketRepository;

        [HttpGet]
        [ActionName("List")]
        public PaginatedListResult<TRA_MARKET> List([FromUri] TRA_COMPANY_SEARCH criteria,[FromUri] Paging paging) {
            return _MarketRepository.Get(criteria,paging);
        }

        [HttpPost]
        [ActionName("UpdateSuperTrend")]
        public IHttpActionResult UpdateSuperTrend() {
            string symbol = Convert.ToString(HttpContext.Current.Request["symbol"]);
            if(string.IsNullOrEmpty(symbol) == false) {
                string sql = string.Format("update tra_market set is_indicator=0,super_trend_signal='',macd_signal='' where symbol='{0}'",symbol);
                MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);
                SupertrendData sp = new SupertrendData();
                sp.Update(symbol);
            }
            return Ok();
        }

    }
}