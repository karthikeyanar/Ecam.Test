using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Models {
    public class ATR {

        public ATR() {
        }
        private int period = 14;

        public void Calculate(Price[] candles) {
            decimal sum = 0;
            for(int i = 0;i < candles.Length;i++) {
                candles[i].hl = (candles[i].high_price ?? 0) - (candles[i].low_price ?? 0);
                if(i > 0) {
                    candles[i].hcp = Math.Abs((candles[i].high_price ?? 0) - (candles[i - 1].close_price ?? 0));
                    candles[i].lcp = Math.Abs((candles[i].low_price ?? 0) - (candles[i - 1].close_price ?? 0));
                }
                var max = candles[i].hl > candles[i].hcp ? candles[i].hl : candles[i].hcp;
                candles[i].tr = max > candles[i].lcp ? max : candles[i].lcp;
                if(i >= (period-1)) {
                    sum = 0;
                    for(var j = i;j > i - period;j--) {
                        sum += candles[j].tr;
                    }
                    candles[i].atr = sum / period;
                }
            }
        }

    }

    public class ATRDetail {

    }
}

