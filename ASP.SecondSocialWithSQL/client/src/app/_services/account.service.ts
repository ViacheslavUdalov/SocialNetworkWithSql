import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {map, ReplaySubject} from "rxjs";
import {User} from "../_models/user";
import {enviroment} from "../../enviroments/enviroment";
import {PresenceService} from "./presence.service";

@Injectable({
  providedIn: 'root'
})
export class AccountService {
baseUrl = enviroment.apiUrl;
private currentUserSource = new ReplaySubject<User>(1);
currentUser$ = this.currentUserSource.asObservable();
  constructor(private http: HttpClient, private presence: PresenceService) { }
  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
        }
      })
    )
  };
  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
        }
      })
    )
  };
  setCurrentUser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }
  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presence.stop();
  }
  getDecodedToken(token) {
    return JSON.parse(atob(token.split('.')[1]));
  }
}
