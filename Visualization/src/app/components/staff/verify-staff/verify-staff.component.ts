import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { MessageComponent } from '../../message/message.component';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { param } from 'jquery';
import { NewPass } from '../../../domain/NewPass';
import { environment } from 'src/environments/environment';

interface ServerResponse {
  message: string;
}

@Component({
  selector: 'app-verify-staff',
  standalone: true,
  imports: [MessageComponent, FormsModule, CommonModule],
  templateUrl: './verify-staff.component.html',
  styleUrl: './verify-staff.component.css'
})
export class VerifyStaffComponent {

  @ViewChild('newPasswordForm') newPasswordForm!: NgForm;
  message = '';

  isSubmitted = false;
  form = {
      newPassword: ''
    };

  constructor(private route: ActivatedRoute, private http: HttpClient) {}

  onSubmit(form: NgForm): void {
    if (form.valid) {

      this.isSubmitted = true;
      const userId = this.route.snapshot.queryParamMap.get('userId');
      const token = this.route.snapshot.queryParamMap.get('token');

      if (userId && token) {
        const url = environment.serverBaseUrl + '/activate-staff?userId='+encodeURIComponent(userId)+'&token='+encodeURIComponent(token);

        let newPass: NewPass = {
          newPassword : this.form.newPassword
        }; 

        this.http.put<ServerResponse>(url, newPass).subscribe({
          next: (response) => {
            this.message = `${response.message}`;
          },
          error: (err) => {
            this.message = 'Account activation failed. Please try again.';
            console.error(err);
          },
        });
        

      } else {
        this.message = 'Invalid activation link.';
      }
    } else {
      this.isSubmitted = false;
    }

  }

  clearForm(): void {

    this.form = {
      newPassword: ''
    };
    this.isSubmitted = false;

    if (this.newPasswordForm) {
      this.newPasswordForm.resetForm();
    }

    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }
}



