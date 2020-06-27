using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Project4
{
    public partial class Form1 : Form
    {
        string[] vulgarismDictionary;
        public Form1()
        {
            InitializeComponent();
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            ReadFromFile();

        }

        private void ConvertBtn_Click(object sender, EventArgs e)
        {
            byteTextBox.Text = StringToBinary(inputTextBox.Text);
            string censoredText = FilterVulgarism(inputTextBox.Text);
            inputTextBox.Text = censoredText;
        }

        private static string reverse(string bin)
        {
            char[] reverse = bin.ToCharArray();
            Array.Reverse(reverse);
            return new string(reverse);

        }

        private static string appendBits(string binReversed)
        {

            return "0" + binReversed + "11";
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            inputTextBox.Clear();
            byteTextBox.Clear();
            outputTextBox.Clear();
        }

        public static string StringToBinary(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in ASCIIEncoding.UTF8.GetBytes(input))
            {
                string x = reverse(Convert.ToString(b, 2).PadLeft(8, '0'));
                sb.Append(appendBits(x));
            }

            return sb.ToString();
        }


        private static string deleteBits(string binReversed, int index)
        {
            return binReversed.Substring(index + 1, 8);
        }

        public static string BinaryToString(string data)
        {
            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < data.Length; i += 11)
            {
                string word;
                word = reverse(deleteBits(data, i));

                byteList.Add(Convert.ToByte(word, 2));

            }

            return Encoding.ASCII.GetString(byteList.ToArray());
        }

        private void binaryBtn_Click(object sender, EventArgs e)
        {
            string decodedText = BinaryToString(byteTextBox.Text);
            string censoredText = FilterVulgarism(decodedText);
            outputTextBox.Text = censoredText;
        }

        private void ReadFromFile()
        {
            var listOfVulgarism = new List<string>();
            var fileStream = new FileStream("wulgaryzmy.txt", FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    listOfVulgarism.Add(line);
                }
            }

            vulgarismDictionary = listOfVulgarism.ToArray();
            Array.Sort(vulgarismDictionary);
        }

        private string FilterVulgarism(string data)
        {
            const char dash = '-';
            const char apostrophe = '`';
            List<char> separators = new List<char>();

            for (char c = (char)0; c < dash; c++)
            {
                separators.Add(c);
            }
            for (char d = (char)(dash + 1); d < apostrophe; d++)
            {
                separators.Add(d);
            }
            for (char e = (char)('z' + 1); e <= 255; e++)
            {
                separators.Add(e);
            }


            WordIndex[] words = data.ToLower() 
                        .Split(separators.ToArray()) 
                        .Where(w => !string.IsNullOrEmpty(w)) 
                        .Select(w => new WordIndex(w, 0)) 
                        .ToArray(); 

            int position = 0; 
            for (int i = 0; i < words.Length; i++)
            {

                words[i].index = data.IndexOf(words[i].word, position, StringComparison.CurrentCultureIgnoreCase); 
                position = words[i].index + words[i].word.Length + 1;
            }

            Array.Sort(words, (a, b) => String.Compare(a.word, b.word));

            StringBuilder censoredData = new StringBuilder(data);

            for (int wordIndex = 0, vulgarismIndex = 0; wordIndex < words.Length && vulgarismIndex < vulgarismDictionary.Length ; )
            {
                if (words[wordIndex].word.CompareTo(vulgarismDictionary[vulgarismIndex]) < 0)
                {
                    wordIndex++;
                } else if (words[wordIndex].word.CompareTo(vulgarismDictionary[vulgarismIndex]) > 0)
                {
                    vulgarismIndex++;
                } else
                {
                    for (int i = words[wordIndex].index; i < words[wordIndex].index + words[wordIndex].word.Length; i++)
                        censoredData[i] = '*';
                    wordIndex++;
                    
                }
            }
            return censoredData.ToString();
        }
    }
}











