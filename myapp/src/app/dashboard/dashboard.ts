import { Component, OnInit, computed, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ClientService } from '../services/client.service';
import { LogoutButton } from '../logout-button/logout-button';
import { MOIS } from '../mois';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, LogoutButton],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  data = signal<any | null>(null);
  error = signal(false);
  protected mois = MOIS;

  sections = computed(() => {
    const d = this.data();
    const liste: { titre: string; stats: any }[] = [];
    if (!d) return liste;

    if (d.moi) liste.push({ titre: `Mon portefeuille : ${d.moi.nom}`, stats: d.moi.stats });
    liste.push({ titre: 'Tous les clients', stats: d.totaux });
    return liste;
  });

  constructor(private clientService: ClientService) { }

  ngOnInit() {
    this.clientService.getDashboard().subscribe({
      next: d => this.data.set(d),
      error: () => this.error.set(true),
    });
  }
}
