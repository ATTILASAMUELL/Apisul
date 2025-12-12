namespace Elevador.API.Models;

/// <summary>
/// Resposta com análise completa dos dados de elevadores
/// </summary>
public class AnaliseResponse
{
    /// <summary>
    /// Andar menos utilizado pelos elevadores
    /// </summary>
    public int AndarMenosUtilizado { get; set; }

    /// <summary>
    /// Elevador e período mais frequentado
    /// </summary>
    public ElevadorPeriodoResponse ElevadorMaisFrequentado { get; set; } = new();

    /// <summary>
    /// Elevador e período menos frequentado
    /// </summary>
    public ElevadorPeriodoResponse ElevadorMenosFrequentado { get; set; } = new();

    /// <summary>
    /// Período de maior utilização
    /// </summary>
    public PeriodoResponse PeriodoMaiorUtilizacao { get; set; } = new();

    /// <summary>
    /// Lista de percentuais de uso por elevador
    /// </summary>
    public List<PercentualUsoResponse> PercentuaisUso { get; set; } = new();
}

/// <summary>
/// Resposta com elevador e período
/// </summary>
public class ElevadorPeriodoResponse
{
    /// <summary>
    /// Identificador do elevador (A, B, C, D ou E)
    /// </summary>
    public char Elevador { get; set; }

    /// <summary>
    /// Código do período (M, V ou N)
    /// </summary>
    public char Periodo { get; set; }

    /// <summary>
    /// Nome do período (Matutino, Vespertino ou Noturno)
    /// </summary>
    public string PeriodoNome { get; set; } = string.Empty;
}

/// <summary>
/// Resposta com período
/// </summary>
public class PeriodoResponse
{
    /// <summary>
    /// Código do período (M, V ou N)
    /// </summary>
    public char Periodo { get; set; }

    /// <summary>
    /// Nome do período (Matutino, Vespertino ou Noturno)
    /// </summary>
    public string PeriodoNome { get; set; } = string.Empty;
}

/// <summary>
/// Resposta com percentual de uso de um elevador
/// </summary>
public class PercentualUsoResponse
{
    /// <summary>
    /// Identificador do elevador (A, B, C, D ou E)
    /// </summary>
    public char Elevador { get; set; }

    /// <summary>
    /// Percentual de uso do elevador
    /// </summary>
    public double Percentual { get; set; }
}

