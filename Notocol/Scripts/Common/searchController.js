app.controller('SearchController', function ($scope) {

    $scope.selectedQuery = null;

    // instantiate the bloodhound suggestion engine
    var phrases = new Bloodhound({
        datumTokenizer: function (d) { return Bloodhound.tokenizers.whitespace(d.phrase); },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        local: [
          { phrase: 'when' },
          { phrase: 'where' },
          { phrase: 'how' },
          { phrase: 'why' }

        ]
    });



    var tags = new Bloodhound({
        datumTokenizer: function (d) { return Bloodhound.tokenizers.whitespace(d.tag); },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        local: [
          { tag: 'work' },
          { tag: 'love' },
          { tag: 'life' },
          { tag: 'happy' },
          { tag: 'sad' }
        ]
    });

    var folders = new Bloodhound({
        datumTokenizer: function (d) { return Bloodhound.tokenizers.whitespace(d.folder); },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        local: [
          { folder: 'notocol' },
          { folder: 'design' },
          { folder: 'algorithm' },
          { folder: 'development' },
          { folder: 'promotion' },
          { folder: 'marketting' }
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
