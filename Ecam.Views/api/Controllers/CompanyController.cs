
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
    public class CompanyController:BaseApiController<TRA_COMPANY,tra_company> {

        public CompanyController()
            : this(new CompanyRepository()) {
        }

        public CompanyController(ICompanyRepository currencyRepository) {
            _CompanyRepository = currencyRepository;
        }

        ICompanyRepository _CompanyRepository;

        public override TRA_COMPANY Get(int? id) {
            return _CompanyRepository.Get(new TRA_COMPANY_SEARCH { id = id },new Paging { }).rows.FirstOrDefault();
        }

        public override PaginatedListResult<TRA_COMPANY> Search([FromUri] TRA_COMPANY criteria,[FromUri] Paging paging) {
            throw new Exception("Not available");
        }

        [HttpGet]
        [ActionName("List")]
        public PaginatedListResult<TRA_COMPANY> List([FromUri] TRA_COMPANY_SEARCH criteria,[FromUri] Paging paging) {
            return _CompanyRepository.Get(criteria,paging);
        }

        [HttpGet]
        [ActionName("Companies")]
        public PaginatedListResult<TRA_COMPANY> Companies([FromUri] TRA_COMPANY_SEARCH criteria,[FromUri] Paging paging) {
            return _CompanyRepository.GetCompanies(criteria,paging);
        }

        [HttpGet]
        [ActionName("BatchList")]
        public List<BatchLog> BatchList([FromUri] TRA_COMPANY_SEARCH criteria,[FromUri] Paging paging) {
            return _CompanyRepository.GetBatchLog(criteria,paging);
        }

        [HttpGet]
        [ActionName("DailyList")]
        public List<DailySummary> DailyList([FromUri] TRA_COMPANY_SEARCH criteria,[FromUri] Paging paging) {
            return _CompanyRepository.GetDailySummary(criteria,paging);
        }

        [HttpGet]
        [ActionName("CategoryGroups")]
        public List<TRA_CATEGORY_GROUP> CategoryGroups([FromUri] TRA_COMPANY_SEARCH criteria,[FromUri] Paging paging) {
            paging.PageSize = 5000;
            paging.PageIndex = 1;
            List<TRA_COMPANY> rows = _CompanyRepository.Get(criteria,paging).rows.ToList();
            List<TRA_CATEGORY_GROUP> groups = new List<TRA_CATEGORY_GROUP>();
            foreach(var row in rows) {
                TRA_CATEGORY_GROUP g = null;
                foreach(var cat in row.category_list) {
                    g = (from q in groups
                         where q.category_name == cat
                         select q).FirstOrDefault();
                    if(g == null) {
                        g = new TRA_CATEGORY_GROUP { category_name = cat,companies = new List<TRA_COMPANY>() };
                        groups.Add(g);
                    }
                    if(g != null) {
                        g.companies.Add(row);
                    }
                }
            }
            foreach(var g in groups) {
                g.companies = (from q in g.companies
                               where q.profit > 0
                               orderby q.profit descending
                               select q).Take(10).ToList();
                g.total_investment = (from q in g.companies select q.first_price).Sum();
                g.total_current = (from q in g.companies select q.last_price).Sum();
                g.total_high = (from q in g.companies select q.high_price).Sum();
                g.total_low = (from q in g.companies select q.low_price).Sum();
            }
            groups = (from q in groups
                      orderby q.total_profit descending
                      select q).ToList();
            return groups;
        }

        [HttpGet]
        [ActionName("MarketList")]
        public PaginatedListResult<TRA_COMPANY> MarketList([FromUri] TRA_COMPANY_SEARCH criteria,[FromUri] Paging paging) {
            return _CompanyRepository.GetMarketList(criteria,paging);
        }

        [HttpGet]
        [ActionName("MonthlyAVG")]
        public PaginatedListResult<TRA_COMPANY> MonthlyAVG([FromUri] TRA_COMPANY_SEARCH criteria,[FromUri] Paging paging) {
            return _CompanyRepository.GetMonthlyAVG(criteria,paging);
        }

        [HttpGet]
        [ActionName("IntradayList")]
        public PaginatedListResult<TRA_MARKET_INTRA_DAY> IntradayList([FromUri] TRA_COMPANY_SEARCH criteria,[FromUri] Paging paging) {
            return _CompanyRepository.GetIntraDay(criteria,paging);
        }

        [HttpGet]
        [ActionName("AvgList")]
        public PaginatedListResult<TRA_MARKET_AVG> AvgList([FromUri] TRA_COMPANY_SEARCH criteria,[FromUri] Paging paging) {
            return _CompanyRepository.GetAvg(criteria,paging);
        }

        [HttpGet]
        [ActionName("RSIList")]
        public PaginatedListResult<TRA_MARKET_RSI> RSIList([FromUri] TRA_COMPANY_SEARCH criteria,[FromUri] Paging paging) {
            return _CompanyRepository.GetRSI(criteria,paging);
        }

        [HttpGet]
        [ActionName("Export")]
        public IHttpActionResult Export([FromUri] TRA_COMPANY_SEARCH criteria,[FromUri] Paging paging) {
            PaginatedListResult<TRA_COMPANY> obj = _CompanyRepository.Get(criteria,paging);
            List<TRA_COMPANY> rows = obj.rows.ToList();
            List<Ecam.Framework.CSVColumn> columnFormats = new List<Ecam.Framework.CSVColumn>();
            columnFormats.Add(new CSVColumn { DisplayName = "Archive",PropertyName = "is_archive" });
            columnFormats.Add(new CSVColumn { DisplayName = "Company",PropertyName = "company_name" });
            columnFormats.Add(new CSVColumn { DisplayName = "Symbol",PropertyName = "symbol" });
            columnFormats.Add(new CSVColumn { DisplayName = "Category",PropertyName = "category_name" });
            columnFormats.Add(new CSVColumn { DisplayName = "LTP",PropertyName = "ltp_price",IsNumber = true });
            columnFormats.Add(new CSVColumn { DisplayName = "Change %",PropertyName = "prev_percentage",IsPercentage = true });
            columnFormats.Add(new CSVColumn { DisplayName = "Week 52 Low",PropertyName = "week_52_low",IsPercentage = true });
            columnFormats.Add(new CSVColumn { DisplayName = "Week 52 Low %",PropertyName = "week_52_low_percentage",IsPercentage = true });
            columnFormats.Add(new CSVColumn { DisplayName = "Week 52 High",PropertyName = "week_52_high",IsPercentage = true });
            columnFormats.Add(new CSVColumn { DisplayName = "Week 52 High %",PropertyName = "week_52_percentage",IsPercentage = true });
            return new ExportToCSV<List<TRA_COMPANY>>(string.Format("{0}",DateTime.Now.ToString("dd_MMM_yyyy")),columnFormats,rows);
        }

        [HttpGet]
        [ActionName("Select")]
        public List<Select2List> GetCompanys([FromUri] string term,[FromUri] int pageSize = 50,string categories = ""
            ,bool isCheckFinancial = false
            ,string financialDate = "") {
            return _CompanyRepository.GetCompanys(term,pageSize,categories,isCheckFinancial,financialDate);
        }

        [HttpGet]
        [ActionName("GetNSEUpdate")]
        public List<TRA_NSE_UPDATE> GetNSEUpdate() {
            List<TRA_NSE_UPDATE> companies = new List<TRA_NSE_UPDATE>();
            DateTime last_trade_date = DataTypeHelper.ToDateTime(HttpContext.Current.Request["last_trade_date"]);
            bool is_book_mark_category = DataTypeHelper.ToBoolean(HttpContext.Current.Request["is_book_mark_category"]);
            if(last_trade_date.Year > 0) {
                using(EcamContext context = new EcamContext()) {
                    List<string> categorySymbols = new List<string>();
                    IQueryable<tra_company> query = context.tra_company;
                    if(is_book_mark_category == true) {
                        categorySymbols = (from q in context.tra_company_category
                                           join c in context.tra_category on q.category_name equals c.category_name
                                           where c.is_book_mark == true
                                           select q.symbol).ToList();
                        query = (from q in query
                                 where categorySymbols.Contains(q.symbol) == true
                                 select q);
                    }
                    List<string> symbols = (from q in query
                                            select q.symbol).ToList();
                    foreach(string symbol in symbols) {
                        var lastMarket = (from q in context.tra_market
                                          join c in context.tra_company on q.symbol equals c.symbol
                                          where q.symbol == symbol
                                          && q.trade_date <= last_trade_date.Date
                                          orderby q.trade_date descending
                                          select new {
                                               q.trade_date,
                                               q.symbol,
                                               c.nse_type
                                          }).FirstOrDefault();
                        if(lastMarket != null) {
                            if(lastMarket.trade_date.Date != last_trade_date.Date) {
                                companies.Add(new TRA_NSE_UPDATE {
                                    symbol = lastMarket.symbol,
                                    start_date = lastMarket.trade_date.Date.ToString("dd-MM-yyyy"),
                                    end_date = last_trade_date.Date.ToString("dd-MM-yyyy"),
                                    nse_type = lastMarket.nse_type
                                });
                            }
                        }
                    }
                }
            }
            return companies.Take(1).ToList();
        }

        [HttpPost]
        [ActionName("UpdateArchive")]
        public IHttpActionResult UpdateArchive() {
            string symbol = HttpContext.Current.Request["symbol"];
            string is_archive = HttpContext.Current.Request["is_archive"];
            this.UpdateArchive(symbol,(is_archive == "true"));
            return Ok();
        }

        public IHttpActionResult UpdateBookMark() {
            string symbol = HttpContext.Current.Request["symbol"];
            string is_book_mark = HttpContext.Current.Request["is_book_mark"];
            this.UpdateBookMark(symbol,(is_book_mark == "true"));
            return Ok();
        }

        private void UpdateArchive(string symbol,bool is_archive) {
            using(EcamContext context = new EcamContext()) {
                tra_company company = (from q in context.tra_company
                                       where q.symbol == symbol
                                       select q).FirstOrDefault();
                if(company != null) {
                    company.is_archive = is_archive;
                    context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    //string categoryName = "2_FAVOURITES";
                    //tra_company_category category = (from q in context.tra_company_category
                    //                                 where q.category_name == categoryName
                    //                                 && q.symbol == company.symbol
                    //                                 select q).FirstOrDefault();
                    //if ((company.is_archive ?? false) == false)
                    //{
                    //    if (category != null)
                    //    {
                    //        context.tra_company_category.Remove(category);
                    //        context.SaveChanges();
                    //    }
                    //}
                    //else
                    //{
                    //    if (category == null)
                    //    {
                    //        context.tra_company_category.Add(new tra_company_category
                    //        {
                    //            category_name = categoryName,
                    //            symbol = symbol
                    //        });
                    //        context.SaveChanges();
                    //    }
                    //}
                }
            }
        }

        private void UpdateBookMark(string symbol,bool is_book_mark) {
            using(EcamContext context = new EcamContext()) {
                tra_company company = (from q in context.tra_company
                                       where q.symbol == symbol
                                       select q).FirstOrDefault();
                if(company != null) {
                    company.is_book_mark = is_book_mark;
                    context.Entry(company).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        [HttpGet]
        [ActionName("RefreshSymbol")]
        public IHttpActionResult RefreshSymbol() {
            string symbol = HttpContext.Current.Request["symbol"];
            TradeHelper.GetUpdatePriceUsingGoogle(symbol);
            return Ok();
            //TRA_COMPANY company = null;
            //using (EcamContext context = new EcamContext()) 
            //{
            //    company = (from q in context.tra_company
            //               where q.symbol == symbol
            //               select new TRA_COMPANY
            //               {
            //                   symbol = q.symbol,
            //                   open_price = q.open_price,
            //                   high_price = q.high_price,
            //                   close_price = q.close_price,
            //                   low_price = q.low_price,
            //                   ltp_price = q.ltp_price,
            //                   prev_price = q.prev_price,
            //               }).FirstOrDefault();

            //}
            //return Ok(company);
        }

        [HttpGet]
        [ActionName("SelectCategories")]
        public List<Select2List> GetSelectCategories([FromUri] string term,[FromUri] int pageSize = 500) {
            return _CompanyRepository.GetCategories(term,pageSize);
        }

        [HttpGet]
        [ActionName("SelectMFS")]
        public List<Select2List> GetSelectMFS([FromUri] string term,[FromUri] int pageSize = 50) {
            return _CompanyRepository.GetMFFunds(term,pageSize);
        }

        public override IHttpActionResult Post(TRA_COMPANY contract) {
            if(contract == null) {
                return BadRequest("Contract is null");
            }
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            string companyName = contract.company_name;
            string symbol = contract.symbol;
            string categoryName = contract.category_name;
            bool isNifty50 = (contract.is_nifty_50 ?? false);
            bool isNifty100 = (contract.is_nifty_100 ?? false);
            bool isNifty200 = (contract.is_nifty_200 ?? false);
            bool isOld = (contract.is_old ?? false);
            string moneyControlURL = contract.money_control_url;
            string moneyControlSymbol = contract.money_control_symbol;
            string nseType = contract.nse_type;
            if((contract.id ?? 0) > 0) {
                contract = _CompanyRepository.GetCompanies(new TRA_COMPANY_SEARCH { id = contract.id },new Paging { }).rows.FirstOrDefault();
                contract.company_name = companyName;
                contract.symbol = symbol;
                contract.category_name = categoryName;
                contract.is_nifty_50 = isNifty50;
                contract.is_nifty_100 = isNifty100;
                contract.is_nifty_200 = isNifty200;
                contract.is_old = isOld;
                contract.money_control_url = moneyControlURL;
                contract.money_control_symbol = moneyControlSymbol;
                contract.nse_type = nseType;
            }
            var saveContract = Repository.Save(contract);
            if(saveContract.Errors == null) {
                using(EcamContext context = new EcamContext()) {
                    var companyCategories = (from q in context.tra_company_category
                                             where q.symbol == contract.symbol
                                             select q).ToList();
                    foreach(var row in companyCategories) {
                        context.tra_company_category.Remove(row);
                    }
                    context.SaveChanges();
                    if(string.IsNullOrEmpty(contract.category_name) == false) {
                        List<string> categories = Helper.ConvertStringList(contract.category_name);
                        foreach(var category in categories) {
                            tra_company_category row = (from q in context.tra_company_category
                                                        where q.symbol == contract.symbol
                                                        && q.category_name == category
                                                        select q).FirstOrDefault();
                            if(row == null) {
                                context.tra_company_category.Add(new tra_company_category {
                                    symbol = contract.symbol,
                                    category_name = category
                                });
                            }
                        }
                        context.SaveChanges();
                    }
                }
                return Ok(_CompanyRepository.GetCompanies(new TRA_COMPANY_SEARCH { id = saveContract.id },new Paging { }).rows.FirstOrDefault());
            } else {
                int index = 0;
                foreach(var err in saveContract.Errors) {
                    index++;
                    ModelState.AddModelError("Error" + index,err.ErrorMessage);
                }
                return BadRequest(ModelState);
            }
        }

        public override IHttpActionResult Put(int id,TRA_COMPANY contract) {
            throw new Exception { };
        }

        public override IHttpActionResult Delete(int id) {
            return base.Delete(id);
        }
    }
}
