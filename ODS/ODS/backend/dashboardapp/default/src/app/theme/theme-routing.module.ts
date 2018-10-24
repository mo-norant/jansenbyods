import { NgModule } from '@angular/core';
import { ThemeComponent } from './theme.component';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../auth/_guards/auth.guard';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptorService } from '../_services/token-interceptor.service';

const routes: Routes = [
    {
        'path': '',
        'component': ThemeComponent,
       'canActivate': [AuthGuard],
        'children': [
            {
                'path': 'index',
                'loadChildren': '.\/pages\/default\/blank\/blank.module#BlankModule',
            },
            {
                'path': 'oogstkaart',
                'loadChildren': '.\/pages\/default\/oogstkaart\/oogstkaart.module#OogstkaartModule',
            },
            {
                'path': 'admin',
                'loadChildren': '.\/pages\/default\/admin\/admin.module#AdminModule',
            },
            {
                'path': '',
                'redirectTo': 'index',
                'pathMatch': 'full',
            },
        ],
    },
    {
        'path': '**',
        'redirectTo': 'index',
        'pathMatch': 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    providers: [{
        provide: HTTP_INTERCEPTORS,
        useClass: TokenInterceptorService,
        multi: true
    }],
    exports: [RouterModule],
})
export class ThemeRoutingModule { }