import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NewPass } from 'src/app/domain/NewPass';
import { UserService } from 'src/app/services/user.service';


@Component({
  selector: 'app-update-password',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './update-password.component.html',
  styleUrl: './update-password.component.css'
})
export class UpdatePasswordComponent {
  @ViewChild('newPasswordForm') newPasswordForm!: NgForm;
  message = 'Processing password reset...';

  isSubmitted = false;
  form = {
    newPassword: ''
  };
  
  constructor(private route: ActivatedRoute, private service: UserService, private http: HttpClient) { }

  onSubmit(form: NgForm) : void {
    if (form.valid) {
      this.isSubmitted = true;
      const email = this.route.snapshot.queryParamMap.get('email');
      const token = this.route.snapshot.queryParamMap.get('token');

      if (email && token) {
        let newPass: NewPass = {
          newPassword: this.form.newPassword
        };

        this.service.updatePassword(email, token, newPass.newPassword).subscribe({
          next: (response) => {
            this.message = `${response.message}`;
          },
          error: (err) => {
            this.message = 'Error changing the password. Please try again.';
            console.error(err);
          },
        });

      } else {
        this.message = 'Invalid link.';
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