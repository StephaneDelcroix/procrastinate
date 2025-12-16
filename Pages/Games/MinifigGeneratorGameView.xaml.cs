using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public partial class MinifigGeneratorGameView : ContentView
{
    // Image sources for each part
    private static readonly string[] HairImages = [
        "hair_flat.png",
        "hair_flat_black.png", 
        "hair_swept.png",
        "hair_cap_red.png"
    ];
    
    private static readonly string[] HeadImages = [
        "head_yellow.png"
    ];
    
    private static readonly string[] TorsoImages = [
        "torso_red_plain.png",
        "torso_blue_plain.png",
        "torso_yellow.png"
    ];
    
    private static readonly string[] LegsImages = [
        "legs_blue.png",
        "legs_black.png",
        "legs_plain_black.png",
        "legs_plain_red.png"
    ];
    
    private static readonly string[] Accessories = ["", "sword", "shield", "cup", "tool", "wand", "coin", "gem", "key", "book", "pizza", "wrench", "guitar"];
    
    private static readonly string[] FirstNames = ["Brick", "Block", "Stud", "Plate", "Minnie", "Figgy", "Lloyd", "Emmet", "Benny", "Wyld", "Lucy", "Rex"];
    private static readonly string[] LastNames = ["Builder", "Master", "Maker", "Creator", "Smith", "Jones", "McBrick", "O'Block", "Von Stud", "Blockson"];
    private static readonly string[] Titles = ["the Brave", "the Builder", "the Wise", "the Swift", "the Bold", "the Creative", "the Tiny", "the Mighty", "Jr.", "III"];
    
    public Action? OnGamePlayed { get; set; }

    public MinifigGeneratorGameView()
    {
        InitializeComponent();
    }

    private void OnGenerateClicked(object? sender, EventArgs e)
    {
        OnGamePlayed?.Invoke();
        GenerateMinifig();
    }

    private void GenerateMinifig()
    {
        // Random parts - set image sources
        HairImage.Source = HairImages[Random.Shared.Next(HairImages.Length)];
        HeadImage.Source = HeadImages[Random.Shared.Next(HeadImages.Length)];
        TorsoImage.Source = TorsoImages[Random.Shared.Next(TorsoImages.Length)];
        LegsImage.Source = LegsImages[Random.Shared.Next(LegsImages.Length)];
        
        var accessory = Accessories[Random.Shared.Next(Accessories.Length)];
        AccessoryLabel.Text = accessory != "" ? $"Holding: {accessory}" : "";
        
        // Random name
        var firstName = FirstNames[Random.Shared.Next(FirstNames.Length)];
        var lastName = LastNames[Random.Shared.Next(LastNames.Length)];
        var title = Random.Shared.Next(100) < 40 ? " " + Titles[Random.Shared.Next(Titles.Length)] : "";
        NameLabel.Text = $"{firstName} {lastName}{title}";
        
        // Random description
        var descriptions = new[]
        {
            AppStrings.GetString("MinifigDesc1"),
            AppStrings.GetString("MinifigDesc2"),
            AppStrings.GetString("MinifigDesc3"),
            AppStrings.GetString("MinifigDesc4"),
            AppStrings.GetString("MinifigDesc5")
        };
        DescriptionLabel.Text = descriptions[Random.Shared.Next(descriptions.Length)];
    }

    public void Stop() { }
}
