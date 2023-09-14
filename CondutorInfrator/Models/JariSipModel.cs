
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace CondutorInfrator.Models
{
    internal class JariSipModel
    {
        public string recAitNumero { get; set; }
        public string recJariProcesso { get; set; } 
        public string recJariDataabertura { get; set; }
        public string recJariUsuariocadastro { get; set; }
        public bool aplicarEfeitoSuspensivo { get; set; }
    }


    internal class DefesaSipModel
    {
        public string recAitNumero { get; set;}
        public string recDpProcesso { get; set;}
        public string recDpDataabertura { get; set;}
        public string recDpUsuariocadastro { get; set;}
        public bool aplicarEfeitoSuspensivo { get; set;}
    }
}
