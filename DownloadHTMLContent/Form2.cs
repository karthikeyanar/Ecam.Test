﻿using Ecam.Framework;
using Ecam.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloadHTMLContent
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private List<tra_company> _companies;
        private tra_company _lastCompany = null;
        private int _index = -1;
        //private List<int> _Years = new List<int> { 2017, 2016 };
        //private int _YearIndex = -1;
        //private int _MonthIndex = 0;
        //private bool _IsYearWise = false;
        private void Form2_Load(object sender, EventArgs e)
        {
            //string IsYearWise = System.Configuration.ConfigurationManager.AppSettings["IsYearWise"];
            //if (IsYearWise == "true")
            //{
            //    _IsYearWise = true;
            //}
            using (EcamContext context = new EcamContext())
            {
                string symbolsAppSetting = System.Configuration.ConfigurationManager.AppSettings["symbols"];
                IQueryable<tra_company> query = context.tra_company;
                if (string.IsNullOrEmpty(symbolsAppSetting) == false)
                {
                    List<string> symbols = Helper.ConvertStringList(symbolsAppSetting);
                    query = (from q in query where symbols.Contains(q.symbol) == true select q);
                }
                _companies = (from q in query
                              select q).ToList();
            }
            //if (_IsYearWise)
            //{
            //    DownloadHTMLCompanies_YearWise();
            //}
            //else
            //{
            DownloadHTMLCompanies();
            //}
        }

        //private void ProcessHTMLFiles()
        //{
        //    string dirPath = System.Configuration.ConfigurationManager.AppSettings["DownloadHTMLPath"];
        //    string backUpDirPath = System.Configuration.ConfigurationManager.AppSettings["DownloadBackUpHTMLPath"];
        //    if (System.IO.Directory.Exists(dirPath) == false)
        //    {
        //        System.IO.Directory.CreateDirectory(dirPath);
        //    }
        //    if (System.IO.Directory.Exists(backUpDirPath) == false)
        //    {
        //        System.IO.Directory.CreateDirectory(backUpDirPath);
        //    }
        //    string[] directories = System.IO.Directory.GetDirectories(dirPath);
        //    foreach (string dirctoryPath in directories)
        //    {
        //        DirectoryInfo dirInfo = new DirectoryInfo(dirctoryPath);
        //        string[] files = System.IO.Directory.GetFiles(dirctoryPath);
        //        foreach (string fullFileName in files)
        //        {
        //            string backUpDirectory = backUpDirPath + "\\" + dirInfo.Name;
        //            if (System.IO.Directory.Exists(backUpDirectory) == false)
        //            {
        //                System.IO.Directory.CreateDirectory(backUpDirectory);
        //            }
        //            string fileName = System.IO.Path.GetFileName(fullFileName);
        //            string backupFileName = backUpDirectory + "\\" + fileName;
        //            string symbol = fileName.Replace(".html", "");
        //            string html = System.IO.File.ReadAllText(fullFileName);
        //            System.IO.File.Copy(fullFileName, backupFileName, true);
        //            lblError.Text = "Prcess html symbol=" + symbol;
        //            NSEIndiaImport(symbol, html);
        //            System.IO.File.Delete(fullFileName);
        //        }
        //    }
        //}

        private void DownloadHTMLCompanies()
        {
            _index += 1;
            if (_companies != null)
            {
                if (_index < _companies.Count)
                {
                    var company = _companies[_index];
                    _lastCompany = company;

                    lblCompanyStatus.Text = "Companies " + _index + " Of " + _companies.Count();
                    if (_lastCompany != null)
                    {

                        string dirPath = System.Configuration.ConfigurationManager.AppSettings["DownloadHTMLPath"];
                        if (System.IO.Directory.Exists(dirPath) == false)
                        {
                            System.IO.Directory.CreateDirectory(dirPath);
                        }
                        string fullFileName = dirPath + "\\" + _lastCompany.symbol + ".html";
                        string html = string.Empty;
                        //Helper.Log(fullFileName);
                        if (System.IO.File.Exists(fullFileName) == true)
                        {
                            html = System.IO.File.ReadAllText(fullFileName);
                            if (html.Contains("Make sure the web address") == true)
                            {
                                Helper.Log(fullFileName, "MakeSure");
                                System.IO.File.Delete(fullFileName);
                            }
                            if (html.Contains("Service Unavailable") == true)
                            {
                                Helper.Log(fullFileName, "ServiceUnavailable");
                                System.IO.File.Delete(fullFileName);
                            }
                            if (html.Contains("What you can try") == true)
                            {
                                Helper.Log(fullFileName, "Whatyoucantry");
                                System.IO.File.Delete(fullFileName);
                            }
                        }
                        if (System.IO.File.Exists(fullFileName) == false)
                        {
                            lblCompany.Text = _lastCompany.company_name;
                            lblSymbol.Text = _lastCompany.symbol;
                            DateTime startDate = DateTime.Now.Date.AddDays(-DataTypeHelper.ToInt32(System.Configuration.ConfigurationManager.AppSettings["no_of_days"].ToString()));
                            DateTime endDate = DateTime.Now.Date;
                            string url = string.Format("https://nseindia.com/products/dynaContent/common/productsSymbolMapping.jsp?symbol={0}&segmentLink=3&symbolCount=2&series=EQ&dateRange=+&fromDate={1}&toDate={2}&dataType=PRICEVOLUME"
                                                                                                              , _lastCompany.symbol.Replace("&", "%26")
                                                                                                              , startDate.ToString("dd-MM-yyyy")
                                                                                                              , endDate.ToString("dd-MM-yyyy")
                                                                                                              );
                            lblStartDate.Text = startDate.ToString("dd/MMM/yyyy");
                            lblEndDate.Text = endDate.ToString("dd/MMM/yyyy");
                            //_lastURL = url;
                            //Helper.Log(url, "NAVIGATE");
                            webBrowser1.Navigate(url);
                        }
                        else
                        {
                            TradeHelper.NSEIndiaImport(html);
                            DownloadHTMLCompanies();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Completed");
                }
            }
        }

        //private void DownloadHTMLCompanies_YearWise()
        //{
        //    _YearIndex = -1;
        //    _MonthIndex = 0;
        //    _index += 1;
        //    if (_companies != null)
        //    {
        //        if (_index < _companies.Count)
        //        {
        //            var company = _companies[_index];
        //            _lastCompany = company;
        //            lblCompanyStatus.Text = "Total Companies " + (_index + 1) + " Of " + _companies.Count();
        //            Helper.Log(lblCompanyStatus.Text);
        //            lblSymbol.Text = _lastCompany.symbol;
        //            lblCompany.Text = _lastCompany.company_name;
        //            DownloadHTMLCompanies_NexYear();
        //        }
        //        else
        //        {
        //            Helper.Log("Completed");
        //            MessageBox.Show("Completed");
        //        }
        //    }
        //}

        //private void DownloadHTMLCompanies_NexYear()
        //{
        //    _YearIndex += 1;
        //    if (_YearIndex < _Years.Count())
        //    {
        //        int year = _Years[_YearIndex];
        //        DownloadHTMLCompanies_MonthWise();
        //    }
        //    else
        //    {
        //        lblError.Text = "All years completed symbol=" + _lastCompany.symbol;
        //        DownloadHTMLCompanies_YearWise();
        //    }
        //}

        //private DateTime _STARTDATE;
        //private DateTime _ENDDATE;
        //private void DownloadHTMLCompanies_MonthWise()
        //{

        //    _MonthIndex += 1;
        //    if (_YearIndex <= _Years.Count())
        //    {
        //        int year = _Years[_YearIndex];
        //        if (_MonthIndex <= 12)
        //        {
        //            DateTime dt = Convert.ToDateTime(_MonthIndex + "/01/" + year);
        //            DateTime startDate = DataTypeHelper.GetFirstDayOfMonth(dt);
        //            DateTime endDate = DataTypeHelper.GetLastDayOfMonth(dt);
        //            _STARTDATE = startDate;
        //            _ENDDATE = endDate;
        //            lblStartDate.Text = startDate.ToString("dd/MMM/yyyy");
        //            lblEndDate.Text = endDate.ToString("dd/MMM/yyyy");
        //            //Helper.Log(lblStartDate.Text + " TO " + lblEndDate.Text);
        //            DownloadHTMLCompanies_MonthWise(startDate, endDate);
        //        }
        //        else
        //        {
        //            _MonthIndex = 0;
        //            DownloadHTMLCompanies_NexYear();
        //        }
        //    }

        //}

        //private string _lastURL = string.Empty;
        //private void DownloadHTMLCompanies_MonthWise(DateTime startDate, DateTime endDate)
        //{
        //    if (_lastCompany != null)
        //    {

        //        string dirPath = System.Configuration.ConfigurationManager.AppSettings["DownloadHTMLPath"];

        //        dirPath += "\\" + _STARTDATE.ToString("dd_MMM_yyyy");

        //        if (System.IO.Directory.Exists(dirPath) == false)
        //        {
        //            System.IO.Directory.CreateDirectory(dirPath);
        //        }
        //        string fullFileName = dirPath + "\\" + _lastCompany.symbol + ".html";
        //        //Helper.Log(fullFileName);
        //        if (System.IO.File.Exists(fullFileName) == true)
        //        {
        //            string html = System.IO.File.ReadAllText(fullFileName);
        //            if (html.Contains("Make sure the web address") == true)
        //            {
        //                Helper.Log(fullFileName, "MakeSure");
        //                System.IO.File.Delete(fullFileName);
        //            }
        //            if (html.Contains("Service Unavailable") == true)
        //            {
        //                Helper.Log(fullFileName, "ServiceUnavailable");
        //                System.IO.File.Delete(fullFileName);
        //            }
        //            if (html.Contains("What you can try") == true)
        //            {
        //                Helper.Log(fullFileName, "Whatyoucantry");
        //                System.IO.File.Delete(fullFileName);
        //            }
        //        }
        //        if (System.IO.File.Exists(fullFileName) == false)
        //        {
        //            string url = string.Format("https://nseindia.com/products/dynaContent/common/productsSymbolMapping.jsp?symbol={0}&segmentLink=3&symbolCount=2&series=EQ&dateRange=+&fromDate={1}&toDate={2}&dataType=PRICEVOLUME"
        //                                                                                              , _lastCompany.symbol.Replace("&", "%26")
        //                                                                                              , startDate.ToString("dd-MM-yyyy")
        //                                                                                              , endDate.ToString("dd-MM-yyyy")
        //                                                                                              );

        //            _lastURL = url;
        //            Helper.Log(url, "NAVIGATE");
        //            webBrowser1.Navigate(url);
        //        }
        //        else
        //        {
        //            DownloadHTMLCompanies_MonthWise();
        //        }

        //    }
        //}

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string html = webBrowser1.DocumentText;
            string dirPath = System.Configuration.ConfigurationManager.AppSettings["DownloadHTMLPath"];
            //if (_IsYearWise)
            //{
            //    dirPath += "\\" + _STARTDATE.ToString("dd_MMM_yyyy");
            //}
            if (System.IO.Directory.Exists(dirPath) == false)
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }
            bool isIgnore = false;
            if (html.Contains("Make sure the web address") == true)
            {
                isIgnore = true;
            }
            if (html.Contains("Service Unavailable") == true)
            {
                isIgnore = true;
            }
            if (html.Contains("What you can try") == true)
            {
                isIgnore = true;
            }
            if (html.Contains("No Records") == true)
            {
                isIgnore = true;
            }
            if (isIgnore == false)
            {
                string fullFileName = dirPath + "\\" + _lastCompany.symbol + ".html";
                if (System.IO.File.Exists(fullFileName) == true)
                {
                    System.IO.File.Delete(fullFileName);
                }
                System.IO.File.WriteAllText(fullFileName, html);
                //Helper.Log("New Symbol=" + _lastCompany.symbol + ",fullFileName=" + fullFileName, "NEWCOMPANY");
                //if (_IsYearWise == false)
                //{
                TradeHelper.NSEIndiaImport(html);
                //}
            }
            //
            //if (_IsYearWise)
            //{
            //    DownloadHTMLCompanies_MonthWise();
            //}
            //else
            //{
            DownloadHTMLCompanies();
            //}
        }
    }
}
