using Elevador.Domain;
using Elevador.Domain.Models;
using System.Text.Json;

namespace Elevador.Service;

public class ElevadorService : IElevadorService
{
    private readonly List<EntradaElevador> _entradas;

    public ElevadorService(List<EntradaElevador> entradas)
    {
        _entradas = entradas ?? throw new ArgumentNullException(nameof(entradas));
    }

    public int AndarMenosUtilizado()
    {
        if (!_entradas.Any())
            throw new InvalidOperationException("Não há entradas para análise.");

        var andares = _entradas
            .GroupBy(e => e.Andar)
            .Select(g => new { Andar = g.Key, Contagem = g.Count() })
            .OrderBy(x => x.Contagem)
            .ThenBy(x => x.Andar)
            .ToList();

        return andares.First().Andar;
    }

    public (char Elevador, char Periodo) ElevadorMaisFrequentado()
    {
        if (!_entradas.Any())
            throw new InvalidOperationException("Não há entradas para análise.");

        var elevadorPeriodo = _entradas
            .GroupBy(e => new { e.Elevador, e.Periodo })
            .Select(g => new 
            { 
                Elevador = g.Key.Elevador, 
                Periodo = g.Key.Periodo, 
                Contagem = g.Count() 
            })
            .OrderByDescending(x => x.Contagem)
            .ThenBy(x => x.Elevador)
            .ThenBy(x => x.Periodo)
            .First();

        return (elevadorPeriodo.Elevador, elevadorPeriodo.Periodo);
    }

    public (char Elevador, char Periodo) ElevadorMenosFrequentado()
    {
        if (!_entradas.Any())
            throw new InvalidOperationException("Não há entradas para análise.");

        var elevadorPeriodo = _entradas
            .GroupBy(e => new { e.Elevador, e.Periodo })
            .Select(g => new 
            { 
                Elevador = g.Key.Elevador, 
                Periodo = g.Key.Periodo, 
                Contagem = g.Count() 
            })
            .OrderBy(x => x.Contagem)
            .ThenBy(x => x.Elevador)
            .ThenBy(x => x.Periodo)
            .First();

        return (elevadorPeriodo.Elevador, elevadorPeriodo.Periodo);
    }

    public char PeriodoMaiorUtilizacao()
    {
        if (!_entradas.Any())
            throw new InvalidOperationException("Não há entradas para análise.");

        var periodo = _entradas
            .GroupBy(e => e.Periodo)
            .Select(g => new { Periodo = g.Key, Contagem = g.Count() })
            .OrderByDescending(x => x.Contagem)
            .ThenBy(x => x.Periodo)
            .First();

        return periodo.Periodo;
    }

    public List<(char Elevador, double Percentual)> PercentualDeUso()
    {
        if (!_entradas.Any())
            throw new InvalidOperationException("Não há entradas para análise.");

        var totalServicos = _entradas.Count;
        var elevadores = new[] { 'A', 'B', 'C', 'D', 'E' };

        var percentuais = elevadores
            .Select(elevador =>
            {
                var uso = _entradas.Count(e => e.Elevador == elevador);
                var percentual = totalServicos > 0 
                    ? (double)uso / totalServicos * 100 
                    : 0.0;
                return (Elevador: elevador, Percentual: Math.Round(percentual, 2));
            })
            .OrderByDescending(x => x.Percentual)
            .ThenBy(x => x.Elevador)
            .ToList();

        return percentuais;
    }
}

