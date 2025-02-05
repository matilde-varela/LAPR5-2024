import { TestBed } from '@angular/core/testing';
import { MessageService } from './message.service';
import { HttpClientModule } from '@angular/common/http';
import { AllergyService } from './allergy.service';

describe('AllergyService', () => {
  let service: AllergyService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientModule],  
      providers: [MessageService]  
    });
    service = TestBed.inject(AllergyService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
