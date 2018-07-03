export class Utils{



static     devlink = 'http://localhost:5000/api/'
static     productionlink = 'http://jansenbyods.com/api/'
static     inDevelopment : boolean = false;

     static getRoot(): any {

        if(this.inDevelopment){

            return this.devlink
        }

        return this.productionlink;

        }



}
