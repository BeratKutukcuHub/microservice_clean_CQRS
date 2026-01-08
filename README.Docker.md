# Clean Architecture Microservices - Docker Deployment

Bu proje, Clean Architecture ve DDD prensipleriyle geliştirilmiş bir mikroservis mimarisidir.

## Servisler

### Infrastructure Services
- **MongoDB** (Port: 27017) - Tüm servislerin veritabanı
- **RabbitMQ** (Port: 5672, Management: 15672) - Message broker
- **Redis** (Port: 6379) - Cache servisi

### Microservices
- **API Gateway** (Port: 5000) - Ocelot API Gateway
- **Identity Service** (Port: 5001) - Kullanıcı kimlik doğrulama ve yetkilendirme
- **Product Service** (Port: 5002) - Ürün yönetimi
- **Category Service** (Port: 5003) - Kategori yönetimi
- **UserProfile Service** (Port: 5004) - Kullanıcı profil yönetimi
- **MailNotification Service** (Port: 5005) - E-posta bildirimleri

## Gereksinimler

- Docker Desktop 4.0+
- Docker Compose 2.0+
- En az 4GB RAM

## Kurulum ve Çalıştırma

### 1. Tüm Servisleri Başlatma

```bash
docker-compose up -d
```

### 2. Servisleri İzleme

```bash
# Tüm servislerin loglarını görüntüleme
docker-compose logs -f

# Belirli bir servisin loglarını görüntüleme
docker-compose logs -f identity-service
docker-compose logs -f product-service
```

### 3. Servis Durumlarını Kontrol Etme

```bash
docker-compose ps
```

### 4. Servisleri Durdurma

```bash
# Servisleri durdur (veriler korunur)
docker-compose stop

# Servisleri durdur ve kaldır (veriler korunur)
docker-compose down

# Servisleri durdur, kaldır ve volumeleri sil (TÜM VERİLER SİLİNİR!)
docker-compose down -v
```

### 5. Servisleri Yeniden Build Etme

```bash
# Tüm servisleri yeniden build et
docker-compose build

# Belirli bir servisi yeniden build et
docker-compose build identity-service

# Build et ve başlat
docker-compose up -d --build
```

## Erişim URL'leri

### API Gateway (Swagger)
- http://localhost:5000/swagger

### Servis Swagger Dokümantasyonları
- Identity Service: http://localhost:5001/swagger
- Product Service: http://localhost:5002/swagger
- Category Service: http://localhost:5003/swagger
- UserProfile Service: http://localhost:5004/swagger
- MailNotification Service: http://localhost:5005/swagger

### Infrastructure Services
- RabbitMQ Management: http://localhost:15672 (guest/guest)
- MongoDB: mongodb://admin:admin123@localhost:27017

### Gateway Üzerinden API Erişimi
- Products: http://localhost:5000/gateway/product
- Identity Users: http://localhost:5000/gateway/identityuser
- Roles: http://localhost:5000/gateway/role
- Categories: http://localhost:5000/gateway/categories
- User Profiles: http://localhost:5000/gateway/userprofile
- Mail: http://localhost:5000/gateway/mail

## Konfigürasyon

### MongoDB
- Username: `admin`
- Password: `admin123`
- Connection String: `mongodb://admin:admin123@mongodb:27017`

### RabbitMQ
- Username: `guest`
- Password: `guest`
- Host: `rabbitmq`
- Port: `5672`

### Redis
- Host: `redis`
- Port: `6379`

### Mail Notification Service
Mail servisi için SMTP ayarlarını docker-compose.yml dosyasında güncelleyin:

```yaml
mailnotification-service:
  environment:
    - SmtpSettings__Host=smtp.gmail.com
    - SmtpSettings__Port=587
    - SmtpSettings__EnableSsl=true
    - SmtpSettings__Username=your-email@gmail.com
    - SmtpSettings__Password=your-app-password
```

## Troubleshooting

### Servis Başlamıyor
```bash
# Servis loglarını kontrol edin
docker-compose logs service-name

# Container'ı yeniden başlatın
docker-compose restart service-name
```

### MongoDB Bağlantı Hatası
```bash
# MongoDB'nin hazır olduğundan emin olun
docker-compose logs mongodb

# Servisleri yeniden başlatın
docker-compose restart identity-service product-service category-service userprofile-service
```

### RabbitMQ Bağlantı Hatası
```bash
# RabbitMQ'nun hazır olduğundan emin olun
docker-compose logs rabbitmq

# Servisleri yeniden başlatın
docker-compose restart product-service category-service userprofile-service mailnotification-service
```

### Port Çakışması
Eğer portlar kullanılıyorsa, docker-compose.yml dosyasındaki port mapping'leri değiştirin:

```yaml
services:
  identity-service:
    ports:
      - "5001:8080"  # Sol taraf host port, sağ taraf container port
```

### Tüm Servisleri Temizleme
```bash
# Tüm container, network ve volume'leri temizle
docker-compose down -v
docker system prune -a --volumes
```

## Geliştirme

### Yeni Değişiklikleri Deploy Etme
```bash
# 1. Servisi yeniden build et
docker-compose build service-name

# 2. Servisi yeniden başlat
docker-compose up -d service-name
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

## Mimari Özellikler

- **Clean Architecture**: Her servis Domain, Application, Infrastructure ve API katmanlarına ayrılmış
- **DDD (Domain-Driven Design)**: Domain entity'ler business logic içerir
- **CQRS**: Command ve Query sorumlulukları ayrılmış
- **Event-Driven**: RabbitMQ ile asenkron mesajlaşma
- **Outbox Pattern**: Domain event'ler entity'lerden raise ediliyor
- **Repository Pattern**: Veri erişim katmanı soyutlanmış
- **Unit of Work**: Transaction yönetimi
- **Validation**: FluentValidation ile input validasyonu
- **Caching**: Memory ve Redis cache desteği
- **API Gateway**: Ocelot ile merkezi API yönetimi
- **Swagger**: Tüm servislerde API dokümantasyonu

## Teknolojiler

- .NET 9.0
- MongoDB
- RabbitMQ
- Redis
- Ocelot API Gateway
- MediatR
- FluentValidation
- AutoMapper
- Docker & Docker Compose
