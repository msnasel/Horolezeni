using Horolezeni.Models; // P�id�no pro p��stup k modelu Akce
using Horolezeni.ViewModels;
using System.Diagnostics; // P�id�no pro diagnostiku

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

    // TOTO JE NOV� TESTOVAC� METODA
    private async void OnAkceSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Zkontrolujeme, zda byla vybr�na n�jak� polo�ka
        if (e.CurrentSelection.FirstOrDefault() is not Akce selectedAkce)
        {
            Debug.WriteLine("[DIAGNOSTIKA] Ud�lost SelectionChanged byla spu�t�na, ale nebyla vybr�na ��dn� akce.");
            return;
        }

        Debug.WriteLine($"[DIAGNOSTIKA] Akce '{selectedAkce.Action}' byla vybr�na p��mo v code-behind. Naviguji...");

        // Provedeme navigaci p��mo z code-behind
        await Shell.Current.GoToAsync("AkceDetailView", new Dictionary<string, object>
        {
            { "SelectedAkce", selectedAkce }
        });

        // D�le�it�: Zru��me v�b�r, aby bylo mo�n� znovu kliknout
        (sender as CollectionView).SelectedItem = null;
    }
}
