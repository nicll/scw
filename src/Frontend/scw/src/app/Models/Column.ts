export class Column{
    position?:number;
    name:string;
    type:string
    nullable:boolean;
    constructor(name:string,type:string,nullable:boolean,position?:number) {
        this.name=name;
        this.type=type;
        this.nullable=nullable
    }
}