using KeyRemappingService.Data;
using KeyRemappingService.Models;
using KeyRemappingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyRemappingService.Controllers
{
    [ApiController]
    [Route("api/keyboards/{keyboardId}/mappings")]
    public class KeyMappingsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public KeyMappingsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult SaveMappings(int keyboardId, MappingRequest request)
        {
            // Vérification du clavier
            var keyboard = _db.Keyboards.FirstOrDefault(k => k.Id == keyboardId);

            if (keyboard == null)
            {
                return NotFound("Clavier introuvable.");
            }

            // Vérifier qu'une même touche source n'est pas envoyée plusieurs fois
            var duplicateSource = request.Mappings
                .GroupBy(m => m.From)
                .FirstOrDefault(group => group.Count() > 1);

            if (duplicateSource != null)
            {
                return BadRequest(
                    $"Le code HID source {duplicateSource.Key} apparaît plusieurs fois.");
            }

            // Vérifier chaque mapping pour s'assurer que les codes HID sont valides
            foreach (var mapping in request.Mappings)
            {
                if (!HidKeyCodes.Keys.ContainsKey(mapping.From))
                {
                    return BadRequest(
                        $"Code HID source invalide : {mapping.From}");
                }

                if (!HidKeyCodes.Keys.ContainsKey(mapping.To))
                {
                    return BadRequest(
                        $"Code HID cible invalide: {mapping.To}");
                }
            }

            // Mise à jour des mappings
            foreach (var mapping in request.Mappings)
            {
                var existingMapping = _db.KeyMappings
                    .FirstOrDefault(m =>
                        m.KeyboardId == keyboardId &&
                        m.SourceKeyCode == mapping.From);

                if (mapping.From == mapping.To)
                {
                    if (existingMapping != null)
                    {
                        _db.KeyMappings.Remove(existingMapping);
                    }

                    continue;
                }


                if (existingMapping != null)
                {
                    // La touche avait déjà un mapping :
                    // on modifie uniquement sa destination.
                    existingMapping.TargetKeyCode = mapping.To;
                }
                else
                {
                    // Aucun mapping n'existe encore pour cette touche :
                    // on en crée un.
                    var newMapping = new KeyMapping
                    {
                        KeyboardId = keyboardId,
                        SourceKeyCode = mapping.From,
                        TargetKeyCode = mapping.To
                    };

                    _db.KeyMappings.Add(newMapping);
                }
            }

            _db.SaveChanges();

            return NoContent();
        }

        public IActionResult GetMappings(int keyboardId)
        {
            // Vérifier que le clavier existe
            var keyboard = _db.Keyboards.FirstOrDefault(k => k.Id == keyboardId);

            if (keyboard == null)
            {
                return NotFound("Clavier introuvable.");
            }

            // Récupère les remappings enregistrés dans SQLite
            var savedMappings = _db.KeyMappings
                .Where(m => m.KeyboardId == keyboardId)
                .ToList();

            var result = HidKeyCodes.Keys.Select(key =>
            {
                // Cherche si cette touche a été remappée
                var savedMapping = savedMappings
                    .FirstOrDefault(m => m.SourceKeyCode == key.Key);

                // Si aucun mapping n'existe, la touche reste elle-même
                var targetCode = savedMapping?.TargetKeyCode ?? key.Key;

                return new
                {
                    sourceKey = key.Value,
                    sourceKeyCode = key.Key,
                    targetKey = HidKeyCodes.Keys[targetCode],
                    targetKeyCode = targetCode
                };
            });

            return Ok(result);
        }
    }
}