using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public partial class MinifigGeneratorGameView : ContentView
{
    // Hair/headwear parts - more LEGO-like
    private static readonly string[] HairStyles = [
        "===",    // Flat top
        "/\\",    // Pointy
        "~~~",    // Wavy
        "(O)",    // Helmet
        "|||",    // Spiky
        "^^^",    // Crown
        "---",    // Bald/cap
        "ooo",    // Curly
        "***",    // Afro
        "###"     // Hat
    ];
    
    // Faces - classic minifig expressions
    private static readonly string[] Faces = [
        "o  o\n  <  \n\\__/",   // Happy
        "o  o\n  <  \n/--\\",   // Neutral
        "-  -\n  <  \n\\__/",   // Chill
        "o  o\n  <  \n/  \\",   // Worried
        "^  ^\n  <  \n\\__/",   // Excited
        "o  o\n  <  \n---",     // Serious
        "*  *\n  <  \n\\__/",   // Starstruck
        "o  o\n  O  \n\\__/",   // Surprised
        "o  -\n  <  \n\\__/",   // Winking
        ">  <\n  <  \n\\__/"    // Angry
    ];
    
    // Simplified face for single line display
    private static readonly string[] SimpleFaces = ["o_o", "^_^", "-_-", "O_O", ">_<", "=_=", "*_*", ";_)", "._.", "~_~"];
    
    private static readonly string[] TorsoPatterns = ["[###]", "[   ]", "[ X ]", "[ = ]", "[ | ]", "[ ~ ]", "[ * ]", "[ + ]", "[ / ]", "[ @ ]"];
    private static readonly string[] LegStyles = ["| |", "| |", "/ \\", "| |", "| |", "| |", "| |"];
    private static readonly string[] Accessories = ["", "sword", "shield", "cup", "tool", "wand", "coin", "gem", "key", "book"];
    
    private static readonly string[] HairColors = ["#8B4513", "#FFD700", "#1C1C1C", "#FF6B35", "#8B0000", "#CD853F", "#A0522D", "#F5F5DC"];
    private static readonly string[] TorsoColors = ["#EF4444", "#3B82F6", "#22C55E", "#F59E0B", "#8B5CF6", "#EC4899", "#14B8A6", "#6366F1"];
    private static readonly string[] LegColors = ["#1E40AF", "#1C1C1C", "#7C3AED", "#059669", "#DC2626", "#92400E"];
    
    private static readonly string[] FirstNames = ["Brick", "Block", "Stud", "Plate", "Minnie", "Figgy", "Lloyd", "Emmet", "Benny", "Wyld"];
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
        // Random parts
        HairLabel.Text = HairStyles[Random.Shared.Next(HairStyles.Length)];
        FaceLabel.Text = SimpleFaces[Random.Shared.Next(SimpleFaces.Length)];
        TorsoLabel.Text = TorsoPatterns[Random.Shared.Next(TorsoPatterns.Length)];
        LegsLabel.Text = LegStyles[Random.Shared.Next(LegStyles.Length)];
        
        var accessory = Accessories[Random.Shared.Next(Accessories.Length)];
        AccessoryLabel.Text = accessory != "" ? $"[{accessory}]" : "";
        
        // Random colors
        HairLabel.TextColor = Color.FromArgb(HairColors[Random.Shared.Next(HairColors.Length)]);
        TorsoLabel.TextColor = Color.FromArgb(TorsoColors[Random.Shared.Next(TorsoColors.Length)]);
        LegsLabel.TextColor = Color.FromArgb(LegColors[Random.Shared.Next(LegColors.Length)]);
        
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
