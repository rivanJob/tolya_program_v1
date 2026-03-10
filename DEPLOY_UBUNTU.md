# DEPLOY_UBUNTU.md

## Цель
Пошаговый запуск web-версии `MyWork2.Web` на Ubuntu сервере с:
- ASP.NET Core 8 runtime
- MariaDB/MySQL
- systemd сервисом
- Nginx reverse proxy

---

## 1) Что загрузить на сервер

Минимальный набор файлов/папок из репозитория:
- `MyWork2.Web/`
- `scheme.sql`
- (опционально) `README_MIGRATION.md`, `MIGRATION_GAPS.md`, `UI_PARITY_MATRIX.md` для операционного контроля parity

Если деплой через готовую публикацию (`dotnet publish`), то можно загрузить только содержимое папки publish + `scheme.sql`.

---

## 2) Установить зависимости на Ubuntu (22.04/24.04)

```bash
sudo apt update
sudo apt install -y wget gnupg apt-transport-https ca-certificates software-properties-common
```

### 2.1 Установить .NET 8 SDK/Runtime

```bash
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

sudo apt update
sudo apt install -y dotnet-sdk-8.0 aspnetcore-runtime-8.0
```

Проверка:
```bash
dotnet --info
```

### 2.2 Установить MariaDB

```bash
sudo apt install -y mariadb-server
sudo systemctl enable mariadb
sudo systemctl start mariadb
```

Первичная защита:
```bash
sudo mysql_secure_installation
```

### 2.3 Установить Nginx

```bash
sudo apt install -y nginx
sudo systemctl enable nginx
sudo systemctl start nginx
```

---

## 3) Подготовить базу данных

### 3.1 Создать БД и пользователя

```bash
sudo mysql -u root -p
```

Внутри MariaDB:
```sql
CREATE DATABASE tolyabase CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
CREATE USER 'tolyapp'@'localhost' IDENTIFIED BY 'StrongPasswordHere!';
GRANT ALL PRIVILEGES ON tolyabase.* TO 'tolyapp'@'localhost';
FLUSH PRIVILEGES;
EXIT;
```

### 3.2 Применить схему

```bash
mysql -u tolyapp -p tolyabase < scheme.sql
```

---

## 4) Настроить приложение

В `MyWork2.Web/appsettings.json` задайте рабочую строку подключения:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=127.0.0.1;port=3306;database=tolyabase;user=tolyapp;password=StrongPasswordHere!;"
  }
}
```

Рекомендовано для production вынести секреты в переменные окружения/systemd, а не хранить пароль в git.

---

## 5) Сборка и публикация

Из корня репозитория:

```bash
dotnet restore MyWork2.Web/MyWork2.Web.csproj
dotnet publish MyWork2.Web/MyWork2.Web.csproj -c Release -o /var/www/mywork2-web
```

Назначить владельца:

```bash
sudo chown -R www-data:www-data /var/www/mywork2-web
```

---

## 6) Создать systemd сервис

Файл `/etc/systemd/system/mywork2-web.service`:

```ini
[Unit]
Description=MyWork2 ASP.NET Core Web App
After=network.target mariadb.service

[Service]
WorkingDirectory=/var/www/mywork2-web
ExecStart=/usr/bin/dotnet /var/www/mywork2-web/MyWork2.Web.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=mywork2-web
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://127.0.0.1:5000

[Install]
WantedBy=multi-user.target
```

Применить:

```bash
sudo systemctl daemon-reload
sudo systemctl enable mywork2-web
sudo systemctl start mywork2-web
sudo systemctl status mywork2-web
```

Логи:

```bash
journalctl -u mywork2-web -f
```

---

## 7) Настроить Nginx reverse proxy

Файл `/etc/nginx/sites-available/mywork2-web`:

```nginx
server {
    listen 80;
    server_name _;

    location / {
        proxy_pass         http://127.0.0.1:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```

Включить сайт:

```bash
sudo ln -s /etc/nginx/sites-available/mywork2-web /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

---

## 8) Проверка после запуска

1. `systemctl status mywork2-web` — сервис активен.
2. `curl -I http://127.0.0.1:5000` — Kestrel отвечает.
3. `curl -I http://<SERVER_IP>` — Nginx проксирует.
4. Открыть в браузере `http://<SERVER_IP>`.

---

## 9) Обновление версии приложения

```bash
# в директории с кодом
sudo systemctl stop mywork2-web
dotnet publish MyWork2.Web/MyWork2.Web.csproj -c Release -o /var/www/mywork2-web
sudo chown -R www-data:www-data /var/www/mywork2-web
sudo systemctl start mywork2-web
```

---

## 10) Частые проблемы

1. **`dotnet: command not found`**
   - Не установлен .NET SDK/runtime или PATH не обновлён.

2. **Ошибка подключения к БД**
   - Проверить `DefaultConnection`, доступ пользователя `tolyapp`, состояние MariaDB.

3. **502 Bad Gateway в Nginx**
   - Проверить, что `mywork2-web` работает и слушает `127.0.0.1:5000`.

4. **После деплоя нет новых изменений**
   - Проверить, что publish делался в ту же папку `/var/www/mywork2-web`.

