import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { StaffService } from '../../../services/staff.service';
import { MessageComponent } from '../../message/message.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-staff-profile',
  standalone: true,
  imports: [SideBarAdminComponent, MessageComponent, FormsModule, CommonModule],
  templateUrl: './edit-staff-profile.component.html',
  styleUrl: './edit-staff-profile.component.css'
})
export class EditStaffProfileComponent {
  @ViewChild('staffForm') staffForm!: NgForm;

  isSubmitted = false;
  staff = {
    id: '',
    phone: '',
    email: '',
    address: '',
    specializationId: ''
  };

  specializations : string[] = [];

  constructor(private service: StaffService) { }

  ngOnInit(): void {  
    this.service.getAllSpecializationsAvailable().subscribe({
      next: data => {
        this.specializations = data;
      }
    });
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
}
