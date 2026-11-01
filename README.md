# Quizz Bot pour Streamer.bot

Système de quizz automatisé pour chat Twitch via Streamer.bot avec gestion du temps, vérification insensible à la casse et aux accents, et classement des joueurs.

---

## 1. Format du fichier de quizz (`MON_FICHIER.txt`)

Le script lit un fichier texte brut pour charger les questions et leurs réponses respectives.

### Règles d'écriture
* **Encodage obligatoire :** Enregistrez le fichier en **UTF-8** (pour éviter la corruption des caractères accentués).
* **Séparateur :** Utilisez uniquement le caractère barre verticale (`|`).
* **Format par ligne :** `Question|Réponse`
* **Une question par ligne :** Les retours à la ligne définissent une nouvelle question. Les lignes vides sont automatiquement ignorées.

### Exemple de contenu
```text
Quelle est la capitale de la France ?|Paris
Quel animal pond les plus gros œufs ?|Autruche
Combien de cœurs possède une pieuvre ?|3
En quelle année est sorti le premier film Star Wars ?|1977
Quel est le métal représenté par le symbole Fe ?|Fer
```

> **Remarque sur la détection :** Inutile de multiplier les variantes (avec ou sans majuscule, avec ou sans accent). Le script nettoie automatiquement les saisies des spectateurs et la réponse cible. Par exemple, si la réponse enregistrée est `Éléphant`, le bot validera aussi `elephant`, `ELEPHANT` ou `Éléphant`.

---

## 2. Installation dans Streamer.bot

### Action 1 : `QuizzStart`
1. Rendez-vous dans l'onglet **Actions** et créez une action nommée **`QuizzStart`**.
2. Ajoutez un sous-élément : **Core > C# > Execute C# Code**.
3. Collez le code C# de démarrage.
4. Modifiez la constante `FILE_PATH` en haut du script avec le chemin exact de votre fichier texte, par exemple :
   ```csharp
   private const string FILE_PATH = @"C:\StreamerBot\quizz.txt";
   ```
5. Cliquez sur **Save and Compile**.

### Action 2 : `QuizzAnswer`
1. Créez une action nommée **`QuizzAnswer`**.
2. Ajoutez un sous-élément : **Core > C# > Execute C# Code** et collez le script de détection des réponses.
3. Cliquez sur **Save and Compile**.
4. Dans le panneau **Triggers** (Déclencheurs) de cette action :
   * Clic droit > **Twitch > Chat > Chat Message**.

### Action 3 : `QuizzTop`
1. Créez une action nommée **`QuizzTop`**.
2. Ajoutez un sous-élément : **Core > C# > Execute C# Code** et collez le script de calcul du classement.
3. Cliquez sur **Save and Compile**.

---

## 3. Configuration des commandes de Chat

Dans l'onglet **Commands** de Streamer.bot, créez les deux commandes suivantes :

| Commande | Action liée | Accès recommandé | Description |
| :--- | :--- | :--- | :--- |
| `!quizzstart` | `QuizzStart` | **Modérateurs / Broadcaster** | Démarre la session de questions. |
| `!quizztop` | `QuizzTop` | **Tout le monde** | Affiche le top 5 actuel dans le chat. |

---

## 4. Déroulement d'une session

1. Un modérateur ou le streamer tape `!quizzstart` dans le chat Twitch.
2. Le bot annonce le nombre total de questions et pose la première.
3. Les spectateurs tapent directement leurs propositions dans le chat.
4. **Si quelqu'un trouve :** Le compte à rebours s'arrête immédiatement, le bot cite le gagnant, ajoute un point à son score, puis lance la question suivante après 3 secondes de pause.
5. **Si personne ne trouve après 60 secondes :** Le bot donne la réponse et passe automatiquement à la suite.
6. Une fois toutes les questions passées, le classement des scores est affiché automatiquement.
