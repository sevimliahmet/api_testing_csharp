# 🧪 API Testing Suite - C#

[![CI](https://github.com/sevimliahmet/api_testing_csharp/actions/workflows/ci.yml/badge.svg)](https://github.com/sevimliahmet/api_testing_csharp/actions/workflows/ci.yml)

Comprehensive API test suite built with **xUnit**, **FluentAssertions**, and **C# 12**. Includes **28 tests** covering positive scenarios, negative scenarios, and edge cases.

---

## 📊 Test Coverage

| Category | Tests | Status |
|----------|-------|--------|
| 🏥 Health Checks | 3 | ✅ |
| ✅ Positive Scenarios | 11 | ✅ |
| ⚠️ Negative Scenarios | 14 | ✅ |
| **TOTAL** | **28** | **✅ ALL PASSING** |

### Test Breakdown

**Health Tests (3):**
- Health endpoint returns 200 OK
- Health endpoint returns "ok" status
- Health endpoint responds within 500ms

**Positive Tests (11):**
- GET valid post (performance check)
- GET invalid post (404 handling)
- POST creates post (201 Created)
- Boundary tests (zero ID, negative ID)
- Special characters handling
- Large content handling
- Parametric tests (multiple userId values)

**Negative Tests (14):**
- Invalid JSON format (4 tests)
- Missing required fields (4 tests)
- Wrong Content-Type (2 tests)
- Null/Invalid values (4 tests)

---

## 🚀 Quick Start

### Prerequisites
- .NET 8.0+
- C# 12+

### Setup

```bash
# Clone repo
git clone https://github.com/sevimliahmet/api_testing_csharp.git
cd api_testing_csharp

# Restore packages
dotnet restore
```

---

## ▶️ Running Tests

### ⚠️ Important: Open 2 Terminals

**Terminal 1 - Start Demo API:**
```bash
cd ApiTesting.DemoApi
dotnet run
# API runs on http://localhost:5052
```

**Terminal 2 - Run Tests:**
```bash
cd ApiTesting.Specs
dotnet test
```

### Test Commands

```bash
# Run all tests (verbose output)
dotnet test

# Run with minimal output
dotnet test --verbosity minimal

# Run specific test category
dotnet test --filter "FullyQualifiedName~HealthTests"

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "FullyQualifiedName~GET_valid_post_returns_200_fast"
```

---

## 🔧 Key Features

### 1️⃣ **Retry Mechanism**
- Automatic retry on network errors
- Configurable retry count and delay
- Settings in `Config/TestSettings.cs`

```csharp
var api = new ApiClient(baseUrl, timeoutMs, retryCount: 2, retryDelayMs: 200);
```

### 2️⃣ **Structured Logging**
- Colored console output for request/response
- Error logging with stack traces
- TestLogger class for debugging

### 3️⃣ **Test Data Builder Pattern**
- Fluent API for clean test data creation
- PostBuilder for flexible post creation

```csharp
var payload = PostBuilder.New()
    .WithTitle("Hello")
    .WithBody("World")
    .WithUserId(1)
    .BuildJson();
```

### 4️⃣ **Setup/Teardown with IAsyncLifetime**
- API health verification before tests run
- Automatic cleanup after tests
- Prevents false negatives due to API unavailability

### 5️⃣ **Content-Type Testing**
- Test custom HTTP headers
- Validate API's media type handling

```csharp
await api.SendAsync(HttpMethod.Post, "/posts", payload, "text/plain");
```

---

## 📁 Project Structure

```
api-testing-csharp/
├── ApiTesting.DemoApi/          # Demo API for testing
│   ├── Controllers/
│   │   └── PostsController.cs   # POST/GET endpoints
│   ├── Program.cs
│   └── appsettings.json
│
├── ApiTesting.Specs/            # Test Project
│   ├── Core/
│   │   ├── ApiClient.cs         # HTTP client with retry
│   │   ├── TestLogger.cs        # Logging system
│   │   └── ResponseResult.cs    # Response model
│   ├── Helpers/
│   │   └── PostBuilder.cs       # Test data builder
│   ├── Config/
│   │   ├── TestSettings.cs      # Configuration
│   │   └── appsettings.json
│   └── Tests/
│       ├── HealthTests.cs       # Health endpoint tests
│       └── PostsTests.cs        # Posts API tests (25 tests)
│
└── .github/workflows/ci.yml     # GitHub Actions CI/CD
```

---

## ⚙️ Configuration

Edit `ApiTesting.Specs/Config/appsettings.json`:

```json
{
  "BaseUrl": "http://localhost:5052",
  "TimeoutMs": 10000,
  "MaxResponseMs": 1500,
  "RetryCount": 2,
  "RetryDelayMs": 200
}
```

Or use environment variables:
```bash
export API_BaseUrl=http://your-api:5000
export API_TimeoutMs=15000
```

---

## 🔄 CI/CD Pipeline

GitHub Actions workflow (`.github/workflows/ci.yml`):
- ✅ Builds solution
- ✅ Runs all 28 tests
- ✅ Collects code coverage
- ✅ Uploads test results as artifacts
- ✅ Automatic on PR and push to main/ci/workflow

---

## 🛠️ Tech Stack

| Technology | Purpose |
|-----------|---------|
| **xUnit** | Test framework |
| **FluentAssertions** | Fluent assertion API |
| **C# 12** | Language |
| **.NET 8** | Runtime |
| **GitHub Actions** | CI/CD |

---

## 📝 Example Test

```csharp
[Fact]
public async Task POST_creates_post()
{
    var payload = PostBuilder.New()
        .WithTitle("hello")
        .WithBody("from test")
        .WithUserId(1)
        .BuildJson();

    var res = await _api.SendAsync(HttpMethod.Post, "/posts", payload);

    res.StatusCode.Should().Be(201);
    
    using var json = JsonDocument.Parse(res.Body);
    json.RootElement.GetProperty("title")
        .GetString()
        .Should()
        .Be("hello");
}
```

---

## 📜 License

MIT License - See LICENSE file for details

---

## 📧 Contact

- GitHub: [@sevimliahmet](https://github.com/sevimliahmet)
- Email: ahmet03sevimli@icloud.com
