import { Component, ViewChild } from '@angular/core';
import { MessageComponent } from '../message/message.component';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [MessageComponent, FormsModule, CommonModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent {
  @ViewChild('emailForm') emailForm!: NgForm;

  isSubmitted = false;
  resetPass = {
    email: ''
  };

  constructor(private service: UserService) { }

  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    if (form.valid) {
      this.service.resetPassword(
        this.resetPass.email
      );
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.isSubmitted = false;

    this.resetPass = {
      email: ''
    };
    if (this.emailForm) {
      this.emailForm.resetForm();
    }
    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }
}
