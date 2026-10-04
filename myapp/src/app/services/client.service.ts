import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ClientService {

  private apiUrl = 'http://localhost:5115/api/client';

  constructor(private http: HttpClient) {}

  getClients() {
    return this.http.get(this.apiUrl);
  }
}
