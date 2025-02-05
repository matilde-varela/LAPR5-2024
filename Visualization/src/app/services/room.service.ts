import { Injectable } from '@angular/core';
import { MessageService } from './message.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, map, Observable, of, tap, throwError } from 'rxjs';
import { Room } from '../domain/room';
import { RoomSchedule } from '../domain/room-schedule';
import { environment } from 'src/environments/environment';
import { RoomType } from '../domain/room-type';
import { CreateRoom } from '../domain/create-room';


@Injectable({
  providedIn: 'root'
})

export class RoomService {

  theServerURL = environment.serverBaseUrl;
  httpOptions = {
    headers: new HttpHeaders({'Content-Type': 'application/json'})
  };
  
  constructor(private messageService: MessageService, private http: HttpClient) { }

  getAllRooms() : Observable<Room[]>{
    const url = `${this.theServerURL}/Room/Get-All`;

    return this.http.get<Room[]>(url).pipe(
        catchError((error) => {
            this.handleError<Room[]>('Get rooms', error);
            return [];
        })
    );
  }

  getRoomsSchedule() {
    const url = `${this.theServerURL}/Room/Get-AllSchedules`;

    return this.http.get<RoomSchedule[]>(url).pipe(
        catchError((error) => {
            this.handleError<RoomSchedule[]>('Get rooms schedule', error);
            return [];
        })
    );
  }

  getAllRoomTypesAvailable() : Observable<string[]> {
      const url = `${this.theServerURL}/RoomType/get-all`;
  
      return this.http.get<RoomType[]>(url, this.httpOptions)
                      .pipe(
                        map(data => data.map(roomtyp => roomtyp.designation)),
                        catchError(this.handleError<string[]>('Get Room Types', []))
                      );
  }

  createRoom(roomNumber: string, typeDesignation: string, capacity: string, availableEquipment: string, maintenanceSlots: string) {
    const url = `${this.theServerURL}/Room/create`;
  
    const availableEquipmentCleaned = availableEquipment.replace(/\s+/g, '').trim();
    const maintenanceSlotsCleaned = maintenanceSlots.replace(/\s+/g, '').trim();

    const body: CreateRoom = {
      roomNumber,
      typeDesignation,
      capacity,
      availableEquipment: availableEquipmentCleaned.split(","),
      maintenanceSlots: maintenanceSlotsCleaned.split(",")
    };

    this.http.post<CreateRoom>(url, body, this.httpOptions)
          .pipe(catchError(this.handleError<CreateRoom>('Create room')))
          .subscribe(data => {
            this.log(`${data}`);
          });
  }
  

  //------------------------/------------------------/------------------------
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(`${operation} failed: ${error.message}`);
  
      // Customize error handling based on status
      if (error.status === 440) {
        this.log("Error: Login session expired.");
      } else if (error.status === 401) {
        this.log("Error: Authentication is required.");
      } else if (error.status === 403) {
        this.log("Error: Not allowed to access the feature.");
      } else if (error.status === 400) {
        this.log(`Bad request: ${error.error.message}`);
      } else {
        this.log(`${operation} failed: an unexpected error occurred.`);
      }
  
      return of(result as T);
    };
  }
  

  private log(message: string) {
    this.messageService.add(`${message}`);
  }
}
