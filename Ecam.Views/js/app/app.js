// and a module that has a controller that depends on the ListOfItemsService  
define("app", ["knockout", "komapping", "../models/AuthModel", "finch", "helper"], function (ko, komapping, AuthModel, finch, helper) {
    return function () {
        var self = this;
        _ECAMAPP = self;
        this.my = ko.observable(null);
        this.page_title = ko.observable("");
        this.viewModel = ko.observable();

        this.setMy = function (json) {
            if (json == null || json == undefined) return;
            setAuth(json);
            var m = new AuthModel();
            komapping.fromJS(json, {}, m);
            m.user_name(json.username);
            self.my(m);
            var $bdy = $("body");
            if (self.my().role() == 'AA' || self.my().role() == 'AM') {
                $bdy.addClass("is-agent-bdy");
            }
            $(".dyn-change-role").unbind('click').click(function () {
                var $this = $(this);
                var role = $this.attr('role');
                jAlert({ "message": "Change Role...", "isNotify": true });
                var url = "/Account/ChangeRole";
                var arr = [];
                arr.push({ "name": "change_role", "value": role });
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "POST",
                    "data": arr
                }).done(function (json) {
                    if (json.length > 0) {
                        if (_ECAMAPP != null) {
                            _ECAMAPP.logOut();
                        }
                    } else {
                        removeUserAuthorizeCache();
                        getUserAuthorizeJSON(function () {
                            var currentUrl = formatCurrentURL();
                            //console.log('current url=',currentUrl);
                            var redirectUrl = '';
                            var menu = self.getMenu(currentUrl);
                            //console.log('current menu=',menu);
                            if (menu == null) {
                                redirectUrl = '/aa-summary-request';//self.getFirstMenu();
                            } else {
                                redirectUrl = menu.url.replaceAll('#', '');
                            }
                            //console.log('redirectUrl=',redirectUrl);
                            finch.navigate('/blank');
                            setTimeout(function () {
                                if (redirectUrl != '') {
                                    finch.navigate(redirectUrl);
                                }
                            }, 1000);
                            jCloseAlert();
                        });
                    }
                }).fail(function (response) {
                    jCloseAlert();
                });
            });
            $.each(_SET_MY_EVENTS, function (i, cb) {
                if (cb) {
                    cb();
                }
            });
        }

        this.viewModel.subscribe(function (previousModel) {
            $(".additional-cnt").remove();
            hideAllToolTips();
            // clear global company events
            _GLOBAL_COMPANY_EVENTS = [];
            if (previousModel) {
                if (previousModel.unInit) {
                    previousModel.unInit();
                }
            }
        }, this, "beforeChange");

        this.initPage = function () {
            var $pageContent = $(".page-content");
            var $pageModel = $(".page-model", $pageContent);
            helper.init();
            if (self.viewModel().init)
                self.viewModel().init();
        }

        this.clearLoginDetails = function (callback) {
            var auth = getAuth();
            if (auth != null) {
                handleBlockUI({ "message": "Logout..." });
                $.ajax({
                    "url": "Account/LogOff", //apiUrl("/Account/Logout"),
                    "cache": false,
                    "type": "POST"
                }).done(function (json) {
                    // unblockUI();
                    //if(callback)
                    // callback();
                }).fail(function (jqxhr) {
                    //if(callback)
                    //   callback();
                });
            } else {
                //if(callback)
                //  callback();
            }
        }

        this.logOut = function () {
            setAuth(null);
            $("#logoutForm").submit();
            /*
            self.clearLoginDetails(function() {
                setAuth(null);
                self.my(null);
                $(".modal-scrollable").remove();
                $(".modal-backdrop").remove();
                goToLogin();
            });
            */
        }
        this.companyMembers = function () {
            finch.navigate("/ga-companymembers");
        }
        this.agentMembers = function () {
            finch.navigate("/aa-agentmembers");
        }
        this.onSearch = function (d, e) {
            // e.stopPropagation();console.log(d,e,e.keyCode);
            if (e.keyCode === 13) {
                var str = $(e.target).val();
                var hashstr = window.location.hash;
                if (str.match(/[a-z]/i)) {
                    var arr = str.split("/");
                    var flightNo = arr[0];
                    var sDate = arr[1] ? arr[1] : '';
                    finch.navigate("/aa-flight-book-status/" + flightNo + "/" + sDate);
                    return false;
                } else {
                    finch.navigate("/cm-awbno-search/" + str);
                    return false;
                    //finch.navigate("/demo-awbno-search");
                }
            }
            else {
                return true;
            }
        };

        this._MyProfileTemplate = '';
        this.getMyProfileTemplate = function (callback) {
            if (self._MyProfileTemplate == '') {
                handleBlockUI();
                $.get("/Home/MyProfile", function (data) {
                    self._MyProfileTemplate = data;
                    unblockUI();
                    if (callback)
                        callback();
                });
            } else {
                if (callback)
                    callback();
            }
        }

        this.saveAgentProfileName = function () {
            var $frm = $("#frmChangeProfile");
            var arr = [];
            var $name = $(":input[name='first_name']", $frm);

            arr[arr.length] = { "name": "agent_id", "value": cInt(getAuth()[getAuth().role + '_agent_ids']) };
            arr[arr.length] = { "name": "first_name", "value": $name.val() };
            var url = apiUrl("/Agent/UpdateAgentProfileName");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "POST",
                "data": arr
            }).done(function (json) {
                $(".navbar-header span.name", "body").html($name.val());
            });
            return true;
        }

        this.changeProfile = function () {
            self.getMyProfileTemplate(function () {
                var modalID = "changeProfileModal";
                $("#" + modalID).remove();
                var $cnt = $("<div id='changeProfileModal'></div>");
                var data = {};
                $.tmpl(self._MyProfileTemplate, data).appendTo($cnt);
                //$("#my-profile-edit-template").tmpl(data).appendTo($cnt);
                $("body").append($cnt);
                ko.applyBindings(self.my(), $cnt[0]);
                var $modal = $(".modal", $cnt);
                $modal.on('shown.bs.modal', function (e) {

                    $("#save", "#frmChangeProfile").click(function () {
                        var $frm = $("#frmChangeProfile");
                        if ($frm.valid()) {
                            if (getAuth().role == "AA" || getAuth().role == "AM") {
                                self.saveAgentProfileName();
                            }
                            var url = apiUrl("/User/UpdateProfile");
                            var $btn = $(this);
                            $btn.button("loading");
                            handleBlockUI({ "target": $("body"), "message": "Save..." });
                            $.ajax({
                                "url": url,
                                "cache": false,
                                "type": "POST",
                                "data": $frm.serializeArray()
                            }).done(function (json) {
                                $btn.button("reset");
                                unblockUI();

                                var isRememberMe = getLS("rememberme");
                                var storage = getAuth();
                                storage.first_name = $(":input[name='first_name']", $frm).val();
                                storage.last_name = $(":input[name='last_name']", $frm).val();
                                setAuth(storage);

                                handleBlockUI({ "message": "Photo upload..." });
                                var uploadFileData = new FormData();
                                var files = $("#userPhotoFileUpload", $frm).get(0).files;
                                var is_remove_file = "false";
                                var $userPhotoFileInput = $("#userPhotoFileInput", $frm);
                                // Add the uploaded image content to the form data collection
                                if (files.length > 0) {
                                    uploadFileData.append("user_photo_file", files[0]);
                                } else {
                                    if ($userPhotoFileInput.hasClass("fileinput-new")) {
                                        is_remove_file = "true";
                                    }
                                }
                                if (files.length <= 0 && $userPhotoFileInput.hasClass("fileinput-exists")) {
                                    $btn.button('reset');
                                    unblockUI();
                                    jAlert("Saved");
                                    $modal.modal('hide');
                                } else {
                                    uploadFileData.append("is_remove_file", is_remove_file);
                                    uploadFileData.append("id", $(":input[name='id']", $frm).val());
                                    url = apiUrl("/User/UploadPhoto");
                                    $.ajax({
                                        "url": url,
                                        "cache": false,
                                        "type": "POST",
                                        "contentType": false,
                                        "processData": false,
                                        "data": uploadFileData
                                    }).done(function (ujson) {
                                        self.my().photo_file_id(ujson.id);
                                        self.my().photo_file_name(ujson.file_name);
                                        storage = getAuth();
                                        storage.photo_file_id = ujson.id;
                                        storage.photo_file_name = ujson.file_name;
                                        setAuth(storage);
                                    }).fail(function (response) {
                                        self.errors(generateErrors(response.responseJSON));
                                    }).always(function (jqxhr) {
                                        unblockUI();
                                        $btn.button('reset');
                                        jAlert("Saved");
                                        $modal.modal('hide');
                                    });
                                }


                            }).always(function () {
                            });
                        }
                    });


                    $("#save", "#frmChangePassword").click(function () {
                        var $frm = $("#frmChangePassword");
                        self.my().errors(null);
                        if ($frm.valid()) {
                            var url = apiUrl("/User/ChangePassword");
                            var $btn = $(this);
                            $btn.button("loading");
                            handleBlockUI({ "target": $("body"), "message": "Save..." });
                            $.ajax({
                                "url": url,
                                "cache": false,
                                "type": "POST",
                                "data": $frm.serializeArray()
                            }).done(function (json) {
                                jAlert("Password Changed");
                                self.my().errors(null);
                                $modal.modal('hide');
                            }).fail(function (response) {
                                self.my().errors(generateErrors(response.responseJSON));
                            }).always(function () {
                                $btn.button("reset");
                                unblockUI();
                            });
                        }
                    });

                });
                $modal.modal('show');
            });
        }

        this.getFirstMenu = function () {
            var url = "";
            if (self.my() != null) {
                if (self.my().menus().length > 0) {
                    $.each(self.my().menus(), function (i, m) {
                        if (url == '') {
                            $.each(m.submenus, function (j, smenu) {
                                if (url == '' && smenu.url != '') {
                                    url = SH(smenu.url).replaceAll('#', '').s;
                                }
                            });
                        }
                    });
                }
            }
            return url;
        }

        this.getMenu = function (url) {
            var findMenu = null;
            var checkUrl = "";
            var arr = url.split("/");
            if (arr.length >= 1) {
                checkUrl = $.trim("/" + arr[1]);
            }
            isPass = false;
            $.each(self.my().menus(), function (i, m) {
                $.each(m.submenus, function (j, smenu) {
                    arr = smenu.url.split("/");
                    var menuUrl = '';
                    if (arr.length >= 1) {
                        menuUrl = $.trim("/" + arr[1]);
                    }
                    //console.log('getMenu checkUrl=',checkUrl,'menuUrl=',menuUrl);
                    if (checkUrl.toLowerCase() == menuUrl.toLowerCase()) {
                        findMenu = smenu;
                    }
                })
            });
            return findMenu;
        }

        this.checkUrlAuth = function (roles, url, callback) {
            var isPass = false;
            //console.log('roles=',roles,'url=',url);
            if (roles.length <= 0 && url.indexOf('blank') >= 0) {
                isPass = true;
            } else {
                //console.log('self.my()=',self.my());
                if (self.my() != null) {
                    var currentRole = self.my().role();
                    //console.log('currentRole=',currentRole);
                    if (self.my() != null) {
                        $.each(roles, function (i, roleName) {
                            if (currentRole == roleName) {
                                isPass = true;
                            }
                        });
                    }
                    //console.log('isPass=',isPass);
                    if (currentRole == "CM") {
                        var checkUrl = "";
                        var arr = url.split("/");
                        if (arr.length >= 1) {
                            checkUrl = $.trim("/" + arr[1]);
                        }
                        //console.log('arr=',arr);
                        isPass = false;
                        //console.log('self.my().menus()=',self.my().menus());
                        $.each(self.my().menus(), function (i, m) {
                            //console.log('menu name=',m.name);
                            //console.log('m.submenus=',m.submenus);
                            $.each(m.submenus, function (j, smenu) {
                                //console.log('smenu name=',smenu.name,',url=',smenu.url);
                                arr = smenu.url.split("/");
                                var menuUrl = '';
                                if (arr.length >= 1) {
                                    menuUrl = $.trim("/" + arr[1]);
                                }
                                //console.log('checkUrl=',checkUrl,',menuUrl=',menuUrl);
                                if (checkUrl.toLowerCase() == menuUrl.toLowerCase()) {
                                    console.log('checkUrl=true');
                                    isPass = true;
                                }
                            })
                        });
                    }
                }
            }
            //console.log('before isPass=',isPass);
            if (callback)
                callback(isPass);
        }

        this.checkRole = function (roles, url) {
            self.checkUrlAuth(roles, url, function (isPass) {
                if (isPass == false) {
                    alert("You are not authorized. Please contact administrator");
                    self.logOut();
                }
            });
        }

        //this.checkAuth = function (callback) {
        //    var auth = getAuth();
        //    //if(auth==null||auth==undefined) {
        //    handleBlockUI();
        //    $.ajax({
        //        "url": "/Account/Cliams",
        //        "cache": false,
        //        "type": "POST",
        //        "contentType": false,
        //        "processData": false,
        //    }).done(function (json) {
        //        //console.log('json=',json);
        //        if (cString(json) == '') {
        //            goToLogin();
        //        } else {
        //            setAuth(json);
        //            self.setMy(getAuth());
        //        }
        //        /*if(json.role=="GA" || json.role=="GM")
        //			getGroupName(json.groupids);*/
        //        $(".navbar-header span.name,.navbar-header img").html("");
        //        if (json && json.group_name)
        //            $(".navbar-header span.name").html(json.group_name);
        //        else if (json.role == "EA" || json.role == "EM")
        //            $(".navbar-header span.name").html($(".navbar-header span.EA_name").html());
        //        else {
        //            if (json.role == "AA" || json.role == "AM") {
        //                if (!json.agent_profile_name)
        //                    json.agent_profile_name = '';

        //                if (json.agent_profile_name.length > 1)
        //                    $(".navbar-header span.name").html(json.agent_profile_name);
        //                else
        //                    $(".navbar-header span.name").html(json.agent_name);
        //            }

        //        }
        //        //else
        //        //    getCompanyAddress();

        //        if (callback)
        //            callback();
        //    }).fail(function (response) {
        //    }).always(function (jqxhr) {
        //        unblockUI();
        //    });
        //    // } else {
        //    //     self.setMy(getAuth());
        //    //     if(callback)
        //    //         callback();
        //    //}
        //}

        this.companyDropdown = function (role, event) {
            if ($("#ui-id-1").html() == "")
                $(event.target).parents('div.header-content').find('form span.caret').click();
            else if ($("#ui-id-1").css('display') == "none")
                $("#ui-id-1").css('display', "block");
            else
                $("#ui-id-1").css('display', "none");
        }

    }
}
);
