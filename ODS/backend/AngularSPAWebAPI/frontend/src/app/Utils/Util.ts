export class Utils{



static     devlink = 'http://localhost:55646/api/'
static     productionlink = 'http://jansenbyods.com/api/'
static     inDevelopment : boolean = false;

     static getRoot(): any {

        if(this.inDevelopment){

            return this.devlink
        }

        return this.productionlink;

        }



}
