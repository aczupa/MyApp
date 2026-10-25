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
  protected mois = MOIS;
  constructor(
    private route: ActivatedRoute,
    private clientService: ClientService,
    protected auth: AuthService
  ) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.clientService.getClient(id).subscribe({
      next: data => {
        this.client.set(data);
      },
      error: err => {
        console.error('Erreur lors du chargement du client', err);
        this.error.set(true);
      }
    });
  }
}
