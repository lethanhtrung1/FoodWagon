using FoodWagon.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoodWagon.Models.ViewModels {
	public class RoleManagementVM {
		public ApplicationUser ApplicationUser { get; set; }
		public IEnumerable<SelectListItem> RoleList { get; set; }
	}
}
