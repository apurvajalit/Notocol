app.controller('SearchController', function ($scope, $window) {

    var tags = new Bloodhound({
        datumTokenizer: function (d) { return Bloodhound.tokenizers.whitespace(d.tag); },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: '/api/Tag/Tags?tagQuery=%QUERY',
            wildcard: '%QUERY'
        }
    });

    var users = new Bloodhound({
        datumTokenizer: function (d) { return Bloodhound.tokenizers.whitespace(d.tag); },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: '/api/User/GetUserNames?query=%QUERY',
            wildcard: '%QUERY'
        }
    });

    tags.initialize();
    users.initialize();

    var searchBox = angular.element('#multiple-datasets .typeahead');
    searchBox.typeahead({
        highlight: true
    },
     {
         name: 'tag-suggest',
         display: 'text',
         source: tags,
         templates: {
             header: '<div>Tags</div>'
         }
     },
     {
        name: 'user-suggest',
        display: 'text',
        source: users,
        templates: {
            header: '<div>Users</div>'
        }
     });

    var datumSelectedStatus = false;
    var datumData = null;
    searchBox.val($scope.$parent.searchData.searchString);
    searchBox.on(
        {
            'typeahead:selected': function (e, datum) {
                datumSelectedStatus = true;
                datumData = datum;
            },
            'keyup': function (e) {
                if (e.which == 13) {
                    searchBox.typeahead('close');
                    if (datumSelectedStatus) {
                        //Tag or user selected
                        if (datumData.type == 2) {
                            //tag
                            $scope.$parent.addTagFilter(datumData.text);
                            searchBox.val('');

                        } else {
                            //user
                            $window.location = 'User/Profile?u=' + datumData.text;
                        }
                    } else {
                        //treat as search string
                        var value = searchBox.typeahead('val');
                        $scope.$parent.addSearchQuery(value);
                        
                    }
                    datumSelectedStatus = false;
                    datumData = null;

                }

                
            }
        });

});
