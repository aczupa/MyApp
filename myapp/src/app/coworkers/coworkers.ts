import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ClientService } from '../services/client.service';
import { LogoutButton } from '../logout-button/logout-button';

@Component({
  selector: 'app-coworkers',
  imports: [RouterLink, LogoutButton],
  templateUrl: './coworkers.html',
  styleUrl: './coworkers.scss',
})
export class Coworkers implements OnInit {
  coworkers = signal<{ id: number; nom: string; nbClients: number }[]>([]);

  constructor(private clientService: ClientService) { }

  ngOnInit() {
    this.clientService.getCoworkers().subscribe(w => this.coworkers.set(w));
  }
}
