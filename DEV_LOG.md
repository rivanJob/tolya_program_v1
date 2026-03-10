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
- Добавлена конфигурация запуска под подпутём `/tolyaprogram` (Nginx + `ReverseProxy:PathBase`) для `prostochatbot.ru`.
- Добавлен recovery-runbook для кейса `systemd: inactive (dead)` и `curl 127.0.0.1:5000` connection refused.
- Добавлен готовый пример Nginx для текущего боевого конфига `prostochatbot.ru` (PHP-сайт + ASP.NET Core на `/tolyaprogram`).
- Добавлен `healthz` endpoint и runbook диагностики `502 Bad Gateway` (порт 5000, systemd, nginx error log).
