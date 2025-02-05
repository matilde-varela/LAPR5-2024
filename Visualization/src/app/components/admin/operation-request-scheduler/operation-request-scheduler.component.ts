import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { MessageComponent } from '../../message/message.component';

import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { StaffService } from '../../../services/staff.service';
import { Router } from '@angular/router';
import { UserInfo } from '../../../domain/user-info';
import { OperationRequestService } from '../../../services/operation-request.service';
import { OperationRequest } from '../../../domain/OperationRequest';
import { Room } from '../../../domain/room';
import { RoomService } from '../../../services/room.service';
import { StaffWithFunction } from '../../../domain/staff-with-function';
import { SchedulingBackend } from 'src/app/domain/scheduling-backend';
import { SchedulingData } from 'src/app/domain/scheduling-data';

@Component({
  selector: 'app-operation-request-scheduler',
  standalone: true,
  imports: [SideBarAdminComponent, CommonModule, TableModule, FormsModule, MessageComponent],
  templateUrl: './operation-request-scheduler.component.html',
  styleUrls: ['./operation-request-scheduler.component.css']
})

export class OperationRequestScheduler implements OnInit {

  staffList: StaffWithFunction[] = [];
  selectedStaff: StaffWithFunction[] = [];
  roomList: Room[] = [];
  selectedRoom: Room[] = [];
  operationRequestList: OperationRequest[] = [];
  selectedOperationRequest!: OperationRequest;
  algorithm = '';
  day !: Date;

  scheduleOperation !: SchedulingData;
  schedulingBackend: SchedulingBackend = {
    roomID : [],
    schedulingData : [],
    algorithm : '',
    date : ''
  };


  storedToken = localStorage.getItem('user');
  isRoomRestricted: boolean = false;

  constructor(private staffService: StaffService, private roomService : RoomService, private opRequestsService : OperationRequestService, private router: Router) {}

  ngOnInit(): void {
    if(this.storedToken){

      var userInfo : UserInfo;

      userInfo = JSON.parse(this.storedToken);

      if(userInfo.roles.includes('Admin')){
        this.fetchStaff();
        this.fetchOperationRequests();
        this.fetchRooms();
      }
      else
      {
        this.router.navigate(['']);
      }
    }
    else
    {
      this.router.navigate(['login']);
    }
  }

  addSchedulingData() {
    this.schedulingBackend.schedulingData.push({
      staff : [],
      operationRequestID : ''
    });
  }

  removeSchedulingData(index: number) {
    this.schedulingBackend.schedulingData.splice(index, 1);
  }

  fetchOperationRequests() {
    if(this.storedToken){
      this.opRequestsService.getAllOperationRequests().subscribe({
        next: (data) => {
          this.operationRequestList = data;
        },
        error: (error) => {
          console.error('Error fetching operation requests:', error);
        }
      });
    }
  }

  fetchStaff() {
    if(this.storedToken){
      this.staffService.getAllActiveStaffProfiles().subscribe({
        next: (data) => {
          this.staffList = data;
        },
        error: (error) => {
          console.error('Error fetching staff profiles:', error);
        }
      });
    }
  }

  fetchRooms() {
    if(this.storedToken){
      this.roomService.getAllRooms().subscribe({
        next: (data) => {
          this.roomList = data;
        },
        error: (error) => {
          console.error('Error fetching hospital rooms:', error);
        }
      });
    }
  }

  scheduleOperationRequest() {
    this.schedulingBackend.algorithm = this.algorithm;

    // Collect roomNumbers from the selectedRoom array
    this.schedulingBackend.roomID = this.selectedRoom.map((room: any) => room.roomNumber);

    this.schedulingBackend.date = this.day.toString();
    this.opRequestsService.scheduleOperationRequest(this.schedulingBackend);
  }

  updateAlgorithmOptions() {
    this.isRoomRestricted = this.selectedRoom.length > 1;
    if (this.isRoomRestricted && this.algorithm !== 'genetic-room-distribution') {
      this.algorithm = 'genetic-room-distribution';
    }
  }
}
