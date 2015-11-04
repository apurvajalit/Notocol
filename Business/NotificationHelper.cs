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

        private void UpdateNotificationsAsync(SourceUser sourceUser, int reasonCode, List<string> tags = null)
        {
            if (reasonCode == 0) return;
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
                        ReasonCode = NOTIFICATION_REASON_PAGE_SAVE,
                        Created = DateTime.Now
                    };

                    notifications.Add(userID, notification);
                }
            }
            if ((reasonCode & NOTIFICATION_REASON_TAG) != 0)
            {
                tagFollowers = new TagHelper().GetUsersForTags(tags);
                if (oldUsers != null && oldUsers.Count > 0)
                {
                    tagFollowers = tagFollowers.Except(oldUsers).ToList();
                }
                foreach (var userID in tagFollowers)
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
                            Created = DateTime.Now
                        };

                        notifications.Add(userID, notification);
                        
                }
            }

            if((reasonCode & (NOTIFICATION_REASON_PAGE_SAVE | NOTIFICATION_REASON_NOTE)) != 0){
                foreach (var userID in followers)
                {
                    NotificationTemp notification = null;
                    notifications.TryGetValue(userID, out notification);
                    if (notification != null)
                    {
                        notification.ReasonCode |= (NOTIFICATION_REASON_PAGE_SAVE | NOTIFICATION_REASON_NOTE);
                        notifications[userID] = notification;
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
                            Created = DateTime.Now
                        };

                        notifications.Add(userID, notification);
                    }
                }
            }
            if (notifications.Count > 0)
            {
                objRepo.AddOrUpdateNotifications(notifications.Values.ToList());
            }
        }

        public void UpdateNotifications(SourceUser sourceUser, int reasonCode, List<string> tags = null)
        {
            if ((bool)sourceUser.Privacy) return;

            var task = Task.Factory.StartNew(() =>
                UpdateNotificationsAsync(sourceUser, reasonCode, tags));

            //task.OnComplete(myErrorHandler, TaskContinuationOptions.OnlyOnFaulted)
        }



        internal void DeleteNotificationForSourceUser(long sourceUserID)
        {
            Task.Factory.StartNew(() =>
            new NotificationRepository().DeleteNotificationsForSourceUser(sourceUserID));
        }

    }
}
