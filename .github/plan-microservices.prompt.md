# Plan: Monolith to Microservices Migration for WebApiShop

## TL;DR
Convert the monolithic WebAPI into **4 independent REST microservices** (User, Catalog, Order, Analytics) with separate databases. Use Docker Compose for deployment. Migrate in phases using a **strangler pattern** to run old and new code in parallel, reducing risk.

---

## Architecture Decision Summary
- **Services**: 4 coarse-grained services
- **Databases**: One per service (SQL Server or PostgreSQL)
- **Communication**: Synchronous REST/HTTP (with upgrade path to async)
- **Migration**: Phased strangler pattern (old monolith + new service in parallel)
- **Deployment**: Docker Compose (Docker host per service)
- **Scaling**: Independent service scaling, separate deployment pipelines

---

## Identified Microservices

### 1. **User Service** ⭐ (Extract First - Least Dependencies)
- **Responsibility**: User CRUD, authentication, password validation
- **Current Owner Code**: UsersController, UsersService, PasswordsService, UsersRepository, Password entity
- **External Dependencies**: None (independent)
- **Data Owned**: Users table
- **API Endpoints**: GET/POST /api/users, POST /api/users/login, PUT /api/users/{id}, POST /api/passwords/validate
- **Database**: Dedicated (SQL Server or PostgreSQL)

### 2. **Catalog Service** ⭐⭐ (Extract Second)
- **Responsibility**: Product and Category management, browsing/filtering
- **Current Owner Code**: ProductsController, CategoriesController, ProductsService, CategoriesService, ProductsRepository, CategoriesRepository, Product, Category entities
- **External Dependencies**: None (independent)
- **Data Owned**: Products, Categories tables
- **API Endpoints**: GET/POST /api/products, GET/POST /api/categories, PUT /api/products/{id}
- **Database**: Dedicated (SQL Server or PostgreSQL)
- **Scaling Need**: High (frequent reads, filtering)

### 3. **Order Service** ⭐⭐⭐ (Extract Third)
- **Responsibility**: Order CRUD, order validation
- **Current Owner Code**: OrdersController, OrdersService, OrdersRepository, Order, OrderItem entities
- **External Dependencies**: **Catalog Service** (to fetch product prices for validation)
- **Data Owned**: Orders, OrderItems tables
- **API Endpoints**: GET/POST /api/orders, GET /api/orders/{id}
- **Database**: Dedicated (SQL Server or PostgreSQL)
- **Critical Integration**: Must call Catalog Service to validate product prices before order creation

### 4. **Analytics Service** (Optional, Extract Last)
- **Responsibility**: Track API usage metrics (requests, endpoints, user agents)
- **Current Owner Code**: RatingMiddleware, RatingService, RatingRepository, Rating entity
- **External Dependencies**: None (event-driven, observes traffic)
- **Data Owned**: Ratings table
- **Note**: Could be removed entirely if not critical; currently integrated via middleware
- **Database**: Dedicated (could use simpler DB, e.g., MongoDB or SQLite)

---

## Migration Phases

### Phase 0: Preparation & Infrastructure Setup
**Duration**: ~3-5 days  
**Blockers**: None  
**Deliverables**: Docker setup, API Gateway ready

1. **Set up containerization**
   - Create Dockerfile for each new service (based on current .NET project)
   - Create docker-compose.yml with 4 services (+ SQL Server)
   - Test local Docker builds

2. **Set up API Gateway** (reverse proxy)
   - Use NGINX or ASP.NET Core middleware to route requests
   - Gateway routes `/api/users/*` → User Service
   - Gateway routes `/api/products/*` → Catalog Service
   - Gateway routes `/api/orders/*` → Order Service
   - Gateway routes `/api/ratings/*` → Analytics Service

3. **Design inter-service REST contracts** (HTTP client patterns)
   - Catalog Service → expose GET /api/internal/products/{id} (for Order Service to fetch prices)
   - Each service defines public vs internal endpoints

4. **Set up shared logging & monitoring** (optional but recommended)
   - Centralized logging (e.g., Serilog → file/database)
   - Service health check endpoints (/health)

---

### Phase 1: Extract User Service
**Duration**: ~5-7 days  
**Depends on**: Phase 0  
**Parallel with**: Monolith still running (strangler pattern)

1. **Create new User Service project**
   - New ASP.NET Core web project: `WebApiShop.Services.User`
   - Copy relevant entities: User, Password
   - Copy services: UsersService, IUsersService, PasswordsService, IPasswordsService
   - Create new DbContext: `UserServiceContext` (only Users table)
   - Implement repositories: UsersRepository, IUsersRepository

