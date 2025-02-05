import { NgModule } from '@angular/core';
import { MessageComponent } from '../../components/message/message.component';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@NgModule({
  declarations: [],
  imports: [CommonModule, RouterLink],
  exports: []  // Export MessageComponent so it can be used in other components
})
export class MessageModule {}
