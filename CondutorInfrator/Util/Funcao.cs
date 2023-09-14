﻿using CondutorInfrator.Models;
using Dapper;
using iTextSharp.text;
using iTextSharp.text.pdf;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace CondutorInfrator.Util
{
    public class Funcao
    {
        public string GetPostJSONString(string url, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Accept = "application/json";
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(postData);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public string GetJSONString(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }


        public string Md5FromStringUTF8(string input)
        {
            string saida = null;
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                saida = sb.ToString();
            }

            return saida;
        }

        public static string NomeMaquina(string input)
        {
            var nome = Environment.MachineName;
            var nomeCompleto = Dns.GetHostEntry(nome).HostName;
            return nome;

        }


        public ErroModel AnalisarAIT(string vlobusca, string vlobusca2, string vloStored)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@NumeroAIT", vlobusca);
            parameters.Add("@CPF_Requerente", vlobusca2);

            ErroModel ErroModel = DapperORM.DapperORM.ReturnList<ErroModel>(vloStored, parameters).FirstOrDefault<ErroModel>();
            return ErroModel;
        }

        public void CriarPasta(string path)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
        }



        public void ConvertPdfToImage(string fileName, string pro_numero, string tipo_doc)
        {

            //PdfFocus f = new PdfFocus();

            //string pdfFile = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"] + pro_numero + '\\' + fileName;
            //string jpegDir = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo_Temp"] + pro_numero;

            //if (!Directory.Exists(jpegDir))
            //    Directory.CreateDirectory(jpegDir);

            //f.OpenPdf(pdfFile);

            //if (f.PageCount > 0)
            //{
            //    f.ImageOptions.ImageFormat = ImageFormat.Jpeg;
            //    f.ImageOptions.Dpi = 100;

            //    f.ImageOptions.JpegQuality = 70;

            //    for (int page = 1; page <= f.PageCount; page++)
            //    {
            //        string nomearquivo = String.Format(Path.GetFileNameWithoutExtension(fileName) + "{0}.jpg", page);
            //        string jpegFile = Path.Combine(jpegDir, nomearquivo);

            //        int result = f.ToImage(jpegFile, page);
            //    }
            //}

        }


        public void SalvarImgbanco(string caminho, string nome, string pro_numero)
        {
            try
            {
                //  var path = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"] + pro_numero + "\\";
                string ImagemAvaria = caminho;

                byte[] arraybytes = null;
                FileInfo imgs = new FileInfo(ImagemAvaria);
                long numeroBytes = imgs.Length;
                FileStream fStream = new FileStream(ImagemAvaria, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fStream);
                arraybytes = br.ReadBytes((int)numeroBytes);
                byte[] dadosDaImagem = arraybytes;


                DynamicParameters parameters2 = new DynamicParameters();
                parameters2.Add("@AnxImg_Pro_id", pro_numero);
                parameters2.Add("@AnxImg_imagem", dadosDaImagem);
                parameters2.Add("@AnxImg_TipoDoc", nome);
                DapperORM.DapperORM.ExecuteWithouReturn("Stb_AnexoImagem_Inserir_Dapper", parameters2);
            }
            catch (Exception ex)
            {

                throw;
            }

         
        }


        public int StatusProcesso(string par1, string par2)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "StatusProcesso");
            parameters.Add("@VloBusca1", par1);
            parameters.Add("@VloBusca2", par2);
            parameters.Add("@VloBusca3", "");

            var statusprocesso = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters).FirstOrDefault<ProcessoViewModel>();

            if (statusprocesso == null)
                return 0;
            else
                return statusprocesso.HisPro_StsPro_Id;
        }


        public void AnexoArquivo_banco(string par1, string par2, string par3)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Pro_Codigo", par1);
                parameter.Add("@Anx_TipoDocumento", par2);
                parameter.Add("@Anx_NomeArquivo", par3);
                DapperORM.DapperORM.ExecuteWithouReturn("Stb_AnexoArquivo_Inserir_Dapper", parameter);
            }
            catch (Exception ex)
            {

                throw;
            }

            

        }


        public string Buscar_IDFotoPerfil_Condutor(string par1)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Condutor");
            parameters.Add("@VloBusca1", par1);
            parameters.Add("@VloBusca2", "");
            parameters.Add("@VloBusca3", "");

            var FotoPerfilCondutor = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters).FirstOrDefault<ProcessoViewModel>().Con_FotoCNH;


            if (FotoPerfilCondutor == null)
                return FotoPerfilCondutor = "";
            else
                return FotoPerfilCondutor;
        }

        public void CopyArquivo(string origem, string destino)
        {
            string[] picList = Directory.GetFiles(origem, "*.*");

            // Copy picture files.
            foreach (string f in picList)
            {
                string fName = f.Substring(origem.Length);
                File.Copy(Path.Combine(origem, fName), Path.Combine(destino, fName), true);
            }

            foreach (string f in picList)
            {
                File.Delete(f);
            }
        }


        public void ApagandoArquivo(string origem)
        {
            string[] picList = Directory.GetFiles(origem, "*.JPEG*");

            foreach (string f in picList)
            {
                File.Delete(f);
            }
        }



        public List<AnexoModel> ObterAnexosPorDemanda(string pro_numero)
        {
            List<AnexoModel> anexos = new List<AnexoModel>();

            DynamicParameters parameters2 = new DynamicParameters();
            parameters2.Add("@VloCampo", "AnexoProcesso");
            parameters2.Add("@VloBusca1", pro_numero);
            parameters2.Add("@VloBusca2", "");
            parameters2.Add("@VloBusca3", "");

            anexos = DapperORM.DapperORM.ReturnList<AnexoModel>("STb_Processo_Localizar_Dapper", parameters2).ToList(); ;

            return anexos;
        }


        public void Convert_Img_PDF(string caminho)
        {
            //Document Doc = new Document(PageSize.LETTER, 20, 20, 20, 20);
            Document Doc = new Document(PageSize.A4, 2, 2, 2, 2);
            string PDFOutput = caminho + "COMPROBRATORIO.pdf";
            string Folder = caminho;

            DirectoryInfo dir = new DirectoryInfo(Folder);
            FileInfo[] jpeg = dir.GetFiles("*.jpeg");

            if (jpeg.Length != 0)
            {
                PdfWriter writer = PdfWriter.GetInstance(Doc, new FileStream(PDFOutput, FileMode.Create, FileAccess.Write, FileShare.Read));
                Doc.Open();

                foreach (string F in Directory.GetFiles(Folder, "*.jpeg"))
                {
                    if (F != null)
                    {
                        Doc.NewPage();
                        Doc.Add(new Jpeg(new Uri(new FileInfo(F).FullName)));
                    }
                }
                Doc.Close();
            }

            

            //apagando as imagem criada temporariamente(O bjetivo é criar apeanas para para gerar o arquivo PDF)
            if (Directory.Exists(Folder))
            {
                string[] files = Directory.GetFiles(Folder, "*.jpeg");
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Delete(file);
                }


            }
        }



        public void excluir_arquivo_comprobatorio_temporarios(string caminho)
        {
            if (Directory.Exists(caminho))
            {
                string[] files = Directory.GetFiles(caminho, "*.jpeg");
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Delete(file);
                }

            }

        }


        //verificando se existe
        public DadosExistenteModel VerificarExistencia(string par1)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "CadastroExistente");
            parameters.Add("@VloBusca1", par1);
            parameters.Add("@VloBusca2", "");

            var Dados = DapperORM.DapperORM.ReturnList<DadosExistenteModel>("STb_Pessoa_Localizar_Dapper", parameters).FirstOrDefault<DadosExistenteModel>();

            return Dados;

        }



        public string NomeAnexo(string Nome)
        {
            var NomeAnexo = "";
            switch (Nome)
            {
                case "CNH_CONDUTOR":
                    NomeAnexo = "CNH CONDUTOR";
                    break;
                case "FORMULARIO_INDICACAO":
                    NomeAnexo = "NOTIFICAÇÃO DE AUTUAÇÃO DE INFRAÇÃO";
                    break;
                case "NOTIFICACAO_PENALIDADE":
                    NomeAnexo = "NOTIFICAÇÃO DE IMPOSIÇÃO DE PENALIDADE";
                    break;
                case "COMPROVANTE_CONDUTOR":
                    NomeAnexo = "COMPROVANTE DO CONDUTOR";
                    break;
                case "CNH_PROPRIETARIO":
                    NomeAnexo = "CNH OU RG DO REQUERENTE";
                    break;

                case "REQDEFESA":
                    NomeAnexo = "REQUERIMENTO";
                    break;
                case "PETICAO":
                    NomeAnexo = "PETIÇÃO";
                    break;
                case "CRLV":
                    NomeAnexo = "CRLV";
                    break;
                case "DOC_REQUERENTE":
                    NomeAnexo = "DOCUMENTO DE IDENTIFICAÇÃO DO REQUERENTE";
                    break;
                case "COMPROBATORIOS":
                    NomeAnexo = "COMPROBATÓRIOS";
                    break;

                case "COMPROVANTE_PROPRIETARIO":
                    NomeAnexo = "COMPROVANTE DE RESIDÊNCIA DO REQUERENTE";
                    break;

                case "DOC_DIRIGENTE":
                    NomeAnexo = "DOCUMENTAÇÃO DO DIRIGENTE";
                    break;
                case "PORTARIA_NOMEACAO":
                    NomeAnexo = "PORTARIA DE NOMEAÇÃO";
                    break;
                case "CONTRATO_SOCIAL":
                    NomeAnexo = "CONTRATO SOCIAL";
                    break;
                case "DOC_SOCIO":
                    NomeAnexo = "DOCUMENTO DO SÓCIO";
                    break;
                case "DOC_PROCURACAO":
                    NomeAnexo = "DOCUMENTO DO PROCURADOR";
                    break;
                case "DOC_OFICIO":
                    NomeAnexo = "OFÍCIO";
                    break;
                case "PROCURACAO":
                    NomeAnexo = "PROCURAÇÃO";
                    break;
                case "DOC_COMPPUBLICO":
                    NomeAnexo = "DOCUMENTO PÚBLICO";
                    break;
                case "COMPROVANTE_EMPRESA":
                    NomeAnexo = "COMPROVANTE RESIDÊNCIA DO REQUERENTE";
                    break;


                default:
                    NomeAnexo = "";
                    break;
            }

            return NomeAnexo;

        }










        public bool ArquivoExistente(string caminho)
        {
            string diretorio = caminho;


            FileInfo file = new FileInfo(diretorio);
            if (file.Exists)
            {
                return true;
            }else
                return false;

        }

    }


}