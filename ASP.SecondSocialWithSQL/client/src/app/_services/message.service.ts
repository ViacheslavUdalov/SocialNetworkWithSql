import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {enviroment} from "../../enviroments/enviroment";
import {getPaginatedResult, getPaginationHeaders} from "./PaginationHelper";
import {Message} from "../_models/message";

@Injectable({
  providedIn: 'root'
})
export class MessageService {
baseUrl = enviroment.apiUrl;
  constructor(private http: HttpClient) { }
  getMessages(pageNumber, pageSize, container) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginatedResult<Message[]>(this.baseUrl + 'message', params, this.http)
  }
  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'message/thread/' + username);
  }
}

