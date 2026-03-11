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
  },
  "Database": {
    "MariaDbVersion": "10.4.32"
  }
}
```

Рекомендовано для production вынести секреты в переменные окружения/systemd, а не хранить пароль в git.

Для размещения приложения по пути `https://prostochatbot.ru/tolyaprogram` оставьте:

```json
"ReverseProxy": {
  "PathBase": "/tolyaprogram"
}
```

---

## 5) Сборка и публикация

> Важно: ошибка `MSB1009: файл проекта не существует` обычно означает, что указан неверный путь к `.csproj`.

### Вариант A — вы находитесь в корне репозитория (например `/opt/tolya_program_v1`)

```bash
dotnet restore MyWork2.Web/MyWork2.Web.csproj
dotnet publish MyWork2.Web/MyWork2.Web.csproj -c Release -o /var/www/mywork2-web
```

### Вариант B — вы уже внутри папки проекта (например `/var/www/MyWork2.Web`)

```bash
dotnet restore MyWork2.Web.csproj
dotnet publish MyWork2.Web.csproj -c Release -o /var/www/mywork2-web
```

### Быстрая диагностика пути

```bash
pwd
ls
find . -maxdepth 3 -name "*.csproj"
```

Если команда нашла файл `./MyWork2.Web.csproj`, используйте путь без префикса `MyWork2.Web/`.

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

### 7.1 Если это отдельный домен/сайт

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

### 7.2 Если у вас уже есть сайт `https://prostochatbot.ru/` и нужен путь `/tolyaprogram`

В существующий `server { ... }` для `prostochatbot.ru` добавьте:

```nginx
location = /tolyaprogram {
    return 301 /tolyaprogram/;
}

location /tolyaprogram/ {
    proxy_pass         http://127.0.0.1:5000;
    proxy_http_version 1.1;
    proxy_set_header   Upgrade $http_upgrade;
    proxy_set_header   Connection keep-alive;
    proxy_set_header   Host $host;
    proxy_cache_bypass $http_upgrade;
    proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header   X-Forwarded-Proto $scheme;
}
```

После изменений:

```bash
sudo nginx -t
sudo systemctl reload nginx
```

Проверьте извне:

```bash
curl -I https://prostochatbot.ru/tolyaprogram/
```


> Команду `ln -s /etc/nginx/sites-available/mywork2-web ...` используйте **только** для сценария 7.1 (отдельный сайт).
> Для сценария 7.2 (уже существующий `prostochatbot.ru`) редактируется текущий конфиг домена, отдельный site-файл подключать не нужно.

---



### 7.3 Готовый пример для вашего текущего конфига `prostochatbot.ru`

Да, идея правильная, но важно:
1. Блоки `location = /tolyaprogram` и `location /tolyaprogram/` должны быть **внутри HTTPS server (443)**.
2. Лучше разместить их **выше** `location /` с `try_files`, чтобы PHP-роутер не перехватывал этот путь.
3. Для стабильности добавьте `proxy_redirect off;`.

Пример:

```nginx
server {
    listen 80;
    server_name prostochatbot.ru www.prostochatbot.ru;
    return 301 https://prostochatbot.ru$request_uri;
}

server {
    listen 443 ssl http2;
    server_name prostochatbot.ru www.prostochatbot.ru;

    root /var/www/autopost_v2/public;
    index index.php index.html;

    ssl_certificate     /etc/nginx/ssl/autopost_v2/fullchain.crt;
    ssl_certificate_key /etc/nginx/ssl/autopost_v2/certificate.key;

    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header Referrer-Policy "strict-origin-when-cross-origin" always;

    # ASP.NET Core под подпутём /tolyaprogram
    location = /tolyaprogram {
        return 301 /tolyaprogram/;
    }

    location /tolyaprogram/ {
        proxy_pass         http://127.0.0.1:5000;
        proxy_http_version 1.1;
        proxy_set_header   Host $host;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_cache_bypass $http_upgrade;
        proxy_redirect     off;
    }

    # ваш текущий PHP-сайт
    location / {
        try_files $uri $uri/ /index.php?$query_string;
    }

    location ~ /\.(?!well-known).* {
        deny all;
    }

    location ~ \.php$ {
        include snippets/fastcgi-php.conf;
        fastcgi_pass unix:/run/php/php8.3-fpm.sock;
        fastcgi_param SCRIPT_FILENAME $realpath_root$fastcgi_script_name;
        include fastcgi_params;
    }
}
```

