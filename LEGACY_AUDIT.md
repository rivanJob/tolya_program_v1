# LEGACY_AUDIT

## Solution structure
- `MyWork2.sln`
- `MyWork2/` — основной WinForms проект.
- `BarcodeLib_NETBarcode_Trial/` — сторонняя библиотека/демо для штрихкодов.
- SQL артефакты: `scheme.sql`, `SIMPLE_TABLES.sql`, `CHANGE_SIMPLE_TABLES.sql`.

## Entry points
- `Program.Main()` → `Application.Run(new Form1())`.
- Главный экран: `Form1`.

## Forms/windows/dialogs inventory
| Form | Event handlers (designer wire-up, approx.) |
|---|---:|
| ActPriemaPoGarantii | 5 |
| ActVidachiPoGarantii | 5 |
| AddPosition | 28 |
| Authorisation | 3 |
| BaseLineNumber | 1 |
| ClientAddForm | 6 |
| ClientEditorTrue | 4 |
| ClientsEditor | 7 |
| ColumnsEdtitor | 4 |
| DataEditor | 7 |
| Editor | 29 |
| Export | 4 |
| Form1 | 41 |
| Graf | 5 |
| HistoryViewer | 5 |
| Printing_AKT_PRIEMA | 6 |
| Printing_AKT_VIDACHI | 5 |
| RedaktorAktov | 5 |
| Registration | 6 |
| SURPRISE | 9 |
| Settings | 33 |
| SmsFromEditor | 7 |
| SmsMain | 6 |
| SmsRassilka | 3 |
| States | 2 |
| Stock | 11 |
| StockAddPosition | 6 |
| StockEditor | 12 |
| StockPictureShowForm | 0 |
| TrackingMail | 2 |

## Data access and business logic
- Основная точка доступа к БД: `BDWorker.cs` (крупный data access/service слой).
- DB provider: `MySqlConnector`/`MySql.Data`.
- Существенная бизнес-логика сосредоточена в:
  - `BDWorker.cs`
  - `Form1.cs`, `Editor.cs`, `Settings.cs`, `Stock*.cs`, `Clients*.cs`
- В системе есть аудит истории (`HistoryBD`), права групп (`GroupDostup`), пользователи (`Users`).

## External integrations / dependencies
- SMS-модули (`SmsMain`, `SmsRassilka`, `SmsFromEditor`).
- Экспорт/отчёты/печать актов (`Export`, `Printing_AKT_*`, `RedaktorAktov`).
- Штрихкоды (`BarcodeLib`).
- Работа с файлами/изображениями (`StockPictureShowForm`, поля `Photo*`).
