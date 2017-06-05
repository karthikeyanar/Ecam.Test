using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using DocumentFormat.OpenXml.Drawing;
using System.Data.Common;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Threading;

namespace Ecam.Framework
{
    public static class JsonSerializer
    {
        public static string ToJsonObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }

    public static class Crypt
    {
        public static string SaltMd5AddSecret(string source)
        {
            string result = string.Empty;
            try
            {
                // Declarations
                Byte[] originalBytes;
                Byte[] encodedBytes;
                Byte[] salt = ASCIIEncoding.Default.GetBytes("qWzxErsd23iOw43");
                HMACMD5 hmacMD5 = new HMACMD5(salt);
                // Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
                originalBytes = ASCIIEncoding.Default.GetBytes(source);
                encodedBytes = hmacMD5.ComputeHash(originalBytes);
                // Convert encoded bytes back to a 'readable' string
                return BitConverter.ToString(encodedBytes);
            }
            catch (Exception)
            {
            }
            return result;
        }
    }

    public static class Helper
    {

        //public static string AuthTokenExpire {
        //    get {
        //        return System.Configuration.ConfigurationManager.AppSettings["auth_token_expire"].ToString();
        //    }
        //}

        public static bool IsECAWBSite {
            get {
                return false; // (Helper.SiteUrl == "http://www.ecawb.com" || Helper.SiteUrl == "http://localhost:5151");
            }
        }

        public static string SiteUrl {
            get {
                string siteUrl = string.Empty;
                try
                {
                    string url = HttpContext.Current.Request.Url.OriginalString.ToLower();
                    Regex regex = new Regex(
                    @"(?<Protocol>\w+):\/\/(?<Domain>[\w|.*|:|-]+\/?)\S*(?x)"
                    + @"  # URL",
                    RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.IgnorePatternWhitespace
                    | RegexOptions.Compiled
                    );
                    MatchCollection collection = regex.Matches(url);
                    foreach (Match m in collection)
                    {
                        string protoCol = m.Groups["Protocol"].Value;
                        string domain = m.Groups["Domain"].Value.ToString().Replace("/", "").Replace(":80", "");
                        siteUrl = (string.IsNullOrEmpty(protoCol) == false ? protoCol + "://" : "") + domain;
                    }
                }
                catch {
                    siteUrl = System.Configuration.ConfigurationManager.AppSettings["server_url"].ToString();
                }
                return siteUrl;
            }
        }

        public static string ServerURL {
            get {
                //return System.Configuration.ConfigurationManager.AppSettings["server_url"].ToString();
                return Helper.SiteUrl;
            }
        }

        public static string ApiURL {
            get {
                //return System.Configuration.ConfigurationManager.AppSettings["api_url"].ToString();
                return Helper.SiteUrl + "/api";
            }
        }

        public static string ProjectName {
            get {
                return System.Configuration.ConfigurationManager.AppSettings["project_name"].ToString();
            }
        }

        public static bool IsDVL {
            get {
                var setting = System.Configuration.ConfigurationManager.AppSettings["is_dvl"];
                if (setting != null)
                    return (setting.ToString() == "true");
                else
                    return false;
            }
        }

        public static string IsLocal {
            get {
                return System.Configuration.ConfigurationManager.AppSettings["is_local"].ToString();
            }
        }

        public static string Version {
            get {
                return System.Configuration.ConfigurationManager.AppSettings["product_version"].ToString();
            }
        }

        public static string IsUseMinifier {
            get {
                return System.Configuration.ConfigurationManager.AppSettings["is_use_minifier"].ToString();
            }
        }

        public static List<string> ImageFileExtensions {
            get {
                return new List<string>() { ".jpg", ".jpeg", ".png", ".bmp" };
            }
        }

        public static string ImageFileErrorMessage {
            get {
                string err = string.Empty;
                foreach (string ext in ImageFileExtensions)
                {
                    err += ext + ",";
                }
                if (string.IsNullOrEmpty(err) == false)
                {
                    err = err.Substring(0, err.Length - 1) + "  files only allowed";
                }
                return err;
            }
        }

        public static string TempPathSettingName {
            get {
                return "TempPath";
            }
        }

        public static string AirlineLogoPathSettingName {
            get {
                return "AirlineLogoPath";
            }
        }

        public static string CSRTemplatePathSettingName {
            get {
                return "CSRTemplatePath";
            }
        }

        public static string PreAlertEmailTemplatePathSettingName {
            get {
                return "PreAlertExcelTemplatePath";
            }
        }

        public static string BarcodeFilePathSettingName {
            get {
                return "BarcodeFilePath";
            }
        }

