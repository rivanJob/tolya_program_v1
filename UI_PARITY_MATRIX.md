# UI_PARITY_MATRIX

| Legacy form | Web route | Web equivalent | Parity status | Notes |
|---|---|---|---|---|
| Form1 | `/Catalog` + `/` | Dashboard + Orders grid | Partial | Базовый список есть, toolbar/actions ещё переносятся |
| Editor | `/Catalog/Edit/{id}` | Order card page | Partial | Перенесены основные поля, не все вкладки/кнопки |
| ClientsEditor | `/Clients` | Clients grid | Partial | Только просмотр, CRUD в следующем этапе |
| ClientAddForm / ClientEditorTrue | `/Clients/Create`, `/Clients/Edit/{id}` | Planned | Gap | Ещё не реализовано |
| Stock | `/Stock` | Stock grid | Partial | Базовая таблица без операций |
| StockEditor / StockAddPosition / AddPosition | `/Stock/*` | Planned | Gap | Модальные формы и списания не перенесены |
| Settings | `/Settings` | Settings page | Partial | Заглушка, роли и настройки в плане |
| Authorisation / Registration | `/Auth/*` | Planned | Gap | Требуется ASP.NET Core Identity/кастом-auth |
| HistoryViewer | `/History` | Planned | Gap | Нет экрана |
| Graf | `/Analytics` | Planned | Gap | Нет графиков |
| Export | `/Export` | Planned | Gap | Нет экспортов |
| Printing_AKT_PRIEMA / VIDACHI | `/Print/*` | Planned | Gap | Нет печатных шаблонов |
| RedaktorAktov | `/Acts/Editor` | Planned | Gap | Нет |
| SmsMain / SmsRassilka / SmsFromEditor | `/Sms/*` | Planned | Gap | Нет интеграции шлюза |
| TrackingMail | `/Tracking/Mail` | Planned | Gap | Нет |
| ActPriemaPoGarantii / ActVidachiPoGarantii | `/Acts/*` | Planned | Gap | Нет |
| States / DataEditor / ColumnsEdtitor / BaseLineNumber / SURPRISE / StockPictureShowForm | various | Planned | Gap | Низкоуровневые/служебные формы в бэклоге |
