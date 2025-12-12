namespace Elevador.Domain;

public interface IElevadorService
{
    int AndarMenosUtilizado();

    (char Elevador, char Periodo) ElevadorMaisFrequentado();

    (char Elevador, char Periodo) ElevadorMenosFrequentado();

    char PeriodoMaiorUtilizacao();

    List<(char Elevador, double Percentual)> PercentualDeUso();
}