        public static string DocumentPathSettingName {
            get {
                return "DocumentPath";
            }
        }

        public static string FlightBookFilePathSettingName {
            get {
                return "FlightBookFilePath";
            }
        }

        public static string CompanyLogoPathSettingName {
            get {
                return "CompanyLogoPath";
            }
        }

        public static string InvoiceLogoPathSettingName {
            get {
                return "InvoiceLogoPath";
            }
        }

        public static string UserPhotoPathSettingName {
            get {
                return "UserPhotoPath";
            }
        }

        public static void Log(string log)
        {
            string fileName = DateTime.Now.ToString("dd-MMM-yyyy") + ".txt";
            log = string.Format("{0} LOG: {1}", DateTime.Now.ToString("hh:mm:ss fff"), log + Environment.NewLine);
            UploadFileHelper.WriteFileText(Helper.TempPathSettingName, fileName, log, true);
            //try
            //{
            //    Console.WriteLine(log);
            //}
            //catch { }
        }

        public static void Log(string log, string description)
        {
            string fileName = description + "_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt";
            log = string.Format("{0} LOG: {1}", DateTime.Now.ToString("hh:mm:ss fff"), log + Environment.NewLine);
            UploadFileHelper.WriteFileText(Helper.TempPathSettingName, fileName, log, true);
            //try
            //{
            //    Console.WriteLine(log);
            //}
            //catch { }
        }

        public static void WriteSQL(string sql, string description)
        {
            string fileName = description + "_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt";
            UploadFileHelper.WriteFileText(Helper.TempPathSettingName, fileName, sql, true);
            //try
            //{
            //    Console.WriteLine(log);
            //}
            //catch { }
        }

        public static void SessionLog(string log)
        {
            string fileName = "SessionLog_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt";
            log = string.Format("{0} LOG: {1}", DateTime.Now.ToString("hh:mm:ss fff"), log + Environment.NewLine);
            UploadFileHelper.WriteFileText(Helper.TempPathSettingName, fileName, log, true);
            // try
            //{
            //    Console.WriteLine(log);
            //}
            //catch { }
        }

        public static void DBErrorLog(string log)
        {
            string fileName = "DBError_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt";
            log = string.Format("{0} LOG: {1}", DateTime.Now.ToString("hh:mm:ss fff"), log + Environment.NewLine);
            UploadFileHelper.WriteFileText(Helper.TempPathSettingName, fileName, log, true);
            //try
            //{
            //    Console.WriteLine(log);
            //}
            //catch { }
        }

        public static void AlertEmailLog(string log)
        {
            string fileName = "AlertEmailLog_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt";
            log = string.Format("{0} LOG: {1}", DateTime.Now.ToString("hh:mm:ss fff"), log + Environment.NewLine);
            UploadFileHelper.WriteFileText(Helper.TempPathSettingName, fileName, log, true);
            //try
            //{
            //    Console.WriteLine(log);
            //}
            //catch { }
        }

        public static void WriteEmail(string html)
        {
            Random rnd = new Random();
            string fileName = "Email_" + DateTime.Now.ToString("yyyy_MMM_dd_hh_mm_ss") + "_" + rnd.Next(1000, 100000) + ".html";
            UploadFileHelper.WriteFileText(Helper.TempPathSettingName, fileName, html, false);
        }

        public static string ConnectionString {
            get {
                return ConfigurationManager.ConnectionStrings["EcamContext"].ConnectionString;
            }
        }

        public static T ParseFromJson<T>(object o)
        {
            dynamic dyn = o;
            T obj = Activator.CreateInstance<T>();
            // loop through all the properties of T and try to find   if J also has that property. If yes, then copy the values from J to T
            Type t = typeof(T);
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                try
                {
                    property.SetValue(obj, dyn[property.Name], null);
                }
                catch
                {
                }
            }
            return obj;
        }

