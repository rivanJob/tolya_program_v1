# DB_USAGE_MAP

## Основные таблицы (`scheme.sql`)
- `catalog` — заказы ремонта.
- `clientsmap` — клиенты.
- `stock` — складские позиции.
- `stockmap` — связь заказа и использованных ЗИП.
- `statesmap` — статусы/состояния по заказу.
- `historybd` — аудит/история.
- `users` — пользователи.
- `groupdostup` — права/группы доступа.

## DB access points in legacy
- `MyWork2/BDWorker.cs`: централизованные SQL операции (SELECT/INSERT/UPDATE/DELETE).
- Дополнительно: `Settings.cs`, `Form1.cs` выполняют проверку соединения/операции.

## Web mapping (current)
- `AppDbContext` маппит: `catalog`, `clientsmap`, `stock`, `users`, `groupdostup`, `historybd`.
- Сервисы:
  - `RepairOrderService` → `catalog`
  - `ClientService` → `clientsmap`
  - `StockService` → `stock`
