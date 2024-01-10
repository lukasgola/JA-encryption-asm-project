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

        private void ImageToBase64(object sender, EventArgs e)
        {


            if (pictureBox.Image != null)
            {

                // Konwersja obrazu na format Base64
                using (MemoryStream memoryStream = new MemoryStream())
                {

                    Bitmap image = ConvertTo24bpp(pictureBox.Image);

                    image.Save(memoryStream, ImageFormat.Bmp); // Możesz wybrać odpowiedni format obrazu
                    byte[] imageBytes = memoryStream.ToArray();

                    string path = "C:\\Users\\atomo\\Documents\\GitHub\\JA-encryption-asm-project\\Encryption\\C#.txt";
                    int[] linesToWrite = imageBytes.Select(n => (int)n).ToArray();
                    string[] stringArray = linesToWrite.Select(n => n.ToString()).ToArray();

                    // Zapis tablicy stringów do pliku
                    File.WriteAllLines(path, stringArray);

                    /*
                    char[] charArray = Encoding.ASCII.GetChars(imageBytes);

                    string test1 = "";

                    for (int i = 0; i < charArray.Length; i++)
                    {
                        test1 += charArray[i];
                    }
                    string byteValue = Convert.ToString((int)test1[0], 2).PadLeft(16, '0');


                    Console.WriteLine("Wynikowy tekst C#: " + sizeof(byte));
                    */



                    string base64String = Convert.ToBase64String(imageBytes);
                    textBox1.Text = base64String;

                    string base64StringASM = returnBase64FromImage(base64String);
                    textBox2.Text = base64StringASM;

                }
            } else
            {
                MessageBox.Show("No image given.");
            }
            
            
        }

        private string returnBase64FromImage(string base64String)
        {
            //Dane wejsciowe
            Bitmap inputImage = ConvertTo24bpp(pictureBox.Image);
            BitmapData bitmapData0 = inputImage.LockBits(new Rectangle(0, 0, inputImage.Width, inputImage.Height), ImageLockMode.ReadWrite, inputImage.PixelFormat);
            IntPtr scan0 = bitmapData0.Scan0;
            int imageSizeInBytes = Math.Abs(bitmapData0.Stride) * inputImage.Height;

            // Przetworzenie obrazu na tablicę bajtów (dane z poszczególnych kanałów kolorów)
            MemoryStream memoryStream = new MemoryStream();
            inputImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] imageBytes = memoryStream.ToArray();

            string path = "C:\\Users\\atomo\\Documents\\GitHub\\JA-encryption-asm-project\\Encryption\\asm.txt";
            int[] linesToWrite = imageBytes.Select(n => (int)n).ToArray();

            char[] charArray = Encoding.ASCII.GetChars(imageBytes);

            string inputString = "";

            for (int i = 0; i < charArray.Length; i++)
            {
                inputString += charArray[i];
            }

            
            Console.WriteLine("Wynikowy tekst byte: " + inputString);

            string[] stringArray = linesToWrite.Select(n => n.ToString()).ToArray();

            // Zapis tablicy stringów do pliku
            //File.WriteAllLines(path, test1);
            File.WriteAllBytes(path, imageBytes);

            System.Drawing.Imaging.PixelFormat formatPikseli = inputImage.PixelFormat;

            Console.WriteLine($"Głębia bitowa obrazu ASM: {System.Drawing.Image.GetPixelFormatSize(formatPikseli)} bitów na piksel");

            //Przygotowanie pamieci do wyniku
            //byte[] outputBytes = Encoding.ASCII.GetBytes(base64String);
            //IntPtr output = Marshal.AllocHGlobal(outputBytes.Length);
            //int outputSize = outputBytes.Length;
            //Marshal.Copy(outputBytes, 0, output, outputSize);

            //Console.WriteLine("imageSizeinBytes: " + imageSizeInBytes);
            //Console.WriteLine("outputSize: " + outputSize);

            //Tablica base64
            //string base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            //byte[] base64Bytes = Encoding.ASCII.GetBytes(base64);
            //IntPtr base_64 = Marshal.AllocHGlobal(base64Bytes.Length);
            //Marshal.Copy(base64Bytes, 0, base_64, base64Bytes.Length);


            //int base64_lenght = ImageToBase64(scan0, output, base_64, imageSizeInBytes);
            //Console.WriteLine("Długosc tekstu wynikowego base64: " + base64_lenght);

            //Marshal.FreeHGlobal(base_64);
            

            //string outputStringResult = Marshal.PtrToStringAnsi(output);

            string textResult = returnBase64FromText(inputString, inputString.Length, base64String);

            Console.WriteLine("Wynik ostateczny: " + textResult);

            return textResult;
        }

        private string returnBase64FromText(string yourString, int inputSize, string base64String)
        {
            //Dane wejsciowe
            byte[] stringBytes = Encoding.ASCII.GetBytes(yourString);
            IntPtr input = Marshal.AllocHGlobal(stringBytes.Length);
            Marshal.Copy(stringBytes, 0, input, stringBytes.Length);

            //Przygotowanie pamieci do wyniku
            byte[] outputBytes = Encoding.ASCII.GetBytes(base64String);
            IntPtr output = Marshal.AllocHGlobal(outputBytes.Length);

            //Tablica base64
            string base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            byte[] base64Bytes = Encoding.ASCII.GetBytes(base64);
            IntPtr base_64 = Marshal.AllocHGlobal(base64Bytes.Length);
            Marshal.Copy(base64Bytes, 0, base_64, base64Bytes.Length);


            int base64_lenght = ImageToBase64(input, output, base_64, inputSize);
            Console.WriteLine("Długosc tekstu wynikowego base64: " + base64_lenght);

            Marshal.FreeHGlobal(base_64);


            string outputStringResult = Marshal.PtrToStringAnsi(output);
            return outputStringResult;
        }

        private void CopyOutput(object sender, EventArgs e)
        {

            if (textBox1.Text != "")
            {
                // Zakładając, że masz kontrolkę TextBox o nazwie textBox1

                // Pobranie tekstu z TextBox
                string textToCopy = textBox1.Text;

                // Kopiowanie tekstu do schowka
                Clipboard.SetText(textToCopy);

                // Komunikat potwierdzający skopiowanie tekstu
                MessageBox.Show("Text was copied to the clipboard.");
            }
            else
            {
                MessageBox.Show("No text given.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                // Zakładając, że masz kontrolkę TextBox o nazwie textBox1

                // Pobranie tekstu z TextBox
                string textToCopy = textBox2.Text;

                // Kopiowanie tekstu do schowka
                Clipboard.SetText(textToCopy);

                // Komunikat potwierdzający skopiowanie tekstu
                MessageBox.Show("Text was copied to the clipboard.");
            }
            else
            {
                MessageBox.Show("No text given.");
            }
        }

        // Funkcja do dekodowania ciągu Base64 na obraz
        private System.Drawing.Image ConvertBase64ToImage(string base64String)
        {;

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

            if (textBox1.Text != "")
            {
                // Przykładowe użycie funkcji ConvertBase64ToImage
                string base64String = textBox1.Text; // Zastąp tym rzeczywistym ciągiem Base64
                System.Drawing.Image imageFromBase64 = ConvertBase64ToImage(base64String);

                // Teraz możesz użyć obiektu 'imageFromBase64', np. wyświetlić go w kontrolce PictureBox
                pictureBox.Image = imageFromBase64;
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

        
    }
}
