using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

[assembly: PreApplicationStartMethod(typeof(Ecam.Framework.InitHelper),"Initialize")]
namespace Ecam.Framework {

    public class InitHelper {
        public static List<string> FileExtensions { get; set; }
        public static void Initialize() {
            FileExtensions = new List<string>();
            string fileExtensions = ConfigurationManager.AppSettings["file_extensions"];
            if(string.IsNullOrEmpty(fileExtensions) == false) {
                string[] arr = fileExtensions.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                foreach(string fileType in arr) {
                    FileExtensions.Add(fileType);
                }
            }
        }
    }

    interface IFileUpload {
        UploadPathKeyCollection UploadPathKeys { get; }
        UploadFileModel UploadFile(HttpPostedFileBase uploadFile,string appSettingName,params object[] args);
        UploadFileModel UploadImageFile(Image imageFile,string appSettingName,params object[] args);
        FileModel UploadFile(HttpPostedFile uploadFile,string appSettingName,params object[] args);
        void DeleteAllFiles(string appSettingName);
        bool DeleteFile(string appSettingName,string fileName);
        bool DeleteDirectory(string appSettingName,string directoryName);
        bool FileExist(string appSettingName,string fileName);
        bool FileCopy(string sourceAppSettingName, string sourceFileName, string destAppSettingName, string destFileName);
        string GetFullFileName(string appSettingName,string fileName);
        Stream GetFileContentStream(string appSettingName,string fileName);
        string GetDirectoryPath(string appSettingName);
        string GetPath(string appSettingName);
        FileInfo WriteFileAllBytes(string appSettingName,string fileName,byte[] bytes);
        FileInfo WriteFileText(string appSettingName,string fileName,string contents,bool isAppend);
        bool ConvertImageFile(string appSettingName,string fileName,string convertName, string imageFormat);
        string GetTempFolderPath();
        string CreateFolderOnTempPath(string directoryName);
    }

    public class UploadFileModel {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
    }

    public class FileModel {

        public FileModel() {
            this.Errors = new List<ErrorInfo>();
        }

        public string FilePath { get; set; }

        public string FileName { get; set; }

        public long Size { get; set; }

        public IEnumerable<ErrorInfo> Errors { get; set; }
    }

    public static class UploadFileHelper {

        private static IFileUpload _FileUpload = null;

        static UploadFileHelper() {
            string windowsazure = ConfigurationManager.AppSettings["windows_azure"];
            if(windowsazure == "true")
                _FileUpload = (IFileUpload)ConfigurationManager.GetSection("WindowsAzureFileUpload");
            else
                _FileUpload = (IFileUpload)ConfigurationManager.GetSection("ServerFileUpload");
        }

        public static UploadFileModel Upload(HttpPostedFileBase uploadFile,string appSettingName,params object[] args) {
            return _FileUpload.UploadFile(uploadFile,appSettingName,args);
        }

        public static UploadFileModel UploadImageFile(Image imageFile,string appSettingName,params object[] args) {
            return _FileUpload.UploadImageFile(imageFile,appSettingName,args);
        }

        public static bool CheckFilePath(string filePath) {
            Regex regex = new Regex(
                                    @"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)"
                                    + @"*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$",
                                    RegexOptions.IgnoreCase
                                    | RegexOptions.Multiline
                                    | RegexOptions.IgnorePatternWhitespace
                                    | RegexOptions.Compiled
                                    );
            return regex.IsMatch(filePath);
        }

        public static IEnumerable<ErrorInfo> CheckFileExtension(string fileName) {
            FileInfo fileInfo = new FileInfo(fileName);
            List<ErrorInfo> errors = new List<ErrorInfo>();
            string extension = fileInfo.Extension.ToLower();
            bool isExist = (from ex in InitHelper.FileExtensions
                            where ex == extension
                            select ex).Count() > 0;
            if(isExist == false) {
                errors.Add(new ErrorInfo { ErrorMessage = string.Format("{0} Extension is not allowed",extension) });
            }
            return errors;
        }

        public static string AppSetting(string key) {
            return _FileUpload.UploadPathKeys[key].Value;
        }
        
