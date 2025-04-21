using System.Security.Cryptography;
using System.Text;

namespace AdministrationDataBase.Helpers
{
	public class HashHelper
	{
		public static string GetSha256Hash(string rawData)
		{
			using SHA256 sha256Hash = SHA256.Create();
			byte[] byteArray = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

			return GetString(byteArray);
		}

        public static string GetMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        public static string GetString(byte[] byteArray)
		{
			StringBuilder stringBuilder = new();
			foreach (byte b in byteArray)
			{
				stringBuilder.Append(b.ToString("x2").ToLower());
			}
			return stringBuilder.ToString();
		}
	}
}