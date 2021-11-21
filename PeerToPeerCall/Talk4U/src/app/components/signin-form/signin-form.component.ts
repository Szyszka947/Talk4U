import { HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiResponse } from '../../models/api-response';
import { ToastService } from '../../services/alert/toast.service';
import { HttpService } from '../../services/http/http.service';

@Component({
  selector: 'app-signin-form',
  templateUrl: './signin-form.component.html',
  styleUrls: ['./signin-form.component.css']
})
export class SigninFormComponent {

  constructor(private _httpService: HttpService, private _toastService: ToastService, private router: Router) { }

  loading: boolean = false;

  signInForm = new FormGroup({
    UserName: new FormControl(''),
    Password: new FormControl('')
  });

  setFormControlValid(formControl: AbstractControl) {
    formControl.setErrors(null);
  }

  getErrorsFromFormControl(controlName: string): string {
    const abstractControl = this.signInForm.controls[controlName];

    return abstractControl.getError('0') ? abstractControl.getError('0')[0] : this.setFormControlValid(abstractControl);
  }

  signInUser() {
    let userCredentials = this.signInForm.value;
    this.loading = true;

    this._httpService.post('/api/user/signin', userCredentials, new HttpHeaders({ 'Content-Type': 'application/json' }), false)
      .then((data: ApiResponse) => {
        this._toastService.toast('success', "Welcome talker!");
        this.router.navigate(['/']);
      })
      .catch((error: HttpErrorResponse) => {
        const response: ApiResponse = error.error;

        Object.keys(this.signInForm.controls).forEach(formControlName => {
          this.signInForm.controls[formControlName].setErrors({ '0': response.data[formControlName] });
        });
      })
      .finally(() => {
        this.loading = false;
      });
  }

}
