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
            tagClickHandler : '&',
            openPageDetails: '&',
            userClickHandler: '&'
        },
        
        templateUrl: "/Templates/Source/PageTile"
    };
});

app.directive("longPageTile", function () {
    return {
        restrict: 'E',
        scope: {
            page: '=',
            tagClickHandler: '&',
            openPageDetails: '&',
            userClickHandler: '&'
        },

        templateUrl: "/Templates/Source/LongPageTile"
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

var tabController = app.controller("TabController", function ($scope) {
    $scope.tabSelected = 'own';
    //console.log($scope.$parent.searchData);
    $scope.IsActiveTab = function (tab) {
        if ($scope.tabSelected == tab) return true;
        else return false;
    }
    $scope.OnClickTab = function (tab) {
        if ($scope.tabSelected != tab) {
            $scope.tabSelected = tab;
            $scope.$parent.searchData.tabSelected = $scope.tabSelected;
            console.log("Tab selected is " + $scope.$parent.searchData.tabSelected);

        }

    }
});

var pageController = app.controller("PageController", function ($scope, $http, $compile, $window) {
    
    $scope.loadCompletePending = 0;
    $scope.pageList = [];
    $scope.$parent.searchAvailable = true;
    $scope.loadingError = false;

    var GetPageList = function () {
        $scope.loading = true;
        console.log($scope.$parent.searchData);
        $http({
            method: 'POST',
            url: '/Source/GetSourceList',
            data: {
                'filter': {
                    query: $scope.$parent.searchData.searchString,
                    tags: $scope.$parent.searchData.tags
                },
                "sourceType": $scope.$parent.searchData.tabSelected == 'own' ? 1 : 3
            }

        }).then(function successCallback(response) {

                $scope.pageList = response.data;
                console.log("Page list lenght is  " + $scope.pageList.length);
                $scope.loadCompletePending = response.data.length;
                $scope.loading = false;
                $scope.loadingError = false;
                $scope.updateLayout();
            
            
        }, function errorCallback(response) {
            $scope.loadingError = true;
            //console.log("something seems wrong with status " + status + " " + xhr);
            setTimeout(GetPageList, 5000);

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
            $scope.loading = false;
        },
        fail: function (instance) { }
    };

    $scope.RefreshPageList = GetPageList;
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
    $scope.$watch('searchData', function () {
        console.log("Search values changed, fetching data")
        GetPageList();
    }, true);
    
    $scope.userClickHandler = function (userName) {
        $window.location = '/User/Profile?u=' + userName;
    }
});
