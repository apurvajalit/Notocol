//Checking message passing to background

console.log("Sending message to bg");
chrome.runtime.sendMessage({ greeting: "hello" }, function (response) {
    console.log(response.farewell);
});