import { Injectable } from '@angular/core';

@Injectable()
export class GeneralService {

    private role: string

    constructor() { }


    public getRole() {
        return this.role;
    }

    public setRole(role: string) {
        this.role = role;
    }

}
