using apiAutenticacao.Data;
using apiAutenticacao.Models;
using apiAutenticacao.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // CORREÇÃO 1: Faltava este using para ler as Claims do Token

namespace apiAutenticacao.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TarefaController(AppDbContext context)
        {
            _context = context;
        }

        private int ObterUsuarioLogadoId()
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claimId!);
        }

        [HttpPost]
        // CORREÇÃO 2: Aqui o ideal é receber um DTO de requisição (TarefaDTO) e não o de Resposta
        public async Task<IActionResult> CadastrarTarefa([FromBody] TarefaDTO dto)
        {
            var tarefa = new Tarefa
            {
                Titulo = dto.Titulo,         // Adicionado para cumprir o requisito
                Descricao = dto.Descricao,
                Concluida = false,           // Ao criar, a tarefa sempre começa como não concluída
                DataCriacao = DateTime.Now,  // O sistema preenche a data sozinho
                UsuarioId = ObterUsuarioLogadoId()
            };

            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ListarTarefas), new { id = tarefa.Id }, new
            {
                tarefa.Id,
                tarefa.Titulo,
                tarefa.Descricao,
                tarefa.Concluida,
                tarefa.DataCriacao
            });
        }

        [HttpGet]
        public async Task<IActionResult> ListarTarefas([FromQuery] bool? concluida) // O sinal de interrogação (?) significa que o parâmetro é opcional
        {
            var usuarioId = ObterUsuarioLogadoId();

            // 1. Começamos pegando as tarefas do usuário logado
            var query = _context.Tarefas.Where(t => t.UsuarioId == usuarioId);

            // 2. Se o usuário passou o filtro na URL (true ou false), nós aplicamos o filtro
            if (concluida.HasValue)
            {
                query = query.Where(t => t.Concluida == concluida.Value);
            }

            // 3. Executamos a busca no banco e transformamos em DTO
            var tarefas = await query
                .Select(t => new TarefaResponseDTO
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    Descricao = t.Descricao,
                    Concluida = t.Concluida,
                    DataCriacao = t.DataCriacao
                })
                .ToListAsync();

            return Ok(tarefas);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarTarefa(int id)
        {
            var usuarioId = ObterUsuarioLogadoId();

            var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (tarefa == null)
            {
                return NotFound(new { Erro = true, Mensagem = "Tarefa não encontrada ou não pertence a você." });
            }

            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarStatusTarefa(int id, [FromBody] bool concluida)
        {
            var usuarioId = ObterUsuarioLogadoId();

            // Busca a tarefa garantindo que ela pertence ao usuário logado
            var tarefa = await _context.Tarefas
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (tarefa == null)
            {
                return NotFound(new { Erro = true, Mensagem = "Tarefa não encontrada ou não pertence a você." });
            }

            // Atualiza apenas o status
            tarefa.Concluida = concluida;

            await _context.SaveChangesAsync();

            return Ok(new { Erro = false, Mensagem = "Status da tarefa atualizado com sucesso!", Tarefa = tarefa });
        }
    }
}