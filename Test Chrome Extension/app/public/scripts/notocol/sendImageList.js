function GetAllImageURLs() {
    
    var imageObjects = [];
    var images = document.getElementsByTagName('img')
    for(var i = 0; i< images.length; i++){
        
        imageObjects.push({
            url: images[i].currentSrc,
            height: images[i].clientHeight,
            width: images[i].clientWidth,
            hidden: images[i].hidden
        });
    }
    
    return imageObjects;
}

function GetPageText() {
    
    var pageTextElements = [];

    var pElements = document.getElementsByTagName('p');

    for(var i = 0; (i < 10) && (i< pElements.length); i++){
            pageTextElements.push(pElements[i].innerText);
    }
   
    return pageTextElements;

}

console.log("I am running to send thumbnailData");
var imageObjects = JSON.stringify(GetAllImageURLs());
var pageText = JSON.stringify(GetPageText());
var thumbnailData = {
    pageURI: inputVariables.sourceURI,
    imageObjects: imageObjects,
    textData: pageText
}
var xhr = new XMLHttpRequest();

xhr.open("POST", inputVariables.ThumbNailDataURl, true);
xhr.setRequestHeader("Content-Type", "application/json");
xhr.setRequestHeader("dataType", "json");
xhr.send(JSON.stringify(thumbnailData));


//$.ajax({
//    url: inputVariables.ThumbNailDataURl,
//    type: "POST",
//    data: JSON.stringify(thumbnailData),
//    contentType:"application/json",
//    dataType: "json",
//    success: function (data) {
//        console.log(data);
//    }
//})