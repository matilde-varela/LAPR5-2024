import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SideBarAdminComponent } from "../sidebar-admin/side-bar-admin.component";
import { PatientService } from '../../../services/patient.service';
import { MessageComponent } from '../../message/message.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Allergy } from 'src/app/domain/Allergy';
import { MedicalCondition } from 'src/app/domain/MedicalCondition';
import { AllergyService } from 'src/app/services/allergy.service';
import { MedicalConditionService } from 'src/app/services/medical-condition.service';
import { PatientWithMedicalRecord } from 'src/app/domain/patient-with-medical-record';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'app-create-patient-profile',
  standalone: true,
  imports: [SideBarAdminComponent, MessageComponent, FormsModule, CommonModule, DropdownModule],
  providers : [MessageComponent],
  templateUrl: './create-patient-profile.component.html',
  styleUrls: ['./create-patient-profile.component.css']
})
export class CreatePatientProfileComponent {
  @ViewChild('patientForm') patientForm!: NgForm;

  isSubmitted = false;
  patient : PatientWithMedicalRecord = {
    firstName: '',
    lastName: '',
    phone: '',
    emergencyContact: '',
    email: '',
    address: '',
    gender: '',
    dateBirth: '',
    medicalConditions : [],
    allergies: [],
    description: ''
  };

    currentMedicalCondition = { id: '', designation: '', description: '', symptoms: '' };
    filteredConditions: string[] = [];
    allMedicalConditionsDesignations: string[] = [];
    allAllergiesDesignations: string[] = [];
  
    allFullAllergies : Allergy[] = [];
    allFullMedicalConditions : MedicalCondition[] = [];
    
  constructor(private service: PatientService, private allergyService : AllergyService , private medicalConditionService : MedicalConditionService,  private message : MessageComponent) { }

  ngOnInit(): void {
    this.fetchAllergies();
    this.fetchMedicalConditions();
}

fetchMedicalConditions() {
  this.medicalConditionService.getAllMedicalConditions().subscribe({
    next: (data) => {
      this.allFullMedicalConditions = data;
      this.allMedicalConditionsDesignations = this.allFullMedicalConditions.map(condition => {
        return condition.designation;
      });
    },
    error: (error) => console.error('Error fetching medical conditions:', error),
  });

}


fetchAllergies() {
  this.allergyService.getAllAllergies().subscribe({
    next: (data) => {
      this.allFullAllergies = data;
      this.allAllergiesDesignations = this.allFullAllergies.map(allergy => {
        return allergy.designation;
      });
    },
    error: (error) => console.error('Error fetching allergies:', error),
  });
}

  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    console.log(this.patient);
    if (form.valid) {
      this.service.createPatientProfile(
        this.patient.firstName,
        this.patient.lastName,
        this.patient.phone,
        this.patient.email,
        this.patient.address,
        this.patient.emergencyContact,
        this.patient.gender,
        this.patient.dateBirth,
        this.patient.medicalConditions,
        this.patient.allergies,
        this.patient.description
      );
    } else {
      this.isSubmitted = false;
    }
  }

  addMedicalCondition(inputValue: string): void {
    console.log(inputValue);
    if (inputValue.trim()) {
      const fullCondition = this.allFullMedicalConditions.find(
        (condition) => condition.designation.toLowerCase() === inputValue.toLowerCase()
      );

      if (fullCondition) {
        const conditionExists = this.patient.medicalConditions.some(
          (condition) => condition.designation.toLowerCase() === fullCondition.designation.toLowerCase()
        );

        if (!conditionExists) {
          this.patient.medicalConditions.push({ ...fullCondition });
        } else {
          console.warn('Condition already exists in the list.');
          this.message.messageService.add('Condition already exists in the list.')
        }
      } else {
        console.warn('Condition not found in data.');
        this.message.messageService.add('Condition not found in data.')
      }
    }
  }

  addAllergy(inputValue: string): void {
    console.log(inputValue);
    if (inputValue.trim()) {
      const fullAllergy = this.allFullAllergies.find(
        (allergy) => allergy.designation.toLowerCase() === inputValue.toLowerCase()
      );

      if (fullAllergy) {
        const allergyExists = this.patient.allergies.some(
          (allergy) => allergy.designation.toLowerCase() === fullAllergy.designation.toLowerCase()
        );

        if (!allergyExists) {
          this.patient.allergies.push({ ...fullAllergy });
        } else {
          console.warn('Allergy already exists in the list.');
          this.message.messageService.add('Allergy already exists in the list.')
        }
      } else {
        console.warn('Allergy not found in data.');
        this.message.messageService.add('Allergy not found in data.')
      }
    }
  }

  removeMedicalCondition(index: number): void {
    this.patient.medicalConditions.splice(index, 1);
  }


  removeAllergy(index: number): void {
    this.patient.allergies.splice(index, 1);
  }


  clearForm(): void {
    this.isSubmitted = false;

    this.patient = {
      firstName: '',
      lastName: '',
      phone: '',
      emergencyContact: '',
      email: '',
      address: '',
      gender: '',
      dateBirth: '',
      medicalConditions : [],
      allergies: [],
      description: ''
    };
    if (this.patientForm) {
      this.patientForm.resetForm();
    }
    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }
}

