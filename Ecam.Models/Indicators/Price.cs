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
        public string super_trend_signal { get; set; }
    }
}
