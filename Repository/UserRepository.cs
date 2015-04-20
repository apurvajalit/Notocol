using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity;

namespace Repository
{
    public class UserRepository:BaseRepository
    {
        public UserRepository()
        {
            CreateDataContext();
        }

        public long addUser(string username, string password, string identifier){
            User user = new User();
            long userID = 0;
            user.Username = username;
            user.Password = password;
            user.Identifier = identifier;
            user.ModifiedAt = DateTime.Now;
            try
            {
                using (GetDataContext())
                {
                    context.Entry(user).State = EntityState.Added ;
                    context.SaveChanges();
                    userID = user.ID;
                }
            }
            catch (Exception e)
            {
                return -1;
            }
            catch
            {
                throw;
            }
            finally
            {
                DisposeContext();
            }
            return userID;
        }

        public long checkUser(string username, string password, string identifier){

            User user;
            try
            {
                using (GetDataContext())
                {
                    user = (from userEntry in context.Users
                                  where userEntry.Username == username
                                  select userEntry).ToList().FirstOrDefault<User>();
                }
            }
            catch (Exception e)
            {

                throw;
            }
            catch
            {
                throw;
            }
            finally
            {
                DisposeContext();
            }
            if (user == null)
            {
                return 0;
            }else{
                if (password != null) { 
                 if(user.Password != null && user.Password == password)
                    return user.ID;
                }
                else if (identifier != null)
                {
                    if (user.Identifier != null && user.Identifier == identifier)
                        return user.ID;
                }
                else
                {
                    return -1;
                }
            }
            return 0;
 
        }

        public bool deleteUser(long ID)
        {
            User user = new User();
            user.ID = ID;
            try
            {
                using (GetDataContext())
                {
                    context.Entry(user).State = EntityState.Deleted;
                    context.SaveChanges();
                   
                }
            }
            catch (Exception e)
            {
                return false;
            }
            catch
            {
                throw;
            }
            finally
            {
                DisposeContext();
            }
            return true;

        }
    }
}
