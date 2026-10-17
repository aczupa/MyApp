import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
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
  };

  formes = signal<{ value: string; label: string }[]>([]);
  error = signal('');

  constructor(private clientService: ClientService, private router: Router) { }

  ngOnInit() {
    this.clientService.getFormesJuridiques().subscribe(f => this.formes.set(f));
  }

  save() {
    if (!this.model.denominationSociale.trim() || !this.model.formeJuridique) {
      this.error.set('Veuillez renseigner la dénomination sociale et la forme juridique.');
      return;
    }

    this.clientService.addClient(this.model).subscribe({
      next: () => this.router.navigate(['/clients']),
      error: () => this.error.set("Impossible d'enregistrer le client."),
    });
  }
}
