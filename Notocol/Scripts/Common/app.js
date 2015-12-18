var app = angular.module('notocolApp', ['siyfion.sfTypeahead', 'angular-images-loaded']);
app.config(function ($locationProvider) {
    $locationProvider.html5Mode(true)
})
var layoutController = app.controller("LayoutController", ['$scope', '$http', '$location', '$window', '$compile', function ($scope, $http, $location, $window, $compile) {
  
    $scope.initCollectionTree = function () {
        ddtreemenu.createTree("treemenu1", true)
    }
    $scope.GotoHome = function () {
        
        $window.location = "/Home/HomeNew";
    };
    $scope.Logout = function () {
        console.log("Logging out");
        $window.location = "/User/SignOutUser";
    }
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
    var arguments = $location.search();
    $scope.searchData = {
        searchString: typeof arguments.q == 'undefined'?"":arguments.q,
        tags: typeof arguments.t == 'undefined'?[]:arguments.t.split(','),
        collection: {
            id: 0,
            name: null
        },
        tabSelected : 'own'
    }
    $scope.searchAvailable = false;
    var activateSearch = function () {
        if (!$scope.searchAvailable) {
            var argString = [];
            if($scope.searchData.tags.length > 0)
                argString[argString.length]  = "&t=" + $scope.searchData.tags.join();
            if($scope.searchData.searchString.length > 0)
                argString[argString.length] = $scope.searchData.searchString

            $window.location = "Home/HomeNew?q=" + argString.join("&");
        }
    }
    $scope.addTagFilter = function (tagName) {
        console.log("Adding tag: " + tagName);
        if ($scope.searchData.tags.indexOf(tagName) < 0) {
            //$scope.$apply(function ($scope) {
                $scope.searchData.tags[$scope.searchData.tags.length] = tagName;
            //})
            
            activateSearch();
        }
    }
    $scope.addSearchQuery = function (q) {
        $scope.$apply(function ($scope) {
            $scope.searchData.searchString = q;
        });
        activateSearch();
    }
    $scope.openPageDetails = function (page) {
        var sourceDetailTemplateURL = '/Templates/Source/SourceDetails';
        var sourceUserDetailDataURL = '/Source/GetSourceUserWithNotes?sourceUserID='
        var sourceDetailDataURL = '/Source/GetSourceWithNotes?sourceID='

        
        $scope.sourceDetails = page;
        $http.get(sourceDetailTemplateURL).then(function (response) {
            if (response.status == 200) {
                var template = angular.element(response.data);
                var compiledTemplate = $compile(template);
                compiledTemplate($scope);

                $.fancybox.open({ content: template, type: 'html' });
            }
        });


        var url = page.sourceUserID == 0 ? sourceDetailDataURL + page.Id : sourceUserDetailDataURL + page.sourceUserID
        $http.get(url).then(function (response) {
            console.log(response);
            $scope.sourceDetails = response.data;
            //$scope.sourceDetails = { "sourceUser": { "ID": 96, "UserID": 6, "Summary": "This book provides a good way of understanding social networking dynamics backed by substantial data and maths", "Privacy": false, "Rating": null, "ModifiedAt": "2015-10-08T22:31:20.023", "thumbnailImageUrl": null, "thumbnailText": null, "FolderID": 21, "noteCount": 9, "SourceID": 79, "PrivacyOverride": null, "PrivateNoteCount": 0, "Folder": { "name": "Social Network", "ID": 21, "description": null, "parentID": 20, "created": "2015-10-08T21:44:39.21", "updated": "2015-10-08T21:44:39.21", "userID": 6, "SourceUsers": [], "User": { "ID": 6, "Username": "apurva", "Password": "welcome", "Identifier": null, "ModifiedAt": "2015-10-05T13:12:58.987", "Email": "apurva@tenet.res.in", "Provider": null, "Gender": "\u0000", "DOB": "0001-01-01T00:00:00", "Address": null, "Name": null, "Photo": null, "Annotations": [], "Folders": [], "SourceUsers": [], "UserTagUsages": [], "Follows": [], "Follows1": [], "Notifications": [], "Notifications1": [] } }, "Source": null, "User": { "ID": 6, "Username": "apurva", "Password": "welcome", "Identifier": null, "ModifiedAt": "2015-10-05T13:12:58.987", "Email": "apurva@tenet.res.in", "Provider": null, "Gender": "\u0000", "DOB": "0001-01-01T00:00:00", "Address": null, "Name": null, "Photo": null, "Annotations": [], "Folders": [{ "name": "Social Network", "ID": 21, "description": null, "parentID": 20, "created": "2015-10-08T21:44:39.21", "updated": "2015-10-08T21:44:39.21", "userID": 6, "SourceUsers": [] }], "SourceUsers": [], "UserTagUsages": [], "Follows": [], "Follows1": [], "Notifications": [], "Notifications1": [] } }, "source": { "ID": 79, "url": "https://www.cs.cornell.edu/home/kleinber/networks-book/networks-book.pdf", "uriHash": "NMh0Vr6aqjoELg1tpiiIvqE2m9YAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==", "faviconURL": null, "title": "networks-book.pdf", "created": "2015-10-08T00:26:10.783", "Annotations": [], "SourceUsers": [{ "ID": 96, "UserID": 6, "Summary": "This book provides a good way of understanding social networking dynamics backed by substantial data and maths", "Privacy": false, "Rating": null, "ModifiedAt": "2015-10-08T22:31:20.023", "thumbnailImageUrl": null, "thumbnailText": null, "FolderID": 21, "noteCount": 9, "SourceID": 79, "PrivacyOverride": null, "PrivateNoteCount": 0, "Folder": { "name": "Social Network", "ID": 21, "description": null, "parentID": 20, "created": "2015-10-08T21:44:39.21", "updated": "2015-10-08T21:44:39.21", "userID": 6, "SourceUsers": [], "User": { "ID": 6, "Username": "apurva", "Password": "welcome", "Identifier": null, "ModifiedAt": "2015-10-05T13:12:58.987", "Email": "apurva@tenet.res.in", "Provider": null, "Gender": "\u0000", "DOB": "0001-01-01T00:00:00", "Address": null, "Name": null, "Photo": null, "Annotations": [], "Folders": [], "SourceUsers": [], "UserTagUsages": [], "Follows": [], "Follows1": [], "Notifications": [], "Notifications1": [] } }, "Source": null, "User": { "ID": 6, "Username": "apurva", "Password": "welcome", "Identifier": null, "ModifiedAt": "2015-10-05T13:12:58.987", "Email": "apurva@tenet.res.in", "Provider": null, "Gender": "\u0000", "DOB": "0001-01-01T00:00:00", "Address": null, "Name": null, "Photo": null, "Annotations": [], "Folders": [{ "name": "Social Network", "ID": 21, "description": null, "parentID": 20, "created": "2015-10-08T21:44:39.21", "updated": "2015-10-08T21:44:39.21", "userID": 6, "SourceUsers": [] }], "SourceUsers": [], "UserTagUsages": [], "Follows": [], "Follows1": [], "Notifications": [], "Notifications1": [] } }], "Notifications": [] }, "userNotes": { "Model.Extended.UserKey": [{ "NoteText": "This book provides a good way of understanding social networking dynamics backed by substantial data and maths", "QuotedText": null, "id": 0, "username": null, "updated": "2015-10-08T22:31:20.023", "tags": null }], "Model.Extended.UserKey": [{ "NoteText": null, "QuotedText": " Graph theoryis the study of network structure, while game theory provides models of individual behavior", "id": 42, "username": "apurva", "updated": "2015-10-08T00:33:10", "tags": [] }, { "NoteText": "Mathematical model to understand the network complexities and also to establish a language to talk about it", "QuotedText": "Graph Theory", "id": 44, "username": "apurva", "updated": "2015-10-08T12:50:26", "tags": ["mathematical-model"] }, { "NoteText": "To understand effects of player's (user's ) responses on others in the network", "QuotedText": "Game  Theory", "id": 45, "username": "apurva", "updated": "2015-10-08T12:51:27", "tags": ["social", "network", "human-behavior"] }, { "NoteText": "Triadic Closure", "QuotedText": "If  two  people  in  a  social  network  have  a  friend  in  common,  then  there  is  anincreased likelihood that they will become friends themselves at some point in thefuture", "id": 47, "username": "apurva", "updated": "2015-10-08T20:20:36", "tags": [] }, { "NoteText": "Strength of weak ties in social network: Weak ties or distant ties can bring in some information that might not usually come from your set of close friends", "QuotedText": " The closely-knit groups thatyou belong to, though they are \flled with people eager to help, are also \flled with peoplewho know roughly the same things that you do", "id": 48, "username": "apurva", "updated": "2015-10-08T20:30:14", "tags": [] }, { "NoteText": null, "QuotedText": "tronger links represent closerfriendship and greater frequency of interaction", "id": 49, "username": "apurva", "updated": "2015-10-08T20:31:33", "tags": [] }, { "NoteText": "1. Study of triadic closures and strong triadic closure properties could be useful in determing friend suggestions in notocol\n2. ", "QuotedText": null, "id": 50, "username": "apurva", "updated": "2015-10-08T20:39:14", "tags": [] }, { "NoteText": null, "QuotedText": "dual roleas weak connections but also valuable conduits to hard-to-reach parts of the network | thisis the surprising strength of weak ties", "id": 51, "username": "apurva", "updated": "2015-10-08T21:28:45", "tags": [] }, { "NoteText": null, "QuotedText": "homophily| theprinciple that we tend to be similar to our friends.  Typically, your friends don't look like arandom sample of the underlying population:  viewed collectively, your friends are generallysimilar to you along racial and ethnic dimensions; they are similar in age; and they are alsosimilar in characteristics that are more or less mutable, including the places they live, theiroccupations, their levels of a\u000fuence, and their interests, beliefs, and opinions.  Clearly mostof us have speci\fc friendships that cross all these boundaries; but in aggregate, the pervasivefact is that links in a social network tend to connect people who are similar to one another", "id": 52, "username": "apurva", "updated": "2015-10-08T22:31:19", "tags": [] }] }, "tags": [{ "text": "notocol", "id": 29 }, { "text": "social", "id": 36 }, { "text": "study", "id": 37 }] }
        });

    };

    GetRecentTags();
    GetUserCollections();

}]);

