using CreditCard.Core;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace CreditCardCore.Tests
{
    public class ModeloPropostaValidacaoTest
    {
        private Mock<IValidadorNumeroCartao> validadorCartao;
        private ValidacaoProposta sut;

        public ModeloPropostaValidacaoTest()
        {
            validadorCartao = new Mock<IValidadorNumeroCartao>();
            validadorCartao.SetupAllProperties();
            validadorCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true);
            validadorCartao.Setup(x => x.Chave).Returns("EXPIRED");

            sut = new ValidacaoProposta(validadorCartao.Object);
        }


        [Fact]
        [Trait("Category","Testes simples")]
        public void AceitarPropostaAltaRenda()
        {
            var proposta = new Proposta() { RendaBrutaMensal = 100_000 };

            DecisaoAprovacao decisao = sut.ValidarProposta(proposta);

            Assert.Equal(DecisaoAprovacao.AceitacaoAutomatica, decisao);
        }

        [Fact]
        [Trait("Category", "Testes simples")]
        public void ValidarPropostaPorIdade()
        {
            Mock<IValidadorNumeroCartao> validadorCartao = new Mock<IValidadorNumeroCartao>();

            var proposta = new Proposta() { Idade = 19 };

            DecisaoAprovacao decisao = sut.ValidarProposta(proposta);

            Assert.Equal(DecisaoAprovacao.DecisaoManual, decisao);
        }

        /// <summary>
        /// Validação da implementação do método NumeroValido(string numeroCartao)
        /// </summary>
        [Fact]
        [Trait("Category", "Testes simples")]
        public void RecusarPropostaBaixaRenda()
        {
            Mock<IValidadorNumeroCartao> validadorCartao = new Mock<IValidadorNumeroCartao>();

            validadorCartao.Setup(x => x.NumeroValido("123")).Returns(true); //mesmo valor que será passado para o objeto de teste abaixo

            // Outras formas de usar o Setup:

            // validadorCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true); //qualquer string será valida para o objeto de teste abaixo
            // validadorCartao.Setup(x => x.NumeroValido(It.Is<string>(numero => numero.Contains("12")))).Returns(true); //qualquer string que contenha o valor '12' será valida para o objeto de teste abaixo
            // validadorCartao.Setup(x => x.NumeroValido(It.IsInRange<string>("1","3",Range.Inclusive))).Returns(true); //qualquer string que esteja entre "1" e "3" será valida para o objeto de teste abaixo
            // validadorCartao.Setup(x => x.NumeroValido(It.IsIn<string>("123", "124", "125X"))).Returns(true); //qualquer string que esteja na lista passada como parametro será valida para o objeto de teste abaixo
            // validadorCartao.Setup(x => x.NumeroValido(It.IsRegex("^[0-9]*$"))).Returns(true); //qualquer string que atenda a exressão regular será valida para o objeto de teste abaixo

            var sut = new ValidacaoProposta(validadorCartao.Object);

            //objeto de teste
            var proposta = new Proposta() 
                { 
                    RendaBrutaMensal = 19_999,
                    Idade = 40,
                    NumeroCartao = "123" // valor para verificação
                };

            DecisaoAprovacao decisao = sut.ValidarProposta(proposta);

            Assert.Equal(DecisaoAprovacao.RecusaAutomatica, decisao);
        }

        [Fact]
        [Trait("Category", "Testes simples")]
        public void RecusarPropostaNumeroCartaoInvalido()
        {
            sut = new ValidacaoProposta(validadorCartao.Object);

            var proposta = new Proposta();

            DecisaoAprovacao decisao = sut.ValidarProposta(proposta);

            Assert.Equal(DecisaoAprovacao.DecisaoManual, decisao);
        }

        /// <summary>
        /// Validação da implementação do método NumeroValido(string numeroCartao, out bool Valido)
        /// </summary>
        [Fact]
        [Trait("Category", "Testes simples")]
        public void RecusarPropostaBaixaRendaOutDemo()
        {
            Mock<IValidadorNumeroCartao> validadorCartao = new Mock<IValidadorNumeroCartao>();

            var retornoValido = true;
            validadorCartao.Setup(x => x.NumeroValido(It.IsAny<string>(), out retornoValido));

            var sut = new ValidacaoProposta(validadorCartao.Object);

            var proposta = new Proposta()
            {
                RendaBrutaMensal = 19_999,
                Idade = 40,
            };

            DecisaoAprovacao decisao = sut.ValidarPropostaUsandoRetornoOut(proposta);

            Assert.Equal(DecisaoAprovacao.RecusaAutomatica, decisao);
        }

        [Fact]
        [Trait("Category", "Teste com retorno de função")]
        public void ValidarPropostaChaveExpirada()
        {
            var validadorCartao = new Mock<IValidadorNumeroCartao>();

            //configuração de varios itens do teste
            validadorCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true);
            //validadorCartao.Setup(x => x.Chave).Returns("EXPIRED");
            validadorCartao.Setup(x => x.Chave).Returns(RetornaValorExpiredTeste());
            var sut = new ValidacaoProposta(validadorCartao.Object);

            var proposta = new Proposta()
            {
                Idade = 42
            };

            DecisaoAprovacao decisao = sut.ValidarProposta(proposta);

            Assert.Equal(DecisaoAprovacao.DecisaoManual, decisao);
        }

        string RetornaValorExpiredTeste()
        {
            //faz algum processamento qualquer aqui, e retorna o valor para o mock
            return "EXPIRED";
        }

        /// <summary>
        /// Simular configuração de propriedade/retorno de método que é um objeto
        /// </summary>
        [Fact]
        [Trait("Category", "Testes retorno objeto composto")]
        public void ValidarPropostaChaveExpiradaPropriedadeObjeto()
        {
            //objeto original
            var validadorNumeroCartao = new Mock<IValidadorNumeroCartao>();
            validadorNumeroCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true);

            //objeto que simula uma propriedade que está em outro objeto
            var validadorCartao = new Mock<IValidadorCartao>();
            validadorCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true);
            var mockChave = new Mock<IChave>();

            mockChave.Setup(x => x.Chave).Returns("EXPIRED");

            validadorCartao.Setup(x => x.ChaveDeAcesso).Returns(mockChave.Object);

            var sut = new ValidacaoPropostaObjeto(validadorCartao.Object);

            var proposta = new Proposta()
            {
                Idade = 42
            };

            DecisaoAprovacao decisao = sut.ValidarPropostaPropriedadeObjeto(proposta);

            Assert.Equal(DecisaoAprovacao.DecisaoManual, decisao);
        }

        /// <summary>
        /// Simular configuração de propriedade/retorno de método que é um objeto
        /// </summary>
        [Fact]
        [Trait("Category", "Testes retorno objeto composto")]
        public void ValidarPropostaChaveExpiradaPropriedadeObjetoValorDireto()
        {
            //objeto que simula uma propriedade que está em outro objeto
            var validadorCartao = new Mock<IValidadorCartao>();
            validadorCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true);
            validadorCartao.Setup(x => x.ChaveDeAcesso.Chave).Returns("EXPIRED"); //atribuicao direta do valor

            var sut = new ValidacaoPropostaObjeto(validadorCartao.Object);

            var proposta = new Proposta()
            {
                Idade = 42
            };

            DecisaoAprovacao decisao = sut.ValidarPropostaPropriedadeObjeto(proposta);

            Assert.Equal(DecisaoAprovacao.DecisaoManual, decisao);
        }

        [Fact]
        [Trait("Category", "Testes verificação alteração de dados propriedade mock object")]
        public void VerificaSeValorDaPropriedadeFoiAlterado()
        {
            var proposta = new Proposta()
            {
                Idade = 19
            };

            sut.ValidarProposta(proposta);

            Assert.Equal(ModoValidacao.Detalhado, validadorCartao.Object.ModoDeValidacao);
        }

        [Fact]
        [Trait("Category","Testes verificando se o método ou propriedade foram acionados")]
        public void VerificaSeNumeroValidoFoiAcionado()
        {
            validadorCartao.Setup(x => x.Chave).Returns("OK");

            var sut = new ValidacaoProposta(validadorCartao.Object);

            var proposta = new Proposta()
            {
                Idade = 19
            };

            sut.ValidarProposta(proposta);

            //verifica se o método NumeroValido foi acionado durante a execução do teste
            validadorCartao.Verify(x => x.NumeroValido(It.IsAny<string>()),"O método não foi acionado");
        }

        [Fact]
        [Trait("Category", "Testes verificando se o método ou propriedade foram acionados")]
        public void VerificaSeNumeroValidoNaoFoiAcionado()
        {

            //por padrao, o objeto mockado não guarda a informação.
            //A linha abaixo permite que o objeto seja alterado e seu valor guardado
            validadorCartao.SetupProperty(x => x.ModoDeValidacao);

            var proposta = new Proposta()
            {
                RendaBrutaMensal = 100_000
            };

            sut.ValidarProposta(proposta);

            //verifica se o método NumeroValido foi acionado durante a execução do teste
            validadorCartao.Verify(x => x.NumeroValido(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Testes verificando se o método ou propriedade foram acionados")]
        public void VerificaSeChaveFoiAcionadoGET()
        {
            //por padrao, o objeto mockado não guarda a informação.
            //A linha abaixo permite que o objeto seja alterado e seu valor guardado
            validadorCartao.SetupProperty(x => x.ModoDeValidacao);

            var sut = new ValidacaoProposta(validadorCartao.Object);

            var proposta = new Proposta()
            {
                RendaBrutaMensal = 99_000
            };

            sut.ValidarProposta(proposta);

            //verifica se o método NumeroValido foi acionado durante a execução do teste
            validadorCartao.VerifyGet(x => x.Chave);
        }
        [Fact]
        [Trait("Category", "Testes verificando se o método ou propriedade foram acionados")]
        public void VerificaSeChaveFoiAcionadoSET()
        {
            //por padrao, o objeto mockado não guarda a informação.
            //A linha abaixo permite que o objeto seja alterado e seu valor guardado
            validadorCartao.SetupProperty(x => x.ModoDeValidacao);

            var sut = new ValidacaoProposta(validadorCartao.Object);

            var proposta = new Proposta()
            {
                RendaBrutaMensal = 99_000,
                Idade = 19
            };

            sut.ValidarProposta(proposta);

            //verifica se o método NumeroValido foi acionado durante a execução do teste
            validadorCartao.VerifySet(x => x.ModoDeValidacao);
        }


        [Fact]
        [Trait("Category", "Testes retorno com tratamento de erro")]
        public void ValidarPropostaComErro()
        {
            validadorCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Throws<Exception>();
            //validadorCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Throws(new Exception("Mensagem para aparecer no console log do teste"));

            var sut = new ValidacaoProposta(validadorCartao.Object);

            var proposta = new Proposta() { Idade = 42 };

            DecisaoAprovacao decisao = sut.ValidarPropostaComErro(proposta);

            Assert.Equal(DecisaoAprovacao.DecisaoManual, decisao);
        }

        [Fact]
        [Trait("Category", "Validacao da proposta com evento")]
        public void ValidarPropostaComEvento()
        {
            validadorCartao.Setup(x => x.Chave).Returns("OK");

            //configuracao do disparo do evento durante a configuração do mock
            //validadorCartao.Setup(x => x.Chave).Returns("OK").Raises(x => x.ValidacaoRealizada += null, EventArgs.Empty);

            var sut = new ValidacaoProposta(validadorCartao.Object);
            var proposta = new Proposta() { Idade = 42 };

            sut.ValidarProposta(proposta);

            //disparo do evento no objeto mock
            validadorCartao.Raise(x => x.ValidacaoRealizada += null, EventArgs.Empty);

            Assert.Equal(1, sut.contadorValidacao);
        }

        [Fact]
        [Trait("Category", "Validacao da proposta com multiplas chamadas do método")]
        public void ValidarPropostaMultiplosRetornos()
        {
            validadorCartao.Setup(x => x.Chave).Returns("OK");
            validadorCartao.SetupSequence(x => x.NumeroValido(It.IsAny<string>()))
                                        .Returns(false)
                                        .Returns(true);

            
            var sut = new ValidacaoProposta(validadorCartao.Object);
            
            var proposta = new Proposta() { Idade = 42 };

            DecisaoAprovacao primeiraChamada =  sut.ValidarProposta(proposta);
            Assert.Equal(DecisaoAprovacao.DecisaoManual, primeiraChamada);

            DecisaoAprovacao segundaChamada = sut.ValidarProposta(proposta);
            Assert.Equal(DecisaoAprovacao.RecusaAutomatica, segundaChamada);
        }

        [Fact]
        [Trait("Category", "Testes de contagem de chamadas para um determinado metodo")]
        public void ValidarPropostaContagemChamadas()
        {
            validadorCartao.Setup(x => x.Chave).Returns("OK");

            var listaValidacoes = new List<string>();
            validadorCartao.Setup(x => x.NumeroValido(Capture.In(listaValidacoes)));
            var sut = new ValidacaoProposta(validadorCartao.Object);

            var proposta1 = new Proposta() { Idade = 25, NumeroCartao = "11" };
            var proposta2 = new Proposta() { Idade = 25, NumeroCartao = "22" };
            var proposta3 = new Proposta() { Idade = 25, NumeroCartao = "33" };

            sut.ValidarProposta(proposta1);
            sut.ValidarProposta(proposta2);
            sut.ValidarProposta(proposta3);

            Assert.Equal(new List<string> {"11","22","33"}, listaValidacoes);
        }

        [Fact]
        [Trait("Category", "Configurando parametros com LinqToMoq")]
        public void ValidarPropostaLinqToMoq()
        {

            IValidadorNumeroCartao validadorCartao = Mock.Of<IValidadorNumeroCartao>(
                
                    validador => validador.Chave == "EXPIRED" &&
                                 validador.NumeroValido(It.IsAny<string>()) == true

                );

            var sut = new ValidacaoProposta(validadorCartao);

            var proposta = new Proposta() { Idade = 42 };

            DecisaoAprovacao decisao = sut.ValidarPropostaComErro(proposta);

            Assert.Equal(DecisaoAprovacao.DecisaoManual, decisao);
        }
    }
}
