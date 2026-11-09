<p align="center">
  <img width="260" alt="Logo Cajaz Royale" src="Assets/Images/Logo%20Jeu.png" />
</p>

<h1 align="center">Cajaz Royale</h1>

<p align="center">
  Battle royale 2D en vue de dessus, multijoueur en ligne, développé avec <b>Unity</b> et <b>C#</b>.
</p>

<p align="center">
  <img alt="Unity" src="https://img.shields.io/badge/Unity-000000?logo=unity&logoColor=white" />
  <img alt="C#" src="https://img.shields.io/badge/C%23-512BD4?logo=dotnet&logoColor=white" />
  <img alt="Photon PUN 2" src="https://img.shields.io/badge/Multijoueur-Photon%20PUN%202-004480" />
</p>

---

## Présentation

Les joueurs apparaissent sur une carte urbaine **générée aléatoirement à chaque partie**. Ils doivent récupérer des armes et des objets, s'affronter, et survivre à une **zone qui rétrécit** jusqu'à ce qu'il ne reste qu'un seul survivant.

Projet de groupe réalisé à l'**EPITA** (2024-2025).

## Fonctionnalités

- **Multijoueur en ligne** avec Photon PUN 2 : lobby, salons, synchronisation des déplacements, du tir, de la vie et des animations.
- **Mode solo** contre des ennemis contrôlés par une IA.
- **Génération procédurale de la carte** : routes (algorithme du *random walker*), bâtiments, décors et végétation placés par bruit.
- **Zone de combat** qui se resserre et inflige des dégâts aux joueurs restés dehors.
- **IA des ennemis** : machine à états (patrouille, poursuite, attaque) et recherche de chemin A*.
- **Combat** : plusieurs armes, rechargement, explosions, points de vie et respawn.
- **Inventaire** et objets à ramasser dans les coffres ou au sol.
- **Menus complets** : menu principal, paramètres (son, affichage), pause, fin de partie, crédits.
- **Rendu URP** avec éclairage 2D, ombres et caméra Cinemachine.

## Commandes

<p align="center">
  <img width="600" alt="Commandes du jeu" src="Assets/Images/Controls.png" />
</p>

| Touche | Action |
|---|---|
| `W` `A` `S` `D` | Se déplacer |
| Clic gauche | Tirer |
| `R` | Recharger |
| `E` | Interagir / ramasser |
| `I` | Ouvrir l'inventaire |
| `1` à `4` | Changer d'arme |

## Organisation du code

Les scripts du jeu se trouvent dans [`Assets/scripts/`](Assets/scripts) :

| Dossier / fichier | Rôle |
|---|---|
| `MapGen/` | Génération procédurale : routes (`WalkerGenerator`), bâtiments (`CityBuider`, `Batiment7`), bruit (`Noise`) |
| `AI/` | IA des ennemis : machine à états (`StateManager`, `PatrolState`, `ChaseState`, `AttackState`) et apparition (`Spawner`) |
| `Inventory/` | Inventaire, objets et interface associée |
| `Menus/` | Menu principal, lobby, paramètres, pause, fin de partie, crédits |
| `Game.cs`, `GameSolo.cs` | Déroulement d'une partie en multijoueur et en solo |
| `RoomManager.cs`, `PlayerSetup.cs` | Connexion Photon, création des salons et apparition des joueurs |
| `DamageCircle.cs` | Zone qui rétrécit |
| `Health.cs`, `Bullet.cs`, `gun.cs` | Vie, projectiles et armes |

Les scènes principales sont dans [`Assets/Scenes/`](Assets/Scenes) : `MainMenu`, `SampleScene` (partie multijoueur) et `Solo`.

## Lancer le projet

1. Créer un nouveau projet **Unity 2D (URP)**.
2. Remplacer son dossier `Assets/` par celui de ce dépôt.
3. Dans `Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings`, renseigner votre propre **App ID Photon PUN**.
4. Ouvrir la scène `Assets/Scenes/MainMenu.unity` et lancer le jeu.

## Équipe

- **Charles Roussel** : réseau et multijoueur, génération procédurale, zone de combat, mode solo, intégration
- **Jad Bendali** : assets graphiques, animations, tiles et inventaire
- **Abdoul-Hakim Mahamat Alhabo** : menus et interface
- **Muhammad-Zuhayr Shaikh** : IA des ennemis
