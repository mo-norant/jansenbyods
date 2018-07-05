import { Request } from './../auth/_models/models';

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Utils } from '../auth/_helpers/Utils';

@Injectable()

export class AdminStatisticsService {

    link = 'Admin'


    constructor(private http: HttpClient) { }



    public GetRequestsFromPeriod(period : string){
        return this.http.get<Request[]>(Utils.getRoot() + this.link + "/requestsfromperiod?period="+period);
    }

}
