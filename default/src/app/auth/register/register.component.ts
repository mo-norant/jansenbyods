import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { Address } from "./../_models/models";
import { Component, OnInit } from "@angular/core";
import { CreateUser, Company } from "../_models/models";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { AuthenticationService } from "../_services";

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


    emailregex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    passwordregex = /^(?=.*\d)(?=.*[A-Z])(?!.*[^a-zA-Z0-9@#$^+=*])(.{8,15})$/;



    constructor(private fb: FormBuilder, private auth: AuthenticationService, private router: Router, private toastr: ToastrService) {
        this.user = new CreateUser();
        this.user.company = new Company();
        this.user.company.address = new Address();
    }

    ngOnInit() {
        this.registerform = this.fb.group({
            name: ["", Validators.required],
            email: ["", [Validators.required, Validators.pattern(this.emailregex)]],
            password: ["", [Validators.required, Validators.pattern(this.passwordregex)]],
            password2: ["", [Validators.required, Validators.pattern(this.passwordregex)]],
            companyname: ["", Validators.required],
            phone: ["", Validators.required],
            street: ["", Validators.required],
            number: ["",],
            zipcode: ["", Validators.required],
            country: ["", Validators.required],
            city: ["", Validators.required]
        });

        
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
            this.router.navigate(['/login/confirmmail'])
          
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
