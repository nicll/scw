import { Column } from "./Column";

export class Table{
    tableRefId?: string;
    displayName: string;
    type?: string;
    ownerUserId?: string;
    columns: Array<Column>;
    constructor(displayName:string, columns:Array<Column>, owner?:string,tableRef?:string,type?:string,){
        this.tableRefId=tableRef;
        this.displayName=displayName;
        this.type=type;
        this.ownerUserId=owner;
        this.columns=columns;
    }
}