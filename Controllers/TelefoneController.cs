using apiAutenticacao.Models;
using apiAutenticacao.Models.DTO;
using apiAutenticacao.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace apiAutenticacao.Controllers; // Ajustado para o seu namespace

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TelefoneController : ControllerBase
{
    private readonly AppDbContext _context;

    public TelefoneController(AppDbContext context)
    {
        _context = context;
    }

    private int ObterUsuarioLogadoId()
    {
        var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(claimId!);
    }

    [HttpPost]
    public async Task<IActionResult> CadastrarTelefone([FromBody] TelefoneDTO dto)
    {
        var telefone = new Telefone
        {
            Numero = dto.Numero, // Ajustado para corresponder ao DTO corrigido
            Tipo = dto.Tipo,     // Faltava preencher o Tipo!
            UsuarioId = ObterUsuarioLogadoId()
        };

        _context.Telefones.Add(telefone);
        await _context.SaveChangesAsync();

        // Passamos a entidade criada para gerar a rota, mas devolvemos os dados em forma de objeto anônimo (ou poderia ser o ResponseDTO)
        return CreatedAtAction(nameof(ListarTelefones), new { id = telefone.Id }, new { telefone.Id, telefone.Numero, telefone.Tipo });
    }

    [HttpGet]
    public async Task<IActionResult> ListarTelefones()
    {
        var usuarioId = ObterUsuarioLogadoId();

        var telefones = await _context.Telefones
            .Where(t => t.UsuarioId == usuarioId)
            // Retornamos o DTO de Resposta em vez da entidade do banco!
            .Select(t => new TelefoneResponseDTO
            {
                Id = t.Id,
                Numero = t.Numero,
                Tipo = t.Tipo
            })
            .ToListAsync();

        return Ok(telefones);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarTelefone(int id)
    {
        var usuarioId = ObterUsuarioLogadoId();

        // Busca o telefone garantindo que pertence ao usuário logado
        var telefone = await _context.Telefones
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

        if (telefone == null)
            return NotFound("Telefone não encontrado ou não pertence a você.");

        _context.Telefones.Remove(telefone);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}