
using Ecam.Contracts;
using Ecam.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecam.Framework.Repository {
    public interface IMyCurrencyRepository{
        //List<AutoCompleteList> GetCurrencies();       
        PaginatedListResult<EC_CURRENCY> GetCurrencies(EC_CURRENCY criteria,Paging paging);
    }

    public class MyCurrencyRepository : IMyCurrencyRepository{
        /*public List<AutoCompleteList> GetCurrencies(){
            using(EcamContext context = new EcamContext()){
                IQueryable<ec_currency> ecCurrency = context.ec_currency;
                IQueryable<AutoCompleteList> query = (from currency in ecCurrency
                                                orderby currency.currency_name
                                                select new AutoCompleteList {
                                                    id = currency.id,
                                                    label = currency.currency_name,
                                                    value = currency.currency_name
                                                });
                return query.ToList();
            }
        }*/       
        public PaginatedListResult<EC_CURRENCY> GetCurrencies(EC_CURRENCY criteria,Paging paging){
            using(EcamContext context = new EcamContext()){
                IQueryable<EC_CURRENCY> query = (from currency in context.ec_currency
                                                select new EC_CURRENCY {
                                                    currency_code = currency.currency_code,
                                                    currency_name = currency.currency_name,
                                                    currency_symbol = currency.currency_symbol,
                                                    currency_remarks = currency.currency_remarks,
                                                    id = currency.id
                                                });
                paging.Total = query.Count();
                if(string.IsNullOrEmpty(paging.SortOrder))
                    paging.SortOrder = "asc";
                if(string.IsNullOrEmpty(paging.SortName) == false)                    					
                    query = query.OrderBy(paging.SortName,(paging.SortOrder == "asc"));
                if(paging.PageSize > 0)
                    query = query.Skip((paging.PageIndex-1) * paging.PageSize).Take(paging.PageSize);
                PaginatedListResult<EC_CURRENCY> paginatedlist = new PaginatedListResult<EC_CURRENCY>();
                paginatedlist.rows = query.ToList();
				paginatedlist.total = paging.Total;
                return paginatedlist;
            }
        }        
    }
}
