using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Horolezeni.Models;
using Horolezeni.Services;
using System.Diagnostics;

namespace Horolezeni.ViewModels
{
    [QueryProperty(nameof(AkceFromNavigation), "SelectedAkce")]
    public partial class AkceDetailViewModel : ObservableObject
    {
        private readonly FirebaseService _dbFirebase;

        // Dočasná vlastnost pro bezpečné přijetí dat
        [ObservableProperty]
        private Akce _akceFromNavigation;

        // Vlastnost, na kterou je navázáno UI
        [ObservableProperty]
        private Akce _selectedAkce;

        // NOVÁ VLASTNOST: Řídí viditelnost obsahu na stránce
        [ObservableProperty]
        private bool _isDataLoaded;

        public AkceDetailViewModel(FirebaseService dbFirebase)
        {
            _dbFirebase = dbFirebase;
            // Na začátku je obsah skrytý a zobrazuje se indikátor načítání
            IsDataLoaded = false;
            // Inicializujeme prázdným objektem, aby se předešlo chybám
            _selectedAkce = new Akce();
        }

        // Metoda se zavolá, když dorazí data z navigace
        partial void OnAkceFromNavigationChanged(Akce value)
        {
            if (value == null || string.IsNullOrEmpty(value.Id))
            {
                Debug.WriteLine("[DIAGNOSTIKA] Detail obdržel neplatná data.");
                SelectedAkce = new Akce { Action = "Nepodařilo se načíst detail akce." };
            }
            else
            {
                Debug.WriteLine($"[DIAGNOSTIKA] Detail úspěšně obdržel data pro: {value.Action}.");
                SelectedAkce = value;
            }
            // AŽ TEĎ, když jsou data zpracována, dáme signál UI, aby se zobrazilo
            IsDataLoaded = true;
        }

        [RelayCommand]
        private async Task GoToEdit()
        {
            if (SelectedAkce == null || string.IsNullOrEmpty(SelectedAkce.Id)) return;

            // OPRAVA: Navigace nyní používá správný název stránky 'AkceFormView'.
            // Použití nameof() je bezpečnější než textový řetězec.
            await Shell.Current.GoToAsync(nameof(AkceFormPage), new Dictionary<string, object>
            {
                { "SelectedAkce", SelectedAkce }
            });
        }

        [RelayCommand]
        private async Task Delete()
        {
            if (SelectedAkce == null || string.IsNullOrEmpty(SelectedAkce.Id)) return;

            bool userConfirmed = await Shell.Current.DisplayAlert(
                "Smazat Akci",
                $"Opravdu si přejete smazat akci '{SelectedAkce.Action}'?",
                "Ano, smazat",
                "Ne");

            if (userConfirmed)
            {
                await _dbFirebase.DeleteAkce(SelectedAkce.Id);
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
