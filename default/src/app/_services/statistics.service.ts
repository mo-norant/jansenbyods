
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OogstKaartItem } from '../auth/_models/models';
import { Utils } from '../auth/_helpers/Utils';

@Injectable()

export class StatisticsService {

    link = 'Statistics'


    constructor(private http: HttpClient) { }



    public GetMostViewedProduct() {
        return this.http.get<OogstKaartItem>(Utils.getRoot() + this.link + "/mostviewedproduct");
    }

    public RequestinReview() {
        return this.http.get<number>(Utils.getRoot() + this.link + "/requestinreview");
    }

    public requestaccepted() {
        return this.http.get<number>(Utils.getRoot() + this.link + "/reviewaccepted");
    }

}
