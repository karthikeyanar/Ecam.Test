using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecam.Views {
	public static class CurrencyHelper {

		// # C# Dictionary ISO { countrycode, countryname } list
		//{ "BN", "Brunei Darussalam" }, 
		//{ "TL", "Timor-Leste" }, 
		public static Dictionary<string, string> Countries = new Dictionary<string, string>() { 
        { "AD", "Andorra" }, 
        { "AE", "United Arab Emirates" }, 
        { "AF", "Afghanistan" }, 
        { "AG", "Antigua and Barbuda" }, 
        { "AI", "Anguilla" }, 
        { "AL", "Albania" }, 
        { "AM", "Armenia" }, 
        { "AN", "Netherlands Antilles" }, 
        { "AO", "Angola" }, 
        { "AQ", "Antarctica" }, 
        { "AR", "Argentina" }, 
        { "AS", "American Samoa" }, 
        { "AT", "Austria" }, { "AU", "Australia" }, 
        { "AW", "Aruba" }, { "AX", "Åland Islands" }, { "AZ", "Azerbaijan" }, { "BA", "Bosnia and Herzegovina" }, { "BB", "Barbados" }, 
		{ "BD", "Bangladesh" }, { "BE", "Belgium" }, { "BF", "Burkina Faso" }, { "BG", "Bulgaria" }, { "BH", "Bahrain" }, { "BI", "Burundi" },
		{ "BJ", "Benin" }, { "BL", "Saint Barthélemy" }, { "BM", "Bermuda" }, { "BN", "Brunei" }, { "BO", "Bolivia, Plurinational State of" },
		{ "BQ", "Bonaire, Sint Eustatius and Saba" }, { "BR", "Brazil" }, { "BS", "Bahamas" },
		{ "BT", "Bhutan" }, { "BV", "Bouvet Island" },
		{ "BW", "Botswana" }, { "BY", "Belarus" }, { "BZ", "Belize" }, { "CA", "Canada" }, 
		{ "CC", "Cocos (Keeling) Islands" }, { "CD", "Congo, the Democratic Republic of the" }, 
		{ "CF", "Central African Republic" }, { "CG", "Congo" }, { "CH", "Switzerland" },
		{ "CI", "Côte d'Ivoire" }, { "CK", "Cook Islands" }, { "CL", "Chile" },
		{ "CM", "Cameroon" }, { "CN", "China" }, { "CO", "Colombia" }, { "CR", "Costa Rica" },
		{ "CS", "Czechoslovak Socialist Republic" }, { "CU", "Cuba" }, { "CV", "Cape Verde" }, 
		{ "CW", "Curaçao" }, { "CX", "Christmas Island" }, { "CY", "Cyprus" }, { "CZ", "Czech Republic" }, 
		{ "DD", "German Democratic Republic" }, { "DE", "Germany" }, { "DJ", "Djibouti" },
		{ "DK", "Denmark" }, { "DM", "Dominica" }, { "DO", "Dominican Republic" },
		{ "DZ", "Algeria" }, { "TL", "East Timor" }, { "EC", "Ecuador" }, { "EE", "Estonia" }, { "EG", "Egypt" },
		{ "EH", "Western Sahara" }, { "ER", "Eritrea" }, { "ES", "Spain" }, { "ET", "Ethiopia" }, { "FI", "Finland" },
		{ "FJ", "Fiji" }, { "FK", "Falkland Islands (Malvinas)" }, { "FM", "Micronesia, Federated States of" },
		{ "FO", "Faroe Islands" }, { "FR", "France" }, { "GA", "Gabon" }, { "GB", "United Kingdom" }, { "GD", "Grenada" },
		{ "GE", "Georgia" }, { "GF", "French Guiana" }, { "GG", "Guernsey" }, { "GH", "Ghana" }, { "GI", "Gibraltar" }, 
		{ "GL", "Greenland" }, { "GM", "Gambia" }, { "GN", "Guinea" }, { "GP", "Guadeloupe" }, { "GQ", "Equatorial Guinea" }, { "GR", "Greece" },
		{ "GS", "South Georgia and the South Sandwich Islands" }, { "GT", "Guatemala" }, { "GU", "Guam" }, { "GW", "Guinea-Bissau" }, { "GY", "Guyana" }, { "HK", "Hong Kong" }, { "HM", "Heard Island and McDonald Islands" }, { "HN", "Honduras" }, { "HR", "Croatia" }, { "HT", "Haiti" }, { "HU", "Hungary" }, { "ID", "Indonesia" }, { "IE", "Ireland" }, { "IL", "Israel" }, { "IM", "Isle of Man" }, { "IN", "India" }, { "IO", "British Indian Ocean Territory" }, { "IQ", "Iraq" }, { "IR", "Iran, Islamic Republic of" }, { "IS", "Iceland" }, { "IT", "Italy" }, { "JE", "Jersey" }, { "JM", "Jamaica" }, { "JO", "Jordan" }, { "JP", "Japan" }, { "KE", "Kenya" }, { "KG", "Kyrgyzstan" }, { "KH", "Cambodia" }, { "KI", "Kiribati" }, { "KM", "Comoros" }, { "KN", "Saint Kitts and Nevis" }, 
        { "KP", "North Korea" }, 
        { "KR", "South Korea" }, 
        { "KW", "Kuwait" }, { "KY", "Cayman Islands" }, { "KZ", "Kazakhstan" }, 
        { "LA", "Laos" }, 
        { "LB", "Lebanon" }, { "LC", "Saint Lucia" }, { "LI", "Liechtenstein" }, { "LK", "Sri Lanka" }, { "LR", "Liberia" }, { "LS", "Lesotho" }, { "LT", "Lithuania" }, { "LU", "Luxembourg" }, { "LV", "Latvia" }, { "LY", "Libya" }, { "MA", "Morocco" }, { "MC", "Monaco" }, { "MD", "Moldova, Republic of" }, { "ME", "Montenegro" }, { "MF", "Saint Martin (French part)" }, { "MG", "Madagascar" }, { "MH", "Marshall Islands" }, { "MK", "Macedonia, The Former Yugoslav Republic of" }, { "ML", "Mali" }, { "MM", "Myanmar" }, { "MN", "Mongolia" }, { "MO", "Macao" }, { "MP", "Northern Mariana Islands" }, { "MQ", "Martinique" }, { "MR", "Mauritania" }, { "MS", "Montserrat" }, { "MT", "Malta" }, { "MU", "Mauritius" }, { "MV", "Maldives" }, { "MW", "Malawi" }, { "MX", "Mexico" }, { "MY", "Malaysia" }, { "MZ", "Mozambique" }, { "NA", "Namibia" }, { "NC", "New Caledonia" }, { "NE", "Niger" }, { "NF", "Norfolk Island" }, { "NG", "Nigeria" }, { "NI", "Nicaragua" }, { "NL", "Netherlands" }, { "NO", "Norway" }, { "NP", "Nepal" }, { "NR", "Nauru" }, { "NU", "Niue" }, { "NZ", "New Zealand" }, { "OM", "Oman" }, { "PA", "Panama" }, { "PE", "Peru" }, { "PF", "French Polynesia" }, { "PG", "Papua New Guinea" }, { "PH", "Philippines" }, { "PK", "Pakistan" }, { "PL", "Poland" }, { "PM", "Saint Pierre and Miquelon" }, { "PN", "Pitcairn" }, { "PR", "Puerto Rico" }, { "PS", "Palestinian Territory, Occupied" }, { "PT", "Portugal" }, { "PW", "Palau" }, { "PY", "Paraguay" }, { "QA", "Qatar" }, { "RE", "Réunion" }, { "RO", "Romania" }, { "RS", "Serbia" }, { "RU", "Russian Federation" }, { "RW", "Rwanda" }, { "SA", "Saudi Arabia" }, { "SB", "Solomon Islands" }, { "SC", "Seychelles" }, { "SD", "Sudan" }, { "SE", "Sweden" }, { "SG", "Singapore" }, { "SH", "Saint Helena, Ascension and Tristan da Cunha" }, { "SI", "Slovenia" }, { "SJ", "Svalbard and Jan Mayen" }, { "SK", "Slovakia" }, { "SL", "Sierra Leone" }, { "SM", "San Marino" }, { "SN", "Senegal" }, { "SO", "Somalia" }, { "SR", "Suriname" }, { "SS", "South Sudan" }, { "ST", "Sao Tome and Principe" }, { "SU", "U.S.S.R." }, { "SV", "El Salvador" }, { "SX", "Sint Maarten (Dutch part)" }, { "SY", "Syrian Arab Republic" }, { "SZ", "Swaziland" }, { "TC", "Turks and Caicos Islands" }, { "TD", "Chad" }, { "TF", "French Southern Territories" }, { "TG", "Togo" }, { "TH", "Thailand" }, { "TJ", "Tajikistan" }, { "TK", "Tokelau" }, { "TM", "Turkmenistan" }, { "TN", "Tunisia" }, { "TO", "Tonga" }, { "TR", "Turkey" }, { "TT", "Trinidad and Tobago" }, { "TV", "Tuvalu" }, 
        { "TW", "Taiwan" }, 
        { "TZ", "Tanzania, United Republic of" }, { "UA", "Ukraine" }, { "UG", "Uganda" }, { "UM", "United States Minor Outlying Islands" }, { "US", "United States" }, { "UY", "Uruguay" }, { "UZ", "Uzbekistan" }, { "VA", "Holy See (Vatican City State)" }, { "VC", "Saint Vincent and the Grenadines" }, { "VE", "Venezuela, Bolivarian Republic of" }, { "VG", "Virgin Islands, British" }, { "VI", "Virgin Islands, U.S." }, 
        { "VN", "Vietnam" }, { "VU", "Vanuatu" }, { "WF", "Wallis and Futuna" }, { "WS", "Samoa" }, { "YD", "People's Democratic Republic of Yemen" }, { "YE", "Yemen" }, { "YT", "Mayotte" }, { "YU", "Yugoslavia" }, { "ZA", "South Africa" }, { "ZM", "Zambia" }, { "ZR", "Zaire" }, { "ZW", "Zimbabwe" } };
		//# C# Dictionary ISO {countrycode, currencycode} list
		public static Dictionary<string, string> CountryCodeAndCurrencies = new Dictionary<string, string>() { 
		{ "AF", "AFN" }, 
		{ "AL", "ALL" }, 
		{ "DZ", "DZD" }, 
		{ "AS", "USD" },
		{ "AD", "EUR" }, 
		{ "AO", "AOA" }, 
		{ "AI", "XCD" },
		{ "AG", "XCD" }, { "AR", "ARP" }, { "AM", "AMD" }, { "AW", "AWG" }, { "AU", "AUD" },
		{ "AT", "EUR" }, { "AZ", "AZN" }, { "BS", "BSD" }, { "BH", "BHD" }, { "BD", "BDT" }, 
		{ "BB", "BBD" }, { "BY", "BYR" }, { "BE", "EUR" }, { "BZ", "BZD" }, { "BJ", "XOF" }, 
		{ "BM", "BMD" }, { "BT", "BTN" }, { "BO", "BOV" }, { "BA", "BAM" }, { "BW", "BWP" },
		{ "BV", "NOK" }, { "BR", "BRL" }, { "IO", "USD" }, { "BN", "BND" }, { "BG", "BGL" }, 
		{ "BF", "XOF" }, { "BI", "BIF" }, { "KH", "KHR" }, { "CM", "XAF" }, { "CA", "CAD" }, 
		{ "CV", "CVE" }, { "KY", "KYD" }, { "CF", "XAF" }, { "TD", "XAF" }, { "CL", "CLF" },
		{ "CN", "CNY" }, { "CX", "AUD" }, { "CC", "AUD" }, { "CO", "COU" }, { "KM", "KMF" },
		{ "CG", "XAF" }, { "CD", "CDF" }, { "CK", "NZD" }, { "CR", "CRC" }, { "HR", "HRK" }, 
		{ "CU", "CUP" }, { "CY", "EUR" }, { "CZ", "CZK" }, { "CS", "CSJ" }, { "CI", "XOF" },
		{ "DK", "DKK" }, { "DJ", "DJF" }, { "DM", "XCD" }, { "DO", "DOP" }, { "EC", "USD" }, 
		{ "EG", "EGP" }, { "SV", "USD" }, { "GQ", "EQE" }, { "ER", "ERN" }, { "EE", "EEK" }, 
		{ "ET", "ETB" }, { "FK", "FKP" }, { "FO", "DKK" }, { "FJ", "FJD" }, { "FI", "FIM" }, 
		{ "FR", "XFO" }, { "GF", "EUR" }, { "PF", "XPF" }, { "TF", "EUR" }, { "GA", "XAF" }, 
		{ "GM", "GMD" }, { "GE", "GEL" }, { "DD", "DDM" }, { "DE", "EUR" }, { "GH", "GHC" }, 
		{ "GI", "GIP" }, { "GR", "GRD" }, { "GL", "DKK" }, { "GD", "XCD" }, { "GP", "EUR" },
		{ "GU", "USD" }, { "GT", "GTQ" }, { "GN", "GNE" }, { "GW", "GWP" }, { "GY", "GYD" }, 
		{ "HT", "USD" }, { "HM", "AUD" }, { "VA", "EUR" }, { "HN", "HNL" }, { "HK", "HKD" },
		{ "HU", "HUF" }, { "IS", "ISJ" }, { "IN", "INR" }, { "ID", "IDR" }, { "IR", "IRR" }, 
		{ "IQ", "IQD" }, { "IE", "IEP" }, { "IL", "ILS" }, { "IT", "ITL" }, { "JM", "JMD" },
		{ "JP", "JPY" }, { "JO", "JOD" }, { "KZ", "KZT" }, { "KE", "KES" }, { "KI", "AUD" }, 
		{ "KP", "KPW" }, { "KR", "KRW" }, { "KW", "KWD" }, { "KG", "KGS" }, { "LA", "LAJ" }, 
		{ "LV", "LVL" }, { "LB", "LBP" }, { "LS", "ZAR" }, { "LR", "LRD" }, { "LY", "LYD" },
		{ "LI", "CHF" }, { "LT", "LTL" }, { "LU", "LUF" }, { "MO", "MOP" }, { "MK", "MKN" }, 
		{ "MG", "MGF" }, { "MW", "MWK" }, { "MY", "MYR" }, { "MV", "MVR" }, { "ML", "MAF" }, 
		{ "MT", "MTL" }, { "MH", "USD" }, { "MQ", "EUR" }, { "MR", "MRO" }, { "MU", "MUR" }, 
		{ "YT", "EUR" }, { "MX", "MXV" }, { "FM", "USD" }, { "MD", "MDL" }, { "MC", "MCF" }, 
		{ "MN", "MNT" }, { "ME", "EUR" }, { "MS", "XCD" }, { "MA", "MAD" }, { "MZ", "MZM" }, 
		{ "MM", "MMK" }, { "NA", "ZAR" }, { "NR", "AUD" }, { "NP", "NPR" }, { "NL", "NLG" },
		{ "AN", "ANG" }, { "NC", "XPF" }, { "NZ", "NZD" }, { "NI", "NIO" }, { "NE", "XOF" }, 
		{ "NG", "NGN" }, { "NU", "NZD" }, { "NF", "AUD" }, { "MP", "USD" }, { "NO", "NOK" }, 
		{ "OM", "OMR" }, { "PK", "PKR" }, { "PW", "USD" }, { "PA", "USD" }, { "PG", "PGK" }, 
		{ "PY", "PYG" }, { "YD", "YDD" }, { "PE", "PEH" }, { "PH", "PHP" }, { "PN", "NZD" }, 
		{ "PL", "PLN" }, { "PT", "TPE" }, { "PR", "USD" }, { "QA", "QAR" }, { "RO", "ROK" }, 
		{ "RU", "RUB" }, { "RW", "RWF" }, { "RE", "EUR" }, { "SH", "SHP" }, { "KN", "XCD" }, 
		{ "LC", "XCD" }, { "PM", "EUR" }, { "VC", "XCD" }, { "WS", "WST" }, { "SM", "EUR" }, 
		{ "ST", "STD" }, { "SA", "SAR" }, { "SN", "XOF" }, { "RS", "CSD" }, { "SC", "SCR" }, 
		{ "SL", "SLL" }, { "SG", "SGD" }, { "SK", "SKK" }, { "SI", "SIT" }, { "SB", "SBD" },
		{ "SO", "SOS" }, { "ZA", "ZAL" }, { "ES", "ESB" }, { "LK", "LKR" }, { "SD", "SDG" },
		{ "SR", "SRG" }, { "SJ", "NOK" }, { "SZ", "SZL" }, { "SE", "SEK" }, { "CH", "CHW" }, 
		{ "SY", "SYP" }, { "TW", "TWD" }, { "TJ", "TJR" }, { "TZ", "TZS" }, { "TH", "THB" }, 
		{ "TL", "USD" }, { "TG", "XOF" }, { "TK", "NZD" }, { "TO", "TOP" }, { "TT", "TTD" }, 
		{ "TN", "TND" }, { "TR", "TRL" }, { "TM", "TMM" }, { "TC", "USD" }, { "TV", "AUD" }, 
		{ "SU", "SUR" }, { "UG", "UGS" }, { "UA", "UAK" }, { "AE", "AED" }, { "GB", "GBP" }
            , { "US", "USS" }
            , { "UM", "USD" }, { "UY", "UYI" }, { "UZ", "UZS" }, { "VU", "VUV" }, { "VE", "VEB" }, 
			{ "VN", "VNC" }, { "VG", "USD" }, { "VI", "USD" }, { "WF", "XPF" }, { "EH", "MAD" }, 
			{ "YE", "YER" }, { "YU", "YUM" }, { "ZR", "ZRZ" }, { "ZM", "ZMK" }, { "ZW", "ZWC" } };

		//# C# Dictionary ISO {currencycode, currencyname} list
		public static Dictionary<string, string> CurrencyCodeAndCurrencyNames = new Dictionary<string, string>() { 
		{ "AFA", "Afghani" }, { "AFN", "Afghani" }, { "ALK", "Albanian old lek" }, { "ALL", "Lek" }, { "DZD", "Algerian Dinar" }, 
		{ "USD", "US Dollar" }, { "ADF", "Andorran Franc (1:1 peg to the french franc)" }, { "ADP", "Andorran Peseta (1:1 peg to the Spanish Peseta)" }, 
		{ "EUR", "Euro" }, { "AOR", "Angolan Kwanza Readjustado" }, 
		{ "AON", "Angolan New Kwanza" },
		{ "AOA", "Kwanza" }, { "XCD", "East Caribbean Dollar" }, { "ARA", "Argentine austral" }, { "ARS", "Argentine Peso" }, 
		{ "ARL", "Argentine peso ley" }, 
		{ "ARM", "Argentine peso moneda nacional" }, { "ARP", "Peso argentino" }, { "AMD", "Armenian Dram" }, { "AWG", "Aruban Guilder" }, 
		{ "AUD", "Australian Dollar" }, { "ATS", "Austrian Schilling" }, { "AZM", "Azerbaijani manat" }, { "AZN", "Azerbaijanian Manat" }, 
		{ "BSD", "Bahamian Dollar" }, { "BHD", "Bahraini Dinar" }, { "BDT", "Taka" }, { "BBD", "Barbados Dollar" }, { "BYR", "Belarussian Ruble" }, 
		{ "BEC", "Belgian Franc (convertible)" }, { "BEF", "Belgian Franc (currency union with LUF)" }, { "BEL", "Belgian Franc (financial)" }, 
		{ "BZD", "Belize Dollar" }, { "XOF", "CFA Franc BCEAO" }, { "BMD", "Bermudian Dollar" }, { "INR", "Indian Rupee" }, { "BTN", "Ngultrum" }, 
		{ "BOP", "Bolivian peso" }, { "BOB", "Boliviano" }, { "BOV", "Mvdol" }, { "BAM", "Convertible Marks" }, { "BWP", "Pula" }, 
		{ "NOK", "Norwegian Krone" }, { "BRC", "Brazilian cruzado" }, { "BRB", "Brazilian cruzeiro" }, { "BRL", "Brazilian Real" }, 
		{ "BND", "Brunei Dollar" }, { "BGN", "Bulgarian Lev" }, { "BGJ", "Bulgarian lev A/52" }, { "BGK", "Bulgarian lev A/62" }, { "BGL", "Bulgarian lev A/99" }, 
		{ "BIF", "Burundi Franc" }, { "KHR", "Riel" }, { "XAF", "CFA Franc BEAC" }, { "CAD", "Canadian Dollar" }, { "CVE", "Cape Verde Escudo" }, 
		{ "KYD", "Cayman Islands Dollar" }, { "CLP", "Chilean Peso" }, { "CLF", "Unidades de fomento" }, { "CNX", "Chinese People's Bank dollar" },
		{ "CNY", "Yuan Renminbi" }, { "COP", "Colombian Peso" }, { "COU", "Unidad de Valor real" }, { "KMF", "Comoro Franc" }, { "CDF", "Franc Congolais" },
		{ "NZD", "New Zealand Dollar" }, { "CRC", "Costa Rican Colon" }, { "HRK", "Croatian Kuna" }, { "CUP", "Cuban Peso" }, { "CYP", "Cyprus Pound" },
		{ "CZK", "Czech Koruna" }, { "CSK", "Czechoslovak koruna" }, { "CSJ", "Czechoslovak koruna A/53" },
		{ "DKK", "Danish Krone" }, 
		{ "DJF", "Djibouti Franc" }, { "DOP", "Dominican Peso" }, { "ECS", "Ecuador sucre" }, { "EGP", "Egyptian Pound" },
		{ "SVC", "Salvadoran colón" }, { "EQE", "Equatorial Guinean ekwele" }, { "ERN", "Nakfa" }, { "EEK", "Kroon" }, 
		{ "ETB", "Ethiopian Birr" }, { "FKP", "Falkland Island Pound" }, { "FJD", "Fiji Dollar" }, { "FIM", "Finnish Markka" },
		{ "FRF", "French Franc" }, { "XFO", "Gold-Franc" }, { "XPF", "CFP Franc" }, { "GMD", "Dalasi" }, 
		{ "GEL", "Lari" }, { "DDM", "East German Mark of the GDR (East Germany)" }, { "DEM", "Deutsche Mark" }, 
		{ "GHS", "Ghana Cedi" }, { "GHC", "Ghanaian cedi" }, { "GIP", "Gibraltar Pound" }, { "GRD", "Greek Drachma" }, 
		{ "GTQ", "Quetzal" }, { "GNF", "Guinea Franc" }, { "GNE", "Guinean syli" }, { "GWP", "Guinea-Bissau Peso" },
		{ "GYD", "Guyana Dollar" }, { "HTG", "Gourde" }, { "HNL", "Lempira" }, { "HKD", "Hong Kong Dollar" },
		{ "HUF", "Forint" }, { "ISK", "Iceland Krona" }, { "ISJ", "Icelandic old krona" }, { "IDR", "Rupiah" },
		{ "IRR", "Iranian Rial" }, { "IQD", "Iraqi Dinar" }, { "IEP", "Irish Pound (Punt in Irish language)" },
		{ "ILP", "Israeli lira" }, { "ILR", "Israeli old sheqel" }, { "ILS", "New Israeli Sheqel" },
		{ "ITL", "Italian Lira" }, { "JMD", "Jamaican Dollar" }, { "JPY", "Yen" }, { "JOD", "Jordanian Dinar" },
		{ "KZT", "Tenge" }, { "KES", "Kenyan Shilling" }, { "KPW", "North Korean Won" }, { "KRW", "Won" },
		{ "KWD", "Kuwaiti Dinar" }, { "KGS", "Som" }, { "LAK", "Kip" }, { "LAJ", "Lao kip" },
		{ "LVL", "Latvian Lats" }, { "LBP", "Lebanese Pound" }, { "LSL", "Loti" }, { "ZAR", "Rand" }, 
		{ "LRD", "Liberian Dollar" }, { "LYD", "Libyan Dinar" }, { "CHF", "Swiss Franc" }, 
		{ "LTL", "Lithuanian Litas" }, { "LUF", "Luxembourg Franc (currency union with BEF)" }, 
		{ "MOP", "Pataca" }, { "MKD", "Denar" }, { "MKN", "Former Yugoslav Republic of Macedonia denar A/93" },
		{ "MGA", "Malagasy Ariary" }, { "MGF", "Malagasy franc" }, { "MWK", "Kwacha" },
		{ "MYR", "Malaysian Ringgit" }, { "MVQ", "Maldive rupee" }, { "MVR", "Rufiyaa" },
		{ "MAF", "Mali franc" }, { "MTL", "Maltese Lira" }, { "MRO", "Ouguiya" }, 
		{ "MUR", "Mauritius Rupee" }, { "MXN", "Mexican Peso" }, { "MXP", "Mexican peso" },
		{ "MXV", "Mexican Unidad de Inversion (UDI)" }, { "MDL", "Moldovan Leu" }, 
		{ "MCF", "Monegasque franc (currency union with FRF)" }, { "MNT", "Tugrik" }, { "MAD", "Moroccan Dirham" }, 
		{ "MZN", "Metical" }, { "MZM", "Mozambican metical" }, { "MMK", "Kyat" }, { "NAD", "Namibia Dollar" },
		{ "NPR", "Nepalese Rupee" }, { "NLG", "Netherlands Guilder" }, { "ANG", "Netherlands Antillian Guilder" },
		{ "NIO", "Cordoba Oro" }, { "NGN", "Naira" }, { "OMR", "Rial Omani" }, { "PKR", "Pakistan Rupee" }, 
		{ "PAB", "Balboa" }, { "PGK", "Kina" }, { "PYG", "Guarani" }, { "YDD", "South Yemeni dinar" }, 
		{ "PEN", "Nuevo Sol" }, { "PEI", "Peruvian inti" }, { "PEH", "Peruvian sol" }, 
		{ "PHP", "Philippine Peso" }, { "PLZ", "Polish zloty A/94" }, { "PLN", "Zloty" }, 
		{ "PTE", "Portuguese Escudo" }, { "TPE", "Portuguese Timorese escudo" }, { "QAR", "Qatari Rial" },
		{ "RON", "New Leu" }, { "ROL", "Romanian leu A/05" }, { "ROK", "Romanian leu A/52" },
		{ "RUB", "Russian Ruble" }, { "RWF", "Rwanda Franc" }, { "SHP", "Saint Helena Pound" },
		{ "WST", "Tala" }, { "STD", "Dobra" }, { "SAR", "Saudi Riyal" }, { "RSD", "Serbian Dinar" }, 
		{ "CSD", "Serbian Dinar" }, { "SCR", "Seychelles Rupee" }, { "SLL", "Leone" }, 
		{ "SGD", "Singapore Dollar" }, { "SKK", "Slovak Koruna" }, { "SIT", "Slovenian Tolar" }, 
		{ "SBD", "Solomon Islands Dollar" }, { "SOS", "Somali Shilling" }, 
		{ "ZAL", "South African financial rand (Funds code) (discont" }, { "ESP", "Spanish Peseta" },
		{ "ESA", "Spanish peseta (account A)" }, { "ESB", "Spanish peseta (account B)" }, 
		{ "LKR", "Sri Lanka Rupee" }, { "SDD", "Sudanese Dinar" }, { "SDP", "Sudanese Pound" },
		{ "SDG", "Sudanese Pound" }, { "SRD", "Surinam Dollar" }, { "SRG", "Suriname guilder" }, 
		{ "SZL", "Lilangeni" }, { "SEK", "Swedish Krona" }, { "CHE", "WIR Euro" }, { "CHW", "WIR Franc" }, 
		{ "SYP", "Syrian Pound" }, { "TWD", "New Taiwan Dollar" }, { "TJS", "Somoni" },
		{ "TJR", "Tajikistan ruble" }, { "TZS", "Tanzanian Shilling" }, { "THB", "Baht" },
		{ "TOP", "Pa'anga" }, { "TTD", "Trinidata and Tobago Dollar" }, { "TND", "Tunisian Dinar" },
		{ "TRY", "New Turkish Lira" }, { "TRL", "Turkish lira A/05" }, { "TMM", "Manat" },
		{ "RUR", "Russian rubleA/97" }, { "SUR", "Soviet Union ruble" }, { "UGX", "Uganda Shilling" },
		{ "UGS", "Ugandan shilling A/87" }, { "UAH", "Hryvnia" }, { "UAK", "Ukrainian karbovanets" },
		{ "AED", "UAE Dirham" }, { "GBP", "Pound Sterling" }, { "USN", "US Dollar (Next Day)" },
		{ "USS", "US Dollar (Same Day)" }, { "UYU", "Peso Uruguayo" }, { "UYN", "Uruguay old peso" }, 
		{ "UYI", "Uruguay Peso en Unidades Indexadas" }, { "UZS", "Uzbekistan Sum" }, { "VUV", "Vatu" }, 
		{ "VEF", "Bolivar Fuerte" }, { "VEB", "Venezuelan Bolivar" }, { "VND", "Dong" }, 
        { "VNC", "Vietnam" },
        { "YER", "Yemeni Rial" }, { "YUD", "Yugoslav Dinar" }, { "YUM", "Yugoslav dinar (new)" }, { "ZRN", "Zairean New Zaire" }, 
		{ "ZRZ", "Zairean Zaire" }, { "ZMK", "Kwacha" }, { "ZWD", "Zimbabwe Dollar" }, { "ZWC", "Zimbabwe Rhodesian dollar" } };

		public static string GetCurrencyCodeByCountryName(string countryName) {
			string countryCode = string.Empty;
			string currencyCode = string.Empty;
			foreach (var country in Countries) {
				if (country.Value == countryName) {
					countryCode = country.Key;
				}
			}
			if (string.IsNullOrEmpty(countryCode) == false) {
				if (CountryCodeAndCurrencies.ContainsKey(countryCode)) {
					currencyCode = CountryCodeAndCurrencies[countryCode];
				}
			}
			return currencyCode;
		}

		public static string GetCurrencyNameByCountryName(string countryName) {
			string countryCode = string.Empty;
			string currencyName = string.Empty;
			foreach (var country in Countries) {
				if (country.Value == countryName) {
					countryCode = country.Key;
				}
			}
			if (string.IsNullOrEmpty(countryCode) == false) {
				string currencyCode = GetCurrencyCodeByCountryName(countryName);
				if (CurrencyCodeAndCurrencyNames.ContainsKey(currencyCode)) {
					currencyName = CurrencyCodeAndCurrencyNames[currencyCode];
				}
			}
			return currencyName;
		}

		public static string GetCountryCodeByCountryName(string countryName) {
			string countryCode = string.Empty;
			foreach (var country in Countries) {
				if (country.Value == countryName) {
					countryCode = country.Key;
				}
			}
			return countryCode;
		}

		public static string GetCurrencyCodeByCurrencyName(string currencyName) {
			string currencyCode = string.Empty;
			foreach (var currency in CurrencyCodeAndCurrencyNames) {
				if (currency.Value == currencyName) {
					currencyCode = currency.Key;
				}
			}
			return currencyCode;
		}

		public static string GetCountryCodeByCurrencyCode(string currencyCode) {
			string countryCode = string.Empty;
			foreach (var country in CountryCodeAndCurrencies) {
				if (country.Value == currencyCode) {
					countryCode = country.Key;
				}
			}
			return countryCode;
		}

		public static string GetCurrencySymbol(string code) {
			if (Currencies.ContainsKey(code)) {
				return Currencies[code];
			} else {
				return code;
			}
		}

		public static Dictionary<string, string> Currencies = new Dictionary<string, string>() {
                                                    {"AED", "د.إ.‏"},
                                                    {"AFN", "؋ "},
                                                    {"ALL", "Lek"},
                                                    {"AMD", "դր."},
                                                    {"ARS", "$"},
                                                    {"AUD", "$"}, 
                                                    {"AZN", "man."}, 
                                                    {"BAM", "KM"}, 
                                                    {"BDT", "৳"}, 
                                                    {"BGN", "лв."}, 
                                                    {"BHD", "د.ب.‏ "},
                                                    {"BND", "$"}, 
                                                    {"BOB", "$b"}, 
                                                    {"BRL", "R$"}, 
                                                    {"BYR", "р."}, 
                                                    {"BZD", "BZ$"}, 
                                                    {"CAD", "$"}, 
                                                    {"CHF", "fr."}, 
                                                    {"CLP", "$"}, 
                                                    {"CNY", "¥"}, 
                                                    {"COP", "$"}, 
                                                    {"CRC", "₡"}, 
                                                    {"CSD", "Din."}, 
                                                    {"CZK", "Kč"}, 
                                                    {"DKK", "kr."}, 
                                                    {"DOP", "RD$"}, 
                                                    {"DZD", "DZD"}, 
                                                    {"EEK", "kr"}, 
                                                    {"EGP", "ج.م.‏ "},
                                                    {"ETB", "ETB"}, 
                                                    {"EUR", "€"}, 
                                                    {"GBP", "£"}, 
                                                    {"GEL", "Lari"}, 
                                                    {"GTQ", "Q"}, 
                                                    {"HKD", "HK$"}, 
                                                    {"HNL", "L."}, 
                                                    {"HRK", "kn"}, 
                                                    {"HUF", "Ft"}, 
                                                    {"IDR", "Rp"}, 
                                                    {"ILS", "₪"}, 
                                                    {"INR", "रु"}, 
                                                    {"IQD", "د.ع.‏ "},
                                                    {"IRR", "ريال "},
                                                    {"ISK", "kr."}, 
                                                    {"JMD", "J$"}, 
                                                    {"JOD", "د.ا.‏ "},
                                                    {"JPY", "¥"}, 
                                                    {"KES", "S"}, 
                                                    {"KGS", "сом"}, 
                                                    {"KHR", "៛"}, 
                                                    {"KRW", "₩"}, 
                                                    {"KWD", "د.ك.‏ "},
                                                    {"KZT", "Т"}, 
                                                    {"LAK", "₭"}, 
                                                    {"LBP", "ل.ل.‏ "},
                                                    {"LKR", "රු."}, 
                                                    {"LTL", "Lt"}, 
                                                    {"LVL", "Ls"}, 
                                                    {"LYD", "د.ل.‏ "},
                                                    {"MAD", "د.م.‏ "},
                                                    {"MKD", "ден."}, 
                                                    {"MNT", "₮"}, 
                                                    {"MOP", "MOP"}, 
                                                    {"MVR", "ރ."}, 
                                                    {"MXN", "$"}, 
                                                    {"MYR", "RM"}, 
                                                    {"NIO", "N"}, 
                                                    {"NOK", "kr"}, 
                                                    {"NPR", "रु"}, 
                                                    {"NZD", "$"}, 
                                                    {"OMR", "ر.ع.‏ "},
                                                    {"PAB", "B/."}, 
                                                    {"PEN", "S/."}, 
                                                    {"PHP", "PhP"}, 
                                                    {"PKR", "Rs"}, 
                                                    {"PLN", "zł"}, 
                                                    {"PYG", "Gs"}, 
                                                    {"QAR", "ر.ق.‏ "},
                                                    {"RON", "lei"}, 
                                                    {"RSD", "Din."}, 
                                                    {"RUB", "р."}, 
                                                    {"RWF", "RWF"}, 
                                                    {"SAR", "ر.س.‏ "},
                                                    {"SEK", "kr"}, 
                                                    {"SGD", "$"}, 
                                                    {"SYP", "ل.س.‏ "},
                                                    {"THB", "฿"}, 
                                                    {"TJS", "т.р."}, 
                                                    {"TMT", "m."}, 
                                                    {"TND", "د.ت.‏ "},
                                                    {"TRY", "TL"}, 
                                                    {"TTD", "TT$"}, 
                                                    {"TWD", "NT$"}, 
                                                    {"UAH", "₴"}, 
                                                    {"USD", "$"}, 
                                                    {"UYU", "$U"}, 
                                                    {"UZS", "so'm"}, 
                                                    {"VEF", "Bs. F."}, 
                                                    {"VND", "₫"}, 
                                                    {"XOF", "XOF"}, 
                                                    {"YER", "ر.ي.‏ "},
                                                    {"ZAR", "R"}, 
                                                    {"ZWL", "Z$"} };
	}
}