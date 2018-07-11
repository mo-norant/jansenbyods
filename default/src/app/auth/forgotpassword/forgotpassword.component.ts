import { Validators } from '@angular/forms';
import { FormControl } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from '../_services';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-forgotpassword',
  templateUrl: './forgotpassword.component.html',
  styles: []
})
export class ForgotpasswordComponent implements OnInit {

  loading = false;

  email: string;
  form : FormGroup;
  errmessage;
  emailregex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;


  constructor(
 
      private _authService: AuthenticationService,
      private router: Router,
      private fb : FormBuilder,
      private toastr: ToastrService
     ) {
  }

  ngOnInit() {
   this.form = this.fb.group({
    email: ["", [Validators.required, Validators.pattern(this.emailregex)]],
  })
  }

  resetMail(){
    this.loading = true;
    this._authService.passwordrequest(this.form.value.email).subscribe(res => {
      this.showSuccess("Email verstuurd.");
      this.router.navigate(["login"]);
    }, err =>{
      if(err.status === 404){
          this.errmessage = "Account niet gevonden.";
          this.loading = false
      }
    });
  }


  showSuccess(message: string) {
    this.toastr.success('Succes', message);
}

}
