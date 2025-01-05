# WeatherApp

WeatherApp est une application de bureau utilisant le framework Avalonia pour afficher des informations météorologiques en temps réel.

## Prérequis

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) ou version supérieure
- Un éditeur de code comme [Visual Studio](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/) avec les extensions nécessaires pour .NET

## Installation

1. Clonez ce dépôt :
   ```bash
   git clone https://github.com/stanthblt/WeatherApp
   cd WeatherApp
   ```

2. Restaurez les dépendances :
   ```bash
   dotnet restore
   ```

3. Construisez le projet :
   ```bash
   dotnet build
   ```

4. Exécutez l'application :
   ```bash
   dotnet run
   ```

## Fonctionnalités

- **Affichage en temps réel des informations météorologiques** : L'application récupère les données météorologiques et les affiche dans une interface utilisateur simple et moderne.
- **Icônes dynamiques** : Les icônes des conditions météorologiques changent en fonction des données reçues.
- **Interface utilisateur avec Avalonia** : L'interface est construite avec Avalonia, un framework multiplateforme pour les applications de bureau.

## Structure du projet

- **App.axaml** : Fichier principal de l'interface utilisateur.
- **App.axaml.cs** : Logique derrière le fichier `App.axaml`.
- **MainWindow.axaml** : Interface principale de l'application.
- **MainWindow.axaml.cs** : Logique de la fenêtre principale.
- **Models** : Contient les classes représentant les données de l'application, comme `AppSettings.cs` et `WeatherData.cs`.
- **Services** : Contient les services responsables de la récupération des données météorologiques et de la gestion des paramètres.
