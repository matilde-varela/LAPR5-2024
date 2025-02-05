import { Injectable } from '@angular/core';
import { MessageService } from './message.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Allergy } from '../domain/Allergy';
import { AllergyUpdate } from '../domain/edit-allergy';
import { AllergyQueryParameters } from '../domain/allergy-query-parameters';

@Injectable({
  providedIn: 'root'
})
export class AllergyService {
   theServerURL = environment.serverPatientManagementUrl;
  token = localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).token : null;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
       Authorization: `Bearer ${this.token}`
    })
   };

  constructor(private messageService: MessageService, private http: HttpClient) { }

  public getAllAllergies(): Observable<Allergy[]> {
    const url = `${this.theServerURL}/Allergy/get-all-allergies`;
  
    return this.http.get<Allergy[]>(url, this.httpOptions)
      .pipe(
        map((data: Allergy[]) => data.map(allergy => allergy)),
        catchError(this.handleError<Allergy[]>('Get Allergies', []))
      );
  }
  


  getAllergyByCode(code: string): Observable<Allergy> {
    const url = `${this.theServerURL}/Allergy/get-allergy-by-code`;
    const payload = { code };

    return this.http.post<Allergy>(url, payload, this.httpOptions)
      .pipe(
        catchError(this.handleError<Allergy>('Get allergy'))
      );
  }

  getAllergiesByFilters(allergiesQueryParameters: AllergyQueryParameters): Observable<Allergy[]> {
      const url = `${this.theServerURL}/Allergy/get-allergies-filtered`;

      return this.http.post<Allergy[]>(url, allergiesQueryParameters, this.httpOptions).pipe(
          catchError((error) => {
            if (error.status = 404) {

              const queryParameters: AllergyQueryParameters = {
                queryfilters: []
              };

              queryParameters.queryfilters.push(
                {
                  code : '',
                  designation : '',
                  description : ''
                }
              )

              this.log('No allergies were found with the chosen criteria.')
              return this.http.post<Allergy[]>(url, queryParameters, this.httpOptions)
            }else {
              this.handleError<Allergy[]>('Get allergies filtered list', error);
              return of([]);
            }
          })
      );
    }


  public createAllergy(code : string, designation : string, description : string) {
    const url = `${this.theServerURL}/Allergy/create-allergy`;
    let allergy: Allergy = {
      code : code,
      designation: designation,
      description: description
    };

    this.http.post<Allergy>(url, allergy, this.httpOptions)
      .pipe(catchError(this.handleError<Allergy>('Create allergy')))
      .subscribe(data => {
        this.log(`Allergy: ${data.designation} was successfully created.`);
      });
  }

  EditAllergy(code: string, designation: string, description: string) {
    const url = `${this.theServerURL}/Allergy/update-allergy`;
    let EditAllergy: AllergyUpdate = {
      code: code,
      designation: designation,
      description: description,
    };

    if (description && description.trim() !== "") {
      EditAllergy.description = description;
    }

    this.http.patch<Allergy>(url, EditAllergy, this.httpOptions)
      .pipe(catchError(this.handleError<Allergy>('Create allergy')))
      .subscribe({
        next: (data) => {
          this.log(`Allergy was successfully updated.`);
        },
        error: (err) => {
          console.error('Error in subscription:', err);
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
