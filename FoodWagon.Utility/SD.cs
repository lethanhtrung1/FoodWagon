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

        // ...
        public static string TokenCookie = "JwtToken";

        // Order Status
        public const string OrderPending = "Pending";
        public const string OrderApproved = "Approved";
        public const string OrderCancelled = "Cancelled";
		public const string OrderShipped = "Shipped";

        // Payment Status
        public const string PaymentPending = "Pending";
        public const string PaymentApproved = "Approved";
        public const string PaymentRejected = "Rejected";
	}
}
