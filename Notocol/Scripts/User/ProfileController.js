app.directive("profilePageTile", function () {
    return {
        restrict: 'E',
        scope: {
            page: '=',
            openPageDetails: '&',
            tagClickHandler: '&',
            foo: '&'
        },
        controller : function($scope){
            
        },
        templateUrl: "/Templates/User/ProfilePageTile"
    };
});

app.controller("ProfileController", function ($scope, userFromService, $http) {
    
    $scope.user = angular.fromJson(userFromService);
    $scope.foo = function () {
        alert('FOo');
    }
    $scope.pages = [];
    $scope.ProfileImageUrl = function (userID) {
        return 'http://lorempixel.com/372/372/abstract/' + (userID % 10);
    }

    $http({
        method: 'GET',
        url: '/Source/GetSourceForProfile?userID='+$scope.user.ID
        

    }).then(function successCallback(response) {
        console.log(response);
        $scope.pages = response.data;
    }, function errorCallback(response) {
        
    });
    
});

