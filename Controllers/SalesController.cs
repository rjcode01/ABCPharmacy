using ABCPharmacy.Models;
using ABCPharmacy.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCPharmacy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly JsonDataStore _store;
    public SalesController(JsonDataStore store) => _store = store;

    [HttpGet]
    public ActionResult<IEnumerable<SaleRecord>> GetAll() => Ok(_store.GetSales().OrderByDescending(x => x.SoldAt));

    [HttpPost]
    public ActionResult<SaleRecord> Create(CreateSaleRequest request)
    {
        if (request.QuantitySold <= 0) return BadRequest("Quantity sold must be greater than zero.");

        var medicines = _store.GetMedicines();
        var medicine = medicines.FirstOrDefault(x => x.Id == request.MedicineId);
        if (medicine is null) return NotFound("Medicine not found.");
        if (medicine.ExpiryDate.Date <= DateTime.Today) return BadRequest("Expired medicine cannot be sold.");
        if (request.QuantitySold > medicine.Quantity) return BadRequest("Insufficient stock.");

        medicine.Quantity -= request.QuantitySold;
        var sales = _store.GetSales();
        var sale = new SaleRecord
        {
            Id = sales.Count == 0 ? 1 : sales.Max(x => x.Id) + 1,
            MedicineId = medicine.Id, MedicineName = medicine.FullName,
            QuantitySold = request.QuantitySold, UnitPrice = medicine.Price,
            TotalAmount = decimal.Round(medicine.Price * request.QuantitySold, 2), SoldAt = DateTime.Now
        };
        sales.Add(sale);
        _store.SaveMedicines(medicines);
        _store.SaveSales(sales);
        return Ok(sale);
    }
}
