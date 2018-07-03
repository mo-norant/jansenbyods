import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';

import { HomeComponent } from './home/home.component';
import { RouterModule, Route } from '@angular/router';
import { ContactComponent } from './contact/contact.component';
import { AboutusComponent } from './aboutus/aboutus.component';
import { NotfoundComponent } from './notfound/notfound.component';
import { FaqComponent } from './faq/faq.component';


const routes: Route[] = [
  {
    path: '',
    component: HomeComponent,
  },
  {
    path: 'contact',
    component: ContactComponent,
  },
  {
    path: 'over-ons',
    component: AboutusComponent,
  },
  {
    path: 'faq',
    component: FaqComponent,
  },

];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  declarations: [NavbarComponent, FooterComponent, HomeComponent, ContactComponent, AboutusComponent, NotfoundComponent, FaqComponent],
  exports: [NavbarComponent, FooterComponent, HomeComponent, ContactComponent, AboutusComponent, NotfoundComponent, FaqComponent]})
export class FrontModule { }
