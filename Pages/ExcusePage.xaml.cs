using procrastinate.Resources.Strings;
using procrastinate.Services;

namespace procrastinate.Pages;

public partial class ExcusePage : ContentPage
{
    private readonly StatsService _statsService;
    private readonly ExcuseService _excuseService;
    private string _currentExcuse = "";

    public ExcusePage(StatsService statsService, ExcuseService excuseService)
    {
        InitializeComponent();
        _statsService = statsService;
        _excuseService = excuseService;
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

    private async void OnGenerateClicked(object? sender, EventArgs e)
    {
        // Refresh zalgo randomness on button click
        AppStrings.Refresh();
        
        // Show loading state
        GenerateBtn.IsEnabled = false;
        ExcuseLabel.Text = AppStrings.GetString("Generating");

        try
        {
            _currentExcuse = await _excuseService.GenerateExcuseAsync(AppStrings.CurrentLanguage);
            
            // Apply Zalgo if enabled
            ExcuseLabel.Text = AppStrings.IsZalgoMode ? AppStrings.Zalgoify(_currentExcuse) : _currentExcuse;
            
            _statsService.IncrementExcusesGenerated();
            UpdateCounterLabel();
        }
        catch (Exception ex)
        {
            ExcuseLabel.Text = $"Error: {ex.Message}";
        }
        finally
        {
            GenerateBtn.IsEnabled = true;
        }
    }

    private async void OnShareClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_currentExcuse)) return;
        
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = _currentExcuse,
            Title = AppStrings.GetString("ShareExcuse")
        });
    }
}
