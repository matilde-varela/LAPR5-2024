import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { RouterLink, Router } from '@angular/router';
import { AdminComponent } from "../admin/admin.component";
import { StaffComponent } from '../staff/staff.component';
import { PatientComponent } from '../patient/patient.component';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css'
})

export class MenuComponent implements OnInit {
  constructor(private userService: UserService, private router: Router) {}

  ngOnInit(): void {}
}
