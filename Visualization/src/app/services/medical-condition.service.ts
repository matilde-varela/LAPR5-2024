import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { MessageService } from './message.service';
import { MedicalCondition } from '../domain/MedicalCondition';
import { MedicalConditionSent } from '../domain/MedicalConditionSent';
import { catchError, map, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MedicalConditionService {
  theServerURL = environment.serverPatientManagementUrl;
    token = localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).token : null;
    httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
       Authorization: `Bearer ${this.token}`
    })
  };

  constructor(private messageService: MessageService, private http: HttpClient) { }

  public createMedicalCondition(id : string, designation : string, description : string, symptoms : string) {
      const url = `${this.theServerURL}/medicalCondition/create`;

      const noSpaces = symptoms.replace(/\s+/g, '');

      const resultArray = noSpaces.split(',');

      let medicalCondition: MedicalConditionSent = {
        id : id,
        designation: designation,
        description: description,
        symptoms: resultArray
      };

      this.http.post<MedicalConditionSent>(url, medicalCondition, this.httpOptions)
        .pipe(catchError(this.handleError<MedicalConditionSent>('Create medical condition')))
        .subscribe(data => {
          this.log(`Medical Condition was successfully created.`);
        });
    }


    getMedicalConditionById(id: string): Observable<MedicalCondition> {
      const url = `${this.theServerURL}/medicalCondition/get-medical-condition-by-id`;
      const payload = { id };
    
      return this.http.post<MedicalCondition>(url, payload, this.httpOptions)
        .pipe(
          catchError(this.handleError<MedicalCondition>('Get Medical Condition'))
        );
    }
    


  public getAllMedicalConditions(): Observable<MedicalCondition[]> {
    const url = `${this.theServerURL}/medicalCondition/get-all-medical-conditions`;

    return this.http.get<MedicalCondition[]>(url, this.httpOptions)
      .pipe(
        map((data: MedicalCondition[]) => data.map(condition => condition), ),
        catchError(this.handleError<MedicalCondition[]>('Get Medical Condition', []))
      );
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
