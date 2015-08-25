using Model;
using Model.Extended.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace Notocol.Models
{
    public class Utility
    {
        public static void SetUserSession(long userID, string userName)
        {
            System.Web.HttpContext.Current.Session["userID"] = userID;
            System.Web.HttpContext.Current.Session["userName"] = userName;
        }
        public static void ResetUserSession()
        {
            System.Web.HttpContext.Current.Session["userID"] = null;
            System.Web.HttpContext.Current.Session["userName"] = null;
        }
        public static long GetCurrentUserID()
        {
            
            return Convert.ToInt64(System.Web.HttpContext.Current.Session["userID"]);
        }

        public static string GetCurrentUserName()
        {
            return Convert.ToString(System.Web.HttpContext.Current.Session["userName"]);
        }

        public static Model.User ExtensionUserToUser(ExtensionUser user)
        {
            Model.User userDB = new Model.User();
            userDB.ID = user.ID;
            userDB.Username = user.Username;
            userDB.Email = user.email;
            userDB.Password = user.Password;


            return userDB;
        }

        public static ExtensionUser UserToExtensionUser(Model.User user)
        {
            ExtensionUser extUser = new ExtensionUser();
            extUser.ID = user.ID;
            extUser.Username = user.Username;
            extUser.Password = user.Password;
            extUser.email = user.Email;
            return extUser;
        }

        

        public static string GenerateUserInfoCookieData(long userID, string userName)
        {
            string data = userID.ToString() + ";" + userName;
            string strKey = "wW4I3G2Tn15Irn1b586m57GX27H72vg0";
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto =
                    new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                string strTempKey = strKey;
                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = ASCIIEncoding.ASCII.GetBytes(data);
                return HttpUtility.UrlEncode(Convert.ToBase64String(objDESCrypto.CreateEncryptor().
                    TransformFinalBlock(byteBuff, 0, byteBuff.Length)));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool GetUserInfoFromCookieData(string cookieData, out long userID, out string userName){
            string[] decryptedDataValues = {};
            string strKey = "wW4I3G2Tn15Irn1b586m57GX27H72vg0";
            userID = 0;
            userName = null;
            try
            {
                
                TripleDESCryptoServiceProvider objDESCrypto =
                    new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                string strTempKey = strKey;
                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = Convert.FromBase64String(cookieData);
                string strDecrypted = ASCIIEncoding.ASCII.GetString
                (objDESCrypto.CreateDecryptor().TransformFinalBlock
                (byteBuff, 0, byteBuff.Length));
                objDESCrypto = null;
                decryptedDataValues = strDecrypted.Split(';');
                userID = Convert.ToInt64(decryptedDataValues[0]);
                userName = decryptedDataValues[1];
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
            return false;
        }

        public static void AddCookie(string name, string value)
        {
            //var resp = new HttpResponseMessage();

            var cookie = new HttpCookie(name, value);
            cookie.Expires = DateTime.Now.AddDays(30);
            cookie.Domain = null;
            cookie.Path = "/";

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void RemoveCookie(string name)
        {
            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                HttpCookie myCookie = new HttpCookie(name);
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
        }
    }
}