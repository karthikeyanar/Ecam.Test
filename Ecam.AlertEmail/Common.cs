using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;

namespace Ecam.AlertEmail
{
    public class Common {
        public static string ConnectionString {
            get {
                return System.Configuration.ConfigurationManager.ConnectionStrings["EcamContext"].ToString();
            }
        }

        public static string RemoveHTML(string value) {
            Regex regex = new Regex(
    @"<[^>]*>",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled
    );
            return regex.Replace(value,"");
        }

        //public static void Log(string message) {
        //    string path = Path.Combine(Helper.LogPath,"LOG_" + DateTime.Now.ToString("dd_MMM_yyyy") + ".txt");
        //    // This text is always added, making the file longer over time 
        //    // if it is not deleted. 
        //    using(StreamWriter sw = File.AppendText(path)) {
        //        sw.WriteLine(message);
        //    }
        //}

        //public static void WriteFile(string content, string fileName) {
        //    // This text is always added, making the file longer over time 
        //    // if it is not deleted. 
        //    File.WriteAllText(fileName,content);
        //}

        public static string ToSQL(string message) {
            return message.Replace("'","''");
        }

        //    Public Function ToSQL(ByVal message As String) As String
        //  Try
        //    Return Replace(message, "'", "''")
        //    'message = Replace(message, "'", "''")
        //    'Return "'" & message & "'"
        //  Catch ex As Exception
        //    Return ""
        //  End Try
        //End Function

    }

    public static class DataTypeHelper
    {

        public static int GetQuarter(this DateTime dateTime)
        {
            if (dateTime.Month <= 3)
                return 1;

            if (dateTime.Month <= 6)
                return 2;

            if (dateTime.Month <= 9)
                return 3;

            return 4;
        }

        public static string ConvertBit(bool? isCondition)
        {
            return ((isCondition ?? false) == true ? "1" : "0");
        }

        public static decimal SafeDivision(decimal f1, decimal f2)
        {
            if (f2 == 0)
                //throw new System.DivideByZeroException();
                return 0;
            else
                return f1 / f2;
        }

        public static double SafeDivision(double d1, double d2)
        {
            if (d2 == 0)
                //throw new System.DivideByZeroException();
                return 0;
            else
                return d1 / d2;
        }

        public static float SafeDivision(float d1, float d2)
        {
            if (d2 == 0)
                //throw new System.DivideByZeroException();
                return 0;
            else
                return d1 / d2;
        }

        private static string RemoveSymbols(string value)
        {
            if (string.IsNullOrEmpty(value) == false)
            {
                value = value.Replace("$", "").Replace("%", "").Replace(",", "").Replace("(", "-").Replace(")", "");
            }
            return value;
        }

        public static float ToFloat(string value)
        {
            value = RemoveSymbols(value);
            float returnValue;
            float.TryParse(value, out returnValue);
            return returnValue;
        }

        public static decimal ToDecimal(string value)
        {
            value = RemoveSymbols(value);
            decimal returnValue;
            decimal.TryParse(value, out returnValue);
            return returnValue;
        }

        public static Int32 ToInt32(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            value = RemoveSymbols(value.ToString());
            int returnValue;
            Int32.TryParse(value.ToString(), out returnValue);
            return returnValue;
        }

        public static Int32 ToInt32(string value)
        {
            value = RemoveSymbols(value);
            int returnValue;
            Int32.TryParse(value, out returnValue);
            return returnValue;
        }

        public static Int16 ToInt16(string value)
        {
            value = RemoveSymbols(value);
            Int16 returnValue;
            Int16.TryParse(value, out returnValue);
            return returnValue;
        }

        public static DateTime ToDateTime(string value)
        {
            DateTime returnValue;
            DateTime.TryParse(value, out returnValue);
            return returnValue.Year <= 1900 ? new DateTime(1900, 1, 1) : returnValue;
        }

        private static DateTime ToDateTime(string value, string format)
        {
            DateTime returnValue;
            DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out returnValue);
            return returnValue.Year <= 1900 ? new DateTime(1900, 1, 1) : returnValue;
        }

        public static DateTime ToDateTimeAllFormat(string value)
        {
            DateTime returnValue = ToDateTime(value, "dd/MM/yyyy");
            if (returnValue.Year <= 1900)
            {
                returnValue = ToDateTime(value, "dd/MMM/yyyy");
            }
            if (returnValue.Year <= 1900)
            {
                returnValue = ToDateTime(value, "dd/M/yyyy");
            }
            if (returnValue.Year <= 1900)
            {
                returnValue = ToDateTime(value, "d/MM/yyyy");
            }
            if (returnValue.Year <= 1900)
            {
                returnValue = ToDateTime(value, "d/MMM/yyyy");
            }
            if (returnValue.Year <= 1900)
            {
                returnValue = ToDateTime(value, "d/M/yyyy");
            }
            return returnValue.Year <= 1900 ? new DateTime(1900, 1, 1) : returnValue;
        }

