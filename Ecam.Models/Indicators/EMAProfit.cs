using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Models {
    public class EMAProfit {

        public EMAProfit() {
        }

        public void Calculate(Price[] candles) {
            List<Price> previous = null;
            Price last = null;
            int cnt = 0;
            for(int i = 0;i < candles.Count();i++) {
                if(candles[i].trade_date.ToString("MM-dd-yyyy") == "07-10-2017") {
                    string s = string.Empty;
                }
                if((candles[i].ema_cross ?? 0) < 0) {
                    cnt = 0;
                    previous = new List<Price>();
                    previous.Add(candles[i]);
                    for(var j = (i - 1);j >= 0;j--) {
                        if(((candles[j].ema_cross ?? 0) < 0)) {
                            cnt += 1;
                            previous.Add(candles[j]);
                        } else {
                            break;
                        }
                    }
                    if(last != null) {
                        if(last.trade_date.ToString("MM/dd/yyyy") == "09-30-2016") {
                            string s = string.Empty;
                        }
                        last.ema_min_profit = ((from q in candles
                                                where q.trade_date > last.trade_date
                                                && q.trade_date < candles[i].trade_date
                                                select q).Count() > 0 ? (from q in candles
                                                                         where q.trade_date > last.trade_date
                                                                         && q.trade_date < candles[i].trade_date
                                                                         select (q.ema_profit ?? 0)).Min() : 0);
                        last.ema_max_profit = ((from q in candles
                                                where q.trade_date > last.trade_date
                                                && q.trade_date < candles[i].trade_date
                                                select q).Count() > 0 ? (from q in candles
                                                                         where q.trade_date > last.trade_date
                                                                         && q.trade_date < candles[i].trade_date
                                                                         select (q.ema_profit ?? 0)).Max() : 0);
                    }
                    if(cnt > 0) {
                        last = candles[i];
                        last.ema_cnt = cnt;
                        var previousFirst = (from q in previous
                                             orderby q.ema_cross ascending
                                             select q).FirstOrDefault();
                        for(int j = 0;j < previous.Count;j++) {
                            if(previous[j].trade_date < previousFirst.trade_date) {
                                previous[j].ema_min_cross = null;
                                previous[j].ema_increase = null;
                                previous[j].ema_increase_profit = null;
                            }
                        }
                        if(previousFirst != null) {
                            //TimeSpan ts = last.trade_date.Date - previousFirst.trade_date.Date;
                            int increaseDays = 0;
                            for(int j = 0;j < previous.Count;j++) {
                                if(previousFirst.trade_date.ToString("MM-dd-yyyy") == "04/20/2017") {
                                    string s = string.Empty;
                                }
                                if(previous[j].trade_date > previousFirst.trade_date) {
                                    increaseDays += 1;
                                }
                            }
                            last.ema_min_cross = previousFirst.ema_cross;
                            last.ema_increase_profit = (((last.close_price ?? 0) - (previousFirst.close_price ?? 0)) / (previousFirst.close_price ?? 0)) * 100;
                            if((last.ema_increase_profit ?? 0) > 0) {
                                last.ema_increase = increaseDays;
                            } else {
                                last.ema_increase = null;
                            }
                        }
                    }
                } else {
                    if(last != null && (candles[i].ema_cross ?? 0) > 0) {
                        last.ema_signal = "B";
                        var lastFirst = (from q in candles
                                         where q.trade_date > last.trade_date
                                         orderby q.trade_date ascending
                                         select q).FirstOrDefault();
                        if(lastFirst != null) {
                            candles[i].ema_profit = (((candles[i].close_price ?? 0) - (lastFirst.close_price ?? 0)) / (lastFirst.close_price ?? 0)) * 100;
                        }
                        last.ema_profit = candles[i].ema_profit;
                    } else {
                        last = null;
                        cnt = 0;
                    }
                }
            }
        }

    }

}

