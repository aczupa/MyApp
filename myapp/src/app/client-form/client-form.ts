
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


  id: number | null = null;


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

  
  error = signal('');
  loading = signal(false);
  pret = signal(false);

  constructor(
    private clientService: ClientService,
    private router: Router,
    private route: ActivatedRoute
  ) { }


  private readonly defauts: Record<string, string> = {
    SARL: 'IS',
    SAS: 'IS',
    SASU: 'IS',
  };
  ngOnInit(): void {

    this.clientService.getFormesJuridiques().subscribe({
      next: formes => this.formes.set(formes),
      error: () => this.error.set('Impossible de charger les formes juridiques.'),
    });

    this.clientService.getPeriodicitesTva().subscribe({
      next: periodicites => this.periodicites.set(periodicites),
      error: () => this.error.set('Impossible de charger les périodicités de TVA.'),
    });

    this.clientService.getCoworkers().subscribe({
      next: coworkers => this.coworkers.set(coworkers),
      error: () => this.error.set('Impossible de charger les collaborateurs.'),
    });

    this.clientService.getRegimesFiscaux().subscribe({
      next: result => {
        this.modes.set(result.modes);
        this.regimes.set(result.regimes);
      },
      error: () => this.error.set('Impossible de charger les informations fiscales.'),
    });

    const id = this.route.snapshot.paramMap.get('id');

    if (id !== null) {
      this.id = +id;

      this.clientService.getClient(this.id).subscribe({
        next: client => {
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
            regimeFiscal: client.regimeFiscal ?? 'NonRenseigne',
          };
          this.pret.set(true);
        },
        error: () => this.error.set('Impossible de charger les informations du client.'),
      });
    } else {
      this.pret.set(true);
    }
  }
  get monaco(): boolean {
    return this.formes().some(
      f => f.value === this.model.formeJuridique && f.monaco
    );
  }

  get modesDisponibles() {
    const systeme = this.monaco ? 'MC' : 'FR';

    return this.modes().filter(
      m => m.systeme === null || m.systeme === systeme
    );
  }

  changerForme(valeur: string): void {
    this.model.formeJuridique = valeur;

    const defaut = this.defauts[valeur];

    if (defaut) {
      this.model.modeImposition = defaut;
    } else if (!this.modesDisponibles.some(m => m.value === this.model.modeImposition)) {
      this.model.modeImposition = 'NonRenseigne';
    }

    if (this.monaco) {
      this.model.regimeFiscal = 'NonRenseigne';
    }
  }

  save(): void {
    if (this.loading()) {
      return;
    }

    this.error.set('');

    if (!this.model.denominationSociale.trim() || !this.model.formeJuridique) {
      this.error.set('Veuillez renseigner la dénomination sociale et la forme juridique.');
      return;
    }

    if (this.model.periodiciteTva === 'NonConcerne') {
      this.model.jourTva = null;
    } else if (!this.model.jourTva || !this.jours.includes(this.model.jourTva)) {
      this.error.set('Veuillez choisir un jour de déclaration de TVA valide.');
      return;
    }

    if (!this.modesDisponibles.some(m => m.value === this.model.modeImposition)) {
      this.error.set('Mode d’imposition invalide.');
      return;
    }

    if (this.monaco) {
      this.model.regimeFiscal = 'NonRenseigne';
    }

    this.loading.set(true);

    const request = this.id !== null
      ? this.clientService.updateClient(this.id, this.model)
      : this.clientService.addClient(this.model);

    request.subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/clients']);
      },
      error: () => {
        this.loading.set(false);
        this.error.set("Impossible d'enregistrer le client.");
      },
    });
  }
}
