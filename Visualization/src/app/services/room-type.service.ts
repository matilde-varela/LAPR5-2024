import { Injectable } from '@angular/core';
import { MessageService } from './message.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { RoomType } from '../domain/room-type';

@Injectable({
  providedIn: 'root'
})
export class RoomTypeService {
   theServerURL = environment.serverBaseUrl;
  token = localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).token : null;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
       Authorization: `Bearer ${this.token}`
    })
   };

  constructor(private messageService: MessageService, private http: HttpClient) { }

  public createRoomType(code : string, designation : string, description : string) {
    const url = `${this.theServerURL}/RoomType`;
    let roomtype: RoomType = {
      code : code,
      designation: designation,
      description: description
    };
    this.http.post<RoomType>(url, roomtype, this.httpOptions)
      .pipe(catchError(this.handleError<RoomType>('Create Room Type')))
      .subscribe(data => {
        this.log(`Room Type: ${data.designation} was successfully created.`);
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
