import { Injectable } from '@angular/core';
import { MessageService } from './message.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Specialization } from '../domain/specialization';

@Injectable({
  providedIn: 'root'
})
export class SpecializationService {
   theServerURL = environment.serverBaseUrl;
  token = localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).token : null;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
       Authorization: `Bearer ${this.token}`
    })
   };

  constructor(private messageService: MessageService, private http: HttpClient) { }

  public createSpecialization(code : string, denomination : string, description : string) {
    const url = `${this.theServerURL}/Specialization`;
    let specialization: Specialization = {
      code : code,
      denomination: denomination,
      description: description
    };
    this.http.post<Specialization>(url, specialization, this.httpOptions)
      .pipe(catchError(this.handleError<Specialization>('Create Specialization')))
      .subscribe(data => {
        this.log(`Specialization: ${data.denomination} was successfully created.`);
      });
  }

  
  public getAllSpecializationsAvailable(): Observable<Specialization[]> {
    const url = `${this.theServerURL}/Specialization/filtered`;

    return this.http.get<Specialization[]>(url, this.httpOptions)
                    .pipe(            
                      catchError(this.handleError<Specialization[]>('Get Specializations', []))
                    );
  }
  public getSpecializationsByFilters(code:string,denomination:string,description:string): Observable<Specialization[]> {
    const url = `${this.theServerURL}/Specialization/filtered`;
    let params = new HttpParams();

    if (code) {
      params = params.set('code', code);
    }
    if (denomination) {
      params = params.set('denomination', denomination);
    }
    if (description) {
      params = params.set('description', description);
    }
    return this.http.get<Specialization[]>(url, { headers: this.httpOptions.headers, params })
                    .pipe(
                      catchError(this.handleError<Specialization[]>('Get Specializations', []))
                    );
  }
  public getSpecializationById(code: string): Observable<Specialization> {
    const url = `${this.theServerURL}/Specialization/filtered?code=${code}`;
  
    return this.http.get<Specialization[]>(url, this.httpOptions).pipe(
      map(specializations => {
        if (specializations.length > 0) {
          return specializations[0];
        } else {
          throw new Error('No specialization found for the given code.');
        }
      }),
      catchError(this.handleError<Specialization>('Get Specialization'))
    );
  }

  public deleteSpecializationById(code: string): void {
    const url = `${this.theServerURL}/Specialization/${code}`;
  
   this.http.delete<{ message: string }>(url, this.httpOptions)
    .pipe().subscribe({
      next: () => {
        this.log(`Specialization: ${code} was successfully removed.`);
      },
      error: (err) => {
        this.log(`Failed to remove Specialization: ${code}. Error: ${err.message}`);
      }
    });
  }

  public updateSpecialization(code: string, denomination: string, description: string) {
    const url = `${this.theServerURL}/Specialization`;

    var update : Specialization = {
      code: code,
      denomination: denomination,
      description: description,
    };
    
    this.http.put<Specialization>(url, update, this.httpOptions)
      .pipe()
      .subscribe({
        next: (data) => {
          this.log(`Specialization was successfully updated.`);
        },
        error: (err) => {
          catchError(this.handleError<Specialization>('Update Specialization'))
        },
      });
  }
  //------------------------/------------------------/------------------------
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      if (error.status === 440) {
        this.log("Error: Login session expired.");
        return of(result as T);
      } else if (error.status === 401) {
        this.log("Error: Authentication is required.");
        return of(result as T);
      } else if (error.status === 403) {
        this.log("Error: Not allowed to access the feature.");
        return of(result as T);
      } else if(error.status === 200 ||error.status === 400 ){
        this.log(`${operation} failed: ${error.error}`);
      } else {
        this.log(`${operation} failed: an unexpected error occured.`);
      }
      return of(result as T);
    };
  }
  private log(message: string) {
    this.messageService.add(`${message}`);
  }
}
