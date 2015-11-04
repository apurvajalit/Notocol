using Business;
using iTextSharp.text.exceptions;
using Model.Extended;
using Newtonsoft.Json;
using Notocol.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Notocol.Controllers.Api
{
    public class FilesController : BaseApiController
    {

        
        [HttpPost] 
        public async Task<HttpResponseMessage> Upload()
        {
            string originalFileName;
            FileInfo uploadedFileInfo;

            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            
            var provider = GetMultipartProvider();

            try
            {
                var result = await Request.Content.ReadAsMultipartAsync(provider);
                originalFileName = GetDeserializedFileName(result.FileData.First());

                uploadedFileInfo = new FileInfo(result.FileData.First().LocalFileName);

            }
            catch
            {
                throw;
            }


            FileUploadData data = new FileUploadData
            {
                originalFileName = originalFileName,
                code = uploadedFileInfo.Name
            };

            return this.Request.CreateResponse(HttpStatusCode.OK, data);
            
        }
        
        [HttpPost]
        public HttpResponseMessage SaveUploadedFile(FileUploadData data)
        {

            FileUploadHelper uploadHelper = new FileUploadHelper();
            string fileAccessLink = null;
            data.userID = Utility.GetCurrentUserID();
            data.userName = Utility.GetCurrentUserName();
            int processResult = uploadHelper.ProcessFileUpload(data, out fileAccessLink);
            uploadHelper.SendUploadFileEvent(data.userName);

            string returnMessage = null;

            switch (processResult)
            {
                case FileUploadHelper.FILE_BAD_PASSWORD:
                    returnMessage = "Invalid Password, could not process the file";
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest, returnMessage);

                case FileUploadHelper.FILE_UPLOAD_FAILED:
                    returnMessage = "Server error, could not upload the file";
                    return this.Request.CreateResponse(HttpStatusCode.InternalServerError, returnMessage);


                case FileUploadHelper.FILE_PROCESS_FAILED:
                    returnMessage = "Invalid PDF, could not process the file";
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest, returnMessage);


                case FileUploadHelper.FILE_UPLOAD_SUCCESS:
                    returnMessage = fileAccessLink;
                    return this.Request.CreateResponse(HttpStatusCode.OK, returnMessage);

            }

            //File.Delete(uploadedFileInfo.FullName);
            return this.Request.CreateResponse(HttpStatusCode.InternalServerError, "Something terrible went wrong");

        }

        private string GetDescription(MultipartFormDataStreamProvider result)
        {
            if (result.FormData.HasKeys() && result.FormData["uploadPDFExtraData[description]"] != null)
            {
                var unescapedFormData = Uri.UnescapeDataString(result.FormData["uploadPDFExtraData[description]"]);
                return unescapedFormData;
            }

            return null;
        }

        // You could extract these two private methods to a separate utility class since
        // they do not really belong to a controller class but that is up to you
        private MultipartFormDataStreamProvider GetMultipartProvider()
        {

            //var root = HttpContext.Current.Server.MapPath(FileUploadHelper.GetTempFileUploadPath());
            Debug("11");
            var root = FileUploadHelper.GetTempFileUploadPath();
            Debug("12");
            //Directory.CreateDirectory(root);
            Debug("13");
            return new MultipartFormDataStreamProvider(root);
        }

        // Extracts Request FormatData as a strongly typed model
        private object GetFormData<T>(MultipartFormDataStreamProvider result)
        {
            if (result.FormData.HasKeys())
            {
                var unescapedFormData = Uri.UnescapeDataString(result.FormData.GetValues(0).FirstOrDefault() ?? String.Empty);
                if (!String.IsNullOrEmpty(unescapedFormData))
                    return JsonConvert.DeserializeObject<T>(unescapedFormData);
            }

            return null;
        }

        private string GetPassword(MultipartFormDataStreamProvider result)
        {
            if (result.FormData.HasKeys() && result.FormData["uploadPDFExtraData[password]"] != null)
            {
                var unescapedFormData = Uri.UnescapeDataString(result.FormData["uploadPDFExtraData[password]"]);
                return unescapedFormData;
            }

            return null;
        }

        private byte[] GetByteArrayFromString(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private string GetDeserializedFileName(MultipartFileData fileData)
        {
            var fileName = GetFileName(fileData);
            return JsonConvert.DeserializeObject(fileName).ToString();
        }

        public string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }
    }

}