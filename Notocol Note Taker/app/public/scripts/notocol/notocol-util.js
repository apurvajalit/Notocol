(function (h) {
    'use strict';
    
    var NOTOCOL_ACTIVE_ICON = {
        19: 'images/notocol-19-active.png',
        38: 'images/notocol-38-active.png'
    };
    var NOTOCOL_INACTIVE_ICON = {
        19: 'images/notocol-19-inactive.png',
        38: 'images/notocol-38-inactive.png'
    };

    function NotocolUtil(dependencies) {
        var vm = this;
        var chromeTabs = dependencies.chromeTabs;
        var isAllowedFileSchemeAccess = dependencies.isAllowedFileSchemeAccess;
        var extensionURL = dependencies.extensionURL;
        var hypothesis = dependencies.hypothesis;
        var tabsData = {};
        //var tabsData =  {};
        
        var TAB_STATUS_COMPLETE = 'complete';
        var LOCAL_HTML_URL = 1;
        var WEB_HTML_URL = 2;
        var LOCAL_PDF_URL = 3;
        var WEB_PDF_URL = 4;
        //var SERVER_BASE_URL = "https://notocol.tenet.res.in:8443/";
        var SERVER_BASE_URL = "https://localhost:44301/";
        var userFolderTree = {
            ID: 0,
            Parent: null,
            Name: "Root",
            Children: []
        }
        var userFolderTreeJson = null;
        this.userLoggedIn = false;

        this.install = function () {
            chromeTabs.query({}, function (tabs) {
                tabs.forEach(function (tab) {
                    vm.settabsData(tab);
                });
            });
        }

        function FillChildrenForNode(node, folders)
        {
            node.Children = [];

            for (var i = 0; i < folders.length; i++)
            {
                if(folders[i].parentID != node.ID) continue;
                var childTree = {};
                childTree.ID = folders[i].ID;
                childTree.Name = folders[i].name;
                //childTree.ParentID = node.ID;
                childTree.Parent = node;

                FillChildrenForNode(childTree, folders);
                node.Children.push(childTree);
            }

        }

        var processUserFolders = function (folders) {
            if (folders != null) {
                FillChildrenForNode(userFolderTree,folders); 
            }
        }

        var GenerateUserFolderTreeJson = function () {
            $.ajax({
                url: SERVER_BASE_URL + "Api/Folder/GetUserFolders",
                type: 'Get',
                success: function (data) {
                    processUserFolders(data);
                    userFolderTreeJson = JSON.stringify(JSON.decycle(userFolderTree));
                }
            });
        }
        GenerateUserFolderTreeJson();

        function isPDFURL(url) {
            //console.log("URL Extension Check " + url.split('.').pop().split(/\#|\?/)[0]);
            return (url.split('.').pop().split(/\#|\?/)[0] == "pdf");

            //var index = url.toLowerCase().indexOf('.pdf');
            //if(index < 0) return false;
            //if (url.indexOf("https://www.google.co.in/url") == 0) return false; //Google redirect URL

            //return true;

        }

        function isFileURL(url) {
            return url.indexOf("file:") === 0 || url.indexOf("file=") === 0;
        }

        function getURLType(url) {
            if (isFileURL(tab.url)) {
                if (isPDFURL(tab.url)) return LOCAL_PDF_URL;
                else return LOCAL_HTML_URL;

            } else {
                if (isPDFURL(tab.url)) return WEB_PDF_URL;
                else return WEB_HTML_URL;
            }

            return 0;
        }
        
        function isLocalPDFUrl(url) {
            return url.indexOf(extentionsURL("content/web/viewer.html?file=file")) === 0;
        }

        this.isURLSupported = function(tab) {
            /*Supported Files: Web page: html & pdf; local pdfs and NOT local html */
            var urlType = getURLType(tab.url);
            if (urlType == WEB_HTML_FILE || urlType == WEB_PDF_FILE ||
                       (urlType == LOCAL_PDF_FILE && isAllowedFileSchemeAccess)) {
                return true;
            }else return false;
        }
        var checkForUserPageData = function (tabId) {
            if (!vm.userLoggedIn) return;

          $.ajax({
                url: SERVER_BASE_URL + "Api/Source/GetSourceData",
                type: 'Get',
                data: {
                    URI: tabsData[tabId].uri,
                    Link: tabsData[tabId].url
                },

                success: function (pageData) {
                    if (pageData != null) {
                        var data = pageData.sourceData;
                        var currenttabsData = tabsData[tabId] || {};

                        if (typeof data != "undefined") {
                            if (data.noteCount > 0) {
                                hypothesis.enableHypothesis(tabId);
                                vm.updateAnnotatorStatus(tabId);
                            }
                            currenttabsData.sourceID = data.sourceID;
                            currenttabsData.sourceUserID = data.sourceUserID;
                            currenttabsData.tags = data.tags;
                            currenttabsData.summary = data.summary;
                            currenttabsData.folderData = data.folderData;
                            currenttabsData.noteCount = data.noteCount;
                            currenttabsData.privacy = data.privacy;
                            
                        } else {
                            currenttabsData.sourceID = 0;
                            currenttabsData.sourceUserID = 0;
                            currenttabsData.tags = null;
                            currenttabsData.summary = null;
                            currenttabsData.folderData = 0;
                            currenttabsData.noteCount = 0;
                            currenttabsData.privacy = false;
                        }

                        vm.settabsData({ id: tabId }, currenttabsData);
                        console.log("Setting tabsData for " + tabId + " as " + JSON.stringify(tabsData[tabId]));
                    }
                },

                failure: function () {
                    console.log("Failed to get info for tabID "+tabId);
                }

            });
            return;
        }
      
        this.updateTabURI = function(tabid, uri){
            var checkServer = false;
            if (tabsData[tabid].uri != uri) checkServer = true;
            tabsData[tabid].uri = uri;
            if (checkServer) checkForUserPageData(tabid);
            console.log("Updated uri for tab " + tabid + " as " + uri);
        }
        
        this.settabsData = function (tab, info) {
            
            var checkServer = false;
            

            if (typeof info != "undefined") {
                if (typeof tabsData[tab.id] == "undefined" || tabsData[tab.id].uri != info.uri || tabsData[tab.id].url != info.url) checkServer = true;
                tabsData[tab.id] = info;
            } else {
                if (typeof tab.url == "undefined") return;
                tab.url = tab.url.substring(0, tab.url.indexOf("#") >= 0 ? tab.url.indexOf("#") : tab.url.length);
                var link = tab.url;
                
                var currenttabsData = tabsData[tab.id];
                var isPDF = false;
                if (isPDFURL(link)) {
                    isPDF = true;
                    link = unescape(tab.url.substring(tab.url.indexOf("=") + 1));
                }
                if (isFileURL(link))link = unescape(link);
                 
                if (typeof currenttabsData == "undefined") {
                    tabsData[tab.id] = {
                        //title: tab.title,
                        url: link,
                        uri: link,
                        //faviconUrl: tab.faviconUrl,
                        sourceUserID: 0,
                        sourceID: 0
                    };
                    checkServer = true;
                    //console.log("Setting data for first time for tab "+ tab.id);

                } else {
                    if (link != currenttabsData.uri || link != currenttabsData.url){
                        checkServer = true;
                        currenttabsData.uri = currenttabsData.url = link;
                        
                    }

                    //currenttabsData.favIconUrl = tab.faviconUrl;
                    //currenttabsData.title = tab.title;
                    
                }
                
                
            }
            if (checkServer) checkForUserPageData(tab.id);
            tabsData[tab.id].status = true;
            tabsData[tab.id].tabID = tab.id;
            vm.updateAnnotatorStatus(tab.id);
            if (tabsData[tab.id].sourceUserID > 0) {
                chrome.browserAction.setIcon({ tabId: tab.id, path: NOTOCOL_ACTIVE_ICON });
                chrome.browserAction.setTitle({ tabId: tab.id, title: 'Notocol is active' });
            } else {
                chrome.browserAction.setIcon({ tabId: tab.id, path: NOTOCOL_INACTIVE_ICON });
                chrome.browserAction.setTitle({ tabId: tab.id, title: 'Notocol is inactive' });
            }
            console.log("Tab data for tab " + tab.id + "set as " + JSON.stringify(tabsData[tab.id]));
        }

        function unsettabsData(tabId) {
            delete tabsData[tabId];
        }

        this.gettabsData = function (tab) {
            //console.log("Request for tabsData for " + tab.id);
            return tabsData[tab.id];
        }

        this.getUserFolderTreeJson = function () {
            return userFolderTreeJson;
        }

        this.setUserFolderTreeJson = function (folderTreeJson) {
            userFolderTreeJson = folderTreeJson;
            
        }
        function onTabCreated(tab) {
            // Clear the info in case there is old, conflicting data
            unsettabsData(tab);
        }

        function onTabUpdated(tabId, changeInfo, tab) {
            //var curr_url = "";
            //if (typeof tabsData[tabId] != "undefined") curr_url = tabsData[tabId].uri;
            
            //console.log("Tab updated for tab " + tab.id + " with status " + changeInfo.status + " and url as " + tab.url);
            vm.settabsData(tab);
            if (changeInfo.status !== TAB_STATUS_COMPLETE) {
                return;
            }

            if (isPDFURL(tab.url)) {
                hypothesis.enableHypothesis(tab.id);
                vm.updateAnnotatorStatus(tab.id);
            }
        }

        function onTabRemoved(tabId) {
            unsettabsData(tabId);
        }

        function handleUserLogin() {
            vm.userLoggedIn = true;
            chromeTabs.query({}, function (tabs) {
                tabs.forEach(function (tab) {
                    checkForUserPageData(tab.id);
                });
            });
            
        }

        function handleUserLogout() {
            vm.userLoggedIn = false;
            chromeTabs.query({}, function (tabs) {
                tabs.forEach(function (tab) {
                    vm.settabsData(tab, {
                        url: tabsData[tab.id].url, 
                        uri: tabsData[tab.id].uri,
                    })
                });
            });
        }

        this.updateAnnotatorStatus = function(tabId){
            if (hypothesis.state.isTabActive(tabId))
                tabsData[tabId].annotator = true;
            else tabsData[tabId].annotator = false;

           //console.log("Setting annotator status to " + tabsData[tabId].annotator + " for tab " + tabId)
        }
        
        this.listen = function () {
            chromeTabs.onCreated.addListener(onTabCreated);
            chromeTabs.onUpdated.addListener(onTabUpdated);
            chromeTabs.onRemoved.addListener(onTabRemoved);
            

            CheckUserLoginStatus();
            chrome.runtime.onMessage.addListener(function (message, sender, sendResponse) {
                if (message.type == "PDFInformation") {
                    if (typeof sender.tab.id != "undefined" && typeof tabsData[sender.tab.id] != "undefined") {
                        //console.log("Received urn for tab " + sender.tab.id + "as " + message.urn);
                        vm.updateTabURI(sender.tab.id, message.urn.substring("urn:x-pdf:".length));
                    } else {
                        console.log("Did not find tab with ID " + sender.tab.id);
                    }
                
                }else if(message.type == "OpenTab" && message.link != null){
                    chromeTabs.create({ url:message.link});
                }
            });
            chrome.runtime.onMessageExternal.addListener(function (message, sender, sendResponse) {
                if (message.type == "OpenPDF" && message.link != null) {
                    chromeTabs.create({ url: "content/web/viewer.html?file="+message.link });
                } else if (message.type == "UserLogin") {
                    handleUserLogin();
                } else if (message.type == "UserLogout") {
                    handleUserLogout();
                }
            });

            chrome.runtime.onStartup.addListener(this.install);

        }

        function CheckUserLoginStatus() {
            $.ajax({
                url: SERVER_BASE_URL + "Api/User/IsUserLoggedIn",
                type: 'Get',
                success: function (data) {
                    vm.userLoggedIn = data;
                }
            });
        }

        function SetUserLoginStatus(value) {
            vm.userLoggedIn = false;
        }
    }
    
    h.NotocolUtil = NotocolUtil;
})(window.h || (window.h = {}));