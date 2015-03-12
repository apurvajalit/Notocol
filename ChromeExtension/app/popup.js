$(document).ready(function() {
    $(".basic").jRating({
        type: 'big', // type of the rate.. can be set to 'small' or 'big'
        decimalLength: 1
    });
    $("#txtTags").on('change keyup paste', function() {
        if (event.which == 188)// check for ',' on key press
        {
            FilterTags($(this).val());
        }
    });

    $("#txtTags").on('itemAdded', function(event) {
        // event.item: contains the item
        tempTags.tags.push(
            { id: 0, natagme: event.item }
        );
    });
    $("#btnSave").on('click', function(event) {
        var title = $("#txtTitle").val();
        var rating = $("#txtTags").val();
        var link = $("#txtUrl").val();
        var summary = $("#txtSummary").val();
        var readLater = $("#chkReadLater");
        var SaveOfflineCopy = $("#chkSaveOfflineCopy");
        var privacy = $("#chkPrivacy");
        var frmJson = $('#frmBookmark').formParams();
        console.log(frmJson);

    });
});

var tagValues = ['Amsterdam', 'Washington', 'Sydney', 'Beijing', 'Cairo'];

var tagVals = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    local: $.map(tagValues, function (tagValue) { return { value: tagValue }; })
});

// kicks off the loading/processing of `local` and `prefetch`
tagVals.initialize();
$("#txtTags").tagsinput({
    typeahead: {
        name: 'tagVals',
        displayKey: 'value',
        valueKey: 'value',
        source: tagVals.ttAdapter()
    }
});

// This callback function is called when the content script has been 
// injected and returned its results
function onPageDetailsReceived(pageDetails) {
    document.getElementById('title').value = pageDetails.title;
    document.getElementById('url').value = pageDetails.url;
    document.getElementById('summary').innerText = pageDetails.summary;
}

// Global reference to the status display SPAN
var statusDisplay = null;

 function SaveBookmark() {
     
 }
// POST the data to the server using XMLHttpRequest
function addBookmark() {
    // Cancel the form submit
    event.preventDefault();

    // The URL to POST our data to
    var postUrl = 'http://';

    // Set up an asynchronous AJAX POST request
    var xhr = new XMLHttpRequest();
    xhr.open('POST', postUrl, true);

    // Prepare the data to be POSTed by URLEncoding each field's contents
    var title = encodeURIComponent(document.getElementById('title').value);
    var url = encodeURIComponent(document.getElementById('url').value);
    var summary = encodeURIComponent(document.getElementById('summary').value);
    var tags = encodeURIComponent(document.getElementById('tags').value);

    var params = 'title=' + title +
                 '&url=' + url +
                 '&summary=' + summary +
                 '&tags=' + tags;

    // Replace any instances of the URLEncoded space char with +
    params = params.replace(/%20/g, '+');

    // Set correct header for form data 
    xhr.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');

    // Handle request state change events
    xhr.onreadystatechange = function () {
        // If the request completed
        if (xhr.readyState == 4) {
            statusDisplay.innerHTML = '';
            if (xhr.status == 200) {
                // If it was a success, close the popup after a short delay
                statusDisplay.innerHTML = 'Saved!';
                window.setTimeout(window.close, 1000);
            } else {
                // Show what went wrong
                statusDisplay.innerHTML = 'Error saving: ' + xhr.statusText;
            }
        }
    };

    // Send the request and set status
    xhr.send(params);
    statusDisplay.innerHTML = 'Saving...';
}

// When the popup HTML has loaded
window.addEventListener('load', function (evt) {

    // Cache a reference to the status display SPAN
    statusDisplay = document.getElementById('status-display');
    // Handle the bookmark form submit event with our addBookmark function
    document.getElementById('addbookmark').addEventListener('submit', addBookmark);
    // Get the event page
    chrome.runtime.getBackgroundPage(function (eventPage) {
        // Call the getPageInfo function in the event page, passing in 
        // our onPageDetailsReceived function as the callback. This injects 
        // content.js into the current tab's HTML
        eventPage.getPageDetails(onPageDetailsReceived);
    });
});

// global Json Object
var tempTags = {
    tags: []
 };
// Add tags to Json Array 
 function AddTempTag(tempID,text) {
     tempTags.tags.push(
         { id: tempID, natagme: text }
     );
 }


// This function is called when user press ',' in tags textbox
 function FilterTags(text) {
     var str='';
     if (text.indexOf(',') > -1) // check for ',' in text
     {
         var strArray = text.split(',');
         str = strArray[strArray.length-2];
     }
     tempTags.tags.push(
         { id: 0, tagname: str }
     );
     //alert(str);
 }


