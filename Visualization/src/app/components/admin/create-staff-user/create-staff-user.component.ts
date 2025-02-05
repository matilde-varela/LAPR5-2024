import { Component, NgModule, OnInit, ViewChild } from '@angular/core';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { MessageComponent } from '../../message/message.component';
import { UserStaff } from '../../../domain/UserStaff';
import { UserService } from '../../../services/user.service';
import { Router } from '@angular/router';

interface UserInfo {
  email: string;
  roles: string[];
  token: string;
}

@Component({
  selector: 'app-create-staff-user',
  standalone: true,
  imports: [SideBarAdminComponent, CommonModule, FormsModule, MessageComponent],
  templateUrl: './create-staff-user.component.html',
  styleUrl: './create-staff-user.component.css'
})
export class CreateStaffUserComponent implements OnInit{
  @ViewChild('userStaffForm') userStaffForm!: NgForm;

  roles: string[] = [`Doctor`, `Technician`,`Admin`, `Nurse`, `Patient`];

  isSubmitted = false;
  userStaff: UserStaff = {
      email: '',
      password: '',
      phone: '',
      role: ''
    };

    userInfo : UserInfo ={
      email : '',
      roles : [],
      token : ''
    }

  storedToken = localStorage.getItem('user');

  constructor(private service: UserService,  private router: Router) { }
  ngOnInit(): void {
    if(this.storedToken){

      this.userInfo = JSON.parse(this.storedToken);

      if(!this.userInfo.roles.includes('Admin')){
        this.router.navigate(['']);
      }
    }
    else
    {
      this.router.navigate(['login']);
    }
  }

  onSubmit(form: NgForm): void {
    if (form.valid) {
      this.isSubmitted = true;
      this.userStaff.role = this.userStaff.role.toLowerCase();
      console.log(this.userStaff);
      this.service.createStaffUser(this.userStaff.email, this.userStaff.password, this.userStaff.phone, this.userStaff.role);
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {

    this.userStaff = {
      email: '',
      password: '',
      phone: '',
      role: ''
    };
    this.isSubmitted = false;

    if (this.userStaffForm) {
      this.userStaffForm.resetForm();
    }

    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }

}
