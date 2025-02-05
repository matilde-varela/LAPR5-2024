import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { PatientService } from '../../../services/patient.service';
import { MessageComponent } from '../../message/message.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SideBarPatientComponent } from '../sidebar-patient/side-bar-patient.component';

@Component({
  selector: 'app-update-profile',
  standalone: true,
  imports: [SideBarPatientComponent, MessageComponent, FormsModule, CommonModule],
  templateUrl: './update-profile.component.html',
  styleUrls: ['./update-profile.component.css']
})
export class UpdateProfileComponent {
  @ViewChild('patientForm') patientForm!: NgForm;

  isSubmitted = false;
  patient = {
    name: '',
    phone: '',
    emergencyContact: '',
    email: '',
    address: ''
  };

  constructor(private service: PatientService) { }

  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    // Submit the data even if some fields are empty
    if (form.valid) {
      this.service.updateProfile(
        this.patient.name || '',
        this.patient.phone || '',
        this.patient.email || '',
        this.patient.address || '',
        this.patient.emergencyContact || ''
      );
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.isSubmitted = false;

    this.patient = {
      name: '',
      phone: '',
      emergencyContact: '',
      email: '',
      address: ''
    };
    if (this.patientForm) {
      this.patientForm.resetForm();
    }
  }
}
