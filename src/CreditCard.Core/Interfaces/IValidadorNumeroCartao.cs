using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditCard.Core
{
    #region teste de configuração de propriedade de um objeto externo
    //teste de configuração de propriedade de um objeto externo
    public interface IChave
    {
        string Chave { get; }
    }
    
    public interface IValidadorCartao
    {
        bool NumeroValido(string numeroCartao);
        void NumeroValido(string numeroCartao, out bool Valido);

        IChave ChaveDeAcesso { get; }
    }
    #endregion

    #region Objeto para os testes
    public interface IValidadorNumeroCartao
    {
        bool NumeroValido(string numeroCartao);
        void NumeroValido(string numeroCartao, out bool Valido);

        string Chave { get; }

        ModoValidacao ModoDeValidacao { get; set; }

        event EventHandler ValidacaoRealizada;
    }
    #endregion
}
