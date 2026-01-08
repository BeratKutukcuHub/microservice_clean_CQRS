.PHONY: help build up down restart logs clean ps health

help: 
	@echo 'Usage: make [target]'
	@echo ''
	@echo 'Available targets:'
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## / {printf "  %-15s %s\n", $$1, $$2}' $(MAKEFILE_LIST)

build: 
	docker-compose build

up:
	docker-compose up -d

down:
	docker-compose down

restart: 
	docker-compose restart

logs: 
	docker-compose logs -f

logs-identity: 
	docker-compose logs -f identity-service

logs-product: 
	docker-compose logs -f product-service

logs-category:
	docker-compose logs -f category-service

logs-userprofile:
	docker-compose logs -f userprofile-service

logs-mail:
	docker-compose logs -f mailnotification-service

logs-gateway: 
	docker-compose logs -f api-gateway

ps: 
	docker-compose ps

health: 
	@echo "Checking service health..."
	@curl -f http://localhost:5000/health || echo "API Gateway: DOWN"
	@curl -f http://localhost:5001/health || echo "Identity Service: DOWN"
	@curl -f http://localhost:5002/health || echo "Product Service: DOWN"
	@curl -f http://localhost:5003/health || echo "Category Service: DOWN"
	@curl -f http://localhost:5004/health || echo "UserProfile Service: DOWN"
	@curl -f http://localhost:5005/health || echo "MailNotification Service: DOWN"

clean: ## Remove all containers, networks, and volumes
	docker-compose down -v
	docker system prune -f

rebuild: ## Rebuild and restart all services
	docker-compose down
	docker-compose build
	docker-compose up -d

rebuild-identity: ## Rebuild and restart identity service
	docker-compose build identity-service
	docker-compose up -d identity-service

rebuild-product: ## Rebuild and restart product service
	docker-compose build product-service
	docker-compose up -d product-service

rebuild-category: ## Rebuild and restart category service
	docker-compose build category-service
	docker-compose up -d category-service

rebuild-userprofile: ## Rebuild and restart userprofile service
	docker-compose build userprofile-service
	docker-compose up -d userprofile-service

rebuild-mail: ## Rebuild and restart mail notification service
	docker-compose build mailnotification-service
	docker-compose up -d mailnotification-service

rebuild-gateway: ## Rebuild and restart api gateway
	docker-compose build api-gateway
	docker-compose up -d api-gateway

stop: ## Stop all services without removing
	docker-compose stop

start: ## Start all stopped services
	docker-compose start

mongodb-shell: ## Connect to MongoDB shell
	docker-compose exec mongodb mongosh -u admin -p admin123 --authenticationDatabase admin

rabbitmq-ui: ## Open RabbitMQ Management UI
	@echo "Opening RabbitMQ Management UI at http://localhost:15672"
	@echo "Username: guest, Password: guest"

swagger-gateway: ## Open API Gateway Swagger
	@echo "Opening API Gateway Swagger at http://localhost:5000/swagger"

swagger-identity: ## Open Identity Service Swagger
	@echo "Opening Identity Service Swagger at http://localhost:5001/swagger"

swagger-product: ## Open Product Service Swagger
	@echo "Opening Product Service Swagger at http://localhost:5002/swagger"

swagger-category: ## Open Category Service Swagger
	@echo "Opening Category Service Swagger at http://localhost:5003/swagger"

swagger-userprofile: ## Open UserProfile Service Swagger
	@echo "Opening UserProfile Service Swagger at http://localhost:5004/swagger"

swagger-mail: ## Open MailNotification Service Swagger
	@echo "Opening MailNotification Service Swagger at http://localhost:5005/swagger"
