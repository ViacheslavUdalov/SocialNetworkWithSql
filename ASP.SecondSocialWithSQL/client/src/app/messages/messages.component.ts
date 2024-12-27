import {Component, OnInit} from '@angular/core';
import {Pagination} from "../_models/pagination";
import {MessageService} from "../_services/message.service";
import {Message} from "../_models/message";
import {ConfirmService} from "../_services/confirm.service";

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit{
  messages : Message[];
  pagination : Pagination;
  container = "Unread";
  pageNumber = 1;
  pageSize = 5;
  loading = false;
  constructor(private messageService: MessageService, private confirmService: ConfirmService) {
  }
  ngOnInit(): void {
    this.loadMessages();
  }
loadMessages() {
    this.loading = true
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe((response) => {
        this.messages = response.result;
      console.log(response.result);
        this.pagination = response.pagination;
      this.loading = false
      }
    )
}
pageChange(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
}
deleteMessage(id: number) {
    this.confirmService.confirm('Удаление сообщения', 'Сообщение нельзя будет вернуть').subscribe(result => {
      if( result) {
        this.messageService.deleteMessage(id).subscribe(() => {
          this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
        })
      }
    })

}
}
