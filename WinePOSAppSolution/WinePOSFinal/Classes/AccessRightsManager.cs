using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinePOSFinal.Classes
{
    public static class AccessRightsManager
    {
        // Static variable to hold the current user's role
        private static string currentUserRole;
        private static string currentUserName;

        // Method to set the current user's role
        public static void SetUserRole(string role)
        {
            currentUserRole = role;
        }

        // Method to get the current user's role
        public static string GetUserRole()
        {
            return currentUserRole;
        }
        
        // Method to set the current user's role
        public static void SetUserName(string role)
        {
            currentUserName = role;
        }

        // Method to get the current user's role
        public static string GetUserName()
        {
            return currentUserName;
        }

    }
}
