import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {AccountService} from "../_services/account.service";
import {logMessages} from "@angular-devkit/build-angular/src/builders/browser-esbuild/esbuild";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{
  @Output() cancelRegister = new EventEmitter();
model: any = {};
constructor(private accountService: AccountService, private toastr: ToastrService) {
}
ngOnInit() {
}
register() {
  this.accountService.register(this.model).subscribe(response => {
    this.cancel();
    console.log(response);
  }, error =>  {
    console.log(error);
    this.toastr.error(error.error);
  })
};
cancel() {
this.cancelRegister.emit(false);
}
}
