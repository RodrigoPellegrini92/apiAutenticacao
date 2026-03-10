namespace apiAutenticacao.Models.DTO
{
    public class TarefaResponseDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public bool Concluida { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}