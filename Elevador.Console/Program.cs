using Elevador.Service;

namespace Elevador.Console;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var caminhoArquivo = args.Length > 0 ? args[0] : "input.json";
            
            if (!File.Exists(caminhoArquivo))
            {
                caminhoArquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.json");
            }

            System.Console.WriteLine("=== Sistema de Análise de Elevadores - Tecnopuc 99A ===\n");
            System.Console.WriteLine($"Carregando dados de: {caminhoArquivo}\n");

            var entradas = JsonLoader.CarregarEntradas(caminhoArquivo);
            System.Console.WriteLine($"Total de entradas carregadas: {entradas.Count}\n");

            IElevadorService servico = new ElevadorService(entradas);

            System.Console.WriteLine("=== RESULTADOS DA ANÁLISE ===\n");

            var andarMenosUtilizado = servico.AndarMenosUtilizado();
            System.Console.WriteLine($"a) Andar menos utilizado: {andarMenosUtilizado}");

            var (elevadorMaisFreq, periodoMaisFreq) = servico.ElevadorMaisFrequentado();
            var periodoMaisFreqTexto = ObterNomePeriodo(periodoMaisFreq);
            System.Console.WriteLine($"b) Elevador mais frequentado: {elevadorMaisFreq} | Período de maior fluxo: {periodoMaisFreqTexto} ({periodoMaisFreq})");

            var (elevadorMenosFreq, periodoMenosFreq) = servico.ElevadorMenosFrequentado();
            var periodoMenosFreqTexto = ObterNomePeriodo(periodoMenosFreq);
            System.Console.WriteLine($"c) Elevador menos frequentado: {elevadorMenosFreq} | Período de menor fluxo: {periodoMenosFreqTexto} ({periodoMenosFreq})");

            var periodoMaiorUtilizacao = servico.PeriodoMaiorUtilizacao();
            var periodoMaiorUtilizacaoTexto = ObterNomePeriodo(periodoMaiorUtilizacao);
            System.Console.WriteLine($"d) Período de maior utilização do conjunto de elevadores: {periodoMaiorUtilizacaoTexto} ({periodoMaiorUtilizacao})");

            System.Console.WriteLine("\ne) Percentual de uso de cada elevador:");
            var percentuais = servico.PercentualDeUso();
            foreach (var (elevador, percentual) in percentuais)
            {
                System.Console.WriteLine($"   Elevador {elevador}: {percentual:F2}%");
            }

            System.Console.WriteLine("\n=== Análise concluída com sucesso! ===");
        }
        catch (FileNotFoundException ex)
        {
            System.Console.WriteLine($"ERRO: {ex.Message}");
            System.Console.WriteLine("\nCertifique-se de que o arquivo input.json existe no diretório da aplicação.");
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"ERRO: {ex.Message}");
            if (ex.InnerException != null)
            {
                System.Console.WriteLine($"Detalhes: {ex.InnerException.Message}");
            }
            Environment.Exit(1);
        }
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

