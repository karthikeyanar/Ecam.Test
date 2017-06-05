using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace Ecam.Framework {

    public class PeriodDates {
        public string name { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
    }

    public static class DataTypeHelper {

        public static int GetQuarter(this DateTime dateTime) {
            if(dateTime.Month <= 3)
                return 1;

            if(dateTime.Month <= 6)
                return 2;

            if(dateTime.Month <= 9)
                return 3;

            return 4;
        }

        public static string ConvertBit(bool? isCondition) {
            return ((isCondition ?? false) == true ? "1" : "0");
        }

        public static decimal SafeDivision(decimal d1,decimal d2) {
            if(d2 == 0)
                //throw new System.DivideByZeroException();
                return 0;
            else
                return decimal.Divide(d1,d2);
        }

        public static double SafeDivision(double d1,double d2) {
            if(d2 == 0)
                //throw new System.DivideByZeroException();
                return 0;
            else
                return d1 / d2;
        }

        public static float SafeDivision(float d1,float d2) {
            if(d2 == 0)
                //throw new System.DivideByZeroException();
                return 0;
            else
                return d1 / d2;
        }

        private static string RemoveSymbols(string value) {
            if(string.IsNullOrEmpty(value) == false) {
                value = value.Replace("₹", "").Replace("&#8377;","").Replace("$","").Replace("%","").Replace(",","").Replace("(","-").Replace(")","");
            }
            return (value == null ? "" : value);
        }

        public static float ToFloat(string value) {
            value = RemoveSymbols(value);
            float returnValue;
            float.TryParse(value,out returnValue);
            return returnValue;
        }

        public static decimal ToDecimal(string value) {
            value = RemoveSymbols(value);
            decimal returnValue;
            decimal.TryParse(value,out returnValue);
            return returnValue;
        }

        public static Int32 ToInt32(object value) {
            if(value == null || value == DBNull.Value) return 0;
            string v = RemoveSymbols(value.ToString());
            if(v.Contains(".")) {
                decimal deValue = 0;
                decimal.TryParse(v,out deValue);
                return (Int32)deValue;
            } else {
                Int32 returnValue;
                Int32.TryParse(v,out returnValue);
                return returnValue;
            }
        }

        public static uint ToUInt(string value)
        {
            value = RemoveSymbols(value);
            if (value.Contains("."))
            {
                decimal deValue = 0;
                decimal.TryParse(value, out deValue);
                return (uint)deValue;
            }
            else
            {
                uint returnValue;
                uint.TryParse(value, out returnValue);
                return returnValue;
            }
        }

        public static Int32 ToInt32(string value) {
            value = RemoveSymbols(value);
            if(value.Contains(".")) {
                decimal deValue = 0;
                decimal.TryParse(value,out deValue);
                return (Int32)deValue;
            } else {
                Int32 returnValue;
                Int32.TryParse(value,out returnValue);
                return returnValue;
            }
        }

        public static Int64 ToInt64(string value) {
            value = RemoveSymbols(value);
            if(value.Contains(".")) {
                decimal deValue = 0;
                decimal.TryParse(value,out deValue);
                return (Int64)deValue;
            } else {
                Int64 returnValue;
                Int64.TryParse(value,out returnValue);
                return returnValue;
            }
        }

        public static Int16 ToInt16(string value) {
            value = RemoveSymbols(value);
            if(value.Contains(".")) {
                decimal deValue = 0;
                decimal.TryParse(value,out deValue);
                return (Int16)deValue;
            } else {
                Int16 returnValue;
                Int16.TryParse(value,out returnValue);
                return returnValue;
            }
        }

        public static DateTime ToDateTime(string value) {
            DateTime returnValue;
            DateTime.TryParse(value,out returnValue);
            return returnValue.Year <= 1900 ? new DateTime(1900,1,1) : returnValue;
        }

        public static DateTime ToDateTime(object value) {
            DateTime returnValue;
            DateTime.TryParse(Convert.ToString(value),out returnValue);
            return returnValue.Year <= 1900 ? new DateTime(1900,1,1) : returnValue;
        }

        public static string ToDateTimeFormat(string value,string format) {
            DateTime dt = ToDateTime(value);
            if(dt.Year <= 1900)
                return string.Empty;
            else
                return dt.ToString(format);
        }

        private static DateTime ToDateTime(string value,string format) {
            DateTime returnValue;
            DateTime.TryParseExact(value,format,CultureInfo.InvariantCulture,DateTimeStyles.None,out returnValue);
            return returnValue.Year <= 1900 ? new DateTime(1900,1,1) : returnValue;
        }
        
        //public static DateTime ToDateTimeAllFormat(string value) {
        //    DateTime returnValue = ToDateTime(value,"dd/MM/yyyy");
        //    if(returnValue.Year <= 1900) {
        //        returnValue = ToDateTime(value,"dd/MMM/yyyy");
        //    }
        //    if(returnValue.Year <= 1900) {
        //        returnValue = ToDateTime(value,"dd/M/yyyy");
        //    }
        //    if(returnValue.Year <= 1900) {
        //        returnValue = ToDateTime(value,"d/MM/yyyy");
        //    }
        //    if(returnValue.Year <= 1900) {
        //        returnValue = ToDateTime(value,"d/MMM/yyyy");
        //    }
        //    if(returnValue.Year <= 1900) {
        //        returnValue = ToDateTime(value,"d/M/yyyy");
        //    }
        //    return returnValue.Year <= 1900 ? new DateTime(1900,1,1) : returnValue;
        //}

        public static DateTime ToFromOADate(string value) {
            if (value.Contains("-") == true || value.Contains("/") == true)
            {
                return DataTypeHelper.ToDateTime(value);
            }
            else
            {
                DateTime returnValue;
                double dateValue = 0;
                double.TryParse(value, out dateValue);
                returnValue = DateTime.FromOADate(dateValue);
                return returnValue.Year <= 1900 ? new DateTime(1900, 1, 1) : returnValue;
            }
        }

        public static DateTime ToFromExcelDate(decimal d) {
            //double d = (double)0.28819444444444448;
            double db = (double)d;
            double x = (24 * 60) * db * 60;
            DateTime dt = DateTime.Now.Date;
            dt = dt.AddSeconds(x);
            return dt;
        }

        public static string ToFromExcelTimeValue(string timeValue) {
            string result = string.Empty;
            if(timeValue.Contains(":")) {
                result = timeValue.Trim();
            } else {
                decimal etdValue = DataTypeHelper.ToDecimal(timeValue);
                if(timeValue.Contains("E-2") == true) {
                    timeValue = timeValue.Replace("E-2","").Trim();
                    etdValue = DataTypeHelper.SafeDivision(DataTypeHelper.ToDecimal(timeValue),100);
                } else if(timeValue.Contains("E+2") == true) {
                    timeValue = timeValue.Replace("E+2","").Trim();
                    etdValue = DataTypeHelper.ToDecimal(timeValue) * 100;
                }
                DateTime minDate = Convert.ToDateTime("01/01/1900");
                DateTime etdDate = minDate;
                if(etdValue <= 1) {
                    etdDate = DataTypeHelper.ToFromExcelDate(etdValue);
                }
                if(etdDate.Year <= 1900) {
                    etdDate = DataTypeHelper.ToDateTime(DateTime.Now.Date.ToString("MM/dd/yyyy") + " " + timeValue);
                }
                if(etdDate.Year > 1900) {
                    result = etdDate.ToString("HH:mm");
                }
            }
            return result;
        }

		public static string Convert24HoursFormat(string value) {
			try {
				if(string.IsNullOrEmpty(value) == false) {
					value = value.Replace(" ","").Trim().ToUpper();
				}
				if(string.IsNullOrEmpty(value) == false) {
					if(value.Contains("AM") == true || value.Contains("PM") == true) {
						value = value.Replace(" ","").Replace("`","").Replace("'","").Replace("\"","").Replace(";","").Trim().ToUpper();
					}
					DateTime dt = Convert.ToDateTime(DateTime.Now.Date.ToString("MM/dd/yyyy") + " " + value);
					value = dt.ToString("HH:mm");
				}
                if(string.IsNullOrEmpty(value) == false) {
                    value = value.Replace(".",":");
                }
            } catch { value = string.Empty; }
			return value;
		}

        public static DateTime ExcelToDate(string value) {
            DateTime returnValue = DataTypeHelper.ToFromOADate(value);
            if(returnValue.Year <= 1900) {
                returnValue = ToDateTime(value,"dd/MMM/yyyy");
            }
            if(returnValue.Year <= 1900) {
                returnValue = ToDateTime(value,"d/MMM/yyyy");
            }
            if(returnValue.Year <= 1900) {
                returnValue = ToDateTime(value,"dd/MMM/yy");
            }
            if(returnValue.Year <= 1900) {
                returnValue = ToDateTime(value,"d/MMM/yy");
            }
            ///
            if(returnValue.Year <= 1900) {
                returnValue = ToDateTime(value,"dd-MMM-yyyy");
            }
            if(returnValue.Year <= 1900) {
                returnValue = ToDateTime(value,"d-MMM-yyyy");
            }
            if(returnValue.Year <= 1900) {
                returnValue = ToDateTime(value,"dd-MMM-yy");
            }
            if(returnValue.Year <= 1900) {
                returnValue = ToDateTime(value,"d-MMM-yy");
            }
            if(returnValue.Year <= 1900) {
                returnValue = ToDateTime(value,"dd/MM/yyyy");
            }
            if(returnValue.Year <= 1900) {
                returnValue = ToDateTime(value,"dd/MM/yy");
            }
            return returnValue.Year <= 1900 ? new DateTime(1900,1,1) : returnValue;
        }

        public static String ConvertString(object value) {
            if(value == null || value == DBNull.Value)
                return string.Empty;
            else
                return value.ToString();
        }

        public static string ConvertCode(object value) {
            string code = DataTypeHelper.ConvertString(value);
            if(string.IsNullOrEmpty(code) == false) {
                code = code.Replace(" ","").ToUpper().Trim().ToString();
            }
            return code;
        }

        public static bool ToBoolean(object value) {
            if(value == null)
                return false;
            value = Convert.ToString(value);
            if(value.ToString() == "true" || value.ToString() == "1")
                return true;
            else
                return false;
        }

        public static bool CheckBoolean(string value) {
            bool calcValue = false;
            if(string.IsNullOrEmpty(value) == false)
                value = value.ToLower().Trim();
            else
                value = string.Empty;

            if(value.ToString().Contains("true"))
                calcValue = true;
            if(value.ToString().Contains("yes"))
                calcValue = true;
            if(value.ToString().Contains("on"))
                calcValue = true;
            if(value.ToString() == "1")
                calcValue = true;

            return calcValue;
        }

        public static int GetQuarterNumber(DateTime dtNow) {
            int quarter = 0;
            if(dtNow.Month >= 1 && dtNow.Month <= 3) {
                quarter = 1;
            } else if(dtNow.Month >= 4 && dtNow.Month <= 6) {
                quarter = 2;
            } else if(dtNow.Month >= 7 && dtNow.Month <= 9) {
                quarter = 3;
            } else if(dtNow.Month >= 10 && dtNow.Month <= 12) {
                quarter = 4;
            }
            return quarter;
        }

        public static DateTime[] DatesOfQuarter(DateTime dtNow) {
            DateTime[] dtReturn = new DateTime[2];
            if(dtNow.Month >= 1 && dtNow.Month <= 3) {
                dtReturn[0] = DateTime.Parse("1/1/" + dtNow.Year.ToString());
                dtReturn[1] = DateTime.Parse("3/31/" + dtNow.Year.ToString());
            } else if(dtNow.Month >= 4 && dtNow.Month <= 6) {
                dtReturn[0] = DateTime.Parse("4/1/" + dtNow.Year.ToString());
                dtReturn[1] = DateTime.Parse("6/30/" + dtNow.Year.ToString());
            } else if(dtNow.Month >= 7 && dtNow.Month <= 9) {
                dtReturn[0] = DateTime.Parse("7/1/" + dtNow.Year.ToString());
                dtReturn[1] = DateTime.Parse("9/30/" + dtNow.Year.ToString());
            } else if(dtNow.Month >= 10 && dtNow.Month <= 12) {
                dtReturn[0] = DateTime.Parse("10/1/" + dtNow.Year.ToString());
                dtReturn[1] = DateTime.Parse("12/31/" + dtNow.Year.ToString());
            }
            return dtReturn;
        }

        public static string GenerateAlphaNumeric(int length) {
            Random random = new Random();
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(length);
            for(int i = 0;i < length;i++) {
                result.Append(characters[random.Next(characters.Length)]);
            }
            return result.ToString();
        }

        public static DateTime GetFirstDayOfMonth(DateTime dateTime) {
            return new DateTime(dateTime.Year,dateTime.Month,1);
        }

        public static DateTime GetLastDayOfMonth(DateTime dateTime) {
            DateTime firstDayOfTheMonth = new DateTime(dateTime.Year,dateTime.Month,1);
            return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }

        public static List<PeriodDates> GetPeriodDates(DateTime startDate,DateTime endDate,string type) {
            List<PeriodDates> periods = new List<PeriodDates>();
            List<DateTime> dates = new List<DateTime>();
            int i;
            switch(type) {
                case "F":
                    dates = GetFortNightDates(startDate,endDate);
                    for(i = 0;i < dates.Count;i++) {
                        startDate = dates[i];
                        if((i + 1) < dates.Count) {
                            endDate = dates[i + 1];
                        }
                        i += 1;
                        string name = string.Empty;
                        int fnNumber = (startDate.Day < 16 ? 1 : 2);
                        if(fnNumber == 1) {
                            name = "F1 " + startDate.ToString("MMM yyyy");
                        } else {
                            name = "F2 " + startDate.ToString("MMM yyyy");
                        }
                        periods.Add(new PeriodDates {
                            start_date = startDate,
                            end_date = endDate,
                            name = name
                        });
                    }
                    break;
                case "M":
                    dates = GetMonthDates(startDate,endDate);
                    for(i = 0;i < dates.Count;i++) {
                        startDate = dates[i];
                        if((i + 1) < dates.Count) {
                            endDate = dates[i + 1];
                        }
                        i += 1;
                        string name = startDate.ToString("MMM yyyy");
                        periods.Add(new PeriodDates {
                            start_date = startDate,
                            end_date = endDate,
                            name = name
                        });
                    }
                    break;
                case "Q":
                    dates = GetQuarterDates(startDate,endDate);
                    for(i = 0;i < dates.Count;i++) {
                        startDate = dates[i];
                        if((i + 1) < dates.Count) {
                            endDate = dates[i + 1];
                        }
                        i += 1;
                        string name = "Q" + GetQuarter(startDate) + " " + startDate.ToString("yyyy");
                        periods.Add(new PeriodDates {
                            start_date = startDate,
                            end_date = endDate,
                            name = name
                        });
                    }
                    break;
                case "Y":
                    dates = GetYearDates(startDate,endDate);
                    for(i = 0;i < dates.Count;i++) {
                        startDate = dates[i];
                        if((i + 1) < dates.Count) {
                            endDate = dates[i + 1];
                        }
                        i += 1;
                        string name = startDate.ToString("yyyy");
                        periods.Add(new PeriodDates {
                            start_date = startDate,
                            end_date = endDate,
                            name = name
                        });
                    }
                    break;
            }
            return periods;
        }

        public static List<DateTime> GetFortNightDates(DateTime startDate,DateTime endDate) {
            List<DateTime> dates = new List<DateTime>();
            var months = MonthDiff(startDate,endDate) + 1;
            int i;
            for(i = 0;i < months;i++) {
                DateTime calcDate = startDate;
                calcDate = GetFirstDayOfMonth(calcDate.AddMonths(i));
                DateTime dt = calcDate;
                if(dt <= endDate) {
                    dates.Add(dt);
                    dt = Convert.ToDateTime(string.Format("{0}/{1}/{2}",calcDate.Month,"15",calcDate.Year));
                    dates.Add(dt);
                }
                dt = Convert.ToDateTime(string.Format("{0}/{1}/{2}",calcDate.Month,"16",calcDate.Year));
                if(dt <= endDate) {
                    dates.Add(dt);
                    dt = GetLastDayOfMonth(calcDate);
                    dates.Add(dt);
                }
            }
            return dates;
        }

        public static List<DateTime> GetMonthDates(DateTime startDate,DateTime endDate) {
            List<DateTime> dates = new List<DateTime>();
            var months = MonthDiff(startDate,endDate) + 1;
            int i;
            for(i = 0;i < months;i++) {
                DateTime calcDate = startDate;
                calcDate = GetFirstDayOfMonth(calcDate.AddMonths(i));
                DateTime dt = calcDate;
                if(dt <= endDate) {
                    dates.Add(dt);
                    dt = GetLastDayOfMonth(calcDate);
                    dates.Add(dt);
                }
            }
            return dates;
        }

        public static List<DateTime> GetQuarterDates(DateTime startDate,DateTime endDate) {
            List<DateTime> dates = new List<DateTime>();
            var years = (endDate.Year - startDate.Year) + 1;
            int i;
            for(i = 0;i < years;i++) {
                DateTime calcDate = startDate;
                calcDate = startDate.AddYears(i);

                //Q1
                DateTime dt = Convert.ToDateTime(string.Format("{0}/{1}/{2}","01","01",calcDate.Year));
                if(dt <= endDate) {
                    dates.Add(dt);
                    dates.Add(Convert.ToDateTime(string.Format("{0}/{1}/{2}","03","31",calcDate.Year)));
                }

                //Q2
                dt = Convert.ToDateTime(string.Format("{0}/{1}/{2}","04","01",calcDate.Year));
                if(dt <= endDate) {
                    dates.Add(dt);
                    dates.Add(Convert.ToDateTime(string.Format("{0}/{1}/{2}","06","30",calcDate.Year)));
                }

                //Q3
                dt = Convert.ToDateTime(string.Format("{0}/{1}/{2}","07","01",calcDate.Year));
                if(dt <= endDate) {
                    dates.Add(dt);
                    dates.Add(Convert.ToDateTime(string.Format("{0}/{1}/{2}","09","30",calcDate.Year)));
                }

                //Q4
                dt = Convert.ToDateTime(string.Format("{0}/{1}/{2}","10","01",calcDate.Year));
                if(dt <= endDate) {
                    dates.Add(dt);
                    dates.Add(Convert.ToDateTime(string.Format("{0}/{1}/{2}","12","31",calcDate.Year)));
                }

            }
            return dates;
        }

        public static List<DateTime> GetYearDates(DateTime startDate,DateTime endDate) {
            List<DateTime> dates = new List<DateTime>();
            var years = (endDate.Year - startDate.Year) + 1;
            int i;
            for(i = 0;i < years;i++) {
                DateTime calcDate = startDate;
                calcDate = startDate.AddYears(i);

                DateTime dt = Convert.ToDateTime(string.Format("{0}/{1}/{2}","01","01",calcDate.Year));
                if(dt <= endDate) {
                    dates.Add(dt);
                    dates.Add(Convert.ToDateTime(string.Format("{0}/{1}/{2}","12","31",calcDate.Year)));
                }
            }
            return dates;
        }

        public static double MonthDiff(DateTime from,DateTime to) {
            /// |-------X----|---------------|---------------|--X-----------|
            ///         ^                                       ^
            ///       from                                     to

            //change the dates if to is before from
            if(to.Ticks < from.Ticks) {
                DateTime temp = from;
                from = to;
                to = temp;
            }

            /// Gets the day percentage of the months = 0...1
            ///
            /// 0            1                               0              1
            /// |-------X----|---------------|---------------|--X-----------|
            /// ^^^^^^^^^                                    ^^^^
            /// percFrom                                    percTo
            double percFrom = (double)from.Day / DateTime.DaysInMonth(from.Year,from.Month);
            double percTo = (double)to.Day / DateTime.DaysInMonth(to.Year,to.Month);

            /// get the amount of months between the two dates based on day one
            /// 
            /// |-------X----|---------------|---------------|--X-----------|
            /// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            ///                        months
            double months = (to.Year * 12 + to.Month) - (from.Year * 12 + from.Month);

            /// Return the right parts
            /// 
            /// |-------X----|---------------|---------------|--X-----------|            
            ///         ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            ///                      return
            return months - percFrom + percTo;
        }

        public static string NumbersToWords(decimal inputNumber) {
            int inputNo = Decimal.ToInt32(inputNumber);

            if(inputNo == 0)
                return "Zero";

            int[] numbers = new int[4];
            int first = 0;
            int u,h,t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if(inputNo < 0) {
                sb.Append("Minus ");
                inputNo = -inputNo;
            }

            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ",
            "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
            "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ",
            "Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ","Lakh ","Crore " };

            numbers[0] = inputNo % 1000; // units
            numbers[1] = inputNo / 1000;
            numbers[2] = inputNo / 100000;
            numbers[1] = numbers[1] - 100 * numbers[2]; // thousands
            numbers[3] = inputNo / 10000000; // crores
            numbers[2] = numbers[2] - 100 * numbers[3]; // lakhs

            for(int i = 3;i > 0;i--) {
                if(numbers[i] != 0) {
                    first = i;
                    break;
                }
            }
            for(int i = first;i >= 0;i--) {
                if(numbers[i] == 0) continue;
                u = numbers[i] % 10; // ones
                t = numbers[i] / 10;
                h = numbers[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if(h > 0) sb.Append(words0[h] + "Hundred ");
                if(u > 0 || t > 0) {
                    if(h > 0 || i == 0) sb.Append("and ");
                    if(t == 0)
                        sb.Append(words0[u]);
                    else if(t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if(i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

        public static string FormatAWBNo(string awb_no) {
            return (awb_no.Length >= 11 ? awb_no.Substring(0,3) + " " + awb_no.Substring(3,4) + " " + awb_no.Substring(7,awb_no.Length - 7) : awb_no);
        }

        public static string FormatAWBNo2(string awb_no) {
            return (awb_no.Length >= 11 ? awb_no.Substring(0,3) + " " + awb_no.Substring(3,awb_no.Length - 3) : awb_no);
        }

        public static string GetAWBNoWithOutPrefix(string awb_no) {
            return (awb_no.Length >= 11 ? awb_no.Substring(3,awb_no.Length - 3) : awb_no);
        }

        public static string GetAWBNoPrefix(string awb_no) {
            return (awb_no.Length >= 11 ? awb_no.Substring(0,3) : "");
        }

        /// <summary>
        /// Returns the first day of the week that the specified
        /// date is in using the current culture. 
        /// </summary>
        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek) {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return GetFirstDayOfWeek(dayInWeek,defaultCultureInfo);
        }

        public static DateTime GetLastDayOfWeek(DateTime dayInWeek) {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return GetFirstDayOfWeek(dayInWeek,defaultCultureInfo).AddDays(6);
        }

        /// <summary>
        /// Returns the first day of the week that the specified date 
        /// is in. 
        /// </summary>
        private static DateTime GetFirstDayOfWeek(DateTime dayInWeek,CultureInfo cultureInfo) {
            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;
            while(firstDayInWeek.DayOfWeek != firstDay)
                firstDayInWeek = firstDayInWeek.AddDays(-1);

            return firstDayInWeek;
        }

        public static string GetLetter(int intCol) {

            int intFirstLetter = ((intCol) / 676) + 64;
            int intSecondLetter = ((intCol % 676) / 26) + 64;
            int intThirdLetter = (intCol % 26) + 65;  // ' SHOULD BE + 64?

            char FirstLetter = (intFirstLetter > 64) ? (char)intFirstLetter : ' ';
            char SecondLetter = (intSecondLetter > 64) ? (char)intSecondLetter : ' ';
            char ThirdLetter = (char)intThirdLetter;

            return string.Concat(FirstLetter,SecondLetter,ThirdLetter).Trim();
        }

        public static object GetValue(PropertyInfo[] properties,string propertyName,object source) {
            object value = null;
            if(properties != null && source != null) {
                PropertyInfo property = (from q in properties
                                         where q.Name == propertyName
                                         select q).FirstOrDefault();
                if(property != null) {
                    value = property.GetValue(source);
                }
            }
            return value;
        }
    }
}
