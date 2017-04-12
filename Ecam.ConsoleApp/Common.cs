using Ecam.Framework;
using Ecam.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;

namespace Ecam.ConsoleApp {
    public class Common {

        public static void DoesNotExistLog(string log) {
            string fileName = "DoesNotExistLog_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt";
            log = string.Format("{0} LOG: {1}",DateTime.Now.ToString("hh:mm:ss fff"),log + Environment.NewLine);
            WriteFileText(fileName,log,true,true);
        }

        public static void NewEntryLog(string description,string log) {
            string fileName = description + "_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt";
            log = string.Format("{0} LOG: {1}",DateTime.Now.ToString("hh:mm:ss fff"),log + Environment.NewLine);
            WriteFileText(fileName,log,true,false);
        }

        public static void Error(string log,bool isNotAppendTime = false) {
            string fileName = "Error_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt";
            if(isNotAppendTime == false) {
                log = string.Format("{0} LOG: {1}",DateTime.Now.ToString("hh:mm:ss fff"),log + Environment.NewLine);
            }
            WriteFileText(fileName,log,true,true);
        }

        public static void Log(string log,bool isNotAppendTime = false) {
            string fileName = "Log_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt";
            if(isNotAppendTime == false) {
                log = string.Format("{0} LOG: {1}",DateTime.Now.ToString("hh:mm:ss fff"),log + Environment.NewLine);
            }
            WriteFileText(fileName,log,true,false);
        }

        public static void WriteFileText(string fileName,string contents,bool isAppend,bool isError) {
            try {
				Console.ForegroundColor = ConsoleColor.White;
                if(isError == true)
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(contents);
                string rootPath = System.Configuration.ConfigurationManager.AppSettings["logpath"]; ;
                string tempFileName = Path.Combine(rootPath,fileName);
                string directoryName = Path.GetDirectoryName(tempFileName);
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
            } catch { }
        }
    }
}
