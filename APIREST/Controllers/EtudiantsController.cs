using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIREST.Data;
using APIREST.Models;
using Serilog;

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
            try
            {
                Log.Information("Récupération de la liste des étudiants");
                var liste = await db.Etudiants.ToListAsync();
                return Ok(liste);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la récupération des étudiants");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        // GET: api/etudiants/1
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                Log.Information("Récupération de l'étudiant avec l'id {Id}", id);
                var etudiant = await db.Etudiants.FindAsync(id);
                if (etudiant == null)
                {
                    Log.Warning("Étudiant avec l'id {Id} introuvable", id);
                    return NotFound();
                }

                return Ok(etudiant);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la récupération de l'étudiant {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        // POST: api/etudiants
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Etudiant etudiant)
        {
            try
            {
                db.Etudiants.Add(etudiant);
                await db.SaveChangesAsync();
                Log.Information("Étudiant ajouté avec succès : {Nom} {Prenom}", etudiant.Nom, etudiant.Prenom);
                return Ok(etudiant);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de l'ajout d'un étudiant");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        // PUT: api/etudiants/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Etudiant etudiant)
        {
            try
            {
                var e = await db.Etudiants.FindAsync(id);
                if (e == null)
                {
                    Log.Warning("Tentative de modification d'un étudiant introuvable, id {Id}", id);
                    return NotFound();
                }

                e.Nom = etudiant.Nom;
                e.Prenom = etudiant.Prenom;
                e.Email = etudiant.Email;

                await db.SaveChangesAsync();
                Log.Information("Étudiant {Id} modifié avec succès", id);
                return Ok(e);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la modification de l'étudiant {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        // DELETE: api/etudiants/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var e = await db.Etudiants.FindAsync(id);
                if (e == null)
                {
                    Log.Warning("Tentative de suppression d'un étudiant introuvable, id {Id}", id);
                    return NotFound();
                }

                db.Etudiants.Remove(e);
                await db.SaveChangesAsync();
                Log.Information("Étudiant {Id} supprimé avec succès", id);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la suppression de l'étudiant {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}