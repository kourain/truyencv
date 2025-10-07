using System;

namespace TruyenCV.Helpers;

public static class Bcrypt
{
	// Mã hóa mật khẩu
	public static string HashPassword(string password, int costFactor = 12)
	{
		// Cost factor 12 = rất an toàn (2^12 = 4096 rounds)
		return BCrypt.Net.BCrypt.HashPassword(password, costFactor);
	}

	// Xác thực mật khẩu
	public static bool VerifyPassword(string password, string hashedPassword)
	{
		return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
	}
}
