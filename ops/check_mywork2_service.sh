#!/usr/bin/env bash
set -u

echo "== mywork2-web quick diagnose =="
echo "[1/8] systemd status"
systemctl status mywork2-web --no-pager -l || true

echo
echo "[2/8] systemd unit state"
systemctl show mywork2-web -p FragmentPath -p UnitFileState -p ActiveState -p SubState -p ExecMainCode -p ExecMainStatus -p Result || true

echo
echo "[3/8] recent systemd logs"
journalctl -u mywork2-web -n 120 --no-pager || true

echo
echo "[4/8] port 5000 listener"
ss -ltnp | grep ':5000' || echo "No listener on 5000"

echo
echo "[5/8] local health checks"
curl -sS -m 3 -D - http://127.0.0.1:5000/healthz -o /dev/null || echo "healthz failed"
curl -sS -m 3 -D - http://127.0.0.1:5000/ -o /dev/null || echo "root failed"

echo
echo "[6/8] nginx config test"
nginx -t || true

echo
echo "[7/8] nginx effective config (filtered)"
nginx -T 2>/dev/null | grep -nE 'server_name|listen 443|tolyaprogram|proxy_pass|location = /tolyaprogram|location /tolyaprogram/' || true

echo
echo "[8/8] nginx recent error log"
tail -n 60 /var/log/nginx/error.log || true

echo
echo "Tip: if service is inactive/dead, run:"
echo "  sudo systemctl daemon-reload"
echo "  sudo systemctl reset-failed mywork2-web"
echo "  sudo systemctl restart mywork2-web"
echo "  sudo systemctl status mywork2-web --no-pager -l"
echo "  sudo -u www-data env ASPNETCORE_ENVIRONMENT=Production ASPNETCORE_URLS=http://127.0.0.1:5000 /usr/bin/dotnet /var/www/mywork2-web/MyWork2.Web.dll"
