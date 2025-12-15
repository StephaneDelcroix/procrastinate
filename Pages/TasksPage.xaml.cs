using procrastinate.Resources.Strings;
using procrastinate.Services;

namespace procrastinate.Pages;

public partial class TasksPage : ContentPage
{
    private readonly StatsService _statsService;

    public TasksPage(StatsService statsService)
    {
        InitializeComponent();
        _statsService = statsService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateLabels();
    }

    private void UpdateLabels()
    {
        TitleLabel.Text = AppStrings.Get("TodaysTasks");
        ProductivityListLabel.Text = AppStrings.Get("YourProductivityList");
        TakeBreakLabel.Text = AppStrings.Get("TakeABreak");
        MotivationLabel.Text = AppStrings.Get("DoingGreat");
        AddTaskBtn.Text = AppStrings.Get("AddMoreTasks");
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private async void OnTaskChecked(object? sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            _statsService.IncrementBreaksTaken();
            MotivationLabel.Text = AppStrings.Get("Congratulations");
            await Task.Delay(2000);
            TaskCheckBox.IsChecked = false;
            MotivationLabel.Text = AppStrings.Get("NeedAnotherBreak");
        }
    }

    private async void OnAddTaskClicked(object? sender, EventArgs e)
    {
        _statsService.IncrementTasksAvoided();
        var excuses = GetLocalizedExcuses();
        var excuse = excuses[Random.Shared.Next(excuses.Length)];
        await DisplayAlertAsync("❌", excuse, "OK");
    }

    private string[] GetLocalizedExcuses()
    {
        return AppStrings.CurrentLanguage switch
        {
            "fr" => ["Oups! La liste est pleine. Réessayez demain!", "Erreur 404: Productivité introuvable.", "Tâche rejetée: Vous méritez une pause!", "Le serveur fait la sieste. Comme vous devriez!", "Productivité maximale atteinte! (1 tâche = maximum)"],
            "es" => ["¡Ups! La lista está llena. ¡Inténtalo mañana!", "Error 404: Productividad no encontrada.", "¡Tarea rechazada: Te mereces un descanso!", "El servidor está durmiendo. ¡Como tú deberías!", "¡Productividad máxima alcanzada! (1 tarea = máximo)"],
            "pt" => ["Ops! A lista está cheia. Tente amanhã!", "Erro 404: Produtividade não encontrada.", "Tarefa rejeitada: Você merece uma pausa!", "O servidor está dormindo. Como você deveria!", "Produtividade máxima atingida! (1 tarefa = máximo)"],
            "nl" => ["Oeps! De lijst is vol. Probeer morgen!", "Fout 404: Productiviteit niet gevonden.", "Taak afgewezen: Je verdient een pauze!", "De server slaapt. Net als jij zou moeten!", "Maximale productiviteit bereikt! (1 taak = maximum)"],
            "cs" => ["Jejda! Seznam je plný. Zkuste zítra!", "Chyba 404: Produktivita nenalezena.", "Úkol odmítnut: Zasloužíte si pauzu!", "Server spí. Jako byste měli vy!", "Maximální produktivita dosažena! (1 úkol = maximum)"],
            _ => ["Oops! The task list is full. Try again tomorrow!", "Error 404: Productivity not found.", "Task rejected: You deserve a break instead!", "Server is napping. Just like you should be!", "Maximum productivity reached! (1 task = maximum)"]
        };
    }
}
