# Exemplo_Xunit_Moq

Exemplo baseado no curso Mocking with Moq 4 and xUnit. O objetivo principal desde projeto é demonstrar a utilização de objetos 
"mockados" nos testes, utilizando XUnit. O projeto é semelhante ao projeto apresentado no curso, com alguma variação de nome de 
classes e atributos Este projeto é composto por uma class library (CreditCard.Core), e o projeto de testes.

- Verificar docs\resumo_xunit.txt para ter uma compreensão melhor do curso em relação ao XUnit
- Verificar docs\resumo_moq_xunit.txt para ter uma compreensão melhor do curso em relação ao Moq

## Relatorio de Cobertura de Testes

Dentro do projeto de testes, adicionar o seguinte pacote:

dotnet add package coverlet.msbuild

Para rodar os testes e exibir o relatorio de cobertura:

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=src/Test/lcov.info [path-do-seu-projeto-de-testes]
