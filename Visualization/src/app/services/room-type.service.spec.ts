import { TestBed } from '@angular/core/testing';
import { MessageService } from './message.service';
import { HttpClientModule } from '@angular/common/http';
import { RoomTypeService } from './room-type.service';

describe('RoomTypeService', () => {
  let service: RoomTypeService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientModule],  
      providers: [MessageService]  
    });
    service = TestBed.inject(RoomTypeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
