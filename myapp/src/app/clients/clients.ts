import { Component, OnInit, signal } from '@angular/core';
import { ClientService } from '../services/client.service';
import { LogoutButton } from '../logout-button/logout-button';

@Component({
  selector: 'app-clients',
  imports: [LogoutButton],
  templateUrl: './clients.html',
  styleUrl: './clients.scss',
})
export class Clients implements OnInit {
  clients = signal<any[]>([]);

  constructor(private clientService: ClientService) { }

  ngOnInit() {
    this.clientService.getClients().subscribe(data => {
      this.clients.set(data as any[]);
    });
  }
}
