using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Models {
    [NotMapped]
    public class Price:tra_market {
        // ATR
        public decimal hl { get; set; }
        public decimal hcp { get; set; }
        public decimal lcp { get; set; }
        public decimal tr { get; set; }
        public decimal atr { get; set; }

        // Supertrend
        public decimal upper_band_basic { get; set; }
        public decimal lower_band_basic { get; set; }
        public decimal upper_band { get; set; }
        public decimal lower_band { get; set; }
        public decimal super_trend { get; set; }
        public DateTime? sp_sell_date { get; set; }
        
        // MACD
        public decimal ema_12 { get; set; }
        public decimal ema_26 { get; set; }
        public decimal m_singal { get; set; }

        // EMA

        //HeikinAshi
        public decimal heikin_ashi { get; set; }
        public decimal? heikin_ashi_open {
            get {
                return ((this.open_price ?? 0) + (this.low_price ?? 0)) / 2;
            }
        }
        public decimal? heikin_ashi_close {
            get {
                return ((this.open_price ?? 0) + (this.high_price ?? 0) + (this.low_price ?? 0) + (this.close_price ?? 0)) / 4;
            }
        }
        public decimal? heikin_ashi_high {
            get {
                return new[] { (this.high_price ?? 0),(this.heikin_ashi_open ?? 0),(this.heikin_ashi_close ?? 0) }.Max();
            }
        }
        public decimal? heikin_ashi_low {
            get {
                return new[] { (this.low_price ?? 0),(this.heikin_ashi_open ?? 0),(this.heikin_ashi_close ?? 0) }.Max();
            }
        }
    }
}
