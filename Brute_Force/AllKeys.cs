using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Brute_Force
{
    public class AllKeys
    {
		/// <summary>
		/// 
		/// </summary>
        public List<Key> Keys { get; set; }

		/// <summary>
		/// 
		/// </summary>
        public AllKeys()
        {
            Keys = new List<Key>();
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		public void saveToJson(string fileName)
		{
			JsonSerializerOptions options = new JsonSerializerOptions
			{
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				WriteIndented = true
			};
			string value = JsonSerializer.Serialize(this, options);
			using StreamWriter streamWriter = new StreamWriter(fileName, append: false);
			streamWriter.WriteLine(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="hashValue"></param>
		/// <returns></returns>
		public string searchKeyByHash(string fileName, string hashValue)
		{
			string result = string.Empty;
			string json = string.Empty;

			using (StreamReader streamReader = new StreamReader(fileName))
			{
				json = streamReader.ReadToEnd();
			}

			Key key  = JsonSerializer.Deserialize<AllKeys>(json)!.Keys.Find(x => x.HashedKey == hashValue);		

			if (key != null)
			{
				result = key.PlainKey;
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="keyValue"></param>
		/// <returns></returns>
		public string searchHashByKey(string fileName, string keyValue)
		{
			string result = string.Empty;
			string json = string.Empty;

			using (StreamReader streamReader = new StreamReader(fileName))
			{
				json = streamReader.ReadToEnd();
			}

			Key key = JsonSerializer.Deserialize<AllKeys>(json)!.Keys.Find(x => x.PlainKey == keyValue);
			
			if (key != null)
			{
				result = key.HashedKey;
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileVolume"></param>
		/// <param name="fileName"></param>
		/// <param name="outputFile"></param>
		/// <returns></returns>
		public bool searchInFile(string fileVolume, string fileName, string outputFile)
		{
			try
			{
				string json = string.Empty;
				
				using (StreamReader streamReader = new StreamReader(fileName))
				{
					json = streamReader.ReadToEnd();
				}
				
				AllKeys allKeys = JsonSerializer.Deserialize<AllKeys>(json);
				
				using (TextFieldParser textFieldParser = new TextFieldParser(fileVolume))
				{
					textFieldParser.TextFieldType = FieldType.Delimited;
					textFieldParser.SetDelimiters(",");
					while (!textFieldParser.EndOfData)
					{
						string[] array = textFieldParser.ReadFields();
						Key item = new Key
						{
							Msisdn = array[1],
							Imei = array[2],
							HashedKey = array[3]
						};
						Keys.Add(item);
					}
				}
				
				foreach (Key k in Keys)
				{
					Key key = allKeys.Keys.Find(x => x.HashedKey == k.HashedKey);
					if (key != null)
					{
						k.PlainKey = key.PlainKey;
					}
				}
				
				JsonSerializerOptions options = new JsonSerializerOptions
				{
					Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
					WriteIndented = true
				};
				
				string outputJson = JsonSerializer.Serialize(this, options);
				
				using (StreamWriter streamWriter = new StreamWriter(outputFile, append: false))
				{
					streamWriter.WriteLine(outputJson);
				}
				
				return true;
			}
			catch (Exception ex)
			{
				ex.ToString();
				return false;
			}
		}
	}
}
