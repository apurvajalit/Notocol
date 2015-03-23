var clickHandler = function (e) {
    var url = e.pageUrl;
    var buzzPostUrl = "";

    
    // Open the page up.
    alert(url);
};

var SelectionClickHandler = function (e) {
    
    alert(encodeURI(e.selectionText));
};

chrome.contextMenus.create({
    "title": "Page Handler",
    "contexts": ["page", "selection"],
    "onclick": clickHandler
});

chrome.contextMenus.create({
    "title": "Selection Handler",
    "contexts": ["selection"],
    "onclick": SelectionClickHandler
});


chrome.browserAction.onClicked.addListener(function (tab) {
    function fun(i) {
        if (i == 0) chrome.tabs.insertCSS(tab.id, { file: "Style/extUI.css" }, function () { fun(1) });
        if (i == 1) chrome.tabs.insertCSS(tab.id, { file: "Style/jquery-ui.css" }, function () { fun(2) });
        if (i == 2) chrome.tabs.insertCSS(tab.id, { file: "Style/jquery.tagit.css" }, function () { fun(3) });
        if (i == 3) chrome.tabs.executeScript(tab.id, { file: "Scripts/External/jquery-2.1.3.min.js" }, function () { fun(4) });
        if (i == 4) chrome.tabs.executeScript(tab.id, { file: "Scripts/External/jquery-ui.min.js" }, function () { fun(5) });
        if (i == 5) chrome.tabs.executeScript(tab.id, { file: "Scripts/External/tag-it.js" }, function () { fun(6) });
        if (i == 6) chrome.tabs.executeScript(tab.id, { file: "Scripts/Content/loadExtUI.js" }, function () { fun(7) });
        if (i == 7) chrome.tabs.executeScript(tab.id, { code: "$('#myTags').tagit();" }, function () { fun(8) });
        else;
    }
    fun(0);
});


chrome.runtime.onMessage.addListener(
  function (request, sender, sendResponse) {
      console.log(sender.tab ?
                  "from a content script:" + sender.tab.url :
                  "from the extension");
      if (request.greeting == "details") {
          console.log("Received data: " + request.tagText);

      } else if (request.greeting == "hello") {
          console.log("Message from script:" + request.message);
      }
      sendResponse({ farewell: "goodbye" });    
  }
);
