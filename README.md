# Mini ERP - Stok Yönetim Sistemi

Katmanlı mimari (UI → BL → DAL) ile geliştirilmiş masaüstü envanter/stok yönetim uygulaması.

## 🛠️ Teknolojiler

| Katman | Teknoloji |
|--------|-----------|
| **UI** | WPF (.NET 10) + MVVM Pattern |
| **BL** | C# Class Library - İş Kuralları |
| **DAL** | Entity Framework Core + SQLite |

## 📁 Proje Yapısı

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

## ✨ Özellikler

- **📦 Ürün Yönetimi** - Ekleme, güncelleme, silme, arama
- **📁 Kategori Yönetimi** - Kategori CRUD işlemleri
- **📥 Stok Giriş** - Ürün stok girişi ve hareket kaydı
- **📤 Stok Çıkış** - Stok çıkışı + yetersiz stok kontrolü
- **📊 Raporlama** - Stok durumu, hareket raporu, düşük stok uyarıları
- **🏠 Dashboard** - Özet bilgiler, günlük hareketler

## 🏗️ Mimari

```
┌──────────────────────────┐
│     MiniERP.UI (WPF)     │  ← Kullanıcı arayüzü
│     MVVM Pattern         │
├──────────────────────────┤
│     MiniERP.BL           │  ← İş kuralları & validasyon
│     Services + DTOs      │
├──────────────────────────┤
│     MiniERP.DAL          │  ← Veri erişim katmanı
│     EF Core + SQLite     │
└──────────────────────────┘
```

## 🚀 Çalıştırma

```bash
# Projeyi build et
dotnet build MiniERP.slnx

# Uygulamayı çalıştır
dotnet run --project MiniERP.UI
```

> **Not:** Uygulama ilk çalıştırıldığında SQLite veritabanı otomatik olarak oluşturulur ve örnek kategoriler eklenir.

## 📋 Veritabanı Şeması

- **Category** - Id, Name, Description
- **Product** - Id, Name, Barcode, CategoryId, PurchasePrice, SalePrice, CurrentStock, MinStockLevel
- **StockMovement** - Id, ProductId, MovementType (Entry/Exit), Quantity, UnitPrice, MovementDate

## 📄 Lisans

Bu proje eğitim amaçlı geliştirilmiştir.
