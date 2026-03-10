# MyWork2 Legacy → Web Migration

Этот репозиторий содержит:
1. Legacy WinForms приложение `MyWork2` (.NET Framework 4.8).
2. Новое web-приложение `MyWork2.Web` (ASP.NET Core MVC + Bootstrap 5).

## Статус
- Выполнены аудит legacy, функциональная карта и матрица parity.
- Запущен базовый перенос ключевых модулей: заказы (`Catalog`), клиенты (`Clients`), склад (`Stock`), главная панель.
- Остальной функционал зафиксирован как migration gaps и запланирован к переносу по этапам.

## Документы миграции
- `LEGACY_AUDIT.md`
- `FUNCTIONAL_MAP.md`
- `UI_PARITY_MATRIX.md`
- `DB_USAGE_MAP.md`
- `MIGRATION_PLAN.md`
- `MIGRATION_GAPS.md`
- `DEV_LOG.md`

## Запуск web-версии (Ubuntu)
1. Установить .NET SDK 8.
2. Настроить MariaDB/MySQL и применить `scheme.sql`.
3. Обновить `MyWork2.Web/appsettings.json`.
4. Выполнить `dotnet run --project MyWork2.Web`.


## Развёртывание на Ubuntu
Подробная production-инструкция вынесена в `DEPLOY_UBUNTU.md` (пакеты, БД, publish, systemd, Nginx).


> Если видите `MSB1009: файл проекта не существует`, проверьте рабочую директорию и используйте корректный путь к `*.csproj` (см. `DEPLOY_UBUNTU.md`).


Поддержан сценарий reverse-proxy размещения на подпути: `https://prostochatbot.ru/tolyaprogram`.


Добавлен troubleshooting для состояния `mywork2-web: inactive (dead)` в `DEPLOY_UBUNTU.md` (пошаговый recovery).


В `DEPLOY_UBUNTU.md` добавлен готовый пример Nginx-конфига для `prostochatbot.ru` + `/tolyaprogram`.


Добавлена отдельная секция `502 Bad Gateway` диагностики в `DEPLOY_UBUNTU.md` (systemd + nginx + healthz).