        public static void SetValue(object inputObject, PropertyInfo propertyInfo, object propertyVal)
        {
            if ((propertyVal == DBNull.Value) == false)
            {
                //try {
                //    propertyInfo.SetValue(inputObject,propertyVal,null);
                //} catch {
                //find out the type
                //Type type = inputObject.GetType();
                //get the property information based on the type
                //System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);
                //find the property type
                Type propertyType = propertyInfo.PropertyType;
                //Convert.ChangeType does not handle conversion to nullable types
                //if the property type is nullable, we need to get the underlying type of the property
                var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                //Returns an System.Object with the specified System.Type and whose value is
                //equivalent to the specified object.
                propertyVal = Convert.ChangeType(propertyVal, targetType);
                //Set the value of the property
                propertyInfo.SetValue(inputObject, propertyVal, null);
                //}
            }
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static List<T> GetDataReaderToGenericList1<T>(IDataReader dr) where T : new()
        {
            Type businessEntityType = typeof(T);
            List<T> entitys = new List<T>();
            //Hashtable hashtable = new Hashtable();
            PropertyInfo[] properties = businessEntityType.GetProperties();
            //foreach(PropertyInfo info in properties) {
            //    hashtable[info.Name.ToUpper()] = info;
            //}
            while (dr.Read())
            {
                T newObject = new T();
                for (int index = 0; index < dr.FieldCount; index++)
                {
                    PropertyInfo info = businessEntityType.GetProperty(dr.GetName(index)); // properties.Select(q => q.Name == dr.GetName(index)).FirstOrDefault(); // (PropertyInfo)hashtable[dr.GetName(index).ToUpper()];
                    if ((info != null) && info.CanWrite)
                    {
                        SetValue(newObject, info, dr.GetValue(index));
                        //info.SetValue(newObject,dr.GetValue(index).ToString());
                        //info.SetValue(newObject,dr.GetValue(index),null);
                    }
                }
                entitys.Add(newObject);
            }
            dr.Close();
            return entitys;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <param name="objectToCopyFrom"></param>
        /// <param name="mapping">Mapping of TargetPropertyName(Key) and SourcePropertyName(Value)</param>
        /// <returns></returns>
        public static T CopyValues<T, J>(J objectToCopyFrom, NameValueCollection mapping = null)
        {
            T objectToCopyTo = Activator.CreateInstance<T>();
            return CopyValues(objectToCopyFrom, objectToCopyTo, mapping);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="J"></typeparam>
        /// <param name="objectToCopyFrom"></param>
        /// <param name="mapping">Mapping of TargetPropertyName(Key) and SourcePropertyName(Value). If you want some properties to be excluded, set the value as string.Empty</param>
        /// <returns></returns>
        public static T CopyValues<T, J>(J objectToCopyFrom, T objectToCopyTo, NameValueCollection mapping = null)
        {
            // loop through all the properties of T and try to find   if J also has that property. If yes, then copy the values from J to T
            Type t = typeof(T);
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] sourceObjProperties = typeof(J).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                string targetPropertyName = property.Name;
                if (mapping != null)
                {
                    if (mapping[property.Name] != null)
                    {
                        targetPropertyName = mapping[property.Name];
                    }
                }

                if (!string.IsNullOrEmpty(targetPropertyName))
                {
                    PropertyInfo targetProperty =
                            sourceObjProperties.Where(x => x.Name == targetPropertyName).FirstOrDefault();
                    if (targetProperty != null)
                    {
                        try
                        {
                            property.SetValue(objectToCopyTo, targetProperty.GetValue(objectToCopyFrom, null), null);
                        }
                        catch (Exception ex)
                        {
                            string err = ex.Message;
                        }
                    }
                }
            }
            return objectToCopyTo;
        }

        public static string RegExReplace(this string source, string oldChar, string newChar)
        {
            if (source == null)
                return source;
            if (oldChar == null)
                oldChar = string.Empty;
            if (newChar == null)
                newChar = string.Empty;
            string v = Regex.Replace(source, oldChar, newChar, RegexOptions.IgnoreCase
                | RegexOptions.Multiline
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled);
            return v;
        }

        public static string GetValidFileName(string rawFileName)
        {

            string fileName = rawFileName;

            //special chars not allowed in filename  

            string specialChars = @"\/:*?""<>|#%&.,{}~";

            //Replace special chars in raw filename with empty spaces to make it valid  

            Array.ForEach(specialChars.ToCharArray(), specialChar => fileName = fileName.Replace(specialChar, ' '));

            fileName = fileName.Replace(" ", "-");//Recommended to remove the empty spaces in filename  

            return fileName;

        }

        public static string ConvertIds(List<int> ids)
        {
            string result = string.Empty;
            foreach (int id in ids)
            {
                result += string.Format("{0},", id);
            }
            if (string.IsNullOrEmpty(result) == false)
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        public static List<int> ConvertIntIds(string ids)
        {
            List<int> result = new List<int>();
            if (string.IsNullOrEmpty(ids) == false)
            {
                string[] arr = ids.Split((",").ToCharArray());
                foreach (string strid in arr)
                {
                    if (string.IsNullOrEmpty(strid) == false)
                    {
                        result.Add(DataTypeHelper.ToInt32(strid));
                    }
                }
            }
            return result;
        }

        public static List<int?> ConvertIntNullIds(string ids)
        {
            List<int?> result = new List<int?>();
            if (string.IsNullOrEmpty(ids) == false)
            {
                string[] arr = ids.Split((",").ToCharArray());
                foreach (string strid in arr)
                {
                    if (string.IsNullOrEmpty(strid) == false)
                    {
                        result.Add(DataTypeHelper.ToInt32(strid));
                    }
                }
            }
            return result;
        }

        public static string ConvertStringIds(string ids)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(ids) == false)
            {
                string[] arr = ids.Split((",").ToCharArray());
                foreach (string strid in arr)
                {
                    if (string.IsNullOrEmpty(strid) == false)
                    {
                        //int id = DataTypeHelper.ToInt32(strid);
                        result += string.Format("{0},", strid);
                    }
                }
            }
            if (string.IsNullOrEmpty(result) == false)
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        public static string ConvertStringIds(List<string> ids)
        {
            string result = string.Empty;
            foreach (string strid in ids)
            {
                if (string.IsNullOrEmpty(strid) == false)
                {
                    result += string.Format("{0},", strid);
                }
            }
            if (string.IsNullOrEmpty(result) == false)
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        public static string ToSQL(string value)
        {
            return value.Replace("'", "''");
        }

        public static string ConvertStringSQLFormat(string values)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(values) == false)
            {
                string[] arr = values.Split((",").ToCharArray());
                foreach (string strid in arr)
                {
                    if (string.IsNullOrEmpty(strid) == false)
                    {
                        result += string.Format("'{0}',", strid.Replace("'", ""));
                    }
                }
            }
            if (string.IsNullOrEmpty(result) == false)
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        public static List<string> ConvertStringList(string ids)
        {
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(ids) == false)
            {
                string[] arr = ids.Split((",").ToCharArray());
                foreach (string strid in arr)
                {
                    if (string.IsNullOrEmpty(strid) == false)
                    {
                        list.Add(strid);
                    }
                }
            }
            return list;
        }

