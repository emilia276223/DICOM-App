using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace DICOMApp.Data
{
    /// <summary>
    /// Klasa przechowujaca dane obrazu
    /// Zawiera informacje o rozmiarze obrazu oraz tablicę pikseli (zapisanych w formacie ARGB)
    /// </summary>
    public class ImageData
    {
        /// <summary>
        /// Szerokość obrazu
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Wysokość obrazu
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Tablica pikseli obrazu
        /// </summary>
        public int[] Pixels { get; }

        /// <summary>
        /// Konstruktor obrazu ImageData
        /// </summary>
        /// <param name="width">Szerokość obrazu</param>
        /// <param name="height">Wysokość obrazu</param>
        /// <param name="pixels">Tablica pikseli obrazu w formacie ARGB</param>
        public ImageData(int width, int height, int[] pixels)
        {
            Width = width;
            Height = height;
            Pixels = pixels;
        }

        /// <summary>
        /// Konstruktor obrazu ImageData na postawie tablicy pikseli w formacie float (skala szarości)
        /// </summary>
        /// <param name="width">Szerokość obrazu</param>
        /// <param name="height">Wysokość obrazu</param>
        /// <param name="pixels">Piksele obrazu w formacie float</param>
        public ImageData(int width, int height, float[] pixels)
        {
            Width = width;
            Height = height;
            Pixels = new int[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                byte gray = (byte)(Math.Clamp(pixels[i], 0.0f, 1.0f) * 255);
                Pixels[i] = (255 << 24) | (gray << 16) | (gray << 8) | gray; // ARGB
            }
        }

        /// <summary>
        /// Metoda tworząca Bitmapę (Avalonia.Bitmap) na podstawie podanych parametrów
        /// </summary>
        /// <param name="width">Szerokość obrazu</param>
        /// <param name="height">Wysokość obrazu</param>
        /// <param name="pixels">Piksele obrazu w formacie ARGB</param>
        /// <returns>Avalonia.Bitmap</returns>
        private static Bitmap CreateBitmap(int width, int height, int[] pixels)
        {
            var wb = new WriteableBitmap(
                new PixelSize(width, height),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Opaque);

            using (var fb = wb.Lock())
            {
                unsafe
                {
                    byte* dst = (byte*)fb.Address;

                    for (int i = 0; i < pixels.Length; i++)
                    {
                        int pixel = pixels[i];

                        byte a = (byte)((pixel >> 24) & 0xFF);
                        byte r = (byte)((pixel >> 16) & 0xFF);
                        byte g = (byte)((pixel >> 8) & 0xFF);
                        byte b = (byte)(pixel & 0xFF);

                        int o = i * 4;
                        dst[o + 0] = b; // B
                        dst[o + 1] = g; // G
                        dst[o + 2] = r; // R
                        dst[o + 3] = a; // a
                    }
                }
            }

            return wb;
        }

        /// <summary>
        /// Metoda zwracająca tablicę pikseli obrazu w formacie float
        /// Na podstawie tablicy pikseli w formacie ARGB tego obiektu
        /// </summary>
        /// <returns>Piksele obrazu w formacie float (skala szarości)</returns>
        public float[] GetFloatPixels() 
        {
            float[] floatPixels = new float[Pixels.Length];
            for (int i = 0; i < Pixels.Length; i++)
            {
                int pixel = Pixels[i];
                byte a = (byte)((pixel >> 24) & 0xFF);
                byte r = (byte)((pixel >> 16) & 0xFF);
                byte g = (byte)(( pixel >> 8) & 0xFF);
                byte b = (byte)(pixel & 0xFF);
                floatPixels[i] = (r + g + b) / 3.0f / 255.0f; // skala szarości, średnia z kolorów
            }
            return floatPixels;
        }

        /// <summary>
        /// Metoda zwracająca Bitmapę (Avalonia.Bitmap) stworzoną 
        /// na podstawie danych obrazu zapisanych w obiekcie
        /// </summary>
        /// <returns>Avalonia.Bitmap</returns>
        public Bitmap GetBitmap()
        {
            return CreateBitmap(Width, Height, Pixels);
        }

        /// <summary>
        /// Metoda tworząca kopię obiektu ImageData
        /// </summary>
        /// <returns>Kopia obiektu ImageData</returns>
        public ImageData Copy()
        {
            var pixels = new int[Width * Height];
            Pixels.CopyTo(pixels, 0);
            return new ImageData(Width, Height, pixels);
        }
    }
}
