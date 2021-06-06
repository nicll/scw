import sha256 from "fast-sha256";
import {Roles } from "./Roles";
export class User{
    username:string;
    password:string;
    uid:string;
    role:Roles;
    constructor(username:string, uid:string, role:Roles, password:string){
        this.username=username;
        this.uid=uid;
        this.role=role;
        this.password=new TextDecoder("utf-8").decode(sha256(new TextEncoder().encode(password)));
    }

}