using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class NotificationRepository:BaseRepository
    {
        public NotificationRepository()
        {
            CreateDataContext();
        }
        public void AddNotification(Notification notification)
        {
            try
            {
                using (GetDataContext())
                {

                    context.Entry(notification).State = EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch
            {

            }
        }

        public Notification GetNotification(long receiverUserID, long sourceUserID)
        {
            Notification n = null;
            try
            {
                using (GetDataContext())
                {
                    n = (from notif in context.Notifications
                         where notif.Receiver == receiverUserID && notif.SourceUserID == sourceUserID
                         select notif).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }

            return n;
        }

        public void UpdateNotification(Notification notification)
        {
            try
            {
                using (GetDataContext())
                {

                    context.Entry(notification).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch
            {

            }
        }


        public void AddOrUpdateNotifications(List<NotificationTemp> notifications)
        {

            try{
                DataTable dt = ConvertToDataTable<NotificationTemp>(notifications);

                using (SqlConnection connection = new SqlConnection(GetDataContext().Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection))
                    {
                        sqlBulkCopy.ColumnMappings.Add("ReadStatus", "ReadStatus");
                        sqlBulkCopy.ColumnMappings.Add("Type", "Type");
                        sqlBulkCopy.ColumnMappings.Add("Receiver", "Receiver");
                        sqlBulkCopy.ColumnMappings.Add("SecondaryUser", "SecondaryUser");
                        sqlBulkCopy.ColumnMappings.Add("Created", "Created");
                        sqlBulkCopy.ColumnMappings.Add("SourceUserID", "SourceUserID");
                        sqlBulkCopy.ColumnMappings.Add("ReasonCode", "ReasonCode");
                        sqlBulkCopy.ColumnMappings.Add("SourceID", "SourceID");
                        sqlBulkCopy.DestinationTableName = "NotificationTemp";
                        sqlBulkCopy.WriteToServer(dt);
                    }

                    string mergeSql = "merge into Notification as Target " +
                                 "using NotificationTemp as Source " +
                                 "on " +
                                 "Target.Receiver=Source.Receiver " +
                                 "and Target.SourceUserID = Source.SourceUserID " +
                                 "when matched then " +
                                 "update set Target.ReasonCode= Target.ReasonCode | Source.ReasonCode " +
                                 "when not matched then " +
                                 "insert (ReadStatus,Type,Receiver, SecondaryUser,Created, SourceUserID, ReasonCode, SourceID) values (Source.ReadStatus, Source.Type,Source.Receiver, Source.SecondaryUser,Source.Created, Source.SourceUserID, Source.ReasonCode, Source.SourceID);";

                    SqlCommand cmd = new SqlCommand(mergeSql, connection);
                    cmd.ExecuteNonQuery();

                    //Clean up the temp table
                    cmd.CommandText = "delete from NotificationTemp";
                    cmd.ExecuteNonQuery();
                }   

            }catch{
                throw;
            }
            
            

            
            
        }

        public void DeleteNotificationsForSourceUser(long sourceUserID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetDataContext().Database.Connection.ConnectionString))
                {
                    string delStatement = "delete from Notification where SourceUserID=" + sourceUserID;
                    SqlCommand cmd = new SqlCommand(delStatement, connection);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
