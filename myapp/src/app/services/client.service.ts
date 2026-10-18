import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ClientService {

  private apiUrl = 'http://localhost:5115/api/client';

  constructor(private http: HttpClient) { }

  getClients() {
    return this.http.get(this.apiUrl);
  }

  getFormesJuridiques() {
    return this.http.get<{ value: string; label: string }[]>(`${this.apiUrl}/formes-juridiques`);
  }

  addClient(client: Record<string, string>) {
    return this.http.post(this.apiUrl, client);
  }

  getClient(id: number) {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  updateClient(id: number, client: Record<string, string>) {
    return this.http.put(`${this.apiUrl}/${id}`, client);
  }
}
