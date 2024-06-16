import { Injectable } from '@angular/core';
import {enviroment} from "../../enviroments/enviroment";
import {ToastrService} from "ngx-toastr";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {User} from "../_models/user";
import {BehaviorSubject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
hubUrl = enviroment.hubUrl;
private hubConnection: HubConnection;
private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable()
  constructor(private toastr: ToastrService) { }
  createHubConnection(user: User) {
  this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl + 'presence', {
      accessTokenFactory: () => user.token
    })
    .withAutomaticReconnect()
    .build()

    this.hubConnection.start()
      .catch(error => console.log(error))
    this.hubConnection.on('UserIsOnline', username => {
      this.toastr.info(username + ' had connected');
    })
      this.hubConnection.on("UserIsOffline", username => {
        this.toastr.info(username + ' is offline');

    })
    this.hubConnection.on("GetOnlineUsers", (usernames: string[]) => {
      this.onlineUsersSource.next(usernames);
    })
    }
    stop() {
  this.hubConnection.stop().catch(error => console.log(error));
  }
}
