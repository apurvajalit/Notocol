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
        var vm = this;
        chrome.extension.sendMessage({
            greeting: "PageDetails"
        },
            function (pageDetailResponse) {
                $timeout(function () {
                    //console.log(pageDetailResponse);
                    if (pageDetailResponse.status) {
                        pageDetails = pageDetailResponse;
                        PageProperties.store('pageDetails', pageDetails);
                        PageProperties.store('pageDetailsStatus', true);
                        $scope.$broadcast('pageDetails');
                    } else {
                        //TODO Better way of asking
                        if(!alert("Please refresh the page since the page was opened before Notocol was installed")) vm.closePopup();
                    }
                })

            });

        this.closePopup = function () {
            window.close();
        }
    }]);

    app.controller("BookmarkCtrl", ['$scope', '$timeout', 'PageProperties', '$http', function ($scope, $timeout, PageProperties, $http) {
        var vm = this;
        $scope.$on('pageDetails', function () {
            vm.pageDetails = PageProperties.get('pageDetails');
            if (vm.pageDetails.tags != null) {
                var tag = "";
                for (var i = 0; i < vm.pageDetails.tags.length; i++)
                    tag += vm.pageDetails.tags[i] + ",";

                tag = tag.substring(0, tag.length - 1);

            }
            console.log("Received vm.pageDetails as "+JSON.stringify(vm.pageDetails));
        });

        GetPageImages = function () {
            var filePath = 'public/scripts/notocol/sendImageList.js';
            var jqueryFile = 'public/scripts/vendor/jquery.min.js';
            
            var inputVariables = {
                sourceURI: vm.pageDetails.url,
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


        this.savePage = function () {
            
            var sourceDetails = vm.pageDetails
            if (sourceDetails.tags != null && typeof sourceDetails.tags.split != "undefined")
                sourceDetails.tags = sourceDetails.tags.split(',');
            

            $http.post(baseURL + "api/Source/UpdateSource", sourceDetails).
                success(function (data, status, headers, config) {
                    console.log("Saved page");
                    chrome.extension.sendMessage({
                        greeting: "PageDetailsUpdated",
                        
                        pageInfo: pageDetails
                    }, function () { alert("Message Send");});
                    if (vm.pageDetails.url.indexOf(".pdf") < 0)
                       GetPageImages();
                    
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
})();
    

