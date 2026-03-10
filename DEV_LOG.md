# DEV_LOG

## 2026-03-10
- Проведён аудит legacy WinForms приложения.
- Создан пакет документации миграции.
- Создан `MyWork2.Web` (ASP.NET Core MVC, Bootstrap, EF Core MySQL).
- Добавлены модели/контекст/сервисы/контроллеры:
  - Home, Catalog, Clients, Stock, Settings.
- Реализованы Bootstrap views для базовых сценариев просмотра и частичного редактирования.
- Зафиксированы неперенесённые функции в `MIGRATION_GAPS.md`.

- Добавлена пошаговая инструкция деплоя на Ubuntu: `DEPLOY_UBUNTU.md`.
- Уточнены команды `dotnet restore/publish` для двух сценариев директорий и добавлен troubleshooting по ошибке `MSB1009`.
