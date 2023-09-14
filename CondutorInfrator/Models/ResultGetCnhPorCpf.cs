using System;

namespace CondutorInfrator.Models
{
    public class ResultGetCnhPorCpf
    {
        public String status { get; set; }
        public String erroCode { get; set; }
        public String erroMessage { get; set; }
        public DadosGetCnhPorCpf dadosGetCnhPorCpf;
    }
}