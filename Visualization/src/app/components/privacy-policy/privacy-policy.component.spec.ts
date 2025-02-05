import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MessageComponent } from '../message/message.component';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

describe('MessageComponent', () => {
  let component: MessageComponent;
  let fixture: ComponentFixture<MessageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [MessageComponent, CommonModule, RouterLink]
    });
    fixture = TestBed.createComponent(MessageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
