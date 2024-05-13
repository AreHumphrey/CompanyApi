using Microsoft.AspNetCore.Identity;

namespace CompanyApi.Models;

public class User : IdentityUser
{
    public Guid Id { get; set; } // This is redundant as IdentityUser already includes an Id
    public string Name { get; set; }
    public string Login { get; set; }
    public string PhoneNumber { get; set; }
    public Guid CompanyId { get; set; }
    public bool IsBoss { get; set; }
}