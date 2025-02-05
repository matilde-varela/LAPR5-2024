import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { MessageService } from './message.service';
import { catchError, Observable, of } from 'rxjs';
import { DownloadMedicalRecord } from '../domain/download-medical-record';

@Injectable({
  providedIn: 'root'
})
export class DownloadService {
theServerURL = environment.serverBaseUrl;
    token = localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).token : null;
    httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
       Authorization: `Bearer ${this.token}`
    })
  };

  constructor(private messageService: MessageService, private http: HttpClient) { }

  public downloadMedicalRecord(filePath : string, password : string) {
      const url = `${this.theServerURL}/Patient/Download-Medical-Record`;

      let info: DownloadMedicalRecord = {
        filePath : filePath,
        password: password
      };

      this.http.post<string>(url, info, this.httpOptions)
        .pipe(catchError(this.handleError<string>('Download medical record')))
        .subscribe(data => {
          this.log(`Medical Record was successfully downloaded.`);
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
