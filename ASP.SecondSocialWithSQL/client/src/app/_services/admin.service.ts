import { Injectable } from '@angular/core';
import {enviroment} from "../../enviroments/enviroment";
import {HttpClient} from "@angular/common/http";
import {User} from "../_models/user";

@Injectable({
  providedIn: 'root'
})
export class AdminService {
baseUrl = enviroment.apiUrl
  constructor(private http: HttpClient) { }
  getUsersWithRoles() {
  return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(username: string, roles: string[]) {
  return this.http.post(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles, {})
  }
}
