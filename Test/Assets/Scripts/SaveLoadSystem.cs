using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
	public int GoldAmount; 

	public float airquality;
	public float soilHealth;
	public float EnvironmentalHealth;
}
public  class SaveLoadSystem 
{

	//public  SaveLoadSystem instance;
	//private void Awake()
	//{
	//	instance = this;
	//}


	public string Save(PlayerSaveData psd)
	{
		string serialized = JsonUtility.ToJson(psd);
		string encrypted = Utils.EncryptAES(serialized);
		Debug.Log("Encrypted: " + encrypted);
		return encrypted;
	}

	public PlayerSaveData Load(string encrypted) 
	{
		string stringData = Utils.DecryptAES(encrypted);
		
		PlayerSaveData derivedPSD = new PlayerSaveData();

		derivedPSD = JsonUtility.FromJson<PlayerSaveData>(stringData);
		Debug.Log("Deserialized: " + derivedPSD.GoldAmount);
		return derivedPSD;
	}

	//void Start()
	//{


	//	//WeaponInfo createdWeaponInfo = new WeaponInfo();
	//	//createdWeaponInfo.weaponID = "Dirty Knife";
	//	//createdWeaponInfo.durability = 5;

	//	//text.text = createdWeaponInfo.ToString();
	//	//Debug.Log("Weapon ID: " + createdWeaponInfo.weaponID);

	//	////////////////////////////////////////////////////////////////
	//	//// Let's first serialize and encrypt....
	//	////////////////////////////////////////////////////////////////

	//	//text.text = JsonUtility.ToJson(createdWeaponInfo);

	//	//string serialized = text.text;
	//	//Debug.Log("Serialized: " + serialized);

	//	//text.text = Utils.EncryptAES(text.text);

	//	//string encrypted = text.text;
	//	//Debug.Log("Encrypted: " + encrypted);


	//	////////////////////////////////////////////////////////////////
	//	//// Now let's de-serialize and de-encrypt....
	//	////////////////////////////////////////////////////////////////

	//	//string stringData = text.text;
	//	//stringData = Utils.DecryptAES(stringData);
	//	//Debug.Log("Decrypted: " + stringData);


	//	//WeaponInfo derivedWeaponInfo = new WeaponInfo();

	//	//derivedWeaponInfo = JsonUtility.FromJson<WeaponInfo>(stringData);

	//	//Debug.Log("Deserialized: " + derivedWeaponInfo.weaponID);

	//}
}

	public static class Utils
	{
		
		static byte[] ivBytes = new byte[16]; // Generate the iv randomly and send it along with the data, to later parse out
		static byte[] keyBytes = new byte[16]; // Generate the key using a deterministic algorithm rather than storing here as a variable

		static void GenerateIVBytes()
		{
			System.Random rnd = new System.Random();
			rnd.NextBytes(ivBytes);
		}

		const string nameOfGame = "Celestial Damp Gulley";
		static void GenerateKeyBytes()
		{
			int sum = 0;
			foreach (char curChar in nameOfGame)
				sum += curChar;

			System.Random rnd = new System.Random(sum);
			rnd.NextBytes(keyBytes);
		}

		public static string EncryptAES(string data)
		{
			GenerateIVBytes();
			GenerateKeyBytes();

			SymmetricAlgorithm algorithm = Aes.Create();
			ICryptoTransform transform = algorithm.CreateEncryptor(keyBytes, ivBytes);
			byte[] inputBuffer = Encoding.Unicode.GetBytes(data);
			byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

			string ivString = Encoding.Unicode.GetString(ivBytes);
			string encryptedString = Convert.ToBase64String(outputBuffer);

			return ivString + encryptedString;
		}

		public static string DecryptAES( string text)
		{
			GenerateIVBytes();
			GenerateKeyBytes();

			int endOfIVBytes = ivBytes.Length / 2;  // Half length because unicode characters are 64-bit width

			string ivString = text.Substring(0, endOfIVBytes);
			byte[] extractedivBytes = Encoding.Unicode.GetBytes(ivString);

			string encryptedString = text.Substring(endOfIVBytes);

			SymmetricAlgorithm algorithm = Aes.Create();
			ICryptoTransform transform = algorithm.CreateDecryptor(keyBytes, extractedivBytes);
			byte[] inputBuffer = Convert.FromBase64String(encryptedString);
			byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

			string decryptedString = Encoding.Unicode.GetString(outputBuffer);

			return decryptedString;
		}
	}

