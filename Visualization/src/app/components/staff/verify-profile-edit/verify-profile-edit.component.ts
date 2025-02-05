import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { StaffService } from '../../../services/staff.service';


@Component({
  selector: 'app-verify-profile-edit',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './verify-profile-edit.component.html',
  styleUrl: './verify-profile-edit.component.css'
})
export class VerifyProfileEditComponent implements OnInit {
  message = 'Processing activation...';
  constructor(private route: ActivatedRoute, private staffService: StaffService) { }

  ngOnInit() {
    const userId = this.route.snapshot.queryParamMap.get('userId');
    const staffId = this.route.snapshot.queryParamMap.get('staffId');
    const token = this.route.snapshot.queryParamMap.get('token');

    if (userId && staffId && token) {
      this.staffService.confirmEmailStaff(userId, staffId, token).subscribe({
        next: (response) => {
          this.message = `${response.message}`;
        },
        error: (err) => {
          this.message = 'The contact information change failed. Please try again.';
          console.error(err);
        },
      });

    } else {
      this.message = 'Invalid link.';
    }

  }
}
