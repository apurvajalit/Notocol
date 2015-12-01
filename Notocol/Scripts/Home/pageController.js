app.directive('fancybox', function ($compile, $http) {
    return {
        restrict: 'A',

        controller: function ($scope) {
            $scope.openFancybox = function (url) {

                $http.get(url).then(function (response) {
                    if (response.status == 200) {

                        var template = angular.element(response.data);
                        var compiledTemplate = $compile(template);
                        compiledTemplate($scope);

                        $.fancybox.open({ content: template, type: 'html' });
                    }
                });
            };
        }
    };
});
app.directive("pageTile", function () {
    return {
        restrict: 'E',
        scope: {
            page: '=',
            openPageDetails: '&'
        },

        templateUrl: "/Templates/Source/PageTile"
    };
});
app.directive('imageonload', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.bind('load', function () {
                $scope.updateLayout();
            });
        }
    };
});
app.directive('sumoSelectBox', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).SumoSelect({ csvDispCount: 3 });
        }
    };
})

var pageController = app.controller("PageController", function ($scope, $http, $compile) {
    $scope.filter = {};
    $scope.loadCompletePending = 0;
    $scope.pageList = [];
    $scope.loading = true;
    $scope.loadingError = false;
    $scope.tabSelected = 'own';
    $scope.initCollectionTree = function () {
        ddtreemenu.createTree("treemenu1", true)
    }
    var sourceDetailTemplateURL = '/Templates/Source/SourceDetails';
    var sourceDetailDataURL = '/Source/GetSourceUserWithNotes?sourceUserID='
    $scope.tilefunc = function (messsage) {
        
    }

    $scope.openPageDetails = function (page) {
        console.log("received page as " + page.title);
        //$scope.selectedPage = $scope.pageList[0];
        $scope.sourceDetails = page;
        $http.get(sourceDetailTemplateURL).then(function (response) {
            if (response.status == 200) {

                var template = angular.element(response.data);
                var compiledTemplate = $compile(template);
                compiledTemplate($scope);

                $.fancybox.open({ content: template, type: 'html' });
            }
        });

        $http.get(sourceDetailDataURL + page.sourceUserID).then(function (response) {
            console.log(response);
            $scope.sourceDetails = response.data;
            $scope.sourceDetails = { "sourceUser": { "ID": 96, "UserID": 6, "Summary": "This book provides a good way of understanding social networking dynamics backed by substantial data and maths", "Privacy": false, "Rating": null, "ModifiedAt": "2015-10-08T22:31:20.023", "thumbnailImageUrl": null, "thumbnailText": null, "FolderID": 21, "noteCount": 9, "SourceID": 79, "PrivacyOverride": null, "PrivateNoteCount": 0, "Folder": { "name": "Social Network", "ID": 21, "description": null, "parentID": 20, "created": "2015-10-08T21:44:39.21", "updated": "2015-10-08T21:44:39.21", "userID": 6, "SourceUsers": [], "User": { "ID": 6, "Username": "apurva", "Password": "welcome", "Identifier": null, "ModifiedAt": "2015-10-05T13:12:58.987", "Email": "apurva@tenet.res.in", "Provider": null, "Gender": "\u0000", "DOB": "0001-01-01T00:00:00", "Address": null, "Name": null, "Photo": null, "Annotations": [], "Folders": [], "SourceUsers": [], "UserTagUsages": [], "Follows": [], "Follows1": [], "Notifications": [], "Notifications1": [] } }, "Source": null, "User": { "ID": 6, "Username": "apurva", "Password": "welcome", "Identifier": null, "ModifiedAt": "2015-10-05T13:12:58.987", "Email": "apurva@tenet.res.in", "Provider": null, "Gender": "\u0000", "DOB": "0001-01-01T00:00:00", "Address": null, "Name": null, "Photo": null, "Annotations": [], "Folders": [{ "name": "Social Network", "ID": 21, "description": null, "parentID": 20, "created": "2015-10-08T21:44:39.21", "updated": "2015-10-08T21:44:39.21", "userID": 6, "SourceUsers": [] }], "SourceUsers": [], "UserTagUsages": [], "Follows": [], "Follows1": [], "Notifications": [], "Notifications1": [] } }, "source": { "ID": 79, "url": "https://www.cs.cornell.edu/home/kleinber/networks-book/networks-book.pdf", "uriHash": "NMh0Vr6aqjoELg1tpiiIvqE2m9YAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==", "faviconURL": null, "title": "networks-book.pdf", "created": "2015-10-08T00:26:10.783", "Annotations": [], "SourceUsers": [{ "ID": 96, "UserID": 6, "Summary": "This book provides a good way of understanding social networking dynamics backed by substantial data and maths", "Privacy": false, "Rating": null, "ModifiedAt": "2015-10-08T22:31:20.023", "thumbnailImageUrl": null, "thumbnailText": null, "FolderID": 21, "noteCount": 9, "SourceID": 79, "PrivacyOverride": null, "PrivateNoteCount": 0, "Folder": { "name": "Social Network", "ID": 21, "description": null, "parentID": 20, "created": "2015-10-08T21:44:39.21", "updated": "2015-10-08T21:44:39.21", "userID": 6, "SourceUsers": [], "User": { "ID": 6, "Username": "apurva", "Password": "welcome", "Identifier": null, "ModifiedAt": "2015-10-05T13:12:58.987", "Email": "apurva@tenet.res.in", "Provider": null, "Gender": "\u0000", "DOB": "0001-01-01T00:00:00", "Address": null, "Name": null, "Photo": null, "Annotations": [], "Folders": [], "SourceUsers": [], "UserTagUsages": [], "Follows": [], "Follows1": [], "Notifications": [], "Notifications1": [] } }, "Source": null, "User": { "ID": 6, "Username": "apurva", "Password": "welcome", "Identifier": null, "ModifiedAt": "2015-10-05T13:12:58.987", "Email": "apurva@tenet.res.in", "Provider": null, "Gender": "\u0000", "DOB": "0001-01-01T00:00:00", "Address": null, "Name": null, "Photo": null, "Annotations": [], "Folders": [{ "name": "Social Network", "ID": 21, "description": null, "parentID": 20, "created": "2015-10-08T21:44:39.21", "updated": "2015-10-08T21:44:39.21", "userID": 6, "SourceUsers": [] }], "SourceUsers": [], "UserTagUsages": [], "Follows": [], "Follows1": [], "Notifications": [], "Notifications1": [] } }], "Notifications": [] }, "userNotes": { "Model.Extended.UserKey": [{ "NoteText": "This book provides a good way of understanding social networking dynamics backed by substantial data and maths", "QuotedText": null, "id": 0, "username": null, "updated": "2015-10-08T22:31:20.023", "tags": null }], "Model.Extended.UserKey": [{ "NoteText": null, "QuotedText": " Graph theoryis the study of network structure, while game theory provides models of individual behavior", "id": 42, "username": "apurva", "updated": "2015-10-08T00:33:10", "tags": [] }, { "NoteText": "Mathematical model to understand the network complexities and also to establish a language to talk about it", "QuotedText": "Graph Theory", "id": 44, "username": "apurva", "updated": "2015-10-08T12:50:26", "tags": ["mathematical-model"] }, { "NoteText": "To understand effects of player's (user's ) responses on others in the network", "QuotedText": "Game  Theory", "id": 45, "username": "apurva", "updated": "2015-10-08T12:51:27", "tags": ["social", "network", "human-behavior"] }, { "NoteText": "Triadic Closure", "QuotedText": "If  two  people  in  a  social  network  have  a  friend  in  common,  then  there  is  anincreased likelihood that they will become friends themselves at some point in thefuture", "id": 47, "username": "apurva", "updated": "2015-10-08T20:20:36", "tags": [] }, { "NoteText": "Strength of weak ties in social network: Weak ties or distant ties can bring in some information that might not usually come from your set of close friends", "QuotedText": " The closely-knit groups thatyou belong to, though they are \flled with people eager to help, are also \flled with peoplewho know roughly the same things that you do", "id": 48, "username": "apurva", "updated": "2015-10-08T20:30:14", "tags": [] }, { "NoteText": null, "QuotedText": "tronger links represent closerfriendship and greater frequency of interaction", "id": 49, "username": "apurva", "updated": "2015-10-08T20:31:33", "tags": [] }, { "NoteText": "1. Study of triadic closures and strong triadic closure properties could be useful in determing friend suggestions in notocol\n2. ", "QuotedText": null, "id": 50, "username": "apurva", "updated": "2015-10-08T20:39:14", "tags": [] }, { "NoteText": null, "QuotedText": "dual roleas weak connections but also valuable conduits to hard-to-reach parts of the network | thisis the surprising strength of weak ties", "id": 51, "username": "apurva", "updated": "2015-10-08T21:28:45", "tags": [] }, { "NoteText": null, "QuotedText": "homophily| theprinciple that we tend to be similar to our friends.  Typically, your friends don't look like arandom sample of the underlying population:  viewed collectively, your friends are generallysimilar to you along racial and ethnic dimensions; they are similar in age; and they are alsosimilar in characteristics that are more or less mutable, including the places they live, theiroccupations, their levels of a\u000fuence, and their interests, beliefs, and opinions.  Clearly mostof us have speci\fc friendships that cross all these boundaries; but in aggregate, the pervasivefact is that links in a social network tend to connect people who are similar to one another", "id": 52, "username": "apurva", "updated": "2015-10-08T22:31:19", "tags": [] }] }, "tags": [{ "text": "notocol", "id": 29 }, { "text": "social", "id": 36 }, { "text": "study", "id": 37 }] }
        });

    };

    $scope.IsActiveTab = function (tab) {
        if ($scope.tabSelected == tab) return true;
        else return false;
    }
    $scope.OnClickTab = function (tab) {
        if ($scope.tabSelected != tab) {
            $scope.tabSelected = tab;
            $scope.GetPageList();

        }

    }
    $scope.GetPageList = function () {
        $http({
            method: 'POST',
            url: '/Source/GetSourceList',
            data: {
                'filter': $scope.filter,
                "sourceType": $scope.tabSelected == 'own' ? 1 : 3
            }

        }).then(function successCallback(response) {
            $scope.pageList = response.data;
            $scope.loadCompletePending = response.data.length;
            $scope.loading = false;
            $scope.loadingError = false;
            $scope.updateLayout();
        }, function errorCallback(response) {
            $scope.loadingError = true;
            console.log("something seems wrong with status " + status + " " + xhr);
            setTimeout($scope.GetPageList, 5000);

        });

    }
    $scope.updateLayout = function () {
        // Prepare layout options.
        var options = {
            autoResize: true, // This will auto-update the layout when the browser window is resized.
            container: $('#main'), // Optional, used for some extra CSS styling
            offset: 20, // Optional, the distance between grid items
            itemWidth: 180 // Optional, the width of a grid item
        };

        // Get a reference to your grid items.
        var handler = $('#tiles li');

        // Call the layout function.
        handler.wookmark(options);

        // Capture clicks on grid items.
        handler.click(function () {
            // Randomize the height of the clicked item.
            //var newHeight = $('img', this).height() + Math.round(Math.random()*300+30);
            // $(this).css('height', newHeight+'px');

            // Update the layout.
            //handler.wookmark();
        });

    };
    var GetRecentTags = function () {
        $http({
            method: 'GET',
            url: '/Tag/GetRecentlyUsedTags',
        }).then(function successCallback(response) {
            $scope.recentTags = response.data;
        }, function errorCallback(response) {
        });
    };

    var GetUserCollections = function () {
        $http({
            method: 'GET',
            url: '/Folder/GetUserFolderTree',
        }).then(function successCallback(response) {
            $scope.collections = response.data;
        }, function errorCallback(response) {
        });
    };

    GetRecentTags();
    GetUserCollections();
    $scope.GetPageList();

    $scope.imgLoadedEvents = {
        always: function (instance) {
            //$scope.updateLayout();
            $scope.loadCompletePending--;
            //console.log($scope.loadCompletePending);
            if ($scope.loadCompletePending <= 0) {
                //  $scope.updateLayout();
            }
        },
        done: function (instance) {
            angular.element(instance.elements[0]).addClass('loaded');
            $scope.updateLayout();
        },
        fail: function (instance) { }
    };

    $scope.RefreshPageList = $scope.GetPageList;
    $scope.DeleteSource = function (sourceUserID) {
        var url = 'Source/DeleteSourceUser/sourceUserID=' + sourceUserID;
        var r = confirm("Deleting a page will delete all the notes with it too. Press OK to continue");
        if (r == true) {

            $.http({
                url: url,
                method: "DELETE"
            }).then(function successCallback(response) {
                if (response == "True") {
                    $scope.RefreshPageList();
                }
                else
                    alert("Failed to delete!Contact the server for any help");
            }, function errorCallback() {
                alert("Failed to delete!Contact the server for any help");
            });

        }
    };

});
