import { ResetpasswordsComponent } from './resetpasswords/resetpasswords.component';
import { ForgotpasswordComponent } from './forgotpassword/forgotpassword.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthComponent } from './auth.component';
import { RegisterComponent } from './register/register.component';
import { RegistersuccesComponent } from './register/registersucces/registersucces.component';
import { ConfirmmailComponent } from './register/confirmmail/confirmmail.component';

const routes: Routes = [
    { path: '', component: AuthComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'registersucces', component: RegistersuccesComponent },
    { path: 'confirmmail', component: ConfirmmailComponent},
    { path: 'forgotpassword', component: ForgotpasswordComponent},
    { path: 'resetpassword', component: ResetpasswordsComponent},

];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class AuthRoutingModule {
}