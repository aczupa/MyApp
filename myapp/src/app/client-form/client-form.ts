
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ClientService } from '../services/client.service';

@Component({
  selector: 'app-client-form',
  imports: [FormsModule, RouterLink],
  templateUrl: './client-form.html',
  styleUrl: '../login/login.scss',
})
export class ClientForm implements OnInit {

  // Modèle du formulaire
  model = {
    denominationSociale: '',
    formeJuridique: '',
    adresse: '',
    ville: '',
    codePostal: '',
    pays: '',
    telephone: '',
    email: '',
    representantLegal: '',
    periodiciteTva: 'NonConcerne',
    jourTva: null as number | null,
    coworkerId: null as number | null,
    modeImposition: 'NonRenseigne',
    regimeFiscal: 'NonRenseigne',
  };

  // Identifiant du client (null = nouveau client)
  id: number | null = null;

  // Listes utilisées dans le formulaire
  formes = signal<{ value: string; label: string; monaco: boolean }[]>([]);

  modes = signal<{
    value: string;
    label: string;
    systeme: string | null;
  }[]>([]);

  regimes = signal<{ value: string; label: string }[]>([]);

  periodicites = signal<{ value: string; label: string }[]>([]);

  coworkers = signal<{ id: number; nom: string }[]>([]);

  jours = [19, 20, 21, 24, 25];

  // Gestion des messages et du chargement
  error = signal('');
  loading = signal(false);

  constructor(
    private clientService: ClientService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  // Valeurs par défaut du mode d'imposition en France
  private readonly defauts: Record<string, string> = {
    SARL: 'IS',
    SAS: 'IS',
    SASU: 'IS',
  };

  ngOnInit(): void {

    // Chargement des formes juridiques
    this.clientService.getFormesJuridiques().subscribe({
      next: formes => this.formes.set(formes),
      error: () => this.error.set(
        'Impossible de charger les formes juridiques.'
      ),
    });

    // Chargement des périodicités TVA
    this.clientService.getPeriodicitesTva().subscribe({
      next: periodicites => this.periodicites.set(periodicites),
      error: () => this.error.set(
        'Impossible de charger les périodicités de TVA.'
      ),
    });

    // Chargement des coworkers
    this.clientService.getCoworkers().subscribe({
      next: coworkers => this.coworkers.set(coworkers),
      error: () => this.error.set(
        'Impossible de charger les collaborateurs.'
      ),
    });

    // Chargement des modes et régimes fiscaux
    this.clientService.getRegimesFiscaux().subscribe({
      next: result => {
        this.modes.set(result.modes);
        this.regimes.set(result.regimes);
      },
      error: () => this.error.set(
        'Impossible de charger les informations fiscales.'
      ),
    });

    // Vérification de l'identifiant dans l'URL
    const id = this.route.snapshot.paramMap.get('id');

    if (id !== null) {

      const clientId = Number(id);

      if (!Number.isInteger(clientId) || clientId <= 0) {
        this.error.set('Identifiant client invalide.');
        return;
      }

      this.id = clientId;

      // Chargement du client à modifier
      this.clientService.getClient(this.id).subscribe({
        next: client => {

          console.log('Données reçues du serveur :', client);

          if (!client || !client.denominationSociale) {
            this.error.set('Les données du client sont absentes.');
            return;
          }

          this.model = {
            denominationSociale: client.denominationSociale ?? '',
            formeJuridique: client.formeJuridique ?? '',
            adresse: client.adresse ?? '',
            ville: client.ville ?? '',
            codePostal: client.codePostal ?? '',
            pays: client.pays ?? '',
            telephone: client.telephone ?? '',
            email: client.email ?? '',
            representantLegal: client.representantLegal ?? '',
            periodiciteTva: client.periodiciteTva ?? 'NonConcerne',
            jourTva: client.jourTva ?? null,
            coworkerId: client.coworkerId ?? null,
            modeImposition: client.modeImposition ?? 'NonRenseigne',
            regimeFiscal: client.regimeFiscal ?? 'NonRenseigne'
          };

          console.log('Modèle chargé :', this.model);
        },
        error: () => {
          this.error.set(
            'Impossible de charger les informations du client.'
          );
        },
      });
    }
  }

  // Vérifie si le client relève du système monégasque
  get monaco(): boolean {
    return this.formes().some(
      f =>
        f.value === this.model.formeJuridique &&
        f.monaco
    );
  }

  // Filtre les modes d'imposition selon le système fiscal
  get modesDisponibles() {
    const systeme = this.monaco ? 'MC' : 'FR';

    return this.modes().filter(
      m =>
        m.systeme === null ||
        m.systeme === systeme
    );
  }

  // Modification de la forme juridique
  changerForme(valeur: string): void {

    this.model.formeJuridique = valeur;

    const defaut = this.defauts[valeur];

    if (defaut) {

      this.model.modeImposition = defaut;

    } else if (
      !this.modesDisponibles.some(
        m => m.value === this.model.modeImposition
      )
    ) {

      this.model.modeImposition = 'NonRenseigne';
    }

    // Monaco : régime fiscal non renseigné
    if (this.monaco) {
      this.model.regimeFiscal = 'NonRenseigne';
    }
  }

  // Enregistrement du client
  save(): void {

    // Évite les doubles soumissions
    if (this.loading()) {
      return;
    }

    // Efface l'ancien message d'erreur
    this.error.set('');

    // Validation des champs obligatoires
    if (
      !this.model.denominationSociale.trim() ||
      !this.model.formeJuridique
    ) {
      this.error.set(
        'Veuillez renseigner la dénomination sociale et la forme juridique.'
      );
      return;
    }

    // Validation de la périodicité TVA
    if (this.model.periodiciteTva === 'NonConcerne') {

      this.model.jourTva = null;

    } else if (
      !this.model.jourTva ||
      !this.jours.includes(this.model.jourTva)
    ) {

      this.error.set(
        'Veuillez choisir un jour de déclaration de TVA valide.'
      );
      return;
    }

    // Vérification du mode d'imposition
    if (
      !this.modesDisponibles.some(
        m => m.value === this.model.modeImposition
      )
    ) {
      this.error.set('Mode d’imposition invalide.');
      return;
    }

    // Monaco : régime fiscal non renseigné
    if (this.monaco) {
      this.model.regimeFiscal = 'NonRenseigne';
    }

    // Début de l'enregistrement
    this.loading.set(true);

    // Création ou modification du client
    const request = this.id !== null
      ? this.clientService.updateClient(this.id, this.model)
      : this.clientService.addClient(this.model);

    request.subscribe({

      // Enregistrement réussi
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/clients']);
      },

      // Erreur lors de l'enregistrement
      error: () => {
        this.loading.set(false);
        this.error.set(
          "Impossible d'enregistrer le client."
        );
      },

    });
  }
}
