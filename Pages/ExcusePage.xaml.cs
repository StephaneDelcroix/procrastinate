using procrastinate.Resources.Strings;
using procrastinate.Services;

namespace procrastinate.Pages;

public partial class ExcusePage : ContentPage
{
    private readonly StatsService _statsService;
    private string _currentExcuse = "";

    public ExcusePage(StatsService statsService)
    {
        InitializeComponent();
        _statsService = statsService;
        UpdateCounterLabel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateCounterLabel();
    }

    private void UpdateCounterLabel()
    {
        CounterLabel.Text = AppStrings.GetString("ExcusesGenerated", _statsService.ExcusesGenerated);
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private void OnGenerateClicked(object? sender, EventArgs e)
    {
        var (starters, middles, endings) = GetLocalizedExcuseParts();
        var starter = starters[Random.Shared.Next(starters.Length)];
        var middle = middles[Random.Shared.Next(middles.Length)];
        var ending = endings[Random.Shared.Next(endings.Length)];

        _currentExcuse = $"{starter} {middle} {ending}";
        ExcuseLabel.Text = _currentExcuse;
        
        _statsService.IncrementExcusesGenerated();
        UpdateCounterLabel();
    }

    private async void OnCopyClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_currentExcuse)) return;
        
        await Clipboard.SetTextAsync(_currentExcuse);
        CopyBtn.Text = $"✓ {AppStrings.GetString("Copied")}";
        await Task.Delay(1500);
        CopyBtn.Text = AppStrings.GetString("CopyToClipboard");
    }

    private (string[], string[], string[]) GetLocalizedExcuseParts()
    {
        return AppStrings.CurrentLanguage switch
        {
            "fr" => (
                ["Je ne peux pas parce que", "Désolé, mais", "J'aurais bien voulu, mais", "Malheureusement,", "J'ai essayé, mais"],
                ["mon poisson rouge", "Mercure est en rétrograde et", "mon horoscope a dit", "un inconnu mystérieux", "mon WiFi", "mes plantes", "le chat du voisin"],
                ["a besoin de moi pour un soutien émotionnel.", "m'a spécifiquement mis en garde.", "me juge trop en ce moment.", "a supprimé toute ma motivation.", "m'a dit de faire une sieste."]
            ),
            "es" => (
                ["No puedo porque", "Lo siento, pero", "Me encantaría, pero", "Desafortunadamente,", "Lo intenté, pero"],
                ["mi pez dorado", "Mercurio está retrógrado y", "mi horóscopo dijo", "un extraño misterioso", "mi WiFi", "mis plantas", "el gato del vecino"],
                ["me necesita para apoyo emocional.", "me advirtió específicamente.", "me está juzgando demasiado.", "borró toda mi motivación.", "me dijo que tomara una siesta."]
            ),
            "pt" => (
                ["Não posso porque", "Desculpe, mas", "Eu gostaria, mas", "Infelizmente,", "Eu tentei, mas"],
                ["meu peixinho dourado", "Mercúrio está retrógrado e", "meu horóscopo disse", "um estranho misterioso", "meu WiFi", "minhas plantas", "o gato do vizinho"],
                ["precisa de mim para apoio emocional.", "me avisou especificamente.", "está me julgando demais.", "deletou toda minha motivação.", "me disse para tirar uma soneca."]
            ),
            "nl" => (
                ["Ik kan niet omdat", "Sorry, maar", "Ik zou wel willen, maar", "Helaas,", "Ik probeerde, maar"],
                ["mijn goudvis", "Mercurius is retrograde en", "mijn horoscoop zei", "een mysterieuze vreemdeling", "mijn WiFi", "mijn planten", "de kat van de buren"],
                ["heeft me nodig voor emotionele steun.", "waarschuwde me specifiek.", "oordeelt me te hard.", "verwijderde al mijn motivatie.", "zei dat ik moest slapen."]
            ),
            "cs" => (
                ["Nemůžu, protože", "Promiň, ale", "Rád bych, ale", "Bohužel,", "Zkoušel jsem, ale"],
                ["moje zlatá rybka", "Merkur je retrográdní a", "můj horoskop řekl", "záhadný cizinec", "moje WiFi", "moje rostliny", "sousedova kočka"],
                ["mě potřebuje pro emocionální podporu.", "mě konkrétně varoval.", "mě příliš soudí.", "smazal veškerou mou motivaci.", "mi řekl, abych si zdříml."]
            ),
            _ => (
                ["I can't do that because", "Sorry, but", "I would, but", "Unfortunately,", "I tried, but"],
                ["my pet goldfish", "Mercury is in retrograde and", "my horoscope said", "a mysterious stranger", "my WiFi", "my plants", "my neighbor's cat"],
                ["needs me for emotional support.", "specifically warned against it.", "is judging me too hard right now.", "deleted all my motivation.", "told me to take a nap instead."]
            )
        };
    }
}
