import { TestBed } from '@angular/core/testing';
import { UserService } from './user.service';
import { MessageService } from './message.service';
import { HttpClientModule } from '@angular/common/http'; // Correct module import
import { RouterTestingModule } from '@angular/router/testing';
import { Router } from '@angular/router';

describe('UserService', () => {
  let service: UserService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientModule, RouterTestingModule],  
      providers: [MessageService]  
    });
    service = TestBed.inject(UserService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});