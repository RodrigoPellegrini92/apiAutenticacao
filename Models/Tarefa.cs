namespace apiAutenticacao.Models
{
    public class Tarefa
    {
        public int Id { get; set; } // PK da tarefa
        public string Titulo { get; set; } = string.Empty; // Título da tarefa
        public string Descricao { get; set; } = string.Empty; // Descrição
        public bool Concluida { get; set; } = false; // Status de conclusão
        public DateTime DataCriacao { get; set; } = DateTime.Now; // Data de criação

        // Relacionamento com Usuário
        public int UsuarioId { get; set; } // Chave Estrangeira
        public Usuario Usuario { get; set; } = null!; // Propriedade de Navegação
    }
}