Проверка после правок:

```bash
sudo nginx -t
sudo systemctl reload nginx
curl -I https://prostochatbot.ru/tolyaprogram/
curl -I http://127.0.0.1:5000
```

Если `curl 127.0.0.1:5000` не отвечает — сначала поднимите `mywork2-web` (см. раздел 11).


## 8) Проверка после запуска

1. `systemctl status mywork2-web` — сервис активен.
2. `curl -I http://127.0.0.1:5000` — Kestrel отвечает.
3. `curl -I http://<SERVER_IP>` — Nginx проксирует.
4. Открыть в браузере `http://<SERVER_IP>`.

---

## 9) Обновление версии приложения

### Если вы в корне репозитория

```bash
sudo systemctl stop mywork2-web
dotnet publish MyWork2.Web/MyWork2.Web.csproj -c Release -o /var/www/mywork2-web
sudo chown -R www-data:www-data /var/www/mywork2-web
sudo systemctl start mywork2-web
```

### Если вы в папке проекта (`/var/www/MyWork2.Web`)

```bash
sudo systemctl stop mywork2-web
dotnet publish MyWork2.Web.csproj -c Release -o /var/www/mywork2-web
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


5. **MSBUILD : error MSB1009: файл проекта не существует**
   - Вы запускаете команду из неверной директории или указали лишний префикс пути.
   - Пример: если текущая папка `/var/www/MyWork2.Web`, правильно так:

```bash
dotnet restore MyWork2.Web.csproj
dotnet publish MyWork2.Web.csproj -c Release -o /var/www/mywork2-web
```

   - Проверка: `find . -maxdepth 3 -name "*.csproj"`.


---

## 11) Быстрый recovery для вашего текущего состояния (service inactive/dead)

Симптомы у вас сейчас:
- `systemctl status mywork2-web` → `Active: inactive (dead)`
- `curl http://127.0.0.1:5000` → connection refused

Это значит, что сервис **не запущен** (часто не выполнены `start/enable`, либо приложение не опубликовано в путь из `ExecStart`).

Выполните строго по порядку:

```bash
# 1) Убедиться, что публикация реально есть
ls -la /var/www/mywork2-web/
ls -la /var/www/mywork2-web/MyWork2.Web.dll

# 2) Если файла нет — опубликовать заново
cd /var/www/MyWork2.Web
dotnet restore MyWork2.Web.csproj
dotnet publish MyWork2.Web.csproj -c Release -o /var/www/mywork2-web
sudo chown -R www-data:www-data /var/www/mywork2-web

# 3) Перечитать unit и сразу включить+запустить
sudo systemctl daemon-reload
sudo systemctl enable --now mywork2-web

# 4) Проверить статус и логи
systemctl status mywork2-web --no-pager
journalctl -u mywork2-web -n 100 --no-pager

# 5) Проверить локальный порт
curl -I http://127.0.0.1:5000
```

Если в статусе снова `inactive (dead)`, проверьте совпадение путей в unit-файле:

```bash
sudo systemctl cat mywork2-web
```

Должно быть:
- `WorkingDirectory=/var/www/mywork2-web`
- `ExecStart=/usr/bin/dotnet /var/www/mywork2-web/MyWork2.Web.dll`

Если меняли unit-файл вручную — снова:

```bash
sudo systemctl daemon-reload
sudo systemctl restart mywork2-web
```


---

## 12) 502 Bad Gateway — диагностика за 2 минуты

Если видите `502 Bad Gateway`, почти всегда Nginx не может достучаться до Kestrel на `127.0.0.1:5000`.

