
##  Cấu Trúc Dự Án

```
TestLapTrinh/
├── BlazorApp/                 # Frontend - Blazor WebAssembly
│   ├── Pages/                 # Các trang Razor
│   │   ├── Home.razor         # Trang chủ
│   │   ├── Users.razor        # Danh sách người dùng (hiển thị + tìm kiếm)
│   │   ├── UserForm.razor     # Form thêm/sửa người dùng
│   │   └── NotFound.razor     # Trang 404
│   ├── Layout/                # Bố cục ứng dụng
│   │   ├── MainLayout.razor   # Layout chính
│   │   └── NavMenu.razor      # Menu điều hướng
│   ├── Services/              # Services giao tiếp với API
│   │   └── IUserService.cs    # Interface dịch vụ người dùng
│   ├── Program.cs             # Cấu hình ứng dụng Blazor
│   └── App.razor              # Component gốc
│
├── WebApi/                    # Backend - ASP.NET Core Web API
│   ├── Controllers/           # API Controllers
│   │   └── UserController.cs  # Endpoints cho người dùng
│   ├── Data/
│   │   └── AppDbContext.cs    # Entity Framework DbContext
│   ├── DTOs/                  # Data Transfer Objects
│   │   └── UserDto.cs         # DTO cho User
│   ├── Repositories/          # Data Access Layer
│   │   ├── IUserRepository.cs # Interface
│   │   ├── UserRepository.cs  # Implement dùng LINQ
│   │   └── UserRepositorySP.cs # Implement dùng Stored Procedures
│   ├── Services/              # Business Logic Layer
│   │   ├── IUserService.cs    # Interface
│   │   └── UserService.cs     # Implement logic kinh doanh
│   ├── Migrations/            # Entity Framework Migrations
│   ├── Program.cs             # Cấu hình Web API
│   ├── appsettings.json       # Cấu hình ứng dụng
│   └── appsettings.Development.json
│
└── SharedLib/                 # Shared Library
    └── Models/
        └── User.cs            # Model User (dùng chung giữa frontend/backend)
```

---

##  Nhiệm Vụ Của Các Thành Phần

### **1. BlazorApp (Frontend)**
- **Tác dụng**: Cung cấp giao diện người dùng
- **Công nghệ**: Blazor WebAssembly
- **Chức năng chính**:
  - Hiển thị danh sách người dùng
  - Tìm kiếm người dùng theo tên hoặc mã
  - Thêm người dùng mới (mở form modal)
  - Sửa thông tin người dùng
  - Xóa người dùng
  - Giao tiếp với WebApi qua HttpClient

**Services**:
- `IUserService`: Gọi các API endpoint từ WebApi

### **2. WebApi (Backend)**
- **Tác dụng**: Xử lý business logic và cung cấp API
- **Công nghệ**: ASP.NET Core 10.0, Entity Framework Core, SQL Server
- **Thành phần chính**:

#### **Controllers** (`UserController.cs`)
- GET `/api/user` - Lấy tất cả người dùng
- GET `/api/user/{id}` - Lấy người dùng theo ID
- GET `/api/user/search?term=xxx` - Tìm kiếm người dùng
- POST `/api/user` - Tạo người dùng mới
- PUT `/api/user/{id}` - Cập nhật người dùng
- DELETE `/api/user/{id}` - Xóa người dùng

#### **Repositories** (Data Access Layer)
Có 2 cách implement có thể lựa chọn:

**a) UserRepository.cs** - Dùng LINQ
- Dùng LINQ queries để truy cập dữ liệu
- Tự động tạo SQL từ LINQ expressions
- Dễ maintain và test
- **Phù hợp**: Các truy vấn đơn giản đến trung bình

**b) UserRepositorySP.cs** - Dùng Stored Procedures
- Gọi trực tiếp các Stored Procedures
- Tối ưu hiệu năng cho truy vấn phức tạp
- Cần tạo SP trước trong SQL Server
- **Phù hợp**: Các truy vấn phức tạp, báo cáo, xử lý dữ liệu lớn

#### **Services** (`UserService.cs`)
- Chứa business logic
- Validate dữ liệu
- Gọi Repository để truy cập dữ liệu
- Ánh xạ Entity → DTO

#### **Data** (`AppDbContext.cs`)
- Cấu hình Entity Framework
- Định nghĩa DbSet
- Mapping entity-table
- Seed mock data

### **3. SharedLib (Thư Viện Chung)**
- **Models**: Chứa `User.cs` - model dùng chung giữa frontend và backend
- Giảm lặp code và đảm bảo consistency

---

##  Quy Trình Gọi API

```
BlazorApp (Frontend)
    ↓
UserService (HttpClient)
    ↓
WebApi.Controllers.UserController
    ↓
WebApi.Services.UserService
    ↓
WebApi.Repositories.IUserRepository
    ├── UserRepository (LINQ) ← hoặc
    └── UserRepositorySP (Stored Procedures)
    ↓
AppDbContext (Entity Framework)
    ↓
SQL Server Database
```

---

##  So Sánh: LINQ vs Stored Procedures

