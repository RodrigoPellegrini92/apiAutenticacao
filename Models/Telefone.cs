using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apiAutenticacao.Models
{
    public class Telefone
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; // Celular, Fixo, Comercial

        // Chave Estrangeira
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }

}
