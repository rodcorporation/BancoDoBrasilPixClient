using Newtonsoft.Json;

namespace BancoDoBrasilPixClient.Lib.Models
{
    public class RevisarCobrancaRequestModel
    {
        [JsonProperty("calendario")]
        public CalendarioRequestModel Calendario { get; set; }

        [JsonProperty("revisao")]
        public DevedorRequestModel Devedor { get; set; }

        [JsonProperty("Valor")]
        public ValorRequestModel Valor { get; set; }

        [JsonProperty("chave")]
        public string Chave { get; set; }

        [JsonProperty("solicitacaoPagador")]
        public string SolicitacaoPagador { get; set; }

        [JsonProperty("infoAdicionais")]
        public InfoAdicionaisRequestModel[] InfoAdicionais { get; set; }

        public class CalendarioRequestModel
        {
            [JsonProperty("expiracao")]
            public string Expiracao { get; set; }
        }

        public class DevedorRequestModel
        {
            [JsonProperty("cpf")]
            public string Cpf { get; set; }

            [JsonProperty("cnpj")]
            public string Cnpj { get; set; }

            [JsonProperty("nome")]
            public string Nome { get; set; }
        }

        public class ValorRequestModel
        {
            [JsonProperty("original")]
            public decimal Original { get; set; }
        }

        public class InfoAdicionaisRequestModel
        {
            [JsonProperty("nome")]
            public string Nome { get; set; }
            
            [JsonProperty("valor")]
            public string Valor { get; set; }
        }
    }
}
