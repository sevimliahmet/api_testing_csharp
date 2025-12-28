# ApiTestingCSharp

Kısa: API testleri için xUnit tabanlı proje. Testler `ApiTesting.Specs` içinde bulunur.

Hızlı başlatma (lokal):

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
