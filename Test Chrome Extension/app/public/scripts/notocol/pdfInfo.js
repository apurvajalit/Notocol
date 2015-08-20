var port = chrome.runtime.connect();
window.addEventListener("message", function (event) {
    // We only accept messages from ourselves
    if (event.source != window)
        return;

    if (event.data.type && (event.data.type == "PDFInformation")) {
        console.log("Content script received: " + event.data.urn + ". Sending to background script");
        //port.postMessage(event.data.urn);

        chrome.runtime.sendMessage({ greeting: "pdfUrn", data: event.data.urn });
        //, function (response) {
        //    console.log(response.farewell);
        //});

    }
}, false);

//if (document.URL.indexOf(".pdf") < 0){
////    console.log("Injecting script");
//    var script = document.createElement("script");
//    script.src = "chrome-extension://namhfjepbaaecpmpgehfppgnhhgaflne/public/scripts/notocol/SendPdfUrn.js";
//    document.body.appendChild(script);
//}