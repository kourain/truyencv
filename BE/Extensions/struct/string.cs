using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

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
    public static string ToSlug(this string source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return string.Empty;

        source = Regex.Replace(source, @"á|à|ả|ạ|ã|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ", "a",RegexOptions.IgnoreCase);
        source = Regex.Replace(source, @"é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ", "e",RegexOptions.IgnoreCase);
        source = Regex.Replace(source, @"i|í|ì|ỉ|ĩ|ị", "i",RegexOptions.IgnoreCase);
        source = Regex.Replace(source, @"ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ", "o",RegexOptions.IgnoreCase);
        source = Regex.Replace(source, @"ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự", "u",RegexOptions.IgnoreCase);
        source = Regex.Replace(source, @"ý|ỳ|ỷ|ỹ|ỵ", "y",RegexOptions.IgnoreCase);
        source = Regex.Replace(source, @"đ", "d", RegexOptions.IgnoreCase);
        //Xóa các ký tự đặt biệt
        StringBuilder sb = new StringBuilder();
        foreach (char c in source.ToLower())
        {
            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '-' || c == ' ')
            {
                sb.Append(c);
            }
        }
        source = sb.ToString();
        //Đổi khoảng trắng thành ký tự gạch ngang
        source = Regex.Replace(source, @" ", "-",RegexOptions.IgnoreCase);
        //Đổi nhiều ký tự gạch ngang liên tiếp thành 1 ký tự gạch ngang
        source = Regex.Replace(source, @"\-+", "-",RegexOptions.IgnoreCase);
        var slug = source.Trim('-');
        return slug.Length > 100 ? slug.Substring(0, 100) : slug;
    }
}