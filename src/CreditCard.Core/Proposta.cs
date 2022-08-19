using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditCard.Core
{
    public class Proposta
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Idade { get; set; }
        public decimal RendaBrutaMensal { get; set; }
        public string NumeroCartao { get; set; }
    }
}
