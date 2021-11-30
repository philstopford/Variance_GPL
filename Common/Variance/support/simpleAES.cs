using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Variance;

public class SimpleAES
{
    // Change these keys
    private readonly byte[] Key;// = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
    private readonly byte[] Vector;// = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 252, 112, 79, 32, 114, 156 };

    private ICryptoTransform EncryptorTransform, DecryptorTransform;
    private UTF8Encoding UTFEncoder;

    public SimpleAES()
    {
        Key = new byte[] {
            32, 161, 218,  27, 238, 106, 241, 131,  89,  76, 230, 247, 200, 133, 154, 136,
            189,  69, 240, 250, 171, 103,  69, 190, 141,  44,  11,  90, 118, 170, 210, 237
        }; // 32
        Vector = new byte[] {
            201, 149, 239, 100, 109, 190, 178, 165, 225, 145, 132,  65, 122, 128,  40, 254
        }; // 16
        init();
    }

    public SimpleAES(byte[] key, byte[] vector)
    {
        Key = key;
        Vector = vector;
        init();
    }

    private void init()
    {
        //This is our encryption method
        // RijndaelManaged rm = new RijndaelManaged();
        Aes rm = Aes.Create();

        //Create an encryptor and a decryptor using our encryption method, key, and vector.
        EncryptorTransform = rm.CreateEncryptor(Key, Vector);
        DecryptorTransform = rm.CreateDecryptor(Key, Vector);

        //Used to translate bytes to text and vice versa
        UTFEncoder = new UTF8Encoding();
    }

    /// -------------- Two Utility Methods (not used but may be useful) -----------
    /// Generates an encryption key.
    public static byte[] GenerateEncryptionKey()
    {
        //Generate a Key.
        // RijndaelManaged rm = new RijndaelManaged();
        Aes rm = Aes.Create();
        rm.GenerateKey();
        return rm.Key;
    }

    /// Generates a unique encryption vector
    public static byte[] GenerateEncryptionVector()
    {
        //Generate a Vector
        // RijndaelManaged rm = new RijndaelManaged();
        Aes rm = Aes.Create();
        rm.GenerateIV();
        return rm.IV;
    }

    /// ----------- The commonly used methods ------------------------------    
    /// Encrypt some text and return a string suitable for passing in a URL.
    public string EncryptToString(string TextValue)
    {
        return ByteArrToString(Encrypt(TextValue));
    }

    /// Encrypt some text and return an encrypted byte array.
    public byte[] Encrypt(string TextValue)
    {
        //Translates our text value into a byte array.
        byte[] bytes = UTFEncoder.GetBytes(TextValue);

        //Used to stream the data in and out of the CryptoStream.
        MemoryStream memoryStream = new();

        /*
         * We will have to write the unencrypted bytes to the stream,
         * then read the encrypted result back from the stream.
         */
        #region Write the decrypted value to the encryption stream
        CryptoStream cs = new(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
        cs.Write(bytes, 0, bytes.Length);
        cs.FlushFinalBlock();
        #endregion

        #region Read encrypted value back out of the stream
        memoryStream.Position = 0;
        byte[] encrypted = new byte[memoryStream.Length];
        memoryStream.Read(encrypted, 0, encrypted.Length);
        #endregion

        //Clean up.
        cs.Close();
        memoryStream.Close();

        return encrypted;
    }

    /// The other side: Decryption methods
    public string DecryptString(string EncryptedString)
    {
        return Decrypt(StrToByteArray(EncryptedString));
    }

    /// Decryption when working with byte arrays.    
    public string Decrypt(byte[] EncryptedValue)
    {
        #region Write the encrypted value to the decryption stream
        MemoryStream encryptedStream = new();
        CryptoStream decryptStream = new(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
        decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
        decryptStream.FlushFinalBlock();
        #endregion

        #region Read the decrypted value from the stream.
        encryptedStream.Position = 0;
        byte[] decryptedBytes = new byte[encryptedStream.Length];
        encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
        encryptedStream.Close();
        #endregion
        return UTFEncoder.GetString(decryptedBytes);
    }

    /// Convert a string to a byte array.  NOTE: Normally we'd create a Byte Array from a string using an ASCII encoding (like so).
    //      System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
    //      return encoding.GetBytes(str);
    // However, this results in character values that cannot be passed in a URL.  So, instead, I just
    // lay out all of the byte values in a long string of numbers (three per - must pad numbers less than 100).
    public byte[] StrToByteArray(string str)
    {
        if (str.Length == 0)
        {
            throw new Exception("Invalid string value in StrToByteArray");
        }

        byte[] byteArr = new byte[str.Length / 3];
        int i = 0;
        int j = 0;
        do
        {
            byte val = byte.Parse(str.Substring(i, 3));
            byteArr[j++] = val;
            i += 3;
        }
        while (i < str.Length);
        return byteArr;
    }

    // Same comment as above.  Normally the conversion would use an ASCII encoding in the other direction:
    //      System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
    //      return enc.GetString(byteArr);    
    public string ByteArrToString(byte[] byteArr)
    {
        string tempStr = "";
        for (int i = 0; i <= byteArr.GetUpperBound(0); i++)
        {
            byte val = byteArr[i];
            switch (val)
            {
                case < 10:
                    tempStr += "00" + val;
                    break;
                case < 100:
                    tempStr += "0" + val;
                    break;
                default:
                    tempStr += val.ToString();
                    break;
            }
        }
        return tempStr;
    }
}