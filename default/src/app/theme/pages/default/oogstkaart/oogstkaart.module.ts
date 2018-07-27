import { ToastrService } from 'ngx-toastr';
import { TableModule } from "primeng/table";
import { CalendarModule } from "primeng/calendar";
import { ProgressBarModule } from "primeng/progressbar";
import { AgmCoreModule } from '@agm/core';
import { FileUploadModule } from 'primeng/fileupload';
import { KeyFilterModule } from 'primeng/keyfilter';
import {DropdownModule} from 'primeng/dropdown';

import { OogstkaartService } from "./../../../../_services/oogstkaart.service";
import { OogstkaartlistComponent } from "./oogstkaartlist/oogstkaartlist.component";
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { DefaultComponent } from "../default.component";
import { Routes, RouterModule } from "@angular/router";
import { LayoutModule } from "../../../layouts/layout.module";
import { OogstkaartformComponent } from "./oogstkaartform/oogstkaartform.component";
import { OogstkaartitemComponent } from "./oogstkaartitem/oogstkaartitem.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { GrowlModule } from 'primeng/growl';
import { Ng4GeoautocompleteModule } from 'ng4-geoautocomplete';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { MessageService } from "primeng/components/common/messageservice";
import { UserguardService } from "../../../../auth/_guards/userguard.service";
import { TokenInterceptorService } from "../../../../_services/token-interceptor.service";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { OogstkaartaanvragenComponent } from './oogstkaartaanvragen/oogstkaartaanvragen.component';
import { OogstkaartAanvraagItemComponent } from './oogstkaart-aanvraag-item/oogstkaart-aanvraag-item.component';


const routes: Routes = [
    {
        path: "",
        component: DefaultComponent,
        canActivate: [UserguardService],
        children: [
            {
                path: "",
                component: OogstkaartlistComponent
            },
            {
                path: "nieuwproduct",
                component: OogstkaartformComponent
            },
            {
                path: "aanvragen",
                component: OogstkaartaanvragenComponent
            },
            {
                path: "aanvragen/:id",
                component: OogstkaartAanvraagItemComponent
            },
            {
                path: ":id",
                component: OogstkaartitemComponent
            }
        ]
    }
];

@NgModule({
    imports: [
        CommonModule,
        RouterModule.forChild(routes),
        LayoutModule,
        TableModule,
        ProgressBarModule,
        CalendarModule,
        AgmCoreModule.forRoot({
            apiKey: 'AIzaSyC20RLiyVsvMLncki9JQdKuIpHdBdSXTY0',
            libraries: ["places"]
        }),
        FormsModule,
        ReactiveFormsModule,
        FileUploadModule,
        ConfirmDialogModule,
        GrowlModule,
        KeyFilterModule,
        DropdownModule,
        Ng4GeoautocompleteModule.forRoot(),


    ],
    providers: [OogstkaartService, ConfirmationService, MessageService, ToastrService, {
        provide: HTTP_INTERCEPTORS,
        useClass: TokenInterceptorService,
        multi: true
    }],
    exports: [RouterModule],
    declarations: [
        OogstkaartlistComponent,
        OogstkaartformComponent,
        OogstkaartitemComponent,
        OogstkaartaanvragenComponent,
        OogstkaartAanvraagItemComponent
    ]
})
export class OogstkaartModule { }