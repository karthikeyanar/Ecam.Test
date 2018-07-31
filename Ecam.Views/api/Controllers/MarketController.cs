
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

        [HttpPost]
        [ActionName("UpdateCSV")]
        public IHttpActionResult UpdateCSV() {
            string csv = Convert.ToString(HttpContext.Current.Request["csv"]);
            string symbol = string.Empty;
            if(string.IsNullOrEmpty(csv) == false) {
                csv = csv.Replace(":",Environment.NewLine);
                Random rnd = new Random();
                string randomNumber = rnd.Next(10000,1000000).ToString();
                string fileName = randomNumber + ".csv";
                UploadFileHelper.WriteFileText("TempPath",fileName,csv,true);
                string fullFileName = UploadFileHelper.GetFullFileName("TempPath",fileName);
                CSVDownloadData csvDownload = new CSVDownloadData();
                using(EcamContext context = new EcamContext()) {
                    csvDownload._SYMBOLS_LIST = (from q in context.tra_company
                                                 select q.symbol).ToList();
                }
                symbol = csvDownload.CSVDataDownload(fullFileName,true);
                SupertrendData superTrendData = new SupertrendData();
                superTrendData.Update(symbol);
            }
            return Ok(new { symbol = symbol });
        }

    }
}