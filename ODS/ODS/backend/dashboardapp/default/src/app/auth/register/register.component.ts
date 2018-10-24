import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { Address } from "./../_models/models";
import { Component, OnInit } from "@angular/core";
import { CreateUser, Company } from "../_models/models";
import { FormGroup, FormBuilder, Validators, FormControl } from "@angular/forms";
import { AuthenticationService } from "../_services";
import { CustomValidators } from 'ngx-custom-validators';

@Component({
    selector: "app-register",
    templateUrl: "./register.component.html",
    styles: []
})
export class RegisterComponent implements OnInit {
    user: CreateUser;

    model: any = {};
    loading: boolean;

    registerform: FormGroup;


    emailregex : RegExp = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    passwordregex : RegExp = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$/

    emailexists : boolean;
    emailcorrect : boolean;

    constructor(private fb: FormBuilder, private auth: AuthenticationService, private router: Router, private toastr: ToastrService) {
        this.user = new CreateUser();
        this.user.company = new Company();
        this.user.company.address = new Address();
    }

    ngOnInit() {

        let password = new FormControl('', [Validators.required, Validators.pattern(this.passwordregex)]);
        let password2 = new FormControl('', CustomValidators.equalTo(password));

        this.registerform = this.fb.group({
            name: ["", Validators.required],
            email: ["", [Validators.required, Validators.pattern(this.emailregex)]],
            password: password,
            password2: password2,
            companyname: ["", Validators.required],
            phone: ["", Validators.required],
            street: ["", Validators.required],
            number: ["",],
            zipcode: ["", Validators.required],
            country: ["", Validators.required],
            city: ["", Validators.required]
        });

        this.registerform.get('email').valueChanges.subscribe(val => {
         
            this.emailexists = false;


            
            if(this.emailregex.test(val)) {
                this.checkEmail(val);
                this.emailcorrect = true;
            }
            else{
                this.emailcorrect = false;
            }


          });

          this.registerform.valueChanges.subscribe(val => {
         console.log(this.registerform.controls)
      });
       
    }

  
    


    checkEmail(username : string){
        
        this.auth.doesEmailExist(username).subscribe(
            res  => {
                this.emailexists = res;
             }, 
            err => {});

    }

    addAccount() {
        this.loading = true;
        this.user.name = this.registerform.value.name;
        this.user.email = this.registerform.value.email;
        this.user.password = this.registerform.value.password;
        this.user.password2 = this.registerform.value.password2;
        this.user.company.phone = this.registerform.value.phone;
        this.user.company.companyName = this.registerform.value.companyname;
        this.user.company.email = this.registerform.value.email;
        this.user.company.address.street = this.registerform.value.street;
        this.user.company.address.number = this.registerform.value.number;
        this.user.company.address.zipcode = this.registerform.value.zipcode;
        this.user.company.address.country = this.registerform.value.country;
        this.user.company.address.city = this.registerform.value.city;

        this.auth.register(this.user).subscribe(data => {
            this.loading = false;
            this.showSuccess('Uw account werd aangemaakt. Activeer je account via uw email.');
            this.router.navigate(['/login/confirmmail']);

        }, err => {
            this.showError("Fout");
            this.loading = false;
        }

        )

    }

    showSuccess(message: string) {
        this.toastr.success('Succes', message);
    }
    showError(message: string) {
        this.toastr.error('Fout', message);
    }
}
