using FuncionalApiDesenvolvedorIO.Data;
using FuncionalApiDesenvolvedorIO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuncionalApiDesenvolvedorIO.Controllers;


[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/v1/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly ApiDbContext _context;
    private bool ProductExist(Guid id)
    {
        return _context.Produtos?.Any(e => e.Id == id) ?? false;
    }
    public ProdutosController(ApiDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<Produto>>> Get()
    {
        if (_context.Produtos == null)
        {
            return NotFound();
        }
        return await _context.Produtos.ToListAsync();
    }

    [HttpGet("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<Produto>> GetById(Guid id)
    {
        if (_context.Produtos == null)
        {
            return NotFound();
        }
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null)
        {
            return NotFound();
        }
        return produto;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<Produto>> Post([FromBody] Produto produto)
    {
        produto.Id = Guid.NewGuid();
        if (_context.Produtos == null) return Problem("Erro interno! Contate o suporte!");
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> PutById(Guid id, [FromBody] Produto produto)
    {
        if (id != produto.Id)
        {
            return BadRequest("O ID na rota não corresponde ao ID do produto.");
        }

        var produtoExistente = await _context.Produtos.FindAsync(id);
        if (produtoExistente == null)
        {
            return NotFound();
        }
        try
        {
            _context.Entry(produtoExistente).CurrentValues.SetValues(produto);
            await _context.SaveChangesAsync();

        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExist(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }


        return NoContent();
    }


    [HttpDelete("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<Produto>> DeleteById(Guid id)
    {
        if (_context.Produtos == null) return NotFound();
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null)
        {
            return NotFound();
        }
        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
