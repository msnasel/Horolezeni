using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using Horolezeni.Models; // <-- Přidejte using pro vaše modely
using System;
using System.Threading.Tasks;

namespace Horolezeni.Services
{
    public class FirebaseService
    {
        public FirebaseAuthClient _authClient;
        public FirebaseClient _dbClient;

        public FirebaseService()
        {
            // !!! NIKDY NENECHÁVEJTE KLÍČE V KÓDU !!!
            // Načtěte je z bezpečného úložiště, např. .NET User Secrets
            var apiKey = "AIzaSyDgVQy4PupEabfDnnvVtj9MnF4Sa5COYU";
            var dbUrl = "https://horolezeni-7e74b-default-rtdb.europe-west1.firebasedatabase.app/";

            var authConfig = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            _authClient = new FirebaseAuthClient(authConfig);
            _dbClient = new FirebaseClient(dbUrl);
        }

        public async Task<UserCredential> LoginAsync(string email, string password)
        {
            return await _authClient.SignInWithEmailAndPasswordAsync(email, password);
        }

        // --- SPRÁVNÁ IMPLEMENTACE PRÁCE S UŽIVATELEM ---

        // Metoda nyní vrací váš vlastní model 'User'
        public async Task<User> GetUserAsync(string uid)
        {
            // Deserializujeme do vašeho modelu, ne do Firebase.Auth.User
            return await _dbClient.Child("users").Child(uid).OnceSingleAsync<User>();
        }

        // Metoda přijímá váš vlastní model 'User'
        public async Task SaveUserAsync(User user)
        {
            // Ukládáme váš model
            await _dbClient.Child("users").Child(user.Uid).PutAsync(user);
        }

        // --- CRUD OPERACE PRO 'Akce' (zůstávají podobné) ---

        public IObservable<FirebaseEvent<Akce>> GetAkceObservable()
        {
            return _dbClient
                .Child("Akce")
                .AsObservable<Akce>();
        }

        // PostAsync správně vrací objekt s vygenerovaným klíčem
        public async Task<FirebaseObject<Akce>> AddAkce(Akce novaAkce)
        {
            return await _dbClient
                .Child("Akce")
                .PostAsync(novaAkce);
        }

        public async Task UpdateAkce(string akceId, Akce aktualizovanaAkce)
        {
            await _dbClient
                .Child("Akce")
                .Child(akceId)
                .PutAsync(aktualizovanaAkce);
        }

        public async Task DeleteAkce(string akceId)
        {
            await _dbClient
                .Child("Akce")
                .Child(akceId)
                .DeleteAsync();
        }
    }
}