update tra_company set is_old = 1 where symbol in (
'3MINDIA',
'8KMILES',
'AARTIIND',
'ABAN',
'ABB',
'ABFRL',
'ABIRLANUVO',
'ACC',
'ADANIENT',
'ADANIPORTS',
'ADANIPOWER',
'ADANITRANS',
'ADVENZYMES',
'AEGISCHEM',
'AHLUCONT',
'AIAENG',
'AJANTPHARM',
'AKZOINDIA',
'ALBK',
'ALKEM',
'ALLCARGO',
'AMARAJABAT',
'AMBUJACEM',
'AMTEKAUTO',
'ANANTRAJ',
'ANDHRABANK',
'APARINDS',
'APLAPOLLO',
'APLLTD',
'APOLLOHOSP',
'APOLLOTYRE',
'ARVIND',
'ASAHIINDIA',
'ASHOKA',
'ASHOKLEY',
'ASIANPAINT',
'ASTRAL',
'ASTRAZEN',
'ATUL',
'AUROPHARMA',
'AVANTIFEED',
'AXISBANK',
'BAJAJ-AUTO',
'BAJAJCORP',
'BAJAJELEC',
'BAJAJFINSV',
'BAJAJHIND',
'BAJAJHLDNG',
'BAJFINANCE',
'BALKRISIND',
'BALLARPUR',
'BALMLAWRIE',
'BALRAMCHIN',
'BANKBARODA',
'BANKINDIA',
'BANKNIFTY',
'BASF',
'BATAINDIA',
'BBTC',
'BEL',
'BEML',
'BERGEPAINT',
'BFUTILITIE',
'BGRENERGY',
'BHARATFIN',
'BHARATFORG',
'BHARTIARTL',
'BHEL',
'BHUSANSTL',
'BIOCON',
'BIRLACORPN',
'BLISSGVS',
'BLUEDART',
'BLUESTARCO',
'BOMDYEING',
'BOSCHLTD',
'BPCL',
'BRITANNIA',
'CADILAHC',
'CANBK',
'CANFINHOME',
'CANTABIL',
'CAPF',
'CAPLIPOINT',
'CARBORUNIV',
'CARERATING',
'CASTROLIND',
'CCL',
'CEATLTD',
'CENTRALBK',
'CENTURYPLY',
'CENTURYTEX',
'CERA',
'CESC',
'CGPOWER',
'CHAMBLFERT',
'CHENNPETRO',
'CHOLAFIN',
'CIPLA',
'COALINDIA',
'COFFEEDAY',
'COLPAL',
'CONCOR',
'COROMANDEL',
'CORPBANK',
'COX&KINGS',
'CRISIL',
'CROMPTON',
'CUB',
'CUMMINSIND',
'CYIENT',
'DAAWAT',
'DABUR',
'DALMIABHA',
'DBCORP',
'DBL',
'DBREALTY',
'DCBBANK',
'DCMSHRIRAM',
'DECCANCE',
'DEEPAKFERT',
'DELTACORP',
'DEN',
'DENABANK',
'DHANUKA',
'DHFL',
'DISHTV',
'DIVISLAB',
'DLF',
'DMART',
'DREDGECORP',
'DRREDDY',
'ECLERX',
'EDELWEISS',
'EICHERMOT',
'EIDPARRY',
'EIHOTEL',
'EMAMILTD',
'ENDURANCE',
'ENGINERSIN',
'EQUITAS',
'EROSMEDIA',
'ESCORTS',
'ESSELPACK',
'EVEREADY',
'EXIDEIND',
'FAGBEARING',
'FCONSUMER',
'FDC',
'FEDERALBNK',
'FEL',
'FINCABLES',
'FINPIPE',
'FLFL',
'FMGOETZE',
'FORTIS',
'FRETAIL',
'FSL',
'GAIL',
'GATI',
'GAYAPROJ',
'GDL',
'GEPIL',
'GESHIP',
'GET&D',
'GHCL',
'GILLETTE',
'GLAXO',
'GLENMARK',
'GMDCLTD',
'GMRINFRA',
'GNFC',
'GODFRYPHLP',
'GODREJCP',
'GODREJIND',
'GODREJPROP',
'GPPL',
'GRANULES',
'GRASIM',
'GREAVESCOT',
'GREENPLY',
'GRINDWELL',
'GRUH',
'GSFC',
'GSKCONS',
'GSPL',
'GUJALKALI',
'GUJFLUORO',
'GUJGASLTD',
'GULFOILLUB',
'GVKPIL',
'HATHWAY',
'HAVELLS',
'HCC',
'HCL-INSYS',
'HCLTECH',
'HDFC',
'HDFCBANK',
'HDIL',
'HEIDELBERG',
'HEROMOTOCO',
'HEXAWARE',
'HFCL',
'HIMATSEIDE',
'HINDALCO',
'HINDCOPPER',
'HINDPETRO',
'HINDUNILVR',
'HINDZINC',
'HMVL',
'HONAUT',
'HSIL',
'HTMEDIA',
'HUDCO',
'IBREALEST',
'IBULHSGFIN',
'ICICIBANK',
'ICICIPRULI',
'ICIL',
'ICRA',
'IDBI',
'IDEA',
'IDFC',
'IDFCBANK',
'IFCI',
'IGARASHI',
'IGL',
'IIFL',
'IL&FSTRANS',
'INDHOTEL',
'INDIACEM',
'INDIANB',
'INDIGO',
'INDOCO',
'INDUSINDBK',
'INFIBEAM',
'INFRATEL',
'INFY',
'INOXLEISUR',
'INOXWIND',
'INTELLECT',
'IOB',
'IOC',
'IPCALAB',
'IRB',
'ITC',
'ITDCEM',
'J&KBANK',
'JAGRAN',
'JAICORPLTD',
'JBCHEPHARM',
'JBFIND',
'JCHAC',
'JETAIRWAYS',
'JINDALPOLY',
'JINDALSTEL',
'JISLJALEQS',
'JKCEMENT',
'JKIL',
'JKLAKSHMI',
'JKTYRE',
'JMFINANCIL',
'JMTAUTOLTD',
'JPASSOCIAT',
'JSWENERGY',
'JSWSTEEL',
'JUBILANT',
'JUBLFOOD',
'JUSTDIAL',
'JYOTHYLAB',
'KAJARIACER',
'KALPATPOWR',
'KANSAINER',
'KARURVYSYA',
'KEC',
'KESORAMIND',
'KITEX',
'KKCL',
'KNRCON',
'KOLTEPATIL',
'KOTAKBANK',
'KPIT',
'KPRMILL',
'KRBL',
'KSCL',
'KTKBANK',
'KWALITY',
'L&TFH',
'LAKSHVILAS',
'LALPATHLAB',
'LAXMIMACH',
'LICHSGFIN',
'LT',
'LTI',
'LTTS',
'LUPIN',
'M&M',
'M&MFIN',
'MAGMA',
'MAHINDCIE',
'MAHSCOOTER',
'MANAPPURAM',
'MANPASAND',
'MARICO',
'MARKSANS',
'MARUTI',
'MCDOWELL-N',
'MCLEODRUSS',
'MERCK',
'MGL',
'MHRIL',
'MINDACORP',
'MINDAIND',
'MINDTREE',
'MMTC',
'MOIL',
'MONSANTO',
'MOTHERSUMI',
'MOTILALOFS',
'MPHASIS',
'MRF',
'MRPL',
'MTNL',
'MUTHOOTFIN',
'NATCOPHARM',
'NATIONALUM',
'NAUKRI',
'NAVINFLUOR',
'NAVKARCORP',
'NAVNETEDUL',
'NBCC',
'NBVENTURES',
'NCC',
'NESTLEIND',
'NETWORK18',
'NH',
'NHPC',
'NIFTY',
'NIITTECH',
'NILKAMAL',
'NITINFIRE',
'NITINSPIN',
'NLCINDIA',
'NMDC',
'NTPC',
'OBEROIRLTY',
'OFSS',
'OIL',
'OMAXE',
'ONGC',
'ORIENTBANK',
'ORIENTCEM',
'ORISSAMINE',
'PAGEIND',
'PARAGMILK',
'PCJEWELLER',
'PEL',
'PERSISTENT',
'PETRONET',
'PFC',
'PFIZER',
'PFS',
'PGHH',
'PHOENIXLTD',
'PIDILITIND',
'PIIND',
'PNB',
'PNCINFRA',
'POLARIS',
'POWERGRID',
'PRAJIND',
'PRESTIGE',
'PRISMCEM',
'PROVOGE',
'PTC',
'PUNJLLOYD',
'PVR',
'QUESS',
'RADICO',
'RAIN',
'RAJESHEXPO',
'RALLIS',
'RAMCOCEM',
'RAMCOSYS',
'RATNAMANI',
'RAYMOND',
'RBLBANK',
'RCF',
'RCOM',
'RDEL',
'RECLTD',
'REDINGTON',
'RELAXO',
'RELCAPITAL',
'RELIANCE',
'RELIGARE',
'RELINFRA',
'RENUKA',
'REPCOHOME',
'RIIL',
'RKFORGE',
'ROLTA',
'RPOWER',
'RTNPOWER',
'RUCHIRA',
'RUCHISOYA',
'SADBHAV',
'SAIL',
'SANOFI',
'SBIN',
'SCHNEIDER',
'SCI',
'SHARDACROP',
'SHILPAMED',
'SHILPI',
'SHK',
'SHOPERSTOP',
'SHREECEM',
'SHRIRAMCIT',
'SIEMENS',
'SJVN',
'SKFINDIA',
'SMLISUZU',
'SNOWMAN',
'SOBHA',
'SOLARINDS',
'SOMANYCERA',
'SONATSOFTW',
'SOUTHBANK',
'SPARC',
'SREINFRA',
'SRF',
'SRTRANSFIN',
'STAR',
'STCINDIA',
'STRTECH',
'SUDARSCHEM',
'SUNDARMFIN',
'SUNDRMFAST',
'SUNPHARMA',
'SUNTECK',
'SUNTV',
'SUPRAJIT',
'SUPREMEIND',
'SUVEN',
'SUZLON',
'SWANENERGY',
'SYMPHONY',
'SYNDIBANK',
'SYNGENE',
'TAKE',
'TATACHEM',
'TATACOFFEE',
'TATACOMM',
'TATAELXSI',
'TATAGLOBAL',
'TATAINVEST',
'TATAMOTORS',
'TATAMTRDVR',
'TATAPOWER',
'TATASPONGE',
'TATASTEEL',
'TCS',
'TECHM',
'TECHNO',
'TEXRAIL',
'THERMAX',
'THOMASCOOK',
'THYROCARE',
'TIDEWATER',
'TIMKEN',
'TITAN',
'TMRVL',
'TNPL',
'TORNTPHARM',
'TORNTPOWER',
'TRENT',
'TRIDENT',
'TRITURBINE',
'TTKPRESTIG',
'TUBEINVEST',
'TV18BRDCST',
'TVSMOTOR',
'TVSSRICHAK',
'TVTODAY',
'UBL',
'UCOBANK',
'UFLEX',
'UJJIVAN',
'ULTRACEMCO',
'UNICHEMLAB',
'UNIONBANK',
'UNITECH',
'UPL',
'V2RETAIL',
'VAKRANGEE',
'VARDHACRLC',
'VEDL',
'VGUARD',
'VIDEOIND',
'VIJAYABANK',
'VINATIORGA',
'VIPIND',
'VMART',
'VOLTAS',
'VRLLOG',
'VSTIND',
'VTL',
'WABAG',
'WABCOINDIA',
'WELCORP',
'WELSPUNIND',
'WHIRLPOOL',
'WIPRO',
'WOCKPHARMA',
'WONDERLA',
'YESBANK',
'ZEEL',
'ZEELEARN',
'ZENSARTECH',
'ZYDUSWELL'
);
update tra_category set is_book_mark = 1 where category_name in (select distinct category_name from tra_company_category cc join tra_company c on c.symbol = cc.symbol where ifnull(c.is_book_mark,0)=1);