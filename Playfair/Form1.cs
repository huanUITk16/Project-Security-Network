using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Playfair
{
    public partial class Form1 : Form
    {
        
        public static string Encrypt(string input, string key)
        {
            return Cipher(input, key, true);
        }
        public static string Decrypt(string input, string key)
        {
            return Cipher(input, key, false);
        }
        private static string Cipher(string input, string key, bool encipher)
        {
            string retVal = string.Empty;
            char[,] keySquare = GenerateKeySquare(key);
            string tempInput = string.Empty;
            tempInput = RemoveOtherChars(input);//return just letters 
            int e;
            if (encipher)
            {
                e = 1;
            }
            else
            {
                e = -1;
            }
            for (int i = 0; i < tempInput.Length - 1; i = i + 2)
            {
                if (tempInput[i + 1] == tempInput[i])
                {
                    tempInput = tempInput.Substring(0, i + 1) + 'x' + tempInput.Substring(i + 1);
                }
            }

            if ((tempInput.Length % 2) != 0)
            {
                tempInput += 'x';
            }
            for (int i = 0; i < tempInput.Length; i += 2)
            {
                int row1 = 0;
                int col1 = 0;
                int row2 = 0;
                int col2 = 0;

                GetPosition(ref keySquare, char.ToUpper(tempInput[i]), ref row1, ref col1);
                GetPosition(ref keySquare, char.ToUpper(tempInput[i + 1]), ref row2, ref col2);


                if (row1 == row2)
                {
                    retVal += new string(SameRow(ref keySquare, row1, col1, col2, e));
                }
                else if (col1 == col2)
                {
                    retVal += new string(SameColumn(ref keySquare, col1, row1, row2, e));
                }
                else
                {
                    retVal += new string(DifferentRowColumn(ref keySquare, row1, col1, row2, col2));
                }
            }

            retVal = AdjustOutput(input, retVal);

            return retVal;
        }
        private static char[,] GenerateKeySquare(string key)
        {
            char[,] keySquare = new char[5, 5];
            string defaultKeySquare = "ABCDEFGHIKLMNOPQRSTUVWXYZ";
            string tempKey;
            if (string.IsNullOrEmpty(key))
            {
                tempKey = "CIPHER"; // để key mặc định là CIPHER
            }
            else
            {
                tempKey = key.ToUpper(); 
            }

            tempKey = tempKey.Replace("J", "I"); // thay thế ký tự J bằng I
            tempKey += defaultKeySquare;

            for (int i = 0; i < 25; ++i)
            {
                List<int> indexes = FindAllOccurrences(tempKey, defaultKeySquare[i]);
                tempKey = RemoveAllDuplicates(tempKey, indexes);
            }

            tempKey = tempKey.Substring(0, 25);

            for (int i = 0; i < 25; ++i)
                keySquare[(i / 5), (i % 5)] = tempKey[i];

            return keySquare;
        }

        private static List<int> FindAllOccurrences(string tempKey, char defaultKeySquare)

        {
            List<int> indexes = new List<int>();

            int index = 0;

            while ((index = tempKey.IndexOf(defaultKeySquare, index)) != -1) // searching in tempkey 
                indexes.Add(index++); //increment index to start counting from this place to find if the letter is duplicated
            return indexes;
        }

        private static string RemoveAllDuplicates(string tempKey, List<int> indexes)
        {
            string retVal = tempKey;

            for (int i = indexes.Count - 1; i >= 1; i--)
                retVal = retVal.Remove(indexes[i], 1);
            return retVal;
        }
        private static string RemoveOtherChars(string input)
        {
            string output = input;

            for (int i = 0; i < output.Length; ++i)
                if (!char.IsLetter(output[i]))
                    output = output.Remove(i, 1);//remove from index i just onr time 
            return output;
        }

        private static void GetPosition(ref char[,] keySquare, char ch, ref int row, ref int col)
        {
            if (ch == 'J')
                GetPosition(ref keySquare, 'I', ref row, ref col); // thay thế ký tự "J" bằng "I"

            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 5; ++j)
                    if (keySquare[i, j] == ch)
                    {
                        row = i;
                        col = j;
                        break;
                    }
        }
        private static int Mod(int a, int b)
        {
            return (a % b + b) % b;
        }
        private static char[] SameRow(ref char[,] keySquare, int row, int col1, int col2, int encipher)
        {
            return new char[] { keySquare[row, Mod((col1 + encipher), 5)], keySquare[row, Mod((col2 + encipher), 5)] };
        }
        private static char[] SameColumn(ref char[,] keySquare, int col, int row1, int row2, int encipher)
        {

            return new char[] { keySquare[Mod((row1 + encipher), 5), col], keySquare[Mod((row2 + encipher), 5), col] };
        }
        private static char[] DifferentRowColumn(ref char[,] keySquare, int row1, int col1, int row2, int col2)
        {
            return new char[] { keySquare[row1, col2], keySquare[row2, col1] };
        }
        private static string AdjustOutput(string input, string output)
        {
            StringBuilder retVal = new StringBuilder(output);

            for (int i = 0; i < input.Length; i++)
            {

                if (!char.IsLetter(input[i]))
                    retVal = retVal.Insert(i, input[i].ToString());

                if (char.IsLower(input[i]))
                    retVal[i] = char.ToLower(retVal[i]);
            }

            return retVal.ToString();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void txt_result_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_encrypt_Click(object sender, EventArgs e)
        {
            string key = txt_key.Text;
            string plainText = txt_plaintext.Text;
            txt_result.Text = Encrypt(plainText, key);


        }

        private void btn_decrypt_Click(object sender, EventArgs e)
        {
            string plainText = txt_plaintext.Text;
            string key = txt_key.Text;
            txt_result.Text = Decrypt(plainText, key);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txt_result.Text = "";
        }
    }
}
