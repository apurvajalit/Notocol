var app = angular.module('h', ['ngTagsInput']);

var tags = [
  { "text": "Tag1" },
  { "text": "Tag2" },
  { "text": "Tag3" },
  { "text": "Tag4" },
  { "text": "Tag5" },
  { "text": "Tag6" },
  { "text": "Tag7" },
  { "text": "Tag8" },
  { "text": "Tag9" },
  { "text": "Tag10" }
];
app.controller('pageTagsCtrl', function ($scope, $http) {
    $scope.tags = [
      { text: 'Tag1' },
      { text: 'Tag2' },
      { text: 'Tag3' }
    ];

    $scope.loadTags = function (query) {
        return tags;
    };
});
