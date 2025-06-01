using System;
using System.IO;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: CryptoSoft.exe <filePath> <key>");
            return;
        }

        string filePath = args[0];
        string key = args[1];

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Fichier non trouvé : " + filePath);
            return;
        }

        try
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] result = new byte[fileBytes.Length];

            for (int i = 0; i < fileBytes.Length; i++)
            {
                result[i] = (byte)(fileBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            File.WriteAllBytes(filePath, result);
            Console.WriteLine("Chiffrement terminé !");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur : {ex.Message}");
        }
    }
}
