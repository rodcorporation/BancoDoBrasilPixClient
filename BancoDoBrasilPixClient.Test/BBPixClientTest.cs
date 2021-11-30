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
        public void Autenticar()
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
                client.Autenticar(PixClient.Scopes.AllScopes);
            }
            catch
            {
                throwsException = true;
            }

            // assert
            Assert.IsFalse(throwsException);
        }

        [TestMethod]
        public async Task AutenticarAsync()
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
                await client.AutenticarAsync(PixClient.Scopes.AllScopes);
            }
            catch
            {
                throwsException = true;
            }

            // assert
            Assert.IsFalse(throwsException);
        }

        [TestMethod]
        public async Task ConsultarAsync()
        {
            // arrange
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);
            await client.AutenticarAsync(PixClient.Scopes.AllScopes);

            // act
            var response = await client.ConsultarPixAsync(Convert.ToDateTime($"{DateTime.Now.AddDays(-1):dd/MM/yyyy} 00:00:00"),
                                                          Convert.ToDateTime($"{DateTime.Now:dd/MM/yyyy} 23:59:59"),
                                                          1);

            // assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void Consultar()
        {
            // arrange
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);
            client.Autenticar(PixClient.Scopes.AllScopes);

            // act
            var response = client.ConsultarPix(Convert.ToDateTime($"{DateTime.Now.AddDays(-1):dd/MM/yyyy} 00:00:00"),
                                               Convert.ToDateTime($"{DateTime.Now:dd/MM/yyyy} 23:59:59"),
                                               1);

            // assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task ConsultarPorTxIdAsync()
        {
            // arrange
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);

            await client.AutenticarAsync(PixClient.Scopes.AllScopes);

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

            var responseCriarCobranca = await client.CriarCobrancaAsync(requestCriarCobranca);

            // act
            var response = await client.ConsultarPixPorTxIdAsync(responseCriarCobranca.TxId);

            // assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void ConsultarPorTxId()
        {
            // arrange
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);

            client.Autenticar(PixClient.Scopes.AllScopes);

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

            var responseCriarCobranca = client.CriarCobranca(requestCriarCobranca);

            // act
            var response = client.ConsultarPixPorTxId(responseCriarCobranca.TxId);

            // assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task CriarCobrancaAsync()
        {
            // arrange
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);
            await client.AutenticarAsync(PixClient.Scopes.AllScopes);
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
            var response = await client.CriarCobrancaAsync(request);

            // assert
            Assert.IsNotNull(response);
        }
        
        [TestMethod]
        public void CriarCobranca()
        {
            // arrange
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);

            client.Autenticar(PixClient.Scopes.AllScopes);
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
            var response = client.CriarCobranca(request);

            // assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task RevisarCobrancaAsync()
        {
            // arrange
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);

            await client.AutenticarAsync(PixClient.Scopes.AllScopes);

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

            var responseCriarCobranca = await client.CriarCobrancaAsync(requestCriarCobranca);

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
            var response = await client.RevisarCobrancaAsync(responseCriarCobranca.TxId,
                                                             requestRevisarCobranca);

            // assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void RevisarCobranca()
        {
            // arrange
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);

            client.Autenticar(PixClient.Scopes.AllScopes);

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

            var responseCriarCobranca = client.CriarCobranca(requestCriarCobranca);

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
            var response = client.RevisarCobranca(responseCriarCobranca.TxId,
                                                  requestRevisarCobranca);

            // assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task ConsultarPorEndToEndIdAsync()
        {
            // arrange
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);
            await client.AutenticarAsync(PixClient.Scopes.AllScopes);

            var responseConsultar = await client.ConsultarPixAsync(Convert.ToDateTime($"{DateTime.Now.AddDays(-1):dd/MM/yyyy} 00:00:00"),
                                                                   Convert.ToDateTime($"{DateTime.Now:dd/MM/yyyy} 23:59:59"),
                                                                   1);

            // act
            var response = await client.ConsultarPixPorEndToEndIdAsync(responseConsultar.Pix.FirstOrDefault().EndToEndId);

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
            await client.AutenticarAsync(PixClient.Scopes.AllScopes);

            var responseConsultar = client.ConsultarPix(Convert.ToDateTime($"{DateTime.Now.AddDays(-1):dd/MM/yyyy} 00:00:00"),
                                                        Convert.ToDateTime($"{DateTime.Now:dd/MM/yyyy} 23:59:59"),
                                                        1);

            // act
            var response = client.ConsultarPixPorEndToEndId(responseConsultar.Pix.FirstOrDefault().EndToEndId);

            // assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GerarQrCodeEmPng()
        {
            // arrange
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);

            await client.AutenticarAsync(PixClient.Scopes.AllScopes);

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

            var responseCriarCobranca = await client.CriarCobrancaAsync(requestCriarCobranca);

            // act   
            var imagem = PixClient.GerarQrCodeEmPng(responseCriarCobranca.TextoImagemQRcode);

            // assert
            // File.WriteAllBytes(@"D:\Temp\imagem.png", imagem);
            Assert.IsNotNull(imagem);
        }

        [TestMethod]
        public async Task GerarQrCodeEmBase64()
        {
            // arrange
            var client = new PixClient(EnvironmentType.Testing,
                                       _credentials.ClientId,
                                       _credentials.ClientSecret,
                                       _credentials.ApplicationKey);

            await client.AutenticarAsync(PixClient.Scopes.AllScopes);

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

            var responseCriarCobranca = await client.CriarCobrancaAsync(requestCriarCobranca);

            // act   
            var imagemEmBase64 = PixClient.GerarQrCodeEmBase64(responseCriarCobranca.TextoImagemQRcode);

            // assert
            Assert.IsNotNull(imagemEmBase64);
        }
    }
}