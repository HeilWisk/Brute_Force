using System.IO;
using System.Text.Json;

namespace Brute_Force
{
    public class Key
    {
		public string PlainKey { get; set; }

		public string HashedKey { get; set; }

		public string Msisdn { get; set; }

		public string Imei { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		public void saveToJson(string fileName)
		{
			string value = JsonSerializer.Serialize(this);
			using StreamWriter streamWriter = new StreamWriter(fileName, append: true);
			streamWriter.WriteLine(value);
		}
	}
}
