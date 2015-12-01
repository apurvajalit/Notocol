using Model.Extended;
using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class NotificationHelper
    {
        public const int NOTIFICATION_TYPE_OWN_CONTENT = 1;
        public const int NOTIFICATION_TYPE_NEW_CONTENT = 2;
        public const int NOTIFICATION_TYPE_ACTIVITY = 3;

        public const int NOTIFICATION_REASON_PAGE_SAVE = 1; //notify old users, followers
        public const int NOTIFICATION_REASON_TAG = 2; //notify tag followers
        public const int NOTIFICATION_REASON_NOTE = 4; //notify followers
        public const int NOTIFICATION_REASON_ANNOTATION = 8; //notify followers
        public const int NOTIFICATION_REASON_OWN_PAGE_SAVE = 16; //notify self


        private void UpdateNotificationsAsync(SourceUser sourceUser, int reasonCode, List<string> tags = null, string note = null)
        {
            if (reasonCode == 0) return;
            bool updateText = false;
            SourceHelper sourceHelper = new SourceHelper();
            NotificationRepository objRepo = new NotificationRepository();
            UserHelper userHelper = new UserHelper();
            
            List<long> followers = userHelper.GetAllFollowers(sourceUser.UserID);
            List<long> oldUsers = null;
            List<long> tagFollowers = null;

            
            Dictionary<long, NotificationTemp> notifications = new Dictionary<long, NotificationTemp>();

            if ((reasonCode & NOTIFICATION_REASON_PAGE_SAVE) != 0)
            {
                oldUsers = sourceHelper.GetSourceUsers((long)sourceUser.SourceID);
                foreach (var userID in oldUsers)
                {
                    if (userID == sourceUser.UserID) continue;
                    NotificationTemp notification = new NotificationTemp
                    {
                        ReadStatus = false,
                        Receiver = userID,
                        SecondaryUser = sourceUser.UserID,
                        SourceUserID = sourceUser.ID,
                        SourceID = (long)sourceUser.SourceID,
                        Type = NOTIFICATION_TYPE_OWN_CONTENT,
                        ReasonCode = NOTIFICATION_REASON_OWN_PAGE_SAVE,
                        Created = DateTime.Now
                    };

                    notifications.Add(userID, notification);
                }
            }
            if ((reasonCode & NOTIFICATION_REASON_TAG) != 0)
            {
                string tagString = "";
                if (tags != null && tags.Count > 0)
                {
                    foreach (var tag in tags)
                    {
                        tagString += tag + ",";
                    }
                }
                followers.AddRange(tagFollowers = new TagHelper().GetUsersForTags(tags).Except(followers).ToList());

                foreach (var userID in followers)
                {
                    if (userID == sourceUser.UserID) continue;

                    NotificationTemp notification = new NotificationTemp
                    {
                        ReadStatus = false,
                        Receiver = userID,
                        SecondaryUser = sourceUser.UserID,
                        SourceUserID = sourceUser.ID,
                        SourceID = (long)sourceUser.SourceID,
                        Type = NOTIFICATION_TYPE_NEW_CONTENT,
                        ReasonCode = NOTIFICATION_REASON_TAG,
                        Created = DateTime.Now,
                        tags = tagString
                    };

                    notifications.Add(userID, notification);

                }
            }

            if((reasonCode & (NOTIFICATION_REASON_PAGE_SAVE | NOTIFICATION_REASON_NOTE | NOTIFICATION_REASON_ANNOTATION)) != 0){
                foreach (var userID in followers)
                {
                    NotificationTemp notification = null;
                    notifications.TryGetValue(userID, out notification);
                    if (notification != null)
                    {
                        notification.ReasonCode |= (reasonCode & (NOTIFICATION_REASON_PAGE_SAVE | NOTIFICATION_REASON_NOTE | NOTIFICATION_REASON_ANNOTATION));
                        notifications[userID] = notification;
                        if (note != null)
                        {
                            notification.note = note;
                        }
                    }
                    else
                    {
                        notification = new NotificationTemp
                        {
                            ReadStatus = false,
                            Receiver = userID,
                            SecondaryUser = sourceUser.UserID,
                            SourceUserID = sourceUser.ID,
                            SourceID = (long)sourceUser.SourceID,
                            Type = NOTIFICATION_TYPE_NEW_CONTENT,
                            ReasonCode = reasonCode,
                            Created = DateTime.Now,
                            note = note
                        };

                        notifications.Add(userID, notification);
                    }
                }
            }
            if (notifications.Count > 0)
            {
                objRepo.AddOrUpdateNotifications(notifications.Values.ToList(), updateText);
            }
        }

        public void UpdateNotifications(SourceUser sourceUser, int reasonCode, List<string> tags = null, string text = null )
        {
            if ((bool)sourceUser.Privacy) return;

            var task = Task.Factory.StartNew(() =>
                UpdateNotificationsAsync(sourceUser, reasonCode, tags, text));

            //task.OnComplete(myErrorHandler, TaskContinuationOptions.OnlyOnFaulted)
        }



        internal void DeleteNotificationForSourceUser(long sourceUserID)
        {
            Task.Factory.StartNew(() =>
            new NotificationRepository().DeleteNotificationsForSourceUser(sourceUserID));
        }

        public List<NotificationDisplay> GetNotificationsForUser(long userID)
        {
            List<NotificationDisplay> notifications = new List<NotificationDisplay>();

            List<Notification> notificationsFromDB = new NotificationRepository().GetUserNotifications(userID);

            if(notificationsFromDB == null || notificationsFromDB.Count <= 0) return notifications;

            foreach (var notification in notificationsFromDB)
            {
                NotificationDisplay n  = new NotificationDisplay{
                    id = notification.ID,
                    notificationDate = notification.Created,
                    readStatus = notification.ReadStatus,
                    reasonCode = notification.ReasonCode,
                    secondaryUserID = notification.SecondaryUser != null? (long)notification.SecondaryUser: 0,
                    secondaryUserName = notification.User.Username,
                    sourceID = notification.SourceID,
                    sourceUserID = notification.SourceUserID != null? (long)notification.SourceUserID: 0,
                    sourceTitle = notification.Source.title,
                    note = notification.note,
                    tags = notification.tags
                };

                //string title = (n.sourceTitle.Length > 50) ? n.sourceTitle.Substring(0, 47) + "..." : n.sourceTitle;
                string userMarker = "$$u";
                string titleMarker = "$$t";
                string tagMarker = "$$tg";
                string noteMarker = "$$n";

                if ((notification.ReasonCode & NOTIFICATION_REASON_OWN_PAGE_SAVE) != 0){
                    n.notificationDetailText = userMarker + " also saved your page " + titleMarker;
                    if ((notification.ReasonCode & (NOTIFICATION_REASON_ANNOTATION | NOTIFICATION_REASON_NOTE)) != 0)
                    {
                        n.notificationDetailText += " and added note "+noteMarker;
                    }
                }
                else
                {
                    bool pageSave = false, tag = false;
                    n.notificationDetailText = userMarker;
                    if ((notification.ReasonCode & NOTIFICATION_REASON_PAGE_SAVE) != 0)
                    {
                        n.notificationDetailText += " saved page " + titleMarker;
                        pageSave = true;
                    }
                    if ((notification.ReasonCode & NOTIFICATION_REASON_TAG) != 0)
                    {
                        if (!pageSave)
                        {
                            n.notificationDetailText += " added tags "+tagMarker+" to page " + titleMarker;
                        }
                        else
                        {
                            n.notificationDetailText += " with tags " + tagMarker;
                        }
                        tag = true;
                        
                    }
                    if ((notification.ReasonCode & (NOTIFICATION_REASON_NOTE | NOTIFICATION_REASON_ANNOTATION)) != 0)
                    {
                        if (!pageSave && !tag)
                        {
                            n.notificationDetailText += " added note "+noteMarker+" to " + titleMarker;
                        }
                        else if (!pageSave && tag)
                            n.notificationDetailText += " and also added note " + noteMarker;
                        else if (pageSave && !tag)
                        {
                            n.notificationDetailText += " and also added note " + noteMarker;
                        }
                        else
                        {
                            n.notificationDetailText += " and added note "+noteMarker;

                        }
                        
                    }
                    
                }

                notifications.Add(n);
            }


            return notifications;

        }

        public void MarkNotificationAsRead(long id)
        {
            NotificationRepository repository = new NotificationRepository();
            repository.MarkAsRead(id);
        }

        public void MarkNotificationsAsRead(List<long> ids)
        {
            NotificationRepository repository = new NotificationRepository();
            foreach(var id in ids) repository.MarkAsRead(id);
        }

        public void MarkNotificationAsUnread(long id)
        {
            NotificationRepository repository = new NotificationRepository();
            repository.MarkAsUnread(id);
        }

        public void MarkNotificationsAsUnread(List<long> ids)
        {
            NotificationRepository repository = new NotificationRepository();
            foreach (var id in ids) repository.MarkAsUnread(id);
        }

        public void MarkAllUserNotificationsAsRead(long userID)
        {
            List<Notification> notifications = new NotificationRepository().GetUserNotifications(userID);
            MarkNotificationsAsRead((from n in notifications select n.ID).ToList());
        }
    }
}
