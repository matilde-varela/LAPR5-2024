import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { UserPatient } from '../../domain/UserPatient';
import { MessageService } from '../../services/message.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  token = localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).token : null;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.token}`
    })
   };

    constructor(private http: HttpClient,
                private messageService: MessageService,
                private router: Router) {
    }

    private handleError<T>(operation = 'operation', result?: T) {
      return (error: any): Observable<T> => {
        console.error(error); // log to console instead of just alerting
        this.log(`${operation} failed: ${error.message}`);

        // Handle different error status codes
        if (error.status === 440) {
          this.log("Session expired.");
        } else if (error.status === 401) {
          this.log("Not authenticated.");
        } else if (error.status === 403) {
          this.log("You do not have permission to access this content.");
        }

        return of(result as T);
      };
    }

  CreateUserPatient(email: string, password: string, phoneNumber: string) {
    const url = environment.serverBaseUrl + '/create-patient';
    let userPatient: UserPatient = {
      email: email,
      password: password,
      phone: phoneNumber,
    };

    this.http.post<{message: string}>(url, userPatient, this.httpOptions)
      .pipe(catchError(this.handleError<{message: string}>('Create user patient')) // Handle errors gracefully
      )
      .subscribe(data => {
          this.log(data.message); // Log the message from the server
      });
  }


  private log(message: string) {
    this.messageService.add(`${message}`);
  }


}
