import { ConfirmationService } from 'primeng/api';
import { AdminService } from './../../../../_services/admin.service';
import { RoleService } from './../../../../auth/_guards/role.service';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { LayoutModule } from '../../../layouts/layout.module';
import { DefaultComponent } from '../default.component';
import {  AdminComponent } from './admin.component';
import { AdminOogstkaartListComponent } from './admin-oogstkaart-list/admin-oogstkaart-list.component';
import { AdminAanvragenComponent } from './admin-aanvragen/admin-aanvragen.component';
import { UsermanagerComponent } from './usermanager/usermanager.component';
import { TableModule } from 'primeng/table';
import { ProgressBarModule } from 'primeng/progressbar';
import { CalendarModule } from 'primeng/calendar';
import { AdminOogstkaartItemComponent } from './admin-oogstkaart-item/admin-oogstkaart-item.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { GrowlModule } from 'primeng/components/growl/growl';
import { ConfirmDialogModule } from 'primeng/components/confirmdialog/confirmdialog';
import {DropdownModule} from 'primeng/dropdown';
import { AdminRequestItemComponent } from './admin-request-item/admin-request-item.component';
import { ToastrService } from 'ngx-toastr';

const routes: Routes = [
    {
        path: '',
        component: DefaultComponent,
        canActivate :[RoleService],
        children: [
            {
                path: '',
                component: AdminComponent,
            },
            {
                path: 'aanvragen',
                component: AdminAanvragenComponent,
                
            },
            {
                path: 'aanvragen/:id',
                component: AdminRequestItemComponent,
                
            },
            {
                path: 'oogstkaart',
                component: AdminOogstkaartListComponent,
                
            },
            {
                path: 'oogstkaart/:id',
                component: AdminOogstkaartItemComponent,
                
            },
            {
                path: 'usermanager',
                component: UsermanagerComponent,
                
            },
        ],
    },
];

@NgModule({
    imports: [
        CommonModule, RouterModule.forChild(routes), LayoutModule,
        TableModule,
        ProgressBarModule,
        CalendarModule,
        FormsModule,
        ReactiveFormsModule,
        ConfirmDialogModule,
        GrowlModule,
        DropdownModule,
    ], exports: [
        RouterModule,
    ], declarations: [
        AdminComponent,
        AdminAanvragenComponent,
        AdminOogstkaartListComponent,
        UsermanagerComponent,
        AdminOogstkaartItemComponent,
        AdminRequestItemComponent 
       ],
        providers: [AdminService, ConfirmationService, ToastrService]
})
export class AdminModule {
}