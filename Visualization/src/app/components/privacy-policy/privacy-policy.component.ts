import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { PatientService } from 'src/app/services/patient.service';
import * as marked from 'marked';

@Component({
  selector: 'app-privacy-policy',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './privacy-policy.component.html',
  styleUrls: ['./privacy-policy.component.css']
})
export class PrivacyPolicyComponent implements OnInit {
  showPopup: boolean = false;
  policyContent: SafeHtml = '';

  constructor(private sanitizer: DomSanitizer, private service: PatientService) {}

  ngOnInit(): void {
    this.loadPrivacyPolicy();
  }

  async loadPrivacyPolicy() {
    try {
      const rawPolicy = await this.getTextFromPrivacyPolicyFile();
      const htmlContent = marked.parse(rawPolicy);
      this.policyContent = this.sanitizer.bypassSecurityTrustHtml(htmlContent.toString());
      console.log(this.policyContent);
    } catch (error) {
      console.error('Failed to load the privacy policy:', error);
      this.policyContent = this.sanitizer.bypassSecurityTrustHtml('<p>Error loading Privacy Policy.</p>');
    }
  }

  async getTextFromPrivacyPolicyFile(): Promise<string> {
    try {
      const response = await this.service.getPrivacyPolicyText();
      return response || '';
    } catch (error) {
      console.error('Failed to load the privacy policy file:', error);
      return 'Error loading Privacy Policy.';
    }
  }
  
}
