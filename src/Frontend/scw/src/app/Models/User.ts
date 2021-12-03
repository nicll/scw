import sha256 from "fast-sha256";
import {Roles } from "./Roles";
export class User{
    name:string;
    password:string;
    userId:string;
    role:Roles;
    constructor(name:string, userId:string, role:Roles, password:string){
        this.name=name;
        this.userId=userId;
        this.role=role;
        this.password=new TextDecoder("utf-8").decode(sha256(new TextEncoder().encode(password)));
    }


}
