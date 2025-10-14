using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TruyenCV;

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

	public static long ToSnowflakeId(this string? value, string fieldName = "ID")
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentException($"{fieldName} không được để trống", fieldName);
		}

		if (!long.TryParse(value, out var parsed))
		{
			throw new ArgumentException($"{fieldName} không hợp lệ", fieldName);
		}

		return parsed;
	}

	public static long? ToNullableSnowflakeId(this string? value, string fieldName)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}

		if (!long.TryParse(value, out var parsed))
		{
			throw new ArgumentException($"{fieldName} không hợp lệ", fieldName);
		}

		return parsed;
	}
}