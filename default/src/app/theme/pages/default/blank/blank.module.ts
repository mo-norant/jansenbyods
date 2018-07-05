import { UserguardService } from './../../../../auth/_guards/userguard.service';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { LayoutModule } from '../../../layouts/layout.module';
import { DefaultComponent } from '../default.component';
import { BlankComponent } from './blank.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptorService } from '../../../../_services/token-interceptor.service';
import { StatisticsService } from '../../../../_services/statistics.service';

const routes: Routes = [
    {
        path: '',
        component: DefaultComponent,
        canActivate: [UserguardService],
        children: [
            {
                'path': '',
                'component': BlankComponent,
            },
        ],
    },
];

@NgModule({
    imports: [
        CommonModule, RouterModule.forChild(routes), LayoutModule,
    ], exports: [
        RouterModule,
    ], declarations: [
        BlankComponent,
    ],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: TokenInterceptorService,
            multi: true
        },
        StatisticsService
    ]
})
export class BlankModule {
}