import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ClientService } from '../services/client.service';
import { AuthService } from '../services/auth.service';
import { LogoutButton } from '../logout-button/logout-button';

@Component({
  selector: 'app-clients',
  imports: [LogoutButton, RouterLink],
  templateUrl: './clients.html',
  styleUrl: './clients.scss',
})
export class Clients implements OnInit {
  clients = signal<any[]>([]);

  constructor(private clientService: ClientService, protected auth: AuthService) { }

  ngOnInit() {
    this.clientService.getClients().subscribe(data => {
      this.clients.set(data as any[]);
    });
  }
}
