import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { OogstKaartItem } from '../../../../auth/_models/models';
import { StatisticsService } from '../../../../_services/statistics.service';

@Component({
    selector: 'app-blank',
    templateUrl: './blank.component.html',
    encapsulation: ViewEncapsulation.None,
})
export class BlankComponent implements OnInit {


    mostpopularproduct: OogstKaartItem
    unreadreq: number;
    reviewaccepted: number;

    constructor(private statistics: StatisticsService) {
    }

    ngOnInit() {
        this.statistics.GetMostViewedProduct().subscribe(res => {
            this.mostpopularproduct = res;
        });

        this.statistics.RequestinReview().subscribe(res => {
            this.unreadreq = res;
        });
        this.statistics.requestaccepted().subscribe(res => {
            this.reviewaccepted = res;
        })
    }
}