import { FrontModule } from './front/front.module';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { OogstkaartModule } from './oogstkaart/oogstkaart.module';
import { HttpClientModule } from '@angular/common/http';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


const appRoutes: Routes = [
  {
    path: "",
    loadChildren: "./front/front.module#FrontModule"
  },
  {
    path: "oogstkaart",
    loadChildren: "./oogstkaart/oogstkaart.module#OogstkaartModule"
  },
  {
    path : "**",
    redirectTo: ""
  }


];

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule,
    FrontModule,
    BrowserAnimationsModule,
    OogstkaartModule,
    HttpClientModule,
    RouterModule.forRoot(appRoutes)    ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
