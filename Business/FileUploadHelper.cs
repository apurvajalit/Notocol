using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using Model;
using Model.Extended;
using Model.Extended.Extension;
using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class FileUploadHelper
    {
        public const string tempUploadDirSuffix = "App_Data\\TmpFileUploads\\";
        public const string finalUploadDirSuffix = "App_Data\\FileUploads\\";
        public const string getFileControllerAction = "File/GetFile/";

        //Error Codes
        public const int FILE_BAD_PASSWORD = 0;
        public const int FILE_UPLOAD_FAILED = 1;
        public const int FILE_UPLOAD_SUCCESS = 2;
        public const int FILE_PROCESS_FAILED = 3;
        public const int FILE_PROCESS_INVALID = -1;

        private string GetTempFilePathFromCode(string code)
        {
            return GetTempFileUploadPath() + code;
        }

        public int ProcessFileUpload(FileUploadData fileData, out string fileAccessLink)
        {
            int versionNumber = 0;
            UploadedFileMappingRepository objRepository = new UploadedFileMappingRepository();
            fileAccessLink = null;
            int retValue = FILE_PROCESS_INVALID;
            string currentFilePath = GetTempFilePathFromCode(fileData.code);

            try{
                string finalFilePath = AppDomain.CurrentDomain.BaseDirectory + finalUploadDirSuffix + fileData.code;

                File.Copy(currentFilePath, finalFilePath);
                versionNumber = objRepository.GetNextVersionNumber(fileData.userID, fileData.originalFileName);
            }
            catch
            {
                retValue = FILE_UPLOAD_FAILED;
            }

            if (retValue == FILE_PROCESS_INVALID)
            {
                UploadedFileMapping fileMapping = new UploadedFileMapping();

                fileMapping.Title = fileData.title;
                fileMapping.Uri = fileData.uri;
                fileMapping.userID = fileData.userID;
                fileMapping.LocalFileName = fileData.code;
                fileMapping.FileNameForLink = fileData.originalFileName;
                fileMapping.UploadedDate = DateTime.Now;
                fileMapping.Version = versionNumber;
                fileAccessLink = GetFileAccessLink(fileMapping);
                objRepository.AddMapping(fileMapping);
                if (AddMappedFileToSource(fileMapping, fileData.userName, fileData.description))
                {
                    retValue = FILE_UPLOAD_SUCCESS;
                    
                }
                else
                {
                    retValue = FILE_UPLOAD_FAILED;
                }    
            }
            return retValue;
        }

        private string GetFileAccessLink(UploadedFileMapping fileMapping)
        {
            return "https://notocol.tenet.res.in:8443/" + getFileControllerAction + fileMapping.userID + "/" + fileMapping.Version + "/" + fileMapping.FileNameForLink;
        }
        private bool AddMappedFileToSource(UploadedFileMapping mappedFile, string userName, string description = null)
        {

            SourceDataForExtension sourceInfo = new SourceDataForExtension
            {
                sourceID = 0,
                sourceUserID = 0,
                folderData = null,
                faviconUrl = null,
                noteCount = 0,
                privacy = false,
                summary = description,
                title = mappedFile.Title == null || mappedFile.Title.Length <= 0 ? mappedFile.FileNameForLink : mappedFile.Title,
                uri = mappedFile.Uri,
                url = GetFileAccessLink(mappedFile)
            };
            SourceHelper helper = new SourceHelper();
            
            sourceInfo = helper.SaveSource(sourceInfo, mappedFile.userID, userName);
            if (sourceInfo.sourceUserID > 0) return true;
            else return false;
        }

        public static string GetTempFileUploadPath()
        {
            
            return AppDomain.CurrentDomain.BaseDirectory + tempUploadDirSuffix; 
        }

        public string GetFilePath(long userId, int version, string fileName)
        {
            string localFileName = new UploadedFileMappingRepository().GetFileLocalName(userId, version, fileName);
            if (localFileName == null)
            {
                return null;
            }
            else
            {
                return AppDomain.CurrentDomain.BaseDirectory + finalUploadDirSuffix + localFileName;
            }
        }

        public string GetTempFilePath(string code)
        {
            return GetTempFilePathFromCode(code);
        }

        public void SendUploadFileEvent(string userName)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            new ActivityTracker().TrackEvent("UploadedFile", userName, properties);
        }
    }

}
