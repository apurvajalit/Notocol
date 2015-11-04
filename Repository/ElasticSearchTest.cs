using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Model;
using Model.Extended;
using System.IO;
using log4net;



namespace Repository.Search
{
    [ElasticType(Name = "userfolder")]
    public class ESUserFolder{
        public long id { get; set; }
        public SuggestField folder_suggest { get; set; }
        public string userID { get; set; }
        
    }

    [ElasticType(Name = "user")]
    public class ESUser
    {
        public long id { get; set; }
        public SuggestField user_suggest { get; set; }
        //public long[] followees { get; set; }
    }

    [ElasticType(Name = "keyphrase")]
    public class ESKeyPhrase
    {
        public long hitCount { get; set; }
        public SuggestField keyphrase_suggest { get; set; }
    }

    public class ESSourceHText
    {
        [ElasticProperty(Index = FieldIndexOption.No)]
        public long aid { get; set; }
        public string htext { get; set; }
    }
    
    [ElasticType( Name = "source")]
    public class ESSource
    {
        [ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public long Id { get; set; }
        public string title { get; set; }

        [ElasticProperty(Index = FieldIndexOption.No)]
        public string link { get; set; }

        //TODO Following property is used just to return results. Need NOT be mapped in the index at all
        [ElasticProperty(Index = FieldIndexOption.No)]
        public long sourceUserID { get; set; } 

        public ESSourceHText[] hTexts { get; set; }
        
        [ElasticProperty(Index = FieldIndexOption.No)]
        public long popularity { get; set; }

        [ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public DateTime lastUsed { get; set; }

        public string tnText { get; set; }

        [ElasticProperty(Index = FieldIndexOption.No)]
        public string faviconURL { get; set; }

        [ElasticProperty(Index = FieldIndexOption.No)]
        public string tnImage { get; set; }

        [ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public string[] tags { get; set; }

        //[ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public SuggestField tag_suggest { get; set; }

        [ElasticProperty(Index = FieldIndexOption.No)]
        public string[] publicUserNames { get; set; }
    }

    public class ESSourceUserNotes{
        public string note { get; set; }
        public long noteID { get; set; }
    }

    public class SuggestField
    {
        public IEnumerable<string> Input { get; set; }
        public string Output { get; set; }
        public object Payload { get; set; }
        public int? Weight { get; set; }

    }

    [ElasticType(Name = "sourceuser")]
    public class ESSourceUser
    {
        [ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public long id { get; set; }

        public string note { get; set; }
  
        [ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public long userID { get; set; }
        
        public ESSourceUserNotes[] usernotes { get; set; }
         
        [ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public string[] tags { get; set; }

        [ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public DateTime lastUsed { get; set; }
    }

    public class ElasticSearchTest
    {
        const string update_s_note = "if(ctx._source.usernotes == null){ctx._source.usernotes = [usernote];}else{var i = 0;for(;i<ctx._source.usernotes.length;i++){if(ctx._source.usernotes[i].noteID == usernote.noteID){break ;}}ctx._source.usernotes[i] = usernote;}";

        const string update_s_htext = "if(ctx._source.hTexts == null){ctx._source.hTexts = [sourcehtext];}else{var i = 0, update = true;for(;i<ctx._source.hTexts.length;i++){if(~ctx._source.hTexts[i].htext.indexOf(sourcehtext.htext)){update = false;break;}if(~sourcehtext.htext.indexOf(ctx._source.hTexts[i].htext)){break ;}}if(update) ctx._source.hTexts[i] = sourcehtext;}";

        const string update_su_tag = "if(ctx._source.tags == null) ctx._source.tags = tags;else{var i=0;for(;i<tags.length;i++){if (ctx._source.tags.indexOf(tags[i]) < 0) {ctx._source.tags[ctx._source.tags.length] = tags[i]}}}";

        const string update_s_tag = "if(ctx._source.tags == null){ ctx._source.tags = tags;ctx._source.tag_suggest = {}; ctx._source.tag_suggest = {'input':tags};}else{var i=0;for(;i<tags.length;i++){if (ctx._source.tags.indexOf(tags[i]) < 0) {ctx._source.tags[ctx._source.tags.length] = tags[i]; ctx._source.tag_suggest.input[ctx._source.tag_suggest.input.length] = tags[i];}}}";

        const string update_s_add_public_user = "if(ctx._source.publicUserNames == null){ctx._source.publicUserNames = [username];}else{if(ctx._source.publicUserNames.indexOf(username) < 0) {ctx._source.publicUserNames[ctx._source.publicUserNames.length] = username}}";

        const string update_s_delete_public_user = "if(ctx._source.publicUserNames != null){ctx._source.publicUserNames.remove(username);}";

        const string update_s_tnImage = "if(ctx._source.tnImage == null) ctx._source.tnImage = tnImage;";
        const string update_s_tnText = "if(ctx._source.tnText == null) ctx._source.tnText = tnText;";


        const string update_u_add_followee = "if(ctx._source.followees == null){ctx._source.followees = [followee];}else{if(ctx._source.followees.indexOf(followee) < 0) {ctx._source.followees[ctx._source.followees.length] = followee}}";

        const string update_u_delete_followee = "if(ctx._source.followees != null){ctx._source.followees.remove(followee);}";

        static Uri uri = new Uri("http://localhost:9200/");//Address of local ElasticSearch instance. On a production ready code, address to access ElasticSearch should be a config entry
        public static ElasticClient Client = null;
        public static ConnectionSettings Settings = null;
        public ElasticSearchTest()
        {
			Settings = new ConnectionSettings(uri, "notocol");

			Client = new ElasticClient(Settings);
		}

		public static string Serialize<T>(T obj) where T : class
		{
			return Encoding.UTF8.GetString(Client.Serializer.Serialize(obj));
		}

		public static T Deserialize<T>(string json) where T : class
		{
			return Client.Serializer.Deserialize<T>(new MemoryStream(Encoding.UTF8.GetBytes(json)));
		}
	
        public void init()
        {



            var notocolIndex = Client.CreateIndex("notocol", null);
            var response = Client.Map<ESSource>(m => m
                                                .MapFromAttributes()
                                                .Properties(p => p
                                                    .Completion(c => c
                                                        .Name(cp => cp.tag_suggest)
                                                        .Payloads()
                                                    )));
                                         

            response = Client.Map<ESSourceUser>(m => m
                                                .MapFromAttributes()
                                                .SetParent<ESSource>());

            response = Client.Map<ESUserFolder>(m => m
                                                    .MapFromAttributes()
                                                    .Properties( p => p 
                                                        .Completion(c => c
                                                            .Name(cp => cp.folder_suggest)
                                                            .Payloads(true)
                                                            .Context(cc => cc
                                                                .Category("user", ccc => ccc
                                                                    .Path("userID"))))));

            response = Client.Map<ESUser>(m => m
                                                .MapFromAttributes()
                                                .Properties(mp => mp
                                                    .Completion(mpc => mpc
                                                        .Name(mpcn => mpcn.user_suggest)
                                                        .Payloads(false))));

            response = Client.Map<ESKeyPhrase>(m => m
                                                .MapFromAttributes()
                                                .Properties(mp => mp
                                                    .Completion(mpc => mpc
                                                        .Name(mpcn => mpcn.keyphrase_suggest)
                                                        .Payloads(false))));
              
        }

        public void AddUserToSource(SourceUser sourceuser)
        {
            ESSourceUser su = new ESSourceUser()
            {
                id = sourceuser.ID,
                userID = sourceuser.UserID,
                note = sourceuser.Summary,
                lastUsed = (DateTime)sourceuser.ModifiedAt
            };
            var response = Client.Index<ESSourceUser>(su, d => d.Parent(sourceuser.SourceID.ToString()));
            
        }

        public void DeleteUserForSource(SourceUser sourceuser)
        {
            var response = Client.Delete<ESSourceUser>(d => d
                                                    .Id(sourceuser.ID)
                                                    .Parent(sourceuser.SourceID.ToString()));
        }

        public void AddSourceSearchIndex(Source source, string tnText, string tnImageURL)
        {
            ESSource essource = new ESSource();
            essource.Id = source.ID;
            essource.title = source.title;
            essource.link = source.url;
            essource.faviconURL = source.faviconURL;
            essource.lastUsed = source.created;
            essource.popularity = 0;
            essource.tnImage = tnImageURL;
            essource.tnText = tnText;
            
            var response = Client.Index<ESSource>(essource);
        }

        public void UpdateSourceTNData(long sourceID, string tnText, string tnImageURL)
        {
            string script = "";
            if(tnText != null) script += update_s_tnText;
            if (tnImageURL != null) script += update_s_tnImage;

            var response = Client.Update<ESSource, object>(u => u
                                            .Id(sourceID)
                                            .Script(script)
                                            .Params(p => p
                                                    .Add("tnImage", tnImageURL)
                                                    .Add("tnText" , tnText)
                                            ));
        }

        public void AddSourceUserTagsData(long sourceID, long sourceUserID, string[] tags)
        {
            if (tags != null && tags.Length > 0)
            {
                var response = Client.Update<ESSourceUser, object>(u => u
                                        .Id(sourceUserID)
                                        .Parent(sourceID.ToString())
                                        .Script(update_su_tag)
                                        .Language("javascript")
                                        .Params(p => p.Add("tags", tags)));

                response = Client.Update<ESSource, object>(u => u
                                        .Id((long)sourceID)
                                        .Script(update_s_tag)
                                        .Language("javascript")
                                        .Params(p => p.Add("tags", tags)));
                
                
            }

        }

        
        public void UpdateNotesForSource(Annotation annotation, bool hTextChange, string[] tags = null)
        {
            string note = annotation.Text;
            

            if (hTextChange)
            {
                string hText = null;
                if (annotation.Target != null)
                {
                    var hTextTarget = JArray.Parse(annotation.Target)[0];

                    if (hTextTarget["selector"] != null)
                    {
                        foreach (var selector in hTextTarget["selector"])
                        {
                            if (selector["type"].ToString() != "TextQuoteSelector") continue;
                            hText = selector["exact"].ToString();
                            break;
                        }
                    }
                }
                if (hText != null && hText.Length > 0)
                {
                    ESSourceHText sourcehtext = new ESSourceHText();
                    sourcehtext.htext = hText;
                    sourcehtext.aid = annotation.ID;

                    var response = Client.Update<ESSource, object>(u => u
                                            .Id(annotation.SourceID)
                                            .Script(update_s_htext)
                                            .Language("javascript")
                                            .Params(p => p.Add("sourcehtext", sourcehtext)));
                }
            }
            
            ESSourceUserNotes usernote = null;
            if (note != null)
            {
                usernote = new ESSourceUserNotes
                {
                    noteID = annotation.ID,
                    note = annotation.Text,
                    
                };

                var response = Client.Update<ESSourceUser, object>(u => u
                                                .Id(annotation.SourceUserID)
                                                .Parent(annotation.SourceID.ToString())
                                                .Script(update_s_note)
                                                .Language("javascript")
                                                .Params(p => p.Add("usernote", usernote)));
                
            }

            if (tags != null && tags.Length > 0)
            {
                var response = Client.Update<ESSourceUser, object>(u => u
                                                               .Id(annotation.SourceUserID)
                                                               .Parent(annotation.SourceID.ToString())
                                                               .Script(update_su_tag)
                                                               .Language("javascript")
                                                               .Params(p => p.Add("tags", tags)));

                response = Client.Update<ESSource, object>(u => u
                                        .Id((long)annotation.SourceUserID)
                                        .Script(update_s_tag)
                                        .Language("javascript")
                                        .Params(p => p.Add("tags", tags)));
            }

        
        }
        
        public void UpdateSourceUserSummary(SourceUser sourceuser)
        {
            
           var response = Client.Update<ESSourceUser, object>(u => u
                                                  .Id(sourceuser.ID)
                                                  .Doc(new { note = sourceuser.Summary})
                                                  .Parent(sourceuser.SourceID.ToString())
                                                  .DocAsUpsert());
 
           
        }
        public List<ESSource> GetSourceFromOthers(SourceListFilter filter, long userID, int offset, int size)
        {
            bool applyTagFilter = (filter.tags != null) ? ((filter.tags.Length > 0) ? true : false) : false;
            bool applyUserFilter = (filter.user != null && filter.user.Length > 0)?true:false;
            List<QueryContainer> queryClauses = new List<QueryContainer>();
            
            if (!applyTagFilter && !applyUserFilter)
            {
                List<string> tagInterests = (from tags in new TagRepository().GetRecentTags(userID)
                                                    select tags.Name).ToList();

                List<object> followees = new List<object>();
                
                new FollowerRepository().GetAllFollowees(userID).ForEach(e => 
                    followees.Add((object)e));

                if (tagInterests != null && tagInterests.Count > 0)
                {
                    queryClauses.Add(new TermsQuery
                    {
                        Terms = tagInterests,
                        Field = "tags"
                    });
                }

                if (followees.Count > 0)
                {
                    queryClauses.Add(new HasChildQuery
                    {
                        Type = "sourceuser",
                        Query = new TermsQuery{
                                          Field = "userID",
                                          Terms = followees
                                }.ToContainer(),
                        ScoreType = ChildScoreType.Sum
                    });
                }
            }
            else
            {
                if (applyTagFilter)
                {
                    queryClauses.Add(new TermsQuery
                    {
                        Field = "tags",
                        Terms = filter.tags,
                        MinimumShouldMatch = "1"
                    });
                }
                
            }

            FilterContainer notOwnSourceFilter = new NotFilter
            {
                Filter = new HasChildFilter
                {
                    Type = "sourceuser",
                    Query = Query<ESSourceUser>.Term("userID", userID),

                }.ToContainer()
            };

            FilterContainer userSourceFilter = new TermFilter
            {
                Field = "publicUserNames",
                Value = filter.user
            };
           
            var query = Query<ESSource>

                            .Filtered(f =>
                            {
                                f.Filter(fq => { return (applyUserFilter)?userSourceFilter:notOwnSourceFilter; });
                                f.Query(q => q
                                    
                                   .Bool(qb => qb.Should(qbs =>
                                        {
                                            return new BoolQuery
                                            {
                                                Should = queryClauses
                                                
                                            };
                                        })));
                                    
                            });

            SearchDescriptor<ESSource> searchDescriptor = new SearchDescriptor<ESSource>();
            searchDescriptor.From(offset);
            searchDescriptor.Size(size);
            searchDescriptor.Query(query);
            if (applyTagFilter)
            {
                searchDescriptor.SortDescending("_score");
                searchDescriptor.SortDescending("lastUsed");
            }
            else
            {
                searchDescriptor.SortDescending("lastUsed");
                searchDescriptor.SortDescending("_score");
            }
            
            ISearchResponse<ESSource> searchResponse = Client.Search<ESSource>(searchDescriptor);

            if(applyUserFilter){
                ILog log = LogManager.GetLogger(GetType().Name);

                 log.Debug("Returning "+ searchResponse.Documents.ToList().Count+" results for user "+filter.user);
                 log.Debug("Result status: " + searchResponse.ServerError);
            }
            
            return searchResponse.Documents.ToList();

        }
        
        
        public List<ESSource> GetOwnSource(SourceListFilter filter, long userID, int offset, int size)
        {
            bool applyTagFilter = (filter.tags != null) ? ((filter.tags.Length > 0) ? true : false) : false;
            
            QueryContainer tagsQuery = new TermsQuery
            {
                Field = "tags",
                Terms = filter.tags,
                MinimumShouldMatch = "1"
            };

            //if (applyTagFilter)
            //{
            //    queryDescriptor.Query = new TermsQuery
            //                                {
            //                                    Field = "tags",
            //                                    Terms = filter.tags,
            //                                    MinimumShouldMatch = "1"
            //                                }.ToContainer();
            //}
            //else
            //{
            //    IList<KeyValuePair<PropertyPathMarker, ISort>> sortDescriptor = new List<KeyValuePair<PropertyPathMarker, ISort>>();

            //    sortDescriptor.Add(new KeyValuePair<PropertyPathMarker, ISort>(
            //        "lastUsed", new Sort{
            //            Order=SortOrder.Descending, 
            //            Field="lastUsed"}));

            //    queryDescriptor.InnerHits = new InnerHits
            //    {
                                      
            //        Sort = sortDescriptor
            //    };
            //}
            
            FilterContainer ownSourceFilter = new HasChildFilter
            {
                Type = "sourceuser",
                Query = Query<ESSourceUser>.Term("userID", userID),
                InnerHits = new InnerHits()

            };

            var query = Query<ESSource>
                           
                            .Filtered(f => {
                                f.Filter(fq => { return ownSourceFilter; });
                                if (applyTagFilter)
                                {
                                    f.Query(q =>
                                    {
                                        return Query<ESSource>.HasChild<ESSourceUser>(qc => qc
                                            .Score(ChildScoreType.Sum)
                                            .Query(qcq => { return tagsQuery; }));
                                    });    
                                }
                                
                                
                            });

            ISearchResponse<ESSource> searchResponse = Client.Search<ESSource>(s => s
                               .From(offset)
                               .Size(size)
                               .Query(query)
                            
                                );

            foreach(var hit in searchResponse.Hits){
                ESSourceUser su = hit.InnerHits.First().Value.Hits.Hits.First().Source.As<ESSourceUser>();
               hit.Source.sourceUserID = Convert.ToInt64(hit.InnerHits.First().Value.Hits.Hits.First().Id);
               hit.Source.tags = su.tags;
               if (su.note != null && su.note.Length > 0)
               {
                   hit.Source.tnText = su.note;
               }
            }
            return searchResponse.Documents.ToList();
        }

        public const int SOURCE_TYPE_OWN = 1;
        public const int SOURCE_TYPE_OTHERS = 2;
        public const int SOURCE_TYPE_ALL = 3;

        public List<ESSource> SearchUsingQuery(SourceListFilter filter, long userID, int sourceType, int offset, int size)
        {
            bool applyTagFilter = (filter.tags != null) ? ((filter.tags.Length > 0) ? true : false) : false;
            bool applyUserFilter = (filter.user != null && filter.user.Length > 0) ? true : false;
            List<ESSource> source = new List<ESSource>();
            QueryContainer tagsQuery = new TermsQuery
            {
                Field = "tags",
                Terms = filter.tags,
                MinimumShouldMatch = "1"
            };

            QueryContainer userQuery = new TermQuery
            {
                Field = "userID",
                Value = userID
            };


            
            List<QueryContainer> childMatchClauses = new List<QueryContainer>();
            childMatchClauses.Add(userQuery);
            childMatchClauses.Add(new MultiMatchQuery
            {
                Query = filter.query,
                //Fuzziness = 6, //TODO Need to set AUTO Here
                Fields = new PropertyPathMarker[] { "note", "usernotes.note" }
            });
            
            List<QueryContainer> sourceMatchClauses = new List<QueryContainer>();
            sourceMatchClauses.Add(new MultiMatchQuery()
            {
                Query = filter.query,
                //Fuzziness = 6, //TODO Need to set AUTO Here
                Fields = new PropertyPathMarker[] { "title", "hTexts.htext", "tnText" }
            });

            if (applyTagFilter){
                if (sourceType == SOURCE_TYPE_OWN) childMatchClauses.Add(tagsQuery);
                else sourceMatchClauses.Add(tagsQuery);
            }

            QueryContainer childMatchQuery = new BoolQuery { 
                    Must = childMatchClauses
            };

            QueryContainer sourceMatchQuery = new BoolQuery
            {
                Must = sourceMatchClauses
            };

            var searchQuery = Query<ESSource>
                                .Filtered(f =>
                                {
                                    if (sourceType == SOURCE_TYPE_OTHERS)
                                    {
                                        if (applyUserFilter)
                                        {
                                            f.Filter(uf => uf
                                                .Term("publicUserNames", filter.user));
                                        }
                                        else
                                        {
                                            f.Filter(of => of.Not(ofn => ofn.HasChild<ESSourceUser>(ofnc => ofnc.Query
                                            (ofncq => { return userQuery; }))));
                                        }
                                        
                                    }
                                    else if (sourceType == SOURCE_TYPE_OWN)
                                    {
                                        f.Filter(of => of.HasChild<ESSourceUser>(ofc => ofc
                                            .Query(ofqc => { return userQuery; })));
                                    }

                                    f.Query(fq => fq.Bool(b => b.Should(
                                        s => s.HasChild<ESSourceUser>(c => c
                                         .Score(ChildScoreType.Sum)
                                         .Query(cq => {return childMatchQuery;})
                                         .InnerHits(ih => ih
                                          .Highlight(h => h
                                            .PreTags("<b style='background-color:yellow'>")
                                            .PostTags("</b>")
                                            .OnFields(
                                                hf => hf.OnField("usernotes.note"),
                                                hf => hf.OnField("note"))))),
                                          
                                        s => { return sourceMatchQuery; }))); 
                                });

             
            
            ISearchResponse<ESSource> searchResponse = Client.Search<ESSource>(s =>s
                               .From(offset)
                               .Size(size)
                               .Query(searchQuery)
                               .Highlight(h=>h
                                   .PreTags("<b style='background-color:yellow'>")
                                   .PostTags("</b>")
                                   .OnFields( 
                                       f => f.OnField("tnText"),
                                       f => f.OnField("hTexts.htext"))));
                                
            if (searchResponse.Hits.Any())
            {
                //Get all the matches found in user's own data
                foreach (var hit in searchResponse.Hits)
                {
                    string tnText = "";
                    if (hit.InnerHits.Any())
                    {
                        foreach(var innerHit in hit.InnerHits){
                            if (!innerHit.Value.Hits.Hits.Any()) continue;
                            foreach(var childHighlightHolder in innerHit.Value.Hits.Hits){
                                if (!childHighlightHolder.Highlights.Any()) continue;
                                foreach (var highlight in childHighlightHolder.Highlights.SelectMany(highlight => highlight.Value.Highlights))
                                {
                                    tnText += highlight;
                                }
                            }
                        }
                    }
                    
                    if (hit.Highlights.Any())
                    {
                        foreach (var highlight in hit.Highlights.SelectMany(highlight => highlight.Value.Highlights))
                        {
                            tnText += highlight;
                        }
                    }

                    if (tnText.Length > 0) hit.Source.tnText = tnText;
                    source.Add(hit.Source);
                }
            }

            return source;
        }


        public void DeleteSourceUser(long sourceUserID, long sourceID)
        {
            Client.Delete<ESSourceUser>(d => d
                        .Id(sourceUserID)
                        .Parent(sourceID.ToString()));
        }


        internal IList<string> GetTagNames(string tagQuery)
        {
            if (tagQuery == null) return null;

            var completionSuggestDescriptor = new CompletionSuggestDescriptor<ESSource>()
                .OnField("tag_suggest")
                .Text("tagQuery");

            var result = Client.Search<ESSource>(s=> s
                                    
                                    .SuggestCompletion("suggest_tag", c=>c  
                                        .Text(tagQuery)
                                        .OnField("tag_suggest")));


            if (result.Suggest.Values.FirstOrDefault() != null)
            {
                if (result.Suggest.Values.FirstOrDefault()[0].Options.Count() > 0)
                {
                    return (from options in result.Suggest.Values.FirstOrDefault()[0].Options
                            select options.Text).ToList();
                }
            }
            //Assert.IsTrue(json.JsonEquals(expected), json);
            return null;
        }

        public void AddFolder(Folder folder)
        {
            ESUserFolder userFolder = new ESUserFolder();
            userFolder.id = folder.ID;
            userFolder.userID = ((long)folder.userID).ToString();
            userFolder.folder_suggest = new SuggestField();
            userFolder.folder_suggest.Input = new string[] { folder.name };

            var response = Client.Index<ESUserFolder>(userFolder);
            

        }

        internal IList<string> GetFolderSuggestions(string folderQuery, long userID)
        {
            var result = Client.Search<ESUserFolder>(s => s
                                    .SuggestCompletion("suggest_folder", c => c
                                        .Text(folderQuery)
                                        .OnField("folder_suggest")
                                        .Context(cc => cc.
                                           Add("user", userID.ToString())
                                        )));



            if (result.Suggest.Values.FirstOrDefault()[0].Options.Count() > 0)
            {
                return (from options in result.Suggest.Values.FirstOrDefault()[0].Options
                        select options.Text).ToList();
            }

            //Assert.IsTrue(json.JsonEquals(expected), json);
            return null;
        }

        public void AddUser(User user)
        {
            ESUser esuser = new ESUser();
            esuser.user_suggest = new SuggestField();
            esuser.id = user.ID;
            esuser.user_suggest.Input = new string[] { user.Name };
            var response = Client.Index<ESUser>(esuser);
        }

        internal IList<string> GetUserSuggestions(string userQuery)
        {
            var result = Client.Search<ESUser>(s => s
                                    .SuggestCompletion("suggest_user", c => c
                                        .Text(userQuery)
                                        .OnField("user_suggest")
                                        ));



            if (result.Suggest.Values.FirstOrDefault()[0].Options.Count() > 0)
            {
                return (from options in result.Suggest.Values.FirstOrDefault()[0].Options
                        select options.Text).ToList();
            }

            //Assert.IsTrue(json.JsonEquals(expected), json);
            return null;
        }

        public void AddKeyPhrase(string keyPhrase)
        {
            ESKeyPhrase esKeyPhrase = new ESKeyPhrase();

            esKeyPhrase.keyphrase_suggest = new SuggestField();

            esKeyPhrase.keyphrase_suggest.Input = new string[] { keyPhrase };
            var response = Client.Index<ESKeyPhrase>(esKeyPhrase);
        }

        internal IList<string> GetKeyPhraseSuggestions(string query)
        {
            var result = Client.Search<ESKeyPhrase>(s => s
                                    .SuggestCompletion("suggest_keyphrase", c => c
                                        .Text(query)
                                        .OnField("keyphrase_suggest")
                                        ));



            if (result.Suggest.Values.FirstOrDefault()[0].Options.Count() > 0)
            {
                return (from options in result.Suggest.Values.FirstOrDefault()[0].Options
                        select options.Text).ToList();
            }

            //Assert.IsTrue(json.JsonEquals(expected), json);
            return null;
        }

        internal void DeleteSource(long sourceID)
        {
            Client.Delete<ESSource>(d => d
                        .Id(sourceID));
                        
        }

        public void AddPublicUser(long sourceID, string username)
        {
            var response = Client.Update<ESSource, object>(u => u
                        .Id(sourceID)
                        .Script(update_s_add_public_user)
                        .Language("javascript")
                        .Params(p => p.Add("username", username)));

        }

        public void DeletePublicUser(long sourceID, string username)
        {
            var response = Client.Update<ESSource, object>(u => u
                        .Id(sourceID)
                        .Script(update_s_delete_public_user)
                        .Language("javascript")
                        .Params(p => p.Add("username", username)));

        }

        internal void AddFollower(long follower, long followee)
        {
            var response = Client.Update<ESUser, Object>(f => f
                        .Id(follower)
                        .Script(update_u_add_followee)
                        .Language("javascript")
                        .Params(p => p.Add("followee", followee)));
        }

        internal void DeleteFollower(long follower, long followee)
        {
            var response = Client.Update<ESUser, Object>(f => f
                        .Id(follower)
                        .Script(update_u_delete_followee)
                        .Language("javascript")
                        .Params(p => p.Add("followee", followee)));
        }

        
    }
}
