using System.ComponentModel.DataAnnotations;
using System.Text;

public static partial class Extensions
{
	public static string ToBase64(this string source)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
	}
	public static string FromBase64(this string base64)
	{
		return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
	}
	public static string GetDisplayName(this Enum value)
	{
		var field = value.GetType().GetField(value.ToString());
		var attribute = Attribute.GetCustomAttribute(field!, typeof(DisplayAttribute)) as DisplayAttribute;
		return attribute == null ? value.ToString() : attribute.Name!;
	}
}