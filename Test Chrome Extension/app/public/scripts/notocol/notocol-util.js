(function (h) {
    'use strict';
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
        var SERVER_BASE_URL = "https://localhost:44301/";

        function isPDFURL(url) {
            return url.toLowerCase().indexOf('.pdf') > 0;
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

                        if (data != "undefined") {
                            if (data.noteCount > 0) {
                                hypothesis.enableHypothesis(tabId);
                                vm.updateAnnotatorStatus(tabId);
                            }
                            currenttabsData.sourceID = data.sourceID;
                            currenttabsData.sourceUserID = data.sourceUserID;
                            currenttabsData.tags = data.tags;
                            currenttabsData.summary = data.summary;
                            currenttabsData.folder = data.folder;
                            currenttabsData.noteCount = data.noteCount;
                            currenttabsData.privacy = data.privacy;
                        } else {
                            currenttabsData.sourceID = 0;
                            currenttabsData.sourceUserID = 0;
                            currenttabsData.tags = null;
                            currenttabsData.summary = null;
                            currenttabsData.folder = 0;
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
            if (tab.url == "undefined") return;

            if (typeof info != "undefined") {
                if (typeof tabsData[tab.id] == "undefined" || tabsData[tab.id].uri != info.uri || tabsData[tab.id].url != info.url) checkServer = true;
                tabsData[tab.id] = info;
            }else{
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
                        title: tab.title,
                        url: link,
                        uri: link,
                        faviconUrl: tab.faviconUrl,
                        sourceUserID: 0,
                        sourceID: 0
                    };
                    checkServer = true;
                    console.log("Setting data for first time for tab "+ tab.id);

                } else {
                    if (!isPDF && (link != currenttabsData.uri || link != currenttabsData.url)){
                        checkServer = true;
                        currenttabsData.uri = currenttabsData.url = link;
                        
                    }
                    currenttabsData.favIconUrl = tab.faviconUrl;
                    currenttabsData.title = tab.title;
                    
                }
                
                
            }
            if (checkServer) checkForUserPageData(tab.id);
            tabsData[tab.id].status = true;
            tabsData[tab.id].tabID = tab.id;
            vm.updateAnnotatorStatus(tab.id);
        }

        function unsettabsData(tabId) {
            delete tabsData[tabId];
        }

        this.gettabsData = function (tab) {
            console.log("Request for tabsData for " + tab.id);
            return tabsData[tab.id];
        }

        function onTabCreated(tab) {
            // Clear the info in case there is old, conflicting data
            unsettabsData(tab);
        }

        function onTabUpdated(tabId, changeInfo, tab) {
            var curr_url = "";
            if (typeof tabsData[tabId] != "undefined") curr_url = tabsData[tabId].uri;

            console.log("Tab updated for tab " + tab.id + " with status " + changeInfo.status + "  " + tab.url + "/" + curr_url);
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

        this.updateAnnotatorStatus = function(tabId){
            if (hypothesis.state.isTabActive(tabId))
                tabsData[tabId].annotator = true;
            else tabsData[tabId].annotator = false;

            console.log("Setting annotator status to " + tabsData[tabId].annotator + " for tab " + tabId)
        }

        this.listen = function () {
            chromeTabs.onCreated.addListener(onTabCreated);
            chromeTabs.onUpdated.addListener(onTabUpdated);
            chromeTabs.onRemoved.addListener(onTabRemoved);

            
            chrome.runtime.onMessage.addListener(function (message, sender, sendResponse) {
                if (message.type == "PDFInformation") {
                    if (typeof sender.tab.id != "undefined" && typeof tabsData[sender.tab.id] != "undefined") {
                        console.log("Received urn for tab " + sender.tab.id + "as " + message.urn);
                        vm.updateTabURI(sender.tab.id, message.urn.substring("urn:x-pdf:".length));
                    } else {
                        console.log("Did not find tab with ID " + sender.tab.id);
                    }
                
                }
            });
        }
    }
    
    h.NotocolUtil = NotocolUtil;
})(window.h || (window.h = {}));