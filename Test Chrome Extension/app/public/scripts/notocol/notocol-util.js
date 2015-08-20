(function (h) {
    'use strict';
    function NotocolUtil(dependencies) {

        var chromeTabs = dependencies.chromeTabs;
        var isAllowedFileSchemeAccess = dependencies.isAllowedFileSchemeAccess;
        var extensionURL = dependencies.extensionURL;
        var tabInfo = tabInfo || {};;

        var TAB_STATUS_COMPLETE = 'complete';
        var LOCAL_HTML_URL = 1;
        var WEB_HTML_URL = 2;
        var LOCAL_PDF_URL = 3;
        var WEB_PDF_URL = 4;


        function isPDFURL(url) {
            return url.toLowerCase().indexOf('.pdf') > 0;
        }
        function isFileURL(url) {
            return url.indexOf("file:") === 0;
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
            return url.indexOf("chrome-extension://namhfjepbaaecpmpgehfppgnhhgaflne/content/web/viewer.html?file=file") === 0;
        }

        this.isURLSupported = function(tab) {
            /*Supported Files: Web page: html & pdf; local pdfs and NOT local html */
            var urlType = getURLType(tab.url);
            if (urlType == WEB_HTML_FILE || urlType == WEB_PDF_FILE ||
                       (urlType == LOCAL_PDF_FILE && isAllowedFileSchemeAccess)) {
                return true;
            }else return false;
        }
           
        function setTabInfo(tab) {
            var urlInfo = {}, link = tab.url;

            if (isPDFURL(link)) link = unescape(tab.url.substring(tab.url.indexOf("=") + 1));
            if (isFileURL(link))link = unescape(link);

            urlInfo.link = urlInfo.url = link;
            tabInfo[tab.id] = {
                title : tab.title,
                url : urlInfo,
                faviconUrl : tab.favIconUrl
            };
            
            console.log("Setting tabinfo for "+tab.id+" as "+JSON.stringify(tabInfo[tab.id]));

            
        }

        function unsetTabInfo(tabId) {
            delete tabInfo[tabId];
        }

        this.getTabInfo = function (tab) {
            //console.log("Request for tabInfo for " + tab.id);
            return tabInfo[tab.id];
        }

        function onTabCreated(tab) {
            // Clear the info in case there is old, conflicting data
            unsetTabInfo(tab);
        }

        function onTabUpdated(tabId, changeInfo, tab) {
            //if (changeInfo.status !== TAB_STATUS_COMPLETE) {
            //    return;
            //}
            
            setTabInfo(tab);
        }

        function onTabRemoved(tabId) {
            unsetTabInfo(tabId);
        }

        this.listen = function () {
            chromeTabs.onCreated.addListener(onTabCreated);
            chromeTabs.onUpdated.addListener(onTabUpdated);
            chromeTabs.onRemoved.addListener(onTabRemoved);

            chrome.runtime.onMessage.addListener(function (request, sender, sendResponse) {
                //console.log(sender.tab);
                if (request.greeting == "pdfUrn") {
                    //console.log("Received data from script at tab:" + sender.tab.id + " as " + request.data);
                    if (typeof tabInfo[sender.tab.id] != "undefined") {
                        tabInfo[sender.tab.id].url.url = request.data;
                        console.log("Updated tabInfo for "+sender.tab.id+" as "+ JSON.stringify(tabInfo[sender.tab.id]));
                    } else {
                        console.log("Did not find tab with ID " + sender.tab.id);
                    }
                    //sendResponse({ farewell: "goodbye" });
                }
            });
        }
    }
    
    h.NotocolUtil = NotocolUtil;
})(window.h || (window.h = {}));