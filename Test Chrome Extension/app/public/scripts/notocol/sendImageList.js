function GetAllImageURLs() {
    
    var imageObjects = [];
    $('img').each(function () {
        imageObjects.push({
            url: this.currentSrc,
            height: this.clientHeight,
            width: this.clientWidth,
            hidden: this.hidden
        });
    })
    
    return imageObjects;
}

function GetPageText() {
    
    var pageTextElements = [];
    $("p").slice(0, 10).each(function() {
        pageTextElements.push(this.innerText);
    });
   
    return pageTextElements;

}


var imageObjects = JSON.stringify(GetAllImageURLs());
var pageText = JSON.stringify(GetPageText());

$.ajax({
    url: inputVariables.imageListURl,
    type: "POST",
    data: {
        "pageURL": inputVariables.sourceURI,
        "imageObjects": imageObjects,
        "textData": pageText
    },
    dataType: "json",
    success: function (data) {
        console.log(data);
    }
})