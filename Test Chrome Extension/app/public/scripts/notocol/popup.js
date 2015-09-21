/**
 * Created by Nupur on 9/6/2015.
 */
var extensionApp = angular.module('extensionApp', ['ngTagsInput', '720kb.tooltips', 'toastr'])
.directive('extensionPopUp', ['$window', '$rootScope', '$timeout', '$compile', '$http', 'toastr', 'TagStore',
function ($window, $rootScope, $timeout, $compile, $http, toastr, TagStore) {
        var obj = {
            restrict: 'E',
            replace: true,
            templateUrl: "popUp.html",

            controller: function ($scope, $element, $document, toastr) {

                $scope.addFolderVisibility = true;
                $scope.createFolderVisibility = false;
                //$scope.selectedFolder = {};
                $scope.showSublist = false;
                //$scope.tags = [];
                $scope.hideList = true;
                $scope.pageDetails = null;
                var baseURL = "https://localhost:44301/";

                var GetCurrentPageDetails = function () {
                    chrome.extension.sendMessage({
                        greeting: "PageDetails"
                    },
                        function (pageDetailResponse) {
                            $timeout(function () {
                                if (pageDetailResponse.status) {
                                    $scope.pageDetails = pageDetailResponse;
                                    if ($scope.pageDetails.sourceUserID <= 0) $scope.SavePage();

                                    if ($scope.pageDetails.tags != null) {
                                        console.log("Received tags are " + $scope.pageDetails.tags);

                                    }

                                    if ($scope.pageDetails.folderData != null && $scope.pageDetails.folderData.selectedFolder != null) {
                                        console.log("Recieved folder is " +
                                            JSON.stringify($scope.pageDetails.folderData.selectedFolder));

                                    }

                                } else {
                                    //TODO Better way of asking
                                    if (!alert("Please refresh the page since the page was opened before Notocol was installed")) $scope.closePopup();
                                }
                            })

                        });
                }

                GetCurrentPageDetails();

                var GetPageImages = function (pageUrl, sourceUserID) {
                    var filePath = 'public/scripts/notocol/sendImageList.js';
                    var jqueryFile = 'public/scripts/vendor/jquery.min.js';

                    var inputVariables = {
                        pageUrl: pageUrl,
                        ID: sourceUserID,
                        tabID: $scope.pageDetails.tabID,
                        ThumbNailDataURl: "https://localhost:44301/Api/ThumbnailData"
                    };
                    chrome.tabs.executeScript($scope.pageDetails.tabID, {
                        code: 'var inputVariables = ' + JSON.stringify(inputVariables)
                    }, function () {
                        chrome.tabs.executeScript($scope.pageDetails.tabID, { file: filePath }, function () {

                        });
                    });
                }

                var loadTags = function (query) {
                    return TagStore.filter(query);
                };

                $scope.TogglePrivacy = function () {
                    $scope.pageDetails.privacy = ($scope.pageDetails.privacy == 0) ? 1 : 0;
                }

                $scope.ToggleAnnotation = function () {
                    console.log("Sending message to background");
                    
                    chrome.extension.sendMessage({ greeting: "ToggleAnnotation" },
                         function (response) {
                             console.log("Received message from toggle:" + response.message);
                         });
                }

                $scope.ClosePopup = function () {
                    window.close();
                }

                $scope.DeletePage = function () {
                    
                        //TODO Delete Page asking for confirmation
                        alert("Deleting a page would also delete any annotation related to it. Are you sure you want to delete the page?");

                        $http.delete(baseURL + "api/Source/DeleteSource?sourceUserID=" + pageDetails.sourceUserID ).
                            success(function (data, status, headers, config) {
                                if (data == "true") {
                                    console.log("Page with sourceID " + pageDetails.sourceUserID + " deleted");
                                    //$scope.tags = [];
                                    //$scope.folderData = {};
                                    $scope.pageDetails.sourceUserID = 0;
                                    $scope.pageDetails.summary = null;
                                    $scope.pageDetails.tags = null;
                                    $scope.pageDetails.folderData = null;
                                    chrome.extension.sendMessage({
                                        greeting: "PageDetailsUpdated",
                                        pageInfo: $scope.pageDetails
                                    });
                                } else {
                                    alert("Failed to delete page");
                                }
                            })
                            .error(function (data, status, headers, config) {
                                console.log("Failed to delete the page");
                            });

                }

                $scope.SavePage = function () {
            
                    var sourceDetails = $scope.pageDetails
                    
                    
                    console.log("Saving page with details: " + JSON.stringify(sourceDetails));
                    $http.post(baseURL + "api/Source/SaveSource", sourceDetails ).
                        success(function (data, status, headers, config) {
                            toastr.success('This page has been saved to your collection', 'Saved!');
                            console.log("Saved page with response data as " + JSON.stringify(data));
                            if (data.status == "failure") {
                                //TODO Changes to handle error
                                console.log("Failed to save page");
                                toastr.error('Failed to save the page', 'Save Failed');
                            }

                            if (sourceDetails.tags != null) TagStore.store(sourceDetails.tags);
                            if (sourceDetails.folderData != null &&
                                sourceDetails.folderData.addedFolders != null) {
                                chrome.extension.sendMessage({
                                    greeting: "UserFoldersUpdated",
                                    pageInfo: pageDetails
                                });
                            }

                            $scope.pageDetails.sourceID = data.sourceData.sourceID;
                            $scope.pageDetails.sourceUserID = data.sourceData.sourceUserID;

                            chrome.extension.sendMessage({
                                greeting: "PageDetailsUpdated",
                                pageInfo: $scope.pageDetails
                            });

                            if ($scope.pageDetails.url.indexOf(".pdf") < 0)
                                GetPageImages($scope.pageDetails.url, data.sourceData.sourceUserID);
                    
                        })
                        .error(function (data, status, headers, config) {
                            toastr.error('Failed to save the page', 'Save Failed');
                            console.log("Failed to save the page");
                        });
                }

                $scope.folderData = {
                    "id": 0,
                    "Name": "Root",
                    "parentID": 0,
                    "children": [
                        {
                            "id": 1,
                            "name": "Project1",
                            "parentID": -1,
                            "children": [
                                {
                                    "id": 6,
                                    "name": "Project11",
                                    "parentID": 1,
                                    "children": [
                                        {
                                            "id": 17,
                                            "name": "Project111",
                                            "parentID": 6,
                                            "children": [
                                                {
                                                    "id": 18,
                                                    "name": "Project1111",
                                                    "parentID": 17,
                                                    "children": []
                                                },
                                                {
                                                    "id": 19,
                                                    "name": "Project1112",
                                                    "parentID": 17,
                                                    "children": []
                                                }
                                            ]
                                        }
                                    ]
                                },
                                {
                                    "id": 7,
                                    "name": "Project12",
                                    "parentID": 1,
                                    "children": []
                                }
                            ]
                        },
                        {
                            "id": 2,
                            "name": "Project2",
                            "parentID": -1,
                            "children": []
                        },
                        {
                            "id": 3,
                            "name": "Project3",
                            "parentID": -1,
                            "children": [
                                {
                                    "id": 8,
                                    "name": "Project31",
                                    "parentID": 3,
                                    "children": []
                                }
                            ]
                        },
                        {
                            "id": 4,
                            "name": "Project4",
                            "parentID": -1,
                            "children": []
                        },
                        {
                            "id": 5,
                            "name": "Project5",
                            "parentID": -1,
                            "children": [
                                {
                                    "id": 9,
                                    "name": "Project51",
                                    "parentID": 5,
                                    "children": []
                                },
                                {
                                    "id": 10,
                                    "name": "Project52",
                                    "parentID": 5,
                                    "children": []
                                },
                                {
                                    "id": 11,
                                    "name": "Project53",
                                    "parentID": 5,
                                    "children": []
                                },
                                {
                                    "id": 12,
                                    "name": "Project54",
                                    "parentID": 5,
                                    "children": []
                                },
                                {
                                    "id": 13,
                                    "name": "Project55",
                                    "parentID": 5,
                                    "children": []
                                },
                                {
                                    "id": 14,
                                    "name": "Project56",
                                    "parentID": 5,
                                    "children": []
                                },
                                {
                                    "id": 15,
                                    "name": "Project57",
                                    "parentID": 1,
                                    "children": []
                                },
                                {
                                    "id": 16,
                                    "name": "Project58",
                                    "parentID": 1,
                                    "children": []
                                }
                            ]
                        }
                    ]
                };

                
                $scope.init = function () {
                    $scope.listOfBookmarkFolders = [];
                    for (var z = 0; z < $scope.folderData.children.length; z++) {
                        var p = $scope.folderData.children[z];
                        $scope.listOfBookmarkFolders.push({ name: p.name, children: p.children });
                    };
                };
                $scope.getTags = function () {
                    var data = [
                        {
                            "id": 1,
                            "Name": "war"
                        },
                        {
                            "id": 2,
                            "Name": "India"
                        },
                        {
                            "id": 3,
                            "Name": "Egypt"
                        },
                        {
                            "id": 4,
                            "Name": "Internship"
                        },
                        {
                            "id": 5,
                            "Name": "GradSchools"
                        },
                        {
                            "id": 6,
                            "Name": "MIT2014"
                        },

                        {
                            "id": 7,
                            "Name": "IITMadrass"
                        }
                    ];
                    for (var x = 0; x < data.length; x++) {
                        $scope.tags.push({ 'text': data[x].Name });
                    }

                }
                $scope.init();
                //$scope.getTags();



                $scope.openSearchBox = function () {
                    $scope.addFolderVisibility = false;
                    $scope.createFolderVisibility = true;

                };



                $scope.showSubFolder = function (f, visibility) {
                    if (visibility) {
                        $scope.listOfBookmarkFolders = [];
                        for (var z = 0; z < f.children.length; z++) {
                            var p = f.children[z];
                            $scope.listOfBookmarkFolders.push({ name: p.name, children: p.children });
                        };

                    } else {
                        $scope.addFolderVisibility = true;
                        $scope.createFolderVisibility = false;
                    }
                    $scope.selectedFolder = f;
                    $scope.hideList = visibility;

                };
            }


        };
        return obj;
    }
])

