import { TestBed, ComponentFixture } from '@angular/core/testing';
import { CreateRoomTypeComponent } from './create-room-type.component';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';
import { MessageComponent } from '../../message/message.component';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { RoomTypeService } from 'src/app/services/room-type.service';
import { DebugElement } from '@angular/core';
import { By } from '@angular/platform-browser';

describe('CreateRoomTypeComponent', () => {
  let component: CreateRoomTypeComponent;
  let fixture: ComponentFixture<CreateRoomTypeComponent>;
  let mockRoomTypeService: jasmine.SpyObj<RoomTypeService>;
  let debugElement: DebugElement;

  beforeEach(async () => {
    mockRoomTypeService = jasmine.createSpyObj('RoomTypeService', ['createRoomType']);

    await TestBed.configureTestingModule({
      imports: [
        CreateRoomTypeComponent,
        RouterTestingModule,
        FormsModule,
        MessageComponent,
        SideBarAdminComponent
      ],
      providers: [
        { provide: RoomTypeService, useValue: mockRoomTypeService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateRoomTypeComponent);
    component = fixture.componentInstance;
    debugElement = fixture.debugElement;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize roomtype with empty values', () => {
    expect(component.roomtype).toEqual({
      code: '',
      designation: '',
      description: '',
    });
  });

  it('should not call createRoomType on invalid form submission', () => {
    // Set up invalid form data
    component.roomtype = {
      code: '',
      designation: '',
      description: '',
    };

    // Simulate form submission
    const form = debugElement.query(By.css('form')).nativeElement;
    form.dispatchEvent(new Event('submit'));

    // Expect service method not to be called
    expect(mockRoomTypeService.createRoomType).not.toHaveBeenCalled();
  });

  it('should remove invalid-placeholder class from inputs on clearForm', () => {
    // Add invalid-placeholder class to input elements
    const inputElements = document.querySelectorAll('.input-field input');
    inputElements.forEach(input => input.classList.add('invalid-placeholder'));

    // Simulate calling clearForm
    component.clearForm();

    // Expect invalid-placeholder class to be removed
    inputElements.forEach(input => {
      expect(input.classList.contains('invalid-placeholder')).toBe(false);
    });
  });
});
