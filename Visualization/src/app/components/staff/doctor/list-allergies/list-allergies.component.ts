import { Component, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { MessageComponent } from '../../../message/message.component';
import { Allergy } from 'src/app/domain/Allergy';
import { AllergyListingFilterParameters } from 'src/app/domain/allergy-listing-filter-parameters';
import { AllergyQueryParameters } from 'src/app/domain/allergy-query-parameters';
import { AllergyService } from 'src/app/services/allergy.service';
import { SideBarDoctorComponent } from '../sidebar-doctor/side-bar-doctor.component';


@Component({
  selector: 'app-list-patient-profiles',
  standalone: true,
  imports: [SideBarDoctorComponent, CommonModule, TableModule, FormsModule, MessageComponent],
  templateUrl: './list-allergies.component.html',
  styleUrls: ['./list-allergies.component.css']
})
export class ListAllergies implements OnInit {
  @ViewChild('allergyForm') allergyForm!: NgForm;

  isSubmitted = false;
  allergy = {
    code: '',
    designation: '',
    description: ''
  };

  allergyList: Allergy[] = [];
  selectedAllergy!: Allergy;
  fullAllergy!: Allergy;
  detailsVisible: boolean = false;
  isEditing = false;
  editDetails: boolean = false;
  queryFiltersList: AllergyListingFilterParameters[] = [];

  constructor(private service: AllergyService) {}

  ngOnInit(): void {
    this.addFilter();
    this.fetchAllergies();
  }

  fetchAllergies(): void {
    this.allergyList = [];
    const queryParameters: AllergyQueryParameters = {
      queryfilters: this.queryFiltersList
    };

    this.service.getAllergiesByFilters(queryParameters).subscribe({
      next: (data) => {
        this.allergyList = data;
      },
      error: (error) => {
        console.error('Error fetching allergies:', error);
      }
    });
  }

  applyFilters(): void {
    this.fetchAllergies();
  }

  toggleEdition(allergy : Allergy): void {
    this.editDetails = true;
  }

  closeEdition(): void {
    this.editDetails = false;
  }

  addFilter(): void {
    this.queryFiltersList.push({
      code: '',
      designation: '',
      description: ''
    });
  }

  removeFilter(index: number): void {
    this.queryFiltersList.splice(index, 1);
  }

  toggleDetails(allergy : Allergy): void {
    console.log(allergy);
    this.service.getAllergyByCode(allergy.code).subscribe({
      next: (fullAllergyInfo: Allergy) => {
        this.fullAllergy = fullAllergyInfo;
        this.detailsVisible = true;
      },
      error: (error: any) => {
        console.error('Error fetching allergy details:', error);
      }
    });
  }

  closeDetails(): void {
    this.detailsVisible = false;
  }

  editPatientProfile(allergy : Allergy): void {
    // Implement edit logic
    this.isEditing = true;
  }

  clearForm(): void {
    this.isSubmitted = false;

    this.allergy = {
      code: '',
      designation: '',
      description: ''
    };
    if (this.allergyForm) {
      this.allergyForm.resetForm();
    }
  }

}
