import { Request } from './../../../../auth/_models/models';
import { AdminStatisticsService } from './../../../../_services/adminstatistics.service';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';

@Component({
    selector: 'app-admin',
    templateUrl: './admin.component.html',
    encapsulation: ViewEncapsulation.None,
})
export class AdminComponent implements OnInit {


    period: string;
    periodrequests: Request[];

    single = [
        {
            "name": "Germany",
            "value": 8940000
        },
        {
            "name": "USA",
            "value": 5000000
        },
        {
            "name": "France",
            "value": 7200000
        }
    ];

    view: any[] = [400, 400];

    // options
    showLegend = true;

    colorScheme = {
        domain: ['#5AA454', '#A10A28', '#C7B42C', '#AAAAAA']
    };

    // pie
    showLabels = true;
    explodeSlices = false;
    doughnut = false;

    constructor(private statisticsservice: AdminStatisticsService) {
    }

    ngOnInit() {

        this.GetRequestsFromPeriod("month");

    }


    GetRequestsFromPeriod(period: string) {
        this.statisticsservice.GetRequestsFromPeriod(period).subscribe(res => {
            this.periodrequests = res;
            console.log(this.periodrequests)
        });


    }

}