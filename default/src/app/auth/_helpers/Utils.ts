export class Utils {

    //static devlink = 'http://10.211.55.3:45455/api/'
    static devlink = 'http://localhost:55646/api/'
    static productionlink = 'http://jansenbyods.com/api/'
    static inDevelopment: boolean = true;

    static getRoot(): any {

        if (this.inDevelopment) {
            return this.devlink
        }

        return this.productionlink;

    }



}