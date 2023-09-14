using CondutorInfrator.Models;
using CondutorInfrator.Util;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Web.Script.Serialization;

namespace CondutorInfrator.DAL
{
    public class Detran
    {
        public ResultGetCnhPorCpf CnhCpf(string cpf)
        {
            string metodo = "/wscnh.aspx/GetCnhPorCpf";
            string url = ConfigurationManager.AppSettings["url_ws_detran"] + metodo;

            string json = "";

            JsonCnhCpfData data = new JsonCnhCpfData
            {
                token = "LWMN523@452@20@19z@$qmaLOf8907",
                cpf = cpf
            };

            string postData = JsonConvert.SerializeObject(data);

            ResultGetCnhPorCpf resultGetCnhPorCpf = null;

            try
            {
                json = new Funcao().GetPostJSONString(url, postData);
                resultGetCnhPorCpf = (ResultGetCnhPorCpf)new JavaScriptSerializer().Deserialize(json, typeof(ResultGetCnhPorCpf));
            }
            catch (Exception)
            {
                resultGetCnhPorCpf.status = "erro";
                resultGetCnhPorCpf.erroCode = "02";
                resultGetCnhPorCpf.erroMessage = "Não foi possível conectar com web service DETRAN! \nPor favor, tente mais tarde.";
            }

            return resultGetCnhPorCpf;
        }
    }
}