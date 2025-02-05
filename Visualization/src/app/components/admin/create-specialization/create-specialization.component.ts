import { Component, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { MessageComponent } from '../../message/message.component';
import { CommonModule } from '@angular/common';
import { Specialization } from 'src/app/domain/specialization';
import { SpecializationService } from 'src/app/services/specialization.service';


@Component({
  selector: 'app-create-specialization',
  standalone: true,
  imports: [SideBarAdminComponent, MessageComponent, FormsModule, CommonModule],
  templateUrl: './create-specialization.component.html',
  styleUrl: './create-specialization.component.css'
})
export class CreateSpecializationComponent {
@ViewChild('specializationForm') specializationForm!: NgForm;

  isSubmitted = false;
  specialization: Specialization = {
    code: '',
    denomination: '',
    description: '',
  };

  constructor(private service: SpecializationService) { }

  onSubmit(form: NgForm): void {
    if (form.valid) {
      this.isSubmitted = true;
      this.service.createSpecialization(this.specialization.code, this.specialization.denomination, this.specialization.description!);
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.specialization = {
      code: '',
      denomination: '',
      description: '',
    };
  
    if (this.specializationForm) {
      this.specializationForm.resetForm();
    }
  
    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }
}
