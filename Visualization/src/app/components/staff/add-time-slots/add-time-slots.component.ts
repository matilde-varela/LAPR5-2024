import { Component, ViewChild } from '@angular/core';
import { NgModule } from '@angular/core';
import { SideBarStaffComponent } from '../sidebar-staff/side-bar-staff.component';
import { MessageComponent } from '../../message/message.component';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { StaffService } from 'src/app/services/staff.service';

@Component({
  selector: 'app-add-time-slots',
  standalone: true,
  imports: [SideBarStaffComponent, MessageComponent, FormsModule, CommonModule],
  templateUrl: './add-time-slots.component.html',
  styleUrl: './add-time-slots.component.css'
})
export class AddTimeSlotsComponent {
  
  @ViewChild('addSlotForm') addSlotForm!: NgForm;

  isSubmitted = false;
  addSlot = {
    slot: '',
    date: ''
  };

  constructor(private service: StaffService) { }

  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    if (form.valid) {
      this.service.StaffProfileAddSlot(
        this.addSlot.slot,
        this.addSlot.date
      ).subscribe(
        (response) => {
            console.log('Slot added:', response);
        },
        (error) => {
            console.error('Error adding slot:', error);
        }
      );
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.isSubmitted = false;

    this.addSlot = {
      slot: '',
      date: ''
    };
    if (this.addSlotForm) {
      this.addSlotForm.resetForm();
    }
  }

}
