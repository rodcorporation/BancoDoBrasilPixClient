using Newtonsoft.Json;

namespace BancoDoBrasilPixClient.Lib.Models
{
    public class CriarCobrancaRequestModel
    {
        [JsonProperty("calendario")]
        public CalendarioRequestModel Calendario { get; set; }

        [JsonProperty("devedor")]
        public DevedorRequestModel Devedor { get; set; }

        [JsonProperty("valor")]
        public ValorRequestModel Valor { get; set; }

        [JsonProperty("chave")]
        public string Chave { get; set; }

        [JsonProperty("solicitacaoPagador")]
        public string SolicitacaoPagador { get; set; }

        public class CalendarioRequestModel
        {
            [JsonProperty("expiracao")]
            public string Expiracao { get; set; }
        }

        public class DevedorRequestModel
        {
            [JsonProperty("cpf")]
            public string Cpf { get; set; }

            [JsonProperty("nome")]
            public string Nome { get; set; }
        }

        public class ValorRequestModel
        {
            [JsonProperty("original")]
            public decimal Original { get; set; }
        }
    }
}
