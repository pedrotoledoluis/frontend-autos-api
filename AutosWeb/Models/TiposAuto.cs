namespace AutosWeb.Models;

/// <summary>
/// Catálogo de tipos de auto admitidos por la UI. Centralizado acá para
/// evitar duplicar la lista hardcodeada en múltiples vistas Razor.
/// </summary>
public static class TiposAuto
{
    public static IReadOnlyList<string> Opciones { get; } = new[]
    {
        "Sedan",
        "Hatchback",
        "SUV",
        "Pickup",
        "Coupe",
        "Convertible",
        "Monovolumen",
        "Deportivo"
    };
}
