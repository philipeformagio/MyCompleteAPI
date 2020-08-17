namespace DevIO.Api.Extensions
{
    public class AppSettings
    {
        public string Secret { get; set; } // chave de cryptografia
        public int ExpiracaoHoras { get; set; } // quantas horas vai levar até perder a validade
        public string Emissor { get; set; } // Quem emite/cria/emite o token(no caso essa aplicacao)
        public string ValidoEm { get; set; } // Em quais URLs esse token é valido
    }
}
