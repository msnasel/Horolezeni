using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Horolezeni.Models;
using Horolezeni.Services;
using System.Collections.Generic;

namespace Horolezeni.ViewModels
{
    [QueryProperty(nameof(CurrentAkce), "SelectedAkce")]
    public partial class AkceFormViewModel : ObservableObject
    {
        private readonly FirebaseService _dbFirebase;

        [ObservableProperty]
        private Akce _currentAkce;

        [ObservableProperty]
        private string _PageTitle;

        [ObservableProperty]
        private bool _isInitialized;

        [ObservableProperty]
        private DateTime _startDate;

        [ObservableProperty]
        private TimeSpan _startTime;

        [ObservableProperty]
        private DateTime _endDate;

        [ObservableProperty]
        private TimeSpan _endTime;

        public List<string> AkceTypes { get; }

        public AkceFormViewModel(FirebaseService dbFirebase)
        {
            _dbFirebase = dbFirebase;
            IsInitialized = false;
            _currentAkce = new Akce();
            AkceTypes = new List<string> { "Ferrata", "Kurz", "Lezení", "Lyžování", "Sraz", "Túra" };
        }

        partial void OnCurrentAkceChanged(Akce value)
        {
            if (value == null) return;

            // Převedení DateTime na pomocné vlastnosti
            StartDate = value.Start.Date;
            StartTime = value.Start.TimeOfDay;
            EndDate = value.End.Date;
            EndTime = value.End.TimeOfDay;

            bool isNew = string.IsNullOrEmpty(value.Id) || Guid.TryParse(value.Id, out _);
            PageTitle = isNew ? "Přidat novou akci" : "Upravit akci";

            // AŽ TEĎ dáme signál, že je vše připraveno a synchronizace se může spustit.
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsInitialized = true;
            });
        }

        // --- UPRAVENÉ METODY PRO SYNCHRONIZACI ---
        partial void OnStartDateChanged(DateTime value)
        {
            if (!IsInitialized) return; // ZABRÁNÍ PŘEDČASNÉMU SPUŠTĚNÍ
            CurrentAkce.Start = value.Date + StartTime;
        }

        partial void OnStartTimeChanged(TimeSpan value)
        {
            if (!IsInitialized) return; // ZABRÁNÍ PŘEDČASNÉMU SPUŠTĚNÍ
            CurrentAkce.Start = StartDate.Date + value;
        }

        partial void OnEndDateChanged(DateTime value)
        {
            if (!IsInitialized) return; // ZABRÁNÍ PŘEDČASNÉMU SPUŠTĚNÍ
            CurrentAkce.End = value.Date + EndTime;
        }

        partial void OnEndTimeChanged(TimeSpan value)
        {
            if (!IsInitialized) return; // ZABRÁNÍ PŘEDČASNÉMU SPUŠTĚNÍ
            CurrentAkce.End = EndDate.Date + value;
        }
        // --- KONEC UPRAVENÝCH METOD ---

        [RelayCommand]
        private async Task Save()
        {
            if (CurrentAkce == null || string.IsNullOrWhiteSpace(CurrentAkce.Action))
            {
                await Shell.Current.DisplayAlert("Chyba", "Název akce je povinný.", "OK");
                return;
            }
            try
            {
                // Před uložením se ujistíme, že jsou hodnoty správně zkombinovány
                CurrentAkce.Start = StartDate.Date + StartTime;
                CurrentAkce.End = EndDate.Date + EndTime;

                bool isNew = string.IsNullOrEmpty(CurrentAkce.Id) || Guid.TryParse(CurrentAkce.Id, out _);
                if (isNew) await _dbFirebase.AddAkce(CurrentAkce);
                else await _dbFirebase.UpdateAkce(CurrentAkce);
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Chyba", $"Uložení selhalo: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}