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
      }
  })
  notocolUtil.listen();
  
  //Notocol Specifici Action Handlers
  chrome.extension.onMessage.addListener(function (request, sender, sendResponse) {
      
      console.log("Recieved a message with greeting:" + request.greeting);
      if (request.greeting === "PageDetails") {
          chrome.tabs.query({ active: true }, function (tabs) {
              if (!(tabs.length === 0)) {
                  var getTabInfo = new Promise(function (resolve) {
                      console.log("Created promise getTabInfo");
                      var tabInfo = notocolUtil.getTabInfo(tabs[0]);
                      console.log(tabInfo);
                      tabInfo.url.then = function(tabInfo){
                          console.log(tabInfo);
                          response = {
                              tabID: tabs[0].id,
                              status: true,
                              title: tabInfo.title,
                              url: tabInfo.url.url,
                              link: tabInfo.url.link,
                              faviconUrl: tabs[0].favIconUrl
                          };
                          console.log("Resolving getTabInfo");
                          resolve(response);
                      }
                  });
                  getTabInfo.then = function (response) {
                      console.log("Sending Page Response");
                      sendResponse(response);
                  }
                      
                  return getTabInfo;
                  
              } else
                  sendResponse({ status: false });



          });
          
          
      } else if (request.greeting === "ToggleAnnotation") {
          chrome.tabs.query({ active: true }, function (tabs) {
              if (!(tabs.length === 0)) {
                  //Toggle Hypothesis Sidebar
                  browserExtension.onBrowserActionClicked(tabs[0]);
              }
          });

          //console.log("Sending Response");
          sendResponse({ message: "Toggled Annotation" });
      

      }
      return true;
  });

})();
