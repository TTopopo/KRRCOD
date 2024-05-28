using Microsoft.AspNetCore.Identity;

namespace Construction.Areas.Identity.Data
{
    public class ConstructionUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

}
}
