import { Column } from "./Column";

export class Table{
    tableRefId: string;
    displayName: string;
    type: string;
    ownerUserId: string;
    columns: Array<Column>;
    constructor(tableRef:string,displayName:string, type:string, owner:string,columns:Array<Column>){
        this.tableRefId=tableRef;
        this.displayName=displayName;
        this.type=type;
        this.ownerUserId=owner;
        this.columns=columns;
    }
}