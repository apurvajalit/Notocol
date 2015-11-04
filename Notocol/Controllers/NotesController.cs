using Business;
using Model;
using Model.Extended;
using Model.Extended.Extension;
using Newtonsoft.Json.Linq;
using Notocol.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notocol.Controllers
{

    public class NotesController : Controller
    {
               
        public ActionResult NoteList(long sourceID, long userIDAtTop = 0, bool ownAtTop = false)
        {
            AnnotationHelper notesHelper = new AnnotationHelper();
            List<NoteData> response = new List<NoteData>();
            long userID = (userIDAtTop != 0) ? userIDAtTop : (ownAtTop)?Utility.GetCurrentUserID():0;

            AnnotationRepository objAnnotationRepository = new AnnotationRepository();

            SourceHelper sourceHelper = new SourceHelper();
            response = sourceHelper.GetSourceUserSummaries(sourceID, userID);
            response.AddRange(notesHelper.GetNoteList(sourceID, userID));

            return PartialView("NoteList", response);

        }

        public ActionResult Note(long ID = 0)
        {
            Annotation annotation = new AnnotationRepository().GetAnnotation(ID);
            if (annotation == null)
                return View(annotation);   //passing null
            return View(new AnnotationHelper().GetNoteData(annotation));
        }
    }
}