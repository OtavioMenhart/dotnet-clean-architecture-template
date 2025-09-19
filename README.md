# dotnet-clean-architecture-template

Execute o docker-compose
docker-compose up -d

Aplique as migrations
dotnet ef database update --context AppDbContext --startup-project src/CleanArchTemplate.Api --project src/CleanArchTemplate.Infrastructure

Se quiser adicionar migrations
dotnet ef migrations add AddProductTable --project src/CleanArchTemplate.Infrastructure --startup-project src/CleanArchTemplate.Api --output-dir Persistence/Migrations --context AppDbContext