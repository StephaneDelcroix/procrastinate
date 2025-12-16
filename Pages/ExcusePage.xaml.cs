using procrastinate.Resources.Strings;
using procrastinate.Services;

namespace procrastinate.Pages;

public partial class ExcusePage : ContentPage
{
    private readonly StatsService _statsService;
    private readonly ExcuseService _excuseService;
    private readonly ImageGeneratorService _imageService;
    private string _currentExcuse = "";

    public ExcusePage(StatsService statsService, ExcuseService excuseService, ImageGeneratorService imageService)
    {
        InitializeComponent();
        _statsService = statsService;
        _excuseService = excuseService;
        _imageService = imageService;
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
        
        // Reset image state
        ExcuseImage.IsVisible = false;
        GenerateImageBtn.IsVisible = false;
        ShareIconBtn.IsVisible = false;
        
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
            
            // Show the generate image and share buttons
            GenerateImageBtn.IsVisible = true;
            ShareIconBtn.IsVisible = true;
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

    private async void OnGenerateImageClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_currentExcuse)) return;

        GenerateImageBtn.IsEnabled = false;
        ImageLoadingIndicator.IsVisible = true;
        ImageLoadingIndicator.IsRunning = true;

        try
        {
            var imageStream = await _imageService.GenerateImageAsync(_currentExcuse);
            
            if (imageStream != null)
            {
                ExcuseImage.Source = ImageSource.FromStream(() => imageStream);
                ExcuseImage.IsVisible = true;
            }
            else
            {
                await DisplayAlert("Oops", AppStrings.GetString("ImageGenerationFailed"), "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            ImageLoadingIndicator.IsVisible = false;
            ImageLoadingIndicator.IsRunning = false;
            GenerateImageBtn.IsEnabled = true;
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
