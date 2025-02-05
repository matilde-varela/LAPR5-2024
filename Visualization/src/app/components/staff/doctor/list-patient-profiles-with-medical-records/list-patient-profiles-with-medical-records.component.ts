import { Component, input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { PatientService } from 'src/app/services/patient.service';
import { PatientWithId } from 'src/app/domain/patient-with-id';
import { PatientQueryParameters } from 'src/app/domain/patient-query-parameters';
import { MessageComponent } from 'src/app/components/message/message.component';
import { SideBarDoctorComponent } from '../sidebar-doctor/side-bar-doctor.component';
import { AllergyMedicalConditionFilterParameters } from 'src/app/domain/allergy-medical-condition-filter-parameters';
import { PatientListingFilterParameters } from 'src/app/domain/patient-listing-filter-parameters';
import { MedicalRecordComplete } from 'src/app/domain/MedicalRecordComplete';
import { MedicalCondition } from 'src/app/domain/MedicalCondition';
import { Allergy } from 'src/app/domain/Allergy';
import { DropdownModule } from 'primeng/dropdown';
import { AllergyService } from 'src/app/services/allergy.service';
import { MedicalConditionService } from 'src/app/services/medical-condition.service';



@Component({
  selector: 'app-list-patient-profiles-with-medical-records',
  standalone: true,
  imports: [CommonModule, TableModule, MessageComponent, SideBarDoctorComponent, FormsModule, DropdownModule],
  providers : [MessageComponent],
  templateUrl: './list-patient-profiles-with-medical-records.component.html',
  styleUrls: ['./list-patient-profiles-with-medical-records.component.css'],
})
export class ListPatientProfilesWithMedicalRecord implements OnInit {
  @ViewChild('medicalRecordForm') medicalRecordForm!: NgForm;

  medicalRecord : MedicalRecordComplete = {
    id : '',
    medicalRecordNumber : '',
    medicalConditions : [],
    allergies: [],
    description : ''
  };

  isSubmitted = false;
  patientList: PatientWithId[] = [];
  selectedPatient!: PatientWithId;
  fullPatient!: PatientWithId;
  fullPatientMedicalRecord!: MedicalRecordComplete;
  showMedicalRecord: boolean = false;
  editingMedicalRecord: boolean = false;

  medicalRecords: MedicalRecordComplete[] = [];
  medicalRecordNumbers: string[] = [];

  filterParameters: AllergyMedicalConditionFilterParameters = {
    filters: [],
  };

  selectedMedicalCondition: string = '';

  currentMedicalCondition = { id: '', designation: '', description: '', symptoms: '' };
  filteredConditions: string[] = [];
  allMedicalConditionsDesignations: string[] = [];
  allAllergiesDesignations: string[] = [];

  allFullAllergies : Allergy[] = [];
  allFullMedicalConditions : MedicalCondition[] = [];

  constructor(private service: PatientService, private allergyService :AllergyService , private medicalConditionService : MedicalConditionService, private message : MessageComponent) {}

  ngOnInit(): void {
    this.addFilter();
    this.fetchAllergies();
    this.fetchMedicalConditions();
    this.fetchPatients();
    this.fetchPatientsMedicalRecords();
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



  fetchPatients(): void {
    const emptyFilter: PatientListingFilterParameters = {
      firstName: "",
      lastName: "",
      email: "",
      gender: "",
      dateBirth: "",
      medicalRecordNumber: ""
    };

    const queryParameters: PatientQueryParameters = {
      queryfilters: [emptyFilter] // Single empty filter
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


  fetchPatientsMedicalRecords(): void {
    this.service.getAllMedicalRecords().subscribe({
      next: (data) => { (this.medicalRecords = data),
      console.log(this.medicalRecords) },
      error: (error) => console.error('Error fetching medical records:', error),
    });
  }

  applyFilters(): void {
    this.fetchFilteredPatientMedicalRecords();
  }

  fetchFilteredPatientMedicalRecords(): void {
    this.service.getFilteredMedicalRecords(this.filterParameters).subscribe({
      next: (records) => {
        this.medicalRecords = records;

        this.medicalRecordNumbers = this.medicalRecords.map(record => record.medicalRecordNumber);

        this.fetchPatientProfilesByMedicalRecordNumber();
      },
      error: (error) => {
        console.error('Error fetching medical records:', error);
      },
    });
  }

  fetchPatientProfilesByMedicalRecordNumber(): void {

    const patientQueryParameters: PatientQueryParameters = {
      queryfilters: this.medicalRecordNumbers.map((recordNumber) => ({
        firstName: '',
        lastName: '',
        email: '',
        gender: '',
        dateBirth: '',
        medicalRecordNumber: recordNumber,
      })),
    };

    this.service.getPatientsByFilters(patientQueryParameters).subscribe({
      next: (patients) => {
        this.patientList = patients;
        console.log('Filtered Patient Profiles:', this.patientList);
      },
      error: (error) => {
        console.error('Error fetching patient profiles:', error);
      },
    });
  }

  addFilter(): void {
    this.filterParameters.filters.push({ allergyDesignation: '', medicalConditionDesignation: '' });
  }

  removeFilter(index: number): void {
    this.filterParameters.filters.splice(index, 1);
  }

  toggleMedicalRecord(patient: PatientWithId): void {
        this.fullPatient = patient;
        this.fullPatientMedicalRecord = this.medicalRecords.find(record => record.id === patient.patientId)!;
        this.medicalRecord = this.fullPatientMedicalRecord;
        this.showMedicalRecord = true;
        this.editingMedicalRecord = false;
        console.log(this.fullPatientMedicalRecord.description);
  }

  get formattedDescription(): string[] {
    return this.fullPatientMedicalRecord.description
      ? this.fullPatientMedicalRecord.description.split('\n')
      : [];
  }

  editMedicalRecord(): void {
    this.showMedicalRecord = false;
    this.editingMedicalRecord = true;
}

closeEditing() : void {
  this.editingMedicalRecord = false;
}

  closeDetails(): void {
    this.showMedicalRecord = false;
  }

  filterConditions(): void {
    this.filteredConditions = this.allMedicalConditionsDesignations.filter((condition) =>
      condition.toLowerCase().includes(this.currentMedicalCondition.designation.toLowerCase())
    );
  }

  addMedicalCondition(inputValue: string): void {
    console.log(inputValue);
    if (inputValue.trim()) {
      const fullCondition = this.allFullMedicalConditions.find(
        (condition) => condition.designation.toLowerCase() === inputValue.toLowerCase()
      );

      if (fullCondition) {
        const conditionExists = this.medicalRecord.medicalConditions.some(
          (condition) => condition.designation.toLowerCase() === fullCondition.designation.toLowerCase()
        );

        if (!conditionExists) {
          this.medicalRecord.medicalConditions.push({ ...fullCondition });
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
        const allergyExists = this.medicalRecord.allergies.some(
          (allergy) => allergy.designation.toLowerCase() === fullAllergy.designation.toLowerCase()
        );

        if (!allergyExists) {
          this.medicalRecord.allergies.push({ ...fullAllergy });
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
    this.medicalRecord.medicalConditions.splice(index, 1);
  }


  removeAllergy(index: number): void {
    this.medicalRecord.allergies.splice(index, 1);
  }


  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    if (form.valid) {
      this.service.updateMedicalRecord(
        this.medicalRecord.id,
        this.medicalRecord.medicalRecordNumber,
        this.medicalRecord.medicalConditions,
        this.medicalRecord.allergies,
        this.medicalRecord.description || ''
      );
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.isSubmitted = false;

    this.medicalRecord = {
      id : '',
      medicalRecordNumber : '',
      medicalConditions : [],
      allergies: [],
      description : ''
    };
    if (this.medicalRecordForm) {
      this.medicalRecordForm.resetForm();
    }
  }
}
