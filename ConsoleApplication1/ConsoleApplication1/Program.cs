using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1 {
    class Program {
        private static string[] _COMPANIES;
        private static int _INDEX = -1;

        static void Main(string[] args) {
            //while(true) {
            //InvestingCSVData();
            //Nifty500Update();
            //System.Threading.Thread.Sleep(10000);
            //Console.WriteLine("Time=" + DateTime.Now);
            //Console.WriteLine("Recheck the folder");
            //}
            UpdateCategorySymbol();
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private static void UpdateCompanySymbol() {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyInvestmentContext"].ConnectionString;
            using(SqlConnection connection = new SqlConnection(connectionString)) {
                string sql = "select * from company where isnull(symbol,'')=''";
                SqlCommand command = new SqlCommand(sql,connection);
                command.Connection.Open();
                SqlDataReader dr = command.ExecuteReader();
                while(dr.Read()) {
                    string companyName = dr["CompanyName"].ToString();
                    sql = "select * from company2 where companyname like '%" + companyName.Replace("'","''") + "%'";
                    SqlCommand command2 = new SqlCommand(sql,connection);
                    SqlDataReader company2Dr = command2.ExecuteReader();
                    while(company2Dr.Read()) {
                        sql = "update company set symbol='" + company2Dr["Symbol"].ToString() + "' where companyid=" + dr["CompanyID"].ToString();
                        SqlCommand command3 = new SqlCommand(sql,connection);
                        command3.ExecuteNonQuery();
                    }
                    Console.WriteLine("Completed company=" + companyName);
                }
            }
        }

        private static void Nifty500Update() {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyInvestmentContext"].ConnectionString;
            string fileName = "E:\\Projects\\Ecam.Test2\\DotNetCore\\ConsoleApplication1\\Nifty500_Investing.csv";
            CsvReader csv = null;
            SqlParameter sqlp = null;
            string sql = string.Empty;
            int i = 0;
            if(File.Exists(fileName) == true) {
                using(TextReader reader = File.OpenText(fileName)) {
                    csv = new CsvReader(reader);
                    i = 0;
                    while(csv.Read()) {
                        i += 1;
                        string name = csv.GetField<string>("CompanyName");
                        string url = csv.GetField<string>("URL");
                        if(string.IsNullOrEmpty(name) == false) {
                            name = name.Replace("&amp;","&");
                            sql = "select isnull(companyid,0) as companyid from company where companyname = @companyname";
                            int companyId = 0;
                            using(SqlConnection connection = new SqlConnection(connectionString)) {
                                SqlCommand command = new SqlCommand(sql,connection);
                                command.Connection.Open();
                                sqlp = new SqlParameter();
                                sqlp.ParameterName = "companyname";
                                sqlp.Value = name;
                                command.Parameters.Add(sqlp);
                                SqlDataReader dr = command.ExecuteReader();
                                while(dr.Read()) {
                                    companyId = (int)dr["companyid"];
                                }
                                dr.Close();
                            }
                            if(companyId <= 0) {
                                sql = " insert into company(companyname,investingurl) values (@companyname,@url)";
                            } else {
                                sql = " update company set investingurl=@url where companyname=@companyname ";
                            }
                            using(SqlConnection connection = new SqlConnection(connectionString)) {
                                SqlCommand command = new SqlCommand(sql,connection);
                                command.Connection.Open();
                                sqlp = new SqlParameter();
                                sqlp.ParameterName = "companyname";
                                sqlp.Value = name;
                                command.Parameters.Add(sqlp);
                                sqlp = new SqlParameter();
                                sqlp.ParameterName = "url";
                                sqlp.Value = url;
                                command.Parameters.Add(sqlp);
                                command.ExecuteNonQuery();
                                Console.WriteLine("Company inserted = " + name);
                            }
                        }
                    }
                }
            }
        }

        private static void UpdateCategorySymbol() {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyInvestmentContext"].ConnectionString;
            string fileName = "E:\\Projects\\Ecam.Test2\\DotNetCore\\ConsoleApplication1\\CategorySymbols.csv";
            CsvReader csv = null;
            SqlParameter sqlp = null;
            string sql = string.Empty;
            int i = 0;
            if(File.Exists(fileName) == true) {
                using(TextReader reader = File.OpenText(fileName)) {
                    csv = new CsvReader(reader);
                    i = 0;
                    while(csv.Read()) {
                        i += 1;
                        string categoryName = csv.GetField<string>("CategoryName");
                        string symbol = csv.GetField<string>("Symbol");
                        if(string.IsNullOrEmpty(categoryName) == false) {
                            categoryName = categoryName.Replace("&amp;","&");
                            sql = "select isnull(categoryid,0) as categoryid from category where categoryname = @categoryname";
                            int categoryId = 0;
                            using(SqlConnection connection = new SqlConnection(connectionString)) {
                                SqlCommand command = new SqlCommand(sql,connection);
                                command.Connection.Open();
                                sqlp = new SqlParameter();
                                sqlp.ParameterName = "categoryname";
                                sqlp.Value = categoryName;
                                command.Parameters.Add(sqlp);
                                SqlDataReader dr = command.ExecuteReader();
                                while(dr.Read()) {
                                    categoryId = (int)dr["categoryid"];
                                }
                                dr.Close();
                            }
                            sql = "select isnull(companyid,0) as companyid from company where symbol = @symbol";
                            int companyId = 0;
                            using(SqlConnection connection = new SqlConnection(connectionString)) {
                                SqlCommand command = new SqlCommand(sql,connection);
                                command.Connection.Open();
                                sqlp = new SqlParameter();
                                sqlp.ParameterName = "symbol";
                                sqlp.Value = symbol;
                                command.Parameters.Add(sqlp);
                                SqlDataReader dr = command.ExecuteReader();
                                while(dr.Read()) {
                                    companyId = (int)dr["companyid"];
                                }
                                dr.Close();
                            }
                            if(companyId > 0 && categoryId > 0) {
                                sql = " insert into companycategory(companyid,categoryid) values (@companyid,@categoryid)";
                                using(SqlConnection connection = new SqlConnection(connectionString)) {
                                    SqlCommand command = new SqlCommand(sql,connection);
                                    command.Connection.Open();
                                    sqlp = new SqlParameter();
                                    sqlp.ParameterName = "companyid";
                                    sqlp.Value = companyId;
                                    command.Parameters.Add(sqlp);
                                    sqlp = new SqlParameter();
                                    sqlp.ParameterName = "categoryid";
                                    sqlp.Value = categoryId;
                                    command.Parameters.Add(sqlp);
                                    command.ExecuteNonQuery();
                                    Console.WriteLine("Company category inserted = " + categoryName + ",symbol=" + symbol);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void InvestingCSVData() {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyInvestmentContext"].ConnectionString;
            string importCSVDirectoryPath = System.Configuration.ConfigurationManager.AppSettings["INVESTING_CSV"];
            string backUpCSVDirectoryPath = importCSVDirectoryPath + "\\Backup";
            List<string> symbols = new List<string>();
            string[] files = System.IO.Directory.GetFiles(importCSVDirectoryPath);
            SqlParameter sqlp = null;
            int i = 0;
            foreach(string fullFileName in files) {
                i += 1;
                CsvReader csv = null;
                if(File.Exists(fullFileName) == true) {
                    List<InvestmentPrice> priceHistory = new List<InvestmentPrice>();
                    using(TextReader reader = File.OpenText(fullFileName)) {
                        csv = new CsvReader(reader);
                        while(csv.Read()) {
                            int companyID = DataTypeHelper.ToInt32(csv.GetField<string>("CompanyID"));
                            DateTime date = DataTypeHelper.ToDateTime(csv.GetField<string>("Date"));
                            decimal open = DataTypeHelper.ToDecimal(csv.GetField<string>("Open Price"));
                            decimal high = DataTypeHelper.ToDecimal(csv.GetField<string>("High Price"));
                            decimal low = DataTypeHelper.ToDecimal(csv.GetField<string>("Low Price"));
                            decimal close = DataTypeHelper.ToDecimal(csv.GetField<string>("Close Price"));
                            decimal change = DataTypeHelper.ToDecimal(csv.GetField<string>("Change"));
                            decimal prevCloseValue = close - (close * change) / 100;
                            DateTime dt = DataTypeHelper.ToDateTime(date);
                            if(companyID > 0) {
                                priceHistory.Add(new InvestmentPrice {
                                    Close = close,
                                    CompanyID = companyID,
                                    Date = date,
                                    High = high,
                                    Low = low,
                                    Open = open,
                                    PrevClose = prevCloseValue
                                });
                            }
                        }
                    }

                    priceHistory = (from q in priceHistory
                                    orderby q.Date ascending
                                    select q).ToList();

                    foreach(var price in priceHistory) {
                        string sql = "";
                        sql = "INSERT INTO [dbo].[CompanyPriceHistory]" + Environment.NewLine +
                       " ([CompanyID]" + Environment.NewLine +
                       ",[Date]" + Environment.NewLine +
                       ",[Open]" + Environment.NewLine +
                       ",[Low]" + Environment.NewLine +
                       ",[High]" + Environment.NewLine +
                       ",[Close]" + Environment.NewLine +
                       ",[PrevClose]" + Environment.NewLine +
                       " ) VALUES (" + Environment.NewLine +
                       "@companyID" + Environment.NewLine +
                       ",@date" + Environment.NewLine +
                       ",@open" + Environment.NewLine +
                       ",@low" + Environment.NewLine +
                       ",@high" + Environment.NewLine +
                       ",@close" + Environment.NewLine +
                       ",@prevclose" + Environment.NewLine +
                       ")";
                        //Console.WriteLine("dt=" + dt.ToString("MM/dd/yyyy"));
                        try {
                            using(SqlConnection connection = new SqlConnection(
                               connectionString)) {
                                SqlCommand command = new SqlCommand(sql,connection);
                                sqlp = new SqlParameter();
                                sqlp.ParameterName = "companyID";
                                sqlp.Value = price.CompanyID;
                                command.Parameters.Add(sqlp);

                                sqlp = new SqlParameter();
                                sqlp.ParameterName = "date";
                                sqlp.Value = price.Date;//.ToString("yyyy-MM-dd");
                                command.Parameters.Add(sqlp);

                                sqlp = new SqlParameter();
                                sqlp.ParameterName = "open";
                                sqlp.Value = price.Open;
                                command.Parameters.Add(sqlp);

                                sqlp = new SqlParameter();
                                sqlp.ParameterName = "low";
                                sqlp.Value = price.Low;
                                command.Parameters.Add(sqlp);

                                sqlp = new SqlParameter();
                                sqlp.ParameterName = "high";
                                sqlp.Value = price.High;
                                command.Parameters.Add(sqlp);

                                sqlp = new SqlParameter();
                                sqlp.ParameterName = "close";
                                sqlp.Value = price.Close;
                                command.Parameters.Add(sqlp);

                                sqlp = new SqlParameter();
                                sqlp.ParameterName = "prevclose";
                                sqlp.Value = price.PrevClose;
                                command.Parameters.Add(sqlp);

                                command.Connection.Open();
                                command.ExecuteNonQuery();
                            }
                        } catch(Exception ex) {
                            if(ex.Message.ToString().Contains("PRIMARY KEY") == false) {
                                Console.WriteLine("ex=" + ex.Message);
                            }
                        }
                    }
                }
                if(Directory.Exists(backUpCSVDirectoryPath) == false) {
                    Directory.CreateDirectory(backUpCSVDirectoryPath);
                }
                string movefile_name = System.IO.Path.Combine(backUpCSVDirectoryPath,System.IO.Path.GetFileName(fullFileName));
                if(System.IO.File.Exists(movefile_name) == true) {
                    System.IO.File.Delete(fullFileName);
                }
                if(System.IO.File.Exists(movefile_name) == false) {
                    System.IO.File.Move(fullFileName,movefile_name);
                }
                Console.WriteLine("Completed i=" + i + " of " + files.Count());
            }
        }

        private static void CSVData() {
            string importCSVDirectoryPath = System.Configuration.ConfigurationManager.AppSettings["IMPORT_CSV"];
            List<string> symbols = new List<string>();
            string[] files = System.IO.Directory.GetFiles(importCSVDirectoryPath);
            foreach(string fullFileName in files) {
                symbols.Add(System.IO.Path.GetFileNameWithoutExtension(fullFileName));
            }
            symbols = (from q in symbols orderby q ascending select q).ToList();
            _COMPANIES = symbols.ToArray();
            _INDEX = -1;
            CSVDownloadStart();
        }

        private static void CSVDownloadStart() {
            int totalCount = _COMPANIES.Length;
            int queueCount = 1;
            if(totalCount <= queueCount) {
                queueCount = totalCount;
            }
            // One event is used for each Fibonacci object
            ManualResetEvent[] doneEvents = new ManualResetEvent[queueCount];
            CSVDownloadData[] downArray = new CSVDownloadData[queueCount];
            //Random r = new Random();
            // Configure and launch threads using ThreadPool:
            Console.WriteLine("launching {0} tasks...",totalCount);
            for(int i = 0;i < queueCount;i++) {
                _INDEX += 1;
                string symbol = "";
                if(_INDEX < _COMPANIES.Length) {
                    symbol = _COMPANIES[_INDEX];
                }
                doneEvents[i] = new ManualResetEvent(false);
                CSVDownloadData f = new CSVDownloadData(symbol,doneEvents[i]);
                downArray[i] = f;
                ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback,i);
            }
            // Wait for all threads in pool to calculation...
            WaitHandle.WaitAll(doneEvents);
            if(_INDEX < _COMPANIES.Length) {
                Console.WriteLine("All calculations are complete.");
                CSVDownloadStart();
            }
        }
    }

    public class InvestmentPrice {
        public int CompanyID { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal PrevClose { get; set; }
    }

    public class CSVDownloadData {
        //public List<string> _SYMBOLS_LIST = null;
        private bool _IS_NOT_SUCCESS = false;
        public CSVDownloadData(string file_name,ManualResetEvent doneEvent) {
            //_SYMBOLS_LIST = symbols;
            _ORIGINAL_FILE_NAME = file_name;
            _FILE_NAME = file_name;
            _doneEvent = doneEvent;
        }

        public CSVDownloadData() {
        }

        // Wrapper method for use with thread pool.
        public void ThreadPoolCallback(Object threadContext) {
            int threadIndex = (int)threadContext;
            Console.WriteLine("thread {0} started...",threadIndex);
            if(string.IsNullOrEmpty(_FILE_NAME) == false) {
                CSVDataDownload(_FILE_NAME);
                //CalculateRSI(_FILE_NAME);
            }
            Console.WriteLine("thread {0} result calculated...",threadIndex);
            string IMPORT_CSV = System.Configuration.ConfigurationManager.AppSettings["IMPORT_CSV"];
            string IMPORT_BACKUP_CSV = IMPORT_CSV + "\\Backup";
            string fileName = IMPORT_CSV + "\\" + _ORIGINAL_FILE_NAME + ".csv";
            //if(File.Exists(fileName) == true) {
            //    File.Delete(fileName);
            //}
            if(this._IS_NOT_SUCCESS == false) {
                if(string.IsNullOrEmpty(_ORIGINAL_FILE_NAME) == false) {
                    if(Directory.Exists(IMPORT_BACKUP_CSV) == false) {
                        Directory.CreateDirectory(IMPORT_BACKUP_CSV);
                    }
                    string movefile_name = System.IO.Path.Combine(IMPORT_BACKUP_CSV,System.IO.Path.GetFileName(fileName));
                    if(System.IO.File.Exists(movefile_name) == true) {
                        System.IO.File.Delete(fileName);
                    }
                    if(System.IO.File.Exists(movefile_name) == false) {
                        System.IO.File.Move(fileName,movefile_name);
                    }
                }
            }
            Console.WriteLine(_FILE_NAME);
            _doneEvent.Set();
        }



        public string CSVDataDownload(string tempfilename,bool isTakeTempFileName = false) {
            string lastSymbol = "";
            List<OldSymbol> oldSymbolList = new List<OldSymbol> {
                new OldSymbol { old_symbol = "PFRL", new_symbol = "ABFRL" },
                new OldSymbol { old_symbol = "SKSMICRO", new_symbol = "BHARATFIN" },
                new OldSymbol { old_symbol = "ADI", new_symbol = "FAIRCHEM" },
                new OldSymbol { old_symbol = "FCEL", new_symbol = "FCONSUMER" },
                new OldSymbol { old_symbol = "FRL", new_symbol = "FEL" },
                new OldSymbol { old_symbol = "HCIL", new_symbol = "HSCL" },
                new OldSymbol { old_symbol = "HITACHIHOM", new_symbol = "JCHAC" },
                new OldSymbol { old_symbol = "MAX", new_symbol = "MFSL" },
                new OldSymbol { old_symbol = "GULFCORP", new_symbol = "GOCLCORP" },
                new OldSymbol { old_symbol = "IBSEC", new_symbol = "IBVENTURES" },
                new OldSymbol { old_symbol = "VIKASGLOB", new_symbol = "VIKASECO" },
                new OldSymbol { old_symbol = "FINANTECH", new_symbol = "63MOONS" },
                new OldSymbol { old_symbol = "CROMPGREAV", new_symbol = "CGPOWER" },
                new OldSymbol { old_symbol = "FEDDERLOYD", new_symbol = "FEDDERELEC" },
                new OldSymbol { old_symbol = "GEOJITBNPP", new_symbol = "GEOJITFSL" },
                new OldSymbol { old_symbol = "HITECHPLAS", new_symbol = "HITECHCORP" },
                new OldSymbol { old_symbol = "LLOYDELENG", new_symbol = "LEEL" },
                new OldSymbol { old_symbol = "NAGAAGRI", new_symbol = "NACLIND" },
                new OldSymbol { old_symbol = "SUJANATWR", new_symbol = "NTL" },
                new OldSymbol { old_symbol = "VIDHIDYE", new_symbol = "VIDHIING" },
                new OldSymbol { old_symbol = "STOREONE", new_symbol = "SORILINFRA" },
                new OldSymbol { old_symbol = "ABSHEKINDS", new_symbol = "TRIDENT" },
                new OldSymbol { old_symbol = "ARVINDMILL", new_symbol = "ARVIND" },
                new OldSymbol { old_symbol = "BAJAJAUTO", new_symbol = "BAJAJHLDNG" },
                new OldSymbol { old_symbol = "BAJAUTOFIN", new_symbol = "BAJFINANCE" },
                new OldSymbol { old_symbol = "BIRLAJUTE", new_symbol = "BIRLACORPN" },
                new OldSymbol { old_symbol = "BOC", new_symbol = "LINDEINDIA" },
                new OldSymbol { old_symbol = "CHOLADBS", new_symbol = "CHOLAFIN" },
                new OldSymbol { old_symbol = "DALMIABEL", new_symbol = "DALMIABHA" },
                new OldSymbol { old_symbol = "DCMSRMCONS", new_symbol = "DCMSHRIRAM" },
                new OldSymbol { old_symbol = "DEWANHOUS", new_symbol = "DHFL" },
                new OldSymbol { old_symbol = "FCH", new_symbol = "CAPF" },
                new OldSymbol { old_symbol = "FVIL", new_symbol = "FCONSUMER" },
                new OldSymbol { old_symbol = "GUJAMBCEM", new_symbol = "AMBUJACEM" },
                new OldSymbol { old_symbol = "GWALCHEM", new_symbol = "GEECEE" },
                new OldSymbol { old_symbol = "HEROHONDA", new_symbol = "HEROMOTOCO" },
                new OldSymbol { old_symbol = "HINDALC0", new_symbol = "HINDALCO" },
                new OldSymbol { old_symbol = "HINDLEVER", new_symbol = "HINDUNILVR" },
                new OldSymbol { old_symbol = "HINDSANIT", new_symbol = "HSIL" },
                new OldSymbol { old_symbol = "IBPOW", new_symbol = "RTNPOWER" },
                new OldSymbol { old_symbol = "ICI", new_symbol = "AKZOINDIA" },
                new OldSymbol { old_symbol = "ILFSTRANS", new_symbol = "IL&FSTRANS" },
                new OldSymbol { old_symbol = "JSTAINLESS", new_symbol = "JSL" },
                new OldSymbol { old_symbol = "MADRASCEM", new_symbol = "RAMCOCEM" },
                new OldSymbol { old_symbol = "MICO", new_symbol = "BOSCHLTD" },
                new OldSymbol { old_symbol = "MYSORECEM", new_symbol = "HEIDELBERG" },
                new OldSymbol { old_symbol = "NAGARCONST", new_symbol = "NCC" },
                new OldSymbol { old_symbol = "NEYVELILIG", new_symbol = "NLCINDIA" },
                new OldSymbol { old_symbol = "OBEROIREAL", new_symbol = "OBEROIRLTY" },
                new OldSymbol { old_symbol = "PANTALOONR", new_symbol = "FEL" },
                new OldSymbol { old_symbol = "RAINCOM", new_symbol = "RAIN" },
                new OldSymbol { old_symbol = "REL", new_symbol = "RELINFRA" },
                new OldSymbol { old_symbol = "SESAGOA", new_symbol = "VEDL" },
                new OldSymbol { old_symbol = "SOLAREX", new_symbol = "SOLARINDS" },
                new OldSymbol { old_symbol = "SPLLTD", new_symbol = "SOMANYCERA" },
                new OldSymbol { old_symbol = "SREINTFIN", new_symbol = "SREINFRA" },
                new OldSymbol { old_symbol = "SSLT", new_symbol = "VEDL" },
                new OldSymbol { old_symbol = "SWARAJMAZD", new_symbol = "SMLISUZU" },
                new OldSymbol { old_symbol = "TATATEA", new_symbol = "TATAGLOBAL" },
                new OldSymbol { old_symbol = "UNIPHOS", new_symbol = "UPL" },
                new OldSymbol { old_symbol = "VISHALRET", new_symbol = "V2RETAIL" },
                new OldSymbol { old_symbol = "WABCO-TVS", new_symbol = "WABCOINDIA" },
                new OldSymbol { old_symbol = "WELGUJ", new_symbol = "WELCORP" },
                new OldSymbol { old_symbol = "PRSMJOHNSN", new_symbol = "PRISMCEM" },
                new OldSymbol { old_symbol = "UTIBANK", new_symbol = "AXISBANK" },
            };
            if(string.IsNullOrEmpty(tempfilename) == false) {
                string url = string.Empty;
                string html = string.Empty;
                string IMPORT_CSV = System.Configuration.ConfigurationManager.AppSettings["IMPORT_CSV"];
                string fileName = IMPORT_CSV + "\\" + tempfilename + ".csv";
                if(isTakeTempFileName == true) {
                    fileName = tempfilename;
                }
                CsvReader csv = null;
                int i = 0;
                if(File.Exists(fileName) == true) {
                    using(TextReader reader = File.OpenText(fileName)) {
                        csv = new CsvReader(reader);
                        i = 0;
                        while(csv.Read()) {
                            i += 1;
                            string symbol = csv.GetField<string>("Symbol");
                            string series = csv.GetField<string>("Series");
                            string date = csv.GetField<string>("Date");
                            string open = csv.GetField<string>("Open Price");
                            string high = csv.GetField<string>("High Price");
                            string low = csv.GetField<string>("Low Price");
                            string close = csv.GetField<string>("Close Price");
                            string lastTrade = csv.GetField<string>("Last Price");
                            string prev = csv.GetField<string>("Prev Close");
                            string turnOver = csv.GetField<string>("Turnover");
                            DateTime dt = DataTypeHelper.ToDateTime(date);
                            if(string.IsNullOrEmpty(symbol) == false) {
                                symbol = symbol.Replace("&amp;","&");
                                OldSymbol oldSymbol = (from q in oldSymbolList
                                                       where q.old_symbol == symbol
                                                       select q).FirstOrDefault();
                                if(oldSymbol != null) {
                                    symbol = oldSymbol.new_symbol;
                                }
                                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["InvestmentContext"].ConnectionString;
                                string sql = "";
                                sql = "INSERT INTO [dbo].[EquityPriceHistory]" + Environment.NewLine +
                               " ([Symbol]" + Environment.NewLine +
                               ",[Date]" + Environment.NewLine +
                               ",[Open]" + Environment.NewLine +
                               ",[Low]" + Environment.NewLine +
                               ",[High]" + Environment.NewLine +
                               ",[Close]" + Environment.NewLine +
                               ",[PrevClose]" + Environment.NewLine +
                               ",[OriginalOpen]" + Environment.NewLine +
                               ",[OriginalLow]" + Environment.NewLine +
                               ",[OriginalHigh]" + Environment.NewLine +
                               ",[OriginalClose]" + Environment.NewLine +
                               ",[OriginalPrevClose])" + Environment.NewLine +
                               " VALUES " + Environment.NewLine +
                               "('" + symbol + "'" + Environment.NewLine +
                               ",'" + dt.ToString("MM/dd/yyyy") + "'" + Environment.NewLine +
                               ",'" + open + "'" + Environment.NewLine +
                               ",'" + low + "'" + Environment.NewLine +
                               ",'" + high + "'" + Environment.NewLine +
                               ",'" + close + "'" + Environment.NewLine +
                               ",'" + prev + "'" + Environment.NewLine +
                               ",'" + open + "'" + Environment.NewLine +
                               ",'" + low + "'" + Environment.NewLine +
                               ",'" + high + "'" + Environment.NewLine +
                               ",'" + close + "'" + Environment.NewLine +
                               ",'" + prev + "'" + Environment.NewLine +
                               ")";
                                //Console.WriteLine("dt=" + dt.ToString("MM/dd/yyyy"));
                                try {
                                    using(SqlConnection connection = new SqlConnection(
                                       connectionString)) {
                                        SqlCommand command = new SqlCommand(sql,connection);
                                        command.Connection.Open();
                                        command.ExecuteNonQuery();
                                    }
                                } catch(Exception ex) {
                                    Console.WriteLine("ex=" + ex.Message);
                                }
                                lastSymbol = symbol;
                            }
                        }
                    }
                }
            }
            return lastSymbol;
        }

        public string file_name { get { return _FILE_NAME; } }
        private string _FILE_NAME;
        private string _ORIGINAL_FILE_NAME;

        private ManualResetEvent _doneEvent;
    }

    public class OldSymbol {
        public string old_symbol { get; set; }
        public string new_symbol { get; set; }
    }

    public static class DataTypeHelper {

        private static string RemoveSymbols(string value) {
            if(string.IsNullOrEmpty(value) == false) {
                value = value.Replace("$","").Replace("%","").Replace(",","").Replace("(","-").Replace(")","");
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

        public static uint ToUInt(string value) {
            value = RemoveSymbols(value);
            if(value.Contains(".")) {
                decimal deValue = 0;
                decimal.TryParse(value,out deValue);
                return (uint)deValue;
            } else {
                uint returnValue;
                uint.TryParse(value,out returnValue);
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

    }

}
