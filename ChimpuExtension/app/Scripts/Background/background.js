chrome.browserAction.onClicked.addListener(function (tab) {
    //Called when the user clicks on the browser action icon.    
    chrome.tabs.insertCSS(tab.id, { file: "Style/extUI.css" }, function () {
            console.log("Inserted CSS using file Style/extUI.css") //This alert works
    });
    console.log("Executing content scripts");
    //TODO Change the following. It is not scalable to introduce more content scripts
    chrome.tabs.executeScript(tab.id, { file: "Scripts/External/jquery-2.1.3.min.js" }, function () {
        chrome.tabs.executeScript(tab.id, { file: "Scripts/Content/loadExtUI.js" }, function () {
            chrome.tabs.executeScript(tab.id, { file: "Scripts/Content/sendDetails.js" });
        });
    });
});

alert("Adding listener at bg")
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
