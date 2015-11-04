using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UploadedFileMappingRepository:BaseRepository
    {
        public UploadedFileMappingRepository()
        {
            CreateDataContext();
        }
        public int GetNextVersionNumber(long userID, string currentFileName)
        {
            int nextFileVersioNumber = 0;
            try{
                using (GetDataContext())
                {
                    var result  = (from mappings in context.UploadedFileMappings
                               where mappings.userID == userID 
                                    && mappings.FileNameForLink == currentFileName
                                orderby mappings.Version descending
                                              select mappings.Version).FirstOrDefault();

                    if (result == null)
                        nextFileVersioNumber = 0;
                    else
                        nextFileVersioNumber = Convert.ToInt32(result) + 1;
                }      
            }catch{
                throw;
            }
            

            return nextFileVersioNumber;
        }

        public string GetFileLocalName(long userId, int version, string fileName)
        {
            UploadedFileMapping mapping = new UploadedFileMapping();
            try
            {
                using (GetDataContext())
                {
                    mapping = (from mappings in context.UploadedFileMappings
                               where mappings.userID == userId &&
                                     mappings.Version == version &&
                                     mappings.FileNameForLink == fileName
                               select mappings).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
            if (mapping != null) return mapping.LocalFileName;
            else return null;
        }

        public void AddMapping(UploadedFileMapping fileMapping)
        {
            try
            {
                using (GetDataContext())
                {
                    context.Entry(fileMapping).State = EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
