using CreditCard.Core;
using Moq;
using System;
using Xunit;

namespace CreditCardCore.Tests
{
    public class ModeloPropostaValidacaoTest
    {
        [Fact]
        [Trait("Category","Testes simples")]
        public void AceitarPropostaAltaRenda()
        {
            Mock<IValidadorNumeroCartao> validadorCartao = new Mock<IValidadorNumeroCartao>();

            var sut = new ValidacaoProposta(validadorCartao.Object);

            var proposta = new Proposta() { RendaBrutaMensal = 100_000 };

            DecisaoAprovacao decisao = sut.ValidarProposta(proposta);

            Assert.Equal(DecisaoAprovacao.AceitacaoAutomatica, decisao);
        }

        [Fact]
        [Trait("Category", "Testes simples")]
        public void ValidarPropostaPorIdade()
        {
            Mock<IValidadorNumeroCartao> validadorCartao = new Mock<IValidadorNumeroCartao>();

            /*
              Se o setup do mock não for realizado, a consistência de idade não será executada corretamente.
              
              Tanto a consistencia de idade quanto numero da conta retornam DecisaoManual.
              A consistencia de numero da conta vem antes no método, e se não é passado nenhum 
              valor, no numero da conta, o método retornará o valor padrao FALSE, e cairá no 
              "if" que retorna DecisaoManual. Note que, no final deste teste, ele espera DecisaoManual!
              
            */

            validadorCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true);

            var sut = new ValidacaoProposta(validadorCartao.Object);

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
            //Mock<IValidadorNumeroCartao> validadorCartao = new Mock<IValidadorNumeroCartao>(MockBehavior.Strict);
            Mock<IValidadorNumeroCartao> validadorCartao = new Mock<IValidadorNumeroCartao>(); //foi necessário retirar o strict, pq ao implementa a propriedade Chave, o strict quebra esse teste


            //como declaramos o mock como strict, é obrigatorio realizar o setup. Se ele não for realizado, o teste irá falhar
            validadorCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(false);

            var sut = new ValidacaoProposta(validadorCartao.Object);

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

            var sut = new ValidacaoProposta(validadorNumeroCartao.Object, validadorCartao.Object);

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
            //objeto original
            var validadorNumeroCartao = new Mock<IValidadorNumeroCartao>();
            validadorNumeroCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true);

            //objeto que simula uma propriedade que está em outro objeto
            var validadorCartao = new Mock<IValidadorCartao>();
            validadorCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true);
            validadorCartao.Setup(x => x.ChaveDeAcesso.Chave).Returns("EXPIRED"); //atribuicao direta do valor

            var sut = new ValidacaoProposta(validadorNumeroCartao.Object, validadorCartao.Object);

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
            var validadorNumeroCartao = new Mock<IValidadorNumeroCartao>();
            
            validadorNumeroCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true);

            //por padrao, o objeto mockado não guarda a informação.
            //A linha abaixo permite que o objeto seja alterado e seu valor guardado
            validadorNumeroCartao.SetupProperty(x => x.ModoDeValidacao); 

            var sut = new ValidacaoProposta(validadorNumeroCartao.Object);

            var proposta = new Proposta()
            {
                Idade = 19
            };

            sut.ValidarProposta(proposta);

            Assert.Equal(ModoValidacao.Detalhado, validadorNumeroCartao.Object.ModoDeValidacao);
        }

        [Fact]
        [Trait("Category","Testes verificando se o método ou propriedade foram acionados")]
        public void VerificaSeNumeroValidoFoiAcionado()
        {
            var validadorNumeroCartao = new Mock<IValidadorNumeroCartao>();

            validadorNumeroCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true);

            //por padrao, o objeto mockado não guarda a informação.
            //A linha abaixo permite que o objeto seja alterado e seu valor guardado
            validadorNumeroCartao.SetupProperty(x => x.ModoDeValidacao);

            var sut = new ValidacaoProposta(validadorNumeroCartao.Object);

            var proposta = new Proposta()
            {
                NumeroCartao = "123"
            };

            sut.ValidarProposta(proposta);

            //verifica se o método NumeroValido foi acionado durante a execução do teste
            validadorNumeroCartao.Verify(x => x.NumeroValido(It.IsAny<string>()),"O método não foi acionado");
        }

        [Fact]
        [Trait("Category", "Testes verificando se o método ou propriedade foram acionados")]
        public void VerificaSeNumeroValidoNaoFoiAcionado()
        {
            var validadorNumeroCartao = new Mock<IValidadorNumeroCartao>();

            validadorNumeroCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true);

            //por padrao, o objeto mockado não guarda a informação.
            //A linha abaixo permite que o objeto seja alterado e seu valor guardado
            validadorNumeroCartao.SetupProperty(x => x.ModoDeValidacao);

            var sut = new ValidacaoProposta(validadorNumeroCartao.Object);

            var proposta = new Proposta()
            {
                RendaBrutaMensal = 100_000
            };

            sut.ValidarProposta(proposta);

            //verifica se o método NumeroValido foi acionado durante a execução do teste
            validadorNumeroCartao.Verify(x => x.NumeroValido(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Testes verificando se o método ou propriedade foram acionados")]
        public void VerificaSeChaveFoiAcionadoGET()
        {
            var validadorNumeroCartao = new Mock<IValidadorNumeroCartao>();

            validadorNumeroCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true);

            //por padrao, o objeto mockado não guarda a informação.
            //A linha abaixo permite que o objeto seja alterado e seu valor guardado
            validadorNumeroCartao.SetupProperty(x => x.ModoDeValidacao);

            var sut = new ValidacaoProposta(validadorNumeroCartao.Object);

            var proposta = new Proposta()
            {
                RendaBrutaMensal = 99_000
            };

            sut.ValidarProposta(proposta);

            //verifica se o método NumeroValido foi acionado durante a execução do teste
            validadorNumeroCartao.VerifyGet(x => x.Chave);
        }
        [Fact]
        [Trait("Category", "Testes verificando se o método ou propriedade foram acionados")]
        public void VerificaSeChaveFoiAcionadoSET()
        {
            var validadorNumeroCartao = new Mock<IValidadorNumeroCartao>();

            validadorNumeroCartao.Setup(x => x.NumeroValido(It.IsAny<string>())).Returns(true);

            //por padrao, o objeto mockado não guarda a informação.
            //A linha abaixo permite que o objeto seja alterado e seu valor guardado
            validadorNumeroCartao.SetupProperty(x => x.ModoDeValidacao);

            var sut = new ValidacaoProposta(validadorNumeroCartao.Object);

            var proposta = new Proposta()
            {
                RendaBrutaMensal = 99_000,
                Idade = 19
            };

            sut.ValidarProposta(proposta);

            //verifica se o método NumeroValido foi acionado durante a execução do teste
            validadorNumeroCartao.VerifySet(x => x.ModoDeValidacao);
        }
    }
}