2. **Implement User Service API**
   - Controllers: UsersController, PasswordsController
   - Copy AutoMapper profiles for User DTOs
   - Implement login logic with JWT tokens (optional enhancement for service-to-service auth)

3. **Set up User Service database**
   - Create separate SQL Server instance/database
   - Run migrations to populate Users table (copy from monolith)

4. **Update API Gateway**
   - Route requests to new User Service
   - Keep monolith User Service as fallback (if new service down, requests go to old)

5. **Testing & Validation**
   - Integration tests for User Service endpoints
   - Test fallback behavior in Gateway

---

### Phase 2: Extract Catalog Service
**Duration**: ~5-7 days  
**Depends on**: Phase 0  
**Parallel with**: Phase 1 + Monolith

1. **Create new Catalog Service project**
   - New ASP.NET Core web project: `WebApiShop.Services.Catalog`
   - Copy entities: Product, Category
   - Copy services & repositories: ProductsService, CategoriesService, and their repositories
   - Create CatalogServiceContext (Products, Categories tables only)

2. **Implement Catalog Service API**
   - Controllers: ProductsController, CategoriesController
   - Filtering, pagination, search endpoints
   - **Key endpoint**: `GET /api/internal/products/{id}` for Order Service (returns product with price)

3. **Set up Catalog Service database**
   - Separate SQL Server instance/database
   - Copy Products, Categories tables from monolith

