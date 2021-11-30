using Newtonsoft.Json;
using System;

namespace BancoDoBrasilPixClient.Lib.Models
{
    public class ConsultarPorTxIdResponseModel
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

        [JsonProperty("valor")]
        public ValorResponseModel Valor { get; set; }

        [JsonProperty("chave")]
        public string Chave { get; set; }

        [JsonProperty("solicitacaoPagador")]
        public string SolicitacaoPagador { get; set; }

        [JsonProperty("pix")]
        public PixResponseModel[] Pix { get; set; }
        
        public class PixResponseModel
        {
            [JsonProperty("endToEndId")]
            public string EndToEndId { get; set; }

            [JsonProperty("txid")]
            public string TxId { get; set; }

            [JsonProperty("valor")]
            public decimal Valor { get; set; }

            [JsonProperty("infoPagador")]
            public string InfoPagador { get; set; }

            [JsonProperty("horario")]
            public DateTime Horario { get; set; }

            [JsonProperty("pagador")]
            public PagadorResponseModel Pagador { get; set; }
        }

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

            [JsonProperty("cnpj")]
            public string Cnpj { get; set; }

            [JsonProperty("nome")]
            public string Nome { get; set; }
        }

        public class PagadorResponseModel
        {
            [JsonProperty("cpf")]
            public string Cpf { get; set; }

            [JsonProperty("cnpj")]
            public string Cnpj { get; set; }

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
