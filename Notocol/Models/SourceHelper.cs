using Model;
using Model.Extended;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notocol.Models
{
    public class SourceHelper
    {
        static SourceRepository obSourceRepository = new SourceRepository();

        public static IList<SourceItem> GetSourceItems(string keywordFilter, IList<long> tagIDs , long userID, bool onlyUserSource = true){
            
            IList<SourceItem> retList = new List<SourceItem>();
            if(onlyUserSource)
                return obSourceRepository.Search(keywordFilter, tagIDs, userID);
            else { 
                
                return obSourceRepository.Search(keywordFilter, tagIDs);
            }
        }
    }
}