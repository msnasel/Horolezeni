using Horolezeni.Models; // Pøidáno pro pøístup k modelu Akce
using Horolezeni.ViewModels;
using System.Diagnostics; // Pøidáno pro diagnostiku

namespace Horolezeni.Views;

public partial class AkcePage : ContentPage
{
    private readonly AkceViewModel _viewModel;

    public AkcePage(AkceViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnDisappearing();
    }

    // TOTO JE NOVÁ TESTOVACÍ METODA
    private async void OnAkceSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Zkontrolujeme, zda byla vybrána nìjaká položka
        if (e.CurrentSelection.FirstOrDefault() is not Akce selectedAkce)
        {
            Debug.WriteLine("[DIAGNOSTIKA] Událost SelectionChanged byla spuštìna, ale nebyla vybrána žádná akce.");
            return;
        }

        Debug.WriteLine($"[DIAGNOSTIKA] Akce '{selectedAkce.Action}' byla vybrána pøímo v code-behind. Naviguji...");

        // Provedeme navigaci pøímo z code-behind
        await Shell.Current.GoToAsync("AkceDetailView", new Dictionary<string, object>
        {
            { "SelectedAkce", selectedAkce }
        });

        // Dùležité: Zrušíme výbìr, aby bylo možné znovu kliknout
        (sender as CollectionView).SelectedItem = null;
    }
}
