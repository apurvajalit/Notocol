using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Model;


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
        public long Id { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public string[] tags { get; set; }

        
        public ESSourceHText[] hTexts { get; set; }
        
        [ElasticProperty(Index = FieldIndexOption.No)]
        public long popularity { get; set; }

        [ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public DateTime lastUsed { get; set; }

        public string tnText { get; set; }

        [ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public string faviconURL { get; set; }
        [ElasticProperty(Index = FieldIndexOption.No)]
        public string tnImage { get; set; }
    }

    [ElasticType(Name = "sourceuser")]
    public class ESSourceUser
    {
        public string note { get; set; }
        public long userID { get; set; }
        public string id { get; set; }
    }

    public class ElasticSearchTest
    {
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

        public void AddSourceSearchIndex(Source source, string tnText, string tnImageURL, string[] tags)
        {
            ESSource essource = new ESSource();
            essource.Id = source.ID;
            essource.title = source.title;
            essource.link = source.url;
            essource.faviconURL = source.faviconURL;
            essource.lastUsed = source.created;
            essource.popularity = 0;
            essource.tags = tags;
            essource.tnImage = tnImageURL;
            essource.tnText = tnText;
            
            //TODO REMOVE THIS
            //essource.hTexts = new ESSourceHText[]{new ESSourceHText{aid = 1, htext="CHeck1"}, new ESSourceHText{aid=2, htext = "Check2"}};
            
            
            var response = client.Index<ESSource>(essource);
        }

        public void UpdateSourceTNData(long sourceID, string tnText, string tnImageURL)
        {
            var response = client.Update<ESSource, object>(u => u
                                            .Id(sourceID)
                                            .Doc(new {tnText = tnText, tnImage = tnImageURL })
                                            .DocAsUpsert()
                                            );
        }

        public void UpdateSourceTagsData(long sourceID, string[] tags)
        {
            var response = client.Update<ESSource, object>(u => u
                                            .Id(sourceID)
                                            .Upsert(new ESSource { tags = tags})
                                            );
        }

        public void AddSourceTagsData(long sourceID, string tag)
        {
            var response = client.Update<ESSource, object>(u => u
                                        .Id(sourceID)
                                        .Script("if(ctx._source.tags!= null){ctx._source.tags += tag}else{ctx._source.tags = [tag]}")
                                        .Params(p => p.Add("tag", tag)));
                                    
        }

        public void AddUpdateNotesForSource(Annotation annotation, bool hTextChange, SourceUser sourceuser = null, Source source = null)
        {
            long sourceID = 0;
            
            if (source != null) sourceID = source.ID;
            else if (sourceuser != null) sourceID = (long)sourceuser.SourceID;
            else{
                sourceuser = new SourceRepository().GetSourceUser(annotation.SourceUserID);
                if (sourceuser == null) return;
                sourceID = (long)sourceuser.SourceID;
            }
            
            
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
                                            .Id(sourceID)
                                            .ScriptFile("update-htext")
                                            .Params(p => p.Add("sourcehtext", sourcehtext)));
                }
            }

            if (note != null)
            {
                var response = client.Update<ESSourceUser, object>(u => u
                                                            .Id(GenerateSourceUserID(sourceuser.ID, annotation.ID))
                                                            .Doc(new {note = note, userID = annotation.UserID})
                                                            .Parent(sourceID.ToString())
                                                            .DocAsUpsert());
                
            }
        }
        private string GenerateSourceUserID(long sourceUserID, long aid)
        {
            return sourceUserID.ToString() + "-" + aid.ToString(); 
        }

        public void UpdateSourceUserSummary(SourceUser sourceuser)
        {
            
           var response = client.Update<ESSourceUser, object>(u => u
                                                  .Id(GenerateSourceUserID(sourceuser.ID, 0))
                                                  .Doc(new { note = sourceuser.Summary, userID = sourceuser.UserID })
                                                  .Parent(sourceuser.SourceID.ToString())
                                                  .DocAsUpsert());
 
           
        }

        public List<ESSource> SearchUsingQuery(string query, long userID, int offset, int size)
        {
             List<ESSource> source = new List<ESSource>();
             var searchQuery = Query<ESSource>
                                .Bool(b => b
                                    .Should(s => s
                                        .HasChild<ESSourceUser>(c => c
                                            .Query(cq => cq
                                                .Term("userID", userID)
                                                && cq.MultiMatch(cm => cm
                                                    .Query(query)
                                                    .OnFields(new string[] { "note"})
                                                )
                                             )
                                             .InnerHits(ih => ih
                                                .Highlight(h => h
                                                    .OnFields(hf => hf
                                                        .OnField("note")
                                                        .PreTags("<b style='background-color:yellow'>")
                                                        .PostTags("</b>")
                                                     )
                                                 )
                                             )
                                        )
                                        || s.MultiMatch(m => m
                                            .Query(query)
                                            .OnFields(new string[] { "title", "link", "hTexts.htext", "tags", "tnText" })
                                        )
                                        
                                    )
                                );
            
            ISearchResponse<ESSource> searchResponse = client.Search<ESSource>(s =>s
                               .From(offset)
                               .Size(size)
                               .Query(searchQuery)
                               .Highlight(h=>h
                                    .OnFields(f => f
                                        .OnField("hTexts.htext")
                                        .OnField("tnText")
                                        .PreTags("<b style='background-color:yellow'>")
                                        .PostTags("</b>")
                                     )
                                ));

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

        public List<ESSource> PopulateDefaultFeed(long userID, bool only_self, int offset = 0, int size = 40)
        {
            var query = Query<Source>.MatchAll();
            ISearchResponse<ESSource> searchResponse = client.Search<ESSource>(s =>s
                               .From(offset)
                               .Size(size)
                               .Query(query));
            
                                

            return searchResponse.Documents.ToList();
        }
        
    }
}
