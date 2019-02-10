using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Models {
    public class EMA {

        public EMA() {
        }

        public void Calculate(Price[] candles,int period) {
            decimal sum = 0;
            for(int i = 0;i < candles.Length;i++) {
                if(i <= (period - 1)) {
                    sum += (candles[i].close_price ?? 0);
                    decimal v = sum / period;
                    if(i == (period - 1)) {
                        if(period == 5) {
                            candles[i].ema_50 = v;
                        } else if(period == 20) {
                            candles[i].ema_200 = v;
                        }
                    }
                } else {
                    decimal avg = (decimal)0;
                    if(period == 5) {
                        avg = (candles[i - 1].ema_50 ?? 0);
                    } else if(period == 20) {
                        avg = (candles[i - 1].ema_200 ?? 0);
                    }
                    //= E9 * (2 / ($G$2 + 1)) +G8 * (1 - (2 / ($G$2 + 1)))
                    decimal coeff = decimal.Divide(2,(period + 1));
                    decimal ema = ((candles[i].close_price ?? 0) * coeff) + avg * (1-coeff);
                    if(period == 5) {
                        candles[i].ema_50 = ema;
                    } else if(period == 20) {
                        candles[i].ema_200 = ema;
                        candles[i].ema_cross = (candles[i].ema_50 ?? 0) - (candles[i].ema_200);
                    }
                }
            }
        }

    }

}

