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
        // GET: Annotation
        //public IList<NoteData> Notes(string url, long userID)
        //{
        //    AnnotationRepository objAnnotationRepository = new AnnotationRepository();
        //    List<Annotation> annotations = objAnnotationRepository.getAnnotations(url, userID);
        //    IList<NoteData> notes = new List<NoteData>();

        //    return notes;
        //}
        private NoteData GetNoteData(Annotation annotation)
        {
            NoteData note = new NoteData();
            note.NoteText = annotation.Text;

            ExtensionAnnotationData extAnnData = Utility.AnnotationToExtensionAnnotation(annotation);
            if (extAnnData.target != null)
            {
                foreach (Selector selector in extAnnData.target[0].selector)
                {
                    if (selector.type == "TextQuoteSelector")
                    {
                        note.QuotedText = selector.exact;
                        break;
                    }
                }
            }
            note.pageURL = annotation.Uri;
            dynamic data = JObject.Parse(annotation.Document);
            note.pageTitle = data.title;
            
            return note;

        }
        public ActionResult NoteList(long sourceID)
        {
            AnnotationRepository objAnnotationRepository = new AnnotationRepository();
            IList<NoteData> notes = new List<NoteData>();
            IList<Annotation> annotations = objAnnotationRepository.GetAnnotationsForPage(sourceID);
            if (annotations.Count > 0)
            {
                foreach (Annotation annotation in annotations)
                {
                    NoteData note = GetNoteData(annotation);
                    
                    if ((note.NoteText != null && note.NoteText.Length > 0) || (note.QuotedText != null && note.QuotedText.Length > 0)) notes.Add(note);
                }
            }
            return PartialView("NoteList", notes);

        }

        public ActionResult Note(long ID = 0)
        {
            Annotation annotation = new AnnotationRepository().getAnnotation(ID);
            if (annotation == null)
                return View(annotation);   //passing null
            return View(GetNoteData(annotation));
        }
    }
}