var app = angular.module('notocolApp', ['siyfion.sfTypeahead']);
app.controller('SearchBoxController', function($scope) {
	
$scope.selectedQuery = null;
  
  // instantiate the bloodhound suggestion engine
    var phrases = new Bloodhound({
    datumTokenizer: function(d) { return Bloodhound.tokenizers.whitespace(d.phrase); },
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    local: [
	  { phrase : 'when'},
	  { phrase : 'where'},
	  { phrase : 'how'},
	  { phrase : 'why'}
	  
    ]
  });

  
  
  var tags = new Bloodhound({
    datumTokenizer: function(d) { return Bloodhound.tokenizers.whitespace(d.tag); },
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    local: [
	  { tag : 'work'},
	  { tag : 'love'},
	  { tag : 'life'},
	  { tag : 'happy'},
	  { tag : 'sad'}
    ]
  });
  
  var folders = new Bloodhound({
    datumTokenizer: function(d) { return Bloodhound.tokenizers.whitespace(d.folder); },
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    local: [
	  { folder : 'notocol'},
	  { folder : 'design'},
	  { folder : 'algorithm'},
	  { folder : 'development'},
	  { folder : 'promotion'},
	  { folder : 'marketting'}
    ]
  }); 
   
  // initialize the bloodhound suggestion engine
  //numbers.initialize();
  tags.initialize();
  folders.initialize();
  phrases.initialize();
	
  $('#multiple-datasets .typeahead').typeahead({
	highlight: true
   },
   {
	name: 'phrase-suggest',
	display: 'phrase',
	source: phrases
  },
   {
	name: 'tag-suggest',
	display: 'tag',
	source: tags,
	templates: {
		header: '<h3 class="league-name">Tags</h3>'
	}
  },
  {
   name: 'folder-suggest',
   display: 'folder',
   source: folders,
   templates: {
     header: '<h3 class="league-name">Folders</h3>'
   }
 });
  
});

var pageTileDirective = {
    restrict: 'E', 
    scope: {
        page: '=',
        //user: '=',
        //deleteSource: '&'

    },
    //controller: function ($scope) {
    //    $scope.deletePage = function (sourceUserID) {
    //        if (typeof $scope.deleteSource == "function") {
    //            $scope.deleteSource({ sourceUserId: sourceUserID });
    //        }
    //    }

    //},
    templateUrl: "/Templates/Source/PageTile"
};

app.directive("pageTile", function () {
    return pageTileDirective;
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
app.filter('capitalizeFirst', function () {
    return function (input) {
        return (!!input) ? input.charAt(0).toUpperCase() + input.substr(1).toLowerCase() : '';
    }
});

app.filter('truncateStringWithEllipsis', function () {
    return function (input, maxCharacters) {
        if (input && input.length > maxCharacters) {
            // Replace this with the real implementation
            return input.substring(0, maxCharacters) + '...';
        } else
            return input;
    }
});

var homeController = app.controller("HomeController", function ($scope, $http) {
    $scope.filter = {};
    $scope.pageList = [];
    $scope.loading = true;
    $scope.loadingError = false;
    $scope.tabSelected = 'own';
    $scope.initCollectionTree = function () {
        ddtreemenu.createTree("treemenu1", true)
    }

    

    $scope.IsActiveTab = function (tab) {
        if ($scope.tabSelected == tab) return true;
        else return false;
    }
    $scope.OnClickTab = function (tab) {
        if ($scope.tabSelected != tab) {
            $scope.tabSelected = tab;
            $scope.GetPageList();
            
        }
        
    }
    $scope.GetPageList = function () {
        $http({
            method: 'POST',
            url: '/Source/GetSourceList',
            data: {
                'filter': $scope.filter,
                "sourceType": $scope.tabSelected == 'own' ? 1 : 3
            }

        }).then(function successCallback(response) {
            $scope.pageList = response.data;
            $scope.loading = false;
            $scope.loadingError = false;
            $scope.updateLayout();
        }, function errorCallback(response) {
            $scope.loadingError = true;
            console.log("something seems wrong with status " + status + " " + xhr);
            setTimeout($scope.GetPageList, 5000);

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

});
