using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditCard.Core
{
    public class ValidadorNumeroCartao : IValidadorNumeroCartao
    {
        

        public bool NumeroValido(string numeroCartao)
        {
            throw new NotImplementedException("Simulação de uma chada real do método");
        }

        public void NumeroValido(string numeroCartao, out bool Valido)
        {
            throw new NotImplementedException("Simulação de uma chada real do método");
        }

        public string Chave => throw new NotImplementedException("Simulação de uma propriedade qualquer");

        public ModoValidacao ModoDeValidacao { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler ValidacaoRealizada;
    }
}
