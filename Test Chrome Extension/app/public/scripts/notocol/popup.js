function ToggleAnnotation() {
   console.log("Sending message to background");

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

var baseURL = "https://localhost:44301/";

(function () {
    app = angular.module('NotocolPopup', []);
    app.controller('TabsCtrl', ['$scope', function ($scope) {
        this.tab = 1;
        this.setTab = function (setTab) {
            this.tab = setTab;
        };
        this.isSet = function (checkTab) {
            return this.tab === checkTab;
        };
    }]);

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
        //var vm = this;
        chrome.extension.sendMessage({
            greeting: "PageDetails"
        },
            function (pageDetailResponse) {
                $timeout(function () {
                    console.log(pageDetailResponse);
                    if (pageDetailResponse.status) {
                        pageDetails = {
                            title: pageDetailResponse.title,
                            url: pageDetailResponse.url,
                            faviconurl: pageDetailResponse.faviconUrl,
                            tabID:pageDetailResponse.tabID
                        };

                        PageProperties.store('pageDetails', pageDetails);
                        PageProperties.store('pageDetailsStatus', true);
                        $scope.$broadcast('pageDetailsUpdated');
                    }
                })

            });

        this.closePopup = function () {
            window.close();
        }
    }]);

    app.controller("BookmarkCtrl", ['$scope', '$timeout', 'PageProperties', '$http', function ($scope, $timeout, PageProperties, $http) {
        var vm = this;
        $scope.$on('pageDetailsUpdated', function () {
            vm.pageDetails = PageProperties.get('pageDetails');
            console.log(vm.pageDetails);
        });

        this.GetPageImages = function () {
            var filePath = 'public/scripts/notocol/sendImageList.js';
            var jqueryFile = 'public/scripts/vendor/jquery.min.js';
            var embedlyFile = 'public/scripts/vendor/embedly.js';
            var inputVariables = {
                sourceURI: vm.pageDetails.url,
                tabID: vm.pageDetails.tabID,
                imageListURl: "https://localhost:44302/Home/ThumbNailData"
            };
            
            chrome.tabs.executeScript(vm.pageDetails.tabID, { file: jqueryFile }
                , function () {
                    chrome.tabs.executeScript(vm.pageDetails.tabID, {
                        code: 'var inputVariables = ' + JSON.stringify(inputVariables)
                    }, function () {
                        chrome.tabs.executeScript(vm.pageDetails.tabID, { file: filePath });
                    });
                });
        }

        this.savePage = function () {
            console.log("title: " + vm.pageDetails.title + " url: " + vm.pageDetails.url + " faviconurl: " + vm.pageDetails.faviconurl + " summary: " + vm.pageSummary);
            var pageTags = vm.pagetags?vm.pagetags.split(','):null;
            var sourceDetails = {
                sourceLink: vm.pageDetails.url,
                sourceURI: vm.pageDetails.url,
                title: vm.pageDetails.title,
                summary: vm.pageSummary,
                tags: pageTags
            };

            $http.post(baseURL + "api/Source/UpdateSource", sourceDetails).
                success(function (data, status, headers, config) {
                    console.log("Saved page");
                    
                })
                .error(function (data, status, headers, config) {
                    console.log("Failed to save the page");
                });

            this.GetPageImages();
        }
        
         
    }]);

    app.controller("SnapshotCtrl", 'PageProperties', ['$scope', 'PageProperties', function ($scope, PageProperties) {

    }]);

    app.controller("AnnotationCtrl", ['$scope', function ($scope) {

    }]);

    app.controller("CheckCtrl", ['$scope', function ($scope) {
        console.log("I am loaded!");
    }]);
})();
    

