using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace WebApplication1.Utility
{

    //Symmetric Encryption/Decryption //use this one to enc/dec the querystring values
    //Same key to encryption and decrypt
    //Adv: Fastest, Simpler
    //it uses Secret Key and IV + salt
    //Disadv: you have to be very careful if you are going to use this one when sending files over an insecure network

    //Asymmetric Encryption/Decryption
    //It uses two different keys: Public Key and Private key
    //Public Key: encrypt
    //Private Key: decrypt (its always recommended never to send over an insecure network the private key)
    //Adv: we now can transfer the keys securely over an insecure network
    //Disadv: this is a slow performing algorithm, you cannot encrypt/decrypt large data. normally used to encrypt base64 data


    //Hybrid Encryption/Decryption //this is need to encrypt the files uploaded by the students & decrypt the files when they are needed by the teacher
    //we r going to take the adv of both the symmetric and the asymmetric to form the hybrid
    //we are going to send over the insecure network the encrypted data which was encrypted symmetrically and therefore we are going to send the
    //encrypted symmetric keys (which were encrypted with the public key)


    //Hashing
    //1 way encryption
    //usage: in passwords
    //sha512
    //two different values should give two different digests e.g. "hello world" and "helloworld" >> 2 different digests


    //Digital Signing/verify
    //against repudiation (when an attacker tampers with the data and he denies the activity)
    //suggestion for the assignment: you generate a private & public key for every student
    //when the user uploads the file, you take the private key and you sign the file and you store the signature with the file's data
    //when the user/teacher needs to download/view the file we need to use the file's owner public key + signature, to verify whether the data is still the same 
    //   i.e belonging to that user




    public class Encryption
    {

        /// <summary>
        /// This method takes a string and returns the digest as a string
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        public static string Hash(string clearText)
        {
            //convert the clearText into an array of bytes
            //clearTexxt is a non base64 data it has to be converted this way:
            byte[] clearTextAsBytes = Encoding.UTF32.GetBytes(clearText);
            byte[] digest = Hash(clearTextAsBytes);
            //base64 data convert to/from a string we have to use this class:
            string digestAsString = Convert.ToBase64String(digest);

            return digestAsString;
        }

        public static byte[] Hash(byte[] clearTextBytes)
        {
            SHA512 myAlg = SHA512.Create();
            byte[] digest = myAlg.ComputeHash(clearTextBytes);
            return digest;
        }


        static string password = "Pa$$w0rd";
        static byte[] salt = new byte[]
        {
            20, 1, 34,56,78,34,11,111,234,43,180,139,127,34,52,45,255,253,1
        };


        /// <summary>
        /// this method is used to take in clear data and encrypt it and returns back the cipher (the encrypted data)
        /// </summary>
        /// <param name="clearData"></param>
        /// <returns></returns>
        public static byte[] SymmetricEncrypt(byte[] clearData )
        {
            //Note:
            //1st thing is to think of how you are going to handle the keys
            //solution a) the key can be hardcoded
            //solution b) the key can be randomized/generated out of something the user's password
            //solution c) a password and salt which are hardcoded (which never change) out of which you generate "randomly" the key
            //in the assignment:
            // 1. to encrypt/decrypt query string values
            // 2. to encrypt/decrypt the file data as part of the hybrid encryption/decryption

            //password will be the source of origin of our secret key
            //the salt will add more security against an attacker guessing the password using dictionary attacks

            //how do we generate a secret key
            //note: each algorithm has a different sized secret key + iv 

            //0. declare the algorithm to use
            Rijndael myAlg = Rijndael.Create();
            //1. first we generate the secret key and iv
            var keys = GenerateKeys();

            //2. load the data into a MemoryStream
            MemoryStream msIn = new MemoryStream(clearData);
            msIn.Position = 0; //making sure that the pointer of the byte to read next is at the beginning so we encrypt everything

            //3. declare where to store the encrypted data
            MemoryStream msOut = new MemoryStream();

            //4. declaring a Stream which handles data encryption
            CryptoStream cs = new CryptoStream(msOut, //target stream where to write the data
                myAlg.CreateEncryptor(keys.SecretKey, keys.Iv), //the engine that operate the encrypting medium
                 CryptoStreamMode.Write //this will write the data fed into the medium
                    );

            //5. we start the encrypting engine
            msIn.CopyTo(cs);

            //6. make sure that the data is all written (flushed) into msOut
            cs.FlushFinalBlock();

            //7. 
            cs.Close();

            //8.
            return msOut.ToArray();

        }

        public static SymmetricKeys GenerateKeys()
        {
            // Password + Salt >>>>> Algorithm >>> Secret Key + IV (which is the input needed by the encryption algorithm)

            Rijndael myAlg = Rijndael.Create();

            Rfc2898DeriveBytes myGenerator = new Rfc2898DeriveBytes(password, salt);

            SymmetricKeys keys = new SymmetricKeys()
            { //1 byte =  8 bits
                SecretKey = myGenerator.GetBytes(myAlg.KeySize / 8),
                Iv = myGenerator.GetBytes(myAlg.BlockSize / 8)
            };

            return keys;
        }

        //clear bytes = original data (input by the user)
        //cipher = encrypted data
        public static byte[] SymmetricDecrypt(byte[] cipherAsBytes)
         {
            //0. declare the algorithm to use
            Rijndael myAlg = Rijndael.Create();
            //1. first we generate the secret key and iv
            var keys = GenerateKeys();

            //2. load the data into a MemoryStream
            MemoryStream msIn = new MemoryStream(cipherAsBytes);
            msIn.Position = 0; //making sure that the pointer of the byte to read next is at the beginning so we encrypt everything

            //3. declare where to store the clear data
            MemoryStream msOut = new MemoryStream();

            //4. declaring a Stream which handles data decryption
            CryptoStream cs = new CryptoStream(msOut, //target stream where to write the data
                myAlg.CreateDecryptor(keys.SecretKey, keys.Iv), //the engine that operate the encrypting medium
                 CryptoStreamMode.Write //this will write the data fed into the medium
                    );

            //5. we start the encrypting engine
            msIn.CopyTo(cs);

            //6. make sure that the data is all written (flushed) into msOut
            cs.FlushFinalBlock();

            //7. 
            cs.Close();

            //8.
            return msOut.ToArray();
        }

        public static string SymmetricEncrypt(string clearData)
         {
            //1. convert
            //   To convert any input (given by the user) we normally use Encoding.<character set>.GetBytes(...)

            byte[] clearDataAsBytes = Encoding.UTF32.GetBytes(clearData);

            //2. encrypting
            byte[] cipherAsBytes = SymmetricEncrypt(clearDataAsBytes);

            //3. converting back to a string
            // to convert from base64 bytes (which is the output of any cryptographic algorithm) we have to use Convert.ToBase64String...
            string cipher = Convert.ToBase64String(cipherAsBytes);


            //if used in querystrings remember to replace the / + = with any other characters which do not mean anything
            //in a querystring

            return cipher;
         }

        public static string SymmetricDecrypt(string cipher)
        {

            //remember to replace back any of the characters / + =

            //1. convert
            //   To convert any input (given by the user) we normally use Encoding.<character set>.GetBytes(...)

            byte[] cipherDataAsBytes = Convert.FromBase64String(cipher);

            //2. decryption
            byte[] clearDataAsBytes = SymmetricDecrypt(cipherDataAsBytes);

            //3. converting back to a string
            // to convert from base64 bytes (which is the output of any cryptographic algorithm) we have to use Convert.ToBase64String...
            string originalText = Encoding.UTF32.GetString(clearDataAsBytes);

            return originalText;
        }


        //Asymmetric Encryption/Decryption
        //public key = is used to encrypt
        //private key = is used to decrypt


        //my recommendation is this:
        //when a user is registered in addition to his/her details, you also generate this pair of keys (and store them in the db)


        public static AsymmetricKeys GenerateAsymmetricKeys()
        {
            //RSA and the DSA
            RSACryptoServiceProvider myAlg = new RSACryptoServiceProvider();

            AsymmetricKeys myKeys = new AsymmetricKeys()
            {
                PublicKey = myAlg.ToXmlString(false),
                PrivateKey = myAlg.ToXmlString(true)
            };

            return myKeys;
        }


        public static string AsymmetricEncrypt(string data, string publicKey)
        {
            RSACryptoServiceProvider myAlg = new RSACryptoServiceProvider();
            myAlg.FromXmlString(publicKey);


            byte[] dataAsBytes = Encoding.UTF32.GetBytes(data);

            byte[] cipher = myAlg.Encrypt(dataAsBytes, RSAEncryptionPadding.Pkcs1);

            return Convert.ToBase64String(cipher);
        }

        public static string AsymmetricDecrypt(string cipher, string privateKey)
        {
            RSACryptoServiceProvider myAlg = new RSACryptoServiceProvider();
            myAlg.FromXmlString(privateKey);

            byte[] cipherAsBytes = Convert.FromBase64String(cipher);

            byte[] originalTextAsBytes = myAlg.Decrypt(cipherAsBytes, RSAEncryptionPadding.Pkcs1);

            return Encoding.UTF32.GetString(originalTextAsBytes);
        }


        //clear = unencrypted
        public static MemoryStream HybridEncrypt(MemoryStream clearFile, string publicKey)
        {
            //preparation:
            //              you need to fetch from db the public pertaining to the uploading/logged in user

            //1. Generate the symmetric keys
            Rijndael myAlg = Rijndael.Create();
            myAlg.GenerateKey(); myAlg.GenerateIV();
            var key = myAlg.Key; var iv = myAlg.IV;

            //2. Encrypting the clearFile using Symmetric Encryption
            //var encryptedBytes =  SymmetricEncrypt(clearFileAsBytes, key, iv)

            //3. Asymmetrically encrypt using the public key, the symm key and iv above
            string keyAsString = Convert.ToBase64String(key);
            string encryptedKeyAsString = AsymmetricEncrypt(keyAsString, publicKey);


            //4. store the above encryted data n one file
            byte[] encrtypedKey= Convert.FromBase64String(encryptedKeyAsString) ;
           // byte[] encyptedIv;
            byte[] encryptedBytes; //this the uploaded file content

            MemoryStream msOut = new MemoryStream();
            msOut.Write(encrtypedKey, 0, encrtypedKey.Length);
            // msOut.Write(encyptedIv, 0, encyptedIv.Length);

            //encryptedBytes  [234alsdjfalsdkfj;alskdfjalsdkjfalskdjflaskdjflaskdjflasdjflaksjdflaksdjflaksd;jflaskdjaskldjflsdkfj]
          /*  MemoryStream encryptedfileContent = new MemoryStream(encryptedBytes);
            encryptedfileContent.Position = 0;
            encryptedfileContent.CopyTo(msOut);
          */
            return msOut;
        }

    }


    public class AsymmetricKeys
    {
        public string PublicKey { get; set; }//<<<<< in real life scenarios this is shared
        public string PrivateKey { get; set; } //<<<<<<< this is the one that should be really really kept safe
    }

    public class SymmetricKeys
    {
        public byte[] SecretKey { get; set; }
        public byte[] Iv { get; set; }
    }
}
