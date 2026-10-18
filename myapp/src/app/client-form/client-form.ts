import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
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

  id: number | null = null;
  formes = signal<{ value: string; label: string }[]>([]);
  error = signal('');

  constructor(
    private clientService: ClientService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.clientService.getFormesJuridiques().subscribe(f => this.formes.set(f));

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.id = +id;
      this.clientService.getClient(this.id).subscribe(c => {
        Object.assign(this.model, c);
        this.cdr.markForCheck();
      });
    }
  }

  save() {
    if (!this.model.denominationSociale.trim() || !this.model.formeJuridique) {
      this.error.set('Veuillez renseigner la dénomination sociale et la forme juridique.');
      return;
    }

    const request = this.id !== null
      ? this.clientService.updateClient(this.id, this.model)
      : this.clientService.addClient(this.model);

    request.subscribe({
      next: () => this.router.navigate(['/clients']),
      error: () => this.error.set("Impossible d'enregistrer le client."),
    });
  }
}
