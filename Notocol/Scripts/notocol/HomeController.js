var homeController = notocolApp.controller("HomeController", function ($scope, $http) {
    $scope.filter = {};
    $scope.pageList = [];
    $scope.loading = true;
    $scope.loadingError = false;

    $scope.GetPageList = function () {
        $http({
            method: 'POST',
            url: '/Source/GetSourceList',
            data: {
                'filter': $scope.filter,
                "sourceType": tabSelected == 0 ? 1 : 3
            }

        }).then(function successCallback(response) {
            $scope.pageList = response.data;
            $scope.loading = false;
            $scope.loadingError = false;
        }, function errorCallback(response) {
            $scope.loadingError = true;
            console.log("something seems wrong with status " + status + " " + xhr);
            setTimeout($scope.GetPageList, 5000);

        });
        
    }

    $scope.GetPageList();

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

})

notocolApp.directive('page-tile-grid', function () {
    var directive = {
        restrict: 'E', // restrict this directive to elements
        scope: { pages: '=', userInfo: '=', max_col: '=', page_delete_func: '=' },
        controller:function(){
            console.log("In pageTile Grid");
        },
        //templateUrl: "Partial/Source/PageTileGrid"
        template: '<p>{{userInfo}}"</p>'
    };
    return directive;
});

notocolApp.directive('page-tile', function () {
    var directive = {
        restrict: 'E', // restrict this directive to elements
        scope: {
            page: '=',
            userInfo: '=',
            page_delete_func: '=',
            DeletePage: function(sourceUserID){
                if (typeof $scope.page_delete_func == "function") {
                    $scope.page_delete_func(sourceUserID);
                }
            }
        },
        controller: function(){
            console.log("In pageTile");
        },
        templateUrl: "Partial/Source/PageTile"
    };
    return directive;
});

notocolApp.directive('username-list', function () {
    var directive = {
        restrict: 'E', // restrict this directive to elements
        scope: { usernames: '=', max_chars: '=', omit_user: "@", refresh_func: "=" },
        controller: function ($scope) {
            var char_count = 0;
            $scope.visible_users = [];
            $scope.numTruncatedUsers = 0;
            $scope.truncatedUserList = "";
            
            for (var user in $scope.usernames) {
                if (user == $scope.omit_user) continue;

                if (char_count < $scope.max_chars) {
                    $scope.visible_users[$scope.visible_users.length] = user;
                    char_count += user.length;
                } else {
                    if ($scope.numTruncatedUsers > 0) $scope.numTruncatedUsers += ", "
                    $scope.numTruncatedUsers++;
                    $scope.truncatedUserList += user;
                }
            }
        },
        template: '<a href=# ng-repeat="user in visible_users">{{user}}<span ng-if="!$last">,</span></a>' +
                    '<a href=# ng-if="numTruncatedUsers > 0" title="truncatedUserList"> and {{numTruncatedUsers}} others </a>'
    };
    return directive;
})
