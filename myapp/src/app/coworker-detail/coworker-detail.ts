import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ClientService } from '../services/client.service';

@Component({
  selector: 'app-coworker-detail',
  imports: [RouterLink],
  templateUrl: './coworker-detail.html',
  styleUrl: './coworker-detail.scss',
})
export class CoworkerDetail implements OnInit {
  coworker = signal<any | null>(null);
  error = signal(false);

  constructor(private route: ActivatedRoute, private clientService: ClientService) { }

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.clientService.getCoworker(id).subscribe({
      next: c => this.coworker.set(c),
      error: () => this.error.set(true),
    });
  }
}
