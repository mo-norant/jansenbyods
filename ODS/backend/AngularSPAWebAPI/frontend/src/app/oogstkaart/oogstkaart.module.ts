import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { OogstkaartService } from './oogstkaart.service';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShopComponent } from './shop/shop.component';
import { Route, RouterModule } from '@angular/router';
import { ItemComponent } from './item/item.component';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';

import {FormsModule} from '@angular/forms';
import { ContactComponent } from './contact/contact.component';
import { SuccesComponent } from './succes/succes.component'

const routes: Route[] = [
  {
    path: 'oogstkaart',
    component: ShopComponent,
    children: [

    ]
  },
  {
    path: 'oogstkaart/:id',
    component: ItemComponent,
  },
  {
    path: 'oogstkaart/contact/:id',
    component: ContactComponent,
  },
  {
    path: 'oogstkaart/succes',
    component: SuccesComponent,
  }
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    ProgressSpinnerModule,
    FormsModule,
    NgbModule.forRoot(),
  ],
  providers:[OogstkaartService],
  declarations: [ShopComponent, ItemComponent, ContactComponent, SuccesComponent],
  exports:[ShopComponent, ItemComponent, ContactComponent, SuccesComponent]
})
export class OogstkaartModule { }