4. **Update API Gateway**
   - Route /api/products/* and /api/categories/* to Catalog Service

5. **Testing**
   - Integration tests
   - Test internal endpoint for Order Service consumption

---

### Phase 3: Extract Order Service
**Duration**: ~7-10 days  
**Depends on**: Phase 0, Phase 1, Phase 2  
**Parallel with**: All previous services + Monolith

1. **Create new Order Service project**
   - New ASP.NET Core web project: `WebApiShop.Services.Orders`
   - Copy entities: Order, OrderItem
   - Copy services & repositories: OrdersService, OrdersRepository
   - Create OrderServiceContext (Orders, OrderItems tables only)

2. **Implement inter-service communication**
   - Order Service needs to call Catalog Service to fetch product prices
   - Create `IProductsServiceClient` (HTTP client)
   - Implement OrdersService.orderSumValidation() to call Catalog Service

3. **Implement Order Service API**
   - Controllers: OrdersController
   - Endpoints: GET /api/orders, POST /api/orders, GET /api/orders/{id}
   - Enhance validation with remote product fetch

4. **Set up Order Service database**
   - Separate SQL Server instance/database
   - Copy Orders, OrderItems tables

5. **Update API Gateway**
   - Route /api/orders/* to Order Service

6. **Error Handling**
   - If Catalog Service is down, fail order creation with meaningful error
   - Implement retry logic with exponential backoff

7. **Testing**
   - Unit tests for orderSumValidation (with mocked Catalog client)
   - Integration tests with actual Catalog Service
   - Chaos testing (Catalog Service fails)

---

### Phase 4: Extract Analytics Service (Optional)
**Duration**: ~3-5 days  
**Depends on**: Phase 0  
**Parallel with**: All others

1. **Create Analytics Service project**
   - Separate ASP.NET Core service
   - RatingService, RatingRepository
   - SimpleDbContext (Ratings table only)

2. **Decouple from middleware**
   - Extract RatingMiddleware → standalone Analytics Service
   - Call Analytics Service via HTTP POST instead of DI

3. **Set up Analytics database**
   - Separate database (could use MongoDB instead of SQL for flexibility)

4. **Testing & Optional Deprecation**
   - If not heavily used, can skip or mark for future removal

---

### Phase 5: Decompose Monolith & Finalize
**Duration**: ~5-7 days  
**Depends on**: Phase 1, 2, 3, 4

1. **Remove extracted services from monolith**
   - Delete User service code
   - Delete Catalog service code
   - Delete Order service code
   - Delete Analytics service code

2. **Update monolith**
   - Now only contains: API Gateway logic, orchestration
   - Or reduce monolith to a shell and deprecate it

3. **Configure Docker Compose**
   - All 4 services + databases running together
   - Health checks enabled
   - Logging aggregation

4. **Production deployment readiness**
   - Load testing across services
   - Cross-service failure scenarios
   - Monitoring & alerting setup

5. **Decommission old monolith**
   - Archive code
   - Redirect all traffic through microservices

---

## Relevant Files to Modify/Create

### New Service Projects (Create These)
- **User Service**: `WebApiShop.Services.User/` (copy from WebApiShop.Services + DTOs structure)
- **Catalog Service**: `WebApiShop.Services.Catalog/` (copy ProductsService, CategoriesService, related code)
- **Order Service**: `WebApiShop.Services.Orders/` (copy OrdersService + add inter-service client)
- **Analytics Service**: `WebApiShop.Services.Analytics/` (copy RatingService)

### Core Files to Modify
- `docker-compose.yml` — Add service definitions, databases, networking
- `WebApiShop.sln` — Add new service projects
- `WebApiShop/Program.cs` — Convert to API Gateway (remove service registrations)
- `WebApiShop/Controllers/` — Replace controller implementations with gateway logic or delete controllers (route to services instead)

### New Files to Create
- `Dockerfile` (one per service: User, Catalog, Order, Analytics)
- `nginx.conf` or API Gateway middleware (route requests)
- `.env` file (database connection strings, service URLs)
- Service-to-service client interfaces (e.g., `IProductsServiceClient` in Order Service)
- Integration test projects for each service

### Database Migration Scripts
- User Service DB initialization (Users table)
- Catalog Service DB initialization (Products, Categories tables)
- Order Service DB initialization (Orders, OrderItems tables)
- Analytics Service DB initialization (Ratings table)

---

## Verification Steps

### Per-Phase Verification
1. **After Phase 0**: Docker Compose runs, API Gateway responds to requests
2. **After Phase 1**: User Service endpoints return same data as monolith; gateway correctly routes user requests
3. **After Phase 2**: Catalog Service serves products/categories; filtering/pagination works identically
4. **After Phase 3**: Order Service creates orders; validates against live Catalog Service data
5. **After Phase 4**: Analytics Service tracks requests independently
6. **After Phase 5**: All services running; monolith decommissioned; load testing passes

### Automated Tests
- **Per service**: Unit tests for business logic (exist from monolith, migrate code)
- **Integration tests**: Each service with its own database
- **Contract tests**: Order Service ↔ Catalog Service inter-service calls
- **End-to-end tests**: Test full request path through API Gateway

### Manual Validation
- Curl/Postman: Test all endpoints on each service
- Check database consistency after migrations
- Verify no data loss during extraction
- Load test with realistic traffic patterns

---

## Decisions Made
1. **REST over gRPC**: Simpler to implement, already HTTP-based, can upgrade later
2. **Phased extraction**: Reduces risk, allows parallel running during transition
3. **Dedicated databases**: Enforces service boundaries, easier scaling
4. **Docker Compose**: Suitable for dev/small scale, can migrate to Kubernetes later
5. **Coarse granularity**: 4 services vs 6+ avoids over-engineering, maintains manageable complexity

---

## Further Considerations

### 1. Handling User Service Dependencies
**Current Issue**: User Service doesn't depend on others, but how does Order Service know which user placed an order?  
**Recommendation**: Order Service stores `UserId` (FK) without foreign key constraint; trusts User Service as source of truth. Order Service can optionally validate user exists before creating order (call User Service).

### 2. Async Communication (Future Enhancement)
**Current**: Synchronous REST calls (Order → Catalog)  
**Future**: Consider message queue (RabbitMQ, Azure Service Bus) for:
- Order placed → publish event → update inventory
- Scales better under high load
- Better failure isolation

### 3. Shared Auth/Token Strategy
**Current**: Each service has password validation  
**Enhancement**: Implement JWT tokens. User Service issues token on login; other services validate token (no per-service password checks needed).

### 4. Logging & Tracing Across Services
**Critical**: With 4 services, need distributed tracing (e.g., correlation IDs, OpenTelemetry) to debug request flows.  
**Recommendation**: Implement early (Phase 0) before complexity grows.

### 5. API Gateway Responsibilities
**Decision needed**: Should gateway do:
- Simple routing only (current plan) ✓
- Authentication/token validation (centralized)
- Rate limiting per service
- Request/response logging

---

## Success Criteria
- [ ] Each service deploys independently
- [ ] Services can scale separately (e.g., Catalog up 3x during sales)
- [ ] Code changes in one service don't require redeployment of others
- [ ] Data integrity maintained (ACID per-service, eventual consistency across services)
- [ ] All tests passing (unit, integration, end-to-end)
- [ ] Monitoring/alerting working per service
- [ ] Monolith decommissioned
