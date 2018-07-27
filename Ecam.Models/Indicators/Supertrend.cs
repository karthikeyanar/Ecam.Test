using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Models {
    public class Supertrend {

        public Supertrend() {
            //this.super_trend = new SupertrendDetail();
            //this.last_super_trend = new SupertrendDetail();
        }

        //private SupertrendDetail super_trend { get; set; }
        //private SupertrendDetail last_super_trend { get; set; }
        //private decimal last_candle_close { get; set; }

        private int band_factor = 2;
        private int bought = 0;

        public void Calculate(Price[] candles) {
            for(int i = 0;i < candles.Length;i++) {
                //if(candles[i].trade_date.ToString("MM-dd-yyyy")== "01-31-2017") {
                //    string s = string.Empty;
                //}
                if(i >= 20) {
                    candles[i].upper_band_basic = (((candles[i].high_price ?? 0) + (candles[i].low_price ?? 0)) / 2) + (this.band_factor * candles[i].atr);
                    candles[i].lower_band_basic = (((candles[i].high_price ?? 0) + (candles[i].low_price ?? 0)) / 2) - (this.band_factor * candles[i].atr);

                    if(candles[i].upper_band_basic < candles[i-1].upper_band || (candles[i-1].close_price ?? 0) > candles[i-1].upper_band)
                        candles[i].upper_band = candles[i].upper_band_basic;
                    else
                        candles[i].upper_band = candles[i-1].upper_band;

                    if(candles[i].lower_band_basic > candles[i-1].lower_band || (candles[i-1].close_price ?? 0) < candles[i-1].lower_band)
                        candles[i].lower_band = candles[i].lower_band_basic;
                    else
                        candles[i].lower_band = candles[i-1].lower_band;

                    if(candles[i-1].super_trend == candles[i-1].upper_band && (candles[i].close_price ?? 0) <= candles[i].upper_band)
                        candles[i].super_trend = candles[i].upper_band;
                    else if(candles[i-1].super_trend == candles[i-1].upper_band && (candles[i].close_price ?? 0) >= candles[i].upper_band)
                        candles[i].super_trend = candles[i].lower_band;
                    else if(candles[i-1].super_trend == candles[i-1].lower_band && (candles[i].close_price ?? 0) >= candles[i].lower_band)
                        candles[i].super_trend = candles[i].lower_band;
                    else if(candles[i-1].super_trend == candles[i-1].lower_band && (candles[i].close_price ?? 0) <= candles[i].lower_band)
                        candles[i].super_trend = candles[i].upper_band;
                    else
                        candles[i].super_trend = 0;

                    if((candles[i].close_price ?? 0) > candles[i].super_trend && this.bought == 0) {
                        //this.advice("long");
                        this.bought = 1;
                        candles[i].super_trend_signal = "B";
                        //log.debug("Buy at: ",(candles[i].close_price ?? 0));
                        Console.WriteLine("symbol=" + candles[i].symbol + ",date=" + candles[i].trade_date.ToString("MM/dd/yyyy") + ",buy at:" + (candles[i].close_price ?? 0));
                    }

                    if((candles[i].close_price ?? 0) < candles[i].super_trend && this.bought == 1) {
                        //this.advice("short")
                        this.bought = 0;
                        candles[i].super_trend_signal = "S";
                        //log.debug("Sell at: ",(candles[i].close_price ?? 0));
                        Console.WriteLine("symbol=" + candles[i].symbol + ",date=" + candles[i].trade_date.ToString("MM/dd/yyyy") + ",sell at:" + (candles[i].close_price ?? 0));
                    }
                    
                    //candles[i-1].close_price ?? 0) = (candles[i].close_price ?? 0);
                    //candles[i-1].upper_band_basic = candles[i].upper_band_basic;
                    //candles[i-1].lower_band_basic = candles[i].lower_band_basic;
                    //candles[i-1].upper_band = candles[i].upper_band;
                    //candles[i-1].lower_band = candles[i].lower_band;
                    //candles[i-1].super_trend = candles[i].super_trend;
                    //candles[i].super_trend = candles[i].super_trend;
                }
            }
        }

        //private void Check(Price candle) {
        //    this.super_trend.upper_band_basic = (((candle.high_price ?? 0) + (candle.low_price ?? 0)) / 2) + (this.band_factor * candle.atr);
        //    this.super_trend.lower_band_basic = (((candle.high_price ?? 0) + (candle.low_price ?? 0)) / 2) - (this.band_factor * candle.atr);

        //    if(this.super_trend.upper_band_basic < this.last_super_trend.upper_band || this.last_candle_close > this.last_super_trend.upper_band)
        //        this.super_trend.upper_band = this.super_trend.upper_band_basic;
        //    else
        //        this.super_trend.upper_band = this.last_super_trend.upper_band;

        //    if(this.super_trend.lower_band_basic > this.last_super_trend.lower_band || this.last_candle_close < this.last_super_trend.lower_band)
        //        this.super_trend.lower_band = this.super_trend.lower_band_basic;
        //    else
        //        this.super_trend.lower_band = this.last_super_trend.lower_band;

        //    if(this.last_super_trend.super_trend == this.last_super_trend.upper_band && (candle.close_price ?? 0) <= this.super_trend.upper_band)
        //        this.super_trend.super_trend = this.super_trend.upper_band;
        //    else if(this.last_super_trend.super_trend == this.last_super_trend.upper_band && (candle.close_price ?? 0) >= this.super_trend.upper_band)
        //        this.super_trend.super_trend = this.super_trend.lower_band;
        //    else if(this.last_super_trend.super_trend == this.last_super_trend.lower_band && (candle.close_price ?? 0) >= this.super_trend.lower_band)
        //        this.super_trend.super_trend = this.super_trend.lower_band;
        //    else if(this.last_super_trend.super_trend == this.last_super_trend.lower_band && (candle.close_price ?? 0) <= this.super_trend.lower_band)
        //        this.super_trend.super_trend = this.super_trend.upper_band;
        //    else
        //        this.super_trend.super_trend = 0;

        //    if((candle.close_price ?? 0) > this.super_trend.super_trend && this.bought == 0) {
        //        //this.advice("long");
        //        this.bought = 1;
        //        candle.super_trend_signal = true;
        //        //log.debug("Buy at: ",(candle.close_price ?? 0));
        //        Console.WriteLine("symbol=" + candle.symbol + ",buy at:" + (candle.close_price ?? 0));
        //    }

        //    if((candle.close_price ?? 0) < this.super_trend.super_trend && this.bought == 1) {
        //        //this.advice("short")
        //        this.bought = 0;
        //        //log.debug("Sell at: ",(candle.close_price ?? 0));
        //        Console.WriteLine("symbol=" + candle.symbol + ",sell at:" + (candle.close_price ?? 0));
        //    }

        //    this.last_candle_close = (candle.close_price ?? 0);
        //    this.last_super_trend.upper_band_basic = this.super_trend.upper_band_basic;
        //    this.last_super_trend.lower_band_basic = this.super_trend.lower_band_basic;
        //    this.last_super_trend.upper_band = this.super_trend.upper_band;
        //    this.last_super_trend.lower_band = this.super_trend.lower_band;
        //    this.last_super_trend.super_trend = this.super_trend.super_trend;

        //    candle.super_trend = this.super_trend.super_trend;
        //}
    }

    public class SupertrendDetail {
        public decimal upper_band_basic { get; set; }
        public decimal lower_band_basic { get; set; }
        public decimal upper_band { get; set; }
        public decimal lower_band { get; set; }
        public decimal super_trend { get; set; }
    }
}

