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
                $scope.tagSuggestions = ["solar", "policy", "content"];
                //$scope.tags = [];
                $scope.hideList = true;
                $scope.pageDetails = {};
                var baseURL = "https://notocol.tenet.res.in:8443/";
                //var baseURL = "https://localhost:44301/";
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
                $scope.selectedFoldername = "";
                $scope.currentParent = null;
                $scope.selectFolder = function (folder) {
                    if (folder != null) {
                        $scope.selectedFoldername = folder.Name;
                        $scope.pageDetails.folderData = $scope.pageDetails.folderData || {}
                        $scope.pageDetails.folderData.selectedFolder = {
                            "folderID": folder.ID,
                            "folderName": folder.Name
                        }
                    }
                    $scope.SetFolderSelect(false);
                }
                $scope.setCurrentParent = function(folder){
                    $scope.currentParent = folder;
                    $scope.selectedFoldername = "";

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
                $scope.AddTag = function (tag) {
                    $scope.pageDetails.tags.push(tag);
                }

                $scope.AddFolder = function () {
                    var selectedFolder = null;
                    if ($scope.selectedFoldername == null) {
                        $scope.SetFolderSelect(false);
                        return;
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
                var URLNotSupported = function (url) {
                    if (url.indexOf("chrome://") == 0) {
                        alert("Notocol not supported for this page");
                            return true;
                    }
                    return false;
                }

                var GetCurrentPageDetails = function () {
                    chrome.extension.sendMessage({
                        greeting: "PageDetails"
                    },
                        function (response) {
                            if (response == null)
                                $scope.ClosePopup();

                            if (response.tabInfo == null || !response.tabInfo.status) {
                                if (!alert("Please refresh the page since the page was opened before Notocol was installed")) $scope.ClosePopup();
                            }

                            if (response.tabInfo.url.indexOf("chrome://newtab/") == 0) {
                                chrome.tabs.update(null, { url: baseURL });
                                //$scope.ClosePopup();
                                return;
                            }

                            if (URLNotSupported(response.tabInfo.url)) {
                                $scope.ClosePopup();
                            }

                            $timeout(function () {
                                if (response.userFolderTreeJson != null){
                                    $scope.folderTree = JSON.retrocycle(JSON.parse(response.userFolderTreeJson));
                                    //$scope.currentParentFolder = $scope.folderTree;
                                }

                                console.log("Received page details as " + response.tabInfo);
                                $scope.pageDetails = response.tabInfo;

                                if ($scope.pageDetails.sourceUserID <= 0) $scope.SavePage();

                                if ($scope.pageDetails.folderData != null && $scope.pageDetails.folderData.selectedFolder != null) {
                                    
                                    $scope.selectedFolder = GetSelectedFolderFromUserFolderTree(
                                        $scope.pageDetails.folderData.selectedFolder.folderID, $scope.folderTree);

                                    $scope.selectedFoldername = $scope.selectedFolder.Name;
                                    $scope.currentParent = $scope.selectedFolder.Parent;
                                } else {
                                    $scope.selectedFolder = $scope.folderTree;
                                    $scope.currentParent = $scope.folderTree;
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
                        ThumbNailDataURl: baseURL+"/Api/ThumbnailData"
                    };
                    chrome.tabs.executeScript($scope.pageDetails.tabID, {
                        code: 'var inputVariables = ' + JSON.stringify(inputVariables)
                    }, function () {
                        chrome.tabs.executeScript($scope.pageDetails.tabID, { file: filePath }, function () {

                        });
                    });
                }

                $scope.loadTags = function (query) {
                    return $http.get(baseURL + 'api/Tag/Tag?tagQuery=' + query);
                };

                $scope.TogglePrivacy = function () {
                    $scope.pageDetails.privacy = ($scope.pageDetails.privacy == 0) ? 1 : 0;
                }

                $scope.ToggleAnnotation = function () {
                    console.log("Sending message to background");
                    $scope.pageDetails.annotator = ($scope.pageDetails.annotator == 0) ? 1 : 0;
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

                $scope.SetFolderSelect = function (value) {
                    $scope.folderSelect = value;
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
