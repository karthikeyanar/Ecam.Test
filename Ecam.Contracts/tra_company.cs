using Ecam.Framework;
using Ecam.Framework.ExcelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Contracts
{
    public class TRA_COMPANY : BaseContract
    {
        public string company_name { get; set; }
        public string symbol { get; set; }
        public Nullable<decimal> open_price { get; set; }
        public Nullable<decimal> high_price { get; set; }
        public Nullable<decimal> low_price { get; set; }
        public Nullable<decimal> ltp_price { get; set; }
        public Nullable<decimal> close_price { get; set; }
        public Nullable<decimal> prev_price { get; set; }
        public Nullable<decimal> week_52_high { get; set; }
        public Nullable<decimal> week_52_low { get; set; }
        public Nullable<bool> is_archive { get; set; }
        public Nullable<bool> is_nifty_50 { get; set; }
        public Nullable<bool> is_nifty_100 { get; set; }
        public Nullable<bool> is_nifty_200 { get; set; }
        public Nullable<decimal> rsi { get; set; }
        public Nullable<decimal> prev_rsi { get; set; }
        public Nullable<int> mf_cnt { get; set; }
        public Nullable<decimal> mf_qty { get; set; }
        public Nullable<decimal> monthly_avg { get; set; }
        public Nullable<decimal> weekly_avg { get; set; }
        public Nullable<bool> is_book_mark { get; set; }
        public string mcstr { get; set; }
        public Nullable<decimal> mc { get; set; }
        public Nullable<decimal> pe { get; set; }
        public Nullable<decimal> volume { get; set; }
        public Nullable<decimal> eps { get; set; }
        public string money_control_url { get; set; }
        public Nullable<bool> is_old { get; set; }
        
        public string category_name { get; set; }
        public Nullable<decimal> ltp_percentage { get; set; }
        public Nullable<decimal> prev_percentage { get; set; }
        public Nullable<decimal> high_percentage { get; set; }
        public Nullable<decimal> low_percentage { get; set; }
        public Nullable<decimal> week_52_low_percentage { get; set; }
        public Nullable<decimal> week_52_percentage { get; set; }
        public Nullable<decimal> week_52_positive_percentage { get; set; }
        public List<string> category_list { get; set; }
        //public Nullable<int> positive_count { get; set; }
        //public Nullable<int> negative_count { get; set; }
        //public Nullable<int> success_count { get; set; }
        //public Nullable<int> fail_count { get; set; }
        public Nullable<decimal> first_price { get; set; }
        public Nullable<decimal> last_price { get; set; }
        public Nullable<decimal> profit { get; set; }
        //public Nullable<int> negative { get; set; }
        //public Nullable<int> positive { get; set; }
        //public Nullable<int> negative_percentage { get; set; }
        //public Nullable<int> positive_percentage { get; set; }

        public Nullable<decimal> total_first_price { get; set; }
        public Nullable<decimal> total_last_price { get; set; }
        public Nullable<decimal> total_profit { get; set; }

        //public Nullable<decimal> total_high_price { get; set; }
        //public Nullable<decimal> total_low_price { get; set; }
        //public Nullable<decimal> profit_high_price { get; set; }
        //public Nullable<decimal> profit_low_price { get; set; }

        //public Nullable<decimal> profit_high_percentage { get; set; }
        //public Nullable<decimal> profit_low_percentage { get; set; }
        public Nullable<DateTime> trade_date { get; set; }
        public Nullable<DateTime> yesterday_date { get; set; }
        public Nullable<decimal> yesterday_percentage { get; set; }
        //public Nullable<decimal> total_rsi { get; set; }
        //public Nullable<decimal> profit_rsi { get; set; }

        //public Nullable<decimal> trigger_first_price { get; set; }
        //public Nullable<decimal> trigger_last_price { get; set; }
        //public Nullable<decimal> trigger_profit { get; set; }
        public Nullable<bool> is_holding { get; set; }

        public Nullable<decimal> percentage_2007 { get; set; }
        public Nullable<decimal> percentage_2008 { get; set; }
        public Nullable<decimal> percentage_2009 { get; set; }
        public Nullable<decimal> percentage_2010 { get; set; }
        public Nullable<decimal> percentage_2011 { get; set; }
        public Nullable<decimal> percentage_2012 { get; set; }
        public Nullable<decimal> percentage_2013 { get; set; }
        public Nullable<decimal> percentage_2014 { get; set; }
        public Nullable<decimal> percentage_2015 { get; set; }
        public Nullable<decimal> percentage_2016 { get; set; }
        public Nullable<decimal> percentage_2017 { get; set; }
        public Nullable<decimal> percentage_2018 { get; set; }
    }

    public class TRA_COMPANY_SEARCH : TRA_COMPANY
    {
        public string symbols { get; set; }
        public string categories { get; set; }
        public decimal? from_price { get; set; }
        public decimal? to_price { get; set; }
        public decimal? from_rsi { get; set; }
        public decimal? to_rsi { get; set; }
        public decimal? from_profit { get; set; }
        public decimal? to_profit { get; set; }

        public decimal? from_prev_rsi { get; set; }
        public decimal? to_prev_rsi { get; set; }

        public string mf_ids { get; set; }

        public Nullable<bool> is_book_mark_category { get; set; }
        public Nullable<bool> is_sell_to_buy { get; set; }
        public Nullable<bool> is_buy_to_sell { get; set; }

        public string ltp_from_percentage { get; set; }
        public string ltp_to_percentage { get; set; }
        public Nullable<bool> is_all_category { get; set; }

        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }

        public decimal? min_profit { get; set; }
        public int? max_negative_count { get; set; }

        public DateTime? total_start_date { get; set; }
        public DateTime? total_end_date { get; set; }

        public DateTime? trigger_start_date { get; set; }
        public DateTime? trigger_end_date { get; set; }

        public decimal? total_from_profit { get; set; }
        public decimal? total_to_profit { get; set; }
        public string ignore_symbols { get; set; }
        public bool? is_current_stock { get; set; }
        public decimal? total_amount { get; set; }

        public decimal? trigger_from_profit { get; set; }
        public decimal? trigger_to_profit { get; set; }
        public decimal? monthly_investment { get; set; }

        public string where_condition { get; set; }
    }

    public class TRA_CATEGORY_GROUP
    {
        public string category_name { get; set; }
        public decimal? total_investment { get; set; }
        public decimal? total_current { get; set; }
        public decimal? total_high { get; set; }
        public decimal? total_low { get; set; }

        public decimal total_profit {
            get {
                return DataTypeHelper.SafeDivision(((total_current ?? 0) - (total_investment ?? 0)), (total_investment ?? 0)) * 100;
            }
        }
        public decimal total_high_profit {
            get {
                return DataTypeHelper.SafeDivision(((total_high ?? 0) - (total_investment ?? 0)), (total_investment ?? 0)) * 100;
            }
        }
        public decimal total_low_profit {
            get {
                return DataTypeHelper.SafeDivision(((total_low ?? 0) - (total_investment ?? 0)), (total_investment ?? 0)) * 100;
            }
        }
        public List<TRA_COMPANY> companies { get; set; }
    }
} 

