import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MessageComponent } from '../../message/message.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { environment } from 'src/environments/environment';


@Component({
  selector: 'app-confirm-patient-account-deletion',
  standalone: true,
  imports: [MessageComponent, FormsModule, CommonModule],
  templateUrl: './confirm-patient-account-deletion.component.html',
  styleUrls: ['./confirm-patient-account-deletion.component.css'],
})
export class PatientAccountDeletionConfirm implements OnInit {
  message = 'Processing deletion...';

  constructor(private route: ActivatedRoute, private http: HttpClient) {}

  ngOnInit() {
    const userId = this.route.snapshot.queryParamMap.get('userId');
    const token = this.route.snapshot.queryParamMap.get('token');

    if (userId && token) {
      const url = environment.serverBaseUrl + '/Update-PatientAccountDeletionConfirmation?userId='+encodeURIComponent(userId)+'&token='+encodeURIComponent(token);

      this.http.put<{ message: string }>(url, {}).subscribe({
        next: (response) => {
          this.message = `${response.message}`;
        },
        error: (err) => {
          this.message = 'Patient account deletion failed. Please try again.';
          console.error(err);
        },
      });
      

    } else {
      this.message = 'Invalid patient account deletion link.';
    }

  }
}
