using Elevador.Domain.Models;
using System.Text.Json;

namespace Elevador.Service;

public static class JsonLoader
{
    public static List<EntradaElevador> CarregarEntradas(string caminhoArquivo)
    {
        if (!File.Exists(caminhoArquivo))
            throw new FileNotFoundException($"Arquivo não encontrado: {caminhoArquivo}");

        var jsonContent = File.ReadAllText(caminhoArquivo);
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        };

        var entradas = JsonSerializer.Deserialize<List<EntradaElevador>>(jsonContent, options);

        if (entradas == null || !entradas.Any())
            throw new InvalidOperationException("O arquivo JSON não contém entradas válidas.");

        ValidarEntradas(entradas);

        return entradas;
    }

    private static void ValidarEntradas(List<EntradaElevador> entradas)
    {
        var elevadoresValidos = new[] { 'A', 'B', 'C', 'D', 'E' };
        var periodosValidos = new[] { 'M', 'V', 'N' };

        foreach (var entrada in entradas)
        {
            if (!elevadoresValidos.Contains(char.ToUpper(entrada.Elevador)))
                throw new ArgumentException($"Elevador inválido: {entrada.Elevador}. Deve ser A, B, C, D ou E.");

            if (entrada.Andar < 0 || entrada.Andar > 15)
                throw new ArgumentException($"Andar inválido: {entrada.Andar}. Deve estar entre 0 e 15.");

            if (!periodosValidos.Contains(char.ToUpper(entrada.Periodo)))
                throw new ArgumentException($"Período inválido: {entrada.Periodo}. Deve ser M, V ou N.");
        }
    }
}

