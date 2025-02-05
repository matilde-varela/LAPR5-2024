import { TestBed } from '@angular/core/testing';

import { OperationRequestService } from './operation-request.service';
import { MessageService } from './message.service';
import { HttpClientModule } from '@angular/common/http';

describe('OperationRequestService', () => {
  let service: OperationRequestService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientModule],  
      providers: [MessageService]  
    });
    service = TestBed.inject(OperationRequestService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
