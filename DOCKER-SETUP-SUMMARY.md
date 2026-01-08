# Docker Setup - Özet

## Oluşturulan Dosyalar

### Ana Dosyalar
- ✅ `docker-compose.yml` - Tüm servislerin orchestration dosyası
- ✅ `.dockerignore` - Docker build'den hariç tutulacak dosyalar
- ✅ `README.Docker.md` - Detaylı Docker dokümantasyonu
- ✅ `QUICKSTART.md` - Hızlı başlangıç rehberi
- ✅ `Makefile` - Linux/Mac için komut kısayolları
- ✅ `docker-commands.ps1` - Windows için PowerShell script

### Dockerfile'lar
- ✅ `src/IdentityService/Dockerfile`
- ✅ `src/ProductService/Dockerfile`
- ✅ `src/CategoryService/Dockerfile`
- ✅ `src/UserProfileService/Dockerfile`
- ✅ `src/MailNotificationService/Dockerfile`
- ✅ `src/ApiGateway/Dockerfile`

### Konfigürasyon
- ✅ `src/ApiGateway/ocelot.Docker.json` - Docker için Ocelot konfigürasyonu

## Servis Portları

| Servis | Host Port | Container Port |
|--------|-----------|----------------|
| API Gateway | 5000 | 8080 |
| Identity Service | 5001 | 8080 |
| Product Service | 5002 | 8080 |
| Category Service | 5003 | 8080 |
| UserProfile Service | 5004 | 8080 |
| MailNotification Service | 5005 | 8080 |
| MongoDB | 27017 | 27017 |
| RabbitMQ | 5672 | 5672 |
| RabbitMQ Management | 15672 | 15672 |
| Redis | 6379 | 6379 |

## Hızlı Başlatma

### Windows
```powershell
# Servisleri başlat
.\docker-commands.ps1 up

# Swagger'ı aç
.\docker-commands.ps1 swagger-gateway

# Logları izle
.\docker-commands.ps1 logs

# Servisleri durdur
.\docker-commands.ps1 down
```

### Linux/Mac
```bash
# Servisleri başlat
make up

# Logları izle
make logs

# Servisleri durdur
make down
```

### Manuel (Tüm Platformlar)
```bash
# Servisleri başlat
docker-compose up -d

# Logları izle
docker-compose logs -f

# Servisleri durdur
docker-compose down
```

## Önemli URL'ler

### Swagger Dokümantasyonları
- API Gateway: http://localhost:5000/swagger
- Identity Service: http://localhost:5001/swagger
- Product Service: http://localhost:5002/swagger
- Category Service: http://localhost:5003/swagger
- UserProfile Service: http://localhost:5004/swagger
- MailNotification Service: http://localhost:5005/swagger

### Infrastructure
- RabbitMQ Management: http://localhost:15672 (guest/guest)
- MongoDB: mongodb://admin:admin123@localhost:27017

## Veritabanı Konfigürasyonu

### MongoDB
- **Username**: admin
- **Password**: admin123
- **Databases**:
  - IdentityServiceDb
  - ProductServiceDb
  - CategoryServiceDb
  - UserProfileServiceDb

### RabbitMQ
- **Username**: guest
- **Password**: guest
- **Exchanges**:
  - product_exchange
  - category_exchange
  - userprofile_exchange
  - mailnotification_exchange

## Docker Compose Özellikleri

### Infrastructure Services
- **MongoDB 7.0**: Tüm servislerin veritabanı
- **RabbitMQ 3.12**: Message broker (management plugin aktif)
- **Redis 7.2**: Cache servisi

### Microservices
Tüm servisler:
- .NET 9.0 runtime
- Health check endpoint'leri
- Otomatik restart
- Shared network (microservices-network)
- Environment variable'lar ile konfigürasyon

### Volumes
- `mongodb_data`: MongoDB verileri
- `rabbitmq_data`: RabbitMQ verileri
- `redis_data`: Redis verileri

## Özellikler

### Health Checks
Her servis için health check yapılandırılmış:
- Interval: 30 saniye
- Timeout: 10 saniye
- Retries: 5
- Start period: 40 saniye

### Dependencies
Servisler doğru sırada başlar:
1. Infrastructure (MongoDB, RabbitMQ, Redis)
2. Microservices (Identity, Product, Category, UserProfile, Mail)
3. API Gateway

### Networking
Tüm servisler `microservices-network` bridge network'ünde:
- Servisler birbirlerine container name ile erişir
- Örnek: `mongodb`, `rabbitmq`, `identity-service`

## Geliştirme İpuçları

### Tek Bir Servisi Yeniden Build Etme
```bash
# Windows
.\docker-commands.ps1 rebuild-identity

# Linux/Mac
make rebuild-identity

# Manuel
docker-compose build identity-service
docker-compose up -d identity-service
```

### Logları Filtreleme
```bash
# Sadece hataları göster
docker-compose logs | grep -i error

# Son 100 satır
docker-compose logs --tail=100

# Belirli bir servisten son 50 satır
docker-compose logs --tail=50 identity-service
```

### Container İçine Girme
```bash
docker-compose exec identity-service /bin/bash
```

### Database Backup
```bash
# MongoDB backup
docker-compose exec mongodb mongodump --username admin --password admin123 --authenticationDatabase admin --out /backup

# Backup'ı host'a kopyala
docker cp mongodb:/backup ./mongodb-backup
```

