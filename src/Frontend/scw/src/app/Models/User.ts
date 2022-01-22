import sha256 from "fast-sha256";
import {Roles } from "./Roles";
import {DataSet} from "./DataSet";
import {Table} from "./Table";
import {Log} from "./Log";
export class User{
    name:string;
    password:string;
    userId:string;
    role:Roles;
    ownedTables?:Table[];
    creationDate?:Date;
    logs?:Log[];
  /**
   * declares when the user last created a table
   */
  lastModifiedDate?:Date;
    constructor(name:string, userId:string, role:Roles, password:string, creationDate?:Date, ownedTables?:Table[], lastModifiedDate?:Date, logs?:Log[]){
        this.name=name;
        this.userId=userId;
        this.role=role;
        this.creationDate=creationDate;
        this.ownedTables=ownedTables;
        this.lastModifiedDate=lastModifiedDate;
        this.logs=logs;
        this.password=new TextDecoder("utf-8").decode(sha256(new TextEncoder().encode(password)));
    }


}