Запустите этот набор команд:

```bash
# 1) Жив ли сервис ASP.NET Core
systemctl status mywork2-web --no-pager
journalctl -u mywork2-web -n 100 --no-pager

# 2) Слушается ли порт 5000
ss -ltnp | rg ':5000' || true
curl -I http://127.0.0.1:5000
curl -I http://127.0.0.1:5000/healthz

# 3) Проверка nginx
sudo nginx -t
sudo tail -n 100 /var/log/nginx/error.log
```

### Частые причины 502
1. Сервис не запущен (`inactive/dead`) — поднимите `enable --now`, см. раздел 11.
2. Путь в `ExecStart` неверный (не найден `MyWork2.Web.dll`).
3. Приложение падает при старте из-за некорректной строки подключения к БД.
4. В Nginx location для `/tolyaprogram/` отсутствует/ошибочен `proxy_pass`.

### Быстрый фикс, если сервис не стартует

```bash
cd /var/www/MyWork2.Web
dotnet restore MyWork2.Web.csproj
dotnet publish MyWork2.Web.csproj -c Release -o /var/www/mywork2-web
sudo chown -R www-data:www-data /var/www/mywork2-web
sudo systemctl daemon-reload
sudo systemctl enable --now mywork2-web
```

После этого ещё раз:

```bash
curl -I http://127.0.0.1:5000/healthz
curl -I https://prostochatbot.ru/tolyaprogram/
```


---

## 13) Если 502 остаётся после publish/start (ваш текущий кейс)

Вы уже сделали `restore/publish/enable --now`, но 502 всё ещё есть. Значит, нужна точечная проверка активной конфигурации и процесса.

### Шаг A. Проверить, что Nginx реально использует нужный `location /tolyaprogram/`

```bash
sudo nginx -T | rg -n "server_name|tolyaprogram|proxy_pass|listen 443"
```

Ищем в **активном** конфиге:
- `location /tolyaprogram/ { ... }`
- `proxy_pass http://127.0.0.1:5000;`

Если этого нет в выводе `nginx -T`, вы редактировали не тот файл.

### Шаг B. Проверить backend локально (до Nginx)

```bash
systemctl status mywork2-web --no-pager
journalctl -u mywork2-web -n 200 --no-pager
ss -ltnp | rg ':5000' || true
curl -v http://127.0.0.1:5000/healthz
```

Если тут ошибка — это не Nginx-проблема, это backend/service.

### Шаг C. Запустить приложение вручную от того же пользователя, что в systemd

```bash
sudo -u www-data /usr/bin/dotnet /var/www/mywork2-web/MyWork2.Web.dll
```

Если приложение падает, ошибка сразу будет в консоли (обычно connection string/доступ к БД/конфиг).

### Шаг D. Проверить точный unit-файл и окружение

```bash
sudo systemctl cat mywork2-web
```

Проверьте обязательно:
- `WorkingDirectory=/var/www/mywork2-web`
- `ExecStart=/usr/bin/dotnet /var/www/mywork2-web/MyWork2.Web.dll`
- `Environment=ASPNETCORE_URLS=http://127.0.0.1:5000`

### Шаг E. Проверить логи Nginx именно в момент запроса

```bash
sudo tail -f /var/log/nginx/error.log /var/log/nginx/access.log
```

В другом окне:

```bash
curl -vk https://prostochatbot.ru/tolyaprogram/
```

По строке в `error.log` обычно сразу видно причину:
- `connect() failed (111: Connection refused)` → backend не слушает 5000.
- `upstream sent too big header` → увеличить `proxy_buffer_size` и related buffers.
- `no live upstreams` → ошибочная upstream-конфигурация.

### Быстрый hard-restart цикл

```bash
sudo systemctl restart mywork2-web
sleep 2
systemctl status mywork2-web --no-pager
curl -I http://127.0.0.1:5000/healthz
sudo nginx -t
sudo systemctl reload nginx
curl -I https://prostochatbot.ru/tolyaprogram/
```
