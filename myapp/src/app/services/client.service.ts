import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ClientService {

  private apiUrl = 'http://localhost:5115/api/client';

  constructor(private http: HttpClient) { }

  getClients(filtre?: { mois: string; annee: string }) {
    return this.http.get(this.apiUrl, { params: filtre });
  }

  getFormesJuridiques() {
    return this.http.get<{ value: string; label: string }[]>(`${this.apiUrl}/formes-juridiques`);
  }

  getPeriodicitesTva() {
    return this.http.get<{ value: string; label: string }[]>(`${this.apiUrl}/periodicites-tva`);
  }

  addClient(client: object) {
    return this.http.post(this.apiUrl, client);
  }

  updateClient(id: number, client: object) {
    return this.http.put(`${this.apiUrl}/${id}`, client);
  }
  getClient(id: number) {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }
  deleteClient(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  setTvaValidation(id: number, annee: number, mois: number, validee: boolean) {
    return this.http.put(`${this.apiUrl}/${id}/tva-validation`, { annee, mois, validee });
  }
}
