import { Component, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { MessageComponent } from '../../message/message.component';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { StaffService } from '../../../services/staff.service';
import { StaffListingFilterParameters } from '../../../domain/staff-listing-filter-parameters';
import { StaffWithId } from '../../../domain/staff-with-id';
import { StaffQueryParameters } from '../../../domain/staff-query-parameters';

interface UserInfo {
  email: string;
  roles: string[];
  token: string;
}

@Component({
  selector: 'app-list-staff-profiles',
  standalone: true,
  imports: [SideBarAdminComponent, CommonModule, TableModule, FormsModule, MessageComponent],
  templateUrl: './list-staff-profiles.component.html',
  styleUrls: ['./list-staff-profiles.component.css']
})
export class ListStaffProfiles implements OnInit {
  @ViewChild('staffForm') staffForm!: NgForm;

  isSubmitted = false;
  staff = {
    id: '',
    phone: '',
    email: '',
    address: '',
    specializationId: ''
  };

  staffList: StaffWithId[] = [];
  selectedStaff!: StaffWithId;
  fullStaff!: StaffWithId;
  detailsVisible: boolean = false;
  isEditing = false;
  editDetails: boolean = false;

  specializations: string[] = [];
  functions: string[] = ['Doctor', 'Intern', 'Nurse', 'Assistant'];

  // Dynamic query filters
  queryFiltersList: StaffListingFilterParameters[] = [];

  storedToken = localStorage.getItem('user');

  constructor(private service: StaffService) { }

  ngOnInit(): void {
    this.service.getAllSpecializationsAvailable().subscribe({
      next: (data) => {
        this.specializations = data;
      }
    });
    this.addFilter();
    this.fetchStaff();
  }

  fetchStaff(): void {
      const queryParameters: StaffQueryParameters = {
        queryfilters: this.queryFiltersList
      };

      this.service.getStaffByFilters(queryParameters).subscribe({
        next: (data) => {
          this.staffList = data;
        },
        error: (error) => {
          console.error('Error fetching staff profiles:', error);
        }
      });
  }

  applyFilters(): void {
    this.fetchStaff();
  }

  addFilter(): void {
    this.queryFiltersList.push({
      firstName: '',
      lastName: '',
      email: '',
      specialization: ''
    });
  }


  toggleEdition(staff : StaffWithId): void {
    this.editDetails = true;
  }

  closeEdition(): void {
    this.editDetails = false;
  }

  removeFilter(index: number): void {
    this.queryFiltersList.splice(index, 1);
  }

  toggleDetails(staff: StaffWithId): void {
    this.service.getStaffById(staff.id).subscribe({
      next: (fullStaffInfo: StaffWithId) => {
        this.fullStaff = fullStaffInfo;
        this.detailsVisible = true;
      },
      error: (error: any) => {
        console.error('Error fetching staff profile details:', error);
      }
    });
  }

  closeDetails(): void {
    this.detailsVisible = false;
  }

  editStaffProfile(staff: StaffWithId): void {
    this.isEditing = true;
  }

  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    if (form.valid) {
      this.service.EditStaffProfile(
        this.staff.id,
        this.staff.phone || '',
        this.staff.email || '',
        this.staff.address || '',
        this.staff.specializationId || ''
      );
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.isSubmitted = false;

    this.staff = {
      id: '',
      phone: '',
      email: '',
      address: '',
      specializationId: ''
    };
    if (this.staffForm) {
      this.staffForm.resetForm();
    }
  }

  deactivateStaffProfile(staff: StaffWithId): void {
    if (this.storedToken) {
      var userInfo: UserInfo = {
        email: '',
        roles: [],
        token: ''
      }

      userInfo = JSON.parse(this.storedToken);

      this.service.deactivateStaffProfile(staff.id, userInfo.token);
    }
  }
}
