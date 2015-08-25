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
  notocolUtil.listen();
  
  //Notocol Specifici Action Handlers
  chrome.extension.onMessage.addListener(function (request, sender, sendResponse) {
      if (request.greeting === "PageDetails") {
          chrome.tabs.query({ active: true }, function (tabs) {
              if (!(tabs.length === 0)) {
                  var tabInfo = notocolUtil.getTabInfo(tabs[0]);
                  
                  if (typeof tabInfo != "undefined") {
                      sendResponse(tabInfo);
                  }
              } 
              sendResponse({ status: false });
          });
          
      } else if (request.greeting === "PageDetailsUpdated") {
          
          if (typeof request.pageInfo.tabID != undefined && request.pageInfo.tabID > 0) {
              notocolUtil.setTabInfo({ id: request.pageInfo.tabID }, request.pageInfo);
          }
      } else if (request.greeting === "ToggleAnnotation") {
          chrome.tabs.query({ active: true }, function (tabs) {
              if (!(tabs.length === 0)) {
                  //Toggle Hypothesis Sidebar
                  browserExtension.onBrowserActionClicked(tabs[0]);
              }
          });
          sendResponse({ message: "Toggled Annotation" });
      }
      return true;
  });

})();
