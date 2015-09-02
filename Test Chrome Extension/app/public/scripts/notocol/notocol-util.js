(function (h) {
    'use strict';
    function NotocolUtil(dependencies) {
        var vm = this;
        var chromeTabs = dependencies.chromeTabs;
        var isAllowedFileSchemeAccess = dependencies.isAllowedFileSchemeAccess;
        var extensionURL = dependencies.extensionURL;
        var hypothesis = dependencies.hypothesis;
        var tabInfo =  {};

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
            //if (typeof tabInfo[tabId].pageCheckRequest != "undefined")
              //  tabInfo[tabId].pageCheckRequest.abort();

            //tabInfo[tabId].pageCheckRequest =
            $.ajax({
                url: SERVER_BASE_URL + "Api/Source/SourceData",
                type: 'Get',
                data: {
                    pageURL: tabInfo[tabId].url
                },

                success: function (pageData) {
                    if (pageData != null) {
                        if (typeof pageData.sourceID != "undefined" && pageData.sourceID != 0) {
                            if (pageData.noteCount > 0) {
                                hypothesis.enableHypothesis(tabId);
                                vm.updateAnnotatorStatus(tabId);
                            }
                            vm.setTabInfo({ id: tabId }, pageData);
                            //console.log("Setting tabInfo for " + tabId + " as " + JSON.stringify(tabInfo[tabId]));
                        }
                    }
                },

                failure: function () {
                    console.log("Failed to get info for tabID "+tabId);
                }

            });
            return;
        }
        
        this.setTabInfo = function(tab, info) {
            if (typeof info != "undefined") {
                tabInfo[tab.id] = info;
                console.log("Updated tabInfo for tab " + tab.id + " as " + JSON.stringify(tabInfo[tab.id]));
            }else{
                var link = tab.url;

                if (isPDFURL(link)) link = unescape(tab.url.substring(tab.url.indexOf("=") + 1));
                if (isFileURL(link))link = unescape(link);

                tabInfo[tab.id] = {
                    title : tab.title,
                    url: link,
                    faviconUrl: tab.favIconUrl,
                    
                };
                checkForUserPageData(tab.id);
            }

            tabInfo[tab.id].status = true;
            tabInfo[tab.id].tabID = tab.id;
            vm.updateAnnotatorStatus(tab.id);
        }

        function unsetTabInfo(tabId) {
            delete tabInfo[tabId];
        }

        this.getTabInfo = function (tab) {
            console.log("Request for tabInfo for " + tab.id);
            return tabInfo[tab.id];
        }

        function onTabCreated(tab) {
            // Clear the info in case there is old, conflicting data
            unsetTabInfo(tab);
        }

        function onTabUpdated(tabId, changeInfo, tab) {
            
            vm.setTabInfo(tab);
            if (changeInfo.status !== TAB_STATUS_COMPLETE) {
                return;
            }

            if (isPDFURL(tab.url)) {
                hypothesis.enableHypothesis(tab.id);
                vm.updateAnnotatorStatus(tab.id);
            }
        }

        function onTabRemoved(tabId) {
            unsetTabInfo(tabId);
        }

        this.updateAnnotatorStatus = function(tabId){
            if (hypothesis.state.isTabActive(tabId))
                tabInfo[tabId].annotator = true;
            else tabInfo[tabId].annotator = false;

            console.log("Setting annotator status to " + tabInfo[tabId].annotator + " for tab " + tabId)
        }

        this.listen = function () {
            chromeTabs.onCreated.addListener(onTabCreated);
            chromeTabs.onUpdated.addListener(onTabUpdated);
            chromeTabs.onRemoved.addListener(onTabRemoved);

            
            chrome.runtime.onMessage.addListener(function (message, sender, sendResponse) {
                if (message.type == "PDFInformation") {
                    if (typeof sender.tab.id != "undefined" && typeof tabInfo[sender.tab.id] != "undefined") {
                        var info = tabInfo[sender.tab.id]
                        info.urn = message.urn.substring("urn:x-pdf:".length);
                        console.log("Received urn for tab " + sender.tab.id + "as " + message.urn);
                        
                        vm.setTabInfo(sender.tab, info);
                    } else {
                        console.log("Did not find tab with ID " + sender.tab.id);
                    }
                
                }
            });
        }
    }
    
    h.NotocolUtil = NotocolUtil;
})(window.h || (window.h = {}));