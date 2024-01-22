using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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


        [DllImport(@"C:\Users\atomo\Documents\GitHub\JA-encryption-asm-project\x64\Debug\JAAsm.dll")]
        public static extern void ASMAdjustBrightness(
            IntPtr bmpDataScan0,
            IntPtr bmpOriginal,
            int value,
            int imageSizeInBytes
        );

        [DllImport(@"C:\Users\atomo\Documents\GitHub\JA-encryption-asm-project\x64\Debug\JAAsm.dll")]
        public static extern int ImageToBase64(
            IntPtr textOriginal,
            IntPtr textOutPut,
            IntPtr base64,
            int textSizeInBytes
        );


        public Bitmap mainImage;

        public string base64String = "";
        public string base64Asm = "";

        public Form1()
        {
            InitializeComponent();
        }



        private void LoadImage(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Pliki obrazów|*.jpg;*.jpeg;*.png;*.bmp"; // Filtr rozszerzeń plików obrazów
                openFileDialog.Title = "Wybierz obraz do załadowania";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Tutaj możesz wykonać działania na wybranym obrazie, np. załadować go do kontrolki PictureBox
                    string selectedImagePath = openFileDialog.FileName;
                    System.Drawing.Image newImage = System.Drawing.Image.FromFile(selectedImagePath);
                    Bitmap originalImage = new Bitmap(selectedImagePath);
                    Bitmap resizedImage = new Bitmap(originalImage, pictureBox.Width, pictureBox.Height);

                    Bitmap newBitmap = ConvertTo24bpp(newImage);


                    
                    // Przypisanie przeskalowanego obrazu do PictureBox
                    pictureBox.Image = newBitmap;
                    mainImage = newBitmap;

                    System.Drawing.Imaging.PixelFormat formatPikseli = mainImage.PixelFormat;

                    Console.WriteLine($"Głębia bitowa obrazu: {System.Drawing.Image.GetPixelFormatSize(formatPikseli)} bitów na piksel");
                }
            }
        }

        public Bitmap ConvertTo24bpp(System.Drawing.Image img)
        {
            var bmp = new Bitmap(pictureBox.Width, pictureBox.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }

        private void ImageToBase64Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                Stopwatch stopwatchC = new Stopwatch();
                stopwatchC.Start();

                ImageToBase64();

                stopwatchC.Stop();

                //textBox1.Text = base64String;
                label3.Text = stopwatchC.ElapsedMilliseconds + " ms";

            }
            else
            {
                MessageBox.Show("No image given.");
            }
        }


        private void ImageToBase64()
        {
            Bitmap image = (Bitmap)pictureBox.Image;


            // Konwersja obiektu Bitmap na ciąg bajtów (array of bytes)
            byte[] imageBytes;
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                imageBytes = stream.ToArray();
            }

            // Konwersja ciągu bajtów na Base64
            base64String = Convert.ToBase64String(imageBytes);

            Console.WriteLine("Długosc tekstu wynikowego base64: " + base64String.Length);
        }

        private string returnBase64FromImage(int outputSize)
        {

            // Przetworzenie obrazu na tablicę bajtów (dane z poszczególnych kanałów kolorów)
            MemoryStream memoryStream = new MemoryStream();
            mainImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] imageBytes = memoryStream.ToArray();

            //Przepisywanie bajtów do tablicy z znakami
            string inputString = Encoding.ASCII.GetString(imageBytes);

            //Uruchamianie funckji
            string textResult = returnBase64FromText(inputString, inputString.Length, outputSize);

            return textResult;
        }

        private string returnBase64FromText(string yourString, int inputSize, int outputSize)
        {
            //Alokowanie pamieci na Dane wejsciowe
            byte[] stringBytes = Encoding.ASCII.GetBytes(yourString);
            IntPtr input = Marshal.AllocHGlobal(stringBytes.Length);
            Marshal.Copy(stringBytes, 0, input, stringBytes.Length);

            
            //Alokowanie pamiecia na Dane wyjsciowe
            IntPtr output = Marshal.AllocHGlobal(outputSize);

            //Alokowanie pamieci na Tablica base64
            string base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            byte[] base64Bytes = Encoding.ASCII.GetBytes(base64);
            IntPtr base_64 = Marshal.AllocHGlobal(base64Bytes.Length);
            Marshal.Copy(base64Bytes, 0, base_64, base64Bytes.Length);

            Stopwatch stopwatchASM = new Stopwatch();
            stopwatchASM.Start();

            //Uruchomienie funkcji
            int base64_lenght = ImageToBase64(input, output, base_64, inputSize);
            //Console.WriteLine("Długosc tekstu wynikowego base64: " + base64_lenght);

            stopwatchASM.Stop();

            

            string outputStringResult = Marshal.PtrToStringAnsi(output);

            //textBox2.Text = outputStringResult;
            label4.Text = stopwatchASM.ElapsedMilliseconds + " ms";

            //Zwolnienie zaalokowanej pamieci
            Marshal.FreeHGlobal(input);
            Marshal.FreeHGlobal(base_64);
            Marshal.FreeHGlobal(output);

            return outputStringResult;
        }

        private void CopyOutput(object sender, EventArgs e)
        {

            if (base64String != "")
            {
                // Zakładając, że masz kontrolkę TextBox o nazwie textBox1

                // Pobranie tekstu z TextBox
                string textToCopy = base64String;

                // Kopiowanie tekstu do schowka
                Clipboard.SetText(textToCopy);
            }
            else
            {
                MessageBox.Show("No text given.");
            }
        }

        private void CopyOutputASM(object sender, EventArgs e)
        {
            if (base64Asm != "")
            {
                // Zakładając, że masz kontrolkę TextBox o nazwie textBox1

                // Pobranie tekstu z TextBox
                string textToCopy = base64Asm;

                // Kopiowanie tekstu do schowka
                Clipboard.SetText(textToCopy);

            }
            else
            {
                MessageBox.Show("No text given.");
            }
        }

        // Funkcja do dekodowania ciągu Base64 na obraz
        private System.Drawing.Image ConvertBase64ToImage(string base64String)
        {

            // Konwersja ciągu Base64 na tablicę bajtów
            byte[] imageBytes = Convert.FromBase64String(base64String);

            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
            {
                // Konwersja tablicy bajtów na obraz
                System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream);
                return image;
            }
            


        }


        private void Base64ToImage(object sender, EventArgs e)
        {

            if (base64String != "")
            {
                // Przykładowe użycie funkcji ConvertBase64ToImage
                //string base64String = textBox1.Text; // Zastąp tym rzeczywistym ciągiem Base64
                System.Drawing.Image imageFromBase64 = ConvertBase64ToImage(base64String);

                Bitmap newBitmap = ConvertTo24bpp(imageFromBase64);

                // Teraz możesz użyć obiektu 'imageFromBase64', np. wyświetlić go w kontrolce PictureBox
                pictureBox.Image = newBitmap;
            } else
            {
                MessageBox.Show("No text given.");
            }
            

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if(mainImage != null)
            {
                // Załóżmy, że resizedImage jest obiektem Bitmap, który chcesz przekształcić
                Bitmap inputImage = new Bitmap(mainImage);
                Bitmap resizedImage = new Bitmap(pictureBox.Image);


                // Pobieranie danych o obrazie

                BitmapData bitmapData0 = inputImage.LockBits(new Rectangle(0, 0, resizedImage.Width, resizedImage.Height), ImageLockMode.ReadWrite, resizedImage.PixelFormat);
                IntPtr scan0 = bitmapData0.Scan0;

                BitmapData bitmapData1 = resizedImage.LockBits(new Rectangle(0, 0, resizedImage.Width, resizedImage.Height), ImageLockMode.ReadWrite, resizedImage.PixelFormat);
                IntPtr scan1 = bitmapData1.Scan0;


                int imageSizeInBytes = Math.Abs(bitmapData1.Stride) * resizedImage.Height;
                int value = trackBar1.Value;

                value -= 255;

                Console.WriteLine(value.ToString());


                // Wywołanie funkcji asemblowej
                ASMAdjustBrightness(scan1, scan0, value, imageSizeInBytes);

                // Odblokowanie danych obrazu po przekształceniach
                resizedImage.UnlockBits(bitmapData0);

                pictureBox.Image = resizedImage;
            }
            
        }

        private void ImageToBase64ASM(object sender, EventArgs e)
        {

            if (pictureBox.Image != null)
            {
                ImageToBase64();

                string base64StringASM = returnBase64FromImage(base64String.Length);

                base64Asm = base64StringASM.Substring(0, base64StringASM.Length - 7);
                //textBox2.Text = base64Asm;

                Console.WriteLine("Długosc tekstu ASM " + base64StringASM.Length);

            }
            else
            {
                MessageBox.Show("No image given.");
            }

            

        }

        private void TextToBase64ASM(object sender, EventArgs e)
        {
            if (base64Asm != "")
            {
                // Przykładowe użycie funkcji ConvertBase64ToImage
                string base64String = base64Asm; // Zastąp tym rzeczywistym ciągiem Base64
                System.Drawing.Image imageFromBase64 = ConvertBase64ToImage(base64String);
                Bitmap newBitmap = ConvertTo24bpp(imageFromBase64);
                // Teraz możesz użyć obiektu 'imageFromBase64', np. wyświetlić go w kontrolce PictureBox
                pictureBox.Image = newBitmap;
            }
            else
            {
                MessageBox.Show("No text given.");
            }
        }

    }
}
