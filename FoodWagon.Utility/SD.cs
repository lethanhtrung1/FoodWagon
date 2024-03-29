using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodWagon.Utility {
    public class SD {
        // Role
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
        public const string Role_Customer = "Customer";

        public static string TokenCookie = "JwtToken";

        // Order Status
        public static string Order_Pending = "Pending";
        public static string Order_Approved = "Approved";
        public static string Order_Cancelled = "Cancelled";
    }
}
