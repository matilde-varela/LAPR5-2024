import { TestBed } from '@angular/core/testing';

import { SurgeryAppointmentService } from './surgery-appointment.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MessageService } from './message.service';
import { environment } from 'src/environments/environment';
import { HttpClientModule } from '@angular/common/http';

describe('SurgeryAppointmentService', () => {
  let service: SurgeryAppointmentService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientModule],  
      providers: [MessageService]  
    });
    service = TestBed.inject(SurgeryAppointmentService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
