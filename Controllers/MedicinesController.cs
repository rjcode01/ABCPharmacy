using ABCPharmacy.Models;
using ABCPharmacy.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCPharmacy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicinesController : ControllerBase
{
    private readonly JsonDataStore _store;
    public MedicinesController(JsonDataStore store) => _store = store;

    [HttpGet]
    public ActionResult<IEnumerable<Medicine>> GetAll([FromQuery] string? search)
    {
        var medicines = _store.GetMedicines();
        if (!string.IsNullOrWhiteSpace(search))
            medicines = medicines.Where(x => x.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        return Ok(medicines.OrderBy(x => x.FullName));
    }

    [HttpGet("{id:int}")]
    public ActionResult<Medicine> GetById(int id)
    {
        var medicine = _store.GetMedicines().FirstOrDefault(x => x.Id == id);
        return medicine is null ? NotFound() : Ok(medicine);
    }

    [HttpPost]
    public ActionResult<Medicine> Create(CreateMedicineRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Brand))
            return BadRequest("Medicine name and brand are required.");
        if (request.ExpiryDate.Date <= DateTime.Today)
            return BadRequest("Expiry date must be in the future.");
        if (request.Quantity < 0 || request.Price < 0)
            return BadRequest("Quantity and price cannot be negative.");

        var medicines = _store.GetMedicines();
        var medicine = new Medicine
        {
            Id = medicines.Count == 0 ? 1 : medicines.Max(x => x.Id) + 1,
            FullName = request.FullName.Trim(), Notes = request.Notes?.Trim() ?? string.Empty,
            ExpiryDate = request.ExpiryDate.Date, Quantity = request.Quantity,
            Price = decimal.Round(request.Price, 2), Brand = request.Brand.Trim()
        };
        medicines.Add(medicine);
        _store.SaveMedicines(medicines);
        return CreatedAtAction(nameof(GetById), new { id = medicine.Id }, medicine);
    }
}