        public static DateTime ToFromOADate(string value)
        {
            DateTime returnValue;
            double dateValue = 0;
            double.TryParse(value, out dateValue);
            returnValue = DateTime.FromOADate(dateValue);
            return returnValue.Year <= 1900 ? new DateTime(1900, 1, 1) : returnValue;
        }

        public static String ConvertString(object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;
            else
                return value.ToString();
        }

        public static bool ToBoolean(object value)
        {
            if (value == null)
                return false;
            value = Convert.ToString(value);
            if (value.ToString() == "true" || value.ToString() == "1")
                return true;
            else
                return false;
        }

        public static bool CheckBoolean(string value)
        {
            bool calcValue = false;
            if (string.IsNullOrEmpty(value) == false)
                value = value.ToLower().Trim();
            else
                value = string.Empty;

            if (value.ToString().Contains("true"))
                calcValue = true;
            if (value.ToString().Contains("yes"))
                calcValue = true;
            if (value.ToString() == "1")
                calcValue = true;

            return calcValue;
        }

        public static int GetQuarterNumber(DateTime dtNow)
        {
            int quarter = 0;
            if (dtNow.Month >= 1 && dtNow.Month <= 3)
            {
                quarter = 1;
            }
            else if (dtNow.Month >= 4 && dtNow.Month <= 6)
            {
                quarter = 2;
            }
            else if (dtNow.Month >= 7 && dtNow.Month <= 9)
            {
                quarter = 3;
            }
            else if (dtNow.Month >= 10 && dtNow.Month <= 12)
            {
                quarter = 4;
            }
            return quarter;
        }

        public static DateTime[] DatesOfQuarter(DateTime dtNow)
        {
            DateTime[] dtReturn = new DateTime[2];
            if (dtNow.Month >= 1 && dtNow.Month <= 3)
            {
                dtReturn[0] = DateTime.Parse("1/1/" + dtNow.Year.ToString());
                dtReturn[1] = DateTime.Parse("3/31/" + dtNow.Year.ToString());
            }
            else if (dtNow.Month >= 4 && dtNow.Month <= 6)
            {
                dtReturn[0] = DateTime.Parse("4/1/" + dtNow.Year.ToString());
                dtReturn[1] = DateTime.Parse("6/30/" + dtNow.Year.ToString());
            }
            else if (dtNow.Month >= 7 && dtNow.Month <= 9)
            {
                dtReturn[0] = DateTime.Parse("7/1/" + dtNow.Year.ToString());
                dtReturn[1] = DateTime.Parse("9/30/" + dtNow.Year.ToString());
            }
            else if (dtNow.Month >= 10 && dtNow.Month <= 12)
            {
                dtReturn[0] = DateTime.Parse("10/1/" + dtNow.Year.ToString());
                dtReturn[1] = DateTime.Parse("12/31/" + dtNow.Year.ToString());
            }
            return dtReturn;
        }

        public static string GenerateAlphaNumeric(int length)
        {
            Random random = new Random();
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            return result.ToString();
        }

        public static DateTime GetFirstDayOfMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static DateTime GetLastDayOfMonth(DateTime dateTime)
        {
            DateTime firstDayOfTheMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }

        public static string NumbersToWords(decimal inputNumber)
        {
            int inputNo = Decimal.ToInt32(inputNumber);

            if (inputNo == 0)
                return "Zero";

            int[] numbers = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (inputNo < 0)
            {
                sb.Append("Minus ");
                inputNo = -inputNo;
            }

            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ",
            "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
            "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ",
            "Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            numbers[0] = inputNo % 1000; // units
            numbers[1] = inputNo / 1000;
            numbers[2] = inputNo / 100000;
            numbers[1] = numbers[1] - 100 * numbers[2]; // thousands
            numbers[3] = inputNo / 10000000; // crores
            numbers[2] = numbers[2] - 100 * numbers[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (numbers[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (numbers[i] == 0) continue;
                u = numbers[i] % 10; // ones
                t = numbers[i] / 10;
                h = numbers[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

        public static string FormatAWBNo(string awb_no)
        {
            return (awb_no.Length >= 11 ? awb_no.Substring(0, 3) + " " + awb_no.Substring(3, 4) + " " + awb_no.Substring(7, awb_no.Length - 7) : awb_no);
        }

        /// <summary>
        /// Returns the first day of the week that the specified
        /// date is in using the current culture. 
        /// </summary>
        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return GetFirstDayOfWeek(dayInWeek, defaultCultureInfo);
        }

        public static DateTime GetLastDayOfWeek(DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return GetFirstDayOfWeek(dayInWeek, defaultCultureInfo).AddDays(6);
        }

        /// <summary>
        /// Returns the first day of the week that the specified date 
        /// is in. 
        /// </summary>
        private static DateTime GetFirstDayOfWeek(DateTime dayInWeek, CultureInfo cultureInfo)
        {
            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
                firstDayInWeek = firstDayInWeek.AddDays(-1);

            return firstDayInWeek;
        }
    }
}
