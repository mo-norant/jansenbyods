import { UserguardService } from './auth/_guards/userguard.service';
import { RoleService } from './auth/_guards/role.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { ThemeComponent } from "./theme/theme.component";
import { LayoutModule } from "./theme/layouts/layout.module";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { ScriptLoaderService } from "./_services/script-loader.service";
import { ThemeRoutingModule } from "./theme/theme-routing.module";
import { AuthModule } from "./auth/auth.module";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { AgmCoreModule } from "@agm/core";
import { GeneralService } from './_services/general.service';
import { AuthenticationService } from './auth/_services';
import { ToastrModule } from 'ngx-toastr';
import { TokenInterceptorService } from './_services/token-interceptor.service';
@NgModule({
    declarations: [ThemeComponent, AppComponent],
    imports: [
        HttpClientModule,
        LayoutModule,
        BrowserModule,
        BrowserAnimationsModule,
        AppRoutingModule,
        FormsModule,
        ReactiveFormsModule,
        ThemeRoutingModule,
        AuthModule,
        AgmCoreModule.forRoot({
            apiKey: "AIzaSyC20RLiyVsvMLncki9JQdKuIpHdBdSXTY0"
        }),
        ToastrModule.forRoot(),

    ],
    providers: [
        ScriptLoaderService,
        GeneralService,
        AuthenticationService,
        RoleService,
        UserguardService

    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
