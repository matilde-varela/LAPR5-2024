import { Injectable } from '@angular/core';
import { MessageService } from './message.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { catchError, Observable, of, tap } from 'rxjs';
import { Router } from '@angular/router';
import { UserInfo } from '../domain/user-info';
import { NewPass } from '../domain/NewPass';
import { environment } from 'src/environments/environment';

interface UserLogin {
  email: string;
  password: string;
}

interface LoginResponse {
  token: string;
}

interface DecodeResponse {
  email: string;
  roles: string[];
}

interface UserStaff {
  email: string;
  password: string;
  phone: string;
  role: string;
}

interface CreateStaffUserResponse {
  message: string;
}

var token: string;

interface ServerResponse {
  message: string;
}

@Injectable({
  providedIn: 'root'
})

export class UserService {
  theServerURL = environment.serverBaseUrl;
  token = localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).token : null;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
       Authorization: `Bearer ${this.token}`
    })
   };
  constructor(private messageService: MessageService, private http: HttpClient, private router: Router) { }

  public loginExternal() {
    const url = `${this.theServerURL}/login-external`;
    window.location.href = url;
  }

  public login(userEmail: string, userPassword: string) {
    const url = `${this.theServerURL}/login-internal`;
    let user: UserLogin = {
      email: userEmail,
      password: userPassword
    };

    this.http.post<LoginResponse>(url, user, this.httpOptions)
      .pipe(catchError(this.handleError<LoginResponse>('User login')))
      .subscribe(data => {
        if (data && data.token) {
          this.decodeTokenandRedirect(data.token);
        } else {
          this.log(`Login failed.`);
        }
      });
  }
  public decodeTokenandRedirect(token: string) {
    const url = `${this.theServerURL}/decode-token?token=${encodeURIComponent(token)}`;

    this.http.get<DecodeResponse>(url, this.httpOptions)
      .pipe(catchError(this.handleError<DecodeResponse>('Role identification')))
      .subscribe(data => {
        if (data && data.roles && data.roles.length > 0) {
          const roles = data.roles;

          const userInfo: UserInfo = {
            email: data.email,
            roles: data.roles,
            token: token
          }

          localStorage.setItem('user', JSON.stringify(userInfo));

          if (roles.includes('Admin')) {
            this.router.navigate(['/admin']);
          } else if (roles.includes('Doctor')) {
            this.router.navigate(['/doctor']);
          } else if (roles.includes('Nurse')) {
            this.router.navigate(['/staff']);
          } else if (roles.includes('Patient')) {
            this.router.navigate(['/patient']);
          }

        } else {
          this.log('No roles found to be associated with the user.');
        }
      });
  }


  public createStaffUser(userEmail: string, userPassword: string, userPhone: string, userRole: string) {
    const url = `${this.theServerURL}/create-staff`;
    let user: UserStaff = {
      email: userEmail,
      password: userPassword,
      phone: userPhone,
      role: userRole
    };
    this.http.post<CreateStaffUserResponse>(url, user, this.httpOptions)
      .pipe(catchError(this.handleError<CreateStaffUserResponse>('Create staff user')))
      .subscribe(data => {
        if (data.message) {
          this.log(data.message);
        } else {
          this.log(`Creation of user failed.`);
        }
      });
  }

  public sendAccountDeleteRequest() {
    const url = `${this.theServerURL}/Delete-PatientAccountDeletionRequest`;

    this.http.delete<{ message: string }>(url, this.httpOptions)
      .subscribe({
        next: (response) => {
          this.log(`${response.message}`);
        },
        error: (err) => {
          catchError(this.handleError('Send patient account deletion email'))
        }
      }

      );
  }

  resetPassword(email: string) {
    const url = `${this.theServerURL}/send-passwordemail`;

    const params = new HttpParams()
      .set('email', email);

    this.http.put<any>(url,  null, { params })
      .pipe(catchError(this.handleError<any>('Reset Password')))
      .subscribe(data => {
        this.log('Password reset email was sent!')
      })
  }

  updatePassword(email: string, token: string, newpassword: string) : Observable<any> {
    const url = `${this.theServerURL}/Update-UserPassword`;

      const params = new HttpParams()
      .set('email', email)
      .set('token', token);

      const body = { NewPassword: newpassword };

    return this.http.put(url, body, { params });
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


  private log(message: string, p0?: { data: UserLogin; }) {
    this.messageService.add(`${message}`);
  }
}
