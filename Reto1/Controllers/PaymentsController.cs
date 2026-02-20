using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reto1.Data;
using Reto1.Models;

namespace Reto1.Controllers;

public class PaymentsController : Controller
{
    private readonly AppDbContext _db;

    public PaymentsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /Payments
    public async Task<IActionResult> Index()
    {
        var payments = await _db.Payments
            .AsNoTracking()
            .OrderByDescending(p => p.PaidOn)
            .ThenByDescending(p => p.Id)
            .ToListAsync();

        return View(payments);
    }

    // GET: /Payments/Details/
    public async Task<IActionResult> Details(int id)
    {
        var payment = await _db.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment is null) return NotFound();
        return View(payment);
    }

    // GET: /Payments/Create
    public IActionResult Create()
    {
        var vm = new PaymentFormVm
        {
            PaidOn = DateTime.Today
        };

        return View(vm);
    }

    // POST: /Payments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentFormVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var payment = new Payment
        {
            PaidOn = vm.PaidOn.Date,
            Amount = vm.Amount,
            Merchant = vm.Merchant,
            Category = vm.Category,
            Notes = vm.Notes,
            CreatedAtUtc = DateTime.UtcNow
        };

        payment.RecalculateNormalized();

        // Check antes de guardar en la DB
        var exists = await _db.Payments.AnyAsync(p =>
            p.PaidOn == payment.PaidOn &&
            p.Amount == payment.Amount &&
            p.MerchantNormalized == payment.MerchantNormalized);

        if (exists)
        {
            ModelState.AddModelError(string.Empty, "Duplicate payment detected (same date, amount and merchant).");
            return View(vm);
        }

        _db.Payments.Add(payment);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            
            ModelState.AddModelError(string.Empty, "Duplicate payment detected (database constraint).");
            return View(vm);
        }

        TempData["Msg"] = "Payment created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Payments/Edit/
    public async Task<IActionResult> Edit(int id)
    {
        var payment = await _db.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (payment is null) return NotFound();

        var vm = new PaymentFormVm
        {
            PaidOn = payment.PaidOn,
            Amount = payment.Amount,
            Merchant = payment.Merchant,
            Category = payment.Category,
            Notes = payment.Notes
        };

        ViewBag.PaymentId = id; 
        return View(vm);
    }

    // POST: /Payments/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PaymentFormVm vm)
    {
        var payment = await _db.Payments.FirstOrDefaultAsync(p => p.Id == id);
        if (payment is null) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.PaymentId = id;
            return View(vm);
        }

        payment.PaidOn = vm.PaidOn.Date;
        payment.Amount = vm.Amount;
        payment.Merchant = vm.Merchant;
        payment.Category = vm.Category;
        payment.Notes = vm.Notes;

        payment.RecalculateNormalized();

        // Duplicado excluyendo el mismo Id
        var exists = await _db.Payments.AnyAsync(p =>
            p.Id != id &&
            p.PaidOn == payment.PaidOn &&
            p.Amount == payment.Amount &&
            p.MerchantNormalized == payment.MerchantNormalized);

        if (exists)
        {
            ModelState.AddModelError(string.Empty, "Duplicate payment detected (same date, amount and merchant).");
            ViewBag.PaymentId = id;
            return View(vm);
        }

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Duplicate payment detected (database constraint).");
            ViewBag.PaymentId = id;
            return View(vm);
        }

        TempData["Msg"] = "Payment updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Payments/Delete/
    public async Task<IActionResult> Delete(int id)
    {
        var payment = await _db.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment is null) return NotFound();
        return View(payment);
    }

    // POST: /Payments/Delete/
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var payment = await _db.Payments.FirstOrDefaultAsync(p => p.Id == id);
        if (payment is null) return NotFound();

        _db.Payments.Remove(payment);
        await _db.SaveChangesAsync();

        TempData["Msg"] = "Payment deleted.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Payments/BulkDelete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkDelete(int[] selectedIds)
    {
        if (selectedIds is null || selectedIds.Length == 0)
        {
            TempData["Msg"] = "No records selected.";
            return RedirectToAction(nameof(Index));
        }

        var toDelete = await _db.Payments
            .Where(p => selectedIds.Contains(p.Id))
            .ToListAsync();

        if (toDelete.Count == 0)
        {
            TempData["Msg"] = "No matching records found.";
            return RedirectToAction(nameof(Index));
        }

        _db.Payments.RemoveRange(toDelete);
        await _db.SaveChangesAsync();

        TempData["Msg"] = $"Deleted {toDelete.Count} payment(s).";
        return RedirectToAction(nameof(Index));
    }
}