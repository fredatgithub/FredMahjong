using Microsoft.Maui.Controls.Shapes;

namespace FredMahjong;

public partial class MainPage : ContentPage
{
    private const int Columns = 8;
    private const int Rows = 6;
    private const int PairsInGame = 26;
    private static readonly Color Gold = Color.FromArgb("#F4D58A");
    private static readonly Color TileColor = Color.FromArgb("#F3E8CA");
    private static readonly Color TileEdge = Color.FromArgb("#D2BD91");
    private readonly List<TileModel> tiles = [];
    private readonly Stack<TileModel[]> moveHistory = [];
    private readonly Random random = new();
    private TileModel? selectedTile;
    private int score;
    private int removedPairs;

    public MainPage()
    {
        InitializeComponent();
        StartNewGame();
    }

    private void StartNewGame()
    {
        BoardGrid.Children.Clear();
        tiles.Clear();
        moveHistory.Clear();
        selectedTile = null;
        score = 0;
        removedPairs = 0;

        var faces = CreateFaces()
            .SelectMany(face => new[] { face, face })
            .OrderBy(_ => random.Next())
            .ToArray();

        for (var index = 0; index < Rows * Columns; index++)
        {
            var tile = new TileModel(index % Columns, index / Columns, 0, faces[index]);
            AddTile(tile);
        }

        var upperLayerPositions = new (int Column, int Row)[]
        {
            (3, 1), (4, 1), (3, 4), (4, 4)
        };

        for (var index = 0; index < upperLayerPositions.Length; index++)
        {
            var (Column, Row) = upperLayerPositions[index];
            var tile = new TileModel(Column, Row, 1, faces[Rows * Columns + index]);
            AddTile(tile);
        }

        UpdateGameInfo("Touchez deux tuiles identiques pour les retirer.");
        RefreshTileStates();
    }

    private static TileFace[] CreateFaces()
    {
        var suits = new[]
        {
            (Name: "萬", Color: Color.FromArgb("#B7443E")),
            (Name: "筒", Color: Color.FromArgb("#287A69")),
            (Name: "索", Color: Color.FromArgb("#3974A3"))
        };
        var numberedFaces = suits
            .SelectMany(suit => Enumerable.Range(1, 8)
                .Select(number => new TileFace($"{suit.Name}{number}", number.ToString(), suit.Name, suit.Color)))
            .ToArray();

        return
        [
          .. numberedFaces
,
          new TileFace("dragon-red", "中", "DRAGON", Color.FromArgb("#B7443E")),
          new TileFace("dragon-green", "發", "DRAGON", Color.FromArgb("#287A69"))
,
        ];
    }

