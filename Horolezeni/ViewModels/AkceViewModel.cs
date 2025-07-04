using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Horolezeni.Models;
using Horolezeni.Services;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Firebase.Database.Streaming;
using System.Diagnostics;

namespace Horolezeni.ViewModels
{
    public partial class AkceViewModel : ObservableObject
    {
        private readonly FirebaseService _dbFirebase;
        private IDisposable _subscription;

        [ObservableProperty]
        private ObservableCollection<Akce> _akceList;

        [ObservableProperty]
        private bool _isBusy;

        private Akce _selectedAkce;
        public Akce SelectedAkce
        {
            get => _selectedAkce;
            set
            {
                if (SetProperty(ref _selectedAkce, value) && value != null)
                {
                    NavigateToDetail(value);
                    SelectedAkce = null;
                }
            }
        }

        public AkceViewModel(FirebaseService dbFirebase)
        {
            _dbFirebase = dbFirebase;
            _akceList = new ObservableCollection<Akce>();
        }

        public void OnAppearing()
        {
            // Zabráníme zbytečnému znovunačítání, pokud již data máme
            if (AkceList.Any() || IsBusy) return;

            // Spustíme novou, optimalizovanou metodu načítání
            Task.Run(LoadDataAndSubscribe);
        }

        public void OnDisappearing()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        /// <summary>
        /// Optimalizovaná metoda, která nejprve rychle načte všechna data
        /// a až poté se přihlásí k odběru změn v reálném čase.
        /// </summary>
        private async Task LoadDataAndSubscribe()
        {
            IsBusy = true;

            // 1. KROK: Jednorázové načtení všech dat pro rychlé zobrazení
            var allAkce = await _dbFirebase._dbClient.Child("Akce").OnceAsync<Akce>();
            var initialList = allAkce
                .Select(item => {
                    var akce = item.Object;
                    akce.Id = item.Key;
                    return akce;
                })
                .OrderByDescending(a => a.Start)
                .ToList();

            // Aktualizujeme UI na hlavním vlákně
            MainThread.BeginInvokeOnMainThread(() =>
            {
                AkceList.Clear();
                foreach (var akce in initialList)
                {
                    AkceList.Add(akce);
                }
                IsBusy = false;
            });

            // 2. KROK: Přihlášení k odběru změn, které nastanou odteď
            SubscribeToRealtimeUpdates();
        }

        /// <summary>
        /// Odebírá pouze nové změny a efektivně je zpracovává.
        /// </summary>
        private void SubscribeToRealtimeUpdates()
        {
            if (_subscription != null) return;

            _subscription = _dbFirebase.GetAkceObservable()
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(firebaseEvent =>
                {
                    var akceItem = firebaseEvent.Object;
                    if (akceItem == null) return;
                    akceItem.Id = firebaseEvent.Key;

                    var existingItem = AkceList.FirstOrDefault(a => a.Id == akceItem.Id);

                    // OPRAVA: Switch nyní používá správné typy událostí z Firebase
                    switch (firebaseEvent.EventType)
                    {
                        case FirebaseEventType.InsertOrUpdate:
                            if (existingItem != null) // Jedná se o aktualizaci
                            {
                                // Pokud se změnilo datum, je potřeba položku přesunout
                                if (existingItem.Start != akceItem.Start)
                                {
                                    AkceList.Remove(existingItem);
                                    var indexToReinsert = AkceList.TakeWhile(a => a.Start > akceItem.Start).Count();
                                    AkceList.Insert(indexToReinsert, akceItem);
                                }
                                else // Jinak stačí jen aktualizovat na místě
                                {
                                    var indexToUpdate = AkceList.IndexOf(existingItem);
                                    AkceList[indexToUpdate] = akceItem;
                                }
                            }
                            else // Jedná se o novou položku
                            {
                                // Efektivní vložení na správné seřazené místo
                                var indexToInsert = AkceList.TakeWhile(a => a.Start > akceItem.Start).Count();
                                AkceList.Insert(indexToInsert, akceItem);
                            }
                            break;

                        case FirebaseEventType.Delete:
                            if (existingItem != null)
                            {
                                AkceList.Remove(existingItem);
                            }
                            break;
                    }

                }, async ex =>
                {
                    await Shell.Current.DisplayAlert("Chyba synchronizace", $"Spojení bylo přerušeno: {ex.Message}", "OK");
                });
        }

        private async void NavigateToDetail(Akce akce)
        {
            await Shell.Current.GoToAsync(nameof(AkceDetailPage), true, new Dictionary<string, object>
            {
                { "SelectedAkce", akce }
            });
        }

        [RelayCommand]
        private async Task GoToAdd()
        {
            var newAkce = new Akce { Id = Guid.NewGuid().ToString() };
            await Shell.Current.GoToAsync(nameof(AkceFormPage), new Dictionary<string, object> { { "SelectedAkce", newAkce } });
        }
    }
}
