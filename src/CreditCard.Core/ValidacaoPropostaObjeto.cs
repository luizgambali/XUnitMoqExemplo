using System;

namespace CreditCard.Core
{
    public class ValidacaoPropostaObjeto
    {
        private const int IdadeMaximaReferenciaAutomatica = 20;
        private const int LimiteRendaSuperior = 100_000;
        private const int LimiteRendaInferior = 20_000;

        private readonly IValidadorNumeroCartao _validador;
        private readonly IValidadorCartao _validadorCartao;

        public ValidacaoPropostaObjeto(IValidadorCartao validadorCartao = null)
        {
            _validadorCartao = validadorCartao;
        }

        /// <summary>
        /// Especifico para simulação do teste de configuração de casos em que a propriedade é um objeto
        /// 
        /// Exemplo: 
        /// 
        ///     public Person person { get; set; }
        /// 
        /// </summary>
        /// <param name="proposta"></param>
        /// <returns></returns>        
        public DecisaoAprovacao ValidarPropostaPropriedadeObjeto(Proposta proposta)
        {
            if (proposta.RendaBrutaMensal >= LimiteRendaSuperior)
            {
                return DecisaoAprovacao.AceitacaoAutomatica;
            }

            if (_validadorCartao.ChaveDeAcesso.Chave == "EXPIRED")
            {
                return DecisaoAprovacao.DecisaoManual;
            }

            var numeroValido = _validadorCartao.NumeroValido(proposta.NumeroCartao);

            if (!numeroValido)
            {
                return DecisaoAprovacao.DecisaoManual;
            }

            if (proposta.Idade <= IdadeMaximaReferenciaAutomatica)
            {
                return DecisaoAprovacao.DecisaoManual;
            }

            if (proposta.RendaBrutaMensal < LimiteRendaInferior)
            {
                return DecisaoAprovacao.RecusaAutomatica;
            }

            return DecisaoAprovacao.DecisaoManual;
        }
    }
}
