import { Component, ViewChild, AfterViewInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { PatientService } from '../../../services/patient.service';
import { MessageComponent } from '../../message/message.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-patient-profile',
  standalone: true,
  imports: [SideBarAdminComponent, MessageComponent, FormsModule, CommonModule],
  templateUrl: './edit-patient-profile.component.html',
  styleUrls: ['./edit-patient-profile.component.css'] // Fixed typo
})
export class EditPatientProfileComponent implements AfterViewInit {
  @ViewChild('patientForm') patientForm!: NgForm;

  isSubmitted = false;
  patient = {
    id: '',
    name: '',
    phone: '',
    email: '',
    address: '',
    dateBirth: ''
  };

  constructor(private service: PatientService) {}

  ngAfterViewInit(): void {
    // Ensure patientForm is available
    console.log('patientForm initialized:', this.patientForm);
  }

  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    if (form.valid) {
      this.service
        .editPatientProfile(
          this.patient.id,
          this.patient.name || '',
          this.patient.phone || '',
          this.patient.email || '',
          this.patient.address || '',
          this.patient.dateBirth || ''
        );
    } else {
      console.warn('Form is invalid');
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.isSubmitted = false;

    this.patient = {
      id: '',
      name: '',
      phone: '',
      email: '',
      address: '',
      dateBirth: ''
    };
    if (this.patientForm) {
      this.patientForm.resetForm();
    }
  }
}
