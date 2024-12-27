import { Injectable } from '@angular/core';
import {enviroment} from "../../enviroments/enviroment";
import {ToastrService} from "ngx-toastr";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {User} from "../_models/user";
import {BehaviorSubject, take} from "rxjs";
import {logMessages} from "@angular-devkit/build-angular/src/builders/browser-esbuild/esbuild";
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
hubUrl = enviroment.hubUrl;
private hubConnection: HubConnection;
private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();
  constructor(private toastr: ToastrService, private router: Router) { }

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
      this.toastr.info(username + ' has connected');
      this.onlineUsers$.pipe(take(1)).subscribe((usernames) => {
        this.onlineUsersSource.next([...usernames, username])
      })
    })

    this.hubConnection.on('UserIsOffline', username => {
      this.toastr.warning(username + ' has disconnected');
      this.onlineUsers$.pipe(take(1)).subscribe((usernames) => {
        this.onlineUsersSource.next([...usernames.filter(x => x !== username)])
      })
    })
    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
      this.onlineUsersSource.next(usernames);
    })
    this.hubConnection.on('NewMessageReceived', ({username, knownAs}) => {
      this.toastr.info(knownAs + 'Отправил вас сообщение')
        .onTap
        .pipe(take(1))
        .subscribe(() => this.router.navigateByUrl('/members/' + username + '?tab=3'))
    })
  }
  stop(){
  this.hubConnection.stop().catch(error => console.log(error))
  }
}
