using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Framework {
    public static class FormatHelper {

        public static string NumberFormat(decimal? value,int? precision = 2,bool? isCheckZero = false,bool? isInteger = false) {
            if(isCheckZero == true) {
                if((value ?? 0) == 0)
                    return string.Empty;
            }
            if(value == null) { return "0"; }
            if((value ?? 0) == 0) { return "0"; }
            string format = "{0:C" + precision + "}";
            string result = String.Format(format,(value ?? 0)).Replace("$","").Replace("₹","").Replace(".0000","");
            if(isInteger == true) {
                result = result.Replace(".00","").Replace(".000","");
            }
            return result;
        }

        public static string CurrencyFormat(decimal? value,int? precision = 2,bool? isCheckZero = false) {
            if(isCheckZero == true) {
                if((value ?? 0) == 0)
                    return string.Empty;
            }
            if(value == null) { return "$0"; }
            if((value ?? 0) == 0) { return "$0"; }
            string format = "{0:C" + precision + "}";
            return String.Format(format,(value ?? 0)).Replace(".0000","");
        }

        public static string PercentageFormat(decimal? value,bool? isCheckZero = false) {
            if(isCheckZero == true) {
                if((value ?? 0) == 0)
                    return string.Empty;
            }
            if(value == null) { return "0%"; }
            if((value ?? 0) == 0) { return "0%"; }
            return NumberFormat(value,2) + "%";
        }

        public static string PercentageFormat(int? value,bool? isCheckZero = false) {
            if(isCheckZero == true) {
                if((value ?? 0) == 0)
                    return string.Empty;
            }
            if(value == null) { return "0%"; }
            if((value ?? 0) == 0) { return "0%"; }
            return String.Format("{0:P0}",(value ?? 0)).Replace(".00","");
        }

        public static string StringFormat(decimal? value,string format) {
            return ((value ?? 0) == 0 ? string.Empty : String.Format(format,(value ?? 0)).Replace("$",""));
        }

        public static string AWBNoFormat(string awbno) {
            return (string.IsNullOrEmpty(awbno) == false ? awbno.LeftOf(3) + "-" + awbno.LeftOf(3,4) + " " + awbno.LeftOf(7,4) : "");
        }

        public static string AWBNoFormat2(string awbno) {
            return (string.IsNullOrEmpty(awbno) == false ? awbno.LeftOf(3) + " " + awbno.LeftOf(3,4) + "" + awbno.LeftOf(7,4) : "");
        }

        public static string LeftOf(this string value,int maxLength) {
            if(string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0,maxLength)
                   );
        }

        public static string LeftOf(this string value,int from,int maxLength) {
            if(string.IsNullOrEmpty(value)) return string.Empty;
            if((value.Length - from) < maxLength)
                maxLength = value.Length - from;
            if(maxLength <= 0) return string.Empty;
            return value.Substring(from,maxLength);
        }

        public static string ToFriendCurrencyFormat(decimal? value) {
            decimal outputValue = (value ?? 0);
            if(outputValue >= 1000000)
                return Math.Round(decimal.Divide(outputValue,1000000),2).ToString("0.##") + "M";
            else if(outputValue >= 1000)
                return Math.Round(decimal.Divide(outputValue,1000),2).ToString("0.##") + "K";
            else
                return Math.Round(outputValue,2).ToString("0.##") + "K";
        }

        public static string ToAWBNoFormat(string awb_no) {
            return (awb_no.Length > 3 ? awb_no.Substring(0,3) + " " + awb_no.Substring(3,awb_no.Length - 3) : awb_no);
        }

        /// <summary>
        /// Number multiplied by 100 and displayed with a percent symbol, upto 1 significant digit. Eg: MercentageFormat(.56789) will return
        /// 56.7%
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Number multiplied by 100 and displayed with a percent symbol.</returns>
        //public static string PercentageFormat(decimal? value) {
        //    return (value ?? 0).ToString("0.#") + "%";
        //}

        //public static string PercentageFormat(int? value) {
        //    return (value ?? 0).ToString("0.#") + "%";
        //}
    }
}
