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
        public Nullable<bool> is_book_mark { get; set; }
        public Nullable<decimal> day_5 { get; set; }
        public Nullable<decimal> day_10 { get; set; }
        public Nullable<decimal> day_15 { get; set; }
        public Nullable<decimal> day_20 { get; set; }
        public Nullable<decimal> day_25 { get; set; }
        public Nullable<decimal> day_30 { get; set; }
        public Nullable<decimal> day_35 { get; set; }
        public Nullable<decimal> day_60 { get; set; }
        public Nullable<bool> is_nifty_50 { get; set; }
        public Nullable<bool> is_nifty_100 { get; set; }
        public Nullable<bool> is_nifty_200 { get; set; }
        public Nullable<decimal> day_40 { get; set; }
        public Nullable<decimal> day_45 { get; set; }
        public Nullable<decimal> day_50 { get; set; }
        public Nullable<decimal> day_55 { get; set; }
        public Nullable<decimal> day_65 { get; set; }
        public Nullable<decimal> day_70 { get; set; }
        public Nullable<decimal> day_75 { get; set; }
        public Nullable<decimal> day_80 { get; set; }
        public Nullable<decimal> day_85 { get; set; }
        public Nullable<decimal> day_90 { get; set; }
        public Nullable<int> mf_cnt { get; set; }
        public Nullable<decimal> mf_qty { get; set; }
        public Nullable<decimal> day_1 { get; set; }
        public Nullable<decimal> day_2 { get; set; }
        public Nullable<decimal> day_3 { get; set; }
        public Nullable<decimal> day_4 { get; set; }
        public Nullable<int> high_count { get; set; }
        public Nullable<int> low_count { get; set; }
        public Nullable<decimal> rsi { get; set; }
        public Nullable<decimal> prev_rsi { get; set; }

        public string category_name { get; set; }
        public Nullable<decimal> prev_percentage { get; set; }
        public Nullable<decimal> day_1_percentage { get; set; }
        public Nullable<decimal> day_2_percentage { get; set; }
        public Nullable<decimal> day_3_percentage { get; set; }
        public Nullable<decimal> day_4_percentage { get; set; }
        public Nullable<decimal> day_5_percentage { get; set; }
        public Nullable<decimal> day_10_percentage { get; set; }
        public Nullable<decimal> day_15_percentage { get; set; }
        public Nullable<decimal> day_20_percentage { get; set; }
        public Nullable<decimal> day_25_percentage { get; set; }
        public Nullable<decimal> day_30_percentage { get; set; }
        public Nullable<decimal> day_35_percentage { get; set; }
        public Nullable<decimal> day_40_percentage { get; set; }
        public Nullable<decimal> day_45_percentage { get; set; }
        public Nullable<decimal> day_50_percentage { get; set; }
        public Nullable<decimal> day_55_percentage { get; set; }
        public Nullable<decimal> day_60_percentage { get; set; }
        public Nullable<decimal> day_65_percentage { get; set; }
        public Nullable<decimal> day_70_percentage { get; set; }
        public Nullable<decimal> day_75_percentage { get; set; }
        public Nullable<decimal> day_80_percentage { get; set; }
        public Nullable<decimal> day_85_percentage { get; set; }
        public Nullable<decimal> day_90_percentage { get; set; }

        public Nullable<decimal> week_52_low_percentage { get; set; }
        public Nullable<decimal> week_52_percentage { get; set; }
        public Nullable<decimal> week_52_positive_percentage { get; set; }
        public List<string> category_list { get; set; }
        public Nullable<bool> is_all_time_high { get; set; }
        public Nullable<bool> is_all_time_low { get; set; }

        public Nullable<bool> is_all_time_high_15_days { get; set; }
        public Nullable<bool> is_all_time_low_15_days { get; set; }
        public Nullable<int> mf_cnt_2 { get; set; }
        public Nullable<decimal> mf_qty_2 { get; set; }

        public Nullable<decimal> diff { get; set; }
        public Nullable<decimal> ltp_percentage { get; set; }
        public Nullable<decimal> high_percentage { get; set; }
        public Nullable<decimal> low_percentage { get; set; }

        //public Nullable<decimal> profit_percentage { get; set; }
        //public Nullable<decimal> stoploss_percentage { get; set; }

        //public Nullable<decimal> available_amount { get; set; }
        //public Nullable<decimal> investment_amount {
        //    get {
        //        return (this.quantity ?? 0) * (this.ltp_price ?? 0);
        //    }
        //}
        //public Nullable<int> quantity {
        //    get {
        //        return (int)DataTypeHelper.SafeDivision((this.available_amount ?? 0), (this.ltp_price ?? 0));
        //    }
        //}

        //public Nullable<decimal> target_price {
        //    get {
        //        return ((this.ltp_price ?? 0) * (this.profit_percentage ?? 0)) / 100;
        //    }
        //}
        //public Nullable<decimal> stop_loss_price {
        //    get {
        //        return ((this.ltp_price ?? 0) * (this.stoploss_percentage ?? 0)) / 100;
        //    }
        //}

        //public Nullable<decimal> bts_sell_target {
        //    get {
        //        return ((this.ltp_price ?? 0) + (this.target_price ?? 0));
        //    }
        //}
        //public Nullable<decimal> bts_stop_loss_target {
        //    get {
        //        return ((this.ltp_price ?? 0) - (this.stop_loss_price ?? 0));
        //    }
        //}
         
        //public Nullable<decimal> stb_sell_target {
        //    get {
        //        return ((this.ltp_price ?? 0) - (this.target_price ?? 0));
        //    }
        //}
        //public Nullable<decimal> stb_stop_loss_target {
        //    get {
        //        return ((this.ltp_price ?? 0) + (this.stop_loss_price ?? 0));
        //    }
        //}

    }

    public class TRA_COMPANY_SEARCH : TRA_COMPANY
    {
        public string symbols { get; set; }
        public string categories { get; set; }
        public decimal? from_price { get; set; }
        public decimal? to_price { get; set; }
        public string mf_ids { get; set; }
        public Nullable<bool> is_mf { get; set; }
        public Nullable<bool> is_all_time_high_5_days { get; set; }
        public Nullable<bool> is_all_time_low_5_days { get; set; }
        public Nullable<bool> is_all_time_high_2_days { get; set; }
        public Nullable<bool> is_all_time_low_2_days { get; set; }
        public Nullable<bool> is_high_yesterday { get; set; }
        public Nullable<bool> is_low_yesterday { get; set; }

        public Nullable<bool> is_sell_to_buy { get; set; }
        public Nullable<bool> is_buy_to_sell { get; set; }

        public string ltp_from_percentage { get; set; }
        public string ltp_to_percentage { get; set; }
    }
}

