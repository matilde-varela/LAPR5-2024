import { Injectable } from '@angular/core';
import { MessageService } from './message.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { Specialization } from '../domain/specialization';
import { StaffWithId } from '../domain/staff-with-id';
import { Staff } from '../domain/staff';
import { StaffQueryParameters } from '../domain/staff-query-parameters';
import { IdPasser } from '../domain/IdPasser';
import { EditStaffProfile } from '../domain/edit-staff';
import { StaffWithFunction } from '../domain/staff-with-function';
import { AddTimeSlotsComponent } from '../components/staff/add-time-slots/add-time-slots.component';
import { AddTimeSlot } from '../domain/add-time-slots';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class StaffService {
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

    return this.http.get<Specialization[]>(url, this.httpOptions)
                    .pipe(
                      map(data => data.map(spec => spec.denomination)),
                      catchError(this.handleError<string[]>('Get Specializations', []))
                    );
  }

  public createStaffProfile(firstName: string, lastName: string, phone: string, email: string, address: string, licenseNumber: string, specialization: string,functionDescrip:string) {
    const url = `${this.theServerURL}/Staff/Create-StaffProfile`;
    let staff: Staff = {
      licenseNumber: licenseNumber,
      firstName: firstName,
      lastName: lastName,
      phone: phone,
      email: email,
      address: address,
      function: functionDescrip,
      specializationId: specialization
    };

    this.http.post<Staff>(url, staff, this.httpOptions)
      .pipe(catchError(this.handleError<Staff>('Create staff profile')))
      .subscribe(data => {
        this.log(`Staff profile: ${data.email} was successfully created.`);
      });
  }


  EditStaffProfile(id: string, phone: string, email: string, address: string, specialization: string) {
    const url = `${this.theServerURL}/Staff/${id}`;
    let editStaff: EditStaffProfile = {
      id: id
    };

    if (phone && phone.trim() !== "") {
      editStaff.phone = phone;
    }

    if (email && email.trim() !== "") {
      editStaff.email = email;
    }

    if (address && address.trim() !== "") {
      editStaff.address = address;
    }

    if (specialization && specialization.trim() !== "") {
      editStaff.specializationId = specialization;
    }

    this.http.put<Staff>(url, editStaff, this.httpOptions)
      .pipe(catchError(this.handleError<EditStaffProfile>('Edited staff profile')))
      .subscribe(data => {
        this.log(`Staff profile: was successfully updated.`);
      });
  }

  confirmEmailStaff(userId: string, staffId: string, token: string): Observable<any> {
    const params = new HttpParams()
      .set('userId', userId)
      .set('staffId', staffId)
      .set('token', token);

    return this.http.put(`${this.theServerURL}/staff/activate-staffProfile`, null, { params });
  }


  deactivateStaffProfile(idStaff: string, token: string) {
    const url = `${this.theServerURL}/Staff/Deactivate-StaffProfile`;
    let idPasser: IdPasser = {
      id: idStaff
    };

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http.put<{ message : string }>(url, idPasser, { headers})
      .pipe(catchError(this.handleError<{ message : string }>('Deactivate staff profile')))
      .subscribe(data => {
        this.log(`${data.message}`);
      });
  }

  getStaffByFilters(staffQueryParameters: StaffQueryParameters): Observable<StaffWithId[]> {
    const url = `${this.theServerURL}/Staff/Filtered-List`;

    return this.http.post<StaffWithId[]>(url, staffQueryParameters, this.httpOptions).pipe(
        catchError((error) => {
          if (error.status = 404) {
            const queryParameters: StaffQueryParameters = {
              queryfilters: []
            };

            queryParameters.queryfilters.push({
              firstName: '',
              lastName: '',
              email: '',
              specialization: ''
            })

            this.log('No staff profiles were found with the chosen criteria.')
            return this.http.post<StaffWithId[]>(url, queryParameters, this.httpOptions)
          }else {
            this.handleError<StaffWithId[]>('Get staff profile filtered list', error);
            return of([]);
          }
        })
    );
  }


  getStaffById(id: string): Observable<StaffWithId> {
    const url = `${this.theServerURL}/Staff/${id}`;

    return this.http.get<StaffWithId>(url).pipe(
        catchError((error) => {
            this.handleError<StaffWithId>('Get staff profile', error);
            return of({} as StaffWithId);
        })
    );
  }

  getAllActiveStaffProfiles() : Observable<StaffWithFunction[]> {
    const url = `${this.theServerURL}/Staff/Get-ActiveStaffProfiles`;

    return this.http.get<StaffWithFunction[]>(url, this.httpOptions).pipe(
        catchError((error) => {
            this.handleError<StaffWithFunction[]>('Get staff profile', error);
            return [];
        })
    );
  }

  StaffProfileAddSlot(slot: string, date: string) {

    const url = `${this.theServerURL}/Staff/Add-TimeSlots`;

    let addTimeSlot: AddTimeSlot = {
      slot: slot,
      date: date
    };
    console.log(addTimeSlot);
    return this.http.post<AddTimeSlot>(url, addTimeSlot, this.httpOptions).pipe(
        catchError((error) => {
            this.handleError<{message: string}>('Add staff profile time slot', error);
            return [];
        })
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
