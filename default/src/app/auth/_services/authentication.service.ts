import { CreateUser } from './../_models/models';
import { Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { HttpHeaders, HttpParams, HttpClient } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Utils } from '../_helpers/Utils';
import { JWTToken } from '../_helpers/Models';
import { isUndefined } from 'util';
import { ForgotPassword } from '../_models';

@Injectable()
export class AuthenticationService {
    helper: JwtHelperService = new JwtHelperService();

    private connectlink = "connect/token";
    private client_id = "AngularSPA";
    private grant_type = "password";
    private scope = "WebAPI";

    constructor(private http: HttpClient, private router: Router) { }


    public login(username: string, password: string) {

        const body = new HttpParams()
            .set('username', username)
            .set('password', password)
            .set('scope', this.scope)
            .set('client_id', this.client_id)
            .set('grant_type', this.grant_type)


        return this.http.post<JWTToken>(Utils.getRoot().replace("/api", "") + this.connectlink, body.toString(), {
            headers: new HttpHeaders()
                .set('Content-Type', 'application/x-www-form-urlencoded')
            //.append("")
        })

    }

    public _role: string;
    public get role(): string {

        if (isUndefined(this._role)) {
            if (this.decodeToken().role === 'administrator') {
                this._role = 'administrator';
            }
            else {
                this._role = 'user';
            }
        }


        return this._role;
    }

    public saveToken(token: JWTToken) {
        localStorage.setItem('jwttoken', JSON.stringify(token));
    }

    public getToken(): JWTToken {
        if (this.hasToken()) {
            return JSON.parse(localStorage.getItem('jwttoken'));
        }
        return null;
    }

    public tokenExpired() {

        if (!this.hasToken()) {
            return true;
        }

        if (this.helper.isTokenExpired(this.getToken().access_token)) {
            this.removeToken();
            return true;
        }
        return false

    }

    public decodeToken() {
        return this.helper.decodeToken(this.getToken().access_token);
    }

    public hasToken() {
        if (localStorage.getItem("jwttoken") !== null) {
            return true
        }
        return false;
    }

    public removeToken() {
        localStorage.removeItem('jwttoken')
    }


    public set role(v: string) {
        this._role = v;
    }

    logout() {

        this.removeToken();
        this.http.post(Utils.getRoot() + 'general/signout', null).subscribe(res => {
            this.router.navigate(['/login']);

        }, err => {
            console.log("niet correct uitgelogd");
            this.router.navigate(['/login']);

        })
        
    }


    public register(user: CreateUser) {
        return this.http.post(Utils.getRoot() + 'identity/create', user);
    }

    public passwordrequest(mail: string) {
        return this.http.post(Utils.getRoot() + 'identity/forgotpassword?email=' + mail, null);
    }

    public passwordVeranderen(passwordview: ForgotPassword, userid: string, code : string) {
        return this.http.post(Utils.getRoot() + 'identity/ResetPassword?userid=' + userid + "&code=" + code, passwordview);
    }
}