## Sorun Giderme

### Servis Başlamıyor
```bash
# 1. Logları kontrol et
docker-compose logs service-name

# 2. Container'ı yeniden başlat
docker-compose restart service-name

# 3. Container'ı yeniden build et
docker-compose build service-name
docker-compose up -d service-name
```

### Port Çakışması
docker-compose.yml'de port mapping'i değiştir:
```yaml
ports:
  - "5101:8080"  # 5001 yerine 5101 kullan
```

### Tüm Servisleri Temizleme
```bash
# Tüm container, network ve volume'leri sil
docker-compose down -v

# Docker cache'i temizle
docker system prune -a --volumes
```

## Mail Servisi Konfigürasyonu

docker-compose.yml'de SMTP ayarlarını güncelleyin:

```yaml
mailnotification-service:
  environment:
    - SmtpSettings__Host=smtp.gmail.com
    - SmtpSettings__Port=587
    - SmtpSettings__EnableSsl=true
    - SmtpSettings__Username=your-email@gmail.com
    - SmtpSettings__Password=your-app-password
```

## Production Notları

Production ortamı için:

1. **Secrets**: Environment variable'ları secret management ile değiştir
2. **Volumes**: Production volume'ları için backup stratejisi oluştur
3. **Monitoring**: Prometheus/Grafana ekle
4. **Logging**: ELK Stack veya Seq ekle
5. **Security**: Network policies ve firewall kuralları ekle
6. **Scaling**: Kubernetes'e geçiş için hazırlık yap

## Sonraki Adımlar

1. ✅ Servisleri başlat: `docker-compose up -d`
2. ✅ Health check'leri kontrol et
3. ✅ Swagger dokümantasyonlarını incele
4. ✅ RabbitMQ Management'ı aç
5. ✅ İlk API çağrılarını yap
6. ✅ Event'lerin RabbitMQ'da publish edildiğini kontrol et
7. ✅ MongoDB'de verileri kontrol et

## Yardım ve Dokümantasyon

- **Hızlı Başlangıç**: [QUICKSTART.md](QUICKSTART.md)
- **Detaylı Dokümantasyon**: [README.Docker.md](README.Docker.md)
- **Komut Yardımı**: 
  - Windows: `.\docker-commands.ps1 help`
  - Linux/Mac: `make help`


---

## Çözülen Sorunlar (08 Ocak 2026)

### 1. JWT Konfigürasyon Hatası
**Sorun:** Servisler "Section 'JwtOptions' not found" hatası ile başlamıyordu.

**Kök Neden:** 
- `SecretProvider` sınıfı `IConfiguration` instance'ı olmadan oluşturuluyordu
- Bu yüzden sadece `secretbase.json` dosyasını arıyordu, appsettings.json ve environment variable'ları okumuyordu

**Çözüm:**
- `AddDICommonAuthentication()` metoduna `IConfiguration` parametresi eklendi
- Tüm servislerin `Program.cs` dosyaları güncellendi: `builder.Services.AddDICommonAuthentication(builder.Configuration)`
- Tüm servislerin appsettings.json dosyalarına `JwtOptions` section'ı eklendi:
  ```json
  "JwtOptions": {
    "SecretKey": "default-secret-key",
    "Issuer": "CleanArchitecture",
    "Audience": "CleanArchitecture"
  }
  ```

**Değiştirilen Dosyalar:**
- `AbstractionBlocks/Common.Authentication/Extensions/ServiceCollectionExtensions.cs`
- `src/IdentityService/Identity.Api/Program.cs`
- `src/IdentityService/Identity.Api/appsettings.json`
- `src/ProductService/Product.Api/Program.cs`
- `src/ProductService/Product.Api/appsettings.json`
- `src/CategoryService/Category.Api/Program.cs`
- `src/CategoryService/Category.Api/appsettings.json`
- `src/UserProfileService/UserProfile.Api/appsettings.json`

### 2. Environment Variable Konfigürasyonu
**Sorun:** Özel karakterler (ünlem işareti) içeren environment variable'lar doğru şekilde set edilmiyordu.

**Çözüm:**
- JWT secret key'lerden özel karakterler kaldırıldı
- docker-compose.yml'de tüm servisler için tutarlı JWT konfigürasyonu eklendi:
  ```yaml
  - JwtOptions__SecretKey=DBLOGHANDLEYourSecretKey123456789
  - JwtOptions__Issuer=CleanArchitecture
  - JwtOptions__Audience=CleanArchitecture
  ```

### Test Sonuçları

Tüm endpoint'ler başarıyla çalışıyor:
- ✅ `http://localhost:5000/gateway/product` - 200 OK
- ✅ `http://localhost:5000/gateway/categories` - 200 OK
- ✅ `http://localhost:5000/gateway/userprofile` - 200 OK
- ✅ `http://localhost:5000/swagger/index.html` - 200 OK

Tüm servisler sağlıklı çalışıyor:
- ✅ MongoDB (port 27017)
- ✅ RabbitMQ (ports 5672, 15672) - healthy
- ✅ Redis (port 6379) - healthy
- ✅ Identity Service (port 5001)
- ✅ Product Service (port 5002)
- ✅ Category Service (port 5003)
- ✅ UserProfile Service (port 5004)
- ✅ Mail Notification Service (port 5005)
- ✅ API Gateway (port 5000)
