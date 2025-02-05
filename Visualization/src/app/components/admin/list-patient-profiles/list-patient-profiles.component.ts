import { Component, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { MessageComponent } from '../../message/message.component';

import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { PatientService } from '../../../services/patient.service';
import { PatientWithId } from '../../../domain/patient-with-id';
import { PatientQueryParameters } from '../../../domain/patient-query-parameters';
import { PatientListingFilterParameters } from '../../../domain/patient-listing-filter-parameters';


@Component({
  selector: 'app-list-patient-profiles',
  standalone: true,
  imports: [SideBarAdminComponent, CommonModule, TableModule, FormsModule, MessageComponent],
  templateUrl: './list-patient-profiles.component.html',
  styleUrls: ['./list-patient-profiles.component.css']
})
export class ListPatientProfiles implements OnInit {
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

  patientList: PatientWithId[] = [];
  selectedPatient!: PatientWithId;
  fullPatient!: PatientWithId;
  detailsVisible: boolean = false;
  isEditing = false;
  editDetails: boolean = false;

  specializations: string[] = [];
  functions: string[] = ['Doctor', 'Intern', 'Nurse', 'Assistant'];
  queryFiltersList: PatientListingFilterParameters[] = [];

  constructor(private service: PatientService) {}

  ngOnInit(): void {
    this.addFilter();
    this.fetchPatients();
  }

  fetchPatients(): void {
    const queryParameters: PatientQueryParameters = {
      queryfilters: this.queryFiltersList
    };

    this.service.getPatientsByFilters(queryParameters).subscribe({
      next: (data) => {
        this.patientList = data;
      },
      error: (error) => {
        console.error('Error fetching patient profiles:', error);
      }
    });
  }

  applyFilters(): void {
    this.fetchPatients();
  }

  toggleEdition(patient : PatientWithId): void {
    this.editDetails = true;
  }

  closeEdition(): void {
    this.editDetails = false;
  }

  addFilter(): void {
    this.queryFiltersList.push({
      firstName : '',
      lastName : '',
      email : '',
      gender : '',
      dateBirth : '',
      medicalRecordNumber : ''
    });
  }

  removeFilter(index: number): void {
    this.queryFiltersList.splice(index, 1);
  }

  toggleDetails(patient : PatientWithId): void {
    console.log(patient);
    this.service.getPatientById(patient.patientId).subscribe({
      next: (fullPatientInfo: PatientWithId) => {
        this.fullPatient = fullPatientInfo;
        this.detailsVisible = true;
      },
      error: (error: any) => {
        console.error('Error fetching patient profile details:', error);
      }
    });
  }

  closeDetails(): void {
    this.detailsVisible = false;
  }

  editPatientProfile(patient : PatientWithId): void {
    // Implement edit logic
    this.isEditing = true;
  }

  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    if (form.valid) {
      this.service.editPatientProfile(
        this.patient.id,
        this.patient.name || '',
        this.patient.phone || '',
        this.patient.email || '',
        this.patient.address || '',
        this.patient.dateBirth || ''
      );
    } else {
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

  deletePatientProfile(patient : PatientWithId): void {
    this.service.deactivatePatientProfile(patient.patientId);
  }

}
