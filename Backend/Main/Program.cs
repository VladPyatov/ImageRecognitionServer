using System;
using System.Diagnostics;
using RecognitionModel;
using System.Linq;
using SQL;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using ImageContracts;
using System.IO;
using System.Collections.Generic;

namespace Main
{
    class Program
    {
        public static void ConsoleOutput(object sender, ImageRepresentation result)
        {
            Console.WriteLine($"{result.ImageName} belongs to class {result.ClassName} with prob. - {result.Prob}");

        }
        static int Main(string[] args)
        {
            Model MnistModel = new Model();

            Parallelizer<ImageRepresentation,ImageRepresentation> ModelParallelizer = new Parallelizer<ImageRepresentation,ImageRepresentation>(MnistModel);
            ModelParallelizer.OutputEvent += ConsoleOutput;

            string[] Files=null;
            try
            {
                Files = Directory.GetFiles(args.FirstOrDefault() ?? "images", "*.*").Where(s => s.EndsWith(".bmp") || s.EndsWith(".jpg") ||
                                                                                           s.EndsWith(".png") || s.EndsWith(".jpeg")).ToArray();

                // 0 images found after LINQ filtering
                if (Files == null) throw new ArgumentNullException("Files == NULL");
                if (Files.Length == 0) throw new Exception("There is no images in the directory");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Wrong input"+ex.Message);
            }

            List<ImageRepresentation> Images = new List<ImageRepresentation>();
            foreach (string FilePath in Files)
            {
                byte[] ByteImage = File.ReadAllBytes(FilePath);
                Images.Add(new ImageRepresentation { ImageName = FilePath, Base64Image = Convert.ToBase64String(ByteImage) });
            }

            ModelParallelizer.Run(Images);

            return 0;
        }
    }
}
