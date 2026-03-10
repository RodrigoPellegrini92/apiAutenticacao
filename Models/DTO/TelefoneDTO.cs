using Microsoft.OpenApi.Models;

namespace apiAutenticacao.Models.DTO
{
    public class TelefoneDTO
    {
        
        public string Tipo { get; set; } = string.Empty;
        public string Numero { get; internal set; }
    }
    public class TelefoneResponseDTO
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }
}
