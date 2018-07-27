export class Utils {



static     devlink = 'http://localhost:55646/api/' ;
static     productionlink = 'https://jansenbyods.com/api/' ;
static     inDevelopment = false;

     static getRoot(): any {

        if (this.inDevelopment) {

            return this.devlink;
        }

        return this.productionlink;

        }



}
