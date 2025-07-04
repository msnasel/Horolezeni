using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horolezeni.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        private readonly FirebaseService _firebaseService;

        public LoginViewModel(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Chyba", "Zadejte email i heslo.", "OK");
                return;
            }

            try
            {
                // 1. Přihlášení přes Firebase Auth
                var userCredential = await _firebaseService.LoginAsync(Email, Password);
                var authLink = userCredential.User;

                // Uložení tokenu pro trvalé přihlášení
                await SecureStorage.Default.SetAsync("firebase_token", userCredential.FirebaseToken);

                // 2. Zjištění, zda uživatel již má přezdívku v databázi
                Horolezeni.Models.User user = null;
                try
                {
                    user = await _firebaseService.GetUserAsync(authLink.LocalId);
                }
                catch
                {
                    // Uživatel v databázi neexistuje, budeme ho vytvářet
                }

                if (user == null || string.IsNullOrWhiteSpace(user.Nickname))
                {
                    // 3. Vyskakovací okno pro zadání přezdívky
                    string nickname = await Shell.Current.DisplayPromptAsync("Vítejte!", "Zvolte si prosím přezdívku.", "Uložit", "Zrušit", "MojePřezdívka");

                    if (!string.IsNullOrWhiteSpace(nickname))
                    {
                        var newUser = new Horolezeni.Models.User
                        {
                            Uid = authLink.LocalId,
                            Email = authLink.Email,
                            Nickname = nickname
                        };
                        // 4. Uložení uživatele s přezdívkou do databáze
                        await _firebaseService.SaveUserAsync(newUser);
                    }
                }

                // Přesměrování na hlavní stránku aplikace
                await Shell.Current.GoToAsync("//MainView");
            }
            catch (FirebaseAuthException ex)
            {
                await Shell.Current.DisplayAlert("Chyba přihlášení", "Nesprávný email nebo heslo.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Chyba", $"Došlo k neočekávané chybě: {ex.Message}", "OK");
            }
        }
    }
}
