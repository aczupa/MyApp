import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ClientService } from '../services/client.service';
import { AuthService } from '../services/auth.service';
import { MOIS } from '../mois';

@Component({
  selector: 'app-client-detail',
  imports: [RouterLink],
  templateUrl: './client-detail.html',
  styleUrl: './client-detail.scss',
})
export class ClientDetail implements OnInit {

  client = signal<any | null>(null);
  error = signal(false);
  annee = signal(new Date().getFullYear());
  protected mois = MOIS;
  private id = 0;

  constructor(
    private route: ActivatedRoute,
    private clientService: ClientService,
    protected auth: AuthService
  ) { }

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.charger();
  }

  changerAnnee(value: string) {
    const annee = Number(value);
    if (!annee) return;

    this.annee.set(annee);
    this.charger();
  }

  valider(mois: number, validee: boolean) {
    this.clientService.setTvaValidation(this.id, this.annee(), mois, validee).subscribe(() => {
      this.client.update(c => ({
        ...c,
        echeances: c.echeances.map((e: any) => e.mois === mois ? { ...e, validee } : e),
      }));
    });
  }

  private charger() {
    this.clientService.getClient(this.id, this.annee()).subscribe({
      next: data => this.client.set(data),
      error: err => {
        console.error('Erreur lors du chargement du client', err);
        this.error.set(true);
      }
    });
  }
}
