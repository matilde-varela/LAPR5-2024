import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { MessageComponent } from '../../message/message.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { param } from 'jquery';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-verify',
  standalone: true,
  imports: [MessageComponent, FormsModule, CommonModule],
  templateUrl: './verify.component.html',
  styleUrls: ['./verify.component.css'],
})
export class ActivationComponent implements OnInit {
  message = 'Processing activation...';

  constructor(private route: ActivatedRoute, private http: HttpClient) {}

  ngOnInit() {
    const userId = this.route.snapshot.queryParamMap.get('userId');
    const token = this.route.snapshot.queryParamMap.get('token');

    if (userId && token) {
      const url = environment.serverBaseUrl + '/activate-patient?userId='+encodeURIComponent(userId)+'&token='+encodeURIComponent(token);


      this.http.put<{ message: string }>(url, {}).subscribe({
        next: (response) => {
          this.message = `${response.message}`;
        },
        error: (err) => {
          this.message = 'Account activation failed. Please try again.';
          console.error(err);
        },
      });
      

    } else {
      this.message = 'Invalid activation link.';
    }

  }
}
