// and a module that has a controller that depends on the ListOfItemsService  
define(["knockout"], function (ko) {
    return function AuthModel() {
        var self = this;
        this.errors = ko.observable(null);
        this.first_name = ko.observable("");
        this.last_name = ko.observable("");
        this.user_name = ko.observable("");
        this.role = ko.observable("");
        this.id = ko.observable("");
        this.group_id = ko.observable("");

        this.photo_file_id = ko.observable("");
        this.photo_file_name = ko.observable("");

        this.photo_file_name_src = ko.computed(function () {
            var src = "";
            if (cString(self.photo_file_name()) != "") {
                var patt1 = /\.([0-9a-z]+)(?:[\?#]|$)/i;
                var ext = self.photo_file_name().match(patt1);
                src = siteUrl("/files/user_photo/" + self.id() + "_" + self.photo_file_id() + "_icon" + ext[0]);
            }
            return src;
        });
        this.photo_file_name_medium_src = ko.computed(function () {
            var src = "";
            if (cString(self.photo_file_name()) != "") {
                var patt1 = /\.([0-9a-z]+)(?:[\?#]|$)/i;
                var ext = self.photo_file_name().match(patt1);
                src = siteUrl("/files/user_photo/" + self.id() + "_" + self.photo_file_id() + "_medium" + ext[0]);
            }
            return src;
        });
        this.getDisplayRoleName = function (role) {
            var roleName = "";
            switch (role) {
                case "EA": roleName = "Ecam Admin"; break;
                case "EM": roleName = "Ecam Member"; break;
                case "CA": roleName = "Company Admin"; break;
                case "CM": roleName = "Company Member"; break;
                case "GA": roleName = "Group Admin"; break;
                case "GM": roleName = "Group Member"; break;
                case "AA": roleName = "Agent Admin"; break;
                case "AM": roleName = "Agent Member"; break;
                case "LA": roleName = "Airline Admin"; break;
                case "LM": roleName = "Airline Member"; break;
            }
            return roleName;
        }

        this.menus = ko.observableArray([]);
        this.roles = ko.observableArray([]);
        this.user_roles = ko.observable("");
        this.computeRoles = ko.computed(function () {
            self.roles.removeAll();
            //console.log('roles=',self.user_roles());
            var userRoles = self.user_roles();
            var arr = [];
            arr = userRoles.split(",");
            $.each(arr, function (i, urole) {
                self.roles.push({
                    "role": self.getDisplayRoleName(urole), "value": urole, "is_current_role": (urole == self.role())
                });
            });
            //console.log('arr=',arr);
            return arr;
        });
        this.getRoleName = ko.computed(function () {
            return self.getDisplayRoleName(self.role());
        });

        this.getMenus = function () {
            return [
		        {
		            "name": "Masters", "url": "#/ea-index", "icon": "", "is_active": false, "modules": [], "roles": ['EA', 'EM'], submenus: []
		        }
                , { "name": "Groups", "url": "#/ea-groups", "icon": "", "is_active": false, "modules": [], "roles": ['EA', 'EM'], submenus: [] }
                , {
                    "name": "Forex", "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "roles": ['EA', 'EM'], submenus: [
                        { "name": "Daily Rate", "url": "#/ea-forex", "icon": "", "is_active": false, "modules": [], "roles": ['EA', 'EM'], submenus: [] },
                        { "name": "IATA 5 Rate", "url": "#/ea-iata-forex", "icon": "", "is_active": false, "modules": [], "roles": ['EA', 'EM'], submenus: [] },
                        { "name": "Monthly Rate", "url": "#/ea-monthly-forex", "icon": "", "is_active": false, "modules": [], "roles": ['EA', 'EM'], submenus: [] },
                    ]
                }
                , {
                    "name": "Email", "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "roles": ['EA', 'EM'], submenus:
                        [
                            { "name": "SMTP Setting", "url": "#/ea-smtp-setting", "icon": "", "is_active": false, "modules": [], "roles": ['EA', 'EM'], submenus: [] },
                            { "name": "Email Template", "url": "#/ea-email-template", "icon": "", "is_active": false, "modules": [], "roles": ['EA', 'EM'], submenus: [] },
                            { "name": "Merge Field", "url": "#/ea-merge-field", "icon": "", "is_active": false, "modules": [], "roles": ['EA', 'EM'], submenus: [] }
                        ]
                }
                , { "name": "Agent Members", "url": "#/ea-agents", "icon": "", "is_active": false, "modules": [], "roles": ['EA', 'EM'], submenus: [] }
                , { "name": "CSR Payments", "url": "#/ea-iata-duedate", "icon": "", "is_active": false, "modules": [], "roles": ['EA', 'EM'], submenus: [] }
                /*{ "name": "Map", "url": "#/ga-map-settings", "icon": "", "is_active": false, "modules": [],  "roles": ['GA'], submenus: [] }
                        ,{ "name": "Map", "url": "#/cm-map-settings", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                ,{ "name": "Map", "url": "#/cm-map", "icon": "", "is_active": false, "modules": [], "is_default_display": true,"roles": ['CA', 'CM', 'GA'], submenus: [] }*/
                , {
                    "name": "Report", "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['AA', 'AM'], submenus:
                    [
                        { "name": "Sales", "url": "#/reports", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['AA', 'AM'], submenus: [] }
                        , { "name": "Target Report", "url": "#/target-report", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['AA', 'AM'], submenus: [] }
                    ]
                }
                , { "name": "Map", "url": "#/cm-map", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['AA', 'AM'], submenus: [] }
                , { "name": "Enquiry", "url": "#/aa-online-enquiry", "icon": "", "is_active": false, "modules": [], "roles": ['AA', 'AM'], submenus: [] }
                //, { "name": "AWBs", "url": "#/aa-summary-request", "icon": "", "is_active": false, "modules": [], "roles": ['AA', 'AM'], submenus: [] }                
                , {
                    "name": "Reports", "url": "javascript:;", "icon": "", "is_active": false, "modules": ["is_reports"], "is_user_config": true, "roles": ['CA', 'CM', 'GA', 'GM'], submenus:
                    [
                        { "name": "Sales", "url": "#/reports", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM', 'GA', 'GM'], submenus: [] }
                        , { "name": "Cargo Movement", "url": "#/cargo-report", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM', 'GA', 'GM'], submenus: [] }
                        , { "name": "Agentwise Sales", "url": "#/agent-report", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM', 'GA', 'GM'], submenus: [] }
                        , { "name": "Top 10", "url": "#/top-agent-report", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM', 'GA', 'GM'], submenus: [] }
                        , { "name": "Target Report", "url": "#/target-report", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "is_user_config": true, "roles": ['GA', 'GM', 'CA'], submenus: [] }
                        , { "name": "Finance Report", "url": "#/cm-pl-report", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "separator", "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['GA', 'GM', 'CA', 'CM'], submenus: [] }
                        , { "name": "Settings", "is_group_title": true, "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['GA', 'GM', 'CA', 'CM'], submenus: [] }
                        , { "name": "Airline Target", "url": "#/ga-target", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "is_user_config": true, "roles": ['GA', 'GM', 'CA'], submenus: [] }
                        , { "name": "Agent Target", "url": "#/ga-agent-target", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "is_user_config": true, "roles": ['GA', 'GM', 'CA'], submenus: [] }
                        , { "name": "PL Value", "url": "#/cm-pl-value", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "is_user_config": true, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "PL Ledger Item", "url": "#/cm-pl-ledger-item", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                    ]
                }
                , {
                    "name": "AWBs", "url": "javascript:;", "icon": "", "is_active": false, "modules": ["is_awb_mgt"], "roles": ['CA', 'CM', 'LA', 'LM', 'AA', 'AM'], submenus:
                    [
                        { "name": "Current Stock / Request", "url": "#/aa-awbs", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['AA', 'AM'], submenus: [] },
                        { "name": "AWB Stock Report", "url": "#/aa-agent-stock-report", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['AA', 'AM'], submenus: [] },
                        { "name": "Create AWBs", "url": "#/cm-awblot", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] },
                        { "name": "Company AWB Request", "url": "#/lm-company-awbrequest", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['LA', 'LM'], submenus: [] },
                        { "name": "AWB Request List", "url": "#/cm-awbrequest", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] },
                        { "name": "AWB Allotment List", "url": "#/cm-awb-allotment-list", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] },
                        { "name": "Void AWB & Return", "url": "#/aa-summary-request", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] },
                        { "name": "AWB Stock Report", "url": "#/cm-awb-stock-report", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA','CM'], submenus: [] },
                        //{ "name": "AWB Stock Summary","url": "#/cm-awb-stock-summary","icon": "","is_active": false,"modules": [],"is_default_display": true,"roles": ['CA','CM'],submenus: [] },
                        //{ "name": "Airline Stock Status","url": "#/cm-awb-stock-status","icon": "","is_active": false,"modules": [],"is_default_display": true,"roles": ['CA','CM','LA','LM'],submenus: [] },
                        //{ "name": "Agent Stock Report","url": "#/aa-agent-stock-report","icon": "","is_active": false,"modules": [],"is_default_display": true,"roles": ['CA','CM'],submenus: [] },
                        { "name": "separator", "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] },
                        { "name": "Settings", "is_group_title": true, "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] },
                        { "name": "Airline Setting", "url": "#/ca-awb-airline-setting", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] },
                        { "name": "Agent Setting", "url": "#/ca-awb-agent-setting", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] },
                        { "name": "Opening AWB Stock", "url": "#/ca-awb-stock-import", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                    ]
                }
                , {
                    "name": "Bookings", "url": "javascript:;", "icon": "", "is_active": false, "modules": ["is_bookings"], "roles": ['CA', 'CM', 'AA', 'AM'], submenus:
                        [
                         { "name": "Book Cargo", "url": "#/aa-flight-book", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM', 'AA', 'AM'], submenus: [] }
                        , { "name": "Booking Confirmation", "url": "#/cm-space-confirmation", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                        //,{ "name": "Booking Status","url": "#/aa-flight-book-request","icon": "","is_active": false,"modules": [],"is_default_display": true,"roles": ['CA','CM','AA','AM'],submenus: [] }
                        , { "name": "Booking List", agent_menu_name: "Reservation Status", "url": "#/aa-flight-book-list", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM', 'AA', 'AM'], submenus: [] }
						, { "name": "Adhoc List", "url": "#/cm-adhoc-list", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "FSU-RCS", "url": "#/cm-cargo-received", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "AWB Upload", "url": "#/aa-awb-upload", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM', 'AA', 'AM'], submenus: [] }
                        , { "name": "Cargo Uplift", "url": "#/cm-cargo-uplift", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "Flt Booking Status", "url": "#/aa-flight-book-status", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "separator", "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "Settings", "is_group_title": true, "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }

                        //,{ "name": "Link Airports","url": "#/cm-company-airport","icon": "","is_active": false,"modules": [],"is_default_display": false,"roles": ['CA','CM'],submenus: [] }
                        //,{ "name": "Commodity Codes","url": "#/ca-commodity-type","icon": "","is_active": false,"modules": [],"is_default_display": false,"roles": ['CA','CM'],submenus: [] }

                        , { "name": "Create Flights", "url": "#/cm-company-flight", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }

                        , { "name": "Routes", "url": "#/cm-routes", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "Pre-Alert Settings", "url": "#/cm-prealert", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "Adhoc Rule", "url": "#/cm-adhoc-rule", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "Adhoc Email Setting", "url": "#/cm-adhoc-email-setting", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "Participants Info", "url": "#/ca-company-participant", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "FFR Contacts", "url": "#/ca-company-ffr", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "Auto Allotment", "url": "#/ga-flight-allotment-type", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                        //, { "name": "IATA Rate", "url": "#/cm-iata-rate", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA','CM'], submenus: [] }
                        ]
                }
		        //, {
		        //    "name": "AWB Print", "url": "#/aa-agent-awb-list", "icon": "", "is_active": false, "modules": [], "roles": ['AA', 'AM'], submenus: []
		        //}                
		        , {
		            "name": "Tracking", "url": "javascript:;", "icon": "", "is_active": false, "modules": ["is_track_trace"], "roles": ['CA', 'CM', 'GA', 'GM'], submenus:
                     [
                         //{ "name": "OUR", "url": "#/cm-cargo-our", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] },
                         { "name": "Transit", "url": "#/cm-track-trace", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] },
                         { "name": "Transit Analysis", "url": "#/cm-transit-report", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] },
                         { "name": "Transit Report", "url": "#/aa-track-trace", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM', 'AA', 'AM'], submenus: [] },
                         { "name": "Pre-Alert List", "url": "#/cm-prealert-list", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] },
                         { "name": "TV Display", "url": "#/cm-tv", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] },
                         //{ "name": "Pre Alert Report", "url": "#/cm-prealert-list", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] },
                         { "name": "Map", "url": "#/cm-map", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['GA', 'GM', 'CA', 'CM'], submenus: [] },
                         { "name": "separator", "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] },
                         { "name": "Settings", "is_group_title": true, "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] },
                         //{ "name": "Map","url": "#/ga-map-settings","icon": "","is_active": false,"modules": [],"roles": ['GA'],submenus: [] },
                         { "name": "Map", "url": "#/cm-map-settings", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] },
                     ]
		        }
                , { "name": "Tracking", "url": "#/aa-track-trace", "icon": "", "is_active": false, "modules": [], "roles": ['AA', 'AM'], submenus: [] }
                , {
                    "name": "Accounts", "url": "javascript:;", "icon": "", "is_active": false, "modules": ["is_invoice"], "is_user_config": true, "roles": ['CA', 'CM', 'AA', 'AM'], submenus:
                        [
                              { "name": "AWB Audit", "url": "#/cm-audit", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                            , { "name": "Invoice", "url": "#/cm-invoice", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM', 'AA', 'AM'], submenus: [] }
                            , { "name": "Credit Control", "url": "#/cm-credit-control", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                            //,{ "name": "Discrepancy","url": "#/cm-invoice-discrepancy","icon": "","is_active": false,"modules": [],"is_default_display": true,"roles": ['CA','CM','AA','AM'],submenus: [] }                            
                            , { "name": "Airline CSR", "url": "#/cm-csr-report", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "is_user_config": true, "roles": ['CA', 'CM'], submenus: [] }
                            , { "name": "Payment", "url": "#/cm-credit-control", "icon": "", "is_active": false, "modules": [], "roles": ['AA', 'AM'], submenus: [] }
                            , { "name": "Invoice Origin Uplift Report", "url": "#/cm-invoice-our", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM', 'AA', 'AM'], submenus: [] }
                            , { "name": "separator", "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                            , { "name": "Reports", "is_group_title": true, "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                            , { "name": "Report", "url": "#/cm-booking-report", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                            , { "name": "Airline CSR Method", "url": "#/cm-report-method", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
							, { "name": "Tally Report", "url": "#/cm-tally-report", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }

							, { "name": "separator", "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM', 'AA', 'AM'], submenus: [] }
                            , { "name": "Settings", "is_group_title": true, "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM', 'AA', 'AM'], submenus: [] }
                            , { "name": "Invoice Address Setting", "url": "#/aa-agent-setting", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['AA', 'AM'], submenus: [] }
                            , { "name": "Tally Setting", "url": "#/cm-tally-setting", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
							, { "name": "Period Selection", "url": "#/cm-invoice-period", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                            , { "name": "Payment Term", "url": "#/cm-invoice-payment-term", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                            //,{ "name": "Rate Sheet Charge Types","url": "#/ca-rschage-types","icon": "","is_active": false,"modules": [],"is_default_display": false,"roles": ['CA','CM'],submenus: [] }
                            , { "name": "Accouting Charge Types", "url": "#/ca-acc-charge-types", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                            , { "name": "Accounting Charges", "url": "#/cm-invoice-charge", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
							, { "name": "Invoice Method", "url": "#/cm-invoice-method", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                            , { "name": "Airline Setting", "url": "#/ca-company-airline-setting", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                            , { "name": "Company Address", "url": "#/cm-company-address", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                        ]
                }
                  , {
                      "name": "Agent", "url": "javascript:;", "icon": "", "is_active": false, "modules": ["is_agent"], "roles": ['CA', 'CM'], submenus:
                      [
                          { "name": "Invitation", "url": "#/ca-agent-invitation", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                          , { "name": "Agent Request", "url": "#/ca-agent-request", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                          , { "name": "Airline Invitation", "url": "#/ca-airline-invitation", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                          , { "name": "Agent", "url": "#/ca-agent", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                          , { "name": "Company To Agent", "url": "#/ca-company-agent", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA'], submenus: [] }
                      ]
                  }
                , {
                    "name": "Import", "url": "javascript:;", "icon": "", "is_active": false, "modules": ["is_import"], "roles": ['CA', 'CM'], submenus:
                    [
                        { "name": "Sales", "url": "#/ca-import-sales", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] },
                        { "name": "Cost 2", "url": "#/ca-import-cost2", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] },
                        { "name": "Cargo Movement", "url": "#/ca-import-sales-detail", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['CA', 'CM'], submenus: [] }
                    ]
                }
                , { "name": "Companies", "url": "#/ga-companies", "icon": "", "is_active": false, "modules": [], "roles": ['GA'], submenus: [] }
                , { "name": "Users", "url": "#/ga-users", "icon": "", "is_active": false, "modules": [], "roles": ['GA'], submenus: [] }
                , {
                    "name": "Manage", "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "roles": ['GA', 'GM'], submenus:
                        [
                            { "name": "Zones", "url": "#/ga-zones", "icon": "", "is_active": false, "modules": [], "is_default_display": true, "roles": ['GA', 'GM'] }
                        ]
                }
                //, { "name": "Uplift", "url": "#/aa-uplift", "icon": "", "is_active": false, "modules": [], "roles": ['AA', 'AM'], submenus: [] }

                , {
                    "name": "Documents", "url": "javascript:;", "icon": "", "is_active": false, "modules": ["is_document_upload"], "is_default_display": false, "is_user_config": true, "roles": ['CA', 'CM'], submenus:
                     [
                         { "name": "Document Upload", "url": "#/cm-documents", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "is_user_config": true, "roles": ['CA', 'CM'], submenus: []
                     }
                         , { "name": "separator", "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                         , { "name": "Settings", "is_group_title": true, "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                         , { "name": "Document Types", "url": "#/cm-document-types", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                     ]
                }
                , { "name": "Mail Setting", "url": "#/aa-awb-mail-setting", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM', 'AA'], submenus: [] }
                , { "name": "My Account", "url": "#/aa-send-request", "icon": "", "is_active": false, "modules": [], "roles": ['AA', 'AM'], submenus: [] }
                , {
                    "name": "Rate", "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "roles": ['CA', 'CM'], submenus:
                    [
                         { "name": "Settings", "is_group_title": true, "url": "javascript:;", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "Sale & Cost Rate", "url": "#/cm-rate-sheet", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "Rate Sheet Charge Types", "url": "#/ca-rschage-types", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                        , { "name": "My Forex", "url": "#/ca-my-forex", "icon": "", "is_active": false, "modules": [], "is_default_display": false, "roles": ['CA', 'CM'], submenus: [] }
                    ]
                }
            ];
        }


        this.getFilterMenus = function (role) {
            var roleMenus = [];
            var menus = self.getMenus();
            var currentRole = getAuth().role;
            $.each(menus, function (i, menu) {
                var m = self.checkMenuRole(role, menu);
                if (m != null) {

                    var check = self.checkAllowedModule(role, menu);
                    if (check) {                       
                        if (role == 'CM') {
                            var auth = getAuth();
                            var mod = cString(auth.modules);
                            var is_settings = false;
                            mods = JSON.parse(mod);
                            $.each(mods, function (key, value) {
                                if (key == "is_settings") {
                                    if (value == true) {
                                        is_settings = true;
                                    }
                                }
                            });

                            if (is_settings == true) {                                
                                roleMenus.push(m);
                            } else {                                
                                var m1 = self.getFilterSubmenus(role, menu);
                                roleMenus.push(m1);
                            }
                        } else {                   
                            //console.log("menu :" + m.name + " is_user_config:" + m.is_user_config);
                            if (role == "CA" || role=="GM") {
                                if (m.is_user_config == true) {
                                    //console.log("menu :" + m.name + " is_user_config:" + m.is_user_config);
                                    if (m.name == "Documents") {
                                        roleMenus.push(m);
                                    } else if (m.name == "Accounts" || m.name == "Reports") {
                                        var m1 = self.getFilterSubmenus(role, menu);
                                        roleMenus.push(m1);
                                    } else {
                                        roleMenus.push(m);
                                    }
                                } else {
                                    roleMenus.push(m);
                                }                                                              
                            } else {
                                roleMenus.push(m);
                            }

                                                                                                           
                        }

                    }
                }
            });
            if (currentRole == 'AA' || currentRole == 'AM') {
                $.each(roleMenus, function (i, rm) {
                    $.each(rm.submenus, function (i, m) {
                        if (m.agent_menu_name) {
                            if (cString(m.agent_menu_name) != '') {
                                m.name = m.agent_menu_name;
                            }
                        }
                    });
                });
            }
            return roleMenus;
        }

        this.getFilterSubmenus = function (role, menu) {
            var m1 = null;
            m1 = {}; 
            $.extend(true, m1, menu);
            if (role == "CM") {
                //if(menu.name == "AWB Mgt") {                    
                m1.submenus = [];                
                $.each(menu.submenus, function (index, row) {
                    if (row.is_default_display == true) {
                        m1.submenus.push(row);
                    } else {
                        //console.log("menu 1 :" + menu.name + " default :" + row.is_default_display + " config:" + row.is_user_config);
                        if (row.is_default_display == false && row.is_user_config == true) {                            
                            if (row.name == "Airline CSR") {
                                if (getAuth().is_cost_profit == "True") {
                                    m1.submenus.push(row);
                                }
                            } else if (row.name == "PL Value" || row.name == "Airline Target" || row.name == "Agent Target") {
                            } else if (row.name == "Target Report" ) {
                                if (getAuth().is_target == "True") {
                                    m1.submenus.push(row);
                                }
                            } else if (row.name == "Document Upload" || row.name == "Document Types") {
                                if (getAuth().is_document == "True") {
                                    m1.submenus.push(row);
                                }
                            } else {
                                m1.submenus.push(row);
                            }
                        } 
                    }
                });
                //}           
            }
            if (role == "CA" || role=="GM") {
                m1.submenus = [];
                $.each(menu.submenus, function (index, row) {
                    if (row.is_default_display == false && row.is_user_config == true) {
                        if (row.name == "Airline CSR") {
                            if (getAuth().is_cost_profit == "True") {
                                m1.submenus.push(row);
                            }                            
                        } else if (row.name == "PL Value") {
                            if (getAuth().is_financial == "True") {
                                m1.submenus.push(row);
                            }
                        } else if (row.name == "Target Report" || row.name=="Airline Target" || row.name=="Agent Target") {
                            if (getAuth().is_target == "True") {
                                m1.submenus.push(row);
                            }
                        } else {
                            m1.submenus.push(row);
                        }
                    } else {
                        m1.submenus.push(row);
                    }                   
                });
            }
            return m1;
        }

        this.checkAllowedModule = function (role, menu) {
            if (role == "CM") {
                var check = false;
                var auth = getAuth();
                //console.log('auth2=',auth);
                if (auth != null) {
                    if (auth["modules"]) {
                        var m = cString(auth.modules);
                        if (m != '') {
                            modules = JSON.parse(m);
                            $.each(modules, function (key, module) {
                                //console.log('key=',key,'module=',module);
                                var arr = [];
                                if (menu.modules) {
                                    arr = menu.modules;
                                }
                                $.each(arr, function (i, moduleName) {
                                    if (key.toLowerCase() == moduleName.toLowerCase() && cBool(module) == true) {                                        
                                        check = true;                                        
                                    }
                                });
                            });
                        }
                    }
                }
                return check;
            } else {
                //console.log("role :" + role + " menu :" + menu.name + " is_user_config :" + menu.is_user_config);
                if (role == "CA" || role == "GM") {
                    if (menu.is_user_config == true) {
                        var result = self.checkUserConfig(role, menu);
                        return result;
                    } else {
                        return true;
                    }
                } else {
                    return true;
                }
               
                //return true;
            }
        }

        this.checkUserConfig = function (role, menu) {
            if (role == "CA" || role == "CM" || role == "GM") {
                if (menu.name != "Documents") {  //Menu and submenu show/hide                  
                    return true;
                } else if (menu.name == "Documents") {
                    if(getAuth().is_document=="True"){
                        return true;
                    }else{
                        return false;
                    }                    
                } else {
                    return true;
                }
            } else {
                return true;
            }
        }

        this.checkMenuRole = function (role, menu) {
            var m = null;
            $.each(menu.roles, function (j, r) {
                if (r == role) {
                    m = {};
                    $.extend(true, m, menu);
                    m.submenus = [];
                    if (menu.submenus) {
                        $.each(menu.submenus, function (z, subMenu) {
                            var sm = self.checkMenuRole(role, subMenu);
                            if (sm != null) {
                                m.submenus.push(sm);
                            }
                        });
                    }
                }
            });
            return m;
        }
        this.role.subscribe(function (newValue) {
            self.menus.removeAll();
            self.menus(self.getFilterMenus(self.role()));
        });
        this.getName = ko.computed(function () {
            return self.first_name() + " " + self.last_name();
        });
        this.selectMenu = function (name) {
            $.each(self.menus, function (i, menu) {
                if (menu.name == name) {
                    menu.is_active = true;
                } else {
                    menu.is_active = false;
                }
            });
        }

    }
});
