using System;

namespace CreditCard.Core
{
    public class ValidacaoProposta
    {
        private const int IdadeMaximaReferenciaAutomatica = 20;
        private const int LimiteRendaSuperior = 100_000;
        private const int LimiteRendaInferior = 20_000;

        private readonly IValidadorNumeroCartao _validador;
        private readonly IValidadorCartao _validadorCartao;

        public int contadorValidacao { get; private set; }

        public ValidacaoProposta(IValidadorNumeroCartao validador)
        {
            _validador = validador ?? throw new ArgumentNullException(nameof(validador));
            _validador.ValidacaoRealizada += ValidacaoPropostaRealizada;
        }

        private void ValidacaoPropostaRealizada(object sender, EventArgs e)
        {
            contadorValidacao++;
        }

        public DecisaoAprovacao ValidarProposta(Proposta proposta)
        {
            if (proposta.RendaBrutaMensal >= LimiteRendaSuperior)
            {
                return DecisaoAprovacao.AceitacaoAutomatica;
            }


            _validador.ModoDeValidacao = proposta.Idade > 30 ? ModoValidacao.Rapido : ModoValidacao.Detalhado;

            if (_validador.Chave == "EXPIRED")
            {
                return DecisaoAprovacao.DecisaoManual;
            }

            var numeroValido = _validador.NumeroValido(proposta.NumeroCartao);

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

        public DecisaoAprovacao ValidarPropostaComErro(Proposta proposta)
        {
            if (proposta.RendaBrutaMensal >= LimiteRendaSuperior)
            {
                return DecisaoAprovacao.AceitacaoAutomatica;
            }


            _validador.ModoDeValidacao = proposta.Idade > 30 ? ModoValidacao.Rapido : ModoValidacao.Detalhado;

            if (_validador.Chave == "EXPIRED")
            {
                return DecisaoAprovacao.DecisaoManual;
            }

            bool numeroValido;

            try
            {
                numeroValido = _validador.NumeroValido(proposta.NumeroCartao);
            }
            catch (Exception)
            {
                return DecisaoAprovacao.DecisaoManual;
            }


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

        //para fins de demonstração, usando o NumeroValido com parametro out
        public DecisaoAprovacao ValidarPropostaUsandoRetornoOut(Proposta proposta)
        {
            if (proposta.RendaBrutaMensal >= LimiteRendaSuperior)
            {
                return DecisaoAprovacao.AceitacaoAutomatica;
            }

            _validador.NumeroValido(proposta.NumeroCartao, out var numeroValido);

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
