"use strict";

var app = angular.module('notocolApp', ['ngFileUpload']);
app.controller('fileUploadController', ['$scope', 'Upload', '$timeout', '$sce', '$http', function ($scope, Upload, $timeout, $sce, $http) {
    $scope.uploadData = {};
    $scope.viewLink = null;
    $scope.submitEnable = false;
    $scope.fileUploadSuccess = false;
    $scope.uploadData = {};
    $scope.submitInit = false;
    $scope.submitSuccess = false;

    

    var baseUrl = "https://notocol.tenet.res.in:8443/";

    var uploadPdf = function (file) {
        $scope.errorMsg = null;
        file.result = null;
        file.upload = Upload.upload({
            url: "../api/Files/upload",
            method: "POST",
            file: file
            //data: {
            //    uploadPDFExtraData: {
            //        "password": $scope.password,
            //        "description" : $scope.description
            //    }
            //},
        });

        file.upload.then(function (response) {
            $timeout(function () {
                file.result = response.data;
                //$scope.viewLink = response.data;
                $scope.uploadData = response.data;
                $scope.fileUploadSuccess = true;
                $scope.submitEnable = true;
                $scope.tempFilePath = $sce.trustAsResourceUrl(baseUrl + "pdfViewer/web/viewer.html?file=" + baseUrl + "File/GetTempFile/" + $scope.uploadData.code);
            });
        }, function (response) {
            if (response.status > 0) {
                $scope.errorMsg = response.status + ': ' + response.data;
                $scope.uploadData = {};
            }
        }, function (evt) {
            // Math.min is to fix IE which reports 200% sometimes
            file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
        });
    }

    $scope.onFileSelect = function (file) {
        if (file != null) {
            console.log("File has been selected: " + file.name);
            $scope.selectedFileName = file.name
            uploadPdf(file);
        }
        
    }

    $scope.onSubmit = function (file) {
        $scope.submitInit = true;
        $scope.uploadData = {
            "originalFileName":$scope.uploadData.originalFileName,
            "code":$scope.uploadData.code,
            "description": $scope.description,
            "uri": window.frames["pdf-container"].window['notocolIDVal'],
            "title": window.frames["pdf-container"].window['notocolTitleVal'],

        }

        $http({
            url: "../api/Files/SaveUploadedFile",
            method: "POST",
            data: $scope.uploadData
        }).then(function (response) {
            $timeout(function () {
                //file.result = response.data;
                //$scope.viewLink = response.data;
                $scope.viewLink = response.data;
                $scope.submitInit = false;
                $scope.submitSuccess = true;

            });
        }, function (response) {
            $scope.submitInit = false;
            if (response.status > 0) {
                $scope.errorMsg = response.status + ': ' + response.data;
                //$scope.uploadData = {};
            }
        });
    }
}]);
