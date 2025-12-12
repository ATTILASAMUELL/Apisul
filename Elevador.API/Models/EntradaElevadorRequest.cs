using System.ComponentModel.DataAnnotations;

namespace Elevador.API.Models;

/// <summary>
/// Modelo de entrada para dados de uso do elevador
/// </summary>
public class EntradaElevadorRequest
{
    /// <summary>
    /// Identificador do elevador (A, B, C, D ou E)
    /// </summary>
    /// <example>A</example>
    [Required(ErrorMessage = "O elevador é obrigatório")]
    [RegularExpression("^[A-E]$", ErrorMessage = "O elevador deve ser A, B, C, D ou E")]
    public char Elevador { get; set; }

    /// <summary>
    /// Número do andar (0 a 15)
    /// </summary>
    /// <example>5</example>
    [Required(ErrorMessage = "O andar é obrigatório")]
    [Range(0, 15, ErrorMessage = "O andar deve estar entre 0 e 15")]
    public int Andar { get; set; }

    /// <summary>
    /// Período do dia: M (Matutino), V (Vespertino) ou N (Noturno)
    /// </summary>
    /// <example>M</example>
    [Required(ErrorMessage = "O período é obrigatório")]
    [RegularExpression("^[MVN]$", ErrorMessage = "O período deve ser M (Matutino), V (Vespertino) ou N (Noturno)")]
    public char Periodo { get; set; }
}

