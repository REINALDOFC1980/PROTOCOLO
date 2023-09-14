using iTextSharp.text.pdf;
using SautinSoft;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CondutorInfrator.Util
{
    public class ConvertPDFtoIMG
    {
        Funcao funcao = new Funcao();
        public int ConvertPdfToImage(string fileName, string pro_numero, string tipo_doc)
        {


            try
            {
                var t = 0;
                PdfFocus f = new PdfFocus();

                f.Serial = "80019787229";

                string pdfFile = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"] + pro_numero + '\\' + fileName;
                string jpegDir = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo_Temp"] + pro_numero;

                if (!Directory.Exists(jpegDir))
                    Directory.CreateDirectory(jpegDir);

                //convert PDF to IMG
                f.OpenPdf(pdfFile);


                if (f.PageCount > 0)
                {
                    
                    f.ImageOptions.ImageFormat = ImageFormat.Jpeg;
                    f.ImageOptions.Dpi = 120;
                    f.ImageOptions.JpegQuality = 95;

                    for ( int page = 1; page <= f.PageCount; page++)
                    {
                        string jpegPath = Path.Combine(jpegDir, page + tipo_doc + ".jpeg");
                        f.ToImage(jpegPath, page);

                        //salvando a imagem convertida no banco
                        if (!File.Exists(jpegPath))
                            return t;

                            funcao.SalvarImgbanco(jpegPath, tipo_doc, pro_numero);
                    }
                }

                if (Directory.Exists(jpegDir))
                {
                    string[] files = Directory.GetFiles(jpegDir, "*.jpeg");
                    foreach (string file in files)
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                       
                        if (File.Exists(file))
                            File.Delete(file);
                    }
                    if (Directory.Exists(jpegDir))
                        Directory.Delete(jpegDir, true);

                }
                 t = f.PageCount;
               
                return t;
            }
            catch (Exception ex)
            {

                throw;
            }        

        }        
    }
}
























        //public void ConvertPdfToImage(string fileName, string pasta, ImageSaveOptions.ImageFileType outputFileType)
        //{

        //    string sourcePath = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"] + pasta;
        //    string targerPath = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo_Temp"] + pasta;

        //    if (!Directory.Exists(targerPath))
        //        Directory.CreateDirectory(targerPath);


        //    var conversionConfig = new ConversionConfig { StoragePath = sourcePath, OutputPath = targerPath };
        //    var conversionhandler = new ConversionHandler(conversionConfig);

        //    var saveOptions = new ImageSaveOptions
        //    {
        //        ConvertFileType = outputFileType
        //    };
        //    var convertedDocumentPath = conversionhandler.Convert(sourcePath + fileName, saveOptions);

        //    for (int pageNum = 1; pageNum <= convertedDocumentPath.PageCount; pageNum++)
        //    {
        //        convertedDocumentPath.Save(Path.GetFileNameWithoutExtension(fileName) + pageNum.ToString() + '.' + outputFileType, pageNum);
        //    }

        // }






            //public void ConvertPdfToImage()

            //{

            //    PdfDocument doc = new PdfDocument();

            //    doc.LoadFromFile(@"C:\Users\reinaldo\Downloads\sample.pdf");

            //    Image bmp = doc.SaveAsImage(0);

            //    Image emf = doc.SaveAsImage(0, Spire.Pdf.Graphics.PdfImageType.Metafile);

            //    Image zoomImg = new Bitmap((int)(emf.Size.Width * 2), (int)(emf.Size.Height * 2));

            //    using (Graphics g = Graphics.FromImage(zoomImg))

            //    {

            //        g.ScaleTransform(2.0f, 2.0f);

            //        g.DrawImage(emf, new Rectangle(new Point(0, 0), emf.Size), new Rectangle(new Point(0, 0), emf.Size), GraphicsUnit.Pixel);

            //    }

            //    bmp.Save("convertToBmp.jpeg", ImageFormat.Jpeg);

            //    //System.Diagnostics.Process.Start("convertToBmp.bmp");

            //    //emf.Save("convertToEmf.png", ImageFormat.Png);

            //    //System.Diagnostics.Process.Start("convertToEmf.png");

            //    //zoomImg.Save("convertToZoom.png", ImageFormat.Png);

            //    //System.Diagnostics.Process.Start("convertToZoom.png");

            //}




    //}





