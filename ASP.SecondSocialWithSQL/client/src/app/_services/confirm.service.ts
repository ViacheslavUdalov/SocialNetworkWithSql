import {Injectable} from '@angular/core';
import {BsModalRef, BsModalService} from "ngx-bootstrap/modal";
import {ConfirmDialogComponent} from "../modals/confirm-dialog/confirm-dialog.component";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModalrRef: BsModalRef

  constructor(private modalService: BsModalService) {
  }

  confirm(title = 'Confirmation', message = 'Вы хотите сделать это?', btnOkText = 'Ok', btnCancelText = 'Cancel'): Observable<boolean> {
    const config = {
      initialState: {
        title,
        message,
        btnOkText,
        btnCancelText

      }
    }
    this.bsModalrRef = this.modalService.show(ConfirmDialogComponent, config);
    return new Observable<boolean>(this.getResult());
  }

  private getResult() {
    return (observer) => {
      const subscription = this.bsModalrRef.onHidden.subscribe(() => {
        observer.next(this.bsModalrRef.content.result);
        observer.complete();
      });
      return {
        unsubscribe() {
          subscription.unsubscribe();
        }
      }
    }
  }
  }
