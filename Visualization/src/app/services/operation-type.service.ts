import { Injectable } from '@angular/core';
import { MessageService } from './message.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, map, Observable, of, switchMap } from 'rxjs';
import { Specialization } from '../domain/specialization';
import { OperationType } from '../domain/OperationType';
import { ListOperationType } from '../domain/list-operation-type';
import { OperationTypeEditEntity } from '../domain/OperationTypeEdit';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OperationTypeService {
  // Proceed with the PUT request to edit the operation type

  theServerURL = environment.serverBaseUrl;
  token = localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).token : null;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
       Authorization: `Bearer ${this.token}`
    })
  };

  constructor(private messageService: MessageService, private http: HttpClient) { }

  public getAllSpecializationsAvailable(): Observable<string[]> {
    const url = `${this.theServerURL}/Specialization/filtered`;

    console.log(url)

    return this.http.get<Specialization[]>(url, this.httpOptions)
      .pipe(
        map(data => data.map(spec => spec.denomination)),
        catchError(this.handleError<string[]>('Get Specializations', []))
      );
  }

  createOperationType(operationType: OperationType) {
    const url = `${this.theServerURL}/OperationTypes`;

    console.log('Payload sent to backend:', JSON.stringify(operationType, null, 2));

    console.log(url)

    this.http.post<OperationType>(url, operationType, this.httpOptions)
      .pipe(catchError(this.handleError<OperationType>('Operation type creation')))
      .subscribe(data => {
        this.log(`Operation type: ${data.name} was successfully created.`);
      });
  }

  //------------------------/------------------------/------------------------
  public getOperationTypeById(name: string): Observable<OperationType> {
    const url = `${this.theServerURL}/OperationTypes/Get-WithName/${name}`;

    return this.http.get<OperationType>(url, this.httpOptions)
      .pipe(
        catchError(this.handleError<OperationType>('Get Operation Type'))
      );

  }

  editOperationType(operationTypeId: string, editOpType: ListOperationType) {
    // Construct the initial entity with provided data
    console.log("It reached here : Start editMethod at service.");
    var operationType = new OperationTypeEditEntity(
      operationTypeId,
      editOpType.name,
      editOpType.estimatedDuration,
      editOpType.status,
      editOpType.requiredStaff,
      editOpType.phases
    );

    const editUrl = `${this.theServerURL}/OperationTypes/Edit-OperationType`;
  
    // Proceed with the PUT request to edit the operation type
    return this.http.put<any>(editUrl, operationType, this.httpOptions).pipe(
      catchError((error) => {
        this.handleError<any>('Edit operation type', error);
        return of(null); // Return null if the PUT request fails
      }));
    }
  
  
  getOperationTypeByName(name: string) {
    const getUrl = `${this.theServerURL}/OperationTypes/Get-WithName/${name}`;

    return this.http.get<any>(getUrl, this.httpOptions).pipe(
      catchError((error) => {
        this.handleError<any>('Get operation type by name', error);
        return of(null);
    })) 
  }
  

  getOperationTypesByFilters(filters: any): Observable<any> {
    const url = `${this.theServerURL}/OperationTypes/Filtered-List`
    // return this.http.post<any>(url, filters);
    return this.http.post<ListOperationType[]>(url, filters, this.httpOptions).pipe(
      catchError((error) => {
          this.handleError<OperationType>('Get operation types list', error);
          return of([]);
      })
    );
  }


  public removeOperationType(name: string) {
    const url = `${this.theServerURL}/OperationTypes/${name}`;
    return this.http.delete<OperationType>(url, this.httpOptions)
      .pipe(
        catchError(this.handleError<OperationType>('Remove Operation Type'))
      ).subscribe({
        next: () => {
          this.log(`Operation type: ${name} was successfully removed.`);
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
      } else if (error.status === 200 || error.status === 400) {
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
