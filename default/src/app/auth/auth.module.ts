import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BaseRequestOptions, HttpModule } from '@angular/http';
import { MockBackend } from '@angular/http/testing';

import { AuthRoutingModule } from './auth-routing.routing';
import { AuthComponent } from './auth.component';
import { AlertComponent } from './_directives/alert.component';
import { LogoutComponent } from './logout/logout.component';
import { AuthGuard } from './_guards/auth.guard';
import { AlertService } from './_services/alert.service';
import { AuthenticationService } from './_services/authentication.service';
import { UserService } from './_services/user.service';
import { RegisterComponent } from './register/register.component';
import { RegistersuccesComponent } from './register/registersucces/registersucces.component';
import { ConfirmmailComponent } from './register/confirmmail/confirmmail.component';
import { ForgotpasswordComponent } from './forgotpassword/forgotpassword.component';
import { ResetpasswordsComponent } from './resetpasswords/resetpasswords.component';
import { LockoutComponent } from './lockout/lockout.component';

@NgModule({
    declarations: [
        AuthComponent,
        AlertComponent,
        LogoutComponent,
        RegisterComponent,
        RegistersuccesComponent,
        ConfirmmailComponent,
        ForgotpasswordComponent,
        ResetpasswordsComponent,
        LockoutComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        HttpModule,
        AuthRoutingModule,
        FormsModule,
        ReactiveFormsModule
    ],
    providers: [
        AuthGuard,
        AlertService,
        UserService,
        BaseRequestOptions,
    ],
    entryComponents: [AlertComponent],
})

export class AuthModule {
}