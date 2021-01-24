import { Injectable, OnInit } from '@angular/core';
import { User } from '../Models/User';
import { Roles } from '../Models/Roles';
import { HttpService } from './http.service';
import sha256, { Hash, HMAC } from "fast-sha256";
import { Observable, of } from 'rxjs';
import {map} from 'rxjs/operators'
@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http:HttpService) { }

  user?:User=undefined;

  public GetUser():User|undefined{
    console.log(this.user);
    return this.user;
  }
  public Login(username:string,password:string):Observable<User>{ 
    let us=new User(username,"",Roles.Common,password);
    const Observable=this.http.Login(us);
    Observable.subscribe(user=>{this.user=user;});
    return Observable;
  }
  public Signup(username:string,password:string):Observable<User>{
    const Observable=this.http.Signup(new User(username,"",Roles.Common,password));
    Observable.subscribe(user=>{this.user=user;});
    return Observable;
  }
  public Logout(){
    this.http.Logout().subscribe();
    this.user=undefined;
  }
}
