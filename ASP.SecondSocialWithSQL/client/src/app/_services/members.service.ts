import { Injectable } from '@angular/core';
import {enviroment} from "../../enviroments/enviroment";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Observable} from "rxjs";
import {Member} from "../_models/member";


const httpOptions = {
  headers: new HttpHeaders({
    Authorizations: "Bearer " + JSON.parse(localStorage.getItem('user')).token
  })
}
@Injectable({
  providedIn: 'root'
})
export class MembersService {
baseUrl = enviroment.apiUrl;
  constructor(private http: HttpClient) { }
  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users', httpOptions);
  }
  getMember(username: string) {
    return this.http.get<Member>(this.baseUrl + 'users' + username, httpOptions);
  }
}
