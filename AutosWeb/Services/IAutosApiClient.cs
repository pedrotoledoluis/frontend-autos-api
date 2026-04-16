using AutosWeb.Models;

namespace AutosWeb.Services;

public interface IAutosApiClient
{
    Task<IReadOnlyList<AutoViewModel>> GetAllAsync(CancellationToken ct = default);
    Task<AutoViewModel?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AutoViewModel?> CreateAsync(AutoViewModel model, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, AutoViewModel model, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
