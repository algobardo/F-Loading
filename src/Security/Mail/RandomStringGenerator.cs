using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security
{
    public static class RandomStringGenerator
    {

        //private static Random randomgen = new Random();

        //public static string GetRandomString(int size)
        //{

        //    StringBuilder builder = new StringBuilder();
        //    for (int i = 0; i < size; i++)
        //    {
        //        //Randomgen
        //        //26 letters in the alfabet, ascii + 65 for the capital letters
        //        builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * randomgen.NextDouble() + 65))));

        //    }
        //    return builder.ToString();

        //}

        //public static string GetRandomString(int numChars, int seed)
        //{
        //    string[] chars = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "P", "Q", "R", "S",
        //                "T", "U", "V", "W", "X", "Y", "Z", "2", "3", "4", "5", "6", "7", "8", "9","q","w","e","r","t","y",
        //                "u","i","o","p","a","s","d","f","g","h","j","k","l","z","x","c","v","b","n","m" };
            

        //    Random rnd = new Random(seed);
        //    string random = string.Empty;
        //    for (int i = 0; i < numChars; i++)
        //    {
        //        random += chars[rnd.Next(0, 33)];
        //    }
        //    return random;
        //}
        
        private static Random randomgen = new Random();

        public static string GetRandomString(int numChars)
        {
            string[] chars = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "P", "Q", "R", "S",
                        "T", "U", "V", "W", "X", "Y", "Z", "2", "3", "4", "5", "6", "7", "8", "9","q","w","e","r","t","y",
                        "u","i","o","p","a","s","d","f","g","h","j","k","l","z","x","c","v","b","n","m" };

            //int seed = 3233;
            //Random rnd = new Random(seed);
            string random = string.Empty;

            for (int i = 0; i < numChars; i++)
            {
                random += chars[randomgen.Next(0, chars.Count() - 1)];
            }
            return random;
        }
        
    }
}
