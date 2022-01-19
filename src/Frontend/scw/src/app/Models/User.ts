import sha256 from "fast-sha256";
import {Roles } from "./Roles";
import {DataSet} from "./DataSet";
import {Table} from "./Table";
export class User{
    name:string;
    password:string;
    userId:string;
    role:Roles;
    ownedTables?:Table[];
    creationDate?:Date;
    constructor(name:string, userId:string, role:Roles, password:string, creationDate?:Date, ownedTables?:Table[]){
        this.name=name;
        this.userId=userId;
        this.role=role;
        this.creationDate=creationDate;
        this.ownedTables=ownedTables;
        this.password=new TextDecoder("utf-8").decode(sha256(new TextEncoder().encode(password)));
    }


}
