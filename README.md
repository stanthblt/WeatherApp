# WeatherApp 🌤️

**WeatherApp** est une application de bureau utilisant le framework **Avalonia** pour afficher des informations météorologiques en temps réel grâce à l'API de **OpenWeatherMap**.

## Sommaire

- [Fonctionnalités](#fonctionnalités)
- [Prérequis](#prérequis)
- [Installation](#installation)
- [Utilisation](#utilisation)
- [Structure du projet](#structure-du-projet)



## Fonctionnalités

- **Affichage en temps réel des informations météorologiques** : L'application récupère les données météorologiques et les affiche dans une interface utilisateur simple et moderne.
- **Icônes dynamiques** : Les icônes des conditions météorologiques changent en fonction des données reçues.
- **Ville par défaut** : L'utilisateur peut choisir une ville par défaut, qui sera sauvegardée.
- **Différents thèmes** : L'application propose un thème sombre et un thème clair, que l'utilisateur peut choisir en fonction de ses préférences.
- **Traduction de l'application** : L'utilisateur peut traduire l'application dans l'une des cinq langues disponibles.
- **Interface utilisateur avec Avalonia** : L'interface est construite avec **Avalonia**, un framework multiplateforme pour les applications de bureau.

---

## Prérequis

- [**.NET 9.0 SDK**](https://dotnet.microsoft.com/download/dotnet/9.0) ou version supérieure.
- Un éditeur de code comme [**Visual Studio**](https://visualstudio.microsoft.com/) ou [**VS Code**](https://code.visualstudio.com/) avec les extensions nécessaires pour .NET.
- Obtenez une clé API OpenWeatherMap : [Créer une clé API ici](https://openweathermap.org/).

---

## Installation

1. **Clonez ce dépôt** :
   ```bash
   git clone https://github.com/stanthblt/WeatherApp
   cd WeatherApp

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
5. Entrez votre clé API dans la fenêtre qui s'affiche

---

## Utilisation

1. **Clé API** : Lors du premier lancement entrez votre clé API OpenWeatherMap dans la fenêtre qui s'affiche lors du lancement de l'application.
2. **Interface utilisateur** : L'interface utilisateur est simple et intuitive. Vous pouvez voir les informations météorologiques de la ville par défaut ou chercher une autre ville dans la search bar .
3. **Thèmes** : Vous pouvez choisir entre un thème sombre et un thème clair en choisisant dans la page Settings.
4. **Langues** : Vous pouvez choisir entre 5 langues différentes pour l'application en choisisant dans la page Settings.
5. **Ville par défaut** : Vous pouvez choisir une ville par défaut depuis la page Settings la météo de cette ville sera affiché par défaut et enregistré par l'aplication .
6. **Météo à 5 jours** : Vous pouvez voir la météo des 5 prochains jours en cliquant sur l'onglet "5 days forecast".

---

## Structure du projet

- **App.axaml** : Fichier principal de l'interface utilisateur.
- **App.axaml.cs** : Logique derrière le fichier `App.axaml`.
- **MainWindow.axaml** : Interface principale de l'application.
- **MainWindow.axaml.cs** : Logique de la fenêtre principale.
- **Models** : Contient les classes représentant les données de l'application, comme `AppSettings.cs` et `WeatherData.cs`.
- **Services** : Contient les services responsables de la récupération des données météorologiques et de la gestion des paramètres.


Cette application a été réalisé lors d'un projet dans le cadre de nos études a [Bordeaux Ynov Campus](https://www.ynov.com/campus/bordeaux)

Il a été réalisé par [**Chort Maxime**](https://github.com/Slaaaayz), [**Thabault Stanislas**](https://github.com/stanthblt/) et [**Prigent Nicolas**](https://github.com/nicoocaa)
