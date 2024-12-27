import {ActivatedRouteSnapshot, Resolve} from "@angular/router";
import {Member} from "../_models/member";
import {Injectable} from "@angular/core";
import {MembersService} from "../_services/members.service";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class MemberDetailedResolver implements Resolve<Member> {
  constructor(private memberService: MembersService) {
  }
  // специальная служба (service), которая предварительно загружает данные для компонента перед его отображением.
  resolve(route: ActivatedRouteSnapshot): Observable<Member> {
    return this.memberService.getMember(route.paramMap.get('username'))
  }
}
