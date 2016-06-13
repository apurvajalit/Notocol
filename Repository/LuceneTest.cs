//using Lucene.Net.Index;
//using Lucene.Net.Store;
////using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Model;
//using Lucene.Net.Search;
//using Lucene.Net.Documents;
//using Lucene.Net.Analysis.Standard;
//using Lucene.Net.Analysis;
//using Lucene.Net.Util;
//using System;
//using Lucene.Net.QueryParsers;

//namespace Repository
//{
//    public class LuceneTest
//    {
//        private static string _luceneDir = Path.Combine("C:\\Users\\apurva.jalit\\Workspace\\Notocol", "lucene_index");
//        private static FSDirectory _directoryTemp;
//        private static FSDirectory _directory
//        {
//            get
//            {
//                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
//                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
//                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
//                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
//                return _directoryTemp;
//            }
//        }

//        private static void _addToLuceneIndex(Source sampleData, IndexWriter writer)
//        {
//            // remove older index entry
//            var searchQuery = new TermQuery(new Term("ID", sampleData.ID.ToString()));
//            writer.DeleteDocuments(searchQuery);

//            // add new index entry
//            var doc = new Document();

//            // add lucene fields mapped to db fields
//            doc.Add(new Field("ID", sampleData.ID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
//            doc.Add(new Field("Title", sampleData.title, Field.Store.YES, Field.Index.ANALYZED));
//            doc.Add(new Field("Link", sampleData.url, Field.Store.YES, Field.Index.ANALYZED));

//            // add entry to index
//            writer.AddDocument(doc);
//        }

//        public static void AddUpdateLuceneIndex(IEnumerable<Source> sampleDatas)
//        {
//            // init lucene
//            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
//            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
//            {
//                // add data to lucene search index (replaces older entry if any)
//                foreach (var sampleData in sampleDatas) _addToLuceneIndex(sampleData, writer);

//                // close handles
//                analyzer.Close();
//                writer.Dispose();
//            }
//        }

//        public static void AddUpdateLuceneIndex(Source source)
//        {
//            AddUpdateLuceneIndex(new List<Source> { source });
//        }

//        public static void ClearLuceneIndexRecord(int record_id)
//        {
//            // init lucene
//            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
//            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
//            {
//                // remove older index entry
//                var searchQuery = new TermQuery(new Term("ID", record_id.ToString()));
//                writer.DeleteDocuments(searchQuery);

//                // close handles
//                analyzer.Close();
//                writer.Dispose();
//            }
//        }
//        public static bool ClearLuceneIndex()
//        {
//            try
//            {
//                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
//                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
//                {
//                    // remove older index entries
//                    writer.DeleteAll();

//                    // close handles
//                    analyzer.Close();
//                    writer.Dispose();
//                }
//            }
//            catch (Exception)
//            {
//                return false;
//            }
//            return true;
//        }

//        public static void Optimize()
//        {
//            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
//            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
//            {
//                analyzer.Close();
//                writer.Optimize();
//                writer.Dispose();
//            }
//        }

//        private static Source _mapLuceneDocumentToData(Document doc)
//        {
//            return new Source
//            {
//                ID = Convert.ToInt32(doc.Get("ID")),
//                title = doc.Get("Title"),
//                url = doc.Get("Link")
//            };
//        }

//        private static IEnumerable<Source> _mapLuceneToDataList(IEnumerable<Document> hits)
//        {
//            return hits.Select(_mapLuceneDocumentToData).ToList();
//        }
//        private static IEnumerable<Source> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits,
//            IndexSearcher searcher)
//        {
//            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
//        }

//        private static Query parseQuery(string searchQuery, QueryParser parser)
//        {
//            Query query;
//            try
//            {
//                query = parser.Parse(searchQuery.Trim());
//            }
//            catch (ParseException)
//            {
//                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
//            }
//            return query;
//        }

//        private static IEnumerable<Source> _search
//    (string searchQuery, string searchField = "")
//        {
//            // validation
//            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<Source>();

//            // set up lucene searcher
//            using (var searcher = new IndexSearcher(_directory, false))
//            {
//                var hits_limit = 1000;
//                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

//                // search by single field
//                if (!string.IsNullOrEmpty(searchField))
//                {
//                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, analyzer);
//                    var query = parseQuery(searchQuery, parser);
//                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
//                    var results = _mapLuceneToDataList(hits, searcher);

//                    Filter shirts = new CachingWrapperFilter(
//                    new QueryWrapperFilter(
//                      new TermQuery(
//                        new Term("type", "shirt"))));

//                    BooleanQuery skuQuery = new BooleanQuery();
//                    skuQuery.Add(new TermQuery(new Term("size", "small")), Occur.MUST);
//                    skuQuery.Add(new TermQuery(new Term("color", "blue")), Occur.MUST);

//                    BlockJoinQuery skuJoinQuery = new BlockJoinQuery(
//    skuQuery,
//    shirts,
//    ScoreMode.None);
//                    analyzer.Close();
//                    searcher.Dispose();
//                    return results;
//                }
//                // search by multiple fields (ordered by RELEVANCE)
//                else
//                {
//                    var parser = new MultiFieldQueryParser
//                        (Version.LUCENE_30, new[] { "Id", "Name", "Description" }, analyzer);
//                    var query = parseQuery(searchQuery, parser);
//                    var hits = searcher.Search
//                    (query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
//                    var results = _mapLuceneToDataList(hits, searcher);
//                    analyzer.Close();
//                    searcher.Dispose();
//                    return results;
//                }
//            }
//        }

//        public static IEnumerable<Source> Search(string input, string fieldName = "")
//        {
//            if (string.IsNullOrEmpty(input)) return new List<Source>();

//            var terms = input.Trim().Replace("-", " ").Split(' ')
//                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
//            input = string.Join(" ", terms);

//            return _search(input, fieldName);
//        }

//        public static IEnumerable<Source> SearchDefault(string input, string fieldName = "")
//        {
//            return string.IsNullOrEmpty(input) ? new List<Source>() : _search(input, fieldName);
//        }

//        public static IEnumerable<Source> GetAllIndexRecords()
//        {
//            // validate search index
//            if (!System.IO.Directory.EnumerateFiles(_luceneDir).Any()) return new List<Source>();

//            // set up lucene searcher
//            var searcher = new IndexSearcher(_directory, false);
//            var reader = IndexReader.Open(_directory, false);
//            var docs = new List<Document>();
//            var term = reader.TermDocs();
//            while (term.Next()) docs.Add(searcher.Doc(term.Doc));
//            reader.Dispose();
//            searcher.Dispose();
//            return _mapLuceneToDataList(docs);
//        }

//    }
//}
