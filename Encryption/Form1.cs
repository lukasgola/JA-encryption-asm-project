using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Encryption
{
    public partial class Form1 : Form
    {


        [DllImport(@"C:\Users\atomo\Desktop\JA-encryption-asm-project\x64\Debug\JAAsm.dll")]
        public static extern int MyProc1(int value1, int value2); // Dostosuj typy parametrów i typ zwracany


        public Form1()
        {
            InitializeComponent();
        }



        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Pliki obrazów|*.jpg;*.jpeg;*.png;*.bmp"; // Filtr rozszerzeń plików obrazów
                openFileDialog.Title = "Wybierz obraz do załadowania";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Tutaj możesz wykonać działania na wybranym obrazie, np. załadować go do kontrolki PictureBox
                    string selectedImagePath = openFileDialog.FileName;
                    Bitmap originalImage = new Bitmap(selectedImagePath);
                    Bitmap resizedImage = new Bitmap(originalImage, pictureBox.Width, pictureBox.Height);

                    // Przypisanie przeskalowanego obrazu do PictureBox
                    pictureBox.Image = resizedImage;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            System.Drawing.Image image = pictureBox.Image;

            if (image != null)
            {
                // Konwersja obrazu na format Base64
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, ImageFormat.Jpeg); // Możesz wybrać odpowiedni format obrazu
                    byte[] imageBytes = memoryStream.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);

                    // Tutaj możesz wykorzystać ciąg base64String do dalszych działań, np. zapisu do pliku lub przesyłki przez sieć
                    // Możesz również wyświetlić ten ciąg w TextBox lub innym elemencie UI w aplikacji
                    Console.WriteLine("Obraz w formacie Base64: " + base64String);
                    textBox1.Text = base64String;
                }
            } else
            {
                MessageBox.Show("Nie wybrano obrazu.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Zakładając, że masz kontrolkę TextBox o nazwie textBox1

            // Pobranie tekstu z TextBox
            string textToCopy = textBox1.Text;

            // Kopiowanie tekstu do schowka
            Clipboard.SetText(textToCopy);

            // Komunikat potwierdzający skopiowanie tekstu
            MessageBox.Show("Tekst został skopiowany do schowka.");

        }

        // Funkcja do dekodowania ciągu Base64 na obraz
        private System.Drawing.Image ConvertBase64ToImage(string base64String)
        {
            int result = MyProc1(5, 10); // Przykładowe wywołanie funkcji z DLL z parametrami 5 i 10
            MessageBox.Show("Wynik: " + result);


            // Konwersja ciągu Base64 na tablicę bajtów
            byte[] imageBytes = Convert.FromBase64String(base64String);

            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
            {
                // Konwersja tablicy bajtów na obraz
                System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream);
                return image;
            }

            

        }


        private void button4_Click(object sender, EventArgs e)
        {

            if (textBox1.Text != "")
            {
                // Przykładowe użycie funkcji ConvertBase64ToImage
                string base64String = textBox1.Text; // Zastąp tym rzeczywistym ciągiem Base64
                System.Drawing.Image imageFromBase64 = ConvertBase64ToImage(base64String);

                // Teraz możesz użyć obiektu 'imageFromBase64', np. wyświetlić go w kontrolce PictureBox
                pictureBox.Image = imageFromBase64;
            } else
            {
                MessageBox.Show("Nie podano tekstu.");
            }
            

        }
    }
}
