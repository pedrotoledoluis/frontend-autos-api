using AutosWeb.Models;
using AutosWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutosWeb.Controllers;

public class AutosController : Controller
{
    private readonly IAutosApiClient _api;
    private readonly ILogger<AutosController> _logger;

    public AutosController(IAutosApiClient api, ILogger<AutosController> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        try
        {
            var autos = await _api.GetAllAsync(ct);
            return View(autos);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "No se pudo contactar a la API de Autos");
            TempData["Error"] = "No se pudo contactar a la API. Verifique que el backend esté ejecutándose.";
            return View(Array.Empty<AutoViewModel>());
        }
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var auto = await _api.GetByIdAsync(id, ct);
        if (auto is null) return NotFound();
        return View(auto);
    }

    public IActionResult Create() => View(new AutoViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AutoViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        var created = await _api.CreateAsync(model, ct);
        if (created is null)
        {
            ModelState.AddModelError(string.Empty, "No se pudo crear el auto. Intente nuevamente.");
            return View(model);
        }

        TempData["Success"] = $"Auto '{created.Marca} {created.Modelo}' creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var auto = await _api.GetByIdAsync(id, ct);
        if (auto is null) return NotFound();
        return View(auto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AutoViewModel model, CancellationToken ct)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var ok = await _api.UpdateAsync(id, model, ct);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, "No se pudo actualizar el auto.");
            return View(model);
        }

        TempData["Success"] = "Auto actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var auto = await _api.GetByIdAsync(id, ct);
        if (auto is null) return NotFound();
        return View(auto);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var ok = await _api.DeleteAsync(id, ct);
        TempData[ok ? "Success" : "Error"] = ok
            ? "Auto eliminado correctamente."
            : "No se pudo eliminar el auto.";
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View("Error", new ErrorViewModel { RequestId = requestId });
    }
}
