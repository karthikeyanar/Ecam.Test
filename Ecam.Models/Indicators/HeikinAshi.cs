using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Models {
    //public class HeikinAshi {

    //    public HeikinAshi() {
    //        //this.heikin_ashi = new SupertrendDetail();
    //        //this.last_heikin_ashi = new SupertrendDetail();
    //    }

    //    //private SupertrendDetail heikin_ashi { get; set; }
    //    //private SupertrendDetail last_heikin_ashi { get; set; }
    //    //private decimal last_candle_close { get; set; }

    //    private int band_factor = 2;
    //    private int bought = 0;

    //    public void Calculate(Price[] candles) {
    //        for(int i = 0;i < candles.Length;i++) {
    //            //if(candles[i].trade_date.ToString("MM-dd-yyyy")== "01-31-2017") {
    //            //    string s = string.Empty;
    //            //}
    //            if(i >= 20) {
    //                candles[i].upper_band_basic = (((candles[i].heikin_ashi_high ?? 0) + (candles[i].heikin_ashi_low ?? 0)) / 2) + (this.band_factor * candles[i].atr);
    //                candles[i].lower_band_basic = (((candles[i].heikin_ashi_high ?? 0) + (candles[i].heikin_ashi_low ?? 0)) / 2) - (this.band_factor * candles[i].atr);

    //                if(candles[i].upper_band_basic < candles[i-1].upper_band || (candles[i-1].heikin_ashi_close ?? 0) > candles[i-1].upper_band)
    //                    candles[i].upper_band = candles[i].upper_band_basic;
    //                else
    //                    candles[i].upper_band = candles[i-1].upper_band;

    //                if(candles[i].lower_band_basic > candles[i-1].lower_band || (candles[i-1].heikin_ashi_close ?? 0) < candles[i-1].lower_band)
    //                    candles[i].lower_band = candles[i].lower_band_basic;
    //                else
    //                    candles[i].lower_band = candles[i-1].lower_band;

    //                if(candles[i-1].heikin_ashi == candles[i-1].upper_band && (candles[i].heikin_ashi_close ?? 0) <= candles[i].upper_band)
    //                    candles[i].heikin_ashi = candles[i].upper_band;
    //                else if(candles[i-1].heikin_ashi == candles[i-1].upper_band && (candles[i].heikin_ashi_close ?? 0) >= candles[i].upper_band)
    //                    candles[i].heikin_ashi = candles[i].lower_band;
    //                else if(candles[i-1].heikin_ashi == candles[i-1].lower_band && (candles[i].heikin_ashi_close ?? 0) >= candles[i].lower_band)
    //                    candles[i].heikin_ashi = candles[i].lower_band;
    //                else if(candles[i-1].heikin_ashi == candles[i-1].lower_band && (candles[i].heikin_ashi_close ?? 0) <= candles[i].lower_band)
    //                    candles[i].heikin_ashi = candles[i].upper_band;
    //                else
    //                    candles[i].heikin_ashi = 0;

    //                if((candles[i].heikin_ashi_close ?? 0) > candles[i].heikin_ashi && this.bought == 0) {
    //                    //this.advice("long");
    //                    this.bought = 1;
    //                    candles[i].heikin_ashi_signal = "B";
    //                    //log.debug("Buy at: ",(candles[i].heikin_ashi_close ?? 0));
    //                    Console.WriteLine("symbol=" + candles[i].symbol + ",date=" + candles[i].trade_date.ToString("MM/dd/yyyy") + ",buy at:" + (candles[i].heikin_ashi_close ?? 0));
    //                }

    //                if((candles[i].heikin_ashi_close ?? 0) < candles[i].heikin_ashi && this.bought == 1) {
    //                    //this.advice("short")
    //                    this.bought = 0;
    //                    candles[i].heikin_ashi_signal = "S";
    //                    //log.debug("Sell at: ",(candles[i].heikin_ashi_close ?? 0));
    //                    Console.WriteLine("symbol=" + candles[i].symbol + ",date=" + candles[i].trade_date.ToString("MM/dd/yyyy") + ",sell at:" + (candles[i].heikin_ashi_close ?? 0));
    //                }
                     
    //            }
    //        }
    //    }
         
    //}
}

