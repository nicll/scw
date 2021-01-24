export class Column{
    position:number;
    name:string;
    type:string
    nullable:boolean;
    constructor(position:number,name:string,type:string,nullable:boolean) {
        this.position=position;
        this.name=name;
        this.type=type;
        this.nullable=nullable
    }
}