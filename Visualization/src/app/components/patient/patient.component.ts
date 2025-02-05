import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SideBarPatientComponent } from "./sidebar-patient/side-bar-patient.component";

@Component({
  selector: 'app-patient',
  standalone: true,
  imports: [RouterLink, SideBarPatientComponent],
  templateUrl: './patient.component.html',
  styleUrl: './patient.component.css'
})
export class PatientComponent {

}
