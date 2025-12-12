# Sistema de Análise de Elevadores - Tecnopuc 99A

## Breve Descrição do Projeto

Sistema desenvolvido em **.NET 8.0** para análise de uso dos elevadores do prédio 99A da Tecnopuc. O sistema processa dados de utilização dos 5 elevadores (A, B, C, D, E) ao longo de 16 andares (0 a 15) em diferentes períodos do dia (Matutino, Vespertino e Noturno), gerando análises estatísticas para otimização do uso dos elevadores.

O projeto oferece duas formas de execução:
- **API REST**: Interface web com documentação Swagger para integração com outros sistemas
- **Aplicação Console**: Execução standalone via linha de comando

## Arquitetura

O projeto segue uma **arquitetura em camadas** (Clean Architecture), separando responsabilidades e garantindo baixo acoplamento e alta coesão:

```
┌─────────────────────────────────────────────────────────┐
│                  Elevador.API (Presentation)            │
│  - Controllers (Endpoints REST)                         │
│  - Models (DTOs/ViewModels)                             │
│  - Swagger/OpenAPI Documentation                        │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│               Elevador.Service (Application)            │
│  - ElevadorService (Lógica de Negócio)                 │
│  - JsonLoader (Carregamento de Dados)                  │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│              Elevador.Domain (Domain)                   │
│  - IElevadorService (Contrato/Interface)                │
│  - EntradaElevador (Entidade de Domínio)                │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│            Elevador.Console (Client/Entry Point)        │
│  - Interface de linha de comando                        │
└─────────────────────────────────────────────────────────┘
```

### Camadas

#### **Elevador.Domain** (Camada de Domínio)
Contém as entidades e contratos do domínio do negócio:
- `IElevadorService`: Interface que define o contrato para serviços de análise de elevadores
- `Models/EntradaElevador`: Modelo de domínio representando uma entrada de uso do elevador

#### **Elevador.Service** (Camada de Aplicação)
Implementa a lógica de negócio e regras de processamento:
- `ElevadorService`: Implementação concreta de `IElevadorService` com algoritmos de análise
- `JsonLoader`: Utilitário para carregar e deserializar dados do arquivo JSON

#### **Elevador.API** (Camada de Apresentação)
API REST com endpoints para acesso via HTTP:
- `Controllers/ElevadorController`: Controladores REST com endpoints da API
- `Models/`: DTOs (Data Transfer Objects) para comunicação (EntradaElevadorRequest, AnaliseResponse, etc.)
- `Program.cs`: Configuração, injeção de dependência e pipeline HTTP
- Documentação Swagger/OpenAPI integrada

#### **Elevador.Console** (Aplicação Cliente)
Aplicação console standalone para execução via linha de comando:
- Interface de usuário em modo texto
- Processa arquivo JSON e exibe resultados formatados

## Funcionalidades

O sistema extrai as seguintes informações:

1. **Andar menos utilizado** pelos usuários
2. **Elevador mais frequentado** e o período de maior fluxo
3. **Elevador menos frequentado** e o período de menor fluxo
4. **Período de maior utilização** do conjunto de elevadores
5. **Percentual de uso** de cada elevador

## Requisitos

- .NET 8.0 SDK
- Docker (opcional, para execução em container)

## Executando Localmente

### API REST (Recomendado)

```bash
# Restaurar dependências
dotnet restore

# Executar a API
dotnet run --project Elevador.API
```

A API estará disponível em:
- **HTTP**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger

### Aplicação Console

```bash
dotnet run --project Elevador.Console
```

### Docker

#### Executar apenas a API

```bash
docker build -f Dockerfile.api -t elevador-api .
docker run -p 5000:80 -v ${PWD}/input.json:/app/input.json elevador-api
```

#### Executar com Docker Compose (API + Console)

```bash
docker-compose up --build
```

A API estará disponível em `http://localhost:5000`

## Formato do Arquivo input.json

O arquivo `input.json` deve conter um array de objetos com a seguinte estrutura:

```json
[
  {
    "elevador": "A",
    "andar": 5,
    "periodo": "M"
  },
  {
    "elevador": "B",
    "andar": 3,
    "periodo": "V"
  }
]
```

### Valores Válidos

- **elevador**: A, B, C, D ou E
- **andar**: 0 a 15
- **periodo**: M (Matutino), V (Vespertino) ou N (Noturno)

## Exemplo de Saída

```
=== Sistema de Análise de Elevadores - Tecnopuc 99A ===

Carregando dados de: input.json

Total de entradas carregadas: 20

=== RESULTADOS DA ANÁLISE ===

a) Andar menos utilizado: 2
b) Elevador mais frequentado: A | Período de maior fluxo: Matutino (M)
c) Elevador menos frequentado: D | Período de menor fluxo: Noturno (N)
d) Período de maior utilização do conjunto de elevadores: Matutino (M)

e) Percentual de uso de cada elevador:
   Elevador A: 25.00%
   Elevador B: 20.00%
   Elevador C: 15.00%
   Elevador D: 15.00%
   Elevador E: 10.00%

=== Análise concluída com sucesso! ===
```

## API Endpoints

### POST /api/elevator/load
Loads input data and returns complete analysis.

**Request Body:**
```json
[
  {
    "elevador": "A",
    "andar": 5,
    "periodo": "M"
  }
]
```

**Response:**
```json
{
  "andarMenosUtilizado": 2,
  "elevadorMaisFrequentado": {
    "elevador": "A",
    "periodo": "M",
    "periodoNome": "Matutino"
  },
  "elevadorMenosFrequentado": {
    "elevador": "D",
    "periodo": "N",
    "periodoNome": "Noturno"
  },
  "periodoMaiorUtilizacao": {
    "periodo": "M",
    "periodoNome": "Matutino"
  },
  "percentuaisUso": [
    {
      "elevador": "A",
      "percentual": 25.0
    }
  ]
}
```

### POST /api/elevator/load-file
Loads data from `input.json` file and returns complete analysis.

### GET /api/elevator/analysis
Returns complete analysis based on `input.json` file.

### GET /api/elevator/least-used-floor
Returns the least used floor.

### GET /api/elevator/most-used
Returns the most used elevator and its period.

### GET /api/elevator/least-used
Returns the least used elevator and its period.

### GET /api/elevator/most-used-period
Returns the period with highest usage.

### GET /api/elevator/usage-percentages
Returns the usage percentage for each elevator.

## Estrutura do Projeto

```
.
├── Dockerfile
├── Dockerfile.api
├── docker-compose.yml
├── .dockerignore
├── .gitignore
├── Elevador.sln
├── input.json
├── README.md
├── Elevador.Domain/
│   ├── Elevador.Domain.csproj
│   ├── IElevadorService.cs
│   └── Models/
│       └── EntradaElevador.cs
├── Elevador.Service/
│   ├── Elevador.Service.csproj
│   ├── ElevadorService.cs
│   └── JsonLoader.cs
├── Elevador.API/
│   ├── Elevador.API.csproj
│   ├── Program.cs
│   ├── Controllers/
│   │   └── ElevadorController.cs
│   ├── Models/
│   │   ├── EntradaElevadorRequest.cs
│   │   └── AnaliseResponse.cs
│   └── Properties/
│       └── launchSettings.json
└── Elevador.Console/
    ├── Elevador.Console.csproj
    └── Program.cs
```

