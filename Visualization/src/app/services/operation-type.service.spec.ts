import { TestBed } from '@angular/core/testing';
import { OperationTypeService } from './operation-type.service';
import { HttpClientModule } from '@angular/common/http';
import { MessageService } from './message.service';

describe('StaffService', () => {
  let service: OperationTypeService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientModule],  
      providers: [MessageService]  
    });
    service = TestBed.inject(OperationTypeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
