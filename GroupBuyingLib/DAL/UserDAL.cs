﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupBuyingLib.Model;

using System.Web;
using System.Data.OleDb;
using System.Data;
using GroupBuyingLib.BL.Commands;

namespace GroupBuyingLib.DAL
{
    public class UserDAL
    {
        /// <summary>
        /// Convert row to user object
        /// </summary>
        public static User FromRow(DataRow row) {
            User returnUser = null;   // Return value

            returnUser = new User((string)row["UserName"],
                    (string)row["Password"]);
            returnUser.Email = row["Email"].ToString();
            returnUser.Profile = (string)row["Profile"];
            returnUser.Authorized = (bool)row["Authorized"];

            return returnUser;
        }

        /// <summary>
        /// Get user details by user name and password
        /// Used to validate user
        /// </summary>
        public User GetUserDetails(string username, string password)
        {
            User returnUser = null;   // Return value
            // Get data form
            DataTable Users = DataProvider.Instance.getTable("Users");

            // Get user from users
            EnumerableRowCollection<DataRow> query = from user in Users.AsEnumerable()
                                         where user.Field<String>("UserName") ==  username &&
                                                     user.Field<String>("Password") == password
                                         select user;
            // Get single
            DataRow first = query.SingleOrDefault<DataRow>();
            
            // If exists
            if (first != null)
                returnUser = FromRow(first);

            return returnUser;
        }

        /// <summary>
        /// Get user details by user name
        /// </summary>
        public User GetUserDetails(string username)
        {
            User returnUser = null;   // Return value
            // Get data form
            DataTable Users = DataProvider.Instance.getTable("Users");

            // Get user from users
            EnumerableRowCollection<DataRow> query = from user in Users.AsEnumerable()
                                                     where user.Field<String>("UserName") == username
                                                     select user;
            // Get single
            DataRow first = query.SingleOrDefault<DataRow>();

            // If exists
            if (first != null)
                returnUser = FromRow(first);

            return returnUser;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        public void RegisterUser(User user)
        {
            // Add user to db
            Object[] parameters = new Object[] {
                user.UserName, user.Password, user.Email, 
                user.Profile, user.Authorized
            };
            DataProvider.Instance.executeCommand("INSERT INTO Users" + 
                " VALUES (@p0, @p1, @p2, @p3, @p4)", parameters);

        }
    }
}
