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
        //Own user authentication Errors
        public const int AUTH_USER_NOT_FOUND = 0;
        public const int AUTH_USER_PASSWORD_INCORRECT = -1;
        public const int AUTH_USER_AUTHENTICATED = 1;

        //User addition errors
        public const int ADD_USER_NAME_EXISTS = -1;
        public const int ADD_USER_EMAIL_EXISTS = -2;
        public const  int ADD_USER_CLIENT_ID_EXISTS = -3;

        public UserRepository()
        {
            CreateDataContext();
        }
        public long AddUser(ref User user){
            user.ModifiedAt = DateTime.Now;
            User currentUser = user;
            //List<long> existingUsers = null;
            long returnValue = 0;
            try
            {
                using (GetDataContext())
                {
                    if (user.Identifier == null)
                    {
                        if ((from users in context.Users
                                              where users.Email == currentUser.Email
                                              select users.ID).ToList().Count != 0)

                            returnValue = ADD_USER_EMAIL_EXISTS;

                        if ((from users in context.Users
                                              where users.Username == currentUser.Username
                                              select users.ID).ToList().Count != 0)

                            returnValue = ADD_USER_NAME_EXISTS;

                        
                    }
                    else
                    {
                        if ((from users in context.Users
                                              where (users.Provider == currentUser.Provider &&      currentUser.Identifier == users.Identifier)
                                              select users.ID).ToList().Count != 0)

                            returnValue = ADD_USER_CLIENT_ID_EXISTS;
                                            
                    }

                    if (returnValue == 0)
                    {
                        context.Entry(user).State = EntityState.Added;
                        context.SaveChanges();
                        returnValue = user.ID;
                    }
                    
 
                                        
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
            return returnValue;
        }

        public int AuthenticateOwnUser(string username, string password, out User userDB)
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
                return AUTH_USER_NOT_FOUND;
            
            if(user.Password == password){
                userDB = user;
                return AUTH_USER_AUTHENTICATED;
            }

            return AUTH_USER_PASSWORD_INCORRECT;
                 
        }

        public bool DeleteUser(User user)
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

        public bool CheckIfUserNameExists(string userName)
        {

            User user;
            try
            {
                using (GetDataContext())
                {
                    user = (from userEntry in context.Users
                                  where userEntry.Username == userName
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
                return true;
            }
            return false;
        }

        public User GetOwnRegisteredUser(string userName, string password)
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

        public User GetExternalRegisteredUser(string provider, string userIdentifier)
        {
            User user;
            try
            {
                using (GetDataContext())
                {
                    user = (from userEntry in context.Users
                            where userEntry.Provider == provider && userEntry.Identifier == userIdentifier
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

        public User GetUser(long id)
        {
            User user = null;
            try
            {
                using (GetDataContext())
                {
                    user = (from users in context.Users
                            where users.ID == id
                            select users).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
            return user;
        }
        public User GetUser(string username)
        {
            User user = null;
            try
            {
                using (GetDataContext())
                {
                    user = (from users in context.Users
                            where users.Username == username
                            select users).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
            return user;
        }

        public List<long> GetAllUsersOfSolar()
        {
            List<long> users = null;
            try
            {
                using (GetDataContext())
                {
                    users = (from user in context.SolarGroupUsers
                             select user.userID).ToList();
                }
                
            }
            catch
            {
                throw;
            }
            return users;
        }

        public void AddToSolar(long userID)
        {
            SolarGroupUser su = new SolarGroupUser();
            su.userID = userID;
            try
            {
                using (GetDataContext())
                {
                    context.Entry(su).State = EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteFromSolar(long userID)
        {
            SolarGroupUser su = new SolarGroupUser();
            su.userID = userID;
            try
            {
                using (GetDataContext())
                {
                    context.Entry(su).State = EntityState.Deleted;
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
