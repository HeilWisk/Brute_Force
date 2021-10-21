using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Brute_Force
{
    class Program
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="characterSet"></param>
		/// <param name="definedLength"></param>
		/// <param name="fileName"></param>
		private static void generateAllKLength(string characterSet, int definedLength, string fileName)
		{
			int length = characterSet.Length;
			AllKeys allKeys = new AllKeys();
			GenerateAllKLengthRecursive(characterSet, string.Empty, length, definedLength, allKeys);
			allKeys.saveToJson(fileName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="characterSet"></param>
		/// <param name="finalKey"></param>
		/// <param name="characterSetLength"></param>
		/// <param name="definedLength"></param>
		/// <param name="keys"></param>
		private static void GenerateAllKLengthRecursive(string characterSet, string finalKey, int characterSetLength, int definedLength, AllKeys keys)
		{
			char[] array = new char[1];
			if (definedLength == 0)
			{
				Key key = new Key
				{
					PlainKey = finalKey,
					HashedKey = cryptAlphanumericKeySHA256(finalKey)
				};
				keys.Keys.Add(key);
				return;
			}
			for (int i = 0; i < characterSetLength; i++)
			{
				int num = Encoding.ASCII.GetBytes(characterSet[i].ToString())[0];
				array[0] = characterSet[i];
                string defKey;
                if (finalKey.Length > 0)
				{
					if (Encoding.ASCII.GetBytes(finalKey)[finalKey.Length - 1] != num)
					{
						defKey = finalKey + array[0];
						GenerateAllKLengthRecursive(characterSet, defKey, characterSetLength, definedLength - 1, keys);
					}
				}
				else
				{
					defKey = finalKey + array[0];
					GenerateAllKLengthRecursive(characterSet, defKey, characterSetLength, definedLength - 1, keys);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private static string cryptAlphanumericKeySHA256(string key)
		{
			try
			{
				SHA256Managed hash = new SHA256Managed();
				byte[] plainTextBytes = Encoding.UTF8.GetBytes(key);
				return Convert.ToBase64String(hash.ComputeHash(plainTextBytes));
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
        {
            bool showMenu = true;
            while (showMenu)
            {
				showMenu = MainMenu();
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private static bool MainMenu()
		{
			Console.Clear();
			Console.WriteLine("Choose an option:");
			Console.WriteLine("1. Generate All Permutations");
			Console.WriteLine("2. Find a Key by Hash");
			Console.WriteLine("3. Find a Hash by Key");
			Console.WriteLine("4. Generate Hash from Key");
			Console.WriteLine("5. Get Keys from File");
			Console.WriteLine("6. Exit");
			Console.WriteLine("\r\nSelect an option: ");
            string value;
            switch (Console.ReadLine())
			{
				case "1":
					string fileName = ConfigurationManager.AppSettings.Get("FilePath") + ConfigurationManager.AppSettings.Get("FileName");
					string characterSet = ConfigurationManager.AppSettings.Get("InitialDictionary");
					int definedLength = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DefinedLength"));
					generateAllKLength(characterSet, definedLength, fileName);
					Console.WriteLine("\r\nSuccesful Key Generation");
					Console.ReadLine();
					return true;
				case "2":
					Console.WriteLine("\r\nEnter Hash Value: ");
					value = Console.ReadLine();
					printKeyValueByHash(value);
					return true;
				case "3":
					Console.WriteLine("\r\nEnter Key Value: ");
					value = Console.ReadLine();
					printHashValueByKey(value);
					return true;
				case "4":
					Console.WriteLine("\r\nEnter Key Value: ");
					value = Console.ReadLine();
					Console.WriteLine("\r\nThe hash value for the key is: " + cryptAlphanumericKeySHA256(value));
					Console.ReadLine();
					return true;
				case "5":
					Console.WriteLine("\r\nEnter file path and name: ");
					value = Console.ReadLine();
					searchInFile(value);
					return true;
				case "6":
					return false;
				default:
					return true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hashValue"></param>
		public static void printKeyValueByHash(string hashValue)
		{
			string fileName = ConfigurationManager.AppSettings.Get("FilePath") + ConfigurationManager.AppSettings.Get("FileName");
			string plainKey = new AllKeys().searchKeyByHash(fileName, hashValue);
			if (string.IsNullOrEmpty(plainKey))
			{
				Console.WriteLine("\r\nThere's no Key for the hash value");
			}
			else
			{
				Console.WriteLine("\r\nThe plain Key is: " + plainKey);
			}
			Console.ReadLine();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="plainKey"></param>
		public static void printHashValueByKey(string plainKey)
		{
			string fileName = ConfigurationManager.AppSettings.Get("FilePath") + ConfigurationManager.AppSettings.Get("FileName");
			string hashValue = new AllKeys().searchHashByKey(fileName, plainKey);
			if (string.IsNullOrEmpty(hashValue))
			{
				Console.WriteLine("\r\nThere's no hash for the key value");
			}
			else
			{
				Console.WriteLine("\r\nThe hash for the Key is: " + hashValue);
			}
			Console.ReadLine();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		public static void searchInFile(string file)
		{
			string fileName = ConfigurationManager.AppSettings.Get("FilePath") + ConfigurationManager.AppSettings.Get("FileName");
			string outputFile = ConfigurationManager.AppSettings.Get("FileOutputPath") + ConfigurationManager.AppSettings.Get("FileOutputName");
			if (new AllKeys().searchInFile(file, fileName, outputFile))
			{
				Console.WriteLine("\r\nSuccesful");
			}
			else
			{
				Console.WriteLine("\r\nFail");
			}
			Console.ReadLine();
		}		
	}
}
