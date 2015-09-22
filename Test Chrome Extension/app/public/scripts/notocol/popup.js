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
                
                //$scope.addFolderVisibility = true;
                //$scope.createFolderVisibility = false;
                //$scope.selectedFolder = {};
                $scope.showSublist = false;
                //$scope.tags = [];
                $scope.hideList = true;
                $scope.pageDetails = {};
                var baseURL = "https://localhost:44301/";
                var addedFolders = [];

                var RootFolder = {
                    ID: 0,
                    Parent: null,
                    Name: "Root",
                    Children: []
                };

                //Variable that shows whether user is selecting folders
                $scope.folderSelect = false;
                //$scope.currentParentFolder = RootFolder;
                $scope.selectedFolder = RootFolder;
                $scope.folderTree = RootFolder;
                $scope.selectedFoldername = null;
                $scope.currentParent = null;
                $scope.selectFolder = function (folder) {
                    $scope.selectedFoldername = folder.Name;
                    $scope.pageDetails.folderData = $scope.pageDetails.folderData || {}
                    $scope.pageDetails.folderData.selectedFolder = {
                        "folderID": folder.ID,
                        "folderName": folder.Name
                    }
                    $scope.SetFolderSelect(false);
                }
                $scope.setCurrentParent = function(folder){
                    $scope.currentParent = folder;
                    $scope.selectedFoldername = null;
                }

                function RunForEachFolderTreeNode(folderTreeNode, functionToRun) {
                    functionToRun(folderTreeNode);
                    if (folderTreeNode.Children.length > 0) {
                        var i = 0;
                        for (; i < folderTreeNode.Children.length; i++) {
                            RunForEachFolderTreeNode(folderTreeNode.Children[i], functionToRun);
                        }

                    }
                }

                $scope.AddFolder = function () {
                    var selectedFolder = null;
                    if ($scope.selectedFoldername == "") {
                        selectedFolder = $scope.folderTree;
                    }

                    if ($scope.currentParent != null && $scope.currentParent.Children.length > 0) {
                        for (var i = 0; i < $scope.currentParent.Children.length; i++) {
                            if ($scope.currentParent.Children[i].Name == $scope.selectedFoldername) {
                                selectedFolder = $scope.currentParent.Children[i];
                                break;
                            }

                        }
                    }
                    if (selectedFolder == null) {
                        selectedFolder = {
                            ID: "a" + addedFolders.length,
                            Parent: $scope.currentParent,
                            Name: $scope.selectedFoldername,
                            Children: []
                        }
                        addedFolders.push({
                            ID: selectedFolder.ID,
                            ParentID: selectedFolder.Parent.ID,
                            Name: selectedFolder.Name
                        })
                        //Add folder to added folders
                        $scope.currentParent.Children.push(selectedFolder);

                    }

                    $scope.selectFolder(selectedFolder);

                }

                function GetSelectedFolderFromUserFolderTree(folderID, checkFolderTree) {
                    var retFolder = null;
                    if (folderID == checkFolderTree.ID) return checkFolderTree;
                    else if(checkFolderTree.Children != null && checkFolderTree.Children.length > 0){
                        for (var i = 0 ; retFolder == null && i < checkFolderTree.Children.length ; i++) {
                            retFolder = GetSelectedFolderFromUserFolderTree(folderID, checkFolderTree.Children[i]);
                        }
                    }
                    return retFolder;
                }

                var GetCurrentPageDetails = function () {
                    chrome.extension.sendMessage({
                        greeting: "PageDetails"
                    },
                        function (response) {
                            
                            $timeout(function () {
                                if (response.userFolderTreeJson != null){
                                    $scope.folderTree = JSON.retrocycle(JSON.parse(response.userFolderTreeJson));
                                    //$scope.currentParentFolder = $scope.folderTree;
                                }

                                if (response.tabInfo != null && response.tabInfo.status) {
                                    $scope.pageDetails = response.tabInfo;
                                    if ($scope.pageDetails.sourceUserID <= 0) $scope.SavePage();

                                    if ($scope.pageDetails.tags != null) {
                                        console.log("Received tags are " + $scope.pageDetails.tags);

                                    }

                                    if ($scope.pageDetails.folderData != null && $scope.pageDetails.folderData.selectedFolder != null) {
                                        console.log("Recieved folder is " +
                                            JSON.stringify($scope.pageDetails.folderData.selectedFolder));
                                        $scope.selectedFolder = GetSelectedFolderFromUserFolderTree(
                                            $scope.pageDetails.folderData.selectedFolder.folderID, $scope.folderTree);
                                        $scope.selectedFoldername = $scope.selectedFolder.Name;
                                        $scope.currentParent = $scope.selectedFolder.Parent;
                                    } else {
                                        $scope.selectedFolder = $scope.folderTree;
                                        $scope.currentParent = $scope.folderTree;
                                    }

                                } else {
                                    //TODO Better way of asking
                                    if (!alert("Please refresh the page since the page was opened before Notocol was installed")) $scope.ClosePopup();
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

                        $http.delete(baseURL + "api/Source/DeleteSource?sourceUserID=" + $scope.pageDetails.sourceUserID ).
                            success(function (data, status, headers, config) {
                                if (data == "true") {
                                    console.log("Page with sourceID " + $scope.pageDetails.sourceUserID + " deleted");
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
            
                    
                    var data = { sourceData: $scope.pageDetails };
                    if (addedFolders.length > 0) {
                        data.addedFolders = addedFolders;
                    }
                    
                    //console.log("Saving page with details: " + JSON.stringify(sourceDetails));
                    $http.post(baseURL + "api/Source/SaveSource", data ).
                        success(function (data, status, headers, config) {
                            toastr.success('This page has been saved to your collection', 'Saved!');
                            console.log("Saved page with response data as " + JSON.stringify(data));
                            if (data.status == "failure") {
                                //TODO Changes to handle error
                                console.log("Failed to save page");
                                toastr.error('Failed to save the page', 'Save Failed');
                            }

                            if ($scope.pageDetails.tags != null) TagStore.store($scope.pageDetails.tags);
                            if (addedFolders.length > 0) {
                                //Process the user tree
                                RunForEachFolderTreeNode($scope.folderTree, function (node) {
                                    if (typeof node.ID.indexOf == "function" &&  node.ID.indexOf("a") == 0) {
                                        node.ID = data.saveSourceData.addedFolderIDs[node.ID];

                                    }
                                    
                                });
                                chrome.extension.sendMessage({
                                    greeting: "UserFoldersUpdated",
                                    userFolderTree: JSON.stringify(JSON.decycle($scope.folderTree))
                                    
                                });
                                addedFolders = [];
                            }

                            $scope.pageDetails.sourceID = data.saveSourceData.sourceData.sourceID;
                            $scope.pageDetails.sourceUserID = data.saveSourceData.sourceData.sourceUserID;
                            $scope.pageDetails.folderData = data.saveSourceData.sourceData.folderData;

                            chrome.extension.sendMessage({
                                greeting: "PageDetailsUpdated",
                                pageInfo: $scope.pageDetails
                            });

                            if ($scope.pageDetails.url.indexOf(".pdf") < 0)
                                GetPageImages($scope.pageDetails.url, data.saveSourceData.sourceData.sourceUserID);
                    
                        })
                        .error(function (data, status, headers, config) {
                            toastr.error('Failed to save the page', 'Save Failed');
                            console.log("Failed to save the page");
                        });
                }

                $scope.folderData = {}

                
                $scope.init = function () {
                    //$scope.listOfBookmarkFolders = [];
                    //for (var z = 0; z < $scope.folderData.children.length; z++) {
                    //    var p = $scope.folderData.children[z];
                    //    $scope.listOfBookmarkFolders.push({ name: p.name, children: p.children });
                    //};
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



                $scope.SetFolderSelect = function (value) {
                    $scope.folderSelect = value;
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
