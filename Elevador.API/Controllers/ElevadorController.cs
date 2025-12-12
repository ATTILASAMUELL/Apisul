using Elevador.API.Models;
using Elevador.Domain;
using Elevador.Domain.Models;
using Elevador.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace Elevador.API.Controllers;

/// <summary>
/// Controller para análise de dados de uso de elevadores
/// </summary>
[ApiController]
[Route("api/elevator")]
public class ElevadorController : ControllerBase
{
    private readonly ILogger<ElevadorController> _logger;
    private readonly IWebHostEnvironment _environment;

    public ElevadorController(ILogger<ElevadorController> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Carrega dados de uso de elevadores e retorna análise completa
    /// </summary>
    /// <param name="entradas">Lista de entradas com dados de uso dos elevadores</param>
    /// <returns>Análise completa dos dados</returns>
    /// <response code="200">Retorna a análise completa dos dados</response>
    /// <response code="400">Dados inválidos ou lista vazia</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("load")]
    [ProducesResponseType(typeof(AnaliseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CarregarDados([FromBody] List<EntradaElevadorRequest> entradas)
    {
        try
        {
            if (entradas == null || !entradas.Any())
            {
                return BadRequest(new { message = "A lista de entradas não pode estar vazia" });
            }

            var entradasDomain = entradas.Select(e => new EntradaElevador
            {
                Elevador = char.ToUpper(e.Elevador),
                Andar = e.Andar,
                Periodo = char.ToUpper(e.Periodo)
            }).ToList();

            var servico = new ElevadorService(entradasDomain);
            var analise = ObterAnalise(servico);

            return Ok(analise);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar dados dos elevadores");
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Carrega dados do arquivo input.json e retorna análise completa
    /// </summary>
    /// <returns>Análise completa dos dados do arquivo</returns>
    /// <response code="200">Retorna a análise completa dos dados</response>
    /// <response code="404">Arquivo input.json não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("load-file")]
    [ProducesResponseType(typeof(AnaliseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CarregarArquivo()
    {
        try
        {
            var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "input.json");

            if (!System.IO.File.Exists(caminhoArquivo))
            {
                return NotFound(new { message = "Arquivo input.json não encontrado" });
            }

            var entradas = JsonLoader.CarregarEntradas(caminhoArquivo);
            var servico = new ElevadorService(entradas);
            var analise = ObterAnalise(servico);

            return Ok(analise);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogWarning(ex, "Arquivo não encontrado");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar arquivo");
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém análise completa dos dados do arquivo input.json
    /// </summary>
    /// <returns>Análise completa com todas as métricas</returns>
    /// <response code="200">Retorna a análise completa</response>
    /// <response code="404">Arquivo input.json não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("analysis")]
    [ProducesResponseType(typeof(AnaliseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ObterAnaliseCompleta()
    {
        try
        {
            var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "input.json");

            _logger.LogInformation($"Procurando arquivo em: {caminhoArquivo}");
            _logger.LogInformation($"Diretório atual: {Directory.GetCurrentDirectory()}");

            if (!System.IO.File.Exists(caminhoArquivo))
            {
                _logger.LogWarning($"Arquivo não encontrado: {caminhoArquivo}");
                return NotFound(new { 
                    message = "Arquivo input.json não encontrado. Use POST /api/elevator/load para enviar dados.",
                    path = caminhoArquivo,
                    currentDirectory = Directory.GetCurrentDirectory()
                });
            }

            var entradas = JsonLoader.CarregarEntradas(caminhoArquivo);
            var servico = new ElevadorService(entradas);
            var analise = ObterAnalise(servico);

            return Ok(analise);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "Arquivo não encontrado");
            return NotFound(new { message = ex.Message });
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogWarning(ex, "Arquivo não encontrado: {Path}", ex.FileName);
            return NotFound(new { 
                message = ex.Message,
                path = caminhoArquivo,
                currentDirectory = Directory.GetCurrentDirectory()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter análise: {Message}", ex.Message);
            return StatusCode(500, new { 
                message = "Erro interno do servidor", 
                error = ex.Message,
                innerException = ex.InnerException?.Message,
                stackTrace = _environment.IsDevelopment() ? ex.StackTrace : null
            });
        }
    }

    /// <summary>
    /// Obtém o andar menos utilizado pelos elevadores
    /// </summary>
    /// <returns>Andar menos utilizado</returns>
    /// <response code="200">Retorna o andar menos utilizado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("least-used-floor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ObterAndarMenosUtilizado()
    {
        try
        {
            var servico = ObterServico();
            var andar = servico.AndarMenosUtilizado();
            return Ok(new { andarMenosUtilizado = andar });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter andar menos utilizado");
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém o elevador e período mais frequentado
    /// </summary>
    /// <returns>Elevador e período mais frequentado</returns>
    /// <response code="200">Retorna elevador e período mais frequentado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("most-used")]
    [ProducesResponseType(typeof(ElevadorPeriodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ObterElevadorMaisFrequentado()
    {
        try
        {
            var servico = ObterServico();
            var (elevador, periodo) = servico.ElevadorMaisFrequentado();
            return Ok(new ElevadorPeriodoResponse
            {
                Elevador = elevador,
                Periodo = periodo,
                PeriodoNome = ObterNomePeriodo(periodo)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter elevador mais frequentado");
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém o elevador e período menos frequentado
    /// </summary>
    /// <returns>Elevador e período menos frequentado</returns>
    /// <response code="200">Retorna elevador e período menos frequentado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("least-used")]
    [ProducesResponseType(typeof(ElevadorPeriodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ObterElevadorMenosFrequentado()
    {
        try
        {
            var servico = ObterServico();
            var (elevador, periodo) = servico.ElevadorMenosFrequentado();
            return Ok(new ElevadorPeriodoResponse
            {
                Elevador = elevador,
                Periodo = periodo,
                PeriodoNome = ObterNomePeriodo(periodo)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter elevador menos frequentado");
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém o período de maior utilização dos elevadores
    /// </summary>
    /// <returns>Período de maior utilização</returns>
    /// <response code="200">Retorna o período de maior utilização</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("most-used-period")]
    [ProducesResponseType(typeof(PeriodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ObterPeriodoMaiorUtilizacao()
    {
        try
        {
            var servico = ObterServico();
            var periodo = servico.PeriodoMaiorUtilizacao();
            return Ok(new PeriodoResponse
            {
                Periodo = periodo,
                PeriodoNome = ObterNomePeriodo(periodo)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter período de maior utilização");
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém informações de diagnóstico do sistema
    /// </summary>
    /// <returns>Informações de diagnóstico</returns>
    /// <response code="200">Retorna informações de diagnóstico</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("diagnostics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ObterDiagnosticos()
    {
        try
        {
            var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "input.json");
            var arquivoExiste = System.IO.File.Exists(caminhoArquivo);
            var diretorioAtual = Directory.GetCurrentDirectory();
            var arquivosNoDiretorio = Directory.GetFiles(diretorioAtual).Select(Path.GetFileName).ToList();

            return Ok(new
            {
                currentDirectory = diretorioAtual,
                inputJsonPath = caminhoArquivo,
                inputJsonExists = arquivoExiste,
                filesInDirectory = arquivosNoDiretorio,
                environment = _environment.EnvironmentName
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao obter diagnósticos", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém percentuais de uso de cada elevador
    /// </summary>
    /// <returns>Lista com percentuais de uso de cada elevador</returns>
    /// <response code="200">Retorna os percentuais de uso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("usage-percentages")]
    [ProducesResponseType(typeof(List<PercentualUsoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ObterPercentuaisUso()
    {
        try
        {
            var servico = ObterServico();
            var percentuais = servico.PercentualDeUso();
            var response = percentuais.Select(p => new PercentualUsoResponse
            {
                Elevador = p.Elevador,
                Percentual = p.Percentual
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter percentuais de uso");
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    private IElevadorService ObterServico()
    {
        var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "input.json");

        _logger.LogInformation($"Procurando arquivo em: {caminhoArquivo}");

        if (!System.IO.File.Exists(caminhoArquivo))
        {
            _logger.LogWarning($"Arquivo não encontrado: {caminhoArquivo}");
            throw new FileNotFoundException($"Arquivo input.json não encontrado em: {caminhoArquivo}");
        }

        try
        {
            var entradas = JsonLoader.CarregarEntradas(caminhoArquivo);
            _logger.LogInformation($"Carregadas {entradas.Count} entradas do arquivo");
            return new ElevadorService(entradas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar entradas do arquivo");
            throw;
        }
    }

    private AnaliseResponse ObterAnalise(IElevadorService servico)
    {
        var andarMenosUtilizado = servico.AndarMenosUtilizado();
        var (elevadorMaisFreq, periodoMaisFreq) = servico.ElevadorMaisFrequentado();
        var (elevadorMenosFreq, periodoMenosFreq) = servico.ElevadorMenosFrequentado();
        var periodoMaiorUtilizacao = servico.PeriodoMaiorUtilizacao();
        var percentuais = servico.PercentualDeUso();

        return new AnaliseResponse
        {
            AndarMenosUtilizado = andarMenosUtilizado,
            ElevadorMaisFrequentado = new ElevadorPeriodoResponse
            {
                Elevador = elevadorMaisFreq,
                Periodo = periodoMaisFreq,
                PeriodoNome = ObterNomePeriodo(periodoMaisFreq)
            },
            ElevadorMenosFrequentado = new ElevadorPeriodoResponse
            {
                Elevador = elevadorMenosFreq,
                Periodo = periodoMenosFreq,
                PeriodoNome = ObterNomePeriodo(periodoMenosFreq)
            },
            PeriodoMaiorUtilizacao = new PeriodoResponse
            {
                Periodo = periodoMaiorUtilizacao,
                PeriodoNome = ObterNomePeriodo(periodoMaiorUtilizacao)
            },
            PercentuaisUso = percentuais.Select(p => new PercentualUsoResponse
            {
                Elevador = p.Elevador,
                Percentual = p.Percentual
            }).ToList()
        };
    }

    private static string ObterNomePeriodo(char periodo)
    {
        return periodo switch
        {
            'M' => "Matutino",
            'V' => "Vespertino",
            'N' => "Noturno",
            _ => "Desconhecido"
        };
    }
}