        public static int GetAirlineDayOfWeek(DateTime? dt)
        {
            int dayIndex = 0;
            DateTime minDate = Convert.ToDateTime("01/01/1900");
            if (dt.HasValue == false)
                dt = minDate;
            string dayName = dt.Value.ToString("dddd").ToUpper();
            //"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
            switch (dayName)
            {
                case "MONDAY": dayIndex = 0; break;
                case "TUESDAY": dayIndex = 1; break;
                case "WEDNESDAY": dayIndex = 2; break;
                case "THURSDAY": dayIndex = 3; break;
                case "FRIDAY": dayIndex = 4; break;
                case "SATURDAY": dayIndex = 5; break;
                case "SUNDAY": dayIndex = 6; break;
            }
            return dayIndex;
        }

        //public static string ConvertStatusList(string ids) {
        //	string types = "";
        //	if(string.IsNullOrEmpty(ids) == false) {
        //		string[] arr = ids.Split((",").ToCharArray());
        //		foreach(string s in arr) {
        //			if(string.IsNullOrEmpty(s) == false) {
        //				types += string.Format("'{0}',",s);
        //			}
        //		}
        //		if(string.IsNullOrEmpty(types) == false) {
        //			types = types.Substring(0,types.Length - 1);
        //		}
        //	}
        //	return types;
        //}

        public static void AddDictionary(string name, string value, ref IDictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey(name) == true)
            {
                dictionary[name] = value;
            }
            else
            {                
                dictionary.Add(name, value);
            }
        }

    }

    public enum ConfigUtil
    {
        //public static int SystemEntityID = 1;
        ///// <summary>
        ///// Each entity should get an ID greater or equal to this value.
        ///// This is used in the validation to make sure we are not inserting any
        ///// data which is for an invalid Entity( 0, or SystemEntity)
        ///// </summary>
        //public static int EntityIDStartRange = 2;
        ///// <summary>
        ///// Data point used to make sure all the IDs start from this range. 
        ///// Used for validation
        ///// </summary>
        //public static int IDStartRange = 1;
        //public static int CurrentEntityID = 2;
        SystemEntityID = 1,

        /// <summary>
        /// Each entity should get an ID greater or equal to this value.
        /// This is used in the validation to make sure we are not inserting any
        /// data which is for an invalid Entity( 0, or SystemEntity)
        /// </summary>
        EntityIDStartRange = 1,

        /// <summary>
        /// Data point used to make sure all the IDs start from this range. 
        /// Used for validation
        /// </summary>
        IDStartRange = 1
        //CurrentEntityID = 2
    }
}

