using Ecam.Framework;
using Ecam.Framework.ExcelHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Contracts
{
    public class TRA_MARKET_INTRA_DAY
    {
        public string symbol { get; set; }
        public System.DateTime trade_date { get; set; }
        public decimal ltp_price { get; set; }
        public decimal? rsi { get; set; }
        public decimal? prev_rsi { get; set; }
        public decimal? diff_rsi {
            get {
                return (this.rsi ?? 0) - (this.prev_rsi ?? 0);
            }
        }

        public decimal open_price { get; set; }
        public decimal ltp_percentage { get; set; }
        public string time { get; set; }
        public int company_id { get; set; }
    }

    public class TRA_MARKET_RSI
    {
        public string symbol { get; set; }
        public System.DateTime trade_date { get; set; }
        public decimal? close_price { get; set; }
        public decimal? rsi { get; set; }
        public decimal? prev_rsi { get; set; }
        public decimal? ltp_percentage { get; set; }
        public decimal? prev_percentage { get; set; }
        public decimal? open_percentage { get; set; }
        public decimal? high_percentage { get; set; }
        public int company_id { get; set; }
    }

    public class TRA_MARKET_AVG
    {
        public string symbol { get; set; }
        public string avg_type { get; set; }
        public System.DateTime avg_date { get; set; }
        public decimal percentage { get; set; }

        public string desc {
            get {
                string result = string.Empty;
                if (this.avg_type == "M")
                {
                    result = this.avg_date.ToString("MMM yyyy");
                }
                else
                {
                    result = this.avg_date.ToString("dd/MMM/yyyy"); // "W" + GetWeekNumberOfMonth(this.avg_date) + " " + this.avg_date.ToString("MMM yyyy");
                }
                return result;
            }
        }

        public static int GetWeekNumber(DateTime date)
        {
            return GetWeekNumber(date, CultureInfo.CurrentCulture);
        }

        public static int GetWeekNumber(DateTime date, CultureInfo culture)
        {
            return culture.Calendar.GetWeekOfYear(date,
                culture.DateTimeFormat.CalendarWeekRule,
                culture.DateTimeFormat.FirstDayOfWeek);
        }

        public static int GetWeekNumberOfMonth(DateTime date)
        {
            return GetWeekNumberOfMonth(date, CultureInfo.CurrentCulture);
        }

        public static int GetWeekNumberOfMonth(DateTime date, CultureInfo culture)
        {
            return GetWeekNumber(date, culture)
                 - GetWeekNumber(new DateTime(date.Year, date.Month, 1), culture)
                 + 1; // Or skip +1 if you want the first week to be 0.
        }
    }
}

