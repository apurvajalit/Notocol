var app = angular.module('notocolApp', ['siyfion.sfTypeahead', 'angular-images-loaded']);

var layoutController = app.controller("LayoutController", function ($scope, $http) {
    $scope.initCollectionTree = function () {
        ddtreemenu.createTree("treemenu1", true)
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

    GetRecentTags();
    GetUserCollections();

    $scope.searchData = {
        searchString: null,
        tags: [],
        collection: {
            id: 0,
            name: null
        },
        tabSelected : 'own'
    }

    
});