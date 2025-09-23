# dotnet-clean-architecture-template

Execute o docker-compose
docker-compose up -d

Se quiser adicionar migrations
dotnet ef migrations add AddProductTable --project src/CleanArchTemplate.Infrastructure --startup-project src/CleanArchTemplate.Api --output-dir Persistence/Migrations --context AppDbContext

O código aplica as migrations automaticamente ao iniciar.