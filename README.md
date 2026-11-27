# Fred Mahjong

Fred Mahjong est une application de jeu de Mahjong en solitaire développée avec **.NET MAUI**. L'interface utilise XAML et C# et propose une partie sur une grille de 8 × 6 tuiles, avec une couche supérieure.

## Fonctionnalités

- Plateau de Mahjong composé de 52 tuiles en deux couches ;
- association de tuiles identiques ;
- détection des tuiles disponibles selon leur couverture et leurs côtés libres ;
- système de score ;
- indice proposant une paire jouable ;
- mélange des tuiles restantes ;
- annulation de la dernière paire retirée ;
- nouvelle partie ;
- interface en français avec des symboles Mahjong.

## Plateformes

Le projet est configuré pour :

- Windows ;
- Android.

Les frameworks ciblés sont définis dans [FredMahjong.csproj](FredMahjong/FredMahjong.csproj).

## Prérequis

- Visual Studio 2022 ou une installation récente du SDK .NET ;
- .NET 9 SDK ;
- charge de travail **.NET MAUI** ;
- pour Android : la charge de travail `maui-android` ;
- pour Windows : la charge de travail `maui-windows`.

Les workloads peuvent être installés avec :

```powershell
dotnet workload restore .\FredMahjong.sln
```

## Démarrage

### Avec Visual Studio

1. Ouvrir `FredMahjong.sln`.
2. Sélectionner la plateforme Windows ou Android.
3. Appuyer sur **Démarrer** ou sur `F5`.

### Depuis le terminal

Compilation de la solution :

```powershell
dotnet build .\FredMahjong.sln
```

Compilation et exécution sur Windows :

```powershell
dotnet build .\FredMahjong\FredMahjong.csproj -f net9.0-windows10.0.19041.0
dotnet run --project .\FredMahjong\FredMahjong.csproj -f net9.0-windows10.0.19041.0
```

Compilation pour Android :

```powershell
dotnet build .\FredMahjong\FredMahjong.csproj -f net9.0-android
```

## Règles du jeu

Une tuile peut être retirée lorsqu'elle n'est pas recouverte par une tuile située sur une couche supérieure et lorsqu'au moins un côté horizontal est libre.

Sélectionner une tuile puis sa paire identique retire les deux tuiles et ajoute 100 points au score. Les boutons disponibles permettent de :

- afficher une paire jouable ;
- mélanger les faces des tuiles restantes ;
- annuler la dernière paire retirée ;
- démarrer une nouvelle partie.

## Structure du projet

```text
FredMahjong/
├── FredMahjong.sln
└── FredMahjong/
    ├── App.xaml                 # Ressources globales de l'application
    ├── App.xaml.cs
    ├── AppShell.xaml            # Navigation principale
    ├── AppShell.xaml.cs
    ├── MainPage.xaml            # Interface du plateau
    ├── MainPage.xaml.cs         # Logique du jeu
    ├── MauiProgram.cs           # Configuration MAUI
    ├── Platforms/               # Configuration Windows et Android
    └── Resources/               # Images, polices et autres ressources
```

## Licence

Ce projet est distribué sous licence MIT. Consultez [LICENSE.txt](LICENSE.txt) pour les informations complètes.

---

# English

Fred Mahjong is a solitaire Mahjong game built with **.NET MAUI**. The interface is implemented with XAML and C# and uses an 8 × 6 tile board with an additional upper layer.

## Features

- 52 tiles displayed across two layers;
- identical tile matching;
- tile availability detection based on upper-layer coverage and free horizontal sides;
- score tracking;
- hint for an available matching pair;
- shuffle of the remaining tiles;
- undo for the last removed pair;
- new game action;
- French interface with Mahjong symbols.

## Platforms

The project supports:

- Windows;
- Android.

The targeted frameworks are defined in [FredMahjong.csproj](FredMahjong/FredMahjong.csproj).

## Prerequisites

- Visual Studio 2022 or a recent .NET SDK installation;
- .NET 9 SDK;
- .NET MAUI workload;
- `maui-android` for Android;
- `maui-windows` for Windows.

Install the project workloads with:

```powershell
dotnet workload restore .\FredMahjong.sln
```

## Getting Started

### Visual Studio

1. Open `FredMahjong.sln`.
2. Select Windows or Android.
3. Press **Start** or `F5`.

### Terminal

Build the solution:

```powershell
dotnet build .\FredMahjong.sln
```

Build and run on Windows:

```powershell
dotnet build .\FredMahjong\FredMahjong.csproj -f net9.0-windows10.0.19041.0
dotnet run --project .\FredMahjong\FredMahjong.csproj -f net9.0-windows10.0.19041.0
```

Build for Android:

```powershell
dotnet build .\FredMahjong\FredMahjong.csproj -f net9.0-android
```

## Game Rules

A tile can be removed when it is not covered by an upper-layer tile and at least one horizontal side is free.

Select a tile and then its identical pair to remove both tiles and add 100 points to the score. The available buttons allow the player to:

- show an available matching pair;
- shuffle the faces of the remaining tiles;
- undo the last removed pair;
- start a new game.

## License

This project is licensed under the MIT License. See [LICENSE.txt](LICENSE.txt) for details.