| Tiêu Chí | LINQ | Stored Procedures |
|---------|------|-----------------|
| **Vị trí** | `UserRepository.cs` | `UserRepositorySP.cs` |
| **Cách viết** | C# LINQ queries | SQL queries |
| **Performance** | Trung bình | Cao hơn (xử lý phức tạp) |
| **Maintain** | Dễ hơn (với C# developers) | Cần DBA hoặc SQL expert |
| **Type-safe** | Có (compile-time check) | Không (runtime) |
| **Bind SQL Injection** | Tự bảo vệ | Cần cẩn thận với parameters |
| **Version Control** | Theo dõi dễ | Khó theo dõi (ngoài code) |
| **Usecase** | Truy vấn đơn-trung bình | Truy vấn phức, báo cáo, bulk operations |

---

## 🛠️ Hướng Dẫn Chuyển Đổi Giữa LINQ và Stored Procedures

### **Bước 1: Chọn Repository Nào Dùng**

Mở file [WebApi/Program.cs](WebApi/Program.cs) tại dòng khoảng **22**:

```csharp
// Hiện tại dùng LINQ
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Để chuyển sang Stored Procedures, thay đổi thành:
builder.Services.AddScoped<IUserRepository, UserRepositorySP>();
```

### **Bước 2: Thêm Stored Procedures vào Database**

Nếu muốn dùng `UserRepositorySP`, cần tạo các SP trong SQL Server:

```sql
-- SP lấy tất cả người dùng
CREATE PROCEDURE sp_GetAllUsers
AS
BEGIN
    SELECT * FROM Users ORDER BY Code
END

-- SP lấy người dùng theo ID
CREATE PROCEDURE sp_GetUserById
    @UserId INT
AS
BEGIN
    SELECT * FROM Users WHERE Id = @UserId
END

-- SP tạo người dùng mới
CREATE PROCEDURE sp_CreateUser
    @Code NVARCHAR(20),
    @FullName NVARCHAR(100),
    @DateOfBirth DATETIME,
    @Email NVARCHAR(100),
    @Phone NVARCHAR(20),
    @Address NVARCHAR(250),
    @NewUserId INT OUTPUT
AS
BEGIN
    INSERT INTO Users (Code, FullName, DateOfBirth, Email, Phone, Address)
    VALUES (@Code, @FullName, @DateOfBirth, @Email, @Phone, @Address)
    
    SET @NewUserId = SCOPE_IDENTITY()
END
```

### **Bước 3: Implement SP Methods trong UserRepositorySP**

Ví dụ cách gọi SP trong [WebApi/Repositories/UserRepositorySP.cs](WebApi/Repositories/UserRepositorySP.cs):

```csharp
public async Task<IEnumerable<User>> GetAllAsync()
{
    return await _context.Users
        .FromSqlRaw("EXEC sp_GetAllUsers")
        .ToListAsync();
}

public async Task<User?> GetByIdAsync(int id)
{
    var parameter = new SqlParameter("@UserId", id);
    var users = await _context.Users
        .FromSqlRaw("EXEC sp_GetUserById @UserId", parameter)
        .ToListAsync();
    return users.FirstOrDefault();
}
```

---

##  Ví Dụ: Thêm Tính Năng Mới

Giả sử muốn thêm method `SearchByEmailAsync` - tìm người dùng theo email:

### **Cách 1: Dùng LINQ (UserRepository.cs)**
```csharp
public async Task<IEnumerable<User>> SearchByEmailAsync(string email)
{
    return await _context.Users
        .Where(u => u.Email.Contains(email))
        .OrderBy(u => u.FullName)
        .ToListAsync();
}
```

### **Cách 2: Dùng Stored Procedures (UserRepositorySP.cs)**
```csharp
// Trước tiên, tạo SP trong SQL Server:
// CREATE PROCEDURE sp_SearchUserByEmail
//     @Email NVARCHAR(100)
// AS
// BEGIN
//     SELECT * FROM Users WHERE Email LIKE '%' + @Email + '%' ORDER BY FullName
// END

public async Task<IEnumerable<User>> SearchByEmailAsync(string email)
{
    var parameter = new SqlParameter("@Email", email);
    return await _context.Users
        .FromSqlRaw("EXEC sp_SearchUserByEmail @Email", parameter)
        .ToListAsync();
}
```

---

##  Các Công Nghệ Sử Dụng

- **Frontend**: Blazor WebAssembly (.NET 10.0)
- **Backend**: ASP.NET Core Web API (.NET 10.0)
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Giao tiếp**: REST API, HttpClient
- **Validation**: Data Annotations
- **CORS**: Cấu hình cho Blazor gọi WebApi

---

##  Ghi Chú Quan Trọng

1. **URL WebApi**: Mặc định `https://localhost:7065` (cấu hình trong [BlazorApp/Program.cs](BlazorApp/Program.cs#L18))
2. **Connection String**: Cấu hình trong [WebApi/appsettings.json](WebApi/appsettings.json)
3. **Migrations**: Dùng `dotnet ef migrations` để tạo/cập nhật schema
4. **Mock Data**: Tự động seed dữ liệu qua [AppDbContext.cs](WebApi/Data/AppDbContext.cs)
5. **Exception Handling**: Custom exceptions trong [WebApi/Exceptions/CustomExceptions.cs](WebApi/Exceptions/CustomExceptions.cs)

---

## 💡 Best Practices

 **LINQ khi**:
- Truy vấn đơn giản đến trung bình
- Cần type-safety
- Dễ test và maintain

 **Stored Procedures khi**:
- Truy vấn phức tạp với nhiều joins
- Cần tối ưu hiệu năng cao
- Xử lý logic phức tạp tại DB
- Báo cáo/Analytics queries

 **Chung cho cả hai**:
- Luôn dùng parameterized queries (tránh SQL Injection)
- Viết unit tests
- Ghi chú code rõ ràng
- Version control cho migrations


