import {Roles } from "./Roles";
export class User{
    username:string;
    uid:string;
    role:Roles;
    constructor(username:string, uid:string, role:Roles){
        this.username=username;
        this.uid=uid;
        this.role=role;
    }
}