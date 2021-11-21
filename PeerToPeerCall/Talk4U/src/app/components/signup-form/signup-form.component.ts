import { HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup } from '@angular/forms';
import { ApiResponse } from '../../models/api-response';
import { ToastService } from '../../services/alert/toast.service';
import { HttpService } from '../../services/http/http.service';

@Component({
  selector: 'app-signup-form',
  templateUrl: './signup-form.component.html',
  styleUrls: ['./signup-form.component.css']
})
export class SignupFormComponent {

  constructor(private _httpService: HttpService, private _toastService: ToastService) { }

  loading: boolean = false;

  signUpForm = new FormGroup({
    UserName: new FormControl(''),
    Email: new FormControl(''),
    Password: new FormControl(''),
    RepeatPassword: new FormControl('')
  });

  setFormControlValid(formControl: AbstractControl) {
    formControl.setErrors(null);
  }

  getErrorsFromFormControl(controlName: string): string {
    const abstractControl = this.signUpForm.controls[controlName];

    return abstractControl.getError('0') ? abstractControl.getError('0')[0] : this.setFormControlValid(abstractControl);
  }

  signUpUser() {
    let userCredentials = this.signUpForm.value;
    this.loading = true;

    this._httpService.post('/api/user/signup', userCredentials, new HttpHeaders({ 'Content-Type': 'application/json' }), false)
      .then((data: ApiResponse) => {
        this._toastService.toast('success', "Signed up successfully! Now sign in.");
      })
      .catch((error: HttpErrorResponse) => {
        const response: ApiResponse = error.error;

        Object.keys(this.signUpForm.controls).forEach(formControlName => {
          this.signUpForm.controls[formControlName].setErrors({ '0': response.data[formControlName] });
        });
      })
      .finally(() => {
        this.loading = false;
      });
  }

}
