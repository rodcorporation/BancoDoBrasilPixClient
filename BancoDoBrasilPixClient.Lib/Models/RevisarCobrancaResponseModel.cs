using Newtonsoft.Json;
using System;

namespace BancoDoBrasilPixClient.Lib.Models
{
    public class RevisarCobrancaResponseModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("calendario")]
        public CalendarioResponseModel Calendario { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("txid")]
        public string TxId { get; set; }

        [JsonProperty("revisao")]
        public int Revisao { get; set; }

        [JsonProperty("devedor")]
        public DevedorResponseModel Devedor { get; set; }

        [JsonProperty("Valor")]
        public ValorResponseModel Valor { get; set; }

        [JsonProperty("chave")]
        public string Chave { get; set; }

        [JsonProperty("solicitacaoPagador")]
        public string SolicitacaoPagador { get; set; }

        public class CalendarioResponseModel
        {
            [JsonProperty("criacao")]
            public DateTime Criacao { get; set; }

            [JsonProperty("expiracao")]
            public string Expiracao { get; set; }
        }

        public class DevedorResponseModel
        {
            [JsonProperty("cpf")]
            public string Cpf { get; set; }

            [JsonProperty("nome")]
            public string Nome { get; set; }
        }

        public class ValorResponseModel
        {
            [JsonProperty("original")]
            public decimal Original { get; set; }
        }
    }
}
