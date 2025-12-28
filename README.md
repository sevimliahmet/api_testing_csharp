# ApiTestingCSharp

Kısa: API testleri için xUnit tabanlı proje. Testler `ApiTesting.Specs` içinde bulunur.

Hızlı başlatma (lokal):

[![CI](https://github.com/sevimliahmet/api_testing_csharp/actions/workflows/ci.yml/badge.svg)](https://github.com/sevimliahmet/api_testing_csharp/actions/workflows/ci.yml)

## Hızlı başlatma (lokal)

```bash
dotnet --info
dotnet restore
dotnet test ApiTesting.Specs/ApiTesting.Specs.csproj

```

CI (GitHub Actions):
- Workflow: `.github/workflows/ci.yml` — push ve PR'larda `dotnet build`, `dotnet test` ve coverage topluyor.
- Test sonuçları ve coverage `TestResults` artefaktı olarak yüklenecek.

Codecov notu:
- Public repository ise Codecov otomatik çalışacaktır; özel repo ise `CODECOV_TOKEN` secret eklemeniz gerekir.

Diğer notlar:
- `Config/appsettings.json` test çalıştırılırken çıktı dizinine kopyalanır.
- `FluentAssertions` kullanımında ticari lisans kısıtları olabilir; ticari kullanım planlıyorsanız lisans kontrolü yapın.

İstersen CI workflow'unda TRX → GitHub Checks raporlamasını veya Codecov entegrasyonunu özelleştireyim.

## CI Pipeline

This project includes a GitHub Actions CI pipeline that:

- Builds the solution
- Starts a demo API
- Runs API integration tests
- Generates test results and coverage artifacts

The pipeline is automatically triggered on pull requests and merges to `main`.

## Demo API ile lokal test

Projede `ApiTesting.DemoApi` adında basit bir demo API var. Testleri gerçek bir endpoint’e bağlamak için bunu kullandım.

### 1) Demo API’yi çalıştır
```bash
cd ApiTesting.DemoApi
dotnet run


