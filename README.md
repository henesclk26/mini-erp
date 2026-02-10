Mini ERP Uygulaması

Katmanlı mimari ile geliştirilmiş masaüstü envanter/stok yönetim uygulaması.

Teknolojiler

| Katman | Teknoloji |
|--------|-----------|
| **UI** | WPF + MVVM Pattern |
| **BL** | C# Class Library|
| **DAL** | Entity Framework Core + SQLite |

Proje Yapısı

```
MiniERP/
├── MiniERP.DAL/          # Data Access Layer
│   ├── Context/          # EF Core DbContext
│   ├── Entities/         # Category, Product, StockMovement
│   └── Repositories/    # Generic + özel repository'ler
│
├── MiniERP.BL/           # Business Logic Layer
│   ├── DTOs/             # Veri transfer nesneleri
│   └── Services/         # İş mantığı servisleri
│
├── MiniERP.UI/           # WPF Presentation Layer
│   ├── Views/            # XAML View dosyaları
│   ├── ViewModels/       # MVVM ViewModel'ler
│   └── Helpers/          # RelayCommand vb.
│
└── MiniERP.slnx          # Solution dosyası
```

Özellikler

Ürün Yönetimi** - Ekleme, güncelleme, silme, arama
Kategori Yönetimi** - Kategori CRUD işlemleri
Stok Giriş** - Ürün stok girişi ve hareket kaydı
Stok Çıkış** - Stok çıkışı + yetersiz stok kontrolü
Raporlama** - Stok durumu, hareket raporu, düşük stok uyarıları
Dashboard** - Özet bilgiler, günlük hareketler

