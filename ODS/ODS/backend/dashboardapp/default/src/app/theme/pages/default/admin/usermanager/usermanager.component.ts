import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../../../../_services/admin.service';

@Component({
    selector: 'app-usermanager',
    templateUrl: './usermanager.component.html',
    styles: [`
    .disabled {
        background-color: red !important;
        color: #ffffff !important;
    }

    .enabled {
        background-color: green !important;
        color: #ffffff !important;
    }
`
    ]
})
export class UsermanagerComponent implements OnInit {

    cols;
    users;
    loading: boolean;
    selecteduser;

    constructor(private adminservice: AdminService, private router: Router) {

        this.cols = [
            { field: "id", header: "UserID" },
            { field: "userName", header: "Gebruikersnaam" },
            { field: "createDate", header: "Aangemaakt op" },
        ];
    }

    ngOnInit() {
        this.loading = true;
        this.adminservice.GetAllUsers().subscribe(res => {
            this.users = res;
            this.loading = false;
        }, err => this.loading = false);
    }


    onRowSelect(event) {
        this.router.navigate(['admin/usermanager', event.data.id]);
    }




}