extensionApp.factory("TagStore", function ($rootScope) {
    var TAGS_LIST_KEY, TAGS_MAP_KEY;

    TAGS_LIST_KEY = 'hypothesis.user.tags.list';
    TAGS_MAP_KEY = 'hypothesis.user.tags.map';

    return {
        store: function (tags) {

            var compareFn, i, len, savedTags, tag, tagsList;
            savedTags = localStorage.getItem(TAGS_MAP_KEY)

            if (savedTags == null) {
                savedTags = {};
            } else
                savedTags = JSON.parse(savedTags)

            for (i = 0, len = tags.length; i < len; i++) {
                tag = tags[i];

                if (savedTags[tag] != null) {
                    savedTags[tag].count += 1;
                    savedTags[tag].updated = Date.now();
                } else {
                    savedTags[tag] = {
                        text: tag,
                        count: 1,
                        updated: Date.now()
                    };
                }
                console.log("Storing tag " + JSON.stringify(savedTags[tag]));
            }
            localStorage.setItem(TAGS_MAP_KEY, JSON.stringify(savedTags));
            tagsList = [];
            for (tag in savedTags) {
                tagsList[tagsList.length] = tag;
            }
            compareFn = function (t1, t2) {
                if (savedTags[t1].count !== savedTags[t2].count) {
                    return savedTags[t2].count - savedTags[t1].count;
                } else {
                    if (t1 < t2) {
                        return -1;
                    }
                    if (t1 > t2) {
                        return 1;
                    }
                    return 0;
                }
            };
            tagsList = tagsList.sort(compareFn);
            return localStorage.setItem(TAGS_LIST_KEY, JSON.stringify(tagsList));
        },


        filter: function (query) {
            console.log("Checking for filter " + query)

            return [
                { text: 'just' },
                { text: 'some' },
                { text: 'cool' },
                { text: 'tags' }
            ]
        }
    };

});
