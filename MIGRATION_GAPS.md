# MIGRATION_GAPS

| Gap | Legacy source | Risk | Planned fix |
|---|---|---|---|
| Не перенесены все кнопки/обработчики `Form1` | `Form1.cs` + Designer | Потеря сценариев оператора | Разбить toolbar actions по контроллерам Orders/Acts/History |
| Неполная карточка заказа | `Editor.cs` | Невозможность полного редактирования | Декомпозировать Editor на вкладки/partial views |
| Нет auth/roles | `Authorisation.cs`, `Registration.cs`, `Settings.cs` | Безопасность и ограничения доступа | Внедрить ASP.NET Core auth + policy based permissions |
| Нет модулей SMS | `SmsMain.cs`, `SmsRassilka.cs` | Потеря уведомлений | Добавить провайдер SMS и журнал доставки |
| Нет печати актов и экспортов | `Printing_AKT_*`, `Export.cs` | Потеря документов | Реализовать PDF/Excel генерацию |
| Нет графиков/истории/состояний | `Graf.cs`, `HistoryViewer.cs`, `States.cs` | Потеря аналитики и аудита | Отдельные контроллеры Analytics/History/States |
| Нет операций по `stockmap`/`statesmap` | `BDWorker.cs`, `StockAddPosition.cs` | Неучтённые расходники | Реализовать связующие экраны и транзакции |
