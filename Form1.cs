using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace btvn
{
    public partial class Form1 : Form
    {
        byte[] key1;
        byte[] key2;
        byte[] key3;
        byte[] iv;
        void createKey(int x)
        {
            if(x == 1)
            {
                using (DES des = DES.Create())
                {
                    key1 = des.Key;
                    iv = des.IV;
                }
            }
            else if (x == 2)
            {
                using (DES des = DES.Create())
                {
                    key2 = des.Key;
                    iv = des.IV;
                }
            }
            else
            {
                using (TripleDES tripleDes = TripleDES.Create())
                {
                    key3 = tripleDes.Key;
                    iv = tripleDes.IV;
                }
            }
        }
        public Form1()
        {
            InitializeComponent();
            createKey(1);
            createKey(3);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string msg = "";
            if (comboBox1.Text == "DES") msg = Des(textBox1.Text, key1);
            else if (comboBox1.Text == "2DES") { createKey(2); msg = Des(Des(textBox1.Text, key1), key2); }
            else if (comboBox1.Text == "3DES") msg = Des3();
            textBox2.Text = msg;
        }

        string Des3()
        {
            TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoServiceProvider.CreateEncryptor(key3, key3), CryptoStreamMode.Write);

            using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(textBox1.Text);
                streamWriter.Flush();
                cryptoStream.FlushFinalBlock();
                streamWriter.Flush();

                return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            }
        }

        string Des(string enc, byte[] key) 
        {
            DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,cryptoServiceProvider.CreateEncryptor(key, key),CryptoStreamMode.Write);

            using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(enc);
                streamWriter.Flush();
                cryptoStream.FlushFinalBlock();
                streamWriter.Flush();

                return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            }
        }

        string Decrypt(string enc, byte[] key) 
        {
            DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(enc));
            CryptoStream cryptoStream = new CryptoStream(memoryStream,cryptoServiceProvider.CreateDecryptor(key, key),CryptoStreamMode.Read);

            using (StreamReader streamReader = new StreamReader(cryptoStream))
            {
                return streamReader.ReadToEnd();
            }
        }

        string Decrypt3()
        {
            MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(textBox3.Text));
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, new TripleDESCryptoServiceProvider().CreateDecryptor(key3, key3),CryptoStreamMode.Read);


            byte[] fromEncrypt = new byte[Convert.FromBase64String(textBox3.Text).Length];


            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            return new ASCIIEncoding().GetString(fromEncrypt);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string msg = "";
            if (comboBox1.Text == "DES") msg = Decrypt(textBox3.Text, key1);
            else if (comboBox1.Text == "2DES") msg = Decrypt(Decrypt(textBox3.Text, key2), key1); 
            else if (comboBox1.Text == "3DES") msg = Decrypt3();

            textBox4.Text = msg;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
