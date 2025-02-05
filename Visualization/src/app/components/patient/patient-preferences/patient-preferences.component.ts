import { Component } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { MessageComponent } from '../../message/message.component';
import { CommonModule } from '@angular/common';
import { SideBarPatientComponent } from '../sidebar-patient/side-bar-patient.component';
import { PrivacyPolicyComponent } from '../../privacy-policy/privacy-policy.component';

@Component({
  selector: 'app-patient-preferences',
  standalone: true,
  imports: [SideBarPatientComponent, MessageComponent, CommonModule, PrivacyPolicyComponent],
  templateUrl: './patient-preferences.component.html',
  styleUrls: ['./patient-preferences.component.css']
})
export class PatientPreferences {

  constructor(private service: UserService) { }

  confirmDelete() {
    this.service.sendAccountDeleteRequest();
  }
}
