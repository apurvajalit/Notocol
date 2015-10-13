using Model;
using Model.Extended;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class UserHelper
    {
        public const string USER_INFO_TOKEN = "TOKEN-INFO";

        static UserRepository userRepository = new UserRepository();


        public User GetExternalUser(string provider, string userIDentifier)
        {
            return userRepository.GetExternalRegisteredUser(provider, userIDentifier);
        }

        public long AddOwnNewUser(UserRegistration userRegistration, out User addedUser)
        {
            User user = new User();
            user.Username = userRegistration.username;
            user.Password = userRegistration.password;
            user.Email = userRegistration.email;

            user.Name = userRegistration.name;
            user.DOB = userRegistration.DOB;
            user.Gender = userRegistration.gender.ToString();
            user.Address = userRegistration.address;
            user.Photo = userRegistration.photo;

            user.ModifiedAt = DateTime.Now;

            long retVal = userRepository.AddUser(ref user);
            addedUser = user;
            return retVal;

        }

        public long AddExternalNewUser(ref User user){
            return userRepository.AddUser(ref user);
        }

        public long AuthenticateOwnUser(string username, string password, out User userDB)
        {
            User user = null;
            long ret = userRepository.AuthenticateOwnUser(username, password, out user);
            userDB = user;
            return ret;

        }

        public bool CheckIfUserNameExists(string username)
        {
            return userRepository.CheckIfUserNameExists(username);
        }



        public User GetUser(string username)
        {
            return userRepository.GetUser(username);

        }
    }
}
