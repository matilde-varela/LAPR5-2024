import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SideBarAdminComponent } from "../sidebar-admin/side-bar-admin.component";
import { StaffService } from '../../../services/staff.service';
import { MessageComponent } from '../../message/message.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create-staff-profile',
  standalone: true,
  imports: [SideBarAdminComponent, MessageComponent, FormsModule, CommonModule],
  templateUrl: './create-staff-profile.component.html',
  styleUrls: ['./create-staff-profile.component.css']
})
export class CreateStaffProfileComponent {
  @ViewChild('staffForm') staffForm!: NgForm;

  isSubmitted = false;
  staff = {
    firstName: '',
    lastName: '',
    phone: '',
    licenseNumber: '',
    email: '',
    address: '',
    specialization: '',
    function: ''
  };

  specializations : string[] = [];
  functions : string[] = [`Doctor`, `Intern`,`Nurse`, `Assistant`];

  constructor(private service: StaffService) { }

  ngOnInit(): void {  
    this.service.getAllSpecializationsAvailable().subscribe({
      next: data => {
        this.specializations = data;
        console.log(data);
      }
    });
    
  }


  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    if (form.valid) {
      this.service.createStaffProfile(
        this.staff.firstName,
        this.staff.lastName,
        this.staff.phone,
        this.staff.email,
        this.staff.address,
        this.staff.licenseNumber,
        this.staff.specialization,
        this.staff.function
      );
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.isSubmitted = false;

    this.staff = {
      firstName: '',
      lastName: '',
      phone: '',
      licenseNumber: '',
      email: '',
      address: '',
      specialization: '',
      function: ''
    };
    if (this.staffForm) {
      this.staffForm.resetForm();
    }
    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }
}

