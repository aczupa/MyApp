import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
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
  filtreActif = signal(false);
  protected mois = MOIS;
  protected annee = new Date().getFullYear();

  constructor(private clientService: ClientService, protected auth: AuthService) { }

  ngOnInit() {
    this.load();
  }

  setFiltre(mois: string, annee: string) {
    const actif = !!mois && !!annee;
    this.filtreActif.set(actif);
    this.load(actif ? { mois, annee } : undefined);
  }

  delete(id: number) {
    if (!confirm('Supprimer ce client ?')) return;

    this.clientService.deleteClient(id).subscribe(() => {
      this.clients.update(list => list.filter(c => c.id !== id));
    });
  }

  private load(filtre?: { mois: string; annee: string }) {
    this.clientService.getClients(filtre).subscribe(data => {
      this.clients.set(data as any[]);
    });
  }
}
