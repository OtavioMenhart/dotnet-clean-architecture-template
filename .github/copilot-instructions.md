# Instruções Copilot para Clean Architecture .NET 9

Este template implementa Clean Architecture em .NET 9, incluindo mensageria RabbitMQ, OpenTelemetry, Serilog, testes e separação de camadas. Siga estas diretrizes para garantir aderência aos princípios de Clean Architecture e à estrutura do projeto.

---

## Princípios Fundamentais
- **Independência de Camadas:**
  - `Domain` nunca depende de outras camadas.
  - `Application` depende apenas de `Domain`.
  - `Infrastructure` depende de `Domain` e implementa detalhes técnicos.
  - `Api` e `Workers` dependem de `Application` e `Infrastructure`.
- **Inversão de Dependência:**
  - Interfaces e abstrações ficam no domínio.
  - Implementações concretais ficam na infraestrutura.
- **Separação de Responsabilidades:**
  - Cada camada tem um papel claro e isolado.

---

## Estrutura do Projeto
- **Domain:**
  - Entidades, Value Objects, Aggregates, Interfaces, regras de negócio puras.
- **Application:**
  - Casos de uso, serviços de aplicação, handlers, orquestração de regras de negócio.
- **Infrastructure:**
  - Persistência (EF Core), integrações externas, mensageria (RabbitMQ), implementações técnicas.
- **Api:**
  - Controllers, endpoints HTTP, validação, documentação Swagger, middlewares.
- **Workers:**
  - Serviços de background (RabbitMQ consumers), implementados via `BackgroundService`.
- **Testes:**
  - Unitários: testam regras de negócio e serviços isolados.
  - Integração: testam comunicação entre camadas, mensageria e persistência.

---

## Mensageria RabbitMQ
- Utilize abstrações para publicação (`IMessagingService`) e consumo.
- Configure exchanges, routing keys e headers conforme o domínio.
- Consumidores devem ser implementados como `BackgroundService` em `Workers`.

---

## Observabilidade e Logging
- Configure OpenTelemetry para coletar métricas e traces em todos os serviços.
- Centralize logs com Serilog, usando sinks apropriados.

---

## Testes
- Use xUnit, Moq e AutoFixture para testes unitários.
- Use Testcontainers para simular RabbitMQ e banco de dados em testes de integração.
- Teste de carga com NBomber

---

## Documentação
- Mantenha comentários XML para geração automática de documentação Swagger/OpenAPI.
- Documente endpoints e principais decisões arquiteturais.

---

## Migrações e Banco de Dados
- Versione migrações do EF Core.
- Execute migrações automaticamente em ambientes não-testes.

---

## Boas Práticas Clean Architecture
- Nunca referencia camadas superiores.
- Prefira abstrações e injeção de dependência.
- Separe regras de negócio de detalhes técnicos.
- Mantenha o domínio puro e testável.
- Atualize dependências e documente decisões relevantes.

---

## Referências
- [Clean Architecture - Jason Taylor](https://github.com/jasontaylordev/CleanArchitecture)
- [RabbitMq.Messaging.Toolkit](https://www.nuget.org/packages/RabbitMq.Messaging.Toolkit)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/instrumentation/net/)
- [Serilog](https://serilog.net/)

---
Siga estas diretrizes para garantir que o código gerado esteja alinhado com os princípios de Clean Architecture e a estrutura do projeto.