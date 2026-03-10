# MIGRATION_PLAN

## Phase 1 (done)
- Legacy audit и полная инвентаризация форм/модулей.
- Создание документов миграции.
- Подготовка web-архитектуры и базового MVC проекта.

## Phase 2 (in progress)
- Перенос ядра: Dashboard, Orders list, Order edit, Clients list, Stock list.
- Bootstrap UI каркас, маршруты, сервисы, EF Core MySQL подключение.

## Phase 3
- Полный CRUD клиентов/склада/заказов с server-side validation.
- Роли и авторизация (`users` + `groupdostup`).
- История изменений (`historybd`) и журналы действий.

## Phase 4
- Печать актов, экспорт, SMS, графики, трекинг.
- Перенос вторичных диалогов и всех toolbar/context actions.

## Phase 5
- Parity verification по каждой форме и кнопке.
- Закрытие `MIGRATION_GAPS.md`.
