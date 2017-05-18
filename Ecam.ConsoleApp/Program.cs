using Ecam.Framework;
using Ecam.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Ecam.ConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            ParseMainHTML();
            Console.ReadLine();
        }

        private static void ParseMainHTML()
        {
            string fileName = "C:\\Users\\kart\\Desktop\\MF\\1.html";
            string content = System.IO.File.ReadAllText(fileName);
            string startWord = "<table width=\"100%\" class=\"tblporhd\">";
            string endWord = "<table width=\"100%\" class=\"tblporhd MT25\">";
            int startIndex = content.IndexOf(startWord);
            int endIndex = content.IndexOf(endWord);
            int length = endIndex - startIndex + endWord.Length;
            string parseContent = content.Substring(startIndex, length);
            endWord = "</table>";
            startIndex = parseContent.IndexOf(startWord);
            endIndex = parseContent.IndexOf(endWord);
            length = endIndex - startIndex + endWord.Length;
            parseContent = parseContent.Substring(startIndex, length);
            Regex regex = new Regex(
     @"<tr>(.*?)</tr>",
     RegexOptions.IgnoreCase
     | RegexOptions.Multiline
     | RegexOptions.IgnorePatternWhitespace
     | RegexOptions.Compiled
     );

            MatchCollection trCollections = regex.Matches(parseContent);
            int i = 0;
            foreach (Match trMatch in trCollections)
            {
                i += 1;
                string tr = trMatch.Value;
                string tagName = "td";
                if (i == 1)
                {
                    tagName = "th";
                }
                regex = new Regex(
                            @"<" + tagName + "[^>]*>(.+?)</" + tagName + ">",
                            RegexOptions.IgnoreCase
                            | RegexOptions.Multiline
                            | RegexOptions.IgnorePatternWhitespace
                            | RegexOptions.Compiled
                            );
                MatchCollection rowMatches = regex.Matches(tr);
                string equity = "";
                string sector = "";
                string qty = "";
                string totalValue = "";
                string percentage = "";
                
                int colIndex = -1;
                foreach (Match colMatch in rowMatches)
                {
                    colIndex += 1;
                    if (i > 1)
                    {
                        string value = string.Empty;
                        if (colMatch.Groups.Count >= 2)
                        {
                            value = colMatch.Groups[1].Value;
                        }
                        if (string.IsNullOrEmpty(value) == false)
                        {
                            value = value.Trim();
                        }
                        switch (colIndex)
                        {
                            case 0: equity = value; break;
                            case 1: sector = value; break;
                            case 2: qty = value; break;
                            case 3: totalValue = value; break;
                            case 4: percentage = value; break;
                        }
                    }
                } 
                if (string.IsNullOrEmpty(equity) == false)
                {
                    string symbol = GetSymbol(equity);
                }
            }
        }

        private static string GetSymbol(string equity)
        {
            string symbol = "";
            Regex regex = new Regex(
    @"href=[""|'](.*?)[""|']",
    RegexOptions.IgnoreCase
    | RegexOptions.Multiline
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled
    );
            MatchCollection collections = regex.Matches(equity);
            if (collections.Count > 0)
            {
                string href = collections[0].Groups[1].Value;
            }
            return symbol;
        }
    }
}
