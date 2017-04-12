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

namespace Ecam.ConsoleApp {
    class Program {

        static void Main(string[] args) {
            using(EcamContext context = new EcamContext())
            {
                var companies = (from q in context.tra_company
                                 select q).ToList();
            }
            Console.ReadLine();
        }
 
		//private static void DownloadAirlineLogo() {
		//	using(EcamContext context = new EcamContext()) {
		//		string siteViewsPath = ConfigurationManager.AppSettings["site_views_path"];
		//		string ecamUrl = ConfigurationManager.AppSettings["ecam_url"];
		//		string logoPath = Path.Combine(siteViewsPath,"files\\airline_logo");

		//		Common.Log("SiteViewsPath=" + siteViewsPath);
		//		Common.Log("logoPath=" + logoPath);
		//		List<ec_airline> airlines = (from q in context.ec_airline
		//									 select q).ToList();
		//		Common.Log("airlines=" + airlines.Count());
		//		string fileName = string.Empty;
		//		WebClient webClient = new WebClient();
		//		foreach(var airline in airlines) {
		//			string airlineFileName = string.Format("{0}.png",airline.airline_code);
		//			fileName = string.Format("{0}/files/airline_logo/{1}",ecamUrl,airlineFileName);
		//			string destFileName = Path.Combine(logoPath,airlineFileName);
		//			if(File.Exists(destFileName) == false) {
		//				try {
		//					webClient.DownloadFile(fileName,destFileName);
		//					Console.WriteLine("Airline Download=" + airline.airline_code);
		//				} catch { }
		//			} else {
		//				Console.WriteLine("Aleready Exist=" + airline.airline_code);
		//			}

		//			airlineFileName = string.Format("{0}_icon.png",airline.airline_code);
		//			fileName = string.Format("{0}/files/airline_logo/{1}",ecamUrl,airlineFileName);
		//			destFileName = Path.Combine(logoPath,airlineFileName);
		//			if(File.Exists(destFileName) == false) {
		//				try {
		//					webClient.DownloadFile(fileName,destFileName);
		//					Console.WriteLine("Airline Download=" + airline.airline_code);
		//				} catch { }
		//			} else {
		//				Console.WriteLine("Aleready Exist=" + airline.airline_code);
		//			}

		//			airlineFileName = string.Format("{0}_medium.png",airline.airline_code);
		//			fileName = string.Format("{0}/files/airline_logo/{1}",ecamUrl,airlineFileName);
		//			destFileName = Path.Combine(logoPath,airlineFileName);
		//			if(File.Exists(destFileName) == false) {
		//				try {
		//					webClient.DownloadFile(fileName,destFileName);
		//					Console.WriteLine("Airline Download=" + airline.airline_code);
		//				} catch { }
		//			} else {
		//				Console.WriteLine("Aleready Exist=" + airline.airline_code);
		//			}
		//		}
		//	}
		//}
    }
}
