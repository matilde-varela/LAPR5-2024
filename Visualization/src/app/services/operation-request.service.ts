import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MessageService } from './message.service';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { OperationType } from '../domain/OperationType';
import { OperationRequest } from '../domain/OperationRequest';
import { ListOperationRequest } from '../domain/list-operation-request';
import { AddOrRemoveFromPatient } from '../domain/add-or-remove-from-patient';
import { StaffWithFunction } from '../domain/staff-with-function';
import { SchedulingBackend } from '../domain/scheduling-backend';
import { environment } from 'src/environments/environment';

interface UpdateOperationRequest {
  id: string,
  priority: string,
  description: string,
  deadlineDate: string
}


@Injectable({
  providedIn: 'root'
})
export class OperationRequestService {
  theServerURL = environment.serverBaseUrl;
  token = localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).token : null;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`
    })
  };

  constructor(private messageService: MessageService, private http: HttpClient) { }

  public getAllOperationTypes(): Observable<string[]> {
    const url = `${this.theServerURL}/OperationTypes`;

    return this.http.get<OperationType[]>(url, this.httpOptions)
      .pipe(
        map(data => data.map(opType => opType.name)),
        catchError(this.handleError<string[]>('Get Operation Types', []))
      );
  }

  public getOperationRequestById(id: string): Observable<OperationRequest> {
    const url = `${this.theServerURL}/OperationRequest/${id}`;

    return this.http.get<OperationRequest>(url, this.httpOptions)
      .pipe(
        catchError(this.handleError<OperationRequest>('Get Operation Request'))
      );
  }

  public updateOperationRequest(updatedInfo: UpdateOperationRequest) {
    const url = `${this.theServerURL}/OperationRequest/Update`;

    this.http.put<OperationRequest>(url, updatedInfo, this.httpOptions)
      .pipe(catchError(this.handleError<OperationRequest>('Update Operation Request')))
      .subscribe(data => {
        console.log(data);
        this.log(`Operation Request: ${data.id} was successfully edited.`);
      });

  }

  public getOperationRequestsByFilters(name: string, priority: string, operationType: string, status: string, dateOfRequest: string, deadlineDate: string): Observable<ListOperationRequest[]> {
    let params = new HttpParams();

    // Conditionally add params if they have values
    if (name) {
      params = params.set('name', name);
    }
    if (priority) {
      params = params.set('priority', priority);
    }
    if (operationType) {
      params = params.set('operationType', operationType);
    }
    if (status) {
      params = params.set('status', status);
    }
    if (dateOfRequest) {
      params = params.set('dateofrequest', dateOfRequest);
    }
    if (deadlineDate) {
      params = params.set('deadlinedate', deadlineDate);
    }
    const url = `${this.theServerURL}/OperationRequest/filtered`;
    return this.http.get<ListOperationRequest[]>(url, { headers: this.httpOptions.headers, params })
      .pipe(
        catchError((error) => {
          catchError(this.handleError<OperationRequest>('Create Operation Request'));
          return of([]);
        })
      );
  }

  public createOperationRequest(deadLineDate: string, priority: string, dateOfRequest: string, status: string, staffId: string, description: string, patientId: string, operationTypeId: string): Observable<string> {
    const url = `${this.theServerURL}/OperationRequest`;
    const opRequest: OperationRequest = {
      deadLineDate: deadLineDate,
      priority: priority,
      dateOfRequest: dateOfRequest,
      status: status,
      staffId: staffId,
      description: description,
      patientId: patientId,
      operationTypeId: operationTypeId
    };

    return this.http.post<OperationRequest>(url, opRequest, this.httpOptions).pipe(
      map((response: OperationRequest) => {
        if (!response.id) {
          throw new Error('Operation Request ID is undefined');
        }
        this.log(`Operation Request successfully created`);
        return response.id;
      }),
      catchError((error) => {
        this.log(`Failed to create Operation Request!`);
        return throwError(error);
      })
    );
  }


  public addOperationRequestToPatient(patientId: string, operationRequestId: string) {
    const url = `${this.theServerURL}/OperationRequest/Add-OperationRequestToPatient`;

    let opReqHis: AddOrRemoveFromPatient = {
      patientId: patientId,
      operationRequestId: operationRequestId
    }

    this.http.post<AddOrRemoveFromPatient>(url, opReqHis, this.httpOptions)
      .pipe(catchError(this.handleError<AddOrRemoveFromPatient>('Add Operation Request to Patient History')))
      .subscribe(data => {
        this.log(`Operation request successfully add to patient history.`);
      });
  }

  public removeOperationRequestToPatient(patientId: string, operationRequestId: string) {
    const url = `${this.theServerURL}/OperationRequest/Delete-OperationRequestFromPatient`;

    let opReqHis: AddOrRemoveFromPatient = {
      patientId: patientId,
      operationRequestId: operationRequestId
    }

    this.http.put<AddOrRemoveFromPatient>(url, opReqHis, this.httpOptions)
      .pipe(catchError(this.handleError<AddOrRemoveFromPatient>('Add Operation Request to Patient History')))
      .subscribe(data => {
        this.log(`Operation request successfully add to patient history.`);
      });
  }


  getAllOperationRequests(): Observable<OperationRequest[]> {
    const url = `${this.theServerURL}/OperationRequest/Get-AllOpRequests`;

    return this.http.get<OperationRequest[]>(url, this.httpOptions)
      .pipe(
        catchError((error) => {
          this.handleError<OperationRequest[]>('Get all operation requests', error);
          return [];
        })
      );
  }


  public scheduleOperationRequest(scheduleBackend: SchedulingBackend) {
    const url = `${this.theServerURL}/OperationRequest/Schedule`;
    console.log('in service');
    // Specify the response type as string
    this.http.post<{ message: string }>(url, scheduleBackend, this.httpOptions).subscribe({
      next: (message) => {
        this.log(`Operation request success message: ${message.message}`); // Log the success message
      },
      error: (err) => {
        console.log(err);
        this.handleError('Error in schedule operation request:', err);
      }
    });
  }


  deleteOperationRequestById(id: string) {
    const url = `${this.theServerURL}/OperationRequest/${id}`;

    return this.http.delete<{ message: string }>(url, this.httpOptions)
      .pipe(
        catchError(this.handleError<{ message: string }>('Remove Operation Type'))
      ).subscribe({
        next: () => {
          this.log(`Operation request: ${id} was successfully removed.`);
        },
        error: (err) => {
          this.log(`Failed to remove operation request: ${id}. Error: ${err.message}`);
        }
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
