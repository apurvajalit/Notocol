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


namespace Repository.Search
{
    

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
    }

    public class ESSourceUserNotes{
        public string note { get; set; }
        public long noteID { get; set; }
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

        const string update_s_tag = "if(ctx._source.tags == null) ctx._source.tags = tags;else{var i=0;for(;i<tags.length;i++){if (ctx._source.tags.indexOf(tags[i]) < 0) {ctx._source.tags[ctx._source.tags.length] = tags[i]}}}";

        const string update_s_tnImage = "if(ctx._source.tnImage == null) ctx._source.tnImage = tnImage;";
        const string update_s_tnText = "if(ctx._source.tnText == null) ctx._source.tnText = tnText;";

        Uri uri = new Uri("http://localhost:9200/");//Address of local ElasticSearch instance. On a production ready code, address to access ElasticSearch should be a config entry
        ElasticClient client = null;
        
        public ElasticSearchTest()
        {
            ConnectionSettings setting = new ConnectionSettings(uri);

            setting.SetDefaultIndex("notocol"); //setting default index

            client = new ElasticClient(setting);
        }

        public void init()
        {
            
            

            var notocolIndex = client.CreateIndex("notocol", null); //incase index does not exist, create it
            var response = client.Map<ESSource>(m => m.MapFromAttributes());
            response = client.Map<ESSourceUser>(m => m
                                                .MapFromAttributes()
                                                .SetParent<ESSource>());
           
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
            var response = client.Index<ESSourceUser>(su, d => d.Parent(sourceuser.SourceID.ToString()));
            
        }

        public void DeleteUserForSource(SourceUser sourceuser)
        {
            var response = client.Delete<ESSourceUser>(d => d
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
            
            var response = client.Index<ESSource>(essource);
        }

        public void UpdateSourceTNData(long sourceID, string tnText, string tnImageURL)
        {
            string script = "";
            if(tnText != null) script += update_s_tnText;
            if (tnImageURL != null) script += update_s_tnImage;

            var response = client.Update<ESSource, object>(u => u
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
                var response = client.Update<ESSourceUser, object>(u => u
                                        .Id(sourceUserID)
                                        .Parent(sourceID.ToString())
                                        .Script(update_su_tag)
                                        .Language("javascript")
                                        .Params(p => p.Add("tags", tags)));

                response = client.Update<ESSource, object>(u => u
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

                    var response = client.Update<ESSource, object>(u => u
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

                var response = client.Update<ESSourceUser, object>(u => u
                                                .Id(annotation.SourceUserID)
                                                .Parent(annotation.SourceID.ToString())
                                                .Script(update_s_note)
                                                .Language("javascript")
                                                .Params(p => p.Add("usernote", usernote)));
                
            }

            if (tags != null && tags.Length > 0)
            {
                var response = client.Update<ESSourceUser, object>(u => u
                                                               .Id(annotation.SourceUserID)
                                                               .Parent(annotation.SourceID.ToString())
                                                               .Script(update_su_tag)
                                                               .Language("javascript")
                                                               .Params(p => p.Add("tags", tags)));

                response = client.Update<ESSource, object>(u => u
                                        .Id((long)annotation.SourceUserID)
                                        .Script(update_s_tag)
                                        .Language("javascript")
                                        .Params(p => p.Add("tags", tags)));
            }

        
        }
        
        public void UpdateSourceUserSummary(SourceUser sourceuser)
        {
            
           var response = client.Update<ESSourceUser, object>(u => u
                                                  .Id(sourceuser.ID)
                                                  .Doc(new { note = sourceuser.Summary})
                                                  .Parent(sourceuser.SourceID.ToString())
                                                  .DocAsUpsert());
 
           
        }

        public List<ESSource> GetSource(SearchFilter filter, long userID, bool own, int offset, int size)
        {
            bool applyTagFilter = (filter.tags != null) ? ((filter.tags.Length > 0) ? true : false) : false;
            QueryContainer tagsQuery = new TermsQuery
                                            {
                                                Field = "tags",
                                                Terms = filter.tags,
                                                MinimumShouldMatch = "1"
                                            };

            FilterContainer ownSourceFilter = new HasChildFilter
            {
                Type = "sourceuser",
                Query = Query<ESSourceUser>.Term("userID", userID),
                InnerHits = new InnerHits() //Needed to get hold of sourceUserID from the child doc
                
            };


            FilterContainer notOwnSourceFilter = new NotFilter
            {
                Filter = new HasChildFilter
                {
                    Type = "sourceuser",
                    Query = Query<ESSourceUser>.Term("userID", userID),
                    
                }.ToContainer()
            };

            var query = Query<ESSource>
                            .Filtered(f => {
                                f.Filter(fq => { if (own)return ownSourceFilter ; else return notOwnSourceFilter; });
                                    
                                if (applyTagFilter)
                                {
                                    if (own)
                                    {
                                        f.Query(q =>
                                        {
                                            return Query<ESSource>.HasChild<ESSourceUser>(qc => qc.Query(qcq => { return tagsQuery; }));
                                            
                                        });
                                    }
                                    else
                                    {
                                        f.Query(q =>
                                        {
                                            return tagsQuery;
                                        });
                                    }
                                }
                            });

            ISearchResponse<ESSource> searchResponse = client.Search<ESSource>(s => s
                               .From(offset)
                               .Size(size)
                               .Query(query));
                               
            if(!own) {
                return searchResponse.Documents.ToList();
            }

            
            foreach(var hit in searchResponse.Hits){
               hit.Source.sourceUserID = Convert.ToInt64(hit.InnerHits.First().Value.Hits.Hits.First().Id);
               hit.Source.tags = hit.InnerHits.First().Value.Hits.Hits.First().Source.As<ESSourceUser>().tags;
            }
                        
                        
                        
                    

            return searchResponse.Documents.ToList();

        }

        public const int SOURCE_TYPE_OWN = 1;
        public const int SOURCE_TYPE_OTHERS = 2;
        public const int SOURCE_TYPE_ALL = 3;

        public List<ESSource> SearchUsingQuery(string searchString, SearchFilter filter, long userID, int sourceType, int offset, int size)
        {
            bool applyTagFilter = (filter.tags != null) ? ((filter.tags.Length > 0) ? true : false) : false;
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
                Query = searchString,
                
                Fields = new PropertyPathMarker[] { "note", "usernotes.note" }
                
            });
            

            List<QueryContainer> sourceMatchClauses = new List<QueryContainer>();
            sourceMatchClauses.Add(new MultiMatchQuery
            {
                Query = searchString,
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
                                        f.Filter(of => of.Not(ofn => ofn.HasChild<ESSourceUser>(ofnc => ofnc.Query
                                            (ofncq => { return userQuery; }))));
                                    }
                                    else if (sourceType == SOURCE_TYPE_OWN)
                                    {
                                        f.Filter(of => of.HasChild<ESSourceUser>(ofc => ofc
                                            .Query(ofqc => { return userQuery; })));
                                    }

                                    f.Query(fq => fq.Bool(b => b.Should(
                                        s => s.HasChild<ESSourceUser>(c => c
                                         .Query(cq => { return childMatchQuery; })

                                         .InnerHits(ih => ih
                                          .Highlight(h => h
                                            .PreTags("<b style='background-color:yellow'>")
                                            .PostTags("</b>")
                                            .OnFields(
                                                hf => hf.OnField("usernotes.note"),
                                                hf => hf.OnField("note"))))),
                                          
                                        s => { return sourceMatchQuery; }))); 
                                });

             
            
            ISearchResponse<ESSource> searchResponse = client.Search<ESSource>(s =>s
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

        public List<ESSource> PopulateDefaultFeed(long userID, int offset = 0, int size = 40)
        {
            var query = Query<Source>.MatchAll();
            ISearchResponse<ESSource> searchResponse = client.Search<ESSource>(s =>s
                               .From(offset)
                               .Size(size)
                               .Query(query));
            
                                

            return searchResponse.Documents.ToList();
        }

        public void DeleteSourceUser(long sourceUserID, long sourceID)
        {
            client.Delete<ESSourceUser>(d => d
                        .Id(sourceUserID)
                        .Parent(sourceID.ToString()));
        }
        
    }
}
