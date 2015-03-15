var popup_root = document.getElementById('chimp_ext_container');
//alert("I am loaded");
if (popup_root == null) {
    //UI is not open yet, open it
    console.log("Loading UI")
    $.get(chrome.extension.getURL("HTML/popup.html"), function (data) {
        
        $($.parseHTML(data)).appendTo('body');
       
        alert("Title and URL: " + document.title + " " + window.location.href);
        $("#page_title").val(document.title);
        $("#page_url").val(window.location.href);

        $('#page_tags').keyup(function () {
            console.log("Sending entered tag text to bg as " + $('#page_tags').val());
            chrome.runtime.sendMessage({ greeting: "details", tagText: $('#page_tags').val() }, function (response) {
                console.log(response.farewell);
            });
        });

        console.log("Adding listener to save Button")
        $('#saveSource').on("click", function () {


            var url = $("#page_url").val();
            var note = $("#page_summary").val();
            var tagnames = $("#page_tag").val().split(",");
            var title = $("#page_title").val();
            alert("Sending data url=" + url + "Note=" + note + "tags=" + $("#page_tag").val() + "title=" + title);
            var tagData = [];

            for (var i = 0; i < tagnames.length; i++) {
                tagData[tagData.length] = {
                    "ID": "0",
                    "Name": tagnames[i],
                    "ParentID": "1",
                    "UserID": "2"
                };
            }

            var srcData = {
                "Source": {
                    "ID": 0,
                    "UserID": 2,
                    "Title": title,
                    "Link": url,
                    "Summary": note,
                    "ReadLater": false,
                    "SaveOffline": false,
                    "Privacy": false,
                    "Rating": 0,
                    "ModifiedAt": Date()
                },
                "Tags": tagData
            }

            // Script to add Source to database.
            $.ajax({
                type: "POST",
                dataType: "application/json",
                data: srcData,
                url: 'http://localhost:5555/api/Source',
                success: function (data) {
                    alert(data);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(errorThrown);
                }
            });
        });


    });
} else { //UI already opened, close it
    console.log("Unloading UI")
    document.body.removeChild(popup_root);
}
