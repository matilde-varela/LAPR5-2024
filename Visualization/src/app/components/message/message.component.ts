import { Component } from '@angular/core';
import { MessageService } from '../../services/message.service';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';  // <-- Import CommonModule

@Component({
  selector: 'app-message',
  standalone: true,
  imports: [CommonModule, RouterLink],  // <-- Add CommonModule here
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css']
})
export class MessageComponent {
  constructor(public messageService: MessageService) {}
}