        public static FileModel Upload(HttpPostedFile uploadFile,string appSettingName,params object[] args) {
            List<ErrorInfo> errors = (List<ErrorInfo>)CheckFileExtension(uploadFile.FileName);
            if(errors.Any() == false) {
                return _FileUpload.UploadFile(uploadFile,appSettingName,args);
            }
            return new FileModel { Errors = errors };
        }
         
        public static bool FileExist(string appSettingName,string fileName) {
            return _FileUpload.FileExist(appSettingName,fileName);
        }

        public static bool FileCopy(string sourceAppSettingName, string sourceFileName, string destAppSettingName, string destFileName)
        {
            return _FileUpload.FileCopy(sourceAppSettingName, sourceFileName, destAppSettingName, destFileName);
        }

        public static bool DeleteFile(string appSettingName,string fileName) {
            return _FileUpload.DeleteFile(appSettingName,fileName);
        }

        public static bool DeleteDirectory(string appSettingName,string directoryName) {
            return _FileUpload.DeleteDirectory(appSettingName,directoryName);
        }

        public static FileInfo WriteFileAllBytes(string appSettingName,string fileName,byte[] bytes) {
            return _FileUpload.WriteFileAllBytes(appSettingName,fileName,bytes);
        }

        public static FileInfo WriteFileText(string appSettingName,string fileName,string text,bool isAppend) {
            return _FileUpload.WriteFileText(appSettingName,fileName,text,isAppend);
        }

        public static string GetDirectoryPath(string appSettingName) {
            return _FileUpload.GetDirectoryPath(appSettingName);
        }

        public static string GetTempFileName(string appSettingName,string fileName) {
            Random rnd = new Random();
            string fname = Path.GetFileNameWithoutExtension(fileName);
            string ext = Path.GetExtension(fileName);
            string randomNumber = rnd.Next(1000,100000).ToString();
            fname = fname + "-" + randomNumber + ext;
            return Path.Combine(UploadFileHelper.GetDirectoryPath(appSettingName),fname);
        }

        public static string GetPath(string appSettingName) {
            return _FileUpload.GetPath(appSettingName);
        }

        public static string GetFullFileName(string appSettingName,string fileName) {
            return _FileUpload.GetFullFileName(appSettingName,fileName);
        }

        public static Stream GetFileContentStream(string appSettingName,string fileName) {
            return _FileUpload.GetFileContentStream(appSettingName,fileName);
        }

        public static void DeleteAllFiles(string appSettingName) {
            _FileUpload.DeleteAllFiles(appSettingName);
        }

        public static bool ConvertImageFile(string appSettingName,string fileName,string convertName,string imageFormat) {
            return _FileUpload.ConvertImageFile(appSettingName,fileName,convertName,imageFormat);
        }

        public static string GetTempFolderPath() {
            return _FileUpload.GetTempFolderPath();
        }

        public static string CreateFolderOnTempPath(string directoryName) {
            return _FileUpload.CreateFolderOnTempPath(directoryName);
        }
    }

    /// <summary>
    /// The collection class that will store the list of each element/item that
    /// is returned back from the configuration manager.
    /// </summary>
    [ConfigurationCollection(typeof(UploadPathElement))]
    public class UploadPathKeyCollection:ConfigurationElementCollection {
        protected override ConfigurationElement CreateNewElement() {
            return new UploadPathElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((UploadPathElement)(element)).Key;
        }

        public UploadPathElement this[int idx] {
            get {
                return (UploadPathElement)BaseGet(idx);
            }
        }

        public UploadPathElement this[string key] {
            get {
                return (UploadPathElement)BaseGet(key);
            }
        }
    }

    /// <summary>
    /// The class that holds onto each element returned by the configuration manager.
    /// </summary>
    public class UploadPathElement:ConfigurationElement {
        [ConfigurationProperty("key",DefaultValue = "",IsKey = true,IsRequired = true)]
        public string Key {
            get {
                return ((string)(base["key"]));
            }
            set {
                base["key"] = value;
            }
        }

        [ConfigurationProperty("value",DefaultValue = "",IsKey = false,IsRequired = false)]
        public string Value {
            get {
                return ((string)(base["value"]));
            }
            set {
                base["value"] = value;
            }
        }
    }
}
