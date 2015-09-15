
var baseURL = "https://localhost:44301/";

(function () {
    app = angular.module('NotocolPopup', []);
    
    app.factory("TagStore", function ($rootScope) {
        var TAGS_LIST_KEY, TAGS_MAP_KEY;

        TAGS_LIST_KEY = 'hypothesis.user.tags.list';
        TAGS_MAP_KEY = 'hypothesis.user.tags.map';

        return {
            store: function (tags) {
                
                var compareFn, i, len, savedTags, tag, tagsList;
                savedTags = localStorage.getItem(TAGS_MAP_KEY)
                
                if (savedTags == null) {
                    savedTags = {};
                }else
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
                compareFn = function(t1, t2) {
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

    //app.controller('TabsCtrl', ['$scope', function ($scope) {
    //    this.tab = 1;
    //    this.setTab = function (setTab) {
    //        this.tab = setTab;
    //    };
    //    this.isSet = function (checkTab) {
    //        return this.tab === checkTab;
    //    };
    //}]);

    app.factory('PageProperties', function ($rootScope) {
        var mem = {};
        return {
            store: function (key, value) {
                mem[key] = value;
                console.log(mem);
            },
            get: function (key) {
                return mem[key];
            }
        };
    });

    app.controller('PageCtrl', ['$scope', '$timeout', 'PageProperties', function ($scope, $timeout, PageProperties) {
        var vm = this;
        //this.annotator = false;
        chrome.extension.sendMessage({
            greeting: "PageDetails"
        },
        function (pageDetailResponse) {
                $timeout(function () {
                    if (pageDetailResponse.status) {
                        pageDetails = pageDetailResponse;
                        PageProperties.store('pageDetails', pageDetails);
                        PageProperties.store('pageDetailsStatus', true);
                        $scope.$broadcast('pageDetails');

                        if (vm.annotator != pageDetails.annotator) {

                            //ToggleAnnotation();
                        }
                    } else {
                        //TODO Better way of asking
                        if(!alert("Please refresh the page since the page was opened before Notocol was installed")) vm.closePopup();
                    }
                })

            });

        this.closePopup = function () {
            
            window.close();
        }

        this.DeletePage = function () {
            $scope.$broadcast('DeletePage');
            return $scope.msg;
        }
        function ToggleAnnotation() {
            console.log("Sending message to background");
            var annotator_button = document.getElementById("toggleAnnotation");

            if (vm.annotator == true) {
                vm.annotator = false;
                annotator_button.value = "off";
            } else {
                vm.annotator = true;
                annotator_button.value = "on";
            }

            chrome.extension.sendMessage({ greeting: "ToggleAnnotation" },
                 function (response) {
                     console.log("Received message from toggle:" + response.message);
                 });
        }
        angular.element(document).ready(function () {
            console.log("Enable Annotation");
            var ann_button = document.getElementById('toggleAnnotation');
            ann_button.addEventListener("click", ToggleAnnotation);
        });

    }]);

    app.controller("BookmarkCtrl", ['$scope', '$timeout', 'PageProperties', '$http', 'TagStore', function ($scope, $timeout, PageProperties, $http, TagStore) {
        var vm = this;
        $scope.$on('pageDetails', function () {
            vm.pageDetails = PageProperties.get('pageDetails');
            console.log("Received vm.pageDetails as " + JSON.stringify(vm.pageDetails));
            if (typeof vm.pageDetails.sourceUserID == "undefined" || vm.pageDetails.sourceUserID <= 0) {
                
                vm.savePage();
            }
        });

        $scope.$on('DeletePage', function () {
            //TODO Delete Page asking for confirmation
            alert("Deleting a page would also delete any annotation related to it. Are you sure you want to delete the page?");
        });
        GetPageImages = function (pageUrl, sourceUserID) {
            var filePath = 'public/scripts/notocol/sendImageList.js';
            var jqueryFile = 'public/scripts/vendor/jquery.min.js';
            
            var inputVariables = {
                pageUrl: pageUrl,
                ID: sourceUserID,
                tabID: vm.pageDetails.tabID,
                ThumbNailDataURl: "https://localhost:44301/Api/ThumbnailData"
            };
            chrome.tabs.executeScript(vm.pageDetails.tabID, {
                code: 'var inputVariables = ' + JSON.stringify(inputVariables)
            }, function () {
                chrome.tabs.executeScript(vm.pageDetails.tabID, { file: filePath }, function () {

                });
            });
            
                    
            
        }

        $scope.loadTags = function (query) {
            return TagStore.filter(query);
        };

        this.savePage = function () {
            
            var sourceDetails = vm.pageDetails
            if (sourceDetails.tags != null && typeof sourceDetails.tags.split != "undefined")
                sourceDetails.tags = sourceDetails.tags.split(',');
            
            console.log("Saving page with details: " + JSON.stringify(sourceDetails));
            $http.post(baseURL + "api/Source/SaveSource", sourceDetails ).
                success(function (data, status, headers, config) {
                    console.log("Saved page with response data as " + JSON.stringify(data));
                    if (data.status == "failure") {
                        //TODO Changes to handle error
                        console.log("Failed to save page");
                    }

                    if (sourceDetails.tags != null) TagStore.store(sourceDetails.tags);
                        

                    pageDetails.sourceID = data.sourceData.sourceID;
                    pageDetails.sourceUserID = data.sourceData.sourceUserID;

                    chrome.extension.sendMessage({
                        greeting: "PageDetailsUpdated",
                        pageInfo: pageDetails
                    });

                    if (vm.pageDetails.url.indexOf(".pdf") < 0)
                        GetPageImages(pageDetails.url, data.sourceData.sourceUserID);
                    
                })
                .error(function (data, status, headers, config) {
                    console.log("Failed to save the page");
                });
        }
    }]);

    app.controller("SnapshotCtrl", 'PageProperties', ['$scope', 'PageProperties', function ($scope, PageProperties) {

    }]);

    app.controller("AnnotationCtrl", ['$scope', function ($scope) {

    }]);

    app.controller("CheckCtrl", ['$scope', function ($scope) {
        console.log("I am loaded!");
    }]);

    app.filter('domain', function () {
        return function (input) {
            var matches,
                output = "",
                urls = /\w+:\/\/([\w|\.]+)/;

            matches = urls.exec(input);

            if (matches !== null) output = matches[1];

            return output;
        };
    });

    
})();
    

