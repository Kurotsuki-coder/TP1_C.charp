using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIREST.Data;
using APIREST.Models;

namespace APIREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtudiantsController : ControllerBase
    {
        private readonly AppDbContext db;

        public EtudiantsController(AppDbContext context)
        {
            db = context;
        }

        // GET: api/etudiants
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var liste = await db.Etudiants.ToListAsync();
            return Ok(liste);
        }

        // GET: api/etudiants/1
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var etudiant = await db.Etudiants.FindAsync(id);
            if (etudiant == null)
                return NotFound();

            return Ok(etudiant);
        }

        // POST: api/etudiants
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Etudiant etudiant)
        {
            db.Etudiants.Add(etudiant);
            await db.SaveChangesAsync();
            return Ok(etudiant);
        }

        // PUT: api/etudiants/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Etudiant etudiant)
        {
            var e = await db.Etudiants.FindAsync(id);
            if (e == null)
                return NotFound();

            e.Nom = etudiant.Nom;
            e.Prenom = etudiant.Prenom;
            e.Email = etudiant.Email;

            await db.SaveChangesAsync();
            return Ok(e);
        }

        // DELETE: api/etudiants/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var e = await db.Etudiants.FindAsync(id);
            if (e == null)
                return NotFound();

            db.Etudiants.Remove(e);
            await db.SaveChangesAsync();
            return Ok();
        }
    }
}