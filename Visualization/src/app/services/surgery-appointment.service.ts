import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MessageService } from './message.service';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SurgeryAppointment } from '../domain/SurgeryAppointment';
import { UpdateAppointment } from '../domain/update-appointment';
import { FullAppointment } from '../domain/full-appointment';
import { StaffWithFunction } from '../domain/staff-with-function';
import { AppointmentWithoutStaff } from '../domain/appointment-without-staff';
import { RoomInfo } from '../domain/room-info';

@Injectable({
  providedIn: 'root'
})

export class SurgeryAppointmentService {

  theServerURL = environment.serverBaseUrl;
  token = localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).token : null;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`
    })
  };

  constructor(private messageService: MessageService, private http: HttpClient) { }

  public createSurgeryAppointment(operationRequestId: string, roomNumber: string, startTime: string, endTime: string, startDate: string, endDate: string, staffList: string[]) {
    const url = `${this.theServerURL}/Appointment`;
    const surgAppointment: SurgeryAppointment = {
      operationRequestId: operationRequestId,
      roomNumber: roomNumber,
      startTime: startTime,
      endTime: endTime,
      startDate: startDate,
      endDate: endDate,
      staffList: staffList
    };

    this.http.post<SurgeryAppointment>(url, surgAppointment, this.httpOptions)
        .pipe(
            catchError((error: any) => {
                this.log(`Create surgery appointment failed: ${error.error?.message || 'An unexpected error occurred.'}`);
                return of(null);
            })
        )
        .subscribe(data => {
            if (data) {
                this.log('Surgery Appointment was successfully created.');
            }
        });
  }

  public getAllStaffs() {
    const url = `${this.theServerURL}/Staff/Get-StaffProfiles`;

    return this.http.get<StaffWithFunction[]>(url, this.httpOptions)
      .pipe(catchError(this.handleError<StaffWithFunction[]>('Get Rooms'))
      );
  }

  public editAppointment(updateAppointment: UpdateAppointment) {
    const url = `${this.theServerURL}/Appointment/update-appointment`;

    console.log(updateAppointment);
    console.log("It reached here.");

    this.http.patch<UpdateAppointment>(url, updateAppointment, this.httpOptions)
      .pipe(catchError(this.handleError<UpdateAppointment>('Edit surgery appointment')))
      .subscribe(data => {
        this.log(`Surgery Appointment was successfully Edited.`);
      });
  }

  public getAppointmentById(appointmentId: string) {
    const url = `${this.theServerURL}/Appointment/get-by-id`;

    return this.http.get<FullAppointment>(url, this.httpOptions)
      .pipe(catchError(this.handleError<FullAppointment>('Get Appointment'))
      );
  }

  public getRoomNumbers() {
    const url = `${this.theServerURL}/Room/Get-All-RoomNumbers`;

    return this.http.get<string[]>(url, this.httpOptions)
      .pipe(catchError(this.handleError<string[]>('Get Rooms'))
      );
  }

  public getAllAppointments() {
    const url = `${this.theServerURL}/Appointment/get-all`;

    return this.http.get<FullAppointment[]>(url, this.httpOptions)
      .pipe(catchError(this.handleError<FullAppointment[]>('Get Appointments', []))
      );
  }

  public getAppointmentFromRoom(roomNumber: string, startTime: string, endTime: string, startDate: string, endDate: string){
    const url = `${this.theServerURL}/Appointment/get-by-roomInfo`;
    const roomInfo: RoomInfo = {
      roomNumber: roomNumber,
      startTime: startTime,
      endTime: endTime,
      startDate: this.formatDate(startDate), 
    endDate: this.formatDate(endDate)
    };

    return this.http.post<AppointmentWithoutStaff>(url, roomInfo, this.httpOptions)
    .pipe(
      catchError(error => {
        console.log("Não é um agendamento válido");
        return of(null);
      })
    );
  }

  private formatDate(date: string): string {
    const [day, month, year] = date.split('/').map(Number);
    return `${year}-${String(month).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
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
