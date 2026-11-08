import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ClientService } from '../services/client.service';
import { AuthService } from '../services/auth.service';
import { LogoutButton } from '../logout-button/logout-button';
import { MOIS } from '../mois';

@Component({
  selector: 'app-clients',
  imports: [LogoutButton, RouterLink],
  templateUrl: './clients.html',
  styleUrl: './clients.scss',
})
export class Clients implements OnInit {
  clients = signal<any[]>([]);
  coworkers = signal<{ id: number; nom: string }[]>([]);
  coworkerId = signal('');
  filtreActif = signal(false);
  protected mois = MOIS;
  protected annee = new Date().getFullYear();
  private filtre: { mois: string; annee: string } | undefined;

  constructor(
    private clientService: ClientService,
    private route: ActivatedRoute,
    private router: Router,
    protected auth: AuthService
  ) { }

  ngOnInit() {
    this.clientService.getCoworkers().subscribe(w => this.coworkers.set(w));

    this.route.queryParamMap.subscribe(p => {
      this.coworkerId.set(p.get('coworker') ?? '');
      this.load();
    });
  }

  setFiltre(mois: string, annee: string) {
    const actif = !!mois && !!annee;
    this.filtre = actif ? { mois, annee } : undefined;
    this.filtreActif.set(actif);
    this.load();
  }

  setCoworker(id: string) {
    this.router.navigate([], {
      queryParams: { coworker: id || null },
      queryParamsHandling: 'merge',
    });
  }

  valider(client: any, validee: boolean) {
    if (!this.filtre) return;

    this.clientService
      .setTvaValidation(client.id, +this.filtre.annee, +this.filtre.mois, validee)
      .subscribe(() => {
        this.clients.update(list => list.map(c => c.id === client.id ? { ...c, validee } : c));
      });
  }

  delete(id: number) {
    if (!confirm('Supprimer ce client ?')) return;

    this.clientService.deleteClient(id).subscribe(() => {
      this.clients.update(list => list.filter(c => c.id !== id));
    });
  }

  private load() {
    const params: Record<string, string> = { ...this.filtre };
    if (this.coworkerId()) params['coworkerId'] = this.coworkerId();

    this.clientService.getClients(params).subscribe(data => {
      this.clients.set(data as any[]);
    });
  }
}
