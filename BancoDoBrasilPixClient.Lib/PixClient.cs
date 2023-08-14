using BancoDoBrasilPixClient.Lib.Models;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BancoDoBrasilPixClient.Lib
{
    /*
     *   Verificação de Recebimento
     *   
     *   A funcionalidade de verificação de recebimento permite que o usuário recebedor verifique o status QR Dinâmico e de transação identificada pelo txid
     *   preenchidas em transferências manuais e QR Estático, bem como o payload JSON vigente. Os possíveis status são:
     *   
     *   ATIVA: a cobrança está disponível, porém ainda não ocorreu pagamento;
     *   CONCLUIDA: a cobrança encontra-se paga. Não se pode alterar e nem remover uma cobrança cujo status esteja “CONCLUÍDA”;
     *   EM_PROCESSAMENTO: liquidação em processamento;
     *   NAO_REALIZADO: indica que a devolução não pode ser realizada em função de algum erro durante a liquidação, como por exemplo, saldo insuficiente.;
     *   DEVOLVIDO: cobrança com devolução realizada pelo Sistema de Pagamentos Instantâneos (SPI);
     *   REMOVIDA_PELO_USUARIO_RECEBEDOR: foi solicitada a remoção da cobrança; a critério do usuário;
     *   REMOVIDA_PELO_PSP: recebedor, por conta de algum critério, solicitou a remoção da cobrança.
     *   
     *   Adicionalmente ao status, a automação do usuário recebedor obterá acesso ao payload assinado que representa a cobrança, para fins de controle.
     */
    public sealed partial class PixClient
    {
        #region Attributes

        private readonly HttpStatusCode[] HttpStatusCode2xx;
        private readonly EnvironmentType _environmentType;
        private readonly string _applicationKey;
        private readonly string _clientSecret;
        private readonly string _clientId;

        // Autenticação
        private bool _autenticado;
        private string _jwt;

        #endregion

        #region Constructor

        public PixClient(EnvironmentType environmentType,
                         string clientId,
                         string clientSecret,
                         string applicationKey)
        {
            _environmentType = environmentType;

            _clientId = clientId;
            _clientSecret = clientSecret;
            _applicationKey = applicationKey;

            HttpStatusCode2xx = new HttpStatusCode[]
            {
                HttpStatusCode.OK,
                HttpStatusCode.Created
            };

            _autenticado = false;
        }

        #endregion

        #region Non-Async Methods

        public void Autenticar(string[] scopes)
        {
            var request = (HttpWebRequest)WebRequest.Create($"{EnvironmentTypeExtension.GetOAuthUrl(_environmentType)}/oauth/token");
            request.Method = "POST";
            request.UserAgent = "BancoDoBrasilPixClient";
            request.ContentType = "application/x-www-form-urlencoded";

            request.Headers.Add("Authorization", $"Basic {GetBasicAuthorization()}");

            byte[] byteArray = Encoding.UTF8.GetBytes($"grant_type=client_credentials&scope={string.Join(" ", scopes)}");

            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                // Se der erro.
                if (!HttpStatusCode2xx.Contains(response.StatusCode))
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string content = reader.ReadToEnd();
                        throw new Exception($"Ocorreu um erro ao tentar autenticar: {content}");
                    }
                }
                else
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseBody = reader.ReadToEnd();

                        var authenticateResponse = JsonConvert.DeserializeObject<AutenticarResponseModel>(responseBody);

                        _autenticado = true;
                        _jwt = authenticateResponse.AccessToken;
                    }
                }
            }
        }

        public ConsultarResponseModel ConsultarPix(DateTime inicio,
                                                   DateTime fim,
                                                   int paginaAtual)
        {
            if (!_autenticado)
                throw new Exception("Cliente não autenticado.");

            var request = (HttpWebRequest)WebRequest.Create($"{EnvironmentTypeExtension.GetPixUrl(_environmentType)}/pix/v1?gw-dev-app-key={_applicationKey}&inicio={inicio:yyyy-MM-ddTHH:mm:ss.00-03:00}&fim={fim:yyyy-MM-ddTHH:mm:ss.00-03:00}&paginaAtual={paginaAtual}");

            request.Method = "GET";
            request.UserAgent = "BancoDoBrasilPixClient";

            request.Headers.Add("Authorization", $"Bearer {_jwt}");

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                // Se der erro.
                if (!HttpStatusCode2xx.Contains(response.StatusCode))
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string content = reader.ReadToEnd();
                        throw new Exception($"Ocorreu um erro ao tentar consultar pix: {content}");
                    }
                }
                else
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseBody = reader.ReadToEnd();

                        var consultarResponse = JsonConvert.DeserializeObject<ConsultarResponseModel>(responseBody);

                        return consultarResponse;
                    }
                }
            }
        }

        public ConsultarPorTxIdResponseModel ConsultarPixPorTxId(string txId)
        {
            if (!_autenticado)
                throw new Exception("Cliente não autenticado.");

            var request = (HttpWebRequest)WebRequest.Create($"{EnvironmentTypeExtension.GetPixUrl(_environmentType)}/pix/v1/cob/{txId}?gw-dev-app-key={_applicationKey}");

            request.Method = "GET";
            request.UserAgent = "BancoDoBrasilPixClient";
            request.ContentType = "application/x-www-form-urlencoded";

            request.Headers.Add("Authorization", $"Bearer {_jwt}");

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                // Se der erro.
                if (!HttpStatusCode2xx.Contains(response.StatusCode))
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string content = reader.ReadToEnd();
                        throw new Exception($"Ocorreu um erro ao tentar consultar pix por txid: {content}");
                    }
                }
                else
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseBody = reader.ReadToEnd();

                        var consultarResponse = JsonConvert.DeserializeObject<ConsultarPorTxIdResponseModel>(responseBody);

                        return consultarResponse;
                    }
                }
            }
        }

        public CriarCobrancaResponseModel CriarCobranca(CriarCobrancaRequestModel requestModel)
        {
            if (!_autenticado)
                throw new Exception("Cliente não autenticado.");

            var request = (HttpWebRequest)WebRequest.Create($"{EnvironmentTypeExtension.GetPixUrl(_environmentType)}/pix/v1/cobqrcode/?gw-dev-app-key={_applicationKey}");
            request.Method = "PUT";
            request.UserAgent = "BancoDoBrasilPixClient";
            request.ContentType = "application/json";

            request.Headers.Add("Authorization", $"Bearer {_jwt}");

            byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestModel));

            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                // Se der erro.
                if (!HttpStatusCode2xx.Contains(response.StatusCode))
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string content = reader.ReadToEnd();
                        throw new Exception($"Ocorreu um erro ao tentar criar uma cobrança: {content}");
                    }
                }
                else
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseBody = reader.ReadToEnd();

                        var criarCobrancaResponse = JsonConvert.DeserializeObject<CriarCobrancaResponseModel>(responseBody);

                        return criarCobrancaResponse;
                    }
                }
            }
        }

        public RevisarCobrancaResponseModel RevisarCobranca(string txId,
                                                            RevisarCobrancaRequestModel requestModel)
        {
            if (!_autenticado)
                throw new Exception("Cliente não autenticado.");

            var request = (HttpWebRequest)WebRequest.Create($"{EnvironmentTypeExtension.GetPixUrl(_environmentType)}/pix/v1/cob/{txId}?gw-dev-app-key={_applicationKey}");
            request.Method = "PATCH";
            request.UserAgent = "BancoDoBrasilPixClient";
            request.ContentType = "application/json";

            request.Headers.Add("Authorization", $"Bearer {_jwt}");

            byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestModel));

            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                // Se der erro.
                if (!HttpStatusCode2xx.Contains(response.StatusCode))
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string content = reader.ReadToEnd();
                        throw new Exception($"Ocorreu um erro ao tentar revisar uma cobrança: {content}");
                    }
                }
                else
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseBody = reader.ReadToEnd();

                        var revisarCobrancaResponse = JsonConvert.DeserializeObject<RevisarCobrancaResponseModel>(responseBody);

                        return revisarCobrancaResponse;
                    }
                }
            }
        }

        public ConsultarPorEndToEndIdResponseModel ConsultarPixPorEndToEndId(string endToEndId)
        {
            if (!_autenticado)
                throw new Exception("Cliente não autenticado.");

            var request = (HttpWebRequest)WebRequest.Create($"{EnvironmentTypeExtension.GetPixUrl(_environmentType)}/pix/v1/pix/{endToEndId}?gw-dev-app-key={_applicationKey}");

            request.Method = "GET";
            request.UserAgent = "BancoDoBrasilPixClient";
            request.ContentType = "application/x-www-form-urlencoded";

            request.Headers.Add("Authorization", $"Bearer {_jwt}");

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                // Se der erro.
                if (!HttpStatusCode2xx.Contains(response.StatusCode))
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string content = reader.ReadToEnd();
                        throw new Exception($"Ocorreu um erro ao tentar consultar o pix por end-to-end-id: {content}");
                    }
                }
                else
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseBody = reader.ReadToEnd();

                        var consultarResponse = JsonConvert.DeserializeObject<ConsultarPorEndToEndIdResponseModel>(responseBody);

                        return consultarResponse;
                    }
                }
            }
        }

        #endregion

        #region Async Methods

        public async Task AutenticarAsync(string[] scopes)
        {
            var basicAuthorization = GetBasicAuthorization();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentTypeExtension.GetOAuthUrl(_environmentType));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthorization);

                var body = new StringContent($"grant_type=client_credentials&scope={string.Join(" ", scopes)}",
                                             Encoding.UTF8,
                                             "application/x-www-form-urlencoded");

                var responseMessage = await client.PostAsync("oauth/token", body);

                if (!HttpStatusCode2xx.Contains(responseMessage.StatusCode))
                {
                    var content = await responseMessage.Content.ReadAsStringAsync();
                    throw new Exception($"Ocorreu um erro ao tentar Autenticar: {content}");
                }

                var responseBody = await responseMessage.Content.ReadAsStringAsync();

                var authenticateResponse = JsonConvert.DeserializeObject<AutenticarResponseModel>(responseBody);

                _autenticado = true;
                _jwt = authenticateResponse.AccessToken;
            }
        }

        public async Task<ConsultarResponseModel> ConsultarPixAsync(DateTime inicio,
                                                                    DateTime fim,
                                                                    int paginaAtual)
        {
            if (!_autenticado)
                throw new Exception("Cliente não autenticado.");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentTypeExtension.GetPixUrl(_environmentType));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);

                var uriRecurso = $"/pix/v1?gw-dev-app-key={_applicationKey}&inicio={inicio:yyyy-MM-ddTHH:mm:ss.00-03:00}&fim={fim:yyyy-MM-ddTHH:mm:ss.00-03:00}&paginaAtual={paginaAtual}";

                var responseMessage = await client.GetAsync(uriRecurso);

                if (!HttpStatusCode2xx.Contains(responseMessage.StatusCode))
                {
                    var content = await responseMessage.Content.ReadAsStringAsync();
                    throw new Exception($"Ocorreu um erro ao tentar Consultar Pix: {content}");
                }

                var responseBody = await responseMessage.Content.ReadAsStringAsync();

                var consultarResponse = JsonConvert.DeserializeObject<ConsultarResponseModel>(responseBody);

                return consultarResponse;
            }
        }

        public async Task<ConsultarPorTxIdResponseModel> ConsultarPixPorTxIdAsync(string txId)
        {
            if (!_autenticado)
                throw new Exception("Cliente não autenticado.");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentTypeExtension.GetPixUrl(_environmentType));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);

                var uriRecurso = $"/pix/v1/cob/{txId}?gw-dev-app-key={_applicationKey}";

                var responseMessage = await client.GetAsync(uriRecurso);

                if (!HttpStatusCode2xx.Contains(responseMessage.StatusCode))
                {
                    var content = await responseMessage.Content.ReadAsStringAsync();
                    throw new Exception($"Ocorreu um erro ao tentar Consultar Pix por TxId: {content}");
                }

                var responseBody = await responseMessage.Content.ReadAsStringAsync();

                var consultarPorTxIdResponse = JsonConvert.DeserializeObject<ConsultarPorTxIdResponseModel>(responseBody);

                return consultarPorTxIdResponse;
            }
        }

        public async Task<CriarCobrancaResponseModel> CriarCobrancaAsync(CriarCobrancaRequestModel requestModel)
        {
            if (!_autenticado)
                throw new Exception("Cliente não autenticado.");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentTypeExtension.GetPixUrl(_environmentType));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);

                var uriRecurso = $"/pix/v1/cobqrcode/?gw-dev-app-key={_applicationKey}";

                var body = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");

                var responseMessage = await client.PutAsync(uriRecurso, body);

                if (!HttpStatusCode2xx.Contains(responseMessage.StatusCode))
                {
                    var content = await responseMessage.Content.ReadAsStringAsync();
                    throw new Exception($"Ocorreu um erro ao tentar criar cobrança: {content}");
                }

                var responseBody = await responseMessage.Content.ReadAsStringAsync();

                var criarCobrancaResponse = JsonConvert.DeserializeObject<CriarCobrancaResponseModel>(responseBody);

                return criarCobrancaResponse;
            }
        }

        public async Task<RevisarCobrancaResponseModel> RevisarCobrancaAsync(string txId,
                                                                             RevisarCobrancaRequestModel requestModel)
        {
            if (!_autenticado)
                throw new Exception("Cliente não autenticado.");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentTypeExtension.GetPixUrl(_environmentType));

                var uriRecurso = $"/pix/v1/cob/{txId}?gw-dev-app-key={_applicationKey}";

                var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), uriRecurso);
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");

                var responseMessage = await client.SendAsync(requestMessage);

                if (!HttpStatusCode2xx.Contains(responseMessage.StatusCode))
                {
                    var content = await responseMessage.Content.ReadAsStringAsync();
                    throw new Exception($"Ocorreu um erro ao tentar Revisar Cobrança: {content}");
                }

                var responseBody = await responseMessage.Content.ReadAsStringAsync();

                var revisarCobrancaResponse = JsonConvert.DeserializeObject<RevisarCobrancaResponseModel>(responseBody);

                return revisarCobrancaResponse;
            }
        }

        public async Task<ConsultarPorEndToEndIdResponseModel> ConsultarPixPorEndToEndIdAsync(string endToEndId)
        {
            if (!_autenticado)
                throw new Exception("Cliente não autenticado.");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentTypeExtension.GetPixUrl(_environmentType));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);

                var uriRecurso = $"/pix/v1/pix/{endToEndId}?gw-dev-app-key={_applicationKey}";

                var responseMessage = await client.GetAsync(uriRecurso);

                if (!HttpStatusCode2xx.Contains(responseMessage.StatusCode))
                {
                    var content = await responseMessage.Content.ReadAsStringAsync();
                    throw new Exception($"Ocorreu um erro ao tentar Consultar Pix por EndToEndId: {content}");
                }

                var responseBody = await responseMessage.Content.ReadAsStringAsync();

                var consultarPorEndToEndIdResponse = JsonConvert.DeserializeObject<ConsultarPorEndToEndIdResponseModel>(responseBody);

                return consultarPorEndToEndIdResponse;
            }
        }

        #endregion

        #region Métodos Estáticos

        public static string GerarQrCodeEmBase64(string textoImagemQrCode)
        {
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerator.CreateQrCode(textoImagemQrCode, QRCodeGenerator.ECCLevel.Q);
            var qrCodeBase64 = new Base64QRCode(qrCodeData);
            return $"data:image/jpg;base64,{qrCodeBase64.GetGraphic(20, Color.Black, Color.White, true, Base64QRCode.ImageType.Jpeg)}";
        }

        public static byte[] GerarQrCodeEmPng(string textoImagemQrCode)
        {
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerator.CreateQrCode(textoImagemQrCode, QRCodeGenerator.ECCLevel.Q);
            var qrCodePng = new PngByteQRCode(qrCodeData);
            return qrCodePng.GetGraphic(20);
        }

        #endregion

        #region Private Methods

        private string GetBasicAuthorization()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
        }

        #endregion

        #region Export Class

        public static class Scopes
        {
            public static string CobRead => "cob.read";
            public static string CobWrite => "cob.write";
            public static string PixRead => "pix.read";
            public static string PixWrite => "pix.write";

            public static string[] AllScopes => new string[]
            {
            CobRead,
            CobWrite,
            PixWrite,
            PixRead
            };

            public static string[] AllCobScopes => new string[]
            {
           CobWrite,
           CobRead
            };

            public static string[] AllPixScopes => new string[]
            {
           CobWrite,
           CobRead
            };

            public static string[] AllReadScopes => new string[]
            {
            CobRead,
            PixRead
            };

            public static string[] AllWriteScopes => new string[]
            {
            CobRead,
            PixRead
            };
        }

        #endregion
    }
}
