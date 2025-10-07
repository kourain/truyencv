using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public static class Roles
{
	[Display(Name ="Admin")] public const string Admin = "Admin";
    [Display(Name ="Moderator")] public const string Moderator = "Moderator";
	[Display(Name ="User")] public const string User = "User";
}