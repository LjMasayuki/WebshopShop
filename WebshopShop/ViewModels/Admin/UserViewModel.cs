namespace WebshopShop.ViewModels.Admin
{
    public class UserListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = [];
        public int OrderCount { get; set; }
    }

    public class EditUserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<RoleToggleViewModel> Roles { get; set; } = [];
    }

    public class RoleToggleViewModel
    {
        public string RoleName { get; set; } = string.Empty;
        public bool IsAssigned { get; set; }
    }
}