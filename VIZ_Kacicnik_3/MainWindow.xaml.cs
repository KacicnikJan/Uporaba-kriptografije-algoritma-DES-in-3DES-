using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace VIZ_Kacicnik_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte[] nesif_dat;
        private byte[] sif_dat;
        private string tip;
        private string kljuc;
        private bool imamo_kljuc = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        private string generiraj_kljuc()
        {

            
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();
            imamo_kljuc = true; 
            return ASCIIEncoding.ASCII.GetString(desCrypto.Key);
        }

        private string generiraj_kljuc_DES3()
        {            
            TripleDESCryptoServiceProvider des3 = new TripleDESCryptoServiceProvider();
            imamo_kljuc = true;             
            return ASCIIEncoding.ASCII.GetString(des3.Key);
        }

        private void DES_sifriraj()
        {
            if (imamo_kljuc == true)
            {
                DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
                DES.Key = ASCIIEncoding.ASCII.GetBytes(kljuc);
                DES.IV = ASCIIEncoding.ASCII.GetBytes(kljuc);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (
                        CryptoStream cryptoStream = new CryptoStream(memoryStream, DES.CreateEncryptor(), CryptoStreamMode.Write)
                        )
                    {
                        cryptoStream.Write(nesif_dat, 0, nesif_dat.Length);
                        cryptoStream.Close();
                        sif_dat = new byte[memoryStream.ToArray().Length];
                        sif_dat = memoryStream.ToArray();
                    }
                }
            }
            else
            {
                MessageBox.Show("Izberite kljuc ki ga zelite uporabljati.");
            }
        }

        private void DES_desifriraj()
        {
            if (imamo_kljuc == true)
            {
                DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
                DES.Key = ASCIIEncoding.ASCII.GetBytes(kljuc);
                DES.IV = ASCIIEncoding.ASCII.GetBytes(kljuc);

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, DES.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(sif_dat, 0, sif_dat.Length);
                            cs.Close();
                            nesif_dat = new byte[ms.ToArray().Length];
                            nesif_dat = ms.ToArray();
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Kljuc ni pravilen.");
                }
            }
            else
            {
                MessageBox.Show("Izberite kljuc ki ga zelite uporabljati.");
            }

        }

        private void DES3_sifriranje()
        {
            if (imamo_kljuc == true)
            {
                TripleDESCryptoServiceProvider DES3 = new TripleDESCryptoServiceProvider();

                byte[] IV_array;

                DES3.Key = ASCIIEncoding.ASCII.GetBytes(kljuc);

                IV_array = ASCIIEncoding.ASCII.GetBytes(kljuc);

                DES3.IV = IV_array.Take(8).ToArray();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (
                        CryptoStream cryptoStream = new CryptoStream(memoryStream, DES3.CreateEncryptor(), CryptoStreamMode.Write)
                        )
                    {
                        cryptoStream.Write(nesif_dat, 0, nesif_dat.Length);
                        cryptoStream.Close();
                        sif_dat = new byte[memoryStream.ToArray().Length];
                        sif_dat = memoryStream.ToArray();
                    }
                }
            }
            else
            {
                MessageBox.Show("Izberite kljuc ki ga zelite uporabljati.");
            }
        }

        private void DES3_desifriranje()
        {
            if (imamo_kljuc == true)
            {
                TripleDESCryptoServiceProvider DES3 = new TripleDESCryptoServiceProvider();

                byte[] IV_array;

                DES3.Key = ASCIIEncoding.ASCII.GetBytes(kljuc);

                IV_array = ASCIIEncoding.ASCII.GetBytes(kljuc);

                DES3.IV = IV_array.Take(8).ToArray();
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (
                            CryptoStream cryptoStream = new CryptoStream(memoryStream, DES3.CreateDecryptor(), CryptoStreamMode.Write)
                            )
                        {
                            cryptoStream.Write(sif_dat, 0, sif_dat.Length);
                            cryptoStream.Close();
                            nesif_dat = new byte[memoryStream.ToArray().Length];
                            nesif_dat = memoryStream.ToArray();
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Kljuc ni pravilen.");
                }
            }
            else
            {
                MessageBox.Show("Izberite kljuc ki ga zelite uporabljati.");
            }
        }

        private void btnNalozi_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog datoteka = new OpenFileDialog();
            if (datoteka.ShowDialog() == true)
            {
                tip = System.IO.Path.GetExtension(datoteka.FileName);
                if (Nalaganje.SelectedItem.ToString().Contains("Nesifrirana"))
                {
                    string podatki = datoteka.FileName;
                    nesif_dat = new byte[Encoding.UTF8.GetBytes(podatki).Length];
                    nesif_dat = File.ReadAllBytes(datoteka.FileName);
                }
                else if (Nalaganje.SelectedItem.ToString().Contains("Sifrirana"))
                {
                    string podatki = datoteka.FileName;
                    sif_dat = new byte[Encoding.UTF8.GetBytes(podatki).Length];
                    sif_dat = File.ReadAllBytes(datoteka.FileName);
                }
            }
            Console.WriteLine(Nalaganje.SelectedItem.ToString());
        }

        private void btnShrani_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if (saveFileDialog1.ShowDialog() == true)
            {
                if (shranjevanje.SelectedItem.ToString().Contains("Nesifrirana"))
                {
                    if (nesif_dat == null)
                    {
                        MessageBox.Show("Nesifrirana datoteka ni nalozena.");
                    }
                    else
                    {
                        File.WriteAllBytes(saveFileDialog1.FileName + tip, nesif_dat);
                    }
                }
                else if (shranjevanje.SelectedItem.ToString().Contains("Sifrirana"))
                {
                    if (sif_dat == null)
                    {
                        MessageBox.Show("Sifrirana datoteka ni nalozena.");
                    }
                    else
                    {
                        File.WriteAllBytes(saveFileDialog1.FileName + tip, sif_dat);
                    }
                }
            }
        }

        private void btnKljuc_Click(object sender, RoutedEventArgs e)
        {
            if (Kljuc_Combo.SelectedItem.ToString().Contains("Naloži"))
            {
                imamo_kljuc = true;
                OpenFileDialog datoteka = new OpenFileDialog();
                datoteka.Filter = "text |*.txt";

                if (datoteka.ShowDialog() == true)
                {
                    kljuc = File.ReadAllText(datoteka.FileName);
                    Console.WriteLine(kljuc);
                }
            }

            if (Kljuc_Combo.SelectedItem.ToString().Contains("Shrani"))
            {
                imamo_kljuc = true;
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "text |*.txt";
                saveFileDialog1.DefaultExt = "txt";
                saveFileDialog1.AddExtension = true;
                if (saveFileDialog1.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog1.FileName, kljuc);
                }
            }

            if (Kljuc_Combo.SelectedItem.ToString().Contains("Generiraj DES"))
            {
                kljuc = generiraj_kljuc();
            }
            if (Kljuc_Combo.SelectedItem.ToString().Contains("Generiraj DES3"))
            {
                kljuc = generiraj_kljuc_DES3();
            }

        }

        private void btnDESsif_Click(object sender, RoutedEventArgs e)
        {
            DES_sifriraj();
        }

        private void btnDESdesif_Click(object sender, RoutedEventArgs e)
        {
            DES_desifriraj();
        }

        private void btnDES3sif_Click(object sender, RoutedEventArgs e)
        {
            DES3_sifriranje();
        }

        private void btnDES3desif_Click(object sender, RoutedEventArgs e)
        {
            DES3_desifriranje();
        }
    }
}
