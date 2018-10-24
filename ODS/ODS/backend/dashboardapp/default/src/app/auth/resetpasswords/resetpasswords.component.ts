import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from '../_services';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-resetpasswords',
    templateUrl: './resetpasswords.component.html',
    styles: []
})
export class ResetpasswordsComponent implements OnInit {


    loading = false;

    email: string;
    form: FormGroup;
    code: string;
    userid: string;
    passwordregex = /^(?=.*\d)(?=.*[A-Z])(?!.*[^a-zA-Z0-9@#$^+=*])(.{8,15})$/;


    constructor(

        private _route: ActivatedRoute,
        private _authService: AuthenticationService,
        private router: Router,
        private fb: FormBuilder,
        private toastr: ToastrService

    ) {
    }

    ngOnInit() {
        this.loading = true;
        this.form = this.fb.group({
            password: ["", [Validators.required, Validators.pattern(this.passwordregex)]],
            password2: ["", [Validators.required, Validators.pattern(this.passwordregex)]],

        });

        this._route.queryParams.subscribe(res => {
            console.log(res["userid"]);
            console.log(res["code"]);

            this.userid = res["userid"];
            this.code = res["code"];
            this.loading = false;

            if (this.userid === undefined || this.code === undefined) {
                this.router.navigate(["login"])
            };

        }, err => {
            this.showSuccess("Geen correcte parameters om wachtwoord te veranderen.", "error");
            this.router.navigate(["login"]);
        });

    }


    changePasswords() {
        this.loading = true;
        if (this.form.value.password !== this.form.value.password2) {
            alert("wachtwoorden zijn niet gelijk aan elkaar");
            //velden terug initialiseren.
            this.form = this.form = this.fb.group({
                password: ["", [Validators.required, Validators.pattern(this.passwordregex)]],
                password2: ["", [Validators.required, Validators.pattern(this.passwordregex)]],

            });
            this.loading = false;
        }
        else {
            this._authService.passwordVeranderen(this.form.value, this.userid, this.code).subscribe(res => {
                this.loading = false;
                this.showSuccess("Wachtwoorden zijn aangepast", "success");
                this.router.navigate(['login']);

            }, err => {
                this.showSuccess("Wachtwoorden zijn niet aangepast", "danger");
                this.loading = false;
            });
        }

    }


    showSuccess(message: string, severity: string) {
        this.toastr.success(severity, message);
    }


}
