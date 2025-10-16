using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TruyenCV;

public static partial class Extensions
{
	public static string ToString(this Enum value)
	{
		var field = value.GetType().GetField(value.ToString());
		var displayAttribute = Attribute.GetCustomAttribute(field!, typeof(DisplayAttribute)) as DisplayAttribute;
		var descriptionAttribute = Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute)) as DescriptionAttribute;
		return displayAttribute == null ? value.ToString() : displayAttribute.Name!;
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