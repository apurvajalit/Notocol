(function () {
  'use strict';

  var browserExtension = new h.HypothesisChromeExtension({
    chromeTabs: chrome.tabs,
    chromeBrowserAction: chrome.browserAction,
    extensionURL: function (path) {
      return chrome.extension.getURL(path);
    },
    isAllowedFileSchemeAccess: function (fn) {
      return chrome.extension.isAllowedFileSchemeAccess(fn);
    },
  });

  browserExtension.listen(window);
  chrome.runtime.onInstalled.addListener(onInstalled);
  chrome.runtime.requestUpdateCheck(function (status) {
    chrome.runtime.onUpdateAvailable.addListener(onUpdateAvailable);
  });

  function onInstalled(installDetails) {
    if (installDetails.reason === 'install') {
      browserExtension.firstRun();
    }

    // We need this so that 3rd party cookie blocking does not kill us.
    // See https://github.com/hypothesis/h/issues/634 for more info.
    // This is intended to be a temporary fix only.
    var details = {
      primaryPattern: 'https://hypothes.is/*',
      setting: 'allow'
    };
    chrome.contentSettings.cookies.set(details);
    chrome.contentSettings.images.set(details);
    chrome.contentSettings.javascript.set(details);

    browserExtension.install();
  }

  function onUpdateAvailable() {
    chrome.runtime.reload();
  }

  var notocolUtil = new h.NotocolUtil({
      chromeTabs: chrome.tabs,
      extensionURL: function (path) {
          return chrome.extension.getURL(path);
      },
      isAllowedFileSchemeAccess: function (fn) {
          return chrome.extension.isAllowedFileSchemeAccess(fn);
      },
      hypothesis: browserExtension
  })
  

  chrome.runtime.onMessageExternal.addListener(
    function (request, sender, sendResponse) {
      
        if (request.reloadExtension)
            chrome.runtime.reload();
  });
  //Notocol Specifici Action Handlers
  chrome.extension.onMessage.addListener(function (request, sender, sendResponse) {
      if (request.greeting === "PageDetails") {
          //if (!notocolUtil.userLoggedIn) {
          //    //Redirect to login
          //    chrome.tabs.create({ url: "https://localhost:44301/User/Login" });
          //}
          chrome.tabs.query({ active: true }, function (tabs) {
              if (!(tabs.length === 0)) {
                  var tabInfo = notocolUtil.gettabsData(tabs[0]);
                  var userFolderTreeJson = notocolUtil.getUserFolderTreeJson();
                  if (typeof tabInfo != "undefined") {
                      sendResponse({ "tabInfo": tabInfo, "userFolderTreeJson" : userFolderTreeJson });
                  } else {
                      sendResponse({ status: false });
                  }
              } else {
                  sendResponse({ status: false });
              }
              
          });
          
      } else if (request.greeting === "PageDetailsUpdated") {
          
          if (typeof request.pageInfo.tabID != undefined && request.pageInfo.tabID > 0) {
              notocolUtil.settabsData({ id: request.pageInfo.tabID }, request.pageInfo);
          }
      } else if (request.greeting === "ToggleAnnotation") {
          chrome.tabs.query({ active: true }, function (tabs) {
              if (!(tabs.length === 0)) {
                  //Toggle Hypothesis Sidebar
                  browserExtension.onBrowserActionClicked(tabs[0]);
                  notocolUtil.updateAnnotatorStatus(tabs[0].id);
              }
          });
          sendResponse({ message: "Toggled Annotation" });
      } else if (request.greeting === "UserFoldersUpdated") {
          notocolUtil.setUserFolderTreeJson(request.userFolderTree);
      }
      return true;
  });
  notocolUtil.listen();

})();
