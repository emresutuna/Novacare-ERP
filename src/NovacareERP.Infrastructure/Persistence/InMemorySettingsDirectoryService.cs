using NovacareERP.Application.Settings;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class InMemorySettingsDirectoryService : ISettingsDirectoryService
{
    private readonly List<SettingsUserViewModel> _users =
    [
        new()
        {
            Id = Guid.Parse("59af7cc7-30f8-46b5-b583-f995d77cc389"),
            FullName = "Emre Sutuna",
            Email = "emre@finora.local",
            PhoneNumber = "0555 000 00 00",
            IsActive = true,
            RoleName = "Yonetici"
        }
    ];

    public SettingsWorkspaceViewModel GetWorkspace()
    {
        return new SettingsWorkspaceViewModel
        {
            Users = _users,
            Definitions =
            [
                new() { Title = "Firma Bilgileri", Description = "Vergi, adres, logo ve sube bilgileri", StatusText = "Hazir", IconName = "bi-building" },
                new() { Title = "Kasa ve Banka Tanimlari", Description = "Hesaplar, para birimleri ve acilis bakiyeleri", StatusText = "7 hesap", IconName = "bi-bank2" },
                new() { Title = "Vergi / KDV Tanimlari", Description = "Fatura ve urunlerde kullanilacak oranlar", StatusText = "4 oran", IconName = "bi-percent" }
            ],
            Integrations =
            [
                new() { Title = "E-Fatura", Description = "GIB e-fatura ve e-arsiv ayarlari", StatusText = "Test modu", IconName = "bi-receipt-cutoff" },
                new() { Title = "e-Fatura POS Ayarlari", Description = "Sanal POS ile tahsilat ve e-belge eslestirme", StatusText = "Yeni", IconName = "bi-credit-card" },
                new() { Title = "Kargo Entegrasyonu", Description = "Kargo firmalari ve otomatik takip bilgisi", StatusText = "Yeni", IconName = "bi-truck-front-fill" },
                new() { Title = "SMS Ayarlari", Description = "SMS gonderici adi ve bildirim sablonlari", StatusText = "Pasif", IconName = "bi-chat-left-text" }
            ],
            Templates =
            [
                new() { Title = "Fatura/Irsaliye Ayari", Description = "Belge numaralari, notlar ve varsayilanlar", StatusText = "Duzenlenebilir", IconName = "bi-file-earmark-ruled" },
                new() { Title = "Teklif ve Ozel Sablonlar", Description = "Teklif PDF tasarimi ve e-posta metinleri", StatusText = "3 sablon", IconName = "bi-file-text" },
                new() { Title = "Etiket Sablonlari", Description = "Urun ve kargo etiketleri", StatusText = "2 sablon", IconName = "bi-tag" }
            ]
        };
    }

    public SettingsUserViewModel? GetUser(Guid id)
    {
        return _users.FirstOrDefault(user => user.Id == id);
    }
}
