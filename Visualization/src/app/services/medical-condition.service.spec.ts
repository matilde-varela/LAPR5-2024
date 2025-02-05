import { TestBed } from '@angular/core/testing';

import { MessageService } from './message.service';
import { MedicalConditionService } from './medical-condition.service';
import { HttpClientModule } from '@angular/common/http';

describe('MedicalConditionService', () => {
  let service: MedicalConditionService;

  beforeEach(() => {
      TestBed.configureTestingModule({
        imports: [HttpClientModule],  
        providers: [MessageService]  
      });
      service = TestBed.inject(MedicalConditionService);
    });
  
    it('should be created', () => {
      expect(service).toBeTruthy();
    });
});
