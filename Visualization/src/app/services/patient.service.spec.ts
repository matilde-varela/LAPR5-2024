import { TestBed } from '@angular/core/testing';

import { PatientService } from './patient.service';
import { HttpClientModule } from '@angular/common/http';
import { MessageService } from './message.service';

describe('PatientService', () => {
  let service: PatientService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientModule],  
      providers: [MessageService]  
    });
    service = TestBed.inject(PatientService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
