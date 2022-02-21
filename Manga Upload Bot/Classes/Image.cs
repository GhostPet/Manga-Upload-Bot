using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Manga_Upload_Bot
{
    internal class Image
    {
        public static bool CheckImageNames(string filepath)
        {
            String[] imgdirs = Directory.GetFiles(filepath);

            // İsimde sayı harici karakter var mı kontrol et
            List<int> chapvals = new List<int>();
            foreach (String img in imgdirs)
            {
                String[] temp = img.Split("\\".ToCharArray());
                String[] filename = temp[temp.Length - 1].Split(".".ToCharArray());
                if (filename.Length > 2) return false;
                try
                {
                    chapvals.Add(int.Parse(filename[0]));
                }
                catch
                {
                    return false;
                }
            }

            // En büyük sayıyı bul
            int maxval = chapvals[0];
            foreach (int chap in chapvals)
            {
                if (maxval < chap) maxval = chap;
            }

            // Düzelt
            foreach (String img in imgdirs)
            {
                String[] temp = img.Split("\\".ToCharArray());
                String filename = temp[temp.Length - 1];
                String basepath = "";
                foreach (String temp2 in temp)
                {
                    if (temp2 != filename) basepath += temp2 + "\\";
                }

                String[] temp3 = filename.Split(".".ToCharArray());
                String filetype = temp3[1];
                int filenum = int.Parse(temp3[0]);

                File.Move(basepath + filename, basepath + filenum.ToString("D" + maxval.ToString().Length) + "." + filetype);
            }
            return true;
        }

        public static bool CropImages(string filepath, int height)
        {
            string[] imgdirs = Directory.GetFiles(filepath);
            foreach (string img in imgdirs)
            {
                // Find the image info
                string[] temp = img.Split("\\".ToCharArray());
                string filename = temp[temp.Length - 1];
                string basepath = "";
                foreach (string temp2 in temp)
                {
                    if (temp2 != filename) basepath += temp2 + "\\";
                }
                string[] temp3 = filename.Split(".".ToCharArray());
                string filetype = temp3[1];
                string filenum = temp3[0];

                // Split the Image
                MagickImage image = new MagickImage(img);
                IEnumerable<IMagickImage<ushort>> littleimgs = image.CropToTiles(image.Width, height);

                // If there's more than 1 slice, save new images,
                if (littleimgs.Count() != 1)
                {
                    int counter = 0;
                    foreach (IMagickImage littleimg in littleimgs)
                    {
                        littleimg.Write(new FileInfo(basepath + filenum + " - " + ++counter + "." + filetype));
                    }
                    // and delete the old one.
                    File.Delete(img);
                }
            }
            return true;
        }

        public static bool WatermarkImages(string filepath, string watermark, int percentage = 40)
        {
            string[] imgdirs = Directory.GetFiles(filepath);
            foreach (string img in imgdirs)
            {
                // Open Images
                MagickImage watermarkimg = new MagickImage(watermark);
                MagickImage image = new MagickImage(img);

                // Resize the watermark
                watermarkimg.Resize(image.Width / 5, 0);
                
                if (!watermarkimg.HasAlpha) watermarkimg.Alpha(AlphaOption.Opaque);
                watermarkimg.Evaluate(Channels.Alpha, EvaluateOperator.Divide, (100 / percentage));

                // Put a random watermark
                Array values = Enum.GetValues(typeof(Gravity));
                Gravity random = (Gravity)values.GetValue(new Random().Next(values.Length));
                image.Composite(watermarkimg, random, CompositeOperator.Over);

                // Save
                image.Write(img);
            }
            return true;
        }

        public static bool OptimizeImages(string filepath, int quality)
        {
            string[] imgdirs = Directory.GetFiles(filepath);
            foreach (string img in imgdirs)
            {
                // Find the image info
                string[] temp = img.Split("\\".ToCharArray());
                string filename = temp[temp.Length - 1];
                string[] temp2 = filename.Split(".".ToCharArray());
                string filetype = temp2[1];

                if (filetype == "jpg" || filetype == "jpeg")
                {
                    // Optimize
                    MagickImage image = new MagickImage(img);
                    image.SetCompression(CompressionMethod.LosslessJPEG);
                    image.Quality = quality;
                    image.Write(img);
                }
            }
            return true;
        }
    }
}
