
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
                string sql = string.Format("update tra_market set is_indicator=0,super_trend_signal='',ema_signal='' where symbol='{0}'",symbol);
                MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);
                IndicatorHelper sp = new IndicatorHelper();
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
                UploadFileHelper.FileCopy("TempPath",fileName,"TempPath",symbol + "_" + DateTime.Now.Date.ToString("yyyy-MM-dd").ToString() + "_" + randomNumber + ".csv");
                UploadFileHelper.DeleteFile("TempPath",fileName);
                AddSplit(symbol);
                //IndicatorHelper indicator = new IndicatorHelper();
                //indicator.Update(symbol);
                //YearLog(symbol);
            }
            return Ok(new { symbol = symbol });
        }

        private void AddSplit(string splitSymbol) {
            List<tra_market> markets = null;
            using(EcamContext context = new EcamContext()) {
                markets = (from q in context.tra_market
                           where q.symbol == splitSymbol
                           && ((((q.close_price ?? 0) - (q.prev_price ?? 0)) / (q.prev_price ?? 0)) * 100) != 0
                           && ((((q.close_price ?? 0) - (q.prev_price ?? 0)) / (q.prev_price ?? 0)) * 100) <= -48
                           //&& (q.percentage ?? 0) != 0 && (q.percentage ?? 0) <= -48
                           where (q.prev_price ?? 0) > 0
                           orderby q.trade_date ascending, q.symbol ascending
                           select q).ToList();
            }
            foreach(var market in markets) {
                string symbol = market.symbol;
                DateTime tradeDate = market.trade_date;
                decimal factor = DataTypeHelper.SafeDivision((market.prev_price ?? 0),(market.open_price ?? 0));
                tra_split split = null;

                using(EcamContext context = new EcamContext()) {
                    split = (from q in context.tra_split
                             where q.symbol == symbol
                             && q.split_date == tradeDate
                             select q).FirstOrDefault();
                    bool isNew = false;
                    if(split == null) {
                        split = new tra_split();
                        isNew = true;
                    } else {
                        split._prev_split = (from q in context.tra_split
                                             where
                                             q.id == split.id
                                             select q).FirstOrDefault();
                    }
                    if(isNew == true) {
                        split.symbol = symbol;
                        split.split_date = tradeDate;
                        split.split_factor = factor;
                        if(split.id > 0) {
                            context.Entry(split).State = System.Data.Entity.EntityState.Modified;
                        } else {
                            context.tra_split.Add(split);
                        }
                        context.SaveChanges();
                    }
                }
                if(split != null) {
                    split.UpdatePrice();
                    Console.WriteLine("Split Completed symbol=" + split.symbol + ",Date=" + split.split_date);
                }
            }
        }

        private void YearLog(string symbol) {
            DateTime startDate = Convert.ToDateTime("01/01/" + (DateTime.Now.Year).ToString());
            TradeHelper.CreateYearLog(symbol,startDate);
        }
    }
}