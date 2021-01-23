export class Table{
    tableRefId: any;
    displayName: string;
    type: string;
    ownerUserId: any;
    columns: any;
    constructor(tableRef:any,displayName:string, type:string, owner:any,columns:any){
        this.tableRefId=tableRef;
        this.displayName=displayName;
        this.type=type;
        this.ownerUserId=owner;
        this.columns=columns;
    }
}