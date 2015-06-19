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
        public long addUser(User user){
            user.ModifiedAt = DateTime.Now;
            try
            {
                using (GetDataContext())
                {
                    List<long> existingUsers = (from users in context.Users
                                   where users.Email == user.Email || users.Username == user.Username
                                   select users.ID).ToList();

                    if (existingUsers.Count == 0)
                    {
                        context.Entry(user).State = EntityState.Added;
                        context.SaveChanges();

                    }
                    else return -1;
 
                                        
                }
            }
            catch 
            {
                return -1;
            }
            
            finally
            {
                DisposeContext();
            }
            return user.ID;
        }

        public long GetAuthorisedUser(string username, string password, string identifier, out User userDB)
        {

            User user;
            userDB = null;
            try
            {
                using (GetDataContext())
                {
                    user = (from userEntry in context.Users
                                  where userEntry.Username == username 
                                  select userEntry).ToList().FirstOrDefault<User>();
                }
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
                    if (user.Password != null && user.Password == password)
                    {
                        userDB = user;
                        return user.ID;
                    }
                    else return -1;
                }
                else if (identifier != null)
                {
                    if (user.Identifier != null && user.Identifier == identifier)
                    {
                        userDB = user;
                        return user.ID;
                    }
                    else
                        return -1;
                }
                else
                {
                    return -1;
                }
            }
            return 0;
 
        }

        public bool deleteUser(User user)
        {
            
            try
            {
                using (GetDataContext())
                {
                    context.Entry(user).State = EntityState.Deleted;
                    context.SaveChanges();
                   
                }
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

        public string getuserName(long userID)
        {

            User user;
            try
            {
                using (GetDataContext())
                {
                    user = (from userEntry in context.Users
                                  where userEntry.ID == userID
                                  select userEntry).ToList().FirstOrDefault<User>();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                DisposeContext();
            }
            if (user != null)
            {
                return user.Username;
            }
            return null;
        }

        public User GetExistingUser(string userName, string password)
        {
            User user;
            try
            {
                using (GetDataContext())
                {
                    user = (from userEntry in context.Users
                            where userEntry.Username == userName && userEntry.Password == password
                            select userEntry).ToList().FirstOrDefault<User>();
                }
            }
            
            catch
            {
                throw;
            }
            finally
            {
                DisposeContext();
            }

            return user;
            
            
 
        }

        public bool ChangePassword(User user, string newPassword)
        {

            user.Password = newPassword;
            try
            {
                using (GetDataContext())
                {
                    context.Entry(user).State = EntityState.Modified;
                    context.SaveChanges();

                }
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
