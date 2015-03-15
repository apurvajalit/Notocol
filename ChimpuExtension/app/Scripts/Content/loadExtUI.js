var popup_root = document.getElementById('chimp_ext_container');
//alert("I am loaded");
if (popup_root == null) {
    //UI is not open yet, open it
    console.log("Loading UI")
    $.get(chrome.extension.getURL("HTML/popup.html"), function (data) {
        
        $($.parseHTML(data)).appendTo('body');
     //   var url = document.getElementById("page_url");
       // var title = document.getElementById("page_title");

        //alert("Title and URL: " + document.title + " " + window.location.href);
        $("#page_url").val(document.title);
        $("#page_title").val(window.location.href);
    });
} else { //UI already opened, close it
    console.log("Unloading UI")
    document.body.removeChild(popup_root);
}
