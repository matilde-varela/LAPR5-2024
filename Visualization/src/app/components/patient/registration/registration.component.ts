import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';  // Importing NgForm only
import { AuthService } from '../../../shared/services/auth.service';
import { MessageComponent } from '../../message/message.component';


@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [MessageComponent, FormsModule, CommonModule], // Only keep CommonModule and MessageComponent
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent {
  @ViewChild('userPatientForm') userPatientForm!: NgForm;  // Using template-driven form
  
  
  isSubmitted = false;
  userPatient = {
    email: '',
    password: '',
    phone: ''
  };

  constructor(private service: AuthService) { }

  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    if (form.valid) {
      this.service.CreateUserPatient(
        this.userPatient.email,
        this.userPatient.password,
        this.userPatient.phone
      );
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.isSubmitted = false;
    this.userPatient = { email: '', password: '', phone: '' };
    
    if (this.userPatientForm) {
      this.userPatientForm.resetForm();
    }

    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }
}
