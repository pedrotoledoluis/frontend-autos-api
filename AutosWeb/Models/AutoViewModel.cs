using System.ComponentModel.DataAnnotations;

namespace AutosWeb.Models;

public class AutoViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La marca es obligatoria")]
    [StringLength(50)]
    [Display(Name = "Marca")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo es obligatorio")]
    [StringLength(50)]
    [Display(Name = "Modelo")]
    public string Modelo { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "Año fuera de rango (1900-2100)")]
    [Display(Name = "Año")]
    public int Anio { get; set; } = DateTime.Now.Year;

    [Required(ErrorMessage = "El tipo de auto es obligatorio")]
    [StringLength(30)]
    [Display(Name = "Tipo de auto")]
    public string TipoAuto { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "Los asientos deben estar entre 1 y 20")]
    [Display(Name = "Cantidad de asientos")]
    public int CantidadAsientos { get; set; } = 5;

    [Required(ErrorMessage = "El color es obligatorio")]
    [StringLength(30)]
    [Display(Name = "Color")]
    public string Color { get; set; } = string.Empty;

    [Range(0, 999999999)]
    [Display(Name = "Precio")]
    public decimal Precio { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Fecha de creación")]
    public DateTime FechaCreacion { get; set; }
}
