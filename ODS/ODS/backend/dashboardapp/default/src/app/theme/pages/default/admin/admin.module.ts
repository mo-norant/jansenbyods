import { ProductconfiguratorService } from './admin-productconfigurator/productconfigurator.service';
import { FAQService } from './../../../../_services/FAQ.service';
import { QuestionComponent } from './question/question.component';
import { QuestionslistComponent } from './questionslist/questionslist.component';
import { ConfirmationService } from 'primeng/api';
import { AdminService } from './../../../../_services/admin.service';
import { RoleService } from './../../../../auth/_guards/role.service';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { LayoutModule } from '../../../layouts/layout.module';
import { DefaultComponent } from '../default.component';
import { AdminComponent } from './admin.component';
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
import { DropdownModule } from 'primeng/dropdown';
import { AdminRequestItemComponent } from './admin-request-item/admin-request-item.component';
import { ToastrService } from 'ngx-toastr';
import { AdminStatisticsService } from '../../../../_services/adminstatistics.service';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { UsermanagerdetailComponent } from './usermanager/usermanagerdetail/usermanagerdetail.component';
import { NgxEditorModule } from 'ngx-editor';
import { DialogModule } from 'primeng/dialog';
import { AdminProductconfiguratorComponent } from './admin-productconfigurator/admin-productconfigurator.component';
import { AdminSubproductFormcomponent } from './admin-productconfigurator/admin-subproductform/admin-subproductform';
import { FormComponent } from './admin-productconfigurator/admin-subproductform/form/form.component';
import { ProductcategoryComponent } from './admin-productconfigurator/productcategory/productcategory.component';
import { ScrollToModule } from '@nicky-lenaers/ngx-scroll-to';

const routes: Routes = [
    {
        path: '',
        component: DefaultComponent,
        canActivate: [RoleService],
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
            {
                path: 'usermanager/:id',
                component: UsermanagerdetailComponent,

            },
            {
                path: 'faq',
                component: QuestionslistComponent,

            },

            {
                path: 'faq/question',
                component: QuestionComponent,

            },
            {
                path: 'productmanager',
                component: AdminProductconfiguratorComponent,

            },
            {
                path: 'productmanager/subproducts',
                component: AdminSubproductFormcomponent
            },
            {
                path: 'productmanager/addsubcomponent',
                component: FormComponent
            },
            {
                path: 'productmanager/categories',
                component: ProductcategoryComponent
            }
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
        NgxChartsModule,
        NgxEditorModule,
        DialogModule,
        ScrollToModule.forRoot()
    ], exports: [
        RouterModule,
    ], declarations: [
        AdminComponent,
        AdminAanvragenComponent,
        AdminOogstkaartListComponent,
        UsermanagerComponent,
        AdminOogstkaartItemComponent,
        AdminRequestItemComponent,
        UsermanagerdetailComponent,
        QuestionslistComponent,
        QuestionComponent,
        AdminProductconfiguratorComponent,
        AdminSubproductFormcomponent,
        FormComponent,
        ProductcategoryComponent
    ],
    providers: [AdminService, ConfirmationService, ToastrService, AdminStatisticsService, FAQService, ProductconfiguratorService]
})
export class AdminModule {
}