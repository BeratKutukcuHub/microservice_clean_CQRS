# Quick Start Guide

## Hızlı Başlangıç (5 Dakika)

### Windows

```powershell
# 1. Tüm servisleri başlat
.\docker-commands.ps1 up

# 2. Servislerin durumunu kontrol et
.\docker-commands.ps1 ps

# 3. API Gateway Swagger'ı aç
.\docker-commands.ps1 swagger-gateway
```

### Linux/Mac

```bash
# 1. Tüm servisleri başlat
make up

# 2. Servislerin durumunu kontrol et
make ps

# 3. Logları izle
make logs
```

### Manuel (Tüm Platformlar)

```bash
# 1. Tüm servisleri başlat
docker-compose up -d

# 2. Servislerin durumunu kontrol et
docker-compose ps

# 3. Logları izle
docker-compose logs -f
```

## İlk API Çağrısı

### 1. Kullanıcı Oluştur (Identity Service)

```bash
curl -X POST "http://localhost:5000/gateway/identityuser" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test User",
    "email": "test@example.com",
    "password": "Test123!"
  }'
```

### 2. Kategori Oluştur (Category Service)

```bash
curl -X POST "http://localhost:5000/gateway/categories" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Electronics",
    "description": "Electronic products"
  }'
```

### 3. Ürün Oluştur (Product Service)

```bash
curl -X POST "http://localhost:5000/gateway/product" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop",
    "description": "Gaming Laptop",
    "price": 1500.00,
    "stock": 10,
    "categoryId": "category-id-from-step-2"
  }'
```

## Swagger UI Erişimi

Tüm servislerin API dokümantasyonuna Swagger üzerinden erişebilirsiniz:

- **API Gateway**: http://localhost:5000/swagger
- **Identity Service**: http://localhost:5001/swagger
- **Product Service**: http://localhost:5002/swagger
- **Category Service**: http://localhost:5003/swagger
- **UserProfile Service**: http://localhost:5004/swagger
- **MailNotification Service**: http://localhost:5005/swagger

## RabbitMQ Management

RabbitMQ yönetim paneline erişim:

- **URL**: http://localhost:15672
- **Username**: guest
- **Password**: guest

## MongoDB Bağlantısı

MongoDB'ye bağlanmak için:

```bash
# Connection String
mongodb://admin:admin123@localhost:27017

# Docker container içinden
docker-compose exec mongodb mongosh -u admin -p admin123 --authenticationDatabase admin
```

## Servisleri Durdurma

### Windows
```powershell
.\docker-commands.ps1 down
```

### Linux/Mac
```bash
make down
```

### Manuel
```bash
docker-compose down
```

## Sorun Giderme

### Servisler başlamıyor?

```bash
# Logları kontrol et
docker-compose logs -f

# Belirli bir servisin loglarını kontrol et
docker-compose logs -f identity-service
```

### Port çakışması?

docker-compose.yml dosyasındaki portları değiştirin:

```yaml
services:
  identity-service:
    ports:
      - "5001:8080"  # Sol tarafı değiştirin (örn: "5101:8080")
```

### Veritabanı bağlantı hatası?

```bash
# MongoDB'nin çalıştığından emin olun
docker-compose ps mongodb

# MongoDB'yi yeniden başlatın
docker-compose restart mongodb

# Servisleri yeniden başlatın
docker-compose restart
```

### Tüm servisleri temizleme

```bash
# Tüm container, network ve volume'leri sil
docker-compose down -v

# Docker cache'i temizle
docker system prune -a --volumes
```

## Sonraki Adımlar

1. **API Gateway** üzerinden tüm servislere erişin
2. **RabbitMQ Management** panelinden mesaj kuyruklarını izleyin
3. **MongoDB** veritabanlarını inceleyin
4. **Swagger** dokümantasyonlarını keşfedin
5. Kendi endpoint'lerinizi test edin

## Daha Fazla Bilgi

Detaylı dokümantasyon için [README.Docker.md](README.Docker.md) dosyasına bakın.

## Yardım

### Windows
```powershell
.\docker-commands.ps1 help
```

### Linux/Mac
```bash
make help
```

## Önemli Notlar

- İlk başlatmada servisler build edildiği için 5-10 dakika sürebilir
- Tüm servisler hazır olana kadar 1-2 dakika bekleyin
- Health check'ler servislerin hazır olduğunu gösterir
- Mail servisi için SMTP ayarlarını docker-compose.yml'de yapılandırın
