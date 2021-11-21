using BancoDoBrasilPixClient.Lib;
using BancoDoBrasilPixClient.Lib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BancoDoBrasilPixClient.Test
{
    [TestClass]
    public class PixClientTest
    {
        private readonly Credentials _credentials;

        public PixClientTest()
        {
            _credentials = JsonConvert.DeserializeObject<Credentials>(File.ReadAllText(@"..\..\credentials.json"));
        }

        [TestMethod]
        public async Task Autenticar()
        {
            // arrange
            var throwsException = false;
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);

            // act
            try
            {
                await client.Autenticar(PixClient.Scopes.AllScopes);
            }
            catch
            {
                throwsException = true;
            }

            // assert
            Assert.IsFalse(throwsException);
        }

        [TestMethod]
        public async Task Consultar()
        {
            // arrange
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);
            await client.Autenticar(PixClient.Scopes.AllScopes);

            // act
            var response = await client.ConsultarPix(Convert.ToDateTime($"{DateTime.Now:dd/MM/yyyy} 00:00:00"),
                                                     Convert.ToDateTime($"{DateTime.Now:dd/MM/yyyy} 23:59:59"),
                                                     1);

            // assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task ConsultarPorTxId()
        {
            // arrange
            var txId = $"{DateTime.Now:yyyyMMddHHmmss}59ac32b2a8b840";
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);

            await client.Autenticar(PixClient.Scopes.AllScopes);

            var requestCriarCobranca = new CriarCobrancaRequestModel()
            {
                Calendario = new CriarCobrancaRequestModel.CalendarioRequestModel()
                {
                    Expiracao = "3600"
                },
                Devedor = new CriarCobrancaRequestModel.DevedorRequestModel
                {
                    Cpf = "12345678909",
                    Nome = "Yuri Roberto Jorge Barros"
                },
                Valor = new CriarCobrancaRequestModel.ValorRequestModel()
                {
                    Original = 57.15m
                },
                Chave = "7f6844d0-de89-47e5-9ef7-e0a35a681615",
                SolicitacaoPagador = "Pagamento pedido Nº 4321"
            };

            await client.CriarCobranca(txId,
                                       requestCriarCobranca);

            // act
            var response = await client.ConsultarPixPorTxId(txId);

            // assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task CriarCobranca()
        {
            // arrange
            var txId = $"{DateTime.Now:yyyyMMddHHmmss}5fc1e5cfe7e94c";
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);
            await client.Autenticar(PixClient.Scopes.AllScopes);
            var request = new CriarCobrancaRequestModel()
            {
                Calendario = new CriarCobrancaRequestModel.CalendarioRequestModel()
                {
                    Expiracao = "3600"
                },
                Devedor = new CriarCobrancaRequestModel.DevedorRequestModel
                {
                    Cpf = "12345678909",
                    Nome = "Evelyn Regina Caldeira"
                },
                Valor = new CriarCobrancaRequestModel.ValorRequestModel()
                {
                    Original = 29.33m
                },
                Chave = "7f6844d0-de89-47e5-9ef7-e0a35a681615",
                SolicitacaoPagador = "Pagamento pedido Nº 1234"
            };

            // act
            var response = await client.CriarCobranca(txId,
                                                      request);

            // assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task RevisarCobranca()
        {
            // arrange
            var txId = $"{DateTime.Now:yyyyMMddHHmmss}fbad743c4ed94a";
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);

            await client.Autenticar(PixClient.Scopes.AllScopes);

            var requestCriarCobranca = new CriarCobrancaRequestModel()
            {
                Calendario = new CriarCobrancaRequestModel.CalendarioRequestModel()
                {
                    Expiracao = "3600"
                },
                Devedor = new CriarCobrancaRequestModel.DevedorRequestModel
                {
                    Cpf = "12345678909",
                    Nome = "Yuri Roberto Jorge Barros"
                },
                Valor = new CriarCobrancaRequestModel.ValorRequestModel()
                {
                    Original = 57.15m
                },
                Chave = "7f6844d0-de89-47e5-9ef7-e0a35a681615",
                SolicitacaoPagador = "Pagamento pedido Nº 4321"
            };

            await client.CriarCobranca(txId,
                                       requestCriarCobranca);

            var requestRevisarCobranca = new RevisarCobrancaRequestModel()
            {
                Calendario = new RevisarCobrancaRequestModel.CalendarioRequestModel()
                {
                    Expiracao = "3600"
                },
                Devedor = new RevisarCobrancaRequestModel.DevedorRequestModel
                {
                    Cpf = "83802670604",
                    Nome = "Yuri Roberto Jorge Barros"
                },
                Valor = new RevisarCobrancaRequestModel.ValorRequestModel()
                {
                    Original = 57.15m
                },
                Chave = "7f6844d0-de89-47e5-9ef7-e0a35a681615",
                SolicitacaoPagador = $"Pagamento pedido Nº 4321 com vencimento para {DateTime.Now.AddDays(2):dd/MM/yyyy}"
            };

            // act
            var response = await client.RevisarCobranca(txId,
                                                        requestRevisarCobranca);

            // assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task ConsultarPorEndToEndId()
        {
            // arrange
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);
            await client.Autenticar(PixClient.Scopes.AllScopes);

            var responseConsultar = await client.ConsultarPix(Convert.ToDateTime($"{DateTime.Now:dd/MM/yyyy} 00:00:00"),
                                                              Convert.ToDateTime($"{DateTime.Now:dd/MM/yyyy} 23:59:59"),
                                                              1);

            // act
            var response = await client.ConsultarPixPorEndToEndId(responseConsultar.Pix.FirstOrDefault().EndToEndId);

            // assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GerarQrCodeEmPng()
        {
            // arrange
            var txId = $"{DateTime.Now:yyyyMMddHHmmss}583f081b913c40";
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);

            await client.Autenticar(PixClient.Scopes.AllScopes);

            var requestCriarCobranca = new CriarCobrancaRequestModel()
            {
                Calendario = new CriarCobrancaRequestModel.CalendarioRequestModel()
                {
                    Expiracao = "3600"
                },
                Devedor = new CriarCobrancaRequestModel.DevedorRequestModel
                {
                    Cpf = "12345678909",
                    Nome = "João Gil Berto"
                },
                Valor = new CriarCobrancaRequestModel.ValorRequestModel()
                {
                    Original = 12.56m
                },
                Chave = "7f6844d0-de89-47e5-9ef7-e0a35a681615",
                SolicitacaoPagador = "Pagamento pedido Nº 4321"
            };

            var responseCriarCobranca = await client.CriarCobranca(txId,
                                                                   requestCriarCobranca);

            // act   
            var imagem = client.GerarQrCodeEmPng(responseCriarCobranca.TextoImagemQRcode);

            // assert
            // File.WriteAllBytes(@"D:\Temp\imagem.png", imagem);
            Assert.IsNotNull(imagem);
        }

        [TestMethod]
        public async Task GerarQrCodeEmBase64()
        {
            // arrange
            var txId = $"{DateTime.Now:yyyyMMddHHmmss}583f081b913c40";
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);

            await client.Autenticar(PixClient.Scopes.AllScopes);

            var requestCriarCobranca = new CriarCobrancaRequestModel()
            {
                Calendario = new CriarCobrancaRequestModel.CalendarioRequestModel()
                {
                    Expiracao = "3600"
                },
                Devedor = new CriarCobrancaRequestModel.DevedorRequestModel
                {
                    Cpf = "12345678909",
                    Nome = "João Gil Berto"
                },
                Valor = new CriarCobrancaRequestModel.ValorRequestModel()
                {
                    Original = 12.56m
                },
                Chave = "7f6844d0-de89-47e5-9ef7-e0a35a681615",
                SolicitacaoPagador = "Pagamento pedido Nº 4321"
            };

            var responseCriarCobranca = await client.CriarCobranca(txId,
                                                                   requestCriarCobranca);

            // act   
            var imagemEmBase64 = client.GerarQrCodeEmBase64(responseCriarCobranca.TextoImagemQRcode);

            // assert
            Assert.IsNotNull(imagemEmBase64);
        }
    }
}