    private void AddTile(TileModel tile)
    {
        var faceLabel = new Label
        {
            FontSize = tile.Face.Suit == "DRAGON" ? 22 : 24,
            FontAttributes = FontAttributes.Bold,
            TextColor = tile.Face.Color,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.End,
            VerticalOptions = LayoutOptions.Fill
        };
        var suitLabel = new Label
        {
            FontSize = 8,
            CharacterSpacing = 0.6,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#958766"),
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Start,
            VerticalOptions = LayoutOptions.Fill
        };
        tile.FaceLabel = faceLabel;
        tile.SuitLabel = suitLabel;
        UpdateFaceLabels(tile);

        var content = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            },
            RowSpacing = 0,
            Padding = new Thickness(1, 4, 1, 3)
        };
        content.Add(faceLabel);
        content.Add(suitLabel);
        Grid.SetRow(suitLabel, 1);

        var view = new Border
        {
            BackgroundColor = TileColor,
            Stroke = TileEdge,
            StrokeThickness = 1,
            Padding = 1,
            Margin = 2,
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) },
            Content = content,
            ZIndex = tile.Layer,
            TranslationX = tile.Layer * 3,
            TranslationY = tile.Layer * -4
        };
        tile.View = view;

        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => OnTileTapped(tile);
        view.GestureRecognizers.Add(tap);

        tiles.Add(tile);
        BoardGrid.Add(view, tile.Column, tile.Row);
    }

    private void OnTileTapped(TileModel tile)
    {
        if (removedPairs == PairsInGame)
        {
            return;
        }

        if (tile.IsRemoved || !IsPlayable(tile))
        {
            StatusLabel.Text = "Choisissez une tuile libre sur un côté.";
            return;
        }

        if (selectedTile == tile)
        {
            selectedTile = null;
            RefreshTileStates();
            return;
        }

        if (selectedTile is null)
        {
            selectedTile = tile;
            StatusLabel.Text = "Sélectionnez sa paire.";
            RefreshTileStates();
            return;
        }

        var firstTile = selectedTile;
        if (firstTile.Face.Id == tile.Face.Id)
        {
            moveHistory.Push([firstTile, tile]);
            RemoveTile(firstTile);
            RemoveTile(tile);
            selectedTile = null;
            removedPairs++;
            score += 100;
            UpdateGameInfo(removedPairs == PairsInGame
                ? "Félicitations ! Le jardin est libéré."
                : "Belle paire ! Continuez votre partie.");

            if (removedPairs < PairsInGame && !FindAvailablePair().Any())
            {
                StatusLabel.Text = "Plus de paire disponible. Mélangez les tuiles.";
            }
        }
        else
        {
            selectedTile = tile;
            StatusLabel.Text = "Ces tuiles ne forment pas une paire.";
        }

        RefreshTileStates();
    }

    private void OnNewGameClicked(object? sender, TappedEventArgs e)
    {
        StartNewGame();
    }

    private void RemoveTile(TileModel tile)
    {
        tile.IsRemoved = true;
        BoardGrid.Children.Remove(tile.View);
    }

    private void OnHintClicked(object? sender, TappedEventArgs e)
    {
        var pair = FindAvailablePair().FirstOrDefault();
        if (pair is null)
        {
            StatusLabel.Text = removedPairs == PairsInGame
                ? "Toutes les paires ont été trouvées !"
                : "Aucune paire disponible. Mélangez les tuiles.";
            return;
        }

        selectedTile = null;
        pair[0].IsHinted = true;
        pair[1].IsHinted = true;
        StatusLabel.Text = "Voici une paire identique à retirer.";
        RefreshTileStates();

        _ = ClearHintAsync(pair);
    }

    private async Task ClearHintAsync(TileModel[] pair)
    {
        await Task.Delay(TimeSpan.FromSeconds(1.5));
        if (pair.Any(tile => tile.IsRemoved))
        {
            return;
        }

        pair[0].IsHinted = false;
        pair[1].IsHinted = false;
        RefreshTileStates();
    }

    private void OnShuffleClicked(object? sender, TappedEventArgs e)
    {
        var remaining = tiles.Where(tile => !tile.IsRemoved).ToArray();
        var faces = remaining.Select(tile => tile.Face).OrderBy(_ => random.Next()).ToArray();
        for (var index = 0; index < remaining.Length; index++)
        {
            remaining[index].Face = faces[index];
            UpdateFaceLabels(remaining[index]);
        }

        selectedTile = null;
        moveHistory.Clear();
        tiles.ForEach(tile => tile.IsHinted = false);
        StatusLabel.Text = "Les tuiles restantes ont été mélangées.";
        RefreshTileStates();
    }

    private void OnUndoClicked(object? sender, TappedEventArgs e)
    {
        if (!moveHistory.TryPop(out var previousMove))
        {
            StatusLabel.Text = "Aucun coup à annuler.";
            return;
        }

        foreach (var tile in previousMove)
        {
            tile.IsRemoved = false;
            BoardGrid.Add(tile.View, tile.Column, tile.Row);
        }

        removedPairs--;
        score = Math.Max(0, score - 100);
        selectedTile = null;
        UpdateGameInfo("Dernière paire restaurée.");
        RefreshTileStates();
    }

    private IEnumerable<TileModel[]> FindAvailablePair()
    {
        return tiles
            .Where(tile => !tile.IsRemoved && IsPlayable(tile))
            .GroupBy(tile => tile.Face.Id)
            .Where(group => group.Count() > 1)
            .Select(group => group.Take(2).ToArray());
    }

    private bool IsPlayable(TileModel tile)
    {
        var covered = tiles.Any(other =>
            !other.IsRemoved &&
            other.Layer > tile.Layer &&
            other.Column == tile.Column &&
            other.Row == tile.Row);

        if (covered)
        {
            return false;
        }

        var blockedOnLeft = tiles.Any(other =>
            !other.IsRemoved &&
            other.Layer == tile.Layer &&
            other.Row == tile.Row &&
            other.Column == tile.Column - 1);
        var blockedOnRight = tiles.Any(other =>
            !other.IsRemoved &&
            other.Layer == tile.Layer &&
            other.Row == tile.Row &&
            other.Column == tile.Column + 1);

        return !blockedOnLeft || !blockedOnRight;
    }

    private void RefreshTileStates()
    {
        foreach (var tile in tiles.Where(tile => !tile.IsRemoved))
        {
            var playable = IsPlayable(tile);
            tile.IsSelected = tile == selectedTile;
            tile.View.Stroke = tile.IsSelected || tile.IsHinted
                ? Gold
                : playable ? TileEdge : Color.FromArgb("#B4AA92");
            tile.View.StrokeThickness = tile.IsSelected || tile.IsHinted ? 2.5 : 1;
            tile.View.Opacity = playable ? 1 : 0.66;
            tile.View.TranslationY = tile.Layer * -4 + (tile.IsSelected ? -3 : 0);
            tile.View.BackgroundColor = tile.IsSelected || tile.IsHinted
                ? Color.FromArgb("#FFF5D9")
                : TileColor;
        }

        PairsLabel.Text = $"{PairsInGame - removedPairs} paires";
        ScoreLabel.Text = score.ToString("D5");
    }

    private void UpdateGameInfo(string message)
    {
        StatusLabel.Text = message;
        RefreshTileStates();
    }

    private static void UpdateFaceLabels(TileModel tile)
    {
        tile.FaceLabel.Text = tile.Face.Value;
        tile.SuitLabel.Text = tile.Face.Suit == "DRAGON" ? "DRAGON" : tile.Face.Suit;
        tile.FaceLabel.TextColor = tile.Face.Color;
    }

    private sealed class TileModel(int column, int row, int layer, TileFace face)
    {
        public int Column { get; } = column;
        public int Row { get; } = row;
        public int Layer { get; } = layer;
        public TileFace Face { get; set; } = face;
        public bool IsRemoved { get; set; }
        public bool IsHinted { get; set; }
        public bool IsSelected { get; set; }
        public Border View { get; set; } = null!;
        public Label FaceLabel { get; set; } = null!;
        public Label SuitLabel { get; set; } = null!;
    }

    private sealed record TileFace(string Id, string Value, string Suit, Color Color);
}
