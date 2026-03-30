# Тестирование сервера на DDoS-устойчивость

Данный набор тестов предназначен для проверки поведения сервера [LizeriumServer](../LizeriumServer) под высокой нагрузкой.

Программа генерирует **100+ запросов в секунду**, после чего сервер должен:

- ограничить нагрузку,
- либо заблокировать IP-адрес через CDN / rate limiting.

---

## Ubuntu тесты

### Apache Benchmark (ab)

#### Установка

```bash
sudo apt install apache2-utils
```

#### Запуск теста

```bash
ab -n 10000 -c 100 https://192.168.1.12:7176/
```

- `-n 10000` — общее количество запросов
- `-c 100` — количество одновременных соединений

---

<details>
<summary>Пример результата</summary>

```bash
Server Software:        Kestrel
Server Hostname:        192.168.1.12
Server Port:            7176
SSL/TLS Protocol:       TLSv1.2,ECDHE-RSA-AES256-GCM-SHA384,2048,256

Document Path:          /
Document Length:        9825 bytes

Concurrency Level:      100
Time taken for tests:   111.463 seconds
Complete requests:      10000
Failed requests:        9901
   (Connect: 0, Receive: 0, Length: 9901, Exceptions: 0)
Non-2xx responses:      9901

Requests per second:    89.72 [#/sec] (mean)
Time per request:       1114.630 ms (mean)
Time per request:       11.146 ms (mean, across all concurrent requests)

Transfer rate:          20.30 KB/sec received
```

</details>

---

### wrk

#### Запуск теста

```bash
wrk -t8 -c400 -d30s https://192.168.1.12:7176/
```

- `-t8` — количество потоков
- `-c400` — количество соединений
- `-d30s` — длительность теста

---

<details>
<summary>Пример результата</summary>

```bash
Running 30s test @ https://192.168.1.12:7176/
  8 threads and 400 connections

  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    35.35ms   12.31ms 108.00ms   63.73%
    Req/Sec   593.25    553.22     3.19k    75.13%

  141868 requests in 30.09s, 20.16MB read
  Non-2xx or 3xx responses: 141868

Requests/sec:   4714.31
Transfer/sec:   685.98KB
```

</details>

---

## Интерпретация результатов

- Большое количество `Failed requests` или `Non-2xx` — ожидаемое поведение при корректной защите
- Это означает, что:
  - сервер не обрабатывает все запросы без ограничений
  - защита (rate limit / CDN / firewall) работает

---
