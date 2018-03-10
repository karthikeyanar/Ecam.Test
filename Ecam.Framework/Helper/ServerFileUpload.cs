using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ecam.Framework
{
    public class ServerFileUpload : ConfigurationSection, IFileUpload
    {

        [ConfigurationProperty("UploadPathKeys")]
        public UploadPathKeyCollection UploadPathKeys {
            get { return ((UploadPathKeyCollection)(base["UploadPathKeys"])); }
        }

        public bool DeleteFile(UploadFileModel file)
        {
            bool result = false;
            try
            {
                if (file != null)
                {
                    string rootPath = this.ServerMapPath();
                    string fileName = Path.Combine(rootPath, file.FilePath, file.FileName);
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                        result = true;
                    }
                }
            }
            catch { }
            return result;
        }

        public UploadFileModel UploadFile(HttpPostedFileBase uploadFile, string appSettingName, params object[] args)
        {
            string rootPath = this.ServerMapPath();
            string uploadFilePath = Path.Combine(rootPath, string.Format(this.UploadPathKeys[appSettingName].Value, args));
            string directoryName = Path.GetDirectoryName(uploadFilePath);
            UploadFileModel uploadFileModel = null;
            if (Directory.Exists(directoryName) == false)
            {
                Directory.CreateDirectory(directoryName);
            }
            if (File.Exists(uploadFilePath))
            {
                File.Delete(uploadFilePath);
            }
            uploadFile.SaveAs(uploadFilePath);
            FileInfo fileInfo = new FileInfo(uploadFilePath);
            uploadFileModel = new UploadFileModel
            {
                FileName = fileInfo.Name,
                FilePath = directoryName.Replace(rootPath, ""),
                Size = fileInfo.Length
            };
            return uploadFileModel;
        }

        public UploadFileModel UploadImageFile(Image imageFile, string appSettingName, params object[] args)
        {
            string rootPath = this.ServerMapPath();
            string uploadFilePath = Path.Combine(rootPath, string.Format(this.UploadPathKeys[appSettingName].Value, args));
            string directoryName = Path.GetDirectoryName(uploadFilePath);
            UploadFileModel uploadFileModel = null;
            if (Directory.Exists(directoryName) == false)
            {
                Directory.CreateDirectory(directoryName);
            }
            if (File.Exists(uploadFilePath))
            {
                File.Delete(uploadFilePath);
            }
            imageFile.Save(uploadFilePath);
            FileInfo fileInfo = new FileInfo(uploadFilePath);
            uploadFileModel = new UploadFileModel
            {
                FileName = fileInfo.Name,
                FilePath = directoryName.Replace(rootPath, ""),
                Size = fileInfo.Length
            };
            return uploadFileModel;
        }

        public FileModel UploadFile(HttpPostedFile uploadFile, string appSettingName, params object[] args)
        {
            string rootPath = this.ServerMapPath();
            string uploadFilePath = Path.Combine(rootPath, string.Format(this.UploadPathKeys[appSettingName].Value, args));
            string directoryName = Path.GetDirectoryName(uploadFilePath);
            FileModel uploadFileModel = null;
            if (Directory.Exists(directoryName) == false)
            {
                Directory.CreateDirectory(directoryName);
            }
            if (File.Exists(uploadFilePath))
            {
                File.Delete(uploadFilePath);
            }
            uploadFile.SaveAs(uploadFilePath);
            FileInfo fileInfo = new FileInfo(uploadFilePath);
            uploadFileModel = new FileModel
            {
                FileName = fileInfo.Name,
                FilePath = directoryName.Replace(rootPath, ""),
                Size = fileInfo.Length
            };
            return uploadFileModel;
        }

        public string GetDirectoryPath(string appSettingName)
        {
            string url = string.Empty;
            if (string.IsNullOrEmpty(appSettingName) == false)
            {
                string rootPath = this.ServerMapPath();
                url = Path.Combine(rootPath, string.Format(this.UploadPathKeys[appSettingName].Value, ""));
            }
            return url;
        }

        public string GetPath(string appSettingName)
        {
            string url = string.Empty;
            if (string.IsNullOrEmpty(appSettingName) == false)
            {
                url = string.Format(this.UploadPathKeys[appSettingName].Value, "");
            }
            return url;
        }

        private string ServerMapPath()
        {
            string path = string.Empty;
            try
            {
                path = HttpContext.Current.Server.MapPath("~/");
            }
            catch
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("file_path"))
                    path = System.Configuration.ConfigurationManager.AppSettings["file_path"].ToString();
            }
            return path;
        }

        public FileInfo WriteFileText(string appSettingName, string fileName, string contents, bool isAppend)
        {
            try {
                string rootPath = this.ServerMapPath();
                string tempFileName = Path.Combine(rootPath,string.Format(this.UploadPathKeys[appSettingName].Value,fileName));
                string directoryName = Path.GetDirectoryName(tempFileName);
                if(Directory.Exists(directoryName) == false) {
                    Directory.CreateDirectory(directoryName);
                }
                //File.WriteAllText(tempFileName,contents);
                if(isAppend == false) {
                    using(TextWriter w = new StreamWriter(tempFileName,true)) {
                        w.WriteLine(contents);
                        w.Flush();
                        w.Close();
                    }
                } else {
                    if(File.Exists(tempFileName) == false) {
                        File.WriteAllText(tempFileName,string.Empty);
                    }
                    using(StreamWriter w = File.AppendText(tempFileName)) {
                        w.WriteLine(contents);
                        w.Flush();
                        w.Close();
                    }
                }
                return new FileInfo(tempFileName);
            } catch {
                return null;
            }
        }

        public FileInfo WriteFileAllBytes(string appSettingName, string fileName, byte[] bytes)
        {
            try {
                string rootPath = this.ServerMapPath();
                string tempFileName = Path.Combine(rootPath,string.Format(this.UploadPathKeys[appSettingName].Value,fileName));
                string directoryName = Path.GetDirectoryName(tempFileName);
                if(Directory.Exists(directoryName) == false) {
                    Directory.CreateDirectory(directoryName);
                }
                File.WriteAllBytes(tempFileName,bytes);
                return new FileInfo(tempFileName);
            } catch {
                return null;
            }
        }

        public bool FileExist(string appSettingName, string fileName)
        {
            string rootPath = this.ServerMapPath();
            string tempFileName = Path.Combine(rootPath, string.Format(this.UploadPathKeys[appSettingName].Value, fileName));
            return File.Exists(tempFileName);
        }

        public bool FileCopy(string sourceAppSettingName, string sourceFileName, string destAppSettingName, string destFileName)
        {
            string rootPath = this.ServerMapPath();
            string tempFileName = Path.Combine(rootPath, string.Format(this.UploadPathKeys[sourceAppSettingName].Value, sourceFileName));
            bool isSourceFileExist =  File.Exists(tempFileName);
            if (isSourceFileExist == true)
            {
                string dest = Path.Combine(rootPath, string.Format(this.UploadPathKeys[destAppSettingName].Value, destFileName));
                if (File.Exists(dest) == true)
                {
                    File.Delete(dest);
                }
                File.Copy(tempFileName, dest);
                return File.Exists(dest);
            }
            return false;
        }

        public bool DeleteFile(string appSettingName, string fileName)
        {
            string rootPath = this.ServerMapPath();
            string deleteFileName = Path.Combine(rootPath, string.Format(this.UploadPathKeys[appSettingName].Value, fileName));
            bool result = false;
            if (File.Exists(deleteFileName))
            {
                File.Delete(deleteFileName);
                result = true;
            }
            return result;
        }

        public bool DeleteDirectory(string appSettingName, string directoryName)
        {
            string rootPath = this.ServerMapPath();
            string deleteDirectoryName = Path.Combine(rootPath, string.Format(this.UploadPathKeys[appSettingName].Value, directoryName));
            bool result = false;
            if (Directory.Exists(deleteDirectoryName))
            {
                Directory.Delete(deleteDirectoryName);
                result = true;
            }
            return result;
        }

        public void DeleteAllFiles(string appSettingName)
        {
            try
            {
                string rootPath = this.ServerMapPath();
                string directoryPath = Path.Combine(rootPath, string.Format(this.UploadPathKeys[appSettingName].Value, ""));
                DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
                FileInfo[] files = dirInfo.GetFiles();
                foreach (FileInfo fle in files)
                {
                    try
                    {
                        if (File.Exists(fle.FullName))
                        {
                            File.Delete(fle.FullName);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        public string GetFullFileName(string appSettingName, string fileName)
        {
            string url = string.Empty;
            if (string.IsNullOrEmpty(fileName) == false && string.IsNullOrEmpty(appSettingName) == false)
            {
                string rootPath = this.ServerMapPath();
                url = Path.Combine(rootPath, string.Format(this.UploadPathKeys[appSettingName].Value, fileName));
            }
            return url;
        }

        public Stream GetFileContentStream(string appSettingName, string fileName)
        {
            string rootPath = this.ServerMapPath();
            string tempFileName = Path.Combine(rootPath, string.Format(this.UploadPathKeys[appSettingName].Value, fileName));
            if (File.Exists(tempFileName) == true)
            {
                byte[] bytes = System.IO.File.ReadAllBytes(tempFileName);
                MemoryStream stream = new MemoryStream(bytes);
                return stream; // new FileStream(tempFileName,FileMode.Open,FileAccess.Read);
            }
            else if (File.Exists(fileName) == true)
            {
                byte[] bytes = System.IO.File.ReadAllBytes(fileName);
                MemoryStream stream = new MemoryStream(bytes);
                return stream;
            }
            else
            {
                return null;
            }
        }



        public bool ConvertImageFile(string appSettingName, string fileName, string convertName, string imageFormat)
        {
            string rootPath = this.ServerMapPath();
            string processFileName = Path.Combine(rootPath, string.Format(this.UploadPathKeys[appSettingName].Value, fileName));
            bool result = false;
            try
            {
                if (File.Exists(processFileName))
                {
                    FileInfo fileInfo = new FileInfo(processFileName);
                    string imageFileName = Path.Combine(fileInfo.DirectoryName, convertName);
                    using (Bitmap map = new Bitmap(processFileName))
                    {
                        switch (imageFormat)
                        {
                            case "png":
                                map.Save(imageFileName, ImageFormat.Png);
                                break;
                            case "bmp":
                                map.Save(imageFileName, ImageFormat.Bmp);
                                break;
                            case "tiff":
                                map.Save(imageFileName, ImageFormat.Tiff);
                                break;
                            default:
                                map.Save(imageFileName, ImageFormat.Jpeg);
                                break;
                        }

                    }
                    result = true;
                }
            }
            catch { }
            return result;
        }

        public string GetTempFolderPath()
        {
            string rootPath = this.ServerMapPath();
            string tempFileName = Path.Combine(rootPath, string.Format(this.UploadPathKeys["TempPath"].Value, string.Empty));
            string directoryName = Path.GetDirectoryName(tempFileName);
            if (Directory.Exists(directoryName) == false)
            {
                Directory.CreateDirectory(directoryName);
            }
            return directoryName;
        }

        public string CreateFolderOnTempPath(string directoryName)
        {
            string tempDirectoryPath = this.GetTempFolderPath();
            directoryName = Path.Combine(tempDirectoryPath, directoryName);
            if (Directory.Exists(directoryName) == false)
            {
                Directory.CreateDirectory(directoryName);
            }
            return directoryName;
        }
    }
}