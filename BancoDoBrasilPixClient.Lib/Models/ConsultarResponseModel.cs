using Newtonsoft.Json;
using System;

namespace BancoDoBrasilPixClient.Lib.Models
{
    public class ConsultarResponseModel
    {
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

            [JsonProperty("horario")]
            public DateTime Horario { get; set; }

            [JsonProperty("pagador")] 
            public PagadorResponseModel Pagador { get; set; }

            [JsonProperty("devolucoes")]
            public DevolucoesResponseModel[] Devolucoes { get; set; }

            [JsonProperty("infoPagador")]
            public string InfoPagador { get; set; }
        }

        public class DevolucoesResponseModel
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("rtrId")]
            public string RtrId { get; set; }

            [JsonProperty("valor")]
            public decimal Valor { get; set; }

            [JsonProperty("horario")]
            public HorarioResponseModel Horario { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }

        public class HorarioResponseModel
        {
            [JsonProperty("solicitacao")]
            public DateTime Solicitacao { get; set; }
            
            [JsonProperty("liquidacao")]
            public DateTime Liquidacao { get; set; }
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

        [JsonProperty("parametros")]
        public ParametrosResponseModel Parametros { get; set; }

        public class ParametrosResponseModel
        {
            [JsonProperty("inicio")]
            public DateTime Inicio { get; set; }

            [JsonProperty("fim")]
            public DateTime Fim { get; set; }

            [JsonProperty("paginacao")]
            public PaginacaoResponseModel Paginacao { get; set; }
        }

        public class PaginacaoResponseModel
        {
            [JsonProperty("paginaAtual")]
            public int PaginaAtual { get; set; }

            [JsonProperty("itensPorPagina")]
            public int ItensPorPagina { get; set; }

            [JsonProperty("quantidadeDePaginas")]
            public int QuantidadeDePaginas { get; set; }

            [JsonProperty("quantidadeTotalDeItens")]
            public int QuantidadeTotaldeItens { get; set; }
        }
    }
}
