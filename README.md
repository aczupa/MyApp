MyApp : gestion de clients et suivi de la TVA

Projet en cours de développement. Je travaille actuellement sur cette application et j'ajoute régulièrement de nouvelles fonctionnalités.
<br>
Application web de gestion de clients avec suivi des échéances de TVA. Un utilisateur connecté consulte la liste et les fiches clients ; un administrateur peut en plus ajouter, modifier et supprimer des clients, et valider les déclarations de TVA mois par mois.
<br>
Le projet se compose d'un front Angular et d'une API ASP.NET Core reliée à SQL Server.
<br>

Fonctionnalités
Authentification : inscription, connexion et déconnexion (ASP.NET Core Identity, jeton Bearer).
Rôles : Admin et utilisateur standard, avec contrôle d'accès côté Angular (guards) et côté API ([Authorize]).
Clients : liste, fiche détaillée en lecture seule, ajout, modification et suppression (administrateur).
Formes juridiques françaises et monégasques (SARL, SAS, SCI, SAM, etc.) sélectionnables dans le formulaire.
TVA par client : périodicité (mensuelle, trimestrielle ou non concerné) et jour de déclaration (19, 20, 21, 24 ou 25). Les mois sans échéance passent automatiquement à « Non concerné ».
Filtre par mois et par année : n'affiche que les clients ayant une échéance de TVA sur la période choisie, avec la date correspondante (par exemple 21/01/2026).
Validation de la TVA : l'administrateur marque la TVA d'un client comme « validée » pour un mois donné.
Vue annuelle par client : les douze mois d'une année avec le statut « Validé » ou « À valider ».
<br>
Aperçu
## Aperçu

### Connexion

<img src="readme/connexion.png" alt="Page de connexion" width="500">

<br>

### Inscription

<img src="readme/inscription.png" alt="Création de compte" width="500">

<br>

### Liste des clients

<img src="readme/page1.png" alt="Liste des clients" width="500">

<br>

### Fiche client

<img src="readme/client.png" alt="Fiche client" width="500">

<br>

### Suivi de la TVA

<img src="readme/tva.png" alt="Suivi de la TVA" width="500">
<br>

Technologies
Partie	Technologies
Front	Angular 20 (composants standalone, signaux, mode zoneless, rendu serveur), SCSS
API	ASP.NET Core 8, Entity Framework Core 8, ASP.NET Core Identity
Base de données	SQL Server (Express)
Documentation de l'API	Swagger (Swashbuckle)

<br>
Structure du dépôt
MyApp/
├── MyApp.api/    API ASP.NET Core (contrôleurs, modèles, migrations)
├── myapp/        Application Angular
└── readme/       Captures d'écran
<br>
Dans myapp/src/app/ :
<br>
login/            page de connexion
register/         page d'inscription
clients/          liste des clients et filtre TVA
client-detail/    fiche client et vue annuelle de la TVA
client-form/      formulaire d'ajout et de modification
guards/           protection des routes (connecté, administrateur)
interceptors/     ajout du jeton aux requêtes HTTP
services/         accès à l'API (clients, authentification)
<br>
Installation
Prérequis
.NET 8 SDK
Node.js 20.19+ ou 22.12+
Angular CLI : npm install -g @angular/cli
SQL Server ou SQL Server Express
<br>
1. API

Les fichiers appsettings.json ne sont pas versionnés. Créez MyApp.api/appsettings.json avec votre chaîne de connexion :
<br>

json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=MyAppDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  },
  "AllowedHosts": "*"
}
<br>
Créez la base et lancez l'API :

bash
cd MyApp.api
dotnet ef database update
dotnet run --launch-profile http

L'API écoute sur http://localhost:5115 et Swagger est disponible sur http://localhost:5115/swagger.

dotnet ef nécessite l'outil : dotnet tool install --global dotnet-ef. Dans Visual Studio, la commande équivalente est Update-Database.
<br>
2. Front Angular
bash
cd myapp
npm install
ng serve -o

L'application s'ouvre sur http://localhost:4200. L'adresse de l'API est définie dans src/app/services/ (auth.service.ts et client.service.ts).
<br>
3. Compte administrateur

Au démarrage, l'API crée le rôle Admin et l'attribue au compte test@test.com s'il existe (voir Program.cs, adresse à adapter). Pour obtenir un administrateur :

créez ce compte depuis la page d'inscription ;
redémarrez l'API ;
reconnectez-vous, car le rôle est inscrit dans le jeton au moment de la connexion.

Le mot de passe doit contenir au moins 6 caractères, une majuscule, une minuscule, un chiffre et un caractère spécial.
<br>
Points d'accès de l'API
Méthode	Route	Accès
POST	/register, /login	public
GET	/api/account/me	connecté
GET	/api/client (filtres facultatifs annee et mois)	connecté
GET	/api/client/{id} (paramètre facultatif annee)	connecté
GET	/api/client/formes-juridiques, /api/client/periodicites-tva	connecté
POST	/api/client	Admin
PUT	/api/client/{id}	Admin
DELETE	/api/client/{id}	Admin
PUT	/api/client/{id}/tva-validation	Admin
Règles de la TVA
Mensuelle : une échéance chaque mois, au jour choisi.
Trimestrielle : une échéance en janvier, avril, juillet et octobre, au jour choisi.
Non concerné : aucune échéance.

