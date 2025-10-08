# Instru��es Copilot para Clean Architecture .NET 9

Este template implementa Clean Architecture em .NET 9, incluindo mensageria RabbitMQ, OpenTelemetry, Serilog, testes e separa��o de camadas. Siga estas diretrizes para garantir ader�ncia aos princ�pios de Clean Architecture e � estrutura do projeto.

---

## Princ�pios Fundamentais
- **Independ�ncia de Camadas:**
  - `Domain` nunca depende de outras camadas.
  - `Application` depende apenas de `Domain`.
  - `Infrastructure` depende de `Domain` e implementa detalhes t�cnicos.
  - `Api` e `Workers` dependem de `Application` e `Infrastructure`.
- **Invers�o de Depend�ncia:**
  - Interfaces e abstra��es ficam no dom�nio.
  - Implementa��es concretais ficam na infraestrutura.
- **Separa��o de Responsabilidades:**
  - Cada camada tem um papel claro e isolado.

---

## Estrutura do Projeto
- **Domain:**
  - Entidades, Value Objects, Aggregates, Interfaces, regras de neg�cio puras.
- **Application:**
  - Casos de uso, servi�os de aplica��o, handlers, orquestra��o de regras de neg�cio.
- **Infrastructure:**
  - Persist�ncia (EF Core), integra��es externas, mensageria (RabbitMQ), implementa��es t�cnicas.
- **Api:**
  - Controllers, endpoints HTTP, valida��o, documenta��o Swagger, middlewares.
- **Workers:**
  - Servi�os de background (RabbitMQ consumers), implementados via `BackgroundService`.
- **Testes:**
  - Unit�rios: testam regras de neg�cio e servi�os isolados.
  - Integra��o: testam comunica��o entre camadas, mensageria e persist�ncia.

---

## Mensageria RabbitMQ
- Utilize abstra��es para publica��o (`IMessagingService`) e consumo.
- Configure exchanges, routing keys e headers conforme o dom�nio.
- Consumidores devem ser implementados como `BackgroundService` em `Workers`.

---

## Observabilidade e Logging
- Configure OpenTelemetry para coletar m�tricas e traces em todos os servi�os.
- Centralize logs com Serilog, usando sinks apropriados.

---

## Testes
- Use xUnit, Moq e AutoFixture para testes unit�rios.
- Use Testcontainers para simular RabbitMQ e banco de dados em testes de integra��o.
- Teste de carga com NBomber

---

## Documenta��o
- Mantenha coment�rios XML para gera��o autom�tica de documenta��o Swagger/OpenAPI.
- Documente endpoints e principais decis�es arquiteturais.

---

## Migra��es e Banco de Dados
- Versione migra��es do EF Core.
- Execute migra��es automaticamente em ambientes n�o-testes.

---

## Boas Pr�ticas Clean Architecture
- Nunca referencia camadas superiores.
- Prefira abstra��es e inje��o de depend�ncia.
- Separe regras de neg�cio de detalhes t�cnicos.
- Mantenha o dom�nio puro e test�vel.
- Atualize depend�ncias e documente decis�es relevantes.

---

## Refer�ncias
- [Clean Architecture - Jason Taylor](https://github.com/jasontaylordev/CleanArchitecture)
- [RabbitMq.Messaging.Toolkit](https://www.nuget.org/packages/RabbitMq.Messaging.Toolkit)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/instrumentation/net/)
- [Serilog](https://serilog.net/)

---
Siga estas diretrizes para garantir que o c�digo gerado esteja alinhado com os princ�pios de Clean Architecture e a estrutura do projeto.