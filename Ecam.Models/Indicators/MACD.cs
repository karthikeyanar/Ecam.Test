using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Models {
    //public class MACD {

    //    public MACD() {
    //    }

    //    public void Calculate(Price[] candles) {
    //        decimal sum = 0;
    //        int period = 0;
    //        for(int i = 0;i < candles.Length;i++) {
    //            period = 12;
    //            sum = 0;
    //            if(i >= (period - 1)) {
    //                sum = 0;
    //                for(var j = i;j > i - period;j--) {
    //                    sum += (candles[j].close_price ?? 0);
    //                }
    //                candles[i].ema_12 = sum / period;
    //            }

    //            period = 26;
    //            sum = 0;
    //            if(i >= (period - 1)) {
    //                sum = 0;
    //                for(var j = i;j > i - period;j--) {
    //                    sum += (candles[j].close_price ?? 0);
    //                }
    //                candles[i].ema_26 = sum / period;
    //            }

    //            if(candles[i].trade_date.ToString("MM-dd-yyyy") == "02-17-2017") {
    //                string s = string.Empty;
    //            }

    //            if(i >= 25) {
    //                candles[i].macd = candles[i].ema_12 - candles[i].ema_26;
    //                if(i >= 33) {
    //                    period = 9;
    //                    sum = 0;
    //                    if(i >= (period - 1)) {
    //                        sum = 0;
    //                        for(var j = i;j > i - period;j--) {
    //                            sum += (candles[j].macd ?? 0);
    //                        }
    //                        candles[i].m_singal = sum / period;
    //                    }
    //                    candles[i].macd_histogram = (candles[i].macd ?? 0) - candles[i].m_singal;
    //                }
    //                if((candles[i].macd_histogram ?? 0) <= (decimal)-0.025) {
    //                    candles[i].macd_signal = "S";
    //                } else if((candles[i].macd_histogram ?? 0) > (decimal)0.025) {
    //                    candles[i].macd_signal = "B";
    //                }
    //            }
    //        }
    //    }

    //}

}

